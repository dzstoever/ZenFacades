using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using NHibernate;
using Zen.Data;
using Zen.Quartz.Automap;

namespace Zen.Quartz.Facade
{
    public class DaoInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<ISessionFactory>()
                .Instance(QuartzDbAutomap.GetSqLiteSessionFactory(QuartzDbAutomap.DefaultDbName, false, null))
                //.Instance(QuartzDbAutomap.GetMsSql2008SessionFactory("QuartzCnn", true, null))              
                .LifestyleSingleton());

            container.Register(Component.For<IGenericDao>()
                .ImplementedBy<NHibernateDao>()
                .LifestyleSingleton());

        }

    }
}