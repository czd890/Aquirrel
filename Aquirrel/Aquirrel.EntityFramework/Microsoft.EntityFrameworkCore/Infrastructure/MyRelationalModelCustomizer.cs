using Aquirrel.EntityFramework.Internal;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;

namespace Microsoft.EntityFrameworkCore.Infrastructure
{
    public class MyRelationalModelCustomizer : RelationalModelCustomizer
    {
        public MyRelationalModelCustomizer(ModelCustomizerDependencies dependencies)
            : base(dependencies)
        {
            Console.WriteLine("MyRelationalModelCustomizer ctor");
        }

        public override void Customize(ModelBuilder modelBuilder, DbContext dbContext)
        {
            Console.WriteLine("MyRelationalModelCustomizer.Customize");
            base.Customize(modelBuilder, dbContext);
            //var sp = (IInfrastructure<IServiceProvider>)dbContext;
            //var dbOptions = sp.Instance.GetServices<DbContextOptions>();
            //foreach (var item in dbOptions)
            //{
            //    Console.WriteLine($"test=====================ss=={item}.context:{item.ContextType}");
            //    if (item.ContextType == dbContext.GetType())
            //        ConfigureDbContextEntityService.Configure(modelBuilder, item, dbContext);
            //}
            var dbcontextType = dbContext.GetType();
            while (true)
            {
                if (dbcontextType == typeof(DbContext))
                    break;
                dbcontextType = dbcontextType.BaseType;
            }
            var dbOptions = (DbContextOptions)dbcontextType.GetField("_options", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.GetField | System.Reflection.BindingFlags.Instance).GetValue(dbContext);
            ConfigureDbContextEntityService.Configure(modelBuilder, dbOptions, dbContext);
        }
    }
}
