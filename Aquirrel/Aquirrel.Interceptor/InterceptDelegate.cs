using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aquirrel.Interceptor
{
    /// <summary>
    /// 拦截器
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public delegate Task InterceptDelegate(InvocationContext context);
}
