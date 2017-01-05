using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Aquirrel.ResetApi.Internal
{
    public class RestApiALS
    {
        /// <summary>
        /// 异步线程共享变量
        /// </summary>
        public static AsyncLocal<RestApiALS> ALS = new AsyncLocal<RestApiALS>();

        public string TraceId { get; set; }
        public string ParentTraceId { get; set; }
    }
}
