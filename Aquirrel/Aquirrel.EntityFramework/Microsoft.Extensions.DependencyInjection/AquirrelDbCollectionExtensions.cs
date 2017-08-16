using Aquirrel.EntityFramework;
using Aquirrel.EntityFramework.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;
using Aquirrel.EntityFramework.Sharding;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AquirrelDbCollectionExtensions
    {
        public static IServiceCollection AddAquirrelDb(this IServiceCollection services)
        {
            Console.WriteLine("AquirrelDbCollectionExtensions.AddAquirrelDb");
            services.AddScoped(typeof(Repository<,>));

            services.AddScoped(typeof(UnitOfWork<>));

            services.AddSingleton<IModelCustomizer, MyRelationalModelCustomizer>();

            services.AddSingleton<ICoreConventionSetBuilder, MyCoreConventionSetBuilder>();

            services.AddScoped<InternalScopeServiceContainer>();

            return services;
        }

        public static IServiceCollection AddAquirrelShardingDb<IShardingDbFactory>(this IServiceCollection services)
            where IShardingDbFactory : ShardingDbFactory
        {
            services.AddSingleton<ShardingDbFactory, IShardingDbFactory>();
            return services;
        }


    }
}
