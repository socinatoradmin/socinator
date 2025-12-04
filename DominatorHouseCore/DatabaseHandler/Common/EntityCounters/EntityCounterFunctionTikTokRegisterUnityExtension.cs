#region

using DominatorHouseCore.DatabaseHandler.TtdTables.Accounts;
using DominatorHouseCore.Enums;
using Unity;
using Unity.Extension;

#endregion

namespace DominatorHouseCore.DatabaseHandler.Common.EntityCounters
{
    public class EntityCounterFunctionTikTokRegisterUnityExtension : UnityContainerExtension
    {
        protected override void Initialize()
        {
            Container
                .RegisterInstance<IEntityCounterFunction<InteractedUsers>>(
                    new EntityCounterFunction<InteractedUsers>(
                        new DateEpochFilterPredicate<InteractedUsers>(a => a.Date),
                        new ActivityTypeAsStringFilterPredicate<InteractedUsers>(a => a.ActivityType)));
            Container
                .RegisterInstance<ICounterKeyFactory<InteractedUsers>>(
                    new CounterKeyFactory<InteractedUsers>(SocialNetworks.TikTok, true));


            Container
                .RegisterInstance<IEntityCounterFunction<InteractedPosts>>(
                    new EntityCounterFunction<InteractedPosts>(
                        new DateEpochFilterPredicate<InteractedPosts>(
                            a => a.InteractionDate),
                        new ActivityTypeAsStringFilterPredicate<InteractedPosts>(
                            a => a.ActivityType)));
            Container
                .RegisterInstance<ICounterKeyFactory<InteractedPosts>>(
                    new CounterKeyFactory<InteractedPosts>(SocialNetworks.TikTok, true));


            Container
                .RegisterInstance<IEntityCounterFunction<UnfollowedUsers>>(
                    new EntityCounterFunction<UnfollowedUsers>(
                        new DateEpochFilterPredicate<UnfollowedUsers>(
                            a => a.Date)));
            Container
                .RegisterInstance<ICounterKeyFactory<UnfollowedUsers>>(
                    new CounterKeyFactory<UnfollowedUsers>(SocialNetworks.TikTok, false));


            Container
                .RegisterInstance<IEntityCounterFunction<InteractedHashTags>>(
                    new EntityCounterFunction<InteractedHashTags>(
                        new DateEpochFilterPredicate<InteractedHashTags>(
                            a => a.Date)));
            Container
                .RegisterInstance<ICounterKeyFactory<InteractedHashTags>>(
                    new CounterKeyFactory<InteractedHashTags>(SocialNetworks.TikTok, false));
        }
    }
}