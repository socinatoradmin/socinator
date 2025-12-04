using System;
using System.Collections.Generic;
using System.Linq;
using ThreadUtils;
using DominatorHouseCore;
using DominatorHouseCore.Enums.TdQuery;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using TwtDominatorCore.Database;
using TwtDominatorCore.TDFactories;
using TwtDominatorCore.TDLibrary.GeneralLibrary.DAL;
using TwtDominatorCore.TDModels;

namespace TwtDominatorCore.TDLibrary.GeneralLibrary.Processors.Tweet
{
    internal class KeywordHashLocationTweetProcessor : BaseTwitterTweetsProcessor, IQueryProcessor
    {
        private readonly IDbInsertionHelper DbInsertionHelper;
        public KeywordHashLocationTweetProcessor(ITdJobProcess jobProcess, IBlackWhiteListHandler blackWhiteListHandler,
            IDbCampaignService campaignService, ITwitterFunctionFactory twitterFunctionFactory,
            IDelayService threadUtility, IDbInsertionHelper dbInsertionHelper)
            : base(jobProcess, blackWhiteListHandler, campaignService, twitterFunctionFactory.TwitterFunctions,
                threadUtility, dbInsertionHelper)
        {
            DbInsertionHelper = dbInsertionHelper;
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            CheckUserFilterActiveForCurrentQuery(queryInfo);
            jobProcessResult = new JobProcessResult();

            var isNotFirst = false;
            TwitterFunction.IsScheduleNext = false;

            jobProcessResult.maxId = GetPaginationId(queryInfo);

            while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
            {
                // save pagination only for campaign having not checked 'IsFilterTweetsLessThanSpecificDaysOld'
                // otherwise it will not fetch tweets first page, fetch tweet from last saved pagination id. 

                if (!_jobProcess.ModuleSetting.TweetFilterModel.IsFilterTweetsLessThanSpecificDaysOld || !_jobProcess.ModuleSetting.TweetFilterModel.IsFilterTweetsLessThanSpecificHoursOld)
                    AddOrUpdatePaginationId(queryInfo, jobProcessResult.maxId, ref isNotFirst);

                var searchTagHandler = TwitterFunction.SearchForTag(_jobProcess.DominatorAccountModel,
                    queryInfo.QueryValue, queryInfo.QueryType,
                    _jobProcess.JobCancellationTokenSource.Token, jobProcessResult.maxId);
                var InteractedUsers = DbInsertionHelper.GetInteractedUser(_jobProcess.ActivityType);
                var SkippedCount = searchTagHandler.ListTagDetails.RemoveAll(x => InteractedUsers.Any(y => y.InteractedUsername == x.Username));
                if (SkippedCount > 0)
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                            _jobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            _jobProcess.DominatorAccountModel.UserName, _jobProcess.ActivityType,
                            $"Successfully Skipped {SkippedCount} Already Interacted Users.");
                _jobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                if (searchTagHandler != null && searchTagHandler.ListTagDetails.Count() > 0 && !searchTagHandler.NoResultStatus)
                {
                    var isLastDayPostReached = false;
                    var lstTagDetails = new List<TagDetails>();

                    lstTagDetails.AddRange(searchTagHandler.ListTagDetails);

                    TweetFilterApply(lstTagDetails);
                    int failedCount = 0;
                    while(lstTagDetails.Count < 20 && !string.IsNullOrEmpty(searchTagHandler.MinPosition ) && failedCount++ < 3)
                    {
                        int count = lstTagDetails.Count;
                        searchTagHandler = TwitterFunction.SearchForTag(_jobProcess.DominatorAccountModel,
                                                    queryInfo.QueryValue, queryInfo.QueryType,
                                                    _jobProcess.JobCancellationTokenSource.Token, searchTagHandler.MinPosition);
                        
                        if (searchTagHandler.ListTagDetails.Count == count)
                            failedCount++;
                        else
                        {
                            lstTagDetails.AddRange(searchTagHandler.ListTagDetails);
                            TweetFilterApply(lstTagDetails);
                        }
                    }
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                    _jobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    _jobProcess.DominatorAccountModel.UserName, _jobProcess.ActivityType,
                    string.Format("Found {0} Results for Query Value {1}", lstTagDetails.Count,
                        queryInfo.QueryValue));

                    try
                    {
                        // check is tweet really have old tweet
                        if (_jobProcess.ModuleSetting.TweetFilterModel.IsFilterTweetsLessThanSpecificHoursOld)
                            isLastDayPostReached = searchTagHandler.ListTagDetails.LastOrDefault().HoursCount >
                                                   _jobProcess.ModuleSetting.TweetFilterModel
                                                       .FilterTweetsLessThanSpecificHoursOldValue;

                        if (_jobProcess.ModuleSetting.TweetFilterModel.IsFilterTweetsLessThanSpecificDaysOld)
                            isLastDayPostReached = searchTagHandler.ListTagDetails.LastOrDefault().DaysCount >
                                                   _jobProcess.ModuleSetting.TweetFilterModel
                                                       .FilterTweetsLessThanSpecificDaysOldValue;
                    }
                    catch (OperationCanceledException)
                    {
                        throw new OperationCanceledException();
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                    #region Skip BlackList or WhiteList

                    lstTagDetails = SkipBlackListOrWhiteList(lstTagDetails);

                    #endregion
                    if(queryInfo.QueryType== "Hashtags")
                    lstTagDetails.RemoveAll(x => !x.Caption.ToLower().Contains(queryInfo.QueryValue.ToLower()));

                    jobProcessResult = StartFinalProcess(queryInfo, jobProcessResult, lstTagDetails);
                    jobProcessResult.maxId = searchTagHandler.MinPosition;

                    if (!searchTagHandler.HasMore || lstTagDetails.Count == 0) jobProcessResult.HasNoResult = true;

                    #region return when  user max tweet reached

                    // only specific users have same user max tweet 
                    if (CheckActionTweetPerUser(queryInfo.QueryValue,
                            _jobProcess.ModuleSetting.NoOfActionTweetPerUser.StartValue)
                        && queryInfo.QueryType.Equals(
                            EnumUtility.GetQueryFromEnum(TdTweetInteractionQueryEnum.SpecificUserTweets)))
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage,
                            _jobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            _jobProcess.DominatorAccountModel.UserName, _jobProcess.ActivityType,
                            "LangKeyReachedMaximumTweetScrape".FromResourceDictionary());
                        return;
                    }

