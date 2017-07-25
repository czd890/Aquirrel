using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
}
