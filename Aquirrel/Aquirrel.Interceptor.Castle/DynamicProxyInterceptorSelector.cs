﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Castle.DynamicProxy;

namespace Aquirrel.Interceptor.Castle
{
    public class DynamicProxyInterceptorSelector : IInterceptorSelector
    {
        IDictionary<MethodInfo, IInterceptor> _interceptors;

        public DynamicProxyInterceptorSelector(IDictionary<MethodInfo, IInterceptor> interceptors)
        {
            _interceptors = interceptors;
        }

        public IInterceptor[] SelectInterceptors(Type type, MethodInfo method, IInterceptor[] interceptors)
        {
            IInterceptor interceptor;
            if (_interceptors.TryGetValue(method, out interceptor) && interceptors.Contains(interceptor))
            {
                return new IInterceptor[] { interceptor };
            }
            return new IInterceptor[0];
        }
    }
}
