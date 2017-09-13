using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Aquirrel;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IdBuilderExtensions
    {
        public static IServiceCollection ConfigureIdBuilderRule(this IServiceCollection services, IConfiguration configuration)
        {
            List<IdBuilderRule> rules = new List<IdBuilderRule>();
            ConfigurationBinder.Bind(configuration, rules);
            return services.ConfigureIdBuilderRule(rules);
        }

        public static IServiceCollection ConfigureIdBuilderRule(this IServiceCollection services, IEnumerable<IdBuilderRule> rules)
        {
            foreach (var p in rules)
            {
                IdBuilder.Register(Type.GetType(p.Type), p);
            }
            return services;
        }
    }
}
