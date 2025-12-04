using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using CefSharp;
using CommonServiceLocator;
using ThreadUtils;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.LdTables.Account;
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
using LinkedDominatorCore.LDUtility;
using LinkedDominatorCore.Request;
using LinkedDominatorCore.Utility;
using Newtonsoft.Json;

namespace LinkedDominatorCore.LDLibrary
{
    public class UserScraperProcess : LDJobProcessInteracted<
        InteractedUsers>
    {
        private readonly IDelayService _delayService;
        private readonly ILdFunctions _ldFunctions;
        private readonly ILdUniqueHandler _ldUniqueHandler;
        private BrowserWindow _browserWindow;

        private bool _isScrolledProfile;
        private BrowserAutomationExtension automationExtension;


        public UserScraperProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IDbGlobalService globalService,
            IExecutionLimitsManager executionLimitsManager, ILdQueryScraperFactory queryScraperFactory,
            ILdHttpHelper ldHttpHelper, ILdLogInProcess logInProcess,
            ILdFunctionFactory ldFunctionFactory, IDbInsertionHelper dbInsertionHelper, IDelayService delayService)
            : base(processScopeModel, accountServiceScoped, globalService, executionLimitsManager, queryScraperFactory,
                ldHttpHelper, logInProcess, dbInsertionHelper)
        {
            UserScraperModel = processScopeModel.GetActivitySettingsAs<UserScraperModel>();
            _ldFunctions = ldFunctionFactory.LdFunctions;
            _delayService = delayService;
            _ldUniqueHandler = InstanceProvider.GetInstance<ILdUniqueHandler>();
        }

        public UserScraperModel UserScraperModel { get; set; }
        private string CurrentProfileUrl { get; set; }


        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            var detailedUserInfoJasonString = string.Empty;
            var jobProcessResult = new JobProcessResult();
            var jsonHandler = JsonJArrayHandler.GetInstance;
            try
            {

                var objLinkedinUser = (LinkedinUser) scrapeResult.ResultUser;
                GlobusLogHelper.log.Info(Log.StartedActivity, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType, objLinkedinUser.FullName);

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
                var objGetDetailedUserInfo = new GetDetailedUserInfo(scrapeResult.QueryInfo, _delayService);

                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                #region Applying User Filter.

                try
                {
                    if (LdUserFilterProcess.IsUserFilterActive(UserScraperModel.LDUserFilterModel))
                    {
                        if (!LdUserFilterProcess.GetFilterStatus(objLinkedinUser.ProfileUrl, UserScraperModel.LDUserFilterModel, _ldFunctions))
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType, string.Format("LangKeyNotAValidUserAccordingToTheFilter".FromResourceDictionary(), objLinkedinUser.FullName));
                            jobProcessResult.IsProcessSuceessfull = false;
                            return jobProcessResult;
                        }
                        else
                            GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType, $"All Filter Matched Successfully,Proceeding For {ActivityType.ToString()} Of {objLinkedinUser.FullName}");
                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                #endregion

