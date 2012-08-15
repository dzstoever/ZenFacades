using System;

namespace Zen.Ux.Mvvm.Model
{
    // The Model's only responsibility is MAINTAINING THE DATA for the ViewModel
    public class SchedulerBmo : BaseBmo
    {
        //public SchedulerBmo() { }
        
        //private readonly IProvider _provider;

        private Guid _id;       
        private string _name;
        private string _cluster;
        private string _schedType;
        //private string _version;//This is not the db version

        public Guid Id
        {
            get { ConfirmOnUIThread(); return _id; }
            set { ConfirmOnUIThread(); if (_id != value) { _id = value; Notify("Id"); } }
        }        
        public string Name
        {
            get { ConfirmOnUIThread(); return _name; }
            set { ConfirmOnUIThread(); if (_name != value) { _name = value; Notify("Name"); } }
        }        
        public string Cluster
        {
            get { ConfirmOnUIThread(); return _cluster; }
            set { ConfirmOnUIThread(); if (_cluster != value) { _cluster = value; Notify("Cluster"); } }
        }
        public string SchedType
        {
            get { ConfirmOnUIThread(); return _schedType; }
            set { ConfirmOnUIThread(); if (_schedType != value) { _schedType = value; Notify("SchedType"); } }
        }
        //public string Version
        //{
        //    get { ConfirmOnUIThread(); return _version; }
        //    set { ConfirmOnUIThread(); if (_version != value) { _version = value; Notify("Version"); } }
        //}

        //public ObservableCollection<JobModel> Jobs
        //{
        //    get { ConfirmOnUIThread(); LazyloadJobs(); return _jobs; }
        //    set { ConfirmOnUIThread(); _jobs = value; Notify("Jobs"); }
        //}
        //private ObservableCollection<JobModel> _jobs;

        // Private helper that performs lazy loading of jobs.
        //private void LazyloadJobs()
        //{
        //    if (_jobs == null)
        //    {
        //        Jobs = _provider.GetJobs(this.Id) ?? new ObservableCollection<JobModel>();
        //    }
        //}
        
        
        public int Add()
        {
            return 0;//_provider.AddScheduler(this);
        }

        public int Update()
        {
            return 0;//_provider.UpdateScheduler(this);
        }

        public int Delete()
        {
            //var jobs = _provider.GetJobs(_schedulerId);
            //if (jobs == null || jobs.Count == 0) 
            //    return _provider.DeleteScheduler(_schedulerId);

            return 0; // Nothing deleted because scheduler has jobs.
        }

    }
}
