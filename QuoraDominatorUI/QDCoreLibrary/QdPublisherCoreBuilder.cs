using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using QuoraDominatorCore.QDFactories;
using QuoraDominatorCore.ViewModel.Publisher;
using QuoraDominatorUI.Utility;

namespace QuoraDominatorUI.QDCoreLibrary
{
    internal class QdPublisherCoreBuilder : PublisherCoreLibraryBuilder
    {
        private static QdPublisherCoreBuilder _instance;

        private QdPublisherCoreBuilder(IPublisherCoreFactory publisherCoreFactory)
            : base(publisherCoreFactory)
        {
            QdInitializer.RegisterModules();
            AddNetwork(SocialNetworks.Quora)
                .AddPostScraper(new QdPublisherPostScraper())
                .AddPublisherJobFactory(new QdPublisherJobProcessFactory());
        }

        public static QdPublisherCoreBuilder Instance(IPublisherCoreFactory publisherCoreFactory)
        {
            return _instance ?? (_instance = new QdPublisherCoreBuilder(publisherCoreFactory));
        }

        public IPublisherCoreFactory GetQdPublisherCoreObjects()
        {
            return PublisherCoreFactory;
        }
    }
}