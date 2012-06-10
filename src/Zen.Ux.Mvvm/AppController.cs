using System;

namespace Zen.Ux.Mvvm
{
    /// <summary>
    /// Central console that holds all the logic for determining the next view in the application.
    /// This class just attaches the actual implementation of INavigationController.
    /// </summary>
    /// <remarks>
    /// Note: different implementations can be 'plugged-in'.
    /// </remarks>
    public static class AppController
    {        
        private static INavigationController _navigator;

        public static void AttachNavigator(INavigationController navigator)
        {
            if (navigator == null) 
                throw new ArgumentNullException("navigator", "You must provide an INavigationController implementation.");
            _navigator = navigator;
        }

        public static void NavigateTo(string urn)
        {
            if (_navigator == null) 
                throw new InvalidOperationException("Navigation service not properly initialized.");
            _navigator.NavigateTo(urn);
        }

        public static void NavigateTo(string urn, object[] args)
        {
            if (_navigator == null) 
                throw new InvalidOperationException("Navigation service not properly initialized.");
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