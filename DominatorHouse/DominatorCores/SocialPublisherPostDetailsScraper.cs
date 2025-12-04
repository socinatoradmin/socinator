using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models.SocioPublisher;
using System.Threading;

namespace DominatorHouse.DominatorCores
{
    public class SocialPublisherPostDetailsScraper : PostScraper
    {
        public SocialPublisherPostDetailsScraper(string CampaignId, CancellationTokenSource campaignCancellationToken, PublisherPostFetchModel postFetchModel) :
            base(CampaignId, campaignCancellationToken, postFetchModel)
        {

        }
    }
}