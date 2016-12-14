using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Aquirrel.EntityFramework;
using Aquirrel.EntityFramework.Mapping;
using Aquirrel.EntityFramework.Internal;
namespace Microsoft.EntityFrameworkCore
{
    public static class AquirrelDbContextOptionsBuilderExtensions
    {
        /// <summary>
        /// 装载<see cref="Assembly"/>里面所有实现<see cref="IEntityBase{TKey}"/>接口的实体类。
        /// <para>如果实体类已经存在显示的<see cref="EntityMapping{TEntity, TKey}"/>配置，则跳过。</para>
        /// </summary>
        /// <param name="assemblys">需要查找的assembly集合</param>
        /// <returns></returns>
        public static DbContextOptionsBuilder ConfigureAutoEntityAssemblys(this DbContextOptionsBuilder builder, Assembly[] assemblys)
        {
            return SetOption(builder, ops => ops.EntityAssebmlys = assemblys);
        }

        /// <summary>
        /// 装载<see cref="EntityMapping{TEntity, TKey}"/>所关联的实体
        /// </summary>
        /// <param name="assemblys">需要查找的assembly集合</param>
        /// <returns></returns>
        public static DbContextOptionsBuilder ConfigureEntityMappings(this DbContextOptionsBuilder builder, Assembly[] assemblys)
        {

            return SetOption(builder, ops =>
            {
                ops.EntityMappingsAssebmlys = assemblys;
            });
        }
        private static DbContextOptionsBuilder SetOption(DbContextOptionsBuilder builder, Action<CoreOptionAquirrelExtension> optionsetup)
        {
            var ext = builder.Options.FindExtension<CoreOptionAquirrelExtension>();
            ext = ext != null ? ext : new CoreOptionAquirrelExtension();
            optionsetup(ext);
            ((IDbContextOptionsBuilderInfrastructure)builder).AddOrUpdateExtension(ext);
            return builder;
        }
    }
}
