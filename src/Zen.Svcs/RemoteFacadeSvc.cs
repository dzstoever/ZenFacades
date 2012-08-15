using System;
using System.Collections.Generic;
using System.ServiceModel;
using Zen.Data;
using Zen.Log;
using Zen.Svcs.Bootstrap;
using Zen.Svcs.DataModel;
using Zen.Svcs.ServiceModel;

namespace Zen.Svcs
{
    
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    //[IocServiceBehavior(StartupShellType = typeof(HostStartupShell))]
    public class RemoteFacadeSvc : SecureVaultSvc, IRemoteFacade
    {
        public RemoteFacadeSvc(IGenericDao dao, ILogger log)
        {
            if (dao == null) throw new ConfigException("Data Acess Object(Dao) is null. Additional configuration may be needed before initializing the " + GetType());
            _dao = dao;
            if (log == null) throw new ConfigException("Logger is null. Additional configuration may be needed before initializing the " + GetType());
            _log = log;// ?? new NoLogger();            
        }

       private readonly IGenericDao _dao;
       private readonly ILogger _log;

        public FacadeResponse GetFacades(FacadeRequest request)
        {
            var response = new FacadeResponse { CorrelationId = request.RequestId };

            if (!ValidRequest(request, response, Validate.ClientTag | Validate.AccessToken))
                return response;

            //Todo: get the right pods for the user
            var facadesForUser = new List<FacadeDto>
                                     {
                                         new FacadeDto
                                             {
                                                 Id = Guid.NewGuid(),
                                                 Title = "I am Facade 1",
                                                 MenuName = "Facade1",
                                                 ImageIndex = 1
                                             },
                                         new FacadeDto
                                             {
                                                 Id = Guid.NewGuid(),
                                                 Title = "I am Facade 2",
                                                 MenuName = "Facade2",
                                                 ImageIndex = 2
                                             },
                                         new FacadeDto
                                             {
                                                 Id = Guid.NewGuid(),
                                                 Title = "I am Facade 3",
                                                 MenuName = "Facade3",
                                                 ImageIndex = 3
                                             }
                                     };

            response.Facades = facadesForUser; 
            //Mapper.Map<FacadeDto, FacadeDto>(facadesForUser);<- not needed
            return response;
        }

    }

}