using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using CefSharp;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using EmbeddedBrowser;
using HtmlAgilityPack;
using LinkedDominatorCore.Utility;

namespace LinkedDominatorCore.LDUtility
{
    public class BrowserAutomationExtension
    {
        private BrowserWindow _browserWindow;

        public BrowserAutomationExtension(BrowserWindow browserWindow)
        {
            _browserWindow = browserWindow;
            if (browserWindow != null)
                CancellationToken = browserWindow.DominatorAccountModel.Token;
        }

        public BrowserAutomationExtension(BrowserWindow browserWindow, CancellationToken cancellationToken)
        {
            _browserWindow = browserWindow;
            CancellationToken = cancellationToken;
        }

        public CancellationToken CancellationToken { get; set; }
        protected ActivityType ActivityType { get; set; }


        public string GetPath(string pageSource, string attributeType, AttributeIdentifierType attributeIdentifierType,
            params string[] containsList)
        {
            var id = GetPathList(pageSource, attributeType, attributeIdentifierType, true, containsList)
                ?.FirstOrDefault();
            return id ?? "";
        }

        public string GetPath(string pageSource, string attributeType, AttributeIdentifierType attributeIdentifierType,
            string[] containsList,
            int index = 0)
        {
            var pathList = GetPathList(pageSource, attributeType, attributeIdentifierType, true, containsList);
            var id = index == 0 ? pathList.FirstOrDefault() : pathList.Count >= index ? pathList[index] : string.Empty;
            return id ?? "";
        }

        public List<string> GetPathList(string pageSource, string attributeType,
            AttributeIdentifierType attributeIdentifierType, bool isFirstOnly, params string[] containsList)
        {
            var htmlDoc = new HtmlDocument();

            htmlDoc.LoadHtml(pageSource);

            var nodeCollection = htmlDoc.DocumentNode.SelectNodes($"//{attributeType}");
            var idOrClassNameList = new List<string>();
            try
            {
                foreach (var node in nodeCollection)
                {
                    var outerHtml = node.OuterHtml;
                    var idOrClassName = "";
                    if (!Utils.IsContains(outerHtml, containsList)
                        || attributeIdentifierType.Equals(AttributeIdentifierType.Id) && string.IsNullOrEmpty(node?.Id))
                        continue;

                    if (attributeIdentifierType.Equals(AttributeIdentifierType.Id))
                        idOrClassName = node.Id;
                    else if (attributeIdentifierType.Equals(AttributeIdentifierType.ClassName))
                        idOrClassName = Utils.GetBetween(outerHtml, "class=\"", "\"");
                    else if (attributeIdentifierType.Equals(AttributeIdentifierType.Xpath))
                        idOrClassName = node.XPath;
                    else if (attributeIdentifierType.Equals(AttributeIdentifierType.componentkey))
                        idOrClassName = Utils.GetBetween(pageSource, "componentkey=\"", "\"");
                    else if (attributeIdentifierType.Equals(AttributeIdentifierType.AriaLabel))
                        idOrClassName = Utils.GetBetween(pageSource, "aria-label=\"", "\"");
                    if (string.IsNullOrEmpty(idOrClassName))
                        continue;
                    idOrClassNameList.Add(idOrClassName);
                    if (isFirstOnly)
                        break;
                }
            }
            catch (Exception exception)
            {
                exception.DebugLog();
            }

            return idOrClassNameList;
        }

        public bool LoadAndClick(string attributeType, AttributeIdentifierType attributeIdentifierType,
            params string[] containsList)
        {
            try
            {
                var idOrClass = GetPath(_browserWindow.GetPageSource(), attributeType, attributeIdentifierType,
                    containsList);

                if (string.IsNullOrEmpty(idOrClass))
                    return false;

                var script = ScriptContructor(attributeIdentifierType, idOrClass);

                if (ExecuteScript(script).Success)
                {
                    var pageresponse = _browserWindow.GetPageSource();
                    if (pageresponse.Contains("artdeco-modal artdeco-modal--layer-default msg-modal-discard-message"))
                        ExecuteScript(AttributeIdentifierType.Xpath, "//span[text()='Discard']");

                    Thread.Sleep(TimeSpan.FromSeconds(2));
                    return true;
                }
            }
            catch (Exception exception)
            {
                exception.DebugLog();
            }

            return false;
        }


