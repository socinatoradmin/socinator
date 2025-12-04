using DominatorHouseCore.Enums;
using DominatorHouseCore.Process;
using FaceDominatorCore.FDLibrary.FdProcesses;
using Unity;
using Unity.Extension;
using Unity.Lifetime;

namespace FaceDominatorCore.UnityContainers
{
    public class FdJobProcessUnityExtension : UnityContainerExtension
    {
        protected override void Initialize()
        {
            var socialNetwork = SocialNetworks.Facebook;
            Container.RegisterType<IJobProcess, CancelFriendRequestProcess>($"{socialNetwork}{ActivityType.WithdrawSentRequest}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, CommentLikerProcesss>($"{socialNetwork}{ActivityType.LikeComment}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, EventInviterProcess>($"{socialNetwork}{ActivityType.EventInviter}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, FdCommentScraperProcess>($"{socialNetwork}{ActivityType.CommentScraper}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, FdBroadCastMessageProcess>($"{socialNetwork}{ActivityType.BroadcastMessages}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, FdAutoReplyMessageProcess>($"{socialNetwork}{ActivityType.AutoReplyToNewMessage}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, FdFanpageLikerProcess>($"{socialNetwork}{ActivityType.FanpageLiker}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, FdFanpageScraperProcess>($"{socialNetwork}{ActivityType.FanpageScraper}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, FdGroupScraperProcess>($"{socialNetwork}{ActivityType.GroupScraper}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, FdIncommingFriendProcess>($"{socialNetwork}{ActivityType.IncommingFriendRequest}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, FdPostScraperProcess>($"{socialNetwork}{ActivityType.PostScraper}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, FdProfileScraperProcess>($"{socialNetwork}{ActivityType.ProfileScraper}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, FdReplyToCommentProcess>($"{socialNetwork}{ActivityType.ReplyToComment}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, FdWebpageLikerCommentorProcess>($"{socialNetwork}{ActivityType.WebpageLikerCommentor}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, GroupInviterProcess>($"{socialNetwork}{ActivityType.GroupInviter}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, GroupJoinerProcess>($"{socialNetwork}{ActivityType.GroupJoiner}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, GroupUnjoinerProecss>($"{socialNetwork}{ActivityType.GroupUnJoiner}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, PageInviterProcess>($"{socialNetwork}{ActivityType.PageInviter}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, PostCommentorProcess>($"{socialNetwork}{ActivityType.PostCommentor}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, PostLikerProcess>($"{socialNetwork}{ActivityType.PostLiker}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, SendFriendRequestProcess>($"{socialNetwork}{ActivityType.SendFriendRequest}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, SendGreetingsToFriendsProcess>($"{socialNetwork}{ActivityType.SendGreetingsToFriends}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, SendMessageToNewFriendsProcess>($"{socialNetwork}{ActivityType.SendMessageToNewFriends}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, UnfriendProcess>($"{socialNetwork}{ActivityType.Unfriend}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, UnfollowFriendProcess>($"{socialNetwork}{ActivityType.Unfollow}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, WatchPartyInviterProcss>($"{socialNetwork}{ActivityType.WatchPartyInviter}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, DownloadPhotosProcess>($"{socialNetwork}{ActivityType.DownloadScraper}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, MessageToFanpageProcess>($"{socialNetwork}{ActivityType.MessageToFanpages}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, MarketplaceScraperProcess>($"{socialNetwork}{ActivityType.MarketPlaceScraper}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, MessageToPlacesProcess>($"{socialNetwork}{ActivityType.MessageToPlaces}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, PlaceScraperProcess>($"{socialNetwork}{ActivityType.PlaceScraper}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, EventCreatorProcess>($"{socialNetwork}{ActivityType.EventCreator}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, MakeGroupAdminProcess>($"{socialNetwork}{ActivityType.MakeAdmin}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, CommentRepliesScraperProcess>($"{socialNetwork}{ActivityType.CommentRepliesScraper}",
                new HierarchicalLifetimeManager());
        }
    }
}
