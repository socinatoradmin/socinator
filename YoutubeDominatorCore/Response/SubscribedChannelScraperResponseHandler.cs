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
    public class SubscribedChannelScraperResponseHandler : YdResponseHandler
    {
        public SubscribedChannelScraperResponseHandler()
        {
        }

        public SubscribedChannelScraperResponseHandler(IResponseParameter response, bool needHeaderElements = true)
        {
            try
            {
                JsonHandler jsonHand;
                JToken channelsToken, continuationToken;

                if (response.Response.Contains("doctype html"))
                {
                    var jsonResponse = Utilities.GetBetween(response.Response,
                        "window[\"ytInitialData\"] = ", ";\n    window[\"ytInitialPlayerResponse\"]");
                    if (string.IsNullOrEmpty(jsonResponse))
                        jsonResponse = Utilities.GetBetween(response.Response,
                            "window[\"ytInitialData\"] = ", ";\r\n    window[\"ytInitialPlayerResponse\"]");

                    if (string.IsNullOrEmpty(jsonResponse))
                    {
                        var scriptParts = YoutubeUtilities.CommonPageSplitter(response.Response);
                        jsonResponse = Utilities
                            .GetBetween(
                                scriptParts.FirstOrDefault(x => x.Trim().StartsWith("window[\"ytInitialData\"]")), "=",
                                "window[").Trim().TrimEnd(';');
                        YdStatic.ExtractCommonJsonData(ref jsonResponse);
                    }

                    jsonHand = new JsonHandler(jsonResponse);

                    if (needHeaderElements)
                        GetHeaderElements(response.Response);

                    channelsToken = jsonHand.GetJToken("contents", "twoColumnBrowseResultsRenderer", "tabs", 0,
                        "tabRenderer", "content", "sectionListRenderer", "contents", 0, "itemSectionRenderer",
                        "contents", 0, "shelfRenderer", "content", "expandedShelfContentsRenderer", "items");

                    continuationToken = jsonHand.GetJToken("contents", "twoColumnBrowseResultsRenderer", "tabs", 0,
                        "tabRenderer", "content", "sectionListRenderer", "continuations", 0);
                }
                else
                {
                    var jsonResponse = Utilities.GetBetween(response.Response, "},\r\n", "]\r\n");

                    jsonHand = new JsonHandler(jsonResponse);

                    channelsToken = jsonHand.GetJToken("response", "continuationContents", "sectionListContinuation",
                        "contents", 0, "itemSectionRenderer", "contents", 0, "shelfRenderer", "content",
                        "expandedShelfContentsRenderer", "items");
                    continuationToken = jsonHand.GetJToken("response", "continuationContents",
                        "sectionListContinuation", "continuations", 0);
                }

                if (continuationToken?.Any() ?? false)
                    PostDataElements = new PostDataElements
                    {
                        ContinuationToken =
                            jsonHand.GetJTokenValue(continuationToken, "nextContinuationData", "continuation"),
                        ClickTrackingParams = jsonHand.GetJTokenValue(continuationToken, "nextContinuationData",
                            "clickTrackingParams")
                    };

                if (!(channelsToken?.Any() ?? false)) return;

                foreach (var token in channelsToken)
                {
                    var channel = new YoutubeChannel
                    {
                        ChannelId = jsonHand.GetJTokenValue(token, "channelRenderer", "channelId"),
                        ChannelUsername = "",
                        ChannelName = jsonHand.GetJTokenValue(token, "channelRenderer", "title", "simpleText"),
                        ChannelDescription = jsonHand.GetJTokenValue(token, "channelRenderer", "descriptionSnippet",
                            "simpleText"),
                        ViewsCount = jsonHand.GetJTokenValue(token, "channelRenderer", "videoCountText", "simpleText"),
                        SubscriberCount = jsonHand.GetJTokenValue(token, "channelRenderer", "subscriberCountText",
                            "simpleText")
                    };
                    YoutubeSubscribedChannelsList.Add(channel);
                }

                Success = true;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public HeadersElements HeadersElements { get; set; }
        public PostDataElements PostDataElements { get; set; }
        public bool HasMoreResults { get; set; }
        public List<YoutubeChannel> YoutubeSubscribedChannelsList { get; set; } = new List<YoutubeChannel>();

        private void GetHeaderElements(string response)
        {
            var scriptParts = YoutubeUtilities.CommonPageSplitter(response);
            var jSonString1 = Utilities
                .GetBetween(
                    scriptParts.FirstOrDefault(x =>
                        x.Trim().StartsWith("if (window.ytcsi) {window.ytcsi.tick(\"lpcs\", null, '');}")),
                    "function() {window.ytplayer = {};ytcfg.set(", ");ytcfg.set(").Trim().TrimEnd(';').TrimEnd(')');
            YdStatic.ExtractCommonJsonData(ref jSonString1);
            var jsonHand = new JsonHandler(jSonString1);

            PostDataElements = PostDataElements ?? new PostDataElements();
            PostDataElements.XsrfToken = Uri.EscapeDataString(jsonHand.GetElementValue("XSRF_TOKEN"));

            HeadersElements = new HeadersElements
            {
                PageCl = jsonHand.GetElementValue("PAGE_CL"),
                PageBuildLabel = jsonHand.GetElementValue("PAGE_BUILD_LABEL"),
                VariantsChecksum = jsonHand.GetElementValue("VARIANTS_CHECKSUM"),
                IdToken = jsonHand.GetElementValue("ID_TOKEN"),
                ClientVersion = jsonHand.GetElementValue("INNERTUBE_CONTEXT_CLIENT_VERSION"),
                ClientName = jsonHand.GetElementValue("INNERTUBE_CONTEXT_CLIENT_NAME"),
                RefererUrl = "https://www.youtube.com/feed/channels"
            };
        }
    }
}