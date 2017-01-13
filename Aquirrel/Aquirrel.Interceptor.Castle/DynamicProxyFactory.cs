using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Aquirrel.Interceptor.Internal;
using Castle.DynamicProxy;

namespace Aquirrel.Interceptor.Castle
{
    public class DynamicProxyFactory : ProxyFactory
    {
        private ProxyGenerator _proxyGenerator;

        public DynamicProxyFactory(IServiceProvider serviceProvider, IInterceptorMatch matchProvider, InterceptorChainBuilder builder)
            : base(serviceProvider, matchProvider, builder)
        {
            _proxyGenerator = new ProxyGenerator();
        }

        protected override object CreateProxyCore(Type proxyType, object target, Dictionary<MethodInfo, InterceptorDelegate> interceptors)
        {
            Dictionary<MethodInfo, IInterceptor> dic = interceptors.ToDictionary(it => it.Key,
                it => (IInterceptor)new DynamicProxyInterceptor(it.Value));

            var selector = new DynamicProxyInterceptorSelector(dic);
            var options = new ProxyGenerationOptions { Selector = selector };
            if (proxyType.GetTypeInfo().IsInterface)
            {
                return _proxyGenerator.CreateInterfaceProxyWithTarget(proxyType, target, options, dic.Values.ToArray());
            }
            else
            {
                return _proxyGenerator.CreateClassProxyWithTarget(proxyType, target, options, dic.Values.ToArray());
            }
        }
    }
}
