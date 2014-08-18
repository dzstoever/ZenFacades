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
            container.Register(Component.For<IProvider>().ImplementedBy<Provider>());
            //.LifeStyle.Singleton
            //.LifeStyle.PerWcfSession()

            var clientModel = new DefaultClientModel(WcfEndpoint.ForContract<IRemoteFacade>()
                                                                .BoundTo(new WSHttpBinding())
                                                                .At("http://localhost:1080/Zen/RemoteFacade"));
                                                                //.At("http://localhost:8080/Zen/RemoteFacadeSvc"));
        
            container.AddFacility<WcfFacility>().Register(WcfClient.ForChannels(clientModel).Configure(null));

            /* register the svcs
            container.AddFacility<WcfFacility>() //<-- let Castle create the client proxies
                .Register(
                Component.For<IProvider>().ImplementedBy<Provider>(),
                Component.For<IRemoteFacade>()//.ImplementedBy<RemoteFacadeSvc>()
                         .AsWcfClient(new DefaultClientModel(WcfEndpoint
                            .ForContract<IRemoteFacade>()
                            .BoundTo(new WSHttpBinding())
                            .At("http://127.0.0.1:1080/Zen"))));
             */
        }
    }
}