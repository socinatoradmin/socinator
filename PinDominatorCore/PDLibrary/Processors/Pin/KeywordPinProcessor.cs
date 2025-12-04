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
using PinDominatorCore.PDUtility;
using PinDominatorCore.Response;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PinDominatorCore.PDLibrary.Processors.Pin
{
    public class KeywordPinProcessor : BasePinterestPinProcessor
    {
        private SearchAllPinResponseHandler _searchAllPinResponseHandler;
        private readonly PinterestOtherConfigModel pinConfig;
        private readonly IProcessScopeModel processScope;
        public KeywordPinProcessor(IPdJobProcess jobProcess, IDbGlobalService globalService,
            IDbCampaignService campaignService, IPinFunction objPinFunct, IDelayService delayService,IProcessScopeModel processScopeModel) :
            base(jobProcess, globalService, campaignService, objPinFunct)
        {
            processScope = processScopeModel;
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            try
            {
                //To check Is Account Selected For Repin Query
                if (JobProcess.ActivityType == ActivityType.Repin && IsAccountSelectedForRepinQuery(queryInfo))
                    return;

                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                var genericFileManager = InstanceProvider.GetInstance<IGenericFileManager>();
                var pinConfig = processScope.GetActivitySettingsAs<PinterestOtherConfigModel>();
                var commentModel=processScope.GetActivitySettingsAs<CommentModel>();
                var CommentRequired = commentModel.UserFilterModel.FilterPostCounts || commentModel.PostFilterModel.FilterComments;
                // Here we are scraping data until maximum pagination given by user and that data we will store into
                // database for further use then we will send the data to perform activity.
                if (pinConfig.IsScrapDataBeforeSendToPerformActivity)
                {
                    var maxPagination = pinConfig.PaginationCount;
                    var currentPagination = 0;
                    var listOfScrapedPins = new List<PinterestPin>();
                    var alreadyScraped = new List<ScrapPins>();
                    var totalData = 0;
                    alreadyScraped = DbAccountService.GetScrapPinsWithSameQuery(ActivityType + "_Scrap", queryInfo)
                        .ToList();
                    totalData = alreadyScraped.Count;

                    if (totalData == 0)
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage,
                            JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            JobProcess.DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                            String.Format("LangKeyWaitForSomeTimeWhileScrappingMessage".FromResourceDictionary(), ActivityType));

                        if (JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser)
                            listOfScrapedPins = BrowserManager.SearchPinsByKeyword(JobProcess.DominatorAccountModel, queryInfo.QueryValue,
                                JobProcess.JobCancellationTokenSource, scroll: maxPagination);
                        else
                            while (maxPagination > currentPagination && jobProcessResult.HasNoResult == false)
                            {
                                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                                _searchAllPinResponseHandler = PinFunction.GetAllPinsByKeyword(queryInfo.QueryValue, JobProcess.DominatorAccountModel,
                                    jobProcessResult.maxId,PdConstants.GetTicks, CommentRequired);
                                System.Threading.Thread.Sleep(new System.Random().Next(3, 5));
                                if (_searchAllPinResponseHandler == null || !_searchAllPinResponseHandler.Success)
                                {
                                    jobProcessResult.HasNoResult = true;
                                    jobProcessResult.maxId = null;
                                }
                                else
                                {
                                    if (_searchAllPinResponseHandler.HasMoreResults == false)
                                    {
                                        jobProcessResult.HasNoResult = true;
                                        jobProcessResult.maxId = null;
                                    }
                                    else
                                        jobProcessResult.maxId = _searchAllPinResponseHandler.BookMark;
                                    listOfScrapedPins.AddRange(_searchAllPinResponseHandler.LstPin.ToList());
                                    if (_searchAllPinResponseHandler.HasMoreResults == false)
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
                                    PublishedDate = pin.PublishDate,
                                    MediaString = pin.MediaString,
                                    Filtered = false,
                                    FullDetailsScraped = false
                                });
                        DbAccountService.AddRange(dataToBeAdded);
                        alreadyScraped.AddRange(dataToBeAdded);
                    }
                    else
                    {
                        if (totalData - alreadyScraped.Count > 0)
                            GlobusLogHelper.log.Info(Log.CustomMessage,
                                SocialNetworks.Pinterest,
                                JobProcess.DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                $"Filtered => {totalData - alreadyScraped.Count} pins.");
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
                        pinterestPin.PublishDate = pin.PublishedDate;
                        listOfPins.Add(pinterestPin);
                    }
                    listOfPins = AlreadyInteractedPin(queryInfo, listOfPins);
                    listOfPins = FilterBlackListUser(TemplateModel, listOfPins);
                    StartProcessForListOfPins(queryInfo, ref jobProcessResult, listOfPins);
                }
                else
                {
                    var lstSearchedKeywordPin = new List<PinterestPin>();
                    var lstKeywordPin = new List<PinterestPin>();
                    if (JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser)
                    {
                        var scroll = 0;
                        var isScroll = false;
                        do
                        {
                            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                            lstSearchedKeywordPin = BrowserManager.SearchPinsByKeyword(JobProcess.DominatorAccountModel,
                                    queryInfo.QueryValue, JobProcess.JobCancellationTokenSource, isScroll, scroll);
                            if(PinFunction!=null && CommentRequired)
                                PinFunction.GetPinComments(ref lstSearchedKeywordPin,JobProcess.DominatorAccountModel);
                            scroll = 1;
                            isScroll = true;

                            GlobusLogHelper.log.Info(Log.FoundXResults, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                  JobProcess.DominatorAccountModel.UserName, lstSearchedKeywordPin.Count, queryInfo.QueryType, queryInfo.QueryValue, ActivityType);

                            if (ActivityType != ActivityType.Repin)
                                lstKeywordPin = AlreadyInteractedPin(queryInfo, lstSearchedKeywordPin);
                            else
                                lstKeywordPin = lstSearchedKeywordPin;

                            lstKeywordPin = FilterBlackListUser(TemplateModel, lstKeywordPin);

                            StartProcessForListOfPins(queryInfo, ref jobProcessResult, lstKeywordPin);
                        }
                        while (lstSearchedKeywordPin.Count > 0);
                    }
                    else
                        while (jobProcessResult.HasNoResult == false)
                        {
                            _searchAllPinResponseHandler = PinFunction.GetAllPinsByKeyword(queryInfo.QueryValue, JobProcess.DominatorAccountModel, jobProcessResult.maxId,PdConstants.GetTicks,CommentRequired);

                            if (_searchAllPinResponseHandler == null || !_searchAllPinResponseHandler.Success
                                || _searchAllPinResponseHandler.LstPin.Count == 0)
                            {
                                GlobusLogHelper.log.Info(Log.NoMoreDataToPerform, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    JobProcess.DominatorAccountModel.UserName, ActivityType);
                                jobProcessResult.HasNoResult = true;
                                jobProcessResult.maxId = null;
                            }
                            else
                            {
                                jobProcessResult.maxId = _searchAllPinResponseHandler.BookMark;

                                if (ActivityType != ActivityType.Repin)
                                    lstKeywordPin = AlreadyInteractedPin(queryInfo, _searchAllPinResponseHandler.LstPin);
                                else
                                    lstKeywordPin = _searchAllPinResponseHandler.LstPin;

                                lstKeywordPin = FilterBlackListUser(TemplateModel, lstKeywordPin);

                                GlobusLogHelper.log.Info(Log.FoundXResults, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    JobProcess.DominatorAccountModel.UserName, _searchAllPinResponseHandler.LstPin.Count,
                                    queryInfo.QueryType, queryInfo.QueryValue, ActivityType);

                                StartProcessForListOfPins(queryInfo, ref jobProcessResult, lstKeywordPin);
                                if (_searchAllPinResponseHandler.HasMoreResults == false)
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