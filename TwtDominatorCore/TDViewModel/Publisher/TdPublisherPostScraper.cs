using System.Collections.Generic;
using System.Threading;
using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Process;

namespace TwtDominatorCore.TDViewModel.Publisher
{
    public class TdPublisherPostScraper : IPublisherPostScraper
    {
        public PostScraper GetPostScraperLibrary(string CampaignId, CancellationTokenSource campaignCancellationToken,
            PublisherPostFetchModel postFetchModel)
        {
            return new TdPublisherPostDetailsScraper(CampaignId, campaignCancellationToken, postFetchModel);
        }
    }

    public class TdPublisherJobProcessFactory : IPublisherJobProcessFactory
    {
        public PublisherJobProcess Create(string campaignId, string accountId, List<string> groupLists,
            List<string> pageLists, List<PublisherCustomDestinationModel> customDestinationModels,
            bool isPublishOnOwnWall, CancellationTokenSource campaignCancellationToken)
        {
            return new TdPublisherJobProcess(campaignId, accountId, SocialNetworks.Twitter, groupLists, pageLists,
                customDestinationModels, isPublishOnOwnWall, campaignCancellationToken);
        }

        public PublisherJobProcess Create(string campaignId, string campaignName, string accountId,
            SocialNetworks network,
            IEnumerable<PublisherDestinationDetailsModel> destinationDetails,
            CancellationTokenSource campaignCancellationToken)
        {
            return new TdPublisherJobProcess(campaignId, campaignName, accountId, network, destinationDetails,
                campaignCancellationToken);
        }
    }
}