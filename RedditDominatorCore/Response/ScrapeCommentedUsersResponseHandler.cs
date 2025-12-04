using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using Newtonsoft.Json.Linq;
using RedditDominatorCore.RDLibrary;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RedditDominatorCore.Response
{
    public class ScrapeCommentedUsersResponseHandler : RdResponseHandler
    {
        public List<string> LstCommentedUser = new List<string>();

        public ScrapeCommentedUsersResponseHandler(IResponseParameter response, bool isPaginatedData,
            PaginationParameter paginationParameter) : base(response)
        {
            try
            {
                var notRequiredData = string.Empty;
                Response = response.Response;
                if (!Success)
                    return;
                if (isPaginatedData)
                {
                    PaginationParameter = paginationParameter;
                    var jsonResp = response.Response;
                    var jsonObject = JObject.Parse(jsonResp);
                    IEnumerable<JToken> jSonData = jsonObject["comments"].Children();
                    NewPageResponse(jSonData);
                }
                else
                {
                    PaginationParameter = new PaginationParameter();
                    var jsonResp = RdConstants.GetJsonPageResponse(response.Response);
                    if (!string.IsNullOrEmpty(jsonResp))
                    {
                        notRequiredData = Utilities.GetBetween(jsonResp, "\"widgets\":", ",\"features\"");
                        if (!string.IsNullOrEmpty(notRequiredData))
                        {
                            jsonResp = jsonResp.Replace(notRequiredData, "{}").Trim();
                            response.Response = jsonResp;
                        }
                        var jsonObject = JObject.Parse(jsonResp);
                        IEnumerable<JToken> jSonData = jsonObject["features"]["comments"]["models"].Children();
                        GetPagination(response);
                        NewPageResponse(jSonData);
                    }
                    else
                    {
                        var Nodes = HtmlParseUtility.GetListNodeFromPartialTagNamecontains(response?.Response, "shreddit-comment", "permalink", "/comments/");
                        if(Nodes != null && Nodes.Count > 0)
                        {
                            foreach(var Node in Nodes)
                            {
                                var author = HtmlParseUtility.GetListInnerTextFromPartialTagNamecontains(Node?.InnerHtml, "a", "href", "/user/")?.LastOrDefault();
                                if(!string.IsNullOrEmpty(author) && !author.Contains("AutoModerator") && !author.Contains("deleted")
                                    && !LstCommentedUser.Any(z=>z == author?.Replace(" ", "")?.Replace("\r\n", "\n")?.Replace("\n", "")))
                                {
                                    LstCommentedUser.Add(author?.Replace(" ", "")?.Replace("\r\n","\n")?.Replace("\n",""));
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public PaginationParameter PaginationParameter { get; set; }

        public string BookMark { get; set; }
        public bool HasMoreResults { get; set; }

        public void GetPagination(IResponseParameter responseParameter)
        {
            var jsonObject = JObject.Parse(responseParameter.Response);
            PaginationParameter.SessionTracker = jsonObject["user"]["sessionTracker"].ToString();
            PaginationParameter.Reddaid = jsonObject["user"]["reddaid"]?.ToString();
            var loidLoid = jsonObject["user"]["loid"]["loid"].ToString();
            var loidBlob = jsonObject["user"]["loid"]["blob"].ToString();
            var loidCreated = jsonObject["user"]["loid"]["loidCreated"].ToString();
            var loidVersion = jsonObject["user"]["loid"]["version"].ToString();
            PaginationParameter.Loid = loidLoid + "." + loidVersion + "." + loidCreated + "." + loidBlob;
            PaginationParameter.AccessToken = jsonObject["user"]["session"]["accessToken"].ToString();
        }

        public void NewPageResponse(IEnumerable<JToken> responseJtoken)
        {
            try
            {
                foreach (var data in responseJtoken)
                {
                    var commentedUser = data.First["author"].ToString();
                    if (string.IsNullOrEmpty(commentedUser) || commentedUser.Contains("deleted"))
                        continue;
                    LstCommentedUser.Add(commentedUser);
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}