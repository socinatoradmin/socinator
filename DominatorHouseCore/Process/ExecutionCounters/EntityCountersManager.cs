#region

using System.Collections.Generic;
using CommonServiceLocator;
using DominatorHouseCore.DatabaseHandler.Common.EntityCounters;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;

#endregion

namespace DominatorHouseCore.Process.ExecutionCounters
{
    public interface IEntityCountersManager
    {
        void IncrementFor<T>(string accountId, SocialNetworks networks, ActivityType? activityType)
            where T : class, new();

        EntityCounter GetCounter<T>(string accountId, SocialNetworks networks, ActivityType activityType)
            where T : class, new();
    }

    public class EntityCountersManager : IEntityCountersManager
    {
        private readonly Dictionary<string, EntityCounter> _jobExecutionCounters;
        private readonly object _syncObject = new object();

        public EntityCountersManager()
        {
            _jobExecutionCounters = new Dictionary<string, EntityCounter>();
        }

        public EntityCounter GetCounter<T>(string accountId, SocialNetworks networks, ActivityType activityType)
            where T : class, new()
        {
            lock (_syncObject)
            {
                var keyFactory = InstanceProvider.GetInstance<ICounterKeyFactory<T>>();
                var key = keyFactory.Create(accountId, activityType);
                if (!_jobExecutionCounters.ContainsKey(key))
                    Init<T>(key, accountId, networks, activityType);
                else
                    Update<T>(key, accountId, networks, activityType);
                return _jobExecutionCounters[key];
            }
        }

        /// <summary>
        ///     The method is executed through reflection <see cref="DbOperations" />
        /// </summary>
        public void IncrementFor<T>(string accountId, SocialNetworks networks, ActivityType? activityType)
            where T : class, new()
        {
            lock (_syncObject)
            {
                InitOrIncrement<T>(accountId, networks, activityType);
            }
        }

        private void InitOrIncrement<T>(string accountId, SocialNetworks networks, ActivityType? activityType)
            where T : class, new()
        {
            var keyFactory = InstanceProvider.GetInstance<ICounterKeyFactory<T>>();
            var key = keyFactory.Create(accountId, activityType);
            if (!_jobExecutionCounters.ContainsKey(key))
                Init<T>(key, accountId, networks, activityType);
            else
                Update<T>(key, accountId, networks, activityType);

            _jobExecutionCounters[key].Increment();
        }

        private void Init<T>(string key, string accountId, SocialNetworks networks, ActivityType? activityType)
            where T : class, new()
        {
            var counterFunction = InstanceProvider.GetInstance<IEntityCounterFunction<T>>();
            var counter = counterFunction.GetCounter(accountId, networks, activityType);
            _jobExecutionCounters.Add(key, counter);
        }

        private void Update<T>(string key, string accountId, SocialNetworks networks, ActivityType? activityType)
            where T : class, new()
        {
            var counterFunction = InstanceProvider.GetInstance<IEntityCounterFunction<T>>();
            var counter = counterFunction.GetCounter(accountId, networks, activityType);
            _jobExecutionCounters[key] = counter;
        }
    }
}