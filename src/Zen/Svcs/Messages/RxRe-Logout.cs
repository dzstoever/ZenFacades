using System.Runtime.Serialization;

namespace Zen.Svcs
{

    [DataContract]
    public class LogoutRequest : BaseRequest
    {
        // Base class has the required parameters.
    }


    [DataContract]
    public class LogoutResponse : BaseResponse
    {
        public LogoutResponse() { }

        public LogoutResponse(string correlationId) : base(correlationId) { }

        // Base class has the required parameters.
    }

}
