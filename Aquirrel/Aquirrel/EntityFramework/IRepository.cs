using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Aquirrel.EntityFramework.Repository
{
    /// <summary>
    /// 基础存储
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IRepositoryBase<TEntity> where TEntity : class
    {
        /// <summary>
        /// 查询方法
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <param name="disableTracking">是否跟踪实体，true：不跟踪，false：跟踪。默认true</param>
        /// <returns></returns>
        IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> predicate = null, bool disableTracking = true);
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <param name="orderBy">排序方式</param>
        /// <param name="pageIndex">页序号</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="disableTracking">是否跟踪实体，true：不跟踪，false：跟踪。默认true</param>
        /// <returns></returns>
        IPagedList<TEntity> GetPagedList(Expression<Func<TEntity, bool>> predicate = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, int pageIndex = 0, int pageSize = 20, bool disableTracking = true);
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <param name="orderBy">排序方式</param>
        /// <param name="pageIndex">页序号</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="disableTracking">是否跟踪实体，true：不跟踪，false：跟踪。默认true</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<IPagedList<TEntity>> GetPagedListAsync(Expression<Func<TEntity, bool>> predicate = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, int pageIndex = 0, int pageSize = 20, bool disableTracking = true, CancellationToken cancellationToken = default(CancellationToken));
        /// <summary>
        /// 原始sql查询
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        IQueryable<TEntity> FromSql(string sql, params object[] parameters);
        /// <summary>
        /// 根据主键查询实体
        /// </summary>
        /// <param name="keyValues"></param>
        /// <returns></returns>
        TEntity Find(params object[] keyValues);
        /// <summary>
        /// 根据主键查询实体
        /// </summary>
        /// <param name="keyValues"></param>
        /// <returns></returns>
        Task<TEntity> FindAsync(params object[] keyValues);
        /// <summary>
        /// 根据主键查询实体
        /// </summary>
        /// <param name="keyValues"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<TEntity> FindAsync(object[] keyValues, CancellationToken cancellationToken);
        /// <summary>
        /// 重新从数据源加载数据
        /// </summary>
        /// <param name="entity"></param>
        void Reload(TEntity entity);
        /// <summary>
        /// 重新从数据源加载数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task ReloadAsync(TEntity entity);
        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="entity"></param>
        void Add(TEntity entity);
        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="entities"></param>
        void Add(params TEntity[] entities);
        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="entities"></param>
        void Add(IEnumerable<TEntity> entities);
        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task AddAsync(TEntity entity, CancellationToken cancellationToken = default(CancellationToken));
        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        Task AddAsync(params TEntity[] entities);
        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task AddAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default(CancellationToken));
        /// <summary>
        /// 修改实体
        /// </summary>
        /// <param name="entity"></param>
        void Update(TEntity entity);
        /// <summary>
        /// 修改实体
        /// </summary>
        /// <param name="entities"></param>
        void Update(params TEntity[] entities);
        /// <summary>
        /// 修改实体
        /// </summary>
        /// <param name="entities"></param>
        void Update(IEnumerable<TEntity> entities);
    }
    /// <summary>
    /// 基础存储删除操作
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IRepositoryDelete<TEntity> where TEntity : class
    {
        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="id"></param>
        void Delete(object id);
        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="ids"></param>
        void Delete(params object[] ids);
        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="entity"></param>
        void Delete(TEntity entity);
        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="entities"></param>
        void Delete(params TEntity[] entities);
        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="entities"></param>
        void Delete(IEnumerable<TEntity> entities);
    }
    /// <summary>
    /// 基础存储持久化操作
    /// </summary>
    public interface IPersistence
    {
        /// <summary>
        /// 保存已跟踪的改动
        /// </summary>
        /// <returns></returns>
        int SaveChanges();
        /// <summary>
        /// 保存已跟踪的改动
        /// </summary>
        /// <param name="acceptAllChangesOnSuccess"></param>
        /// <returns></returns>
        int SaveChanges(bool acceptAllChangesOnSuccess);
        /// <summary>
        /// 保存已跟踪的改动
        /// </summary>
        /// <param name="acceptAllChangesOnSuccess"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken));
        /// <summary>
        /// 保存已跟踪的改动
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}