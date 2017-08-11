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
        public static string SqlConnectionString = "server=.;database=test_ef_core;uid=sa;pwd=sasa;";
        IConfiguration appsettings;
        public Startup(IHostingEnvironment env)
        {
            appsettings = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
        }
        public IServiceProvider ConfigureServices(IServiceCollection serviceCollection)
        {
            Console.WriteLine("Startup   ConfigureServices");

            serviceCollection.AddEntityFrameworkSqlServer()
               .AddDbContext<TestDbContext>((sp, opt) =>
               {
                   opt.UseInternalServiceProvider(sp);

                   opt.UseSqlServer(Startup.SqlConnectionString, sqlOpt =>
                   {
                       sqlOpt.CommandTimeout(10);
                       sqlOpt.EnableRetryOnFailure(3, TimeSpan.FromSeconds(10), null);
                       sqlOpt.UseRowNumberForPaging(true);
                       sqlOpt.UseRelationalNulls(false);
                   });

                   opt.ConfigureEntityMappings(this.GetType().Assembly);

               })
               .AddAquirrelDb<TestDbContext>();


            return serviceCollection.BuildServiceProvider();
        }
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {

        }
    }
}
