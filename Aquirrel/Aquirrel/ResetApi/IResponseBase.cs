using System.Net.Http;

namespace Aquirrel.ResetApi
{
    public interface IResponseBase
    {
        int resCode { get; set; }
        string msg { get; set; }
        bool IsSuccess { get; }
        void ThrowIfError();
    }
    public interface IResponseBase<TData> : IResponseBase
    {
        TData data { get; set; }
    }

    public static class ResponseErrorCode
    {
        public static int TaskCancel = 650;
        public static int TaskFail = 651;
        public static int ReadRpcContentError = 652;
        public static int ToJsonError = 653;
        public static int Error = 500;
        public static int BusinessError = 5000;
    }
}