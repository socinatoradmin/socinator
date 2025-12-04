using ControlzEx.Standard;
using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDResponse.BaseResponse;
using FaceDominatorCore.Interface;
using FaceDominatorCore.Utility;
using Newtonsoft.Json.Linq;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace FaceDominatorCore.FDResponse.ScrapersResponse
{
    public class CommentRepliesResponseHandler : FdResponseHandler, IResponseHandler
    {
        public string EntityId { get; set; }
        public string PageletData { get; set; }
        public bool HasMoreResults { get; set; }
        public bool Status { get; set; }
        public FdScraperResponseParameters ObjFdScraperResponseParameters { get; set; }
            = new FdScraperResponseParameters();

        public CommentRepliesResponseHandler(IResponseParameter responseParameter, FdPostCommentDetails commentDetails)
            : base(responseParameter)
        {

            string decodedResponse = FdFunctions.GetNewPrtialDecodedResponse(responseParameter.Response);
            ObjFdScraperResponseParameters.CommentRepliesList = new List<FdPostCommentRepliesDetails>();

            GetCommentRepliesList(decodedResponse, commentDetails.CommentId);

            Status = ObjFdScraperResponseParameters.CommentRepliesList.Count > 0;
        }
        public CommentRepliesResponseHandler(IResponseParameter responseParameter, List<string> listUserData,
            FdPostCommentDetails commentDetails, bool hasMoreResults = false, bool isClassicUi = true)
            : base(responseParameter)
        {
            ObjFdScraperResponseParameters = new FdScraperResponseParameters();

            ObjFdScraperResponseParameters.PostDetails = new FacebookPostDetails()
            {
                PostUrl = $"{FdConstants.FbHomeUrl}{commentDetails.PostId}",
                Id = commentDetails.PostId
            };

            ObjFdScraperResponseParameters.CommentRepliesList = new List<FdPostCommentRepliesDetails>();

            GetCommentRepliesList(listUserData, commentDetails);

            Status = ObjFdScraperResponseParameters.CommentRepliesList.Count > 0;

            HasMoreResults = hasMoreResults;
        }

        public CommentRepliesResponseHandler(List<string> jsonlistCommentData,IResponseParameter responseParameter,
            FdPostCommentDetails commentDetails, bool hasMoreResults = false, bool isClassicUi = true)
            : base(responseParameter)
        {
            ObjFdScraperResponseParameters = new FdScraperResponseParameters();

            ObjFdScraperResponseParameters.PostDetails = new FacebookPostDetails()
            {
                PostUrl = $"{FdConstants.FbHomeUrl}{commentDetails.PostId}",
                Id = commentDetails.PostId
            };

            ObjFdScraperResponseParameters.CommentRepliesList = new List<FdPostCommentRepliesDetails>();

            GetCommentRepliesListFromJson(jsonlistCommentData, commentDetails);

            Status = ObjFdScraperResponseParameters.CommentRepliesList.Count > 0;

            HasMoreResults = hasMoreResults;
        }
        private void GetCommentRepliesListFromJson(List<string> JsonListofComment,
            FdPostCommentDetails commentDetails)
        {
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
                            var nodes = parser.GetJTokenOfJToken(jObject, "data", "node", "comment_rendering_instance_for_feed_location", "comments", "edges");
                            if (nodes == null || nodes.Count() == 0)
                                nodes = JsonSearcher.FindByKey(JsonSearcher.FindByKey(jObject, "comments"), "edges");

                            if (nodes != null && nodes.Count() != 0)
                            {
                                var firstCommentNode = nodes.FirstOrDefault();
                                if (firstCommentNode != null && firstCommentNode.Count() != 0)
                                {
                                    var replynodes = JsonSearcher.FindByKey(JsonSearcher.FindByKey(firstCommentNode, "replies_connection"), "edges");
                                    if (replynodes != null && replynodes.Count() != 0)
                                    {
                                        if (replynodes.Type == JTokenType.Array)
                                        {
                                            foreach (var nodepost in replynodes)
                                                elements.Add(nodepost);
                                        }
                                        else
                                            elements.Add(replynodes);
                                    }
                                }
                                
                            }
                        }
                        foreach (var node in fdjArray)
                        {
                            var nodes = parser.GetJTokenOfJToken(node, "data", "node", "comment_rendering_instance_for_feed_location", "comments", "edges");
                            if (nodes == null || nodes.Count() == 0)
                                nodes = JsonSearcher.FindByKey(JsonSearcher.FindByKey(jObject, "comments"), "edges");
                            if (nodes == null || nodes.Count() == 0)
                                nodes = JsonSearcher.FindByKey(JsonSearcher.FindByKey(jObject, "data"), "node");
                            if (nodes != null&& nodes.Count() != 0)
                            {
                                var firstCommentNode = nodes.FirstOrDefault();
                                if(firstCommentNode!=null&&firstCommentNode.Count()!=0)
                                {
                                    var storyObject = firstCommentNode;
                                    var firstnodeObject = JsonSearcher.FindByKey(firstCommentNode, "node");
                                    if (firstnodeObject != null && firstnodeObject.Count() > 0 && parser.GetJTokenValue(firstnodeObject, "__typename") == "Comment")
                                        storyObject = firstnodeObject;
                                    var replynodes= JsonSearcher.FindByKey(JsonSearcher.FindByKey(firstCommentNode, "replies_connection"), "edges");
                                    if (replynodes != null && replynodes.Count() != 0)
                                    {
                                        if (replynodes.Type == JTokenType.Array)
                                        {
                                            foreach (var nodepost in replynodes)
                                                elements.Add(nodepost);
                                        }
                                        else
                                            elements.Add(replynodes);
                                    }
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
                        if (nodeObject != null && nodeObject.Count() > 0 && parser.GetJTokenValue(nodeObject, "__typename") == "Comment")
                            storyObject = nodeObject;
                        var replycommentText = parser.GetJTokenValue(storyObject, "body", "text");
                        if (string.IsNullOrEmpty(replycommentText))
                            replycommentText = parser.GetJTokenValue(storyObject, "preferred_body", "text");
                        if (string.IsNullOrEmpty(replycommentText))
                            replycommentText = parser.GetJTokenValue(storyObject, "body_renderer", "text");
                        var url = parser.GetJTokenValue(storyObject, "feedback", "url");
                        if (string.IsNullOrEmpty(url))
                            url = parser.GetJTokenValue(storyObject, "comment_action_links", 0, "comment", "url");


                        var replycommentId = parser.GetJTokenValue(storyObject, "legacy_fbid");
                        if (string.IsNullOrEmpty(replycommentId))
                            replycommentId = parser.GetJTokenValue(storyObject, "feedback", "id");
                        if (!string.IsNullOrEmpty(replycommentId) && !FdFunctions.IsIntegerOnly(replycommentId))
                            replycommentId = StringHelper.Base64Decode(replycommentId)?.Split(':')?.LastOrDefault()?.Split('_')?.LastOrDefault();

                        if (string.IsNullOrEmpty(url))
                            url = $"{FdConstants.FbHomeUrl}{replycommentId}";

                        var replycommenterId = parser.GetJTokenValue(storyObject, "author", "id");
                        if (string.IsNullOrEmpty(replycommenterId))
                            replycommenterId = parser.GetJTokenValue(JsonSearcher.FindByKey(JsonSearcher.FindByKey(storyObject, "comment_action_links"), "author"), "id");

                        var replycommenterName = parser.GetJTokenValue(storyObject, "author", "name");
                        if (string.IsNullOrEmpty(replycommenterName))
                            replycommenterName = parser.GetJTokenValue(JsonSearcher.FindByKey(JsonSearcher.FindByKey(storyObject, "comment_action_links"), "author"), "name");


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
                        var replycommentDateTime = parser.GetJTokenValue(storyObject, "created_time");
                        if (string.IsNullOrEmpty(replycommentDateTime))
                            replycommentDateTime = JsonSearcher.FindStringValueByKey(JsonSearcher.FindByKey(storyObject, "comment_action_links"), "created_time");
                        var commentTime = int.TryParse(replycommentDateTime, out creationtime) ? DateTimeUtilities.EpochToDateTimeUtc(creationtime) : new DateTime();

                        var isDisabled = parser.GetJTokenValue(storyObject, "is_disabled");
                        var commentPostId = JsonSearcher.FindStringValueByKey(JsonSearcher.FindByKey(storyObject, "plugins"), "post_id");
                        if (string.IsNullOrEmpty(commentPostId)) commentPostId = commentDetails.PostId;
                        if (ObjFdScraperResponseParameters.CommentRepliesList.Any(x => replycommentId != "" && x.ReplyCommenterID == replycommentId)) continue;
                        ObjFdScraperResponseParameters.CommentRepliesList.Add(new FdPostCommentRepliesDetails()
                        {

                            ReplyCommenterName = replycommenterName,
                            ReplyCommentText = replycommentText,
                            ReplyCommenterID = replycommenterId,
                            ReplyCommentId = replycommentId,
                            ReplyCommentUrl = $"{FdConstants.FbHomeUrl}{replycommentId}",
                            CommentText = commentDetails.CommentText,
                            CommentId = commentDetails.CommentId,
                            CommentUrl = url,
                            CommenterID = commentDetails.CommenterID,
                            CommentTimeWithDate = replycommentDateTime,
                            PostId = commentPostId,
                            ReactionCountOnReplyComment = reactionCount.ToString()
                        }
                        );
                    }

                }

            }
            catch (Exception)
            { }

        }
        private void GetCommentRepliesList(List<string> commentResponseList,
             FdPostCommentDetails commentDetails)
        {
            try
            {
                foreach (var commentDetail in commentResponseList)
                {
                    var decodedResponse = FdFunctions.GetNewPrtialDecodedResponse(commentDetail);

                    if (!decodedResponse.Contains("Reply by "))
                        continue;

                    using (var htmlUtility = new FdHtmlParseUtility())
                    {
                        FdPostCommentRepliesDetails commentRepliesDetails = new FdPostCommentRepliesDetails();

                        commentRepliesDetails.ReplyCommentText = htmlUtility.GetInnerTextFromPartialTagName(decodedResponse, "div", "class", "x1lliihq xjkvuk6 x1iorvi4");

                        commentRepliesDetails.ReplyCommenterID = FdRegexUtility.FirstMatchExtractor(decodedResponse, FdConstants.ReplyCommenterIdRegex);

                        if (string.IsNullOrEmpty(commentRepliesDetails.ReplyCommenterID))
                            commentRepliesDetails.ReplyCommenterID = FdRegexUtility.FirstMatchExtractor(decodedResponse, "/user/(.*?)/");
                        if (string.IsNullOrEmpty(commentRepliesDetails.ReplyCommenterID))
                            commentRepliesDetails.ReplyCommenterID = FdRegexUtility.FirstMatchExtractor(decodedResponse, "id=(.*?)&");

                        commentRepliesDetails.ReplyCommenterName = htmlUtility.GetInnerTextFromPartialTagNameContains(decodedResponse, "a", "class",
                                                                       "x1hl2dhg xggy1nq x1a2a7pz x1heor9g xt0b8zv");
                        if (string.IsNullOrEmpty(commentRepliesDetails.ReplyCommenterName))
                            commentRepliesDetails.ReplyCommenterName = htmlUtility.GetInnerTextFromTagName(decodedResponse, "div", "class", "_2b05");

                        commentRepliesDetails.CommentId = commentDetails.CommentId;


                        commentRepliesDetails.ReplyCommentId = FdRegexUtility.FirstMatchExtractor(decodedResponse, FdConstants.ReplyCommentIdRegex);

                        commentRepliesDetails.CommentUrl = $"{FdConstants.FbHomeUrl}{commentRepliesDetails.CommentId}";

                        commentRepliesDetails.ReplyCommentUrl = $"{FdConstants.FbHomeUrl}{commentRepliesDetails.ReplyCommentId}";

                        //commentRepliesDetails.CommentTimeWithDate = DateTimeUtilities.EpochToDateTimeUtc(int.Parse(FdRegexUtility.FirstMatchExtractor(decodedResponse, "data-utime=\"(.*?)\""))).ToString();

                        commentRepliesDetails.PostId = commentDetails.PostId;
                        commentRepliesDetails.CommenterID = commentDetails.CommenterID;
                        commentRepliesDetails.CommentText = commentDetails.CommentText;
                        commentRepliesDetails.CommentTimeWithDate = commentDetails.CommentTimeWithDate;
                        if (ObjFdScraperResponseParameters.CommentRepliesList.FirstOrDefault
                            (x => x.ReplyCommentId == commentRepliesDetails.ReplyCommentId) == null
                            && commentRepliesDetails.CommentId == commentDetails.CommentId)
                            ObjFdScraperResponseParameters.CommentRepliesList.Add(commentRepliesDetails);
                    }
                }

                ObjFdScraperResponseParameters.CommentRepliesList.Reverse();



            }
            catch (Exception)
            {

            }


        }

        private void GetCommentRepliesList(string decodedResponse, string commentId)
        {

            try
            {
                string[] commentArray = Regex.Split(decodedResponse, "author:{__typename:\"User\",id").Skip(1).ToArray();

                string postCommentorId;
                string commentDateTime;
                foreach (var commentDetails in commentArray)
                {
                    FdPostCommentRepliesDetails postCommentDetails = new FdPostCommentRepliesDetails();

                    try
                    {
                        if (!commentDetails.Contains(commentId))
                            continue;

                        postCommentDetails.ReplyCommentId = FdRegexUtility.FirstMatchExtractor(commentDetails, FdConstants.CommentReplyIdRegex);
                        postCommentDetails.ReplyCommentUrl = $"{FdConstants.FbHomeUrl}{postCommentDetails.ReplyCommentId}";
                        postCommentorId = FdRegexUtility.FirstMatchExtractor(commentDetails, ":\"(.*?)\"");

                        postCommentDetails.ReplyCommenterID = FdFunctions.GetIntegerOnlyString(postCommentorId);

                        postCommentDetails.ReplyCommentText = FdRegexUtility.FirstMatchExtractor(commentDetails, FdConstants.CommentTextRegex);

                        postCommentDetails.PostId = FdRegexUtility.FirstMatchExtractor(commentDetails, FdConstants.LegacyPostIdRegex);

                        commentDateTime = FdRegexUtility.FirstMatchExtractor(commentDetails, FdConstants.CommentTimeRegex);

                        postCommentDetails.CommentTimeWithDate = Int32.Parse(commentDateTime).EpochToDateTimeUtc().ToString(CultureInfo.InvariantCulture);

                        postCommentDetails.CommentUrl = $"{FdConstants.FbHomeUrl}{commentId}";

                        if (!string.IsNullOrEmpty(postCommentDetails.ReplyCommentId) && ObjFdScraperResponseParameters.CommentRepliesList.FirstOrDefault(x => x.ReplyCommentId == postCommentDetails.ReplyCommentId) == null)
                            ObjFdScraperResponseParameters.CommentRepliesList.Add(postCommentDetails);

                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog(ex.Message);
                    }
                }

                Status = ObjFdScraperResponseParameters.CommentRepliesList.Count > 0;
            }
            catch (Exception ex)
            {
                ex.DebugLog(ex.Message);
            }

        }

    }
}
