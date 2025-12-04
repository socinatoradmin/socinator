using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using FaceDominatorUI.FDViews.TabManager;
using System.Collections.Generic;
using System.Windows.Controls;

namespace FaceDominatorUI.FdCoreLibrary
{
    public class FdAccountToolsFactory : IAccountToolsFactory
    {
        private static FdAccountToolsFactory _instance;


        private FdAccountToolsFactory()
        {
        }

        public static FdAccountToolsFactory Instance
            => _instance ?? (_instance = new FdAccountToolsFactory());

        public string RecentlySelectedAccount { get; set; } = string.Empty;

        public UserControl GetStartupToolsView()
        {
            return new ToolsTabs();
        }

        public IEnumerable<ActivityType> GetImportantActivityTypes()
        {
            return new List<ActivityType>
            {
                ActivityType.SendFriendRequest,
                ActivityType.Unfriend,
                ActivityType.PostLiker,
                ActivityType.PostCommentor,
                ActivityType.GroupJoiner,
                ActivityType.BroadcastMessages
            };
        }

        public IEnumerable<ActivityType> GetOtherActivityTypes()
        {
            return new List<ActivityType>
            {
                ActivityType.IncommingFriendRequest,
                ActivityType.GroupUnJoiner,
                ActivityType.FanpageLiker,
                ActivityType.PostLikerCommentor,
                ActivityType.ProfileScraper,
                ActivityType.FanpageScraper,
                ActivityType.GroupScraper,
                ActivityType.CommentScraper,
                ActivityType.PostScraper,
                ActivityType.WithdrawSentRequest,
                ActivityType.AutoReplyToNewMessage,
                ActivityType.GroupInviter,
                ActivityType.PageInviter,
                ActivityType.LikeComment,
                ActivityType.ReplyToComment,
                ActivityType.DownloadScraper,
                ActivityType.EventInviter,
                ActivityType.SendMessageToNewFriends,
                ActivityType.WatchPartyInviter,
                ActivityType.SendGreetingsToFriends,
                ActivityType.MessageToFanpages,
                ActivityType.MessageToPlaces,
                ActivityType.PlaceScraper,
                ActivityType.EventCreator,
                ActivityType.MakeAdmin,
                ActivityType.CommentRepliesScraper
            };
        }
    }
}