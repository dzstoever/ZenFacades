using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using AutoMapper;
using Zen.Svcs;
using Zen.Svcs.DataModel;
using Zen.Ux.Mvvm.Model;

namespace Zen.Ux.Mvvm
{
    /// <summary>
    /// Provides an interface to the Services(Svcs), by handling user authentication 
    /// and other request-response message processing for the view models.  
    /// </summary>
    /// <remarks>
    /// While we are currently handling everything here in 1 provider it could be broken
    /// out into a set of finer grained providers possibly derived from a generic base class. 
    /// i.e. BaseProvider(T)...where T : IViewModel
    /// </remarks>    
    public interface IProvider
    {
        // secure sign on
        void Login(string userName, string password);
        void Logout();

        // app facade
        ObservableCollection<FacadeBmo> GetFacades(); 

        // job scheduler
        ObservableCollection<SchedulerBmo> GetSchedulers(string sort);
        SchedulerBmo GetScheduler(int id);
        int AddScheduler(SchedulerBmo schedulerBmo);
        int UpdateScheduler(SchedulerBmo schedulerBmo);
        int DeleteScheduler(int id);
        //ObservableCollection<JobModel> GetJobs(int jobId);
    }

    public class Provider : IProvider
    {
        //SecureSignonSvc.GetToken() is called here...Todo? move into explicit GetToken() method
        public Provider(IRemoteFacade appFacadeProxy)
        {            
            _appFacadeProxy = appFacadeProxy;
            
            // Gets client tag from app.config configuration file
            _clientTag = "YesterdayUponTheStairIMetAGirlWhoWasntThere";//ConfigurationManager.AppSettings.Get("ClientTag");

            // Retrieve AccessToken as first step
            var request = PrepareRequest(new TokenRequest());
            var response = _appFacadeProxy.GetToken(request);

            // Store access token for subsequent service calls.
            _accessToken = response.AccessToken;
            
        }

        private readonly IRemoteFacade _appFacadeProxy;
        private readonly string _clientTag;
        private readonly string _accessToken;

        /// <summary>Adds RequestId, ClientTag, and AccessToken to all request types.
        /// </summary>
        /// <typeparam name="T">The request type.</typeparam>
        private T PrepareRequest<T>(T request) where T : BaseRequest
        {
            request.RequestId = Guid.NewGuid().ToString();  // Generates unique request id
            request.ClientTag = _clientTag;
            request.AccessToken = _accessToken;

            return request;
        }
        
        public void Login(string userName, string password)
        {
            var request = PrepareRequest(new LoginRequest());
            request.UserName = userName;
            request.Password = password;
            var response = _appFacadeProxy.Login(request);

            if (request.RequestId != response.CorrelationId) throw new ApplicationException("Login: RequestId and CorrelationId do not match.");
            if (response.Acknowledge != Acknowlege.Success) throw new ApplicationException(response.Message);
        }

        public void Logout()
        {
            var request = PrepareRequest(new LogoutRequest());
            var response = _appFacadeProxy.Logout(request);

            if (request.RequestId != response.CorrelationId) throw new ApplicationException("Logout: RequestId and CorrelationId do not match.");
            if (response.Acknowledge != Acknowlege.Success) throw new ApplicationException(response.Message);
        }


        public ObservableCollection<FacadeBmo> GetFacades()
        {
            
            var request = PrepareRequest(new FacadeRequest());
            request.LoadOptions = new[] { "Facades" };
            ////request.Criteria = new SchedulerCriteria { SortExpression = sortExpression };

            var response = _appFacadeProxy.GetFacades(request);


            if (request.RequestId != response.CorrelationId) throw new ApplicationException("RequestId and CorrelationId do not match.");
            if (response.Acknowledge != Acknowlege.Success) throw new ApplicationException(response.Message);

            // Move data from DTO to BMO
            var modelList = Mapper.Map<IList<FacadeDto>, IList<FacadeBmo>> (response.Facades);

            // Wrap list in an ObservableCollection
            return new ObservableCollection<FacadeBmo>(modelList); 
            
        }



