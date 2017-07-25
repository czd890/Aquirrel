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

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AquirrelDbCollectionExtensions
    {
        public static IServiceCollection AddAquirrelDb<TDbConext>(this IServiceCollection services)
            where TDbConext : AquirrelDbContext
        {
            Console.WriteLine("AquirrelDbCollectionExtensions.AddAquirrelDb");

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            services.AddSingleton<IModelCustomizer, MyRelationalModelCustomizer>();

            services.AddSingleton<ICoreConventionSetBuilder, MyCoreConventionSetBuilder>();

            return services;
        }
    }
}
