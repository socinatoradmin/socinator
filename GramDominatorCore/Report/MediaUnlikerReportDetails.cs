using System;
using DominatorHouseCore.Enums;

namespace GramDominatorCore.Report
{
    public class MediaUnlikerReportDetails
    {
        public int Id { get; set; }

        public string AccountUsername { get; set; }

        public ActivityType ActivityType { get; set; } = ActivityType.Unlike;

        public string MediaCode { get; set; }

        public string MediaOwner { get; set; }

        public MediaType MediaType { get; set; }

        public DateTime Date { get; set; }
    }
}
