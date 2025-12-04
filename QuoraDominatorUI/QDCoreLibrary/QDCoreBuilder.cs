using CommonServiceLocator;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using QuoraDominatorCore.Interface;
using QuoraDominatorCore.QDFactories;
using QuoraDominatorUI.Utility;

namespace QuoraDominatorUI.QDCoreLibrary
{
    public class QdCoreBuilder : NetworkCoreLibraryBuilder
    {
        private static QdCoreBuilder _instance;

        private QdCoreBuilder(INetworkCoreFactory networkCoreFactory)
            : base(networkCoreFactory)
        {
            QdInitializer.RegisterModules();
            InstanceProvider.GetInstance<IQDSessionManager>();
            AddNetwork(SocialNetworks.Quora)
                .AddAccountFactory(InstanceProvider.GetInstance<IQDAccountUpdateFactory>())
                .AddTabFactory(QdTabHandlerFactory.Instance())
                .AddAccountCounts(QdAccountCountFactory.Instance)
                .AddAccountUiTools(QdAccountToolsFactory.Instance)
                .AddAccountDbConnection(
                    InstanceProvider.GetInstance<IAccountDatabaseConnection>(SocialNetworks.Quora.ToString()))
                .AddCampaignDbConnection(
                    InstanceProvider.GetInstance<ICampaignDatabaseConnection>(SocialNetworks.Quora.ToString()))
                .AddReportFactory(new QdReportFactory())
                .AddViewCampaignFactory(new QdViewCampaignsFactory())
                .AddAccountVerificationFactory(InstanceProvider.GetInstance<QdAccountVerification>())
                .AddChatFactory(new QdLiveChatFactory(InstanceProvider.GetInstance<IAccountScopeFactory>(),
                    InstanceProvider.GetInstance<IAccountsFileManager>()));
        }

        public static QdCoreBuilder Instance(INetworkCoreFactory networkCoreFactory)
        {
            return _instance ?? (_instance = new QdCoreBuilder(networkCoreFactory));
        }

        public INetworkCoreFactory GetQdCoreObjects()
        {
            return NetworkCoreFactory;
        }
    }
}