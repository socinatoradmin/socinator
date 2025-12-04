using DominatorHouseCore.Enums;
using DominatorHouseCore.Process;
using TwtDominatorCore.TDLibrary.GeneralLibrary.Subprocesses;
using TwtDominatorCore.TDLibrary.TwtPosterLibrary;
using Unity;
using Unity.Extension;
using Unity.Lifetime;

namespace TwtDominatorCore.TDLibrary
{
    public class TwitterJobProcessUnityExtension : UnityContainerExtension
    {
        protected override void Initialize()
        {
            Container.AddNewExtension<TdSubprocessContainerExtension>();

            var socialNetwork = SocialNetworks.Twitter;
            Container.RegisterType<IJobProcess, FollowProcess>($"{socialNetwork}{ActivityType.Follow}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, FollowProcess>($"{socialNetwork}{ActivityType.FollowBack}",
                new HierarchicalLifetimeManager());

            Container.RegisterType<IJobProcess, MessageProcess>($"{socialNetwork}{ActivityType.BroadcastMessages}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, MessageProcess>($"{socialNetwork}{ActivityType.AutoReplyToNewMessage}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, MessageProcess>($"{socialNetwork}{ActivityType.SendMessageToFollower}",
                new HierarchicalLifetimeManager());

            Container.RegisterType<IJobProcess, MuteProcess>($"{socialNetwork}{ActivityType.Mute}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, Unfollowprocess>($"{socialNetwork}{ActivityType.Unfollow}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, LikeProcess>($"{socialNetwork}{ActivityType.Like}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, CommentProcess>($"{socialNetwork}{ActivityType.Comment}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, DeleteProcess>($"{socialNetwork}{ActivityType.Delete}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, ScrapeUserProcess>($"{socialNetwork}{ActivityType.UserScraper}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, ReposterProcess>($"{socialNetwork}{ActivityType.Reposter}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, RetweetProcess>($"{socialNetwork}{ActivityType.Retweet}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, ScrapeTweetProcess>($"{socialNetwork}{ActivityType.TweetScraper}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, DownloadProcess>($"{socialNetwork}{ActivityType.DownloadScraper}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, WelcomeTweetProcess>($"{socialNetwork}{ActivityType.WelcomeTweet}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, UnLikeProcess>($"{socialNetwork}{ActivityType.Unlike}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, TweetToProcess>($"{socialNetwork}{ActivityType.TweetTo}",
                new HierarchicalLifetimeManager());
        }
    }
}