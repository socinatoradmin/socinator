using DominatorHouseCore;
using DominatorHouseCore.BusinessLogic.ActivitiesWorkflow;
using DominatorHouseCore.DatabaseHandler.DHTables;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Extensions;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.PuppeteerBrowser;
using DominatorHouseCore.Request;
using DominatorHouseCore.Utility;
using RedditDominatorCore.Interface;
using RedditDominatorCore.RDLibrary.BrowserManager;
using RedditDominatorCore.RDRequest;
using RedditDominatorCore.RDUtility;
using RedditDominatorCore.Response;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;

namespace RedditDominatorCore.RDLibrary
{
    public interface IRedditLogInProcess : ILoginProcessAsync
    {
        bool CheckLogin(DominatorAccountModel dominatorAccountModel, CancellationToken token);
        Task LoginForAdsScrapingAsync(DominatorAccountModel account, CancellationToken cancellatrionToken);
        IRdBrowserManager _browserManager { get; set; }
    }

    public class LogInProcess : IRedditLogInProcess
    {
        private static SemaphoreSlim SemaphoreSlim = new SemaphoreSlim(5, 5);

        private readonly IAccountsFileManager _accountsFileManager;
        private readonly IRdHttpHelper _httpHelper;
        private readonly IRedditFunction _redditFunction;
        public IRdBrowserManager _browserManager { get; set; }
        private readonly IRDAccountSessionManager sessionManager;
        public LogInProcess(IAccountsFileManager accountsFileManager, IRdHttpHelper httpHelper,
            IRedditFunction redditFunction, IRdBrowserManager browserManager, IRDAccountSessionManager rDAccountSession)
        {
            _accountsFileManager = accountsFileManager;
            _httpHelper = httpHelper;
            _redditFunction = redditFunction;
            _browserManager = browserManager;
            sessionManager = rDAccountSession;
        }
        /// <summary>
        /// CheckLogin is used to check whether Account is Logged in
        /// It will return true if Account Logged in Otherwise it will return false
        /// </summary>
        /// <param name="dominatorAccountModel"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public bool CheckLogin(DominatorAccountModel dominatorAccountModel, CancellationToken token)
        {
            sessionManager.AddOrUpdateSession(ref dominatorAccountModel);
            _httpHelper.GetRequestParameter().Cookies = dominatorAccountModel.Cookies;
            var accloginResp = _httpHelper.GetRequest("https://www.reddit.com/").Response;

            bool isloggedIn=false;
            try
            {
                isloggedIn = accloginResp.Contains("user-logged-in=\"true");
            }
            catch (Exception) { }

            if (isloggedIn)
            {
                dominatorAccountModel.IsUserLoggedIn = true;
                sessionManager.AddOrUpdateSession(ref dominatorAccountModel, true);
                return true;
            }

            dominatorAccountModel.IsUserLoggedIn = false;
            return false;
        }

        public async Task<bool> CheckLoginAsync(DominatorAccountModel dominatorAccountModel, CancellationToken cancellationToken, bool displayLoginMsg = false, LoginType loginType = LoginType.AutomationLogin)
        {
            sessionManager.AddOrUpdateSession(ref dominatorAccountModel);
            _httpHelper.GetRequestParameter().Cookies = dominatorAccountModel.Cookies;

            // Check Login through browser
            if (dominatorAccountModel.IsRunProcessThroughBrowser)
            {
                LoginWithBrowserMethod(dominatorAccountModel, cancellationToken);
                if (dominatorAccountModel.IsUserLoggedIn)
                {
                    dominatorAccountModel.AccountBaseModel.Status = dominatorAccountModel.AccountBaseModel.Status != AccountStatus.TemporarilyBlocked ? AccountStatus.Success : AccountStatus.TemporarilyBlocked;
                    SocinatorAccountBuilder.Instance(dominatorAccountModel.AccountBaseModel.AccountId)
                    .AddOrUpdateDominatorAccountBase(dominatorAccountModel.AccountBaseModel)
                    .AddOrUpdateCookies(dominatorAccountModel.Cookies)
                    .SaveToBinFile();
                }
                _browserManager.CloseBrowser();
            }

            else if (_httpHelper.GetRequestParameter().Cookies.Count != 0)
                await LoginWithDataBaseCookiesAsync(dominatorAccountModel, false, cancellationToken);

            else
                await LoginWithAlternativeMethodAsync(dominatorAccountModel, cancellationToken);
            //if (dominatorAccountModel.IsUserLoggedIn)
            //{
            //    await ThreadFactory.Instance.Start(async () =>
            //    {
            //        var count = RandomUtilties.GetRandomNumber(20, 10);
            //        await _redditFunction.StartScrapingAds(dominatorAccountModel, count);
            //    });
            //}
            return dominatorAccountModel.IsUserLoggedIn;
        }

