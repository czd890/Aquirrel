using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aquirrel.ResetApi.Internal;
using Aquirrel.Tracing;
using Aquirrel.Tracing.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class TraceReportExtensions
    {
        public static IServiceCollection AddAquirrelTrace(this IServiceCollection services, IConfiguration configuration)
        {
            AddAquirrelTrace(services, new Aquirrel.Tracing.Internal.TracingSetting(configuration));
            return services;
        }
        public static IServiceCollection AddAquirrelTrace(this IServiceCollection services, TracingSetting conf)
        {
            services.AddSingleton<ITraceClient, TraceClient>();
            services.AddSingleton<IReportClient, ReportClient>(sp => new ReportClient(
                sp.GetRequiredService<ILogger<ReportClient>>(),
                sp, conf));
            return services;
        }

    }
}
