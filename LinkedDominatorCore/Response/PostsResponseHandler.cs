using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using DominatorHouseCore.DatabaseHandler.LdTables.Account;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using HtmlAgilityPack;
using LinkedDominatorCore.LDModel;
using LinkedDominatorCore.LDUtility;
using LinkedDominatorCore.Utility;
using Newtonsoft.Json.Linq;

namespace LinkedDominatorCore.Response
{
    public class PostsResponseHandler : LdResponseHandler
    {
        public PostsResponseHandler(IResponseParameter response, ActivityType activityType,
            string currentAccountProfileId, List<string> lstCommentInDom) : base(response)
        {
            Success = !string.IsNullOrEmpty(response?.Response) && (response.Response.Contains("\"type\":\"feed\"") ||response.Response.Contains("com.linkedin.voyager.dash.deco.search.SearchClusterViewModel") || response.Response.Contains("<!DOCTYPE html>"));
            if (!Success)
                return;
            try
            {
                if (response.Response.Contains("<!DOCTYPE html>"))
                {
                    BrowserProcessResponse(activityType);
                    return;
                }

                //scrap post only for keyword query type response
                if (string.IsNullOrEmpty(currentAccountProfileId))
                {
                    ScrapePostForKeyword(response.Response);
                    return;
                }

                var arr = JsonJArrayHandler.GetJArrayElement(JsonJArrayHandler.GetJTokenValue(RespJ,"elements"));
                if (response.Response.Contains(",\"elements\":[]") &&
                    response.Response.Contains("\"metadata\":{\"paginationToken\":\"\",\"newRelevanceFeed\":false,"))
                {
                    Success = false;
                    return;
                }

                PaginationToken = JsonJArrayHandler.GetJTokenValue(RespJ,"metadata","paginationToken");

                #region Get PostsList

                foreach (var item in arr)
                    try
                    {
                        #region IsLiked From Current Account

                        //actor
                        var isLiked = JsonJArrayHandler.GetJTokenValue(item, "socialDetail",
                            "totalSocialActivityCounts", "liked");

                        if (isLiked == "True" && activityType == ActivityType.Like)
                            continue;

                        #endregion

                        #region IsCommented same comment From Current Account

                        if (activityType == ActivityType.Comment)
                            try
                            {
                                currentAccountProfileId = Regex.Split(currentAccountProfileId, "in/")[1].Trim('/');
                                var commentContentArray = JsonJArrayHandler.GetJArrayElement(
                                    JsonJArrayHandler.GetJTokenValue(item,"socialDetail","comments","elements"));

                                var isAlreadyCommented = false;
                                var commenterPublicIdentifier = string.Empty;
                                foreach (var commentItem in commentContentArray)
                                {
                                    isAlreadyCommented = false;
                                    var comment = JsonJArrayHandler.GetJTokenValue(commentItem, "comment", "values", 0,
                                        "value");
                                    commenterPublicIdentifier = JsonJArrayHandler.GetJTokenValue(commentItem,
                                        "commenter", "com.linkedin.voyager.feed.MemberActor", "miniProfile",
                                        "publicIdentifier");

                                    if (lstCommentInDom != null)
                                        isAlreadyCommented = lstCommentInDom.Any(commentInDom =>
                                            string.Equals(commentInDom, comment,
                                                StringComparison.CurrentCultureIgnoreCase));
                                    if (isAlreadyCommented)
                                        break;
                                }

                                if (isAlreadyCommented && currentAccountProfileId == commenterPublicIdentifier)
                                    continue;
                            }
                            catch (Exception)
                            {
                                // ignored
                            }

                        #endregion

                        #region MyRegion

                        var postId =JsonJArrayHandler.GetJTokenValue(item,"socialDetail","urn");

                        #endregion

                        //  by default here are we getting details of normal post.
                        var objLinkedinPost = new LinkedinPost
                        {
                            Id = postId?.Replace("urn:li:activity:", "")?.Replace("urn:li:ugcPost:", ""),
                            PostLink = Utils.AssignNa($"https://www.linkedin.com/feed/update/{postId}"),
                            PostTrackingId =
                                Utils.AssignNa(JsonJArrayHandler.GetJTokenValue(item, "tracking", "trackingId")),
                            IsLiked = isLiked,
                            Caption = GetFullCaption(item),
                            PostVideoUrl = Utils.AssignNa(JsonJArrayHandler.GetJTokenValue(item, "value",
                                "com.linkedin.voyager.feed.ShareUpdate"
                                , "content", "com.linkedin.voyager.feed.ShareVideo", "video",
                                "com.linkedin.voyager.common.MediaProxyVideo", "url")),
                            PublicIdentifier = JsonJArrayHandler.GetJTokenValue(item, "actor",
                                "image", "attributes", 0, "miniProfile", "publicIdentifier"),
                            ShareUrn = JsonJArrayHandler.GetJTokenValue(item, "updateMetadata", "shareUrn")
                        };

                        #region PostCaption

                        if (item.ToString().Contains("resharedUpdate"))
                            SharedPostLinkDetails(item, objLinkedinPost);

                        else if (objLinkedinPost.Caption == "N/A" && objLinkedinPost.PostImageUrl == "N/A" &&
                                 objLinkedinPost.PostVideoUrl == "N/A")
                            CaptionWithShareArticle(item, objLinkedinPost);
                        // again try if not get any of these(Caption, PostImageUrl, PostVideoUrl) because post may be shared post
                        else
                            NormalPostLinkDetails(item, objLinkedinPost);

                        #endregion


                        //MediaType
                        objLinkedinPost.MediaType = GetMediaType(objLinkedinPost);

                        SetPostCountDetails(item, objLinkedinPost);

                        objLinkedinPost.ShowShareButton =
                            Utils.AssignNa(JsonJArrayHandler.GetJTokenValue(item, "socialDetail", "showShareButton"));

                        SetPostedUserDetails(item, objLinkedinPost);
                        if (PostsList.All(x => x.PostLink != objLinkedinPost.PostLink))
                            PostsList.Add(objLinkedinPost);
                    }
                    // ReSharper disable once EmptyGeneralCatchClause
                    catch (Exception)
                    {
                    }

                #endregion
            }
            catch (Exception)
            {
                Success = false;
            }
        }

