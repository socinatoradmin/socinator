using System;

namespace RedditDominatorCore.ReportModel
{
    public class RemoveVoteReportModel
    {
        public int Id { get; set; }

        public string AccountUsername { get; set; }

        public string ActivityType { get; set; }

        public DateTime InteractionTimeStamp { get; set; }

        public int CommentsCount { get; set; }

        public bool IsCrosspostable { get; set; }

        public bool IsStickied { get; set; }

        public int NumComments { get; set; }

        public string Author { get; set; }

        public int Score { get; set; }

        public bool Hidden { get; set; }

        public bool IsSpoiler { get; set; }

        public bool IsNsfw { get; set; }

        public string PostId { get; set; }
        public int ViewCount { get; set; }
        public string Permalink { get; set; }
        public long Created { get; set; }

        public string Title { get; set; }

        public bool IsOriginalContent { get; set; }

        public string Status { get; set; }
    }

    public class RemoveVoteReportModelAccount
    {
        public int Id { get; set; }
        public string ActivityType { get; set; }
        public DateTime InteractionTimeStamp { get; set; }
        public int CommentsCount { get; set; }
        public bool IsCrosspostable { get; set; }
        public bool IsStickied { get; set; }
        public int NumComments { get; set; }
        public string Author { get; set; }
        public int Score { get; set; }
        public bool Hidden { get; set; }
        public bool IsSpoiler { get; set; }
        public bool IsNsfw { get; set; }
        public string PostId { get; set; }
        public int ViewCount { get; set; }
        public string Permalink { get; set; }
        public long Created { get; set; }
        public string Title { get; set; }
        public bool IsOriginalContent { get; set; }
        public string Status { get; set; }
    }
}