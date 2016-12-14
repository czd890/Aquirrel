using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Aquirrel.EntityFramework
{
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
    {
        public BaseRepository(AquirrelDbContext dbContext)
        {
            this.DbContext = dbContext;
        }
        public AquirrelDbContext DbContext { get; private set; }

        public DbSet<TEntity> Collection { get { return this.DbContext.Set<TEntity>(); } }

        public void Add(TEntity entity)
        {
            this.Collection.Add(entity);
            this.DbContext.SaveChanges();
        }

        public TEntity Single(Expression<Func<TEntity, bool>> predicate)
        {
            return this.Collection.Single(predicate);
        }
        public TEntity SingleOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return this.Collection.SingleOrDefault(predicate);
        }
    }
}
