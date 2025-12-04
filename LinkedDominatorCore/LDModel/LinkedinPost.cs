using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;

namespace LinkedDominatorCore.LDModel
{
    public class LinkedinPost : LinkedinUser, IPost
    {
        public string PostTitle { get; set; }
        public string PostTrackingId { get; set; }
        public long PostedTime { get; set; }
        public string ShareUrn { get; set; }
        public MediaType MediaType { get; set; }
        public string PostVideoUrl { get; set; }
        public string PostImageUrl { get; set; }
        public string PostLink { get; set; }
        public string MyComment { get; set; }
        public string LikeCount { get; set; }
        public string CommentCount { get; set; }
        public string ShareCount { get; set; }

        public string IsLiked { get; set; }

        public string ShowShareButton { get; set; }
        public string Id { get; set; }
        public string Caption { get; set; }
        public string Code { get; set; }
    }
}