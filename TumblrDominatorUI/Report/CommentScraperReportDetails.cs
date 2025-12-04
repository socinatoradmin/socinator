using System;

namespace TumblrDominatorUI.Report
{
    public class CommentScraperReportDetails
    {
        public int Id { get; set; }

        public string AccountName { get; set; }

        public string QueryValue { get; set; }

        public string QueryType { get; set; }

        public string ProfileUrl { get; set; }
        public string PostUrl { get; set; }
        public string NotesCount { get; set; }
        public string LikesCount { get; set; }

        public string UserName { get; set; }
        public string Type { get; set; }
        public string CommentText { get; set; }
        public string ReblogText { get; set; }
        public string ContentId { get; set; }

        public DateTime Date { get; set; }
    }
}