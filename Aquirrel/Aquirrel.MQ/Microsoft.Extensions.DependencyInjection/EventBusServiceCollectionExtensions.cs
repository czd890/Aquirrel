using Aquirrel.MQ;
using Aquirrel.MQ.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class EventBusServiceCollectionExtensions
    {
        public static IServiceCollection AddEventBus(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(sp => new EventBusSettings(configuration, sp.GetRequiredService<ILogger<EventBus>>()));
            services.AddSingleton<CacheManager>();
            services.AddSingleton<EventBus>();
            return services;
        }
    }
}
