using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aquirrel.EntityFramework.Sharding
{
    public class ShardingOptions
    {
        public string ShardingDbValue { get; set; }

        public string ShardingTableValue { get; set; }

        public object Data { get; set; }

        public override string ToString()
        {
            return $"[{ShardingDbValue}][{ShardingTableValue}][{Data}]";
        }
    }
}
