using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDResponse.BaseResponse;
using FaceDominatorCore.Interface;
using FaceDominatorCore.Utility;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace FaceDominatorCore.FDResponse.BrowserResponseHandler.PostResponseHandler
{
    public class BrowserPostDetailsResponseHandler : FdResponseHandler, IResponseHandler
    {
        public bool HasMoreResults { get; set; }

        public string EntityId { get; set; }

        public string PageletData { get; set; }

        public bool Status { get; set; }

        public FdScraperResponseParameters ObjFdScraperResponseParameters { get; set; }

        public FacebookAdsDetails FacebookAdsDetails { get; set; }
        public BrowserPostDetailsResponseHandler(IResponseParameter responseParameter, List<string> jsonStringresponse
           , FacebookPostDetails postDetails, bool isWatchVideo = false, bool isClassicUi = true) : base(responseParameter)
        {
            ObjFdScraperResponseParameters = new FdScraperResponseParameters();
            FacebookAdsDetails = new FacebookAdsDetails();
            if (jsonStringresponse.Count == 0 && !string.IsNullOrEmpty(responseParameter.Response))
                jsonStringresponse.Add(responseParameter.Response);
            try
            {
                foreach (var postResponse in jsonStringresponse)
                {
                    JObject jObject = new JObject();
                    JArray fdjArray = new JArray();
                    var elements = new JArray();
                    try
                    {
                        if (!postResponse.IsValidJson())
                        {
                            var decodedResponse = "";
                            if (postResponse.StartsWith("<!DOCTYPE html>"))
                            {
                                var splittedArray = "";
                                if (postDetails.PostUrl.Contains("watch/?v=") || postDetails.PostUrl.Contains("/videos/"))
                                    splittedArray = Utilities.GetBetween(Regex.Split(postResponse, "</script>").FirstOrDefault(x => x.Contains("{\"__typename\":\"Video\"") && x.Contains("creation_time")) + "/end", "{\"require\"", "/end");
                                else
                                    try
                                    {
                                        splittedArray = Utilities.GetBetween(Regex.Split(postResponse, "</script>").SingleOrDefault(x => x.Contains("{\"__typename\":\"Story\"")) + "/end", "{\"require\"", "/end");
                                    }
                                    catch (Exception)
                                    {
                                        splittedArray = Utilities.GetBetween(Regex.Split(postResponse, "</script>").FirstOrDefault(x => x.Contains("{\"__typename\":\"Video\"") && x.Contains("creation_time")) + "/end", "{\"require\"", "/end");
                                        if (string.IsNullOrEmpty(splittedArray))
                                            splittedArray = Utilities.GetBetween(Regex.Split(postResponse, "</script>").FirstOrDefault(x => x.Contains("{\"__typename\":\"Story\"")) + "/end", "{\"require\"", "/end");
                                    }

                                decodedResponse = "{\"require\"" + splittedArray;
                                if (decodedResponse.IsValidJson())
                                    jObject = parser.ParseJsonToJObject(decodedResponse);
                            }
                            else
                            {
                                decodedResponse = "[" + postResponse.Replace("}}}}\r\n{\"label\":", "}}}},\r\n{\"label\":") + "]";
                                if(!decodedResponse.IsValidJson())
                                    decodedResponse= "[" + postResponse.Replace("}}\r\n{\"label\":", "}},\r\n{\"label\":") + "]";
                            }
                            fdjArray = parser.GetJArrayElement(decodedResponse);
                        }
                        else
                        {
                            fdjArray = parser.GetJArrayElement(postResponse);
                            if (fdjArray.Count() == 0)
                                jObject = parser.ParseJsonToJObject(postResponse);
                        }
                        if (jObject != null && jObject.Count > 0)
                        {
                            var nodes = parser.GetJTokenOfJToken(jObject, "data", "serpResponse", "results", "edges");
                            if (nodes == null || nodes.Count() == 0)
                                nodes = parser.GetJTokenOfJToken(jObject, "data", "node", "group_feed", "edges");
                            if (nodes == null || nodes.Count() == 0)
                                nodes = parser.GetJTokenOfJToken(jObject, "data", "node", "timeline_list_feed_units", "edges");
                            if (nodes == null || nodes.Count() == 0)
                                nodes = parser.GetJTokenOfJToken(jObject, "data", "feedback", "reshares", "edges");
                            if (nodes == null || nodes.Count() == 0)
                                nodes = parser.GetJTokenOfJToken(jObject, "data", "node");
                            if (nodes == null || nodes.Count() == 0)
                                nodes = JsonSearcher.FindByKey(JsonSearcher.FindByKey(jObject, "data"), "video");
                            if (nodes == null || nodes.Count() == 0)
                                nodes = JsonSearcher.FindByKey(JsonSearcher.FindByKey(jObject, "data"), "attachments");
                            if (nodes == null || nodes.Count() == 0)
                                nodes = JsonSearcher.FindByKey(jObject, "node");
                            if (nodes == null || nodes.Count() == 0)
                                nodes = JsonSearcher.FindByKey(jObject, "video");
                            if (nodes == null || nodes.Count() == 0)
                                nodes = parser.GetJTokenOfJToken(jObject, "require", 0, 3, 0, "__bbox", "require", 73, 3, 1, "__bbox", "result", "data", "node");
                            if (nodes != null || nodes.Count() != 0)
                            {
                                if (nodes.Type == JTokenType.Array)
                                {
                                    foreach (var nodepost in nodes)
                                        elements.Add(nodepost);
                                }
                                else
                                    elements.Add(nodes);
                            }
                        }
                        foreach (var node in fdjArray)
                        {
                            var nodes = parser.GetJTokenOfJToken(node, "data", "serpResponse", "results", "edges");
                            if (nodes == null || nodes.Count() == 0)
                                nodes = parser.GetJTokenOfJToken(node, "data", "node", "group_feed", "edges");
                            if (nodes == null || nodes.Count() == 0)
                                nodes = parser.GetJTokenOfJToken(node, "data", "node", "timeline_list_feed_units", "edges");
                            if (nodes == null || nodes.Count() == 0)
                                nodes = parser.GetJTokenOfJToken(node, "data", "feedback", "reshares", "edges");
                            if (nodes == null || nodes.Count() == 0)
                                nodes = parser.GetJTokenOfJToken(node, "data", "node");
                            if (nodes != null || nodes.Count() != 0)
                            {
                                foreach (var nodepost in nodes)
                                    elements.Add(nodepost);
                            }
                        }
                    }
                    catch (Exception)
                    {
                    }
                    foreach (var postObject in elements)
                    {
                        var storyObject = postObject;
                        var nodeObject = JsonSearcher.FindByKey(postObject, "node");
                        if (nodeObject.Count() == 0)
                            nodeObject = JsonSearcher.FindByKey(postObject, "media");
                        if (nodeObject.Count() == 0)
                            nodeObject = JsonSearcher.FindByKey(postObject, "creation_story");
                        if (nodeObject.Count() > 0 && (JsonSearcher.FindStringValueByKey(nodeObject, "__typename") == "Story" || JsonSearcher.FindStringValueByKey(nodeObject, "__typename") == "Video"))
                            storyObject = nodeObject;
                        var caption = parser.GetJTokenValue(storyObject, "relay_rendering_strategy", "view_model", "click_model", "story", "comet_sections", "content", "story", "comet_sections", "message", "story", "message", "text");
                        if (string.IsNullOrEmpty(caption))
                            caption = parser.GetJTokenValue(storyObject, "comet_sections", "content", "story", "comet_sections", "message", "story", "message", "text");
                        if (string.IsNullOrEmpty(caption))
                            caption = parser.GetJTokenValue(storyObject, "comet_sections", "message", "story", "message", "text");
                        if (string.IsNullOrEmpty(caption))
                            caption = JsonSearcher.FindStringValueByKey(JsonSearcher.FindByKey(storyObject, "message"), "text");
                        var storyJToken = parser.GetJTokenOfJToken(storyObject, "relay_rendering_strategy", "view_model", "click_model", "story", "comet_sections", "content", "story", "comet_sections", "context_layout", "story", "comet_sections", "metadata");
                        if (storyJToken.Count() == 0)
                            storyJToken = parser.GetJTokenOfJToken(storyObject, "relay_rendering_strategy", "view_model", "click_model", "story", "comet_sections", "context_layout", "story", "comet_sections", "metadata");
                        if (storyJToken.Count() == 0)
                            storyJToken = parser.GetJTokenOfJToken(storyObject, "comet_sections", "content", "story", "comet_sections", "context_layout", "story", "comet_sections", "metadata");
                        if (storyJToken.Count() == 0)
                            storyJToken = parser.GetJTokenOfJToken(storyObject, "comet_sections", "context_layout", "story", "comet_sections", "metadata");
                        if (storyJToken.Count() == 0)
                            storyJToken = JsonSearcher.FindByKey(postObject, "metadata");
                        if (storyJToken.Count() == 0)
                            storyJToken = JsonSearcher.FindByKey(postObject, "short_form_video_context");
                        var postId = parser.GetJTokenValue(postObject, "relay_rendering_strategy", "view_model", "click_model", "story", "post_id");
                        if (string.IsNullOrEmpty(postId))
                            postId = parser.GetJTokenValue(storyObject, "post_id");
                        if (string.IsNullOrEmpty(postId))
                            postId = parser.GetJTokenValue(storyObject, "feedback", "id");
                        if (string.IsNullOrEmpty(postId))
                            postId = parser.GetJTokenValue(storyJToken, 0, "story", "id");
                        if (string.IsNullOrEmpty(postId))
                            postId = parser.GetJTokenValue(storyJToken, "video", "id");
                        var url = parser.GetJTokenValue(storyJToken, 0, "story", "url");
                        if (string.IsNullOrEmpty(url))
                            url = parser.GetJTokenValue(storyJToken, "shareable_url");
                        if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(postId))
                        {
                            foreach (var token in storyJToken)
                            {
                                var tempUrl = parser.GetJTokenValue(token, "story", "url");
                                var tempId = parser.GetJTokenValue(token, "story", "id");
                                if (!string.IsNullOrEmpty(tempUrl) && !string.IsNullOrEmpty(tempId))
                                { url = tempUrl; postId = tempId; break; }
                            }
                        }

                        if (!string.IsNullOrEmpty(postId) && !FdFunctions.IsIntegerOnly(postId))
                            postId = StringHelper.Base64Decode(postId).Split(':').LastOrDefault();
                        if (string.IsNullOrEmpty(url))
                            url = $"{FdConstants.FbHomeUrl}{postId}";
                        var ownerToken = parser.GetJTokenOfJToken(storyObject, "relay_rendering_strategy", "view_model", "click_model", "story", "comet_sections", "content", "story", "comet_sections", "context_layout", "story", "comet_sections", "actor_photo", "story", "actors");
                        if (ownerToken.Count() == 0)
                            ownerToken = parser.GetJTokenOfJToken(storyObject, "relay_rendering_strategy", "view_model", "click_model", "story", "comet_sections", "context_layout", "story", "comet_sections", "actor_photo", "story", "actors");
                        if (ownerToken.Count() == 0)
                            ownerToken = parser.GetJTokenOfJToken(storyObject, "comet_sections", "content", "story", "comet_sections", "context_layout", "story", "comet_sections", "actor_photo", "story", "actors");
                        if (ownerToken.Count() == 0)
                            ownerToken = parser.GetJTokenOfJToken(storyObject, "comet_sections", "context_layout", "story", "comet_sections", "actor_photo", "story", "actors");
                        if (ownerToken.Count() == 0)
                            ownerToken = JsonSearcher.FindByKey(JsonSearcher.FindByKey(postObject, "actor_photo"), "actors");
                        if (ownerToken.Count() == 0)
                            ownerToken = JsonSearcher.FindByKey(postObject, "video_owner");
                        var ownerId = parser.GetJTokenValue(ownerToken, 0, "id");
                        if (string.IsNullOrEmpty(ownerId))
                            ownerId = parser.GetJTokenValue(ownerToken, "id");

                        var ownerName = parser.GetJTokenValue(ownerToken, 0, "name");
                        if (string.IsNullOrEmpty(ownerName))
                            ownerName = parser.GetJTokenValue(ownerToken, "name");
                        var ownerUrl = parser.GetJTokenValue(ownerToken, 0, "url");

                        if (string.IsNullOrEmpty(ownerUrl))
                            ownerUrl = parser.GetJTokenValue(ownerToken, "url");

                        var _shareCount = parser.GetJTokenValue(storyObject, "relay_rendering_strategy", "view_model", "click_model", "story", "comet_sections", "feedback", "story", "comet_feed_ufi_container", "story", "feedback_context", "feedback_target_with_context", "ufi_renderer", "feedback", "comet_ufi_summary_and_actions_renderer", "feedback", "share_count", "count");
                        if (string.IsNullOrEmpty(_shareCount))
                            _shareCount = parser.GetJTokenValue(storyObject, "comet_sections", "feedback", "story", "comet_feed_ufi_container", "story", "feedback_context", "feedback_target_with_context", "ufi_renderer", "feedback", "comet_ufi_summary_and_actions_renderer", "feedback", "share_count", "count");
                        if (string.IsNullOrEmpty(_shareCount))
                            _shareCount = JsonSearcher.FindStringValueByKey(JsonSearcher.FindByKey(storyObject, "share_count"), "count");
                        if (string.IsNullOrEmpty(_shareCount))
                            _shareCount = Utilities.GetBetween(Regex.Split(jsonStringresponse.FirstOrDefault() ?? "", "</script>").FirstOrDefault(x => x.Contains("share_count_reduced") && (x.Contains(url.Replace("/", "\\/")) || x.Contains($"\"post_id\":\"{postId}\"") || x.Contains($"\\\"post_id\\\":\\\"{postId}\\\""))) ?? "", "\"share_count_reduced\":\"", "\"");

                        var _commentCount = JsonSearcher.FindStringValueByKey(storyObject, "total_comment_count");
                        if (string.IsNullOrEmpty(_commentCount))
                            _commentCount = JsonSearcher.FindStringValueByKey(JsonSearcher.FindByKey(storyObject, "comments"), "total_count");
                        if (string.IsNullOrEmpty(_commentCount))
                            _commentCount = Utilities.GetBetween(jsonStringresponse.FirstOrDefault() ?? "", "\"comments\":{\"total_count\":", "}");
                        var commentCountOfreel = Utilities.GetBetween(Regex.Split(jsonStringresponse.FirstOrDefault() ?? "", "</script>").FirstOrDefault(x => x.Contains("share_count_reduced") && (x.Contains(url.Replace("/", "\\/")) || x.Contains($"\"post_id\":\"{postId}\"") || x.Contains($"\\\"post_id\\\":\\\"{postId}\\\""))) ?? "", "\"total_comment_count\":", ",");
                        if (url.Contains("/reel/") || !string.IsNullOrEmpty(commentCountOfreel))
                            _commentCount = commentCountOfreel;



                        var reactionCount = 0;
                        var reactionElements = parser.GetJTokenOfJToken(storyObject, "comet_sections", "feedback", "story", "comet_feed_ufi_container", "story", "feedback_context", "feedback_target_with_context", "ufi_renderer", "feedback", "comet_ufi_summary_and_actions_renderer", "feedback", "top_reactions", "edges");
                        if (reactionElements.Count() == 0)
                            reactionElements = JsonSearcher.FindByKey(JsonSearcher.FindByKey(JsonSearcher.FindByKey(storyObject, "comet_ufi_summary_and_actions_renderer"), "top_reactions"), "edges");

                        foreach (var reactionType in reactionElements)
                        {
                            if (int.TryParse(parser.GetJTokenValue(reactionType, "reaction_count"), out int tempCount))
                                reactionCount += tempCount;
                        }
                        if (reactionCount == 0)
                        {
                            if (int.TryParse(JsonSearcher.FindStringValueByKey(JsonSearcher.FindByKey(storyObject, "unified_reactors"), "count"), out int tempCount))
                                reactionCount += tempCount;
                        }
                        if (reactionCount == 0)
                        {
                            if (int.TryParse(Utilities.GetBetween(jsonStringresponse.FirstOrDefault() ?? "", "reaction_count\":{\"count\":", ","), out int tempCount))
                                reactionCount += tempCount;
                        }
                        if (reactionCount == 0)
                        {
                            if (int.TryParse(Utilities.GetBetween(Regex.Split(jsonStringresponse.FirstOrDefault() ?? "", "</script>").FirstOrDefault(x => x.Contains("share_count_reduced") && (x.Contains(url.Replace("/", "\\/")) || x.Contains($"\"post_id\":\"{postId}\"") || x.Contains($"\\\"post_id\\\":\\\"{postId}\\\""))) ?? "", "\"unified_reactors\":{\"count\":", "}"), out int tempCount))
                                reactionCount += tempCount;
                        }
                        if (reactionCount == 0)
                        {
                            if (int.TryParse(Utilities.GetBetween(Regex.Split(jsonStringresponse.FirstOrDefault() ?? "", "</script>").FirstOrDefault(x => x.Contains("share_count_reduced") && (x.Contains(url.Replace("/", "\\/")) || x.Contains($"\"post_id\":\"{postId}\"") || x.Contains($"\\\"post_id\\\":\\\"{postId}\\\""))) ?? "", "\"unified_reactors\":{\"count\":", ","), out int tempCount))
                                reactionCount += tempCount;
                        }
                        var postDateTime = parser.GetJTokenValue(storyJToken, 0, "story", "creation_time");
                        if (string.IsNullOrEmpty(postDateTime))
                            postDateTime = JsonSearcher.FindStringValueByKey(storyJToken, "creation_time");
                        if (string.IsNullOrEmpty(postDateTime))
                            postDateTime = JsonSearcher.FindStringValueByKey(storyObject, "creation_time");
                        var media = JsonSearcher.FindByKey(storyObject, "media");
                        var mediaurl = "";
                        var mediaTypestring = "";
                        if (media != null && media.Count() > 0)
                        {
                            mediaTypestring = JsonSearcher.FindStringValueByKey(media, "__typename");
                            if (mediaTypestring == "Video")
                                mediaurl = JsonSearcher.FindStringValueByKey(media, "url");
                            else if (mediaTypestring == "Photo")
                                mediaurl = JsonSearcher.FindStringValueByKey(media, "uri");
                        }
                        if (string.IsNullOrEmpty(mediaTypestring) && media.Count() == 0)
                            mediaTypestring = JsonSearcher.FindStringValueByKey(nodeObject, "__typename");
                        var _mediaType = mediaTypestring == "Photo" ? MediaType.Image : mediaTypestring == "Video" || url.Contains("/reel/") ? MediaType.Video : MediaType.NoMedia;
                        if (string.IsNullOrEmpty(mediaurl) && _mediaType == MediaType.Video)
                            mediaurl = url;
                        var CommentDisabled = parser.GetJTokenValue(storyObject, "comet_sections", "feedback", "story", "comet_feed_ufi_container", "story", "feedback_context", "feedback_target_with_context", "ufi_renderer", "feedback", "comment_list_renderer", "feedback", "have_comments_been_disabled");
                        if (string.IsNullOrEmpty(CommentDisabled))
                            CommentDisabled = JsonSearcher.FindStringValueByKey(storyObject, "have_comments_been_disabled");
                        if (string.IsNullOrEmpty(CommentDisabled))
                            CommentDisabled = Utilities.GetBetween(jsonStringresponse.FirstOrDefault() ?? "", "\"have_comments_been_disabled\":", ",");


                        if (postDetails == null)
                        {
                            postDetails = new FacebookPostDetails()
                            {
                                Caption = caption,
                                Id = postId,
                                PostUrl = url,
                                OwnerId = ownerId,
                                OwnerName = ownerName,
                                LikersCount = reactionCount.ToString(),
                                CommentorCount = !string.IsNullOrEmpty(_commentCount) && FdFunctions.IsIntegerOnly(_commentCount) ? _commentCount : "0",
                                SharerCount = !string.IsNullOrEmpty(_shareCount) && FdFunctions.IsIntegerOnly(_shareCount) ? _shareCount : "0",
                                PostedBy = ownerName,
                                PostedDateTime = int.TryParse(postDateTime, out int creationtime) ? DateTimeUtilities.EpochToDateTimeLocal(creationtime) : new DateTime(),
                                ScapedUrl = $"{FdConstants.FbHomeUrl}{postId}",
                                FullPostUrl = url,
                                MediaUrl = mediaurl,
                                MediaType = mediaTypestring == "Photo" ? MediaType.Image : mediaTypestring == "Video" || url.Contains("/reel/") ? MediaType.Video : MediaType.NoMedia,
                                EntityId = ownerId,
                                EntityName = ownerName,
                                CanComment = CommentDisabled.ToLower().Contains("true") ? false : true
                            };

                        }
                        else
                        {
                            if (string.IsNullOrEmpty(postDetails.Caption) && !string.IsNullOrEmpty(caption))
                                postDetails.Caption = caption;
                            if (string.IsNullOrEmpty(postDetails.Id) && !string.IsNullOrEmpty(postId))
                                postDetails.Id = postId;
                            if (string.IsNullOrEmpty(postDetails.PostUrl) && !string.IsNullOrEmpty(url))
                                postDetails.PostUrl = url;
                            if (string.IsNullOrEmpty(postDetails.OwnerId) && !string.IsNullOrEmpty(ownerId))
                                postDetails.OwnerId = ownerId;
                            if (string.IsNullOrEmpty(postDetails.OwnerName) && !string.IsNullOrEmpty(ownerName))
                                postDetails.OwnerName = ownerName;
                            if (string.IsNullOrEmpty(postDetails.LikersCount) || (int.TryParse(postDetails.LikersCount, out int liker) && liker == 0))
                                postDetails.LikersCount = reactionCount.ToString();
                            if (string.IsNullOrEmpty(postDetails.CommentorCount) || (int.TryParse(postDetails.CommentorCount, out int commenter) && commenter == 0))
                                postDetails.CommentorCount =
                                    !string.IsNullOrEmpty(_commentCount) && FdFunctions.IsIntegerOnly(_commentCount)
                                        ? _commentCount
                                        : "0";
                            if (string.IsNullOrEmpty(postDetails.SharerCount) || (int.TryParse(postDetails.SharerCount, out int sharer) && sharer == 0))
                                postDetails.SharerCount =
                                !string.IsNullOrEmpty(_shareCount) && FdFunctions.IsIntegerOnly(_shareCount)
                                    ? _shareCount
                                    : "0";
                            if (string.IsNullOrEmpty(postDetails.PostedBy) && !string.IsNullOrEmpty(ownerName))
                                postDetails.PostedBy = ownerName;
                            if (postDetails.PostedDateTime == new DateTime())
                                postDetails.PostedDateTime = int.TryParse(postDateTime, out int creationtime) ? DateTimeUtilities.EpochToDateTimeLocal(creationtime) : postDetails.PostedDateTime;
                            if (string.IsNullOrEmpty(postDetails.ScapedUrl) && !string.IsNullOrEmpty(postId))
                                postDetails.ScapedUrl = $"{FdConstants.FbHomeUrl}{postId}";
                            if (string.IsNullOrEmpty(postDetails.FullPostUrl) && !string.IsNullOrEmpty(url))
                                postDetails.FullPostUrl = url;
                            if (string.IsNullOrEmpty(postDetails.MediaUrl) && !string.IsNullOrEmpty(mediaurl))
                                postDetails.MediaUrl = mediaurl;
                            if (!string.IsNullOrEmpty(mediaTypestring))
                                postDetails.MediaType = mediaTypestring == "Photo" ? MediaType.Image : mediaTypestring == "Video" || url.Contains("/reel/") ? MediaType.Video : MediaType.NoMedia;
                            if (string.IsNullOrEmpty(postDetails.EntityId) && !string.IsNullOrEmpty(ownerId))
                                postDetails.EntityId = ownerId;
                            if (string.IsNullOrEmpty(postDetails.EntityName) && !string.IsNullOrEmpty(ownerName))
                                postDetails.EntityName = ownerName;
                            if (!string.IsNullOrEmpty(CommentDisabled))
                                postDetails.CanComment = CommentDisabled.ToLower().Contains("true") ? false : true;
                        }


                    }

                }

            }
            catch (Exception)
            { }
            ObjFdScraperResponseParameters.PostDetails = postDetails;
            if (jsonStringresponse.Count > 0)
                Status = true;
            ObjFdScraperResponseParameters.IsCommentedOnPost = ObjFdScraperResponseParameters.PostDetails.IsActorChangeable;

        }
    }
}
