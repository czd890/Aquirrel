using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aquirrel.EntityFramework.Test
{
    public class ModelA : EntityBase
    {
        public string StringDefault { get; set; }

        [MaxLength]
        public string StringMax { get; set; }

        [StringLength(640)]
        public string StringSetLength { get; set; }

        public int intDefault { get; set; }

        public decimal decimalDefault { get; set; }

        [DecimalPrecision]
        public decimal decimalSetSacle { get; set; }

    }

    [Table("Table_attr_name")]
    public class AutoEntryTable : EntityBase
    {
        public string name { get; set; }
    }


    public class ShardTable : EntityBase
    {
        public ShardTable()
        {
            this.Version = 99;
        }
        public string MaxName { get; set; }
        public string DefaultName { get; set; }

        public byte[] ts { get; set; }
    }

    class ShardTableMapping : Mapping.EntityMapping<ShardTable>
    {
        public override void Mapping(EntityTypeBuilder<ShardTable> entityTypeBuilder)
        {
            Console.WriteLine("ShardTable mapping.............");

            //base.Mapping(entityTypeBuilder);

            entityTypeBuilder.HasKey(p => p.Id);

            entityTypeBuilder.Property(p => p.MaxName).HasMaxLength(true);

            entityTypeBuilder.Property(p => p.ts).IsRowVersion();
            //entityTypeBuilder.Ignore(p => p.ts);

            //entityTypeBuilder.Property(p => p.Version).ValueGeneratedOnAddOrUpdate();
            entityTypeBuilder.Property(p => p.Version).ValueGeneratedOnAdd();

            //entityTypeBuilder.HasShardTable(new ShardTableShardRule() { });

        }

        class ShardTableShardRule : Shard.ShardRule<ShardTable>
        {
            public string GetTableName(ShardTable entity)
            {
                var number = new String(entity.Id.TakeLast(2).ToArray());
                return "ShardTable_" + number;
            }
        }
    }
}
