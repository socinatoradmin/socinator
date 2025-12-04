using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using CefSharp;
using CommonServiceLocator;
using ThreadUtils;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.LdTables.Account;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.DHEnum;
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
using LinkedDominatorCore.LDModel.LDUtility;
using LinkedDominatorCore.LDUtility;
using LinkedDominatorCore.Request;
using LinkedDominatorCore.Utility;
using Newtonsoft.Json;
using UserScraperModel = LinkedDominatorCore.LDModel.SalesNavigatorScraper.UserScraperModel;

namespace LinkedDominatorCore.LDLibrary.SalesNavigatorScraperProcesses
{
    public class UserScraperProcess : LDJobProcessInteracted<
        InteractedUsers>
    {
        public static readonly object lockBrowser = new object();
        private readonly IDelayService _delayService;

        private readonly ILdFunctionFactory _ldFunctionFactory;
        private readonly ILdUniqueHandler _ldUniqueHandler;
        private BrowserWindow _browserWindow;

        private bool _isScrolledProfile;
        private BrowserAutomationExtension automationExtension;
        private bool IsFirst = true;

        public UserScraperProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IDbGlobalService globalService,
            IExecutionLimitsManager executionLimitsManager, ILdQueryScraperFactory queryScraperFactory,
            ILdHttpHelper ldHttpHelper,
            ILdLogInProcess logInProcess, ILdFunctionFactory ldFunctionFactory, IDbInsertionHelper dbInsertionHelper,
            IDelayService delayService)
            : base(processScopeModel, accountServiceScoped, globalService, executionLimitsManager, queryScraperFactory,
                ldHttpHelper, logInProcess, dbInsertionHelper)
        {
            UserScraperModel = processScopeModel.GetActivitySettingsAs<UserScraperModel>();
            _ldFunctionFactory = ldFunctionFactory;
            _ldUniqueHandler = InstanceProvider.GetInstance<ILdUniqueHandler>();
            _delayService = delayService;
        }

        public UserScraperModel UserScraperModel { get; set; }
        private string CurrentProfileUrl { get; set; }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            var jobProcessResult = new JobProcessResult();
            try
            {
                var objLinkedinUser = (LinkedinUser) scrapeResult.ResultUser;

                if (_ldUniqueHandler.IsCampaignWiseUnique(
                    new UniquePreRequisticProperties
                    {
                        AccountModel = DominatorAccountModel,
                        ActivityType = ActivityType,
                        CampaignId = CampaignId,
                        IsUniqueOperationChecked = UserScraperModel.IsUniqueOperationChecked,
                        ProfileUrl = objLinkedinUser.ProfileUrl
                    }))
                    return jobProcessResult;

                _isScrolledProfile = false;
                try
                {
                    JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                    if (FilterBlackListedUser(objLinkedinUser))
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.LinkedIn,
                            DominatorAccountModel.UserName
                            , UserType.BlackListedUser, $"Filtered user {objLinkedinUser.PublicIdentifier}.");
                        return jobProcessResult;
                    }

                    objLinkedinUser.FullName = Utils.RemoveHtmlTags(objLinkedinUser.FullName);
                    GlobusLogHelper.log.Info(Log.StartedActivity, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType, objLinkedinUser.FullName);

