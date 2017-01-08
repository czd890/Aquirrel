using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Aquirrel.ResetApi;

namespace Aquirrel.Tracing.Internal
{
    public class ReportRequest : RequestBase<ResponseBase>
    {
        public ReportRequest(HttpMethod method, string app, string apiName) : base(method, app, apiName)
        {
        }
        public object input { get; set; }
    }
}
