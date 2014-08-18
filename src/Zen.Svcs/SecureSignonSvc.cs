using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace Zen.Svcs
{
    // Note:  What if you are keeping some information between calls? 
    // For the service to 'scaleable' we must keep the service 'stateless', 
    // this means it can not hold state between calls - 
    // state  needs to be stored somewhere else, not in the service. 
    // 1 Good option is the database, but if that is too slow, you can use AppFabric caching.
    //[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    //[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class SecureSignonSvc : ISecureSignon
    {
        public SecureSignonSvc()
        { 
            ClientTags = new List<string> {MasterClientTag};
        }
        //public SecureSignonSvc(IEnumerable<string> clientTags) { }...could be set using IoC
        
        public IList<string> ClientTags { private get; set; }
        private const string MasterClientTag = "YesterdayUponTheStairIMetAGirlWhoWasntThere";

        // stores the _accessToken and _userName only for the session duration
        private string _accessToken;// set after a valid TokenRequest, and set back to null after Logout
        private string _userName;   // set after a valid LoginRequest, and set back to null after Logout
       
        /// <summary>
        /// Creates a unique access token that is stored for the duration of the session.
        /// </summary>
        /// <remarks>you must call Logout() to destroy the access token, even if Login() was never called...
        /// Note: Login() is only relevant if you have methods that use the Validate.UserCredentials option.</remarks>
        public TokenResponse GetToken(TokenRequest request)
        {
            var response = new TokenResponse { CorrelationId = request.RequestId };
            
            if (!ValidRequest(request, response, Validate.ClientTag))
                return response;

            // issue a session-based access token
            _accessToken = Guid.NewGuid().ToString();// Console.WriteLine("Access Token granted: " + _accessToken);
            
            response.AccessToken = _accessToken;
            return response;
        }

        public LoginResponse Login(LoginRequest request)
        {
            var response = new LoginResponse { CorrelationId = request.RequestId };

            if (!ValidRequest(request, response, Validate.AccessToken))
                return response;

            if( (request.UserName != "leroy" || request.Password != "secret123") &&
                !System.Web.Security.Membership.ValidateUser(request.UserName, request.Password))
            {
                response.Acknowledge = Acknowledge.Failure;
                response.Message = "Invalid username and/or password.";
                return response;
            }


            _userName = request.UserName;

            return response;
        }

        public LogoutResponse Logout(LogoutRequest request)
        {
            var response = new LogoutResponse { CorrelationId = request.RequestId };

            if (!ValidRequest(request, response, Validate.AccessToken))
                return response;
            
            _accessToken = null;// Console.WriteLine("Access Token destroyed: " + _accessToken);
            _userName = null;   // Console.WriteLine("User is logged out: " + _userName);

            return response;
        }

       
        /// <summary>
        /// Validate 3 possible security levels for a request: ClientTag, AccessToken, UserCredentials, or All
        /// </summary>
        /// <remarks>
        /// validate can also be passed as a bitwise combination:
        /// Ex. Validate.ClientTag | Validate.AccessToken
        /// </remarks>
        protected bool ValidRequest(BaseRequest request, BaseResponse response, Validate validate)
        {
            if (request.ClientTag == MasterClientTag)
                return true;//bypass all security
            
            // Validate Client Tag. 
            if ((Validate.ClientTag & validate) == Validate.ClientTag)
            {   
                if(!ClientTags.Contains(request.ClientTag))
                {
                    response.Acknowledge = Acknowledge.Failure;
                    response.Message = "Unknown Client Tag.";
                    return false;
                }
            }

            // Validate access token
            if ((Validate.AccessToken & validate) == Validate.AccessToken)
            {
                if (_accessToken == null)
                {
                    response.Acknowledge = Acknowledge.Failure;
                    response.Message = "Invalid or expired AccessToken.";
                    return false;
                }
            }

            // Validate user credentials
            if ((Validate.UserCredentials & validate) == Validate.UserCredentials)
            {
                //Todo: tie userName to WindowsPrincipal or .net membership provider?
                //System.Security.Principal.WindowsPrincipal userPrincipal;

                if (_userName == null)//temp - any user name constitutes valid credentials
                {
                    response.Acknowledge = Acknowledge.Failure;
                    response.Message = "Please login and provide user credentials before accessing these methods.";
                    return false;
                }
            }


            return true;
        }

        [Flags]//treated as a bit field, or a set of flags
        protected enum Validate
        {
            ClientTag = 0x0001,
            AccessToken = 0x0002,
            UserCredentials = 0x0004,
            All = ClientTag | AccessToken | UserCredentials
        }
         

    }
}