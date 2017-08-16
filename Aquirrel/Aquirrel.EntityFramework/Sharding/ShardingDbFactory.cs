using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Aquirrel.EntityFramework.Internal;
using System.Linq;
using System.Collections.Concurrent;

namespace Aquirrel.EntityFramework.Sharding
{
    /// <summary>
    /// DI Scope
    /// </summary>
    public class ShardingDbFactory
    {
        IServiceProvider provider;

        public ShardingDbFactory(IServiceProvider provider)
        {
            this.provider = provider;
        }
        //public IRepository<TEntity> GetShardingRepository<TContext, TEntity>(ShardingOptions options)
        //    where TContext : AquirrelDbContext<TEntity>
        //    where TEntity : class
        //{
        //    return null;
        //}

        public IRepository<TEntity> GetShardingRepository<TContext, TEntity>(ShardingOptions options)
            where TContext : DbContext
            where TEntity : class
        {
            if (options == null)
                return provider.GetRequiredService<Repository<TContext, TEntity>>();

            var t = typeof(TContext);

            var key = $"[sharding repo][{typeof(TContext).FullName}][{typeof(TEntity).Name}[{options}]]";
            TContext db = null;
            if (options.ShardingTableValue.IsNotNullOrEmpty() && typeof(TContext).IsGenericType)
            {
                var tableName = this.GetShardingTableName<TContext, TEntity>(options);
                if (tableName.IsNotNullOrEmpty())
                {
                    db = this.GetShardingDbContext<TContext>(options);
                    var xxx = db.Model.FindEntityType(typeof(TEntity));
                    var x2 = xxx.Relational();
                    if (db.Model.FindEntityType(typeof(TEntity)).Relational()
                        is Microsoft.EntityFrameworkCore.Metadata.RelationalEntityTypeAnnotations extensions)
                        extensions.TableName += "_" + tableName;
                }
            }
            else if (options.ShardingDbValue.IsNotNullOrEmpty())

                db = this.GetShardingDbContext<TContext>(options);
            else
                throw new NotImplementedException();

            return (IRepository<TEntity>)this.provider.GetRequiredService<InternalScopeServiceContainer>()
                .GetOrAdd(key, k => new Repository<TContext, TEntity>(db));

        }

        public TContext GetShardingDbContext<TContext>(ShardingOptions options)
            where TContext : DbContext
        {


            var key = $"[sharding dbcontext][{typeof(TContext).FullName}][{options}]";

            return (TContext)this.provider.GetRequiredService<InternalScopeServiceContainer>()
                .GetOrAdd(key, k =>
                {
                    Type dbContextType = typeof(TContext).IsGenericType ? typeof(TContext).BaseType : typeof(TContext);
                    var optionsType = typeof(DbContextOptions<>).MakeGenericType(dbContextType);

                    var dbOptionsExtensions = ((DbContextOptions)this.provider.GetRequiredService(optionsType))
                    .Extensions.ToDictionary(p => p.GetType(), p => p);


                    var builder = new DbContextOptionsBuilder<TContext>(new DbContextOptions<TContext>(dbOptionsExtensions));

                    builder = this.BuilderDbContextOptions<TContext>(builder, options);

                    return (TContext)Activator.CreateInstance(typeof(TContext), builder.Options);
                });
        }

        public virtual DbContextOptionsBuilder<TContext> BuilderDbContextOptions<TContext>(DbContextOptionsBuilder<TContext> builder, ShardingOptions options)
            where TContext : DbContext
        {
            return builder;
        }

        public virtual string GetShardingTableName<TContext, TEntity>(ShardingOptions options)
        {
            return null;
        }
    }
}
