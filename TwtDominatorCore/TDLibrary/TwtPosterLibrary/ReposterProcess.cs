using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.TdTables.Accounts;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Process.JobLimits;
using DominatorHouseCore.Utility;
using Emgu.CV.Dnn;
using TwtDominatorCore.Database;
using TwtDominatorCore.Factories;
using TwtDominatorCore.Requests;
using TwtDominatorCore.TDFactories;
using TwtDominatorCore.TDLibrary.GeneralLibrary;
using TwtDominatorCore.TDLibrary.GeneralLibrary.DAL;
using TwtDominatorCore.TDModels;
using TwtDominatorCore.TDUtility;

namespace TwtDominatorCore.TDLibrary
{
    public class ReposterProcess : TdJobProcessInteracted<InteractedPosts>
    {
        #region Construction

        public ReposterProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IDbGlobalService globalService,
            IExecutionLimitsManager executionLimitsManager, ITwitterFunctionFactory twitterFunctionFactory,
            IJobActivityConfigurationManager jobActivityConfigurationManager,
            ITdQueryScraperFactory queryScraperFactory, ITdHttpHelper tdHttpHelper, ITwtLogInProcess twtLogInProcess,
            IDbInsertionHelper dbInsertionHelper)
            : base(processScopeModel, accountServiceScoped, globalService, executionLimitsManager, queryScraperFactory,
                tdHttpHelper, twtLogInProcess)
        {
            _twtFuncFactory = twitterFunctionFactory;
            _jobActivityConfigurationManager = jobActivityConfigurationManager;
            _dbInsertionHelper = dbInsertionHelper;
            RepostModel = processScopeModel.GetActivitySettingsAs<ReposterModel>();
            // postCaption = processScopeModel.GetActivitySettingsAs<PostCaptionFormate>();
        }

        #endregion

        #region Public Properties

        public ReposterModel RepostModel { get; set; }
        //public PostCaptionFormate postCaption { get; set; }

        #endregion

        // changement in to assign functions factory beacuse of NewUi error to slove this we use this way.
        private ITwitterFunctions _twtFunc => _twtFuncFactory.TwitterFunctions;

        #region Private Fields

        private readonly ITwitterFunctionFactory _twtFuncFactory;
        private readonly IJobActivityConfigurationManager _jobActivityConfigurationManager;
        private readonly IDbInsertionHelper _dbInsertionHelper;

        #endregion

