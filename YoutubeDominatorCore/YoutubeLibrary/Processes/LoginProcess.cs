using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.BusinessLogic.ActivitiesWorkflow;
using DominatorHouseCore.DatabaseHandler.YdTables.Accounts;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Extensions;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Request;
using DominatorHouseCore.Utility;
using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using YoutubeDominatorCore.Request;
using YoutubeDominatorCore.Response;
using YoutubeDominatorCore.YDUtility;
using YoutubeDominatorCore.YoutubeLibrary.YdFunctions;
using Constants = DominatorHouseCore.Utility.Constants;

namespace YoutubeDominatorCore.YoutubeLibrary.Processes
{
    public interface IYoutubeLogInProcess : ILoginProcessAsync
    {
        IYdBrowserManager BrowserManager { get; set; }
        SoftwareSettingsModel SoftwareSettingsModel { get; set; }
        void SetRequestParameter(ref DominatorAccountModel dominatorAccountModel);

        bool BrowserLogin(DominatorAccountModel dominatorAccountModel, CancellationToken cancellationToken,
            bool itsJustLogin = true);
    }

    public class LoginProcess : IYoutubeLogInProcess
    {
        public readonly IYdHttpHelper HttpHelper;

        public LoginProcess(IYdHttpHelper httpHelper, IYdBrowserManager browserManager)
        {
            HttpHelper = httpHelper;
            BrowserManager = browserManager;
            var softwareSettingManager = InstanceProvider.GetInstance<ISoftwareSettingsFileManager>();
            SoftwareSettingsModel = softwareSettingManager.GetSoftwareSettings();
        }

        public IYdBrowserManager BrowserManager { get; set; }
        public SoftwareSettingsModel SoftwareSettingsModel { get; set; }

        public bool CheckLogin(DominatorAccountModel dominatorAccountModel, CancellationToken cancellationToken)
        {
            return CheckLoginAsync(dominatorAccountModel, dominatorAccountModel.Token).Result;
        }

        public void LoginWithAlternativeMethod(DominatorAccountModel dominatorAccountModel,
            CancellationToken cancellationToken)
        {
            CheckLogin(dominatorAccountModel, cancellationToken);
        }

        public void LoginWithDataBaseCookies(DominatorAccountModel dominatorAccountModel, bool isMobileRequired,
            CancellationToken cancellationToken)
        {
            CheckLogin(dominatorAccountModel, cancellationToken);
        }

