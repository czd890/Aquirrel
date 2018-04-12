using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using Aquirrel.EntityFramework;
namespace Aquirrel.Test
{
    [TestClass]
    public class yelidMap
    {
        class A
        {
            public string Pro { get; set; }
        }
        [TestMethod]
        public void yelidMap_each()
        {
            List<A> arr = new List<A>();
            arr.Add(new A() { Pro = "1" });
            arr.Add(new A() { Pro = "2" });
            arr.Add(new A() { Pro = "3" });
            arr.Add(new A() { Pro = "4" });
            arr.Add(new A() { Pro = "5" });
            arr.Add(new A() { Pro = "6" });
            arr.Add(new A() { Pro = "7" });
            arr.Add(new A() { Pro = "8" });
            arr.Add(new A() { Pro = "9" });
            arr.Add(new A() { Pro = "10" });

            var result = arr.ToPagedList(0, 2);
            var mr = result.Map(a => { Console.WriteLine("conv:" + a.Pro); return new A() { Pro = "MAP:" + a.Pro }; });
            foreach (var item in mr.Items)
            {
                Console.WriteLine("each");
                item.Pro += "each";
            }
            int i = 0;
            foreach (var item in mr.Items)
            {
                i++;
                Assert.AreEqual(item.Pro, $"MAP:{i}each");
            }
        }
    }
}
