using System;
using System.Collections.Generic;
using System.Linq;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.TdTables.Accounts;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using TwtDominatorCore.QueryHelper;
using TwtDominatorCore.TDLibrary.GeneralLibrary.DAL;
using TwtDominatorCore.TDModels;

namespace TwtDominatorCore.Database
{
    public interface IDbInsertionHelper
    {
        void AddInteractedUserDetailsInAccountDb(TwitterUser user, string activityType,
            string processDescription, ScrapeResultNew ScrapeResult = null, string MessageText = null,
            string MediaPath = null);

        void AddInteractedTweetDetailInAccountDb(TagDetails tweetDetails, string activityType,
            string processDescription, ScrapeResultNew ScrapeResult = null, string CommentedText = null);

        void AddInteractedTweetDetailInCampaignDb(TagDetails tweetDetails, string activityType,
            ScrapeResultNew ScrapeResult, string CommentedText = null);

        void AddUnfollowedDataInAccountDb(TwitterUser user, string ProcessDescription,
            ScrapeResultNew ScrapeResult = null);

        void AddInteractedUserDetailsInCampaignDb(TwitterUser user, string activityType,
            ScrapeResultNew ScrapeResult, string MessageText = null, string MediaPath = null);

        void AddUnfollowedDataInCampaignDb(TwitterUser user, ScrapeResultNew ScrapeResult);
        void AddFriendshipData(TwitterUser user, FollowType followType, int sendMessageStatus);

        void UpdateAccountStatus(string userId, string userName, string accountStatus);

        int AddFriendshipListData(List<TwitterUser> listUser, FollowType followType, int SendMessageStatus);
        void UpdateFriendshipData(string userId);
        void DeleteTweetFromFeedInfo(string tweetId);
        int AddListFeedsInfo(List<TagDetails> ListTweet);
        void AddFeedsInfo(TagDetails tweet, bool InteractedBySoftware = false);
        void UpdateMessageStatusOfFriendship(string UserID);
        List<InteractedUsers> GetInteractedUser(ActivityType activityType);
    }

    public class DbInsertionHelper : IDbInsertionHelper
    {
        private readonly IDbCampaignService _campaignService;
        private readonly IDbAccountService _dbAccountService;


        //public DbInsertionHelper(DominatorAccountModel dominatorAccountModel)
        //{
        //    try
        //    {
        //        _dbAccountService = InstanceProvider.ResolveWithDominatorAccount<IDbAccountService>(dominatorAccountModel);
        //        AccountName = dominatorAccountModel.UserName;
        //        AccountId = dominatorAccountModel.AccountId;
        //    }
        //    catch (Exception ex)
        //    {
        //        ex.DebugLog();
        //    }
        //}

