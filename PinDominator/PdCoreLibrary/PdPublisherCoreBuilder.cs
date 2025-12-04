using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using PinDominatorCore.PDViewModel.Publisher;

namespace PinDominator.PdCoreLibrary
{
    public class PdPublisherCoreBuilder : PublisherCoreLibraryBuilder
    {
        private static PdPublisherCoreBuilder _instance;

        private PdPublisherCoreBuilder(IPublisherCoreFactory publisherCoreFactory)
            : base(publisherCoreFactory)
        {
            PdInitializer.RegisterModules();
            AddNetwork(SocialNetworks.Pinterest)
                .AddPostScraper(new PdPublisherPostScraper())
                .AddPublisherJobFactory(new PdPublisherJobProcessFactory());
        }

        public static PdPublisherCoreBuilder Instance(IPublisherCoreFactory publisherCoreFactory)
        {
            return _instance ?? (_instance = new PdPublisherCoreBuilder(publisherCoreFactory));
        }

        public IPublisherCoreFactory GetPdPublisherCoreObjects()
        {
            return PublisherCoreFactory;
        }
    }
}