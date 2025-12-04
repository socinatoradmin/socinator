using DominatorHouseCore;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.PuppeteerBrowser;
using DominatorHouseCore.Utility;
using EmbeddedBrowser;
using LinkedDominatorCore.Interfaces;
using LinkedDominatorCore.LDUtility;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace LinkedDominatorCore.Utility
{
    public class LDAccountsBrowserDetails
    {
        private static LDAccountsBrowserDetails _instance;

        public static readonly object LockBrowser = new object();

        private CancellationToken _token;
        private readonly IDispatcherUtility _dispatcherUtility;

        private LDAccountsBrowserDetails()
        {
            _dispatcherUtility = InstanceProvider.GetInstance<IDispatcherUtility>();
            AccountBrowserCollections = new Dictionary<string, BrowserWindow>();
        }

        public Dictionary<string, BrowserWindow> AccountBrowserCollections { get; set; }

        public static LDAccountsBrowserDetails GetInstance()
        {
            return _instance ?? (_instance = new LDAccountsBrowserDetails());
        }

        public static void CloseBrowser(DominatorAccountModel dominatorAccountModel, bool isReturn = false)
        {
            if (isReturn)
                return;
            CheckAndCloseBrowser(dominatorAccountModel);
            CheckAndCloseBrowser(dominatorAccountModel, browserInstanceType: BrowserInstanceType.Secondary);
        }

        public static void CheckAndCloseBrowser(DominatorAccountModel dominatorAccountModel, bool isReturn = false,
            BrowserInstanceType browserInstanceType = BrowserInstanceType.Primary)
        {
            try
            {
                if (isReturn)
                    return;
                var name = GetBrowserName(dominatorAccountModel, browserInstanceType);
                GetInstance().AccountBrowserCollections.TryGetValue(name, out var browserWindow);

                if (browserWindow == null)
                    return;

                lock (LockBrowser)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        browserWindow.Close();
                        browserWindow.Dispose();
                    });
                    GetInstance().AccountBrowserCollections.Remove(name);
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void StartBrowserLogin(DominatorAccountModel dominatorAccountModel, CancellationToken cancellationToken,
            bool isSave, BrowserInstanceType browserInstanceType = BrowserInstanceType.Primary, ILDAccountSessionManager sessionManager=null,
            string TargetUrl="",bool WaitToLoad=false)
        {
            _token = cancellationToken;
            BrowserWindow _browserWindow = null;
            var isCheckedLogin = false;

            // we may use '.wait()' in task factory instead of using while here
            Task.Factory.StartNew(() =>
            {
                _dispatcherUtility.InvokeAsync(() =>
                {
                    try
                    {
                        _token.ThrowIfCancellationRequested();
                        _browserWindow = new BrowserWindow(dominatorAccountModel) {Visibility = Visibility.Hidden};
                        _browserWindow.SetCookie();
                        if (browserInstanceType.Equals(BrowserInstanceType.BrowserLogin))
                            _browserWindow.Visibility = Visibility.Visible;
                        // make hidden after work is done
#if (DEBUG)
                        _browserWindow.Visibility = Visibility.Visible;
#endif
                        _browserWindow.WindowState = WindowState.Maximized;
                        if (isSave)
                            try
                            {
                                var saveName = GetBrowserName(dominatorAccountModel, browserInstanceType);
                                GetInstance().AccountBrowserCollections.Add(saveName, _browserWindow);
                            }
                            catch (Exception ex)
                            {
                                ex.DebugLog();
                            }

                        _browserWindow.Show();
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                });
                Sleep(7);
                BrowserLoginProcess(dominatorAccountModel, _browserWindow, browserInstanceType, ref isCheckedLogin,sessionManager,WaitToLoad,TargetUrl);
            }, cancellationToken);

            while (!isCheckedLogin && !browserInstanceType.Equals(BrowserInstanceType.BrowserLogin))
                Sleep(15);
        }

        public void BrowserLoginProcess(DominatorAccountModel dominatorAccountModel, BrowserWindow browserWindow, BrowserInstanceType browserInstanceType,
            ref bool isCheckedLogin,ILDAccountSessionManager sessionManager,bool WaitToLoad=false,string TargetUrl="")
        {
            try
            {
                // waiting until login page or main page is loaded successfully
                LoadWhile(browserWindow);
                Thread.Sleep(1000);
                var html = browserWindow.GetPageSource();
                if (!string.IsNullOrEmpty(html) && !html.Contains("denialReasonCodes"))
                {
                    if (browserWindow.CurrentUrl().Contains("https://www.linkedin.com/hp") ||
                        browserWindow.CurrentUrl().Trim('/').Equals("https://www.linkedin.com"))
                    {
                        browserWindow.BrowserAct(ActType.ClickByClass, "nav__button-secondary", delayAfter: 5);
                        LoadWhile(browserWindow);
                        html = browserWindow.GetPageSource();
                    }
                    Thread.Sleep(8000);
                    var isLoginPage = false;
                    if (html.Contains("consumer_login__text_plain__large_username"))
                    {
                        browserWindow
                            .ExecuteScriptAsync(
                                $"document.getElementById('username').value= '{dominatorAccountModel.AccountBaseModel.UserName}'")
                            .Wait();
                        browserWindow
                            .ExecuteScriptAsync(
                                $"document.getElementById('password').value= '{dominatorAccountModel.AccountBaseModel.Password}'")
                            .Wait();
                        _token.ThrowIfCancellationRequested();
                        browserWindow
                            .ExecuteScriptAsync(
                                "document.getElementById('btn__primary--large from__button--floating').disabled = false",
                                5).Wait();
                        _token.ThrowIfCancellationRequested();
                        browserWindow
                            .ExecuteScriptAsync(
                                "document.getElementsByClassName('btn__primary--large from__button--floating')[0].click()",
                                5).Wait();
                        Thread.Sleep(1000);
                        isLoginPage = true;
                    }
                    else if (!string.IsNullOrEmpty(html) &&
                             (html.Contains("By clicking Join now, you agree to the LinkedIn") ||
                              html.Contains("By clicking Join for free, you agree to the LinkedIn ")))
                    {
                        _token.ThrowIfCancellationRequested();
                        browserWindow
                            .ExecuteScriptAsync(
                                $"document.getElementById('login-email').value= '{dominatorAccountModel.AccountBaseModel.UserName}'")
                            .Wait();
                        browserWindow
                            .ExecuteScriptAsync(
                                $"document.getElementById('login-password').value= '{dominatorAccountModel.AccountBaseModel.Password}'")
                            .Wait();
                        _token.ThrowIfCancellationRequested();
                        browserWindow.ExecuteScriptAsync("document.getElementById('login-submit').disabled = false", 5)
                            .Wait();
                        _token.ThrowIfCancellationRequested();
                        browserWindow.ExecuteScriptAsync("document.getElementById('login-submit').click()", 5).Wait();
                        isLoginPage = true;
                    }
                    else if(!string.IsNullOrEmpty(html) && html.Contains("Welcome back"))
                    {
                        var xy = browserWindow.GetXAndY(AttributeType.Id,"password");
                        browserWindow.MouseClick(xy.Key+5,xy.Value+5,delayAfter:10);
                        browserWindow.EnterChars(dominatorAccountModel.AccountBaseModel.Password,delayAtLast:10);
                        html = browserWindow.GetPageSource();
                        browserWindow.ExecuteScript("document.getElementsByClassName('btn__primary--large from__button--floating')[0].click();", 5);
                        isLoginPage = true;
                    }
                    else if(!string.IsNullOrEmpty(html) && html.Contains(dominatorAccountModel.AccountBaseModel.UserFullName) || html.Contains("member-profile__details"))
                    {
                        browserWindow.ExecuteScriptAsync("document.getElementsByClassName('member-profile__details')[0].click();", 5).Wait();
                        isLoginPage = true;
                    }
                    else
                    {
                        isLoginPage = true;
                    }
                    if (isLoginPage)
                    {
                        LoadWhile(browserWindow);
                        Sleep(5);
                        html = browserWindow.GetPageSource();
                        dominatorAccountModel.AccountBaseModel.Status = DominatorHouseCore.Enums.AccountStatus.Success;
                        if (html.Contains("Adding a phone number adds security"))
                        {
                            var automation = new BrowserAutomationExtension(browserWindow);
                            automation.ExecuteScript(AttributeIdentifierType.Xpath,
                                "//button[@class='secondary-action']", 5);
                            LoadWhile(browserWindow);
                        }
                        if (browserInstanceType != BrowserInstanceType.CheckAccountStatus)
                        {
                            html = browserWindow.GetPageSource();
                            if (html.Contains("Let's do a quick verification") ||html.Contains("Let's do a quick security check") || html.Contains("Resend verification code") || browserWindow.CurrentUrl().Contains("/checkpoint/challenge/"))
                            {
                                GlobusLogHelper.log.Info(Log.CustomMessage,
                                    dominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    dominatorAccountModel.AccountBaseModel.UserName, "Account Browser Login",
                                    "Let's do a quick verification, Please enter verfication code");
                                dominatorAccountModel.AccountBaseModel.Status = DominatorHouseCore.Enums.AccountStatus.FoundCaptcha;
                                WaitForSecurityVerificationProcess(browserWindow);
                                Sleep(10);
                                html = browserWindow.GetPageSourceAsync().Result;
                                if (!html.Contains("Let's do a quick verification") ||!html.Contains("Resend verification code"))
                                {
                                    dominatorAccountModel.AccountBaseModel.Status = DominatorHouseCore.Enums.AccountStatus.Success;
                                    GlobusLogHelper.log.Info(Log.CustomMessage,
                                        dominatorAccountModel.AccountBaseModel.AccountNetwork,
                                        dominatorAccountModel.AccountBaseModel.UserName, "Account Browser Login",
                                        "Browser Login Successful");
                                }
                                else
                                    GlobusLogHelper.log.Info(Log.CustomMessage,
                                        dominatorAccountModel.AccountBaseModel.AccountNetwork,
                                        dominatorAccountModel.AccountBaseModel.UserName, "Account Browser Login",
                                        "Session timeout, Please try again");
                            }
                        }
                        html = browserWindow.GetPageSource();
                        if (WaitToLoad && !string.IsNullOrEmpty(TargetUrl) && LdConstants.IsSalesAccount(browserWindow.GetPageSource())
                            || (browserInstanceType == BrowserInstanceType.BrowserLogin) && html.Contains("https://www.linkedin.com/sales/index?trk=d_flagship3_nav&")) {
                            browserWindow.GoToCustomUrl(LdConstants.GetSalesHomePageUrl,delayAfter:5);
                            Sleep(15);
                            browserWindow.GoToCustomUrl("https://www.linkedin.com/feed/", delayAfter: 5);
                            Sleep(8);
                        }
                        dominatorAccountModel.Cookies = browserWindow.BrowserCookiesIntoModel().Result;
                        dominatorAccountModel.BrowserCookies= browserWindow.BrowserCookiesIntoModel().Result;
                        SocinatorAccountBuilder.Instance(dominatorAccountModel
                                .AccountBaseModel.AccountId)
                            .AddOrUpdateDominatorAccountBase(dominatorAccountModel.AccountBaseModel)
                            .AddOrUpdateCookies(dominatorAccountModel.Cookies)
                            .AddOrUpdateBrowserCookies(dominatorAccountModel.BrowserCookies)
                            .SaveToBinFile();   
                    }
                    else
                        dominatorAccountModel.AccountBaseModel.Status = DominatorHouseCore.Enums.AccountStatus.Failed;
                }
            }
            catch (Exception exception)
            {
                exception.DebugLog();
            }
            finally
            {
                sessionManager.AddOrUpdateSession(ref dominatorAccountModel, true);
                isCheckedLogin = true;
            }
        }

        private void WaitForSecurityVerificationProcess(BrowserWindow browserWindow)
        {
            var count = 0;
            var pageSource = browserWindow.GetPageSourceAsync().Result;
            while ((pageSource.Contains("Resend verification code") 
                || pageSource.Contains("/checkpoint/challenge/verify") 
                || pageSource.Contains("Let's do a quick security check") 
                || pageSource.Contains("Let's do a quick verification")) 
                && count++ <= 10)
            {
                _token.ThrowIfCancellationRequested();
                Sleep(25);
                pageSource = browserWindow.GetPageSourceAsync().Result;
            }
        }

        public void LoadWhile(BrowserWindow browserWindow)
        {
            var count = 0;
            var pageSource = browserWindow.GetPageSource();
            if (pageSource.Equals("<html><head></head><body></body></html>"))
            {
                browserWindow.GoToCustomUrl("https://www.linkedin.com", 2).Wait();
                browserWindow.Refresh();
                Sleep(15);
                pageSource = browserWindow.GetPageSource();
            }

            while (browserWindow.GetPageSource().Contains("data-view-name=\"identity-module\"") ? false:!(pageSource = browserWindow.GetPageSource()).Contains("</code>") && ++count <= 3 &&
                   !pageSource.Contains("denialReasonCodes"))
            {
                _token.ThrowIfCancellationRequested();
                Sleep(15);
            }
        }

        private void Sleep(double seconds = 1)
        {
            Task.Delay(TimeSpan.FromSeconds(seconds)).Wait(_token);
        }

        public static string GetBrowserName(DominatorAccountModel accountModel, BrowserInstanceType browserInstanceType)
        {
            return accountModel.UserName + (browserInstanceType.Equals(BrowserInstanceType.Primary)
                       ? ""
                       : browserInstanceType.ToString());
        }
    }

    public enum BrowserInstanceType
    {
        Primary,
        Secondary,
        BrowserLogin,
        CheckAccountStatus
    }
}