using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aquirrel.EntityFramework.Test
{
    [TestClass]
    public class 并发版本测试
    {
        [TestMethod]
        public void Test()
        {
            var sp = new Startup(null).ConfigureServices(null);

            var db = sp.GetService<TestDbContext>();

            var ms = db.ModelA.ToArray();
        }
    }
}
