using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel;

namespace Aquirrel.EntityFramework.Mapping
{
    /// <summary>
    /// 实体配置接口
    /// </summary>
    interface IEntityMapping
    {
        void Mapping(ModelBuilder modelBuilder);
    }
    /// <summary>
    /// mapping实体配置。加载dbconext自动调用。
    /// <para>实体继承自<see cref="IEntityBase"/>的情况下，如果TKey是guid，则id建立唯一非聚集索引，CreatedDate建立不唯一聚集索引。否则建立唯一聚集索引 可重写<see cref="Mapping(ModelBuilder)"/>方法覆盖</para>
    /// <para>非继承自<see cref="IEntityBase"/>的情况下，没有默认配置</para>
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public class EntityMapping<TEntity> : IEntityMapping where TEntity : class
    {
        public Type EntityType { get; set; } = typeof(TEntity);

        /// <summary>
        /// 配置实体
        /// </summary>
        /// <param name="entityTypeBuilder"></param>
        public virtual void Mapping(EntityTypeBuilder<TEntity> entityTypeBuilder)
        {
            var entityTypeInfo = this.EntityType.GetTypeInfo();
            var baseType = typeof(IEntityBase<>);


            if (entityTypeInfo.GetInterfaces().Any(face => face.GetTypeInfo().IsGenericType && face.GetGenericTypeDefinition() == baseType))
            {
                var keyBuilder = entityTypeBuilder.HasKey("Id");
                if (keyBuilder.Metadata.DeclaringEntityType.ClrType == typeof(Guid))
                {
                    keyBuilder.ForSqlServerIsClustered(false);
                    entityTypeBuilder.HasIndex("CreatedDate").IsUnique(false).ForSqlServerIsClustered(true);
                }
                entityTypeBuilder.Property("RowVersion").IsConcurrencyToken();
            }
        }
        /// <summary>
        /// 配置实体
        /// </summary>
        /// <param name="modelBuilder"></param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void Mapping(ModelBuilder modelBuilder)
        {
            var entityTypeBuilder = modelBuilder.Model.FindEntityType(this.EntityType);
            if (entityTypeBuilder == null)
                entityTypeBuilder = modelBuilder.Model.AddEntityType(this.EntityType);

            this.Mapping(modelBuilder.Entity<TEntity>());
        }
    }
}
