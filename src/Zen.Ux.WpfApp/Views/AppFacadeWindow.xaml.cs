using System.Windows;
using System.Windows.Input;
using Zen.Log;
using Zen.Ux.Mvvm.ViewModel;

namespace Zen.Ux.WpfApp.Views
{
    public partial class AppFacadeWindow : IMainView
    {
        const string WindowTitle = "Zen WPF Application";
        const string WelcomeMessage = "Façade Dashboard";

        private readonly ILogger _log = Aspects.GetLogger();
        private FacadeVM _viewModel;// { get { return this.DataContext as FacadeVM; } }

        public AppFacadeWindow(FacadeVM viewModel)
        {
            InitializeComponent();            
            Title = WindowTitle;
            Banner.Text = WelcomeMessage;
            _viewModel = viewModel;

            // we inverted this and let IoC set the DataContext...via the ViewActivator
            //Loaded += (sender, e) => { DataContext = viewModel; }; // bind view to the proper context            
        }


        private void LoginCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true; //!ViewModel.IsLoaded;
        }
        
        private void LoginCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var window = new SignonScreen(null) { Owner = this };

            if (window.ShowDialog() == true)
            {
                LoginLabel.Content = " Logged In";
                Announcement.Visibility = Visibility.Collapsed;

                Cursor = Cursors.Wait;
                    _viewModel.LoadFacades();                
                Cursor = Cursors.Arrow;

                CommandManager.InvalidateRequerySuggested();
            }
        }
        
        private void LogoutCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;// ViewModel.IsLoaded;
        }
        
        private void LogoutCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            //ViewModel.UnloadSchedulers();
            LoginLabel.Content = "Logged Out";
            Announcement.Visibility = Visibility.Visible;

            CommandManager.InvalidateRequerySuggested();
        }
        
        private void ExitCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void HowDoICommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            MessageBox.Show("God helps those that help themselves.", "How Do I");
        }

        private void IndexCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            MessageBox.Show("Not available.", "Help Index");
        }

        private void AboutCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var window = new AboutScreen {Owner = this};
            window.ShowDialog();
        }

    }

   
}
