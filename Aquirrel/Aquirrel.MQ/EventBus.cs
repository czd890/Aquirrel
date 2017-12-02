using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Aquirrel.MQ.Internal;

namespace Aquirrel.MQ
{
    internal class EventBus : IEventBus, IMQ
    {
        public EventBus(EventBusSettings settings, CacheManager cacheManager, ILogger<IEventBus> logger)
        {
            _settings = settings;
            _CacheManager = cacheManager;
            _logger = logger;
        }
        EventBusSettings _settings;
        ILogger _logger;
        CacheManager _CacheManager;
        static int[] retryInterval = new[] { 0, 10, 20, 40 };
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="productId"></param>
        /// <param name="topic">exchange</param>
        /// <param name="tag">routingKey</param>
        /// <param name="id"></param>
        /// <param name="message"></param>
        public void Publish<T>(string productId, string topic, string tag, string id, T message, PublishOptions options = null)
        {
            options = options ?? PublishOptions.Default;
            var t = typeof(T);
            string msgStr;
            var isJson = true;
            if (t == typeof(string))
            {
                isJson = false;
                msgStr = message.ToString();
            }
            else
                msgStr = message.ToJson<T>();

            var msg = Encoding.UTF8.GetBytes(msgStr);
            var channel = _CacheManager.GetChannel(productId, $"{productId}-{topic}-{tag}", options.ShardingConn);
            IBasicProperties props = channel.Channel.CreateBasicProperties();
            props.ContentType = isJson ? "application/json" : "text/plain";
            props.DeliveryMode = 2;
            props.Headers = new Dictionary<string, object>() { { "mid", id } };
            this.Publish_internal(productId, topic, tag, id, msg, props, channel.Channel);
        }
        void Publish_internal(string productId, string topic, string tag, string id, byte[] body, IBasicProperties basicProperties, IModel channel)
        {
            using (this._logger.BeginScope($"event bus pub {id}"))
            {
                var _exec = Aquirrel.FailureRetry.FailureRetryBuilder.Bind(() =>
                {
                    lock (channel)
                    {
                        channel.ConfirmSelect();
                        channel.BasicPublish(topic, tag, true, basicProperties, body);
                        channel.WaitForConfirmsOrDie();
                    }
                });

                _exec.RetryCount(3)
                 .RetryFilter(ex =>
                 {
                     _exec.RetryInterval((oldInterval, invCount) => retryInterval[invCount - 1]);
                     this._logger.LogError(ex, ex.Message);
                     return true;
                 })
                 .Failure(ex =>
                 {
                     this._logger.LogError($"event bus publish retry error {ex.RetryCount}.{Environment.NewLine}{productId}-{topic}-{tag};{Encoding.UTF8.GetString(body)}");
                 });

                _exec.Execute();
            }
        }
        public void Publish(string productId, string topic, string tag, string id, byte[] body, IBasicProperties basicProperties, PublishOptions options = null)
        {
            options = options ?? PublishOptions.Default;
            var channel = _CacheManager.GetChannel(productId, $"{productId}-{topic}-{tag}", options.ShardingConn);

            this.Publish_internal(productId, topic, tag, id, body, basicProperties, channel.Channel);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="productId"></param>
        /// <param name="topic">集群消费：queuename;广播消费:exchange</param>
        /// <param name="action"></param>
        /// <param name="options"></param>
        public async void Subscribe<T>(string productId, string topic, Func<T, bool> action, SubscribeOptions options = null)
        {
            this.Subscribe_internal<T>(productId, topic, true, (body, message) => action(body), options);
        }
        public async void Subscribe_internal<T>(string productId, string topic, bool hasReloveBody, Func<T, BasicDeliverEventArgs, bool> action, SubscribeOptions options = null)
        {
            options = options ?? SubscribeOptions.Default;
            var queueName = topic;
            var channel = _CacheManager.GetChannel(productId, $"{productId}-{topic}", options.ShardingConn);

            if (options.Model == MessageModel.Broadcasting)
            {
                var hostName = System.Net.Dns.GetHostName();
                var ipaddress = await System.Net.Dns.GetHostEntryAsync(hostName).ConfigureAwait(false);
                var ip = ipaddress.AddressList.FirstOrDefault(p => p.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)?.ToString() ?? hostName;
                var fanoutQueueName = ip + "." + productId + "." + topic + "." + DateTime.UtcNow.Ticks;
                channel.Channel.QueueDeclare(queue: fanoutQueueName);
                channel.Channel.QueueBind(fanoutQueueName, topic, "", null);

                queueName = fanoutQueueName;
            }

            var consumer = new EventingBasicConsumer(channel.Channel);
            consumer.Shutdown += (obj, ea) =>
            {
                _logger.LogError($"eventbus.consumer.shutdown;{Environment.NewLine}{productId}-{topic}-{ea.ToJson()}");
            };
            channel.Channel.BasicQos(0, (ushort)options.BasicQos, false);
            var tx = typeof(T) == typeof(string);
            consumer.Received += (obj, ea) =>
            {
                var isSuccess = false;
                try
                {
                    if (hasReloveBody)
                    {
                        var body = Encoding.UTF8.GetString(ea.Body);
                        if (tx)
                            isSuccess = action((T)((object)body), ea);
                        else
                        {
                            var msg = body.ToJson<T>();
                            isSuccess = action(msg, ea);
                        }
                    }
                    else
                    {
                        isSuccess = action(default(T), ea);
                    }


                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"eventbus.subscribe.consumer.exception;{Environment.NewLine}{productId}-{topic}-" +
                        $"{{RoutingKey:{ea.RoutingKey},ConsumerTag:{ea.ConsumerTag},DeliveryTag:{ea.DeliveryTag},Exchange:{ea.Exchange}}}");
                }
                finally
                {
                    try
                    {
                        if (isSuccess)
                        {
                            consumer.Model.BasicAck(ea.DeliveryTag, false);
                        }
                        else
                        {
                            consumer.Model.BasicNack(ea.DeliveryTag, false, options.FailMesaageReQueue);
                        }
                    }
                    catch { }
                }
            };
            _logger.LogInformation($"eventbus.subscribe;{productId}-{queueName}");
            channel.Channel.BasicConsume(queueName, false, consumer);
        }

        public void Exit()
        {
            ((IDisposable)_CacheManager).Dispose();
        }

        public void Subscribe(string productId, string topic, Func<BasicDeliverEventArgs, bool> action, SubscribeOptions options = null)
        {
            this.Subscribe_internal<object>(productId, topic, false, (body, message) => action(message), options);
        }
    }
}
