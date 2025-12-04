using System;
using System.Collections.Generic;
using System.Linq;
using CommonServiceLocator;
using ThreadUtils;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.PdTables.Accounts;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using Newtonsoft.Json;
using PinDominatorCore.PDLibrary.DAL;
using PinDominatorCore.PDModel;
using PinDominatorCore.Response;

namespace PinDominatorCore.PDLibrary.Processors.Pin
{
    public class EditPinProcessor : BasePinterestPinProcessor
    {
        public EditPinProcessor(IPdJobProcess jobProcess, IDbGlobalService globalService,
            IDbCampaignService campaignService, IPinFunction objPinFunct) :
            base(jobProcess, globalService, campaignService, objPinFunct)
        {
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            try
            {
                var alreadyUsed = CampaignService.GetInteractedPosts(ActivityType).ToList();
                var editPinModel = JsonConvert.DeserializeObject<EditPinModel>(TemplateModel.ActivitySettings);
                queryInfo = new QueryInfo
                {
                    QueryType = "EditPin",
                    QueryValue = "All Pins"
                };
                var pins = editPinModel.PinDetails.Where(
                    pin => pin.Account == JobProcess.DominatorAccountModel.UserName);

                //To Edit all pins of the user
                if (pins.Any(pin => pin.PinToBeEdit.ToLower().Equals("all")))
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
                        var maxPagination = 20;
                        var currentPagination = 0;
                        var listOfScrapedPins = new List<PinterestPin>();
                        var alreadyScraped = new List<ScrapPins>();

                        try
                        {
                            alreadyScraped = DbAccountService
                                .GetScrapPinsWithSameQuery(ActivityType + "_Scrap", queryInfo).ToList();
                            if (alreadyScraped.Count == 0)
                            {
                                GlobusLogHelper.log.Info(Log.CustomMessage, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                JobProcess.DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                               String.Format("LangKeyWaitForSomeTimeWhileScrappingMessage".FromResourceDictionary(), ActivityType));

                                if (JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser)
                                    listOfScrapedPins = BrowserManager.SearchPinsOfUser(JobProcess.DominatorAccountModel,
                                        JobProcess.DominatorAccountModel.AccountBaseModel.ProfileId,
                                        JobProcess.JobCancellationTokenSource,
                                        scroll: maxPagination);
                                else
                                    while (maxPagination >= currentPagination && jobProcessResult.HasNoResult == false)
                                    {
                                        JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                                        PinsFromSpecificUserResponseHandler allPins = PinFunction.GetPinsFromSpecificUser
                                        (JobProcess.DominatorAccountModel.AccountBaseModel.ProfileId,
                                            JobProcess.DominatorAccountModel, jobProcessResult.maxId);
                                        System.Threading.Thread.Sleep(new Random().Next(3, 5));
                                        if (allPins == null || !allPins.Success || allPins.HasMoreResults == false)
                                        {
                                            jobProcessResult.HasNoResult = true;
                                            jobProcessResult.maxId = null;
                                        }
                                        else
                                        {
                                            jobProcessResult.maxId = allPins.BookMark;
                                            listOfScrapedPins.AddRange(allPins.LstUserPin.ToList());
                                            if (allPins.HasMoreResults == false)
                                                jobProcessResult.HasNoResult = true;
                                            currentPagination++;
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
                        }
                        catch
                        {
                            // ignored
                        }

                        var listOfPins = new List<PinterestPin>();
                        var savedPinDetails =
                            editPinModel.PinDetails.FirstOrDefault(pin => pin.PinToBeEdit.ToLower().Equals("all") &&
                                                                          pin.Account == JobProcess
                                                                              .DominatorAccountModel
                                                                              .UserName);

                        foreach (var pin in alreadyScraped)
                        {
                            var pinterestPin = new PinterestPin();
                            pinterestPin.User.Username = pin.Username;
                            pinterestPin.BoardName = pin.SourceBoardName;
                            pinterestPin.CommentCount = pin.CommentCount;
                            pinterestPin.MediaType = pin.MediaType;
                            pinterestPin.PinId = pin.PinId;
                            pinterestPin.User.UserId = pin.UserId;
                            pinterestPin.NoOfTried = pin.TryCount;
                            pinterestPin.MediaString = pin.MediaString;
                            pinterestPin.PinName = pin.PinTitle;

                            if (!string.IsNullOrEmpty(savedPinDetails.PinDescription))
                                pinterestPin.Description = savedPinDetails.PinDescription;
                            else
                                pinterestPin.Description = pin.PinDescription;
                            if (!string.IsNullOrEmpty(savedPinDetails.Board))
                                pinterestPin.BoardUrl = savedPinDetails.Board;
                            else
                                pinterestPin.BoardUrl = pin.SourceBoardUrl;

                            if (!string.IsNullOrEmpty(savedPinDetails.Title))
                                pinterestPin.PinName = savedPinDetails.Title;
                            else
                                pinterestPin.PinName = pin.PinTitle;

                            if (!string.IsNullOrEmpty(savedPinDetails.WebsiteUrl))
                                pinterestPin.PinWebUrl = savedPinDetails.WebsiteUrl;
                            else
                                pinterestPin.PinWebUrl = pin.PinWebUrl;
                            if (!string.IsNullOrEmpty(savedPinDetails.Section))
                                pinterestPin.Section = savedPinDetails.Section;
                            if (!string.IsNullOrEmpty(savedPinDetails.Board))
                                pinterestPin.BoardName = savedPinDetails.Board;
                            else
                                pinterestPin.BoardName = pin.SourceBoardName;
                            listOfPins.Add(pinterestPin);
                        }

                        listOfPins = AlreadyInteractedPin(queryInfo, listOfPins);
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
                                PinsFromSpecificUserResponseHandler allPins = 
                                    PinFunction.GetPinsFromSpecificUser(JobProcess.DominatorAccountModel.AccountBaseModel.ProfileId,
                                    JobProcess.DominatorAccountModel, jobProcessResult.maxId);

                                if (allPins == null || !allPins.Success || allPins.LstUserPin.Count == 0)
                                {
                                    GlobusLogHelper.log.Info(Log.NoMoreDataToPerform,
                                        JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                        JobProcess.DominatorAccountModel.UserName, ActivityType);
                                    jobProcessResult.HasNoResult = true;
                                    jobProcessResult.maxId = null;
                                }
                                else
                                {
                                    listOfPins = allPins.LstUserPin;
                                    listOfPins = AlreadyInteractedPin(queryInfo, listOfPins);
                                    StartProcessForListOfPins(queryInfo, ref jobProcessResult, listOfPins);
                                    jobProcessResult.maxId = allPins.BookMark;
                                    if (allPins.HasMoreResults == false)
                                        jobProcessResult.HasNoResult = true;
                                }
                            }
                    }
                }
                else
                {
                    var pinIDs = pins.Select(x => x.PinToBeEdit).ToList();
                    alreadyUsed.ForEach(x =>
                    {
                        if (pinIDs.Any(y => y.Contains(x.PinId))) pinIDs.RemoveAll(y => y.Contains(x.PinId));
                    });
                    foreach (var pin in pinIDs)
                    {
                        PinInfoPtResponseHandler pinInfoPtResponseHandler = null;
                        JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                        if (JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser)
                            pinInfoPtResponseHandler = BrowserManager.SearchByCustomPin(JobProcess.DominatorAccountModel, pin,
                                JobProcess.JobCancellationTokenSource);
                        else
                            pinInfoPtResponseHandler = PinFunction.GetPinDetails(pin, JobProcess.DominatorAccountModel);
                        var savedPinDetails =
                            editPinModel.PinDetails.FirstOrDefault(x =>
                                x.PinToBeEdit.Contains(pinInfoPtResponseHandler.PinId));
                        if (savedPinDetails != null)
                        {
                            if (!string.IsNullOrEmpty(savedPinDetails.PinDescription))
                                pinInfoPtResponseHandler.Description = savedPinDetails.PinDescription;
                            if (!string.IsNullOrEmpty(savedPinDetails.Board))
                                pinInfoPtResponseHandler.BoardId = savedPinDetails.Board;
                            if (!string.IsNullOrEmpty(savedPinDetails.Title))
                                pinInfoPtResponseHandler.PinName = savedPinDetails.Title;
                            if (!string.IsNullOrEmpty(savedPinDetails.WebsiteUrl))
                                pinInfoPtResponseHandler.PinWebUrl = savedPinDetails.WebsiteUrl;
                            if (!string.IsNullOrEmpty(savedPinDetails.Section))
                                pinInfoPtResponseHandler.Section = savedPinDetails.Section;
                            if (!string.IsNullOrEmpty(savedPinDetails.Board))
                                pinInfoPtResponseHandler.BoardName = savedPinDetails.Board;
                        }                        
                        StartFinalProcess(ref jobProcessResult, pinInfoPtResponseHandler, queryInfo);
                    }

                    jobProcessResult.HasNoResult = true;
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