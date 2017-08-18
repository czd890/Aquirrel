using Aquirrel.EntityFramework.Sharding;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Aquirrel.EntityFramework.Repository;

namespace Aquirrel.EntityFramework
{
    public class RepositoryBase<TEntity> : IRepositoryBase<TEntity>
        where TEntity : class
    {
        public RepositoryBase(IRepository<TEntity> repository)
        {
            this.repository = repository;
        }

        protected IRepository<TEntity> repository;

        public DbContext Instance => throw new NotImplementedException();

        public IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> predicate = null, bool disableTracking = true)
        {
            return this.repository.Query(predicate, disableTracking);
        }

        public IPagedList<TEntity> GetPagedList(
            Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            int pageIndex = 0,
            int pageSize = 20,
            bool disableTracking = true,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null)
        {
            return this.repository.GetPagedList(predicate, orderBy, pageIndex, pageSize, disableTracking, include);
        }

        public Task<IPagedList<TEntity>> GetPagedListAsync(
            Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            int pageIndex = 0,
            int pageSize = 20,
            bool disableTracking = true,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return this.repository.GetPagedListAsync(predicate, orderBy, pageIndex, pageSize, disableTracking, include, cancellationToken);
        }

        public IQueryable<TEntity> FromSql(string sql, params object[] parameters)
        {
            return this.repository.FromSql(sql, parameters);
        }

        public TEntity Find(params object[] keyValues)
        {
            return this.repository.Find(keyValues);
        }

        public Task<TEntity> FindAsync(params object[] keyValues)
        {
            return this.repository.FindAsync(keyValues);
        }

        public Task<TEntity> FindAsync(object[] keyValues, CancellationToken cancellationToken)
        {
            return this.repository.FindAsync(keyValues, cancellationToken);
        }

        public void Add(TEntity entity)
        {
            this.repository.Add(entity);
        }

        public void Add(params TEntity[] entities)
        {
            this.repository.Add(entities);
        }

        public void Add(IEnumerable<TEntity> entities)
        {
            this.repository.Add(entities);
        }

        public Task AddAsync(TEntity entity, CancellationToken cancellationToken = default(CancellationToken))
        {
            return this.repository.AddAsync(entity, cancellationToken);
        }

        public Task AddAsync(params TEntity[] entities)
        {
            return this.repository.AddAsync(entities);
        }

        public Task AddAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default(CancellationToken))
        {
            return this.repository.AddAsync(entities, cancellationToken);
        }

        public void Update(TEntity entity)
        {
            this.repository.Update(entity);
        }

        public void Update(params TEntity[] entities)
        {
            this.repository.Update(entities);
        }

        public void Update(IEnumerable<TEntity> entities)
        {
            this.repository.Update(entities);
        }
    }
}
