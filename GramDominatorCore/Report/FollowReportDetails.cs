using System;
using DominatorHouseCore.Enums;

namespace GramDominatorCore.Report
{
    public class FollowReportDetails
    {
        public int Id { get; set; }

        public string AccountUsername { get; set; }

        public ActivityType ActivityType { get; set; } = ActivityType.Follow;

        public string Query { get; set; }

        public string QueryType { get; set; }
       
        //public int FollowedBack{ get; set; }
        
       // public int FollowedBackDate{ get; set; }

        public DateTime Date { get; set; }
        
        public string InteractedUsername { get; set; }

        public string InteractedUserId { get; set; }

        public string Status { get; set; }

        public string MediaCode { get; set; }


    }

}
