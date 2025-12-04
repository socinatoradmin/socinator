using DominatorHouseCore;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using LinkedDominatorCore.LDModel;
using Newtonsoft.Json;
using System;
using DominatorHouseCore.Process.JobLimits;
using LinkedDominatorCore.Factories;
using LinkedDominatorCore.LDLibrary.DAL;
using LinkedDominatorCore.Request;
using LinkedDominatorCore.Utility;
using DominatorHouse.ThreadUtils;
using DominatorHouseCore.Process;
using LinkedDominatorCore.LDUtility;
using System.Diagnostics;
using EmbeddedBrowser;
using CefSharp;
using System.Threading;
using System.Linq;
using HtmlAgilityPack;

namespace LinkedDominatorCore.LDLibrary
{
    public class UserScraperProcess : LDJobProcessInteracted<DominatorHouseCore.DatabaseHandler.LdTables.Account.InteractedUsers>
    {
        public UserScraperModel UserScraperModel { get; set; }
        private readonly ILdFunctions _ldFunctions;
        private readonly IDelayService _delayService;
        private BrowserWindow _browserWindow;
        private string CurrentProfileUrl { get; set; }

        private bool _isScrolledProfile;
        private BrowserAutomationExtension automationExtension;


        public UserScraperProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IDbGlobalService globalService,
            IExecutionLimitsManager executionLimitsManager, ILdQueryScraperFactory queryScraperFactory, ILdHttpHelper ldHttpHelper, ILdLogInProcess logInProcess,
            ILdFunctionFactory ldFunctionFactory, IDbInsertionHelper dbInsertionHelper, IDelayService delayService)
            : base(processScopeModel, accountServiceScoped, globalService, executionLimitsManager, queryScraperFactory, ldHttpHelper, logInProcess, dbInsertionHelper)
        {
            UserScraperModel = processScopeModel.GetActivitySettingsAs<UserScraperModel>();
            _ldFunctions = ldFunctionFactory.LdFunctions;
            _delayService = delayService;
        }


        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            var detailedUserInfoJasonString = string.Empty;
            var jobProcessResult = new JobProcessResult();

