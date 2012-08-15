using System.Runtime.Serialization;

namespace Zen.Svcs
{

    [DataContract]
    public class LoginRequest : BaseRequest
    {
        [DataMember]
        public string UserName = "";

        [DataMember]
        public string Password = "";
    }


    [DataContract]
    public class LoginResponse : BaseResponse
    {
        public LoginResponse() { }

        public LoginResponse(string correlationId) : base(correlationId) { }

        /// <summary>
        /// Uri to which client should redirect following successful login. 
        /// This would be necessary if authentication is handled centrally 
        /// and other services are distributed accross multiple servers. 
        /// Not used in this sample application. 
        /// SalesForce.com uses this in their API.
        /// </summary>
        [DataMember]
        public string Uri = "";

        /// <summary>
        /// Session identifier. Useful when sessions are maintained using 
        /// SOAP headers (rather than cookies). Not implemented yet.
        /// </summary>
        [DataMember]
        public string SessionId = "";
    }

}
