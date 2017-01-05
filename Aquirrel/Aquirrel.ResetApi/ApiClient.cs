using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Aquirrel.ResetApi
{
    public class ApiClient
    {
        public ApiClient(ILogger<ApiClient> logger)
        {
            this.Logger = logger;
        }
        ILogger<ApiClient> Logger { get; set; }
        public T Execute<T>(IRequestBase<IResponseBase<T>> request)
        {
            Execute<IResponseBase<T>, T>(request);
            throw new NotImplementedException();
        }
        public IRes Execute<IRes, IData>(IRequestBase<IRes> request) where IRes : class, IResponseBase<IData>
        {
            this.ExecuteInternal(request).ContinueWith(new Func<Task<HttpResponseMessage>, object>(pp => { return null; }));

            return this.ExecuteInternal(request).ContinueWith(resTask =>
             {
                 var resType = typeof(IRes);
                 var obj = (IRes)Activator.CreateInstance(resType);

                 if (resTask.IsCanceled)
                 {
                     obj.error = ResponseErrorCode.TaskCancel;
                     obj.msg = "task is canceled";
                     return obj;
                 }
                 if (resTask.IsFaulted)
                 {
                     obj.error = ResponseErrorCode.TaskFail;
                     obj.msg = resTask.Exception.GetBaseException().Message;
                     this.Logger.LogError(0, resTask.Exception, "请求rpc接口错误.{0}", request.ToJson());
                     return obj;
                 }
                 else
                 {
                     var res = resTask.Result;
                     if (res.StatusCode != System.Net.HttpStatusCode.OK)
                     {
                         obj.error = res.IsSuccessStatusCode
 
                         return obj;
                     }
                     return obj;
                 }
             });
            return null;
        }
        public IResponseBase Execute(IRequestBase<IResponseBase> request)
        {
            throw new NotImplementedException();
        }

        HttpClient httpClient = new HttpClient();
        Task<HttpResponseMessage> ExecuteInternal(IRequest request)
        {
            var content = request.ToJson();
            return httpClient.SendAsync(new HttpRequestMessage()
            {
                Method = request.Method,
                Content = new StringContent(content, Encoding.UTF8, "application/json")
            });
        }
    }
}
