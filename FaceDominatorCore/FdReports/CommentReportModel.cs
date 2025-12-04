using System;

namespace FaceDominatorCore.FdReports
{
    public class CommentReportModel
    {

        public int Id { get; set; }


        public string AccountEmail { get; set; } = string.Empty;


        public string QueryType
        { get; set; } = string.Empty;


        public string QueryValue { get; set; } = string.Empty;


        public string ActivityType { get; set; } = string.Empty;


        public string CommentId { get; set; } = string.Empty;

        public string CommenterId { get; set; } = string.Empty;

        public string CommentText { get; set; } = string.Empty;

        public string ReactionCountOnComment { get; set; } = string.Empty;

        public string CommentUrl { get; set; } = string.Empty;

        public string CommentTimeWithDate { get; set; } = string.Empty;

        /*
                public string HasLikedByUser { get; set; } = string.Empty;
        */

        public string PostId { get; set; } = string.Empty;

        public DateTime InteractionTimeStamp { get; set; } = DateTime.Now;

        public string ReactionType { get; set; } = string.Empty;

        public string ReactAsPageId { get; set; } = string.Empty;

        public string ReplyForComment { get; set; } = string.Empty;


        public string Mention { get; set; } = string.Empty;
    }
}
