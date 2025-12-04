using System;
using DominatorHouseCore.Enums;

namespace GramDominatorCore.Report
{
    public class LikeReportDetails
    {
        public int Id { get; set; }

        public string AccountUsername { get; set; }

        public ActivityType ActivityType { get; set; } = ActivityType.Like;

        public string LikedMediaCode { get; set; }

        public string LikedMediaOwner { get; set; }

        public string MediaType { get; set; }

        public DateTime Date { get; set; }
    }
}
