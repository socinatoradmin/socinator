using DominatorHouseCore.Enums;
using YoutubeDominatorCore.YDUtility;
using YoutubeDominatorUI.Utility.ChannelScraperUtility;
using YoutubeDominatorUI.Utility.CommentScraperUtility;
using YoutubeDominatorUI.Utility.CommentUtility;
using YoutubeDominatorUI.Utility.DisikeUtility;
using YoutubeDominatorUI.Utility.LikeCommentUtility;
using YoutubeDominatorUI.Utility.LikeUtility;
using YoutubeDominatorUI.Utility.PostScraperUtility;
using YoutubeDominatorUI.Utility.ReportVideoUtility;
using YoutubeDominatorUI.Utility.SubscribeUtility;
using YoutubeDominatorUI.Utility.UnsubscribeUtility;
using YoutubeDominatorUI.Utility.ViewIncreaserUtility;

namespace YoutubeDominatorUI.YdCoreLibrary
{
    public class YdInitialiser
    {
        public static void RegisterModules()
        {
            YoutubeInitialize.YdModulesRegister(ActivityType.Like, new LikeBaseFactory());
            YoutubeInitialize.YdModulesRegister(ActivityType.Dislike, new DislikeBaseFactory());
            YoutubeInitialize.YdModulesRegister(ActivityType.Comment, new CommentBaseFactory());
            YoutubeInitialize.YdModulesRegister(ActivityType.LikeComment, new LikeCommentBaseFactory());
            YoutubeInitialize.YdModulesRegister(ActivityType.Subscribe, new SubscribeBaseFactory());
            YoutubeInitialize.YdModulesRegister(ActivityType.UnSubscribe, new UnsubscribeBaseFactory());
            YoutubeInitialize.YdModulesRegister(ActivityType.PostScraper, new PostScraperBaseFactory());
            YoutubeInitialize.YdModulesRegister(ActivityType.ChannelScraper, new ChannelScraperBaseFactory());
            YoutubeInitialize.YdModulesRegister(ActivityType.ViewIncreaser, new ViewIncreaserBaseFactory());
            YoutubeInitialize.YdModulesRegister(ActivityType.ReportVideo, new ReportVideoBaseFactory());
            YoutubeInitialize.YdModulesRegister(ActivityType.CommentScraper, new CommentScraperBaseFactory());
        }
    }
}