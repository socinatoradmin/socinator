using System;

namespace TumblrDominatorUI.Report
{
    public class UnfollowReportDetails
    {
        public int Id { get; set; }
        public string AccountName { get; set; }

        public DateTime Date { get; set; }

        public string UnfollowedUsername { get; set; }
    }
}