using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IO;
namespace Aquirrel.Logger.File.Test
{

    public class Program
    {
        static Startup startup;
        static IServiceProvider sp;
        internal static int C = 10;
        public static void Main(string[] args)
        {
            //Aquirrel.Logger.File.Internal.FileLogWrite.AddWriteProvider(new Internal.LoggerOptionsModel()
            //{
            //    CategoryName = "Aquirrel.Logger.File.Test.Program",
            //    FileDiretoryPath = "log\\Program",
            //    FileNameTemplate = "yyyyMMdd.log",
            //    MaxSize_Bytes = 1,
            //    MinLevel = LogLevel.Debug
            //}, options => { return new Aquirrel.Logger.File.Internal.FileLogWrite(options); });
            //FileLogger.FileFormatProvider = new Internal.FileFormatProvider();

            startup = new Startup(null);
            sp = startup.ConfigureServices(new ServiceCollection());
            DateTime now = DateTime.Now;
            var logger = sp.GetService<ILogger<Program>>();
            logger.LogInformation("begin");


            var t1 = Task.Run(() => { sp.GetService<TestService>().Do(); });
            var t2 = Task.Run(() => { sp.GetService<TestService2>().Do(); });
            Task.WaitAll(t1, t2);
            Console.WriteLine("任务完成！！！！！！" + (DateTime.Now - now));
            using (logger.BeginScope("begin scope:Id:221"))
            {
                sp.GetService<TestService>().Do();
                var ct = Task.Factory.StartNew(sp.GetService<TestService2>().Do).ContinueWith(p =>
                  {
                      logger.LogError(99, new Exception("this is exception"), "message");
                  });

                Task.WaitAll(ct);
            }

            sp.GetService<TestService>().Do();
            sp.GetService<TestService2>().Do();



            sp.GetService<ILogger<Microsoft.AspNetCore.Hosting.IHostingEnvironment>>().LogDebug("不记录");
            sp.GetService<ILogger<Microsoft.AspNetCore.Hosting.IHostingEnvironment>>().LogInformation("记录");
            logger.LogInformation("finish");
            Console.Read();

        }
    }
    public class TestService
    {
        public TestService(ILogger<TestService> logger)
        {
            _logger = logger;
        }
        ILogger _logger;
        public void Do()
        {
            using (_logger.BeginScope("scope child"))
            {

                for (int i = 0; i < Program.C; i++)
                {
                    _logger.LogInformation("A" + i.ToString());

                }
            }
        }
    }
    public class TestService2
    {
        public TestService2(ILogger<TestService2> logger)
        {
            _logger = logger;
        }
        ILogger _logger;
        public void Do()
        {
            using (_logger.BeginScope("scope child"))
            {
                for (int i = 0; i < Program.C; i++)
                {

                    _logger.LogInformation("B" + i.ToString());
                }

            }
        }
    }
}
