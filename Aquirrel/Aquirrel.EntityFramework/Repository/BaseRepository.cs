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
        public BaseRepository(DbContext dbContext)
        {
            this.DbContext = dbContext;
        }
        protected DbContext DbContext { get; private set; }

        protected DbSet<TEntity> Collections { get { return this.DbContext.Set<TEntity>(); } }

        public IQueryable<TEntity> Collection { get { return this.Collections.AsQueryable(); } }

        public async Task CreateAsync(TEntity entity)
        {
            await this.Collections.AddAsync(entity);
            await this.DbContext.SaveChangesAsync();
        }
        public void Create(TEntity entity)
        {
            this.Collections.Add(entity);
            this.DbContext.SaveChanges();
        }
        public async Task CreateAsync(IEnumerable<TEntity> entity)
        {
            await this.Collections.AddRangeAsync(entity);
            await this.DbContext.SaveChangesAsync();
        }
        public void Create(IEnumerable<TEntity> entity)
        {
            this.Collections.AddRange(entity);
            this.DbContext.SaveChanges();
        }
        public void Update(TEntity entity)
        {
            this.Collections.Update(entity);
            this.DbContext.SaveChanges();
        }
        public async Task UpdateAsync(TEntity entity)
        {
            this.Collections.Update(entity);
            await this.DbContext.SaveChangesAsync();
        }
        public void Update(IEnumerable<TEntity> entity)
        {
            this.Collections.UpdateRange(entity);
            this.DbContext.SaveChanges();
        }
        public async Task UpdateAsync(IEnumerable<TEntity> entity)
        {
            this.Collections.UpdateRange(entity);
            await this.DbContext.SaveChangesAsync();
        }
    }
}
