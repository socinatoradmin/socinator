#region

using System.Collections.Generic;
using DominatorHouseCore.Interfaces;

#endregion

namespace DominatorHouseCore.Process.JobLimits
{
    public interface IJobLimitsHolder
    {
        JobLimits this[JobKey key] { get; }

        void Reset(JobKey jobKey, IJobConfiguration jobConfiguration);
    }

    public class JobLimitsHolder : IJobLimitsHolder
    {
        private readonly Dictionary<JobKey, JobLimits> _jobLimits;
        private readonly object _syncObject;

        public JobLimits this[JobKey key]
        {
            get
            {
                lock (_syncObject)
                {
                    return _jobLimits[key];
                }
            }
        }

        public JobLimitsHolder()
        {
            _jobLimits = new Dictionary<JobKey, JobLimits>();
            _syncObject = new object();
        }

        public void Reset(JobKey jobKey, IJobConfiguration jobConfiguration)
        {
            var maxNoOfActionPerJob = jobConfiguration.ActivitiesPerJob.GetRandom();
            var maxNoOfActionPerHour = jobConfiguration.ActivitiesPerHour.GetRandom();
            var maxNoOfActionPerDay = jobConfiguration.ActivitiesPerDay.GetRandom();
            var maxNoOfActionPerWeek = jobConfiguration.ActivitiesPerWeek.GetRandom();
            var limits = new JobLimits(maxNoOfActionPerWeek, maxNoOfActionPerDay, maxNoOfActionPerHour,
                maxNoOfActionPerJob);

            lock (_syncObject)
            {
                if (_jobLimits.ContainsKey(jobKey))
                    _jobLimits[jobKey] = limits;
                else
                    _jobLimits.Add(jobKey, limits);
            }
        }
    }
}