#region

using System;
using DominatorHouseCore.Enums;
using ProtoBuf;

#endregion

namespace DominatorHouseCore.Models.SocioPublisher
{
    [Serializable]
    [ProtoContract]
    public class PublishedPostDetailsModel
    {
        [ProtoMember(1)] public string AccountName { get; set; }

        [ProtoMember(2)] public string Destination { get; set; }

        [ProtoMember(3)] public string DestinationUrl { get; set; }

        [ProtoMember(4)] public string Description { get; set; }

        [ProtoMember(5)] public string IsPublished { get; set; }

        [ProtoMember(6)] public string Successful { get; set; }

        [ProtoMember(7)] public DateTime PublishedDate { get; set; }

        [ProtoMember(8)] public string Link { get; set; }

        [ProtoMember(9)] public DateTime DeletionDate { get; set; }

        [ProtoMember(10)] public string CampaignId { get; set; }

        [ProtoMember(11)] public string CampaignName { get; set; }

        [ProtoMember(12)] public string AccountId { get; set; }

        [ProtoMember(13)] public string ErrorDetails { get; set; }

        [ProtoMember(14)] public SocialNetworks SocialNetworks { get; set; }
        [ProtoMember(15)] public string Title { get; set; }
    }
}