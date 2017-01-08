using System.Threading;
using System.Threading.Tasks;

namespace Aquirrel.ResetApi
{
    public interface IApiClient
    {
        Task<IResponseBase> ExecuteAsync(IRequestBase<IResponseBase> request);
        Task<IResponseBase> ExecuteAsync(IRequestBase<IResponseBase> request, CancellationToken token);

        Task<T> ExecuteAsync<T>(IRequestBase<IResponseBase<T>> request);
        Task<T> ExecuteAsync<T>(IRequestBase<IResponseBase<T>> request, CancellationToken token);

        Task<IRes> ExecuteAsync<IRes, IData>(IRequestBase<IRes> request) where IRes : class, IResponseBase<IData>;
        Task<IRes> ExecuteAsync<IRes, IData>(IRequestBase<IRes> request, CancellationToken token) where IRes : class, IResponseBase<IData>;
    }
}