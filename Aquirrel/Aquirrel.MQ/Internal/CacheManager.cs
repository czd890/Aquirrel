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
        class CacheItem
        {
            public string ProductId { get; set; }
            public List<CacheConnItem> ConnItems { get; set; } = new List<CacheConnItem>();
        }
        class CacheConnItem
        {
            public IConnection Connection { get; set; }
            public int ChannelSize => ChannelItems.Count;

            public Dictionary<string, CacheChannelItem> ChannelItems { get; set; } = new Dictionary<string, CacheChannelItem>();
        }
        class CacheChannelItem
        {
            public IModel Channel { get; set; }

            public DateTime LastGetTime { get; set; }
        }
        public CacheManager(EventBusSettings setting, ILogger<IEventBus> logger)
        {
            _settings = setting;
            _logger = logger;
        }
        Dictionary<string, CacheItem> _cacheConn = new Dictionary<string, CacheItem>();
        EventBusSettings _settings;
        ILogger<IEventBus> _logger;
        DateTime _lastCleanUpTime = DateTime.Now;
        public IConnection GetConnection(string productId)
        {
            if (this.disposedValue)
                throw new ObjectDisposedException(nameof(CacheManager));

            //if (!_cacheConn.ContainsKey(productId))
            //{
            //    if (!_settings.Products.ContainsKey(productId))
            //    {
            //        throw new Exception(productId + " product setting not found");
            //    }
            //    var hKey = _settings.Products[productId].Host;
            //    if (!_settings.Hosts.ContainsKey(hKey))
            //    {
            //        throw new Exception(hKey + " host setting not found");
            //    }
            //    _cacheConn[productId] = new CacheItem();
            //}
            var hKey = _settings.Products[productId].Host;
            var option = _settings.Hosts[hKey];
            var factory = new RabbitMQ.Client.ConnectionFactory();
            factory.HostName = option.Address;
            factory.UserName = option.UserName;
            factory.Password = option.UserPassword;
            factory.VirtualHost = option.VHost;
            factory.RequestedHeartbeat = (ushort)option.Heartbeat;
            factory.AutomaticRecoveryEnabled = option.AutoRecovery;
            //factory.ClientProperties["customerName"] = "aaaaaaa";
            return factory.CreateConnection();
        }
        public IModel GetChannel(string productId, string topic)
        {

            if (this.disposedValue)
                throw new ObjectDisposedException(nameof(CacheManager));

            var cacheKey = $"{productId}-{topic}";

            if (_cacheConn.ContainsKey(productId))
            {
                foreach (var item in _cacheConn[productId].ConnItems)
                {
                    if (item.ChannelItems.ContainsKey(cacheKey))
                    {
                        item.ChannelItems[cacheKey].LastGetTime = DateTime.Now;

                        if (this._lastCleanUpTime.AddMinutes(15) < DateTime.Now)
                            this.CleanIdleConn();

                        return item.ChannelItems[cacheKey].Channel;
                    }
                }
            }


            lock (this)
            {
                if (!_cacheConn.ContainsKey(productId))
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
                    _cacheConn[productId] = new CacheItem();
                }

                Lable_GetModel:
                bool hasGet = false;
                IModel _channel = null;
                foreach (var item in _cacheConn[productId].ConnItems)
                {
                    if (item.ChannelSize < this._settings.Options.ConnectionMaxChannelSize)
                    {
                        _channel = item.Connection.CreateModel();
                        item.ChannelItems.Add(cacheKey, new CacheChannelItem()
                        {
                            Channel = _channel,
                            LastGetTime = DateTime.Now
                        });
                        hasGet = true;
                    }
                }
                if (!hasGet)
                {
                    _cacheConn[productId].ConnItems.Add(new CacheConnItem()
                    {
                        Connection = this.GetConnection(productId)
                    });
                    goto Lable_GetModel;
                }

                _channel.BasicReturn += (obj, ea) =>
                {
                    _logger.LogError($"{productId} BasicReturn.{ea.Exchange},{ea.RoutingKey},{ea.ReplyCode},{ea.ReplyText},{Encoding.UTF8.GetString(ea.Body)}");
                };
                _channel.CallbackException += (obj, ea) =>
                {
                    _logger.LogError($"{productId} CallbackException.{ea.Detail?.ToJson()}{Environment.NewLine}{ea.Exception?.ToString()}");
                };

                _channel.FlowControl += (obj, ea) =>
                {
                    _logger.LogError($"{productId} FlowControl.{ea.Active}");
                };

                _channel.ModelShutdown += (obj, ea) =>
                {
                    _logger.LogError($"{productId} ModelShutdown.{ea.ToJson()}");
                };
                return _channel;
            }
        }

        void CleanIdleConn()
        {
            Task.Run(() => this.CleanTask());
        }
        void CleanTask()
        {
            if (this._lastCleanUpTime.AddMinutes(15) < DateTime.Now)
            {
                this._lastCleanUpTime = DateTime.Now;

                lock (this)
                {
                    foreach (var item in this._cacheConn)
                    {
                        List<CacheConnItem> _cleanConn = new List<CacheConnItem>();
                        foreach (var conn in item.Value.ConnItems)
                        {
                            List<string> _clean = new List<string>();
                            foreach (var channel in conn.ChannelItems)
                            {
                                if (channel.Value.LastGetTime.AddMinutes(30) < DateTime.Now)
                                {
                                    channel.Value.Channel.Close();
                                    channel.Value.Channel.Dispose();
                                    _clean.Add(channel.Key);
                                }
                            }
                            _clean.Each(_k => conn.ChannelItems.Remove(_k));
                            if (conn.ChannelSize == 0)
                            {
                                conn.Connection.Close();
                                conn.Connection.Dispose();
                                _cleanConn.Add(conn);
                            }
                        }
                        _cleanConn.Each(_ck => item.Value.ConnItems.Remove(_ck));
                    }
                }
            }
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
                        foreach (var item in this._cacheConn)
                        {
                            foreach (var conn in item.Value.ConnItems)
                            {
                                foreach (var channel in conn.ChannelItems)
                                {
                                    try
                                    {
                                        channel.Value.Channel.Close();
                                        channel.Value.Channel.Dispose();
                                    }
                                    catch { }
                                }
                                try
                                {
                                    conn.Connection.Close();
                                    conn.Connection.Dispose();
                                }
                                catch { }
                            }
                        }

                    }
                    catch { }
                    this._cacheConn.Clear();
                }
                disposedValue = true;
            }
        }
        void IDisposable.Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
