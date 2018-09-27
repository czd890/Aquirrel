using System.Net.Http;

namespace Aquirrel.ResetApi
{
    /// <summary>
    /// RPC请求返回值对象
    /// </summary>
    public interface IResponseBase
    {
        /// <summary>
        /// 响应code <see cref="ResponseCode"/>
        /// </summary>
        int resCode { get; set; }
        /// <summary>
        /// 响应附加消息
        /// </summary>
        string msg { get; set; }
        /// <summary>
        /// 是否正确的响应
        /// </summary>
        bool IsSuccess { get; }
        /// <summary>
        /// 响应错误则抛出异常
        /// </summary>
        void ThrowIfError();
    }
    /// <summary>
    /// RPC请求返回值对象
    /// </summary>
    /// <typeparam name="TData">返回值内容数据对象类型</typeparam>
    public interface IResponseBase<TData> : IResponseBase
    {
        /// <summary>
        /// 返回数据
        /// </summary>
        TData data { get; set; }
    }
    /// <summary>
    /// 相应码
    /// </summary>
    public static class ResponseCode
    {
        /// <summary>
        /// 请求任务取消
        /// </summary>
        public static int TaskCancel = 650;
        /// <summary>
        /// 请求任务失败
        /// </summary>
        public static int TaskFail = 651;
        /// <summary>
        /// 响应读取错误
        /// </summary>
        public static int ReadRpcContentError = 652;
        /// <summary>
        /// 响应数据转json错误
        /// </summary>
        public static int ToJsonError = 653;
        /// <summary>
        /// 错误
        /// </summary>
        public static int Error = 500;
        /// <summary>
        /// 业务异常
        /// </summary>
        public static int BusinessError = 5000;
        /// <summary>
        /// 成功
        /// </summary>
        public static int Success = 200;
    }
}