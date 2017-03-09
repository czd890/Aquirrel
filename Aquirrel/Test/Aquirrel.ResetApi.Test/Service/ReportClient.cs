using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Aquirrel.ResetApi.Test.Service
{
    public class ReportClient : Aquirrel.Tracing.ReportClient
    {
        Microsoft.Extensions.Logging.ILogger _logger;
        public ReportClient(
           ILogger<ReportClient> logger,
            IServiceProvider sp,
            Tracing.Internal.TracingSetting setting) : base(logger, sp, setting)
        {
            _logger = logger;
        }
        protected override Task<bool> Report(List<dynamic> datas)
        {
            //return base.Report(datas);
            foreach (var item in datas)
            {
                this._logger.LogInformation((item as object).ToJson().Replace("\\r\\n",Environment.NewLine));
            }
            return Task.FromResult(true);
        }
    }
}
