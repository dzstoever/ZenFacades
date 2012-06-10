using System;
using System.Reflection;
using System.Windows;
using System.Windows.Input;

namespace Zen.Ux.Mvvm
{
    /// <summary>
    /// Provides one possible implementation of a navigation workflow 
    /// </summary>
    public class AppNavigator : INavigationController
    {
        //helpers
        private static void ActivateView(IView view)
        {
            var window = (Window)view;
            if (!window.IsVisible) window.Show();
            window.Activate();
        }
        private static void HideView(IView view)
        {
            var window = (Window)view;
            if (window.IsVisible) window.Hide();            
        }


        public AppNavigator(IViewFactory viewFactory)
        {
            _viewFactory = viewFactory;
        }

        private readonly IViewFactory _viewFactory;

        public ICommand Launch
        {
            get{ return _launchCommand ?? (_launchCommand = new DelegateCmd(ShowQuartz)); }
            set { _launchCommand = value; }
        }
        private ICommand _launchCommand;

        private void ShowQuartz()
        {
            var view = _viewFactory.CreateView<IQuartzView>();
            ActivateView(view);
        }


        public void NavigateTo(string urn)
        {
            switch (urn.ToLower())
            {
                //case "help":
                //case "about":
                //case "login":


                //case "main":
                //    HideView((IView)AppLoginView);
                //    HideView((IView)QuartzFacadeView);
                //    HideView((IView)QuartzSchedulerView);
                //    break;

                case "quartz":
                    ShowQuartz();
                    break;

                //case "quartz.scheduler":
                //    if (QuartzSchedulerView == null) QuartzSchedulerView = DI.Resolve<QuartzSchedulerWindow>();
                //    ActivateView((IView)QuartzSchedulerView);
                //    break;

                default:
                    throw new ApplicationException("Unrecognized urn.");
            }
        }

        public void NavigateTo(string urn, object[] args)
        {
            switch (urn.ToLower())
            {
                default:
                    NavigateTo(urn);//try with no args
                    break;
            }
        }

        public bool ConfirmQuit()
        {
            return MessageBox.Show("Are you sure you want to exit?", Assembly.GetEntryAssembly().GetName().Name,
                   MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
        }

        public void Quit()
        {
            Application.Current.Shutdown(0);
            //Environment.Exit(0);
        }


        
    }

}