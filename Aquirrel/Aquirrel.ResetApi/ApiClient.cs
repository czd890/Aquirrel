using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Http.Headers;
using Aquirrel.ResetApi.Internal;
using Aquirrel.Tracing;
using Aquirrel.Tracing.Internal;

namespace Aquirrel.ResetApi
{
    public class ApiClient : IApiClient
    {
        static HttpClient httpClient = new HttpClient();
        ILogger<ApiClient> Logger;
        IRestApiResolveApiUrl ApiResolveService;
        ITraceClient TraceClient;

        public ApiClient(ILogger<ApiClient> logger, IRestApiResolveApiUrl apiResolveService, ITraceClient traceClient)
        {
            this.Logger = logger;
            this.ApiResolveService = apiResolveService;
            this.TraceClient = traceClient;
        }

        public Task<TData> ExecuteAsync<TData>(IRequestBase<IResponseBase<TData>> request)
        {
            return this.ExecuteAsync(request, CancellationToken.None);
        }

        public async Task<TData> ExecuteAsync<TData>(IRequestBase<IResponseBase<TData>> request, CancellationToken token)
        {
            var res = await ExecuteAsync<IResponseBase<TData>, TData>(request, token);
            if (res.IsSuccess)
            {
                return res.data;
            }
            throw new BusinessException(res.msg, res.resCode);
        }


        public Task<TRes> ExecuteAsync<TRes, TData>(IRequestBase<TRes> request)
            where TRes : class, IResponseBase<TData>
        {
            return this.ExecuteAsync<TRes, TData>(request, CancellationToken.None);
        }
        public async Task<TRes> ExecuteAsync<TRes, TData>(IRequestBase<TRes> request, CancellationToken token)
            where TRes : class, IResponseBase<TData>
        {
            return (TRes)await this.ExecuteAsync((IRequestBase<IResponseBase>)request, token);
        }


        public Task<IResponseBase> ExecuteAsync(IRequestBase<IResponseBase> request)
        {
            return this.ExecuteAsync(request, CancellationToken.None);
        }
        public async Task<IResponseBase> ExecuteAsync(IRequestBase<IResponseBase> request, CancellationToken token)
        {
            var obj = (IResponseBase)Activator.CreateInstance(request.ResponseType);
            var read = await ReadAsync(request, obj, token);
            if (read.isSuccess)
            {
                try
                {
                    obj = read.body.ToJson<IResponseBase>(request.ResponseType);
                }
                catch (Exception ex)
                {
                    obj.resCode = ResponseErrorCode.ToJsonError;
                    obj.msg = ex.Message;
                    this.Logger.LogError(0, ex, "rpc content to json error.{0}", read.Item2);
                }
            }
            return obj;
        }

        async Task<(bool isSuccess, string body)> ReadAsync(IRequest request, IResponseBase resObj, CancellationToken token)
        {
            IRequestEntry nextALS = null;
            try
            {
                if (this.TraceClient != null && this.TraceClient.Current != null)
                {
                    var als = this.TraceClient.Current;
                    nextALS = als.NewChildRequest();
                }

                var res = await ReadInternalAsync(request, nextALS, resObj, token);
                if (res.isSuccess)
                    return (true, res.body);
                else
                    nextALS.Exception = new BusinessException(this.ApiResolveService.Resolve(request.App, request.ApiName).ToString() + resObj.msg, resObj.resCode);
            }
            catch (Exception ex)
            {
                if (nextALS != null)
                    nextALS.Exception = ex;
            }
            finally
            {
                if (nextALS != null)
                    nextALS.EndTime = DateTime.Now;
            }
            return (false, null);
        }

