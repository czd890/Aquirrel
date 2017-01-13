using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aquirrel.Interceptor;
using Castle.DynamicProxy;

namespace Aquirrel.Interceptor.Castle
{
    public class DynamicProxyInterceptor : IInterceptor
    {
        private InterceptorDelegate _interceptor;

        public DynamicProxyInterceptor(InterceptorDelegate inteceptor)
        {
            _interceptor = inteceptor;
        }
        public void Intercept(IInvocation invocation)
        {
            InterceptDelegate next = async context => await ((DynamicProxyInvocationContext)context).ProceedAsync();

            _interceptor(next)(new DynamicProxyInvocationContext(invocation)).Wait();
        }
    }
}