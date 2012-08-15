namespace Zen.Ux
{
    public interface IViewFactory
    {
        T CreateView<T>() where T : IView;
        
        T CreateView<T>(object argumentsAsAnonymousType) where T : IView;
    }
}