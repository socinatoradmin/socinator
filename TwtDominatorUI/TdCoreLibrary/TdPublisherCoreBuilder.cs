using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using TwtDominatorCore.TDViewModel.Publisher;

namespace TwtDominatorUI.TdCoreLibrary
{
    internal class TdPublisherCoreBuilder : PublisherCoreLibraryBuilder
    {
        private static TdPublisherCoreBuilder _instance;

        private TdPublisherCoreBuilder(IPublisherCoreFactory publisherCoreFactory)
            : base(publisherCoreFactory)
        {
            TDInitialiser.RegisterModules();
            AddNetwork(SocialNetworks.Twitter)
                .AddPostScraper(new TdPublisherPostScraper())
                .AddPublisherJobFactory(new TdPublisherJobProcessFactory());
            //.AddPublishingPost(new TdPublishingPost());
        }

        public static TdPublisherCoreBuilder Instance(IPublisherCoreFactory publisherCoreFactory)
        {
            return _instance ?? (_instance = new TdPublisherCoreBuilder(publisherCoreFactory));
        }

        public IPublisherCoreFactory GetTdPublisherCoreObjects()
        {
            return PublisherCoreFactory;
        }
    }
}