using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using RedditDominatorCore.Publisher;

namespace RedditDominatorUI.RdCoreLibrary
{
    public class RdPublisherCoreBuilder : PublisherCoreLibraryBuilder
    {
        private static RdPublisherCoreBuilder _instance;

        private RdPublisherCoreBuilder(IPublisherCoreFactory publisherCoreFactory)
            : base(publisherCoreFactory)
        {
            RdInitializer.RegisterModules();
            AddNetwork(SocialNetworks.Reddit)
                .AddPostScraper(new RdPublisherPostScraper())
                .AddPublisherJobFactory(new RdPublisherJobProcessFactory());
        }

        public static RdPublisherCoreBuilder Instance(IPublisherCoreFactory publisherCoreFactory)
        {
            return _instance ?? (_instance = new RdPublisherCoreBuilder(publisherCoreFactory));
        }

        public IPublisherCoreFactory GetRdPublisherCoreObjects()
        {
            return PublisherCoreFactory;
        }
    }
}