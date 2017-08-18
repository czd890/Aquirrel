using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.DependencyInjection;
using Aquirrel.EntityFramework.Sharding;

namespace Aquirrel.EntityFramework.Test
{
    public class TestDbContext : AquirrelDbContext
    {
        public TestDbContext(DbContextOptions options) : base(options)
        {
        }
        //public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
        //{
        //    Console.WriteLine("TestDbContext ctor(options)");
        //}

        public DbSet<ModelA> ModelASet { get; set; }
    }

    public class LogDbContext : AquirrelDbContext
    {
        //public LogDbContext(DbContextOptions options) : base(options) { }
        public LogDbContext(DbContextOptions<LogDbContext> options) : base(options) {
        }

    }

    public class LogDbContext<TEntity> : LogDbContext
    {
        public LogDbContext(DbContextOptions<LogDbContext<TEntity>> options) : base(new DbContextOptions<LogDbContext>(options.Extensions.ToDictionary(p => p.GetType(), p => p)))
        {
        }
        //public LogDbContext(DbContextOptions<LogDbContext> options) : base(options) { }
    }

    public class testDbIDesignTimeDbContextFactory : IDesignTimeDbContextFactory<LogDbContext>
    {
        public LogDbContext CreateDbContext(string[] args)
        {
            var sp = new Startup(null).ConfigureServices(new ServiceCollection());
            var db = sp.GetService<LogDbContext>();

            return db;
        }
    }
    public class logDbIDesignTimeDbContextFactory : IDesignTimeDbContextFactory<TestDbContext>
    {
        public TestDbContext CreateDbContext(string[] args)
        {
            var sp = new Startup(null).ConfigureServices(new ServiceCollection());
            var db = sp.GetService<TestDbContext>();

            return db;
        }
    }
}
