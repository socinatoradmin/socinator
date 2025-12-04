using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;

namespace TwtDominatorUI.Factories
{
    public class TdPublisherCoreFactory : IPublisherCoreFactory
    {
        public SocialNetworks Network { get; set; }

        public IPublisherJobProcessFactory PublisherJobFactory { get; set; }

        public IPublisherPostScraper PostScraper { get; set; }

        public IPublishingPost PublishingPost { get; set; }
    }
}