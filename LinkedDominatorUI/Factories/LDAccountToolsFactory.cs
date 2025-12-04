using System.Collections.Generic;
using System.Windows.Controls;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using LinkedDominatorUI.TabManager;

namespace LinkedDominatorUI.Factories
{
    public class LDAccountToolsFactory : IAccountToolsFactory
    {
        private static LDAccountToolsFactory _instance;

        private LDAccountToolsFactory()
        {
        }

        public static LDAccountToolsFactory Instance
            => _instance ?? (_instance = new LDAccountToolsFactory());

        public UserControl GetStartupToolsView()
        {
            return new ToolTabs();
        }

        public IEnumerable<ActivityType> GetImportantActivityTypes()
        {
            return new List<ActivityType>
            {
                ActivityType.ConnectionRequest,
                ActivityType.WithdrawConnectionRequest,
                ActivityType.GroupJoiner,
                ActivityType.GroupUnJoiner,
                ActivityType.SendMessageToNewConnection,
                ActivityType.ProfileEndorsement
            };
        }

        public IEnumerable<ActivityType> GetOtherActivityTypes()
        {
            return new List<ActivityType>
            {
                ActivityType.AcceptConnectionRequest,
                ActivityType.RemoveConnections,
                ActivityType.ExportConnection,
                ActivityType.FollowPages,
                ActivityType.BlockUser,
                ActivityType.BroadcastMessages,
                ActivityType.AutoReplyToNewMessage,
                ActivityType.SendGreetingsToConnections,
                ActivityType.DeleteConversations,
                ActivityType.Like,
                ActivityType.Comment,
                ActivityType.Share,
                ActivityType.GroupInviter,
                ActivityType.UserScraper,
                ActivityType.SalesNavigatorUserScraper,
                ActivityType.SalesNavigatorCompanyScraper,
                ActivityType.JobScraper,
                ActivityType.CompanyScraper,
                ActivityType.AttachmnetsMessageScraper
            };
        }

        public string RecentlySelectedAccount { get; set; } = string.Empty;
    }
}