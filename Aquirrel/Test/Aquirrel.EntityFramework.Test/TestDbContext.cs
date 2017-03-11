using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Aquirrel.EntityFramework.Test
{
    public class TestDbContext : Aquirrel.EntityFramework.AquirrelDbContext
    {
        public TestDbContext(DbContextOptions options) : base(options)
        {

        }
    }
}
