using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace RedditDominatorCore.RDUtility
{
    public class HtmlUtility
    {
        public static List<string> GetListInnerTextFromClassName(string Response, string ClassName)
        {

            List<string> mainList = new List<string>();
            try
            {
                HtmlDocument htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(Response);
                string xpath = string.Format("//*[contains(@class,'{0}')]", ClassName);
                var nodes = htmlDoc.DocumentNode.SelectNodes(xpath);
                if(nodes != null)
                foreach (var node in nodes)
                {
                    mainList.Add(node.InnerText);
                }
            }
            catch (Exception)
            {
                return mainList;
            }
            return mainList;
        }
        public static HtmlNodeCollection GetListOfNodesFromTagName(string pageSource, string tagName, string attributeName,
            string attributeValue)
        {
            var htmlDoc = new HtmlDocument();
            try
            {
                htmlDoc.LoadHtml(pageSource);
                return htmlDoc.DocumentNode.SelectNodes($"//{tagName}[@{attributeName}='{attributeValue}']");
            }
            catch (Exception)
            {
            }
            return null;
        }
        public static HtmlNodeCollection GetListOfNodesFromTagNameWithoutAttribute(string pageSource, string tagName)
        {
            var htmlDoc = new HtmlDocument();
            try
            {
                htmlDoc.LoadHtml(pageSource);
                return htmlDoc.DocumentNode.SelectNodes($"//{tagName}");
            }
            catch (Exception)
            {
            }
            return null;
        }
    }
}
