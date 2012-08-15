using System;
using System.Windows.Input;

namespace Zen.Ux.Mvvm
{
    /// <summary>
    /// Exposes an action command to which a button can bind, etc. 
    /// </summary>
    /// <example>
    /// public ICommand Launch
    /// {
    ///   get { return launchCommand ?? (launchCommand = new DelegateCommand(ShowSecondWindow)); }
    ///   set { launchCommand = value; }
    /// }
    /// </example>
    public class DelegateCmd : ICommand
    {
        private readonly Action _executeMethod;
        private readonly Func<bool> _canExecuteMethod;

        public DelegateCmd(Action executeMethod)
            : this(executeMethod, () => true)
        {
        }

        public DelegateCmd(Action executeMethod, Func<bool> canExecuteMethod)
        {
            if (executeMethod == null)
                throw new ArgumentNullException("executeMethod");
            if (canExecuteMethod == null)
                throw new ArgumentNullException("canExecuteMethod");

            _executeMethod = executeMethod;
            _canExecuteMethod = canExecuteMethod;
        }

        #region ICommand Members

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object stupid)
        {
            return CanExecute();
        }

        public bool CanExecute()
        {
            return _canExecuteMethod();
        }

        public void Execute(object parameter)
        {
            Execute();
        }

        public void Execute()
        {
            _executeMethod();
        }

        public void OnCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }

        #endregion ICommand Members
    }
}