        public async Task<bool> CheckLoginAsync(DominatorAccountModel dominatorAccountModel,
            CancellationToken cancellationToken, bool displayLoginMsg = false, LoginType loginType = LoginType.AutomationLogin)
        {
            var statusToSetAtLastWhenCancel =
                dominatorAccountModel.AccountBaseModel.Status == AccountStatus.TryingToLogin
                    ? AccountStatus.NotChecked
                    : dominatorAccountModel.AccountBaseModel.Status;
            try
            {
                var itsJustLogin = false;
                if (await Task.Run(() => CheckProxy(dominatorAccountModel, cancellationToken)) &&
                    (dominatorAccountModel.Cookies.Count > 0 || dominatorAccountModel.IsRunProcessThroughBrowser))
                {
                    try
                    {
                        var homePage = "";

                        if (dominatorAccountModel.IsRunProcessThroughBrowser)
                        {
                            itsJustLogin = dominatorAccountModel.AccountBaseModel.NeedToCloseBrowser ? dominatorAccountModel.IsVerificationCodeSent ||
                                           dominatorAccountModel.AccountBaseModel.Status == AccountStatus.TryingToLogin : dominatorAccountModel.AccountBaseModel.NeedToCloseBrowser;

                            if (dominatorAccountModel.AccountBaseModel.Status == AccountStatus.Success ||
                                !dominatorAccountModel.IsVerificationCodeSent)
                                dominatorAccountModel.AccountBaseModel.Status = AccountStatus.TryingToLogin;
                            if (await Task.Run(() => BrowserLogin(dominatorAccountModel, cancellationToken, false)))
                                homePage = BrowserManager._html;
                        }
                        else
                        {
                            await Task.Run(() => SetRequestParameter(ref dominatorAccountModel));
                            var objResponseParameter =
                                await HttpHelper.GetRequestAsync("https://www.youtube.com/ ", cancellationToken);
                            homePage = objResponseParameter?.Response ?? "";
                        }

                        cancellationToken.ThrowIfCancellationRequested();
                        var homePageRespHand = new YoutubeHomePageHandler(homePage);
                        if (homePageRespHand.IsLoggedIn)
                        {
                            var dbAccountService = InstanceProvider.ResolveAccountDbOperations(dominatorAccountModel.AccountId, dominatorAccountModel.AccountBaseModel.AccountNetwork);
                            var Channel = dbAccountService.Get<OwnChannels>(x => x.PageId == homePageRespHand.ChannelId).FirstOrDefault();
                            dominatorAccountModel.AccountBaseModel.Status = AccountStatus.Success;
                            dominatorAccountModel.AccountBaseModel.ProfileId = Channel != null ? Channel.ChannelName : homePageRespHand.ChannelUsername;
                            dominatorAccountModel.AccountBaseModel.UserId = homePageRespHand.ChannelId;
                            if (!cancellationToken.IsCancellationRequested &&
                                dominatorAccountModel.IsRunProcessThroughBrowser)
                                dominatorAccountModel.Cookies = await BrowserManager.BrowserWindow.BrowserCookiesIntoModel();
                            else
                                dominatorAccountModel.Cookies = HttpHelper.GetRequestParameter().Cookies;

                            dominatorAccountModel.IsUserLoggedIn = true;

                            SocinatorAccountBuilder.Instance(dominatorAccountModel.AccountBaseModel.AccountId)
                                .AddOrUpdateDominatorAccountBase(dominatorAccountModel.AccountBaseModel)
                                .AddOrUpdateCookies(dominatorAccountModel.Cookies)
                                .AddOrUpdateBrowserCookies(dominatorAccountModel.BrowserCookies)
                                .AddOrUpdateLoginStatus(dominatorAccountModel.IsUserLoggedIn)
                                .SaveToBinFile();
                        }
                        else if (dominatorAccountModel.AccountBaseModel.Status == AccountStatus.Success)
                        {
                            dominatorAccountModel.AccountBaseModel.Status = AccountStatus.Failed;
                            SocinatorAccountBuilder.Instance(dominatorAccountModel.AccountBaseModel.AccountId)
                                .AddOrUpdateDominatorAccountBase(dominatorAccountModel.AccountBaseModel)
                                .AddOrUpdateLoginStatus(dominatorAccountModel.IsUserLoggedIn)
                                .SaveToBinFile();
                        }
                        else { }
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
                        if (cancellationToken.IsCancellationRequested &&
                            dominatorAccountModel.AccountBaseModel.Status == AccountStatus.TryingToLogin)
                        {
                            if (dominatorAccountModel.IsUserLoggedIn)
                                dominatorAccountModel.AccountBaseModel.Status = AccountStatus.Success;
                            else if (!dominatorAccountModel.IsUserLoggedIn && dominatorAccountModel.CookieHelperList.Count > 20 && statusToSetAtLastWhenCancel == AccountStatus.Success)
                            {
                                dominatorAccountModel.AccountBaseModel.Status = AccountStatus.Success;
                                dominatorAccountModel.IsUserLoggedIn = true;
                            }
                            else
                                dominatorAccountModel.AccountBaseModel.Status = statusToSetAtLastWhenCancel;
                            SocinatorAccountBuilder.Instance(dominatorAccountModel.AccountBaseModel.AccountId)
                        .AddOrUpdateDominatorAccountBase(dominatorAccountModel.AccountBaseModel)
                        .AddOrUpdateLoginStatus(dominatorAccountModel.IsUserLoggedIn)
                        .UpdateLastUpdateTime(DateTime.Now.ConvertToEpoch())
                        .SaveToBinFile();
                        }

                        if (itsJustLogin && !dominatorAccountModel.IsVerificationCodeSent || dominatorAccountModel.AccountBaseModel.Status == AccountStatus.PhoneVerification && !dominatorAccountModel.IsVerificationCodeSent)
                            await Task.Run(() => CloseBrowserAfterCompletion(BrowserManager));
                    }
                }
                if (dominatorAccountModel.IsRunProcessThroughBrowser && !dominatorAccountModel.IsUserLoggedIn ||
                    dominatorAccountModel.AccountBaseModel.Status == AccountStatus.ProxyNotWorking)
                {
                    if (dominatorAccountModel.AccountBaseModel.Status == AccountStatus.TryingToLogin)
                        dominatorAccountModel.AccountBaseModel.Status = AccountStatus.Failed;
                    if (dominatorAccountModel.AccountBaseModel.Status != AccountStatus.ProxyNotWorking)
                        dominatorAccountModel.Cookies = new CookieCollection();
                    dominatorAccountModel.IsUserLoggedIn = false;
                    SocinatorAccountBuilder.Instance(dominatorAccountModel.AccountBaseModel.AccountId)
                        .AddOrUpdateDominatorAccountBase(dominatorAccountModel.AccountBaseModel)
                        .AddOrUpdateLoginStatus(dominatorAccountModel.IsUserLoggedIn)
                        .UpdateLastUpdateTime(DateTime.Now.ConvertToEpoch())
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

            return dominatorAccountModel.IsUserLoggedIn;
        }

        public bool BrowserLogin(DominatorAccountModel dominatorAccountModel, CancellationToken cancellationToken,
            bool itsJustLogin = true)
        {
            try
            {
                if (cancellationToken.IsCancellationRequested)
                    return false;
                lock (YdStatic.BrowserLock)
                {
                    if (cancellationToken.IsCancellationRequested)
                        return false;

                    if (++YdStatic.BrowserOpening > (itsJustLogin ? 10 : 100))
                        Monitor.Wait(YdStatic.BrowserLock);
                }

                cancellationToken.ThrowIfCancellationRequested();

                return OpenBrowserToLogin(dominatorAccountModel, cancellationToken, itsJustLogin);
            }
            catch (OperationCanceledException)
            {
                lock (YdStatic.BrowserLock)
                {
                    --YdStatic.BrowserOpening;
                    Monitor.Pulse(YdStatic.BrowserLock);
                }
            }

            return false;
        }

        public void SetRequestParameter(ref DominatorAccountModel dominatorAccountModel)
        {
            var objRequestParameters = new RequestParameters
            {
                Accept =
                    "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9",
                Headers =
                {
                    ["Accept-Language"] = "en-US,en;q=0.9", ["Upgrade-Insecure-Requests"] = "1",
                    ["X-Client-Data"] =
                        "CIW2yQEIprbJAQjBtskBCKmdygEIlqzKAQiGtcoBCJm1ygEIq8fKAQj1x8oBCOfIygEI6cjKAQi0y8oBCNnXygE=",
                    ["Sec-Fetch-Site"] = "same-origin", ["Sec-Fetch-Mode"] = "navigate", ["Sec-Fetch-User"] = "?1",
                    ["Sec-Fetch-Dest"] = "document", ["Upgrade-Insecure-Requests"] = "1",
                    ["Cache-Control"] = "max-age=0"
                },
                KeepAlive = true,
                UserAgent =
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.135 Safari/537.36",
                Cookies = dominatorAccountModel.Cookies, //RemoveDuplicateCookies(dominatorAccountModel.Cookies),
                ContentType = Constants.ContentTypeDefault
            };
            HttpHelper.SetRequestParameter(objRequestParameters);
        }

        public Task LoginWithDataBaseCookiesAsync(DominatorAccountModel dominatorAccountModel, bool isMobileRequired,
            CancellationToken cancellationToken)
        {
            return CheckLoginAsync(dominatorAccountModel, cancellationToken);
        }

        public Task LoginWithAlternativeMethodAsync(DominatorAccountModel dominatorAccountModel,
            CancellationToken cancellationToken)
        {
            return CheckLoginAsync(dominatorAccountModel, cancellationToken);
        }

        public void LoginWithBrowserMethod(DominatorAccountModel dominatorAccountModel,
            CancellationToken cancellationToken, VerificationType verification = 0,LoginType loginType = LoginType.AutomationLogin)
        {
            CheckLoginAsync(dominatorAccountModel, cancellationToken).Wait(cancellationToken);
        }

        private bool OpenBrowserToLogin(DominatorAccountModel dominatorAccountModel,
            CancellationToken cancellationToken, bool itsJustLogin)
        {
            var browserManager = BrowserManager ?? new YdBrowserManager();
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                return browserManager.BrowserLogin(dominatorAccountModel, cancellationToken);
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            finally
            {
                if (itsJustLogin && !dominatorAccountModel.IsVerificationCodeSent)
                    CloseBrowserAfterCompletion(browserManager);
            }

            return false;
        }

        private void CloseBrowserAfterCompletion(IYdBrowserManager browserManager)
        {
            try
            {
                if (browserManager != null)
                {
                    browserManager.CloseBrowser();

                    lock (YdStatic.BrowserLock)
                    {
                        --YdStatic.BrowserOpening;
                        Monitor.Pulse(YdStatic.BrowserLock);
                    }
                }
            }
            catch (Exception e)
            {
                e.DebugLog();
            }
        }

        private bool CheckProxy(DominatorAccountModel dominatorAccountModel, CancellationToken cancellationToken)
        {
            try
            {
                if (dominatorAccountModel.AccountBaseModel.AccountProxy.ProxyIp != null && !string.IsNullOrEmpty(dominatorAccountModel.AccountBaseModel.AccountProxy.ProxyIp))
                {
                    var http = new YdHttpHelper();
                    var igReq = new RequestParameters
                    {
                        Proxy = !string.IsNullOrEmpty(dominatorAccountModel.AccountBaseModel.AccountProxy.ProxyIp.Trim())
                            ? dominatorAccountModel.AccountBaseModel.AccountProxy
                            : new Proxy()
                    };
                    igReq.KeepAlive = true;
                    igReq.AddHeader("Upgrade-Insecure-Requests", "1");
                    igReq.UserAgent =
                        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.105 Safari/537.36";
                    igReq.Accept =
                        "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9";
                    igReq.Headers.Add("Accept-Language", "en-US,en;q=0.9");
                    igReq.ContentType = "application/x-www-form-urlencoded";
                    http.SetRequestParameter(igReq);
                    var response = http.GetRequestAsync("https://youtube.com/", cancellationToken);

                    //string myIp = http.GetRequest("https://app.multiloginapp.com/WhatIsMyIP ").Response;

                    if (response?.Result.Response?.Contains("<title>YouTube</title>") ?? false)
                        return true;

                    dominatorAccountModel.AccountBaseModel.Status = AccountStatus.ProxyNotWorking;
                    SocinatorAccountBuilder.Instance(dominatorAccountModel.AccountBaseModel.AccountId)
                        .AddOrUpdateDominatorAccountBase(dominatorAccountModel.AccountBaseModel)
                        .SaveToBinFile();
                    GlobusLogHelper.log.Info(Log.LoginFailed, SocialNetworks.YouTube,
                        dominatorAccountModel.AccountBaseModel.UserName, dominatorAccountModel.AccountBaseModel.Status);

                    return false;
                }
                else
                    return true;
            }
            catch (Exception)
            {
                return true;
            }
        }
    }
}