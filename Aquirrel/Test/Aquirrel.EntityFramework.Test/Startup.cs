using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Reflection;

namespace Aquirrel.EntityFramework.Test
{
    public class Startup
    {
        IConfiguration appsettings;
        public Startup(IHostingEnvironment env)
        {
            appsettings = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
        }
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            Console.WriteLine("Startup   ConfigureServices");
            var sp = new ServiceCollection().AddAquirrelDb<TestDbContext>((_sp, op) =>
            {
                op.ConfigureEntityMappings(this.GetType().GetTypeInfo().Assembly);
                op.ConfigureAutoEntityAssemblys(this.GetType().GetTypeInfo().Assembly);
                var sqlConnStr = appsettings.GetConnectionString("jiangzhi");
                Console.WriteLine(sqlConnStr);
                op.UseSqlServer(sqlConnStr, sqlop =>
                {
                    //sqlop.EnableRetryOnFailure(3);
                });

            }, ServiceLifetime.Scoped)
                .BuildServiceProvider();

            return sp;
        }
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {

        }
    }
}
