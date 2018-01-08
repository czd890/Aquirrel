using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aquirrel.Tracing.Internal;
using Microsoft.Extensions.Logging;
using Aquirrel.Tools;
using System.Threading;

namespace Aquirrel.Tracing
{
    public class TraceClient : ITraceClient
    {
        ILogger _logger;
        public TraceClient(ILogger<TraceClient> logger)
        {
            this._logger = logger;
        }

        public IRequestEntry Current => RequestEntry.ALS.Value;
        public void Init()
        {
            RequestEntry.ALS.Value = new RequestEntry();
        }
        public Task<IRequestEntry> BeginRequestAsync(string app, string traceId, string traceDepth, string clientIp = "")
        {
            var als = RequestEntry.ALS.Value;

            als.App = app;
            als.TraceId = traceId;
            als.TraceDepth = traceDepth;
            als.ClientIp = clientIp;

            als.BeginTime = DateTime.Now;
            als.LocalIp = LocalIp.GetLocalIPV4();
            return Task.FromResult(als);
        }

        public void CompleteRequest()
        {
            RequestEntry.ALS.Value.EndTime = DateTime.Now;
            //TODO 上报接口请求日志
            this._logger.LogTrace(RequestEntry.ALS.Value.ToJson());
        }

        public void ErrorRequest(Exception exception)
        {
            RequestEntry.ALS.Value.Exception = exception;
        }
    }
}
