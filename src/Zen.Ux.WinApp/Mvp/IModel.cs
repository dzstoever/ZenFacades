using System;
using System.Configuration;
using Zen.Svcs;

namespace Zen.Ux.WinApp.Mvp
{
    public interface IModel
    {
        void Login(string userName, string password);
        void Logout();
    }

    /// <summary>
    /// The Model in MVP design pattern. 
    /// Implements IModel and communicates with WCF Service.
    /// </summary>
    public class Model : IModel
    {
        #region statics

        //private static ActionServiceClient Client { get; set; }
        private static string AccessToken { get; set; }
        private static string ClientTag { get; set; }

        /// <summary>
        /// Static constructor
        /// </summary>
        static Model()
        {
            // Gets client tag from app.config configuration file
            ClientTag = ConfigurationManager.AppSettings.Get("ClientTag");
            /*
            // Create proxy object.
            Client = new ActionServiceClient();
            
            // Retrieve AccessToken as first step
            var request = PrepareRequest(new TokenRequest());

            var response = Client.GetToken(request);

            // Store access token for all subsequent service calls.
            AccessToken = response.AccessToken;*/
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

            //var response = Client.Login(request);

            //if (response.CorrelationId != request.RequestId)
            //    throw new ApplicationException("Login: RequestId and CorrelationId do not match.");

            //if (response.Acknowledge != Acknowlege.Success)
            //    throw new ApplicationException(response.Message);
        }

        /// <summary>
        /// Logs out of the service.
        /// </summary>
        public void Logout()
        {
            var request = PrepareRequest(new LogoutRequest());

            //var response = Client.Logout(request);

            //if (response.CorrelationId != request.RequestId)
            //    throw new ApplicationException("Logout: RequestId and CorrelationId do not match.");

            //if (response.Acknowledge != Acknowlege.Success)
            //    throw new ApplicationException(response.Message);
        }

        #endregion 


    }
}