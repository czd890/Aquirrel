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
            if (_traceClient != null)
            {
                string traceId = null;
                if (traceId == null && httpContext.Request.Headers.ContainsKey(RestApiConst.TraceId))
                    traceId = httpContext.Request.Headers[RestApiConst.TraceId].FirstOrDefault();
                if (traceId == null && httpContext.Request.Query.ContainsKey(RestApiConst.TraceId))
                    traceId = httpContext.Request.Query[RestApiConst.TraceId].FirstOrDefault();
                if (traceId == null && httpContext.Request.HasFormContentType && httpContext.Request.Form.ContainsKey(RestApiConst.TraceId))
                    traceId = httpContext.Request.Form[RestApiConst.TraceId];
                if (traceId == null && httpContext.Request.Cookies.ContainsKey(RestApiConst.TraceId))
                    traceId = httpContext.Request.Cookies[RestApiConst.TraceId];

                if (traceId.IsNullOrEmpty())
                    traceId = RestApiConst.NewTraceId();

                httpContext.Response.Cookies.Append(RestApiConst.TraceId, traceId);
                httpContext.Response.Headers.Add(RestApiConst.TraceId, new Microsoft.Extensions.Primitives.StringValues(traceId));

                var traceRoot = await _traceClient.CreateTransaction(_env.ApplicationName, httpContext.Request.Path, RestApiConst.NewTraceId(), traceId ?? "");
                traceRoot.UserId = httpContext.User?.Identity?.Name;
                traceRoot.ClientIp = httpContext.Connection.RemoteIpAddress.ToString();
                traceRoot.ExtendData.contentType = httpContext.Request.ContentType;
                traceRoot.ExtendData.headers = httpContext.Request.Headers.ToArray();
                if (httpContext.Request.HasFormContentType)
                    traceRoot.ExtendData.form = httpContext.Request.Form.ToArray();
                traceRoot.ExtendData.query = httpContext.Request.QueryString;
                traceRoot.ExtendData.method = httpContext.Request.Method;

                _traceClient.Begin(traceRoot);
            }
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
