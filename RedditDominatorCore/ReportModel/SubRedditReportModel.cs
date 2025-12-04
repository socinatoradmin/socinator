using System;

namespace RedditDominatorCore.ReportModel
{
    public class SubRedditReportModel
    {
        public int Id { get; set; }
        public string AccountUsername { get; set; } = string.Empty;
        public string QueryType { get; set; } = string.Empty;
        public string QueryValue { get; set; } = string.Empty;
        public string ActivityType { get; set; } = string.Empty;
        public DateTime InteractionTimeStamp { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public int Subscribers { get; set; }
        public bool IsNsfw { get; set; }
        public string SubRedditId { get; set; } = string.Empty;
        public bool IsQuarantined { get; set; }
        public string Url { get; set; } = string.Empty;
        public string DisplayText { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string CommunityIcon { get; set; } = string.Empty;
        public string PublicDescription { get; set; } = string.Empty;
        public bool UserIsSubscriber { get; set; }
        public string AccountsActive { get; set; } = string.Empty;
    }

    public class SubRedditReportModelAccount
    {
        public int Id { get; set; }

        public string AccountUsername { get; set; } = string.Empty;

        public string ActivityType { get; set; } = string.Empty;

        public DateTime InteractionTimeStamp { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;

        public int Subscribers { get; set; }
        public bool IsNsfw { get; set; }

        public string SubRedditId { get; set; } = string.Empty;
        public bool IsQuarantined { get; set; }

        public string Url { get; set; } = string.Empty;

        public string DisplayText { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string CommunityIcon { get; set; } = string.Empty;

        public string PublicDescription { get; set; } = string.Empty;

        public bool UserIsSubscriber { get; set; }
        public string AccountsActive { get; set; } = string.Empty;
    }
}