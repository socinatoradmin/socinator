using System.Collections.Generic;
using System.Windows.Controls;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using QuoraDominatorUI.TabManager;

namespace QuoraDominatorUI.QDCoreLibrary
{
    public class QdAccountToolsFactory : IAccountToolsFactory
    {
        private static QdAccountToolsFactory _instance;

        private QdAccountToolsFactory()
        {
        }

        public static QdAccountToolsFactory Instance
            => _instance ?? (_instance = new QdAccountToolsFactory());

        public UserControl GetStartupToolsView()
        {
            return new ToolsTab();
        }

        public IEnumerable<ActivityType> GetImportantActivityTypes()
        {
            return new List<ActivityType>
            {
                ActivityType.Follow, ActivityType.Unfollow, ActivityType.UpvoteAnswers, ActivityType.DownvoteAnswers,
                ActivityType.ReportUsers, ActivityType.ReportQuestions
            };
        }

        public IEnumerable<ActivityType> GetOtherActivityTypes()
        {
            return new List<ActivityType>
            {
                ActivityType.UserScraper, ActivityType.QuestionsScraper, ActivityType.AnswersScraper,
                ActivityType.DownvoteQuestions, ActivityType.BroadcastMessages, ActivityType.AutoReplyToNewMessage,
                ActivityType.AnswerOnQuestions
            };
        }

        public string RecentlySelectedAccount { get; set; } = string.Empty;
    }
}