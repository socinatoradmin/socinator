using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDResponse.BaseResponse;
using FaceDominatorCore.Interface;
using Newtonsoft.Json.Linq;
using System;
using System.Text.RegularExpressions;

namespace FaceDominatorCore.FDResponse.CommonResponse
{

    public class ReactionCountResponseHandler : FdResponseHandler, IResponseHandler
    {

        public int LikeCount { get; set; }

        public int CommentCount { get; set; }

        public int ShareCount { get; set; }

        public int Offset { get; set; }

        public bool Status { get; set; }

        public bool HasMoreResults { get; set; }

        public string EntityId { get; set; }

        public string PageletData { get; set; }

        public FdScraperResponseParameters ObjFdScraperResponseParameters { get; set; }
            = new FdScraperResponseParameters();

        public ReactionCountResponseHandler(IResponseParameter responseParameter, ref string postId)
            : base(responseParameter)
        {
            if (responseParameter.HasError)
                return;

            if (FbErrorDetails == null)
            {

                string decodedResponse = FdFunctions.GetNewPrtialDecodedResponse(responseParameter.Response);

                if (decodedResponse.Contains("<!DOCTYPE html>"))
                    GetNormalData(decodedResponse, postId);

                if (CommentCount == 0 || Offset == 0)
                    GetDataInJson(decodedResponse, ref postId);


                if (CommentCount == 0 && ShareCount == 0 && LikeCount == 0 && Offset == 0)
                    GetDataInJsonPagination(responseParameter.Response, ref postId);


                if (CommentCount == 0 && !decodedResponse.Contains("<!DOCTYPE html>"))
                    GetNormalData(decodedResponse, postId);

                //Offset = CommentCount != 0 && Offset==0 ? CommentCount : Offset;

            }

        }

