using System;
using System.ServiceModel;
using System.ServiceModel.Activation;

namespace Zen.Svcs.ServiceModel.Hosts
{
    public class ZenHostFactory : ServiceHostFactory
    {

        public ZenHostFactory()
        {
            var di = Aspects.GetIocDI();

        }

        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            
            return new ServiceHost(serviceType, baseAddresses);
        }

        //var hostFactoryImpl = di.Resolve<ServiceHostFactory>();

    }
}