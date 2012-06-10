using System;
using System.Collections.Generic;

namespace Zen.Ux.Mvp.Model
{
    /// <summary>
    /// Order business object as seen by the Service client.
    /// </summary>
    public class OrderModel
    {
        /// <summary>
        /// Gets or sets order identifier. 
        /// </summary>
        public int OrderId{ get; set; }

        /// <summary>
        /// Gets or sets order date.
        /// </summary>
        public DateTime OrderDate{ get; set; }

        /// <summary>
        /// Gets or set required order delivery date.
        /// </summary>
        public DateTime RequiredDate{ get; set; }

        /// <summary>
        /// Gets or sets freight (shipping) costs.
        /// </summary>
        public float Freight{ get; set; }

        /// <summary>
        /// Gets or sets list of order details (line items) for order.
        /// </summary>
        public IList<OrderDetailModel> OrderDetails { get; set; }

        /// <summary>
        /// Gets or sets customer for which order is placed.
        /// </summary>
        public CustomerModel Customer{ get; set; }

        /// <summary>
        /// Version number for optimistic concurrency. Not used at the client.
        /// </summary>
        public string Version { get; set; }
    }
}
