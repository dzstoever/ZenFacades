using System;
using System.Windows.Input;

namespace Zen.Ux.Mvvm
{
    /// <summary>Abstract class that encapsulates a System.Windows.Input.RoutedUICommand.
    /// </summary>
    public abstract class BaseUICmd 
    {
        protected BaseUICmd()
        {
            Command = new RoutedUICommand();
        }

        public RoutedUICommand Command{ private set; get; }

        
        public abstract void OnExecute(object sender, ExecutedRoutedEventArgs e);

        /// <summary>
        /// Determines if a command is enabled. Override to provide custom behavior. 
        /// Do not call the base version when overriding.
        /// </summary>
        public virtual void OnCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
            e.Handled = true;
        }
    }

}
