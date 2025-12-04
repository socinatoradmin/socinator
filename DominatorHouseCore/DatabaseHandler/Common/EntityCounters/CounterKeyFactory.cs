#region

using DominatorHouseCore.Enums;

#endregion

namespace DominatorHouseCore.DatabaseHandler.Common.EntityCounters
{
    // ReSharper disable once UnusedTypeParameter
    public interface ICounterKeyFactory<TSource> where TSource : class, new()
    {
        string Create(string accountId, object activityType);
    }

    public class CounterKeyFactory<TSource> : ICounterKeyFactory<TSource> where TSource : class, new()
    {
        private readonly SocialNetworks _socialNetworks;
        private readonly bool _isActivityTypeBased;

        public CounterKeyFactory(SocialNetworks socialNetworks, bool isActivityTypeBased)
        {
            _socialNetworks = socialNetworks;
            _isActivityTypeBased = isActivityTypeBased;
        }

        public string Create(string accountId, object activityType)
        {
            var activityTypeSufix = _isActivityTypeBased ? activityType.ToString() : string.Empty;
            return $"{accountId}{typeof(TSource).Name}{_socialNetworks}{activityTypeSufix}";
        }
    }
}