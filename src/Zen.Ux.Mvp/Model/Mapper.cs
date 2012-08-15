using System.Collections.Generic;
using System.Linq;

namespace Zen.Ux.Mvp.Model
{
    /// <summary>
    /// Static class that maps data transfer objects to model objects and vice versa.
    /// </summary>
    internal static class Mapper
    {
        /// <summary>
        /// Maps array of customer data transfer objects to customer model objects.
        /// </summary>
        /// <param name="customers">Array of customer data transfer objects.</param>
        /// <returns>List of customer models.</returns>
        internal static IList<CustomerModel> FromDataTransferObjects(Customer[] customers)
        {
            if (customers == null)
                return null;

            return customers.Select(FromDataTransferObject).ToList();
        }

        /// <summary>
        /// Maps single customer data transfer object to customer model.
        /// </summary>
        /// <param name="customer">Customer data transfer object.</param>
        /// <returns>Customer model object.</returns>
        internal static CustomerModel FromDataTransferObject(Customer customer)
        {
            return new CustomerModel();          
        }

        /// <summary>
        /// Maps array of customer data transfer objects to customer model objects.
        /// </summary>
        /// <param name="orders">Array of order data transfer objects.</param>
        /// <returns>List of order model objects.</returns>
        internal static IList<OrderModel> FromDataTransferObjects(Order[] orders)
        {
            if (orders == null)
                return null;

            return orders.Select(FromDataTransferObject).ToList();
        }

        /// <summary>
        /// Maps single order data transfer object to order model.
        /// </summary>
        /// <param name="order">Order data transfer object.</param>
        /// <returns>Order model object.</returns>
        internal static OrderModel FromDataTransferObject(Order order)
        {
            return new OrderModel();
        }

        /// <summary>
        /// Maps arrary of order detail data transfer objects to list of order details models.
        /// </summary>
        /// <param name="orderDetails">Array of order detail data transfer objects.</param>
        /// <returns>List of order detail models.</returns>
        internal static IList<OrderDetailModel> FromDataTransferObjects(OrderDetail[] orderDetails)
        {
            if (orderDetails == null)
                return null;

            return orderDetails.Select(FromDataTransferObject).ToList();
        }

        /// <summary>
        /// Maps order detail data transfer object to order model object.
        /// </summary>
        /// <param name="orderDetail">Order detail data transfer object.</param>
        /// <returns>Orderdetail model object.</returns>
        internal static OrderDetailModel FromDataTransferObject(OrderDetail orderDetail)
        {
            return new OrderDetailModel();

        }

        /// <summary>
        /// Maps customer model object to customer data transfer object.
        /// </summary>
        /// <param name="customer">Customer model object.</param>
        /// <returns>Customer data transfer object.</returns>
        internal static Customer ToDataTransferObject(CustomerModel customer)
        {
            return new Customer();                    
        }
    }

    internal class OrderDetail
    {
    }

    internal class Order
    {
    }

    internal class Customer
    {
    }
}
