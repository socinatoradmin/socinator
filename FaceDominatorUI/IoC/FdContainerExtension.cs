using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.FdQuery;
using DominatorHouseCore.Interfaces;
using DominatorUIUtility.IoC;
using FaceDominatorCore.DbMigration;
using FaceDominatorCore.FDFactories;
using FaceDominatorCore.FDLibrary.DAL;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDLibrary.FdFunctions.FdBrowserManager;
using FaceDominatorCore.FDLibrary.FdProcesses;
using FaceDominatorCore.FDRequest;
using FaceDominatorCore.UnityContainers;
using FaceDominatorUI.Factories;
using Unity;
using Unity.Extension;
using Unity.Injection;
using Unity.Lifetime;

namespace FaceDominatorUI.IoC
{
    // ReSharper disable once UnusedMember.Global
    public class FdContainerExtension : UnityContainerExtension
    {
        private const SocialNetworks CurrentNetwork = SocialNetworks.Facebook;

        protected override void Initialize()
        {
            Container.AddNewExtension<FdDbMigrationUnityExtension>();
            Container.AddNewExtension<FdJobProcessUnityExtension>();
            Container.RegisterSingleton<IAccountDatabaseConnection, FdAccountDbConnection>(SocialNetworks.Facebook
                .ToString());
            Container.RegisterSingleton<ICampaignDatabaseConnection, FdCampaignDbConnection>(SocialNetworks.Facebook
                .ToString());

            Container.RegisterType<ISocialNetworkModule, FdDominatorModule>(CurrentNetwork.ToString());
            Container.RegisterType<INetworkCollectionFactory, FacebookNetworkCollectionFactory>(
                CurrentNetwork.ToString());
            Container.RegisterType<IPublisherCollectionFactory, FacebookPublisherCollectionFactory>(
                CurrentNetwork.ToString());
            Container.RegisterType<IJobProcessFactory, FdJobProcessFactory>(CurrentNetwork.ToString(),
                new HierarchicalLifetimeManager());
            Container.RegisterType<IDbAccountService, DbAccountService>();
            Container.RegisterType<IDbAccountServiceScoped, DbAccountServiceScoped>();
            Container.RegisterType<IDbCampaignServiceScoped, DbCampaignServiceScoped>();
            Container.RegisterSingleton<IDbGlobalService, DbGlobalService>();
            Container.RegisterType<IFacebookScraperActionTables, FacebookScraperActionTables>(
                new HierarchicalLifetimeManager());
            Container.RegisterType<IFdQueryScraperFactory, FdQueryScraperFactory>(new HierarchicalLifetimeManager());
            Container.RegisterType<IFdHttpHelper, FdHttpHelper>(new HierarchicalLifetimeManager());
            Container.RegisterType<IHttpHelper>(CurrentNetwork.ToString(),
                new HierarchicalLifetimeManager(),
                new InjectionFactory(container => { return container.Resolve<IFdHttpHelper>(); }));
            Container.RegisterType<IFdAdsScraperFactory, FdAdsScraperFactory>(new HierarchicalLifetimeManager());
            Container.RegisterType<IFdLcsScraperFactory, FdLcsScraperFactory>(new HierarchicalLifetimeManager());
            Container.RegisterType<IAdScraperFactory>(AdUpdationType.FbAds.ToString(),
                new HierarchicalLifetimeManager(),
                new InjectionFactory(container => { return container.Resolve<IFdAdsScraperFactory>(); }));

            Container.RegisterType<IAdScraperFactory>(AdUpdationType.Lcs.ToString(),
                new HierarchicalLifetimeManager(),
                new InjectionFactory(container => { return container.Resolve<IFdLcsScraperFactory>(); }));
            Container.RegisterType<IFdRequestLibrary, FdRequestLibrary>(new HierarchicalLifetimeManager());
            Container.RegisterType<IFdLoginProcess, FdLoginProcess>(new HierarchicalLifetimeManager());

            Container.RegisterType<IBrowserManager, FdBrowserManager>(CurrentNetwork.ToString(),
                new HierarchicalLifetimeManager());
            Container.RegisterType<IFdBaseBrowserManger, FdBaseBrowserManger>(new HierarchicalLifetimeManager());

            Container.RegisterType<IFdBrowserManager, FdBrowserManager>(new HierarchicalLifetimeManager());
            Container.RegisterType<IBrowserManagerFactory, BrowserManagerFactory>(new HierarchicalLifetimeManager());

            Container.RegisterType<IFdAccountUpdateFactory, FdAccountUpdateFactory>();
            Container.RegisterSingleton<IPostScraperConstants, PostScraperConstants>();
            Container.RegisterSingleton<IFdUpdateAccountProcess, FdUpdateAccountProcess>();


            //Container.RegisterSingleton<IJobProcessFactory, FdJobProcessFactory>(CurrentNetwork.ToString());
            // Container.RegisterSingleton<IFdQueryScraperFactory, FdQueryScraperFactory>(CurrentNetwork.ToString());
        }
    }
}