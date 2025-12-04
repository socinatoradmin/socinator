using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.FdQuery;
using DominatorHouseCore.Interfaces;
using DominatorUIUtility.IoC;
using QuoraDominatorCore.DbMigrations;
using QuoraDominatorCore.Factories;
using QuoraDominatorCore.Interface;
using QuoraDominatorCore.QDFactories;
using QuoraDominatorCore.QdLibrary;
using QuoraDominatorCore.QdLibrary.DAL;
using QuoraDominatorCore.QdLibrary.Processors.Message;
using QuoraDominatorCore.QdUtility;
using QuoraDominatorCore.Request;
using QuoraDominatorCore.UnityContainers;
using QuoraDominatorUI.Factories;
using Unity;
using Unity.Extension;
using Unity.Injection;
using Unity.Lifetime;

namespace QuoraDominatorUI.IoC
{
    public class QdContainerExtension : UnityContainerExtension
    {
        private const SocialNetworks CurrentNetwork = SocialNetworks.Quora;

        protected override void Initialize()
        {
            Container.AddNewExtension<QdDbMigrationUnityExtension>();
            Container.AddNewExtension<QuoraJobProcessUnityExtension>();
            Container.RegisterType<IQDBrowserManagerFactory, QDBrowserManagerFactory>();
            Container.RegisterSingleton<IAccountDatabaseConnection, QdAccountDbConnection>(CurrentNetwork.ToString());
            Container.RegisterSingleton<ICampaignDatabaseConnection, QdCampaignDbConnection>(CurrentNetwork.ToString());
            Container.RegisterType<ISocialNetworkModule, QdDominatorModule>(CurrentNetwork.ToString());
            Container.RegisterType<INetworkCollectionFactory, QuoraNetworkCollectionFactory>(CurrentNetwork.ToString());
            Container.RegisterType<IPublisherCollectionFactory, QuoraPublisherCollectionFactory>(
                CurrentNetwork.ToString());

            Container.RegisterType<IJobProcessFactory, QdJobProcessFactory>(CurrentNetwork.ToString(),
                new HierarchicalLifetimeManager());
            Container.RegisterType<IDbAccountService, DbAccountService>();
            Container.RegisterType<IDbAccountServiceScoped, DbAccountServiceScoped>();
            Container.RegisterSingleton<IDbGlobalService, DbGlobalService>();
            Container.RegisterType<IQuoraScraperActionTables, QuoraScraperActionTables>(
                new HierarchicalLifetimeManager());
            Container.RegisterType<IQdQueryScraperFactory, QdQueryScraperFactory>(new HierarchicalLifetimeManager());
            Container.RegisterType<IQdHttpHelper, QdHttpHelper>(new HierarchicalLifetimeManager());
            Container.RegisterType<IHttpHelper>(CurrentNetwork.ToString(),
                new HierarchicalLifetimeManager(),
                new InjectionFactory(container => container.Resolve<IQdHttpHelper>()));
            Container.RegisterType<IQuoraFunctions, QuoraFunct>(new HierarchicalLifetimeManager());
            Container.RegisterType<IQdLogInProcess, LogInProcess>(new HierarchicalLifetimeManager());
            Container.RegisterSingleton<IQDAccountUpdateFactory, QdAccountUpdateFactory>();

            Container.RegisterType<IQdPostScraperFactory, QdPostScraperFactory>(new HierarchicalLifetimeManager());
            Container.RegisterType<IAdScraperFactory>(AdUpdationType.QuoraAds.ToString(),
                new HierarchicalLifetimeManager(),
                new InjectionFactory(container => container.Resolve<IQdPostScraperFactory>()));
            Container.RegisterSingleton<IQDSessionManager, QDAccountSessionManager>();
            Container.RegisterType<ISendMessageToFollowerProcessor, SendMessageToFollowerProcessor>(new HierarchicalLifetimeManager());
        }
    }
}