using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using DominatorHouseCore;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using Cookie = OpenQA.Selenium.Cookie;
using Proxy = OpenQA.Selenium.Proxy;

namespace QuoraDominatorCore.QdLibrary.MozillaBrowsers
{
    public class MozillaBrowser
    {
        public static readonly object lockBrowserInitializer = new object();

        /// <summary>
        ///     Mozilla Browser actions
        /// </summary>
        /// <param name="actType">Type of activity doing on browser window</param>
        /// <param name="element">type of element by which the action gonna be performed</param>
        /// <param name="delayBefore">delay before the action (In seconds)</param>
        /// <param name="delayAfter">delay after the action (In seconds)</param>
        /// <param name="value">value which is going to be entered</param>
        /// <param name="clickIndex">Sometimes multiple buttons have same tag-value</param>
        private readonly object DownloadGecko = new object();

        public string PageResponse = string.Empty;
        public IWebDriver WebDriver;

        public MozillaBrowser(DominatorAccountModel accountModel)
        {
            try
            {
                var firefoxOptions = new FirefoxOptions();

                DownloadGeckoDriver();

                #region Headless Configuration

                firefoxOptions.AddArguments("--headless");

                #endregion

                #region Open Youtube at start

                firefoxOptions.AddArgument("https://www.quora.com/");

                #endregion

                var profile = new FirefoxProfile();

                #region Proxy Configuration

                var proxy = new Proxy
                {
                    Kind = ProxyKind.Manual,
                    IsAutoDetect = false
                };
                proxy.HttpProxy = proxy.SslProxy =
                    !string.IsNullOrEmpty(accountModel.AccountBaseModel.AccountProxy.ProxyIp)
                        ? $"{accountModel.AccountBaseModel.AccountProxy.ProxyIp}:{accountModel.AccountBaseModel.AccountProxy.ProxyPort}"
                        : "";

                firefoxOptions.Proxy = proxy;
                profile.SetProxyPreferences(proxy);

                #endregion

                profile.DeleteAfterUse = true;

                firefoxOptions.Profile = profile;

                lock (lockBrowserInitializer)
                {
                    var driverService = FirefoxDriverService.CreateDefaultService(
                        $@"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\Socinator\\Configurations");
                    driverService.HideCommandPromptWindow = true;

                    WebDriver = new FirefoxDriver(driverService, firefoxOptions);

                    if (WebDriver == null)
                        return;

                    #region Set PageLoad Timeout

                    WebDriver.Manage().Timeouts().PageLoad = TimeSpan.FromMinutes(100);

                    #endregion
                }

                #region Managing coockie

                Thread.Sleep(2000);
                IReadOnlyCollection<Cookie> hitYoutubeCookie = WebDriver.Manage().Cookies.AllCookies;

                WebDriver.Manage().Cookies.DeleteAllCookies();

                foreach (System.Net.Cookie o in accountModel.Cookies)
                {
                    var temp = new Cookie(o.Name, o.Value, hitYoutubeCookie.First().Domain,
                        hitYoutubeCookie.First().Path, hitYoutubeCookie.First().Expiry);
                    WebDriver.Manage().Cookies.AddCookie(temp);
                }

                #endregion
            }
            catch (Exception)
            {
                /*Ignored*/
            }
        }

        public bool StartBrowsing(string url, int numberOfPage = 10)
        {
            if (WebDriver == null) return false;
            try
            {
                lock (lockBrowserInitializer)
                {
                    WebDriver.Navigate().GoToUrl(url);
                }

                try
                {
                    Thread.Sleep(7000);
                    for (var i = 0; i <= numberOfPage; i++)
                    {
                        var js = (IJavaScriptExecutor) WebDriver;
                        js.ExecuteScript("window.scrollBy(0,6000)", "");
                        Thread.Sleep(3000);
                    }

                    PageResponse = WebDriver.PageSource;
                }
                catch (Exception)
                {
                    /*Ignored*/
                }
            }
            catch (Exception)
            {
                /*Ignored*/
            }
            finally
            {
                try
                {
                    WebDriver.Close();
                    WebDriver.Quit();
                    WebDriver.Dispose();
                }
                catch (Exception)
                {
                    /*Ignored*/
                }
            }

            return true;
        }

        public void DownloadGeckoDriver()
        {
            try
            {
                lock (DownloadGecko)
                {
                    var baseDir =
                        $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\Socinator\\Configurations";

                    if (!Directory.Exists(baseDir)) Directory.CreateDirectory(baseDir);

                    if (File.Exists($"{baseDir}/geckodriver.exe"))
                        return;

                    GlobusLogHelper.log.Info(
                        "Downloading driver to run mozilla browser (Downloading only for the first time). Please wait..");

                    var imgBytes = new WebClient().DownloadData("http://209.250.252.53/GeckoDriver/geckodriver.exe");
                    File.WriteAllBytes($"{baseDir}/geckodriver.exe", imgBytes);

                    GlobusLogHelper.log.Info("Completed downloading driver process.");
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}