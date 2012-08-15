using System;

namespace Zen.Ux.WinApp.Mvp
{
    public interface INavigator
    {       
        void NavigateTo(string urn);

        void NavigateTo(string urn, object[] args);

        /// <summary>Warn the user before exiting the application
        /// </summary>
        /// <returns>indication of whether they really want to quit</returns>
        bool ConfirmQuit();

        /// <summary>Quit the application
        /// </summary>
        void Quit();
    }

    /// <summary>Attaches(sets) the actual implementation and allows
    /// us to manage navigation staticly from the main presenter
    /// </summary>
    public static class Navigation
    {
        private static INavigator _navigator;

        public static void AttachNavigator(INavigator instance)
        {
            if (instance == null) throw new ArgumentNullException("instance", "Parameter can't be null.");
            _navigator = instance;
        }

        public static void NavigateTo(string urn)
        {
            if (_navigator == null) throw new InvalidOperationException("Navigation service not properly initialized.");
            _navigator.NavigateTo(urn);
        }

        public static void NavigateTo(string urn, object[] args)
        {
            if (_navigator == null) throw new InvalidOperationException("Navigation service not properly initialized.");
            _navigator.NavigateTo(urn, args);
        }

        public static bool ConfirmQuit()
        {
            return _navigator.ConfirmQuit();
        }

        public static void Quit()
        {
            _navigator.Quit();
        }

    }
}