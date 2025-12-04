using System.Collections.Generic;
using System.Windows.Controls;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using GramDominatorUI.TabManager;

namespace GramDominatorUI.GDCoreLibrary
{
    public class GdAccountToolsFactory : IAccountToolsFactory
    {
        private static GdAccountToolsFactory _instance;

        private GdAccountToolsFactory()
        {
        }

        public static GdAccountToolsFactory Instance
            => _instance ?? (_instance = new GdAccountToolsFactory());

        public UserControl GetStartupToolsView()
        {
            return new ToolTabs();
        }

        public IEnumerable<ActivityType> GetImportantActivityTypes()
        {
            return new List<ActivityType>
            {
                ActivityType.Follow, ActivityType.Unfollow, ActivityType.Like, ActivityType.Comment,
                ActivityType.BroadcastMessages, ActivityType.Reposter
            };
        }

        public IEnumerable<ActivityType> GetOtherActivityTypes()
        {
            return new List<ActivityType>
            {
                ActivityType.FollowBack,
                ActivityType.BlockFollower,
                ActivityType.AutoReplyToNewMessage,
                ActivityType.SendMessageToFollower,
                ActivityType.Unlike,
                ActivityType.DeletePost,
                ActivityType.UserScraper,
                ActivityType.HashtagsScraper,
                ActivityType.PostScraper,
                ActivityType.LikeComment,
                ActivityType.CommentScraper,
                ActivityType.StoryViewer,
                ActivityType.ReplyToComment
            };
        }

        public string RecentlySelectedAccount { get; set; } = string.Empty;
    }
}