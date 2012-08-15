using AutoMapper;
using Bootstrap.AutoMapper;
using Zen.Svcs.DataModel;
using Zen.Ux.Mvvm.Model;

//using Zen.QuartzFacade.DataModel;

namespace Zen.Ux.Bootstrap.MapCreators
{
    /// <summary>
    /// Converts Entities-or-DTOs into BMOs (Business Model Objects) and vice-versa.
    /// Source and destination classes contain properties of the same name and type.
    /// </summary>
    /// <remarks>
    /// Initialize is only done once per AppDomain so we are using the Bootstrapper.
    /// </remarks>
    public class BmoMapCreator : IMapCreator
    {
        public void CreateMap(IProfileExpression mapper)
        {
            mapper.CreateMap<FacadeDto, FacadeBmo>(); //<-- Entity to Model 

            //mapper.CreateMap<SchedulerDto, SchedulerBmo>(); //<-- Dto to Model
            
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