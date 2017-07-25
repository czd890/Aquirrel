using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aquirrel.EntityFramework.Test
{
    [TestClass]

    public class 数据库测试
    {
        [TestMethod]
        public void 测试全局自定义规则应用情况()
        {
            var sp = new Startup(null).ConfigureServices(new ServiceCollection());
            var _db = sp.GetService<TestDbContext>();

            //var obj = sp.GetService<Microsoft.EntityFrameworkCore.Infrastructure.Internal.SqlServerModelSource>();
            //Console.WriteLine(obj.GetType().FullName);
            var iscreated = _db.ModelASet.ToList().ToJson();
            Console.WriteLine(iscreated);

        }

        //public class TemporaryDbContextFactory : IDbContextFactory<TestDbContext>
        //{

        //    public TestDbContext Create(DbContextFactoryOptions options)
        //    {
        //        //var sp = new ServiceCollection().AddAquirrelDb<db>().BuildServiceProvider();
        //        //var opx = new DbContextOptionsBuilder<db>()
        //        //        .UseSqlServer(
        //        //        new ConfigurationBuilder()
        //        //        .SetBasePath(Directory.GetCurrentDirectory())
        //        //        .AddJsonFile("appsettings.json")
        //        //        .Build()
        //        //        .GetConnectionString("jiangzhi")
        //        //        ).UseInternalServiceProvider(sp).Options;


        //        var sp = new Startup(null).ConfigureServices(new ServiceCollection());
        //        return sp.GetService<TestDbContext>();
        //    }
        //}

    }
}