        private void ScrapePostForKeyword(string response)
        {
            try
            {
                var jsonHandler = new JsonHandler(response);
                var token = jsonHandler.GetJToken("elements");
                var array = new JsonHandler(token.LastOrDefault(x => jsonHandler.GetJTokenOfJToken(x, "items").ToList().Count > 1)).GetJToken("items");
                if (array.HasValues)
                {
                    foreach (var post in array)
                    {
                        var postDetails = jsonHandler.GetJTokenOfJToken(post, "itemUnion", "entityResult");
                        var fullName = jsonHandler.GetJTokenValue(postDetails, "title", "text");
                        var splittedName = Regex.Split(fullName, " ");
                        var profileUrl = jsonHandler.GetJTokenValue(postDetails, "actorNavigationUrl");
                        var ShareId = Utils.GetBetween(jsonHandler.GetJTokenValue(postDetails, "overflowActions"), " \"urn\": \"", "\"");
                        ShareId = string.IsNullOrEmpty(ShareId) ? jsonHandler.GetJTokenValue(postDetails, "insightsResolutionResults", "0", "socialActivityCountsInsight", "entityUrn")?.Replace("urn:li:fsd_socialActivityCounts:", "") : ShareId;
                        ShareId = string.IsNullOrEmpty(ShareId) ? jsonHandler.GetJTokenValue(postDetails, "insightsResolutionResults", "0", "socialDetailInsight", "threadUrn")?.Replace("urn:li:fsd_socialActivityCounts:", "") : ShareId;
                        var navigationUrl = jsonHandler.GetJTokenValue(postDetails, "navigationUrl");
                        navigationUrl = navigationUrl is null || !navigationUrl.Contains("urn:li") ?$"https://www.linkedin.com/feed/update/{jsonHandler.GetJTokenValue(postDetails, "trackingUrn")}/" : navigationUrl;
                        var likeCount = jsonHandler.GetJTokenValue(postDetails, "insightsResolutionResults", "0", "socialActivityCountsInsight", "numLikes");
                        likeCount = string.IsNullOrEmpty(likeCount) ? jsonHandler.GetJTokenValue(postDetails, "insightsResolutionResults", "0", "socialDetailInsight", "totalSocialActivityCounts", "numLikes") : likeCount;
                        var commentCount = jsonHandler.GetJTokenValue(postDetails, "insightsResolutionResults", "0", "socialActivityCountsInsight", "numComments");
                        commentCount = string.IsNullOrEmpty(commentCount) ? jsonHandler.GetJTokenValue(postDetails, "insightsResolutionResults", "0", "socialActivityCountsInsight", "numComments") : commentCount;
                        var objLinkedinPost = new LinkedinPost
                        {
                            PostLink = navigationUrl,
                            ProfileUrl = profileUrl,
                            Caption = jsonHandler.GetJTokenValue(postDetails, "summary", "text"),
                            Firstname = splittedName.First(),
                            Lastname = splittedName.Last(),
                            FullName = fullName,
                            CommentCount = commentCount,
                            LikeCount = likeCount,
                            IsLiked = jsonHandler.GetJTokenValue(postDetails, "insightsResolutionResults", "0", "socialActivityCountsInsight", "liked"),
                            ShareUrn = ShareId,
                            TrackingId = jsonHandler.GetJTokenValue(postDetails, "trackingId"),
                            PublicIdentifier =!string.IsNullOrEmpty(profileUrl)?profileUrl.Contains("company")?profileUrl.Split('/').Last(x=>x!=string.Empty):Utils.GetBetween(profileUrl, "https://www.linkedin.com/in/", "?miniProfileUrn="):"",
                            Id=jsonHandler.GetJTokenValue(postDetails, "trackingUrn")?.Replace("urn:li:activity:", ""),
                            ProfilePicUrl=jsonHandler.GetJTokenValue(postDetails,"image", "attributes",0, "detailDataUnion", "nonEntityCompanyLogo", "vectorImage", "artifacts",0, "fileIdentifyingUrlPathSegment")
                        };
                        if (PostsList.All(x => x.PostLink != objLinkedinPost.PostLink))
                            PostsList.Add(objLinkedinPost);

                    }
                }
                else
                {
                    array = jsonHandler.GetJToken("elements");
                    if (array.HasValues)
                    {
                        foreach(var element in array)
                        {
                            var objLinkedinPost = new LinkedinPost
                            {
                                FullName = jsonHandler.GetJTokenValue(element, "actor", "name", "text"),
                                PostLink = jsonHandler.GetJTokenValue(jsonHandler.GetJTokenOfJToken(element, "updateMetadata", "actions").FirstOrDefault(x => x.ToString().Contains("\"SHARE_VIA\"")), "url")?.Replace("_android", "_desktop"),
                                ProfilePicUrl = jsonHandler.GetJTokenValue(element, "actor", "image", "attributes", 0, "miniCompany", "logo", "com.linkedin.common.VectorImage", "rootUrl"),
                                ProfileUrl = jsonHandler.GetJTokenValue(element, "actor", "navigationContext", "actionTarget"),
                                Caption = jsonHandler.GetJTokenValue(element, "commentary", "text", "text"),
                                CommentCount = jsonHandler.GetJTokenValue(element, "socialDetail", "totalSocialActivityCounts", "numComments"),
                                LikeCount=jsonHandler.GetJTokenValue(element, "socialDetail", "totalSocialActivityCounts", "numLikes"),
                                ShareUrn=jsonHandler.GetJTokenValue(element, "updateMetadata", "shareUrn"),
                                TrackingId=jsonHandler.GetJTokenValue(element, "updateMetadata", "trackingData", "trackingId"),
                                PublicIdentifier=jsonHandler.GetJTokenValue(element, "actor", "image", "attributes", 0, "miniCompany", "universalName"),
                                Id=jsonHandler.GetJTokenValue(element, "updateMetadata", "urn")?.Replace("urn:li:activity:", ""),
                                IsLiked=jsonHandler.GetJTokenValue(element, "socialDetail", "totalSocialActivityCounts", "liked"),
                            };
                            if (PostsList.All(x => x.PostLink != objLinkedinPost.PostLink))
                                PostsList.Add(objLinkedinPost);
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        internal JsonJArrayHandler JsonJArrayHandler { get; set; } = JsonJArrayHandler.GetInstance;

        public List<LinkedinPost> PostsList { get; } = new List<LinkedinPost>();
        public string PaginationToken { get; }


        private void BrowserProcessResponse(ActivityType activityType)
        {
            try
            {                
                var postList = new List<string>();
                var automationExtension = new BrowserAutomationExtension(null);
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(Utils.GetDecodedResponse(Response.Response,true,true));
                var GroupId= Utils.GetBetween(Response.Response, "www.linkedin.com/groups/", "\"");
                PostsList.Clear();
                var userNodeList = HtmlAgilityHelper.GetListNodesFromAttibute(Response.Response, "div",
                    AttributeIdentifierType.Id, htmlDoc, "data-urn=\"urn:li:activity:");
                if(userNodeList.Count()<=0)
                    userNodeList = HtmlAgilityHelper.GetListNodesFromAttibute(Response.Response, "div",
                    AttributeIdentifierType.ClassName, htmlDoc, "ember-view  occludable-update ");
                foreach (var user in userNodeList)
                {
                    var postDoc = new HtmlDocument();
                    postDoc.LoadHtml(user.OuterHtml);

                    var linkedinPost = new LinkedinPost();
                    if (user.OuterHtml.Contains("artdeco-button__text react-button__text react-button__text--like") || user.OuterHtml.Contains("react-button__text--like"))
                        continue;
                    linkedinPost.ProfileUrl = LdConstants.ProfileUrlConstant +
                                              LdDataHelper.GetInstance.GetPublicIdentifierFromPageSource(user.OuterHtml);
                    var InnerHtml =user.OuterHtml.Contains("reposted this")?HtmlAgilityHelper.GetListInnerHtmlOrInnerTextOrOuterHtmlFromIdOrClassName(user.OuterHtml,string.Empty,false, "update-components-actor__name hoverable-link-text")?.FirstOrDefault() :HtmlAgilityHelper.GetStringInnerHtmlFromClassName(user.OuterHtml, "update-components-actor__name hoverable-link-text");
                    InnerHtml = string.IsNullOrEmpty(InnerHtml) ? HtmlAgilityHelper.GetStringInnerHtmlFromClassName(user.OuterHtml, "update-components-actor__title") : InnerHtml;
                    var FullName =Utils.RemoveHtmlTags(Utils.GetBetween(InnerHtml, "dir=\"ltr\">", "</span>"));
                    if (FullName.Contains("<span aria-hidden"))
                        FullName = Utils.GetBetween(user.OuterHtml, "<!---->", "<!---->");
                    linkedinPost.FullName=FullName;
                    var UserPostId= Utils.GetBetween(user.OuterHtml, "data-urn=\"", "\"");
                    var PostLink = "https://www.linkedin.com/feed/update/" + UserPostId;
                    if (!PostLink.Contains("urn:li:activity"))
                        PostLink = "https://www.linkedin.com/feed/update/" + Utils.GetBetween(user.OuterHtml, "data-chameleon-result-urn=\"", "\">");
                    linkedinPost.PostLink= PostLink;
                    if (linkedinPost.FullName == "Linkedin Member" || postList.Contains(linkedinPost.PostLink))
                        continue;
                    var HeadLine= WebUtility.HtmlDecode(HtmlAgilityHelper.GetStringInnerTextFromClassName("","feed-shared-actor__description t-12 t-normal t-black--light", postDoc)?.Trim())?.Split('\n')[0];
                    HeadLine = string.IsNullOrEmpty(HeadLine) ? WebUtility.HtmlDecode(HtmlAgilityHelper.GetStringInnerTextFromClassName("","entity-result__primary-subtitle t-14 t-black t-normal", postDoc)?.Trim())?.Split('\n')[0]: HeadLine;
                    linkedinPost.HeadlineTitle = HeadLine?.Replace("\r\n", "")?.Replace("\n", "")?.Replace(",", "")?.Replace("\r","");
                    linkedinPost.Location =HtmlAgilityHelper.GetStringInnerTextFromClassName("", "result-lockup__misc-item", postDoc);
                    var ProfileId= Utils.GetBetween(user.OuterHtml, "urn:li:fs_miniProfile:", "\"");
                    ProfileId = string.IsNullOrEmpty(ProfileId) ? Utils.GetBetween(user.OuterHtml, "fs_miniProfile%3A", "\">")?.Replace("\" data-test-app-aware-link=\"", ""): ProfileId;
                    ProfileId = string.IsNullOrEmpty(ProfileId) ?Utils.GetBetween(user.OuterHtml, "urn:li:fsd_profile:", "\"") : ProfileId;
                    linkedinPost.ProfileId = ProfileId;
                    linkedinPost.ProfilePicUrl = LdDataHelper.GetInstance.GetSource(user.OuterHtml);
                    if (string.IsNullOrEmpty(linkedinPost.ProfilePicUrl))
                    {
                        var ProfilePicUrlString = Utils.GetBetween(user.OuterHtml, "<div class=\"relative full-width display-flex flex-column align-items-center\">", "</a>");
                        linkedinPost.ProfilePicUrl = Utils.GetBetween(ProfilePicUrlString, "src=\"", "\" height=\"");
                    }
                    var Caption= Utils.RemoveHtmlTags(HtmlAgilityHelper.GetStringInnerTextFromClassName("",
                        "feed-shared-update-v2__description-wrapper", postDoc));
                    Caption = string.IsNullOrEmpty(Caption) ? Utils.RemoveHtmlTags(HtmlAgilityHelper.GetStringInnerTextFromClassName("",
                        "feed-shared-inline-show-more-text", postDoc)): Caption;
                    linkedinPost.Caption = Caption?.Replace("\r\n","")?.Replace("\n","")?.Replace(",","")?.Replace("\r", "");
                    var date = HtmlAgilityHelper.GetStringInnerHtmlFromClassName("",
                        "feed-shared-actor__sub-description t-12 t-normal t-black--light", postDoc);
                    date = string.IsNullOrEmpty(date) ? HtmlAgilityHelper.GetStringInnerHtmlFromClassName("",
                        "feed-shared-actor__sub-description t-12 t-normal", postDoc): date;
                    date = string.IsNullOrEmpty(date) ? HtmlAgilityHelper.GetStringInnerHtmlFromClassName("",
                        "update-components-actor__sub-description", postDoc) : date;
                    var datetime = Utils.GetBetween(date, "<span class=\"visually-hidden\">", "<");
                    if (linkedinPost.ProfilePicUrl.Contains("data:image/gif"))
                        linkedinPost.ProfilePicUrl = "";
                    var NodeId = Utils.GetBetween(Utils.GetBetween(user.OuterHtml, "class=\"ember-view  occludable-update \">", $"data-urn=\"{UserPostId}\">"), "id=\"", "\"");
                    linkedinPost.NodeId =string.IsNullOrEmpty(NodeId)?HtmlAgilityHelper.GetListNodesFromAttibute(user.OuterHtml,"div",AttributeIdentifierType.Id,null,$"data-urn=\"{UserPostId}\"")?.LastOrDefault(x=>!x.Id.Contains("voyager-feed"))?.Id:NodeId;
                    var CommentCount= Regex.Replace(
                            HtmlAgilityHelper.GetStringInnerTextFromClassName("",
                                "social-details-social-counts__comments social-details-social-counts__item", postDoc),
                            "[^0-9]", "");
                    CommentCount = string.IsNullOrEmpty(CommentCount) ? Regex.Replace(
                            HtmlAgilityHelper.GetStringInnerTextFromClassName("",
                                "v-align-middle", postDoc),
                            "[^0-9]", ""): CommentCount;
                    CommentCount = string.IsNullOrEmpty(CommentCount) ? Regex.Replace(
                            HtmlAgilityHelper.GetStringInnerTextFromClassName("",
                                "social-details-social-counts__item social-details-social-counts__comments", postDoc),
                            "[^0-9]", ""): CommentCount;
                    CommentCount= string.IsNullOrEmpty(CommentCount) ? Regex.Replace(
                            HtmlAgilityHelper.GetStringInnerTextFromClassName("",
                                "t-black--light social-details-social-counts__count-value", postDoc),
                            "[^0-9]", "") : CommentCount;
                    linkedinPost.CommentCount = CommentCount;
                    var LikeCount = Regex.Replace(
                            HtmlAgilityHelper.GetStringInnerTextFromClassName("",
                                "social-details-social-counts__reactions-count", postDoc),
                            "[^0-9]", "");
                    LikeCount = string.IsNullOrEmpty(LikeCount) ? Utils.GetBetween(Utils.GetBetween(user.OuterHtml, "data-test-reactions-icon-theme=\"light\">", "&nbsp;</span>"), "<span aria-hidden=\"true\">", "</span>") : LikeCount;
                    LikeCount = string.IsNullOrEmpty(LikeCount) ? Regex.Replace(HtmlAgilityHelper.GetStringInnerTextFromClassName("","social-details-social-counts__reactions-count", postDoc),"[^0-9]", ""): LikeCount;
                    LikeCount = string.IsNullOrEmpty(LikeCount) ? Regex.Replace(HtmlAgilityHelper.GetStringInnerTextFromClassName("", "social-details-social-counts__reactions-count", postDoc), "[^0-9]", "") : LikeCount;
                    linkedinPost.LikeCount = LikeCount;
                    linkedinPost.Id = GroupId;
                    postList.Add(linkedinPost.PostLink);
                    PostsList.Add(linkedinPost);
                }

                if (PostsList.Count == 0)
                    Success = false;
            }
            catch (Exception)
            {
                // exception.DebugLog();
            }
        }

        private void SharedPostLinkDetails(JToken jToken, LinkedinPost objLinkedinPost)
        {
            #region Caption with SharedPostLinkDetails

            objLinkedinPost.Caption =string.IsNullOrEmpty(objLinkedinPost.Caption)? Utils.AssignNa(JsonJArrayHandler.GetJTokenValue(jToken, "value",
                "com.linkedin.voyager.feed.render.UpdateV2"
                , "resharedUpdate", "commentary", "text", "text")):objLinkedinPost.Caption;

            objLinkedinPost.ShareUrn =string.IsNullOrEmpty(objLinkedinPost.ShareUrn)? JsonJArrayHandler.GetJTokenValue(jToken, "value",
                "com.linkedin.voyager.feed.render.UpdateV2", "resharedUpdate", "updateMetadata"
                , "urn"):objLinkedinPost.ShareUrn;

            objLinkedinPost.PostLink = string.IsNullOrEmpty(objLinkedinPost.PostLink) ? $"https://www.linkedin.com/feed/update/{objLinkedinPost.ShareUrn}": objLinkedinPost.PostLink;
            
            objLinkedinPost.Caption = Utils.RemoveSpecialCharacters(objLinkedinPost.Caption);

            #endregion

            objLinkedinPost.PostImageUrl = Utils.AssignNa(JsonJArrayHandler.GetJTokenValue(jToken, "value",
                "originalUpdate", "value", "com.linkedin.voyager.feed.ShareUpdate"
                , "content", "com.linkedin.voyager.feed.ShareImage", "image",
                "com.linkedin.voyager.common.MediaProxyImage", "url"));

            objLinkedinPost.PostVideoUrl = Utils.AssignNa(JsonJArrayHandler.GetJTokenValue(jToken, "value",
                "com.linkedin.voyager.feed.render.UpdateV2", "resharedUpdate", "content",
                "com.linkedin.voyager.feed.render.LinkedInVideoComponent", "videoPlayMetadata", "entityUrn"));
        }

        private void NormalPostLinkDetails(JToken jToken, LinkedinPost objLinkedinPost)
        {
            objLinkedinPost.Caption =string.IsNullOrEmpty(objLinkedinPost.Caption)? Utils.AssignNa($"{objLinkedinPost.Caption.Replace("N/A", "")} " +
                                                     JsonJArrayHandler.GetJTokenValue(jToken, "value",
                                                         "com.linkedin.voyager.feed.render.UpdateV2", "content",
                                                         "com.linkedin.voyager.feed.render.ArticleComponent",
                                                         "navigationContext", "actionTarget")):objLinkedinPost.Caption;
            objLinkedinPost.Caption = Utils.RemoveSpecialCharacters(objLinkedinPost.Caption?.Trim());
            
            objLinkedinPost.PostImageUrl = Utils.AssignNa(JsonJArrayHandler.GetJTokenValue(jToken, "value",
                "com.linkedin.voyager.feed.ShareUpdate", "content", "com.linkedin.voyager.feed.ShareImage", "image",
                "com.linkedin.voyager.common.MediaProxyImage", "url"));

            #region PostVideoUrl

            try
            {
                var videoPlayMetadataArray = JsonJArrayHandler.GetJArrayElement(JsonJArrayHandler.GetJTokenValue(jToken, "value", "com.linkedin.voyager.feed.ShareUpdate",
                    "content", "com.linkedin.voyager.feed.ShareNativeVideo", "videoPlayMetadata", "progressiveStreams"));
                if (videoPlayMetadataArray != null && videoPlayMetadataArray.Count > 0)
                    objLinkedinPost.PostVideoUrl =
                        videoPlayMetadataArray.First["streamingLocations"][0]["url"].ToString();
            }
            catch (Exception)
            {
                objLinkedinPost.PostVideoUrl = "N/A";
            }

            #endregion
        }

        private void CaptionWithShareArticle(JToken jToken, LinkedinPost objLinkedinPost)
        {
            #region Caption with ShareArticle details

            objLinkedinPost.Caption =string.IsNullOrEmpty(objLinkedinPost.Caption)? Utils.AssignNa(JsonJArrayHandler.GetJTokenValue(jToken, "value",
                "com.linkedin.voyager.feed.ShareUpdate", "content", "com.linkedin.voyager.feed.ShareArticle", "text",
                "values", 0, "value")):objLinkedinPost.Caption;
            
            objLinkedinPost.Caption = Utils.RemoveSpecialCharacters(objLinkedinPost.Caption);

            #endregion

            objLinkedinPost.PostImageUrl = Utils.AssignNa(JsonJArrayHandler.GetJTokenValue(jToken, "value",
                "com.linkedin.voyager.feed.ShareUpdate", "content", "com.linkedin.voyager.feed.ShareArticle", "image",
                "com.linkedin.voyager.common.MediaProxyImage", "url"));
            objLinkedinPost.PostVideoUrl = Utils.AssignNa(JsonJArrayHandler.GetJTokenValue(jToken, "value",
                "com.linkedin.voyager.feed.ShareUpdate", "content", "com.linkedin.voyager.feed.ShareArticle", "video",
                "com.linkedin.voyager.common.MediaProxyVideo", "url"));
        }


        private void SetPostCountDetails(JToken jToken, LinkedinPost objLinkedinPost)
        {
            var tempToken = jToken.DeepClone();
            objLinkedinPost.LikeCount =
                JsonJArrayHandler.GetJTokenValue(tempToken, "socialDetail", "totalSocialActivityCounts", "numLikes");


            if (string.IsNullOrEmpty(objLinkedinPost.LikeCount))
            {
                // we keeping in tempToken because from start again we crawling same path
                tempToken = JsonJArrayHandler.GetTokenElement(tempToken, "value",
                    "com.linkedin.voyager.feed.render.UpdateV2", "socialDetail");
                objLinkedinPost.LikeCount =
                    Utils.AssignNa(JsonJArrayHandler.GetJTokenValue(tempToken, "likes", "paging", "total"));
            }

            objLinkedinPost.CommentCount =
                JsonJArrayHandler.GetJTokenValue(tempToken, "socialDetail", "totalSocialActivityCounts", "numComments");
            if (string.IsNullOrEmpty(objLinkedinPost.CommentCount))
                objLinkedinPost.CommentCount =
                    Utils.AssignNa(JsonJArrayHandler.GetJTokenValue(tempToken, "comments", "paging", "total"));

            objLinkedinPost.ShareCount =
                JsonJArrayHandler.GetJTokenValue(tempToken, "socialDetail", "totalShares");
            if (string.IsNullOrEmpty(objLinkedinPost.ShareCount))
                objLinkedinPost.ShareCount = Utils.AssignNa(JsonJArrayHandler.GetJTokenValue(tempToken, "totalShares"));
        }


        private void SetPostedUserDetails(JToken jToken, LinkedinPost objLinkedinPost)
        {
            #region PosterDetails

            try
            {
                var tempToken = jToken.DeepClone();

                #region first Name

                tempToken = JsonJArrayHandler.GetTokenElement(tempToken, "actor", "name", "attributes", 0,
                                "miniProfile") ??
                            JsonJArrayHandler.GetTokenElement(jToken, "value",
                                "com.linkedin.voyager.feed.ShareUpdate", "actor",
                                "com.linkedin.voyager.feed.MemberActor", "miniProfile");
                var firstName = JsonJArrayHandler.GetJTokenValue(tempToken, "firstName");

                #endregion

                var lastName = JsonJArrayHandler.GetJTokenValue(tempToken, "lastName");

                #region FullName

                if (string.IsNullOrEmpty(firstName) || firstName == "N/A")
                    objLinkedinPost.FullName = "Linkedin Member";
                else if (string.IsNullOrEmpty(lastName) || lastName == "N/A")
                    objLinkedinPost.FullName = firstName;
                else
                    objLinkedinPost.FullName = firstName + " " + lastName;

                objLinkedinPost.FullName = Utils.InsertSpecialCharactersInCsv(objLinkedinPost.FullName);

                #endregion

                #region PublicIdentifier,ProfileUrl,HasAnonymousProfilePicture,ProfilePicUrl

                objLinkedinPost.PublicIdentifier = JsonJArrayHandler.GetJTokenValue(jToken, "value",
                    "com.linkedin.voyager.feed.ShareUpdate", "actor", "com.linkedin.voyager.feed.MemberActor",
                    "miniProfile", "publicIdentifier");
                if (string.IsNullOrEmpty(objLinkedinPost.PublicIdentifier))
                    objLinkedinPost.PublicIdentifier = JsonJArrayHandler.GetJTokenValue(jToken, "value",
                        "com.linkedin.voyager.feed.Reshare", "actor", "com.linkedin.voyager.feed.MemberActor",
                        "miniProfile", "publicIdentifier");
                if (string.IsNullOrEmpty(objLinkedinPost.PublicIdentifier))
                    objLinkedinPost.PublicIdentifier = JsonJArrayHandler.GetJTokenValue(tempToken, "publicIdentifier");


                objLinkedinPost.ProfileUrl = "https://www.linkedin.com/in/" + objLinkedinPost.PublicIdentifier;

                #region you might remove it later since it keeps changing

                // backgroundImage
                var backgroundImage = JsonJArrayHandler.GetJTokenValue(jToken, "value","com.linkedin.voyager.feed.ShareUpdate", "actor", "com.linkedin.voyager.feed.MemberActor","miniProfile", "backgroundImage", "com.linkedin.common.VectorImage");
                if (string.IsNullOrEmpty(backgroundImage))
                    backgroundImage = JsonJArrayHandler.GetJTokenValue(jToken, "value","resharedUpdate", "actor", "com.linkedin.voyager.feed.MemberActor","miniProfile", "backgroundImage", "com.linkedin.common.VectorImage");
                backgroundImage = string.IsNullOrEmpty(backgroundImage) ? JsonJArrayHandler.GetJTokenValue(tempToken, "backgroundImage", "com.linkedin.common.VectorImage", "rootUrl")+ JsonJArrayHandler.GetJTokenValue(JsonJArrayHandler.GetJArrayElement(JsonJArrayHandler.GetJTokenValue(tempToken, "backgroundImage", "com.linkedin.common.VectorImage", "artifacts"))?.LastOrDefault(), "fileIdentifyingUrlPathSegment") : backgroundImage;
                if (string.IsNullOrEmpty(backgroundImage))
                    backgroundImage = JsonJArrayHandler.GetJTokenValue(jToken, "value","com.linkedin.voyager.feed.Reshare", "actor", "com.linkedin.voyager.feed.MemberActor","miniProfile", "backgroundImage", "com.linkedin.common.VectorImage");
                backgroundImage = Utils.AssignNa(backgroundImage);
                // picture
                var picture = JsonJArrayHandler.GetJTokenValue(jToken, "value", "com.linkedin.voyager.feed.ShareUpdate","actor", "com.linkedin.voyager.feed.MemberActor", "miniProfile", "picture","com.linkedin.common.VectorImage");
                if (string.IsNullOrEmpty(picture))
                    picture = JsonJArrayHandler.GetJTokenValue(jToken, "value","com.linkedin.voyager.feed.Reshare", "actor", "com.linkedin.voyager.feed.MemberActor","miniProfile", "picture", "com.linkedin.common.VectorImage");
                picture =Utils.AssignNa(string.IsNullOrEmpty(picture) ? JsonJArrayHandler.GetJTokenValue(tempToken, "picture", "com.linkedin.common.VectorImage", "rootUrl") + JsonJArrayHandler.GetJTokenValue(JsonJArrayHandler.GetJArrayElement(JsonJArrayHandler.GetJTokenValue(tempToken, "picture", "com.linkedin.common.VectorImage", "artifacts"))?.LastOrDefault(), "fileIdentifyingUrlPathSegment") : picture);
                #endregion

                try
                {
                    var profilePicData = Utils.GetBetween(jToken.ToString(), "com.linkedin.common.VectorImage",
                                "publicIdentifier");
                    var haveProfilePic = profilePicData.Contains("https://media.licdn.com/dms/image/");

                    if (backgroundImage != "N/A" || picture != "N/A" || haveProfilePic)
                    {
                        objLinkedinPost.HasAnonymousProfilePicture = true;
                        objLinkedinPost.ProfilePicUrl = picture;
                    }
                }
                catch (Exception)
                {
                }
                #endregion

                #region Occupation

                objLinkedinPost.Occupation = JsonJArrayHandler.GetJTokenValue(jToken, "value",
                    "com.linkedin.voyager.feed.ShareUpdate", "actor", "com.linkedin.voyager.feed.MemberActor",
                    "miniProfile", "occupation");
                if (string.IsNullOrEmpty(objLinkedinPost.Occupation))
                    objLinkedinPost.Occupation = JsonJArrayHandler.GetJTokenValue(jToken, "value",
                        "com.linkedin.voyager.feed.Reshare", "actor", "com.linkedin.voyager.feed.MemberActor",
                        "miniProfile", "occupation");

                if (string.IsNullOrEmpty(objLinkedinPost.Occupation))
                    objLinkedinPost.Occupation = JsonJArrayHandler.GetJTokenValue(tempToken, "occupation");

                objLinkedinPost.Occupation = string.IsNullOrEmpty(objLinkedinPost.Occupation)
                    ? "N/A"
                    : Utils.InsertSpecialCharactersInCsv(objLinkedinPost.Occupation);

                #endregion

                #region CompanyName

                try
                {
                    if (objLinkedinPost.Occupation != "N/A" && objLinkedinPost.Occupation.Contains(" at ")&& !objLinkedinPost.Occupation.Contains("|"))
                        objLinkedinPost.CompanyName = Regex.Split(objLinkedinPost.Occupation, " at ")?.LastOrDefault();
                    else
                    {
                        var Occupation = Regex.Split(objLinkedinPost.Occupation, "\\|")?.FirstOrDefault(x=>x.Contains(" at "));
                        objLinkedinPost.CurrentCompany = Regex.Split(Occupation, " at ")?.LastOrDefault();
                    }
                }

                catch (Exception)
                {
                    //ignored
                }

                #endregion

                #region ProfileId

                objLinkedinPost.ProfileId = JsonJArrayHandler.GetJTokenValue(jToken, "value",
                    "com.linkedin.voyager.feed.ShareUpdate", "actor", "com.linkedin.voyager.feed.MemberActor",
                    "miniProfile", "entityUrn");
                if (string.IsNullOrEmpty(objLinkedinPost.ProfileId))
                    objLinkedinPost.ProfileId = JsonJArrayHandler.GetJTokenValue(jToken, "value",
                        "com.linkedin.voyager.feed.Reshare", "actor", "com.linkedin.voyager.feed.MemberActor",
                        "miniProfile", "occupation");

                if (string.IsNullOrEmpty(objLinkedinPost.ProfileId))
                    objLinkedinPost.ProfileId = JsonJArrayHandler.GetJTokenValue(tempToken, "entityUrn")
                        .Replace("urn:li:fs_miniProfile:", "");

                #endregion

                //MemberId
                objLinkedinPost.MemberId = JsonJArrayHandler.GetJTokenValue(tempToken, "objectUrn")
                    .Replace("urn:li:member:", "");


                #region ConnectionType

                var degree = jToken.ToString().Contains($"Message {firstName}") ? "1" : "";
               
                if (string.IsNullOrEmpty(degree))
                    degree = Utils.AssignNa(JsonJArrayHandler.GetJTokenValue(jToken, "value",
                        "com.linkedin.voyager.feed.Reshare", "actor", "com.linkedin.voyager.feed.MemberActor",
                        "distance", "value").Replace("DISTANCE_", ""));

                objLinkedinPost.ConnectionType = GetConnectionType(degree);

                #endregion

                var postedTimeStringElement = JsonJArrayHandler.GetJTokenValue(jToken, "value",
                    "com.linkedin.voyager.feed.Reshare", "createdTime");
                var postedTime = objLinkedinPost.PostedTime;
                if (string.IsNullOrEmpty(postedTimeStringElement))
                    postedTimeStringElement = JsonJArrayHandler.GetJTokenValue(jToken, "value",
                        "com.linkedin.voyager.feed.Reshare",
                        "createdTime");
                long.TryParse(postedTimeStringElement, out postedTime);
            }
            catch (Exception)
            {
                //ignored
            }

            #endregion
        }

        private string GetFullCaption(JToken jToken)
        {
            var fullCaption = JsonJArrayHandler.GetJTokenValue(jToken, "commentary", "text", "text");
            if (string.IsNullOrEmpty(fullCaption))
                fullCaption = JsonJArrayHandler.GetJTokenValue(jToken, "value",
                    "com.linkedin.voyager.feed.render.UpdateV2", "commentary", "text", "text");
            return Utils.AssignNa(Utils.InsertSpecialCharactersInCsv(fullCaption));
        }

        private MediaType GetMediaType(LinkedinPost objLinkedinPost)
        {
            if (objLinkedinPost.PostVideoUrl != "N/A" && objLinkedinPost.PostImageUrl == "N/A")
                return MediaType.Video;
            if (objLinkedinPost.PostImageUrl != "N/A" && objLinkedinPost.PostVideoUrl == "N/A")
                return MediaType.Image;
            return MediaType.NoMedia;
        }

        private ConnectionType GetConnectionType(string degree)
        {
            switch (degree)
            {
                case "1":
                    return ConnectionType.FirstDegree;
                case "2":
                    return ConnectionType.SeondDegree;
                default:
                    return ConnectionType.ThirdPlusDegree;
            }
        }
    }
}