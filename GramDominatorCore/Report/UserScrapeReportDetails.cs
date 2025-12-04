using System;
using DominatorHouseCore.Enums;

namespace GramDominatorCore.Report
{
    public class UserScrapeReportDetails
    {
        public int Id { get; set; }

        public string AccountUsername { get; set; }

        public string QueryType { get; set; }

        public string QueryValue { get; set; }

        public ActivityType ActivityType { get; set; } = ActivityType.UserScraper;

        public string ScrapedUsername { get; set; }

        public string ScrapedUserId { get; set; }

        public bool IsPrivate { get; set; }

        public bool IsBusiness { get; set; }

        public bool IsVerified { get; set; }

        public bool IsProfilePicAvailable { get; set; }

        public string ProfilePicUrl { get; set; }

        public DateTime Date { get; set; }

        public bool IsFollowedAlready { get; set; }
        public string UserName { get; set; }
        public string UserId { get; set; }
        public string UserFullName { get; set; }
        public string PostCount { get; set; }
        public string FollowerCount { get; set; }
        public string FollowingCount { get; set; }
        public string EamilId { get; set; }
        public string ContactNo { get; set; }
        public string EngagementRate { get; set; }
        public string CommentCount { get; set; }
        public string LikeCount { get; set; }
        public string TaggedUser { get; set; }
        public string Gender { get; set; }
        public string Biography { get; set; } 
        public bool IsBusinessAccount { get; set; }
        public string BusinessCategory { get; set; }

    }

    public class UserRequiredData
    {
        public string ProfilePictureUrl { get; set; }
        public string UserName { get; set; }
        public string UserId { get; set; }
        public string UserFullName { get; set; }
        public bool IsFollowedAlready { get; set; }
        public int PostCount { get; set; }
        public int FollowerCount { get; set; }
        public int FollowingCount { get; set; }
        public string EamilId { get; set; }
        public string ContactNo { get; set; }
        public string EngagementRate { get; set; }
        public string LikeCount { get; set; }
        public string CommentCount { get; set; }
        public string Biography { get; set; }
        public bool IsBusinessAccounts { get; set; }
        public string BusinessCategory { get; set; }
    }

    public class IsUserRequiredData
    {
        public bool IsProfilePictureUrl = false;
        public bool IsUserName = false;
        public bool IsUserId = false;
        public bool IsUserFullName = false;
        public bool IsFollowedAlready = false;
        public bool IsPostCount = false;
        public bool IsFollowerCount = false;
        public bool IsFollowingCount = false;
        public bool IsEamilId = false;
        public bool IsContactNo = false;
        public bool IsEngagementRate = false;
        public bool IsCommentCount = false;
        public bool IsLikeCount = false;
        public bool IsBusinessCategory = false;
    }
}
