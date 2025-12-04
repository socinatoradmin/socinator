using System;

namespace TumblrDominatorUI.Report
{
    public class UserScraperReportDetails
    {
        public int Id { get; set; }

        public string AccountName { get; set; }

        public string QueryValue { get; set; }

        public string QueryType { get; set; }

        public string ProfileUrl { get; set; }
        public string PostUrl { get; set; }
        public string NotesCount { get; set; }
        public string LikesCount { get; set; }
        public string InteractedUserName { get; set; }


        public string ContentId { get; set; }

        public DateTime Date { get; set; }
    }
}