using DominatorHouseCore.Enums;
using TwtDominatorCore.TDUtility;
using TwtDominatorUI.Utility.GrowFollowersReportPack.FollowBackPack;
using TwtDominatorUI.Utility.GrowFollowersReportPack.FollowerPack;
using TwtDominatorUI.Utility.GrowFollowersReportPack.MuteUsersPack;
using TwtDominatorUI.Utility.GrowFollowersReportPack.UnfollowerPack;
using TwtDominatorUI.Utility.ScraperReportPack.ScrapeTweetPack;
using TwtDominatorUI.Utility.ScraperReportPack.ScrapeUserPack;
using TwtDominatorUI.Utility.TwtBlasterReportPack.DeletePack;
using TwtDominatorUI.Utility.TwtBlasterReportPack.ReposterPack;
using TwtDominatorUI.Utility.TwtBlasterReportPack.RetweetPack;
using TwtDominatorUI.Utility.TwtBlasterReportPack.TweetToPack;
using TwtDominatorUI.Utility.TwtBlasterReportPack.WelcomeTweetPack;
using TwtDominatorUI.Utility.TwtEngageReportPack.CommentPack;
using TwtDominatorUI.Utility.TwtEngageReportPack.TwtLikerPack;
using TwtDominatorUI.Utility.TwtEngageReportPack.TwtUnLikerPack;
using TwtDominatorUI.Utility.TwtMessengerReportPack.AutoReplyPack;
using TwtDominatorUI.Utility.TwtMessengerReportPack.BroadCastMessagePack;
using TwtDominatorUI.Utility.TwtMessengerReportPack.ReplyToNewFollowersPack;

namespace TwtDominatorUI.TdCoreLibrary
{
    public class TDInitialiser
    {
        public static void RegisterModules()
        {
            TDInitialise.TDModulesRegister(ActivityType.FollowBack, new FollowBackBaseFactory());
            TDInitialise.TDModulesRegister(ActivityType.Follow, new FollowerBaseFactory());
            TDInitialise.TDModulesRegister(ActivityType.Unfollow, new UnfollowerBaseFactory());
            TDInitialise.TDModulesRegister(ActivityType.Mute, new MuteUsersBaseFactory());
            TDInitialise.TDModulesRegister(ActivityType.TweetScraper, new ScrapeTweetBaseFactory());
            TDInitialise.TDModulesRegister(ActivityType.UserScraper, new ScrapeUserBaseFactory());
            TDInitialise.TDModulesRegister(ActivityType.Reposter, new ReposterBaseFactory());
            TDInitialise.TDModulesRegister(ActivityType.Retweet, new RetweetBaseFactory());
            TDInitialise.TDModulesRegister(ActivityType.Delete, new DeleteBaseFactory());
            TDInitialise.TDModulesRegister(ActivityType.Comment, new CommentBaseFactory());
            TDInitialise.TDModulesRegister(ActivityType.Like, new TwtLikerBaseFactory());
            TDInitialise.TDModulesRegister(ActivityType.AutoReplyToNewMessage, new AutoReplyBaseFactory());
            TDInitialise.TDModulesRegister(ActivityType.BroadcastMessages, new BroadCastMessageBaseFactory());
            //TDInitialise.TDModulesRegister(ActivityType.SendMessageToFollower, new DirectMessageBaseFactory());
            TDInitialise.TDModulesRegister(ActivityType.SendMessageToFollower, new ReplyToNewFollowersBaseFactory());
            TDInitialise.TDModulesRegister(ActivityType.WelcomeTweet, new WelcomeTweetBaseFactory());
            TDInitialise.TDModulesRegister(ActivityType.TweetTo, new TweetToBaseFactory());
            TDInitialise.TDModulesRegister(ActivityType.Unlike, new TwtUnLikerBaseFactory());
        }
    }
}