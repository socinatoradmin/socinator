using System;

namespace RedditDominatorCore.ReportModel
{
    public class UnfollowedUsersReportModel
    {
        public int Id { get; set; }
        public string SinAccUsername { get; set; } = string.Empty;
        public DateTime InteractionDateTime { get; set; }
        public string Username { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
    }
}