        public bool CheckLogin(DominatorAccountModel dominatorAccountModel)
        {
            return CheckLoginAsync(dominatorAccountModel, dominatorAccountModel.Token).Result;
        }

        public void LoginWithDataBaseCookies(DominatorAccountModel dominatorAccountModel, bool isMobileRequired, CancellationToken cancellationToken)
        {
            LoginWithDataBaseCookiesAsync(dominatorAccountModel, isMobileRequired, cancellationToken).Wait();
        }

        public void LoginWithAlternativeMethod(DominatorAccountModel dominatorAccountModel)
        {
            LoginWithAlternativeMethodAsync(dominatorAccountModel, dominatorAccountModel.Token).Wait();
        }

        public async Task LoginWithDataBaseCookiesAsync(DominatorAccountModel dominatorAccountModel, bool isMobileRequired, CancellationToken cancellationToken)
        {
            try
            {
                sessionManager.AddOrUpdateSession(ref dominatorAccountModel);
                var requestParameters = _redditFunction.SetRequestParametersAndProxy(dominatorAccountModel);
                var userName = string.Empty;
                var isloggedIn = false;
                IResponseParameter accountLoginResponse;

                if (dominatorAccountModel != null && dominatorAccountModel.Cookies.Count > 0)
                {
                    requestParameters.Cookies = dominatorAccountModel.Cookies;
                    _httpHelper.SetRequestParameter(requestParameters);
                    //accountLoginResponse = await _httpHelper.GetRequestAsync("https://www.reddit.com/top", dominatorAccountModel.Token);
                    accountLoginResponse = await _httpHelper.GetRequestAsync(RdConstants.GetProfileDetailsAPI(dominatorAccountModel.AccountBaseModel.UserName), dominatorAccountModel.Token);

                    try
                    {
                        if(accountLoginResponse !=null && accountLoginResponse.Response.Contains("error\": 404"))
                        {
                            var PageSource = _httpHelper.GetRequest(RdConstants.GetRedditHomePageAPI).Response;
                            if (!string.IsNullOrEmpty(PageSource) && !PageSource.Contains("Log In") &&
                              (PageSource.ToLower().Contains(dominatorAccountModel.AccountBaseModel.UserName.ToLower().Trim()) ||
                               PageSource.Contains("Log out") || PageSource.Contains("logged in") ||
                               PageSource.Contains("user-logged-in=\"true\"") || PageSource.Contains("is-logged-in-user=\"true\"") ||
                               PageSource.Contains("id=\"USER_DROPDOWN_ID\"") || PageSource.Contains("User account menu")
                               || PageSource.Contains("is-logged-in=\"true\"")))
                            {
                                isloggedIn = true;
                            }
                        }
                        else
                        {
                            var jsonHand = new JsonHandler(accountLoginResponse?.Response);
                            var accountUserId = jsonHand.GetJToken("data");
                            userName = accountUserId.HasValues ? jsonHand.GetJTokenValue(accountUserId, "name")?.ToString() : string.Empty;
                        }
                            
                    }
                    catch (Exception) { }
                }
                else
                {
                    LoginWithAlternativeMethod(dominatorAccountModel, cancellationToken);
                    return;
                }

                if (!string.IsNullOrEmpty(userName) || isloggedIn)
                {
                    GlobusLogHelper.log.Info(Log.SuccessfulLogin, dominatorAccountModel.AccountBaseModel.AccountNetwork,
                        dominatorAccountModel.AccountBaseModel.UserName);
                    sessionManager.AddOrUpdateSession(ref dominatorAccountModel, true);
                    dominatorAccountModel.IsUserLoggedIn = true;
                    dominatorAccountModel.AccountBaseModel.Status = AccountStatus.Success;
                    if (!string.IsNullOrEmpty(accountLoginResponse.Response) && accountLoginResponse.Response.Contains(RdConstants.SuspendedMessage))
                        dominatorAccountModel.AccountBaseModel.Status = AccountStatus.TemporarilyBlocked;
                    else if (!string.IsNullOrEmpty(accountLoginResponse.Response) && (accountLoginResponse.Response.Contains(RdConstants.PermanentlyBanned) || accountLoginResponse.Response.Contains("\"is_suspended\": true")))
                        dominatorAccountModel.AccountBaseModel.Status = AccountStatus.PermanentlyBlocked;
                    SocinatorAccountBuilder.Instance(dominatorAccountModel.AccountBaseModel.AccountId)
                    .AddOrUpdateDominatorAccountBase(dominatorAccountModel.AccountBaseModel)
                    .AddOrUpdateCookies(dominatorAccountModel.Cookies)
                    .SaveToBinFile();
                }
                else
                {
                    await LoginWithAlternativeMethodAsync(dominatorAccountModel, cancellationToken);
                    return;
                }
            }
            catch (Exception ex)
            {
                dominatorAccountModel.IsUserLoggedIn = false;
                dominatorAccountModel.AccountBaseModel.Status = AccountStatus.Failed;
                GlobusLogHelper.log.Info(Log.LoginFailed, dominatorAccountModel.AccountBaseModel.AccountNetwork,
                    dominatorAccountModel.AccountBaseModel.UserName);
                ex.DebugLog();
            }
        }

