using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aquirrel.Tracing.Internal;

namespace Aquirrel.Tracing
{
    /// <summary>
    /// 上报埋点情况
    /// </summary>
    public interface ITraceClient
    {
        TransactionEntry Current { get; }
        /// <summary>
        /// 创建一个埋点
        /// </summary>
        /// <param name="app"></param>
        /// <param name="name"></param>
        TransactionEntry CreateTransaction(string app, string name);
        TransactionEntry CreateTransaction(string app, string name, string traceId, string parentId);

        void Event(string eventName);

        void Exception(Exception ex);
        void Exception(string message);
        void Exception(string message, Exception ex);
        void Complete();
    }
}
