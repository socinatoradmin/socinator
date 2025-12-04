using System;
using DominatorHouseCore.Enums;

namespace GramDominatorCore.Report
{
    public class FollowBackReportDetails
    {
        public int Id { get; set; }

        public string AccountUsername { get; set; }

        public ActivityType ActivityType { get; set; } = ActivityType.FollowBack;

        public string FollowedBackUsername { get; set; }

        public string FollowedBackUserId { get; set; }

        public DateTime Date { get; set; }
    }
}
