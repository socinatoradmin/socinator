using System;

namespace TumblrDominatorUI.Report
{
    public class ReblogReportDetails
    {
        public int Id { get; set; }

        public string AccountName { get; set; }

        public string Query { get; set; }

        public string QueryType { get; set; }

        public string PostOwner { get; set; }
        public string ContentId { get; set; }
        public string PostUrl { get; set; }
        public string ReblogUrl { get; set; }

        public DateTime Date { get; set; }
    }
}