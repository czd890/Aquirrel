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

        public IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> predicate = null, bool disableTracking = true)
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

        public IPagedList<TEntity> GetPagedList(Expression<Func<TEntity, bool>> predicate = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, int pageIndex = 0, int pageSize = 20, bool disableTracking = true)
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

        public Task<IPagedList<TEntity>> GetPagedListAsync(Expression<Func<TEntity, bool>> predicate = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, int pageIndex = 0, int pageSize = 20, bool disableTracking = true, CancellationToken cancellationToken = default(CancellationToken))
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

        public IQueryable<TEntity> FromSql(string sql, params object[] parameters) => DbSet.FromSql(sql, parameters);

        public TEntity Find(params object[] keyValues) => DbSet.Find(keyValues);

        public Task<TEntity> FindAsync(params object[] keyValues) => DbSet.FindAsync(keyValues);

        public Task<TEntity> FindAsync(object[] keyValues, CancellationToken cancellationToken) => DbSet.FindAsync(keyValues, cancellationToken);

        public void Add(TEntity entity)
        {
            var entry = DbSet.Add(entity);
        }

        public void Add(params TEntity[] entities) => DbSet.AddRange(entities);

        public void Add(IEnumerable<TEntity> entities) => DbSet.AddRange(entities);

        public Task AddAsync(TEntity entity, CancellationToken cancellationToken = default(CancellationToken))
        {
            return DbSet.AddAsync(entity, cancellationToken);
        }

        public Task AddAsync(params TEntity[] entities) => DbSet.AddRangeAsync(entities);

        public Task AddAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default(CancellationToken)) => DbSet.AddRangeAsync(entities, cancellationToken);

        public void Update(TEntity entity)
        {
            DbSet.Update(entity);
        }

        public void Update(params TEntity[] entities) => DbSet.UpdateRange(entities);

        public void Update(IEnumerable<TEntity> entities) => DbSet.UpdateRange(entities);

        public void Delete(TEntity entity) => DbSet.Remove(entity);

        public void Delete(object id)
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

        public void Delete(params TEntity[] entities) => DbSet.RemoveRange(entities);

        public void Delete(IEnumerable<TEntity> entities) => DbSet.RemoveRange(entities);

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

        public void Before()
        {
            this.DbContext.ChangeTracker.Entries()
                  .Where(p => p.State == EntityState.Modified)
                  .Where(p => p.Entity is ISaveEntityEvent)
                  .Each(p => (p.Entity as ISaveEntityEvent).Before());
        }
    }
}
