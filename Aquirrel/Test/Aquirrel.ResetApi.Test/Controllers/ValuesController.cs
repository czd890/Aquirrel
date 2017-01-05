using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Aquirrel.ResetApi.Test.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        ApiClient ApiClient;
        ILogger _logger;
        public ValuesController(ApiClient apiClient, ILogger<ValuesController> logger)
        {
            this.ApiClient = apiClient;
            this._logger = logger;
        }
        // GET api/values
        [HttpGet]
        public async Task<IEnumerable<string>> Get()
        {
            var req = /*(IRequestBase<IResponseBase>)*/new RequestBase<ResponseBase>(HttpMethod.Post, "http://localhost:5000/", "api/values");
            await this.ApiClient.ExecuteAsync(req);
            //await Task.Delay(3000);
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<string> Get(int id)
        {
            await Task.Delay(3000);
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
            this._logger.LogDebug("post--------------------");
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
