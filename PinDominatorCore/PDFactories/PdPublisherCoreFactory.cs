using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;

namespace PinDominatorCore.PDFactories
{
    public class PdPublisherCoreFactory : IPublisherCoreFactory
    {
        public SocialNetworks Network { get; set; }

        public IPublisherPostScraper PostScraper { get; set; }

        public IPublisherJobProcessFactory PublisherJobFactory { get; set; }

    }
}