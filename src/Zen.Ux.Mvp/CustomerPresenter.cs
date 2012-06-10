using Zen.Ux.Mvp.Model;
using Zen.Ux.Mvp.View;

namespace Zen.Ux.Mvp
{
    /// <summary>
    /// Customer Presenter class.
    /// </summary>
    /// <remarks>
    /// MV Patterns: MVP design pattern.
    /// </remarks>
    public class CustomerPresenter : Presenter<ICustomerView>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="view">The view</param>
        public CustomerPresenter(ICustomerView view)
            : base(view)
        {
        }

        /// <summary>
        /// Gets customer from model and sets values in the view.
        /// </summary>
        /// <param name="customerId">Customer to display</param>
        public void Display(int customerId)
        {
            if (customerId <= 0) return;

            //var customer = Model.GetCustomer(customerId);

            //View.CustomerId = customer.CustomerId;
            //View.Company = customer.Company;
            //View.City = customer.City;
            //View.Country = customer.Country;
            //View.Version = customer.Version;
        }

        /// <summary>
        /// Saves a customer by getting view data and sending it to model.
        /// </summary>
        /// <returns>Number of records affected.</returns>
        public int Save()
        {
            var customer = new CustomerModel
            {
                CustomerId = View.CustomerId,
                Company = View.Company,
                City = View.City,
                Country = View.Country,
                Version = View.Version
            };

            //if (customer.CustomerId == 0)
            //    return Model.AddCustomer(customer);
            //else
            //    return Model.UpdateCustomer(customer);
            return 0;
        }

        /// <summary>
        /// Deletes a customer by calling into model.
        /// </summary>
        /// <param name="customerId">The customer to delete</param>
        /// <returns>Number of records affected.</returns>
        public int Delete(int customerId)
        {
            return 0;// Model.DeleteCustomer(customerId);
        }
    }
}
