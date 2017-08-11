using Aquirrel.EntityFramework.Shard;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.EntityFrameworkCore.Metadata
{
    public class ShardRuleAnnotation
    {

        ShardRule shardRule;
        public ShardRuleAnnotation(ShardRule shardRule)
        {
            this.shardRule = shardRule;
        }

        public static implicit operator string(ShardRuleAnnotation shardRuleAnnotation)
        {
            return "";// shardRuleAnnotation.shardRule
        }
    }
}
