using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.ViewModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using YoutubeDominatorCore.Report;
using YoutubeDominatorCore.YDEnums;
using YoutubeDominatorCore.YoutubeModels;

namespace YoutubeDominatorCore.YDUtility
{
    public class YoutubeUtilities
    {
        /// <summary>
        ///     Create post data by using YoutubePost properties for Liking comment web request
        /// </summary>
        /// <param name="accountModel">Dominator Account Model of current youtube Account</param>
        /// <param name="youtubePost">Object containing Post's Details</param>
        /// <returns></returns>
        public JsonElements CreateJsonPostDataForLikeComment(DominatorAccountModel accountModel,
            YoutubePost youtubePost)
        {
            var updateCommentVoteAction = new JsonElements
            {
                VoteCount = new JsonElements
                {
                    Accessibility = new JsonElements
                    {
                        AccessibilityData = new JsonElements
                        {
                            Label = youtubePost.LikeCount + " likes"
                        }
                    },
                    SimpleText = youtubePost.LikeCount
                },
                VoteStatus = "LIKE"
            };

            var array = new JsonElements[1];
            array[0] = updateCommentVoteAction;

            var objJsonElement = new JsonElements
            {
                Sej = new JsonElements
                {
                    ClickTrackingParams = youtubePost.PostDataElements.TrackingParams,
                    CommandMetadata = new JsonElements
                    {
                        WebCommandMetadata = new JsonElements
                        {
                            Url = "/service_ajax",
                            SendPost = true
                        }
                    },
                    PerformCommentActionEndpoint = new JsonElements
                    {
                        Action = youtubePost.CommentActionParam,
                        ClientActions = array
                    }
                },
                Csn = youtubePost.PostDataElements.Csn,
                SessionToken = youtubePost.PostDataElements.XsrfToken
            };
            return objJsonElement;
        }
        public static bool IsIntegerOnly(string input)
        {
            if (string.IsNullOrEmpty(input)) return false;

            if (!input.Any(char.IsNumber))
                return false;

            var match = Regex.Match(input, "^[0-9]+$");
            return match.Success;
        }
        public static string GetIntegerOnlyString(string data)
        {
            if (data == null || data.Contains("null"))
                return "0";

            if (!data.Any(char.IsNumber))
                return "0";

            return Regex.Replace(data, "[^0-9]+", string.Empty);
        }

        public static string YoutubeElementsCountInNumber(string elementCount)
        {
            try
            {
                var splitted = elementCount.Split(' ')[0];
                if (string.IsNullOrWhiteSpace(splitted)) return "0";

                if (splitted.EndsWith("B")) return (Convert.ToDouble(splitted.TrimEnd('B')) * 100000000).ToString();

                if (splitted.EndsWith("M")) return (Convert.ToDouble(splitted.TrimEnd('M')) * 1000000).ToString();

                if (splitted.EndsWith("K"))
                {
                    return (Convert.ToDouble(splitted.TrimEnd('K')) * 1000).ToString();
                }

                var returnIt = splitted.RemoveAllExceptWholeNumb();
                if (string.IsNullOrWhiteSpace(returnIt))
                    return "0";
                return returnIt;
            }
            catch (Exception)
            {
                return "0";
            }
        }

        public string GeneratePostUrlFromYoutubeChannelObject(string channelUrl)
        {
            var youtubeChannelUrl = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(channelUrl))
                    if (channelUrl.Contains("/user/") || channelUrl.Contains("/channel/"))
                    {
                        if (channelUrl.StartsWith("http"))
                            youtubeChannelUrl = channelUrl;
                        else
                            youtubeChannelUrl = "https://www.youtube.com" + channelUrl;
                    }

                if (!string.IsNullOrEmpty(channelUrl))
                    if (string.IsNullOrEmpty(youtubeChannelUrl))
                        youtubeChannelUrl = channelUrl;

                if (youtubeChannelUrl.StartsWith("http"))
                {
                    if (!youtubeChannelUrl.Trim().TrimEnd('/').EndsWith("/about"))
                        youtubeChannelUrl = youtubeChannelUrl.Trim().TrimEnd('/') + "/about";
                }
                else
                {
                    if (!string.IsNullOrEmpty(youtubeChannelUrl))
                        youtubeChannelUrl = "https://www.youtube.com/channel/" + youtubeChannelUrl + "/about";
                }

