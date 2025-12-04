using System;

namespace RedditDominatorCore.RDModel
{
    public class RedditAutoActivityModel
    {
        public int Id { get; set; }
        public string PostUrl { get; set; }
        public string CommunityUrl { get; set; }
        public string ActivityType { get; set; }
        public string PostId { get; set; }
        public string UserName { get; set; }
        public DateTime InteractedDate { get; set; }
        public bool IsFollowing { get; set; }
        public bool IsJoined { get; set; }
        public bool IsUpvoted { get; set; }
        public bool IsDownvoted { get; set; }
        public string ProfileUrl { get; set; }
        public DateTime Created { get; set; }
    }
}
