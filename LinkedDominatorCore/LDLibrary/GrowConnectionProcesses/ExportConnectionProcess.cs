using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using CefSharp;
using CommonServiceLocator;
using ThreadUtils;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.LdTables.Account;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Process.JobLimits;
using DominatorHouseCore.Utility;
using EmbeddedBrowser;
using HtmlAgilityPack;
using LinkedDominatorCore.Factories;
using LinkedDominatorCore.LDLibrary.DAL;
using LinkedDominatorCore.LDModel;
using LinkedDominatorCore.LDModel.GrowConnection;
using LinkedDominatorCore.LDUtility;
using LinkedDominatorCore.Request;
using LinkedDominatorCore.Utility;
using MahApps.Metro.Controls;

namespace LinkedDominatorCore.LDLibrary.GrowConnectionProcesses
{
    public class ExportConnectionProcess : LDJobProcessInteracted<
        InteractedUsers>
    {
        private readonly IDelayService _delayService;
        private readonly ILdFunctions _ldFunctions;
        private BrowserWindow _browserWindow;
        private int _currentScrollExpandThreadCount;
        private BrowserAutomationExtension automationExtension;


        private int _currentThreadCount;

        public ExportConnectionProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IDbGlobalService globalService,
            IExecutionLimitsManager executionLimitsManager, ILdQueryScraperFactory queryScraperFactory,
            ILdHttpHelper ldHttpHelper, ILdLogInProcess logInProcess,
            ILdFunctionFactory ldFunctionFactory, IDbInsertionHelper dbInsertionHelper, IDelayService delayService)
            : base(processScopeModel, accountServiceScoped, globalService, executionLimitsManager, queryScraperFactory,
                ldHttpHelper, logInProcess, dbInsertionHelper)
        {
            ExportConnectionModel = processScopeModel.GetActivitySettingsAs<ExportConnectionModel>();
            _ldFunctions = ldFunctionFactory.LdFunctions;
            _delayService = delayService;
        }


        public ExportConnectionModel ExportConnectionModel { get; set; }
        public ScrapeResultNew ScrapeResult { get; set; }

        public LinkedinUser ObjLinkedinUser { get; set; }

        //public List<CampaignDetails> CampaignDetails { get; set; }
        public string FolderPath { get; set; }

        private string ContactInfoHtml { get; set; }
        private CampaignDetails Campaign { get; set; }
        public bool IsFooterReached { get; set; }
        public bool IsExportTimeCompleted { get; set; }
        public bool IsDownloadPdfSuccess { get; set; }

        public string FilePathForPdf { get; set; }
        public bool IsExpandedAllSeeMore { get; set; }
        private string CurrentProfileUrl { get; set; }

        private LinkedInModel LinkedInModel { get; set; }

        public Dictionary<string, string> DicShowMoreButton { get; set; }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            JobProcessResult jobProcessResult = null;
            #region Export Connection Process.
            var isSuccess = false;

