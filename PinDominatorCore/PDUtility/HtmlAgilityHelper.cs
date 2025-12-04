using DominatorHouseCore;
using DominatorHouseCore.Utility;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;

namespace PinDominatorCore.PDUtility
{
    public class HtmlAgilityHelper
    {
        public static string GetStringInnerTextFromClassName(string response, string className)
        {
            var result = string.Empty;

            try
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(response);
                var xpath = $"//*[contains(@class,'{className}')]";
                var node = htmlDoc.DocumentNode.SelectSingleNode(xpath);
                result = node.InnerText.Trim();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return result;
        }
        public static List<string> GetListInnerHtmlOrInnerTextOrOuterHtmlFromIdOrClass(string response, string Id,bool getInnerText = false,string className="",bool getOuterHtml=false)
        {
            var result = new List<string>();
            try
            {
                HtmlDocument htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(response);
                string xpath = string.IsNullOrEmpty(className) ? string.Format("//*[contains(@id,'{0}')]", Id) : string.Format("//*[contains(@class,'{0}')]", className);
                var nodes = htmlDoc.DocumentNode.SelectNodes(xpath);
                foreach (var node in nodes)
                    result.Add(getInnerText?node.InnerText?.Trim():getOuterHtml?node.OuterHtml:node.InnerHtml);
                return result;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return result;
            }
        }
        public static List<string> GetListInnerHtmlFromClassName(string Response, string ClassName)
        {

            List<string> mainList = new List<string>();
            try
            {
                HtmlDocument htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(Response);

                string xpath = string.Format("//*[contains(@class,'{0}')]", ClassName);
                var nodes = htmlDoc.DocumentNode.SelectNodes(xpath);
                foreach (var node in nodes)
                {
                    mainList.Add(node.InnerHtml);
                }
            }
            catch (Exception)
            {
            }
            return mainList;
        }
    }
}