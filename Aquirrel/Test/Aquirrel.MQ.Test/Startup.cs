using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Aquirrel.MQ.Test
{
    public class Startup
    {
        IConfiguration appsettings;
        IConfiguration mqsettings;
        public Startup(IHostingEnvironment env)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            appsettings = new ConfigurationBuilder()
            .SetBasePath(System.IO.Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).Build();

            mqsettings = new ConfigurationBuilder()
            .SetBasePath(System.IO.Directory.GetCurrentDirectory())
            .AddJsonFile("mqsettings.json", optional: true, reloadOnChange: true).Build();


            var sc = new ServiceCollection();

            sc.AddLogging();
            sc.AddEventBus(mqsettings);

            var sp = sc.BuildServiceProvider();

            sp.GetService<ILoggerFactory>()
                .AddDebug()
                .AddConsole(appsettings.GetSection("FileLogging"))
                .AddFile(appsettings.GetSection("FileLogging"));


            return sp;
        }
        //public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        //{

        //}
    }
}
