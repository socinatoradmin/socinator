using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using Newtonsoft.Json.Linq;
using RedditDominatorCore.RDLibrary;
using RedditDominatorCore.RDModel;
using RedditDominatorCore.RDUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace RedditDominatorCore.Response
{
    public class AutoActivityResponseHandler : RdResponseHandler
    {
        public List<RedditPost> LstRedditPost { get; set; } = new List<RedditPost>();
        private readonly JsonJArrayHandler _jsonHand;
        public bool HasMoreResult { get; set; }
        public string NextCursor { get; set; }
        public PaginationParameter PaginationParameter { get; }
        public AutoActivityResponseHandler(IResponseParameter response, bool IsPagination = false,
            PaginationParameter paginationParameter = null, bool IsOnlyScroll = false, bool IsBrowser = false, string BrowserPageResponse = "") : base(response)
        {
            try
            {
                _jsonHand = jsonHandler;
                if (IsOnlyScroll)
                {
                    Success = true;
                    return;
                }
                GetPagination(response);
                if (!IsPagination)
                {
                    PaginationParameter = new PaginationParameter();
                    var json = RdConstants.GetJsonPageResponse(RdConstants.GetDecodedResponse(response.Response, true, true));
                    var ListPost = _jsonHand.GetJTokenOfJToken(_jsonHand.GetJTokenOfJToken(_jsonHand.ParseJsonToJObject(json), "posts", "models"));
                    NewPageResponse(ListPost);
                    NextCursor = PaginationParameter.LastPaginationId;
                    HasMoreResult = !string.IsNullOrEmpty(NextCursor);
                }
                else
                {
                    PaginationParameter = paginationParameter;
                    var Jtoken = _jsonHand.GetJTokenOfJToken(_jsonHand.ParseJsonToJObject(response.Response), "data", "home", "elements");
                    NextCursor = _jsonHand.GetJTokenValue(Jtoken, "pageInfo", "endCursor");
                    bool.TryParse(_jsonHand.GetJTokenValue(Jtoken, "pageInfo", "hasNextPage"), out bool HasNext);
                    HasMoreResult = HasNext;
                    var ListPost = _jsonHand.GetJArrayElement(_jsonHand.GetJTokenValue(Jtoken, "edges"));
                    NewPageResponse(ListPost);
                }
            }
            catch
            {

            }
            finally { GetBrowserResponse(BrowserPageResponse); }
        }

        private void GetBrowserResponse(string browserPageResponse)
        {
            try
            {
                if (string.IsNullOrEmpty(browserPageResponse))
                    return;
                var Nodes = HtmlParseUtility.GetListInnerHtmlFromPartialTagNamecontains(browserPageResponse, "div", "data-testid", "post-container");
                if (Nodes != null && Nodes.Count > 0)
                {
                    foreach (var Node in Nodes)
                    {
                        var redditPost = new RedditPost();
                        var postNode = HtmlParseUtility.GetListInnerHtmlFromPartialTagNamecontains(Node, "div", "data-adclicklocation", "title");
                        redditPost.Permalink = HomePage + Utils.GetBetween(postNode?.FirstOrDefault(), "href=\"", "\">");
                        redditPost.PostId = "t3_" + Utils.GetBetween(Node, "/comments/", "/");
                        var subreddit = !string.IsNullOrEmpty(redditPost.Permalink) && redditPost.Permalink.Contains("/r") ? Utils.GetBetween(redditPost.Permalink, "/r/", "/comments") : string.Empty;
                        var username = !string.IsNullOrEmpty(redditPost.Permalink) && redditPost.Permalink.Contains("/user") ? Utils.GetBetween(redditPost.Permalink, "/user/", "/comments") : Utils.GetBetween(Utils.GetBetween(Node, "data-testid=\"post_author_link\" href=\"", "\""), "/user/", "/");
                        redditPost.Created = GetCreated(HtmlParseUtility.GetInnerTextFromTagName(Node, "span", "data-testid", "post_timestamp"));
                        redditPost.SubReddit = new SubRedditModel
                        {
                            Url = HomePage + $"/r/{subreddit}/",
                            Name = subreddit,
                        };
                        redditPost.User = new RedditUser
                        {
                            Username = username,
                            Url = HomePage + $"/user/{username}/"
                        };
                        LstRedditPost.Add(redditPost);
                    }
                }
            }
            catch { }
        }

        private long GetCreated(string posted)
        {
            var time = DateTime.Now;
            try
            {
                if (!string.IsNullOrEmpty(posted))
                {
                    int.TryParse(Regex.Match(posted, "\\d+").Value, out int TimeInt);
                    time = posted.Contains("hours") || posted.Contains("hour")
                        ? time.AddHours(-TimeInt) : posted.Contains("months") || posted.Contains("month")
                        ? time.AddMonths(-TimeInt) : posted.Contains("minutes") || posted.Contains("minute")
                        ? time.AddMinutes(-TimeInt) : posted.Contains("day") || posted.Contains("days")
                        ? time.AddDays(-TimeInt) : time;
                }
            }
            catch { }
            return time.GetCurrentEpochTimeMilliSeconds();
        }

        public void GetPagination(IResponseParameter responseParameter)
        {
            try
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
            catch { }
        }
        public void NewPageResponse(IEnumerable<JToken> jToken)
        {
            foreach (var data in jToken)
                try
                {
                    var redditPost = new RedditPost();
                    redditPost.Author = _jsonHand.GetJTokenValue(data.First, "author");
                    bool.TryParse(_jsonHand.GetJTokenValue(data.First, "isArchived"), out var isArchived);
                    redditPost.IsArchived = isArchived;
                    if (string.IsNullOrEmpty(redditPost.Author) || isArchived || redditPost.Author.Contains("deleted") || redditPost.Author.Contains("reddit"))
                        continue;
                    redditPost.Id = _jsonHand.GetJTokenValue(data.First, "id");
                    redditPost.Caption = _jsonHand.GetJTokenValue(data.First, "Caption");
                    redditPost.DomainOverride = _jsonHand.GetJTokenValue(data.First, "domainOverdataride");
                    redditPost.CallToAction = _jsonHand.GetJTokenValue(data.First, "callToAction");
                    bool.TryParse(_jsonHand.GetJTokenValue(data.First, "saved"), out var saved);
                    redditPost.Saved = saved;
                    int.TryParse(_jsonHand.GetJTokenValue(data.First, "numComments"), out int commentCount);
                    redditPost.NumComments = commentCount;
                    redditPost.UpvoteRatio = _jsonHand.GetJTokenValue(data.First, "upvoteRatio");
                    int.TryParse(_jsonHand.GetJTokenValue(data.First, "numCrossposts"), out int crosspostCount);
                    redditPost.NumCrossposts = crosspostCount;
                    bool.TryParse(_jsonHand.GetJTokenValue(data.First, "isSponsored"), out bool IsSponsored);
                    redditPost.IsSponsored = IsSponsored;
                    redditPost.ContentCategories = _jsonHand.GetJTokenValue(data.First, "contentCategories");
                    redditPost.Source = _jsonHand.GetJTokenValue(data.First, "source");
                    bool.TryParse(_jsonHand.GetJTokenValue(data.First, "isLocked"), out var isLocked);
                    redditPost.IsLocked = isLocked;
                    int.TryParse(_jsonHand.GetJTokenValue(data.First, "score"), out int score);
                    redditPost.Score = score;
                    bool.TryParse(_jsonHand.GetJTokenValue(data.First, "hidden"), out bool IsHidden);
                    redditPost.Hidden = IsHidden;
                    redditPost.Preview = _jsonHand.GetJTokenValue(data.First, "preview");
                    redditPost.CrosspostRootId = _jsonHand.GetJTokenValue(data.First, "crosspostRootId");
                    redditPost.CrosspostParentId = _jsonHand.GetJTokenValue(data.First, "crosspostParentId");
                    bool.TryParse(_jsonHand.GetJTokenValue(data.First, "sendReplies"), out bool IsEnabledSendReplies);
                    redditPost.SendReplies = IsEnabledSendReplies;
                    int.TryParse(_jsonHand.GetJTokenValue(data.First, "awardCountsById"), out int goldCount);
                    redditPost.GoldCount = goldCount;
                    bool.TryParse(_jsonHand.GetJTokenValue(data.First, "isSpoiler"), out bool IsSpoiler);
                    redditPost.IsSpoiler = IsSpoiler;
                    bool.TryParse(_jsonHand.GetJTokenValue(data.First, "isNSFW"), out bool isNsfw);
                    redditPost.IsNsfw = isNsfw;
                    bool.TryParse(_jsonHand.GetJTokenValue(data.First, "isMediaOnly"), out var isMediaOnly);
                    redditPost.IsMediaOnly = isMediaOnly;
                    redditPost.PostId = _jsonHand.GetJTokenValue(data.First, "postId");
                    redditPost.SuggestedSort = _jsonHand.GetJTokenValue(data.First, "suggestedSort");
                    bool.TryParse(_jsonHand.GetJTokenValue(data.First, "isBlank"), out bool isBlank);
                    redditPost.IsBlank = isBlank;
                    int.TryParse(_jsonHand.GetJTokenValue(data.First, "viewCount"), out int viewCount);
                    redditPost.ViewCount = viewCount;
                    redditPost.Permalink = _jsonHand.GetJTokenValue(data.First, "permalink");
                    long.TryParse(_jsonHand.GetJTokenValue(data.First, "created"), out long created);
                    redditPost.Created = created;
                    redditPost.Title = _jsonHand.GetJTokenValue(data.First, "title");
                    bool.TryParse(_jsonHand.GetJTokenValue(data.First, "isOriginalContent"), out var isOriginalContent);
                    redditPost.IsOriginalContent = isOriginalContent;
                    redditPost.DistinguishType = _jsonHand.GetJTokenValue(data.First, "distinguishType");
                    int.TryParse(_jsonHand.GetJTokenValue(data.First, "voteState"), out int voteState);
                    redditPost.VoteState = voteState;

                    var mediaType = _jsonHand.GetJTokenValue(data.First, "media");
                    redditPost.MediaType = string.IsNullOrEmpty(mediaType)
                        ? string.Empty
                        : _jsonHand.GetJTokenValue(data.First, "media", "type");
                    if (!string.IsNullOrEmpty(redditPost.MediaType) && redditPost.MediaType.Equals("video"))
                    {
                        int.TryParse(Utilities.GetBetween(mediaType, "height\":", ","), out var videoResolution);
                        if (videoResolution == 0)
                            int.TryParse(Utilities.GetBetween(mediaType, "\"height\": ", "\r\n"), out videoResolution);
                        if (videoResolution < 400)
                            redditPost.MediaResolution = 360.ToString();

                        if (string.IsNullOrEmpty(redditPost.MediaResolution))
                            redditPost.MediaResolution = videoResolution > 400 && videoResolution < 500
                                ? 480.ToString()
                                : 720.ToString();

                        redditPost.PostVideoUrl = _jsonHand.GetJTokenValue(data.First, "media", "dashUrl")
                            ?.Replace("/DASHPlaylist.mpd", $"/DASH_{redditPost.MediaResolution}");
                    }
                    bool.TryParse(_jsonHand.GetJTokenValue(data.First, "isCrosspostable"), out var isCrosspostable);
                    redditPost.IsCrosspostable = isCrosspostable;
                    bool.TryParse(_jsonHand.GetJTokenValue(data.First, "isStickied"), out var isStickied);
                    redditPost.IsStickied = isStickied;
                    redditPost.PaginationParameter = PaginationParameter;
                    redditPost.User = new RedditUser
                    {
                        Username = _jsonHand.GetJTokenValue(data.First, "author"),
                        Url = HomePage + $"/user/{_jsonHand.GetJTokenValue(data.First, "author")}/",
                        UserId = _jsonHand.GetJTokenValue(data.First, "authorId")
                    };
                    var communityName = _jsonHand.GetJTokenValue(data.First, "domain")?.Split('.')?.LastOrDefault(x => !string.IsNullOrEmpty(x));
                    redditPost.SubReddit = new SubRedditModel
                    {
                        Id = _jsonHand.GetJTokenValue(data.First, "belongsTo", "id"),
                        Name = communityName,
                        Url = HomePage + $"/r/{communityName}/"
                    };
                    LstRedditPost.Add(redditPost);
                }
                catch (Exception)
                {
                    // ignored
                }
            PaginationParameter.LastPaginationId = LstRedditPost.Count > 0 ? LstRedditPost.LastOrDefault().PostId : string.Empty;
        }
    }
}
