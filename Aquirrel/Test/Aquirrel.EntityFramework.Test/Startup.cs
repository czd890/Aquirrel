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

            //    .AddAquirrelDb<TestDbContext>((_sp, op) =>
            //{
            //    op.ConfigureEntityMappings(this.GetType().GetTypeInfo().Assembly);
            //    op.ConfigureAutoEntityAssemblys(this.GetType().GetTypeInfo().Assembly);
            //    var sqlConnStr = appsettings.GetConnectionString("jiangzhi");
            //    Console.WriteLine(sqlConnStr);
            //    op.UseSqlServer(sqlConnStr, sqlop =>
            //    {
            //        //sqlop.EnableRetryOnFailure(3);
            //    });

            //}, ServiceLifetime.Scoped)

            services
                .AddEntityFrameworkSqlServer()
                .AddDbContext<TestDbContext>((sp3, optionBuilder) =>
                {
                    optionBuilder.UseSqlServer("server=172.16.100.172;database=efcoretest;uid=sa_test;pwd=123456;");
                }, ServiceLifetime.Scoped);


            services.AddAquirrelDb<TestDbContext>(sp2 =>
            {

            });


            return services.BuildServiceProvider();
        }
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {

        }
    }
}
