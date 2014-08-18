using System;
using Topshelf.Configuration.Dsl;
using Topshelf.Shelving;
using Zen.Log;

namespace Zen.Quartz.Shelf
{
    /// <summary>
    /// We should be able to copy this dll into the TopShelf 
    /// 'Services' folder and have a RemoteScheduler available
    /// </summary>
    /// <remarks>It becomes a wcf service hosted by the TopShelf.Host</remarks>
    public class ShelvedSchedulerServer : Bootstrapper<SchedulerServer>
    {
        private const string LogFileName = "Zen.QZ.Shelf.log";
        private static readonly ILogger log = Aspects.GetLogger(typeof(ShelvedSchedulerServer).Namespace);
        
        static ShelvedSchedulerServer()
        {
            Log4netConfigurator.ErrorHandler = typeof(LoggingErrorHandler);
            Log4netConfigurator.ErrorHandlerAppenders = new[] { Appenders.File };
            Log4netConfigurator.FilePath = @"..\..\logs\" + LogFileName;// log file can't be in the directory being monitored
            Log4netConfigurator.TurnAppenders(new[] { 
                Appenders.Debug, Appenders.Console, Appenders.File }, OnOff.On);
            Log4netConfigurator.SetLoggerAppenders("Zen", LogLevel.All, new[] { 
                Appenders.Debug, Appenders.Console, Appenders.File });            
            Log4netConfigurator.Configure();
            Common.Logging.LogManager.Adapter = new CommonAdapter(); // On 
                //new Common.Logging.Simple.NoOpLoggerFactoryAdapter();     // Off                
        }

        public void InitializeHostedService(IServiceConfigurator<SchedulerServer> cfg)
        {
            log.Debug("Initializing Hosted Service...");
            cfg.HowToBuildService(n => 
            {
                var server = new SchedulerServer();
                server.Initialize();
                return server;
            });
            cfg.WhenStarted(server =>   { log.Debug("Starting SchedulerServer..."); server.Start(); });
            cfg.WhenStopped(server =>   { log.Debug("Stopping SchedulerServer..."); server.Shutdown(); });
            cfg.WhenPaused(server =>    { log.Debug("Pausing SchedulerServer...");  server.Pause(); });
            cfg.WhenContinued(server => { log.Debug("Resuming SchedulerServer..."); server.Resume(); });
        }

    }


    public class LoggingErrorHandler : Log4netErrorHandler, log4net.Core.IErrorHandler
    {
        //handles logging errors, not application errors
        public void Error(string message, Exception exc, log4net.Core.ErrorCode errorCode)
        {
            base.Error(message, exc, errorCode); //logs message to the assigned appender(s)
        }
    }
}