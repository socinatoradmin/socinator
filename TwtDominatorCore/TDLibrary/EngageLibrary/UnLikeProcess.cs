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
using TwtDominatorCore.TDFactories;
using TwtDominatorCore.TDLibrary.GeneralLibrary.DAL;
using TwtDominatorCore.TDModels;
using TwtDominatorCore.TDUtility;

namespace TwtDominatorCore.TDLibrary
{
    public class UnLikeProcess : TdJobProcessInteracted<InteractedPosts>
    {
        #region Construction

        public UnLikeProcess(IProcessScopeModel processScopeModel,
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
            UnLikeModel = processScopeModel.GetActivitySettingsAs<UnLikeModel>();
        }

        #endregion

        #region Public Properties

        public UnLikeModel UnLikeModel { get; set; }

        #endregion

        private ITwitterFunctions _twtFunc => _twtFuncFactory.TwitterFunctions;

        #region Public Methods

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            var jobProcessResult = new JobProcessResult();
            try
            {
                var tagForProcess = (TagDetails) scrapeResult.ResultUser;

                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                var unLikeResponse = _twtFunc.Unlike(DominatorAccountModel, tagForProcess.Id, tagForProcess.Username);

                if (unLikeResponse.Success)
                {
                    IncrementCounters();

                    _dbInsertionHelper.AddInteractedTweetDetailInAccountDb(tagForProcess, ActivityType.ToString(),
                        ActivityType.ToString(), scrapeResult);

                    // Updated from normal mode

                    #region  GetModuleSetting

                    var moduleModeSetting =
                        _jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];

                    if (moduleModeSetting == null) return jobProcessResult;

                    #endregion

                    if (moduleModeSetting.IsTemplateMadeByCampaignMode)
                        _dbInsertionHelper.AddInteractedTweetDetailInCampaignDb(tagForProcess, ActivityType.ToString(),
                            scrapeResult);

                    JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                        DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.UserName,
                        ActivityType.Unlike, TdUtility.GetTweetUrl(tagForProcess.Username, tagForProcess.Id));

                    jobProcessResult.IsProcessSuceessfull = true;
                }
                else
                {
                    GlobusLogHelper.log.Info(Log.ActivityFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.UserName, ActivityType.Unlike, TdUtility.GetTweetUrl(tagForProcess.Username, tagForProcess.Id),
                        unLikeResponse.Issue?.Message);
                    jobProcessResult.IsProcessSuceessfull = false;
                }

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

        #endregion

        #region Private Fields

        private readonly ITwitterFunctionFactory _twtFuncFactory;
        private readonly IJobActivityConfigurationManager _jobActivityConfigurationManager;
        private readonly IDbInsertionHelper _dbInsertionHelper;

        #endregion
    }
}