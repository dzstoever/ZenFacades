using System.Collections.Generic;
using System.Runtime.Serialization;
using Zen.Svcs.DataModel;

namespace Zen.Svcs
{

    [DataContract]
    public class FacadeRequest : BaseRequest
    {
        //nothing more needed here         
    }

    [DataContract]
    public class FacadeResponse : BaseResponse
    {
        public FacadeResponse() { }

        public FacadeResponse(string correlationId) : base(correlationId) { }

        [DataMember] 
        public IList<FacadeDto> Facades;

    }
}