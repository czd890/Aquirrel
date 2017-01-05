using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Aquirrel.ResetApi
{

    public class RequestBase<TResponse> : IRequestBase<TResponse>, ITraceRequest where TResponse : IResponseBase
    {
        public RequestBase(HttpMethod method, string app, string apiName)
        {
            this._currentId = Guid.NewGuid();
            this._method = method;
            this._app = app;
            this.apiName = apiName;
        }
        Guid _currentId { get; set; }
        Guid _parentId { get; set; }
        Guid ITraceRequest.currentId { get { return _currentId; } }

        Guid ITraceRequest.parentId { get { return _parentId; } }
        internal void SetParentId(Guid parentId) { this._parentId = parentId; }


        HttpMethod _method;
        string _app;
        string apiName;
        HttpMethod IRequest.Method { get { return this._method; } }

        string IRequest.App { get { return this._app; } }

        string IRequest.ApiName { get { return this.apiName; } }


        public string version { get; set; }
    }
}
