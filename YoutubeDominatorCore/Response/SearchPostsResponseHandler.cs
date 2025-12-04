using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YoutubeDominatorCore.YDUtility;
using YoutubeDominatorCore.YoutubeModels;
using static YoutubeDominatorCore.YDEnums.Enums;

namespace YoutubeDominatorCore.Response
{
    public class SearchPostsResponseHandler : YdResponseHandler
    {

        public PostDataElements PostDataElements { get; set; }
        public HeadersElements HeadersElements { get; set; }

        public bool HasMoreResults { get; set; }
        public List<YoutubePost> ListOfYoutubePosts { get; set; } = new List<YoutubePost>();
        public SearchPostsResponseHandler()
        {
        }
        public SearchPostsResponseHandler(IResponseParameter response, List<string> JsonPostList, bool _hasMoreResults)
        {

            try
            {
                foreach (var item in JsonPostList)
                {
                    JObject jObject = new JObject();
                    var vedioElements = new JArray();
                    var postItems = new JArray();
                    try
                    {
                        var jSonString = item;
                        if (!jSonString.IsValidJson())
                        {

                            var scriptParts = YoutubeUtilities.CommonPageSplitter(jSonString);
                            jSonString = Utilities
                                .GetBetween(
                                    scriptParts.FirstOrDefault(x => x.Trim().StartsWith("window[\"ytInitialData\"]")), "=",
                                    "window[").Trim().TrimEnd(';');
                            if (string.IsNullOrEmpty(jSonString))
                                jSonString = Utilities
                                    .GetBetween(scriptParts.FirstOrDefault(x => x.Trim().StartsWith("var ytInitialData")),
                                        "=", "</script>").Trim().TrimEnd(';');
                        }
                        YdStatic.ExtractCommonJsonData(ref jSonString);
                        jObject = handler.ParseJsonToJsonObject(jSonString);
                        var nodeToken = handler.GetJTokenOfJToken(jObject, "contents", "twoColumnSearchResultsRenderer", "primaryContents", "sectionListRenderer", "contents");
                        if (nodeToken.Count() == 0)
                            nodeToken= JsonSearcher.FindByKey(JsonSearcher.FindByKey(jObject, "twoColumnSearchResultsRenderer"), "contents");
                        if (nodeToken.Count() == 0)
                            nodeToken = JsonSearcher.FindByKey(JsonSearcher.FindByKey(jObject, "itemSectionRenderer"), "contents");
                        foreach (var nodeP in nodeToken)
                        {
                            var postItemstoken = handler.GetJTokenOfJToken(nodeP, "itemSectionRenderer", "contents");
                            if (postItemstoken.Count() == 0)
                                postItemstoken = JsonSearcher.FindByKey(JsonSearcher.FindByKey(nodeP, "itemSectionRenderer"), "contents");
                            if (postItemstoken.Count() != 0)
                            {
                                if (postItemstoken.Type == JTokenType.Array)
                                {
                                    foreach (var node in postItemstoken)
                                        postItems.Add(node);
                                }
                                else
                                    postItems.Add(postItemstoken);
                            }
                            else if(nodeP.Count()!=0)
                            {
                                postItems.Add(nodeP);
                            }
                        }
                        
                        
                            



                        foreach (var node in postItems)
                        {
                            var vedioItem = JsonSearcher.FindByKey(node, "videoRenderer");
                            if (vedioItem.Count() == 0)
                                vedioItem = JsonSearcher.FindByKey(node, "movieRenderer");
                            if (vedioItem.Count() == 0)
                            {
                                vedioItem = JsonSearcher.FindByKey(node, "reelShelfRenderer");
                                if (vedioItem.Count() != 0)
                                    vedioItem = handler.GetJTokenOfJToken(vedioItem, "items");
                            }
                            if (vedioItem.Count() == 0)
                            {
                                vedioItem = JsonSearcher.FindByKey(node, "shelfRenderer");
                                if (vedioItem.Count() != 0)
                                    vedioItem = handler.GetJTokenOfJToken(vedioItem, "items");
                            }
                            if (vedioItem.Type == JTokenType.Array)
                            {
                                foreach (var nextitem in vedioItem)
                                {
                                    var Item = JsonSearcher.FindByKey(nextitem, "videoRenderer");
                                    if (Item.Count() == 0)
                                        Item = JsonSearcher.FindByKey(nextitem, "reelItemRenderer");
                                    if (Item.Count() != 0)
                                        vedioElements.Add(Item);
                                }
                            }
                            else
                                vedioElements.Add(vedioItem);

                        }

                        foreach (var postItem in vedioElements)
                        {
                            var postNode = postItem;
                            var type = VedioType.Video.ToString();

                            var title = JsonSearcher.FindStringValueByKey(JsonSearcher.FindByKey(postNode, "title"), "text");
                            if (string.IsNullOrEmpty(title))
                                title = JsonSearcher.FindStringValueByKey(JsonSearcher.FindByKey(postNode, "headline"), "simpleText");
                            var postId = JsonSearcher.FindStringValueByKey(postNode, "videoId");
                            var url = $"https://www.youtube.com/watch?v={postId}";
                            var viewsCount = JsonSearcher.FindStringValueByKey(JsonSearcher.FindByKey(postNode, "viewCountText"), "simpleText").Replace(" views", "");

                            var channelTitle = JsonSearcher.FindStringValueByKey(JsonSearcher.FindByKey(postNode, "longBylineText"), "text");
                            var channeliD = JsonSearcher.FindStringValueByKey(JsonSearcher.FindByKey(postNode, "longBylineText"), "browseId");
                            if (string.IsNullOrEmpty(channeliD))
                                channeliD = JsonSearcher.FindStringValueByKey(JsonSearcher.FindByKey(postNode, "channelThumbnailSupportedRenderers"), "browseId");
                            var channelUserName = JsonSearcher.FindStringValueByKey(JsonSearcher.FindByKey(postNode, "longBylineText"), "url").TrimStart('/');
                            var caption = "";
                            var captionToken = JsonSearcher.FindByKey(JsonSearcher.FindByKey(postNode, "detailedMetadataSnippets"), "runs");
                            foreach (var captionItem in captionToken)
                                caption += JsonSearcher.FindStringValueByKey(captionItem, "text") + " ";
                            if (string.IsNullOrEmpty(caption))
                                caption = JsonSearcher.FindStringValueByKey(JsonSearcher.FindByKey(postNode, "descriptionSnippet"), "text");
                            if (string.IsNullOrEmpty(caption))
                                caption = JsonSearcher.FindStringValueByKey(JsonSearcher.FindByKey(postNode, "descriptionSnippet"), "text");
                            if (string.IsNullOrEmpty(caption))
                                caption = JsonSearcher.FindStringValueByKey(JsonSearcher.FindByKey(postNode, "headline"), "simpleText");
                            var vedioLength = JsonSearcher.FindStringValueByKey(JsonSearcher.FindByKey(postNode, "lengthText"), "simpleText");
                            TimeSpan.TryParse(vedioLength, out TimeSpan lengthTimeSpan);
                            int.TryParse(lengthTimeSpan.TotalSeconds.ToString(), out int length);

                            if (ListOfYoutubePosts.Any(x => postId != "" && x.Id == postId) || postId == "") continue;
                            ListOfYoutubePosts.Add(new YoutubePost()
                            {
                                Title = title,
                                Code = postId,
                                PostUrl = url,
                                ViewsCount = YoutubeUtilities.YoutubeElementsCountInNumber(viewsCount),
                                ChannelTitle = channelTitle,
                                ChannelId = channeliD,
                                ChannelUsername = channelUserName,
                                Caption = caption,
                                VideoLength = length,
                                Type = type
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                }

            }
            catch (Exception)
            {
            }
            Success = JsonPostList.Count > 0;
            HasMoreResults = _hasMoreResults;
        }

        public SearchPostsResponseHandler(IResponseParameter response, SearchPostsResponseHandler postHandler = null)
        {
            try
            {
                JToken jTokenOfPostDetails = null;

                if (postHandler == null)
                {
                    var scriptParts = YoutubeUtilities.CommonPageSplitter(response.Response);
                    try
                    {
                        var jSonString = Utilities
                            .GetBetween(
                                scriptParts.FirstOrDefault(x => x.Trim().StartsWith("window[\"ytInitialData\"]")), "=",
                                "window[").Trim().TrimEnd(';');
                        if (string.IsNullOrEmpty(jSonString))
                            jSonString = Utilities
                                .GetBetween(scriptParts.FirstOrDefault(x => x.Trim().StartsWith("var ytInitialData")),
                                    "=", "</script>").Trim().TrimEnd(';');

                        YdStatic.ExtractCommonJsonData(ref jSonString);
                        var jsonHand1 = new JsonHandler(jSonString);

                        // old one
                        //PostDataElements = new PostDataElements
                        //{
                        //    Itct = jsonHand1.GetElementValue("contents", "twoColumnSearchResultsRenderer",
                        //        "primaryContents", "sectionListRenderer", "contents", 0, "itemSectionRenderer",
                        //        "continuations", 0, "nextContinuationData", "clickTrackingParams"),
                        //    ContinuationToken = jsonHand1.GetElementValue("contents", "twoColumnSearchResultsRenderer", "primaryContents", "sectionListRenderer", "contents", 0, "itemSectionRenderer", "continuations", 0, "nextContinuationData", "continuation")
                        //};

                        PostDataElements = new PostDataElements
                        {
                            Itct = jsonHand1.GetElementValue("contents", "twoColumnSearchResultsRenderer",
                                "primaryContents", "sectionListRenderer", "contents", 1, "continuationItemRenderer",
                                "continuationEndpoint", "clickTrackingParams"),
                            ContinuationToken = jsonHand1.GetElementValue("contents", "twoColumnSearchResultsRenderer",
                                "primaryContents", "sectionListRenderer", "contents", 1, "continuationItemRenderer",
                                "continuationEndpoint", "continuationCommand", "token")
                        };

                        jTokenOfPostDetails = jsonHand1.GetJToken("contents", "twoColumnSearchResultsRenderer",
                            "primaryContents", "sectionListRenderer", "contents", 1, "itemSectionRenderer", "contents");
                        if (!jTokenOfPostDetails.Any())
                            jTokenOfPostDetails = jsonHand1.GetJToken("contents", "twoColumnSearchResultsRenderer",
                                "primaryContents", "sectionListRenderer", "contents", 0, "itemSectionRenderer",
                                "contents");
                        Success = true;
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                    try
                    {
                        var jSonString1 = Utilities
                            .GetBetween(
                                scriptParts.FirstOrDefault(x =>
                                    x.Trim().StartsWith(
                                        "if (window.ytcsi) {window.ytcsi.tick(\"lpcs\", null, '');}window.Polymer =")),
                                "function() {window.ytplayer = {};ytcfg.set(", ");ytcfg.set(").Trim().TrimEnd(';')
                            .TrimEnd(')');
                        if (string.IsNullOrEmpty(jSonString1))
                            jSonString1 = Utilities
                                .GetBetween(
                                    scriptParts.FirstOrDefault(x =>
                                        x.Trim().StartsWith(
                                            "if (window.ytcsi) {window.ytcsi.tick(\"lpcs\", null, '');}(function() {window.ytplayer = {};ytcfg.set(")),
                                    "function() {window.ytplayer = {};ytcfg.set(", ");ytcfg.set(").Trim().TrimEnd(';')
                                .TrimEnd(')');
                        if (string.IsNullOrEmpty(jSonString1))
                            jSonString1 = Utilities
                                .GetBetween(
                                    scriptParts.FirstOrDefault(x =>
                                        x.Trim().StartsWith("(function() {window.ytplayer={};\nytcfg.set(")),
                                    "function() {window.ytplayer={};\nytcfg.set(", ");var").Trim().TrimEnd(';')
                                .TrimEnd(')');

                        YdStatic.ExtractCommonJsonData(ref jSonString1);

                        var jsonHand = new JsonHandler(jSonString1);

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
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                    try
                    {
                        PostDataElements.AddInUrl =
                            Utilities.GetBetween(response.Response, "\"urlEndpoint\":{\"url\":\"", "\"");
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                }
                else
                {
                    try
                    {
                        HeadersElements = postHandler.HeadersElements;
                        PostDataElements = postHandler.PostDataElements;

                        var jsonHand = new JsonHandler("{\"JsonData\":" + response.Response + "}");

                        var useThisToken = jsonHand.GetJToken("JsonData", 1, "response", "onResponseReceivedCommands",
                            0, "appendContinuationItemsAction", "continuationItems");

                        jTokenOfPostDetails =
                            jsonHand.GetJTokenOfJToken(useThisToken, 0, "itemSectionRenderer", "contents");

                        PostDataElements.ContinuationToken = jsonHand.GetJTokenValue(useThisToken, 1,
                            "continuationItemRenderer", "continuationEndpoint", "continuationCommand", "token");
                        PostDataElements.Itct = jsonHand.GetJTokenValue(useThisToken, 1, "continuationItemRenderer",
                            "continuationEndpoint", "clickTrackingParams");
                        Success = true;
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                }

                ScrapPostsFromJTokenObj(jTokenOfPostDetails);
                HasMoreResults = !string.IsNullOrEmpty(PostDataElements?.ContinuationToken);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }



        public void ScrapPostsFromJTokenObj(JToken objPost)
        {
            foreach (var item in objPost ?? new JArray())
            {
                var youtubePost = new YoutubePost();
                try
                {
                    if (item["videoRenderer"] == null)
                        continue;

                    var jToken = new JsonHandler(item);

                    youtubePost.Code = jToken.GetElementValue("videoRenderer", "videoId");
                    if (string.IsNullOrEmpty(youtubePost.Code))
                        continue;

                    youtubePost.PostUrl = "https://www.youtube.com/watch?v=" + youtubePost.Code;
                    // youtubePost.ProfileThumbNail = jToken.GetElementValue("videoRenderer", "thumbnail", "thumbnails", 0, "url");

                    youtubePost.ViewsCount = jToken.GetElementValue("videoRenderer", "viewCountText", "simpleText")
                        .RemoveAllExceptWholeNumb();

                    //youtubePost.Username = jToken.GetElementValue("videoRenderer", "longBylineText", "runs", 0, "text");

                    youtubePost.Title = jToken.GetElementValue("videoRenderer", "title", "simpleText");
                    if (string.IsNullOrWhiteSpace(youtubePost.Title))
                        youtubePost.Title = jToken.GetElementValue("videoRenderer", "title", "runs", 0, "text");
                    //youtubePost.Label = jToken.GetElementValue("videoRenderer", "title", "accessibility", "accessibilityData", "label");

                    youtubePost.PublishedDate =
                        jToken.GetElementValue("videoRenderer", "publishedTimeText", "simpleText");

                    youtubePost.ChannelId = jToken.GetElementValue("videoRenderer",
                        "channelThumbnailSupportedRenderers", "channelThumbnailWithLinkRenderer", "navigationEndpoint",
                        "browseEndpoint", "browseId");

                    youtubePost.ChannelUsername = jToken.GetElementValue("videoRenderer", "longBylineText", "runs", 0,
                        "navigationEndpoint", "browseEndpoint", "canonicalBaseUrl").Replace("/user/", "");

                    var sb = new StringBuilder();
                    foreach (var element in jToken.GetJToken("videoRenderer", "descriptionSnippet", "runs"))
                        sb.Append($"{jToken.GetJTokenValue(element, "text")}\r\n");
                    youtubePost.Caption = sb.ToString();

                    var videoLengthTime = jToken.GetElementValue("videoRenderer", "lengthText", "simpleText");
                    if (!string.IsNullOrEmpty(videoLengthTime))
                    {
                        var videoLengthInt = 0;
                        if (videoLengthTime.Contains(":"))
                        {
                            var videoLengthArr = videoLengthTime.Split(':').Reverse().ToList();

                            for (var i = 0; i < videoLengthArr.Count; i++)
                                videoLengthInt +=
                                    Convert.ToInt32(videoLengthArr[i]) * (i != 0 ? i == 1 ? 60 : 3600 : 1);
                        }
                        else
                        {
                            var lengthArray = videoLengthTime.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                            int getNumber;
                            foreach (var itemLength in lengthArray)
                                if (itemLength.Contains("second") || itemLength.Contains("minute") ||
                                    itemLength.Contains("hour"))
                                {
                                    int.TryParse(itemLength.RemoveAllExceptWholeNumb(), out getNumber);
                                    videoLengthInt += getNumber;
                                }
                        }

                        youtubePost.VideoLength = videoLengthInt;
                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                ListOfYoutubePosts.Add(youtubePost);
            }
        }
    }
}