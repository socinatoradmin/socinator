#region

using System.Collections.Generic;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using ProtoBuf;

#endregion

namespace DominatorHouseCore.ViewModel
{
    [ProtoContract]
    public class GlobalInteractionViewModel
    {
        [ProtoMember(1)]
        public Dictionary<ActivityType, GlobalInteractionDataModel> GlobalInteractedCollections { get; set; } =
            new Dictionary<ActivityType, GlobalInteractionDataModel>();
    }
}