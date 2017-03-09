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
        public Task<T> ExecuteAsync<T>(IRequestBase<IResponseBase<T>> request)
        {
            return this.ExecuteAsync(request, CancellationToken.None);
        }
        public async Task<T> ExecuteAsync<T>(IRequestBase<IResponseBase<T>> request, CancellationToken token)
        {
            var res = await ExecuteAsync<IResponseBase<T>, T>(request, token);
            if (res.IsSuccess)
            {
                return res.data;
            }
            throw new BusinessException($"{res.msg}.errorcode:{res.resCode}");
        }
        public Task<IRes> ExecuteAsync<IRes, IData>(IRequestBase<IRes> request) where IRes : class, IResponseBase<IData>
        {
            return this.ExecuteAsync<IRes, IData>(request, CancellationToken.None);
        }
        public async Task<IRes> ExecuteAsync<IRes, IData>(IRequestBase<IRes> request, CancellationToken token) where IRes : class, IResponseBase<IData>
        {
            var resType = typeof(IRes);
            var obj = (IRes)Activator.CreateInstance(resType);

            var read = await ReadAsync(request, obj, token);
            if (read.Item1)
            {
                try
                {
                    obj = read.Item2.ToJson<IRes>();
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
        public Task<IResponseBase> ExecuteAsync(IRequestBase<IResponseBase> request)
        {
            return this.ExecuteAsync(request, CancellationToken.None);
        }
        public async Task<IResponseBase> ExecuteAsync(IRequestBase<IResponseBase> request, CancellationToken token)
        {
            var obj = new ResponseBase();
            var read = await this.ReadAsync(request, obj, token);
            if (read.Item1)
            {
                try
                {
                    obj = read.Item2.ToJson<ResponseBase>();
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
            return new Tuple<bool, string>(true, resultStr);
        }

        HttpClient httpClient = new HttpClient();
        Task<HttpResponseMessage> SendAsync(IRequest request, CancellationToken token)
        {
            var content = request.ToJson();
            var req = new HttpRequestMessage()
            {
                Method = request.Method,
                Content = new StringContent(content, Encoding.UTF8, "application/json"),
                RequestUri = this.ApiResolveService.Resolve(request.App, request.ApiName)
            };
            if (this.TraceClient != null&& this.TraceClient.Current!=null)
            {
                req.Headers.Add(RestApiConst.TraceId, this.TraceClient.Current.TraceId);
            }
            return httpClient.SendAsync(req, token);
        }
    }
}
