using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Aquirrel.Tools;
using Aquirrel.Tracing.Internal;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace Aquirrel.Tracing
{
    public class TraceClient : ITraceClient
    {
        private readonly ILogger _logger;
        private readonly IHostingEnvironment env;
        public TraceClient(ILogger<TraceClient> logger, IHostingEnvironment env)
        {
            this._logger = logger;
            this.env = env;
        }

        public IRequestEntry Current
        {
            get
            {
                if (RequestEntry.ALS.Value == null)
                {
                    this.Init();
                    this.BeginRequestAsync(this.env.ApplicationName, ResetApi.Internal.RestApiConst.NewTraceId(),
                        ResetApi.Internal.RestApiConst.NewDepth().ToString()).ConfigureAwait(false).GetAwaiter().GetResult();
                }
                return RequestEntry.ALS.Value;
            }
        }
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
