using System.Net.Http;

namespace Aquirrel.ResetApi
{
    public interface IResponseBase
    {
        int resCode { get; set; }
        string msg { get; set; }
        bool IsSuccess { get; }
    }
    public interface IResponseBase<TData> : IResponseBase
    {
        TData data { get; set; }
    }
    public interface IRequest
    {
        HttpMethod Method { get; }
        string App { get; }
        string ApiName { get; }
    }
    public interface IRequestBase<out TResponse> : IRequest where TResponse : IResponseBase
    {
        string version { get; set; }
    }
    public static class ResponseErrorCode
    {
        public static int TaskCancel = 650;
        public static int TaskFail = 651;
        public static int ReadRpcContentError = 652;
        public static int ToJsonError = 653;
    }
}