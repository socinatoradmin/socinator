using DominatorHouseCore.Enums;
using DominatorHouseCore.Process;
using TumblrDominatorCore.TumblrLibrary.TumblrProcesses;
using Unity;
using Unity.Extension;
using Unity.Lifetime;

namespace TumblrDominatorCore.UnityContainers
{
    public class TumblrJobProcessUnityExtension : UnityContainerExtension
    {
        protected override void Initialize()
        {
            var socialNetwork = SocialNetworks.Tumblr;
            Container.RegisterType<IJobProcess, FollowJobProcess>($"{socialNetwork}{ActivityType.Follow}",
                new HierarchicalLifetimeManager());

            Container.RegisterType<IJobProcess, BroadCastMessegeProcess>(
                $"{socialNetwork}{ActivityType.BroadcastMessages}",
                new HierarchicalLifetimeManager());

            Container.RegisterType<IJobProcess, UnFollowJobProcess>($"{socialNetwork}{ActivityType.Unfollow}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, LikeJobProcess>($"{socialNetwork}{ActivityType.Like}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, CommentProcess>($"{socialNetwork}{ActivityType.Comment}",
                new HierarchicalLifetimeManager());

            Container.RegisterType<IJobProcess, ReblogJobProcess>($"{socialNetwork}{ActivityType.Reblog}",
                new HierarchicalLifetimeManager());

            Container.RegisterType<IJobProcess, PostScraperJobProcess>($"{socialNetwork}{ActivityType.PostScraper}",
                new HierarchicalLifetimeManager());

            Container.RegisterType<IJobProcess, UserScraperJobProcess>($"{socialNetwork}{ActivityType.UserScraper}",
                new HierarchicalLifetimeManager());

            Container.RegisterType<IJobProcess, CommentScraperJobProcess>(
                $"{socialNetwork}{ActivityType.CommentScraper}",
                new HierarchicalLifetimeManager());

            Container.RegisterType<IJobProcess, UnLikeJobProcess>($"{socialNetwork}{ActivityType.Unlike}",
                new HierarchicalLifetimeManager());
        }
    }
}