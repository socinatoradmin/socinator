using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using YoutubeDominatorCore.YDUtility;
using YoutubeDominatorCore.YoutubeModels;

namespace YoutubeDominatorCore.Response
{
    public class ScrapPostFromChannelResponseHandler : YdResponseHandler
    {
        public ScrapPostFromChannelResponseHandler()
        {
        }

        public ScrapPostFromChannelResponseHandler(IResponseParameter response, bool needHeaderElements)
        {
            try
            {
                JsonHandler jsonHand;
                JToken needNextJToken;
                string channelId, channelUsername;

                if (response.Response.Contains("doctype html") || response.Response.Contains("DOCTYPE html"))
                {
                    var splitResp = Regex.Split(response.Response, "ytcfg.set");
                    var jsonData = Utilities.GetBetween(splitResp.FirstOrDefault(x => x.Contains("PAGE_BUILD_LABEL")),
                        "(", ");");
                    YdStatic.ExtractCommonJsonData(ref jsonData);
                    jsonHand = new JsonHandler(jsonData);
                    if (needHeaderElements)
                        HeadersElements = new HeadersElements
                        {
                            PageBuildLabel = jsonHand.GetElementValue("PAGE_BUILD_LABEL"),
                            VariantsChecksum = jsonHand.GetElementValue("VARIANTS_CHECKSUM"),
                            PageCl = jsonHand.GetElementValue("PAGE_CL"),
                            ClientVersion = jsonHand.GetElementValue("INNERTUBE_CONTEXT_CLIENT_VERSION"),
                            ClientName = jsonHand.GetElementValue("INNERTUBE_CONTEXT_CLIENT_NAME"),
                            IdToken = jsonHand.GetElementValue("ID_TOKEN")
                        };

                    jsonData = Utilities.GetBetween(response.Response, "window[\"ytInitialData\"] = ", "};") + "}";
                    YdStatic.ExtractCommonJsonData(ref jsonData);
                    jsonHand = new JsonHandler(jsonData);

                    needNextJToken = jsonHand.GetJToken("contents", "twoColumnBrowseResultsRenderer", "tabs", 0,
                        "tabRenderer", "content", "sectionListRenderer", "contents", 1, "itemSectionRenderer",
                        "contents", 0, "shelfRenderer");
                    if (!needNextJToken.Any())
                        needNextJToken = jsonHand.GetJToken("contents", "twoColumnBrowseResultsRenderer", "tabs", 0,
                            "tabRenderer", "content", "sectionListRenderer", "contents", 0, "itemSectionRenderer",
                            "contents", 0, "gridRenderer");
                    if (!needNextJToken.Any())
                        needNextJToken = jsonHand.GetJToken("contents", "twoColumnBrowseResultsRenderer", "tabs", 1,
                            "tabRenderer", "content", "sectionListRenderer", "contents", 0, "itemSectionRenderer",
                            "contents", 0, "gridRenderer");

                    channelId = jsonHand.GetElementValue("header", "c4TabbedHeaderRenderer", "navigationEndpoint",
                        "browseEndpoint", "browseId");
                    channelUsername = jsonHand.GetElementValue("header", "c4TabbedHeaderRenderer", "navigationEndpoint",
                        "browseEndpoint", "canonicalBaseUrl").Replace("/user/", "");
                    if (channelUsername.StartsWith("/channel/"))
                        channelUsername = "";
                }
                else
                {
                    var jsonData = Utilities.GetBetween(response.Response, "},\r\n", "]\r\n");
                    jsonHand = new JsonHandler(jsonData);
                    needNextJToken = jsonHand.GetJToken("response", "continuationContents", "gridContinuation");

                    channelId = jsonHand.GetElementValue("response", "metadata", "channelMetadataRenderer",
                        "externalId");
                    channelUsername = jsonHand
                        .GetElementValue("response", "metadata", "channelMetadataRenderer", "vanityChannelUrl")
                        .Replace("http://www.youtube.com/user/", "");
                }


                var needJTokenForVideos = jsonHand.GetJTokenOfJToken(needNextJToken, "items");
                if (!needJTokenForVideos.Any())
                    needJTokenForVideos =
                        jsonHand.GetJTokenOfJToken(needNextJToken, "content", "horizontalListRenderer", "items");
                if (!needJTokenForVideos.Any())
                    needJTokenForVideos = jsonHand.GetJTokenOfJToken(needNextJToken, "items");

                foreach (var token in needJTokenForVideos)
                {
                    var post = new YoutubePost
                    {
                        Code = jsonHand.GetJTokenValue(token, "gridVideoRenderer", "videoId"),
                        Title = jsonHand.GetJTokenValue(token, "gridVideoRenderer", "title", "simpleText"),
                        ViewsCount = jsonHand.GetJTokenValue(token, "gridVideoRenderer", "viewCountText", "simpleText")
                            .RemoveAllExceptWholeNumb(),
                        ChannelId = channelId,
                        ChannelUsername = channelUsername
                    };

                    var videoDuration = "";
                    foreach (var vLength in jsonHand.GetJTokenOfJToken(token, "gridVideoRenderer", "thumbnailOverlays"))
                    {
                        var innerToken = jsonHand.GetJTokenOfJToken(vLength, "thumbnailOverlayTimeStatusRenderer");
                        if (innerToken.Any())
                        {
                            videoDuration = jsonHand.GetJTokenValue(innerToken, "text", "simpleText");
                            break;
                        }
                    }

                    try
                    {
                        if (!string.IsNullOrEmpty(videoDuration))
                            post.VideoLength = Convert.ToInt32(TimeSpan
                                .Parse(videoDuration.Length < 6 ? $"00:{videoDuration}" : videoDuration).TotalSeconds);
                    }
                    catch
                    {
                        /*ignore*/
                    }

                    post.PostUrl = $"https://www.youtube.com/watch?v={post.Code}";

                    ListYoutubePost.Add(post);
                }

                var needContinuationToken =
                    jsonHand.GetJTokenOfJToken(needNextJToken, "continuations", 0, "nextContinuationData");

                if (needContinuationToken.Any())
                    PostDataElements = new PostDataElements
                    {
                        ContinuationToken = jsonHand.GetJTokenValue(needContinuationToken, "continuation"),
                        Itct = jsonHand.GetJTokenValue(needContinuationToken, "clickTrackingParams")
                    };

                Success = true;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public List<YoutubePost> ListYoutubePost { get; set; } = new List<YoutubePost>();
        public PostDataElements PostDataElements { get; set; }
        public HeadersElements HeadersElements { get; set; }
    }
}