using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.BusinessLogic.ActivitiesWorkflow;
using DominatorHouseCore.DatabaseHandler.DHTables;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.EmailService;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Extensions;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.Config;
using DominatorHouseCore.Utility;
using GramDominatorCore.CaptchaSolvers;
using GramDominatorCore.GDEnums;
using GramDominatorCore.GDFactories;
using GramDominatorCore.GDLibrary.InstagramBrowser;
using GramDominatorCore.GDLibrary.Response;
using GramDominatorCore.GDModel;
using GramDominatorCore.Request;
using GramDominatorCore.Response;
using GramDominatorCore.Utility;
using GramDominatorCore.WebResponseHandler;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using ThreadUtils;
using Unity;

namespace GramDominatorCore.GDLibrary
{

    public interface IGdLogInProcess : ILoginProcessAsync
    {
        // bool CheckLogin(DominatorAccountModel dominatorAccountModel);
        // Task<bool> CheckLoginAsync(DominatorAccountModel dominatorAccountModel, CancellationToken token);
        // void LoginWithDataBaseCookies(DominatorAccountModel dominatorAccountModel, bool isMobileRequired);
        // Task LoginWithDataBaseCookiesAsync(DominatorAccountModel dominatorAccountModel, bool isMobileRequired,CancellationToken token);
        //Task LoginWithAlternativeMethodAsync(DominatorAccountModel dominatorAccountModel, CancellationToken token);
        //Task SetSessionCookieMobileAsync(DominatorAccountModel dominatorAccountModel);
        //void LoginWithAlternativeMethod(DominatorAccountModel dominatorAccountModel);
        bool LoginWithAlternativeMethodForBlocking(DominatorAccountModel dominatorAccountModel);
        void LoginWithMobileDevice(DominatorAccountModel dominatorAccountModel, CancellationToken token);
        void CheckProxy(DominatorAccountModel dominatorAccountModel);
        Task LoginWithMobileDeviceAsync(DominatorAccountModel dominatorAccountModel, CancellationToken token);
        bool AutoVerifyViaEmail(DominatorAccountModel dominatorAccountModel, AccountModel accountModel,
            CancellationToken token, LoginIgResponseHandler logInResponseHandler);
        Task CheckisLoggedinMobileAsync(DominatorAccountModel dominatorAccountModel, AccountModel accountModel);
        Task InitializeRequestParameterIgAsync(DominatorAccountModel accountModel, AccountModel Model);
        Task GetUserInfoAndSetIntoUiWithoutLoginAsync(DominatorAccountModel dominatorAccountModel, AccountModel accountModel);

        Task<bool> TwoFactorLoginProcess(DominatorAccountModel dominatorAccountModel, AccountModel accountModel,
            IRequestParameters requestParameter, CancellationToken token);

        Task<bool> SendSecurityCodeTwoFactorLoginProcess(DominatorAccountModel dominatorAccountModel,
            AccountModel accountModel, IRequestParameters requestParameter, VerificationType verificationType, CancellationToken token);

        Task<bool> SendSecurityCodeAsync(DominatorAccountModel dominatorAccountModel, CancellationToken token, VerificationType verificationType,
            bool isAutoVerification = false);

        Task<bool> VerifyAccountAsync(DominatorAccountModel dominatorAccountModel, CancellationToken token);
        IInstaFunction AssignBrowserFunction(DominatorAccountModel dominatorAccountModel);

        IGdHttpHelper GetHttpHelper();
        IGdBrowserManager GdBrowserManager { get; set; }
        IInstaFunctFactory InstagramFunctFactory { get; set; }

        Task<bool> ManualCaptchaAsync(DominatorAccountModel dominatorAccountModel, CancellationToken token);

    }

    public class LogInProcess : IGdLogInProcess
    {
        // make it protected if issue arise in processors
        private IGdHttpHelper _httpHelper { get; set; }
        private IGdHttpHelper CheckProxyhttpHelper { get; set; }
        private IGdHttpHelper httpHelper { get; set; }
        private IAccountScopeFactory _accountScopeFactory;
        public IGdBrowserManager GdBrowserManager { get; set; }
        public IInstaFunctFactory InstagramFunctFactory { get; set; }
        public IDelayService DelayService { get; set; }
        public IBrowserManagerFactory _browserManagerFactory { get; set; }


        public LogInProcess(IGdHttpHelper httpHelper, IAccountScopeFactory accountScopeFactory, 
                            IGdBrowserManager gdBrowserManager, IInstaFunctFactory instaFunctFactory, 
                            IDelayService _delayService, IBrowserManagerFactory browserManagerFactory)
        {
            _httpHelper = httpHelper;
            CheckProxyhttpHelper = httpHelper;
            _accountScopeFactory = accountScopeFactory;
            GdBrowserManager = gdBrowserManager;
            InstagramFunctFactory = instaFunctFactory;
            DelayService = _delayService;
            _browserManagerFactory = browserManagerFactory;
        }

        IInstaFunction instaFunct => InstagramFunctFactory.InstaFunctions;

