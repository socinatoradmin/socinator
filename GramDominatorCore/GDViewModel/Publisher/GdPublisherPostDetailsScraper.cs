using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models.SocioPublisher;
using System.Threading;

namespace GramDominatorCore.GDViewModel.Publisher
{
    public class GdPublisherPostDetailsScraper : PostScraper
    {

        public GdPublisherPostDetailsScraper(string CampaignId, CancellationTokenSource campaignCancellationToken, PublisherPostFetchModel postFetchModel) :
            base(CampaignId, campaignCancellationToken, postFetchModel)
        {

        }
        //public override void ScrapePosts(string accountId, string campaignId, ScrapePostModel scrapePostDetails, int count = 10)
        //{
        //    if (string.IsNullOrEmpty(accountId) || string.IsNullOrEmpty(campaignId) || scrapePostDetails == null)
        //        return;

        //    base.ScrapePosts(accountId, campaignId, scrapePostDetails, count);


        //    PostlistFileManager.Add(campaignId, new PublisherPostlistModel());
        //}

    }
}
