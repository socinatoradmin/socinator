#region

using System;
using System.Diagnostics.CodeAnalysis;
using FluentScheduler;

#endregion

namespace DominatorHouseCore.BusinessLogic.Scheduler
{
    public interface ISchedulerProxy
    {
        /// <summary>
        ///     Returns Null if schedule doesn't exist
        /// </summary>
        /// <param name="name">Schedule name</param>
        Schedule this[string name] { get; }

        void AddJob(Action action, Action<Schedule> schedule);
        void RemoveJob(string name);
    }

    [ExcludeFromCodeCoverage]
    public class SchedulerProxy : ISchedulerProxy
    {
        public Schedule this[string name] => JobManager.GetSchedule(name);

        public void AddJob(Action action, Action<Schedule> schedule)
        {
            JobManager.AddJob(action, schedule);
        }

        public void RemoveJob(string name)
        {
            JobManager.RemoveJob(name);
        }
    }
}