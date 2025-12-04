using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.TdTables.Accounts;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Process.JobLimits;
using DominatorHouseCore.Utility;
using DominatorUIUtility.Views.AccountSetting.Activity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TwtDominatorCore.Database;
using TwtDominatorCore.Factories;
using TwtDominatorCore.Requests;
using TwtDominatorCore.TDFactories;
using TwtDominatorCore.TDLibrary.GeneralLibrary.DAL;
using TwtDominatorCore.TDLibrary.GeneralLibrary.Subprocesses;
using TwtDominatorCore.TDModels;
using TwtDominatorCore.TDUtility;

namespace TwtDominatorCore.TDLibrary
{
    public class RetweetProcess : TdJobProcessInteracted<InteractedPosts>
    {
        #region Construction

        public RetweetProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IDbGlobalService globalService,
            IExecutionLimitsManager executionLimitsManager, ITwitterFunctionFactory twitterFunctionFactory,
            IJobActivityConfigurationManager jobActivityConfigurationManager,
            ITdQueryScraperFactory queryScraperFactory,
            ITdHttpHelper tdHttpHelper, ITwtLogInProcess twtLogInProcess, IDbInsertionHelper dbInsertionHelper,
            IEnumerable<ISubprocess<RetweetModel>> subprocess)
            : base(processScopeModel, accountServiceScoped, globalService, executionLimitsManager, queryScraperFactory,
                tdHttpHelper, twtLogInProcess)
        {
            _twtFuncFactory = twitterFunctionFactory;
            _jobActivityConfigurationManager = jobActivityConfigurationManager;
            _dbInsertionHelper = dbInsertionHelper;
            _subprocess = subprocess;
            RetweetModel = processScopeModel.GetActivitySettingsAs<RetweetModel>();
            InitializePerDayAndPercentageCount();
        }

        #endregion

        #region Public Properties

        public RetweetModel RetweetModel { get; set; }

        #endregion

        // changement in to assign functions factory beacuse of NewUi error to slove this we use this way.
        private ITwitterFunctions _twtFunc => _twtFuncFactory.TwitterFunctions;

