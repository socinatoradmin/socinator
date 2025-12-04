using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.BusinessLogic.ActivitiesWorkflow;
using DominatorHouseCore.DatabaseHandler.DHTables;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.PuppeteerBrowser;
using DominatorHouseCore.Utility;
using PinDominatorCore.Interface;
using PinDominatorCore.PDLibrary.BrowserManager;
using PinDominatorCore.PDModel;
using PinDominatorCore.PDUtility;
using PinDominatorCore.Request;
using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Unity;

namespace PinDominatorCore.PDLibrary
{
    public interface IPdLogInProcess : ILoginProcessAsync
    {
        IPinFunction PinFunct { get; set; }
        IPdBrowserManager BrowserManager { get; set; }
        SoftwareSettingsModel SoftwareSettingsModel { get; set; }
        bool CheckLoginSync(DominatorAccountModel dominatorAccountModel, CancellationToken cancellationToken);
    }

    public class LogInProcess : IPdLogInProcess
    {
        private static SemaphoreSlim SemaphoreSlim = new SemaphoreSlim(5, 5);

        public IPdHttpHelper HttpHelper;
        public HttpWebRequest GRequest;
        public HttpWebResponse GResponse;
        public IPinFunction PinFunct { get; set; }
        public IPdBrowserManager BrowserManager { get; set; }
        public SoftwareSettingsModel SoftwareSettingsModel { get; set; }
        public IPDAccountSessionManager sessionManager { get; set; }
        string adsresponse = string.Empty;
        private JsonFunct JsonFunct = JsonFunct.GetInstance;
        public LogInProcess(IPdHttpHelper httpHelper, IPinFunction pinFunct, IPdBrowserManager browserManager)
        {
            HttpHelper = httpHelper;
            PinFunct = pinFunct;
            BrowserManager = browserManager;
            var softwareSettingManager = InstanceProvider.GetInstance<ISoftwareSettingsFileManager>();
            SoftwareSettingsModel = softwareSettingManager.GetSoftwareSettings();
            sessionManager = sessionManager ?? InstanceProvider.GetInstance<IPDAccountSessionManager>();
        }

        public LogInProcess(IPdHttpHelper httpHelper, IPdBrowserManager browserManager)
        {
            HttpHelper = httpHelper;
            BrowserManager = browserManager;
        }

        /// <summary>
        ///     CheckLogin is used to check whether Account is Logged in
        ///     It will return true if Account Logged in Otherwise it will return false
        /// </summary>
        /// <param name="dominatorAccountModel"></param>
        /// <returns></returns>
        public bool CheckLogin(DominatorAccountModel dominatorAccountModel, CancellationToken cancellationToken)
        {
            sessionManager.AddOrUpdateSession(ref dominatorAccountModel);
            PinFunct.SetHeaders(dominatorAccountModel);
            var accloginResp = HttpHelper.GetRequest("https://www.pinterest.com/").Response;
            return dominatorAccountModel.IsUserLoggedIn = LoginSucces(dominatorAccountModel, string.IsNullOrEmpty(Domain(dominatorAccountModel)), accloginResp);
        }

        /// <summary>
        ///     LoginWithDataBaseCookies is used to Login Account using Database Cookies
        /// </summary>
        /// <param name="dominatorAccountModel"></param>
        /// <param name="isMobileRequired"></param>
        public void LoginWithDataBaseCookies(DominatorAccountModel dominatorAccountModel, bool isMobileRequired, CancellationToken cancellationToken)
        =>  CheckLoginAsync(dominatorAccountModel, cancellationToken).Wait();


        /// <summary>
        ///     LoginWithAlternativeMethod is used to Login Account Using Account credentials
        /// </summary>
        /// <param name="dominatorAccountModel"></param>
        public async void LoginWithAlternativeMethod(DominatorAccountModel dominatorAccountModel, CancellationToken cancellationToken)
        => await CheckLoginAsync(dominatorAccountModel, cancellationToken);

        public bool CheckLoginSync(DominatorAccountModel dominatorAccountModel, CancellationToken cancellationToken)
         => CheckLoginAsync(dominatorAccountModel, cancellationToken).Result;

        private void InitializeHttpHelp(DominatorAccountModel dominatorAccountModel)
        {
            IRequestParameters requestParameters = PinFunct.SetRequestHeaders(dominatorAccountModel);

            requestParameters.Cookies = dominatorAccountModel.Cookies;
            HttpHelper.SetRequestParameter(requestParameters);
        }

        string Domain(DominatorAccountModel dominatorAccountModel)
        {
            string domain = "www.pinterest.com";
            if (HttpHelper.GetRequestParameter().Cookies != null && HttpHelper.GetRequestParameter().Cookies.Count != 0)
            {
                domain = HttpHelper.GetRequestParameter().Cookies["csrftoken"]?.Domain;
                domain = domain?.Trim('.');
            }
            else if (dominatorAccountModel.Cookies != null && dominatorAccountModel.Cookies.Count != 0)
            {
                domain = dominatorAccountModel.Cookies["csrftoken"]?.Domain;
                domain = domain?.Trim('.');
            }
            return domain;
        }

