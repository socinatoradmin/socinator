using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using System.Collections.Generic;

namespace TumblrDominatorCore.Models
{
    public class TumblrPost : IPost
    {
        public string RebloggedRootId { get; set; }

        public string PostKey { get; set; }
        public string PostType { get; set; }
        public string ProfileUrl { get; set; }
        public string CommentText { get; set; } //ReblogText
        public string ReblogText { get; set; }
        public string BlogUrl { get; set; }
        public string Type { get; set; }
        public string OwnerUsername { get; set; }
        public string PostUrl { get; set; }
        public long PostPostedTimeStamp { get; set; }
        public int PostNotes { get; set; }
        public bool PostComment { get; set; }
        public string LikeCount { get; set; }
        public string NotesCount { get; set; }
        public string BlogName { get; set; }
        public string ReblogCount { get; set; }
        public string ReplyCount { get; set; }
        public string MediaUrl { get; set; }
        public MediaType MediaType { get; set; }
        public List<TumblrComments> ListComments { get; set; }
        public bool IsLiked { get; set; }
        public bool CanLike { get; set; }
        public bool CanReblog { get; set; }
        public bool CanReply { get; set; }
        public string Id { get; set; }
        public string Caption { get; set; }
        public string Code { get; set; }
        public string Uuid { get; set; }
        public string PlacementId { get; set; }
        public bool isVidioType { get; set; }
        public bool isTextType { get; set; }
        public bool isImageType { get; set; }
        public string PostName { get; set; }
        public string ReblogUrl { get; set; }
        public bool isFollowedPostOwner { get; set; }
    }

    public class TumblrComments : IComments
    {
        public TumblrUser commenter { get; set; }
        public string CommenterID { get; set; }
        public string Type { get; set; }
        public string CommentId { get; set; }
        public string PostId { get; set; }
        public string CommentText { get; set; }
        public string AddedText { get; set; }
        public string CommentTimeWithDate { get; set; }
    }
}