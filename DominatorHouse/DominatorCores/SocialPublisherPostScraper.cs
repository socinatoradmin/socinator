using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models.SocioPublisher;
using System.Threading;

namespace DominatorHouse.DominatorCores
{
    public class SocialPublisherPostScraper : IPublisherPostScraper
    {
        public PostScraper GetPostScraperLibrary(string CampaignId, CancellationTokenSource campaignCancellationToken, PublisherPostFetchModel postFetchModel) =>
            new SocialPublisherPostDetailsScraper(CampaignId, campaignCancellationToken, postFetchModel); 

    }
}