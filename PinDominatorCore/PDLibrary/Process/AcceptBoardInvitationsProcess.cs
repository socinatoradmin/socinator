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
    public class AcceptBoardInvitationsProcess : PdJobProcessInteracted<InteractedBoards>
    {
        private readonly IDbCampaignService _dbCampaignService;

        public AcceptBoardInvitationsProcess(IProcessScopeModel processScopeModel,
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
            var board = (PinterestBoard) scrapeResult.ResultPost;
            GlobusLogHelper.log.Info(Log.StartedActivity, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                DominatorAccountModel.AccountBaseModel.UserName, ActivityType, board.BoardUrl);

            try
            {
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                AcceptBoardInvitationResponseHandler response;

                if (DominatorAccountModel.IsRunProcessThroughBrowser)
                    response = PdLogInProcess.BrowserManager.AcceptBoardInvitation(DominatorAccountModel, board.BoardUrl,
                        JobCancellationTokenSource);
                else
                    response = PinFunct.AcceptBoardInvitation(board, DominatorAccountModel);

                if (response != null && response.Success)
                {
                    JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                    GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                        DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType, board.BoardUrl);

                    IncrementCounters();
                    
                    BoardInfoPtResponseHandler boardInfo;
                    if (DominatorAccountModel.IsRunProcessThroughBrowser)
                        boardInfo = PdLogInProcess.BrowserManager.SearchByCustomBoard(DominatorAccountModel, board.BoardUrl,
                         JobCancellationTokenSource);
                    else
                        boardInfo = PinFunct.GetBoardDetails(board.BoardUrl, DominatorAccountModel);
                    
                    AddAcceptBoardInvitationDataToDataBase(boardInfo);
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

        private void AddAcceptBoardInvitationDataToDataBase(PinterestBoard board)
        {
            try
            {
                var jobActivityConfigurationManager =
                    InstanceProvider.GetInstance<IJobActivityConfigurationManager>();
                var moduleSetting = jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];
                if (moduleSetting == null)
                    return;
                if (moduleSetting.IsTemplateMadeByCampaignMode)
                    _dbCampaignService.Add(new DominatorHouseCore.DatabaseHandler.PdTables.Campaigns.InteractedBoards
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
                DbAccountService.Add(new InteractedBoards
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
                    Username = DominatorAccountModel.AccountBaseModel.UserName
                });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}