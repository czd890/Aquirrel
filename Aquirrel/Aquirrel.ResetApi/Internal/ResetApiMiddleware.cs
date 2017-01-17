using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Aquirrel.Tracing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Framework.DependencyInjection;

namespace Aquirrel.ResetApi.Internal
{
    public class ResetApiMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private readonly IServiceProvider _sp;
        private readonly ITraceClient _traceClient;
        private readonly IHostingEnvironment _env;
        public ResetApiMiddleware(RequestDelegate next, ILoggerFactory loggerFactory, IHostingEnvironment env, IServiceProvider sp)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<ResetApiMiddleware>();
            _sp = sp;
            _env = env;

            _traceClient = _sp.GetService<ITraceClient>();
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var begin = DateTime.Now;
            string pid = "";
            if (httpContext.Request.Headers.ContainsKey(RestApiConst.TraceId))
                pid = httpContext.Request.Headers[RestApiConst.TraceId].FirstOrDefault();
            if (pid.IsNullOrEmpty())
                pid = RestApiConst.NewTraceId();

            var entry = _traceClient?.CreateTransaction(_env.ApplicationName, httpContext.Request.Method + ":" + httpContext.Request.Path, RestApiConst.NewTraceId(), pid);
            entry.ExtendData.clientIp = httpContext.Connection.RemoteIpAddress.ToString();
            entry.ExtendData.headers = httpContext.Request.Headers;
            entry.ExtendData.user = httpContext.User?.Identity?.Name;
            entry.ExtendData.query = httpContext.Request.QueryString;
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                _traceClient?.Exception(ex);
                throw;
            }
            finally
            {
                _traceClient?.Complete();
            }
        }
    }
}
