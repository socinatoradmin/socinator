#region

using DominatorHouseCore.DatabaseHandler.TumblrTables.Account;
using DominatorHouseCore.Enums;
using Unity;
using Unity.Extension;

#endregion

namespace DominatorHouseCore.DatabaseHandler.Common.EntityCounters
{
    public class EntityCounterFunctionTumblrRegisterUnityExtension : UnityContainerExtension
    {
        protected override void Initialize()
        {
            Container
                .RegisterInstance<IEntityCounterFunction<InteractedUser>>(
                    new EntityCounterFunction<InteractedUser>(
                        new DateEpochFilterPredicate<InteractedUser>(
                            a => a.InteractionTimeStamp),
                        new ActivityTypeAsStringFilterPredicate<InteractedUser>(
                            a => a.ActivityType)));
            Container
                .RegisterInstance<ICounterKeyFactory<InteractedUser>>(
                    new CounterKeyFactory<InteractedUser>(SocialNetworks.Tumblr, true));


            Container
                .RegisterInstance<IEntityCounterFunction<InteractedPosts>>(
                    new EntityCounterFunction<InteractedPosts>(
                        new DateEpochFilterPredicate<InteractedPosts>(
                            a => a.InteractionTimeStamp),
                        new ActivityTypeAsStringFilterPredicate<InteractedPosts>(
                            a => a.ActivityType)));
            Container
                .RegisterInstance<ICounterKeyFactory<InteractedPosts>>(
                    new CounterKeyFactory<InteractedPosts>(SocialNetworks.Tumblr, true));

            Container
                .RegisterInstance<IEntityCounterFunction<UnFollowedUser>>(
                    new EntityCounterFunction<UnFollowedUser>(
                        new DateEpochFilterPredicate<UnFollowedUser>(
                            a => a.InteractionTimeStamp)));
            Container
                .RegisterInstance<ICounterKeyFactory<UnFollowedUser>>(
                    new CounterKeyFactory<UnFollowedUser>(SocialNetworks.Tumblr, false));
        }
    }
}