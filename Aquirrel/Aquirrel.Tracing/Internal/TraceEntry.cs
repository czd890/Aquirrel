using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aquirrel.Tracing.Internal
{

    public class TraceEventEntry
    {
        public string Event { get; set; }
    }
    public class TraceExceptionEntry
    {
        public Exception EX { get; set; }
        public string Message { get; set; }
    }

    public class TraceCompleteEntry
    {
        public string Message { get; set; }
    }
}
