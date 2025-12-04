using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using System;
using System.Collections.Generic;
using System.Net;
using TumblrDominatorCore.Models;
using TumblrDominatorCore.TmblrUtility;

namespace TumblrDominatorCore.TumblrResponseHandler
{
    public class SearchLikedPostResponse : ResponseHandler
    {
        public SearchLikedPostResponse(IResponseParameter responeParameter) : base(responeParameter)
        {
            try
            {
                if (responeParameter == null || string.IsNullOrEmpty(responeParameter?.Response)) return;
                TumblrPosts = new List<TumblrPost>();
                var response = WebUtility.HtmlDecode(responeParameter.Response);
                if (response.Contains("{\"meta\":{\"status\":200,\"msg\":\"OK\"}"))
                {
                    Success = true;
                    JsonHandler handler;
                    if (!response.IsValidJson())
                    {
                        var validJsonResponse = TumblrUtility.GetDecodedResponseOfJson(response);
                        handler = new JsonHandler(validJsonResponse);
                    }
                    else
                        handler = new JsonHandler(response);
                    NextPageUrl = handler.GetElementValue("response", "links", "next", "href");
                    if (!string.IsNullOrEmpty(NextPageUrl))
                        NextPageUrl = "https://www.tumblr.com" + NextPageUrl;
                    TotalLikeCount = Convert.ToInt32(handler.GetJToken("response", "likedCount"));
                    var data = handler.GetJToken("response", "likedPosts");
                    foreach (var item in data)
                    {
                        var handler1 = new JsonHandler(item);
                        var post = new TumblrPost();
                        post.BlogName = handler1.GetElementValue("blogName");
                        post.Id = handler1.GetElementValue("id");
                        post.PlacementId = handler1.GetElementValue("placementId");
                        post.OwnerUsername = handler1.GetElementValue("blog", "name");
                        post.NotesCount = handler1.GetElementValue("noteCount");
                        post.PostKey = handler1.GetElementValue("reblogKey");
                        post.RebloggedRootId = handler1.GetElementValue("reblogKey");
                        post.Caption = handler1.GetElementValue("summary");
                        post.PostUrl = handler1.GetElementValue("postUrl");
                        post.BlogUrl = handler1.GetElementValue("blog", "url");
                        var content = handler1.GetJToken("content");
                        post.IsLiked = handler1.GetElementValue("liked").Contains("True") ? true : false;
                        post.CanLike = handler1.GetElementValue("canLike").Contains("True") ? true : false;
                        post.CanReblog = handler1.GetElementValue("canReblog").Contains("True") ? true : false;
                        var contentPresent = content.HasValues;
                        if (contentPresent)
                        {
                            foreach (var rawData in content)
                            {
                                var hand = new JsonHandler(rawData);
                                var mediaType = hand.GetElementValue("type");
                                if (mediaType.Contains("image"))
                                {
                                    var media = hand.GetJToken("media");
                                    foreach (var raw in media)
                                    {
                                        var jhand = new JsonHandler(raw);
                                        post.MediaType = MediaType.Image;
                                        post.MediaUrl = jhand.GetElementValue("url");
                                        post.isImageType = true;
                                        post.PostType = "photo";
                                        break;
                                    }
                                    break;
                                }

                                if (mediaType.Contains("video"))
                                {
                                    try
                                    {
                                        var media = hand.GetJToken("media");
                                        var jhand = new JsonHandler(media);
                                        post.MediaType = MediaType.Video;
                                        post.MediaUrl = jhand.GetElementValue("url");
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
                                        var media = hand.GetJToken("media");
                                        var jhand = new JsonHandler(rawData);
                                        post.MediaType = MediaType.NoMedia;
                                        post.MediaUrl = jhand.GetElementValue("type");
                                        post.isTextType = true;
                                        post.PostType = "text";
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


                        TumblrPosts.Add(post);
                    }
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public List<TumblrPost> TumblrPosts { get; set; } = new List<TumblrPost>();
        public string NextPageUrl { get; set; }
        public int TotalLikeCount { get; set; }
        public string TumblrFormKey { get; set; }
    }
}