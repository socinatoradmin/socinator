using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using QuoraDominatorCore.Models;
using QuoraDominatorCore.QdUtility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QuoraDominatorCore.Response
{
    public class CustomUserPostResponseHandler: QuoraResponseHandler
    {
        public List<PostDetails> postDetailsList = new List<PostDetails>();
        public int PaginationCount = 0;
        public bool HasMoreResults = false;
        public CustomUserPostResponseHandler(IResponseParameter response, bool IsBrowser) : base(response)
        {
            try
            {
                if(IsBrowser)
                    response.Response = QdConstants.GetJsonForAllTypePosts(response.Response.Replace("\\\"", "\"").Replace("\\\"", "\""), "user");
                var jsonObject = jsonHandler.ParseJsonToJObject(response.Response);
                if (jsonObject != null)
                {
                    var Posts = jsonHandler.GetJArrayElement(jsonHandler.GetJTokenValue(jsonObject, "data", "user", "postsConnection", "edges"));
                    int.TryParse(jsonHandler.GetJTokenValue(jsonObject, "data", "user", "postsConnection", "pageInfo", "endCursor"), out int endCursor);
                    bool.TryParse(jsonHandler.GetJTokenValue(jsonObject, "data", "user", "postsConnection", "pageInfo", "hasNextPage"), out bool hasNextPage);
                    HasMoreResults = hasNextPage;
                    PaginationCount = endCursor;
                    if (Posts != null && Posts.HasValues)
                        Posts.ForEach(post =>
                        {
                            var posturl = jsonHandler.GetJTokenValue(post, "node", "url");
                            posturl = posturl.Contains(".quora.com") ? posturl : $"{QdConstants.HomePageUrl}{posturl}";
                            int.TryParse(jsonHandler.GetJTokenValue(post, "node", "numShares"), out int shareCount);
                            int.TryParse(jsonHandler.GetJTokenValue(post, "node", "numDisplayComments"), out int commentCount);
                            int.TryParse(jsonHandler.GetJTokenValue(post, "node", "numUpvotes"), out int UpvoteCount);
                            int.TryParse(jsonHandler.GetJTokenValue(post, "node", "numViews"), out int viewsCount);
                            long.TryParse(jsonHandler.GetJTokenValue(post, "node", "creationTime"), out long creationtime);
                            int.TryParse(jsonHandler.GetJTokenValue(post, "node", "author", "followerCount"), out int followerCount);
                            bool.TryParse(jsonHandler.GetJTokenValue(post, "node", "author", "viewerIsFollowing"), out bool isFollowing);
                            var datetime = (creationtime / 1000).EpochToDateTimeUtc();
                            postDetailsList.Add(new PostDetails
                            {
                                PostUrl = posturl,
                                ShareCount = shareCount,
                                UpvoteCount = UpvoteCount,
                                PostId = jsonHandler.GetJTokenValue(post, "node", "pid"),
                                CommentCount = commentCount,
                                ViewsCount = viewsCount,
                                Created = datetime,
                                ViewerVoteType = jsonHandler.GetJTokenValue(post, "node", "viewerVoteType"),
                                PostType = jsonHandler.GetJTokenValue(post, "node", "__typename"),
                                PostAuthorProfileUrl = jsonHandler.GetJTokenValue(post, "node", "author", "profileUrl").Split('/').LastOrDefault(),
                                PostAuthorFollowerCount = followerCount,
                                PostAuthorIsFollowing = isFollowing
                            });
                        });
                }
                
            }
            catch (Exception)
            {
            }
        }
    }
}
