using System;
using Xbehave;
using NHibernate;
using Zen.Quartz.Entities;
using Zen.Xunit.Tests.Data;

namespace Zen.Quartz.Xunit
{
    //Note: using an 'in memory db'(SqLite) to act as a 'mock' database (see remarks)        
    //var sqLiteSessionFactory = 
    //QuartzDbAutomap.GetSqLiteSessionFactory("Quartz.db", true, "QuartzDb.SqLiteMappings");


    public class ZenSchedRepositoryScenarios : RepositoryScenarios<Scheduler, Guid>
    {
        [Scenario]
        //Todo: add the inline data for session factorys
        public override void FetchAll(ISessionFactory sessionFactory)
        { base.FetchAll(sessionFactory); }
    }

    public class ZenJobRepositoryScenarios : RepositoryScenarios<Job, Guid>
    {
        [Scenario]
        public override void FetchAll(ISessionFactory sessionFactory)
        { base.FetchAll(sessionFactory); }
    }

}
