using System;
using System.Linq;
using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.PdTables.Accounts;
using DominatorHouseCore.FileManagers;
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

namespace PinDominatorCore.PDLibrary.Process
{
    public class BoardScraperProcess : PdJobProcessInteracted<InteractedBoards>
    {
        private readonly IDbCampaignService _dbCampaignService;

        public BoardScraperProcess(IProcessScopeModel processScopeModel,
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

            try
            {
                var board = (PinterestBoard) scrapeResult.ResultPost;
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                GlobusLogHelper.log.Info(Log.ActivitySuccessful, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType, board.BoardUrl);

                IncrementCounters();

                AddBoardScrapedDataToDataBase(scrapeResult);
                jobProcessResult.IsProcessSuceessfull = true;
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

        private void AddBoardScrapedDataToDataBase(ScrapeResultNew scrapeResult)
        {
            try
            {
                var board = (PinterestBoard) scrapeResult.ResultPost;
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
                        QueryType = scrapeResult.QueryInfo.QueryType,
                        Query = scrapeResult.QueryInfo.QueryValue,
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
                    QueryType = scrapeResult.QueryInfo.QueryType,
                    Query = scrapeResult.QueryInfo.QueryValue,
                    BoardId = board.BoardUrl,
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