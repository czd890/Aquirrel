using System;
using Aquirrel.Tracing.Internal;

namespace Aquirrel.Tracing
{
    /// <summary>
    /// 将客户端收集的信息上报给服务端
    /// <para>异步队列发送</para>
    /// </summary>
    public interface IReportClient
    {

        void Report(TraceRoot root);
        void Report(TraceRoot root, string message);
        void Report(TraceRoot root, string message, Exception exception);
        void Report(TraceRoot root, Exception exception);
    }
}