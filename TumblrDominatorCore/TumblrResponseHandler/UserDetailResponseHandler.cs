using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using System;
using System.Collections.Generic;
using TumblrDominatorCore.Models;
using TumblrDominatorCore.TmblrUtility;

namespace TumblrDominatorCore.TumblrResponseHandler
{
    public class UserDetailResponseHandler : ResponseHandler
    {
        public JsonHandler JsonHand { get; set; }

        public TumblrUser tumblrUser = new TumblrUser();

        public List<TumblrPost> LstTumblrPosts = new List<TumblrPost>();
        public UserDetailResponseHandler(IResponseParameter responeParameter) : base(responeParameter)
        {
            try
            {
                if (responeParameter.Response.Contains("{\"meta\":{\"status\":200,\"msg\":\"OK\"}"))
                {
                    Success = true;

                    JsonHand = null;
                    if (!responeParameter.Response.IsValidJson())
                    {
                        var validJsonResponse = TumblrUtility.GetDecodedResponseOfJson(responeParameter.Response);
                        JsonHand = new JsonHandler(validJsonResponse);
                    }
                    else
                        JsonHand = new JsonHandler(responeParameter.Response);
                    tumblrUser.Username = JsonHand.GetElementValue("response", "blog", "name");
                    tumblrUser.PageUrl = JsonHand.GetElementValue("response", "blog", "url");
                    tumblrUser.Uuid = JsonHand.GetElementValue("response", "blog", "uuid");
                    tumblrUser.IsFollowed = JsonHand.GetElementValue("response", "blog", "followed").Contains("True") ? true : false;
                    tumblrUser.CanFollow = JsonHand.GetElementValue("response", "blog", "canBeFollowed").Contains("True") ? true : false;
                    tumblrUser.CanMessage = JsonHand.GetElementValue("response", "blog", "canMessage").Contains("True") ? true : false;

                    var elements = JsonHand.GetJToken("response", "posts");
                    foreach (var element in elements)
                    {
                        var Elementhandler = new JsonHandler(element.ToString());
                        var post = new TumblrPost();
                        post.PlacementId = Elementhandler.GetJToken("placementId")?.ToString();
                        post.Uuid = Elementhandler.GetElementValue("blog", "uuid");

                        try
                        {
                            #region Post Data.
                            post.BlogName = Elementhandler.GetElementValue("blogName");
                            post.Id = Elementhandler.GetElementValue("id");
                            post.OwnerUsername = Elementhandler.GetElementValue("blog", "name");
                            post.NotesCount = Elementhandler.GetElementValue("noteCount");
                            post.PostKey = Elementhandler.GetElementValue("reblogKey");
                            post.RebloggedRootId = Elementhandler.GetElementValue("reblogKey");
                            post.Caption = Elementhandler.GetElementValue("summary");
                            post.PostUrl = Elementhandler.GetElementValue("postUrl");
                            post.BlogUrl = Elementhandler.GetElementValue("blog", "url");
                            post.ProfileUrl = Elementhandler.GetElementValue("blog", "blogViewUrl");
                            var content = Elementhandler.GetJToken("content");
                            post.IsLiked = Elementhandler.GetElementValue("liked").Contains("True") ? true : false;
                            post.CanLike = Elementhandler.GetElementValue("canLike").Contains("True") ? true : false;
                            post.CanReply = Elementhandler.GetElementValue("canReply").Contains("True") ? true : false;
                            post.CanReblog = Elementhandler.GetElementValue("canReblog").Contains("True") ? true : false;

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

                            #endregion
                            LstTumblrPosts.Add(post);

                        }
                        catch (Exception)
                        {
                            continue;
                        }
                        //handler = new JsonHandler(post.ToString());

                    }

                }
            }
            catch (Exception ex)
            {
                Success = false;
                ex.DebugLog();
            }
        }


    }
}

