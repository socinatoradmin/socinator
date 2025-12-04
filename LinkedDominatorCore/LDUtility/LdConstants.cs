using System;
using System.Net;

namespace LinkedDominatorCore.LDUtility
{
    public class LdConstants
    {
        

        #region For Web Request

        public static string GroupSearchApiConstant =
            "https://www.linkedin.com/voyager/api/search/cluster?&guides=List(v-%3EGROUPS)";
        public static string GetSalesHomePageUrl => "https://www.linkedin.com/sales/index?trk=d_flagship3_nav&";
        public static bool IsSalesAccount(string Response)=>string.IsNullOrEmpty(Response)?false:(Response.Contains(GetSalesHomePageUrl)|| Response.Contains("https://www.linkedin.com/sales"));
        #endregion


        #region Sales API.
        public static string GetSalesUserIdentityAPI { get; set; } = "https://www.linkedin.com/sales-api/salesApiIdentity?q=findLicensesByCurrentMember&includeRecentlyInactiveDueToOverallocation=true";//GetAPI
        public static string GetSalesUserAuthenticationAPI { get; set; } = "https://www.linkedin.com/sales-api/salesApiAgnosticAuthentication";//PostAPI
        public static string GetSalesUsersSearchAPI(string SearchId, string SessionId,int PaginationCount) => $"https://www.linkedin.com/sales-api/salesApiLeadSearch?q=savedSearchId&start={PaginationCount}&count=25&savedSearchId={SearchId}&trackingParam=(sessionId:{SessionId})&decorationId=com.linkedin.sales.deco.desktop.searchv2.LeadSearchResult-13";
        public static string GetSalesAccountAuthenticationPostData(string Name,string ContractId,string SeatId)=>$"{{\"viewerDeviceType\":\"DESKTOP\",\"identity\":{{\"name\":\"{Name}\",\"agnosticIdentity\":{{\"com.linkedin.sales.authentication.SalesCapIdentity\":{{\"contractUrn\":\"urn:li:contract:{ContractId}\",\"seatUrn\":\"urn:li:seat:{SeatId}\"}}}}}}}}";
        public static string GetSalesCompanyDetailsAPI(string CompanyId) => $"https://www.linkedin.com/sales-api/salesApiCompanies/{CompanyId}?decoration=%28entityUrn%2Cname%2Caccount%28saved%2CnoteCount%2ClistCount%2CcrmStatus%2Cstarred%29%2CpictureInfo%2CcompanyPictureDisplayImage%2Cdescription%2Cindustry%2Clocation%2Cheadquarters%2Cwebsite%2CrevenueRange%2CcrmOpportunities%2CflagshipCompanyUrl%2CemployeeGrowthPercentages%2Cemployees*~fs_salesProfile%28entityUrn%2CfirstName%2ClastName%2CfullName%2CpictureInfo%2CprofilePictureDisplayImage%29%2Cspecialties%2Ctype%2CyearFounded%29";
        #endregion


        #region APIs.
        public static string ProfileUrlConstant { get; set; } = "https://www.linkedin.com/in/";
        public static string BroadCastMessageAPI => $"https://www.linkedin.com/voyager/api/messaging/conversations?action=create";
        public static string UserActivityApiConstant { get; set; } =
            "https://www.linkedin.com/voyager/api/feed/updates?q=memberFeed&moduleKey=member-recent-activity%3Aphone&includeLongTermHistory=false&numLikes=0&numComments=0&count=40&nc=" +
            Utils.GenerateNc() + "&start=";

