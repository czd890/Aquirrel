using System;
using System.Collections.Generic;
using System.Text;

namespace Aquirrel.EntityFramework.Shard
{
    public interface ShardRule
    {

    }
    public interface ShardRule<TEntity>: ShardRule
    {
        string GetTableName(TEntity entity);
    }
}
