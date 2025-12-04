using System;
using CommonServiceLocator;
using ThreadUtils;
using DominatorHouseCore;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using TwtDominatorCore.Database;
using TwtDominatorCore.TDFactories;
using TwtDominatorCore.TDLibrary.GeneralLibrary.DAL;
using TwtDominatorCore.TDUtility;

namespace TwtDominatorCore.TDLibrary.GeneralLibrary.Processors.User
{
    internal class FollowersOfFollowings : BaseTwitterUserProcessor, IQueryProcessor
    {
        public FollowersOfFollowings(ITdJobProcess jobProcess,
            IBlackWhiteListHandler blackWhiteListHandler, IDbCampaignService campaignService,
            ITwitterFunctionFactory twitterFunctionFactory, IDbInsertionHelper dbInsertionHelper)
            : base(jobProcess, blackWhiteListHandler, campaignService, twitterFunctionFactory.TwitterFunctions,
                dbInsertionHelper)
        {
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            _jobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

            var delayService = InstanceProvider.GetInstance<IDelayService>();

            if (_jobProcess.checkJobCompleted())
                return;
            CheckUserFilterActiveForCurrentQuery(queryInfo);
            string maxId = null;
            var hasNoResult = false;
            jobProcessResult = new JobProcessResult();

            // get user info
            var userInfo = TwitterFunction.GetUserDetails(_jobProcess.DominatorAccountModel, queryInfo.QueryValue,
                queryInfo.QueryType);

            if (userInfo.Success && userInfo.UserDetail.FollowersCount > 0 && !userInfo.UserDetail.IsPrivate)
            {
                #region Getting Followers operation

                var isNotFirst = false;
                jobProcessResult.maxId = GetPaginationId(queryInfo);

                while (!jobProcessResult.IsProcessCompleted && !hasNoResult)
                {
                    // get user followings
                    var followingsHandler = TwitterFunction
                        .GetUserFollowingsAsync(_jobProcess.DominatorAccountModel, queryInfo.QueryValue,
                            _jobProcess.JobCancellationTokenSource.Token, maxId).Result;
                    _jobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                    AddOrUpdatePaginationId(queryInfo, jobProcessResult.maxId, ref isNotFirst);

                    if (followingsHandler.Success)
                    {
                        foreach (var singleTwitterUser in followingsHandler.ListOfTwitterUser)
                        {
                            #region Inner loop mentioning Followers of Followings Operation

                            try
                            {
                                _jobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                                // HasNoResult will be false and maxId will be null for each user at first
                                jobProcessResult.HasNoResult = false;
                                jobProcessResult.maxId = null;

                                while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult &&
                                       !singleTwitterUser.IsPrivate)
                                {
                                    var followerOfFollowingsHandler = TwitterFunction
                                        .GetUserFollowersAsync(_jobProcess.DominatorAccountModel,
                                            singleTwitterUser.Username, _jobProcess.JobCancellationTokenSource.Token,
                                            jobProcessResult.maxId).Result;
                                    _jobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                                    if (followerOfFollowingsHandler.Success &&
                                        followerOfFollowingsHandler?.ListOfTwitterUser.Count > 0)
                                    {
                                        var lstTwitterUser = followerOfFollowingsHandler.ListOfTwitterUser;

                                        #region Skip BlackList or WhiteList

                                        lstTwitterUser = SkipBlackListOrWhiteList(lstTwitterUser);

                                        #endregion

                                        jobProcessResult = StartFinalProcess(queryInfo, jobProcessResult,
                                            lstTwitterUser);
                                        jobProcessResult.maxId = followerOfFollowingsHandler.MinPosition;
                                        if (!followerOfFollowingsHandler.HasMoreResults)
                                            jobProcessResult.HasNoResult = true;
                                    }
                                    else
                                    {
                                        jobProcessResult.HasNoResult = true;
                                        jobProcessResult.maxId = null;
                                    }

                                    delayService.ThreadSleep(TdConstants.ConsecutiveGetReqInteval);
                                }

                                if (jobProcessResult.IsProcessCompleted)
                                    break;
                            }

                            catch (OperationCanceledException)
                            {
                                throw new OperationCanceledException();
                            }
                            catch (Exception ex)
                            {
                                if (_jobProcess.DominatorAccountModel?.AccountBaseModel?.UserName != null)
                                    ex.DebugLog(
                                        $"TwtDominator : [Account: {_jobProcess.DominatorAccountModel.AccountBaseModel.UserName}]   (Module => {ActivityType.ToString()})");
                            }

                            #endregion
                        }

                        maxId = followingsHandler.MinPosition;
                        if (!followingsHandler.HasMoreResults)
                            jobProcessResult.HasNoResult = true;
                    }
                    else
                    {
                        hasNoResult = true;
                        maxId = null;
                    }
                }

                #endregion
            }

            if (!jobProcessResult.IsProcessCompleted)
                GlobusLogHelper.log.Info(Log.CustomMessage,
                    _jobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    _jobProcess.DominatorAccountModel.UserName, ActivityType,
                    $"No more data available to perform {ActivityType} for query value {queryInfo.QueryValue}");
        }
    }
}