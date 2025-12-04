using System;
using System.Collections.Generic;
using System.Threading;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.TdTables.Accounts;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Process.JobLimits;
using DominatorHouseCore.Utility;
using DominatorUIUtility.Views.AccountSetting.Activity;
using TwtDominatorCore.Database;
using TwtDominatorCore.Factories;
using TwtDominatorCore.Requests;
using TwtDominatorCore.TDFactories;
using TwtDominatorCore.TDLibrary.GeneralLibrary.DAL;
using TwtDominatorCore.TDLibrary.GeneralLibrary.Subprocesses;
using TwtDominatorCore.TDModels;
using TwtDominatorCore.TDUtility;
using static System.Net.Mime.MediaTypeNames;

namespace TwtDominatorCore.TDLibrary
{
    public class LikeProcess : TdJobProcessInteracted<InteractedPosts>
    {
        #region Construction

        public LikeProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IDbGlobalService globalService,
            IExecutionLimitsManager executionLimitsManager, ITwitterFunctionFactory twitterFunctionFactory,
            IJobActivityConfigurationManager jobActivityConfigurationManager,
            ITdQueryScraperFactory queryScraperFactory,
            ITdHttpHelper tdHttpHelper, ITwtLogInProcess twtLogInProcess, IDbInsertionHelper dbInsertionHelper,
            IEnumerable<ISubprocess<LikeModel>> subprocess)
            : base(processScopeModel, accountServiceScoped, globalService, executionLimitsManager, queryScraperFactory,
                tdHttpHelper, twtLogInProcess)
        {
            _twitterFunctionsFactory = twitterFunctionFactory;
            _jobActivityConfigurationManager = jobActivityConfigurationManager;
            _dbInsertionHelper = dbInsertionHelper;
            _subprocess = subprocess;
            LikeModel = processScopeModel.GetActivitySettingsAs<LikeModel>();
            InitializePerDayAndPercentageCount();
        }

        #endregion

        #region Public Properties

        public LikeModel LikeModel { get; set; }

        #endregion

        private ITwitterFunctions _twitterFunctions => _twitterFunctionsFactory.TwitterFunctions;

        #region Public Methods

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            var jobProcessResult = new JobProcessResult();

