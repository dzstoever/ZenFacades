using Common.Logging.Simple;
using Zen.Log;
using LogLevel = Common.Logging.LogLevel;

namespace Zen.Quartz
{
    /// <summary>
    /// Wrapper class for Common.Logging that redirects messages to a Zen.Log.ILogger
    /// If the Logger property is not set it will be obtained from Zen.Log.LogProvider
    /// </summary>
    /// <example>
    /// Common.Logging.LogManager.Adapter = new Zen.Quartz.Log.CommonAdapter();
    /// </example>
    /// <see cref="https://bitbucket.org/jawc/jasonsoft/src/c03370110e28/JasonSoft.Core/Components/Logging/Simple/CapturingLoggerFactoryAdapter.cs"/>
    public class CommonAdapter : CapturingLoggerFactoryAdapter
    {
        public ILogger Logger { get; set; }

        public override void AddEvent(CapturingLoggerEvent loggerEvent)
        {
            //base.AddEvent(loggerEvent); - keeps a history of all events...

            if (Logger == null) Logger = Aspects.GetLogger("Common");

            var msg = loggerEvent.RenderedMessage;
            var ex = loggerEvent.Exception;
            
            switch (loggerEvent.Level)
            {                                
                case LogLevel.Info: 
                    if(ex == null) Logger.Info(msg);
                    else Logger.Info(msg, ex); break;                
                
                case LogLevel.Warn: 
                    if(ex == null) Logger.Warn(msg);
                    else Logger.Warn(msg, ex); break;                
                
                case LogLevel.Error: 
                    if(ex == null) Logger.Error(msg);
                    else Logger.Error(msg, ex); break;                
                
                case LogLevel.Fatal: 
                    if(ex == null) Logger.Fatal(msg);
                    else Logger.Fatal(msg, ex); break;
                
                default: //Trace, Debug, Other
                    if(ex == null) Logger.Debug(msg);
                    else Logger.Debug(msg, ex); break;
            }

        }
    }
}