        public string ScriptContructor(AttributeIdentifierType attributeIdentifierType, string idOrClass, int index = 0,
            EventType eventType = EventType.click)
        {
            var script = "";
            if (attributeIdentifierType.Equals(AttributeIdentifierType.Id))
                script = $"document.getElementById('{idOrClass}').{eventType}();";
            else if (attributeIdentifierType.Equals(AttributeIdentifierType.ClassName))
                script = $"document.getElementsByClassName('{idOrClass}')[{index}].{eventType}();";
            else if (attributeIdentifierType.Equals(AttributeIdentifierType.Xpath))
                script =
                    $"document.evaluate('{idOrClass.Replace("\'", "\\'")}', document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue.{eventType}();";
            return script;
        }

        public bool LoadAndMouseClick(string attributeType, AttributeIdentifierType attributeIdentifierType,
            int addPixel = 0, params string[] containsList)
        {
            try
            {
                var idOrClass = GetPath(_browserWindow.GetPageSource(), attributeType, attributeIdentifierType,
                    containsList);
                if (string.IsNullOrEmpty(idOrClass))
                    return false;
                return ExecuteXAndYClick(idOrClass, attributeIdentifierType, addPixel);
            }
            catch (Exception exception)
            {
                exception.DebugLog();
            }

            return false;
        }

        public bool ExecuteXAndYClick(string elementName, AttributeIdentifierType attributeIdentifierType,
            int addPixels = 0)
        {
            var script = attributeIdentifierType == AttributeIdentifierType.Id
                ? $"document.getElementById('{elementName}').scrollIntoView()"
                : $"document.getElementsByClassName('{elementName}')[0].scrollIntoView()";

            ExecuteScript(script);
            _browserWindow.ExecuteScript("window.scrollBy(0,-200)");
            Thread.Sleep(TimeSpan.FromSeconds(2));
            var axis = GetXAndY(elementName, attributeIdentifierType);
            _browserWindow.MouseClick(axis.Key + addPixels, axis.Value + addPixels, delayAfter: 2);
            return axis.Key != 0 || axis.Value != 0;
        }

        public KeyValuePair<int, int> GetXAndYPositionByScript(string attributeType,
            AttributeIdentifierType attributeIdentifierType, params string[] containsList)
        {
            var xAndYPosition = new KeyValuePair<int, int>();

            try
            {
                var idOrClass = GetPath(_browserWindow.GetPageSource(), attributeType, attributeIdentifierType,
                    containsList);
                if (string.IsNullOrEmpty(idOrClass))
                    return xAndYPosition;
                return GetXAndY(idOrClass);
            }
            catch (Exception exception)
            {
                exception.DebugLog();
            }

            return xAndYPosition;
        }
        public KeyValuePair<int, int> GetXAndY(string elementName,
            AttributeIdentifierType attributeIdentifierType = AttributeIdentifierType.Id,int Index = 0)
        {
            var xAndY = new KeyValuePair<int, int>();

            var scripty = attributeIdentifierType == AttributeIdentifierType.Id
                ? $"$('#{elementName}').offset().top":
                attributeIdentifierType==AttributeIdentifierType.AriaLabel?
                $"document.querySelector('[aria-label=\"{elementName}\"]').getBoundingClientRect().top"
                : $"document.getElementsByClassName('{elementName}')[{Index}].getBoundingClientRect().top";
            var scriptx = attributeIdentifierType == AttributeIdentifierType.Id
                ? $"$('#{elementName}').offset().left":
                attributeIdentifierType==AttributeIdentifierType.AriaLabel?
                $"document.querySelector('[aria-label=\"{elementName}\"]').getBoundingClientRect().left"
                : $"document.getElementsByClassName('{elementName}')[{Index}].getBoundingClientRect().left";

            if (!ExecuteScript(scriptx, 0).Success)
            {
                scripty = (attributeIdentifierType == AttributeIdentifierType.Id
                              ? $"document.getElementById('{elementName}')"
                              : $"document.getElementsByClassName('{elementName}')[{Index}]") +
                          ".getBoundingClientRect().top"; //.getBoundingClientRect().top
                scriptx = (attributeIdentifierType == AttributeIdentifierType.Id
                              ? $"document.getElementById('{elementName}')"
                              : $"document.getElementsByClassName('{elementName}')[{Index}]") +
                          ".getBoundingClientRect().left"; //.getBoundingClientRect().left
            }

            if (ExecuteScript(scriptx, 0).Success)
            {
                var scriptResponse = ExecuteScript(scriptx, 0);
                var x = Utils.ConvertDoubleAndInt(scriptResponse.Result.ToString());
                scriptResponse = ExecuteScript(scripty, 0);
                var y = Utils.ConvertDoubleAndInt(scriptResponse.Result.ToString());
                xAndY = new KeyValuePair<int, int>(x, y);
                return xAndY;
            }

            return xAndY;
        }
        public JavascriptResponse ExecuteScript(string script, int delayInSec = 2)
        {
            var resp = _browserWindow.Browser.EvaluateScriptAsync(script).Result;


            if (resp.Success)
                TaskDelay(delayInSec);

            return resp;
        }

