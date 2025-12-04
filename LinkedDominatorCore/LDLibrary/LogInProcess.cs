using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using CefSharp;
using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.BusinessLogic.ActivitiesWorkflow;
using DominatorHouseCore.DatabaseHandler.DHTables;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.Config;
using DominatorHouseCore.Request;
using DominatorHouseCore.Utility;
using EmbeddedBrowser;
using LinkedDominatorCore.Factories;
using LinkedDominatorCore.Interfaces;
using LinkedDominatorCore.LDModel;
using LinkedDominatorCore.LDUtility;
using LinkedDominatorCore.Request;
using LinkedDominatorCore.Response;
using LinkedDominatorCore.Utility;
using Newtonsoft.Json;

namespace LinkedDominatorCore.LDLibrary
{
    public interface ILdLogInProcess : ILoginProcessAsync
    {
        // Task LoginWithWeb(DominatorAccountModel dominatorAccountModel);
        //  bool InitializeSalesNavRequestParameter(DominatorAccountModel dominatorAccountModel);

        // Task<bool> SolveCaptcha(LoginResponseHandler loginResponseHandler, LdFunctions ldFunctions);

        // string GetSolvedCaptcha(LoginResponseHandler loginResponseHandler, LdFunctions ldFunctions);
        // LdFunctions GetLdFunctions();
        bool IsBrowser { get; set; }
        bool IsCheckAccountStatus { get; set; }


        void OnSuccess(DominatorAccountModel dominatorAccountModel, bool isSave = false);
        //  bool CheckLogin(DominatorAccountModel dominatorAccountModel);

        Task LoginWithMobileDevice(DominatorAccountModel dominatorAccountModel, CancellationToken cancellationToken);
        void AssignFunctions(DominatorAccountModel dominatorAccountModel, IAccountScopeFactory _accountScopeFactory);
        void AccountBaseModelStatus(DominatorAccountModel dominatorAccountModel,LoginResponseHandler loginResponseHandler,bool NeedToSave=true,bool ShowLogMessage=true);
    }

    public interface ILoginSalesNavigator
    {
    }

    // this class is used to pass variable in constructor of LoginProcess
    // since we need to pass two variables view profile and service locator only takes 
    // registered class
    public class LoginSalesNavigator : ILoginSalesNavigator
    {
        public ActivityType ActivityType { get; set; }
        public bool IsViewProfileUsingEmbeddedBrowser { get; set; }
        public string QueryType { get; set; }
    }


    public class LogInProcess : ILdLogInProcess
    {
        public static readonly object LockGlobalDb = new object();
        private readonly ILdHttpHelper _httpHelper;
        private readonly ILdFunctionFactory _ldFunctionFactory;
        private readonly ILDAccountSessionManager accountSessionManager;
        private ILdFunctions _ldFunctions;

        public LogInProcess(DominatorAccountModel dominatorAccountModel, CancellationToken cancellationToken,
            ILoginSalesNavigator loginSalesNavigator,
            ILdHttpHelper ldHttpHelper, ILdFunctionFactory ldFunctionFactory,ILDAccountSessionManager lDAccountSession)
        {
            var temp = (LoginSalesNavigator) loginSalesNavigator;
            //var normalAccounts=(LogInProcess)aaaa;
            ActivityType = temp.ActivityType;
            if (DominatorAccountModel == null ||
                string.IsNullOrEmpty(DominatorAccountModel?.AccountBaseModel?.AccountId))
                DominatorAccountModel = dominatorAccountModel;
            _httpHelper = ldHttpHelper;
            _ldFunctionFactory = ldFunctionFactory;
            _ldFunctions = ldFunctionFactory.LdFunctions;
            IsViewProfileUsingEmbeddedBrowser = temp.IsViewProfileUsingEmbeddedBrowser;
            if (ActivityType.Equals(ActivityType.ExportConnection) ||
                ActivityType.Equals(ActivityType.SalesNavigatorCompanyScraper) ||
                ActivityType.Equals(ActivityType.SalesNavigatorUserScraper) ||
                ActivityType.Equals(ActivityType.UserScraper))
                LoginWithAlternativeMethod(dominatorAccountModel, cancellationToken);
            IsBrowser = ldFunctionFactory.LdFunctions.IsBrowser;
            accountSessionManager = lDAccountSession;
        }
        // Get a handle to an application window.
        //[DllImport("USER32.DLL", CharSet = CharSet.Unicode)]
        //public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        //// Activate an application window.
        //[DllImport("USER32.DLL")]
        //public static extern bool SetForegroundWindow(IntPtr hWnd);

        //[DllImport("user32.dll")]
        //static extern IntPtr GetForegroundWindow();
        //[DllImport("user32.dll")]
        //static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);
        //[DllImport("user32.dll")]
        //public static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

        public DominatorAccountModel DominatorAccountModel { get; set; }
        public ActivityType ActivityType { get; set; }
        public bool IsConnectionRequestSalesSearchUrl { get; set; }
        public bool IsViewProfileUsingEmbeddedBrowser { get; set; }
        private BrowserWindow _browserWindow { get; set; }

        public bool IsBrowser { get; set; }
        public bool IsCheckAccountStatus { get; set; }

        public void AssignFunctions(DominatorAccountModel dominatorAccountModel,
            IAccountScopeFactory _accountScopeFactory)
        {
            _ldFunctionFactory.AssignFunction(dominatorAccountModel);
            _ldFunctions = _ldFunctionFactory.LdFunctions;
        }


        public bool CheckLogin(DominatorAccountModel dominatorAccountModel, CancellationToken cancellationToken)
        {
            return CheckLoginAsync(dominatorAccountModel, cancellationToken).Result;
        }
#pragma warning disable 1998
        public async Task<bool> CheckLoginAsync(DominatorAccountModel dominatorAccountModel,
            CancellationToken cancellationToken, bool displayLoginMsg = false, LoginType loginType = LoginType.AutomationLogin)
#pragma warning disable 1998
        {
            try
            {
                accountSessionManager.AddOrUpdateSession(ref dominatorAccountModel);
                _ldFunctions.SetCookieAndProxy(dominatorAccountModel, _httpHelper);
                var preLoginPageSource = _ldFunctions.PreLoginResponse(cancellationToken, false);
                CheckLoggedInFromPageResponse(dominatorAccountModel, preLoginPageSource);
                return dominatorAccountModel.IsUserLoggedIn;
            }
            catch (Exception ex)
            {
                dominatorAccountModel.IsUserLoggedIn = false;
                ex.DebugLog();
                return false;
            }
        }