                if (UserScraperModel.IsCheckedWithoutVisiting)
                {
                    #region MyRegion

                    if (!string.IsNullOrEmpty(objLinkedinUser.ProfileUrl))
                    {
                        JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                        GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType, objLinkedinUser.FullName);
                        IncrementCounters();
                        try
                        {
                            objLinkedinUser.AccountUserFullName = DominatorAccountModel.AccountBaseModel.UserFullName;
                            objLinkedinUser.AccountUserProfileUrl =
                                DominatorAccountModel.AccountBaseModel.ProfilePictureUrl;

                            #region Getting userlocation
                            var resp =_ldFunctions.GetInnerHttpHelper().GetRequest($"https://www.linkedin.com/voyager/api/identity/dash/profiles?q=memberIdentity&memberIdentity={objLinkedinUser.PublicIdentifier}&decorationId=com.linkedin.voyager.dash.deco.identity.profile.FullProfileWithEntities-35").Response;
                            resp = string.IsNullOrEmpty(resp) ? _ldFunctions.GetInnerHttpHelper().GetRequest($"https://www.linkedin.com/voyager/api/identity/dash/profiles?q=memberIdentity&memberIdentity={objLinkedinUser.PublicIdentifier}&decorationId=com.linkedin.voyager.dash.deco.identity.profile.FullProfileWithEntities-35").Response:resp;
                            var jobj = jsonHandler.ParseJsonToJObject(resp);
                            var location = jsonHandler.GetJTokenValue(jobj,"elements", 0, "locationName");
                            objLinkedinUser.Location = string.IsNullOrEmpty(location)? jsonHandler.GetJTokenValue(jobj,"elements", 0, "geoLocation", "geo","defaultLocalizedName"):location;
                            objLinkedinUser.Firstname = jsonHandler.GetJTokenValue(jobj,"elements", 0, "firstName");
                            objLinkedinUser.Lastname = jsonHandler.GetJTokenValue(jobj,"elements", 0, "lastName");
                            objLinkedinUser.HeadlineTitle = jsonHandler.GetJTokenValue(jobj, "elements", 0, "headline");
                            objLinkedinUser.Industry = jsonHandler.GetJTokenValue(jobj, "elements", 0, "industry", "name");
                            if (string.IsNullOrEmpty(location) ?false:location != objLinkedinUser.Location)
                                objLinkedinUser.Location = location;
                            if (objLinkedinUser.ConnectedTime == null &&objLinkedinUser.ConnectedTimeStamp.ToString() != null)
                            {
                                if (objLinkedinUser.ConnectedTimeStamp == 0)
                                    objLinkedinUser.ConnectedTime = "N/A";
                                else
                                    objLinkedinUser.ConnectedTime = objLinkedinUser.ConnectedTimeStamp.EpochToDateTimeUtc().ToLocalTime().ToString(CultureInfo.InvariantCulture);
                            }
                            var ProfileDetails = jsonHandler.GetJTokenOfJToken(jobj, "elements",0, "profilePicture", "displayImageWithFrameReferenceUnion", "vectorImage");
                            ProfileDetails = ProfileDetails == null || !ProfileDetails.HasValues ? jsonHandler.GetJTokenOfJToken(jobj, "elements", 0, "profilePicture", "displayImageReference", "vectorImage") : ProfileDetails;
                            var ProfilePic = ProfileDetails != null ? jsonHandler.GetJTokenValue(ProfileDetails, "rootUrl") + jsonHandler.GetJTokenValue(jsonHandler.GetJArrayElement(jsonHandler.GetJTokenValue(ProfileDetails, "artifacts"))?.LastOrDefault(x=>x.ToString().Contains("\"width\": 800")|| x.ToString().Contains("\"width\": 400")|| x.ToString().Contains("\"width\": 200")), "fileIdentifyingUrlPathSegment") : string.Empty;
                            objLinkedinUser.ProfilePicUrl = ProfilePic;
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
                        GlobusLogHelper.log.Info(Log.ActivityFailed,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType, objLinkedinUser.FullName,
                            "");
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
                            GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                                DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                objLinkedinUser.FullName);
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
                            GlobusLogHelper.log.Info(Log.ActivityFailed,
                                DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                DominatorAccountModel.AccountBaseModel.UserName, ActivityType, objLinkedinUser.FullName,
                                "");
                            jobProcessResult.IsProcessSuceessfull = false;
                        }

