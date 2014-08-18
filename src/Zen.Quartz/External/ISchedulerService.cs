using System;
using Quartz;
using Quartz.Impl;

namespace Zen.Quartz.External
{
    /* ? Should be treated as an 'External Service' ? */
    public interface ISchedulerService
    {

    //    //[OperationContract]
    //    SchedulerResponse Start(SchedulerRequest rx);

    //    //[OperationContract]
    //    SchedulerResponse Shutdown(SchedulerRequest rx);

    //    //[OperationContract]
    //    SchedulerResponse Pause(SchedulerRequest rx);

    //    //[OperationContract]
    //    SchedulerResponse Resume(SchedulerRequest rx);
         
    }

    //public class SchedulerService
    //{
    //    public SchedulerService()
    //    {
    //        _scheduler = new StdSchedulerFactory().GetScheduler();                            
    //    }


    //    private readonly IScheduler _scheduler;

    //    public SchedulerResponse Start(SchedulerRequest rx)
    //    {
    //        var response = new SchedulerResponse();
    //        try
    //        {
    //            _scheduler.Start(); 
    //        }
    //        catch (Exception ex)
    //        {
    //            response.Acknowledge = Acknowlege.Failure;
    //            ex.ToString().LogMe(Zen.Log.LogLevel.Error);
    //        }
    //        if (_scheduler != null)
    //            response.SchedulerDescription = _scheduler.GetDescription();

    //        return response;
    //    }

    //    public SchedulerResponse Shutdown(SchedulerRequest rx)
    //    {
    //        var response = new SchedulerResponse();
    //        try
    //        {
    //            _scheduler.Shutdown();
    //        }
    //        catch (Exception ex)
    //        {
    //            response.Acknowledge = Acknowlege.Failure;
    //            ex.ToString().LogMe(Zen.Log.LogLevel.Error);
    //        }
    //        if (_scheduler != null)
    //            response.SchedulerDescription = _scheduler.GetDescription();

    //        return response;
    //    }

    //    public SchedulerResponse Pause(SchedulerRequest rx)
    //    {
    //        var response = new SchedulerResponse();
    //        try
    //        {
    //            _scheduler.PauseAll();
    //        }
    //        catch (Exception ex)
    //        {
    //            response.Acknowledge = Acknowlege.Failure;
    //            ex.ToString().LogMe(Zen.Log.LogLevel.Error);
    //        }
    //        if (_scheduler != null)
    //            response.SchedulerDescription = _scheduler.GetDescription();

    //        return response;
    //    }

    //    public SchedulerResponse Resume(SchedulerRequest rx)
    //    {
    //        var response = new SchedulerResponse();
    //        try
    //        {
    //            _scheduler.ResumeAll();
    //        }
    //        catch (Exception ex)
    //        {
    //            response.Acknowledge = Acknowlege.Failure;
    //            ex.ToString().LogMe(Zen.Log.LogLevel.Error);
    //        }
    //        if (_scheduler != null)
    //            response.SchedulerDescription = _scheduler.GetDescription();

    //        return response;
    //    }
    //}
}