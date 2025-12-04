using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using HtmlAgilityPack;
using System;
using System.Text.RegularExpressions;

namespace RedditDominatorCore.RDLibrary.BrowserManager.BrowserUtility
{
    public static class BrowserUtilities
    {
        public static string GetAttributeValueForActionForMediaAndLink(string pageSource, string tagName,
            string attributeName, string attributeValue, int index = 1)
        {
            var actionAttributeValue = string.Empty;
            try
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(pageSource);
                var nodes = htmlDoc.DocumentNode.SelectSingleNode($"//{tagName}[@{attributeName}='{attributeValue}']")
                    .InnerHtml;
                var lstAttribute = Regex.Split(nodes, "button class=");
                actionAttributeValue = Utilities.GetBetween(lstAttribute[index], "\"", "\"");
            }
            catch (Exception)
            {
                // ignored
            }

            return actionAttributeValue;
        }

        public static string GetAttributeValueForVoterFromInnerHtml(string pageSource, string tagName,
            string attributeName, string attributeValue, ActivityType activityType)
        {
            var actionAttributeValue = string.Empty;
            try
            {
                var InnerHtml =
                    HtmlParseUtility.GetInnerHtmlFromTagName(pageSource, tagName, attributeName, attributeValue);
                switch (activityType)
                {
                    case ActivityType.Upvote:
                        actionAttributeValue = Utilities.GetBetween(InnerHtml, "class=\"", "\"");
                        return !string.IsNullOrEmpty(actionAttributeValue)
                            ? actionAttributeValue
                            : actionAttributeValue = "_" + Utilities.GetBetween(InnerHtml, "class=\"_", "\"><i");
                        ;

                    case ActivityType.Downvote:
                        var lstInnerHtml = Regex.Split(InnerHtml, "id=\"downvote\"");
                        actionAttributeValue = GetAttributeValueForDownVote(activityType, InnerHtml);
                        return !string.IsNullOrEmpty(actionAttributeValue)
                            ? actionAttributeValue
                            : actionAttributeValue = "_" + Utilities.GetBetween(lstInnerHtml[1], "class=\"_", "\"><i");
                }
            }
            catch (Exception)
            {
                // ignored
            }

            return actionAttributeValue;
        }

        public static string GetAttributeValueForCommentVote(string pageSource, string tagName,
            string attributeName, string attributeValue, ActivityType activityType)
        {
            var actionAttributeValue = string.Empty;
            try
            {
                var InnerHtml =
                    HtmlParseUtility.GetInnerHtmlFromTagName(pageSource, tagName, attributeName, attributeValue);
                switch (activityType)
                {
                    case ActivityType.Upvote:
                        actionAttributeValue = Utilities.GetBetween(InnerHtml, "<button class=\"", "\"");
                        return !string.IsNullOrEmpty(actionAttributeValue)
                            ? actionAttributeValue
                            : actionAttributeValue = "_" + Utilities.GetBetween(InnerHtml, "class=\"_", "\"><i");
                        ;

                    case ActivityType.Downvote:
                        var lstInnerHtml = Regex.Split(InnerHtml, "id=\"downvote\"");
                        actionAttributeValue = GetAttributeValueForDownVote(activityType, InnerHtml);
                        return !string.IsNullOrEmpty(actionAttributeValue)
                            ? actionAttributeValue
                            : actionAttributeValue = Utilities.GetBetween(lstInnerHtml[1], "class=\"", "\"><i");
                }
            }
            catch (Exception)
            {
                // ignored
            }

            return actionAttributeValue;
        }

        private static string GetAttributeValueForDownVote(ActivityType activityType, string InnerHtml)
        {
            //Attribute value for downvote comment for custom design
            switch (activityType)
            {
                case ActivityType.Downvote:
                    var lstInnerHtml = Regex.Split(InnerHtml, "id=\"downvote\"");
                    return Utilities.GetBetween(lstInnerHtml[1], "class=\"", "\"");
            }

            return Utilities.GetBetween(InnerHtml, "<button class=\"", "\"");
        }

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

        public static bool IsPostPublishable(string pageSource, string tagName, string containText)
        {
            try
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(pageSource);
                var nodeCollection = htmlDoc.DocumentNode.SelectNodes($"//{tagName}");
                var postCount = 0;
                foreach (var node in nodeCollection)
                {
                    var outerHtml = node.OuterHtml;
                    var post = node.InnerText;
                    if (post == "Post")
                        postCount++;
                    if (postCount == 2)
                        break;
                    if (outerHtml.Contains(containText) && outerHtml.Contains("disabled"))
                        return false;
                    if (containText == "Images &amp; Video")
                    {
                        if ((outerHtml.Contains(containText) || outerHtml.Contains("Image")) && outerHtml.Contains("disabled"))
                            return false;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return true;
        }
        public static string GetMainAttributeValueToVoteForPost(string pageSource, string tagName, string attributeName,
            string attributeValue)
        {
            var mainAttributeValue = string.Empty;
            try
            {
                var InnerHtml =
                    HtmlParseUtility.GetInnerHtmlFromTagName(pageSource, tagName, attributeName, attributeValue);
                mainAttributeValue = Utilities.GetBetween(InnerHtml, "><div class=\"", "\"");
            }
            catch (Exception)
            {
                // ignored
            }

            return mainAttributeValue;
        }

        public static string GetMainAttributeValueToVoteForComment(string pageSource, string tagName,
            string attributeName, string attributeValue)
        {
            var mainAttributeValue = string.Empty;
            try
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(pageSource);
                var InnerHtml = htmlDoc.DocumentNode
                    .SelectSingleNode($"//{tagName}[starts-with(@{attributeName}, '{attributeValue}')]").InnerHtml;

                mainAttributeValue = Utilities.GetBetween(InnerHtml, "<div class=\"", "\"><button");
            }
            catch (Exception)
            {
                // ignored
            }

            return mainAttributeValue;
        }

        public static string GetMainAttributeValueForCommentVote(string pageSource, string tagName,
            string attributeName, string attributeValue)
        {
            var mainAttributeValue = string.Empty;
            try
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(pageSource);
                var outerHtml =
                    htmlDoc.DocumentNode.SelectNodes($"//{tagName}[starts-with(@{attributeName}, '{attributeValue}')]")
                        [1].OuterHtml;

                mainAttributeValue = Utilities.GetBetween(outerHtml, "<div class=\"", "\"");
            }
            catch (Exception)
            {
                // ignored
            }

            return mainAttributeValue;
        }

        public static string GetInnerTextWithPartialAttributeNameInSingleNode(string pageSource, string tagName,
            string attributeName, string attributeValue)
        {
            var text = string.Empty;
            try
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(pageSource);
                text = htmlDoc.DocumentNode
                    .SelectSingleNode($"//{tagName}[starts-with(@{attributeName}, '{attributeValue}')]").InnerText;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return text;
        }
    }
}