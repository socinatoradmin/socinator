using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.YdTables.Campaign;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using YoutubeDominatorCore.Response;
using YoutubeDominatorCore.YDEnums;
using YoutubeDominatorCore.YDEnums.VideoSearchFilterEnums;
using YoutubeDominatorCore.YoutubeLibrary.DAL;
using YoutubeDominatorCore.YoutubeModels;

namespace YoutubeDominatorCore.YDUtility
{
    public static class YdStatic
    {
        #region LikeComment Module
        public static string LikeCommentClass => "yt-spec-button-shape-next yt-spec-button-shape-next--text yt-spec-button-shape-next--mono yt-spec-button-shape-next--size-s yt-spec-button-shape-next--icon-button yt-spec-button-shape-next--override-small-size-icon";
        #endregion 
        public const string DefaultChannel = "Default Channel";
        public static string GetDefaultChannel => $"[{DefaultChannel}]";

        public static readonly object BrowserLock = new object();

        public static readonly object LockViewIncreaser = new object();

        public static Dictionary<string, Dictionary<string, List<string>>> UniqueCommentsListWithVideoId =
            new Dictionary<string, Dictionary<string, List<string>>>();

        public static readonly object LockInitializingCommentLockDict = new object();
        public static Dictionary<string, object> LockUniqueCommentsDict = new Dictionary<string, object>();

        public static Dictionary<string, Dictionary<string, bool>> FirstTimeUniqueCommentListFromDb =
            new Dictionary<string, Dictionary<string, bool>>();

        public static Dictionary<string, List<string>> UniqueCommentsListUniqueCmntUniqueAccount =
            new Dictionary<string, List<string>>();

        public static Dictionary<string, bool> FirstTimeUniqueCommentListFromDbUniqueCmntUniqueAccount =
            new Dictionary<string, bool>();

        public static int BrowserOpening { get; set; } = 0;
        public static int BrowserOpeningViewIncreaser { get; set; } = 0;

        public static string MyCoreLocation()
        {
            return @"D:\Atul_Projects\Socinator\youtubedominator-library\YoutubeDominatorCore";
        }

        public static Rectangle ScreenResolution()
        {
            return Screen.PrimaryScreen.Bounds;
        }
        public static KeyValuePair<int, int> GetScreenResolution()
        {
            int height = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;
            int width = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
            return new KeyValuePair<int, int>(width, height);
        }

        public static List<string> UnusedCommentsForUnique(string accountId, string campaignId, string videoUrl,
            List<string> msgList, IDbCampaignService campaignService, IDbAccountServiceScoped accountServiceScoped,
            bool campaignWise, bool accountwise, bool isAReply, string activityType)
        {
            var videoId = videoUrl;
            if (FirstTimeUniqueCommentListFromDb[campaignId][videoId])
            {
                FirstTimeUniqueCommentListFromDb[campaignId][videoId] = false;

                var usedComment = new List<string>();

                if (campaignWise)
                {
                    var postsDone = campaignService.Get<InteractedPosts>(x => x.ActivityType == activityType);
                    postsDone = (isAReply
                        ? postsDone.Where(x => x.InteractedCommentUrl == videoId)
                        : postsDone.Where(x => x.VideoUrl.Contains(videoId))).ToList();
                    postsDone.ForEach(x =>
                    {
                        if (!string.IsNullOrEmpty(x.MyCommentedText))
                            usedComment.Add(x.MyCommentedText);
                    });
                }
                else
                {
                    var postsDone = accountServiceScoped.Get<InteractedPosts>(x => x.ActivityType == activityType);
                    postsDone = (isAReply
                        ? postsDone.Where(x => x.InteractedCommentUrl == videoId)
                        : postsDone.Where(x => x.VideoUrl.Contains(videoId))).ToList();
                    postsDone.ForEach(x =>
                    {
                        if (!string.IsNullOrEmpty(x.MyCommentedText))
                            usedComment.Add(x.MyCommentedText);
                    });
                }

                if (usedComment.Count > 0)
                    foreach (var cmnt in msgList)
                    {
                        var usedCommentsChangedTemp = new List<string>();
                        usedCommentsChangedTemp.AddRange(usedComment);
                        var addit = true;
                        var index = 0;
                        if (usedCommentsChangedTemp.Count > 0)
                            foreach (var usedCmnt in usedCommentsChangedTemp)
                            {
                                ++index;
                                if (usedCmnt == cmnt)
                                {
                                    usedComment.RemoveAt(index - 1);
                                    addit = false;
                                    break;
                                }
                            }

                        if (addit)
                            UniqueCommentsListWithVideoId[campaignId][videoId].Add(cmnt);
                    }
                else
                    UniqueCommentsListWithVideoId[campaignId][videoId].AddRange(msgList);
            }


            return UniqueCommentsListWithVideoId[campaignId][videoId];
        }

