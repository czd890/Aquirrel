using System;

namespace Aquirrel.ResetApi.Internal
{
    public interface IRestApiResolveApiUrl
    {
        Uri Resolve(string app, string apiName);
    }
}