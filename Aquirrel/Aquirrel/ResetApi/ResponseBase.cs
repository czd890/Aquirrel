using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aquirrel.ResetApi
{
    /// <summary>
    /// RPC请求响应对象
    /// </summary>
    public class ResponseBase : IResponseBase
    {
        public ResponseBase() : this(ResponseCode.Success)
        {

        }
        public ResponseBase(int resCode) : this(resCode, null)
        {

        }
        public ResponseBase(int resCode, string msg)
        {
            this.resCode = resCode;
            this.msg = msg;
        }
        /// <summary>
        /// 响应code <see cref="ResponseCode"/>
        /// </summary>
        public int resCode { get; set; }
        /// <summary>
        /// 是否正确的响应
        /// </summary>
        public bool IsSuccess { get { return this.resCode == ResponseCode.Success; } }
        /// <summary>
        /// 响应错误则抛出异常
        /// </summary>
        public void ThrowIfError()
        {
            if (!this.IsSuccess) throw new BusinessException(this.msg, this.resCode);
        }
        /// <summary>
        /// 响应附加消息
        /// </summary>
        public string msg { get; set; }
        /// <summary>
        /// 默认响应成功对象
        /// </summary>
        public static ResponseBase Success { get; } = new ResponseBase();
        /// <summary>
        /// 默认响应错误对象
        /// </summary>
        public static ResponseBase Fail { get; } = new ResponseBase(ResponseCode.Error);
        public static ResponseBase<TData> Create<TData>(TData data) { return new ResponseBase<TData>(data); }
        public static ResponseBase<TData> Create<TData>(int resCode, TData data) { return new ResponseBase<TData>(data) { resCode = resCode }; }
        public static ResponseBase<TData> Create<TData>(int resCode, string msg, TData data) { return new ResponseBase<TData>(data) { msg = msg, resCode = resCode }; }

        public static ResponseBase Create(int resCode, string msg = null) { return new ResponseBase(resCode, msg); }
    }
    /// <summary>
    /// RPC请求响应对象
    /// </summary>
    /// <typeparam name="TData">返回值内容数据对象类型</typeparam>
    public class ResponseBase<TData> : ResponseBase, IResponseBase<TData>
    {
        public ResponseBase()
        {

        }
        public ResponseBase(TData data)
        {
            this.data = data;
        }
        /// <summary>
        /// 返回值内容数据对象
        /// </summary>
        public TData data { get; set; }
    }
}
