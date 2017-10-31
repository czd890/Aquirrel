using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace Aquirrel.MQ.Internal
{
    internal class CacheManager : IDisposable
    {

        public CacheManager(EventBusSettings setting, ILogger<IEventBus> logger)
        {
            _settings = setting;
            _logger = logger;
        }
        Dictionary<string, RabbitMQ.Client.IConnection> rabittmqConn = new Dictionary<string, RabbitMQ.Client.IConnection>();
        Dictionary<string, ChannelModel> rabittmqChannel = new Dictionary<string, ChannelModel>();
        EventBusSettings _settings;
        ILogger<IEventBus> _logger;

        public IConnection GetConnection(string productId,string shardingKey)
        {
            if (this.disposedValue)
                throw new ObjectDisposedException(nameof(CacheManager));
            if (!rabittmqConn.ContainsKey(shardingKey))
            {
                if (!_settings.Products.ContainsKey(productId))
                {
                    throw new Exception(productId + " product setting not found");
                }
                var hKey = _settings.Products[productId].Host;
                if (!_settings.Hosts.ContainsKey(hKey))
                {
                    throw new Exception(hKey + " host setting not found");
                }
                var option = _settings.Hosts[hKey];
                var factory = new RabbitMQ.Client.ConnectionFactory();
                factory.HostName = option.Address;
                factory.UserName = option.UserName;
                factory.Password = option.UserPassword;
                factory.VirtualHost = option.VHost;
                factory.RequestedHeartbeat = (ushort)option.Heartbeat;
                factory.AutomaticRecoveryEnabled = option.AutoRecovery;
                rabittmqConn[shardingKey] = factory.CreateConnection();
            }


            return rabittmqConn[shardingKey];
        }

        public struct ChannelModel
        {
            public IModel Channel { get; set; }
        }
        public ChannelModel GetChannel(string productId, string shardingKey, bool shardingConn)
        {

            if (this.disposedValue)
                throw new ObjectDisposedException(nameof(CacheManager));
            //return GetConnection(productId).CreateModel();
            if (!rabittmqChannel.ContainsKey(shardingKey))
            {
                lock (rabittmqChannel)
                {
                    if (!rabittmqChannel.ContainsKey(shardingKey))
                    {
                        var conn = GetConnection(productId, shardingKey);

                        rabittmqChannel[shardingKey] = new ChannelModel()
                        {
                            Channel = conn.CreateModel()
                        };
                        rabittmqChannel[shardingKey].Channel.BasicReturn += (obj, ea) =>
                        {
                            _logger.LogError($"{productId} BasicReturn.{ea.Exchange},{ea.RoutingKey},{ea.ReplyCode},{ea.ReplyText},{Encoding.UTF8.GetString(ea.Body)}");
                        };
                        //rabittmqChannel[productId].Channel.BasicAcks += (obj, ea) =>
                        //{

                        //};

                        //rabittmqChannel[productId].Channel.BasicNacks += (obj, ea) =>
                        //{

                        //};

                        //rabittmqChannel[productId].Channel.BasicRecoverOk += (obj, ea) =>
                        //{

                        //};

                        rabittmqChannel[shardingKey].Channel.CallbackException += (obj, ea) =>
                        {
                            _logger.LogError($"{productId} CallbackException.{ea.Detail?.ToJson()}{Environment.NewLine}{ea.Exception?.ToString()}");
                        };

                        rabittmqChannel[shardingKey].Channel.FlowControl += (obj, ea) =>
                        {
                            _logger.LogError($"{productId} FlowControl.{ea.Active}");
                        };

                        rabittmqChannel[shardingKey].Channel.ModelShutdown += (obj, ea) =>
                        {
                            _logger.LogError($"{productId} ModelShutdown.{ea.ToJson()}");
                        };
                    }
                }

            }
            return rabittmqChannel[shardingKey];
        }

        #region IDisposable Support
        private bool disposedValue = false; // 要检测冗余调用

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)。


                    try
                    {
                        foreach (var item in rabittmqChannel)
                        {
                            try
                            {
                                item.Value.Channel.Close();
                                item.Value.Channel.Dispose();
                            }
                            catch (Exception)
                            {

                                throw;
                            }
                        }
                        foreach (var item in rabittmqConn)
                        {
                            try
                            {
                                item.Value.Close();
                                item.Value.Dispose();
                            }
                            catch (Exception)
                            {

                                throw;
                            }
                        }
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                    rabittmqChannel.Clear();
                    rabittmqConn.Clear();

                }

                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。

                disposedValue = true;
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        // ~CacheManager() {
        //   // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
        //   Dispose(false);
        // }

        // 添加此代码以正确实现可处置模式。
        void IDisposable.Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
