using System.Windows;
using System.Windows.Input;
using Zen.Log;
using Zen.Ux.Mvvm.ViewModel;

namespace Zen.Ux.WpfApp.Views
{
    public partial class AppFacadeWindow : IMainView
    {
        const string WindowTitle = "Zen Application";
        const string WelcomeMessage = "Service Dashboard";

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
            var window = new SignonScreen(_viewModel.Provider) { Owner = this };
            var result = window.ShowDialog();
            
            if (result == true)
            {
                LoginLabel.Content = " Logged In";
                Announcement.Visibility = Visibility.Collapsed;

                Cursor = Cursors.Wait;
                    _viewModel.LoadFacades();                
                Cursor = Cursors.Arrow;

                foreach (var facade in _viewModel.Facades)
                {
                    this.ViewMenu.DropDown.Items.Add(facade.MenuName);
                }

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

            //for (var i = 2; i < ViewMenu.DropDown.Items.Count; i++)
            //{
                ViewMenu.DropDown.Items.Clear();
            //}

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
