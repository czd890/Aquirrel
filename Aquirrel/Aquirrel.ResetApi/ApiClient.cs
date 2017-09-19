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

namespace Aquirrel.ResetApi
{
    public class ApiClient : IApiClient
    {
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
            try
            {
                var obj = (IResponseBase)Activator.CreateInstance(request.ResponseType);


                var read = await ReadAsync(request, obj, token);
                if (read.Item1)
                {
                    try
                    {
                        obj = read.Item2.ToJson<IResponseBase>(request.ResponseType);
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
            catch (Exception ex)
            {

                throw;
            }
        }

        //public Task<TData> ExecuteWithNoErrorAsync<TData>(IRequestBase<IResponseBase<TData>> request) 
        //{
        //    this.ExecuteAsync(request);
        //    return null;
        //}

        async Task<Tuple<bool, string>> ReadAsync(IRequest request, IResponseBase resObj, CancellationToken token)
        {
            var r = new Tuple<bool, string>(false, null);
            var resTask = this.SendAsync(request, token);
            var res = await resTask;
            token.ThrowIfCancellationRequested();
            if (resTask.IsCanceled)
            {
                resObj.resCode = ResponseErrorCode.TaskCancel;
                resObj.msg = "req rpc task is canceled";
                return r;
            }
            if (resTask.IsFaulted)
            {
                resObj.resCode = ResponseErrorCode.TaskFail;
                resObj.msg = resTask.Exception.GetBaseException().Message;
                this.Logger.LogError(0, resTask.Exception, "req rpc api error.{0}", request.ToJson());
                return r;
            }

            if (!res.IsSuccessStatusCode)
            {
                resObj.resCode = res.StatusCode.ToInt();
                resObj.msg = res.ReasonPhrase;
                return r;
            }
            token.ThrowIfCancellationRequested();
            var readTask = res.Content.ReadAsStringAsync();
            var resultStr = await readTask;
            token.ThrowIfCancellationRequested();
            if (readTask.IsFaulted)
            {
                resObj.resCode = ResponseErrorCode.ReadRpcContentError;
                resObj.msg = readTask.Exception.GetBaseException().Message;
                return r;
            }
            if (readTask.IsCanceled)
            {
                resObj.resCode = ResponseErrorCode.TaskCancel;
                resObj.msg = "read prc content task is canceled";
                return r;
            }
            if (this.TraceClient != null && this.TraceClient.Current != null)
            {
                var als = this.TraceClient.Current;
                als.ChildRequest[res.RequestMessage.Headers.GetValues(RestApiConst.RequestDepth).FirstOrDefault()].EndTime = DateTime.Now;
            }
            return new Tuple<bool, string>(true, resultStr);
        }

        HttpClient httpClient = new HttpClient();
        Task<HttpResponseMessage> SendAsync(IRequest request, CancellationToken token)
        {
            var content = request.ToJson();
            HttpRequestMessage req;
            if (request.Method == HttpMethod.Post || request.Method == HttpMethod.Put)
            {
                req = new HttpRequestMessage()
                {
                    Method = request.Method,
                    Content = new StringContent(content, Encoding.UTF8, "application/json"),
                    RequestUri = this.ApiResolveService.Resolve(request.App, request.ApiName)
                };
            }
            else
            {
                var uri = this.ApiResolveService.Resolve(request.App, request.ApiName);
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

            if (this.TraceClient != null && this.TraceClient.Current != null)
            {
                var als = this.TraceClient.Current;
                var nextALS = als.NewChildRequest();
                req.Headers.TryAddWithoutValidation(RestApiConst.TraceId, nextALS.TraceId);
                req.Headers.TryAddWithoutValidation(RestApiConst.RequestDepth, nextALS.TraceDepth);
            }
            return httpClient.SendAsync(req, token);
        }
    }
}
