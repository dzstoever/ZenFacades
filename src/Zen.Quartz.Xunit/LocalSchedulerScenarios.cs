using Xbehave;
using FluentAssertions;
using Quartz;
using Quartz.Impl;
using System.Collections.Specialized;
using Zen.Log;
using Zen.Xunit.Tests;

namespace Zen.Quartz.Xunit
{

    public class LocalSchedulerScenarios : UseLogFixture
    {   

        protected NameValueCollection _props = new NameValueCollection(); //if empty, properties are loaded by Quartz from embedded resource file
        //Todo: pass different scheduler properties as inline parameters
        
        [Scenario]
        public virtual void GetScheduler()
        {
            IScheduler sched = null;

            "When getting a scheduler instance".When(() =>
            {//act
                 sched = new StdSchedulerFactory(_props).GetScheduler();
                 sched.GetDescription().LogMe(LogLevel.Debug);                
            });

            "Then ".Then(() => 
            {
                sched.Should().NotBeNull("it is not null");
                sched.InStandbyMode.Should().BeTrue("it is in standby");
                
            });
        }

        [Scenario]
        public virtual void StartScheduler()
        {
            IScheduler sched = null;

            "Given a scheduler that is in standby".Given(() =>
            {//arrange
                sched = new StdSchedulerFactory(_props).GetScheduler();
            });

            "When calling start".When(() =>
            {//act
                sched.Start();
                sched.GetDescription().LogMe(LogLevel.Debug);
            });

            "Then ".Then(() => sched.IsStarted.Should().BeTrue("it is started"));                
        }

        [Scenario]
        public virtual void ShutdownScheduler()
        {
            IScheduler sched = null;

            "Given a scheduler that is started".Given(() =>
            {//arrange
                sched = new StdSchedulerFactory(_props).GetScheduler();
                sched.Start();
            });

            "When calling shutdown".When(() =>
            {//act
                sched.Shutdown();
                sched.GetDescription().LogMe(LogLevel.Debug);
            });
            
            "Then ".Then(() => sched.IsShutdown.Should().BeTrue("it is shutdown"));
            //Note: IsStarted, and InStandbyMode are also true.
        }

        [Scenario]
        public virtual void PauseScheduler()
        {
            IScheduler sched = null;

            "Given a scheduler that is started".Given(() =>
            {//arrange
                sched = new StdSchedulerFactory(_props).GetScheduler();
                sched.Start();
            });

            "When calling pause all".When(() =>
            {//act
                sched.PauseAll();
                sched.GetDescription().LogMe(LogLevel.Debug);
            });

            "Then ".Then(() =>
            {
                sched.IsStarted.Should().BeTrue("it is still running.");
                
                var groupNames = sched.GetJobGroupNames();
                if (groupNames.Count == 0) Log.Info("No job groups found to pause.");
                foreach (var groupName in groupNames)
                {
                    Log.InfoFormat("GroupName = {0}", groupName);
                    sched.IsJobGroupPaused(groupName).Should().BeTrue("pause all should pause all job groups");
                }             
            });
        }

        [Scenario]
        public virtual void ResumeScheduler()
        {
            IScheduler sched = null;

            "Given a scheduler that is paused".Given(() =>
            {//arrange
                sched = new StdSchedulerFactory(_props).GetScheduler();
                sched.Start();
                sched.PauseAll();
            });

            "When calling resume all".When(() =>
            {//act
                sched.ResumeAll();
                sched.GetDescription().LogMe(LogLevel.Debug);
            });

            "Then ".Then(() =>
            {
                sched.IsStarted.Should().BeTrue("it is still running.");
                
                var groupNames = sched.GetJobGroupNames();
                if (groupNames.Count == 0) Log.Info("No job groups found to resume.");
                foreach (var groupName in groupNames)
                {
                    Log.InfoFormat("GroupName = {0}", groupName);
                    sched.IsJobGroupPaused(groupName).Should().BeFalse("resume all should resume all job groups");
                }
            });
        }

    }








    //Todo: testing for whether we can add a link from a QuartzScheduler/Job to a ZenSched/Job
    public class adding_a_ZenLocalScheduler_to_the_Db
    {

    }

    /* Remote
    public class using_a_RemoteScheduler_hosted_by_a_server_running_in_a_windows_service {}

    public class starting_a_RemoteScheduler { }
    /...
    public class adding_a_RemoteScheduler { } */

    /* RemoteWcf
    
    public class using_a_RemoteScheduler_hosted_by_a_server_running_in_a_wcf_service 

    public class starting_a_WcfScheduler { } */
}