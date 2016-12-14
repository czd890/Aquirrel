using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Aquirrel;

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
            rules.Each(p => IdBuilder.Register(Type.GetType(p.Type), p));
            return services;
        }
    }
}
