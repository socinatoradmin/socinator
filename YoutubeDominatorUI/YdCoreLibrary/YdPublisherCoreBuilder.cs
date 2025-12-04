using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using YoutubeDominatorCore.YoutubeViewModel.Publisher;

namespace YoutubeDominatorUI.YdCoreLibrary
{
    public class YdPublisherCoreBuilder : PublisherCoreLibraryBuilder
    {
        private static YdPublisherCoreBuilder _instance;

        private YdPublisherCoreBuilder(IPublisherCoreFactory publisherCoreFactory)
            : base(publisherCoreFactory)
        {
            YdInitialiser.RegisterModules();
            AddNetwork(SocialNetworks.YouTube)
                .AddPostScraper(new YdPublisherPostScraper())
                .AddPublisherJobFactory(new YdPublisherJobProcessFactory());
        }

        public static YdPublisherCoreBuilder Instance(IPublisherCoreFactory publisherCoreFactory)
        {
            return _instance ?? (_instance = new YdPublisherCoreBuilder(publisherCoreFactory));
        }

        public IPublisherCoreFactory GetPdPublisherCoreObjects()
        {
            return PublisherCoreFactory;
        }
    }
}