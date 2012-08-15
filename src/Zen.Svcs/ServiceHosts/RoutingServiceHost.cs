using System;
using System.ServiceModel;
using System.ServiceModel.Routing;

namespace Zen.Svcs.ServiceModel.Hosts
{
    /// <summary>
    /// Custom derivative of ServiceHost that automatically
    /// enables metadata generation for any service it hosts. 
    /// </summary>
    class RoutingServiceHost : ServiceHost // i.e. RouterServiceHost
    {
        public RoutingServiceHost(Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses) { }

        //alter the ServiceDescription prior to opening the service host. 
        protected override void ApplyConfiguration()
        {
            // First, we call base.ApplyConfiguration() to read any configuration
            // that was provided for the service we're hosting so when we read
            // .ServiceDescription, it describes the service as it was configured.
            base.ApplyConfiguration();

            // add the routing behavior and configuration 
            Description.Behaviors.Add(new RoutingBehavior(new RoutingConfiguration()));

            // add the discovery behavior
            Description.Behaviors.Add(new DiscoveryRoutingBehavior());
        }
    }
}