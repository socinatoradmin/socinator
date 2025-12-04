using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using CommonServiceLocator;
using ThreadUtils;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.GdTables.Campaigns;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.TdQuery;
using DominatorHouseCore.Extensions;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using DominatorUIUtility.Views.AccountSetting.Activity;
using TwtDominatorCore.Database;
using TwtDominatorCore.TDFactories;
using TwtDominatorCore.TDLibrary.GeneralLibrary.DAL;
using TwtDominatorCore.TDModels;
using TwtDominatorCore.TDUtility;

namespace TwtDominatorCore.TDLibrary.GeneralLibrary.Processors.Tweet
{
    internal abstract class BaseTwitterTweetsProcessor : BaseTwitterProcessor
    {
        private static readonly ConcurrentDictionary<string, object> LockObjects =
            new ConcurrentDictionary<string, object>();

        protected readonly string _campaignId;
        protected IDbAccountService dbAccountService;
        private readonly IDelayService _delayService;

        private readonly Dictionary<ActivityType, Func<QueryInfo, JobProcessResult, TagDetails, JobProcessResult>>
            ListStartTweetFinalProcess =
                new Dictionary<ActivityType, Func<QueryInfo, JobProcessResult, TagDetails, JobProcessResult>>();

        protected BaseTwitterTweetsProcessor(ITdJobProcess jobProcess, IBlackWhiteListHandler blackWhiteListHandler,
            IDbCampaignService campaignService, ITwitterFunctions twitterFunctions, IDelayService DelayService,
            IDbInsertionHelper dbInsertionHelper) :
            base(jobProcess, blackWhiteListHandler, campaignService, twitterFunctions, dbInsertionHelper)
        {
            _campaignId = campaignService.CampaignId;
            _delayService = DelayService;
            dbAccountService = jobProcess?.DbAccountService;
            ListStartTweetFinalProcess.Add(ActivityType.TweetScraper, StartTweetFinalProcessTweetScraper);
        }

        protected void CheckIsFeedsUpdated(bool IsTweetWithReply=true)
        {
            var CurrentEpoch = DateTime.UtcNow.GetCurrentEpochTime();
            var tdAccountUpdateFactory = InstanceProvider.GetInstance<ITDAccountUpdateFactory>();
            if (CurrentEpoch - AccountModel.LastFeedsUpdatedTime > TdConstants.UpdateDbInterval)
                tdAccountUpdateFactory.UpdatePosts(_jobProcess.DominatorAccountModel, TwitterFunction, dbAccountService, IsTweetWithReply).Wait();
        }

        public JobProcessResult StartFinalProcess(QueryInfo QueryInfo, JobProcessResult jobProcessResult,
            List<TagDetails> LstUserTag)
        {
            UniqueListTwtUser(ref LstUserTag);
            var Skipped = LstUserTag.RemoveAll(x => _dbAccountService.IsActivtyDoneWithThisTweetId(x.Id, ActivityType));
            if(Skipped > 0)
                GlobusLogHelper.log.Info(Log.CustomMessage,
                        _jobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        _jobProcess.DominatorAccountModel.UserName, ActivityType,
                        $"Successfully Skipped {Skipped} Already Interacted Tweets.");
            foreach (var EachUserTag in LstUserTag)
            {
                _jobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                QueryInfo.QueryValue = string.IsNullOrEmpty(QueryInfo.QueryValue) ?TdUtility.GetTweetUrl(EachUserTag.Username,EachUserTag.Id):QueryInfo.QueryValue;
                if (ActivityType.Equals(ActivityType.TweetScraper))
                    StartTweetFinalProcessTweetScraper(QueryInfo, jobProcessResult, EachUserTag);
                else
                    StartTweetFinalProcessTweetActions(QueryInfo, EachUserTag, jobProcessResult);
            }

            _delayService.ThreadSleep(TdConstants.ConsecutiveGetReqInteval);
            return jobProcessResult;
        }

