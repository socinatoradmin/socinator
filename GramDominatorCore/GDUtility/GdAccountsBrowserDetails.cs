using DominatorHouseCore;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.PuppeteerBrowser;
using DominatorHouseCore.Utility;
using EmbeddedBrowser;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace GramDominatorCore.GDUtility
{
    public class GdAccountsBrowserDetails
    {
        private static GdAccountsBrowserDetails _instance;

        public static GdAccountsBrowserDetails GetInstance() => _instance ?? (_instance = new GdAccountsBrowserDetails());

        private GdAccountsBrowserDetails()
        {
            AccountBrowserCollections = new Dictionary<string, BrowserWindow>();
        }
        public Dictionary<string, BrowserWindow> AccountBrowserCollections { get; set; }

        public static readonly object LockBrowser = new object();

        private bool _isThisWasMe;

        public static void CloseAllBrowser(DominatorAccountModel dominatorAccountModel, bool isReturn = false)
        {
            if (isReturn)
                return;
            // BrowserWindow browserWindow; 
            CloseBrowser(dominatorAccountModel, BrowserInstanceType.Primary);
            CloseBrowser(dominatorAccountModel, BrowserInstanceType.Secondary);

        }

        public static void CloseBrowser(DominatorAccountModel dominatorAccountModel, BrowserInstanceType browserInstanceType)
        {
            try
            {
                BrowserWindow browserWindow;
                var name = GetBrowserName(dominatorAccountModel, browserInstanceType);
                GetInstance().AccountBrowserCollections.TryGetValue(name, out browserWindow);
                //browserWindow= GetInstance().AccountBrowserCollections[name];

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
        
        public void CreateBrowser(DominatorAccountModel dominatorAccountModel, BrowserInstanceType browserInstanceType = BrowserInstanceType.Primary, bool isSave = true)
        {
            BrowserWindow browser = null;
            var isCheckedLogin = false;
            Task.Factory.StartNew(() =>
            {
                Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    try
                    {
                        // browser._isLoggedIn
                        dominatorAccountModel.Token.ThrowIfCancellationRequested();
                        browser = new BrowserWindow(dominatorAccountModel) //, userAgent:"Mozilla/5.0 (Windows NT 6.3; Trident/7.0; rv:11.0) like Gecko"
                        {
                            Visibility = Visibility.Visible, IsLoggedIn = !browserInstanceType.Equals(BrowserInstanceType.Secondary)|| !browserInstanceType.Equals(BrowserInstanceType.Third)|| !browserInstanceType.Equals(BrowserInstanceType.Fourth) && dominatorAccountModel.BrowserCookieHelperList.Count != 0
                        };
                        browser.BrowserSetCookie();
                        if (browserInstanceType.Equals(BrowserInstanceType.BrowserLogin))
                            browser.Visibility = Visibility.Visible;
                        browser.WindowState = WindowState.Maximized;    
                        if (isSave)
                        {
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
                        }

                        browser.Show();
                        Thread.Sleep(TimeSpan.FromSeconds(2));
                        ProcessBrowserLogin(dominatorAccountModel, browser, ref isCheckedLogin);
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                });
                
            }, dominatorAccountModel.Token);
            while (!isCheckedLogin && !browserInstanceType.Equals(BrowserInstanceType.BrowserLogin))
            {
                if (_isThisWasMe)
                    break;
                Thread.Sleep(TimeSpan.FromSeconds(2));
            } 
        }

        private void ProcessBrowserLogin(DominatorAccountModel _account, BrowserWindow browserWindow, ref bool isCheckedLogin)
        {
            try
            {
                Thread.Sleep(2000);

                if (browserWindow.IsLoggedIn)
                {
                    _account.IsUserLoggedIn = true;
                    return;
                }
                var pageSource = browserWindow.GetPageSourceAsync().Result;
                if (pageSource.Contains("Phone number, username, or email") || pageSource.Contains("not-logged-in client-root"))
                {
                    // click on browser's location(325,329) to get focus on cef browser
                    browserWindow.MouseClickAsync(325, 329, delayBefore: 0.5, delayAfter: 0.5);

                    // Press Tab to get focus on Username textBox
                    browserWindow.PressAnyKeyUpdated(winKeyCode: 9);

                    // Enter username
                    browserWindow.EnterCharsAsync(" " + _account.AccountBaseModel.UserName, 0, delayAtLast: 1);

                    // Press Tab
                    browserWindow.PressAnyKeyUpdated(winKeyCode: 9);

                    // Enter password
                    browserWindow.EnterCharsAsync(" " + _account.AccountBaseModel.Password, 0, delayAtLast: 1);

                    // Press Enter
                    browserWindow.PressAnyKeyUpdated(winKeyCode: 9, delayAtLast: 1);

                    // Get Loaded PageSource
                    var updatedHtml = browserWindow.GetPageSourceAsync().Result;

                    var require = updatedHtml.Contains("choice_1") && updatedHtml.Contains("choice_0");
                    if (!require && !updatedHtml.Contains("Submit"))
                    {
                        //BrowserAct(ActType.ClickByClass, "_5f5mN");
                        browserWindow.BrowserActAsync(ActType.Click, AttributeType.ClassName, "sqdOP", delayAfter: 2, index: 1);
                    }
                }
                var result = browserWindow.GetPageSourceAsync().Result;
                if (result.Contains("This Was Me") && result.Contains("logged-in client-root"))
                {
                    _isThisWasMe = true;
                    isCheckedLogin = false;
                    browserWindow.IsLoggedIn = false;
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                          _account.AccountBaseModel.AccountNetwork,
                          _account.AccountBaseModel.UserName, "Login",
                          "Instagram's -this was me-challenge ,Please update your account once ");

                }
                if (!string.IsNullOrEmpty(result) && result.Contains(",\"has_profile_pic\":") && !result.Contains("This Was Me"))
                {
                    browserWindow.IsLoggedIn = true;
                    _account.IsUserLoggedIn = true;
                    browserWindow.LoadPostPage(true);
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            finally
            {
                isCheckedLogin = true;
            }
        }

        public static string GetBrowserName(DominatorAccountModel accountModel, BrowserInstanceType browserInstanceType = BrowserInstanceType.Primary)
            => browserInstanceType.Equals(BrowserInstanceType.Primary) ? accountModel.UserName : $"{accountModel.UserName}{browserInstanceType}";

    }

    public enum BrowserInstanceType
    {

        Primary,
        Secondary,
        Third,
        Fourth,
        CheckAccountStatus,
        BrowserLogin
    }
}
