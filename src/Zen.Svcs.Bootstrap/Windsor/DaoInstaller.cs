using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using NHibernate;
using Zen.Data;
using Zen.Log;

namespace Zen.Svcs.Bootstrap.Windsor
{
    /// <summary>
    /// Register for data access using the specified SessionFactory and Dao 
    ///  
    /// Don't forget to set the SessionFactory instance before initializing the container!
    /// </summary>
    public class DaoInstaller : IWindsorInstaller
    {

        /// <summary>
        /// This must be set prior to initializing the WindsorDI calling startup
        /// </summary>
        public static ISessionFactory SessionFactory { get; set; }
        
        //NHibernateDaoInstaller.SessionFactory = Zen.Quartz.Automap.
        //        QuartzDbAutomap.GetSqLiteSessionFactory("FUQ",false,null);

        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            if (SessionFactory == null)
            {
                "SessionFactory is not set on DaoInstaller. Using NoDao.".LogMe(LogLevel.Warn);
                container.Register(Component.For<IGenericDao>()
                                   .ImplementedBy<NoDao>()
                                   .LifestyleSingleton());
                return;

            }
            container.Register(Component.For<ISessionFactory>()
                                   .Instance(SessionFactory)
                                   .LifestyleSingleton());

            container.Register(Component.For<IGenericDao>()
                                   .ImplementedBy<NHibernateDao>()
                                   .LifestyleSingleton());

        }

    }
}