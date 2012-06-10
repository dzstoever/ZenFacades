using Zen.Ux.Mvp.View;

//using WindowsFormsModel;

namespace Zen.Ux.Mvp
{
    /// <summary>
    /// Logout Presenter class.
    /// </summary>
    /// <remarks>
    /// MV Patterns: MVP design pattern.
    /// </remarks>
    public class LogoutPresenter : Presenter<IView>
    {
        /// <summary>
        /// Constructor. View is not really used here.
        /// </summary>
        /// <param name="view">The view.</param>
        public LogoutPresenter(IView view)
            : base(view)
        {
        }

        /// <summary>
        /// Informs model to logout.
        /// </summary>
        public void Logout()
        {
            Model.Logout();
        }
    }
}
