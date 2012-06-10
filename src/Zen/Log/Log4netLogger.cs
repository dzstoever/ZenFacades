using System;
using System.Linq.Expressions;

namespace Zen.Log
{
    public class Log4netLogger : ILogger
    {
        private static readonly Type ILogType = Type.GetType("log4net.ILog, log4net");
        private static readonly Type ILoggerType = Type.GetType("log4net.Core.ILogger, log4net");
        private static readonly Type ILoggerWrapperType = Type.GetType("log4net.Core.ILoggerWrapper, log4net");

        private static readonly Func<object, object> LoggerDelegate;
        private static readonly Func<object, string> NameDelegate;

        private static readonly Func<object, bool> IsDebugEnabledDelegate;
        private static readonly Func<object, bool> IsInfoEnabledDelegate;
        private static readonly Func<object, bool> IsWarnEnabledDelegate;
        private static readonly Func<object, bool> IsErrorEnabledDelegate;
        private static readonly Func<object, bool> IsFatalEnabledDelegate;

        private static readonly Action<object, object> DebugDelegate;
        private static readonly Action<object, object, Exception> DebugExceptionDelegate;
        private static readonly Action<object, string, object[]> DebugFormatDelegate;

        private static readonly Action<object, object> InfoDelegate;
        private static readonly Action<object, object, Exception> InfoExceptionDelegate;
        private static readonly Action<object, string, object[]> InfoFormatDelegate;

        private static readonly Action<object, object> WarnDelegate;
        private static readonly Action<object, object, Exception> WarnExceptionDelegate;
        private static readonly Action<object, string, object[]> WarnFormatDelegate;

        private static readonly Action<object, object> ErrorDelegate;
        private static readonly Action<object, object, Exception> ErrorExceptionDelegate;
        private static readonly Action<object, string, object[]> ErrorFormatDelegate;

        private static readonly Action<object, object> FatalDelegate;
        private static readonly Action<object, object, Exception> FatalExceptionDelegate;

        static Log4netLogger()
        {
            LoggerDelegate = GetPropertyGetter<object>("Logger", ILoggerWrapperType);
            NameDelegate = GetPropertyGetter<string>("Name", ILoggerType);

            IsErrorEnabledDelegate = GetPropertyGetter<bool>("IsErrorEnabled", ILogType);
            IsFatalEnabledDelegate = GetPropertyGetter<bool>("IsFatalEnabled", ILogType);
            IsDebugEnabledDelegate = GetPropertyGetter<bool>("IsDebugEnabled", ILogType);
            IsInfoEnabledDelegate = GetPropertyGetter<bool>("IsInfoEnabled", ILogType);
            IsWarnEnabledDelegate = GetPropertyGetter<bool>("IsWarnEnabled", ILogType);
            ErrorDelegate = GetMethodCallForMessage("Error");
            ErrorExceptionDelegate = GetMethodCallForMessageException("Error");
            ErrorFormatDelegate = GetMethodCallForMessageFormat("ErrorFormat");

            FatalDelegate = GetMethodCallForMessage("Fatal");
            FatalExceptionDelegate = GetMethodCallForMessageException("Fatal");

            DebugDelegate = GetMethodCallForMessage("Debug");
            DebugExceptionDelegate = GetMethodCallForMessageException("Debug");
            DebugFormatDelegate = GetMethodCallForMessageFormat("DebugFormat");

            InfoDelegate = GetMethodCallForMessage("Info");
            InfoExceptionDelegate = GetMethodCallForMessageException("Info");
            InfoFormatDelegate = GetMethodCallForMessageFormat("InfoFormat");

            WarnDelegate = GetMethodCallForMessage("Warn");
            WarnExceptionDelegate = GetMethodCallForMessageException("Warn");
            WarnFormatDelegate = GetMethodCallForMessageFormat("WarnFormat");
        }

        
        /// <typeparam name="TP">property Type</typeparam>        
        private static Func<object, TP> GetPropertyGetter<TP>(string propertyName, Type objectType)
        {
            ParameterExpression funcParam = Expression.Parameter(typeof(object), "f");
            Expression convertedParam = Expression.Convert(funcParam, objectType);
            Expression property = Expression.Property(convertedParam, propertyName);
            return (Func<object, TP>)Expression.Lambda(property, funcParam).Compile();
        }

        private static Action<object, object> GetMethodCallForMessage(string methodName)
        {
            ParameterExpression loggerParam = Expression.Parameter(typeof(object), "l");
            ParameterExpression messageParam = Expression.Parameter(typeof(object), "o");
            Expression convertedParam = Expression.Convert(loggerParam, ILogType);
            MethodCallExpression methodCall = Expression.Call(convertedParam, ILogType.GetMethod(methodName, new[] { typeof(object) }), messageParam);
            return (Action<object, object>)Expression.Lambda(methodCall, new[] { loggerParam, messageParam }).Compile();
        }

        private static Action<object, object, Exception> GetMethodCallForMessageException(string methodName)
        {
            ParameterExpression loggerParam = Expression.Parameter(typeof(object), "l");
            ParameterExpression messageParam = Expression.Parameter(typeof(object), "o");
            ParameterExpression exceptionParam = Expression.Parameter(typeof(Exception), "e");
            Expression convertedParam = Expression.Convert(loggerParam, ILogType);
            MethodCallExpression methodCall = Expression.Call(convertedParam, ILogType.GetMethod(methodName, new[] { typeof(object), typeof(Exception) }), messageParam, exceptionParam);
            return (Action<object, object, Exception>)Expression.Lambda(methodCall, new[] { loggerParam, messageParam, exceptionParam }).Compile();
        }

