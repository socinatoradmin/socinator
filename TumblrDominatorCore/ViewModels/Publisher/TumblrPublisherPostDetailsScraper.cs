using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models.SocioPublisher;
using System.Threading;

namespace TumblrDominatorCore.ViewModels.Publisher
{
    public class TumblrPublisherPostDetailsScraper : PostScraper
    {
        public TumblrPublisherPostDetailsScraper(string CampaignId, CancellationTokenSource campaignCancellationToken,
            PublisherPostFetchModel postFetchModel) :
            base(CampaignId, campaignCancellationToken, postFetchModel)
        {
        }
    }
}