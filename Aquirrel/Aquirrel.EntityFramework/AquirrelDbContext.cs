using Aquirrel.EntityFramework.Internal;
using Aquirrel.EntityFramework.Sharding;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Aquirrel.EntityFramework
{
    /// <summary>
    /// ef上下文对象
    /// </summary>
    public class AquirrelDbContext : DbContext
    {
        protected static DbContextOptions<TContext> CTORConvert<TContext>(DbContextOptions options)
           where TContext : DbContext
        {
            return new DbContextOptions<TContext>(options.Extensions.ToDictionary(p => p.GetType(), p => p));
        }

        public AquirrelDbContext(DbContextOptions options) : base(options)
        {
        }
        //public AquirrelDbContext(Sharding.ShardingDbContextOptions options,IServiceProvider provider)
        //    : this(provider.GetRequiredService<ShardingDbContextFactory>().GetShardingDbContextOptions(options)) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            Console.WriteLine("AquirrelDbContext.OnModelCreating");
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            Console.WriteLine("AquirrelDbContext.OnConfiguring");
            base.OnConfiguring(optionsBuilder);
        }
    }
}