using Zen.Ux.Mvp.View;

namespace Zen.Ux.Mvp
{
    /// <summary>
    /// Orders Presenter class.
    /// </summary>
    /// <remarks>
    /// MV Patterns: MVP design pattern.
    /// </remarks>
    public class OrdersPresenter : Presenter<IOrdersView>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="view">The view</param>
        public OrdersPresenter(IOrdersView view)
            : base(view)
        {
        }

        /// <summary>
        /// Displays list of orders.
        /// </summary>
        /// <param name="customerId">Customer id to display.</param>
        public void Display(int customerId)
        {
            //View.Orders = Model.GetOrders(customerId); 
        }
    }
}
