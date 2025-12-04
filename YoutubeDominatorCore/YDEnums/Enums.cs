namespace YoutubeDominatorCore.YDEnums
{
    public class Enums
    {
        public enum EnumChannelHeader
        {
            Id,
            QueryValue,
            QueryType,
            ActivityType,
            AccountUsername,
            AccountChannelId,
            ChannelJoinedDate,
            ChannelLocation,
            ChannelProfilePic,
            ChannelUrl,
            ChannelDescription,
            ExternalLinks,
            InteractedChannelId,
            InteractedChannelName,
            InteractionTimeStamp,
            SubscriberCount,
            ViewsCount,
            VideosCount,
            IsSubscribed
        }
        public enum VedioType
        {
            Movie,
            Short,
            Video

        }

        public enum EnumCommentsHeader
        {
            CommentText,
            CommentId,
            ReactionOnComment,
            CommentPostedTime,
            CommenterChannelName,
            CommenterChannelId,
            CommentLikesCount
        }

        public enum EnumPostHeader
        {
            Id,
            QueryValue,
            QueryType,
            ActivityType,
            AccountUsername,
            AccountChannelId,
            ViewsCount,
            OwnerChannelId,
            OwnerChannelName,
            CommentCount,
            MyCommentedText,
            CommentId,
            DislikeCount,
            LikeCount,
            LikeStatus,
            PostDescription,
            PublishedDate,
            SubscribeCount,
            IsSubscribed,
            VideoDurationInHourMinSec,
            VideoUrl,
            VideoTitle,
            InteractionTimeStamp,
            RepeatedWatchCount,
            InteractedCommentUrl,
            ReportedToVideoWithOption,
            ReportedToVideoWithAdditionalText,
            ReportedToVideoWithSelectedTimestamp
        }

        public enum YdMainModule
        {
            Like = 1,
            Unlike = 2,
            LikeComment = 3,
            Comment = 4,
            DeleteComment = 5,
            Subscribe = 9,
            UnSubscribe = 10,
            Message = 11,
            Share = 12,
            ViewIncreaser = 13,
            PostScraper = 14,
            ChannelScraper = 16,
            Campaign = 17,
            ReportVideo = 18,
            CommentScraper = 19
        }
    }
}