                if (!youtubeChannelUrl.Trim().EndsWith("/about"))
                    youtubeChannelUrl = youtubeChannelUrl + "/about";
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return youtubeChannelUrl;
        }

        public string HeaderStringForChannelReport(bool addInteractedCommentUrl = false)
        {
            var sbH = new StringBuilder();
            sbH.Append(Enums.EnumChannelHeader.Id + ",");
            sbH.Append(Enums.EnumChannelHeader.AccountUsername + ",");
            sbH.Append(Enums.EnumChannelHeader.AccountChannelId + ",");
            sbH.Append(Enums.EnumChannelHeader.QueryType + ",");
            sbH.Append(Enums.EnumChannelHeader.QueryValue + ",");
            sbH.Append(Enums.EnumChannelHeader.InteractedChannelName + ",");
            sbH.Append(Enums.EnumChannelHeader.InteractedChannelId + ",");
            sbH.Append(Enums.EnumChannelHeader.ChannelUrl + ",");
            sbH.Append(Enums.EnumChannelHeader.ChannelDescription + ",");
            sbH.Append(Enums.EnumChannelHeader.ChannelJoinedDate + ",");
            sbH.Append(Enums.EnumChannelHeader.ChannelLocation + ",");
            sbH.Append(Enums.EnumChannelHeader.ChannelProfilePic + ",");
            sbH.Append(Enums.EnumChannelHeader.SubscriberCount + ",");
            sbH.Append(Enums.EnumChannelHeader.ViewsCount + ",");
            sbH.Append(Enums.EnumChannelHeader.VideosCount + ",");
            sbH.Append(Enums.EnumChannelHeader.IsSubscribed + ",");
            sbH.Append(Enums.EnumChannelHeader.InteractionTimeStamp + ",");
            sbH.Append(Enums.EnumChannelHeader.ActivityType + ",");
            sbH.Append(Enums.EnumChannelHeader.ExternalLinks);
            if (addInteractedCommentUrl)
                sbH.Append("," + Enums.EnumPostHeader.InteractedCommentUrl);
            return sbH.ToString();
        }

        public string HeaderStringForPostReport(bool addCommentedText = false, bool addRepeatedWatchCount = false,
            bool addCommentId = false, bool addInteractedCommentUrl = false, bool addReportedToVideoDetails = false)
        {
            var sbH = new StringBuilder();
            sbH.Append(Enums.EnumPostHeader.Id + ",");
            sbH.Append(Enums.EnumPostHeader.AccountUsername + ",");
            sbH.Append(Enums.EnumChannelHeader.AccountChannelId + ",");
            sbH.Append(Enums.EnumPostHeader.QueryType + ",");
            sbH.Append(Enums.EnumPostHeader.QueryValue + ",");
            sbH.Append(Enums.EnumPostHeader.VideoUrl + ",");
            sbH.Append(Enums.EnumPostHeader.VideoTitle + ",");
            if (addReportedToVideoDetails)
            {
                sbH.Append(Enums.EnumPostHeader.ReportedToVideoWithOption + ",");
                sbH.Append(Enums.EnumPostHeader.ReportedToVideoWithAdditionalText + ",");
                sbH.Append(Enums.EnumPostHeader.ReportedToVideoWithSelectedTimestamp + ",");
            }

            if (addCommentedText)
                sbH.Append(Enums.EnumPostHeader.MyCommentedText + ",");
            if (addCommentId)
                sbH.Append(Enums.EnumPostHeader.CommentId + ",");
            sbH.Append(Enums.EnumPostHeader.LikeStatus + ",");
            sbH.Append(Enums.EnumPostHeader.ViewsCount + ",");
            sbH.Append(Enums.EnumPostHeader.LikeCount + ",");
            sbH.Append(Enums.EnumPostHeader.DislikeCount + ",");
            sbH.Append(Enums.EnumPostHeader.CommentCount + ",");
            sbH.Append(Enums.EnumPostHeader.VideoDurationInHourMinSec + ",");
            sbH.Append(Enums.EnumPostHeader.PostDescription + ",");
            sbH.Append(Enums.EnumPostHeader.PublishedDate + ",");
            sbH.Append(Enums.EnumPostHeader.OwnerChannelName + ",");
            sbH.Append(Enums.EnumPostHeader.OwnerChannelId + ",");
            sbH.Append(Enums.EnumPostHeader.IsSubscribed + ",");
            sbH.Append(Enums.EnumPostHeader.SubscribeCount + ",");
            sbH.Append(Enums.EnumPostHeader.InteractionTimeStamp + ",");
            sbH.Append(Enums.EnumPostHeader.ActivityType);
            if (addRepeatedWatchCount)
                sbH.Append("," + Enums.EnumPostHeader.RepeatedWatchCount);
            if (addInteractedCommentUrl)
                sbH.Append("," + Enums.EnumPostHeader.InteractedCommentUrl);

            return sbH.ToString();
        }

