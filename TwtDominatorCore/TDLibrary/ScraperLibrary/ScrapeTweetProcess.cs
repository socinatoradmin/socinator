using System;
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
using TwtDominatorCore.TDLibrary.GeneralLibrary.DAL;
using TwtDominatorCore.TDModels;
using TwtDominatorCore.TDUtility;

namespace TwtDominatorCore.TDLibrary
{
    internal class ScrapeTweetProcess : TdJobProcessInteracted<InteractedPosts>
    {
        private readonly IDbInsertionHelper _dbInsertionHelper;
        private readonly IJobActivityConfigurationManager _jobActivityConfigurationManager;

        public ScrapeTweetProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IDbGlobalService globalService,
            IExecutionLimitsManager executionLimitsManager,
            IJobActivityConfigurationManager jobActivityConfigurationManager,
            ITdQueryScraperFactory queryScraperFactory, ITdHttpHelper tdHttpHelper, ITwtLogInProcess twtLogInProcess,
            IDbInsertionHelper dbInsertionHelper)
            : base(processScopeModel, accountServiceScoped, globalService, executionLimitsManager, queryScraperFactory,
                tdHttpHelper, twtLogInProcess)
        {
            _jobActivityConfigurationManager = jobActivityConfigurationManager;
            _dbInsertionHelper = dbInsertionHelper;
        }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            var jobProcessResult = new JobProcessResult();
            try
            {
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                var tweetDetails = (TagDetails) scrapeResult.ResultUser;

                GlobusLogHelper.log.Info(Log.ActivitySuccessful, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.UserName, ActivityType.TweetScraper, TdUtility.GetTweetUrl(tweetDetails.Username, tweetDetails.Id));

                IncrementCounters();

                _dbInsertionHelper.AddInteractedTweetDetailInAccountDb(tweetDetails, ActivityType.ToString(),
                    ActivityType.ToString(), scrapeResult);

                /// Updated from normal mode

                #region  GetModuleSetting

                var moduleModeSetting =
                    _jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];

                if (moduleModeSetting == null) return jobProcessResult;

                #endregion

                if (moduleModeSetting.IsTemplateMadeByCampaignMode)
                    _dbInsertionHelper.AddInteractedTweetDetailInCampaignDb(tweetDetails, ActivityType.ToString(),
                        scrapeResult);

                jobProcessResult.IsProcessSuceessfull = true;

                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
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
    }
}