        public static void RemoveUsedUniqueComment(ref string usedUniqueComment, string campaignId,
            string processingUrl)
        {
            if (UniqueCommentsListWithVideoId?[campaignId]?[processingUrl]?.Count > 0)
            {
                var tempListComments = new List<string>();

                usedUniqueComment = UniqueCommentsListWithVideoId[campaignId][processingUrl]
                    .ElementAt(new Random().Next(0, UniqueCommentsListWithVideoId[campaignId][processingUrl].Count));
                tempListComments.AddRange(UniqueCommentsListWithVideoId[campaignId][processingUrl]);

                var index = 0;
                foreach (var cmnt in tempListComments)
                {
                    ++index;
                    if (cmnt == usedUniqueComment)
                    {
                        UniqueCommentsListWithVideoId[campaignId][processingUrl].RemoveAt(index - 1);
                        break;
                    }
                }
            }
        }

        public static List<string> UnusedCommentsForUniqueCommentFromUniqueUser(string campaignId, List<string> msgList,
            IDbCampaignService campaignService, IDbAccountServiceScoped accountServiceScoped, bool campaignWise,
            string activityType)
        {
            if (FirstTimeUniqueCommentListFromDbUniqueCmntUniqueAccount[campaignId])
            {
                FirstTimeUniqueCommentListFromDbUniqueCmntUniqueAccount[campaignId] = false;

                var usedComment = new List<string>();

                if (campaignWise)
                {
                    var postsDone = campaignService.Get<InteractedPosts>(x => x.ActivityType == activityType);
                    postsDone.ForEach(x =>
                    {
                        if (!string.IsNullOrEmpty(x.MyCommentedText))
                            usedComment.Add(x.MyCommentedText);
                    });
                }
                else
                {
                    var postsDone = accountServiceScoped.Get<InteractedPosts>(x => x.ActivityType == activityType);
                    postsDone.ForEach(x =>
                    {
                        if (!string.IsNullOrEmpty(x.MyCommentedText))
                            usedComment.Add(x.MyCommentedText);
                    });
                }

                if (usedComment.Count > 0)
                    foreach (var cmnt in msgList)
                    {
                        var usedCommentsChangedTemp = new List<string>();
                        usedCommentsChangedTemp.AddRange(usedComment);
                        var addit = true;
                        var index = 0;
                        if (usedCommentsChangedTemp.Count > 0)
                            foreach (var usedCmnt in usedCommentsChangedTemp)
                            {
                                ++index;
                                if (usedCmnt == cmnt)
                                {
                                    usedComment.RemoveAt(index - 1);
                                    addit = false;
                                    break;
                                }
                            }

                        if (addit)
                            UniqueCommentsListUniqueCmntUniqueAccount[campaignId].Add(cmnt);
                    }
                else
                    UniqueCommentsListUniqueCmntUniqueAccount[campaignId].AddRange(msgList);
            }

            return UniqueCommentsListUniqueCmntUniqueAccount[campaignId];
        }

        public static void RemoveUsedUniqueCommentFromUniqueUser(ref string usedUniqueComment, string campaignId)
        {
            if (UniqueCommentsListUniqueCmntUniqueAccount?[campaignId]?.Count > 0)
            {
                var tempListComments = new List<string>();

                usedUniqueComment = UniqueCommentsListUniqueCmntUniqueAccount[campaignId]
                    .ElementAt(new Random().Next(0, UniqueCommentsListUniqueCmntUniqueAccount[campaignId].Count));
                tempListComments.AddRange(UniqueCommentsListUniqueCmntUniqueAccount[campaignId]);

                var index = 0;
                foreach (var cmnt in tempListComments)
                {
                    ++index;
                    if (cmnt == usedUniqueComment)
                    {
                        UniqueCommentsListUniqueCmntUniqueAccount[campaignId].RemoveAt(index - 1);
                        break;
                    }
                }
            }
        }

        /// <summary>
        ///     returns Youtube Module Type (Post or User or Channel) - On which Youtube thing, the ActivityType is going to
        ///     perform
        /// </summary>
        /// <param name="actType"></param>
        /// <returns>YoutubeElements</returns>
        public static YdElements GetYdElementByActivityType(this ActivityType actType)
        {
            if (actType == ActivityType.PostScraper || actType == ActivityType.Like ||
                actType == ActivityType.Comment ||
                actType == ActivityType.LikeComment || actType == ActivityType.Dislike ||
                actType == ActivityType.ViewIncreaser || actType == ActivityType.ReportVideo ||
                actType == ActivityType.CommentScraper)
                return YdElements.Posts;
            if (actType == ActivityType.ChannelScraper || actType == ActivityType.Subscribe ||
                actType == ActivityType.UnSubscribe || actType == ActivityType.BroadcastMessages)
                return YdElements.Channel;

            return YdElements.None;
        }

