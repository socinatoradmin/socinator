using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.PdTables.Accounts;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Process.JobLimits;
using DominatorHouseCore.Utility;
using PinDominatorCore.PDFactories;
using PinDominatorCore.PDLibrary.DAL;
using PinDominatorCore.PDModel;
using PinDominatorCore.Request;
using PinDominatorCore.Response;
using System;

namespace PinDominatorCore.PDLibrary.Process
{
    public class EditPinProcess : PdJobProcessInteracted<InteractedPosts>
    {
        private readonly IDbCampaignService _dbCampaignService;
        public EditPinProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IDbGlobalService globalService, IDbCampaignService dbCampaignService,
            IExecutionLimitsManager executionLimitsManager, 
            IPdQueryScraperFactory queryScraperFactory, IPdHttpHelper pdhttpHelper, IPdLogInProcess pdLogInProcess)
            : base(processScopeModel, accountServiceScoped, globalService, executionLimitsManager, queryScraperFactory,
                pdhttpHelper, pdLogInProcess)
        {
            _dbCampaignService = dbCampaignService;
        }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            var jobProcessResult = new JobProcessResult();
            var pin = (PinterestPin)scrapeResult.ResultPost;
            GlobusLogHelper.log.Info(Log.StartedActivity, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                DominatorAccountModel.AccountBaseModel.UserName, ActivityType, pin.PinId);

            try
            {
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                RepostPinResponseHandler response;
                if (DominatorAccountModel.IsRunProcessThroughBrowser)
                    response = PdLogInProcess.BrowserManager.EditPin(DominatorAccountModel, pin, JobCancellationTokenSource);
                else
                    response = PinFunct.UpdatePin(pin, DominatorAccountModel);

                if (response != null && response.Success)
                {
                    GlobusLogHelper.log.Info(Log.ActivitySuccessful, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType, pin.PinId);

                    IncrementCounters();

                    AddEditPinDataToDataBase(response, pin);
                    jobProcessResult.IsProcessSuceessfull = true;
                }
                else
                {
                    GlobusLogHelper.log.Info(Log.ActivityFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType, response?.Issue?.Message);

                    jobProcessResult.IsProcessSuceessfull = false;
                }

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

        private void AddEditPinDataToDataBase(RepostPinResponseHandler pinResponse, PinterestPin pin)
        {
            try
            {
                var jobActivityConfigurationManager =
                    InstanceProvider.GetInstance<IJobActivityConfigurationManager>();
                var moduleSetting = jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];
                if (moduleSetting == null)
                    return;
                if (moduleSetting.IsTemplateMadeByCampaignMode)
                    _dbCampaignService.Add(new DominatorHouseCore.DatabaseHandler.PdTables.Campaigns.InteractedPosts
                    {
                        OperationType = ActivityType.ToString(),
                        InteractionDate = DateTimeUtilities.GetEpochTime(),
                        Username = pin.User.Username,
                        PinDescription = pin.Description,
                        PinId = pin.PinId,
                        CommentCount = pin.CommentCount,
                        MediaString = pin.MediaString,
                        PinWebUrl = pin.PinWebUrl,
                        TryCount = pin.NoOfTried,
                        MediaType = pin.MediaType,
                        DestinationBoard = pin.BoardUrl,
                        SourceBoardName=pin.BoardName,
                        SinAccId = DominatorAccountModel.AccountBaseModel.UserId,
                        UserId= pin.User.UserId,
                        SinAccUsername = DominatorAccountModel.AccountBaseModel.UserName
                    });
                DbAccountService.Add(new InteractedPosts
                {
                    OperationType = ActivityType.ToString(),
                    InteractionDate = DateTimeUtilities.GetEpochTime(),
                    PinDescription = pin.Description,
                    PinId = pin.PinId,
                    CommentCount = pin.CommentCount,
                    MediaString = pin.MediaString,
                    PinWebUrl = pin.PinWebUrl,
                    TryCount = pin.NoOfTried,
                    MediaType = pin.MediaType,
                    UserId = pin.User.UserId,
                    Username = pin.User.Username
                });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}