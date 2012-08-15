using System.Linq;
using System.Reflection;
using System.Windows;
using Castle.Core;
using Castle.MicroKernel;
using Castle.MicroKernel.ComponentActivator;
using Zen.Ux;

namespace Zen.BS
{
    public class WpfWindowActivator : DefaultComponentActivator
    {
        public WpfWindowActivator(ComponentModel model, IKernel kernel, ComponentInstanceDelegate onCreation, ComponentInstanceDelegate onDestruction)
            : base(model, kernel, onCreation, onDestruction)
        {
        }

        protected override object CreateInstance(Castle.MicroKernel.Context.CreationContext context, ConstructorCandidate constructor, object[] arguments)
        {
            var component = base.CreateInstance(context, constructor, arguments);
            AssignViewModel(component, arguments);
            return component;
        }

        /// <summary>
        /// Find the first ctor argument that implements IViewModel and assign it to the component's DataContext property.
        /// </summary>
        /// <param name="component">The activated WPF element.</param>
        /// <param name="arguments">The constructor arguments</param>
        private void AssignViewModel(object component, object[] arguments)
        {
            var frameworkElement = component as FrameworkElement;
            if (frameworkElement == null || arguments == null) return;
            
            var vm = arguments.FirstOrDefault(a => a is IViewModel);
            if (vm == null) return;
                            
            frameworkElement.DataContext = vm;
            AssignParentView(frameworkElement, vm);            
        }

        private void AssignParentView(FrameworkElement frameworkElement, object dataContext)
        {
            var view = frameworkElement as IView;
            if (view == null) return;
            
            var viewProp = dataContext.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .FirstOrDefault(p => p.CanWrite && typeof(IView).IsAssignableFrom(p.PropertyType));
            if (viewProp == null) return;
                
            viewProp.SetValue(dataContext, frameworkElement, null);            
        }

    }
}