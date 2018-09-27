using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Aquirrel.ResetApi
{
    /// <summary>
    /// RPC接口调用，内部实现跟踪调用链相关信息
    /// </summary>
    public interface IApiClient
    {
        /// <summary>
        /// 执行RPC请求
        /// </summary>
        /// <param name="request">请求对象</param>
        /// <returns></returns>
        Task<IResponseBase> ExecuteAsync(IRequestBase<IResponseBase> request);
        /// <summary>
        /// 执行RPC请求
        /// </summary>
        /// <param name="request">请求对象</param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<IResponseBase> ExecuteAsync(IRequestBase<IResponseBase> request, CancellationToken token);
        /// <summary>
        /// 执行RPC请求
        /// </summary>
        /// <typeparam name="T">返回值类型</typeparam>
        /// <param name="request">请求对象</param>
        /// <returns></returns>
        Task<T> ExecuteAsync<T>(IRequestBase<IResponseBase<T>> request);
        /// <summary>
        /// 执行RPC请求
        /// </summary>
        /// <typeparam name="T">返回值类型</typeparam>
        /// <param name="request">请求对象</param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<T> ExecuteAsync<T>(IRequestBase<IResponseBase<T>> request, CancellationToken token);
        /// <summary>
        /// 执行RPC请求
        /// </summary>
        /// <typeparam name="IRes">返回值类型，继承自<see cref="IResponseBase"/> </typeparam>
        /// <typeparam name="IData">返回值内容数据对象类型</typeparam>
        /// <param name="request">请求对象</param>
        /// <returns></returns>
        Task<IRes> ExecuteAsync<IRes, IData>(IRequestBase<IRes> request) where IRes : class, IResponseBase<IData>;
        /// <summary>
        /// 执行RPC请求
        /// </summary>
        /// <typeparam name="IRes">返回值类型，继承自<see cref="IResponseBase"/> </typeparam>
        /// <typeparam name="IData">返回值内容数据对象类型</typeparam>
        /// <param name="request">请求对象</param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<IRes> ExecuteAsync<IRes, IData>(IRequestBase<IRes> request, CancellationToken token) where IRes : class, IResponseBase<IData>;

        /// <summary>
        /// 发送原始http RPC请求
        /// </summary>
        /// <param name="httpRequestMessage">原始请求对象</param>
        /// <returns></returns>
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage httpRequestMessage);
        /// <summary>
        /// 发送原始http RPC请求
        /// </summary>
        /// <param name="httpRequestMessage">原始请求对象</param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage httpRequestMessage, CancellationToken token);
    }
}