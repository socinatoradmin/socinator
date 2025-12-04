using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Process;
using System.Collections.Generic;
using System.Threading;

namespace FaceDominatorCore.FDFactories
{
    public class FdPublisherJobProcessFactory : IPublisherJobProcessFactory
    {
        //public PublisherJobProcess Create(string campaignId, string accountId, List<string> groupLists, List<string> pageLists,
        //    bool isPublishOnOwnWall, CancellationTokenSource campaignCancellationToken) => new FdPublisherJobProcess(campaignId,  accountId,SocialNetworks.Facebook,  groupLists, pageLists,isPublishOnOwnWall, campaignCancellationToken);

        public PublisherJobProcess Create(string campaignId, string accountId, List<string> groupLists, List<string> pageLists, List<PublisherCustomDestinationModel> customDestinationModels, bool isPublishOnOwnWall, CancellationTokenSource campaignCancellationToken)
        => new FdPublisherJobProcess(campaignId, accountId, SocialNetworks.Facebook, groupLists, pageLists, customDestinationModels, isPublishOnOwnWall, campaignCancellationToken);

        public PublisherJobProcess Create(string campaignId, string campaignName, string accountId,
            SocialNetworks network,
            IEnumerable<PublisherDestinationDetailsModel> destinationDetails,
            CancellationTokenSource campaignCancellationToken)
            => new FdPublisherJobProcess(campaignId, campaignName, accountId, network, destinationDetails,
                campaignCancellationToken);
    }
}