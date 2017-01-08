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
    public class ValuesController : Controller
    {
        IApiClient ApiClient;
        ILogger _logger;
        public ValuesController(IApiClient apiClient, ILogger<ValuesController> logger)
        {
            this.ApiClient = apiClient;
            this._logger = logger;

        }
        public class myreq : RequestBase<ResponseBase>
        {
            public myreq() : base(HttpMethod.Post, "http://localhost:5000/", "api/values")
            {

            }
            public List<int> heihei { get; set; }


        }
        // GET api/values
        [HttpGet]
        public async Task<IEnumerable<string>> Get()
        {
            //var req = /*(IRequestBase<IResponseBase>)*/new RequestBase<ResponseBase>(HttpMethod.Post, "http://localhost:5000/", "api/values");
            var req = new myreq() { heihei = new List<int>() { 1, 2, 3 } };
            await this.ApiClient.ExecuteAsync(req);
            await this.ApiClient.ExecuteAsync(req);
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
        public async Task Post(myreq model)
        {
            var req = new RequestBase<ResponseBase>(HttpMethod.Put, "http://localhost:5000/", "api/values");
            await this.ApiClient.ExecuteAsync(req);
            await this.ApiClient.ExecuteAsync(req);
            await this.ApiClient.ExecuteAsync(req);
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
