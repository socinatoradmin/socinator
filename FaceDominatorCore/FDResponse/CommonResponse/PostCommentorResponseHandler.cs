using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDResponse.BaseResponse;
using FaceDominatorCore.Interface;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace FaceDominatorCore.FDResponse.CommonResponse
{

    public class PostCommentorResponseHandler : FdResponseHandler, IResponseHandler
    {
        public bool HasMoreResults { get; set; }

        public string EntityId { get; set; }

        public string PageletData { get; set; }

        public bool Status { get; set; }

        public FdScraperResponseParameters ObjFdScraperResponseParameters { get; set; }
            = new FdScraperResponseParameters();

        public PostCommentorResponseHandler(IResponseParameter responseParameter, bool isFirstPagination,
                        string postId, string feedLocation, bool isPostsProcess, string postUrl = "", string tempPostId = "")
            : base(responseParameter)
        {
            if (responseParameter.HasError)
                return;

            ObjFdScraperResponseParameters.FeedLocation = feedLocation ?? (feedLocation = string.Empty);

            ObjFdScraperResponseParameters.CommentList = new List<FdPostCommentDetails>();
            ObjFdScraperResponseParameters.ListUser = new List<FacebookUser>();

            string decodedResponse = FdFunctions.GetNewPrtialDecodedResponse(responseParameter.Response);


            try
            {
                GetPostCommentor(decodedResponse, postId, isPostsProcess, postUrl);


                if (ObjFdScraperResponseParameters.CommentList.Count == 0)
                    GetPostCommentorNew(decodedResponse, postId, isPostsProcess, postUrl);
                if (ObjFdScraperResponseParameters.CommentList.Count == 0)
                    GetPostCommentorJson(responseParameter.Response, isPostsProcess);
                UpadetePaginationData(decodedResponse, isFirstPagination, isPostsProcess);

                EntityId = string.IsNullOrEmpty(tempPostId)
                          ? FdRegexUtility.FirstMatchExtractor(decodedResponse, "ft_ent_identifier\" value=\"(.*?)\"")
                          : tempPostId;

                Status = ObjFdScraperResponseParameters.CommentList.Count > 0;

            }
            catch (Exception ex)
            {
                if (isPostsProcess)
                    Console.WriteLine(ex.StackTrace);
                else
                    ex.DebugLog(ex.Message);
            }
        }


        private void GetPostCommentor(string pageLikersData, string postId, bool isPostsProcess, string postUrl = "")
        {
            try
            {
                string[] commentArray = Regex.Split(pageLikersData, "body\":{\"text").Skip(1).ToArray();

                string postCommentorId;
                string commentPostId;
                string hasLikedByUser;
                string likecount;
                string commentText;
                string commentDateTime;
                string commentId;
                foreach (var commentDetails in commentArray)
                {
                    FdPostCommentDetails objFdPostCommentDetails = new FdPostCommentDetails();

                    try
                    {
                        postCommentorId = FdRegexUtility.FirstMatchExtractor(commentDetails, "author\":\"(.*?)\"");
                        postCommentorId = FdFunctions.GetIntegerOnlyString(postCommentorId);

                        objFdPostCommentDetails.CommenterID = postCommentorId;

                        commentText = FdRegexUtility.FirstMatchExtractor(commentDetails, "\":\"(.*?)\"");

                        objFdPostCommentDetails.CommentText = commentText;

                        commentId = FdRegexUtility.FirstMatchExtractor(commentDetails, "fbid\":\"(.*?)\"");

                        objFdPostCommentDetails.CommentId = commentId;

                        commentPostId = FdRegexUtility.FirstMatchExtractor(commentDetails, "ftentidentifier\":\"(.*?)\"");

                        objFdPostCommentDetails.PostId = commentPostId;

                        hasLikedByUser = FdRegexUtility.FirstMatchExtractor(commentDetails, "hasviewerliked\":(.*?),");

                        objFdPostCommentDetails.HasLikedByUser = hasLikedByUser;

                        commentDateTime = FdRegexUtility.FirstMatchExtractor(commentDetails, "\"time\":(.*?),");

                        objFdPostCommentDetails.CommentTimeWithDate = Int32.Parse(commentDateTime).EpochToDateTimeUtc().ToString(CultureInfo.InvariantCulture);

                        likecount = FdRegexUtility.FirstMatchExtractor(commentDetails, "likecount\":(.*?),");

                        objFdPostCommentDetails.ReactionCountOnComment = likecount;

                        objFdPostCommentDetails.CommentUrl = $"{FdConstants.FbHomeUrl}{commentId}";

                        if (ObjFdScraperResponseParameters.CommentList.FirstOrDefault(x => x.CommentId == objFdPostCommentDetails.CommentId) == null && (postId != commentPostId || !postUrl.Contains(commentPostId)))
                            ObjFdScraperResponseParameters.CommentList.Add(objFdPostCommentDetails);



                    }
                    catch (Exception ex)
                    {
                        if (isPostsProcess)
                            Console.WriteLine(ex.StackTrace);
                        else
                            ex.DebugLog(ex.Message);
                    }

                }

                commentArray = commentArray.Length <= 1 && pageLikersData.Contains("node:{created_time") ?
                    Regex.Split(pageLikersData, "node:{created_time").Skip(1).ToArray()
                    : commentArray.Length <= 1 && pageLikersData.Contains("node:{author:")
                    ? Regex.Split(pageLikersData, "node:{author:").Skip(1).ToArray()
                    : Regex.Split(pageLikersData, "body:{text").Skip(1).ToArray();

                foreach (var commentDetails in commentArray)
                {
                    FdPostCommentDetails objFdPostCommentDetails = new FdPostCommentDetails();

                    try
                    {
                        postCommentorId = FdRegexUtility.FirstMatchExtractor(commentDetails, "author:\"(.*?)\"");

                        if (string.IsNullOrEmpty(postCommentorId))
                            postCommentorId = FdRegexUtility.FirstMatchExtractor(commentDetails, "author:{__typename:\"User\",id:\"(.*?)\"");

                        if (string.IsNullOrEmpty(postCommentorId))
                            postCommentorId = FdRegexUtility.FirstMatchExtractor(commentDetails, "entity:{__typename:\"User\",id:\"(.*?)\"");

                        postCommentorId = FdFunctions.GetIntegerOnlyString(postCommentorId);

                        objFdPostCommentDetails.CommenterID = postCommentorId;

                        commentText = commentDetails.Contains("body:{text:")
                            ? FdRegexUtility.FirstMatchExtractor(commentDetails, "body:{text:\"(.*?)\"")
                            : FdRegexUtility.FirstMatchExtractor(commentDetails, ":\"(.*?)\"");

                        objFdPostCommentDetails.CommentText = commentText;

                        objFdPostCommentDetails.CommentId = string.IsNullOrEmpty(FdRegexUtility.FirstMatchExtractor(commentDetails, "fbid:\"(.*?)\""))
                            ? FdRegexUtility.FirstMatchExtractor(commentDetails, "comment_id=(.*?)\"")
                            : FdRegexUtility.FirstMatchExtractor(commentDetails, "fbid:\"(.*?)\"");

                        commentPostId = FdRegexUtility.FirstMatchExtractor(commentDetails, "ftentidentifier:\"(.*?)\"");

                        if (string.IsNullOrEmpty(commentPostId))
                            commentPostId = FdRegexUtility.FirstMatchExtractor(commentDetails, "legacy_token:\"(.*?)_");

                        objFdPostCommentDetails.PostId = commentPostId;

                        var authorDetails = FdRegexUtility.FirstMatchExtractor(commentDetails, "author:\\{(.*?)\\}");

                        objFdPostCommentDetails.CommenterName =
                                FdRegexUtility.FirstMatchExtractor(authorDetails, ",name:\"(.*?)\"");

                        if (string.IsNullOrEmpty(objFdPostCommentDetails.CommenterID))
                        {

                            postCommentorId = FdRegexUtility.FirstMatchExtractor(authorDetails, "id:\"(.*?)\"");
                            postCommentorId = FdFunctions.GetIntegerOnlyString(postCommentorId);
                            objFdPostCommentDetails.CommenterID = postCommentorId;
                        }

                        hasLikedByUser = FdRegexUtility.FirstMatchExtractor(commentDetails, "hasviewerliked:(.*?),");


                        if (string.IsNullOrEmpty(hasLikedByUser))
                            hasLikedByUser = "false";

                        objFdPostCommentDetails.HasLikedByUser = hasLikedByUser;

                        commentDateTime = FdRegexUtility.FirstMatchExtractor(commentDetails, "time:(.*?),");

                        objFdPostCommentDetails.CommentTimeWithDate = Int32.Parse(string.IsNullOrEmpty(commentDateTime) ? "0" : commentDateTime).EpochToDateTimeUtc().ToString(CultureInfo.InvariantCulture);

                        likecount = FdRegexUtility.FirstMatchExtractor(commentDetails, "likecount:(.*?),");

                        if (string.IsNullOrEmpty(likecount))
                            likecount = commentDetails.Contains("reaction_count:")
                                ? FdRegexUtility.FirstMatchExtractor(commentDetails, "reaction_count:(.*?),") : "0";

                        objFdPostCommentDetails.ReactionCountOnComment = likecount;

                        objFdPostCommentDetails.CommentUrl = $"{FdConstants.FbHomeUrl}{objFdPostCommentDetails.CommentId}";

                        if (ObjFdScraperResponseParameters.CommentList.FirstOrDefault(x => x.CommentId == objFdPostCommentDetails.CommentId) == null && (postId == commentPostId || postUrl.Contains(commentPostId)))
                            ObjFdScraperResponseParameters.CommentList.Add(objFdPostCommentDetails);

                    }
                    catch (Exception ex)
                    {
                        if (isPostsProcess)
                            Console.WriteLine(ex.StackTrace);
                        else
                            ex.DebugLog(ex.Message);
                    }
                }

                Status = ObjFdScraperResponseParameters.CommentList.Count > 0;
            }
            catch (Exception ex)
            {
                if (isPostsProcess)
                    Console.WriteLine(ex.StackTrace);
                else
                    ex.DebugLog(ex.Message);
            }

        }


        private void GetPostCommentorNew(string pageLikersData, string postId, bool isPostsProcess, string postUrl = "")
        {

            try
            {
                string[] commentArray = Regex.Split(pageLikersData, "body:{text").Skip(1).ToArray();

                string postCommentorId;
                string commentDateTime;
                string likecount;
                string hasLikedByUser;
                string commentText;
                string commentId;
                string commentPostId;
                foreach (var commentDetails in commentArray)
                {
                    FdPostCommentDetails objFdPostCommentDetails = new FdPostCommentDetails();

                    try
                    {

                        postCommentorId = FdRegexUtility.FirstMatchExtractor(commentDetails, "author:\"(.*?)\"");
                        postCommentorId = FdFunctions.GetIntegerOnlyString(postCommentorId);

                        objFdPostCommentDetails.CommenterID = postCommentorId;

                        commentText = FdRegexUtility.FirstMatchExtractor(commentDetails, ":\"(.*?)\"");

                        objFdPostCommentDetails.CommentText = commentText;

                        commentId = FdRegexUtility.FirstMatchExtractor(commentDetails, "fbid:\"(.*?)\"");

                        objFdPostCommentDetails.CommentId = commentId;

                        commentPostId = FdRegexUtility.FirstMatchExtractor(commentDetails, "ftentidentifier:\"(.*?)\"");

                        objFdPostCommentDetails.PostId = commentPostId;

                        hasLikedByUser = FdRegexUtility.FirstMatchExtractor(commentDetails, "hasviewerliked:(.*?),");

                        objFdPostCommentDetails.HasLikedByUser = hasLikedByUser;

                        commentDateTime = FdRegexUtility.FirstMatchExtractor(commentDetails, "time:(.*?),");

                        objFdPostCommentDetails.CommentTimeWithDate = Int32.Parse(commentDateTime).EpochToDateTimeUtc().ToString(CultureInfo.InvariantCulture);

                        likecount = FdRegexUtility.FirstMatchExtractor(commentDetails, "likecount:(.*?),");

                        objFdPostCommentDetails.ReactionCountOnComment = likecount;

                        objFdPostCommentDetails.CommentUrl = $"{FdConstants.FbHomeUrl}{commentId}";

                        if (ObjFdScraperResponseParameters.CommentList.FirstOrDefault(x => x.CommentId == objFdPostCommentDetails.CommentId) == null && (postId == commentPostId || postUrl.Contains(commentPostId)))
                            ObjFdScraperResponseParameters.CommentList.Add(objFdPostCommentDetails);

                    }
                    catch (Exception ex)
                    {
                        if (isPostsProcess)
                            Console.WriteLine(ex.StackTrace);
                        else
                            ex.DebugLog(ex.Message);
                    }

                }


                if (commentArray.Length <= 1)
                {
                    commentArray = Regex.Split(pageLikersData, "body:{text").Skip(1).ToArray();
                }


                foreach (var commentDetails in commentArray)
                {
                    FdPostCommentDetails objFdPostCommentDetails = new FdPostCommentDetails();

                    try
                    {

                        postCommentorId = FdRegexUtility.FirstMatchExtractor(commentDetails, "author:\"(.*?)\"");
                        postCommentorId = FdFunctions.GetIntegerOnlyString(postCommentorId);

                        objFdPostCommentDetails.CommenterID = postCommentorId;

                        commentText = FdRegexUtility.FirstMatchExtractor(commentDetails, ":\"(.*?)\"");

                        objFdPostCommentDetails.CommentText = commentText;

                        commentId = FdRegexUtility.FirstMatchExtractor(commentDetails, "fbid:\"(.*?)\"");

                        objFdPostCommentDetails.CommentId = commentId;

                        commentPostId = FdRegexUtility.FirstMatchExtractor(commentDetails, "ftentidentifier:\"(.*?)\"");

                        objFdPostCommentDetails.PostId = commentPostId;

                        hasLikedByUser = FdRegexUtility.FirstMatchExtractor(commentDetails, "hasviewerliked:(.*?),");

                        objFdPostCommentDetails.HasLikedByUser = hasLikedByUser;

                        commentDateTime = FdRegexUtility.FirstMatchExtractor(commentDetails, "verbose:\"(.*?)\"");

                        objFdPostCommentDetails.CommentTimeWithDate = commentDateTime;

                        likecount = FdRegexUtility.FirstMatchExtractor(commentDetails, "likecount:(.*?),");

                        objFdPostCommentDetails.ReactionCountOnComment = likecount;


                        objFdPostCommentDetails.CommentUrl = $"{FdConstants.FbHomeUrl}{commentId}";

                        if (ObjFdScraperResponseParameters.CommentList.FirstOrDefault(x => x.CommentId == objFdPostCommentDetails.CommentId) == null && (postId == commentPostId || postUrl.Contains(commentPostId)))
                        {
                            ObjFdScraperResponseParameters.CommentList.Add(objFdPostCommentDetails);
                        }


                    }
                    catch (Exception ex)
                    {
                        if (isPostsProcess)
                            Console.WriteLine(ex.StackTrace);
                        else
                            ex.DebugLog(ex.Message);
                    }
                }

                Status = ObjFdScraperResponseParameters.CommentList.Count > 0;
            }
            catch (Exception ex)
            {
                if (isPostsProcess)
                    Console.WriteLine(ex.StackTrace);
                else
                    ex.DebugLog(ex.Message);
            }

        }
        private void GetPostCommentorNewUI(string pageLikersData, string postId, bool isPostsProcess, string postUrl = "")
        {

            try
            {
                string[] commentArray = Regex.Split(pageLikersData, "\"body\":").Skip(1).ToArray();

                //var list=HtmlParseUtility.GetListInnerHtmlFromPartialTagNamecontains(pageLikersData, "div", "class", "l9j0dhe7 ecm0bbzt hv4rvrfc qt6c0cv9");

                string postCommentorId;
                string commentDateTime;
                string likecount;
                string hasLikedByUser;
                string commentText;
                string commentId;
                string commentPostId;

                commentArray.Reverse();

                foreach (var commentDetails in commentArray)
                {

                    if (commentDetails.Contains(postId))
                    {

                    }

                    FdPostCommentDetails objFdPostCommentDetails = new FdPostCommentDetails();

                    try
                    {
                        //"\"name\":\"(.*?)\"" == commenter name
                        postCommentorId = FdRegexUtility.FirstMatchExtractor(commentDetails, "{\"__typename\":\"User\",\"id\":\"(.*?)\"");

                        postCommentorId = FdFunctions.GetIntegerOnlyString(postCommentorId);

                        objFdPostCommentDetails.CommenterID = postCommentorId;

                        commentText = FdRegexUtility.FirstMatchExtractor(commentDetails, ":\"(.*?)\"");

                        objFdPostCommentDetails.CommentText = commentText;

                        commentId = FdRegexUtility.FirstMatchExtractor(commentDetails, "comment_id=(.*?)\"");

                        objFdPostCommentDetails.CommentId = commentId;

                        commentPostId = FdRegexUtility.FirstMatchExtractor(commentDetails, "share_fbid\":\"(.*?)\"");

                        objFdPostCommentDetails.PostId = commentPostId;

                        hasLikedByUser = FdRegexUtility.FirstMatchExtractor(commentDetails, "hasviewerliked:(.*?),");

                        objFdPostCommentDetails.HasLikedByUser = hasLikedByUser;

                        commentDateTime = FdRegexUtility.FirstMatchExtractor(commentDetails, "time:(.*?),");

                        //objFdPostCommentDetails.CommentTimeWithDate = Int32.Parse(commentDateTime).EpochToDateTimeUtc().ToString(CultureInfo.InvariantCulture);

                        likecount = FdRegexUtility.FirstMatchExtractor(commentDetails, "{\"count\":(.*?),");

                        objFdPostCommentDetails.ReactionCountOnComment = likecount;

                        objFdPostCommentDetails.CommentUrl = $"{FdConstants.FbHomeUrl}{commentId}";

                        if (ObjFdScraperResponseParameters.CommentList.FirstOrDefault(x => x.CommentId == objFdPostCommentDetails.CommentId) == null && (postId == commentPostId || postUrl.Contains(commentPostId)))
                            ObjFdScraperResponseParameters.CommentList.Add(objFdPostCommentDetails);

                    }
                    catch (Exception ex)
                    {
                        if (isPostsProcess)
                            Console.WriteLine(ex.StackTrace);
                        else
                            ex.DebugLog(ex.Message);
                    }

                }


                if (commentArray.Length <= 1)
                {
                    commentArray = Regex.Split(pageLikersData, "body:{text").Skip(1).ToArray();
                }


                foreach (var commentDetails in commentArray)
                {
                    FdPostCommentDetails objFdPostCommentDetails = new FdPostCommentDetails();

                    try
                    {

                        postCommentorId = FdRegexUtility.FirstMatchExtractor(commentDetails, "author:\"(.*?)\"");
                        postCommentorId = FdFunctions.GetIntegerOnlyString(postCommentorId);

                        objFdPostCommentDetails.CommenterID = postCommentorId;

                        commentText = FdRegexUtility.FirstMatchExtractor(commentDetails, ":\"(.*?)\"");

                        objFdPostCommentDetails.CommentText = commentText;

                        commentId = FdRegexUtility.FirstMatchExtractor(commentDetails, "fbid:\"(.*?)\"");

                        objFdPostCommentDetails.CommentId = commentId;

                        commentPostId = FdRegexUtility.FirstMatchExtractor(commentDetails, "ftentidentifier:\"(.*?)\"");

                        objFdPostCommentDetails.PostId = commentPostId;

                        hasLikedByUser = FdRegexUtility.FirstMatchExtractor(commentDetails, "hasviewerliked:(.*?),");

                        objFdPostCommentDetails.HasLikedByUser = hasLikedByUser;

                        commentDateTime = FdRegexUtility.FirstMatchExtractor(commentDetails, "verbose:\"(.*?)\"");

                        objFdPostCommentDetails.CommentTimeWithDate = commentDateTime;

                        likecount = FdRegexUtility.FirstMatchExtractor(commentDetails, "likecount:(.*?),");

                        objFdPostCommentDetails.ReactionCountOnComment = likecount;


                        objFdPostCommentDetails.CommentUrl = $"{FdConstants.FbHomeUrl}{commentId}";

                        if (ObjFdScraperResponseParameters.CommentList.FirstOrDefault(x => x.CommentId == objFdPostCommentDetails.CommentId) == null && (postId == commentPostId || postUrl.Contains(commentPostId)))
                        {
                            ObjFdScraperResponseParameters.CommentList.Add(objFdPostCommentDetails);
                        }


                    }
                    catch (Exception ex)
                    {
                        if (isPostsProcess)
                            Console.WriteLine(ex.StackTrace);
                        else
                            ex.DebugLog(ex.Message);
                    }
                }

                Status = ObjFdScraperResponseParameters.CommentList.Count > 0;
            }
            catch (Exception ex)
            {
                if (isPostsProcess)
                    Console.WriteLine(ex.StackTrace);
                else
                    ex.DebugLog(ex.Message);
            }

        }

        private void GetPostCommentorJson(string pageLikersData, bool isPostsProcess)
        {

            try
            {

                if (pageLikersData.Contains("\"errors\":"))
                {
                    pageLikersData = Regex.Split(pageLikersData, ",\n   \"errors\"")[0];

                    pageLikersData += "}";
                }

                JObject commentList = JObject.Parse(pageLikersData);

                var messageDetails = commentList["data"]["feedback"]["display_comments"]["edges"];



                ObjFdScraperResponseParameters.CommentList = (from token in messageDetails
                                                              let postCommentorId = token["node"]["author"]["id"]
                                                              let commentDateTime = token["node"]["created_time"]
                                                              let likecount = token["node"]["feedback"]["reactors"]["count"]
                                                              let commentText = token["node"]["body"].Children().Count() == 0 ? null : token["node"]["body"]["text"]
                                                              let commentId = token["node"]["legacy_fbid"]
                                                              let legacyToken = token["node"]["legacy_token"]
                                                              let commenterName = token["node"]["author"]["name"]
                                                              where commentId != null && commentText != null
                                                              select new FdPostCommentDetails
                                                              {
                                                                  CommentId = commentId.ToString(),
                                                                  CommentText = commentText.ToString(),
                                                                  CommentTimeWithDate = int.Parse(commentDateTime.ToString()).EpochToDateTimeUtc().ToString(CultureInfo.InvariantCulture),
                                                                  ReactionCountOnComment = likecount.ToString(),
                                                                  CommenterID = postCommentorId.ToString(),
                                                                  CommenterName = commenterName.ToString(),
                                                                  HasLikedByUser = "false",
                                                                  CommentUrl = FdConstants.FbHomeUrl + commentId,
                                                                  PostId = FdRegexUtility.FirstMatchExtractor(legacyToken.ToString(), "(.*?)_")
                                                              }).ToList();

                Status = ObjFdScraperResponseParameters.CommentList.Count > 0;
            }
            catch (Exception ex)
            {
                if (isPostsProcess)
                    Console.WriteLine(ex.StackTrace);
                else
                    ex.DebugLog(ex.Message);
            }

        }


        private void UpadetePaginationData(string responseParameter, bool isFirstPagination, bool isPostsProcess)
        {
            try
            {
                //{ "story_width":230,"is_narrow":true,"fbfeed_context":true}
                if (isFirstPagination)
                {
                    if (responseParameter.Contains("feedcontext\":\""))
                    {
                        var fbFeedContext = Regex.Split(responseParameter, "feedcontext\":\"")[1];


                        var feedContext = FdRegexUtility.FirstMatchExtractor(fbFeedContext, "fbfeed_context\":(.*?),");

                        var storyWidth = FdRegexUtility.FirstMatchExtractor(fbFeedContext, "story_width\":(.*?),");

                        ObjFdScraperResponseParameters.FeedContext = "{\"story_width\":" + storyWidth + ",\"is_narrow\":true,\"fbfeed_context\":" + feedContext + "}";
                    }

                }
                if (responseParameter.Contains("end_cursor:\"")
                || responseParameter.Contains("end_cursor\":"))
                {
                    var endCursor = FdRegexUtility.FirstMatchExtractor(responseParameter, "end_cursor:\"(.*?)\"");
                    var feedLocation = FdRegexUtility.FirstMatchExtractor(responseParameter, "feedLocation:\"(.*?)\"");
                    var feedbackId = FdRegexUtility.FirstMatchExtractor(responseParameter, "feedbackTargetID:\"(.*?)\"");

                    if (string.IsNullOrEmpty(endCursor))
                    {
                        var lstEndCursor = Regex.Split(responseParameter, "end_cursor\":");
                        endCursor = FdRegexUtility.FirstMatchExtractor(lstEndCursor.LastOrDefault(), "\"(.*?)\"");
                        var feedbackIdData = Regex.Split(responseParameter, "feedback\":");
                        feedbackId = FdRegexUtility.FirstMatchExtractor(feedbackIdData[1] ?? feedbackIdData[0], "id\": \"(.*?)\"");
                        feedLocation = ObjFdScraperResponseParameters.FeedLocation;
                    }

                    if (ObjFdScraperResponseParameters.Offset == 0)
                        ObjFdScraperResponseParameters.Offset = ObjFdScraperResponseParameters.CommentCount;

                    string completeFeedLocation = "\"feedLocation\":null";

                    completeFeedLocation = string.IsNullOrEmpty(completeFeedLocation)
                        ? completeFeedLocation
                        : $"\"feedLocation\":\"{feedLocation}\"";

                    ObjFdScraperResponseParameters.Offset = ObjFdScraperResponseParameters.Offset - ObjFdScraperResponseParameters.CommentList.Count;
                    PageletData =
                        "{\"after\":\"" + endCursor + "\"," + completeFeedLocation + ",\"feedbackID\":\"" + feedbackId +
                        "\",\"feedbackSource\":2,\"first\":50,\"isComet\":false}";
                    ObjFdScraperResponseParameters.FeedLocation = feedLocation;
                    //{ "after":"AQHRGS6fRJfon2kas-O9-HNit091-OG80u055u2FQDlslTzwHd1C_HnnO4SC-rsIxq4lClB0_Zctk4F_bGGAQetzvQ","feedLocation":"GROUP_PERMALINK","feedbackID":"ZmVlZGJhY2s6OTMyNTI3MjgwMjgzNzgw","feedbackSource":2,"first":25}{"after":"AQHRGS6fRJfon2kas-O9-HNit091-OG80u055u2FQDlslTzwHd1C_HnnO4SC-rsIxq4lClB0_Zctk4F_bGGAQetzvQ","feedLocation":"GROUP_PERMALINK","feedbackID":"ZmVlZGJhY2s6OTMyNTI3MjgwMjgzNzgw","feedbackSource":2,"first":25}
                }

            }
            catch (Exception ex)
            {
                if (isPostsProcess)
                    Console.WriteLine(ex.StackTrace);
                else
                    ex.DebugLog(ex.Message);
            }
        }
    }
}
