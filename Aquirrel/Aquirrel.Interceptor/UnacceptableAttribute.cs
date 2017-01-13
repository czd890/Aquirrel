using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aquirrel.Interceptor
{
    /// <summary>
    /// 排除的类或者方法标记属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class UnacceptableAttribute : Attribute
    {
    }
}
