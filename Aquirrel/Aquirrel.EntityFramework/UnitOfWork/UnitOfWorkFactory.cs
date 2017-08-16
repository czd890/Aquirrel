using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aquirrel.EntityFramework
{
    public class UnitOfWorkFactory
    {
        IServiceProvider provider;
        public UnitOfWorkFactory(IServiceProvider provider)
        {
            this.provider = provider;
        }
        public IUnitOfWork GetUnitOfWork<TContext>()
            where TContext : Microsoft.EntityFrameworkCore.DbContext
        {
            return provider.GetRequiredService<Internal.UnitOfWork<TContext>>();
        }
    }
}