        public static string UserPostApiConstantBeforPagination { get; set; } =
            "https://www.linkedin.com/voyager/api/feed/updates?count=20&includeLongTermHistory=true&moduleKey=member-shares%3Aphone&numComments=0&numLikes=0";
        public static string UserPostsApiConstantAfterPagination { get; set; } =
            "https://www.linkedin.com/voyager/api/identity/profileUpdatesV2?count=20&includeLongTermHistory=true&moduleKey=member-shares%3Aphone&q=memberShareFeed&profileUrn=urn%3Ali%3Afs_profile%3AACoAAA8BYqEBCGLg_vT_ca6mMEqkpp9nVffJ3hc";
        public static string UserPostsApiConstantPagination(string profileId)=>$"https://www.linkedin.com/voyager/api/identity/profileUpdatesV2?count=20&includeLongTermHistory=true&moduleKey=member-shares%3Aphone&numComments=0&numLikes=0&profileUrn=urn%3Ali%3Afsd_profile%3A{profileId}&q=memberShareFeed";
        public static string GetUserFeedLikeAPI(string ActivityId)=>$"https://www.linkedin.com/voyager/api/voyagerSocialDashReactions?threadUrn={ActivityId}";
        public static string GetSharedFeedAPI(string ActivityId)=>$"https://www.linkedin.com/feed/update/{ActivityId}";
        public static string GetJobDetailsAPI(string JobId)=>$"https://www.linkedin.com/voyager/api/jobs/jobPostings/{JobId}?decorationId=com.linkedin.voyager.deco.jobs.web.shared.WebFullJobPosting-65&topN=1&topNRequestedFlavors=List(TOP_APPLICANT,IN_NETWORK,COMPANY_RECRUIT,SCHOOL_RECRUIT,HIDDEN_GEM,ACTIVELY_HIRING_COMPANY)";
        public static string GetSalesUserProfileAPI(string profileId, string authToken) =>
            $"https://www.linkedin.com/sales-api/salesApiProfiles/(profileId:{profileId},authType:NAME_SEARCH,authToken:{authToken})?decoration=%28entityUrn%2CobjectUrn%2CpictureInfo%2CprofilePictureDisplayImage%2CfirstName%2ClastName%2CfullName%2Cheadline%2CmemberBadges%2Cdegree%2CprofileUnlockInfo%2Clocation%2ClistCount%2Cindustry%2CnumOfConnections%2CinmailRestriction%2CsavedLead%2CdefaultPosition%2CcontactInfo%2Csummary%2CcrmStatus%2CpendingInvitation%2Cunlocked%2CrelatedColleagueCompanyId%2CnumOfSharedConnections%2CshowTotalConnectionsPage%2CblockThirdPartyDataSharing%2CconnectedTime%2CnoteCount%2CflagshipProfileUrl%2CfullNamePronunciationAudio%2Cmemorialized%2CfullNamePronunciationAudio%2Cpositions*%2Ceducations*%29";
        public static string GetSalesCompanyScrapperAPI(string searchId, string sessionId, int paginationCount = 0)=>$"https://www.linkedin.com/sales-api/salesApiAccountSearch?q=savedSearch&start={paginationCount.ToString()}&count=25&savedSearchId={searchId}&trackingParam=(sessionId:{sessionId})&decorationId=com.linkedin.sales.deco.desktop.searchv2.AccountSearchResult-2";
        public static string GetCompanyScrapperAPI(string Keyword, string QueryParameter, string SearchId, string Origin = "SWITCH_SEARCH_VERTICAL", int Start = 0)=>
                $"https://www.linkedin.com/voyager/api/search/dash/clusters?decorationId=com.linkedin.voyager.dash.deco.search.SearchClusterCollection-180&origin={Origin}&q=all&query=(keywords:{Keyword},flagshipSearchIntent:SEARCH_SRP,queryParameters:(heroEntityKey:List({QueryParameter}),position:List(0),resultType:List(COMPANIES),searchId:List({SearchId})),includeFiltersInResponse:false)&start={Start}";
        public static string GetJobPosterDetailsAPI(string JobPostId) =>
            $"https://www.linkedin.com/voyager/api/jobs/jobPostings/{JobPostId?.Replace("urn:li:fsd_jobPosting:", "")}?decorationId=com.linkedin.voyager.deco.jobs.web.shared.WebFullJobPosting-65&topN=1&topNRequestedFlavors=List(TOP_APPLICANT,IN_NETWORK,COMPANY_RECRUIT,SCHOOL_RECRUIT,HIDDEN_GEM,ACTIVELY_HIRING_COMPANY)";
                //$"https://www.linkedin.com/voyager/api/voyagerJobsDashJobPostingDetailSections?decorationId=com.linkedin.voyager.dash.deco.jobs.FullJobPostingDetailSection-132&cardSectionType=HIRING_TEAM_CARD&jobPostingUrn={JobPostId.Replace(":", "%3A")}&q=cardSectionType";
        public static string GetCompanyDetailsAPI(string CompanyName)=>
                $"https://www.linkedin.com/voyager/api/graphql?includeWebMetadata=true&variables=(universalName:{CompanyName})&&queryId=voyagerOrganizationDashCompanies.02cf12708e5c68a988b585e2b8cde6c3";
        public static string GetPageDetailsById(string PageId) => $"https://www.linkedin.com/voyager/api/organization/companies/{PageId}?decorationId=com.linkedin.voyager.deco.organization.web.WebCompanyAdmin-26";
        public static string GetPostPreviewAPI(string PostUrl)=> $"https://www.linkedin.com/voyager/api/contentcreation/urlPreview/{Uri.EscapeDataString(PostUrl)}";
        public static string GetFeedCommentAPI { get; set; } = "https://www.linkedin.com/voyager/api/voyagerSocialDashNormComments?decorationId=com.linkedin.voyager.dash.deco.social.NormComment-43";
        public static string GetMediaShareAPI { get; set; } = "https://www.linkedin.com/voyager/api/contentcreation/normShares";
        public static string GetRepostAPI => "https://www.linkedin.com/voyager/api/voyagerFeedDashReposts?decorationId=com.linkedin.voyager.dash.deco.feed.repost.Repost-3";
        public static string GetMultipartMediaAPI(string shareId) => $"https://www.linkedin.com/voyager/api/graphql?includeWebMetadata=true&variables=(shareUrn:{WebUtility.UrlEncode(shareId)})&queryId=voyagerContentcreationDashShares.b802f9f485da9e03597508d5fc3d502b";
        public static string GetLDMediaUploadAPI { get; set; } = "https://www.linkedin.com/voyager/api/voyagerVideoDashMediaUploadMetadata?action=upload";
        public static string GetLDUserDetailsAPI(string PublicIdentifier) => $"https://www.linkedin.com/voyager/api/identity/dash/profiles?q=memberIdentity&memberIdentity={PublicIdentifier}&decorationId=com.linkedin.voyager.dash.deco.identity.profile.TopCardSupplementary-88";
        public static string GetOwnerProfileDetailsAPI { get; set; } = "https://www.linkedin.com/voyager/api/me";
        public static string GetSomeonesConnectionUsersAPI(string queryParameter, int paginationCount = 0) => $"https://www.linkedin.com/voyager/api/graphql?includeWebMetadata=true&variables=(start:{paginationCount.ToString()},origin:MEMBER_PROFILE_CANNED_SEARCH,query:(flagshipSearchIntent:SEARCH_SRP,queryParameters:List({queryParameter}),includeFiltersInResponse:false))&&queryId=voyagerSearchDashClusters.181547298141ca2c72182b748713641b";
        public static string UserActivityURL(string PublicIdentifier, string ActivityType) => $"https://www.linkedin.com/in/{PublicIdentifier}/details/{ActivityType}/";
        public static string GetUserSearchAPI(string queryValue, int paginationCount = 0) => $"https://www.linkedin.com/voyager/api/graphql?variables=(start:{paginationCount},origin:GLOBAL_SEARCH_HEADER,query:(keywords:{queryValue},flagshipSearchIntent:SEARCH_SRP,queryParameters:List((key:resultType,value:List(PEOPLE))),includeFiltersInResponse:false))&&queryId=voyagerSearchDashClusters.b0928897b71bd00a5a7291755dcd64f0";
        public static string GetPageSearchAPI(string queryValue, int paginationCount = 0) => $"https://www.linkedin.com/voyager/api/graphql?variables=(start:{paginationCount},origin:SWITCH_SEARCH_VERTICAL,query:(keywords:{queryValue},flagshipSearchIntent:SEARCH_SRP,queryParameters:List((key:resultType,value:List(COMPANIES))),includeFiltersInResponse:false))&&queryId=voyagerSearchDashClusters.b0928897b71bd00a5a7291755dcd64f0";
        public static string GetGroupJoinAPI => $"https://www.linkedin.com/voyager/api/voyagerGroupsDashGroupMemberships?action=updateMembership";
        public static string GetHasTagUrlPostAPI(string QueryValue, string PaginationToken = "", int PaginationCount = 0) =>
            string.IsNullOrEmpty(PaginationToken) ? $"https://www.linkedin.com/voyager/api/feed/interestUpdatesV2?count=20&keywords={QueryValue}&q=interestFeedByKeywords&sortOrder=RELEVANCE&start={PaginationCount}":
            $"https://www.linkedin.com/voyager/api/feed/interestUpdatesV2?count=6&keywords={QueryValue}&paginationToken={PaginationToken}&q=interestFeedByKeywords&sortOrder=RELEVANCE&start={PaginationCount}";
        public static string GetBlockUserAPI(string ProfileId, string CsrfToken) =>
            $"https://www.linkedin.com/psettings/member-blocking/block?memberId={ProfileId}&trk=block-profile&csrfToken=ajax%3A{CsrfToken}";

