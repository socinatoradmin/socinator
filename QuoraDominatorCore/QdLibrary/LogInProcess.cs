using DominatorHouseCore;
using DominatorHouseCore.BusinessLogic.ActivitiesWorkflow;
using DominatorHouseCore.DatabaseHandler.DHTables;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Extensions;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Request;
using DominatorHouseCore.Utility;
using QuoraDominatorCore.Interface;
using QuoraDominatorCore.QDFactories;
using QuoraDominatorCore.QdLibrary.QdFunctions;
using QuoraDominatorCore.QdUtility;
using QuoraDominatorCore.Request;
using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QuoraDominatorCore.QdLibrary
{
    public interface IQdLogInProcess : ILoginProcessAsync
    {
        IQDBrowserManagerFactory managerFactory { get; set; }
        Task LoginUsingGlobusHttpQuoraAsync(DominatorAccountModel dominatorAccountModel,
            CancellationToken cancellation,bool NeedToShowLog = true);

        Task LoginForAdsScrapingAsync(DominatorAccountModel account, CancellationToken cancellatrionToken);
        void LoginUsingGlobusHttpQuora(DominatorAccountModel dominatorAccountModel, CancellationToken cancellation, bool NeedToShowLog = true);
    }

    public class LogInProcess : IQdLogInProcess
    {
        // lock updating account details for verification
        public static readonly object LockFile = new object();

        // lock update global account details
        public static readonly object LockUpdateGlobalAccountDetails = new object();
        public static readonly object LockUpdateAccountDetails = new object();
        private IQdHttpHelper _httpHelper;
        private IQuoraFunctions quoraFunctions;
        private IQDSessionManager sessionManager;
        public IQDBrowserManagerFactory managerFactory { get; set; }
        public LogInProcess(IQuoraBrowserManager browser, IQdHttpHelper httpHelper, IQuoraFunctions QuoraFunctions, IQDSessionManager qDSessionManager,
            IQDBrowserManagerFactory factory)
        {
            _httpHelper = httpHelper;
            quoraFunctions = QuoraFunctions;
            sessionManager = qDSessionManager;
            managerFactory = factory;
        }

        #region LogIn

        public void LoginWithDataBaseCookies(DominatorAccountModel dominatorAccountModel, bool isMobileRequired,
            CancellationToken cancellationToken)
        {
            LoginWithDataBaseCookiesAsync(dominatorAccountModel, isMobileRequired, dominatorAccountModel.Token).Wait();
        }

        private async Task CheckisLoggedInAsync(DominatorAccountModel dominatorAccountModel, CancellationToken token)
        {
            try
            {                
                dominatorAccountModel.AccountBaseModel.Status = AccountStatus.TryingToLogin;
                var RequestParam = _httpHelper.GetRequestParameter();
                RequestParam.Cookies = dominatorAccountModel.Cookies;
                _httpHelper.SetRequestParameter(RequestParam);
                token.ThrowIfCancellationRequested();
                var dataBeforelogin = (await _httpHelper.GetRequestAsync("https://www.quora.com", token)).Response;
                token.ThrowIfCancellationRequested();
                if (dataBeforelogin.Contains("Add Question") || dataBeforelogin.Contains("isLoggedIn\": true"))
                {
                    sessionManager.AddOrUpdateSession(ref dominatorAccountModel,true);
                    dominatorAccountModel.IsUserLoggedIn = true;
                    dominatorAccountModel.AccountBaseModel.Status = AccountStatus.Success;
                    GlobusLogHelper.log.Info(Log.SuccessfulLogin, dominatorAccountModel.AccountBaseModel.AccountNetwork,
                        dominatorAccountModel.AccountBaseModel.UserName);
                    return;
                }

                if (dataBeforelogin.Contains("Proxy is Not Working"))
                {
                    dominatorAccountModel.AccountBaseModel.Status = AccountStatus.ProxyNotWorking;
                    GlobusLogHelper.log.Info(Log.LoginFailed, dominatorAccountModel.AccountBaseModel.AccountNetwork,
                        dominatorAccountModel.AccountBaseModel.UserName, " proxy is not Working !!");
                }

                if (string.IsNullOrEmpty(dataBeforelogin))
                {
                    token.ThrowIfCancellationRequested();
                    dataBeforelogin = (await _httpHelper.GetRequestAsync(QdConstants.HomePageUrl, token)).Response;

                    if (!dataBeforelogin.Contains(dominatorAccountModel.AccountBaseModel.UserName.ToLower()))
                    {
                        dominatorAccountModel.IsUserLoggedIn = false;
                    }
                    else
                    {
                        dominatorAccountModel.IsUserLoggedIn = true;
                        sessionManager.AddOrUpdateSession(ref dominatorAccountModel, true);
                        dominatorAccountModel.AccountBaseModel.Status = AccountStatus.Success;
                        GlobusLogHelper.log.Info(Log.SuccessfulLogin,
                            dominatorAccountModel.AccountBaseModel.AccountNetwork,
                            dominatorAccountModel.AccountBaseModel.UserName);
                    }
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }


        public void LoginUsingGlobusHttpQuora(DominatorAccountModel dominatorAccountModel,
            CancellationToken cancellationToken,bool NeedToShowLog=true)
        {
            LoginUsingGlobusHttpQuoraAsync(dominatorAccountModel, cancellationToken,NeedToShowLog).Wait();
        }

        public async Task LoginUsingGlobusHttpQuoraAsync(DominatorAccountModel dominatorAccountModel,
            CancellationToken cancellationToken, bool NeedToShowLog = true)
        {
            try
            {
                sessionManager.AddOrUpdateSession(ref dominatorAccountModel);
                if (NeedToShowLog)
                    GlobusLogHelper.log.Info(Log.AccountLogin, dominatorAccountModel.AccountBaseModel.AccountNetwork,dominatorAccountModel.AccountBaseModel.UserName);
                dominatorAccountModel.AccountBaseModel.Status = AccountStatus.TryingToLogin;
                var objRequestParametersWeb = new RequestParameters();
                try
                {
                    var objWebHeaderCollectionWeb = new WebHeaderCollection
                    {
                        {
                            "User-Agent",
                            "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/80.0.3987.122 Safari/537.36"
                        }
                    };
                    objRequestParametersWeb.KeepAlive = true;
                    objRequestParametersWeb.ContentType = "application/x-www-form-urlencoded";
                    objRequestParametersWeb.Accept =
                        "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9";
                    objWebHeaderCollectionWeb.Add("Accept-Language", "en-us,en;q=0.5");
                    objRequestParametersWeb.Headers = objWebHeaderCollectionWeb;
                    dominatorAccountModel.UserAgentWeb =
                        "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/55.0.2883.87 Safari/537.36";
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

                    try
                    {
                        objRequestParametersWeb.Cookies = new CookieCollection();
                        objRequestParametersWeb.Cookies = dominatorAccountModel.Cookies;
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                    _httpHelper.SetRequestParameter(objRequestParametersWeb);
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
                dominatorAccountModel.Token.ThrowIfCancellationRequested();
                var responseparameter =
                    await _httpHelper.GetRequestAsync($"{QdConstants.HomePageUrl}/", dominatorAccountModel.Token);

                try
                {
                    if (responseparameter.Exception != null &&
                        responseparameter.Exception.ToString()
                            .Contains("The remote name could not be resolved: 'www.quora.com'") &&
                        responseparameter.Exception.InnerException == null)
                    {
                        dominatorAccountModel.AccountBaseModel.Status = AccountStatus.Failed;
                        GlobusLogHelper.log.Info(Log.LoginFailed, dominatorAccountModel.AccountBaseModel.AccountNetwork,
                            dominatorAccountModel.AccountBaseModel.UserName,
                            "Please check your Internet connection and try again !!");
                        try
                        {
                            dominatorAccountModel.Token.ThrowIfCancellationRequested();
                            SocinatorAccountBuilder.Instance(dominatorAccountModel.AccountBaseModel.AccountId)
                                .AddOrUpdateDominatorAccountBase(dominatorAccountModel.AccountBaseModel)
                                .SaveToBinFile();
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

                        UpdateAccountDetailToDatabase(dominatorAccountModel, cancellationToken);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                try
                {
                    if (responseparameter.Exception != null && responseparameter.Exception.InnerException != null &&
                        (responseparameter.Exception.InnerException == null && responseparameter.Exception
                             .InnerException.ToString()
                             .Contains("connection failed because connected host has failed to respond") ||
                         responseparameter.Exception.ToString().Contains("Unable to connect to the remote server")))
                    {
                        dominatorAccountModel.AccountBaseModel.Status = AccountStatus.ProxyNotWorking;
                        GlobusLogHelper.log.Info(Log.LoginFailed, dominatorAccountModel.AccountBaseModel.AccountNetwork,
                            dominatorAccountModel.AccountBaseModel.UserName, " proxy is not Working !!");
                        try
                        {
                            dominatorAccountModel.Token.ThrowIfCancellationRequested();
                            SocinatorAccountBuilder.Instance(dominatorAccountModel.AccountBaseModel.AccountId)
                                .AddOrUpdateDominatorAccountBase(dominatorAccountModel.AccountBaseModel)
                                .SaveToBinFile();
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

                        UpdateAccountDetailToDatabase(dominatorAccountModel, cancellationToken);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                var requestParameter = _httpHelper.GetRequestParameter();
                if (dominatorAccountModel.Cookies.Count == 0 && requestParameter.Cookies.Count > 0)
                    dominatorAccountModel.Cookies = requestParameter.Cookies;
                //QdUtilities.RemoveUnUsedCookies(ref dominatorAccountModel);
                objRequestParametersWeb.Cookies = dominatorAccountModel.Cookies;
                _httpHelper.SetRequestParameter(requestParameter);
                var responsaeparameter =
                    await _httpHelper.GetRequestAsync("https://www.quora.com/", dominatorAccountModel.Token);

                if (!(responsaeparameter.Response.Contains("What do you want to ask or share?") || responsaeparameter.Response.Contains("What is your question or link?") || responsaeparameter.Response.Contains("isLoggedIn\": true")))
                {
                    #region OLD Login Logic.
                    //var formkey = Utilities.GetBetween(responseparameter.Response, "\"formkey\":", ","); //isLoggedIn
                    //formkey = formkey.Replace(" ", "").Replace("\"", "").Replace(",", "");
                    //var postkey = Utilities.GetBetween(responseparameter.Response, "postkey\": \"", "\",");
                    //var windowId = Utilities.GetBetween(responseparameter.Response, "windowId\": \"", "\"");
                    //var passWord = dominatorAccountModel.AccountBaseModel.Password;
                    //passWord = Uri.EscapeDataString(passWord);
                    //var revision = Utilities.GetBetween(responseparameter.Response, "revision\": \"", "\"");
                    //var postdata = "json=%7B%22args%22%3A%5B%5D%2C%22kwargs%22%3A%7B%22email%22%3A%22" +
                    //               dominatorAccountModel.AccountBaseModel.UserName.Replace("@", "%40") +
                    //               "%22%2C%22password%22%3A%22" + passWord +
                    //               "%22%7D%7D&revision=" + revision + "&formkey=" + formkey + "&postkey=" + postkey +
                    //               "&window_id=" + windowId +
                    //               "&referring_controller=index&referring_action=index&__hmac=Vn03YsuKFZvHV9&__method=do_login&__e2e_action_id=f3wl10lkdb&js_init=%7B%7D&__metadata=%7B%7D";

                    //dominatorAccountModel.Token.ThrowIfCancellationRequested();
                    //var resp = (await _httpHelper.PostRequestAsync("https://www.quora.com/webnode2/server_call_POST",
                    //    postdata, dominatorAccountModel.Token)).Response;
                    #endregion
                    var CaptchaTokenString = Utilities.GetBetween(responsaeparameter.Response, "\"rCaptchaToken\":\"", "\"");
                    await quoraFunctions.SetGeneralHeaders(_httpHelper.GetRequestParameter(), dominatorAccountModel, string.Empty, quoraFunctions.GetBasePostData(string.Empty, dominatorAccountModel, responsaeparameter.Response));
                    var LoginResult = await _httpHelper.PostRequestAsync(QdConstants.LoginAPI, Encoding.UTF8.GetBytes(QdConstants.LoginPostBody(dominatorAccountModel.AccountBaseModel.UserName,dominatorAccountModel.AccountBaseModel.Password,CaptchaTokenString)),dominatorAccountModel.Token);
                    var LoginResponse=LoginResult.Response;
                    if (LoginResponse.Contains("{\"value\": ") && LoginResponse.Contains(", \"pmsg\": null") || LoginResponse.Contains("{\"loginDo\": {\"success\": true"))
                    {
                        responsaeparameter = await _httpHelper.GetRequestAsync("https://www.quora.com/", dominatorAccountModel.Token);
                        var UserId= Utilities.GetBetween(LoginResponse, "\"value\": ", ",");
                        dominatorAccountModel.AccountBaseModel.UserId = string.IsNullOrEmpty(UserId) ?quoraFunctions.GetUserId("https://www.quora.com/"): UserId;
                        dominatorAccountModel.IsUserLoggedIn = true;
                        dominatorAccountModel.AccountBaseModel.Status = AccountStatus.Success;
                        _httpHelper.GetRequestParameter().Cookies.Cast<Cookie>().ForEach(x => {
                            if (!string.IsNullOrEmpty(x.Domain) && !x.Domain.StartsWith(".quora.com")) x.Domain = ".quora.com";
                        });
                        if (_httpHelper.GetRequestParameter().Cookies != null && _httpHelper.GetRequestParameter().Cookies.Count > 0)
                            dominatorAccountModel.Cookies = _httpHelper.GetRequestParameter().Cookies;
                        sessionManager.AddOrUpdateSession(ref dominatorAccountModel, true);
                        if (NeedToShowLog)
                            GlobusLogHelper.log.Info(Log.SuccessfulLogin,dominatorAccountModel.AccountBaseModel.AccountNetwork,dominatorAccountModel.AccountBaseModel.UserName);
                    }
                    else if (LoginResponse.Contains("exception") || LoginResponse.Contains("errorType"))
                    {
                        try
                        {
                            var field = Utilities.GetBetween(LoginResponse, "[", "]");
                            field = string.IsNullOrEmpty(field) ?Utilities.GetBetween(LoginResponse, "\"errorType\": \"", "\""): field;
                            if (field.Contains("phone_number") && field.Contains("email"))
                            {
                                dominatorAccountModel.AccountBaseModel.Status = AccountStatus.NeedsVerification;
                                GlobusLogHelper.log.Info(Log.LoginFailed,
                                    dominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    dominatorAccountModel.AccountBaseModel.UserName,
                                    " verification via Phone Number or Email require!!");
                            }
                            else if (field.Contains("phone_number"))
                            {
                                dominatorAccountModel.AccountBaseModel.Status = AccountStatus.PhoneVerification;
                                GlobusLogHelper.log.Info(Log.LoginFailed,
                                    dominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    dominatorAccountModel.AccountBaseModel.UserName,
                                    " verification via Phone Number require!!");
                            }
                            else if (field.Contains("email_not_found"))
                            {
                                dominatorAccountModel.AccountBaseModel.Status = AccountStatus.InvalidCredentials;
                                GlobusLogHelper.log.Info(Log.LoginFailed,
                                    dominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    dominatorAccountModel.AccountBaseModel.UserName, " email not found!!");
                            }
                            else if (field.Contains("email"))
                            {
                                dominatorAccountModel.AccountBaseModel.Status = AccountStatus.EmailVerification;
                                GlobusLogHelper.log.Info(Log.LoginFailed,
                                    dominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    dominatorAccountModel.AccountBaseModel.UserName,
                                    " verification via Email require!!");
                            }

                            else if (LoginResponse.Contains("authenticated\": false"))
                            {
                                dominatorAccountModel.AccountBaseModel.Status = AccountStatus.InvalidCredentials;
                                GlobusLogHelper.log.Info(Log.LoginFailed,
                                    dominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    dominatorAccountModel.AccountBaseModel.UserName, " invalid Credentials!!");
                            }
                            else if (field.Contains("incorrect_password"))
                            {
                                dominatorAccountModel.AccountBaseModel.Status = AccountStatus.InvalidCredentials;
                                GlobusLogHelper.log.Info(Log.LoginFailed,
                                    dominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    dominatorAccountModel.AccountBaseModel.UserName, " incorrect password!!");
                            }
                            else if (field.Contains("two_factor_authentication"))
                            {
                                dominatorAccountModel.AccountBaseModel.Status = AccountStatus.TwoFactorLoginAttempt;
                                GlobusLogHelper.log.Info(Log.LoginFailed,
                                    dominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    dominatorAccountModel.AccountBaseModel.UserName, "two factor authentication");
                            }else if (field.Contains("failed_captcha"))
                            {
                                dominatorAccountModel.AccountBaseModel.Status = AccountStatus.FoundCaptcha;
                                GlobusLogHelper.log.Info(Log.LoginFailed,
                                    dominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    dominatorAccountModel.AccountBaseModel.UserName, "Found Captcha Verification");
                            }
                            else
                            {
                                dominatorAccountModel.AccountBaseModel.Status = AccountStatus.Failed;
                                GlobusLogHelper.log.Info(Log.LoginFailed,
                                    dominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    dominatorAccountModel.AccountBaseModel.UserName, " failed!!");
                            }
                        }
                        catch (Exception ex)
                        {
                            if (LoginResponse == "")
                            {
                                dominatorAccountModel.AccountBaseModel.Status = AccountStatus.Failed;
                                GlobusLogHelper.log.Info(Log.LoginFailed,
                                    dominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    dominatorAccountModel.AccountBaseModel.UserName, " failed!!");
                            }

                            ex.DebugLog();
                        }
                    }
                    else if (LoginResponse.Contains("Error 403"))
                    {
                        dominatorAccountModel.AccountBaseModel.Status = AccountStatus.Failed;
                        GlobusLogHelper.log.Info(Log.LoginFailed, dominatorAccountModel.AccountBaseModel.AccountNetwork,
                            dominatorAccountModel.AccountBaseModel.UserName, " Error 403 (Forbidden error)");
                    }

                    if (LoginResponse == "")
                    {
                        dominatorAccountModel.AccountBaseModel.Status = AccountStatus.Failed;
                        GlobusLogHelper.log.Info(Log.LoginFailed, dominatorAccountModel.AccountBaseModel.AccountNetwork,
                            dominatorAccountModel.AccountBaseModel.UserName, " failed!!");
                    }
                }

                else
                {
                    dominatorAccountModel.IsUserLoggedIn = true;
                    dominatorAccountModel.AccountBaseModel.Status = AccountStatus.Success;
                    sessionManager.AddOrUpdateSession(ref dominatorAccountModel,true);
                    if (NeedToShowLog)
                        GlobusLogHelper.log.Info(Log.SuccessfulLogin, dominatorAccountModel.AccountBaseModel.AccountNetwork,dominatorAccountModel.AccountBaseModel.UserName);
                }
            }
            catch (Exception ex)
            {
                dominatorAccountModel.AccountBaseModel.Status = AccountStatus.Failed;
                GlobusLogHelper.log.Info(Log.LoginFailed, dominatorAccountModel.AccountBaseModel.AccountNetwork,
                    dominatorAccountModel.AccountBaseModel.UserName, " failed!!");
                ex.DebugLog();
            }

            try
            {
                dominatorAccountModel.Token.ThrowIfCancellationRequested();
                SocinatorAccountBuilder.Instance(dominatorAccountModel.AccountBaseModel.AccountId)
                    .AddOrUpdateDominatorAccountBase(dominatorAccountModel.AccountBaseModel)
                    .AddOrUpdateUserAgentWeb(dominatorAccountModel.UserAgentWeb)
                    .AddOrUpdateCookies(dominatorAccountModel.Cookies)
                    .SaveToBinFile();
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

            UpdateAccountDetailToDatabase(dominatorAccountModel, cancellationToken);
        }

        public async Task LoginForAdsScrapingAsync(DominatorAccountModel dominatorAccountModel,
            CancellationToken cancellationToken)
        {
            try
            {
                sessionManager.AddOrUpdateSession(ref dominatorAccountModel);
                var objRequestParametersWeb = new RequestParameters();
                try
                {
                    var objWebHeaderCollectionWeb = new WebHeaderCollection
                    {
                        {
                            "User-Agent",
                            "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/55.0.2883.87 Safari/537.36"
                        }
                    };
                    objRequestParametersWeb.KeepAlive = true;
                    objRequestParametersWeb.ContentType = "application/x-www-form-urlencoded";
                    objRequestParametersWeb.Accept = "application/json, text/javascript, */*; q=0.01";
                    objWebHeaderCollectionWeb.Add("Accept-Language", "en-us,en;q=0.5");
                    objRequestParametersWeb.Headers = objWebHeaderCollectionWeb;
                    dominatorAccountModel.UserAgentWeb =
                        "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/55.0.2883.87 Safari/537.36";
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

                    try
                    {
                        objRequestParametersWeb.Cookies = new CookieCollection();
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                    _httpHelper.SetRequestParameter(objRequestParametersWeb);
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                objRequestParametersWeb.Cookies = new CookieCollection();


                dominatorAccountModel.Token.ThrowIfCancellationRequested();
                var responseparameter =
                    await _httpHelper.GetRequestAsync("https://www.quora.com/", dominatorAccountModel.Token);
                try
                {
                    if (responseparameter.Exception != null &&
                        responseparameter.Exception.ToString()
                            .Contains("The remote name could not be resolved: 'www.quora.com'") &&
                        responseparameter.Exception.InnerException == null)
                    {
                        try
                        {
                            dominatorAccountModel.Token.ThrowIfCancellationRequested();
                            //SocinatorAccountBuilder.Instance(dominatorAccountModel.AccountBaseModel.AccountId)
                            //.AddOrUpdateDominatorAccountBase(dominatorAccountModel.AccountBaseModel)
                            //.SaveToBinFile();
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

                        //UpdateAccountDetailToDatabase(dominatorAccountModel);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                try
                {
                    if (responseparameter.Exception != null && responseparameter.Exception.InnerException != null &&
                        (responseparameter.Exception.InnerException == null && responseparameter.Exception
                             .InnerException.ToString()
                             .Contains("connection failed because connected host has failed to respond") ||
                         responseparameter.Exception.ToString().Contains("Unable to connect to the remote server")))
                    {
                        //dominatorAccountModel.AccountBaseModel.Status = AccountStatus.ProxyNotWorking;
                        try
                        {
                            dominatorAccountModel.Token.ThrowIfCancellationRequested();
                            //SocinatorAccountBuilder.Instance(dominatorAccountModel.AccountBaseModel.AccountId)
                            //.AddOrUpdateDominatorAccountBase(dominatorAccountModel.AccountBaseModel)
                            // .SaveToBinFile();
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

                        //UpdateAccountDetailToDatabase(dominatorAccountModel);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                var requestParameter = _httpHelper.GetRequestParameter();


                foreach (Cookie eachCookie in requestParameter.Cookies)
                    try
                    {
                        dominatorAccountModel.Token.ThrowIfCancellationRequested();
                        if (!eachCookie.Domain.Contains("www.")) eachCookie.Domain = "www" + eachCookie.Domain;
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                dominatorAccountModel.Cookies = requestParameter.Cookies;

                _httpHelper.SetRequestParameter(requestParameter);
                var formkey = Utilities.GetBetween(responseparameter.Response, "\"formkey\":", ","); //isLoggedIn
                formkey = formkey.Replace(" ", "").Replace("\"", "").Replace(",", "");
                var postkey = Utilities.GetBetween(responseparameter.Response, "postkey\": \"", "\",");
                var windowId = Utilities.GetBetween(responseparameter.Response, "windowId\": \"", "\"");
                var passWord = dominatorAccountModel.AccountBaseModel.Password;
                passWord = Uri.EscapeDataString(passWord);
                var revision = Utilities.GetBetween(responseparameter.Response, "revision\": \"", "\"");
                var postdata = "json=%7B%22args%22%3A%5B%5D%2C%22kwargs%22%3A%7B%22email%22%3A%22" +
                               dominatorAccountModel.AccountBaseModel.UserName.Replace("@", "%40") +
                               "%22%2C%22password%22%3A%22" + passWord +
                               "%22%7D%7D&revision=" + revision + "&formkey=" + formkey + "&postkey=" + postkey +
                               "&window_id=" + windowId +
                               "&referring_controller=index&referring_action=index&__hmac=Vn03YsuKFZvHV9&__method=do_login&__e2e_action_id=f3wl10lkdb&js_init=%7B%7D&__metadata=%7B%7D";


                dominatorAccountModel.Token.ThrowIfCancellationRequested();
                var resp = (await _httpHelper.PostRequestAsync("https://www.quora.com/webnode2/server_call_POST",
                    postdata, dominatorAccountModel.Token)).Response;

                if (resp.Contains("{\"value\": ") && resp.Contains(", \"pmsg\": null"))
                {
                    dominatorAccountModel.AccountBaseModel.UserId = Utilities.GetBetween(resp, "\"value\": ", ",");
                    dominatorAccountModel.IsUserLoggedIn = true;
                    sessionManager.AddOrUpdateSession(ref dominatorAccountModel,true);
                    dominatorAccountModel.AccountBaseModel.Status = AccountStatus.Success;
                }
                else if (resp.Contains("exception"))
                {
                    try
                    {
                        var field = Utilities.GetBetween(resp, "[", "]");
                        if (field.Contains("phone_number") && field.Contains("email"))
                        {
                            //dominatorAccountModel.AccountBaseModel.Status = AccountStatus.NeedsVerification;
                        }
                        else if (field.Contains("phone_number"))
                        {
                            //dominatorAccountModel.AccountBaseModel.Status = AccountStatus.PhoneVerification;
                        }
                        else if (field.Contains("email_not_found"))
                        {
                            // dominatorAccountModel.AccountBaseModel.Status = "Email not found";
                            //dominatorAccountModel.AccountBaseModel.Status = AccountStatus.InvalidCredentials;
                        }
                        else if (field.Contains("email"))
                        {
                            //dominatorAccountModel.AccountBaseModel.Status = AccountStatus.EmailVerification;
                        }

                        else if (resp.Contains("authenticated\": false"))
                        {
                            //dominatorAccountModel.AccountBaseModel.Status = AccountStatus.InvalidCredentials;
                        }
                        else if (field.Contains("incorrect_password"))
                        {
                            //dominatorAccountModel.AccountBaseModel.Status = AccountStatus.InvalidCredentials;
                        }
                        else if (field.Contains("two_factor_authentication"))
                        {
                            //dominatorAccountModel.AccountBaseModel.Status = AccountStatus.TwoFactorLoginAttempt;
                        }
                    }
                    catch (Exception ex)
                    {
                        if (resp == "")
                        {
                            //dominatorAccountModel.AccountBaseModel.Status = AccountStatus.Failed;
                        }

                        ex.DebugLog();
                    }
                }

                if (resp == "")
                {
                    //dominatorAccountModel.AccountBaseModel.Status = AccountStatus.Failed;
                }
            }
            catch (Exception)
            {
                //dominatorAccountModel.AccountBaseModel.Status = AccountStatus.Failed;
            }

            try
            {
                dominatorAccountModel.Token.ThrowIfCancellationRequested();
                //SocinatorAccountBuilder.Instance(dominatorAccountModel.AccountBaseModel.AccountId)
                //  .AddOrUpdateDominatorAccountBase(dominatorAccountModel.AccountBaseModel)
                //  .AddOrUpdateUserAgentWeb(dominatorAccountModel.UserAgentWeb)
                //.AddOrUpdateCookies(dominatorAccountModel.Cookies)
                //   .SaveToBinFile();
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

            UpdateAccountDetailToDatabase(dominatorAccountModel, cancellationToken);
        }

        public bool CheckLogin(DominatorAccountModel dominatorAccountModel, CancellationToken cancellationToken)
        {
            return CheckLoginAsync(dominatorAccountModel, cancellationToken).Result;
        }

        public void LoginWithAlternativeMethod(DominatorAccountModel dominatorAccountModel,
            CancellationToken cancellationToken)
        {
        }

        public async Task<bool> CheckLoginAsync(DominatorAccountModel dominatorAccountModel,
            CancellationToken cancellationToken, bool displayLoginMsg = false, LoginType loginType = LoginType.AutomationLogin)
        {
            if (!dominatorAccountModel.IsUserLoggedIn)
            {
                dominatorAccountModel.Token.ThrowIfCancellationRequested();
                try
                {
                    await LoginUsingGlobusHttpQuoraAsync(dominatorAccountModel, cancellationToken);
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }
            //if(dominatorAccountModel.IsUserLoggedIn)
            //    await ThreadFactory.Instance.Start(async () =>
            //    {
            //        var Count = RandomUtilties.GetRandomNumber(30, 10);
            //        await quoraFunctions.StartScrapingAds(dominatorAccountModel, Count);
            //    });
            return dominatorAccountModel.IsUserLoggedIn;
        }

        public async Task LoginWithDataBaseCookiesAsync(DominatorAccountModel dominatorAccountModel,
            bool isMobileRequired, CancellationToken cancellationToken)
        {
            try
            {
                try
                {
                    sessionManager.AddOrUpdateSession(ref dominatorAccountModel);
                    var requestParameter = _httpHelper.GetRequestParameter();
                    requestParameter.Cookies = new CookieCollection();
                    _httpHelper.SetRequestParameter(requestParameter);

                    var objWebHeaderCollectionWeb = new WebHeaderCollection
                    {
                        {
                            "User-Agent",
                            "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/55.0.2883.87 Safari/537.36"
                        }
                    };
                    requestParameter.KeepAlive = true;
                    requestParameter.ContentType = "application/x-www-form-urlencoded";
                    requestParameter.Accept = "application/json, text/javascript, */*; q=0.01";
                    objWebHeaderCollectionWeb.Add("Accept-Language", "en-us,en;q=0.5");
                    requestParameter.Headers = objWebHeaderCollectionWeb;
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                try
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var accountsFileManager = InstanceProvider.GetInstance<IAccountsFileManager>();
                    var account = accountsFileManager.GetAccount(dominatorAccountModel.UserName,
                        dominatorAccountModel.AccountBaseModel.AccountNetwork);
                    dominatorAccountModel.Cookies = account.Cookies;
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
                sessionManager.AddOrUpdateSession(ref dominatorAccountModel);
                if (dominatorAccountModel.Cookies != null)
                    try
                    {
                        dominatorAccountModel.Token.ThrowIfCancellationRequested();
                        await CheckisLoggedInAsync(dominatorAccountModel, cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                if (!dominatorAccountModel.IsUserLoggedIn)
                {
                    dominatorAccountModel.Token.ThrowIfCancellationRequested();
                    LoginUsingGlobusHttpQuora(dominatorAccountModel, cancellationToken,false);

                    if (dominatorAccountModel.IsUserLoggedIn)
                    {
                        try
                        {
                            dominatorAccountModel.Token.ThrowIfCancellationRequested();
                            //AccountsFileManager.Edit(dominatorAccountModel);
                            SocinatorAccountBuilder.Instance(dominatorAccountModel.AccountBaseModel.AccountId)
                                .AddOrUpdateDominatorAccountBase(dominatorAccountModel.AccountBaseModel)
                                .AddOrUpdateUserAgentWeb(dominatorAccountModel.UserAgentWeb)
                                .AddOrUpdateCookies(dominatorAccountModel.Cookies)
                                .SaveToBinFile();
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

                        UpdateAccountDetailToDatabase(dominatorAccountModel, cancellationToken);
                    }
                }
                if (dominatorAccountModel.AccountBaseModel.Status == AccountStatus.NeedsVerification)
                {
                    dominatorAccountModel.AccountBaseModel.Status = AccountStatus.NeedsVerification;
                    GlobusLogHelper.log.Info(Log.LoginFailed, dominatorAccountModel.AccountBaseModel.AccountNetwork,
                        dominatorAccountModel.AccountBaseModel.UserName, " verification required !!");
                }
                else if (dominatorAccountModel.AccountBaseModel.Status == AccountStatus.ProxyNotWorking)
                {
                    dominatorAccountModel.AccountBaseModel.Status = AccountStatus.ProxyNotWorking;
                    GlobusLogHelper.log.Info(Log.LoginFailed, dominatorAccountModel.AccountBaseModel.AccountNetwork,
                        dominatorAccountModel.AccountBaseModel.UserName, " proxy is not Working !!");
                }
                if (dominatorAccountModel.IsUserLoggedIn && dominatorAccountModel.AccountBaseModel.Status==AccountStatus.TryingToLogin)
                    dominatorAccountModel.AccountBaseModel.Status = AccountStatus.Success;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void UpdateAccountDetailToDatabase(DominatorAccountModel dominatorAccountModel,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var globalDbOperation = new DbOperations(SocinatorInitialize.GetGlobalDatabase().GetSqlConnection());
            if (globalDbOperation.Get<AccountDetails>().Any(account => dominatorAccountModel.AccountId == account.AccountId && account.ActivityManager is null))
                globalDbOperation.RemoveMatch<AccountDetails>(account => dominatorAccountModel.AccountId == account.AccountId && account.ActivityManager == null);
            globalDbOperation.UpdateAccountDetails(dominatorAccountModel);
        }

        public Task LoginWithAlternativeMethodAsync(DominatorAccountModel dominatorAccountModel,
            CancellationToken token)
        {
            return null;
        }

        public void LoginWithBrowserMethod(DominatorAccountModel dominatorAccountModel,
            CancellationToken cancellationToken, VerificationType verificationType = 0, LoginType loginType = LoginType.AutomationLogin)
        {
            var logged  =  managerFactory.CheckStatusAsync(dominatorAccountModel, cancellationToken, loginType);
            if(logged)
                GlobusLogHelper.log.Info("Browser Login Sucessfull", dominatorAccountModel.AccountBaseModel.AccountNetwork,
                    dominatorAccountModel.AccountBaseModel.UserName);
        }
        #endregion
    }
}