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
        [TestMethod]
        public void timestamp()
        {
            var s = Aquirrel.IdBuilder.TimeStampUTC();
            Console.WriteLine(s);
        }

        [TestMethod]
        public void IdWorker()
        {
            var s = new IdWorker(1, 1).nextId();
            Console.WriteLine(s);
        }

        [TestMethod]
        public void getid()
        {
            var id = IdBuilder.NextStringId();
            var id2 = IdBuilder.NextStringId();
            var oid = new Aquirrel.ObjectId(id);
            
            Console.WriteLine(id);
        }

    }
}
