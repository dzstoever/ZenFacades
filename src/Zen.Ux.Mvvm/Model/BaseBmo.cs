using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Threading;

namespace Zen.Ux.Mvvm.Model
{
    /// <summary>
    /// Abstract base class for business model objects (BMOs). 
    /// </summary>
    /// <remarks>
    /// Methods ensure that they are called on the UI thread only.
    /// </remarks>
    public abstract class BaseBmo : INotifyPropertyChanged
    {
        protected BaseBmo()
        {
            // Save off dispatcher 
            Dispatcher = Dispatcher.CurrentDispatcher;
        }

        protected Dispatcher Dispatcher;// Dispatcher associated with model

        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                ConfirmOnUIThread();
                _propertyChangedEvent += value;
            }
            remove
            {
                ConfirmOnUIThread();
                if (_propertyChangedEvent != null) _propertyChangedEvent -= value;
            }
        }
        private PropertyChangedEventHandler _propertyChangedEvent;

        /// <summary>Use by subclasses to notify that a property value has changed.
        /// </summary>
        protected void Notify(string propertyName)
        {
            ConfirmOnUIThread();
            ConfirmPropertyName(propertyName);

            if (_propertyChangedEvent != null)
            {
                _propertyChangedEvent(this, new PropertyChangedEventArgs(propertyName));
            }
        }



        /// <summary>
        /// Debugging facility that ensures methods are called on the UI thread.
        /// </summary>
        [Conditional("Debug")]
        protected void ConfirmOnUIThread()
        {
            Debug.Assert(Dispatcher.CurrentDispatcher == Dispatcher, "Call must be made on UI thread.");
        }

        /// <summary>
        /// Debugging facility that ensures the property does exist on the class.
        /// </summary>
        /// <param name="propertyName"></param>
        [Conditional("Debug")]
        private void ConfirmPropertyName(string propertyName)
        {
            Debug.Assert(GetType().GetProperty(propertyName) != null, "Property " + propertyName + " is not a valid name.");
        }
    }
}