        async Task<(bool isSuccess, string body, HttpResponseMessage response)> ReadInternalAsync(IRequest request, IRequestEntry nextALS, IResponseBase resObj, CancellationToken token)
        {
            var resTask = await this.SendAsync(request, nextALS, token).ContinueWith(p => p);

            token.ThrowIfCancellationRequested();

            if (resTask.IsCanceled)
            {
                resObj.resCode = ResponseErrorCode.TaskCancel;
                resObj.msg = "req rpc task is canceled";
                return (false, null, null);
            }
            if (resTask.IsFaulted)
            {
                resObj.resCode = ResponseErrorCode.TaskFail;
                resObj.msg = resTask.Exception.GetBaseException().GetBaseException().Message;
                this.Logger.LogError(0, resTask.Exception, "req rpc api error.{0}", request.ToJson());
                return (false, null, null);
            }

            var res = await resTask;

            if (!res.IsSuccessStatusCode)
            {
                resObj.resCode = res.StatusCode.ToInt();
                resObj.msg = res.ReasonPhrase;
                return (false, null, null);
            }
            token.ThrowIfCancellationRequested();
            var readTask = await res.Content.ReadAsStringAsync().ContinueWith(p => p);


            token.ThrowIfCancellationRequested();
            if (readTask.IsFaulted)
            {
                resObj.resCode = ResponseErrorCode.ReadRpcContentError;
                resObj.msg = readTask.Exception.GetBaseException().GetBaseException().Message;
                return (false, null, null);
            }
            if (readTask.IsCanceled)
            {
                resObj.resCode = ResponseErrorCode.TaskCancel;
                resObj.msg = "read prc content task is canceled";
                return (false, null, null);
            }

            var resultStr = await readTask;

            return (true, resultStr, res);
        }
        Task<HttpResponseMessage> SendAsync(IRequest request, IRequestEntry nextALS, CancellationToken token)
        {
            var requestUri = this.ApiResolveService.Resolve(request.App, request.ApiName);
            HttpRequestMessage req;
            if (request.Method == HttpMethod.Post || request.Method == HttpMethod.Put)
            {
                req = new HttpRequestMessage()
                {
                    Method = request.Method,
                    Content = new StringContent(request.ToJson(), Encoding.UTF8, "application/json"),
                    RequestUri = requestUri
                };
            }
            else
            {
                var uri = requestUri;
                System.Text.StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append(uri.Query);
                if (uri.Query.IsNotNullOrEmpty())
                    stringBuilder.Append("&");
                else
                    stringBuilder.Append("?");

                request.GetType().GetProperties()
                    .Each(p =>
                    {
                        stringBuilder.Append($"{System.Web.HttpUtility.UrlEncode(p.Name)}={System.Web.HttpUtility.UrlEncode(p.GetValue(request)?.ToString())}&");
                    });

                var u = new UriBuilder(uri);
                u.Query = stringBuilder.ToString();

                req = new HttpRequestMessage()
                {
                    Method = request.Method,
                    RequestUri = u.Uri
                };
            }

            if (nextALS != null)
            {
                req.Headers.TryAddWithoutValidation(RestApiConst.TraceId, nextALS.TraceId);
                req.Headers.TryAddWithoutValidation(RestApiConst.RequestDepth, nextALS.TraceDepth);
                req.Headers.TryAddWithoutValidation(RestApiConst.UserOpenId, nextALS.UserOpenId);
                req.Headers.TryAddWithoutValidation(RestApiConst.UserTraceId, nextALS.UserTraceId);
                req.Headers.TryAddWithoutValidation(RestApiConst.AccessToken, nextALS.AccessToken);
                req.Headers.TryAddWithoutValidation(RestApiConst.RealIp, nextALS.RealIp);
            }
            return httpClient.SendAsync(req, token);
        }


        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage httpRequestMessage) => this.SendAsync(httpRequestMessage, CancellationToken.None);
        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage httpRequestMessage, CancellationToken token)
        {
            IRequestEntry nextALS = null;
            if (this.TraceClient != null && this.TraceClient.Current != null)
            {
                var als = this.TraceClient.Current;
                nextALS = als.NewChildRequest();
            }

            if (nextALS != null)
            {
                httpRequestMessage.Headers.TryAddWithoutValidation(RestApiConst.TraceId, nextALS.TraceId);
                httpRequestMessage.Headers.TryAddWithoutValidation(RestApiConst.RequestDepth, nextALS.TraceDepth);
                httpRequestMessage.Headers.TryAddWithoutValidation(RestApiConst.UserOpenId, nextALS.UserOpenId);
                httpRequestMessage.Headers.TryAddWithoutValidation(RestApiConst.UserTraceId, nextALS.UserTraceId);
                httpRequestMessage.Headers.TryAddWithoutValidation(RestApiConst.AccessToken, nextALS.AccessToken);
                httpRequestMessage.Headers.TryAddWithoutValidation(RestApiConst.RealIp, nextALS.RealIp);
            }
            return httpClient.SendAsync(httpRequestMessage, token).ContinueWith(resTask =>
            {
                if (nextALS != null)
                    nextALS.EndTime = DateTime.Now;

                if (resTask.IsFaulted)
                {
                    if (nextALS != null)
                        nextALS.Exception = resTask.Exception;
                }
                return resTask.Result;
            });

        }
    }
}
