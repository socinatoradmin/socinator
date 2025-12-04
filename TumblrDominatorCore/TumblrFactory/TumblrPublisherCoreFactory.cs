using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;

namespace TumblrDominatorCore.TumblrFactory
{
    public class TumblrPublisherCoreFactory : IPublisherCoreFactory
    {
        public SocialNetworks Network { get; set; }

        public IPublisherJobProcessFactory PublisherJobFactory { get; set; }

        public IPublisherPostScraper PostScraper { get; set; }
    }
}