        public void CheckLoggedInFromPageResponse(DominatorAccountModel dominatorAccountModel,
            string preLoginPageSource)
        {
            if (ProxyFailedStatus(dominatorAccountModel, preLoginPageSource))
            {
                dominatorAccountModel.IsUserLoggedIn = false;
            }

            else if (!string.IsNullOrEmpty(preLoginPageSource))
            {
                LoginResponseHandler loginResponseHandler = null;
                var jsonElements = new LdJsonElements { ChallengeUrl = "" };
                if (preLoginPageSource.Contains("Your account has been restricted")
                    || preLoginPageSource.Contains("Your LinkedIn account has been temporarily restricted"))
                {
                    jsonElements.LoginResult = "LOGIN_RESTRICTED";
                }
                else if (preLoginPageSource.Contains("Let's do a quick verification") || preLoginPageSource.Contains("Resend verification code"))
                {
                    jsonElements.LoginResult = "CHALLENGE";

                }
                else if (preLoginPageSource.Contains("sign-in-form__submit-btn") ? false :!preLoginPageSource.Contains("LinkedIn: Log In or Sign Up") &&
                            !preLoginPageSource.Contains("Be great at what you do") &&
                            !string.IsNullOrEmpty(preLoginPageSource) &&
                            !preLoginPageSource.Contains("By clicking Join now, you agree to the LinkedIn") &&
                            !string.IsNullOrEmpty(preLoginPageSource) &&
                            !preLoginPageSource.Contains("class=\"login submit-button\"") && !preLoginPageSource.Contains("Sign in"))

                    {
                        if (!preLoginPageSource.Contains("Sign in to LinkedIn") || !preLoginPageSource.Contains("Sign in"))
                        {
                            if (IsBrowser)
                                if (dominatorAccountModel.CookieHelperList.Count <
                                    _browserWindow.BrowserCookiesIntoModel().Result.Count)
                                    _browserWindow.SaveCookies();
                            dominatorAccountModel.IsUserLoggedIn = true;
                            dominatorAccountModel.AccountBaseModel.Status = AccountStatus.Success;
                            jsonElements.LoginResult = "PASS";
                        }
                    }
                    else
                    {
                        dominatorAccountModel.IsUserLoggedIn = false;
                        jsonElements.LoginResult = "";
                    }
                
                
                var jsonBody = new LdRequestParameters().GenerateStringBody(jsonElements);
                loginResponseHandler = new LoginResponseHandler(new ResponseParameter {Response = jsonBody});
                AccountBaseModelStatus(dominatorAccountModel, loginResponseHandler,dominatorAccountModel.IsUserLoggedIn);
            }
        }


