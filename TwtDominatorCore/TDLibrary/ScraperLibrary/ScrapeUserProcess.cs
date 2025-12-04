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
    public class ScrapeUserProcess : TdJobProcessInteracted<InteractedUsers>
    {
        private readonly IDbInsertionHelper _dbInsertionHelper;
        private readonly IJobActivityConfigurationManager _jobActivityConfigurationManager;
        private readonly ITwitterFunctionFactory _twtFuncFactory;

        public ScrapeUserProcess(IProcessScopeModel processScopeModel,
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
            ScrapeUserModel = processScopeModel.GetActivitySettingsAs<ScrapeUserModel>();
        }

        private ScrapeUserModel ScrapeUserModel { get; }

        // changement in to assign functions factory beacuse of NewUi error to slove this we use this way.
        private ITwitterFunctions _twtFunc => _twtFuncFactory.TwitterFunctions;

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            var jobProcessResult = new JobProcessResult();
            try
            {
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                var twitterUser = new TwitterUser();
                try
                {
                    var type = scrapeResult.ResultUser.GetType().Name;

                    if (type.Equals("TagDetails"))
                        twitterUser = _twtFunc.GetUserDetails(DominatorAccountModel, scrapeResult.ResultUser.Username,
                                scrapeResult.QueryInfo.QueryType)
                            .UserDetail;
                    else if (type.Equals("TwitterUser")) twitterUser = scrapeResult.ResultUser as TwitterUser;
                }
                catch (OperationCanceledException)
                {
                    throw new OperationCanceledException();
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                GlobusLogHelper.log.Info(Log.ActivitySuccessful, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.UserName, ActivityType.UserScraper, TdUtility.GetProfileUrl(twitterUser.Username));

                IncrementCounters();

                _dbInsertionHelper.AddInteractedUserDetailsInAccountDb(twitterUser, ActivityType.UserScraper.ToString(),
                    ActivityType.UserScraper.ToString(), scrapeResult);

                #region  GetModuleSetting

                var moduleModeSetting =
                    _jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];

                if (moduleModeSetting == null) return jobProcessResult;

                #endregion

                if (moduleModeSetting.IsTemplateMadeByCampaignMode)
                    _dbInsertionHelper.AddInteractedUserDetailsInCampaignDb(twitterUser,
                        ActivityType.UserScraper.ToString(), scrapeResult);

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
                        $"TwtDominator : [Account: {DominatorAccountModel.AccountBaseModel.UserName}] => (Module => {ActivityType.ToString()})");
            }

            return jobProcessResult;
        }
    }
}