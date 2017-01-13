using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Aquirrel.Interceptor.Internal;

namespace Aquirrel.Interceptor
{
    public abstract class ProxyFactory : IProxyFactory
    {
        public IServiceProvider ServiceProvider { get; }
        public IInterceptorMatch MatchProvider { get; }
        InterceptorChainBuilder Builder { get; set; }
        Dictionary<Type, ChcheModel> _Cache = new Dictionary<Type, ChcheModel>();
        class ChcheModel
        {
            public Dictionary<MethodInfo, InterceptorDelegate> Interceptors { get; set; }
            public bool CanProxy { get; set; }
        }
        public ProxyFactory(IServiceProvider serviceProvider, IInterceptorMatch matchProvider, InterceptorChainBuilder builder)
        {
            this.ServiceProvider = serviceProvider;
            this.MatchProvider = matchProvider;
            this.Builder = builder;

            this.MatchProvider.ChangeToken.RegisterChangeCallback(_ =>
            {
                _Cache.Clear();
            }, null);
        }

        public virtual object CreateProxy(Type proxyType, object target)
        {
            if (proxyType == null || target == null)
                return null;
            if (!_Cache.ContainsKey(proxyType))
            {
                Dictionary<MethodInfo, InterceptorDelegate> interceptorMethods = new Dictionary<MethodInfo, InterceptorDelegate>();
                if (this.MatchProvider.IsMatchType(target.GetType()))
                {
                    var proxyTypeMethods = proxyType.GetMethods();
                    var typeMethods = target.GetType().GetMethods();

                    var interceptors = this.MatchProvider.GetMatchedInterceptor(target.GetType());
                    var builder = this.Builder.Use(interceptors);
                    var chainDelegate = builder.Build();

                    foreach (var proxyTypeMethod in proxyTypeMethods)
                    {

                        var method = typeMethods.FirstOrDefault(p => p.Name == proxyTypeMethod.Name);
                        if (!this.MatchProvider.IsMatchMethod(method))
                            continue;
                        interceptorMethods[method] = chainDelegate;
                    }
                }
                _Cache[proxyType] = new ChcheModel()
                {
                    Interceptors = interceptorMethods,
                    CanProxy = interceptorMethods.Any()
                };

            }
            if (_Cache[proxyType].CanProxy)
                return this.CreateProxyCore(proxyType, target, _Cache[proxyType].Interceptors);
            else
                return target;
        }
        protected abstract object CreateProxyCore(Type proxyType, object target, Dictionary<MethodInfo, InterceptorDelegate> interceptors);
    }
}
