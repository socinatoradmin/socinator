using System;

namespace QuoraDominatorCore.Reports.AccountConfigReport
{
    public class InteractedPost
    {
        public int Id { get; set; }
        public string Pid { get; set; }
        public string QueryValue { get; set; }
        public string ActivityType { get; set; }

        public string QueryType { get; set; }

        public DateTime InteractionDateTime { get; set; }
        public DateTime PostCreationTime { get; set; }
        public string PostUrl { get; set; }
        public string AccountName { get; set; }
        public int UpvoteCount { get; set; }
        public int CommentCount { get; set; }
        public int ViewsCount { get; set; }
        public int ShareCount { get; set; }
        public int PostOwnerFollowerCount { get; set; }
        public string PostOwnerName { get; set; }
    }
}
