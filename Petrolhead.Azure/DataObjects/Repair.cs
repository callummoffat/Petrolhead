using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Petrolhead.Azure.DataObjects
{
    public class Repair
        : ModelBase
    {

        public Repair()
        {
            Components = new List<Component>();
        }

        public DateTimeOffset? DateOfTransaction { get; set; }
        public decimal Cost { get; set; }
        public virtual ICollection<Component> Components { get; set; }
        
        public string VehicleId { get; set; }
        [ForeignKey(nameof(VehicleId))]
        public virtual Vehicle Vehicle { get; set; }
    }
}