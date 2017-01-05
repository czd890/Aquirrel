﻿using System;
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

namespace Aquirrel.ResetApi
{
    public class ApiClient
    {
        ILogger<ApiClient> Logger { get; set; }
        IRestApiResolveApiUrl ApiResolveService { get; set; }
        public ApiClient(ILogger<ApiClient> logger, IRestApiResolveApiUrl apiResolveService)
        {
            this.Logger = logger;
            this.ApiResolveService = apiResolveService;
        }
        public async Task<T> ExecuteAsync<T>(IRequestBase<IResponseBase<T>> request)
        {
            var res = await ExecuteAsync<IResponseBase<T>, T>(request);
            if (res.IsSuccess)
            {
                return res.data;
            }
            throw new BusinessException($"{res.msg}.errorcode:{res.error}");
        }
        public async Task<IRes> ExecuteAsync<IRes, IData>(IRequestBase<IRes> request) where IRes : class, IResponseBase<IData>
        {
            var resType = typeof(IRes);
            var obj = (IRes)Activator.CreateInstance(resType);

            var read = await ReadAsync(request, obj);
            if (read.Item1)
            {
                try
                {
                    obj = read.Item2.ToJson<IRes>();
                }
                catch (Exception ex)
                {
                    obj.error = ResponseErrorCode.ToJsonError;
                    obj.msg = ex.Message;
                    this.Logger.LogError(0, ex, "rpc content to json error.{0}", read.Item2);
                }
            }
            return obj;

        }
        public async Task<IResponseBase> ExecuteAsync(IRequestBase<IResponseBase> request)
        {
            var obj = new ResponseBase();
            var read = await this.ReadAsync(request, obj);
            if (read.Item1)
            {
                try
                {
                    obj = read.Item2.ToJson<ResponseBase>();
                }
                catch (Exception ex)
                {
                    obj.error = ResponseErrorCode.ToJsonError;
                    obj.msg = ex.Message;
                    this.Logger.LogError(0, ex, "rpc content to json error.{0}", read.Item2);
                }
            }
            return obj;
        }

        async Task<Tuple<bool, string>> ReadAsync(IRequest request, IResponseBase resObj)
        {
            var r = new Tuple<bool, string>(false, null);
            var resTask = this.SendAsync(request);
            var res = await resTask;
            if (resTask.IsCanceled)
            {
                resObj.error = ResponseErrorCode.TaskCancel;
                resObj.msg = "req rpc task is canceled";
                return r;
            }
            if (resTask.IsFaulted)
            {
                resObj.error = ResponseErrorCode.TaskFail;
                resObj.msg = resTask.Exception.GetBaseException().Message;
                this.Logger.LogError(0, resTask.Exception, "req rpc api error.{0}", request.ToJson());
                return r;
            }

            if (!res.IsSuccessStatusCode)
            {
                resObj.error = res.StatusCode.ToInt();
                resObj.msg = res.ReasonPhrase;
                return r;
            }

            var readTask = res.Content.ReadAsStringAsync();
            var resultStr = await readTask;
            if (readTask.IsFaulted)
            {
                resObj.error = ResponseErrorCode.ReadRpcContentError;
                resObj.msg = readTask.Exception.GetBaseException().Message;
                return r;
            }
            if (readTask.IsCanceled)
            {
                resObj.error = ResponseErrorCode.TaskCancel;
                resObj.msg = "read prc content task is canceled";
                return r;
            }
            return new Tuple<bool, string>(true, resultStr);
        }

        HttpClient httpClient = new HttpClient();
        Task<HttpResponseMessage> SendAsync(IRequest request)
        {
            var content = request.ToJson();
            var req = new HttpRequestMessage()
            {
                Method = request.Method,
                Content = new StringContent(content, Encoding.UTF8, "application/json"),
                RequestUri = this.ApiResolveService.Resolve(request.App, request.ApiName)
            };
            req.Headers.Add(RestApiConst.TraceId, request.currentId);
            return httpClient.SendAsync(req);
        }
    }
}
