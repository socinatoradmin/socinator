using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CefSharp;
using CommonServiceLocator;
using ThreadUtils;
using DominatorHouseCore;
using DominatorHouseCore.Utility;
using EmbeddedBrowser;
using HtmlAgilityPack;

namespace TwtDominatorCore.TDUtility
{
    public class BrowserAutomationExtension
    {
        public enum EventType
        {
            click
        }

        public BrowserWindow _browserWindow;
        public CancellationToken jobCancellationToken;
        private readonly IDelayService _delayService;

        public BrowserAutomationExtension(BrowserWindow browserWindow, CancellationToken token)
        {
            _browserWindow = browserWindow;
            jobCancellationToken = token;
            _delayService = InstanceProvider.GetInstance<IDelayService>();
        }

        public string GetPath(string pageSource, string attributeType, AttributeIdentifierType attributeIdentifierType,
            params string[] containsList)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(pageSource);
            var nodeCollection = htmlDoc.DocumentNode.SelectNodes($"//{attributeType}");
            var idOrClassName = "";
            try
            {
                foreach (var node in nodeCollection)
                {
                    var outerHtml = node.OuterHtml;

                    if (!TdUtility.IsContains(outerHtml, containsList)
                        || attributeIdentifierType.Equals(AttributeIdentifierType.Id) && string.IsNullOrEmpty(node.Id))
                        continue;

                    if (attributeIdentifierType.Equals(AttributeIdentifierType.Id))
                        idOrClassName = node.Id;
                    else if (attributeIdentifierType.Equals(AttributeIdentifierType.ClassName))
                        idOrClassName = Utilities.GetBetween(outerHtml, "class=\"", "\"");
                    else if (attributeIdentifierType.Equals(AttributeIdentifierType.Xpath))
                        idOrClassName = node.XPath;
                    if (!string.IsNullOrEmpty(idOrClassName))
                        break;
                }
            }
            catch (Exception exception)
            {
                exception.DebugLog();
            }

            return idOrClassName;
        }

        public bool LoadAndClick(string attributeType, AttributeIdentifierType attributeIdentifierType,
            params string[] containsList)
        {
            var script = "";
            try
            {
                var idOrClass = GetPath(_browserWindow.GetPageSource(), attributeType, attributeIdentifierType,
                    containsList);
                if (string.IsNullOrEmpty(idOrClass))
                    return false;

                if (attributeIdentifierType.Equals(AttributeIdentifierType.Id))
                    script = $"document.getElementById('{idOrClass}').click();";
                else if (attributeIdentifierType.Equals(AttributeIdentifierType.ClassName))
                    script = $"document.getElementsByClassName('{idOrClass}')[0].click();";
                else if (attributeIdentifierType.Equals(AttributeIdentifierType.Xpath))
                    script =
                        $"document.evaluate('{idOrClass.Replace("\'", @"\\'")}', document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue.click();";

                return ExecuteScript(script, 2).Success;
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception exception)
            {
                exception.DebugLog();
            }

            return false;
        }

        public bool LoadAndMouseClick(string attributeType, AttributeIdentifierType attributeIdentifierType,
            params string[] containsList)
        {
            try
            {
                var idOrClass = GetPath(_browserWindow.GetPageSource(), attributeType, attributeIdentifierType,
                    containsList);
                if (string.IsNullOrEmpty(idOrClass))
                    return false;
                return ExecuteXAndYClick(idOrClass, attributeIdentifierType);
            }
            catch (Exception exception)
            {
                exception.DebugLog();
            }


            return false;
        }

