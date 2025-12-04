using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using RedditDominatorUI.TabManager;
using System.Collections.Generic;
using System.Windows.Controls;

namespace RedditDominatorUI.RdCoreLibrary
{
    public class RdAccountToolsFactory : IAccountToolsFactory
    {
        private static RdAccountToolsFactory _instance;

        private RdAccountToolsFactory()
        {
        }

        public static RdAccountToolsFactory Instance
            => _instance ?? (_instance = new RdAccountToolsFactory());

        public string RecentlySelectedAccount { get; set; } = string.Empty;

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
                ActivityType.Comment,
                ActivityType.Reply,
                ActivityType.Upvote,
                ActivityType.Downvote
            };
        }

        public IEnumerable<ActivityType> GetOtherActivityTypes()
        {
            return new List<ActivityType>
            {
                ActivityType.RemoveVote,
                ActivityType.Subscribe,
                ActivityType.UnSubscribe,
                ActivityType.UrlScraper,
                ActivityType.UserScraper,
                ActivityType.EditComment,
                ActivityType.BroadcastMessages,
                ActivityType.UpvoteComment,
                ActivityType.DownvoteComment,
                ActivityType.RemoveVoteComment,
                ActivityType.ChannelScraper,
                ActivityType.CommentScraper
            };
        }
    }
}