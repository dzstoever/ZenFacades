using System.Collections.Generic;
using System.ServiceModel;
using AutoMapper;
using Zen.Data;
using Zen.Ioc;
using Zen.Quartz.Entities;
using Zen.Quartz.Facade.DataModel;

namespace Zen.Quartz.Facade
{
    [ServiceContract] 
    public interface IQuartzFacadeSvc //: ISecureSignonSvc
    {

        [OperationContract]
        SchedulerResponse GetSchedulers(SchedulerRequest rx);

    }

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]// Important: Each Session has its own instance of this class 
    public class QuartzFacadeSvc : IQuartzFacadeSvc
    {

        protected IGenericDao Dao { get; set; }

        public QuartzFacadeSvc()
        {
            WindsorDI.ConfigureFromEntryAssembly = false;
            WindsorDI.ConfigureFromInstallers = new object[] { new DaoInstaller() };

            var DI = Aspects.GetIocDI() as WindsorDI;
            if (DI == null) throw new DependencyException("Failed to create the Windsor DI.");
            DI.Initialize();
            Dao = DI.Resolve<IGenericDao>();
            if (Dao == null) throw new DataAccessException("Failed to create the Data Acess Object.");
        }


        public SchedulerResponse GetSchedulers(SchedulerRequest request)
        {
            var response = new SchedulerResponse();

            // Fetch data using Dao
            Dao.StartUnitOfWork();
            var entities = Dao.FetchAll<Scheduler>();
            Dao.CloseUnitOfWork();

            // Move Enties to Dtos
            response.Schedulers = Mapper.Map<
                IList<Scheduler>, IList<SchedulerDto>>(entities);

            return response;
        }


    }
}