        public static string PostSearchAPI(int paginationCount, string queryValue) =>
            paginationCount > 0 ? $"https://www.linkedin.com/voyager/api/graphql?includeWebMetadata=true&variables=(query:(keywords:{queryValue},flagshipSearchIntent:SEARCH_SRP,queryParameters:List((key:resultType,value:List(CONTENT)))))&queryId=voyagerSearchDashFilterClusters.a316af94acc09f9e8762cfb5021dc130" 
            : $"https://www.linkedin.com/voyager/api/graphql?variables=(start:{paginationCount},origin:SWITCH_SEARCH_VERTICAL,query:(keywords:{queryValue},flagshipSearchIntent:SEARCH_SRP,queryParameters:List((key:resultType,value:List(CONTENT)))))&queryId=voyagerSearchDashClusters.0d1dfeebfce461654ef1279a11e52846";
        #region For Mobile Request

        public static string SearchTypeApiConstant { get; set; } =
            "https://www.linkedin.com/voyager/api/search/cluster?q=guided&searchId=";

        public static string CompanySearchTypeApiConstant { get; set; } =
            "https://www.linkedin.com/voyager/api/search/blended?origin=SWITCH_SEARCH_VERTICAL&queryContext=List(spellCorrectionEnabled-%3Etrue,kcardTypes-%3EPROFILE)&q=all&filters=List(resultType-%3ECOMPANIES)";

