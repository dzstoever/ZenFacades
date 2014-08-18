using System.Collections.Generic;
using System.Runtime.Serialization;
using Zen.Data.QueryModel;
using Zen.Quartz.Facade.DataModel;

namespace Zen.Quartz.Facade
{
    [DataContract]
    public class SchedulerRequest //: BaseRequest
    {
        [DataMember]
        public Query Query { get; set; }
        
        [DataMember]
        public SchedulerDto Scheduler;

        
        
        //move me   
        [DataMember]
        public string InstanceId { get; set; }
    }

    [DataContract]
    public class SchedulerResponse //: BaseResponse
    {
        public SchedulerResponse() { }
        //public SchedulerResponse(string correlationId) : base(correlationId) { }

        [DataMember]
        public IList<SchedulerDto> Schedulers { get; set; }

        [DataMember]
        public SchedulerDto Scheduler { get; set; }



        //move me   
        [DataMember]
        public string SchedulerDescription { get; set; }
    }
}