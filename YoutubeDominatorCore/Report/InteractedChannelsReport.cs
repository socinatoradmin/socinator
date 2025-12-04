namespace YoutubeDominatorCore.Report
{
    public class InteractedChannelsReport
    {
        public int Id { get; set; }

        public string AccountUsername { get; set; }

        public string AccountChannelId { get; set; }

        public string QueryType { get; set; }

        public string QueryValue { get; set; }

        public string ActivityType { get; set; }

        public string InteractedChannelName { get; set; }

        public string InteractedChannelId { get; set; }

        public string InteractionTime { get; set; }

        public string SubscriberCount { get; set; }

        public bool IsSubscribed { get; set; }

        public string ViewsCount { get; set; }

        public string ChannelProfilePic { get; set; }

        public string ChannelLocation { get; set; }

        public string ChannelDescription { get; set; }

        public string ChannelJoinedDate { get; set; }

        public string ExternalLinks { get; set; }

        public string ChannelUrl { get; set; }

        public string VideosCount { get; set; }

        public string InteractedCommentUrl { get; set; }
    }
}