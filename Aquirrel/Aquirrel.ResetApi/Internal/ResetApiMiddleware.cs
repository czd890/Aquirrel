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
            int pLevel = 0;
            if (httpContext.Request.Headers.ContainsKey(RestApiConst.TraceId))
                pid = httpContext.Request.Headers[RestApiConst.TraceId].FirstOrDefault();
            if (httpContext.Request.Headers.ContainsKey(RestApiConst.TraceLevel))
                pLevel = httpContext.Request.Headers[RestApiConst.TraceLevel].FirstOrDefault().ToInt(0);
            if (pid.IsNullOrEmpty())
                pid = RestApiConst.NewTraceId();
           
            pLevel += RestApiConst.TraceLevelRPCIncrement;
            _traceClient?.CreateTransaction(_env.ApplicationName, httpContext.Request.Method + ":" + httpContext.Request.Path, pid, pLevel,
                httpContext.Connection.RemoteIpAddress.ToString());

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
