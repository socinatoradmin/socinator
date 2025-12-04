using System;
using System.Collections.Generic;
using ThreadUtils;
using DominatorHouseCore.DatabaseHandler.TdTables.Accounts;
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
    internal class SomeonesFollowers : BaseTwitterUserProcessor, IQueryProcessor
    {
        private readonly ITDAccountUpdateFactory _accountUpdateFactory;
        private readonly IDelayService _delayService;

        public SomeonesFollowers(ITdJobProcess jobProcess, IBlackWhiteListHandler blackWhiteListHandler,
            IDbCampaignService campaignService,
            ITwitterFunctionFactory twitterFunctionFactory, IDelayService threadUtility,
            ITDAccountUpdateFactory accountUpdateFactory, IDbInsertionHelper dbInsertionHelper)
            : base(jobProcess, blackWhiteListHandler, campaignService, twitterFunctionFactory.TwitterFunctions,
                dbInsertionHelper)
        {
            _accountUpdateFactory = accountUpdateFactory;
            _delayService = threadUtility;
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            _jobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

            if (_jobProcess.checkJobCompleted()) return;

            CheckUserFilterActiveForCurrentQuery(queryInfo);
            jobProcessResult = new JobProcessResult();

            if (string.Equals(queryInfo.QueryValue, "[Own]", StringComparison.CurrentCultureIgnoreCase))
            {
                var followersList = StartProcessForOwnFollowers();
                StartFinalProcess(queryInfo, jobProcessResult, followersList);
            }
            else
            {
                var userInfo = TwitterFunction.GetUserDetails(_jobProcess.DominatorAccountModel, queryInfo.QueryValue,
                    queryInfo.QueryType);
                var isNotFirst = false;

                if (_jobProcess.ModuleSetting.IsSavePagination) jobProcessResult.maxId = GetPaginationId(queryInfo);

                if (userInfo.Success && userInfo.UserDetail.FollowersCount > 0 &&
                    (!userInfo.UserDetail.IsPrivate || queryInfo.QueryValue.Trim()
                         .Equals(_jobProcess.DominatorAccountModel.AccountBaseModel.UserName,
                             StringComparison.InvariantCultureIgnoreCase)))
                    while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
                    {
                        // We are not saving pagination first time we hit the url we will 
                        // save next time when we paginate it because pagination id gives us next page users
                        // here we save last page id

                        if (_jobProcess.ModuleSetting.IsSavePagination)
                            AddOrUpdatePaginationId(queryInfo, jobProcessResult.maxId, ref isNotFirst);

                        var followerResponseHandler = TwitterFunction.GetUserFollowersAsync(
                            _jobProcess.DominatorAccountModel,
                            queryInfo.QueryValue,
                            _jobProcess.JobCancellationTokenSource.Token, jobProcessResult.maxId).Result;

                        _jobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                        if (followerResponseHandler.Success)
                        {
                            var lstTwitterUser = followerResponseHandler.ListOfTwitterUser;

                            #region Skip BlackList or WhiteList

                            lstTwitterUser = SkipBlackListOrWhiteList(lstTwitterUser);

                            #endregion

                            jobProcessResult = StartFinalProcess(queryInfo, jobProcessResult, lstTwitterUser);
                            jobProcessResult.maxId = followerResponseHandler.MinPosition;

                            if (!followerResponseHandler.HasMoreResults) jobProcessResult.HasNoResult = true;
                        }
                        else
                        {
                            jobProcessResult.maxId = null;
                            jobProcessResult.HasNoResult = true;
                        }

                        _delayService.ThreadSleep(TdConstants.ConsecutiveGetReqInteval);
                    }

                if (!jobProcessResult.IsProcessCompleted)
                {
                    //schedule job next auto activity
                    ++TwitterFunction.UsedQueryCount;
                    TwitterFunction.IsScheduleNext = true;

                    GlobusLogHelper.log.Info(Log.CustomMessage,
                        _jobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        _jobProcess.DominatorAccountModel.UserName,
                        ActivityType,
                        $"No more data available to perform {ActivityType} for query value {queryInfo.QueryValue}");
                }
            }
        }

        protected List<TwitterUser> StartProcessForOwnFollowers()
        {
            var listTwitterUser = new List<TwitterUser>();

            if (_jobProcess.checkJobCompleted()) return listTwitterUser;

            _accountUpdateFactory.CheckIsFollowersUpdated(_jobProcess.DominatorAccountModel,
                AccountModel.LastFollowersUpdatedTime, _jobProcess.JobCancellationTokenSource.Token);

            listTwitterUser = GetFriendshipsFromDb(FollowType.Followers);

            listTwitterUser.Shuffle();

            _jobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

            return listTwitterUser;
        }
    }
}