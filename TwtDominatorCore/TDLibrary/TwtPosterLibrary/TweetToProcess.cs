using System;
using System.Collections.Generic;
using System.Linq;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.TdTables.Accounts;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Process.JobLimits;
using DominatorHouseCore.Utility;
using TwtDominatorCore.Database;
using TwtDominatorCore.Factories;
using TwtDominatorCore.Requests;
using TwtDominatorCore.TDFactories;
using TwtDominatorCore.TDLibrary.GeneralLibrary;
using TwtDominatorCore.TDLibrary.GeneralLibrary.DAL;
using TwtDominatorCore.TDLibrary.GeneralLibrary.Subprocesses;
using TwtDominatorCore.TDModels;
using TwtDominatorCore.TDUtility;

namespace TwtDominatorCore.TDLibrary.TwtPosterLibrary
{
    public class TweetToProcess : TdJobProcessInteracted<InteractedPosts>
    {
        #region Construction

        public TweetToProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IDbGlobalService globalService,
            IExecutionLimitsManager executionLimitsManager, ITwitterFunctionFactory twitterFunctionFactory,
            IJobActivityConfigurationManager jobActivityConfigurationManager,
            ITdQueryScraperFactory queryScraperFactory, ITdHttpHelper tdHttpHelper, ITwtLogInProcess twtLogInProcess,
            IDbInsertionHelper dbInsertionHelper, IEnumerable<ISubprocess<TweetToModel>> subprocesses)
            : base(processScopeModel, accountServiceScoped, globalService, executionLimitsManager, queryScraperFactory,
                tdHttpHelper, twtLogInProcess)
        {
            _twtFuncFactory = twitterFunctionFactory;
            _jobActivityConfigurationManager = jobActivityConfigurationManager;
            _dbInsertionHelper = dbInsertionHelper;
            _subprocesses = subprocesses;
            TweetToModel = processScopeModel.GetActivitySettingsAs<TweetToModel>();
        }

        #endregion

        #region Public Properties

        public TweetToModel TweetToModel { get; set; }

        #endregion

        // changement in to assign functions factory beacuse of NewUi error to slove this we use this way.
        private ITwitterFunctions _twtFunc => _twtFuncFactory.TwitterFunctions;

        #region Public Methods

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            var jobProcessResult = new JobProcessResult();
            try
            {
                var twitterUserProcess = (TwitterUser) scrapeResult.ResultUser;
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                var tagForProcess = new TagDetails
                {
                    UserId = twitterUserProcess.UserId,
                    TweetedTimeStamp = DateTime.Now.GetCurrentEpochTime(),
                    Username = twitterUserProcess.Username
                };
                var tweetBody = GetTweetBody(TweetToModel, twitterUserProcess);
                if (TweetToModel.IsSpintax)
                    tweetBody = SpintaxParser.GenerateMultilineMessageWithSpintext(tweetBody);

                var isContainVideo = !string.IsNullOrEmpty(TweetToModel.VideoFilePath?.Trim());

                var newMediaList = GetMediaList(isContainVideo, TweetToModel);

                var tweetResponse = _twtFunc.Tweet(DominatorAccountModel, tweetBody, JobCancellationTokenSource.Token,
                    tagForProcess.Id, tagForProcess.Username, scrapeResult.QueryInfo.QueryType, ActivityType,
                    isContainVideo, newMediaList);
                var Tweeturl = TdUtility.GetProfileUrl(tagForProcess.Username);
                if (tweetResponse.Success)
                {
                    IncrementCounters();
                    tagForProcess.Id = tweetResponse.TweetId;
                    var Tweetedurl = TdUtility.GetTweetUrl(string.IsNullOrEmpty(DominatorAccountModel.UserName) ? tagForProcess.Username:DominatorAccountModel.UserName, tagForProcess.Id);
                    tagForProcess.Code = AllMediaPath();
                    tagForProcess.Caption = tweetBody;

                    InsertIntoDb(tagForProcess, scrapeResult, twitterUserProcess);

                    GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                        DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.UserName,
                        ActivityType, $"{Tweeturl} at => {Tweetedurl}");

                    var moduleModeSetting =
                        _jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];

                    if (moduleModeSetting == null) return jobProcessResult;

                    if (moduleModeSetting.IsTemplateMadeByCampaignMode)
                        _dbInsertionHelper.AddInteractedTweetDetailInCampaignDb(tagForProcess, ActivityType.ToString(),
                            scrapeResult);

                    jobProcessResult.IsProcessSuceessfull = true;
                }
                else
                {
                    GlobusLogHelper.log.Info(Log.ActivityFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.UserName, ActivityType, Tweeturl + " ==> " + tweetResponse.Issue?.Message);
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

        #region Private fields

        private readonly IEnumerable<ISubprocess<TweetToModel>> _subprocesses;

        private readonly ITwitterFunctionFactory _twtFuncFactory;
        private readonly IJobActivityConfigurationManager _jobActivityConfigurationManager;
        private readonly IDbInsertionHelper _dbInsertionHelper;

        #endregion

        #region Private Methods

        private string AllMediaPath()
        {
            var mediaPath = string.Empty;

            try
            {
                if (TweetToModel.MediaList.Count != 0)
                    TweetToModel.MediaList.ForEach(x => mediaPath += x + "\n");
                else if (!string.IsNullOrEmpty(TweetToModel.VideoFilePath)) mediaPath = TweetToModel.VideoFilePath;
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }

            return mediaPath;
        }

        private void InsertIntoDb(TagDetails tagForProcess, ScrapeResultNew scrapeResult,
            TwitterUser twitterUserProcess)
        {
            _dbInsertionHelper.AddFeedsInfo(tagForProcess, true);
            _dbInsertionHelper.AddInteractedTweetDetailInAccountDb(tagForProcess, ActivityType.ToString(),
                ActivityType.ToString(), scrapeResult);
            _dbInsertionHelper.AddInteractedUserDetailsInCampaignDb(twitterUserProcess, ActivityType.ToString(),
                scrapeResult);
            _dbInsertionHelper.AddInteractedUserDetailsInAccountDb(twitterUserProcess, ActivityType.ToString(),
                ActivityType.ToString(), scrapeResult);
        }

        private List<string> GetMediaList(bool isContainVideo, TweetToModel tweetToModel)
        {
            var newMediaList = new List<string>();

            if (isContainVideo)
                newMediaList.Add(tweetToModel.VideoFilePath);
            else if (tweetToModel.MediaList.Any()) newMediaList = tweetToModel.MediaList.ToList();

            return newMediaList;
        }

        private string GetTweetBody(TweetToModel tweetToModel, TwitterUser twitterUserProcess)
        {
            return tweetToModel.TweetToText.Contains("{username}")
                ? tweetToModel.TweetToText.Replace("{username}", $"@{twitterUserProcess.Username}")
                : $"@{twitterUserProcess.Username} {tweetToModel.TweetToText}";
        }

        #endregion
    }
}