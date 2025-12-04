using DominatorHouseCore.Enums;
using TumblrDominatorCore.TmblrUtility;
using TumblrDominatorUI.Utility.Comment;
using TumblrDominatorUI.Utility.CommentScraper;
using TumblrDominatorUI.Utility.Follow;
using TumblrDominatorUI.Utility.Like;
using TumblrDominatorUI.Utility.Message;
using TumblrDominatorUI.Utility.PostScraper;
using TumblrDominatorUI.Utility.Reblog;
using TumblrDominatorUI.Utility.Unfollow;
using TumblrDominatorUI.Utility.Unlike;
using TumblrDominatorUI.Utility.UserScraper;

namespace TumblrDominatorUI.TumblrCoreLibrary
{
    public class TumblrInitialiser
    {
        public static void RegisterModules()
        {
            TumblrInitialize.TumblrModulesRegister(ActivityType.Follow, new FollowBaseFactory());
            TumblrInitialize.TumblrModulesRegister(ActivityType.Like, new LikeBaseFactory());
            TumblrInitialize.TumblrModulesRegister(ActivityType.Reblog, new ReblogBaseFactory());
            TumblrInitialize.TumblrModulesRegister(ActivityType.Unfollow, new UnfollowBaseFactory());
            TumblrInitialize.TumblrModulesRegister(ActivityType.Comment, new CommentBaseFactory());
            TumblrInitialize.TumblrModulesRegister(ActivityType.BroadcastMessages, new MessageBaseFactory());
            TumblrInitialize.TumblrModulesRegister(ActivityType.PostScraper, new PostScraperBaseFactory());
            TumblrInitialize.TumblrModulesRegister(ActivityType.UserScraper, new UserScraperBaseFactory());
            TumblrInitialize.TumblrModulesRegister(ActivityType.CommentScraper, new CommentScraperBaseFactory());
            TumblrInitialize.TumblrModulesRegister(ActivityType.Unlike, new UnLikeBaseFactory());
        }
    }
}