using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Petrolhead.Azure.DataObjects
{
    public class PurchaseDTO
        : ModelBase
    {
        /// <summary>
        /// Date purchase was made
        /// </summary>
        public DateTimeOffset? DatePurchased { get; set; }
        /// <summary>
        /// Location at which purchase was made
        /// </summary>
        public string PurchasedAtLocation { get; set; }
        /// <summary>
        /// Cost of purchase
        /// </summary>
        public decimal Cost { get; set; }
    }
}