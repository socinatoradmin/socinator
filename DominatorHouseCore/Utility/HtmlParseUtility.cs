#region

using System;
using System.Collections.Generic;
using HtmlAgilityPack;

#endregion

namespace DominatorHouseCore.Utility
{
    public static class HtmlParseUtility
    {
        public static readonly string NotFound = "Not Found";

        public static string GetInnerHtmlFromTagName(string pageSource, string tagName, string attributeName,
            string attributeValue)
        {
            var htmlDoc = new HtmlDocument();
            try
            {
                htmlDoc.LoadHtml(pageSource);
                return htmlDoc.DocumentNode?.SelectSingleNode($"//{tagName}[@{attributeName}='{attributeValue}']")?.InnerHtml ?? "";
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return "";
            }
        }

        public static List<string> GetListInnerHtmlFromTagName(string pageSource, string tagName, string attributeName,
            string attributeValue)
        {
            var lstInnerhtml = new List<string>();
            var htmlDoc = new HtmlDocument();
            try
            {
                htmlDoc.LoadHtml(pageSource);
                htmlDoc.DocumentNode?.SelectNodes($"//{tagName}[@{attributeName}='{attributeValue}']")
                    ?.ForEach(x => { lstInnerhtml.Add(x.InnerHtml.ToString()); });
                return lstInnerhtml;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return lstInnerhtml;
        }


        public static string GetAttributeValueFromId(string pageSource, string idValue, string attributeName)
        {
            var htmlDoc = new HtmlDocument();
            try
            {
                htmlDoc.LoadHtml(pageSource);
                return htmlDoc?.GetElementbyId(idValue)?.GetAttributeValue(attributeName, NotFound) ?? "";
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return string.Empty;
            }
        }

        public static string GetAttributeValueFromTagName(string pageSource, string tagName, string attributeName,
            string attributeValue, string searchByAttribute)
        {
            var htmlDoc = new HtmlDocument();
            try
            {
                htmlDoc.LoadHtml(pageSource);
                return htmlDoc.DocumentNode?.SelectSingleNode($"//{tagName}[@{attributeName}='{attributeValue}']")
                    ?.GetAttributeValue(searchByAttribute, NotFound);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return string.Empty;
            }
        }

        public static string GetOuterHtmlFromTagName(string pageSource, string tagName, string attributeName,
            string attributeValue)
        {
            var htmlDoc = new HtmlDocument();
            try
            {
                htmlDoc.LoadHtml(pageSource);
                return htmlDoc.DocumentNode?.SelectSingleNode($"//{tagName}[@{attributeName}='{attributeValue}']")?.OuterHtml ?? "";
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return string.Empty;
            }
        }

        public static List<string> GetListInnerHtmlFromPartialTagName(string pageSource, string tagName,
            string attributeName,
            string attributeValue)
        {
            var lstInnerhtml = new List<string>();
            var htmlDoc = new HtmlDocument();
            try
            {
                htmlDoc.LoadHtml(pageSource);
                htmlDoc.DocumentNode
                    ?.SelectNodes($"//{tagName}[starts-with(@{attributeName}, '{attributeValue}')]")
                    ?.ForEach(x => { lstInnerhtml.Add(x.InnerHtml.ToString()); });
                return lstInnerhtml;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return lstInnerhtml;
        }

        public static string GetAllInnerTextFromTags(string pageSource)
        {
            var text = string.Empty;
            var htmlDoc = new HtmlDocument();
            try
            {
                htmlDoc.LoadHtml(pageSource);
                text = htmlDoc.DocumentNode?.InnerText;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return text;
        }

        /// <summary>
        ///     Get inner text from single node by atteribute
        /// </summary>
        /// <param name="pageSource"></param>
        /// <param name="tagName"></param>
        /// <param name="attributeName"></param>
        /// <param name="attributeValue"></param>
        /// <returns></returns>
        public static string GetInnerTextFromSingleNode(string pageSource, string tagName, string attributeName,
            string attributeValue)
        {
            var text = string.Empty;
            try
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(pageSource);
                text = htmlDoc.DocumentNode?.SelectSingleNode($"//{tagName}[@{attributeName}='{attributeValue}']")
                    ?.InnerText;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return text;
        }

        /// <summary>
        /// </summary>
        /// <param name="pageSource"></param>
        /// <param name="tagName"></param>
        /// <param name="attributeName"></param>
        /// <param name="attributeValue"></param>
        /// <returns></returns>
        public static List<string> GetListInnerHtmlFromPartialTagNamecontains(string pageSource, string tagName,
            string attributeName,
            string attributeValue)
        {
            var lstInnerhtml = new List<string>();
            var htmlDoc = new HtmlDocument();
            try
            {
                htmlDoc.LoadHtml(pageSource);
                htmlDoc.DocumentNode
                    ?.SelectNodes($"//{tagName}[contains(@{attributeName}, '{attributeValue}')]")
                    ?.ForEach(x => { lstInnerhtml.Add(x.InnerHtml.ToString()); });
                return lstInnerhtml;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return lstInnerhtml;
        }

        public static List<string> GetListInnerTextFromPartialTagNamecontains(string pageSource, string tagName,
            string attributeName,
            string attributeValue)
        {
            var lstInnerhtml = new List<string>();
            var htmlDoc = new HtmlDocument();
            try
            {
                htmlDoc.LoadHtml(pageSource);
                htmlDoc.DocumentNode
                    ?.SelectNodes($"//{tagName}[contains(@{attributeName}, '{attributeValue}')]")
                    ?.ForEach(x => { lstInnerhtml.Add(x.InnerText.ToString()); });
                return lstInnerhtml;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return lstInnerhtml;
        }
        public static List<HtmlNode> GetListNodeFromPartialTagNamecontains(string pageSource, string tagName,
            string attributeName,
            string attributeValue)
        {
            var lstInnerhtml = new List<HtmlNode>();
            var htmlDoc = new HtmlDocument();
            try
            {
                htmlDoc.LoadHtml(pageSource);
                htmlDoc.DocumentNode
                    ?.SelectNodes($"//{tagName}[contains(@{attributeName}, '{attributeValue}')]")
                    ?.ForEach(x => { lstInnerhtml.Add(x); });
                return lstInnerhtml;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return lstInnerhtml;
        }
        public static string GetInnerTextFromTagName(string pageSource, string tagName, string attributeName,
            string attributeValue)
        {
            try
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(pageSource);
                return htmlDoc.DocumentNode?.SelectSingleNode($"//{tagName}[@{attributeName}='{attributeValue}']")
                    ?.InnerText;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return string.Empty;
            }
        }
        public static List<HtmlNode> GetListNodesFromClassName(string Response, string ClassName,
            HtmlDocument htmlDoc = null)
        {
            var Result = new List<HtmlNode>();

            try
            {
                if (htmlDoc?.Text == null)
                {
                    htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(Response);
                }

                var xpath = string.Format("//*[contains(@class,'{0}')]", ClassName);
                var nodes = htmlDoc.DocumentNode?.SelectNodes(xpath);

                if (nodes != null)
                    foreach (var singleNode in nodes) Result.Add(singleNode);
            }
            catch
            {
            }

            return Result;
        }
        public static List<string> GetListInnerHtmlOrInnerTextOrOuterHtmlFromIdOrClassName(string Response, string Id, bool getInnerText = false, string className = "", bool getOuterHtml = false)
        {
            var result = new List<string>();
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(Response);
            var xpath = string.IsNullOrEmpty(className) ? string.Format("//*[contains(@id,'{0}')]", Id) : string.Format("//*[contains(@class,'{0}')]", className);
            var nodes = htmlDocument.DocumentNode?.SelectNodes(xpath);
            if (nodes != null)
                foreach (var node in nodes)
                    result.Add(getInnerText ? node.InnerText?.Trim() : getOuterHtml ? node.OuterHtml : node.InnerHtml);
            return result;
        }
    }
}