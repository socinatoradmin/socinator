using CommonServiceLocator;
using ThreadUtils;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using LinkedDominatorCore.Factories;
using LinkedDominatorUI.LDCoreLibrary;
using DominatorHouseCore.Utility;

namespace LinkedDominatorUI.Factories
{
    public class LDPublisherCoreBuilder : PublisherCoreLibraryBuilder
    {
        private static LDPublisherCoreBuilder _instance;

        private LDPublisherCoreBuilder(IPublisherCoreFactory publisherCoreFactory)
            : base(publisherCoreFactory)
        {
            LdInitialiser.RegisterModules();
            AddNetwork(SocialNetworks.LinkedIn)
                .AddPostScraper(new LdPublisherPostScraper())
                .AddPublisherJobFactory(
                    new LDPublisherJobProcessFactory(InstanceProvider.GetInstance<IDelayService>()));
            //.AddPublishingPost(new LDPublishingPost());
        }

        public static LDPublisherCoreBuilder Instance(IPublisherCoreFactory publisherCoreFactory)
        {
            return _instance ?? (_instance = new LDPublisherCoreBuilder(publisherCoreFactory));
        }

        public IPublisherCoreFactory GetLDPublisherCoreObjects()
        {
            return PublisherCoreFactory;
        }
    }
}