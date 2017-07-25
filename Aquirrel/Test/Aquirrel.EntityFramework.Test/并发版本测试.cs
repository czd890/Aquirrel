using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;

namespace Aquirrel.EntityFramework.Test
{
    [TestClass]
    public class 并发版本测试
    {
        [TestMethod]
        public void Test()
        {
            var serviceCollection = new ServiceCollection();
            var sp = new Startup(null).ConfigureServices(serviceCollection);

            serviceCollection.AsQueryable()
                .Where(p => p.ServiceType.ToString().StartsWith("Microsoft.EntityFrameworkCore"))
                .Each(sd =>
            {
                Console.WriteLine($"{sd.Lifetime.ToString().PadRight(15, ' ')}{sd.ServiceType.FullName}");
            });


            var xx = sp.GetService<IModelCustomizer>();

            var xx2 = sp.GetService<ICoreConventionSetBuilder>();

            var db = sp.GetService<TestDbContext>();

            var ms = db.ModelASet.ToArray();
            
            var ms2 = db.Set<AutoEntryTable>().ToArray();
            Console.WriteLine("finish");

        }
    }
}
