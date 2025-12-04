using System;
using System.Text.RegularExpressions;
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
    public class DownloadProcess : TdJobProcessInteracted<InteractedPosts>
    {
        #region Construction

        public DownloadProcess(IProcessScopeModel processScopeModel,
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
            ScrapeTweetModel = processScopeModel.GetActivitySettingsAs<ScrapeTweetModel>();
        }

        #endregion

        #region Private Properties

        private ScrapeTweetModel ScrapeTweetModel { get; }

        #endregion

        // changement in to assign functions factory beacuse of NewUi error to slove this we use this way.
        private ITwitterFunctions _twtFunc => _twtFuncFactory.TwitterFunctions;

        #region Public Methods

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            var jobProcessResult = new JobProcessResult();
            try
            {
                var filename = Regex.Replace(
                    scrapeResult.QueryInfo.QueryValue,
                    "[\\/:*?<>|\"]",
                    "-");

                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                var tweetDetails = (TagDetails) scrapeResult.ResultUser;

                if ((ScrapeTweetModel.DownloadSetting.IsChkCategoryAll ||
                     ScrapeTweetModel.DownloadSetting.IsChkCategoryVideo) && tweetDetails.IsTweetContainedVideo)
                {
                    var url = _twtFunc.DownloadVideoUsingThirdParty(tweetDetails.Id,
                        tweetDetails.Username, ScrapeTweetModel.DownloadFolderPath, filename + "-" + tweetDetails.Id,
                        ScrapeTweetModel.DownloadSetting.IsChkQualityHigh ? 1 : 0);

                    if (url.Count == 0) return jobProcessResult;
                }
                else if ((ScrapeTweetModel.DownloadSetting.IsChkCategoryAll ||
                          ScrapeTweetModel.DownloadSetting.IsChkCategoryImage) &&
                         !string.IsNullOrEmpty(tweetDetails.Code))
                {
                    TdUtility.DownloadFileFromTwitter(tweetDetails.Code, ".jpg", ScrapeTweetModel.DownloadFolderPath,
                        filename + "-" + tweetDetails.Id);
                }

                GlobusLogHelper.log.Info(Log.ActivitySuccessful, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.UserName, ActivityType.DownloadScraper, tweetDetails.Id);

                IncrementCounters();

                var moduleModeSetting =
                    _jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];

                if (moduleModeSetting == null) return jobProcessResult;

                _dbInsertionHelper.AddInteractedTweetDetailInAccountDb(tweetDetails, ActivityType.ToString(),
                    ActivityType.ToString(), scrapeResult);

                if (moduleModeSetting.IsTemplateMadeByCampaignMode)
                    _dbInsertionHelper.AddInteractedTweetDetailInCampaignDb(tweetDetails, ActivityType.ToString(),
                        scrapeResult);

                jobProcessResult.IsProcessSuceessfull = true;
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
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

        #endregion

        #region Private Fields

        private readonly ITwitterFunctionFactory _twtFuncFactory;
        private readonly IJobActivityConfigurationManager _jobActivityConfigurationManager;
        private readonly IDbInsertionHelper _dbInsertionHelper;

        #endregion
    }
}