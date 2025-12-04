using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;

namespace GramDominatorUI.GDCoreLibrary
{
    public class GdPublisherCoreBuilder : PublisherCoreLibraryBuilder
    {
        private static GdPublisherCoreBuilder _instance;

        private GdPublisherCoreBuilder(IPublisherCoreFactory publisherCoreFactory)
            : base(publisherCoreFactory)
        {
            GdInitialiser.RegisterModules();
            AddNetwork(SocialNetworks.Instagram)
                .AddPublisherJobFactory(new GdPublisherJobProcessFactory());
        }

        public static GdPublisherCoreBuilder Instance(IPublisherCoreFactory publisherCoreFactory)
        {
            return _instance ?? (_instance = new GdPublisherCoreBuilder(publisherCoreFactory));
        }

        public IPublisherCoreFactory GetGdPublisherCoreObjects()
        {
            return PublisherCoreFactory;
        }
    }
}