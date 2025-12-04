using System;

namespace TumblrDominatorUI.Report
{
    public class CommentUserDetails
    {
        public int Id { get; set; }

        public string AccountName { get; set; }

        public string Query { get; set; }

        public string QueryType { get; set; }

        public string ContentId { get; set; } //Comments

        public string Comments { get; set; }

        public DateTime Date { get; set; }
    }
}