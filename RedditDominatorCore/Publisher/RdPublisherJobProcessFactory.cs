using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Process;
using System.Collections.Generic;
using System.Threading;

namespace RedditDominatorCore.Publisher
{
    public class RdPublisherJobProcessFactory : IPublisherJobProcessFactory
    {
        public PublisherJobProcess Create(string campaignId, string campaignName, string accountId,
            SocialNetworks network, IEnumerable<PublisherDestinationDetailsModel> destinationDetails,
            CancellationTokenSource campaignCancellationToken)
        {
            return new RdPublisherJobProcess(campaignId, campaignName, accountId, network, destinationDetails,
                campaignCancellationToken);
        }


        public PublisherJobProcess Create(string campaignId, string accountId, List<string> groupLists,
            List<string> pageLists,
            List<PublisherCustomDestinationModel> customDestinationModels, bool isPublishOnOwnWall,
            CancellationTokenSource campaignCancellationToken)
        {
            return new RdPublisherJobProcess(campaignId, accountId, SocialNetworks.Reddit, groupLists, pageLists,
                customDestinationModels, isPublishOnOwnWall, campaignCancellationToken);
        }
    }
}