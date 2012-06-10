using System;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace Zen.Svcs.ServiceModel
{    

    /// <summary>
    /// Adds the StartupExtension to any ServiceHost allowing for a custom IStartup 
    /// implementation(StartupShell) or type(StartupShellType) to be specified
    /// 
    /// * service classes can be decorated in code with this behavior as an attribute.
    /// </summary>    
    /// <remarks>
    /// IServiceBehavior
    ///     Provides a mechanism to modify or insert custom extensions across an entire service, 
    ///     including the System.ServiceModel.ServiceHostBase. 
    ///     Important: has no bearing on the service contract, WCF behaviors only
    ///                affect the internals of the service implementation (class).
    /// * Attribute               
    ///     Represents the base class for custom attributes.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class StartupServiceBehavior : Attribute, IServiceBehavior
    {
        #region Custom Attribute properties - set in the attribute decorator

        /// <summary>
        /// IStartup implementation type. The Startup() method of this object will be called once per service host.
        /// </summary>        
        public virtual Type StartupShellType
        {
            get { return _startupShellType; }
            set
            {
                if (value == null) throw new ConfigException("StartupShellType can not be null.");
                _startupShellType = value;

                StartupShell = Activator.CreateInstance(_startupShellType) as IStartup;
                if (StartupShell == null) throw new ConfigException("{0} does not implement IStartup.".FormatWith(StartupShellType));
            }
        }
        private Type _startupShellType;

        #endregion


        /// <summary>
        /// IStartup implementation. The Startup() method of this object will be called once per service host.
        /// </summary>
        public IStartup StartupShell { protected get; set; }
        

        /// <remarks>
        /// Main extension point for custom service behaviours.
        ///     Provides the ability to change run-time property values or insert custom extension objects 
        ///     such as error handlers, message or parameter interceptors, security extensions, and other 
        ///     custom extension objects.
        /// </remarks>
        /// <param name="serviceDescription">used mainly for inspection</param>
        /// <param name="serviceHostBase">the host that is currently being built</param>
        public virtual void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            if (StartupShell == null) throw new ConfigException("StartupShell can not be null.");
            serviceHostBase.Extensions.Add(new StartupExtension(StartupShell));
        }

        void IServiceBehavior.AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
        { // nothing needed here
        }

        void IServiceBehavior.Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        { // nothing needed here
        }

    }


    /// <summary>
    /// Adds the IocExtension to any ServiceHost allowing for the IocInstanceProvider to
    /// be used when creating instances of the services. Inherits StartupServiceBehavior.
    /// 
    /// * service classes can be decorated in code with this behavior as an attribute.
    /// </summary>
    /// <remarks>
    /// IServiceBehavior
    ///     Provides a mechanism to modify or insert custom extensions across an entire service, 
    ///     including the System.ServiceModel.ServiceHostBase. 
    ///     Important: has no bearing on the service contract, WCF behaviors only
    ///                affect the internals of the service implementation (class).
    /// * Attribute               
    ///     Represents the base class for custom attributes.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class IocServiceBehavior : StartupServiceBehavior, IServiceBehavior
    {
        #region Custom Attribute properties - set in the attribute decorator

        /// <summary>
        /// Indication of whether to use the IocServiceBehaviorExtension (IocInstanceProvider)
        /// Note: true by default, but inheritors could set it to false as an override
        /// </summary>        
        public virtual bool UseIocDI
        {
            get { return _useIocDI; }
            set { _useIocDI = value; }
        }
        private bool _useIocDI = true; // default

        #endregion


        /// <remarks>
        /// Main extension point for custom service behaviours.
        ///     Provides the ability to change run-time property values or insert custom extension objects 
        ///     such as error handlers, message or parameter interceptors, security extensions, and other 
        ///     custom extension objects.
        /// </remarks>
        /// <param name="serviceDescription">used mainly for inspection</param>
        /// <param name="serviceHostBase">the host that is currently being built</param>
        public override void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            //Note: though we expect that any service using the Ioc behavior will need to apply some initial configuration we are
            //      allowing the consumer the choice of whether or not to perform that initiailization using the startup behavior
            if (StartupShell != null) serviceHostBase.Extensions.Add(new StartupExtension(StartupShell));
            
            if (UseIocDI) serviceHostBase.Extensions.Add(new IocExtension(serviceDescription));
        }

        void IServiceBehavior.AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
        { // nothing needed here
        }

        void IServiceBehavior.Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        { // nothing needed here
        }

    }
}