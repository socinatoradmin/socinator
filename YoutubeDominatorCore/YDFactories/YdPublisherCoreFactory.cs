using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;

namespace YoutubeDominatorCore.YDFactories
{
    public class YdPublisherCoreFactory : IPublisherCoreFactory
    {
        public SocialNetworks Network { get; set; }

        public IPublisherPostScraper PostScraper { get; set; }

        public IPublisherJobProcessFactory PublisherJobFactory { get; set; }
    }
}