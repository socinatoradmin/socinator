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
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace FaceDominatorCore.FDResponse.BrowserResponseHandler.FanpageResponseHandler
{
    public class FdCommentResponseHandler : FdResponseHandler, IResponseHandler
    {
        public string EntityId { get; set; }


        public bool HasMoreResults { get; set; }

        public FdScraperResponseParameters ObjFdScraperResponseParameters { get; set; }

        public string PageletData { get; set; }

        public bool Status { get; set; }
        public FdCommentResponseHandler(IResponseParameter responseParameter,
            FacebookPostDetails post, List<string> JsonListofComment, bool hasMoreResults = false, bool isClassicUi = false)
            : base(responseParameter)
        {
            ObjFdScraperResponseParameters = new FdScraperResponseParameters();
            ObjFdScraperResponseParameters.PostDetails = post;
            ObjFdScraperResponseParameters.CommentList = new List<FdPostCommentDetails>();

            try
            {
                foreach (var postResponse in JsonListofComment)
                {
                    JObject jObject = null;
                    JArray fdjArray = new JArray();
                    var elements = new JArray();
                    try
                    {
                        if (!postResponse.IsValidJson())
                        {
                            var decodedResponse = "";
                            if (postResponse.StartsWith("<!DOCTYPE html>"))
                            {
                                var splittedArray = Utilities.GetBetween(Regex.Split(postResponse, "</script>").SingleOrDefault(x => x.Contains("{\"__typename\":\"Story\"")) + "/end", "{\"require\"", "/end");
                                decodedResponse = "{\"require\"" + splittedArray;
                                jObject = parser.ParseJsonToJObject(decodedResponse);
                            }
                            else
                                decodedResponse = "[" + postResponse.Replace("}}}}\r\n{\"label\":", "}}}},\r\n{\"label\":") + "]";
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
                            var nodes = parser.GetJTokenOfJToken(jObject, "data", "node", "comment_rendering_instance_for_feed_location", "comments", "edges");
                            if (nodes == null || nodes.Count() == 0)
                                nodes = JsonSearcher.FindStringValueByKey(JsonSearcher.FindByKey(jObject, "comments"), "edges");

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
                            var nodes = parser.GetJTokenOfJToken(node, "data", "node", "comment_rendering_instance_for_feed_location", "comments", "edges");
                            if (nodes == null || nodes.Count() == 0)
                                nodes = JsonSearcher.FindByKey(JsonSearcher.FindByKey(jObject, "comments"), "edges");
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
                        if (nodeObject != null && nodeObject.Count() > 0 && parser.GetJTokenValue(nodeObject, "__typename") == "Comment")
                            storyObject = nodeObject;
                        var commentText = parser.GetJTokenValue(storyObject, "body", "text");
                        if (string.IsNullOrEmpty(commentText))
                            commentText = parser.GetJTokenValue(storyObject, "preferred_body", "text");
                        if (string.IsNullOrEmpty(commentText))
                            commentText = parser.GetJTokenValue(storyObject, "body_renderer", "text");
                        var url = parser.GetJTokenValue(storyObject, "feedback", "url");
                        if (string.IsNullOrEmpty(url))
                            url = parser.GetJTokenValue(storyObject, "comment_action_links", 0, "comment", "url");


                        var commentId = parser.GetJTokenValue(storyObject, "legacy_fbid");
                        if (string.IsNullOrEmpty(commentId))
                            commentId = parser.GetJTokenValue(storyObject, "feedback", "id");
                        if (!string.IsNullOrEmpty(commentId) && !FdFunctions.IsIntegerOnly(commentId))
                            commentId = StringHelper.Base64Decode(commentId)?.Split(':')?.LastOrDefault()?.Split('_')?.LastOrDefault();

                        if (string.IsNullOrEmpty(url))
                            url = $"{FdConstants.FbHomeUrl}{commentId}";

                        var commenterId = parser.GetJTokenValue(storyObject, "author", "id");
                        if (string.IsNullOrEmpty(commenterId))
                            commenterId = parser.GetJTokenValue(JsonSearcher.FindByKey(JsonSearcher.FindByKey(storyObject, "comment_action_links"), "author"), "id");

                        var commenterName = parser.GetJTokenValue(storyObject, "author", "name");
                        if (string.IsNullOrEmpty(commenterName))
                            commenterName = parser.GetJTokenValue(JsonSearcher.FindByKey(JsonSearcher.FindByKey(storyObject, "comment_action_links"), "author"), "name");

                        int replycount = 0;
                        var _replyCount= parser.GetJTokenValue(storyObject, "feedback", "replies_fields","count");
                        if (string.IsNullOrEmpty(_replyCount))
                            _replyCount = JsonSearcher.FindStringValueByKey(JsonSearcher.FindByKey(JsonSearcher.FindByKey(storyObject, "feedback"), "replies_fields"), "count");
                        int.TryParse(_replyCount, out replycount);

                        var reactionCount = 0;
                        var reactionElements = parser.GetJTokenOfJToken(storyObject, "feedback", "top_reactions", "edges");
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
                        int creationtime = 0;
                        var commentDateTime = parser.GetJTokenValue(storyObject, "created_time");
                        if (string.IsNullOrEmpty(commentDateTime))
                            commentDateTime = JsonSearcher.FindStringValueByKey(JsonSearcher.FindByKey(storyObject, "comment_action_links"), "created_time");
                        var commentTime = int.TryParse(commentDateTime, out creationtime) ? DateTimeUtilities.EpochToDateTimeUtc(creationtime) : new DateTime();

                        var isDisabled = parser.GetJTokenValue(storyObject, "is_disabled");
                        var commentPostId = JsonSearcher.FindStringValueByKey(JsonSearcher.FindByKey(storyObject, "plugins"), "post_id");
                        if (string.IsNullOrEmpty(commentPostId)) commentPostId = post.Id;
                        if (ObjFdScraperResponseParameters.CommentList.Any(x => commentId != "" && x.CommentId == commentId)) continue;
                        ObjFdScraperResponseParameters.CommentList.Add(new FdPostCommentDetails()
                        {
                            CommentText = commentText,
                            CommentId = commentId,
                            CommenterName = commenterName,
                            CommentUrl = url,
                            CommenterID = commenterId,
                            ReactionCountOnComment = reactionCount.ToString(),
                            CommentTimeWithDate = commentTime.ToString(CultureInfo.CurrentCulture),
                            PostId = commentPostId,
                            CanCommentByUser = isDisabled.ToLower() == "false",
                            ReplyCount=replycount
                        }
                        );
                    }

                }

            }
            catch (Exception)
            { }
            Status = ObjFdScraperResponseParameters.CommentList.Count > 0;
            HasMoreResults = hasMoreResults;
        }
    }
}
