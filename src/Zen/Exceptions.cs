using System;
using System.Runtime.Serialization;

namespace Zen
{
    public class BusinessException : ApplicationException
    {
        public BusinessException()
        {
        }

        public BusinessException(string message)
            : base(message)
        {
        }

        public BusinessException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected BusinessException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }

    public class ConfigException : ApplicationException
    {
        public ConfigException()
        {
        }

        public ConfigException(string message)
            : base(message)
        {
        }

        public ConfigException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected ConfigException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }

    public class DataAccessException : ApplicationException
    {
        public DataAccessException()
        {
        }

        public DataAccessException(string message)
            : base(message)
        {
        }

        public DataAccessException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected DataAccessException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }

    public class DependencyException : ApplicationException
    {
        public DependencyException()
        {
        }

        public DependencyException(string message)
            : base(message)
        {
        }

        public DependencyException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected DependencyException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }

    /// <summary>
    /// This exception is intended as a wrapper of any logging related exceptions
    /// Note: it is used by the Log4netErrorHandler
    /// </summary>
    /// <remarks>
    /// When this type of error occurs it is most likely due to an configuration
    /// problem or unavailable resource(such as the database or email server)
    /// </remarks>
    public class LoggingException : ApplicationException
    {
        public LoggingException()
        {
        }

        public LoggingException(string message)
            : base(message)
        {
        }

        public LoggingException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected LoggingException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}