                    #endregion

                    #region return when  user have no more updated tweet 

                    // only for specific users latest tweets 'IsFilterTweetsLessThanSpecificDaysOld' and 'IsFilterTweetsLessThanSpecificHoursOld'
                    if (lstTagDetails.Count <= 0 && isLastDayPostReached)
                    {
                        if(_jobProcess.ModuleSetting.TweetFilterModel.IsFilterTweetsLessThanSpecificHoursOld)
                        GlobusLogHelper.log.Info(Log.CustomMessage,
                            _jobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            _jobProcess.DominatorAccountModel.UserName, _jobProcess.ActivityType,
                            string.Format("LangKeyNoMoreUpdatedTweetsAvailableBeforeHours".FromResourceDictionary(),
                                _jobProcess.ModuleSetting.TweetFilterModel.FilterTweetsLessThanSpecificHoursOldValue));
                        else if (_jobProcess.ModuleSetting.TweetFilterModel.IsFilterTweetsLessThanSpecificDaysOld)
                            GlobusLogHelper.log.Info(Log.CustomMessage,
                                _jobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                _jobProcess.DominatorAccountModel.UserName, _jobProcess.ActivityType,
                                string.Format("LangKeyNoMoreUpdatedTweetsAvailableBeforeDays".FromResourceDictionary(),
                                    _jobProcess.ModuleSetting.TweetFilterModel.FilterTweetsLessThanSpecificDaysOldValue));
                        return;
                    }

                    #endregion
                }

                else if(searchTagHandler.Issue.Message.Contains("this account is temporarily locked."))
                {
                    StopActivitiesForLockedAccount(queryInfo, jobProcessResult, searchTagHandler);
                }
                else
                {
                    //schedule job next auto activity
                    ++TwitterFunction.UsedQueryCount;
                    TwitterFunction.IsScheduleNext = true;

                    jobProcessResult.maxId = null;
                    jobProcessResult.HasNoResult = true;
                }
            }

            if (!jobProcessResult.IsProcessCompleted)
                GlobusLogHelper.log.Info(Log.CustomMessage,
                    _jobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    _jobProcess.DominatorAccountModel.UserName, _jobProcess.ActivityType,
                    string.Format("LangKeyNoMoreDataToActivityForQuery".FromResourceDictionary(), ActivityType,
                        queryInfo.QueryValue));
        }

        private void StopActivitiesForLockedAccount(QueryInfo queryInfo, JobProcessResult jobProcessResult, Response.SearchTagResponseHandler searchTagHandler)
        {
            TwitterFunction.IsScheduleNext = false;
            jobProcessResult.IsProcessCompleted = true;
            GlobusLogHelper.log.Info(Log.CustomMessage,
            _jobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
            _jobProcess.DominatorAccountModel.UserName, _jobProcess.ActivityType,
            searchTagHandler.Issue.Message, ActivityType,
                queryInfo.QueryValue);
            var dominatorScheduler = InstanceProvider.GetInstance<DominatorHouseCore.BusinessLogic.Scheduler.IDominatorScheduler>();
            dominatorScheduler.ChangeAccountsRunningStatus(false, _jobProcess.DominatorAccountModel.AccountBaseModel.AccountId, ActivityType);
        }
    }
}