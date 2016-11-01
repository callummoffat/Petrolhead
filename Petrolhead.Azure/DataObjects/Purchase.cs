using Microsoft.Azure.Mobile.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Petrolhead.Azure.DataObjects
{
    public class Purchase
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

        /// <summary>
        /// ID of parent Expense
        /// </summary>
        public string ExpenseId { get; set; }

        /// <summary>
        /// Parent Expense
        /// </summary>
        [ForeignKey(nameof(ExpenseId))]
        public virtual Expense Expense { get; set; }
    }
}