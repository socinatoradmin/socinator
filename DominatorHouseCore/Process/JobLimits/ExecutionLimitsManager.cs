#region

using System;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Process.ExecutionCounters;
using DominatorHouseCore.Utility;

#endregion

namespace DominatorHouseCore.Process.JobLimits
{
    public interface IExecutionLimitsManager
    {
        ReachedLimitInfo CheckIfLimitreached<T>(JobKey key, SocialNetworks networks, ActivityType activityType)
            where T : class, new();

        int GetCurrentJobCount(JobKey key);
    }

    public class ExecutionLimitsManager : IExecutionLimitsManager
    {
        private readonly IEntityCountersManager _entityCountersManager;
        private readonly IJobLimitsHolder _jobLimitsHolder;
        private readonly IJobCountersManager _jobCountersManager;

        public ExecutionLimitsManager(IEntityCountersManager entityCountersManager, IJobLimitsHolder jobLimitsHolder,
            IJobCountersManager jobCountersManager)
        {
            _entityCountersManager = entityCountersManager;
            _jobLimitsHolder = jobLimitsHolder;
            _jobCountersManager = jobCountersManager;
        }

        public ReachedLimitInfo CheckIfLimitreached<T>(JobKey key, SocialNetworks networks, ActivityType activityType)
            where T : class, new()
        {
            var noOfActionPerformedCurrentJob = _jobCountersManager[key];
            var counters = _entityCountersManager.GetCounter<T>(key.AccountId, networks, activityType);
            var limits = _jobLimitsHolder[key];

            if (counters.NoOfActionPerformedCurrentWeek >= limits.MaxNoOfActionPerWeek)
                return new ReachedLimitInfo(ReachedLimitType.Weekly, limits.MaxNoOfActionPerWeek);

            if (counters.NoOfActionPerformedCurrentDay >= limits.MaxNoOfActionPerDay)
                return new ReachedLimitInfo(ReachedLimitType.Daily, limits.MaxNoOfActionPerDay);

            if (counters.NoOfActionPerformedCurrentHour >= limits.MaxNoOfActionPerHour)
                return new ReachedLimitInfo(ReachedLimitType.Hourly, limits.MaxNoOfActionPerHour);
            return noOfActionPerformedCurrentJob >= limits.MaxNoOfActionPerJob
                ? new ReachedLimitInfo(ReachedLimitType.Job, limits.MaxNoOfActionPerJob)
                : new ReachedLimitInfo(ReachedLimitType.NoLimit, 0);

            ;
        }

        public int GetCurrentJobCount(JobKey key)
        {
            try
            {
                return _jobLimitsHolder[key].MaxNoOfActionPerJob;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return 0;
        }
    }
}