        public async Task<bool> CheckLoginAsync(DominatorAccountModel dominatorAccountModel, CancellationToken cancellationToken, bool displayLoginMsg = false, LoginType loginType = LoginType.AutomationLogin)
        {
            try
            {
                await SemaphoreSlim.WaitAsync();

                sessionManager.AddOrUpdateSession(ref dominatorAccountModel);
                if (displayLoginMsg)
                    GlobusLogHelper.log.Info(Log.AccountLogin, dominatorAccountModel.AccountBaseModel.AccountNetwork,
                    dominatorAccountModel.AccountBaseModel.UserName);

                ProxyStringCheck(dominatorAccountModel);

                if (dominatorAccountModel.IsRunProcessThroughBrowser || true && dominatorAccountModel?.Cookies?.Count==0)
                {
                    //To check proxy status
                    IAccountScopeFactory accountScopeFactory = InstanceProvider.GetInstance<IAccountScopeFactory>();
                    var httpForProxyCheck = accountScopeFactory[$"{dominatorAccountModel.AccountBaseModel.AccountId}_ProxyCheck"].Resolve<IPdHttpHelper>();
                    var reqProxyCheck = httpForProxyCheck.GetRequestParameter();
                    reqProxyCheck.Proxy = dominatorAccountModel.AccountBaseModel.AccountProxy;
                    httpForProxyCheck.SetRequestParameter(reqProxyCheck);
                    var proxyCheckResp = await httpForProxyCheck.GetRequestAsync("https://www.pinterest.com", cancellationToken);

                    if (proxyCheckResp != null && (proxyCheckResp?.Exception != null && proxyCheckResp.Exception.Message.Contains("Unable to connect to the remote server")
                                                   || (!string.IsNullOrEmpty(proxyCheckResp.Response)
                                                       && proxyCheckResp.Response.Contains("ERROR: The requested URL could not be retrieved"))))
                    {
                        dominatorAccountModel.AccountBaseModel.Status = AccountStatus.ProxyNotWorking;
                        return false;
                    }
                    cancellationToken.ThrowIfCancellationRequested();
                    await Task.Run(()=>BrowserManager.BrowserLogin(dominatorAccountModel,cancellationToken));
                    IResponseParameter accloginResp = new DominatorHouseCore.Request.ResponseParameter() { Response = BrowserManager.CurrentData };
                    if (accloginResp.Response.Contains("There's been some strange activity on your account, so we put it in safe mode to protect your Pins. To get going again, just reset your password.") ||
                        accloginResp.Response.Contains("We noticed some strange activity on your account. Reset your password or log in with Facebook or Google to get back into your account.") ||
                        accloginResp.Response.Contains("We noticed some strange activity on your Pinterest account so we reset your password and logged everyone out (including you)."))
                    {
                        dominatorAccountModel.AccountBaseModel.Status = AccountStatus.SetNewPassword;
                        dominatorAccountModel.IsUserLoggedIn = false;
                    }

                    else if (accloginResp.Response.Contains("The password you entered is incorrect") ||
                        accloginResp.Response.Contains("that password isn't right") ||
                        accloginResp.Response.Contains("doesn't look like an email address or phone number"))
                    {
                        dominatorAccountModel.AccountBaseModel.Status = AccountStatus.InvalidCredentials;
                        dominatorAccountModel.IsUserLoggedIn = false;
                    }

                    else if (accloginResp.Response.Contains("Oops! You logged in too quickly. Please try again with the reCAPTCHA") 
                        || accloginResp.Response.Contains("It looks like you’re logging in a lot. Log in with Facebook or Google if you’re connected, or reset your password. Or you can wait 30 minutes and try again."))
                    {
                        dominatorAccountModel.AccountBaseModel.Status = AccountStatus.TemporarilyBlocked;
                        dominatorAccountModel.IsUserLoggedIn = false;
                    }

                    if (!string.IsNullOrEmpty(accloginResp.Response) && accloginResp.Response.Contains("\"isAuth\": true")
                        || accloginResp.Response.Contains("pinHolder") || accloginResp.Response.Contains("header1\": \"What are you interested in?") 
                        || accloginResp.Response.Contains("\"isLoggedIn\": true") || accloginResp.Response.Contains("domainVerified")
                        || accloginResp.Response.Contains("\"login_state\":1")|| accloginResp.Response.Contains(PdConstants.CheckLoginStatusString))
                    {
                        sessionManager.AddOrUpdateSession(ref dominatorAccountModel,true);
                        dominatorAccountModel.IsUserLoggedIn = true;
                        InitializeHttpHelp(dominatorAccountModel);
                        var domain = Domain(dominatorAccountModel);
                        var langUrl = $"https://{domain}/resource/UserSettingsResource/update/ ";
                        adsresponse = accloginResp.Response;
                        var langPostData = new PdJsonElement
                        {
                            SourceUrl = "/settings/account-settings/",
                            Data = new PdJsonElement
                            {
                                Options = new PdJsonElement
                                {
                                    Locale = "en-IN"
                                },
                                Context = new PdJsonElement()
                            }
                        };

                        var objParameters = new PdRequestParameters();
                        objParameters.PdPostElements = langPostData;
                        var postData = objParameters.GetPostDataFromJson();
                        PinFunct.SetRequestHeaders(dominatorAccountModel);
                        var langResp =await HttpHelper.PostRequestAsync(langUrl, postData, cancellationToken);
                        var status = Utilities.GetBetween(langResp.Response, "status\":\"", "\"");

                        if (!status.Equals("success"))
                            BrowserManager.ChangeLanguage(dominatorAccountModel.CancellationSource);
                    }
                    else
                    {
                        await LoginWithAlternativeMethodAsync(dominatorAccountModel, cancellationToken);
                        var Resp =await HttpHelper.GetRequestAsync("https://www.pinterest.com/", cancellationToken);
                        accloginResp = new DominatorHouseCore.Request.ResponseParameter() { Response = Resp.Response };
                    }
                    var RequestHeaderDetails = PdRequestHeaderDetails.GetRequestHeader(accloginResp.Response,TokenDetailsType.Users);
                    if (!string.IsNullOrEmpty(RequestHeaderDetails.Username))
                        dominatorAccountModel.AccountBaseModel.ProfileId = RequestHeaderDetails.Username;
                    //Check for Account Mode Status
                    CheckAccountBusinessModeStatus(dominatorAccountModel, accloginResp.Response);
                }
                else
                {
                    InitializeHttpHelp(dominatorAccountModel);

                    var domain = Domain(dominatorAccountModel);
                    cancellationToken.ThrowIfCancellationRequested();

                    IResponseParameter accloginResp =await HttpHelper.GetRequestAsync(PdConstants.Https + domain, cancellationToken);

                    int failedCount = 1;
                    while (string.IsNullOrEmpty(accloginResp.Response) && failedCount++ < 3)
                    {
                        Sleep();
                        cancellationToken.ThrowIfCancellationRequested();
                        accloginResp =await HttpHelper.GetRequestAsync(PdConstants.Https + domain, cancellationToken);
                    }

                    if (!string.IsNullOrEmpty(accloginResp.Response) && (accloginResp.Response.Contains(dominatorAccountModel.AccountBaseModel.UserName)
                        || accloginResp.Response.Contains("pinHolder") || accloginResp.Response.Contains("header1\": \"What are you interested in?")
                        || accloginResp.Response.Contains("\"isLoggedIn\": true") || accloginResp.Response.Contains("\"isLoggedIn\":true")
                        || accloginResp.Response.Contains("domainVerified")) || accloginResp.Response.Contains(PdConstants.CheckLoginStatusString)
                        || accloginResp.Response.Contains("\"login_state\":1"))
                    {
                        sessionManager.AddOrUpdateSession(ref dominatorAccountModel, true);
                        //Check for Account Mode Status
                        CheckAccountBusinessModeStatus(dominatorAccountModel, accloginResp.Response);
                        dominatorAccountModel.IsUserLoggedIn = true;
                        adsresponse = accloginResp.Response;
                    }
                    else
                        await LoginWithAlternativeMethodAsync(dominatorAccountModel, cancellationToken);
                }
                if (dominatorAccountModel.IsUserLoggedIn)
                {
                    BrowserManager.Domain = Domain(dominatorAccountModel);
                    PinFunct.Domain = Domain(dominatorAccountModel);

                    #region To select latest csrf token from cookies
                    var helperCookie = dominatorAccountModel.CookieHelperList;
                    var newCookies = dominatorAccountModel.Cookies;
                    dominatorAccountModel.Cookies = new CookieCollection();
                    var cookieIndex = new System.Collections.Generic.List<int>();
                    int iteration = 0;
                    foreach (Cookie cook in newCookies)
                    {
                        if (cook.Name == "csrftoken")
                            cookieIndex.Add(iteration);
                        iteration++;
                        if (cookieIndex.Count > 1)
                            break;
                    }
                    if (cookieIndex.Count > 1)
                    {
                        int newIteration = 0;
                        foreach (Cookie cook in newCookies)
                        {
                            if (newIteration != cookieIndex[0])
                            {
                                dominatorAccountModel.CookieHelperList.Add(new CookieHelper
                                {
                                    Domain = cook.Domain,
                                    Name = cook.Name,
                                    Value = cook.Value
                                });
                            }
                            else { }
                            newIteration++;
                        }
                    }
                    else
                        dominatorAccountModel.CookieHelperList = helperCookie;
                    #endregion

                    dominatorAccountModel.AccountBaseModel.Status = AccountStatus.Success;

                    if (displayLoginMsg)
                        GlobusLogHelper.log.Info(Log.SuccessfulLogin, dominatorAccountModel.AccountBaseModel.AccountNetwork,
                        dominatorAccountModel.AccountBaseModel.UserName);

                    if (dominatorAccountModel.AccountBaseModel.Status == AccountStatus.TryingToLogin)
                        dominatorAccountModel.AccountBaseModel.Status = AccountStatus.NotChecked;
                    SocinatorAccountBuilder.Instance(dominatorAccountModel.AccountBaseModel.AccountId)
                        .AddOrUpdateDominatorAccountBase(dominatorAccountModel.AccountBaseModel)
                        .AddOrUpdateCookies(dominatorAccountModel.Cookies)
                        .AddOrUpdateDisplayColumn1(dominatorAccountModel.DisplayColumnValue1)
                        .AddOrUpdateDisplayColumn2(dominatorAccountModel.DisplayColumnValue2)
                        .AddOrUpdateDisplayColumn3(dominatorAccountModel.DisplayColumnValue3)
                        .AddOrUpdateDisplayColumn4(dominatorAccountModel.DisplayColumnValue4)
                        .AddOrUpdateDisplayColumn11(dominatorAccountModel.DisplayColumnValue11)
                        .AddOrUpdateUserAgentWeb(dominatorAccountModel.UserAgentWeb)
                        .SaveToBinFile();
                    //To scrape Pinterest ads
                    #region Pinterest Add Scrapping.

                    //var IAdsscraperfunction = InstanceProvider.GetInstance<IAdsScraperFunction>();
                    //var threadRedditAds = new Thread(() =>
                    // IAdsscraperfunction.GetAllPinsForAdsScrape(dominatorAccountModel, adsresponse))
                    //{ IsBackground = true };
                    //threadRedditAds.Start();

                    #endregion
                }
                else
                {
                    PdStatic.FailedLog(dominatorAccountModel, PdStatic.GetFailedMessage(dominatorAccountModel.AccountBaseModel.Status));
                    if (dominatorAccountModel.AccountBaseModel.Status == AccountStatus.TryingToLogin)
                        dominatorAccountModel.AccountBaseModel.Status = AccountStatus.NotChecked;
                    SocinatorAccountBuilder.Instance(dominatorAccountModel.AccountBaseModel.AccountId)
                         .AddOrUpdateDominatorAccountBase(dominatorAccountModel.AccountBaseModel)
                         .SaveToBinFile();
                }

                UpdateGlobalAccountDetails(dominatorAccountModel);
                return dominatorAccountModel.IsUserLoggedIn;
            }
            catch (OperationCanceledException)
            {
                if (!dominatorAccountModel.AccountBaseModel.Status.Equals(AccountStatus.Success))
                    dominatorAccountModel.AccountBaseModel.Status = AccountStatus.NotChecked;
                return false;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return false;
            }
            finally
            {
                SemaphoreSlim.Release();
            }
        }

