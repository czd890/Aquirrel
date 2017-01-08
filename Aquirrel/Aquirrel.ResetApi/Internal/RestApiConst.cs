using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aquirrel.ResetApi.Internal
{
    public static class RestApiConst
    {
        public static string TraceId = "traceid";
        public static string TraceLevel = "tracelevel";
        public static int TraceLevelRPCIncrement = 10000000;
        public static int TraceLevelCurrentIncrement = 1;
        //1000|0000
        public static string NewTraceId() { return Guid.NewGuid().ToString(); }
    }
}
