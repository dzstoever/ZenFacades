using System.Linq;
using System.Windows;
using Castle.Core;
using Castle.MicroKernel;
using Castle.MicroKernel.ComponentActivator;
using Castle.MicroKernel.Facilities;

namespace Zen.Ux.Bootstrap.Windsor
{
    /// <summary>
    /// Automate the assigning of the WpfWindowActivator for any component that implements IView.
    /// The facility must be registered before any of the IView Componenents.
    /// </summary>
    public class ViewActivatorFacility : AbstractFacility
    {
        protected override void Init()
        {
            Kernel.ComponentModelCreated += Kernel_ComponentModelCreated;
        }

        // fired as soon as a component model (based on the registration of the component) has been created
        void Kernel_ComponentModelCreated(Castle.Core.ComponentModel model)
        {
            bool isView = false;
            foreach (var service in model.Services)
                isView = typeof(IView).IsAssignableFrom(service);            
            if (!isView) return;

            if (model.CustomComponentActivator == null)// make sure a custom activator hasn't already been assigned
                model.CustomComponentActivator = typeof(ViewActivator);
        }
    }

    
    /// <summary>
    /// This custom activator along with the facility makes WPF development with IoC a breeze:
    /// * ViewModels are fully integrated with Windsor, so they get all those benefits -- dependency injection, custom lifestyles and so on.
    /// * Views are fully integrated with Windsor, along with automatic assignment of the ViewModel to the DataContext.
    /// * The marker interfaces of IView and IViewModel make registration of all components in the container much easier and virtually maintenance-free.
    /// * ViewModel is fully decoupled from the View and can be tested in isolation. It also encourages reuse of my ViewModel, as it has no WPF references. 
    /// </summary>
    public class ViewActivator : DefaultComponentActivator
    {
        public ViewActivator(ComponentModel model, IKernel kernel, ComponentInstanceDelegate onCreation, ComponentInstanceDelegate onDestruction)
            : base(model, kernel, onCreation, onDestruction)
        {
        }

        protected override object CreateInstance(Castle.MicroKernel.Context.CreationContext context, ConstructorCandidate constructor, object[] arguments)
        {
            var component = base.CreateInstance(context, constructor, arguments);
            AssignDataContext(component, arguments);
            return component;
        }

        /// <summary>
        /// Find the first ctor argument that implements IViewModel and assign it to the component's DataContext property.
        /// </summary>
        /// <param name="component">The activated WPF element.</param>
        /// <param name="arguments">The constructor arguments</param>
        private void AssignDataContext(object component, object[] arguments)
        {
            var frameworkElement = component as FrameworkElement;// make sure we have a bindable WPF object (FrameworkElement).
            if (frameworkElement == null || arguments == null) return;
            
            var vm = arguments.FirstOrDefault(a => a is IViewModel);// get the first argument that implements IViewModel
            if (vm == null) return;
                            
            frameworkElement.DataContext = vm;
            //AssignParentView(frameworkElement, vm);            
        }

        // * not used - the ViewModel should not know about the View...
        // assigns the ParentView property of the IViewModel to the first public property it finds that implements IView,
        // so if the view in question has a public .ParentView property it gets assigned here?
        //private void AssignParentView(FrameworkElement frameworkElement, object dataContext)
        //{
        //    var view = frameworkElement as IView;
        //    if (view == null) return;
            
        //    var viewProp = dataContext.GetType()
        //        .GetProperties(BindingFlags.Public | BindingFlags.Instance)
        //        .FirstOrDefault(p => p.CanWrite && typeof(IView).IsAssignableFrom(p.PropertyType));
        //    if (viewProp == null) return;
                
        //    viewProp.SetValue(dataContext, frameworkElement, null);            
        //}

    }
}