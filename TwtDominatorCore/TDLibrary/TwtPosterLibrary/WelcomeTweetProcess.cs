using System;
using System.Collections.Generic;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.TdTables.Accounts;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Process.JobLimits;
using DominatorHouseCore.Utility;
using TwtDominatorCore.Database;
using TwtDominatorCore.Factories;
using TwtDominatorCore.Requests;
using TwtDominatorCore.TDFactories;
using TwtDominatorCore.TDLibrary.GeneralLibrary.DAL;
using TwtDominatorCore.TDModels;
using TwtDominatorCore.TDUtility;

namespace TwtDominatorCore.TDLibrary
{
    public class WelcomeTweetProcess : TdJobProcessInteracted<InteractedPosts>
    {
        #region Construction

        public WelcomeTweetProcess(IProcessScopeModel processScopeModel,
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
            WelcomeTweetModel = processScopeModel.GetActivitySettingsAs<WelcomeTweetModel>();
        }

        #endregion

        #region Private Properties

        public WelcomeTweetModel WelcomeTweetModel { get; set; }

        #endregion

        // changement in to assign functions factory beacuse of NewUi error to slove this we use this way.
        private ITwitterFunctions _twtFunc => _twtFuncFactory.TwitterFunctions;

        #region Public Methods

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            var jobProcessResult = new JobProcessResult();
            try
            {
                var twtUser = (TwitterUser) scrapeResult.ResultUser;
                var filePath = new List<string>();

                string currentCaption;

                // if format is not given by default username will be at starting in tweet
                if (WelcomeTweetModel.WelcomeMessageText.Contains("{username}"))
                    currentCaption = WelcomeTweetModel.WelcomeMessageText.Replace("{username}", $"@{twtUser.Username}");
                else
                    currentCaption = $"@{twtUser.Username} {WelcomeTweetModel.WelcomeMessageText}";

                var tagForProcess = new TagDetails
                {
                    Caption = currentCaption,
                    UserId = twtUser.UserId,
                    Username = twtUser.Username
                };

                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                var twtResponse = _twtFunc.Tweet(DominatorAccountModel, tagForProcess.Caption,
                    JobCancellationTokenSource.Token, "", "", "", ActivityType, tagForProcess.IsTweetContainedVideo,
                    filePath);
                var Tweeturl = TdUtility.GetTweetUrl(tagForProcess.Username, tagForProcess.Id);
                if (twtResponse.Success)
                {
                    tagForProcess.Id = twtResponse.TweetId;

                    IncrementCounters();

                    _dbInsertionHelper.AddInteractedTweetDetailInAccountDb(tagForProcess, ActivityType.ToString(),
                        ActivityType.ToString(), scrapeResult);

                    #region  GetModuleSetting

                    var moduleModeSetting =
                        _jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];

                    if (moduleModeSetting == null) return jobProcessResult;

                    #endregion

                    if (moduleModeSetting.IsTemplateMadeByCampaignMode)
                        _dbInsertionHelper.AddInteractedTweetDetailInCampaignDb(tagForProcess, ActivityType.ToString(),
                            scrapeResult);

                    // Updating status after tweeting welcome tweet 
                    // Removing its only for new followers
                    // ObjDbInsertionHelper.UpdateMessageStatusOfFriendship(TwtUser.UserId);

                    _dbInsertionHelper.AddFeedsInfo(
                        new TagDetails
                        {
                            Id = twtResponse.TweetId,
                            Caption = tagForProcess.Caption,
                            Code = string.Join(",", filePath),
                            TweetedTimeStamp = DateTime.UtcNow.GetCurrentEpochTime()
                        }, true);

                    jobProcessResult.IsProcessSuceessfull = true;

                    JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                    GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                        DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.UserName,
                        ActivityType.WelcomeTweet, Tweeturl);
                }
                else
                {
                    JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    GlobusLogHelper.log.Info(Log.ActivityFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.UserName, ActivityType.WelcomeTweet, Tweeturl + " ==> "+
                        twtResponse.Issue?.Message);
                    jobProcessResult.IsProcessSuceessfull = false;
                }

                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                DelayBeforeNextActivity();
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                if (DominatorAccountModel?.AccountBaseModel?.UserName != null)
                    ex.DebugLog(
                        $"TwtDominator : [Account: {DominatorAccountModel.AccountBaseModel.UserName}] => Error : {ex.Message}. StackTrace => {ex.StackTrace}  (Module => {ActivityType.ToString()})");
            }

            return jobProcessResult;
        }

        #endregion

        #region Private Fields

        private readonly ITwitterFunctionFactory _twtFuncFactory;
        private readonly IJobActivityConfigurationManager _jobActivityConfigurationManager;
        private readonly IDbInsertionHelper _dbInsertionHelper;

        #endregion
    }
}