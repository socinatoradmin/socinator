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
using Newtonsoft.Json;
using PinDominatorCore.PDLibrary.DAL;
using PinDominatorCore.PDModel;
using System;
using System.Collections.Generic;
using System.Linq;
using BoardModel = PinDominatorCore.PDModel.Board;

namespace PinDominatorCore.PDLibrary.Processors.Pin
{
    public class DeletePinProcessor : BasePinterestPinProcessor
    {
        private readonly IDelayService _delayService;
        public DeletePinProcessor(IPdJobProcess jobProcess, IDbAccountService dbAccountService, IDbGlobalService globalService,
            IDbCampaignService campaignService, IPinFunction objPinFunct, IPdBrowserManager browserManager, IDelayService delayService) :
            base(jobProcess, globalService, campaignService, objPinFunct)
        {
            _delayService = delayService;
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            var deletePinModel =
                JsonConvert.DeserializeObject<DeletePinModel>(TemplateModel.ActivitySettings);

            if (deletePinModel.IsDeteleAllPins)
                DeleteAllPins(jobProcessResult);

            if (deletePinModel.IsDeteleAllPinsFromBoard)
                DeleteAllPinsOfBoard(jobProcessResult);
        }

        private void DeleteAllPins(JobProcessResult jobProcessResult)
        {
            try
            {
                var genericFileManager = InstanceProvider.GetInstance<IGenericFileManager>();
                var pinConfig =
                    genericFileManager.GetModel<PinterestOtherConfigModel>(ConstantVariable
                        .GetOtherPinterestSettingsFile());

                GlobusLogHelper.log.Info(Log.CustomMessage, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    JobProcess.DominatorAccountModel.UserName, JobProcess.ActivityType, "LangKeySearchingForPinsToDelete".FromResourceDictionary());
                var queryInfo = new QueryInfo
                {
                    QueryType = "DeletePins",
                    QueryValue = "DeleteAllPins"
                };

                if (pinConfig.IsScrapDataBeforeSendToPerformActivity)
                {
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    var maxPagination = pinConfig.PaginationCount;
                    var currentPagination = 0;
                    var listOfScrapedPins = new List<PinterestPin>();
                    var alreadyScraped = new List<ScrapPins>();

                    alreadyScraped = DbAccountService.GetScrapPins(ActivityType + "_Scrap").ToList();

                    if (alreadyScraped.Count == 0)
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                      JobProcess.DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                      String.Format("LangKeyWaitForSomeTimeWhileScrappingMessage".FromResourceDictionary(), ActivityType));

                        if (JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser)
                            listOfScrapedPins = BrowserManager.SearchPinsOfUser(JobProcess.DominatorAccountModel,
                                JobProcess.DominatorAccountModel.AccountBaseModel.ProfileId,
                                JobProcess.JobCancellationTokenSource, scroll: maxPagination);
                        else
                            while (maxPagination >= currentPagination && jobProcessResult.HasNoResult == false)
                            {
                                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                                var allPins = PinFunction.GetPinsFromSpecificUser
                                   (JobProcess.DominatorAccountModel.AccountBaseModel.ProfileId, JobProcess.DominatorAccountModel,
                                   jobProcessResult.maxId,NeedCommentCount:ModuleSetting.PostFilterModel.FilterComments);
                                _delayService.ThreadSleep(new Random().Next(3, 5));
                                if (allPins == null || !allPins.Success)
                                {
                                    jobProcessResult.HasNoResult = true;
                                    jobProcessResult.maxId = null;
                                }
                                else
                                {
                                    if (allPins.HasMoreResults == false)
                                    {
                                        jobProcessResult.HasNoResult = true;
                                        jobProcessResult.maxId = null;
                                    }
                                    else
                                        jobProcessResult.maxId = allPins.BookMark;
                                    listOfScrapedPins.AddRange(allPins.LstUserPin.ToList());
                                    if (allPins.HasMoreResults == false)
                                        jobProcessResult.HasNoResult = true;
                                    currentPagination++;
                                }
                            }
                        List<ScrapPins> dataToBeAdded = new List<ScrapPins>();
                        foreach (PinterestPin pin in listOfScrapedPins)
                            if (alreadyScraped.All(x => x.PinId != pin.PinId))
                                dataToBeAdded.Add(new ScrapPins
                                {
                                    ActivityType = ActivityType + "_Scrap",
                                    InteractionDate = DateTimeUtilities.GetEpochTime(),
                                    QueryType = queryInfo.QueryType,
                                    QueryValue = queryInfo.QueryValue,
                                    Username = pin.User.Username,
                                    SourceBoardUrl = pin.BoardUrl,
                                    SourceBoardName = pin.BoardName,
                                    DestinationBoard = pin.BoardUrlToRepin,
                                    CommentCount = pin.CommentCount,
                                    MediaType = pin.MediaType,
                                    PinDescription = pin.Description,
                                    PinId = pin.PinId,
                                    PinWebUrl = pin.PinWebUrl,
                                    UserId = pin.User.UserId,
                                    TryCount = pin.NoOfTried,
                                    MediaString = pin.MediaString,
                                    Filtered = false,
                                    FullDetailsScraped = false
                                });
                        DbAccountService.AddRange(dataToBeAdded);
                        alreadyScraped.AddRange(dataToBeAdded);
                    }

                    var listOfPins = new List<PinterestPin>();
                    foreach (var pin in alreadyScraped)
                    {
                        var pinterestPin = new PinterestPin();
                        pinterestPin.User.Username = pin.Username;
                        pinterestPin.BoardUrl = pin.SourceBoardUrl;
                        pinterestPin.BoardName = pin.SourceBoardName;
                        pinterestPin.BoardUrlToRepin = pin.DestinationBoard;
                        pinterestPin.CommentCount = pin.CommentCount;
                        pinterestPin.MediaType = pin.MediaType;
                        pinterestPin.Description = pin.PinDescription;
                        pinterestPin.PinId = pin.PinId;
                        pinterestPin.PinWebUrl = pin.PinWebUrl;
                        pinterestPin.User.UserId = pin.UserId;
                        pinterestPin.NoOfTried = pin.TryCount;
                        pinterestPin.MediaString = pin.MediaString;
                        listOfPins.Add(pinterestPin);
                    }
                    listOfPins = AlreadyInteractedPin(queryInfo, listOfPins);
                    StartProcessForListOfPins(queryInfo, ref jobProcessResult, listOfPins);
                    alreadyScraped.Clear();
                }
                else
                {
                    List<PinterestPin> listOfPins = new List<PinterestPin>();
                    if (JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser)
                    {
                        bool isScroll = false;
                        int scroll = 0;
                        do
                        {
                            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                            listOfPins = BrowserManager.SearchPinsOfUser(JobProcess.DominatorAccountModel,
                                JobProcess.DominatorAccountModel.AccountBaseModel.ProfileId,
                                JobProcess.JobCancellationTokenSource, isScroll, scroll);
                            scroll = 1;
                            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                            listOfPins = AlreadyInteractedPin(queryInfo, listOfPins);
                            listOfPins = FilterBlackListUser(TemplateModel, listOfPins);
                            StartProcessForListOfPins(queryInfo, ref jobProcessResult, listOfPins);
                            isScroll = true;
                        }
                        while (listOfPins.Count > 0);
                    }
                    else
                        while (jobProcessResult.HasNoResult == false)
                        {
                            var allPins = PinFunction.GetPinsFromSpecificUser(JobProcess.DominatorAccountModel.AccountBaseModel.ProfileId,
                                                                              JobProcess.DominatorAccountModel, jobProcessResult.maxId,NeedCommentCount:ModuleSetting.PostFilterModel.FilterComments);

                            if (allPins == null || !allPins.Success
                                || allPins.LstUserPin.Count == 0)
                            {
                                GlobusLogHelper.log.Info(Log.NoMoreDataToPerform, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    JobProcess.DominatorAccountModel.UserName, ActivityType);
                                jobProcessResult.HasNoResult = true;
                                jobProcessResult.maxId = null;
                            }
                            else
                            {
                                listOfPins = AlreadyInteractedPin(queryInfo, allPins.LstUserPin);
                                StartProcessForListOfPins(queryInfo, ref jobProcessResult, listOfPins);
                                jobProcessResult.maxId = allPins.BookMark;
                                if (allPins.HasMoreResults == false)
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

        private void DeleteAllPinsOfBoard(JobProcessResult jobProcessResult)
        {
            try
            {
                GlobusLogHelper.log.Info(Log.CustomMessage,
                    JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    JobProcess.DominatorAccountModel.UserName, JobProcess.ActivityType,
                    "LangKeySearchingForPinsToDelete".FromResourceDictionary());
                var templatesFileManager = InstanceProvider.GetInstance<ITemplatesFileManager>();
                var templateModel = templatesFileManager.GetTemplateById(JobProcess.TemplateId);
                var deletePinModel = JsonConvert.DeserializeObject<DeletePinModel>(templateModel.ActivitySettings);
                var alreadyUsed = DbAccountService.GetInteractedPosts(ActivityType).ToList();
                var boards = deletePinModel.LstBoardsDetails.FirstOrDefault(x =>
                        x.Account == JobProcess.DominatorAccountModel.AccountBaseModel.UserName)
                    ?.LstBoards.FindAll(x => x.IsCheckBoard);
                if (boards != null && boards.Count == 0)
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                        JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        JobProcess.DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                        "LangKeySelectAtleastOneBoard".FromResourceDictionary());
                    return;
                }

                var queryInfo = new QueryInfo
                {
                    QueryType = "DeletePins",
                    QueryValue = "DeleteAllPins"
                };
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                var genericFileManager = InstanceProvider.GetInstance<IGenericFileManager>();
                var pinConfig =
                    genericFileManager.GetModel<PinterestOtherConfigModel>(ConstantVariable
                        .GetOtherPinterestSettingsFile());

                // Here we are scraping data until maximum pagination given by user and that data we will store into
                // database for further use then we will send the data to perform activity.
                if (pinConfig.IsScrapDataBeforeSendToPerformActivity)
                {
                    var maxPagination = pinConfig.PaginationCount;
                    var currentPagination = 0;
                    var listOfScrapedPins = new List<PinterestPin>();
                    var alreadyScraped = new List<ScrapPins>();

                    try
                    {
                        if (boards != null)
                            foreach (var item in boards)
                            {
                                alreadyScraped = DbAccountService.GetScrapPins(ActivityType + "_Scrap").ToList();
                                if (alreadyScraped.Count == 0)
                                {
                                    GlobusLogHelper.log.Info(Log.CustomMessage, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                         JobProcess.DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                         String.Format("LangKeyWaitForSomeTimeWhileScrappingMessage".FromResourceDictionary(), ActivityType));

                                    if (JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser)
                                    {
                                        listOfScrapedPins = BrowserManager.SearchPinsOfBoard(JobProcess.DominatorAccountModel, item.BoardUrl,
                                            JobProcess.JobCancellationTokenSource);
                                    }
                                    else
                                        while (maxPagination >= currentPagination && jobProcessResult.HasNoResult == false)
                                        {
                                            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                                            var allPins = PinFunction.GetPinsByBoardUrl(item.BoardUrl, JobProcess.DominatorAccountModel,
                                                            jobProcessResult.maxId,NeedCommentCount:deletePinModel.PostFilterModel.FilterComments);
                                            _delayService.ThreadSleep(new Random().Next(3, 5));
                                            if (allPins == null || !allPins.Success)
                                            {
                                                jobProcessResult.HasNoResult = true;
                                                jobProcessResult.maxId = null;
                                            }
                                            else
                                            {
                                                if (allPins.HasMoreResults == false)
                                                {
                                                    jobProcessResult.HasNoResult = true;
                                                    jobProcessResult.maxId = null;
                                                }
                                                else
                                                    jobProcessResult.maxId = allPins.BookMark;
                                                listOfScrapedPins.AddRange(allPins.LstBoardPin.ToList());
                                                if (allPins.HasMoreResults == false)
                                                    jobProcessResult.HasNoResult = true;
                                                currentPagination++;
                                            }
                                        }

                                    if (listOfScrapedPins.Count == 0)
                                    {
                                        GlobusLogHelper.log.Info(Log.CustomMessage,
                                            SocialNetworks.Pinterest,
                                            JobProcess.DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                            String.Format("LangKeyNoPinsFoundInBoardToDelete".FromResourceDictionary(), item.BoardUrl));
                                        continue;
                                    }

                                    var dataToBeAdded = new List<ScrapPins>();
                                    foreach (var pin in listOfScrapedPins)
                                        if (alreadyScraped.All(x => x.PinId != pin.PinId))
                                            dataToBeAdded.Add(new ScrapPins
                                            {
                                                ActivityType = ActivityType + "_Scrap",
                                                InteractionDate = DateTimeUtilities.GetEpochTime(),
                                                QueryType = queryInfo.QueryType,
                                                QueryValue = queryInfo.QueryValue,
                                                Username = pin.User.Username,
                                                SourceBoardUrl = pin.BoardUrl,
                                                SourceBoardName = pin.BoardName,
                                                DestinationBoard = pin.BoardUrlToRepin,
                                                CommentCount = pin.CommentCount,
                                                MediaType = pin.MediaType,
                                                PinDescription = pin.Description,
                                                PinId = pin.PinId,
                                                PinWebUrl = pin.PinWebUrl,
                                                UserId = pin.User.UserId,
                                                TryCount = pin.NoOfTried,
                                                MediaString = pin.MediaString,
                                                Filtered = false,
                                                FullDetailsScraped = false
                                            });

                                    DbAccountService.AddRange(dataToBeAdded);
                                    alreadyScraped.AddRange(dataToBeAdded);
                                }

                                if (maxPagination <= currentPagination)
                                    break;
                            }
                    }
                    catch
                    {
                        // ignored
                    }

                    var listOfPins = new List<PinterestPin>();
                    foreach (var pin in alreadyScraped)
                    {
                        listOfPins.Add(new PinterestPin
                        {
                            User = { Username = pin.Username, UserId = pin.UserId },
                            BoardUrl = pin.SourceBoardUrl,
                            BoardName = pin.SourceBoardName,
                            BoardUrlToRepin = pin.DestinationBoard,
                            CommentCount = pin.CommentCount,
                            MediaType = pin.MediaType,
                            Description = pin.PinDescription,
                            PinId = pin.PinId,
                            PinWebUrl = pin.PinWebUrl,
                            NoOfTried = pin.TryCount,
                            MediaString = pin.MediaString
                        });
                    }

                    listOfPins = AlreadyInteractedPin(queryInfo, listOfPins);
                    StartProcessForListOfPins(queryInfo, ref jobProcessResult, listOfPins);
                }
                else
                {
                    if (boards != null)
                        foreach (BoardModel item in boards)
                        {
                            List<PinterestPin> listOfPins = new List<PinterestPin>();
                            if (JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser)
                            {
                                bool isScroll = false;
                                int scroll = 0;
                                do
                                {
                                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                                    listOfPins = BrowserManager.SearchPinsOfBoard(JobProcess.DominatorAccountModel,
                                        item.BoardUrl, JobProcess.JobCancellationTokenSource, isScroll, scroll);
                                    scroll = 1;
                                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                                    listOfPins = AlreadyInteractedPin(queryInfo, listOfPins);
                                    listOfPins = FilterBlackListUser(TemplateModel, listOfPins);
                                    StartProcessForListOfPins(queryInfo, ref jobProcessResult, listOfPins);
                                    isScroll = true;
                                }
                                while (listOfPins.Count > 0);
                            }
                            else
                                while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
                                {
                                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                                    var allPins =
                                        PinFunction.GetPinsByBoardUrl(item.BoardUrl, JobProcess.DominatorAccountModel, jobProcessResult.maxId,NeedCommentCount:deletePinModel.PostFilterModel.FilterComments);

                                    if (allPins.Success)
                                    {
                                        List<string> pinIDs = allPins.LstBoardPin.Select(x => x.PinId).ToList();
                                        alreadyUsed.ForEach(x =>
                                        {
                                            if (pinIDs.Contains(x.PinId))
                                            {
                                                pinIDs.Remove(x.PinId);
                                            }
                                        });
                                        QueryInfo queryinfo = new QueryInfo() { QueryType = "DeletePins" };
                                        StartProcessForListOfPins(queryinfo, ref jobProcessResult, pinIDs);
                                        if (allPins.HasMoreResults == false)
                                            jobProcessResult.HasNoResult = true;
                                    }
                                    else
                                    {
                                        jobProcessResult.HasNoResult = true;
                                        jobProcessResult.maxId = null;
                                    }
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
                jobProcessResult.HasNoResult = true;
                ex.DebugLog();
            }
        }
    }
}