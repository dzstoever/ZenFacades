using Castle.Windsor;

//http://visualstudiomagazine.com/articles/2011/10/01/wpf-and-inversion-of-control.aspx
namespace Zen.Ux.Bootstrap.Windsor
{
    /// <summary>
    /// Allow Windsor to create our views so that they are get the benefits of Ioc
    /// </summary>
    public class ViewFactory : IViewFactory
    {
        private readonly IWindsorContainer _container;

        public ViewFactory(IWindsorContainer container)
        {
            _container = container;
        }

        ~ViewFactory()
        {
            if (_container != null) _container.Dispose();
        }
       
        public T CreateView<T>() where T : IView
        {
            return _container.Resolve<T>();
            /* I was disposing a Castle Windsor dependency that was configured with a singleton life cycle.    
             * Apparently, Castle will, by default, track dependencies with decommission concerns.
             * Here is a good article on it: http://kozmic.pl/archive/2010/08/19/must-windsor-track-my-components.aspx?utm_source=feedburner&utm_medium=feed&utm_campaign=Feed%3A+kozmic+%28Krzysztof+Ko%3Fmic%27s+blog%29
             */
        }

        public T CreateView<T>(object argumentsAsAnonymousType) where T : IView
        {
            return _container.Resolve<T>(argumentsAsAnonymousType);
        }
    }
}