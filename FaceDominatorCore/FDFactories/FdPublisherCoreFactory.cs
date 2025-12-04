using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;

namespace FaceDominatorCore.FDFactories
{
    public class FdPublisherCoreFactory : IPublisherCoreFactory
    {
        public SocialNetworks Network { get; set; }

        public IPublisherJobProcessFactory PublisherJobFactory { get; set; }

        public IPublisherPostScraper PostScraper { get; set; }

    }
}