using DominatorHouseCore.Enums;
using DominatorHouseCore.Process;
using RedditDominatorCore.RDLibrary.Actions;
using Unity;
using Unity.Extension;
using Unity.Lifetime;

namespace RedditDominatorCore.RDLibrary
{
    public class RedditJobProcessUnityExtension : UnityContainerExtension
    {
        protected override void Initialize()
        {
            var socialNetwork = SocialNetworks.Reddit;

            Container.RegisterType<IJobProcess, UpVoteProcess>($"{socialNetwork}{ActivityType.Upvote}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, UpVoteProcess>($"{socialNetwork}{ActivityType.UpvoteComment}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, UrlScraperProcess>($"{socialNetwork}{ActivityType.UrlScraper}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, UserScraperProcess>($"{socialNetwork}{ActivityType.UserScraper}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, FollowProcess>($"{socialNetwork}{ActivityType.Follow}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, UnFollowProcess>($"{socialNetwork}{ActivityType.Unfollow}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, DownVoteProcess>($"{socialNetwork}{ActivityType.Downvote}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, RemoveVoteProcess>($"{socialNetwork}{ActivityType.RemoveVote}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, SubscribeProcess>($"{socialNetwork}{ActivityType.Subscribe}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, UnSubscribeProcess>($"{socialNetwork}{ActivityType.UnSubscribe}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, CommentProcess>($"{socialNetwork}{ActivityType.Comment}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, ReplyProcess>($"{socialNetwork}{ActivityType.Reply}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, EditCommentProcess>($"{socialNetwork}{ActivityType.EditComment}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, BroadcastMessageProcess>($"{socialNetwork}{ActivityType.BroadcastMessages}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, BroadcastMessageProcess>($"{socialNetwork}{ActivityType.AutoReplyToNewMessage}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, DownVoteProcess>($"{socialNetwork}{ActivityType.DownvoteComment}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, RemoveVoteProcess>($"{socialNetwork}{ActivityType.RemoveVoteComment}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, ChannelScraperProcess>($"{socialNetwork}{ActivityType.ChannelScraper}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, CommentScraperProcess>($"{socialNetwork}{ActivityType.CommentScraper}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, PostAutoActivityProcess>($"{socialNetwork}{ActivityType.AutoActivity}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IRdAdScrapperProcess, RdAdScrapperProcess>(new HierarchicalLifetimeManager());
        }
    }
}