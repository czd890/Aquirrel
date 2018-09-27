using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Aquirrel.EntityFramework.Mapping;
using System.Reflection;
using Aquirrel;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Aquirrel.EntityFramework.Internal
{

    /// <summary>
    /// ef初始化配置服务
    /// </summary>
    public class ConfigureDbContextEntityService
    {
        /// <summary>
        /// 当前ef配置是否包含<see cref="CoreOptionAquirrelExtension"/>对象
        /// </summary>
        /// <param name="dbContextOptions"></param>
        /// <returns></returns>
        public static bool CanConfigure(DbContextOptions dbContextOptions)
        {
            return dbContextOptions.FindExtension<CoreOptionAquirrelExtension>() != null;
        }
        /// <summary>
        /// 完成ef的扩展配置服务
        /// </summary>
        /// <param name="modelBuilder"></param>
        /// <param name="dbContextOptions"></param>
        /// <param name="dbContext"></param>
        public static void Configure(ModelBuilder modelBuilder, DbContextOptions dbContextOptions, DbContext dbContext)
        {
            if (!CanConfigure(dbContextOptions)) return;

            Configure_Mapping(modelBuilder, dbContextOptions, dbContext);

        }
        static void Configure_Mapping(ModelBuilder modelBuilder, DbContextOptions dbContextOptions, DbContext dbContext)
        {
            if (!CanConfigure(dbContextOptions)) return;

            var coreOption = dbContextOptions.FindExtension<CoreOptionAquirrelExtension>();

            var EntityMapping__type = typeof(EntityMapping<>);
            var IEntityMapping_typeInfo = typeof(IEntityMapping).GetTypeInfo();
            var IEntityBase__type = typeof(IEntityBase<>);
            var EntityBase_type = typeof(EntityBase);
            Type[] allMappingEntityTypes = Array.Empty<Type>();
            //装载显示mapping的entites
            if (coreOption.EntityMappingsAssebmlys != null && coreOption.EntityMappingsAssebmlys.Any())
            {
                Console.WriteLine("auto mapping impl IEntityMapping，EntityMapping<,> entity。assebmlys:" + coreOption.EntityMappingsAssebmlys.Select(ass => ass.FullName).ConcatEx("，"));
                //所有ass的types
                var allAssebmlysTypes = coreOption.EntityMappingsAssebmlys.SelectMany(ass => ass.GetTypes()).ToArray();

                //所有显示mapping的mappings
                var allMappings = allAssebmlysTypes
                    .Where(type => type.GetTypeInfo().IsClass)//当前类型是class
                    .Where(type => !type.IsGenericType)
                    .Where(type => IEntityMapping_typeInfo.IsAssignableFrom(type))//当前类型是IEntityMapping接口的实现
                    .ToArray();

                //所有显示mapping的实体
                allMappingEntityTypes = allMappings.Select(type => type.GetTypeInfo().BaseType.GetTypeInfo().GetGenericArguments()[0]).ToArray();

                //装载显示实现了mapping的实体
                allMappings.Each(type => (Activator.CreateInstance(type) as IEntityMapping).Mapping(modelBuilder));
            }

            //自动装载实现了IEntityBase的entites
            if (coreOption.EntityAssebmlys != null && coreOption.EntityAssebmlys.Any())
            {
                Console.WriteLine("auto add entity type with impl IEntityBase。assebmlys:" + coreOption.EntityAssebmlys.Select(ass => ass.FullName).ConcatEx("，"));
                var allEntityTypes = coreOption.EntityAssebmlys
                    .SelectMany(ass => ass.GetTypes())//所有要自动发现entity的assebmly
                    .Select(type => new { type = type, typeInfo = type.GetTypeInfo() })//投影
                    .Where(type => type.typeInfo.IsClass)//当前类型是class
                    .Where(type => !type.typeInfo.IsAbstract)//当前类型不是抽象类
                    .Where(type => type.type != EntityBase_type)//不是entity基类本身
                                                                //当前类型所实现的接口中存在IEntityBase<>
                    .Where(type => type.typeInfo.GetInterfaces().Any(i => i.GetTypeInfo().IsGenericType && i.GetGenericTypeDefinition() == IEntityBase__type))
                    .Select(type => type.type)
                    .ToArray();

                allEntityTypes
                    .Except(allMappingEntityTypes)
                    .Each(type =>
                    {
                        //var entityKeyType = type.GetTypeInfo().GetInterfaces().Single(extendInterface => extendInterface.GetTypeInfo().IsGenericType && extendInterface.GetGenericTypeDefinition() == IEntityBase__type).GetTypeInfo().GenericTypeArguments[0];
                        (Activator.CreateInstance(typeof(EntityMapping<>).MakeGenericType(type)) as IEntityMapping).Mapping(modelBuilder);
                    });
            }
        }


    }
}
