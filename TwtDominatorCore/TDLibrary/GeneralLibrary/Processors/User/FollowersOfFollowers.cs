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
    internal class FollowersOfFollowers : BaseTwitterUserProcessor, IQueryProcessor
    {
        public FollowersOfFollowers(ITdJobProcess jobProcess, IBlackWhiteListHandler blackWhiteListHandler,
            IDbCampaignService campaignService,
            ITwitterFunctionFactory twitterFunctionFactory, IDbInsertionHelper dbInsertionHelper)
            : base(jobProcess, blackWhiteListHandler, campaignService, twitterFunctionFactory.TwitterFunctions,
                dbInsertionHelper)
        {
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            var delayService = InstanceProvider.GetInstance<IDelayService>();

            _jobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            if (_jobProcess.checkJobCompleted())
                return;
            CheckUserFilterActiveForCurrentQuery(queryInfo);
            string maxId = null;
            var hasNoResult = false;
            jobProcessResult = new JobProcessResult();

            _jobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

            var userInfo = TwitterFunction.GetUserDetails(_jobProcess.DominatorAccountModel, queryInfo.QueryValue,
                queryInfo.QueryType);


            if (userInfo.Success && userInfo.UserDetail.FollowersCount > 0 && !userInfo.UserDetail.IsPrivate)
            {
                var isNotFirst = false;
                jobProcessResult.maxId = GetPaginationId(queryInfo);

                #region Getting Followers operation

                while (!jobProcessResult.IsProcessCompleted && !hasNoResult)
                {
                    // We are not saving pagination first time we hit the url we will 
                    // save next time when we paginate it because pagination id gives us next page users
                    // here we save last page id
                    AddOrUpdatePaginationId(queryInfo, jobProcessResult.maxId, ref isNotFirst);

                    // get user followers
                    var followersHandler = TwitterFunction
                        .GetUserFollowersAsync(_jobProcess.DominatorAccountModel, queryInfo.QueryValue,
                            _jobProcess.JobCancellationTokenSource.Token, maxId).Result;

                    if (followersHandler.Success)
                    {
                        if (ActivityType == DominatorHouseCore.Enums.ActivityType.Mute && followersHandler != null && followersHandler.ListOfTwitterUser.Count > 0)
                            followersHandler.ListOfTwitterUser.RemoveAll(x => x.IsMuted);

                        foreach (var singleTwitterUser in followersHandler.ListOfTwitterUser)
                        {
                            #region Inner loop mentioning Followers of Followers Opertaion

                            try
                            {
                                // HasNoResult will be false and maxId will be null for each user at first
                                jobProcessResult.HasNoResult = false;
                                jobProcessResult.maxId = null;

                                var hasNoUserForCurrentUser = false;
                                while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult &&
                                       !singleTwitterUser.IsPrivate && !hasNoUserForCurrentUser)
                                {
                                    // get followers of user followers
                                    var followerOfFollowersHandler = TwitterFunction
                                        .GetUserFollowersAsync(_jobProcess.DominatorAccountModel,
                                            singleTwitterUser.Username, _jobProcess.JobCancellationTokenSource.Token,
                                            jobProcessResult.maxId).Result;
                                    if (ActivityType == DominatorHouseCore.Enums.ActivityType.Mute && followerOfFollowersHandler != null && followerOfFollowersHandler.ListOfTwitterUser.Count > 0)
                                        followerOfFollowersHandler.ListOfTwitterUser.RemoveAll(x => x.IsMuted);
                                    _jobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                                    if (followerOfFollowersHandler.Success &&
                                        followerOfFollowersHandler?.ListOfTwitterUser.Count > 0)
                                    {
                                        var lstTwitterUser = followerOfFollowersHandler.ListOfTwitterUser;

                                        #region Skip BlackList or WhiteList

                                        lstTwitterUser = SkipBlackListOrWhiteList(lstTwitterUser);

                                        #endregion

                                        jobProcessResult = StartFinalProcess(queryInfo, jobProcessResult,
                                            lstTwitterUser);
                                        jobProcessResult.maxId = followerOfFollowersHandler.MinPosition;
                                        if (!followerOfFollowersHandler.HasMoreResults)
                                            jobProcessResult.HasNoResult = true;
                                    }
                                    else
                                    {
                                        hasNoUserForCurrentUser = true;
                                        jobProcessResult.maxId = null;
                                    }

                                    delayService.ThreadSleep(TdConstants.ConsecutiveGetReqInteval);
                                }
                            }
                            catch (OperationCanceledException)
                            {
                                throw;
                            }
                            catch (Exception ex)
                            {
                                ex.ErrorLog();
                            }

                            #endregion
                        }

                        maxId = followersHandler.MinPosition;
                        if (!followersHandler.HasMoreResults)
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