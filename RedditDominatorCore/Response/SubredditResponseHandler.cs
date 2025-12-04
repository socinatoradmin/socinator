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
    public class SubredditResponseHandler : RdResponseHandler
    {
        private readonly JsonHandler _jsonHand;
        public List<SubRedditModel> LstSubReddit = new List<SubRedditModel>();

        public SubredditResponseHandler(IResponseParameter response, bool isPaginatedData,
            PaginationParameter paginationParameter, string queryValue = "") : base(response)
        {
            try
            {
                var notRequiredData = string.Empty;
                Response = response.Response;
                if (!Success) return;

                if (isPaginatedData)
                {
                    PaginationParameter = paginationParameter;
                    var jsonResponse = response.Response;
                    var jsonObject = JObject.Parse(jsonResponse);
                    IEnumerable<JToken> modelsJson = jsonObject["subreddits"].Children();
                    IEnumerable<JToken> aboutJson = jsonObject["subredditAboutInfo"].Children();
                    NewPageResponse(modelsJson, aboutJson, queryValue);
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
                        _jsonHand = new JsonHandler(jsonResp);
                        IEnumerable<JToken> jTokenModel = _jsonHand.GetJToken("subreddits", "models").Children();
                        IEnumerable<JToken> jTokenAbout = _jsonHand.GetJToken("subreddits", "about").Children();
                        GetPagination(response);
                        NewPageResponse(jTokenModel, jTokenAbout, queryValue);
                    }
                    else
                    {
                        try
                        {
                            var nodes = HtmlUtility.GetListOfNodesFromTagName(Response, "faceplate-tracker", "noun", "subreddit");
                            if (nodes == null)
                                nodes = HtmlUtility.GetListOfNodesFromTagName(Response, "search-telemetry-tracker", "view-events", "search/view/subreddit");
                            if (nodes != null)
                            {
                                foreach (var node in nodes)
                                {
                                    var subReddit = new SubRedditModel();
                                    string PostData = node.GetAttributeValue("data-faceplate-tracking-context", null);
                                    string desc = HtmlParseUtility.GetInnerTextFromSingleNode(node.InnerHtml, "p", "data-testid", "search-subreddit-desc-text").Trim();
                                    subReddit.PublicDescription = desc;
                                    int.TryParse(Utilities.GetBetween(node.InnerHtml, "number=\"", "\""), out int subscribers);
                                    subReddit.Subscribers = subscribers;
                                    PostData = PostData.Replace("&quot;", "\"");
                                    _jsonHand = new JsonHandler(PostData);
                                    var jToken = _jsonHand.GetJToken("subreddit");
                                    subReddit.Id = _jsonHand.GetJTokenValue(jToken, "id");
                                    subReddit.Name = _jsonHand.GetJTokenValue(jToken, "name");
                                    subReddit.DisplayText = $"r/{subReddit.Name}";
                                    subReddit.Url = RdConstants.GetRedditHomePageAPI + "/" + subReddit.DisplayText;
                                    bool.TryParse(_jsonHand.GetJTokenValue(jToken, "nsfw"), out bool isNSFW);
                                    subReddit.IsNsfw = isNSFW;
                                    bool.TryParse(_jsonHand.GetJTokenValue(jToken, "quarantined"), out bool isQuarantined);
                                    subReddit.IsQuarantined = isQuarantined;
                                    if (LstSubReddit.Any(x => x.Id.Contains(subReddit.Id))) continue;
                                    LstSubReddit.Add(subReddit);
                                }
                            }
                            else
                            {
                                nodes = HtmlUtility.GetListOfNodesFromTagName(Response, "shreddit-subreddit-header", "router-name", "subreddit");
                                if (nodes != null)
                                    foreach (var node in nodes)
                                    {
                                        var subReddit = new SubRedditModel();
                                        subReddit.Name = node.GetAttributeValue("name", null);
                                        subReddit.Id = node.GetAttributeValue("subreddit-id", null);
                                        subReddit.DisplayText = node.GetAttributeValue("prefixed-name", null);
                                        subReddit.Url = RdConstants.GetRedditHomePageAPI + "/" + subReddit.DisplayText;
                                        subReddit.PublicDescription = node.GetAttributeValue("description", null);
                                        int.TryParse(node.GetAttributeValue("subscribers", null), out int subscribers);
                                        subReddit.Subscribers = subscribers;
                                        LstSubReddit.Add(subReddit);
                                    }
                            }
                        }
                        catch (Exception)
                        { }
                        PaginationParameter.LastPaginationId = Utilities.GetBetween(Response, "cursor=", "\"");
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
            var jsonHand = new JsonHandler(responseParameter.Response);
            PaginationParameter.SessionTracker = jsonHand.GetElementValue("user", "sessionTracker");
            PaginationParameter.Reddaid = jsonHand.GetElementValue("user", "reddaid");
            var loid = jsonHand.GetElementValue("user", "loid", "loid");
            var blob = jsonHand.GetElementValue("user", "loid", "blob");
            var created = jsonHand.GetElementValue("user", "loid", "loidCreated");
            var version = jsonHand.GetElementValue("user", "loid", "version");
            PaginationParameter.Loid = loid + "." + version + "." + created + "." + blob;
            PaginationParameter.AccessToken = jsonHand.GetElementValue("user", "session", "accessToken");
        }

        public void NewPageResponse(IEnumerable<JToken> jTokenModel, IEnumerable<JToken> jTokenAbout,
            string queryValue = "")
        {
            try
            {
                foreach (var data in jTokenModel)
                {
                    var subReddit = new SubRedditModel();

                    // Obtaining username and matched with user in SubRedditList in case of custom query
                    var userName = string.Empty;
                    if (queryValue.Contains($"{HomePage}/"))
                        userName = queryValue?.Replace($"{HomePage}/", "")?.TrimEnd('/');

                    subReddit.DisplayText = data.First["displayText"].ToString();

                    //if (userName != subReddit.DisplayText && string.IsNullOrEmpty(queryValue)) continue;

                    subReddit.WhitelistStatus = data.First["whitelistStatus"].ToString();
                    var num1 = Convert.ToBoolean(data.First["isNSFW"].ToString()) ? 1 : 0;
                    subReddit.IsNsfw = num1 != 0;
                    subReddit.Subscribers = (int)data.First["subscribers"];
                    subReddit.PrimaryColor = data.First["primaryColor"].ToString();
                    subReddit.Id = data.First["id"].ToString();
                    var num2 = Convert.ToBoolean(data.First["isQuarantined"].ToString()) ? 1 : 0;
                    subReddit.IsQuarantined = num2 != 0;
                    subReddit.Name = data.First["name"].ToString();
                    subReddit.Title = data.First["title"].ToString();
                    var url = data.First["url"].ToString();
                    if (!string.IsNullOrEmpty(url))
                        subReddit.Url = (HomePage + (url.Length > 0 && url?[0] == '/' ? url : "/" + url));
                    int.TryParse((string)data.First["wls"], out int wls);
                    subReddit.Wls = wls;
                    subReddit.Type = data.First["type"].ToString();
                    subReddit.CommunityIcon = data.First["icon"]["url"].ToString();
                    subReddit.PaginationParameter = PaginationParameter;
                    LstSubReddit.Add(subReddit);
                }

                var enumerable = jTokenAbout as JToken[] ?? jTokenAbout.ToArray();
                foreach (var data in enumerable)
                {
                    var i = enumerable.IndexOf(data);
                    var post = LstSubReddit.FirstOrDefault(x => x.Id == LstSubReddit[i].Id);
                    if (post == null) continue;
                    post.PublicDescription = data.First["publicDescription"]?.ToString()?.Replace("\n", " ")?.Replace(",", " ");
                    var num3 = string.IsNullOrEmpty(data.First["userIsSubscriber"].ToString()) ? 0 :
                        Convert.ToBoolean(data.First["userIsSubscriber"].ToString()) ? 1 : 0;
                    post.UserIsSubscriber = num3 != 0;
                    post.AccountsActive = data.First["accountsActive"].ToString();
                    var num4 = Convert.ToBoolean(data.First["showMedia"].ToString()) ? 1 : 0;
                    post.ShowMedia = num4 != 0;
                    var num5 = Convert.ToBoolean(data.First["emojisEnabled"].ToString()) ? 1 : 0;
                    post.EmojisEnabled = num5 != 0;
                    var num6 = Convert.ToBoolean(data.First["originalContentTagEnabled"].ToString()) ? 1 : 0;
                    post.OriginalContentTagEnabled = num6 != 0;
                    var num7 = Convert.ToBoolean(data.First["allOriginalContent"].ToString()) ? 1 : 0;
                    post.AllOriginalContent = num7 != 0;
                }

                PaginationParameter.LastPaginationId = LstSubReddit.Last().Id;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}