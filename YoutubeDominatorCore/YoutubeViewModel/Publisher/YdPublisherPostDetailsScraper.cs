using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models.SocioPublisher;
using System.Threading;

namespace YoutubeDominatorCore.YoutubeViewModel.Publisher
{
    public class YdPublisherPostDetailsScraper : PostScraper
    {
        public YdPublisherPostDetailsScraper(string CampaignId, CancellationTokenSource campaignCancellationToken,
            PublisherPostFetchModel postFetchModel) :
            base(CampaignId, campaignCancellationToken, postFetchModel)
        {
        }
    }
}