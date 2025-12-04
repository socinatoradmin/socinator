using System;
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

namespace TwtDominatorCore.TDLibrary.GeneralLibrary.Processors.Tweet
{
    internal class FollowersOfFollowingsTweet : BaseTwitterTweetsProcessor, IQueryProcessor
    {
        public FollowersOfFollowingsTweet(ITdJobProcess jobProcess, IBlackWhiteListHandler blackWhiteListHandler,
            IDbCampaignService campaignService,
            ITwitterFunctionFactory twitterFunctionFactory, IDelayService threadUtility,
            IDbInsertionHelper dbInsertionHelper)
            : base(jobProcess, blackWhiteListHandler, campaignService, twitterFunctionFactory.TwitterFunctions,
                threadUtility, dbInsertionHelper)
        {
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            _jobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

            if (_jobProcess.checkJobCompleted()) return;

            CheckUserFilterActiveForCurrentQuery(queryInfo);
            string maxId = null;
            var hasNoResult = false;
            jobProcessResult = new JobProcessResult();

            var isNotFirst = false;
            jobProcessResult.maxId = GetPaginationId(queryInfo);

            _jobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

            GlobusLogHelper.log.Info(Log.CustomMessage,
                _jobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                _jobProcess.DominatorAccountModel.UserName, _jobProcess.ActivityType,
                "LangKeyCheckingPrivacySettingForUser".FromResourceDictionary() +
                TdUtility.GetUserNameFromUrl(queryInfo.QueryValue));

            var userInfo = TwitterFunction.GetUserDetails(_jobProcess.DominatorAccountModel, queryInfo.QueryValue,
                queryInfo.QueryValue);

            if (userInfo.Success && userInfo.UserDetail.FollowersCount > 0 && !userInfo.UserDetail.IsPrivate)
            {
                #region Getting Followers operation

                while (!jobProcessResult.IsProcessCompleted && !hasNoResult)
                {
                    AddOrUpdatePaginationId(queryInfo, jobProcessResult.maxId, ref isNotFirst);

                    ///  get user followings
                    var followingsHandler = TwitterFunction
                        .GetUserFollowingsAsync(_jobProcess.DominatorAccountModel, queryInfo.QueryValue,
                            _jobProcess.JobCancellationTokenSource.Token, maxId).Result;

                    if (followingsHandler.Success)
                    {
                        foreach (var singleTwitterUser in followingsHandler.ListOfTwitterUser)
                        {
                            #region Inner loop mentioning Followers of followings 

                            try
                            {
                                var hasNoUserForCurrentUser = false;

                                while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult &&
                                       !singleTwitterUser.IsPrivate && !hasNoUserForCurrentUser)
                                {
                                    ///  get followers of user followers
                                    var followerOfFollowersHandler = TwitterFunction
                                        .GetUserFollowersAsync(_jobProcess.DominatorAccountModel,
                                            singleTwitterUser.Username, _jobProcess.JobCancellationTokenSource.Token,
                                            jobProcessResult.maxId).Result;

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
                                }
                            }
                            catch (OperationCanceledException)
                            {
                                throw new OperationCanceledException();
                            }

                            catch (Exception ex)
                            {
                                ex.DebugLog();
                                // todo: handle exception
                            }

                            #endregion
                        }

                        maxId = followingsHandler.MinPosition;

                        if (!followingsHandler.HasMoreResults) jobProcessResult.HasNoResult = true;
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
                    string.Format("LangKeyNoMoreDataToActivityForQuery".FromResourceDictionary(), ActivityType,
                        queryInfo.QueryValue));
        }
    }
}