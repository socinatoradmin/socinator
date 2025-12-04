using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorUIUtility.IoC;
using TumblrDominatorCore.DbMigrations;
using TumblrDominatorCore.Interface;
using TumblrDominatorCore.TmblrUtility;
using TumblrDominatorCore.TumblrFactory;
using TumblrDominatorCore.TumblrLibrary.DAL;
using TumblrDominatorCore.TumblrLibrary.TumblrFunction;
using TumblrDominatorCore.TumblrLibrary.TumblrFunction.TumblrBrowserManager;
using TumblrDominatorCore.TumblrLibrary.TumblrProcesses;
using TumblrDominatorCore.TumblrRequest;
using TumblrDominatorCore.UnityContainers;
using TumblrDominatorUI.Factories;
using Unity;
using Unity.Extension;
using Unity.Injection;
using Unity.Lifetime;

namespace TumblrDominatorUI.IoC
{
    // ReSharper disable once UnusedMember.Global
    public class TumblrContainerExtension : UnityContainerExtension
    {
        private const SocialNetworks CurrentNetwork = SocialNetworks.Tumblr;

        protected override void Initialize()
        {
            Container.AddNewExtension<TumblrDbMigrationUnityExtension>();
            Container.AddNewExtension<TumblrJobProcessUnityExtension>();
            Container.RegisterSingleton<IAccountDatabaseConnection, TumblrAccountDbConnection>(
                CurrentNetwork.ToString());
            Container.RegisterSingleton<ICampaignDatabaseConnection, TumblrCampaignDbConnection>(
                CurrentNetwork.ToString());

            Container.RegisterType<ISocialNetworkModule, TumblrDominatorModule>(CurrentNetwork.ToString());
            Container.RegisterType<INetworkCollectionFactory, TumblrNetworkCollectionFactory>(
                CurrentNetwork.ToString());
            Container.RegisterType<IPublisherCollectionFactory, TumblrPublisherCollectionFactory>(
                CurrentNetwork.ToString());
            Container.RegisterType<ITumblrHttpHelper, TumblrHttpHelper>(new HierarchicalLifetimeManager());

            Container.RegisterType<IJobProcessFactory, TumblrJobProcessFactory>(CurrentNetwork.ToString(),
                new HierarchicalLifetimeManager());
            Container.RegisterType<ITumblrQueryScraperFactory, TumblrQueryScraperFactory>(
                new HierarchicalLifetimeManager());
            Container.RegisterType<ITumblrScraperActionTables, TumblrScraperActionTables>(
                new HierarchicalLifetimeManager());

            Container.RegisterType<IDbAccountService, DbAccountService>();
            Container.RegisterType<IDbAccountServiceScoped, DbAccountServiceScoped>();
            Container.RegisterSingleton<IDbGlobalService, DbGlobalService>();
            Container.RegisterType<IHttpHelper>(CurrentNetwork.ToString(),
                new HierarchicalLifetimeManager(),
                new InjectionFactory(container => { return container.Resolve<ITumblrHttpHelper>(); }));
            Container.RegisterSingleton<ITumblrAccountSession, TumblrAccountSessionManager>();
            Container.RegisterType<ITumblrFunct, TumblrFunct>(new HierarchicalLifetimeManager());
            Container.RegisterType<ITumblrLoginProcess, LoginProcess>(new HierarchicalLifetimeManager());
            Container.RegisterSingleton<ITumblrAccountUpdateFactory, TumblrAccountUpdateFactory>();
            Container.RegisterType<IBrowserManager, TumblrBrowserManager>(CurrentNetwork.ToString(),
                new HierarchicalLifetimeManager());
            Container.RegisterType<ITumblrBrowserManager, TumblrBrowserManager>(new HierarchicalLifetimeManager());

        }
    }
}