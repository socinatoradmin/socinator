using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models.SocioPublisher;
using System.Threading;

namespace FaceDominatorCore.FDViewModel.Publisher
{
    public class FdPublisherPostScraper : IPublisherPostScraper
    {
        public PostScraper GetPostScraperLibrary(string CampaignId, CancellationTokenSource campaignCancellationToken, PublisherPostFetchModel postFetchModel) =>
            new FdPublisherPostDetailsScraper(CampaignId, campaignCancellationToken, postFetchModel);
    }
}