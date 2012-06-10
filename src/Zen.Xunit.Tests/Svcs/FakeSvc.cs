using System.ServiceModel;
using Zen.Log;
using Zen.Svcs.ServiceModel;

namespace Zen.Xunit.Tests
{
    [ServiceContract]
    public interface IFakeSvc
    {
        [OperationContract]
        [FaultContract(typeof(ConfigException))]
        void TraceOperation(string message);

        [OperationContract]
        [FaultContract(typeof(ConfigException))]
        void LogOperation(string message);
    }


    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, IncludeExceptionDetailInFaults = true)]    
    public class FakeSvc : IFakeSvc
    {
        private static void Trace(string msg, params object[] args)
        {
            System.Diagnostics.Trace.WriteLine(string.Format(msg, args));
        }

        public FakeSvc()
        {
        }

        public FakeSvc(ILogger logger)
        {   
            Logger = logger;
        }

        protected readonly ILogger Logger;


        public void TraceOperation(string message)
        {
            Trace(message);
        }

        public void LogOperation(string message)
        {   
            if (Logger == null) throw new ConfigException("Logger is null.");
            Logger.Debug(message);                        
        }
    }

    //[StartupServiceBehavior(StartupShellType = typeof(TestStartupShell))] Note: IocServiceBehavior encapsulates StartupServiceBehavior
    [IocServiceBehavior(StartupShellType = typeof(TestStartupShell))]
    public class FakeSvcWithCustomAttributes : FakeSvc
    {
        public FakeSvcWithCustomAttributes() { }

        public FakeSvcWithCustomAttributes(ILogger logger) : base(logger) { }
    }
}
