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
            ConfigureDbContextEntityService.ConfigureMapping(modelBuilder, _builder, this);
            _builder = null;
            base.OnModelCreating(modelBuilder);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            _builder = optionsBuilder;
            base.OnConfiguring(optionsBuilder);
        }

    }
}
