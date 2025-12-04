
using DominatorHouseCore.Interfaces;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDResponse.BaseResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace FaceDominatorCore.FDResponse.CommonResponse
{
    public class AdReactionListResponseHandler : FdResponseHandler
    {

        public List<KeyValuePair<string, string>> ListPostReaction = new List<KeyValuePair<string, string>>();

        public List<KeyValuePair<string, string>> ListReactionPermission = new List<KeyValuePair<string, string>>();



        public AdReactionListResponseHandler(IResponseParameter responseParameter)
            : base(responseParameter)
        {
            if (responseParameter.HasError)
                return;

            if (FbErrorDetails == null)
            {
                try
                {
                    string decodedResponse = FdFunctions.GetDecodedResponse(responseParameter.Response);

                    string[] postReactionArray = Regex.Split(decodedResponse, "_1dwg _1w_m");

                    if (postReactionArray.Length <= 1)
                    {
                        return;
                    }

                    var listReactionCount = UpdateReactionCount(responseParameter);

                    foreach (string postData in postReactionArray)
                    {
                        string canReact = string.Empty;

                        string canComment = string.Empty;

                        string adId = string.Empty;

                        if (postData.Contains("<!DOCTYPE html>"))
                        {
                            continue;
                        }

                        if (string.IsNullOrEmpty(adId))
                        {
                            adId = FdRegexUtility.FirstMatchExtractor(postData,
                                "shareableStoryContext:{ad_id:(.*?),");
                        }

                        if (string.IsNullOrEmpty(adId) ||
                            !FdFunctions.IsIntegerOnly(adId))
                        {
                            adId = FdRegexUtility.FirstMatchExtractor(postData,
                                "shareableStoryContext\":{\"ad_id\":(.*?),");
                        }

                        if ((string.IsNullOrEmpty(adId) ||
                             !FdFunctions.IsIntegerOnly(adId)) &&
                            adId != "null")
                        {
                            adId = FdRegexUtility.FirstMatchExtractor(postData,
                                "shareableStoryContext\":{\"ad_id\":\"(.*?)\"");
                        }

                        if (string.IsNullOrEmpty(adId) || !FdFunctions.IsIntegerOnly(adId))
                            continue;

                        var postId = FdRegexUtility.FirstMatchExtractor(postData, "permalink\":\"(.*?)\"")
                            .Replace("\\", "");

                        if (string.IsNullOrEmpty(postId) ||
                            !FdFunctions.IsIntegerOnly(postId))
                        {
                            postId = FdRegexUtility.FirstMatchExtractor(postData, "permalink:\"(.*?)\"").Replace("\\", "");
                        }

                        if (string.IsNullOrEmpty(postId) ||
                            !FdFunctions.IsIntegerOnly(postId))
                        {
                            postId = FdRegexUtility.FirstMatchExtractor(postData, "ft_ent_identifier\" value=\"(.*?)\"")
                                .Replace("\\", "");
                        }

                        if (string.IsNullOrEmpty(postId) || !FdFunctions.IsIntegerOnly(postId))
                            continue;


                        if (!string.IsNullOrEmpty(postId) && !string.IsNullOrEmpty(adId) && adId.Length >= 2)
                        {
                            Tuple<string, string, string, string> currentAdReaction = listReactionCount.Where(x => x.Item1 == postId).FirstOrDefault();

                            string count = currentAdReaction.Item3 + "<:>" + currentAdReaction.Item2 + "<:>" + currentAdReaction.Item4
                                 + "<:>" + adId;
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
                    Console.WriteLine(ex.StackTrace);
                }
            }
        }


        public List<Tuple<string, string, string, string>> UpdateReactionCount(IResponseParameter responseParameter)
        {
            var reactionList = new List<Tuple<string, string, string, string>>();

            var reactionSplitCount = Regex.Split(responseParameter.Response, "comment_count:{");

            if (reactionSplitCount.Length <= 1)
                reactionSplitCount = Regex.Split(responseParameter.Response, "\"comment_count\":");

            foreach (var item in reactionSplitCount)
            {
                var commentCount = string.Empty;

                var shareCount = string.Empty;

                var likeCount = string.Empty;

                var postId = string.Empty;

                if (item.Contains("total_count:"))
                {
                    commentCount = FdRegexUtility.FirstMatchExtractor(item, "total_count:(.*?)}");

                    shareCount = FdRegexUtility.FirstMatchExtractor(item, "share_count:{count:(.*?)}");

                    likeCount = FdRegexUtility.FirstMatchExtractor(item, "reaction_count:{count:(.*?)}");

                    postId = FdRegexUtility.FirstMatchExtractor(item, "share_fbid:\"(.*?)\"");
                }
                else if (item.Contains("{\"total_count\":"))
                {
                    commentCount = FdRegexUtility.FirstMatchExtractor(item, "{\"total_count\":(.*?)}");

                    shareCount = FdRegexUtility.FirstMatchExtractor(item, "share_count\":{\"count\":(.*?)}");

                    likeCount = FdRegexUtility.FirstMatchExtractor(item, "reaction_count\":{\"count\":(.*?)}");

                    postId = FdRegexUtility.FirstMatchExtractor(item, "\"share_fbid\":\"(.*?)\"");
                }


                if (reactionList.FirstOrDefault(x => x.Item1 == postId) == null && !string.IsNullOrEmpty(postId))
                    reactionList.Add(new Tuple<string, string, string, string>(postId, likeCount, commentCount, shareCount));

            }

            return reactionList;
        }

    }



}


