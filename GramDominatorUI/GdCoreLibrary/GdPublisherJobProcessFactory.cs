using System.Collections.Generic;
using System.Threading;
using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Process;
using GramDominatorCore.GDLibrary;

namespace GramDominatorUI.GDCoreLibrary
{
    public class GdPublisherJobProcessFactory : IPublisherJobProcessFactory
    {
        public PublisherJobProcess Create(string campaignId, string accountId, List<string> groupLists,
            List<string> pageLists,
            List<PublisherCustomDestinationModel> customDestinationModels, bool isPublishOnOwnWall,
            CancellationTokenSource campaignCancellationToken)
        {
            return new GdPublisherJobProcess(campaignId, accountId, SocialNetworks.Instagram, groupLists, pageLists,
                customDestinationModels, isPublishOnOwnWall, campaignCancellationToken);
        }

        public PublisherJobProcess Create(string campaignId, string campaignName, string accountId,
            SocialNetworks network,
            IEnumerable<PublisherDestinationDetailsModel> destinationDetails,
            CancellationTokenSource campaignCancellationToken)
        {
            return new GdPublisherJobProcess(campaignId, campaignName, accountId, network, destinationDetails,
                campaignCancellationToken);
        }
    }
}