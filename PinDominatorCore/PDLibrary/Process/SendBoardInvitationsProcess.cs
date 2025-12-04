using System;
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

namespace PinDominatorCore.PDLibrary.Process
{
    public class SendBoardInvitationsProcess : PdJobProcessInteracted<InteractedBoards>
    {
        private SendBoardInvitationModel SendBoardInvitationModel { get; }
        private int _activityFailedCount = 1;

        public SendBoardInvitationsProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped,
            IDbGlobalService globalService, IExecutionLimitsManager executionLimitsManager,
            IPdQueryScraperFactory queryScraperFactory,
            IPdHttpHelper qdHttpHelper, IPdLogInProcess qdLogInProcess) : base(processScopeModel, accountServiceScoped,
            globalService,
            executionLimitsManager, queryScraperFactory, qdHttpHelper, qdLogInProcess)
        {
            SendBoardInvitationModel = JsonConvert.DeserializeObject<SendBoardInvitationModel>(TemplateModel.ActivitySettings);
        }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            var jobProcessResult = new JobProcessResult();
            var board = (PinterestBoard) scrapeResult.ResultPost;
            GlobusLogHelper.log.Info(Log.StartedActivity, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                DominatorAccountModel.AccountBaseModel.UserName, ActivityType, board.BoardUrl);

            try
            {
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                SendBoardInvitationResponseHandler response;

                if (DominatorAccountModel.IsRunProcessThroughBrowser)
                    response = PdLogInProcess.BrowserManager.SendBoardInvitation(DominatorAccountModel, board, JobCancellationTokenSource);
                else
                    response = PinFunct.SendBoardInvitation(board, DominatorAccountModel);

                if (response != null && response.Success)
                {
                    JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                    GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                        DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType, board.BoardUrl);

                    IncrementCounters();

                    AddSendBoardInvitationDataToDataBase(scrapeResult);
                    jobProcessResult.IsProcessSuceessfull = true;
                }
                else if (!response.Success&&response.Issue.Message.Contains("User Already Invited"))
                {
                    GlobusLogHelper.log.Info(Log.ActivityFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType, response?.Issue?.Message);
                }
                else
                {
                    GlobusLogHelper.log.Info(Log.ActivityFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType, response?.Issue?.Message);

                    //Reschedule if action block
                    if (SendBoardInvitationModel.IsChkStopActivityAfterXXFailed && _activityFailedCount++ == SendBoardInvitationModel.ActivityFailedCount)
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                            $"{"LangKeyStopActivityAfter".FromResourceDictionary()} {_activityFailedCount - 1} " +
                            $"{"LangKeyFailedAttemptFor".FromResourceDictionary()} {SendBoardInvitationModel.FailedActivityReschedule} " +
                            $"{"LangKeyHour".FromResourceDictionary()}");

                        StopAndRescheduleJob(SendBoardInvitationModel.FailedActivityReschedule);
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

        private void AddSendBoardInvitationDataToDataBase(ScrapeResultNew scrapeResult)
        {
            try
            {
                IDbAccountService dbAccountService = new DbAccountService(DominatorAccountModel);
                IDbCampaignService dbCampaignService = new DbCampaignService(CampaignId);
                var jobActivityConfigurationManager =
                    InstanceProvider.GetInstance<IJobActivityConfigurationManager>();
                var moduleSetting = jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];
                if (moduleSetting == null)
                    return;

                var board = (PinterestBoard) scrapeResult.ResultPost;

                if (moduleSetting.IsTemplateMadeByCampaignMode)
                    dbCampaignService.Add(new DominatorHouseCore.DatabaseHandler.PdTables.Campaigns.InteractedBoards
                    {
                        OperationType = ActivityType,
                        InteractionDate = DateTimeUtilities.GetEpochTime(),
                        BoardUrl = board.BoardUrl,
                        BoardId = board.Id,
                        BoardName = board.BoardName,
                        BoardDescription = board.BoardDescription,
                        FollowerCount = board.FollowersCount,
                        PinCount = board.PinsCount,
                        UserId = board.UserId,
                        Username = board.UserName,
                        SinAccId = DominatorAccountModel.AccountBaseModel.UserId,
                        SinAccUsername = DominatorAccountModel.AccountBaseModel.UserName
                    });
                dbAccountService.Add(new InteractedBoards
                {
                    OperationType = ActivityType,
                    InteractionDate = DateTimeUtilities.GetEpochTime(),
                    BoardId = board.Id,
                    BoardUrl = board.BoardUrl,
                    BoardName = board.BoardName,
                    BoardDescription = board.BoardDescription,
                    FollowerCount = board.FollowersCount,
                    PinCount = board.PinsCount,
                    UserId = DominatorAccountModel.AccountBaseModel.UserId,
                    Username = DominatorAccountModel.AccountBaseModel.UserName,
                    UserIdToCollaborate= board.EmailToCollaborate,
                });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}