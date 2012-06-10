using System.ComponentModel;
using System.Windows.Input;

namespace Zen.Ux.Mvvm.ViewModel
{
    public abstract class BaseVM : IViewModel, INotifyPropertyChanged 
    {

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;

            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        
    }
}
