using System;
using System.Collections.Generic;
using ThreadUtils;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using TwtDominatorCore.Database;
using TwtDominatorCore.TDFactories;
using TwtDominatorCore.TDLibrary.GeneralLibrary.DAL;
using TwtDominatorCore.TDModels;

namespace TwtDominatorCore.TDLibrary.GeneralLibrary.Processors.User
{
    internal class CustomUsers : BaseTwitterUserProcessor, IQueryProcessor
    {
        private readonly IDelayService _delayService;

        public CustomUsers(ITdJobProcess jobProcess, IBlackWhiteListHandler blackWhiteListHandler,
            IDbCampaignService campaignService, ITwitterFunctionFactory twitterFunctionFactory,
            IDelayService threadUtility, IDbInsertionHelper dbInsertionHelper)
            : base(jobProcess, blackWhiteListHandler, campaignService, twitterFunctionFactory.TwitterFunctions,
                dbInsertionHelper)

        {
            _delayService = threadUtility;
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            if (_jobProcess.checkJobCompleted())
                return;


            CheckUserFilterActiveForCurrentQuery(queryInfo);
            jobProcessResult = new JobProcessResult();

            if (queryInfo.QueryValue == _jobProcess.DominatorAccountModel.UserName
                && ActivityType == ActivityType.Follow)
            {
                GlobusLogHelper.log.Info(Log.CustomMessage,
                    _jobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    _jobProcess.DominatorAccountModel.UserName, _jobProcess.ActivityType,
                    $"Custom Username '{queryInfo.QueryValue}' and selected Account Username must not be same for Follow Activity.");
                return;
            }

            ;


            var userInfo =
                TwitterFunction.GetUserDetails(_jobProcess.DominatorAccountModel, queryInfo.QueryValue,
                    queryInfo.QueryType, true);

            var lstTwitterUser = new List<TwitterUser>
            {
                userInfo.UserDetail
            };

            if (_jobProcess.ModuleSetting.IsCampaignWiseUniqueChecked &&
                (IsActivityDoneWithThisUserIdCampaignWise(userInfo.UserDetail.UserId) ||
                 !IsUniqueUser(userInfo.UserDetail)) && _jobProcess.ActivityType != ActivityType.UserScraper)
                return;

            #region Skip BlackList or WhiteList

            lstTwitterUser = SkipBlackListOrWhiteList(lstTwitterUser);

            #endregion

            if (lstTwitterUser.Count == 0)
            {
                GlobusLogHelper.log.Info(Log.CustomMessage,
                    _jobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    _jobProcess.DominatorAccountModel.UserName, ActivityType,
                    $"{userInfo.UserDetail.Username} user skipped present in BlackList");
                return;
            }


            if (userInfo.Success && !string.IsNullOrEmpty(userInfo.UserDetail.UserId))
            {
                _jobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                if (ActivityType == ActivityType.Follow || ActivityType == ActivityType.UserScraper ||
                    ActivityType == ActivityType.Mute || ActivityType == ActivityType.TweetTo)
                {
                    try
                    {
                        _delayService.ThreadSleep(new Random().Next(2000, 7000));
                        var IsSkippedUser=false;
                        if (ActivityType == ActivityType.UserScraper 
                            && ((IsSkippedUser = _dbAccountService.IsActivityDoneWithThisUserId(userInfo.UserDetail.UserId, ActivityType)) 
                            ||!IsUniqueUser(userInfo.UserDetail)))
                        {
                            if(IsSkippedUser)
                                GlobusLogHelper.log.Info(Log.CustomMessage,_jobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    _jobProcess.DominatorAccountModel.UserName, ActivityType,
                                    $"Skipped {userInfo.UserDetail.Username} As Interatced User.");
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                    FinalProcessForEachUser(queryInfo, out jobProcessResult, userInfo.UserDetail);
                }

                #region For Liking, retweeting, reposting custom users post

                else
                {
                    if (!UserFilterApply(userInfo.UserDetail))
                    {
                        var hasNoMore = false;
                        InitializeTweetScrapePerUser();

                        do
                        {
                            foreach (var eachTag in userInfo.UserDetail.ListTag)
                            {
                                FinalProcessForEachTag(queryInfo, out jobProcessResult, eachTag);

                                if (jobProcessResult.IsProcessSuceessfull) NoOfTweetsToBeScrapePerUser--;

                                if (jobProcessResult.IsProcessCompleted || NoOfTweetsToBeScrapePerUser == 0) return;
                            }

                            userInfo = TwitterFunction.GetUserDetails(_jobProcess.DominatorAccountModel,
                                queryInfo.QueryValue, queryInfo.QueryType, true,
                                userInfo.MinPosition);

                            if (!userInfo.Success || !userInfo.Hasmore) hasNoMore = true;
                        } while (!hasNoMore);
                    }
                }

                #endregion
            }
            else
            {
                GlobusLogHelper.log.Info(Log.CustomMessage,
                    _jobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    _jobProcess.DominatorAccountModel.UserName, ActivityType,
                    $"{userInfo.UserDetail.Username} was not found");
            }
        }
    }
}