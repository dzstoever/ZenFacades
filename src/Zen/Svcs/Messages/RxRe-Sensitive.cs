using System.Runtime.Serialization;

namespace Zen.Svcs
{
    [DataContract]
    public class SensitiveRequest : BaseRequest
    {
        [DataMember]
        public string DataKey = "";
    }

    [DataContract]
    public class SensitiveResponse : BaseResponse
    {
        public SensitiveResponse() { }

        public SensitiveResponse(string correlationId) : base(correlationId) { }

        [DataMember]
        public string EncryptedData;

    }
}

