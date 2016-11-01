using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Petrolhead.Azure.DataObjects
{
    public class Component
        : ModelBase
    {
        public DateTimeOffset? DateFailed { get; set; }
        public DateTimeOffset? DateRepaired { get; set; }
        public string ComponentType { get; set; }
        public string ComponentModel { get; set; }
        public decimal Cost { get; set; }


        public string RepairId { get; set; }
        [ForeignKey(nameof(RepairId))]
        public virtual Repair Repair { get; set; }
    }
}