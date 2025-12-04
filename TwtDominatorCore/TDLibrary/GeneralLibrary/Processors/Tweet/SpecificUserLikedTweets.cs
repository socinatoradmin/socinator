using System;
using System.Linq;
using ThreadUtils;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using Newtonsoft.Json;
using TwtDominatorCore.Database;
using TwtDominatorCore.Response;
using TwtDominatorCore.TDFactories;
using TwtDominatorCore.TDLibrary.GeneralLibrary.DAL;
using TwtDominatorCore.TDUtility;
using static TwtDominatorCore.TDEnums.Enums;

namespace TwtDominatorCore.TDLibrary.GeneralLibrary.Processors.Tweet
{
    internal class SpecificUserLikedTweets : BaseTwitterTweetsProcessor, IQueryProcessor
    {
        public SpecificUserLikedTweets(ITdJobProcess jobProcess, IBlackWhiteListHandler blackWhiteListHandler,
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
            jobProcessResult = new JobProcessResult();
            _jobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

            UserDetailsResponseHandler userInfo;

            // Getting own liked tweet for unlike own tweets
            if (ActivityType == ActivityType.Unlike)
            {
                GlobusLogHelper.log.Info(Log.CustomMessage,
                    _jobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    _jobProcess.DominatorAccountModel.UserName, ActivityType,
                    string.Format("LangKeyGettingLikedTweet".FromResourceDictionary(),
                        _jobProcess.DominatorAccountModel.UserName));

                var userScreenName = _jobProcess.DominatorAccountModel.AccountBaseModel.ProfileId;
                userScreenName = string.IsNullOrEmpty(userScreenName) ?_jobProcess.DominatorAccountModel.AccountBaseModel.UserName:userScreenName;
                #region if profileId contains emailId

                try
                {
                    if (userScreenName.Contains('@'))
                    {
                        var dominatorAccUserDetails =
                            _jobProcess.DominatorAccountModel.ExtraParameters[
                                ModuleExtraDetails.UserProfileDetails.ToString()];
                        var userDetails = JsonConvert.DeserializeObject<UserProfileDetails>(dominatorAccUserDetails);
                        userScreenName = userDetails.UserName;
                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                #endregion

                userInfo = TwitterFunction.GetUserDetails(_jobProcess.DominatorAccountModel, userScreenName,
                    queryInfo.QueryType, true);
            }
            else
            {
                // Getting liked tweets of specific user
                GlobusLogHelper.log.Info(Log.CustomMessage,
                    _jobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    _jobProcess.DominatorAccountModel.UserName, ActivityType,
                    "LangKeyCheckingPrivacySettingForUser".FromResourceDictionary() +
                    TdUtility.GetUserNameFromUrl(queryInfo.QueryValue));

                userInfo = TwitterFunction.GetUserDetails(_jobProcess.DominatorAccountModel, queryInfo.QueryValue,
                    queryInfo.QueryType);
            }

            if (userInfo.Success && userInfo.UserDetail.LikesCount > 0 && !userInfo.UserDetail.IsPrivate)
            {
                #region Inner loop getting user liked tweets

                var hasNoTweetForCurrentUser = false;

                while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult &&
                       !userInfo.UserDetail.IsPrivate && !hasNoTweetForCurrentUser)
                {
                    //  get liked tweet of user 
                    var userLikedTweetsResponseHandler = TwitterFunction
                        .GetTweetsFromUserFeedAsync(_jobProcess.DominatorAccountModel, userInfo.UserDetail.Username,
                            _jobProcess.JobCancellationTokenSource.Token,
                            jobProcessResult.maxId, ActivityType.TweetScraper).Result;
                    _jobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                    if (userLikedTweetsResponseHandler.Success &&
                        userLikedTweetsResponseHandler.UserTweetsDetail.Count > 0)
                    {
                        var lstTwitterTags = userLikedTweetsResponseHandler.UserTweetsDetail;
                        jobProcessResult = StartFinalProcess(queryInfo, jobProcessResult, lstTwitterTags);
                        jobProcessResult.maxId = userLikedTweetsResponseHandler.MinPosition;

                        if (!userLikedTweetsResponseHandler.hasmore) jobProcessResult.HasNoResult = true;
                    }
                    else
                    {
                        hasNoTweetForCurrentUser = true;
                        jobProcessResult.maxId = null;
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