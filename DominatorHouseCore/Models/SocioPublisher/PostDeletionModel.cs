#region

using System;
using DominatorHouseCore.Enums;
using ProtoBuf;

#endregion

namespace DominatorHouseCore.Models.SocioPublisher
{
    [Serializable]
    [ProtoContract]
    public class PostDeletionModel
    {
        [ProtoMember(1)] public string CampaignId { get; set; }

        [ProtoMember(2)] public string PostId { get; set; }

        [ProtoMember(3)] public string PublishedIdOrUrl { get; set; }

        [ProtoMember(4)] public DateTime DeletionTime { get; set; }

        [ProtoMember(5)] public string AccountId { get; set; }

        [ProtoMember(6)] public SocialNetworks Networks { get; set; }

        [ProtoMember(7)] public string DestinationType { get; set; }

        [ProtoMember(8)] public string DestinationUrl { get; set; }

        [ProtoMember(9)] public bool IsDeletedAlready { get; set; }
    }
}