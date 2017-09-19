using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace Aquirrel.EntityFramework.Repository
{
    public class RepositoryFactory
    {
        IServiceProvider provider;
        public RepositoryFactory(IServiceProvider provider)
        {
            this.provider = provider;
        }
        public IRepositoryBase<TEntity> GetRepository<TContext, TEntity>()
            where TContext : DbContext
            where TEntity : class
        {
            return this.provider.GetService<Repository<TContext, TEntity>>();
        }
    }
}
