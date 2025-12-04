using CommonServiceLocator;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using PinDominatorCore.Interface;
using PinDominatorCore.PDFactories;

namespace PinDominator.PdCoreLibrary
{
    public class PdCoreBuilder : NetworkCoreLibraryBuilder
    {
        private static PdCoreBuilder _instance;

        private PdCoreBuilder(INetworkCoreFactory networkCoreFactory, AccessorStrategies strategies) : base(
            networkCoreFactory)
        {
            PdInitializer.RegisterModules();
            InstanceProvider.GetInstance<IPDAccountSessionManager>();
            AddNetwork(SocialNetworks.Pinterest)
                .AddAccountFactory(InstanceProvider.GetInstance<IPdAccountUpdateFactory>())
                .AddTabFactory(PdTabHandlerFactory.Instance(strategies))
                .AddAccountCounts(PdAccountCountFactory.Instance)
                .AddAccountUiTools(PdAccountToolsFactory.Instance)
                .AddAccountSelectors(PdAccountSelectorFactory.Instance)
                .AddAccountDbConnection(
                    InstanceProvider.GetInstance<IAccountDatabaseConnection>(SocialNetworks.Pinterest.ToString()))
                .AddCampaignDbConnection(
                    InstanceProvider.GetInstance<ICampaignDatabaseConnection>(SocialNetworks.Pinterest
                        .ToString()))
                .AddReportFactory(new PdReportFactory())
                .AddViewCampaignFactory(new PdViewCampaignsFactory())
                .AddAccountVerificationFactory(new PdResetAccountPasswordFactory());
            //.AddGlobalInteractedDetailsFactory(PdGlobalInteractionDetails.GetInstance())
            //.AddCampaignInteractedDetailsFactory(PdCampaignInteractionDetails.GetInstance());
        }

        public static PdCoreBuilder Instance(INetworkCoreFactory networkCoreFactory, AccessorStrategies strategies)
        {
            return _instance ?? (_instance = new PdCoreBuilder(networkCoreFactory, strategies));
        }

        public INetworkCoreFactory GetPdCoreObjects()
        {
            return NetworkCoreFactory;
        }
    }
}