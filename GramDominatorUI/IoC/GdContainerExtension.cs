using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorUIUtility.IoC;
using GramDominatorCore.DbMigrations;
using GramDominatorCore.Factories;
using GramDominatorCore.GDFactories;
using GramDominatorCore.GDLibrary;
using GramDominatorCore.GDLibrary.DAL;
using GramDominatorCore.GDLibrary.InstagramBrowser;
using GramDominatorCore.GDLibrary.LoginAndUpdate;
using GramDominatorCore.PowerAdSpy;
using GramDominatorCore.Request;
using GramDominatorCore.UnityContainers;
using GramDominatorUI.CustomControl;
using GramDominatorUI.Factories;
using Unity;
using Unity.Extension;
using Unity.Injection;
using Unity.Lifetime;

namespace GramDominatorUI.IoC
{
    public class GdContainerExtension : UnityContainerExtension
    {
        private const SocialNetworks CurrentNetwork = SocialNetworks.Instagram;

        protected override void Initialize()
        {
            Container.AddNewExtension<GdDbMigrationUnityExtension>();
            Container.AddNewExtension<GDJobProcessUnityExtension>();

            Container.RegisterSingleton<IAccountDatabaseConnection, GdAccountDbConnection>(CurrentNetwork.ToString());
            Container.RegisterSingleton<ICampaignDatabaseConnection, GdCampaignDbConnection>(CurrentNetwork.ToString());

            Container.RegisterType<IDbAccountService, DbAccountService>();
            Container.RegisterType<IDbAccountServiceScoped, DbAccountServiceScoped>();
            Container.RegisterType<IDbGlobalService, DbGlobalService>();

            Container.RegisterType<ISocialNetworkModule, GdDominatorModule>(CurrentNetwork.ToString());
            Container.RegisterType<INetworkCollectionFactory, InstagramNetworkCollectionFactory>(
                CurrentNetwork.ToString());
            Container.RegisterType<IPublisherCollectionFactory, InstagramPublisherCollectionFactory>(
                CurrentNetwork.ToString());
            Container.RegisterType<IInstagramScraperActionTables, InstagramScraperActionTables>(
                new HierarchicalLifetimeManager());
            Container.RegisterType<IGdQueryScraperFactory, GDQueryScraperFactory>(new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcessFactory, GdJobProcessFactory>(CurrentNetwork.ToString(),
                new HierarchicalLifetimeManager());

            Container.RegisterType<IHttpHelper>(CurrentNetwork.ToString(), new HierarchicalLifetimeManager(),
                new InjectionFactory(container => { return container.Resolve<IGdHttpHelper>(); }));

            Container.RegisterType<IGdHttpHelper, IgHttpHelper>(new HierarchicalLifetimeManager());
            // Container.RegisterType<IInstaFunction, InstaFunct>(new HierarchicalLifetimeManager());
            Container.RegisterType<IEncryptPassword, EncryptPassword>(new HierarchicalLifetimeManager());
            Container.RegisterType<IGdLogInProcess, LogInProcess>(new HierarchicalLifetimeManager());
            Container.RegisterType<IGDAccountUpdateFactory, GDAccountUpdateFactory>();
            Container.RegisterType<IAccountUpdateProcess, AccountUpdateProcess>();
            Container.RegisterType<IInstaAdScrappers, InstaAdsScrappers>();
            Container.RegisterSingleton<IQueryFollowedControl, QueryFollowedControl>();
            // Container.RegisterType<IBrowserManager, GdBrowserManager>(CurrentNetwork.ToString(), new HierarchicalLifetimeManager());

            Container.RegisterType<IInstaFunctFactory, InstaFunctFactory>(new HierarchicalLifetimeManager());
            Container.RegisterType<IInstaFunction, InstaFunct>(new HierarchicalLifetimeManager());
            //Container.RegisterType<IInstaFunction, BrowserInstaFunct>("browser");
            Container.RegisterType<IGdBrowserManager, GdBrowserManager>(new HierarchicalLifetimeManager());
            Container.RegisterType<IBrowserManagerFactory, BrowserManagerFactory>(new HierarchicalLifetimeManager());

            Container.RegisterType<IBrowserManager, GdBrowserManager>(CurrentNetwork.ToString(),
                new HierarchicalLifetimeManager());
        }
    }
}