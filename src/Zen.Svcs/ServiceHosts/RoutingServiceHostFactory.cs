using System;
using System.ServiceModel;
using System.ServiceModel.Activation;

namespace Zen.Svcs.ServiceModel.Hosts
{
    /* Router.svc
    <%@ ServiceHost Language="C#" 
    Debug="true" 
    Factory="Zen.Svcs.RoutingServiceHostFactory"
    Service="System.ServiceModel.Routing.RoutingService,System.ServiceModel.Routing, version=4.0.0.0, Culture=neutral,PublicKeyToken=31bf3856ad364e35"
    %> 
    */
    
    /// <summary>
    /// return a new instance of our custom host
    /// </summary>
    public class RoutingServiceHostFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            return new RoutingServiceHost(serviceType, baseAddresses);
        }
    }

    
}