using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Aquirrel.Interceptor.Castle;
namespace Aquirrel.Interceptor.Test
{
    [TestClass]
    public class AopTest
    {


        [TestMethod]
        public void MyTestMethod()
        {
            var x = Type.GetType("Aquirrel.Interceptor.Test.exceptionInterceptor");
            Startup startup = new Startup(null);
            var sp = startup.ConfigureServices(new ServiceCollection().AddSingleton<IAAA, AAA>().AddSingleton<AAA>());
            var logger = sp.GetService<ILogger<AopTest>>();
            var aaa = sp.GetService<IAAA>();
            logger.LogInformation(aaa.GetType().FullName);
            logger.LogInformation(aaa.Name);
            logger.LogInformation(aaa.NameVirtual);
            sp.GetService<AAA>().HeiHei2();
            aaa.HeiHei();

            aaa.AsProxyTarget<AAA>().HeiHei2();

        }

        public class AAA : IAAA
        {
            ILogger logger;
            public AAA(ILogger<AAA> logger)
            {
                this.logger = logger;
            }
            public AAA()
            {

            }
            public string Name { get; set; } = "AAA";

            public virtual string NameVirtual { get; set; } = "NameVirtual";

            public virtual void HeiHei()
            {
                logger.LogInformation("method heihei invoked");
            }
            public virtual void HeiHei2()
            {
                logger.LogInformation("method heihei2 invoked");
            }
        }
    }

    public class exceptionInterceptor
    {
        InterceptDelegate _next;
        public exceptionInterceptor(InterceptDelegate next, ILogger<timingInterceptor> logger)
        {
            _next = next;
        }
        public async Task InvokeAsync(InvocationContext context)
        {
            await _next(context);
        }
    }
    public class timingInterceptor
    {
        InterceptDelegate _next;
        ILogger _logger;
        public timingInterceptor(InterceptDelegate next, ILogger<timingInterceptor> logger)
        {
            _next = next;
            _logger = logger;
        }
        public async Task InvokeAsync(InvocationContext context)
        {
            _logger.LogInformation(context.Method.Name + " begin");
            await _next(context);
            _logger.LogInformation(context.Method.Name + " end");
        }
    }
}
