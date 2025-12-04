#region

using System.Collections.Generic;

#endregion

namespace DominatorHouseCore.Process.ExecutionCounters
{
    public interface IJobCountersManager
    {
        void Reset(JobKey key);
        void InitOrIncrement(JobKey key);
        int this[JobKey key] { get; }
    }

    public class JobCountersManager : IJobCountersManager
    {
        private readonly Dictionary<JobKey, int> _parsingsPerJob;
        private readonly object _syncContext;

        public JobCountersManager()
        {
            _parsingsPerJob = new Dictionary<JobKey, int>();
            _syncContext = new object();
        }

        public void Reset(JobKey key)
        {
            lock (_syncContext)
            {
                if (_parsingsPerJob.ContainsKey(key))
                    _parsingsPerJob[key] = 0;
                else
                    _parsingsPerJob.Add(key, 0);
            }
        }

        public void InitOrIncrement(JobKey key)
        {
            lock (_syncContext)
            {
                if (_parsingsPerJob.ContainsKey(key))
                    _parsingsPerJob[key]++;
                else
                    _parsingsPerJob.Add(key, 1);
            }
        }

        public int this[JobKey key]
        {
            get
            {
                lock (_syncContext)
                {
                    return _parsingsPerJob.ContainsKey(key) ? _parsingsPerJob[key] : 0;
                }
            }
        }
    }
}