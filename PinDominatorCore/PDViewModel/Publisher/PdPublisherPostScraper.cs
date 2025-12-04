using System.Collections.Generic;
using System.Threading;
using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Process;
using PinDominatorCore.PDLibrary;

namespace PinDominatorCore.PDViewModel.Publisher
{
    public class PdPublisherPostScraper : IPublisherPostScraper
    {
        public PostScraper GetPostScraperLibrary(string campaignId, CancellationTokenSource campaignCancellationToken, PublisherPostFetchModel postFetchModel)
            => new PdPublisherPostDetailsScraper(campaignId, campaignCancellationToken, postFetchModel);

    }

    public class PdPublisherJobProcessFactory : IPublisherJobProcessFactory
    {
        public PublisherJobProcess Create(string campaignId, string campaignName, string accountId,
            SocialNetworks network,
            IEnumerable<PublisherDestinationDetailsModel> destinationDetails,
            CancellationTokenSource campaignCancellationToken)
        {
            return new PdPublisherJobProcess(campaignId, campaignName, accountId, network, destinationDetails,
                campaignCancellationToken);
        }

        public PublisherJobProcess Create(string campaignId, string accountId, List<string> groupLists,
            List<string> pageLists, List<PublisherCustomDestinationModel> customDestinationModels,
            bool isPublishOnOwnWall, CancellationTokenSource campaignCancellationToken)
        {
            return new PdPublisherJobProcess(campaignId, accountId, SocialNetworks.Pinterest, groupLists, pageLists,
                customDestinationModels, isPublishOnOwnWall, campaignCancellationToken);
        }
    }
}