using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Aquirrel.Tracing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
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
            string depth = "";

            if (httpContext.Request.Headers.ContainsKey(RestApiConst.TraceId))
                pid = httpContext.Request.Headers[RestApiConst.TraceId].FirstOrDefault();
            if (pid.IsNullOrEmpty())
                pid = RestApiConst.NewTraceId();

            if (httpContext.Request.Headers.ContainsKey(RestApiConst.RequestDepth))
                depth = httpContext.Request.Headers[RestApiConst.RequestDepth].FirstOrDefault();
            if (depth.IsNullOrEmpty())
                depth = RestApiConst.NewDepth().ToString();
            httpContext.Response.Headers[RestApiConst.RequestDepth] = depth;
            httpContext.Response.Headers[RestApiConst.TraceId] = pid;

            _traceClient?.Init();

            var als = await _traceClient?.BeginRequestAsync(_env.ApplicationName, pid, depth, httpContext.Connection.RemoteIpAddress.ToString());
            //als.Datas["headers"] = httpContext.Request.Headers.ToJson();
            als.Datas["httpMethod"] = httpContext.Request.Method;
            als.Datas["requestUrl"] = httpContext.Request.Path.Value;

            if (httpContext.Request.Headers.ContainsKey(RestApiConst.UserOpenId))
                als.UserOpenId = httpContext.Request.Headers[RestApiConst.UserOpenId].FirstOrDefault();
            if (httpContext.Request.Headers.ContainsKey(RestApiConst.UserTraceId))
                als.UserTraceId = httpContext.Request.Headers[RestApiConst.UserTraceId].FirstOrDefault();
            if (httpContext.Request.Headers.ContainsKey(RestApiConst.AccessToken))
                als.AccessToken = httpContext.Request.Headers[RestApiConst.AccessToken].FirstOrDefault();
            if (httpContext.Request.Headers.ContainsKey(RestApiConst.RealIp))
                als.RealIp = httpContext.Request.Headers[RestApiConst.RealIp].FirstOrDefault();

            httpContext.Items["__ResetApiMiddleware_als"] = als;
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                _traceClient?.ErrorRequest(ex);
                throw;
            }
            finally
            {
                _traceClient?.CompleteRequest();
            }
        }
    }
}
