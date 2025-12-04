#region

using System.Collections.Generic;
using DominatorHouseCore.Models;
using ProtoBuf;

#endregion

namespace DominatorHouseCore.ViewModel
{
    [ProtoContract]
    public class CampaignInteractionViewModel
    {
        [ProtoMember(1)]
        public Dictionary<string, CampaignInteractionDataModel> CampaignInteractedCollections { get; set; } =
            new Dictionary<string, CampaignInteractionDataModel>();
    }
}