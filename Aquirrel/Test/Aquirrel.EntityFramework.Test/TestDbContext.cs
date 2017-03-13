using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Aquirrel.EntityFramework.Test
{
    public class TestDbContext : Aquirrel.EntityFramework.AquirrelDbContext
    {
        public TestDbContext(DbContextOptions options) : base(options)
        {
            Console.WriteLine("TestDbContext ctor(options)");
        }

        public DbSet<ModelA> ModelA { get; set; }
        public DbSet<ModelB> ModelB { get; set; }
        public DbSet<ModelC> ModelC { get; set; }

    }
}
