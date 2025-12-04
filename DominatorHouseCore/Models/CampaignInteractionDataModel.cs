#region

using System;
using System.Collections.Generic;
using ProtoBuf;

#endregion

namespace DominatorHouseCore.Models
{
    [ProtoContract]
    public class CampaignInteractionDataModel
    {
        [ProtoMember(1)] public string CampaignId { get; set; } = string.Empty;

        [ProtoMember(2)]
        public SortedList<string, DateTime> InteractedData { get; set; } = new SortedList<string, DateTime>();
    }
}