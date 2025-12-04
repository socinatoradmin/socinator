using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using System.Collections.Generic;
using System.Windows.Controls;
using YoutubeDominatorUI.TabManager;

namespace YoutubeDominatorUI.YdCoreLibrary
{
    public class YdAccountToolsFactory : IAccountToolsFactory
    {
        private static YdAccountToolsFactory _instance;

        private YdAccountToolsFactory()
        {
        }

        public static YdAccountToolsFactory Instance
            => _instance ?? (_instance = new YdAccountToolsFactory());

        public string RecentlySelectedAccount { get; set; } = string.Empty;

        public UserControl GetStartupToolsView()
        {
            return new ToolsTab();
        }

        public IEnumerable<ActivityType> GetImportantActivityTypes()
        {
            return new List<ActivityType>
            {
                ActivityType.ViewIncreaser, ActivityType.Like, ActivityType.Comment, ActivityType.LikeComment,
                ActivityType.Dislike, ActivityType.Subscribe
            };
        }

        public IEnumerable<ActivityType> GetOtherActivityTypes()
        {
            return new List<ActivityType>
            {
                ActivityType.UnSubscribe, ActivityType.ChannelScraper, ActivityType.PostScraper,
                ActivityType.ReportVideo, ActivityType.CommentScraper
            };
        }
    }
}