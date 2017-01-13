using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Aquirrel.Interceptor;
using Aquirrel.Interceptor.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class InterceptorServiceCollectionExtensions
    {
        public static void AddInterceptor<TProxyFactory>(this IServiceCollection services, Func<IServiceProvider, TProxyFactory> setup, IConfiguration configuration)
            where TProxyFactory : class, IProxyFactory
        {
            services.AddSingleton<IProxyFactory, TProxyFactory>(setup);
            services.AddSingleton<InterceptionSetting>(sp => new InterceptionSetting(configuration));
            services.AddSingleton<IInterceptorMatch, DefaultInterceptorMatch>();
            services.AddSingleton<InterceptorChainBuilder>(sp => new InterceptorChainBuilder(sp));

            //replace services to aop service collection
            Warp(services);
        }

        static void Warp(IServiceCollection services)
        {
            var sp = services.BuildServiceProvider();

            for (int i = 0; i < services.Count; i++)
            {
                var service = services[i];

                bool isGeneric = service.ServiceType.GetTypeInfo().IsGenericTypeDefinition;
                if (!isGeneric)
                {
                    //_：被替换过的sc生成的sp
                    Func<IServiceProvider, object> factory = _ => Wrap(sp, service.ServiceType, service.ImplementationType ?? service.ServiceType);

                    services[i] = new ServiceDescriptor(service.ServiceType, factory, service.Lifetime);
                }
            }

        }

        static object Wrap(IServiceProvider sp, Type serviceType, Type impType)
        {
            object target = sp.GetService(serviceType);
            if (target != null)
            {
                IProxyFactory proxyFactory = sp.GetRequiredService<IProxyFactory>();
                var proxyTarget = proxyFactory.CreateProxy(serviceType, target);
                target = proxyTarget ?? target;
            }
            return target;
        }
    }
}
