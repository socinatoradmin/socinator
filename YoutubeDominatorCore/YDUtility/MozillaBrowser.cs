using DominatorHouseCore;
using DominatorHouseCore.Models;
using DominatorHouseCore.PuppeteerBrowser;
using DominatorHouseCore.Utility;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Cookie = OpenQA.Selenium.Cookie;
using Proxy = DominatorHouseCore.Models.Proxy;

namespace YoutubeDominatorCore.YDUtility
{
    public class MozillaBrowser
    {
        private bool _browserClosed;
        private readonly CancellationToken _cancellationToken;
        private bool _initializeDriverOneOreTime = true;
        private Tuple<int, DateTime, DateTime> _thisProcessTuple;
        public IWebDriver WebDriver;

        public MozillaBrowser(DominatorAccountModel accountModel, CancellationToken cancellationTokenSource,
            bool hiddenBrowser)
        {
            _cancellationToken = cancellationTokenSource;
            var startTime = DateTime.Now;
            try
            {
                var firefoxOptions = FirefoxSettings(accountModel.AccountBaseModel.AccountProxy, hiddenBrowser);

                cancellationTokenSource.ThrowIfCancellationRequested();
                if (!DriverInitializer(ref startTime, firefoxOptions, cancellationTokenSource)) return;

                // Minimize this firefox window
                WebDriver.Manage().Window.Minimize();

                // Set PageLoad Timeout
                WebDriver.Manage().Timeouts().PageLoad = TimeSpan.FromMinutes(2);

                cancellationTokenSource.ThrowIfCancellationRequested();
                // Open Youtube at start to initialize youtube cookies
                WebDriver.Navigate().GoToUrl("https://www.youtube.com/");
                Sleep(0.5);

                cancellationTokenSource.ThrowIfCancellationRequested();
                SetCookies(accountModel.Cookies, cancellationTokenSource);
            }
            catch (OperationCanceledException)
            {
                CloseByTimeIfDriverNull(startTime);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                CloseByTimeIfDriverNull(startTime);
            }
        }

        public bool FoundAd { get; set; }

        private void Sleep(double seconds = 1)
        {
            Task.Delay(TimeSpan.FromSeconds(seconds)).Wait(_cancellationToken);
        }

        private FirefoxOptions FirefoxSettings(Proxy proxy, bool hiddenBrowser)
        {
            var proxyString = !string.IsNullOrEmpty(proxy?.ProxyIp?.Trim())
                ? $"{proxy.ProxyIp.Trim()}:{proxy.ProxyPort?.Trim()}"
                : "";

            var firefoxOptions = new FirefoxOptions
            {
                // Proxy Configuration
                Proxy = new OpenQA.Selenium.Proxy
                {
                    Kind = ProxyKind.Manual,
                    IsAutoDetect = false,
                    HttpProxy = proxyString,
                    SslProxy = proxyString
                },

                // Delete the profile from the location after WebDriver being used
                Profile = new FirefoxProfile { DeleteAfterUse = true, AcceptUntrustedCertificates = true }
            };

            if (hiddenBrowser) // Headless Browser Configuration
                firefoxOptions.AddArguments("--headless");

            return firefoxOptions;
        }

        private bool DriverInitializer(ref DateTime startTime, FirefoxOptions firefoxOptions,
            CancellationToken cancellationTokenSource)
        {
        forInitializingDriverOneOreTime:
            var gotException = false;

            lock (Utilities.LockOpeningBrowser)
            {
                var exc = new Exception();
                try
                {
                    if (Utilities.AppClosing) return false;
                    if (cancellationTokenSource.IsCancellationRequested)
                        return false;

                    var driverService =
                        FirefoxDriverService.CreateDefaultService(ConstantVariable.GetPlatformBaseDirectory());
                    driverService.HideCommandPromptWindow = true;
                    startTime = DateTime.Now;
                    WebDriver = new FirefoxDriver(driverService, firefoxOptions, TimeSpan.FromSeconds(100));
                    if (Utilities.AppClosing)
                    {
                        try
                        {
                            WebDriver.Quit();
                        }
                        catch
                        {
                            /*Ignored*/
                        }

                        WebDriver = null;
                        return false;
                    }

                    _thisProcessTuple = Tuple.Create(driverService.ProcessId, startTime, DateTime.Now);
                    Utilities.RunningWebDrivers.Add(_thisProcessTuple);
                }
                catch (Exception ex)
                {
                    exc = ex;
                    gotException = WebDriver == null;
                    ex.DebugLog();
                }

                if (WebDriver == null)
                {
                    CloseByTimeIfDriverNull(startTime);
                    if (!_initializeDriverOneOreTime)
                    {
                        exc.DebugLog();
                        return false;
                    }
                }
            }

            if (gotException && _initializeDriverOneOreTime)
            {
                _initializeDriverOneOreTime = false;
                goto forInitializingDriverOneOreTime;
            }

            return true;
        }

        private void SetCookies(CookieCollection cookies, CancellationToken cancellationTokenSource)
        {
            IReadOnlyCollection<Cookie> hitYoutubeCookie = WebDriver.Manage().Cookies.AllCookies;

            WebDriver.Manage().Cookies.DeleteAllCookies();

            foreach (System.Net.Cookie o in cookies)
            {
                cancellationTokenSource.ThrowIfCancellationRequested();
                var temp = new Cookie(o.Name, o.Value, hitYoutubeCookie.First().Domain, hitYoutubeCookie.First().Path,
                    hitYoutubeCookie.First().Expiry);
                WebDriver.Manage().Cookies.AddCookie(temp);
            }
        }

