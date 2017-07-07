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


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Model.AddAnnotation("MaxLength", 32);

            modelBuilder.Entity<ModelA>().Property(p => p.Name).IsMaxLength();

            foreach (var item in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var item2 in item.GetProperties())
                {
                    var ss = $"table:{item.Name}; propertyName:{item2.Name}; properyType:{item2.ClrType.ToString()}";
                    Console.WriteLine(ss);

                    foreach (var item3 in item2.GetAnnotations())
                    {
                        Console.WriteLine($"{item3.Name}---{item3.Value}");
                    }
                    //扩展自定义方法，maxlenght
                    var IsMaxLength = item2.FindAnnotation(Extend.AnnotationName);
                    if ((IsMaxLength == null || !(bool)IsMaxLength.Value)&& item2.FindAnnotation("MaxLength") ==null&&item2.ClrType==typeof(string)) item2.AddAnnotation("MaxLength", 32);
                }
            }
            //Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal.CoreConventionSetBuilder s;s.CreateConventionSet().PropertyAddedConventions.Add()
            
            base.OnModelCreating(modelBuilder);
        }
    }
    public class DefaultStringLength : IPropertyAddedConvention
    {
        public InternalPropertyBuilder Apply(InternalPropertyBuilder propertyBuilder)
        {
            //propertyBuilder.

            return propertyBuilder;
        }
    }

    public class xx : IDesignTimeDbContextFactory<TestDbContext>
    {
        public TestDbContext CreateDbContext(string[] args)
        {
            Console.WriteLine(args.ToJson());
            DbContextOptionsBuilder<TestDbContext> s = new DbContextOptionsBuilder<TestDbContext>();
            s.UseSqlServer("server=172.16.100.172;database=efcoretest;uid=sa_test;pwd=123456;");
            return new TestDbContext(s.Options);
        }
    }
    public static class Extend
    {
        public static string AnnotationName = "IsMaxLength";
        public static void IsMaxLength<TProperty>(this PropertyBuilder<TProperty> propertyBuilder, bool? ismaxLen = true)
        {
            propertyBuilder.HasAnnotation(AnnotationName, ismaxLen);
        }
    }
}
