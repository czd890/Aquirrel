using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Aquirrel.Tracing.Internal
{
    public class TraceRoot
    {
        /// <summary>
        /// 异步线程共享变量
        /// </summary>
        public static AsyncLocal<TraceRoot> ALS = new AsyncLocal<TraceRoot>();


        public TraceRoot()
        {
            this.ExtendData = new System.Dynamic.ExpandoObject();
        }

        public string App { get; set; }
        public string Name { get; set; }
        public string UserId { get; set; }

        public string TraceId { get; set; }
        public int TraceLevel { get; set; }
        /// <summary>
        /// 本机ip
        /// </summary>
        public string LocalIp { get; set; }
        /// <summary>
        /// 客户端ip
        /// </summary>
        public string ClientIp { get; set; }
        public DateTime LastTime { get; set; }

        public dynamic ExtendData { get; set; }
        public string ParentId { get; set; }
        public int Seq { get; private set; }
        public int SeqIncremental() => ++Seq;
    }
}