            try
            {
                ScrapeResult = scrapeResult;
                IsDownloadPdfSuccess = false;
                FilePathForPdf = string.Empty;
                IsExportTimeCompleted = false;
                IsFooterReached = false;
                IsExpandedAllSeeMore = false;

                DicShowMoreButton = new Dictionary<string, string>();

                ObjLinkedinUser = (LinkedinUser) scrapeResult.ResultUser;
                jobProcessResult = new JobProcessResult();
                var ldGlobalInteractionDetails = InstanceProvider.GetInstance<IGlobalInteractionDetails>();
                var genericFileManager = InstanceProvider.GetInstance<IGenericFileManager>();
                LinkedInModel =
                    genericFileManager.GetModel<LinkedInModel>(ConstantVariable.GetOtherLinkedInSettingsFile());

                if (LinkedInModel.IsEnableExportingHTMLOfDifferentConnections)
                {
                    #region Check in LDGlobalInteractionDetails

                    try
                    {
                        ldGlobalInteractionDetails.AddInteractedData(SocialNetworks.LinkedIn, ActivityType,
                            ObjLinkedinUser.ProfileUrl);
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                        jobProcessResult.IsProcessCompleted = false;
                        return jobProcessResult;
                    }

                    #endregion
                }

                // filter if not a first connection
                if (!new GetDetailedUserInfo(_delayService).IsFirstConnection(DominatorAccountModel, DbAccountService,
                    ObjLinkedinUser, ActivityType, _ldFunctions))
                {
                    RemoveNonInteractedUser(ObjLinkedinUser, ldGlobalInteractionDetails);
                    return jobProcessResult;
                }


                #region FolderPath

                try
                {
                    var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    string campaignName;
                    if (string.IsNullOrEmpty(Campaign?.CampaignName?.Trim()))
                    {
                        var campaignFileManager = InstanceProvider.GetInstance<ICampaignsFileManager>();
                        Campaign = campaignFileManager.GetCampaignById(CampaignId);
                        campaignName = Campaign?.CampaignName;
                    }
                    else
                    {
                        campaignName = Campaign.CampaignName;
                    }

                    if (string.IsNullOrEmpty(campaignName))
                        FolderPath = documentsPath + "\\LinkedinHtmlExport\\" +
                                     DominatorAccountModel.AccountBaseModel.UserName.Replace(".", "");
                    else
                        FolderPath = documentsPath + "\\LinkedinHtmlExport\\" +
                                     campaignName.Replace("-", "_").Replace("/", "_").Replace(":", "_") + "\\" +
                                     DominatorAccountModel.AccountBaseModel.UserName.Replace(".", "");

                    if (!Directory.Exists(FolderPath))
                        Directory.CreateDirectory(FolderPath);
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                #endregion

                string contactInfo;
                string contactInfoPath;
                var htmlFilePath = GetExportFilePath(_ldFunctions, out contactInfo, out contactInfoPath);


                try
                {
                    if (_browserWindow == null)
                        LDAccountsBrowserDetails.GetInstance().AccountBrowserCollections
                            .TryGetValue(DominatorAccountModel.UserName, out _browserWindow);
                    if (_browserWindow == null)
                    {
                        _delayService.ThreadSleep(15000);
                        LDAccountsBrowserDetails.GetInstance().AccountBrowserCollections
                            .TryGetValue(DominatorAccountModel.UserName, out _browserWindow);
                    }

                     automationExtension = new BrowserAutomationExtension(_browserWindow);
                    JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                    #region  Navigate To Connection's Profile

                    try
                    {
                        // trying to export connection
                        GlobusLogHelper.log.Info(Log.StartedActivity,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType, ObjLinkedinUser.FullName);
                        CurrentProfileUrl = ObjLinkedinUser.ProfileUrl;
                        if (_browserWindow != null)
                        {
                            _browserWindow.Browser.Load(ObjLinkedinUser.ProfileUrl);
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                _browserWindow.Browser.FrameLoadEnd += NavigateToLinkedInConnections;
                            });
                            _delayService.ThreadSleep(15000);


                            var minutes = ExportConnectionModel.WaitOnEachProfileBetween.GetRandom();

                            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                            GlobusLogHelper.log.Info(Log.CustomMessage,
                                DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                "to save complete page please wait for " + minutes +
                                " minutes or until scrolled to bottom of the page");
                            var maximumWaitTime = DateTime.Now.AddMinutes(minutes);

                            // waiting main thread until reach footer
                            while (true)
                            {
                                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                                _delayService.ThreadSleep(5000);
                                var text = _browserWindow.Browser.GetTextAsync().Result;
                                if (!text.Contains("Save to PDF") &&
                                    (IsFooterReached || DateTime.Now > maximumWaitTime))
                                {
                                    IsExportTimeCompleted = true;
                                    break;
                                }
                            }

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
                    }

                    #endregion

                    ExpandAllDetailsOfUsers(automationExtension);
                    var completeProfilePageSource = _browserWindow.Browser.GetSourceAsync().Result
                        .Replace("ad-banner-container ember-view", "");

                    JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    var saveAsHtmlSuccess = SaveAsHtmlAlterNative(htmlFilePath, completeProfilePageSource);


                    if (ExportConnectionModel.IsCheckedOnlyContactInfo)
                    {
                        var contactInfoPageSource = "";
                        SeeContactInfo();
                        while (ExportConnectionModel.IsCheckedOnlyContactInfo &&
                               !contactInfoPageSource.Contains("\">Connected</header>"))
                        {
                            _delayService.ThreadSleep(5000);
                            contactInfoPageSource = _browserWindow.Browser.GetSourceAsync().Result;
                            break;
                        }

                        SaveAsHtmlAlterNative(contactInfoPath, contactInfoPageSource);
                    }


                    _delayService.ThreadSleep(5000);
                    JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    // sometimes pdf having issue in clicking save button so, we again try save pdf
                    if (ExportConnectionModel.IsCheckedDownloadPdf && !IsDownloadPdfSuccess)
                        DownloadPdf(automationExtension, ObjLinkedinUser.ProfileUrl);

                    var exceptionMessage = saveAsHtmlSuccess.Item2;
                    isSuccess = saveAsHtmlSuccess.Item1;

                    if (isSuccess)
                    {
                        IncrementCounters();
                        DbInsertionHelper.ExportConnection(scrapeResult, ObjLinkedinUser, contactInfo);
                        jobProcessResult.IsProcessSuceessfull = true;

                         GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType, ObjLinkedinUser.FullName);
                        GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                            "to Path [ " + htmlFilePath + " ]");
                    }
                    else
                    {
                        if (LinkedInModel.IsEnableExportingHTMLOfDifferentConnections)
                            RemoveNonInteractedUser(ObjLinkedinUser, ldGlobalInteractionDetails);
                        GlobusLogHelper.log.Info(Log.ActivityFailed,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType, exceptionMessage);
                        jobProcessResult.IsProcessSuceessfull = false;
                    }

