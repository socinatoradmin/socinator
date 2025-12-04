using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;

namespace DominatorHouse.DominatorCores
{
    public class SocialPublisherCoreBuilder : PublisherCoreLibraryBuilder
    {
        private static SocialPublisherCoreBuilder _instance;

        public static SocialPublisherCoreBuilder Instance(IPublisherCoreFactory publisherCoreFactory)
            => _instance ?? (_instance = new SocialPublisherCoreBuilder(publisherCoreFactory));

        private SocialPublisherCoreBuilder(IPublisherCoreFactory publisherCoreFactory)
            : base(publisherCoreFactory)
        {
            AddNetwork(SocialNetworks.Social)
                .AddPostScraper(new SocialPublisherPostScraper());
        }

        public IPublisherCoreFactory GetSocialPublisherCoreObjects() => PublisherCoreFactory;
    }
}