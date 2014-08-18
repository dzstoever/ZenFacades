using System;
using System.Collections.Specialized;
using Quartz;
using Quartz.Impl;
using Zen.Log;

namespace Zen.Quartz
{
    /// <summary>
    /// A wrapper around the StdSchedulerFactory that provides some built-in Scheduler configurations
    /// </summary>
    /// <remarks>
    /// With no configuration props provided the StdSchedulerFactory will:
    /// 1) check for <quartz /> section in app.config (AVOID CUSTOM SECTION HANDLERS!)
    /// 2) check for a value in the Environment variable named 'quartz.config' 
    /// 3) check for a file named quartz.config in the current working directory (Preferred method)
    /// 4) load default properties from embedded resource file in Quartz.dll
    /// 
    /// Note: custom properties can always be passed in the overloaded constructor.
    /// </remarks>
    public class SchedulerFactory : StdSchedulerFactory
    {
        private static readonly ILogger log = Aspects.GetLogger(typeof(SchedulerFactory));

                
        //public SchedulerFactory()
        //    : base()
        //{ }

        //public SchedulerFactory(NameValueCollection props)
        //    : base(props)
        //{ }


        //public override IScheduler GetScheduler()
        //{
        //    return base.GetScheduler();
        //}

        const string DefaultInstanceName = "ZenScheduler";
        const string DefaultHost = "127.0.0.1"; //localhost
        const string DefaultPort = "555";
        const string DefaultBindName = "QuartzScheduler";
        
        /// <summary>
        /// Creates an IScheduler instance (Server-side) 
        /// and uses a Quartz.Simpl.RemotingSchedulerExporter to bind the scheduler and export it to a remoting context
        /// 
        /// If any params are null the Default value is used. 
        /// Default Uri => tcp://127.0.0.1:555/QuartzScheduler
        /// </summary>
        /// <remarks>
        /// using System.Runtime.Remoting;
        /// using System.Runtime.Remoting.Channels;
        /// using System.Runtime.Remoting.Channels.Http;
        /// using System.Runtime.Remoting.Channels.Tcp;
        /// using System.Runtime.Serialization.Formatters; 
        /// </remarks>        
        public static IScheduler CreateServerSideSched(string instanceName, int? port, string bindName)
        {
            var props = new NameValueCollection();
            props["quartz.scheduler.instanceName"] = instanceName ?? DefaultInstanceName;
            props["quartz.scheduler.exporter.port"] = (port != null) ? port.ToString() : DefaultPort;
            props["quartz.scheduler.exporter.type"] = "Quartz.Simpl.RemotingSchedulerExporter, Quartz";
            props["quartz.scheduler.exporter.bindName"] = bindName ?? DefaultBindName;

            log.Debug("Creating server-side scheduler instance.");
            return new StdSchedulerFactory(props).GetScheduler();                        
        }

        /// <summary>
        /// Creates an IScheduler instance, (Client-side)
        /// to control a scheduler operating in a remoting context (Server-side)
        /// </summary>
        /// <returns>Quartz.Impl.RemoteScheduler</returns>
        public static IScheduler CreateClientSideSched(string host, int? port, string bindName)
        {
            var props = new NameValueCollection();
            props["quartz.scheduler.proxy"] = "true";
            props["quartz.scheduler.proxy.address"] = string.Format("tcp://{0}:{1}/{2}", 
                host ?? DefaultHost, (port != null) ? port.ToString() : DefaultPort, bindName ?? DefaultBindName);

            log.Debug("Creating client-side scheduler instance.");
            return new StdSchedulerFactory(props).GetScheduler();// as Quartz.Impl.RemoteScheduler;
        }


        //Todo: create a local scheduler using the factory
        public static IScheduler CreateLocalSched()
        {
            throw new NotImplementedException();
        }
    }


    //public enum SchedulerCfg
    //{
    //    DefaultServerSide,
    //    DefaultClientSide,
    //    DefaultLocal
    //}


    /* standard configuration property reference      
      
     *  ConfigurationKeyPrefix =        "quartz.";
            CheckConfiguration =        "quartz.checkConfiguration";    
            SchedulerContextPrefix =    "quartz.context.key";
            ObjectSerializer =          "quartz.serializer";        
            PluginPrefix =              "quartz.plugin";
              PluginType =              "type"; (in section group)            
            ThreadPoolPrefix =          "quartz.threadPool";
              ThreadPoolType =          "quartz.threadPool.type";
            ThreadExecutor =            "quartz.threadExecutor";
              ThreadExecutorType=       "quartz.threadExecutor.type";
     
     * SchedulerThreadName =                        "quartz.scheduler.threadName";
        SchedulerMakeSchedulerThreadDaemon =        "quartz.scheduler.makeSchedulerThreadDaemon";
     
     * SchedulerInstanceName =                      "quartz.scheduler.instanceName";
        SchedulerInstanceId =                       "quartz.scheduler.instanceId";
        SchedulerProxy =                            "quartz.scheduler.proxy";
          SchedulerProxyType =                      "quartz.scheduler.proxy.type";                    
        SchedulerExporterPrefix =                   "quartz.scheduler.exporter";
          SchedulerExporterType =                   "quartz.scheduler.exporter.type";     
        SchedulerInstanceIdGeneratorPrefix =        "quartz.scheduler.instanceIdGenerator";
          SchedulerInstanceIdGeneratorType =        "quartz.scheduler.instanceIdGenerator.type";
        SchedulerIdleWaitTime =                     "quartz.scheduler.idleWaitTime";    
        SchedulerBatchTimeWindow =                  "quartz.scheduler.batchTriggerAcquisitionFireAhead
                                                    ...TimeWindow";
        SchedulerMaxBatchSize =                     "quartz.scheduler.batchTriggerAcquisitionMaxCount";
        SchedulerDbFailureRetryInterval =           "quartz.scheduler.dbFailureRetryInterval";         
        SchedulerInterruptJobsOnShutdown =          "quartz.scheduler.interruptJobsOnShutdown";
        SchedulerInterruptJobsOnShutdownWithWait =  "quartz.scheduler.interruptJobsOnShutdownWithWait";        
        SchedulerTypeLoadHelperType =               "quartz.scheduler.typeLoadHelper.type";       
      
           
     * SchedulerJobFactoryPrefix =      "quartz.scheduler.jobFactory";   
         SchedulerJobFactoryType =      "quartz.scheduler.jobFactory.type";
      
     *  JobListenerPrefix =             "quartz.jobListener";        
     *  TriggerListenerPrefix =         "quartz.triggerListener";
          ListenerType =                "type"; (in section group) 
     
     * DbProviderType =                 "connectionProvider.type";
      
     * DataSourcePrefix =               "quartz.dataSource";      
       DataSourceProvider =             "provider";
       DataSourceConnectionString =     "connectionString";
       DataSourceConnectionStringName = "connectionStringName";
      
     * JobStorePrefix =                 "quartz.jobStore";         
         JobStoreType =                 "quartz.jobStore.type";
         JobStoreLockHandlerPrefix =    "quartz.jobStore.lockHandler";
         JobStoreLockHandlerType =      "quartz.jobStore.lockHandler.type";        
         SchedulerName =                "schedName"; <- yes this goes here
         TablePrefix =                  "tablePrefix";

     *  PropertiesFile = "quartz.config";      
     *  SystemPropertyAsInstanceId = "SYS_PROP";
     *  AutoGenerateInstanceId = "AUTO";
     *  DefaultInstanceId = "NON_CLUSTERED";
    */
}