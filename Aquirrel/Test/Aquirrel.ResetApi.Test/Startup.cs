using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Aquirrel.ResetApi.Test
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            var builder2 = new ConfigurationBuilder()
                 .SetBasePath(env.ContentRootPath)
                 .AddJsonFile("aquirrel.json", optional: true, reloadOnChange: true)
                 .AddJsonFile($"aquirrel.{env.EnvironmentName}.json", optional: true);

            if (env.IsEnvironment("Development"))
            {
                // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
                builder.AddApplicationInsightsSettings(developerMode: true);
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
            this.AquirrelConf = builder2.Build();
        }

        public IConfigurationRoot Configuration { get; }
        public IConfiguration AquirrelConf { get; }

        // This method gets called by the runtime. Use this method to add services to the container
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddApplicationInsightsTelemetry(Configuration);

            services.AddMvc();
            services.AddAquirrelTrace(AquirrelConf.GetSection("Aquirrel.Tracing"));
            services.AddRestApi();
            services.AddSingleton<Aquirrel.Tracing.IReportClient>(
                sp => new Service.ReportClient(
                    sp.GetService<ILogger<Service.ReportClient>>(),
                    sp,
                    new Tracing.Internal.TracingSetting(AquirrelConf.GetSection("Aquirrel.Tracing"))));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(AquirrelConf.GetSection("FileLogging"));
            loggerFactory.AddDebug();
            loggerFactory.AddFile(AquirrelConf.GetSection("FileLogging"));

            app.UseRestApiTrace();

            app.UseApplicationInsightsRequestTelemetry();

            app.UseApplicationInsightsExceptionTelemetry();

            app.UseMvc();

        }
    }
}
