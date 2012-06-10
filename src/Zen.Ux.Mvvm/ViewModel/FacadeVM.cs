using System.Collections.ObjectModel;
using Zen.Ux.Mvvm.Model;

namespace Zen.Ux.Mvvm.ViewModel
{
    //Translates data provided by the Model into a 
    //convenient structure for the View
    public class FacadeVM : BaseVM, IViewModel
    {
        private readonly IProvider _provider;
        private FacadeBmo _currentFacade;

        public FacadeVM(IProvider provider)
        {
            _provider = provider;
            _provider.Login("leroy","abc123");
        }

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
        
        public bool IsLoaded { private set; get; }
     

        public void LoadFacades()
        {
            Facades = _provider.GetFacades();

            foreach (var entity in Facades)
                Facades.Add(entity);

            if (Facades.Count > 0)
                CurrentFacade = Facades[0];

            IsLoaded = true;
        }

        public void UnloadSchedulers()
        {
            // clear entitys from display.
            Facades.Clear();

            CurrentFacade = null;
            IsLoaded = false;
        }
    }
}
