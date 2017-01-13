using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CD = Castle.DynamicProxy;
namespace Aquirrel.Interceptor.Castle
{
    public static class CastleDynamicProxyExtensions
    {
        public static T AsProxyTarget<T>(this object proxy)
        {
            var proxyInterface = proxy as CD.IProxyTargetAccessor;
            if (proxyInterface != null)
                return (T)proxyInterface.DynProxyGetTarget();
            return default(T);
        }
    }
}