        private static Action<object, string, object[]> GetMethodCallForMessageFormat(string methodName)
        {
            ParameterExpression loggerParam = Expression.Parameter(typeof(object), "l");
            ParameterExpression formatParam = Expression.Parameter(typeof(string), "f");
            ParameterExpression parametersParam = Expression.Parameter(typeof(object[]), "p");
            Expression convertedParam = Expression.Convert(loggerParam, ILogType);
            MethodCallExpression methodCall = Expression.Call(convertedParam, ILogType.GetMethod(methodName, new[] { typeof(string), typeof(object[]) }), formatParam, parametersParam);
            return (Action<object, string, object[]>)Expression.Lambda(methodCall, new[] { loggerParam, formatParam, parametersParam }).Compile();
        }


        private readonly object log;//log4net.Core.LogImpl

        private object Logger//log4net.Core.ILogger
        {
            get { return LoggerDelegate(log); }
        }


        public Log4netLogger()
        {
            var getLoggerByNameDelegate = Log4netLoggerFactory.GetGetLoggerMethodCall<string>();
            log = getLoggerByNameDelegate(Log4netConfigurator.StandardLogName);
        }
        

        public Log4netLogger(object logger)
        {
            log = logger;
        }

        public string Name
        {
            get { return NameDelegate(Logger); }
        }

        public bool IsDebugEnabled
        {
            get { return IsDebugEnabledDelegate(log); }
        }

        public bool IsInfoEnabled
        {
            get { return IsInfoEnabledDelegate(log); }
        }

        public bool IsWarnEnabled
        {
            get { return IsWarnEnabledDelegate(log); }
        }

        public bool IsErrorEnabled
        {
            get { return IsErrorEnabledDelegate(log); }
        }

        public bool IsFatalEnabled
        {
            get { return IsFatalEnabledDelegate(log); }
        }

        public void Debug(object message)
        {
            if (IsDebugEnabled)
                DebugDelegate(log, message);
        }

        public void Debug(object message, Exception exception)
        {
            if (IsDebugEnabled)
                DebugExceptionDelegate(log, message, exception);
        }

        public void DebugFormat(string format, params object[] args)
        {
            if (IsDebugEnabled)
                DebugFormatDelegate(log, format, args);
        }

        public void Info(object message)
        {
            if (IsInfoEnabled)
                InfoDelegate(log, message);
        }

        public void Info(object message, Exception exception)
        {
            if (IsInfoEnabled)
                InfoExceptionDelegate(log, message, exception);
        }

        public void InfoFormat(string format, params object[] args)
        {
            if (IsInfoEnabled)
                InfoFormatDelegate(log, format, args);
        }

        public void Warn(object message)
        {
            if (IsWarnEnabled)
                WarnDelegate(log, message);
        }

        public void Warn(object message, Exception exception)
        {
            if (IsWarnEnabled)
                WarnExceptionDelegate(log, message, exception);
        }

        public void WarnFormat(string format, params object[] args)
        {
            if (IsWarnEnabled)
                WarnFormatDelegate(log, format, args);
        }

        public void Error(object message)
        {
            if (IsErrorEnabled)
                ErrorDelegate(log, message);
        }

        public void Error(object message, Exception exception)
        {
            if (IsErrorEnabled)
                ErrorExceptionDelegate(log, message, exception);
        }

        public void ErrorFormat(string format, params object[] args)
        {
            if (IsErrorEnabled)
                ErrorFormatDelegate(log, format, args);
        }

        public void Fatal(object message)
        {
            if (IsFatalEnabled)
                FatalDelegate(log, message);
        }

        public void Fatal(object message, Exception exception)
        {
            if (IsFatalEnabled)
                FatalExceptionDelegate(log, message, exception);
        }
    }

    public class Log4netLoggerFactory : ILoggerFactory
    {
        private static readonly Type LogManagerType = Type.GetType("log4net.LogManager, log4net");
        private static readonly Func<string, object> GetLoggerByNameDelegate;
        private static readonly Func<Type, object> GetLoggerByTypeDelegate;

        static Log4netLoggerFactory()
        {
            GetLoggerByNameDelegate = GetGetLoggerMethodCall<string>();
            GetLoggerByTypeDelegate = GetGetLoggerMethodCall<Type>();
        }

        public ILogger Create()
        {
            return new Log4netLogger(GetLoggerByNameDelegate(Log4netConfigurator.StandardLogName));
        }

        public ILogger Create(string name)
        {
            return new Log4netLogger(GetLoggerByNameDelegate(name));
        }

        public ILogger Create(Type type)
        {
            return new Log4netLogger(GetLoggerByTypeDelegate(type));
        }

        internal static Func<TParameter, object> GetGetLoggerMethodCall<TParameter>()
        {
            var method = LogManagerType.GetMethod("GetLogger", new[] { typeof(TParameter) });
            ParameterExpression resultValue;
            ParameterExpression keyParam = Expression.Parameter(typeof(TParameter), "key");
            MethodCallExpression methodCall = Expression.Call(null, method, new Expression[] { resultValue = keyParam });
            return Expression.Lambda<Func<TParameter, object>>(methodCall, new[] { resultValue }).Compile();
        }
    }
}