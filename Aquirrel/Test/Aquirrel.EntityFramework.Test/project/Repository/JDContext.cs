using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Aquirrel.EntityFramework.Test.project.Repository
{
    public class JDContext : AquirrelDbContext
    {
        public JDContext(DbContextOptions<JDContext> options) : base(options)
        {
        }
    }
    //public class JDContext<TEntity> : JDContext
    //{
    //    public JDContext(DbContextOptions<JDContext<TEntity>> options) : base(CTORConvert<JDContext>(options))
    //    {
    //    }
    //}
}