        public void LoginWithAlternativeMethod(DominatorAccountModel dominatorAccountModel, CancellationToken cancellationToken)
        {
            LoginWithAlternativeMethodAsync(dominatorAccountModel, cancellationToken).Wait(cancellationToken);
        }

        public async Task LoginWithAlternativeMethodAsync(DominatorAccountModel dominatorAccountModel, CancellationToken cancellationToken)
        {
            try
            {
                await SemaphoreSlim.WaitAsync();
                #region Rd Login 
                sessionManager.AddOrUpdateSession(ref dominatorAccountModel);
                await Task.Run(()=>LoginWithBrowserMethod(dominatorAccountModel, dominatorAccountModel.Token));

                #region Old Login Code.
                //var requestParameters = _redditFunction.SetRequestParametersAndProxy(dominatorAccountModel);
                //SetHeader(requestParameters);
                //_httpHelper.SetRequestParameter(requestParameters);

                ////New Login 
                //var csrfResponse = await _httpHelper.GetRequestAsync("https://www.reddit.com/login/", dominatorAccountModel.Token);
                //var csrfToken = Utilities.GetBetween(csrfResponse.Response, "name=\"csrf_token\" value=\"", "\">");
                //if (string.IsNullOrEmpty(csrfToken))
                //    csrfToken = _httpHelper.GetRequestParameter()?.Cookies?.Cast<Cookie>()?.FirstOrDefault(x => x.Name == "csrf_token")?.Value;
                //if (string.IsNullOrEmpty(csrfToken) && csrfResponse.Response.Contains("Access Denied"))
                //{
                //    dominatorAccountModel.AccountBaseModel.Status = AccountStatus.ProxyNotWorking;
                //    dominatorAccountModel.IsUserLoggedIn = false;
                //    SocinatorAccountBuilder.Instance(dominatorAccountModel.AccountBaseModel.AccountId)
                //        .AddOrUpdateDominatorAccountBase(dominatorAccountModel.AccountBaseModel)
                //        .SaveToBinFile();
                //    return;
                //}

                ////var loginElements = new RdLoginJsonElements
                ////{
                ////    CsrfToken = csrfToken,
                ////    OTP = string.Empty,
                ////    Password = Uri.EscapeDataString(dominatorAccountModel.AccountBaseModel.Password),
                ////    Destination = Uri.EscapeDataString("https://www.reddit.com/"),
                ////    Username = dominatorAccountModel.AccountBaseModel.UserName
                ////};
                //var rdRequestParameter = new RequestParameter();
                ////var loginPostData = rdRequestParameter.GetPostDataFromJson(loginElements);
                //var loginPostData = Encoding.UTF8.GetBytes($"recaptchaToken=&username={dominatorAccountModel.AccountBaseModel.UserName}&password={Uri.EscapeDataString(dominatorAccountModel.AccountBaseModel.Password)}&csrf_token={csrfToken}");
                ////var postRespo = await _httpHelper.PostRequestAsync("https://www.reddit.com/login", loginPostData, dominatorAccountModel.Token);
                //var postRespo = await _httpHelper.PostRequestAsync("https://www.reddit.com/svc/shreddit/account/login", loginPostData, dominatorAccountModel.Token);

                ////Check with Login response
                //var loginRdResponseHandler = new LoginRdResponseHandler(postRespo);

                //if (!loginRdResponseHandler.HasError && !postRespo.Response.Contains("page not found"))
                //{
                //    var response = await _httpHelper.GetRequestAsync(RdConstants.UserProfileUrlByUsername(dominatorAccountModel.AccountBaseModel.UserName), dominatorAccountModel.Token);
                //    var accountType = response.Response.DetectResponseTypeOldOrNewPage();

                //    sessionManager.AddOrUpdateSession(ref dominatorAccountModel, true);
                //    dominatorAccountModel.IsUserLoggedIn = true;
                //    dominatorAccountModel.AccountBaseModel.Status = AccountStatus.Success;
                //    if(!string.IsNullOrEmpty(response.Response) && response.Response.Contains(RdConstants.SuspendedMessage))
                //        dominatorAccountModel.AccountBaseModel.Status = AccountStatus.TemporarilyBlocked;
                //    dominatorAccountModel.Cookies = _httpHelper.GetRequestParameter().Cookies;

                //    GlobusLogHelper.log.Info(Log.SuccessfulLogin, dominatorAccountModel.AccountBaseModel.AccountNetwork,
                //        dominatorAccountModel.AccountBaseModel.UserName);

                //    dominatorAccountModel.Token.ThrowIfCancellationRequested();
                //    SocinatorAccountBuilder.Instance(dominatorAccountModel.AccountBaseModel.AccountId)
                //.AddOrUpdateDominatorAccountBase(dominatorAccountModel.AccountBaseModel)
                //.AddOrUpdateCookies(dominatorAccountModel.Cookies)
                //.SaveToBinFile();
                //}
                //else
                //{
                //    dominatorAccountModel.IsUserLoggedIn = false;
                //    dominatorAccountModel.AccountBaseModel.Status = AccountStatus.Failed;
                //    GlobusLogHelper.log.Info(Log.LoginFailed, dominatorAccountModel.AccountBaseModel.AccountNetwork,
                //    dominatorAccountModel.AccountBaseModel.UserName, loginRdResponseHandler.Error);
                //    SocinatorAccountBuilder.Instance(dominatorAccountModel.AccountBaseModel.AccountId)
                //        .AddOrUpdateDominatorAccountBase(dominatorAccountModel.AccountBaseModel)
                //        .SaveToBinFile();
                //}
                #endregion

                if (dominatorAccountModel.IsUserLoggedIn)
                {
                    var response = await _httpHelper.GetRequestAsync(RdConstants.UserProfileUrlByUsername(dominatorAccountModel.AccountBaseModel.UserName), dominatorAccountModel.Token);
                    sessionManager.AddOrUpdateSession(ref dominatorAccountModel, true);
                    dominatorAccountModel.IsUserLoggedIn = true;
                    dominatorAccountModel.AccountBaseModel.Status = AccountStatus.Success;
                    if (!string.IsNullOrEmpty(response.Response) && response.Response.Contains(RdConstants.SuspendedMessage))
                        dominatorAccountModel.AccountBaseModel.Status = AccountStatus.TemporarilyBlocked;
                    else if (!string.IsNullOrEmpty(response.Response) && response.Response.Contains(RdConstants.PermanentlyBanned))
                        dominatorAccountModel.AccountBaseModel.Status = AccountStatus.PermanentlyBlocked;
                    dominatorAccountModel.Cookies = _httpHelper.GetRequestParameter().Cookies;
                    GlobusLogHelper.log.Info(Log.SuccessfulLogin, dominatorAccountModel.AccountBaseModel.AccountNetwork,
                        dominatorAccountModel.AccountBaseModel.UserName);
                    dominatorAccountModel.Token.ThrowIfCancellationRequested();
                    SocinatorAccountBuilder.Instance(dominatorAccountModel.AccountBaseModel.AccountId)
                .AddOrUpdateDominatorAccountBase(dominatorAccountModel.AccountBaseModel)
                .AddOrUpdateCookies(dominatorAccountModel.Cookies)
                .SaveToBinFile();
                }
                else
                {
                    dominatorAccountModel.IsUserLoggedIn = false;
                    dominatorAccountModel.AccountBaseModel.Status = dominatorAccountModel.AccountBaseModel.Status != AccountStatus.InvalidCredentials ? AccountStatus.Failed : dominatorAccountModel.AccountBaseModel.Status;
                    GlobusLogHelper.log.Info(Log.LoginFailed, dominatorAccountModel.AccountBaseModel.AccountNetwork,
                    dominatorAccountModel.AccountBaseModel.UserName, dominatorAccountModel.AccountBaseModel.Status.ToString());
                    SocinatorAccountBuilder.Instance(dominatorAccountModel.AccountBaseModel.AccountId)
                        .AddOrUpdateDominatorAccountBase(dominatorAccountModel.AccountBaseModel)
                        .SaveToBinFile();
                }
                #endregion Rd Login 
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (AggregateException ae)
            {
                ae.HandleOperationCancellation();
            }
            catch (Exception ex) { ex.DebugLog(); }
            finally
            {
                var globalDbOperation = new DbOperations(SocinatorInitialize.GetGlobalDatabase().GetSqlConnection());
                if (globalDbOperation.Get<AccountDetails>().Any(account => dominatorAccountModel.AccountId == account.AccountId && account.ActivityManager is null))
                    globalDbOperation.RemoveMatch<AccountDetails>(account => dominatorAccountModel.AccountId == account.AccountId && account.ActivityManager == null);
                globalDbOperation.UpdateAccountDetails(dominatorAccountModel);
                if (_browserManager != null && _browserManager.BrowserWindow != null)
                    _browserManager.CloseBrowser();
                SemaphoreSlim.Release();
            }
        }

        private void SetHeader(RequestParameters requestParameters)
        {
            requestParameters.Headers["Sec-Fetch-Mode"] = "cors";
            requestParameters.Headers["Sec-Fetch-Site"] = "same-origin";
            requestParameters.Headers["sec-ch-ua"] = "\"Chromium\";v=\"92\", \" Not A; Brand\";v=\"99\", \"Google Chrome\";v=\"92\"";
            requestParameters.Headers["sec-ch-ua-mobile"] = "?0";
            requestParameters.Headers["Referer"] = "https://www.reddit.com/login/";
            requestParameters.Headers["Accept-Language"] = "en-US,en;q=0.9";
        }

        public void LoginWithBrowserMethod(DominatorAccountModel dominatorAccountModel, CancellationToken cancellationToken, VerificationType verificationType = 0
            , LoginType loginType = LoginType.AutomationLogin)
        {
            try
            {
                var isLoggedIn=_browserManager.BrowserLogin(dominatorAccountModel, cancellationToken);
                Task.Delay(TimeSpan.FromSeconds(4)).Wait(cancellationToken);
                var loginResponse = string.Empty;
                if(_browserManager.BrowserWindow !=null)
                   loginResponse = _browserManager.BrowserWindow?.GetPageSource();
                if (!string.IsNullOrEmpty(loginResponse) && loginResponse.Contains(RdConstants.SuspendedMessage))
                    dominatorAccountModel.AccountBaseModel.Status = AccountStatus.TemporarilyBlocked;
                else if (!string.IsNullOrEmpty(loginResponse) && loginResponse.Contains(RdConstants.PermanentlyBanned))
                    dominatorAccountModel.AccountBaseModel.Status = AccountStatus.PermanentlyBlocked;
                else if (!string.IsNullOrEmpty(loginResponse) && loginResponse.Contains(RdConstants.LockedMessage))
                    dominatorAccountModel.AccountBaseModel.Status = AccountStatus.TemporarilyBlocked;

                if (loginResponse.Contains("Incorrect password") || loginResponse.Contains("Incorrect username or password") ||!isLoggedIn)
                {
                    dominatorAccountModel.IsUserLoggedIn = false;
                    dominatorAccountModel.AccountBaseModel.Status = AccountStatus.Failed;
                    SocinatorAccountBuilder.Instance(dominatorAccountModel.AccountBaseModel.AccountId)
                        .AddOrUpdateDominatorAccountBase(dominatorAccountModel.AccountBaseModel)
                        .SaveToBinFile();
                    GlobusLogHelper.log.Info(Log.LoginFailed, dominatorAccountModel.AccountBaseModel.AccountNetwork,
                        dominatorAccountModel.AccountBaseModel.UserName, "Incorrect username or password");
                }
                else
                {
                    var objRequestParametersWeb = _redditFunction.SetRequestParametersAndProxy(dominatorAccountModel);
                    objRequestParametersWeb.Cookies = dominatorAccountModel.Cookies;
                    _httpHelper.SetRequestParameter(objRequestParametersWeb);
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public async Task LoginForAdsScrapingAsync(DominatorAccountModel dominatorAccountModel, CancellationToken cancellationToken)
        {
            try
            {
                _httpHelper.GetRequestParameter().Cookies = dominatorAccountModel.Cookies;
                if (_httpHelper.GetRequestParameter().Cookies.Count != 0)
                    await LoginWithCookies(dominatorAccountModel, cancellationToken);
                else
                    await LoginWithCredential(dominatorAccountModel, cancellationToken);
            }
            catch (Exception ex)
            {
                dominatorAccountModel.IsUserLoggedIn = false;
                dominatorAccountModel.AccountBaseModel.Status = AccountStatus.Failed;
                ex.DebugLog();
            }
        }

        private async Task LoginWithCookies(DominatorAccountModel dominatorAccountModel, CancellationToken cancellationToken)
        {
            try
            {
                sessionManager.AddOrUpdateSession(ref dominatorAccountModel);
                var account = _accountsFileManager.GetAll().FirstOrDefault(x => x.AccountId == dominatorAccountModel.AccountId);
                var requestParameters = _redditFunction.SetRequestParametersAndProxy(dominatorAccountModel);
                var userName = string.Empty;
                IResponseParameter accountLoginResponse;
                if (account != null && account.Cookies.Count > 0)
                {
                    requestParameters.Cookies = account.Cookies;
                    _httpHelper.SetRequestParameter(requestParameters);
                    accountLoginResponse = await _httpHelper.GetRequestAsync("https://www.reddit.com/top", dominatorAccountModel.Token);
                    var response = RdConstants.GetJsonPageResponse(accountLoginResponse.Response);
                    try
                    {
                        var jsonHand = new JsonHandler(response);
                        var accountUserId = jsonHand.GetJToken("user", "account");
                        userName = accountUserId.HasValues ? jsonHand.GetJTokenValue(accountUserId, "displayText")?.ToString() : string.Empty;
                    }
                    catch (Exception) { }
                }
                else
                {
                    LoginWithAlternativeMethod(dominatorAccountModel, cancellationToken);
                    return;
                }
                if (!string.IsNullOrEmpty(userName))
                {
                    sessionManager.AddOrUpdateSession(ref dominatorAccountModel, true);
                    dominatorAccountModel.IsUserLoggedIn = true;
                    dominatorAccountModel.AccountBaseModel.Status = AccountStatus.Success;
                    SocinatorAccountBuilder.Instance(dominatorAccountModel.AccountBaseModel.AccountId)
                    .AddOrUpdateDominatorAccountBase(dominatorAccountModel.AccountBaseModel)
                    .AddOrUpdateCookies(dominatorAccountModel.Cookies)
                    .SaveToBinFile();
                }
                else
                {
                    await LoginWithCredential(dominatorAccountModel, cancellationToken);
                    return;
                }
            }
            catch (Exception ex)
            {
                dominatorAccountModel.IsUserLoggedIn = false;
                dominatorAccountModel.AccountBaseModel.Status = AccountStatus.Failed;
                ex.DebugLog();
            }
        }

        private async Task LoginWithCredential(DominatorAccountModel dominatorAccountModel, CancellationToken cancellationToken)
        {
            #region Rd Login 
            sessionManager.AddOrUpdateSession(ref dominatorAccountModel);
            var requestParameters = _redditFunction.SetRequestParametersAndProxy(dominatorAccountModel);
            SetHeader(requestParameters);
            _httpHelper.SetRequestParameter(requestParameters);
            //New Login 
            var csrfResponse = await _httpHelper.GetRequestAsync("https://www.reddit.com/login/", dominatorAccountModel.Token);
            var csrfToken = Utilities.GetBetween(csrfResponse.Response, "name=\"csrf_token\" value=\"", "\">");
            if (string.IsNullOrEmpty(csrfToken) && csrfResponse.Response.Contains("Access Denied"))
            {
                dominatorAccountModel.AccountBaseModel.Status = AccountStatus.ProxyNotWorking;
                dominatorAccountModel.IsUserLoggedIn = false;
                SocinatorAccountBuilder.Instance(dominatorAccountModel.AccountBaseModel.AccountId)
                    .AddOrUpdateDominatorAccountBase(dominatorAccountModel.AccountBaseModel)
                    .SaveToBinFile();
                return;
            }

            var loginElements = new RdLoginJsonElements
            {
                CsrfToken = csrfToken,
                OTP = string.Empty,
                Password = Uri.EscapeDataString(dominatorAccountModel.AccountBaseModel.Password),
                Destination = Uri.EscapeDataString("https://www.reddit.com/"),
                Username = dominatorAccountModel.AccountBaseModel.UserName
            };
            var rdRequestParameter = new RequestParameter();
            var loginPostData = rdRequestParameter.GetPostDataFromJson(loginElements);
            var postRespo = await _httpHelper.PostRequestAsync("https://www.reddit.com/login", loginPostData, dominatorAccountModel.Token);

            //Check with Login response
            var loginRdResponseHandler = new LoginRdResponseHandler(postRespo);

            if (!loginRdResponseHandler.HasError && !postRespo.Response.Contains("page not found"))
            {
                var response = await _httpHelper.GetRequestAsync("https://www.reddit.com/user/" + dominatorAccountModel.AccountBaseModel.UserName, dominatorAccountModel.Token);
                sessionManager.AddOrUpdateSession(ref dominatorAccountModel, true);
                dominatorAccountModel.IsUserLoggedIn = true;
                dominatorAccountModel.AccountBaseModel.Status = AccountStatus.Success;
                dominatorAccountModel.Cookies = _httpHelper.GetRequestParameter().Cookies;
                dominatorAccountModel.Token.ThrowIfCancellationRequested();
                SocinatorAccountBuilder.Instance(dominatorAccountModel.AccountBaseModel.AccountId)
            .AddOrUpdateDominatorAccountBase(dominatorAccountModel.AccountBaseModel)
            .AddOrUpdateCookies(dominatorAccountModel.Cookies)
            .SaveToBinFile();
            }
            else
            {
                dominatorAccountModel.IsUserLoggedIn = false;
                dominatorAccountModel.AccountBaseModel.Status = AccountStatus.Failed;
                SocinatorAccountBuilder.Instance(dominatorAccountModel.AccountBaseModel.AccountId)
                    .AddOrUpdateDominatorAccountBase(dominatorAccountModel.AccountBaseModel)
                    .SaveToBinFile();
            }

            #endregion Rd Login 
        }
    }
}
