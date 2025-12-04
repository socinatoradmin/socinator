#region

using DominatorHouseCore.DatabaseHandler.YdTables.Accounts;
using DominatorHouseCore.Enums;
using Unity;
using Unity.Extension;

#endregion

namespace DominatorHouseCore.DatabaseHandler.Common.EntityCounters
{
    public class EntityCounterFunctionYoutubeRegisterUnityExtension : UnityContainerExtension
    {
        protected override void Initialize()
        {
            Container
                .RegisterInstance<IEntityCounterFunction<InteractedPosts>>(
                    new EntityCounterFunction<InteractedPosts>(
                        new DateEpochFilterPredicate<InteractedPosts>(
                            a => a.InteractionTimeStamp),
                        new ActivityTypeAsStringFilterPredicate<InteractedPosts>(
                            a => a.ActivityType)));
            Container
                .RegisterInstance<ICounterKeyFactory<InteractedPosts>>(
                    new CounterKeyFactory<InteractedPosts>(SocialNetworks.YouTube, true));

            Container
                .RegisterInstance<IEntityCounterFunction<InteractedChannels>>(
                    new EntityCounterFunction<InteractedChannels>(
                        new DateEpochFilterPredicate<InteractedChannels>(
                            a => a.InteractionTimeStamp),
                        new ActivityTypeAsStringFilterPredicate<InteractedChannels>(
                            a => a.ActivityType)));
            Container
                .RegisterInstance<ICounterKeyFactory<InteractedChannels>>(
                    new CounterKeyFactory<InteractedChannels>(SocialNetworks.YouTube, true));
        }
    }
}