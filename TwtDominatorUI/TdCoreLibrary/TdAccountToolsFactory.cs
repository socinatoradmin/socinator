using System.Collections.Generic;
using System.Windows.Controls;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using TwtDominatorUI.TabManager;

namespace TwtDominatorUI.TdCoreLibrary
{
    public class TdAccountToolsFactory : IAccountToolsFactory
    {
        private static TdAccountToolsFactory _instance;


        private TdAccountToolsFactory()
        {
        }

        public static TdAccountToolsFactory Instance
            => _instance ?? (_instance = new TdAccountToolsFactory());

        public UserControl GetStartupToolsView()
        {
            return new ToolsTab();
        }

        public IEnumerable<ActivityType> GetImportantActivityTypes()
        {
            return new List<ActivityType>
            {
                ActivityType.Follow,
                ActivityType.Unfollow,
                ActivityType.Retweet,
                ActivityType.Comment,
                ActivityType.Like,
                ActivityType.BroadcastMessages
            };
        }

        public IEnumerable<ActivityType> GetOtherActivityTypes()
        {
            return new List<ActivityType>
            {
                ActivityType.FollowBack,
                ActivityType.Mute,
                ActivityType.TweetScraper,
                ActivityType.UserScraper,
                ActivityType.Reposter,
                ActivityType.Delete,
                ActivityType.AutoReplyToNewMessage,
                ActivityType.SendMessageToFollower,
                ActivityType.WelcomeTweet,
                ActivityType.TweetTo,
                ActivityType.Unlike
            };
        }

        public string RecentlySelectedAccount { get; set; } = string.Empty;
    }
}