        public void GetDataInJson(string decodedResponse, ref string postId)
        {
            try
            {
                var jsonResponse = string.Empty;
                var jsonOffsetResponse = string.Empty;
                //""[" + FdRegexUtility.FirstMatchExtractor(decodedResponse, "\"feedbacktarget\":(.*?),\"viewerreaction\"") + "}]";"

                MatchCollection matches = Regex.Matches(decodedResponse, "\"feedbacktarget\":(.*?),\"viewerreaction\"");

                if (matches.Count > 0)
                    foreach (Match match in matches)
                        if (match.Groups[0].ToString().Contains(postId))
                            jsonResponse = "[" + match.Groups[1].ToString() + "}]";

                JArray objArray = JArray.Parse(jsonResponse);

                var ftEntIdentifier = string.Empty;


                try
                {
                    var likeCount = objArray[0]["likecount"].ToString();

                    var shareCount = objArray[0]["sharecountreduced"].ToString();

                    var commentCount = objArray[0]["commentcountreduced"].ToString();

                    ftEntIdentifier = objArray[0]["commentstargetfbid"].ToString();


                    LikeCount = Int32.Parse(likeCount);

                    ShareCount = Int32.Parse(shareCount);

                    CommentCount = Int32.Parse(commentCount);


                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                string offset;

                matches = Regex.Matches(decodedResponse, "\"commentlists\":(.*?),\"featuredcommentlists");


                if (matches.Count > 0)
                    foreach (Match match in matches)
                        if (match.Groups[0].ToString().Contains(postId))
                            jsonOffsetResponse = "[" + match.Groups[1].ToString() + "]";

                //jsonOffsetResponse = "[" + FdRegexUtility.FirstMatchExtractor(decodedResponse, "\"commentlists\":(.*?),\"featuredcommentlists") + "]";

                JArray objJsonOffsetArray = JArray.Parse(jsonOffsetResponse);

                try
                {
                    offset =
                        objJsonOffsetArray[0]["comments"][ftEntIdentifier]["toplevel"]
                        != null
                            ? objJsonOffsetArray[0]["comments"][ftEntIdentifier]["toplevel"]["range"]["offset"]
                                .ToString()
                            : (objJsonOffsetArray[0]["comments"][ftEntIdentifier]["ranked_threaded"] != null
                                ? objJsonOffsetArray[0]["comments"][ftEntIdentifier]["ranked_threaded"]["range"][
                                    "offset"].ToString()
                                : objJsonOffsetArray[0]["comments"][ftEntIdentifier]["filtered"]["range"][
                                    "offset"].ToString());
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                    offset = objJsonOffsetArray[0]["comments"][ftEntIdentifier]["toplevel"]["range"]["offset"].ToString();

                }


                Offset = Int32.Parse(offset);

                postId = ftEntIdentifier;

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }


        }

        public void GetNormalData(string decodedResponse, string postId)
        {
            try
            {
                string[] postReactionArray = Regex.Split(decodedResponse, "commentcount\":");

                if (postReactionArray.Length <= 1)
                    postReactionArray = Regex.Split(decodedResponse, "commentcount:");

                if (postReactionArray.Length <= 1)
                    postReactionArray = Regex.Split(decodedResponse, ",comment_count");

                foreach (string postData in postReactionArray)
                {
                    if (postData.Contains($"commentstargetfbid\":\"{postId}\""))
                    {
                        CommentCount = Int32.Parse(FdFunctions.GetIntegerOnlyString(Regex.Matches(postData, "(.*?),", RegexOptions.Singleline)[0].Groups[1].ToString()));
                        LikeCount = Int32.Parse(FdFunctions.GetIntegerOnlyString(Regex.Matches(postData, "likecount\":(.*?),", RegexOptions.Singleline)[0].Groups[1].ToString()));
                        ShareCount = Int32.Parse(FdFunctions.GetIntegerOnlyString(Regex.Matches(postData, "sharecount\":(.*?),", RegexOptions.Singleline)[0].Groups[1].ToString()));
                    }
                    else if (postData.Contains($"commentstargetfbid:\"{postId}\""))
                    {
                        CommentCount = Int32.Parse(FdFunctions.GetIntegerOnlyString(Regex.Matches(postData, "commentTotalCount:(.*?),", RegexOptions.Singleline)[0].Groups[1].ToString()));
                        LikeCount = Int32.Parse(FdFunctions.GetIntegerOnlyString(Regex.Matches(postData, "likecount:(.*?),", RegexOptions.Singleline)[0].Groups[1].ToString()));
                        ShareCount = Int32.Parse(FdFunctions.GetIntegerOnlyString(Regex.Matches(postData, "sharecountreduced:\"(.*?)\"", RegexOptions.Singleline)[0].Groups[1].ToString()));

                    }
                    else if (postData.Contains("comment_count:"))
                    {
                        CommentCount = Int32.Parse(FdFunctions.GetIntegerOnlyString(Regex.Matches(postData, "total_count:(.*?)\\},", RegexOptions.Singleline)[0].Groups[1].ToString()));
                        LikeCount = Int32.Parse(FdFunctions.GetIntegerOnlyString(Regex.Matches(postData, "{i18n_reaction_count:(.*?),", RegexOptions.Singleline)[0].Groups[1].ToString()));
                        ShareCount = Int32.Parse(FdFunctions.GetIntegerOnlyString(Regex.Matches(postData, "i18n_share_count:\"(.*?)\"", RegexOptions.Singleline)[0].Groups[1].ToString()));

                    }
                    else if (postData.Contains("commentcountreduced") && decodedResponse.Contains(postId) &&
                             decodedResponse.Contains("\"sidePane\":"))
                    {
                        CommentCount = Int32.Parse(FdFunctions.GetIntegerOnlyString(Regex.Matches(postData, "commentcountreduced\":\"(.*?),", RegexOptions.Singleline)[0].Groups[1].ToString()));
                        LikeCount = Int32.Parse(FdFunctions.GetIntegerOnlyString(Regex.Matches(postData, "{i18n_reaction_count:(.*?),", RegexOptions.Singleline)[0].Groups[1].ToString()));
                        ShareCount = Int32.Parse(FdFunctions.GetIntegerOnlyString(Regex.Matches(postData, "i18n_share_count:\"(.*?)\"", RegexOptions.Singleline)[0].Groups[1].ToString()));
                    }
                    if (postData.Contains("{\"filtered\":{\"range\":{\""))
                    {
                        try
                        {
                            var offsetDescArray = Regex.Split(postData, "{\"filtered\":{\"range\":{\"");

                            if (offsetDescArray.Length > 1)
                                Offset = Int32.Parse(FdFunctions.GetIntegerOnlyString(Regex.Matches(offsetDescArray[1], "offset\":(.*?),", RegexOptions.Singleline)[0].Groups[1].ToString()));
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }
                    }
                    else if (postData.Contains("{toplevel:{range:{"))
                    {
                        try
                        {
                            var offsetDescArray = Regex.Split(postData, "{toplevel:{range:{");

                            if (offsetDescArray.Length > 1)
                                Offset = Int32.Parse(FdFunctions.GetIntegerOnlyString(Regex.Matches(offsetDescArray[1], "offset:(.*?),", RegexOptions.Singleline)[0].Groups[1].ToString()));
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }
                    }
                }



            }
            catch (Exception ex)
            {
                ex.DebugLog(ex.Message);
            }
        }


        public void GetDataInJsonPagination(string decodedResponse, ref string postId)
        {
            try
            {
                var jsonResponse = string.Empty;

                //""[" + FdRegexUtility.FirstMatchExtractor(decodedResponse, "\"feedbacktarget\":(.*?),\"viewerreaction\"") + "}]";"
                if (decodedResponse.Contains("for (;;);"))
                {
                    jsonResponse = "[" + Regex.Replace(decodedResponse, "for \\(;;\\);", string.Empty) + "]";

                    JArray objJsonOffsetArray = JArray.Parse(jsonResponse);

                    try
                    {
                        var offset =
                            objJsonOffsetArray[0]["jsmods"]["require"][0][3][1]["commentlists"]["comments"][postId][
                                "ranked_threaded"]["range"]["offset"];

                        Offset = int.Parse(offset.ToString());
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();


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
