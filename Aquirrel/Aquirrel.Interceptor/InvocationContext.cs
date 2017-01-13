using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Aquirrel.Interceptor
{
    public abstract class InvocationContext
    {
        public abstract object[] Arguments { get; }

        public abstract Type[] GenericArguments { get; }

        public abstract object InvocationTarget { get; }

        public abstract MethodInfo Method { get; }

        public abstract MethodInfo MethodInvocationTarget { get; }

        public abstract object Proxy { get; }

        public abstract object ReturnValue { get; set; }

        public abstract Type TargetType { get; }


        public abstract object GetArgumentValue(int index);

        public abstract void SetArgumentValue(int index, object value);
    }
}
