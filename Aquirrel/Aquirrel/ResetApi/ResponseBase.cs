using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aquirrel.ResetApi
{

    public class ResponseBase : IResponseBase
    {
        public ResponseBase() : this(200)
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
        public int resCode { get; set; }

        public bool IsSuccess { get { return this.resCode == 200; } }
        public void ThrowIfError()
        {
            if (!this.IsSuccess) throw new BusinessException(this.msg, this.resCode);
        }

        public string msg { get; set; }

        public static ResponseBase Success { get; } = new ResponseBase();
        public static ResponseBase Fail { get; } = new ResponseBase(500);
        public static ResponseBase<TData> Create<TData>(TData data) { return new ResponseBase<TData>(data); }
        public static ResponseBase<TData> Create<TData>(int resCode, TData data) { return new ResponseBase<TData>(data) { resCode = resCode }; }
        public static ResponseBase<TData> Create<TData>(int resCode, string msg, TData data) { return new ResponseBase<TData>(data) { msg = msg, resCode = resCode }; }

        public static ResponseBase Create(int resCode, string msg = null) { return new ResponseBase(resCode, msg); }
    }
    public class ResponseBase<TData> : ResponseBase, IResponseBase<TData>
    {
        public ResponseBase()
        {

        }
        public ResponseBase(TData data)
        {
            this.data = data;
        }
        public TData data { get; set; }
    }
}
