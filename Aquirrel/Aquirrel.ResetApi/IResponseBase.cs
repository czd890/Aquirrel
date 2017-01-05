using System.Net.Http;

namespace Aquirrel.ResetApi
{
    public interface IResponseBase
    {
        int error { get; set; }
        string msg { get; set; }
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
    public interface IRequestBase<TResponse> : IRequest where TResponse : IResponseBase
    {
        string version { get; set; }
    }
    public static class ResponseErrorCode
    {
        public static int TaskCancel = 650;
        public static int TaskFail = 651;
    }
}