        public bool CheckLogin(DominatorAccountModel dominatorAccountModel, CancellationToken token)
        {
            try
            {
                if (!dominatorAccountModel.IsUserLoggedIn ||
                    (_httpHelper.GetRequestParameter().Cookies == null))
                {
                    LoginWithMobileDevice(dominatorAccountModel, dominatorAccountModel.Token);
                }

            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (AggregateException ae)
            {
                ae.HandleOperationCancellation();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return dominatorAccountModel.IsUserLoggedIn;
        }

        public async Task<bool> CheckLoginAsync(DominatorAccountModel dominatorAccountModel, CancellationToken token, bool displayLoginMsg = false, LoginType loginType = LoginType.AutomationLogin)
        {
            try
            {
                if (!dominatorAccountModel.IsRunProcessThroughBrowser)
                    await LoginWithMobileDeviceAsync(dominatorAccountModel, token);
                else
                   await Task.Run(()=> LoginWithBrowserMethod(dominatorAccountModel, token, loginType: loginType));
                if (dominatorAccountModel.IsRunProcessThroughBrowser && dominatorAccountModel.AccountBaseModel.Status == AccountStatus.Success && dominatorAccountModel.IsUserLoggedIn)
                {
                    GlobusLogHelper.log.Info(Log.SuccessfulLogin, dominatorAccountModel.AccountBaseModel.AccountNetwork,
                    dominatorAccountModel.AccountBaseModel.UserName);
                }
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (AggregateException ae)
            {
                ae.HandleOperationCancellation();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return dominatorAccountModel.IsUserLoggedIn;
        }

        public void LoginWithDataBaseCookies(DominatorAccountModel dominatorAccountModel, bool isMobileRequired, CancellationToken token)
        {
            LoginWithDataBaseCookiesAsync(dominatorAccountModel, isMobileRequired, dominatorAccountModel.Token).Wait();
        }

        public async Task LoginWithDataBaseCookiesAsync(DominatorAccountModel dominatorAccountModel, bool isMobileRequired, CancellationToken token)
        {
            var AccountModel = new AccountModel(dominatorAccountModel);
            if (isMobileRequired)
            {
                try
                {
                    try
                    {
                        IRequestParameters requestParamter = _httpHelper.GetRequestParameter();
                        requestParamter.Cookies = dominatorAccountModel.Cookies;
                        _httpHelper.SetRequestParameter(requestParamter);
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                    if (_httpHelper.GetRequestParameter()?.Cookies?.Count == 0)
                    {
                        SetSessionCookieMobileAsync(dominatorAccountModel);
                        await InitializeRequestParameterIgAsync(dominatorAccountModel, AccountModel);
                        await CheckisLoggedinMobileAsync(dominatorAccountModel, AccountModel);
                    }

                    if (!(dominatorAccountModel.IsUserLoggedIn))
                    {
                        //try
                        //{
                        LoginWithMobileDevice(dominatorAccountModel, token);
                        //}
                        //catch (Exception ex)
                        //{
                        //    ex.DebugLog();
                        //}
                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog();

                }
            }
        }

        public Task LoginWithAlternativeMethodAsync(DominatorAccountModel dominatorAccountModel, CancellationToken token) => LoginWithMobileDeviceAsync(dominatorAccountModel, token);

        public void SetSessionCookieMobileAsync(DominatorAccountModel dominatorAccountModel)
        {
            try
            {
                if (dominatorAccountModel.Cookies != null && dominatorAccountModel.Cookies.Count != 0)
                    _httpHelper.GetRequestParameter().Cookies = dominatorAccountModel.Cookies;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        public void LoginWithAlternativeMethod(DominatorAccountModel dominatorAccountModel, CancellationToken token) => LoginWithMobileDevice(dominatorAccountModel, dominatorAccountModel.Token);
        public bool LoginWithAlternativeMethodForBlocking(DominatorAccountModel dominatorAccountModel)
        {
            if (!LoginWithMobileDeviceAsyncBlocking(dominatorAccountModel, dominatorAccountModel.Token).Result)
                return false;
            return true;
        }
        public void LoginWithMobileDevice(DominatorAccountModel dominatorAccountModel, CancellationToken token)
        {
            try
            {
                LoginWithMobileDeviceAsync(dominatorAccountModel, token).Wait(token);
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (AggregateException ae)
            {
                ae.HandleOperationCancellation();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void CheckProxy(DominatorAccountModel dominatorAccountModel)
        {
            string response = string.Empty;
            try
            {
                // if (string.IsNullOrEmpty(dominatorAccountModel.AccountBaseModel.AccountProxy?.ProxyIp?.Trim())) return false;
                var scope = InstanceProvider.GetInstance<IAccountScopeFactory>();
                httpHelper = scope[dominatorAccountModel.AccountId].Resolve<IGdHttpHelper>();
                IRequestParameters requestParameters = _httpHelper.GetRequestParameter();
                requestParameters.Proxy = new Proxy();
                requestParameters.Proxy.ProxyIp = dominatorAccountModel.AccountBaseModel.AccountProxy.ProxyIp;
                requestParameters.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.75 Safari/537.36 Edg/86.0.622.38";
                requestParameters.Accept = "*/*";
                requestParameters.Headers.Add("Accept-Language", "en-US,en;q=0.9");
                requestParameters.KeepAlive = true;
                requestParameters.ContentType = "application/x-www-form-urlencoded";
                requestParameters.AddHeader("Host", "ipinfo.io");
                requestParameters.AddHeader("Referer", "https://ipinfo.io/");
                requestParameters.AddHeader("Sec-Fetch-Site", "same-origin");
                requestParameters.AddHeader("Sec-Fetch-Mode", "cors");
                requestParameters.AddHeader("Sec-Fetch-Dest", "empty");
                _httpHelper.SetRequestParameter(requestParameters);
                var url = "https://ipinfo.io/widget/" + dominatorAccountModel.AccountBaseModel.AccountProxy.ProxyIp;
                _httpHelper.SetRequestParameter(requestParameters);
                response = httpHelper.GetRequest(url).Response;
            }
            catch
            {

            }
        }

        private async Task RunNeededRequestByStep(int step, DominatorAccountModel dominatorAccountModel, AccountModel accountModel)
        {
            var delay = RandomUtilties.GetRandomNumber(1500, 500);
            DelayService.ThreadSleep(TimeSpan.FromMilliseconds(delay));
            switch (step)
            {
                case 0:
                    await instaFunct.GetReelsTrayFeed(dominatorAccountModel, accountModel);
                    break;
                case 1:
                    await instaFunct.GetBlockedMedia(dominatorAccountModel, accountModel);
                    break;
                case 2:
                    instaFunct.GetStoriesUsers(dominatorAccountModel, accountModel, new List<string>() { dominatorAccountModel.AccountBaseModel.UserId });
                    break;
                case 3:
                    await instaFunct.GetRecentActivity(dominatorAccountModel);
                    break;
                case 4:
                    await instaFunct.GetLinkageStatus(dominatorAccountModel, accountModel);
                    break;
                case 5:
                    await instaFunct.Scores(dominatorAccountModel, accountModel);
                    break;
                default:
                    break;
            }
        }

        //ToDo: Move to Utilities
        public static void Shuffle(int[] arr)
        {
            var keys = arr.Select(e => RandomUtilties.ObjRandom.NextDouble()).ToArray();
            Array.Sort(keys, arr);
        }

        public async Task HittinAllNeededRequest(DominatorAccountModel dominatorAccountModel, AccountModel accountModel, CancellationToken token)
        {
            //Requests can looks like suspicious in case we call it evenly
            var steps = Enumerable.Range(0, 5).ToArray();
            Shuffle(steps);
            
            foreach(var step in steps)
            {
                await RunNeededRequestByStep(step, dominatorAccountModel, accountModel);
            }

            /*
                DelayService.ThreadSleep(TimeSpan.FromMilliseconds(500));
                await instaFunct.GetReelsTrayFeed(dominatorAccountModel, accountModel);
                DelayService.ThreadSleep(TimeSpan.FromMilliseconds(500));
                await instaFunct.GetBlockedMedia(dominatorAccountModel, accountModel);
                DelayService.ThreadSleep(TimeSpan.FromMilliseconds(500));
                instaFunct.GetStoriesUsers(dominatorAccountModel, accountModel, new List<String>() { dominatorAccountModel.AccountBaseModel.UserId });
                DelayService.ThreadSleep(TimeSpan.FromMilliseconds(500));
                await instaFunct.GetRecentActivity();
                DelayService.ThreadSleep(TimeSpan.FromMilliseconds(500));
                await instaFunct.GetLinkageStatus(dominatorAccountModel, accountModel);
                DelayService.ThreadSleep(TimeSpan.FromMilliseconds(500));
                await instaFunct.Scores(dominatorAccountModel, accountModel);             
             */
            //await instaFunct.GetAccountFamily(dominatorAccountModel, accountModel);
            //DelayService.ThreadSleep(TimeSpan.FromMilliseconds(500));

            //await instaFunct.LauncherSyncAsyncAfterLogin(dominatorAccountModel, accountModel);
            //DelayService.ThreadSleep(TimeSpan.FromSeconds(1));
            //await instaFunct.SyncDeviceFeaturesAsyncAfterLogin(dominatorAccountModel, accountModel);

            //DelayService.ThreadSleep(TimeSpan.FromMilliseconds(500));
            //await instaFunct.SuggestedSearches(dominatorAccountModel, accountModel);
            //DelayService.ThreadSleep(TimeSpan.FromMilliseconds(500));
            //await instaFunct.RecentSearches(dominatorAccountModel, accountModel);
            //DelayService.ThreadSleep(TimeSpan.FromMilliseconds(500));
            //await instaFunct.SuggestedSearches1(dominatorAccountModel, accountModel);
            //DelayService.ThreadSleep(TimeSpan.FromMilliseconds(500));
            //await instaFunct.ranked_recipients(dominatorAccountModel, accountModel);
            //DelayService.ThreadSleep(TimeSpan.FromMilliseconds(500));
            //await instaFunct.ranked_recipients_Mode_Raven(dominatorAccountModel, accountModel);
            //DelayService.ThreadSleep(TimeSpan.FromSeconds(3));
            //await instaFunct.PersistentBadging(dominatorAccountModel, accountModel);
            //DelayService.ThreadSleep(TimeSpan.FromSeconds(3));
            //await instaFunct.GetPresence(dominatorAccountModel, accountModel);

            //DelayService.ThreadSleep(TimeSpan.FromMilliseconds(500));
            //await instaFunct.GetProfileNotice(dominatorAccountModel, accountModel);
            //DelayService.ThreadSleep(TimeSpan.FromMilliseconds(500));
            //await instaFunct.GetBlockedMedia(dominatorAccountModel, accountModel);

            //DelayService.ThreadSleep(TimeSpan.FromMilliseconds(500));
            //await instaFunct.topical_explore(dominatorAccountModel, accountModel);
            //DelayService.ThreadSleep(TimeSpan.FromMilliseconds(500));
            //await instaFunct.PushRegister(dominatorAccountModel, accountModel);
        }
        public async Task<bool> LoginWithMobileDeviceAsyncBlocking(DominatorAccountModel dominatorAccountModel, CancellationToken token)
        {
            try
            {
                AccountModel accountModel = new AccountModel(dominatorAccountModel);
                await InitializeRequestParameterIgAsync(dominatorAccountModel, accountModel);
                LoginIgResponseHandler logInResponseHandler = await instaFunct.LoginAsync(dominatorAccountModel, accountModel);
                if (logInResponseHandler.Success)
                {
                    if (_httpHelper.GetRequestParameter().Cookies != null && _httpHelper.GetRequestParameter().Cookies.Count > 4)
                    {
                        var isAutorized = _httpHelper.Response.Headers.GetValues("ig-set-authorization");
                        if (isAutorized != null)
                        {
                            _httpHelper.GetRequestParameter().Headers["ig-set-authorization"] = isAutorized[0].ToString();
                            accountModel.AuthorizationHeader = isAutorized[0].ToString();
                        }
                        var isMid = _httpHelper.Response.Headers.GetValues("ig-set-x-mid");
                        if (isMid != null)
                        {
                            _httpHelper.GetRequestParameter().Headers["ig-set-x-mid"] = isMid[0].ToString();
                            accountModel.MidHeader = isMid[0].ToString();
                        }
                        //dominatorAccountModel.SessionId = _httpHelper.GetRequestParameter().Cookies["sessionid"].Value;
                        //accountModel.CsrfToken = _httpHelper.GetRequestParameter().Cookies["csrftoken"].Value;
                    }
                    DelayService.ThreadSleep(TimeSpan.FromSeconds(2));
                    var resp = instaFunct.GetFeedTimeLineData(dominatorAccountModel, accountModel);
                    if (resp.ToString().Contains("challenge_required"))
                        return false;
                    if (string.IsNullOrEmpty(dominatorAccountModel.AccountBaseModel.ProfileId))
                        dominatorAccountModel.AccountBaseModel.ProfileId = logInResponseHandler.Username;
                    SetAccountDetails(dominatorAccountModel, accountModel, logInResponseHandler.Pk, isBlock: true);
                    await HittinAllNeededRequest(dominatorAccountModel, accountModel, token);
                    DelayService.ThreadSleep(TimeSpan.FromSeconds(5));
                    return true;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return false;
        }
        public async Task IntialLoginForCaptcha(DominatorAccountModel dominatorAccountModel, AccountModel accountModel, CancellationToken token)
        {
            await InitializeRequestParameterIgAsync(dominatorAccountModel, accountModel);
            IRequestParameters requestParameter = _httpHelper.GetRequestParameter();
            if (dominatorAccountModel.Cookies.Count <= 4 || accountModel.publicKey == null)
            {
                await instaFunct.SyncDeviceFeaturesAsyncBeforeLogin(dominatorAccountModel, accountModel);
                await instaFunct.B_ZrTokenAsyncBeforeLogin(dominatorAccountModel, accountModel);
                string x_mid = _httpHelper.Response.Headers["ig-set-x-mid"].Trim(',');
                _httpHelper.GetRequestParameter().AddHeader("X-MID", x_mid);
                accountModel.publicId = _httpHelper.Response.Headers["ig-set-password-encryption-key-id"];
                accountModel.publicKey = _httpHelper.Response.Headers["ig-set-password-encryption-pub-key"];
                if (_httpHelper.GetRequestParameter().Cookies != null && _httpHelper.GetRequestParameter().Cookies.Count != 0 && string.IsNullOrEmpty(accountModel.CsrfToken))
                    accountModel.CsrfToken = _httpHelper.GetRequestParameter().Cookies["csrftoken"].Value?.ToString();
            }
            PasswordEncryptionProcess.EncrptLoginPassword(dominatorAccountModel, accountModel);
            LoginIgResponseHandler logInResponseHandler = await instaFunct.LoginAsync(dominatorAccountModel, accountModel);
            await AfterLoginProcess(dominatorAccountModel, accountModel, logInResponseHandler, requestParameter, token);
        }
        public async Task LoginWithMobileDeviceAsync(DominatorAccountModel dominatorAccountModel, CancellationToken token)
        {
            //return;  //only for testing a client folder
            AccountModel accountModel = new AccountModel(dominatorAccountModel);
            if (string.IsNullOrEmpty(accountModel.AuthorizationHeader))
            {
                accountModel.AuthorizationHeader = "";
                accountModel.WwwClaim = "";
            }

            if (accountModel.AccountStatus != AccountStatus.Success.ToString().Trim())
            {
                var globalDbOperation = new DbOperations(SocinatorInitialize.GetGlobalDatabase().GetSqlConnection());
                var accounts = globalDbOperation.Get<InstaAccountBackup>().FirstOrDefault(x => x.AccountName == dominatorAccountModel.UserName);
                if (accounts != null)
                {
                    accountModel.Device_Id = accounts.DeviceId;
                }

            }
            if (AccountStatus.TemporarilyBlocked.Equals(accountModel.AccountStatus))
            {
                accountModel.Device_Id = dominatorAccountModel.DeviceDetails.DeviceId;
            }
            try
            {

                await InitializeRequestParameterIgAsync(dominatorAccountModel, accountModel);
                if (!string.IsNullOrEmpty(dominatorAccountModel.AccountBaseModel.AccountProxy.ProxyIp))
                {
                    await checkingProxy(dominatorAccountModel, accountModel);
                    if (AccountStatus.ProxyNotWorking == dominatorAccountModel.AccountBaseModel.Status)
                        return;
                    await InitializeRequestParameterIgAsync(dominatorAccountModel, accountModel);
                }
                if (AccountStatus.ProxyNotWorking == dominatorAccountModel.AccountBaseModel.Status)
                    return;
                //if(!string.IsNullOrEmpty(accountModel.AuthorizationHeader))
                //{
                //    await CheckisLoggedinMobileAsync(dominatorAccountModel, accountModel);
                //}
                
                IRequestParameters requestParameter = _httpHelper.GetRequestParameter();
                if (!string.IsNullOrEmpty(accountModel.AuthorizationHeader))
                {
                    //GetUserInfoAndSetIntoUiWithoutLoginAsync(dominatorAccountModel, accountModel);
                    await CheckisLoggedinMobileAsync(dominatorAccountModel, accountModel);
                }
                if (dominatorAccountModel.IsloggedinWithPhone && !(string.IsNullOrEmpty(accountModel.AuthorizationHeader)))
                {
                    await HittinAllNeededRequest(dominatorAccountModel, accountModel, token);
                    SetAccountDetails(dominatorAccountModel, accountModel);
                    if (!InstagramDataScraper(dominatorAccountModel, accountModel))
                    {
                        dominatorAccountModel.AccountBaseModel.Status = AccountStatus.Success;
                        return;
                    }
                }
                await GetHeadersBeforeLogin(dominatorAccountModel, accountModel);
                PasswordEncryptionProcess.EncrptLoginPassword(dominatorAccountModel, accountModel);
                var splitEncryptedPassword = accountModel?.EncPwd?.Split(':')[3] ?? "";

                if (string.IsNullOrEmpty(splitEncryptedPassword))
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                        dominatorAccountModel.AccountBaseModel.AccountNetwork,
                        dominatorAccountModel.AccountBaseModel.UserName, "Login",
                        "Account failed to login, its unable to create password, so please try to login after some times");
                    dominatorAccountModel.AccountBaseModel.Status = AccountStatus.Failed;
                    SetLoginStatus(dominatorAccountModel, accountModel, AccountStatus.Failed, false);
                    return;
                }
                if (!await StartTwoFactorLogin(dominatorAccountModel, accountModel, requestParameter, token))
                    return;
                var isKeyboardSuccess = await instaFunct.TypeKeyboardPreLoginAsync(dominatorAccountModel, accountModel);
                LoginIgResponseHandler logInResponseHandler = await instaFunct.LoginAsync(dominatorAccountModel, accountModel);
                await AfterLoginProcess(dominatorAccountModel, accountModel, logInResponseHandler, requestParameter, token);              //{"message": "challenge_required", "challenge": {"url": "https://i.instagram.com/challenge/?next=/api/v1/accounts/login/", "api_path": "/challenge/", "hide_webview_header": true, "lock": true, "logout": false, "native_flow": true, "flow_render_type": 0}, "status": "fail"}
                //torAccountModel, accountModel, logInResponseHandler, requestParameter, token);
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (AggregateException ae)
            {
                ae.HandleOperationCancellation();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }



        public async Task<bool> SendSecurityCodeAsync(DominatorAccountModel dominatorAccountModel, CancellationToken token, VerificationType verificationType, bool isAutoVerification = false)
        {
            try
            {
                if (dominatorAccountModel.IsRunProcessThroughBrowser)
                {
                    LoginWithBrowserMethod(dominatorAccountModel, token, verificationType);
                    return true;
                }
                AccountModel accountModel = new AccountModel(dominatorAccountModel);
                await InitializeRequestParameterIgAsync(dominatorAccountModel, accountModel);
                IRequestParameters requestParameter = _httpHelper.GetRequestParameter();

                await GetHeadersBeforeLogin(dominatorAccountModel, accountModel);
                PasswordEncryptionProcess.EncrptLoginPassword(dominatorAccountModel, accountModel);
                if (requestParameter.Cookies != null)
                    accountModel.CsrfToken = requestParameter?.Cookies["csrftoken"]?.Value ?? String.Empty;

                if (dominatorAccountModel.AccountBaseModel.IsChkTwoFactorLogin)
                {
                    bool status = await SendSecurityCodeTwoFactorLoginProcess(dominatorAccountModel, accountModel, requestParameter, verificationType, token);
                    if (status)
                    {
                        return true;
                    }
                    return false;
                }
                LoginIgResponseHandler logInResponseHandler = await instaFunct.LoginAsync(dominatorAccountModel, accountModel);
                if (logInResponseHandler != null && logInResponseHandler.Issue != null)
                {
                    if (logInResponseHandler.Issue.ApiPath.Equals("/challenge/"))
                    {
                        dominatorAccountModel.Cookies = null;
                        _httpHelper.GetRequestParameter().Cookies = null;
                        DelayService.ThreadSleep(TimeSpan.FromSeconds(10));
                        logInResponseHandler = await instaFunct.LoginAsync(dominatorAccountModel, accountModel);
                    }
                }

                if (logInResponseHandler.Success)
                {
                    try { if (string.IsNullOrEmpty(dominatorAccountModel.AccountBaseModel.ProfileId)) dominatorAccountModel.AccountBaseModel.ProfileId = logInResponseHandler.Username; } catch (Exception) { }
                    SetAccountDetails(dominatorAccountModel, accountModel, logInResponseHandler.Pk);
                    return false;
                }
                else if (!logInResponseHandler.Success && logInResponseHandler.Issue?.Error == InstagramError.Challenge)
                {
                    IResponseParameter response = await GetChallengeResponse(dominatorAccountModel, accountModel, requestParameter, logInResponseHandler, null, CancellationToken.None);

                    if (!response.Response.Contains("select_verify_method") && !response.Response.Contains("{\"action\": \"close\", \"status\": \"ok\"}") && !response.Response.ToString().Contains("bloks_action"))
                    {
                        accountModel.AccountVerifyType = "DifferentAccount";
                        response = await GetCodeSendingType(dominatorAccountModel, accountModel, requestParameter, logInResponseHandler, response, token);

                        string apiUrl = $"https://i.instagram.com{logInResponseHandler.Issue.ApiPath}";
                        CaptchaRequestResponseHandler afterCaptchaResponse = null;
                        SubmitPhoneNumberAfterCaptchaResponseHandler submitPhoneNumerResponse = null;
                        if (response.Response.ToString().Contains("sitekey") || response.Response.ToString().Contains("RecaptchaChallengeForm"))
                        {
                            try
                            {
                                var captchaToken = SolveCaptcha(response.Response.ToString(), apiUrl, dominatorAccountModel);
                                afterCaptchaResponse = instaFunct.SubmittingCaptchaSolution(dominatorAccountModel, accountModel, captchaToken, apiUrl, token);
                            }
                            catch (Exception ex)
                            {
                                ex.DebugLog();
                            }
                        }
                        if ((afterCaptchaResponse != null && afterCaptchaResponse.Issue.Error.ToString() == "SubmitPhoneNumber") || response.Response.Contains("SubmitPhoneNumberForm"))
                        {
                            string challengeUrl = string.Empty;
                            if (afterCaptchaResponse != null && afterCaptchaResponse.Issue != null && !string.IsNullOrEmpty(afterCaptchaResponse.Issue.ApiPath))
                                challengeUrl = afterCaptchaResponse.Issue.ApiPath;
                            else
                                challengeUrl = Utilities.GetBetween(response.Response.ToString(), "challenge_context\":\"", "\"}");

                            submitPhoneNumerResponse = instaFunct.ConfirmPhoneNumberRequest(dominatorAccountModel, accountModel, challengeUrl, dominatorAccountModel.AccountBaseModel.PhoneNumber, apiUrl, token);
                            // submitCodeResponse = instaFunct.SubmitPhoneNumberRequest(dominatorAccountModel, accountModel, challengeUrl, code, apiUrl, token);
                            if (submitPhoneNumerResponse.challengeType == "VerifySMSCodeFormForSMSCaptcha")
                            {
                                accountModel.apiUrl = apiUrl;
                                accountModel.challengeContext = challengeUrl;
                                accountModel.codeSubmittingtype = "VerifySMSCodeFormForSMSCaptcha";
                                try
                                {
                                    JToken jTokenStepdata = JObject.Parse(submitPhoneNumerResponse.ToString())["fields"];
                                    if (jTokenStepdata != null)
                                    {
                                        string loggerDescription = String.Empty;
                                        if (verificationType == VerificationType.FoundCaptcha)
                                            loggerDescription = $"ends with {jTokenStepdata["phone_number_preview"]}";
                                    }
                                    else
                                        GlobusLogHelper.log.Info(Log.SentVerificationCode, dominatorAccountModel.AccountBaseModel.AccountNetwork, dominatorAccountModel.UserName, verificationType);
                                }
                                catch (Exception ex)
                                {
                                    ex.DebugLog();
                                }
                                return true;
                            }

                        }
                    }
                    if (response.Response.ToString().Contains("UFACBlockingForm"))
                    {
                        dominatorAccountModel.AccountBaseModel.Status = AccountStatus.AccountCompromised;
                        SetLoginStatus(dominatorAccountModel, accountModel, AccountStatus.AccountCompromised, false);
                        GlobusLogHelper.log.Info(Log.CustomMessage,
                       dominatorAccountModel.AccountBaseModel.AccountNetwork,
                       dominatorAccountModel.AccountBaseModel.UserName, "Login", "we'll review your info and if we can confirm it,you'll be able to access your account within approximately 24 hours");
                        return false;
                    }
                    else
                    {
                        CommonIgResponseHandler sendSecurityCodeResponse = null;
                        if (!AccountChallenge(dominatorAccountModel, response))
                            return false;
                        //nonce_code=dHnr0Xvx3J
                        string challengeApiPathUrl = dominatorAccountModel.ChallengeUrl;
                        //JObject jObj = JObject.Parse(logInResponseHandler.ToString());
                        JObject jObj = JObject.Parse(response.Response.ToString());
                        //var context_challenges = jObj["challenge"]["challenge_context"].ToString();
                        var context_challenges = jObj["challenge_context"].ToString();
                        dominatorAccountModel.Challenge_Context = context_challenges;
                        JToken jToken = JObject.Parse(response.Response.ToString());
                        if (accountModel.AccountVerifyType == "DifferentAccount")
                            sendSecurityCodeResponse = await instaFunct.SendSecurityCodeAsyncForSomeAccounts(challengeApiPathUrl, context_challenges,
                           verificationType == VerificationType.Phone ? "0" : "1", dominatorAccountModel, accountModel);
                        else if (response.Response.ToString().Contains("com.instagram.challenge.navigation.take_challenge"))
                        {
                            sendSecurityCodeResponse = await instaFunct.ActionBlockSendSecurityCodeAsync(challengeApiPathUrl, context_challenges,
                                verificationType == VerificationType.Phone ? "0" : "1", dominatorAccountModel, accountModel);

                            var data = Regex.Split(sendSecurityCodeResponse.ToString(), "perf_logging_id")[1];
                            var check = Regex.Split(data, "bk.action.i32.Const")[2];
                            var preLoginId = check.Substring(1, 10);
                            accountModel.PerfLoggingId = preLoginId.Trim();
                        }
                        else
                            sendSecurityCodeResponse = await instaFunct.SendSecurityCodeAsync(challengeApiPathUrl, context_challenges,
                          verificationType == VerificationType.Phone ? "0" : "1", dominatorAccountModel, accountModel);


                        if (sendSecurityCodeResponse.ToString().Contains("{\"action\": \"close\", \"status\": \"ok\"}"))
                        {
                            AccountActionClose(dominatorAccountModel);
                            return false;
                        }

                        if (sendSecurityCodeResponse == null)
                        {
                            DelayService.ThreadSleep(TimeSpan.FromSeconds(5));
                            instaFunct.ResetSendRequest(challengeApiPathUrl, dominatorAccountModel, accountModel);
                            sendSecurityCodeResponse = await instaFunct.SendSecurityCodeAsync(challengeApiPathUrl, context_challenges,
                            verificationType == VerificationType.Phone ? "0" : "1", dominatorAccountModel, accountModel);
                        }

                        if (sendSecurityCodeResponse.Success)
                        {
                            try
                            {
                                // This region picks the short contact info
                                // (Last 4 digits of Phone no. or Email Id) and shows onto the UI
                                #region Retrieves Short Contact Info
                                JToken jTokenStepdata = JObject.Parse(sendSecurityCodeResponse.ToString())["step_data"];
                                if (jTokenStepdata != null)
                                {
                                    string loggerDescription = String.Empty;
                                    if (verificationType == VerificationType.Phone)
                                        loggerDescription = $"ends with {jTokenStepdata["phone_number_preview"]}";
                                    else if (verificationType == VerificationType.Email)
                                        loggerDescription = jTokenStepdata["contact_point"].ToString();

                                    GlobusLogHelper.log.Info(!isAutoVerification ? Log.SentVerificationCode : Log.SentVerificationCodeForAutoVerify, dominatorAccountModel.AccountBaseModel.AccountNetwork, dominatorAccountModel.UserName, $"{verificationType} {loggerDescription}");
                                }
                                else
                                    GlobusLogHelper.log.Info(Log.SentVerificationCode, dominatorAccountModel.AccountBaseModel.AccountNetwork, dominatorAccountModel.UserName, verificationType);
                                #endregion
                            }
                            catch (Exception ex)
                            {
                                ex.DebugLog();
                            }

                            return sendSecurityCodeResponse.Success;
                        }

                        GlobusLogHelper.log.Info(Log.FailedToSendVerificationCodeFaild, dominatorAccountModel.AccountBaseModel.AccountNetwork, dominatorAccountModel.UserName, verificationType);
                    }
                }
                else
                {
                    if (logInResponseHandler.Issue != null)
                        GlobusLogHelper.log.Info(Log.CustomMessage, dominatorAccountModel.AccountBaseModel.AccountNetwork,
                        dominatorAccountModel.UserName, "LangKeyVerification".FromResourceDictionary(), $"Could not able to login with Error Response : {logInResponseHandler.Issue.Message}");

                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();

                GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Instagram, dominatorAccountModel.UserName, "LangKeyVerification".FromResourceDictionary(),
                    "Could not able to send verificaton code.");
            }
            return true;
        }
        private async Task<bool> SolveCaptchaManualAndLoginAsync(DominatorAccountModel dominatorAccountModel, CancellationToken token)
        {
            try
            {
                AccountModel accountModel = new AccountModel(dominatorAccountModel);
                await InitializeRequestParameterIgAsync(dominatorAccountModel, accountModel);
                IRequestParameters requestParameter = _httpHelper.GetRequestParameter();

                await GetHeadersBeforeLogin(dominatorAccountModel, accountModel);
                PasswordEncryptionProcess.EncrptLoginPassword(dominatorAccountModel, accountModel);
                if (requestParameter.Cookies != null)
                    accountModel.CsrfToken = requestParameter?.Cookies["csrftoken"]?.Value ?? String.Empty;

                VerificationType verificationType = VerificationType.FoundCaptcha;
                LoginIgResponseHandler logInResponseHandler = await instaFunct.LoginAsync(dominatorAccountModel, accountModel);
                if (logInResponseHandler != null && logInResponseHandler.Issue != null)
                {
                    if (logInResponseHandler.Issue.ApiPath.Equals("/challenge/"))
                    {
                        dominatorAccountModel.Cookies = null;
                        _httpHelper.GetRequestParameter().Cookies = null;
                        DelayService.ThreadSleep(TimeSpan.FromSeconds(10));
                        logInResponseHandler = await instaFunct.LoginAsync(dominatorAccountModel, accountModel);
                    }
                }

                if (logInResponseHandler.Success)
                {
                    try { if (string.IsNullOrEmpty(dominatorAccountModel.AccountBaseModel.ProfileId)) dominatorAccountModel.AccountBaseModel.ProfileId = logInResponseHandler.Username; } catch (Exception) { }
                    SetAccountDetails(dominatorAccountModel, accountModel, logInResponseHandler.Pk);
                    return false;
                }
                else if (!logInResponseHandler.Success && logInResponseHandler.Issue?.Error == InstagramError.Challenge)
                {
                    IResponseParameter response = await GetChallengeResponse(dominatorAccountModel, accountModel, requestParameter, logInResponseHandler, null, CancellationToken.None);

                    if (!response.Response.Contains("select_verify_method") && !response.Response.Contains("{\"action\": \"close\", \"status\": \"ok\"}") && !response.Response.ToString().Contains("bloks_action"))
                    {
                        accountModel.AccountVerifyType = "DifferentAccount";
                        response = await GetCodeSendingType(dominatorAccountModel, accountModel, requestParameter, logInResponseHandler, response, token);

                        string apiUrl = $"https://i.instagram.com{logInResponseHandler.Issue.ApiPath}";
                        CaptchaRequestResponseHandler afterCaptchaResponse = null;
                        SubmitPhoneNumberAfterCaptchaResponseHandler submitPhoneNumerResponse = null;
                        if (response.Response.ToString().Contains("sitekey") || response.Response.ToString().Contains("RecaptchaChallengeForm"))
                        {
                            try
                            {
                                var captchaToken = await SolveCaptchaManualInBrowser();
                                afterCaptchaResponse = instaFunct.SubmittingCaptchaSolution(dominatorAccountModel, accountModel, captchaToken, apiUrl, token);
                            }
                            catch (Exception ex)
                            {
                                ex.DebugLog();
                            }
                        }
                        if ((afterCaptchaResponse != null && afterCaptchaResponse.Issue.Error.ToString() == "SubmitPhoneNumber") || response.Response.Contains("SubmitPhoneNumberForm"))
                        {
                            string challengeUrl = string.Empty;
                            if (afterCaptchaResponse != null && afterCaptchaResponse.Issue != null && !string.IsNullOrEmpty(afterCaptchaResponse.Issue.ApiPath))
                                challengeUrl = afterCaptchaResponse.Issue.ApiPath;
                            else
                                challengeUrl = Utilities.GetBetween(response.Response.ToString(), "challenge_context\":\"", "\"}");

                            submitPhoneNumerResponse = instaFunct.ConfirmPhoneNumberRequest(dominatorAccountModel, accountModel, challengeUrl, dominatorAccountModel.AccountBaseModel.PhoneNumber, apiUrl, token);
                            // submitCodeResponse = instaFunct.SubmitPhoneNumberRequest(dominatorAccountModel, accountModel, challengeUrl, code, apiUrl, token);
                            if (submitPhoneNumerResponse.challengeType == "VerifySMSCodeFormForSMSCaptcha")
                            {
                                accountModel.apiUrl = apiUrl;
                                accountModel.challengeContext = challengeUrl;
                                accountModel.codeSubmittingtype = "VerifySMSCodeFormForSMSCaptcha";
                                try
                                {
                                    JToken jTokenStepdata = JObject.Parse(submitPhoneNumerResponse.ToString())["fields"];
                                    if (jTokenStepdata != null)
                                    {
                                        string loggerDescription = String.Empty;
                                        if (verificationType == VerificationType.FoundCaptcha)
                                            loggerDescription = $"ends with {jTokenStepdata["phone_number_preview"]}";
                                    }
                                    else
                                        GlobusLogHelper.log.Info(Log.SentVerificationCode, dominatorAccountModel.AccountBaseModel.AccountNetwork, dominatorAccountModel.UserName, verificationType);
                                }
                                catch (Exception ex)
                                {
                                    ex.DebugLog();
                                }
                                return true;
                            }

                        }
                    }
                    if (response.Response.ToString().Contains("UFACBlockingForm"))
                    {
                        dominatorAccountModel.AccountBaseModel.Status = AccountStatus.AccountCompromised;
                        SetLoginStatus(dominatorAccountModel, accountModel, AccountStatus.AccountCompromised, false);
                        GlobusLogHelper.log.Info(Log.CustomMessage,
                       dominatorAccountModel.AccountBaseModel.AccountNetwork,
                       dominatorAccountModel.AccountBaseModel.UserName, "Login", "we'll review your info and if we can confirm it,you'll be able to access your account within approximately 24 hours");
                        return false;
                    }
                }
                else
                {
                    if (logInResponseHandler.Issue != null)
                        GlobusLogHelper.log.Info(Log.CustomMessage, dominatorAccountModel.AccountBaseModel.AccountNetwork,
                        dominatorAccountModel.UserName, "LangKeyVerification".FromResourceDictionary(), $"Could not able to login with Error Response : {logInResponseHandler.Issue.Message}");

                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();

                GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Instagram, dominatorAccountModel.UserName, "LangKeyVerification".FromResourceDictionary(),
                    "Could not able to send verificaton code.");
            }
            return true;
        }

        private async Task<string> SolveCaptchaManualInBrowser()
        {
            try
            {
                GdBrowserManager.CloseBrowser();
                var token = await GdBrowserManager.SolveCaptchaBrowserMode();
                GdBrowserManager.CloseBrowser();
                return token;
            }
            catch (Exception)
            {
                return "";
            }
        }
        public async Task<bool> VerifyAccountAsync(DominatorAccountModel dominatorAccountModel, CancellationToken token)
        {
            try
            {
                if (dominatorAccountModel.IsRunProcessThroughBrowser)
                {
                    LoginWithBrowserMethod(dominatorAccountModel, token);
                    return true;
                }
                AccountModel accountModel = new AccountModel(dominatorAccountModel);
                await InitializeRequestParameterIgAsync(dominatorAccountModel, accountModel);
                LoginIgResponseHandler logInResponseHandler = null;
                dominatorAccountModel.VarificationCode = dominatorAccountModel.VarificationCode.Trim();
                if (!IsSecurityCodeValid(dominatorAccountModel, dominatorAccountModel.VarificationCode))
                    return false;

                IRequestParameters requestParameter = _httpHelper.GetRequestParameter();
                string challengeApiPathUrl = dominatorAccountModel.ChallengeUrl;
                if (requestParameter.Headers["X-CSRFToken"] == null && requestParameter.Cookies != null)
                    requestParameter.Headers.Add("X-CSRFToken", requestParameter.Cookies["csrftoken"]?.Value);

                #region  Two Factor verification
                if (dominatorAccountModel.AccountBaseModel.Status == AccountStatus.TwoFactorLoginAttempt)
                {
                    string identifier = dominatorAccountModel.two_factor_identifier;
                    logInResponseHandler = instaFunct.TwoFactorLogin(dominatorAccountModel, accountModel, identifier);
                    string loginResponse = logInResponseHandler.ToString();
                    if (logInResponseHandler.Success)
                    {
                        try
                        {
                            if (_httpHelper.Response.Headers["ig-set-authorization"] != null)
                            {
                                var autorizedToken = _httpHelper.Response.Headers.GetValues("ig-set-authorization");
                                _httpHelper.GetRequestParameter().AddHeader("Authorization", autorizedToken[0].ToString());
                                accountModel.AuthorizationHeader = autorizedToken[0].ToString();
                            }
                            if (_httpHelper.Response.Headers["ig-set-x-mid"] != null)
                            {
                                var mid = _httpHelper.Response.Headers.GetValues("ig-set-x-mid");
                                _httpHelper.GetRequestParameter().AddHeader("X-MID", mid[0].ToString());
                                accountModel.MidHeader = mid[0].ToString();
                            }
                            if (string.IsNullOrEmpty(dominatorAccountModel.AccountBaseModel.ProfileId))
                                dominatorAccountModel.AccountBaseModel.ProfileId = logInResponseHandler.Username;
                        }
                        catch (Exception)
                        {

                        }
                        SetAccountDetails(dominatorAccountModel, accountModel, logInResponseHandler.Pk);
                        dominatorAccountModel.two_factor_identifier = string.Empty;
                        return true;
                    }
                    else if (loginResponse.Contains("This code is no longer valid. Please request a new one"))
                    {

                        ResendTwoFactorLoginCodeResponseHandler resendLoginSendCode = instaFunct.SendAgainTwoFactorLoginCode(identifier);
                        if (resendLoginSendCode.Success)
                        {
                            dominatorAccountModel.two_factor_identifier = resendLoginSendCode.two_factor_identifier;
                            GlobusLogHelper.log.Info(Log.CustomMessage,
                  dominatorAccountModel.AccountBaseModel.AccountNetwork,
                  dominatorAccountModel.AccountBaseModel.UserName, "Two Factor Login",
                  "This code is no longer valid please enter new valid code");
                            return false;
                        }
                    }
                    else if (loginResponse.Contains("sms_code_validation_code_invalid"))
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage,
                   dominatorAccountModel.AccountBaseModel.AccountNetwork,
                   dominatorAccountModel.AccountBaseModel.UserName, "Two Factor Login",
                   "Security code is invalid please try again.");
                        return false;
                    }
                    else
                        return false;
                }
                #endregion
                if (dominatorAccountModel.AccountBaseModel.Status == AccountStatus.NeedsVerification && dominatorAccountModel.AccountBaseModel.IsChkTwoFactorLogin)
                {
                    requestParameter.Referer = $"https://i.instagram.com/api/v1{challengeApiPathUrl}";
                    _httpHelper.SetRequestParameter(requestParameter);

                    logInResponseHandler = instaFunct.VerifyTwoFactorAccount(challengeApiPathUrl, dominatorAccountModel.VarificationCode).Result;
                    if (logInResponseHandler.Issue.Message.Contains("{\"action\": \"close\", \"status\": \"ok\"}"))
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage,
                   dominatorAccountModel.AccountBaseModel.AccountNetwork,
                   dominatorAccountModel.AccountBaseModel.UserName, "Login",
                   "verification successfully please login again");
                        return true;
                    }
                }

                requestParameter.Referer = $"https://i.instagram.com/api/v1{challengeApiPathUrl}";
                _httpHelper.SetRequestParameter(requestParameter);
                if (accountModel.AccountVerifyType == "DifferentAccount")
                {
                    SubmitPhoneCodeForCaptchaResponseHandler submitCodeResponse = null;
                    if (accountModel.codeSubmittingtype == "VerifySMSCodeFormForSMSCaptcha")
                    {
                        //{"challengeType": "UFACBlockingForm", "errors": [], "experiments": {}, "extraData": null, "fields": {"code_whitelisted": true}, "navigation": {"forward": "/challenge/34938605558/POU8VIp0AH/", "replay": "/challenge/replay/34938605558/POU8VIp0AH/", "dismiss": "instagram://checkpoint/dismiss"}, "privacyPolicyUrl": "/about/legal/privacy/", "type": "CHALLENGE", "challenge_context": "{\"step_name\": \"\", \"nonce_code\": \"POU8VIp0AH\", \"user_id\": 34938605558, \"security_code\": \"038947\", \"is_stateless\": false, \"challenge_type_enum\": \"UNKNOWN\"}", "status": "ok"}
                        submitCodeResponse = instaFunct.SubmitPhoneNumberRequest(dominatorAccountModel, accountModel, accountModel.challengeContext, dominatorAccountModel.VarificationCode, accountModel.apiUrl, token);
                        if (submitCodeResponse != null && submitCodeResponse.type.Contains("CHALLENGE_REDIRECTION"))
                        {
                            //await IntialLoginForCaptcha(dominatorAccountModel, accountModel, token);
                            return false;
                        }
                        if (submitCodeResponse.Issue.Error == InstagramError.AccountCompromised)
                        {
                            dominatorAccountModel.AccountBaseModel.Status = AccountStatus.AccountCompromised;
                            SetLoginStatus(dominatorAccountModel, accountModel, AccountStatus.AccountCompromised, false);
                            GlobusLogHelper.log.Info(Log.CustomMessage,
                           dominatorAccountModel.AccountBaseModel.AccountNetwork,
                           dominatorAccountModel.AccountBaseModel.UserName, "Login",
                           submitCodeResponse.Issue.Message);
                            return false;
                        }
                    }
                    else
                        logInResponseHandler = instaFunct.SubmitChallengeAsyncForDifferentAccounts(dominatorAccountModel, accountModel, dominatorAccountModel.Challenge_Context, challengeApiPathUrl, dominatorAccountModel.VarificationCode).Result;
                    _httpHelper.SetRequestParameter(requestParameter);
                }
                else if (accountModel.blockAction.Contains("com.instagram.challenge.navigation.take_challenge"))
                {
                    logInResponseHandler = instaFunct.ActionBlockSubmitChallengeAsync(dominatorAccountModel, accountModel, dominatorAccountModel.Challenge_Context, challengeApiPathUrl, dominatorAccountModel.VarificationCode).Result;
                }
                else
                    logInResponseHandler = instaFunct.SubmitChallengeAsync(dominatorAccountModel, accountModel, dominatorAccountModel.Challenge_Context, challengeApiPathUrl, dominatorAccountModel.VarificationCode).Result;

                if (_httpHelper.GetRequestParameter().Cookies.Count < 11 && logInResponseHandler.Success)
                {

                    instaFunct.GetFeedTimeLineData(dominatorAccountModel, accountModel);
                }

                if (logInResponseHandler.Success && logInResponseHandler.Username != null && logInResponseHandler.Username.ToLower() == dominatorAccountModel.UserName.ToLower().Trim())
                {
                    try { if (string.IsNullOrEmpty(dominatorAccountModel.AccountBaseModel.ProfileId)) dominatorAccountModel.AccountBaseModel.ProfileId = logInResponseHandler.Username; } catch (Exception ex) { ex.DebugLog(); }
                    SetAccountDetails(dominatorAccountModel, accountModel, logInResponseHandler.Pk);
                    await HittinAllNeededRequest(dominatorAccountModel, accountModel, CancellationToken.None);
                    return true;
                }
                else if (logInResponseHandler.Success && logInResponseHandler.ActionBlockResponse != null && logInResponseHandler.ActionBlockResponse.Contains("profile_pic_url"))
                {
                    var userId = string.Empty;
                    var res = instaFunct.Get_AccountUserid(dominatorAccountModel, accountModel, "", token);
                    var userID = Utilities.GetBetween(res.ToString(), "\"current_account\":{\"pk\":", ",\"username\"");
                    if (!string.IsNullOrEmpty(userID))
                    {
                        userId = userID;
                    }
                    else
                        userId = Regex.Split(challengeApiPathUrl, "/")[2];
                    var infoDetails = instaFunct.SearchUserInfoById(dominatorAccountModel, accountModel, userId, token).Result;
                    try
                    {
                        if (string.IsNullOrEmpty(dominatorAccountModel.AccountBaseModel.ProfileId) && infoDetails != null && !string.IsNullOrEmpty(infoDetails.Username))
                            dominatorAccountModel.AccountBaseModel.ProfileId = infoDetails.Username;

                        SetAccountDetails(dominatorAccountModel, accountModel, userId);
                        await HittinAllNeededRequest(dominatorAccountModel, accountModel, CancellationToken.None);
                        return true;
                    }
                    catch (Exception ex) { ex.DebugLog(); }
                }
                else if (logInResponseHandler.Success)
                {
                    DelayService.ThreadSleep(TimeSpan.FromSeconds(5));
                    var profileInfo = instaFunct.GetProfileDetails(dominatorAccountModel);
                    if (profileInfo.Email.ToLower() == dominatorAccountModel.UserName.ToLower())
                    {
                        try { if (string.IsNullOrEmpty(dominatorAccountModel.AccountBaseModel.ProfileId)) dominatorAccountModel.AccountBaseModel.ProfileId = logInResponseHandler.Username; } catch (Exception) { }
                        SetAccountDetails(dominatorAccountModel, accountModel, logInResponseHandler.Pk);
                        return true;
                    }
                }
                else if (logInResponseHandler.Issue != null)
                {
                    if (logInResponseHandler.Issue.Message.Contains("\"action\": \"close\","))
                    {
                        dominatorAccountModel.AccountBaseModel.Status = AccountStatus.ActionBlocked;
                        dominatorAccountModel.IsloggedinWithPhone = false;
                        dominatorAccountModel.IsUserLoggedIn = false;
                        GlobusLogHelper.log.Info(Log.CustomMessage,
                           dominatorAccountModel.AccountBaseModel.AccountNetwork,
                           dominatorAccountModel.AccountBaseModel.UserName, "Login",
                           "Your Account has been blocked please check once manually");
                    }
                    if (logInResponseHandler.Issue.Error == InstagramError.wrongSecurityCode)
                    {
                        dominatorAccountModel.AccountBaseModel.Status = AccountStatus.InvalidSecurityCode;
                        dominatorAccountModel.IsloggedinWithPhone = false;
                        dominatorAccountModel.IsUserLoggedIn = false;
                        GlobusLogHelper.log.Info(Log.CustomMessage,
                           dominatorAccountModel.AccountBaseModel.AccountNetwork,
                           dominatorAccountModel.AccountBaseModel.UserName, "Login",
                           "Invalid code please check the code we sent you and try again.");
                    }
                }
                else
                {
                    dominatorAccountModel.IsloggedinWithPhone = false;
                    dominatorAccountModel.IsUserLoggedIn = false;
                }

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return false;
        }

        public bool AutoVerifyViaEmail(DominatorAccountModel dominatorAccountModel, AccountModel accountModel, CancellationToken token, LoginIgResponseHandler logInResponseHandler)
        {
            if (dominatorAccountModel.IsAutoVerifyByEmail && !string.IsNullOrEmpty(dominatorAccountModel.MailCredentials.Username) &&
                !string.IsNullOrEmpty(dominatorAccountModel.MailCredentials.Password) && !string.IsNullOrEmpty(dominatorAccountModel.MailCredentials.Hostname) && dominatorAccountModel.MailCredentials.Port != null)
            {
                try
                {

                    int verificationAttempt = 2;
                    while (verificationAttempt != 0)
                    {
                        if (SendSecurityCodeAsync(dominatorAccountModel, token, VerificationType.Email, true).Result)
                            ReadVerificationCodeFromEmail(dominatorAccountModel);
                        if (VerifyAccountAsync(dominatorAccountModel, token).Result)
                        {
                            try { if (string.IsNullOrEmpty(dominatorAccountModel.AccountBaseModel.ProfileId)) dominatorAccountModel.AccountBaseModel.ProfileId = logInResponseHandler.Username; } catch (Exception) { }
                            SetAccountDetails(dominatorAccountModel, accountModel, logInResponseHandler.Pk);
                            return true;
                        }
                        verificationAttempt--;
                    }
                }
                catch (Exception ex)
                {
                    if (ex.Source == "OpenPop")
                        GlobusLogHelper.log.Info(Log.CustomMessage,
                                        dominatorAccountModel.AccountBaseModel.AccountNetwork, dominatorAccountModel.UserName, "Auto Verification",
                                        $"Email Verification Failed : {ex.Message.ToString()}");
                    ex.DebugLog();
                }
            }

            return false;
        }

        public static void ReadVerificationCodeFromEmail(DominatorAccountModel accountModel)
        {
            GlobusLogHelper.log.Info(Log.CustomMessage,
                accountModel.AccountBaseModel.AccountNetwork,
                accountModel.AccountBaseModel.UserName, "Auto Verification",
                "Trying to fetch verification code from associated email address.");
            Thread.Sleep(TimeSpan.FromMinutes(2));
            MailCredentials model =
                new MailCredentials
                {
                    Hostname = accountModel.MailCredentials.Hostname,
                    Port = accountModel.MailCredentials.Port,
                    Username = accountModel.MailCredentials.Username,
                    Password = accountModel.MailCredentials.Password
                };

            var messageData = EmailClient.FetchLastMailFromSender(model, true, "security@mail.instagram.com");

            accountModel.VarificationCode = Utilities.GetBetween(messageData.Message, "<font size=\"6\">", "</font>");
        }

        public static bool IsSecurityCodeValid(DominatorAccountModel dominatorAccountModel, string securityCode)
        {
            if (!Regex.IsMatch(securityCode, "^[0-9]+$") || String.IsNullOrEmpty(securityCode) || securityCode.Length != 6)
            {
                if (String.IsNullOrEmpty(securityCode))
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                        dominatorAccountModel.AccountBaseModel.AccountNetwork, dominatorAccountModel.UserName, "Login",
                        "You didn't enter any security code. To verify account click \"Check Account Status\" option again.");
                }
                else if (securityCode.Length != 6 || !Regex.IsMatch(securityCode, "^[0-9]+$"))
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                        dominatorAccountModel.AccountBaseModel.AccountNetwork, dominatorAccountModel.UserName, "Login",
                        "You have entered a wrong security code. To verify again please click \"Check Account Status\" option.");
                }
                dominatorAccountModel.IsloggedinWithPhone = false;
                dominatorAccountModel.IsUserLoggedIn = false;
                return false;
            }
            return true;
        }

        public static void SetLoginStatus(DominatorAccountModel dominatorAccountModel, AccountModel accountModel, AccountStatus accountStatus, bool isEditIntoBinFile)
        {

            if (accountStatus == AccountStatus.Success)
            {
                dominatorAccountModel.IsUserLoggedIn = true;
                dominatorAccountModel.IsloggedinWithPhone = true;
            }
            else
            {
                dominatorAccountModel.IsUserLoggedIn = false;
                dominatorAccountModel.IsloggedinWithPhone = false;
            }

            dominatorAccountModel.AccountBaseModel.Status = accountStatus;
            accountModel.AccountStatus = accountStatus.ToString();
            if (isEditIntoBinFile)
            {
                dominatorAccountModel.Token.ThrowIfCancellationRequested();
                SocinatorAccountBuilder.Instance(dominatorAccountModel.AccountBaseModel.AccountId)
                     .AddOrUpdateDominatorAccountBase(dominatorAccountModel.AccountBaseModel)
                     .SaveToBinFile();

                UpdateGlobalAccountDetails(dominatorAccountModel, accountModel);
                SocinatorAccountBuilder.Instance(dominatorAccountModel.AccountBaseModel.AccountId)
                    .AddOrUpdateCookies(dominatorAccountModel.Cookies)
                    .SaveToBinFile();
            }
        }

        private void SetAccountDetails(DominatorAccountModel dominatorAccountModel, AccountModel accountModel, string userId = null, bool isBlock = false)
        {
            try
            {
                dominatorAccountModel.Cookies = _httpHelper.GetRequestParameter().Cookies;
                SetLoginStatus(dominatorAccountModel, accountModel, AccountStatus.Success, true);
                IRequestParameters requestParameters = _httpHelper.GetRequestParameter();
                accountModel.CsrfToken = requestParameters?.Cookies["csrftoken"]?.Value ?? String.Empty;
                dominatorAccountModel.AccountBaseModel.UserId = userId;
                dominatorAccountModel.AccountBaseModel.UserId = userId ?? accountModel.DsUserId;
                dominatorAccountModel.Token.ThrowIfCancellationRequested();
                SocinatorAccountBuilder.Instance(dominatorAccountModel.AccountBaseModel.AccountId)
                    .AddOrUpdateDominatorAccountBase(dominatorAccountModel.AccountBaseModel)
                    .AddOrUpdateMobileAgentWeb(dominatorAccountModel.UserAgentMobile)
                    .AddOrUpdateCookies(dominatorAccountModel.Cookies)
                    .SaveToBinFile();

                UpdateGlobalAccountDetails(dominatorAccountModel, accountModel);

                if (!isBlock)
                    GlobusLogHelper.log.Info(Log.SuccessfulLogin, dominatorAccountModel.AccountBaseModel.AccountNetwork,
                        dominatorAccountModel.AccountBaseModel.UserName);
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (AggregateException ae)
            {
                ae.HandleOperationCancellation();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private static void UpdateGlobalAccountDetails(DominatorAccountModel dominatorAccountModel, AccountModel accountModel)
        {
            var globalDbOperation = new DbOperations(SocinatorInitialize.GetGlobalDatabase().GetSqlConnection());
            globalDbOperation.UpdateAccountDetails(dominatorAccountModel);
            globalDbOperation.UpdateAccountBackupDetails(dominatorAccountModel, accountModel.Device_Id, dominatorAccountModel.UserAgentMobile);
        }

        public async Task CheckisLoggedinMobileAsync(DominatorAccountModel dominatorAccountModel, AccountModel AccountModel)
        {
            try
            {
                IRequestParameters requestParameters = _httpHelper.GetRequestParameter();
                int ResponseCount = 0;
                var resp = instaFunct.GetFeedTimeLineData(dominatorAccountModel, AccountModel);
                if (string.IsNullOrEmpty(AccountModel.MidHeader))
                {
                    AccountModel.MidHeader = _httpHelper.Response.Headers["ig-set-x-mid"];
                }
                if (string.IsNullOrEmpty(AccountModel.DsUserId))
                {
                    AccountModel.DsUserId = _httpHelper.Response.Headers["ig-set-ig-u-ds-user-id"];
                }
                if (resp == null)
                    return;

                while (resp.ToString().Contains("The underlying connection was closed"))
                {
                    DelayService.ThreadSleep(TimeSpan.FromSeconds(5));
                    resp = instaFunct.GetFeedTimeLineData(dominatorAccountModel, AccountModel);
                    ResponseCount++;
                    if (ResponseCount == 1)
                    {
                        dominatorAccountModel.IsUserLoggedIn = false;
                        dominatorAccountModel.IsloggedinWithPhone = false;
                        GlobusLogHelper.log.Info("Unable to get Response From Account =>" +
                                                 dominatorAccountModel.UserName);
                        return;
                    }
                }

                if (resp.ToString().Contains(dominatorAccountModel.AccountBaseModel.UserName) ||
                    resp.ToString().Contains("profile_pic_url"))
                {
                    await Task.Delay(20);
                    //if (!string.IsNullOrEmpty(AccountModel.AuthorizationHeader))
                    //    await instaFunct.GetBlockedMedia(dominatorAccountModel, AccountModel);
                    dominatorAccountModel.IsloggedinWithPhone = true;
                    dominatorAccountModel.IsUserLoggedIn = true;
                    SetAccountDetails(dominatorAccountModel, new AccountModel(dominatorAccountModel));
                }
                else if(resp.Issue!=null && resp.Issue.Error==InstagramError.Challenge)
                {
                    dominatorAccountModel.IsUserLoggedIn = false;
                    dominatorAccountModel.IsloggedinWithPhone = false;
                    dominatorAccountModel.AccountBaseModel.Status = AccountStatus.NeedsVerification;
                    SetAccountDetails(dominatorAccountModel, new AccountModel(dominatorAccountModel));
                }
                else
                {
                    dominatorAccountModel.IsUserLoggedIn = false;
                    dominatorAccountModel.IsloggedinWithPhone = false;
                    dominatorAccountModel.AccountBaseModel.Status = AccountStatus.Failed;

                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public async Task InitializeRequestParameterIgAsync(DominatorAccountModel accountModel, AccountModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.WaterfallId))
                    model.WaterfallId = Utilities.GetGuid();
                IgRequestParameters requestParameter = new IgRequestParameters(accountModel.UserAgentMobile);
                requestParameter.Headers["X-Pigeon-Session-Id"] = (model.PigeonSessionId.StartsWith("UFS")) ? model.PigeonSessionId : "UFS-" + model.PigeonSessionId + "-0";
                requestParameter.Headers["X-IG-Device-ID"] = model.Id;
                requestParameter.Headers["X-IG-Android-ID"] = model.Device_Id;
                requestParameter.Proxy = accountModel.AccountBaseModel.AccountProxy;
                _httpHelper.SetRequestParameter(requestParameter);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public async Task GetUserInfoAndSetIntoUiWithoutLoginAsync(DominatorAccountModel dominatorAccountModel, AccountModel accountModel)
        {
            try
            {
                accountModel.DsUserId = dominatorAccountModel.AccountBaseModel.UserId;
                if (string.IsNullOrEmpty(accountModel.DsUserId))
                {
                    var userid = _httpHelper.Response.Headers.GetValues("ig-set-ig-u-ds-user-id");
                    accountModel.DsUserId = userid[0].ToString();
                }
                if (string.IsNullOrEmpty(dominatorAccountModel.AccountBaseModel.UserId))
                    return;
                IRequestParameters requestParameters = _httpHelper.GetRequestParameter();
                requestParameters.Headers.Remove("Accept-Encoding");
                var userInfoResponse = instaFunct.SearchUserInfoById(dominatorAccountModel, accountModel,
                    dominatorAccountModel.AccountBaseModel.UserId, CancellationToken.None).Result;
                if (userInfoResponse != null && userInfoResponse.Issue != null)
                {
                    if (userInfoResponse.Issue.Message.Contains("Unable to resolve host name."))
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage,
                            dominatorAccountModel.AccountBaseModel.AccountNetwork,
                            dominatorAccountModel.AccountBaseModel.UserName, "Login",
                            "Please check your internet connection");
                        return;
                    }
                    if (userInfoResponse.Issue.Message.Contains("Unable to resolve host name."))
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage,
                            dominatorAccountModel.AccountBaseModel.AccountNetwork,
                            dominatorAccountModel.AccountBaseModel.UserName, "Login",
                            "Please check your internet connection");
                        return;
                    }
                    if (userInfoResponse != null && userInfoResponse.Issue.ApiPath.Contains("/challenge/"))
                    {
                        accountModel.AccountStatus = AccountStatus.NeedsVerification.ToString();
                        DelayService.ThreadSleep(TimeSpan.FromSeconds(2));
                    }
                }

                if (!userInfoResponse.Success)
                    return;
                else
                {

                    if (!dominatorAccountModel.UserName.Equals(userInfoResponse.Username) && !dominatorAccountModel.UserName.Contains("@"))
                        dominatorAccountModel.AccountBaseModel.UserName = userInfoResponse.Username;
                    else
                        dominatorAccountModel.AccountBaseModel.UserName = userInfoResponse.Username;

                    dominatorAccountModel.DisplayColumnValue1 = userInfoResponse.FollowerCount;
                    dominatorAccountModel.DisplayColumnValue2 = userInfoResponse.FollowingCount;
                    dominatorAccountModel.DisplayColumnValue3 = userInfoResponse.MediaCount;
                    accountModel.AccountStatus = AccountStatus.Success.ToString();
                }
                SocinatorAccountBuilder.Instance(dominatorAccountModel.AccountBaseModel.AccountId)
                    .AddOrUpdateDisplayColumn1(dominatorAccountModel.DisplayColumnValue1)
                    .AddOrUpdateDisplayColumn2(dominatorAccountModel.DisplayColumnValue2)
                    .AddOrUpdateDisplayColumn3(dominatorAccountModel.DisplayColumnValue3)
                    .SaveToBinFile();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public async Task checkingProxy(DominatorAccountModel dominatorAccountModel, AccountModel accountModel)
        {
            try
            {
                IgRequestParameters requestParameter = new IgRequestParameters(dominatorAccountModel.UserAgentMobile);
                dominatorAccountModel.AccountBaseModel.AccountProxy.ProxyIp = dominatorAccountModel.AccountBaseModel.AccountProxy.ProxyIp.Trim().Trim(')').Trim('(');
                requestParameter.Proxy = dominatorAccountModel.AccountBaseModel.AccountProxy;
                CheckProxyhttpHelper.SetRequestParameter(requestParameter);
                string checkUrl = "https://i.instagram.com/api/v1/";//"https://i.instagram.com/api/v1/";  //https://instagram.com
                IResponseParameter timelineFeedResponse =
                   await CheckProxyhttpHelper.GetRequestAsync(checkUrl, dominatorAccountModel.Token);
                if ((timelineFeedResponse != null && timelineFeedResponse.Exception != null) &&
                    (timelineFeedResponse.Exception.Message.Contains("Unable to connect to the remote server") ||
                    timelineFeedResponse.Exception.Message.Contains("The remote name could not be resolved") ||
                    timelineFeedResponse.Response.ToString().Contains("The requested URL could not be retrieved") ||
                    timelineFeedResponse.Response.ToString().Contains("WebSite Blocked / Access to Site Restricted") ||
                    timelineFeedResponse.Response.ToString().Contains("Please ensure this IP is set in your Authorized IP list")))
                {
                    dominatorAccountModel.IsUserLoggedIn = false;
                    dominatorAccountModel.AccountBaseModel.Status = AccountStatus.ProxyNotWorking;
                    SocinatorAccountBuilder.Instance(dominatorAccountModel.AccountBaseModel.AccountId)
                        .AddOrUpdateDominatorAccountBase(dominatorAccountModel.AccountBaseModel)
                        .SaveToBinFile();
                    return;

                }
                if ((timelineFeedResponse != null && timelineFeedResponse.Exception != null) &&
                    (timelineFeedResponse.Exception.Message.Contains("your IP is not authorized to access this proxy")) || timelineFeedResponse.Response.ToString().Contains("Please ensure this IP is set in your Authorized IP list"))
                {
                    dominatorAccountModel.IsUserLoggedIn = false;
                    dominatorAccountModel.AccountBaseModel.Status = AccountStatus.ProxyNotWorking;
                    SocinatorAccountBuilder.Instance(dominatorAccountModel.AccountBaseModel.AccountId)
                        .AddOrUpdateDominatorAccountBase(dominatorAccountModel.AccountBaseModel)
                        .SaveToBinFile();
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                            dominatorAccountModel.AccountBaseModel.AccountNetwork,
                            dominatorAccountModel.AccountBaseModel.UserName, "Login",
                            "Authentication failure, your IP is not authorized to access this proxy.");
                    return;
                }
                else
                {
                    if (dominatorAccountModel.AccountBaseModel.Status == AccountStatus.ProxyNotWorking)
                    {
                        dominatorAccountModel.IsUserLoggedIn = false;
                        dominatorAccountModel.AccountBaseModel.Status = AccountStatus.TryingToLogin;
                    }
                }
            }
            catch (Exception)
            {
                //ex.DebugLog();
            }
        }
        //GetCookiesBeforeLogin
        public async Task GetHeadersBeforeLogin(DominatorAccountModel dominatorAccountModel, AccountModel accountModel)
        {
            try
            {
                await instaFunct.B_ZrTokenAsyncBeforeLogin(dominatorAccountModel, accountModel);
                var syncLaunchResponseHandler = await instaFunct.B_SyncDeviceFeaturesAsyncBeforeLogin(dominatorAccountModel, accountModel);
                //await instaFunct.GetPrefillCandidatesBeforeLogin(dominatorAccountModel, accountModel);
                //await instaFunct.LoginAttributionAsync(dominatorAccountModel, accountModel);
                await instaFunct.LauncherSyncAsyncBeforeLogin(dominatorAccountModel, accountModel);

                //await instaFunct.SyncDeviceFeaturesAsyncBeforeLogin(dominatorAccountModel, accountModel);
                if (syncLaunchResponseHandler != null && syncLaunchResponseHandler.Experiments.Count == 10)
                {
                    accountModel.UsernameTextId = syncLaunchResponseHandler.Experiments["username_id"];
                    accountModel.PasswordTextId = syncLaunchResponseHandler.Experiments["password_id"];
                    accountModel.InstanceId = syncLaunchResponseHandler.Experiments["instance_id"];
                    accountModel.MarkerId = syncLaunchResponseHandler.Experiments["marker_id"];
                    accountModel.ScreenId = syncLaunchResponseHandler.Experiments["screen_id"];
                    accountModel.ComponentId = syncLaunchResponseHandler.Experiments["component_id"];
                    accountModel.TextInputId = syncLaunchResponseHandler.Experiments["text_input_id"];
                    accountModel.TypeAheadId = syncLaunchResponseHandler.Experiments["type_ahead_id"];
                    accountModel.Fdid = syncLaunchResponseHandler.Experiments["fdid"];
                    accountModel.TextInstanceId = syncLaunchResponseHandler.Experiments["text_instance_id"];
                }
                accountModel.publicId = _httpHelper.Response.Headers["ig-set-password-encryption-key-id"];
                accountModel.publicKey = _httpHelper.Response.Headers["ig-set-password-encryption-pub-key"];
            }
            catch (Exception)
            {
                if (_httpHelper.Response.StatusCode == HttpStatusCode.Forbidden)
                    dominatorAccountModel.AccountBaseModel.Status = AccountStatus.ProxyNotWorking;
            }
        }


        public async Task<bool> TwoFactorLoginProcess(DominatorAccountModel dominatorAccountModel, AccountModel accountModel, IRequestParameters requestParameter, CancellationToken token)
        {
            LoginIgResponseHandler logInResponseHandler = await instaFunct.LoginAsyncForTwoFactor(dominatorAccountModel, accountModel);

            string loginResponse = logInResponseHandler.ToString();
            accountModel.AuthorizationHeader = _httpHelper.Response.Headers["ig-set-authorization"];
            if (logInResponseHandler.Success)
            {
                try { if (string.IsNullOrEmpty(dominatorAccountModel.AccountBaseModel.ProfileId)) dominatorAccountModel.AccountBaseModel.ProfileId = logInResponseHandler.Username; } catch (Exception) { }
                SetAccountDetails(dominatorAccountModel, accountModel, logInResponseHandler.Pk);
                return true;
            }
            else if (logInResponseHandler.Issue?.Error == InstagramError.Challenge)
            {
                IResponseParameter response = await GetChallengeResponse(dominatorAccountModel, accountModel, requestParameter, logInResponseHandler, null, token);
                if (response.Response.Contains("select_verify_method") && response.Response.Contains("\"status\": \"ok\""))
                {
                    dominatorAccountModel.AccountBaseModel.Status = AccountStatus.NeedsVerification;
                    return true;
                }
                else if (response.Response.Contains("\"status\": \"ok\"") && response.Response.Contains("close"))
                {
                    logInResponseHandler = await instaFunct.LoginAsyncForTwoFactor(dominatorAccountModel, accountModel);
                    loginResponse = logInResponseHandler.ToString();
                    if (logInResponseHandler.Success)
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage,
                    dominatorAccountModel.AccountBaseModel.AccountNetwork,
                    dominatorAccountModel.AccountBaseModel.UserName, "Two Factor Login",
                    "Two factor authentication please enter code which has been sent to your associate phone no");
                        JToken jToken = JObject.Parse(loginResponse);
                        string choice = jToken["two_factor_info"]["two_factor_identifier"].ToString();
                        dominatorAccountModel.two_factor_identifier = choice;
                        dominatorAccountModel.AccountBaseModel.Status = AccountStatus.TwoFactorLoginAttempt;

                        SocinatorAccountBuilder.Instance(dominatorAccountModel.AccountBaseModel.AccountId)
                            .AddOrUpdateDominatorAccountBase(dominatorAccountModel.AccountBaseModel).AddOrUpdateCookies(dominatorAccountModel.Cookies)
                            .SaveToBinFile();
                        return true;
                    }
                }
            }
            else if (loginResponse.Contains("two_factor_required"))
            {
                TwofactorRequired(dominatorAccountModel, loginResponse);
                return true;
            }
            #region Integrity Solve Section
            else if (loginResponse.Contains("https://i.instagram.com/integrity"))
            {
                await AccountItWasMe(dominatorAccountModel, token);
            }
            else if (loginResponse.ToLower().Contains("the username you entered doesn't appear to belong to an account"))
            {
                WrongUserName(dominatorAccountModel, accountModel, loginResponse);
                return false;
            }
            else if (loginResponse.ToLower().Contains("the password you entered is incorrect"))
            {
                WrongPassword(dominatorAccountModel, accountModel, loginResponse);
                return false;
            }
            #endregion
            return false;
        }

        public async Task<bool> SendSecurityCodeTwoFactorLoginProcess(DominatorAccountModel dominatorAccountModel, AccountModel accountModel, IRequestParameters requestParameter, VerificationType verificationType, CancellationToken token)
        {
            LoginIgResponseHandler logInResponseHandler = await instaFunct.LoginAsyncForTwoFactor(dominatorAccountModel, accountModel);
            string loginResponse = logInResponseHandler.ToString();
            if (logInResponseHandler.Success)
            {
                try { if (string.IsNullOrEmpty(dominatorAccountModel.AccountBaseModel.ProfileId)) dominatorAccountModel.AccountBaseModel.ProfileId = logInResponseHandler.Username; } catch (Exception) { }
                SetAccountDetails(dominatorAccountModel, accountModel, logInResponseHandler.Pk);
                return true;
            }
            else if (logInResponseHandler.Issue?.Error == InstagramError.Challenge)
            {
                DelayService.ThreadSleep(TimeSpan.FromSeconds(1));
                GlobusLogHelper.log.Info(Log.AccountNeedsVerification, dominatorAccountModel.AccountBaseModel.AccountNetwork, dominatorAccountModel.AccountBaseModel.UserName, "Two Factor Login");

                dominatorAccountModel.IsloggedinWithPhone = false;
                SocinatorAccountBuilder.Instance(dominatorAccountModel.AccountBaseModel.AccountId)
                  .AddOrUpdateCookies(_httpHelper.GetRequestParameter().Cookies)
                  .SaveToBinFile();

                string challengeApiPathUrl = Utilities.GetBetween(loginResponse, "checkpoint_url\": \"", "\"");
                if (String.IsNullOrEmpty(challengeApiPathUrl))
                    challengeApiPathUrl = Utilities.GetBetween(loginResponse, "api_path\": \"", "\"");

                if (requestParameter.Headers["X-CSRFToken"] == null)
                    requestParameter.Headers.Add("X-CSRFToken", requestParameter.Cookies["csrftoken"]?.Value);
                else
                    requestParameter.Headers["X-CSRFToken"] = requestParameter.Cookies["csrftoken"]?.Value;

                dominatorAccountModel.ChallengeUrl = challengeApiPathUrl;
                requestParameter.Referer = $"https://i.instagram.com/api/v1{challengeApiPathUrl}";
                _httpHelper.SetRequestParameter(requestParameter);
                string challengeUrl = $"https://i.instagram.com/api/v1{challengeApiPathUrl}?guid={accountModel.Guid}&device_id={dominatorAccountModel.DeviceDetails.DeviceId}";
                requestParameter.Headers.Remove("Accept-Encoding");
                IResponseParameter response = await _httpHelper.GetRequestAsync(challengeUrl, dominatorAccountModel.Token);
                if (response.Response.Contains("verify_code") && response.Response.Contains("\"status\": \"ok\""))
                {
                    GlobusLogHelper.log.Info(Log.SentVerificationCode, dominatorAccountModel.AccountBaseModel.AccountNetwork,
                                   dominatorAccountModel.UserName, verificationType);
                    return true;
                }
                if (response.Response.Contains("select_verify_method") && response.Response.Contains("\"status\": \"ok\""))
                {
                    CommonIgResponseHandler sendSecurityCodeResponse = await instaFunct.SendSecurityCodeAsyncTwoFactorLogin(dominatorAccountModel, accountModel, challengeApiPathUrl, "0");
                    if (sendSecurityCodeResponse.Success)
                    {
                        GlobusLogHelper.log.Info(Log.SentVerificationCode, dominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    dominatorAccountModel.UserName, verificationType);
                        return true;
                    }
                    else
                        GlobusLogHelper.log.Info(Log.FailedToSendVerificationCodeFaild, dominatorAccountModel.AccountBaseModel.AccountNetwork, dominatorAccountModel.UserName, verificationType);

                    return true;
                }
                else if (response.Response.Contains("\"status\": \"ok\"") && response.Response.Contains("close"))
                {
                    logInResponseHandler = await instaFunct.LoginAsyncForTwoFactor(dominatorAccountModel, accountModel);
                    loginResponse = logInResponseHandler.ToString();
                    if (logInResponseHandler.Success)
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage,
                    dominatorAccountModel.AccountBaseModel.AccountNetwork,
                    dominatorAccountModel.AccountBaseModel.UserName, "Two Factor Login",
                    "Two factor authentication please enter code which has been sent to your associate phone no");
                        JToken jToken = JObject.Parse(loginResponse);
                        string choice = jToken["two_factor_info"]["two_factor_identifier"].ToString();
                        dominatorAccountModel.two_factor_identifier = choice;
                        dominatorAccountModel.AccountBaseModel.Status = AccountStatus.TwoFactorLoginAttempt;

                        SocinatorAccountBuilder.Instance(dominatorAccountModel.AccountBaseModel.AccountId)
                            .AddOrUpdateDominatorAccountBase(dominatorAccountModel.AccountBaseModel).AddOrUpdateCookies(dominatorAccountModel.Cookies)
                            .SaveToBinFile();
                        return true;
                    }
                }
            }
            else if (loginResponse.Contains("two_factor_required"))
            {
                GlobusLogHelper.log.Info(Log.CustomMessage,
                    dominatorAccountModel.AccountBaseModel.AccountNetwork,
                    dominatorAccountModel.AccountBaseModel.UserName, "Two Factor Login",
                    "Two factor authentication please enter code which has been sent to your associate phone no");
                JToken jToken = JObject.Parse(loginResponse);
                string choice = jToken["two_factor_info"]["two_factor_identifier"].ToString();
                dominatorAccountModel.two_factor_identifier = choice;
                dominatorAccountModel.AccountBaseModel.Status = AccountStatus.TwoFactorLoginAttempt;

                SocinatorAccountBuilder.Instance(dominatorAccountModel.AccountBaseModel.AccountId)
                    .AddOrUpdateDominatorAccountBase(dominatorAccountModel.AccountBaseModel).AddOrUpdateCookies(dominatorAccountModel.Cookies)
                    .SaveToBinFile();
                return true;
            }

            else if (loginResponse.Contains("https://i.instagram.com/integrity"))
            {
                GlobusLogHelper.log.Info(Log.CustomMessage,
                    dominatorAccountModel.AccountBaseModel.AccountNetwork,
                    dominatorAccountModel.AccountBaseModel.UserName, "Login",
                    "Auto verifying, Instagram's -this was me-challenge");

                if (!dominatorAccountModel.IsUserLoggedIn)
                {
                    GlobusLogHelper.log.Info(Log.LoginFailed,
                        dominatorAccountModel.AccountBaseModel.AccountNetwork,
                        dominatorAccountModel.AccountBaseModel.UserName, "Login");
                    LoginWithDataBaseCookies(dominatorAccountModel, true, token);
                }

                if (dominatorAccountModel.IsUserLoggedIn)
                {
                    string responseForChallange =
                        (await _httpHelper.GetRequestAsync("https://www.instagram.com", dominatorAccountModel.Token))
                        .Response;
                    if (responseForChallange.Contains("https://www.instagram.com/challenge/"))
                    {
                        try
                        {
                            IRequestParameters requestParameterNew = _httpHelper.GetRequestParameter();
                            if (requestParameterNew.Headers["X-CSRFToken"] == null)
                                requestParameterNew.Headers.Add("X-CSRFToken", requestParameterNew.Cookies["csrftoken"]?.Value);
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }

                        string reponseAfterChallange = (await _httpHelper.PostRequestAsync(
                            "https://www.instagram.com/challenge/", "choice=0", dominatorAccountModel.Token)).Response;

                        if (reponseAfterChallange.Contains("CHALLENGE_REDIRECTION") &&
                            reponseAfterChallange.Contains("\"status\": \"ok\""))
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage,
                                dominatorAccountModel.AccountBaseModel.AccountNetwork,
                                dominatorAccountModel.AccountBaseModel.UserName, "Login",
                                "Instagram's -this was me-challenge has successfully completed. Now login in again...");
                        }
                        else
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage,
                                dominatorAccountModel.AccountBaseModel.AccountNetwork,
                                dominatorAccountModel.AccountBaseModel.UserName, "Login",
                                "Instagram's -this was me-challenge has failed. Please try again...");
                        }
                    }
                }
                else
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                        dominatorAccountModel.AccountBaseModel.AccountNetwork,
                        dominatorAccountModel.AccountBaseModel.UserName, "Login", "Instagram login failed.");
                }
            }
            return false;
        }

        public IGdHttpHelper GetHttpHelper() => _httpHelper;

        public async Task<IResponseParameter> GetChallengeResponse(DominatorAccountModel dominatorAccountModel, AccountModel accountModel, IRequestParameters requestParameter, LoginIgResponseHandler logInResponseHandler, CommonIgResponseHandler responseHandler, CancellationToken token)
        {
            DelayService.ThreadSleep(TimeSpan.FromSeconds(1));
            GlobusLogHelper.log.Info(Log.AccountNeedsVerification, dominatorAccountModel.AccountBaseModel.AccountNetwork, dominatorAccountModel.AccountBaseModel.UserName, "Login");

            dominatorAccountModel.IsloggedinWithPhone = false;
            SocinatorAccountBuilder.Instance(dominatorAccountModel.AccountBaseModel.AccountId)
              .AddOrUpdateCookies(_httpHelper.GetRequestParameter().Cookies)
              .SaveToBinFile();
            string challengeApiPathUrl = string.Empty;

            if (logInResponseHandler != null)
                challengeApiPathUrl = logInResponseHandler.Issue.ApiPath;
            else
                challengeApiPathUrl = responseHandler.Issue.ApiPath;
            //var randomString = generator.RandomString(10);
            //var obj = Guid.NewGuid();
            dominatorAccountModel.ChallengeUrl = challengeApiPathUrl;

            JObject jObj = JObject.Parse(logInResponseHandler.ChallengeResponse);
            var context_challenges = jObj["error"]["error_data"]["challenge_context"].ToString();
            requestParameter.Referer = $"https://i.instagram.com/api/v1{challengeApiPathUrl}";
            requestParameter.Headers.Remove("X-Ads-Opt-Out");
            _httpHelper.GetRequestParameter().ContentType = "";
            //requestParameter.Headers.Remove("Authorization");
            //requestParameter.Headers.Remove("X-CSRFToken");
            //requestParameter.Headers.Remove("X-MID");               
            //var mid = requestParameter.Headers["X-MID"].Trim(',');
            //requestParameter.Headers["X-MID"] = mid;
            requestParameter.Headers.Add("X-IG-App-Startup-Country", "IN");
            requestParameter.Headers.Add("X-IG-WWW-Claim", "0");
            _httpHelper.SetRequestParameter(requestParameter);
            string challengeUrl = $"https://i.instagram.com/api/v1{challengeApiPathUrl}?guid={accountModel.Guid}&device_id={accountModel.Device_Id}&challenge_context={context_challenges}";
            requestParameter.Headers.Remove("Accept-Encoding");
            IResponseParameter response = await _httpHelper.GetRequestAsync(challengeUrl, token);
            return response;
        }


        public async Task<IResponseParameter> GetCodeSendingType(DominatorAccountModel dominatorAccountModel, AccountModel accountModel, IRequestParameters requestParameter, LoginIgResponseHandler logInResponseHandler, IResponseParameter responseParameter, CancellationToken token)
        {
            //{"config":{"csrf_token":"dQpA2GKohqzyBDsZYtFgS2oa1S0YhDOC","viewer":null,"viewerId":null},"country_code":"unknown","language_code":"en","locale":"en_US","entry_data":{"Challenge":[{"challengeType":"RecaptchaChallengeForm","errors":[],"experiments":{},"extraData":null,"fields":{"g-recaptcha-response":"None","disable_num_days_remaining":-56,"sitekey":"6LebnxwUAAAAAGm3yH06pfqQtcMH0AYDwlsXnh-u","code_whitelisted":true},"navigation":{"forward":"/challenge/40026793751/GNxOtZcXmv/","replay":"/challenge/replay/40026793751/GNxOtZcXmv/","dismiss":"instagram://checkpoint/dismiss"},"privacyPolicyUrl":"/about/legal/privacy/","type":"CHALLENGE","challenge_context":"{\"step_name\": \"\", \"nonce_code\": \"GNxOtZcXmv\", \"user_id\": 40026793751, \"is_stateless\": false}"}]}
            string challengeApiPathUrl = string.Empty;
            dominatorAccountModel.UserAgentMobileWeb = $"Mozilla / 5.0(Linux; Android 10; RMX2027 Build/ QP1A.190711.020; wv) AppleWebKit / 537.36(KHTML, like Gecko) Version / 4.0 Chrome / 83.0.4103.106 Mobile Safari/ 537.36 {dominatorAccountModel.UserAgentMobile}";
            IgRequestParameters requestParameter1 = new IgRequestParameters(dominatorAccountModel.UserAgentMobileWeb);
            if (logInResponseHandler != null)
                challengeApiPathUrl = logInResponseHandler.Issue.ApiPath;

            dominatorAccountModel.ChallengeUrl = challengeApiPathUrl;
            //requestParameter1.Cookies = requestParameter.Cookies;
            string url = $"https://i.instagram.com{challengeApiPathUrl}";
            _httpHelper.SetRequestParameter(requestParameter1);
            IResponseParameter response = await _httpHelper.GetRequestAsync(url, token);
            return response;
        }
        public bool AccountChallenge(DominatorAccountModel dominatorAccountModel, IResponseParameter response)
        {
            if (response.Response.Contains("{\"action\": \"close\", \"status\": \"ok\"}"))
            {
                AccountActionClose(dominatorAccountModel);
                return false;
            }

            if (response.Response.Contains("submit_phone"))
            {
                try
                {
                    string responseTypeName = JObject.Parse(response.Response)["step_name"].ToString();
                    if (responseTypeName == "submit_phone")
                    {
                        SubmitPhoneNo(dominatorAccountModel);
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }
            return true;
        }

        public void AccountActionClose(DominatorAccountModel dominatorAccountModel)
        {
            dominatorAccountModel.AccountBaseModel.Status = AccountStatus.ActionBlocked;
            GlobusLogHelper.log.Info(Log.CustomMessage,
                dominatorAccountModel.AccountBaseModel.AccountNetwork,
                dominatorAccountModel.AccountBaseModel.UserName, "Login",
                "Your Account has been blocked please check once manually");
            dominatorAccountModel.IsloggedinWithPhone = false;
            // dominatorAccountModel.AccountBaseModel.AccountGroup.Content = "ActionBlocked";
            SocinatorAccountBuilder.Instance(dominatorAccountModel.AccountBaseModel.AccountId)
                .AddOrUpdateDominatorAccountBase(dominatorAccountModel.AccountBaseModel)
                .SaveToBinFile();
        }

        public void SubmitPhoneNo(DominatorAccountModel dominatorAccountModel)
        {
            DelayService.ThreadSleep(TimeSpan.FromSeconds(1));
            GlobusLogHelper.log.Info(Log.CustomMessage,
                dominatorAccountModel.AccountBaseModel.AccountNetwork,
                dominatorAccountModel.AccountBaseModel.UserName, "Login",
                "Add phone number to your account and verify again.");
            //dominatorAccountModel.AccountBaseModel.AccountGroup.Content = "AddPhoneNumberToYourAccount";
            dominatorAccountModel.AccountBaseModel.Status = AccountStatus.AddPhoneNumberToYourAccount;
            dominatorAccountModel.IsloggedinWithPhone = false;
            SocinatorAccountBuilder.Instance(dominatorAccountModel.AccountBaseModel.AccountId)
                .AddOrUpdateDominatorAccountBase(dominatorAccountModel.AccountBaseModel)
                .SaveToBinFile();
        }

        public void TwofactorRequired(DominatorAccountModel dominatorAccountModel, string loginResponse)
        {
            GlobusLogHelper.log.Info(Log.CustomMessage,
                       dominatorAccountModel.AccountBaseModel.AccountNetwork,
                       dominatorAccountModel.AccountBaseModel.UserName, "Two Factor Login",
                       "Two factor authentication please enter code which has been sent to your associate phone no");
            JToken jToken = JObject.Parse(loginResponse);
            string choice = jToken["two_factor_info"]["two_factor_identifier"].ToString();
            dominatorAccountModel.two_factor_identifier = choice;
            dominatorAccountModel.AccountBaseModel.Status = AccountStatus.TwoFactorLoginAttempt;
            dominatorAccountModel.Cookies = _httpHelper.GetRequestParameter().Cookies;
            SocinatorAccountBuilder.Instance(dominatorAccountModel.AccountBaseModel.AccountId)
                .AddOrUpdateDominatorAccountBase(dominatorAccountModel.AccountBaseModel)
                .AddOrUpdateMobileAgentWeb(dominatorAccountModel.UserAgentMobile)
                .AddOrUpdateCookies(dominatorAccountModel.Cookies)
                .SaveToBinFile();
        }

        public void WrongUserName(DominatorAccountModel dominatorAccountModel, AccountModel accountModel, string loginResponse)
        {
            string errorMessage = JObject.Parse(loginResponse)["message"].ToString();
            // dominatorAccountModel.AccountBaseModel.AccountGroup.Content = "InvalidCredentials";
            SetLoginStatus(dominatorAccountModel, accountModel, AccountStatus.InvalidCredentials, true);
            GlobusLogHelper.log.Info(Log.CustomMessage,
                                    dominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    dominatorAccountModel.AccountBaseModel.UserName, "Login", errorMessage);
        }
        public void FoundCaptcha(DominatorAccountModel dominatorAccountModel, AccountModel accountModel, string response)
        {
            // string errorMessage = JObject.Parse(loginResponse)["message"].ToString();
            // dominatorAccountModel.AccountBaseModel.AccountGroup.Content = "FoundCaptcha";
            SetLoginStatus(dominatorAccountModel, accountModel, AccountStatus.FoundCaptcha, true);
            GlobusLogHelper.log.Info(Log.CustomMessage,
                                    dominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    dominatorAccountModel.AccountBaseModel.UserName, "Login", "Need to resolve captcha");
        }
        public void SubmitPhoneNumber(DominatorAccountModel dominatorAccountModel, AccountModel accountModel)

        {
            // string errorMessage = JObject.Parse(loginResponse)["message"].ToString();
            // dominatorAccountModel.AccountBaseModel.AccountGroup.Content = "AddPhoneAndVerify";
            SetLoginStatus(dominatorAccountModel, accountModel, AccountStatus.AddPhoneAndVerify, true);
            GlobusLogHelper.log.Info(Log.CustomMessage,
                                    dominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    dominatorAccountModel.AccountBaseModel.UserName, "Login", "Please submit your phone number");
        }
        private string SaveImage(string captchaImage)
        {
            string downloadPath = $@"{Path.GetTempPath()}{DateTime.Now.GetCurrentEpochTime()}.jpg";
            File.WriteAllBytes(downloadPath, Convert.FromBase64String(captchaImage));

            return downloadPath;
        }

        private string SolveCaptcha(string response, string url, DominatorAccountModel dominatorAccountModel)
        {
            string siteKey = String.Empty;
            siteKey = "6Lc9qjcUAAAAADTnJq5kJMjN9aD1lxpRLMnCS2TR";
            //siteKey = "6LebnxwUAAAAAGm3yH06pfqQtcMH0AYDwlsXnh-u";
            url = "https://www.fbsbx.com/captcha/recaptcha/iframe/?compact=0&referer=https%3A%2F%2Fi.instagram.com&locale=en_US&__cci=ig_captcha_iframe";
            var genericFileManager = InstanceProvider.GetInstance<IGenericFileManager>();
            var imageCaptchaServicesModel = genericFileManager.GetModel<ImageCaptchaServicesModel>(ConstantVariable.GetImageCaptchaServicesFile()) ?? new ImageCaptchaServicesModel();

            if (string.IsNullOrEmpty(imageCaptchaServicesModel.Token))
                throw new Exception("ImageTypers captcha solver token not found");
            var compositProxy = string.Empty;
            if (!string.IsNullOrEmpty(dominatorAccountModel.AccountBaseModel.AccountProxy.ProxyIp) && !string.IsNullOrEmpty(dominatorAccountModel.AccountBaseModel.AccountProxy.ProxyPort))
            {
                compositProxy = $"{dominatorAccountModel.AccountBaseModel.AccountProxy.ProxyIp}:{dominatorAccountModel.AccountBaseModel.AccountProxy.ProxyPort}";
                if (!string.IsNullOrEmpty(dominatorAccountModel.AccountBaseModel.AccountProxy.ProxyUsername) && !string.IsNullOrEmpty(dominatorAccountModel.AccountBaseModel.AccountProxy.ProxyPassword))
                {
                    compositProxy = $"{compositProxy}:{dominatorAccountModel.AccountBaseModel.AccountProxy.ProxyUsername}:{dominatorAccountModel.AccountBaseModel.AccountProxy.ProxyPassword}";
                }
            }
            //var two_captcha = new two_captcha_helper("");
            var imageTyperzHelper = new ImageTyperzHelper(imageCaptchaServicesModel.Token, compositProxy);
            var textCaptcha = imageTyperzHelper.SubmitSiteKey(url, siteKey);//two_captcha.SubmitSiteKey(siteKey, url).Result;//;
            var captchaResponse = imageTyperzHelper.GetGResponseCaptcha(textCaptcha, 2); //two_captcha.GetCaptchaResponse(textCaptcha).Result;//
            return captchaResponse;
        }

        public void WrongPassword(DominatorAccountModel dominatorAccountModel, AccountModel accountModel, string loginResponse)
        {
            // dominatorAccountModel.AccountBaseModel.AccountGroup.Content = "InvalidCredentials";
            SetLoginStatus(dominatorAccountModel, accountModel, AccountStatus.InvalidCredentials, true);
            GlobusLogHelper.log.Info(Log.CustomMessage,
                                    dominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    dominatorAccountModel.AccountBaseModel.UserName, "Login", "The password you entered is incorrect. Please enter the correct password and try again.");
        }

        public void AccountFailStatus(DominatorAccountModel dominatorAccountModel, AccountModel accountModel)
        {
            //  dominatorAccountModel.AccountBaseModel.AccountGroup.Content = "Failed";
            SetLoginStatus(dominatorAccountModel, accountModel, AccountStatus.Failed, false);
            GlobusLogHelper.log.Info(Log.CustomMessage,
                                    dominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    dominatorAccountModel.AccountBaseModel.UserName, "Login", "Login failed.");
        }

        public void ProxyIssueWithAccount(DominatorAccountModel dominatorAccountModel, AccountModel accountModel)
        {
            // dominatorAccountModel.AccountBaseModel.AccountGroup.Content = "Check Proxy";
            SetLoginStatus(dominatorAccountModel, accountModel, AccountStatus.CheckProxy, false);
            GlobusLogHelper.log.Info(Log.CustomMessage,
                                    dominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    dominatorAccountModel.AccountBaseModel.UserName, "Login", "Login failed.");
            GlobusLogHelper.log.Info(Log.CustomMessage,
                                    dominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    dominatorAccountModel.AccountBaseModel.UserName, "Login", "There is issue with your IP while using in instagram so please check either without IP or try " +
                                    "with other IP");
        }

        public void AccountDisable(DominatorAccountModel dominatorAccountModel, AccountModel accountModel)
        {
            //dominatorAccountModel.AccountBaseModel.AccountGroup.Content = "DisableAccount";
            SetLoginStatus(dominatorAccountModel, accountModel, AccountStatus.DisableAccount, false);
            GlobusLogHelper.log.Info(Log.CustomMessage,
                                    dominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    dominatorAccountModel.AccountBaseModel.UserName, "Login", "Your account has been disabled for violating our terms.");
        }

        public void SentryBlock(DominatorAccountModel dominatorAccountModel, AccountModel account)
        {
            // dominatorAccountModel.AccountBaseModel.AccountGroup.Content = "TemporarilyBlocked";
            SetLoginStatus(dominatorAccountModel, account, AccountStatus.TemporarilyBlocked, false);
            GlobusLogHelper.log.Info(Log.CustomMessage,
                                    dominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    dominatorAccountModel.AccountBaseModel.UserName, "Login", "Your account has been Temperory Block");
        }
        public void SuspiciousAccount(DominatorAccountModel dominatorAccountModel, AccountModel account)
        {
            // dominatorAccountModel.AccountBaseModel.AccountGroup.Content = "SuspiciousAccount";
            SetLoginStatus(dominatorAccountModel, account, AccountStatus.DisableAccount, false);
            GlobusLogHelper.log.Info(Log.CustomMessage,
                                    dominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    dominatorAccountModel.AccountBaseModel.UserName, "Login", "Your account has been disabled for violating our terms. Please try to login after few days");
        }
        public bool GetSendingCodeType(DominatorAccountModel dominatorAccountModel, IResponseParameter response, LoginIgResponseHandler logInResponseHandler, AccountModel accountModel, CancellationToken token)
        {
            try
            {
                string emailId = string.Empty;
                string phoneNo = string.Empty;
                if (response.Response.Contains("DOCTYPE html"))
                {
                    var fields = Regex.Split(response.Response, "fields");
                    var type = Utilities.GetBetween(fields[2], ":", ",\"navigation");
                    // Choice value will be always 0 or 1.
                    // Choice "0" value means verify through Phone No.
                    // Choice "1" value means verify through Email Id.
                    JObject jObj = JObject.Parse(type);

                    if (type.Contains("phone_number"))
                    {
                        phoneNo = jObj["phone_number"].ToString();
                    }
                    if (type.Contains("email"))
                    {
                        emailId = jObj["email"].ToString();
                    }
                }
                else
                {
                    JToken jToken = JObject.Parse(response.Response)["step_data"];
                    emailId = jToken["email"]?.ToString() ?? String.Empty;
                    if (string.IsNullOrEmpty(emailId) && (jToken["form_type"]?.ToString() ?? string.Empty) == "email")
                        emailId = jToken["contact_point"]?.ToString() ?? String.Empty;
                    phoneNo = jToken["phone_number"]?.ToString() ?? String.Empty;
                    if (string.IsNullOrEmpty(phoneNo))
                        phoneNo = jToken["phone_number_formatted"]?.ToString() ?? string.Empty;
                }
                if (!string.IsNullOrEmpty(emailId) && !string.IsNullOrEmpty(phoneNo))
                {
                    if (AutoVerifyViaEmail(dominatorAccountModel, accountModel, token, logInResponseHandler)) return false;
                    // dominatorAccountModel.AccountBaseModel.AccountGroup.Content = "NeedsVerification";
                    dominatorAccountModel.AccountBaseModel.Status = AccountStatus.NeedsVerification;
                }
                else if (!String.IsNullOrEmpty(phoneNo) && String.IsNullOrEmpty(emailId))
                {
                    // dominatorAccountModel.AccountBaseModel.AccountGroup.Content = "PhoneVerification";
                    dominatorAccountModel.AccountBaseModel.Status = AccountStatus.PhoneVerification;
                }
                else if (!string.IsNullOrEmpty(emailId) && String.IsNullOrEmpty(phoneNo))
                {
                    if (AutoVerifyViaEmail(dominatorAccountModel, accountModel, token, logInResponseHandler)) return false;

                    dominatorAccountModel.AccountBaseModel.Status = AccountStatus.EmailVerification;
                    //dominatorAccountModel.AccountBaseModel.AccountGroup.Content = "EmailVerification";
                }
                SocinatorAccountBuilder.Instance(dominatorAccountModel.AccountBaseModel.AccountId)
                    .AddOrUpdateDominatorAccountBase(dominatorAccountModel.AccountBaseModel).AddOrUpdateCookies(dominatorAccountModel.Cookies)
                    .SaveToBinFile();
            }
            catch (Exception)
            {
                // if no, by default choice exists in response
                dominatorAccountModel.AccountBaseModel.Status = AccountStatus.NeedsVerification;
                SocinatorAccountBuilder.Instance(dominatorAccountModel.AccountBaseModel.AccountId)
                    .AddOrUpdateDominatorAccountBase(dominatorAccountModel.AccountBaseModel)
                    .SaveToBinFile();
            }
            return true;
        }

        public async Task AccountItWasMe(DominatorAccountModel dominatorAccountModel, CancellationToken token)
        {
            GlobusLogHelper.log.Info(Log.CustomMessage,
                        dominatorAccountModel.AccountBaseModel.AccountNetwork,
                        dominatorAccountModel.AccountBaseModel.UserName, "Login",
                        "Auto verifying, Instagram's -this was me-challenge");

            if (!dominatorAccountModel.IsUserLoggedIn)
            {
                GlobusLogHelper.log.Info(Log.LoginFailed,
                    dominatorAccountModel.AccountBaseModel.AccountNetwork,
                    dominatorAccountModel.AccountBaseModel.UserName, "Login");
                LoginWithDataBaseCookies(dominatorAccountModel, true, token);
            }
            if (dominatorAccountModel.IsUserLoggedIn)
            {
                string responseForChallange =
                    (await _httpHelper.GetRequestAsync("https://www.instagram.com", token))
                    .Response;
                if (responseForChallange.Contains("https://www.instagram.com/challenge/"))
                {
                    try
                    {
                        IRequestParameters requestParameters =
                            _httpHelper.GetRequestParameter();
                        #region Commented Cookies Condition
                        //if (requestParameters.Headers["X-CSRFToken"] == null)
                        //{
                        //    requestParameters.Headers.Add("X-CSRFToken",
                        //        requestParameters.Cookies["csrftoken"].Value);
                        //}
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                    string reponseAfterChallange = (await _httpHelper.PostRequestAsync(
                        "https://www.instagram.com/challenge/", "choice=0", token)).Response;

                    if (reponseAfterChallange.Contains("CHALLENGE_REDIRECTION") &&
                        reponseAfterChallange.Contains("\"status\": \"ok\""))
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage,
                            dominatorAccountModel.AccountBaseModel.AccountNetwork,
                            dominatorAccountModel.AccountBaseModel.UserName, "Login",
                            "Instagram's -this was me-challenge has successfully completed. Now login in again...");
                        LoginWithDataBaseCookies(dominatorAccountModel, true, token);
                    }
                    else
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage,
                            dominatorAccountModel.AccountBaseModel.AccountNetwork,
                            dominatorAccountModel.AccountBaseModel.UserName, "Login",
                            "Instagram's -this was me-challenge has failed. Please try again...");
                    }
                }
            }
            else
            {
                GlobusLogHelper.log.Info(Log.CustomMessage,
                    dominatorAccountModel.AccountBaseModel.AccountNetwork,
                    dominatorAccountModel.AccountBaseModel.UserName, "Login", "Instagram login failed.");
            }
        }

        private bool InstagramDataScraper(DominatorAccountModel dominatorAccountModel, AccountModel accountModel)
        {
            try
            {
                if (dominatorAccountModel.IsUserLoggedIn )
                {
//#if !DEBUG
//                    var instaAdsScrappers = _accountScopeFactory[dominatorAccountModel.AccountId].Resolve<IInstaAdScrappers>();
//                    Thread Ads = new Thread(() => instaAdsScrappers.InstaDataScraper(dominatorAccountModel, accountModel));
//                    Ads.Start();
//#endif
                    return false;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return true;
        }

        private async Task<bool> StartTwoFactorLogin(DominatorAccountModel dominatorAccountModel, AccountModel accountModel, IRequestParameters requestParameter, CancellationToken token)
        {
            if (dominatorAccountModel.AccountBaseModel.IsChkTwoFactorLogin)
            {
                bool status = await TwoFactorLoginProcess(dominatorAccountModel, accountModel, requestParameter, token);
                if (status)
                    return false;
            }
            return true;
        }

        public void LoginWithBrowserMethod(DominatorAccountModel dominatorAccountModel, CancellationToken token, VerificationType verificationType = 0, LoginType loginType = LoginType.AutomationLogin)
        {
            _browserManagerFactory.CheckStatusAsync(dominatorAccountModel, token,loginType);
            GdBrowserManager = _browserManagerFactory.GdBrowserManager(dominatorAccountModel, token);
        }

        public IInstaFunction AssignBrowserFunction(DominatorAccountModel dominatorAccountModel)
        {
            InstagramFunctFactory.AssignInstaFunctions(dominatorAccountModel);
            return instaFunct;
        }
        #region commented save cookies process in DB

        //public void ResetCookies(CookieCollection Cookies, DominatorAccountModel dominatorAccountModel)
        //{
        //    dominatorAccountModel.Cookies = new CookieCollection();
        //    foreach (Cookie cookie in Cookies)
        //    {
        //        var cookieHelper = new CookieHelper();
        //        cookieHelper.Name = cookie.Name;
        //        cookieHelper.Value = cookie.Value;
        //        cookieHelper.Domain = cookie.Domain;
        //        cookieHelper.Expires = cookie.Expires;
        //        cookieHelper.HttpOnly = cookie.HttpOnly;
        //        cookieHelper.Secure = cookie.Secure;

        //        if (cookie.Name.Contains("mid") || cookie.Name.Contains("csrftoken") || cookie.Name.Contains("sessionid") || cookie.Name.Contains("ds_user_id")
        //            || cookie.Name.Contains("rur") || cookie.Name.Contains("ds_user") || cookie.Name.Contains("igfl"))
        //        {
        //            dominatorAccountModel.CookieHelperList.Add(cookieHelper);
        //            dominatorAccountModel.Cookies.Add(cookie);
        //        }

        //    }
        //}
        #endregion

        private async Task GetCountryByIP(DominatorAccountModel dominatorAccountModel, AccountModel accountModel)
        {

            IpInfo ipInfo = new IpInfo();
            IgRequestParameters objIgHttpHelper = new IgRequestParameters(dominatorAccountModel.UserAgentMobile);
            string ips;
            string ipUrl = "https://app.multiloginapp.com/WhatIsMyIP";
            if (string.IsNullOrEmpty(dominatorAccountModel.AccountBaseModel.AccountProxy.ProxyIp))
            {
                string ip = objIgHttpHelper.IgGetResponse(ipUrl);
                ips = Utilities.GetBetween(ip, "pti-header bgm-green\">", "/h2>");
                ips = Utilities.GetBetween(ips, ">", "<").Trim('\n').Trim(' ');
            }
            else
                ips = dominatorAccountModel.AccountBaseModel.AccountProxy.ProxyIp;

            var locationUrl = $"http://api.db-ip.com/v2/f36e244f142c952d67116fbbba1aca49f74579d1/{ips.Trim()}";
            string response = objIgHttpHelper.IgGetResponse(locationUrl);
            JObject JResp;
            JResp = JObject.Parse(response);

            var city = JResp["city"].ToString();
            var state = JResp["stateProv"].ToString();
            var country = JResp["countryName"].ToString();
            var phonePrefix = JResp["phonePrefix"].ToString();
            var countryCode = JResp["countryCode"].ToString();
            dominatorAccountModel.UserAgentMobile = dominatorAccountModel.UserAgentMobile.Replace("en_US", $"en_{countryCode}");
            accountModel.CountryCode = phonePrefix;
            accountModel.CountryName = countryCode;
        }

        private async Task<bool> LoginCaptcha(DominatorAccountModel dominatorAccountModel, AccountModel accountModel, IResponseParameter response, IRequestParameters requestParameter, LoginIgResponseHandler logInResponseHandler, CancellationToken token)
        {
            if (!response.Response.Contains("select_verify_method") && !response.Response.Contains("{\"action\": \"close\", \"status\": \"ok\"}") && !response.Response.ToString().Contains("bloks_action"))
            {
                accountModel.AccountVerifyType = "DifferentAccount";
                response = await GetCodeSendingType(dominatorAccountModel, accountModel, requestParameter, logInResponseHandler, response, token);

                string apiUrl = $"https://i.instagram.com{logInResponseHandler.Issue.ApiPath}";
                if (response.Response.ToString().Contains("sitekey") || response.Response.ToString().Contains("RecaptchaChallengeForm"))
                {
                    FoundCaptcha(dominatorAccountModel, accountModel, logInResponseHandler.ToString());
                    return true;
                }
                if (response.Response.ToString().Contains("SubmitPhoneNumberForm"))
                {
                    SubmitPhoneNumber(dominatorAccountModel, accountModel);
                    return true;
                }
            }
            return false;
        }

        private async Task BlocksAction(DominatorAccountModel dominatorAccountModel, AccountModel accountModel, IResponseParameter response, CancellationToken token)
        {
            if (response.Response.ToString().Contains("bloks_action"))
            {
                JToken jToken = JObject.Parse(response.Response.ToString());
                string choice = jToken["bloks_action"].ToString();
                accountModel.blockAction = choice;
            }
        }

        private async Task InstagramChallenge(DominatorAccountModel dominatorAccountModel, AccountModel accountModel, IRequestParameters requestParameter, LoginIgResponseHandler logInResponseHandler, CommonIgResponseHandler resp, CancellationToken token)
        {
            IResponseParameter response = null;
            if (logInResponseHandler.Issue != null && logInResponseHandler.Issue?.Error == InstagramError.Challenge)
                response = await GetChallengeResponse(dominatorAccountModel, accountModel, requestParameter, logInResponseHandler, resp, token);
            else
            {
                response = await GetChallengeResponse(dominatorAccountModel, accountModel, requestParameter, logInResponseHandler, null, token);
                if (await LoginCaptcha(dominatorAccountModel, accountModel, response, requestParameter, logInResponseHandler, token))
                    return;
                await BlocksAction(dominatorAccountModel, accountModel, response, token);
            }
            if (!AccountChallenge(dominatorAccountModel, response))
                return;
            if (!GetSendingCodeType(dominatorAccountModel, response, logInResponseHandler, accountModel, token))
                return;
        }

        private async Task LoginSuccesStatus(DominatorAccountModel dominatorAccountModel, AccountModel accountModel, LoginIgResponseHandler logInResponseHandler, CancellationToken token)
        {
            try
            {
                if (string.IsNullOrEmpty(dominatorAccountModel.AccountBaseModel.ProfileId))
                    dominatorAccountModel.AccountBaseModel.ProfileId = logInResponseHandler.Username;
            }
            catch (Exception) { }
            await HittinAllNeededRequest(dominatorAccountModel, accountModel, token);
            SetAccountDetails(dominatorAccountModel, accountModel, logInResponseHandler.Pk);
        }

        private async Task LoginHeaderSet(DominatorAccountModel dominatorAccountModel, AccountModel accountModel, LoginIgResponseHandler logInResponseHandler, CancellationToken token)
        {
            if (logInResponseHandler.Success)
            {

                if (_httpHelper.Response.Headers["x-ig-set-WWW-Claim"] != null)
                {
                    var wwwclaim = _httpHelper.Response.Headers["x-ig-set-WWW-Claim"];
                    _httpHelper.GetRequestParameter().AddHeader("X-IG-WWW-Claim", wwwclaim);
                    accountModel.WwwClaim = wwwclaim;
                }
            }
        }

        private async Task AfterLoginProcess(DominatorAccountModel dominatorAccountModel, AccountModel accountModel, LoginIgResponseHandler logInResponseHandler, IRequestParameters requestParameter, CancellationToken token)
        {
            //await LoginHeaderSet(dominatorAccountModel, accountModel, logInResponseHandler, token);
            accountModel.AuthorizationHeader = logInResponseHandler.AuthorizationHeader;
            if (logInResponseHandler.ToString().Contains("The password that you've entered is incorrect. Please try again."))
            {
                WrongPassword(dominatorAccountModel, accountModel, logInResponseHandler.ToString());
                return;
            }
            CommonIgResponseHandler resp = null;
            if (logInResponseHandler != null && !logInResponseHandler.IsChallenge && !logInResponseHandler.ToString().Contains("Your account has been disabled"))
                resp = instaFunct.GetFeedTimeLineData(dominatorAccountModel, accountModel);
            await LoginHeaderSet(dominatorAccountModel, accountModel, logInResponseHandler, token);
            string loginResponse = logInResponseHandler.ToString();
            if (logInResponseHandler.Success && !logInResponseHandler.IsChallenge && !logInResponseHandler.ToString().Contains("Your account has been disabled for violating our terms. Learn how you may be able to restore your account") && !resp.ToString().Contains("challenge_required"))
                await LoginSuccesStatus(dominatorAccountModel, accountModel, logInResponseHandler, token);
            else if (logInResponseHandler.Issue?.Error == InstagramError.Challenge && !logInResponseHandler.ToString().Contains("Your account has been disabled for violating our terms. Learn how you may be able to restore your account"))
                await InstagramChallenge(dominatorAccountModel, accountModel, requestParameter, logInResponseHandler, resp, token);
            else if (loginResponse.Contains("two_factor_required"))
                TwofactorRequired(dominatorAccountModel, loginResponse);
            else if (loginResponse.Contains("https://i.instagram.com/integrity"))
                await AccountItWasMe(dominatorAccountModel, token);
            else if (loginResponse.ToLower().Contains("the username you entered doesn't appear to belong to an account"))
                WrongUserName(dominatorAccountModel, accountModel, loginResponse);
            else if (loginResponse.ToLower().Contains("the password you entered is incorrect"))
                WrongPassword(dominatorAccountModel, accountModel, loginResponse);
            else if (loginResponse.Contains("Your account has been disabled for violating our term"))
                AccountDisable(dominatorAccountModel, accountModel);
            else if (loginResponse.Contains("Sorry, there was a problem with your request") || loginResponse.Contains("There was an error with your request. Please try again"))
                SentryBlock(dominatorAccountModel, accountModel);
            else if (loginResponse.Contains("Your account has been disabled for violating our terms. Learn how you may be able to restore your account"))
                SuspiciousAccount(dominatorAccountModel, accountModel);
            else if (loginResponse.Contains("Please wait a few minutes before trying again."))
                ProxyIssueWithAccount(dominatorAccountModel, accountModel);
            else
                AccountFailStatus(dominatorAccountModel, accountModel);
        }

        public async Task<bool> ManualCaptchaAsync(DominatorAccountModel dominatorAccountModel, CancellationToken cancellationToken)
        {
            try
            {
                await SolveCaptchaManualAndLoginAsync(dominatorAccountModel, dominatorAccountModel.Token);
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (AggregateException ae)
            {
                ae.HandleOperationCancellation();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return dominatorAccountModel.IsUserLoggedIn;
        }
    }
}