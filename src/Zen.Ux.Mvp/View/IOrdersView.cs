using System.Collections.Generic;
using Zen.Ux.Mvp.Model;

namespace Zen.Ux.Mvp.View
{
    /// <summary>
    /// Represents view of orders.
    /// </summary>
    public interface IOrdersView : IView
    {
        IList<OrderModel> Orders { set; }
    }
}
