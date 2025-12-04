using System;

namespace RedditDominatorCore.ReportModel
{
    public class InteractedUsersReportModel
    {
        public int Id { get; set; }
        public string SinAccUsername { get; set; } = string.Empty;
        public string QueryType { get; set; } = string.Empty;
        public string QueryValue { get; set; } = string.Empty;
        public string ActivityType { get; set; } = string.Empty;
        public DateTime InteractionDateTime { get; set; }
        public string InteractedUsername { get; set; } = string.Empty;
        public int CommentKarma { get; set; }
        public DateTime Created { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public bool IsEmployee { get; set; }
        public bool IsFollowing { get; set; }
        public bool IsMod { get; set; }
        public bool IsNsfw { get; set; }
        public int PostKarma { get; set; }
        public string Url { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public int FollowedBack { get; set; }
    }
}