        public async Task LoginWithDataBaseCookiesAsync(DominatorAccountModel account, bool isMobileRequired, CancellationToken cancellationToken)
        => await LoginWithAlternativeMethodAsync(account, account.Token);

        private static void UpdateGlobalAccountDetails(DominatorAccountModel dominatorAccountModel)
        {
            try
            {
                var globalDbOperation = new DbOperations(SocinatorInitialize.GetGlobalDatabase().GetSqlConnection());
                if (globalDbOperation.Get<AccountDetails>().Any(account => dominatorAccountModel.AccountId == account.AccountId && account.ActivityManager is null))
                    globalDbOperation.RemoveMatch<AccountDetails>(account => dominatorAccountModel.AccountId == account.AccountId && account.ActivityManager == null);
                globalDbOperation.UpdateAccountDetails(dominatorAccountModel);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void LoginWithBrowserMethod(DominatorAccountModel dominatorAccountModel, CancellationToken cancellationToken, VerificationType verificationType = 0, LoginType loginType = LoginType.AutomationLogin) => CheckLoginAsync(dominatorAccountModel, cancellationToken).Wait();

        public async Task LoginWithAlternativeMethodAsync(DominatorAccountModel accountModel, CancellationToken cancellationToken)
        {
            try
            {
                var checkLoginResp = string.Empty;
                sessionManager.AddOrUpdateSession(ref accountModel);
                if (IsntEmail(accountModel))
                    return;
                NewReqParam();
                PinFunct.SetHeaders(accountModel);

                var defaultUrl = "https://www.pinterest.com";

                cancellationToken.ThrowIfCancellationRequested();
                var firstResponse = await HitFrontPage(defaultUrl, cancellationToken);
                if (HttpHelper.GetRequestParameter().Cookies != null && HttpHelper.GetRequestParameter().Cookies.Count > 0)
                {
                    PinFunct.Domain = HttpHelper.GetRequestParameter().Cookies["csrftoken"].Domain;
                    PinFunct.Domain = PinFunct.Domain[0].Equals('.') ? PinFunct.Domain.Remove(0, 1) : PinFunct.Domain;
                }
                var proxyFailed = ProxyFailedReason(firstResponse);
                if (IfProxyFailed(accountModel, proxyFailed))
                    return;
                if (LoginSucces(accountModel, string.IsNullOrEmpty(PinFunct.Domain), firstResponse.Response))
                {
                    accountModel.Cookies = HttpHelper.GetRequestParameter().Cookies;
                    //Check for Account Mode Status
                    CheckAccountBusinessModeStatus(accountModel, firstResponse.Response);
                    var requestHeader1 = PinFunct.SetHeadersToChangeDomain(accountModel);
                    // set account cookies in request header
                    if (requestHeader1.Cookies.Count == 0)
                    {
                        requestHeader1.Cookies = accountModel.Cookies;
                        HttpHelper.SetRequestParameter(requestHeader1);
                    }
                    return;
                }
                else
                {
                    accountModel.AccountBaseModel.Status = AccountStatus.TryingToLogin;
                    var loginResp = await PinFunct.PuppeteerLogin(accountModel,PinFunct.Domain);
                    checkLoginResp = loginResp.Response;
                }
                var requestHeader = PinFunct.SetHeadersToChangeDomain(accountModel);
                #region HTTP Logic Requests.
                //Request Headers for login
                //PinFunct.SetRequestHeaders(accountModel);

                //var appVersion = Utilities.GetBetween(firstResponse.Response, "app_version\":\"", "\"");
                //var experimentHash = Utilities.GetBetween(firstResponse.Response, "triggerable_experiments_hash\":\"", "\"");
                //experimentHash = string.IsNullOrEmpty(experimentHash) ? Utilities.GetBetween(firstResponse.Response, "experiment_hash\":\"", "\"") : experimentHash;
                //var req = HttpHelper.GetRequestParameter();
                //req.Headers["X-APP-VERSION"] = appVersion;
                //req.Headers["sec-ch-ua"] = "\"Google Chrome\";v=\"117\", \"Not;A=Brand\";v=\"8\", \"Chromium\";v=\"117\"";
                //req.Headers["X-Pinterest-PWS-Handler"] = "www/index.js";
                //req.Headers["sec-ch-ua-mobile"] = "?0";
                //req.Headers["sec-ch-ua-platform-version"] = "\"10.0.0\"";
                //req.Headers["X-Pinterest-Source-Url"] = "/";
                //req.Headers["sec-ch-ua-full-version-list"] = "\"Google Chrome\";v=\"117.0.5938.132\", \"Not;A=Brand\";v=\"8.0.0.0\", \"Chromium\";v=\"117.0.5938.132\"";
                //req.Headers["sec-ch-ua-model"] = "\"\"";
                //req.Headers["Accept-Language"] = "en-GB,en-US;q=0.9,en;q=0.8";
                //req.Headers["sec-ch-ua-platform"] = "\"Windows\"";
                //req.Headers["Sec-Fetch-Dest"] = "empty";
                //req.Headers["Accept-Encoding"] = "gzip, deflate";
                //req.Headers["Sec-Fetch-Site"] = "same-origin";
                //req.Headers["Sec-Fetch-Mode"] = "cors";

                //HttpHelper.SetRequestParameter(req);

                //var objParameters = new PdRequestParameters
                //{ PdPostElements = JsonFunct.GetPostDataFromJsonLogin(accountModel) };

                //cancellationToken.ThrowIfCancellationRequested();
                //Sleep(5, 8);

                //cancellationToken.ThrowIfCancellationRequested();
                //var jsonString=objParameters.GetJsonString();
                //var username=Uri.UnescapeDataString(Utilities.GetBetween(jsonString, "\"username_or_email\":\"", "\""));
                //var pswd=Uri.UnescapeDataString(Utilities.GetBetween(jsonString, "\"password\":\"", "\""));
                //var captchaTokenResponse =await PinFunct.HttpHelper.GetRequestAsync($"https://www.recaptcha.net/recaptcha/enterprise/anchor?ar=1&k=6Ldx7ZkUAAAAAF3SZ05DRL2Kdh911tCa3qFP0-0r&co=aHR0cHM6Ly9pbi5waW50ZXJlc3QuY29tOjQ0Mw..&hl=en&v=3sU2vDRVDmUU2E0Ro4VadvPr&size=invisible&cb={DateTime.Now.Ticks.ToString("x")}",accountModel.Token);
                //var rCaptchaV3TokenValue = Utilities.GetBetween(captchaTokenResponse.Response, "id=\"recaptcha-token\" value=\"", "\"");
                //var postBody = "source_url=%2F&data=" +WebUtility.UrlEncode($"{{\"options\":{{\"username_or_email\":\"{username}\",\"password\":\"{pswd}\",\"referrer\":\"{Domain(accountModel)}\",\"app_type_from_client\":5,\"visited_pages_before_login\":\"[{{\\\"path\\\":\\\"/\\\",\\\"pageType\\\":\\\"home\\\",\\\"ts\\\":{PdConstants.GetTicks}}}]\",\"recaptchaV3Token\":\"{rCaptchaV3TokenValue}\",\"context\":{{}}}}}}");
                //var resp = await PostHit(PdConstants.Https + PinFunct.Domain,Encoding.UTF8.GetBytes(postBody), cancellationToken);

                //string errorMessage = string.Empty;
                //try
                //{
                //    var jsonHand = new JsonHandler(resp);
                //    if (string.IsNullOrEmpty(jsonHand.GetElementValue("resource_response", "data")) &&
                //        jsonHand.GetElementValue("resource_response", "error") != null)
                //    {
                //        errorMessage = jsonHand.GetElementValue("resource_response", "error", "message");
                //    }
                //}
                //catch (Exception ex)
                //{
                //    ex.DebugLog();
                //}

                //#region Captcha Implementation
                //var genericFileManager = InstanceProvider.GetInstance<IGenericFileManager>();
                //var imageCaptchaServicesModel = genericFileManager.GetModel<DominatorHouseCore.Models.Config.ImageCaptchaServicesModel>(ConstantVariable.GetImageCaptchaServicesFile());

                //if (errorMessage.Contains("Sorry! We've had to block actions (logins) from your area because of suspicious activity.") &&
                //    !string.IsNullOrEmpty(imageCaptchaServicesModel.Token))
                //{
                //    GlobusLogHelper.log.Info(Log.CustomMessage, accountModel.AccountBaseModel.UserName, SocialNetworks.Pinterest,
                //         "LangKeyCaptcha".FromResourceDictionary(), "LangKeySolvingCaptcha".FromResourceDictionary());
                //    ImageTypersHelper imageTyperz = new ImageTypersHelper(imageCaptchaServicesModel.Token);
                //    var captchaId = imageTyperz.SubmitSiteKey("https://www.pinterest.com/resource/UserSessionResource/create/", "6LeZIo0UAAAAAKFpimHio-ff_ddIT8f_QzpXG0_1");
                //    string capcthaResponse = imageTyperz.GetGResponseCaptcha(captchaId, accountModel.AccountBaseModel.UserName);

                //    if (HttpHelper.GetRequestParameter().Cookies != null && HttpHelper.GetRequestParameter().Cookies.Count > 0)
                //    {
                //        PinFunct.Domain = HttpHelper.GetRequestParameter().Cookies["csrftoken"]?.Domain;
                //        PinFunct.Domain = PinFunct.Domain != null && PinFunct.Domain[0].Equals('.') ? PinFunct.Domain.Remove(0, 1) : PinFunct.Domain;
                //    }

                //    string submitUrl = $"https://{PinFunct.Domain}/resource/UserSessionResource/create/";
                //    var objJsonFunct = JsonFunct;
                //    objParameters = new PdRequestParameters
                //    {
                //        PdPostElements = objJsonFunct.GetPostDataFromJsonLoginForCaptcha(accountModel, capcthaResponse)
                //    };

                //    var postData = objParameters.GetPostDataFromJson();

                //    resp = HttpHelper.PostRequest(submitUrl, postData).Response;
                //}

                //else if (errorMessage.Contains("Sorry! We've had to block actions (logins) from your area because of suspicious activity.") &&
                //    string.IsNullOrEmpty(imageCaptchaServicesModel.Token))
                //{
                //    GlobusLogHelper.log.Info(Log.CustomMessage, accountModel.AccountBaseModel.UserName, SocialNetworks.Pinterest,
                //        "LangKeyCaptcha", "LangKeyProvideImageTyperzTokenForCaptcha".FromResourceDictionary());
                //}
                //#endregion

                //if (GotErrorAfterPostHit(accountModel, resp))
                //    return;

                ////Navigate to set Header to get Domain
                //var requestHeader = PinFunct.SetHeadersToChangeDomain(accountModel);

                ////To check accout login status
                //Thread.Sleep(TimeSpan.FromSeconds(3));
                ////var checkLoginResp = HttpHelper.GetRequest(PdConstants.Https + PinFunct.Domain).Response;
                //checkLoginResp = resp;

                //if (!string.IsNullOrEmpty(checkLoginResp) && Utilities.GetBetween(resp, "status\":\"", "\"").Equals("success"))
                //{
                //    var userRegTrackActUrl = "https://www.pinterest.com/resource/UserRegisterTrackActionResource/update/";

                //    PinFunct.SetRequestHeaders(accountModel);
                //    var userActRegTrackActReq = HttpHelper.GetRequestParameter();
                //    userActRegTrackActReq.AddHeader("X-APP-VERSION", appVersion);
                //    //userActRegTrackActReq.AddHeader("X-Pinterest-ExperimentHash", experimentHash);
                //    userActRegTrackActReq.Headers["Sec-Fetch-Site"] = "same-origin";
                //    userActRegTrackActReq.Headers["Sec-Fetch-Mode"] = "same-origin";
                //    userActRegTrackActReq.Headers["Sec-Fetch-Dest"] = "empty";

                //    HttpHelper.SetRequestParameter(userActRegTrackActReq);

                //    try
                //    {
                //        var postDataForRegTrackAct = new PdRequestParameters
                //        { PdPostElements = JsonFunct.GetPostDataForUserRegTrackActRes(Domain(accountModel)) }.GetPostDataFromJson();
                //        var failedCount = 0;
                //        TryAgain:
                //        var UpdateResponse=HttpHelper.PostRequest(userRegTrackActUrl, postDataForRegTrackAct);
                //        while (failedCount++ <= 2 && UpdateResponse is null||!UpdateResponse.Response.Contains("\"status\":\"success"))
                //            goto TryAgain;
                //    }
                //    catch (Exception ex)
                //    {
                //        var e=ex.Message;
                //    }
                //    requestHeader = PinFunct.SetHeadersToChangeDomain(accountModel);
                //    Thread.Sleep(TimeSpan.FromSeconds(5));
                //    checkLoginResp = HttpHelper.GetRequest(PdConstants.Https + PinFunct.Domain).Response;
                //}
                    
                //#region For Handshake Request and Logout
                //if (!string.IsNullOrEmpty(checkLoginResp) && !checkLoginResp.Contains("\"isLoggedIn\": true") && !checkLoginResp.Contains("\"isLoggedIn\":true")&&!checkLoginResp.Contains(PdConstants.CheckLoginStatusString))
                //    checkLoginResp = PerformHandShakeRequest(accountModel, resp, appVersion, experimentHash);

                ////To perform logout thereafter handshake 
                //if (!string.IsNullOrEmpty(checkLoginResp) && !checkLoginResp.Contains("\"isLoggedIn\": true") && !checkLoginResp.Contains("\"isLoggedIn\":true") && !checkLoginResp.Contains(PdConstants.CheckLoginStatusString))
                //{
                //    var reqLogout = PinFunct.SetHeaders(accountModel);
                //    reqLogout.Headers["X-APP-VERSION"] = appVersion;
                //    reqLogout.Headers["X-Pinterest-ExperimentHash"] = experimentHash;
                //    reqLogout.Headers["Sec-Fetch-Site"] = "same-origin";
                //    reqLogout.Headers["Sec-Fetch-Mode"] = "cors";
                //    reqLogout.Headers["Sec-Fetch-Dest"] = "empty";
                //    HttpHelper.SetRequestParameter(reqLogout);

                //    var logoutPostData = new PdRequestParameters
                //    { PdPostElements = JsonFunct.GetPostDataForLogout(accountModel) }.GetPostDataFromJson();

                //    HttpHelper.PostRequest($"https://{PinFunct.Domain}/resource/UserSessionResource/delete/", logoutPostData);

                //    checkLoginResp = PerformHandShakeRequest(accountModel, resp, appVersion, experimentHash);
                //}
                //#endregion

                #endregion
                if (!LoginSucces(accountModel, string.IsNullOrEmpty(PinFunct.Domain), checkLoginResp))
                    return;

                accountModel.Cookies = HttpHelper.GetRequestParameter().Cookies;

                //Check for Account Mode Status
                CheckAccountBusinessModeStatus(accountModel, checkLoginResp);

                // set account cookies in request header
                if (requestHeader.Cookies.Count == 0)
                    requestHeader.Cookies = accountModel.Cookies;
                else if(requestHeader.Cookies.Count < accountModel.BrowserCookies.Count)
                    requestHeader.Cookies = accountModel.BrowserCookies;
                accountModel.Cookies = accountModel.BrowserCookies;
                HttpHelper.SetRequestParameter(requestHeader);
                sessionManager.AddOrUpdateSession(ref accountModel,true);
            }
            catch (OperationCanceledException)
            {
                if (!accountModel.AccountBaseModel.Status.Equals(AccountStatus.Success))
                    accountModel.AccountBaseModel.Status = AccountStatus.NotChecked;
                return;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private string PerformHandShakeRequest(DominatorAccountModel accountModel, string resp, string appVersion, string experimentHash)
        {
            var unauthId = Utilities.GetBetween(resp, "unauth_id\":\"", "\"");
            PinFunct.SetHeadersForHandShake(accountModel, unauthId);
            var handShakePostData = $"username_or_email={Uri.EscapeDataString(accountModel.AccountBaseModel.UserName)}&password={Uri.EscapeDataString(accountModel.AccountBaseModel.Password)}&";
            var handShakeResp = HttpHelper.PostRequest("https://accounts.pinterest.com/v3/login/handshake/", handShakePostData);

            var jsonResp = new JsonHandler(handShakeResp.Response);
            var token = jsonResp.GetElementValue("data");

            PinFunct.SetHeadersForHandShakePost(accountModel, appVersion, experimentHash);

            var requestParameters = new PdRequestParameters
            { PdPostElements = JsonFunct.GetPostDataFromLoginHandshake(token) };

            var handShakeSessionPostData = requestParameters.GetPostDataFromJson();
            HttpHelper.PostRequest($"https://{PinFunct.Domain}/resource/HandshakeSessionResource/create/", handShakeSessionPostData);

            var checkLoginResp = HttpHelper.GetRequest($"https://{PinFunct.Domain}" + "/").Response;

            return checkLoginResp;
        }

        void ProxyStringCheck(DominatorAccountModel accountModel)
        {
            var proxy = accountModel.AccountBaseModel.AccountProxy?.ProxyIp;
            if (!string.IsNullOrEmpty(proxy))
            {
                accountModel.AccountBaseModel.AccountProxy.ProxyIp = proxy.Contains("http://") ? proxy.Replace("http://", "")
                    : proxy.Contains("https://") ? proxy.Replace("https://", "") : proxy;
            }
        }

        private bool IsntEmail(DominatorAccountModel accountModel)
        {
            if (!accountModel.AccountBaseModel.UserName.Contains("@"))
            {
                accountModel.AccountBaseModel.Status = AccountStatus.InvalidCredentials;
                accountModel.IsUserLoggedIn = false;
                return true;
            }
            return false;
        }

        IRequestParameters NewReqParam()
        {
            var reqParam = HttpHelper.GetRequestParameter();
            reqParam.Cookies = new CookieCollection();
            reqParam.Headers = new WebHeaderCollection();
            reqParam.Accept = null;
            reqParam.ContentType = null;
            reqParam.Referer = null;
            reqParam.UserAgent = null;
            HttpHelper.SetRequestParameter(reqParam);
            return reqParam;
        }

        async Task<IResponseParameter> HitFrontPage(string defaultUrl, CancellationToken cancellationToken)
        {
            IResponseParameter firstreponce = null;
            int delayTime = 0;
            do
            {
                Sleep();
                firstreponce = await HttpHelper.GetRequestAsync(defaultUrl, cancellationToken);
                if (delayTime++ > 2)
                    break;
            }
            while (string.IsNullOrEmpty(firstreponce?.Response) || HttpHelper.GetRequestParameter()?.Cookies?.Count == 0);

            return firstreponce;
        }

        async Task<string> PostHit(string defaultUrl, byte[] postData, CancellationToken cancellationToken)
        {
            string responce = null;
            var delayTime = 1;
            do
            {
                cancellationToken.ThrowIfCancellationRequested();
                Sleep();
                responce = (await HttpHelper.PostRequestAsync(defaultUrl + PdConstants.Resource + PdConstants.UserFollowersResourceCreate
                    , postData, cancellationToken)).Response;
                if (delayTime++ > 2)
                    break;
            }
            while (string.IsNullOrEmpty(responce));
            return responce;
        }

        bool GotErrorAfterPostHit(DominatorAccountModel accountModel, string response)
        {
            try
            {
                if (string.IsNullOrEmpty(response))
                {
                    accountModel.AccountBaseModel.Status = AccountStatus.Failed;
                    accountModel.IsUserLoggedIn = false;
                    return true;
                }

                var jsonHand = new JsonHandler(response);
                if (string.IsNullOrEmpty(jsonHand.GetElementValue("resource_response", "data")) && jsonHand.GetJToken("resource_response", "error").Count() != 0)
                {
                    var errorMessage = jsonHand.GetElementValue("resource_response", "error", "message");
                    var httpStatus = jsonHand.GetElementValue("resource_response", "error", "http_status");

                    accountModel.IsUserLoggedIn = false;

                    if (response.Contains("There's been some strange activity on your account, so we put it in safe mode to protect your Pins. To get going again, just reset your password.")
                        ||response.Contains("We reset your password to keep your Pins safe"))
                        accountModel.AccountBaseModel.Status = AccountStatus.SetNewPassword;

                    else if (errorMessage.Equals("Sorry! We've had to block actions (logins) from your area because of suspicious activity. Please try again later.") ||
                        errorMessage.Equals("Sorry! You've hit a block (logins) we have in place to combat spam. Please try again later!"))
                        accountModel.AccountBaseModel.Status = AccountStatus.TemporarilyBlocked;

                    else if (response.Contains("The password you entered is incorrect") || response.Contains("that password isn't right") || httpStatus == "401")
                        accountModel.AccountBaseModel.Status = AccountStatus.InvalidCredentials;

                    else
                        accountModel.AccountBaseModel.Status = AccountStatus.Failed;
                    return true;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return false;
        }

        bool IfProxyFailed(DominatorAccountModel accountModel, string proxyFailed)
        {
            if (string.IsNullOrEmpty(proxyFailed))
                return false;

            PdStatic.FailedLog(accountModel, proxyFailed);
            accountModel.AccountBaseModel.Status = !string.IsNullOrEmpty(accountModel.AccountBaseModel.AccountProxy.ProxyIp) ?
                                                             AccountStatus.ProxyNotWorking : AccountStatus.Failed;
            accountModel.IsUserLoggedIn = false;
            return true; ;
        }

        bool LoginSucces(DominatorAccountModel accountModel, bool originEmpty, string response)
        {
            if (!originEmpty && !string.IsNullOrEmpty(response) && (response.Contains(accountModel.AccountBaseModel.UserName)
                || response.Contains("pinHolder") || response.Contains("header1\": \"What are you interested in?")
                || response.Contains("\"isLoggedIn\": true") || response.Contains("\"isLoggedIn\":true")
                || response.Contains("domainVerified") || response.Contains(PdConstants.CheckLoginStatusString)
                || response.Contains("\"login_state\":1")))
            {
                accountModel.AccountBaseModel.Status = AccountStatus.Success;
                accountModel.IsUserLoggedIn = true;
                adsresponse = response;
                return true;
            }
            else
            {
                accountModel.IsUserLoggedIn = false;
                accountModel.AccountBaseModel.Status = !string.IsNullOrEmpty(response) && response.Contains("Authentication failure, your IP is not authorized to access this proxy.")
                                                               ? AccountStatus.ProxyNotWorking : AccountStatus.Failed;
                return false;
            }
        }

        void Sleep(int min = 3, int max = 5) => Thread.Sleep(TimeSpan.FromSeconds(new Random().Next(min, max)));

        string ProxyFailedReason(IResponseParameter response)
        {
            if (response.Exception != null &&
                (response.Exception.Message.Contains("The remote name could not be resolved:") ||
                 response.Exception.Message == "Unable to connect to the remote server"))
                return "LangKeyUnableToConnectToTheRemoteServer".FromResourceDictionary();

            if (response.Response.Contains("Authentication failure, your IP is not authorized to access this proxy."))
                return "LangKeyAuthenticationFailureIPIsNotAuthorized".FromResourceDictionary();

            if (response.Response.Contains("Please ensure this IP is set in your Authorized IP list"))
                return "LangKeyEnsureThisIPIsSetInAuthorizedIPList".FromResourceDictionary();

            if (response.Response.Contains("Proxy Authentication Required"))
                return "LangKeyProxyAuthenticationRequired".FromResourceDictionary();

            if (response.Response.Contains("Access control configuration prevents your request from being allowed at this time. Please contact your service provider if you feel this is incorrect."))
                return "LangKeyAccessControlConfigurationPreventsRequestAllowedAtThisTime".FromResourceDictionary();

            if (response.Response.Contains("The server encountered an internal error or\nmisconfiguration and was unable to complete\nyour request"))
                return "LangKeyServerEncounteredAnInternalError".FromResourceDictionary();

            if (response.Response.Contains("ERR_FORWARDING_DENIED"))
                return "LangKeyERRFORWARDINGDENIED".FromResourceDictionary();

            if (response.Response.Contains("ERR_ACCESS_DENIED"))
                return "LangKeyERRACCESSDENIED".FromResourceDictionary();

            return "";
        }
        private void CheckAccountBusinessModeStatus(DominatorAccountModel accountModel, string response)
        {
            try
            {
                if (string.IsNullOrEmpty(response))
                    return;

                var checkWithBusinessProfile = BrowserUtilities.GetAttributeNameWithInnerText(response, "div", "Analytics");
                if (!string.IsNullOrEmpty(checkWithBusinessProfile) || response.Contains("Business Hub") || response.Contains("/business/hub/"))
                {
                    accountModel.AccountBaseModel.PinterestAccountType = PinterestAccountType.Active;
                    accountModel.DisplayColumnValue11 = PinterestAccountType.Active.GetDescriptionAttr().FromResourceDictionary();
                }
                else
                {
                    accountModel.AccountBaseModel.PinterestAccountType = PinterestAccountType.Inactive;
                    accountModel.DisplayColumnValue11 = PinterestAccountType.Inactive.GetDescriptionAttr().FromResourceDictionary();
                }
            }
            catch
            {
                // ignored
            }
        }
    }
}