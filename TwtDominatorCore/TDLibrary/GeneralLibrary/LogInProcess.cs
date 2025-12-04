using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using CommonServiceLocator;
using ThreadUtils;
using DominatorHouseCore;
using DominatorHouseCore.BusinessLogic.ActivitiesWorkflow;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Extensions;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using EmbeddedBrowser;
using Newtonsoft.Json;
using TwtDominatorCore.Interface;
using TwtDominatorCore.Requests;
using TwtDominatorCore.Response;
using TwtDominatorCore.TDFactories;
using TwtDominatorCore.TDLibrary.GeneralLibrary;
using TwtDominatorCore.TDModels;
using TwtDominatorCore.TDUtility;
using static TwtDominatorCore.TDEnums.Enums;
using DominatorHouseCore.PuppeteerBrowser;

namespace TwtDominatorCore.TDLibrary
{
    public interface ITwtLogInProcess : ILoginProcessAsync
    {
        bool IsCheckAccountStatus { get; set; }
        Task UpdatingUserProfileDetails(DominatorAccountModel dominatorAccountModel);
        void SetRequestParameter(ref DominatorAccountModel dominatorAccountModel);
        Task AfterLoginSuccess(DominatorAccountModel dominatorAccountModel);
        void SetRequestHeader(string accept, string refer, string dest);
    }

    public class LogInProcess : ITwtLogInProcess
    {
        // lock updating account details for verification
        public static readonly object LockFile = new object();
        private static string Domain => TdConstants.Domain;
        // lock update global account details
        public static readonly object LockUpdateGlobalAccountDetails = new object();
        public static readonly object LockUpdateAccountDetails = new object();
        private readonly IAccountContactConfig _accountContactConfig;
        private readonly IAccountsFileManager _accountsFileManager;
        private readonly ITdHttpHelper _httpHelper;
        private readonly IDelayService _delayService;

        private readonly ITwitterFunctionFactory _twitterFunctionFactory;
        private readonly ITwitterAccountSessionManager twitterAccountSession;
        private readonly JsonJArrayHandler handler = JsonJArrayHandler.GetInstance;
        public LogInProcess(ITwitterFunctionFactory twitterFunctionFactory, IAccountContactConfig accountContactConfig,
            IAccountsFileManager accountsFileManager, ITdHttpHelper httpHelper, IDelayService delayService, ITwitterAccountSessionManager sessionManager)

        {
            _twitterFunctionFactory = twitterFunctionFactory;
            _accountContactConfig = accountContactConfig;
            _accountsFileManager = accountsFileManager;
            _httpHelper = httpHelper;
            _delayService = delayService;
            twitterAccountSession = sessionManager;
        }

        public BrowserWindow _browserWindow { get; set; }

        // changement in to assign functions factory beacuse of NewUi error to slove this we use this way.
        private ITwitterFunctions _twitterFunctions => _twitterFunctionFactory.TwitterFunctions;
        public bool IsCheckAccountStatus { get; set; }

        public bool CheckLogin(DominatorAccountModel dominatorAccountModel, CancellationToken cancellationToken)
        {
            return CheckLoginAsync(dominatorAccountModel, cancellationToken).Result;
        }

        /// <summary>
        ///     First time login and non job process
        /// </summary>
        /// <param name="dominatorAccountModel"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> CheckLoginAsync(DominatorAccountModel dominatorAccountModel,
            CancellationToken cancellationToken, bool displayLoginMsg = false, LoginType loginType = LoginType.AutomationLogin)
        {
            IsCheckAccountStatus = true;
            //Attempt to login status
            twitterAccountSession.AddOrUpdateSession(ref dominatorAccountModel);
            GlobusLogHelper.log.Info(Log.AccountLogin, dominatorAccountModel.AccountBaseModel.AccountNetwork,
                dominatorAccountModel.UserName);

            // checking already exists user or not
            if (CheckAlreadyExist(dominatorAccountModel))
            {
                GlobusLogHelper.log.Info(Log.LoginFailed, dominatorAccountModel.AccountBaseModel.AccountNetwork,
                    dominatorAccountModel.UserName, "LangKeyRemoveAccountAlreadyPresent".FromResourceDictionary());
                dominatorAccountModel.AccountBaseModel.Status = AccountStatus.Failed;
                return false;
            }

            if (dominatorAccountModel.IsRunProcessThroughBrowser)
                LoginWithBrowserMethod(dominatorAccountModel, cancellationToken);
            else
                await LoginWithDataBaseCookiesAsync(dominatorAccountModel, false, cancellationToken);
            // Attempt to Login
            //  login successful
            if (dominatorAccountModel.IsUserLoggedIn)
            {
                twitterAccountSession.AddOrUpdateSession(ref dominatorAccountModel,true);
                GlobusLogHelper.log.Info(Log.SuccessfulLogin, dominatorAccountModel.AccountBaseModel.AccountNetwork,
                    dominatorAccountModel.UserName);
            }
            else
            {
                GlobusLogHelper.log.Info(Log.LoginFailed,
                        dominatorAccountModel.AccountBaseModel.AccountNetwork,
                        dominatorAccountModel.AccountBaseModel.UserName, dominatorAccountModel.AccountBaseModel.Status);
            }
            return dominatorAccountModel.IsUserLoggedIn;
        }

