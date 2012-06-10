using System;
using System.Threading;
using System.Windows;
using Zen.Ux.Bootstrap;
using Zen.Log;
using Zen.Ux.Mvvm;

namespace Zen.Ux.WpfApp
{
    public partial class StartupScreen : Window
    {        
        public StartupScreen()
        {
            InitializeComponent();

            Title = "Starting... Zen 4.0";
            labelProductName.Content = AboutScreen.AssemblyProduct;
            labelVersion.Content = "Version " + AboutScreen.AssemblyVersion;
            labelCopyright.Content = AboutScreen.AssemblyCopyright;
            labelCompanyName.Content = AboutScreen.AssemblyCompany;

            // After the startup screen is shown to the user we will kick off some threads to perform
            // any required startup actions. This way there is no perceived delay to the user.
            // http://www.codeproject.com/Articles/31971/Understanding-SynchronizationContext-Part-I
            Loaded += (sender, e) => 
            {
                _startTime = DateTime.Now;
                _startupTimer = new Timer(BackgroundTick, SynchronizationContext.Current, 0, 1000);
                var thread = new Thread(BackgroundStartup);
                thread.Start(SynchronizationContext.Current);
            };
            
        }


        /// <summary>
        /// Use the bootstrapper to:
        ///  1) register all our components with Ioc, 
        ///  2) add our mappings(Entity-Dto-Bmo) 
        ///  3) and perform any startup tasks
        /// </summary>
        /// <remarks>
        /// different bootstrappers can be plugged to change configurations and dependency implementations 
        /// for testing or at run-time to alter behaviors like logging, navigation, data-acesss, etc.
        /// </remarks>
        private static void BootstrapStartup()
        {
            var shell = new WpfShell();
            shell.Startup();                         
        }

        /// <summary>
        /// Hide the startup screen and use the view factory to show the main view 
        /// Note: CreateView must be called on UI thread 
        /// </summary>        
        private void BootstrapStartupComplete()
        {
            AppController.AttachNavigator(new AppNavigator(WpfShell.ViewFactory));
            var mainView = WpfShell.ViewFactory.CreateView<IMainView>();                        
            Application.Current.MainWindow = (Window)mainView;
            mainView.Show();
            Close(); 
        }


        #region background thread synchronization context

        private DateTime _startTime;
        private Timer _startupTimer;
        private Timer _shutdownTimer;

        private void BackgroundStartup(object state)
        {// This method is executed on it's own thread. 
                         
            var uiContext = state as SynchronizationContext;// grab the context from the state 
            Exception exState = null;
            try
            {
                //Thread.Sleep(5000);//<-- fake startup
                //throw new Exception("Fake error.");

                BootstrapStartup();
            }
            catch (Exception ex)
            {
                exState = ex;
                // Note: startup errors may not get logged depending on the bootstapper.
                Aspects.GetLogger().Fatal("Startup failed.", ex);                                                                       
            }
            finally
            {
                "Program startup took [{0}]".FormatWith(_startTime.GetElapsedTime(true)).LogMe(LogLevel.Info);
                if (uiContext != null)
                    uiContext.Send(UIAfterStartup, exState);// notify the UI synchronously                
            }
        }
        
        private void UIAfterStartup(object state)
        {// This method is executed on the main UI thread.            

            var exState = state as Exception;// indication of startup failure
            if (exState != null)
            {
                labelProductName.Content = "Startup failure!";
                labelCompanyName.Content = "Shutting down...";

                _startTime = DateTime.Now;// restart the time frame for the shutdown timer
                _shutdownTimer = new Timer(BackgroundShutdown, SynchronizationContext.Current, 11000, Timeout.Infinite);
                return;
            }
            labelTimer.Visibility = Visibility.Hidden;
            _startupTimer.Dispose();

            BootstrapStartupComplete();            
        }


        private void BackgroundShutdown(object state)
        {// This method is executed on it's own thread.                          
            
            var uiContext = state as SynchronizationContext;
            if (uiContext != null)
                uiContext.Send(UIShutdown, null);
        }

        private void UIShutdown(object state)
        {// This method is executed on the main UI thread.
            
            if (_shutdownTimer != null) _shutdownTimer.Dispose();
            Application.Current.Shutdown(-1);
        }


        private void BackgroundTick(object state)
        {// This method is executed on it's own thread.
            
            var uiContext = state as SynchronizationContext;
            if (uiContext != null)
                uiContext.Send(UITick, null);
        }

        private void UITick(object state)
        {// This method is executed on the main UI thread.            
            
            labelTimer.Content = _startTime.GetElapsedTime(false);
        }

        #endregion
    }
}
