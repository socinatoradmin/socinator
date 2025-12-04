using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using Newtonsoft.Json.Linq;
using QuoraDominatorCore.Models;
using QuoraDominatorCore.QdUtility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QuoraDominatorCore.Response
{
    public class PostByKeywordResponseHandler : QuoraResponseHandler
    {
        public List<PostDetails> postDetailsList = new List<PostDetails>();
        public int PaginationCount = 0;
        public bool HasMoreResults = false;
        public PostByKeywordResponseHandler(IResponseParameter response,bool IsBrowser) : base(response)
        {
            try
            {
                var shareCount=0;var viewsCount=0;var UpvoteCount=0;var creationtime=0L;var followerCount=0;var isFollowing=false;var commentCount=0;
                var PostResponse = string.Empty;
                if (IsBrowser)
                    PostResponse = "{\"data\":{\"searchConnection\"" + Utilities.GetBetween(response.Response.Replace("\\\"","\"").Replace("\\\"", "\""), "{\"data\":{\"searchConnection\"", "{\"is_final\":true}}") + "{\"is_final\":true}}";
                else
                    PostResponse = response.Response;
                var jsonObject = jsonHandler.ParseJsonToJObject(PostResponse);
                if (jsonObject != null)
                {
                    var Posts = jsonHandler.GetJArrayElement(jsonHandler.GetJTokenValue(jsonObject, "data", "searchConnection", "edges"));
                    int.TryParse(jsonHandler.GetJTokenValue(jsonObject, "data", "searchConnection", "pageInfo", "endCursor"), out int endCursor);
                    bool.TryParse(jsonHandler.GetJTokenValue(jsonObject, "data", "searchConnection", "pageInfo", "hasNextPage"), out bool hasNextPage);
                    HasMoreResults = hasNextPage;
                    PaginationCount = endCursor;
                    if (Posts != null && Posts.HasValues)
                        Posts.ForEach(post =>
                        {
                            var posturl = jsonHandler.GetJTokenValue(post, "node", "post", "url");
                            posturl = posturl.Contains(".quora.com") ? posturl : $"{QdConstants.HomePageUrl}{posturl}";
                            int.TryParse(jsonHandler.GetJTokenValue(post, "node", "post", "numShares"), out shareCount);
                            int.TryParse(jsonHandler.GetJTokenValue(post, "node", "post", "numDisplayComments"), out commentCount);
                            int.TryParse(jsonHandler.GetJTokenValue(post, "node", "post", "numUpvotes"), out UpvoteCount);
                            int.TryParse(jsonHandler.GetJTokenValue(post, "node", "post", "numViews"), out viewsCount);
                            long.TryParse(jsonHandler.GetJTokenValue(post, "node", "post", "creationTime"), out creationtime);
                            int.TryParse(jsonHandler.GetJTokenValue(post, "node", "post", "author", "followerCount"), out followerCount);
                            bool.TryParse(jsonHandler.GetJTokenValue(post, "node", "post", "author", "viewerIsFollowing"), out isFollowing);
                            var datetime = (creationtime / 1000).EpochToDateTimeUtc();
                            postDetailsList.Add(new PostDetails
                            {
                                PostUrl = posturl,
                                ShareCount = shareCount,
                                UpvoteCount = UpvoteCount,
                                PostId = jsonHandler.GetJTokenValue(post, "node", "post", "pid"),
                                CommentCount = commentCount,
                                ViewsCount = viewsCount,
                                Created = datetime,
                                ViewerVoteType = jsonHandler.GetJTokenValue(post, "node", "post", "viewerVoteType"),
                                PostType = jsonHandler.GetJTokenValue(post, "node", "post", "__typename"),
                                PostAuthorProfileUrl = jsonHandler.GetJTokenValue(post, "node", "post", "author", "profileUrl").Split('/').LastOrDefault(),
                                PostAuthorFollowerCount = followerCount,
                                PostAuthorIsFollowing = isFollowing
                            });
                        });
                }
                else
                {
                     PostResponse = response.Response.Replace("\\\"", "\"").Replace("\\\"", "\"");
                    PostResponse = QdConstants.GetJsonForAllTypePosts(PostResponse, "tribeItem");
                    jsonObject = jsonHandler.ParseJsonToJObject(PostResponse);
                    if (jsonObject == null)
                    {
                        PostResponse = QdConstants.GetJsonForAllTypePosts(response.Response.Replace("\\\"", "\"").Replace("\\\"", "\""), "post");
                        jsonObject = jsonHandler.ParseJsonToJObject(PostResponse);
                        jsonObject = jsonHandler.GetJTokenOfJToken(jsonObject, "data") as JObject;
                    }
                    else
                       jsonObject = jsonHandler.GetJTokenOfJToken(jsonObject, "data", "tribeItem") as JObject;
                    var posturl = jsonHandler.GetJTokenValue(jsonObject, "post", "url");
                    if (string.IsNullOrEmpty(posturl)) return;
                    posturl = posturl.Contains(".quora.com") ? posturl : $"{QdConstants.HomePageUrl}{posturl}";
                    int.TryParse(jsonHandler.GetJTokenValue(jsonObject, "post", "numShares"), out shareCount);
                    int.TryParse(jsonHandler.GetJTokenValue(jsonObject, "post", "numDisplayComments"), out commentCount);
                    int.TryParse(jsonHandler.GetJTokenValue(jsonObject, "post", "numUpvotes"), out UpvoteCount);
                    int.TryParse(jsonHandler.GetJTokenValue(jsonObject, "post", "numViews"), out viewsCount);
                    long.TryParse(jsonHandler.GetJTokenValue(jsonObject, "post", "creationTime"), out creationtime);
                    int.TryParse(jsonHandler.GetJTokenValue(jsonObject, "post", "author", "followerCount"), out followerCount);
                    bool.TryParse(jsonHandler.GetJTokenValue(jsonObject, "post", "author", "viewerIsFollowing"), out isFollowing);
                    var datetime = (creationtime / 1000).EpochToDateTimeUtc();
                    postDetailsList.Add(new PostDetails
                    {
                        PostUrl = posturl,
                        ShareCount = shareCount,
                        UpvoteCount = UpvoteCount,
                        PostId = jsonHandler.GetJTokenValue(jsonObject, "post", "pid"),
                        CommentCount = commentCount,
                        ViewsCount = viewsCount,
                        Created = datetime,
                        ViewerVoteType = jsonHandler.GetJTokenValue(jsonObject, "post", "viewerVoteType"),
                        PostType = jsonHandler.GetJTokenValue(jsonObject, "post", "__typename"),
                        PostAuthorProfileUrl = jsonHandler.GetJTokenValue(jsonObject, "post", "author", "profileUrl").Split('/').LastOrDefault(),
                        PostAuthorFollowerCount = followerCount,
                        PostAuthorIsFollowing = isFollowing
                    });
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