        public static string SearchTypeApiConstantV2 { get; set; } =
            "https://www.linkedin.com/voyager/api/search/blended?";
        #endregion
        #endregion


        #region Others.
        public const string CurrentNodeResponse = "CurrentNodeResponse";
        public static string MobileUserAgent { get; set; } =
            "{\"model\":\"samsung_SM-G925F\",\"appId\":\"com.linkedin.android\",\"osVersion\":\"4.4.2\",\"mpName\":\"voyager-android\",\"timezoneOffset\":5,\"mpVersion\":\"0.225.25\",\"clientMinorVersion\":106032,\"deviceType\":\"android\",\"isAdTrackingLimited\":false,\"dpi\":\"hdpi\",\"storeId\":\"us_googleplay\",\"clientVersion\":\"4.1.152\",\"deviceId\":\"fc61c979-3c11-4cc0-9537-b08a3dbfcc87\",\"osName\":\"Android OS\"}";

        public static string GroupProcessor { get; set; } = "GroupProcessor";
        public static string IsPresentSalesNavigatorCookies { get; set; } = "IsPresentSalesNavigatorCookies";
        public static string AlreadySentConnectionRequest { get; set; } = "AlreadySentConnectionRequest";
        public static string CurrentNode { get; set; } = "CurrentNode";
        public static string AlreadyLikedFeed { get; set; } = "Feed Already Liked";
        public static string Separator => "<:>";
        #endregion


