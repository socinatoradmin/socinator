using DominatorHouseCore.Enums;

namespace RedditDominatorCore.Report
{
    public class InteractedPostReportDetails
    {
        public int Id { get; set; }


        public int InteractionDate { get; set; }


        public string Username { get; set; }

        public string Query { get; set; }

        public string QueryType { get; set; }

        public double VideoDuration { get; set; }


        public int ViewCount { get; set; }

        public ActivityType OperationType { get; set; }
        public string UserId { get; set; }

        public string BoardId { get; set; }

        public string SinAccId { get; set; }

        public string SinAccUsername { get; set; }


        public string Title { get; set; }

        public string CommentUrl { get; set; }

        public string UserUrl { get; set; }


        public string PointsCount { get; set; }

        public string CommentsCount { get; set; }
    }
}