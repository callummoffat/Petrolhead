using Microsoft.Azure.Mobile.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Petrolhead.Azure.DataObjects
{
    public class Expense
        : ModelBase
    {
        public Expense()
        {
            Purchases = new List<Purchase>();
        }

        public DateTimeOffset? DateOfTransaction { get; set; }
        public decimal? Cost { get; set; }        
        public virtual ICollection<Purchase> Purchases { get; set; }


        public string VehicleId { get; set; }
        [ForeignKey(nameof(VehicleId))]
        public virtual Vehicle Vehicle { get; set; }
    }
}