        #region Public  Methods

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            var jobProcessResult = new JobProcessResult();
            try
            {
                var tagForProcess = (TagDetails)scrapeResult.ResultUser;
                jobProcessResult = PerformActionOnTweet(tagForProcess, scrapeResult,jobProcessResult);
                if (RetweetModel.IsChkActionTweetPerUser)
                {
                    var Count = RetweetModel.NoOfActionTweetPerUser.StartValue;
                    if (jobProcessResult.IsProcessSuceessfull)
                        Count--;
                    if(Count > 0)
                    {
                        var UserTweets = _twtFunc.GetTweetsFromUserFeedAsync(DominatorAccountModel, tagForProcess.Username, DominatorAccountModel.Token, "", ActivityType.Tweet, false, 100).Result;
                        UserTweets.UserTweetsDetail.RemoveAll(x => x.Id == tagForProcess.Id);
                        foreach (var Tweet in UserTweets.UserTweetsDetail)
                        {
                            jobProcessResult = PerformActionOnTweet(Tweet, scrapeResult, jobProcessResult);
                            if (jobProcessResult.IsProcessSuceessfull)
                                Count--;
                            if (Count <= 0)
                                break;
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                if (DominatorAccountModel?.AccountBaseModel?.UserName != null)
                    ex.DebugLog(
                        $"TwtDominator : [Account: {DominatorAccountModel.AccountBaseModel.UserName}] => Error : {ex.Message}. StackTrace => {ex.StackTrace}  (Module => {ActivityType.ToString()})");
            }

            return jobProcessResult;
        }

        private JobProcessResult PerformActionOnTweet(TagDetails tagForProcess,ScrapeResultNew scrapeResult,JobProcessResult jobProcessResult)
        {
            try
            {
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                if (RetweetModel.IsChkQuoteTweet && (RetweetModel.Unfollower.ListCustomUsers.Count != 0 || string.IsNullOrEmpty(RetweetModel.UploadQuotesTweets)))
                {
                    var quoteTweetText = string.Empty;
                    quoteTweetText = RetweetModel.IsSpintax ? SpinTexHelper.GetSpinText(RetweetModel.UploadQuotesTweets) : RetweetModel.UploadQuotesTweets;
                    var QuoteTweetResponse = _twtFunc.QuoteTweets(DominatorAccountModel, tagForProcess.Username, tagForProcess.Id, quoteTweetText?.Replace("\r\n","\\n"));
                    var Tweeturl = TdUtility.GetTweetUrl(tagForProcess.Username, tagForProcess.Id);
                    var OriginalID = tagForProcess.Id;
                    if (QuoteTweetResponse.Success)
                    {
                        IncrementCounters();
                        _dbInsertionHelper.AddInteractedTweetDetailInAccountDb(tagForProcess, ActivityType.ToString(),
                            ActivityType.ToString(), scrapeResult);
                        var QuotesTweetUrl = TdUtility.GetTweetUrl(string.IsNullOrEmpty(DominatorAccountModel.UserName)? tagForProcess.Username:DominatorAccountModel.UserName, QuoteTweetResponse.TweetId);
                        /// Updated from normal mode

                        #region  GetModuleSetting

                        var moduleModeSetting =
                            _jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];


                        if (moduleModeSetting == null) return jobProcessResult;

                        #endregion

                        if (moduleModeSetting.IsTemplateMadeByCampaignMode)
                            _dbInsertionHelper.AddInteractedTweetDetailInCampaignDb(tagForProcess, ActivityType.ToString(),
                                scrapeResult);

                        _dbInsertionHelper.AddFeedsInfo(
                            new TagDetails
                            {
                                Id = QuoteTweetResponse.TweetId,
                                Caption = RetweetModel.UploadQuotesTweets,
                                TweetedTimeStamp = DateTime.UtcNow.GetCurrentEpochTime()
                            }, true);
                        jobProcessResult.IsProcessSuceessfull = true;

                        JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                        GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.UserName,
                            ActivityType.Retweet, $"Quoted {Tweeturl} at => {QuotesTweetUrl}");
                        PostRepostAction(Tweeturl, OriginalID);
                        PostRetweetProcess(tagForProcess);
                        jobProcessResult.IsProcessSuceessfull = true;
                    }
                    else
                    {
                        GlobusLogHelper.log.Info(Log.ActivityFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.UserName, ActivityType.Retweet, $"Quoted {Tweeturl} ==> "+
                            QuoteTweetResponse.Issue?.Message);
                        jobProcessResult.IsProcessSuceessfull = false;
                    }
                }
                else
                {
                    var retweetResponse = _twtFunc.Retweet(DominatorAccountModel, tagForProcess.Id, tagForProcess.Username,scrapeResult.QueryInfo.QueryType);
                    var Tweeturl = TdUtility.GetTweetUrl(tagForProcess.Username, tagForProcess.Id);
                    var OriginalID = tagForProcess.Id;
                    if (retweetResponse.Success)
                    {
                        tagForProcess.IsRetweet = true;
                        IncrementCounters();

                        _dbInsertionHelper.AddFeedsInfo(tagForProcess, true);
                        _dbInsertionHelper.AddInteractedTweetDetailInAccountDb(tagForProcess, ActivityType.ToString(),
                            ActivityType.ToString(), scrapeResult);
                        var RetweetTweetUrl = TdUtility.GetTweetUrl(string.IsNullOrEmpty(DominatorAccountModel.UserName) ? tagForProcess.Username : DominatorAccountModel.UserName, retweetResponse?.TweetId);
                        GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.UserName,
                            ActivityType.Retweet,$"{Tweeturl} at => {RetweetTweetUrl}");



                        // Updated from normal mode

                        #region  GetModuleSetting

                        var moduleModeSetting =
                            _jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];

                        if (moduleModeSetting == null) return jobProcessResult;

                        #endregion

                        if (moduleModeSetting.IsTemplateMadeByCampaignMode)
                            _dbInsertionHelper.AddInteractedTweetDetailInCampaignDb(tagForProcess, ActivityType.ToString(),
                                scrapeResult);

                        PostRepostAction(Tweeturl, OriginalID);
                        PostRetweetProcess(tagForProcess);
                        jobProcessResult.IsProcessSuceessfull = true;
                    }

                    else
                    {
                        GlobusLogHelper.log.Info(Log.ActivityFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.UserName, ActivityType.Retweet,$"{Tweeturl} ==> {retweetResponse.Issue?.Message}");
                        jobProcessResult.IsProcessSuceessfull = false;
                    }
                }

                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                if (RetweetModel != null && RetweetModel.EnableDelayBetweenPerformingActionOnSamePost)
                    DelayBeforeNextActivity(RetweetModel.DelayBetweenPerformingActionOnSamePost.GetRandom());
                else
                    DelayBeforeNextActivity();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return jobProcessResult;
        }

        #endregion

        #region Private Fields

        private readonly IEnumerable<ISubprocess<RetweetModel>> _subprocess;
        private readonly ITwitterFunctionFactory _twtFuncFactory;
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
            PerDayCountToLike = RetweetModel.LikeMaxRange.GetRandom();
            PerDayCountToRetweet = RetweetModel.RetweetMaxRange.GetRandom();
            PerDayCountToComment = RetweetModel.CommentMaxRange.GetRandom();
            PerDayCountToFollow = RetweetModel.FollowMaxRange.GetRandom();
        }

        private void PostRetweetProcess(TagDetails tweet)
        {
            try
            {
                GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.UserName, ActivityType, $"Post retweet process started to tweet {tweet.Id}");
                if (RetweetModel.IsChkEnableLikeComments || RetweetModel.IsChkEnableRetweetComments ||
                    RetweetModel.IsChkUploadComments || RetweetModel.IsChkFollowTWeetOwner ||
                    RetweetModel.IsChkLikeTweet)
                    _subprocess.ForEach(a => { a.Run(JobCancellationTokenSource, tweet, RetweetModel); });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
        private async void PostRepostAction(string Tweeturl,string ID)
        {
            if (RetweetModel.IsBookmarkTweet)
            {
                GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, "BookMarkTweet",
                    $"Trying to bookmark => {Tweeturl}");
                var bookMarkResponse = await _twtFunc.BookMarkTweet(DominatorAccountModel, ID);
                if (bookMarkResponse != null && bookMarkResponse.Success)
                {
                    GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                        DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.UserName,
                        "BookMarkTweet", $"{Tweeturl}");
                }
                else
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName,
                        "BookMarkTweet", $"Failed to bookmark => {Tweeturl} With Error ==>{bookMarkResponse?.Issue?.Message}");
                }
                await Task.Delay(TimeSpan.FromSeconds(3), DominatorAccountModel.Token);
            }
        }
        #endregion
    }
}