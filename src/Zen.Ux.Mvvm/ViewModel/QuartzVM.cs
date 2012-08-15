using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Zen.Ux.Mvvm.Model;

namespace Zen.Ux.Mvvm.ViewModel
{
    //Translates data provided by the Model into a 
    //convenient structure for the View
    public class QuartzVM : BaseVM, IViewModel
    {
        private readonly IProvider _provider;
        private SchedulerBmo _currentSchedulerBmo;
        
        public QuartzVM()//IProvider provider)
        {
            //_provider = provider;

            Schedulers = new ObservableCollection<SchedulerBmo>();

            AddCmd = new AddUICmd(this);
            EditCmd = new EditUICmd(this);
            DeleteCmd = new DeleteUICmd(this);
        }        



        public SchedulerBmo NewSchedulerBmo
        {
            get { return new SchedulerBmo(); }
        }
        
        public SchedulerBmo CurrentScheduler
        {
            get { return _currentSchedulerBmo; }
            set
            {
                if (_currentSchedulerBmo != value)
                {
                    _currentSchedulerBmo = value;
                    OnPropertyChanged("CurrentScheduler");
                }
            }
        }
                
        public ObservableCollection<SchedulerBmo> Schedulers { private set; get; }

        /// <summary>Indicates whether the  data has been loaded.
        /// </summary>
        public bool IsLoaded { private set; get; }

        /// <summary>Retrieves and displays entitys in given sort order.
        /// </summary>        
        public void LoadSchedulers()
        {
            const string sortExpression = "Name ASC";
            foreach(var entity in _provider.GetSchedulers(sortExpression))
                Schedulers.Add(entity);
            
            if (Schedulers.Count > 0)
                CurrentScheduler = Schedulers[0];

            IsLoaded = true;
        }

        /// <summary>Clear entitys from display.
        /// </summary>
        public void UnloadSchedulers()
        {
            Schedulers.Clear();

            CurrentScheduler = null;
            IsLoaded = false;
        }


        public bool CanView
        {
            get { return IsLoaded && CurrentScheduler != null; }
        }

        public bool CanAdd
        {
            get { return IsLoaded; }
        }

        public bool CanEdit
        {
            get { return IsLoaded && CurrentScheduler != null; }
        }

        public bool CanDelete
        {
            get { return IsLoaded && CurrentScheduler != null; }
        }
        
        public BaseUICmd AddCmd { private set; get; }

        public BaseUICmd EditCmd { private set; get; }

        public BaseUICmd DeleteCmd { private set; get; }

        #region Private Command classes

        private class AddUICmd : BaseUICmd
        {
            private readonly QuartzVM _vm;

            public AddUICmd(QuartzVM viewModel)
            {
                _vm = viewModel;
            }

            public override void OnCanExecute(object sender, CanExecuteRoutedEventArgs e)
            {
                var entity = e.Parameter as SchedulerBmo;

                // Check that all values have been entered.
                e.CanExecute =
                    (!string.IsNullOrEmpty(entity.Name)
                    && !string.IsNullOrEmpty(entity.Cluster));


                e.Handled = true;
            }

            public override void OnExecute(object sender, ExecutedRoutedEventArgs e)
            {
                var entity = e.Parameter as SchedulerBmo;
                entity.Add();

                _vm.Schedulers.Add(entity);
                _vm.CurrentScheduler = entity;
            }
        }

        private class EditUICmd : BaseUICmd
        {
            private readonly QuartzVM _vm;

            public EditUICmd(QuartzVM viewModel)
            {
                _vm = viewModel;
            }

            public override void OnCanExecute(object sender, CanExecuteRoutedEventArgs e)
            {
                var model = e.Parameter as SchedulerBmo;

                // Check that all values have been set
                e.CanExecute = (model.Id != default(Guid)
                  && !string.IsNullOrEmpty(model.Name)
                  && !string.IsNullOrEmpty(model.Cluster));

                e.CanExecute = _vm.CanEdit;
                e.Handled = true;
            }

            public override void OnExecute(object sender, ExecutedRoutedEventArgs e)
            {
                var entityModel = e.Parameter as SchedulerBmo;
                entityModel.Update();
            }
        }

        private class DeleteUICmd : BaseUICmd
        {
            private readonly QuartzVM _vm;

            public DeleteUICmd(QuartzVM viewModel)
            {
                _vm = viewModel;
            }

            public override void OnCanExecute(object sender, CanExecuteRoutedEventArgs e)
            {
                e.CanExecute = _vm.CanDelete;
                e.Handled = true;
            }

            public override void OnExecute(object sender, ExecutedRoutedEventArgs e)
            {
                var entityModel = _vm.CurrentScheduler;

                if (entityModel.Delete() > 0)
                {
                    _vm.Schedulers.Remove(entityModel);
                    e.Handled = true;
                }
            }
        }

        #endregion
         
    }

    
}