                    if (UserScraperModel.IsCheckedWithoutVisiting)
                    {
                        JobProcessSuccessAndFail(!string.IsNullOrEmpty(objLinkedinUser.ProfileUrl), objLinkedinUser,
                            scrapeResult, "", "", jobProcessResult);
                    }
                    else if (UserScraperModel.IsCheckedVisiting)
                    {
                        JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                        // var profileDetailApi = "https://www.linkedin.com/sales/api/v1/profile/" + objLinkedinUser.MemberId + "?authToken=" + objLinkedinUser.AuthToken + "&authType=NAME_SEARCH";

                        // here ntb(i.e. sessionId) value is optional
                        //var profileDetailUpdatedApi =
                        //  $"https://www.linkedin.com/sales/people/{objLinkedinUser.ProfileId},NAME_SEARCH,{objLinkedinUser.AuthToken}?_ntb=";
                        var profileDetailUpdatedApi = string.Empty;
                        if (!IsBrowser)
                        {
                            if (string.IsNullOrEmpty(objLinkedinUser.ProfileId))
                                objLinkedinUser.ProfileId = Utils.GetBetween(objLinkedinUser.ProfileUrl, "/lead/", ",NAME_SEARCH");
                            profileDetailUpdatedApi = LdConstants.GetSalesUserProfileAPI(objLinkedinUser.ProfileId,objLinkedinUser.AuthToken);
                        }
                        else
                            profileDetailUpdatedApi = objLinkedinUser.ProfileUrl;

                        // ViewHitting(objLinkedinUser);
                        // var objSalesNavigatorProfileDetailsResponseHandler = ldFunctions.GetSalesNavigatorProfileDetails(UserScraperModel, profileDetailUpdatedApi, ldFunctions);
                        var objSalesNavigatorProfileDetailsResponseHandler =
                            _ldFunctionFactory.LdFunctions.GetSalesNavigatorProfileDetails(UserScraperModel,
                                profileDetailUpdatedApi, objLinkedinUser);

                        if (LdUserFilterProcess.IsUserFilterActive(UserScraperModel.LDUserFilterModel) &&
                            !objSalesNavigatorProfileDetailsResponseHandler.IsValidUser)
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage,
                                DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                "[ " + objLinkedinUser.FullName + " ] is not a valid user according to the filter.");
                            return jobProcessResult;
                        }

                        JobProcessSuccessAndFail(objSalesNavigatorProfileDetailsResponseHandler.Success,
                            objLinkedinUser, scrapeResult, objSalesNavigatorProfileDetailsResponseHandler.JsonObject,
                            objSalesNavigatorProfileDetailsResponseHandler.ProfileUrl, jobProcessResult);
                    }
                    else if (UserScraperModel.IsViewProfileUsingEmbeddedBrowser)
                    {
                        // just in case if _isScrolledProfile not get true we are using stopWatch timeout
                        var stopWatch = new Stopwatch();
                        stopWatch.Start();
                        NavigateProfile(objLinkedinUser);
                        while (!_isScrolledProfile && stopWatch.Elapsed.Minutes < 2)
                        {
                            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                            _delayService.ThreadSleep(5000);
                        }

                        stopWatch.Stop();
                        // client using small delay causing problem for another job start running to avoid we using 
                        // this internal delay
                        //Thread.Sleep(15000);
                        if (!automationExtension.GetCurrentAddress().Contains("sales/search/"))
                        {
                            _browserWindow.Browser.Back();
                            _delayService.ThreadSleep(2000);
                        }

                        automationExtension.ScrollWindow(4000);
                        JobProcessSuccessAndFail(!string.IsNullOrEmpty(objLinkedinUser.ProfileUrl), objLinkedinUser,
                            scrapeResult, "", "", jobProcessResult);
                    }
                }
                catch (OperationCanceledException)
                {
                    LDAccountsBrowserDetails.CloseBrowser(DominatorAccountModel);
                    throw new OperationCanceledException("Operation Cancelled!");
                }
                catch (Exception ex)
                {
                    LDAccountsBrowserDetails.CloseBrowser(DominatorAccountModel);
                    ex.DebugLog();
                }

                DelayBeforeNextActivity();
                //var pageSource = _browserWindow.GetPageSource();
            }
            catch (OperationCanceledException)
            {
                LDAccountsBrowserDetails.CloseBrowser(DominatorAccountModel);
                throw new OperationCanceledException("Operation Cancelled!");
            }
            catch (Exception ex)
            {
                LDAccountsBrowserDetails.CloseBrowser(DominatorAccountModel);
                ex.DebugLog();
            }

            return jobProcessResult;
        }


        // ReSharper disable once UnusedMember.Local
        public void NavigateProfile(LinkedinUser objLinkedinUser)
        {
            #region  Navigate To Connection's Profile

            try
            {
                // trying to view profile

                if (_browserWindow == null)
                {
                    LDAccountsBrowserDetails.GetInstance().AccountBrowserCollections
                        .TryGetValue(DominatorAccountModel.UserName, out _browserWindow);
                    if (_browserWindow == null) return;
                    automationExtension = new BrowserAutomationExtension(_browserWindow);
                }

                GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                    "please wait while viewing profile.");

                //if first page than we scrolling down to load whole page

                var isPresentProfileId =
                    automationExtension.LoadAndClick(HTMLTags.Anchor, AttributeIdentifierType.Id, objLinkedinUser.ProfileId);
                if (isPresentProfileId)
                {
                    CurrentProfileUrl = automationExtension.GetCurrentAddress();
                    _delayService.ThreadSleep(8000);
                    // ();
                    ClickAndExpandForSalesViewProfile();
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


        private void ClickAndExpandForSalesViewProfile()
        {
            var htmlData = "";
            var htmlDoc = new HtmlDocument();
            var userPage = automationExtension.GetCurrentAddress();
            try
            {
                automationExtension = new BrowserAutomationExtension(_browserWindow);
                _browserWindow.Browser.GetSourceAsync().ContinueWith(taskHtml => htmlData = taskHtml.Result);
                htmlData = _browserWindow.Browser.GetSourceAsync().Result;
                htmlDoc.LoadHtml(htmlData);
                var profileExpansion = "profile-section__expansion-button button-tertiary-small-muted full-width";
                var profileInterest = "profile-interests__see-all-button button-tertiary-small full-width";
                var showContact = "profile-topcard__contact-info-show-all button-tertiary-small";
                if (htmlData.Contains(profileExpansion))
                    automationExtension.ExecuteXAndYClick(profileExpansion, AttributeIdentifierType.ClassName);
                // _browserWindow.Browser.ExecuteScriptAsync($"document.getElementsByClassName('{profileExpansion}')[0].click()");

                if (htmlData.Contains(profileInterest))
                    InterestClick();


                if (htmlData.Contains(showContact))
                {
                    _browserWindow.Browser.ExecuteScriptAsync(string.Format(LDClassesConstant.ScriptConstant.ScriptGetElementsByClassNameToClick,showContact,0));
                    _delayService.ThreadSleep(2000);
                    automationExtension.ExecuteXAndYClick("button-primary-medium fr",
                        AttributeIdentifierType.ClassName);
                }

                ScrollWindow(true);
                ScrollWindow();


                htmlData = _browserWindow.Browser.GetSourceAsync().Result;
                htmlDoc.LoadHtml(htmlData);
                var allLinks = htmlDoc.DocumentNode.SelectNodes($"//{HTMLTags.Anchor}");
                var clickableLinks = allLinks.ToList().Where(x => !string.IsNullOrEmpty(x.Id) && x.Id.Contains("ember"))
                    .Select(x => x.Id).ToList();

                var count = 0;
                foreach (var link in clickableLinks)
                {
                    if (clickableLinks.Count > 10)
                    {
                        var linkIdNum = 0;
                        int.TryParse(link.Replace("ember", ""), out linkIdNum);
                        if (linkIdNum < 150)
                            continue;
                    }

                    var resp = _browserWindow.Browser.EvaluateScriptAsync(string.Format(LDClassesConstant.ScriptConstant.ScriptGetElementByIdToClick,link)).Result.Success;
                    // var resp = automationExtension.ExecuteXAndYClick(link);
                    if (resp)
                    {
                        ++count;
                        _delayService.ThreadSleep(5000);
                        ScrollWindow(true);
                        if (userPage.Equals(automationExtension.GetCurrentAddress()))
                            continue;
                        _browserWindow.Browser.Back();
                        _delayService.ThreadSleep(2000);
                        if (count == 2)
                            break;
                    }
                }
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
                    "profile-interests__see-all-button button-tertiary-small full-width",
                    AttributeIdentifierType.ClassName);
                _delayService.ThreadSleep(3000);

                #region switch tab 

                var htmlData = _browserWindow.Browser.GetSourceAsync().Result;
                htmlDoc.LoadHtml(htmlData);

                var allLink = htmlDoc.DocumentNode.SelectNodes("//artdeco-tab");
                var tabSwitchcount = 0;

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
            var down = isDown ? 100 : -100;

            for (var i = 1; i < 20; i++)
            {
                _delayService.ThreadSleep(100);
                _browserWindow.Browser.ExecuteScriptAsync(string.Format(LDClassesConstant.ScriptConstant.ScrollWindowByXXPixel,0,down));
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

        private void JobProcessSuccessAndFail(bool isSuccess, LinkedinUser objLinkedinUser,
            ScrapeResultNew scrapeResult, string jsonData, string profileurl, JobProcessResult jobProcessResult)
        {
            if (isSuccess)
            {
                GlobusLogHelper.log.Info(Log.ActivitySuccessful, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType, objLinkedinUser.FullName);
                IncrementCounters();

                //jsonData empty for non visit
                if (string.IsNullOrEmpty(jsonData))
                    //jsonData=LDUtility.Utils.SerializeData(objLinkedinUser);
                    try
                    {
                        jsonData = JsonConvert.SerializeObject(objLinkedinUser);
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                objLinkedinUser.ProfileUrl = string.IsNullOrEmpty(profileurl)?objLinkedinUser.ProfileUrl:profileurl;
                DbInsertionHelper.SalesNavUser(scrapeResult, objLinkedinUser, jsonData);
                //this.AccountModel.LstConnections.Add(LinkedinUser);
                jobProcessResult.IsProcessSuceessfull = true;
            }
            else
            {
                GlobusLogHelper.log.Info(Log.ActivityFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType, objLinkedinUser.FullName, "");
                jobProcessResult.IsProcessSuceessfull = false;
            }
        }

        private bool FilterBlackListedUser(LinkedinUser linkedinUser)
        {
            // if filter is checked go further else no need to go
            if (!UserScraperModel.IsChkSkipBlackListedUser ||
                !UserScraperModel.IsChkGroupBlackList && !UserScraperModel.IsChkGroupBlackList)
                return false;
            var manageBlacklistWhitelist = new ManageBlacklistWhitelist(DbAccountService, _delayService);
            return manageBlacklistWhitelist.FilterSalesBlackListedUser(_ldFunctionFactory.LdFunctions, linkedinUser,
                UserScraperModel.IsChkPrivateBlackList, UserScraperModel.IsChkGroupBlackList);
        }

        private void CloseBrowser()
        {
            LDAccountsBrowserDetails.CloseBrowser(DominatorAccountModel);
        }
    }
}