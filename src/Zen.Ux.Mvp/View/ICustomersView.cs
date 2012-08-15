using System.Collections.Generic;
using Zen.Ux.Mvp.Model;

namespace Zen.Ux.Mvp.View
{
    /// <summary>
    /// Respresents view of a list of customers
    /// </summary>
    public interface ICustomersView : IView
    {
        IList<CustomerModel> Customers { set; }
    }
}
