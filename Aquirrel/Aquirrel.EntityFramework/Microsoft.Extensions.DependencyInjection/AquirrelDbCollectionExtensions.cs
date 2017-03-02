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
        public static IServiceCollection AddAquirrelDb<TDbConext>(this IServiceCollection services,
            Action<IServiceProvider, DbContextOptionsBuilder> optionsAction = null,
            ServiceLifetime contextLifetime = ServiceLifetime.Scoped)
            where TDbConext : AquirrelDbContext
        {
            services.AddEntityFramework()
                .AddEntityFrameworkSqlServer()
                .AddSingleton<SqlServerModelSource, AquirrelDbModelSource>()
                .AddDbContext<TDbConext>(optionsAction, contextLifetime);

            return services;
        }
    }
}
