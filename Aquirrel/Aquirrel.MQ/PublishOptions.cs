using System;
using System.Collections.Generic;
using System.Text;

namespace Aquirrel.MQ
{
    public class PublishOptions
    {
        public bool ShardingConn { get; set; } = true;

        public static PublishOptions Default { get; } = new PublishOptions();
    }
}
