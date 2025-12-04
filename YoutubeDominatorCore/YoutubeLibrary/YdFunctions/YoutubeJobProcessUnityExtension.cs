using DominatorHouseCore.Enums;
using DominatorHouseCore.Process;
using Unity;
using Unity.Extension;
using Unity.Lifetime;
using YoutubeDominatorCore.YoutubeLibrary.Processes;

namespace YoutubeDominatorCore.YoutubeLibrary.YdFunctions
{
    public class YoutubeJobProcessUnityExtension : UnityContainerExtension
    {
        protected override void Initialize()
        {
            // Container.AddNewExtension<TdSubprocessContainerExtension>();

            var socialNetwork = SocialNetworks.YouTube;
            Container.RegisterType<IJobProcess, SubscribeProcess>($"{socialNetwork}{ActivityType.Subscribe}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, UnsubscribeProcess>($"{socialNetwork}{ActivityType.UnSubscribe}",
                new HierarchicalLifetimeManager());

            Container.RegisterType<IJobProcess, LikeProcess>($"{socialNetwork}{ActivityType.Like}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, DisLikeProcess>($"{socialNetwork}{ActivityType.Dislike}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, CommentProcess>($"{socialNetwork}{ActivityType.Comment}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, LikeCommentProcess>($"{socialNetwork}{ActivityType.LikeComment}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, ReportVideoProcess>($"{socialNetwork}{ActivityType.ReportVideo}",
                new HierarchicalLifetimeManager());

            Container.RegisterType<IJobProcess, ViewIncreaserProcess>($"{socialNetwork}{ActivityType.ViewIncreaser}",
                new HierarchicalLifetimeManager());

            Container.RegisterType<IJobProcess, ScrapChannelProcess>($"{socialNetwork}{ActivityType.ChannelScraper}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, ScrapePostProcess>($"{socialNetwork}{ActivityType.PostScraper}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, CommentScraperProcess>($"{socialNetwork}{ActivityType.CommentScraper}",
                new HierarchicalLifetimeManager());
        }
    }
}