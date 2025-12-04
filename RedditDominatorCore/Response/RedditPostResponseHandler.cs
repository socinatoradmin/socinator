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
using System.Net;

namespace RedditDominatorCore.Response
{
    public class RedditPostResponseHandler : RdResponseHandler
    {
        public readonly List<RedditPost> LstRedditPost = new List<RedditPost>();
        private readonly JsonHandler _jsonHand;

        public RedditPostResponseHandler(IResponseParameter response, bool isPaginatedData,
            PaginationParameter paginationParameter=null) : base(response)
        {
            try
            {
                {
                    var notRequiredData = string.Empty;
                    Response = response.Response;
                    if (!Success) return;

                    if (isPaginatedData && false)
                    {
                        PaginationParameter = paginationParameter;
                        _jsonHand = new JsonHandler(response.Response);
                        var jToken = _jsonHand.GetJToken("posts").Children();
                        NewPageResponse(jToken);
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
                            var id = _jsonHand.GetJToken("posts", "models").ToString().Replace("{", "").Replace("}", "").Trim();
                            var node = Utilities.GetBetween(id, "\"", "\": true");
                            var jToken = _jsonHand.GetJToken("posts", "models").Children();
                            GetPagination(response);
                            NewPageResponse(jToken);
                        }
                        else
                        {
                            try
                            {
                                var nodes = HtmlUtility.GetListOfNodesFromTagName(Response, "faceplate-tracker", "data-testid", "search-post");
                                if(nodes== null)
                                    nodes = HtmlUtility.GetListOfNodesFromTagName(Response, "search-telemetry-tracker", "consume-events", "post/consume/post");

                                if (nodes != null)
                                {
                                    foreach (var node in nodes)
                                    {
                                        string PostData = node.GetAttributeValue("data-faceplate-tracking-context", null);
                                        PostData=PostData.Replace("&quot;", "\"");
                                        _jsonHand = new JsonHandler(PostData);
                                        var jToken=_jsonHand.GetJToken("post");
                                        var jTokenSubreddit=_jsonHand.GetJToken("subreddit");

                                        var anchorNode = node.SelectSingleNode(".//a[@data-testid='post-title']");
                                        string permalink = null;
                                        if (anchorNode != null)
                                        {
                                            permalink = anchorNode.GetAttributeValue("href", string.Empty);
                                        }

                                        NewUIPageResponse(jToken, permalink, jTokenSubreddit);
                                    }
                                }
                                else
                                {
                                    nodes = HtmlUtility.GetListOfNodesFromTagName(Response, "shreddit-post", "view-context", "CommentsPage");
                                    if (nodes == null)
                                        nodes = HtmlUtility.GetListOfNodesFromTagName(Response, "shreddit-post", "view-type", "cardView");
                                    if (nodes != null)
                                    foreach (var node in nodes)
                                    {
                                        var redditPost = new RedditPost();
                                        redditPost.TypeName= node.GetAttributeValue("name", string.Empty);
                                        redditPost.Id = node.GetAttributeValue("id", null);
                                        redditPost.Title = node.GetAttributeValue("post-title", string.Empty);
                                        int.TryParse(node.GetAttributeValue("comment-count", string.Empty),out  int commentCount);
                                        redditPost.NumComments = commentCount;
                                        int.TryParse(node.GetAttributeValue("score", string.Empty), out int score);
                                        redditPost.Score = score;
                                        redditPost.Author = node.GetAttributeValue("author", string.Empty);
                                        redditPost.MediaType=node.GetAttributeValue("post-type", string.Empty);
                                        redditPost.Upvoted=node.GetAttributeValue("vote-type", string.Empty).Contains("upvote");
                                        redditPost.Downvoted=node.GetAttributeValue("vote-type", string.Empty).Contains("downvote");
                                        redditPost.Permalink = $"{RdConstants.GetRedditHomePageAPI}{node.GetAttributeValue("permalink", string.Empty)}";
                                        LstRedditPost.Add(redditPost);
                                    }
                                }
                            }
                            catch (Exception)
                            {
                            }
                            PaginationParameter.LastPaginationId = Utilities.GetBetween(Response, "postsCursor=", "\"");
                            if(PaginationParameter != null && string.IsNullOrEmpty(PaginationParameter.LastPaginationId))
                                PaginationParameter.LastPaginationId = Utilities.GetBetween(Response, "cursor=", "\"");
                            
                            if (PaginationParameter != null && string.IsNullOrEmpty(PaginationParameter.LastPaginationId))
                            {
                                var paginationRes = HtmlUtility.GetListOfNodesFromTagName(Response, "faceplate-partial", "slot", "load-after");
                                if (paginationRes != null)
                                {
                                    foreach (var pagination in paginationRes)
                                    {
                                        var Id = pagination.GetAttributeValue("src", string.Empty);
                                        if (!string.IsNullOrEmpty(Id))
                                            PaginationParameter.LastPaginationId = Id;
                                    }
                                }
                            }
                            if (PaginationParameter != null && string.IsNullOrEmpty(PaginationParameter.LastPaginationId))
                                PaginationParameter.LastPaginationId = Utilities.GetBetween(Response, "posts-cursor=", "\"");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void NewUIPageResponse(JToken jToken , string permalink= "", JToken subredditToken=null)
        {
            try
            {
                var redditPost = new RedditPost();
                redditPost.Author = _jsonHand.GetJTokenValue(jToken, "author_id");
                if (string.IsNullOrEmpty(redditPost.Author))
                    redditPost.Author = _jsonHand.GetJTokenValue(subredditToken, "id");
                bool.TryParse(_jsonHand.GetJTokenValue(jToken, "archived"), out var isArchived);
                redditPost.IsArchived = isArchived;
                redditPost.Id = _jsonHand.GetJTokenValue(jToken, "id");
                if (string.IsNullOrEmpty(redditPost.Author) && string.IsNullOrEmpty(redditPost.Id) || isArchived || redditPost.Author.Contains("deleted") || redditPost.Author.Contains("reddit"))
                    return;
                redditPost.Caption = _jsonHand.GetJTokenValue(jToken, "Caption");
                redditPost.DomainOverride = _jsonHand.GetJTokenValue(jToken, "domain");
                redditPost.CallToAction = _jsonHand.GetJTokenValue(jToken, "callToAction");
                bool.TryParse(_jsonHand.GetJTokenValue(jToken, "saved"), out var saved);
                redditPost.Saved = saved;
                int.TryParse(_jsonHand.GetJTokenValue(jToken, "number_comments"), out int commentCount);
                redditPost.NumComments = commentCount;
                redditPost.UpvoteRatio = _jsonHand.GetJTokenValue(jToken, "score");
                bool.TryParse(_jsonHand.GetJTokenValue(jToken, "promoted"), out bool IsSponsored);
                redditPost.IsSponsored = IsSponsored;
                redditPost.Source = _jsonHand.GetJTokenValue(jToken, "url");
                int.TryParse(_jsonHand.GetJTokenValue(jToken, "score"), out int score);
                redditPost.Score = score;
                bool.TryParse(_jsonHand.GetJTokenValue(jToken, "hidden"), out bool IsHidden);
                redditPost.Hidden = IsHidden;
                redditPost.Preview = _jsonHand.GetJTokenValue(jToken, "preview");
                redditPost.CrosspostRootId = _jsonHand.GetJTokenValue(jToken, "crosspostRootId");
                redditPost.CrosspostParentId = _jsonHand.GetJTokenValue(jToken, "crosspostParentId");
                bool.TryParse(_jsonHand.GetJTokenValue(jToken, "sendReplies"), out bool IsEnabledSendReplies);
                redditPost.SendReplies = IsEnabledSendReplies;
                bool.TryParse(_jsonHand.GetJTokenValue(jToken, "spoiler"), out bool IsSpoiler);
                redditPost.IsSpoiler = IsSpoiler;
                bool.TryParse(_jsonHand.GetJTokenValue(jToken, "nsfw"), out bool isNsfw);
                redditPost.IsNsfw = isNsfw;
                int.TryParse(_jsonHand.GetJTokenValue(jToken, "viewCount"), out int viewCount);
                redditPost.ViewCount = viewCount;
                redditPost.Permalink = !string.IsNullOrEmpty(permalink)? $"{RdConstants.GetRedditHomePageAPI}{permalink}":"";
                if(string.IsNullOrEmpty(redditPost.Permalink))
                    redditPost.Permalink = $"{RdConstants.GetRedditHomePageAPI}{_jsonHand.GetJTokenValue(jToken, "url")}";
                long.TryParse(_jsonHand.GetJTokenValue(jToken, "created_timestamp"), out long created);
                redditPost.Created = created;
                redditPost.Title = _jsonHand.GetJTokenValue(jToken, "title");
                bool.TryParse(_jsonHand.GetJTokenValue(jToken, "isOriginalContent"), out var isOriginalContent);
                redditPost.IsOriginalContent = isOriginalContent;
                redditPost.DistinguishType = _jsonHand.GetJTokenValue(jToken, "distinguishType");
                int.TryParse(_jsonHand.GetJTokenValue(jToken, "voteState"), out int voteState);
                redditPost.VoteState = voteState;
                redditPost.FlairText = _jsonHand.GetJTokenValue(jToken, "flair", 0, "text");
                if (string.IsNullOrEmpty(redditPost.FlairText))
                    redditPost.FlairText = _jsonHand.GetJTokenValue(jToken, "flair", 0, "richtext", 0, "t");
                redditPost.MediaType = _jsonHand.GetJTokenValue(jToken, "type");
                if (!string.IsNullOrEmpty(redditPost.MediaType) && redditPost.MediaType.Equals("video"))
                {
                    redditPost.MediaResolution = 360.ToString();
                    //redditPost.PostVideoUrl = _jsonHand.GetJTokenValue(jToken, "media", "dashUrl")
                    //    ?.Replace("/DASHPlaylist.mpd", $"/DASH_{redditPost.MediaResolution}");
                }
                redditPost.PaginationParameter = PaginationParameter;
                LstRedditPost.Add(redditPost);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public PaginationParameter PaginationParameter { get; }

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
                    redditPost.FlairText = _jsonHand.GetJTokenValue(data.First, "flair", 0, "text");
                    if (string.IsNullOrEmpty(redditPost.FlairText))
                        redditPost.FlairText = _jsonHand.GetJTokenValue(data.First, "flair", 0, "richtext", 0, "t");
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