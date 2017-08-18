using Aquirrel.EntityFramework.Repository;
using Aquirrel.EntityFramework.Sharding;
using Aquirrel.EntityFramework.Test.project.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aquirrel.EntityFramework.Test.project.Repository
{
    public class OrderRepo : RepositoryBase<Order>, IOrderRepo
    {
        ShardingDbFactory shardingDbFactory;

        //IRepository<Order> ReadonlyRepo { get { return shardingDbFactory.GetShardingDbContext} }
        public OrderRepo(ShardingDbFactory shardingDbFactory, RepositoryFactory repositoryFactory) : base(repositoryFactory.GetRepository<JDContext, Order>())
        {
            this.shardingDbFactory = shardingDbFactory;
        }
    }
}
