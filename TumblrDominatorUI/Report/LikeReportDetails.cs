using System;

namespace TumblrDominatorUI.Report
{
    public class LikeReportDetails
    {
        public int Id { get; set; }

        public string AccountName { get; set; }

        public string Query { get; set; }

        public string QueryType { get; set; }

        public string ContentId { get; set; }

        public DateTime Date { get; set; }
    }
}