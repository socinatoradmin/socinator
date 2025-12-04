using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorUIUtility.IoC;
using PinDominator.Factories;
using PinDominatorCore.DbMigrations;
using PinDominatorCore.Interface;
using PinDominatorCore.PDFactories;
using PinDominatorCore.PDLibrary;
using PinDominatorCore.PDLibrary.DAL;
using PinDominatorCore.PDLibrary.PowerAdsSpy;
using PinDominatorCore.PDLibrary.Process;
using PinDominatorCore.PDUtility;
using PinDominatorCore.Request;
using PinDominatorCore.UnityContainers;
using Unity;
using Unity.Extension;
using Unity.Injection;
using Unity.Lifetime;

namespace PinDominator.IoC
{
    public class PdContainerExtension : UnityContainerExtension
    {
        private const SocialNetworks CurrentNetwork = SocialNetworks.Pinterest;

        protected override void Initialize()
        {
            Container.AddNewExtension<PdDbMigrationUnityExtension>();
            Container.AddNewExtension<PinterestJobProcessUnityExtension>();

            Container.RegisterSingleton<IAccountDatabaseConnection, PdAccountDbConnection>(CurrentNetwork.ToString());
            Container.RegisterSingleton<ICampaignDatabaseConnection, PdCampaignDbConnection>(CurrentNetwork.ToString());
            Container.RegisterType<ISocialNetworkModule, PdDominatorModule>(CurrentNetwork.ToString());
            Container.RegisterType<INetworkCollectionFactory, PinterestNetworkCollectionFactory>(
                CurrentNetwork.ToString());
            Container.RegisterType<IPublisherCollectionFactory, PinterestPublisherCollectionFactory>(
                CurrentNetwork.ToString());

            Container.RegisterType<IJobProcessFactory, PdJobProcessFactory>(CurrentNetwork.ToString(),
                new HierarchicalLifetimeManager());
            Container.RegisterType<IDbAccountService, DbAccountService>();
            Container.RegisterType<IDbAccountServiceScoped, DbAccountServiceScoped>();
            Container.RegisterSingleton<IDbGlobalService, DbGlobalService>();
            Container.RegisterType<IPinterestScraperActionTables, PinterestScraperActionTables>(
                new HierarchicalLifetimeManager());
            Container.RegisterType<IPdQueryScraperFactory, PdQueryScraperFactory>(new HierarchicalLifetimeManager());
            Container.RegisterType<IPdHttpHelper, PdHttpHelper>(new HierarchicalLifetimeManager());
            Container.RegisterType<IHttpHelper>(CurrentNetwork.ToString(),
                new HierarchicalLifetimeManager(),
                new InjectionFactory(container => { return container.Resolve<IPdHttpHelper>(); }));
            Container.RegisterType<IPinFunction, PinFunction>(new HierarchicalLifetimeManager());
            Container.RegisterType<IPdLogInProcess, LogInProcess>(new HierarchicalLifetimeManager());
            Container.RegisterType<IBrowserManager, PdBrowserManager>(CurrentNetwork.ToString(),
                new HierarchicalLifetimeManager());
            Container.RegisterType<IPdBrowserManager, PdBrowserManager>(new HierarchicalLifetimeManager());
            Container.RegisterSingleton<IPdAccountUpdateFactory, PdAccountUpdateFactory>();
            Container.RegisterSingleton<IPDAccountSessionManager, PDAccountSessionManager>();
            Container.RegisterType<IAdsScraperFunction, AdsScraperFunction>(new HierarchicalLifetimeManager());
        }
    }
}