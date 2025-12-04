using System.Collections.Generic;
using System.Threading;
using ThreadUtils;
using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Process;
using LinkedDominatorCore.LDLibrary.Publisher;

namespace LinkedDominatorCore.Factories
{
    //LDPublisherPostScraper
    public class LdPublisherPostScraper : IPublisherPostScraper
    {
        public PostScraper GetPostScraperLibrary(string CampaignId, CancellationTokenSource campaignCancellationToken,
            PublisherPostFetchModel postFetchModel)
        {
            return new LDPublisherPostDetailsScraper(CampaignId, campaignCancellationToken, postFetchModel);
        }
    }


    public class LDPublisherJobProcessFactory : IPublisherJobProcessFactory
    {
        private readonly IDelayService _delayService;

        public LDPublisherJobProcessFactory(IDelayService delayService)
        {
            _delayService = delayService;
        }

        public PublisherJobProcess Create(string campaignId, string accountId, List<string> groupLists,
            List<string> pageLists,
            List<PublisherCustomDestinationModel> customDestinationModels, bool isPublishOnOwnWall,
            CancellationTokenSource campaignCancellationToken)
        {
            return new LDPublisherJobProcess(campaignId, accountId, SocialNetworks.LinkedIn, groupLists,
                pageLists, customDestinationModels, isPublishOnOwnWall, campaignCancellationToken, _delayService);
        }

        public PublisherJobProcess Create(string campaignId, string campaignName, string accountId,
            SocialNetworks network, IEnumerable<PublisherDestinationDetailsModel> destinationDetails,
            CancellationTokenSource campaignCancellationToken)
        {
            return new LDPublisherJobProcess(campaignId, campaignName, accountId, network, destinationDetails,
                campaignCancellationToken, _delayService);
        }
    }
}