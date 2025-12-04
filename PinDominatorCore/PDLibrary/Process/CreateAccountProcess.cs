using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Process.JobLimits;
using DominatorHouseCore.Utility;
using DominatorUIUtility.ViewModel;
using PinDominatorCore.PDFactories;
using PinDominatorCore.PDLibrary.DAL;
using PinDominatorCore.PDModel;
using PinDominatorCore.Request;
using System;
using System.Collections.Generic;

namespace PinDominatorCore.PDLibrary.Process
{
    public class CreateAccountProcess : PdJobProcessInteracted<DominatorHouseCore.DatabaseHandler.PdTables.Accounts.CreateAccount>
    {
        private readonly IDbCampaignService _dbCampaignService;
        private IDominatorAccountViewModel dominatorAccountViewModel {  get; set; }
        public CreateAccountProcess(IProcessScopeModel processScopeModel, IDbAccountServiceScoped accountServiceScoped,
            IDbGlobalService globalService, IDbCampaignService dbCampaignService,
            IExecutionLimitsManager executionLimitsManager, IPinFunction pdFunc,
            IJobActivityConfigurationManager jobActivityConfigurationManager,
            IPdQueryScraperFactory queryScraperFactory, IPdHttpHelper pdhttpHelper, IPdLogInProcess pdLogInProcess)
            : base(processScopeModel, accountServiceScoped, globalService, executionLimitsManager, queryScraperFactory,
                pdhttpHelper, pdLogInProcess)
        {
            _dbCampaignService = dbCampaignService;
            dominatorAccountViewModel = dominatorAccountViewModel ?? InstanceProvider.GetInstance<IDominatorAccountViewModel>();
        }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            var jobProcessResult = new JobProcessResult();
            var accountInfo = (CreateAccountInfo)scrapeResult.ResultPost;
            GlobusLogHelper.log.Info(Log.StartedActivity, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                DominatorAccountModel.AccountBaseModel.UserName, ActivityType, accountInfo.Email);

            try
            {
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                var response = PinFunct.CreateAccount(accountInfo, DominatorAccountModel).Result;

                if (response != null && response.Success)
                {
                    GlobusLogHelper.log.Info(Log.ActivitySuccessful, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType, accountInfo.Email);

                    IncrementCounters();

                    AddEditPinDataToDataBase(accountInfo);

                    if(accountInfo.IsCheckedToAccountManager)
                        AddAccountToAccountManager(accountInfo);

                    jobProcessResult.IsProcessSuceessfull = true;

                    JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                }
                else
                {
                    GlobusLogHelper.log.Info(Log.ActivityFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType, response?.Issue?.Message);
                    jobProcessResult.IsProcessSuceessfull = false;
                    JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
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
                ex.DebugLog();
            }

            return jobProcessResult;
        }

        private void AddAccountToAccountManager(CreateAccountInfo accountInfo)
        {
            try
            {
                GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.UserName, ActivityType,
                    "Trying To Add Account Into Account Manager.....");
                var objDominatorAccountBaseModel = new DominatorAccountBaseModel();
                objDominatorAccountBaseModel.UserName =accountInfo.Email;
                objDominatorAccountBaseModel.Password = accountInfo.Password;
                objDominatorAccountBaseModel.AccountNetwork = SocialNetworks.Pinterest;
                var isIgOrTik = objDominatorAccountBaseModel.AccountNetwork == SocialNetworks.TikTok;
                var dictNetLasNum = new Dictionary<SocialNetworks, int>();
                var nickName = dominatorAccountViewModel.DefaultAccountNameFromModel(
                    dominatorAccountViewModel.LstDominatorAccountModel.BySocialNetwork(objDominatorAccountBaseModel.AccountNetwork),
                    ref dictNetLasNum, objDominatorAccountBaseModel.AccountNetwork);
                objDominatorAccountBaseModel.AccountName = nickName;
                dominatorAccountViewModel.AddSingleAccountInThread(objDominatorAccountBaseModel,
                    isIgOrTik,false,null,string.Empty,string.Empty);
                GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.UserName, ActivityType,
                    "Successfully Processed Account For Login And Account Update");
            }
            catch (Exception) { }
        }

        private void AddEditPinDataToDataBase(CreateAccountInfo accountInfo)
        {
            try
            {
                var jobActivityConfigurationManager =
                    InstanceProvider.GetInstance<IJobActivityConfigurationManager>();
                var moduleSetting = jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];
                if (moduleSetting == null)
                    return;
                if (moduleSetting.IsTemplateMadeByCampaignMode)
                    _dbCampaignService.Add(new DominatorHouseCore.DatabaseHandler.PdTables.Campaigns.CreateAccount
                    {
                        ActivityType = ActivityType.ToString(),
                        InteractionDate = DateTimeUtilities.GetEpochTime(),
                        Email = accountInfo.Email,
                        Password = accountInfo.Password,
                        Age = accountInfo.Age,
                        Gender = accountInfo.Gender
                    });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}
