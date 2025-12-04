#region

using System.Collections.Generic;

#endregion

namespace DominatorHouseCore.Process
{
    public interface IRunningJobsHolder
    {
        bool IsRunning(JobKey id);
        bool Stop(JobKey id, bool isStopIfAccountLoginFail = false);
        bool StartIfNotRunning(JobKey id, IJobProcess jobProcess);
        bool IsActivityRunningForAccount(string accountId);
    }

    /// <summary>
    ///     Stores all running job processes. Key - TemplateId
    /// </summary>
    public class RunningJobsHolder : IRunningJobsHolder
    {
        private readonly Dictionary<JobKey, IJobProcess> _runningJobProcesses = new Dictionary<JobKey, IJobProcess>();
        private readonly HashSet<string> _runningAccounts = new HashSet<string>();
        private readonly object _syncJobProcess = new object();

        public bool IsRunning(JobKey id)
        {
            lock (_syncJobProcess)
            {
                return _runningJobProcesses.ContainsKey(id);
            }
        }

        public bool Stop(JobKey id, bool isStopIfAccountLoginFail = false)
        {
            lock (_syncJobProcess)
            {
                if (!_runningJobProcesses.ContainsKey(id)) return false;
                var jobProcess = _runningJobProcesses[id];
                _runningJobProcesses.Remove(id);
                _runningAccounts.Remove(id.AccountId);
                if (!isStopIfAccountLoginFail)
                    jobProcess.Stop();
                return true;
            }
        }

        public bool StartIfNotRunning(JobKey id, IJobProcess jobProcess)
        {
            lock (_syncJobProcess)
            {
                if (_runningJobProcesses.ContainsKey(id))
                    return false;
                _runningAccounts.Add(id.AccountId);
                _runningJobProcesses.Add(id, jobProcess);
                return true;
            }
        }

        public bool IsActivityRunningForAccount(string accountId)
        {
            lock (_syncJobProcess)
            {
                return _runningAccounts.Contains(accountId);
            }
        }
    }
}