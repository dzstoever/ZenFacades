using Xunit;
using System;
using Zen.Log;

namespace Zen.Xunit.Tests
{

    /// <summary>
    /// Base class for other scenarios providing a log component
    /// </summary>
    public class UseLogFixture : IUseFixture<LogFixture>
    {
        public void SetFixture(LogFixture data) { }
        protected static readonly ILogger Log = Aspects.GetLogger();
    }

    /// <summary>
    /// Configures logging for any typical Test class.
    /// Allows us to use the same logging configuration for any test classes while calling .Configure() only once.
    /// </summary>    
    public class LogFixture : IDisposable
    {
        static LogFixture()
        {
            //dynamic T_CommonLoggingLogManager...

            //Common.Logging.LogManager.Adapter = //new Common.Logging.Simple.NoOpLoggerFactoryAdapter(); // Off
            //new CommonLoggingAdapter();// On


            Log4netConfigurator.DefaultPattern = "|%-5level| %message %n";
            Log4netConfigurator.RootLogLevel = LogLevel.Debug;
            Log4netConfigurator.TurnAppender(Appenders.Trace, OnOff.On);
            Log4netConfigurator.TurnLoggerOff("NHibernate");
            Log4netConfigurator.TurnLoggerOff("NHibernate.SQL"); //Note: may be superceded by .ShowSql() in our FluentNHibernate Configuration
            Log4netConfigurator.Configure();

            Aspects.GetLogger("LoggingFixture").DebugFormat("Logging configured."); //this message should be in the debug window (only once)
        }

        public void Dispose()
        { }
    }

}