        #region Public Methods

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            var SpintaxCaption = string.Empty;
            var jobProcessResult = new JobProcessResult();
            try
            {
                var FailedCount = 0;
            TryAgain:
                var tagForProcess = (TagDetails) scrapeResult.ResultUser;
                var text = RepostModel.OriginalPostCaptionInputText;
                var filePath = new List<string>();
                var CaptionText = string.Empty;
                var RepostedTweetId = string.Empty;
                var IsSuccess = false;
                var Message= string.Empty;
                if (RepostModel.IsChkQuoteTweet && (RepostModel.Unfollower.ListCustomUsers.Count != 0 || string.IsNullOrEmpty(RepostModel.UploadQuotesTweets)))
                {
                    var quoteTweetText = string.Empty;
                    quoteTweetText = RepostModel.IsSpintax ? SpinTexHelper.GetSpinText(RepostModel.UploadQuotesTweets) : RepostModel.UploadQuotesTweets;
                    var QuoteTweetResponse = _twtFunc.QuoteTweets(DominatorAccountModel, tagForProcess.Username, tagForProcess.Id, quoteTweetText?.Replace("\r\n", "\\n"));
                    var Tweeturl1 = TdUtility.GetTweetUrl(tagForProcess.Username, tagForProcess.Id);
                    var OriginalID = tagForProcess.Id;
                    while(FailedCount++ <=3 && (QuoteTweetResponse!= null && QuoteTweetResponse.Issue != null && !string.IsNullOrEmpty(QuoteTweetResponse?.Issue?.Message)
                        && QuoteTweetResponse.Issue.Message.Contains("Authorization: This request looks like it might be automated. To protect our users from spam and other malicious activity")))
                    {
                        Thread.Sleep(TimeSpan.FromSeconds(6));
                        goto TryAgain;
                    }
                    if (QuoteTweetResponse.Success)
                    {
                        IncrementCounters();
                        _dbInsertionHelper.AddInteractedTweetDetailInAccountDb(tagForProcess, ActivityType.ToString(),
                            ActivityType.ToString(), scrapeResult);
                        var QuotesTweetUrl = TdUtility.GetTweetUrl(string.IsNullOrEmpty(DominatorAccountModel.UserName) ? tagForProcess.Username : DominatorAccountModel.UserName, QuoteTweetResponse.TweetId);
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
                                Caption = RepostModel.UploadQuotesTweets,
                                TweetedTimeStamp = DateTime.UtcNow.GetCurrentEpochTime()
                            }, true);
                        jobProcessResult.IsProcessSuceessfull = true;

                        JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                        GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.UserName,
                            ActivityType.Reposter, $"Quoted {Tweeturl1} at => {QuotesTweetUrl}");
                        PostRepostAction(Tweeturl1, OriginalID);
                        jobProcessResult.IsProcessSuceessfull = true;
                    }
                    else
                    {
                        GlobusLogHelper.log.Info(Log.ActivityFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.UserName, ActivityType.Reposter, $"Quoted {Tweeturl1} ==> " +
                            QuoteTweetResponse.Issue?.Message);
                        jobProcessResult.IsProcessSuceessfull = false;
                    }
                }
                else
                {
                    if (!_twtFunc.GettingTweetMedia(DominatorAccountModel, tagForProcess, ref filePath, ActivityType, RepostModel.DownloadFolderPath)) return jobProcessResult;
                    if (RepostModel.IsChkPostCaption)
                    {
                        if (RepostModel.IsSpintax)
                            SpintaxCaption = SpinTexHelper.GetSpinText(text);
                        CaptionText = PostCaptionText(tagForProcess.Caption, tagForProcess.Username,
                            SpintaxCaption, text);
                        tagForProcess.Caption = CaptionText;
                    }
                    JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    if (!string.IsNullOrEmpty(CaptionText))
                    {
                        var response = _twtFunc.QuoteTweets(DominatorAccountModel, tagForProcess.Username, tagForProcess.Id, CaptionText?.Replace("\r\n", "\\n"));
                        IsSuccess = response != null && response.Success;
                        if (IsSuccess)
                            RepostedTweetId = response.TweetId;
                        Message = response?.Issue?.Message ?? string.Empty;
                    }
                    else
                    {
                        var tweetUrl = TdUtility.GetTweetUrl(tagForProcess.Username, tagForProcess.Id);
                        var response = _twtFunc.RepostTweet(DominatorAccountModel, tweetUrl);
                        IsSuccess = response != null && response.Success;
                        if (IsSuccess)
                            RepostedTweetId = response.TweetId;
                        Message = response?.Issue?.Message ?? string.Empty;
                    }
                    var Tweeturl = TdUtility.GetTweetUrl(tagForProcess.Username, tagForProcess.Id);
                    if (IsSuccess)
                    {
                        JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                        var RepostedTweetUrl = TdUtility.GetTweetUrl(string.IsNullOrEmpty(DominatorAccountModel.UserName) ? tagForProcess.Username : DominatorAccountModel.UserName, RepostedTweetId);
                        GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.UserName,
                            ActivityType.Reposter, $"{Tweeturl} at {RepostedTweetUrl}");
                        var OriginalID = tagForProcess.Id;
                        tagForProcess.Id = RepostedTweetId;
                        IncrementCounters();
                        _dbInsertionHelper.AddInteractedTweetDetailInAccountDb(tagForProcess, ActivityType.ToString(),
                            ActivityType.ToString(), scrapeResult);

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
                                Id = RepostedTweetId,
                                Caption = tagForProcess.Caption,
                                Code = filePath == null ? "" : string.Join(",", filePath),
                                TweetedTimeStamp = DateTime.UtcNow.GetCurrentEpochTime()
                            }, true);
                        PostRepostAction(Tweeturl, OriginalID);
                        jobProcessResult.IsProcessSuceessfull = true;
                    }
                    else
                    {
                        JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                        if (Message == "Status is a duplicate.")
                            GlobusLogHelper.log.Info(Log.ActivityFailed,
                                DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                DominatorAccountModel.UserName, ActivityType.Reposter,
                                Tweeturl + " ==> " + Message);
                        else
                            GlobusLogHelper.log.Info(Log.ActivityFailed,
                                DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                DominatorAccountModel.UserName, ActivityType.Reposter, Tweeturl + " ==> " +
                                Message);

                        jobProcessResult.IsProcessSuceessfull = false;
                    }
                }
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                if (RepostModel != null && RepostModel.IsEnableAdvancedUserMode && RepostModel.EnableDelayBetweenPerformingActionOnSamePost)
                    DelayBeforeNextActivity(RepostModel.DelayBetweenPerformingActionOnSamePost.GetRandom());
                else
                    DelayBeforeNextActivity();
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

        private async void PostRepostAction(string Tweeturl,string ID)
        {
            if (RepostModel.IsBookmarkTweet)
            {
                GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                DominatorAccountModel.AccountBaseModel.UserName, "BookMarkTweet",
                                $"Trying to bookmark => {Tweeturl}");
                var bookMarkResponse = await _twtFunc.BookMarkTweet(DominatorAccountModel, ID);
                if(bookMarkResponse != null && bookMarkResponse.Success)
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

        public string PostCaptionText(string caption, string Username, string Captionspintax, string CaptionText)
        {
            string captions;
            if (RepostModel.IsSpintax)
                captions = Captionspintax;
            else
                captions = CaptionText;


            captions = captions.Contains(PostCaptionFormate.OriginalPostCaption)
                ? captions.Replace(PostCaptionFormate.OriginalPostCaption, $" {caption}")
                : captions.Replace(PostCaptionFormate.OriginalPostCaption, "");
            captions = captions.Contains(PostCaptionFormate.UserName)
                ? captions.Replace(PostCaptionFormate.UserName, $"@{Username}")
                : captions.Replace(PostCaptionFormate.UserName, "");
            captions = captions.Contains(PostCaptionFormate.AccountUserName)
                ? captions.Replace(PostCaptionFormate.AccountUserName, $"@{DominatorAccountModel.UserName}")
                : captions.Replace(PostCaptionFormate.AccountUserName, "");
            return captions;
        }
        #endregion
    }
}