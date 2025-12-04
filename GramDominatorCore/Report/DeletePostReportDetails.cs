using System;
using DominatorHouseCore.Enums;

namespace GramDominatorCore.Report
{
    public class DeletePostReportDetails
    {
        public int Id { get; set; }

        public string AccountUsername { get; set; }

        public ActivityType ActivityType { get; set; } = ActivityType.DeletePost;

        public string MediaCode { get; set; }

        public MediaType MediaType { get; set; }
        
        public DateTime Date { get; set; }
    }
}
