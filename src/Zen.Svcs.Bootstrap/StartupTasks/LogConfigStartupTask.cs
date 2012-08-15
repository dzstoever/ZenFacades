using System;
using Bootstrap.Extensions.StartupTasks;
using Zen.Log;

namespace Zen.Svcs.Bootstrap.StartupTasks
{
    public class LogConfigStartupTask : IStartupTask
    {
        private static void ConfigureLogging()
        {
            var appenders = new[] { Appenders.Debug, Appenders.File };
            Log4netConfigurator.SetLoggerAppenders("Zen", LogLevel.All, appenders);
            Log4netConfigurator.TurnAppenders(appenders, OnOff.On);
            Log4netConfigurator.TurnLoggerOff("NHibernate");
            Log4netConfigurator.Configure();

            Log4netConfigurator.ErrorHandler = typeof(LoggingErrorHandler);
            Log4netConfigurator.ErrorHandlerAppenders = new[] { Appenders.File };
        }

        public void Run()
        {
            ConfigureLogging();
        }

        public void Reset()
        {
            ConfigureLogging(); // reconfigure to undo anything that changed...
        }

        //nested class
        public class LoggingErrorHandler : Log4netErrorHandler, log4net.Core.IErrorHandler
        {
            public void Error(string message, Exception exc, log4net.Core.ErrorCode errorCode)
            { base.Error(message, exc, errorCode); }
        }

    }

    
}