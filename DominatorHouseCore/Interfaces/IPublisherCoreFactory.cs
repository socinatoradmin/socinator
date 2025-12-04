#region

using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.Enums;

#endregion

namespace DominatorHouseCore.Interfaces
{
    public interface IPublisherCoreFactory
    {
        /// <summary>
        ///     Specify the network of the dominator
        /// </summary>
        SocialNetworks Network { get; set; }

        /// <summary>
        ///     To hold the publishing objects for the networks
        /// </summary>
        IPublisherJobProcessFactory PublisherJobFactory { get; set; }

        /// <summary>
        ///     To hold the scraping objects for the networks, its only for Facebook, Pinterest, Twitter
        /// </summary>
        IPublisherPostScraper PostScraper { get; set; }
    }
}