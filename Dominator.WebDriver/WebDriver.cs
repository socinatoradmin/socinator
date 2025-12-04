using Dominator.WebDriver.Models;
using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.DevTools;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
namespace Dominator.WebDriver
{
    /// <inheritdoc cref="IWebDriver"/>
    public class WebDriver : IWebDriver
    {
        private readonly ChromeDriver _driver;

        public event EventHandler<EventReceivedEventArgs> DevToolsEventReceived;

        public DevToolsSession GetDevToolsSession => _driver.GetDevToolsSession();

        /// <summary>
        ///  Initializes a new instance of the <see cref="WebDriver"/> class.
        /// </summary>
        /// <param name="driver">ChromeDriver</param>
        public WebDriver(ChromeDriver driver)
        {
            _driver = driver;
        }
        public WebDriver()
        {

        }

        public void Dispose()
        {
            _driver.Quit();
            _driver.Dispose();
        }

        public string GetCurrentUrl() => _driver.Url;

        public async Task RunDevToolsSession()
        {
            var session = _driver.GetDevToolsSession();
            await session.Domains.Network.EnableNetwork(); 
            session.DevToolsEventReceived += (sender, e) =>
            {
                var args = e.EventData;
                DevToolsEventReceived?.Invoke(this, new EventReceivedEventArgs(args.ToString()));
            };
        }

        public void StopDevToolsSession()
        {
            _driver.CloseDevToolsSession();
        }

        public void GoToUrl(string url)
        {
            _driver.Navigate().GoToUrl(url);
        }

        public void SwitchToNewWindow()
        {
            _driver.SwitchTo().NewWindow(WindowType.Tab);
        }

        public void Refresh()
        {
            _driver.Navigate().Refresh();
        }

        public object ExecuteScript(string script)
        {
            return ((IJavaScriptExecutor)_driver).ExecuteScript(script);
        }

        public void WaitUntilElementIsVisible(string xPath, TimeSpan timeSpan)
        {
            //ToDo: Add Wrapper for WebDriverWait
            var wait = new WebDriverWait(_driver, timeSpan);
            //wait.Until(ExpectedConditions.ElementIsVisible(By.XPath(xPath)));
        }

        public string GetContent(string tagName = "body")
        {
            return _driver.FindElement(By.TagName(tagName)).Text;
        }

        public string PageSource() => _driver.PageSource;

        public string GetScriptContent(string htmlString, string searchKey)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(htmlString);
            return doc.DocumentNode.Descendants()
                         .Where(n => n.Name == "script" && n.InnerText.Contains(searchKey))
                         .Select(x => x.InnerText)
                         .FirstOrDefault();
        }

        public object FindElement(string xPath)
        {
            return FindElementSafety(By.XPath(xPath));
        }

        public void FindElementAndClick(string xPath)
        {
            var element = FindElementSafety(By.XPath(xPath));
            element?.Click();
        }

        public void SwitchToFrame(string xPath)
        {
            var iframe = _driver.FindElement(By.XPath(xPath));
            _driver.SwitchTo().Frame(iframe);
        }

        public void FindElementAndSendKeys(string xPath, string text)
        {
            _driver.FindElement(By.XPath(xPath)).SendKeys(text);
        }

        public void Slide(string xPath)
        {
            var elem = _driver.FindElement(By.XPath(xPath));
            var move = new Actions(_driver);
            //var action = move.DragAndDropToOffset(elem, 30, 0).Build();
            //action.Perform();

            var sourceElement = _driver.FindElement(By.XPath(xPath));
            var targetElement = _driver.FindElement(By.XPath(""));
            Actions mouseActionBuilder = new Actions(_driver);
            Actions dragAndDrop = mouseActionBuilder.ClickAndHold(sourceElement).MoveToElement(targetElement).Release(targetElement);
            dragAndDrop.Build().Perform();
        }

        public void FindElementAndSendKeysReplace(string xPath, string text)
        {
            _driver.FindElement(By.XPath(xPath)).SendKeys(Keys.Control + "a");
            _driver.FindElement(By.XPath(xPath)).SendKeys(Keys.Backspace);
            _driver.FindElement(By.XPath(xPath)).SendKeys(text);
        }

        public void AddCookies(CookieCollection cookies)
        {
            var mainDomain = _driver.ExecuteScript("return document.domain;")?.ToString();
            mainDomain = mainDomain.Replace("www", string.Empty);

            foreach (System.Net.Cookie cook in cookies)
            {
                if (!cook.Domain.Equals(mainDomain))
                {
                    //Skip not domain cookies to escape InvalidCookieDomainException
                    continue;
                }

                var cookie = new OpenQA.Selenium.Cookie(
                    name: cook.Name,
                    value: cook.Value,
                    domain: cook.Domain,
                    path: cook.Path,
                    expiry: null);
                _driver.Manage().Cookies.AddCookie(cookie);
            }
        }
        
        public void WaitForPageToLoad(Action doing, TimeSpan timeSpan)
        {
            IWebElement oldPage = _driver.FindElement(By.TagName("html"));
            doing();
            var wait = new WebDriverWait(_driver, timeSpan);
            try
            {
                //wait.Until(driver => ExpectedConditions.StalenessOf(oldPage)(_driver) &&
                //    ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
            }
            catch (Exception pageLoadWaitError)
            {
                throw new TimeoutException("Timeout during page load", pageLoadWaitError);
            }
        }

        public async Task<int> ScrollBottomPage(long maxScroll = 100000)
        {
            var scrollCount = 0;
            var jsBodyHeight = "return document.body.scrollHeight;";
            var height = (long)_driver.ExecuteScript(jsBodyHeight);

            while (height < maxScroll)
            {
                _driver.ExecuteScript($"window.scrollTo(0, {height})");

                //ToDo: Change to waiting for psrt of videos loading
                await Task.Delay(TimeSpan.FromSeconds(4));
                var newHeight = (long)_driver.ExecuteScript(jsBodyHeight);
                if (height == newHeight)
                {
                    maxScroll = newHeight;
                }
                height = newHeight;
                scrollCount = scrollCount + 1;
            }
            return scrollCount;
        }

        //ToDo: It can bt several billions items on page. How scroll all of it?
        public async Task<int> ScrollBottomElement(string cssSelector, long maxScroll = 100000)
        {
            var scrollCount = 0;
            var bottomReached = false;

            var getHeightScript = $"return document.querySelector(\"{cssSelector}\")?.scrollHeight ?? 0";
            var currentHeight = (long)_driver.ExecuteScript(getHeightScript);
            if (currentHeight == 0)
            {
                //Where is no cssSelector element
                return scrollCount;
            }

            do
            {
                //ToDo: Is it possible to jump min position of scrollBy?
                var scrollScript = $"document.querySelector(\"{cssSelector}\").scrollBy(0, {currentHeight})";
                _driver.ExecuteScript(scrollScript);

                //ToDo: Wait for scrolled elements load
                await Task.Delay(TimeSpan.FromSeconds(3));

                var newHeight = (long)_driver.ExecuteScript(getHeightScript);
                bottomReached = currentHeight >= newHeight;
                currentHeight = newHeight;
                scrollCount = scrollCount + 1;
            } while (!bottomReached && currentHeight < maxScroll);
            return scrollCount;
        }

        public bool IsExists(string xPath)
        {
            return FindElementSafety(By.XPath(xPath)) != null;
        }

        private IWebElement FindElementSafety(By by)
        {
            try
            {
                return _driver.FindElement(by);
            }
            catch (Exception)
            {
                // Don't need break action, hide exception and return null instead of exception
                return null;
            }
        }
    }
}