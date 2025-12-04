using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using CommonServiceLocator;
using ThreadUtils;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.LdTables.Account;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Process.JobLimits;
using DominatorHouseCore.Utility;
using LinkedDominatorCore.Factories;
using LinkedDominatorCore.LDLibrary.DAL;
using LinkedDominatorCore.LDModel;
using LinkedDominatorCore.LDModel.Messenger;
using LinkedDominatorCore.LDUtility;
using LinkedDominatorCore.Request;
using LinkedDominatorCore.Utility;
using EmbeddedBrowser;
using System.Diagnostics;
using CefSharp;
using HtmlAgilityPack;
using System.Linq;
using System.Threading;
using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.Enums;
using System.IO;
using System.Text;

namespace LinkedDominatorCore.LDLibrary.MessengerProcesses
{
    public class SendMessageToNewConnectionProcess : LDJobProcessInteracted<
        InteractedUsers>
    {
        private readonly IDelayService _delayService;
        private readonly ILdFunctions _ldFunctions;
        private BrowserWindow _browserWindow;
        private bool _isScrolledProfile;
        private BrowserAutomationExtension automationExtension;
        private int failedConnectionCount;
        private LdDataHelper dataHelper = LdDataHelper.GetInstance;
        private ILdLogInProcess ldLogInProcess;
        public SendMessageToNewConnectionProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IDbGlobalService globalService,
            IExecutionLimitsManager executionLimitsManager, ILdQueryScraperFactory queryScraperFactory,
            ILdHttpHelper ldHttpHelper, ILdLogInProcess logInProcess,
            ILdFunctionFactory ldFunctionFactory, IDbInsertionHelper dbInsertionHelper, IDelayService delayService)
            : base(processScopeModel, accountServiceScoped, globalService, executionLimitsManager, queryScraperFactory,
                ldHttpHelper, logInProcess, dbInsertionHelper)
        {
            SendMessageToNewConnectionModel =
                processScopeModel.GetActivitySettingsAs<SendMessageToNewConnectionModel>();
            _ldFunctions = ldFunctionFactory.LdFunctions;
            _delayService = delayService;
            ldLogInProcess = logInProcess;
        }

