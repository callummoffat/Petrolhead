using Microsoft.Azure.Mobile.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Petrolhead.Azure.DataObjects
{
    public class Refuel
        : ModelBase
    {
        public DateTimeOffset? DateRefueled { get; set; }
        public string Location { get; set; }
        public decimal AmountRefueledBy { get; set; }
        public decimal Cost { get; set; }

        public string VehicleId { get; set; }
        [ForeignKey(nameof(VehicleId))]
        public virtual Vehicle Vehicle { get; set; }
    }
}