                    JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    DelayBeforeNextActivity();
                }
                catch (OperationCanceledException)
                {
                    CloseBrowser(!isSuccess);
                    throw new OperationCanceledException("Operation Cancelled!");
                }
                catch (AggregateException ae)
                {
                    CloseBrowser(!isSuccess);
                    foreach (var e in ae.InnerExceptions)
                        if (e is TaskCanceledException || e is OperationCanceledException)
                            e.DebugLog("Cancellation requested before task completion!");
                        else
                            e.DebugLog(e.StackTrace + e.Message);
                }
                catch (Exception ex)
                {
                    if (!isSuccess)
                        RemoveNonInteractedUser(ObjLinkedinUser);
                    ex.DebugLog();
                }
            }
            catch (Exception ex)
            {
                if (!isSuccess)
                    RemoveNonInteractedUser(ObjLinkedinUser);
                ex.DebugLog();
            }
            #endregion
            return jobProcessResult;
        }

        private void ExpandAllDetailsOfUsers(BrowserAutomationExtension automationExtension)
        {
            automationExtension.ExecuteScript(string.Format(LDClassesConstant.ScriptConstant.ScriptGetElementsByClassNameToClick, "artdeco-toast-item__dismiss artdeco-button artdeco-button--circle artdeco-button--muted artdeco-button--1 artdeco-button--tertiary ember-view",0));
            var pageresponse = _browserWindow.GetPageSource();

            automationExtension.ScrollWindow(6000, false);

            pageresponse = _browserWindow.GetPageSource();
            if (pageresponse.Contains("pv-profile-section pv-about-section artdeco-card p5 mt4 ember-view"))
            {
                var getAboutSectionDetailsData = HtmlAgilityHelper.GetListInnerHtmlFromClassName(pageresponse,
                    "pv-profile-section pv-about-section artdeco-card p5 mt4 ember-view").FirstOrDefault();
                if (getAboutSectionDetailsData.Contains("lt-line-clamp__more") &&
                    getAboutSectionDetailsData.Contains("see more"))
                    automationExtension.ExecuteScript(string.Format(LDClassesConstant.ScriptConstant.ScriptGetElementsByClassNameToClick, "lt-line-clamp__more",0));
            }
            automationExtension.ScrollWindow(1000);
            var count = 0;
            var getExperienceDetailsData = string.Empty;
            if (pageresponse.Contains("pv-profile-section__section-info section-info pv-profile-section__section-info--has-no-more"))
                getExperienceDetailsData = HtmlAgilityHelper.GetListInnerHtmlFromClassName(pageresponse,
                   "pv-profile-section__section-info section-info pv-profile-section__section-info--has-no-more").First();
            if(pageresponse.Contains("pv-profile-section__section-info section-info pv-profile-section__section-info--has-more"))
                getExperienceDetailsData = HtmlAgilityHelper.GetListInnerHtmlFromClassName(pageresponse,
                    "pv-profile-section__section-info section-info pv-profile-section__section-info--has-more").First();
            
            if (pageresponse.Contains("pv-profile-section__section-info--has-no-more") && getExperienceDetailsData.Count()<=0 && pageresponse.Contains("pv-entity__position-group-pager pv-profile-section__list-item ember-view"))
                 getExperienceDetailsData = HtmlAgilityHelper.GetListInnerHtmlFromClassName(pageresponse, "pv-profile-section experience-section ember-view").First();
           
            var listofExperiencedata = HtmlAgilityHelper.GetListNodesFromClassName(getExperienceDetailsData,
                "pv-entity__position-group-pager pv-profile-section__list-item ember-view");
            foreach (var item in listofExperiencedata)
            {
                var data = item.OuterHtml;
                if (data.Contains("see more") ||
                    data.Contains(
                        "pv-profile-section__see-more-inline pv-profile-section__text-truncate-toggle link link-without-hover-state")
                )
                {
                    automationExtension.ExecuteScript(string.Format(LDClassesConstant.ScriptConstant.ScriptGetElementsByClassNameToClick, "inline-show-more-text__button link",count), 5);

                    count++;
                    automationExtension.ExecuteScript(string.Format(LDClassesConstant.ScriptConstant.ScriptGetElementsByClassNameToClick, "pv-profile-section__see-more-inline pv-profile-section__text-truncate-toggle link link-without-hover-state", 0));
                }
            }

            if (pageresponse.Contains(
                    "pv-profile-section__card-action-bar pv-skills-section__additional-skills artdeco-container-card-action-bar artdeco-button artdeco-button--tertiary artdeco-button--3 artdeco-button--fluid") &&
                !pageresponse.Contains("pv-skill-categories-section__expanded"))
            {
                automationExtension.ScrollWindow();
                automationExtension.ExecuteScript(string.Format(LDClassesConstant.ScriptConstant.ScriptGetElementsByClassNameToClick, "pv-profile-section__card-action-bar pv-skills-section__additional-skills artdeco-container-card-action-bar artdeco-button artdeco-button--tertiary artdeco-button--3 artdeco-button--fluid",0),5);
            }
        }

        private string GetExportFilePath(ILdFunctions lDFunctions, out string contactInfo, out string contactInfoPath)
        {
            #region Initializations required for this method

            var fileName = string.Empty;
            var htmlFilePath = string.Empty;
            var phoneNumber = string.Empty;
            var emailAddress = string.Empty;
            contactInfoPath = string.Empty;
            contactInfo = string.Empty;
            var objGetDetailedUserInfo = new GetDetailedUserInfo(_delayService);

            #endregion

            var personalDetails =
                objGetDetailedUserInfo.GetPersonalDetailsForExportConnection(ObjLinkedinUser.ProfileUrl, lDFunctions,
                    ObjLinkedinUser.ProfileId);

            #region phoneNumber And Emailid

            try
            {
                emailAddress = Regex.Split(personalDetails, "<:>")[1];
                phoneNumber = Regex.Split(personalDetails, "<:>")[2];
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }


            contactInfo = phoneNumber + "<:>" + emailAddress;

            #endregion

            try
            {
                if (ExportConnectionModel.IsFilenameFormatChecked)
                {
                    fileName = GetFilenameFormatForExport(phoneNumber, emailAddress);
                }

                else
                {
                    #region FileName with Default Format FullName_ProfileUrl

                    var replacedConnectionName = ObjLinkedinUser.FullName.Replace("\"", "").Replace("|", "_")
                        .Replace(".", "").Replace("\\", "").Replace(",", "_").Replace("\"", "");
                    if (!string.IsNullOrEmpty(replacedConnectionName))
                        fileName = replacedConnectionName + "_";

                    if (!string.IsNullOrEmpty(ObjLinkedinUser.ProfileUrl))
                    {
                        var connectionUrl = ObjLinkedinUser.ProfileUrl;
                        fileName = fileName + connectionUrl.Replace("https://", "").Replace("/", "__");
                    }

                    #endregion
                }

                contactInfoPath = FolderPath + "\\" + fileName + "_Contact_Info.html";
                htmlFilePath = FolderPath + "\\" + fileName + ".html";
                FilePathForPdf = FolderPath + "\\" + fileName + ".pdf";
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return htmlFilePath;
        }

        private string GetFilenameFormatForExport(string phoneNumber, string emailAddress)
        {
            try
            {
                var fileName = "";
                string firstName;

                var middleName = "";
                var lastName = "";
                var nameArray = Regex.Split(ObjLinkedinUser.FullName, " ");
                if (nameArray.Length >= 3)
                {
                    firstName = Regex.Split(ObjLinkedinUser.FullName, " ")[0].Replace(".", "").Replace("\\", "")
                        .Replace(",", "_").Replace("\"", "");
                    middleName = Regex.Split(ObjLinkedinUser.FullName, " ")[1].Replace(".", "").Replace("\\", "")
                        .Replace(",", "_").Replace("\"", "");
                    foreach (var item in nameArray)
                        if (item != firstName && item != middleName)
                            lastName += item + " ";
                    lastName = lastName.Trim(' ').Replace(".", "").Replace("\\", "").Replace(",", "_")
                        .Replace("\"", "");
                }
                else
                {
                    firstName = Regex.Split(ObjLinkedinUser.FullName, " ")[0].Replace(".", "").Replace("\\", "")
                        .Replace(",", "_").Replace("\"", "");
                    lastName = Regex.Split(ObjLinkedinUser.FullName, " ")[1].Replace(".", "").Replace("\\", "")
                        .Replace(",", "_").Replace("\"", "");
                }

                #region FileName With UserInput Format

                fileName = ExportConnectionModel.FilenameFormat;
                if (!string.IsNullOrEmpty(firstName) && fileName.Contains("{FirstName}"))
                    fileName = fileName.Replace("{FirstName}", firstName);
                else
                    fileName = fileName.Replace("{FirstName}", "");


                if (!string.IsNullOrEmpty(middleName) && fileName.Contains("{MiddleName}"))
                    fileName = fileName.Replace("{MiddleName}", middleName);
                else
                    fileName = fileName.Replace("{MiddleName}", "");


                if (!string.IsNullOrEmpty(lastName) && fileName.Contains("{LastName}"))
                    fileName = fileName.Replace("{LastName}", lastName);
                else
                    fileName = fileName.Replace("{LastName}", "");


                if (!string.IsNullOrEmpty(phoneNumber) && fileName.Contains("{PhoneNumber}"))
                    fileName = fileName.Replace("{PhoneNumber}", phoneNumber);
                else
                    fileName = fileName.Replace("{PhoneNumber}", "");

                if (!string.IsNullOrEmpty(emailAddress) && fileName.Contains("{EmailAddress}"))
                    fileName = fileName.Replace("{EmailAddress}", emailAddress);
                else
                    fileName = fileName.Replace("{EmailAddress}", "");

                if (!string.IsNullOrEmpty(ObjLinkedinUser.ProfileUrl) && fileName.Contains("{ProfileUrl}"))
                {
                    var connectionUrl = ObjLinkedinUser.ProfileUrl;
                    fileName = fileName.Replace("{ProfileUrl}",
                        connectionUrl.Replace("https://", "").Replace("/", "__"));
                }
                else
                {
                    fileName = fileName.Replace("{ProfileUrl}", "");
                }

                if (DominatorAccountModel.AccountBaseModel.UserName.Contains("@") &&
                    fileName.Contains("{FromAccountEmail}"))
                    fileName = fileName.Replace("{FromAccountEmail}", DominatorAccountModel.AccountBaseModel.UserName);


                fileName = fileName.Replace("__", "_");
                var last_char = fileName[fileName.Length - 1].ToString();
                if (last_char == "_")
                    fileName = fileName.Remove(fileName.Length - 1, 1);

                #endregion

                return fileName;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return "";
            }
        }

        private async void NavigateToLinkedInConnections(object sender,
            FrameLoadEndEventArgs loadingStateChangedEventArgs)
        {
            try
            {
                var isLoaded = false;

                try
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        try
                        {
                            isLoaded = loadingStateChangedEventArgs.Frame.IsMain;
                        }
                        catch (Exception e)
                        {
                            e.DebugLog();
                        }
                    });
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                if (!isLoaded)
                    return;
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                if (!string.IsNullOrEmpty(DominatorAccountModel.AccountBaseModel.UserName) &&
                    !string.IsNullOrEmpty(DominatorAccountModel.AccountBaseModel.Password))
                {
                    var html = string.Empty;
                    for (var i = 0; i < 5; i++)
                    {
                        await _browserWindow.Browser.GetSourceAsync().ContinueWith(taskHtml => html = taskHtml.Result);
                        if (!string.IsNullOrEmpty(html))
                            break;
                        _delayService.ThreadSleep(5000);
                    }

                    if (!_browserWindow.Browser.GetMainFrame().IsMain)
                        return;

                    try
                    {
                        ScrollAndExpand();
                        if (IsExportTimeCompleted)
                            // ReSharper disable once RedundantJumpStatement
                            return;
                    }
                    catch (OperationCanceledException)
                    {
                        CloseBrowser();
                        // using cancellation token here might leads to s/w crashing 
                        //  throw new OperationCanceledException("Operation Cancelled!");
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog(ex.StackTrace);
                        CloseBrowser(false);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                CloseBrowser();
                //  throw new OperationCanceledException("Operation Cancelled!");
            }
            catch (AggregateException ae)
            {
                ae.DebugLog();
                CloseBrowser(false);
            }
            catch (Exception ex)
            {
                CloseBrowser(false);
                ex.DebugLog(ex.StackTrace);
            }
        }

        /// <summary>
        ///     here we using close browser and RemoveNonInteractedUser together
        ///     mostly places we close browser on unhandled Exception and profile is not saved properly
        /// </summary>
        /// <param name="isExceptionClose"></param>
        public void CloseBrowser(bool isExceptionClose = true)
        {
            try
            {
                // exception close or sometimes after successful delay pause campaign then no need to remove
                if (isExceptionClose)
                    RemoveNonInteractedUser(ObjLinkedinUser);

                if (_browserWindow != null)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        _browserWindow.Close();
                        _browserWindow.Dispose();
                    });
                    LDAccountsBrowserDetails.GetInstance().AccountBrowserCollections
                        .Remove(DominatorAccountModel.UserName);
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void SeeContactInfo()
        {
            try
            {
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                _browserWindow.Browser.ExecuteScriptAsync("window.scrollTo(0, " + 80 + ")");
                var isSuccess = _browserWindow.Browser
                    .EvaluateScriptAsync(string.Format(LDClassesConstant.ScriptConstant.ScriptGetElementsByClassNameToClick, "pv-top-card-v2-section__entity-name pv-top-card-v2-section__contact-info ml2 t-14 t-black t-bold", 0))
                    .Result.Success;

                if (!isSuccess)
                {
                    var browserExtension = new BrowserAutomationExtension(_browserWindow);
                    var path = browserExtension.GetPath(_browserWindow.GetPageSource(), "a", AttributeIdentifierType.Id,
                        "detail/contact-info/");
                    _browserWindow.Browser.ExecuteScriptAsync(string.Format(LDClassesConstant.ScriptConstant.ScriptGetElementByIdToClick,path));
                }

                SplitDelay(DominatorAccountModel.Token, 5);
                ContactInfoHtml = _browserWindow.GetPageSource();
            }
            catch (OperationCanceledException)
            {
                CloseBrowser();
                _browserWindow.Browser.FrameLoadEnd -= NavigateToLinkedInConnections;
                throw new OperationCanceledException("Operation Cancelled!");
            }
            catch (AggregateException ae)
            {
                RemoveNonInteractedUser(ObjLinkedinUser);
                ae.DebugLog();
                _browserWindow.Browser.FrameLoadEnd -= NavigateToLinkedInConnections;
                Application.Current.Dispatcher.Invoke(() => { _browserWindow.Close(); });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void DownloadPdf(BrowserAutomationExtension automationExtension,string userProfileUrl = "")
        {
            try
            {
                // here several thread might come together
                // which causes issue for download options
                // sometimes extra worker thread get created with no name 

                if (string.IsNullOrEmpty(Thread.CurrentThread.Name))
                    return;
                _delayService.ThreadSleep(1000);
                lock (this)
                {
                    if (_currentThreadCount > 0)
                        return;
                    ++_currentThreadCount;
                }
            }
            catch (Exception exception)
            {
                exception.DebugLog();
            }

            try
            {
                {
                    try
                    {
                        if (IsDownloadPdfSuccess)
                            return;

                        var downloadHandler = new DownloadHandler(_browserWindow);
                        _browserWindow.Browser.DownloadHandler = downloadHandler;
                        downloadHandler.OnBeforeDownloadFired += OnBeforeDownloadFired;
                        downloadHandler.OnDownloadUpdatedFired += OnDownloadUpdatedFired;
                        downloadHandler.SuggestedName = FilePathForPdf;
                    }
                    catch (Exception exception)
                    {
                        exception.DebugLog();
                    }
                    JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                    // going back to show more option
                    _browserWindow.Browser.ExecuteScriptAsync(string.Format(LDClassesConstant.ScriptConstant.ScrollWindowByXXPixel,0,80));
                    _browserWindow.Browser.ExecuteScriptAsync(string.Format(LDClassesConstant.ScriptConstant.ScrollWindowToXXPixel,0,80));
                    _delayService.ThreadSleep(5000);
                    JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    if (!string.IsNullOrEmpty(userProfileUrl?.Trim()))
                    {
                        _browserWindow.Browser.Load(userProfileUrl);
                        GlobusLogHelper.log.Debug(Log.StartedActivity,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                            ObjLinkedinUser.FullName + "trying to download again");
                        SplitDelay(JobCancellationTokenSource.Token, 4);
                    }

                    JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    for (var i = 0; i < 1; i++)
                    {
                        if (IsDownloadPdfSuccess)
                            break;
                        automationExtension.ExecuteScript(AttributeIdentifierType.Xpath, $"//{HTMLTags.Span}[text()='Save to PDF']");
                        _delayService.ThreadSleep(5000);
                        JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                        //second try for download pdf
                        if (!IsDownloadPdfSuccess)
                        {
                            automationExtension.ExecuteScript(AttributeIdentifierType.Xpath, $"//{HTMLTags.Span}[text()='Save to PDF']");
                            SplitDelay(JobCancellationTokenSource.Token, 3);
                        }

                        JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                        // client facing issue of opening show more option 
                        // immediately getting closed before clicking save to pdf
                        if (!IsDownloadPdfSuccess)
                        {
                            automationExtension.ExecuteScript(AttributeIdentifierType.Xpath, $"//{HTMLTags.Span}[text()='Save to PDF']");
                            _delayService.ThreadSleep(5000);
                        }
                        SplitDelay(JobCancellationTokenSource.Token, 5);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                CloseBrowser();
                _browserWindow.Browser.FrameLoadEnd -= NavigateToLinkedInConnections;
                throw new OperationCanceledException("Operation Cancelled!");
            }
            catch (AggregateException ae)
            {
                _browserWindow.Browser.FrameLoadEnd -= NavigateToLinkedInConnections;
                Application.Current.Dispatcher.Invoke(() => { _browserWindow.Close(); });
                foreach (var e in ae.InnerExceptions)
                    if (e is TaskCanceledException || e is OperationCanceledException)
                        e.DebugLog("Cancellation requested before task completion!");
                    else
                        e.DebugLog(e.StackTrace + e.Message);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            finally
            {
                try
                {
                    IsFooterReached = true;
                    lock (this)
                    {
                        _currentThreadCount = 0;
                    }
                }
                catch (Exception e)
                {
                    e.DebugLog();
                }
            }
        }

        private void RemoveNonInteractedUser(LinkedinUser objLinkedinUser,
            IGlobalInteractionDetails ldGlobalInteractionDetails = null)
        {
            try
            {
                if (!LinkedInModel.IsEnableExportingHTMLOfDifferentConnections)
                    return;

                if (ldGlobalInteractionDetails == null)
                    ldGlobalInteractionDetails = InstanceProvider.GetInstance<IGlobalInteractionDetails>();
                ldGlobalInteractionDetails.RemoveIfExist(SocialNetworks.LinkedIn, ActivityType,
                    objLinkedinUser.ProfileUrl);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private async void ScrollAndExpand()
        {
            try
            {
                // here several thread might come together
                // which causes issue for download options
                // sometimes extra worker thread get created with no name 

                _delayService.ThreadSleep(1000);
                lock (this)
                {
                    if (_currentScrollExpandThreadCount > 0)
                        return;
                    ++_currentScrollExpandThreadCount;
                }
            }
            catch (Exception exception)
            {
                exception.DebugLog();
            }


            try
            {
                var _b = 0;
                var htmlDoc = new HtmlDocument();
                var htmlData = string.Empty;
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                for (var i = 0; i < 5; i++)
                {
                    await _browserWindow.Browser.GetSourceAsync().ContinueWith(taskHtml => htmlData = taskHtml.Result);
                    if (!string.IsNullOrEmpty(htmlData))
                        break;
                    _delayService.ThreadSleep(5000);
                }

                htmlDoc.LoadHtml(htmlData);

                if (!string.IsNullOrEmpty(htmlData) && htmlData.Contains("profile-unavailable"))
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.LinkedIn, Campaign?.CampaignName,
                        "LangKeyPublisher".FromResourceDictionary(), "User Page Not Found.");
                    return;
                }

                // MoveNext()
                var divs = htmlDoc.DocumentNode.SelectNodes($"//{HTMLTags.Div}"); //sharing-create-share-view__create-content 
                if (!divs.Any(x => x.OuterHtml.Contains("sharing-create-share-view__create-content")))
                {
                    #region When No Feed Page Then Scroll and Expand See More And Finally Save As Html

                    while (!IsFooterReached)
                        try
                        {
                            if (IsExportTimeCompleted)
                                return;

                            #region Scroll

                            _browserWindow.Browser.ExecuteScriptAsync("window.scrollBy(0, " + _b + ")");
                            _browserWindow.Browser.ExecuteScriptAsync("window.scrollTo(0, " + _b + ")");
                            _b = _b + 250;
                            _delayService.ThreadSleep(5000);

                            #endregion

                            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                            try
                            {
                                await _browserWindow.Browser.GetSourceAsync()
                                    .ContinueWith(taskHtml => htmlData = taskHtml.Result);
                            }
                            catch (Exception ex)
                            {
                                ex.DebugLog();
                            }

                            htmlDoc = new HtmlDocument();
                            htmlDoc.LoadHtml(htmlData);
                            var allButtonElements = htmlDoc.DocumentNode.SelectNodes($"//{HTMLTags.Button}");
                            var allLinks = htmlDoc.DocumentNode.SelectNodes($"//{HTMLTags.Anchor}");
                            divs = htmlDoc.DocumentNode.SelectNodes($"//{HTMLTags.Div}");
                            var headings = htmlDoc.DocumentNode.SelectNodes($"//{HTMLTags.Heading(2)}");

                            foreach (var buttonElement in allButtonElements)
                                try
                                {
                                    if (IsExportTimeCompleted)
                                        return;

                                    JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                                    #region Click Show more

                                    if (buttonElement.InnerText.Contains("Show") &&
                                        buttonElement.InnerText.Contains("more"))
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
                                                _browserWindow.Browser.ExecuteScriptAsync(string.Format(LDClassesConstant.ScriptConstant.ScriptGetElementsByClassNameToClick,val,2));
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

                            #region Experience Section and Expand See more

                            try
                            {
                                if (headings.Any(x => x.InnerText.Contains("Experience")) && _b > 780)
                                    if (!IsExpandedAllSeeMore)
                                    {
                                        await _browserWindow.Browser.GetSourceAsync()
                                            .ContinueWith(taskHtml => htmlData = taskHtml.Result);
                                        var divisionsInExperience =
                                            htmlDoc.DocumentNode.SelectNodes(
                                                "//div[@class=\"pv-entity__position-group-pager ember-view\"]");

                                        foreach (var division in divisionsInExperience)
                                            try
                                            {
                                                var divisionContent = division.ChildNodes[1].ChildNodes[1];
                                                if (divisionContent != null)
                                                {
                                                    //string className = DivisionContent.Attributes[1].Value;
                                                    var index = divisionsInExperience.IndexOf(division);
                                                    _browserWindow.Browser.ExecuteScriptAsync(string.Format(LDClassesConstant.ScriptConstant.ScriptGetElementsByClassNameToClick, "lt-line-clamp__more",index));
                                                    _delayService.ThreadSleep(2000);
                                                    _b = _b + 114;
                                                    _browserWindow.Browser.ExecuteScriptAsync(string.Format(LDClassesConstant.ScriptConstant.ScrollWindowByXXPixel,0,_b));
                                                    _browserWindow.Browser.ExecuteScriptAsync(string.Format(LDClassesConstant.ScriptConstant.ScrollWindowToXXPixel,0,_b));
                                                    _delayService.ThreadSleep(3000);
                                                }
                                            }
                                            catch (ArgumentNullException ex)
                                            {
                                                ex.DebugLog("ArgumentNullException  Expand See more");
                                            }
                                            catch (Exception ex)
                                            {
                                                ex.DebugLog("Exception  Expand See more");
                                            }

                                        IsExpandedAllSeeMore = true;
                                    }
                            }
                            catch (ArgumentNullException ex)
                            {
                                ex.DebugLog("ArgumentNullException Experience Section and Expand See more");
                            }
                            catch (Exception ex)
                            {
                                ex.DebugLog("Exception  Experience Section and Expand See more");
                            }

                            #endregion

                            #region Check IsFooterReached

                            try
                            {
                                #region IsFooterReached

                                if (divs.Any(x => x.InnerHtml.Contains("LinkedIn Corporation ©")))
                                {
                                    try
                                    {
                                        Thread.CurrentThread.Name =
                                            DominatorAccountModel.UserName + "_scroll_" +
                                            CurrentProfileUrl.Replace("https://www.linkedin.com/in/", "");
                                    }
                                    catch (Exception exception)
                                    {
                                        exception.DebugLog();
                                    }

                                    if (ExportConnectionModel.IsCheckedDownloadPdf && !IsDownloadPdfSuccess)
                                        DownloadPdf(automationExtension);

                                    _browserWindow.Browser.ExecuteScriptAsync(string.Format(LDClassesConstant.ScriptConstant.ScrollWindowByXXPixel, 0, _b));
                                    _browserWindow.Browser.ExecuteScriptAsync(string.Format(LDClassesConstant.ScriptConstant.ScrollWindowToXXPixel, 0, _b));
                                    // we setting footer reached inside downloadPdf
                                    if (!ExportConnectionModel.IsCheckedDownloadPdf)
                                        IsFooterReached = true;
                                }
                                else
                                {
                                    _delayService.ThreadSleep(5000);
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
                                ex.DebugLog("IsFooterReached = true;");
                                IsFooterReached = true;
                                _delayService.ThreadSleep(5000);
                            }

                            #endregion
                        }
                        catch (OperationCanceledException)
                        {
                            CloseBrowser();
                            throw new OperationCanceledException("Operation Cancelled!");
                        }

                    #endregion
                }
            }
            catch (OperationCanceledException)
            {
                CloseBrowser();
            }
            catch (ArgumentNullException ex)
            {
                ex.DebugLog("ArgumentNullException final.");
            }
            catch (AggregateException ae)
            {
                ae.DebugLog();
                CloseBrowser();
            }

            catch (Exception ex)
            {
                ex.DebugLog();
            }
            finally
            {
                try
                {
                    lock (this)
                    {
                        _currentScrollExpandThreadCount = 0;
                    }
                }
                catch (Exception e)
                {
                    e.DebugLog();
                }
            }
        }

        /// <summary>
        ///     large delay causes issue
        ///     sometimes browser window last instance not get closed properly and new campaign browser instance
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <param name="runTimes"></param>
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

        public Tuple<bool, string> SaveAsHtmlAlterNative(string filePath, string content)
        {
            try
            {
                Tuple<bool, string> isSaveAsHtmlSucess = null;
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                if (!File.Exists(filePath))
                {
                    File.WriteAllText(filePath, content, Encoding.UTF8);

                    isSaveAsHtmlSucess = new Tuple<bool, string>(true, "");
                }
                else

                {
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                        DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                        "File already exists in your location [ " + filePath + " ]");
                }

                return isSaveAsHtmlSucess;
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException("Operation Cancelled!");
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("file name, or both are too long."))
                    GlobusLogHelper.log.Info(Log.ActivityFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType, ex.Message, "");
                return new Tuple<bool, string>(false, ex.Message);
            }
        }

        #region download region

        public void OnBeforeDownloadFired(object sender, DownloadItem e)
        {
            UpdateDownloadAction("OnBeforeDownload", e);
        }

        public void OnDownloadUpdatedFired(object sender, DownloadItem e)
        {
            IsDownloadPdfSuccess = true;
            UpdateDownloadAction("OnDownloadUpdated", e);
        }

        public void UpdateDownloadAction(string downloadAction, DownloadItem downloadItem)
        {
            /*
            this.Dispatcher.Invoke(() =>
            {
                var viewModel = (BrowserTabViewModel)this.DataContext;
                viewModel.LastDownloadAction = downloadAction;
                viewModel.DownloadItem = downloadItem;
            });
            */
        }

        #endregion
    }

    public class DownloadHandler : IDownloadHandler
    {
        private MetroWindow _mainForm;
        public string SuggestedName = string.Empty;

        public DownloadHandler(MetroWindow form)
        {
            _mainForm = form;
        }


        public void OnBeforeDownload(IWebBrowser chromiumWebBrowser, IBrowser browser, DownloadItem downloadItem,
            IBeforeDownloadCallback callback)
        {
            try
            {
                var handler = OnBeforeDownloadFired;
                if (handler != null) handler(this, downloadItem);

                if (!callback.IsDisposed)
                    using (callback)
                    {
                        callback.Continue(SuggestedName, false);
                    }
            }
            catch (Exception exception)
            {
                exception.DebugLog();
            }
        }

        public void OnDownloadUpdated(IWebBrowser chromiumWebBrowser, IBrowser browser, DownloadItem downloadItem,
            IDownloadItemCallback callback)
        {
            var handler = OnDownloadUpdatedFired;
            if (handler != null) handler(this, downloadItem);
        }

        public bool CanDownload(IWebBrowser chromiumWebBrowser, IBrowser browser, string url, string requestMethod)
        {
            return true;
        }

        public event EventHandler<DownloadItem> OnBeforeDownloadFired;
        public event EventHandler<DownloadItem> OnDownloadUpdatedFired;
    }
}