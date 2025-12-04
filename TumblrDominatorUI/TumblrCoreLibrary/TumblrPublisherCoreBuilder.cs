using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using TumblrDominatorCore.ViewModels.Publisher;

namespace TumblrDominatorUI.TumblrCoreLibrary
{
    public class TumblrPublisherCoreBuilder : PublisherCoreLibraryBuilder
    {
        private static TumblrPublisherCoreBuilder _instance;

        private TumblrPublisherCoreBuilder(IPublisherCoreFactory publisherCoreFactory)
            : base(publisherCoreFactory)
        {
            TumblrInitialiser.RegisterModules();
            AddNetwork(SocialNetworks.Tumblr)
                .AddPostScraper(new TumblrPublisherPostScraper())
                .AddPublisherJobFactory(new TumblrPublisherJobProcessFactory());
        }

        public static TumblrPublisherCoreBuilder Instance(IPublisherCoreFactory publisherCoreFactory)
        {
            return _instance ?? (_instance = new TumblrPublisherCoreBuilder(publisherCoreFactory));
        }

        public IPublisherCoreFactory TumblrPublisherCoreObjects()
        {
            return PublisherCoreFactory;
        }
    }
}