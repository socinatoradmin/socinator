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

namespace PinDominatorCore.PDLibrary.Processors.Pin
{
    public class OwnFollowersPinProcessor : BasePinterestPinProcessor
    {
        private FollowerAndFollowingPtResponseHandler _followerUsers;
        public OwnFollowersPinProcessor(IPdJobProcess jobProcess, IDbGlobalService globalService,
            IDbCampaignService campaignService, IPinFunction objPinFunct, IDelayService delayService) :
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

                if (pinConfig.IsScrapDataBeforeSendToPerformActivity)
                {
                    var maxPagination = pinConfig.PaginationCount;
                    var currentPagination = 0;
                    var listOfScrapedPins = new List<PinterestPin>();
                    var alreadyScraped = new List<ScrapPins>();
                    alreadyScraped = DbAccountService.GetScrapPinsWithSameQuery(ActivityType + "_Scrap", queryInfo)
                        .ToList();

                    // Here we are scraping data until maximum pagination given by user and that data we will store into
                    // database for further use then we will send the data to perform activity.
                    if (alreadyScraped.Count == 0)
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage,
                            JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            JobProcess.DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                            String.Format("LangKeyWaitForSomeTimeWhileScrappingMessage".FromResourceDictionary(), ActivityType));
                        if (JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser)
                        {
                            while (maxPagination >= currentPagination && jobProcessResult.HasNoResult == false)
                            {
                                bool isScroll = false;
                                int scroll = 0;
                                List<PinterestUser> followers = new List<PinterestUser>();
                                do
                                {
                                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                                    followers = BrowserManager.GetUserFollowers(JobProcess.DominatorAccountModel,
                                        JobProcess.DominatorAccountModel.AccountBaseModel.ProfileId, JobProcess.JobCancellationTokenSource,
                                         isScroll, scroll);
                                    scroll++;
                                    foreach (PinterestUser user in followers)
                                    {
                                        try
                                        {
                                            BrowserManager.AddNew(JobProcess.JobCancellationTokenSource, $"https://{BrowserManager.Domain}");
                                            bool isScrollInner = false;
                                            int scrollInner = 0;
                                            List<PinterestPin> pins = new List<PinterestPin>();
                                            do
                                            {
                                                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                                                pins = BrowserManager.SearchPinsOfUser(JobProcess.DominatorAccountModel,
                                                user.Username, JobProcess.JobCancellationTokenSource, isScrollInner, scrollInner);
                                                pins = AlreadyInteractedPin(queryInfo, pins);
                                                pins = FilterBlackListUser(TemplateModel, pins);
                                                StartProcessForListOfPins(queryInfo, ref jobProcessResult, pins);
                                                scrollInner = 1;
                                                isScrollInner = true;
                                            }
                                            while (pins.Count > 0);
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
                                            BrowserManager.CloseLast();
                                        }
                                    }
                                    isScroll = true;
                                }
                                while (followers.Count > 0);
                            }
                        }
                        else
                            while (maxPagination >= currentPagination && jobProcessResult.HasNoResult == false)
                            {
                                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                                _followerUsers = PinFunction.GetUserFollowers(JobProcess.DominatorAccountModel.AccountBaseModel.ProfileId,
                                    JobProcess.DominatorAccountModel, jobProcessResult.maxId);
                                System.Threading.Thread.Sleep(new Random().Next(3, 5));
                                if (_followerUsers == null || !_followerUsers.Success
                                                           || _followerUsers.HasMoreResults == false)
                                {
                                    jobProcessResult.HasNoResult = true;
                                    jobProcessResult.maxId = null;
                                }
                                else
                                {
                                    foreach (PinterestUser pinterestUser in _followerUsers.UsersList)
                                    {
                                        #region Inner loop mentioning Followers of Followers Opertaion
                                        try
                                        {
                                            while (maxPagination >= currentPagination && jobProcessResult.HasNoResult == false)
                                            {
                                                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                                                // Get Pins of Followers Users
                                                PinsFromSpecificUserResponseHandler allPins = PinFunction.GetPinsFromSpecificUser(pinterestUser.Username,
                                                    JobProcess.DominatorAccountModel, jobProcessResult.maxId,NeedCommentCount:ModuleSetting.PostFilterModel.FilterComments);

                                                if (allPins != null && allPins.Success)
                                                {
                                                    jobProcessResult.maxId = allPins.BookMark;
                                                    listOfScrapedPins.AddRange(allPins.LstUserPin.ToList());

                                                    if (allPins.HasMoreResults == false)
                                                        jobProcessResult.HasNoResult = true;
                                                    currentPagination++;
                                                }
                                                else
                                                {
                                                    jobProcessResult.HasNoResult = true;
                                                    jobProcessResult.maxId = _followerUsers.BookMark;
                                                }
                                            }
                                            jobProcessResult.HasNoResult = false;
                                            jobProcessResult.maxId = null;
                                            if (maxPagination <= currentPagination)
                                                break;
                                        }
                                        catch (Exception ex)
                                        {
                                            ex.DebugLog();
                                        }
                                        #endregion
                                    }

                                    if (_followerUsers.HasMoreResults == false)
                                    {
                                        jobProcessResult.HasNoResult = true;
                                        jobProcessResult.maxId = null;
                                    }
                                    else
                                        jobProcessResult.maxId = _followerUsers.BookMark;
                                    if (_followerUsers.HasMoreResults == false)
                                        jobProcessResult.HasNoResult = true;
                                }
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
                    List<PinterestUser> followers = new List<PinterestUser>();

                    if (JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser)
                    {
                        bool isScroll = false;
                        int scroll = 0;
                        do
                        {
                            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                            followers = BrowserManager.GetUserFollowers(JobProcess.DominatorAccountModel,
                                JobProcess.DominatorAccountModel.AccountBaseModel.ProfileId, JobProcess.JobCancellationTokenSource,
                                 isScroll, scroll);
                            scroll = 1;
                            foreach (PinterestUser user in followers)
                            {
                                try
                                {
                                    BrowserManager.AddNew(JobProcess.JobCancellationTokenSource, $"https://{BrowserManager.Domain}");
                                    bool isScrollInner = false;
                                    int scrollInner = 0;
                                    List<PinterestPin> pins = new List<PinterestPin>();
                                    do
                                    {
                                        JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                                        pins = BrowserManager.SearchPinsOfUser(JobProcess.DominatorAccountModel,
                                        user.Username, JobProcess.JobCancellationTokenSource, isScrollInner, scrollInner);
                                        pins = AlreadyInteractedPin(queryInfo, pins);
                                        pins = FilterBlackListUser(TemplateModel, pins);
                                        StartProcessForListOfPins(queryInfo, ref jobProcessResult, pins);
                                        scrollInner = 1;
                                        isScrollInner = true;
                                    }
                                    while (pins.Count > 0);
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
                                    BrowserManager.CloseLast();
                                }
                            }
                            isScroll = true;
                        }
                        while (followers.Count > 0);
                    }
                    else
                        while (jobProcessResult.HasNoResult == false)
                        {
                            _followerUsers = PinFunction.GetUserFollowers(JobProcess.DominatorAccountModel.AccountBaseModel.ProfileId,
                                    JobProcess.DominatorAccountModel, jobProcessResult.maxId);

                            if (_followerUsers == null || !_followerUsers.Success || _followerUsers.UsersList.Count == 0)
                            {
                                GlobusLogHelper.log.Info(Log.NoMoreDataToPerform, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    JobProcess.DominatorAccountModel.UserName, ActivityType);
                                jobProcessResult.HasNoResult = true;
                                jobProcessResult.maxId = null;
                            }
                            else
                            {
                                jobProcessResult.maxId = _followerUsers.BookMark;
                                foreach (PinterestUser user in _followerUsers.UsersList)
                                {
                                    string bookMark = null;
                                    PinsFromSpecificUserResponseHandler allPins = null;
                                    do
                                    {
                                        allPins = PinFunction.GetPinsFromSpecificUser(user.Username,
                                                        JobProcess.DominatorAccountModel, bookMark);
                                        if (allPins != null && allPins.Success)
                                        {
                                            bookMark = allPins.BookMark;
                                            List<PinterestPin> listOfPins = allPins.LstUserPin;
                                            if (ActivityType != ActivityType.Repin)
                                                listOfPins = AlreadyInteractedPin(queryInfo, listOfPins);
                                            listOfPins = FilterBlackListUser(TemplateModel, listOfPins);
                                            StartProcessForListOfPins(queryInfo, ref jobProcessResult, listOfPins);
                                        }
                                    }
                                    while (allPins != null && allPins.HasMoreResults);
                                }
                                if (_followerUsers.HasMoreResults == false)
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