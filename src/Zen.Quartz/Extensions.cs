using System;
using Quartz;
using Quartz.Impl;

namespace Zen.Quartz
{
    public static class Extensions
    {
        /* Scheduler extensions */

        public static string GetDescription(this IScheduler scheduler)
        {
            var isRemote = scheduler.GetType() == typeof(RemoteScheduler);
            return (
            "{0}-------------------------" +
            "{0}  Scheduler Description  " +
            "{0}-------------------------" +
            "{0}  .SchedulerInstanceId:  " + scheduler.SchedulerInstanceId +
            "{0}  .SchedulerName:        " + scheduler.SchedulerName +
            "{0}  .Context:              " + scheduler.Context +            
            "{0}  .InStandbyMode:        " + scheduler.InStandbyMode +
            "{0}  .IsStarted:            " + scheduler.IsStarted +
            "{0}  .IsShutdown:           " + scheduler.IsShutdown +
            "{0}  .ListenerManager:      " + (isRemote ? "can't listen to a RemoteScheduler" : scheduler.ListenerManager.ToString())
            ).FormatWith(Environment.NewLine);                
        }
    }
}
