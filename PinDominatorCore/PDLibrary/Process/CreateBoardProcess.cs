using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.PdTables.Accounts;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Process.JobLimits;
using DominatorHouseCore.Utility;
using Newtonsoft.Json;
using PinDominatorCore.PDFactories;
using PinDominatorCore.PDLibrary.DAL;
using PinDominatorCore.PDModel;
using PinDominatorCore.Request;
using PinDominatorCore.Response;
using System;

namespace PinDominatorCore.PDLibrary.Process
{
    public class CreateBoardProcess : PdJobProcessInteracted<InteractedBoards>
    {
        private readonly IDbCampaignService _dbCampaignService;
        private int _activityFailedCount = 1;
        private BoardModel BoardModel { get; set; }

        public CreateBoardProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IDbGlobalService globalService, IDbCampaignService dbCampaignService,
            IExecutionLimitsManager executionLimitsManager, 
            IPdQueryScraperFactory queryScraperFactory, IPdHttpHelper pdhttpHelper, IPdLogInProcess pdLogInProcess)
            : base(processScopeModel, accountServiceScoped, globalService, executionLimitsManager, queryScraperFactory,
                pdhttpHelper, pdLogInProcess)
        {
            _dbCampaignService = dbCampaignService;
            BoardModel = JsonConvert.DeserializeObject<BoardModel>(TemplateModel.ActivitySettings);
        }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            var jobProcessResult = new JobProcessResult();
            var info = (BoardInfo)scrapeResult.ResultPost;
            GlobusLogHelper.log.Info(Log.StartedActivity, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                DominatorAccountModel.AccountBaseModel.UserName, ActivityType, info.BoardName);
            try
            {
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                BoardResponse response;
                if (DominatorAccountModel.IsRunProcessThroughBrowser)
                    response = PdLogInProcess.BrowserManager.CreateBoard(DominatorAccountModel, info, JobCancellationTokenSource);
                else
                    response = PinFunct.CreateBoard(info, DominatorAccountModel);
                if (PinFunct != null)
                    response=PinFunct.CreateBoardSection(response,info,DominatorAccountModel);
                if (response != null && response.Success)
                {
                    GlobusLogHelper.log.Info(Log.ActivitySuccessful, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType, info.BoardName);

                    IncrementCounters();

                    AddBoardDataToDataBase(response, info);
                    jobProcessResult.IsProcessSuceessfull = true;
                }
                else
                {
                    GlobusLogHelper.log.Info(Log.ActivityFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType, response?.Issue?.Message);

                    //Reschedule if action block
                    if (BoardModel.IsChkStopActivityAfterXXFailed && _activityFailedCount++ == BoardModel.ActivityFailedCount)
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                            $"{"LangKeyStopActivityAfter".FromResourceDictionary()} {_activityFailedCount - 1} " +
                            $"{"LangKeyFailedAttemptFor".FromResourceDictionary()} {BoardModel.FailedActivityReschedule} " +
                            $"{"LangKeyHour".FromResourceDictionary()}");

                        StopAndRescheduleJob(BoardModel.FailedActivityReschedule);
                    }

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

        private void AddBoardDataToDataBase(BoardResponse boardResponse, BoardInfo info)
        {
            try
            {
                var jobActivityConfigurationManager =
                    InstanceProvider.GetInstance<IJobActivityConfigurationManager>();
                var moduleSetting = jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];
                if (moduleSetting == null)
                    return;
                var section = JsonConvert.SerializeObject(boardResponse.BoardSections);
                if (moduleSetting.IsTemplateMadeByCampaignMode)
                    _dbCampaignService.Add(new DominatorHouseCore.DatabaseHandler.PdTables.Campaigns.InteractedBoards
                    {
                        OperationType = ActivityType,
                        InteractionDate = DateTimeUtilities.GetEpochTime(),
                        QueryType = info.BoardName,
                        BoardId = boardResponse.BoardId,
                        BoardName = info.BoardName,
                        Category = info.Category,
                        BoardDescription = info.BoardDescription,
                        SinAccId = DominatorAccountModel.AccountBaseModel.UserId,
                        SinAccUsername = DominatorAccountModel.AccountBaseModel.UserName,
                        BoardSection = section
                    });
                DbAccountService.Add(new InteractedBoards
                {
                    OperationType = ActivityType,
                    InteractionDate = DateTimeUtilities.GetEpochTime(),
                    QueryType = info.BoardName,
                    BoardId = boardResponse.BoardId,
                    BoardName = info.BoardName,
                    Category = info.Category,
                    BoardDescription = info.BoardDescription,
                    UserId = DominatorAccountModel.AccountBaseModel.UserId,
                    Username = DominatorAccountModel.AccountBaseModel.UserName,
                    BoardSection=section
                });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}