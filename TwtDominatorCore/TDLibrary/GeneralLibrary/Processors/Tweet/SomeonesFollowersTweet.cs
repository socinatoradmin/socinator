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

namespace TwtDominatorCore.TDLibrary.GeneralLibrary.Processors.Tweet
{
    internal class SomeonesFollowersTweet : BaseTwitterTweetsProcessor, IQueryProcessor
    {
        private readonly ITDAccountUpdateFactory _accountUpdateFactory;

        public SomeonesFollowersTweet(ITdJobProcess jobProcess, IBlackWhiteListHandler blackWhiteListHandler,
            IDbCampaignService campaignService, ITwitterFunctionFactory twitterFunctionFactory,
            ITDAccountUpdateFactory accountUpdateFactory, IDelayService threadUtility,
            IDbInsertionHelper dbInsertionHelper)
            : base(jobProcess, blackWhiteListHandler, campaignService, twitterFunctionFactory.TwitterFunctions,
                threadUtility, dbInsertionHelper)
        {
            _accountUpdateFactory = accountUpdateFactory;
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            _jobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

            if (_jobProcess.checkJobCompleted()) return;

            CheckUserFilterActiveForCurrentQuery(queryInfo);
            jobProcessResult = new JobProcessResult();

            var isNotFirst = false;
            jobProcessResult.maxId = GetPaginationId(queryInfo);

            if (string.Equals(queryInfo.QueryValue, "[Own]", StringComparison.CurrentCultureIgnoreCase))
            {
                var followersList = StartProcessForOwnFollowers();
                StartFinalProcess(queryInfo, jobProcessResult, followersList);
            }
            else
            {
                GlobusLogHelper.log.Info(Log.CustomMessage,
                    _jobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    _jobProcess.DominatorAccountModel.UserName, _jobProcess.ActivityType,
                    "LangKeyCheckingPrivacySettingForUser".FromResourceDictionary() +
                    $" {TdUtility.GetUserNameFromUrl(queryInfo.QueryValue)}");

                var userInfo = TwitterFunction.GetUserDetails(_jobProcess.DominatorAccountModel, queryInfo.QueryValue,
                    queryInfo.QueryType);

                if (userInfo.Success && userInfo.UserDetail.FollowersCount > 0 &&
                    (!userInfo.UserDetail.IsPrivate || queryInfo.QueryValue.Trim()
                         .Equals(_jobProcess.DominatorAccountModel.AccountBaseModel.UserName,
                             StringComparison.InvariantCultureIgnoreCase)))
                    while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
                    {
                        AddOrUpdatePaginationId(queryInfo, jobProcessResult.maxId, ref isNotFirst);

                        var followersHandler = TwitterFunction.GetUserFollowersAsync(_jobProcess.DominatorAccountModel,
                            queryInfo.QueryValue,
                            _jobProcess.JobCancellationTokenSource.Token, jobProcessResult.maxId).Result;

                        _jobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                        if (followersHandler.Success)
                        {
                            var lstTwitterUser = followersHandler.ListOfTwitterUser;

                            #region Skip BlackList or WhiteList

                            lstTwitterUser = SkipBlackListOrWhiteList(lstTwitterUser);

                            #endregion

                            jobProcessResult = StartFinalProcess(queryInfo, jobProcessResult, lstTwitterUser);
                            jobProcessResult.maxId = followersHandler.MinPosition;

                            if (!followersHandler.HasMoreResults) jobProcessResult.HasNoResult = true;
                        }
                        else
                        {
                            jobProcessResult.maxId = null;
                            jobProcessResult.HasNoResult = true;
                        }
                    }


                if (!jobProcessResult.IsProcessCompleted)
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                        _jobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        _jobProcess.DominatorAccountModel.UserName,
                        ActivityType,
                        string.Format("LangKeyNoMoreDataToActivityForQuery".FromResourceDictionary(), ActivityType,
                            queryInfo.QueryValue));
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