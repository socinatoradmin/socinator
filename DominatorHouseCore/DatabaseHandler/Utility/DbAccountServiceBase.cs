#region

using DominatorHouseCore.DatabaseHandler.Common;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Process.ExecutionCounters;

#endregion

namespace DominatorHouseCore.DatabaseHandler.Utility
{
    public abstract class DbAccountServiceBase
    {
        private readonly IEntityCountersManager _entityCountersManager;

        protected DbAccountServiceBase(IEntityCountersManager entityCountersManager)
        {
            _entityCountersManager = entityCountersManager;
        }

        protected void CountInteracted<T>(string accountId, SocialNetworks networks, params T[] data)
            where T : class, new()
        {
            if (typeof(IActivityTypeEntity).IsAssignableFrom(typeof(T)))
                foreach (var entity in data)
                    _entityCountersManager.IncrementFor<T>(accountId, networks,
                        ((IActivityTypeEntity) entity).GetActivityType());
            else
                foreach (var unused in data)
                    _entityCountersManager.IncrementFor<T>(accountId, networks, null);
        }
    }
}