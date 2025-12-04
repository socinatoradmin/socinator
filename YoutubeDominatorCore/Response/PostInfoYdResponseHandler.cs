using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using YoutubeDominatorCore.YDUtility;
using YoutubeDominatorCore.YoutubeModels;
using static DominatorHouseCore.DatabaseHandler.YdTables.Accounts.InteractedPosts;

namespace YoutubeDominatorCore.Response
{
    public class PostInfoYdResponseHandler : YdResponseHandler
    {
        public PostInfoYdResponseHandler()
        {
        }
        JObject jobject = new JObject();
        public PostInfoYdResponseHandler(IResponseParameter responsePara, bool getPostDetail = true,
            bool getNecessaryElements = true, bool hasSpecificCommentId = false)
        {
            if (IsEmptyResponse || CaptchaFound)
                return;

            string[] scriptParts = { };
            try
            {

                scriptParts = YoutubeUtilities.CommonPageSplitter(responsePara.Response);

                var first = scriptParts.FirstOrDefault(x => x.Trim().StartsWith("window[\"ytInitialData\"]"));

                var jSonString = Utilities.GetBetween(first, "=", "window[").Trim().TrimEnd(';');

                if (string.IsNullOrEmpty(jSonString))
                    jSonString = Utilities
                        .GetBetween(
                            scriptParts.FirstOrDefault(x =>
                                x.Trim().StartsWith("var ytplayer = ytplayer || {};ytplayer.config =")), ".config =",
                            "ytplayer.load").Trim().TrimEnd(';');
                if (string.IsNullOrEmpty(jSonString))
                    jSonString = Utilities
                        .GetBetween(scriptParts.FirstOrDefault(x => x.Trim().StartsWith("window[\"ytInitialData\"]")),
                            "=", "window[").Trim().TrimEnd(';');
                if (string.IsNullOrEmpty(jSonString))
                    jSonString = Utilities
                        .GetBetween(scriptParts.FirstOrDefault(x => x.Trim().StartsWith("var ytInitialData")), "=",
                            "</script>").Trim().TrimEnd(';');

                YdStatic.ExtractCommonJsonData(ref jSonString);
                if (jSonString.IsValidJson())
                    jobject = handler.ParseJsonToJsonObject(jSonString);

                YoutubePost.Code = JsonSearcher.FindStringValueByKey(JsonSearcher.FindByKey(jobject, "likeButtonViewModel"), "videoId");
                if (string.IsNullOrEmpty(YoutubePost.Code))
                    YoutubePost.Code = JsonSearcher.FindStringValueByKey(JsonSearcher.FindByKey(jobject, "likeEndpoint"), "videoId");
                if (string.IsNullOrEmpty(YoutubePost.Code))
                    YoutubePost.Code = JsonSearcher.FindStringValueByKey(JsonSearcher.FindByKey(jobject, "currentVideoEndpoint"), "videoId");

                if (!string.IsNullOrEmpty(YoutubePost.Code))
                    YoutubePost.PostUrl = "https://www.youtube.com/watch?v=" + YoutubePost.Code;

                var ownerJtoken = JsonSearcher.FindByKey(JsonSearcher.FindByKey(jobject, "contents"), "videoSecondaryInfoRenderer");
                YoutubePost.ChannelId = JsonSearcher.FindStringValueByKey(JsonSearcher.FindByKey(ownerJtoken, "videoOwnerRenderer"), "browseId");
                if (string.IsNullOrEmpty(YoutubePost.ChannelId))
                    YoutubePost.ChannelId = JsonSearcher.FindStringValueByKey(JsonSearcher.FindByKey(JsonSearcher.FindByKey(ownerJtoken, "videoOwnerRenderer"), "navigationEndpoint"), "browseId");
                if (string.IsNullOrEmpty(YoutubePost.ChannelId))
                    YoutubePost.ChannelId = JsonSearcher.FindStringValueByKey(JsonSearcher.FindByKey(JsonSearcher.FindByKey(ownerJtoken, "videoOwnerRenderer"), "subscribeButton"), "channelId");

                YoutubePost.ChannelUsername = JsonSearcher.FindStringValueByKey(JsonSearcher.FindByKey(JsonSearcher.FindByKey(ownerJtoken, "videoOwnerRenderer"), "navigationEndpoint"), "url").Replace("/", "");
                if (string.IsNullOrEmpty(YoutubePost.ChannelUsername))
                    YoutubePost.ChannelUsername = JsonSearcher.FindStringValueByKey(JsonSearcher.FindByKey(JsonSearcher.FindByKey(ownerJtoken, "videoOwnerRenderer"), "navigationEndpoint"), "canonicalBaseUrl").Replace("/", "");
                if (string.IsNullOrEmpty(YoutubePost.ChannelUsername))
                    YoutubePost.ChannelUsername = JsonSearcher.FindStringValueByKey(JsonSearcher.FindByKey(JsonSearcher.FindByKey(jobject, "videoPrimaryInfoRenderer"), "navigationEndpoint"), "canonicalBaseUrl").Replace("/", "");

                var subscriberCount = JsonSearcher.FindStringValueByKey(JsonSearcher.FindByKey(JsonSearcher.FindByKey(ownerJtoken, "videoOwnerRenderer"), "subscriberCountText"), "simpleText").Replace(" subscribers", "");
                if (string.IsNullOrEmpty(subscriberCount))
                    subscriberCount = JsonSearcher.FindStringValueByKey(JsonSearcher.FindByKey(JsonSearcher.FindByKey(jobject, "videoDescriptionInfocardsSectionRenderer"), "sectionSubtitle"), "simpleText").Replace(" subscribers", "");
                YoutubePost.ChannelSubscriberCount = YoutubeUtilities.YoutubeElementsCountInNumber(subscriberCount);


                YoutubePost.IsChannelSubscribed = JsonSearcher.FindStringValueByKey(JsonSearcher.FindByKey(JsonSearcher.FindByKey(ownerJtoken, "subscribeButtonRenderer"), "buttonText"), "text").Contains("Subscribed")
                    || JsonSearcher.FindStringValueByKey(JsonSearcher.FindByKey(JsonSearcher.FindByKey(ownerJtoken, "subscribeButton"), "subscribeButtonRenderer"), "subscribed").Contains("true")
                    || JsonSearcher.FindStringValueByKey(JsonSearcher.FindByKey(JsonSearcher.FindByKey(jobject, "frameworkUpdates"), "mutations"), "subscribed").Contains("true");
                if (getPostDetail)
                    CollectPostDetails(jobject, jSonString);

                var getLengthData = scriptParts.FirstOrDefault(x => x.Trim().Contains("\"lengthSeconds\":\""));
                if (!string.IsNullOrEmpty(getLengthData))
                {
                    var seconds = Utilities.GetBetween(getLengthData, "\"lengthSeconds\":\"", "\"");
                    IsLiveVideo = getLengthData.Contains("\"isLive\":true");
                    if (!IsLiveVideo)
                    {
                        if (string.IsNullOrEmpty(seconds))
                            seconds = "0";
                        YoutubePost.VideoLength = Convert.ToInt32(seconds);
                    }
                }

                if (!IsLiveVideo)
                {
                    GetCommentsListJson(jobject, hasSpecificCommentId);
                }

                Success = true;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            try
            {
                if (getNecessaryElements && Success)
                {
                    var str1 = scriptParts.FirstOrDefault(x =>
                                x.Trim().StartsWith(
                                    "var ytInitialData ="));
                    var jSonString = Utilities.GetBetween(str1, "var ytInitialData = ", ";</script>");
                    CollectNecessaryElements(handler, jSonString);
                    YoutubePost.PostDataElements.InnertubeContext = "{" + Utilities.GetBetween(responsePara.Response, "{\"INNERTUBE_CONTEXT\":{", "}}}");
                    YoutubePost.HeadersElements.XGoogVisitorId = Utilities.GetBetween(YoutubePost.PostDataElements.InnertubeContext, "\"visitorData\":\"", "\"");
                    //"createCommentParams":"Egt3S3VhbVFzUFdaTSoCCABQBw%3D%3D"}
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public YoutubePost YoutubePost { get; set; } = new YoutubePost();
        public bool IsLiveVideo { get; set; }

        private void CollectPostDetails(JObject jobject, string jSonString)
        {
            YoutubePost.Title = JsonSearcher.FindStringValueByKey(JsonSearcher.FindByKey(jobject, "title"), "text");
            if (string.IsNullOrEmpty(YoutubePost.Title))
            {
                YoutubePost.Title = handler.GetJTokenValue(jobject, "contents", "twoColumnWatchNextResults", "results",
                    "results", "contents", 0, "videoPrimaryInfoRenderer", "title", "runs", 0, "text");
                if (string.IsNullOrEmpty(YoutubePost.Title))
                    YoutubePost.Title = handler.GetJTokenValue(jobject, "contents", "twoColumnWatchNextResults", "results",
                        "results", "contents", 1, "videoPrimaryInfoRenderer", "title", "runs", 0, "text");
            }
            if (string.IsNullOrEmpty(YoutubePost.Title))
                YoutubePost.Title = handler.GetJTokenValue(jobject, "playerOverlays", "playerOverlayRenderer", "videoDetails",
                    "playerOverlayVideoDetailsRenderer", "title", "simpleText");
            if (string.IsNullOrEmpty(YoutubePost.Title))
                YoutubePost.Title = handler.GetJTokenValue(jobject, "engagementPanels", 2, "engagementPanelSectionListRenderer", "content",
                    "structuredDescriptionContentRenderer", "items", 0, "videoDescriptionHeaderRenderer", "title", "runs", 0, "text");

            YoutubePost.LikeCount = handler.GetJTokenValue(jobject, "contents", "twoColumnWatchNextResults", "results",
                "results", "contents", 0, "videoPrimaryInfoRenderer", "videoActions", "menuRenderer", "topLevelButtons",
                0, "toggleButtonRenderer", "defaultText", "accessibility", "accessibilityData", "label");
            if (string.IsNullOrEmpty(YoutubePost.LikeCount))
                YoutubePost.LikeCount = handler.GetJTokenValue(jobject, "contents", "twoColumnWatchNextResults", "results",
                    "results", "contents", 1, "videoPrimaryInfoRenderer", "videoActions", "menuRenderer",
                    "topLevelButtons", 0, "toggleButtonRenderer", "defaultText", "accessibility", "accessibilityData",
                    "label");
            if (string.IsNullOrEmpty(YoutubePost.LikeCount))
                YoutubePost.LikeCount = handler.GetJTokenValue(jobject, "contents", "twoColumnWatchNextResults", "results",
                    "results", "contents", 0, "videoPrimaryInfoRenderer", "videoActions", "menuRenderer", "topLevelButtons",
                    0, "segmentedLikeDislikeButtonRenderer", "likeButton", "toggleButtonRenderer", "toggledText", "accessibility",
                    "accessibilityData", "label");
            if (string.IsNullOrEmpty(YoutubePost.LikeCount))
                YoutubePost.LikeCount = handler.GetJTokenValue(jobject, "contents", "twoColumnWatchNextResults", "results",
                    "results", "contents", 0, "videoPrimaryInfoRenderer", "videoActions", "menuRenderer", "topLevelButtons",
                    0, "segmentedLikeDislikeButtonViewModel", "likeButtonViewModel", "likeButtonViewModel"
                    , "toggleButtonViewModel", "toggleButtonViewModel", "defaultButtonViewModel",
                    "buttonViewModel", "title");
            if (string.IsNullOrEmpty(YoutubePost.LikeCount))
                YoutubePost.LikeCount = handler.GetJTokenValue(jobject, "contents", "twoColumnWatchNextResults", "results",
                    "results", "contents", 0, "videoPrimaryInfoRenderer", "videoActions", "menuRenderer", "topLevelButtons",
                    0, "segmentedLikeDislikeButtonViewModel", "likeButtonViewModel", "likeButtonViewModel"
                    , "toggleButtonViewModel", "toggleButtonViewModel", "toggledButtonViewModel",
                    "buttonViewModel", "title");
            YoutubePost.LikeCount = YoutubeUtilities.YoutubeElementsCountInNumber(YoutubePost.LikeCount);

            YoutubePost.DislikeCount = handler.GetJTokenValue(jobject, "contents", "twoColumnWatchNextResults", "results",
                "results", "contents", 0, "videoPrimaryInfoRenderer", "videoActions", "menuRenderer", "topLevelButtons",
                1, "toggleButtonRenderer", "defaultText", "accessibility", "accessibilityData", "label");

            YoutubePost.DislikeCount = YoutubeUtilities.YoutubeElementsCountInNumber(YoutubePost.DislikeCount);

            YoutubePost.ViewsCount = handler.GetJTokenValue(jobject, "contents", "twoColumnWatchNextResults", "results",
                "results", "contents", 0, "videoPrimaryInfoRenderer", "viewCount", "videoViewCountRenderer",
                "viewCount", "simpleText");
            if (string.IsNullOrEmpty(YoutubePost.ViewsCount))
                YoutubePost.ViewsCount = handler.GetJTokenValue(jobject, "contents", "twoColumnWatchNextResults", "results",
                    "results", "contents", 1, "videoPrimaryInfoRenderer", "viewCount", "videoViewCountRenderer",
                    "viewCount", "simpleText");
            if (string.IsNullOrEmpty(YoutubePost.ViewsCount))
                YoutubePost.Title = handler.GetJTokenValue(jobject, "playerOverlays", "playerOverlayRenderer", "videoDetails",
                    "playerOverlayVideoDetailsRenderer", "subtitle", "runs", 2, "text");
            if (string.IsNullOrEmpty(YoutubePost.ViewsCount))
                YoutubePost.Title = handler.GetJTokenValue(jobject, "engagementPanels", 2, "engagementPanelSectionListRenderer", "content",
                    "structuredDescriptionContentRenderer", "items", 0, "videoDescriptionHeaderRenderer"
                    , "views", "simpleText");
            YoutubePost.ViewsCount = YoutubeUtilities.YoutubeElementsCountInNumber(YoutubePost.ViewsCount);

            YoutubePost.ChannelTitle = handler.GetJTokenValue(jobject, "contents", "twoColumnWatchNextResults", "results",
                "results", "contents", 1, "videoSecondaryInfoRenderer", "owner", "videoOwnerRenderer", "title", "runs",
                0, "text");
            if (string.IsNullOrEmpty(YoutubePost.ChannelTitle))
                YoutubePost.ChannelTitle = handler.GetJTokenValue(jobject, "contents", "twoColumnWatchNextResults", "results",
                    "results", "contents", 2, "videoSecondaryInfoRenderer", "owner", "videoOwnerRenderer", "title",
                    "runs", 0, "text");
            if (string.IsNullOrEmpty(YoutubePost.ChannelTitle))
                YoutubePost.ChannelTitle = handler.GetJTokenValue(jobject, "engagementPanels", 2, "engagementPanelSectionListRenderer", "content", "structuredDescriptionContentRenderer", "items", 0, "videoDescriptionHeaderRenderer"
                    , "channel", "simpleText");

            YoutubePost.PublishedDate = handler.GetJTokenValue(jobject, "contents", "twoColumnWatchNextResults", "results",
                "results", "contents", 0, "videoPrimaryInfoRenderer", "dateText", "simpleText").Trim();
            if (string.IsNullOrEmpty(YoutubePost.PublishedDate))
                YoutubePost.PublishedDate = handler.GetJTokenValue(jobject, "contents", "twoColumnWatchNextResults", "results",
                    "results", "contents", 1, "videoPrimaryInfoRenderer", "dateText", "simpleText").Trim();
            if (string.IsNullOrEmpty(YoutubePost.PublishedDate))
                YoutubePost.PublishedDate = handler.GetJTokenValue(jobject, "engagementPanels", 2, "engagementPanelSectionListRenderer", "content", "structuredDescriptionContentRenderer", "items", 0, "videoDescriptionHeaderRenderer"
                    , "publishDate", "simpleText").Trim();

            if (string.IsNullOrEmpty(YoutubePost.ChannelSubscriberCount))
            {
                YoutubePost.ChannelSubscriberCount = handler.GetJTokenValue(jobject, "contents", "twoColumnWatchNextResults",
                    "results", "results", "contents", 2, "videoSecondaryInfoRenderer", "owner", "videoOwnerRenderer",
                    "subscriberCountText", "simpleText");

                if (string.IsNullOrEmpty(YoutubePost.ChannelSubscriberCount))
                    YoutubePost.ChannelSubscriberCount = handler.GetJTokenValue(jobject, "contents",
                        "twoColumnWatchNextResults", "results", "results", "contents", 1, "videoSecondaryInfoRenderer",
                        "owner", "videoOwnerRenderer", "subscriberCountText", "simpleText");
                if (string.IsNullOrEmpty(YoutubePost.ChannelSubscriberCount))
                    YoutubePost.ChannelSubscriberCount = handler.GetJTokenValue(jobject, "engagementPanels", 2, "engagementPanelSectionListRenderer", "content", "structuredDescriptionContentRenderer",
                        "items", 3, "videoDescriptionInfocardsSectionRenderer", "sectionSubtitle", "simpleText");
                YoutubePost.ChannelSubscriberCount =
                YoutubeUtilities.YoutubeElementsCountInNumber(YoutubePost.ChannelSubscriberCount);
            }
            YoutubePost.Reaction = LikeStatus.Indifferent;
            if (jSonString.Contains(",\"isDisabled\":false,\"defaultIcon\":{\"iconType\":\"LIKE\"}") ||
                (jSonString.Contains("\"buttonViewModel\":{\"iconName\":\"LIKE\"") && jSonString.Contains("\"isTogglingDisabled\":false}")))
            {
                YoutubePost.LikeEnabled = true;
                YoutubePost.Reaction = jSonString.Contains("\"likeStatus\":\"LIKE\"") ? LikeStatus.Like : jSonString.Contains("\"likeStatus\":\"INDIFFERENT\"") ? LikeStatus.Indifferent : LikeStatus.Dislike;
            }

            if (jSonString.Contains(",\"isDisabled\":false,\"defaultIcon\":{\"iconType\":\"DISLIKE\"}") ||
                (jSonString.Contains("\"buttonViewModel\":{\"iconName\":\"DISLIKE\"") && jSonString.Contains("\"isTogglingDisabled\":false}")))
            {
                YoutubePost.DislikeEnabled = true;
                YoutubePost.Reaction = jSonString.Contains("\"likeStatus\":\"DISLIKE\"") ? LikeStatus.Dislike : jSonString.Contains("\"likeStatus\":\"LIKE\"") ? LikeStatus.Like : LikeStatus.Indifferent;
            }

            YoutubePost.CommentCount = handler.GetJTokenValue(jobject, "contents", "twoColumnWatchNextResults", "results",
                "results", "contents", 2, "itemSectionRenderer", "header", "commentsHeaderRenderer", "countText",
                "runs", 0, "text");
            if (string.IsNullOrEmpty(YoutubePost.CommentCount))
                YoutubePost.CommentCount = handler.GetJTokenValue(jobject, "contents", "twoColumnWatchNextResults", "results",
                    "results", "contents", 3, "itemSectionRenderer", "header", "commentsHeaderRenderer", "countText",
                    "runs", 0, "text");
            if (string.IsNullOrEmpty(YoutubePost.CommentCount))
                YoutubePost.CommentCount = handler.GetJTokenValue(jobject, "contents", "twoColumnWatchNextResults", "results",
                    "results", "contents", 2, "itemSectionRenderer", "contents", 0, "commentsEntryPointHeaderRenderer", "commentCount",
                    "simpleText");
            if (string.IsNullOrEmpty(YoutubePost.CommentCount))
                YoutubePost.CommentCount = handler.GetJTokenValue(jobject, "engagementPanels", 3, "engagementPanelSectionListRenderer", "header",
                    "engagementPanelTitleHeaderRenderer", "contextualInfo", "runs", 0, "text");
            YoutubePost.CommentCount = YoutubeUtilities.YoutubeElementsCountInNumber(YoutubePost.CommentCount);

            GetDescription(jobject);
        }

        private void GetDescription(JObject jobject)
        {
            try
            {
                var sb = new StringBuilder();
                var token = handler.GetJTokenOfJToken(jobject, "contents", "twoColumnWatchNextResults", "results", "results",
                    "contents", 1, "videoSecondaryInfoRenderer", "description", "runs");
                if (token.Count() == 0)
                    token = handler.GetJTokenOfJToken(jobject, "contents", "twoColumnWatchNextResults", "results", "results",
                        "contents", 2, "videoSecondaryInfoRenderer", "description", "runs");
                foreach (var descItem in token) sb.Append($"{handler.GetJTokenValue(descItem, "text")}\r\n");

                var moreText = handler.GetJTokenValue(jobject, "contents", "twoColumnWatchNextResults", "results",
                    "results", "contents", 1, "videoSecondaryInfoRenderer", "description", "simpleText");
                if (string.IsNullOrEmpty(moreText))
                    moreText = handler.GetJTokenValue(jobject, "contents", "twoColumnWatchNextResults", "results",
                        "results", "contents", 2, "videoSecondaryInfoRenderer", "description", "simpleText");

                if (!string.IsNullOrEmpty(moreText.Trim())) sb.Append($"{moreText}\r\n");
                var token2 = handler.GetJTokenOfJToken(jobject, "contents", "twoColumnWatchNextResults", "results", "results",
                    "contents", 1, "videoSecondaryInfoRenderer", "metadataRowContainer", "metadataRowContainerRenderer",
                    "rows");
                if (token2.Count() == 0)
                    token2 = handler.GetJTokenOfJToken(jobject, "contents", "twoColumnWatchNextResults", "results", "results",
                        "contents", 2, "videoSecondaryInfoRenderer", "metadataRowContainer",
                        "metadataRowContainerRenderer", "rows");

                foreach (var descItem in token2)
                {
                    foreach (var innerDesc in handler.GetJTokenOfJToken(descItem, "metadataRowRenderer", "contents"))
                        foreach (var innerDes in handler.GetJTokenOfJToken(innerDesc, "runs"))
                            sb.Append($"{handler.GetJTokenValue(innerDes, "text")}\r\n");

                    foreach (var innerDesc in handler.GetJTokenOfJToken(descItem, "metadataRowRenderer", "contents"))
                        sb.Append($"{handler.GetJTokenValue(innerDesc, "simpleText")}\r\n");
                }

                YoutubePost.Caption = sb.ToString();
                if (string.IsNullOrEmpty(YoutubePost.Caption))
                    YoutubePost.Caption = handler.GetJTokenValue(jobject, "contents", "twoColumnWatchNextResults", "results", "results",
                    "contents", 1, "videoSecondaryInfoRenderer", "attributedDescription", "content").Replace("\n", "");
                if (string.IsNullOrEmpty(YoutubePost.Caption))
                    YoutubePost.Caption = handler.GetJTokenValue(jobject, "engagementPanels", 2, "engagementPanelSectionListRenderer", "content", "structuredDescriptionContentRenderer", "items",
                        1, "expandableVideoDescriptionBodyRenderer", "attributedDescriptionBodyText", "content").Replace("\n", "");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void GetCommentsListJson(JObject jobject, bool hasSpecificCommentId)
        {

            var jsonToken = handler.GetJTokenOfJToken(jobject, "contents", "twoColumnWatchNextResults", "results", "results",
                    "contents", 2, "itemSectionRenderer", "contents");
            if (jsonToken.Count() == 0)
                jsonToken = handler.GetJTokenOfJToken(jobject, "contents", "twoColumnWatchNextResults", "results", "results",
                    "contents", 3, "itemSectionRenderer", "contents");

            YoutubePost.ListOfCommentsInfo = new List<YoutubePostCommentModel>();

            var commentStatus = handler.GetJTokenValue(jobject, "contents", "twoColumnWatchNextResults", "conversationBar",
                "conversationBarRenderer", "availabilityMessage", "messageRenderer", "text", "simpleText");

            if (string.IsNullOrEmpty(commentStatus))
                commentStatus = handler.GetJTokenValue(jobject, "contents", "twoColumnWatchNextResults", "results", "results",
                    "contents", 2, "itemSectionRenderer", "contents", 0, "messageRenderer", "text", "runs", 0, "text");
            if (string.IsNullOrEmpty(commentStatus))
                commentStatus = handler.GetJTokenValue(jobject, "contents", "twoColumnWatchNextResults", "results", "results",
                    "contents", 2, "itemSectionRenderer", "contents", 0, "commentsEntryPointHeaderRenderer", "contentRenderer",
                    "commentEntryPointTeaserRenderer", "teaserContent", "simpleText");

            YoutubePost.CommentEnabled = string.IsNullOrEmpty(commentStatus) ||
                                         commentStatus.Contains("Chat Replay is disabled for this Premiere.") ||
                                         commentStatus.Contains("Live chat replay was turned off for this video.");
            if (!YoutubePost.CommentEnabled)
                return;

            foreach (var token in jsonToken)
            {
                var jToken = handler.GetJTokenOfJToken(token, "commentThreadRenderer", "comment", "commentRenderer");

                var youtubePostCommentModel = EachCommentModelJson(handler, jToken);

                if (YoutubePost.ListOfCommentsInfo.Count == 0 && hasSpecificCommentId)
                {
                    var replyToken = handler.GetJTokenOfJToken(token, "commentThreadRenderer", "replies",
                        "commentRepliesRenderer", "teaserContents", 0, "commentRenderer");
                    if (replyToken.Count() > 0)
                    {
                        var replyCommentModel = EachCommentModelJson(handler, replyToken);
                        if (!string.IsNullOrWhiteSpace(youtubePostCommentModel.CommentText))
                            YoutubePost.ListOfCommentsInfo.Add(replyCommentModel);
                    }
                }

                if (!string.IsNullOrWhiteSpace(youtubePostCommentModel.CommentText))
                    YoutubePost.ListOfCommentsInfo.Add(youtubePostCommentModel);
            }

        }

        private void GetCommentsList(string response, bool hasSpecificCommentId)
        {

            try
            {
                var commentList = Regex.Split(response, "id=\"comment\"").Skip(1).ToList();
                YoutubePost.ListOfCommentsInfo = new List<YoutubePostCommentModel>();
                foreach (var item in commentList)
                {
                    var youtubePostCommentModel = EachCommentModel(item);

                    if (!string.IsNullOrWhiteSpace(youtubePostCommentModel.CommentText))
                        YoutubePost.ListOfCommentsInfo.Add(youtubePostCommentModel);
                }
            }
            catch (Exception)
            {

            }
        }

        private YoutubePostCommentModel EachCommentModelJson(JsonHandler handler, JToken jToken)
        {
            var youtubePostCommentModel = new YoutubePostCommentModel
            {
                CommentText = handler.GetJTokenValue(jToken, "contentText", "simpleText"),
                CommentActionParam = handler.GetJTokenValue(jToken, "actionButtons", "commentActionButtonsRenderer",
                    "likeButton", "toggleButtonRenderer", "defaultServiceEndpoint", "performCommentActionEndpoint",
                    "action"),
                CommentId = handler.GetJTokenValue(jToken, "commentId"),
                CommentTime = handler.GetJTokenValue(jToken, "publishedTimeText", "runs", 0, "text"),
                CommenterChannelName = handler.GetJTokenValue(jToken, "authorText", "simpleText"),
                CommenterChannelId = handler.GetJTokenValue(jToken, "authorEndpoint", "browseEndpoint", "browseId"),
                CommentLikesCount = handler.GetJTokenValue(jToken, "likeCount"),
                TrackingParams = handler.GetJTokenValue(jToken, "trackingParams"),
                CreateReplyParams = handler.GetJTokenValue(jToken, "actionButtons", "commentActionButtonsRenderer",
                    "replyButton", "buttonRenderer", "navigationEndpoint", "createCommentReplyDialogEndpoint", "dialog",
                    "commentReplyDialogRenderer", "replyButton", "buttonRenderer", "serviceEndpoint",
                    "createCommentReplyEndpoint", "createReplyParams")
            };

            if (string.IsNullOrEmpty(youtubePostCommentModel.CommentText))
            {
                var textJToken = handler.GetJTokenOfJToken(jToken, "contentText", "runs");

                foreach (var eachToken in textJToken)
                    youtubePostCommentModel.CommentText += handler.GetJTokenValue(eachToken, "text");
            }

            var vote = handler.GetJTokenValue(jToken, "voteStatus");
            youtubePostCommentModel.CommentLikeStatus = vote.Equals("like", StringComparison.CurrentCultureIgnoreCase)
                ?
                LikeStatus.Like
                : vote.Equals("dislike", StringComparison.CurrentCultureIgnoreCase)
                    ? LikeStatus.Dislike
                    : LikeStatus.Indifferent;

            return youtubePostCommentModel;
        }

        private YoutubePostCommentModel EachCommentModel(string response)
        {

            try
            {
                //var decoderesponse = 
                var youtubePostCommentModel = new YoutubePostCommentModel();
                youtubePostCommentModel.CommentText = "abcd";
                youtubePostCommentModel.CommentActionParam = "";
                var commentIdData = HtmlParseUtility.GetOuterHtmlFromTagName(response, "a", "class", "yt-simple-endpoint style-scope yt-formatted-string");

                youtubePostCommentModel.CommentId = Utilities.FirstMatchExtractor(commentIdData, "href=\"(.*?)\"");
                youtubePostCommentModel.CommentTime = "";

                youtubePostCommentModel.CommenterChannelName = "";
                youtubePostCommentModel.CommenterChannelId = "";
                var ydUtils = new YoutubeUtilities();

                youtubePostCommentModel.CommentLikesCount = YoutubeUtilities.YoutubeElementsCountInNumber(HtmlParseUtility.GetInnerTextFromTagName(response, "span", "id", "vote-count-middle"));
                youtubePostCommentModel.TrackingParams = "";
                youtubePostCommentModel.CreateReplyParams = "";


                //var vote = handler.GetJTokenValue(jToken, "voteStatus");
                //youtubePostCommentModel.CommentLikeStatus = vote.Equals("like", StringComparison.CurrentCultureIgnoreCase)
                //    ?
                //    LikeStatus.Like
                //    : vote.Equals("dislike", StringComparison.CurrentCultureIgnoreCase)
                //        ? LikeStatus.Dislike
                //        : LikeStatus.Indifferent;

                return youtubePostCommentModel;
            }
            catch (Exception)
            {
                return new YoutubePostCommentModel();
            }
        }

        private void CollectNecessaryElements(JsonHandler handler, string jSonString)
        {
            YoutubePost.PostDataElements = new PostDataElements
            {
                XsrfToken = JsonSearcher.FindStringValueByKey(jobject, "XSRF_TOKEN"),

                TrackingParams = handler.GetJTokenValue(jobject, "contents", "twoColumnWatchNextResults", "results"
                    , "results", "contents", 0, "videoPrimaryInfoRenderer", "videoActions", "menuRenderer",
                    "trackingParams"),

                LikeParams = handler.GetJTokenValue(jobject, "contents", "twoColumnWatchNextResults", "results"
                    , "results", "contents", 1, "videoPrimaryInfoRenderer", "videoActions", "menuRenderer",
                    "topLevelButtons", 0,
                    "toggleButtonRenderer", "defaultServiceEndpoint", "likeEndpoint", "likeParams"),

                DislikeParams = handler.GetJTokenValue(jobject, "contents", "twoColumnWatchNextResults", "results"
                    , "results", "contents", 1, "videoPrimaryInfoRenderer", "videoActions", "menuRenderer",
                    "topLevelButtons", 1,
                    "toggleButtonRenderer", "defaultServiceEndpoint", "likeEndpoint", "dislikeParams"),

                ClickTrackingParams = handler.GetJTokenValue(jobject, "responseContext", "webResponseContextExtensionData",
                    "webPrefetchData", "navigationEndpoints", 0, "clickTrackingParams"),

                //UploadButtonClickTracking =
                //    handler2.GetElementValue("UPLOAD_BTN_CLICKTRACKING").Split('&').First().Split('=')[1].Trim(),

                Csn = handler.GetJTokenValue(jobject, "responseContext", "webResponseContextExtensionData", "ytConfigData",
                    "csn"),

                ContinuationToken = handler.GetJTokenValue(jobject, "contents", "twoColumnWatchNextResults", "results",
                    "results", "contents", 2, "itemSectionRenderer", "continuations", 0, "nextContinuationData",
                    "continuation"),

                CreateCommentParams = handler.GetJTokenValue(jobject, "contents", "twoColumnWatchNextResults", "results",
                    "results", "contents", 2,
                    "itemSectionRenderer", "header", "commentsHeaderRenderer", "createRenderer",
                    "commentSimpleboxRenderer", "submitButton", "buttonRenderer",
                    "serviceEndpoint", "createCommentEndpoint", "createCommentParams"),

                ReportFromEndPointParams = handler.GetJTokenValue(jobject, "contents", "twoColumnWatchNextResults", "results",
                    "results", "contents", 0, "videoPrimaryInfoRenderer", "videoActions", "menuRenderer", "items", 0,
                    "menuServiceItemRenderer", "serviceEndpoint", "getReportFormEndpoint", "params"),

                InnertubeApiKey = JsonSearcher.FindStringValueByKey(jobject, "INNERTUBE_API_KEY")
            };

            if (string.IsNullOrEmpty(YoutubePost.PostDataElements.ContinuationToken))
                YoutubePost.PostDataElements.ContinuationToken = handler.GetJTokenValue(jobject, "contents",
                    "twoColumnWatchNextResults", "secondaryResults", "secondaryResults", "continuations", 0,
                    "nextContinuationData", "continuation");
            if (string.IsNullOrEmpty(YoutubePost.PostDataElements.CreateCommentParams))
                YoutubePost.PostDataElements.CreateCommentParams = handler.GetJTokenValue(jobject, "contents",
                    "twoColumnWatchNextResults", "results", "results", "contents", 3, "itemSectionRenderer", "header",
                    "commentsHeaderRenderer", "createRenderer", "commentSimpleboxRenderer", "submitButton",
                    "buttonRenderer", "serviceEndpoint", "createCommentEndpoint", "createCommentParams");
            if (string.IsNullOrEmpty(YoutubePost.PostDataElements.TrackingParams))
                YoutubePost.PostDataElements.TrackingParams = handler.GetJTokenValue(jobject, "contents",
                    "twoColumnWatchNextResults", "results", "results", "contents", 0, "videoPrimaryInfoRenderer",
                    "trackingParams");
            if (string.IsNullOrEmpty(YoutubePost.PostDataElements.TrackingParams))
                YoutubePost.PostDataElements.TrackingParams = handler.GetJTokenValue(jobject, "contents",
                    "twoColumnWatchNextResults", "results", "results", "contents", 3, "itemSectionRenderer", "header",
                    "commentsHeaderRenderer", "createRenderer", "commentSimpleboxRenderer", "submitButton",
                    "buttonRenderer", "serviceEndpoint", "clickTrackingParams");
            if (string.IsNullOrEmpty(YoutubePost.PostDataElements.ReportFromEndPointParams))
                YoutubePost.PostDataElements.ReportFromEndPointParams = handler.GetJTokenValue(jobject, "contents",
                    "twoColumnWatchNextResults", "results", "results", "contents", 1, "videoPrimaryInfoRenderer",
                    "videoActions", "menuRenderer", "items", 0, "menuServiceItemRenderer", "serviceEndpoint",
                    "getReportFormEndpoint", "params");


            YoutubePost.HeadersElements = new HeadersElements
            {
                PageBuildLabel = handler.GetJTokenValue(jobject, "PAGE_BUILD_LABEL"),
                VariantsChecksum =
                    handler.GetJTokenValue(jobject, "LATEST_ECATCHER_SERVICE_TRACKING_PARAMS", "VARIANTS_CHECKSUM"),
                PageCl = handler.GetJTokenValue(jobject, "SPACECAST_SETTINGS", "PAGE_CL"),
                ClientName = handler.GetJTokenValue(jobject, "EXPERIMENT_FLAGS", "INNERTUBE_CONTEXT_CLIENT_NAME"),
                ClientVersion = handler.GetJTokenValue(jobject, "SBOX_SETTINGS", "INNERTUBE_CONTEXT_CLIENT_VERSION"),
                IdToken = JsonSearcher.FindStringValueByKey(jobject, "ID_TOKEN"),
                RefererUrl = YoutubePost.PostUrl,
                XYouTubeDevice = Utilities.ReplaceUniCode(handler.GetJTokenValue(jobject, "DEVICE")), //Utilities.ReplaceUniCode(Utilities.GetBetween(jSonString, "\"cbr=", "\"")),
            };

            if (string.IsNullOrEmpty(YoutubePost.HeadersElements.VariantsChecksum))
                YoutubePost.HeadersElements.VariantsChecksum = JsonSearcher.FindStringValueByKey(jobject, "VARIANTS_CHECKSUM");
            if (string.IsNullOrEmpty(YoutubePost.HeadersElements.PageBuildLabel))
                YoutubePost.HeadersElements.PageBuildLabel = JsonSearcher.FindStringValueByKey(jobject, "PAGE_BUILD_LABEL");
            if (string.IsNullOrEmpty(YoutubePost.HeadersElements.PageCl))
                YoutubePost.HeadersElements.PageCl = JsonSearcher.FindStringValueByKey(jobject, "PAGE_CL");
            if (string.IsNullOrEmpty(YoutubePost.HeadersElements.ClientName))
                YoutubePost.HeadersElements.ClientName = JsonSearcher.FindStringValueByKey(jobject, "INNERTUBE_CONTEXT_CLIENT_NAME");
            if (string.IsNullOrEmpty(YoutubePost.HeadersElements.ClientVersion))
                YoutubePost.HeadersElements.ClientVersion =
                    JsonSearcher.FindStringValueByKey(jobject, "INNERTUBE_CONTEXT_CLIENT_VERSION");
            // in scraping Comments
            // continuation = Utilities.GetBetween(response, "\"itemSectionRenderer\":{\"continuations\":[{\"nextContinuationData\":{\"continuation\":\"", "\"");
            //  trackingParams = Utilities.GetBetween(response, continuation + "\",\"clickTrackingParams\":\"", "\"");
            // string csn = Utilities.GetBetween(response, "\"EVENT_ID\":\"", "\"");
        }
    }
}