using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.PuppeteerBrowser;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CaptchaUtility;
using EmbeddedBrowser;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using ThreadUtils;
using TwtDominatorCore.Interface;
using AttributeType1 = DominatorHouseCore.PuppeteerBrowser.AttributeType;
namespace TwtDominatorCore.TDUtility
{
    public class TdAccountsBrowserDetails:CaptchaSolverUtility
    {
        private static TdAccountsBrowserDetails _instance;
        public static readonly object LockBrowser = new object();
        private readonly IDispatcherUtility _dispatcherUtility;
        private readonly IDelayService _delayService;
        private CancellationToken _token;
        private TdAccountsBrowserDetails()
        {
            AccountBrowserCollections = new Dictionary<string, BrowserWindow>();
            _dispatcherUtility = InstanceProvider.GetInstance<IDispatcherUtility>();
            _delayService = InstanceProvider.GetInstance<IDelayService>();
        }

        public Dictionary<string, BrowserWindow> AccountBrowserCollections { get; set; }

        public static TdAccountsBrowserDetails GetInstance()
        {
            return _instance ?? (_instance = new TdAccountsBrowserDetails());
        }

        public static void CloseAllBrowser(DominatorAccountModel dominatorAccountModel, bool isReturn = false)
        {
            //if (isReturn)
            //    return;
            CloseBrowser(dominatorAccountModel, BrowserInstanceType.CheckAccountStatus);
            CloseBrowser(dominatorAccountModel, BrowserInstanceType.Primary);
            CloseBrowser(dominatorAccountModel, BrowserInstanceType.Secondary);
        }

