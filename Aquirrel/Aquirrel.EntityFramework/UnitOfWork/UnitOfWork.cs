using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Aquirrel.EntityFramework.Internal
{
    public class UnitOfWork<TContext> : IUnitOfWork where TContext : DbContext
    {
        private readonly TContext _context;
        private IServiceProvider _serviceProvider;

        public UnitOfWork(IServiceProvider serviceProvider)
        {
            this._serviceProvider = serviceProvider;
            this._context = this._serviceProvider.GetRequiredService<TContext>();
        }

        public IRepository<TEntity> GetRepository<TEntity>() where TEntity : class
        {
            return this._serviceProvider.GetRequiredService<Repository<TContext, TEntity>>();
        }

        public int ExecuteSql(string sql, params object[] parameters)
            => _context.Database.ExecuteSqlCommand(sql, parameters);

        public IQueryable<TEntity> FromSql<TEntity>(string sql, params object[] parameters) where TEntity : class
            => _context.Set<TEntity>().FromSql(sql, parameters);

        public int SaveChanges()
            => _context.SaveChanges();

        public async Task<int> SaveChangesAsync()
            => await _context.SaveChangesAsync();

    }
}
