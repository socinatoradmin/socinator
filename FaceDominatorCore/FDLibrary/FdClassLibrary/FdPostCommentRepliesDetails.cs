using DominatorHouseCore.Interfaces;

namespace FaceDominatorCore.FDLibrary.FdClassLibrary
{
    public class FdPostCommentRepliesDetails : IComments
    {
        public string PostId { get; set; }
        public string CommentId { get; set; }
        public string CommentUrl { get; set; }
        public string ReplyCommentId { get; set; }
        public string ReplyCommentUrl { get; set; }
        public string ReplyCommenterID { get; set; }
        public string ReplyCommenterName { get; set; }
        public string CommentTimeWithDate { get; set; }
        public string CommenterID { get; set; }
        public string CommentText { get; set; }
        public string ReplyCommentText { get; set; }

        public string ReactionCountOnReplyComment { get; set; }
    }
}
