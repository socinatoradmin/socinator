using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorUIUtility.IoC;
using Unity;
using Unity.Extension;
using Unity.Injection;
using Unity.Lifetime;
using YoutubeDominatorCore.DbMigrations;
using YoutubeDominatorCore.Request;
using YoutubeDominatorCore.YDFactories;
using YoutubeDominatorCore.YDUtility;
using YoutubeDominatorCore.YoutubeLibrary.DAL;
using YoutubeDominatorCore.YoutubeLibrary.Processes;
using YoutubeDominatorCore.YoutubeLibrary.YdFunctions;
using YoutubeDominatorUI.Factories;

namespace YoutubeDominatorUI.IoC
{
    public class YdContainerExtension : UnityContainerExtension
    {
        private const SocialNetworks CurrentNetwork = SocialNetworks.YouTube;

        protected override void Initialize()
        {
            Container.AddNewExtension<YdDbMigrationUnityExtension>();
            Container.AddNewExtension<YoutubeJobProcessUnityExtension>();

            Container.RegisterSingleton<IAccountDatabaseConnection, YdAccountDbConnection>(CurrentNetwork.ToString());
            Container.RegisterSingleton<ICampaignDatabaseConnection, YdCampaignDbConnection>(CurrentNetwork.ToString());

            Container.RegisterType<ISocialNetworkModule, YdDominatorModule>(CurrentNetwork.ToString());
            Container.RegisterType<INetworkCollectionFactory, YoutubeNetworkCollectionFactory>(
                CurrentNetwork.ToString());
            Container.RegisterType<IPublisherCollectionFactory, YoutubePublisherCollectionFactory>(
                CurrentNetwork.ToString());

            Container.RegisterType<IJobProcessFactory, YdJobProcessFactory>(CurrentNetwork.ToString(),
                new HierarchicalLifetimeManager());

            Container.RegisterType<IDbAccountService, DbAccountService>();
            Container.RegisterType<IDbAccountServiceScoped, DbAccountServiceScoped>();
            Container.RegisterSingleton<IDbGlobalService, DbGlobalService>();

            // this must be different for all accounts(RegisterType instead of RegisterSingleton)
            // since all account have their own module setting for BlackWhiteListHandler
            Container.RegisterType<IBlackWhiteListHandler, BlackWhiteListHandlerScoped>();

            Container.RegisterType<IYoutubeScraperActionTables, YoutubeScraperActionTables>(
                new HierarchicalLifetimeManager());
            Container.RegisterType<IYdQueryScraperFactory, YdQueryScraperFactory>(new HierarchicalLifetimeManager());
            Container.RegisterType<IYdHttpHelper, YdHttpHelper>(new HierarchicalLifetimeManager());
            Container.RegisterType<IHttpHelper>(CurrentNetwork.ToString(),
                new HierarchicalLifetimeManager(),
                new InjectionFactory(container => { return container.Resolve<IYdHttpHelper>(); }));
            Container.RegisterType<IYoutubeFunctionality, YoutubeFunctionality>(new HierarchicalLifetimeManager());
            Container.RegisterType<IYoutubeLogInProcess, LoginProcess>(new HierarchicalLifetimeManager());

            Container.RegisterSingleton<IYdAccountUpdateFactory, YdAccountUpdateFactory>();
            Container.RegisterSingleton<IAccountVerificationFactory, YdAccountVerificationFactory>();
            Container.RegisterType<IYdBrowserManager, YdBrowserManager>(new HierarchicalLifetimeManager());

            Container.RegisterType<IBrowserManager, YdBrowserManager>(CurrentNetwork.ToString(),
                new HierarchicalLifetimeManager());
            //Container.RegisterType<IThreadUtility, ThreadUtility>();
        }
    }
}