        public DbInsertionHelper(IDbAccountServiceScoped dbAccountService, IProcessScopeModel processScopeModel,
            IDbCampaignService campaignService)
        {
            try
            {
                _dbAccountService = dbAccountService;
                AccountName = processScopeModel.Account?.UserName;
                AccountId = processScopeModel.Account.AccountId;
                if (campaignService != null)
                    _campaignService = campaignService;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private string AccountName { get; }
        private string AccountId { get; }

        private int Count { get; }

        public void AddInteractedUserDetailsInAccountDb(TwitterUser user, string activityType,
            string processDescription, ScrapeResultNew ScrapeResult = null, string MessageText = null,
            string MediaPath = null)
        {
            try
            {
                var FollowStatus = user.FollowStatus ? 1 : 0;
                var followActType = ActivityType.Follow.ToString();
                var followBackActType = ActivityType.FollowBack.ToString();
                // in case first follow...next unfollow ...again if try to follow same user 
                if (activityType == followActType ||
                    activityType == followBackActType)
                {
                    FollowStatus = 1;
                    var AlreadyInteractedUser = _dbAccountService.GetInteractedUsers(ActivityType.Follow)
                        .FirstOrDefault(x =>
                            x.ActivityType == followActType && x.InteractedUserId == user.UserId &&
                            x.FollowStatus == 0);
                    if (AlreadyInteractedUser != null)
                    {
                        AlreadyInteractedUser.FollowStatus = 1;
                        _dbAccountService.Update(AlreadyInteractedUser);
                        return;
                    }
                }

                var interactedUser = new InteractedUsers
                {
                    SinAccUsername = AccountName,
                    QueryValue = ScrapeResult == null ? "NoQuery" : ScrapeResult.QueryInfo?.QueryValue,
                    QueryType = ScrapeResult?.QueryInfo?.QueryType,
                    ActivityType = activityType,
                    FollowStatus = FollowStatus,
                    FollowBackStatus = user.FollowBackStatus ? 1 : 0,
                    InteractionTimeStamp = DateTimeUtilities.GetEpochTime(),
                    InteractedUsername = user.Username,
                    InteractedUserId = user.UserId,
                    InteractedUserFullName = user.FullName,
                    FollowersCount = user.FollowersCount,
                    FollowingsCount = user.FollowingsCount,
                    TweetsCount = user.TweetsCount,
                    LikesCount = user.LikesCount,
                    Bio = user.UserBio,
                    IsPrivate = user.IsPrivate ? 1 : 0,
                    JoinedDate = user.JoiningDate,
                    HasAnonymousProfilePicture = user.HasProfilePic ? 0 : 1,
                    UpdatedTime = DateTimeUtilities.GetEpochTime(),
                    ProfilePicUrl = user.ProfilePicUrl,
                    Location = user.UserLocation,
                    ProcessType = processDescription,
                    Website = user.WebPageURL,
                    InteractionDateTime = DateTime.Now,
                    DirectMessage = MessageText,
                    IsVerified = user.IsVerified ? 1 : 0,
                    MediaPath = MediaPath
                };

                _dbAccountService.Add(interactedUser);
            }
            catch (Exception ex)
            {
                ex.DebugLog(
                    $"TwtDominator : [Account : AccountName] => Error in DbInsertion Message: {ex.Message} \n StackTrace : {ex.StackTrace}");
            }
        }

        public void AddInteractedTweetDetailInAccountDb(TagDetails tweetDetails, string activityType,
            string processDescription, ScrapeResultNew ScrapeResult = null, string CommentedText = null)
        {
            try
            {
                var interactedPost = new InteractedPosts
                {
                    QueryType = ScrapeResult?.QueryInfo?.QueryType,
                    QueryValue = ScrapeResult == null ? "No Query" : ScrapeResult.QueryInfo?.QueryValue,
                    ActivityType = activityType,
                    ProcessType = processDescription,
                    TweetId = tweetDetails.Id,
                    TwtMessage = tweetDetails.Caption.Replace("\n", ". ").Replace(",", " ")?.Replace("\\n","\n"),
                    UserId = tweetDetails.UserId,
                    Username = tweetDetails.Username,
                    TweetedTimeStamp = tweetDetails.TweetedTimeStamp,
                    LikeCount = tweetDetails.LikeCount,
                    CommentCount = tweetDetails.CommentCount,
                    RetweetCount = tweetDetails.RetweetCount,
                    InteractionTimeStamp = DateTimeUtilities.GetEpochTime(),
                    MediaId = tweetDetails.Code,
                    IsRetweet = tweetDetails.IsRetweet ? 1 : 0,
                    FollowStatus = tweetDetails.FollowStatus ? 1 : 0,
                    FollowBackStatus = tweetDetails.FollowBackStatus ? 1 : 0,
                    CommentedText = activityType.Equals(ActivityType.Comment.ToString()) ? CommentedText : "",
                    InteractionDate = DateTime.Now,
                    IsAlreadyLiked = tweetDetails.IsAlreadyLiked ? 1 : 0,
                    IsAlreadyRetweeted = tweetDetails.IsAlreadyRetweeted ? 1 : 0,
                    MediaType = tweetDetails.IsTweetContainedVideo ? MediaType.Video : MediaType.Image
                };
                _dbAccountService.Add(interactedPost);
            }
            catch (Exception ex)
            {
                if (AccountName != null)
                    ex.DebugLog($"Account Name : {AccountName}");
            }
        }

        public void AddInteractedUserDetailsInCampaignDb(TwitterUser user, string activityType,
            ScrapeResultNew ScrapeResult, string MessageText = null, string MediaPath = null)
        {
            try
            {
                var FollowStatus = user.FollowStatus ? 1 : 0;

                var interactedUser = new DominatorHouseCore.DatabaseHandler.TdTables.Campaign.InteractedUsers
                {
                    SinAccUsername = AccountName,
                    QueryValue = ScrapeResult?.QueryInfo?.QueryValue,
                    QueryType = ScrapeResult?.QueryInfo?.QueryType,
                    ActivityType = activityType,
                    FollowStatus = activityType.Equals(ActivityType.Follow.ToString()) ? 1 : FollowStatus,
                    FollowBackStatus = user.FollowBackStatus ? 1 : 0,
                    InteractionTimeStamp = DateTimeUtilities.GetEpochTime(),
                    InteractedUsername = user.Username,
                    InteractedUserId = user.UserId,
                    InteractedUserFullName = user.FullName,
                    FollowersCount = user.FollowersCount,
                    FollowingsCount = user.FollowingsCount,
                    TweetsCount = user.TweetsCount,
                    LikesCount = user.LikesCount,
                    Bio = user.UserBio,
                    IsPrivate = user.IsPrivate ? 1 : 0,
                    JoinedDate = user.JoiningDate,
                    HasAnonymousProfilePicture = user.HasProfilePic ? 0 : 1,
                    UpdatedTime = DateTimeUtilities.GetEpochTime(),
                    ProfilePicUrl = user.ProfilePicUrl,
                    Location = user.UserLocation,
                    Website = user.WebPageURL,
                    InteractionDateTime = DateTime.Now,
                    DirectMessage = MessageText,
                    IsVerified = user.IsVerified ? 1 : 0,
                    MediaPath = MediaPath
                };

                _campaignService.Add(interactedUser);
            }
            catch (Exception ex)
            {
                if (AccountName != null)
                    ex.DebugLog($"Account Name : {AccountName}");
            }
        }

        public void AddInteractedTweetDetailInCampaignDb(TagDetails tweetDetails, string activityType,
            ScrapeResultNew ScrapeResult, string CommentedText = null)
        {
            try
            {
                var interactedPost = new DominatorHouseCore.DatabaseHandler.TdTables.Campaign.InteractedPosts
                {
                    SinAccUsername = AccountName,
                    QueryType = ScrapeResult?.QueryInfo?.QueryType,
                    QueryValue = ScrapeResult?.QueryInfo?.QueryValue,
                    ActivityType = activityType,
                    TweetId = tweetDetails.Id,
                    TwtMessage = tweetDetails.Caption.Replace("\n", ". ").Replace(",", " ")?.Replace("\\n","\n"),
                    UserId = tweetDetails.UserId,
                    Username = tweetDetails.Username,
                    TweetedTimeStamp = tweetDetails.TweetedTimeStamp,
                    LikeCount = tweetDetails.LikeCount,
                    CommentCount = tweetDetails.CommentCount,
                    RetweetCount = tweetDetails.RetweetCount,
                    InteractionTimeStamp = DateTimeUtilities.GetEpochTime(),
                    MediaId = tweetDetails.Code,
                    IsRetweet = tweetDetails.IsRetweet ? 1 : 0,
                    FollowStatus = tweetDetails.FollowStatus ? 1 : 0,
                    FollowBackStatus = tweetDetails.FollowBackStatus ? 1 : 0,
                    CommentedText = activityType.Equals(ActivityType.Comment.ToString()) ? CommentedText : "",
                    InteractionDate = DateTime.Now,
                    IsAlreadyLiked = tweetDetails.IsAlreadyLiked ? 1 : 0,
                    IsAlreadyRetweeted = tweetDetails.IsAlreadyRetweeted ? 1 : 0,
                    MediaType = tweetDetails.IsTweetContainedVideo ? MediaType.Video : MediaType.Image,
                    TwtPostDateTime = tweetDetails.DateTime
                };
                _campaignService.Add(interactedPost);
            }
            catch (Exception ex)
            {
                if (AccountName != null)
                    ex.DebugLog($"Account Name : {AccountName}");
            }
        }

        public void AddUnfollowedDataInAccountDb(TwitterUser user, string ProcessDescription,
            ScrapeResultNew ScrapeResult = null)
        {
            try
            {
                var UnfollowedUser = new UnfollowedUsers
                {
                    SinAccUsername = AccountName,
                    SourceFilter = 0,
                    SourceType = ScrapeResult != null ? ScrapeResult.QueryInfo.QueryValue : " NA ",
                    UnfollowSource = ScrapeResult != null ? ScrapeResult.QueryInfo.QueryType : " NA ",
                    FollowBackStatus = user.FollowBackStatus ? 1 : 0,
                    UserId = user.UserId,
                    Username = user.Username,
                    InteractionDate = DateTime.Now,
                    InteractionTimeStamp = DateTimeUtilities.GetEpochTime(),
                    ProcessType = ProcessDescription
                };

                _dbAccountService.Add(UnfollowedUser);
            }
            catch (Exception ex)
            {
                if (AccountName != null)
                    ex.DebugLog($"Account Name : {AccountName}");
            }
        }

        public void AddUnfollowedDataInCampaignDb(TwitterUser user, ScrapeResultNew ScrapeResult)
        {
            try
            {
                var UnfollowedUser = new DominatorHouseCore.DatabaseHandler.TdTables.Campaign.UnfollowedUsers
                {
                    SinAccUsername = AccountName,
                    SourceFilter = 0,
                    SourceType = ScrapeResult != null ? ScrapeResult.QueryInfo?.QueryValue : " Not Found ",
                    UnfollowSource = ScrapeResult != null ? ScrapeResult.QueryInfo?.QueryType : " Not Found ",
                    FollowBackStatus = user.FollowBackStatus ? 1 : 0,
                    UserId = user.UserId,
                    Username = user.Username,
                    InteractionDate = DateTime.Now,
                    InteractionTimeStamp = DateTimeUtilities.GetEpochTime()
                };

                _campaignService.Add(UnfollowedUser);
            }
            catch (Exception ex)
            {
                if (AccountName != null)
                    ex.DebugLog($"Account Name : {AccountName}");
            }
        }

        /// <summary>
        ///     For add single data to db
        /// </summary>
        public void AddFriendshipData(TwitterUser user, FollowType followType, int sendMessageStatus)
        {
            var updatedFollowType = user.FollowStatus && user.FollowBackStatus ? FollowType.Mutual : followType;
            try
            {
                try
                {
                    // it might already contain so adding give error there removing and adding with update follow status
                    var friend = _dbAccountService.GetAllFriendships().SingleOrDefault(x => x.UserId == user.UserId);
                    if (friend != null)
                        _dbAccountService.Remove(friend);
                }
                catch (Exception ex)
                {
                    ex.ErrorLog();
                }

                // again check updated data might already present in db
                var objFriendship = _dbAccountService.GetFriendships(updatedFollowType)
                    .FirstOrDefault(x => x.UserId == user.UserId);
                if (objFriendship != null)
                {
                    if (objFriendship.FollowType == updatedFollowType) return;
                    objFriendship.FollowType = updatedFollowType;
                    objFriendship.FirstMessageStatus = sendMessageStatus;
                    _dbAccountService.Update(objFriendship);
                }
                else
                {
                    var friendship = new Friendships
                    {
                        Username = user.Username,
                        IsPrivate = user.IsPrivate ? 1 : 0,
                        UserId = user.UserId,
                        FullName = user.FullName,
                        HasAnonymousProfilePicture = user.HasProfilePic ? 0 : 1,
                        Time = DateTimeUtilities.GetEpochTime(),
                        FollowType = updatedFollowType,
                        IsMuted = user.IsMuted ? 1 : 0,
                        IsVerified = user.IsVerified ? 1 : 0,
                        FirstMessageStatus = sendMessageStatus
                    };

                    _dbAccountService.Add(friendship);
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog($"Account Name : {user.Username ?? "null"}");
            }
        }

        /// <summary>
        ///     For add list of data to db
        /// </summary>
        /// <param name="listUser"></param>
        /// <param name="followType"></param>
        /// <param name="SendMessageStatus"></param>
        /// <returns></returns>
        public int AddFriendshipListData(List<TwitterUser> listUser, FollowType followType, int SendMessageStatus)
        {
            var alreadyAddedCount = 0;
            try
            {
                var ListObjFriendship = _dbAccountService.GetAllFriendships();
                var AddUserToDb = new List<Friendships>();
                //var demo = ListObjFriendship.Where(x => x.FollowType == FollowType.Mutual).ToList();
                foreach (var user in listUser)
                {
                    // follow type is following and user is a follower then it is mutual or vice versa
                    var UpdatedFollowType = user.FollowStatus && user.FollowBackStatus ? FollowType.Mutual : followType;
                    // var all = _dbAccountService.GetFriendships();
                    var ObjFriendship = ListObjFriendship.FirstOrDefault(x => x.UserId == user.UserId);
                    if (ObjFriendship != null)
                    {
                        // added condition for updating the user when user's FollowType doesn't match 
                        if (ObjFriendship.FollowType != UpdatedFollowType)
                        {
                            ObjFriendship.FollowType = UpdatedFollowType;
                            ObjFriendship.FirstMessageStatus = SendMessageStatus;
                            _dbAccountService.Update(ObjFriendship);
                        }
                        else if (ObjFriendship.FollowType == UpdatedFollowType ||
                                 ObjFriendship.FollowType == FollowType.Mutual)
                        {
                            ++alreadyAddedCount;
                        }
                    }
                    else
                    {
                        var friendship = new Friendships
                        {
                            Username = user.Username,
                            IsPrivate = user.IsPrivate ? 1 : 0,
                            UserId = user.UserId,
                            FullName = user.FullName,
                            HasAnonymousProfilePicture = user.HasProfilePic ? 0 : 1,
                            Time = DateTimeUtilities.GetEpochTime(),
                            FollowType = UpdatedFollowType,
                            IsMuted = user.IsMuted ? 1 : 0,
                            IsVerified = user.IsVerified ? 1 : 0,
                            FirstMessageStatus = SendMessageStatus
                        };

                        AddUserToDb.Add(friendship);
                    }
                }

                if (AddUserToDb.Count != 0)
                    _dbAccountService.AddRange(AddUserToDb);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return alreadyAddedCount;
        }

        public void UpdateFriendshipData(string userId)
        {
            try
            {
                var objFriendship = _dbAccountService.GetSingle<Friendships>(x => x.UserId == userId);

                // var demo = _dbAccountService.GetAllFriendships();
                // if unFollowed user  present in db update follow type
                if (objFriendship != null)
                    switch (objFriendship.FollowType)
                    {
                        // for mutual update to Followers
                        case FollowType.Mutual:
                            objFriendship.FollowType = FollowType.Followers;
                            _dbAccountService.Update(objFriendship);
                            break;
                        // for mutual update to Following simply remove from db
                        case FollowType.Following:
                            _dbAccountService.Remove(objFriendship);
                            break;
                    }

                #region last code

                //var followActType = ActivityType.Follow.ToString();
                //var followBackActType = ActivityType.FollowBack.ToString();
                //var objInteractedUser = _dbAccountService.GetSingle<InteractedUsers>(x =>
                //    x.InteractedUserId == userId && (x.ActivityType == followActType ||
                //                                     x.ActivityType == followBackActType));
                //// if update status already present make it to 0
                //if (objInteractedUser != null)
                //{
                //    objInteractedUser.FollowStatus = 0;
                //    _dbAccountService.Update(objInteractedUser);
                //} 

                #endregion
            }
            catch (Exception ex)
            {
                if (userId != null)
                    ex.DebugLog($"Account UserId : {userId}");
            }
        }

        public void DeleteTweetFromFeedInfo(string tweetId)
        {
            try
            {
                var ObjExistingFeedInfo = _dbAccountService.GetSingle<FeedInfoes>(x => x.TweetId == tweetId);
                _dbAccountService.Remove(ObjExistingFeedInfo);
            }
            catch (Exception ex)
            {
                if (tweetId != null)
                    ex.DebugLog($"TweetId : {tweetId}");
            }
        }

        public int AddListFeedsInfo(List<TagDetails> ListTweet)
        {
            var AlreadyAddedCount = 0;
            var AddTagListToDB = new List<FeedInfoes>();
            try
            {
                var ListAllTweets = _dbAccountService.GetAllFeedInfos();
                foreach (var tweet in ListTweet)
                    try
                    {
                        var ObjExistingFeedInfo = ListAllTweets.Where(x => x.TweetId == tweet.Id);
                        if (ObjExistingFeedInfo.Any())
                        {
                            ++AlreadyAddedCount;
                            continue;
                        }

                        var classMapper = new ClassMapper(AccountId, AccountName);
                        var ObjFeedInfo = classMapper.TagDetailsToFeedsMapper(tweet);
                        AddTagListToDB.Add(ObjFeedInfo);
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                _dbAccountService.AddRange(AddTagListToDB);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return AlreadyAddedCount;
        }

        public void AddFeedsInfo(TagDetails tweet, bool InteractedBySoftware = false)
        {
            try
            {
                var ObjExistingFeedInfo = _dbAccountService.GetSingle<FeedInfoes>(x => x.TweetId == tweet.Id);
                if (ObjExistingFeedInfo != null) return;
                var classMapper = new ClassMapper(AccountId, AccountName);
                var ObjFeedInfo = classMapper.TagDetailsToFeedsMapper(tweet, InteractedBySoftware);

                _dbAccountService.Add(ObjFeedInfo);
            }
            catch (Exception ex)
            {
                if (tweet?.Id != null && tweet.Username != null)
                    ex.DebugLog($"Tweet Detail :{tweet.Id} Username :{tweet.Username}");
            }
        }

        public void UpdateMessageStatusOfFriendship(string UserID)
        {
            try
            {
                var ObjFriendship = _dbAccountService.GetSingle<Friendships>(x => x.UserId == UserID);
                if (ObjFriendship != null)
                {
                    ObjFriendship.FirstMessageStatus = 2;
                    _dbAccountService.Update(ObjFriendship);
                }
            }
            catch (Exception ex)
            {
                if (UserID != null)
                    ex.DebugLog($"User Id : {UserID}");
            }
        }


        public void UpdateAccountStatus(string userId, string userName, string accountStatus)
        {
            try
            {
                Friendships user;
                if ((user = _dbAccountService.GetAUserFromFriendships(userId)) != null)
                {
                    user.DetailedInfoWillNotBeRetrieved = 1;
                    _dbAccountService.Update(user);
                }
                // adding in to interacted user so that we can filter

                _dbAccountService.Add(new InteractedUsers
                {
                    // ActivityType = accountStatus,
                    ProfilePicUrl = accountStatus,
                    InteractedUserId = userId,
                    InteractedUsername = userName,
                    InteractionTimeStamp = DateTimeUtilities.GetEpochTime()
                });
                //var suspendedAccounts =
                //    _dbAccountService.GetList<InteractedUsers>(x => x.ProfilePicUrl == TdConstants.AccountSuspended || x.ProfilePicUrl == TdConstants.AccountBlockedToYou).Select(x => x.InteractedUserId).Distinct().ToList();
            }
            catch (Exception)
            {
            }
        }

        public List<InteractedUsers> GetInteractedUser(ActivityType activityType)
        {
            return _dbAccountService.GetInteractedUsers(activityType).ToList();
        }
    }
}