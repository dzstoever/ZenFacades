using System.ServiceModel;
using Castle.Facilities.WcfIntegration;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Zen.Svcs;
using Zen.Ux.Mvvm;

namespace Zen.Ux.Bootstrap.Windsor
{
    //Todo: get our host to use the WindsorServiceHostFactory
    //WindsorContainer container = new WindsorContainer("windsor.xml");
    //WindsorServiceHostFactory.RegisterContainer(container.Kernel);

    public class SvcInstaller : IWindsorInstaller
    {

        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            // register the svcs
            container.AddFacility<WcfFacility>() //<-- let Castle create the client proxies
                .Register(
                Component.For<IProvider>().ImplementedBy<Provider>(),
                Component.For<IRemoteFacade>()
                        .ImplementedBy<RemoteFacadeSvc>()
                        .AsWcfClient(new DefaultClientModel(WcfEndpoint
                            .ForContract<IRemoteFacade>()
                            .BoundTo(new WSHttpBinding())
                            .At("http://127.0.0.1:1080/Zen/AppFacade"))));
        }
    }
}