using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aquirrel.ResetApi;
using Aquirrel.ResetApi.Internal;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ResetApiExtensions
    {
        public static IServiceCollection AddRestApi(this IServiceCollection services)
        {
            services.AddSingleton<ApiClient>();
            services.AddSingleton<IRestApiResolveApiUrl, RestApiResolveApiUrl>();
            return services;
        }
    }
}
