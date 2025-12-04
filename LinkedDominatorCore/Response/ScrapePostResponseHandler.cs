using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using LinkedDominatorCore.LDModel;
using LinkedDominatorCore.LDUtility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LinkedDominatorCore.Response
{
    public class ScrapePostResponseHandler:LdResponseHandler
    {
        public List<LinkedinPost> PostCollection { get; set; } = new List<LinkedinPost>();
        public int PaginationCount {  get; set; }
        public bool HasMoreResults {  get; set; }
        public ScrapePostResponseHandler(IResponseParameter response,bool IsBrowser=false):base(response) {

            try
            {
                if (!IsBrowser && !string.IsNullOrEmpty(response.Response))
                {
                    var JObject = handler.ParseJsonToJObject(response.Response);
                    var PostInfo = handler.GetJTokenOfJToken(JObject, "data", "searchDashClustersByAll");
                    var PaginationInfo = handler.GetJTokenOfJToken(PostInfo, "paging");
                    int.TryParse(handler.GetJTokenValue(PaginationInfo, "count"), out int paginationCount);
                    PaginationCount= paginationCount;
                    HasMoreResults = PaginationCount > 0;
                    var elements = handler.GetJArrayElement(handler.GetJTokenValue(PostInfo, "elements"));
                    if(elements != null && elements.HasValues)
                    {
                        var Posts = Utils.GetArrayToken(response.Response,false,handler).Item1;
                        if (Posts != null && Posts.HasValues)
                            Posts.ForEach(postToken =>
                            {
                                var postdetails = handler.GetJTokenOfJToken(postToken, "item", "entityResult");
                                var postId = handler.GetJTokenValue(postdetails, "trackingUrn")?.Trim()?.Split(':')?.LastOrDefault();
                                var SocialDetails = handler.GetJTokenOfJToken(postdetails, "insightsResolutionResults",0, "socialActivityCountsInsight");
                                var ProfileDetails = handler.GetJTokenOfJToken(postdetails, "image", "attributes",0, "detailData", "nonEntityProfilePicture");
                                var PostMediaDetails = handler.GetJTokenOfJToken(postdetails, "entityEmbeddedObject", "image", "attributes",0, "detailData", "vectorImage");
                                if (!PostCollection.Any(x => x.Id == postId))
                                {
                                    try
                                    {
                                        PostCollection.Add(new LinkedinPost
                                        {
                                            Id = postId,
                                            PostTitle = handler.GetJTokenValue(postdetails, "summary", "text"),
                                            CommentCount = handler.GetJTokenValue(SocialDetails, "numComments"),
                                            LikeCount = handler.GetJTokenValue(SocialDetails, "numLikes"),
                                            IsLiked = handler.GetJTokenValue(SocialDetails, "liked"),
                                            FullName = handler.GetJTokenValue(postdetails, "title", "text"),
                                            HeadlineTitle = handler.GetJTokenValue(postdetails, "primarySubtitle", "text"),
                                            TrackingId = handler.GetJTokenValue(postdetails, "trackingId"),
                                            ProfileUrl = handler.GetJTokenValue(postdetails, "actorNavigationUrl"),
                                            PostLink = handler.GetJTokenValue(postdetails, "navigationUrl"),
                                            MemberId = handler.GetJTokenValue(postdetails, "actorTrackingUrn")?.Trim()?.Split(':')?.LastOrDefault(),
                                            ProfileId = handler.GetJTokenValue(ProfileDetails, "profile", "entityUrn")?.Trim()?.Split(':')?.LastOrDefault(),
                                            ProfilePicUrl = handler.GetJTokenValue(ProfileDetails, "vectorImage", "artifacts", 0, "fileIdentifyingUrlPathSegment"),
                                            PostImageUrl = handler.GetJTokenValue(PostMediaDetails, "rootUrl") + handler.GetJTokenValue(PostMediaDetails, "artifacts", 0, "fileIdentifyingUrlPathSegment")
                                        });
                                    }
                                    catch (Exception) { }
                                }
                            });
                    }
                }
            }catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}
