using DominatorHouseCore.Enums;
using DominatorHouseCore.Process;
using PinDominatorCore.PDLibrary.Process;
using Unity;
using Unity.Extension;
using Unity.Lifetime;

namespace PinDominatorCore.UnityContainers
{
    public class PinterestJobProcessUnityExtension : UnityContainerExtension
    {
        protected override void Initialize()
        {
            var socialNetwork = SocialNetworks.Pinterest;

            Container.RegisterType<IJobProcess, AcceptBoardInvitationsProcess>(
                $"{socialNetwork}{ActivityType.AcceptBoardInvitation}",
                new HierarchicalLifetimeManager());

            Container.RegisterType<IJobProcess, AutoReplyToNewMessageProcess>(
                $"{socialNetwork}{ActivityType.AutoReplyToNewMessage}",
                new HierarchicalLifetimeManager());

            Container.RegisterType<IJobProcess, BoardScraperProcess>($"{socialNetwork}{ActivityType.BoardScraper}",
                new HierarchicalLifetimeManager());

            Container.RegisterType<IJobProcess, BroadCastMessegeProcess>(
                $"{socialNetwork}{ActivityType.BroadcastMessages}",
                new HierarchicalLifetimeManager());

            Container.RegisterType<IJobProcess, CommentProcess>($"{socialNetwork}{ActivityType.Comment}",
                new HierarchicalLifetimeManager());

            Container.RegisterType<IJobProcess, CreateBoardProcess>($"{socialNetwork}{ActivityType.CreateBoard}",
                new HierarchicalLifetimeManager());

            Container.RegisterType<IJobProcess, DeletePinProcess>($"{socialNetwork}{ActivityType.DeletePin}",
                new HierarchicalLifetimeManager());

            Container.RegisterType<IJobProcess, EditPinProcess>($"{socialNetwork}{ActivityType.EditPin}",
                new HierarchicalLifetimeManager());

            Container.RegisterType<IJobProcess, FollowBackProcess>($"{socialNetwork}{ActivityType.FollowBack}",
                new HierarchicalLifetimeManager());

            Container.RegisterType<IJobProcess, FollowProcess>($"{socialNetwork}{ActivityType.Follow}",
                new HierarchicalLifetimeManager());

            Container.RegisterType<IJobProcess, PinScraperProcess>($"{socialNetwork}{ActivityType.PinScraper}",
                new HierarchicalLifetimeManager());

            Container.RegisterType<IJobProcess, RepinProcess>($"{socialNetwork}{ActivityType.Repin}",
                new HierarchicalLifetimeManager());

            Container.RegisterType<IJobProcess, SendBoardInvitationsProcess>(
                $"{socialNetwork}{ActivityType.SendBoardInvitation}",
                new HierarchicalLifetimeManager());

            Container.RegisterType<IJobProcess, SendMessageToNewFollowersProcess>(
                $"{socialNetwork}{ActivityType.SendMessageToFollower}",
                new HierarchicalLifetimeManager());

            Container.RegisterType<IJobProcess, TryProcess>($"{socialNetwork}{ActivityType.Try}",
                new HierarchicalLifetimeManager());

            Container.RegisterType<IJobProcess, UnfollowProcess>($"{socialNetwork}{ActivityType.Unfollow}",
                new HierarchicalLifetimeManager());

            Container.RegisterType<IJobProcess, UserScraperProcess>($"{socialNetwork}{ActivityType.UserScraper}",
                new HierarchicalLifetimeManager());

            Container.RegisterType<IJobProcess, CreateAccountProcess>($"{socialNetwork}{ActivityType.CreateAccount}",
                new HierarchicalLifetimeManager());
        }
    }
}