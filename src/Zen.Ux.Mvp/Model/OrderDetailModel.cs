namespace Zen.Ux.Mvp.Model
{
    /// <summary>
    /// Order detail business object as defined on service client side.
    /// </summary>
    public class OrderDetailModel
    {
        /// <summary>
        /// Gets or sets product name.
        /// </summary>
        public string ProductName{ get; set; }

        /// <summary>
        /// Gets or sets quantity of products ordered.
        /// </summary>
        public int Quantity{ get; set; }

        /// <summary>
        /// Gets or set unit price of product.
        /// </summary>
        public float UnitPrice{ get; set; }

        /// <summary>
        /// Gets or sets discount applied to unit price in this order.
        /// </summary>
        public float Discount{ get; set; }

        /// <summary>
        /// Gets or sets order of which this order detail is a part.
        /// </summary>
        public OrderModel Order{ get; set; }

        /// <summary>
        /// Version number for optimistic concurrency. Not used at the client.
        /// </summary>
        public string Version { get; set; }
    }
}
