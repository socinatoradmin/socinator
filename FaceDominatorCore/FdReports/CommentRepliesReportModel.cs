using System;

namespace FaceDominatorCore.FdReports
{
    public class CommentRepliesReportModel
    {
        public int Id { get; set; }
        public string AccountEmail { get; set; } = string.Empty;
        public string QueryType { get; set; } = string.Empty;
        public string QueryValue { get; set; } = string.Empty;
        public string ActivityType { get; set; } = string.Empty;
        public string PostId { get; set; } = string.Empty;
        public string CommentId { get; set; } = string.Empty;
        public string CommentUrl { get; set; } = string.Empty;
        public string ReplyCommentId { get; set; } = string.Empty;
        public string ReplyCommentUrl { get; set; } = string.Empty;
        public string ReplyCommenterID { get; set; } = string.Empty;
        public string ReplyCommenterName { get; set; } = string.Empty;
        public string ReplyCommentText { get; set; } = string.Empty;
        public string CommentTimeWithDate { get; set; } = string.Empty;
        public string CommenterID { get; set; } = string.Empty;
        public DateTime InteractionTimeStamp { get; set; } = DateTime.Now;
    }

    public class CommentRepliesReportModelAccountModel
    {
        public int Id { get; set; }
        public string QueryType { get; set; } = string.Empty;
        public string QueryValue { get; set; } = string.Empty;
        public string ActivityType { get; set; } = string.Empty;
        public string PostId { get; set; } = string.Empty;
        public string CommentId { get; set; } = string.Empty;
        public string CommentUrl { get; set; } = string.Empty;
        public string ReplyCommentId { get; set; } = string.Empty;
        public string ReplyCommentUrl { get; set; } = string.Empty;
        public string ReplyCommenterID { get; set; } = string.Empty;
        public string ReplyCommenterName { get; set; } = string.Empty;
        public string ReplyCommentText { get; set; } = string.Empty;
        public string CommentTimeWithDate { get; set; } = string.Empty;
        public string CommenterID { get; set; } = string.Empty;
        public DateTime InteractionTimeStamp { get; set; } = DateTime.Now;
    }

}
