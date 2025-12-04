using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models.SocioPublisher;
using System.Threading;

namespace RedditDominatorCore.Publisher
{
    public class RdPublisherPostScraper : IPublisherPostScraper
    {
        public PostScraper GetPostScraperLibrary(string CampaignId, CancellationTokenSource campaignCancellationToken,
            PublisherPostFetchModel postFetchModel)
        {
            return new RdPublisherPostDetailScraper(CampaignId, campaignCancellationToken, postFetchModel);
        }
    }
}