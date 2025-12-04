using System;
using DominatorHouseCore.Enums;

namespace GramDominatorCore.Report
{
    public class CommentReportDetails
    {
        public int Id { get; set; }

        public string AccountUsername { get; set; }

        public ActivityType ActivityType { get; set; } = ActivityType.Comment;

        public string CommentedMediaCode { get; set; }

        public string CommentedMediaOwner { get; set; }

        public string Follower { get; set; }

        public string MediaType { get; set; }

        public DateTime Date { get; set; }

        public string Comment { get; set; }

        public string Status { get; set; }
    }
}
