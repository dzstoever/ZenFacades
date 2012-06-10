using HibernatingRhinos.Profiler.Appender.NHibernate;

namespace Zen.Svcs.Bootstrap.StartupTasks
{

    /// <summary>
    /// Initialize the profiler
    /// </summary>
    /// <remarks>
    /// If the OfflineFileName is set, this will generate a file with a
    /// snapshot of all the NHibernate activity in the application, which 
    /// you can use for later analysis by loading the file into the profiler.
    /// </remarks>    
    public class NHProfilerStartupTask //: IStartupTask - disabled due to compiler exceptions for missing dlls...
    {         
        public static string OfflineFileName  { get; set; }

        public void Run()
        {
            //only initialize if the dll exists
            if (!new ImplChecker().CheckForDll("HibernatingRhinos.Profiler.Appender.v4.0.dll")) 
                return;
            
            if(string.IsNullOrWhiteSpace(OfflineFileName)) 
                 NHibernateProfiler.Initialize();
            else NHibernateProfiler.InitializeOfflineProfiling(OfflineFileName);
        }

        public void Reset()
        { //nothing to do 
        }	
	}
}