        public JavascriptResponse ExecuteScript(AttributeIdentifierType attributeIdentifierType, string idOrClass,
            int delayInSec = 2, int index = 0, EventType eventType = EventType.click)
        {
            var script = ScriptContructor(attributeIdentifierType, idOrClass,index);
            var resp = _browserWindow.Browser.EvaluateScriptAsync(script).Result;
            if (resp.Success)
                TaskDelay(delayInSec);
            return resp;
        }

        public string GetCurrentAddress()
        {
            var address = "";
            try
            {
                Application.Current.Dispatcher.Invoke(
                    () => { address = _browserWindow.Browser.Address; });
            }
            catch (Exception)
            {
            }

            return address;
        }

        public void LoadAndScroll(string pageUrl, int loadDelay, bool isScroll = false, int scrollDown = 3000,
            bool isDown = true, string breakIfContains = null,bool IsExpandSeeAll=false)
        {
            LoadPageUrlAndWait(pageUrl, loadDelay);
            if (isScroll)
                ScrollWindow(scrollDown, isDown, breakIfContains);
            if(IsExpandSeeAll)
            {
                var Nodes = HtmlAgilityHelper.GetListNodesFromAttibute(_browserWindow.GetPageSource(),HTMLTags.Button, AttributeIdentifierType.ClassName, null, "…see more");
                foreach (var Node in Nodes)
                {
                    var Index = Nodes.IndexOf(Node);
                    var className= Utils.GetBetween(Node.OuterHtml, "class=\"", "\"");
                    ExecuteScript(string.Format(LDClassesConstant.ScriptConstant.ScriptGetElementsByClassNameToClick, className,Index), 4);
                }
            }
        }

        public void ScrollWindow(int scrollDown = 2000, bool isDown = true, string breakIfContains = null,
            bool isDocumentEnd = false)
        {
            var down = isDown ? 500 : -500;
            var downTimes = 500 < scrollDown ? scrollDown / 500 : 2;
            for (var i = 1; i < downTimes; i++)
            {
                if (!string.IsNullOrEmpty(breakIfContains) && _browserWindow.GetPageSource().Contains(breakIfContains))
                    break;
                Thread.Sleep(1000);
                CancellationToken.ThrowIfCancellationRequested();
                 var screenResolution = GetScreenResolution();                
                _browserWindow.MouseScroll(screenResolution.Key / 2, screenResolution.Value / 2, 0, -300, delayAfter: 4, scrollCount: 1);
                
            }
        }
        public static KeyValuePair<int, int> GetScreenResolution()
        {
            int height = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;
            int width = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
            return new KeyValuePair<int, int>(width, height);
        }

