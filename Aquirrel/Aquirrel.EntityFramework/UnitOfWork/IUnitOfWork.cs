using Aquirrel.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Aquirrel.EntityFramework
{
    public interface IUnitOfWork
    {

        IRepository<TEntity> GetRepository<TEntity>() where TEntity : class;
        int SaveChanges();
        //int SaveChanges(params IUnitOfWork[] unitOfWorks);

        Task<int> SaveChangesAsync();
        //Task<int> SaveChangesAsync(params IUnitOfWork[] unitOfWorks);


        int ExecuteSql(string sql, params object[] parameters);

        IQueryable<TEntity> FromSql<TEntity>(string sql, params object[] parameters) where TEntity : class;
    }
}
