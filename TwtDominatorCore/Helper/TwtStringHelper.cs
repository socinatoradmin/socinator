using System.Linq;
using DominatorHouseCore.Enums;

namespace TwtDominatorCore.Helper
{
    public class TwtStringHelper
    {
        public static bool IsMultiEqual(ActivityType ActivitiesComparer, params ActivityType[] ActivitiesComparing)
        {
            return ActivitiesComparing.Contains(ActivitiesComparer);
        }

        public static bool IsUserProcessor(ActivityType ActivityType)
        {
            return IsMultiEqual(ActivityType, ActivityType.Follow, ActivityType.UserScraper, ActivityType.Mute,
                ActivityType.TweetTo);
        }

        public static bool IsUserProcessorAccountReport(ActivityType ActivityType)
        {
            return IsMultiEqual(ActivityType, ActivityType.Follow, ActivityType.UserScraper, ActivityType.Mute,
                ActivityType.TweetTo, ActivityType.FollowBack, ActivityType.BroadcastMessages,
                ActivityType.SendMessageToFollower, ActivityType.Unlike, ActivityType.AutoReplyToNewMessage,
                ActivityType.WelcomeTweet);
        }
    }
}