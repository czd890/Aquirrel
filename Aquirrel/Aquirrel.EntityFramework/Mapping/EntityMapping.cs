using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Aquirrel.EntityFramework.Mapping
{
    interface IEntityMapping
    {
        void Mapping(ModelBuilder modelBuilder);
    }
    /// <summary>
    /// mapping实体配置。加载dbconext自动调用。
    /// <para>如果TKey是guid。建立唯一非聚集索引。否则建立唯一聚集索引 可重写<see cref="Mapping(ModelBuilder)"/>方法覆盖</para>
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public class EntityMapping<TEntity, TKey> : IEntityMapping where TEntity : class/*, IEntityBase<TKey>*/
    {
        public virtual void Mapping(ModelBuilder modelBuilder)
        {
            var entityType = typeof(TEntity);
            var entityTypeInfo = entityType.GetTypeInfo();
            var baseType = typeof(IEntityBase<>);

            if (null == modelBuilder.Model.FindEntityType(entityType))
                modelBuilder.Model.AddEntityType(entityType);

            
            if (entityTypeInfo.GetInterfaces().Any(face => face.GetTypeInfo().IsGenericType && face.GetGenericTypeDefinition() == baseType))
            {
                if (typeof(TKey) == typeof(Guid))
                {
                    modelBuilder.Entity(entityType).HasKey("Id").ForSqlServerIsClustered(false);
                    modelBuilder.Entity(entityType).HasIndex("CreatedDate").IsUnique(false).ForSqlServerIsClustered(true);
                }
                else
                {
                    modelBuilder.Entity(entityType).HasKey("Id");
                }
            }
        }
    }
}
