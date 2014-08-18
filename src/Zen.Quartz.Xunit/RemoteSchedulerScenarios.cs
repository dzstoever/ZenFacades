using Xbehave;
using FluentAssertions;
using Quartz;
using Quartz.Impl;
using Zen.Log;
using Zen.Xunit.Tests;

namespace Zen.Quartz.Xunit
{
    public class RemoteSchedulerScenarios : UseLogFixture
    {
        [Scenario]
        public virtual void GetRemoteScheduler()
        {
            IScheduler sched = null;

            "Given a set of properties".Given(() =>
            {
                //var props = new NameValueCollection();
                //props.Add("quartz.scheduler.proxy", "true");
                //props.Add("quartz.scheduler.proxy.address", "tcp://localhost:555/QuartzScheduler");                
                //sched = new StdSchedulerFactory(props).GetScheduler();
            });

            "When getting a remote scheduler client".When(() =>
            {//act
                sched = SchedulerFactory.CreateClientSideSched(null, null, null);
                sched.GetDescription().LogMe(LogLevel.Debug);
            });

            "Then ".Then(() =>
            {        
                sched.Should().NotBeNull("it is not null");
                sched.Should().BeAssignableTo<RemoteScheduler>("it should be a remoteable scheduler");

            });
        }

        /*
         *	3 - The user can view a list of available schedulers 
         *		- the view must group/filter by the cluster(context)
         *		- the view must group/filter by the status (win service [Running-Stopped]) 
         */
        [Scenario]
        public virtual void GetRemoteSchedulerList()
        {
            "Given some arranged preconditions".Given(() =>
            {
                Log.InfoFormat("given 1");
                Log.InfoFormat("given 2");
                Log.InfoFormat("given 3");
            }, () =>
            {
                Log.InfoFormat("release 1");
                Log.InfoFormat("release 2");
                Log.InfoFormat("release 3");
            });

            "When the code under test runs".When(() =>
            {
                Log.InfoFormat("when a");
                Log.InfoFormat("when b");
                Log.InfoFormat("when c");
            });

            "Then the actual results meet expectations".Then(() =>
            {
                Log.InfoFormat("then x");
                Log.InfoFormat("then y");
                Log.InfoFormat("then z");
            });


        }
    }
}
