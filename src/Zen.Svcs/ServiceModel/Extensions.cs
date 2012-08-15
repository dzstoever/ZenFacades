using System;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Zen.Svcs.ServiceModel
{
    /// <summary>
    /// Adds startup functionality to the given ServiceHost, allowing
    /// for initialization logic to be provided by an IStartup shell.
    /// </summary>
    /// <remarks>
    /// IExtension 
    ///     where T: The object that participates in the custom behavior.
    ///     Enables an object to extend another object through aggregation.
    /// </remarks>
    public class StartupExtension :  IExtension<ServiceHostBase>, IDisposable
    {
        public StartupExtension(IStartup startupShell)
        {
            _startupShell = startupShell;
        }
        private readonly IStartup _startupShell;
                
        /// <summary>
        /// Enables an extension object to find out when it has been aggregated. 
        /// Called when the extension is added to the owner.
        /// </summary>
        /// <param name="owner">The extensible object that aggregates this extension.</param>
        void IExtension<ServiceHostBase>.Attach(ServiceHostBase owner)
        {
            if (_startupShell == null) throw new ConfigException("startupShell can not be null.");
            _startupShell.Startup();            
        }

        /// <summary>
        /// Enables an object to find out when it is no longer aggregated. 
        /// Called when an extension is removed from the owner.
        /// </summary>
        /// <param name="owner">The extensible object that aggregates this extension.</param>
        void IExtension<ServiceHostBase>.Detach(ServiceHostBase owner)
        {
            Dispose();
        }
        
        public virtual void Dispose()
        {//no-op 
        }

    }

    /// <summary>
    /// Adds Inversion of Control (dependency injection) capabilities to the given ServiceHost
    /// by setting the InstanceProvider property for all Endpoints of all ChannelDispatchers  
    /// to use the Zen.Svc.IocInstanceProvider when creating service instances. 
    /// </summary>
    /// <remarks>
    /// IExtension 
    ///     where T: The object that participates in the custom behavior.
    ///     Enables an object to extend another object through aggregation.    
    /// </remarks>
    public class IocExtension : IExtension<ServiceHostBase>, IDisposable
    {
        public IocExtension(ServiceDescription serviceDescription)
        {
            _serviceDescription = serviceDescription;
        }
        private readonly ServiceDescription _serviceDescription;

        /// <summary>
        /// Enables an extension object to find out when it has been aggregated. 
        /// Called when the extension is added to the owner.
        /// </summary>
        /// <param name="owner">The extensible object that aggregates this extension.</param>
        void IExtension<ServiceHostBase>.Attach(ServiceHostBase owner)
        {
            if (_serviceDescription == null) throw new ConfigException("serviceDescription can not be null.");

            foreach (var channelDispatcherBase in owner.ChannelDispatchers)
            {
                var channelDispatcher = channelDispatcherBase as ChannelDispatcher;
                if (channelDispatcher == null) continue;
                
                foreach (var endpointDispatcher in channelDispatcher.Endpoints)
                {
                    if (endpointDispatcher == null) continue;
                    foreach (var endpoint in _serviceDescription.Endpoints.Where(endpoint => 
                        endpointDispatcher.ContractName == endpoint.Contract.Name &&
                        endpointDispatcher.ContractNamespace == endpoint.Contract.Namespace))

                        endpointDispatcher.DispatchRuntime.InstanceProvider =
                            new IocInstanceProvider(endpoint.Contract.ContractType);
                }
            }
        }

        /// <summary>
        /// Enables an object to find out when it is no longer aggregated. 
        /// Called when an extension is removed from the owner.
        /// </summary>
        /// <param name="owner">The extensible object that aggregates this extension.</param>
        void IExtension<ServiceHostBase>.Detach(ServiceHostBase owner)
        {
            Dispose();
        }

        public void Dispose()
        {//no-op 
        }

    }
}