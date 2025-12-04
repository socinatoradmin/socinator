using System;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.TdTables.Accounts;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using TwtDominatorCore.TDModels;
using InteractedPosts = DominatorHouseCore.DatabaseHandler.TdTables.Campaign.InteractedPosts;
using InteractedUsers = DominatorHouseCore.DatabaseHandler.TdTables.Campaign.InteractedUsers;

namespace TwtDominatorCore.QueryHelper
{
    public class ClassMapper
    {
        public ClassMapper(string accountName, string accountId)
        {
            try
            {
                AccountName = accountName;
                AccountId = accountId;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private string AccountName { get; }
        private string AccountId { get; }

        public TwitterUser InteractedUserToTwitterUserMapper(InteractedUsers interactedUser)
        {
            var user = new TwitterUser();
            try
            {
                user = new TwitterUser
                {
                    FollowBackStatus = interactedUser.FollowBackStatus == 1,
                    Username = interactedUser.InteractedUsername,
                    UserId = interactedUser.InteractedUserId,
                    FullName = interactedUser.InteractedUserFullName,
                    FollowersCount = interactedUser.FollowersCount,
                    FollowingsCount = interactedUser.FollowingsCount,
                    TweetsCount = interactedUser.TweetsCount,
                    LikesCount = interactedUser.LikesCount,
                    UserBio = interactedUser.Bio,
                    IsPrivate = interactedUser.IsPrivate == 1,
                    JoiningDate = interactedUser.JoinedDate,
                    HasProfilePic = interactedUser.HasAnonymousProfilePicture == 0,
                    ProfilePicUrl = interactedUser.ProfilePicUrl,
                    UserLocation = interactedUser.Location,
                    WebPageURL = interactedUser.Website,
                    IsVerified = interactedUser.IsVerified == 1
                };
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return user;
        }

        public FeedInfoes TagDetailsToFeedsMapper(TagDetails tweet, bool InteractedBySoftware = false)
        {
            var ObjFeedInfo = new FeedInfoes();

            try
            {
                ObjFeedInfo = new FeedInfoes
                {
                    TweetId = tweet.Id,
                    TwtMessage = tweet.Caption?.Replace("\\n","\n"),
                    MediaId = tweet.Code,
                    LikeCount = tweet.LikeCount,
                    RetweetCount = tweet.RetweetCount,
                    CommentCount = tweet.CommentCount,
                    IsRetweet = tweet.IsRetweet ? 1 : 0,
                    IsComment = tweet.IsComment ? 1 : 0,
                    SourceTweetId=tweet.OriginalTweetId,
                    IsRetweetedOwnTweet = tweet.IsRetweetedOwnTweet,
                    TweetedTimeStamp =
                        InteractedBySoftware ? DateTime.UtcNow.GetCurrentEpochTime() : tweet.TweetedTimeStamp,
                    TweetOwnerIfRetweetedOrCommented = tweet.IsRetweet
                        ? tweet.Username
                        : tweet.IsComment
                            ? tweet.CommentedOnTweetOwner
                            : ""
                };
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return ObjFeedInfo;
        }

        public TagDetails InteractedTweetToTweetDetailsMapper(InteractedPosts interactedTweet)
        {
            var tweetDetails = new TagDetails();
            try
            {
                tweetDetails = new TagDetails
                {
                    Id = interactedTweet.TweetId,
                    Caption = interactedTweet.TwtMessage,
                    DateTime =interactedTweet.TwtPostDateTime,
                    UserId = interactedTweet.UserId,
                    Username = interactedTweet.Username,
                    TweetedTimeStamp = interactedTweet.TweetedTimeStamp,
                    LikeCount = interactedTweet.LikeCount,
                    CommentCount = interactedTweet.CommentCount,
                    RetweetCount = interactedTweet.RetweetCount,
                    Code = interactedTweet.MediaId = tweetDetails.Code,
                    IsRetweet = interactedTweet.IsRetweet == 1,
                    FollowStatus = interactedTweet.FollowStatus == 1,
                    FollowBackStatus = interactedTweet.FollowBackStatus == 1,
                    IsAlreadyLiked = interactedTweet.IsAlreadyLiked == 1,
                    IsAlreadyRetweeted = interactedTweet.IsAlreadyRetweeted == 1,
                    IsTweetContainedVideo = interactedTweet.MediaType == MediaType.Video
                };
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return tweetDetails;
        }
    }
}