using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;

namespace TwtDominatorCore.Factories
{
    public class TdPublisherCoreFactory : IPublisherCoreFactory
    {
        public SocialNetworks Network { get; set; }

        public IPublisherJobProcessFactory PublisherJobFactory { get; set; }

        public IPublisherPostScraper PostScraper { get; set; }
    }
}