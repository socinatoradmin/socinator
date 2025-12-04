using DominatorHouseCore.Interfaces;

namespace FaceDominatorCore.FDLibrary.FdClassLibrary
{
    public class FdPostCommentDetails : IComments
    {
        public string CommentId { get; set; }

        public string CommenterID { get; set; }

        public string CommenterName { get; set; }

        public string CommentText { get; set; }

        public string ReactionCountOnComment { get; set; }

        public string CommentUrl { get; set; }

        public string CommentTimeWithDate { get; set; }

        public string HasLikedByUser { get; set; }

        public string PostId { get; set; }

        public bool CanCommentByUser { get; set; }

        public int ReplyCount { get; set; }

    }
}
