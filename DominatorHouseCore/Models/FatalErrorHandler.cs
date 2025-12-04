#region

using System;
using System.Collections.Generic;
using DominatorHouseCore.Enums;
using ProtoBuf;

#endregion

namespace DominatorHouseCore.Models
{
    [ProtoContract]
    public class FatalErrorHandler
    {
        [ProtoMember(1)] public string FatalErrorMessage { get; set; }

        [ProtoMember(2)] public DateTime FatalErrorAddedDate { get; set; }

        [ProtoMember(3)] public string ErrorSource { get; set; }

        [ProtoMember(4)] public HashSet<SocialNetworks> ErrorNetworks { get; set; }
        [ProtoMember(5)] public bool IsUnSubscribed { get; set; }
        [ProtoMember(6)] public bool IsSubscribed { get; set; }
    }
}