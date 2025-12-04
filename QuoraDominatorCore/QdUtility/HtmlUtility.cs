using System;
using System.Collections.Generic;
using System.Linq;
using DominatorHouseCore.Utility;
using HtmlAgilityPack;

namespace QuoraDominatorCore.QdUtility
{
    public static class HtmlUtility
    {
        public static string GetIdValue(string pageSource, string tagName, string attributeName, string attributeValue)
        {
            var id = string.Empty;

            try
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(pageSource);
                var nodeCollection =
                    htmlDoc.DocumentNode.SelectNodes($"//{tagName}[@{attributeName}='{attributeValue}']");

                foreach (var items in nodeCollection)
                {
                    var InnerHtml = items.InnerHtml;
                    if (InnerHtml.Contains("\">Submit</a>"))
                    {
                        id = Utilities.GetBetween(InnerHtml, "id=\"", "\">Submit</a>");
                        if (!string.IsNullOrEmpty(id))
                            break;
                    }
                }
            }
            catch { }
            return id;
        }
        public static List<string> GetListInnerTextFromClassName(string Response, string ClassName)
        {

            List<string> mainList = new List<string>();
            try
            {
                HtmlDocument htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(Response);
                string xpath = string.Format("//*[contains(@class,'{0}')]", ClassName);
                var nodes = htmlDoc.DocumentNode.SelectNodes(xpath);
                foreach (var node in nodes)
                    mainList.Add(node.InnerText);
            }
            catch (Exception)
            {
            }
            return mainList;
        }
        public static string GetInnerTextFromClassName(string Response, string ClassName)
        {
            var InnerText = string.Empty;
            try
            {
                var document = new HtmlDocument();
                document.LoadHtml(Response);
                InnerText = document.DocumentNode.SelectSingleNode($"//*[contains(@class,'{ClassName}')]").InnerText;
            }
            catch { }
            return InnerText;
        }
        public static List<HtmlNode> GetListNodesFromClassName(string Response,string className,string attributeValue="")
        {
            var Nodes = new List<HtmlNode>();
            var document = new HtmlDocument();
            try
            {
                document.LoadHtml(Response);
                var xpath = !string.IsNullOrEmpty(attributeValue) ?string.Format($"//{attributeValue}[@class='{className}']") : string.Format("//*[contains(@class,'{0}')]", className);
                var nodes = document.DocumentNode.SelectNodes(xpath);
                nodes.ForEach(node => Nodes.Add(node));
            }
            catch (Exception)
            {
                Nodes = document.DocumentNode.Descendants("a")
                      .Where(node => node.GetAttributeValue("class", "").Contains($"{className}")).ToList();
                      //.Select(node => node.GetAttributeValue("href", ""));
            }
            return Nodes;
        }
        public static HtmlNode GetSingleNode(string Response,string attributeType,string className)
        {
            HtmlNode node=null;
            try
            {
                var document = new HtmlDocument();
                document.LoadHtml(Response);
                node = document.DocumentNode.SelectSingleNode($"//{attributeType}[@class='{className}']");
            }
            catch { }
            return node;
        }
    }
}