using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using System.Collections.Generic;
using System.Windows.Controls;
using TumblrDominatorUI.TabManager;

namespace TumblrDominatorUI.TumblrCoreLibrary
{
    public class TumblrAccountToolsFactory : IAccountToolsFactory
    {
        private static TumblrAccountToolsFactory _instance;

        private TumblrAccountToolsFactory()
        {
        }

        public static TumblrAccountToolsFactory Instance
            => _instance ?? (_instance = new TumblrAccountToolsFactory());

        public UserControl GetStartupToolsView()
        {
            return ToolsTab.GetSingletonToolTabs();
        }

        public IEnumerable<ActivityType> GetImportantActivityTypes()
        {
            return new List<ActivityType>
            {
                ActivityType.Follow, ActivityType.Unfollow, ActivityType.Like, ActivityType.Reblog,
                ActivityType.Comment, ActivityType.BroadcastMessages
            };
        }

        public IEnumerable<ActivityType> GetOtherActivityTypes()
        {
            return new List<ActivityType>
                {ActivityType.PostScraper, ActivityType.UserScraper, ActivityType.CommentScraper, ActivityType.Unlike};
        }

        public string RecentlySelectedAccount { get; set; }
    }
}