﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

namespace Aquirrel.MQ.Internal
{
    public class EventBusSettings
    {
        IConfiguration _configuration;
        public IChangeToken ChangeToken { get; private set; }
        ILogger _logger;
        public EventBusSettings(IConfiguration configuration, ILogger logger)
        {
            _logger = logger;
            _configuration = configuration;
            this.ChangeToken = _configuration.GetReloadToken();


            List<EventBusHostOptions> hostsOptions = new List<EventBusHostOptions>();
            ConfigurationBinder.Bind(_configuration.GetSection("Hosts"), hostsOptions);
            this.Hosts = hostsOptions.ToDictionary(p => p.Name, p => p);

            List<ProductOptions> productOptions = new List<ProductOptions>();
            ConfigurationBinder.Bind(_configuration.GetSection("Products"), productOptions);
            this.Products = productOptions.ToDictionary(p => p.Id, p => p);

            EventBusConfigOptions configOptions = new EventBusConfigOptions();
            ConfigurationBinder.Bind(_configuration.GetSection("Options"), configOptions);
            this.Options = configOptions;


            _logger.LogInformation("init EventBus.Hosts：" + this.Hosts.ToJson());
            _logger.LogInformation("init EventBus.Products：" + this.Products.ToJson());
            _logger.LogInformation("init EventBus.Options：" + this.Options.ToJson());

        }


        public Dictionary<string, EventBusHostOptions> Hosts { get; set; }
        public Dictionary<string, ProductOptions> Products { get; set; }

        public EventBusConfigOptions Options { get; set; }

    }
    public class EventBusConfigOptions
    {
        public int ConnectionMaxChannelSize { get; set; } = 15;
    }
    public class EventBusHostOptions
    {
        public string Name { get; set; } = "default";
        public string Address { get; set; }
        public string UserName { get; set; } = "guest";
        public string UserPassword { get; set; } = "guest";
        public string Port { get; set; } = "5672";
        public string VHost { get; set; } = "/";
        public int Heartbeat { get; set; } = 60;
        public bool AutoRecovery { get; set; } = true;
    }

    public class ProductOptions
    {
        public string Id { get; set; }
        public string Host { get; set; } = "default";
    }
}
