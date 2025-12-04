using DominatorHouseCore.Enums;
using GramDominatorCore.Utility;
using GramDominatorUI.Utility.AddStory;
using GramDominatorUI.Utility.GrowFollowers.BlockFollower;
using GramDominatorUI.Utility.GrowFollowers.FollowBack;
using GramDominatorUI.Utility.GrowFollowers.Follower;
using GramDominatorUI.Utility.GrowFollowers.MakeCloseFriend;
using GramDominatorUI.Utility.GrowFollowers.Unfollower;
using GramDominatorUI.Utility.InstaChat.AutoReplyToNewMessage;
using GramDominatorUI.Utility.InstaChat.BroadcastMessages;
using GramDominatorUI.Utility.InstaChat.SendMessageToNewFollowers;
using GramDominatorUI.Utility.InstalikerCommenter.Comment;
using GramDominatorUI.Utility.InstalikerCommenter.Like;
using GramDominatorUI.Utility.InstalikerCommenter.LikeComments;
using GramDominatorUI.Utility.InstalikerCommenter.MediaUnliker;
using GramDominatorUI.Utility.InstalikerCommenter.ReplyComment;
using GramDominatorUI.Utility.InstaPoster.Delete;
using GramDominatorUI.Utility.InstaPoster.Reposter;
using GramDominatorUI.Utility.InstaScrape.CommentScraper;
using GramDominatorUI.Utility.InstaScrape.DownloadPhotos;
using GramDominatorUI.Utility.InstaScrape.HashtagsScraper;
using GramDominatorUI.Utility.InstaScrape.UserScraper;
using GramDominatorUI.Utility.StoryViews;

namespace GramDominatorUI.GDCoreLibrary
{
    public class GdInitialiser
    {
        public static void RegisterModules()
        {
            InstagramInitialize.GdModulesRegister(ActivityType.Follow, new FollowerBaseFactory());
            InstagramInitialize.GdModulesRegister(ActivityType.Unfollow, new UnfollowerBaseFactory());
            InstagramInitialize.GdModulesRegister(ActivityType.CloseFriend, new CloseFriendBaseFactory());
            InstagramInitialize.GdModulesRegister(ActivityType.FollowBack, new FollowBackBaseFactory());
            InstagramInitialize.GdModulesRegister(ActivityType.BlockFollower, new BlockFollowerBaseFactory());
            InstagramInitialize.GdModulesRegister(ActivityType.BroadcastMessages, new BroadcastMessagesBaseFactory());
            InstagramInitialize.GdModulesRegister(ActivityType.AutoReplyToNewMessage,
                new AutoReplyToNewMessageBaseFactory());
            InstagramInitialize.GdModulesRegister(ActivityType.SendMessageToFollower,
                new SendMessageToNewFollowerBaseFactory());
            InstagramInitialize.GdModulesRegister(ActivityType.Like, new LikeBaseFactory());
            InstagramInitialize.GdModulesRegister(ActivityType.Comment, new CommentBaseFactory());
            InstagramInitialize.GdModulesRegister(ActivityType.Unlike, new MediaUnlikerBaseFactory());
            InstagramInitialize.GdModulesRegister(ActivityType.Reposter, new ReposterBaseFactory());
            InstagramInitialize.GdModulesRegister(ActivityType.DeletePost, new DeleteBaseFactory());
            InstagramInitialize.GdModulesRegister(ActivityType.UserScraper, new UserScraperBaseFactory());
            InstagramInitialize.GdModulesRegister(ActivityType.HashtagsScraper, new HashtagsScraperBaseFactory());
            InstagramInitialize.GdModulesRegister(ActivityType.PostScraper, new DownloadPhotosBaseFactory());
            InstagramInitialize.GdModulesRegister(ActivityType.LikeComment, new LikeCommentBaseFactory());
            InstagramInitialize.GdModulesRegister(ActivityType.CommentScraper, new CommentScraperBaseFactory());
            InstagramInitialize.GdModulesRegister(ActivityType.StoryViewer, new StoryViewBaseFactory());
            InstagramInitialize.GdModulesRegister(ActivityType.AddStory, new AddStoryViewBaseFactory());
            InstagramInitialize.GdModulesRegister(ActivityType.ReplyToComment, new ReplyCommentBaseFactory());
        }
    }
}