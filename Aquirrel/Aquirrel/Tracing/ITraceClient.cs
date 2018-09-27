using System;
using Aquirrel.Tracing.Internal;
using System.Threading.Tasks;

namespace Aquirrel.Tracing
{
    /// <summary>
    /// 上报埋点情况
    /// </summary>
    public interface ITraceClient
    {
        IRequestEntry Current { get; }
        /// <summary>
        /// 请求开始
        /// </summary>
        /// <param name="app"></param>
        /// <param name="name"></param>
        Task<IRequestEntry> BeginRequestAsync(string app, string traceId, string traceDepth, string clientIp = "");

        void ErrorRequest(Exception exception);
        void CompleteRequest();

        void Init();
    }
}
