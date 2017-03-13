using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Aquirrel.EntityFramework
{
    public interface IBaseRepository<TEntity>
        where TEntity : class

    {
        IQueryable<TEntity> Collection { get; }
        //AquirrelDbContext DbContext { get; }

        void Create(TEntity entity);
        void Create(IEnumerable<TEntity> entity);
        Task CreateAsync(TEntity entity);
        Task CreateAsync(IEnumerable<TEntity> entity);


        void Update(TEntity entity);
        void Update(IEnumerable<TEntity> entity);
        Task UpdateAsync(TEntity entity);
        Task UpdateAsync(IEnumerable<TEntity> entity);


        void Delete(TEntity entity);
        void Delete(IEnumerable<TEntity> entity);
        Task DeleteAsync(TEntity entity);
        Task DeleteAsync(IEnumerable<TEntity> entity);

    }
}