        public bool StartWatching(string url, int delay, string videoTitle, bool skipAd = false)
        {
            if (WebDriver == null) return false;
            try
            {
                _cancellationToken.ThrowIfCancellationRequested();
                WebDriver.Navigate().GoToUrl(url);

                if (WebDriver.Title.Contains("Oops! Something went"))
                {
                    Sleep();
                    var onlyUrl = Uri.UnescapeDataString(Utilities.GetBetween(url + "MyCustomAdded",
                        "feature=masthead_switcher&next=", "MyCustomAdded"));
                    WebDriver.Navigate().GoToUrl(onlyUrl);
                    Sleep(5);
                }

                //Play youtube video [video is paused by default in the browser]
                BrowserAct(ActType.ClickByClass, "ytp-play-button", 0.5);

                //Mute youtube video player Audio
                BrowserAct(ActType.ClickByClass /*ClickById*/, /*ytp-id-15*/"ytp-mute-button", 0.5);

                //BrowserAct(ActType.ClickByClass, ,0.5);
                BrowserAct(ActType.ClickById /*ClickByClass*/, "ytp-id-18" /*"ytp-button ytp-settings-button"*/, 0.5);

                //ytp-button ytp-settings-button
                //activate skipping add process after every 4.5 secs
                new Task(() => SkipAdInLoop(skipAd)).Start();

                var increaseTime = 0;
                while (delay > ++increaseTime)
                {
                    _cancellationToken.ThrowIfCancellationRequested();

                    while (FoundAd)
                    {
                        _cancellationToken.ThrowIfCancellationRequested();
                        Sleep();
                    }

                    Sleep();
                }

                return true;
            }
            catch (OperationCanceledException)
            {
                return false;
            }
            catch (Exception ex)
            {
                if (ex.InnerException?.Message != "A task was canceled.")
                    ex.DebugLog();
                return false;
            }
            finally
            {
                QuitDriver();
            }
        }

        private void SkipAdInLoop(bool skipAd)
        {
            while (!_browserClosed)
            {
                if (!_browserClosed)
                    FoundAd = GetElementValue(ActType.GetLengthByClass, "ytp-ad-skip-button-icon", 1.5) == "1";
                if (FoundAd)
                {
                    /*Just for Checking*/
                }

                if (!_browserClosed && skipAd && FoundAd)
                    BrowserAct(ActType.ClickByClass, "ytp-ad-skip-button-icon", 3.0); // for Skipping add
            }
        }

        private void QuitDriver()
        {
            _browserClosed = true;
            try
            {
                Utilities.RunningWebDrivers.Remove(_thisProcessTuple);
            }
            catch
            {
                /*Ignored*/
            }

            try
            {
                WebDriver.Quit();
            }
            catch
            {
                try
                {
                    WebDriver.Quit();
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }
        }

        /// <summary>
        ///     Mozilla Browser actions
        /// </summary>
        /// <param name="actType">Type of activity doing on browser window</param>
        /// <param name="element">type of element by which the action gonna be performed</param>
        /// <param name="delayBefore">delay before the action (In seconds)</param>
        /// <param name="delayAfter">delay after the action (In seconds)</param>
        /// <param name="clickIndex">Sometimes multiple buttons have same tag-value</param>
        private void BrowserAct(ActType actType, string element, double delayBefore = 0, double delayAfter = 0,
            int clickIndex = 0)
        {
            try
            {
                if (delayBefore > 0)
                    Sleep(delayBefore);

                if (_browserClosed) return;

                By findingElement = null;
                switch (actType)
                {
                    case ActType.ClickByClass:
                        findingElement = By.ClassName(element);
                        break;

                    case ActType.ClickById:
                        findingElement = By.Id(element);
                        break;
                }

                WebDriver.FindElements(findingElement)[clickIndex].Click();

                if (delayAfter > 0)
                    Sleep(delayAfter);
            }
            catch (Exception)
            {
                //Ignored
            }
        }

        /// <summary>
        ///     Browser actions
        /// </summary>
        /// <param name="actType">Type of activity doing on browser window</param>
        /// <param name="element">type of element by which the action gonna be performed</param>
        /// <param name="delayBefore">delay before the action (In seconds)</param>
        /// <param name="clickIndex">Sometimes multiple buttons have same tag-value</param>
        private string GetElementValue(ActType actType, string element, double delayBefore = 0)
        {
            if (delayBefore > 0)
                Sleep(delayBefore);

            if (_browserClosed) return "";
            try
            {
                By findingElement = null;
                switch (actType)
                {
                    case ActType.GetLengthByClass:
                        findingElement = By.ClassName(element);
                        break;

                    case ActType.ClickById:
                        findingElement = By.Id(element);
                        break;

                    case ActType.GetValueByTagName:
                        findingElement = By.TagName(element);
                        break;
                }

                if (element == "ytp-ad-skip-button-icon")
                {
                    WebDriver.FindElement(findingElement);
                    return "1";
                }

                var returnString = WebDriver.FindElement(findingElement).Text;
                return returnString;
            }
            catch (Exception)
            {
                return "";
            }
        }

        private void CloseByTimeIfDriverNull(DateTime startTime)
        {
            if (WebDriver != null)
                return;
            try
            {
                var allFirefoxProcessed = Process.GetProcessesByName("firefox").ToList();
                allFirefoxProcessed.AddRange(Process.GetProcessesByName("geckodriver"));
                foreach (var process in allFirefoxProcessed)
                    try
                    {
                        if (!process.HasExited && process.StartTime >= startTime &&
                            process.StartTime <= startTime.AddSeconds(105))
                            process.Kill();
                    }
                    catch (Exception)
                    {
                        /*Ignore*/
                    }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}