            try
            {
                var objLinkedinUser = (LinkedinUser)scrapeResult.ResultUser;

                if (new LdUniqueHandler().IsCampaignWiseUnique(
                 new UniquePreRequisticProperties()
                 {
                     AccountModel = DominatorAccountModel,
                     ActivityType = ActivityType,
                     CampaignId = CampaignId,
                     IsUniqueOperationChecked = UserScraperModel.IsUniqueOperationChecked,
                     ProfileUrl = objLinkedinUser.ProfileUrl
                 }))
                    return jobProcessResult;
                var objGetDetailedUserInfo = new GetDetailedUserInfo(scrapeResult.QueryInfo, _delayService);

                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                if (UserScraperModel.IsCheckedWithoutVisiting)
                {
                    #region MyRegion
                    if (!string.IsNullOrEmpty(objLinkedinUser.ProfileUrl))
                    {
                        JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                        GlobusLogHelper.log.Info(Log.ActivitySuccessful, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType, objLinkedinUser.FullName);
                        IncrementCounters();
                        try
                        {
                            var jArr = new JsonJArrayHandler();
                            objLinkedinUser.AccountUserFullName = DominatorAccountModel.AccountBaseModel.UserFullName;
                            objLinkedinUser.AccountUserProfileUrl =
                                DominatorAccountModel.AccountBaseModel.ProfilePictureUrl;
                            #region Getting userlocation
                            var resp = 
                          _ldFunctions.GetInnerHttpHelper().GetRequest($"https://www.linkedin.com/voyager/api/identity/dash/profiles?q=memberIdentity&memberIdentity={objLinkedinUser.PublicIdentifier}&decorationId=com.linkedin.voyager.dash.deco.identity.profile.FullProfileWithEntities-35").Response;
                            var jobj = new JsonHandler(resp);
                            var location = jobj.GetElementValue("elements", 0, "locationName");
                            objLinkedinUser.Location = jobj.GetElementValue("elements", 0, "geoLocation", "geo", "defaultLocalizedName");
                            if (location != objLinkedinUser.Location)
                                objLinkedinUser.Location = location;
                           
                            #endregion
                            detailedUserInfoJasonString = JsonConvert.SerializeObject(objLinkedinUser);


                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }

                        DbInsertionHelper.UserScraper(scrapeResult, objLinkedinUser, detailedUserInfoJasonString);
                        jobProcessResult.IsProcessSuceessfull = true;
                    }
                    else
                    {
                        GlobusLogHelper.log.Info(Log.ActivityFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType, objLinkedinUser.FullName, "");
                        jobProcessResult.IsProcessSuceessfull = false;
                    }
                    #endregion
                }
                else if (UserScraperModel.IsCheckedVisitOnly || UserScraperModel.IsCheckedVisiting)
                {
                    var profilePageSource = string.Empty;
                    if (UserScraperModel.IsCheckedVisitOnly)
                    {
                        #region MyRegion
                        JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                        profilePageSource = _ldFunctions.GetRequestUpdatedUserAgent(objLinkedinUser.ProfileUrl);

                        if (!string.IsNullOrEmpty(profilePageSource))
                        {
                            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                            GlobusLogHelper.log.Info(Log.ActivitySuccessful, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType, objLinkedinUser.FullName);
                            IncrementCounters();
                            try
                            {
                                objLinkedinUser.AccountUserFullName =
                                    DominatorAccountModel.AccountBaseModel.UserFullName;
                                objLinkedinUser.AccountUserProfileUrl =
                                    DominatorAccountModel.AccountBaseModel.ProfilePictureUrl;
                                detailedUserInfoJasonString = JsonConvert.SerializeObject(objLinkedinUser);
                            }
                            catch (Exception ex)
                            {
                                ex.DebugLog();
                            }
                            DbInsertionHelper.UserScraper(scrapeResult, objLinkedinUser, detailedUserInfoJasonString);

                            //this.AccountModel.LstConnections.Add(LinkedinUser);
                            jobProcessResult.IsProcessSuceessfull = true;
                        }
                        else
                        {
                            GlobusLogHelper.log.Info(Log.ActivityFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType, objLinkedinUser.FullName, "");
                            jobProcessResult.IsProcessSuceessfull = false;
                        }
                        #endregion
                    }
                    else if (UserScraperModel.IsCheckedVisiting)
                    {
                        #region Filters After Visiting Profile
                        try
                        {
                            #region Filter
                            if (LdUserFilterProcess.IsUserFilterActive(UserScraperModel.LDUserFilterModel))
                            {
                                var isValidUser = LdUserFilterProcess.GetFilterStatus(objLinkedinUser.ProfileUrl, UserScraperModel.LDUserFilterModel, _ldFunctions);
                                if (!isValidUser)
                                {
                                    GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType, "[ " + objLinkedinUser.FullName + " ] is not a valid user according to the filter.");
                                    jobProcessResult.IsProcessSuceessfull = false;
                                    return jobProcessResult;
                                }
                            }
                            #endregion
                        }
                        catch (Exception ex)
                        { ex.DebugLog(); }

                        #endregion
                        JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                        GlobusLogHelper.log.Info(Log.StartedActivity, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType, objLinkedinUser.FullName);

                        var resultScrapeProfileDetails = objGetDetailedUserInfo.ScrapeProfileDetails(objLinkedinUser.ProfileUrl, DominatorAccountModel, _ldFunctions, UserScraperModel.IsCheckedVisiting);
                        detailedUserInfoJasonString = resultScrapeProfileDetails.Item2;
                        var connectedTime = Utils.GetBetween(detailedUserInfoJasonString, "\"ConnectedTime\":\"", "\"");
                        if(!connectedTime.Contains("N/A") && !string.IsNullOrEmpty(connectedTime))
                        objLinkedinUser.ConnectedTimeStamp = Int64.Parse(Utils.GetBetween(detailedUserInfoJasonString, "\"ConnectedTime\":\"", "\""));

                        var success = resultScrapeProfileDetails.Item1;
                        if (success)
                        {
                            GlobusLogHelper.log.Info(Log.ActivitySuccessful, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType, objLinkedinUser.FullName);
                            IncrementCounters();
                            //  DatabaseInsertion(scrapeResult, objLinkedinUser, detailedUserInfoJasonString);
                            DbInsertionHelper.UserScraper(scrapeResult, objLinkedinUser, detailedUserInfoJasonString);
                            jobProcessResult.IsProcessSuceessfull = true;
                        }
                        else
                        {
                            GlobusLogHelper.log.Info(Log.ActivityFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType, objLinkedinUser.FullName, "");
                            jobProcessResult.IsProcessSuceessfull = false;
                        }
                    }
                }
                else
                {
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
                    automationExtension.ScrollWindow(4000);
                    JobProcessSuccessAndFail(!string.IsNullOrEmpty(objLinkedinUser.ProfileUrl), objLinkedinUser, scrapeResult, "", jobProcessResult);


                }



            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException("Operation Cancelled!");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            DelayBeforeNextActivity();
            return jobProcessResult;
        }


        public void NavigateProfile(LinkedinUser objLinkedinUser)
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
                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType, "please wait while viewing profile.");

                //if first page than we scrolling down to load whole page
                var url = _browserWindow.CurrentUrl();
                var isPresentProfileId = url.Contains(objLinkedinUser.PublicIdentifier)
                    || automationExtension.LoadAndMouseClick("a", AttributeIdentifierType.Id, 5, objLinkedinUser.PublicIdentifier);

                if (isPresentProfileId)
                {
                    CurrentProfileUrl = automationExtension.GetCurrentAddress();
                    _delayService.ThreadSleep(8000);
                    ClickAndExpandForNormalViewProfile();
                    //ClickAndExpandForSalesViewProfile();
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
            string htmlData = "";
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
                var divs = htmlDoc.DocumentNode.SelectNodes("//div");
                var headings = htmlDoc.DocumentNode.SelectNodes("//h2");
                var outerHtmlSeeAll =
                    allButtonElements.FirstOrDefault(x => x.InnerText.Contains("\n      See all\n"))?.OuterHtml;
                foreach (var buttonElement in allButtonElements)
                {
                    try
                    {
                        JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                        #region Click Show more

                        if (buttonElement.InnerText.Contains("Show") && buttonElement.InnerText.Contains("more"))
                        {
                            try
                            {
                                var val = buttonElement.Attributes["class"].Value;

                                if (!allLinks.Any(x => x.OuterHtml.Contains(val)))
                                {
                                    //pv-profile-section__see-more-inline pv-profile-section__text-truncate-toggle link
                                    _browserWindow.Browser.ExecuteScriptAsync(
                                        "document.getElementsByClassName('" + val + "')[0].click()");
                                    _delayService.ThreadSleep(5000);
                                    break;
                                }

                                if (allLinks.Any(x => x.InnerText.Contains("See all")) &&
                                    !allLinks.Any(x => x.InnerText.Contains("See all articles")) &&
                                    !allLinks.Any(x => x.InnerText.Contains("See all activity")))
                                {
                                    _browserWindow.Browser.ExecuteScriptAsync(
                                        "document.getElementsByClassName('" + val + "')[1].click()");
                                    SplitDelay(JobCancellationTokenSource.Token, 1);
                                    _browserWindow.Browser.ExecuteScriptAsync(
                                        "document.getElementsByClassName('" + val + "')[2].click()");
                                    _delayService.ThreadSleep(5000);
                                }
                                else
                                {
                                    _browserWindow.Browser.ExecuteScriptAsync(
                                        "document.getElementsByClassName('" + val + "')[2].click()");
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
                }


                _browserWindow.Browser.ExecuteScriptAsync(
                    "document.getElementsByClassName('pv-profile-section__card-action-bar artdeco-container-card-action-bar ember-view')[0].click()");
                _delayService.ThreadSleep(2000);
                //pv-profile-detail__nav-link t-14 t-black--light t-bold ember-view
                _browserWindow.Browser.ExecuteScriptAsync(
                    "document.getElementsByClassName('pv-profile-detail__nav-link t-14 t-black--light t-bold ember-view')[1].click();");
                _delayService.ThreadSleep(2000);

                _browserWindow.Browser.ExecuteScriptAsync(
                    "document.getElementsByClassName('pv-profile-detail__nav-link t-14 t-black--light t-bold ember-view')[2].click()");
                _delayService.ThreadSleep(2000);
                _browserWindow.Browser.ExecuteScriptAsync("document.getElementsByClassName('artdeco-modal__dismiss artdeco-button artdeco-button--circle artdeco-button--muted artdeco-button--2 artdeco-button--tertiary ember-view')[0].click()");
                _delayService.ThreadSleep(1000);


                ScrollWindow(true);
                ScrollWindow();
                ScrollWindow(true);
                htmlData = _browserWindow.Browser.GetSourceAsync().Result;

                _delayService.ThreadSleep(2000);
                _browserWindow.Browser.ExecuteScriptAsync("document.getElementsByClassName('t-16 t-bold')[0].click()");
                _delayService.ThreadSleep(2000);
                _browserWindow.Browser.Back();
                //_browserWindow.Browser.ExecuteScriptAsync("document.getElementsByClassName('artdeco-modal__dismiss artdeco-button artdeco-button--circle artdeco-button--muted artdeco-button--2 artdeco-button--tertiary ember-view')[0].click()");
                _delayService.ThreadSleep(5000);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }


        }

        private void InterestClick()
        {
            try
            {
                var htmlDoc = new HtmlDocument();

                //_browserWindow.Browser.ExecuteScriptAsync(
                //    "document.getElementsByClassName('profile-interests__see-all-button button-tertiary-small full-width')[0].click()");
                automationExtension.ExecuteXAndYClick(
                    "profile-interests__see-all-button button-tertiary-small full-width", AttributeIdentifierType.ClassName);
                _delayService.ThreadSleep(3000);

                #region switch tab 

                var htmlData = _browserWindow.Browser.GetSourceAsync().Result;
                htmlDoc.LoadHtml(htmlData);

                var allLink = htmlDoc.DocumentNode.SelectNodes("//artdeco-tab");
                int tabSwitchcount = 0;

                foreach (var link in allLink)
                {
                    if (++tabSwitchcount == 1) continue;
                    _browserWindow.Browser.ExecuteScriptAsync($"document.getElementById('{link.Id}').click()");
                    // automationExtension.ExecuteXAndYClick(link.Id);
                    _delayService.ThreadSleep(2000);
                }

                #endregion

                // _browserWindow.Browser.ExecuteScriptAsync("document.getElementsByClassName('artdeco-button__icon')[0].click()");
                automationExtension.ExecuteXAndYClick("artdeco-button__icon", AttributeIdentifierType.ClassName);

            }
            catch (Exception exception)
            {
                exception.DebugLog();
            }

        }

        private void ScrollWindow(bool isDown = false)
        {
            int down = isDown ? 100 : -100;

            for (int i = 1; i < 20; i++)
            {
                _delayService.ThreadSleep(100);
                _browserWindow.Browser.ExecuteScriptAsync($"window.scrollBy(0, {down})");
            }
        }


        public void SplitDelay(CancellationToken cancellationToken, int runTimes)
        {
            try
            {
                for (int i = 0; i < runTimes; i++)
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


        private void JobProcessSuccessAndFail(bool isSuccess, LinkedinUser objLinkedinUser, ScrapeResultNew scrapeResult, string jsonData, JobProcessResult jobProcessResult)
        {
            if (isSuccess)
            {
                GlobusLogHelper.log.Info(Log.ActivitySuccessful, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType, objLinkedinUser.FullName);
                IncrementCounters();

                //jsonData empty for non visit
                if (string.IsNullOrEmpty(jsonData))
                //jsonData=LDUtility.Utils.SerializeData(objLinkedinUser);
                {
                    try
                    {
                        jsonData = JsonConvert.SerializeObject(objLinkedinUser);
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                }

                DbInsertionHelper.UserScraper(scrapeResult, objLinkedinUser, jsonData);
                //this.AccountModel.LstConnections.Add(LinkedinUser);
                jobProcessResult.IsProcessSuceessfull = true;
            }
            else
            {
                GlobusLogHelper.log.Info(Log.ActivityFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType, objLinkedinUser.FullName, "");
                jobProcessResult.IsProcessSuceessfull = false;
            }
        }
        private void CloseBrowser()
         => LDAccountsBrowserDetails.CloseBrowser(DominatorAccountModel);


    }
}
