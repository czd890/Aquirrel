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
    [Route("api/[controller]/[action]")]
    public class ValuesController : Controller
    {
        IApiClient ApiClient;
        ILogger _logger;
        public ValuesController(IApiClient apiClient, ILogger<ValuesController> logger)
        {
            this.ApiClient = apiClient;
            this._logger = logger;

        }
        public class MyPostReq : RequestBase<ResponseBase<IEnumerable<string>>>
        {
            public MyPostReq() : base(HttpMethod.Post, "http://localhost:6367", "api/values/postreqggs")
            {

            }
            public List<int> heihei { get; set; }
        }

        public class MyPostThirdReq : RequestBase<ResponseBase<IEnumerable<string>>>
        {
            public MyPostThirdReq() : base(HttpMethod.Post, "http://localhost:6367", "api/values/PostThirdReq")
            {

            }
            public List<int> heihei { get; set; }
        }


        public class MyGettReq : RequestBase<ResponseBase<IEnumerable<string>>>
        {
            public MyGettReq() : base(HttpMethod.Get, "http://localhost:6367", "api/values/getreqAsync")
            {

            }
            public List<int> heihei { get; set; }
        }

        [HttpGet]
        public async Task<IEnumerable<string>> beginreq(string aa)
        {
            //var req = /*(IRequestBase<IResponseBase>)*/new RequestBase<ResponseBase>(HttpMethod.Post, "http://localhost:5000/", "api/values");
            var postReq = new MyPostReq() { heihei = new List<int>() { 1, 2, 3 } };
            var values = await this.ApiClient.ExecuteAsync(postReq);

            var getReq = new MyGettReq() { heihei = new List<int>() { 4, 5, 6 } };

            var v2 = await this.ApiClient.ExecuteAsync<ResponseBase<IEnumerable<string>>, IEnumerable<string>>(getReq);


            var x = values.ToList();
            x.AddRange(v2.data);
            return x;
        }


        [HttpGet]
        public async Task<IResponseBase> getreqAsync([FromQuery]MyGettReq req)
        {

            var postReq = new MyPostThirdReq() { heihei = new List<int>() { 1, 2, 3 } };
            var values = await this.ApiClient.ExecuteAsync(postReq);

            return new ResponseBase<IEnumerable<string>>(new string[] { "value1", "value2" });
        }

        [HttpPost]
        public IResponseBase postreq([FromBody]MyPostReq req)
        {
            this._logger.LogDebug("post--------------------");

            return new ResponseBase<IEnumerable<string>>(new string[] { "value3", "value4" });
        }

        public IResponseBase PostThirdReq([FromBody]MyPostThirdReq req)
        {
            throw new Exception("aaaaaaaaaaaaaa");
            return new ResponseBase<IEnumerable<string>>(new string[] { "value5", "value6" });
        }

        [HttpGet]
        public async Task<ActionResult> xx2Async()
        {
            this._logger.LogError("IIIIIIIIIIIIIIIIIIIIIIII");
            try
            {
                var res = await ApiClient.SendAsync(new HttpRequestMessage()
                {
                    RequestUri = new Uri("http://sadfasf.asga.com"),
                    Method = HttpMethod.Get
                });

            }
            catch (Exception ex)
            {
            }
            return this.Content("ok");
        }
    }
}
