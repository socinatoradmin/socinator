using System;
using System.Collections.Generic;
using System.Linq;
using CommonServiceLocator;
using ThreadUtils;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.PdTables.Accounts;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using PinDominatorCore.PDLibrary.DAL;
using PinDominatorCore.PDModel;
using PinDominatorCore.Response;

namespace PinDominatorCore.PDLibrary.Processors.Board
{
    public class CustomUserBoardProcessor : BasePinterestBoardProcessor
    {
        private readonly IDelayService _delayService;
        private BoardsOfUserResponseHandler _allBoards;
        public CustomUserBoardProcessor(IPdJobProcess jobProcess, IDbGlobalService globalService,
            IDbCampaignService campaignService, IPinFunction objPinFunct, IDelayService delayService) :
            base(jobProcess, globalService, campaignService, objPinFunct)
        {
            _delayService = delayService;
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            try
            {
                var arrUser = queryInfo.QueryValue.Split('/');

                if (queryInfo.QueryValue.Contains("/") && (arrUser.Length >= 5 && !string.IsNullOrEmpty(arrUser[4]) ||
                                                           !(arrUser.Length >= 4 && arrUser[0].Contains("http") &&
                                                             arrUser[2].Contains("pinterest") &&
                                                             !string.IsNullOrEmpty(arrUser[3]))))
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                        JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        JobProcess.DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                        string.Format("LangKeyCheckSomeUrlSeemsIncorrect".FromResourceDictionary(), "LangKeyUser".FromResourceDictionary()));
                    jobProcessResult.IsProcessCompleted = true;
                    return;
                }

                var genericFileManager = InstanceProvider.GetInstance<IGenericFileManager>();
                var pinConfig =
                    genericFileManager.GetModel<PinterestOtherConfigModel>(ConstantVariable
                        .GetOtherPinterestSettingsFile());
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                // Here we are scraping data until maximum pagination given by user and that data we will store into
                // database for further use then we will send the data to perform activity.
                if (pinConfig.IsScrapDataBeforeSendToPerformActivity)
                {
                    var maxPagination = pinConfig.PaginationCount;
                    var currentPagination = 0;
                    var listOfScrapedBoards = new List<PinterestBoard>();
                    var alreadyScraped = new List<ScrapBoards>();

                    alreadyScraped = DbAccountService.GetScrapBoardsWithSameQuery(ActivityType + "_Scrap", queryInfo).ToList();
                    if (alreadyScraped.Count == 0)
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                      JobProcess.DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                      String.Format("LangKeyWaitForSomeTimeWhileScrappingMessage".FromResourceDictionary(), ActivityType));
                        if (JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser)
                            listOfScrapedBoards = BrowserManager.SearchBoardsOfUser(JobProcess.DominatorAccountModel, queryInfo.QueryValue,
                                JobProcess.JobCancellationTokenSource, scroll: maxPagination);
                        else
                            while (maxPagination >= currentPagination && jobProcessResult.HasNoResult == false)
                            {
                                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                                _allBoards = PinFunction.GetBoardsOfUser(queryInfo.QueryValue, JobProcess.DominatorAccountModel);
                                _delayService.ThreadSleep(new Random().Next(3, 5) * 1000);
                                if (_allBoards == null || !_allBoards.Success)
                                {
                                    jobProcessResult.HasNoResult = true;
                                    jobProcessResult.maxId = null;
                                }
                                else
                                {
                                    if (_allBoards.HasMoreResults == false)
                                    {
                                        jobProcessResult.HasNoResult = true;
                                        jobProcessResult.maxId = null;
                                    }
                                    else
                                        jobProcessResult.maxId = _allBoards.BookMark;
                                    if (ActivityType == ActivityType.Follow)
                                        listOfScrapedBoards.AddRange(_allBoards.BoardsList.Where(x => !x.IsFollowed).ToList());
                                    else
                                        listOfScrapedBoards.AddRange(_allBoards.BoardsList.ToList());
                                    if (_allBoards.HasMoreResults == false)
                                        jobProcessResult.HasNoResult = true;
                                    currentPagination++;
                                }
                            }
                        List<ScrapBoards> dataToBeAdded = new List<ScrapBoards>();
                        foreach (PinterestBoard board in listOfScrapedBoards)
                            if (alreadyScraped.All(x => x.BoardUrl != board.BoardUrl))
                                dataToBeAdded.Add(new ScrapBoards
                                {
                                    OperationType = ActivityType + "_Scrap",
                                    InteractionTime = DateTimeUtilities.GetEpochTime(),
                                    QueryType = queryInfo.QueryType,
                                    Query = queryInfo.QueryValue,
                                    Username = board.UserName,
                                    UserId = board.UserId,
                                    BoardDescription = board.BoardDescription,
                                    BoardName = board.BoardName,
                                    BoardUrl = board.BoardUrl,
                                    BoardId = board.Id,
                                    FollowerCount = board.FollowersCount,
                                    PinCount = board.PinsCount,
                                    Filtered = false,
                                    FullDetailsScraped = false
                                });
                        DbAccountService.AddRange(dataToBeAdded);
                        alreadyScraped.AddRange(dataToBeAdded);
                    }


                    var listOfBoards = new List<PinterestBoard>();
                    foreach (var board in alreadyScraped)
                    {
                        var pinterestBoard = new PinterestBoard();
                        pinterestBoard.UserName = board.Username;
                        pinterestBoard.UserId = board.UserId;
                        pinterestBoard.FollowersCount = board.FollowerCount;
                        pinterestBoard.BoardDescription = board.BoardDescription;
                        pinterestBoard.BoardUrl = board.BoardUrl;
                        pinterestBoard.BoardName = board.BoardName;
                        pinterestBoard.PinsCount = board.PinCount;
                        pinterestBoard.Id = board.BoardId;
                        listOfBoards.Add(pinterestBoard);
                    }

                    if (ActivityType == ActivityType.Follow)
                        listOfBoards = listOfBoards.Where(x => !x.IsFollowed).ToList();
                    StartProcessForListOfBoards(queryInfo, ref jobProcessResult, listOfBoards);
                }
                else
                {
                    List<PinterestBoard> listOfBoards = new List<PinterestBoard>();
                    if (JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser)
                    {
                        var isScroll = false;
                        var scroll = 0;
                        do
                        {
                            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                            listOfBoards = BrowserManager.SearchBoardsOfUser(JobProcess.DominatorAccountModel,
                                queryInfo.QueryValue, JobProcess.JobCancellationTokenSource, isScroll, scroll);
                            scroll = 1;
                            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                            StartProcessForListOfBoards(queryInfo, ref jobProcessResult, listOfBoards);
                            isScroll = true;
                        }
                        while (listOfBoards.Count > 0);
                    }
                    else
                        while (jobProcessResult.HasNoResult == false)
                        {
                            _allBoards = PinFunction.GetBoardsOfUser(queryInfo.QueryValue, JobProcess.DominatorAccountModel);
                            if (_allBoards == null || !_allBoards.Success || _allBoards.BoardsList.Count == 0)
                                GlobusLogHelper.log.Info(Log.NoMoreDataToPerform, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    JobProcess.DominatorAccountModel.UserName, ActivityType);
                            else
                            {
                                if (ActivityType == ActivityType.Follow)
                                    listOfBoards = _allBoards.BoardsList.Where(x => !x.IsFollowed).ToList();
                                else
                                    listOfBoards = _allBoards.BoardsList;
                                StartProcessForListOfBoards(queryInfo, ref jobProcessResult, listOfBoards);
                                jobProcessResult.HasNoResult = true;
                            }
                        }
                }
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}