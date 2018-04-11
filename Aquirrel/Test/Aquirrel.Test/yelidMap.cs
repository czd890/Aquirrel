using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

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

            
        }
    }
}
