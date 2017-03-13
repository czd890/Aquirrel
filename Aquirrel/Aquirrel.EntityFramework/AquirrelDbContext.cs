using Aquirrel.EntityFramework.Internal;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aquirrel.EntityFramework
{
    public class AquirrelDbContext : DbContext
    {
        public AquirrelDbContext(DbContextOptions options) : base(options)
        {

        }
        DbContextOptionsBuilder _builder;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            Console.WriteLine("AquirrelDbContext.OnModelCreating");
            ConfigureDbContextEntityService.ConfigureMapping(modelBuilder, _builder, this);
            _builder = null;
            
            base.OnModelCreating(modelBuilder);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            Console.WriteLine("AquirrelDbContext.OnConfiguring");
            _builder = optionsBuilder;
            base.OnConfiguring(optionsBuilder);
            //Console.WriteLine("AquirrelDbContext.OnConfiguring ReplaceService");
            //optionsBuilder.ReplaceService<Microsoft.EntityFrameworkCore.Infrastructure.Internal.SqlServerModelSource, AquirrelDbModelSource>();
        }
    }
}
