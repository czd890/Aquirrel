using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Aquirrel.MQ.Internal;

namespace Aquirrel.MQ
{
    internal class EventBus : IEventBus
    {
        public EventBus(EventBusSettings settings, CacheManager cacheManager, ILogger<EventBus> logger)
        {
            _settings = settings;
            _CacheManager = cacheManager;
            _logger = logger;
        }
        EventBusSettings _settings;
        ILogger _logger;
        CacheManager _CacheManager;
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="productId"></param>
        /// <param name="topic">exchange</param>
        /// <param name="tag">routingKey</param>
        /// <param name="id"></param>
        /// <param name="message"></param>
        public void Publish<T>(string productId, string topic, string tag, string id, T message)
        {
            var t = typeof(T);
            string msgStr;
            var isJson = false;
            if (t == typeof(string))
            {
                isJson = true;
                msgStr = message.ToString();
            }
            else
                msgStr = message.ToJson<T>();

            var msg = Encoding.UTF8.GetBytes(msgStr);
            var channel = _CacheManager.GetChannel(productId);
            bool isLock = false;
            channel.SL.Enter(ref isLock);
            try
            {
                IBasicProperties props = channel.Channel.CreateBasicProperties();
                props.ContentType = isJson ? "application/json" : "text/plain";
                props.DeliveryMode = 2;

                channel.Channel.BasicPublish(topic, tag, false, props, msg);
            }
            catch
            {

                throw;
            }
            finally
            {
                if (isLock)
                    channel.SL.Exit();
            }
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
            options = options ?? SubscribeOptions.Default;
            var queueName = topic;
            var channel = _CacheManager.GetChannel(productId);

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
                _logger.LogError("eventbus.consumer.shutdown:" + ea.ToJson());
            };
            channel.Channel.BasicQos(0, (ushort)options.BasicQos, false);
            var tx = typeof(T) == typeof(string);
            consumer.Received += (obj, ea) =>
            {
                try
                {
                    var body = Encoding.UTF8.GetString(ea.Body);
                    var result = false;
                    if (tx)
                        result = action((T)((object)body));
                    else
                    {
                        var msg = body.ToJson<T>();
                        result = action(msg);
                    }
                    if (result)
                    {
                        consumer.Model.BasicAck(ea.DeliveryTag, true);
                    }
                    else
                    {
                        consumer.Model.BasicReject(ea.DeliveryTag, false);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(0, ex, "eventbus.subscribe.consumer");
                }
            };
            _logger.LogInformation("eventbus.subscribe." + queueName);
            channel.Channel.BasicConsume(queueName, false, consumer);
        }

        public void Exit()
        {
            ((IDisposable)_CacheManager).Dispose();
        }
    }
}