        public static void CloseBrowser(DominatorAccountModel dominatorAccountModel,
            BrowserInstanceType browserInstanceType)
        {
            try
            {
                BrowserWindow browserWindow;
                var name = GetBrowserName(dominatorAccountModel, browserInstanceType);
                GetInstance().AccountBrowserCollections.TryGetValue(name, out browserWindow);

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

        public void CreateBrowser(DominatorAccountModel dominatorAccountModel, CancellationToken cancellationToken,ITwitterAccountSessionManager twitterAccountSession,
            BrowserInstanceType browserInstanceType = BrowserInstanceType.Primary, bool isSave = true,bool ShowLogin=true)
        {
            _token = cancellationToken;
            BrowserWindow browser = null;
            var isCheckedLogin = false;
            _dispatcherUtility.InvokeAsync(async() =>
            {
                try
                {
                    if (dominatorAccountModel.Cookies.Count < dominatorAccountModel.BrowserCookies.Count)
                        dominatorAccountModel.Cookies = dominatorAccountModel.BrowserCookies;
                    _token.ThrowIfCancellationRequested();
                    browser =
                        new BrowserWindow(dominatorAccountModel, isNeedResourceData: true) { Visibility = Visibility.Hidden };
                    browser.SetCookie();
                    if (browserInstanceType.Equals(BrowserInstanceType.BrowserLogin))
                        browser.Visibility = Visibility.Visible;
                    // make hidden after work is done
#if DEBUG
                    browser.Visibility = Visibility.Visible;
#endif
                    browser.WindowState = WindowState.Maximized;
                    // make hidden after work is done
                    if (isSave)
                        try
                        {
                            var saveName = GetBrowserName(dominatorAccountModel, browserInstanceType);
                            GetInstance().AccountBrowserCollections
                                .Add(saveName, browser);
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }

                    browser.Show();
                    await Task.Delay(10000, _token);
                    isCheckedLogin = await ProcessBrowserLogin(dominatorAccountModel, browser,isCheckedLogin, twitterAccountSession, ShowLogin);
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            });
            while (!isCheckedLogin)
                Sleep(15);
        }

        private void Sleep(double seconds = 1)
        {
            _delayService.ThreadSleep(TimeSpan.FromSeconds(seconds));
        }

        private async Task<bool> ProcessBrowserLogin(DominatorAccountModel twitterModel, BrowserWindow browserWindow,
            bool isCheckedLogin, ITwitterAccountSessionManager twitterAccountSession,bool IsSave=true)
        {
            try
            {
                //await Task.Delay(20000, _token);
                await WaitToLoadThePage(browserWindow, twitterModel, "Sign in to X");
                var html = await browserWindow.GetPageSourceAsync();
                try
                {
                    var current = browserWindow.CurrentUrl();
                    if(!string.IsNullOrEmpty(current) && current.Contains("?logout="))
                    {
                        await browserWindow.GoToCustomUrl("https://x.com/i/flow/login", delayAfter: 10);
                        await Task.Delay(5000, _token);
                    }
                }
                catch { }
                {
                    #region BrowserLoginRegion.
                    if (!html.Contains("Account unlocked") && !html.Contains("Your account has been locked") && !html.Contains("arkose_iframe") &&!(html.Contains("SideNav_AccountSwitcher_Button") || html.Contains("AppTabBar_More_Menu")))
                    {
                        var usernameBoxXandY =await browserWindow.GetXAndYAsync(AttributeType.ClassName, "r-30o5oe r-1niwhzg r-17gur6a r-1yadl64 r-deolkf r-homxoj r-poiln3 r-7cikom r-1ny4l3l r-t60dpp r-1dz5y72 r-fdjqy7 r-13qz1uu", 0);
                        await browserWindow.MouseClickAsync(usernameBoxXandY.Key + 5, usernameBoxXandY.Value + 5, delayBefore: 2, delayAfter: 5);

                        //Entering username
                        await browserWindow.EnterCharsAsync(twitterModel.AccountBaseModel.UserName, .009);
                        await Task.Delay(TimeSpan.FromSeconds(3), _token);
                        //Click on next
                        await browserWindow.ExecuteScriptAsync("[...document.querySelectorAll('button[role=\"button\"]')].filter(x=>x.textContent.trim()===\"Next\")[0].click();", delayInSec: 8);
                        html = await browserWindow.GetPageSourceAsync();
                        if (html.Contains("There was unusual login activity on your account") || html.Contains("Verify your identity by entering the email address"))
                        {
                            var enterusername = html.Contains("please enter your phone number or username to verify it’s you");
                            if((enterusername && !string.IsNullOrEmpty(twitterModel.AccountBaseModel.UserName) && twitterModel.AccountBaseModel.UserName.Contains("@")) ||!string.IsNullOrEmpty(string.IsNullOrEmpty(twitterModel.AccountBaseModel.AlternateEmail) ? twitterModel.UserAgentWeb : twitterModel.AccountBaseModel.AlternateEmail))
                            {
                                var email = enterusername ? twitterModel.AccountBaseModel.UserName : string.IsNullOrEmpty(twitterModel.AccountBaseModel.AlternateEmail) ? twitterModel.UserAgentWeb : twitterModel.AccountBaseModel.AlternateEmail;
                                await browserWindow.EnterCharsAsync(email, .009, delayAtLast: 7);
                                await browserWindow.ExecuteScriptAsync("[...document.querySelectorAll('button[role=\"button\"]')].filter(x=>x.textContent.trim()===\"Next\")[0].click();", delayInSec: 5);
                            }
                            else
                            {
                                GlobusLogHelper.log.Info(Log.CustomMessage, twitterModel.AccountBaseModel.AccountNetwork,
                                twitterModel.AccountBaseModel.UserName, "Suspicious Login",
                                "Suspicious Login Detected!");
                                return isCheckedLogin;
                            }
                        }
                        await WaitToLoadThePage(browserWindow, twitterModel, "Enter your password");
                        //Entering password
                        await browserWindow.EnterCharsAsync(twitterModel.AccountBaseModel.Password, .009, delayAtLast: 7);
                        html = await browserWindow.GetPageSourceAsync();
                        //Clicking on the LogIn
                        await browserWindow.ExecuteScriptAsync("[...document.querySelectorAll('button[role=\"button\"]')].filter(x=>x.textContent.trim()===\"Log in\")[0].click();", delayInSec: 5);
                        //await Task.Delay(7000, _token);
                        html = await browserWindow.GetPageSourceAsync();
                        if (await IsVerificationNeeded(browserWindow))
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage, twitterModel.AccountBaseModel.AccountNetwork,
                                twitterModel.AccountBaseModel.UserName, "Verification Started",
                                "Socinator ! I am waiting for code 2FA code submit.Please check your 2FA device and submit 2FA code.");
                            var failedCount = 0;
                            while (failedCount++ <= 15 && await IsVerificationNeeded(browserWindow))
                                await Task.Delay(10000, _token);
                            GlobusLogHelper.log.Info(Log.CustomMessage, twitterModel.AccountBaseModel.AccountNetwork,
                                twitterModel.AccountBaseModel.UserName, "Verification Timeout",
                                "Socinator ! Sorry timeout for code submit please try again later.");
                            html = await browserWindow.GetPageSourceAsync();
                        }
                        await WaitToLoadThePage(browserWindow, twitterModel, "", CheckHomePage: true);
                    }
                    else
                    {
                        twitterModel.AccountBaseModel.Status = DominatorHouseCore.Enums.AccountStatus.Success;
                    }
                    if (html.Contains("arkose_form") || html.Contains("arkose_iframe"))
                    {
                        twitterModel.AccountBaseModel.Status = DominatorHouseCore.Enums.AccountStatus.FoundCaptcha;
                        SolveFunCaptha(browserWindow, twitterModel);
                    }
                    else if (html.Contains("Your account has been locked"))
                    {
                        await Task.Delay(5000, _token);
                        twitterModel.AccountBaseModel.Status = DominatorHouseCore.Enums.AccountStatus.FoundCaptcha;
                        await browserWindow.ExecuteScriptAsync("document.querySelector('input[type=\"submit\" i]').click();", delayInSec: 7);
                        SolveFunCaptha(browserWindow, twitterModel);
                    }
                    else
                    {
                        if(html.Contains("Account unlocked"))
                            await browserWindow.ExecuteScriptAsync("document.querySelector('input[type=\"submit\" i]').click();", delayInSec: 7);
                        twitterModel.AccountBaseModel.Status = DominatorHouseCore.Enums.AccountStatus.Success;
                    }
                    #endregion
                    var isSaved = await browserWindow.BrowserSaveCookies(IsSave);
                    if(isSaved)
                        twitterModel.Cookies = browserWindow.DominatorAccountModel.BrowserCookies;
                    SocinatorAccountBuilder.Instance(twitterModel
                            .AccountBaseModel.AccountId)
                        .AddOrUpdateDominatorAccountBase(twitterModel.AccountBaseModel)
                        .AddOrUpdateLoginStatus(twitterModel.IsUserLoggedIn)
                        .AddOrUpdateCookies(twitterModel.Cookies)
                        .AddOrUpdateBrowserCookies(twitterModel.BrowserCookies)
                        .SaveToBinFile();
                    if (html.Contains(TdConstants.AccountSuspendedString))
                        twitterModel.AccountBaseModel.Status = DominatorHouseCore.Enums.AccountStatus.PermanentlyBlocked;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            finally
            {
                twitterModel.AccountBaseModel.Status = twitterModel.AccountBaseModel.Status != DominatorHouseCore.Enums.AccountStatus.Success && twitterModel.AccountBaseModel.Status != DominatorHouseCore.Enums.AccountStatus.PermanentlyBlocked
                    && twitterModel.AccountBaseModel.Status != DominatorHouseCore.Enums.AccountStatus.FoundCaptcha ? DominatorHouseCore.Enums.AccountStatus.Success:twitterModel.AccountBaseModel.Status;
                twitterAccountSession.AddOrUpdateSession(ref twitterModel, true);
                isCheckedLogin = true;
            }
            return isCheckedLogin;
        }

        private async Task WaitToLoadThePage(BrowserWindow browserWindow,DominatorAccountModel dominatorAccount,string condition, int count=8,bool CheckHomePage=false)
        {
            if (browserWindow is null || browserWindow.IsDisposed)
                return;
            var page = await browserWindow.GetPageSourceAsync();
            while(count -- >=0 && CheckHomePage ? !page.Contains("data-testid=\"AppTabBar_Home_Link\"") : (!page.Contains(condition) && !page.Contains("data-testid=\"AppTabBar_Home_Link\"")))
            {
                await Task.Delay(TimeSpan.FromSeconds(4), dominatorAccount.Token);
                page = await browserWindow.GetPageSourceAsync();
            }
        }

        private async Task<bool> IsVerificationNeeded(BrowserWindow browserWindow)
        {
            var html = await browserWindow.GetPageSourceAsync();
            return (html.Contains("In order to protect your account from suspicious activity, we've sent a confirmation code to") && html.Contains("Check your email")
                            || html.Contains("Enter your verification code"));
        }

        private void SolveFunCaptha(BrowserWindow browserWindow,DominatorAccountModel dominatorAccount)
        {
            var IsRunning = true;
            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    if (!base.IsBalanceAvailable(dominatorAccount))
                        return;
                    var IsHeadLess = true;
#if DEBUG
                    IsHeadLess = false;
#endif
                    if (await base.LaunchPuppeteerBrowser(dominatorAccount, IsHeadLess,ExtensionsCollections:new List<string> {funCaptcha.CaptchaSettingPath }))
                    {
                        await ApplyKeyBoardShortCutToCaptchaExtension(dominatorAccount);
                        await ConfigureCaptchaExtension(dominatorAccount);
                        GlobusLogHelper.log.Info(Log.CustomMessage, dominatorAccount.AccountBaseModel.AccountNetwork,
                                    dominatorAccount.AccountBaseModel.UserName, "Captcha Solve",
                                    "Trying To Solve Captcha.Please Wait A While....");
                        var IsCaptchaSolved = await SolveCaptcha(dominatorAccount);
                        if (IsCaptchaSolved)
                        {
                            browserWindow.Refresh();
                            await Task.Delay(TimeSpan.FromSeconds(8));
                            browserWindow.ExecuteScript("document.querySelector('input[type=\"submit\" i]').click();", delayInSec: 7);
                            dominatorAccount.AccountBaseModel.Status = DominatorHouseCore.Enums.AccountStatus.Success;
                            GlobusLogHelper.log.Info(Log.CustomMessage, dominatorAccount.AccountBaseModel.AccountNetwork,
                                    dominatorAccount.AccountBaseModel.UserName, "Captcha Solve",
                                    "Successfully Solved Captcha.Please Do Browser Login Once To Update The Details.");
                        }
                        else
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage, dominatorAccount.AccountBaseModel.AccountNetwork,
                                    dominatorAccount.AccountBaseModel.UserName, "Captcha Solve",
                                    "Failed To Solve Captcha.");
                        }
                    }
                    IsRunning = false;
                }
                catch
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, dominatorAccount.AccountBaseModel.AccountNetwork,
                                 dominatorAccount.AccountBaseModel.UserName, "Captcha Solve",
                                 "Failed To Solve Captcha.");
                    IsRunning = false;
                }
                finally { if (PuppeteerBrowser != null) PuppeteerBrowser.ClosedBrowser(); }
            });
            while (IsRunning)
                Task.Delay(2000).Wait(dominatorAccount.Token);
        }
        public override async Task<bool> SolveCaptcha(DominatorAccountModel dominatorAccount)
        {
            try
            {
                var IsSolved = false;
                try
                {
                    await PuppeteerBrowser.ClearCookies();
                    await Task.Delay(TimeSpan.FromSeconds(5));
                    await PuppeteerBrowser.SetCookies(dominatorAccount);
                    await Task.Delay(TimeSpan.FromSeconds(5));
                    var html = PuppeteerBrowser.GetPageSource();
                    if (!html.Contains("Your account has been locked") && !html.Contains("arkose_iframe") && !(html.Contains("SideNav_AccountSwitcher_Button") || html.Contains("AppTabBar_More_Menu")))
                    {
                        await PuppeteerBrowser.GotoCustomUrl(dominatorAccount, dominatorAccount.Token, "https://x.com/home", delayInSec: 10);
                        var usernameBoxXandY = PuppeteerBrowser.GetXAndY(AttributeType1.ClassName, "r-30o5oe r-1niwhzg r-17gur6a r-1yadl64 r-deolkf r-homxoj r-poiln3 r-7cikom r-1ny4l3l r-t60dpp r-1dz5y72 r-fdjqy7 r-13qz1uu", 0);
                        await PuppeteerBrowser.MouseClickAsync(usernameBoxXandY.Key + 5, usernameBoxXandY.Value + 5, delayBefore: 2, delayAfter: 5);

                        //Entering username
                        await PuppeteerBrowser.EnterCharsAsync(dominatorAccount.AccountBaseModel.UserName, .009);
                        await Task.Delay(TimeSpan.FromSeconds(10));

                        //Click on next
                        await PuppeteerBrowser.ExecuteScriptAsync("[...document.querySelectorAll('button[role=\"button\"]')].filter(x=>x.textContent.trim()===\"Next\")[0].click();", delayInSec: 5);
                        if (PuppeteerBrowser.GetPageSource().Contains("There was unusual login activity on your account"))
                        {
                            await PuppeteerBrowser.EnterCharsAsync(dominatorAccount.AccountBaseModel.UserName, .009, delayAtLast: 7);
                            await PuppeteerBrowser.ExecuteScriptAsync("[...document.querySelectorAll('button[role=\"button\"]')].filter(x=>x.textContent.trim()===\"Next\")[0].click();", delayInSec: 5);
                        }
                        //Entering password
                        await PuppeteerBrowser.EnterCharsAsync(dominatorAccount.AccountBaseModel.Password, .009, delayAtLast: 7);

                        //Clicking on the LogIn
                        await PuppeteerBrowser.ExecuteScriptAsync("[...document.querySelectorAll('button[role=\"button\"]')].filter(x=>x.textContent.trim()===\"Log in\")[0].click();", delayInSec: 5);
                        await Task.Delay(TimeSpan.FromSeconds(10));
                        html = PuppeteerBrowser.GetPageSource();
                    }
                Exit:
                    IsSolved = !string.IsNullOrEmpty(html) && html.Contains("Account unlocked");
                    if (IsSolved)
                    {
                        try
                        {
                            if (viewModel != null && viewModel.CheckBalance != null)
                                viewModel.CheckBalance.Execute();
                        }
                        catch { }
                    }
                }
                catch { IsSolved = false; }
                return IsSolved;
            }catch(Exception ex) { return false; }
        }

        public override async Task<bool> ConfigureCaptchaExtension(DominatorAccountModel dominatorAccount)
        {
            var IsConfigured = false;
            try
            {
                await PuppeteerBrowser.GotoCustomUrl(dominatorAccount, dominatorAccount.Token, TdConstants.ExtensionShortCutAPI?.Replace("/shortcuts", ""), 5);
                var IdResult = await PuppeteerBrowser.ExecuteScriptAsync(TdConstants.GetIdOfExtension(0), delayInSec: 3);
                if (IdResult != null && !string.IsNullOrEmpty(IdResult.Result?.ToString()))
                {
                    await PuppeteerBrowser.GotoCustomUrl(dominatorAccount, dominatorAccount.Token, TdConstants.GetExtensionSettingApi(IdResult?.Result?.ToString()?.Replace("\"", "")), 5);
                    var isEnabled = await PuppeteerBrowser.ExecuteScriptAsync("document.querySelector('div[role=\"switch\" i]').ariaChecked", delayInSec: 3);
                    if (isEnabled != null && isEnabled.Success && isEnabled.Result?.ToString()?.ToLower() == "false")
                        await PuppeteerBrowser.ExecuteScriptAsync("document.querySelector('div[role=\"switch\" i]').click();", delayInSec: 3);
                    var IsKeyApplied = await PuppeteerBrowser.ExecuteScriptAsync("document.querySelector('[class=\"q-field__native q-placeholder\"]').placeholder", delayInSec: 3);
                    if (IsKeyApplied != null && IsKeyApplied.Success && !string.IsNullOrEmpty(IsKeyApplied.Result?.ToString()))
                    {
                        if (funCaptcha.APIKey?.Trim() != IsKeyApplied.Result?.ToString()?.Trim())
                        {
                            var xy = PuppeteerBrowser.GetXAndY(AttributeType1.ClassName, "q-field__native q-placeholder", 0);
                            await PuppeteerBrowser.MouseClickAsync(xy.Key + 10, xy.Value + 4, delayAfter: 4);
                            await PuppeteerBrowser.PressControlWithkey(winkeyCode: 65, delayInSec: 3);
                            await PuppeteerBrowser.PressAnyKeyUpdated(winKeyCode: 8, delayAtLast: 4);
                            await PuppeteerBrowser.EnterCharsAsync(funCaptcha.APIKey, delayAtLast: 4);
                            await PuppeteerBrowser.MouseClickAsync(xy.Key + 20, xy.Value - 50, delayAfter: 4);
                        }
                        if (!string.IsNullOrEmpty(dominatorAccount.AccountBaseModel.AccountProxy.ProxyIp) && !string.IsNullOrEmpty(dominatorAccount.AccountBaseModel.AccountProxy.ProxyPort))
                        {
                            await PuppeteerBrowser.ExecuteScriptAsync("[...document.querySelectorAll('[role=\"listitem\" i]')].filter(x=>x.innerText==\"Proxy\")[0].querySelector('[role=\"switch\" i]').scrollIntoViewIfNeeded();", delayInSec: 5);
                            var EnabledProxy = await PuppeteerBrowser.ExecuteScriptAsync("[...document.querySelectorAll('[role=\"listitem\" i]')].filter(x=>x.innerText==\"Proxy\")[0].querySelector('[role=\"switch\" i]').ariaChecked", delayInSec: 3);
                            if(EnabledProxy != null && EnabledProxy.Success && EnabledProxy.Result?.ToString()=="false")
                            {
                                await PuppeteerBrowser.ExecuteScriptAsync("[...document.querySelectorAll('[role=\"listitem\" i]')].filter(x=>x.innerText==\"Proxy\")[0].querySelector('[role=\"switch\" i]').click();", delayInSec: 3);
                                var xyProxy = PuppeteerBrowser.GetXAndY(AttributeType1.ClassName, "captcha-proxy-host", 0);
                                await PuppeteerBrowser.MouseClickAsync(xyProxy.Key + 40, xyProxy.Value + 50, delayAfter: 8);
                                PuppeteerBrowser.SelectAllText();
                                await PuppeteerBrowser.PressAnyKeyUpdated(winKeyCode: 8, delayAtLast: 3);
                                await PuppeteerBrowser.EnterCharsAsync(dominatorAccount.AccountBaseModel.AccountProxy.ProxyIp, delayAtLast: 3);
                                xyProxy = PuppeteerBrowser.GetXAndY(AttributeType1.ClassName, "captcha-proxy-port", 0);
                                await PuppeteerBrowser.MouseClickAsync(xyProxy.Key + 40, xyProxy.Value + 50, delayAfter: 4);
                                PuppeteerBrowser.SelectAllText();
                                await PuppeteerBrowser.PressAnyKeyUpdated(winKeyCode: 8, delayAtLast: 3);
                                await PuppeteerBrowser.EnterCharsAsync(dominatorAccount.AccountBaseModel.AccountProxy.ProxyPort, delayAtLast: 3);
                                if(!string.IsNullOrEmpty(dominatorAccount.AccountBaseModel.AccountProxy.ProxyUsername) && !string.IsNullOrEmpty(dominatorAccount.AccountBaseModel.AccountProxy.ProxyPassword))
                                {
                                    xyProxy = PuppeteerBrowser.GetXAndY(AttributeType1.ClassName, "captcha-proxy-login", 0);
                                    await PuppeteerBrowser.MouseClickAsync(xyProxy.Key + 40, xyProxy.Value + 50, delayAfter: 4);
                                    PuppeteerBrowser.SelectAllText();
                                    await PuppeteerBrowser.PressAnyKeyUpdated(winKeyCode: 8, delayAtLast: 3);
                                    await PuppeteerBrowser.EnterCharsAsync(dominatorAccount.AccountBaseModel.AccountProxy.ProxyUsername, delayAtLast: 3);
                                    xyProxy = PuppeteerBrowser.GetXAndY(AttributeType1.ClassName, "captcha-proxy-password", 0);
                                    await PuppeteerBrowser.MouseClickAsync(xyProxy.Key + 40, xyProxy.Value + 50, delayAfter: 4);
                                    PuppeteerBrowser.SelectAllText();
                                    await PuppeteerBrowser.PressAnyKeyUpdated(winKeyCode: 8, delayAtLast: 3);
                                    await PuppeteerBrowser.EnterCharsAsync(dominatorAccount.AccountBaseModel.AccountProxy.ProxyPassword, delayAtLast: 3);
                                }
                            }
                        }
                    }
                }
                IsConfigured = true;
            }
            catch (Exception ex) { IsConfigured = false; }
            return IsConfigured;
        }

        public override async Task ApplyKeyBoardShortCutToCaptchaExtension(DominatorAccountModel dominatorAccount)
        {
                try
                {
                    await PuppeteerBrowser.ExecuteScriptAsync("document.querySelector('privacy-sandbox-notice-dialog-app').shadowRoot.querySelector('div[role=\"main\"]>div>div[class=\"buttons-container\"]>cr-button[id=\"ackButton\"]').click();", delayInSec: 3);
                    await PuppeteerBrowser.GotoCustomUrl(dominatorAccount, dominatorAccount.Token, TdConstants.ExtensionShortCutAPI);
                    var ShortCutResponse = await PuppeteerBrowser.ExecuteScriptAsync(TdConstants.GetShortCutScript(0), delayInSec: 3);
                    if (ShortCutResponse != null && !string.IsNullOrEmpty(ShortCutResponse.Result?.ToString()))
                    {
                        if (ShortCutResponse.Result?.ToString()?.ToLower() == "not set")
                        {
                            var IsClickedResult = await PuppeteerBrowser.ExecuteScriptAsync(TdConstants.ClickOnEditShortCut(0), delayInSec: 3);
                            if (IsClickedResult != null && IsClickedResult.Success)
                            {
                                await PuppeteerBrowser.PressControlWithkey(winkeyCode: 79, delayInSec: 4);
                            }
                        }
                    }
                }
                catch {}
        }
        public static string GetBrowserName(DominatorAccountModel accountModel,
            BrowserInstanceType browserInstanceType = BrowserInstanceType.Primary)
        {
            return browserInstanceType.Equals(BrowserInstanceType.Primary)
                ? accountModel.UserName
                : $"{accountModel.UserName}{browserInstanceType}";
        }
    }

    public enum BrowserInstanceType
    {
        Primary,
        Secondary,
        CheckAccountStatus,
        BrowserLogin
    }
}