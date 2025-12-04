using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDResponse.BaseResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace FaceDominatorCore.FDResponse.CommonResponse
{
    public class PostReactionListResponseHandler : FdResponseHandler
    {
        public List<KeyValuePair<string, string>> ListPostReaction = new List<KeyValuePair<string, string>>();

        public List<KeyValuePair<string, string>> ListReactionPermission = new List<KeyValuePair<string, string>>();

        public PostReactionListResponseHandler(IResponseParameter responseParameter)
            : base(responseParameter)
        {
            if (responseParameter.HasError)
                return;

            string decodedResponse = FdFunctions.GetDecodedResponse(responseParameter.Response);
            try
            {

                string[] postReactionArray = Regex.Split(decodedResponse, "actorforpost\":");

                if (postReactionArray.Length <= 1)
                    postReactionArray = Regex.Split(decodedResponse, "actorforpost:");

                if (postReactionArray.Length <= 1)
                    postReactionArray = Regex.Split(decodedResponse, "name=\"ft_ent_identifier\" ");

                foreach (string postData in postReactionArray)
                {
                    int likeCount = 0;

                    int commentCount = 0;

                    int shareCount = 0;

                    string canReact = string.Empty;

                    string canComment = string.Empty;

                    var postId = FdRegexUtility.FirstMatchExtractor(postData, "commentstargetfbid\":\"(.*?)\"");

                    if (string.IsNullOrEmpty(postId))
                        postId = FdRegexUtility.FirstMatchExtractor(postData, "commentstargetfbid:\"(.*?)\"");

                    if (string.IsNullOrEmpty(postId))
                        postId = FdRegexUtility.FirstMatchExtractor(postData, "post_fbid\":(.*?)\"");

                    if (string.IsNullOrEmpty(postId) && postData.StartsWith("value=\""))
                        postId = FdRegexUtility.FirstMatchExtractor(postData, "\"(.*?)\"");

                    if (string.IsNullOrEmpty(postId))
                        continue;

                    postId = FdFunctions.GetIntegerOnlyString(postId);

                    if (postData.Contains($"commentstargetfbid\":\"{postId}\""))
                    {
                        commentCount = Int32.Parse(FdFunctions.GetIntegerOnlyString(FdRegexUtility.FirstMatchExtractor(postData, "commentTotalCount\":(.*?),")));
                        likeCount = Int32.Parse(FdFunctions.GetIntegerOnlyString(FdRegexUtility.FirstMatchExtractor(postData, "reactioncount\":(.*?),")));
                        shareCount = Int32.Parse(FdFunctions.GetIntegerOnlyString(FdRegexUtility.FirstMatchExtractor(postData, "sharecount\":(.*?),")));

                        canReact = FdRegexUtility.FirstMatchExtractor(postData, FdConstants.CanCommentRegx);
                        canComment = FdRegexUtility.FirstMatchExtractor(postData, "viewercanlike\":(.*?),");
                    }
                    else if (postData.Contains($"commentstargetfbid:\"{postId}\""))
                    {
                        commentCount = Int32.Parse(FdFunctions.GetIntegerOnlyString(FdRegexUtility.FirstMatchExtractor(postData, "commentcount:(.*?),")));
                        likeCount = Int32.Parse(FdFunctions.GetIntegerOnlyString(FdRegexUtility.FirstMatchExtractor(postData, "reactioncount:(.*?),")));
                        shareCount = Int32.Parse(FdFunctions.GetIntegerOnlyString(FdRegexUtility.FirstMatchExtractor(postData, "sharecount:(.*?),")));

                        canReact = FdRegexUtility.FirstMatchExtractor(postData, "cancomment:(.*?),");
                        canComment = FdRegexUtility.FirstMatchExtractor(postData, "viewercanlike:(.*?),");
                    }
                    else if (responseParameter.Response.Contains(",comment_count:{"))
                    {
                        var reactionDetailsList = Regex.Split(responseParameter.Response, ",comment_count:{").ToList();

                        var currentPostReactionData = reactionDetailsList.FirstOrDefault(x => x.Contains($"share_fbid:\"{postId}\""));

                        commentCount = currentPostReactionData == null ? 0 : int.Parse(FdFunctions.GetIntegerOnlyString(FdRegexUtility.FirstMatchExtractor(currentPostReactionData, "total_count:(.*?)}")));
                        likeCount = currentPostReactionData == null ? 0 : int.Parse(FdFunctions.GetIntegerOnlyString(FdRegexUtility.FirstMatchExtractor(currentPostReactionData, "reaction_count:{count:(.*?)}")));
                        shareCount = currentPostReactionData == null ? 0 : int.Parse(FdFunctions.GetIntegerOnlyString(FdRegexUtility.FirstMatchExtractor(currentPostReactionData, "share_count:{count:(.*?)}")));

                    }

                    if (!string.IsNullOrEmpty(postId))
                    {
                        string count = commentCount + "<:>" + likeCount + "<:>" + shareCount;
                        KeyValuePair<string, string> objKeyvaluePair = new KeyValuePair<string, string>(postId, count);

                        ListPostReaction.Add(objKeyvaluePair);

                        string reactionPermissionStatus = canReact + "<:>" + canComment;
                        KeyValuePair<string, string> objKeyvaluePairReactionPerm = new KeyValuePair<string, string>(postId, reactionPermissionStatus);

                        ListReactionPermission.Add(objKeyvaluePairReactionPerm);
                    }

                }

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }


        }
    }



}
