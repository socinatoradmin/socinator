using DominatorHouseCore.Interfaces;
using GramDominatorCore.GDModel;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace GramDominatorCore.Response
{
    [Localizable(false)]
    public class MediaCommentsIgResponseHandler : MediaInteractionIgResponseHandler
    {
        public MediaCommentsIgResponseHandler(IResponseParameter response,bool IsCommentDetails = false)
          : base(response)
        {
            try
            {
                if (!Success)
                    return;
                if (Success && (response.Response.Contains("<!DOCTYPE html>") || (response.Response.Contains("<div class"))))
                {
                    Success = true;
                    UserList = new List<InstagramUser>();
                    GetCommentsList(response.Response);
                    HasMoreResults = true;
                    return;
                }
                else if (Success && response.Response.Contains("edges"))
                {
                    UserList = new List<InstagramUser>();
                    var obj = handler.ParseJsonToJObject(response.Response);
                    MaxId = handler.GetJTokenValue(obj, "data", "shortcode_media", "edge_media_to_comment", "page_info", "end_cursor");
                    bool.TryParse(handler.GetJTokenValue(obj, "data", "shortcode_media", "edge_media_to_comment", "page_info", "has_next_page"), out bool hasMore);
                    HasMoreResults = hasMore;
                    var elements = new string[] { response.Response };
                    foreach (var item in elements)
                    {
                        GetCommentsList(item);
                    }
                    Success = true;
                    return;
                }
            }
            catch
            {
            }
        }

        public void GetCommentsList(string response)
        {
            try
            {
                var RespJ = handler.ParseJsonToJObject(response);
                if (RespJ == null)
                {
                    var array1 = handler.GetJArrayElement(response);
                    if (array1 != null && array1.HasValues)
                    {
                        foreach (var item in array1)
                        {
                            var dataArray = handler.GetJArrayElement(handler.GetJTokenValue(handler.ParseJsonToJObject(item.ToString()), "edges"));
                            GetCommentData(dataArray);
                        }
                    }
                }
                else
                {
                    var array = handler.GetJArrayElement(handler.GetJTokenValue(RespJ, "edges"));
                    array = array is null || !array.HasValues ? handler.GetJArrayElement(handler.GetJTokenValue(RespJ, "data", "shortcode_media", "edge_media_to_comment", "edges")) : array;
                    if (response.StartsWith("{\"data\":{\"xdt_api__v1__media__media_id__comments__connection\":"))
                    {
                        var token = handler.GetJTokenOfJToken(RespJ, "data", "xdt_api__v1__media__media_id__comments__connection");
                        array = handler.GetJArrayElement(handler.GetJTokenValue(token, "edges"));
                    }
                    GetCommentData(array);
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                UserList = CommentList.Select(x => x.ItemUser).ToList();
                InteractionCount = CommentCount;
                HasMoreResults = true;
            }
        }

        private void GetCommentData(JArray array)
        {
            try
            {
                foreach (JToken jtoken in array)
                {
                    var commentList = CommentList;
                    var node = jtoken.ToString().Contains("\"owner\"") ? "owner" : "user";
                    var id = handler.GetJTokenValue(jtoken, "node", node, "pk");
                    id = string.IsNullOrEmpty(id) ? handler.GetJTokenValue(jtoken, "node", node, "id") : id;
                    ResultCommentItemUser resultCommentItemUser = new ResultCommentItemUser();
                    InstagramUser instagramUser = new InstagramUser(id,
                        handler.GetJTokenValue(jtoken, "node", node, "username"))
                    {
                        ProfilePicUrl = handler.GetJTokenValue(jtoken, "node", node, "profile_pic_url")
                    };
                    bool.TryParse(handler.GetJTokenValue(jtoken, "node", node, "is_verified"), out bool is_verified);
                    instagramUser.IsVerified = is_verified;
                    resultCommentItemUser.ItemUser = instagramUser;
                    var commentID = handler.GetJTokenValue(jtoken, "node", "pk");
                    commentID = string.IsNullOrEmpty(commentID) ? handler.GetJTokenValue(jtoken, "node", "id") : commentID;
                    resultCommentItemUser.CommentId = commentID;
                    if (commentList != null && commentList.Any(x => x.CommentId == resultCommentItemUser.CommentId))
                        continue;
                    int.TryParse(handler.GetJTokenValue(jtoken, "node", "comment_like_count"), out int comment_like_count);
                    resultCommentItemUser.CommentLikeCount = comment_like_count;
                    int.TryParse(handler.GetJTokenValue(jtoken, "node", "created_at"), out int created_at);
                    resultCommentItemUser.CreatedAt = created_at;
                    bool.TryParse(handler.GetJTokenValue(jtoken, "node", "has_liked_comment"), out bool has_liked_comment);
                    resultCommentItemUser.HasLikedComment = has_liked_comment;
                    resultCommentItemUser.Text = handler.GetJTokenValue(jtoken, "node", "text");
                    commentList.Add(resultCommentItemUser);

                }
            }
            catch { }
        }

        public int CommentCount { get; set; }
        public List<ResultCommentItemUser> CommentList { get; set; } = new List<ResultCommentItemUser>();

        private string Caption { get; set; }

        private string CaptionCreatedAt { get; set; }

        private bool CommentLikesEnabled { get; set; }
    }
}
