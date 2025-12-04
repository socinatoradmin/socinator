using System;

namespace FaceDominatorCore.FdReports
{
    public class CommentReportAccountModel
    {

        public int Id { get; set; }


        public string QueryType
        { get; set; } = string.Empty;


        public string QueryValue { get; set; } = string.Empty;

        public string ActivityType { get; set; } = string.Empty;

        public string CommenterId { get; set; } = string.Empty;

        public string CommentText { get; set; } = string.Empty;

        public string ReactionCountOnComment { get; set; } = string.Empty;

        public string CommentUrl { get; set; } = string.Empty;

        public string CommentTimeWithDate { get; set; } = string.Empty;

        public string PostId { get; set; } = string.Empty;

        public string ReactionType { get; set; } = string.Empty;

        public DateTime InteractionTimeStamp { get; set; } = DateTime.Now;

        public string ReactAsPageId { get; set; } = string.Empty;

        public string ReplyForComment { get; set; } = string.Empty;

        public string Mention { get; set; } = string.Empty;
    }
}
