using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Aquirrel.ResetApi
{
    /// <summary>
    /// RPC请求对象
    /// </summary>
    /// <typeparam name="TResponse">返回值内容数据对象类型</typeparam>
    public class RequestBase<TResponse> : IRequestBase<TResponse>
        where TResponse : class, IResponseBase, new()
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="method">请求方式</param>
        /// <param name="app">当前app名称（域名地址）</param>
        /// <param name="apiName">当前请求接口名称（url地址）</param>
        public RequestBase(HttpMethod method, string app, string apiName)
        {
            this._method = method;
            this._app = app;
            this.apiName = apiName;
        }

        HttpMethod _method;
        string _app;
        string apiName;
        /// <summary>
        /// 请求方式
        /// </summary>
        HttpMethod IRequest.Method { get { return this._method; } }
        /// <summary>
        /// 当前app名称（域名地址）
        /// </summary>
        string IRequest.App { get { return this._app; } }
        /// <summary>
        /// 当前请求接口名称（url地址）
        /// </summary>
        string IRequest.ApiName { get { return this.apiName; } }
        /// <summary>
        /// 返回值对象类型
        /// </summary>
        Type IRequestBase<TResponse>.ResponseType => typeof(TResponse);
        /// <summary>
        /// 请求版本号
        /// </summary>

        public string version { get; set; }

    }
}
