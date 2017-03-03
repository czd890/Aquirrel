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

        IQueryable<TEntity> IBaseRepository<TEntity>.Collection { get { return this.Collection.AsQueryable(); } }

        public async Task CreateAsync(TEntity entity)
        {
            await this.Collection.AddAsync(entity);
            await this.DbContext.SaveChangesAsync();
        }
        public void Create(TEntity entity)
        {
            this.Collection.Add(entity);
            this.DbContext.SaveChanges();
        }
        public async Task CreateAsync(IEnumerable<TEntity> entity)
        {
            await this.Collection.AddRangeAsync(entity);
            await this.DbContext.SaveChangesAsync();
        }
        public void Create(IEnumerable<TEntity> entity)
        {
            this.Collection.AddRange(entity);
            this.DbContext.SaveChanges();
        }
        public void Update(TEntity entity)
        {
            this.Collection.Update(entity);
            this.DbContext.SaveChanges();
        }
        public async Task UpdateAsync(TEntity entity)
        {
            this.Collection.Update(entity);
            await this.DbContext.SaveChangesAsync();
        }
        public void Update(IEnumerable<TEntity> entity)
        {
            this.Collection.UpdateRange(entity);
            this.DbContext.SaveChanges();
        }
        public async Task UpdateAsync(IEnumerable<TEntity> entity)
        {
            this.Collection.UpdateRange(entity);
            await this.DbContext.SaveChangesAsync();
        }
    }
}