        public void LoginWithDataBaseCookies(DominatorAccountModel dominatorAccountModel, bool isMobileRequired,
            CancellationToken cancellationToken)
        {
            LoginWithDataBaseCookiesAsync(dominatorAccountModel, isMobileRequired, cancellationToken).Wait();
        }

        public async Task LoginWithDataBaseCookiesAsync(DominatorAccountModel dominatorAccountModel,
            bool isMobileRequired, CancellationToken cancellationToken)
        {
            try
            {
                twitterAccountSession.AddOrUpdateSession(ref dominatorAccountModel);
                if (await CheckLogInWithDatabaseCookieAsync(dominatorAccountModel,dominatorAccountModel.Token))
                {
                    dominatorAccountModel.AccountBaseModel.Status = AccountStatus.Success;
                    twitterAccountSession.AddOrUpdateSession(ref dominatorAccountModel,true);
                }
                else
                {
                    #region BrowserLogin
                    //LoginWithBrowserMethod(dominatorAccountModel, dominatorAccountModel.Token);
                    //if (await CheckLogInWithDatabaseCookieAsync(dominatorAccountModel, cancellationToken))
                    //{
                    //    dominatorAccountModel.AccountBaseModel.Status = AccountStatus.Success;
                    //    dominatorAccountModel.IsUserLoggedIn = true;
                    //}
                    #endregion
                    #region Old Login Code.
                    SetRequestParameter(ref dominatorAccountModel);
                    if (dominatorAccountModel.Cookies.Count != 0)
                    {
                        // Attempt to login
                        setCookieFromDatabase(dominatorAccountModel);
                        if (await CheckLogInWithDatabaseCookieAsync(dominatorAccountModel, cancellationToken))
                        {
                            dominatorAccountModel.IsUserLoggedIn = true;
                            dominatorAccountModel.AccountBaseModel.Status = AccountStatus.Success;
                        }
                        else
                            LoginWithAlternativeMethodAsync(dominatorAccountModel, cancellationToken).Wait();
                    }
                    else
                    {
                        LoginWithAlternativeMethodAsync(dominatorAccountModel, cancellationToken).Wait();
                    }
                    if (!dominatorAccountModel.IsUserLoggedIn && !InvalidAccount(dominatorAccountModel))
                    {
                        LoginWithBrowserMethod(dominatorAccountModel, dominatorAccountModel.Token);
                        if (await CheckLogInWithDatabaseCookieAsync(dominatorAccountModel, cancellationToken))
                        {
                            dominatorAccountModel.AccountBaseModel.Status = AccountStatus.Success;
                            dominatorAccountModel.IsUserLoggedIn = true;
                        }
                    }
                    // updating global account details
                    #endregion
                }
                UpdateGlobalAccountDetails(dominatorAccountModel);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            finally
            {
                TdAccountsBrowserDetails.CloseBrowser(dominatorAccountModel, BrowserInstanceType.CheckAccountStatus);
            }
        }

        private bool InvalidAccount(DominatorAccountModel dominatorAccountModel)
        {
            return dominatorAccountModel.AccountBaseModel.Status
                == AccountStatus.EmailVerification
                || dominatorAccountModel.AccountBaseModel.Status == AccountStatus.InvalidCredentials
                || dominatorAccountModel.AccountBaseModel.Status == AccountStatus.ReTypeEmail
                || dominatorAccountModel.AccountBaseModel.Status == AccountStatus.TemporarilyBlocked;
        }
        public void LoginWithAlternativeMethod(DominatorAccountModel dominatorAccountModel,
            CancellationToken cancellationToken)
        {
            LoginWithAlternativeMethodAsync(dominatorAccountModel, cancellationToken).Wait();
        }

        public async Task LoginWithAlternativeMethodAsync(DominatorAccountModel dominatorAccountModel,
            CancellationToken cancellationToken)
        {
            try
            {
                twitterAccountSession.AddOrUpdateSession(ref dominatorAccountModel);
                var accountModel = new AccountModel(dominatorAccountModel);

                SetRequestParameter(ref dominatorAccountModel);
                string accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9";
                SetRequestHeader(accept, "", "document");
                var firstresponse =
                    await _httpHelper.GetRequestAsync(TdConstants.MainHomeUrl, cancellationToken);
                var gta = Utilities.GetBetween(firstresponse.Response, "document.cookie=\"gt=", ";").Replace("gt=", "");
                var RequestParam = _httpHelper.GetRequestParameter();
                if(!string.IsNullOrEmpty(gta))
                    RequestParam.Cookies.Add(new Cookie("gt", gta) { Domain = $"{Domain}" });

                var ctoRandomly = TdUtility.GetRandomHexNumber(32).ToLower();
                RequestParam.Cookies
                    .Add(new Cookie("ct0", ctoRandomly) { Domain = $"{Domain}" });
                SetRequestHeader("*/*", $"https://{Domain}/", "script");
                if (string.IsNullOrEmpty(firstresponse.Response))
                {
                    await _delayService.DelayAsync(new Random().Next(0, 6000), cancellationToken);
                    firstresponse =
                        await _httpHelper.GetRequestAsync(TdConstants.MainHomeUrl, cancellationToken);
                }

                if (firstresponse.HasError)
                {
                    if (!string.IsNullOrEmpty(dominatorAccountModel.AccountBaseModel.AccountProxy.ProxyIp))
                        dominatorAccountModel.AccountBaseModel.Status = AccountStatus.ProxyNotWorking;
                    else
                        dominatorAccountModel.AccountBaseModel.Status = AccountStatus.Failed;

                    cancellationToken.ThrowIfCancellationRequested();

                    GlobusLogHelper.log.Info(Log.LoginFailed, dominatorAccountModel.AccountBaseModel.AccountNetwork,
                        dominatorAccountModel.UserName,
                        string.Format("LangKeyProxyNotWorkingMessage".FromResourceDictionary(),
                            firstresponse.Exception?.Message));
                    return;
                }
                _httpHelper.SetRequestParameter(RequestParam);
                accountModel.CsrfToken = ctoRandomly;
                dominatorAccountModel.Cookies = new CookieCollection();
                var logInResponse = await _twitterFunctions.LoginUsingFlowToken(dominatorAccountModel, cancellationToken);
                if (logInResponse.Response.Contains("temporarily limited some of your account features"))
                    GlobusLogHelper.log.Debug(
                        $" {dominatorAccountModel.UserName} We've temporarily limited some of your account features");

                // add await here
                if (logInResponse.Success)
                {
                    await AfterLoginSuccess(dominatorAccountModel);
                }
                else
                {
                    dominatorAccountModel.IsUserLoggedIn = false;
                    if (logInResponse != null && logInResponse.DominatorStatus != null)
                        dominatorAccountModel.AccountBaseModel.Status = logInResponse.DominatorStatus;
                    //AccountContactConfig.ErrorMessage(dominatorAccountModel, logInResponse.status);
                    var accountVerification = new AccountVerification
                    {
                        AccountModel = accountModel,
                        LogInPageResponse = logInResponse
                    };
                    accountVerification.VerifyingAccount(dominatorAccountModel, logInResponse);

                    UpdateDominatorAccountModel(dominatorAccountModel);

                    // after verification if verified successfully
                }

                UpdateGlobalAccountDetails(dominatorAccountModel);
                cancellationToken.ThrowIfCancellationRequested();
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

        public async Task AfterLoginSuccess(DominatorAccountModel dominatorAccountModel)
        {
            #region after login success

            dominatorAccountModel.Cookies = _httpHelper.GetRequestParameter().Cookies;
            dominatorAccountModel.AccountBaseModel.Status = AccountStatus.Success;
            dominatorAccountModel.IsUserLoggedIn = true;
            twitterAccountSession.AddOrUpdateSession(ref dominatorAccountModel,true);
            try
            {
                if (HasUserProfileDetails(dominatorAccountModel))
                    await UpdatingUserProfileDetails(dominatorAccountModel);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            #endregion
        }

        public void SetRequestParameter(ref DominatorAccountModel dominatorAccountModel)
        {
            var objRequestParametersWeb = new TdRequestParameters();
            objRequestParametersWeb.Cookies = new CookieCollection();
            try
            {
                objRequestParametersWeb.Proxy = new Proxy
                {
                    ProxyIp = dominatorAccountModel.AccountBaseModel.AccountProxy.ProxyIp,
                    ProxyPassword = dominatorAccountModel.AccountBaseModel.AccountProxy.ProxyPassword,
                    ProxyPort = dominatorAccountModel.AccountBaseModel.AccountProxy.ProxyPort,
                    ProxyUsername = dominatorAccountModel.AccountBaseModel.AccountProxy.ProxyUsername
                };
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            _httpHelper.SetRequestParameter(objRequestParametersWeb);
        }
        public void SetRequestHeader(string accept, string refer, string dest)
        {
            var objTdRequestParameters = _httpHelper.GetRequestParameter();

            objTdRequestParameters.Headers.Clear();
            objTdRequestParameters.Accept = accept;
            objTdRequestParameters.UserAgent = "Mozilla/5.0 (Windows NT 6.3; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.198 Safari/537.36";
            objTdRequestParameters.AddHeader("upgrade-insecure-requests", "1");
            objTdRequestParameters.AddHeader("Accept-Language", "en-US,en;q=0.9");
            objTdRequestParameters.KeepAlive = true;
            objTdRequestParameters.ContentType = "application/x-www-form-urlencoded";
            objTdRequestParameters.Referer = refer;
            objTdRequestParameters.AddHeader("Cache-Control", "max-age=0");
            objTdRequestParameters.AddHeader("Host", $"{Domain}");
            objTdRequestParameters.AddHeader("Sec-Fetch-Site", "same-origin");
            objTdRequestParameters.AddHeader("Sec-Fetch-Mode", "navigate");
            objTdRequestParameters.AddHeader("Sec-Fetch-User", "?1");
            objTdRequestParameters.AddHeader("Sec-Fetch-Dest", dest);
        }

        /// <summary>
        ///     Updating account manager details
        /// </summary>
        /// <param name="dominatorAccountModel"></param>
        /// <param name="LogInResponse"></param>
        public async Task UpdatingUserProfileDetails(DominatorAccountModel dominatorAccountModel)
        {
            TdUtility.SaveUserProfileDetails(dominatorAccountModel, _accountContactConfig);
        }


        public void LoginWithBrowserMethod(DominatorAccountModel dominatorAccountModel,
            CancellationToken cancellationToken, VerificationType verficationtype = VerificationType.Email
            , LoginType loginType = LoginType.AutomationLogin)
        {
            try
            {
                twitterAccountSession.AddOrUpdateSession(ref dominatorAccountModel);
                var browserType = IsCheckAccountStatus
                    ? BrowserInstanceType.CheckAccountStatus
                    : BrowserInstanceType.Primary;
                TdAccountsBrowserDetails.GetInstance()
                    .CreateBrowser(dominatorAccountModel, cancellationToken, twitterAccountSession, browserType);
                var name = TdAccountsBrowserDetails.GetBrowserName(dominatorAccountModel, browserType);
                _browserWindow = TdAccountsBrowserDetails.GetInstance().AccountBrowserCollections[name];
                var preLoginPageSource = _browserWindow.GetPageSource();
                if (preLoginPageSource.Contains("If you’re not redirected soon, please"))
                {
                    var Auto = new BrowserAutomationExtension(_browserWindow,
                        dominatorAccountModel.CancellationSource.Token);
                    Auto.LoadAndClick("a", AttributeIdentifierType.Xpath, "/login");
                    _delayService.ThreadSleep(10000);
                    dominatorAccountModel.Cookies = _browserWindow.BrowserCookiesIntoModel().Result;
                    UpdateDominatorAccountModel(dominatorAccountModel);
                    preLoginPageSource = _browserWindow.GetPageSource();
                }


                if (preLoginPageSource.Contains("What’s happening") || preLoginPageSource.Contains("What is happening?!"))
                {
                    AssignBrowserFunction();

                    _twitterFunctions.SetBrowser(dominatorAccountModel, dominatorAccountModel.CancellationSource.Token,
                        browserType);
                    dominatorAccountModel.Cookies = _browserWindow.BrowserCookiesIntoModel().Result;
                    OnBrowserLoginSuccess(dominatorAccountModel);
                    if (dominatorAccountModel.IsRunProcessThroughBrowser)
                        _browserWindow.SaveCookies();
                    else
                        _browserWindow.SaveCookies(false);
                    setCookieFromDatabase(dominatorAccountModel);

                    //to set Cookies for Newui function.
                    // setCookieFromDatabase(dominatorAccountModel);
                }
                else
                {
                    AssignBrowserFunction();
                    _twitterFunctions.SetBrowser(dominatorAccountModel, dominatorAccountModel.CancellationSource.Token,
                        browserType);

                    if (preLoginPageSource.Contains("signout") && preLoginPageSource.Contains("timeline-tweet-box") &&
                        !preLoginPageSource.Contains("password you entered did not match our records") &&
                        !preLoginPageSource.Contains(
                            "We have detected unusual activity on your account. For your security, your account has been locked")
                    )
                    {
                        OnBrowserLoginSuccess(dominatorAccountModel);
                    }
                    else
                    {
                        if (preLoginPageSource.Contains(
                                "We have detected unusual activity on your account. For your security, your account has been locked") ||
                            preLoginPageSource.Contains("Your account has been locked."))
                            GlobusLogHelper.log.Info(Log.LoginFailed,
                                dominatorAccountModel.AccountBaseModel.AccountNetwork,
                                dominatorAccountModel.UserName,
                                "LangKeyDetectedUnusualActivityAccountLocked".FromResourceDictionary());
                        if (preLoginPageSource.Contains("Verify your identity by entering the phone number associated with your Twitter account.") && preLoginPageSource.Contains("Your phone number ends"))
                        {
                            GlobusLogHelper.log.Info(Log.LoginFailed,
                                dominatorAccountModel.AccountBaseModel.AccountNetwork,
                                dominatorAccountModel.UserName,
                                "LangKeyPhoneVerification".FromResourceDictionary());
                            dominatorAccountModel.AccountBaseModel.Status = AccountStatus.PhoneVerification;
                            dominatorAccountModel.IsUserLoggedIn = false;
                            return;
                        }
                        if (preLoginPageSource.Contains("There was unusual login activity on your account. To help keep your account safe, please enter your phone number or username to verify it’s you."))
                        {
                            GlobusLogHelper.log.Info(Log.LoginFailed,
                                dominatorAccountModel.AccountBaseModel.AccountNetwork,
                                dominatorAccountModel.UserName,
                                "There was unusual login activity on your account.please enter your phone number or username to verify it’s you");
                        }
                        if(preLoginPageSource.Contains("In order to protect your account from suspicious activity, we've sent a confirmation code to") && preLoginPageSource.Contains("Check your email"))
                        {
                            GlobusLogHelper.log.Info(Log.LoginFailed,
                               dominatorAccountModel.AccountBaseModel.AccountNetwork,
                               dominatorAccountModel.UserName,
                               "In order to protect your account from suspicious activity, we've sent a confirmation code to your mail");
                            dominatorAccountModel.AccountBaseModel.Status = AccountStatus.EmailVerification;
                            dominatorAccountModel.IsUserLoggedIn = false;
                        }
                        if (preLoginPageSource.Contains("Verify your identity by entering the email address associated with your Twitter account.") && preLoginPageSource.Contains("Help us keep your account safe."))
                        {
                            GlobusLogHelper.log.Info(Log.LoginFailed,
                               dominatorAccountModel.AccountBaseModel.AccountNetwork,
                               dominatorAccountModel.UserName,
                               "Verify your identity by entering the email address associated with your Twitter account.");
                            dominatorAccountModel.AccountBaseModel.Status = AccountStatus.EmailVerification;
                            dominatorAccountModel.IsUserLoggedIn = false;
                        }
                        if (preLoginPageSource.Contains("password you entered did not match our records"))
                            GlobusLogHelper.log.Info(Log.LoginFailed,
                                dominatorAccountModel.AccountBaseModel.AccountNetwork,
                                dominatorAccountModel.UserName,
                                "LangKeyUsernamePasswordNotMatch".FromResourceDictionary());
                        if (preLoginPageSource.Contains("Pass a Google reCAPTCHA challenge") && preLoginPageSource.Contains("We've temporarily limited some of your account features"))
                        {
                            _browserWindow.BrowserAct(ActType.ClickByClass, "Button EdgeButton EdgeButton--primary", delayAfter: 2);
                            Thread.Sleep(10000);
                            preLoginPageSource= _browserWindow.GetPageSource();
                        }

                        if (preLoginPageSource.Contains("div id=\"recaptcha_element\">") &&
                            preLoginPageSource.Contains("id=\"verification_string\"") &&
                            preLoginPageSource.Contains("id=\"continue_button\"") &&
                            preLoginPageSource.Contains("g-recaptcha-response"))
                        {
                            _delayService.ThreadSleep(25000); 
                            GlobusLogHelper.log.Info(Log.LoginFailed,
                                dominatorAccountModel.AccountBaseModel.AccountNetwork,
                                dominatorAccountModel.UserName,
                                "LangKeyCaptchFound".FromResourceDictionary());
                            dominatorAccountModel.AccountBaseModel.Status = AccountStatus.FoundCaptcha;
                            dominatorAccountModel.IsUserLoggedIn = false;
                            preLoginPageSource = SolvingBrowserCaptchaIssue(dominatorAccountModel);
                            if (preLoginPageSource.Contains("\"screen_name\":\""))
                                OnBrowserLoginSuccess(dominatorAccountModel);
                            else
                            {
                                dominatorAccountModel.AccountBaseModel.Status = AccountStatus.Failed;
                                dominatorAccountModel.IsUserLoggedIn = false;
                            }

                        }
                    }
                }

                UpdateDominatorAccountModel(dominatorAccountModel);
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
                TdAccountsBrowserDetails.CloseAllBrowser(dominatorAccountModel);
            }
        }

        private string SolvingBrowserCaptchaIssue(DominatorAccountModel dominatorAccountModel)
        {
            try
            {

                GlobusLogHelper.log.Info(Log.CustomMessage, dominatorAccountModel.AccountBaseModel.AccountNetwork,
                                dominatorAccountModel.AccountBaseModel.UserName, "Login",
                                "Solving captcha please wait...");

                string preLoginPageSource;
                var genericFileManager = InstanceProvider.GetInstance<IGenericFileManager>();
                var imageCaptchaServicesModel =
                    genericFileManager.GetModel<DominatorHouseCore.Models.Config.ImageCaptchaServicesModel>(
                        ConstantVariable.GetImageCaptchaServicesFile()) ?? new DominatorHouseCore.Models.Config.ImageCaptchaServicesModel();
                if (!string.IsNullOrEmpty(imageCaptchaServicesModel.Token))
                {
                    var imageHelper = new ImageTypersHelper(imageCaptchaServicesModel.Token);
                    var urlcap = _browserWindow.CurrentUrl();
                    var captchaId = imageHelper.SubmitSiteKey(urlcap, "6Lc5hC4UAAAAAEx-pIfqjpmg-_-1dLnDwIZ8RToe");
                    var capcthaResult = imageHelper.GetGResponseCaptcha(captchaId);
                    _browserWindow.BrowserAct(ActType.EnterValueById, "g-recaptcha-response", value: capcthaResult, delayAfter: 2);
                    _browserWindow.BrowserAct(ActType.EnterValueById, "verification_string", value: capcthaResult, delayAfter: 2);
                    _browserWindow.BrowserAct(ActType.ClickById, "continue_button", delayAfter: 2);
                    Thread.Sleep(10000);
                    _browserWindow.BrowserAct(ActType.ClickByClass, "Button EdgeButton EdgeButton--primary", delayAfter: 2);
                    Thread.Sleep(10000);
                    preLoginPageSource = _browserWindow.GetPageSource();
                    return preLoginPageSource;
                }
                else
                {
                    GlobusLogHelper.log.Info(Log.LoginFailed, dominatorAccountModel.AccountBaseModel.AccountNetwork,
                            dominatorAccountModel.AccountBaseModel.UserName,
                            "Not found valid userName and password or token for ImageTyperz.");
                    return "";
                }
            }
            catch (Exception ex)
            {


                GlobusLogHelper.log.Info(Log.CustomMessage, dominatorAccountModel.AccountBaseModel.AccountNetwork,
                                dominatorAccountModel.AccountBaseModel.UserName, "Login Failed",
                                ex.Message.ToString());
                if (ex.Message.Contains("AUTHENTICATION_FAILED"))
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, dominatorAccountModel.AccountBaseModel.AccountNetwork,
                                dominatorAccountModel.AccountBaseModel.UserName, "Login Failed",
                                "Please check the token once");
                }
                return "";
            }

        }

        private async Task<LogInResponseHandler> VerficatiomLockedAccount(DominatorAccountModel dominatorAccountModel,
            CancellationToken cancellationToken, string assignmentToken, AccountModel accountModel)
        {
            var tdRequestParameter = (TdRequestParameters)_httpHelper.GetRequestParameter();
            tdRequestParameter.SetupHeaders(Path: TdConstants.AccessUrl,Method:"POST");
            var responseForUriMatrix = _httpHelper
                .GetRequestAsync(TdConstants.UriMatrixUrl, cancellationToken).Result.Response;
            var uriMatrix = TdUtility.getUriMatrix(responseForUriMatrix);
            if (dominatorAccountModel.ExtraParameters.Keys.Contains("UriMatrix"))
                dominatorAccountModel.ExtraParameters.Remove("UriMatrix");
            dominatorAccountModel.ExtraParameters.Add("UriMatrix", uriMatrix);
            tdRequestParameter.SetupHeaders(Path: TdConstants.AccessUrl,Method:"POST");
            tdRequestParameter.Referer = TdConstants.AccessUrl;

            var postData =
                $"authenticity_token={HttpUtility.UrlEncode(accountModel.postAuthenticityToken)}&assignment_token={HttpUtility.UrlEncode(assignmentToken)}&lang=en&flow=&ui_metrics={HttpUtility.UrlEncode(uriMatrix)}";

            return new LogInResponseHandler(
                await _httpHelper.PostRequestAsync(TdConstants.AccessUrl, postData,
                    cancellationToken));
        }

        /// <summary>
        ///     Getting first hit response on login with cookies
        /// </summary>
        /// <param name="dominatorAccountModel"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<bool> CheckLogInWithDatabaseCookieAsync(DominatorAccountModel dominatorAccountModel,
            CancellationToken cancellationToken)
        {
            var IsLoggedIn = false;
            try
            {
                var reqparam = _httpHelper.GetRequestParameter();
                var cto = dominatorAccountModel.Cookies.OfType<Cookie>().FirstOrDefault(x => x.Name == "ct0");
                if(!string.IsNullOrEmpty(cto?.Value))
                    reqparam.Headers["x-csrf-token"] = cto.Value;
                reqparam.Cookies = dominatorAccountModel.Cookies;
                _httpHelper.SetRequestParameter(reqparam);
                var response =await _httpHelper.GetRequestAsync(TdConstants.MainHomeUrl, cancellationToken);
                //var response = await _httpHelper.GetRequestAsync(TdConstants.AccessUrl, cancellationToken);
                if (response.HasError || !response.Response.Contains("\"screen_name\":\"")) return false;
                IsLoggedIn = true;
                dominatorAccountModel.IsUserLoggedIn = true;
                if (!response.HasError && !string.IsNullOrEmpty(response.Response))
                {
                    var jsonResponse = TdUtility.GetProfileDetails(response.Response);
                    if(!string.IsNullOrEmpty(jsonResponse))
                        UpdateProfileDetails(jsonResponse, dominatorAccountModel);
                }   
            }
            catch (Exception)
            {
            }
            return IsLoggedIn;
        }

        private void UpdateProfileDetails(string jsonResponse, DominatorAccountModel dominatorAccountModel)
        {
            var jObject = handler.ParseJsonToJObject(jsonResponse);
            var Details = handler.GetJTokenOfJToken(jObject, "entities", "users", "entities")?.First?.First;
            var userId = handler.GetJTokenValue(Details, "id_str");
            var username = handler.GetJTokenValue(Details, "screen_name");
            var fullName = handler.GetJTokenValue(Details, "name");
            int.TryParse(handler.GetJTokenValue(Details, "followers_count"), out int follower);
            int.TryParse(handler.GetJTokenValue(Details, "friends_count"), out int following);
            var profilePic = handler.GetJTokenValue(Details, "profile_image_url_https");
            dominatorAccountModel.AccountBaseModel.UserFullName = string.IsNullOrEmpty(dominatorAccountModel.AccountBaseModel.UserFullName) ? fullName : dominatorAccountModel.AccountBaseModel.UserFullName;
            dominatorAccountModel.AccountBaseModel.UserId = string.IsNullOrEmpty(dominatorAccountModel.AccountBaseModel.UserId) ? userId : dominatorAccountModel.AccountBaseModel.UserId;
            dominatorAccountModel.AccountBaseModel.ProfileId = string.IsNullOrEmpty(dominatorAccountModel.AccountBaseModel.ProfileId) ? userId : dominatorAccountModel.AccountBaseModel.ProfileId;
            dominatorAccountModel.AccountBaseModel.UserName = string.IsNullOrEmpty(dominatorAccountModel.AccountBaseModel.UserName) ? username : dominatorAccountModel.AccountBaseModel.UserName;
            dominatorAccountModel.AccountBaseModel.ProfilePictureUrl = string.IsNullOrEmpty(dominatorAccountModel.AccountBaseModel.ProfilePictureUrl) ? profilePic : dominatorAccountModel.AccountBaseModel.ProfilePictureUrl;
            dominatorAccountModel.DisplayColumnValue1 = dominatorAccountModel.DisplayColumnValue1 == 0 ? follower : dominatorAccountModel.DisplayColumnValue1;
            dominatorAccountModel.DisplayColumnValue2 = dominatorAccountModel.DisplayColumnValue2 == 0 ? following : dominatorAccountModel.DisplayColumnValue2;
        }

        private void setCookieFromDatabase(DominatorAccountModel dominatorAccountModel)
        {
            try
            {
                var reqParam = _httpHelper.GetRequestParameter();
                if (dominatorAccountModel.Cookies != null && dominatorAccountModel.Cookies.Count != 0
                    || (reqParam != null && reqParam?.Cookies?.Count <= dominatorAccountModel?.Cookies?.Count))
                {
                    reqParam.Cookies = dominatorAccountModel.Cookies;
                    _httpHelper.SetRequestParameter(reqParam);
                }

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public static bool HasUserProfileDetails(DominatorAccountModel dominatorAccountModel)
        {
            var HasProfileDetails = false;
            try
            {
                var accountsFileManager = InstanceProvider.GetInstance<IAccountsFileManager>();
                var existingAccount = accountsFileManager.GetAccountById(dominatorAccountModel.AccountId);
                if (existingAccount == null) return false;

                if (!existingAccount.ExtraParameters.ContainsKey(ModuleExtraDetails.UserProfileDetails.ToString()))
                {
                    HasProfileDetails = true;
                }
                else
                {
                    var dominatorAccUserDetails = accountsFileManager.GetAccountById(dominatorAccountModel.AccountId)
                        .ExtraParameters[ModuleExtraDetails.UserProfileDetails.ToString()];
                    var serializeUserProfileData = dominatorAccUserDetails;
                    var userDetails = JsonConvert.DeserializeObject<UserProfileDetails>(serializeUserProfileData);
                    if (string.IsNullOrEmpty(userDetails?.UserName))
                        HasProfileDetails = true;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return HasProfileDetails;
        }


        private bool CheckAlreadyExist(DominatorAccountModel dominatorAccountModel)
        {
            if (dominatorAccountModel.AccountBaseModel.Status == AccountStatus.NotChecked)
            {
                var getAllAccount = _accountsFileManager.GetAll()
                    .Where(x => x.ExtraParameters.ContainsKey(ModuleExtraDetails.UserProfileDetails.ToString()))
                    .Select(y => y.ExtraParameters[ModuleExtraDetails.UserProfileDetails.ToString()]).ToList();
                var UserProfileDetailsList =
                    getAllAccount.Select(x => JsonConvert.DeserializeObject<UserProfileDetails>(x)).ToList();
                UserProfileDetailsList.RemoveAll(x => x == null);
                var IsExist = UserProfileDetailsList.Any(x =>
                    x.Email == dominatorAccountModel.UserName || x.UserName == dominatorAccountModel.UserName);

                return IsExist;
            }

            return false;
        }

        private static void UpdateGlobalAccountDetails(DominatorAccountModel dominatorAccountModel)
        {
            try
            {
                lock (LockUpdateGlobalAccountDetails)
                {
                    var globalDbOperation =
                        new DbOperations(SocinatorInitialize.GetGlobalDatabase().GetSqlConnection());
                    if (globalDbOperation.Get<DominatorHouseCore.DatabaseHandler.DHTables.AccountDetails>().Any(account => dominatorAccountModel.AccountId == account.AccountId && account.ActivityManager is null))
                        globalDbOperation.RemoveMatch<DominatorHouseCore.DatabaseHandler.DHTables.AccountDetails>(account => dominatorAccountModel.AccountId == account.AccountId && account.ActivityManager == null);
                    globalDbOperation.UpdateAccountDetails(dominatorAccountModel);
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public static void UpdateDominatorAccountModel(DominatorAccountModel dominatorAccountModel,
            int FollowingCount = 0, int TweetCount = 0)
        {
            try
            {
                lock (LockUpdateAccountDetails)
                {
                    var serializeUserProfileData = "";

                    try
                    {
                        var accountsFileManager = InstanceProvider.GetInstance<IAccountsFileManager>();
                        if (accountsFileManager.GetAccountById(dominatorAccountModel.AccountId).ExtraParameters
                            .ContainsKey(ModuleExtraDetails.UserProfileDetails.ToString()))
                            serializeUserProfileData = accountsFileManager
                                .GetAccountById(dominatorAccountModel.AccountId)
                                .ExtraParameters[ModuleExtraDetails.UserProfileDetails.ToString()];
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                    SocinatorAccountBuilder.Instance(dominatorAccountModel.AccountBaseModel.AccountId)
                        .AddOrUpdateDominatorAccountBase(dominatorAccountModel.AccountBaseModel)
                        .AddOrUpdateCookies(dominatorAccountModel.Cookies)
                        .AddOrUpdateDisplayColumn1(dominatorAccountModel.DisplayColumnValue1)
                        .AddOrUpdateDisplayColumn2(dominatorAccountModel.DisplayColumnValue2 + FollowingCount)
                        .AddOrUpdateDisplayColumn3(dominatorAccountModel.DisplayColumnValue3 + TweetCount)
                        .AddOrUpdateExtraParameter(ModuleExtraDetails.ModulePrivateDetails.ToString(),
                            dominatorAccountModel.ModulePrivateDetails)
                        .AddOrUpdateExtraParameter(ModuleExtraDetails.UserProfileDetails.ToString(),
                            serializeUserProfileData ?? "")
                        .SaveToBinFile();
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void AssignBrowserFunction()
        {
            _twitterFunctionFactory.AssignHttpTwitterFunctions();
        }


        private void OnBrowserLoginSuccess(DominatorAccountModel dominatorAccountModel)
        {
            dominatorAccountModel.AccountBaseModel.Status = AccountStatus.Success;
            dominatorAccountModel.IsUserLoggedIn = true;
            twitterAccountSession.AddOrUpdateSession(ref dominatorAccountModel,true);
        }
    }
}