        public string HeaderStringForCommentsReport()
        {
            var sbH = new StringBuilder();
            sbH.Append(Enums.EnumPostHeader.Id + ",");
            sbH.Append(Enums.EnumPostHeader.AccountUsername + ",");
            sbH.Append(Enums.EnumChannelHeader.AccountChannelId + ",");
            sbH.Append(Enums.EnumPostHeader.QueryType + ",");
            sbH.Append(Enums.EnumPostHeader.QueryValue + ",");
            sbH.Append(Enums.EnumPostHeader.VideoUrl + ",");
            sbH.Append(Enums.EnumPostHeader.VideoTitle + ",");
            sbH.Append(Enums.EnumCommentsHeader.CommentText + ","); // Liked Comment Text
            sbH.Append(Enums.EnumCommentsHeader.CommentId + ",");
            sbH.Append(Enums.EnumCommentsHeader.ReactionOnComment + ",");
            sbH.Append(Enums.EnumCommentsHeader.CommentPostedTime + ",");
            sbH.Append(Enums.EnumCommentsHeader.CommenterChannelName + ",");
            sbH.Append(Enums.EnumCommentsHeader.CommenterChannelId + ",");
            sbH.Append(Enums.EnumCommentsHeader.CommentLikesCount + ",");
            sbH.Append(Enums.EnumPostHeader.ActivityType + ",");
            sbH.Append(Enums.EnumPostHeader.InteractionTimeStamp + ",");
            sbH.Append(Enums.EnumPostHeader.ViewsCount + ",");
            sbH.Append(Enums.EnumPostHeader.CommentCount + ",");
            sbH.Append(Enums.EnumPostHeader.VideoDurationInHourMinSec);
            sbH.Append(Enums.EnumPostHeader.PostDescription);
            sbH.Append(Enums.EnumPostHeader.InteractedCommentUrl);

            return sbH.ToString();
        }

        public string CreateCsvDataForChannel(InteractedChannelsReport report, bool addInteractedCommentUrl = false)
        {
            try
            {
                return
                    $"{report.Id},{report.AccountUsername},{report.AccountChannelId},{report.QueryType},{report.QueryValue.AsPerCsv()}{report.InteractedChannelName.AsPerCsv()}" +
                    $"{report.InteractedChannelId},{report.ChannelUrl},{report.ChannelDescription.AsPerCsv()}{report.ChannelJoinedDate.AsPerCsv()}" +
                    $"{report.ChannelLocation.AsPerCsv()}{report.ChannelProfilePic},{report.SubscriberCount},{report.ViewsCount}," +
                    $"{report.VideosCount},"
                    + (report.IsSubscribed ? "Yes," : "No,") +
                    $"{report.InteractionTime.AsPerCsv()}{report.ActivityType},{report.ExternalLinks.TrimEnd(',').AsPerCsv(true)}"
                    + (addInteractedCommentUrl ? $",{report.InteractedCommentUrl}" : "");
            }
            catch (Exception)
            {
                return "";
            }
        }

