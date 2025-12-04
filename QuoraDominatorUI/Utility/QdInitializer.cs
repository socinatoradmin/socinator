using DominatorHouseCore.Enums;
using QuoraDominatorCore.QdUtility;
using QuoraDominatorUI.Factories.GrowFollower;
using QuoraDominatorUI.Factories.Messages;
using QuoraDominatorUI.Factories.Reports;
using QuoraDominatorUI.Factories.Scrape;
using QuoraDominatorUI.Factories.Voting;

namespace QuoraDominatorUI.Utility
{
    public class QdInitializer
    {
        public static void RegisterModules()
        {
            QuoraInitialize.QdModulesRegister(ActivityType.Follow, new FollowFactory());
            QuoraInitialize.QdModulesRegister(ActivityType.Unfollow, new UnfollowFactory());
            QuoraInitialize.QdModulesRegister(ActivityType.ReportAnswers, new ReportAnswersFactory());
            QuoraInitialize.QdModulesRegister(ActivityType.ReportUsers, new ReportUsersFactory());
            QuoraInitialize.QdModulesRegister(ActivityType.AutoReplyToNewMessage, new AutoReplyToNewMessageFactory());
            QuoraInitialize.QdModulesRegister(ActivityType.SendMessageToFollower, new SendMessageToFollowerFactory());
            QuoraInitialize.QdModulesRegister(ActivityType.BroadcastMessages, new BroadcastMessagesFactory());
            QuoraInitialize.QdModulesRegister(ActivityType.UserScraper, new UserScraperFactory());
            QuoraInitialize.QdModulesRegister(ActivityType.AnswersScraper, new AnswersScraperFactory());
            QuoraInitialize.QdModulesRegister(ActivityType.QuestionsScraper, new QuestionsScraperFactory());
            QuoraInitialize.QdModulesRegister(ActivityType.UpvoteAnswers, new UpvoteAnswersFactory());
            QuoraInitialize.QdModulesRegister(ActivityType.DownvoteQuestions, new DownvoteQuestionsFactory());
            QuoraInitialize.QdModulesRegister(ActivityType.DownvoteAnswers, new DownvoteAnswersFactory());
            QuoraInitialize.QdModulesRegister(ActivityType.AnswerOnQuestions, new AnswerOnQuestionFactory());
            QuoraInitialize.QdModulesRegister(ActivityType.UpvotePost, new UpvotePostsFactory());
            QuoraInitialize.QdModulesRegister(ActivityType.DownVotePost, new DownvotePostFactory());
        }
    }
}