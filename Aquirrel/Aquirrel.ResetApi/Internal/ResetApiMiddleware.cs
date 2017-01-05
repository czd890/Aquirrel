using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Aquirrel.ResetApi.Internal
{
    public class ResetApiMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public ResetApiMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<ResetApiMiddleware>();
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var begin = DateTime.Now;
            string pid = "unknown";
            if (httpContext.Request.Headers.ContainsKey(RestApiConst.TraceId))
                pid = httpContext.Request.Headers[RestApiConst.TraceId].FirstOrDefault();

            RestApiALS.ALS.Value = new RestApiALS();
            RestApiALS.ALS.Value.ParentTraceId = pid;
            RestApiALS.ALS.Value.TraceId = Guid.NewGuid().ToString();
            this._logger.LogDebug($"begin traceId:{RestApiALS.ALS.Value.TraceId}. parentId:{RestApiALS.ALS.Value.ParentTraceId}. {httpContext.Request.Method}:{httpContext.Request.Path}");

            await _next(httpContext);
            var execTime = DateTime.Now - begin;
            this._logger.LogDebug($"  end traceId:{RestApiALS.ALS.Value.TraceId}. parentId:{RestApiALS.ALS.Value.ParentTraceId}. 执行时间:{execTime}. {httpContext.Request.Method}:{httpContext.Request.Path}");
            //TODO 上报执行时间
        }                                             
    }
}
