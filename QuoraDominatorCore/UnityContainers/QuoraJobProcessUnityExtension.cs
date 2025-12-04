using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Process;
using QuoraDominatorCore.QdLibrary;
using QuoraDominatorCore.QdLibrary.QdFunctions;
using Unity;
using Unity.Extension;
using Unity.Lifetime;

namespace QuoraDominatorCore.UnityContainers
{
    public class QuoraJobProcessUnityExtension : UnityContainerExtension
    {
        protected override void Initialize()
        {
            var socialNetwork = SocialNetworks.Quora;
            Container.RegisterType<IJobProcess, FollowProcess>($"{socialNetwork}{ActivityType.Follow}",
                new HierarchicalLifetimeManager());

            Container.RegisterType<IJobProcess, BroadCastMessageProcess>(
                $"{socialNetwork}{ActivityType.BroadcastMessages}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, BroadCastMessageProcess>(
                $"{socialNetwork}{ActivityType.AutoReplyToNewMessage}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, BroadCastMessageProcess>(
                $"{socialNetwork}{ActivityType.SendMessageToFollower}",
                new HierarchicalLifetimeManager());

            Container.RegisterType<IJobProcess, UnfollowProcess>($"{socialNetwork}{ActivityType.Unfollow}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, AnswerOnQuestionProcess>(
                $"{socialNetwork}{ActivityType.AnswerOnQuestions}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, AnswerScraperProcess>($"{socialNetwork}{ActivityType.AnswersScraper}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, DownvoteAnswerProcess>($"{socialNetwork}{ActivityType.DownvoteAnswers}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, DownvoteQuestionProcess>(
                $"{socialNetwork}{ActivityType.DownvoteQuestions}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, QuestionScraperProcess>(
                $"{socialNetwork}{ActivityType.QuestionsScraper}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, ReportAnswerProcess>($"{socialNetwork}{ActivityType.ReportAnswers}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, ReportUser>($"{socialNetwork}{ActivityType.ReportUsers}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, UpvoteAnswerProcess>($"{socialNetwork}{ActivityType.UpvoteAnswers}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, UserScraperProcess>($"{socialNetwork}{ActivityType.UserScraper}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IBrowserManager, QdBrowserManager>(new HierarchicalLifetimeManager());
            Container.RegisterType<IBrowserManager, QdBrowserManager>(socialNetwork.ToString(),
                new HierarchicalLifetimeManager());
            Container.RegisterType<IBrowserManagerAsync, QdBrowserManager>(socialNetwork.ToString(),
                new HierarchicalLifetimeManager());
            Container.RegisterType<IQuoraBrowserManager, QdBrowserManager>(new HierarchicalLifetimeManager());
            Container.RegisterType<IQdAdScraperProcess, QdAdScraperProcess>(new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, UpvotePostProcess>($"{socialNetwork}{ActivityType.UpvotePost}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, DownvotePostProcess>($"{socialNetwork}{ActivityType.DownVotePost}",
                new HierarchicalLifetimeManager());
        }
    }
}