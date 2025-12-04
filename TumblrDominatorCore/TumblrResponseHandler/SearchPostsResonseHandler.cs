using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using TumblrDominatorCore.Models;
using TumblrDominatorCore.TmblrUtility;

namespace TumblrDominatorCore.TumblrResponseHandler
{
    public class SearchPostsResonseHandler : ResponseHandler
    {
        public SearchPostsResonseHandler()
        {

        }
        public SearchPostsResonseHandler(bool isSuccess)
        {
            Success = isSuccess;
        }
        public SearchPostsResonseHandler(IResponseParameter responeParameter) : base(responeParameter)
        {
            if (responeParameter == null || string.IsNullOrEmpty(responeParameter.Response)) return;
            #region Posts
            try
            {
                var response = WebUtility.HtmlDecode(responeParameter.Response);
                if (response.Contains("{\"meta\":{\"status\":200,\"msg\":\"OK\"") ||
                     response.Contains("\"queryKey\":[\"user-info\",true]"))
                {
                    Success = true;
                    JObject jObject = null;
                    try
                    {
                        if (!response.IsValidJson())
                        {
                            var decodedResponse = TumblrUtility.GetDecodedResponseOfJson(response)?.Replace("undefined", "\"\"");
                            jObject = parser.ParseJsonToJObject(decodedResponse);
                            if (jObject == null)
                                jObject = parser.ParseJsonToJObject(TumblrUtility.GetJsonFromPageResponse(decodedResponse));
                        }
                        else if (jObject == null || response.StartsWith("<!DOCTYPE html>"))
                        {
                            jObject = parser.ParseJsonToJObject(response);
                            if (jObject is null)
                                jObject = parser.ParseJsonToJObject(TumblrUtility.GetJsonFromPageResponse(response));
                        }
                        else
                            jObject = parser.ParseJsonToJObject(response);
                    }
                    catch (Exception e)
                    {
                        e.DebugLog();
                    }
                    var elements = parser.GetJArrayElement(parser.GetJTokenValue(jObject, "response", "timeline", "elements"));
                    if (elements == null || elements?.Count == 0) elements = parser.GetJArrayElement(parser.GetJTokenValue(jObject, "PeeprRoute", "initialTimeline", "objects"));
                    if (elements == null || elements?.Count == 0) elements = parser.GetJArrayElement(parser.GetJTokenValue(jObject, "initialTimeline", "objects"));
                    if (elements == null || elements?.Count == 0) elements = parser.GetJArrayElement(parser.GetJTokenValue(jObject, "BlogView", "objects"));
                    if (elements == null || elements?.Count == 0)
                        elements = parser.GetJArrayElement(parser.GetJTokenValue(jObject, "SearchRoute", "timelines", "post", "response", "timeline", "elements"));
                    foreach (var element in elements)
                    {
                        try
                        {
                            #region Post Data.
                            var post = new TumblrPost();
                            post.PlacementId = parser.GetJTokenValue(element, "placementId");
                            post.Uuid = parser.GetJTokenValue(element, "blog", "uuid");
                            post.BlogName = parser.GetJTokenValue(element, "blogName");
                            post.Id = parser.GetJTokenValue(element, "id");
                            if (!string.IsNullOrEmpty(post.Id) && post.Id.Contains("timelineObject"))
                                continue;
                            post.OwnerUsername = parser.GetJTokenValue(element, "blog", "name");
                            post.NotesCount = parser.GetJTokenValue(element, "noteCount");
                            post.PostKey = parser.GetJTokenValue(element, "reblogKey");
                            post.RebloggedRootId = parser.GetJTokenValue(element, "reblogKey");
                            post.Caption = parser.GetJTokenValue(element, "summary");
                            post.Caption = string.IsNullOrEmpty(post.Caption) ? parser.GetJTokenValue(element, "slug")?.Replace("-", " ") : post.Caption;
                            post.ProfileUrl = parser.GetJTokenValue(element, "blog", "url");
                            post.BlogUrl = parser.GetJTokenValue(element, "blog", "blogViewUrl")?.Replace("u002F", "/");
                            post.PostUrl = parser.GetJTokenValue(element, "postUrl")?.Replace("u002F", "/");
                            post.PostUrl = !string.IsNullOrEmpty(post.BlogUrl) && post.PostUrl.Contains(".com/post/") ? post.BlogUrl + Utilities.GetBetween(post.PostUrl + "/end", ".com/post", "/end") : post.PostUrl;
                            var content = parser.GetJTokenOfJToken(element, "content");
                            bool.TryParse(parser.GetJTokenValue(element, "liked"), out bool isLiked);
                            post.IsLiked = isLiked;
                            bool.TryParse(parser.GetJTokenValue(element, "canLike"), out bool canLike);
                            post.CanLike = canLike;
                            post.LikeCount = parser.GetJTokenValue(element, "likeCount");
                            bool.TryParse(parser.GetJTokenValue(element, "canReply"), out bool canReply);
                            post.CanReply = canReply;
                            post.ReplyCount = parser.GetJTokenValue(element, "replyCount");
                            bool.TryParse(parser.GetJTokenValue(element, "canReblog"), out bool canReblog);
                            post.CanReblog = canReblog;
                            post.ReblogCount = parser.GetJTokenValue(element, "reblogCount");
                            bool.TryParse(parser.GetJTokenValue(element, "blog", "followed"), out bool followed);
                            post.isFollowedPostOwner = followed;
                            var contentPresent = content != null && content.HasValues;
                            if (contentPresent)
                            {
                                foreach (var rawData in content)
                                {
                                    var mediaType = parser.GetJTokenValue(rawData, "type");
                                    if (!string.IsNullOrEmpty(mediaType) && mediaType.Contains("image"))
                                    {
                                        var media = parser.GetJTokenOfJToken(rawData, "media");
                                        if (media != null && media.HasValues)
                                        {
                                            foreach (var raw in media)
                                            {
                                                post.MediaType = MediaType.Image;
                                                post.MediaUrl = parser.GetJTokenValue(raw, "url");
                                                post.isImageType = true;
                                                post.PostType = "image";
                                                break;
                                            }
                                        }
                                        break;
                                    }

                                    if (!string.IsNullOrEmpty(mediaType) && mediaType.Contains("video"))
                                    {
                                        try
                                        {
                                            var media = parser.GetJTokenOfJToken(rawData, "media");
                                            post.MediaType = MediaType.Video;
                                            post.MediaUrl = parser.GetJTokenValue(media, "url");
                                            post.isVidioType = true;
                                            post.PostType = "video";
                                            break;
                                        }
                                        catch (Exception)
                                        {
                                        }
                                    }
                                    if (mediaType.Contains("text"))
                                    {
                                        try
                                        {
                                            var media = parser.GetJTokenOfJToken(rawData, "media");
                                            post.MediaType = MediaType.NoMedia;
                                            post.MediaUrl = parser.GetJTokenValue(media, "type");
                                            post.isTextType = true;
                                            post.PostType = "text";
                                            break;
                                        }
                                        catch (Exception)
                                        {
                                        }
                                    }
                                }
                            }
                            if ((post.isImageType && post.isTextType) ||
                             (post.isVidioType && post.isTextType) ||
                             (post.isImageType && post.isVidioType) ||
                             (post.isImageType && post.isVidioType && post.isVidioType) ||
                             (!post.isImageType && !post.isVidioType && !post.isVidioType))
                                post.PostType = "hybrid";

                            if(!string.IsNullOrEmpty(post?.PostUrl))
                                LstTumblrPosts.Add(post);

                        }
                        catch (Exception)
                        {
                            continue;
                        }

                    }
                    NextPageUrl = parser.GetJTokenValue(jObject, "response", "timeline", "links", "next", "href");
                    if (string.IsNullOrEmpty(NextPageUrl))
                        NextPageUrl = parser.GetJTokenValue(jObject, "SearchRoute", "timelines", "post", "response", "timeline", "links", "next", "href");
                    if (!string.IsNullOrEmpty(NextPageUrl))
                        NextPageUrl = "https://www.tumblr.com/api" + NextPageUrl;
                    hasMoreResults = !string.IsNullOrEmpty(NextPageUrl);
                    #endregion Post Data.
                }

            }
            catch (Exception ex)
            {
                ex.DebugLog("Search Error Due to No More Data ===>>>>>" + ex.Message);
            }

            #endregion
        }
        public List<TumblrPost> LstTumblrPosts = new List<TumblrPost>();
        // ReSharper disable once InconsistentNaming
        public string tumblr_form_key { get; set; }
        public string NextPageUrl { get; set; }
        public bool hasMoreResults { get; set; }
    }
}