using System;
using DominatorHouseCore.Interfaces;

namespace GramDominatorCore.GDModel
{
    public class ResultCommentItemUser: IPostComment
    {

        public string CommentId { get ; set; }

        public int CommentLikeCount { get; set; }

        public string ContentType { get; set; }

        public int CreatedAt { get; set; }

        public bool HasLikedComment { get; set; }

        public InstagramUser ItemUser { get; set; }

        public string Status { get; set; }

        public string Text { get; set; }

        public int Type { get; set; }

        public string UserId { get; set; }
        
    }
}
