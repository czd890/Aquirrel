using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aquirrel.Interceptor
{
    /// <summary>
    /// 拦截器调用链
    /// </summary>
    /// <param name="next"></param>
    /// <returns></returns>
    public delegate InterceptDelegate InterceptorDelegate(InterceptDelegate next);
}
