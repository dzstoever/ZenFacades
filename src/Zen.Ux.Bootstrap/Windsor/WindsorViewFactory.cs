using System;
using Castle.Windsor;
using Zen.Ux;

//http://visualstudiomagazine.com/articles/2011/10/01/wpf-and-inversion-of-control.aspx
namespace Zen.BS
{
    
    public class WindsorViewFactory : IViewFactory
    {
        private readonly IWindsorContainer _container;


        public WindsorViewFactory(IWindsorContainer container)
        {
            _container = container;
        }
        
        ~WindsorViewFactory()
        {
            if (_container != null) _container.Dispose();
        }


        public T CreateView<T>() where T : IView
        {
            return _container.Resolve<T>();
        }

        public T CreateView<T>(object argumentsAsAnonymousType) where T : IView
        {
            return _container.Resolve<T>(argumentsAsAnonymousType);
        }
    }
}