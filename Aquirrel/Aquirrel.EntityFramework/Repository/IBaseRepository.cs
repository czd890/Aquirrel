using Microsoft.EntityFrameworkCore;
using System;
using System.Linq.Expressions;

namespace Aquirrel.EntityFramework
{
    public interface IBaseRepository<TEntity> where TEntity : class
    {
        DbSet<TEntity> Collection { get; }
        //AquirrelDbContext DbContext { get; }

        void Add(TEntity entity);



        TEntity Single(Expression<Func<TEntity, bool>> predicate);
        TEntity SingleOrDefault(Expression<Func<TEntity, bool>> predicate);
        
    }
}