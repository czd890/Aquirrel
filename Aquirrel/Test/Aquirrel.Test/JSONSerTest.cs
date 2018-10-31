using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Aquirrel.ResetApi;
using System.Threading;
using Aquirrel;

namespace Aquirrel.Test
{
    [TestClass]
    public class JSONSerTest
    {

        public class myres : Aquirrel.ResetApi.ResponseBase
        {
            public string resp { get; set; }
        }
        public class myreq : Aquirrel.ResetApi.RequestBase<myres>
        {
            public myreq() : base(System.Net.Http.HttpMethod.Get, "http://", "/api/controller/name")
            {

            }
            public string p1 { get; set; }
        }
        [TestMethod]
        public void MyTestMethod()
        {
            IRequest r = new myreq();
            var s = r.ToJson();
            Console.WriteLine(s);
        }
        [TestMethod]
        public void timestamp()
        {
            var s = Aquirrel.IdBuilder.TimeStampUTC();
            Console.WriteLine(s);
        }

        [TestMethod]
        public void IdWorker()
        {
            var s = new IdWorker(1, 1).nextId();
            Console.WriteLine(s);
        }

        [TestMethod]
        public void getid()
        {
            var id = IdBuilder.NextStringId();
            var id2 = IdBuilder.NextStringId();
            var oid = new Aquirrel.ObjectId(id);

            Console.WriteLine(id);
        }

        [TestMethod]
        public void logck_spinklock_inc()
        {
            object o = new object();
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            var j = 0;
            for (int i = 0; i < 100000; i++)
            {
                lock (o)
                {
                    j++;
                }
            }

            Console.WriteLine(stopwatch.ElapsedTicks);

            Console.WriteLine($"--------------{DateTime.Now.Ticks}-----------------");
            SpinLock spinLock = new SpinLock(false);

            stopwatch.Reset();
            Console.WriteLine(stopwatch.ElapsedTicks);
            stopwatch.Start();
            for (int i = 0; i < 100000; i++)
            {
                var islock = false;
                spinLock.Enter(ref islock);
                if (islock)
                {
                    j++;

                }
                if (islock)
                {
                    spinLock.Exit(false);

                }
            }
            Console.WriteLine(stopwatch.ElapsedTicks);
            Console.WriteLine("-------------------------------");

            /**
             * 
             * release 模式
             * 测试名称:	logck_spinklock_inc
               测试结果:	已通过
               结果 StandardOutput:	
               6317
               --------------636719295756100579-----------------
               0
               4353
               -------------------------------
             debug 模式
                测试名称:	logck_spinklock_inc
                测试结果:	已通过
                结果 StandardOutput:	
                6220
                --------------636719296623408640-----------------
                0
                5201
                -------------------------------
             * 
             * **/
        }


        [TestMethod]
        public void logck_spinklock_inc_concurrent()
        {
            var t1 = Task.Run(() => { Console.WriteLine("1111111111"); });
            var t2 = Task.Run(() => { Console.WriteLine("2222222222"); });
            t1.GetAwaiter().GetResult();
            t2.GetAwaiter().GetResult();

            object o = new object();
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            var j = 0;
            var t3 = Task.Run(() => { lock_work(o, j); });
            var t4 = Task.Run(() => { lock_work(o, j); });

            t3.GetAwaiter().GetResult();
            t4.GetAwaiter().GetResult();

            Console.WriteLine(stopwatch.ElapsedTicks);

            Console.WriteLine($"--------------{DateTime.Now.Ticks}-----------------");
            SpinLock spinLock = new SpinLock(false);

            stopwatch.Reset();
            Console.WriteLine(stopwatch.ElapsedTicks);
            stopwatch.Start();
            var t5 = Task.Run(() => { lock_spinlock(j, spinLock); });
            var t6 = Task.Run(() => { lock_spinlock(j, spinLock); });

            t5.GetAwaiter().GetResult();
            t6.GetAwaiter().GetResult();
            Console.WriteLine(stopwatch.ElapsedTicks);
            Console.WriteLine("-------------------------------");

            /**
             * 
            测试名称:	logck_spinklock_inc_concurrent
            测试结果:	已通过
            结果 StandardOutput:	
            1111111111
            2222222222
            19976
            --------------636719305602122167-----------------
            0
            6573
            -------------------------------


             * 
             * **/
        }

        private static int lock_spinlock(int j, SpinLock spinLock)
        {
            for (int i = 0; i < 100000; i++)
            {
                var islock = false;
                spinLock.Enter(ref islock);
                if (islock)
                {
                    j++;

                }
                if (islock)
                {
                    spinLock.Exit(false);

                }
            }

            return j;
        }

        private static int lock_work(object o, int j)
        {
            for (int i = 0; i < 100000; i++)
            {
                lock (o)
                {
                    j++;
                }
            }

            return j;
        }
    }
}
