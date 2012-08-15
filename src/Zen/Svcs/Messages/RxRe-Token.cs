using System.Runtime.Serialization;

namespace Zen.Svcs
{

    [DataContract]
    public class TokenRequest : BaseRequest
    {
        // Base class has the required parameters.
    }


    [DataContract]
    public class TokenResponse : BaseResponse
    {
        public TokenResponse() { }

        public TokenResponse(string correlationId) : base(correlationId) { }

        /// <summary>Security token returned to the consumer
        /// </summary>
        [DataMember]
        public string AccessToken;
    }

}

