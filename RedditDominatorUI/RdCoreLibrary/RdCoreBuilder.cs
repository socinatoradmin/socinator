using CommonServiceLocator;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using RedditDominatorCore.Interface;
using RedditDominatorCore.RDFactories;

namespace RedditDominatorUI.RdCoreLibrary
{
    public class RdCoreBuilder : NetworkCoreLibraryBuilder
    {
        private static RdCoreBuilder _instance;

        private RdCoreBuilder(INetworkCoreFactory networkCoreFactory, AccessorStrategies strategies)
            : base(networkCoreFactory)
        {
            RdInitializer.RegisterModules();
            InstanceProvider.GetInstance<IRDAccountSessionManager>();
            AddNetwork(SocialNetworks.Reddit)
                .AddAccountFactory(InstanceProvider.GetInstance<IRdAccountUpdateFactory>())
                .AddTabFactory(RdTabHandlerFactory.Instance(strategies))
                .AddAccountCounts(RdAccountCountFactory.Instance)
                .AddAccountUiTools(RdAccountToolsFactory.Instance)
                .AddAccountSelectors(RdAccountSelectorFactory.Instance)
                .AddAccountDbConnection(
                    InstanceProvider.GetInstance<IAccountDatabaseConnection>(SocialNetworks.Reddit.ToString()))
                .AddCampaignDbConnection(
                    InstanceProvider.GetInstance<ICampaignDatabaseConnection>(SocialNetworks.Reddit.ToString()))
                .AddReportFactory(new RdReportFactory())
                .AddViewCampaignFactory(new RdViewCampaignsFactory());
        }

        public static RdCoreBuilder Instance(INetworkCoreFactory networkCoreFactory, AccessorStrategies strategies)
        {
            return _instance ?? (_instance = new RdCoreBuilder(networkCoreFactory, strategies));
        }

        public INetworkCoreFactory GetRdCoreObjects()
        {
            return NetworkCoreFactory;
        }
    }
}