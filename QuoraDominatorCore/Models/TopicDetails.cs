namespace QuoraDominatorCore.Models
{
    public class TopicDetails
    {
        public string TopicId { get; set; } = string.Empty;
        public string TopicUrl { get; set; } = string.Empty;
        public bool IsSensitive { get; set; } = false;
        public bool IsFollowing { get; set; } = false;
        public int FollowerCount { get; set; } = 0;
        public string TopicName { get; set; } = string.Empty;
        public string TopicProfilePicUrl { get; set; } = string.Empty;
    }
}
