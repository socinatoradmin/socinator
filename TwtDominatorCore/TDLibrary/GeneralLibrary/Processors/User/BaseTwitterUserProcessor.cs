using System;
using System.Collections.Generic;
using System.Linq;
using CommonServiceLocator;
using ThreadUtils;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.TdTables.Accounts;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using TwtDominatorCore.Database;
using TwtDominatorCore.TDLibrary.GeneralLibrary.DAL;
using TwtDominatorCore.TDModels;
using TwtDominatorCore.TDUtility;

namespace TwtDominatorCore.TDLibrary.GeneralLibrary.Processors.User
{
    internal abstract class BaseTwitterUserProcessor : BaseTwitterProcessor
    {
        private static readonly Dictionary<string, HashSet<string>> UniqueDict =
            new Dictionary<string, HashSet<string>>();

        // protected readonly IDbCampaignService _campaignService;
        private readonly ITwitterFunctions _twitterFunctions;

        private readonly Dictionary<ActivityType, Func<QueryInfo, JobProcessResult, TagDetails, JobProcessResult>>
            ListStartTweetFinalProcess =
                new Dictionary<ActivityType, Func<QueryInfo, JobProcessResult, TagDetails, JobProcessResult>>();

        private readonly Dictionary<ActivityType, Func<QueryInfo, JobProcessResult, TwitterUser, JobProcessResult>>
            ListStartUserFinalProcess =
                new Dictionary<ActivityType, Func<QueryInfo, JobProcessResult, TwitterUser, JobProcessResult>>();

        private readonly object LockTweetScrapedUser = new object();
        private readonly IDelayService _delayService;

        protected BaseTwitterUserProcessor(ITdJobProcess jobProcess, IBlackWhiteListHandler blackWhiteListHandler,
            IDbCampaignService campaignService, ITwitterFunctions twitterFunctions,
            IDbInsertionHelper dbInsertionHelper) :
            base(jobProcess, blackWhiteListHandler, campaignService, twitterFunctions, dbInsertionHelper)
        {
            _delayService = InstanceProvider.GetInstance<IDelayService>();
            _twitterFunctions = twitterFunctions;

            // Start process for tweets list
            ListStartTweetFinalProcess.Add(ActivityType.Follow, StartTweetFinalProcessFollow);
            ListStartTweetFinalProcess.Add(ActivityType.Mute, StartTweetFinalProcessMute);
            ListStartTweetFinalProcess.Add(ActivityType.TweetTo, StartTweetFinalProcessTweetTo);
            ListStartTweetFinalProcess.Add(ActivityType.UserScraper, StartTweetFinalProcessUserScraper);

            // Start process for users list
            ListStartUserFinalProcess.Add(ActivityType.Follow, StartUserFinalProcessFollow);
            ListStartUserFinalProcess.Add(ActivityType.Mute, StartUserFinalProcessMute);
            ListStartUserFinalProcess.Add(ActivityType.FollowBack, StartUserFinalProcessFollowBack);
            ListStartUserFinalProcess.Add(ActivityType.BroadcastMessages, StartUserFinalProcessBroadcastMessages);
            ListStartUserFinalProcess.Add(ActivityType.AutoReplyToNewMessage,
                StartUserFinalProcessAutoReplyToNewMessage);
            ListStartUserFinalProcess.Add(ActivityType.UserScraper, StartUserFinalProcessUserScraper);
            ListStartUserFinalProcess.Add(ActivityType.TweetTo, StartUserFinalProcessTweetTo);
            ListStartUserFinalProcess.Add(ActivityType.SendMessageToFollower,
                StartUserFinalProcessSendMessageToFollower);
            ListStartUserFinalProcess.Add(ActivityType.WelcomeTweet, StartUserFinalProcessWelcomeTweet);
        }

        public JobProcessResult StartFinalProcess(QueryInfo QueryInfo, JobProcessResult jobProcessResult,
            List<TwitterUser> LstTwitterUser)
        {
            // remove users already followed 
            LstTwitterUser = UniqueListTwtUser(LstTwitterUser);
            foreach (var EachUser in LstTwitterUser)
            {
                _jobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                try
                {
                    if (_jobProcess.ModuleSetting.IsCampaignWiseUniqueChecked &&
                        (IsActivityDoneWithThisUserIdCampaignWise(EachUser.UserId) || !IsUniqueUser(EachUser)) &&
                        _jobProcess.ActivityType != ActivityType.UserScraper)
                        continue;
                    jobProcessResult = ListStartUserFinalProcess[ActivityType](QueryInfo, jobProcessResult, EachUser);

                    if (jobProcessResult.IsProcessCompleted)
                        break;
                }
                catch (Exception ex)
                {
                    if (_jobProcess.DominatorAccountModel?.AccountBaseModel?.UserName != null)
                        ex.DebugLog(
                            $"TwtDominator : [Account: {_jobProcess.DominatorAccountModel.AccountBaseModel.UserName}]   (Module => {ActivityType.ToString()})");
                }
            }

            _delayService.ThreadSleep(TdConstants.ConsecutiveGetReqInteval);
            return jobProcessResult;
        }

