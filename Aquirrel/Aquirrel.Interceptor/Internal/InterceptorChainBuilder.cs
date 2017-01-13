using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Aquirrel;
using Microsoft.Extensions.DependencyInjection;

namespace Aquirrel.Interceptor.Internal
{
    /// <summary>
    /// 拦截器调用链生成器
    /// </summary>
    public class InterceptorChainBuilder
    {
        private delegate Task InvokeDelegate(object interceptor, InvocationContext context, IServiceProvider serviceProvider);

        public IServiceProvider ServiceProvider;
        List<KeyValuePair<int, InterceptorDelegate>> _interceptors = new List<KeyValuePair<int, InterceptorDelegate>>();
        public InterceptorChainBuilder(IServiceProvider serviceProvider)
        {
            this.ServiceProvider = serviceProvider;
        }
        public InterceptorChainBuilder New()
        {
            return new InterceptorChainBuilder(this.ServiceProvider);
        }
        public InterceptorDelegate Build()
        {
            var interceptors = this._interceptors.OrderByDescending(p => p.Key).Select(p => p.Value);

            InterceptorDelegate intercept = _next =>
            {
                var current = _next;
                foreach (var item in interceptors)
                {
                    current = item(current);
                }
                return current;
            };

            return intercept;

        }
        public InterceptorChainBuilder Use(IEnumerable<Type> interceptorTypes)
        {
            interceptorTypes.Each((type, index) => this.Use(type, index));
            return this;
        }


        public InterceptorChainBuilder Use(Type interceptorType, int order, params object[] args)
        {
            var invokeMethod = interceptorType.GetMethods().Where(p => p.Name == "InvokeAsync" && p.ReturnType == typeof(Task) && p.GetParameters().FirstOrDefault()?.ParameterType == typeof(InvocationContext)).FirstOrDefault();
            if (invokeMethod == null)
                return this;


            //InvokeDelegate(object interceptor, InvocationContext context, IServiceProvider serviceProvider);

            ParameterExpression interceptor = Expression.Parameter(typeof(object), "interceptor");
            ParameterExpression context = Expression.Parameter(typeof(InvocationContext), "context");
            ParameterExpression serviceProvider = Expression.Parameter(typeof(IServiceProvider), "serviceProvider");

            var invArgs = invokeMethod.GetParameters().Select(p =>
              {
                  if (p.ParameterType == typeof(InvocationContext))
                      return (Expression)context;

                  Expression serviceType = Expression.Constant(p.ParameterType, typeof(Type));
                  Expression callGetService = Expression.Call(_getServiceMethod, serviceProvider, serviceType);
                  return (Expression)Expression.Convert(callGetService, p.ParameterType);

              });

            Expression instanceConvert = Expression.Convert(interceptor, interceptorType);
            var invoke = Expression.Call(instanceConvert, invokeMethod, invArgs);
            var invoker = Expression.Lambda<InvokeDelegate>(invoke, interceptor, context, serviceProvider).Compile();

            InterceptorDelegate interceptorDelegate = _next =>
            {
                var instance = ActivatorUtilities.CreateInstance(this.ServiceProvider, interceptorType, _next);
                InterceptDelegate _ = async invContext =>
                {

                    await invoker(instance, invContext, this.ServiceProvider);
                };

                return _;
            };

            this._interceptors.Add(order, interceptorDelegate);
            return this;
        }

        static MethodInfo _getServiceMethod = typeof(InterceptorChainBuilder).GetTypeInfo().GetMethod("GetService", BindingFlags.Static | BindingFlags.NonPublic);
        static object GetService(IServiceProvider serviceProvider, Type type)
        {
            return serviceProvider.GetService(type);
        }
    }
}