                        #endregion
                    }
                    else if (UserScraperModel.IsCheckedVisiting)
                    {
                        #region Filters After Visiting Profile

                        //try
                        //{
                        //    #region Filter

                        //    if (LdUserFilterProcess.IsUserFilterActive(UserScraperModel.LDUserFilterModel))
                        //    {
                        //        var isValidUser = LdUserFilterProcess.GetFilterStatus(objLinkedinUser.ProfileUrl,
                        //            UserScraperModel.LDUserFilterModel, _ldFunctions);
                        //        if (!isValidUser)
                        //        {
                        //            GlobusLogHelper.log.Info(Log.CustomMessage,
                        //                DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        //                DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                        //                "[ " + objLinkedinUser.FullName +
                        //                " ] is not a valid user according to the filter.");
                        //            jobProcessResult.IsProcessSuceessfull = false;
                        //            return jobProcessResult;
                        //        }else
                        //            GlobusLogHelper.log.Info(Log.CustomMessage,DominatorAccountModel.AccountBaseModel.AccountNetwork,DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                        //                $"All Filter Matched Successfully,Proceeding for {ActivityType} of {objLinkedinUser.FullName}");
                        //    }

                        //    #endregion
                        //}
                        //catch (Exception ex)
                        //{
                        //    ex.DebugLog();
                        //}

                        #endregion

                        JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                        var resultScrapeProfileDetails = objGetDetailedUserInfo.ScrapeProfileDetails(
                            objLinkedinUser.ProfileUrl, DominatorAccountModel, _ldFunctions,
                            UserScraperModel.IsCheckedVisiting);
                        detailedUserInfoJasonString = resultScrapeProfileDetails.Item2;
                        var connectedTime = Utils.GetBetween(detailedUserInfoJasonString, "\"ConnectedTime\":\"", "\"");
                        if (string.IsNullOrEmpty(connectedTime) ? false : !connectedTime.Contains("N/A"))
                            objLinkedinUser.ConnectedTime = connectedTime;
                        else
                            objLinkedinUser.ConnectedTime = string.Empty;

                        //to check email is present or not
                        if(UserScraperModel.IsEmailExistCheckbox)
                        {
                            var email = Utils.GetBetween(detailedUserInfoJasonString, "\"EmailId\":\"", "\"");
                            if (string.IsNullOrEmpty(email)|| email.Contains("N/A"))
                            {
                                GlobusLogHelper.log.Info(Log.CustomMessage,
                                        DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                        "[ " + objLinkedinUser.FullName +
                                        " ] is not a valid user according to the filter.");
                                jobProcessResult.IsProcessSuceessfull = false;
                                return jobProcessResult;
                            }
                        }
                        //to check phone number is present or not
                        if(UserScraperModel.IsPhoneNumberExistCheckbox)
                        {
                            var phonenumber = Utils.GetBetween(detailedUserInfoJasonString, "\"PersonalPhoneNumber\":\"", "\"");
                            if (string.IsNullOrEmpty(phonenumber)|| phonenumber.Contains("N/A"))
                            {
                                GlobusLogHelper.log.Info(Log.CustomMessage,
                                        DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                        "[ " + objLinkedinUser.FullName +
                                        " ] is not a valid user according to the filter.");
                                jobProcessResult.IsProcessSuceessfull = false;
                                return jobProcessResult;
                            }
                        }
                        var success = resultScrapeProfileDetails.Item1;
                        if (success)
                        {
                            GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                                DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                objLinkedinUser.FullName);
                            IncrementCounters();
                            //  DatabaseInsertion(scrapeResult, objLinkedinUser, detailedUserInfoJasonString);
                            DbInsertionHelper.UserScraper(scrapeResult, objLinkedinUser, detailedUserInfoJasonString);
                            jobProcessResult.IsProcessSuceessfull = true;
                        }
                        else
                        {
                            GlobusLogHelper.log.Info(Log.ActivityFailed,
                                DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                DominatorAccountModel.AccountBaseModel.UserName, ActivityType, objLinkedinUser.FullName,
                                "");
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
                    JobProcessSuccessAndFail(!string.IsNullOrEmpty(objLinkedinUser.ProfileUrl), objLinkedinUser,
                        scrapeResult, "", jobProcessResult);
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
                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                    "please wait while viewing profile.");
                var url = _browserWindow.CurrentUrl();
                if(url!= objLinkedinUser.ProfileUrl)
                automationExtension.LoadPageUrlAndWait(objLinkedinUser.ProfileUrl,5);
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
            var htmlData = "";
            var htmlDoc = new HtmlDocument();

            try
            {
                _browserWindow.Browser.GetSourceAsync().ContinueWith(taskHtml => htmlData = taskHtml.Result);
                ScrollWindow(true);
                ScrollWindow();

                htmlData = _browserWindow.Browser.GetSourceAsync().Result;
                htmlDoc.LoadHtml(htmlData);
                var allButtonElements = htmlDoc.DocumentNode.SelectNodes($"//{HTMLTags.Button}");
                var allLinks = htmlDoc.DocumentNode.SelectNodes($"//{HTMLTags.Anchor}");
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
                                    //pv-profile-section__see-more-inline pv-profile-section__text-truncate-toggle link
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
                //pv-profile-detail__nav-link t-14 t-black--light t-bold ember-view
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

        private void ScrollWindow(bool isDown = false)
        {
            var down = isDown ? 100 : -100;

            for (var i = 1; i < 20; i++)
            {
                _delayService.ThreadSleep(100);
                _browserWindow.Browser.ExecuteScriptAsync(string.Format(LDClassesConstant.ScriptConstant.ScrollWindowByXXPixel,0,down));
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


        private void JobProcessSuccessAndFail(bool isSuccess, LinkedinUser objLinkedinUser,
            ScrapeResultNew scrapeResult, string jsonData, JobProcessResult jobProcessResult)
        {
            if (isSuccess)
            {
                GlobusLogHelper.log.Info(Log.ActivitySuccessful, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType, objLinkedinUser.FullName);
                IncrementCounters();

                //jsonData empty for non visit
                if (string.IsNullOrEmpty(jsonData))
                    try
                    {
                        jsonData = JsonConvert.SerializeObject(objLinkedinUser);
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                DbInsertionHelper.UserScraper(scrapeResult, objLinkedinUser, jsonData);
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

        private void CloseBrowser()
        {
            LDAccountsBrowserDetails.CloseBrowser(DominatorAccountModel);
        }
    }
}