using DominatorHouseCore;
using DominatorHouseCore.Utility;
using HtmlAgilityPack;
using System;

namespace QuoraDominatorCore.QdUtility
{
    public class BrowserUtilities
    {
        public static string GetPath(string pageSource, string tagName, string containText)
        {
            var className = string.Empty;
            try
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(pageSource);
                var nodeCollection = htmlDoc.DocumentNode.SelectNodes($"//{tagName}");

                foreach (var node in nodeCollection)
                {
                    var outerHtml = node.OuterHtml;

                    if (!outerHtml.Contains(containText))
                        continue;

                    className = Utilities.GetBetween(outerHtml, "class=\"", "\"");
                    if (!string.IsNullOrEmpty(className))
                        break;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return className;
        }
    }
}
