using CommonServiceLocator;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDFactories;
using FaceDominatorCore.FDLibrary.FdFunctions.FdBrowserManager;
using ThreadUtils;

namespace FaceDominatorUI.FdCoreLibrary
{
    public class FdCoreBuilder : NetworkCoreLibraryBuilder
    {
        private static FdCoreBuilder _instance;

        private FdCoreBuilder(INetworkCoreFactory networkCoreFactory, AccessorStrategies strategies)
            : base(networkCoreFactory)
        {
            FdInitialiser.RegisterModules();

            AddNetwork(SocialNetworks.Facebook)
                .AddAccountFactory(InstanceProvider.GetInstance<IFdAccountUpdateFactory>())
                .AddTabFactory(FdTabHandlerFactory.Instance(strategies))
                .AddAccountCounts(FdAccountCountFactory.Instance)
                .AddAccountUiTools(FdAccountToolsFactory.Instance)
                .AddAccountSelectors(FdAccountSelectorFactory.Instance)
                .AddAccountDbConnection(
                    InstanceProvider.GetInstance<IAccountDatabaseConnection>(SocialNetworks.Facebook.ToString()))
                .AddCampaignDbConnection(
                    InstanceProvider.GetInstance<ICampaignDatabaseConnection>(SocialNetworks.Facebook.ToString()))
                .AddReportFactory(new FdReportFactory())
                .AddViewCampaignFactory(new FdViewCampaignsFactory())
                //.AddCampaignInteractedDetailsFactory(FdCampaignInteractionDetails.GetInstance())
                //.AddGlobalInteractedDetailsFactory(FdGlobalInteractionDetails.GetInstance())
                .AddChatFactory(new FdChatFactory(InstanceProvider.GetInstance<IAccountScopeFactory>(),
                    InstanceProvider.GetInstance<IDelayService>(),
                    InstanceProvider.GetInstance<IFdBrowserManager>()))
                .AddProfileFactory(InstanceProvider.GetInstance<FdProfileFactory>());
            ////.AddProfileFactory(FdProfileaFactory.Instance);
        }

        public static FdCoreBuilder Instance(INetworkCoreFactory networkCoreFactory, AccessorStrategies strategies)
        {
            return _instance ?? (_instance = new FdCoreBuilder(networkCoreFactory, strategies));
        }

        public INetworkCoreFactory GetFdCoreObjects()
        {
            return NetworkCoreFactory;
        }
    }
}