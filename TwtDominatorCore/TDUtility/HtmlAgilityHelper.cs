using System;
using System.Collections.Generic;
using DominatorHouseCore.Utility;
using HtmlAgilityPack;

namespace TwtDominatorCore.TDUtility
{
    public class HtmlAgilityHelper
    {
        public static string MethodGetStringFromId(string response, string id)
        {
            var idPart = string.Empty;
            try
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(response);
                idPart = htmlDoc.GetElementbyId(id)?.OuterHtml ?? string.Empty;
            }
            catch (Exception)
            {
            }

            return idPart;
        }

        public static string MethodGetInnerStringFromId(string Response, string Id)
        {
            var IdPart = string.Empty;
            try
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(Response);
                IdPart = htmlDoc.GetElementbyId(Id).InnerHtml;
            }
            catch (Exception)
            {
            }

            return IdPart;
        }

        public static string getStringInnerTextFromClassName(string Response, string ClassName,
            HtmlDocument htmlDoc = null)
        {
            var Result = string.Empty;

            try
            {
                if (htmlDoc?.Text == null)
                {
                    htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(Response);
                }

                var xpath = string.Format("//*[contains(@class,'{0}')]", ClassName);
                var node = htmlDoc.DocumentNode.SelectSingleNode(xpath);
                if(node != null)
                    Result = node.InnerText.Trim();
            }
            catch
            {
            }

            return Result;
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
                var nodes = htmlDoc.DocumentNode.SelectNodes(xpath);
                foreach (var singleNode in nodes) Result.Add(singleNode);
            }
            catch
            {
            }

            return Result;
        }

        public static string getStringInnerHtmlFromClassName(string Response, string ClassName,
            HtmlDocument htmlDoc = null)
        {
            var Result = string.Empty;

            try
            {
                if (htmlDoc?.Text == null)
                {
                    htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(Response);
                }

                var xpath = string.Format("//*[contains(@class,'{0}')]", ClassName);
                var node = htmlDoc.DocumentNode.SelectSingleNode(xpath);
                Result = node?.InnerHtml?.Trim();
            }
            catch
            {
            }

            return Result;
        }

        public static List<string> getListInnerHtmlFromClassName(string Response, string ClassName,
            HtmlDocument htmlDoc = null)
        {
            var Result = new List<string>();

            try
            {
                if (htmlDoc?.Text == null)
                {
                    htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(Response);
                }

                var xpath = string.Format("//*[contains(@class,'{0}')]", ClassName);
                var nodes = htmlDoc.DocumentNode.SelectNodes(xpath);
                if(nodes != null)
                    foreach (var singleNode in nodes) 
                        Result.Add(singleNode.InnerHtml);
            }
            catch
            {
            }

            return Result;
        }

        public static List<HtmlNode> GetNodesFromClassName(string Response, string ClassName,
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
                var nodes = htmlDoc.DocumentNode.SelectNodes(xpath);
                foreach (var singleNode in nodes) Result.Add(singleNode);
            }
            catch
            {
            }

            return Result;
        }


        public static string getValueWithAttributeNameFromInnerHtml(string Response, string ClassName,
            string AttributeName, HtmlDocument htmlDoc = null)
        {
            string result;

            try
            {
                if (htmlDoc?.Text == null)
                {
                    htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(Response);
                }

                var xpath = string.Format("//*[contains(@class,'{0}')]", ClassName);
                var node = htmlDoc.DocumentNode.SelectSingleNode(xpath);
                var TempResult = node.InnerHtml.Trim();
                var startString = AttributeName + "=\"";
                var endString = "\"";
                result = Utilities.GetBetween(TempResult, startString, endString);
                result = result.Trim();
            }
            catch
            {
                return null;
            }

            return result;
        }

        public static string getXpathWithAttributeNameFromInnerHtml(string Response, string ClassName,
            string AttributeName, HtmlDocument htmlDoc = null)
        {
            string result;

            try
            {
                if (htmlDoc?.Text == null)
                {
                    htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(Response);
                }

                var xpath = string.Format("//*[contains(@class,'{0}')]", ClassName);
                var node = htmlDoc.DocumentNode.SelectSingleNode(xpath);
                result = node.XPath;
            }
            catch
            {
                return null;
            }

            return result;
        }


        public static List<string> getListValueWithAttributeNameFromInnerHtml(string Response, string ClassName,
            string AttributeName, HtmlDocument htmlDoc = null)
        {
            var Result = new List<string>();
            try
            {
                if (htmlDoc?.Text == null)
                {
                    htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(Response);
                }

                var startString = AttributeName + "=\"";
                var endString = "\"";
                var xpath = string.Format("//*[contains(@class,'{0}')]", ClassName);
                var nodes = htmlDoc.DocumentNode.SelectNodes(xpath);
                if (nodes != null)
                    foreach (var singleNode in nodes)
                        Result.Add(Utilities.GetBetween(singleNode.InnerHtml, startString, endString));
            }
            catch
            {
                return new List<string>();
            }

            return Result;
        }

        public static List<string> getListValueWithAttributeNameFromClassName(string Response, string ClassName,
            string AttributeName)
        {
            var Result = new List<string>();
            try
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(Response);
                var xpath = string.Format("//*[contains(@class,'{0}')]", ClassName);
                var nodes = htmlDoc.DocumentNode.SelectNodes(xpath);
                if (nodes != null)
                    foreach (var singleNode in nodes)
                        Result.Add(singleNode.Attributes[AttributeName].Value);
            }
            catch
            {
                return new List<string>();
            }

            return Result;
        }

        public static string getStringTextFromClassName(string Response, string ClassName)
        {
            var Result = string.Empty;

            try
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(Response);
                var xpath = string.Format("//*[contains(@class,'{0}')]", ClassName);
                var node = htmlDoc.DocumentNode.SelectSingleNode(xpath);
                Result = node.InnerHtml.Trim();
            }
            catch
            {
            }

            return Result;
        }

        public static string GetStringTextFromClassName(HtmlNode htmlNode, string ClassName)
        {
            var Result = string.Empty;

            try
            {
                var xpath = string.Format("//*[contains(@class,'{0}')]", ClassName);
                var node = htmlNode.SelectSingleNode(xpath);
                Result = node.InnerHtml.Trim();
            }
            catch
            {
            }

            return Result;
        }
    }
}