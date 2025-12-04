using CommonServiceLocator;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDFactories;
using GramDominatorCore.GDLibrary;

namespace GramDominatorUI.GDCoreLibrary
{
    public class GdCoreBuilder : NetworkCoreLibraryBuilder
    {
        private static GdCoreBuilder _instance;

        private GdCoreBuilder(INetworkCoreFactory networkCoreFactory, AccessorStrategies strategies)
            : base(networkCoreFactory)
        {
            GdInitialiser.RegisterModules();

            AddNetwork(SocialNetworks.Instagram)
                .AddAccountFactory(InstanceProvider.GetInstance<IGDAccountUpdateFactory>())
                .AddTabFactory(GdTabHandlerFactory.Instance(strategies))
                .AddAccountCounts(GDAccountCountFactory.Instance)
                .AddAccountUiTools(GdAccountToolsFactory.Instance)
                .AddAccountDbConnection(
                    InstanceProvider.GetInstance<IAccountDatabaseConnection>(SocialNetworks.Instagram.ToString()))
                .AddCampaignDbConnection(
                    InstanceProvider.GetInstance<ICampaignDatabaseConnection>(SocialNetworks.Instagram
                        .ToString()))
                .AddReportFactory(new GdReportFactory())
                .AddViewCampaignFactory(new GdViewCampaignsFactory())
                .AddAccountVerificationFactory(new GdAccountVerificationFactory())
                .AddProfileFactory(new GdProfileFactory())
                .AddChatFactory(new GdLiveChatFactory(InstanceProvider.GetInstance<IAccountScopeFactory>()));
        }

        public static GdCoreBuilder Instance(INetworkCoreFactory networkCoreFactory, AccessorStrategies strategies)
        {
            return _instance ?? (_instance = new GdCoreBuilder(networkCoreFactory, strategies));
        }

        public INetworkCoreFactory GetGDCoreObjects()
        {
            return NetworkCoreFactory;
        }
    }
}