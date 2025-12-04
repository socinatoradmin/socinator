using System;

namespace RedditDominatorCore.ReportModel
{
    public class UpvoteReportModel
    {
        public int Id { get; set; }

        public string AccountUsername { get; set; } = string.Empty;
        public string QueryType { get; set; } = string.Empty;

        public string ActivityType { get; set; } = string.Empty;

        public DateTime InteractionTimeStamp { get; set; }

        public int CommentsCount { get; set; }

        public bool IsCrosspostable { get; set; }

        public bool IsStickied { get; set; }

        public int NumComments { get; set; } = 0;

        public string Author { get; set; } = string.Empty;

        public int Score { get; set; }

        public bool Hidden { get; set; }

        public bool IsSpoiler { get; set; }

        public bool IsNsfw { get; set; }

        public string PostId { get; set; } = string.Empty;
        public int ViewCount { get; set; }
        public string Permalink { get; set; } = string.Empty;
        public DateTime Created { get; set; }

        public string Title { get; set; } = string.Empty;

        public bool IsOriginalContent { get; set; }

        public string Status { get; set; } = string.Empty;
    }

    public class UpvoteReportModelAccount
    {
        public int Id { get; set; }

        public string QueryType { get; set; } = string.Empty;

        public string ActivityType { get; set; } = string.Empty;

        public DateTime InteractionTimeStamp { get; set; }

        public int CommentsCount { get; set; }

        public bool IsCrosspostable { get; set; }

        public bool IsStickied { get; set; }

        public int NumComments { get; set; } = 0;

        public string Author { get; set; } = string.Empty;

        public int Score { get; set; }

        public bool Hidden { get; set; }

        public bool IsSpoiler { get; set; }
        public bool IsNsfw { get; set; }

        public string PostId { get; set; } = string.Empty;

        public int ViewCount { get; set; }

        public string Permalink { get; set; } = string.Empty;

        public DateTime Created { get; set; }

        public string Title { get; set; } = string.Empty;

        public bool IsOriginalContent { get; set; }

        public string Status { get; set; } = string.Empty;
    }
}