using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Aquirrel.EntityFramework.Mapping;
using System.Reflection;
using Aquirrel;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aquirrel.EntityFramework.Internal
{
    public class ConfigureDbContextEntityService
    {


        public static void ConfigureMapping(ModelBuilder modelBuilder, DbContextOptionsBuilder optionsBuilder, AquirrelDbContext aquirrelDbContext)
        {
            var coreOption = optionsBuilder.Options.FindExtension<CoreOptionAquirrelExtension>();
            if (coreOption == null)
                return;
            var EntityMapping__type = typeof(EntityMapping<,>);
            var IEntityMapping_typeInfo = typeof(IEntityMapping).GetTypeInfo();
            var IEntityBase__type = typeof(IEntityBase<>);
            var EntityBase_type = typeof(EntityBase);
            Type[] allMappingEntityTypes = Array.Empty<Type>();
            //装载显示mapping的entites
            if (coreOption.EntityMappingsAssebmlys != null && coreOption.EntityMappingsAssebmlys.Any())
            {
                //所有mapping所在assebmly的type
                var allMappingTypes = coreOption.EntityMappingsAssebmlys.SelectMany(ass => ass.GetTypes()).ToArray();
                //所有显示mapping的mappings
                var allMappings = allMappingTypes.Where(type => type.GetTypeInfo().IsClass && type != EntityMapping__type && IEntityMapping_typeInfo.IsAssignableFrom(type)).ToArray();
                //所有显示mapping的实体
                allMappingEntityTypes = allMappings.Select(type => type.GetTypeInfo().BaseType.GetTypeInfo().GetGenericArguments()[0]).ToArray();
                //装载显示实现了mapping的实体
                allMappings.Each(type => (Activator.CreateInstance(type) as IEntityMapping).Mapping(modelBuilder));
            }

            //自动装载实现了IEntityBase的entites
            if (coreOption.EntityAssebmlys != null && coreOption.EntityAssebmlys.Any())
            {
                var allEntityTypes = coreOption.EntityAssebmlys.SelectMany(ass => ass.GetTypes()).Select(type => new { type = type, typeInfo = type.GetTypeInfo() })
                    .Where(type => type.typeInfo.IsClass && !type.typeInfo.IsAbstract && type.typeInfo.GetInterfaces().Any(interFace => interFace.GetTypeInfo().IsGenericType && interFace.GetGenericTypeDefinition() == IEntityBase__type) && type.type != EntityBase_type)
                    .Select(type => type.type).ToArray();

                allEntityTypes.Except(allMappingEntityTypes).Each(type =>
                {
                    var entityKeyType = type.GetTypeInfo().GetInterfaces().Single(extendInterface => extendInterface.GetTypeInfo().IsGenericType && extendInterface.GetGenericTypeDefinition() == IEntityBase__type).GetTypeInfo().GenericTypeArguments[0];
                    (Activator.CreateInstance(typeof(EntityMapping<,>).MakeGenericType(type, entityKeyType)) as IEntityMapping).Mapping(modelBuilder);
                });
            }
        }

        public static void ConfigureDefaultAnnotation(ModelBuilder modelBuilder, DbContextOptionsBuilder optionsBuilder, AquirrelDbContext aquirrelDbContext)
        {
            var coreOption = optionsBuilder.Options.FindExtension<CoreOptionAquirrelExtension>();
            if (coreOption == null)
                return;

            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entity.GetProperties())
                {
                    var isMaxLengthAnnotation = property.FindAnnotation(PropertyBuilderOfTExtensions.IsMaxLengthAnnotationName);
                    if ((isMaxLengthAnnotation==null||!((bool)isMaxLengthAnnotation.Value))
                        &&  true                      )
                    {

                    }
                }
            }
        }
    }
}
