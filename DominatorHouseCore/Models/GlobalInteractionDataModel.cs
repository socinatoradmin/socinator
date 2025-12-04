#region

using System;
using System.Collections.Generic;
using ProtoBuf;

#endregion

namespace DominatorHouseCore.Models
{
    [ProtoContract]
    public class GlobalInteractionDataModel
    {
        //[ProtoMember(1)]
        //public ActivityType ActivityType { get; set; }

        [ProtoMember(1)]
        public SortedList<string, DateTime> InteractedData { get; set; } = new SortedList<string, DateTime>();
    }
}