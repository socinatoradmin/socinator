using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using YoutubeDominatorCore.YDUtility;
using YoutubeDominatorCore.YoutubeModels;

namespace YoutubeDominatorCore.Response
{
    public class ChannelInfoResponseHandler : YdResponseHandler
    {
        public ChannelInfoResponseHandler()
        {
        }

        public ChannelInfoResponseHandler(IResponseParameter responsePara, bool getChannelDetails,
            bool getNecessaryElements)
        {
            var scriptParts = YoutubeUtilities.CommonPageSplitter(responsePara.Response);
            try
            {
                JObject jobject = new JObject();
                var jSonString = Utilities
                    .GetBetween(scriptParts.FirstOrDefault(x => x.Trim().StartsWith("var ytInitialData = ")),
                        "=", ";</script>").Trim().TrimEnd(';');
                YdStatic.ExtractCommonJsonData(ref jSonString);
                if (jSonString.IsValidJson())
                    jobject = handler.ParseJsonToJsonObject(jSonString);

                var aboutChannelJtoken = JsonSearcher.FindByKey(JsonSearcher.FindByKey(jobject, "aboutChannelRenderer"), "aboutChannelViewModel");
                if (aboutChannelJtoken.Count() == 0)
                    aboutChannelJtoken = JsonSearcher.FindByKey(jobject, "aboutChannelViewModel");
                var channelID = handler.GetJTokenValue(aboutChannelJtoken, "channelId");
                if (string.IsNullOrEmpty(channelID))
                    channelID = JsonSearcher.FindStringValueByKey(jobject, "channelId");
                var country = handler.GetJTokenValue(aboutChannelJtoken, "country");

                var viewsCount = handler.GetJTokenValue(aboutChannelJtoken, "viewCountText");
                viewsCount = YoutubeUtilities.GetIntegerOnlyString(viewsCount);

                var vediosCount = handler.GetJTokenValue(aboutChannelJtoken, "videoCountText");
                vediosCount = YoutubeUtilities.GetIntegerOnlyString(vediosCount);

                var SubScriberCount = handler.GetJTokenValue(aboutChannelJtoken, "subscriberCountText").Replace(" subscribers", "");
                SubScriberCount = YoutubeUtilities.YoutubeElementsCountInNumber(SubScriberCount);

                var joinedDate = handler.GetJTokenValue(aboutChannelJtoken, "joinedDateText", "content").Replace("Joined ", "");
                var linksJToken = handler.GetJTokenOfJToken(aboutChannelJtoken, "links");
                if (!linksJToken.HasValues)
                {
                    var innerJToken = handler.GetJTokenOfJToken(jobject, "sectionListRenderer",
                     "contents", 0, "itemSectionRenderer", "contents", 0,
                     "channelVideoPlayerRenderer");
                    linksJToken = handler.GetJTokenOfJToken(innerJToken, "description", "runs");
                }
                var externalLinks = "";
                foreach (var linkJToken in linksJToken)
                {
                    var url = "https://www." + WebUtility.UrlDecode(handler.GetJTokenValue(linkJToken, "channelExternalLinkViewModel",
                         "link", "content"));
                    if (string.IsNullOrEmpty(url))
                        url = WebUtility.UrlDecode(JsonSearcher.FindStringValueByKey(JsonSearcher.FindByKey(linkJToken, "webCommandMetadata"), "url"));
                    if (string.IsNullOrEmpty(url))
                        url = WebUtility.UrlDecode(handler.GetJTokenValue(linkJToken, "text"));
                    if (!string.IsNullOrEmpty(url) && url.StartsWith("https://"))
                        externalLinks += url;
                    if (linksJToken.Count() > 1)
                        externalLinks += "\n";
                }

                var HeaderTabJtoken = handler.GetJTokenOfJToken(jobject, "header", "c4TabbedHeaderRenderer");
                if (HeaderTabJtoken.Count() == 0)
                    HeaderTabJtoken = JsonSearcher.FindByKey(jobject, "c4TabbedHeaderRenderer");
                if (HeaderTabJtoken.Count() == 0)
                    HeaderTabJtoken = JsonSearcher.FindByKey(jobject, "pageHeaderRenderer");
                var channelName = handler.GetJTokenValue(HeaderTabJtoken, "title");
                if (string.IsNullOrEmpty(channelName))
                    channelName = handler.GetJTokenValue(HeaderTabJtoken, "pageTitle");
                var isSubscribed = handler.GetJTokenValue(HeaderTabJtoken, "frameworkUpdates", "subscriptionStateEntity", "subscribed");
                if (string.IsNullOrEmpty(isSubscribed))
                    isSubscribed = handler.GetJTokenValue(JsonSearcher.FindByKey(HeaderTabJtoken, "unsubscribeButtonContent"), "subscribeState", "subscribed");
                if (string.IsNullOrEmpty(isSubscribed))
                    isSubscribed = JsonSearcher.FindStringValueByKey(JsonSearcher.FindByKey(JsonSearcher.FindByKey(jobject, "frameworkUpdates"), "subscriptionStateEntity"), "subscribed");
                if (string.IsNullOrEmpty(isSubscribed))
                    isSubscribed = JsonSearcher.FindStringValueByKey(JsonSearcher.FindByKey(jobject, "subscriptionStateEntity"), "subscribed");
                var channelUserName = handler.GetJTokenValue(HeaderTabJtoken, "channelHandleText", "runs", 0, "text");
                if (string.IsNullOrEmpty(channelUserName))
                    channelUserName = JsonSearcher.FindStringValueByKey(JsonSearcher.FindByKey(HeaderTabJtoken, "contentMetadataViewModel"), "text");
                if (string.IsNullOrEmpty(channelUserName))
                    channelUserName = handler.GetJTokenValue(HeaderTabJtoken, "navigationEndpoint", "commandMetadata", "webCommandMetadata", "url")
                     .Replace("/", "").Trim();

                var MetaDataJtoken = JsonSearcher.FindByKey(JsonSearcher.FindByKey(jobject, "metadata"), "channelMetadataRenderer");
                if (MetaDataJtoken.Count() == 0)
                    MetaDataJtoken = JsonSearcher.FindByKey(jobject, "channelMetadataRenderer");
                if (string.IsNullOrEmpty(channelName))
                    channelName = handler.GetJTokenValue(MetaDataJtoken, "title");
                var channelUrl = handler.GetJTokenValue(MetaDataJtoken, "channelUrl");
                if (string.IsNullOrEmpty(channelUrl) && !string.IsNullOrEmpty(channelID))
                    channelUrl = $"https://www.youtube.com/channel/{channelID}";
                if (string.IsNullOrEmpty(channelUserName))
                    channelUserName = handler.GetJTokenValue(MetaDataJtoken, "ownerUrls", 0).Split('/').LastOrDefault() ?? "";
                var description = handler.GetJTokenValue(MetaDataJtoken, "description").Replace("\n", "");
                var profileUrl = handler.GetJTokenValue(MetaDataJtoken, "avatar", "thumbnails", 0, "url");


                YoutubeChannel = new YoutubeChannel
                {
                    ChannelId = channelID,
                    ChannelName = channelName,
                    IsSubscribed = isSubscribed.ToLower().Equals("true", StringComparison.CurrentCultureIgnoreCase),
                    ChannelUsername = channelUserName,
                    ChannelLocation = country,
                    ViewsCount = viewsCount,
                    VideosCount = vediosCount,
                    ChannelJoinedDate = joinedDate,
                    ChannelUrl = channelUrl,
                    ProfilePicUrl = profileUrl,
                    SubscriberCount = SubScriberCount,
                    HasChannelUsername = !string.IsNullOrEmpty(channelUserName),
                    ExternalLinks = externalLinks,
                    ChannelDescription = description
                };
                if (string.IsNullOrEmpty(YoutubeChannel.SubscriberCount))
                {
                    var Nodes = HtmlParseUtility.GetListNodesFromClassName(responsePara.Response, "description-item style-scope ytd-about-channel-renderer");
                    if (Nodes != null && Nodes.Count > 0)
                    {
                        try
                        {
                            var Node = Nodes.FirstOrDefault(x => !string.IsNullOrEmpty(x.InnerText) && (x.InnerText.Contains("videos") || x.InnerText.Contains("video")));
                            if (Node != null && !string.IsNullOrEmpty(Node.InnerText))
                                YoutubeChannel.VideosCount = Regex.Replace(Node.InnerText, "[^0-9]+", "");
                            Node = Nodes.FirstOrDefault(x => !string.IsNullOrEmpty(x.InnerText) && (x.InnerText.Contains("subscribers") || x.InnerText.Contains("subscriber")));
                            if (Node != null && !string.IsNullOrEmpty(Node.InnerText))
                                YoutubeChannel.SubscriberCount = Regex.Replace(Node.InnerText, "[^0-9]+", "");
                            Node = Nodes.FirstOrDefault(x => !string.IsNullOrEmpty(x.InnerText) && (x.InnerText.Contains("views") || x.InnerText.Contains("view")));
                            if (Node != null && !string.IsNullOrEmpty(Node.InnerText))
                                YoutubeChannel.ViewsCount = Regex.Replace(Node.InnerText, "[^0-9]+", "");
                            YoutubeChannel.ChannelId = Utilities.GetBetween(responsePara.Response, "\"channelId\":\"", "\"");
                        }
                        catch { }
                    }
                }
                Success = true;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            try
            {
                if (getNecessaryElements)
                {
                    var jSonString2 = Utilities
                    .GetBetween(
                        scriptParts.FirstOrDefault(x =>
                            x.Trim().StartsWith(
                                "if (window.ytcsi) {window.ytcsi.tick(\"lpcs\", null, '');}window.Polymer = ")),
                        "(function() {window.ytplayer = {};ytcfg.set(", ");ytcfg.set(").Trim().TrimEnd(';');
                    if (string.IsNullOrEmpty(jSonString2))
                        jSonString2 = Utilities
                            .GetBetween(
                                scriptParts.FirstOrDefault(x =>
                                    x.Trim().StartsWith(
                                        "if (window.ytcsi) {window.ytcsi.tick(\"lpcs\", null, '');}(function() {window.ytplayer = {};ytcfg.set(")),
                                "function() {window.ytplayer = {};ytcfg.set(", ");ytcfg.set(").Trim().TrimEnd(';')
                            .TrimEnd(')');
                    if (string.IsNullOrEmpty(jSonString2))
                        jSonString2 = Utilities
                            .GetBetween(
                                scriptParts.FirstOrDefault(x =>
                                    x.Trim().StartsWith(
                                        "(function() {window.ytplayer={};\nytcfg.set(")), "(function() {window.ytplayer={};\nytcfg.set(",
                                    ");");
                    var jsonHand2 = new JsonHandler("{}");
                    if (jSonString2.IsValidJson())
                        jsonHand2 = new JsonHandler(jSonString2);

                    CollectNecessaryElements(handler, jsonHand2);
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public YoutubeChannel YoutubeChannel { get; set; }

        private void CollectChannelDetails(JsonHandler jsonHand, JToken jToken1)
        {
            if (jToken1.Any())
            {
                YoutubeChannel.ChannelName = jsonHand.GetJTokenValue(jToken1, "title");
                if (string.IsNullOrEmpty(YoutubeChannel.ChannelName)) jsonHand.GetElementValue("metadata", "channelMetadataRenderer", "title");

                YoutubeChannel.ProfilePicUrl = jsonHand.GetJTokenValue(jToken1, "avatar", "thumbnails", 0, "url");

                var subscribed = jsonHand
                    .GetJTokenValue(jToken1, "subscribeButton", "subscribeButtonRenderer", "subscribed");
                if (string.IsNullOrEmpty(subscribed)) subscribed = jsonHand
                     .GetElementValue("frameworkUpdates", "entityBatchUpdate", "mutations", 0, "payload", "subscriptionStateEntity", "subscribed");
                YoutubeChannel.IsSubscribed = subscribed.ToLower().Equals("true", StringComparison.CurrentCultureIgnoreCase);
                if (string.IsNullOrEmpty(YoutubeChannel.VideosCount))
                    YoutubeChannel.VideosCount = jsonHand.GetJTokenValue(jToken1, "videosCountText", "runs", 0, "text");
                YoutubeChannel.SubscriberCount =
                    jsonHand.GetJTokenValue(jToken1, "subscriberCountText", "runs", 0, "text");
                if (string.IsNullOrEmpty(YoutubeChannel.SubscriberCount))
                    YoutubeChannel.SubscriberCount =
                        jsonHand.GetJTokenValue(jToken1, "subscriberCountText", "simpleText");
                if (string.IsNullOrEmpty(YoutubeChannel.SubscriberCount))
                    YoutubeChannel.SubscriberCount =
                        jsonHand.GetElementValue("onResponseReceivedEndpoints", 0, "showEngagementPanelEndpoint", "engagementPanel", "engagementPanelSectionListRenderer", "content", "sectionListRenderer", "contents",
                        0, "itemSectionRenderer", "contents", 0, "aboutChannelRenderer", "metadata", "aboutChannelViewModel", "subscriberCountText");

                YoutubeChannel.SubscriberCount =
                    YoutubeUtilities.YoutubeElementsCountInNumber(YoutubeChannel.SubscriberCount);
                YoutubeChannel.VideosCount =
                    YoutubeUtilities.YoutubeElementsCountInNumber(YoutubeChannel.VideosCount);

            }

            var jToken2 = jsonHand.GetJToken("contents", "twoColumnBrowseResultsRenderer", "tabs");
            if (jToken2.Any())
                foreach (var jToken in jToken2)
                {
                    var internalJToken = jsonHand.GetJTokenOfJToken(jToken, "tabRenderer", "content");
                    if (internalJToken == null || !internalJToken.Any()) continue;

                    var innerJToken = jsonHand.GetJTokenOfJToken(internalJToken, "sectionListRenderer",
                        "contents", 0, "itemSectionRenderer", "contents", 0,
                        "channelAboutFullMetadataRenderer");
                    //YoutubeChannel.ChannelJoinedDate = Utilities.GetBetween(jsonHand.GetJTokenValue(innerJToken, "joinedDateText", "simpleText") + "#", "Joined", "#");
                    YoutubeChannel.ChannelJoinedDate =
                        jsonHand.GetJTokenValue(innerJToken, "joinedDateText", "runs", 1, "text");
                    var views = jsonHand.GetJTokenValue(innerJToken, "viewCountText", "runs", 0, "text");
                    if (string.IsNullOrEmpty(views))
                        views = jsonHand.GetJTokenValue(innerJToken, "viewCountText", "simpleText");
                    if (string.IsNullOrEmpty(views))
                        views = jsonHand.GetJTokenValue(innerJToken, "viewCountText", "simpleText");
                    YoutubeChannel.ViewsCount = string.IsNullOrEmpty(YoutubeChannel.ViewsCount) ? views.RemoveAllExceptWholeNumb() : YoutubeChannel.ViewsCount;

                    YoutubeChannel.ViewsCount = string.IsNullOrEmpty(YoutubeChannel.ViewsCount)
                        ? "0"
                        : YoutubeChannel.ViewsCount;

                    YoutubeChannel.ChannelLocation = jsonHand.GetJTokenValue(innerJToken, "country", "simpleText");

                    YoutubeChannel.ChannelDescription =
                        jsonHand.GetJTokenValue(innerJToken, "description", "simpleText").Replace("\n", "");


                    break;
                }

        }

        private void CollectNecessaryElements(JsonHandler jsonHand1, JsonHandler jsonHand2)
        {
            YoutubeChannel.PostDataElements = new PostDataElements
            {
                XsrfToken = jsonHand2.GetElementValue("XSRF_TOKEN"),

                TrackingParams = handler.GetElementValue("contents", "twoColumnBrowseResultsRenderer",
                    "secondaryContents", "browseSecondaryContentsRenderer", "contents", 0,
                    "verticalChannelSectionRenderer", "items", 1, "miniChannelRenderer", "subscribeButton",
                    "subscribeButtonRenderer", "trackingParams"),
                Csn = handler.GetElementValue("responseContext", "webResponseContextExtensionData", "ytConfigData",
                    "csn"),

                ContinuationToken = handler.GetElementValue("contents", "twoColumnWatchNextResults", "results",
                    "results", "contents", 2, "itemSectionRenderer", "continuations", 0, "nextContinuationData",
                    "continuation"),

                Params = handler.GetElementValue("contents", "twoColumnBrowseResultsRenderer", "secondaryContents",
                    "browseSecondaryContentsRenderer", "contents", 0, "verticalChannelSectionRenderer", "items", 1,
                    "miniChannelRenderer", "subscribeButton", "subscribeButtonRenderer", "onSubscribeEndpoints", 0,
                    "subscribeEndpoint", "params")
            };

            YoutubeChannel.HeadersElements = new HeadersElements
            {
                PageBuildLabel = jsonHand2.GetElementValue("PAGE_BUILD_LABEL"),
                VariantsChecksum = jsonHand2.GetElementValue("VARIANTS_CHECKSUM"),
                PageCl = jsonHand2.GetElementValue("PAGE_CL"),
                ClientName = jsonHand2.GetElementValue("INNERTUBE_CONTEXT_CLIENT_NAME"),
                ClientVersion = jsonHand2.GetElementValue("INNERTUBE_CONTEXT_CLIENT_VERSION"),
                RefererUrl = YoutubeChannel.ChannelUrl,
                IdToken = jsonHand2.GetElementValue("ID_TOKEN")
            };
        }
    }
}