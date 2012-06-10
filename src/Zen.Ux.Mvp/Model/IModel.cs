using System.Collections.Generic;

namespace Zen.Ux.Mvp.Model
{
    /// <summary>
    /// IModel interface, used in MVP design pattern. 
    /// See implementation methods for further details.
    /// </summary>
    public interface IModel
    {
        void Login(string userName, string password);
        void Logout();

        /*
        IList<CustomerModel> GetCustomers(string sortExpression);
        CustomerModel GetCustomer(int customerId);

        int AddCustomer(CustomerModel customer);
        int UpdateCustomer(CustomerModel customer);
        int DeleteCustomer(int customerId);

        IList<OrderModel> GetOrders(int customerId);
        */
    }
}
