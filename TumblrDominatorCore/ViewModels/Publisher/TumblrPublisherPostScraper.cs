using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Process;
using System.Collections.Generic;
using System.Threading;
using TumblrDominatorCore.TumblrLibrary.TumblrProcesses;

namespace TumblrDominatorCore.ViewModels.Publisher
{
    public class TumblrPublisherPostScraper : IPublisherPostScraper
    {
        public PostScraper GetPostScraperLibrary(string CampaignId, CancellationTokenSource campaignCancellationToken,
            PublisherPostFetchModel postFetchModel)
        {
            return new TumblrPublisherPostDetailsScraper(CampaignId, campaignCancellationToken, postFetchModel);
        }
    }


    public class TumblrPublisherJobProcessFactory : IPublisherJobProcessFactory
    {
        public PublisherJobProcess Create(string campaignId, string accountId, List<string> groupLists,
            List<string> pageLists, List<PublisherCustomDestinationModel> customDestinationModels,
            bool isPublishOnOwnWall, CancellationTokenSource campaignCancellationToken)
        {
            return new TumblrPublisherJobProcess(campaignId, accountId, SocialNetworks.Tumblr, groupLists, pageLists,
                customDestinationModels, isPublishOnOwnWall, campaignCancellationToken);
        }

        public PublisherJobProcess Create(string campaignId, string campaignName, string accountId,
            SocialNetworks network,
            IEnumerable<PublisherDestinationDetailsModel> destinationDetails,
            CancellationTokenSource campaignCancellationToken)
        {
            return new TumblrPublisherJobProcess(campaignId, campaignName, accountId, network, destinationDetails,
                campaignCancellationToken);
        }
    }
}