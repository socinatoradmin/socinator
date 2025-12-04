using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ThreadUtils;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using HtmlAgilityPack;
using LinkedDominatorCore.Factories;
using LinkedDominatorCore.LDLibrary.DAL;
using LinkedDominatorCore.LDModel;
using LinkedDominatorCore.LDModel.Engage;
using LinkedDominatorCore.LDModel.LDUtility;
using LinkedDominatorCore.LDUtility;
using LinkedDominatorCore.Utility;
using Newtonsoft.Json.Linq;

namespace LinkedDominatorCore.LDLibrary.Processor.Posts
{
    public class CustomUrlPostsProcessor : BaseLinkedinPostProcessor
    {
        private readonly IProcessScopeModel _processScopeModel;
        bool IsCheckSkipBlackListedUser = false;
        bool isChkPrivateBlackList = false;
        bool isChkGroupBlackList = false;
        int SkippedUserCount = 0;
        public CustomUrlPostsProcessor(ILdJobProcess jobProcess,
            IDbCampaignService campaignService, ILdFunctionFactory ldFunctionFactory,
            IProcessScopeModel processScopeModel, IDelayService delayService) :
            base(jobProcess, campaignService, ldFunctionFactory, delayService, processScopeModel)
        {
            _processScopeModel = processScopeModel;
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            try
            {
                var maxNumberOfActionPerUser = 0;
                var maxNumberofActionPerGroup = 0;
                var CustomPostUrl = new List<LinkedinPost>();
                var jArr = JsonJArrayHandler.GetInstance;
                List<string> lstCommentInDom = null;

                switch (ActivityType)
                {
                    case ActivityType.Like:
                    {
                        var likeModel = _processScopeModel.GetActivitySettingsAs<LikeModel>();
                        isChkPrivateBlackList = likeModel.IsChkPrivateBlackList;
                        IsCheckSkipBlackListedUser = likeModel.IsChkSkipBlackListedUser;
                        isChkGroupBlackList = likeModel.IsChkGroupBlackList;
                        maxNumberOfActionPerUser =
                            likeModel.IsNumberOfPostToLike ? likeModel.MaxNumberOfPostPerUserToLike : 0;
                        maxNumberofActionPerGroup = likeModel.IsNumberOfGroupPostToLike
                            ? likeModel.MaxNumberOfPostPerGroupToLike
                            : 0;
                        break;
                    }

                    case ActivityType.Comment:
                    {
                        var commentModel = _processScopeModel.GetActivitySettingsAs<CommentModel>();
                        IsCheckSkipBlackListedUser = commentModel.IsChkSkipBlackListedUser;
                        isChkPrivateBlackList = commentModel.IsChkPrivateBlackList;
                        isChkGroupBlackList = commentModel.IsChkGroupBlackList;
                        lstCommentInDom = new List<string>();
                        commentModel.LstDisplayManageCommentModel.ForEach(x =>
                        {
                            if (!lstCommentInDom.Contains(x.CommentText))
                                lstCommentInDom.Add(x.CommentText);
                        });
                        maxNumberOfActionPerUser = commentModel.IsNumberOfPostToComment
                            ? commentModel.MaxNumberOfPostPerUserToComment
                            : 0;
                        maxNumberofActionPerGroup = commentModel.IsNumberOfGroupPostToComment
                            ? commentModel.MaxNumberOfPostPerGroupToComment
                            : 0;
                        break;
                    }

                    case ActivityType.Share:
                    {
                        var shareModel = _processScopeModel.GetActivitySettingsAs<ShareModel>();
                        IsCheckSkipBlackListedUser = shareModel.IsChkSkipBlackListedUser;
                        isChkPrivateBlackList = shareModel.IsChkPrivateBlackList;
                        isChkGroupBlackList = shareModel.IsChkGroupBlackList;
                        maxNumberOfActionPerUser = shareModel.IsNumberOfPostToShare
                            ? shareModel.MaxNumberOfPostPerUserToShare
                            : 0;
                        break;
                    }
                }
                var isliked = false;
                var url = "";
                var postId = "";
                if(queryInfo.QueryType== "Custom Posts List")
                {
                    postId = Utils.GetBetween($"{queryInfo.QueryValue}/", "activity:", "?utm_source=");
                    postId = string.IsNullOrEmpty(postId) ?Utils.GetBetween($"{queryInfo.QueryValue}/", "activity-", "-"): postId;
                    postId = string.IsNullOrEmpty(postId) ?Utils.GetBetween(queryInfo.QueryValue,"ugcPost-","-"): postId;
                    postId = string.IsNullOrEmpty(postId) ?Regex.Replace(queryInfo.QueryValue,"[^0-9]",""): postId;
                    postId = Regex.Replace(postId, "[^0-9]", "");
                    url = IsBrowser
                        ? queryInfo.QueryValue
                        : $"https://www.linkedin.com/voyager/api/feed/updatesV2?commentsCount=10&likesCount=10&moduleKey=feed-item%3Adesktop&q=backendUrnOrNss&urnOrNss=urn%3Ali%3Aactivity%3A{postId}";
                }
                else if (queryInfo.QueryValue.Contains("https://www.linkedin.com/feed/update/"))
                {
                    postId = Utils.GetBetween($"{queryInfo.QueryValue}/", "activity:", "/");
                    url = IsBrowser
                        ? queryInfo.QueryValue
                        : $"https://www.linkedin.com/voyager/api/feed/updatesV2?commentsCount=10&likesCount=10&moduleKey=feed-item%3Adesktop&q=backendUrnOrNss&urnOrNss=urn%3Ali%3Aactivity%3A{postId}";
                }
                else
                {
                    postId = Utils.GetBetween($"{queryInfo.QueryValue}/", "posts/", "/");
                    url = IsBrowser
                        ? queryInfo.QueryValue
                        : $"https://www.linkedin.com/voyager/api/feed/updatesV2?commentsCount=10&likesCount=10&moduleKey=feed-item%3Adesktop&q=postSlug&slug={postId}";
                }

                var UserDetails = LdFunctions.GetHtmlFromUrlForMobileRequest(url, "");
                
                if (IsBrowser)
                {
                    SkippedUserCount = 0;
                    isliked = BrowserReponseHandler(queryInfo, CustomPostUrl, isliked, postId, UserDetails);
                }
                else
                {
                    SkippedUserCount = 0;
                    var jobj = jArr.ParseJsonToJObject(UserDetails);
                    var elements = jArr.GetTokenElement(jobj, "elements", 0);
                    var Firstname = jArr.GetJTokenValue(elements, "actor", "image", "attributes", 0, "miniProfile",
                        "firstName");
                    var Lastname = jArr.GetJTokenValue(elements, "actor", "image", "attributes", 0, "miniProfile",
                        "lastName");
                    var fullName = $"{Firstname} {Lastname}";
                    var PostDescribtion = jArr.GetJTokenValue(elements, "commentary", "text", "text");
                    var publicidentifier = jArr.GetJTokenValue(elements, "actor", "image", "attributes", 0,
                        "miniProfile", "publicIdentifier");
                    var profileId = $"https://www.linkedin.com/in/{publicidentifier}";
                    var numComment = jArr.GetJTokenValue(elements, "socialDetail", "totalSocialActivityCounts",
                        "numComments");
                    var numLike = jArr.GetJTokenValue(elements, "socialDetail", "totalSocialActivityCounts",
                        "numLikes");
                    var createdTime = jArr.GetJTokenValue(elements, "socialDetail", "comments", "elements", 0,
                        "createdTime");
                    var ActivityId = jArr.GetJTokenValue(elements, "updateMetadata", "shareUrn");
                    ActivityId = string.IsNullOrEmpty(ActivityId) ?jArr.GetJTokenValue(elements, "updateMetadata", "shareMediaUrn") : ActivityId;
                    long posttime;
                    long.TryParse(createdTime, out posttime);
                    isliked = jArr.GetJTokenValue(elements, "socialDetail", "liked").Contains("True");

                    var postlink = new LinkedinPost
                    {
                        PostLink = queryInfo.QueryValue,
                        Id = postId,
                        CommentCount = numComment,
                        LikeCount = numLike,
                        FullName = fullName,
                        PostedTime = posttime,
                        ProfileUrl = profileId,
                        Caption = PostDescribtion,
                        IsLiked = isliked.ToString(),
                        ShareUrn=ActivityId,
                        PublicIdentifier=publicidentifier,
                        ProfileId=profileId,
                        Firstname=Firstname,
                        Lastname=Lastname
                    };
                    if(!SkippedBlackListedUser(ref IsCheckSkipBlackListedUser,ref isChkGroupBlackList,ref isChkPrivateBlackList,manageBlacklistWhitelist,postlink,ref SkippedUserCount))
                        CustomPostUrl.Add(postlink);
                }
                if(SkippedUserCount > 0)
                    GlobusLogHelper.log.Info(Log.CustomMessage,DominatorAccountModel.AccountBaseModel.AccountNetwork,DominatorAccountModel.AccountBaseModel.UserName, ActivityType,$"Sucessfully Skipped {SkippedUserCount} BlackListed Users.");
                if (isliked && ActivityType == ActivityType.Like)
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                        DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                        "Already liked this post " + queryInfo.QueryValue + "");
                    return;
                }

