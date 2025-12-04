using System.Collections.Generic;
using System.Windows.Controls;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using PinDominator.TabManager;

namespace PinDominator.PdCoreLibrary
{
    public class PdAccountToolsFactory : IAccountToolsFactory
    {
        private static PdAccountToolsFactory _instance;

        private PdAccountToolsFactory()
        {
        }

        public static PdAccountToolsFactory Instance
            => _instance ?? (_instance = new PdAccountToolsFactory());

        public string RecentlySelectedAccount { get; set; }

        public UserControl GetStartupToolsView()
        {
            return new ToolTabs();
        }

        public IEnumerable<ActivityType> GetImportantActivityTypes()
        {
            return new List<ActivityType>
            {
                ActivityType.Follow,
                ActivityType.Unfollow,
                //ActivityType.Try,
                ActivityType.Comment,
                ActivityType.CreateBoard,
                ActivityType.FollowBack
            };
        }

        public IEnumerable<ActivityType> GetOtherActivityTypes()
        {
            return new List<ActivityType>
            {
                ActivityType.AcceptBoardInvitation,
                ActivityType.SendBoardInvitation,
                ActivityType.Repin,
                ActivityType.DeletePin,
                ActivityType.EditPin,
                ActivityType.BroadcastMessages,
                ActivityType.AutoReplyToNewMessage,
                ActivityType.SendMessageToFollower,
                ActivityType.UserScraper,
                ActivityType.BoardScraper,
                ActivityType.PinScraper
            };
        }
    }
}