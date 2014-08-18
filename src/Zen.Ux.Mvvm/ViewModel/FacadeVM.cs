using System.Collections.ObjectModel;
using Zen.Ux.Mvvm.Model;

namespace Zen.Ux.Mvvm.ViewModel
{
    //Translates data provided by the Model into a 
    //convenient structure for the View
    public class FacadeVM : BaseVM, IViewModel
    {
        public FacadeVM(IProvider provider)
        {
            Provider = provider;            
        }

        public IProvider Provider { get; private set; }

        public bool IsLoaded { private set; get; }

        public ObservableCollection<FacadeBmo> Facades { get; private set; }

        public FacadeBmo CurrentFacade
        {
            get { return _currentFacade; }
            set
            {
                if (_currentFacade != value)
                {
                    _currentFacade = value;
                    OnPropertyChanged("CurrentFacade");
                }
            }
        }
        private FacadeBmo _currentFacade;

        
        public void LoadFacades()
        {
            Facades = Provider.GetFacades();

            //foreach (var facade in Facades)
            

            if (Facades.Count > 0)
                CurrentFacade = Facades[0];

            IsLoaded = true;
        }

        public void UnloadFacades()
        {
            // clear entitys from display.
            Facades.Clear();

            CurrentFacade = null;
            IsLoaded = false;
        }
    }
}
