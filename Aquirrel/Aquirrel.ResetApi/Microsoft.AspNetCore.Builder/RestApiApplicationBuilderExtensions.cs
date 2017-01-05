using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aquirrel.ResetApi.Internal;
using Microsoft.AspNetCore.Builder;

namespace Microsoft.AspNetCore.Builder
{
    public static class RestApiApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseRestApiTrace(this IApplicationBuilder app)
        {
            app.UseMiddleware<ResetApiMiddleware>();
            return app;
        }
    }
}
