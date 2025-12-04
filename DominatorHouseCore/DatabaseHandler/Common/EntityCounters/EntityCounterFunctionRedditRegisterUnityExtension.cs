#region

using DominatorHouseCore.DatabaseHandler.RdTables.Accounts;
using DominatorHouseCore.Enums;
using Unity;
using Unity.Extension;

#endregion

namespace DominatorHouseCore.DatabaseHandler.Common.EntityCounters
{
    public class EntityCounterFunctionRedditRegisterUnityExtension : UnityContainerExtension
    {
        protected override void Initialize()
        {
            Container
                .RegisterInstance<IEntityCounterFunction<InteractedUsers>>(
                    new EntityCounterFunction<InteractedUsers>(
                        new DateEpochFilterPredicate<InteractedUsers>(
                            a => a.InteractionTimeStamp),
                        new ActivityTypeAsStringFilterPredicate<InteractedUsers>(
                            a => a.ActivityType)));
            Container
                .RegisterInstance<ICounterKeyFactory<InteractedUsers>>(
                    new CounterKeyFactory<InteractedUsers>(SocialNetworks.Reddit, true));

            Container
                .RegisterInstance<IEntityCounterFunction<InteractedSubreddit>>(
                    new EntityCounterFunction<InteractedSubreddit>(
                        new DateFilterPredicate<InteractedSubreddit>(
                            a => a.InteractionDateTime),
                        new ActivityTypeAsStringFilterPredicate<InteractedSubreddit>(
                            a => a.ActivityType)));
            Container
                .RegisterInstance<ICounterKeyFactory<InteractedSubreddit>>(
                    new CounterKeyFactory<InteractedSubreddit>(SocialNetworks.Reddit, true));

            Container
                .RegisterInstance<IEntityCounterFunction<InteractedPost>>(
                    new EntityCounterFunction<InteractedPost>(
                        new DateFilterPredicate<InteractedPost>(
                            a => a.InteractionDateTime),
                        new ActivityTypeAsStringFilterPredicate<InteractedPost>(
                            a => a.ActivityType)));
            Container
                .RegisterInstance<ICounterKeyFactory<InteractedPost>>(
                    new CounterKeyFactory<InteractedPost>(SocialNetworks.Reddit, true));
            Container
                .RegisterInstance<IEntityCounterFunction<InteractedAutoActivityPost>>(
                    new EntityCounterFunction<InteractedAutoActivityPost>(
                        new DateFilterPredicate<InteractedAutoActivityPost>(
                            a => a.InteractedDate),
                        new ActivityTypeAsStringFilterPredicate<InteractedAutoActivityPost>(
                            a => a.ActivityType)));
            Container
                .RegisterInstance<ICounterKeyFactory<InteractedAutoActivityPost>>(
                    new CounterKeyFactory<InteractedAutoActivityPost>(SocialNetworks.Reddit, true));

            Container
                .RegisterInstance<IEntityCounterFunction<UnfollowedUsers>>(
                    new EntityCounterFunction<UnfollowedUsers>(
                        new DateEpochFilterPredicate<UnfollowedUsers>(
                            a => a.InteractionDate)));
            Container
                .RegisterInstance<ICounterKeyFactory<UnfollowedUsers>>(
                    new CounterKeyFactory<UnfollowedUsers>(SocialNetworks.Reddit, false));
        }
    }
}