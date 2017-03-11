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
            var sp = new ServiceCollection().AddAquirrelDb<TestDbContext>((_sp, op) =>
            {
                op.UseSqlServer(appsettings.GetConnectionString("jiangzhi"), sqlop =>
                {
                    sqlop.EnableRetryOnFailure(3);
                }).EnableSensitiveDataLogging();

            }, ServiceLifetime.Scoped)
                .BuildServiceProvider();

            return sp;
        }
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {

        }
    }
}