            try
            {
                var tagForProcess = (TagDetails) scrapeResult.ResultUser;
                if(LikeModel != null && LikeModel.IsChkActionTweetPerUser)
                {
                    var MaxLikeCount = LikeModel.NoOfActionTweetPerUser.StartValue;
                    var CurrentCount = 0;
                    GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.UserName, ActivityType,
                    $"Getting Tweets of {tagForProcess.Username}");
                    var tags = _twitterFunctions.GetTweetsFromUserFeedAsync(DominatorAccountModel, tagForProcess.Username, JobCancellationTokenSource.Token, string.Empty, ActivityType.Tweet, false, MaxLikeCount).Result;
                    if(tags != null && tags.UserTweetsDetail.Count >= MaxLikeCount)
                    {
                        foreach(var tweet in tags.UserTweetsDetail)
                        {
                            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                            try
                            {
                                if (LikeTweets(tweet, scrapeResult, ref jobProcessResult,true))
                                    CurrentCount++;
                                if (CurrentCount >= MaxLikeCount)
                                    break;
                            }
                            catch (OperationCanceledException)
                            {
                                throw new OperationCanceledException("Operation Cancelled!");
                            }
                            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                        }
                    }
                    else
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.UserName, ActivityType,
                            $"Tweet Not Found As Per Campaign Setting.Trying To Like Latest Tweet.");
                        LikeTweets(tagForProcess, scrapeResult, ref jobProcessResult, true);
                    }
                }
                else
                {
                    LikeTweets(tagForProcess, scrapeResult,ref jobProcessResult);
                }
                
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException("Operation Cancelled!");
            }
            catch (Exception ex)
            {
                if (DominatorAccountModel?.AccountBaseModel?.UserName != null)
                    ex.DebugLog(
                        $"TwtDominator : [Account: {DominatorAccountModel.AccountBaseModel.UserName}] => Error : {ex.Message}. StackTrace => {ex.StackTrace}  (Module => {ActivityType.ToString()})");
            }

            return jobProcessResult;
        }

        private bool LikeTweets(TagDetails tagForProcess,ScrapeResultNew scrapeResult,ref JobProcessResult jobProcessResult,bool IsChkActionTweetPerUser=false)
        {
            var tweetURL = TdUtility.GetTweetUrl(tagForProcess.Username, tagForProcess.Id);
            var Message = IsChkActionTweetPerUser ? tweetURL + $" of User {tagForProcess.Username}" : tweetURL;
            GlobusLogHelper.log.Info(Log.StartedActivity,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.UserName,
                            ActivityType,Message);
            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

            var likeResponse = _twitterFunctions.Like(DominatorAccountModel, tagForProcess.Id,
                tagForProcess.Username, scrapeResult.QueryInfo.QueryType);

            #region  GetModuleSetting

            var moduleModeSetting =
                _jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];

            if (moduleModeSetting == null) return false;

            #endregion

            if (likeResponse.Success)
            {
                GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                    DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.UserName,
                    ActivityType.Like, Message);

                IncrementCounters();

                _dbInsertionHelper.AddInteractedTweetDetailInAccountDb(tagForProcess, ActivityType.ToString(),
                    ActivityType.ToString(), scrapeResult);



                if (moduleModeSetting.IsTemplateMadeByCampaignMode)
                    _dbInsertionHelper.AddInteractedTweetDetailInCampaignDb(tagForProcess, ActivityType.ToString(),
                        scrapeResult);
                if (LikeModel.IsChkEnableLikeComments || LikeModel.IsChkEnableRetweetComments
                                                      || LikeModel.IsChkUploadComments ||
                                                      LikeModel.IsChkFollowTWeetOwner)
                    PostLikeProcess(tagForProcess, JobCancellationTokenSource);
                jobProcessResult.IsProcessSuceessfull = true;
            }
            else
            {
                GlobusLogHelper.log.Info(Log.ActivityFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.UserName, ActivityType.Like,Message + " ==> "+
                    likeResponse.Issue?.Message);
                jobProcessResult.IsProcessSuceessfull = false;
            }

            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            if (ModuleSetting.EnableDelayBetweenPerformingActionOnSamePost)
            {
                var specificDelay = ModuleSetting.DelayBetweenPerformingActionOnSamePost.GetRandom();
                DelayBeforeNextActivity(specificDelay);
            }
            else
                DelayBeforeNextActivity();
            return likeResponse.Success;
        }

        #endregion

        #region Private Fields

        private readonly IEnumerable<ISubprocess<LikeModel>> _subprocess;
        private readonly ITwitterFunctionFactory _twitterFunctionsFactory;
        private readonly IJobActivityConfigurationManager _jobActivityConfigurationManager;
        private readonly IDbInsertionHelper _dbInsertionHelper;

        #endregion

        #region Private Properties

        private int PerDayCountToLike { get; set; }
        private int PerDayCountToRetweet { get; set; }
        private int PerDayCountToComment { get; set; }
        private int PerDayCountToFollow { get; set; }

        #endregion

        #region Private Methods

        private void InitializePerDayAndPercentageCount()
        {
            PerDayCountToLike = LikeModel.LikeMaxRange.GetRandom();
            PerDayCountToRetweet = LikeModel.RetweetMaxRange.GetRandom();
            PerDayCountToComment = LikeModel.CommentMaxRange.GetRandom();
            PerDayCountToFollow = LikeModel.FollowMaxRange.GetRandom();
        }

        private void PostLikeProcess(TagDetails tweet, CancellationTokenSource jobCancellationTokenSource)
        {
            try
            {
                GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.UserName, ActivityType,
                    string.Format("LangKeyPostLikeProcessStartedTweet".FromResourceDictionary(), tweet.IsRetweet?tweet.OriginalTweetId:tweet.Id));

                _subprocess.ForEach(a => { a.Run(jobCancellationTokenSource, tweet, LikeModel); });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        #endregion
    }
}