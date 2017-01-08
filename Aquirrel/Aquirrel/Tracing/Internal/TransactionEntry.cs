using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Aquirrel.Tracing.Internal
{
    public class TransactionEntry
    {
        /// <summary>
        /// 异步线程共享变量
        /// </summary>
        public static AsyncLocal<TransactionEntry> ALS = new AsyncLocal<TransactionEntry>();


        public TransactionEntry()
        {
            this.ExtendData = new System.Dynamic.ExpandoObject();
        }

        public string App { get; set; }
        public string Name { get; set; }

        public string TraceId { get; set; }
        public int TraceLevel { get; set; }
        public string LocalIp { get; set; }
        public string ClientIp { get; set; }
        public DateTime LastTime { get; set; }

        public dynamic ExtendData { get; set; }
    }
}
