using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Petrolhead.Azure.DataObjects
{
    public class ComponentDTO
        : ModelBase
    {
        public DateTimeOffset? DateFailed { get; set; }
        public DateTimeOffset? DateRepaired { get; set; }
        public string ComponentType { get; set; }
        public string ComponentModel { get; set; }
        public decimal Cost { get; set; }
    }
}