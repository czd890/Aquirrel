using Aquirrel.Tracing.Internal;

namespace Aquirrel.Tracing
{
    /// <summary>
    /// 将客户端收集的信息上报给服务端
    /// <para>异步队列发送</para>
    /// </summary>
    public interface IReportClient
    {

        void Report(TraceEventEntry entry);
        void Report(TraceExceptionEntry entry);
        void Report(TraceCompleteEntry entry);
    }
}