using System;
using DominatorHouseCore.Enums;

namespace GramDominatorCore.Report
{
    public class UnfollowReportDetails
    {
        public int Id { get; set; }

        public string AccountUsername { get; set; }

        public ActivityType ActivityType { get; set; } = ActivityType.Unfollow;

        // public int FollowedBack { get; set; }

        // public DateTime FollowedBackDate { get; set; }

        public DateTime Date { get; set; }

        public string UnfollowedUsername { get; set; }
    }
}