        public string CreateCsvDataForPost(InteractedPostsReport report,
            bool addCommentedText = false, bool addRepeatedWatchCount = false, bool addCommentId = false,
            bool addInteractedCommentUrl = false, bool addReportedToVideoDetails = false)
        {
            try
            {
                var addUrl = addCommentId ? $"&lc={report.CommentId}" : "";
                return
                    $"{report.Id},{report.AccountUsername},{report.AccountChannelId},{report.QueryType},{report.QueryValue.AsPerCsv()}{report.VideoUrl}{addUrl},{report.VideoTitle.AsPerCsv()}" +
                    (addReportedToVideoDetails
                        ? $"{report.SelectedOptionToVideoReport.AsPerCsv()}{report.MyCommentedText.AsPerCsv()}{report.SelectedTimeStampToVideoReport},"
                        : "") +
                    (addCommentedText ? $"{report.MyCommentedText.AsPerCsv()}" : "") +
                    (addCommentId ? $"{report.CommentId}," : "") +
                    $"{report.Reaction.ToString().AsPerCsv()}{report.ViewsCount.AsPerCsv()}{report.LikeCount.AsPerCsv()}{report.DislikeCount.AsPerCsv()}{report.CommentCount.AsPerCsv()}" +
                    $"{report.VideoDuration},{report.PostDescription.AsPerCsv()}{report.PublishedDate.AsPerCsv()}" +
                    $"{report.ChannelName.AsPerCsv()}{report.ChannelId.AsPerCsv()}" +
                    (report.IsSubscribed ? "Yes," : "No,") +
                    $"{report.SubscribeCount.AsPerCsv()}{report.InteractionTime.AsPerCsv()}{report.ActivityType}" +
                    (addRepeatedWatchCount ? $",{report.RepeatedWatchCount}" : "")
                    + (addInteractedCommentUrl ? $",{report.InteractedCommentUrl}" : "");
            }
            catch (Exception)
            {
                return "";
            }
        }

        public string CreateCsvDataForCommentsOfPost(InteractedPostsReport report)
        {
            try
            {
                var addUrl = $"&lc={report.CommentId}";
                return
                    $"{report.Id},{report.AccountUsername},{report.AccountChannelId},{report.QueryType},{report.QueryValue.AsPerCsv()}{report.VideoUrl}{addUrl},{report.VideoTitle.AsPerCsv()}" +
                    $"{report.MyCommentedText.AsPerCsv()}{report.CommentId},{report.Reaction.ToString().AsPerCsv()}{report.PublishedDate.AsPerCsv()}" +
                    $"{report.ChannelName.AsPerCsv()}{report.ChannelId.AsPerCsv()}{report.LikeCount.AsPerCsv()}{report.ActivityType}," +
                    $"{report.InteractionTime.AsPerCsv()}{report.ViewsCount.AsPerCsv()}{report.CommentCount.AsPerCsv()}" +
                    $"{report.VideoDuration},{report.PostDescription.AsCsvData()}{report.InteractedCommentUrl}";
            }
            catch (Exception)
            {
                return "";
            }
        }

        public List<DominatorAccountModel> GetYdSuccessAccounts()
        {
            return InstanceProvider.GetInstance<IDominatorAccountViewModel>().LstDominatorAccountModel.Where(x =>
                x.AccountBaseModel.Status == AccountStatus.Success &&
                x.AccountBaseModel.AccountNetwork == SocialNetworks.YouTube).ToList();
        }

