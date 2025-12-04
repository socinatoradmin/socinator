using DominatorHouseCore.Interfaces;

namespace YoutubeDominatorCore.YoutubeModels
{
    public class YoutubeChannel : IChannel
    {
        public string ChannelUsername { get; set; }
        public bool HasChannelUsername { get; set; }
        public string ChannelUrl { get; set; }
        public string SubscriberCount { get; set; }
        public string ChannelJoinedDate { get; set; }
        public string VideosCount { get; set; }
        public string ViewsCount { get; set; }
        public string ExternalLinks { get; set; }
        public string ChannelDescription { get; set; }
        public string ChannelLocation { get; set; }
        public bool IsSubscribed { get; set; }
        public string InteractedCommentUrl { get; set; }

        public HeadersElements HeadersElements { get; set; }
        public PostDataElements PostDataElements { get; set; }
        public string ChannelName { get; set; }

        public string ChannelId { get; set; }
        public string ProfilePicUrl { get; set; }
    }
}