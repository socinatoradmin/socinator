using System;

namespace TumblrDominatorUI.Report
{
    public class FollowReportDetails
    {
        public int Id { get; set; }
        public string AccountName { get; set; }

        public string Query { get; set; }
        public string QueryType { get; set; }

        public DateTime Date { get; set; }

        public string FollowedUsername { get; set; }
    }
}