using AutoMapper;
using Bootstrap.AutoMapper;

//using Zen.Quartz.Entities;
//using Zen.QuartzFacade.DataModel;

namespace Zen.Ux.Bootstrap.MapCreators
{
    /// <summary>
    /// Converts Domain Entities into DTOs (Data Transer Objects) and vice-versa. 
    /// Source and destination classes contain properties of the same name and type.
    /// * Most DTO's will become a [DataMember] of one or more [DataContract]s
    /// </summary>
    /// <remarks>
    /// Initialize is only done once per AppDomain so we are using the Bootstrapper.
    /// </remarks>
    public class DtoMapCreator : IMapCreator
    {
        public void CreateMap(IProfileExpression mapper)
        {
            //Mapper.CreateMap<Scheduler, SchedulerDto>();
            
            // design-time helper   
            // throws a compiler exception if any fields in the destination aren't mapped.   
            Mapper.AssertConfigurationIsValid();           
        }
    }

    /*or we could use this in the consumer and make sure the Bootstrapper looks for it...
    public class TestAutoMapperProfile : Profile
    {
        protected override void Configure()
        {
            Mapper.CreateMap<ITestInterface, TestImplementation>();
        }
    }*/
}