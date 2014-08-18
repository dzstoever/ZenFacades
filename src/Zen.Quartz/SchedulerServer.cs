using System;
using Quartz;
using Zen.Log;

namespace Zen.Quartz
{
    /// <summary>
    /// This server acts as host for 1 scheduler instance.
    /// </summary>
    public class SchedulerServer : IDisposable
    {
        public IScheduler Scheduler { get; private set; }

        private readonly ILogger log = Aspects.GetLogger(typeof(SchedulerServer));

        
        public void Initialize()
        {
            try
            {
                // Gets the scheduler instance which this server should operate.
                Scheduler = SchedulerFactory.CreateServerSideSched(null, null, null);
                log.Info("Server Initialization Complete.");
            }
            catch (Exception e)
            {
                log.Error("Server Initialization Failed!" + e.Message, e);
                throw;
            }
        }


        public void Start()
        {
            Scheduler.Start();
            log.Info("Started Scheduler.");
            log.Debug(Scheduler.GetDescription());
        }        

        public void Shutdown()
        {
            Scheduler.Shutdown(true);
            log.Info("Shutdown Scheduler.");

            //SchedulerMetaData metaData = Scheduler.GetMetaData();
            //log.Info("Executed " + metaData.NumberOfJobsExecuted + " jobs.");
        }

        public void Pause()
        {
            Scheduler.PauseAll();
            log.Info("Paused Scheduler.");
        }

        public void Resume()
        {
            Scheduler.ResumeAll();
            log.Info("Resumed Scheduler.");
        }


        public void Dispose()
        {
            // shutdown the scheduler (don't wait for jobs to complete)
            if (!Scheduler.IsShutdown) Scheduler.Shutdown(false);
            Scheduler = null;
            log.Info("--- Server Disposed ---");
        }

    }

    //public interface ISchedulerServer : IDisposable
    //{
    //    /// <summary>
    //    /// Initializes the instance, 
    //    /// should be called only once in server's lifetime.
    //    /// </summary>
    //    void Initialize();

    //    /// <summary>
    //    /// Starts this instance.
    //    /// </summary>
    //    void Start();

    //    /// <summary>
    //    /// Stops this instance.
    //    /// </summary>
    //    void Shutdown();

    //    /// <summary>
    //    /// Pauses all activity in scheduler.
    //    /// </summary>
    //    void Pause();

    //    /// <summary>
    //    /// Resumes all acitivity in scheduler.
    //    /// </summary>
    //    void Resume();
    //}
}