        public SendMessageToNewConnectionModel SendMessageToNewConnectionModel { get; set; }
        private string CurrentProfileUrl { get; set; }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            JobProcessResult jobProcessResult = null;
            #region Send Message To New Connections Process.
            try
            {
                jobProcessResult = new JobProcessResult();
                var objLinkedinUser = (LinkedinUser)scrapeResult.ResultUser;

                #region Filters After Visiting Profile

                try
                {
                    if (LdUserFilterProcess.IsUserFilterActive(SendMessageToNewConnectionModel.LDUserFilterModel)
                        && !LdUserFilterProcess.GetFilterStatus(objLinkedinUser.ProfileUrl,
                            SendMessageToNewConnectionModel.LDUserFilterModel, _ldFunctions))
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                            "[ " + objLinkedinUser.FullName + " ] is not a valid user according to the filter.");
                        jobProcessResult.IsProcessSuceessfull = false;
                        return jobProcessResult;
                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                #endregion

                #region View Profile Started
                if (SendMessageToNewConnectionModel.IsViewProfileUsingEmbeddedBrowser)
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                    "Started Viewing Profile");
                    var stopWatch = new Stopwatch();
                    stopWatch.Start();
                    NavigateProfile(objLinkedinUser);
                    while (!_isScrolledProfile && stopWatch.Elapsed.Minutes < 2)
                    {
                        JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                        _delayService.ThreadSleep(5000);
                    }
                    stopWatch.Stop();
                    if (!automationExtension.GetCurrentAddress().Contains("search/results"))
                    {
                        _browserWindow.Browser.Back();
                        _delayService.ThreadSleep(2000);
                    }
                    var delay = SendMessageToNewConnectionModel.DelayBetweenViewProfileBeforeMessage.GetRandom();
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                        DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType, $"Message will send  after {delay} seconds");
                    _delayService.ThreadSleep(delay * 1000);
                }
                #endregion

                #region  Variables Initializations

                var message = string.Empty;
                var finalMessage = string.Empty;
                var imageSource = string.Empty;


                var sendMessageToNewConnectionResponse = string.Empty;

                #endregion

                #region Sender's Variables Initializtions

                var fromFullName = string.Empty;
                var fromFirstName = string.Empty;
                var fromLastName = string.Empty;
                var fromcompanyname = string.Empty;

                #endregion

                #region Recipient's Variables Initializations

                var firstName = string.Empty;
                var lastName = string.Empty;
                var fullName = string.Empty;
                var companyname = string.Empty;
                #endregion

                GlobusLogHelper.log.Info(Log.StartedActivity, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, "send message to new connection",
                    objLinkedinUser.FullName);

                #region Sender's Details

                //FromFullName


                #region FromFirstName

                try
                {
                    fromFullName = Regex.Unescape(Regex.Replace(DominatorAccountModel?.AccountBaseModel?.UserFullName,
                        "\\\\([^u])", "\\\\$1"));
                    fromFirstName = Regex.Split(fromFullName, " ")[0];
                    fromLastName = Regex.Split(fromFullName, " ")[1];
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                #endregion

                #region fromcompanyname
                //if sender company name is null then by default it will take sender occupation
                try
                {
                    var companynameandoccupation = Regex.Split(DominatorAccountModel.ExtraParameters.Keys.Contains("Occupation") ? DominatorAccountModel.ExtraParameters["Occupation"] : "", " at");
                    if (companynameandoccupation.Count() == 2)
                        fromcompanyname = companynameandoccupation[1];
                    else
                        fromcompanyname = companynameandoccupation[0];
                }
                catch (Exception)
                {
                }
                #endregion

                #endregion

                #region Recipent Details

                try
                {
                    #region SplittedFullName

                    fullName = objLinkedinUser?.FullName;
                    var splitedFullName = Regex.Split(fullName, " ");

                    #endregion

                    if (splitedFullName.Length > 2)
                    {
                        firstName = splitedFullName[0];
                        lastName = splitedFullName[1] + " " + splitedFullName[2];
                    }
                    else
                    {
                        firstName = splitedFullName[0];
                        lastName = " " + splitedFullName[1];
                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
                #region company name
                try
                {
                    var occupationandcompany = Regex.Split(objLinkedinUser.Occupation, " at");
                    if (occupationandcompany.Count() == 2)
                        companyname = occupationandcompany[1];
                    //if it tagged company name present then only hit reponse to avoid blocking issue
                    if (string.IsNullOrEmpty(companyname) &&
                       (SendMessageToNewConnectionModel.Message.Contains("<Company Name>") || SendMessageToNewConnectionModel.Message.Contains("<company name>")))
                    {
                        var res = _ldFunctions.GetInnerHttpHelper().GetRequest($"https://www.linkedin.com/voyager/api/identity/dash/profiles?q=memberIdentity&memberIdentity={objLinkedinUser.ProfileId}&decorationId=com.linkedin.voyager.dash.deco.identity.profile.FullProfileWithEntities-35");
                        JsonHandler json = new JsonHandler(res.Response);
                        companyname = json.GetElementValue("elements", 0, "profilePositionGroups", "elements", 0, "companyName");
                    }
                }
                catch (Exception)
                {
                }

                #endregion


                #endregion

                #region IsChkSpintaxChecked

                try
                {
                    if (SendMessageToNewConnectionModel.LstDisplayManageMessagesModel.Count > 0)
                        message = GetRandomMessage(objLinkedinUser.SelectedMessageFilter);
                }
                catch (Exception ex)
                {
                    message = SendMessageToNewConnectionModel.Message;
                    ex.DebugLog();
                }

                try
                {
                    finalMessage = Regex.Split(message, "<:>")[0];
                    imageSource = Regex.Split(message, "<:>")[1];
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
                if (SendMessageToNewConnectionModel.IsChkSpintaxChecked)
                {
                    var lstMessages = new List<string>();
                    try
                    {
                        lstMessages = SpinTexHelper.GetSpinMessageCollection(message);
                        lstMessages.Shuffle();
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                    try
                    {
                        finalMessage =
                            lstMessages[new RandomNumberGenerator().GenerateRandom(0, lstMessages.Count - 1)];
                    }
                    catch (Exception ex)
                    {
                        finalMessage = message;
                        ex.DebugLog();
                    }
                }
                #endregion

                #region IsChkTagChecked

                try
                {
                    if (SendMessageToNewConnectionModel.IsChkTagChecked)
                    {
                        #region FinalMessage With Recipient's Tags

                        finalMessage = finalMessage.Replace("<First Name>", firstName);
                        finalMessage = finalMessage.Replace("<First name>", firstName);
                        finalMessage = finalMessage.Replace("<first Name>", firstName);
                        finalMessage = finalMessage.Replace("<first name>", firstName);

                        finalMessage = finalMessage.Replace("<Last Name>", lastName);
                        finalMessage = finalMessage.Replace("<Last name>", lastName);
                        finalMessage = finalMessage.Replace("<last Name>", lastName);
                        finalMessage = finalMessage.Replace("<last name>", lastName);

                        finalMessage = finalMessage.Replace("<Full Name>", fullName);
                        finalMessage = finalMessage.Replace("<Full name>", fullName);
                        finalMessage = finalMessage.Replace("<full Name>", fullName);
                        finalMessage = finalMessage.Replace("<full name>", fullName);

                        finalMessage = finalMessage.Replace("<Company Name>", companyname);
                        finalMessage = finalMessage.Replace("<company Name>", companyname);
                        finalMessage = finalMessage.Replace("<Company name>", companyname);
                        finalMessage = finalMessage.Replace("<company name>", companyname);


                        #endregion

                        #region FinalMessage With Sender's Tags

                        finalMessage = finalMessage.Replace("<From First Name>", fromFirstName);
                        finalMessage = finalMessage.Replace("<From first name>", fromFirstName);
                        finalMessage = finalMessage.Replace("<from first name>", fromFirstName);

                        finalMessage = finalMessage.Replace("<From Last Name>", fromLastName);
                        finalMessage = finalMessage.Replace("<From last name>", fromLastName);
                        finalMessage = finalMessage.Replace("<from last name>", fromLastName);

                        finalMessage = finalMessage.Replace("<From Full Name>", fromFullName);
                        finalMessage = finalMessage.Replace("<From full name>", fromFullName);
                        finalMessage = finalMessage.Replace("<from full name>", fromFullName);

                        finalMessage = finalMessage.Replace("<From Company Name>", fromcompanyname);
                        finalMessage = finalMessage.Replace("<From company name>", fromcompanyname);
                        finalMessage = finalMessage.Replace("< from company name>", fromcompanyname);

                        #endregion
                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                #endregion



                //skip those user who already conversation
                //sometimes conversation happen with another module or outside software 
                if (SendMessageToNewConnectionModel.IsSkipUserAlreadyRecievedMessageFromOutsideSoftware)
                {
                    var conversation = new FilterMessage(_delayService);
                    var isMessageFiltered = _ldFunctions.IsBrowser ? conversation.CheckConversationFromBrowser(_ldFunctions,
                        objLinkedinUser, finalMessage,DominatorAccountModel, out _) : conversation.CheckConversation(_ldFunctions,
                        objLinkedinUser, finalMessage,DominatorAccountModel, out _);
                    if (isMessageFiltered)
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage,
                               DominatorAccountModel.AccountBaseModel.AccountNetwork,
                               DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                               "already sent message to " + objLinkedinUser.FullName + " From OutSide Software");
                        jobProcessResult.IsProcessCompleted = false;
                        return jobProcessResult;
                    }
                }
                #region IsMessageFiltered 
                var genericFileManager = InstanceProvider.GetInstance<IGenericFileManager>();
                var linkedinConfig =
                    genericFileManager.GetModel<LinkedInModel>(ConstantVariable.GetOtherLinkedInSettingsFile());

                if (linkedinConfig.IsFilterDuplicateMessageByCheckingConversationsHistory)
                {
                    var objFilterMessage = new FilterMessage(_delayService);
                    var isMessageFiltered = objFilterMessage.FilterMessageFromConversationHistory(_ldFunctions,
                        objLinkedinUser, DominatorAccountModel, ActivityType, finalMessage);
                    JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    if (isMessageFiltered)
                    {
                        jobProcessResult.IsProcessCompleted = false;
                        DelayBeforeNextActivity();
                        return jobProcessResult;
                    }
                }

                #endregion

                if (string.IsNullOrWhiteSpace(finalMessage))
                {
                    GlobusLogHelper.log.Info(Log.ActivityFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, "send message to new connection",
                        objLinkedinUser.FullName,
                        "Not found suitable message please, try sending message with creating new Campaign.");
                    return jobProcessResult;
                }

                sendMessageToNewConnectionResponse = IsBrowser
                    ? _ldFunctions.UploadImageAndGetContentIdForMessaging(objLinkedinUser.ProfileUrl, string.IsNullOrEmpty(imageSource) ? null : new FileInfo(imageSource),
                        finalMessage)
                    : SendMessageToNewConnectionResponse(imageSource, objLinkedinUser, jobProcessResult, ref finalMessage, sendMessageToNewConnectionResponse);


                #region Send Message
                if (string.IsNullOrEmpty(sendMessageToNewConnectionResponse))
                {
                    sendMessageToNewConnectionResponse = "failed";
                }
                if (sendMessageToNewConnectionResponse.Contains("{\"value\":{\"createdAt\":"))
                {
                    GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                        DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, "send message to new connection",
                        objLinkedinUser.FullName);

                    IncrementCounters();
                    DbInsertionHelper.SendMessageToNewConnection(scrapeResult, objLinkedinUser, finalMessage);
                    jobProcessResult.IsProcessSuceessfull = true;
                }
                else
                {
                    ++failedConnectionCount;
                    GlobusLogHelper.log.Info(Log.ActivityFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, "send message to new connection",
                        objLinkedinUser.FullName, "");
                    jobProcessResult.IsProcessSuceessfull = false;
                }

                if (SendMessageToNewConnectionModel.IsStopSendMessageOnFailed &&
                        SendMessageToNewConnectionModel.StopSendMessageOnCount <= failedConnectionCount)
                {
                    var dominatorScheduler = InstanceProvider.GetInstance<IDominatorScheduler>();
                    dominatorScheduler.ChangeAccountsRunningStatus(false, AccountId, ActivityType);
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                        DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                        "LangKeyStoppingActivityReachedStopActivityOnContinuousFailsCount"
                            .FromResourceDictionary());
                    return JobProcessResult;
                }
                #endregion

                DelayBeforeNextActivity();
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException("Operation Cancelled!");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            #endregion
            return jobProcessResult;
        }

        private void NavigateProfile(LinkedinUser objLinkedinUser)
        {
            #region  Navigate To Connection's Profile

            try
            {
                //tryin g to view profile
                if (_browserWindow == null)
                {
                    LDAccountsBrowserDetails.GetInstance().AccountBrowserCollections
                        .TryGetValue(DominatorAccountModel.UserName, out _browserWindow);
                    if (_browserWindow == null) return;
                    automationExtension = new BrowserAutomationExtension(_browserWindow);
                }

                GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                    "Please wait while viewing profile.");
                var url = _browserWindow.CurrentUrl();
                if (url != objLinkedinUser.ProfileUrl)
                    automationExtension.LoadPageUrlAndWait(objLinkedinUser.ProfileUrl, 10);
                //if first page than we scrolling down to load whole page
                url = _browserWindow.CurrentUrl();
                var isPresentProfileId = url.Contains(objLinkedinUser.PublicIdentifier)
                                         || automationExtension.LoadAndMouseClick("a", AttributeIdentifierType.Id, 5,
                                             objLinkedinUser.PublicIdentifier);

                if (isPresentProfileId)
                {
                    CurrentProfileUrl = automationExtension.GetCurrentAddress();
                    _delayService.ThreadSleep(8000);
                    ClickAndExpandForNormalViewProfile();
                    _isScrolledProfile = true;
                    JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                }
            }
            catch (OperationCanceledException)
            {
                CloseBrowser();
                throw new OperationCanceledException("Operation Cancelled!");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                _isScrolledProfile = true;
            }

            #endregion
        }

        private void ClickAndExpandForNormalViewProfile()
        {
            var htmlData = "";
            var htmlDoc = new HtmlDocument();

            try
            {
                _browserWindow.Browser.GetSourceAsync().ContinueWith(taskHtml => htmlData = taskHtml.Result);
                ScrollWindow(true);
                ScrollWindow();

                htmlData = _browserWindow.Browser.GetSourceAsync().Result;
                htmlDoc.LoadHtml(htmlData);
                var allButtonElements = htmlDoc.DocumentNode.SelectNodes("//button");
                var allLinks = htmlDoc.DocumentNode.SelectNodes("//a");
                foreach (var buttonElement in allButtonElements)
                    try
                    {
                        JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                        #region Click Show more

                        if (buttonElement.InnerText.Contains("Show") && buttonElement.InnerText.Contains("more"))
                            try
                            {
                                var val = buttonElement.Attributes["class"].Value;

                                if (!allLinks.Any(x => x.OuterHtml.Contains(val)))
                                {
                                    _browserWindow.Browser.ExecuteScriptAsync(string.Format(LDClassesConstant.ScriptConstant.ScriptGetElementsByClassNameToClick,val,0));
                                    _delayService.ThreadSleep(5000);
                                    break;
                                }

                                if (allLinks.Any(x => x.InnerText.Contains("See all")) &&
                                    !allLinks.Any(x => x.InnerText.Contains("See all articles")) &&
                                    !allLinks.Any(x => x.InnerText.Contains("See all activity")))
                                {
                                    _browserWindow.Browser.ExecuteScriptAsync(string.Format(LDClassesConstant.ScriptConstant.ScriptGetElementsByClassNameToClick, val, 1));
                                    SplitDelay(JobCancellationTokenSource.Token, 1);
                                    _browserWindow.Browser.ExecuteScriptAsync(string.Format(LDClassesConstant.ScriptConstant.ScriptGetElementsByClassNameToClick, val, 2));
                                    _delayService.ThreadSleep(5000);
                                }
                                else
                                {
                                    _browserWindow.Browser.ExecuteScriptAsync(string.Format(LDClassesConstant.ScriptConstant.ScriptGetElementsByClassNameToClick, val, 2));
                                    SplitDelay(JobCancellationTokenSource.Token, 1);
                                    break;
                                }
                            }
                            catch (ArgumentNullException ex)
                            {
                                ex.DebugLog("ArgumentNullException Click Show more");
                            }
                            catch (Exception ex)
                            {
                                ex.DebugLog("Exception Click Show more");
                            }

                        #endregion
                    }
                    catch (OperationCanceledException)
                    {
                        CloseBrowser();
                        throw new OperationCanceledException("Operation Cancelled!");
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }


                _browserWindow.Browser.ExecuteScriptAsync(string.Format(LDClassesConstant.ScriptConstant.ScriptGetElementsByClassNameToClick, "pv-profile-section__card-action-bar artdeco-container-card-action-bar ember-view", 0));
                _delayService.ThreadSleep(2000);
                _browserWindow.Browser.ExecuteScriptAsync(string.Format(LDClassesConstant.ScriptConstant.ScriptGetElementsByClassNameToClick, "pv-profile-detail__nav-link t-14 t-black--light t-bold ember-view", 1));
                _delayService.ThreadSleep(2000);

                _browserWindow.Browser.ExecuteScriptAsync(string.Format(LDClassesConstant.ScriptConstant.ScriptGetElementsByClassNameToClick, "pv-profile-detail__nav-link t-14 t-black--light t-bold ember-view", 2));
                _delayService.ThreadSleep(2000);
                _browserWindow.Browser.ExecuteScriptAsync(string.Format(LDClassesConstant.ScriptConstant.ScriptGetElementsByClassNameToClick, "artdeco-modal__dismiss artdeco-button artdeco-button--circle artdeco-button--muted artdeco-button--2 artdeco-button--tertiary ember-view", 0));
                _delayService.ThreadSleep(1000);


                ScrollWindow(true);
                ScrollWindow();
                ScrollWindow(true);
                htmlData = _browserWindow.Browser.GetSourceAsync().Result;

                _delayService.ThreadSleep(2000);
                _browserWindow.Browser.ExecuteScriptAsync(string.Format(LDClassesConstant.ScriptConstant.ScriptGetElementsByClassNameToClick, "t-16 t-bold", 0));
                _delayService.ThreadSleep(2000);
                _browserWindow.Browser.Back();
                _delayService.ThreadSleep(5000);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

        }
        public void SplitDelay(CancellationToken cancellationToken, int runTimes)
        {
            try
            {
                for (var i = 0; i < runTimes; i++)
                {
                    _delayService.ThreadSleep(5000);
                    cancellationToken.ThrowIfCancellationRequested();
                }
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException("Operation Cancelled!");
            }
            catch (Exception exception)
            {
                exception.DebugLog();
            }
        }
        private void ScrollWindow(bool isDown = false)
        {
            var down = isDown ? 100 : -100;

            for (var i = 1; i < 20; i++)
            {
                _delayService.ThreadSleep(100);
                _browserWindow.Browser.ExecuteScriptAsync(string.Format(LDClassesConstant.ScriptConstant.ScrollWindowByXXPixel,0,down));
            }
        }

        private void CloseBrowser()
        {
            LDAccountsBrowserDetails.CloseBrowser(DominatorAccountModel);
        }
        private string SendMessageToNewConnectionResponse(string imageSource, LinkedinUser objLinkedinUser, JobProcessResult jobProcessResult,
            ref string finalMessage, string sendMessageToConnectionResponse)
        {
            if (string.IsNullOrEmpty(objLinkedinUser.ProfileId))
            {
                try
                {
                    var url = $"https://www.linkedin.com/voyager/api/identity/dash/profiles?q\u003DmemberIdentity\u0026memberIdentity\u003D{objLinkedinUser.PublicIdentifier}\u0026decorationId\u003Dcom.linkedin.voyager.dash.deco.identity.profile.TopCardSecondaryData-37";
                    var res = _ldFunctions.GetInnerLdHttpHelper().HandleGetResponse(url)?.Response;
                    JsonHandler json = new JsonHandler(res);
                    objLinkedinUser.ProfileId = json.GetElementValue("elements", 0, "entityUrn")?.Replace("urn:li:fsd_profile:", "");
                    if (string.IsNullOrEmpty(objLinkedinUser.ProfileId))
                        objLinkedinUser.ProfileId = json.GetElementValue("elements", 0, "memberRelationship", "entityUrn").Replace("urn:li:fsd_memberRelationship:", "");
                }
                catch (Exception)
                {
                }
            }
            var postString = string.Empty;
            var TimemilliSecond = DateTime.UtcNow.GetCurrentEpochTimeMilliSeconds();
            var actionUrl = $"https://www.linkedin.com/voyager/api/messaging/conversations?action=create&nc={TimemilliSecond}";
            if (!string.IsNullOrEmpty(imageSource))
            {
                Tuple<bool, string> imageUploadStatusAndPostData = null;
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                imageUploadStatusAndPostData = GetPostDataToSendMessage(_ldFunctions,
                    objLinkedinUser.ProfileId, finalMessage, imageSource);
                if (!imageUploadStatusAndPostData.Item1)

                {
                    DelayBeforeNextActivity();
                    jobProcessResult.IsProcessSuceessfull = false;
                    {
                        return sendMessageToConnectionResponse;
                    }
                }

                postString = imageUploadStatusAndPostData.Item2;
                finalMessage = $"{finalMessage}<:>{imageSource}";
            }
            else
            {
                #region PostSting and Requirements

                try
                {
                    var originToken = Utilities.GetGuid();
                    finalMessage = finalMessage.Replace("\r\n", "\\n").Replace("\"", "\\\"").Trim();
                    finalMessage = finalMessage.Replace(Environment.NewLine, "").Trim();
                    postString = "{\"conversationCreate\":{\"eventCreate\":{\"value\":{\"com.linkedin.voyager.messaging.create.MessageCreate\":{\"body\":\"" +
                        finalMessage + "\"}}},\"recipients\":[\"" + objLinkedinUser.ProfileId
                       + "\"]}}";
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                #endregion
            }
            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            var sendMessageToNewConnectionResponse =
                _ldFunctions.GetInnerLdHttpHelper().PostRequest(actionUrl,Encoding.UTF8.GetBytes(postString))?.Response;
            return sendMessageToNewConnectionResponse;
        }

        private Tuple<bool, string> GetPostDataToSendMessage(ILdFunctions objLdFunctions, string profileId,
           string finalMessage, string imageSource)
        {
            try
            {
                Tuple<bool, string> imageUploadStatusAndPostData = null;
                var mediaUrn = string.Empty;
                var mediaId = string.Empty;
                var singleUploadUrl = string.Empty;

                #region PostData And PostDataResponse For Message Sending

                #region PhotoUploading

                var imageUploadUrl = LdConstants.GetLDMediaUploadAPI;
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                var objFileInfo = new FileInfo(imageSource);
                var imageUploadResponse =
                    objLdFunctions.UploadImageAndGetContentIdForMessaging(imageUploadUrl, objFileInfo);
                if (imageUploadResponse == null || !imageUploadResponse.Contains("{\"value\":{\"urn\":\""))
                {
                    imageUploadStatusAndPostData = new Tuple<bool, string>(false, "");
                    GlobusLogHelper.log.Info(Log.ActivityFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType.BroadcastMessages,
                        "failed to upload Image");
                    return imageUploadStatusAndPostData;
                }
                mediaUrn = Utils.GetBetween(imageUploadResponse, "urn\":\"", "\"");
                singleUploadUrl = Utils.GetBetween(imageUploadResponse, "singleUploadUrl\":\"", "\"");

                #endregion

                var referer = "https://www.linkedin.com/messaging/compose/";
                var singleUploadResponse =
                    objLdFunctions.GetSingleUploadResponse(singleUploadUrl, objFileInfo, referer);

                if (singleUploadResponse == null)
                {
                    imageUploadStatusAndPostData = new Tuple<bool, string>(true, "");
                    return imageUploadStatusAndPostData;
                }

                if (singleUploadResponse != string.Empty)
                {
                    imageUploadStatusAndPostData = new Tuple<bool, string>(true, "");
                    return imageUploadStatusAndPostData;
                }

                var contentType = Utils.GetMediaType(objFileInfo.Extension);
                mediaId = mediaUrn.Split(':').Last();
                var postData =
                    "{\"conversationCreate\":{\"eventCreate\":{\"value\":{\"com.linkedin.voyager.messaging.create.MessageCreate\":{\"body\":\"" +
                    finalMessage + "\",\"attachments\":[{\"id\":\"" + mediaUrn + "\",\"originalId\":\"" + mediaUrn +
                    "\",\"name\":\"" + objFileInfo.Name + "\",\"byteSize\":" + objFileInfo.Length +
                    ",\"mediaType\":\"" + contentType +
                    "\",\"reference\":{\"string\":\"blob:https://www.linkedin.com/" + mediaId +
                    "\"}}]}}},\"recipients\":[\"" + profileId +
                    "\"],\"subtype\":\"MEMBER_TO_MEMBER\"},\"keyVersion\":\"LEGACY_INBOX\"}";
                imageUploadStatusAndPostData = new Tuple<bool, string>(true, postData);
                return imageUploadStatusAndPostData;

                #endregion
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException("Operation Cancelled!");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return new Tuple<bool, string>(false, "");
            }
        }

        public string GetRandomMessage(string selectedSource)
        {
            try
            {
                string messageContent;
                var imageSource = string.Empty;

                var message = "";
                if (selectedSource == null)
                {
                    try
                    {
                        var MessageList = SendMessageToNewConnectionModel.LstDisplayManageMessagesModel
                        .Where(x => x.SelectedQuery.Any(y =>
                            y.Content.QueryValue.ToString() == "All"))
                        ?.Select(x => x.MessagesText)?.ToList();
                        message = MessageList?.Count > 0 ? MessageList?.GetRandomItem()
                            : SendMessageToNewConnectionModel.LstDisplayManageMessagesModel.FirstOrDefault()?.MessagesText;
                        var ImageList = SendMessageToNewConnectionModel.LstDisplayManageMessagesModel
                                        .Where(x => x.SelectedQuery.Any(y =>
                                            y.Content.QueryValue.ToString() == "All"))
                                        ?.Select(x => x.MediaPath)?.ToList();
                        imageSource = ImageList?.Count > 0 ? ImageList?.GetRandomItem()
                            : SendMessageToNewConnectionModel.LstDisplayManageMessagesModel.FirstOrDefault()?.MediaPath;
                    }
                    catch (Exception ex)
                    {
                        ex?.DebugLog();
                    }
                }
                else
                {
                    
                    try
                    {
                        var MessageList = SendMessageToNewConnectionModel.LstDisplayManageMessagesModel
                        .Where(x => x.SelectedQuery.Any(y => y.Content.QueryValue.ToString() == selectedSource))
                        ?.Select(x => x.MessagesText)?.ToList();
                        message = MessageList?.Count > 0 ?MessageList?.GetRandomItem() :
                            SendMessageToNewConnectionModel.LstDisplayManageMessagesModel.FirstOrDefault()?.MessagesText;
                        var ImageList = SendMessageToNewConnectionModel.LstDisplayManageMessagesModel
                            .Where(x => x.SelectedQuery.Any(y => y.Content.QueryValue.ToString() == selectedSource))
                            ?.Select(x => x.MediaPath)?.ToList();
                        imageSource = ImageList?.Count > 0 ? ImageList?.GetRandomItem()
                            : SendMessageToNewConnectionModel.LstDisplayManageMessagesModel.FirstOrDefault()?.MediaPath;
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                }

                var lstMessage = new List<string>();
                try
                {
                    lstMessage = Regex.Split(message, "<End>").ToList();
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                if (lstMessage.Count > 1)
                    message = lstMessage.GetRandomItem();

                if (!string.IsNullOrEmpty(imageSource))
                    messageContent = message + "<:>" + imageSource;
                else
                    messageContent = message;
                return IsBrowser?messageContent:dataHelper.ReplaceSpecialCharacter(messageContent);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

    }
}