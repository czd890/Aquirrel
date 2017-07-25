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

namespace Aquirrel.EntityFramework.Test
{
    public class TestDbContext :
        //Microsoft.EntityFrameworkCore.DbContext
    Aquirrel.EntityFramework.AquirrelDbContext
    {
        public TestDbContext(DbContextOptions options) : base(options)
        {
            Console.WriteLine("TestDbContext ctor(options)");
        }

        public DbSet<ModelA> ModelASet { get; set; }
    }

    public class xx : IDesignTimeDbContextFactory<TestDbContext>
    {
        public TestDbContext CreateDbContext(string[] args)
        {
            var sp = new Startup(null).ConfigureServices(new ServiceCollection());
            var db = sp.GetService<TestDbContext>();

            return db;
        }
    }
}
