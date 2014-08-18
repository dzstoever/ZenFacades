using System;
using Zen.Core;

namespace Zen.Quartz.Entities
{
    /// <summary>
    /// Scheduler entity with some custom info needed by the domain model.
    /// Also acts as wrapper around a Quartz.IScheduler, allowing us to 
    /// interact the Quartz built-in interfaces.
    /// </summary>
    public class Scheduler : DomainEntity<Guid>
    {        
        public virtual string Name { get; set; }
        public virtual string Cluster { get; set; }
        public virtual string SchedType { get; set; }

        //private static ISchedulerFactory schedFactory;
        //private IScheduler sched;// = new StdScheduler();
    }
}