        public JobProcessResult StartFinalProcess(QueryInfo QueryInfo, JobProcessResult jobProcessResult,
            List<TwitterUser> LstTwitterUser)
        {
            foreach (var EachUser in LstTwitterUser)
            {
                _jobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                try
                {
                    jobProcessResult = StartFinalProcessForTags(QueryInfo, jobProcessResult, EachUser.Username);
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

        private JobProcessResult StartTweetFinalProcessTweetScraper(QueryInfo queryInfo,
            JobProcessResult jobProcessResult, TagDetails EachUserTag)
        {
            try
            {
                var isCampaign = _campaignService?.CampaignId != null;
                _jobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                if (isCampaign)
                    _delayService.ThreadSleep(new Random().Next(1000, 5000));

                var TempTag = new TagDetails
                {
                    Username = EachUserTag.Username,
                    UserId = EachUserTag.UserId,
                    Id = EachUserTag.Id,
                    FollowStatus = EachUserTag.FollowStatus,
                    FollowBackStatus = EachUserTag.FollowBackStatus,
                    IsPrivate = EachUserTag.IsPrivate,
                    HasProfilePic = EachUserTag.HasProfilePic,
                    IsVerified = EachUserTag.IsVerified,
                    IsMuted = EachUserTag.IsMuted
                };
                if (isCampaign && IsActivityDoneWithThisUserIdCampaignWise(EachUserTag.Id) ||
                    !IsUniqueTweet(TempTag))
                    return jobProcessResult;


                FinalProcessForEachTag(queryInfo, out jobProcessResult, EachUserTag);
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
        ///     Here we do like, follow, comment etc tweet actions
        /// </summary>
        /// <returns></returns>
        private void StartTweetFinalProcessTweetActions(QueryInfo QueryInfo, TagDetails EachUserTag,
            JobProcessResult jobProcessResult)
        {
            try
            {
                if (QueryInfo.QueryType.Equals(
                        EnumUtility.GetQueryFromEnum(TdTweetInteractionQueryEnum.SpecificUserTweets))
                    && CheckActionTweetPerUser(EachUserTag.Username,
                        _jobProcess.ModuleSetting.NoOfActionTweetPerUser.StartValue))
                    return;
                var IsFiltered = false;
                if (IsUserFilterActive)
                {
                    var UserDetailsHandler = TwitterFunction.GetUserDetails(_jobProcess.DominatorAccountModel,
                        EachUserTag.Username, QueryInfo.QueryType);
                    if (UserDetailsHandler.Success)
                        IsFiltered = UserFilterApply(UserDetailsHandler.UserDetail);
                }


                if (!CheckPostUniqueNess(jobProcessResult, EachUserTag))
                    return;

                if (!ApplyCampaignLevelSettings(QueryInfo, EachUserTag.Id))
                    return;

                if (!IsFiltered)
                    FinalProcessForEachTag(QueryInfo, out jobProcessResult, EachUserTag);
                else
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                        _jobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        _jobProcess.DominatorAccountModel.UserName, ActivityType,
                        string.Format("LangKeyFilteredTweetWithId".FromResourceDictionary(), EachUserTag.Id));
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public JobProcessResult StartFinalProcessForTags(QueryInfo QueryInfo, JobProcessResult jobProcessResult,
            string user)
        {
            GlobusLogHelper.log.Info(Log.CustomMessage,
                _jobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                _jobProcess.DominatorAccountModel.UserName, ActivityType,
                string.Format("LangKeyBioAndOtherInfoForUser".FromResourceDictionary(), user));

            var userDetailsHandler =
                TwitterFunction.GetUserDetails(_jobProcess.DominatorAccountModel, user, QueryInfo.QueryType, true);

            if (!UserFilterApply(userDetailsHandler.UserDetail))
            {
                // No of tweet to scrap per user
                InitializeTweetScrapePerUser();
                TweetFilterApply(userDetailsHandler.UserDetail.ListTag);
                var SkippedTweet = 0;
                if((SkippedTweet=userDetailsHandler.UserDetail.ListTag.RemoveAll(tweet=>_dbAccountService.IsActivtyDoneWithThisTweetId(tweet.Id,ActivityType)))> 0)
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, _jobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    _jobProcess.DominatorAccountModel.UserName, ActivityType,$"Skipped {SkippedTweet} Already Interacted Tweets.");
                    NoOfTweetsToBeScrapePerUser -= SkippedTweet;
                }
                foreach (var EachTag in userDetailsHandler.UserDetail.ListTag)
                {
                    try
                    {
                        if (ActivityType == ActivityType.TweetScraper &&
                            (_dbAccountService.IsActivtyDoneWithThisTweetId(EachTag.Id, ActivityType) ||
                             !IsUniqueTweet(EachTag)))
                            continue;
                    }
                    catch (OperationCanceledException)
                    {
                        throw new OperationCanceledException();
                    }
                    catch (Exception ex)
                    {
                        ex.ErrorLog();
                        //todo: handle excpetion
                    }


                    if (_jobProcess.ModuleSetting.IsChkActionTweetPerUser &&
                        (_jobProcess.ActivityType == ActivityType.Reposter || _jobProcess.ActivityType ==
                                                                           ActivityType.Retweet
                                                                           || _jobProcess.ActivityType ==
                                                                           ActivityType.Like ||
                                                                           _jobProcess.ActivityType ==
                                                                           ActivityType.Comment))
                        if (_jobProcess.ModuleSetting.IsChkActionTweetPerUser && IsReachedMaxTweetActionPerUser(user,
                                _jobProcess.ModuleSetting.NoOfActionTweetPerUser.StartValue))
                            return jobProcessResult;

                    _jobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    if (NoOfTweetsToBeScrapePerUser == 0)
                        break;
                    FinalProcessForEachTag(QueryInfo, out jobProcessResult, EachTag);

                    // Limit max actions per user on tweet 
                    if (jobProcessResult.IsProcessSuceessfull)
                        NoOfTweetsToBeScrapePerUser--;

                    if (jobProcessResult.IsProcessCompleted || NoOfTweetsToBeScrapePerUser == 0)
                        break;
                }
            }
            else
            {
                GlobusLogHelper.log.Info(Log.CustomMessage,
                    _jobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    _jobProcess.DominatorAccountModel.UserName, ActivityType,
                    string.Format("LangKeyFilteredUserWithUserName".FromResourceDictionary(), user));
            }

            return jobProcessResult;
        }

        protected bool CheckPostUniqueNess(JobProcessResult jobProcessResult, TagDetails post)
        {
            var jobActivityConfigurationManager =
                InstanceProvider.GetInstance<IJobActivityConfigurationManager>();
            var moduleConfiguration =
                jobActivityConfigurationManager[_jobProcess.DominatorAccountModel.AccountId, ActivityType];
            if (moduleConfiguration != null && moduleConfiguration.IsTemplateMadeByCampaignMode)
            {
                var instance = InstanceProvider.GetInstance<ICampaignInteractionDetails>();

                if (_jobProcess.ModuleSetting.IschkUniquePostForCampaign)
                    try
                    {
                        instance.AddInteractedData(SocialNetworks.Twitter, $"{_campaignService.CampaignId}.post",
                            post.Id);
                    }
                    catch (Exception)
                    {
                        jobProcessResult.IsProcessSuceessfull = false;
                        return false;
                    }

                if (_jobProcess.ModuleSetting.IschkUniqueUserForCampaign)
                    try
                    {
                        instance.AddInteractedData(SocialNetworks.Twitter, _campaignService.CampaignId, post.Username);
                    }
                    catch (Exception)
                    {
                        jobProcessResult.IsProcessSuceessfull = false;
                        return false;
                    }
            }

            if (_jobProcess.ModuleSetting.IschkUniqueUserForAccount)
                try
                {
                    if (ActivityType == ActivityType.Reposter || ActivityType == ActivityType.Retweet ||
                        ActivityType == ActivityType.Like || ActivityType == ActivityType.Comment)
                        if (_dbAccountService.GetInteractedPosts(ActivityType).Where(x => x.Username == post.Username)
                            .Any())

                            return false;
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

            return true;
        }


        protected bool ApplyCampaignLevelSettings(QueryInfo queryInfo, string postPermalink)
        {
            if (_campaignId == null) return true;
            var campaignFileManager = InstanceProvider.GetInstance<ICampaignsFileManager>();
            var campaignDetails = campaignFileManager.GetCampaignById(_campaignService.CampaignId);

            if (campaignDetails != null)
                try
                {
                    _jobProcess.AddedToDb = false;

                    #region Action From Random Percentage Of Accounts

                    if (_jobProcess.ModuleSetting.IsPerformActionFromRandomPercentageOfAccounts)
                    {
                        var lockObject = LockObjects.GetOrAdd("Lock1" + postPermalink, new object());
                        lock (lockObject)
                        {
                            _jobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                            var dbOperation = new DbOperations(campaignDetails.CampaignId, SocialNetworks.Twitter,
                                ConstantVariable.GetCampaignDb);
                            try
                            {
                                decimal count = campaignDetails.SelectedAccountList.Count;
                                var randomMaxAccountToPerform = (int) Math.Round(
                                    count * _jobProcess.ModuleSetting.PerformActionFromRandomPercentage.GetRandom() /
                                    100);

                                var numberOfAccountsAlreadyPerformedAction = _campaignService.GetAllInteractedPosts()
                                    .Where(x => x.ActivityType == ActivityType.ToString() && x.TweetId == postPermalink)
                                    .ToList();

                                if (randomMaxAccountToPerform <= numberOfAccountsAlreadyPerformedAction.Count)
                                    return false;

                                AddPendingActivityValueToDb(queryInfo, postPermalink, dbOperation);
                                _jobProcess.AddedToDb = true;
                            }
                            catch (OperationCanceledException)
                            {
                                throw new OperationCanceledException(@"Cancellation Requested !");
                            }
                            catch (AggregateException ae)
                            {
                                ae.HandleOperationCancellation();
                            }
                            catch (Exception ex)
                            {
                                ex.DebugLog();
                            }
                        }
                    }

                    #endregion

                    #region Delay Between action On SamePost

                    if (_jobProcess.ModuleSetting.EnableDelayBetweenPerformingActionOnSamePost)
                    {
                        var lockObject = LockObjects.GetOrAdd("Lock2" + postPermalink, new object());
                        lock (lockObject)
                        {
                            _jobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                            var dbOperation = new DbOperations(campaignDetails.CampaignId, SocialNetworks.Twitter,
                                ConstantVariable.GetCampaignDb);
                            try
                            {
                                List<int> recentlyPerformedActions;
                                recentlyPerformedActions = _campaignService.GetAllInteractedPosts()
                                    .Where(x => x.ActivityType == ActivityType.ToString() &&
                                                x.TweetId == postPermalink &&
                                                (x.Status == "Success" || x.Status == "Working"))
                                    .OrderByDescending(x => x.TweetedTimeStamp).Select(x => x.TweetedTimeStamp)
                                    .Take(1).ToList();

                                if (recentlyPerformedActions.Count > 0)
                                {
                                    var recentlyPerformedTime = recentlyPerformedActions[0];
                                    var delay = _jobProcess.ModuleSetting.DelayBetweenPerformingActionOnSamePost
                                        .GetRandom();
                                    var time = DateTimeUtilities.GetEpochTime();
                                    var time2 = recentlyPerformedTime + delay;
                                    if (time < time2)
                                        _delayService.ThreadSleep(
                                            (time2 - time) * 1000); 
                                }

                                if (!_jobProcess.AddedToDb)
                                {
                                    AddWorkingActivityValueToDb(queryInfo, postPermalink, dbOperation);
                                }
                                else
                                {
                                    var interactedPost = dbOperation.GetSingle<InteractedPosts>(
                                        x => x.Permalink == postPermalink && x.ActivityType == ActivityType &&
                                             x.Username == _jobProcess.DominatorAccountModel.AccountBaseModel
                                                 .UserName && (x.Status == "Pending" || x.Status == "Working"));
                                    interactedPost.InteractionDate = DateTimeUtilities.GetEpochTime();
                                    interactedPost.Status = "Working";
                                    dbOperation.Update(interactedPost);
                                }
                            }
                            catch (OperationCanceledException)
                            {
                                throw new OperationCanceledException(@"Cancellation Requested !");
                            }
                            catch (AggregateException ae)
                            {
                                ae.HandleOperationCancellation();
                            }
                            catch (Exception ex)
                            {
                                ex.DebugLog();
                            }
                        }
                    }

                    #endregion
                }
                catch (OperationCanceledException)
                {
                    throw new OperationCanceledException(@"Cancellation Requested !");
                }
                catch (AggregateException ae)
                {
                    ae.HandleOperationCancellation();
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

            return true;
        }


        protected void AddPendingActivityValueToDb(QueryInfo queryInfo, string postPermalink, DbOperations dbOperation)
        {
            dbOperation.Add(new InteractedPosts
            {
                ActivityType = ActivityType,
                QueryType = queryInfo.QueryType,
                QueryValue = queryInfo.QueryValue,
                Username = _jobProcess.DominatorAccountModel.AccountBaseModel.UserName,
                InteractionDate = DateTimeUtilities.GetEpochTime(),
                Permalink = postPermalink,
                Status = "Pending"
            });
        }

        protected void AddWorkingActivityValueToDb(QueryInfo queryInfo, string postPermalink, DbOperations dbOperation)
        {
            dbOperation.Add(new InteractedPosts
            {
                ActivityType = ActivityType,
                QueryType = queryInfo.QueryType,
                QueryValue = queryInfo.QueryValue,
                Username = _jobProcess.DominatorAccountModel.AccountBaseModel.UserName,
                InteractionDate = DateTimeUtilities.GetEpochTime(),
                Permalink = postPermalink,
                Status = "Working"
            });
        }
    }
}