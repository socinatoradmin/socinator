#region

using System.Collections.Generic;
using System.Threading;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Process;

#endregion

namespace DominatorHouseCore.BusinessLogic.Scheduler
{
    public interface IPublisherJobProcessFactory
    {
        /// <summary>
        ///     To create the Publisher job process objects from each networks
        /// </summary>
        /// <param name="campaignId">Campaign Id for job process resposible</param>
        /// <param name="accountId">Account Id which is running currently</param>
        /// <param name="groupLists">groups destinations</param>
        /// <param name="pageLists">pages destinations</param>
        /// <param name="customDestinationModels">custom destinations</param>
        /// <param name="isPublishOnOwnWall">Is need to publish on own wall</param>
        /// <param name="campaignCancellationToken">Cancellation token</param>
        /// <returns>
        ///     <see cref="PublisherJobProcess" />
        /// </returns>
        PublisherJobProcess Create(string campaignId,
            string accountId,
            List<string> groupLists,
            List<string> pageLists,
            List<PublisherCustomDestinationModel> customDestinationModels,
            bool isPublishOnOwnWall,
            CancellationTokenSource campaignCancellationToken);


        /// <summary>
        ///     To create the Publisher job process objects from each networks
        /// </summary>
        /// <param name="campaignId">Campaign Id</param>
        /// <param name="campaignName">Campaign Name</param>
        /// <param name="accountId">Account's Id</param>
        /// <param name="network">SocialNetwork</param>
        /// <param name="destinationDetails">Destination with Post Details</param>
        /// <param name="campaignCancellationToken">Cancellation token</param>
        /// <returns></returns>
        PublisherJobProcess Create(string campaignId, string campaignName, string accountId, SocialNetworks network,
            IEnumerable<PublisherDestinationDetailsModel> destinationDetails,
            CancellationTokenSource campaignCancellationToken);
    }
}