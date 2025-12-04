using System;
using DominatorHouseCore.Enums;

namespace GramDominatorCore.Report
{
    public class BlockFollowerReportDetails
    {
        public int Id { get; set; }

        public string AccountUsername { get; set; }

        public ActivityType ActivityType { get; set; } = ActivityType.BlockFollower;

        public string BlockedFollowerUsername { get; set; }

        public string BlockedFollowerUserId { get; set; }

        public DateTime Date { get; set; }
    }
}
