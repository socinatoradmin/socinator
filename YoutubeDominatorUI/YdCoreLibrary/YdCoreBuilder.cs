using CommonServiceLocator;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using YoutubeDominatorCore.YDFactories;

namespace YoutubeDominatorUI.YdCoreLibrary
{
    public class YdCoreBuilder : NetworkCoreLibraryBuilder
    {
        private static YdCoreBuilder _instance;

        private YdCoreBuilder(INetworkCoreFactory networkCoreFactory, AccessorStrategies strategies)
            : base(networkCoreFactory)
        {
            YdInitialiser.RegisterModules();

            try
            {
                AddNetwork(SocialNetworks.YouTube)
                    .AddAccountFactory(InstanceProvider.GetInstance<IYdAccountUpdateFactory>())
                    .AddTabFactory(YdTabHandlerFactory.Instance(strategies))
                    .AddAccountCounts(YdAccountCountFactory.Instance)
                    .AddAccountUiTools(YdAccountToolsFactory.Instance)
                    .AddAccountSelectors(YdAccountSelectorFactory.Instance)
                    .AddAccountDbConnection(
                        InstanceProvider.GetInstance<IAccountDatabaseConnection>(
                            SocialNetworks.YouTube.ToString()))
                    .AddCampaignDbConnection(
                        InstanceProvider.GetInstance<ICampaignDatabaseConnection>(
                            SocialNetworks.YouTube.ToString()))
                    .AddReportFactory(new YdReportFactory())
                    .AddViewCampaignFactory(new YdViewCampaignsFactory())
                    .AddAccountVerificationFactory(InstanceProvider.GetInstance<YdAccountVerificationFactory>());
            }
            catch
            {
            }
        }

        public static YdCoreBuilder Instance(INetworkCoreFactory networkCoreFactory, AccessorStrategies strategies)
        {
            return _instance ?? (_instance = new YdCoreBuilder(networkCoreFactory, strategies));
        }

        public INetworkCoreFactory GetYdCoreObjects()
        {
            return NetworkCoreFactory;
        }
    }
}