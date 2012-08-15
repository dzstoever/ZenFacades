using Zen.Ux.WinApp.Mvp.Views;

namespace Zen.Ux.WinApp.Mvp.Presenters
{
    /// <summary>Base class for all presenter classes. Keeps track of Model and View classes.
    /// Notice that Model is static and View is set in the constructor.
    /// </summary>
    /// <typeparam name="T">Type of view.</typeparam>
    public class Presenter<T> where T : IBaseView
    {
        /// <summary>Gets and sets the model statically.
        /// </summary>
        protected static IModel Model { get; private set; }

        static Presenter()
        {
            Model = new Model();
        }


        /// <summary>Sets the view.
        /// </summary>
        public Presenter(T view)
        {
            View = view;
        }

        protected T View { get; private set; }
    }
}