using System.Collections.Generic;
using System.Linq;
using CommonServiceLocator;
using ThreadUtils;
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
using System;
using DominatorHouseCore;

namespace PinDominatorCore.PDLibrary.Processors.Pin
{
    public class NewsFeedPinProcessor : BasePinterestPinProcessor
    {
        private NewsFeedPinsResponseHandler _newsFeedPinsResponseHandler;
        public NewsFeedPinProcessor(IPdJobProcess jobProcess, IDbGlobalService globalService,
            IDbCampaignService campaignService, IPinFunction objPinFunct) :
            base(jobProcess, globalService, campaignService, objPinFunct)
        {
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            try
            {
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
                    alreadyScraped = DbAccountService.GetScrapPinsWithSameQuery(ActivityType + "_Scrap", queryInfo)
                        .ToList();
                    if (alreadyScraped.Count == 0)
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                         JobProcess.DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                         String.Format("LangKeyWaitForSomeTimeWhileScrappingMessage".FromResourceDictionary(), ActivityType));

                        if (JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser)
                            listOfScrapedPins = BrowserManager.SearchNewsFeedPins(JobProcess.DominatorAccountModel,
                                JobProcess.JobCancellationTokenSource, scroll: maxPagination);
                        else
                            while (maxPagination >= currentPagination && jobProcessResult.HasNoResult == false)
                            {
                                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                                _newsFeedPinsResponseHandler = PinFunction.GetNewsFeedPins(JobProcess.DominatorAccountModel, jobProcessResult.maxId);
                                System.Threading.Thread.Sleep(new Random().Next(3, 5));
                                if (_newsFeedPinsResponseHandler == null || !_newsFeedPinsResponseHandler.Success)
                                {
                                    jobProcessResult.HasNoResult = true;
                                    jobProcessResult.maxId = null;
                                }
                                else
                                {
                                    if (_newsFeedPinsResponseHandler.HasMoreResults == false)
                                    {
                                        jobProcessResult.HasNoResult = true;
                                        jobProcessResult.maxId = null;
                                    }
                                    else
                                        jobProcessResult.maxId = _newsFeedPinsResponseHandler.BookMark;
                                    listOfScrapedPins.AddRange(_newsFeedPinsResponseHandler.PinList.ToList());
                                    if (_newsFeedPinsResponseHandler.HasMoreResults == false)
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

                    if (ActivityType != ActivityType.Repin)
                        listOfPins = AlreadyInteractedPin(queryInfo, listOfPins);
                    listOfPins = FilterBlackListUser(TemplateModel, listOfPins);
                    StartProcessForListOfPins(queryInfo, ref jobProcessResult, listOfPins);
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
                            listOfPins = PinFunction.GetNewsFeedPins(JobProcess.DominatorAccountModel, jobProcessResult.maxId).PinList;                            
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
                            _newsFeedPinsResponseHandler = PinFunction.GetNewsFeedPins(JobProcess.DominatorAccountModel, jobProcessResult.maxId);

                            if (_newsFeedPinsResponseHandler == null || !_newsFeedPinsResponseHandler.Success
                                || _newsFeedPinsResponseHandler.PinList.Count == 0)
                            {
                                GlobusLogHelper.log.Info(Log.NoMoreDataToPerform, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    JobProcess.DominatorAccountModel.UserName, ActivityType);
                                jobProcessResult.HasNoResult = true;
                                jobProcessResult.maxId = null;
                            }
                            else
                            {
                                jobProcessResult.maxId = _newsFeedPinsResponseHandler.BookMark;
                                if (ActivityType != ActivityType.Repin)
                                    listOfPins = AlreadyInteractedPin(queryInfo, _newsFeedPinsResponseHandler.PinList);
                                else
                                    listOfPins = _newsFeedPinsResponseHandler.PinList;
                                listOfPins = FilterBlackListUser(TemplateModel, listOfPins);
                                StartProcessForListOfPins(queryInfo, ref jobProcessResult, listOfPins);
                                if (_newsFeedPinsResponseHandler.HasMoreResults == false)
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