using Microsoft.Azure.Mobile.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Petrolhead.Azure.DataObjects
{
    public abstract class ModelBase
        : EntityData
    {
        /// <summary>
        /// Model name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Model description
        /// </summary>
        public string Description { get; set; }
    }
}