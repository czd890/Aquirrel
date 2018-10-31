using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Aquirrel.EntityFramework.Sharding;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MySql.Data.EntityFrameworkCore;

namespace Aquirrel.EntityFramework.Test
{

    public class Startup_RV
    {
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            Console.WriteLine(DateTime.Now);
            var mysql = "Data Source=localhost;Port=3306;Database=test;User ID=root;Password=123456;CharSet=utf8;Allow User Variables=True";

            //IServiceCollection services = new ServiceCollection();

            //services.AddEntityFrameworkMySql();

            var appsettings = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json")
                   .Build();
            //services.AddEntityFrameworkSqlServer();
            services.AddDbContext<RVDbContext>((sp, op) =>
            {
                op.UseMySQL(mysql, mysqlOption =>
                {
                    mysqlOption.CommandTimeout(10);
                    mysqlOption.UseRelationalNulls(false);
                });
                op.UseAquirrelDb();
                op.ConfigureEntityMappings(typeof(RVDbContext).Assembly);
                op.ConfigureAutoEntityAssemblys(typeof(RVDbContext).Assembly);
                
            });
            Console.WriteLine("注入 ef 自定义替换服务");
            services.AddAquirrelDb();
            services.AddLogging(op =>
            {
                op.AddConfiguration(appsettings.GetSection("FileLogging"));
                op.AddFile(appsettings.GetSection("FileLogging"));
                op.SetMinimumLevel(LogLevel.Trace);
                op.AddConsole(cp => cp.IncludeScopes = true);
            });

            Console.WriteLine("ConfigureServices finish");
            return services.BuildServiceProvider();
        }
    }

    public class Startup
    {
        public static string SqlConnectionString = "server=.;database=test_ef_core;uid=sa;pwd=sasa;";

        public static string SqlConnectionString_Log = "server=.;database=log_ef_core;uid=sa;pwd=sasa;";

        public static string mysql = "Data Source=localhost;Port=3306;Database=test;User ID=root;Password=123456;CharSet=utf8;Allow User Variables=True";
        private readonly IConfiguration appsettings;
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

            //serviceCollection.AddEntityFrameworkSqlServer();

            serviceCollection.AddLogging(op =>
                {
                    op.AddConfiguration(this.appsettings.GetSection("FileLogging"));
                    op.AddDebug();
                    op.AddConsole(op2 => { op2.IncludeScopes = true; });
                    op.AddFile(this.appsettings.GetSection("FileLogging"));
                })

               .AddDbContext<TestDbContext>((sp, opt) =>
               {
                   //opt.UseInternalServiceProvider(sp);

                   //opt.UseSqlServer(Startup.SqlConnectionString, sqlOpt =>
                   //{
                   //    sqlOpt.CommandTimeout(10);
                   //    sqlOpt.EnableRetryOnFailure(3, TimeSpan.FromSeconds(10), null);
                   //    sqlOpt.UseRowNumberForPaging(true);
                   //    sqlOpt.UseRelationalNulls(false);
                   //});

                   opt.UseMySQL(Startup.mysql, mysqlOption =>
                   {
                       mysqlOption.CommandTimeout(10);
                       mysqlOption.UseRelationalNulls(false);
                   });
                   opt.UseAquirrelDb();
                   opt.ConfigureEntityMappings(this.GetType().Assembly);
                   opt.ConfigureAutoEntityAssemblys(this.GetType().Assembly);

               })
               //.AddDbContext<LogDbContext>((sp, opt) =>
               //{
               //    opt.UseInternalServiceProvider(sp);
               //    opt.UseSqlServer(Startup.SqlConnectionString_Log, sqlOpt =>
               //    {
               //        sqlOpt.CommandTimeout(10);
               //        sqlOpt.EnableRetryOnFailure(3, TimeSpan.FromSeconds(10), null);
               //        sqlOpt.UseRowNumberForPaging(true);
               //        sqlOpt.UseRelationalNulls(false);
               //    });
               //    opt.ConfigureEntityMappings(typeof(project.Entity.Log).Assembly);
               //    opt.ConfigureAutoEntityAssemblys(typeof(project.Entity.Log).Assembly);
               //})
               .AddAquirrelDb();
            //.AddAquirrelShardingDb<MyShardingFactory>();

            serviceCollection.AddScoped<project.Repository.ShardTableRepo>();


            return serviceCollection.BuildServiceProvider();
        }
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {

        }
    }

    internal class MyShardingFactory : Sharding.ShardingDbFactory
    {
        public MyShardingFactory(IServiceProvider provider) : base(provider)
        {
        }

        public override DbContextOptionsBuilder<TContext> BuilderDbContextOptions<TContext>(DbContextOptionsBuilder<TContext> builder,
            ShardingOptions options)
        {
            if (options.ShardingDbValue.IsNotNullOrEmpty())
            {
                if (typeof(LogDbContext).IsAssignableFrom(typeof(TContext)))
                {
                    var connStr = Startup.SqlConnectionString_Log.Replace("log_ef_core", "log_ef_core_" + options.ShardingDbValue);
                    builder.UseSqlServer(connStr);
                }
            }
            return builder;
        }
        public override string GetShardingTableName<TContext, TEntity>(ShardingOptions options)
        {
            if (options.ShardingTableValue.IsNotNullOrEmpty())
            {
                return options.ShardingTableValue;
            }
            return base.GetShardingTableName<TContext, TEntity>(options);
        }
    }
}
