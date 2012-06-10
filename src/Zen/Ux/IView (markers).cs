namespace Zen.Ux
{
    // marker interfaces for views

    public interface IView
    {
        void Close();
        void Show();
        bool? ShowDialog();
    }

    public interface IMainView : IView
    {
    }

    public interface IQuartzView : IView
    {
    }

    public interface IQuartzSchedulerView : IView
    {
    }

}