        public ObservableCollection<SchedulerBmo> GetSchedulers(string sortExpression)
        {
            throw new NotImplementedException();
            //var request = PrepareRequest(new SchedulerRequest());
            //request.LoadOptions = new[] { "Schedulers" };
            ////request.Criteria = new SchedulerCriteria { SortExpression = sortExpression };

            //var response = Client.GetSchedulers(request);


            //if (request.RequestId != response.CorrelationId)
            //    throw new ApplicationException("GetSchedulers: RequestId and CorrelationId do not match.");

            //if (response.Acknowledge != Acknowlege.Success)
            //    throw new ApplicationException(response.Message);

            //// Move Dtos to Bmos
            //var modelList = Mapper.Map<
            //    IList<SchedulerDto>, IList<SchedulerBmo>>(response.Schedulers);

            //// Wrap list in an ObservableCollection
            //return new ObservableCollection<SchedulerBmo>(modelList); //Mapper.FromDataTransferObjects(response.Schedulers, this);
        }

        public SchedulerBmo GetScheduler(int schedulerId)
        {
            //var request = PrepareRequest(new SchedulerRequest());
            //request.LoadOptions = new string[] { "Scheduler" };
            //request.Criteria = new SchedulerCriteria { SchedulerId = schedulerId };

            //var response = Client.GetSchedulers(request);


            //if (request.RequestId != response.CorrelationId)
            //    throw new ApplicationException("GetScheduler: RequestId and CorrelationId do not match.");

            //if (response.Acknowledge != Acknowlege.Success)
            //    throw new ApplicationException(response.Message);

            //return Mapper.FromDataTransferObject(response.Scheduler, this);
            throw new NotImplementedException();
        }


        public int AddScheduler(SchedulerBmo schedulerBmo)
        {
            //var request = PrepareRequest(new SchedulerRequest());
            //request.Action = "Create";
            //request.Scheduler = Mapper.ToDataTransferObject(scheduler);

            //var response = Client.SetSchedulers(request);


            //if (request.RequestId != response.CorrelationId)
            //    throw new ApplicationException("AddScheduler: RequestId and CorrelationId do not match.");

            //if (response.Acknowledge != Acknowlege.Success)
            //    throw new ApplicationException(response.Message);

            //// Update version & new schedulerId
            //scheduler.Version = response.Scheduler.Version;
            //scheduler.SchedulerId = response.Scheduler.SchedulerId;

            //return response.RowsAffected;
            throw new NotImplementedException();
        }

        public int UpdateScheduler(SchedulerBmo schedulerBmo)
        {
            //var request = PrepareRequest(new SchedulerRequest());
            //request.Action = "Update";
            //request.Scheduler = Mapper.ToDataTransferObject(scheduler);

            //var response = Client.SetSchedulers(request);


            //if (request.RequestId != response.CorrelationId)
            //    throw new ApplicationException("UpdateScheduler: RequestId and CorrelationId do not match.");

            //if (response.Acknowledge != Acknowlege.Success)
            //    throw new ApplicationException(response.Message);

            //// Update version
            //scheduler.Version = response.Scheduler.Version;

            //return response.RowsAffected;
            throw new NotImplementedException();
        }

        public int DeleteScheduler(int schedulerId)
        {
            //var request = PrepareRequest(new SchedulerRequest());
            //request.Action = "Delete";
            //request.Criteria = new SchedulerCriteria { SchedulerId = schedulerId };

            //var response = Client.SetSchedulers(request);


            //if (request.RequestId != response.CorrelationId)
            //    throw new ApplicationException("DeleteScheduler: RequestId and CorrelationId do not match.");

            //if (response.Acknowledge != Acknowlege.Success)
            //    throw new ApplicationException(response.Message);

            //return response.RowsAffected;
            throw new NotImplementedException();
        }


        //public ObservableCollection<JobModel> GetJobs(int instanceId)
        //{
        //    var request = PrepareRequest(new JobRequest());

        //    request.LoadOptions = new string[] { "Jobs", "JobDetails", "Trigger" };
        //    request.Criteria = new JobCriteria { SchedulerId = schedulerId, SortExpression = "JobId ASC" };

        //    var response = Client.GetJobs(request);


        //    if (request.RequestId != response.CorrelationId)
        //        throw new ApplicationException("GetJobs: RequestId and CorrelationId do not match.");

        //    if (response.Acknowledge != Acknowlege.Success)
        //        throw new ApplicationException(response.Message);

        //    return Mapper.FromDataTransferObjects(response.Jobs);
        //}

    }
}
