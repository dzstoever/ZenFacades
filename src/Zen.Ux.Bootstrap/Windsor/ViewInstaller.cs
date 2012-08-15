using System.Reflection;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Zen.Ux.Mvvm.ViewModel;

namespace Zen.Ux.Bootstrap.Windsor
{
    public class ViewInstaller : IWindsorInstaller
    {
        // register the navigation components (Views and ViewModels)
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            
            container.AddFacility<ViewActivatorFacility>();       // add this facility to Windsor before registering any IView components. 
            container.Register(
                Component.For<IWindsorContainer>().Instance(container),  // register the container with itself - for the IViewFactory

                /* The WindsorViewFactory will be registered in Windsor, as any ViewModel that needs to launch another View 
                 * (or the INavigationController in this case) will have a dependency on IViewFactory. */
                Component.For<IViewFactory>().ImplementedBy<ViewFactory>(),

                /* Register all IView-based and all IViewModel-based types instead of doing them one-by-one. 
                 * This means we don't have to revisit the component registration code every time to add a 
                 * new WPF Window or ViewModel to the application. */
                AllTypes.FromAssembly(Assembly.GetEntryAssembly())          // windows are in the .WpfApp assembly
                        .BasedOn<IView>().WithService.FromInterface()
                        .LifestyleTransient(),

                AllTypes.FromAssemblyContaining<FacadeVM>()                 // view models are in .Mvvm this assembly
                        .BasedOn<IViewModel>()
                        .LifestyleTransient()
            );
        }
    }
}