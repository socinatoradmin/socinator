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
    public class DeletePinProcess : PdJobProcessInteracted<InteractedPosts>
    {
        private readonly IDbCampaignService _dbCampaignService;
        public DeletePinProcess(IProcessScopeModel processScopeModel,
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
                DeletePinResponseHandler deletePinResponse = null;
                if (DominatorAccountModel.IsRunProcessThroughBrowser)
                    deletePinResponse = PinFunct.DeletePin(pin.PinId, DominatorAccountModel);
                else
                    deletePinResponse = PinFunct.DeletePin(pin.PinId, DominatorAccountModel);

                if (deletePinResponse != null && deletePinResponse.Success)
                {
                    GlobusLogHelper.log.Info(Log.ActivitySuccessful, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType, pin.PinId);

                    IncrementCounters();

                    AfterDeletePinAddDataToDataBase(scrapeResult);
                    jobProcessResult.IsProcessSuceessfull = true;
                }
                else
                {
                    GlobusLogHelper.log.Info(Log.ActivityFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                        deletePinResponse?.Issue?.Message);

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

        private void AfterDeletePinAddDataToDataBase(ScrapeResultNew scrapeResult)
        {
            try
            {
                var pin = (PinterestPin)scrapeResult.ResultPost;
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
                        QueryType = scrapeResult.QueryInfo.QueryType,
                        Query = scrapeResult.QueryInfo.QueryValue,
                        Username = pin.User.Username,
                        SourceBoard = pin.BoardUrl,
                        SourceBoardName = pin.BoardName,
                        CommentCount = pin.CommentCount,
                        MediaType = pin.MediaType,
                        PinDescription = pin.Description,
                        PinId = pin.PinId,
                        PinWebUrl = pin.PinWebUrl,
                        UserId = pin.User.UserId,
                        TryCount = pin.NoOfTried,
                        MediaString = pin.MediaString,
                        SinAccId = DominatorAccountModel.AccountBaseModel.UserId,
                        SinAccUsername = DominatorAccountModel.AccountBaseModel.UserName
                    });
                DbAccountService.Add(new InteractedPosts
                {
                    OperationType = ActivityType.ToString(),
                    InteractionDate = DateTimeUtilities.GetEpochTime(),
                    QueryType = scrapeResult.QueryInfo.QueryType,
                    Query = scrapeResult.QueryInfo.QueryValue,
                    Username = pin.User.Username,
                    SourceBoard = pin.BoardUrl,
                    SourceBoardName = pin.BoardName,
                    CommentCount = pin.CommentCount,
                    MediaType = pin.MediaType,
                    PinDescription = pin.Description,
                    PinId = pin.PinId,
                    PinWebUrl = pin.PinWebUrl,
                    UserId = pin.User.UserId,
                    TryCount = pin.NoOfTried,
                    MediaString = pin.MediaString
                });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}