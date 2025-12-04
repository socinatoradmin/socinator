using CommonServiceLocator;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using TumblrDominatorCore.Interface;
using TumblrDominatorCore.TumblrFactory;

namespace TumblrDominatorUI.TumblrCoreLibrary
{
    public class TumblrCoreBuilder : NetworkCoreLibraryBuilder
    {
        private static TumblrCoreBuilder _instance;

        //private TumblrCoreBuilder(INetworkCoreFactory networkCoreFactory) 
        //    : base(networkCoreFactory)
        //{
        //    try
        //    {
        //        TumblrInitialiser.RegisterModules();

        //        AddNetwork(SocialNetworks.Tumblr)
        //            .AddAccountFactory(InstanceProvider.GetInstance<ITumblrAccountUpdateFactory>())
        //            .AddTabFactory(TumblrTabHandlerFactory.Instance(Strategies))
        //            .AddAccountCounts(TumblrAccountCountFactory.Instance)
        //            .AddAccountSelectors(TumblrAccountSelectorFactory.Instance)
        //            .AddAccountUiTools(TumblrAccountToolsFactory.Instance)
        //            .AddAccountDbConnection(InstanceProvider.GetInstance<IAccountDatabaseConnection>(SocialNetworks.Tumblr.ToString()))
        //        .AddCampaignDbConnection(InstanceProvider.GetInstance<ICampaignDatabaseConnection>(SocialNetworks.Tumblr.ToString()))
        //            .AddReportFactory(new TumblrReportFactory())
        //            .AddViewCampaignFactory(new TumblrViewCampaignsFactory());
        //        // .AddGlobalInteractedDetailsFactory(TumblrGlobalInteractionDetails.GetInstance())
        //        // .AddCampaignInteractedDetailsFactory(TumblrCampaignInteractionDetails.GetInstance());
        //    }
        //    catch (Exception ex)
        //    {
        //        // ignored
        //    }
        //}


        private TumblrCoreBuilder(INetworkCoreFactory networkCoreFactory, AccessorStrategies strategies)
            : base(networkCoreFactory)
        {
            TumblrInitialiser.RegisterModules();
            InstanceProvider.GetInstance<ITumblrAccountSession>();
            AddNetwork(SocialNetworks.Tumblr)
                .AddAccountFactory(InstanceProvider.GetInstance<ITumblrAccountUpdateFactory>())
                .AddTabFactory(TumblrTabHandlerFactory.Instance(strategies))
                .AddAccountCounts(TumblrAccountCountFactory.Instance)
                .AddAccountUiTools(TumblrAccountToolsFactory.Instance)
                .AddAccountSelectors(TumblrAccountSelectorFactory.Instance)
                .AddAccountDbConnection(
                    InstanceProvider.GetInstance<IAccountDatabaseConnection>(SocialNetworks.Tumblr.ToString()))
                .AddCampaignDbConnection(
                    InstanceProvider.GetInstance<ICampaignDatabaseConnection>(SocialNetworks.Tumblr.ToString()))
                .AddReportFactory(new TumblrReportFactory())
                .AddViewCampaignFactory(new TumblrViewCampaignsFactory());
        }

        // ReSharper disable once UnassignedGetOnlyAutoProperty
        public static AccessorStrategies Strategies { get; }

        public static TumblrCoreBuilder Instance(INetworkCoreFactory networkCoreFactory)
        {
            return _instance ?? (_instance = new TumblrCoreBuilder(networkCoreFactory, Strategies));
        }

        public INetworkCoreFactory GetTumblrCoreObjects()
        {
            return NetworkCoreFactory;
        }
    }
}