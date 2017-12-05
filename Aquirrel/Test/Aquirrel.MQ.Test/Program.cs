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





            eventBus.Subscribe<string>("defualt", "ua_fund_pay_paidcallback", str =>
            {
                Console.WriteLine($"{DateTime.Now}  {str}");
                return false;
            }, new SubscribeOptions() { FailMesaageReQueue = false });

            //((Internal.IMQ)eventBus).Subscribe("defualt", "dead_letter_queue", message =>
            //{
            //    Console.WriteLine("死信队列");
            //    var x_Death = (List<object>)message.BasicProperties.Headers["x-death"];
            //    var death = x_Death.Last() as Dictionary<string, object>;

            //    var count = (long)death["count"];
            //    var reason = System.Text.Encoding.UTF8.GetString((byte[])death["reason"]);
            //    var queue = System.Text.Encoding.UTF8.GetString((byte[])(death["queue"]));
            //    var time = Aquirrel.IdBuilder.ToDateTime((int)((RabbitMQ.Client.AmqpTimestamp)death["time"]).UnixTime);
            //    var exchange = System.Text.Encoding.UTF8.GetString((byte[])(death["exchange"]));
            //    var routeKeys = (List<object>)death["routing-keys"];
            //    var routing_keys = System.Text.Encoding.UTF8.GetString((byte[])(routeKeys.First()));
            //    var id = "";
            //    if (message.BasicProperties.Headers.ContainsKey("mid"))
            //    {
            //        id = System.Text.Encoding.UTF8.GetString((byte[])message.BasicProperties.Headers["mid"]);
            //    }
            //    ((Internal.IMQ)eventBus).Publish("defualt", "", queue, id, message.Body, message.BasicProperties);
            //    return true;
            //});

            //while (true)
            //{
            //    Console.WriteLine("回车发送消息，输入q则退出");
            //    if (Console.ReadLine() == "q")
            //    {
            //        return;
            //    }
            //    else
            //    {
            //        var msg = Guid.NewGuid().ToString("N");
            //        eventBus.Publish("defualt", "fund", "fund.pay.paidcallback", msg, msg);

            //    }
            //}
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
            Task.Run(() =>
            {
                while (true)
                {
                    var msg = Guid.NewGuid().ToString("N");
                    eventBus.Publish("defualt", "fund", "fund.pay.paidcallback", msg, msg);
                    Thread.Sleep(6000);
                }
            });
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
