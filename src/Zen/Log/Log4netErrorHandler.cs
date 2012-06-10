using System;
using System.Collections.Generic;
using System.Text;

namespace Zen.Log
{
    /// <summary>
    /// Class to handle and report logging errors not related to the application.
    /// In other words, the app did not have an error, the logger failed to log a message.
    /// <para>
    /// This class must be inherited in the calling application by a class that implements
    /// log4net.Core.IErrorHandler and have the Log4netConfigurator.ErrorHandler property set to 
    /// the implementation class. It's base functionality can still be used including the 
    /// ability to send logging errors to any appenders in the .ErrorHandlerAppenders list.
    /// </para>
    /// </summary>
    /// <remarks>
    /// <para>
    /// Log4net never actually throws an exception back to the application.
    /// Error events from any of our appenders are caught here.
    /// </para>
    /// </remarks>
    /// <example>
    /// public class MyLoggingErrorHandler : Log4netErrorHandler, log4net.Core.IErrorHandler
    /// {
    ///     //Note: the following COULD also be overriden
    ///     //void Error(string message, System.Exception exc)
    ///     //void Error(string message)
    /// 
    ///     //This MUST be overriden
    ///     public void Error(string message, System.Exception exc, log4net.Core.ErrorCode errorCode)
    ///     { base.Error(message, exc, errorCode); }
    /// }
    /// 
    /// ...and somwhere in the app startup
    /// Log4netConfigurator.ErrorHandler = typeof(MyLoggingErrorHandler);
    /// Log4netConfigurator.ErrorHandlerAppenders = new Appenders[] { Appenders.File };
    /// Log4netConfigurator.AppenderSwitch(Appenders.File, OnOff.On, true);
    /// </example>
    public abstract class Log4netErrorHandler //: log4net.Core.IErrorHandler
    {
        /// <summary>
        /// This log should be assigned to the appender deemed 'most-reliable' since 
        /// it's only responsibility is to report on errors with the other appenders.
        /// </summary>
        const string LoggerName = "Log4netErrorHandler";

        //private static ILogger log;

        /// <summary>
        /// Indicator that 1 or more errors has occured when logging messages
        /// </summary>
        /// <remarks>
        /// In other words, the app did not have an error, the logger failed to log a message. 
        /// </remarks>
        public static bool HasEncounteredErrors { get; private set; }
        
        /// <summary>
        /// Combines all logging exception messages into on Exception
        /// </summary>
        public static Exception ExceptionSummary
        {
            get
            {
                if (Exceptions == null) return null;

                var sb = new StringBuilder();
                foreach (var ex in Exceptions)
                {
                    sb.AppendLine(ex.Message);
                    var innerExc = ex.InnerException;
                    while (innerExc != null)
                    {
                        sb.AppendLine(" --> " + innerExc.Message);
                        innerExc = innerExc.InnerException;
                    }
                }
                return new LoggingException(sb.ToString());
            }
        }
        
        /// <summary>
        /// List of all errors that have occured
        /// </summary>
        public static IList<Exception> Exceptions { get; private set; }
        
        //helper
        private static void AddToExceptionList(string message, Exception exc)
        {
            if (Exceptions == null) Exceptions = new List<Exception>();
            Exceptions.Add(new LoggingException(message, exc));
            HasEncounteredErrors = true;
        }


        
        #region log4net.Core.IErrorHandler Members

        public void Error(string message)
        {
            AddToExceptionList(message, null);
            Aspects.GetLogger(LoggerName).Error(message);
        }

        public void Error(string message, Exception exc)
        {
            AddToExceptionList(message, exc);
            Aspects.GetLogger(LoggerName).Error(message, exc);
        }

        //Note: this is the only method that must be overriden
        public void Error(string message, Exception exc, dynamic errorCode) //<- log4net.Core.ErrorCode errorCode)
        {
            var msg = string.Format("[{0}] {1}", errorCode, message);
            AddToExceptionList(msg, exc);
            Aspects.GetLogger(LoggerName).Error(msg, exc);
        }

        #endregion

    }
}