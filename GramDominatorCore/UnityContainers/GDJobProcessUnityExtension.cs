using DominatorHouseCore.Enums;
using DominatorHouseCore.Process;
using GramDominatorCore.GDLibrary;
using GramDominatorCore.GDLibrary.Process;
using Unity;
using Unity.Extension;
using Unity.Lifetime;

namespace GramDominatorCore.UnityContainers
{
    public class GDJobProcessUnityExtension : UnityContainerExtension
    {
        protected override void Initialize()
        {
            var socialNetwork = SocialNetworks.Instagram;
            Container.RegisterType<IJobProcess, FollowProcess>($"{socialNetwork}{ActivityType.Follow}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, UnFollowProcess>($"{socialNetwork}{ActivityType.Unfollow}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, CloseFriendProcess>($"{socialNetwork}{ActivityType.CloseFriend}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, BlockFollowerProcess>($"{socialNetwork}{ActivityType.BlockFollower}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, FollowBackProcess>($"{socialNetwork}{ActivityType.FollowBack}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, ReposterProcess>($"{socialNetwork}{ActivityType.Reposter}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, DeletePostProcess>($"{socialNetwork}{ActivityType.DeletePost}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, BroadcastMessageProcess>($"{socialNetwork}{ActivityType.BroadcastMessages}",
                new HierarchicalLifetimeManager());

            Container.RegisterType<IJobProcess, SendMessageToNewFollowersProcess>($"{socialNetwork}{ActivityType.SendMessageToFollower}", new HierarchicalLifetimeManager());

            Container.RegisterType<IJobProcess, AutoReplyToNewMessagesProcess>($"{socialNetwork}{ActivityType.AutoReplyToNewMessage}",new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, LikeProcess>($"{socialNetwork}{ActivityType.Like}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, CommentProcess>($"{socialNetwork}{ActivityType.Comment}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, LikeCommentProcess>($"{socialNetwork}{ActivityType.LikeComment}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, MediaUnlikeProcess>($"{socialNetwork}{ActivityType.Unlike}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, UserScrapeProcess>($"{socialNetwork}{ActivityType.UserScraper}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, DownloadPhotoProcess>($"{socialNetwork}{ActivityType.PostScraper}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, HashtagScrapeProcess>($"{socialNetwork}{ActivityType.HashtagsScraper}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, CommentScraperProcess>($"{socialNetwork}{ActivityType.CommentScraper}", new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, StoryProcess>($"{socialNetwork}{ActivityType.StoryViewer}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, ReplyCommentProcess>($"{socialNetwork}{ActivityType.ReplyToComment}",
               new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, AddStoryProcess>($"{socialNetwork}{ActivityType.AddStory}",
              new HierarchicalLifetimeManager());
        }
    }
}
