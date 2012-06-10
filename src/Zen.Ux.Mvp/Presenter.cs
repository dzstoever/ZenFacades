using Zen.Ux.Mvp.Model;
using Zen.Ux.Mvp.View;

namespace Zen.Ux.Mvp
{
    /// <summary>
    /// Base class for all presenter classes. Keeps track of Model and View classes.
    /// Notice that Model is static and View is set in the constructor.
    /// </summary>
    /// <remarks>
    /// MV Patterns: MVP design pattern.
    /// </remarks>
    /// <typeparam name="T">Type of view.</typeparam>
    public class Presenter<T> where T : IView
    {
        /// <summary>
        /// Gets and sets the model statically.
        /// </summary>
        protected static IModel Model { get; private set; }

        /// <summary>
        /// Static constructor
        /// </summary>
        static Presenter()
        {
            Model = new Model.Model();
        }

        /// <summary>
        /// Constructor. Sets the view.
        /// </summary>
        /// <param name="view">The view.</param>
        public Presenter(T view)
        {
            View = view;
        }

        /// <summary>
        /// Gets and sets the view.
        /// </summary>
        protected T View { get; private set; }
    }
}
