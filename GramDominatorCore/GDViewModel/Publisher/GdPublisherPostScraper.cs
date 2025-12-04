using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models.SocioPublisher;
using System.Threading;

namespace GramDominatorCore.GDViewModel.Publisher
{
    // ReSharper disable once UnusedMember.Global
    public class GdPublisherPostScraper : IPublisherPostScraper
    {
        public PostScraper GetPostScraperLibrary(string CampaignId, CancellationTokenSource campaignCancellationToken, PublisherPostFetchModel postFetchModel) =>
             new GdPublisherPostDetailsScraper(CampaignId, campaignCancellationToken, postFetchModel);
    }
}
