using System;

namespace Zen.Log
{
    /// <summary>
    /// This class can be used when no logging is needed
    /// </summary>
    /// <remarks>All method calls do nothing</remarks>
    public class NoLogger : ILogger
    {
        public string Name
        {
            get { return "NoLogger"; }
        }

        public bool IsDebugEnabled
        {
            get { return false; }
        }
        public bool IsInfoEnabled
        {
            get { return false; }
        }
        public bool IsWarnEnabled
        {
            get { return false; }
        }
        public bool IsErrorEnabled
        {
            get { return false; }
        }
        public bool IsFatalEnabled
        {
            get { return false; }
        }
        
        public void Debug(object message)
        {
        }
        public void Debug(object message, Exception exception)
        {
        }
        public void DebugFormat(string format, params object[] args)
        {
        }

        public void Info(object message)
        {
        }
        public void Info(object message, Exception exception)
        {
        }
        public void InfoFormat(string format, params object[] args)
        {
        }
        
        public void Warn(object message)
        {
        }
        public void Warn(object message, Exception exception)
        {
        }
        public void WarnFormat(string format, params object[] args)
        {
        }

        public void Error(object message)
        {
        }
        public void Error(object message, Exception exception)
        {
        }
        public void ErrorFormat(string format, params object[] args)
        {
        }

        public void Fatal(object message)
        {
        }
        public void Fatal(object message, Exception exception)
        { 
        }
        
    }

    public class NoLoggerFactory : ILoggerFactory
    {
        private static readonly ILogger NoLogger = new NoLogger();

        public ILogger Create()
        {
            return NoLogger;
        }

        public ILogger Create(string name)
        {
            return NoLogger;
        }

        public ILogger Create(Type type)
        {
            return NoLogger;
        }
    }
}