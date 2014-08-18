using System;
using Topshelf;
using Zen.Log;

namespace Zen.Quartz.Server
{
    /// <summary>
    /// This program can be run as a console application,
    /// or installed as a windows service
    /// </summary>
    class Program
    {
        private const string LogFileName = "Zen.QZ.Host.log";
        private static readonly ILogger log = Aspects.GetLogger(typeof(Program).Namespace);

        static Program()
        {
            Log4netConfigurator.ErrorHandler = typeof(LoggingErrorHandler);
            Log4netConfigurator.ErrorHandlerAppenders = new[] { Appenders.File };
            Log4netConfigurator.FilePath = LogFileName;
            Log4netConfigurator.TurnAppenders(new[] { 
                Appenders.Debug, Appenders.Console, Appenders.File }, OnOff.On);
            Log4netConfigurator.SetLoggerAppenders("Zen", LogLevel.All, new[] { 
                Appenders.Debug, Appenders.Console, Appenders.File });            
            Log4netConfigurator.Configure();
            Common.Logging.LogManager.Adapter = new CommonAdapter();    // On
                //new Common.Logging.Simple.NoOpLoggerFactoryAdapter(); // Off                
        }
        
        static void Main()
        {   Console.Title = "Zen Scheduler Server Host"; 
            Console.WriteLine("");

            log.Debug("Initializing Host...");
            var host = HostFactory.New(x =>
            {
                x.Service<SchedulerServer>(s =>
                {                    
                    s.ConstructUsing(builder =>
                    {
                        var server = new SchedulerServer();
                        server.Initialize();                        
                        return server;
                    });
                    s.WhenStarted(server =>     
                        { log.Debug("Starting SchedulerServer..."); server.Start();     });
                    s.WhenStopped(server =>     
                        { log.Debug("Stopping SchedulerServer..."); server.Shutdown();  });
                    s.WhenPaused(server =>      
                        { log.Debug("Pausing SchedulerServer...");  server.Pause();     });
                    s.WhenContinued(server =>   
                        { log.Debug("Resuming SchedulerServer..."); server.Resume();    });                    
                });

                x.RunAsLocalSystem();
                x.SetServiceName("ZenSchedulerServerHost");
                x.SetDisplayName("Zen Scheduler Server Host");                
                x.SetDescription("The server acts as host for 1 Quartz Scheduler instance.");
            });

            log.InfoFormat("Hosting a {0}.", typeof(SchedulerServer));
            host.Run();

            log.Warn("The Host has exited.");
            System.Threading.Thread.Sleep(3000);            
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
