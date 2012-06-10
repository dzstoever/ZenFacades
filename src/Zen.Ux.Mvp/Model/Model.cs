using System;
using System.Collections.Generic;
using System.Configuration;
using Zen.Svcs;

namespace Zen.Ux.Mvp.Model
{
    /// <summary>
    /// The Model in MVP design pattern. 
    /// Implements IModel and communicates with WCF Service.
    /// </summary>
    public class Model : IModel
    {
        #region statics

        private static IRemoteFacade Client { get; set; }
        private static string AccessToken { get; set; }
        private static string ClientTag { get; set; }

        /// <summary>
        /// Static constructor
        /// </summary>
        static Model()
        {
            // Create proxy object.
            //Client = new RemoteFacadeSvc();

            // Gets client tag from app.config configuration file
            ClientTag = ConfigurationManager.AppSettings.Get("ClientTag");

            // Retrieve AccessToken as first step
            var request =  PrepareRequest(new TokenRequest());
            
            var response = Client.GetToken(request);

            // Store access token for all subsequent service calls.
            AccessToken = response.AccessToken;
        }

        /// <summary>
        /// Adds RequestId, ClientTag, and AccessToken to all request types.
        /// </summary>
        /// <typeparam name="T">The request type.</typeparam>
        /// <param name="request">The request</param>
        /// <returns>Fully prepared request, ready to use.</returns>
        private static T PrepareRequest<T>(T request) where T : BaseRequest
        {
            request.RequestId = Guid.NewGuid().ToString();  // Generates unique request id
            request.ClientTag = ClientTag;
            request.AccessToken = AccessToken;

            return request;
        }

        #endregion

        #region Login / Logout

        /// <summary>
        /// Logs in to the service.
        /// </summary>
        /// <param name="userName">User name.</param>
        /// <param name="password">Password.</param>
        public void Login(string userName, string password)
        {
            var request = PrepareRequest(new LoginRequest());

            request.UserName = userName;
            request.Password = password;

            var response = Client.Login(request);

            if (response.CorrelationId != request.RequestId)
                throw new ApplicationException("Login: RequestId and CorrelationId do not match.");

            if (response.Acknowledge != Acknowlege.Success)
                throw new ApplicationException(response.Message);
        }

        /// <summary>
        /// Logs out of the service.
        /// </summary>
        public void Logout()
        {
            var request = PrepareRequest(new LogoutRequest());

            var response = Client.Logout(request);

            if (response.CorrelationId != request.RequestId)
                throw new ApplicationException("Logout: RequestId and CorrelationId do not match.");

            if (response.Acknowledge != Acknowlege.Success)
                throw new ApplicationException(response.Message);
        }

        #endregion

        /*
        #region Customers

        /// <summary>
        /// Gets a list of all customers in the given sort order.
        /// </summary>
        /// <param name="sortExpression">Sort order.</param>
        /// <returns>List of customers.</returns>
        public IList<CustomerModel> GetCustomers(string sortExpression)
        {
            var request = PrepareRequest(new CustomerRequest());

            request.LoadOptions = new string[] { "Customers" };
            request.Criteria = new CustomerCriteria { SortExpression = sortExpression };

            var response = Client.GetCustomers(request);

            if (response.CorrelationId != request.RequestId)
                throw new ApplicationException("GetCustomers: RequestId and CorrelationId do not match.");

            if (response.Acknowledge != AcknowledgeType.Success)
                throw new ApplicationException(response.Message);

            return Mapper.FromDataTransferObjects(response.Customers);
        }

        /// <summary>
        /// Gets a specific customer.
        /// </summary>
        /// <param name="customerId">Unique customer identifier.</param>
        /// <returns>Customer.</returns>
        public CustomerModel GetCustomer(int customerId)
        {
            var request = PrepareRequest(new CustomerRequest());
            request.LoadOptions = new string[] { "Customer" };
            request.Criteria = new CustomerCriteria { CustomerId = customerId };

            var response = Client.GetCustomers(request);

            if (response.CorrelationId != request.RequestId)
                throw new ApplicationException("GetCustomer: RequestId and CorrelationId do not match.");

            if (response.Acknowledge != AcknowledgeType.Success)
                throw new ApplicationException(response.Message);

            return Mapper.FromDataTransferObject(response.Customer);
        }

        #endregion

        #region Customer persistence

        /// <summary>
        /// Adds a new customer to the database.
        /// </summary>
        /// <param name="customer">Customer.</param>
        /// <returns>Number of records affected. If all worked well, then should be 1.</returns>
        public int AddCustomer(CustomerModel customer)
        {
            var request = PrepareRequest(new CustomerRequest());
            request.Action = "Create";
            request.Customer = Mapper.ToDataTransferObject(customer);

            var response = Client.SetCustomers(request);

            if (response.CorrelationId != request.RequestId)
                throw new ApplicationException("AddCustomer: RequestId and CorrelationId do not match.");

            if (response.Acknowledge != AcknowledgeType.Success)
                throw new ApplicationException(response.Message);

            return response.RowsAffected;

        }

        /// <summary>
        /// Updates an existing customer in the database.
        /// </summary>
        /// <param name="customer">The updated customer.</param>
        /// <returns>Number or records affected. Should be 1.</returns>
        public int UpdateCustomer(CustomerModel customer)
        {
            var request = PrepareRequest(new CustomerRequest());
            request.Action = "Update";
            request.Customer = Mapper.ToDataTransferObject(customer);

            var response = Client.SetCustomers(request);

            if (response.CorrelationId != request.RequestId)
                throw new ApplicationException("UpdateCustomer: RequestId and CorrelationId do not match.");

            if (response.Acknowledge != AcknowledgeType.Success)
                throw new ApplicationException(response.Message);

            return response.RowsAffected;
        }

        /// <summary>
        /// Deletes a customer record.
        /// </summary>
        /// <param name="customerId">Unique customer identifier.</param>
        /// <returns>Number of records affected. Should be 1.</returns>
        public int DeleteCustomer(int customerId)
        {
            var request = PrepareRequest(new CustomerRequest());
            request.Action = "Delete";
            request.Criteria = new CustomerCriteria { CustomerId = customerId };

            var response = Client.SetCustomers(request);

            if (response.CorrelationId != request.RequestId)
                throw new ApplicationException("DeleteCustomer: RequestId and CorrelationId do not match.");

            if (response.Acknowledge != AcknowledgeType.Success)
                throw new ApplicationException(response.Message);

            return response.RowsAffected;
        }

        #endregion

        #region Orders

        /// <summary>
        /// Gets a list of orders for a given customer.
        /// </summary>
        /// <param name="customerId">Unique customer identifier.</param>
        /// <returns>List of orders.</returns>
        public IList<OrderModel> GetOrders(int customerId)
        {
            var request = PrepareRequest(new OrderRequest());

            request.LoadOptions = new string[] { "Orders", "OrderDetails", "Product" };
            request.Criteria = new OrderCriteria { CustomerId = customerId, SortExpression = "OrderId ASC" };

            var response = Client.GetOrders(request);

            if (response.CorrelationId != request.RequestId)
                throw new ApplicationException("GetOrders: RequestId and CorrelationId do not match.");

            if (response.Acknowledge != AcknowledgeType.Success)
                throw new ApplicationException(response.Message);

            return Mapper.FromDataTransferObjects(response.Orders);
        }

        #endregion
        */
    }
}
