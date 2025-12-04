using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using YoutubeDominatorCore.YDUtility;
using YoutubeDominatorCore.YoutubeModels;

namespace YoutubeDominatorCore.Response
{
    public class SearchChannelsResponseHandler : YdResponseHandler
    {
        public SearchChannelsResponseHandler()
        {
        }

        public SearchChannelsResponseHandler(IResponseParameter response, bool needHeaderElements)
        {
            try
            {
                JToken listChannelsJToken;
                if (needHeaderElements)
                {
                    var scriptParts = YoutubeUtilities.CommonPageSplitter(response.Response);

                    var jsonString = Utilities
                        .GetBetween(scriptParts.FirstOrDefault(x => x.Trim().StartsWith("window[\"ytInitialData\"]")),
                            "=", "window[").Trim().TrimEnd(';');
                    if (string.IsNullOrEmpty(jsonString))
                        jsonString = Utilities
                            .GetBetween(scriptParts.FirstOrDefault(x => x.Trim().StartsWith("var ytInitialData")), "=",
                                "</script>").Trim().TrimEnd(';');

                    YdStatic.ExtractCommonJsonData(ref jsonString);
                    var jsonHand = new JsonHandler(jsonString);

                    var commonJToken = jsonHand.GetJToken("contents", "twoColumnSearchResultsRenderer",
                        "primaryContents", "sectionListRenderer", "contents", 0, "itemSectionRenderer");

                    PostDataElements = new PostDataElements
                    {
                        Itct = jsonHand.GetJTokenValue(commonJToken, "continuations", 0, "nextContinuationData",
                            "clickTrackingParams"),
                        ContinuationToken = jsonHand.GetJTokenValue(commonJToken, "continuations", 0,
                            "nextContinuationData", "continuation")
                    };

                    listChannelsJToken = jsonHand.GetJTokenOfJToken(commonJToken, "contents");

                    jsonString = Utilities
                        .GetBetween(
                            scriptParts.FirstOrDefault(x =>
                                x.Trim().StartsWith(
                                    "if (window.ytcsi) {window.ytcsi.tick(\"lpcs\", null, '');}window.Polymer =")),
                            "function() {window.ytplayer = {};ytcfg.set(", ");ytcfg.set(").Trim().TrimEnd(';')
                        .TrimEnd(')');
                    if (string.IsNullOrEmpty(jsonString))
                        jsonString = Utilities
                            .GetBetween(
                                scriptParts.FirstOrDefault(x =>
                                    x.Trim().StartsWith(
                                        "if (window.ytcsi) {window.ytcsi.tick(\"lpcs\", null, '');}(function() {window.ytplayer = {};ytcfg.set(")),
                                "function() {window.ytplayer = {};ytcfg.set(", ");ytcfg.set(").Trim().TrimEnd(';')
                            .TrimEnd(')');
                    if (string.IsNullOrEmpty(jsonString))
                        jsonString = Utilities
                            .GetBetween(
                                scriptParts.FirstOrDefault(x =>
                                    x.Trim().StartsWith("(function() {window.ytplayer={};\nytcfg.set(")),
                                "function() {window.ytplayer={};\nytcfg.set(", ");var").Trim().TrimEnd(';')
                            .TrimEnd(')');

                    jsonHand = new JsonHandler(jsonString);

                    PostDataElements.XsrfToken = Uri.EscapeDataString(jsonHand.GetElementValue("XSRF_TOKEN"));

                    HeadersElements = new HeadersElements
                    {
                        PageCl = jsonHand.GetElementValue("PAGE_CL"),
                        PageBuildLabel = jsonHand.GetElementValue("PAGE_BUILD_LABEL"),
                        VariantsChecksum = jsonHand.GetElementValue("VARIANTS_CHECKSUM"),
                        IdToken = jsonHand.GetElementValue("ID_TOKEN"),
                        ClientVersion = jsonHand.GetElementValue("INNERTUBE_CONTEXT_CLIENT_VERSION"),
                        ClientName = jsonHand.GetElementValue("INNERTUBE_CONTEXT_CLIENT_NAME")
                    };

                    #region No need of this, Currently. (Commented code)

                    // jsonString = Utilities.GetBetween(scriptParts.FirstOrDefault(x => x.Trim().StartsWith("(function() {\n      var setFiller = function()")), "if (window.ytcsi) {window.ytcsi.tick(\"pr\", null, '');}\n          var endpoint = null;\n            endpoint =", "var data = {").Trim().TrimEnd(';');
                    // jsonHand = new JsonHandler(jsonString);
                    //PostDataElements.AddInUrl = Uri.EscapeDataString(jsonHand.GetElementValue("urlEndpoint", "url"));

                    #endregion
                }
                else
                {
                    var jsonHand = new JsonHandler("{\"JsonData\":" + response.Response + "}");
                    listChannelsJToken = jsonHand.GetJToken("JsonData", 1, "response", "continuationContents",
                        "itemSectionContinuation", "contents");
                    PostDataElements = new PostDataElements
                    {
                        ContinuationToken = jsonHand.GetElementValue("JsonData", 1, "response", "continuationContents",
                            "itemSectionContinuation", "continuations", 0, "nextContinuationData", "continuation")
                    };
                }

                ListOfYoutubeChannels = ScrapChannelsFromJTokenObj(listChannelsJToken);

                Success = true;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public List<YoutubeChannel> ListOfYoutubeChannels { get; set; }
        public HeadersElements HeadersElements { get; set; }
        public PostDataElements PostDataElements { get; set; }

        private List<YoutubeChannel> ScrapChannelsFromJTokenObj(JToken channelsJToken)
        {
            var listYoutubeChannel = new List<YoutubeChannel>();
            try
            {
                foreach (var item in channelsJToken)
                {
                    var youtubeChannel = new YoutubeChannel();
                    var jsonHand = new JsonHandler(item);

                    youtubeChannel.ChannelId = jsonHand.GetElementValue("channelRenderer", "channelId");
                    if (string.IsNullOrEmpty(youtubeChannel.ChannelId))
                        youtubeChannel.ChannelId = jsonHand.GetElementValue("searchPyvRenderer", "ads", 0,
                            "promotedVideoRenderer", "longBylineText", "runs", 0, "navigationEndpoint",
                            "browseEndpoint", "browseId");
                    if (string.IsNullOrEmpty(youtubeChannel.ChannelId))
                        youtubeChannel.ChannelId = jsonHand.GetElementValue("searchPyvRenderer", "ads", 0,
                            "promotedVideoRenderer", "shortBylineText", "runs", 0, "navigationEndpoint",
                            "browseEndpoint", "browseId");

                    youtubeChannel.ChannelUsername = jsonHand
                        .GetElementValue("channelRenderer", "navigationEndpoint", "browseEndpoint", "canonicalBaseUrl")
                        .Replace("/user/", "");

                    if (string.IsNullOrEmpty(youtubeChannel.ChannelId.Trim()) &&
                        string.IsNullOrEmpty(youtubeChannel.ChannelUsername.Trim()))
                        continue;

                    if (!string.IsNullOrEmpty(youtubeChannel.ChannelId.Trim()))
                        youtubeChannel.ChannelUrl = $"https://www.youtube.com/channel/{youtubeChannel.ChannelId}";
                    else if (!string.IsNullOrEmpty(youtubeChannel.ChannelUsername.Trim()))
                        youtubeChannel.ChannelUrl = $"https://www.youtube.com/user/{youtubeChannel.ChannelUsername}";

                    youtubeChannel.ChannelName = jsonHand.GetElementValue("channelRenderer", "title", "simpleText");
                    if (string.IsNullOrEmpty(youtubeChannel.ChannelName))
                        youtubeChannel.ChannelName = jsonHand.GetElementValue("searchPyvRenderer", "ads", 0,
                            "promotedVideoRenderer", "longBylineText", "runs", 0, "text");
                    if (string.IsNullOrEmpty(youtubeChannel.ChannelName))
                        youtubeChannel.ChannelName = jsonHand.GetElementValue("searchPyvRenderer", "ads", 0,
                            "promotedVideoRenderer", "shortBylineText", "runs", 0, "text");

                    youtubeChannel.ProfilePicUrl =
                        jsonHand.GetElementValue("channelRenderer", "thumbnail", "thumbnails", 0, "url");
                    if (string.IsNullOrEmpty(youtubeChannel.ProfilePicUrl))
                        youtubeChannel.ProfilePicUrl = jsonHand.GetElementValue("searchPyvRenderer", "ads", 0,
                            "promotedVideoRenderer", "richThumbnail", "movingThumbnailRenderer",
                            "movingThumbnailDetails", "thumbnails", 0, "url");
                    if (string.IsNullOrEmpty(youtubeChannel.ProfilePicUrl))
                        youtubeChannel.ProfilePicUrl = jsonHand.GetElementValue("searchPyvRenderer", "ads", 0,
                            "promotedVideoRenderer", "thumbnail", "thumbnails", 0, "url");

                    var descJToken = jsonHand.GetJToken("channelRenderer", "descriptionSnippet", "runs");
                    foreach (var itemDesc in descJToken)
                        youtubeChannel.ChannelDescription += jsonHand.GetJTokenValue(itemDesc, "text");

                    if (string.IsNullOrEmpty(youtubeChannel.ChannelDescription))
                        youtubeChannel.ChannelDescription =
                            jsonHand.GetElementValue("channelRenderer", "descriptionSnippet", "simpleText");

                    youtubeChannel.VideosCount =
                        jsonHand.GetElementValue("channelRenderer", "videoCountText", "runs", 0, "text");
                    if (string.IsNullOrEmpty(youtubeChannel.VideosCount))
                        youtubeChannel.VideosCount =
                            jsonHand.GetElementValue("channelRenderer", "videoCountText", "simpleText");
                    youtubeChannel.VideosCount = youtubeChannel.VideosCount.RemoveAllExceptWholeNumb();

                    youtubeChannel.SubscriberCount = jsonHand
                        .GetElementValue("channelRenderer", "subscriberCountText", "simpleText")
                        .RemoveAllExceptWholeNumb();
                    if (string.IsNullOrEmpty(youtubeChannel.ChannelId))
                        youtubeChannel.ChannelId = jsonHand.GetElementValue("searchPyvRenderer", "ads", 0,
                            "promotedVideoRenderer", "longBylineText", "runs", 0, "navigationEndpoint",
                            "browseEndpoint", "browseId");
                    if (string.IsNullOrEmpty(youtubeChannel.ChannelId))
                        youtubeChannel.ChannelId = jsonHand.GetElementValue("searchPyvRenderer", "ads", 0,
                            "promotedVideoRenderer", "shortBylineText", "runs", 0, "navigationEndpoint",
                            "browseEndpoint", "browseId");

                    youtubeChannel.IsSubscribed =
                        jsonHand.GetElementValue("channelRenderer", "subscriptionButton", "subscribed")
                            .Equals("true", StringComparison.CurrentCultureIgnoreCase);

                    listYoutubeChannel.Add(youtubeChannel);
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return listYoutubeChannel;
        }
    }
}