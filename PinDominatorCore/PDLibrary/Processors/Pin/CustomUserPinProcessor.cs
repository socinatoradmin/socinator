using CommonServiceLocator;
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

namespace PinDominatorCore.PDLibrary.Processors.Pin
{
    public class CustomUserPinProcessor : BasePinterestPinProcessor
    {
        private PinsFromSpecificUserResponseHandler _allPins;
        public CustomUserPinProcessor(IPdJobProcess jobProcess, IDbGlobalService globalService,
            IDbCampaignService campaignService, IPinFunction objPinFunct) :
            base(jobProcess, globalService, campaignService, objPinFunct)
        {
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
                var pinConfig =
                    genericFileManager.GetModel<PinterestOtherConfigModel>(ConstantVariable
                        .GetOtherPinterestSettingsFile());
                var arrUser = queryInfo.QueryValue.Split('/');
                if (queryInfo.QueryValue.Contains("/") && (arrUser.Length >= 5 && !string.IsNullOrEmpty(arrUser[4]) ||
                                                           !(arrUser.Length >= 4 && arrUser[0].Contains("http") &&
                                                             arrUser[2].Contains("pinterest") &&
                                                             !string.IsNullOrEmpty(arrUser[3]))))
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        JobProcess.DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                        String.Format("LangKeyCheckSomeUrlSeemsIncorrect".FromResourceDictionary(), "LangKeyUser".FromResourceDictionary()));
                    jobProcessResult.IsProcessCompleted = true;
                    return;
                }

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
                            listOfScrapedPins = BrowserManager.SearchPinsOfUser(JobProcess.DominatorAccountModel, queryInfo.QueryValue,
                                JobProcess.JobCancellationTokenSource, scroll: maxPagination);
                        else
                            while (maxPagination >= currentPagination && jobProcessResult.HasNoResult == false)
                            {
                                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                                _allPins = PinFunction.GetPinsFromSpecificUser(queryInfo.QueryValue, JobProcess.DominatorAccountModel,
                                    jobProcessResult.maxId);
                                System.Threading.Thread.Sleep(new System.Random().Next(3, 5));
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
                                    listOfScrapedPins.AddRange(_allPins.LstUserPin.ToList());
                                    if (_allPins.HasMoreResults == false)
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
                                    PinTitle = pin.PinName,
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
                        pinterestPin.PinName = pin.PinTitle;
                        listOfPins.Add(pinterestPin);
                    }

                    if (ActivityType != ActivityType.Repin)
                        listOfPins = AlreadyInteractedPin(queryInfo, listOfPins);
                    listOfPins = FilterBlackListUser(TemplateModel, listOfPins);
                    StartProcessForListOfPins(queryInfo, ref jobProcessResult, listOfPins);
                    jobProcessResult.HasNoResult = true;
                }
                else
                {
                    var lstSearchedUserPin = new List<PinterestPin>();
                    var lstUserPin = new List<PinterestPin>();
                    if (JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser)
                    {
                        bool isScroll = false;
                        int scroll = 0;
                        do
                        {
                            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                            lstSearchedUserPin = BrowserManager.SearchPinsOfUser(JobProcess.DominatorAccountModel, queryInfo.QueryValue,
                                JobProcess.JobCancellationTokenSource, isScroll, scroll);
                            scroll = 1;
                            isScroll = true;

                            GlobusLogHelper.log.Info(Log.FoundXResults, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                  JobProcess.DominatorAccountModel.UserName, lstSearchedUserPin.Count, queryInfo.QueryType, queryInfo.QueryValue, ActivityType);

                            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                            if (ActivityType != ActivityType.Repin)
                                lstUserPin = AlreadyInteractedPin(queryInfo, lstSearchedUserPin);
                            else
                                lstUserPin = lstSearchedUserPin;
                            var FilteredResults = IsFilteredBlackListed(TemplateModel,ref lstUserPin);
                            if (FilteredResults.Item1)
                                break;
                            lstUserPin = FilteredResults.Item2;
                            StartProcessForListOfPins(queryInfo, ref jobProcessResult, lstUserPin);
                            if (jobProcessResult.IsProcessCompleted)
                                break;
                        }
                        while (lstUserPin?.Count > 0);
                    }
                    else
                        while (jobProcessResult.HasNoResult == false)
                        {
                            _allPins = PinFunction.GetPinsFromSpecificUser(queryInfo.QueryValue, JobProcess.DominatorAccountModel, jobProcessResult.maxId,string.Empty,false);
                            if (_allPins == null || !_allPins.Success)
                            {
                                GlobusLogHelper.log.Info(Log.NoMoreDataToPerform, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    JobProcess.DominatorAccountModel.UserName, ActivityType);
                                break;
                            }
                            else
                            {
                                jobProcessResult.maxId = _allPins.BookMark;

                                GlobusLogHelper.log.Info(Log.FoundXResults, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                 JobProcess.DominatorAccountModel.UserName, _allPins.LstUserPin.Count, queryInfo.QueryType, queryInfo.QueryValue, ActivityType);

                                if (ActivityType != ActivityType.Repin)
                                    lstUserPin = AlreadyInteractedPin(queryInfo, _allPins.LstUserPin);
                                else
                                    lstUserPin = _allPins.LstUserPin;
                                var FilteredResults = IsFilteredBlackListed(TemplateModel, ref lstUserPin);
                                if (FilteredResults.Item1)
                                    break;
                                lstUserPin = FilteredResults.Item2;
                                StartProcessForListOfPins(queryInfo, ref jobProcessResult, lstUserPin);
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
            finally
            {
                jobProcessResult.IsProcessCompleted = true;
            }
        }
    }
}