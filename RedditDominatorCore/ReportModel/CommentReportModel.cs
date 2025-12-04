using System;

namespace RedditDominatorCore.ReportModel
{
    public class CommentReportModel
    {
        public int Id { get; set; }
        public string AccountUsername { get; set; }
        public string QueryType { get; set; } = string.Empty;
        public string QueryValue { get; set; } = string.Empty;
        public string ActivityType { get; set; }
        public DateTime InteractionTimeStamp { get; set; }
        public string Author { get; set; }
        public DateTime Created { get; set; }
        public int NumComments { get; set; }
        public int NumCrossposts { get; set; }
        public string Permalink { get; set; }
        public string PostId { get; set; }
        public int Score { get; set; }
        public bool SendReplies { get; set; }
        public string Title { get; set; }
        public string CommentText { get; set; }
        public string OldComment { get; set; }
    }
}