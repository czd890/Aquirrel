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

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AquirrelDbCollectionExtensions
    {
        public static IServiceCollection AddAquirrelDb<TDbConext>(this IServiceCollection services, Action<IServiceCollection> optionsAction)
            where TDbConext : AquirrelDbContext
        {
            Console.WriteLine("AquirrelDbCollectionExtensions AddAquirrelDb");
            //services.AddEntityFramework()
            //    .AddEntityFrameworkSqlServer()
            //    .AddSingleton<SqlServerModelSource, AquirrelDbModelSource>()
            //    .AddDbContext<TDbConext>((sp, options) =>
            //    {
            //        Console.WriteLine("UseInternalServiceProvider");
            //        options.UseInternalServiceProvider(sp);

            //        optionsAction?.Invoke(sp, options);
            //    }, contextLifetime);

            optionsAction(services);
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            return services;
        }
    }
}
