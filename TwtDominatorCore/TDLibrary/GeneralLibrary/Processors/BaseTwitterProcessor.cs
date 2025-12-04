using System;
using System.Collections.Generic;
using System.Linq;
using CommonServiceLocator;
using ThreadUtils;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.TdTables.Accounts;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using Newtonsoft.Json;
using TwtDominatorCore.Database;
using TwtDominatorCore.TDLibrary.GeneralLibrary.DAL;
using TwtDominatorCore.TDModels;
using TwtDominatorCore.TDUtility;

namespace TwtDominatorCore.TDLibrary.GeneralLibrary.Processors
{
    internal abstract class BaseTwitterProcessor
    {
        private readonly IDelayService _delayService;
        private int _currentCount = 0;

        protected BaseTwitterProcessor(ITdJobProcess jobProcess, IBlackWhiteListHandler blackWhiteListHandler,
            IDbCampaignService campaignService, ITwitterFunctions twitterFunctions,
            IDbInsertionHelper dbInsertionHelper)
        {
            try
            {
                _jobProcess = jobProcess;
                _dbAccountService = _jobProcess.DbAccountService;
                _campaignService = campaignService;
                TwitterFunction = twitterFunctions;
                DbInsertionHelper = dbInsertionHelper;
                jobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                AccountModel = new AccountModel(_jobProcess.DominatorAccountModel);
                BlackWhiteListHandler = blackWhiteListHandler;

                #region  GetModuleSetting

                var jobActivityConfigurationManager =
                    InstanceProvider.GetInstance<IJobActivityConfigurationManager>();
                var moduleModeSetting =
                    jobActivityConfigurationManager[_jobProcess.DominatorAccountModel.AccountId, ActivityType];

                if (moduleModeSetting == null)
                    return;

                #endregion

                _delayService = InstanceProvider.GetInstance<IDelayService>();

                InitializeFilters();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void Start(QueryInfo queryInfo)
        {
            try
            {
                _jobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                var jobProcessResult = new JobProcessResult();
                if(_jobProcess.ActivityType != ActivityType.Unlike && !string.IsNullOrEmpty(queryInfo.QueryType) && !string.IsNullOrEmpty(queryInfo.QueryValue))
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                    _jobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    _jobProcess.DominatorAccountModel.UserName, _jobProcess.ActivityType,
                    string.Format("LangKeySearchingFor".FromResourceDictionary(), queryInfo.QueryType,
                        queryInfo.QueryValue));
                TwitterFunction.SetBrowser(_jobProcess.DominatorAccountModel,
                    _jobProcess.JobCancellationTokenSource.Token);
                Process(queryInfo, ref jobProcessResult);
            }
            catch (OperationCanceledException)
            {
                TdAccountsBrowserDetails.CloseAllBrowser(_jobProcess.DominatorAccountModel);
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                TdAccountsBrowserDetails.CloseAllBrowser(_jobProcess.DominatorAccountModel);
                ex.DebugLog();
            }
        }


        protected abstract void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult);

        protected void CheckUserFilterActiveForCurrentQuery(QueryInfo QueryInfo)
        {
            if (QueryInfo.IsCustomFilterSelected)
            {
                UserFilterModel userfilter = null;
                try
                {
                    userfilter = JsonConvert.DeserializeObject<UserFilterModel>(QueryInfo.CustomFilters);
                }
                catch
                {
                    //todo: write reason why we skip it
                }

                if (userfilter != null)
                    _jobProcess.ModuleSetting.UserFilterModel = userfilter;
                InitializeFilters();
            }
        }

        private void InitializeFilters()
        {
            UserFilter = new TDFilters.UserFilterFunctions(_jobProcess.ModuleSetting);
            TweetFilter = new TDFilters.TweetFilterFunctions(_jobProcess.ModuleSetting);
            IsUserFilterActive = UserFilter.IsUserFilterActive();
            IsTweetFilterActive = TweetFilter.IsTweetFilterActive();
        }

        protected void InitializeTweetScrapePerUser()
        {
            if (_jobProcess.ModuleSetting.IsChkActionTweetPerUser)
                NoOfTweetsToBeScrapePerUser = _jobProcess.ModuleSetting.NoOfActionTweetPerUser.StartValue;
        }

