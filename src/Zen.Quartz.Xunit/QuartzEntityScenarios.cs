using Xbehave;
using System;
using Zen.Quartz.Entities;
using Zen.Xunit.Tests.Core;

namespace Zen.Quartz.Xunit
{
    public class ZenSchedulerEntityScenarios : EntityScenarios<Scheduler, Guid>
    {
        [Scenario]
        public override void CreateDefault()
        { base.CreateDefault(); 
        }
    }


    public class ZenJobEntityScenarios  : EntityScenarios<Job, Guid>
    {
        [Scenario]
        public override void CreateDefault()
        { base.CreateDefault(); 
        }
    }

}
