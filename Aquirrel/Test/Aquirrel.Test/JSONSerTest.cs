using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Aquirrel.ResetApi;
namespace Aquirrel.Test
{
    [TestClass]
    public class JSONSerTest
    {

        public class myres : Aquirrel.ResetApi.ResponseBase
        {
            public string resp { get; set; }
        }
        public class myreq : Aquirrel.ResetApi.RequestBase<myres>
        {
            public myreq() : base(System.Net.Http.HttpMethod.Get, "http://", "/api/controller/name")
            {

            }
            public string p1 { get; set; }
        }
        [TestMethod]
        public void MyTestMethod()
        {
            IRequest r = new myreq();
            var s = r.ToJson();
            Console.WriteLine(s);
        }
    }
}