        public void ScrollPostWindow(int scrollDown = 2000, bool isDown = true, string breakIfContains = null,
            bool isDocumentEnd = false)
        {
            var down = isDown ? 100 : -100;
            var downTimes = 500 < scrollDown ? scrollDown / 500 : 2;
            for (var i = 1; i < downTimes; i++)
            {
                if (!string.IsNullOrEmpty(breakIfContains) && _browserWindow.GetPageSource().Contains(breakIfContains))
                    break;
                Thread.Sleep(100);
                CancellationToken.ThrowIfCancellationRequested();
                ExecuteScript(isDocumentEnd
                    ? "window.scrollTo(0,document.documentElement.scrollHeight)"
                    : $"window.scrollBy(0, {down})");
            }
        }


        public string ScrollAndSaveCurrentPage()
        {
            // click to go to second page if pageurl not have pageNum
            var CurrentPageUrl = GetCurrentAddress();
            var pageNumber = Utils.GetBetween(CurrentPageUrl, "page=", "&");
            if (string.IsNullOrEmpty(pageNumber) &&
                string.IsNullOrEmpty(pageNumber = Utils.GetBetween(CurrentPageUrl + "<>", "page=", "<>")))
            {
                var isSuccess = LoadAndClick("button", AttributeIdentifierType.Xpath, "Navigate to page 2");
                if (!isSuccess)
                    LoadAndClick("button", AttributeIdentifierType.Xpath,
                        "artdeco-pagination__button artdeco-pagination__button--next artdeco-button artdeco-button--muted artdeco-button--icon-right artdeco-button--1 artdeco-button--tertiary ember-view");
            }
            else
            {
                var pageNum = 1;
                int.TryParse(pageNumber, out pageNum);
                var nextPage = CurrentPageUrl.Replace($"page={pageNum}", $"page={pageNum + 1}");
                _browserWindow.Browser.Load(nextPage);
            }

            TaskDelay(10);
            ScrollWindow(4000);

            return GetCurrentAddress();
        }

        public void LoadPageUrlAndWait(string url, int delayInSec = 10)
        {
            _browserWindow.Browser.Load(url);
            var pageResponse = "";
            var count = 0;
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            var isMessageDisplayed = false;
            while (count <= 3 && !pageResponse.Contains("artdeco-modal-outlet") && !pageResponse.Contains("artdeco-toasts__wormhole"))
            {
                TaskDelay(delayInSec);
                pageResponse = _browserWindow.GetPageSourceAsync().Result;
                if (stopWatch.Elapsed.TotalSeconds > 60 && !isMessageDisplayed && (isMessageDisplayed = true))
                {
                    //GlobusLogHelper.log.Info(Log.CustomMessage,
                    //    _browserWindow.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    //    _browserWindow.DominatorAccountModel.AccountBaseModel.UserName, "Browser",
                    //    $"Loading {url} taking more time than expected.");
                    break;
                }
                else
                    count++;
            }
        }

        public BrowserWindow ViewProfileBrowserInitializing(DominatorAccountModel _linkedInModel, string loadUrl,
            out string CurrentPageUrl)
        {
            BrowserWindow browserWindow;
            LDAccountsBrowserDetails.GetInstance().AccountBrowserCollections
                .TryGetValue(_linkedInModel.UserName, out browserWindow);
            _browserWindow = browserWindow;
            LoadPageUrlAndWait(loadUrl);
            ScrollWindow(5000);
            ExecuteScript("window.scrollTo(0, 100)");
            CurrentPageUrl = GetCurrentAddress();
            return browserWindow;
        }

        public void TaskDelay(int delayInSec)
        {
            try
            {
                Task.Delay(TimeSpan.FromSeconds(delayInSec), CancellationToken).Wait();
            }

            catch (Exception)
            {
                CancellationToken.ThrowIfCancellationRequested();
            }
        }
    }


    public enum AttributeIdentifierType
    {
        Id,
        ClassName,
        Xpath,
        AriaLabel,
        Href,
        componentkey,
        DataViewName
    }

    public enum EventType
    {
        click
    }

    public class AutomationModel
    {
        public string AttributeName { get; set; }
        public List<string> PresentList { get; set; } = new List<string>();
        public HtmlNode HtmlNode { get; set; }
        public string PageSource { get; set; }
        public BrowserWindow BrowserWindow { get; set; }
        public int DelayInSec { get; set; }
    }
}