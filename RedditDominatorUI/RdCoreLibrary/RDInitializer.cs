using DominatorHouseCore.Enums;
using RedditDominatorCore.Utility;
using RedditDominatorUI.Utility.AutoActivity;
using RedditDominatorUI.Utility.Engage;
using RedditDominatorUI.Utility.GrowFollowers;
using RedditDominatorUI.Utility.Messanger;
using RedditDominatorUI.Utility.Scraper;
using RedditDominatorUI.Utility.SubscriberChannels;
using RedditDominatorUI.Utility.Voting;

namespace RedditDominatorUI.RdCoreLibrary
{
    public class RdInitializer
    {
        public static void RegisterModules()
        {
            RedditInitialize.RdModulesRegister(ActivityType.Follow, new FollowBaseFactory());
            RedditInitialize.RdModulesRegister(ActivityType.Unfollow, new UnFollowBaseFactory());
            RedditInitialize.RdModulesRegister(ActivityType.Upvote, new UpVoteBaseFactory());
            RedditInitialize.RdModulesRegister(ActivityType.Downvote, new DownVotingBaseFactory());
            RedditInitialize.RdModulesRegister(ActivityType.RemoveVote, new RemoveVotingBaseFactory());
            RedditInitialize.RdModulesRegister(ActivityType.Subscribe, new SubscribeBaseFactory());
            RedditInitialize.RdModulesRegister(ActivityType.UnSubscribe, new UnSubscriberBaseFactory());
            RedditInitialize.RdModulesRegister(ActivityType.UrlScraper, new UrlScraperBaseFactory());
            RedditInitialize.RdModulesRegister(ActivityType.UserScraper, new UserScraperBaseFactory());
            RedditInitialize.RdModulesRegister(ActivityType.Comment, new CommentBaseFactory());
            RedditInitialize.RdModulesRegister(ActivityType.Reply, new ReplyBaseFactory());
            RedditInitialize.RdModulesRegister(ActivityType.EditComment, new EditCommentBaseFactory());
            RedditInitialize.RdModulesRegister(ActivityType.BroadcastMessages, new BroadcastMessageBaseFactory());
            RedditInitialize.RdModulesRegister(ActivityType.AutoReplyToNewMessage, new AutoReplyBaseFactory());
            RedditInitialize.RdModulesRegister(ActivityType.UpvoteComment, new UpVoteCommentBaseFactory());
            RedditInitialize.RdModulesRegister(ActivityType.DownvoteComment, new DownVoteCommentBaseFactory());
            RedditInitialize.RdModulesRegister(ActivityType.RemoveVoteComment, new RemoveVoteCommentBaseFactory());
            RedditInitialize.RdModulesRegister(ActivityType.ChannelScraper, new ChannelScraperBaseFactory());
            RedditInitialize.RdModulesRegister(ActivityType.CommentScraper, new CommentScraperBaseFactory());
            RedditInitialize.RdModulesRegister(ActivityType.AutoActivity, new AutoActivityBaseFactory());
        }
    }
}