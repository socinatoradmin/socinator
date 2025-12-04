using DominatorHouseCore.Models;
using System.Collections.Generic;

namespace FaceDominatorCore.FDLibrary.FdClassLibrary
{
    public class FdScraperResponseParameters
    {
        public List<FacebookPostDetails> ListPostDetails { get; set; }

        public List<FacebookUser> ListUser { get; set; }

        public List<FanpageDetails> ListPage { get; set; }

        public List<FacebookAdsDetails> ListAds { get; set; }

        public List<GroupDetails> ListGroup { get; set; }
        public List<FdPostCommentRepliesDetails> CommentRepliesList { get; set; }

        public List<FdPostCommentDetails> CommentList { get; set; }

        public List<FdMessageDetails> MessageDetailsList { get; set; }

        public List<SenderDetails> MessageSenderDetailsList { get; set; }

        public List<ChatDetails> ListSenderDetails { get; set; }

        public List<ChatDetails> ListChatDetails { get; set; }

        public FacebookPostDetails PostDetails { get; set; }

        public FacebookUser FacebookUser { get; set; }

        public FacebookAdsDetails FacebookAdsDetails { get; set; }

        public FanpageDetails FanpageDetails { get; set; }
        public GroupDetails facebookGroup { get; set; }

        public bool IsPagination { get; set; }

        public string FinalEncodedQuery { get; set; } = string.Empty;

        public string AlbumName { get; set; } = string.Empty;

        public string AjaxToken { get; set; } = string.Empty;

        public int ScrollCount { get; set; } = 0;

        public int ClientStoriesCount { get; set; } = 0;

        public string ImpressionSource = string.Empty;

        public double SectionId { get; set; } = 0;

        public List<KeyValuePair<string, string>> ListPostReaction { get; set; }

        public List<KeyValuePair<string, string>> ListReactionPermission { get; set; }

        public string FetchStream { get; set; }

        public string ShownIds { get; set; }

        public string TotalCount { get; set; }

        public List<string> PaginationUserIds { get; set; }

        public string ExtraData { get; set; }

        public string FeedLocation { get; set; }

        public string FeedContext { get; set; }

        public int Offset { get; set; }

        public int CommentCount { get; set; }

        public int Length { get; set; }

        public bool IsFirstPage { get; set; }

        public bool IsIncreasingOrder { get; set; }

        public string SeenTimeStamp { get; set; }

        public string EndingId { get; set; }

        public FriendsPager FriendsPager { get; set; }

        public string Query { get; set; }

        public string OriginalQuery { get; set; }

        public string Cursor { get; set; }

        public string CollectionToken { get; set; }

        public string PaginationTimestamp { get; internal set; }

        public FdPageLikersParameters FdPageLikersParameters { get; set; }

        public bool IsBlocked { get; set; }

        public string CommentId { get; set; }

        public bool IsCommentedOnPost { get; set; }

        public string FailedReason = string.Empty;

    }
}
