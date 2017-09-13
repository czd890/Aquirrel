using System.Net.Http;

namespace Aquirrel.ResetApi
{
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
}
