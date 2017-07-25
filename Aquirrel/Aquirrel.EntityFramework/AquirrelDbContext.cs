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
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            Console.WriteLine("AquirrelDbContext.OnModelCreating");
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            Console.WriteLine("AquirrelDbContext.OnConfiguring");
            base.OnConfiguring(optionsBuilder);
        }
    }
}
