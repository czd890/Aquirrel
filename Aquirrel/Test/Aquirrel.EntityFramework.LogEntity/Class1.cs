using System;
using System.ComponentModel.DataAnnotations;

namespace Aquirrel.EntityFramework.LogEntity
{
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
