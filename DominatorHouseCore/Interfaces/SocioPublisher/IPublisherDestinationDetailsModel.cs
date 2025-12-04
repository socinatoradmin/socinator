#region

using DominatorHouseCore.Enums;
using DominatorHouseCore.Models.SocioPublisher;

#endregion

namespace DominatorHouseCore.Interfaces.SocioPublisher
{
    public interface IPublisherDestinationDetailsModel
    {
        string AccountId { get; set; }
        string AccountName { get; set; }
        string DestinationType { get; set; }
        string DestinationUrl { get; set; }
        PublisherPostlistModel PublisherPostlistModel { get; set; }
        SocialNetworks SocialNetworks { get; set; }
    }
}