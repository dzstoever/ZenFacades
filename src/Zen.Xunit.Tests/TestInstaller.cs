using Castle.Facilities.WcfIntegration;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Zen.Data;
using Zen.Log;

namespace Zen.Xunit.Tests
{
    public class TestInstaller   : IWindsorInstaller
    {

        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.AddFacility<WcfFacility>()
                .Register(
                    Component.For<ILogger>().ImplementedBy<Log4netLogger>(),                    
                    Component.For<IFakeSvc>().ImplementedBy<FakeSvc>()
                    //Component.For<ServiceHostFactory>().ImplementedBy<DefaultServiceHostFactory>(),
                );

        }
    }
}