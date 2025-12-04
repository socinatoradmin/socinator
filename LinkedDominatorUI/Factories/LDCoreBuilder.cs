using CommonServiceLocator;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using LinkedDominatorCore.Factories;
using LinkedDominatorCore.Interfaces;
using LinkedDominatorUI.LDCoreLibrary;

namespace LinkedDominatorUI.Factories
{
    public class LDCoreBuilder : NetworkCoreLibraryBuilder
    {
        private static LDCoreBuilder _instance;


        private LDCoreBuilder(INetworkCoreFactory networkCoreFactory, AccessorStrategies strategies)
            : base(networkCoreFactory)
        {
            LdInitialiser.RegisterModules();

            InstanceProvider.GetInstance<ILDAccountSessionManager>();
            AddNetwork(SocialNetworks.LinkedIn)
                .AddAccountFactory(InstanceProvider.GetInstance<ILdAccountUpdateFactory>())
                .AddTabFactory(LDTabHandlerFactory.Instance(strategies))
                .AddAccountCounts(LDAccountCountFactory.Instance)
                .AddAccountUiTools(LDAccountToolsFactory.Instance)
                .AddAccountSelectors(LDAccountSelectorFactory.Instance)
                .AddAccountDbConnection(
                    InstanceProvider.GetInstance<IAccountDatabaseConnection>(SocialNetworks.LinkedIn.ToString()))
                .AddCampaignDbConnection(
                    InstanceProvider.GetInstance<ICampaignDatabaseConnection>(SocialNetworks.LinkedIn.ToString()))
                .AddReportFactory(new LdReportFactory())
                .AddViewCampaignFactory(new LdViewCampaignsFactory())
                .AddChatFactory(new LDLiveChatFactory(InstanceProvider.GetInstance<IAccountScopeFactory>()));
        }

        public static LDCoreBuilder Instance(INetworkCoreFactory networkCoreFactory, AccessorStrategies strategies)
        {
            return _instance ?? (_instance = new LDCoreBuilder(networkCoreFactory, strategies));
        }

        public INetworkCoreFactory GetLdCoreObjects()
        {
            return NetworkCoreFactory;
        }
    }
}