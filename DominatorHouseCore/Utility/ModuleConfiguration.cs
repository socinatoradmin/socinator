#region

using System;
using System.Collections.Generic;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using ProtoBuf;

#endregion

namespace DominatorHouseCore.Utility
{
    [ProtoContract]
    public class ModuleConfiguration
    {
        [ProtoMember(1)] public string TemplateId { get; set; }

        [ProtoMember(2)] public bool IsEnabled { get; set; }

        [ProtoMember(3)] public string Status { get; set; }

        [ProtoMember(4)] public int LastUpdatedDate { get; set; } = DateTimeUtilities.GetEpochTime();

        [ProtoMember(5)] public List<RunningTimes> LstRunningTimes { get; set; }

        [ProtoMember(6)] public ActivityType ActivityType { get; set; }

        [ProtoMember(7)] public int MaximumCountPerDay { get; set; } = 30;

        [ProtoMember(8)] public bool IsTemplateMadeByCampaignMode { get; set; }

        [ProtoMember(9)] public DateTime LastRun { get; set; }

        [ProtoMember(10)] public DateTime NextRun { get; set; }

        [ProtoMember(11)] public RangeUtilities DelayBetweenJobs { get; set; }

        [ProtoMember(12)] public RangeUtilities DelayBetweenAccounts { get; set; } = new RangeUtilities(0, 0);

        [ProtoMember(13)] public int AccountCount { get; set; } = 0;
    }
}