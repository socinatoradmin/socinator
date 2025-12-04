using System.Threading;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models.SocioPublisher;

namespace QuoraDominatorCore.ViewModel.Publisher
{
    public class QdPublisherPostScraper : IPublisherPostScraper
    {
        public PostScraper GetPostScraperLibrary(string CampaignId, CancellationTokenSource campaignCancellationToken,
            PublisherPostFetchModel postFetchModel)
        {
            return new QdPublisherPostDetailsScraper(CampaignId, campaignCancellationToken, postFetchModel);
        }
    }
}