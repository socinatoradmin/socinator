#region

using DominatorHouseCore.Interfaces.StartUp;

#endregion

namespace DominatorHouseCore.StartupActivity.Quora
{
    public class QuoraActivity : INetworkActivity
    {
        public BaseActivity GetActivity(string activity)
        {
            switch (activity)
            {
                case "Follow":
                    return new QuoraFollowActivity();
                case "BroadcastMessages":
                    return new QuoraBroadcastMessagesActivity();
                case "ReportUsers":
                case "UserScraper":
                    return new QuoraUserActivity();
                case "AnswerOnQuestions":
                case "QuestionsScraper":
                case "DownvoteQuestions":
                    return new QuoraQuestionActivity();
                default:
                    return new QuoraAnswerActivity();
            }
        }
    }
}