        public async Task LoginWithMobileDevice(DominatorAccountModel dominatorAccountModel,
            CancellationToken cancellationToken)
        {
            try
            {
                accountSessionManager.AddOrUpdateSession(ref dominatorAccountModel);
                DominatorAccountModel = dominatorAccountModel;
                if (dominatorAccountModel.Cookies.Count != 0 && (DominatorAccountModel.IsUserLoggedIn =
                        CheckLogin(dominatorAccountModel, cancellationToken)))
                {
                    dominatorAccountModel.IsloggedinWithPhone = true;
                    return;
                }
                var li_rm=DominatorAccountModel.Cookies.Cast<System.Net.Cookie>().FirstOrDefault(x=>x.Name=="li_rm");
                DominatorAccountModel.Cookies = new CookieCollection();
                _httpHelper.GetRequestParameter().Cookies = new CookieCollection();
                _ldFunctions.SetCookieAndProxy(dominatorAccountModel, _httpHelper);
                var preLoginResponse = _ldFunctions.PreLoginResponse(cancellationToken, false);
                if(string.IsNullOrEmpty(preLoginResponse))
                    preLoginResponse = _ldFunctions.PreLoginResponse(cancellationToken, false);
                if (ProxyFailedStatus(dominatorAccountModel, preLoginResponse))
                    return;
                var mobileLoginResponseHandler = _ldFunctions.Login(cancellationToken,string.Empty,string.Empty,li_rm?.Value).Result;
                AccountBaseModelStatus(dominatorAccountModel, mobileLoginResponseHandler,true,true);
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (AggregateException ae)
            {
                foreach (var e in ae.InnerExceptions)
                    if (e is TaskCanceledException || e is OperationCanceledException)
                        e.DebugLog("Cancellation requested before task completion!");
                    else
                        e.DebugLog(e.StackTrace + e.Message);
            }
            catch (Exception ex)
            {
                OnFailed(dominatorAccountModel);
                ex.DebugLog();
            }
        }

        public void AccountBaseModelStatus(DominatorAccountModel dominatorAccountModel,
            LoginResponseHandler loginResponseHandler,bool NeedToSave=true, bool ShowLogMessage = true)
        {
            if (loginResponseHandler.Success)
            {
                switch (loginResponseHandler.LoginResult)
                {
                    case "PASS":
                        OnSuccess(dominatorAccountModel,NeedToSave);
                        break;

                    case "BAD_EMAIL":
                    case "BAD_PASSWORD":
                        dominatorAccountModel.IsUserLoggedIn = false;
                        dominatorAccountModel.AccountBaseModel.Status = AccountStatus.InvalidCredentials;
                        dominatorAccountModel.Cookies = new CookieCollection();
                        GlobusLogHelper.log.Info(Log.LoginFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName,
                            Application.Current.FindResource("LangKeyInvalidCredentials"));
                        break;
                    case "CHALLENGE":
                    {
                        var redirectChallenge = dominatorAccountModel.IsRunProcessThroughBrowser ? 
                                _ldFunctions.BrowserWindow.GetPageSource() :
                                _ldFunctions.RedirectChallenge(dominatorAccountModel,loginResponseHandler).Result;
                        if (redirectChallenge.Contains("request-password-reset"))
                            GlobusLogHelper.log.Debug(
                                $"request-password-reset for {dominatorAccountModel.AccountBaseModel.UserName}.");
                        if (redirectChallenge.Contains("form__input--text input_verification_pin"))
                        {
                                dominatorAccountModel.IsUserLoggedIn = false;
                                dominatorAccountModel.AccountBaseModel.Status = AccountStatus.NeedsVerification;
                                if (string.IsNullOrEmpty(dominatorAccountModel.MailCredentials.Hostname) && string.IsNullOrEmpty(dominatorAccountModel.MailCredentials.Username) && string.IsNullOrEmpty(dominatorAccountModel.MailCredentials.Password)
                                    && dominatorAccountModel.MailCredentials.Port == null)
                                {
                                    GlobusLogHelper.log.Info(Log.CustomMessage,
                               dominatorAccountModel.AccountBaseModel.AccountNetwork,
                               dominatorAccountModel.AccountBaseModel.UserName, "Auto Email Verification",
                               "Please Fill the Details of Auto verification in Account Details section");
                                    break;
                                }

                            GlobusLogHelper.log.Info(Log.CustomMessage,
                                dominatorAccountModel.AccountBaseModel.AccountNetwork,
                                dominatorAccountModel.AccountBaseModel.UserName, "Auto Email Verification",
                                "Please wait,While verifying email");
                                _ldFunctions.ReadVerificationCodeFromEmail(dominatorAccountModel);
                                var pin = dominatorAccountModel.VarificationCode;
                                if (!dominatorAccountModel.IsRunProcessThroughBrowser)
                                {
                                    string pageInstance,
                                        resendUrl,
                                        challengeId,
                                        language,
                                        displayTime,
                                        challengeSource,
                                        requestSubmissionId,
                                        challengeType,
                                        challengeData,
                                        challengeDetails,
                                        failureRedirectUri,
                                        csrfToken,
                                        consumerLogin;
                                    GetDataofPost(redirectChallenge, out pageInstance, out resendUrl, out challengeId,
                                        out language, out displayTime, out challengeSource, out requestSubmissionId,
                                        out challengeType, out challengeData, out challengeDetails, out failureRedirectUri,
                                        out csrfToken, out consumerLogin);
                                    if (VerifyEmailCode(dominatorAccountModel, loginResponseHandler, pageInstance, resendUrl,
                                challengeId, language, displayTime, challengeSource, requestSubmissionId, challengeType,
                                challengeData, challengeDetails, failureRedirectUri, csrfToken, consumerLogin, pin))
                                        break;
                                }
                                else
                                {
                                    _ldFunctions.BrowserMailverfication(pin);
                                    redirectChallenge = _ldFunctions.BrowserWindow.GetPageSource();
                                    CheckLoggedInFromPageResponse(dominatorAccountModel, redirectChallenge);
                                    break;
                                }
                            
                            
                        }

                        if (redirectChallenge.Contains("do a quick security check") &&
                            redirectChallenge.Contains("captcha"))
                        {
                                dominatorAccountModel.IsUserLoggedIn = false;
                                dominatorAccountModel.AccountBaseModel.Status = AccountStatus.FoundCaptcha;
                                string csrfToken,
                                captchaSiteKey,
                                challengeId,
                                language,
                                displayTime,
                                challengeType,
                                challengeSource,
                                requestSubmissionId,
                                challengeData,
                                pageInstance,
                                challengeDetails,
                                failureRedirectUri,
                                signingvalue,
                                joinNowLink,
                                cousmerLogin;
                            GetDataForSolvingCaptcha(redirectChallenge, out csrfToken, out captchaSiteKey,
                                out challengeId, out language, out displayTime, out challengeType,
                                out challengeSource, out requestSubmissionId, out challengeData, out pageInstance,
                                out challengeDetails, out failureRedirectUri, out signingvalue, out joinNowLink,
                                out cousmerLogin);
                            var genericFileManager = InstanceProvider.GetInstance<IGenericFileManager>();
                            var imageCaptchaServicesModel =
                                genericFileManager.GetModel<ImageCaptchaServicesModel>(ConstantVariable
                                    .GetImageCaptchaServicesFile()) ?? new ImageCaptchaServicesModel();
                                if ((string.IsNullOrEmpty(imageCaptchaServicesModel.UserName) && string.IsNullOrEmpty(imageCaptchaServicesModel.Password)) && string.IsNullOrEmpty(imageCaptchaServicesModel.Token))
                                {
                                    GlobusLogHelper.log.Info(Log.CustomMessage,
                                    dominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    dominatorAccountModel.AccountBaseModel.UserName, "ImageCaptchaSolver",
                                    "Please Fill the Details of ImageCaptcha Token or Username and Password in Third Party Service => Image Captcha Service");
                                    break;
                                }
                            var imageHelper = new ImageTypersHelper(imageCaptchaServicesModel.Token);
                            var captchaId = imageHelper.SubmitSiteKey(loginResponseHandler.ChallengeUrl,
                                captchaSiteKey);
                            GlobusLogHelper.log.Info(Log.CustomMessage,
                                dominatorAccountModel.AccountBaseModel.AccountNetwork,
                                dominatorAccountModel.AccountBaseModel.UserName, "Image Captcha",
                                "Please wait, While solving captcha");
                            var captchaRes = imageHelper.GetGResponseCaptcha(captchaId);
                            var CaptchaPotdata =
                                $"csrfToken={csrfToken}&captchaSiteKey={captchaSiteKey}&challengeId={challengeId}&language={language}&displayTime={displayTime}&challengeType={challengeType}&challengeSource={challengeSource}&requestSubmissionId={requestSubmissionId}&captchaUserResponseToken={captchaRes}&challengeData={challengeData}&pageInstance={pageInstance}&challengeDetails={challengeDetails}&failureRedirectUri={failureRedirectUri}&signInLink={signingvalue}&joinNowLink={joinNowLink}&_s={cousmerLogin}";
                            _ldFunctions.SetWebRequestParametersforCaptcha(loginResponseHandler.ChallengeUrl);
                            _ldFunctions.GetInnerHttpHelper().Request.AllowAutoRedirect = false;
                            _ldFunctions.GetInnerHttpHelper().PostRequest("https://www.linkedin.com/checkpoint/challenge/verify",CaptchaPotdata);
                            var location = _ldFunctions.GetInnerHttpHelper().Response.Headers["Location"];
                            var url = $"https://www.linkedin.com{location}";
                            _ldFunctions.GetInnerHttpHelper().GetRequest(url);
                            OnSuccess(dominatorAccountModel,NeedToSave);
                        }
                        else
                        {
                            dominatorAccountModel.IsUserLoggedIn = false;
                            dominatorAccountModel.AccountBaseModel.Status = AccountStatus.NeedsVerification;
                            dominatorAccountModel.Cookies = new CookieCollection();
                                if(ShowLogMessage)
                                    GlobusLogHelper.log.Info(Log.LoginFailed,DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                        DominatorAccountModel.AccountBaseModel.UserName,Application.Current.FindResource("LangKeyFailed"));
                        }

                        break;
                    }

                    case "LOGIN_RESTRICTED":
                        dominatorAccountModel.IsUserLoggedIn = false;
                        dominatorAccountModel.AccountBaseModel.Status = AccountStatus.TemporarilyBlocked;
                        dominatorAccountModel.Cookies = new CookieCollection();
                        GlobusLogHelper.log.Info(Log.LoginFailed, dominatorAccountModel.AccountBaseModel.AccountNetwork,
                            dominatorAccountModel.AccountBaseModel.UserName,
                            "LangKeyTemporarilyBlocked".FromResourceDictionary());
                        break;
                }

                SocinatorAccountBuilder.Instance(dominatorAccountModel.AccountBaseModel.AccountId)
                    .AddOrUpdateDominatorAccountBase(dominatorAccountModel.AccountBaseModel)
                    .AddOrUpdateCookies(dominatorAccountModel.Cookies)
                    .AddOrUpdateExtraParameter(dominatorAccountModel.ExtraParameters)
                    .SaveToBinFile();
            }
            else
            {
                OnFailed(dominatorAccountModel, true);
            }
        }

        private static void GetDataForSolvingCaptcha(string redirectChallenge, out string csrfToken,
            out string captchaSiteKey, out string challengeId, out string language, out string displayTime,
            out string challengeType, out string challengeSource, out string requestSubmissionId,
            out string challengeData, out string pageInstance, out string challengeDetails,
            out string failureRedirectUri, out string signingvalue, out string joinNowLink, out string cousmerLogin)
        {
            csrfToken = Uri.EscapeDataString(
                HtmlParseUtility.GetAttributeValueFromTagName(redirectChallenge, "input", "name", "csrfToken",
                    "value"));
            captchaSiteKey =
                HtmlParseUtility.GetAttributeValueFromTagName(redirectChallenge, "input", "name", "captchaSiteKey",
                    "value");
            challengeId =
                HtmlParseUtility.GetAttributeValueFromTagName(redirectChallenge, "input", "name", "challengeId",
                    "value");
            language = HtmlParseUtility.GetAttributeValueFromTagName(redirectChallenge, "input", "name", "language",
                "value");
            displayTime =
                HtmlParseUtility.GetAttributeValueFromTagName(redirectChallenge, "input", "name", "displayTime",
                    "value");
            challengeType =
                HtmlParseUtility.GetAttributeValueFromTagName(redirectChallenge, "input", "name", "challengeType",
                    "value");
            challengeSource =
                HtmlParseUtility.GetAttributeValueFromTagName(redirectChallenge, "input", "name", "challengeSource",
                    "value");
            requestSubmissionId = HtmlParseUtility.GetAttributeValueFromTagName(redirectChallenge, "input", "name",
                "requestSubmissionId", "value");
            challengeData =
                HtmlParseUtility.GetAttributeValueFromTagName(redirectChallenge, "input", "name", "challengeData",
                    "value");
            pageInstance =
                Uri.EscapeDataString(HtmlParseUtility.GetAttributeValueFromTagName(redirectChallenge, "input", "name",
                    "pageInstance", "value"));
            challengeDetails =
                HtmlParseUtility.GetAttributeValueFromTagName(redirectChallenge, "input", "name", "challengeDetails",
                    "value");
            failureRedirectUri = HtmlParseUtility.GetAttributeValueFromTagName(redirectChallenge, "input", "name",
                "failureRedirectUri", "value");
            signingvalue =
                WebUtility.HtmlDecode(HtmlParseUtility.GetAttributeValueFromTagName(redirectChallenge, "input", "name",
                    "signInLink", "value"));
            signingvalue = Uri.EscapeDataString(signingvalue);
            joinNowLink =
                Uri.EscapeDataString(HtmlParseUtility.GetAttributeValueFromTagName(redirectChallenge, "input", "name",
                    "joinNowLink", "value"));
            cousmerLogin =
                HtmlParseUtility.GetAttributeValueFromTagName(redirectChallenge, "input", "name", "_s", "value");
        }

        private bool VerifyEmailCode(DominatorAccountModel dominatorAccountModel,
            LoginResponseHandler loginResponseHandler, string pageInstance, string resendUrl, string challengeId,
            string language, string displayTime, string challengeSource, string requestSubmissionId,
            string challengeType, string challengeData, string challengeDetails, string failureRedirectUri,
            string csrfToken, string consumerLogin, string pin)
        {
            var postdata =
                $"csrfToken={csrfToken}&pageInstance={pageInstance}&resendUrl={resendUrl}&challengeId={challengeId}&language={language}&displayTime={displayTime}&challengeSource={challengeSource}&requestSubmissionId={requestSubmissionId}&challengeType={challengeType}&challengeData={challengeData}&challengeDetails={challengeDetails}&failureRedirectUri={failureRedirectUri}&_s={consumerLogin}&pin={pin}";
            _ldFunctions.SetWebRequestParametersforCaptcha(loginResponseHandler.ChallengeUrl);
            _ldFunctions.GetInnerHttpHelper().Request.AllowAutoRedirect = false;
            _ldFunctions.GetInnerHttpHelper()
                .PostRequest("https://www.linkedin.com/checkpoint/challenge/verify", postdata);
            if (CheckLogin(dominatorAccountModel, dominatorAccountModel.Token))
                return true;
            return false;
        }

        private static void GetDataofPost(string redirectChallenge, out string pageInstance, out string resendUrl,
            out string challengeId, out string language, out string displayTime, out string challengeSource,
            out string requestSubmissionId, out string challengeType, out string challengeData,
            out string challengeDetails, out string failureRedirectUri, out string csrfToken, out string consumerLogin)
        {
            csrfToken = Uri.EscapeDataString(
                HtmlParseUtility.GetAttributeValueFromTagName(redirectChallenge, "input", "name", "csrfToken",
                    "value"));
            pageInstance =
                Uri.EscapeDataString(HtmlParseUtility.GetAttributeValueFromTagName(redirectChallenge, "input", "name",
                    "pageInstance", "value"));
            resendUrl = Uri.EscapeDataString(
                HtmlParseUtility.GetAttributeValueFromTagName(redirectChallenge, "input", "name", "resendUrl",
                    "value"));
            challengeId =
                HtmlParseUtility.GetAttributeValueFromTagName(redirectChallenge, "input", "name", "challengeId",
                    "value");
            language = HtmlParseUtility.GetAttributeValueFromTagName(redirectChallenge, "input", "name", "language",
                "value");
            displayTime =
                HtmlParseUtility.GetAttributeValueFromTagName(redirectChallenge, "input", "name", "displayTime",
                    "value");
            challengeSource =
                HtmlParseUtility.GetAttributeValueFromTagName(redirectChallenge, "input", "name", "challengeSource",
                    "value");
            requestSubmissionId = HtmlParseUtility.GetAttributeValueFromTagName(redirectChallenge, "input", "name",
                "requestSubmissionId", "value");
            challengeType =
                HtmlParseUtility.GetAttributeValueFromTagName(redirectChallenge, "input", "name", "challengeType",
                    "value");
            challengeData =
                HtmlParseUtility.GetAttributeValueFromTagName(redirectChallenge, "input", "name", "challengeData",
                    "value");
            challengeDetails =
                HtmlParseUtility.GetAttributeValueFromTagName(redirectChallenge, "input", "name", "challengeDetails",
                    "value");
            failureRedirectUri = HtmlParseUtility.GetAttributeValueFromTagName(redirectChallenge, "input", "name",
                "failureRedirectUri", "value");
            consumerLogin =
                HtmlParseUtility.GetAttributeValueFromTagName(redirectChallenge, "input", "name", "_s", "value");
        }

        public bool ProxyFailedStatus(DominatorAccountModel dominatorAccountModel, string preLoginResponse)
        {
            // this problem occur when our local ip is banned
            if (string.IsNullOrEmpty(preLoginResponse) &&
                string.IsNullOrEmpty(dominatorAccountModel.AccountBaseModel.AccountProxy.ProxyIp))
                return OnFailed(dominatorAccountModel);

            if (!string.IsNullOrEmpty(dominatorAccountModel.AccountBaseModel.AccountProxy.ProxyIp) &&
                string.IsNullOrEmpty(preLoginResponse))
                return ProxyNotWorking(dominatorAccountModel);

            if (!string.IsNullOrEmpty(preLoginResponse) &&
                (preLoginResponse.Contains(
                     "<p>The following error was encountered while trying to retrieve the URL: <a href=\"https://www.linkedin.com/*\">https://www.linkedin.com/*</a></p>") ||
                 preLoginResponse.Contains("Request denied") ||
                 preLoginResponse.Contains("we are unable to serve your request at this time due to unusual traffic") ||
                 preLoginResponse.Contains("Please ensure this IP is set in your Authorized IP list:") ||
                 preLoginResponse == "407\n"))
                return ProxyNotWorking(dominatorAccountModel);

            return false;
        }

        private bool ProxyNotWorking(DominatorAccountModel dominatorAccountModel)
        {
            GlobusLogHelper.log.Info(Log.LoginFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                DominatorAccountModel.AccountBaseModel.UserName,
                Application.Current.FindResource("LangKeyProxyNotWorking"));
            dominatorAccountModel.IsUserLoggedIn = false;
            dominatorAccountModel.AccountBaseModel.Status = AccountStatus.ProxyNotWorking;
            SocinatorAccountBuilder.Instance(DominatorAccountModel.AccountBaseModel.AccountId)
                .AddOrUpdateDominatorAccountBase(DominatorAccountModel.AccountBaseModel)
                .AddOrUpdateCookies(DominatorAccountModel.Cookies)
                .AddOrUpdateExtraParameter(DominatorAccountModel.ExtraParameters)
                .SaveToBinFile();
            return true;
        }

        private bool OnFailed(DominatorAccountModel dominatorAccountModel, bool isSaveExtraParameters = false,
            string message = "")
        {
            dominatorAccountModel.IsUserLoggedIn = false;
            dominatorAccountModel.AccountBaseModel.Status = AccountStatus.Failed;
            dominatorAccountModel.Cookies = new CookieCollection();
            GlobusLogHelper.log.Info(Log.LoginFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                DominatorAccountModel.AccountBaseModel.UserName, message);
            dominatorAccountModel.Token.ThrowIfCancellationRequested();
            var saveToBin = SocinatorAccountBuilder.Instance(DominatorAccountModel.AccountBaseModel.AccountId);
            saveToBin.AddOrUpdateDominatorAccountBase(DominatorAccountModel.AccountBaseModel)
                .AddOrUpdateCookies(DominatorAccountModel.Cookies)
                .SaveToBinFile();

            if (isSaveExtraParameters)
                saveToBin.AddOrUpdateExtraParameter(DominatorAccountModel.ExtraParameters)
                    .SaveToBinFile();
            return true;
        }

        /// <summary>
        ///     LogIn with Login With Web
        /// </summary>
        /// <param name="dominatorAccountModel"></param>

        // ReSharper disable once UnusedMember.Global
        public async Task LoginWithWeb(DominatorAccountModel dominatorAccountModel, CancellationToken cancellationToken,
            bool isSaveToDb = true)
        {
            try
            {
                var loginResponse = string.Empty;
                var cookieCollection = new CookieCollection();
                //AccountModel.Cookies = new CookieCollection();
                // InitializeRequestParameter(dominatorAccountModel);
                // here we logging for get cookies of web browser so that we can use it for non english language search url
                if (isSaveToDb)
                {
                    if (dominatorAccountModel.Cookies.Count != 0)
                    {
                        CheckLogin(DominatorAccountModel, cancellationToken);
                        if (DominatorAccountModel.IsUserLoggedIn)
                        {
                            dominatorAccountModel.IsloggedinWithPhone = false;
                            return;
                        }
                    }
                }
                else if (dominatorAccountModel.Cookies.Count != 0)
                {
                    cookieCollection = dominatorAccountModel.Cookies;
                }

                try
                {
                    loginResponse = await _ldFunctions.WebLogin();
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                if (!loginResponse.Contains("LinkedIn: Log In or Sign Up") &&
                    !loginResponse.Contains("Be great at what you do") &&
                    !loginResponse.Contains("By clicking Join now, you agree to the LinkedIn") &&
                    !loginResponse.Contains("Your LinkedIn account has been temporarily restricted") &&
                    !string.IsNullOrEmpty(loginResponse))
                    if (!loginResponse.Contains("Sign in to LinkedIn") || !loginResponse.Contains("Sign in"))
                    {
                        if (isSaveToDb)
                        {
                            dominatorAccountModel.IsUserLoggedIn = true;
                            dominatorAccountModel.IsloggedinWithPhone = false;
                            dominatorAccountModel.AccountBaseModel.Status = AccountStatus.Success;
                            dominatorAccountModel.Cookies =
                                _ldFunctions.GetInnerHttpHelper().GetRequestParameter().Cookies;
                            dominatorAccountModel.Token.ThrowIfCancellationRequested();
                            SocinatorAccountBuilder.Instance(DominatorAccountModel.AccountBaseModel.AccountId)
                                .AddOrUpdateDominatorAccountBase(DominatorAccountModel.AccountBaseModel)
                                .AddOrUpdateCookies(DominatorAccountModel.Cookies)
                                .SaveToBinFile();
                        }
                        else
                        {
                            try
                            {
                                var serializeCookie = JsonConvert.SerializeObject(DominatorAccountModel.Cookies);
                                var mobSerializeCookie = JsonConvert.SerializeObject(cookieCollection);
                                SocinatorAccountBuilder.Instance(DominatorAccountModel.AccountBaseModel.AccountId)
                                    .AddOrUpdateExtraParameter("WebCookies", serializeCookie)
                                    .AddOrUpdateExtraParameter("MobileCookies", mobSerializeCookie)
                                    .SaveToBinFile();
                            }
                            catch (Exception exception)
                            {
                                exception.DebugLog();
                            }
                        }
                    }
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (AggregateException ae)
            {
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
        }

        /// <summary>
        ///     Login With AlternativeMethod
        /// </summary>
        /// <param name="dominatorAccountModel"></param>
        public void LoginWithAlternativeMethod(DominatorAccountModel dominatorAccountModel,
            CancellationToken cancellationToken)
        {
            try
            {
                LoginWithAlternativeMethodAsync(dominatorAccountModel, cancellationToken).Wait();
            }
            catch (OperationCanceledException)
            {
                LDAccountsBrowserDetails.CloseBrowser(DominatorAccountModel);
                throw new OperationCanceledException("Operation Cancelled!");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public async Task LoginWithAlternativeMethodAsync(DominatorAccountModel dominatorAccountModel,
            CancellationToken cancellationToken)
        {
            try
            {
                // Thread.Sleep(15000);

                #region login region
                accountSessionManager.AddOrUpdateSession(ref dominatorAccountModel);
                DominatorAccountModel= dominatorAccountModel;
                var isSuccess = CheckLoginAsync(dominatorAccountModel, dominatorAccountModel.Token).Result;


                // here we have to open embedded browser to view profile
                // is view from salesNav and setting is logged in
                if (!IsViewProfileUsingEmbeddedBrowser && InitializeSalesNavRequestParameter(dominatorAccountModel)
                                                       && (DominatorAccountModel.IsUserLoggedIn = true))
                    return;

                #endregion

                if (isSuccess)
                    await EmbeddedBrowserLoginProcess(dominatorAccountModel, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                CloseBrowser();
                throw new OperationCanceledException();
            }
            catch (AggregateException ae)
            {
                CloseBrowser();
                foreach (var e in ae.InnerExceptions)
                    if (e is TaskCanceledException || e is OperationCanceledException)
                        e.DebugLog("Cancellation requested before task completion!");
                    else
                        e.DebugLog(e.StackTrace + e.Message);
            }
            catch (Exception ex)
            {
                CloseBrowser();
                ex.DebugLog();
            }
        }

        private async Task EmbeddedBrowserLoginProcess(DominatorAccountModel dominatorAccountModel,
            CancellationToken cancellationToken)
        {
            try
            {
                // keep the delay between s/w login and browser
                Thread.Sleep(new Random().Next(5000, 10000));
                accountSessionManager.AddOrUpdateSession(ref dominatorAccountModel);
                LDAccountsBrowserDetails.GetInstance()
                    .StartBrowserLogin(dominatorAccountModel, cancellationToken, true,BrowserInstanceType.Primary,accountSessionManager);

                // until loaded main login page
                // if (dominatorAccountModel.IsUserLoggedIn)

                _browserWindow = LDAccountsBrowserDetails.GetInstance()
                    .AccountBrowserCollections[dominatorAccountModel.UserName];

                var apiAssist = new ApiAssist();
                var ldDataHelper = LdDataHelper.GetInstance;
                if (ActivityType == ActivityType.SalesNavigatorUserScraper ||
                    ActivityType == ActivityType.SalesNavigatorCompanyScraper || IsConnectionRequestSalesSearchUrl)
                    for (var index = 0; index < 3; index++)
                    {
                        if (!CheckIfSuccessFullyNavigatedAndSaveCookies())
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage,
                                dominatorAccountModel.AccountBaseModel.AccountNetwork,
                                dominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                $"your premium has expired, Please reactivate to use {ActivityType}");
                            dominatorAccountModel.IsNeedToSchedule = false;
                            dominatorAccountModel.CancellationSource.Cancel();

                            return;
                        }

                        Thread.Sleep(5000);

                        var demoSalesUrl =
                            "https://www.linkedin.com/sales/search/people?companySize=D&function=13,8,19&geo=us:il&industry=96&logHistory=true&logId=7328892403&page=1&searchSessionId=g7VeG01JT7+8kjlHI1YlGQ==&seniority=6,5,7";
                        //var demoSalesUrl = "https://www.linkedin.com/sales/search/people?companyIncluded=C%25C3%25A2mara%20Municipal%20de%20S%25C3%25A3o%20Paulo%3A823688&companyTimeScope=CURRENT&functionIncluded=5&geoIncluded=br%3A6368&keywords=assessor%20parlamentar&logHistory=true&logId=1883545885&page=1&relationship=S%2CO&searchSessionId=twAvdg2fT4St1vIkvJkbQQ%3D%3D&seniorityIncluded=3";

                        var salesNavUserApi = ldDataHelper.GetSalesNavUserApi(_ldFunctions, demoSalesUrl);

                        if (string.IsNullOrEmpty(salesNavUserApi))
                            salesNavUserApi = apiAssist.GetNewSalesNavApiPeoplesType(demoSalesUrl);
                        var actionUrl = ldDataHelper.ActionUrl(salesNavUserApi, 0.ToString());

                        var demoSalesResponse = _ldFunctions.SalesNavigatorLinkedinUsersFromSearchUrl(actionUrl, "");

                        // not go further for break if not successful first time
                        if (!demoSalesResponse.Success || string.IsNullOrEmpty(demoSalesResponse.TotalResultsInSearch))
                            continue;

                        if (!DominatorAccountModel.ExtraParameters.ContainsKey(LdConstants
                            .IsPresentSalesNavigatorCookies))
                            DominatorAccountModel.ExtraParameters.Add(LdConstants.IsPresentSalesNavigatorCookies,
                                "Yes");
                        GlobusLogHelper.log.Info(
                            $"Browser login successfull with {DominatorAccountModel.AccountBaseModel.UserName} !");

                        SocinatorAccountBuilder.Instance(DominatorAccountModel.AccountBaseModel.AccountId)
                            .AddOrUpdateDominatorAccountBase(DominatorAccountModel.AccountBaseModel)
                            .AddOrUpdateCookies(DominatorAccountModel.Cookies)
                            .SaveToBinFile();
                        break;
                    }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                //sometimes get IBrowser issue initialize issue in debug time 
                // because of debug is inserted in between code execution
            }
            finally
            {
                try
                {
                    if (ActivityType == ActivityType.SalesNavigatorUserScraper ||
                        ActivityType == ActivityType.SalesNavigatorCompanyScraper || IsConnectionRequestSalesSearchUrl)
                        if (!IsViewProfileUsingEmbeddedBrowser)
                        {
                            Application.Current.Dispatcher.Invoke(() => { _browserWindow?.Close(); });
                            LDAccountsBrowserDetails.CloseBrowser(dominatorAccountModel);
                        }
                }
                catch (Exception exception)
                {
                    exception.DebugLog();
                }
            }
        }


        private void CloseBrowser()
        {
            if (_browserWindow == null) return;
            Application.Current.Dispatcher.Invoke(() =>
            {
                _browserWindow.Close();
                _browserWindow.Dispose();
            });
            LDAccountsBrowserDetails.GetInstance().AccountBrowserCollections.Remove(DominatorAccountModel.UserName);
        }

        private bool CheckIfSuccessFullyNavigatedAndSaveCookies()
        {
            try
            {
                // when cookies count is greater than 20 
                // if (DominatorAccountModel.Cookies.Count < 21)
                if (DominatorAccountModel.Cookies.Count < 21 ||
                    !DominatorAccountModel.ExtraParameters.ContainsKey(LdConstants.IsPresentSalesNavigatorCookies))
                {
                    //_browserWindow.Browser.ExecuteScriptAsync("document.getElementById($($('.nav-item__link.ember-view')[1]).attr('id')).click()");

                    // instead of clicking sales icon simply visit account the sales nav page url
                    // sometime we are not getting icon for click
                    _browserWindow.Browser.Load(
                        "https://www.linkedin.com/sales?trk=d_flagship3_nav&lipi=urn%3Ali%3Apage%3Ad_flagship3_feed%3BrymttEIhTses%2BGTmxIpXwg%3D%3D&licu=urn%3Ali%3Acontrol%3Ad_flagship3_feed-nav.sales_navigator");
                    Thread.Sleep(25000);
                }
                else
                {
                    if (!IsViewProfileUsingEmbeddedBrowser)
                        Application.Current.Dispatcher.Invoke(() => { _browserWindow.Close(); });

                    // DominatorAccountModel.IsUserLoggedIn = CheckLoginAsync(DominatorAccountModel, DominatorAccountModel.Token).Result;
                    return true;
                }

                #region Condition to check if successfully navigated to SalesNav

                var SalesNavigatorResponse = _browserWindow.Browser.GetSourceAsync();
                var SalesResponse = HttpUtility.HtmlDecode(SalesNavigatorResponse.Result);
                if (SalesResponse.Contains("premium-card-select-cta artdeco-button artdeco-button--3 artdeco-button--secondary ember-view") &&
                    SalesResponse.Contains("t-24 t-bold premium-chooser__welcome-headline"))
                    return false;

                #endregion

                DominatorAccountModel.IsUserLoggedIn = true;
                var cookieCollection = LdDataHelper.GetInstance.GetCookieCollectionFromEmbeddedBrowser(_browserWindow, out _);

                var requestParameters = (RequestParameters) _ldFunctions.GetInnerHttpHelper().GetRequestParameter();
                requestParameters.Cookies = cookieCollection;
                _ldFunctions.GetInnerHttpHelper().SetRequestParameter(requestParameters);
                DominatorAccountModel.IsUserLoggedIn = true;
                DominatorAccountModel.Cookies = cookieCollection;


                SocinatorAccountBuilder.Instance(DominatorAccountModel.AccountBaseModel.AccountId)
                    .AddOrUpdateLoginStatus(DominatorAccountModel.IsUserLoggedIn)
                    .AddOrUpdateCookies(DominatorAccountModel.Cookies)
                    .AddOrUpdateExtraParameter(DominatorAccountModel.ExtraParameters)
                    .SaveToBinFile();


                // DominatorAccountModel.ExtraParameters.Add();

                #region close opened popUp window

                //try
                //{
                //    Mouse.Click();
                //    SendKeys.SendWait("{TAB}");
                //    // closing pop up window of sales navigator
                //    if (AccountModel.Cookies.Count < 15)
                //    {
                //        Thread.Sleep(2000);
                //        SendKeys.SendWait("(%{F4})");
                //        Thread.Sleep(2000);
                //    }
                //    Application.Current.Dispatcher.Invoke(() => { BrowserWindow.Browser.Dispose(); });

                //}
                //catch (Exception exception)
                //{
                //    exception.DebugLog();
                //} 

                #endregion

                return true;
            }
            catch (Exception ex)
            {
                return false;
                ex.DebugLog();
            }
        }


        public bool InitializeSalesNavRequestParameter(DominatorAccountModel dominatorAccountModel)
        {
            var isHaveSufficientCookies = false;
            try
            {
                //var ldFunctions = GetLdFunctions();
                var requestParameters = (RequestParameters) _ldFunctions.GetInnerHttpHelper().GetRequestParameter();
                requestParameters.Cookies = dominatorAccountModel.Cookies;

                _ldFunctions.GetInnerHttpHelper().SetRequestParameter(requestParameters);
                if (ActivityType != ActivityType.ExportConnection && ActivityType != ActivityType.SendGroupInvitations 
                    ? dominatorAccountModel.Cookies.Count > 20:false &&
                    dominatorAccountModel.ExtraParameters.ContainsKey(LdConstants.IsPresentSalesNavigatorCookies))
                    isHaveSufficientCookies = true;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return isHaveSufficientCookies;
        }

        #region Login

        public void LoginWithDataBaseCookies(DominatorAccountModel dominatorAccountModel, bool isMobileRequired,
            CancellationToken cancellationToke)
        {
            LoginWithDataBaseCookiesAsync(dominatorAccountModel, isMobileRequired, cancellationToke).Wait();
        }

        public async Task LoginWithDataBaseCookiesAsync(DominatorAccountModel dominatorAccountModel,
            bool isMobileRequired, CancellationToken cancellationToken)
        {
            try
            {
                //var check = _httpHelper.GetRequest("https://whatismyipaddress.com/");
                if (ActivityType == ActivityType.SalesNavigatorUserScraper ||
                    ActivityType == ActivityType.ExportConnection || IsViewProfileUsingEmbeddedBrowser ||
                    ActivityType == ActivityType.SalesNavigatorCompanyScraper || IsConnectionRequestSalesSearchUrl || ActivityType== ActivityType.SendGroupInvitations)
                    LoginWithAlternativeMethod(dominatorAccountModel, cancellationToken);
                else
                    await LoginWithMobileDevice(dominatorAccountModel, cancellationToken);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            dominatorAccountModel.Token.ThrowIfCancellationRequested();
        }

        #endregion

        private void GetThisAccountPersonalDetails(DominatorAccountModel objDominatorAccountModel, string resLogin)
        {
            #region Default Values For Variables in this method

            var firstName = string.Empty;
            var lastName = string.Empty;
            var userdetails = string.Empty;
            var publicIdentifier = string.Empty;
            var profileUrl = string.Empty;
            var trackingId = string.Empty;
            var memberId = string.Empty;
            var occupation = string.Empty;
            var jsonHandler = JsonJArrayHandler.GetInstance;
            var jsonObject = jsonHandler.ParseJsonToJObject(resLogin) ?? jsonHandler.ParseJsonToJObject(_ldFunctions.GetInnerLdHttpHelper().GetRequest(LdConstants.GetOwnerProfileDetailsAPI).Response);
            var profileDetails = jsonHandler.GetTokenElement(jsonObject, "miniProfile");
            #endregion

            try
            {
                objDominatorAccountModel.AccountBaseModel.UserFullName = string.Empty;

                #region FirstName,LastName,UserFullName

                try
                {
                    CheckAndRemove(DominatorAccountModel, "FirstName");

                    if (!objDominatorAccountModel.ExtraParameters.ContainsKey("FirstName"))
                    {
                        firstName = jsonHandler.GetJTokenValue(profileDetails, "firstName");
                        objDominatorAccountModel.ExtraParameters.Add("FirstName", firstName);
                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                try
                {
                    CheckAndRemove(DominatorAccountModel, "LastName");
                    if (!objDominatorAccountModel.ExtraParameters.ContainsKey("LastName"))
                    {
                        lastName = jsonHandler.GetJTokenValue(profileDetails, "lastName");
                        objDominatorAccountModel.ExtraParameters.Add("LastName", lastName);
                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                if (!string.IsNullOrEmpty(firstName))
                {
                    var userFullName = firstName + " " + lastName;
                    objDominatorAccountModel.AccountBaseModel.UserFullName = userFullName;
                }

                #endregion

                #region PublicIdentifier

                try
                {
                    CheckAndRemove(DominatorAccountModel, "PublicIdentifier");
                    if (!objDominatorAccountModel.ExtraParameters.ContainsKey("PublicIdentifier"))
                    {
                        publicIdentifier = jsonHandler.GetJTokenValue(profileDetails, "publicIdentifier");
                        objDominatorAccountModel.ExtraParameters.Add("PublicIdentifier", publicIdentifier);
                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                #endregion

                #region UserId/ProfileId

                try
                {
                    var profileId = jsonHandler.GetJTokenValue(profileDetails, "entityUrn")?.Replace("urn:li:fs_miniProfile:", "");
                    objDominatorAccountModel.AccountBaseModel.UserId = profileId;
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                #endregion

                #region TrackingId

                try
                {
                    CheckAndRemove(DominatorAccountModel, "TrackingId");
                    if (!objDominatorAccountModel.ExtraParameters.ContainsKey("TrackingId"))
                    {
                        trackingId = jsonHandler.GetJTokenValue(profileDetails, "trackingId");
                        objDominatorAccountModel.ExtraParameters.Add("TrackingId", trackingId);
                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                #endregion

                #region ObjectUrn

                try
                {
                    CheckAndRemove(DominatorAccountModel, "MemberId");
                    if (!objDominatorAccountModel.ExtraParameters.ContainsKey("MemberId"))
                    {
                        memberId = jsonHandler.GetJTokenValue(jsonObject, "plainId");
                        objDominatorAccountModel.ExtraParameters.Add("MemberId", memberId);
                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                #endregion

                #region Occupation

                try
                {
                    CheckAndRemove(DominatorAccountModel, "Occupation");
                    if (!objDominatorAccountModel.ExtraParameters.ContainsKey("Occupation"))
                    {
                        occupation = jsonHandler.GetJTokenValue(profileDetails, "occupation");
                        objDominatorAccountModel.ExtraParameters.Add("Occupation", occupation);
                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                #endregion

                if (string.IsNullOrEmpty(publicIdentifier))
                    return;

                profileUrl = "https://www.linkedin.com/in/" + publicIdentifier;
                objDominatorAccountModel.AccountBaseModel.ProfilePictureUrl = profileUrl;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private bool CheckAndRemove(DominatorAccountModel dominatorAccountModel, string propertyName)
        {
            // check if contains key, if contains then not be empty, then remove
            string propertyValue;
            if (dominatorAccountModel.ExtraParameters.ContainsKey(propertyName)
                && dominatorAccountModel.ExtraParameters.TryGetValue(propertyName, out propertyValue)
                && string.IsNullOrEmpty(propertyValue))
                return dominatorAccountModel.ExtraParameters.Remove(propertyName);
            return false;
        }

        public void OnSuccess(DominatorAccountModel dominatorAccountModel, bool isSave = false)
        {
            try
            {
                var loginResponse = "";
                string propertyValue;
                dominatorAccountModel.ExtraParameters.TryGetValue("FirstName", out propertyValue);
                if (string.IsNullOrEmpty(loginResponse) && string.IsNullOrEmpty(propertyValue))
                    loginResponse = _ldFunctions.GetRequestUpdatedUserAgent("https://www.linkedin.com/", true);

                if (!string.IsNullOrEmpty(loginResponse) && string.IsNullOrEmpty(propertyValue))
                    GetThisAccountPersonalDetails(dominatorAccountModel, loginResponse);

                dominatorAccountModel.IsloggedinWithPhone = true;
                dominatorAccountModel.IsUserLoggedIn = true;
                dominatorAccountModel.AccountBaseModel.Status = AccountStatus.Success;
                if (!IsBrowser)
                    dominatorAccountModel.Cookies = _httpHelper.GetRequestParameter().Cookies;

                if (isSave)
                    SocinatorAccountBuilder.Instance(dominatorAccountModel.AccountBaseModel.AccountId)
                        .AddOrUpdateDominatorAccountBase(dominatorAccountModel.AccountBaseModel)
                        .AddOrUpdateCookies(dominatorAccountModel.Cookies)
                        .AddOrUpdateExtraParameter(dominatorAccountModel.ExtraParameters)
                        .SaveToBinFile();
                lock (LockGlobalDb)
                {
                    var globalDbOperation =
                        new DbOperations(SocinatorInitialize.GetGlobalDatabase().GetSqlConnection());
                    if (globalDbOperation.Get<AccountDetails>().Any(account => dominatorAccountModel.AccountId == account.AccountId && account.ActivityManager is null))
                        globalDbOperation.RemoveMatch<AccountDetails>(account => dominatorAccountModel.AccountId == account.AccountId && account.ActivityManager == null);
                    globalDbOperation.UpdateAccountDetails(dominatorAccountModel);
                    accountSessionManager.AddOrUpdateSession(ref dominatorAccountModel,true);
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }


        public async Task<bool> SolveCaptcha(LoginResponseHandler loginResponseHandler, LdFunctions ldFunctions)
        {
            var isSolvedCaptcha = false;
            var imageHelper = new ImageTypersHelper("5EA22F642D6D4C9BAFCD966685E9B4D1");
            var captchaId = imageHelper.SubmitSiteKey("", "");
            imageHelper.GetGResponseCaptcha(captchaId);
            // var firstUrl = ldFunction.ObjLdHttpHelper.Response.ResponseUri.ToString();// $"https://www.linkedin.com/uas/authenticate?nc={DateTime.Now.GetCurrentEpochTimeMilliSeconds()}";
            try
            {
                GlobusLogHelper.log.Info(Log.LoginFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, "Found Captcha.");

                var solvedCaptchaKey = GetSolvedCaptcha(loginResponseHandler, ldFunctions);
                if (solvedCaptchaKey != null && !string.IsNullOrEmpty(solvedCaptchaKey) ||
                    solvedCaptchaKey.Equals("ERROR: INVALID_CAPTCHA_ID"))
                {
                    //string verifyCaptcha =
                    //    "https://www.google.com/recaptcha/api2/userverify?k=6LdcXFUUAAAAANu32LPaK4zJiXYrjHj4efRGaMFu";
                    var captchaCheckPointUrl =
                        $"https://www.linkedin.com/checkpoint/challenges/nativeCaptchaChallenge/{solvedCaptchaKey}?displayTime=33902038&nc=" +
                        DateTime.Now.GetCurrentEpochTimeMilliSeconds();
                    var captchaCheckPointPostData = "{\"userResponseToken\":\"" + solvedCaptchaKey + "\"}";
                    var response = _ldFunctions.GetInnerHttpHelper()
                        .PostRequest(captchaCheckPointUrl, captchaCheckPointPostData);
                    if (response.Response.Contains("SUCCESS\"}"))
                    {
                        var responseHandler = await ldFunctions.Login(DominatorAccountModel.CancellationSource.Token);
                        ldFunctions.GetInnerHttpHelper();
                        if (responseHandler.Success)
                            isSolvedCaptcha = responseHandler.Success;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return isSolvedCaptcha;
        }

        public string GetSolvedCaptcha(LoginResponseHandler loginResponseHandler, LdFunctions ldFunctions)
        {
            var captchaKeyResponse = string.Empty;
            var responseUri =
                "https://www.linkedin.com/uas/authenticate?nc=" +
                Utils.GenerateNc(); //AccountModel.HttpHelper.Response.ResponseUri.ToString();

            try
            {
                var genericFileManager = InstanceProvider.GetInstance<IGenericFileManager>();
                var imageCaptchaServicesModel =
                    genericFileManager.GetModel<ImageCaptchaServicesModel>(
                        ConstantVariable.GetImageCaptchaServicesFile()) ?? new ImageCaptchaServicesModel();

                if (string.IsNullOrEmpty(imageCaptchaServicesModel.UserName) ||
                    string.IsNullOrEmpty(imageCaptchaServicesModel.Password))
                {
                    GlobusLogHelper.log.Info(Log.LoginFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName,
                        "Not found valid userName and password for ImageTyperz.");
                    return captchaKeyResponse;
                }

                for (var index = 0; index < 5; index++)
                {
                    var getCaptchaResultFirstUrl =
                        "http://captchatypers.com/captchaapi/UploadRecaptchav1.ashx?action=UPLOADCAPTCHA&username=" +
                        $"{imageCaptchaServicesModel.UserName}" +
                        "&password=" + $"{imageCaptchaServicesModel.Password}" +
                        "&googlekey=6LdcXFUUAAAAANu32LPaK4zJiXYrjHj4efRGaMFu&pageurl=" + responseUri;


                    var captchaIdGotFromImageTyperz =
                        _ldFunctions.GetInnerHttpHelper().GetRequest(getCaptchaResultFirstUrl);
                    if (captchaIdGotFromImageTyperz.Response.Contains("ERROR:INVALID_DOMAIN"))
                        return "ERROR:INVALID_DOMAIN";
                    var captchaId = captchaIdGotFromImageTyperz.Response;
                    var getCaptchaResultSecondUrl =
                        "http://captchatypers.com/captchaapi/GetRecaptchaText.ashx?action=GETTEXT&username=" + $"{imageCaptchaServicesModel.UserName}"
                                                                                                             + "&password=" +
                                                                                                             $"{imageCaptchaServicesModel.Password}" +
                                                                                                             "&Captchaid=" +
                                                                                                             captchaId;

                    while (string.IsNullOrEmpty(captchaKeyResponse) ||
                           captchaKeyResponse.Contains("ERROR: NOT_DECODED"))
                    {
                        var responseCaptchaResultSecondUrl =
                            _ldFunctions.GetInnerHttpHelper().GetRequest(getCaptchaResultSecondUrl);
                        captchaKeyResponse = responseCaptchaResultSecondUrl.Response;
                        if (responseCaptchaResultSecondUrl.Response.Contains("ERROR: IMAGE_TIMED_OUT"))
                        {
                            GlobusLogHelper.log.Debug(DominatorAccountModel.UserName + " TimeOut in captcha S");
                            return string.Empty;
                        }

                        Thread.Sleep(5 * 1000);
                    }
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return captchaKeyResponse;
        }

        public void LoginWithBrowserMethod(DominatorAccountModel dominatorAccountModel,
            CancellationToken cancellationToken, VerificationType verificationType = 0, LoginType loginType = LoginType.AutomationLogin)
        {
            try
            {
                DominatorAccountModel = dominatorAccountModel;
                IsBrowser = true;
                //Assigning BrowserLdFunctions for checkLoginProcess
                if (_ldFunctions.GetType().Name.Equals("LdFunctions"))
                {
                    _ldFunctionFactory.AssignBrowserFunction();
                    _ldFunctions = _ldFunctionFactory.LdFunctions;
                }

                var browserType = IsCheckAccountStatus
                    ? BrowserInstanceType.CheckAccountStatus
                    : BrowserInstanceType.Primary;
                accountSessionManager.AddOrUpdateSession(ref dominatorAccountModel);
                LDAccountsBrowserDetails.GetInstance()
                    .StartBrowserLogin(dominatorAccountModel, cancellationToken, true, browserType, accountSessionManager);

                var name = LDAccountsBrowserDetails.GetBrowserName(dominatorAccountModel, browserType);
                _browserWindow = LDAccountsBrowserDetails.GetInstance().AccountBrowserCollections[name];
                _ldFunctions.SetCookieAndProxy(dominatorAccountModel, _httpHelper, browserType);

                CheckLoggedInFromPageResponse(dominatorAccountModel, _browserWindow.GetPageSource());
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }

    // ReSharper disable once UnusedMember.Global
    public class LifeSpanHandler : ILifeSpanHandler
    {
        public bool DoClose(IWebBrowser browserControl, IBrowser browser)
        {
            return false;
        }

        public bool OnBeforePopup(IWebBrowser browserControl, IBrowser browser, IFrame frame, string targetUrl,
            string targetFrameName,
            WindowOpenDisposition targetDisposition, bool userGesture, IPopupFeatures popupFeatures,
            IWindowInfo windowInfo,
            IBrowserSettings browserSettings, ref bool noJavascriptAccess, out IWebBrowser newBrowser)
        {
            Process.Start(targetUrl);
            newBrowser = null;
            return true;
        }

        public void OnAfterCreated(IWebBrowser browserControl, IBrowser browser)
        {
            // DO NOTHING
        }

        public void OnBeforeClose(IWebBrowser browserControl, IBrowser browser)
        {
            // DO NOTHING
        }
    }
}