using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using TumblrDominatorCore.Models;
using TumblrDominatorCore.TmblrUtility;

namespace TumblrDominatorCore.TumblrResponseHandler
{
    public class PostScraperResponse : ResponseHandler
    {
        public PostScraperResponse(IResponseParameter responeParameter, TumblrPost tumblrPost = null) :
            base(responeParameter)
        {
            try
            {
                if (responeParameter.Response.Contains("{\"meta\":{\"status\":200,\"msg\":\"OK\"}"))
                    Success = true;
                JObject jObject;
                if (tumblrPost != null)//for Single Post.
                {
                    if (!responeParameter.Response.IsValidJson())
                    {
                        var decodedResponse = TumblrUtility.GetDecodedResponseOfJson(responeParameter.Response);
                        jObject = parser.ParseJsonToJObject(decodedResponse);
                    }
                    else
                        jObject = parser.ParseJsonToJObject(responeParameter.Response);

                    this.tumblrPost = tumblrPost;
                    this.tumblrPost.NotesCount = parser.GetJTokenValue(jObject, "response", "total_notes");
                    this.tumblrPost.LikeCount = parser.GetJTokenValue(jObject, "response", "total_likes");
                    this.tumblrPost.ReblogCount = parser.GetJTokenValue(jObject, "response", "total_reblogs");
                }
                else
                {
                    jObject = parser.ParseJsonToJObject(responeParameter.Response);
                    var ScrappedPosts = parser.GetJArrayElement(parser.GetJTokenValue(jObject, "SearchRoute", "timelines", "post", "response", "timeline", "elements"));
                    if (ScrappedPosts != null && ScrappedPosts.HasValues)
                        ScrappedPosts.ForEach(post =>
                        {
                            var blogName = parser.GetJTokenValue(post, "blogName");
                            bool.TryParse(parser.GetJTokenValue(post, "liked"), out bool IsLikedPost);
                            bool.TryParse(parser.GetJTokenValue(post, "canLike"), out bool CanLikePost);
                            bool.TryParse(parser.GetJTokenValue(post, "canReblog"), out bool CanReblogPost);
                            bool.TryParse(parser.GetJTokenValue(post, "canReply"), out bool CanReplyPost);
                            var postType = parser.GetJTokenValue(post, "content", 0, "type") ?? "image";
                            var PostId = parser.GetJTokenValue(post, "id");
                            TumblrPosts.Add(new TumblrPost
                            {
                                Id = PostId,
                                Caption = parser.GetJTokenValue(post, "slug")?.Replace("-", ""),
                                BlogName = blogName,
                                OwnerUsername = blogName,
                                PostUrl = ConstantHelpDetails.TumblrUrl + "/" + blogName + "/" + PostId,
                                IsLiked = IsLikedPost,
                                CanLike = CanLikePost,
                                CanReblog = CanReplyPost,
                                CanReply = CanReplyPost,
                                BlogUrl = parser.GetJTokenValue(post, "blog", "url"),
                                NotesCount = parser.GetJTokenValue(post, "noteCount"),
                                LikeCount = parser.GetJTokenValue(post, "likeCount"),
                                ReblogCount = parser.GetJTokenValue(post, "reblogCount"),
                                ReplyCount = parser.GetJTokenValue(post, "replyCount"),
                                RebloggedRootId = parser.GetJTokenValue(post, "reblogKey"),
                                PostType = postType,
                                ProfileUrl = parser.GetJTokenValue(post, "blog", "blogViewUrl"),
                                isImageType = postType == "image",
                                isTextType = postType == "text",
                                isVidioType = postType == "video" || postType == "gif"
                            });
                        });
                }
            }
            catch (Exception e)
            {
                e.DebugLog();
            }
        }

        public TumblrPost tumblrPost { get; set; } = new TumblrPost();
        public List<TumblrPost> TumblrPosts { get; set; } = new List<TumblrPost>();
    }
}