        /// <summary>
        ///     Match Any string With custom expression
        /// </summary>
        /// <param name="sourceString">The string Text in which particular string gonna be match</param>
        /// <param name="expression">Match Expression</param>
        /// <returns></returns>
        public static MatchCollection StringMatches(this string sourceString, string expression)
        {
            try
            {
                if (sourceString.Substring(5) == "") sourceString = sourceString.Insert(4, "0");
                var getIt = Regex.Matches(sourceString, expression, RegexOptions.Compiled | RegexOptions.IgnoreCase);
                return getIt;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public static string GetTextRemoveHtmlTags(string htmlPage)
        {
            var returnString = "";
            try
            {
                htmlPage = htmlPage.Replace("<br/>", "\n").Replace("</p>", "\n").Replace("</tr>", "\n");

                while (htmlPage.Contains("<") && htmlPage.Contains(">"))
                {
                    var replacingPart = "<" + Utilities.GetBetween(htmlPage, "<", ">") + ">";
                    if (!string.IsNullOrEmpty(replacingPart.Trim()))
                        htmlPage = htmlPage.Replace(replacingPart, "");
                }

                returnString = WebUtility.HtmlDecode(htmlPage.Trim());
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return returnString;
        }

        /// <summary>
        ///     Get month's number
        /// </summary>
        /// <param name="month">Month Name</param>
        /// <returns></returns>
        public static string MonthNumber(string month)
        {
            month = month.ToLower().Trim();
            if (month.Contains("jan"))
                return "1";
            if (month.Contains("feb"))
                return "2";
            if (month.Contains("mar"))
                return "3";
            if (month.Contains("apr"))
                return "4";
            if (month.Contains("may"))
                return "5";
            if (month.Contains("jun"))
                return "6";
            if (month.Contains("jul"))
                return "7";
            if (month.Contains("aug"))
                return "8";
            if (month.Contains("sep"))
                return "9";
            if (month.Contains("oct"))
                return "10";
            if (month.Contains("nov"))
                return "11";
            return month.Contains("dec") ? "12" : "Not Found";
        }

        /// <summary>
        ///     Returns true if keyword is an email
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public static bool IsEmail(string keyword)
        {
            const string strRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                                    @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                                    @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
            var re = new Regex(strRegex);
            return re.IsMatch(keyword);
        }

        /// <summary>
        ///     Remove all the character except a whole number inside the selected string
        /// </summary>
        /// <param name="gotString"></param>
        /// <returns></returns>
        public static string RemoveAllExceptWholeNumb(this string gotString)
        {
            gotString = Uri.EscapeDataString(gotString.Trim());

            var escapedStringMatch = Regex.Match(gotString, @"(%)\w{2}");

            while (escapedStringMatch.Success)
            {
                gotString = gotString.Replace(escapedStringMatch.ToString(), "");
                escapedStringMatch = escapedStringMatch.NextMatch();
            }

            return YoutubeUtilities.GetIntegerOnlyString(gotString);
        }

        public static void ExtractCommonJsonData(ref string pageSource)
        {
            DecodeSomeUnknownEntityInHtml(ref pageSource);
            if (pageSource.ToLower().StartsWith("json.parse("))
                pageSource = pageSource.Trim().TrimEnd(')').Replace("JSON.parse(", "").Replace("\\\"", "\"").Trim('\"')
                    .Trim('\'');
        }

        public static void DecodeSomeUnknownEntityInHtml(ref string pageSource)
        {
            pageSource = pageSource.Trim('\'').Replace("\\x00", "\u0000");
            pageSource = pageSource.Replace("\\x01", "\u0001");
            pageSource = pageSource.Replace("\\x02", "\u0002");
            pageSource = pageSource.Replace("\\x03", "\u0003");
            pageSource = pageSource.Replace("\\x04", "\u0004");
            pageSource = pageSource.Replace("\\x05", "\u0005");
            pageSource = pageSource.Replace("\\x06", "\u0006");
            pageSource = pageSource.Replace("\\x07", "\u0007");
            pageSource = pageSource.Replace("\\x08", "\b");
            pageSource = pageSource.Replace("\\x09", "\t");
            pageSource = pageSource.Replace("\\x0a", "\n");
            pageSource = pageSource.Replace("\\x0b", "\u000b");
            pageSource = pageSource.Replace("\\x0c", "\f");
            pageSource = pageSource.Replace("\\x0d", "\r");
            pageSource = pageSource.Replace("\\x0e", "\u000e");
            pageSource = pageSource.Replace("\\x0f", "\u000f");
            pageSource = pageSource.Replace("\\x10", "\u0010");
            pageSource = pageSource.Replace("\\x11", "\u0011");
            pageSource = pageSource.Replace("\\x12", "\u0012");
            pageSource = pageSource.Replace("\\x13", "\u0013");
            pageSource = pageSource.Replace("\\x14", "\u0014");
            pageSource = pageSource.Replace("\\x15", "\u0015");
            pageSource = pageSource.Replace("\\x16", "\u0016");
            pageSource = pageSource.Replace("\\x17", "\u0017");
            pageSource = pageSource.Replace("\\x18", "\u0018");
            pageSource = pageSource.Replace("\\x19", "\u0019");
            pageSource = pageSource.Replace("\\x1a", "\u001a");
            pageSource = pageSource.Replace("\\x1b", "\u001b");
            pageSource = pageSource.Replace("\\x1c", "\u001c");
            pageSource = pageSource.Replace("\\x1d", "\u001d");
            pageSource = pageSource.Replace("\\x1e", "\u001e");
            pageSource = pageSource.Replace("\\x1f", "\u001f");
            pageSource = pageSource.Replace("\\x20", " ");
            pageSource = pageSource.Replace("\\x21", "!");
            pageSource = pageSource.Replace("\\x22", "\"");
            pageSource = pageSource.Replace("\\x23", "#");
            pageSource = pageSource.Replace("\\x24", "$");
            pageSource = pageSource.Replace("\\x25", "%");
            pageSource = pageSource.Replace("\\x26", "&");
            pageSource = pageSource.Replace("\\x27", "'");
            pageSource = pageSource.Replace("\\x28", "(");
            pageSource = pageSource.Replace("\\x29", ")");
            pageSource = pageSource.Replace("\\x2a", "*");
            pageSource = pageSource.Replace("\\x2b", "+");
            pageSource = pageSource.Replace("\\x2c", ",");
            pageSource = pageSource.Replace("\\x2d", "-");
            pageSource = pageSource.Replace("\\x2e", ".");
            pageSource = pageSource.Replace("\\x2f", "/");
            pageSource = pageSource.Replace("\\x30", "0");
            pageSource = pageSource.Replace("\\x31", "1");
            pageSource = pageSource.Replace("\\x32", "2");
            pageSource = pageSource.Replace("\\x33", "3");
            pageSource = pageSource.Replace("\\x34", "4");
            pageSource = pageSource.Replace("\\x35", "5");
            pageSource = pageSource.Replace("\\x36", "6");
            pageSource = pageSource.Replace("\\x37", "7");
            pageSource = pageSource.Replace("\\x38", "8");
            pageSource = pageSource.Replace("\\x39", "9");
            pageSource = pageSource.Replace("\\x3a", ":");
            pageSource = pageSource.Replace("\\x3b", ";");
            pageSource = pageSource.Replace("\\x3c", "<");
            pageSource = pageSource.Replace("\\x3d", "=");
            pageSource = pageSource.Replace("\\x3e", ">");
            pageSource = pageSource.Replace("\\x3f", "?");
            pageSource = pageSource.Replace("\\x7b", "{");
            pageSource = pageSource.Replace("\\x7d", "}");
            pageSource = pageSource.Replace("\\x5b", "[");
            pageSource = pageSource.Replace("\\x5d", "]");
            pageSource = pageSource.Replace("\x60", "`");
            pageSource = pageSource.Replace("\x5C", "\\");
        }

        /// <summary>
        ///     Get a deleting query from the list of QueryContent Just by comparing the another QueryContent with any of the list
        /// </summary>
        /// <param name="queryList">The list of QueryContent</param>
        /// <param name="queryToDelete">the another QueryContent to compare</param>
        /// <returns></returns>
        public static QueryContent GetDeletingQuery(this ObservableCollection<QueryContent> queryList,
            QueryContent queryToDelete)
        {
            return queryList.FirstOrDefault(x =>
                x.Content.QueryType == queryToDelete.Content.QueryType &&
                x.Content.QueryValue == queryToDelete.Content.QueryValue);
        }

        /// <summary>
        ///     returns list of strings (Except repeated and empty string) by splitting the text(string) with NextLine('\n')
        /// </summary>
        /// <param name="text">string sent as parameter</param>
        /// <returns></returns>
        public static List<string> GetListSplittedWithNextLine(string text)
        {
            try
            {
                if (!string.IsNullOrEmpty(text))
                    return text.Split('\n').Where(x => !string.IsNullOrEmpty(x.Trim())).Select(y => y.Trim()).Distinct()
                        .ToList();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return new List<string>();
        }

        public static string ChannelUrl(string channelId, string channelUsername)
        {
            var channelIdentity = !string.IsNullOrEmpty(channelId) && channelId.Contains("@") ? channelId : channelUsername;
            return RecorrectChannelUrl(channelIdentity, string.IsNullOrEmpty(channelId));
        }

        public static string VideoUrl(string videoId, string commentId = null)
        {
            var url = $"https://www.youtube.com/watch?v={videoId}";
            if (!string.IsNullOrWhiteSpace(commentId))
                url = $"{url}&lc={commentId}";
            return url;
        }

        public static string RecorrectChannelUrl(string channelUrl, bool hasChannelUsername = false)
        {
            try
            {
                if (channelUrl.Contains("/user/"))
                {
                    if (!channelUrl.Contains("/about"))
                        return $"{channelUrl.TrimEnd('/')}/about";
                    return channelUrl;
                }

                if (channelUrl.Contains("/channel/"))
                {
                    if (!channelUrl.Contains("/about"))
                        return $"{channelUrl.TrimEnd('/')}/about";
                    return channelUrl;
                }

                // var isId = channelUrl.ToList().Any(x => !((x > 47 && x < 58) || (x > 64 && x < 91) || (x > 96 && x < 123)));
                 var channelNameType = hasChannelUsername ? "channel" : "user";
                return channelUrl.Contains("@") ? $"https://www.youtube.com/{channelUrl}/about" : $"https://www.youtube.com/@{channelUrl}/";
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return channelUrl;
            }
        }

        /// <summary>
        ///     Key = ChannelId, Value = ChannelUsername
        /// </summary>
        /// <param name="queryValue"></param>
        /// <returns></returns>
        public static KeyValuePair<string, string> ChannelIdUsernameApart(string queryValue)
        {
            try
            {
                var channelUsername = queryValue.Contains("/user/")
                        ? Utilities.GetBetween($"{queryValue}/", "/user/", "/")
                        : "";
                var channelId = queryValue.Contains("/channel/")
                    ? Utilities.GetBetween($"{queryValue}/", "/channel/", "/")
                    : "";
                if (string.IsNullOrEmpty(channelId))
                    channelId = Regex.Split(queryValue, "/")[3].ToString();
                if (string.IsNullOrEmpty(channelId) && string.IsNullOrEmpty(channelUsername))
                    channelId = queryValue;
                return new KeyValuePair<string, string>(channelId, channelUsername);
            }
            catch (Exception)
            {
                return new KeyValuePair<string, string>(string.Empty, queryValue);
            }
        }

        public static string AsPerCsv(this string data, bool isLast = false)
        {
            return $"\"{data?.Replace("\"", "\"\"")}\"" + (!isLast ? "," : "");
        }

        public static string SearchUrlParamFromFilter(SearchVideoFilterModel searchVideoFilterModel)
        {
            try
            {
                if (searchVideoFilterModel == null ||
                    searchVideoFilterModel.UploadDate == UploadDate.None.ToString() &&
                    searchVideoFilterModel.Duration == Duration.None.ToString() &&
                    searchVideoFilterModel.SortBy == SortBy.Relevance.ToString())
                    return null;

                if (searchVideoFilterModel.Duration == Duration.None.ToString())
                {
                    if (searchVideoFilterModel.SortBy == SortBy.Relevance.ToString())
                        switch (searchVideoFilterModel.UploadDate)
                        {
                            case "None": return "EgIQAQ%253D%253D";
                            case "LastHour": return "EgQIARAB";
                            case "Today": return "EgQIAhAB";
                            case "ThisWeek": return "EgQIAxAB";
                            case "ThisMonth": return "EgQIBBAB";
                            case "ThisYear": return "EgQIBRAB";
                        }
                    else if (searchVideoFilterModel.SortBy == SortBy.UploadDate.ToString())
                        switch (searchVideoFilterModel.UploadDate)
                        {
                            case "None": return "CAISAhAB";
                            case "LastHour": return "CAISBAgBEAE%253D";
                            case "Today": return "CAISBAgCEAE%253D";
                            case "ThisWeek": return "CAISBAgDEAE%253D";
                            case "ThisMonth": return "CAISBAgEEAE%253D";
                            case "ThisYear": return "CAISBAgFEAE%253D";
                        }
                    else if (searchVideoFilterModel.SortBy == SortBy.ViewCount.ToString())
                        switch (searchVideoFilterModel.UploadDate)
                        {
                            case "None": return "CAMSAhAB";
                            case "LastHour": return "CAMSBAgBEAE%253D";
                            case "Today": return "CAMSBAgCEAE%253D";
                            case "ThisWeek": return "CAMSBAgDEAE%253D";
                            case "ThisMonth": return "CAMSBAgEEAE%253D";
                            case "ThisYear": return "CAMSBAgFEAE%253D";
                        }
                    else if (searchVideoFilterModel.SortBy == SortBy.Rating.ToString())
                        switch (searchVideoFilterModel.UploadDate)
                        {
                            case "None": return "CAESAhAB";
                            case "LastHour": return "CAESBAgBEAE%253D";
                            case "Today": return "CAESBAgCEAE%253D";
                            case "ThisWeek": return "CAESBAgDEAE%253D";
                            case "ThisMonth": return "CAESBAgEEAE%253D";
                            case "ThisYear": return "CAESBAgFEAE%253D";
                        }
                }
                else if (searchVideoFilterModel.Duration == Duration.Short.ToString())
                {
                    if (searchVideoFilterModel.SortBy == SortBy.Relevance.ToString())
                        switch (searchVideoFilterModel.UploadDate)
                        {
                            case "None": return "EgQQARgB";
                            case "LastHour": return "EgYIARABGAE%253D";
                            case "Today": return "EgYIAhABGAE%253D";
                            case "ThisWeek": return "EgYIAxABGAE%253D";
                            case "ThisMonth": return "EgYIBBABGAE%253D";
                            case "ThisYear": return "EgYIBRABGAE%253D";
                        }
                    else if (searchVideoFilterModel.SortBy == SortBy.UploadDate.ToString())
                        switch (searchVideoFilterModel.UploadDate)
                        {
                            case "None": return "CAISBBABGAE%253D";
                            case "LastHour": return "CAISBggBEAEYAQ%253D%253D";
                            case "Today": return "CAISBggCEAEYAQ%253D%253D";
                            case "ThisWeek": return "CAISBggDEAEYAQ%253D%253D";
                            case "ThisMonth": return "CAISBggEEAEYAQ%253D%253D";
                            case "ThisYear": return "CAISBggFEAEYAQ%253D%253D";
                        }
                    else if (searchVideoFilterModel.SortBy == SortBy.ViewCount.ToString())
                        switch (searchVideoFilterModel.UploadDate)
                        {
                            case "None": return "CAMSBBABGAE%253D";
                            case "LastHour": return "CAMSBggBEAEYAQ%253D%253D";
                            case "Today": return "CAMSBggCEAEYAQ%253D%253D";
                            case "ThisWeek": return "CAMSBggDEAEYAQ%253D%253D";
                            case "ThisMonth": return "CAMSBggEEAEYAQ%253D%253D";
                            case "ThisYear": return "CAMSBggFEAEYAQ%253D%253D";
                        }
                    else if (searchVideoFilterModel.SortBy == SortBy.Rating.ToString())
                        switch (searchVideoFilterModel.UploadDate)
                        {
                            case "None": return "CAESBBABGAE%253D";
                            case "LastHour": return "CAESBggBEAEYAQ%253D%253D";
                            case "Today": return "CAESBggCEAEYAQ%253D%253D";
                            case "ThisWeek": return "CAESBggDEAEYAQ%253D%253D";
                            case "ThisMonth": return "CAESBggEEAEYAQ%253D%253D";
                            case "ThisYear": return "CAESBggFEAEYAQ%253D%253D";
                        }
                }
                else if (searchVideoFilterModel.Duration == Duration.Long.ToString())
                {
                    if (searchVideoFilterModel.SortBy == SortBy.Relevance.ToString())
                        switch (searchVideoFilterModel.UploadDate)
                        {
                            case "None": return "CAASBBABGAI%253D";
                            case "LastHour": return "EgYIARABGAI%253D";
                            case "Today": return "EgYIAhABGAI%253D";
                            case "ThisWeek": return "EgYIAxABGAI%253D";
                            case "ThisMonth": return "EgYIBBABGAI%253D";
                            case "ThisYear": return "EgYIBRABGAI%253D";
                        }
                    else if (searchVideoFilterModel.SortBy == SortBy.UploadDate.ToString())
                        switch (searchVideoFilterModel.UploadDate)
                        {
                            case "None": return "CAISBBABGAI%253D";
                            case "LastHour": return "CAISBggBEAEYAg%253D%253D";
                            case "Today": return "CAISBggCEAEYAg%253D%253D";
                            case "ThisWeek": return "CAISBggDEAEYAg%253D%253D";
                            case "ThisMonth": return "CAISBggEEAEYAg%253D%253D";
                            case "ThisYear": return "CAISBggFEAEYAg%253D%253D";
                        }
                    else if (searchVideoFilterModel.SortBy == SortBy.ViewCount.ToString())
                        switch (searchVideoFilterModel.UploadDate)
                        {
                            case "None": return "CAMSBBABGAI%253D";
                            case "LastHour": return "CAMSBggBEAEYAg%253D%253D";
                            case "Today": return "CAMSBggCEAEYAg%253D%253D";
                            case "ThisWeek": return "CAMSBggDEAEYAg%253D%253D";
                            case "ThisMonth": return "CAMSBggEEAEYAg%253D%253D";
                            case "ThisYear": return "CAMSBggFEAEYAg%253D%253D";
                        }
                    else if (searchVideoFilterModel.SortBy == SortBy.Rating.ToString())
                        switch (searchVideoFilterModel.UploadDate)
                        {
                            case "None": return "CAESBBABGAI%253D";
                            case "LastHour": return "CAESBggBEAEYAg%253D%253D";
                            case "Today": return "CAESBggCEAEYAg%253D%253D";
                            case "ThisWeek": return "CAESBggDEAEYAg%253D%253D";
                            case "ThisMonth": return "CAESBggEEAEYAg%253D%253D";
                            case "ThisYear": return "CAESBggFEAEYAg%253D%253D";
                        }
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        public static bool IsBetween(ulong minValue, ulong midValue, ulong maxValue)
        {
            return midValue >= minValue && midValue <= maxValue;
        }

        public static string PublishVideoPostData(string videoId, string postTitle, string postDescription,
            string fileNameWithoutExtension, string sessionToken)
        {
            if (string.IsNullOrEmpty(postTitle) && string.IsNullOrEmpty(postDescription))
                return "title=" + Uri.EscapeDataString(fileNameWithoutExtension) +
                       "&description&keywords&privacy=public&notify_via_email=true&share_emails&deleted_ogids&deleted_circle_ids&deleted_emails&privacy_draft=none&publish_time=2018-10-30T10%3A30&publish_timezone=UTC&still_id=2&thumbnail_preview_version&allow_comments=yes&allow_comments_detail=all&allow_comments_sort_order=top_comments&allow_ratings=yes&reuse=all_rights_reserved&user_enabled_live_chat_replay=yes&captions_certificate_reason&allow_embedding=yes&creator_share_feeds=yes&audio_language&recorded_date&allow_public_stats=yes&live_premiere=no&creator_share_gplus=no&creator_share_twitter=no&self_racy=no&captions_crowdsource=no&category=22&modified_fields=privacy%2Cprivacy_draft&video_id=" +
                       videoId + "&session_token=" + Uri.EscapeDataString(sessionToken);

            if (!string.IsNullOrEmpty(postTitle) && string.IsNullOrEmpty(postDescription))
                return "title=" + Uri.EscapeDataString(postTitle) +
                       "&description&keywords&privacy=public&notify_via_email=true&share_emails&deleted_ogids&deleted_circle_ids&deleted_emails&privacy_draft=none&publish_time=2018-10-30T10%3A30&publish_timezone=UTC&still_id=2&thumbnail_preview_version&allow_comments=yes&allow_comments_detail=all&allow_comments_sort_order=top_comments&allow_ratings=yes&reuse=all_rights_reserved&user_enabled_live_chat_replay=yes&captions_certificate_reason&allow_embedding=yes&creator_share_feeds=yes&audio_language&recorded_date&allow_public_stats=yes&live_premiere=no&creator_share_gplus=no&creator_share_twitter=no&self_racy=no&captions_crowdsource=no&category=22&modified_fields=title%2Cdescription%2Cprivacy%2Cprivacy_draft&video_id=" +
                       videoId + "&session_token=" + Uri.EscapeDataString(sessionToken);

            if (string.IsNullOrEmpty(postTitle) && !string.IsNullOrEmpty(postDescription))
                return "title=" + Uri.EscapeDataString(fileNameWithoutExtension) +
                       "&description=" + Uri.EscapeDataString(postDescription) +
                       "&keywords&privacy=public&notify_via_email=true&share_emails&deleted_ogids&deleted_circle_ids&deleted_emails&privacy_draft=none&publish_time=2018-10-30T10%3A30&publish_timezone=UTC&still_id=2&thumbnail_preview_version&allow_comments=yes&allow_comments_detail=all&allow_comments_sort_order=top_comments&allow_ratings=yes&reuse=all_rights_reserved&user_enabled_live_chat_replay=yes&captions_certificate_reason&allow_embedding=yes&creator_share_feeds=yes&audio_language&recorded_date&allow_public_stats=yes&live_premiere=no&creator_share_gplus=no&creator_share_twitter=no&self_racy=no&captions_crowdsource=no&category=22&modified_fields=title%2Cdescription%2Cprivacy%2Cprivacy_draft&video_id=" +
                       videoId + "&session_token=" + Uri.EscapeDataString(sessionToken);

            return "title=" + Uri.EscapeDataString(postTitle ?? "") +
                   "&description=" + Uri.EscapeDataString(postDescription ?? "") +
                   "&keywords&privacy=public&notify_via_email=true&share_emails&deleted_ogids&deleted_circle_ids&deleted_emails&privacy_draft=none&publish_time=2018-10-30T10%3A30&publish_timezone=UTC&still_id=2&thumbnail_preview_version&allow_comments=yes&allow_comments_detail=all&allow_comments_sort_order=top_comments&allow_ratings=yes&reuse=all_rights_reserved&user_enabled_live_chat_replay=yes&captions_certificate_reason&allow_embedding=yes&creator_share_feeds=yes&audio_language&recorded_date&allow_public_stats=yes&live_premiere=no&creator_share_gplus=no&creator_share_twitter=no&self_racy=no&captions_crowdsource=no&category=22&modified_fields=title%2Cdescription%2Cprivacy%2Cprivacy_draft&video_id=" +
                   videoId + "&session_token=" + Uri.EscapeDataString(sessionToken);
        }

        public static string UploadingVideoPostData(string fileNameWithoutExtension, string fileExtension,
            long fileInfoLength, UploadPageResponseHandler clickedUpload)
        {
            return
                "{\"protocolVersion\":\"0.8\",\"createSessionRequest\":{\"fields\":[{\"external\":{\"name\":\"file\",\"filename\":\""
                + fileNameWithoutExtension + fileExtension + "\",\"put\":{},\"size\":" + fileInfoLength +
                "}},{\"inlined\":{\"name\":\"return_address\",\"content\":\"www.youtube.com\",\"contentType\":\"text/plain\"}},{\"inlined\":{\"name\":\"user_token\",\"content\":\"" +
                clickedUpload.UserToken +
                "\",\"contentType\":\"text/plain\"}},{\"inlined\":{\"name\":\"authuser\",\"content\":\"0\",\"contentType\":\"text/plain\"}},{\"inlined\":{\"name\":\"uploader_type\",\"content\":\"Web_XHR\",\"contentType\":\"text/plain\"}},{\"inlined\":{\"name\":\"frontend_id\",\"content\":\"web_upload:" +
                clickedUpload.FrontEndUploadIdBase +
                ":0\",\"contentType\":\"text/plain\"}},{\"inlined\":{\"name\":\"experiment_ids\",\"content\":\"" +
                clickedUpload.ExperimentIds +
                "\",\"contentType\":\"text/plain\"}},{\"inlined\":{\"name\":\"field_privacy\",\"content\":\"public\",\"contentType\":\"text/plain\"}},{\"inlined\":{\"name\":\"field_myvideo_title\",\"content\":\"" +
                fileNameWithoutExtension?.Replace("_", " ") +
                "\",\"contentType\":\"text/plain\"}},{\"inlined\":{\"name\":\"field_myvideo_descr\",\"content\":\"\",\"contentType\":\"text/plain\"}},{\"inlined\":{\"name\":\"field_myvideo_keywords\",\"content\":\"\",\"contentType\":\"text/plain\"}},{\"inlined\":{\"name\":\"allow_public_stats\",\"content\":\"yes\",\"contentType\":\"text/plain\"}},{\"inlined\":{\"name\":\"allow_comments\",\"content\":\"yes\",\"contentType\":\"text/plain\"}},{\"inlined\":{\"name\":\"allow_comments_detail\",\"content\":\"all\",\"contentType\":\"text/plain\"}},{\"inlined\":{\"name\":\"allow_ratings\",\"content\":\"yes\",\"contentType\":\"text/plain\"}},{\"inlined\":{\"name\":\"privacy_draft\",\"content\":\"public\",\"contentType\":\"text/plain\"}},{\"inlined\":{\"name\":\"session_token\",\"content\":\"" +
                clickedUpload.SessionToken + "\",\"contentType\":\"text/plain\"}}]}}";
        }

        public static string UploadingVideoPostDataWithPageId(string pageId, string fileNameWithoutExtension,
            string fileExtension, long fileInfoLength, UploadPageResponseHandler clickedUpload)
        {
            return
                "{\"protocolVersion\":\"0.8\",\"createSessionRequest\":{\"fields\":[{\"external\":{\"name\":\"file\",\"filename\":\""
                + fileNameWithoutExtension + fileExtension + "\",\"put\":{},\"size\":" + fileInfoLength +
                "}},{\"inlined\":{\"name\":\"return_address\",\"content\":\"www.youtube.com\",\"contentType\":\"text/plain\"}},{\"inlined\":{\"name\":\"user_token\",\"content\":\"" +
                clickedUpload.UserToken +
                "\",\"contentType\":\"text/plain\"}},{\"inlined\":{\"name\":\"authuser\",\"content\":\"0\",\"contentType\":\"text/plain\"}},{\"inlined\":{\"name\":\"uploader_type\",\"content\":\"Web_XHR\",\"contentType\":\"text/plain\"}},{\"inlined\":{\"name\":\"frontend_id\",\"content\":\"web_upload:" +
                clickedUpload.FrontEndUploadIdBase +
                ":0\",\"contentType\":\"text/plain\"}}" +
                ",{\"inlined\":{\"name\":\"pageid\",\"content\":\"" + pageId +
                "\",\"contentType\":\"text/plain\"}}" +
                ",{\"inlined\":{\"name\":\"experiment_ids\",\"content\":\"" + clickedUpload.ExperimentIds +
                "\",\"contentType\":\"text/plain\"}},{\"inlined\":{\"name\":\"field_privacy\",\"content\":\"public\",\"contentType\":\"text/plain\"}},{\"inlined\":{\"name\":\"field_myvideo_title\",\"content\":\"" +
                fileNameWithoutExtension?.Replace("_", " ") +
                "\",\"contentType\":\"text/plain\"}},{\"inlined\":{\"name\":\"field_myvideo_descr\",\"content\":\"\",\"contentType\":\"text/plain\"}},{\"inlined\":{\"name\":\"field_myvideo_keywords\",\"content\":\"\",\"contentType\":\"text/plain\"}},{\"inlined\":{\"name\":\"allow_public_stats\",\"content\":\"yes\",\"contentType\":\"text/plain\"}},{\"inlined\":{\"name\":\"allow_comments\",\"content\":\"yes\",\"contentType\":\"text/plain\"}},{\"inlined\":{\"name\":\"allow_comments_detail\",\"content\":\"all\",\"contentType\":\"text/plain\"}},{\"inlined\":{\"name\":\"allow_ratings\",\"content\":\"yes\",\"contentType\":\"text/plain\"}},{\"inlined\":{\"name\":\"privacy_draft\",\"content\":\"public\",\"contentType\":\"text/plain\"}},{\"inlined\":{\"name\":\"session_token\",\"content\":\"" +
                clickedUpload.SessionToken + "\",\"contentType\":\"text/plain\"}}]}}";
        }
        #region CustomScripts
        public static string DismissButtonScript =>
            "[...document.querySelectorAll('button>span')].filter(x=>x.innerText?.toLowerCase().includes(\"dismiss\"))";
        public static string EmailorPhoneScript =>
            "document.querySelectorAll('input[aria-label=\"Email or phone\"]')";
        public static string ByTagAttributeAndValueButtonScript =>
           "[...document.querySelectorAll('{0}[{1}=\"{2}\"i]')].filter(x=>x.{3}?.toLowerCase().includes(\"{4}\"))";
        public static string ByTagWithAttributesAndExactValuesScript =>
           "[...document.querySelectorAll('{0}{1}')].filter(x=>x.{2}===\"{3}\"||x.{4}===\"{5}\")";
        public static string ByTagWithAttributesAndValuesButtonScript =>
           "[...document.querySelectorAll('{0}{1}')].filter(x=>x.{2}?.toLowerCase().includes(\"{3}\"))";
        public static string ByTagAttributeAndValueAndClassScript =>
            "[...document.querySelectorAll('{0}[{1}=\"{2}\"i]')].filter(x=>x.{3}?.toLowerCase().includes(\"{4}\")||x.className?.includes(\"{5}\"))";
        #endregion
    }
}