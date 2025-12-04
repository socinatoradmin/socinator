#region

using DominatorHouseCore.DatabaseHandler.GdTables.Accounts;
using DominatorHouseCore.Enums;
using Unity;
using Unity.Extension;

#endregion

namespace DominatorHouseCore.DatabaseHandler.Common.EntityCounters
{
    public class EntityCounterFunctionInstagramRegisterUnityExtension : UnityContainerExtension
    {
        protected override void Initialize()
        {
            Container
                .RegisterInstance<IEntityCounterFunction<InteractedUsers>>(
                    new EntityCounterFunction<InteractedUsers>(
                        new DateEpochFilterPredicate<InteractedUsers>(
                            a => a.Date),
                        new ActivityTypeAsStringFilterPredicate<InteractedUsers>(
                            a => a.ActivityType)));
            Container
                .RegisterInstance<ICounterKeyFactory<InteractedUsers>>(
                    new CounterKeyFactory<InteractedUsers>(SocialNetworks.Instagram, true));

            Container
                .RegisterInstance<IEntityCounterFunction<InteractedPosts>>(
                    new EntityCounterFunction<InteractedPosts>(
                        new DateEpochFilterPredicate<InteractedPosts>(
                            a => a.InteractionDate),
                        new ActivityTypeFilterPredicate<InteractedPosts>(
                            a => a.ActivityType)));
            Container
                .RegisterInstance<ICounterKeyFactory<InteractedPosts>>(
                    new CounterKeyFactory<InteractedPosts>(SocialNetworks.Instagram, true));

            Container
                .RegisterInstance<IEntityCounterFunction<UnfollowedUsers>>(
                    new EntityCounterFunction<UnfollowedUsers>(
                        new DateEpochFilterPredicate<UnfollowedUsers>(
                            a => a.InteractionDate)));
            Container
                .RegisterInstance<ICounterKeyFactory<UnfollowedUsers>>(
                    new CounterKeyFactory<UnfollowedUsers>(SocialNetworks.Instagram, false));


            Container
                .RegisterInstance<IEntityCounterFunction<MakeCloseFriendAccount>>(
                    new EntityCounterFunction<MakeCloseFriendAccount>(
                        new DateEpochFilterPredicate<MakeCloseFriendAccount>(
                            a => a.InteractedDate)));
            Container
                .RegisterInstance<ICounterKeyFactory<MakeCloseFriendAccount>>(
                    new CounterKeyFactory<MakeCloseFriendAccount>(SocialNetworks.Instagram, false));



            Container
                .RegisterInstance<IEntityCounterFunction<HashtagScrape>>(
                    new EntityCounterFunction<HashtagScrape>(
                        new DateEpochFilterPredicate<HashtagScrape>(
                            a => a.Date)));
            Container
                .RegisterInstance<ICounterKeyFactory<HashtagScrape>>(
                    new CounterKeyFactory<HashtagScrape>(SocialNetworks.Instagram, false));

            Container
                .RegisterInstance<ICounterKeyFactory<UserConversation>>(
                    new CounterKeyFactory<UserConversation>(SocialNetworks.Instagram, false));
        }
    }
}