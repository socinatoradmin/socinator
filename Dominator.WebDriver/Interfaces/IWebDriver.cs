namespace Dominator.WebDriver
{
    using Dominator.WebDriver.Models;
    using System;
    using System.Net;
    using System.Threading.Tasks;

    /// <summary>
    /// Chrome driver provider.
    /// <remarks>
    /// Use Selenium.WebDriver v.4.11. It's last version works with .Net Framework.
    /// In this reason we can't use construction like WebDriverWait, IWebElement etc.
    /// </remarks>
    /// </summary>
    public interface IWebDriver : IDisposable
    {
        /// <summary>
        /// Launch the developer console and subscribing to network events.
        /// </summary>
        Task RunDevToolsSession();

        /// <summary>
        /// Stop subscribing to network events.
        /// </summary>
        void StopDevToolsSession();

        /// <summary>
        /// Network events from developer console.
        /// </summary>
        event EventHandler<EventReceivedEventArgs> DevToolsEventReceived;

        /// <summary>
        /// This method return current url.
        /// </summary>
        /// <returns>Url string.</returns>
        string GetCurrentUrl();

        /// <summary>
        /// Slide interactive browser element by xpath.
        /// </summary>
        /// <param name="xPath">XPath.</param>
        void Slide(string xPath);

        /// <summary>
        /// Check is the element exists by xpath.
        /// </summary>
        /// <param name="xPath">XPath.</param>
        /// <returns>True o false</returns>
        bool IsExists(string xPath);

        /// <summary>
        /// Open new tab in browser and switch to.
        /// </summary>
        void SwitchToNewWindow();

        /// <summary>
        /// Open url in browser.
        /// </summary>
        /// <param name="url">Address url.</param>
        void GoToUrl(string url);

        /// <summary>
        /// Refresh page.
        /// </summary>
        void Refresh();

        /// <summary>
        /// Wait until page complete loading after action.
        /// </summary>
        /// <param name="doing">Any action.</param>
        /// <param name="timeSpan">Timeout for action.</param>
        void WaitForPageToLoad(Action doing, TimeSpan timeSpan);

        /// <summary>
        /// Return node content.
        /// </summary>
        /// <param name="tagName">Tag name.</param>
        /// <returns>Html string</returns>
        string GetContent(string tagName = "body");

        /// <summary>
        /// Return page source.
        /// </summary>
        /// <returns>Html string</returns>
        string PageSource();

        /// <summary>
        /// Return script content from page.
        /// </summary>
        string GetScriptContent(string htmlString, string searchKey);

        /// <summary>
        /// Return js script result.
        /// </summary>
        /// <param name="script">Js script.</param>
        /// <returns>Js script object result.</returns>
        object ExecuteScript(string script);

        /// <summary>
        /// Add cookies from collection to browser page session.
        /// </summary>
        /// <param name="cookies">Cookie collection.</param>
        void AddCookies(CookieCollection cookies);

        /// <summary>
        /// Find and return element.
        /// </summary>
        /// <param name="xPath">XPath.</param>
        object FindElement(string xPath);

        /// <summary>
        /// Find and click to element.
        /// </summary>
        /// <param name="xPath">XPath.</param>
        void FindElementAndClick(string xPath);

        /// <summary>
        /// Send key or file path to element.
        /// </summary>
        /// <param name="xPath">XPath.</param>
        /// <param name="path">Key like Keys.Right or file path.</param>
        void FindElementAndSendKeys(string xPath, string path);

        /// <summary>
        /// Select and replace text in element.
        /// </summary>
        /// <param name="xPath">XPath.</param>
        /// <param name="text">Any text.</param>
        void FindElementAndSendKeysReplace(string xPath, string text);

        /// <summary>
        /// Wait until element is visible.
        /// </summary>
        /// <param name="xPath">XPath.</param>
        /// <param name="timeSpan">Timeout for waiting.</param>
        void WaitUntilElementIsVisible(string xPath, TimeSpan timeSpan);

        /// <summary>
        /// Find and switch to frame at page.
        /// </summary>
        /// <param name="xPath">XPath.</param>
        void SwitchToFrame(string xPath);

        /// <summary>
        /// Scroll page top down.
        /// </summary>
        /// <param name="maxScroll">Limit for max body height.</param>
        Task<int> ScrollBottomPage(long maxScroll = 100000);

        /// <summary>
        /// Scroll specific element top down.
        /// </summary>
        /// <param name="cssSelector">XPath.</param>
        /// <param name="maxScroll">Limit for max body height.</param>
        Task<int> ScrollBottomElement(string cssSelector, long maxScroll = 100000);
    }
}