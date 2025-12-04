using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.BusinessLogic.ActivitiesWorkflow;
using DominatorHouseCore.DatabaseHandler.TumblrTables.Account;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Extensions;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Request;
using DominatorHouseCore.Utility;
using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using TumblrDominatorCore.Interface;
using TumblrDominatorCore.Models;
using TumblrDominatorCore.TmblrUtility;
using TumblrDominatorCore.TumblrLibrary.TumblrFunction;
using TumblrDominatorCore.TumblrLibrary.TumblrFunction.TumblrBrowserManager;
using TumblrDominatorCore.TumblrRequest;
using TumblrDominatorCore.TumblrResponseHandler;
using Unity;

namespace TumblrDominatorCore.TumblrLibrary.TumblrProcesses
{
    public interface ITumblrLoginProcess : ILoginProcessAsync
    {
        DominatorAccountModel DominatorAccountModel { get; }

        ITumblrBrowserManager _browser { set; get; }
        Task UpdateAccountAsync(DominatorAccountModel accountModel, CancellationToken cancellationToken);
        Task<bool> CheckAutomationLogin(DominatorAccountModel dominatorAccount, CancellationToken cancellationToken, LoginType loginType = LoginType.AutomationLogin);
    }

    public class LoginProcess : ITumblrLoginProcess
    {
        private readonly ITumblrHttpHelper HttpHelper;
        private readonly ITumblrFunct tumblrFucntion;
        private readonly ITumblrAccountSession tumblrAccountSession;
        public LoginProcess(ITumblrBrowserManager browser, ITumblrFunct tumblrFunct, ITumblrHttpHelper _httpHelper, ITumblrAccountSession accountSession)
        {
            tumblrFucntion = tumblrFunct;
            HttpHelper = _httpHelper;
            _browser = browser;
            tumblrAccountSession = accountSession;
        }
        public DominatorAccountModel DominatorAccountModel { get; }
        public ITumblrBrowserManager _browser { get; set; }
        bool isLogin = false;
        LogInResponseHandler logInResponse;
        UserInfoResponeHandler userinfo;
        public bool CheckLogin(DominatorAccountModel dominatorAccountModel, CancellationToken cancellationToken)
        {
            return CheckLoginAsync(dominatorAccountModel, cancellationToken).Result;
        }
        public async Task<bool> CheckLoginAsync(DominatorAccountModel dominatorAccountModel,
            CancellationToken cancellationToken, bool displayLoginMsg = false, LoginType loginType = LoginType.AutomationLogin)
        {
            bool isalternativelyLoggedIn = false;
            try
            {
                tumblrAccountSession.AddOrUpdateSession(ref dominatorAccountModel);
                if (dominatorAccountModel.IsRunProcessThroughBrowser)
                {
                    LoginWithBrowserMethod(dominatorAccountModel, cancellationToken);
                    _browser.CloseBrowser(dominatorAccountModel);
                }
                else if (!dominatorAccountModel.IsRunProcessThroughBrowser && dominatorAccountModel.Cookies.Count == 0)
                    await LoginWithAlternativeMethodAsync(dominatorAccountModel, cancellationToken);
                else
                    await LoginWithDataBaseCookiesAsync(dominatorAccountModel, false, cancellationToken);
                if (logInResponse != null && logInResponse.ToString().Contains("Invalid username and password combination"))
                {
                    dominatorAccountModel.AccountBaseModel.Status = AccountStatus.InvalidCredentials;
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                                        dominatorAccountModel.AccountBaseModel.AccountNetwork,
                                        dominatorAccountModel.AccountBaseModel.UserName, "Login", "Wrong Credentials. Please enter the correct Credentials and try again.");
                    return false;
                }
                if ((!dominatorAccountModel.IsRunProcessThroughBrowser && logInResponse.Response.Response.Contains("\"queryKey\":[\"user-info\",false]"))
                    || (!dominatorAccountModel.IsRunProcessThroughBrowser && logInResponse.Response.Response.Contains("\"isLoggedIn\":false"))
                    || (!dominatorAccountModel.IsRunProcessThroughBrowser && logInResponse != null && !logInResponse.IsLoggedIn))
                {
                    await LoginWithAlternativeMethodAsync(dominatorAccountModel, cancellationToken);
                    isalternativelyLoggedIn = true;
                }

                if (logInResponse != null && !logInResponse.IsLoggedIn)
                {
                    if (dominatorAccountModel.AccountBaseModel.Status == AccountStatus.ProfileSuspended)
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage,
                                    dominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    dominatorAccountModel.AccountBaseModel.UserName, "Login", "Failed To Login As profile Suspended");
                        return logInResponse.IsLoggedIn;
                    }
                    dominatorAccountModel.AccountBaseModel.Status = AccountStatus.Failed;
                    dominatorAccountModel.IsUserLoggedIn = false;
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                                    dominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    dominatorAccountModel.AccountBaseModel.UserName, "Login", "Failed To Login");
                    return logInResponse.IsLoggedIn;
                }
                if (logInResponse != null && logInResponse.IsLoggedIn &&
                        (logInResponse.Response.Response.Contains("\"queryKey\":[\"user-info\",true]") || logInResponse != null && logInResponse.Response.Response.Contains("\"status\":200,\"msg\":\"OK\"},\"response\":{\"user\"")))
                {
                    dominatorAccountModel.AccountBaseModel.Status = AccountStatus.Success;
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                                    dominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    dominatorAccountModel.AccountBaseModel.UserName, "Account Status", "User Successful to Login");
                }
                UpdateAccountModel(logInResponse, dominatorAccountModel, isalternativelyLoggedIn);
                if (dominatorAccountModel.IsUserLoggedIn)
                    tumblrAccountSession.AddOrUpdateSession(ref dominatorAccountModel, true);
                UpdateAccountDetailToDatabase(dominatorAccountModel, cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception)
            {
            }

            return dominatorAccountModel.IsUserLoggedIn;
        }


        public void LoginWithAlternativeMethod(DominatorAccountModel dominatorAccountModel,
            CancellationToken cancellationToken)
        {
            LoginWithAlternativeMethodAsync(dominatorAccountModel, cancellationToken).Wait(cancellationToken);
        }
        public void LoginWithDataBaseCookies(DominatorAccountModel dominatorAccountModel, bool isMobileRequired,
            CancellationToken cancellationToken)
        {
            CheckLogin(dominatorAccountModel, cancellationToken);
        }

        public async Task UpdateAccountAsync(DominatorAccountModel accountModel, CancellationToken cancellationToken)
        {
            try
            {

                if (!accountModel.IsUserLoggedIn)
                    CheckLogin(accountModel, cancellationToken);
                await Task.Factory.StartNew(() =>
                {
                    MainPageResponseHandler mainpageHandler = null;
                    var apiResponse = tumblrFucntion.GetApiResponse(accountModel, ConstantHelpDetails.GetUserStarterInfo(), ConstantHelpDetails.BearerToken);
                    if (apiResponse != null && apiResponse.Success)
                        mainpageHandler = new MainPageResponseHandler(apiResponse.Response);
                    UpdateBlogs(mainpageHandler, accountModel, cancellationToken);

                });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void LoginWithBrowserMethod(DominatorAccountModel dominatorAccountModel,
            CancellationToken cancellationToken, VerificationType verificationType = 0
            , LoginType loginType = LoginType.AutomationLogin)
        {
            _browser.BrowserLogin(dominatorAccountModel, cancellationToken);
            var resp = _browser.GetCurrentPageResponse(dominatorAccountModel);
            var Response = new ResponseParameter() { Response = resp };
            logInResponse = new LogInResponseHandler(Response);
            var csrf = Utilities.GetBetween(logInResponse?.Response?.Response, "\"csrfToken\":\"", "\"");
            tumblrFucntion.Csrf_Token = !string.IsNullOrEmpty(csrf) ? csrf : tumblrFucntion.Csrf_Token;
            userinfo = new UserInfoResponeHandler(logInResponse?.Response);
            dominatorAccountModel.CrmUuid = userinfo.TumblrUser?.Uuid;
            dominatorAccountModel.UserUuid = userinfo.TumblrUser?.UserUuid;
            dominatorAccountModel.AccountBaseModel.UserId = string.IsNullOrEmpty(dominatorAccountModel.AccountBaseModel.UserId) ? userinfo.TumblrUser?.UserId : dominatorAccountModel.AccountBaseModel.UserId;
            tumblrFucntion.Participant_key = userinfo.TumblrUser?.Uuid;
            tumblrAccountSession.AddOrUpdateSession(ref dominatorAccountModel, true);
        }



        public async Task LoginWithDataBaseCookiesAsync(DominatorAccountModel dominatorAccountModel, bool isMobileRequired,
            CancellationToken cancellationToken)
        {
            await Task.Run(() => logInResponse = tumblrFucntion.LogIn(dominatorAccountModel));
            var csrf = Utilities.GetBetween(logInResponse.Response.Response, "\"csrfToken\":\"", "\"");
            tumblrFucntion.Csrf_Token = !string.IsNullOrEmpty(csrf) ? csrf : tumblrFucntion.Csrf_Token;
            logInResponse = new LogInResponseHandler(logInResponse.Response);
            userinfo = new UserInfoResponeHandler(logInResponse.Response);
            dominatorAccountModel.CrmUuid = userinfo.TumblrUser?.Uuid;
            dominatorAccountModel.AccountBaseModel.UserId = string.IsNullOrEmpty(dominatorAccountModel.AccountBaseModel.UserId) ? userinfo.TumblrUser?.UserId : dominatorAccountModel.AccountBaseModel.UserId;
            dominatorAccountModel.UserUuid = userinfo.TumblrUser?.UserUuid;
            tumblrFucntion.Participant_key = userinfo.TumblrUser?.Uuid;

        }
        public async Task LoginWithAlternativeMethodAsync(DominatorAccountModel dominatorAccountModel,
            CancellationToken cancellationToken)
        {
            var accountScopeFactory = InstanceProvider.GetInstance<IAccountScopeFactory>();
            var browserManager = accountScopeFactory[$"{dominatorAccountModel.AccountId}_HiddenLogin"]
                .Resolve<ITumblrBrowserManager>();
            try
            {
                await Task.Run(() => browserManager.BrowserLogin(dominatorAccountModel, cancellationToken, LoginType.HiddenLogin));
                if (dominatorAccountModel.IsUserLoggedIn)
                {
                    //browserManager.GetCurrentPageResponse(dominatorAccount);
                    var resp = browserManager.LasAutomationLoginResponse;
                    var Response = new ResponseParameter() { Response = resp };
                    logInResponse = new LogInResponseHandler(Response);
                    var csrf = Utilities.GetBetween(logInResponse.Response.Response, "\"csrfToken\":\"", "\"");
                    tumblrFucntion.Csrf_Token = !string.IsNullOrEmpty(csrf) ? csrf : tumblrFucntion.Csrf_Token;
                    userinfo = new UserInfoResponeHandler(logInResponse.Response);
                    dominatorAccountModel.CrmUuid = userinfo.TumblrUser?.Uuid;
                    dominatorAccountModel.UserUuid = userinfo.TumblrUser?.UserUuid;
                    dominatorAccountModel.AccountBaseModel.UserId = string.IsNullOrEmpty(dominatorAccountModel.AccountBaseModel.UserId) ? userinfo.TumblrUser?.UserId : dominatorAccountModel.AccountBaseModel.UserId;
                    tumblrFucntion.Participant_key = userinfo.TumblrUser?.Uuid;
                }
            }
            finally
            {
                if (browserManager != null && browserManager.BrowserWindow != null)
                    browserManager.CloseBrowser(dominatorAccountModel);
            }

        }

        private void UpdateAccountDetailToDatabase(DominatorAccountModel dominatorAccountModel,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var globalDbOperation = new DbOperations(SocinatorInitialize.GetGlobalDatabase().GetSqlConnection());
            globalDbOperation.UpdateAccountDetails(dominatorAccountModel);
        }

        public void UpdateAccountModel(MainPageResponseHandler mainPageResponseHandler,
            DominatorAccountModel dominatorAccountModel, bool isalternativelyLogginIn = false)
        {
            try
            {
                if (mainPageResponseHandler == null) return;

                var getHtml = mainPageResponseHandler.ToString();

                if (!string.IsNullOrEmpty(getHtml))
                {
                    var loginreqparam = HttpHelper.GetRequestParameter();
                    #region Updating Cookies
                    if (!isalternativelyLogginIn)
                    {

                        if (loginreqparam.Cookies?.Count > 0
                            && loginreqparam.Cookies.Cast<Cookie>().Any(x => x.Name.Contains("logged_in") && x.Value == "1"))
                            dominatorAccountModel.Cookies = loginreqparam.Cookies;
                        else if (loginreqparam.Cookies?.Count > 0
                             && loginreqparam.Cookies.Cast<Cookie>().Any(x => x.Name.Contains("logged_in") && x.Value == "0")
                             && dominatorAccountModel.BrowserCookieHelperList.Any(x => x.Name.Contains("logged_in") && x.Value == "1"))
                            dominatorAccountModel.Cookies = dominatorAccountModel.BrowserCookies;
                    }
                    else if (isalternativelyLogginIn && !dominatorAccountModel.IsRunProcessThroughBrowser)
                        loginreqparam.Cookies = dominatorAccountModel.Cookies;

                    if ((dominatorAccountModel.BrowserCookies.Count == 0 || dominatorAccountModel.BrowserCookieHelperList.Any(x => x.Name.Contains("logged_in") && x.Value == "0"))
                        && dominatorAccountModel.CookieHelperList.Any(x => x.Name.Contains("logged_in") && x.Value == "1"))
                        dominatorAccountModel.BrowserCookies = dominatorAccountModel.Cookies;
                    #endregion
                    dominatorAccountModel.AccountBaseModel.Status = AccountStatus.Success;
                    dominatorAccountModel.IsUserLoggedIn = true;
                    dominatorAccountModel.AccountBaseModel.UserId = mainPageResponseHandler.UserContextId;

                    dominatorAccountModel.DisplayColumnValue1 = mainPageResponseHandler.LoginJsonResponse.Context
                        .userinfo.channels[0]
                        ?.follower_count; //mainPageResponseHandler.loginJsonResponse.Context.userinfo.channels[0]?.follower_count;
                    dominatorAccountModel.DisplayColumnValue2 = mainPageResponseHandler.LoginJsonResponse.Context
                        .userinfo.channels[0]?.post_count;
                    dominatorAccountModel.DisplayColumnValue4 =
                        mainPageResponseHandler.LoginJsonResponse.Context.userinfo?.friend_count;
                    dominatorAccountModel.DisplayColumnValue3 =
                        mainPageResponseHandler.LoginJsonResponse.Context.userinfo?.channels
                            .Count;

                    if (dominatorAccountModel.DisplayColumnValue2 != null)
                        if (dominatorAccountModel.DisplayColumnValue1 != null)
                        {
                            //tumblrAccountUpdate.AddToDailyGrowth(dominatorAccountModel.AccountId,
                            //    dominatorAccountModel.DisplayColumnValue2.Value,
                            //    dominatorAccountModel.DisplayColumnValue3.Value,
                            //    dominatorAccountModel.DisplayColumnValue1.Value,
                            //    dominatorAccountModel.DisplayColumnValue4.Value);
                            var accountModel = new AccountModel(dominatorAccountModel)
                            {
                                TumblrFormKey = mainPageResponseHandler.TumblrFormKey,
                                FollowingCount =
                                    (int)mainPageResponseHandler.LoginJsonResponse.Context.userinfo?.friend_count,
                                FollowersCount =
                                    mainPageResponseHandler.LoginJsonResponse.Context.userinfo.channels[0]
                                        ?.follower_count ??
                                    0,
                                PostsCount =
                                    mainPageResponseHandler.LoginJsonResponse.Context.userinfo.channels[0]
                                        ?.post_count ??
                                    0
                            };

                            accountModel.TumblrFormKey = mainPageResponseHandler.TumblrFormKey;
                            accountModel.BlogCount =
                                mainPageResponseHandler.LoginJsonResponse.Context.userinfo.channels.Count;


                            dominatorAccountModel.Token.ThrowIfCancellationRequested();
                            // ReSharper disable once UnusedVariable
                            var socinatorAccountBuilder =
                                SocinatorAccountBuilder.Instance(dominatorAccountModel.AccountBaseModel.AccountId)
                                    .AddOrUpdateDominatorAccountBase(dominatorAccountModel.AccountBaseModel)
                                    .AddOrUpdateLoginStatus(true)
                                    .AddOrUpdateDisplayColumn1(dominatorAccountModel.DisplayColumnValue1)
                                    .AddOrUpdateDisplayColumn2(dominatorAccountModel.DisplayColumnValue2)
                                    .AddOrUpdateDisplayColumn3(dominatorAccountModel.DisplayColumnValue3)
                                    .AddOrUpdateDisplayColumn4(dominatorAccountModel.DisplayColumnValue4)
                                    .AddOrUpdateCookies(dominatorAccountModel.Cookies)
                                    .AddOrUpdateBrowserCookies(dominatorAccountModel.BrowserCookies)
                                    .SaveToBinFile();
                        }
                }
                else if (string.IsNullOrEmpty(getHtml))
                {
                    dominatorAccountModel.AccountBaseModel.Status = AccountStatus.ProxyNotWorking;
                    dominatorAccountModel.IsUserLoggedIn = false;
                    SocinatorAccountBuilder.Instance(dominatorAccountModel.AccountBaseModel.AccountId)
                        .AddOrUpdateDominatorAccountBase(dominatorAccountModel.AccountBaseModel)
                        .SaveToBinFile();
                }
                else
                {
                    dominatorAccountModel.AccountBaseModel.Status = AccountStatus.Failed;
                    dominatorAccountModel.IsUserLoggedIn = false;
                    SocinatorAccountBuilder.Instance(dominatorAccountModel.AccountBaseModel.AccountId)
                        .AddOrUpdateDominatorAccountBase(dominatorAccountModel.AccountBaseModel)
                        .SaveToBinFile();
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
        }




        public void UpdateBlogs(MainPageResponseHandler mainresp, DominatorAccountModel accountModel,
            CancellationToken cancellationToken)
        {
            #region UpdateBlogs
            try
            {
                var db = new DbOperations(accountModel.AccountId, SocialNetworks.Tumblr, ConstantVariable.GetAccountDb);
                var blogChannel = mainresp.LoginJsonResponse.Context.userinfo.channels.ToList();

                while (blogChannel.Count != 0)
                    try
                    {
                        foreach (var item in blogChannel)
                        {
                            var interactedblog = db.GetSingle<OwnBlogs>(x => x.BlogName == item.directory_safe_title);
                            if (interactedblog != null)
                            {
                            }
                            else
                            {
                                var usersPostsInfo
                                    = new OwnBlogs
                                    {
                                        BlogName = item.directory_safe_title,
                                        BlogKey = item.mention_key,
                                        BlogUrl = item.blog_url,
                                        Postcount = item.post_count
                                    };
                                db.Add(usersPostsInfo);
                            }
                        }

                        break;
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            #endregion
        }

        public async Task<bool> CheckAutomationLogin(DominatorAccountModel dominatorAccount, CancellationToken cancellationToken, LoginType loginType = LoginType.AutomationLogin)
        {
            try
            {
                if (!dominatorAccount.IsRunProcessThroughBrowser)
                {
                    tumblrAccountSession.AddOrUpdateSession(ref dominatorAccount);
                    logInResponse = tumblrFucntion.LogIn(dominatorAccount);
                    if (dominatorAccount.IsUserLoggedIn)
                    {
                        tumblrAccountSession.AddOrUpdateSession(ref dominatorAccount, true);
                        return true;
                    }
                    await LoginWithAlternativeMethodAsync(dominatorAccount, cancellationToken);
                    tumblrAccountSession.AddOrUpdateSession(ref dominatorAccount, true);
                    UpdateAccountModel(logInResponse, dominatorAccount, true);
                }
                else
                    LoginWithBrowserMethod(dominatorAccount, cancellationToken);

            }
            catch (Exception) { }
            return dominatorAccount.IsUserLoggedIn;
        }
    }
}