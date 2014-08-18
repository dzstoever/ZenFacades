using System;
using System.Windows.Forms;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
//using Zen.Ux.Bootstrap;
using Zen.Ioc;
using Zen.Log;
using Zen.Ux.WinApp.Mvp;
using Zen.Ux.WinApp.Mvp.Views;

namespace Zen.Ux.WinApp
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);            
            
            //ZenBootstrapper.Startup();

            //var logZen = ZenProvider.GetLogger(typeof (Program));
            //Log4netConfigurator.LogAllLevelMessages(logZen);

            
            // add any installers from the Bootstrap assembly
            WindsorDI.ConfigureFromAssembly = typeof(Zen.Svcs.Bootstrap.HostStartupShell).Assembly;            
            using (var di = Aspects.GetIocDI())
            {
                try
                {   
                    di.Initialize();
                    Navigation.AttachNavigator(di.Resolve<INavigator>()); 

                    //var logDI = di.Resolve<ILogger>();
                    //Log4netConfigurator.LogAllLevelMessages(logDI);
                }
                catch (Exception exc)
                {
                    Aspects.GetLogger().Fatal("Could not initialize Ioc/DI.", exc); 
                }
                Application.Run(di.Resolve<IMainView>() as Form);    
            }
            
        }

        
    }

    // Note: any installers in this assembly are used by default    
    /// <summary>
    /// Navigator is the King,
    ///     it contains all the views,
    ///     which contain the presenters,
    ///     which contain the services,
    ///     which contain the daos,
    ///     *anyone can have a logger
    /// </summary>
    public class ProgramInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<INavigator>()
                .ImplementedBy<Navigator>().LifestyleSingleton());

            container.Register(Component.For<IMainView>()
                .ImplementedBy<MainForm>().LifestyleSingleton());

            container.Register(Component.For<IView2>()
                .ImplementedBy<Form2>().LifestyleTransient());

            container.Register(Component.For<IView3>()
                .ImplementedBy<Form3>().LifestyleTransient());
        }
    }

}
