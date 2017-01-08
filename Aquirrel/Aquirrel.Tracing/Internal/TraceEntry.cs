using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aquirrel.Tracing.Internal
{
    public class EntryBase
    {
        public TransactionEntry ALS { get; set; }
    }
    public class TraceEventEntry : EntryBase
    {
        public string Event { get; set; }
    }
    public class TraceExceptionEntry : EntryBase
    {
        public Exception EX { get; set; }
        public string Message { get; set; }
    }

    public class TraceCompleteEntry : EntryBase
    {
        public string Message { get; set; }
    }
}
