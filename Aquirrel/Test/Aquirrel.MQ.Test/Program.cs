using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IO;
using Aquirrel.MQ;

namespace Aquirrel.MQ.Test
{

    public class Program
    {
        static Startup startup;
        static IServiceProvider sp;
        internal static int C = 10;
        public static void Main(string[] args)
        {


            startup = new Startup(null);
            sp = startup.ConfigureServices(new ServiceCollection());
            var eventBus = sp.GetService<IEventBus>();
            var logger = sp.GetService<ILogger<Program>>();

            eventBus.Subscribe<string>("p3", "guangbo", str =>
            {
                logger.LogDebug($"{DateTime.Now} 收到广播消息：{str}");
                return true;
            }, new SubscribeOptions() { Model = MessageModel.Broadcasting });

            eventBus.Subscribe<string>("p2", "queue_1", str =>
            {
                logger.LogDebug($"{DateTime.Now} 收到集群消费消息：{str}");
                return true;
            });

            //Task.Run(() =>
            //{
            //    while (true)
            //    {
            //        Console.WriteLine($"500毫秒后发送消息");
            //        Thread.Sleep(500);
            //        eventBus.Publish("p2", "amq.topic", "route.order.created", "", DateTime.Now.ToString());
            //        eventBus.Publish("p2", "amq.topic", "PPP", "", DateTime.Now.ToString());

            //        eventBus.Publish("p3", "guangbo", "", Guid.NewGuid().ToString("N"), DateTime.Now.ToString());
            //    }
            //});
            Console.WriteLine("订阅完成");
            Console.ReadLine();
        }
    }
}