        #region PostData.
        public static string GetCustomPostListPostData(string ShareUrl, string ActivityId, string GroupId = "",string Title="")=>
            !string.IsNullOrEmpty(ShareUrl)?ShareUrl.Contains("https://www.linkedin.com/feed/update") || ShareUrl.StartsWith("https://www.linkedin.com/posts/") ? $"{{\"visibleToConnectionsOnly\":false,\"externalAudienceProviders\":[],\"commentaryV2\":{{\"text\":\"{Title}\",\"attributes\":[]}}{GroupId},\"origin\":\"FEED\",\"allowedCommentersScope\":\"ALL\",\"postState\":\"PUBLISHED\",\"parentUrn\":\"{ActivityId}\"}}":
                $"{{\"visibleToConnectionsOnly\":false,\"externalAudienceProviders\":[],\"commentaryV2\":{{\"text\":\"{Title}\",\"attributes\":[]}}{GroupId},\"origin\":\"FEED\",\"allowedCommentersScope\":\"ALL\",\"postState\":\"PUBLISHED\",\"media\":[]}}"
                : string.Empty;
        
        public static string GetFeedCommentPostData(string CommentText,string ActivityId)=> $"{{\"commentary\":{{\"text\":\"{CommentText}\",\"attributesV2\":[],\"$type\":\"com.linkedin.voyager.dash.common.text.TextViewModel\"}},\"threadUrn\":\"{ActivityId}\"}}";
        public static string GetGroupJoinPostData(string GroupID, string ProfileId) => $"{{\"actionType\":\"SEND_REQUEST\",\"groupUrn\":\"urn:li:fsd_group:{GroupID}\",\"profileUrn\":\"urn:li:fsd_profile:{ProfileId}\"}}";
        #endregion


        #region UserConnectionRequestResponse
        public static string UserIsAlreadyYourConnection { get; set; } = "User is already your connection";
        public static string InvitationHasBeenSent { get; set; } = "An invitation has been sent";
        public static string YouHaveReachedWeeklyInvitationLimit { get; set; } = "You’ve reached the weekly invitation limit";
        public static string InvitationSentSuccessFully { get; set; } = "Invitation Sent Successfully";
        public static string UnableToConnectToUser(string Username="")
        {
            return $"Unable to connect to {Username}";
        }
        public static string YourInvitationToConnectWasNotSent { get; set; } = "Your invitation to connect was not sent.";
        #endregion


        #region Share Post Response.
        public static string UnableToSharePost { get; set; } = "Oops - something went wrong. We were unable to post at this time. Please try posting again.";
        public static string TooSmallFile { get; set; } = "Please choose a file which is larger than 75 Kb.";
        #endregion


        #region Web Request Header Accept Parameter.
        public static string AcceptApplicationOrJson { get; set; } = "application/json";
        public static string AcceptUrlEncoded { get; set; } = "application/x-www-form-urlencoded";
        public static string UserAgent => $"LIAuthLibrary:0.0.3 com.linkedin.android:4.1.256 LAVA_V23GB:android_6.0";
        public static string AcceptApplicationOrVndLinkedInMobileDedupedJson { get; set; } = "application/vnd.linkedin.mobile.deduped+json";
        public static string AcceptApplicationOrVndLinkedInMobileDedupedJson21 { get; set; } = "application/vnd.linkedin.normalized+json+2.1";
        #endregion
    }

    [Flags]
    public enum ActivityStatus
    {
        Failed = 2,
        Success = 4
    }
}