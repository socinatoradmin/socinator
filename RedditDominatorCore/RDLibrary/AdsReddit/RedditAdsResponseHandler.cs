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
    public class RedditAdsResponseHandler : RdResponseHandler
    {
        public readonly List<RedditPost> LstRedditPost = new List<RedditPost>();

        public RedditAdsResponseHandler(IResponseParameter response, bool isPaginatedData,
            PaginationParameter paginationParameter, bool IsSubReddit = false) : base(response)
        {
            try
            {
                Response = response.Response;
                if (!Success) return;
                if (IsSubReddit)
                {
                    if (isPaginatedData)
                    {
                        PaginationParameter = paginationParameter;
                        var jsonObject = jsonHandler.ParseJsonToJObject(response.Response);
                        var Elements = jsonHandler.GetJArrayElement(jsonHandler.GetJTokenValue(jsonObject, "data", "subredditInfoByName", "elements", "edges"));
                        PaginationParameter.LastPaginationId = jsonHandler.GetJTokenValue(jsonObject, "data", "subredditInfoByName", "elements", "pageInfo", "endCursor");
                        bool.TryParse(jsonHandler.GetJTokenValue(jsonObject, "data", "subredditInfoByName", "elements", "pageInfo", "endCursor"), out bool hasNextPage);
                        PaginationParameter.HasNextPage = hasNextPage;
                        GetSubRedditDetails(Elements, isPaginatedData);
                    }
                    else
                    {
                        PaginationParameter = new PaginationParameter();
                        GetPagination(response);
                        var jsonResponse = RdConstants.GetJsonPageResponse(response.Response);
                        var jsonObject = jsonHandler.ParseJsonToJObject(jsonResponse);
                        var Elements = jsonObject["posts"]["models"].Children();
                        GetSubRedditDetails(Elements);
                    }
                }
                else
                {
                    if (isPaginatedData)
                    {
                        PaginationParameter = paginationParameter;
                        var jsonData = response.Response;
                        var jsonObject = jsonHandler.ParseJsonToJObject(jsonData);
                        var jSonData = jsonObject["data"]["home"]["posts"]["edges"].Children();
                        PageResponseForNextPage(jSonData, jsonHandler);
                    }
                    else
                    {
                        PaginationParameter = new PaginationParameter();
                        var jsonData = RdConstants.GetJsonPageResponse(response.Response);
                        var jsonObject = jsonHandler.ParseJsonToJObject(jsonData);
                        var jSonData = jsonObject["posts"]["models"].Children();
                        GetPagination(response);
                        NewPageResponse(jSonData, jsonHandler);
                    }
                }

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void GetSubRedditDetails(IEnumerable<JToken> elements, bool IsPagination = false)
        {
            if (IsPagination)
                PageResponseForNextPage(elements, jsonHandler);
            else
                NewPageResponse(elements, jsonHandler);
        }

        public PaginationParameter PaginationParameter { get; }

        public void GetPagination(IResponseParameter responseParameter)
        {
            var jsonReponse = RdConstants.GetJsonPageResponse(responseParameter.Response);
            var handler = jsonHandler;
            var jsonObject = handler.ParseJsonToJObject(jsonReponse);
            PaginationParameter.SessionTracker = handler.GetJTokenValue(jsonObject, "user", "sessionTracker");
            PaginationParameter.Reddaid = handler.GetJTokenValue(jsonObject, "user", "reddaid");
            var loidLoid = handler.GetJTokenValue(jsonObject, "user", "loid", "loid");
            var loidBlob = handler.GetJTokenValue(jsonObject, "user", "loid", "blob");
            var loidCreated = handler.GetJTokenValue(jsonObject, "user", "loid", "loidCreated");
            var loidVersion = handler.GetJTokenValue(jsonObject, "user", "loid", "version");
            PaginationParameter.Loid = loidLoid + "." + loidVersion + "." + loidCreated + "." + loidBlob;
            PaginationParameter.AccessToken = handler.GetJTokenValue(jsonObject, "user", "session", "accessToken");
        }

        public void NewPageResponse(IEnumerable<JToken> responseJToken, JsonJArrayHandler handler)
        {
            foreach (var token in responseJToken)
                try
                {
                    var data = token.First;
                    var redditPost = new RedditPost();
                    bool.TryParse(handler.GetJTokenValue(data, "isCrosspostable"), out bool num1);
                    redditPost.IsCrosspostable = num1;
                    bool.TryParse(handler.GetJTokenValue(data, "isStickied"), out bool num2);
                    redditPost.IsStickied = num2;
                    redditPost.CallToAction = handler.GetJTokenValue(data, "callToAction");
                    bool.TryParse(handler.GetJTokenValue(data, "saved"), out bool num3);
                    redditPost.Saved = num3;
                    int.TryParse(handler.GetJTokenValue(data, "numComments"), out int num4);
                    redditPost.NumComments = num4;
                    redditPost.UpvoteRatio = handler.GetJTokenValue(data, "upvoteRatio");
                    redditPost.Author = handler.GetJTokenValue(data, "author");
                    int.TryParse(handler.GetJTokenValue(data, "numCrossposts"), out int crossPost);
                    redditPost.NumCrossposts = crossPost;
                    redditPost.Id = handler.GetJTokenValue(data, "id");
                    bool.TryParse(handler.GetJTokenValue(data, "isSponsored"), out bool num5);
                    redditPost.IsSponsored = num5;
                    redditPost.Source = handler.GetJTokenValue(data, "source");
                    bool.TryParse(handler.GetJTokenValue(data, "isLocked"), out bool num6);
                    redditPost.IsLocked = num6;
                    int.TryParse(handler.GetJTokenValue(data, "score"), out int score);
                    redditPost.Score = score;
                    bool.TryParse(handler.GetJTokenValue(data, "isArchived"), out bool num7);
                    redditPost.IsArchived = num7;
                    bool.TryParse(handler.GetJTokenValue(data, "hidden"), out bool num8);
                    redditPost.Hidden = num8;
                    redditPost.Preview = handler.GetJTokenValue(data, "preview");
                    redditPost.CrosspostRootId = handler.GetJTokenValue(data, "crosspostRootId");
                    bool.TryParse(handler.GetJTokenValue(data, "sendReplies"), out bool num10);
                    redditPost.SendReplies = num10;
                    bool.TryParse(handler.GetJTokenValue(data, "isSpoiler"), out bool num11);
                    redditPost.IsSpoiler = num11;
                    bool.TryParse(handler.GetJTokenValue(data, "isNSFW"), out bool num12);
                    redditPost.IsNsfw = num12;
                    bool.TryParse(handler.GetJTokenValue(data, "isMediaOnly"), out bool num13);
                    redditPost.IsMediaOnly = num13;
                    redditPost.PostId = handler.GetJTokenValue(data, "postId");
                    redditPost.SuggestedSort = handler.GetJTokenValue(data, "suggestedSort");
                    bool.TryParse(handler.GetJTokenValue(data, "isBlank"), out bool num14);
                    redditPost.IsBlank = num14;
                    int.TryParse(handler.GetJTokenValue(data, "viewCount"), out int viewcount);
                    redditPost.ViewCount = viewcount;
                    redditPost.Permalink = handler.GetJTokenValue(data, "permalink");
                    long.TryParse(handler.GetJTokenValue(data, "created"), out long created);
                    redditPost.Created = created;
                    redditPost.Title = handler.GetJTokenValue(data, "title");
                    bool.TryParse(handler.GetJTokenValue(data, "isOriginalContent"), out bool num15);
                    redditPost.IsOriginalContent = num15;
                    redditPost.DistinguishType = handler.GetJTokenValue(data, "distinguishType");
                    int.TryParse(handler.GetJTokenValue(data, "voteState"), out int voteState);
                    redditPost.VoteState = voteState;
                    var mediaType = handler.GetJTokenValue(data, "media");
                    redditPost.MediaType = string.IsNullOrEmpty(mediaType)
                        ? string.Empty
                        : handler.GetJTokenValue(data, "media", "type");

                    if (string.Equals(redditPost.MediaType, "video", StringComparison.OrdinalIgnoreCase))
                    {
                        int.TryParse(handler.GetJTokenValue(data, "media", "height"), out int videoResolution);
                        redditPost.MediaResolution = videoResolution < 500 ? 480.ToString() : 720.ToString();
                        redditPost.VideoUrl = handler.GetJTokenValue(data, "media", "scrubberThumbSource");
                        redditPost.VideoUrl = redditPost.VideoUrl.Replace("DASH_96", $"DASH_360").Replace("DASH_64", $"DASH_360");
                    }
                    if (string.IsNullOrEmpty(redditPost.MediaType) ? false : (redditPost.MediaType.Contains("embed") || redditPost.MediaType.Contains("gallery")) && string.IsNullOrEmpty(redditPost.VideoUrl))
                        redditPost.MediaType = "image";
                    else if (!string.IsNullOrEmpty(redditPost.VideoUrl))
                        redditPost.MediaType = "video";
                    redditPost.PaginationParameter = PaginationParameter;
                    LstRedditPost.Add(redditPost);
                }
                catch (Exception)
                {
                    // ignored
                }

            PaginationParameter.LastPaginationId = LstRedditPost.Count > 0 ? LstRedditPost.LastOrDefault().Id : string.Empty;
            LstRedditPost.RemoveAll(y => !y.IsSponsored || y.Id.Length > 10 || string.IsNullOrEmpty(y.MediaType));
        }

        public void PageResponseForNextPage(IEnumerable<JToken> responseJToken, JsonJArrayHandler handler)
        {
            foreach (var token in responseJToken)
                try
                {
                    var redditPost = new RedditPost();
                    var data = token;
                    redditPost.TypeName = handler.GetJTokenValue(data, "node", "__typename");
                    redditPost.MediaType = handler.GetJTokenValue(data, "node", "media", "typeHint")?.ToLower();
                    int.TryParse(handler.GetJTokenValue(data, "node", "crosspostCount"), out int crosspost);
                    redditPost.NumCrossposts = crosspost;
                    redditPost.IsSponsored = true;
                    redditPost.PostId = handler.GetJTokenValue(data, "node", "id");
                    redditPost.Permalink = "https://www.reddit.com" + handler.GetJTokenValue(data, "node", "permalink");
                    redditPost.Title = handler.GetJTokenValue(data, "node", "title");
                    redditPost.Author = handler.GetJTokenValue(data, "node", "authorInfo", "name");
                    redditPost.Preview = handler.GetJTokenValue(data, "node", "media", "still", "source", "url");
                    redditPost.Preview = string.IsNullOrEmpty(redditPost.Preview) ? handler.GetJTokenValue(data, "node", "thumbnail", "url") : redditPost.Preview;
                    redditPost.Source = handler.GetJTokenValue(data, "node", "url");
                    int.TryParse(handler.GetJTokenValue(data, "node", "score"), out int score);
                    redditPost.Score = score;
                    redditPost.CallToAction = handler.GetJTokenValue(data, "node", "callToAction");
                    var adDateTime = handler.GetJTokenValue(data, "node", "createdAt");
                    redditPost.Created = DateTime.Parse(adDateTime).GetCurrentEpochTimeMilliSeconds();
                    int.TryParse(handler.GetJTokenValue(data, "node", "commentCount"), out int comment);
                    redditPost.NumComments = comment;
                    redditPost.DominForAd = handler.GetJTokenValue(data, "node", "domain");
                    var streaming = handler.GetJTokenValue(data, "node", "media", "streaming");
                    if (!string.IsNullOrEmpty(streaming))
                    {
                        int.TryParse(handler.GetJTokenValue(data, "node", "media", "streaming", "dimensions", "height"), out int videoResolution);
                        redditPost.MediaResolution = (videoResolution < 500 ? 480 : 720).ToString();
                    }
                    if (string.IsNullOrEmpty(redditPost.MediaType) ? false : (redditPost.MediaType.Contains("embed") || redditPost.MediaType.Contains("gallery")) && string.IsNullOrEmpty(redditPost.VideoUrl))
                        redditPost.MediaType = "image";
                    else if (!string.IsNullOrEmpty(redditPost.VideoUrl))
                        redditPost.MediaType = "video";
                    if (string.IsNullOrEmpty(redditPost.MediaType) ? false : redditPost.MediaType.ToLower().Contains("video"))
                        redditPost.VideoUrl = handler.GetJTokenValue(data, "node", "media", "streaming", "scrubberMediaUrl");
                    LstRedditPost.Add(redditPost);
                }
                catch (Exception)
                {
                    // ignored
                }

            PaginationParameter.LastPaginationId = LstRedditPost.Count > 0 ? LstRedditPost.LastOrDefault().PostId : string.Empty;
            LstRedditPost.RemoveAll(x => string.IsNullOrEmpty(x.TypeName) || string.IsNullOrEmpty(x.MediaType) || !x.TypeName.Contains("AdPost"));
        }
    }
}