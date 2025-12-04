using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using FaceDominatorCore.FDFactories;
using FaceDominatorCore.FDViewModel.Publisher;

namespace FaceDominatorUI.FdCoreLibrary
{
    public class FdPublisherCoreBuilder : PublisherCoreLibraryBuilder
    {
        private static FdPublisherCoreBuilder _instance;

        private FdPublisherCoreBuilder(IPublisherCoreFactory publisherCoreFactory)
            : base(publisherCoreFactory)
        {
            FdInitialiser.RegisterModules();
            AddNetwork(SocialNetworks.Facebook)
                .AddPostScraper(new FdPublisherPostScraper())
                .AddPublisherJobFactory(new FdPublisherJobProcessFactory());
        }

        public static FdPublisherCoreBuilder Instance(IPublisherCoreFactory publisherCoreFactory)
        {
            return _instance ?? (_instance = new FdPublisherCoreBuilder(publisherCoreFactory));
        }

        public IPublisherCoreFactory GetFdPublisherCoreObjects()
        {
            return PublisherCoreFactory;
        }
    }
}