                ProcessLinkedinPostsFromPost(queryInfo, ref jobProcessResult, CustomPostUrl, maxNumberOfActionPerUser,
                    maxNumberofActionPerGroup);
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException("Operation Cancelled!");
            }
            catch (Exception ex)
            {
                jobProcessResult.HasNoResult = true;
                ex.DebugLog();
            }
        }

        private bool SkippedBlackListedUser(ref bool isCheckSkipBlackListedUser, ref bool isChkGroupBlackList, ref bool isChkPrivateBlackList, ManageBlacklistWhitelist manageBlacklistWhitelist, LinkedinPost linkedinPost,ref int SkippedUserCount)
        {
            var ISkippedUser = false;
            if (isCheckSkipBlackListedUser && (isChkPrivateBlackList || isChkGroupBlackList) && manageBlacklistWhitelist.FilterBlackListedUser(linkedinPost.PublicIdentifier, isChkPrivateBlackList, isChkGroupBlackList))
            {
                ISkippedUser = true;
                SkippedUserCount++;
            }
            return ISkippedUser;
        }

        private bool BrowserReponseHandler(QueryInfo queryInfo, List<LinkedinPost> CustomPostUrl, bool isliked,
            string postId, string UserDetails)
        {
            var nodeId = "";
            var ShareNodeId = "";
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(Utils.GetDecodedResponse(UserDetails,true,true));
            var automationExtension = new BrowserAutomationExtension(null);
            var userNodeList = HtmlAgilityHelper.GetListNodesFromAttibute(UserDetails, "div",
                AttributeIdentifierType.Id, htmlDoc, "data-urn=\"urn:li:activity:");
            var userNodeList1 = new List<HtmlNode>();
            userNodeList.ForEach(x =>
            {
                var postactivityId = Utils.GetBetween(x.OuterHtml, "data-urn=\"", "\"");
                if (!userNodeList1.Any(y => y.OuterHtml.Contains(postactivityId))) userNodeList1.Add(x);
            });
            foreach (var users in userNodeList1)
            {
                var postactivityId = Utils.GetBetween(users.OuterHtml, "data-urn=\"", "\"");
                var PostId = Utils.GetBetween($"{postactivityId}/", "activity:", "/");
                if (!queryInfo.QueryValue.Contains(PostId) && queryInfo.QueryType!= "Custom Posts List")
                    continue;
                var postDoc = new HtmlDocument();
                postDoc.LoadHtml(users.OuterHtml);
                var ProfileNodeString = HtmlAgilityHelper.GetListNodesFromAttibute(users.OuterHtml,"a",AttributeIdentifierType.ClassName,postDoc, "app-aware-link  profile-rail-card__profile-link t-16 t-black t-bold tap-target")?.FirstOrDefault(Node=>!string.IsNullOrEmpty(Node.OuterHtml))?.OuterHtml;
                var ProfileUrl = Utils.GetBetween(ProfileNodeString, "href=\"", "\"");
                ProfileUrl =string.IsNullOrEmpty(ProfileUrl)? LdConstants.ProfileUrlConstant +
                                 LdDataHelper.GetInstance.GetPublicIdentifierFromPageSource(users.OuterHtml):ProfileUrl;
                var Caption = Utils.RemoveHtmlTags(
                    HtmlAgilityHelper.GetStringInnerHtmlFromClassName("",
                        "update-components-text relative feed-shared-update-v2__commentary ", postDoc));
                Caption = string.IsNullOrEmpty(Caption) ?Utils.RemoveHtmlTags(HtmlAgilityHelper.GetStringInnerTextFromClassName("", "update-components-text relative update-components-update-v2__commentary ",postDoc)) : Caption;
                var numComments = Utils.RemoveHtmlTags(
                        HtmlAgilityHelper.GetStringInnerHtmlFromClassName("",
                            "t-black--light social-details-social-counts__count-value", postDoc))
                    .Replace("Comments", "");
                var numLikes = Utils.RemoveHtmlTags(
                    HtmlAgilityHelper.GetStringInnerHtmlFromClassName("",
                        "t-black--light display-flex social-details-social-counts__count-value", postDoc))?.Replace(",","");
                var FullName = HtmlAgilityHelper.GetStringInnerTextFromClassName("",
                    "update-components-actor__name t-14 t-bold hoverable-link-text", postDoc)?.Trim();
                if (string.IsNullOrEmpty(nodeId = automationExtension.GetPath(users.OuterHtml, "button",
                    AttributeIdentifierType.Id, ActivityType.ToString())))
                {
                    nodeId = automationExtension.GetPath(users.OuterHtml, "artdeco-dropdown-trigger",
                        AttributeIdentifierType.Id, ActivityType.ToString());
                    ShareNodeId = automationExtension.GetPath(users.OuterHtml, "artdeco-dropdown-item",
                        AttributeIdentifierType.Id, "compose-icon");
                }
                var publicIdentifier =Utils.GetBetween(HtmlAgilityHelper.GetStringFromClassName(users.InnerHtml, "app-aware-link  profile-rail-card__profile-link t-16 t-black t-bold tap-target"), "href=\"https://www.linkedin.com/in/", "?miniProfileUrn=");
                publicIdentifier = string.IsNullOrEmpty(publicIdentifier) ?Utils.GetBetween(ProfileUrl, "https://www.linkedin.com/company/", "/?miniCompanyUrn") : publicIdentifier;
                isliked = users.OuterHtml.Contains("artdeco-button__text react-button__text react-button__text--like") || users.OuterHtml.Contains("react-button__text--like");

                var posdetails = new LinkedinPost
                {
                    PostLink = queryInfo.QueryValue,
                    Id = postId,
                    CommentCount = numComments,
                    LikeCount = numLikes,
                    FullName = FullName,
                    ProfileUrl = ProfileUrl,
                    Caption = Caption,
                    NodeId = nodeId,
                    ShareNodeId = ShareNodeId,
                    IsLiked = isliked.ToString(),
                    PublicIdentifier= publicIdentifier
                };
                if (!SkippedBlackListedUser(ref IsCheckSkipBlackListedUser, ref isChkGroupBlackList, ref isChkPrivateBlackList, manageBlacklistWhitelist, posdetails, ref SkippedUserCount))
                {
                    if(!CustomPostUrl.Any(y=>y.PostLink.Contains(posdetails.PostLink)))
                        CustomPostUrl.Add(posdetails);
                }
            }

            return isliked;
        }
    }
}