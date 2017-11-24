using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Aquirrel.EntityFramework.Test.project.Entity
{
    public class Order : EntityBase
    {
        public int OrderStatus { get; set; }
    }

    public class Log : EntityBase
    {
        public string msg { get; set; }
    }

    public class Log2 : EntityBase
    {
        [MaxLength]
        public string msg2 { get; set; }
    }
}
