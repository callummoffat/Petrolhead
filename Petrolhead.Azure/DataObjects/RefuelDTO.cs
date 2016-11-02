using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Petrolhead.Azure.DataObjects
{
    public class RefuelDTO
        : ModelBase
    {
        public DateTimeOffset? DateRefueled { get; set; }
        public string Location { get; set; }
        public decimal AmountRefueledBy { get; set; }
        public decimal Cost { get; set; }
    }
}