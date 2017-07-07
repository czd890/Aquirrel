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
        public string Name { get; set; }
    }
    public class ModelB : EntityBase
    {
        [StringLength(22)]
        public string Desc { get; set; }
        public string AId { get; set; }

        [ForeignKey("AId")]
        public List<ModelA> ModelA { get; set; }
    }

    public class ModelC : EntityBase
    {

        public string HH { get; set; }

        public DateTime Date { get; set; }

        [MaxLength]
        public string MaxLen { get; set; }
    }
}
