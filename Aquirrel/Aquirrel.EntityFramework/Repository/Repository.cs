using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Aquirrel.EntityFramework.Repository
{
    public class Repository<TContext, TEntity> : IRepositoryBase<TEntity>, IRepositoryDelete<TEntity>, IPersistence, ISaveEntityEvent
        where TContext : DbContext
        where TEntity : class
    {
        protected Type EntityType { get; } = typeof(TEntity);

        protected TContext DbContext { get; private set; }
        protected DbSet<TEntity> DbSet { get; private set; }

        //public DbContext DbContext => this._dbContext;

        //DbContext IRepository.DbConext => this._dbContext;

        //DbContext IInfrastructure<DbContext>.Instance => this._dbContext;

        public Repository(TContext dbContext)
        {
            DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            DbSet = DbContext.Set<TEntity>();
        }

        public void ChangeTable(string table)
        {
            if (DbContext.Model.FindEntityType(typeof(TEntity)).Relational() is RelationalEntityTypeAnnotations relational)
            {
                relational.TableName = table;
            }
        }

        public virtual IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> predicate = null, bool disableTracking = true)
        {
            IQueryable<TEntity> set;
            if (disableTracking)
            {
                set = DbSet.AsNoTracking();
            }
            else
            {
                set = DbSet;
            }

            if (predicate != null)
            {
                set = set.Where(predicate);
            }

            return set;
        }

        public virtual IPagedList<TEntity> GetPagedList(Expression<Func<TEntity, bool>> predicate = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, int pageIndex = 0, int pageSize = 20, bool disableTracking = true)
        {
            IQueryable<TEntity> query = DbSet;
            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (orderBy != null)
            {
                return orderBy(query).ToPagedList(pageIndex, pageSize);
            }
            else
            {
                return query.ToPagedList(pageIndex, pageSize);
            }
        }

        public virtual Task<IPagedList<TEntity>> GetPagedListAsync(Expression<Func<TEntity, bool>> predicate = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, int pageIndex = 0, int pageSize = 20, bool disableTracking = true, CancellationToken cancellationToken = default(CancellationToken))
        {
            IQueryable<TEntity> query = DbSet;
            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (orderBy != null)
            {
                return orderBy(query).ToPagedListAsync(pageIndex, pageSize, 0, cancellationToken);
            }
            else
            {
                return query.ToPagedListAsync(pageIndex, pageSize, 0, cancellationToken);
            }
        }

        public virtual IQueryable<TEntity> FromSql(string sql, params object[] parameters) => DbSet.FromSql(sql, parameters);

        public virtual TEntity Find(params object[] keyValues) => DbSet.Find(keyValues);

        public virtual Task<TEntity> FindAsync(params object[] keyValues) => DbSet.FindAsync(keyValues);

        public virtual Task<TEntity> FindAsync(object[] keyValues, CancellationToken cancellationToken) => DbSet.FindAsync(keyValues, cancellationToken);

        public virtual void Add(TEntity entity)
        {
            var entry = DbSet.Add(entity);
        }

        public virtual void Add(params TEntity[] entities) => DbSet.AddRange(entities);

        public virtual void Add(IEnumerable<TEntity> entities) => DbSet.AddRange(entities);

        public virtual Task AddAsync(TEntity entity, CancellationToken cancellationToken = default(CancellationToken))
        {
            return DbSet.AddAsync(entity, cancellationToken);
        }

        public virtual Task AddAsync(params TEntity[] entities) => DbSet.AddRangeAsync(entities);

        public virtual Task AddAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default(CancellationToken)) => DbSet.AddRangeAsync(entities, cancellationToken);

        public virtual void Update(TEntity entity)
        {
            DbSet.Update(entity);
        }

        public virtual void Update(params TEntity[] entities) => DbSet.UpdateRange(entities);

        public virtual void Update(IEnumerable<TEntity> entities) => DbSet.UpdateRange(entities);

        public virtual void Delete(TEntity entity) => DbSet.Remove(entity);

        public virtual void Delete(object id)
        {
            // using a stub entity to mark for deletion
            var typeInfo = typeof(TEntity).GetTypeInfo();
            var key = DbContext.Model.FindEntityType(typeInfo.Name).FindPrimaryKey().Properties.FirstOrDefault();
            var property = typeInfo.GetProperty(key?.Name);
            if (property != null)
            {
                var entity = Activator.CreateInstance<TEntity>();
                property.SetValue(entity, id);
                DbContext.Entry(entity).State = EntityState.Deleted;
            }
            else
            {
                var entity = DbSet.Find(id);
                if (entity != null)
                {
                    Delete(entity);
                }
            }
        }

        public virtual void Delete(params TEntity[] entities) => DbSet.RemoveRange(entities);

        public virtual void Delete(IEnumerable<TEntity> entities) => DbSet.RemoveRange(entities);

        int IPersistence.SaveChanges()
        {
            this.Before();
            return this.DbContext.SaveChanges();
        }

        int IPersistence.SaveChanges(bool acceptAllChangesOnSuccess)
        {
            this.Before();
            return this.DbContext.SaveChanges(acceptAllChangesOnSuccess);
        }

        Task<int> IPersistence.SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken)
        {
            this.Before();
            return this.DbContext.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        Task<int> IPersistence.SaveChangesAsync(CancellationToken cancellationToken)
        {
            this.Before();
            return this.DbContext.SaveChangesAsync(cancellationToken);
        }

        public virtual void Before()
        {
            this.DbContext.ChangeTracker.Entries()
                  .Where(p => p.State == EntityState.Modified)
                  .Where(p => p.Entity is ISaveEntityEvent)
                  .Each(p => (p.Entity as ISaveEntityEvent).Before());
        }
    }
}
