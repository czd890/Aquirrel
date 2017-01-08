using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Framework.DependencyInjection;

namespace Aquirrel.ResetApi.Test.Controllers
{
    [Route("api/[controller]")]
    public class TraceReportController : Controller
    {
        ILogger _logger;
        public TraceReportController(ILogger<ValuesController> logger)
        {
            this._logger = logger;

        }

        [HttpPost]
        public IResponseBase Post([FromBody]tm model)
        {

            this._logger.LogDebug("post--------------------", (model as object).ToJson());
            return new ResponseBase();
        }
        [HttpPost, Route("ppp")]
        public IResponseBase P2([FromBody]tm model)
        {
            return new ResponseBase();

        }
        public class tm
        {
            public TraceModel input { get; set; }
        }
        public class TraceModel
        {
            public string app { get; set; }
            public string name { get; set; }
            public string clientip { get; set; }
            public string localip { get; set; }
            public string level { get; set; }

            public string seq { get; set; }
            public string traceid { get; set; }
            public List<KeyValuePair<string, string>> data { get; set; }
            public string time { get; set; }
        }
    }
}
