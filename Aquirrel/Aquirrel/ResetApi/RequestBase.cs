using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Aquirrel.ResetApi
{

    public class RequestBase<TResponse> : IRequestBase<TResponse>
        where TResponse : class, IResponseBase, new()
    {
        public RequestBase(HttpMethod method, string app, string apiName)
        {
            //if (TracingALS.ALS.Value != null)
            //{
            //    this._currentId = TracingALS.ALS.Value.TraceId;
            //    this._parentId = TracingALS.ALS.Value.ParentTraceId;
            //}
            this._method = method;
            this._app = app;
            this.apiName = apiName;
        }
        //string _currentId { get; set; }
        //string _parentId { get; set; }
        //string ITraceRequest.currentId { get { return _currentId; } }

        //string ITraceRequest.parentId { get { return _parentId; } }

        HttpMethod _method;
        string _app;
        string apiName;
        HttpMethod IRequest.Method { get { return this._method; } }

        string IRequest.App { get { return this._app; } }

        string IRequest.ApiName { get { return this.apiName; } }
        Type IRequestBase<TResponse>.ResponseType => typeof(TResponse);


        public string version { get; set; }

    }
}
