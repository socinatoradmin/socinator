using System;

namespace TumblrDominatorUI.Report
{
    public class UnLikeReportDetails
    {
        public int Id { get; set; }
        public string AccountName { get; set; }

        public DateTime Date { get; set; }

        public string PostUrl { get; set; }
    }
}