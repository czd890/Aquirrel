using System;
using System.Net.Http;

namespace Aquirrel.ResetApi
{
    /// <summary>
    /// 请求对象
    /// </summary>
    public interface IRequest
    {
        /// <summary>
        /// 请求方式
        /// </summary>
        HttpMethod Method { get; }
        /// <summary>
        /// 当前app名称（域名地址）
        /// </summary>
        string App { get; }
        /// <summary>
        /// 当前请求接口名称（url地址）
        /// </summary>
        string ApiName { get; }
    }
    /// <summary>
    /// 请求对象
    /// </summary>
    /// <typeparam name="TResponse"></typeparam>
    public interface IRequestBase<out TResponse> : IRequest where TResponse : IResponseBase
    {
        /// <summary>
        /// 请求版本号
        /// </summary>
        string version { get; set; }

        /// <summary>
        /// 返回值对象类型
        /// </summary>
        Type ResponseType { get; }
    }
}
