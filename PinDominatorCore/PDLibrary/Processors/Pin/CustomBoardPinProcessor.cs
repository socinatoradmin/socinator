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
using System;
using System.Collections.Generic;
using System.Linq;
using DominatorHouseCore.Request;

namespace PinDominatorCore.PDLibrary.Processors.Pin
{
    public class CustomBoardPinProcessor : BasePinterestPinProcessor
    {
        private readonly IDelayService _delayService;
        private PinsByBoardUrlResponseHandler _allPins;
        public CustomBoardPinProcessor(IPdJobProcess jobProcess, IDbGlobalService globalService,
            IDbCampaignService campaignService, IPinFunction objPinFunct, IDelayService delayService) :
            base(jobProcess, globalService, campaignService, objPinFunct)
        {
            _delayService = delayService;
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            try
            {
                //To check Is Account Selected For Repin Query
                if (JobProcess.ActivityType == ActivityType.Repin && IsAccountSelectedForRepinQuery(queryInfo))
                    return;

                var genericFileManager = InstanceProvider.GetInstance<IGenericFileManager>();
                var pinConfig =
                    genericFileManager.GetModel<PinterestOtherConfigModel>(ConstantVariable
                        .GetOtherPinterestSettingsFile());
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                var arrUser = queryInfo.QueryValue.Split('/');
                if (arrUser.Length >= 5 && string.IsNullOrEmpty(arrUser[4]) || arrUser.Length < 5)
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        JobProcess.DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                        String.Format("LangKeyCheckSomeUrlSeemsIncorrect".FromResourceDictionary(), "LangKeyBoard".FromResourceDictionary()));
                    jobProcessResult.IsProcessCompleted = true;
                    return;
                }

                // Here we are scraping data until maximum pagination given by user and that data we will store into
                // database for further use then we will send the data to perform activity.
                if (pinConfig.IsScrapDataBeforeSendToPerformActivity)
                {
                    var maxPagination = pinConfig.PaginationCount;
                    var currentPagination = 0;
                    var lstOfScrapedPins = new List<PinterestPin>();
                    var alreadyScraped = DbAccountService.GetScrapPinsWithSameQuery(ActivityType + "_Scrap", queryInfo).ToList();
                    if (alreadyScraped.Count == 0)
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                      JobProcess.DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                      String.Format("LangKeyWaitForSomeTimeWhileScrappingMessage".FromResourceDictionary(), ActivityType));

                        if (JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser)
                            lstOfScrapedPins = BrowserManager.SearchPinsOfBoard(JobProcess.DominatorAccountModel, queryInfo.QueryValue,
                                JobProcess.JobCancellationTokenSource, scroll: maxPagination);
                        else
                            while (maxPagination >= currentPagination && jobProcessResult.HasNoResult == false)
                            {
                                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                                _allPins = PinFunction.GetPinsByBoardUrl(queryInfo.QueryValue, JobProcess.DominatorAccountModel, jobProcessResult.maxId,NeedCommentCount:false);
                                _delayService.ThreadSleep(new Random().Next(3, 5) * 1000);
                                if (_allPins == null || !_allPins.Success)
                                {
                                    jobProcessResult.HasNoResult = true;
                                    jobProcessResult.maxId = null;
                                }
                                else
                                {
                                    if (_allPins.HasMoreResults == false)
                                    {
                                        jobProcessResult.HasNoResult = true;
                                        jobProcessResult.maxId = null;
                                    }
                                    else
                                        jobProcessResult.maxId = _allPins.BookMark;
                                    lstOfScrapedPins.AddRange(_allPins.LstBoardPin.ToList());
                                    if (_allPins.HasMoreResults == false)
                                        jobProcessResult.HasNoResult = true;
                                    currentPagination++;
                                }
                            }

                        var dataToBeAdded = new List<ScrapPins>();
                        foreach (PinterestPin pin in lstOfScrapedPins)
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


                    var lstPin = new List<PinterestPin>();
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
                        lstPin.Add(pinterestPin);
                    }

                    lstPin = AlreadyInteractedPin(queryInfo, lstPin);
                    lstPin = FilterBlackListUser(TemplateModel, lstPin);
                    StartProcessForListOfPins(queryInfo, ref jobProcessResult, lstPin);
                }
                else
                {
                    var lstSearchedBoardPin = new List<PinterestPin>();
                    var lstBoardPin = new List<PinterestPin>();
                    while (jobProcessResult.HasNoResult == false)
                    {
                        JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                        if (JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser)
                            lstBoardPin = BrowserManager.SearchPinsOfBoard(JobProcess.DominatorAccountModel, queryInfo.QueryValue,
                                JobProcess.JobCancellationTokenSource, scroll:5);
                        else
                            _allPins = PinFunction.GetPinsByBoardUrl(queryInfo.QueryValue, JobProcess.DominatorAccountModel, jobProcessResult.maxId,NeedCommentCount:false);

                        JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                        if (( _allPins == null || !_allPins.Success || _allPins.LstBoardPin.Count == 0) && lstBoardPin.Count <= 0)
                        {
                            GlobusLogHelper.log.Info(Log.NoMoreDataToPerform, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                JobProcess.DominatorAccountModel.UserName, ActivityType);
                            jobProcessResult.HasNoResult = true;
                        }
                        else
                        {
                            if(_allPins == null)
                                _allPins = new PinsByBoardUrlResponseHandler(new ResponseParameter());
                            jobProcessResult.maxId = _allPins.BookMark;

                            GlobusLogHelper.log.Info(Log.FoundXResults, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                             JobProcess.DominatorAccountModel.UserName, (_allPins.LstBoardPin.Count > 0) ? _allPins.LstBoardPin.Count : lstBoardPin.Count, queryInfo.QueryType, queryInfo.QueryValue, ActivityType);

                            if (ActivityType != ActivityType.Repin)
                                lstBoardPin = AlreadyInteractedPin(queryInfo, _allPins.LstBoardPin);
                            else
                                lstBoardPin = (_allPins.LstBoardPin.Count > 0) ? _allPins.LstBoardPin : lstBoardPin;

                            lstBoardPin = FilterBlackListUser(TemplateModel, lstBoardPin);

                            StartProcessForListOfPins(queryInfo, ref jobProcessResult, lstBoardPin);
                            if (_allPins.HasMoreResults == false)
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