using ThreadUtils;
using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorUIUtility.IoC;
using TwtDominatorCore.Database;
using TwtDominatorCore.DbMigrations;
using TwtDominatorCore.Factories;
using TwtDominatorCore.Interface;
using TwtDominatorCore.Requests;
using TwtDominatorCore.TDFactories;
using TwtDominatorCore.TDLibrary;
using TwtDominatorCore.TDLibrary.GeneralLibrary;
using TwtDominatorCore.TDLibrary.GeneralLibrary.DAL;
using TwtDominatorCore.TDUtility;
using TwtDominatorUI.Factories;
using Unity;
using Unity.Extension;
using Unity.Injection;
using Unity.Lifetime;

namespace TwtDominatorUI.IoC
{
    public class TwtContainerExtension : UnityContainerExtension
    {
        private const SocialNetworks CurrentNetwork = SocialNetworks.Twitter;

        protected override void Initialize()
        {
            Container.AddNewExtension<TdDbMigrationUnityExtension>();
            Container.AddNewExtension<TwitterJobProcessUnityExtension>();

            Container.RegisterSingleton<IAccountDatabaseConnection, TdAccountDbConnection>(CurrentNetwork.ToString());
            Container.RegisterSingleton<ICampaignDatabaseConnection, TdCampaignDbConnection>(CurrentNetwork.ToString());

            Container.RegisterType<IDbAccountService, DbAccountService>();
            Container.RegisterType<IDbAccountServiceScoped, DbAccountServiceScoped>();
            Container.RegisterSingleton<IDbGlobalService, DbGlobalService>();

            Container.RegisterType<ISocialNetworkModule, TwtDominatorModule>(CurrentNetwork.ToString());
            Container.RegisterType<INetworkCollectionFactory, TwitterNetworkCollectionFactory>(
                CurrentNetwork.ToString());
            Container.RegisterType<IPublisherCollectionFactory, TwitterPublisherCollectionFactory>(
                CurrentNetwork.ToString());


            Container.RegisterSingleton<IAccountContactConfig, AccountContactConfig>();

            Container.RegisterSingleton<ITwitterRequestUrlProvider, TwitterRequestUrlProvider>();
            Container.RegisterSingleton<IContentUploaderService, ContentUploaderService>();

            // this must be different for all accounts(RegisterType instead of RegisterSingleton)
            // since all account have their own module setting for BlackWhiteListHandler
            Container.RegisterType<IBlackWhiteListHandler, BlackWhiteListHandlerScoped>();


            Container.RegisterType<IJobProcessFactory, TdJobProcessFactory>(CurrentNetwork.ToString(),
                new HierarchicalLifetimeManager());
            Container.RegisterType<ITwitterScraperActionTables, TwitterScraperActionTables>(
                new HierarchicalLifetimeManager());
            Container.RegisterType<ITdQueryScraperFactory, TdQueryScraperFactory>(new HierarchicalLifetimeManager());
            Container.RegisterType<ITdHttpHelper, TdHttpHelper>(new HierarchicalLifetimeManager());
            Container.RegisterType<IHttpHelper>(CurrentNetwork.ToString(),
                new HierarchicalLifetimeManager(),
                new InjectionFactory(container => { return container.Resolve<ITdHttpHelper>(); }));

            Container.RegisterType<ITwitterFunctionFactory, TwitterFunctionFactory>(new HierarchicalLifetimeManager());
            Container.RegisterType<ITwitterFunctions, TwitterFunctions>(new HierarchicalLifetimeManager());

            Container.RegisterType<ITwitterFunctions, BrowserTwitterFunctions>("browser");
            Container.RegisterType<IBrowserManager, BrowserTwitterFunctions>(CurrentNetwork.ToString(),
                new HierarchicalLifetimeManager());

            //Container.RegisterType<TwitterFunctions>(new InjectionConstructor(new ResolvedParameter<ITwitterFunctions>("http")));
            //Container.RegisterType<BrowserTwitterFunctions>(new InjectionConstructor(new ResolvedParameter<ITwitterFunctions>("browser")));
            Container.RegisterType<IDbInsertionHelper, DbInsertionHelper>(new HierarchicalLifetimeManager(),
                new InjectionConstructor(typeof(IDbAccountServiceScoped), typeof(IProcessScopeModel),
                    typeof(IDbCampaignService)));

            Container.RegisterType<ITwtLogInProcess, LogInProcess>(new HierarchicalLifetimeManager());
            Container.RegisterType<IDelayService, DelayService>(new HierarchicalLifetimeManager());

            Container.RegisterType<ITwtLogInProcess, LogInProcess>(new HierarchicalLifetimeManager());
            Container.RegisterType<IDelayService, DelayService>(new HierarchicalLifetimeManager());
            Container.RegisterType<IDbInsertionHelper, DbInsertionHelper>(new HierarchicalLifetimeManager(),
                new InjectionConstructor(typeof(IDbAccountServiceScoped), typeof(IProcessScopeModel),
                    typeof(IDbCampaignService)));
            //Container.RegisterType<IDbCampaignService, DbCampaignService>(new HierarchicalLifetimeManager());
            //Container.RegisterType<IDbInsertionHelper, DbInsertionHelper>(new InjectionConstructor(typeof(DbCampaignService)));
            Container.RegisterSingleton<ITwitterAccountSessionManager, TwitterAccountSessionManager>();
            Container.RegisterSingleton<ITDAccountUpdateFactory, TDAccountUpdateFactory>();
        }
    }
}