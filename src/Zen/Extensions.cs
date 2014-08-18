using System;
using System.Reflection;
using Zen.Log;

namespace Zen
{    
    public static class Extensions
    {
        /// <summary>Combines the ex.Message with each inner exception message
        /// on a new line 
        /// </summary>
        public static string FullMessage(this Exception ex)
        {
            var fullMessage = ex.Message;
            var inner = ex.InnerException;
            while (inner != null)
            {
                fullMessage += Environment.NewLine + " * " + inner.Message;
                inner = inner.InnerException;
            }
            return fullMessage;
        }
        

        public static string GetEntryName(this Assembly assembly)
        {
            return Assembly.GetEntryAssembly().GetName().Name; 
        }

        
        public static string GetElapsedTime(this DateTime str, bool includeMs)
        {
            var elapsed = DateTime.Now.Subtract(str).ToString();
            if (elapsed.Contains(".")) 
                return !includeMs ? elapsed.Remove(elapsed.LastIndexOf('.')) : elapsed;
            return elapsed;
        }


        public static string FormatWith(this String str, params object[] args)
        {
            return string.Format(str, args);
        }

        
        public static void LogMe(this String str, LogLevel level, params object[] args)
        {
            if(level == LogLevel.Off) return;

            var log = Aspects.GetLogger();
            switch (level)
            {
                case LogLevel.Debug: log.DebugFormat(str, args);
                    break;
                case LogLevel.Info: log.InfoFormat(str, args);
                    break;
                case LogLevel.Warn: log.WarnFormat(str, args);
                    break;
                case LogLevel.Error: log.ErrorFormat(str, args);
                    break;
                case LogLevel.Fatal: log.Fatal(string.Format(str, args));
                    break;
                case LogLevel.All:
                    log.DebugFormat(str, args);
                    log.InfoFormat(str, args);
                    log.WarnFormat(str, args);
                    log.ErrorFormat(str, args);
                    log.Fatal(string.Format(str, args));
                    break;                
            }           
        }


        /// <summary> return "null", "empty string", or obj.ToString() for any object
        /// </summary>
        public static string ShowNullorEmptyString(this object obj)
        {
            if (obj == null) return "null";
            if (obj is string && (string)obj == "") return "empty string";
            return obj.ToString();
        }

        
    }
}
