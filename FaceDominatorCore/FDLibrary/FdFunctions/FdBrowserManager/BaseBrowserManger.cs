using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.PuppeteerBrowser;
using DominatorHouseCore.Request;
using DominatorHouseCore.Utility;
using EmbeddedBrowser;
using FaceDominatorCore.FDEnums;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDResponse.AccountsResponse;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace FaceDominatorCore.FDLibrary.FdFunctions.FdBrowserManager
{
    public interface IFdBaseBrowserManger : IBrowserManager
    {
        BrowserWindow BrowserWindow { get; set; }
        void CloseBrowser(DominatorAccountModel account);
        void CloseBrowserCustom(DominatorAccountModel account);
    }
    public class FdBaseBrowserManger : IFdBaseBrowserManger
    {
        public BrowserWindow BrowserWindow { get; set; }

        protected BrowserWindow CustomBrowserWindow { get; set; }

        protected CancellationToken cancellationToken;

        public bool BrowserLogin(DominatorAccountModel account, CancellationToken accountCancellationToken, LoginType loginType = LoginType.AutomationLogin,
             VerificationType verificationType = 0)
        {
            bool isRunning = true;
            cancellationToken = accountCancellationToken;
            var fbDtsg = string.Empty;
            var pageSourceData = string.Empty;
            var checkFor10Mins = DateTime.Now.AddMinutes(10);

            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    account.IsUserLoggedIn = false;

                    if (loginType == LoginType.UpdateDetails || loginType == LoginType.AutomationLogin || loginType == LoginType.BrowserLogin
                          || loginType == LoginType.InitialiseBrowser || (loginType == LoginType.CheckLogin && BrowserWindow.IsDisposed))
                        BrowserWindow = new BrowserWindow(account, isNeedResourceData: true)
                        { Visibility = Visibility.Hidden, IsLoggedIn = false };

                    if (loginType == LoginType.BrowserLogin)
                        BrowserWindow.Visibility = Visibility.Visible;

#if DEBUG
                    BrowserWindow.Visibility = Visibility.Visible;
#endif
                    if (account.BrowserCookies.Count == 0)
                        account.BrowserCookies = account.Cookies;
                    // Always make it Non async to avoid browserwindow visibility issue oe else it will open browser window
                    BrowserWindow.SetCookie();
                    BrowserWindow.Show();
                    await Task.Delay(5000, accountCancellationToken);
                    try
                    {
                        var last3Min = DateTime.Now.AddMinutes(2);

                        while (!BrowserWindow.IsLoggedIn && last3Min >= DateTime.Now)
                        {
                            if (!BrowserWindow.IsLoaded)
                            {
                                await Task.Delay(4000, accountCancellationToken);
                                if (!BrowserWindow.IsLoaded)
                                {
                                    BrowserWindow.Refresh();
                                    await Task.Delay(7000, accountCancellationToken);
                                    continue;
                                }
                            }

                            account.Token.ThrowIfCancellationRequested();

                            if (last3Min.AddMinutes(3.5) > DateTime.Now)
                            {
                                pageSourceData = await BrowserWindow.GetPageSourceAsync();
                                if (!BrowserWindow.IsLoggedIn && (pageSourceData.Contains("loginbutton") || pageSourceData.Contains("royal_login_button") || pageSourceData.Contains("royal_login_form") || (pageSourceData.Contains("name=\"email\"") && pageSourceData.Contains("name=\"pass\""))) && !pageSourceData.ToLower().Contains("the password that you've entered is incorrect"))//royal_login_button
                                {
                                    await BrowserWindow.GoToCustomUrl($"{FdConstants.FbHomeUrl}login", 3);

                                    await BrowserWindow.BrowserActAsync(ActType.EnterValue, AttributeType.Name, "email", value: account.AccountBaseModel.UserName, delayBefore: 3, delayAfter: 3);

                                    var xyValue = await BrowserWindow.GetXAndYAsync(customScriptX: "document.querySelectorAll('[name=\"pass\"]')[0].getBoundingClientRect().x", customScriptY: "document.querySelectorAll('[name=\"pass\"]')[0].getBoundingClientRect().y");

                                    await BrowserWindow.MouseClickAsync(xyValue.Key + 10, xyValue.Value);

                                    await BrowserWindow.EnterCharsAsync(account.AccountBaseModel.Password, delayBefore: 3, delayAtLast: 3);

                                    pageSourceData = await BrowserWindow.GetPageSourceAsync();

                                    var sumbitBtnClass = pageSourceData.Contains("_42ft _4jy0 _52e0 _4jy6 _4jy1 selected _51sy") ? "_42ft _4jy0 _52e0 _4jy6 _4jy1 selected _51sy"
                                    : "_42ft _4jy0 _6lth _4jy6 _4jy1 selected _51sy";
                                    await BrowserWindow.ExecuteScriptAsync($"{FdConstants.LoginButtonScriptBYClass(sumbitBtnClass)}[0].click()", delayInSec: 7);
                                    await LoadSource(6);
                                }
                            }
                            if (BrowserWindow.CurrentUrl().Contains("/checkpoint/") || BrowserWindow.CurrentUrl().Contains("/two_step_verification/") || BrowserWindow.CurrentUrl().Contains("/auth_platform/"))
                            {
                                GlobusLogHelper.log.Info(Log.CustomMessage, account.AccountBaseModel.AccountNetwork, account.AccountBaseModel.UserName, "Login", "Socinator ! I am waiting for 2FA Code Submit,Please Check your 2FA device and submit code.");
                                await WaitForTwoFactorCodeSubmit(accountCancellationToken);
                                GlobusLogHelper.log.Info(Log.CustomMessage, account.AccountBaseModel.AccountNetwork, account.AccountBaseModel.UserName, "Login", "Socinator ! Waiting time is over.Sorry ! if you not able to submit code please try again later.");
                            }

                            if (!BrowserWindow.IsLoggedIn)
                            {
                                pageSourceData = await BrowserWindow.GetPageSourceAsync();

                                if (pageSourceData.Contains("<body></body>") || string.IsNullOrEmpty(pageSourceData))
                                {
                                    BrowserWindow.Refresh();
                                    await Task.Delay(TimeSpan.FromSeconds(10), accountCancellationToken);
                                }

                                if (!string.IsNullOrEmpty(pageSourceData) && (pageSourceData.Contains("profile_icon") || pageSourceData.Contains("/me/")) || pageSourceData.Contains("aria-label=\"Account\"") || pageSourceData.Contains("href=\"/friends/\"") || pageSourceData.Contains("aria-label=\"Your profile\""))
                                {
                                    await BrowserWindow.SaveCookies(loginType == LoginType.BrowserLogin);
                                    if (!BrowserWindow.CurrentUrl().Contains(FdConstants.FbHomeUrl))
                                        BrowserWindow.LoadPostPage(account.IsUserLoggedIn);
                                }

                                if ((pageSourceData.Contains("profile_icon") || pageSourceData.Contains("href=\"/me/\"")) || pageSourceData.Contains("aria-label=\"Account\"") || pageSourceData.Contains("href=\"/friends/\"") && account.IsUserLoggedIn
                                || pageSourceData.Contains("aria-label=\"Your profile\""))
                                {
                                    var loginHandler = new FdLoginResponseHandler(new ResponseParameter() { Response = pageSourceData }
                                    , false, account);

                                    account.SessionId = fbDtsg;
                                    account.AccountBaseModel.UserId = loginHandler.UserId;
                                    account.SessionId = loginHandler.FbDtsg;
                                    if (loginHandler.LoginStatus)
                                    {
                                        account.IsUserLoggedIn = true;
                                        account.AccountBaseModel.Status = AccountStatus.Success;
                                    }
                                    SocinatorAccountBuilder.Instance(account.AccountBaseModel.AccountId)
                                        .AddOrUpdateLoginStatus(account.IsUserLoggedIn)
                                        .AddOrUpdateDominatorAccountBase(account.AccountBaseModel)
                                        .AddOrUpdateBrowserCookies(account.Cookies)
                                        .AddOrUpdateCookies(account.Cookies)
                                        .SaveToBinFile();
                                }
                            }
                            if (!account.IsUserLoggedIn)
                            {
                                await Task.Delay(8000, accountCancellationToken);

                                pageSourceData = await BrowserWindow.GetPageSourceAsync();

                                FdConstants.IsWebFacebook = pageSourceData.Contains("https://web.facebook.com/");
                            }
                            if (account.IsUserLoggedIn || !string.IsNullOrEmpty(pageSourceData))
                                break;
                        }

                        if (!account.IsUserLoggedIn)
                        {
                            var loginHandler = new FdLoginResponseHandler(new ResponseParameter() { Response = pageSourceData }
                                , false, account);

                            if (pageSourceData.Contains("/login.php?lhsrc=h_noacct") || pageSourceData.Contains("/recover/initiate/"))
                            {
                                account.AccountBaseModel.Status = AccountStatus.InvalidCredentials;
                            }
                            else if (pageSourceData.Contains("<body></body>") && !string.IsNullOrEmpty(account.AccountBaseModel.AccountProxy.ProxyIp)
                                    && !string.IsNullOrEmpty(account.AccountBaseModel.AccountProxy.ProxyIp))
                            {
                                var proxyFileManager = InstanceProvider.GetInstance<DominatorHouseCore.FileManagers.IProxyFileManager>();
                                if (!proxyFileManager.VerifyProxy(account.AccountBaseModel.AccountProxy,
                                    ConstantVariable.GoogleLink))
                                {
                                    account.AccountBaseModel.Status = AccountStatus.ProxyNotWorking;
                                }

                            }
                            else if (!loginHandler.LoginStatus && loginHandler.FbErrorDetails != null)
                            {
                                if (loginHandler.FbErrorDetails.FacebookErrors == FacebookErrors.CheckPoint)
                                    account.AccountBaseModel.Status = AccountStatus.NeedsVerification;
                                else if (loginHandler.FbErrorDetails.FacebookErrors == FacebookErrors.AccountDisbled)
                                    account.AccountBaseModel.Status = AccountStatus.PermanentlyBlocked;
                                else if (loginHandler.FbErrorDetails.FacebookErrors == FacebookErrors.InvalidLogin)
                                    account.AccountBaseModel.Status = AccountStatus.InvalidCredentials;
                            }
                            else if (pageSourceData.Contains("Enter mobile number"))
                            {
                                account.IsUserLoggedIn = false;
                                account.AccountBaseModel.Status = AccountStatus.AddPhoneNumberToYourAccount;
                                loginHandler.LoginStatus = false;
                            }

                            if (loginHandler.LoginStatus)
                            {
                                account.IsUserLoggedIn = true;
                                account.AccountBaseModel.Status = AccountStatus.Success;
                            }


                            SocinatorAccountBuilder.Instance(account.AccountBaseModel.AccountId)
                                        .AddOrUpdateLoginStatus(account.IsUserLoggedIn)
                                        .AddOrUpdateDominatorAccountBase(account.AccountBaseModel)
                                        .AddOrUpdateBrowserCookies(account.BrowserCookies)
                                        .SaveToBinFile();
                        }

                    }
                    catch (OperationCanceledException)
                    {

                    }
                    finally
                    {
                        UpdateAccount(account);
                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                isRunning = false;
            });
            while (isRunning && checkFor10Mins > DateTime.Now)
                Task.Delay(1000).Wait(accountCancellationToken);

            if (loginType == LoginType.InitialiseBrowser)
                CloseBrowser(account);

            return account.IsUserLoggedIn;
        }
        public async Task LoadSource(int timesec = 15)
        {
            DateTime currentTime = DateTime.Now;
            string pageSource;
            try
            {
                do
                {
                    await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
                    pageSource = await BrowserWindow.GetPageSourceAsync();
                    cancellationToken.ThrowIfCancellationRequested();
                } while ((!BrowserWindow.IsLoaded && (DateTime.Now - currentTime).TotalSeconds < timesec + 15) || (string.IsNullOrEmpty(pageSource) && (DateTime.Now - currentTime).TotalSeconds < timesec));
            }
            catch (Exception)
            { }
        }
        private async Task WaitForTwoFactorCodeSubmit(CancellationToken accountCancellationToken)
        {
            var counter = 0;
            try
            {
                while (counter++ < 12 && (BrowserWindow.CurrentUrl().Contains("/checkpoint/") || BrowserWindow.CurrentUrl().Contains("/two_step_verification/") || BrowserWindow.CurrentUrl().Contains("/auth_platform/")))
                    await Task.Delay(15000, accountCancellationToken);
            }
            catch { }
        }
        public void UpdateAccount(DominatorAccountModel account)
        {
            if (account.AccountBaseModel.UserName == "socinator" && account.AccountBaseModel.Password == "socinator")
                return;
            var globalDbOperation =
                        new DbOperations(SocinatorInitialize.GetGlobalDatabase().GetSqlConnection());
            globalDbOperation.UpdateAccountDetails(account);
        }

        public void CloseBrowser(DominatorAccountModel account)
        {
            try
            {

                if (BrowserWindow != null)
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        BrowserWindow.Close();
                        BrowserWindow.Dispose();
                        BrowserWindow = null;
                        GC.Collect();
                    });
            }
            catch (Exception ex)
            { ex.DebugLog(); }
            Thread.Sleep(1000);
            #region Closing Browser Asnynchronously
            //bool isRunning = true;
            //try
            //{
            //    if (BrowserWindow == null) return;
            //    Application.Current.Dispatcher.Invoke(async () =>
            //    {
            //        try
            //        {
            //            BrowserWindow.Close();
            //            BrowserWindow.Dispose();
            //            await Task.Delay(1000);
            //            isRunning = false;
            //        }
            //        catch (Exception)
            //        {
            //            isRunning = false;
            //        }
            //    });
            //}
            //catch (Exception)
            //{
            //    isRunning = false;
            //}
            //finally
            //{
            //    isRunning = false;
            //}
            //while (isRunning)
            //    Task.Delay(500).Wait();
            #endregion
        }

        public void CloseBrowserCustom(DominatorAccountModel account)
        {

            try
            {

                if (CustomBrowserWindow != null)
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        CustomBrowserWindow.Close();
                        CustomBrowserWindow.Dispose();
                        CustomBrowserWindow = null;
                        GC.Collect();
                    });
            }
            catch (Exception ex)
            { ex.DebugLog(); }
            Thread.Sleep(1000);
            #region Closing Custom Browser Asnynchronously
            //bool isRunning = true;
            //try
            //{
            //    if (CustomBrowserWindow == null) return;
            //    Application.Current.Dispatcher.Invoke(async () =>
            //    {
            //        try
            //        {
            //            CustomBrowserWindow.Close();
            //            CustomBrowserWindow.Dispose();
            //            await Task.Delay(1000);
            //            isRunning = false;
            //        }
            //        catch (Exception)
            //        {
            //            isRunning = false;
            //        }
            //    });
            //}
            //catch (Exception)
            //{
            //    isRunning = false;
            //}
            //finally
            //{
            //    isRunning = false;
            //}

            //while (isRunning)
            //    Task.Delay(500).Wait();
            #endregion
        }

    }
}