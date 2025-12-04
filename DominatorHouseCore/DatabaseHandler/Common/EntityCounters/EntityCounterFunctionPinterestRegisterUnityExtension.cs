#region

using DominatorHouseCore.DatabaseHandler.PdTables.Accounts;
using DominatorHouseCore.Enums;
using Unity;
using Unity.Extension;

#endregion

namespace DominatorHouseCore.DatabaseHandler.Common.EntityCounters
{
    public class EntityCounterFunctionPinterestRegisterUnityExtension : UnityContainerExtension
    {
        protected override void Initialize()
        {
            Container
                .RegisterInstance<IEntityCounterFunction<InteractedUsers>>(
                    new EntityCounterFunction<InteractedUsers>(
                        new DateEpochFilterPredicate<InteractedUsers>(
                            a => a.InteractionTime),
                        new ActivityTypeAsStringFilterPredicate<InteractedUsers>(
                            a => a.ActivityType)));
            Container
                .RegisterInstance<ICounterKeyFactory<InteractedUsers>>(
                    new CounterKeyFactory<InteractedUsers>(SocialNetworks.Pinterest, true));

            Container
                .RegisterInstance<IEntityCounterFunction<InteractedBoards>>(
                    new EntityCounterFunction<InteractedBoards>(
                        new DateEpochFilterPredicate<InteractedBoards>(
                            a => a.InteractionDate),
                        new ActivityTypeFilterPredicate<InteractedBoards>(
                            a => a.OperationType)));
            Container
                .RegisterInstance<ICounterKeyFactory<InteractedBoards>>(
                    new CounterKeyFactory<InteractedBoards>(SocialNetworks.Pinterest, true));

            Container
                .RegisterInstance<IEntityCounterFunction<InteractedPosts>>(
                    new EntityCounterFunction<InteractedPosts>(
                        new DateEpochFilterPredicate<InteractedPosts>(
                            a => a.InteractionDate),
                        new ActivityTypeAsStringFilterPredicate<InteractedPosts>(
                            a => a.OperationType)));
            Container
                .RegisterInstance<ICounterKeyFactory<InteractedPosts>>(
                    new CounterKeyFactory<InteractedPosts>(SocialNetworks.Pinterest, true));


            Container
                .RegisterInstance<IEntityCounterFunction<UnfollowedUsers>>(
                    new EntityCounterFunction<UnfollowedUsers>(
                        new DateEpochFilterPredicate<UnfollowedUsers>(
                            a => a.InteractionDate)));
            Container
                .RegisterInstance<ICounterKeyFactory<UnfollowedUsers>>(
                    new CounterKeyFactory<UnfollowedUsers>(SocialNetworks.Pinterest, false));

            Container
               .RegisterInstance<IEntityCounterFunction<CreateAccount>>(
                   new EntityCounterFunction<CreateAccount>(
                       new DateEpochFilterPredicate<CreateAccount>(
                           a => a.InteractionDate),
                       new ActivityTypeAsStringFilterPredicate<CreateAccount>(
                           a => a.ActivityType)));
            Container
                .RegisterInstance<ICounterKeyFactory<CreateAccount>>(
                    new CounterKeyFactory<CreateAccount>(SocialNetworks.Pinterest, true));
        }
    }
}