        public bool ExecuteXAndYClick(string elementName, AttributeIdentifierType attributeIdentifierType)
        {
            var script = attributeIdentifierType == AttributeIdentifierType.Id
                ? $"document.getElementById('{elementName}').scrollIntoView()"
                : $"document.getElementsByClassName('{elementName}')[0].scrollIntoView()";

            ExecuteScript(script, 2);

            var axis = GetXAndY(elementName, attributeIdentifierType);
            _browserWindow.MouseClick(axis.Key, axis.Value, delayAfter: 2);
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
            AttributeIdentifierType attributeIdentifierType = AttributeIdentifierType.Id, int index = 0)
        {
            var xAndY = new KeyValuePair<int, int>();
            var scriptx = "";
            var scripty = "";

            switch (attributeIdentifierType)
            {
                case AttributeIdentifierType.Id:
                    scriptx = $"$('#{elementName}').offset().top";
                    scripty = $"$('#{elementName}').offset().left";
                    break;
                case AttributeIdentifierType.Name:
                    scriptx = $"document.getElementsByName('{elementName}')[{index}].getBoundingClientRect().left";
                    scripty = $"document.getElementsByName('{elementName}')[{index}].getBoundingClientRect().top";
                    break;
                case AttributeIdentifierType.ClassName:
                    scriptx = $"document.getElementsByClassName('{elementName}')[{index}].getBoundingClientRect().left";
                    scripty = $"document.getElementsByClassName('{elementName}')[{index}].getBoundingClientRect().top";
                    break;
                case AttributeIdentifierType.Xpath:
                    scriptx =
                        $"document.evaluate('{elementName.Replace(@"'", @"\'")}', document, null,XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue.getBoundingClientRect().left";
                    scripty =
                        $"document.evaluate('{elementName.Replace(@"'", @"\'")}', document, null,XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue.getBoundingClientRect().top";
                    break;
            }

            if (ExecuteScript(scriptx, 0).Success)
            {
                var scriptResponse = ExecuteScript(scriptx, 0);
                var x = TdUtility.ConvertDoubleAndInt(scriptResponse.Result.ToString());
                scriptResponse = ExecuteScript(scripty, 0);
                var y = TdUtility.ConvertDoubleAndInt(scriptResponse.Result.ToString());
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
            var script = ScriptContructor(attributeIdentifierType, idOrClass);
            var resp = _browserWindow.Browser.EvaluateScriptAsync(script).Result;
            if (resp.Success)
                TaskDelay(delayInSec);
            return resp;
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

        public void LoadAndScroll(string pageUrl, int loadDelay, bool isScroll = false, int scrollDown = 3000,
            bool isDown = true, string breakIfContains = null)
        {
            LoadPageUrlAndWait(pageUrl, loadDelay);
            var pageresponse = _browserWindow.GetPageSource();
            if (pageresponse.Contains("If you’re not redirected soon, please"))
            {
                LoadAndClick("a", AttributeIdentifierType.Xpath, "use this link");
                _delayService.ThreadSleep(10000);
            }

            if (isScroll)
                ScrollWindow(scrollDown, isDown, breakIfContains);
        }

        public JavascriptResponse ExecuteScript(string script, int sleepSeconds = 2, BrowserWindow browserWindow = null)
        {
            var scriptResponse = browserWindow.Browser.EvaluateScriptAsync(script).Result;
            TaskDelay(sleepSeconds);
            return scriptResponse;
        }

        public void ScrollWindow(int scrollDown = 2000, bool isDown = true, string breakIfContains = null)
        {
            var down = isDown ? 500 : -500;
            var downTimes = scrollDown / 500;
            for (var i = 1; i < downTimes; i++)
            {
                if (!string.IsNullOrEmpty(breakIfContains) && _browserWindow.GetPageSource().Contains(breakIfContains))
                    break;
                _delayService.ThreadSleep(300);
                jobCancellationToken.ThrowIfCancellationRequested();
                ExecuteScript($"window.scrollBy(0, {down})", 0);
            }
        }

        public string ScrollAndSaveCurrentPage()
        {
            // click to go to second page if pageurl not have pageNum
            var currentPageUrl = _browserWindow.CurrentUrl();
            var pageNumber = Utilities.GetBetween(currentPageUrl, "page=", "&");
            if (string.IsNullOrEmpty(pageNumber) &&
                string.IsNullOrEmpty(pageNumber = Utilities.GetBetween(currentPageUrl + "<>", "page=", "<>")))
            {
                LoadAndClick("button", AttributeIdentifierType.Xpath, "Navigate to page 2");
            }
            else
            {
                int.TryParse(pageNumber, out var pageNum);
                var nextPage = currentPageUrl.Replace($"page={pageNum}", $"page={pageNum + 1}");
                _browserWindow.Browser.Load(nextPage);
            }

            TaskDelay(1);
            ScrollWindow(4000);
            ExecuteScript("window.scrollTo(0, 100)", 2);
            return _browserWindow.CurrentUrl();
        }

        public void LoadPageUrlAndWait(string url, int delayInSec = 10)
        {
            _browserWindow.Browser.Load(url);
            TaskDelay(delayInSec);
        }


        public void TaskDelay(int delayInSec)
        {
            try
            {
                _delayService.DelayAsync(TimeSpan.FromSeconds(delayInSec), jobCancellationToken).Wait();
            }
            catch (Exception)
            {
                jobCancellationToken.ThrowIfCancellationRequested();
            }
        }
    }


    public enum AttributeIdentifierType
    {
        Id,
        Name,
        ClassName,
        Xpath
    }

    [Obsolete("May be use later")]
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