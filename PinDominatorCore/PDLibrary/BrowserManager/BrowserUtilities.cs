using DominatorHouseCore.Utility;
using HtmlAgilityPack;
using System;

namespace PinDominatorCore.PDLibrary.BrowserManager
{
    public static class BrowserUtilities
    {
        public static string GetPath(string pageSource, string tagName, string matchWithText)
        {
            var attributeName = string.Empty;
            try
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(pageSource);
                var nodeCollection = htmlDoc.DocumentNode.SelectNodes($"//{tagName}");

                foreach (var items in nodeCollection)
                {
                    var nodeValue = items.OuterHtml.ToString();
                    if (nodeValue.Contains(matchWithText))
                    {
                        attributeName = Utilities.GetBetween(nodeValue, "class=\"", "\"");
                        if (!string.IsNullOrEmpty(attributeName))
                            break;
                    }
                }
            }
            catch
            {
                // ignored
            }

            return attributeName;
        }

        public static string GetAttributeNameWithInnerText(string pageSource, string tagName, string innerText)
        {
            var attributeName = string.Empty;
            try
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(pageSource);
                var nodeCollection = htmlDoc.DocumentNode.SelectNodes($"//{tagName}");

                foreach (var items in nodeCollection)
                {
                    var obtainInnerText = items.InnerText;
                    if (!obtainInnerText.Equals(innerText))
                        continue;

                    if (obtainInnerText.Equals(innerText))
                    {
                        var nodeValue = items.OuterHtml.ToString();

                        var attributeClass = System.Text.RegularExpressions.Regex.Split(nodeValue, "class");
                        if (attributeClass.Length > 1)
                        {
                            foreach (var item in attributeClass)
                            {
                                if (item.Contains($">{innerText}"))
                                {
                                    attributeName = Utilities.GetBetween(item, "=\"", "\"");
                                    break;
                                }
                            }
                        }

                        else
                            attributeName = Utilities.GetBetween(nodeValue, "class=\"", "\"");

                        if (!string.IsNullOrEmpty(attributeName))
                            break;
                    }
                }
            }
            catch
            {
                // ignored
            }

            return attributeName;
        }
    }
}
