using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Process;
using System.Collections.Generic;
using System.Threading;
using YoutubeDominatorCore.YoutubeLibrary.Processes;

namespace YoutubeDominatorCore.YoutubeViewModel.Publisher
{
    public class YdPublisherPostScraper : IPublisherPostScraper
    {
        public PostScraper GetPostScraperLibrary(string CampaignId, CancellationTokenSource campaignCancellationToken,
            PublisherPostFetchModel postFetchModel)
        {
            return new YdPublisherPostDetailsScraper(CampaignId, campaignCancellationToken, postFetchModel);
        }
    }

    public class YdPublisherJobProcessFactory : IPublisherJobProcessFactory
    {
        public PublisherJobProcess Create(string campaignId, string campaignName, string accountId,
            SocialNetworks network,
            IEnumerable<PublisherDestinationDetailsModel> destinationDetails,
            CancellationTokenSource campaignCancellationToken)
        {
            return new YdPublisherJobProcess(campaignId, campaignName, accountId, network, destinationDetails,
                campaignCancellationToken);
        }

        public PublisherJobProcess Create(string campaignId, string accountId, List<string> groupLists,
            List<string> pageLists, List<PublisherCustomDestinationModel> customDestinationModels,
            bool isPublishOnOwnWall, CancellationTokenSource campaignCancellationToken)
        {
            return new YdPublisherJobProcess(campaignId, accountId, SocialNetworks.YouTube, groupLists, pageLists,
                customDestinationModels, isPublishOnOwnWall, campaignCancellationToken);
        }
    }
}