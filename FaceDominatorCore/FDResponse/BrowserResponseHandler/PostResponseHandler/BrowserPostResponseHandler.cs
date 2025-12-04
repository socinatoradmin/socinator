using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDEnums;
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
    public class BrowserPostResponseHandler : FdResponseHandler, IResponseHandler
    {
        public bool HasMoreResults { get; set; }

        public string EntityId { get; set; }

        public string PageletData { get; set; }

        public bool Status { get; set; }

        public FdScraperResponseParameters ObjFdScraperResponseParameters { get; set; }
        public BrowserPostResponseHandler(IResponseParameter response, List<string> listResponse, FbEntityType fbEntityType = FbEntityType.Post, bool hasmoreResults = false) : base(response)
        {
            ObjFdScraperResponseParameters = new FdScraperResponseParameters();

            ObjFdScraperResponseParameters.ListPostDetails = new List<FacebookPostDetails>();

            try
            {
                foreach (var postResponse in listResponse)
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
                                try
                                {
                                    splittedArray = Utilities.GetBetween(Regex.Split(postResponse, "</script>").SingleOrDefault(x => x.Contains("{\"__typename\":\"Story\"")) + "/end", "{\"require\"", "/end");
                                }
                                catch (Exception)
                                {
                                    splittedArray = Utilities.GetBetween(Regex.Split(postResponse, "</script>").FirstOrDefault(x => x.Contains("{\"__typename\":\"Video\"") && x.Contains("creation_time")) + "/end", "{\"require\"", "/end");
                                    if(string.IsNullOrEmpty(splittedArray))
                                        splittedArray = Utilities.GetBetween(Regex.Split(postResponse, "</script>").FirstOrDefault(x => x.Contains("{\"__typename\":\"Story\"")) + "/end", "{\"require\"", "/end");
                                }
                                
                                decodedResponse = "{\"require\"" + splittedArray;
                                if (decodedResponse.IsValidJson())
                                    jObject = parser.ParseJsonToJObject(decodedResponse);
                            }
                            else
                            {
                                decodedResponse = "[" + postResponse.Replace("}}}}\r\n{\"label\":", "}}}},\r\n{\"label\":") + "]";
                                if (!decodedResponse.IsValidJson())
                                    decodedResponse = "[" + postResponse.Replace("}}\r\n{\"label\":", "}},\r\n{\"label\":") + "]";
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
                                nodes = JsonSearcher.FindByKey(jObject, "node");
                            if (nodes == null || nodes.Count() == 0)
                                nodes = JsonSearcher.FindByKey(JsonSearcher.FindByKey(jObject, "data"), "attachments");
                            if (nodes == null || nodes.Count() == 0)
                                nodes = JsonSearcher.FindByKey(jObject, "video");
                            if (nodes == null || nodes.Count() == 0)
                                nodes = parser.GetJTokenOfJToken(jObject, "require", 0, 3, 0, "__bbox", "require", 73, 3, 1, "__bbox", "result", "data", "node");
                            if (nodes != null || nodes.Count() != 0)
                            {
                                if (nodes.Type == JTokenType.Array)
                                {
                                    foreach (var node in nodes)
                                        elements.Add(node);
                                }
                                else
                                    elements.Add(nodes);
                            }
                        }
                        if (fdjArray != null && fdjArray.Count() > 0)
                        {
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
                                    if (nodes.Type == JTokenType.Array)
                                    {
                                        foreach (var nodepost in nodes)
                                            elements.Add(nodepost);
                                    }
                                    else
                                        elements.Add(nodes);
                                }

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
                        var postId = parser.GetJTokenValue(postObject, "relay_rendering_strategy", "view_model", "click_model", "story", "post_id");
                        if (string.IsNullOrEmpty(postId))
                            postId = parser.GetJTokenValue(storyObject, "post_id");
                        if (string.IsNullOrEmpty(postId))
                            postId = parser.GetJTokenValue(storyObject, "feedback", "id");
                        if (string.IsNullOrEmpty(postId))
                            postId = parser.GetJTokenValue(storyJToken, 0, "story", "id");
                        var url = parser.GetJTokenValue(storyJToken, 0, "story", "url");
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
                        if (!string.IsNullOrEmpty(postId) && !FdFunctions.IsIntegerOnly(postId) && StringHelper.IsBase64String(postId))
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
                        var ownerId = parser.GetJTokenValue(ownerToken, 0, "id");
                        var ownerName = parser.GetJTokenValue(ownerToken, 0, "name");

                        var ownerUrl = parser.GetJTokenValue(ownerToken, 0, "url");

                        var _shareCount = parser.GetJTokenValue(storyObject, "relay_rendering_strategy", "view_model", "click_model", "story", "comet_sections", "feedback", "story", "comet_feed_ufi_container", "story", "feedback_context", "feedback_target_with_context", "ufi_renderer", "feedback", "comet_ufi_summary_and_actions_renderer", "feedback", "share_count", "count");
                        if (string.IsNullOrEmpty(_shareCount))
                            _shareCount = parser.GetJTokenValue(storyObject, "comet_sections", "feedback", "story", "comet_feed_ufi_container", "story", "feedback_context", "feedback_target_with_context", "ufi_renderer", "feedback", "comet_ufi_summary_and_actions_renderer", "feedback", "share_count", "count");
                        if (string.IsNullOrEmpty(_shareCount))
                            _shareCount = JsonSearcher.FindStringValueByKey(JsonSearcher.FindByKey(storyObject, "share_count"), "count");
                        var _commentCount = JsonSearcher.FindStringValueByKey(storyObject, "total_comment_count");
                        if (string.IsNullOrEmpty(_commentCount))
                            _commentCount = JsonSearcher.FindStringValueByKey(JsonSearcher.FindByKey(storyObject, "comments"), "total_count");

                        var reactionCount = 0;
                        var reactionElements = parser.GetJTokenOfJToken(storyObject, "comet_sections", "feedback", "story", "comet_feed_ufi_container", "story", "feedback_context", "feedback_target_with_context", "ufi_renderer", "feedback", "comet_ufi_summary_and_actions_renderer", "feedback", "top_reactions", "edges");
                        if (reactionElements.Count() == 0)
                            reactionElements = JsonSearcher.FindByKey(JsonSearcher.FindByKey(storyObject, "top_reactions"), "edges");
                        foreach (var reactionType in reactionElements)
                        {
                            var tempCount = 0;
                            if (int.TryParse(parser.GetJTokenValue(reactionType, "reaction_count"), out tempCount))
                                reactionCount += tempCount;
                        }
                        if (reactionCount == 0)
                        {
                            int tempCount = 0;
                            if (int.TryParse(JsonSearcher.FindStringValueByKey(JsonSearcher.FindByKey(storyObject, "unified_reactors"), "count"), out tempCount))
                                reactionCount += tempCount;
                        }
                        var postDateTime = parser.GetJTokenValue(storyJToken, 0, "story", "creation_time");
                        if (string.IsNullOrEmpty(postDateTime))
                            postDateTime = JsonSearcher.FindStringValueByKey(storyJToken, "creation_time");
                        if (string.IsNullOrEmpty(postDateTime))
                            postDateTime = JsonSearcher.FindStringValueByKey(storyObject, "creation_time");
                        var postTime = int.TryParse(postDateTime, out int creationtime) ? DateTimeUtilities.EpochToDateTimeLocal(creationtime) : new DateTime();
                        var media = JsonSearcher.FindByKey(JsonSearcher.FindByKey(storyObject, "story"), "media");
                        if (media.Count() == 0)
                            media = JsonSearcher.FindByKey(JsonSearcher.FindByKey(JsonSearcher.FindByKey(storyObject, "feedback_context"), "story"), "media");

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
                        var _mediaType = mediaTypestring == "Photo" ? MediaType.Image : mediaTypestring == "Video" || url.Contains("/reel/")||(postObject.ToString().Contains($"\\\"top_level_post_id\\\":\\\"{postId}\\\"") && postObject.ToString().Contains("\\\"video_id\\\":\\\"")) ? MediaType.Video : MediaType.NoMedia;
                        var CommentDisabled = parser.GetJTokenValue(storyObject, "comet_sections", "feedback", "story", "comet_feed_ufi_container", "story", "feedback_context", "feedback_target_with_context", "ufi_renderer", "feedback", "comment_list_renderer", "feedback", "have_comments_been_disabled");
                        if (string.IsNullOrEmpty(CommentDisabled))
                            CommentDisabled = JsonSearcher.FindStringValueByKey(storyObject, "have_comments_been_disabled");
                        if (ObjFdScraperResponseParameters.ListPostDetails.Any(x => postId != "" && x.Id == postId)||(string.IsNullOrEmpty(postId)||string.IsNullOrEmpty(url)||url.Equals(FdConstants.FbHomeUrl))) continue;

                        ObjFdScraperResponseParameters.ListPostDetails.Add(new FacebookPostDetails()
                        {
                            Caption = caption,
                            Id = postId,
                            PostUrl = url,
                            OwnerId = ownerId,
                            OwnerName = ownerName,
                            LikersCount = reactionCount.ToString(),
                            CommentorCount = string.IsNullOrEmpty(_commentCount) ? "0" : _commentCount,
                            SharerCount = string.IsNullOrEmpty(_shareCount) ? "0" : _shareCount,
                            PostedBy = ownerName,
                            PostedDateTime = postTime,
                            ScapedUrl = $"{FdConstants.FbHomeUrl}{postId}",
                            FullPostUrl = url,
                            MediaUrl = string.IsNullOrEmpty(mediaurl) && _mediaType == MediaType.Video ? url : mediaurl,
                            MediaType = _mediaType,
                            EntityId = ownerId,
                            EntityName = ownerName,
                            CanComment = CommentDisabled.ToLower().Contains("true") ? false : true
                        }
                         );
                    }
                }
                Status = ObjFdScraperResponseParameters.ListPostDetails.Count > 0;
            }
            catch (Exception)
            { }

            HasMoreResults = hasmoreResults;
        }

    }
}
