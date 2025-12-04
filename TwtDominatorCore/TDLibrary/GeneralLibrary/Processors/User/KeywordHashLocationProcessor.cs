using System.Collections.Generic;
using System.Linq;
using CommonServiceLocator;
using ThreadUtils;
using DominatorHouseCore.Enums.TdQuery;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using TwtDominatorCore.Database;
using TwtDominatorCore.TDFactories;
using TwtDominatorCore.TDLibrary.GeneralLibrary.DAL;
using TwtDominatorCore.TDModels;
using TwtDominatorCore.TDUtility;

namespace TwtDominatorCore.TDLibrary.GeneralLibrary.Processors.User
{
    internal class KeywordHashLocationProcessor : BaseTwitterUserProcessor, IQueryProcessor
    {
        private readonly IDbInsertionHelper DbInsertionHelper;
        public KeywordHashLocationProcessor(ITdJobProcess jobProcess,
            IBlackWhiteListHandler blackWhiteListHandler, IDbCampaignService campaignService,
            ITwitterFunctionFactory twitterFunctionFactory, IDbInsertionHelper dbInsertionHelper)
            : base(jobProcess, blackWhiteListHandler, campaignService, twitterFunctionFactory.TwitterFunctions,
                dbInsertionHelper)
        {
            DbInsertionHelper = dbInsertionHelper;
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            if (_jobProcess.checkJobCompleted()) return;
            var delayService = InstanceProvider.GetInstance<IDelayService>();
            CheckUserFilterActiveForCurrentQuery(queryInfo);
            jobProcessResult = new JobProcessResult();

            var pagination = 0;

            while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
            {
                if (pagination > 0)
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                        _jobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        _jobProcess.DominatorAccountModel.UserName, _jobProcess.ActivityType,
                        $"Searching for {queryInfo.QueryType} {queryInfo.QueryValue} .");
                pagination++;
                var searchTagHandler = TwitterFunction.SearchForTag(_jobProcess.DominatorAccountModel,
                    queryInfo.QueryValue, queryInfo.QueryType,_jobProcess.JobCancellationTokenSource.Token,
                    jobProcessResult.maxId,_jobProcess.ActivityType == DominatorHouseCore.Enums.ActivityType.Follow ? "People":"Top");
                _jobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                var InteractedUsers = DbInsertionHelper.GetInteractedUser(_jobProcess.ActivityType);
                var SkippedCount = searchTagHandler.ListTagDetails.RemoveAll(x => InteractedUsers.Any(y => y.InteractedUsername == x.Username));
                if (SkippedCount > 0)
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                            _jobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            _jobProcess.DominatorAccountModel.UserName, _jobProcess.ActivityType,
                            $"Successfully Skipped {SkippedCount} Already Interacted Users.");
                if (searchTagHandler != null && searchTagHandler.Success && !searchTagHandler.NoResultStatus)
                {
                    var lstTagDetails = new List<TagDetails>();
                    lstTagDetails.AddRange(searchTagHandler.ListTagDetails);
                    TweetFilterApply(lstTagDetails);

                    #region Skip BlackList or WhiteList

                    lstTagDetails = SkipBlackListOrWhiteList(lstTagDetails);

                    #endregion

                    jobProcessResult = StartFinalProcess(queryInfo, jobProcessResult, lstTagDetails);
                    jobProcessResult.maxId = searchTagHandler.MinPosition;
                    if (!searchTagHandler.HasMore) jobProcessResult.HasNoResult = true;

                    if (CheckActionTweetPerUser(queryInfo.QueryValue,
                            _jobProcess.ModuleSetting.NoOfActionTweetPerUser.StartValue)
                        || queryInfo.QueryType.Equals(
                            EnumUtility.GetQueryFromEnum(TdTweetInteractionQueryEnum.SpecificUserTweets)))
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage,
                            _jobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            _jobProcess.DominatorAccountModel.UserName, _jobProcess.ActivityType,
                            "Reached maximum Tweet to scrap per user.");
                        return;
                    }
                }
                else
                {
                    jobProcessResult.maxId = null;
                    jobProcessResult.HasNoResult = true;
                }

                delayService.ThreadSleep(TdConstants.ConsecutiveGetReqInteval);
            }

            if (!jobProcessResult.IsProcessCompleted)
                GlobusLogHelper.log.Info(Log.CustomMessage,
                    _jobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    _jobProcess.DominatorAccountModel.UserName, _jobProcess.ActivityType,
                    $"No more data available to perform {ActivityType} for query value {queryInfo.QueryValue}");
        }
    }
}