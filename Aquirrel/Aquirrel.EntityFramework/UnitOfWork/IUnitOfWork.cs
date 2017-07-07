using Aquirrel.EntityFramework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Aquirrel.EntityFramework
{
    public interface IUnitOfWork : IDisposable
    {
        void ChangeDatabase(string database);

        int ExecuteSqlCommand(string sql, params object[] parameters);

        IQueryable<TEntity> FromSql<TEntity>(string sql, params object[] parameters) where TEntity : class;
    }
}
