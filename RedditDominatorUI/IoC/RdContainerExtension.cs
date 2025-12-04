using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.FdQuery;
using DominatorHouseCore.Interfaces;
using DominatorUIUtility.IoC;
using RedditDominatorCore.DbMigrations;
using RedditDominatorCore.Factories;
using RedditDominatorCore.Interface;
using RedditDominatorCore.RDFactories;
using RedditDominatorCore.RDLibrary;
using RedditDominatorCore.RDLibrary.BrowserManager;
using RedditDominatorCore.RDLibrary.DAL;
using RedditDominatorCore.RDRequest;
using RedditDominatorCore.Utility;
using RedditDominatorUI.Factories;
using Unity;
using Unity.Extension;
using Unity.Injection;
using Unity.Lifetime;

namespace RedditDominatorUI.IoC
{
    // ReSharper disable once UnusedMember.Global
    public class RdContainerExtension : UnityContainerExtension
    {
        private const SocialNetworks CurrentNetwork = SocialNetworks.Reddit;

        protected override void Initialize()
        {
            Container.AddNewExtension<RdDbMigrationUnityExtension>();
            Container.AddNewExtension<RedditJobProcessUnityExtension>();

            Container.RegisterSingleton<IAccountDatabaseConnection, RdAccountDbConnection>(CurrentNetwork.ToString());
            Container.RegisterSingleton<ICampaignDatabaseConnection, RdCampaignDbConnection>(CurrentNetwork.ToString());

            Container.RegisterType<ISocialNetworkModule, RdDominatorModule>(CurrentNetwork.ToString());
            Container.RegisterType<INetworkCollectionFactory, RedditNetworkCollectionFactory>(
                CurrentNetwork.ToString());
            Container.RegisterType<IPublisherCollectionFactory, RedditPublisherCollectionFactory>(
                CurrentNetwork.ToString());

            Container.RegisterType<IJobProcessFactory, RdJobProcessFactory>(CurrentNetwork.ToString(),
                new HierarchicalLifetimeManager());

            Container.RegisterType<IDbAccountServiceScoped, DbAccountServiceScoped>();
            Container.RegisterType<IDbAccountService, DbAccountService>();
            Container.RegisterSingleton<IDbGlobalService, DbGlobalService>();

            Container.RegisterType<IRedditScraperActionTables, RedditScraperActionTables>(
                new HierarchicalLifetimeManager());
            Container.RegisterType<IRdQueryScraperFactory, RdQueryScraperFactory>(new HierarchicalLifetimeManager());
            Container.RegisterType<IRdHttpHelper, RdHttpHelper>(new HierarchicalLifetimeManager());
            Container.RegisterType<IHttpHelper>(CurrentNetwork.ToString(),
                new HierarchicalLifetimeManager(),
                new InjectionFactory(container => container.Resolve<IRdHttpHelper>()));
            Container.RegisterType<IRDAccountSessionManager, RDAccountSessionManager>(new HierarchicalLifetimeManager());
            Container.RegisterType<IRedditFunction, RedditFunction>(new HierarchicalLifetimeManager());
            Container.RegisterType<IRedditLogInProcess, LogInProcess>(new HierarchicalLifetimeManager());

            Container.RegisterSingleton<IRdAccountUpdateFactory, RdAccountUpdateFactory>();
            Container.RegisterSingleton<IRdUpdateAccountProcess, RdUpdateAccountProcess>(CurrentNetwork.ToString());
            Container.RegisterType<IRdBrowserManager, RdBrowserManager>(new HierarchicalLifetimeManager());
            Container.RegisterType<IBrowserManager, RdBrowserManager>(CurrentNetwork.ToString(),
                new HierarchicalLifetimeManager());
            Container.RegisterType<IRdBrowserManager, RdBrowserManager>(new HierarchicalLifetimeManager());
            Container.RegisterType<IRdAdsScrapperFactory, RdAdsScrapperFactory>(new HierarchicalLifetimeManager());
            Container.RegisterType<IAdScraperFactory>(AdUpdationType.RedditAds.ToString(),
                new HierarchicalLifetimeManager(),
                new InjectionFactory(container => container.Resolve<IRdAdsScrapperFactory>()));
        }
    }
}