using System;
using DominatorHouseCore.Enums;

namespace GramDominatorCore.Report
{
    public class ReposterReportDetails
    {
        public int Id { get; set; }

        public string AccountUsername { get; set; }

        public ActivityType ActivityType { get; set; } = ActivityType.Reposter;

        public string OriginalMediaCode { get; set; }

        public string OriginalMediaOwner { get; set; }

        public string GeneratedMediaCode { get; set; }

        public MediaType MediaType { get; set; }
        public string ReposterComment { get; set; }

        public DateTime Date { get; set; }
    }
}
