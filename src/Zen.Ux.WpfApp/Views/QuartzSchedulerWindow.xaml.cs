using System.Windows;
using System.Windows.Input;
using Zen.Log;
using Zen.Ux.Mvvm;
using Zen.Ux.Mvvm.ViewModel;

namespace Zen.Ux.WpfApp.Views
{
    /// <summary>
    /// Scheduler details + add-edit window.
    /// </summary>
    public partial class QuartzSchedulerWindow : IQuartzSchedulerView
    {
        const string WindowTitle = "Quartz Scheduler";

        private readonly ILogger _log = Aspects.GetLogger();
        private QuartzVM _viewModel;// { get { return this.DataContext as QuartzVM; } }

        private string _originalName;
        private string _originalCluster;
        private string _originalSchedType;

        
        public QuartzSchedulerWindow(QuartzVM viewModel)
        {
            InitializeComponent();            
            Title = WindowTitle;
            _viewModel = viewModel;
        }

        
        /// <summary>
        /// Flag indicating new scheduler or not.
        /// </summary>
        public bool IsNewScheduler { get; set; }

        /// <summary>
        /// Loads new or existing record.
        /// </summary>
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            BaseUICmd cmd;

            // New scheduler.
            if (IsNewScheduler)
            {

                DataContext = _viewModel.NewSchedulerBmo;
                Title = "Add new scheduler";

                cmd = _viewModel.AddCmd;

                // Display little hint message
                LabelNewMessage1.Visibility = Visibility.Visible;
                LabelNewMessage2.Visibility = Visibility.Visible;
            }
            else
            {
                DataContext = _viewModel.CurrentScheduler;

                // Save off original values. Due to binding viewmodel is changed immediately when editing.
                // So, when canceling we have these values to restore original state.
                // Suggestion: could be implemented as Memento pattern.
                _originalName = _viewModel.CurrentScheduler.Name;
                _originalCluster = _viewModel.CurrentScheduler.Cluster;
                _originalSchedType = _viewModel.CurrentScheduler.SchedType;

                Title = "Edit scheduler";

                cmd = _viewModel.EditCmd;
            }

            textBoxScheduler.Focus();

            // The command helps determine whether save button is enabled or not
            buttonSave.Command = cmd.Command;
            buttonSave.CommandParameter = DataContext;
            buttonSave.CommandBindings.Add(new CommandBinding(cmd.Command, cmd.OnExecute, cmd.OnCanExecute));
        }

       
        private void ButtonSaveClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        
        private void ButtonCancelClick(object sender, RoutedEventArgs e)
        {
            // Restore viewmodel to original values
            if (!IsNewScheduler)
            {
                _viewModel.CurrentScheduler.Name = _originalName;
                _viewModel.CurrentScheduler.Cluster = _originalCluster;
                _viewModel.CurrentScheduler.SchedType = _originalSchedType;
            }
        }

        /*
        /// <summary>Opens scheduler dialog.
        /// </summary>
        private void AddCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var window = new QuartzSchedulerWindow();
            window.Owner = this;
            window.IsNewScheduler = true;

            if (window.ShowDialog() == true)
            {
                //FacadeListBox.ScrollIntoView(ViewModel.CurrentScheduler);
                CommandManager.InvalidateRequerySuggested();
            }
        }
        private void AddCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ViewModel.CanAdd;
        }

        /// <summary>Opens scheduler dialog.
        /// </summary>
        private void EditCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var window = new QuartzSchedulerWindow();
            window.Owner = this;

            if (window.ShowDialog() == true)
            {
                CommandManager.InvalidateRequerySuggested();
            }
        }
        private void EditCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ViewModel.CanEdit;
        }

        /// <summary>Executes delete-scheduler command.
        /// </summary>
        private void DeleteCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            ViewModel.DeleteCommand.OnExecute(this, e);

            if (!e.Handled)
            {
                var name = ViewModel.CurrentScheduler != null ? ViewModel.CurrentScheduler.Name : "scheduler";
                MessageBox.Show("Cannot delete " + name + " because they have existing jobs.", "Delete Scheduler");
            }

            CommandManager.InvalidateRequerySuggested();
        }
        private void DeleteCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ViewModel.CanDelete;
        }
         */ 

    }
}
