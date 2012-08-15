using System.Collections.Generic;

namespace Zen.Ux.Mvp.Model
{
    /// <summary>
    /// Customer business object as seen by the Service client.
    /// </summary>
    public class CustomerModel
    {
        /// <summary>
        /// Gets or sets customerId. 
        /// </summary>
        public int CustomerId{ get; set; }

        /// <summary>
        /// Gets or sets customer name.
        /// </summary>
        public string Company{ get; set; }

        /// <summary>
        /// Gets or sets customer city.
        /// </summary>
        public string City{ get; set; }

        /// <summary>
        /// Gets or set customer country.
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// Gets or sets list of orders associated with customer.
        /// </summary>
        public IList<OrderModel> Orders { get; set; }

        /// <summary>
        /// Version number for optimistic concurrency. Not used at the client.
        /// </summary>
        public string Version { get; set; }
    }
}