        /// <summary>
        ///     taking ActivityType:QueryType:QueryValue combination as a key
        ///     current user for FollowersOfFollowers and FollowersOfFollowings
        /// </summary>
        /// <param name="queryInfo"></param>
        /// <param name="PaginationId"></param>
        /// <param name="IsToAddPagination"></param>
        /// <param name="currentUser"></param>
        protected void AddOrUpdatePaginationId(QueryInfo queryInfo, string PaginationId, ref bool IsToAddPagination)
        {
            try
            {
                if (string.IsNullOrEmpty(PaginationId) || !IsToAddPagination)
                {
                    IsToAddPagination = true;
                    return;
                }

                if (!_jobProcess.DominatorAccountModel.IsRunProcessThroughBrowser)
                {
                    var QueryCombination =
                        $"{_jobProcess.ActivityType}:{queryInfo.QueryType}:{queryInfo.QueryValue}:{_jobProcess.DominatorAccountModel.UserName}:{queryInfo.Id}";

                    if (_jobProcess.ActivityType.Equals(ActivityType.Unfollow))
                        QueryCombination =
                            $"{_jobProcess.ActivityType}:{queryInfo.QueryType}:{queryInfo.QueryValue}:{_jobProcess.DominatorAccountModel.UserName}:{((TdJobProcess) _jobProcess)?.CampaignId}";

                    SocinatorAccountBuilder.Instance(_jobProcess.DominatorAccountModel.AccountBaseModel.AccountId)
                        .AddOrUpdatePaginationId(QueryCombination, PaginationId).SaveToBinFile();
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        protected string GetPaginationId(QueryInfo queryInfo, string currentUser = "")
        {
            var paginationId = string.Empty;
            try
            {
                //Unfollow:Followed outside software:Who are not Following back:allanglobus:1140033c-bd92-4043-91c1-4fee60213827
                var accountsFileManager = InstanceProvider.GetInstance<IAccountsFileManager>();
                var dominatorAccountModel =
                    accountsFileManager.GetAccountById(_jobProcess.DominatorAccountModel.AccountId);
                if (!dominatorAccountModel.IsRunProcessThroughBrowser)
                {
                    var queryCombination =
                        $"{_jobProcess.ActivityType}:{queryInfo.QueryType}:{queryInfo.QueryValue}:{_jobProcess.DominatorAccountModel.UserName}:{queryInfo.Id}";

                    if (_jobProcess.ActivityType.Equals(ActivityType.Unfollow))
                        queryCombination =
                            $"{_jobProcess.ActivityType}:{queryInfo.QueryType}:{queryInfo.QueryValue}:{_jobProcess.DominatorAccountModel.UserName}:{((TdJobProcess) _jobProcess)?.CampaignId}";

                    dominatorAccountModel.PaginationId.TryGetValue(queryCombination, out paginationId);
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return paginationId;
        }

        #region Fields

        protected readonly ITdJobProcess _jobProcess;
        protected readonly IDbAccountService _dbAccountService;
        protected readonly IDbCampaignService _campaignService;
        protected readonly ITwitterFunctions TwitterFunction;
        protected readonly IDbInsertionHelper DbInsertionHelper;

        private static Dictionary<string, HashSet<string>> UniqueDict { get; set; }

        protected static readonly object InteractedUserObject = new object();

        protected ActivityType ActivityType => _jobProcess.ActivityType;

        protected readonly AccountModel AccountModel;
        private readonly List<string> TotalListOfUserNameScraped = new List<string>();
        protected int NoOfTweetsToBeScrapePerUser = 1000;
        private readonly object LockScrapedUser = new object();
        private readonly object LockReachedMaxTweetActionPerUser = new object();

        #region Filters

        protected TDFilters.UserFilterFunctions UserFilter;
        protected TDFilters.TweetFilterFunctions TweetFilter;
        protected bool IsUserFilterActive;
        protected bool IsTweetFilterActive;

        #endregion

        protected readonly IBlackWhiteListHandler BlackWhiteListHandler;

        #endregion

        #region Final Processes

        protected bool CheckActionTweetPerUser(string user, int maxCount)
        {
            try
            {
                if (_jobProcess.ModuleSetting.IsChkActionTweetPerUser &&
                    (_jobProcess.ActivityType == ActivityType.Reposter ||
                     _jobProcess.ActivityType == ActivityType.Retweet
                     || _jobProcess.ActivityType == ActivityType.Like ||
                     _jobProcess.ActivityType == ActivityType.Comment) &&
                    _jobProcess.ModuleSetting.IsChkActionTweetPerUser &&
                    IsReachedMaxTweetActionPerUser(user, maxCount))
                    return true;
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }

            return false;
        }

        protected bool IsReachedMaxTweetActionPerUser(string user, int maxCount)
        {
            var count = 0;
            lock (LockReachedMaxTweetActionPerUser)
            {
                if (string.IsNullOrEmpty(_campaignService?.CampaignId))
                    count = _dbAccountService.GetInteractedPosts(user, ActivityType).ToList().Count;
                else
                    count = _campaignService.GetInteractedPosts(user, ActivityType).ToList().Count;
                return maxCount <= count;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="QueryInfo"></param>
        /// <param name="jobProcessResult"></param>
        /// <param name="User"></param>
        public void FinalProcessForEachUser(QueryInfo QueryInfo, out JobProcessResult jobProcessResult,
            TwitterUser User)
        {
            // Updated from normal mode

            #region  GetModuleSetting

            var jobActivityConfigurationManager =
                InstanceProvider.GetInstance<IJobActivityConfigurationManager>();
            var moduleModeSetting =
                jobActivityConfigurationManager[_jobProcess.DominatorAccountModel.AccountId, ActivityType];


            if (moduleModeSetting == null)
            {
                jobProcessResult = new JobProcessResult();
                return;
            }
            if (moduleModeSetting.IsTemplateMadeByCampaignMode &&
                    _jobProcess.ModuleSetting.IsCampaignWiseUniqueChecked &&
                    !IsUniqueUser(User))
            {
                jobProcessResult = new JobProcessResult();
                return;
            }
            #endregion

            _jobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

            if (!UserFilterApply(User))
            {
                GlobusLogHelper.log.Info(Log.StartedActivity,
                    _jobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    _jobProcess.DominatorAccountModel.UserName, ActivityType, User.Username);

                jobProcessResult = _jobProcess.FinalProcess(new ScrapeResultNew
                {
                    ResultUser = User,
                    QueryInfo = QueryInfo
                });
            }
            else if(ActivityType == ActivityType.Follow && User.FollowStatus)
            {
                GlobusLogHelper.log.Info(Log.CustomMessage,
                                        _jobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                        _jobProcess.DominatorAccountModel.UserName, ActivityType,
                                        string.Format($"LangKeyYouAreAlreadyFollowingThisUser".FromResourceDictionary(), $"{User.FullName}"));
                jobProcessResult = new JobProcessResult();
            }
            else
            {
                GlobusLogHelper.log.Info(Log.CustomMessage,
                                        _jobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                        _jobProcess.DominatorAccountModel.UserName, ActivityType,
                                        $"Filter Not Matched For {User.FullName}");
                jobProcessResult = new JobProcessResult();
            }
        }

        public void FinalProcessForEachTag(QueryInfo QueryInfo, out JobProcessResult jobProcessResult, TagDetails Tag)
        {
            try
            {
                // Updated from normal mode

                #region  GetModuleSetting

                var jobActivityConfigurationManager =
                    InstanceProvider.GetInstance<IJobActivityConfigurationManager>();
                var moduleModeSettings =
                    jobActivityConfigurationManager[_jobProcess.DominatorAccountModel.AccountId, ActivityType];

                if (moduleModeSettings == null)
                {
                    jobProcessResult = new JobProcessResult();
                    return;
                }

                #endregion

                #region campaign wise uniqueness

                if (moduleModeSettings.IsTemplateMadeByCampaignMode &&
                    _jobProcess.ModuleSetting.IsCampaignWiseUniqueChecked &&
                    !IsUniqueTweet(Tag))
                {
                    jobProcessResult = new JobProcessResult();
                    return;
                }

                #endregion

                _jobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                #region logger info setting before performing task

                try
                {
                    // only reason for setting logger here so that user not get confused with undo retweet, comment, tweet
                    // because otherwise it activity type in logger
                    //  setting  logger info for delete  and other activities
                    if (ActivityType.ToString() == "LangKeyDelete".FromResourceDictionary() &&
                        QueryInfo.QueryType == "LangKeyRetweet".FromResourceDictionary())
                        GlobusLogHelper.log.Info(Log.StartedActivity,
                            _jobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            _jobProcess.DominatorAccountModel.UserName,
                            "LangKeyUndoRetweets".FromResourceDictionary(), TdUtility.GetTweetUrl(Tag.Username,Tag.Id));

                    else if (ActivityType.ToString() == "LangKeyDelete".FromResourceDictionary() &&
                             QueryInfo.QueryType == "LangKeyComment".FromResourceDictionary())
                        GlobusLogHelper.log.Info(Log.StartedActivity,
                            _jobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            _jobProcess.DominatorAccountModel.UserName,
                            "LangKeyDeleteComment".FromResourceDictionary(), TdUtility.GetTweetUrl(Tag.Username, Tag.Id));
                    else if(ActivityType != ActivityType.Like)
                        GlobusLogHelper.log.Info(Log.StartedActivity,
                            _jobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            _jobProcess.DominatorAccountModel.UserName,
                            ActivityType, TdUtility.GetTweetUrl(Tag.Username, Tag.Id));
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                #endregion

                if (!TweetFilterApply(Tag, QueryInfo))
                {
                    jobProcessResult = _jobProcess.FinalProcess(new ScrapeResultNew
                    {
                        ResultPost = Tag,
                        ResultUser = Tag,
                        QueryInfo = QueryInfo
                    });
                }
                else
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                        _jobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        _jobProcess.DominatorAccountModel.UserName, ActivityType,
                        string.Format("LangKeyFilteredTweetWithId".FromResourceDictionary(), Tag.Id));
                    _jobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    jobProcessResult = new JobProcessResult();
                    _delayService.ThreadSleep(TdConstants.ConsecutiveGetReqInteval);
                }

                // delay using for scraper module we don't have process to give delay 
                //if (ActivityType.Equals(ActivityType.TweetScraper))
                //  _jobProcess.DelayBeforeNextActivity();
            }
            catch (OperationCanceledException)
            {
                TdAccountsBrowserDetails.CloseAllBrowser(_jobProcess.DominatorAccountModel,
                    true); // replace true with IsBrowser
                throw new OperationCanceledException("Operation Cancelled!");
            }
        }

        #endregion

        #region Filteration

        #region Tweet Filter

        protected void TweetFilterApply(List<TagDetails> lstTagDetails)
        {
            var TweetFilter = new TDFilters.TweetFilterFunctions(_jobProcess.ModuleSetting);
            try
            {
                TweetFilter.FilterAlreadyLiked(lstTagDetails);
                TweetFilter.FilterAlreadyRetweeted(lstTagDetails);
                TweetFilter.FilterByFavoritesRange(lstTagDetails);
                TweetFilter.FilterByRetweetsRange(lstTagDetails);
                TweetFilter.FilterOldTweets(lstTagDetails);
                TweetFilter.FilterTweetByRestrictedWords(lstTagDetails);
                TweetFilter.FilterTweetContainingAtSign(lstTagDetails);
                TweetFilter.FilterTweetsWithNonEnglishCharacter(lstTagDetails);
                TweetFilter.FilterWithLinks(lstTagDetails);
                TweetFilter.FilterWithoutLinks(lstTagDetails);
                TweetFilter.FilterIsRetweetedTweet(lstTagDetails);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        protected bool TweetFilterApply(TagDetails tagDetails, QueryInfo query)
        {
            var TweetFilter = new TDFilters.TweetFilterFunctions(_jobProcess.ModuleSetting);

            #region if user is private

            if (tagDetails == null)
                return TweetFilter.IsTweetFilterActive();

            #endregion

            #region if allready Activity Done

            // moved from here to CommentProcess PostScrapeProcess
            // having issue of multiple comments issue since we don't have comment model here to check it.

            //if (ActivityType == ActivityType.Comment && _dbAccountService.IsAlreadyCommentedOnTweetwithSameQuery(
            //       query.QueryType, query.QueryValue,
            //       tagDetails.Id, ActivityType))
            //   return true;

            if (ActivityType == ActivityType.Like && tagDetails.IsAlreadyLiked)
                return true;
            if (ActivityType == ActivityType.Retweet && tagDetails.IsAlreadyRetweeted)
                return true;
            if (ActivityType == ActivityType.Unlike && !tagDetails.IsAlreadyLiked)
                return true;

            if (query.QueryType == "CustomTweetsList" || query.QueryType == "CustomUsersList" &&
                _dbAccountService.IsActivtyDoneWithThisTweetId(tagDetails.Id, ActivityType))
                return true;

            #endregion

            if (ActivityType == ActivityType.DownloadScraper && string.IsNullOrEmpty(tagDetails.Code) &&
                !tagDetails.IsTweetContainedVideo)
                return true;

            if (TweetFilter.FilterAlreadyLiked(tagDetails))
                return true;
            if (TweetFilter.FilterAlreadyRetweeted(tagDetails))
                return true;
            // check later
            if (ActivityType == ActivityType.Reposter &&
                TweetFilter.FilterAlreadyReposted(tagDetails, _dbAccountService))
                return true;
            if (TweetFilter.FilterByFavoritesRange(tagDetails))
                return true;
            if (TweetFilter.FilterByRetweetsRange(tagDetails))
                return true;
            if (TweetFilter.FilterOldTweets(tagDetails))
                return true;
            if (TweetFilter.FilterTweetByRestrictedWords(tagDetails))
                return true;
            if (TweetFilter.FilterTweetContainingAtSign(tagDetails))
                return true;
            if (TweetFilter.FilterTweetsWithNonEnglishCharacter(tagDetails))
                return true;
            if (TweetFilter.FilterWithLinks(tagDetails))
                return true;
            if (TweetFilter.FilterWithoutLinks(tagDetails))
                return true;
            if (TweetFilter.FilterIsRetweetedTweet(tagDetails))
                return true;
            //if (_jobProcess.ModuleSetting.TweetFilterModel.IsFilterSkipRetweets && tagDetails.IsRetweet)
            //    return true;

            return false;
        }

        #endregion

        #region User Filter

        protected bool UserFilterApply(TwitterUser TwitterUser)
        {
            try
            {
                if ((ActivityType == ActivityType.Follow || ActivityType == ActivityType.FollowBack) &&
                    UserFilter.IsAlreadyFollowing(TwitterUser))
                    return true;
                if (ActivityType == ActivityType.Mute && TwitterUser.IsMuted)
                    return true;
                if (UserFilter.FilterIfPrivateUser(TwitterUser))
                    return true;
                if (UserFilter.FilterByProfileImage(TwitterUser))
                    return true;
                if (UserFilter.FilterIfUserIsNotFollowing(TwitterUser))
                    return true;
                if (UserFilter.FilterByBioCharacterLength(TwitterUser))
                    return true;
                if (UserFilter.FilterIfBioContainInvalidWords(TwitterUser))
                    return true;
                if (UserFilter.FilterIfBioNotContainSpecificWords(TwitterUser))
                    return true;

                if (UserFilter.FilterIfUserHaveNonEnglishCharacterInBio(TwitterUser))
                    return true;
                if (UserFilter.FilterByTweetsCount(TwitterUser))
                    return true;
                if (UserFilter.FilterByFollowersCount(TwitterUser))
                    return true;
                if (UserFilter.FilterByFollowingsCount(TwitterUser))
                    return true;
                if (UserFilter.FilterByFollowRatioGreaterThan(TwitterUser))
                    return true;
                if (UserFilter.FilterByFollowRatioSmallerThan(TwitterUser))
                    return true;
                if (UserFilter.FilterByLastTweetAge(TwitterUser))
                    return true;
                if (UserFilter.FilterIfUserMuted(TwitterUser))
                    return true;
                if (UserFilter.FilterBasedOnVerification(TwitterUser))
                    return true;
                if (UserFilter.FilterByLastTweetAge(TwitterUser))
                    return true;

                // checking is following same follower
                if (ActivityType == ActivityType.Follow &&
                    TwitterUser.UserId == _jobProcess.DominatorAccountModel.AccountBaseModel.UserId)
                    return true;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return false;
        }

        #endregion

        #endregion

        #region Helper Functions

        protected void UniqueListTwtUser(ref List<TagDetails> LstUserTag)
        {
            // DominatorHouseCore.Process.JobProcessResult jobProcessResult = new DominatorHouseCore.Process.JobProcessResult();

            if (ActivityType == ActivityType.Follow || ActivityType == ActivityType.UserScraper ||
                ActivityType == ActivityType.TweetTo)
            {
                // Distincts user by username
                LstUserTag = LstUserTag.GroupBy(x => x.Username).Select(y => y.FirstOrDefault()).ToList();

                LstUserTag.RemoveAll(x => TotalListOfUserNameScraped.Any(y => y.Equals(x.UserId)));
                LstUserTag.ForEach(x => TotalListOfUserNameScraped.Add(x.UserId));
                if (ActivityType == ActivityType.Follow)
                    LstUserTag.RemoveAll(x => x.FollowStatus);
            }
            // This is for like module if user already liked post then by default it will remove
            // the post either filter is checked or not.
            else if (ActivityType == ActivityType.Like)
            {
                LstUserTag.RemoveAll(x => x.IsAlreadyLiked);
            }

            // This is for like module if user already retweeted post then by default it will remove
            // the post either filter is checked or not.
            else if (ActivityType == ActivityType.Retweet)
            {
                LstUserTag.RemoveAll(x => x.IsAlreadyRetweeted);
            }

            else if (ActivityType == ActivityType.Mute)
            {
                // Distincts user by username
                LstUserTag = LstUserTag.GroupBy(x => x.Username).Select(y => y.FirstOrDefault()).ToList();
                LstUserTag.RemoveAll(x => x.IsMuted);

                var listInteractedUsers = _dbAccountService.GetInteractedUsers(ActivityType)
                    .Select(y => y.InteractedUsername);
                LstUserTag.RemoveAll(x => listInteractedUsers.Contains(x.Username));
            }
            else if (ActivityType == ActivityType.Reposter)
            {
                var listInteractedTweet = _dbAccountService.GetInteractedPosts(ActivityType)
                    .Select(y => y.TweetId).ToList();

                LstUserTag.RemoveAll(x => listInteractedTweet.Contains(x.Id));
            }
        }

        protected List<TwitterUser> GetFriendshipsFromDb(FollowType followType, bool excludeMutual = false)
        {
            if (excludeMutual)
                return _dbAccountService.GetFriendships(followType).Select(y => new TwitterUser
                {
                    UserId = y.UserId,
                    Username = y.Username,
                    HasProfilePic = y.HasAnonymousProfilePicture != 1,
                    IsVerified = y.IsVerified == 1,
                    IsPrivate = y.IsPrivate == 1,
                    FollowBackStatus = true
                }).ToList();


            return _dbAccountService.GetFriendships(followType, FollowType.Mutual).Select(y => new TwitterUser
            {
                UserId = y.UserId,
                Username = y.Username,
                FullName = y.FullName,
                HasProfilePic = y.HasAnonymousProfilePicture != 1,
                IsVerified = y.IsVerified == 1,
                IsPrivate = y.IsPrivate == 1,
                FollowBackStatus = true
            }).ToList();
        }

        protected bool IsActivityDoneWithThisUserIdCampaignWise(string userId)
        {
            lock (LockScrapedUser)
            {
                if (_campaignService?.CampaignId != null)
                    return _campaignService.DoesInteractedUserExist(userId, ActivityType);
                return false;
            }
        }

        protected bool IsUniqueTweet(TagDetails tweet)
        {
            lock (InteractedUserObject)
            {
                if (_campaignService?.CampaignId == null)
                    return true;
                if (UniqueDict == null) UniqueDict = new Dictionary<string, HashSet<string>>();
                if (!UniqueDict.ContainsKey(_campaignService.CampaignId))
                    UniqueDict.Add(_campaignService.CampaignId,
                        _campaignService.GetAllInteractedPosts().Select(a => a.TweetId).ToHashSet());
                return UniqueDict[_campaignService.CampaignId].Add(tweet.Id);
            }
        }
        protected bool IsUniqueUser(TwitterUser user)
        {
            lock (InteractedUserObject)
            {
                if (_campaignService?.CampaignId == null)
                    return true;
                if (UniqueDict == null) UniqueDict = new Dictionary<string, HashSet<string>>();
                if (!UniqueDict.ContainsKey(_campaignService.CampaignId))
                    UniqueDict.Add(_campaignService.CampaignId,
                        ActivityType == ActivityType.Follow
                            ? _campaignService.GetAllUnfollowedUsers().Select(a => a.UserId).ToHashSet()
                            : _campaignService.GetAllInteractedUsers().Select(a => a.InteractedUserId).ToHashSet());

                return UniqueDict[_campaignService.CampaignId].Add(user.UserId);
            }
        }
            protected List<T> SkipBlackListOrWhiteList<T>(List<T> lstTagDetails)
        {
            if (_jobProcess.ModuleSetting.ManageBlackWhiteListModel.IsSkipWhiteListUsers &&
                (_jobProcess.ModuleSetting.ManageBlackWhiteListModel.IsUseGroupWhiteList ||
                 _jobProcess.ModuleSetting.ManageBlackWhiteListModel.IsUsePrivateWhiteList))
                lstTagDetails = BlackWhiteListHandler.SkipWhiteListUsers(lstTagDetails);

            if (_jobProcess.ModuleSetting.SkipBlacklist.IsSkipBlackListUsers &&
                (_jobProcess.ModuleSetting.SkipBlacklist.IsSkipGroupBlackListUsers ||
                 _jobProcess.ModuleSetting.SkipBlacklist.IsSkipPrivateBlackListUser))
                lstTagDetails = BlackWhiteListHandler.SkipBlackListUsers(lstTagDetails);
            return lstTagDetails;
        }

        #endregion
    }
}