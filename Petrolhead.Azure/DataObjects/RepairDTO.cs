using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Petrolhead.Azure.DataObjects
{
    public class RepairDTO
        : ModelBase
    {
        public DateTimeOffset? DateOfTransaction { get; set; }
        public decimal Cost { get; set; }
        public virtual ICollection<ComponentDTO> Components { get; set; }
    }
}