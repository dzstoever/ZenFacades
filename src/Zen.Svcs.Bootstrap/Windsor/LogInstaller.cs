using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Zen.Log;

namespace Zen.Svcs.Bootstrap.Windsor
{
    /// <summary>
    /// Register logging components with IoC        
    /// </summary>
    /// <remarks>
    /// Loggers can always be obtained from the Aspects, but this adds 
    /// the ability to get a Logger/LoggerFactory from the IoC/DI container 
    /// and any other registered components with a Logger/LoggerFactory 
    /// dependency will have it injected automatically 
    /// </remarks>  
    public class LogInstaller : IWindsorInstaller
    {
              
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<ILoggerFactory>()
                                   .ImplementedBy<Log4netLoggerFactory>()
                                   .LifestyleSingleton());

            container.Register(Component.For<ILogger>()
                                   .ImplementedBy<Log4netLogger>()
                                   .LifestyleSingleton());
            
        }

    }
}