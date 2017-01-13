using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aquirrel.Interceptor.Castle;
using Aquirrel.Interceptor.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CastleInterceptorServiceCollectionExtensions
    {
        public static void AddInterceptor(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddInterceptor<DynamicProxyFactory>(
                sp => new DynamicProxyFactory(
                    sp,
                    sp.GetRequiredService<IInterceptorMatch>(),
                    sp.GetRequiredService<InterceptorChainBuilder>()),
                configuration);
        }
    }
}
