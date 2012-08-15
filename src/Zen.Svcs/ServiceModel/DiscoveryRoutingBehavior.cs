using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;

namespace Zen.Svcs.ServiceModel
{
    /// <summary>
    /// Add the discovery extension to the router.
    /// </summary>
    /// <remarks>
    /// BehaviorExtensionElement
    ///     Represents a configuration element that contains sub-elements that specify
    ///     behavior extensions, which enable the user to customize service or endpoint
    ///     behaviors. 
    /// IServiceBehavior
    ///    Provides a mechanism to modify or insert custom extensions across an entire
    ///    service, including the System.ServiceModel.ServiceHostBase.
    /// </remarks>
    public class DiscoveryRoutingBehavior : BehaviorExtensionElement, IServiceBehavior
    {
        public override Type BehaviorType
        {
            get { return typeof(DiscoveryRoutingBehavior); }
        }

        protected override object CreateBehavior()
        {
            return new DiscoveryRoutingBehavior();
        }


        void IServiceBehavior.ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            serviceHostBase.Extensions.Add(new DiscoveryRoutingExtension());
        }

        void IServiceBehavior.AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, System.Collections.ObjectModel.Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
        { // nothing needed here
        }

        void IServiceBehavior.Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        { // nothing needed here
        }
        
    }
}