        public List<string> SelectedAccountsName(
            ObservableCollection<ChannelDestinationSelectModel> listSelectDestination)
        {
            try
            {
                if (listSelectDestination.Count > 0)
                {
                    var allAccounts = GetYdSuccessAccounts();
                    var selectedAccounts = listSelectDestination
                        .Where(x => x.IsAccountSelected && allAccounts.Any(z =>
                                        z.AccountBaseModel.UserName == x.AccountName && z.AccountId == x.AccountId))
                        .Select(y => y.AccountName).ToList();
                    return selectedAccounts;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return new List<string>();
        }

        public void GoInsideListMakeString(ref StringBuilder sb, object obj, string startingObjName)
        {
            var listProperties = obj.GetType().GetProperties();

            foreach (var element in listProperties)
            {
                var startingObjStringName = startingObjName;
                try
                {
                    var elemName = element.PropertyType.Name;
                    if (elemName == "int" || elemName == "Boolean")
                    {
                        var value = elemName == "Boolean"
                            ? element.GetValue(obj, null).ToString().ToLower()
                            : element.GetValue(obj, null).ToString();
                        if (value != null)
                            sb.AppendLine(GetString(element.Name, value, startingObjStringName, true));
                    }
                    else if (elemName == "String")
                    {
                        var value = element.GetValue(obj, null);
                        if (value != null)
                        {
                            var stringValue = value.ToString();
                            if (stringValue.Contains("\""))
                                stringValue = stringValue.Replace("\"", "\\\"");
                            if (stringValue.Contains("\n"))
                                stringValue = stringValue.Replace("\n", "\\n");
                            if (stringValue.Contains("\r\n"))
                                stringValue = stringValue.Replace("\r\n", "\\r\\n");

                            sb.AppendLine(GetString(element.Name, stringValue, startingObjStringName));
                        }
                    }
                    else if (elemName.Contains("List"))
                    {
                        var value = element.GetValue(obj, new object[0]);
                        startingObjStringName = startingObjName + $".{element.Name}";
                        if (value != null)
                            sb.AppendLine(GetStringFromListObj(value, startingObjStringName, 1));
                    }
                    else
                    {
                        var value = element.GetValue(obj, new object[0]);
                        startingObjStringName = startingObjName + $".{element.Name}"; //value.GetType().Name
                        if (value != null)
                            GoInsideListMakeString(ref sb, value, startingObjStringName);
                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }
        }

        public string GetStringFromListObj(object obj, string startingObjStringName, int numberOfDataFromList = 0)
        {
            try
            {
                var sb = new StringBuilder();
                var value = (IList)obj;
                var iteration = 0;
                if (numberOfDataFromList == 0)
                    numberOfDataFromList = value.Count;

                foreach (var each in value)
                {
                    GoInsideListMakeString(ref sb, each, $"{startingObjStringName}[{iteration++}]");
                    if (iteration >= numberOfDataFromList) break;
                }

                return sb.ToString();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return "";
            }
        }

        public string GetString(string name, string value, string startingObjStringName, bool isInt = false)
        {
            value = isInt ? value : $"\"{value}\"";
            return $"\n{startingObjStringName}.{name}.Should().Be({value});";
        }

        public static string[] CommonPageSplitter(string response)
        {
            var scriptParts = Regex.Split(response, "<script >").Skip(1).ToArray();
            if (scriptParts.Length == 0)
                scriptParts = Regex.Split(response, "<script>").Skip(1).ToArray();
            if (scriptParts.Length < 4)
            {
                var removeWithIt = Utilities.GetBetween(response, "<script nonce=\"", "\">");
                var splitWith = "<script nonce=\"\">";
                var responseSplitIn =
                    string.IsNullOrEmpty(removeWithIt) ? response : response.Replace(removeWithIt, "");
                scriptParts = Regex.Split(responseSplitIn, splitWith).Skip(1).ToArray();
            }

            return scriptParts;
        }

        public static string GetDownloadedMediaPath()
        {
            var folderPath = "";
            try
            {
                RemovePreviousDirectory();
                folderPath = $@"{ConstantVariable.MediaTempFolder}\[{DateTime.Now:MM-dd-yyyy}]";// $@"{documentsPath}\{ConstantVariable.ApplicationName}\CaptchaImages\{todaydate.ToString()}";
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return folderPath;
        }
        private static void RemovePreviousDirectory()
        {
            try
            {
                var lstTemprorayFolderList = DirectoryUtilities.GetSubDirectories(ConstantVariable.MediaTempFolder);

                lstTemprorayFolderList.RemoveAll(x => x.Contains($@"[{DateTime.Now:MM-dd-yyyy}]") ||
                                                      x.Contains(
                                                          $@"[{DateTime.Now.AddDays(-1):MM-dd-yyyy}]"));

                DirectoryUtilities.DeleteFolder(lstTemprorayFolderList, true);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

    }
}