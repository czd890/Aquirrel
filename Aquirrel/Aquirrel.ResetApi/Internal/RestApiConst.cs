using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aquirrel.ResetApi.Internal
{
    public static class RestApiConst
    {
        public static string TraceId = "X-Request-Id";
        public static string RequestDepth = "X-Request-Depth";
        public static string NewTraceId() { return Guid.NewGuid().ToString("N"); }
        public static int NewDepth() => 10;
    }
}
