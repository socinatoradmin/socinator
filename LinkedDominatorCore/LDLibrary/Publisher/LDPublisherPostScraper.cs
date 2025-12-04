using DominatorHouseCore.Interfaces;

namespace LinkedDominatorCore.LDLibrary.Publisher
{
    public class LDPublisherPostScraper : IPublisherPostScraper
    {
        public PostScraper GetPostScraperLibrary() => new LDPublisherPostDetailsScraper();
    }

}
