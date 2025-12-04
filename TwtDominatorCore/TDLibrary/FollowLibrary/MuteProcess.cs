using System;
using CommonServiceLocator;
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
    public class MuteProcess : TdJobProcessInteracted<InteractedUsers>
    {
        private readonly IBlackWhiteListHandler _blackWhiteListHandler;
        private readonly IDbInsertionHelper _dbInsertionHelper;
        private readonly ITwitterFunctionFactory _twitterFunctionFactory;

        public MuteProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IDbGlobalService globalService,
            IExecutionLimitsManager executionLimitsManager, ITdQueryScraperFactory queryScraperFactory,
            IBlackWhiteListHandler blackWhiteListHandler, ITdHttpHelper tdHttpHelper, ITwtLogInProcess twtLogInProcess,
            ITwitterFunctionFactory twitterFunctionFactory, IDbInsertionHelper dbInsertionHelper)
            : base(processScopeModel, accountServiceScoped, globalService, executionLimitsManager, queryScraperFactory,
                tdHttpHelper, twtLogInProcess)
        {
            _blackWhiteListHandler = blackWhiteListHandler;
            _twitterFunctionFactory = twitterFunctionFactory;
            _dbInsertionHelper = dbInsertionHelper;
            MuteModel = processScopeModel.GetActivitySettingsAs<MuteModel>();
        }

        private ITwitterFunctions _twitterFunctions => _twitterFunctionFactory.TwitterFunctions;
        private ManageUniqueProcess ManageUniqueProcess { get; set; }

        public MuteModel MuteModel { get; set; }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            var jobProcessResult = new JobProcessResult();
            try
            {
                var twitterUser = (TwitterUser) scrapeResult.ResultUser;

                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                #region  GetModuleSetting

                var jobActivityConfigurationManager =
                    InstanceProvider.GetInstance<IJobActivityConfigurationManager>();

                var moduleModeSetting =
                    jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];

                if (moduleModeSetting == null) return jobProcessResult;

                #endregion

                #region Unique user to follow check

                if (moduleModeSetting.IsTemplateMadeByCampaignMode && MuteModel.IsCampaignWiseUniqueChecked)
                {
                    ManageUniqueProcess = new ManageUniqueProcess(CampaignId, ActivityType, DominatorAccountModel);

                    if (ManageUniqueProcess.IsActivityDoneWithThisUserId(twitterUser.UserId) ||
                        !ManageUniqueProcess.IsUniqueUser(twitterUser))
                        return jobProcessResult;
                }

                #endregion

                var muteResponse =
                    _twitterFunctions.Mute(DominatorAccountModel, twitterUser.UserId, twitterUser.Username);

                if (muteResponse.Success)
                {
                    _blackWhiteListHandler.AddToBlackList(twitterUser.UserId, twitterUser.Username);

                    GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                        DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.UserName,
                        ActivityType.Mute, TdUtility.GetProfileUrl(twitterUser.Username));

                    JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                    IncrementCounters();

                    _dbInsertionHelper.AddInteractedUserDetailsInAccountDb(twitterUser, ActivityType.ToString(),
                        ActivityType.ToString(), scrapeResult);

                    /// Updated from normal mode

                    moduleModeSetting =
                        jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];

                    if (moduleModeSetting == null) return jobProcessResult;

                    if (moduleModeSetting.IsTemplateMadeByCampaignMode)
                        _dbInsertionHelper.AddInteractedUserDetailsInCampaignDb(twitterUser, ActivityType.ToString(),
                            scrapeResult);

                    jobProcessResult.IsProcessSuceessfull = true;
                }
                else
                {
                    GlobusLogHelper.log.Info(Log.ActivityFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.UserName, ActivityType.Mute, TdUtility.GetProfileUrl(twitterUser.Username) + "==>" +muteResponse.Issue?.Message);
                    jobProcessResult.IsProcessSuceessfull = false;
                }

                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                DelayBeforeNextActivity();
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
    }
}