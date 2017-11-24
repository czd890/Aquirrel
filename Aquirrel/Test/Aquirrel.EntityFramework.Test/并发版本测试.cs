using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;
using Aquirrel.EntityFramework.Sharding;
using Microsoft.EntityFrameworkCore;
using Aquirrel.EntityFramework.Internal;
using Aquirrel.EntityFramework.Repository;
using AutoMapper;

namespace Aquirrel.EntityFramework.Test
{
    [TestClass]
    public class 并发版本测试
    {
        [TestMethod]
        public void Test()
        {
            var serviceCollection = new ServiceCollection();
            var sp = new Startup(null).ConfigureServices(serviceCollection);

            serviceCollection.AsQueryable()
                .Where(p => p.ServiceType.ToString().StartsWith("Microsoft.EntityFrameworkCore"))
                .Each(sd =>
            {
                Console.WriteLine($"{sd.Lifetime.ToString().PadRight(15, ' ')}{sd.ServiceType.FullName}");
            });


            var xx = sp.GetService<IModelCustomizer>();

            var xx2 = sp.GetService<ICoreConventionSetBuilder>();

            var db = sp.GetService<TestDbContext>();
            var hasC = db.Database.EnsureCreated();
            var ms = db.ModelASet.ToArray();

            var ms2 = db.Set<ShardTable>().ToArray();



            Console.WriteLine("finish");

        }

        [TestMethod]
        public void add()
        {
            var sp = new Startup(null).ConfigureServices(new ServiceCollection());

            var db = sp.GetService<TestDbContext>();
            var m = new ShardTable()
            {
                DefaultName = "hahahah",
            };

            db.Set<ShardTable>().Add(m);
            db.SaveChanges();

        }

        [TestMethod]
        public void get()
        {
            var sp = new Startup(null).ConfigureServices(new ServiceCollection());

            var db = sp.GetService<TestDbContext>();

            var m = db.Set<ShardTable>().Find("201707261707414299");
            Console.WriteLine(m.ToJson());
        }

        [TestMethod]
        public void pagelist_map()
        {
            var sp = new Startup(null).ConfigureServices(new ServiceCollection());

            var db = sp.GetService<TestDbContext>();

            var rep = sp.GetRequiredService<Repository<TestDbContext, ShardTable>>();
            var pl = rep.GetPagedList();

            var pl2 = pl.Map(s =>
            {
                var r = new Exception(s.StringId);
                Console.WriteLine("map");
                return r;
            });
            Console.WriteLine("---");
            foreach (var item in pl2.Items)
            {
                Console.WriteLine(item.Message);
            }
            var pl3 = pl2.Map(s => new ShardTable(s.Message));

            Console.WriteLine("-----------");
        }

        class shardTable_map
        {
            public string DefaultName { get; set; }
        }
        class map_class : AutoMapper.AutoMapperConfiguration
        {
            public override void Configure()
            {
                this.CreateMap<shardTable_map, ShardTable>();
            }
        }
        [TestMethod]
        public void entity_map()
        {

            try
            {
                var sc = new ServiceCollection();
                sc.AddAutoMapperProfile<map_class>();
                sc.ConfigureAutoMapper();
                var sp = sc.BuildServiceProvider();
                var dto = new shardTable_map()
                {
                    DefaultName = "33333333333333333333333"
                };
                sp.GetService<IMapper>();

                var entity = dto.Map<shardTable_map, ShardTable>();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                //throw;
            }
        }

        [TestMethod]
        public void update()
        {
            var sp = new Startup(null).ConfigureServices(new ServiceCollection());

            var db = sp.GetService<TestDbContext>();

            var m = db.Set<ShardTable>().Find("201707261707414299");

            m.MaxName = DateTime.Now.ToString();
            db.SaveChanges();

        }

        [TestMethod]
        public void orgGetRepo()
        {
            var sp = new Startup(null).ConfigureServices(new ServiceCollection());

            sp.GetRequiredService<Repository<LogDbContext, project.Entity.Log>>();

            sp.GetRequiredService<Repository<TestDbContext, ShardTable>>();
        }


        [TestMethod]
        public void TestShardingDb()
        {


            var sp = new Startup(null).ConfigureServices(new ServiceCollection());

            //var shardingDbContextOptions = sp.GetService<Sharding.ShardingDbContextOptions<TestDbContext>>();
            //shardingDbContextOptions.ShardingValue = "2017";

            var shardingFactory = sp.GetService<ShardingDbFactory>();

            //只分库
            var shardingRepo = shardingFactory.GetShardingRepository<LogDbContext, project.Entity.Log>(new ShardingOptions()
            {
                ShardingDbValue = "2017",
                ShardingTableValue = "03"
            });
            var xx = shardingRepo.Query().FirstOrDefault();
            Assert.AreEqual(xx.msg, "分库主表");


            //分库又分表
            shardingRepo = shardingFactory.GetShardingRepository<LogDbContext<project.Entity.Log>, project.Entity.Log>(new ShardingOptions()
            {
                ShardingDbValue = "2017",
                ShardingTableValue = "03"
            });
            xx = shardingRepo.Query().FirstOrDefault();
            Assert.AreEqual(xx.msg, "分库分表");


            //只分表
            shardingRepo = shardingFactory.GetShardingRepository<LogDbContext<project.Entity.Log>, project.Entity.Log>(new ShardingOptions()
            {
                ShardingTableValue = "03"
            });
            xx = shardingRepo.Query().FirstOrDefault();
            Assert.AreEqual(xx.msg, "主库分表");

            //只分表
            shardingRepo = shardingFactory.GetShardingRepository<LogDbContext<project.Entity.Log>, project.Entity.Log>(new ShardingOptions()
            {
                ShardingTableValue = "04"
            });
            xx = shardingRepo.Query().FirstOrDefault();
            Assert.AreEqual(xx.msg, "主库分表");


            //不分库分表
            shardingRepo = shardingFactory.GetShardingRepository<LogDbContext, project.Entity.Log>(null);
            xx = shardingRepo.Query().FirstOrDefault();
            Assert.AreEqual(xx.msg, "主库主表");
        }

        [TestMethod]
        public void TaskLast()
        {

            var r = new String("abc".TakeLast(2).ToArray());

            Assert.AreEqual(r, "bc");
        }


        [TestMethod]
        public void conv()
        {
        }

        [TestMethod]
        public void mysql乐观锁测试()
        {
            var sp = new Startup_RV().ConfigureServices(new ServiceCollection());
            var db = sp.GetService<RVDbContext>();
            //var m = new ModelA() { StringSetLength = "BBBB" };
            //db.ModelASet.Add(m);
            //m.Before();
            //db.SaveChanges();


            //m.StringMax = "stringmax";
            //m.Before();
            //db.SaveChanges();

            //var m = db.ModelASet.Find("201709191740078993");
            //m.StringDefault = "StringDefault";
            //m.Before();
            //db.SaveChanges();

            //var st = new ShardTable() {  DefaultName="DN"};
            //db.Set<ShardTable>().Add(st);
            //st.Before();
            //db.SaveChanges();

            var m = db.Set<ShardTable>().Find("201709191809163785");
            m.DecimalSacle += 99;

            ((ISaveEntityEvent)m).Before();
            db.SaveChanges();

        }


    }
}
