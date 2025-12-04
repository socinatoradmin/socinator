using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorUIUtility.IoC;
using LinkedDominatorCore.DbMigrations;
using LinkedDominatorCore.Factories;
using LinkedDominatorCore.Interfaces;
using LinkedDominatorCore.LDLibrary;
using LinkedDominatorCore.LDLibrary.DAL;
using LinkedDominatorCore.LDUtility;
using LinkedDominatorCore.Request;
using LinkedDominatorCore.Utility;
using LinkedDominatorUI.Factories;
using Unity;
using Unity.Extension;
using Unity.Injection;
using Unity.Lifetime;

namespace LinkedDominatorUI.IoC
{
    public class LdContainerExtension : UnityContainerExtension
    {
        private const SocialNetworks CurrentNetwork = SocialNetworks.LinkedIn;

        protected override void Initialize()
        {
            Container.AddNewExtension<LdDbMigrationUnityExtension>();
            Container.AddNewExtension<LinkedInJobProcessUnityExtension>();

            Container.RegisterType<IAccountDatabaseConnection, LdAccountDbConnection>(CurrentNetwork.ToString());
            Container.RegisterType<ICampaignDatabaseConnection, LdCampaignDbConnection>(CurrentNetwork.ToString());

            Container.RegisterType<ISocialNetworkModule, LdDominatorModule>(CurrentNetwork.ToString());
            Container.RegisterType<INetworkCollectionFactory, LinkedInNetworkCollectionFactory>(
                CurrentNetwork.ToString());
            Container.RegisterType<IPublisherCollectionFactory, LinkedInPublisherCollectionFactory>(
                CurrentNetwork.ToString());


            Container.RegisterType<IJobProcessFactory, LDJobProcessFactory>(CurrentNetwork.ToString());

            Container.RegisterType<IHttpHelper, LdHttpHelper>(CurrentNetwork.ToString());
            Container.RegisterType<ILdHttpHelper, LdHttpHelper>(new HierarchicalLifetimeManager());
            Container.RegisterType<IHttpHelper>(CurrentNetwork.ToString(),
                new HierarchicalLifetimeManager(),
                new InjectionFactory(container => container.Resolve<ILdHttpHelper>()));

            Container.RegisterType<ILdFunctions, LdFunctions>(new HierarchicalLifetimeManager());
            Container.RegisterType<ILdFunctions, BrowserLdFunction>("browser");
            Container.RegisterType<ILdFunctionFactory, LdFunctionFactory>(new HierarchicalLifetimeManager());
            Container.RegisterType<IBrowserManager, BrowserLdFunction>(CurrentNetwork.ToString(),
                new HierarchicalLifetimeManager());

            Container.RegisterSingleton<ILdAccountUpdateFactory, LdAccountUpdateFactory>();
            Container.RegisterSingleton<ILDAccountSessionManager, LDAccountSessionManager>();

            // unit test
            Container.RegisterType<ILdLogInProcess, LogInProcess>(new HierarchicalLifetimeManager());
            Container.RegisterType<ILoginSalesNavigator, LoginSalesNavigator>();

            Container.RegisterType<IDbAccountService, DbAccountService>();
            Container.RegisterType<IDbAccountServiceScoped, DbAccountServiceScoped>();
            Container.RegisterSingleton<IDbGlobalService, DbGlobalService>();
            Container.RegisterType<ILdQueryScraperFactory, LdQueryScraperFactory>(new HierarchicalLifetimeManager());
            Container.RegisterType<ILinkedInScraperActionTables, LinkedInScraperActionTables>(
                new HierarchicalLifetimeManager());

            // Network based
            Container.RegisterSingleton<IClassMapper, ClassMapper>();
            Container.RegisterSingleton<IDetailsFetcher, DetailsFetcher>();
            Container.RegisterSingleton<ILdUserFilterProcess, LdUserFilterProcess>();
            Container.RegisterType<IDbInsertionHelper, DbInsertionHelper>();

            Container.RegisterSingleton<ILdUniqueHandler, LdUniqueHandler>();
        }
    }
}