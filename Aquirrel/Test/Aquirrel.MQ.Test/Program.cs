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
            logger.LogDebug("deug");
            logger.LogInformation("aaaaa");
            //eventBus.Subscribe<string>("defualt", "guangbo", str =>
            //{
            //    logger.LogDebug($"{DateTime.Now} 收到广播消息：{str}");
            //    return true;
            //}, new SubscribeOptions() { Model = MessageModel.Broadcasting });

            //eventBus.Subscribe<string>("defualt", "product_user_async_product.#", str =>
            //{
            //    logger.LogDebug($"{DateTime.Now} 收到集群消费消息：{str}");
            //    Console.WriteLine(str);
            //    return true;
            //}, new SubscribeOptions() { BasicQos = 1 });


            eventBus.Publish("defualt", "product", "product.create", Guid.NewGuid().ToString("N"), "message content");


            //var tasks = new List<Task>();

            //for (int i = 0; i < 10; i++)
            //{
            //    var task = Task.Run(() =>
            //    {
            //        while (true)
            //        {
            //            eventBus.Publish("defualt", "product", "product.create", Guid.NewGuid().ToString("N"), "message content");
            //        }
            //    });
            //    tasks.Add(task);
            //}
            //var t = NewMethod(eventBus, tasks);
            //tasks.Add(t);

            Console.WriteLine("订阅完成");
            Console.ReadLine();
        }

        private static async Task NewMethod(IEventBus eventBus, List<Task> tasks)
        {
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine("begin task publish message with " + i);

                var t = Task.Run(() =>
                {
                    while (true)
                    {
                        eventBus.Publish("defualt", "product", "product.create", Guid.NewGuid().ToString("N"), "message content");
                    }
                });
                tasks.Add(t);
                await Task.Delay(30000);
            }
        }
    }
}