        private List<TwitterUser> UniqueListTwtUser(List<TwitterUser> ListTwitterUser)
        {
            var UpdatedTweetUsers = new List<TwitterUser>();
            try
            {
                UpdatedTweetUsers.AddRange(ListTwitterUser);
                if (ActivityType.Equals(ActivityType.Follow))
                {
                    var InteractedUsers = _dbAccountService.GetInteractedUsers(ActivityType)
                        .Select(x => x.InteractedUsername).ToList();
                    UpdatedTweetUsers.RemoveAll(x => InteractedUsers.Contains(x.Username));
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return UpdatedTweetUsers;
        }


        protected bool IsUniqueUserFromTweets(TagDetails EachUserTag)
        {
            var IsDuplicate = false;
            try
            {
                lock (LockTweetScrapedUser)
                {
                    var User = new TwitterUser
                    {
                        Username = EachUserTag.Username,
                        UserId = EachUserTag.UserId,
                        FollowStatus = EachUserTag.FollowStatus,
                        FollowBackStatus = EachUserTag.FollowBackStatus,
                        IsPrivate = EachUserTag.IsPrivate,
                        HasProfilePic = EachUserTag.HasProfilePic,
                        IsVerified = EachUserTag.IsVerified,
                        IsMuted = EachUserTag.IsMuted
                    };
                    IsDuplicate = !IsUniqueUser(User) ||
                                  _campaignService.DoesInteractedUserExist(EachUserTag.UserId, ActivityType);
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return IsDuplicate;
        }

        protected List<InteractedUsers> RandomizeDictionary(List<InteractedUsers> originalDictionary)
        {
            var currentDictionary = originalDictionary;
            try
            {
                var random = new Random();
                currentDictionary = originalDictionary.OrderBy(x => random.Next()).ToList();
            }
            catch (Exception exception)
            {
                exception.DebugLog();
            }

            return currentDictionary;
        }

        protected bool IsUniqueUser(TwitterUser user)
        {
            lock (InteractedUserObject)
            {
                if (_campaignService?.CampaignId == null)
                    return true;

                if (!UniqueDict.ContainsKey(_campaignService.CampaignId))
                    UniqueDict.Add(_campaignService.CampaignId,
                        ActivityType == ActivityType.Follow
                            ? _campaignService.GetAllUnfollowedUsers().Select(a => a.UserId).ToHashSet()
                            : _campaignService.GetAllInteractedUsers().Select(a => a.InteractedUserId).ToHashSet());

                return UniqueDict[_campaignService.CampaignId].Add(user.UserId);
            }
        }


        #region Start Tweet Final Process

        private JobProcessResult StartTweetFinalProcessFollow(QueryInfo queryInfo, JobProcessResult jobProcessResult,
            TagDetails eachUserTag)
        {
            try
            {
                if (eachUserTag.FollowStatus || _jobProcess.ModuleSetting.IsCampaignWiseUniqueChecked &&
                    IsUniqueUserFromTweets(eachUserTag))
                    return jobProcessResult;

                TwitterUser user;
                if (IsUserFilterActive || IsTweetFilterActive)
                {
                    #region Scrape all the details from User profile page and apply tweet filter

                    var userDetailsHandler = TwitterFunction.GetUserDetails(_jobProcess.DominatorAccountModel,
                        eachUserTag.Username, queryInfo.QueryType);
                    if (!userDetailsHandler.Success ||
                        TweetFilterApply(userDetailsHandler.UserDetail.TagDetail, queryInfo))
                        return jobProcessResult;
                    user = userDetailsHandler.UserDetail;

                    #endregion
                }
                else
                {
                    if (eachUserTag.twitterUser is null)
                    {
                        user = new TwitterUser
                        {
                            Username = eachUserTag.Username,
                            UserId = eachUserTag.UserId,
                            FollowStatus = eachUserTag.FollowStatus,
                            FollowBackStatus = eachUserTag.FollowBackStatus,
                            IsPrivate = eachUserTag.IsPrivate,
                            HasProfilePic = eachUserTag.HasProfilePic,
                            IsVerified = eachUserTag.IsVerified,
                            IsMuted = eachUserTag.IsMuted
                        };
                    }
                    else
                        user = eachUserTag.twitterUser;
                    
                }
                FinalProcessForEachUser(queryInfo, out jobProcessResult, user);
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return jobProcessResult;
        }

        private JobProcessResult StartTweetFinalProcessMute(QueryInfo queryInfo, JobProcessResult jobProcessResult,
            TagDetails eachUserTag)
        {
            try
            {
                if (eachUserTag.IsMuted) return jobProcessResult;
                TwitterUser user;
                if (IsUserFilterActive || IsTweetFilterActive)
                {
                    #region Scrape all the details from User profile page and apply tweet filter

                    var userDetailsHandler = _twitterFunctions.GetUserDetails(_jobProcess.DominatorAccountModel,
                        eachUserTag.Username, queryInfo.QueryType);
                    if (!userDetailsHandler.Success ||
                        TweetFilterApply(userDetailsHandler.UserDetail.TagDetail, queryInfo))
                        return jobProcessResult;
                    user = userDetailsHandler.UserDetail;

                    #endregion
                }
                else
                {
                    user = new TwitterUser
                    {
                        Username = eachUserTag.Username,
                        UserId = eachUserTag.UserId,
                        FollowStatus = eachUserTag.FollowStatus,
                        FollowBackStatus = eachUserTag.FollowBackStatus,
                        IsPrivate = eachUserTag.IsPrivate,
                        HasProfilePic = eachUserTag.HasProfilePic,
                        IsVerified = eachUserTag.IsVerified,
                        IsMuted = eachUserTag.IsMuted
                    };
                }

                FinalProcessForEachUser(queryInfo, out jobProcessResult, user);
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return jobProcessResult;
        }

        private JobProcessResult StartTweetFinalProcessTweetTo(QueryInfo queryInfo, JobProcessResult jobProcessResult,
            TagDetails eachUserTag)
        {
            try
            {
                // unique user from listTweets for TweetTo
                if (IsUniqueUserFromTweets(eachUserTag))
                    return jobProcessResult;

                TwitterUser user;
                if (IsUserFilterActive || IsTweetFilterActive)
                {
                    #region Scrape all the details from User profile page and apply tweet filter

                    var userDetailsHandler = _twitterFunctions.GetUserDetails(_jobProcess.DominatorAccountModel,
                        eachUserTag.Username, queryInfo.QueryType);
                    if (!userDetailsHandler.Success ||
                        TweetFilterApply(userDetailsHandler.UserDetail.TagDetail, queryInfo))
                        return jobProcessResult;
                    user = userDetailsHandler.UserDetail;

                    #endregion
                }
                else
                {
                    user = new TwitterUser
                    {
                        Username = eachUserTag.Username,
                        UserId = eachUserTag.UserId,
                        FollowStatus = eachUserTag.FollowStatus,
                        FollowBackStatus = eachUserTag.FollowBackStatus,
                        IsPrivate = eachUserTag.IsPrivate,
                        HasProfilePic = eachUserTag.HasProfilePic,
                        IsVerified = eachUserTag.IsVerified,
                        IsMuted = eachUserTag.IsMuted
                    };
                }

                FinalProcessForEachUser(queryInfo, out jobProcessResult, user);
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return jobProcessResult;
        }

        private JobProcessResult StartTweetFinalProcessUserScraper(QueryInfo QueryInfo,
            JobProcessResult jobProcessResult, TagDetails eachUserTag)
        {
            try
            {
                try
                {
                    // delay only for campaign so that all account get enough delay for unique
                    if (_campaignService?.CampaignId != null)
                        _delayService.ThreadSleep(new Random().Next(1000, 5000));

                    if (IsUniqueUserFromTweets(eachUserTag))
                        return jobProcessResult;
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                var userDetailsHandler = TwitterFunction.GetUserDetails(_jobProcess.DominatorAccountModel,
                    eachUserTag.Username, QueryInfo.QueryType);
                FinalProcessForEachUser(QueryInfo, out jobProcessResult, userDetailsHandler.UserDetail);
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return jobProcessResult;
        }

        #endregion


        #region  Start User Final Process

        private JobProcessResult StartUserFinalProcessFollow(QueryInfo queryInfo, JobProcessResult jobProcessResult,
            TwitterUser eachUser)
        {
            try
            {
                if (eachUser.FollowStatus) return jobProcessResult;
                TwitterUser user;
                if (IsUserFilterActive || IsTweetFilterActive)
                {
                    #region Scrape all the details from User profile page and apply tweet filter

                    var userDetailsHandler = _twitterFunctions.GetUserDetails(_jobProcess.DominatorAccountModel,
                        eachUser.Username, queryInfo.QueryType);
                    if (!userDetailsHandler.Success ||
                        TweetFilterApply(userDetailsHandler.UserDetail.TagDetail, queryInfo))
                        return jobProcessResult;
                    user = userDetailsHandler.UserDetail;

                    #endregion
                }
                else
                {
                    user = eachUser;
                }

                FinalProcessForEachUser(queryInfo, out jobProcessResult, user);
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return jobProcessResult;
        }

        private JobProcessResult StartUserFinalProcessMute(QueryInfo queryInfo, JobProcessResult jobProcessResult,
            TwitterUser eachUser)
        {
            try
            {
                if (eachUser.IsMuted) return jobProcessResult;
                TwitterUser user;
                if (IsUserFilterActive || IsTweetFilterActive)
                {
                    #region Scrape all the details from User profile page and apply tweet filter

                    var userDetailsHandler = _twitterFunctions.GetUserDetails(_jobProcess.DominatorAccountModel,
                        eachUser.Username, queryInfo.QueryType);
                    if (!userDetailsHandler.Success ||
                        TweetFilterApply(userDetailsHandler.UserDetail.TagDetail, queryInfo))
                        return jobProcessResult;
                    user = userDetailsHandler.UserDetail;

                    #endregion
                }
                else
                {
                    user = eachUser;
                }

                FinalProcessForEachUser(queryInfo, out jobProcessResult, user);
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return jobProcessResult;
        }

        private JobProcessResult StartUserFinalProcessFollowBack(QueryInfo queryInfo, JobProcessResult jobProcessResult,
            TwitterUser eachUser)
        {
            try
            {
                if (eachUser.FollowStatus) return jobProcessResult;
                TwitterUser user;
                if (IsUserFilterActive || IsTweetFilterActive)
                {
                    #region Scrape all the details from User profile page and apply tweet filter

                    var userDetailsHandler = _twitterFunctions.GetUserDetails(_jobProcess.DominatorAccountModel,
                        eachUser.Username, queryInfo.QueryType);
                    if (!userDetailsHandler.Success ||
                        TweetFilterApply(userDetailsHandler.UserDetail.TagDetail, queryInfo))
                        return jobProcessResult;

                    user = userDetailsHandler.UserDetail;

                    #endregion
                }
                else
                {
                    user = eachUser;
                }

                FinalProcessForEachUser(queryInfo, out jobProcessResult, user);
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return jobProcessResult;
        }

        private JobProcessResult StartUserFinalProcessBroadcastMessages(QueryInfo queryInfo,
            JobProcessResult jobProcessResult, TwitterUser eachUser)
        {
            try
            {
                if (_jobProcess.ModuleSetting.UserFilterModel.IsSkipUsersWhoWereAlreadySentAMessageFromSoftware && _dbAccountService.IsActivityDoneWithThisUserId(eachUser.UserId, ActivityType))
                    return jobProcessResult;
                if (_campaignService.IsActivityDoneWithThisUserId(eachUser.UserId, ActivityType))
                    return jobProcessResult;
                TwitterUser user;
                if (IsUserFilterActive)
                {
                    #region Scrape all the details from User profile page and apply tweet filter

                    var userDetailsHandler = _twitterFunctions.GetUserDetails(_jobProcess.DominatorAccountModel,
                        eachUser.Username, queryInfo.QueryType);
                    if (!userDetailsHandler.Success)
                        return jobProcessResult;
                    user = userDetailsHandler.UserDetail;

                    #endregion
                }
                else
                {
                    user = eachUser;
                }

                FinalProcessForEachUser(queryInfo, out jobProcessResult, user);
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return jobProcessResult;
        }

        private JobProcessResult StartUserFinalProcessAutoReplyToNewMessage(QueryInfo queryInfo,
            JobProcessResult jobProcessResult, TwitterUser eachUser)
        {
            if (queryInfo == null) throw new ArgumentNullException(nameof(queryInfo));
            if (jobProcessResult == null) throw new ArgumentNullException(nameof(jobProcessResult));
            FinalProcessForEachUser(queryInfo, out jobProcessResult, eachUser);
            return jobProcessResult;
        }

        private JobProcessResult StartUserFinalProcessUserScraper(QueryInfo QueryInfo,
            JobProcessResult jobProcessResult, TwitterUser EachUser)
        {
            try
            {
                if (IsActivityDoneWithThisUserIdCampaignWise(EachUser.UserId) ||
                    !IsUniqueUser(EachUser))
                    return jobProcessResult;
                var userDetailsHandler = TwitterFunction.GetUserDetails(_jobProcess.DominatorAccountModel,
                    EachUser.Username, QueryInfo.QueryType);
                userDetailsHandler.UserDetail.JoiningDate = EachUser.JoiningDate;
                if (!userDetailsHandler.Success ||
                    TweetFilterApply(userDetailsHandler.UserDetail.TagDetail, QueryInfo))
                    return jobProcessResult;
                FinalProcessForEachUser(QueryInfo, out jobProcessResult, userDetailsHandler.UserDetail);
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return jobProcessResult;
        }

        private JobProcessResult StartUserFinalProcessTweetTo(QueryInfo QueryInfo, JobProcessResult jobProcessResult,
            TwitterUser EachUser)
        {
            try
            {
                #region Scrape all details and apply tweet filter

                if (IsActivityDoneWithThisUserIdCampaignWise(EachUser.UserId) ||
                    !IsUniqueUser(EachUser))
                    return jobProcessResult;
                TwitterUser user;
                if (IsUserFilterActive)
                {
                    #region Scrape all the details from User profile page and apply tweet filter

                    var userDetailsHandler = TwitterFunction.GetUserDetails(_jobProcess.DominatorAccountModel,
                        EachUser.Username, QueryInfo.QueryType);
                    if (!userDetailsHandler.Success)
                        return jobProcessResult;
                    user = userDetailsHandler.UserDetail;

                    #endregion
                }
                else
                {
                    user = EachUser;
                }

                FinalProcessForEachUser(QueryInfo, out jobProcessResult, user);

                #endregion
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }

            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return jobProcessResult;
        }

        private JobProcessResult StartUserFinalProcessSendMessageToFollower(QueryInfo QueryInfo,
            JobProcessResult jobProcessResult, TwitterUser EachUser)
        {
            try
            {
                TwitterUser user;
                if (IsUserFilterActive)
                {
                    #region Scrape all the details from User profile page and apply tweet filter

                    var userDetailsHandler = TwitterFunction.GetUserDetails(_jobProcess.DominatorAccountModel,
                        EachUser.Username, QueryInfo.QueryType);
                    if (!userDetailsHandler.Success)
                        return jobProcessResult;
                    user = userDetailsHandler.UserDetail;

                    #endregion
                }
                else
                {
                    user = EachUser;
                }

                FinalProcessForEachUser(QueryInfo, out jobProcessResult, user);
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return jobProcessResult;
        }

        private JobProcessResult StartUserFinalProcessWelcomeTweet(QueryInfo QueryInfo,
            JobProcessResult jobProcessResult, TwitterUser EachUser)
        {
            try
            {
                var user = EachUser;
                if (IsUserFilterActive)
                    user = TwitterFunction
                        .GetUserDetails(_jobProcess.DominatorAccountModel, user.Username, QueryInfo.QueryType)
                        .UserDetail;
                FinalProcessForEachUser(QueryInfo, out jobProcessResult, user);
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return jobProcessResult;
        }

        /// <summary>
        ///     like,retweet,reposter,comment
        /// </summary>
        /// <param name="queryInfo"></param>
        /// <param name="jobProcessResult"></param>
        /// <param name="lstUserTag"></param>
        /// <returns></returns>
        //private JobProcessResult StartUserFinalProcessTweetActions(QueryInfo QueryInfo, JobProcessResult jobProcessResult, TwitterUser EachUser)
        //{
        //    jobProcessResult = StartFinalProcessForTags(QueryInfo, jobProcessResult, EachUser.Username);
        //    return jobProcessResult;
        //}

        #endregion

        public JobProcessResult StartFinalProcess(QueryInfo queryInfo, JobProcessResult jobProcessResult,
            List<TagDetails> lstUserTag)
        {
            UniqueListTwtUser(ref lstUserTag);
            foreach (var eachUserTag in lstUserTag)
            {
                _jobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                jobProcessResult = ListStartTweetFinalProcess[ActivityType](queryInfo, jobProcessResult, eachUserTag);

                if (jobProcessResult.IsProcessCompleted)
                    break;
            }

            _delayService.ThreadSleep(TdConstants.ConsecutiveGetReqInteval);

            return jobProcessResult;
        }
    }
}