#region

using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using ProtoBuf;

#endregion

namespace DominatorHouseCore.Models.SocioPublisher
{
    [ProtoContract]
    public class PublisherDestinationDetails : BindableBase
    {
        // To Specify the actual social networks
        [ProtoMember(1)] public SocialNetworks SocialNetworks { get; set; }

        /// <summary>
        ///     To specify the category of destination belongs to whether Groups or BoardsOrPages
        /// </summary>
        [ProtoMember(2)]
        public DestinationCategory Category { get; set; }

        /// <summary>
        ///     To specify the account Id where current destination belongs
        /// </summary>
        [ProtoMember(3)]
        public string AccountId { get; set; }

        /// <summary>
        ///     To specify the account name where current destination belongs
        /// </summary>
        [ProtoMember(4)]
        public string AccountName { get; set; }

        /// <summary>
        ///     To specify the Details Url where current destination belongs.
        ///     Suppose category belongs to groups means need to add groupUrl, else add boards or page Url
        /// </summary>
        [ProtoMember(5)]
        public string DetailsUrl { get; set; }

        /// <summary>
        ///     To specify the Details Name where current destination belongs.
        ///     Suppose category belongs to groups means need to add groupUrl, else add boards or page Url
        /// </summary>
        [ProtoMember(6)]
        public string DetailsName { get; set; }
    }

    public enum DestinationCategory
    {
        Groups = 1,
        BoardsOrPages = 2
    }
}