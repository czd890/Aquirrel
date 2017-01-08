using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aquirrel.ResetApi;
using Aquirrel.ResetApi.Internal;
using Aquirrel.Tracing;
using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ResetApiExtensions
    {
        public static IServiceCollection AddRestApi(this IServiceCollection services)
        {
            services.AddSingleton<IApiClient, ApiClient>(sp => new ApiClient(
                sp.GetRequiredService<ILogger<ApiClient>>(),
                sp.GetRequiredService<IRestApiResolveApiUrl>(),
                sp.GetService<ITraceClient>()));

            services.AddSingleton<IRestApiResolveApiUrl, RestApiResolveApiUrl>();
            return services;
        }
    }
}
