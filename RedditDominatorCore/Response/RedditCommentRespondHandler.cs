using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using Newtonsoft.Json.Linq;
using RedditDominatorCore.RDLibrary;
using RedditDominatorCore.RDModel;
using RedditDominatorCore.RDUtility;
using System;
using System.Collections.Generic;
using System.Linq;
namespace RedditDominatorCore.Response
{
    public class RedditCommentRespondHandler : RdResponseHandler
    {
        public List<RedditPost> LstCommentOnRedditPost = new List<RedditPost>();
        
        public RedditCommentRespondHandler(IResponseParameter response, bool isPaginatedData,
            PaginationParameter paginationParameter, string userid = "") : base(response)
        {
            try
            {
                Response = response.Response;
                if (!Success)
                    return;
                if (isPaginatedData)
                {
                    PaginationParameter = paginationParameter;
                    var json2 = response.Response;
                    var jsonobject = JObject.Parse(json2);
                    IEnumerable<JToken> jSonData = jsonobject["comments"].Children();
                    NewPageResponse(jSonData, userid);
                }
                else
                {
                    string notRequiredData = string.Empty;
                    PaginationParameter = new PaginationParameter();
                    var json2 = RdConstants.GetJsonPageResponse(response.Response);
                    if (string.IsNullOrEmpty(json2))
                        json2 = response.Response;
                    if (json2.IsValidJson())
                    {
                        notRequiredData = Utilities.GetBetween(json2, "\"widgets\":", ",\"features\"");
                        if (!string.IsNullOrEmpty(notRequiredData))
                        {
                            json2 = json2.Replace(notRequiredData, "{}").Trim();
                            response.Response = json2;
                        }
                        var jsonobject = JObject.Parse(json2);
                        var id = jsonobject["features"]["comments"]["focused"].ToString().Replace("{", "").Replace("}", "").Trim();
                        var node = Utilities.GetBetween(id, "\"", "\": true");
                        IEnumerable<JToken> jsonData = jsonobject["features"]["comments"]["models"].Children();
                        GetPagination(response);
                        NewPageResponse(jsonData, userid);
                    }
                    else {
                        try
                        {
                            var nodes=HtmlUtility.GetListOfNodesFromTagNameWithoutAttribute(Response, "shreddit-comment");
                            if (nodes != null)
                            {
                                foreach (var node in nodes)
                                {
                                    var redditPost = new RedditPost();
                                    var textNodes=HtmlUtility.GetListOfNodesFromTagName(node.InnerHtml, "div", "slot", "comment");
                                    redditPost.Commenttext = textNodes[0]?.InnerText?.Trim();
                                    redditPost.PostId = node.GetAttributeValue("postid", string.Empty);
                                    redditPost.Permalink = $"{RdConstants.GetRedditHomePageAPI}{node.GetAttributeValue("permalink", string.Empty)}";
                                    try
                                    {
                                        var commentDetails = node.SelectSingleNode("shreddit-comment-action-row");
                                        if (commentDetails == null)
                                        {
                                            commentDetails = HtmlUtility.GetListOfNodesFromTagNameWithoutAttribute(node.InnerHtml, "shreddit-comment-action-row").FirstOrDefault();
                                        }
                                        redditPost.Id = commentDetails.GetAttributeValue("comment-id", string.Empty);

                                        int.TryParse(commentDetails.GetAttributeValue("score", string.Empty), out int score);
                                        redditPost.Score = score;
                                        redditPost.Author = node.GetAttributeValue("author", string.Empty);
                                        redditPost.MediaType = node.GetAttributeValue("post-type", string.Empty);
                                        redditPost.Upvoted = commentDetails.GetAttributeValue("vote-state", string.Empty).Contains("UP");
                                        redditPost.Downvoted = commentDetails.GetAttributeValue("vote-state", string.Empty).Contains("DOWN");
                                    }
                                    catch (Exception)
                                    {
                                    }
                                    LstCommentOnRedditPost.Add(redditPost);
                                }
                            }
                        }
                        catch (Exception)
                        {
                        }
                        PaginationParameter.LastPaginationId = Utilities.GetBetween(Response, "postsCursor=", "\"");
                    }
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public PaginationParameter PaginationParameter { get; set; }

        public void GetPagination(IResponseParameter responseParameter)
        {
            var jsonResponse = RdConstants.GetJsonPageResponse(responseParameter.Response);
            if (string.IsNullOrEmpty(jsonResponse))
                jsonResponse = responseParameter.Response;
            var jsonObject = JObject.Parse(jsonResponse);
            PaginationParameter.SessionTracker = jsonObject["user"]["sessionTracker"].ToString();
            PaginationParameter.Reddaid = jsonObject["user"]["reddaid"]?.ToString();
            var loidLoid = jsonObject["user"]["loid"]["loid"].ToString();
            var loidBlob = jsonObject["user"]["loid"]["blob"].ToString();
            var loidCreated = jsonObject["user"]["loid"]["loidCreated"].ToString();
            var loidVersion = jsonObject["user"]["loid"]["version"].ToString();
            PaginationParameter.Loid = loidLoid + "." + loidVersion + "." + loidCreated + "." + loidBlob;
            PaginationParameter.AccessToken = jsonObject["user"]["session"]["accessToken"].ToString();
        }

        public void NewPageResponse(IEnumerable<JToken> responseJToken, string userid)
        {
            try
            {
                foreach (var data in responseJToken)
                {
                    var redditPost = new RedditPost();
                    var userId = data.First["id"].ToString();
                    userId = GetRedditUserName(userId);
                    if (userId != userid && !string.IsNullOrEmpty(userid)) continue;
                    var num2 = Convert.ToBoolean(data.First["isStickied"].ToString()) ? 1 : 0;
                    redditPost.IsStickied = num2 != 0;
                    redditPost.Author = data.First["author"].ToString();
                    redditPost.Id = data.First["id"].ToString();
                    redditPost.Score = (int)data.First["score"];
                    var num4 = Convert.ToBoolean(data.First["sendReplies"].ToString()) ? 1 : 0;
                    redditPost.SendReplies = num4 != 0;
                    redditPost.GoldCount = (int)data.First["goldCount"];
                    redditPost.PostId = data.First["postId"].ToString();
                    redditPost.Permalink = data.First["permalink"].ToString();
                    redditPost.Permalink = redditPost.Permalink.Contains("https://www.reddit.com") ? redditPost.Permalink.ToString() : "https://www.reddit.com" + redditPost.Permalink.ToString();
                    redditPost.Created = (long)data.First["created"];
                    try
                    {
                        redditPost.Commenttext = data.First["media"]["richtextContent"]["document"][0]["c"][0]["t"]
                            .ToString();
                        redditPost.OldComment = redditPost.Commenttext;
                    }
                    catch (Exception)
                    {
                        redditPost.Commenttext =
                            data.First["media"]["richtextContent"]["document"][0]["c"][0]["c"][0]["t"].ToString();
                        redditPost.OldComment = redditPost.Commenttext;
                    }
                    finally
                    {
                        redditPost.DistinguishType = data.First["distinguishType"].ToString();
                        redditPost.VoteState = (int)data.First["voteState"];
                        PaginationParameter.LastPaginationId = redditPost.Id;
                        redditPost.PaginationParameter = PaginationParameter;
                        redditPost.NumComments = responseJToken.Count();
                        LstCommentOnRedditPost.Add(redditPost);
                    }
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public static string GetRedditUserName(string inputString)
        {
            if (!inputString.Contains("_")) return inputString;
            var text = inputString.Split('_').ToList();
            inputString = text.Last();
            return inputString;
        }
    }
}