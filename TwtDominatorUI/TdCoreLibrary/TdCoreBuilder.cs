using CommonServiceLocator;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using TwtDominatorCore.Interface;
using TwtDominatorCore.TDFactories;
using TwtDominatorCore.TDLibrary.GeneralLibrary;

namespace TwtDominatorUI.TdCoreLibrary
{
    public class TdCoreBuilder : NetworkCoreLibraryBuilder
    {
        private static TdCoreBuilder _instance;


        private TdCoreBuilder(INetworkCoreFactory networkCoreFactory, AccessorStrategies strategies)
            : base(networkCoreFactory)
        {
            TDInitialiser.RegisterModules();
            InstanceProvider.GetInstance<ITwitterAccountSessionManager>();
            AddNetwork(SocialNetworks.Twitter)
                .AddAccountFactory(InstanceProvider.GetInstance<ITDAccountUpdateFactory>())
                .AddTabFactory(TdTabHandlerFactory.Instance(strategies))
                .AddAccountCounts(TdAccountCountFactory.Instance)
                .AddAccountUiTools(TdAccountToolsFactory.Instance)
                .AddAccountDbConnection(
                    InstanceProvider.GetInstance<IAccountDatabaseConnection>(SocialNetworks.Twitter.ToString()))
                .AddCampaignDbConnection(
                    InstanceProvider.GetInstance<ICampaignDatabaseConnection>(SocialNetworks.Twitter.ToString()))
                .AddReportFactory(new TDReportFactory())
                .AddViewCampaignFactory(new TDViewCampaignsFactory())
                .AddAccountSelectors(TdAccountSelectorFactory.Instance)
                //.AddChatFactory(new TDLiveChatFactory(InstanceProvider.GetInstance<IAccountScopeFactory>(),
                // InstanceProvider.GetInstance<IAccountsFileManager>()))
                .AddAccountVerificationFactory(InstanceProvider.GetInstance<TdAccountVerificationFactory>())
                .AddProfileFactory(InstanceProvider.GetInstance<TdProfileFactory>());
        }

        public static TdCoreBuilder Instance(INetworkCoreFactory networkCoreFactory, AccessorStrategies strategies)
        {
            return _instance ?? (_instance = new TdCoreBuilder(networkCoreFactory, strategies));
        }

        //     .AddChatFactory(new TDLiveChatFactory(InstanceProvider.GetInstance<IAccountScopeFactory>(),
        // InstanceProvider.GetInstance<IAccountsFileManager>()))
        public INetworkCoreFactory GetTdCoreObjects()
        {
            return NetworkCoreFactory;
        }
    }
}