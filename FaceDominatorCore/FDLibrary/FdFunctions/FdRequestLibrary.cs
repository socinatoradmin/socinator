using DominatorHouseCore;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.FdQuery;
using DominatorHouseCore.Extensions;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.FacebookModels;
using DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Request;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDEnums;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdProcesses;
using FaceDominatorCore.FDModel.FilterModel;
using FaceDominatorCore.FDRequest;
using FaceDominatorCore.FDResponse;
using FaceDominatorCore.FDResponse.AccountsResponse;
using FaceDominatorCore.FDResponse.CommonResponse;
using FaceDominatorCore.FDResponse.EventsResponse;
using FaceDominatorCore.FDResponse.FriendsResponse;
using FaceDominatorCore.FDResponse.GroupsResponse;
using FaceDominatorCore.FDResponse.InviterResponse;
using FaceDominatorCore.FDResponse.LikeCommentsResponse;
using FaceDominatorCore.FDResponse.MessagesResponse;
using FaceDominatorCore.FDResponse.Publisher;
using FaceDominatorCore.FDResponse.ScrapersResponse;
using FaceDominatorCore.Interface;
using HtmlAgilityPack;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using ThreadUtils;

// ReSharper disable SuggestVarOrType_BuiltInTypes

namespace FaceDominatorCore.FDLibrary.FdFunctions
{

    public interface IFdRequestLibrary
    {
        string SessionId { get; set; }
        bool Login(DominatorAccountModel account);

        void SetCoockies(DominatorAccountModel account);

        Task<bool> LoginAsync(DominatorAccountModel account, CancellationToken token);

        Task<bool> LoginForPostScrapperAsync(DominatorAccountModel account, CancellationToken token);

        bool HasAlreadySentMessage(DominatorAccountModel accountModel, string friendId);
        void CheckGroupJoinStatus(DominatorAccountModel account, GroupDetails objGroupDetails, string url);
        string GetGroupMemberCount(DominatorAccountModel accountModel, string groupId);
        string GetGroupIdFromUrl(DominatorAccountModel account, string groupUrl);
        FanpageDetails GetPageDetailsFromUrl(DominatorAccountModel account, string fanpageUrl);
        EventDetailsResponseHandler GetEventDetailsFromUrl(DominatorAccountModel account, string eventUrl);

        IResponseHandler UpdateFriendsNewSync
            (DominatorAccountModel account, IResponseHandler responseHandler);


        FbUserIdResponseHandler GetFriendUserId(DominatorAccountModel account, string friendUrl);

        IResponseHandler GetDetailedInfoUserMobile
            (FacebookUser user, DominatorAccountModel account, bool isFullDeatailsRequired, bool isUseOriginalCookie);


        IResponseHandler GetDetailedInfoUserMobileScraper
            (FacebookUser user, DominatorAccountModel account, bool isFullDeatailsRequired, bool isUseOriginalCookie);

        IResponseHandler GetFriendOfFriend
            (DominatorAccountModel account, string keyword, IResponseHandler responseHandler);

        IResponseHandler GetGroupMembers(DominatorAccountModel account, string groupUrl,
            IResponseHandler responseHandler, GroupMemberCategory groupMemberCategory = GroupMemberCategory.AllMembers);

        FanpageLikersResponseHandler GetPageLikers
            (DominatorAccountModel account, string fanpageUrl, FanpageLikersResponseHandler responseHandler);

        FanpageLikersResponseHandler GetUserFollowers
                    (DominatorAccountModel account, string userProfilleUrl, IResponseHandler responseHandler);
        bool GetUserLikedPages(DominatorAccountModel accountModel,
            IResponseHandler mobileUserFanPageScraper, FacebookUser user, string pageId = "");
        IResponseHandler SearchPeopleFromKeyword
                   (DominatorAccountModel account, string keyword, IResponseHandler responseHandler);

        IResponseHandler SearchPeopleFromGraphSearch
                    (DominatorAccountModel account, string graphSearchUrl, IResponseHandler responseHandler, CancellationToken token);

        IResponseHandler SearchPeopleByLocation
                    (DominatorAccountModel account, string location, IResponseHandler responseHandler);

        string GetLocationCityId(DominatorAccountModel account, string location);

        IResponseHandler GetPostSharer(DominatorAccountModel account, string postUrl,
                    IResponseHandler responseHandler);

        IResponseHandler GetPostCommentor(DominatorAccountModel account, string postUrl,
                    IResponseHandler responseHandler, CancellationToken token);

        IResponseParameter GetCommentResponseforVideos(DominatorAccountModel account, string postId, string storyIdentifier);

        IResponseHandler GetPostLikers(DominatorAccountModel account, string postUrl,
                    IResponseHandler responseHandler);

        bool GetGroupJoiningStatus
                    (DominatorAccountModel accountModel, string groupUrl);

        void GetQuestionsAsked(DominatorAccountModel accountModel, CheckJoiningStatusResponseHandler groupResponseHandler);

        string SendFriendRequest(DominatorAccountModel account, string friendId);

        IResponseHandler GetFriendSuggestedByFacebook(DominatorAccountModel account
                    , IResponseHandler responseHandler);

        IResponseHandler GetIncomingFriendRequests
                    (DominatorAccountModel account, IResponseHandler responseHandler);

        IResponseHandler GetSentFriendRequestIdsNew
                  (DominatorAccountModel account, IResponseHandler responseHandler);

        bool CancelSentRequest(DominatorAccountModel account, string friendId);

        CancelSentRequestResponseHandler Unfriend(DominatorAccountModel account, ref FacebookUser facebookUser);

        bool CancelIncomingRequest(DominatorAccountModel account, string friendId);

        bool AcceptFriendRequest(DominatorAccountModel account, string friendId);

        List<string> GetAlreadyGroupJoinedFriendsList(DominatorAccountModel account, string group_id);

        IResponseHandler GetFanpageDetailsFromKeyword(DominatorAccountModel account, string keyword,
                        bool isVerifiedFilter, bool isLikedByFriends, FanpageCategory objFanpageCategory,
                        IResponseHandler responseHandler);

        IResponseHandler GetPlaceDetailsFromKeyword(DominatorAccountModel account, string keyword,
                       FdPlaceFilterModel fdPlaceFilterModel,
                       IResponseHandler responseHandler);

        IResponseHandler GetFanpageDetailsFromGraphSearch(DominatorAccountModel account, string graphSearchUrl,
                    bool isVerifiedFilter, bool isLikedByFriends, FanpageCategory objFanpageCategory,
                    IResponseHandler responseHandler);


        FanpageScraperResponseHandler GetFanpageDetails
                    (DominatorAccountModel account, FanpageDetails objFanpageDetails, bool isLocationPage = false);


        PostScraperResponseHandler GetPostDetails
                    (DominatorAccountModel account, FacebookPostDetails objFacebookPostDetails, bool isWatchParty = false);

        PostScraperResponseHandler GetPostDetailNew
              (DominatorAccountModel account, FacebookPostDetails objFacebookPostDetails, bool isWatchParty = false);

        string GetVideoDetails(DominatorAccountModel account, string composerId,
                    ref FacebookPostDetails objFacebookPostDetails, string watchPartyUrl = "");

        PostScraperResponseHandler GetPostDetailsNew
                    (DominatorAccountModel account, FacebookPostDetails objFacebookPostDetails);

        PostScraperResponseHandler GetPostDetailsNewDownloadMedia
                    (DominatorAccountModel account, FacebookPostDetails objFacebookPostDetails);

        void GetFullSizeImages(DominatorAccountModel account, ref FacebookPostDetails objFacebookPostDetails);

        IResponseHandler ScrapGroups
                    (DominatorAccountModel account, string keyword, GroupMemberShip objGroupMemberShip,
                    GroupType objGroupType, IResponseHandler responseHandler, string option);

        IResponseHandler GetPostListFromNewsFeed
                    (DominatorAccountModel account, IResponseHandler responseHandler, string newsFeedUrl = null);

        IResponseHandler GetPostListFromFanpages
                    (DominatorAccountModel account, string fanpageUrl, IResponseHandler responseHandler);

        IResponseHandler GetPostListFromFanpagesNew
                    (DominatorAccountModel account, IResponseHandler responseHandler, string fanpageUrl);

        IResponseHandler GetPostListFromGroups
                    (DominatorAccountModel account, IResponseHandler responseHandler, string groupUrl);

        IResponseHandler GetPostListFromGroupsNew
                    (DominatorAccountModel account, IResponseHandler responseHandler, string groupUrl);

        IResponseHandler GetPostListFromTimeline
                    (DominatorAccountModel account, IResponseHandler responseHandler, string accountUrl = null);

        bool GetVerifyAfterFanPageLiker(DominatorAccountModel accountModel, string url, string postId, string comment, ref string pageId);

        IResponseHandler GetPostListFromFriendTimelineNew
                    (DominatorAccountModel account, IResponseHandler responseHandler, string friendId);

        bool LikeUnlikePost
                    (DominatorAccountModel account, string postId, ReactionType objReactionType, string pageId = "");

        LikeFanpageResponseHandler LikeFanpage(DominatorAccountModel account, string fanpageId, CancellationToken token);

        CommentOnPostResponseHandler CommentOnPost
            (DominatorAccountModel account, string postId, string commentText, string pageId = "");

        FdSendTextMessageResponseHandler SendTextMessage
            (DominatorAccountModel account, string friendId, string commentText, bool isMessageToPage = false);

        FdSendTextMessageResponseHandler SendTextMessageWithLinkPreview
            (DominatorAccountModel account, string friendId, string commentText,
            bool isMessageToPage = false);

        Task<IResponseHandler> SendTextMessageAsync(DominatorAccountModel account, SenderDetails senderDetails, string commentText
                            , CancellationToken cancellation);
        ScrapedLinkDetails GetScrapedLinkDetails(string response);

        string UpdateLinkAndGetShareId(DominatorAccountModel account, string link, ref string shareType,
                    ref ScrapedLinkDetails objDetails);

        bool SendImageWithText(DominatorAccountModel account, string friendId, List<string> imagePathList, string timeStamp = null);

        Task<IResponseHandler> SendImageWithTextAsync(DominatorAccountModel account, SenderDetails senderDetails, List<string> imagePathList, string timeStamp = null);

        bool UnjoinGroup(DominatorAccountModel account, string groupId);

        GroupInviterResponseHandler SendGroupInvittationTofriends
                   (DominatorAccountModel account, string groupId, FacebookUser objFacebookUser, string message);

        bool SendPageInvittationTofriends
                    (DominatorAccountModel account, string pageId, string inviteNote, FacebookUser objFacebookUser, bool isSendInMessage);

        bool SendPageInvittationTofriendsWithoutNote
            (DominatorAccountModel account, string pageId, FacebookUser objFacebookUser);

        bool SendPageInvittationToPageLikers
            (DominatorAccountModel account, string pageId, FacebookUser objFacebookUser, string postUrl);

        void GetLangugae(DominatorAccountModel account);

        void ChangeLanguage(DominatorAccountModel account, string languageCode);

        void ChangeToClassicUIMode(DominatorAccountModel account);

        bool SendEventInvittationTofriends
                    (DominatorAccountModel account, string eventId, FacebookUser objFacebookUser, string message);

        bool InviteAsPersonalMessage
                  (DominatorAccountModel account, string entityId, FacebookUser objFacebookUser, string message);

        IResponseHandler GetMessageRequestDetails
                    (DominatorAccountModel account, IResponseHandler responseHandler, MessageType messageType);

        PublisherResponseHandler PostToPages(DominatorAccountModel account,
                    string pageUrl, PublisherPostlistModel postDetails,
                    CancellationTokenSource campaignCancellationToken, GeneralModel generalsettingsModel,
                    DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting.FacebookModel advanceSettingsModel);

        PublisherResponseHandler PostToOwnWall
                    (DominatorAccountModel account, PublisherPostlistModel postDetails,
                    CancellationTokenSource campaignCancellationToken, GeneralModel generalSettingsModel,
                    FacebookModel advanceSettingsModel);

        PublisherResponseHandler PostToGroups(DominatorAccountModel account,
                     string groupUrl, PublisherPostlistModel postDetails,
                    CancellationTokenSource campaignCancellationToken, GeneralModel generalSettingsModel,
                    DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting.FacebookModel advanceSettingsModel);

        PublisherResponseHandler PostToFriends(DominatorAccountModel account,
                     string friendUrl, PublisherPostlistModel postDetails,
                     CancellationTokenSource campaignCancellationToken, GeneralModel generalSettingsModel,
                     DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting.FacebookModel advanceSettingsModel);

        PublisherResponseHandler SellPostToGroups(DominatorAccountModel account,
                    string groupUrl, PublisherPostlistModel postDetails,
                    CancellationTokenSource campaignCancellationToken, GeneralModel generalSettingsModel,
                    DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting.FacebookModel advanceSettingsModel);

        PublisherResponseHandler ExtractPublisherParameterWall(DominatorAccountModel account, ref PublisherParameter objPublisherParameter,
                    CancellationTokenSource campaignCancellationToken);


        PublisherResponseHandler ShareToPages(DominatorAccountModel account,
                    string pageUrl, PublisherPostlistModel postDetails,
                    CancellationTokenSource campaignCancellationToken, GeneralModel generalSettingsModel,
                    DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting.FacebookModel advanceSettingsModel);

        PublisherResponseHandler ShareToEvents(DominatorAccountModel account,
                    string eventUrl, PublisherPostlistModel postDetails,
                    CancellationTokenSource campaignCancellationToken, GeneralModel generalSettingsModel,
                    DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting.FacebookModel advanceSettingsModel);

        PublisherResponseHandler ShareToFriendProfiles(DominatorAccountModel account,
                    string friendUrl, PublisherPostlistModel postDetails,
                    CancellationTokenSource campaignCancellationToken, GeneralModel generalSettingsModel,
                    DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting.FacebookModel advanceSettingsModel);

        PublisherResponseHandler ShareToOwnWall(DominatorAccountModel account, PublisherPostlistModel postDetails,
                    CancellationTokenSource campaignCancellationToken, GeneralModel generalSettingsModel,
                    DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting.FacebookModel advanceSettingsModel);


        PublisherResponseHandler ShareToGroups(DominatorAccountModel account,
                    string groupUrl, PublisherPostlistModel postDetails,
                    CancellationTokenSource campaignCancellationToken, GeneralModel generalSettingsModel,
                    DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting.FacebookModel advanceSettingsModel);

        PublisherResponseHandler ShareToFriendAsPrivateMessage(DominatorAccountModel account, string friendUrl, PublisherPostlistModel postDetails, CancellationTokenSource campaignCancellationToken, DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting.FacebookModel advanceSettingsModel);


        //IResponseHandler GetRecentFriendMessageDetails
        //           (DominatorAccountModel account, IResponseHandler responseHandler, CancellationToken token);

        IResponseHandler GetRecentMessageDetails
                        (DominatorAccountModel account, IResponseHandler responseHandler, LiveChatModel liveChatModel, CancellationToken token);


        IResponseHandler GetInterestedGuestsForEvents(DominatorAccountModel account,
                  IResponseHandler responseHandler, string eventUrl, EventGuestType objEventGuestType);

        List<FacebookUser> GetAllMutualFriends(DominatorAccountModel account, string friendId);

        IResponseHandler LikeComments(DominatorAccountModel accountModel, FdPostCommentDetails commentDetails, ReactionType objReactionType, FanpageDetails fanpageDetails = null);



        bool ChangeActor(DominatorAccountModel account, string postId, string pageId);

        bool ChangeActorForPost(DominatorAccountModel account, string postId, string pageId,
                    string actorId, string composerId);

        IResponseHandler GetPostListFromAlbums
                      (DominatorAccountModel account, IResponseHandler responseHandler, string albumUrl = null);


        IResponseHandler GetPostListFromKeyWords
                      (DominatorAccountModel account, IResponseHandler responseHandler, string keyword);
        IResponseHandler GetCommentReplies(DominatorAccountModel accountModel, FdPostCommentDetails commentDetails);

        //bool EditName(DominatorAccountModel account, string fullName);

        //void EditDateOfBirth(DominatorAccountModel account, DateTime dob);


        //bool EditGender(DominatorAccountModel account, int genderValue);


        //bool EditBio(DominatorAccountModel account, string bio);

        //bool UploadProfilePic(DominatorAccountModel account, string imagePath);

        FdErrorDetails InviteFriendOrPage(DominatorAccountModel account, string recipientId, ref FacebookPostDetails watchPartyDetails,
                    bool isInviteInMessaenger = true);

        IResponseHandler GetUsersBirtdayResponse
                    (DominatorAccountModel account, IResponseHandler responseHandler);

        IResponseHandler ReplyOnPost(DominatorAccountModel account, string postId, string commentText
            , string commentId, string pageId = "");

        //   string GetPostEntIdentifier(DominatorAccountModel account, string postUrl);


        bool DeletePostFromGroup(DominatorAccountModel account, string postId, string groupUrl, string ftEntIdentifier);

        bool DeletePostFromTimeline(DominatorAccountModel account, string postId, PostDeletionModel postDeletionModel, string ftEntIdentifier);

        bool StopCommenting(DominatorAccountModel account, string postId, string ftEntIdentifier);

        bool StopNotification(DominatorAccountModel account, string postId, string ftEntIdentifier);

        string GetCurrencyDetails(DominatorAccountModel account, string groupId, string composerId);

        bool StopNotificationGroups(DominatorAccountModel account, string postId, string groupId, string ftEntIdentifier);

        bool HidePostFromOwnPage(DominatorAccountModel account, string postId, string pageUrl);

        string GetPageIdFromUrl(DominatorAccountModel account, string fanpageUrl);

        Task<IResponseHandler> ScrapGroupsNewAsync(DominatorAccountModel account,
            GroupScraperResponseHandlerNew responseHandler, CancellationToken token);

        Task<GroupScraperResponseHandlerNew> ScrapGroupAsync(DominatorAccountModel account,
            GroupScraperResponseHandlerNew responseHandler, string groupType, CancellationToken token);

        Task SaveDetailsInDb(DominatorAccountModel account, FacebookAdsDetails currentAd);

        //    Task<AdDetailsResponseHandler> GetAdCountryDetails(DominatorAccountModel account);

        //  Task<AdReactionDetailsResponseHandler> GetAdReactionDetails(DominatorAccountModel account, FacebookAdsDetails adDetails);


        //   Task<bool> UpdateLcsDetails(DominatorAccountModel account, FacebookAdsDetails adDetails);

        Task<bool> CheckDuplicatesFromDb(DominatorAccountModel account, FacebookAdsDetails currentAd);

        Task GetNavigationUrl(DominatorAccountModel account, string postResponse,
          FacebookAdsDetails postDetails, string postUrl);

        Task ScrapNewPostsNavigationUrl(DominatorAccountModel account, string adDetails,
                    FacebookAdsDetails postDetails, string postUrl, string postResponse);

        Task ViewersDetailsParser(DominatorAccountModel account, FacebookAdsDetails currentAd);

        Task<GetFanpageFullDetailsResponseHandler> GetPageDetails(DominatorAccountModel account, FacebookAdsDetails currentAd);

        Task ScrapeComments(DominatorAccountModel account, FacebookAdsDetails currentAd);

        Task<FdLoginResponseHandler> IsLoggedIn
                    ([NotNull] DominatorAccountModel accountModel, CancellationToken token);

        Task<FdLoginResponseHandler> IsLoggedIn
                    ([NotNull] DominatorAccountModel accountModel, CancellationToken token, IResponseParameter loginResponse);

        Task<FdUserInfoResponseHandlerMobile> GetDetailedInfoUserMobileAsync
                     (FacebookUser user, DominatorAccountModel account, bool isFullDeatailsRequired, bool isUseOriginalCookie,
                        CancellationToken token);

        Task<IResponseHandler> UpdateFriendsNew
              (DominatorAccountModel account, IResponseHandler responseHandler, CancellationToken token, bool isLastPage);

        Task<IResponseHandler> GetUnfriendPaginationAsync(DominatorAccountModel account, IResponseHandler responseHandler,
                    CancellationToken token, bool isLastPage);

        Task<IResponseHandler> UpdateFriendsFromPage
                   (DominatorAccountModel account, FriendsUpdateResponseHandler responseHandler, CancellationToken token, List<string> lstPageId);

        Task<FbUserIdResponseHandler> GetFriendUserIdAsync(DominatorAccountModel account, string friendUrl);

        Task<FdUserInfoResponseHandlerMobile> GetDetailedInfoUserMobileScraperAsync
                    (FacebookUser user, DominatorAccountModel account, bool isFullDeatailsRequired,
                    bool isUseOriginalCookie, CancellationToken token, string timeSpan = "");

        Task<SearchOwnPageResponseHandler> UpdateOwnPagesAsync
                    (DominatorAccountModel account,
                    SearchOwnPageResponseHandler responseHandler, CancellationToken token);

        Task<IResponseHandler> UpdateLikedPagesAsync(DominatorAccountModel account,
            IResponseHandler responseHandler, CancellationToken token, bool isUpdate = false);

        Task<PostDetailsScraperResponseHandlerNew> GetPostDetailWithDestinationUrl
               (DominatorAccountModel account, FacebookAdsDetails objFacebookPostDetails, string composerId);

        Task GetVideoDetails(DominatorAccountModel account, string composerId,
                       FacebookAdsDetails objFacebookPostDetails);

        Task<string> RedirectPath(string url);

        Task<ScrapNewPostListFromNewsFeedResponseHandler> GetPostListFromNewsFeedAsync
                     (DominatorAccountModel account, ScrapNewPostListFromNewsFeedResponseHandler responseHandler);

        Task<Dictionary<IpLocationDetails, string>> GetIpDetails(DominatorAccountModel account, bool isLocalIp = false);


        Task ScrapOwnProfileInfoAsync(DominatorAccountModel account);

        //  Task UpdateProfileInfo(DominatorAccountModel account , FacebookUser objUser);



        //bool LikeUnlikePostAsPage(DominatorAccountModel account, string postId, ReactionType objReactionType, string pageId);

        //bool LikeCommentsAsPage(DominatorAccountModel accountModel, FdPostCommentDetails commentDetails,
        //            ReactionType objReactionType, FanpageDetails pageDetails);

        // CommentOnPostResponseHandler CommentOnPostAsPage(DominatorAccountModel account, string postId, string commentText, string pageId);

        //ScrapPostListFromNewsFeedResponseHandler GetPostListFromNewsFeed
        //            (DominatorAccountModel account, ScrapPostListFromNewsFeedResponseHandler responseHandler);


        //SearchFanpageDetailsResponseHandler UpdateLikedPagesSync
        //    (DominatorAccountModel account,
        //    SearchFanpageDetailsResponseHandler responseHandler, CancellationToken token);

        bool DeleteStoryPost(DominatorAccountModel account, string postId, string storyId);

        AccountMarketplaceDetailsHandler GetAccountMarketPlaceDetails(DominatorAccountModel account);

        AccountMarketplaceDetailsHandler ChangeMarketplaceLocation(DominatorAccountModel account,
            string location, MarketplaceFilterModel marketplaceFilterModel);

        MarketplaceScraperResponseHandler GetProductListFromMarketplace(DominatorAccountModel account
            , string keyword, AccountMarketplaceDetailsHandler accountMarketPlaceDetails, MarketplaceFilterModel filterModel,
            MarketplaceScraperResponseHandler responseHandler);

        IResponseHandler GetFanpageDetailsLikedByFriend(DominatorAccountModel account, string friendUrl,
                bool isVerifiedFilter, bool isLikedByFriends, FanpageCategory objFanpageCategory,
                IResponseHandler responseHandler);

        EventCreaterResponseHandler EventCreater(DominatorAccountModel account, EventCreaterManagerModel eventCreaterManagerModel);

        bool IsGroupAdmin(DominatorAccountModel accountModel, string groupUrl);

        GetGroupMembersResponseHandler GetAllGroupMembers(DominatorAccountModel accountModel, string groupId);

        MakeAdminResponseHandler MakeGroupAdmin(DominatorAccountModel accountModel, string groupId, string userId);

        //      IResponseHandler UpdateFriendsFromPageSync
        //(DominatorAccountModel account, FriendsUpdateResponseHandler responseHandler, CancellationToken token
        // , List<string> lstPageId);

        //WebPostCommentLikerResponseHandler GetPostCommentorForWebPage
        //     (DominatorAccountModel account, string postUrl, WebPostCommentLikerResponseHandler webPageCommentLikerResponseHandler);

        //IncommingPageMessageResponseHandler GetMessageDetailsFromPage
        //          (DominatorAccountModel account, ref string timeStampPrecise, MessageType messageType, string pageId);

        //bool LikeWebPostComment(DominatorAccountModel account, string commentId, ReactionType reactionType);

        //bool CommnentOnWebPostComment(DominatorAccountModel account, FdPostCommentDetails fdPostCommentDetails, string comment);

        //    Task<GroupScraperResponseHandler> ScrapGroupsAsync
        //              (DominatorAccountModel account,
        //            GroupScraperResponseHandler responseHandler, CancellationToken token);

        //string getLocationDetailsFromKeyword(string location);
        //Task<GroupDetailsResponseHandler> GetGroupDetails(DominatorAccountModel account, CancellationToken token);
    }

    public class FdRequestLibrary : IFdRequestLibrary
    {

        private readonly IFdHttpHelper _httpHelper;
        private readonly IDelayService _delayService;

        public IFdHttpHelper GetFdHttpHelper() => _httpHelper;

        public FdRequestLibrary(IFdHttpHelper httpHelper, IDelayService delayService)
        {
            _httpHelper = httpHelper;
            _delayService = delayService;
        }

        private readonly List<Tuple<string, string, DateTime>> _dictAccountComposer =
            new List<Tuple<string, string, DateTime>>();

        public string SessionId { get; set; } = string.Empty;

        /// <summary>
        /// Use this method for login
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public bool Login(DominatorAccountModel account)
        {
            //requestParameter value should be set

            return LoginAsync(account, account.Token).Result;
        }

        public void SetCoockies(DominatorAccountModel account)
        {
            var request = _httpHelper.GetRequestParameter();
            request.Cookies = account.BrowserCookies;
            request.ContentType = FdConstants.ContentType;
            _httpHelper.SetRequestParameter(request);
        }


        public async Task<bool> LoginAsync(DominatorAccountModel account, CancellationToken token)
        {
            try
            {
                var objFdRequestParameter = new FdRequestParameter();
                var objFdFunctions = new FdFunctions();
                // check once if the request parameter value set or not
                if (_httpHelper?.GetRequestParameter()?.Cookies == null)
                    _httpHelper.SetRequestParameter(objFdRequestParameter);

                var requestParameter = _httpHelper.GetRequestParameter();

                bool isCookieChanged = false;

                if (requestParameter.Cookies != null)
                    requestParameter.Cookies = objFdRequestParameter.RefreshCookies(requestParameter.Cookies, ref isCookieChanged);

                if (isCookieChanged)
                {
                    _httpHelper.SetRequestParameter(requestParameter);
                    account.Cookies = requestParameter.Cookies;
                    objFdFunctions.ChangeAccountCookies(account, _httpHelper);
                }

                var objFdLoginResponseHandler = await IsLoggedIn(account, token);


                if (!objFdLoginResponseHandler.LoginStatus && objFdLoginResponseHandler.LoginParameters != null)
                {

                    objFdRequestParameter.UrlParameters.Add("login_attempt", "1");
                    objFdRequestParameter.UrlParameters.Add("lwv", "111");

                    var url = objFdRequestParameter.GenerateUrl(FdConstants.FbLoginPhpUrl);

                    var objFdJsonElement = objFdFunctions.GetJsonElementsForLogin(account, objFdLoginResponseHandler);

                    objFdRequestParameter.FdPostElements = objFdJsonElement;

                    var postdata = objFdRequestParameter.GetPostDataFromJson();

                    var request = _httpHelper.GetRequestParameter();

                    request.ContentType = FdConstants.ContentType;

                    if (request.Cookies == null)
                        request.Cookies = new CookieCollection();

                    CookieCollection ck = new CookieCollection()
                    {
                        new Cookie() { Name = "_js_datr",
                            Value = objFdLoginResponseHandler.LoginParameters.RegCookieValue,
                            Domain = "facebook.com" },
                        new Cookie() { Name = "_js_reg_fb_ref",
                            Value = "https%3A%2F%2Fwww.facebook.com%2F",
                            Domain = "facebook.com" },
                        new Cookie() { Name = "_js_reg_fb_gate",
                            Value = "https%3A%2F%2Fwww.facebook.com%2F",
                            Domain = "facebook.com" },

                    };

                    request.Cookies.Add(ck);

                    _httpHelper.SetRequestParameter(request);

                    var loginResponse = await _httpHelper.PostRequestAsync(url, postdata, token);

                    objFdLoginResponseHandler = await IsLoggedIn(account, token, loginResponse);

                    account.IsUserLoggedIn = objFdLoginResponseHandler.LoginStatus;

                }
                else if (account.IsUserLoggedIn)
                {
                    account.IsUserLoggedIn = true;

                    try
                    {
                        account.CookieHelperList.RemoveWhere(x =>
                            x.Name == "_js_reg_fb_ref" || x.Name == "_js_reg_fb_gate");
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                    objFdFunctions.ChangeSessionId(account, objFdLoginResponseHandler);
                }

                if (!objFdLoginResponseHandler.LoginStatus)
                {
                    GlobusLogHelper.log.Info(Log.LoginFailed, account.AccountBaseModel.AccountNetwork, account.AccountBaseModel.UserName, objFdLoginResponseHandler.FbErrorDetails.Description);
                    objFdFunctions.ChangeAccountStatus(account, objFdLoginResponseHandler, _httpHelper);
                }
                else
                {
                    //ChangeToClassicUIMode(objFdLoginResponseHandler.DominatorAccountModel);

                    account.IsUserLoggedIn = true;

                    try
                    {
                        account.CookieHelperList.RemoveWhere(x =>
                            x.Name == "_js_reg_fb_ref" || x.Name == "_js_reg_fb_gate");
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                    objFdFunctions.ChangeSessionId(account, objFdLoginResponseHandler);

                    account.SessionId = objFdLoginResponseHandler.FbDtsg;

                    SessionId = objFdLoginResponseHandler.FbDtsg;

                    objFdFunctions.ChangeAccountStatus(account, objFdLoginResponseHandler, _httpHelper);


                    #region Commented
                    //if (!FdConstants.RunningAdAccounts.ContainsKey(account.AccountBaseModel.UserName))
                    //{
                    //    try
                    //    {
                    //        FdConstants.RunningAdAccounts.Add(account.AccountBaseModel.UserName, account.AccountBaseModel.UserName);
                    //        Instance.SendAsync(new FdPostDetailsScraperProcess(account));
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        ex.DebugLog();
                    //    }
                    //} 
                    #endregion
                }

                #region Commented
                //GetPostListFromKeyWords(account, null, "shahrukh khan");


                //used for testing languages
                //IResponseParameter homePageResponse = await _httpHelper.GetRequestAsync($"{FdConstants.FbHomeUrl}citymobiltaxi/posts/2247861501996198", account.Token);
                //new GetFanpageFullDetailsResponseHandler(homePageResponse, true, new FacebookAdsDetails());

                #endregion

                return account.IsUserLoggedIn;


            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (AggregateException ae)
            {
                ae.HandleOperationCancellation();
            }
            catch (ArgumentNullException ex)
            {
                ex.ErrorLog($"{ex.GetType().Name} : A null parameter passed to login method.");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return false;
            }
            return true;
        }

        public async Task<bool> LoginForPostScrapperAsync(DominatorAccountModel account, CancellationToken token)
        {
            try
            {
                var isBrowserCookies = false;
                var objFdRequestParameter = new FdRequestParameter();
                var objFdFunctions = new FdFunctions();

                if (_httpHelper?.GetRequestParameter()?.Cookies == null)
                    _httpHelper.SetRequestParameter(objFdRequestParameter);

                var requestParameter = _httpHelper.GetRequestParameter();

                bool isCookieChanged = false;

                if (requestParameter.Cookies != null)
                    requestParameter.Cookies = objFdRequestParameter.RefreshCookies(requestParameter.Cookies, ref isCookieChanged);

                if (isCookieChanged)
                {
                    _httpHelper.SetRequestParameter(requestParameter);
                    account.Cookies = requestParameter.Cookies;
                    objFdFunctions.ChangeAccountCookies(account, _httpHelper);
                }

                if (account.Cookies.Count == 0 && account.BrowserCookies != null)
                {
                    requestParameter.Cookies = account.BrowserCookies;
                    _httpHelper.SetRequestParameter(requestParameter);
                    isBrowserCookies = true;
                }

                var objFdLoginResponseHandler = await IsLoggedIn(account, token);

                if (!objFdLoginResponseHandler.LoginStatus && !isBrowserCookies)
                {
                    requestParameter.Cookies = account.BrowserCookies;
                    _httpHelper.SetRequestParameter(requestParameter);
                    objFdLoginResponseHandler = await IsLoggedIn(account, token);
                }

                if (!objFdLoginResponseHandler.LoginStatus && objFdLoginResponseHandler.LoginParameters != null)
                {

                    //if (account.IsRunProcessThroughBrowser && account.BrowserCookies != null)
                    //{
                    //    requestParameter.Cookies = account.BrowserCookies;
                    //    _httpHelper.SetRequestParameter(requestParameter);
                    //    objFdLoginResponseHandler = await IsLoggedIn(account, token);
                    //}


                    //objFdRequestParameter.UrlParameters.Add("login_attempt", "1");
                    //objFdRequestParameter.UrlParameters.Add("lwv", "111");

                    //var url = objFdRequestParameter.GenerateUrl(FdConstants.FbLoginPhpUrl);

                    //var objFdJsonElement = objFdFunctions.GetJsonElementsForLogin(account, objFdLoginResponseHandler);

                    //objFdRequestParameter.FdPostElements = objFdJsonElement;

                    //var postdata = objFdRequestParameter.GetPostDataFromJson();

                    //var request = _httpHelper.GetRequestParameter();

                    //request.ContentType = FdConstants.ContentType;

                    //if (request.Cookies == null)
                    //    request.Cookies = new CookieCollection();

                    //CookieCollection ck = new CookieCollection()
                    //{
                    //    new Cookie() { Name = "_js_datr",
                    //        Value = objFdLoginResponseHandler.LoginParameters.RegCookieValue,
                    //        Domain = "facebook.com" },
                    //    new Cookie() { Name = "_js_reg_fb_ref",
                    //        Value = "https%3A%2F%2Fwww.facebook.com%2F",
                    //        Domain = "facebook.com" },
                    //    new Cookie() { Name = "_js_reg_fb_gate",
                    //        Value = "https%3A%2F%2Fwww.facebook.com%2F",
                    //        Domain = "facebook.com" },

                    //};

                    //request.Cookies.Add(ck);



                    //var loginResponse = await _httpHelper.PostRequestAsync(url, postdata, token);

                    //objFdLoginResponseHandler = await IsLoggedIn(account, token, loginResponse);


                    account.IsUserLoggedIn = objFdLoginResponseHandler.LoginStatus;

                }
                else if (account.IsUserLoggedIn)
                {
                    account.IsUserLoggedIn = true;

                    account.AccountBaseModel.UserId = objFdLoginResponseHandler.UserId;
                    account.SessionId = objFdLoginResponseHandler.FbDtsg;

                    try
                    {
                        account.CookieHelperList.RemoveWhere(x =>
                            x.Name == "_js_reg_fb_ref" || x.Name == "_js_reg_fb_gate");
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                }

                if (objFdLoginResponseHandler.LoginStatus)
                {
                    account.IsUserLoggedIn = true;

                    account.AccountBaseModel.UserId = objFdLoginResponseHandler.UserId;

                    account.SessionId = objFdLoginResponseHandler.FbDtsg;

                    try
                    {
                        account.CookieHelperList.RemoveWhere(x =>
                            x.Name == "_js_reg_fb_ref" || x.Name == "_js_reg_fb_gate");
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                    //objFdFunctions.ChangeSessionId(account, objFdLoginResponseHandler);

                    SessionId = objFdLoginResponseHandler.FbDtsg;

                }

                return account.IsUserLoggedIn;

            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (ArgumentNullException)
            {
                Console.WriteLine();
            }
            catch (Exception)
            {
                Console.WriteLine();
                return false;
            }
            return true;
        }

        public bool HasAlreadySentMessage(DominatorAccountModel accountModel, string friendId)
        {
            var url = FdConstants.CheckLastMessageUrl;

            FdRequestParameter objFdRequestParameter = new FdRequestParameter();

            var queries = FdConstants.GetLastMessageQueriesForUser(friendId, "20000");

            objFdRequestParameter.PostDataParameters.Add("batch_name", "MessengerGraphQLThreadFetcher");
            objFdRequestParameter.PostDataParameters.Add("queries", queries);
            objFdRequestParameter = CommonPostDataParameters(accountModel, objFdRequestParameter);

            var parameter = _httpHelper.GetRequestParameter();
            parameter.ContentType = FdConstants.ContentType;
            parameter.AddHeader("X-MSGR-Region", "FRC");
            _httpHelper.SetRequestParameter(parameter);

            var postData = objFdRequestParameter.GetPostDataFromParameters();

            var messageResponse = _httpHelper.PostRequest(url, postData);

            parameter = _httpHelper.GetRequestParameter();
            if (parameter.Headers != null)
                parameter.Headers.Remove("X-MSGR-Region");
            _httpHelper.SetRequestParameter(parameter);

            return new GetLastMessageResponseHandler(messageResponse, accountModel.AccountBaseModel.UserId)
                .Status;

        }

        public async Task SaveDetailsInDb(DominatorAccountModel account, FacebookAdsDetails currentAd)
        {
            try
            {
                //      var url = FdConstants.SaveAdsDataDev + "adsdata";

                var url = FdConstants.SaveAdsUrlMain + "adsdata";

                if (string.IsNullOrEmpty(currentAd.AdType) || currentAd.AdType == "FEED")
                {
                    currentAd.AdType = "FEED";
                    currentAd.PostUrl = FdConstants.FbHomeUrl + currentAd.Id;
                }
                else
                {
                    currentAd.PostedDateTime = DateTime.Now;
                    currentAd.PostUrl = string.Empty;
                }

                var objRequestParameter = _httpHelper.GetRequestParameter();

                objRequestParameter.AddHeader("Host", "api.poweradspy.com");


                objRequestParameter.ContentType = FdConstants.ContentTypeJson;

                _httpHelper.SetRequestParameter(objRequestParameter);

                if (string.IsNullOrEmpty(currentAd.UpperAge) || string.IsNullOrEmpty(currentAd.UpperAge))
                {
                    currentAd.LowerAge = "20";
                    currentAd.UpperAge = "65";
                }

                var adId = currentAd.AdType == "FEED" ? currentAd.Id : currentAd.AdId;

                FdAdsScraperJsonElement objlement = new FdAdsScraperJsonElement()
                {
                    Type = currentAd.AdMediaType.ToString(),
                    Category = currentAd.OwnerCategory,
                    CallToAction = currentAd.CallActionType,
                    MediaUrl = currentAd.MediaUrl,
                    OtherMediaUrl = currentAd.OtherMediaUrl,
                    DestinationUrl = currentAd.NavigationUrl,
                    AdTitle = string.IsNullOrEmpty(currentAd.Title) ? currentAd.Caption : currentAd.Title,
                    NewsFeedDescription = currentAd.SubDescription == null ? string.Empty : currentAd.SubDescription,
                    AdId = string.IsNullOrEmpty(adId) ? currentAd.AdId : adId,
                    PostDate = currentAd.PostedDateTime.ConvertToEpoch().ToString(),
                    FirstSeen = DateTimeUtilities.GetEpochTime().ToString(),
                    LastSeen = DateTimeUtilities.GetEpochTime().ToString(),
                    City = currentAd.City,
                    State = currentAd.State,
                    Country = currentAd.Country,
                    LowerAge = currentAd.LowerAge,
                    UpperAge = currentAd.UpperAge,
                    PostOwner = currentAd.OwnerName,
                    PostOwnerImage = currentAd.OwnerLogoUrl,
                    AdPosition = currentAd.AdType,
                    LikeCount = currentAd.LikersCount,
                    CommentCount = string.IsNullOrEmpty(currentAd.CommentorCount) ? currentAd.CommentData.Count().ToString() : currentAd.CommentorCount,
                    ShareCount = currentAd.SharerCount,
                    AdText = currentAd.Caption,
                    AdUrl = string.IsNullOrEmpty(currentAd.PostUrl) ? string.Empty : Uri.EscapeDataString(currentAd.PostUrl),
                    FacebookId = account.AccountBaseModel.UserId,
                    SideUrl = string.Empty,
                    Platform = "2",
                    Version = "1.0.32",
                    CommentData = currentAd.CommentData,
                    Verified = currentAd.IsOwnerVerified ? "1" : "0"
                };

                var reqParameter = new FdRequestParameter();

                var data = reqParameter.GetJsonString(objlement);

                var response = await _httpHelper.PostApiRequestAsync(url, data);

                if (response.Response.Contains("successfully"))
                {
                    try
                    {

                    }
                    catch (Exception)
                    {

                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }

        //public async Task<AdDetailsResponseHandler> GetAdCountryDetails(DominatorAccountModel account)
        //{
        //    try
        //    {
        //        var url = FdConstants.GetAdCountryDetails;

        //        var response = await _httpHelper.GetApiRequestAsync(url);

        //        return new AdDetailsResponseHandler(response);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.StackTrace);
        //    }

        //    return null;
        //}

        //public async Task<AdReactionDetailsResponseHandler> GetAdReactionDetails(DominatorAccountModel account,
        //    FacebookAdsDetails adDetails)
        //{
        //    try
        //    {

        //        var fullAdResponse = await _httpHelper.GetRequestAsync(adDetails.PostUrl, account.Token);

        //        await GetNavigationUrl(account, fullAdResponse.Response, adDetails, adDetails.PostUrl);

        //        return new AdReactionDetailsResponseHandler(fullAdResponse, adDetails);
        //    }
        //    catch (Exception)
        //    {

        //    }

        //    return null;
        //}

        //public async Task<bool> UpdateLcsDetails(DominatorAccountModel account, FacebookAdsDetails adDetails)
        //{
        //    try
        //    {
        //        var url = FdConstants.SaveAdCountryDetails;

        //        var objRequestParameter = _httpHelper.GetRequestParameter();

        //        objRequestParameter.AddHeader("Host", "api.socinator.com");

        //        objRequestParameter.ContentType = FdConstants.ContentTypeJson;

        //        _httpHelper.SetRequestParameter(objRequestParameter);

        //        var reactionDetails = new FdPublisherJsonElement()
        //        {
        //            AdId = adDetails.AdId,
        //            LikeCount = adDetails.LikersCount,
        //            CommentCount = adDetails.CommentorCount,
        //            ShareCount = adDetails.SharerCount,
        //            DestinationUrl = adDetails.NavigationUrl
        //        };

        //        FdPublisherJsonElement adAnalyticsData = new FdPublisherJsonElement()
        //        {
        //            Analytics = new FdPublisherJsonElement[1]
        //                        {
        //                            reactionDetails
        //                        }
        //        };

        //        var reqParameter = new FdRequestParameter();

        //        var data = reqParameter.GetJsonString(adAnalyticsData);

        //        var response = await _httpHelper.PostApiRequestAsync(url, data);

        //        if (response.Response.Contains("successfully"))
        //        {
        //            return true;
        //        }
        //    }
        //    catch (Exception)
        //    {

        //    }

        //    return false;
        //}

        public async Task<bool> CheckDuplicatesFromDb(DominatorAccountModel account, FacebookAdsDetails currentAd)
        {
            try
            {
                var url = FdConstants.SaveAdsUrlMain + "adsdata";

                //        var url = FdConstants.SaveAdsDataDev + "adsdata";

                if (Directory.Exists(FdConstants.SaveAdsPath))
                    Directory.Delete(FdConstants.SaveAdsPath);

                if (string.IsNullOrEmpty(currentAd.AdType) || currentAd.AdType != "SIDE")
                    currentAd.AdType = "FEED";

                currentAd.PostUrl = FdConstants.FbHomeUrl + currentAd.Id;

                var objRequestParameter = _httpHelper.GetRequestParameter();

                objRequestParameter.AddHeader("Host", "api.poweradspy.com");


                objRequestParameter.ContentType = FdConstants.ContentTypeJson;

                _httpHelper.SetRequestParameter(objRequestParameter);

                var adId = currentAd.AdType == "FEED" ? currentAd.Id : currentAd.AdId;

                FdAdsScraperJsonElement objlementNew = new FdAdsScraperJsonElement()
                {
                    AdId = adId,
                    Country = currentAd.Country,
                    AdPosition = currentAd.AdType,
                    LikeCount = string.IsNullOrWhiteSpace(currentAd.LikersCount) ? "0" : currentAd.LikersCount,
                    CommentCount = string.IsNullOrEmpty(currentAd.CommentorCount) ? "0" : currentAd.CommentorCount,
                    ShareCount = string.IsNullOrEmpty(currentAd.SharerCount) ? "0" : currentAd.SharerCount,
                    FacebookId = account.AccountBaseModel.UserId,
                    LastSeen = DateTimeUtilities.GetEpochTime().ToString(),
                    Platform = "2",
                    Socionator = "1"
                };

                var reqParameter = new FdRequestParameter();

                var data = reqParameter.GetJsonString(objlementNew);

                var response = await _httpHelper.PostApiRequestAsync(url, data);

                await _delayService.DelayAsync(1000);

                return response.Response.Contains("Ad already present");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }

            return false;
        }

        public async Task GetNavigationUrl(DominatorAccountModel account, string postResponse,
          FacebookAdsDetails postDetails, string postUrl)
        {
            try
            {


                HtmlDocument objDocument = new HtmlDocument();

                postResponse = FdFunctions.GetDecodedResponse(postResponse);

                objDocument.LoadHtml(postResponse);

                string adDetails;


                var adDetailsFull = objDocument.DocumentNode.SelectNodes("//div[@class=\"_5pcr userContentWrapper\"]");

                var fullDetailsForAd = adDetailsFull != null && adDetailsFull.Count > 0
                    ? adDetailsFull[0].InnerHtml
                    : postResponse;

                var adDetailsArr = Regex.Split(postResponse, "_42ft _4jy0 _4jy3 _517h _51sy");

                if (adDetailsArr.Length == 1)
                    adDetailsArr = Regex.Split(postResponse, "_42ft _4jy0 _4jy4 _517h _51sy");

                if (adDetailsArr.Length > 1)
                {
                    for (int i = 1; i < adDetailsArr.Length; i++)
                    {
                        adDetails = adDetailsArr[i];

                        //Ads NavigationUrl
                        await ScrapNewPostsNavigationUrl(account, adDetails, postDetails, postUrl, fullDetailsForAd);

                        if (!string.IsNullOrEmpty(postDetails.NavigationUrl))
                            break;
                    }
                }

                if (string.IsNullOrEmpty(postDetails.NavigationUrl))
                {

                    adDetailsArr = Regex.Split(postResponse, "_35sk _tbz _3htz");

                    if (adDetailsArr.Length > 1)
                    {
                        for (int i = 1; i < adDetailsArr.Length; i++)
                        {
                            adDetails = adDetailsArr[1];

                            //Ads NavigationUrl
                            await ScrapNewPostsNavigationUrl(account, adDetails, postDetails, postUrl, fullDetailsForAd);

                            if (!string.IsNullOrEmpty(postDetails.NavigationUrl))
                                break;
                        }
                    }
                }
                if (string.IsNullOrEmpty(postDetails.NavigationUrl))
                {

                    adDetailsArr = Regex.Split(postResponse, "PageLikeButton _4jy3 _517h _51sy");

                    if (adDetailsArr.Length > 1)
                    {
                        for (int i = 1; i < adDetailsArr.Length; i++)
                        {
                            adDetails = adDetailsArr[1];

                            //Ads NavigationUrl
                            await ScrapNewPostsNavigationUrl(account, adDetails, postDetails, postUrl, fullDetailsForAd);

                            if (!string.IsNullOrEmpty(postDetails.NavigationUrl))
                                break;
                        }
                    }
                }

                if (adDetailsFull != null)
                {
                    var navigationUrlList = Regex.Split(adDetailsFull[0].InnerHtml, "href=\"https://l.facebook.com/l.php\\?u=(.*?)\"").Skip(1).ToArray();

                    if (navigationUrlList.Length > 1)
                        foreach (string url in navigationUrlList)
                            if (FdFunctions.CheckUrlValid(Uri.UnescapeDataString(url)))
                                await ScrapNewPostsNavigationUrl(account, $"https://l.facebook.com/l.php?u={url}\"", postDetails, postUrl, fullDetailsForAd);//Ads Navigation Url
                }
            }
            catch (Exception)
            {

            }
        }

        public async Task ScrapNewPostsNavigationUrl(DominatorAccountModel account, string adDetails,
            FacebookAdsDetails postDetails, string postUrl, string postResponse)
        {
            try
            {
                string adCallAction = string.Empty;


                if (adDetails.Contains("</script>"))
                    adDetails = FdRegexUtility.FirstMatchExtractor(adDetails, FdConstants.AdDetailsRegex);

                FdHttpHelper objHttpHelper = new FdHttpHelper();

                FdLoginProcess.RequestParameterInitializeHttpHelper(account, ref objHttpHelper);


                try
                {
                    if (postDetails.CallActionType == "<" || string.IsNullOrEmpty(postDetails.CallActionType))
                    {

                        foreach (CallActionType calltoAction in Enum.GetValues(typeof(CallActionType)))
                        {
                            try
                            {
                                var possibleCallAction = calltoAction.GetDescriptionAttr();
                                if (!postResponse.Contains(possibleCallAction))
                                    continue;
                                adCallAction = possibleCallAction;
                                break;
                            }
                            catch (Exception ex)
                            {
                                ex.DebugLog();
                            }

                        }

                    }
                }
                catch (Exception)
                {

                }

                postDetails.CallActionType =
                    string.IsNullOrEmpty(postDetails.CallActionType) && !string.IsNullOrEmpty(adCallAction)
                        ? adCallAction
                        : postDetails.CallActionType;



                postDetails.NavigationUrl = FdRegexUtility.FirstMatchExtractor(adDetails, FdConstants.AdNavigationUrlRegex);


                var adNavigationUrl = Uri.UnescapeDataString(WebUtility.HtmlDecode(postDetails.NavigationUrl));

                if (!string.IsNullOrEmpty(adNavigationUrl) && !adNavigationUrl.Contains("https://") && !adNavigationUrl.Contains("http://") && !adNavigationUrl.Contains("apps.facebook.com"))
                {
                    adNavigationUrl = "https://l.facebook.com/l.php?u=" + adNavigationUrl;
                    adNavigationUrl = adNavigationUrl.Replace("\\", string.Empty).Replace("amp;", string.Empty);

                    if (!FdFunctions.CheckUrlValid(adNavigationUrl))
                    {
                        postDetails.NavigationUrl = postUrl;
                        return;
                    }

                    var pageResponse = await objHttpHelper.GetRequestAsync(adNavigationUrl, account.Token);

                    var referenceDetails = Utilities.GetBetween(pageResponse.Response, "location.replace(\"", "\"");

                    if (!string.IsNullOrEmpty(referenceDetails))
                    {
                        var path = await RedirectPath(FdFunctions.GetDecodedResponse(referenceDetails));
                        adNavigationUrl = WebUtility.HtmlDecode(path);
                    }
                    else
                        adNavigationUrl = postUrl;

                }
                else if (adNavigationUrl.Contains("apps.facebook.com"))
                {
                    adNavigationUrl = WebUtility.HtmlDecode(adNavigationUrl);

                    adNavigationUrl = FdRegexUtility.FirstMatchExtractor(adNavigationUrl, "(.*?)&h=");

                }
                else
                {
                    adNavigationUrl = WebUtility.HtmlDecode(adNavigationUrl);

                    adNavigationUrl = FdRegexUtility.FirstMatchExtractor(adNavigationUrl, "(.*?)&h=");

                }

                postDetails.NavigationUrl = adNavigationUrl;

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public async Task ViewersDetailsParser(DominatorAccountModel account, FacebookAdsDetails currentAd)
        {
            try
            {
                var objRequestParameter = new FdRequestParameter();

                objRequestParameter.UrlParameters.Add("id", currentAd.AdId);
                objRequestParameter.UrlParameters.Add("optout_url", "/settings/?tab=ads");
                objRequestParameter.UrlParameters.Add("page_type", "16");
                objRequestParameter.UrlParameters.Add("serialized_nfx_action_info", "");
                objRequestParameter.UrlParameters.Add("session_id", "29459");
                objRequestParameter.UrlParameters.Add("use_adchoices", "1");
                objRequestParameter.UrlParameters.Add("dpr", "1");
                objRequestParameter.UrlParameters.Add("__asyncDialog", "1");
                objRequestParameter = CommonUrlParameters(account, objRequestParameter);

                var url = objRequestParameter.GenerateUrl(FdConstants.AdsPreferenceUrl);

                var adsPreferencesResponse = await _httpHelper.GetRequestAsync(url, account.Token);

                var deccoededResponse = FdFunctions.GetDecodedResponse(adsPreferencesResponse.Response);

                var adViewerAge = string.Empty;

                var viewerAge = Utilities.GetBetween(deccoededResponse, "ages", "who");

                if (string.IsNullOrEmpty(viewerAge) || deccoededResponse.Contains("aged"))
                {
                    viewerAge = Utilities.GetBetween(deccoededResponse, "aged", "who");

                    viewerAge = string.IsNullOrEmpty(viewerAge) ? Utilities.GetBetween(deccoededResponse, "aged", "<")
                        : viewerAge;
                }

                if (!string.IsNullOrEmpty(viewerAge))
                {

                    adViewerAge = viewerAge.Contains("and older") ? viewerAge.Replace("and older", "to 65") : viewerAge;

                    var splitAge = Regex.Split(adViewerAge, "to").ToArray();
                    if (splitAge.Length > 0)
                    {
                        currentAd.LowerAge = splitAge[0].Trim();
                        if (splitAge.Length > 1)
                            currentAd.UpperAge = splitAge[1].Trim();
                    }
                }

                if (string.IsNullOrEmpty(currentAd.OwnerId))
                {
                    currentAd.OwnerId = FdRegexUtility.FirstMatchExtractor(deccoededResponse, "hovercard.php\\?id=(.*?)\"");
                    currentAd.OwnerName = FdRegexUtility.FirstMatchExtractor(deccoededResponse, ">(.*?)</b>");
                    currentAd.OwnerName = Regex.Split(currentAd.OwnerName, ">").LastOrDefault();
                }


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task<GetFanpageFullDetailsResponseHandler> GetPageDetails(DominatorAccountModel account, FacebookAdsDetails currentAd)
        {
            try
            {
                var pageId = !FdFunctions.IsIntegerOnly(currentAd.OwnerId) ? FdFunctions.GetIntegerOnlyString(currentAd.OwnerId) : currentAd.OwnerId;

                var pageUrl = FdConstants.FbFanpageUrl(pageId);

                IResponseParameter homePageResponse = await _httpHelper.GetRequestAsync(pageUrl, account.Token);

                await _delayService.DelayAsync(1000);

                return new GetFanpageFullDetailsResponseHandler(homePageResponse);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return null;
        }

        public async Task ScrapeComments(DominatorAccountModel account, FacebookAdsDetails currentAd)
        {
            try
            {
                FdRequestParameter requestParameter = new FdRequestParameter();

                var offset = int.Parse(currentAd.CommentorCount) - 50;

                offset = offset < 0 ? 0 : offset;

                if (FdFunctions.IsIntegerOnly(currentAd.CommentorCount) && int.Parse(currentAd.CommentorCount) > 0)
                {
                    string url = FdConstants.FbCommentPaginationUrl;
                    requestParameter.PostDataParameters.Add("ft_ent_identifier", currentAd.Id);
                    requestParameter.PostDataParameters.Add("viewas", "");
                    requestParameter.PostDataParameters.Add("source", "17");
                    requestParameter.PostDataParameters.Add("offset", offset.ToString());
                    requestParameter.PostDataParameters.Add("length", 50.ToString());
                    requestParameter.PostDataParameters.Add("orderingmode", "recent_activity");
                    requestParameter.PostDataParameters.Add("section", "default");
                    requestParameter.PostDataParameters.Add("direction", "bottom");
                    requestParameter.PostDataParameters.Add("feed_context", string.Empty);
                    requestParameter.PostDataParameters.Add("numpagerclicks", "1");
                    requestParameter.PostDataParameters.Add("av", account.AccountBaseModel.UserId);
                    requestParameter.PostDataParameters.Add("__req", "11");

                    requestParameter = CommonPostDataParameters(account, requestParameter);

                    var postData = requestParameter.GetPostDataFromParameters();

                    var request = _httpHelper.GetRequestParameter();

                    request.ContentType = FdConstants.ContentType;

                    _httpHelper.SetRequestParameter(request);

                    var paginationResponse = _httpHelper.PostRequest(url, postData);

                    await _delayService.DelayAsync(2000);

                    var commentScraperHandler =
                        new PostCommentorResponseHandler(paginationResponse, false, currentAd.Id, string.Empty, true, currentAd.PostUrl);

                    currentAd.CommentData = commentScraperHandler.ObjFdScraperResponseParameters.CommentList.Where(x => !string.IsNullOrEmpty(x.CommentText)).Select(x => FdFunctions.GetNewPrtialDecodedResponse(x.CommentText, true)).Take(30).ToArray();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void CheckGroupJoinStatus(DominatorAccountModel account, GroupDetails objGroupDetails, string url)
        {
            var htmlDocument = new HtmlDocument();
            var searchFanpageResponse = _httpHelper.GetRequest(url);
            try
            {
                htmlDocument.LoadHtml(searchFanpageResponse.Response);
                if (htmlDocument.DocumentNode.SelectNodes("//abbr") != null)
                    objGroupDetails.GroupJoinStatus = "Member";
                else if (htmlDocument.DocumentNode.SelectNodes("//*[@class=\"_42ft _4jy0 _55pi _2agf _4o_4 _p _4jy4 _517h _51sy mrm\"]") != null)
                    objGroupDetails.GroupJoinStatus = "Request Sent";
                else
                    objGroupDetails.GroupJoinStatus = "Not a member";

                //    objGroupDetails.GroupName = htmlDocument.DocumentNode.SelectSingleNode("//*[@id=\"seo_h1_tag\"]/a").InnerText;

                if (string.IsNullOrEmpty(objGroupDetails.GroupMemberCount) || FdFunctions.IsIntegerOnly(objGroupDetails.GroupMemberCount))
                    objGroupDetails.GroupMemberCount = GetGroupMemberCount(account, objGroupDetails.GroupId);

                if (string.IsNullOrEmpty(objGroupDetails.GroupMemberCount))
                {
                    htmlDocument.LoadHtml(searchFanpageResponse.Response.Replace("<!--", string.Empty).Replace("-->", string.Empty));
                    objGroupDetails.GroupMemberCount = htmlDocument.DocumentNode.SelectNodes("//div[@class=\"_63om _6qq6\"]")[1].InnerText;
                }

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public string GetGroupMemberCount(DominatorAccountModel accountModel, string groupId)
        {
            var url = FdConstants.FbGroupMemberPageUrl(groupId);

            var groupPageResponse = _httpHelper.GetRequest(url);

            string memberCount = "0";

            try
            {
                memberCount = FdRegexUtility.FirstMatchExtractor(groupPageResponse.Response, FdConstants.GroupMemberCountDetailsRegex);

                memberCount = FdRegexUtility.FirstMatchExtractor(memberCount, FdConstants.GroupMemberCountRegex);

                //memberCount = FdRegexUtility.FirstMatchExtractor(groupPageResponse.Response, "pam _grm uiBoxWhite noborder(.*?)</div>");

                //memberCount = FdRegexUtility.FirstMatchExtractor(memberCount, "_grt _50f8\">(.*?)</span>");

                memberCount = FdFunctions.GetIntegerOnlyString(memberCount);

                if (groupPageResponse.Response.Contains("Private group") && memberCount.Equals("0"))
                {
                    groupPageResponse = _httpHelper.GetRequest($"{FdConstants.FbHomeUrl}{groupId}");

                    memberCount = FdRegexUtility.FirstMatchExtractor(groupPageResponse.Response, "Members · (.*?)<");

                    memberCount = FdFunctions.GetIntegerOnlyString(memberCount);
                }


            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return memberCount;

        }

        /// <summary>
        /// Method to check login status
        /// </summary>
        /// <param name="accountModel"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<FdLoginResponseHandler> IsLoggedIn
            ([NotNull] DominatorAccountModel accountModel, CancellationToken token)
        {
            if (accountModel == null)
                throw new ArgumentNullException(nameof(accountModel));

            try
            {
                var parameters = _httpHelper.GetRequestParameter();

                if (parameters.Cookies["checkpoint"] != null)
                {
                    parameters.Cookies = new CookieCollection();
                    _httpHelper.SetRequestParameter(parameters);
                }

            }
            catch (Exception)
            {

            }
            var homepageResponse = await _httpHelper.GetRequestAsync(FdConstants.FbHomeUrl, token);

            FdConstants.IsWebFacebook = homepageResponse.Response?.Contains("https://web.facebook.com/") ?? false;

            await GetIpDetails(accountModel);

            var loginHandler = new FdLoginResponseHandler(homepageResponse, true, accountModel);

            if (!loginHandler.LoginStatus)
            {
                try
                {
                    if (loginHandler.FbErrorDetails != null && (loginHandler.LoginParameters == null || string.IsNullOrEmpty(loginHandler.LoginParameters.Lsd)))
                    {
                        DirectoryUtilities.CreateDirectory(FdConstants.FailedReponsePath);

                        var fileName = accountModel.AccountBaseModel.UserName.Split('@')[0].Replace('.', '_');

                        var filePath = FdConstants.FailedReponsePath + $@"\{fileName}.txt";

                        File.WriteAllText(filePath, homepageResponse.Response);

                        File.WriteAllText(filePath, JsonConvert.SerializeObject(loginHandler.FbErrorDetails));
                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }

            return loginHandler;
        }

        public async Task<FdLoginResponseHandler> IsLoggedIn
            ([NotNull] DominatorAccountModel accountModel, CancellationToken token, IResponseParameter loginResponse)
        {
            FdLoginResponseHandler loginHandler = null;
            try
            {
                if (string.IsNullOrEmpty(loginResponse.Response))
                    loginResponse = await _httpHelper.GetRequestAsync(FdConstants.FbHomeUrl, token);

                loginHandler = new FdLoginResponseHandler(loginResponse, false, accountModel);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return loginHandler;
        }

        public async Task<FdUserInfoResponseHandlerMobile> GetDetailedInfoUserMobileAsync
             (FacebookUser user, DominatorAccountModel account, bool isFullDeatailsRequired, bool isUseOriginalCookie,
                CancellationToken token)
        {

            var friendId = string.Empty;

            string userName = string.Empty;

            string aboutPageurl;

            if (user.UserId.Contains($"{FdConstants.FbHomeUrl}"))
                friendId = GetFriendUserId(account, user.UserId).UserId;
            else if (!string.IsNullOrEmpty(user.UserId))
                friendId = user.UserId;

            if (!string.IsNullOrEmpty(user.ScrapedProfileUrl))
            {
                userName = FdRegexUtility.FirstMatchExtractor(user.ScrapedProfileUrl, FdConstants.UsernameRegex);

                aboutPageurl = !string.IsNullOrEmpty(userName) && !userName.Contains("profile.php")
                    ? FdConstants.FbFriendAboutPageUrlMobileUsername(userName)
                    : FdConstants.FbFriendAboutPageUrlMobile(friendId);
            }
            else
                aboutPageurl = FdConstants.FbFriendAboutPageUrlMobile(friendId);

            var objFdRequestParameter = new FdRequestParameter();

            var objParameter = _httpHelper.GetRequestParameter();

            if (objParameter.Headers != null)
                objParameter.Headers["Host"] = "m.facebook.com";

            objParameter.Referer = !string.IsNullOrEmpty(userName) && !userName.Contains("profile.php")
                ? $"https://m.facebook.com/{userName}"
                : $"https://m.facebook.com/profile.php?id={friendId}";

            _httpHelper.SetRequestParameter(objParameter);

            string timeSpan = DateTime.Now.GetCurrentEpochTime().ToString();

            string lstValue = $"{account.AccountBaseModel.UserId}:{friendId}:{timeSpan}";

            if (aboutPageurl.Contains("profile.php"))
            {
                objFdRequestParameter.UrlParameters.Add("v", "info");
                objFdRequestParameter.UrlParameters.Add("lst", lstValue);
                objFdRequestParameter.UrlParameters.Add("id", friendId);
            }
            else
                objFdRequestParameter.UrlParameters.Add("lst", lstValue);

            aboutPageurl = objFdRequestParameter.GenerateUrl(aboutPageurl);

            var aboutPageResponse = await _httpHelper.GetRequestAsync(aboutPageurl, token);

            var userResponseHandler = new FdUserInfoResponseHandlerMobile(aboutPageResponse, new FacebookUser());

            var parameter = _httpHelper.GetRequestParameter();

            parameter.Headers["Host"] = "www.facebook.com";

            _httpHelper.SetRequestParameter(parameter);

            return userResponseHandler;
        }

        public string GetGroupIdFromUrl(DominatorAccountModel account, string groupUrl)
        {
            var groupPageResponse = _httpHelper.GetRequest(groupUrl);

            string groupId = string.Empty;
            try
            {
                groupId = FdRegexUtility.FirstMatchExtractor(groupPageResponse.Response, FdConstants.GroupIdRegex);

                if (string.IsNullOrEmpty(groupId))
                    groupId = FdRegexUtility.FirstMatchExtractor(groupPageResponse.Response, "group\\/\\?id=(.*?)\"");
                if (string.IsNullOrEmpty(groupId))
                    groupId = FdRegexUtility.FirstMatchExtractor(groupPageResponse.Response, "groupID\":\"(.*?)\"");
                groupId = FdFunctions.GetIntegerOnlyString(groupId);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            if (string.IsNullOrEmpty(groupId))
            {

                try
                {
                    if (!groupUrl.Contains("/about/") && !groupUrl.Contains("groups"))
                        groupUrl = $"{FdConstants.FbHomeUrl}groups/{FdFunctions.GetIntegerOnlyString(groupUrl)}/about";
                    else if (!groupUrl.Contains("/about/"))
                        groupUrl = groupUrl + "/about/";

                    groupPageResponse = _httpHelper.GetRequest(groupUrl);

                    groupId = FdRegexUtility.FirstMatchExtractor(groupPageResponse.Response, FdConstants.GroupIdRegex);

                    //groupId = FdRegexUtility.FirstMatchExtractor(groupPageResponse.Response, FdConstants.GroupIdRegex);

                    groupId = FdFunctions.GetIntegerOnlyString(groupId);
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

            }

            return groupId;
        }

        public string GetTokenValueFromGroup(DominatorAccountModel account, string group_id)
        {

            FdRequestParameter objFdRequestParameter = new FdRequestParameter();

            objFdRequestParameter.UrlParameters.Add("group_id", group_id);
            objFdRequestParameter.UrlParameters.Add("__asyncDialog", "3");
            objFdRequestParameter.UrlParameters.Add("__req", "46");
            objFdRequestParameter.UrlParameters.Add("refresh", "1");
            CommonUrlParameters(account, objFdRequestParameter);

            string url = objFdRequestParameter.GenerateUrl();

            IResponseParameter tokenResponse = _httpHelper.GetRequest(url);

            return new GetGroupTokenResponseHandler(tokenResponse).Token;
        }

        public string GetPageIdFromUrl(DominatorAccountModel account, string fanpageUrl)
        {

            if (!fanpageUrl.Contains(FdConstants.FbHomeUrl) && FdFunctions.IsIntegerOnly(fanpageUrl))
                fanpageUrl = FdConstants.FbHomeUrl + fanpageUrl;

            var groupPageResponse = _httpHelper.GetRequest(fanpageUrl);

            string pageId = groupPageResponse.Response == null
                ? string.Empty
                : FdRegexUtility.FirstMatchExtractor(groupPageResponse.Response, FdConstants.PageIdRegex);

            //string pageId = FdRegexUtility.FirstMatchExtractor(groupPageResponse.Response, FdConstants.PageIdRegex);

            pageId = string.IsNullOrEmpty(pageId) && groupPageResponse.Response != null
                ? FdRegexUtility.FirstMatchExtractor(groupPageResponse.Response, FdConstants.EntityIdRegex)
                : pageId;

            pageId = string.IsNullOrEmpty(pageId) && groupPageResponse.Response != null
                        ? FdRegexUtility.FirstMatchExtractor(groupPageResponse.Response, FdConstants.UserId2Regex)
                        : pageId;

            if (fanpageUrl.Contains("profile.php"))
                pageId = FdRegexUtility.FirstMatchExtractor(fanpageUrl, "id=(.*?)$");

            return FdFunctions.GetIntegerOnlyString(pageId);
        }

        public FanpageDetails GetPageDetailsFromUrl(DominatorAccountModel account, string fanpageUrl)
        {


            var url = fanpageUrl;

            var pageDetails = new FanpageDetails();

            try
            {
                if (!url.Contains(FdConstants.FbHomeUrl))
                    url = FdConstants.FbHomeUrl + fanpageUrl;

                var groupPageResponse = _httpHelper.GetRequest(url);


                string pageId = FdRegexUtility.FirstMatchExtractor(groupPageResponse.Response, FdConstants.PageIdRegex);

                pageId = string.IsNullOrEmpty(pageId)
                    ? FdRegexUtility.FirstMatchExtractor(groupPageResponse.Response, FdConstants.UserId2Regex)
                    : pageId;

                if (string.IsNullOrEmpty(pageId))
                    pageId = FdRegexUtility.FirstMatchExtractor(url, $"{FdConstants.FbHomeUrl}(.*?)$");

                var decodedResponse = FdFunctions.GetDecodedResponse(groupPageResponse.Response);

                var pageName = FdRegexUtility.FirstMatchExtractor(decodedResponse, FdConstants.FanpageNameRegx);

                if (string.IsNullOrEmpty(pageName))
                    pageName = FdRegexUtility.FirstMatchExtractor(decodedResponse, FdConstants.PageTitleRegx);

                if (string.IsNullOrEmpty(pageName))
                    pageName = FdRegexUtility.FirstMatchExtractor(decodedResponse, "{\"title\":\"(.*?)\"");

                pageName = string.IsNullOrEmpty(pageName)
                    ? FdRegexUtility.FirstMatchExtractor(groupPageResponse.Response, "Switch to (.*?) to view professional")
                    : pageName;

                pageName = FdFunctions.GetDecodedResponse(pageName);

                pageDetails.FanPageUrl = url;
                pageDetails.FanPageID = pageId;
                pageDetails.FanPageName = pageName;

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return pageDetails;
        }

        public EventDetailsResponseHandler GetEventDetailsFromUrl(DominatorAccountModel account, string eventUrl)
            => new EventDetailsResponseHandler(_httpHelper.GetRequest(eventUrl));

        public IResponseHandler UpdateFriendsNewSync(DominatorAccountModel account, IResponseHandler responseHandler)
            => UpdateFriendsNew(account, responseHandler, account.Token, false).Result;

        public async Task<IResponseHandler> UpdateFriendsNew(DominatorAccountModel account, IResponseHandler responseHandler,
            CancellationToken token, bool isLastPage)
        {

            FdFriendsInfoNewResponseHandler objFdFriendsInfoResponseHandler = null;

            FdRequestParameter objFdRequestParameter = new FdRequestParameter();

            await _delayService.DelayAsync(500);

            try
            {
                if (responseHandler == null)
                {

                    objFdRequestParameter.UrlParameters.Add("privacy_source", "activity_log");
                    objFdRequestParameter.UrlParameters.Add("log_filter", "friends");

                    var url = objFdRequestParameter.GenerateUrl(FdConstants.FbSentRequestUrl(account.AccountBaseModel.UserId));

                    var sentFriendResponse = await _httpHelper.GetRequestAsync(url, account.Token);

                    objFdFriendsInfoResponseHandler = new FdFriendsInfoNewResponseHandler(sentFriendResponse, null, account.AccountBaseModel.UserId, false);

                    if (objFdFriendsInfoResponseHandler.ObjFdScraperResponseParameters.ListUser?.Count == 0)
                        return await GetUnfriendPaginationAsync(account, objFdFriendsInfoResponseHandler, account.Token, isLastPage);
                }
                else
                    return await GetUnfriendPaginationAsync(account, responseHandler, account.Token, isLastPage);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return objFdFriendsInfoResponseHandler;
        }

        public async Task<IResponseHandler> GetUnfriendPaginationAsync
            (DominatorAccountModel account, IResponseHandler responseHandler,
            CancellationToken token, bool isLastPage)
        {
            FdFriendsInfoNewResponseHandler objFdFriendsInfoResponseHandler;


            FdRequestParameter objFdRequestParameter = new FdRequestParameter();

            do
            {
                objFdRequestParameter.UrlParameters.Clear();

                objFdRequestParameter.UrlParameters.Add("profile_id", account.AccountBaseModel.UserId);
                objFdRequestParameter.UrlParameters.Add("hidden_filter", "");
                objFdRequestParameter.UrlParameters.Add("only_me_filter", "0");
                objFdRequestParameter.UrlParameters.Add("prev_cursor", responseHandler.ObjFdScraperResponseParameters.FriendsPager.PrevousCursor);
                objFdRequestParameter.UrlParameters.Add("prev_shown_time", responseHandler.ObjFdScraperResponseParameters.FriendsPager.ShownTime);
                objFdRequestParameter.UrlParameters.Add("privacy_filter", "");
                objFdRequestParameter.UrlParameters.Add("sidenav_filter", "friends");
                objFdRequestParameter.UrlParameters.Add("scrubber_month", responseHandler.ObjFdScraperResponseParameters.FriendsPager.ScrubberMonth);
                objFdRequestParameter.UrlParameters.Add("scrubber_year", responseHandler.ObjFdScraperResponseParameters.FriendsPager.ScrubberYear);
                objFdRequestParameter.UrlParameters.Add("data", "1.5");
                objFdRequestParameter.UrlParameters.Add("__req", "10");

                objFdRequestParameter = CommonUrlParameters(account, objFdRequestParameter);


                var url = objFdRequestParameter.GenerateUrl(FdConstants.SentFriendExtraPager);

                var paginationResponse = await _httpHelper.GetRequestAsync(url, token);


                objFdFriendsInfoResponseHandler = new FdFriendsInfoNewResponseHandler(paginationResponse, responseHandler.ObjFdScraperResponseParameters.FriendsPager, account.AccountBaseModel.UserId, true);

                if (objFdFriendsInfoResponseHandler.ObjFdScraperResponseParameters.ListUser.Count == 0)
                {
                    objFdRequestParameter.UrlParameters.Clear();

                    objFdRequestParameter.UrlParameters.Add("dpr", "1.5");
                    objFdRequestParameter.UrlParameters.Add("ajaxpipe", "1");
                    objFdRequestParameter.UrlParameters.Add("ajaxpipe_token", responseHandler.ObjFdScraperResponseParameters.FriendsPager.AjaxPipeToken);
                    objFdRequestParameter.UrlParameters.Add("no_script_path", "1");
                    objFdRequestParameter.UrlParameters.Add("data", responseHandler.ObjFdScraperResponseParameters.FriendsPager.Data);
                    objFdRequestParameter.UrlParameters.Add("__req", "fetchstream_1");
                    objFdRequestParameter.UrlParameters.Add("__adt", "1");
                    objFdRequestParameter.UrlParameters.Add("ajaxpipe_fetch_stream", "1");

                    objFdRequestParameter = CommonUrlParameters(account, objFdRequestParameter);

                    url = objFdRequestParameter.GenerateUrl(FdConstants.FriendActivityNextPager);

                    paginationResponse = await _httpHelper.GetRequestAsync(url, account.Token);


                    objFdFriendsInfoResponseHandler = new FdFriendsInfoNewResponseHandler(paginationResponse, responseHandler.ObjFdScraperResponseParameters.FriendsPager, account.AccountBaseModel.UserId, false);

                }
                if (isLastPage && objFdFriendsInfoResponseHandler.ObjFdScraperResponseParameters?.ListUser?.Count == 0)
                    break;


            } while (objFdFriendsInfoResponseHandler.ObjFdScraperResponseParameters?.ListUser?.Count == 0 &&
                objFdFriendsInfoResponseHandler.ObjFdScraperResponseParameters?.FriendsPager?.CurrentDataKey != objFdFriendsInfoResponseHandler.ObjFdScraperResponseParameters?.FriendsPager?.MaxDataKey);


            return objFdFriendsInfoResponseHandler;
        }

        public async Task<IResponseHandler> UpdateFriendsFromPage
           (DominatorAccountModel account, FriendsUpdateResponseHandler responseHandler, CancellationToken token, List<string> lstPageId)
        {
            int count = 0;

            bool isCompleted = false;

            FriendsUpdateResponseHandler objFdFriendsInfoResponseHandler = null;

            FdRequestParameter objFdRequestParameter = new FdRequestParameter();

            try
            {
                if (responseHandler == null)
                {
                    while (count < 5 && !isCompleted)
                    {
                        var randomPageId = lstPageId.GetRandomItem();

                        _delayService.ThreadSleep(2000);

                        objFdRequestParameter.UrlParameters.Clear();

                        objFdRequestParameter.UrlParameters.Add("ref", "context_row");
                        objFdRequestParameter.UrlParameters.Add("dpr", "1");
                        objFdRequestParameter.UrlParameters.Add("__asyncDialog", "1");
                        objFdRequestParameter.UrlParameters.Add("__req", "17");

                        objFdRequestParameter = CommonUrlParameters(account, objFdRequestParameter);

                        var url = FdConstants.InviterDialogUrl(randomPageId);

                        url = objFdRequestParameter.GenerateUrl(url);

                        var friendsResponse = await _httpHelper.GetRequestAsync(url, account.Token);

                        objFdFriendsInfoResponseHandler = new FriendsUpdateResponseHandler(friendsResponse, false);

                        if (objFdFriendsInfoResponseHandler.ObjFdScraperResponseParameters.ListUser.Count > 0)
                            isCompleted = true;

                        count++;

                    }
                }

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return objFdFriendsInfoResponseHandler;
        }

        public FbUserIdResponseHandler GetFriendUserId(DominatorAccountModel account, string friendUrl)
        {
            if (string.IsNullOrEmpty(friendUrl))
                return new FbUserIdResponseHandler(new ResponseParameter() { Response = string.Empty });

            var homepageResponse = _httpHelper.GetRequest(friendUrl);

            var userIdResponseHandler = new FbUserIdResponseHandler(homepageResponse);

            //try
            //{
            //    string entityId = FdRegexUtility.FirstMatchExtractor(homepageResponse.Response, FdConstants.EntityIdRegex);

            //    if (string.IsNullOrEmpty(entityId))
            //        entityId = FdRegexUtility.FirstMatchExtractor(homepageResponse.Response, "entity_id\":(.*?),");
            //    if (string.IsNullOrEmpty(entityId))
            //        entityId = FdRegexUtility.FirstMatchExtractor(homepageResponse.Response, "entity_id:(.*?),");

            //    userIdResponseHandler.UserId = entityId;
            //}
            //catch (Exception ex)
            //{
            //    ex.DebugLog();
            //}

            return userIdResponseHandler;
        }

        public async Task<FbUserIdResponseHandler> GetFriendUserIdAsync(DominatorAccountModel account, string friendUrl)
        {
            var homepageResponse = await _httpHelper.GetRequestAsync(friendUrl, account.Token);

            var userIdResponseHandler = new FbUserIdResponseHandler(homepageResponse);

            try
            {
                string entityId = FdRegexUtility.FirstMatchExtractor(homepageResponse.Response, FdConstants.EntityIdRegex);

                if (string.IsNullOrEmpty(entityId))
                    entityId = FdRegexUtility.FirstMatchExtractor(homepageResponse.Response, "entity_id\":(.*?),");

                userIdResponseHandler.UserId = entityId;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return userIdResponseHandler;
        }

        public IResponseHandler GetDetailedInfoUserMobile
            (FacebookUser user, DominatorAccountModel account, bool isFullDeatailsRequired, bool isUseOriginalCookie)
        {
            var friendId = string.Empty;

            string userName = string.Empty;

            string aboutPageurl;

            var objFdRequestParameter = new FdRequestParameter();

            var objParameter = _httpHelper.GetRequestParameter();

            string url = string.IsNullOrEmpty(user.UserId) ? user.ProfileUrl : $"https://m.facebook.com/{user.UserId}";

            url = string.IsNullOrEmpty(url) ? user.ScrapedProfileUrl : url;

            if (objParameter.Headers != null)
                objParameter.Headers["Host"] = "m.facebook.com";

            objParameter.Referer = "https://m.facebook.com/";

            var homepageResponse = _httpHelper.GetRequest(url).Response;


            if (homepageResponse != null && !homepageResponse.Contains($"profile_add_friend.php?subjectid={user.UserId}"))
                return null;

            if (string.IsNullOrEmpty(user.UserId) || user.UserId.Contains($"{FdConstants.FbHomeUrl}"))
                friendId = FdRegexUtility.FirstMatchExtractor(homepageResponse, FdConstants.FriendIdRegex);
            else if (!string.IsNullOrEmpty(user.UserId))
                friendId = user.UserId;

            //var mobileIdValue = FdRegexUtility.FirstMatchExtractor(homepageResponse, "page_uri:\"https://m.facebook.com/(.*?)\\?").Contains("profile.php")
            //    ? string.Empty
            //    : FdRegexUtility.FirstMatchExtractor(homepageResponse, "page_uri:\"https://m.facebook.com/(.*?)\\?");

            if (!string.IsNullOrEmpty(user.ScrapedProfileUrl))
            {
                userName = FdRegexUtility.FirstMatchExtractor(user.ScrapedProfileUrl, FdConstants.UsernameRegex);

                if (string.IsNullOrEmpty(userName))
                {
                    string tempData = $"{user.ScrapedProfileUrl}>";
                    userName = FdRegexUtility.FirstMatchExtractor(tempData, "facebook.com/(.*?)>");
                }

                aboutPageurl = !string.IsNullOrEmpty(userName) && !userName.Contains("profile.php")
                                   ? FdConstants.FbFriendAboutPageUrlMobileUsername(userName)
                                   : FdConstants.FbFriendAboutPageUrlMobile(friendId);

            }
            else
                aboutPageurl = FdConstants.FbFriendAboutPageUrlMobile(friendId);

            objParameter = _httpHelper.GetRequestParameter();

            if (objParameter.Headers != null)
                objParameter.Headers["Host"] = "m.facebook.com";

            objParameter.Referer = !string.IsNullOrEmpty(userName) && !userName.Contains("profile.php")
                ? $"https://m.facebook.com/{userName}"
                : $"https://m.facebook.com/profile.php?id={friendId}";

            _httpHelper.SetRequestParameter(objParameter);

            string timeSpan = DateTime.Now.GetCurrentEpochTime().ToString();

            string lstValue = $"{account.AccountBaseModel.UserId}:{friendId}:{timeSpan}";

            if (aboutPageurl.Contains("profile.php"))
            {
                objFdRequestParameter.UrlParameters.Add("v", "info");
                objFdRequestParameter.UrlParameters.Add("lst", lstValue);
                objFdRequestParameter.UrlParameters.Add("id", friendId);
            }
            else
                objFdRequestParameter.UrlParameters.Add("lst", lstValue);

            aboutPageurl = objFdRequestParameter.GenerateUrl(aboutPageurl);

            var aboutPageResponse = _httpHelper.GetRequest(aboutPageurl);

            //if (aboutPageResponse.Response == null)
            //{
            //    aboutPageurl = !string.IsNullOrEmpty(mobileIdValue)
            //   ? $"https://m.facebook.com/{mobileIdValue}/about?lst={account.AccountBaseModel.UserId}%3A{friendId}%3A{timeSpan}"
            //   : $"https://m.facebook.com/profile.php?id={friendId}";

            //    aboutPageResponse = _httpHelper.GetRequest(aboutPageurl);
            //}

            var userResponseHandler = new FdUserInfoResponseHandlerMobile(aboutPageResponse, new FacebookUser());

            var parameter = _httpHelper.GetRequestParameter();

            parameter.Headers["Host"] = "www.facebook.com";

            _httpHelper.SetRequestParameter(parameter);

            return userResponseHandler;
        }

        public IResponseHandler GetDetailedInfoUserMobileScraper
            (FacebookUser user, DominatorAccountModel account, bool isFullDeatailsRequired, bool isUseOriginalCookie)
        {

            var friendId = string.Empty;

            string userName = string.Empty;

            string aboutPageurl;

            var objFdRequestParameter = new FdRequestParameter();

            var objParameter = _httpHelper.GetRequestParameter();

            if (objParameter.Headers != null)
                objParameter.Headers["Host"] = "m.facebook.com";

            objParameter.Referer = "https://m.facebook.com/";

            if (user.UserId != null && user.UserId.Contains($"{FdConstants.FbHomeUrl}"))
            {
                var homepageResponse = _httpHelper.GetRequest(user.UserId).Response;

                _delayService.ThreadSleep(1000);

                friendId = FdRegexUtility.FirstMatchExtractor(homepageResponse, FdConstants.FriendIdRegex);

                if (string.IsNullOrEmpty(friendId))
                    friendId = FdRegexUtility.FirstMatchExtractor(homepageResponse, FdConstants.EntityIdRegex);
            }
            else if (!string.IsNullOrEmpty(user.UserId))
                friendId = user.UserId;

            if (!string.IsNullOrEmpty(user.ScrapedProfileUrl) || !string.IsNullOrEmpty(user.ProfileUrl))
            {
                var profileURL = string.IsNullOrEmpty(user.ScrapedProfileUrl) ? user.ProfileUrl : user.ScrapedProfileUrl;
                userName = FdRegexUtility.FirstMatchExtractor(profileURL, FdConstants.UsernameRegex);

                if (string.IsNullOrEmpty(userName))
                    userName = FdRegexUtility.FirstMatchExtractor($"{profileURL} ", "facebook.com/(.*?) ");

                if (!string.IsNullOrEmpty(userName) && !userName.Contains("profile.php"))
                    aboutPageurl = FdConstants.FbFriendAboutPageUrlMobileUsername(userName);
                else
                {
                    if (!string.IsNullOrEmpty(userName) && string.IsNullOrEmpty(friendId))
                        friendId = FdRegexUtility.FirstMatchExtractor($"{profileURL} ", "php\\?id=(.*?) ");

                    friendId = string.IsNullOrEmpty(friendId) ? userName : friendId;

                    aboutPageurl = FdConstants.FbFriendAboutPageUrlMobile(friendId);
                }
            }
            else
            {
                aboutPageurl = FdConstants.FbFriendAboutPageUrlMobile(friendId);
            }


            objParameter = _httpHelper.GetRequestParameter();

            if (objParameter.Headers != null)
                objParameter.Headers["Host"] = "m.facebook.com";

            objParameter.Referer = !string.IsNullOrEmpty(userName) && !userName.Contains("profile.php")
                ? $"https://m.facebook.com/{userName}"
                : $"https://m.facebook.com/profile.php?id={friendId}";

            _httpHelper.SetRequestParameter(objParameter);

            string timeSpan = DateTime.Now.GetCurrentEpochTime().ToString();

            if (string.IsNullOrEmpty(friendId))
            {
                friendId = !string.IsNullOrEmpty(user.ScrapedProfileUrl)
                    ? GetFriendUserId(account, user.ScrapedProfileUrl).UserId
                    : GetFriendUserId(account, user.ProfileUrl).UserId;

                Thread.Sleep(1000);
            }

            string lstValue = $"{account.AccountBaseModel.UserId}:{friendId}:{timeSpan}";

            if (aboutPageurl.Contains("profile.php"))
            {
                objFdRequestParameter.UrlParameters.Add("v", "info");
                objFdRequestParameter.UrlParameters.Add("lst", lstValue);
                objFdRequestParameter.UrlParameters.Add("id", friendId);
            }
            else
                objFdRequestParameter.UrlParameters.Add("lst", lstValue);


            aboutPageurl = objFdRequestParameter.GenerateUrl(aboutPageurl);

            var aboutPageResponse = _httpHelper.GetRequest(aboutPageurl);

            var userResponseHandler = new FdUserInfoResponseHandlerMobile(aboutPageResponse,
                new FacebookUser() { UserId = friendId == null ? String.Empty : friendId });

            var parameter = _httpHelper.GetRequestParameter();

            if (parameter.Headers["Host"] != null)
                parameter.Headers["Host"] = "www.facebook.com";

            _delayService.ThreadSleep(1000);

            _httpHelper.SetRequestParameter(parameter);

            return userResponseHandler;
        }

        public async Task<FdUserInfoResponseHandlerMobile> GetDetailedInfoUserMobileScraperAsync
            (FacebookUser user, DominatorAccountModel account, bool isFullDeatailsRequired,
            bool isUseOriginalCookie, CancellationToken token, string timeSpan = "")
        {

            var friendId = string.Empty;

            string userName = string.Empty;

            string aboutPageurl;

            var objFdRequestParameter = new FdRequestParameter();

            var objParameter = _httpHelper.GetRequestParameter();

            if (objParameter.Headers != null)
                objParameter.Headers["Host"] = "m.facebook.com";

            objParameter.Referer = "https://m.facebook.com/";

            if (user.UserId != null && user.UserId.Contains($"{FdConstants.FbHomeUrl}"))
            {
                var homepageResponse = await _httpHelper.GetRequestAsync(user.UserId, account.Token);

                _delayService.ThreadSleep(1000);

                friendId = FdRegexUtility.FirstMatchExtractor(homepageResponse.Response, FdConstants.FriendIdRegex);

                if (string.IsNullOrEmpty(friendId))
                    friendId = FdRegexUtility.FirstMatchExtractor(homepageResponse.Response, FdConstants.EntityIdRegex);
            }
            else if (!string.IsNullOrEmpty(user.UserId))
                friendId = user.UserId;

            if (!string.IsNullOrEmpty(user.ScrapedProfileUrl))
            {
                userName = FdRegexUtility.FirstMatchExtractor(user.ScrapedProfileUrl, FdConstants.UsernameRegex);

                if (string.IsNullOrEmpty(userName))
                    userName = FdRegexUtility.FirstMatchExtractor($"{user.ScrapedProfileUrl} ", "facebook.com/(.*?) ");

                if (!string.IsNullOrEmpty(userName) && !userName.Contains("profile.php"))
                    aboutPageurl = FdConstants.FbFriendAboutPageUrlMobileUsername(userName);
                else
                {
                    if (!string.IsNullOrEmpty(userName) && string.IsNullOrEmpty(friendId))
                        friendId = FdRegexUtility.FirstMatchExtractor($"{user.ScrapedProfileUrl} ", "php\\?id=(.*?) ");

                    aboutPageurl = FdConstants.FbFriendAboutPageUrlMobile(friendId);
                }
            }
            else
                aboutPageurl = FdConstants.FbFriendAboutPageUrlMobile(friendId);

            objParameter = _httpHelper.GetRequestParameter();

            if (objParameter.Headers != null)
                objParameter.Headers["Host"] = "m.facebook.com";

            objParameter.Referer = !string.IsNullOrEmpty(userName) && !userName.Contains("profile.php")
                ? $"https://m.facebook.com/{userName}"
                : $"https://m.facebook.com/profile.php?id={friendId}";

            _httpHelper.SetRequestParameter(objParameter);

            if (string.IsNullOrEmpty(timeSpan))
                timeSpan = DateTime.Now.GetCurrentEpochTime().ToString();

            if (string.IsNullOrEmpty(friendId))
            {
                var friendDetails = await GetFriendUserIdAsync(account, user.ScrapedProfileUrl);

                friendId = friendDetails.UserId;

                await _delayService.DelayAsync(1000);
            }

            string lstValue = $"{account.AccountBaseModel.UserId}:{friendId}:{timeSpan}";

            if (aboutPageurl.Contains("profile.php"))
            {
                objFdRequestParameter.UrlParameters.Add("v", "info");
                objFdRequestParameter.UrlParameters.Add("lst", lstValue);
                objFdRequestParameter.UrlParameters.Add("id", friendId);
            }
            else
                objFdRequestParameter.UrlParameters.Add("lst", lstValue);


            aboutPageurl = objFdRequestParameter.GenerateUrl(aboutPageurl);

            var aboutPageResponse = await _httpHelper.GetRequestAsync(aboutPageurl, account.Token);

            var userResponseHandler = new FdUserInfoResponseHandlerMobile(aboutPageResponse, new FacebookUser());

            var parameter = _httpHelper.GetRequestParameter();

            if (parameter.Headers != null)
                parameter.Headers["Host"] = "www.facebook.com";

            _delayService.ThreadSleep(1000);

            _httpHelper.SetRequestParameter(parameter);

            return userResponseHandler;
        }

        public async Task<IResponseHandler> ScrapGroupsNewAsync
            (DominatorAccountModel account,
            GroupScraperResponseHandlerNew responseHandler, CancellationToken token)
        {
            string url;

            GroupScraperResponseHandlerNew groupScraperResponseHandler;

            FdRequestParameter objFdRequestParameter = new FdRequestParameter();

            if (responseHandler == null)
            {
                url = FdConstants.UpdateJoinedGroupUrl;

                #region Old Method
                var searchFanpageResponse = await _httpHelper.GetRequestAsync(url, token);

                if (searchFanpageResponse.Response.Contains("errorSummary\":\"") || string.IsNullOrEmpty(searchFanpageResponse.Response))
                {
                    _delayService.ThreadSleep(2000);
                    searchFanpageResponse = await _httpHelper.GetRequestAsync(url, token);
                }
                //_k _77by _1a1e
                //if (!searchFanpageResponse.Response.Contains("suggestions:{nodes:"))
                groupScraperResponseHandler = new GroupScraperResponseHandlerNew(searchFanpageResponse, null, null);
                #endregion

                if (groupScraperResponseHandler.ObjFdScraperResponseParameters.ListGroup.Count == 0)
                {
                    url = objFdRequestParameter.GenerateUrl(FdConstants.GetCommentUrl);

                    objFdRequestParameter.PostDataParameters.Clear();
                    objFdRequestParameter.PostDataParameters.Add("av", account.AccountBaseModel.UserId);
                    objFdRequestParameter.PostDataParameters.Add("variables", FdConstants.GetOwnGroupVariables);
                    objFdRequestParameter.PostDataParameters.Add("fb_api_caller_class", "RelayModern");
                    objFdRequestParameter.PostDataParameters.Add("doc_id", "2034588956654971");
                    objFdRequestParameter.PostDataParameters.Add("fb_api_req_friendly_name", "GroupsLandingYourGroupsQuery");
                    objFdRequestParameter.PostDataParameters.Add("__req", "11");
                    objFdRequestParameter = CommonPostDataParameters(account, objFdRequestParameter);

                    var postData = objFdRequestParameter.GetPostDataFromParameters();

                    var request = _httpHelper.GetRequestParameter();

                    request.ContentType = FdConstants.ContentType;

                    _httpHelper.SetRequestParameter(request);

                    searchFanpageResponse = _httpHelper.PostRequest(url, postData);

                    if (searchFanpageResponse.HasError)
                    {
                        _delayService.ThreadSleep(2000);
                        searchFanpageResponse = _httpHelper.PostRequest(url, postData);
                    }
                    groupScraperResponseHandler = new GroupScraperResponseHandlerNew(searchFanpageResponse, null, null, true);
                }

                groupScraperResponseHandler.HasMoreResults = !string.IsNullOrEmpty(groupScraperResponseHandler.PageletData);
            }
            else
            {

                var paginationData = responseHandler.PageletData;

                objFdRequestParameter.UrlParameters.Clear();

                objFdRequestParameter.UrlParameters.Add("ref", "bookmarks");
                objFdRequestParameter.UrlParameters.Add("existing_ids", paginationData);
                objFdRequestParameter.UrlParameters.Add("twocolumns", "1");
                objFdRequestParameter.UrlParameters.Add("card_type", "1");
                objFdRequestParameter.UrlParameters.Add("display_type", "default");
                objFdRequestParameter.UrlParameters.Add("category_id", "membership");
                objFdRequestParameter.UrlParameters.Add("page_type", "admin_and_member");
                objFdRequestParameter.UrlParameters.Add("auto_load_more", "1");
                objFdRequestParameter.UrlParameters.Add("seemore_id", "membership");
                objFdRequestParameter.UrlParameters.Add("order", "top_groups");
                objFdRequestParameter.UrlParameters.Add("dpr", "1");
                objFdRequestParameter.UrlParameters.Add("__req", "10");
                objFdRequestParameter = CommonUrlParameters(account, objFdRequestParameter);

                url = objFdRequestParameter.GenerateUrl(FdConstants.JoinedGroupPageletUrl);

                var requestParameter = _httpHelper.GetRequestParameter();

                requestParameter.Referer = FdConstants.UpdateJoinedGroupUrl;

                _httpHelper.SetRequestParameter(requestParameter);

                var paginationResponse = await _httpHelper.GetRequestAsync(url, token);

                if (paginationResponse.Response.Contains("errorSummary\":\""))
                {
                    _delayService.ThreadSleep(2000);
                    paginationResponse = await _httpHelper.GetRequestAsync(url, token);
                }

                groupScraperResponseHandler = new GroupScraperResponseHandlerNew(paginationResponse, responseHandler.ObjFdScraperResponseParameters.ListGroup, responseHandler.PageletData);

                groupScraperResponseHandler.HasMoreResults = !string.IsNullOrEmpty(groupScraperResponseHandler.PageletData);

            }

            return groupScraperResponseHandler;
        }

        //public async Task<GroupScraperResponseHandler> ScrapGroupsAsync
        //    (DominatorAccountModel account,
        //     GroupScraperResponseHandler responseHandler, CancellationToken token)
        //{
        //    string url;

        //    GroupScraperResponseHandler groupScraperResponseHandler;

        //    NewsFeedPaginationResonseHandler paginationResponseHandler;

        //    FdRequestParameter objFdRequestParameter = new FdRequestParameter();

        //    if (responseHandler == null)
        //    {

        //        url = FdConstants.UpdateOwnGroupUrl;

        //        var searchFanpageResponse = await _httpHelper.GetRequestAsync(url, token);


        //        groupScraperResponseHandler = new GroupScraperResponseHandler(searchFanpageResponse, null);

        //        paginationResponseHandler = new NewsFeedPaginationResonseHandler(searchFanpageResponse, false, string.Empty);

        //        groupScraperResponseHandler.PageletData = paginationResponseHandler.PageletData;

        //        groupScraperResponseHandler.FinalEncodedQuery = paginationResponseHandler.ObjFdScraperResponseParameters.FinalEncodedQuery;

        //        if (!string.IsNullOrEmpty(groupScraperResponseHandler.PageletData))
        //            groupScraperResponseHandler.HasMoreResults = true;
        //    }
        //    else
        //    {



        //        var paginationData = responseHandler.PageletData;

        //        objFdRequestParameter.UrlParameters.Clear();

        //        objFdRequestParameter.UrlParameters.Add("dpr", "1");
        //        objFdRequestParameter.UrlParameters.Add("data", paginationData);
        //        objFdRequestParameter.UrlParameters.Add("__req", "10");
        //        objFdRequestParameter = CommonUrlParameters(account, objFdRequestParameter);

        //        url = objFdRequestParameter.GenerateUrl(FdConstants.FbFanpageLikerPageleUrl);

        //        var requestParameter = _httpHelper.GetRequestParameter();

        //        requestParameter.Referer = FdConstants.UpdateOwnGroupUrl;

        //        _httpHelper.SetRequestParameter(requestParameter);

        //        var paginationResponse = await _httpHelper.GetRequestAsync(url, token);

        //        if (paginationResponse.Response.Contains("errorSummary\":\""))
        //        {
        //            Task.Delay(TimeSpan.FromSeconds(2000)).Wait(token);
        //            paginationResponse = await _httpHelper.GetRequestAsync(url, token);
        //        }

        //        groupScraperResponseHandler = new GroupScraperResponseHandler(paginationResponse, responseHandler.ObjFdScraperResponseParameters.ListGroup);
        //        paginationResponseHandler = new NewsFeedPaginationResonseHandler(paginationResponse, true, responseHandler.FinalEncodedQuery);

        //        groupScraperResponseHandler.PageletData = paginationResponseHandler.PageletData;

        //        groupScraperResponseHandler.FinalEncodedQuery = paginationResponseHandler.ObjFdScraperResponseParameters.FinalEncodedQuery;

        //        if (!string.IsNullOrEmpty(groupScraperResponseHandler.PageletData))
        //            groupScraperResponseHandler.HasMoreResults = true;

        //    }

        //    return groupScraperResponseHandler;
        //}

        public async Task<GroupScraperResponseHandlerNew> ScrapGroupAsync(DominatorAccountModel account,
            GroupScraperResponseHandlerNew responseHandler, string groupType, CancellationToken token)
        {
            GroupScraperResponseHandlerNew groupScraperResponseHandler = null;

            try
            {
                FdRequestParameter objFdRequestParameter = new FdRequestParameter();

                string url = FdConstants.UpdateJoinedGroupUrl;

                url = objFdRequestParameter.GenerateUrl(FdConstants.GetCommentUrl);

                int count = 5000;

                int incrementCounters = 500;

                do
                {
                    await Task.Delay(1000);
                    objFdRequestParameter.PostDataParameters.Clear();
                    objFdRequestParameter.PostDataParameters.Add("variables", groupType == "Admin" ? "{\"count\":5000,\"isAdminGroups\":true}" :
                            groupScraperResponseHandler == null || string.IsNullOrEmpty(groupScraperResponseHandler.PageletData) ? "{\"count\":" + count + ",\"isAdminGroups\":false}" :
                            "{\"count\":" + count + ",\"isAdminGroups\":false,\"cursor\":\"" + groupScraperResponseHandler.PageletData + "\"}");
                    objFdRequestParameter.PostDataParameters.Add("fb_api_caller_class", "RelayModern");
                    objFdRequestParameter.PostDataParameters.Add("__hsi", "6780343469288922397-0");
                    objFdRequestParameter.PostDataParameters.Add("doc_id", "1309056129218188");
                    objFdRequestParameter.PostDataParameters.Add("fb_api_req_friendly_name", "GroupsLandingYourGroupsQuery");
                    objFdRequestParameter.PostDataParameters.Add("__req", "11");
                    objFdRequestParameter = CommonPostDataParameters(account, objFdRequestParameter);

                    var postData = objFdRequestParameter.GetPostDataFromParameters();

                    var request = _httpHelper.GetRequestParameter();

                    request.ContentType = FdConstants.ContentType;

                    _httpHelper.SetRequestParameter(request);

                    var searchFanpageResponse = await _httpHelper.PostRequestAsync(url, postData, token);

                    if (searchFanpageResponse.HasError)
                    {
                        Task.Delay(TimeSpan.FromSeconds(20)).Wait(token);
                        searchFanpageResponse = await _httpHelper.PostRequestAsync(url, postData, token);
                    }

                    if (searchFanpageResponse.Response.Contains("something went wrong") && count == 5000 && incrementCounters == 500)
                    {
                        count = 500;
                        continue;
                    }
                    else if (searchFanpageResponse.Response.Contains("something went wrong") && count < 5000 && incrementCounters == 500)
                    {
                        count = count - 450;
                        incrementCounters = 50;
                        continue;
                    }
                    else if (searchFanpageResponse.Response.Contains("something went wrong") && count < 5000 && incrementCounters == 50)
                    {
                        count = count - 45;
                        incrementCounters = 5;
                        continue;
                    }
                    else if (searchFanpageResponse.Response.Contains("something went wrong"))
                        break;

                    groupScraperResponseHandler = new GroupScraperResponseHandlerNew(searchFanpageResponse,
                        groupScraperResponseHandler != null && groupScraperResponseHandler.ObjFdScraperResponseParameters.ListGroup != null ?
                        groupScraperResponseHandler.ObjFdScraperResponseParameters.ListGroup : new List<GroupDetails>(), null, true, groupType);


                    count += incrementCounters;
                    //groupScraperResponseHandler.HasMoreResults = !string.IsNullOrEmpty(groupScraperResponseHandler.PageletData);

                } while (groupScraperResponseHandler == null || groupScraperResponseHandler.HasMoreResults);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return groupScraperResponseHandler;
        }


        /// <summary>
        /// UpdateOwnPagesAsyncronusly
        /// </summary>
        /// <param name="account"></param>
        /// <param name="responseHandler"></param>
        /// <param name="token"></param>
        public async Task<SearchOwnPageResponseHandler> UpdateOwnPagesAsync
            (DominatorAccountModel account,
            SearchOwnPageResponseHandler responseHandler, CancellationToken token)
        {
            string url;

            SearchOwnPageResponseHandler groupScraperResponseHandler;

            FdRequestParameter objFdRequestParameter = new FdRequestParameter();

            if (responseHandler == null)
            {



                var searchFanpageResponse = await _httpHelper.GetRequestAsync(FdConstants.UpdateOwnPageUrl, token);

                groupScraperResponseHandler = new SearchOwnPageResponseHandler(searchFanpageResponse, null);

                groupScraperResponseHandler.HasMoreResults = !string.IsNullOrEmpty(groupScraperResponseHandler.PageletData);
            }


            else
            {

                var paginationData = responseHandler.PageletData;

                objFdRequestParameter.UrlParameters.Clear();

                objFdRequestParameter.UrlParameters.Add("dpr", "1");
                objFdRequestParameter.UrlParameters.Add("data", paginationData);
                objFdRequestParameter.UrlParameters.Add("__req", "10");
                objFdRequestParameter = CommonUrlParameters(account, objFdRequestParameter);

                url = objFdRequestParameter.GenerateUrl(FdConstants.FbFanpageLikerPageleUrl);

                var paginationResponse = await _httpHelper.GetRequestAsync(url, token);

                groupScraperResponseHandler = new SearchOwnPageResponseHandler(paginationResponse, responseHandler.ObjFdScraperResponseParameters.ListPage);
                var paginationResponseHandler = new NewsFeedPaginationResonseHandler(paginationResponse, true, responseHandler.ObjFdScraperResponseParameters.FinalEncodedQuery);

                groupScraperResponseHandler.PageletData = paginationResponseHandler.PageletData;

                groupScraperResponseHandler.ObjFdScraperResponseParameters.FinalEncodedQuery = paginationResponseHandler.ObjFdScraperResponseParameters.FinalEncodedQuery;

                groupScraperResponseHandler.HasMoreResults = !string.IsNullOrEmpty(groupScraperResponseHandler.PageletData);

            }

            return groupScraperResponseHandler;
        }

        public IResponseHandler UpdateLikedPagesSync(DominatorAccountModel account,
            IResponseHandler responseHandler, CancellationToken token, bool isUpdate = false)
            => UpdateLikedPagesAsync(account, responseHandler, token, isUpdate).Result;

        /// <summary>
        /// UpdateLikedPagesAsyncronously
        /// </summary>
        /// <param name="account"></param>
        /// <param name="responseHandler"></param>
        /// <param name="token"></param>
        public async Task<IResponseHandler> UpdateLikedPagesAsync(DominatorAccountModel account,
            IResponseHandler responseHandler, CancellationToken token, bool isUpdate = false)
        {
            string url;
            SearchFanpageDetailsResponseHandler searchFanpageDetailsResponseHandler = null;
            FdRequestParameter objFdRequestParameter = new FdRequestParameter();

            Task.Delay(TimeSpan.FromSeconds(1)).Wait(token);

            #region Old Codes
            //NewsFeedPaginationResonseHandler paginationResponseHandler;
            //try
            //{
            //    if (responseHandler == null)
            //    {


            //        url = FdConstants.UpdateLikedPageUrl;

            //        var searchFanpageResponse = await _httpHelper.GetRequestAsync(url, token);


            //        searchFanpageDetailsResponseHandler = new SearchFanpageDetailsResponseHandler(searchFanpageResponse);

            //        paginationResponseHandler = new NewsFeedPaginationResonseHandler(searchFanpageResponse, false, string.Empty);

            //        searchFanpageDetailsResponseHandler.PaginationData = paginationResponseHandler.PaginationData;

            //        searchFanpageDetailsResponseHandler.FinalEncodedQuery = paginationResponseHandler.FinalEncodedQuery;

            //        if (!string.IsNullOrEmpty(searchFanpageDetailsResponseHandler.PaginationData))
            //            searchFanpageDetailsResponseHandler.HasMoreResults = true;
            //    }


            //    else
            //    {

            //        var paginationData = responseHandler.PaginationData;

            //        objFdRequestParameter.UrlParameters.Clear();

            //        objFdRequestParameter.UrlParameters.Add("dpr", "1");
            //        objFdRequestParameter.UrlParameters.Add("data", paginationData);
            //        objFdRequestParameter.UrlParameters.Add("__req", "10");
            //        objFdRequestParameter = CommonUrlParameters(account, objFdRequestParameter);

            //        url = objFdRequestParameter.GenerateUrl(FdConstants.FbFanpageLikerPageleUrl);

            //        var requestParameter = _httpHelper.GetRequestParameter();

            //        requestParameter.Referer = FdConstants.UpdateLikedPageUrl;

            //        _httpHelper.SetRequestParameter(requestParameter);

            //        var paginationResponse = await _httpHelper.GetRequestAsync(url, token);

            //        searchFanpageDetailsResponseHandler = new SearchFanpageDetailsResponseHandler(paginationResponse);
            //        paginationResponseHandler = new NewsFeedPaginationResonseHandler(paginationResponse, true, responseHandler.FinalEncodedQuery);

            //        searchFanpageDetailsResponseHandler.PaginationData = paginationResponseHandler.PaginationData;

            //        searchFanpageDetailsResponseHandler.FinalEncodedQuery = paginationResponseHandler.FinalEncodedQuery;

            //        if (!string.IsNullOrEmpty(searchFanpageDetailsResponseHandler.PaginationData))
            //            searchFanpageDetailsResponseHandler.HasMoreResults = true;

            //    } 


            //return searchFanpageDetailsResponseHandler;
            //}
            //catch (Exception ex)
            //{
            //    ex.DebugLog();
            //}
            #endregion

            try
            {

                if (responseHandler == null)
                {
                    url = objFdRequestParameter.GenerateUrl($"{FdConstants.FbHomeUrl}pages/?category=liked");
                    var searchFanpageResponse = await _httpHelper.GetRequestAsync(url, token);
                    searchFanpageDetailsResponseHandler = new SearchFanpageDetailsResponseHandler(searchFanpageResponse, isUpdate);

                    searchFanpageDetailsResponseHandler.HasMoreResults = !string.IsNullOrEmpty(searchFanpageDetailsResponseHandler.PageletData);

                }
                else
                {
                    var paginationData = responseHandler.PageletData;
                    objFdRequestParameter.UrlParameters.Clear();
                    objFdRequestParameter.UrlParameters.Add("dpr", "1");
                    objFdRequestParameter.UrlParameters.Add("data", paginationData);
                    objFdRequestParameter.UrlParameters.Add("__req", "10");
                    objFdRequestParameter = CommonUrlParameters(account, objFdRequestParameter);
                    url = objFdRequestParameter.GenerateUrl($"{FdConstants.FbHomeUrl}ajax/pagelet/generic.php/PageBrowserAllLikedPagesPagelet");

                    var requestParameter = _httpHelper.GetRequestParameter();
                    requestParameter.Referer = FdConstants.UpdateLikedPageUrl;
                    _httpHelper.SetRequestParameter(requestParameter);
                    var paginationResponse = await _httpHelper.GetRequestAsync(url, token);

                    searchFanpageDetailsResponseHandler = new SearchFanpageDetailsResponseHandler(paginationResponse);

                    searchFanpageDetailsResponseHandler.HasMoreResults = !string.IsNullOrEmpty(searchFanpageDetailsResponseHandler.PageletData);

                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return searchFanpageDetailsResponseHandler;
        }

        /// <summary>
        /// Method to get friend of friend
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="account"></param>
        /// <param name="responseHandler"></param>
        /// <returns></returns>
        public IResponseHandler GetFriendOfFriend
            (DominatorAccountModel account, string keyword, IResponseHandler responseHandler)
        {
            string url;


            FdRequestParameter objFdRequestParameter = new FdRequestParameter();

            if (responseHandler == null)
            {
                if (!keyword.Contains(FdConstants.FbHomeUrl))
                    url = FdConstants.FbFriendsPageUrl(keyword);
                else
                {
                    url = keyword;

                    var pageResponse = _httpHelper.GetRequest(url);

                    var userId = FdRegexUtility.FirstMatchExtractor(pageResponse.Response, "profileid=\"(.*?)\"");

                    userId = FdFunctions.GetIntegerOnlyString(userId);

                    url = FdConstants.FbFriendsPageUrl(userId);
                }
                var friendsPageResponse = _httpHelper.GetRequest(url);

                return new FdFriendOfFriendResponseHandler(friendsPageResponse, false, string.Empty);

            }
            else
            {
                objFdRequestParameter.UrlParameters.Clear();

                objFdRequestParameter.UrlParameters.Add("dpr", "1");
                objFdRequestParameter.UrlParameters.Add("data", responseHandler.PageletData);
                objFdRequestParameter.UrlParameters.Add("__req", "14");
                objFdRequestParameter = CommonUrlParameters(account, objFdRequestParameter);

                url = objFdRequestParameter.GenerateUrl(FdConstants.FbFriendsPageletUrl);

                var paginationResponse = _httpHelper.GetRequest(url);

                return new FdFriendOfFriendResponseHandler(paginationResponse, true, responseHandler.PageletData);
            }

        }

        /// <summary>
        /// Use this method to get group members
        /// </summary>
        /// <param name="account"></param>
        /// <param name="groupUrl"></param>
        /// <param name="responseHandler"></param>
        /// <param name="isGetOnlyFriends"></param>
        /// <returns></returns>
        public IResponseHandler GetGroupMembers
            (DominatorAccountModel account, string groupUrl, IResponseHandler responseHandler
            , GroupMemberCategory objMemberCategory = GroupMemberCategory.AllMembers)
        {
            FdRequestParameter objFdRequestParameter = new FdRequestParameter();

            GroupMembersResponseHandler groupMemberResponseHandler = null;

            string url;

            if (responseHandler == null)
            {
                url = !groupUrl.Contains(FdConstants.FbHomeUrl)
                    ? FdConstants.FbHomeUrl + groupUrl
                    : groupUrl;

                var groupPageResponse = _httpHelper.GetRequest(url);

                string groupId = string.Empty;
                try
                {
                    groupId = FdRegexUtility.FirstMatchExtractor(groupPageResponse.Response, FdConstants.GroupIdRegex);

                    if (string.IsNullOrEmpty(groupId))
                        groupId = FdRegexUtility.FirstMatchExtractor(groupPageResponse.Response, FdConstants.EntityIdRegex);

                    groupId = FdFunctions.GetIntegerOnlyString(groupId);
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
                if (string.IsNullOrEmpty(groupId))
                {

                    try
                    {
                        groupUrl = url.Split('?')[0];

                        if (!groupUrl.Contains("/about/"))
                        {
                            groupUrl = groupUrl[groupUrl.Length - 1] == '/'
                                ? groupUrl + "about/"
                                : groupUrl = groupUrl + "/about/";
                        }

                        groupPageResponse = _httpHelper.GetRequest(groupUrl);

                        groupId = FdRegexUtility.FirstMatchExtractor(groupPageResponse.Response, FdConstants.GroupIdRegex);

                        //groupId = FdRegexUtility.FirstMatchExtractor(groupPageResponse.Response, FdConstants.GroupIdRegex);

                        groupId = FdFunctions.GetIntegerOnlyString(groupId);

                        //if (!groupPageResponse.Response.Contains("Public group") && groupPageResponse.Response.Contains("aria-label=\"Join Group") )
                        //{

                        //}


                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                }

                url = FdConstants.FbGroupMemberPageUrl(groupId);

                if (objMemberCategory == GroupMemberCategory.Friends)
                    url = FdConstants.FbGroupMemberFrindsUrl(groupId);

                else if (objMemberCategory == GroupMemberCategory.MembersWithThingsInCommon)
                    url = FdConstants.FbGroupMemberWithCommonUrl(groupId);

                else if (objMemberCategory == GroupMemberCategory.LocalMembers)
                    url = FdConstants.FbGroupLocalMemberUrl(groupId);

                else if (objMemberCategory == GroupMemberCategory.AdminsAndModerators)
                    url = FdConstants.FbGroupAdminsAndModeratorUrl(groupId);

                var membersPageResponse = _httpHelper.GetRequest(url);

                groupMemberResponseHandler =
                    new GroupMembersResponseHandler(membersPageResponse, false, string.Empty, objMemberCategory) { EntityId = groupId };

            }
            else
            {

                try
                {
                    var paginationData = responseHandler.PageletData;

                    objFdRequestParameter.UrlParameters.Clear();

                    url = FdConstants.FbGroupPageletUrl;

                    objFdRequestParameter.UrlParameters.Add("gid", responseHandler.EntityId);
                    objFdRequestParameter.UrlParameters.Add("limit", "500");
                    objFdRequestParameter.UrlParameters.Add("av", account.AccountBaseModel.UserId);

                    if (objMemberCategory == GroupMemberCategory.Friends)
                    {
                        objFdRequestParameter.UrlParameters.Add("uid", account.AccountBaseModel.UserId);
                        objFdRequestParameter.UrlParameters.Add("order", "default");
                        objFdRequestParameter.UrlParameters.Add("cursor", paginationData);
                    }
                    else if (objMemberCategory == GroupMemberCategory.MembersWithThingsInCommon)
                    {
                        url = FdConstants.FbGroupDiscoverUrl;
                        objFdRequestParameter.UrlParameters.Add("sectiontype", "things_in_common");
                        objFdRequestParameter.UrlParameters.Add("aftercursor", paginationData);
                    }
                    else if (objMemberCategory == GroupMemberCategory.LocalMembers)
                    {
                        url = FdConstants.FbGroupDiscoverUrl;
                        objFdRequestParameter.UrlParameters.Add("aftercursor", paginationData);
                        objFdRequestParameter.UrlParameters.Add("sectiontype", "local_members");
                    }
                    else if (objMemberCategory == GroupMemberCategory.AdminsAndModerators)
                    {
                        url = FdConstants.FbGroupAdminModeratorUrl;
                        objFdRequestParameter.UrlParameters.Add("uid", account.AccountBaseModel.UserId);
                        objFdRequestParameter.UrlParameters.Add("order", "default");
                        objFdRequestParameter.UrlParameters.Add("cursor", paginationData);

                    }
                    else if (objMemberCategory == GroupMemberCategory.AllMembers)
                    {
                        objFdRequestParameter.UrlParameters.Add("sectiontype", "recently_joined");
                        objFdRequestParameter.UrlParameters.Add("cursor", paginationData);
                        objFdRequestParameter.UrlParameters.Add("order", "date");
                        objFdRequestParameter.UrlParameters.Add("view", "list");
                    }

                    objFdRequestParameter.UrlParameters.Add("start", "15");
                    objFdRequestParameter.UrlParameters.Add("dpr", "1");
                    objFdRequestParameter.UrlParameters.Add("__req", "t");
                    objFdRequestParameter = CommonUrlParameters(account, objFdRequestParameter);

                    url = objFdRequestParameter.GenerateUrl(url);

                    var paginationResponse = _httpHelper.GetRequest(url);

                    groupMemberResponseHandler = new GroupMembersResponseHandler(paginationResponse, true, paginationData, objMemberCategory) { EntityId = responseHandler.EntityId };
                }
                catch (Exception ex)
                {
                    ex.DebugLog(ex.Message);
                }
            }

            return groupMemberResponseHandler;
        }

        /// <summary>
        /// Use this method to get page likers
        /// </summary>
        /// <param name="account"></param>
        /// <param name="fanpageUrl"></param>
        /// <param name="responseHandler"></param>
        /// <returns></returns>
        public FanpageLikersResponseHandler GetPageLikers
            (DominatorAccountModel account, string fanpageUrl, FanpageLikersResponseHandler responseHandler)
        {
            FanpageLikersResponseHandler pageLikersResponseHandler = null;

            FdRequestParameter objFdRequestParameter = new FdRequestParameter();

            string url;

            if (responseHandler == null)
            {
                string pageId = GetPageIdFromUrl(account, fanpageUrl);

                url = FdConstants.FbFanpageLikersUrl(pageId);

                var pageLikersResponse = _httpHelper.GetRequest(url);

                pageLikersResponseHandler = new FanpageLikersResponseHandler(pageLikersResponse, null);

                if (!pageLikersResponseHandler.Status)
                {
                    pageLikersResponse = _httpHelper.GetRequest(url);

                    pageLikersResponseHandler = new FanpageLikersResponseHandler(pageLikersResponse, null);
                }

                //FileUtilities.Writestringtotextfile(@"C:\Users\GLB_266\AppData\Socinator\SavedData\SavedData.txt", pageLikersResponse.Response);
            }

            else
            {

                try
                {
                    Thread.Sleep(50);
                    responseHandler.ObjFdScraperResponseParameters.FdPageLikersParameters.IsPagination = true;
                    var paginationData = responseHandler.ObjFdScraperResponseParameters.FdPageLikersParameters.PaginationData;

                    objFdRequestParameter.UrlParameters.Clear();
                    objFdRequestParameter.UrlParameters.Add("dpr", "1");
                    objFdRequestParameter.UrlParameters.Add("data", paginationData);
                    objFdRequestParameter.UrlParameters.Add("__req", "10");
                    objFdRequestParameter = CommonUrlParameters(account, objFdRequestParameter);
                    url = objFdRequestParameter.GenerateUrl(FdConstants.FbFanpageLikerPageleUrl);

                    var paginationResponse = _httpHelper.GetRequest(url);

                    pageLikersResponseHandler = new FanpageLikersResponseHandler(paginationResponse, responseHandler.ObjFdScraperResponseParameters.FdPageLikersParameters);
                    //FileUtilities.Writestringtotextfile(@"C:\Users\GLB_266\AppData\Socinator\SavedData\SavedData.txt", paginationResponse.Response);
                }
                catch (Exception ex)
                {
                    ex.DebugLog(ex.Message);
                }
            }

            return pageLikersResponseHandler;
        }

        public FanpageLikersResponseHandler GetUserFollowers
            (DominatorAccountModel account, string userProfilleUrl, IResponseHandler responseHandler)
        {
            FanpageLikersResponseHandler pageLikersResponseHandler = null;

            FdRequestParameter objFdRequestParameter = new FdRequestParameter();
            string userId = string.Empty;
            string url;

            if (responseHandler == null)
            {
                url = !userProfilleUrl.Contains(FdConstants.FbHomeUrl)
                    ? $"{FdConstants.FbHomeUrl}{userProfilleUrl}"
                    : userProfilleUrl;

                var userPageResponse = _httpHelper.GetRequest(url);

                userId = FdRegexUtility.FirstMatchExtractor(userPageResponse.Response, FdConstants.EntityIdRegex);

                url = FdConstants.FbUserFollowersUrl(userId);

                var pageLikersResponse = _httpHelper.GetRequest(url);

                pageLikersResponseHandler = new FanpageLikersResponseHandler(pageLikersResponse, null);

                if (pageLikersResponseHandler.ObjFdScraperResponseParameters.FdPageLikersParameters.LstFacebookUser.Count == 0)
                {
                    url = userProfilleUrl + "?sk=followers";

                    if (!url.Contains(FdConstants.FbHomeUrl))
                        url = FdConstants.FbHomeUrl + userProfilleUrl;

                    var userFollowersResponse = _httpHelper.GetRequest(url);

                    pageLikersResponseHandler = new FanpageLikersResponseHandler(userFollowersResponse, null);

                    pageLikersResponseHandler.HasMoreResults = true;
                }


                //FileUtilities.Writestringtotextfile(@"C:\Users\GLB_266\AppData\Socinator\SavedData\SavedData.txt", pageLikersResponse.Response);
            }

            else
            {
                try
                {
                    responseHandler.ObjFdScraperResponseParameters.IsPagination = true;
                    var paginationData = responseHandler.PageletData;
                    userId = responseHandler.ObjFdScraperResponseParameters.FacebookUser.UserId;

                    objFdRequestParameter.UrlParameters.Clear();
                    objFdRequestParameter.UrlParameters.Add("dpr", "1");
                    objFdRequestParameter.UrlParameters.Add("data", paginationData);
                    objFdRequestParameter.UrlParameters.Add("__req", "10");
                    objFdRequestParameter = CommonUrlParameters(account, objFdRequestParameter);
                    url = objFdRequestParameter.GenerateUrl(FdConstants.FbFanpageLikerPageleUrl);

                    var paginationResponse = _httpHelper.GetRequest(url);

                    pageLikersResponseHandler = new FanpageLikersResponseHandler(paginationResponse, responseHandler.ObjFdScraperResponseParameters.FdPageLikersParameters);
                    //FileUtilities.Writestringtotextfile(@"C:\Users\GLB_266\AppData\Socinator\SavedData\SavedData.txt", paginationResponse.Response);

                    if (pageLikersResponseHandler.ObjFdScraperResponseParameters.ListUser == null)
                    {
                        //for pagination userId is not coming
                        userId = string.IsNullOrEmpty(userId)
                            ? FdRegexUtility.FirstMatchExtractor(paginationData, "profile_id=(.*?)&")
                            : userId;

                        objFdRequestParameter.UrlParameters.Clear();
                        objFdRequestParameter.UrlParameters.Add("__req", "m");
                        objFdRequestParameter = CommonUrlParameters(account, objFdRequestParameter);
                        url = objFdRequestParameter.GenerateUrl($"{FdConstants.FbHomeUrl}{paginationData}");
                        objFdRequestParameter.UrlParameters.Remove("fb_dtsg");
                        objFdRequestParameter.UrlParameters.Add("fb_dtsg_ag", Uri.UnescapeDataString(account.SessionId));
                        paginationResponse = _httpHelper.GetRequest(url);

                        pageLikersResponseHandler = new FanpageLikersResponseHandler(paginationResponse, responseHandler.ObjFdScraperResponseParameters.FdPageLikersParameters);

                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog(ex.Message);
                }
            }

            return pageLikersResponseHandler;
        }

        /// <summary>
        /// Search People From Keyword
        /// </summary>
        /// <param name="account"></param>
        /// <param name="keyword"></param>
        /// <param name="responseHandler"></param>
        /// <returns></returns>
        public IResponseHandler SearchPeopleFromKeyword
            (DominatorAccountModel account, string keyword, IResponseHandler responseHandler)
        {
            string url;

            FdSearchPeopleResponseHandler searchPeopleResponseHandler = null;

            NewsFeedPaginationResonseHandler paginationResponseHandler;

            FdRequestParameter objFdRequestParameter = new FdRequestParameter();

            if (responseHandler == null)
            {

                objFdRequestParameter.UrlParameters.Add("q", keyword);

                url = objFdRequestParameter.GenerateUrl(FdConstants.FbPeopleSearchUrl);

                var searchPeopleResponse = _httpHelper.GetRequest(url);


                searchPeopleResponseHandler = new FdSearchPeopleResponseHandler(searchPeopleResponse);
                paginationResponseHandler = new NewsFeedPaginationResonseHandler(searchPeopleResponse, false, string.Empty);

                searchPeopleResponseHandler.ObjFdScraperResponseParameters.FinalEncodedQuery = paginationResponseHandler.ObjFdScraperResponseParameters.FinalEncodedQuery;
                searchPeopleResponseHandler.PageletData = paginationResponseHandler.PageletData;

                searchPeopleResponseHandler.HasMoreResults = !string.IsNullOrEmpty(searchPeopleResponseHandler.PageletData);
            }

            else
            {

                try
                {
                    //paginationResponseHandler.IsPagination = true;
                    var paginationData = responseHandler.PageletData;

                    objFdRequestParameter.UrlParameters.Clear();

                    objFdRequestParameter.UrlParameters.Add("dpr", "1");
                    objFdRequestParameter.UrlParameters.Add("data", paginationData);
                    objFdRequestParameter.UrlParameters.Add("__req", "10");
                    objFdRequestParameter = CommonUrlParameters(account, objFdRequestParameter);

                    url = objFdRequestParameter.GenerateUrl(FdConstants.FbFanpageLikerPageleUrl);

                    var paginationResponse = _httpHelper.GetRequest(url);


                    searchPeopleResponseHandler = new FdSearchPeopleResponseHandler(paginationResponse);
                    paginationResponseHandler = new NewsFeedPaginationResonseHandler(paginationResponse, true,
                        responseHandler.ObjFdScraperResponseParameters.FinalEncodedQuery);

                    searchPeopleResponseHandler.ObjFdScraperResponseParameters.FinalEncodedQuery = paginationResponseHandler.ObjFdScraperResponseParameters.FinalEncodedQuery;
                    searchPeopleResponseHandler.PageletData = paginationResponseHandler.PageletData;

                    searchPeopleResponseHandler.HasMoreResults = !string.IsNullOrEmpty(searchPeopleResponseHandler.PageletData);
                }
                catch (Exception ex)
                {
                    ex.DebugLog(ex.Message);
                }
            }


            return searchPeopleResponseHandler;
        }

        /// <summary>
        /// Search People From Graph Search
        /// </summary>
        /// <param name="account"></param>
        /// <param name="graphSearchUrl"></param>
        /// <param name="responseHandler"></param>
        /// <returns></returns>
        public IResponseHandler SearchPeopleFromGraphSearch
            (DominatorAccountModel account, string graphSearchUrl, IResponseHandler responseHandler, CancellationToken token)
        {
            FdSearchPeopleResponseHandler searchPeopleResponseHandler = null;

            NewsFeedPaginationResonseHandler paginationResponseHandler;

            FdRequestParameter objFdRequestParameter = new FdRequestParameter();

            if (responseHandler == null)
            {

                var searchPeopleResponse = _httpHelper.GetRequest(graphSearchUrl);


                searchPeopleResponseHandler = new FdSearchPeopleResponseHandler(searchPeopleResponse);
                paginationResponseHandler = new NewsFeedPaginationResonseHandler(searchPeopleResponse, false, string.Empty);

                searchPeopleResponseHandler.ObjFdScraperResponseParameters.FinalEncodedQuery = paginationResponseHandler.ObjFdScraperResponseParameters.FinalEncodedQuery;
                searchPeopleResponseHandler.PageletData = paginationResponseHandler.PageletData;

                searchPeopleResponseHandler.HasMoreResults = !string.IsNullOrEmpty(searchPeopleResponseHandler.PageletData);
            }

            else
            {

                try
                {
                    //paginationResponseHandler.IsPagination = true;
                    var paginationData = responseHandler.PageletData;

                    objFdRequestParameter.UrlParameters.Clear();

                    objFdRequestParameter.UrlParameters.Add("dpr", "1");
                    objFdRequestParameter.UrlParameters.Add("data", paginationData);
                    objFdRequestParameter.UrlParameters.Add("__req", "10");
                    objFdRequestParameter = CommonUrlParameters(account, objFdRequestParameter);

                    var url = objFdRequestParameter.GenerateUrl(FdConstants.FbFanpageLikerPageleUrl);

                    var paginationResponse = _httpHelper.GetRequest(url);

                    if (paginationResponse.Response.Contains("errorSummary") || paginationResponse.Response.Contains("Security Check Required"))
                    {
                        Task.Delay(TimeSpan.FromSeconds(3000)).Wait(token);

                        paginationResponse = _httpHelper.GetRequest(url);
                    }

                    searchPeopleResponseHandler = new FdSearchPeopleResponseHandler(paginationResponse);
                    paginationResponseHandler = new NewsFeedPaginationResonseHandler(paginationResponse, true, responseHandler.ObjFdScraperResponseParameters.FinalEncodedQuery);

                    searchPeopleResponseHandler.ObjFdScraperResponseParameters.FinalEncodedQuery = paginationResponseHandler.ObjFdScraperResponseParameters.FinalEncodedQuery;
                    searchPeopleResponseHandler.PageletData = paginationResponseHandler.PageletData;

                    if (!string.IsNullOrEmpty(searchPeopleResponseHandler.PageletData))
                        searchPeopleResponseHandler.HasMoreResults = true;
                }
                catch (Exception ex)
                {
                    ex.DebugLog(ex.Message);
                }
            }


            return searchPeopleResponseHandler;
        }

        /// <summary>
        /// Search People By Location
        /// </summary>
        /// <param name="account"></param>
        /// <param name="location"></param>
        /// <param name="responseHandler"></param>
        /// <returns></returns>
        public IResponseHandler SearchPeopleByLocation
            (DominatorAccountModel account, string location, IResponseHandler responseHandler)
        {
            string url;

            FdSearchPeopleResponseHandler searchPeopleResponseHandler = null;

            NewsFeedPaginationResonseHandler paginationResponseHandler;

            FdRequestParameter objFdRequestParameter = new FdRequestParameter();

            if (responseHandler == null)
            {
                var locationId = GetLocationCityId(account, Uri.EscapeDataString(location));

                if (string.IsNullOrEmpty(locationId))
                {
                    objFdRequestParameter.UrlParameters.Add("q", $"people in {location}");

                    url = objFdRequestParameter.GenerateUrl(FdConstants.FbPeopleSearchUrl);

                    var searchPeopleResponse = _httpHelper.GetRequest(url);

                    searchPeopleResponseHandler = new FdSearchPeopleResponseHandler(searchPeopleResponse);

                    paginationResponseHandler = new NewsFeedPaginationResonseHandler(searchPeopleResponse, false, string.Empty);

                }
                else
                {
                    url = FdConstants.FbPeopleSearchByLocation(locationId);

                    var searchPeopleResponse = _httpHelper.GetRequest(url);

                    searchPeopleResponseHandler = new FdSearchPeopleResponseHandler(searchPeopleResponse);

                    paginationResponseHandler = new NewsFeedPaginationResonseHandler(searchPeopleResponse, false, string.Empty);

                }


                searchPeopleResponseHandler.ObjFdScraperResponseParameters.FinalEncodedQuery = paginationResponseHandler.ObjFdScraperResponseParameters.FinalEncodedQuery;
                searchPeopleResponseHandler.PageletData = paginationResponseHandler.PageletData;

                searchPeopleResponseHandler.HasMoreResults = !string.IsNullOrEmpty(searchPeopleResponseHandler.PageletData);
            }

            else
            {

                try
                {
                    //paginationResponseHandler.IsPagination = true;
                    var paginationData = responseHandler.PageletData;

                    objFdRequestParameter.UrlParameters.Clear();

                    objFdRequestParameter.UrlParameters.Add("dpr", "1");
                    objFdRequestParameter.UrlParameters.Add("data", paginationData);
                    objFdRequestParameter.UrlParameters.Add("__req", "10");
                    objFdRequestParameter = CommonUrlParameters(account, objFdRequestParameter);

                    url = objFdRequestParameter.GenerateUrl(FdConstants.FbFanpageLikerPageleUrl);

                    var paginationResponse = _httpHelper.GetRequest(url);


                    searchPeopleResponseHandler = new FdSearchPeopleResponseHandler(paginationResponse);
                    paginationResponseHandler = new NewsFeedPaginationResonseHandler(paginationResponse, true, responseHandler.ObjFdScraperResponseParameters.FinalEncodedQuery);

                    searchPeopleResponseHandler.ObjFdScraperResponseParameters.FinalEncodedQuery = paginationResponseHandler.ObjFdScraperResponseParameters.FinalEncodedQuery;
                    searchPeopleResponseHandler.PageletData = paginationResponseHandler.PageletData;

                    searchPeopleResponseHandler.HasMoreResults = !string.IsNullOrEmpty(searchPeopleResponseHandler.PageletData);
                }
                catch (Exception ex)
                {
                    ex.DebugLog(ex.Message);
                }
            }


            return searchPeopleResponseHandler;
        }

        public string GetLocationCityId(DominatorAccountModel account, string location)
            => new LocationResponseHandler(_httpHelper.GetRequest(FdConstants.LocationUrl(location))).LocationId;

        /// <summary>
        /// Use this method to get post sharer
        /// </summary>
        /// <param name="account"></param>
        /// <param name="postUrl"></param>
        /// <param name="responseHandler"></param>
        /// <returns></returns>
        public IResponseHandler GetPostSharer
            (DominatorAccountModel account, string postUrl, IResponseHandler responseHandler)
        {
            string url = string.Empty;

            FdRequestParameter objFdRequestParameter = new FdRequestParameter();

            PostSharerResponseHandler postLikersResponseHandler = null;

            if (responseHandler == null)
            {

                url = !postUrl.Contains(FdConstants.FbHomeUrl)
                    ? FdConstants.FbHomeUrl + postUrl
                    : postUrl;

                var postUrlResponse = _httpHelper.GetRequest(url);

                var postId = new PostIdResponseHandler(postUrlResponse).PostId;

                objFdRequestParameter.UrlParameters.Add("target_fbid", postId);
                objFdRequestParameter.UrlParameters.Add("av", account.AccountBaseModel.UserId);
                objFdRequestParameter.UrlParameters.Add("dpr", "1");
                objFdRequestParameter.UrlParameters.Add("__asyncDialog", "4");
                objFdRequestParameter.UrlParameters.Add("__req", "1e");
                objFdRequestParameter = CommonUrlParameters(account, objFdRequestParameter);

                url = objFdRequestParameter.GenerateUrl(FdConstants.FbPostSharerUrl);

                var postLikersResponse = _httpHelper.GetRequest(url);


                postLikersResponseHandler = new PostSharerResponseHandler(postLikersResponse, true);

                if (!postLikersResponseHandler.Status)
                {
                    FdHttpHelper httpHelper = new FdHttpHelper();
                    IRequestParameters requestParameter = _httpHelper.GetRequestParameter();
                    var cookies = requestParameter.Cookies;
                    requestParameter.Cookies = new CookieCollection();
                    httpHelper.SetRequestParameter(requestParameter);
                    var response = httpHelper.GetRequest(postUrl);
                    //set Request Parameters
                    requestParameter.Cookies = cookies;
                    httpHelper.SetRequestParameter(requestParameter);

                    var ftEntIdentifier = new PostIdResponseHandler(response).PostId;

                    objFdRequestParameter = new FdRequestParameter();
                    objFdRequestParameter.UrlParameters.Add("target_fbid", ftEntIdentifier);
                    objFdRequestParameter.UrlParameters.Add("av", account.AccountBaseModel.UserId);
                    objFdRequestParameter.UrlParameters.Add("dpr", "1");
                    objFdRequestParameter.UrlParameters.Add("__asyncDialog", "4");
                    objFdRequestParameter.UrlParameters.Add("__req", "1e");
                    objFdRequestParameter = CommonUrlParameters(account, objFdRequestParameter);

                    url = objFdRequestParameter.GenerateUrl(FdConstants.FbPostSharerUrl);

                    postLikersResponse = _httpHelper.GetRequest(url);

                    postLikersResponseHandler = new PostSharerResponseHandler(postLikersResponse, true);

                    postLikersResponseHandler.EntityId = ftEntIdentifier;
                }

            }
            else
            {

                try
                {
                    var paginationData = responseHandler.PageletData;

                    objFdRequestParameter.UrlParameters.Clear();

                    objFdRequestParameter.UrlParameters.Add("dpr", "1");
                    objFdRequestParameter.UrlParameters.Add("data", paginationData);
                    objFdRequestParameter.UrlParameters.Add("__req", "1e");
                    objFdRequestParameter = CommonUrlParameters(account, objFdRequestParameter);

                    url = objFdRequestParameter.GenerateUrl(FdConstants.FbPostSharerPgeletUrl);

                    var paginationResponse = _httpHelper.GetRequest(url);


                    postLikersResponseHandler = new PostSharerResponseHandler(paginationResponse, false);

                }
                catch (Exception ex)
                {
                    ex.DebugLog(ex.Message);
                }
            }


            return postLikersResponseHandler;

        }

        /// <summary>
        /// Use this method to get post commentor
        /// </summary>
        /// <param name="account"></param>
        /// <param name="postUrl"></param>
        /// <param name="responseHandler"></param>
        /// <returns></returns>

        string commentCount = string.Empty;
        string PostIdTemp = string.Empty;
        public IResponseHandler GetPostCommentor(DominatorAccountModel account, string postUrl,
                 IResponseHandler responseHandler, CancellationToken token)
        {
            int length;

            PostCommentorResponseHandler postLikersResponseHandler = null;

            FdRequestParameter objFdRequestParameter = new FdRequestParameter();

            var offset = 0;

            var url = postUrl.Contains(FdConstants.FbHomeUrl)
                ? postUrl
                : FdConstants.FbHomeUrl + postUrl;

            var postId = string.Empty;

            var paginationData = string.Empty;

            FacebookPostDetails objFacebookPostDetails = null;

            if (responseHandler == null)
            {

                var postUrlResponse = _httpHelper.GetRequest(url);

                if (postUrlResponse.HasError)
                {
                    Task.Delay(TimeSpan.FromMilliseconds(1000)).Wait(token);
                    postUrlResponse = _httpHelper.GetRequest(url);
                }

                postId = new PostIdResponseHandler(postUrlResponse).PostId;


                if (string.IsNullOrEmpty(postId))
                {
                    HttpHelper httpHelper = new FdHttpHelper();
                    IRequestParameters requestParameter = _httpHelper.GetRequestParameter();
                    var cookies = _httpHelper.GetRequestParameter().Cookies;
                    requestParameter.Cookies = new CookieCollection();
                    httpHelper.SetRequestParameter(requestParameter);
                    var response = httpHelper.GetRequest(postUrl);
                    postId = new PostIdResponseHandler(response).PostId;
                    commentCount = FdFunctions.GetIntegerOnlyString
                        (FdRegexUtility.FirstMatchExtractor(response.Response, "comment_count:{total_count:(.*?)}"));
                    requestParameter.Cookies = cookies;
                    _httpHelper.SetRequestParameter(requestParameter);
                }

                if (string.IsNullOrEmpty(postId))
                {
                    url = FdConstants.FbHomeUrl + FdRegexUtility.FirstMatchExtractor(postUrlResponse.Response, "URL=/(.*?)\"");

                    if (url.Contains("www.facebook.com/watch/"))
                        postId = Utilities.GetBetween(url, "watch/?v=", "&");
                }

                postLikersResponseHandler = new PostCommentorResponseHandler(postUrlResponse, true, postId,
                    responseHandler != null ? responseHandler.ObjFdScraperResponseParameters.FeedLocation : string.Empty, false, postUrl);

                if (postLikersResponseHandler.ObjFdScraperResponseParameters.CommentList?.Count == 0)
                {
                    try
                    {
                        IFdHttpHelper httpHelper = new FdHttpHelper();
                        var requestParameter = _httpHelper.GetRequestParameter();
                        var cookies = _httpHelper.GetRequestParameter().Cookies;
                        objFdRequestParameter.Cookies = new CookieCollection();
                        httpHelper.SetRequestParameter(requestParameter);

                        objFdRequestParameter.UrlParameters.Clear();
                        objFdRequestParameter.PostDataParameters.Clear();

                        objFdRequestParameter.PostDataParameters.Add("__req", "27");
                        CommonPostDataParameters(account, objFdRequestParameter);

                        postUrl = $"{FdConstants.FbHomeUrl}video/tahoe/async/{postId}/?originalmediaid={postId}&playerorigin=video_home&playersuborigin=permalink&feedtracking[0]=%7B%22ft%22%3A%7B%7D%7D&numcopyrightmatchedvideoplayedconsecutively=0&payloadtype=secondary";
                        var postData = objFdRequestParameter.GetPostDataFromParameters();
                        objFdRequestParameter.ContentType = FdConstants.ContentType;
                        objFdRequestParameter.Cookies = cookies;
                        httpHelper.SetRequestParameter(objFdRequestParameter);
                        postUrlResponse = httpHelper.PostRequest(postUrl, postData);

                        postLikersResponseHandler = new PostCommentorResponseHandler(postUrlResponse, true, postId, responseHandler != null
                        ? responseHandler.ObjFdScraperResponseParameters.FeedLocation
                        : string.Empty, false, postUrl);

                        commentCount = FdRegexUtility.FirstMatchExtractor(postUrlResponse.Response,
                        "\"count\":(.*?),");
                    }
                    catch (Exception)
                    {

                    }
                }


                objFacebookPostDetails = new FacebookPostDetails() { Id = postId };
                GetPostDetails(account, objFacebookPostDetails);
            }

            if (postLikersResponseHandler != null && postLikersResponseHandler.ObjFdScraperResponseParameters.CommentList != null
                                                  && postLikersResponseHandler.ObjFdScraperResponseParameters.CommentList.Count() > 0)
            {
                postLikersResponseHandler.ObjFdScraperResponseParameters.PostDetails = objFacebookPostDetails;
                postLikersResponseHandler.ObjFdScraperResponseParameters.Length = 0;
                postLikersResponseHandler.ObjFdScraperResponseParameters.Offset = 0;
                postLikersResponseHandler.ObjFdScraperResponseParameters.IsFirstPage = true;
                postLikersResponseHandler.HasMoreResults = true;
                return postLikersResponseHandler;
            }

            try
            {
                length = 50;
                PostIdTemp = responseHandler.EntityId;

                if (responseHandler != null && responseHandler.ObjFdScraperResponseParameters.PostDetails != null &&
                    !string.IsNullOrEmpty(responseHandler.ObjFdScraperResponseParameters.PostDetails.Id))
                {
                    offset = responseHandler.ObjFdScraperResponseParameters.IsIncreasingOrder ? responseHandler.ObjFdScraperResponseParameters.Offset - 50 :
                        responseHandler.ObjFdScraperResponseParameters.IsFirstPage ? 0 : responseHandler.ObjFdScraperResponseParameters.Offset + 50;

                    postId = responseHandler.ObjFdScraperResponseParameters.PostDetails.Id;

                    paginationData = string.IsNullOrEmpty(responseHandler.ObjFdScraperResponseParameters.FeedContext)
                        ? string.Empty
                        : responseHandler.ObjFdScraperResponseParameters.FeedContext;

                }

                objFdRequestParameter.PostDataParameters.Clear();

                if (responseHandler != null && !string.IsNullOrEmpty(responseHandler.PageletData))
                {
                    url = FdConstants.GetCommentUrl;

                    objFdRequestParameter.PostDataParameters.Add("av", account.AccountBaseModel.UserId);
                    objFdRequestParameter.PostDataParameters.Add("fb_api_caller_class", "RelayModern");
                    objFdRequestParameter.PostDataParameters.Add("variables", responseHandler.PageletData);
                    objFdRequestParameter.PostDataParameters.Add("doc_id", "1997037697012760");

                }
                else
                {
                    url = FdConstants.FbCommentPaginationUrl;

                    if (!string.IsNullOrEmpty(commentCount))
                        offset = int.Parse(commentCount) - 50;

                    objFdRequestParameter.PostDataParameters.Add("ft_ent_identifier", PostIdTemp);
                    objFdRequestParameter.PostDataParameters.Add("viewas", "");
                    objFdRequestParameter.PostDataParameters.Add("source", "17");
                    objFdRequestParameter.PostDataParameters.Add("offset", offset.ToString());
                    objFdRequestParameter.PostDataParameters.Add("length", length.ToString());
                    objFdRequestParameter.PostDataParameters.Add("orderingmode", "ranked_unfiltered");
                    objFdRequestParameter.PostDataParameters.Add("section", "default");
                    objFdRequestParameter.PostDataParameters.Add("direction", "bottom");
                    objFdRequestParameter.PostDataParameters.Add("feed_context", Uri.EscapeDataString(paginationData));
                    objFdRequestParameter.PostDataParameters.Add("numpagerclicks", "1");
                    objFdRequestParameter.PostDataParameters.Add("av", account.AccountBaseModel.UserId);
                    objFdRequestParameter.PostDataParameters.Add("__req", "11");

                }

                objFdRequestParameter = CommonPostDataParameters(account, objFdRequestParameter);

                var postData = objFdRequestParameter.GetPostDataFromParameters();

                var request = _httpHelper.GetRequestParameter();

                request.ContentType = FdConstants.ContentType;

                _httpHelper.SetRequestParameter(request);



                var paginationResponse = _httpHelper.PostRequest(url, postData);

                postLikersResponseHandler = new PostCommentorResponseHandler(paginationResponse, true, postId,
                    responseHandler != null ? responseHandler.ObjFdScraperResponseParameters.FeedLocation : string.Empty, false, postUrl, PostIdTemp);


                if (responseHandler == null && postLikersResponseHandler.ObjFdScraperResponseParameters.CommentList != null && postLikersResponseHandler.ObjFdScraperResponseParameters.CommentList.Count != 0 &&
                    offset != 0)
                {
                    postLikersResponseHandler.ObjFdScraperResponseParameters.CommentCount = int.Parse(FdFunctions.GetIntegerOnlyString(commentCount));

                    postLikersResponseHandler.ObjFdScraperResponseParameters.Offset = postLikersResponseHandler.ObjFdScraperResponseParameters.CommentCount - 51;

                    postLikersResponseHandler.ObjFdScraperResponseParameters.IsIncreasingOrder = true;
                }

                if (responseHandler == null && postLikersResponseHandler.ObjFdScraperResponseParameters.CommentList != null &&
                    postLikersResponseHandler.ObjFdScraperResponseParameters.CommentList.Count == 0)
                {
                    commentCount = FdRegexUtility.FirstMatchExtractor(paginationResponse.Response,
                        "\"count\":(.*?),");
                    int commentCountTemp = 0;
                    int.TryParse(FdFunctions.GetIntegerOnlyString(commentCount), out commentCountTemp);
                    postLikersResponseHandler.ObjFdScraperResponseParameters.Offset = commentCountTemp - 51;

                    objFdRequestParameter.PostDataParameters.Remove("offset");
                    objFdRequestParameter.PostDataParameters.Add("offset", postLikersResponseHandler.ObjFdScraperResponseParameters.Offset.ToString());

                    postData = objFdRequestParameter.GetPostDataFromParameters();
                    paginationResponse = _httpHelper.PostRequest(url, postData);

                    var reactionCountResponse = new ReactionCountResponseHandler(paginationResponse, ref postId);

                    postLikersResponseHandler = new PostCommentorResponseHandler(paginationResponse, true, postId,
                        responseHandler != null ? responseHandler.ObjFdScraperResponseParameters.FeedLocation : string.Empty, false, postUrl);
                    int.TryParse(FdFunctions.GetIntegerOnlyString(commentCount), out commentCountTemp);
                    postLikersResponseHandler.ObjFdScraperResponseParameters.CommentCount = commentCountTemp;

                    postLikersResponseHandler.ObjFdScraperResponseParameters.Offset = postLikersResponseHandler.ObjFdScraperResponseParameters.CommentCount - 51;

                    postLikersResponseHandler.ObjFdScraperResponseParameters.IsIncreasingOrder = true;

                }

                if (postLikersResponseHandler.ObjFdScraperResponseParameters.CommentList != null
                    && postLikersResponseHandler.ObjFdScraperResponseParameters.CommentList.Count == 0
                  && objFdRequestParameter.PostDataParameters.ContainsKey("orderingmode"))
                {
                    objFdRequestParameter.PostDataParameters.Remove("ft_ent_identifier");
                    objFdRequestParameter.PostDataParameters.Add("ft_ent_identifier", PostIdTemp);
                    objFdRequestParameter.PostDataParameters.Remove("orderingmode");
                    objFdRequestParameter.PostDataParameters.Add("orderingmode", "ranked_threaded");
                    objFdRequestParameter.PostDataParameters.Add("ircid", "f096506c-f7e7-4dea-8fa8-078d94161543");
                    postData = objFdRequestParameter.GetPostDataFromParameters();
                    paginationResponse = _httpHelper.PostRequest(url, postData);

                    var reactionCountResponse = new ReactionCountResponseHandler(paginationResponse, ref postId);

                    postLikersResponseHandler = new PostCommentorResponseHandler(paginationResponse, true, postId,
                        responseHandler != null ? responseHandler.ObjFdScraperResponseParameters.FeedLocation
                        : string.Empty, false, postUrl, PostIdTemp);
                }

                if (responseHandler != null && responseHandler.ObjFdScraperResponseParameters.IsIncreasingOrder)
                {
                    postLikersResponseHandler.ObjFdScraperResponseParameters.CommentCount = responseHandler.ObjFdScraperResponseParameters.CommentCount;
                    postLikersResponseHandler.ObjFdScraperResponseParameters.Offset = offset;
                    postLikersResponseHandler.ObjFdScraperResponseParameters.IsIncreasingOrder = responseHandler.ObjFdScraperResponseParameters.IsIncreasingOrder;
                }
                else if (responseHandler != null)
                {
                    postLikersResponseHandler.ObjFdScraperResponseParameters.Offset = offset;
                    postLikersResponseHandler.ObjFdScraperResponseParameters.IsFirstPage = false;
                }

                postLikersResponseHandler.ObjFdScraperResponseParameters.Length = 50;

                postLikersResponseHandler.ObjFdScraperResponseParameters.PostDetails = responseHandler != null && responseHandler.ObjFdScraperResponseParameters.PostDetails != null ?
                    responseHandler.ObjFdScraperResponseParameters.PostDetails : objFacebookPostDetails;

                postLikersResponseHandler.HasMoreResults
                    = postLikersResponseHandler.ObjFdScraperResponseParameters.CommentList != null
                        && postLikersResponseHandler.ObjFdScraperResponseParameters.CommentList.Count() > 0;

            }
            catch (Exception ex)
            {
                ex.DebugLog(ex.Message);
            }

            return postLikersResponseHandler;

        }


        public IResponseParameter GetCommentResponseforVideos(DominatorAccountModel account, string postId, string storyIdentifier)
        {
            FdRequestParameter objFdRequestParameter = new FdRequestParameter();


            objFdRequestParameter.UrlParameters.Add("originalmediaid", postId);
            objFdRequestParameter.UrlParameters.Add("playerorigin", "permalink");
            objFdRequestParameter.UrlParameters.Add("playersuborigin", "tahoe");
            objFdRequestParameter.UrlParameters.Add("ispermalink", "true");
            objFdRequestParameter.UrlParameters.Add("numcopyrightmatchedvideoplayedconsecutively", "0");
            objFdRequestParameter.UrlParameters.Add("storyidentifier", storyIdentifier);
            objFdRequestParameter.UrlParameters.Add("payloadtype", "secondary");
            objFdRequestParameter.UrlParameters.Add("dpr", "1");

            var postUrl = FdConstants.PostVideoCommentUrl(postId);

            postUrl = objFdRequestParameter.GenerateUrl(postUrl);

            objFdRequestParameter.PostDataParameters.Clear();

            objFdRequestParameter.PostDataParameters.Add("__req", "11");

            objFdRequestParameter = CommonPostDataParameters(account, objFdRequestParameter);

            var postData = objFdRequestParameter.GetPostDataFromParameters();

            var request = _httpHelper.GetRequestParameter();

            request.ContentType = FdConstants.ContentType;

            _httpHelper.SetRequestParameter(request);

            return _httpHelper.PostRequest(postUrl, postData);
        }

        /// <summary>
        /// Get All Post Likers
        /// </summary>
        /// <param name="account"></param>
        /// <param name="postUrl"></param>
        /// <param name="responseHandler"></param>
        /// <returns></returns>
        public IResponseHandler GetPostLikers
            (DominatorAccountModel account, string postUrl, IResponseHandler responseHandler)
        {
            var postId = string.Empty;

            PostLikersResponseHandler postLikersResponseHandler = null;

            FdRequestParameter objFdRequestParameter = new FdRequestParameter();

            var url = !postUrl.Contains(FdConstants.FbHomeUrl)
                ? FdConstants.FbHomeUrl + postUrl
                : postUrl;


            if (responseHandler == null)
            {
                var postUrlResponse = _httpHelper.GetRequest(url);

                postId = new PostIdResponseHandler(postUrlResponse).PostId;

                objFdRequestParameter = new FdRequestParameter();
                objFdRequestParameter.UrlParameters.Add("ft_ent_identifier", postId);
                objFdRequestParameter.UrlParameters.Add("av", account.AccountBaseModel.UserId);
                objFdRequestParameter.UrlParameters.Add("dpr", "1");
                objFdRequestParameter.UrlParameters.Add("__asyncDialog", "3");
                objFdRequestParameter.UrlParameters.Add("__req", "1e");

                objFdRequestParameter = CommonUrlParameters(account, objFdRequestParameter);

                url = objFdRequestParameter.GenerateUrl(FdConstants.FbPostLikersUrl);

                var postLikersResponse = _httpHelper.GetRequest(url);


                postLikersResponseHandler = new PostLikersResponseHandler(postLikersResponse);


                if (!postLikersResponseHandler.Status)
                {
                    FdHttpHelper httpHelper = new FdHttpHelper();
                    IRequestParameters requestParameter = _httpHelper.GetRequestParameter();
                    var cookies = requestParameter.Cookies;
                    requestParameter.Cookies = new CookieCollection();
                    httpHelper.SetRequestParameter(requestParameter);
                    var response = httpHelper.GetRequest(postUrl);
                    postId = new PostIdResponseHandler(response).PostId;
                    // set Request parameter
                    requestParameter.Cookies = cookies;
                    httpHelper.SetRequestParameter(requestParameter);

                    objFdRequestParameter = new FdRequestParameter();
                    objFdRequestParameter.UrlParameters.Add("ft_ent_identifier", postId);
                    objFdRequestParameter.UrlParameters.Add("av", account.AccountBaseModel.UserId);
                    objFdRequestParameter.UrlParameters.Add("dpr", "1");
                    objFdRequestParameter.UrlParameters.Add("__asyncDialog", "3");
                    objFdRequestParameter.UrlParameters.Add("__req", "1e");

                    objFdRequestParameter = CommonUrlParameters(account, objFdRequestParameter);

                    url = objFdRequestParameter.GenerateUrl(FdConstants.FbPostLikersUrl);

                    postLikersResponse = _httpHelper.GetRequest(url);

                    postLikersResponseHandler = new PostLikersResponseHandler(postLikersResponse);

                }


                postLikersResponseHandler.EntityId = postId;
            }

            else
            {

                try
                {
                    postId = responseHandler.EntityId;

                    var shownIds = responseHandler.ObjFdScraperResponseParameters.ShownIds;
                    var toatlLikersCount = responseHandler.ObjFdScraperResponseParameters.TotalCount;

                    objFdRequestParameter.UrlParameters.Clear();


                    objFdRequestParameter.UrlParameters.Add("limit", "50");
                    objFdRequestParameter.UrlParameters.Add("shown_ids", shownIds);
                    objFdRequestParameter.UrlParameters.Add("total_count", toatlLikersCount);
                    objFdRequestParameter.UrlParameters.Add("ft_ent_identifier", postId);
                    objFdRequestParameter.UrlParameters.Add("dpr", "1");
                    objFdRequestParameter.UrlParameters.Add("__req", "1e");

                    objFdRequestParameter = CommonUrlParameters(account, objFdRequestParameter);

                    url = objFdRequestParameter.GenerateUrl(FdConstants.FbPostLikersPageletUrl);

                    var paginationResponse = _httpHelper.GetRequest(url);


                    postLikersResponseHandler = new PostLikersResponseHandler(paginationResponse);
                }
                catch (Exception ex)
                {
                    ex.DebugLog(ex.Message);
                }
            }

            return postLikersResponseHandler;
        }

        /// <summary>
        /// Method to get group joining status
        /// </summary>
        /// <param name="accountModel"></param>
        /// <param name="groupUrl"></param>
        public bool GetGroupJoiningStatus
            (DominatorAccountModel accountModel, string groupUrl)
        {
            if (accountModel == null)
                throw new ArgumentNullException(nameof(accountModel));

            bool isRequestSent = false;

            FdRequestParameter objFdRequestParameter = new FdRequestParameter();

            var groupResponse = _httpHelper.GetRequest(groupUrl);

            var groupResponseHandler = new CheckJoiningStatusResponseHandler(groupResponse);

            if (groupResponseHandler.IsIncompleteSource)
            {
                groupResponse = _httpHelper.GetRequest(FdConstants.FbGroupDiscussionPage(groupResponseHandler.GroupId));

                groupResponseHandler = new CheckJoiningStatusResponseHandler(groupResponse);
            }

            if (groupResponseHandler.JoiningStatus != null &&
                groupResponseHandler.JoiningStatus.Contains("Join Group"))
            {
                string groupJoinerUrl = FdConstants.FbJoinGroupUrl;

                objFdRequestParameter.PostDataParameters.Clear();

                objFdRequestParameter.PostDataParameters.Add("ref", "child_search");
                objFdRequestParameter.PostDataParameters.Add("group_id", groupResponseHandler.GroupId);
                objFdRequestParameter.PostDataParameters.Add("client_custom_questions", "1");
                objFdRequestParameter.PostDataParameters.Add("__req", "y");

                objFdRequestParameter = CommonPostDataParameters(accountModel, objFdRequestParameter);

                var postdata = objFdRequestParameter.GetPostDataFromParameters();

                var request = _httpHelper.GetRequestParameter();

                request.ContentType = FdConstants.ContentType;

                _httpHelper.SetRequestParameter(request);

                var groupJoinerResponse = _httpHelper.PostRequest(groupJoinerUrl, postdata);

                isRequestSent = new GroupJoinerResponseHandler(groupJoinerResponse, groupResponseHandler.IsQuestionsAsked).IsRequestSent;

                if (groupResponseHandler.IsQuestionsAsked)
                    GetQuestionsAsked(accountModel, groupResponseHandler);

            }


            return isRequestSent;


        }

        /// <summary>
        /// Use this method t check if questions were asked
        /// </summary>
        /// <param name="accountModel"></param>
        /// <param name="groupResponseHandler"></param>
        public void GetQuestionsAsked(DominatorAccountModel accountModel, CheckJoiningStatusResponseHandler groupResponseHandler)
        {
            FdRequestParameter objFdRequestParameter = new FdRequestParameter();

            objFdRequestParameter.UrlParameters.Add("group_id", groupResponseHandler.GroupId);
            objFdRequestParameter.UrlParameters.Add("source", "group_mall");
            objFdRequestParameter.UrlParameters.Add("dpr", "1");
            objFdRequestParameter.UrlParameters.Add("__user", accountModel.AccountBaseModel.UserId);
            objFdRequestParameter.UrlParameters.Add("__a", "1");
            objFdRequestParameter.UrlParameters.Add("__dyn", FdConstants.DynParameter);
            objFdRequestParameter.UrlParameters.Add("__req", "13");
            objFdRequestParameter.UrlParameters.Add("__be", "1");

            string groupAnswerUrl = FdConstants.FbGroupAnswerUrl;

            groupAnswerUrl = objFdRequestParameter.GenerateUrl(groupAnswerUrl);

            // ReSharper disable once UnusedVariable
            var groupAnswerResponse = _httpHelper.GetRequest(groupAnswerUrl);

            //            var groupAnserResponseHandler = new GroupJoinerResponseHandler(groupAnswerResponse, groupResponseHandler.IsQuestionsAsked);


        }

        public string SendFriendRequest(DominatorAccountModel account, string friendId)
        {
            FdRequestParameter objFdRequestParameter = new FdRequestParameter();

            objFdRequestParameter.PostDataParameters.Add("to_friend", friendId);
            objFdRequestParameter.PostDataParameters.Add("action", "add_friend");
            objFdRequestParameter.PostDataParameters.Add("how_found", "profile_button");
            objFdRequestParameter.PostDataParameters.Add("ref_param", "unknown");
            objFdRequestParameter.PostDataParameters.Add("link_data[gt][type]", "xtracking");
            objFdRequestParameter.PostDataParameters.Add("link_data[gt][xt]", FdConstants.FriendLinkDataGt(friendId));
            objFdRequestParameter.PostDataParameters.Add("link_data[gt][profile_owner]", friendId);
            objFdRequestParameter.PostDataParameters.Add("link_data[gt][ref]", "timeline:timeline");
            objFdRequestParameter.PostDataParameters.Add("no_flyout_on_click", "true");
            objFdRequestParameter.PostDataParameters.Add("frefs[0]", "unknown");
            objFdRequestParameter.PostDataParameters.Add("__req", "11");

            objFdRequestParameter = CommonPostDataParameters(account, objFdRequestParameter);

            var postData = objFdRequestParameter.GetPostDataFromParameters();

            var request = _httpHelper.GetRequestParameter();

            request.ContentType = FdConstants.ContentType;

            _httpHelper.SetRequestParameter(request);

            var url = FdConstants.FbSendFriendRequestUrl;

            var sendRequestResponse = _httpHelper.PostRequest(url, postData);

            var senRequestResponseHandler = new SendRequestResponseHandler(sendRequestResponse);

            if (senRequestResponseHandler.RequestStatus.Contains("confirmation required"))
            {
                objFdRequestParameter.PostDataParameters.Add("confirmed", "1");

                postData = objFdRequestParameter.GetPostDataFromParameters();

                request = _httpHelper.GetRequestParameter();

                request.ContentType = FdConstants.ContentType;

                _httpHelper.SetRequestParameter(request);

                senRequestResponseHandler = new SendRequestResponseHandler(_httpHelper.PostRequest(FdConstants.FbSendFriendRequestUrl, postData));
            }

            return senRequestResponseHandler.RequestStatus;
        }

        public IResponseHandler GetFriendSuggestedByFacebook(DominatorAccountModel account
            , IResponseHandler responseHandler)
        {

            FdRequestParameter objFdRequestParameter = new FdRequestParameter();

            FriendSuggestedByAFriendResponseHandler findFriendResponseHandler;

            string url;

            string fbDtsg = Uri.UnescapeDataString(account.SessionId);

            if (responseHandler == null)
            {
                url = FdConstants.FbFindFriendUrl;

                var findFriendResponse = _httpHelper.GetRequest(url);

                findFriendResponseHandler = new FriendSuggestedByAFriendResponseHandler(findFriendResponse, null);

            }

            else
            {


                objFdRequestParameter.UrlParameters.Clear();


                foreach (string id in responseHandler.ObjFdScraperResponseParameters.PaginationUserIds)
                {
                    objFdRequestParameter.PostDataParameters.Add($"friend_browser_id[{responseHandler.ObjFdScraperResponseParameters.PaginationUserIds.IndexOf(id).ToString()}]", id);
                }
                objFdRequestParameter.PostDataParameters.Add("extra_data", responseHandler.ObjFdScraperResponseParameters.ExtraData);
                objFdRequestParameter.PostDataParameters.Add("how_found", "requests_page_pymk");
                objFdRequestParameter.PostDataParameters.Add("page", "friends_center");
                objFdRequestParameter.PostDataParameters.Add("instance_name", "friend-browser");
                objFdRequestParameter.PostDataParameters.Add("social_context", "1");
                objFdRequestParameter.PostDataParameters.Add("network_context", "1");
                objFdRequestParameter.PostDataParameters.Add("show_more", "true");
                objFdRequestParameter.PostDataParameters.Add("__user", account.AccountBaseModel.UserId);
                objFdRequestParameter.PostDataParameters.Add("__a", "1");
                objFdRequestParameter.PostDataParameters.Add("__dyn", FdConstants.DynParameter);
                objFdRequestParameter.PostDataParameters.Add("__req", "10");
                objFdRequestParameter.PostDataParameters.Add("__be", "1");
                objFdRequestParameter.PostDataParameters.Add("fb_dtsg", fbDtsg);
                objFdRequestParameter.PostDataParameters.Add("jazoest", FdConstants.JazoestParameterGender);

                var postData = objFdRequestParameter.GetPostDataFromParameters();

                var request = _httpHelper.GetRequestParameter();

                request.ContentType = FdConstants.ContentType;

                _httpHelper.SetRequestParameter(request);

                url = FdConstants.FriendSuggestedByFriendUrl;

                var paginationResponse = _httpHelper.PostRequest(url, postData);

                findFriendResponseHandler = new FriendSuggestedByAFriendResponseHandler(paginationResponse, responseHandler.ObjFdScraperResponseParameters.ListUser);

            }

            findFriendResponseHandler.HasMoreResults = findFriendResponseHandler.ObjFdScraperResponseParameters.PaginationUserIds.Count > 0;

            return findFriendResponseHandler;

        }

        public IResponseHandler GetIncomingFriendRequests
            (DominatorAccountModel account, IResponseHandler responseHandler)
        {
            FdRequestParameter objFdRequestParameter = new FdRequestParameter();


            if (responseHandler == null)
                return new IncomingFriendListResponseHandler(_httpHelper.GetRequest(FdConstants.FbFindFriendUrl));
            else
            {


                objFdRequestParameter.UrlParameters.Clear();

                objFdRequestParameter.UrlParameters.Add("seenTimestamp", responseHandler.ObjFdScraperResponseParameters.SeenTimeStamp);
                objFdRequestParameter.UrlParameters.Add("endingID", responseHandler.ObjFdScraperResponseParameters.EndingId);
                objFdRequestParameter.UrlParameters.Add("hidden", "0");
                objFdRequestParameter.UrlParameters.Add("fill", "1");
                objFdRequestParameter.UrlParameters.Add("split", "0");
                objFdRequestParameter.UrlParameters.Add("frefs[0]", "none");
                objFdRequestParameter.UrlParameters.Add("dpr", "1");
                objFdRequestParameter.UrlParameters.Add("__user", account.AccountBaseModel.UserId);
                objFdRequestParameter.UrlParameters.Add("__a", "1");
                objFdRequestParameter.UrlParameters.Add("__dyn", FdConstants.DynParameter);
                objFdRequestParameter.UrlParameters.Add("__req", "10");
                objFdRequestParameter.UrlParameters.Add("__be", "1");

                var url = objFdRequestParameter.GenerateUrl(FdConstants.IncomingFriendPagerUrl);

                return new IncomingFriendListResponseHandler(_httpHelper.GetRequest(url));

            }

        }

        public IResponseHandler GetSentFriendRequestIdsNew
          (DominatorAccountModel account, IResponseHandler responseHandler)
        {
            FdRequestParameter objFdRequestParameter = new FdRequestParameter();

            string url;

            SentFriendRequestListNewResponseHandler sentFriendResponseHandler;

            if (responseHandler == null)
            {
                objFdRequestParameter.UrlParameters.Add("privacy_source", "activity_log");
                objFdRequestParameter.UrlParameters.Add("log_filter", "sentfriendrequests");

                url = objFdRequestParameter.GenerateUrl(FdConstants.FbSentRequestUrl(account.AccountBaseModel.UserId));

                sentFriendResponseHandler = new SentFriendRequestListNewResponseHandler(_httpHelper.GetRequest(url), null, account.AccountBaseModel.UserId, false);
            }
            else
            {

                do
                {
                    objFdRequestParameter.UrlParameters.Clear();

                    objFdRequestParameter.UrlParameters.Add("profile_id", account.AccountBaseModel.UserId);
                    objFdRequestParameter.UrlParameters.Add("hidden_filter", "");
                    objFdRequestParameter.UrlParameters.Add("only_me_filter", "0");
                    objFdRequestParameter.UrlParameters.Add("prev_cursor", $"{responseHandler.ObjFdScraperResponseParameters.FriendsPager.ShownTime}:1");
                    objFdRequestParameter.UrlParameters.Add("prev_shown_time", responseHandler.ObjFdScraperResponseParameters.FriendsPager.ShownTime);
                    objFdRequestParameter.UrlParameters.Add("privacy_filter", "");
                    objFdRequestParameter.UrlParameters.Add("sidenav_filter", "sentfriendrequests");
                    objFdRequestParameter.UrlParameters.Add("scrubber_month", responseHandler.ObjFdScraperResponseParameters.FriendsPager.ScrubberMonth);
                    objFdRequestParameter.UrlParameters.Add("scrubber_year", responseHandler.ObjFdScraperResponseParameters.FriendsPager.ScrubberYear);
                    objFdRequestParameter.UrlParameters.Add("data", "1.5");
                    objFdRequestParameter.UrlParameters.Add("__req", "10");

                    objFdRequestParameter = CommonUrlParameters(account, objFdRequestParameter);

                    url = objFdRequestParameter.GenerateUrl(FdConstants.SentFriendExtraPager);

                    var paginationResponse = _httpHelper.GetRequest(url);


                    sentFriendResponseHandler = new SentFriendRequestListNewResponseHandler(paginationResponse, responseHandler.ObjFdScraperResponseParameters.FriendsPager, account.AccountBaseModel.UserId, true);

                    if (sentFriendResponseHandler.ObjFdScraperResponseParameters.ListUser.Count == 0)
                    {
                        objFdRequestParameter.UrlParameters.Clear();

                        objFdRequestParameter.UrlParameters.Add("dpr", "1");
                        objFdRequestParameter.UrlParameters.Add("ajaxpipe", "1");
                        objFdRequestParameter.UrlParameters.Add("ajaxpipe_token", responseHandler.ObjFdScraperResponseParameters.FriendsPager.AjaxPipeToken);
                        objFdRequestParameter.UrlParameters.Add("no_script_path", "1");
                        objFdRequestParameter.UrlParameters.Add("data", responseHandler.ObjFdScraperResponseParameters.FriendsPager.Data);
                        objFdRequestParameter.UrlParameters.Add("__req", "fetchstream_1");
                        objFdRequestParameter.UrlParameters.Add("__adt", "2");
                        objFdRequestParameter.UrlParameters.Add("ajaxpipe_fetch_stream", "1");

                        objFdRequestParameter = CommonUrlParameters(account, objFdRequestParameter);

                        url = objFdRequestParameter.GenerateUrl(FdConstants.FriendActivityNextPager);

                        paginationResponse = _httpHelper.GetRequest(url);


                        sentFriendResponseHandler = new SentFriendRequestListNewResponseHandler(paginationResponse, responseHandler.ObjFdScraperResponseParameters.FriendsPager, account.AccountBaseModel.UserId, false);

                    }

                } while (sentFriendResponseHandler.ObjFdScraperResponseParameters.ListUser.Count == 0 && sentFriendResponseHandler.ObjFdScraperResponseParameters.FriendsPager.CurrentDataKey != sentFriendResponseHandler.ObjFdScraperResponseParameters.FriendsPager.MaxDataKey);


            }

            return sentFriendResponseHandler;
        }

        public bool CancelSentRequest(DominatorAccountModel account, string friendId)
        {
            FdRequestParameter objFdRequestParameter = new FdRequestParameter();


            objFdRequestParameter.PostDataParameters.Add("friend", friendId);
            objFdRequestParameter.PostDataParameters.Add("cancel_ref", "profile");
            objFdRequestParameter.PostDataParameters.Add("floc", "profile_button");
            objFdRequestParameter.PostDataParameters.Add("__req", "11");

            objFdRequestParameter = CommonPostDataParameters(account, objFdRequestParameter);

            objFdRequestParameter.PostDataParameters.Add("confirmed", "1");

            var postData = objFdRequestParameter.GetPostDataFromParameters();

            var request = _httpHelper.GetRequestParameter();

            request.ContentType = FdConstants.ContentType;

            _httpHelper.SetRequestParameter(request);

            return new CancelSentRequestResponseHandler(_httpHelper.PostRequest(FdConstants.CancelFriendRequest, postData)).IsCancelledRequest;

        }


        public CancelSentRequestResponseHandler Unfriend(DominatorAccountModel account, ref FacebookUser facebookUser)
        {
            FdRequestParameter objFdRequestParameter = new FdRequestParameter();

            if (facebookUser.UserId.Contains($"{FdConstants.FbHomeUrl}"))
            {
                try
                {
                    var friendurl = facebookUser.UserId;
                    var response = _httpHelper.GetRequest(friendurl);

                    facebookUser.UserId = FdRegexUtility.FirstMatchExtractor(response.Response, FdConstants.ProfileIdRegex);

                    //FacebookUser.UserId = FdRegexUtility.FirstMatchExtractor(response.Response, FdConstants.ProfileIdRegex);

                    facebookUser.ProfileUrl = $"{FdConstants.FbHomeUrl}{facebookUser.UserId}";

                    HtmlDocument objHtmlDocument = new HtmlDocument();

                    var decodedResponse = FdFunctions.GetDecodedResponse(response.Response);

                    objHtmlDocument.LoadHtml(decodedResponse);

                    facebookUser.Familyname = objHtmlDocument.DocumentNode.SelectNodes("(//span[@data-testid=\"profile_name_in_profile_page\"])")[0].InnerHtml;

                    //FacebookUser.Familyname = FdRegexUtility.FirstMatchExtractor(FacebookUser.Familyname, FdConstants.FamilyNameRegex);

                    facebookUser.Familyname = FdRegexUtility.FirstMatchExtractor(facebookUser.Familyname, FdConstants.FamilyNameRegex);
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }

            objFdRequestParameter.PostDataParameters.Add("uid", facebookUser.UserId);
            objFdRequestParameter.PostDataParameters.Add("unref", "bd_profile_button");
            objFdRequestParameter.PostDataParameters.Add("floc", "profile_button");
            objFdRequestParameter.PostDataParameters.Add("nctr[_mod]", "pagelet_timeline_profile_actions");
            objFdRequestParameter.PostDataParameters.Add("__req", "11");

            objFdRequestParameter = CommonPostDataParameters(account, objFdRequestParameter);

            objFdRequestParameter.PostDataParameters.Add("confirmed", "1");

            var postData = objFdRequestParameter.GetPostDataFromParameters();

            var request = _httpHelper.GetRequestParameter();

            request.ContentType = FdConstants.ContentType;

            _httpHelper.SetRequestParameter(request);

            return new CancelSentRequestResponseHandler(_httpHelper.PostRequest(FdConstants.Unfriend, postData));
        }

        public bool CancelIncomingRequest(DominatorAccountModel account, string friendId)
        {
            FdRequestParameter objFdRequestParameter = new FdRequestParameter();

            objFdRequestParameter.PostDataParameters.Add("action", "reject");
            objFdRequestParameter.PostDataParameters.Add("id", friendId);
            objFdRequestParameter.PostDataParameters.Add("ref", "/reqs.php");
            objFdRequestParameter.PostDataParameters.Add("floc", "friend_center_requests");
            objFdRequestParameter.PostDataParameters.Add("frefs[0]", "ff");
            objFdRequestParameter.PostDataParameters.Add("viewer_id", account.AccountBaseModel.UserId);
            objFdRequestParameter.PostDataParameters.Add("__req", "11");

            objFdRequestParameter = CommonPostDataParameters(account, objFdRequestParameter);

            var postData = objFdRequestParameter.GetPostDataFromParameters();

            var request = _httpHelper.GetRequestParameter();

            request.ContentType = FdConstants.ContentType;

            _httpHelper.SetRequestParameter(request);


            return new CancelIncomingRequestResponseHandler(_httpHelper.PostRequest(FdConstants.CancelIncomingRequestUrl, postData)).IsCancelledRequest;
        }

        public bool AcceptFriendRequest(DominatorAccountModel account, string friendId)
        {

            FdRequestParameter objFdRequestParameter = new FdRequestParameter();

            objFdRequestParameter.PostDataParameters.Add("action", "confirm");
            objFdRequestParameter.PostDataParameters.Add("id", friendId);
            objFdRequestParameter.PostDataParameters.Add("ref", "/reqs.php");
            objFdRequestParameter.PostDataParameters.Add("floc", "friend_center_requests");
            objFdRequestParameter.PostDataParameters.Add("frefs[0]", "ff");
            objFdRequestParameter.PostDataParameters.Add("viewer_id", account.AccountBaseModel.UserId);
            objFdRequestParameter.PostDataParameters.Add("__req", "11");

            objFdRequestParameter = CommonPostDataParameters(account, objFdRequestParameter);

            var postData = objFdRequestParameter.GetPostDataFromParameters();

            var request = _httpHelper.GetRequestParameter();

            request.ContentType = FdConstants.ContentType;

            _httpHelper.SetRequestParameter(request);

            return new AcceptFriendRequestResponseHandler(_httpHelper.PostRequest(FdConstants.AcceptRequestUrl, postData)).IsAcceptedRequest;
        }

        public List<string> GetAlreadyGroupJoinedFriendsList(DominatorAccountModel account, string group_id)
        {
            FdRequestParameter objFdRequestParameter = new FdRequestParameter();

            objFdRequestParameter.UrlParameters.Add("group_id", group_id);
            objFdRequestParameter.UrlParameters.Add("__asyncDialog", "3");
            objFdRequestParameter.UrlParameters.Add("__req", "46");
            objFdRequestParameter.UrlParameters.Add("refresh", "1");
            CommonUrlParameters(account, objFdRequestParameter);
            string url = objFdRequestParameter.GenerateUrl($"{FdConstants.FbHomeUrl}ajax/groups/members/add_get/");

            IResponseParameter tokenResponse = _httpHelper.GetRequest(url);
            string token = new GetGroupTokenResponseHandler(tokenResponse).Token;

            objFdRequestParameter.UrlParameters.Clear();
            objFdRequestParameter.UrlParameters.Add("fb_dtsg_ag", "AQyHJCYe2bCxfdZbGU-4ucdoU7Ueka-vhadVl0K6F7uG3Q:AQx0JpqmAusMcCOXDQ4MKJ3uz8-zERN9gmido9f1kwgQVg");
            objFdRequestParameter.UrlParameters.Add("membership_group_id", group_id);
            objFdRequestParameter.UrlParameters.Add("annotate_weak_references", "false");
            objFdRequestParameter.UrlParameters.Add("token", token);
            objFdRequestParameter.UrlParameters.Add("include_contact_importer", "true");
            objFdRequestParameter.UrlParameters.Add("request_id", "bfb14aef-b15d-442d-f768-1ef500cb64c5");
            objFdRequestParameter.UrlParameters.Add("__req", "48");
            CommonUrlParameters(account, objFdRequestParameter);

            url = objFdRequestParameter.GenerateUrl($"{FdConstants.FbHomeUrl}ajax/typeahead/groups/friend.php");

            IResponseParameter memberResponse = _httpHelper.GetRequest(url);

            return new GetFriendsJoiningStatusResponseHandler(memberResponse).memberList;

        }

        public IResponseHandler GetFanpageDetailsFromKeyword(DominatorAccountModel account, string keyword,
                bool isVerifiedFilter, bool isLikedByFriends, FanpageCategory objFanpageCategory,
                IResponseHandler responseHandler)
        {
            string url;

            SearchFanpageDetailsResponseHandler pageLikersResponseHandler = null;

            NewsFeedPaginationResonseHandler paginationResponseHandler;

            string filtersNew = string.Empty;

            FdRequestParameter objFdRequestParameter = new FdRequestParameter();

            if (responseHandler == null)
            {


                if (isLikedByFriends)
                {
                    //objFdRequestParameter.UrlParameters.Add("filters_page_liked", "{\"name\":\"pages_liked\",\"args\":\"\"}");
                }

                if (isVerifiedFilter)
                {
                    filtersNew = "{\"verified\":\"{\\\"name\\\":\\\"pages_verified\\\",\\\"args\\\":\\\"\\\"}";
                }

                if (objFanpageCategory != FanpageCategory.AnyCategory)
                {

                    if (!string.IsNullOrEmpty(filtersNew))
                    {
                        filtersNew += isVerifiedFilter ? "\"," : "\"}";
                    }
                    switch (objFanpageCategory)
                    {
                        case FanpageCategory.ArtistBandorPublicFigure:
                            filtersNew += "\"category\":\"{" + $"\\\"name\\\":\\\"pages_category\\\",\\\"args\\\":\\\"{(int)FanpageCategory.ArtistBandorPublicFigure}\\\"" + "}\"}";
                            break;

                        case FanpageCategory.BrandorProduct:
                            filtersNew += "\"category\":\"{" + $"\\\"name\\\":\\\"pages_category\\\",\\\"args\\\":\\\"{(int)FanpageCategory.BrandorProduct}\\\"" + "}\"}";
                            break;

                        case FanpageCategory.CauseorCommunity:
                            filtersNew += "\"category\":\"{" + $"\\\"name\\\":\\\"pages_category\\\",\\\"args\\\":\\\"{(int)FanpageCategory.CauseorCommunity}\\\"" + "}\"}";
                            break;

                        case FanpageCategory.CompanyOrganizationorInstitution:
                            filtersNew += "\"category\":\"{" + $"\\\"name\\\":\\\"pages_category\\\",\\\"args\\\":\\\"{(int)FanpageCategory.CompanyOrganizationorInstitution}\\\"" + "}\"}";
                            break;

                        case FanpageCategory.Entertainment:
                            filtersNew += "\"category\":\"{" + $"\\\"name\\\":\\\"pages_category\\\",\\\"args\\\":\\\"{(int)FanpageCategory.Entertainment}\\\"" + "}\"}";
                            break;

                        case FanpageCategory.LocalBusinessorPlace:
                            filtersNew += "\"category\":\"{" + $"\\\"name\\\":\\\"pages_category\\\",\\\"args\\\":\\\"{(int)FanpageCategory.LocalBusinessorPlace}\\\"" + "}\"}"; break;
                    }

                }

                if (!string.IsNullOrEmpty(filtersNew))
                    objFdRequestParameter.UrlParameters.Add("filters", filtersNew.Base64Encode());

                url = objFdRequestParameter.GenerateUrl(FdConstants.FanpageSearchUrlNew(keyword));

                var searchFanpageResponse = _httpHelper.GetRequest(url);


                pageLikersResponseHandler = new SearchFanpageDetailsResponseHandler(searchFanpageResponse);

                paginationResponseHandler = new NewsFeedPaginationResonseHandler(searchFanpageResponse, false, string.Empty);

                pageLikersResponseHandler.PageletData = paginationResponseHandler.PageletData;

                pageLikersResponseHandler.ObjFdScraperResponseParameters.FinalEncodedQuery = paginationResponseHandler.ObjFdScraperResponseParameters.FinalEncodedQuery;

                pageLikersResponseHandler.HasMoreResults = !string.IsNullOrEmpty(pageLikersResponseHandler.PageletData);

            }


            else
            {

                try
                {

                    var paginationData = responseHandler.PageletData;

                    objFdRequestParameter.UrlParameters.Clear();

                    objFdRequestParameter.UrlParameters.Add("dpr", "1");
                    objFdRequestParameter.UrlParameters.Add("data", paginationData);
                    objFdRequestParameter.UrlParameters.Add("__req", "10");

                    objFdRequestParameter = CommonUrlParameters(account, objFdRequestParameter);

                    url = objFdRequestParameter.GenerateUrl(FdConstants.FbFanpageLikerPageleUrl);

                    var paginationResponse = _httpHelper.GetRequest(url);



                    pageLikersResponseHandler = new SearchFanpageDetailsResponseHandler(paginationResponse);
                    paginationResponseHandler = new NewsFeedPaginationResonseHandler(paginationResponse, true, responseHandler.ObjFdScraperResponseParameters.FinalEncodedQuery);

                    pageLikersResponseHandler.PageletData = paginationResponseHandler.PageletData;

                    pageLikersResponseHandler.ObjFdScraperResponseParameters.FinalEncodedQuery = paginationResponseHandler.ObjFdScraperResponseParameters.FinalEncodedQuery;

                    pageLikersResponseHandler.HasMoreResults = !string.IsNullOrEmpty(pageLikersResponseHandler.PageletData);

                }
                catch (Exception ex)
                {
                    ex.DebugLog(ex.Message);
                }

            }

            return pageLikersResponseHandler;
        }

        public IResponseHandler GetFanpageDetailsLikedByFriend(DominatorAccountModel account, string friendUrl,
                bool isVerifiedFilter, bool isLikedByFriends, FanpageCategory objFanpageCategory,
                IResponseHandler responseHandler)
        {
            string url;

            PagesLikedByFriendsRsponseHandler pageLikersResponseHandler = null;

            FdRequestParameter objFdRequestParameter = new FdRequestParameter();

            if (responseHandler == null)
            {
                var friendId = GetFriendUserId(account, friendUrl).UserId;


                url = objFdRequestParameter.GenerateUrl(FdConstants.PageLikedByFrendsUrl(friendId));

                var searchFanpageResponse = _httpHelper.GetRequest(url);


                pageLikersResponseHandler = new PagesLikedByFriendsRsponseHandler(searchFanpageResponse);

                //pageLikersResponseHandler.PaginationData = paginationResponseHandler.PaginationData;

                //pageLikersResponseHandler.FinalEncodedQuery = paginationResponseHandler.FinalEncodedQuery;

                pageLikersResponseHandler.HasMoreResults = !string.IsNullOrEmpty(pageLikersResponseHandler.PageletData);

            }
            else
            {

                try
                {

                    var paginationData = responseHandler.PageletData;

                    objFdRequestParameter.UrlParameters.Clear();

                    objFdRequestParameter.UrlParameters.Add("dpr", "1");
                    objFdRequestParameter.UrlParameters.Add("data", paginationData);
                    objFdRequestParameter.UrlParameters.Add("__req", "7");
                    objFdRequestParameter.UrlParameters.Add("__comet_req", "false");

                    objFdRequestParameter = CommonUrlParameters(account, objFdRequestParameter);

                    url = objFdRequestParameter.GenerateUrl(FdConstants.PageLikedByFriendsPageleUrl);

                    var paginationResponse = _httpHelper.GetRequest(url);

                    pageLikersResponseHandler = new PagesLikedByFriendsRsponseHandler(paginationResponse, true, responseHandler.PageletData ?? string.Empty);

                    //pageLikersResponseHandler.PaginationData = paginationResponseHandler.PaginationData;

                    //pageLikersResponseHandler.FinalEncodedQuery = paginationResponseHandler.FinalEncodedQuery;

                    pageLikersResponseHandler.HasMoreResults = !string.IsNullOrEmpty(pageLikersResponseHandler.PageletData);

                }
                catch (Exception ex)
                {
                    ex.DebugLog(ex.Message);
                }

            }

            return pageLikersResponseHandler;
        }

        public IResponseHandler GetPlaceDetailsFromKeyword(DominatorAccountModel account, string keyword,
            FdPlaceFilterModel fdPlaceFilterModel,
            IResponseHandler responseHandler)
        {
            string url = keyword;

            SearchPlaceDetailsResponseHandler pageLikersResponseHandler = null;

            string filtersNew = string.Empty;

            FdRequestParameter objFdRequestParameter = new FdRequestParameter();

            if (responseHandler == null)
            {

                if (!string.IsNullOrEmpty(filtersNew))
                    objFdRequestParameter.UrlParameters.Add("filters", filtersNew.Base64Encode());

                if (!url.Contains($"{FdConstants.FbHomeUrl}"))
                    url = objFdRequestParameter.GenerateUrl(FdConstants.PlaceSearchUrl(keyword));

                var searchFanpageResponse = _httpHelper.GetRequest(url);


                pageLikersResponseHandler = new SearchPlaceDetailsResponseHandler(searchFanpageResponse);

            }
            else
            {

                try
                {


                    objFdRequestParameter.PostDataParameters.Clear();

                    url = FdConstants.PlaceScraperUrl;

                    objFdRequestParameter.PostDataParameters.Add("query", Uri.EscapeDataString(responseHandler.ObjFdScraperResponseParameters.Query));
                    objFdRequestParameter.PostDataParameters.Add("original_query", Uri.EscapeDataString(responseHandler.ObjFdScraperResponseParameters.OriginalQuery));
                    objFdRequestParameter.PostDataParameters.Add("applied_rp_filters", "[]");
                    objFdRequestParameter.PostDataParameters.Add("applied_instant_filters", "[]");
                    objFdRequestParameter.PostDataParameters.Add("cursor", responseHandler.ObjFdScraperResponseParameters.Cursor);
                    objFdRequestParameter.PostDataParameters.Add("dpr", "1");
                    objFdRequestParameter.PostDataParameters.Add("__req", "1o");

                    objFdRequestParameter = CommonPostDataParameters(account, objFdRequestParameter);

                    var postData = objFdRequestParameter.GetPostDataFromParameters();

                    var request = _httpHelper.GetRequestParameter();

                    request.ContentType = FdConstants.ContentType;

                    _httpHelper.SetRequestParameter(request);

                    var paginationResponse = _httpHelper.PostRequest(url, postData);

                    pageLikersResponseHandler = new SearchPlaceDetailsResponseHandler(paginationResponse, true);

                    pageLikersResponseHandler.ObjFdScraperResponseParameters.OriginalQuery = responseHandler.ObjFdScraperResponseParameters.OriginalQuery;

                    pageLikersResponseHandler.ObjFdScraperResponseParameters.Query = responseHandler.ObjFdScraperResponseParameters.Query;

                }
                catch (Exception ex)
                {
                    ex.DebugLog(ex.Message);
                }
            }
            return pageLikersResponseHandler;
        }

        public IResponseHandler GetFanpageDetailsFromGraphSearch(DominatorAccountModel account, string graphSearchUrl,
            bool isVerifiedFilter, bool isLikedByFriends, FanpageCategory objFanpageCategory,
            IResponseHandler responseHandler)
        {
            string graphSearchFilters = string.Empty;

            SearchFanpageDetailsResponseHandler pageLikersResponseHandler = null;

            NewsFeedPaginationResonseHandler paginationResponseHandler;

            FdRequestParameter objFdRequestParameter = new FdRequestParameter();

            if (responseHandler == null)
            {

                if (isLikedByFriends)
                    objFdRequestParameter.UrlParameters.Add("filters_page_liked", "{\"name\":\"pages_liked\",\"args\":\"\"}");


                if (isVerifiedFilter || graphSearchFilters.Contains("verified"))
                    objFdRequestParameter.UrlParameters.Add("filters_verified", "{\"name\":\"pages_verified\",\"args\":\"\"}");

                if (!graphSearchFilters.Contains("category"))
                {
                    string filterCategory;
                    switch (objFanpageCategory)
                    {
                        case FanpageCategory.ArtistBandorPublicFigure:
                            filterCategory = "{" + $"\"name\":\"pages_category\",\"args\":\"{(int)FanpageCategory.ArtistBandorPublicFigure}\"" + "}";
                            objFdRequestParameter.UrlParameters.Add("filters_category", filterCategory);
                            break;

                        case FanpageCategory.BrandorProduct:
                            filterCategory = "{" + $"\"name\":\"pages_category\",\"args\":\"{(int)FanpageCategory.BrandorProduct}\"" + "}";
                            objFdRequestParameter.UrlParameters.Add("filters_category", filterCategory);
                            break;

                        case FanpageCategory.CauseorCommunity:
                            filterCategory = "{" + $"\"name\":\"pages_category\",\"args\":\"{(int)FanpageCategory.CauseorCommunity}\"" + "}";
                            objFdRequestParameter.UrlParameters.Add("filters_category", filterCategory);
                            break;

                        case FanpageCategory.CompanyOrganizationorInstitution:
                            filterCategory = "{" + $"\"name\":\"pages_category\",\"args\":\"{(int)FanpageCategory.CompanyOrganizationorInstitution}\"" + "}";
                            objFdRequestParameter.UrlParameters.Add("filters_category", filterCategory);
                            break;

                        case FanpageCategory.Entertainment:
                            filterCategory = "{" + $"\"name\":\"pages_category\",\"args\":\"{(int)FanpageCategory.Entertainment}\"" + "}";
                            objFdRequestParameter.UrlParameters.Add("filters_category", filterCategory);
                            break;

                        case FanpageCategory.LocalBusinessorPlace:
                            filterCategory = "{" + $"\"name\":\"pages_category\",\"args\":\"{(int)FanpageCategory.LocalBusinessorPlace}\"" + "}";
                            objFdRequestParameter.UrlParameters.Add("filters_category", filterCategory);
                            break;
                    }
                }



                //url = objFdRequestParameter.GenerateUrl(FdConstants.FbFanpageSearchUrl);

                var searchFanpageResponse = _httpHelper.GetRequest(graphSearchUrl);


                pageLikersResponseHandler = new SearchFanpageDetailsResponseHandler(searchFanpageResponse);

                paginationResponseHandler = new NewsFeedPaginationResonseHandler(searchFanpageResponse, false, string.Empty);

                pageLikersResponseHandler.PageletData = paginationResponseHandler.PageletData;

                pageLikersResponseHandler.ObjFdScraperResponseParameters.FinalEncodedQuery = paginationResponseHandler.ObjFdScraperResponseParameters.FinalEncodedQuery;

                if (!string.IsNullOrEmpty(pageLikersResponseHandler.PageletData))
                    pageLikersResponseHandler.HasMoreResults = true;

            }


            else
            {

                try
                {

                    var paginationData = responseHandler.PageletData;

                    objFdRequestParameter.UrlParameters.Clear();

                    objFdRequestParameter.UrlParameters.Add("dpr", "1");
                    objFdRequestParameter.UrlParameters.Add("data", paginationData);
                    objFdRequestParameter.UrlParameters.Add("__req", "10");

                    objFdRequestParameter = CommonUrlParameters(account, objFdRequestParameter);

                    var url = objFdRequestParameter.GenerateUrl(FdConstants.FbFanpageLikerPageleUrl);

                    var paginationResponse = _httpHelper.GetRequest(url);



                    pageLikersResponseHandler = new SearchFanpageDetailsResponseHandler(paginationResponse);
                    paginationResponseHandler = new NewsFeedPaginationResonseHandler(paginationResponse, true, responseHandler.ObjFdScraperResponseParameters.FinalEncodedQuery);

                    pageLikersResponseHandler.PageletData = paginationResponseHandler.PageletData;

                    pageLikersResponseHandler.ObjFdScraperResponseParameters.FinalEncodedQuery = paginationResponseHandler.ObjFdScraperResponseParameters.FinalEncodedQuery;

                    if (!string.IsNullOrEmpty(pageLikersResponseHandler.PageletData))
                        pageLikersResponseHandler.HasMoreResults = true;

                }
                catch (Exception ex)
                {
                    ex.DebugLog(ex.Message);
                }

            }

            return pageLikersResponseHandler;
        }

        public FanpageScraperResponseHandler GetFanpageDetails
            (DominatorAccountModel account, FanpageDetails objFanpageDetails, bool isLocationPage = false)
        {
            var url = FdConstants.FbHomeUrl + objFanpageDetails.FanPageID;

            var fanpageResponse = _httpHelper.GetRequest(url);

            return !isLocationPage ? new FanpageScraperResponseHandler(fanpageResponse, objFanpageDetails)
                : new FanpageScraperResponseHandler(fanpageResponse, objFanpageDetails, true);
        }

        public PostScraperResponseHandler GetPostDetails
            (DominatorAccountModel account, FacebookPostDetails objFacebookPostDetails, bool isWatchParty = false)

        {
            var composerId = string.Empty;

            var url = objFacebookPostDetails.Id.Contains(FdConstants.FbHomeUrl)
                ? objFacebookPostDetails.Id
                : $"{FdConstants.FbHomeUrl}{objFacebookPostDetails.Id}";

            var publisherResponse = _httpHelper.GetRequest(url);

            var postDetailsResponseHandler = new PostScraperResponseHandler(publisherResponse, objFacebookPostDetails);

            if (objFacebookPostDetails.MediaType == MediaType.Video)
            {

                var homePageResponse = _httpHelper.GetRequest(FdConstants.FbHomeUrl);

                try
                {
                    composerId = FdRegexUtility.FirstMatchExtractor(homePageResponse.Response, FdConstants.ComposerIdRegex);
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                GetVideoDetails(account, composerId, ref objFacebookPostDetails);
            }

            return postDetailsResponseHandler;
        }

        public PostScraperResponseHandler GetPostDetailNew
      (DominatorAccountModel account, FacebookPostDetails objFacebookPostDetails, bool isWatchParty = false)
        {
            var composerId = string.Empty;

            var url = objFacebookPostDetails.Id;

            if (string.IsNullOrEmpty(url))
                url = objFacebookPostDetails.PostUrl;

            if (!url.Contains(FdConstants.FbHomeUrl))
                url = FdConstants.FbHomeUrl + url;

            var publisherResponse = _httpHelper.GetRequest(url);

            var postDetailsResponseHandler = new PostScraperResponseHandler(publisherResponse, objFacebookPostDetails);


            if ((objFacebookPostDetails.MediaType == MediaType.Video || objFacebookPostDetails.MediaType == MediaType.NoMedia)
                 && !isWatchParty)
            {

                var homePageResponse = _httpHelper.GetRequest(FdConstants.FbHomeUrl);

                try
                {
                    composerId = FdRegexUtility.FirstMatchExtractor(homePageResponse.Response, FdConstants.ComposerIdRegex);
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                GetVideoDetails(account, composerId, ref objFacebookPostDetails, url);
            }

            return postDetailsResponseHandler;
        }

        public async Task<PostDetailsScraperResponseHandlerNew> GetPostDetailWithDestinationUrl
            (DominatorAccountModel account, FacebookAdsDetails objFacebookPostDetails, string composerId)
        {
            try
            {
                var url = objFacebookPostDetails.Id.Contains(FdConstants.FbHomeUrl)
                ? objFacebookPostDetails.Id
                : $"{FdConstants.FbHomeUrl}{objFacebookPostDetails.Id}";

                var parameter = _httpHelper.GetRequestParameter();

                RequestParameters objParameter = new FdRequestParameter(parameter.Proxy);

                objParameter.Cookies = new CookieCollection();

                HttpHelper objHelper = new FdHttpHelper();

                objHelper.SetRequestParameter(objParameter);

                var publisherResponse = await objHelper.GetRequestAsync(url, account.Token);

                if (publisherResponse.Response.Contains("href=\"/r.php?r=101\""))
                    publisherResponse = await _httpHelper.GetRequestAsync(url, account.Token);

                var postDetailsResponseHandler = new PostDetailsScraperResponseHandlerNew(publisherResponse, objFacebookPostDetails);

                if (postDetailsResponseHandler.PostDetails.PostedDateTime > DateTime.Now)
                {
                    publisherResponse = await _httpHelper.GetRequestAsync(url, account.Token);
                    postDetailsResponseHandler = new PostDetailsScraperResponseHandlerNew(publisherResponse, postDetailsResponseHandler.PostDetails, isFetchCorrectDate: true);
                }

                if (!FdFunctions.CheckUrlValid(objFacebookPostDetails.NavigationUrl))
                    objFacebookPostDetails.NavigationUrl = string.Empty;

                await _delayService.DelayAsync(2000);
                await GetNavigationUrl(account, publisherResponse.Response, objFacebookPostDetails, url);

                if (string.IsNullOrEmpty(composerId))
                {
                    var homePageResponse = await _httpHelper.GetRequestAsync(FdConstants.FbHomeUrl, account.Token);
                    composerId = FdRegexUtility.FirstMatchExtractor(homePageResponse.Response, FdConstants.ComposerIdRegex);
                }

                await _delayService.DelayAsync(2000);
                await GetVideoDetails(account, composerId, objFacebookPostDetails);

                postDetailsResponseHandler.ComposerId = composerId;

                return postDetailsResponseHandler;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
            return null;
        }

        public string GetVideoDetails(DominatorAccountModel account, string composerId,
            ref FacebookPostDetails objFacebookPostDetails, string watchPartyUrl = "")
        {

            try
            {

                string adVideoUrl;

                if (string.IsNullOrEmpty(watchPartyUrl) && string.IsNullOrEmpty(objFacebookPostDetails.ScapedUrl))
                    adVideoUrl = $"{FdConstants.FbHomeUrl}{objFacebookPostDetails.Id}";

                adVideoUrl = string.IsNullOrEmpty(watchPartyUrl) ? objFacebookPostDetails.ScapedUrl : watchPartyUrl;

                var adScraperComposerPostUrl = $"{FdConstants.FbHomeUrl}react_composer/scraper?composer_id=" + composerId + "&target_id=" + account.AccountBaseModel.UserId + "&scrape_url=" + adVideoUrl + "&entry_point=feedx_sprouts&source_attachment=STATUS&source_logging_name=link_pasted&av=" + account.AccountBaseModel.UserId + "&dpr=1";

                FdRequestParameter objParameter = new FdRequestParameter();

                objParameter.PostDataParameters.Add("__req", "4w");

                objParameter = CommonPostDataParameters(account, objParameter);

                var postData = objParameter.GetPostDataFromParameters();

                var request = _httpHelper.GetRequestParameter();

                request.ContentType = FdConstants.ContentType;

                _httpHelper.SetRequestParameter(request);

                var parameter = _httpHelper.GetRequestParameter();

                var publisherResponse = _httpHelper.PostRequest(adScraperComposerPostUrl, postData);

                _delayService.ThreadSleep(500);

                if (!string.IsNullOrEmpty(publisherResponse.Response))
                    new PostScraperResponseHandler(publisherResponse, objFacebookPostDetails);
                else
                    objFacebookPostDetails.MediaType = MediaType.NoMedia;

                var afterParameter = _httpHelper.GetRequestParameter();

                afterParameter.Cookies = parameter.Cookies;

                _httpHelper.SetRequestParameter(afterParameter);


                return publisherResponse.Response;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return string.Empty;
        }

        public async Task GetVideoDetails(DominatorAccountModel account, string composerId,
               FacebookAdsDetails objFacebookPostDetails)
        {

            try
            {

                var adVideoUrl = string.IsNullOrEmpty(objFacebookPostDetails.FullPostUrl) ? $"{FdConstants.FbHomeUrl}{objFacebookPostDetails.Id}"
                    : objFacebookPostDetails.FullPostUrl;

                var adScraperComposerPostUrl = $"{FdConstants.FbHomeUrl}react_composer/scraper?composer_id=" + composerId + "&target_id=" + account.AccountBaseModel.UserId + "&scrape_url=" + adVideoUrl + "&entry_point=feedx_sprouts&source_attachment=STATUS&source_logging_name=link_pasted&av=" + account.AccountBaseModel.UserId + "&dpr=1";

                FdRequestParameter objParameter = new FdRequestParameter();

                objParameter.PostDataParameters.Add("__req", "4w");

                objParameter = CommonPostDataParameters(account, objParameter);

                var postData = objParameter.GetPostDataFromParameters();

                var request = _httpHelper.GetRequestParameter();

                request.ContentType = FdConstants.ContentType;

                _httpHelper.SetRequestParameter(request);

                var publisherResponse = await _httpHelper.PostRequestAsync(adScraperComposerPostUrl, postData, account.Token);

                if (publisherResponse.Response.Contains("uiScaledImageContainer profilePic"))
                    return;

                _delayService.ThreadSleep(1500);

                if (!string.IsNullOrEmpty(publisherResponse.Response))
                    new PostDetailsScraperResponseHandlerNew(publisherResponse, objFacebookPostDetails, true);
                else if (string.IsNullOrEmpty(objFacebookPostDetails.MediaUrl))
                    objFacebookPostDetails.AdMediaType = AdMediaType.NoMedia;

                if (string.IsNullOrEmpty(objFacebookPostDetails.CallActionType))
                {
                    var adCallAction = string.Empty;

                    foreach (CallActionType calltoAction in Enum.GetValues(typeof(CallActionType)))
                    {
                        try
                        {
                            var possibleCallAction = calltoAction.GetDescriptionAttr();
                            if (!publisherResponse.Response.Contains(possibleCallAction))
                                continue;
                            adCallAction = possibleCallAction;
                            break;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.StackTrace);
                        }

                    }

                    objFacebookPostDetails.CallActionType =
                         !string.IsNullOrEmpty(adCallAction)
                            ? adCallAction
                            : objFacebookPostDetails.CallActionType;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }

        }

        public PostScraperResponseHandler GetPostDetailsNew
            (DominatorAccountModel account, FacebookPostDetails objFacebookPostDetails)
        {
            string composerId = string.Empty;

            var url = objFacebookPostDetails.PostUrl;

            var postResponse = _httpHelper.GetRequest(url);

            List<KeyValuePair<string, string>> listPostReaction = new PostReactionListResponseHandler(postResponse).ListPostReaction;

            try
            {

                var scrapedurl = FdRegexUtility.FirstMatchExtractor(postResponse.Response, "URL=(.*?)&");

                scrapedurl = FdRegexUtility.FirstMatchExtractor(scrapedurl, "/(.*?)\"");

                if (scrapedurl.Contains("/pending/"))
                {
                    objFacebookPostDetails.IsPendingPost = true;

                    return new PostScraperResponseHandler(postResponse, objFacebookPostDetails);
                }

                if (scrapedurl.Contains("/watch/"))
                {
                    var parameter = _httpHelper.GetRequestParameter();

                    RequestParameters objParameter = new FdRequestParameter(parameter.Proxy);

                    objParameter.Cookies = new CookieCollection();

                    HttpHelper objHelper = new FdHttpHelper();

                    objHelper.SetRequestParameter(objParameter);

                    postResponse = objHelper.GetRequest(url);
                }

                var decodedResponse = FdFunctions.GetDecodedResponse(postResponse.Response);


                var postId = FdRegexUtility.FirstMatchExtractor(decodedResponse, FdConstants.TopLevelPostIdRegex);
                if (string.IsNullOrEmpty(postId))
                    postId = FdRegexUtility.FirstMatchExtractor(decodedResponse, FdConstants.EntIdentifierPostIdRegex);

                if (string.IsNullOrEmpty(postId))
                    if (objFacebookPostDetails.PostUrl.Contains("/videos/") && objFacebookPostDetails.PostUrl.Contains($"{FdConstants.FbHomeUrl}"))
                        postId = FdRegexUtility.FirstMatchExtractor(objFacebookPostDetails.PostUrl + "/", FdConstants.VideoPostRegex);

                if (!string.IsNullOrEmpty(postId))
                    objFacebookPostDetails.Id = postId;


                string postReactionDetails = listPostReaction.FirstOrDefault(x => x.Key == objFacebookPostDetails.Id).Value;

                var reactionArray = Regex.Split(postReactionDetails, "<:>").ToArray();

                if (reactionArray.Length > 2)
                {
                    objFacebookPostDetails.CommentorCount = reactionArray[0];
                    objFacebookPostDetails.LikersCount = reactionArray[1];
                    objFacebookPostDetails.SharerCount = reactionArray[2];
                }

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            var postDetailsResponseHandler = new PostScraperResponseHandler(postResponse, objFacebookPostDetails);

            if (objFacebookPostDetails.MediaType == MediaType.Video)
            {
                if (_dictAccountComposer.FirstOrDefault(x => x.Item1 == account.AccountId && (x.Item3 - DateTime.Now).Hours < 1) == null)
                {
                    var homePageResponse = _httpHelper.GetRequest(FdConstants.FbHomeUrl).Response;

                    try
                    {
                        composerId = FdRegexUtility.FirstMatchExtractor(homePageResponse, FdConstants.ComposerIdRegex);
                        _dictAccountComposer.RemoveAll(x => x.Item1 == account.AccountId);
                        _dictAccountComposer.Add(
                            new Tuple<string, string, DateTime>(account.AccountId, composerId, DateTime.Now));
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                }
                else
                    composerId = _dictAccountComposer.FirstOrDefault(x => x.Item1 == account.AccountId)?.Item2;

                if (!string.IsNullOrEmpty(composerId))
                    GetVideoDetails(account, composerId, ref objFacebookPostDetails);

            }


            return postDetailsResponseHandler;
        }

        public PostScraperResponseHandler GetPostDetailsNewDownloadMedia
            (DominatorAccountModel account, FacebookPostDetails objFacebookPostDetails)
        {
            string composerId = string.Empty;

            var url = objFacebookPostDetails.PostUrl;

            var fanpageResponse = _httpHelper.GetRequest(url);

            List<KeyValuePair<string, string>> listPostReaction
                = new PostReactionListResponseHandler(fanpageResponse).ListPostReaction;

            try
            {
                var decodedResponse = FdFunctions.GetDecodedResponse(fanpageResponse.Response);
                var postId = FdRegexUtility.FirstMatchExtractor(decodedResponse, FdConstants.TopLevelPostIdRegex);
                if (string.IsNullOrEmpty(postId))
                    postId = FdRegexUtility.FirstMatchExtractor(decodedResponse, FdConstants.EntIdentifierPostIdRegex);

                if (string.IsNullOrEmpty(postId))
                    if (objFacebookPostDetails.PostUrl.Contains("/videos/") && objFacebookPostDetails.PostUrl.Contains($"{FdConstants.FbHomeUrl}"))
                        postId = FdRegexUtility.FirstMatchExtractor(objFacebookPostDetails.PostUrl + "/", FdConstants.VideoPostRegex);


                if (!string.IsNullOrEmpty(postId))
                    objFacebookPostDetails.Id = postId;


                string postReactionDetails = listPostReaction.FirstOrDefault(x => x.Key == objFacebookPostDetails.Id).Value;

                var reactionArray = Regex.Split(postReactionDetails, "<:>").ToArray();

                if (reactionArray.Length > 2)
                {
                    objFacebookPostDetails.CommentorCount = reactionArray[0];
                    objFacebookPostDetails.LikersCount = reactionArray[1];
                    objFacebookPostDetails.SharerCount = reactionArray[2];
                }

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            var postDetailsResponseHandler = new PostScraperResponseHandler(fanpageResponse, objFacebookPostDetails);

            if (objFacebookPostDetails.MediaType == MediaType.Video)
            {

                var homePageResponse = _httpHelper.GetRequest(FdConstants.FbHomeUrl);

                try
                {
                    composerId = FdRegexUtility.FirstMatchExtractor(homePageResponse.Response, FdConstants.ComposerIdRegex);
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                GetVideoDetails(account, composerId, ref objFacebookPostDetails);
            }
            else
                GetFullSizeImages(account, ref objFacebookPostDetails);

            return postDetailsResponseHandler;
        }

        public void GetFullSizeImages(DominatorAccountModel account, ref FacebookPostDetails objFacebookPostDetails)
        {
            try
            {
                var postUrl = "https://m.facebook.com/" + objFacebookPostDetails.Id;

                var objParameter = _httpHelper.GetRequestParameter();

                if (objParameter.Headers != null)
                    objParameter.Headers["Host"] = "m.facebook.com";

                _httpHelper.SetRequestParameter(objParameter);

                var response = _httpHelper.GetRequest(postUrl);

                var decodedResponse = FdFunctions.GetDecodedResponse(response.Response.Replace("\\", string.Empty));

                objFacebookPostDetails.MediaUrl = FdRegexUtility.FirstMatchExtractor(decodedResponse, FdConstants.MediaIdRegex);

                if (!objFacebookPostDetails.MediaUrl.Contains("https://m.facebook.com/"))
                    objFacebookPostDetails.MediaUrl = "https://m.facebook.com" + objFacebookPostDetails.MediaUrl;

                response = _httpHelper.GetRequest(objFacebookPostDetails.MediaUrl);

                objFacebookPostDetails.MediaUrl = FdRegexUtility.FirstMatchExtractor(response.Response, "url=(.*?)\"");

                objFacebookPostDetails.MediaUrl = HttpUtility.HtmlDecode(objFacebookPostDetails.MediaUrl);

                if (!string.IsNullOrEmpty(objFacebookPostDetails.MediaUrl))
                    objFacebookPostDetails.MediaType = MediaType.Image;

                objParameter = _httpHelper.GetRequestParameter();

                if (objParameter.Headers != null)
                    objParameter.Headers["Host"] = "www.facebook.com";

                _httpHelper.SetRequestParameter(objParameter);
            }
            catch (Exception exception)
            {
                exception.DebugLog();
            }
        }

        public async Task<string> RedirectPath(string url)
        {
            var finalRedirectedUrl = string.Empty;

            var getHeaderLocationValue = string.Copy(url);

            try
            {
                while (!string.IsNullOrWhiteSpace(getHeaderLocationValue))
                {
                    try
                    {
                        if (FdFunctions.CheckUrlValid(getHeaderLocationValue))
                            finalRedirectedUrl = getHeaderLocationValue;

                        var httpWebRequest = WebRequest.CreateHttp(getHeaderLocationValue);
                        ServicePointManager.Expect100Continue = true;
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                        httpWebRequest.AllowAutoRedirect = false;

                        using (var httpWebResponse = (HttpWebResponse)await httpWebRequest.GetResponseAsync())
                        {
                            getHeaderLocationValue = httpWebResponse.GetResponseHeader("Location");
                        }
                    }
                    catch (Exception)
                    {
                        return finalRedirectedUrl;
                    }
                }
            }
            catch (Exception)
            {

            }
            return finalRedirectedUrl;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="account"></param>
        /// <param name="keyword"></param>
        /// <param name="objGroupMemberShip"></param>
        /// <param name="objGroupType"></param>
        /// <param name="responseHandler"></param>
        /// <param name="option"></param>
        /// <returns>
        /// 
        /// </returns>
        public IResponseHandler ScrapGroups
             (DominatorAccountModel account, string keyword, GroupMemberShip objGroupMemberShip,
             GroupType objGroupType, IResponseHandler responseHandler, string option)
        {
            string url;

            string modifiedUrl = string.Empty;

            GroupScraperResponseHandler groupScraperResponseHandler;

            NewsFeedPaginationResonseHandler paginationResponseHandler;

            FdRequestParameter objFdRequestParameter = new FdRequestParameter();

            if (responseHandler == null)
            {
                if (keyword != "My groups" && option.ToLower() != "Graph Search Url".ToLower()
                    && option.ToLower() != "CustomGroupUrl".ToLower())
                {
                    keyword = Uri.UnescapeDataString(keyword);

                    objFdRequestParameter.UrlParameters.Add("q", keyword);
                }

                var filters = string.Empty;

                if (!keyword.Contains("filters_groups_memebership"))
                {
                    switch (objGroupMemberShip)
                    {
                        case GroupMemberShip.MyGroups:
                            filters = "{\"groups_memebership\":\"{\\\"name\\\":\\\"my_groups\\\",\\\"args\\\":\\\"\\\"}";
                            break;
                    }
                }

                if (!keyword.Contains("filters_groups_show_only"))
                {
                    if (!string.IsNullOrEmpty(filters))
                        filters += objGroupType != GroupType.Any ? "\"," : "\"}";

                    switch (objGroupType)
                    {
                        case GroupType.Public:
                            filters += "{\"groups_show_only\":\"{\\\"name\\\":\\\"public_groups\\\",\\\"args\\\":\\\"\\\"}\"}";
                            break;

                        case GroupType.Closed:
                            filters += "{\"groups_show_only\":\"{\\\"name\\\":\\\"closed_groups\\\",\\\"args\\\":\\\"\\\"}\"}";
                            break;
                    }
                }

                objFdRequestParameter.UrlParameters.Add("filters", filters.Base64Encode());

                if (keyword == "My groups")
                    url = objFdRequestParameter.GenerateUrl(FdConstants.MyGroupUrl);

                else if (option.ToLower() == "Graph Search Url".ToLower() || option.ToLower() == "CustomGroupUrl".ToLower())
                {
                    url = objFdRequestParameter.GenerateUrl(keyword);

                    string[] arr = url.Split('?');
                    for (int i = 0; i < arr.Length; i++)
                    {
                        if (i == 0)
                        {
                            if (!string.IsNullOrEmpty(arr[i]))
                                modifiedUrl += arr[i] + "?";
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(arr[i]))
                                modifiedUrl += arr[i] + "&";
                        }
                    }
                    modifiedUrl = modifiedUrl.Trim('&').Trim('?');

                    url = modifiedUrl;

                }
                else
                    url = objFdRequestParameter.GenerateUrl(FdConstants.FbGroupSearchUrl);

                if (url.EndsWith("="))
                    url = Regex.Replace(url, "&filters=", "");

                var searchFanpageResponse = _httpHelper.GetRequest(url);

                groupScraperResponseHandler = new GroupScraperResponseHandler(searchFanpageResponse, new List<GroupDetails>());

                paginationResponseHandler = new NewsFeedPaginationResonseHandler(searchFanpageResponse, false, string.Empty);

                groupScraperResponseHandler.PageletData = paginationResponseHandler.PageletData;

                groupScraperResponseHandler.ObjFdScraperResponseParameters.FinalEncodedQuery = paginationResponseHandler.ObjFdScraperResponseParameters.FinalEncodedQuery;

                if (!string.IsNullOrEmpty(groupScraperResponseHandler.PageletData))
                    groupScraperResponseHandler.HasMoreResults = true;
            }
            else
            {
                var paginationData = responseHandler.PageletData;

                objFdRequestParameter.UrlParameters.Clear();

                objFdRequestParameter.UrlParameters.Add("dpr", "1");
                objFdRequestParameter.UrlParameters.Add("data", paginationData);
                objFdRequestParameter.UrlParameters.Add("__req", "10");
                CommonUrlParameters(account, objFdRequestParameter);

                url = objFdRequestParameter.GenerateUrl(FdConstants.FbFanpageLikerPageleUrl);

                var paginationResponse = _httpHelper.GetRequest(url);

                groupScraperResponseHandler = new GroupScraperResponseHandler(paginationResponse, new List<GroupDetails>());
                paginationResponseHandler = new NewsFeedPaginationResonseHandler(paginationResponse, true, responseHandler.ObjFdScraperResponseParameters.FinalEncodedQuery);

                groupScraperResponseHandler.PageletData = paginationResponseHandler.PageletData;

                groupScraperResponseHandler.ObjFdScraperResponseParameters.FinalEncodedQuery = paginationResponseHandler.ObjFdScraperResponseParameters.FinalEncodedQuery;

                groupScraperResponseHandler.HasMoreResults = !string.IsNullOrEmpty(groupScraperResponseHandler.PageletData);
            }

            return groupScraperResponseHandler;
        }

        #region Scrap Post From NewsFeed
        /// <summary>
        ///  Use this methid to scrap posts from newsFeed
        /// </summary>
        /// <param name="account"></param>
        /// <param name="responseHandler"></param>
        /// <returns>
        ///  ScrapPostListFromNewsFeedResponseHandler object containing List of Post and Pagination token
        /// </returns>

        public async Task<ScrapNewPostListFromNewsFeedResponseHandler> GetPostListFromNewsFeedAsync
             (DominatorAccountModel account, ScrapNewPostListFromNewsFeedResponseHandler responseHandler)
        {
            string url;

            int count = 0;

            FdRequestParameter objFdRequestParameter = new FdRequestParameter();

            ScrapNewPostListFromNewsFeedResponseHandler newsFeedResponseHandler;

            if (responseHandler == null)
            {

                url = FdConstants.FbHomeUrl;

                var newsFeedResponse = await _httpHelper.GetRequestAsync(url, account.Token);

                List<KeyValuePair<string, string>> listPostReaction = new AdReactionListResponseHandler(newsFeedResponse).ListPostReaction;

                newsFeedResponseHandler = new ScrapNewPostListFromNewsFeedResponseHandler(newsFeedResponse, null, listPostReaction);

                if (newsFeedResponseHandler.ListFacebookAdsDetails.Count == 0)
                {
                    try
                    {
                        objFdRequestParameter.UrlParameters.Clear();
                        await _delayService.DelayAsync(1000);
                        count++;
                        var fetchStream = $"fetchstream_{count}";
                        objFdRequestParameter.UrlParameters.Add("fb_dtsg", Uri.UnescapeDataString(account.SessionId));
                        objFdRequestParameter.UrlParameters.Add("dpr", "1");
                        objFdRequestParameter.UrlParameters.Add("ajaxpipe", "1");
                        objFdRequestParameter.UrlParameters.Add("ajaxpipe_token", newsFeedResponseHandler.ObjPajination.AjaxToken);
                        objFdRequestParameter.UrlParameters.Add("no_script_path", "1");
                        objFdRequestParameter.UrlParameters.Add("data", newsFeedResponseHandler.ObjPajination.Pagelet);
                        objFdRequestParameter.UrlParameters.Add("__a", "fetchstream_1");
                        objFdRequestParameter.UrlParameters.Add("__dyn", FdConstants.DynParameter);
                        objFdRequestParameter.UrlParameters.Add("__req", fetchStream);
                        objFdRequestParameter.UrlParameters.Add("__be", "1");
                        objFdRequestParameter.UrlParameters.Add("__adt", count.ToString());
                        objFdRequestParameter.UrlParameters.Add("ajaxpipe_fetch_stream", "1");

                        url = objFdRequestParameter.GenerateUrl(FdConstants.NewsFeedPaginationUrl);

                        var paginationResponse = await _httpHelper.GetRequestAsync(url, account.Token);

                        listPostReaction = new AdReactionListResponseHandler(paginationResponse).ListPostReaction;

                        newsFeedResponseHandler = new ScrapNewPostListFromNewsFeedResponseHandler(paginationResponse, newsFeedResponseHandler.ObjPajination, listPostReaction);

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.StackTrace);
                    }
                }
            }
            else
            {

                string fetchStream;

                if (string.IsNullOrEmpty(responseHandler.ObjFdScraperResponseParameters.FetchStream))
                {
                    fetchStream = "fetchstream_1";
                    count++;
                }
                else
                {
                    count = Int32.Parse(FdFunctions.GetIntegerOnlyString(responseHandler.ObjFdScraperResponseParameters.FetchStream));
                    count++;
                    fetchStream = $"fetchstream_{count}";
                }
                objFdRequestParameter.UrlParameters.Clear();

                fetchStream = $"fetchstream_{count}";
                objFdRequestParameter.UrlParameters.Add("fb_dtsg", Uri.UnescapeDataString(account.SessionId));
                objFdRequestParameter.UrlParameters.Add("dpr", "1");
                objFdRequestParameter.UrlParameters.Add("ajaxpipe", "1");
                objFdRequestParameter.UrlParameters.Add("ajaxpipe_token", responseHandler.ObjPajination.AjaxToken);
                objFdRequestParameter.UrlParameters.Add("no_script_path", "1");
                objFdRequestParameter.UrlParameters.Add("data", responseHandler.ObjPajination.Pagelet);
                objFdRequestParameter.UrlParameters.Add("__a", "fetchstream_1");
                objFdRequestParameter.UrlParameters.Add("__dyn", FdConstants.DynParameter);
                objFdRequestParameter.UrlParameters.Add("__req", fetchStream);
                objFdRequestParameter.UrlParameters.Add("__be", "1");
                objFdRequestParameter.UrlParameters.Add("__adt", count.ToString());
                objFdRequestParameter.UrlParameters.Add("ajaxpipe_fetch_stream", "1");

                url = objFdRequestParameter.GenerateUrl(FdConstants.NewsFeedPaginationUrl);

                var paginationResponse = await _httpHelper.GetRequestAsync(url, account.Token);

                List<KeyValuePair<string, string>> listPostReaction = new AdReactionListResponseHandler(paginationResponse).ListPostReaction;

                newsFeedResponseHandler = new ScrapNewPostListFromNewsFeedResponseHandler(paginationResponse, responseHandler.ObjPajination, listPostReaction);

            }

            return newsFeedResponseHandler;
        }

        public IResponseHandler GetPostListFromNewsFeed
            (DominatorAccountModel account, IResponseHandler responseHandler, string newsFeedUrl = null)
        {
            string url;

            int count = 1;

            FdRequestParameter objFdRequestParameter = new FdRequestParameter();

            ScrapPostListFromNewsFeedResponseHandlerNew newsFeedResponseHandler;

            if (responseHandler == null)
            {
                url = FdConstants.FbHomeUrl;

                var newsFeedResponse = _httpHelper.GetRequest(url);

                List<KeyValuePair<string, string>> listPostReaction = new PostReactionListResponseHandler(newsFeedResponse).ListPostReaction;

                newsFeedResponseHandler = new ScrapPostListFromNewsFeedResponseHandlerNew(newsFeedResponse, null, listPostReaction, _httpHelper);

                if (newsFeedResponseHandler.ObjFdScraperResponseParameters.ListPostDetails != null
                    && newsFeedResponseHandler.ObjFdScraperResponseParameters.ListPostDetails.Count == 0)
                {
                    try
                    {
                        objFdRequestParameter.UrlParameters.Clear();

                        //                        if (responseHandler != null)
                        //                            count = Int32.Parse(
                        //                                FdFunctions.GetIntegerOnlyString(responseHandler.ObjFdScraperResponseParameters
                        //                                    .FetchStream));
                        count++;
                        var fetchStream = $"fetchstream_{count}";
                        objFdRequestParameter.UrlParameters.Add("fb_dtsg", Uri.UnescapeDataString(account.SessionId));
                        objFdRequestParameter.UrlParameters.Add("dpr", "1");
                        objFdRequestParameter.UrlParameters.Add("ajaxpipe", "1");
                        objFdRequestParameter.UrlParameters.Add("ajaxpipe_token", newsFeedResponseHandler.ObjFdScraperResponseParameters.AjaxToken);
                        objFdRequestParameter.UrlParameters.Add("no_script_path", "1");
                        objFdRequestParameter.UrlParameters.Add("data", newsFeedResponseHandler.PageletData);
                        objFdRequestParameter.UrlParameters.Add("__a", "fetchstream_1");
                        objFdRequestParameter.UrlParameters.Add("__dyn", FdConstants.DynParameter);
                        objFdRequestParameter.UrlParameters.Add("__req", fetchStream);
                        objFdRequestParameter.UrlParameters.Add("__be", "1");
                        objFdRequestParameter.UrlParameters.Add("__adt", count.ToString());
                        objFdRequestParameter.UrlParameters.Add("ajaxpipe_fetch_stream", "1");

                        url = objFdRequestParameter.GenerateUrl(FdConstants.NewsFeedPaginationUrl);

                        var paginationResponse = _httpHelper.GetRequest(url);

                        listPostReaction = new PostReactionListResponseHandler(paginationResponse).ListPostReaction;

                        newsFeedResponseHandler = new ScrapPostListFromNewsFeedResponseHandlerNew(paginationResponse, newsFeedResponseHandler.ObjFdScraperResponseParameters, listPostReaction, _httpHelper);

                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                }


            }
            else
            {
                objFdRequestParameter.UrlParameters.Clear();

                string fetchStream;

                if (string.IsNullOrEmpty(responseHandler.ObjFdScraperResponseParameters.FetchStream))
                {
                    fetchStream = "fetchstream_1";
                    count++;
                }
                else
                {
                    count = Int32.Parse(FdFunctions.GetIntegerOnlyString(responseHandler.ObjFdScraperResponseParameters.FetchStream));
                    count++;
                    fetchStream = $"fetchstream_{count}";
                }
                objFdRequestParameter.UrlParameters.Add("fb_dtsg", Uri.UnescapeDataString(account.SessionId));
                objFdRequestParameter.UrlParameters.Add("dpr", "1");
                objFdRequestParameter.UrlParameters.Add("ajaxpipe", "1");
                objFdRequestParameter.UrlParameters.Add("ajaxpipe_token", responseHandler.ObjFdScraperResponseParameters.AjaxToken);
                objFdRequestParameter.UrlParameters.Add("no_script_path", "1");
                objFdRequestParameter.UrlParameters.Add("data", responseHandler.PageletData);
                objFdRequestParameter.UrlParameters.Add("__a", "1");
                objFdRequestParameter.UrlParameters.Add("__dyn", FdConstants.DynParameter);
                objFdRequestParameter.UrlParameters.Add("__req", fetchStream);
                objFdRequestParameter.UrlParameters.Add("__be", "1");
                objFdRequestParameter.UrlParameters.Add("__adt", count.ToString());
                objFdRequestParameter.UrlParameters.Add("ajaxpipe_fetch_stream", "1");

                url = objFdRequestParameter.GenerateUrl(FdConstants.NewsFeedPaginationUrl);

                var paginationResponse = _httpHelper.GetRequest(url);

                List<KeyValuePair<string, string>> listPostReaction = new PostReactionListResponseHandler(paginationResponse).ListPostReaction;

                newsFeedResponseHandler = new ScrapPostListFromNewsFeedResponseHandlerNew(paginationResponse,
                    responseHandler.ObjFdScraperResponseParameters, listPostReaction, _httpHelper);

            }

            return newsFeedResponseHandler;
        }






        #endregion

        #region Scrap Post From Fanpages

        /// <summary>
        /// Use this methid to scrap posts from Pages
        /// </summary>
        /// <param name="account"></param>
        /// <param name="fanpageUrl"></param>
        /// <param name="responseHandler"></param>
        /// <returns>
        /// ScrapPostListFromFanpageResponseHandler object containing List of Post and Pagination token   
        /// </returns>
        public IResponseHandler GetPostListFromFanpages
                 (DominatorAccountModel account, string fanpageUrl, IResponseHandler responseHandler)
        {
            string url;

            FdRequestParameter objFdRequestParameter = new FdRequestParameter();

            if (responseHandler == null)
            {
                var pageId = fanpageUrl;

                if (!pageId.Contains(FdConstants.FbHomeUrl) &&
                    FdFunctions.IsIntegerOnly(pageId))
                    url = FdConstants.FanpagePostUrl(pageId);
                else
                {
                    pageId = GetPageIdFromUrl(account, fanpageUrl);

                    url = FdConstants.FanpagePostUrl(pageId);
                }

                var newsFeedResponse = _httpHelper.GetRequest(url);

                List<KeyValuePair<string, string>> listPostReaction = new PostReactionListResponseHandler(newsFeedResponse).ListPostReaction;

                return new ScrapPostListFromFanpageResponseHandler(newsFeedResponse, listPostReaction) { EntityId = pageId };

            }

            else
            {
                objFdRequestParameter.UrlParameters.Clear();

                objFdRequestParameter.UrlParameters.Add("fb_dtsg", Uri.UnescapeDataString(account.SessionId));
                objFdRequestParameter.UrlParameters.Add("page_id", responseHandler.EntityId);
                objFdRequestParameter.UrlParameters.Add("cursor", responseHandler.PageletData);
                objFdRequestParameter.UrlParameters.Add("surface", "www_pages_posts");
                objFdRequestParameter.UrlParameters.Add("unit_count", "100");
                objFdRequestParameter.UrlParameters.Add("__user", account.AccountBaseModel.UserId);
                objFdRequestParameter.UrlParameters.Add("dpr", "1");
                objFdRequestParameter.UrlParameters.Add("__a", "fetchstream_1");
                objFdRequestParameter.UrlParameters.Add("__dyn", FdConstants.DynParameter);
                objFdRequestParameter.UrlParameters.Add("__req", "11");
                objFdRequestParameter.UrlParameters.Add("__be", "1");

                url = objFdRequestParameter.GenerateUrl(FdConstants.FanpagePostPaginationUrl);

                var paginationResponse = _httpHelper.GetRequest(url);

                List<KeyValuePair<string, string>> listPostReaction = new PostReactionListResponseHandler(paginationResponse).ListPostReaction;

                return new ScrapPostListFromFanpageResponseHandler(paginationResponse, listPostReaction)
                {
                    EntityId = responseHandler.EntityId
                };

            }

        }

        public IResponseHandler GetPostListFromFanpagesNew
            (DominatorAccountModel account, IResponseHandler responseHandler, string fanpageUrl)
        {
            string url;

            FdRequestParameter objFdRequestParameter = new FdRequestParameter();

            ScrapPostListFromFanpageResponseHandlerNew fanpagePostResponseHandler;

            if (responseHandler == null)
            {
                var pageId = fanpageUrl;

                if (pageId.Contains(FdConstants.FbHomeUrl))
                {
                    pageId = GetPageIdFromUrl(account, fanpageUrl);

                    url = FdConstants.FanpagePostUrl(pageId);
                }
                else
                    url = FdConstants.FanpagePostUrl(pageId);

                var newsFeedResponse = _httpHelper.GetRequest(url);

                List<KeyValuePair<string, string>> listPostReaction = new PostReactionListResponseHandler(newsFeedResponse).ListPostReaction;


                fanpagePostResponseHandler =
                    new ScrapPostListFromFanpageResponseHandlerNew(newsFeedResponse, listPostReaction)
                    {
                        EntityId = pageId
                    };

            }

            else
            {
                objFdRequestParameter.UrlParameters.Clear();

                objFdRequestParameter.UrlParameters.Add("fb_dtsg", Uri.UnescapeDataString(account.SessionId));
                objFdRequestParameter.UrlParameters.Add("page_id", responseHandler.EntityId);
                objFdRequestParameter.UrlParameters.Add("cursor", responseHandler.PageletData);
                objFdRequestParameter.UrlParameters.Add("surface", "www_pages_posts");
                objFdRequestParameter.UrlParameters.Add("unit_count", "50");
                objFdRequestParameter.UrlParameters.Add("__user", account.AccountBaseModel.UserId);
                objFdRequestParameter.UrlParameters.Add("dpr", "1");
                objFdRequestParameter.UrlParameters.Add("__a", "fetchstream_1");
                objFdRequestParameter.UrlParameters.Add("__dyn", FdConstants.DynParameter);
                objFdRequestParameter.UrlParameters.Add("__req", "11");
                objFdRequestParameter.UrlParameters.Add("__be", "1");

                url = objFdRequestParameter.GenerateUrl(FdConstants.FanpagePostPaginationUrl);

                var paginationResponse = _httpHelper.GetRequest(url);

                List<KeyValuePair<string, string>> listPostReaction = new PostReactionListResponseHandler(paginationResponse).ListPostReaction;

                fanpagePostResponseHandler =
                    new ScrapPostListFromFanpageResponseHandlerNew(paginationResponse, listPostReaction)
                    {
                        EntityId = responseHandler.EntityId
                    };

            }

            return fanpagePostResponseHandler;
        }

        #endregion

        #region Scrap Post From Groups

        /// <summary>
        /// Use this methid to scrap posts from Groups
        /// </summary>
        /// <param name="account"></param>
        /// <param name="responseHandler"></param>
        /// <param name="groupUrl"></param>
        /// <returns>
        /// ScrapPostListFromFanpageResponseHandler object containing List of Post and Pagination token
        /// </returns>
        public IResponseHandler GetPostListFromGroups
            (DominatorAccountModel account, IResponseHandler responseHandler, string groupUrl)
        {
            string url;

            string groupId = string.Empty;

            string ajaxPipeToken = string.Empty;

            IResponseHandler timelinePostResponseHandler;

            FdRequestParameter objFdRequestParameter = new FdRequestParameter();

            if (responseHandler == null)
            {
                url = groupUrl;

                if (url.Contains(FdConstants.FbHomeUrl))
                {
                    var groupPageResponse = _httpHelper.GetRequest(url);

                    groupId = FdRegexUtility.FirstMatchExtractor(groupPageResponse.Response, FdConstants.GroupIdRegex);

                    groupId = FdFunctions.GetIntegerOnlyString(groupId);

                    url = FdConstants.FbGroupUrl(groupId);
                }

                if (groupUrl.Contains("ref=direct") && !string.IsNullOrEmpty(groupId))
                    url = FdConstants.FbGroupUrl(groupId) + "/?ref=direct";

                if (string.IsNullOrEmpty(groupId))
                {
                    try
                    {
                        groupUrl = groupUrl.Split('?')[0];

                        if (!groupUrl.Contains("/about/"))
                            groupUrl = groupUrl[groupUrl.Length - 1] == '/'
                                ? groupUrl + "about/"
                                : groupUrl + "/about/";

                        var groupPageResponse = _httpHelper.GetRequest(groupUrl);

                        groupId = FdRegexUtility.FirstMatchExtractor(groupPageResponse.Response, FdConstants.GroupIdRegex);
                        //groupId = FdRegexUtility.FirstMatchExtractor(groupPageResponse.Response, FdConstants.GroupIdRegex);
                        groupId = FdFunctions.GetIntegerOnlyString(groupId);

                        ajaxPipeToken = FdRegexUtility.FirstMatchExtractor(groupPageResponse.Response, FdConstants.AjaxPipeTokenRegex);
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                    url = $"{FdConstants.FbHomeUrl}ajax/home/generic.php";

                    objFdRequestParameter.UrlParameters.Add("fb_dtsg", Uri.UnescapeDataString(account.SessionId));
                    objFdRequestParameter.UrlParameters.Add("dpr", "1");
                    objFdRequestParameter.UrlParameters.Add("ajaxpipe", "1");
                    objFdRequestParameter.UrlParameters.Add("ajaxpipe_token", ajaxPipeToken);
                    objFdRequestParameter.UrlParameters.Add("no_script_path", "1");
                    objFdRequestParameter.UrlParameters.Add("ref", "direct");
                    objFdRequestParameter.UrlParameters.Add("path", $"/groups/{groupId}/");
                    objFdRequestParameter.UrlParameters.Add("__user", account.AccountBaseModel.UserId);
                    objFdRequestParameter.UrlParameters.Add("__a", "1");
                    objFdRequestParameter.UrlParameters.Add("__dyn", FdConstants.DynParameter);
                    objFdRequestParameter.UrlParameters.Add("__req", "fetchstream_2");
                    objFdRequestParameter.UrlParameters.Add("__be", "1");
                    objFdRequestParameter.UrlParameters.Add("__adt", "2");
                    objFdRequestParameter.UrlParameters.Add("ajaxpipe_fetch_stream", "1");

                    url = objFdRequestParameter.GenerateUrl(url);
                }

                var newsFeedResponse = _httpHelper.GetRequest(url);

                List<KeyValuePair<string, string>> listPostReaction = new PostReactionListResponseHandler(newsFeedResponse).ListPostReaction;

                timelinePostResponseHandler = new ScrapGroupPostListResponseHandler(newsFeedResponse, listPostReaction, null);

                if (string.IsNullOrEmpty(timelinePostResponseHandler.ObjFdScraperResponseParameters.AjaxToken))
                    timelinePostResponseHandler.ObjFdScraperResponseParameters.AjaxToken = ajaxPipeToken;

            }

            else
            {
                objFdRequestParameter.UrlParameters.Clear();
                objFdRequestParameter.UrlParameters.Add("fb_dtsg", Uri.UnescapeDataString(account.SessionId));
                objFdRequestParameter.UrlParameters.Add("dpr", "1");
                objFdRequestParameter.UrlParameters.Add("ajaxpipe", "1");
                objFdRequestParameter.UrlParameters.Add("ajaxpipe_token", responseHandler.ObjFdScraperResponseParameters.AjaxToken);
                objFdRequestParameter.UrlParameters.Add("no_script_path", "1");
                objFdRequestParameter.UrlParameters.Add("data", responseHandler.PageletData);
                objFdRequestParameter.UrlParameters.Add("__user", account.AccountBaseModel.UserId);
                objFdRequestParameter.UrlParameters.Add("__a", "1");
                objFdRequestParameter.UrlParameters.Add("__dyn", FdConstants.DynParameter);
                objFdRequestParameter.UrlParameters.Add("__req", "fetchstream_2");
                objFdRequestParameter.UrlParameters.Add("__be", "1");
                objFdRequestParameter.UrlParameters.Add("__adt", "2");
                objFdRequestParameter.UrlParameters.Add("ajaxpipe_fetch_stream", "1");

                url = objFdRequestParameter.GenerateUrl(FdConstants.GroupPostPaginationUrl);

                var paginationResponse = _httpHelper.GetRequest(url);

                if (paginationResponse != null)
                    paginationResponse = _httpHelper.GetRequest(url);

                var listPostReaction = new PostReactionListResponseHandler(paginationResponse).ListPostReaction;

                timelinePostResponseHandler = new ScrapGroupPostListResponseHandler(paginationResponse, listPostReaction, responseHandler.ObjFdScraperResponseParameters.AjaxToken);

            }

            return timelinePostResponseHandler;
        }



        public IResponseHandler GetPostListFromGroupsNew
            (DominatorAccountModel account, IResponseHandler responseHandler, string groupUrl)
        {
            string url;
            string groupId = string.Empty;
            string ajaxPipeToken = string.Empty;
            ScrapGroupPostListResponseHandlerNew timelinePostResponseHandler;
            FdRequestParameter objFdRequestParameter = new FdRequestParameter();

            if (responseHandler == null)
            {
                url = groupUrl;

                if (url.Contains(FdConstants.FbHomeUrl))
                {
                    var groupPageResponse = _httpHelper.GetRequest(url);

                    if (groupPageResponse.Exception != null && groupPageResponse.Exception.Message.Equals("The operation has timed out"))
                        groupPageResponse = _httpHelper.GetRequest(url);

                    groupId = groupPageResponse.Response == null
                        ? "0"
                        : FdRegexUtility.FirstMatchExtractor(groupPageResponse.Response, FdConstants.GroupIdRegex);

                    groupId = FdFunctions.GetIntegerOnlyString(groupId);

                    if (groupId == "0")
                        return null;

                    url = FdConstants.FbGroupUrl(groupId);
                }

                if (groupUrl.Contains("ref=direct") && !string.IsNullOrEmpty(groupId))
                    url = FdConstants.FbGroupUrl(groupId) + "/?ref=direct";

                if (string.IsNullOrEmpty(groupId))
                {
                    try
                    {
                        groupUrl = groupUrl.Split('?')[0];

                        if (!groupUrl.Contains("/about/"))
                        {
                            groupUrl = groupUrl[groupUrl.Length - 1] == '/'
                                ? groupUrl + "about/"
                                : groupUrl + "/about/";

                        }
                        var groupPageResponse = _httpHelper.GetRequest(groupUrl);

                        groupId = FdRegexUtility.FirstMatchExtractor(groupPageResponse.Response, FdConstants.GroupIdRegex);
                        //groupId = FdRegexUtility.FirstMatchExtractor(groupPageResponse.Response, FdConstants.GroupIdRegex);
                        groupId = FdFunctions.GetIntegerOnlyString(groupId);

                        ajaxPipeToken = FdRegexUtility.FirstMatchExtractor(groupPageResponse.Response, FdConstants.AjaxPipeTokenRegex);
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                    url = $"{FdConstants.FbHomeUrl}ajax/home/generic.php";

                    objFdRequestParameter.UrlParameters.Add("fb_dtsg", Uri.UnescapeDataString(account.SessionId));
                    objFdRequestParameter.UrlParameters.Add("dpr", "1");
                    objFdRequestParameter.UrlParameters.Add("ajaxpipe", "1");
                    objFdRequestParameter.UrlParameters.Add("ajaxpipe_token", ajaxPipeToken);
                    objFdRequestParameter.UrlParameters.Add("no_script_path", "1");
                    objFdRequestParameter.UrlParameters.Add("ref", "direct");
                    objFdRequestParameter.UrlParameters.Add("path", $"/groups/{groupId}/");
                    objFdRequestParameter.UrlParameters.Add("__user", account.AccountBaseModel.UserId);
                    objFdRequestParameter.UrlParameters.Add("__a", "1");
                    objFdRequestParameter.UrlParameters.Add("__dyn", FdConstants.DynParameter);
                    objFdRequestParameter.UrlParameters.Add("__req", "fetchstream_2");
                    objFdRequestParameter.UrlParameters.Add("__be", "1");
                    objFdRequestParameter.UrlParameters.Add("__adt", "2");
                    objFdRequestParameter.UrlParameters.Add("ajaxpipe_fetch_stream", "1");

                    url = objFdRequestParameter.GenerateUrl(url);
                }

                var groupResponse = _httpHelper.GetRequest(url);

                List<KeyValuePair<string, string>> listPostReaction = new PostReactionListResponseHandler(groupResponse).ListPostReaction;

                timelinePostResponseHandler = new ScrapGroupPostListResponseHandlerNew(groupResponse, listPostReaction, null);

                if (groupResponse.Exception != null && string.IsNullOrEmpty(timelinePostResponseHandler.ObjFdScraperResponseParameters.AjaxToken))
                    timelinePostResponseHandler.ObjFdScraperResponseParameters.AjaxToken = ajaxPipeToken;

            }
            else
            {
                objFdRequestParameter.UrlParameters.Clear();
                objFdRequestParameter.UrlParameters.Add("fb_dtsg", Uri.UnescapeDataString(account.SessionId));
                objFdRequestParameter.UrlParameters.Add("dpr", "1");
                objFdRequestParameter.UrlParameters.Add("ajaxpipe", "1");
                objFdRequestParameter.UrlParameters.Add("ajaxpipe_token", responseHandler.ObjFdScraperResponseParameters.AjaxToken);
                objFdRequestParameter.UrlParameters.Add("no_script_path", "1");
                objFdRequestParameter.UrlParameters.Add("data", responseHandler.PageletData);
                objFdRequestParameter.UrlParameters.Add("__user", account.AccountBaseModel.UserId);
                objFdRequestParameter.UrlParameters.Add("__a", "1");
                objFdRequestParameter.UrlParameters.Add("__dyn", FdConstants.DynParameter);
                objFdRequestParameter.UrlParameters.Add("__req", "fetchstream_2");
                objFdRequestParameter.UrlParameters.Add("__be", "1");
                objFdRequestParameter.UrlParameters.Add("__adt", "2");
                objFdRequestParameter.UrlParameters.Add("ajaxpipe_fetch_stream", "1");

                url = objFdRequestParameter.GenerateUrl(FdConstants.GroupPostPaginationUrl);

                var paginationResponse = _httpHelper.GetRequest(url);
                //some time operation timeout exception is coming so one more hit on ex time
                if (paginationResponse.Exception != null)
                    paginationResponse = _httpHelper.GetRequest(url);

                List<KeyValuePair<string, string>> listPostReaction = new PostReactionListResponseHandler(paginationResponse).ListPostReaction;

                timelinePostResponseHandler = new ScrapGroupPostListResponseHandlerNew(paginationResponse,
                    listPostReaction, responseHandler.ObjFdScraperResponseParameters.AjaxToken);
            }

            return timelinePostResponseHandler;
        }

        #endregion

        public IResponseHandler GetPostListFromTimeline
            (DominatorAccountModel account, IResponseHandler responseHandler, string accountUrl = null)
        {
            string url;

            FdRequestParameter objFdRequestParameter = new FdRequestParameter();
            if (responseHandler == null)
            {
                url = FdConstants.FbOwnTimelineUrl(account.AccountBaseModel.UserId);

                var timelineResponse = _httpHelper.GetRequest(url);

                List<KeyValuePair<string, string>> listPostReaction = new PostReactionListResponseHandler(timelineResponse).ListPostReaction;

                return new ScrapPostListFromTimelineResponseHandler(timelineResponse, string.Empty, listPostReaction);
            }
            else
            {
                objFdRequestParameter.UrlParameters.Clear();
                objFdRequestParameter.UrlParameters.Add("fb_dtsg", Uri.UnescapeDataString(account.SessionId));
                objFdRequestParameter.UrlParameters.Add("dpr", "1");
                objFdRequestParameter.UrlParameters.Add("ajaxpipe", "1");
                objFdRequestParameter.UrlParameters.Add("ajaxpipe_token", responseHandler.ObjFdScraperResponseParameters.AjaxToken);
                objFdRequestParameter.UrlParameters.Add("no_script_path", "1");
                objFdRequestParameter.UrlParameters.Add("data", responseHandler.PageletData);
                objFdRequestParameter.UrlParameters.Add("__a", "1");
                objFdRequestParameter.UrlParameters.Add("__dyn", FdConstants.DynParameter);
                objFdRequestParameter.UrlParameters.Add("__req", "fetchstream_1");
                objFdRequestParameter.UrlParameters.Add("__be", "1");
                objFdRequestParameter.UrlParameters.Add("__adt", "1");
                objFdRequestParameter.UrlParameters.Add("ajaxpipe_fetch_stream", "1");

                url = objFdRequestParameter.GenerateUrl(FdConstants.TimeLinePaginationUrl);

                var paginationResponse = _httpHelper.GetRequest(url);

                List<KeyValuePair<string, string>> listPostReaction = new PostReactionListResponseHandler(paginationResponse).ListPostReaction;

                return new ScrapPostListFromTimelineResponseHandler(paginationResponse, responseHandler.ObjFdScraperResponseParameters.AjaxToken, listPostReaction);

            }

        }

        #region Scrap Post From Groups

        /// <summary>
        /// Use this methid to scrap posts from Groups
        /// </summary>
        /// <returns>
        /// ScrapPostListFromFanpageResponseHandler object containing List of Post and Pagination token
        /// </returns>
        /*public ScrapGroupPostListResponseHandler GetPostListFromGroupsNew
            (DominatorAccountModel account, string groupUrl, ScrapGroupPostListResponseHandler responseHandler)
        {
            string url;

            ScrapGroupPostListResponseHandler timelinePostResponseHandler;

            FdRequestParameter objFdRequestParameter = new FdRequestParameter();

            if (responseHandler == null)
            {
                url = groupUrl;

                if (url.Contains(FdConstants.FbHomeUrl))
                {
                    var groupPageResponse = _httpHelper.GetRequest(url);

                    string groupId = FdRegexUtility.FirstMatchExtractor(groupPageResponse.Response, FdConstants.GroupIdRegex);

                    //string groupId = FdRegexUtility.FirstMatchExtractor(groupPageResponse.Response, FdConstants.GroupIdRegex);

                    groupId = FdFunctions.GetIntegerOnlyString(groupId);

                    url = FdConstants.FbGroupUrl(groupId);
                }

                var newsFeedResponse = _httpHelper.GetRequest(url);

                timelinePostResponseHandler = new ScrapGroupPostListResponseHandler(newsFeedResponse, null, null);
            }

            else
            {
                objFdRequestParameter.UrlParameters.Clear();

                objFdRequestParameter.UrlParameters.Add("dpr", "1");
                objFdRequestParameter.UrlParameters.Add("ajaxpipe", "1");
                objFdRequestParameter.UrlParameters.Add("ajaxpipe_token", responseHandler.AjaxToken);
                objFdRequestParameter.UrlParameters.Add("no_script_path", "1");
                objFdRequestParameter.UrlParameters.Add("data", responseHandler.Pagelet);
                objFdRequestParameter.UrlParameters.Add("__user", account.AccountBaseModel.UserId);
                objFdRequestParameter.UrlParameters.Add("__a", "1");
                objFdRequestParameter.UrlParameters.Add("__dyn", FdConstants.DynParameter);
                objFdRequestParameter.UrlParameters.Add("__req", "fetchstream_2");
                objFdRequestParameter.UrlParameters.Add("__be", "1");
                objFdRequestParameter.UrlParameters.Add("__adt", "2");
                objFdRequestParameter.UrlParameters.Add("ajaxpipe_fetch_stream", "1");

                url = objFdRequestParameter.GenerateUrl(FdConstants.GroupPostPaginationUrl);

                var paginationResponse = _httpHelper.GetRequest(url);

                timelinePostResponseHandler = new ScrapGroupPostListResponseHandler(paginationResponse, null, responseHandler.AjaxToken);

            }

            return timelinePostResponseHandler;
        }*/

        #endregion

        public bool GetVerifyAfterFanPageLiker(DominatorAccountModel accountModel, string url, string postId, string comment, ref string pageId)
        {
            try
            {
                var id = !url.Contains(FdConstants.FbHomeUrl) ? GetPageDetailsFromUrl(accountModel, url).FanPageID : url;

                if (string.IsNullOrEmpty(id))
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, accountModel.AccountBaseModel.AccountNetwork,
                        accountModel.AccountBaseModel.UserName, "FanpageLiker AfterAction", string.Format("LangKeyInvalidURL".FromResourceDictionary(), $"{url}"));
                    return false;
                }
                else
                    pageId = id;

                bool isSuccess = false;

                var isChangedActor = ChangeActor(accountModel, postId, id);

                if (isChangedActor)
                    isSuccess = string.IsNullOrEmpty(comment) ? LikeUnlikePost(accountModel, postId, ReactionType.Like, id) : CommentOnPost(accountModel, postId, comment, id).ObjFdScraperResponseParameters.IsCommentedOnPost;

                if (isSuccess)
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, accountModel.AccountBaseModel.AccountNetwork,
                        accountModel.UserName, "FanpageLiker AfterAction",
                        string.IsNullOrEmpty(comment)
                            ? string.Format("LangKeyLikedByPageSuccess".FromResourceDictionary(), $"{FdConstants.FbHomeUrl + postId}", $"{FdConstants.FbHomeUrl + id}")
                            : string.Format("LangKeyCommentedByPageSuccess".FromResourceDictionary(), $"{FdConstants.FbHomeUrl + postId}", $"{FdConstants.FbHomeUrl + id}"));
                    return true;
                }
                else
                    GlobusLogHelper.log.Info(Log.CustomMessage, accountModel.AccountBaseModel.AccountNetwork,
                        accountModel.AccountBaseModel.UserName, "FanpageLiker AfterAction", string.Format("LangKeyFailedToComment".FromResourceDictionary(), $"{FdConstants.FbHomeUrl + id}", $"{FdConstants.FbHomeUrl + id}"));

            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return false;
        }


        public IResponseHandler GetPostListFromFriendTimelineNew
            (DominatorAccountModel account, IResponseHandler responseHandler, string friendId)
        {
            string url;

            ScrapPostListFromTimelineResponseHandler timelinePostResponseHandler;

            FdRequestParameter objFdRequestParameter = new FdRequestParameter();
            if (responseHandler == null)
            {
                url = !friendId.Contains(FdConstants.FbHomeUrl) ? FdConstants.FbOwnTimelineUrl(friendId) : friendId;

                var timelineResponse = _httpHelper.GetRequest(url);

                var listPostReactionResponseHandler = new PostReactionListResponseHandler(timelineResponse);


                timelinePostResponseHandler = new ScrapPostListFromTimelineResponseHandler(timelineResponse,
                    string.Empty, listPostReactionResponseHandler.ListPostReaction)
                {
                    ObjFdScraperResponseParameters =
                    {
                        ListReactionPermission = listPostReactionResponseHandler.ListReactionPermission
                    }
                };

            }
            else
            {
                objFdRequestParameter.UrlParameters.Clear();

                objFdRequestParameter.UrlParameters.Add("fb_dtsg", Uri.UnescapeDataString(account.SessionId));
                objFdRequestParameter.UrlParameters.Add("dpr", "1");
                objFdRequestParameter.UrlParameters.Add("ajaxpipe", "1");
                objFdRequestParameter.UrlParameters.Add("ajaxpipe_token", responseHandler.ObjFdScraperResponseParameters.AjaxToken);
                objFdRequestParameter.UrlParameters.Add("no_script_path", "1");
                objFdRequestParameter.UrlParameters.Add("data", responseHandler.PageletData);
                objFdRequestParameter.UrlParameters.Add("__a", "1");
                objFdRequestParameter.UrlParameters.Add("__dyn", FdConstants.DynParameter);
                objFdRequestParameter.UrlParameters.Add("__req", "fetchstream_1");
                objFdRequestParameter.UrlParameters.Add("__be", "1");
                objFdRequestParameter.UrlParameters.Add("__adt", "1");
                objFdRequestParameter.UrlParameters.Add("ajaxpipe_fetch_stream", "1");

                url = objFdRequestParameter.GenerateUrl(FdConstants.TimeLinePaginationUrl);

                var paginationResponse = _httpHelper.GetRequest(url);

                var listPostReactionResponseHandler = new PostReactionListResponseHandler(paginationResponse);

                timelinePostResponseHandler = new ScrapPostListFromTimelineResponseHandler(paginationResponse,
                    responseHandler.ObjFdScraperResponseParameters.AjaxToken,
                    listPostReactionResponseHandler.ListPostReaction)
                {
                    ObjFdScraperResponseParameters =
                    {
                        ListReactionPermission = listPostReactionResponseHandler.ListReactionPermission
                    }
                };

            }


            return timelinePostResponseHandler;
        }

        #region Like or Unlike Post
        public bool LikeUnlikePost
            (DominatorAccountModel account, string postId, ReactionType objReactionType, string pageId = "")
        {
            FdRequestParameter objFdRequestParameter = new FdRequestParameter();

            var url = objFdRequestParameter.GenerateUrl(FdConstants.PostLikerUrl);

            objFdRequestParameter.PostDataParameters.Add("client_id", "1522337984580:698763417");
            objFdRequestParameter.PostDataParameters.Add("ft_ent_identifier", postId);

            switch (objReactionType)
            {
                case ReactionType.Like:
                    objFdRequestParameter.PostDataParameters.Add("reaction_type", ((int)ReactionType.Like).ToString());
                    break;

                case ReactionType.Love:
                    objFdRequestParameter.PostDataParameters.Add("reaction_type", ((int)ReactionType.Love).ToString());
                    break;

                case ReactionType.Haha:
                    objFdRequestParameter.PostDataParameters.Add("reaction_type", ((int)ReactionType.Haha).ToString());
                    break;

                case ReactionType.Wow:
                    objFdRequestParameter.PostDataParameters.Add("reaction_type", ((int)ReactionType.Wow).ToString());
                    break;

                case ReactionType.Sad:
                    objFdRequestParameter.PostDataParameters.Add("reaction_type", ((int)ReactionType.Sad).ToString());
                    break;

                case ReactionType.Angry:
                    objFdRequestParameter.PostDataParameters.Add("reaction_type", ((int)ReactionType.Angry).ToString());
                    break;

                case ReactionType.Unlike:
                    objFdRequestParameter.PostDataParameters.Add("reaction_type", ((int)ReactionType.Unlike).ToString());
                    break;

            }


            objFdRequestParameter.PostDataParameters.Add("root_id", "u_fetchstream_1_6");
            objFdRequestParameter.PostDataParameters.Add("session_id", "21480490");
            objFdRequestParameter.PostDataParameters.Add("source", "17");
            objFdRequestParameter.PostDataParameters.Add("feedback_referrer", "");
            objFdRequestParameter.PostDataParameters.Add("instance_id", "u_fetchstream_1_0");

            objFdRequestParameter.PostDataParameters.Add("av",
                string.IsNullOrEmpty(pageId) ? account.AccountBaseModel.UserId : pageId);

            objFdRequestParameter.PostDataParameters.Add("ft[tn]", "]*F");
            objFdRequestParameter.PostDataParameters.Add("__req", "11");

            objFdRequestParameter = CommonPostDataParameters(account, objFdRequestParameter);

            var postData = objFdRequestParameter.GetPostDataFromParameters();

            var request = _httpHelper.GetRequestParameter();

            request.ContentType = FdConstants.ContentType;

            _httpHelper.SetRequestParameter(request);

            var likeFanpageResponse = _httpHelper.PostRequest(url, postData);

            return new LikeFanpageResponseHandler(likeFanpageResponse).Status;

        }

        #endregion


        #region Like Fanpage

        public LikeFanpageResponseHandler LikeFanpage(DominatorAccountModel account, string fanpageId, CancellationToken token)
        {
            FdRequestParameter objFdRequestParameter = new FdRequestParameter();

            objFdRequestParameter.UrlParameters.Add("av", account.AccountBaseModel.UserId);
            objFdRequestParameter.UrlParameters.Add("dpr", "1");

            var url = objFdRequestParameter.GenerateUrl(FdConstants.FanpageLikerUrl);

            objFdRequestParameter.PostDataParameters.Add("fbpage_id", fanpageId);
            objFdRequestParameter.PostDataParameters.Add("add", "true");
            objFdRequestParameter.PostDataParameters.Add("reload", "false");
            objFdRequestParameter.PostDataParameters.Add("fan_origin", "search");
            objFdRequestParameter.PostDataParameters.Add("fan_source", "");
            objFdRequestParameter.PostDataParameters.Add("cat", "");
            objFdRequestParameter.PostDataParameters.Add("actor_id", "");
            objFdRequestParameter.PostDataParameters.Add("nctr[_mod]", "pagelet_loader_initial_browse_result");
            objFdRequestParameter.PostDataParameters.Add("__req", "11");

            objFdRequestParameter = CommonPostDataParameters(account, objFdRequestParameter);

            var postData = objFdRequestParameter.GetPostDataFromParameters();

            var request = _httpHelper.GetRequestParameter();

            request.ContentType = FdConstants.ContentType;

            _httpHelper.SetRequestParameter(request);

            var likeFanpageResponse = _httpHelper.PostRequest(url, postData);

            if (likeFanpageResponse.Response.Contains("errorSummary") && likeFanpageResponse.Response.Contains("Page like feature limit"))
            {
                Task.Delay(TimeSpan.FromSeconds(2000)).Wait(token);
                likeFanpageResponse = _httpHelper.PostRequest(url, postData);
            }

            return new LikeFanpageResponseHandler(likeFanpageResponse);

        }


        #endregion


        #region Comment On Post

        public CommentOnPostResponseHandler CommentOnPost(DominatorAccountModel account, string postId, string commentText, string pageId = "")
        {
            FdRequestParameter objFdRequestParameter = new FdRequestParameter();


            objFdRequestParameter.UrlParameters.Add("dpr", "1");

            var url = objFdRequestParameter.GenerateUrl(FdConstants.PostCommentUrl);

            objFdRequestParameter.PostDataParameters.Add("ft_ent_identifier", postId);
            objFdRequestParameter.PostDataParameters.Add("comment_text", Uri.EscapeDataString(commentText));
            objFdRequestParameter.PostDataParameters.Add("source", "17");
            objFdRequestParameter.PostDataParameters.Add("client_id", "1523504279817:2735809926");
            objFdRequestParameter.PostDataParameters.Add("session_id", "3f632d74");
            objFdRequestParameter.PostDataParameters.Add("reply_fbid", "");
            objFdRequestParameter.PostDataParameters.Add("attached_sticker_fbid", "0");
            objFdRequestParameter.PostDataParameters.Add("attached_photo_fbid", "0");
            objFdRequestParameter.PostDataParameters.Add("attached_video_fbid", "0");
            objFdRequestParameter.PostDataParameters.Add("attached_file_fbid", "0");
            objFdRequestParameter.PostDataParameters.Add("attached_share_url", "");

            objFdRequestParameter.PostDataParameters.Add("av",
                string.IsNullOrEmpty(pageId) ? account.AccountBaseModel.UserId : pageId);

            objFdRequestParameter.PostDataParameters.Add("__req", "11");

            objFdRequestParameter = CommonPostDataParameters(account, objFdRequestParameter);

            var postData = objFdRequestParameter.GetPostDataFromParameters();

            var request = _httpHelper.GetRequestParameter();

            request.ContentType = FdConstants.ContentType;

            _httpHelper.SetRequestParameter(request);

            var commentOnPostResponse = _httpHelper.PostRequest(url, postData);

            return new CommentOnPostResponseHandler(commentOnPostResponse);

        }


        #endregion


        #region Send Message

        public FdSendTextMessageResponseHandler SendTextMessage(DominatorAccountModel account, string friendId,
           string commentText, bool isMessageToFnpages = false)
        {
            FdRequestParameter objFdRequestParameter = new FdRequestParameter();

            objFdRequestParameter.UrlParameters.Add("dpr", "1");

            var url = objFdRequestParameter.GenerateUrl(FdConstants.SendDirectMessageUrl);

            var textmessageId = (617161).ToString() + DateTime.Now.ConvertToEpoch();

            objFdRequestParameter.PostDataParameters.Add("client", "mercury");
            objFdRequestParameter.PostDataParameters.Add("action_type", "ma-type:user-generated-message");
            objFdRequestParameter.PostDataParameters.Add("body", Uri.EscapeDataString(commentText));
            objFdRequestParameter.PostDataParameters.Add("ephemeral_ttl_mode", "0");
            objFdRequestParameter.PostDataParameters.Add("has_attachment", "false");
            objFdRequestParameter.PostDataParameters.Add("message_id", textmessageId);
            objFdRequestParameter.PostDataParameters.Add("offline_threading_id", textmessageId);
            objFdRequestParameter.PostDataParameters.Add("other_user_fbid", friendId);
            objFdRequestParameter.PostDataParameters.Add("signature_id", "5bc0758d");//5bc0758d  //6219f1c3
            objFdRequestParameter.PostDataParameters.Add("source", "source:chat:web");
            objFdRequestParameter.PostDataParameters.Add("specific_to_list[0]", $"fbid:{friendId}");
            objFdRequestParameter.PostDataParameters.Add("specific_to_list[1]", $"fbid:{account.AccountBaseModel.UserId}");

            if (!isMessageToFnpages)
                objFdRequestParameter.PostDataParameters.Add("tags[0]", "web:trigger:timeline");//web:trigger:timeline //web:trigger:fb_header_dock:jewel_thread
            else
            {
                objFdRequestParameter.PostDataParameters.Add("tags[0]", "entrypoint:fb_page:secondary_message_button");
                objFdRequestParameter.PostDataParameters.Add("tags[1]", "web:trigger:fb_page:secondary_message_button");
            }

            objFdRequestParameter.PostDataParameters.Add("timestamp", DateTime.Now.ConvertToEpoch().ToString());
            objFdRequestParameter.PostDataParameters.Add("ui_push_phase", "C3");
            objFdRequestParameter.PostDataParameters.Add("__req", "11");

            objFdRequestParameter = CommonPostDataParameters(account, objFdRequestParameter);

            var postData = objFdRequestParameter.GetPostDataFromParameters();

            var request = _httpHelper.GetRequestParameter();

            request.ContentType = FdConstants.ContentType;

            _httpHelper.SetRequestParameter(request);

            var commentOnPostResponse = _httpHelper.PostRequest(url, postData);

            return new FdSendTextMessageResponseHandler(commentOnPostResponse);

        }

        public async Task<IResponseHandler> SendTextMessageAsync(DominatorAccountModel account, SenderDetails senderDetails, string textMessage
            , CancellationToken cancellation)
        {
            FdSendTextMessageResponseHandler fdSendTextMessageResponseHandler = null;

            try
            {
                FdRequestParameter objFdRequestParameter = new FdRequestParameter();

                objFdRequestParameter.UrlParameters.Add("dpr", "1");

                var url = objFdRequestParameter.GenerateUrl(FdConstants.SendDirectMessageUrl);

                var textmessageId = (617161).ToString() + DateTime.Now.ConvertToEpoch();

                objFdRequestParameter.PostDataParameters.Add("client", "mercury");
                objFdRequestParameter.PostDataParameters.Add("action_type", "ma-type:user-generated-message");
                objFdRequestParameter.PostDataParameters.Add("body", Uri.EscapeDataString(textMessage));
                objFdRequestParameter.PostDataParameters.Add("ephemeral_ttl_mode", "0");
                objFdRequestParameter.PostDataParameters.Add("has_attachment", "false");
                objFdRequestParameter.PostDataParameters.Add("message_id", textmessageId);
                objFdRequestParameter.PostDataParameters.Add("offline_threading_id", textmessageId);
                objFdRequestParameter.PostDataParameters.Add("other_user_fbid", senderDetails.SenderId);
                objFdRequestParameter.PostDataParameters.Add("signature_id", "6219f1c3");
                objFdRequestParameter.PostDataParameters.Add("source", "source:chat:web");
                objFdRequestParameter.PostDataParameters.Add("specific_to_list[0]", $"fbid:{senderDetails.SenderId}");
                objFdRequestParameter.PostDataParameters.Add("specific_to_list[1]", $"fbid:{account.AccountBaseModel.UserId}");
                objFdRequestParameter.PostDataParameters.Add("tags[0]", "web:trigger:fb_header_dock:jewel_thread");
                objFdRequestParameter.PostDataParameters.Add("timestamp", DateTime.Now.ConvertToEpoch().ToString());
                objFdRequestParameter.PostDataParameters.Add("ui_push_phase", "C3");
                objFdRequestParameter.PostDataParameters.Add("__req", "11");

                objFdRequestParameter = CommonPostDataParameters(account, objFdRequestParameter);

                var postData = objFdRequestParameter.GetPostDataFromParameters();

                var request = _httpHelper.GetRequestParameter();

                //Assigning Browser Cookie to http
                if (request.Cookies.Count == 0)
                    request.Cookies = account.BrowserCookies;

                request.ContentType = FdConstants.ContentType;

                _httpHelper.SetRequestParameter(request);

                var commentOnPostResponse = await _httpHelper.PostRequestAsync(url, postData, cancellation);

                fdSendTextMessageResponseHandler = new FdSendTextMessageResponseHandler(commentOnPostResponse, senderDetails, textMessage);

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return fdSendTextMessageResponseHandler;

        }


        public FdSendTextMessageResponseHandler SendTextMessageWithLinkPreview
        (DominatorAccountModel account, string friendId, string commentText, bool isMessageToPage = false)
        {
            string shareType = string.Empty;

            string link = commentText;

            string sharerId = string.Empty;

            bool isLinkAvailable = FdFunctions.GetValidUrl(ref link);

            ScrapedLinkDetails objScraperDetails = null;

            if (isLinkAvailable)
                sharerId = UpdateLinkAndGetShareId(account, link, ref shareType, ref objScraperDetails);

            FdRequestParameter objFdRequestParameter = new FdRequestParameter();

            objFdRequestParameter.UrlParameters.Add("dpr", "1");

            var url = objFdRequestParameter.GenerateUrl(FdConstants.SendDirectMessageUrl);

            var textmessageId = (617161).ToString() + DateTime.Now.ConvertToEpoch();

            objFdRequestParameter.PostDataParameters.Add("client", "mercury");
            objFdRequestParameter.PostDataParameters.Add("action_type", "ma-type:user-generated-message");
            objFdRequestParameter.PostDataParameters.Add("body", Uri.EscapeDataString(commentText));
            objFdRequestParameter.PostDataParameters.Add("ephemeral_ttl_mode", "0");
            objFdRequestParameter.PostDataParameters.Add("has_attachment", "false");
            objFdRequestParameter.PostDataParameters.Add("message_id", textmessageId);
            objFdRequestParameter.PostDataParameters.Add("offline_threading_id", textmessageId);
            objFdRequestParameter.PostDataParameters.Add("other_user_fbid", friendId);

            if (objScraperDetails != null)
            {
                objFdRequestParameter.PostDataParameters.Add("shareable_attachment[share_type]", shareType);
                objFdRequestParameter.PostDataParameters.Add("shareable_attachment[share_params][urlInfo][canonical]", Uri.UnescapeDataString(objScraperDetails.CanonocalLink));
                objFdRequestParameter.PostDataParameters.Add("shareable_attachment[share_params][urlInfo][final]", Uri.UnescapeDataString(objScraperDetails.FinalLink));
                objFdRequestParameter.PostDataParameters.Add("shareable_attachment[share_params][urlInfo][user]", Uri.UnescapeDataString(objScraperDetails.UserLink));

                if (objScraperDetails.LogDict.Count > 0)
                {
                    objScraperDetails.LogDict.ForEach(x =>
                    {
                        // ReSharper disable once AccessToModifiedClosure
                        objFdRequestParameter.PostDataParameters.Add($"shareable_attachment[share_params][urlInfo][log][{x.Key}]", x.Value);

                    });
                }

                objFdRequestParameter.PostDataParameters.Add("shareable_attachment[share_params][favicon]", Uri.UnescapeDataString(objScraperDetails.FaviconLink));
                objFdRequestParameter.PostDataParameters.Add("shareable_attachment[share_params][title]", Uri.EscapeDataString(objScraperDetails.Title));
                objFdRequestParameter.PostDataParameters.Add("shareable_attachment[share_params][summary]", Uri.EscapeDataString(objScraperDetails.Summary));

                if (objScraperDetails.ImageArray.Length > 0)
                {
                    objScraperDetails.ImageArray.ForEach(x =>
                    {
                        // ReSharper disable once AccessToModifiedClosure
                        objFdRequestParameter.PostDataParameters.Add($"shareable_attachment[share_params][ranked_images][images][{objScraperDetails.ImageArray.IndexOf(x).ToString()}]", Uri.UnescapeDataString(x));

                    });
                }
                objFdRequestParameter.PostDataParameters.Add("shareable_attachment[share_params][ranked_images][ranking_model_version]", objScraperDetails.RankingModelVersion);
                objFdRequestParameter.PostDataParameters.Add("shareable_attachment[share_params][ranked_images][specified_og]", objScraperDetails.SpecifiedOg);

                objFdRequestParameter.PostDataParameters.Add("shareable_attachment[share_params][medium]", objScraperDetails.Medium);
                objFdRequestParameter.PostDataParameters.Add("shareable_attachment[share_params][url]", Uri.UnescapeDataString(objScraperDetails.Url));
                objFdRequestParameter.PostDataParameters.Add("shareable_attachment[share_params][global_share_id]", objScraperDetails.GlobalShareId.Replace("\"", string.Empty));
                objFdRequestParameter.PostDataParameters.Add("shareable_attachment[share_params][amp_url]", Uri.UnescapeDataString(objScraperDetails.AmpUrl));
                objFdRequestParameter.PostDataParameters.Add("shareable_attachment[share_params][url_scrape_id]", objScraperDetails.UrlScrapeId);
                objFdRequestParameter.PostDataParameters.Add("shareable_attachment[share_params][hmac]", objScraperDetails.Hmac);
                objFdRequestParameter.PostDataParameters.Add("shareable_attachment[share_params][locale]", objScraperDetails.Locale);
                objFdRequestParameter.PostDataParameters.Add("shareable_attachment[share_params][external_img]", "{" + objScraperDetails.ExternalImage.Replace("/", "\\/") + "}");
            }
            else if (string.IsNullOrEmpty(sharerId) || string.IsNullOrEmpty(shareType))
                objFdRequestParameter.PostDataParameters.Add("signature_id", "6219f1c3");
            else
            {
                objFdRequestParameter.PostDataParameters.Add("shareable_attachment[share_type]", shareType);
                objFdRequestParameter.PostDataParameters.Add("shareable_attachment[share_params][0]",
                    FdFunctions.GetIntegerOnlyString(sharerId));
            }

            objFdRequestParameter.PostDataParameters.Add("source", "source:chat:web");
            objFdRequestParameter.PostDataParameters.Add("specific_to_list[0]", $"fbid:{friendId}");
            objFdRequestParameter.PostDataParameters.Add("specific_to_list[1]", $"fbid:{account.AccountBaseModel.UserId}");

            if (!isMessageToPage)
                objFdRequestParameter.PostDataParameters.Add("tags[0]", "web:trigger:fb_header_dock:jewel_thread");
            else
            {
                objFdRequestParameter.PostDataParameters.Add("tags[0]", "entrypoint:fb_page:secondary_message_button");
                objFdRequestParameter.PostDataParameters.Add("tags[1]", "web:trigger:fb_page:secondary_message_button");
            }

            objFdRequestParameter.PostDataParameters.Add("timestamp", DateTime.Now.ConvertToEpoch().ToString());
            objFdRequestParameter.PostDataParameters.Add("ui_push_phase", "C3");
            objFdRequestParameter.PostDataParameters.Add("__req", "11");

            objFdRequestParameter = CommonPostDataParameters(account, objFdRequestParameter);

            var postData = objFdRequestParameter.GetPostDataFromParameters();

            var request = _httpHelper.GetRequestParameter();

            request.ContentType = FdConstants.ContentType;

            _httpHelper.SetRequestParameter(request);

            var commentOnPostResponse = _httpHelper.PostRequest(url, postData);

            return new FdSendTextMessageResponseHandler(commentOnPostResponse);
        }

        public ScrapedLinkDetails GetScrapedLinkDetails(string response)
        {

            var decodedResponse = FdFunctions.GetPrtialDecodedResponse(response);

            var log = Utilities.GetBetween(decodedResponse, "log\":{", "}");

            var logArray = log.Split(',');

            Dictionary<string, string> logDic = new Dictionary<string, string>();

            string[] array = new string[0];

            var count = 0;

            var rankedImageDetails = Utilities.GetBetween(decodedResponse, "ranked_images\":{\"images\":[", "]");

            var rankedImageArray = rankedImageDetails.Split(',');

            if (rankedImageDetails.Length > 0)
            {
                array = new string[rankedImageArray.Length];

                rankedImageArray.ForEach(x =>
                {
                    var imageLink = Utilities.GetBetween(x, "\"", "\"");

                    array[count] = imageLink;

                    count++;
                });
            }

            logArray.ForEach(x =>
            {
                var logArrayTime = Utilities.GetBetween(x.Split(':')[0], "\"", "\"");

                var url = x;

                FdFunctions.GetValidUrl(ref url);

                var logArrayLink = url;

                logDic.Add(logArrayTime, logArrayLink);
            });

            ScrapedLinkDetails objDetails = new ScrapedLinkDetails()
            {
                CanonocalLink = Utilities.GetBetween(decodedResponse, "{\"canonical\":\"", "\""),
                FinalLink = Utilities.GetBetween(decodedResponse, "final\":\"", "\""),
                UserLink = Utilities.GetBetween(decodedResponse, "\"user\":\"", "\""),
                LogDict = logDic,
                ImageArray = array,
                FaviconLink = Utilities.GetBetween(decodedResponse, "\"favicon\":\"", "\""),
                Title = Utilities.GetBetween(decodedResponse, "\"title\":\"", "\""),
                Summary = Utilities.GetBetween(decodedResponse, "\"summary\":\"", "\""),
                RankingModelVersion = Utilities.GetBetween(decodedResponse, "\"ranking_model_version\":", ","),
                SpecifiedOg = Utilities.GetBetween(decodedResponse, "\"specified_og\":", "}"),
                Medium = Utilities.GetBetween(decodedResponse, "\"medium\":", ","),
                Url = Utilities.GetBetween(decodedResponse, "\"url\":\"", "\""),
                GlobalShareId = Utilities.GetBetween(decodedResponse, "\"global_share_id\":", ","),
                AmpUrl = Utilities.GetBetween(decodedResponse, "\"amp_url\":\"", "\""),
                UrlScrapeId = Utilities.GetBetween(decodedResponse, "\"url_scrape_id\":\"", "\""),
                Hmac = Utilities.GetBetween(decodedResponse, "\"hmac\":\"", "\""),
                Locale = Utilities.GetBetween(decodedResponse, "\"locale\":\"", "\""),
                ExternalImage = Utilities.GetBetween(decodedResponse, "\"external_img\":\"{", "}"),
            };

            return objDetails;
        }


        // ReSharper disable once RedundantAssignment
        public string UpdateLinkAndGetShareId(DominatorAccountModel account, string link, ref string shareType,
            ref ScrapedLinkDetails objDetails)
        {
            FdRequestParameter objFdRequestParameter = new FdRequestParameter();

            objFdRequestParameter.UrlParameters.Add("dpr", "1");

            var url = objFdRequestParameter.GenerateUrl(FdConstants.SendLinkMessageUrl);

            objFdRequestParameter.PostDataParameters.Clear();

            objFdRequestParameter.PostDataParameters.Add("image_height", "960");
            objFdRequestParameter.PostDataParameters.Add("image_width", "960");
            objFdRequestParameter.PostDataParameters.Add("uri", link);
            objFdRequestParameter.PostDataParameters.Add("__req", "11");

            objFdRequestParameter = CommonPostDataParameters(account, objFdRequestParameter);

            var postData = objFdRequestParameter.GetPostDataFromParameters();

            var request = _httpHelper.GetRequestParameter();

            request.ContentType = FdConstants.ContentType;

            _httpHelper.SetRequestParameter(request);

            var commentOnPostResponse = _httpHelper.PostRequest(url, postData);

            var sharerId = Utilities.GetBetween(commentOnPostResponse.Response, "share_params\":[", "]");

            shareType = Utilities.GetBetween(commentOnPostResponse.Response, "share_type\":", ",");

            if (string.IsNullOrEmpty(sharerId) && shareType == "100")
                objDetails = GetScrapedLinkDetails(commentOnPostResponse.Response);

            return sharerId;
        }


        public bool SendImageWithText(DominatorAccountModel account, string friendId, List<string> imagePathList, string timeStamp = null)
        {

            FdRequestParameter objFdRequestParameter = new FdRequestParameter();

            objFdRequestParameter.UrlParameters.Add("dpr", "1");

            var url = objFdRequestParameter.GenerateUrl(FdConstants.SendDirectMessageUrl);

            var textmessageId = "";
            if (timeStamp == null)
            {
                textmessageId = (617161).ToString() + DateTime.Now.ConvertToEpoch();
                timeStamp = DateTime.Now.ConvertToEpoch().ToString();
            }
            else
                textmessageId = (617161).ToString() + timeStamp;

            List<string> lstImage = imagePathList;

            var mediaId = UploadImageAndGetMediaIdForMessage(account, lstImage);

            objFdRequestParameter.PostDataParameters.Clear();

            objFdRequestParameter.PostDataParameters.Add("client", "mercury");
            objFdRequestParameter.PostDataParameters.Add("action_type", "ma-type:user-generated-message");
            objFdRequestParameter.PostDataParameters.Add("body", "");
            objFdRequestParameter.PostDataParameters.Add("ephemeral_ttl_mode", "0");
            objFdRequestParameter.PostDataParameters.Add("has_attachment", "true");
            objFdRequestParameter.PostDataParameters.Add("image_ids[0]", mediaId);
            objFdRequestParameter.PostDataParameters.Add("message_id", textmessageId);
            objFdRequestParameter.PostDataParameters.Add("offline_threading_id", textmessageId);
            objFdRequestParameter.PostDataParameters.Add("other_user_fbid", friendId);
            objFdRequestParameter.PostDataParameters.Add("signature_id", "6219f1c3");
            objFdRequestParameter.PostDataParameters.Add("source", "source:chat:web");
            objFdRequestParameter.PostDataParameters.Add("specific_to_list[0]", $"fbid:{friendId}");
            objFdRequestParameter.PostDataParameters.Add("specific_to_list[1]", $"fbid:{account.AccountBaseModel.UserId}");
            objFdRequestParameter.PostDataParameters.Add("tags[0]", "web:trigger:fb_header_dock:jewel_thread");
            objFdRequestParameter.PostDataParameters.Add("timestamp", timeStamp);
            objFdRequestParameter.PostDataParameters.Add("ui_push_phase", "C3");
            objFdRequestParameter.PostDataParameters.Add("__req", "11");

            objFdRequestParameter = CommonPostDataParameters(account, objFdRequestParameter);

            var postData = objFdRequestParameter.GetPostDataFromParameters();

            var request = _httpHelper.GetRequestParameter();

            request.ContentType = FdConstants.ContentType;

            _httpHelper.SetRequestParameter(request);

            var commentOnPostResponse = _httpHelper.PostRequest(url, postData);

            return new FdSendTextMessageResponseHandler(commentOnPostResponse).IsMessageSent;
        }

        public async Task<IResponseHandler> SendImageWithTextAsync(DominatorAccountModel account, SenderDetails senderDetails, List<string> imagePathList, string timeStamp = null)
        {
            var imageCount = 0;

            FdRequestParameter objFdRequestParameter = new FdRequestParameter();

            objFdRequestParameter.UrlParameters.Add("dpr", "1");

            var url = objFdRequestParameter.GenerateUrl(FdConstants.SendDirectMessageUrl);

            var textmessageId = "";
            if (timeStamp == null)
            {
                textmessageId = (617161).ToString() + DateTime.Now.ConvertToEpoch();
                timeStamp = DateTime.Now.ConvertToEpoch().ToString();
            }
            else
                textmessageId = (617161).ToString() + timeStamp;

            List<string> lstImage = imagePathList;

            var mediaIdList = await UploadImageAndGetMediaIdForMessageAsyc(account, lstImage);

            objFdRequestParameter.PostDataParameters.Clear();

            objFdRequestParameter.PostDataParameters.Add("client", "mercury");
            objFdRequestParameter.PostDataParameters.Add("action_type", "ma-type:user-generated-message");
            objFdRequestParameter.PostDataParameters.Add("body", "");
            objFdRequestParameter.PostDataParameters.Add("ephemeral_ttl_mode", "0");
            objFdRequestParameter.PostDataParameters.Add("has_attachment", "true");
            foreach (var media in mediaIdList)
            {
                objFdRequestParameter.PostDataParameters.Add($"image_ids[{imageCount}]", media);
                imageCount++;
            }
            objFdRequestParameter.PostDataParameters.Add("message_id", textmessageId);
            objFdRequestParameter.PostDataParameters.Add("offline_threading_id", textmessageId);
            objFdRequestParameter.PostDataParameters.Add("other_user_fbid", senderDetails.SenderId);
            objFdRequestParameter.PostDataParameters.Add("signature_id", "6219f1c3");
            objFdRequestParameter.PostDataParameters.Add("source", "source:chat:web");
            objFdRequestParameter.PostDataParameters.Add("specific_to_list[0]", $"fbid:{senderDetails.SenderId}");
            objFdRequestParameter.PostDataParameters.Add("specific_to_list[1]", $"fbid:{account.AccountBaseModel.UserId}");
            objFdRequestParameter.PostDataParameters.Add("tags[0]", "web:trigger:fb_header_dock:jewel_thread");
            objFdRequestParameter.PostDataParameters.Add("timestamp", timeStamp);
            objFdRequestParameter.PostDataParameters.Add("ui_push_phase", "C3");
            objFdRequestParameter.PostDataParameters.Add("__req", "11");

            objFdRequestParameter = CommonPostDataParameters(account, objFdRequestParameter);

            var postData = objFdRequestParameter.GetPostDataFromParameters();

            var request = _httpHelper.GetRequestParameter();

            //Assigning Browser Cookie to http
            if (request.Cookies.Count == 0)
                request.Cookies = account.BrowserCookies;

            request.ContentType = FdConstants.ContentType;

            _httpHelper.SetRequestParameter(request);

            var commentOnPostResponse = _httpHelper.PostRequest(url, postData);

            return new FdSendTextMessageResponseHandler(commentOnPostResponse, senderDetails);
        }

        #endregion


        #region Group Unjoiner

        public bool UnjoinGroup(DominatorAccountModel account, string groupId)
        {
            FdRequestParameter objFdRequestParameter = new FdRequestParameter();

            objFdRequestParameter.UrlParameters.Add("group_id", groupId);
            objFdRequestParameter.UrlParameters.Add("ref", "group_unjoined");

            var url = objFdRequestParameter.GenerateUrl(FdConstants.LeaveGroupUrl);

            objFdRequestParameter.PostDataParameters.Add("confirmed", "1");
            objFdRequestParameter.PostDataParameters.Add("__req", "11");

            objFdRequestParameter = CommonPostDataParameters(account, objFdRequestParameter);

            var postData = objFdRequestParameter.GetPostDataFromParameters();

            var request = _httpHelper.GetRequestParameter();

            request.ContentType = FdConstants.ContentType;

            _httpHelper.SetRequestParameter(request);

            var commentOnPostResponse = _httpHelper.PostRequest(url, postData);

            return new UnjoinGroupResponseHandler(commentOnPostResponse).IsUnjoinedGroup;
        }

        #endregion


        #region Inviter

        /// <summary>
        /// Invite Groups
        /// </summary>
        /// <param name="account">DominatorAccount</param>
        /// <param name="groupId">GroupId</param>
        /// <param name="objFacebookUser">FacebookUser</param>
        /// <param name="message">Note</param>
        /// <returns></returns>
        public GroupInviterResponseHandler SendGroupInvittationTofriends
           (DominatorAccountModel account, string groupId, FacebookUser objFacebookUser, string message)
        {
            FdRequestParameter objFdRequestParameter = new FdRequestParameter();

            objFdRequestParameter.UrlParameters.Add("group_id", groupId);
            objFdRequestParameter.UrlParameters.Add("refresh", "1");
            objFdRequestParameter.UrlParameters.Add("source", "dialog_typeahead");

            var url = objFdRequestParameter.GenerateUrl(FdConstants.GroupInviteUrl);

            objFdRequestParameter.PostDataParameters.Add("dpr", "1");
            objFdRequestParameter.PostDataParameters.Add("members[0]", objFacebookUser.UserId);
            objFdRequestParameter.PostDataParameters.Add("text_members[0]", objFacebookUser.Familyname);

            if (!string.IsNullOrEmpty(message))
                objFdRequestParameter.PostDataParameters.Add("message", Uri.EscapeDataString(message));

            objFdRequestParameter.PostDataParameters.Add("__req", "13");

            objFdRequestParameter = CommonPostDataParameters(account, objFdRequestParameter);

            var postData = objFdRequestParameter.GetPostDataFromParameters();

            var request = _httpHelper.GetRequestParameter();

            request.ContentType = FdConstants.ContentType;

            _httpHelper.SetRequestParameter(request);

            var commentOnPostResponse = _httpHelper.PostRequest(url, postData);

            return new GroupInviterResponseHandler(commentOnPostResponse);

        }

        /// <summary>
        /// Send Invitations Fro Pages
        /// </summary>
        /// <param name="account"></param>
        /// <param name="pageId"></param>
        /// <param name="inviteNote">Invite with NOTE</param>
        /// <param name="objFacebookUser"></param>
        /// <param name="isSendInMessage">Send Invitation in Messanger</param>
        /// <returns></returns>
        public bool SendPageInvittationTofriends
            (DominatorAccountModel account, string pageId, string inviteNote, FacebookUser objFacebookUser, bool isSendInMessage)
        {
            FdRequestParameter objFdRequestParameter = new FdRequestParameter();

            objFdRequestParameter.UrlParameters.Add("dpr", "1.5");


            var url = objFdRequestParameter.GenerateUrl(FdConstants.PageInviteUrl);

            objFdRequestParameter.PostDataParameters.Add("page_id", pageId);

            if (!string.IsNullOrEmpty(inviteNote))
                objFdRequestParameter.PostDataParameters.Add("invite_note", Uri.EscapeDataString(inviteNote));

            objFdRequestParameter.PostDataParameters.Add("send_in_messenger", isSendInMessage.ToString());
            objFdRequestParameter.PostDataParameters.Add("invitees[0]", objFacebookUser.UserId);
            objFdRequestParameter.PostDataParameters.Add("ref", "modal_page_invite_dialog_v2");
            objFdRequestParameter.PostDataParameters.Add("__req", "11");

            objFdRequestParameter = CommonPostDataParameters(account, objFdRequestParameter);

            var postData = objFdRequestParameter.GetPostDataFromParameters();

            var request = _httpHelper.GetRequestParameter();

            request.ContentType = FdConstants.ContentType;

            _httpHelper.SetRequestParameter(request);

            var commentOnPostResponse = _httpHelper.PostRequest(url, postData);

            return new PageInviterResponseHandler(commentOnPostResponse).Status;
        }

        public bool SendPageInvittationTofriendsWithoutNote
            (DominatorAccountModel account, string pageId, FacebookUser objFacebookUser)
        {
            FdRequestParameter objFdRequestParameter = new FdRequestParameter();

            objFdRequestParameter.UrlParameters.Add("page_id", pageId);
            objFdRequestParameter.UrlParameters.Add("invitee", objFacebookUser.UserId);
            objFdRequestParameter.UrlParameters.Add("ref", "chaining");
            objFdRequestParameter.UrlParameters.Add("invite_callsite", "PagesFriendInviterChaining");

            var url = objFdRequestParameter.GenerateUrl(FdConstants.SinglePageInviteUrl);

            objFdRequestParameter = CommonPostDataParameters(account, objFdRequestParameter);

            var postData = objFdRequestParameter.GetPostDataFromParameters();

            var request = _httpHelper.GetRequestParameter();

            request.ContentType = FdConstants.ContentType;

            _httpHelper.SetRequestParameter(request);

            var commentOnPostResponse = _httpHelper.PostRequest(url, postData);

            return new PageInviterResponseHandler(commentOnPostResponse).Status;
        }


        public bool SendPageInvittationToPageLikers
           (DominatorAccountModel account, string pageId, FacebookUser objFacebookUser, string postUrl)
        {
            var postId = postUrl.Replace(FdConstants.FbHomeUrl, string.Empty);

            FdRequestParameter objFdRequestParameter = new FdRequestParameter();

            objFdRequestParameter.UrlParameters.Add("dpr", "1");


            var url = objFdRequestParameter.GenerateUrl(FdConstants.InvitePageLikerUrl);

            objFdRequestParameter.PostDataParameters.Add("page_id", pageId);

            objFdRequestParameter.PostDataParameters.Add("invitee", objFacebookUser.UserId);
            objFdRequestParameter.PostDataParameters.Add("ref", "likes_dialog");
            objFdRequestParameter.PostDataParameters.Add("content_id", postId);
            objFdRequestParameter.PostDataParameters.Add("__req", "11");

            objFdRequestParameter = CommonPostDataParameters(account, objFdRequestParameter);

            var postData = objFdRequestParameter.GetPostDataFromParameters();

            var request = _httpHelper.GetRequestParameter();

            request.ContentType = FdConstants.ContentType;

            _httpHelper.SetRequestParameter(request);

            var commentOnPostResponse = _httpHelper.PostRequest(url, postData);

            return new PageInviterResponseHandler(commentOnPostResponse).Status;

        }


        public void GetLangugae(DominatorAccountModel account)
        {
            try
            {
                string url = $"{FdConstants.FbHomeUrl}";

                string homepageResponse = _httpHelper.GetRequest(url).Response;

                string languageCode = FdRegexUtility.FirstMatchExtractor(homepageResponse, FdConstants.LanguageCodeRegex);

                if (!FdConstants.AccountLanguage.ContainsKey(account.AccountBaseModel.UserId))
                    FdConstants.AccountLanguage.Add(account.AccountBaseModel.UserId, languageCode);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }


        public void ChangeLanguage(DominatorAccountModel account, string languageCode)
        {
            try
            {
                string postUrl = FdConstants.ChangeLanguageUrl;

                FdRequestParameter objParameter = new FdRequestParameter();

                objParameter.PostDataParameters.Add("new_language", languageCode);
                objParameter.PostDataParameters.Add("new_fallback_language", FdConstants.AccountLanguage[account.AccountBaseModel.UserId]);
                objParameter.PostDataParameters.Add("__req", "x");

                objParameter = CommonPostDataParameters(account, objParameter);

                var postData = objParameter.GetPostDataFromParameters();

                var request = _httpHelper.GetRequestParameter();

                request.ContentType = FdConstants.ContentType;

                _httpHelper.SetRequestParameter(request);

                _httpHelper.PostRequest(postUrl, postData);

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

        }

        public void ChangeToClassicUIMode(DominatorAccountModel account)
        {
            try
            {
                string postUrl = FdConstants.ApiGraphql;

                FdRequestParameter objParameter = new FdRequestParameter();

                FdPublisherJsonElement jsonelement = new FdPublisherJsonElement()
                {
                    Input = new FdPublisherJsonElement()
                    {
                        ChangeType = "OPT_OUT",
                        Source = "SETTINGS_MENU",
                        ActorId = account.AccountBaseModel.UserId,
                        ClientMutationId = "2"
                    }
                };

                var variableValue = Uri.EscapeDataString(objParameter.GetJsonString(jsonelement));
                string fbDtsg = Uri.UnescapeDataString(account.SessionId);
                objParameter.PostDataParameters.Clear();


                objParameter.PostDataParameters.Add("av", account.AccountBaseModel.UserId);
                objParameter.PostDataParameters.Add("__csr", "");
                objParameter.PostDataParameters.Add("__req", "w");
                objParameter.PostDataParameters.Add("__beoa", "1");
                objParameter.PostDataParameters.Add("__pc", "EXP2:comet_pkg");
                objParameter.PostDataParameters.Add("dpr", "1");
                objParameter.PostDataParameters.Add("__ccg", "GOOD");
                objParameter.PostDataParameters.Add("__rev", "1002564919");
                objParameter.PostDataParameters.Add("__s", "wr9gaw:caooob:u5rc4g");
                objParameter.PostDataParameters.Add("__hsi", "6864511561232486401-0");
                objParameter.PostDataParameters.Add("__comet_req", "1");
                objParameter.PostDataParameters.Add("fb_api_caller_class", "RelayModern");
                objParameter.PostDataParameters.Add("fb_api_req_friendly_name", "CometTrialParticipationChangeMutation");
                objParameter.PostDataParameters.Add("variables", variableValue);
                objParameter.PostDataParameters.Add("server_timestamps", "true");
                objParameter.PostDataParameters.Add("doc_id", "2317726921658975");
                //CommonPostDataParameters(account, objParameter);

                objParameter = CommonPostDataParameters(account, objParameter);

                var postData = objParameter.GetPostDataFromParameters();

                var request = _httpHelper.GetRequestParameter();

                request.ContentType = FdConstants.ContentType;

                _httpHelper.SetRequestParameter(request);

                _httpHelper.PostRequest(postUrl, postData);

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

        }


        /// <summary>
        /// Send invitations for Events
        /// </summary>
        /// <param name="account"></param>
        /// <param name="eventId"></param>
        /// <param name="objFacebookUser"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool SendEventInvittationTofriends
            (DominatorAccountModel account, string eventId, FacebookUser objFacebookUser, string message)
        {
            FdRequestParameter objFdRequestParameter = new FdRequestParameter();

            objFdRequestParameter.UrlParameters.Add("plan_id", eventId);
            objFdRequestParameter.UrlParameters.Add("acontext[ref]", "51");
            objFdRequestParameter.UrlParameters.Add("acontext[source]", "1");
            objFdRequestParameter.UrlParameters.Add("acontext[no_referrer]", "true");
            objFdRequestParameter.UrlParameters.Add("acontext[action_history]", "[{\"surface\":\"permalink\",\"mechanism\":\"surface\",\"extra_data\":[]}]");
            objFdRequestParameter.UrlParameters.Add("acontext[has_source]", "true");
            objFdRequestParameter.UrlParameters.Add("dpr", "1.5");


            var url = objFdRequestParameter.GenerateUrl(FdConstants.EventInviteUrl);


            objFdRequestParameter.PostDataParameters.Add("at_limit", "false");
            objFdRequestParameter.PostDataParameters.Add("session_id", "2384444232");
            objFdRequestParameter.PostDataParameters.Add("profileChooserItems", FdConstants.FbEventInviterId(objFacebookUser.UserId));
            objFdRequestParameter.PostDataParameters.Add("pagelets_to_update", FdConstants.FbEventInviterParameters);
            objFdRequestParameter.PostDataParameters.Add("invite_message", message);
            objFdRequestParameter.PostDataParameters.Add("message_option", "invite_only");
            objFdRequestParameter.PostDataParameters.Add("__req", "11");

            objFdRequestParameter = CommonPostDataParameters(account, objFdRequestParameter);

            var postData = objFdRequestParameter.GetPostDataFromParameters();

            var request = _httpHelper.GetRequestParameter();

            request.ContentType = FdConstants.ContentType;

            _httpHelper.SetRequestParameter(request);

            var commentOnPostResponse = _httpHelper.PostRequest(url, postData);

            return new EventAndMessangerResponse(commentOnPostResponse).IsEventInviteSent;
        }

        public bool InviteAsPersonalMessage
           (DominatorAccountModel account, string entityId, FacebookUser objFacebookUser, string message)
        {
            FdRequestParameter objFdRequestParameter = new FdRequestParameter();

            objFdRequestParameter.UrlParameters.Add("app_id", "256281040558");
            objFdRequestParameter.UrlParameters.Add("audience_type", "message");
            objFdRequestParameter.UrlParameters.Add("audience_targets[0]", objFacebookUser.UserId);
            objFdRequestParameter.UrlParameters.Add("composer_session_id", FdConstants.ComposerSessionId);
            objFdRequestParameter.UrlParameters.Add("ephemeral_ttl_mod", "0");
            objFdRequestParameter.UrlParameters.Add("message", message);
            objFdRequestParameter.UrlParameters.Add("owner_id", "");
            objFdRequestParameter.UrlParameters.Add("post_id", entityId);
            objFdRequestParameter.UrlParameters.Add("share_to_group_as_page", "false");
            objFdRequestParameter.UrlParameters.Add("share_type", "7");
            objFdRequestParameter.UrlParameters.Add("shared_ad_id", "");
            objFdRequestParameter.UrlParameters.Add("source", "7");
            objFdRequestParameter.UrlParameters.Add("is_throwback_post", "false");
            objFdRequestParameter.UrlParameters.Add("url", "");
            objFdRequestParameter.UrlParameters.Add("shared_from_post_id", FdConstants.LoggingSessionId);
            objFdRequestParameter.UrlParameters.Add("logging_session_id", "true");
            objFdRequestParameter.UrlParameters.Add("perform_messenger_logging", entityId);
            objFdRequestParameter.UrlParameters.Add("av", account.AccountBaseModel.UserId);
            objFdRequestParameter.UrlParameters.Add("dpr", "1.5");

            var url = objFdRequestParameter.GenerateUrl(FdConstants.ShareAsMessageUrl);

            objFdRequestParameter.PostDataParameters.Add("__req", "11");

            objFdRequestParameter = CommonPostDataParameters(account, objFdRequestParameter);


            var postData = objFdRequestParameter.GetPostDataFromParameters();

            var request = _httpHelper.GetRequestParameter();

            request.ContentType = FdConstants.ContentType;

            _httpHelper.SetRequestParameter(request);

            var commentOnPostResponse = _httpHelper.PostRequest(url, postData);

            return new EventAndMessangerResponse(commentOnPostResponse).IsEventInviteSent;
        }


        #endregion



        public IResponseHandler GetMessageRequestDetails
            (DominatorAccountModel account, IResponseHandler responseHandler, MessageType messageType)
        {
            FdRequestParameter objFdRequestParameter = new FdRequestParameter();

            var url = objFdRequestParameter.GenerateUrl(FdConstants.GetMessagesUrl);

            objFdRequestParameter.PostDataParameters.Clear();

            objFdRequestParameter.PostDataParameters.Add("batch_name", "MessengerGraphQLThreadlistFetcher");
            objFdRequestParameter.PostDataParameters.Add("__req", "11");

            objFdRequestParameter = CommonPostDataParameters(account, objFdRequestParameter);

            objFdRequestParameter.PostDataParameters.Add("queries", FdConstants.GetMessageQueries
                ("100", responseHandler == null ? "null" : responseHandler.ObjFdScraperResponseParameters.PaginationTimestamp, messageType));

            var postData = objFdRequestParameter.GetPostDataFromParameters();

            var request = _httpHelper.GetRequestParameter();

            request.ContentType = FdConstants.ContentType;

            _httpHelper.SetRequestParameter(request);

            var incommingMessageResponse = _httpHelper.PostRequest(url, postData);

            return responseHandler != null ? new IncommingMessageResponseHandler(incommingMessageResponse,
                          account.AccountBaseModel.UserId, responseHandler.ObjFdScraperResponseParameters.MessageDetailsList)
                           : new IncommingMessageResponseHandler(incommingMessageResponse, account.AccountBaseModel.UserId, null);

            //if (responseHandler != null)
            //{
            //    var incommingMessageResponseHandler = new IncommingMessageResponseHandler(incommingMessageResponse,
            //        account.AccountBaseModel.UserId, responseHandler.ObjFdScraperResponseParameters.MessageDetailsList);

            //    return incommingMessageResponseHandler;
            //}
            //else
            //{
            //    var incommingMessageResponseHandler = new IncommingMessageResponseHandler(incommingMessageResponse,
            //        account.AccountBaseModel.UserId, null);

            //    return incommingMessageResponseHandler;
            //}
        }


        #region Publishing Functions

        public PublisherResponseHandler PostToPages(DominatorAccountModel account,
             string pageUrl, PublisherPostlistModel postDetails,
             CancellationTokenSource campaignCancellationToken, GeneralModel generalsettingsModel,
             FacebookModel advanceSettingsModel)
        {
            //bool isTagSuccesfull = false;
            try
            {
                #region Properties


                FdFunctions objFdFunctions = new FdFunctions(account);
                var randomFriends = new List<string>();
                string composerId;
                string pageId;
                var mediaDictionary = new Dictionary<string, string>();
                var postId = string.Empty;
                var url = pageUrl;
                var postAsPageId = string.Empty;
                var waterfallId = FdFunctions.GetRandomHexNumber(32).ToLower();

                #endregion

                #region Making page Url and get page id 

                campaignCancellationToken.Token.ThrowIfCancellationRequested();

                if (!url.Contains(FdConstants.FbHomeUrl) && FdFunctions.IsIntegerOnly(url))
                    url = FdConstants.FbHomeUrl + pageUrl;

                var fanPageResponse = _httpHelper.GetRequest(url);

                try
                {
                    pageId = FdRegexUtility.FirstMatchExtractor(fanPageResponse.Response, FdConstants.PageIdRegex);
                    composerId = FdRegexUtility.FirstMatchExtractor(fanPageResponse.Response, FdConstants.ComposerIdRegex);

                }
                catch (Exception)
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, account.AccountBaseModel.AccountNetwork, account.AccountBaseModel.UserName, "", "LangKeyPageNotAllowVisitorsPost".FromResourceDictionary());
                    //Todo :  Analysis why failed with response, if FacebookErrors enum doesn't contains then add to enum list as well.
                    FacebookErrors errors = FacebookErrors.UnknownErrorOccurred;
                    return new PublisherResponseHandler(fanPageResponse, errors);
                }

                #endregion

                if (string.IsNullOrEmpty(composerId))
                {
                    FacebookErrors errors = FacebookErrors.PageDosentAllowVisitorPot;
                    GlobusLogHelper.log.Info(Log.CustomMessage, account.AccountBaseModel.AccountNetwork, account.AccountBaseModel.UserName, "", "LangKeyPageNotAllowVisitorsPost".FromResourceDictionary());
                    return new PublisherResponseHandler(new ResponseParameter(), errors);
                }

                #region Uploading medias and posting

                campaignCancellationToken.Token.ThrowIfCancellationRequested();

                FdRequestParameter objParameter = new FdRequestParameter();

                objParameter.UrlParameters.Add("doc_id", "1740513229408093");

                if (advanceSettingsModel.IsPostAsPage)
                {
                    var randomPages = objFdFunctions.GetRandomPageActor(account, 1, advanceSettingsModel);

                    randomPages.ForEach(x =>
                    {
                        var pageIdNew = GetPageIdFromUrl(account, x);
                        advanceSettingsModel.ListCustomPageUrl.Remove(x);
                        advanceSettingsModel.SelectPageDetailsModel.AccountPagesBoardsPair.Add(new KeyValuePair<string, string>(account.AccountId, $"{FdConstants.FbHomeUrl}{pageIdNew}"));
                        _delayService.ThreadSleep(1000);
                    });

                    randomPages = advanceSettingsModel.SelectPageDetailsModel.AccountPagesBoardsPair.Where(x => x.Key == account.AccountId).Select(x => x.Value).ToList();

                    Random random = new Random();

                    postAsPageId = randomPages.OrderBy(x => random.Next()).Take(1).FirstOrDefault();


                }
                else if (advanceSettingsModel.IsPostAsSamePage)
                    postAsPageId = pageId;

                randomFriends = TagFriends(advanceSettingsModel, account);

                var mentions = MentionUsers(advanceSettingsModel, account);

                PublisherParameter publisherParameter = new PublisherParameter();

                url = $"{FdConstants.FbHomeUrl}webgraphql/mutation/";

                if (!string.IsNullOrEmpty(postAsPageId) && ChangeActorForPost(account, postId, pageId, postAsPageId, composerId)
                    && postDetails.MediaList.Count > 0)
                {
                    if (postDetails.MediaList.Count > 0)
                    {
                        campaignCancellationToken.Token.ThrowIfCancellationRequested();
                        mediaDictionary = UploadImageAndGetMediaId(postDetails, account, pageId, waterfallId, "Pages", postAsPageId);
                    }


                    publisherParameter.Tags = null;
                    objParameter.UrlParameters.Clear();
                    objParameter.PostDataParameters.Clear();

                    objParameter.UrlParameters.Add("av", FdFunctions.GetIntegerOnlyString(postAsPageId));
                    objParameter.UrlParameters.Add("dpr", "1");

                    objParameter.PostDataParameters.Add("breaking_news_expiration", "0");
                    objParameter.PostDataParameters.Add("breaking_news_selected", "false");
                    objParameter.PostDataParameters.Add("composer_entry_point", "pages_feed");
                    objParameter.PostDataParameters.Add("composer_entry_time", "438");
                    objParameter.PostDataParameters.Add("composer_session_id", $"{Utilities.GetGuid()}");
                    objParameter.PostDataParameters.Add("composer_source_surface", "page");
                    objParameter.PostDataParameters.Add("direct_share_status", "0");
                    objParameter.PostDataParameters.Add("sponsor_relationship", "0");
                    objParameter.PostDataParameters.Add("is_explicit_place", "false");
                    objParameter.PostDataParameters.Add("is_markdown", "false");
                    objParameter.PostDataParameters.Add("is_post_to_group", "false");
                    objParameter.PostDataParameters.Add("is_welcome_to_group_post", "false");
                    objParameter.PostDataParameters.Add("is_q_and_a", "false");
                    objParameter.PostDataParameters.Add("is_profile_badge_post", "false");
                    objParameter.PostDataParameters.Add("multilingual_specified_lang", "en_XX");
                    objParameter.PostDataParameters.Add("num_keystrokes", "0");
                    objParameter.PostDataParameters.Add("num_pastes", "0");
                    objParameter.PostDataParameters.Add("place_attachment_setting", "1");
                    objParameter.PostDataParameters.Add("stories_selected", "false");
                    objParameter.PostDataParameters.Add("timeline_selected", "true");
                    objParameter.PostDataParameters.Add("xc_sticker_id", "0");
                    objParameter.PostDataParameters.Add("target_type", "wall");
                    objParameter.PostDataParameters.Add("xhpc_message", postDetails.PostDescription);
                    objParameter.PostDataParameters.Add("xhpc_message_text", postDetails.PostDescription);
                    objParameter.PostDataParameters.Add("is_react", "true");
                    objParameter.PostDataParameters.Add("xhpc_composerid", composerId);
                    objParameter.PostDataParameters.Add("xhpc_targetid", pageId);
                    objParameter.PostDataParameters.Add("xhpc_context", "profile");
                    objParameter.PostDataParameters.Add("xhpc_timeline", "true");
                    objParameter.PostDataParameters.Add("xhpc_finch", "true");
                    objParameter.PostDataParameters.Add("xhpc_aggregated_story_composer", "false");
                    objParameter.PostDataParameters.Add("xhpc_publish_type", "1");
                    objParameter.PostDataParameters.Add("xhpc_fundraiser_page", "false");
                    objParameter.PostDataParameters.Add("draft", "false");
                    objParameter.PostDataParameters.Add("application", "composer");

                    int mediaCount = 0;

                    mediaDictionary.ForEach(x =>
                    {
                        // ReSharper disable once AccessToModifiedClosure
                        objParameter.PostDataParameters.Add($"composer_unpublished_photo[{mediaCount}]", x.Key);
                        mediaCount++;
                    });

                    objParameter.PostDataParameters.Add("qn", $"{Utilities.GetGuid()}");
                    objParameter.PostDataParameters.Add("waterfallxapp", "web_react_composer");

                    url = $"{FdConstants.FbHomeUrl}media/upload/photos/composer/";
                }
                else
                {
                    campaignCancellationToken.Token.ThrowIfCancellationRequested();
                    mediaDictionary = UploadImageAndGetMediaId(postDetails, account, pageId, waterfallId, "Pages");

                    publisherParameter.ComposerId = composerId;
                    publisherParameter.MediaDictionary = mediaDictionary;
                    publisherParameter.PageId = pageId;
                    publisherParameter.Message = postDetails.PostDescription;
                    publisherParameter.WaterfallId = waterfallId;
                    publisherParameter.TargetType = "Pages";
                    publisherParameter.Mentions = mentions.ToArray();
                    publisherParameter.PostDetails = postDetails;

                    postAsPageId = FdFunctions.GetIntegerOnlyString(postAsPageId);

                    if (randomFriends.Count > 0)
                        publisherParameter.Tags = randomFriends.ToArray();

                    var jsonString = GetPublisherParameterImagePage(account, publisherParameter, string.IsNullOrEmpty(postAsPageId) ? string.Empty : postAsPageId);

                    objParameter.PostDataParameters.Add("variables", Uri.EscapeDataString(jsonString));
                    objParameter.PostDataParameters.Add("__req", "4w");
                }


                objParameter = CommonPostDataParameters(account, objParameter);


                url = objParameter.GenerateUrl(url);

                var postData = objParameter.GetPostDataFromParameters();

                var request = _httpHelper.GetRequestParameter();

                request.ContentType = FdConstants.ContentType;

                _httpHelper.SetRequestParameter(request);

                campaignCancellationToken.Token.ThrowIfCancellationRequested();

                var publisherResponse = _httpHelper.PostRequest(url, postData);

                _delayService.ThreadSleep(3000);

                #endregion

                #region Response Handler

                if (publisherResponse.Response.Contains("post_fbid"))
                {
                    var decodedResponse = FdFunctions.GetDecodedResponse(publisherResponse.Response);
                    var publishedId = FdRegexUtility.FirstMatchExtractor(decodedResponse, FdConstants.PublishedIdRegex);

                    if (string.IsNullOrEmpty(publishedId))
                        publishedId = FdRegexUtility.FirstMatchExtractor(decodedResponse, FdConstants.PublishedIdModRegex);

                    return new PublisherResponseHandler(publisherResponse, $"{FdConstants.FbHomeUrl}{publishedId}");
                }
                else if (publisherResponse.Response.Contains("story_id\":"))
                {
                    _delayService.ThreadSleep(2000);
                    url = FdFunctions.GetIntegerOnlyString(postAsPageId) == pageId
                        ? $"{FdConstants.FbHomeUrl}pg/{pageId}/?ref=page_internal"
                        : $"{FdConstants.FbHomeUrl}{pageId}/posts_to_page/";

                    var getPostIdResponse = _httpHelper.GetRequest(url);

                    return new PublisherResponseHandler(getPostIdResponse, pageUrl, "Page");
                }
                else
                {
                    if (advanceSettingsModel.IsAutoTagFriends && advanceSettingsModel.IsTagUniqueFriends)
                        objFdFunctions.RemoveTaggedFriendsOnError(account, randomFriends, advanceSettingsModel);

                    FacebookErrors errors = FacebookErrors.UnknownErrorOccurred;

                    //Todo :  Analysis why failed with response, if FacebookErrors enum doesn't contains then add to enum list as well.
                    if (publisherResponse.Response.Contains("\"errorSummary\":\"You've been temporarily blocked\""))
                        errors = FacebookErrors.ActivityBlocked;
                    else if (publisherResponse.Response.Contains("Errors while executing operation") &&
                            !string.IsNullOrEmpty(composerId))
                        errors = FacebookErrors.UnknownErrorOccurred;
                    //Todo :  Analysis why failed with response, if FacebookErrors enum doesn't contains then add to enum list as well.

                    return new PublisherResponseHandler(publisherResponse, errors);
                }

                #endregion

            }
            catch (AssertFailedException)
            {
                throw;
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException("Operation Cancellated!");
            }
            catch (AggregateException ae)
            {
                ae.HandleOperationCancellation();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return null;
        }


        public PublisherResponseHandler PostToOwnWall
            (DominatorAccountModel account, PublisherPostlistModel postDetails,
            CancellationTokenSource campaignCancellationToken, GeneralModel generalSettingsModel,
            FacebookModel advanceSettingsModel)
        {
            try
            {
                #region Properties

                PublisherParameter publisherParameter = new PublisherParameter
                {
                    WaterfallId = FdFunctions.GetRandomHexNumber(32).ToLower()
                };
                var mediaId = new Dictionary<string, string>();
                var url = string.Empty;
                var randomFriends = new List<string>();
                var mentions = new List<string>();

                #endregion

                var responseHandler = ExtractPublisherParameterWall(account, ref publisherParameter, campaignCancellationToken);

                if (responseHandler != null)
                    return responseHandler;

                #region Tag Friends

                try
                {
                    randomFriends = TagFriends(advanceSettingsModel, account);
                    mentions = MentionUsers(advanceSettingsModel, account);
                }

                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                #endregion

                #region Uploading Medias and publishing

                FdRequestParameter objParameter = new FdRequestParameter();

                GetStoryDetailsResponseHandler storyDetailsResponseHandler = null;

                try
                {
                    if (postDetails.FdPostSettings.IsPostAsStoryPost)
                        storyDetailsResponseHandler = GetLastStoryDetails(account);

                    if (postDetails.MediaList.Count > 0)
                    {
                        campaignCancellationToken.Token.ThrowIfCancellationRequested();

                        GlobusLogHelper.log.Info(Log.CustomMessage, account.AccountBaseModel.AccountNetwork,
                            account.AccountBaseModel.UserName, "", "LangKeyUploadingMediaFile".FromResourceDictionary());

                        mediaId = UploadImageAndGetMediaId(postDetails, account, account.AccountBaseModel.UserId, publisherParameter.WaterfallId, "OwnWall");
                    }
                    //objParameter.UrlParameters.Clear();
                    objParameter.UrlParameters.Add("doc_id", "1740513229408093");
                    objParameter.UrlParameters.Add("dpr", "1.5");

                    publisherParameter.MediaDictionary = mediaId;
                    publisherParameter.Message = postDetails.PostDescription;
                    publisherParameter.Mentions = mentions.ToArray();
                    publisherParameter.PostDetails = postDetails;

                    if (randomFriends.Count > 0)
                        publisherParameter.Tags = randomFriends.ToArray();

                    var jsonString = GetPublisherParameterWall(account, publisherParameter, postDetails);

                    //objParameter.PostDataParameters.Clear();
                    objParameter.PostDataParameters.Add("variables", Uri.EscapeDataString(jsonString));
                    //objParameter.PostDataParameters.Add("__hsi", "6738262015005640591-0");
                    //objParameter.PostDataParameters.Add("__s", ":u7kxjh:06bhed");
                    objParameter.PostDataParameters.Add("__req", "4w");
                    objParameter.PostDataParameters.Add("__rev", "4715602");


                    objParameter = CommonPostDataParameters(account, objParameter);

                    url = $" {FdConstants.FbHomeUrl}webgraphql/mutation/";
                    url = objParameter.GenerateUrl(url);


                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                var postData = objParameter.GetPostDataFromParameters();

                var request = _httpHelper.GetRequestParameter();

                request.ContentType = FdConstants.ContentType;

                _httpHelper.SetRequestParameter(request);

                campaignCancellationToken.Token.ThrowIfCancellationRequested();
                var publisherResponse = _httpHelper.PostRequest(url, postData);

                _delayService.ThreadSleep(100);


                //calling comment
                // CommentMediaData(account,postDetails,campaignCancellationToken,generalSettingsModel,advanceSettingsModel);

                #endregion

                #region Response Handler

                if (publisherResponse.Response.Contains("story_create") && postDetails.FdPostSettings.IsPostAsStoryPost)
                {
                    _delayService.ThreadSleep(2000);
                    var currentStoryDetails = GetLastStoryDetails(account);

                    if (currentStoryDetails.ListOwnStories.Count > storyDetailsResponseHandler.ListOwnStories.Count
                        && currentStoryDetails.ListOwnStories.LastOrDefault().PostedDateTime > storyDetailsResponseHandler
                        .ListOwnStories.LastOrDefault().PostedDateTime)
                    {
                        var storyId = currentStoryDetails.ListOwnStories.LastOrDefault().Id;
                        return new PublisherResponseHandler(publisherResponse, storyId);
                    }
                }

                else if (publisherResponse.Response.Contains("privacy_fbid"))
                {
                    bool flag = true;
                    while (flag)
                    {
                        _delayService.ThreadSleep(5000);
                        flag = false;
                    }
                    //privacy_fbid":"
                    var decodedResponse = FdFunctions.GetDecodedResponse(publisherResponse.Response);
                    var publishedId = FdRegexUtility.FirstMatchExtractor(decodedResponse, FdConstants.PublishedPostPrivacyIdRegex);
                    return new PublisherResponseHandler(publisherResponse, $"{FdConstants.FbHomeUrl}{publishedId}");
                }
                else if (publisherResponse.Response.Contains("post_fbid\":") || publisherResponse.Response.Contains("post_fbid\":"))
                {
                    url = $"{FdConstants.FbHomeUrl}{account.AccountBaseModel.UserId}/allactivity";

                    //FdLoginProcess.RequestParameterInitialize(account);

                    var getPostIdResponse = _httpHelper.GetRequest(url);

                    return new PublisherResponseHandler(getPostIdResponse);
                }
                else if (publisherResponse.Response.Contains("story_create") &&
                         publisherResponse.Response.Contains("story_id"))
                {
                    if (mediaId.Count != 0)
                    {
                        var videoId = mediaId.First(x => x.Key != null).Key;
                        return new PublisherResponseHandler(publisherResponse, $"{FdConstants.FbHomeUrl}{videoId}");
                    }
                    else
                    {
                        var storyid = Utilities.GetBetween(publisherResponse.Response, "\"id\":\"", "\"");
                        return GetPostResponseWithoutImage(account, storyid, postDetails, publisherParameter.WebPrivacy, publisherParameter.ComposerId, "timeline");
                    }
                }
                else
                {
                    //Todo :  Analysis why failed with response, if FacebookErrors enum doesn't contains then add to enum list as well.
                    FacebookErrors errors = FacebookErrors.UnknownErrorOccurred;
                    return new PublisherResponseHandler(publisherResponse, errors);
                }

                #endregion


            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException("Operation Cancellated!");
            }
            catch (AggregateException ae)
            {
                ae.HandleOperationCancellation();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return null;
        }


        public PublisherResponseHandler GetPostResponseWithoutImage(DominatorAccountModel account, string storyid, PublisherPostlistModel postDetails, string id, string composerId, string postOn)
        {
            FdRequestParameter objParameter = new FdRequestParameter();
            objParameter.UrlParameters.Add("av", account.AccountBaseModel.UserId);

            string url = objParameter.GenerateUrl($"{FdConstants.FbHomeUrl}async/publisher/creation-hooks/");

            objParameter.PostDataParameters.Add(
                postOn == "timeline" ? "data[audience][web_privacyx]" : "data[audience][to_id]", id);

            objParameter.PostDataParameters.Add("data[web_graphml_migration_params][is_also_posting_video_to_feed]", "false");
            objParameter.PostDataParameters.Add("data[web_graphml_migration_params][target_type]", "feed");
            objParameter.PostDataParameters.Add("data[web_graphml_migration_params][xhpc_composerid]", composerId);
            objParameter.PostDataParameters.Add("data[web_graphml_migration_params][xhpc_context]", "profile");
            objParameter.PostDataParameters.Add("data[web_graphml_migration_params][xhpc_publish_type]", "1");
            objParameter.PostDataParameters.Add("data[web_graphml_migration_params][xhpc_timeline]", "true");
            objParameter.PostDataParameters.Add("data[is_local_dev_platform_app_instance]", "false");
            objParameter.PostDataParameters.Add("data[is_page_recommendation]", "false");

            if (postOn == "group")
                objParameter.PostDataParameters.Add("data[logging_ref]", "group");

            if (postOn == "timeline")
                objParameter.PostDataParameters.Add("data[logging_ref]", "timeline");

            objParameter.PostDataParameters.Add("data[message_text]", postDetails.PostDescription);
            objParameter.PostDataParameters.Add("story_id", storyid);
            objParameter.PostDataParameters.Add("__req", "2a");
            objParameter = CommonPostDataParameters(account, objParameter);

            byte[] postData = objParameter.GeneratePostData();

            var request = _httpHelper.GetRequestParameter();

            request.ContentType = FdConstants.ContentType;

            _httpHelper.SetRequestParameter(request);

            var publisherResponse = _httpHelper.PostRequest(url, postData);

            string entityId = string.Empty;

            if (postOn == "group")
                entityId = Utilities.GetBetween(publisherResponse.Response, "post_id=", "&");

            if (postOn == "timeline")
                entityId = Utilities.GetBetween(publisherResponse.Response, "\"ent_id\":\"", "\"");

            return new PublisherResponseHandler(publisherResponse, $"{FdConstants.FbHomeUrl}{entityId}");
        }

        public PublisherResponseHandler PostToGroups(DominatorAccountModel account,
             string groupUrl, PublisherPostlistModel postDetails,
            CancellationTokenSource campaignCancellationToken, GeneralModel generalSettingsModel,
            FacebookModel advanceSettingsModel)
        {
            try
            {
                #region Properties
                FdFunctions objFdFunctions = new FdFunctions(account);

                //bool isTagSuccesfull = false;
                string composerId;
                string groupId;
                Dictionary<string, string> mediaId = new Dictionary<string, string>();
                string waterfallId = FdFunctions.GetRandomHexNumber(32).ToLower();
                var url = groupUrl;
                var groupPageResponse = _httpHelper.GetRequest(url);

                #endregion

                #region Collect group name , group Id, composer Id

                campaignCancellationToken.Token.ThrowIfCancellationRequested();

                try
                {
                    string decodedResponse = FdFunctions.GetDecodedResponse(groupPageResponse.Response);
                    groupId = FdRegexUtility.FirstMatchExtractor(decodedResponse, FdConstants.GroupIdRegex);

                    if (string.IsNullOrEmpty(groupId))
                        groupId = Utilities.GetBetween(groupPageResponse.Response, "groups%2F", "%2F");

                    groupId = FdFunctions.GetIntegerOnlyString(groupId);
                }
                catch (Exception)
                {
                    //Todo :  Analysis why failed with response, if FacebookErrors enum doesn't contains then add to enum list as well.
                    FacebookErrors errors = FacebookErrors.UnknownErrorOccurred;
                    var responseHandler = new PublisherResponseHandler(groupPageResponse, errors);
                    return responseHandler;
                }
                if (string.IsNullOrEmpty(groupId))
                {
                    try
                    {
                        groupUrl = url.Split('?')[0];

                        if (!groupUrl.Contains("/about/"))
                            groupUrl = groupUrl[groupUrl.Length - 1] == '/'
                                ? groupUrl + "about/"
                                : groupUrl + "/about/";

                        groupPageResponse = _httpHelper.GetRequest(groupUrl);

                        groupId = FdRegexUtility.FirstMatchExtractor(groupPageResponse.Response, FdConstants.GroupIdRegex);
                        //groupId = FdRegexUtility.FirstMatchExtractor(groupPageResponse.Response, FdConstants.GroupIdRegex);
                        groupId = FdFunctions.GetIntegerOnlyString(groupId);
                    }
                    catch (Exception)
                    {
                        //Todo :  Analysis why failed with response, if FacebookErrors enum doesn't contains then add to enum list as well.
                        FacebookErrors errors = FacebookErrors.UnknownErrorOccurred;
                        return new PublisherResponseHandler(groupPageResponse, errors);
                    }
                }

                /*
                                groupUrl = FdConstants.FbHomeUrl + groupId;
                */

                try
                {
                    composerId = FdRegexUtility.FirstMatchExtractor(groupPageResponse.Response, FdConstants.ComposerIdRegex);
                }
                catch (Exception)
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, account.AccountBaseModel.AccountNetwork,
                        account.AccountBaseModel.UserName, "", "LangKeyPageNotAllowVisitorsPost".FromResourceDictionary());
                    //Todo :  Analysis why failed with response, if FacebookErrors enum doesn't contains then add to enum list as well.
                    FacebookErrors errors = FacebookErrors.UnknownErrorOccurred;
                    return new PublisherResponseHandler(groupPageResponse, errors);
                }

                #endregion

                #region Tag Friends

                var randomFriends = TagFriends(advanceSettingsModel, account);

                var mentions = MentionUsers(advanceSettingsModel, account);

                #endregion

                #region Uploading medias and publishing

                campaignCancellationToken.Token.ThrowIfCancellationRequested();

                if (postDetails.MediaList.Count > 0)
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, account.AccountBaseModel.AccountNetwork,
                        account.AccountBaseModel.UserName, "", "LangKeyUploadingMediaFile".FromResourceDictionary());
                    mediaId = UploadImageAndGetMediaId(postDetails, account, groupId, waterfallId, "Groups");
                }

                FdRequestParameter objParameter = new FdRequestParameter();

                objParameter.UrlParameters.Add("doc_id", "1740513229408093");
                objParameter.UrlParameters.Add("dpr", "1.5");

                PublisherParameter publisherParameter = new PublisherParameter
                {
                    ComposerId = composerId,
                    MediaDictionary = mediaId,
                    PageId = groupId,
                    Message = postDetails.PostDescription,
                    WaterfallId = waterfallId,
                    TargetType = "Groups",
                    Mentions = mentions.ToArray(),
                    PostDetails = postDetails
                };

                if (randomFriends.Count > 0)
                    publisherParameter.Tags = randomFriends.ToArray();

                var jsonString = GetPublisherParameterImagePage(account, publisherParameter);

                objParameter.PostDataParameters.Add("variables", Uri.EscapeDataString(jsonString));
                objParameter.PostDataParameters.Add("__req", "4w");

                objParameter = CommonPostDataParameters(account, objParameter);

                url = $" {FdConstants.FbHomeUrl}webgraphql/mutation/";
                url = objParameter.GenerateUrl(url);

                var postData = objParameter.GetPostDataFromParameters();

                var request = _httpHelper.GetRequestParameter();

                request.ContentType = FdConstants.ContentType;

                _httpHelper.SetRequestParameter(request);

                campaignCancellationToken.Token.ThrowIfCancellationRequested();

                _delayService.ThreadSleep(1000);

                var publisherResponse = _httpHelper.PostRequest(url, postData);

                var decodedFinalResponse = WebUtility.HtmlDecode(publisherResponse.Response);

                #endregion

                #region Response Handler

                if (decodedFinalResponse.Contains("story_creat") && decodedFinalResponse.Contains("composeFinished"))
                {
                    var publishedPostId = FdRegexUtility.FirstMatchExtractor(decodedFinalResponse, FdConstants.PublishedPostIdRegex);
                    return new PublisherResponseHandler(publisherResponse, $"{FdConstants.FbHomeUrl}{publishedPostId}");
                }
                else if (publisherResponse.Response.Contains("story_create") &&
                        publisherResponse.Response.Contains("story_id"))
                {
                    try
                    {
                        if (mediaId.Count != 0)
                        {
                            var videoId = mediaId.First(x => x.Key != null).Key;
                            return new PublisherResponseHandler(publisherResponse, $"{FdConstants.FbHomeUrl}{videoId}");
                        }
                        else
                        {
                            var storyid = Utilities.GetBetween(publisherResponse.Response, "\"id\":\"", "\"");//UzpfSTEwMDAxMzEzOTk4NTAxNjo2MjUzMDE4MDc5MTc4MTM=
                            return GetPostResponseWithoutImage(account, storyid, postDetails, groupId, composerId, "group");
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                }
                if (publisherResponse.Response.Contains("post_fbid\":") || publisherResponse.Response.Contains("story_id\":\"")
                    || publisherResponse.Response.Contains("story_id\":"))
                {
                    url = $"{FdConstants.FbHomeUrl}{account.AccountBaseModel.UserId}/allactivity?privacy_source=activity_log&log_filter=groupposts";

                    //FdLoginProcess.RequestParameterInitialize(account);

                    var getPostIdResponse = _httpHelper.GetRequest(url);
                    return new PublisherResponseHandler(getPostIdResponse);
                }
                else if (publisherResponse.Response.Contains("submitted and is pending approval"))
                {
                    var response = FdFunctions.GetDecodedResponse(publisherResponse.Response);
                    var path = Utilities.GetBetween(response, "href=\"", "\"");
                    return new PublisherResponseHandler(publisherResponse, $"https://www.facebook.com{path}");
                }

                else if (publisherResponse.Response.Contains("summary"))
                {
                    return new PublisherResponseHandler(publisherResponse);
                }
                else
                {
                    if (advanceSettingsModel.IsAutoTagFriends && advanceSettingsModel.IsTagUniqueFriends)
                        objFdFunctions.RemoveTaggedFriendsOnError(account, randomFriends, advanceSettingsModel);

                    FacebookErrors errors = FacebookErrors.UnknownErrorOccurred;

                    //Todo :  Analysis why failed with response, if FacebookErrors enum doesn't contains then add to enum list as well.
                    if (publisherResponse.Response.Contains("\"errorSummary\":\"You've been temporarily blocked\""))
                        errors = FacebookErrors.ActivityBlocked;

                    //Todo :  Analysis why failed with response, if FacebookErrors enum doesn't contains then add to enum list as well.

                    return new PublisherResponseHandler(publisherResponse, errors);
                }
                #endregion

            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException("Operation Cancellated!");
            }
            catch (AggregateException ae)
            {
                ae.HandleOperationCancellation();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return null;
        }


        public PublisherResponseHandler PostToFriends(DominatorAccountModel account,
             string friendUrl, PublisherPostlistModel postDetails,
             CancellationTokenSource campaignCancellationToken, GeneralModel generalSettingsModel,
             FacebookModel advanceSettingsModel)
        {
            try
            {
                #region Properties


                //bool isTagSuccesfull = false;
                string composerId;
                var mediaId = new Dictionary<string, string>();
                string waterfallId = Utilities.GetGuid();
                string friendId;


                #endregion

                campaignCancellationToken.Token.ThrowIfCancellationRequested();

                #region Making urls and gather composer id with privacy id

                var url = friendUrl.Contains(FdConstants.FbHomeUrl)
                    ? friendUrl
                    : $"{FdConstants.FbHomeUrl}{friendUrl}";

                var fanPageResponse = _httpHelper.GetRequest(url);
                try
                {
                    friendId = FdRegexUtility.FirstMatchExtractor(fanPageResponse.Response, FdConstants.ProfileIdRegex);

                    composerId = FdRegexUtility.FirstMatchExtractor(fanPageResponse.Response, FdConstants.ComposerIdRegex);
                    //friendId = FdRegexUtility.FirstMatchExtractor(fanPageResponse.Response, FdConstants.ProfileIdRegex);

                    //composerId = FdRegexUtility.FirstMatchExtractor(fanPageResponse.Response, FdConstants.ComposerIdRegex);
                }
                catch (Exception)
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, account.AccountBaseModel.AccountNetwork,
                        account.AccountBaseModel.UserName, "", "LangKeyPageNotAllowVisitorsPost".FromResourceDictionary());

                    //Todo :  Analysis why failed with response, if FacebookErrors enum doesn't contains then add to enum list as well.
                    FacebookErrors errors = FacebookErrors.UnknownErrorOccurred;
                    return new PublisherResponseHandler(fanPageResponse, errors);
                }

                #endregion

                #region Tag Friends

                var randomFriends = TagFriends(advanceSettingsModel, account);

                var mentions = MentionUsers(advanceSettingsModel, account);

                #endregion

                #region Uploading Medias and publishing

                if (postDetails.MediaList.Count > 0)
                {
                    campaignCancellationToken.Token.ThrowIfCancellationRequested();

                    GlobusLogHelper.log.Info(Log.CustomMessage, account.AccountBaseModel.AccountNetwork,
                        account.AccountBaseModel.UserName, "", "LangKeyUploadingMediaFile".FromResourceDictionary());

                    mediaId = UploadImageAndGetMediaId(postDetails, account, account.AccountBaseModel.UserId, waterfallId, "OwnWall");
                }

                FdRequestParameter objParameter = new FdRequestParameter();

                objParameter.UrlParameters.Add("doc_id", "1931212663571278");
                objParameter.UrlParameters.Add("dpr", "1.5");

                PublisherParameter publisherParameter = new PublisherParameter
                {
                    ComposerId = composerId,
                    MediaDictionary = mediaId,
                    PageId = friendId,
                    Message = postDetails.PostDescription,
                    WaterfallId = waterfallId,
                    TargetType = "FriendWall",
                    Mentions = mentions.ToArray(),
                    PostDetails = postDetails
                };

                if (randomFriends.Count > 0)
                    publisherParameter.Tags = randomFriends.ToArray();

                var jsonString = GetFriendWallPublisherParameter(account, publisherParameter);

                objParameter.PostDataParameters.Add("variables", Uri.EscapeDataString(jsonString));
                objParameter.PostDataParameters.Add("__req", "4w");

                objParameter = CommonPostDataParameters(account, objParameter);

                url = $" {FdConstants.FbHomeUrl}webgraphql/mutation/";
                url = objParameter.GenerateUrl(url);

                var postData = objParameter.GetPostDataFromParameters();

                var request = _httpHelper.GetRequestParameter();

                request.ContentType = FdConstants.ContentType;

                _httpHelper.SetRequestParameter(request);

                campaignCancellationToken.Token.ThrowIfCancellationRequested();

                var publisherResponse = _httpHelper.PostRequest(url, postData);

                #endregion

                #region Response Handler


                if (publisherResponse.Response.Contains("story_create") &&
                         publisherResponse.Response.Contains("story_id"))
                {
                    url = $"{FdConstants.FbHomeUrl}{account.AccountBaseModel.UserId}/allactivity";

                    //FdLoginProcess.RequestParameterInitialize(account);

                    var getPostIdResponse = _httpHelper.GetRequest(url);

                    return new PublisherResponseHandler(getPostIdResponse, true, friendId);
                }
                else
                {
                    //Todo :  Analysis why failed with response, if FacebookErrors enum doesn't contains then add to enum list as well.
                    FacebookErrors errors = FacebookErrors.UnknownErrorOccurred;
                    return new PublisherResponseHandler(publisherResponse, errors);
                }

                #endregion
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException("Operation Cancellated!");
            }
            catch (AggregateException ae)
            {
                ae.HandleOperationCancellation();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return null;
        }



        public PublisherResponseHandler SellPostToGroups(DominatorAccountModel account,
            string groupUrl, PublisherPostlistModel postDetails,
            CancellationTokenSource campaignCancellationToken, GeneralModel generalSettingsModel,
            FacebookModel advanceSettingsModel)
        {
            try
            {
                #region Properties

                //bool isTagSuccesfull = false;
                string composerId;
                string groupId;
                Dictionary<string, string> mediaId = new Dictionary<string, string>();
                string waterfallId = FdFunctions.GetRandomHexNumber(32).ToLower();
                var url = groupUrl;
                var groupPageResponse = _httpHelper.GetRequest(url);

                #endregion

                #region Collect group name , group Id, composer Id

                try
                {
                    campaignCancellationToken.Token.ThrowIfCancellationRequested();

                    string decodedResponse = FdFunctions.GetDecodedResponse(groupPageResponse.Response);
                    groupId = FdRegexUtility.FirstMatchExtractor(decodedResponse, FdConstants.GroupIdRegex);

                    if (string.IsNullOrEmpty(groupId))
                        groupId = Utilities.GetBetween(groupPageResponse.Response, "groups%2F", "%2F");

                    groupId = FdFunctions.GetIntegerOnlyString(groupId);
                }
                catch (Exception)
                {
                    //Todo :  Analysis why failed with response, if FacebookErrors enum doesn't contains then add to enum list as well.
                    FacebookErrors errors = FacebookErrors.UnknownErrorOccurred;
                    return new PublisherResponseHandler(groupPageResponse, errors);
                }
                if (string.IsNullOrEmpty(groupId))
                {
                    try
                    {
                        groupUrl = url.Split('?')[0];

                        if (!groupUrl.Contains("/about/"))
                        {

                            groupUrl = groupUrl[groupUrl.Length - 1] == '/'
                                ? groupUrl + "about/"
                                : groupUrl + "/about/";

                        }
                        groupPageResponse = _httpHelper.GetRequest(groupUrl);

                        groupId = FdRegexUtility.FirstMatchExtractor(groupPageResponse.Response, FdConstants.GroupIdRegex);
                        //groupId = FdRegexUtility.FirstMatchExtractor(groupPageResponse.Response, FdConstants.GroupIdRegex);
                        groupId = FdFunctions.GetIntegerOnlyString(groupId);
                    }
                    catch (Exception)
                    {
                        //Todo :  Analysis why failed with response, if FacebookErrors enum doesn't contains then add to enum list as well.
                        FacebookErrors errors = FacebookErrors.UnknownErrorOccurred;
                        return new PublisherResponseHandler(groupPageResponse, errors);
                    }
                }
                try
                {
                    var response = FdFunctions.GetDecodedResponse(groupPageResponse.Response);
                    composerId = FdRegexUtility.FirstMatchExtractor(response, FdConstants.ComposerIdRegex);
                }
                catch (Exception)
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, account.AccountBaseModel.AccountNetwork,
                        account.AccountBaseModel.UserName, "", "LangKeyGroupNotAllowVisitorsPost".FromResourceDictionary());
                    //Todo :  Analysis why failed with response, if FacebookErrors enum doesn't contains then add to enum list as well.
                    FacebookErrors errors = FacebookErrors.UnknownErrorOccurred;
                    return new PublisherResponseHandler(groupPageResponse, errors);
                }

                if (composerId == string.Empty)
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, account.AccountBaseModel.AccountNetwork,
                        account.AccountBaseModel.UserName, "", "LangKeyGroupNotAllowVisitorsPost".FromResourceDictionary());
                    //Todo :  Analysis why failed with response, if FacebookErrors enum doesn't contains then add to enum list as well.
                    FacebookErrors errors = FacebookErrors.UnknownErrorOccurred;
                    return new PublisherResponseHandler(groupPageResponse, errors);
                }

                #endregion

                #region Tag Friends

                var randomFriends = TagFriends(advanceSettingsModel, account);

                var mentions = MentionUsers(advanceSettingsModel, account);

                #endregion

                #region Uploading medias and publishing

                if (postDetails.MediaList.Count > 0)
                {
                    campaignCancellationToken.Token.ThrowIfCancellationRequested();
                    GlobusLogHelper.log.Info(Log.CustomMessage, account.AccountBaseModel.AccountNetwork,
                        account.AccountBaseModel.UserName, "", "LangKeyUploadingMediaFile".FromResourceDictionary());
                    mediaId = UploadImageAndGetMediaId(postDetails, account, groupId, waterfallId, "Groups");
                }

                _delayService.ThreadSleep(500);

                var currency = GetCurrencyDetails(account, groupId, composerId);

                _delayService.ThreadSleep(500);

                var locationId = GetLocationDetails(account, postDetails.FdSellLocation, composerId);

                _delayService.ThreadSleep(500);

                FdRequestParameter objParameter = new FdRequestParameter();

                objParameter.UrlParameters.Add("doc_id", "1931212663571278");
                objParameter.UrlParameters.Add("dpr", "1.5");


                PublisherParameter publisherParameter = new PublisherParameter
                {
                    ComposerId = composerId,
                    MediaDictionary = mediaId,
                    PageId = groupId,
                    WaterfallId = waterfallId,
                    PostDetails = postDetails,
                    Currency = currency,
                    LocationId = locationId,
                    Mentions = mentions.ToArray()
                };

                if (randomFriends.Count > 0)
                    publisherParameter.Tags = randomFriends.ToArray();

                var jsonString = GetPublisherParameterBuySell
                    (account, publisherParameter);

                objParameter.PostDataParameters.Add("variables", Uri.EscapeDataString(jsonString));
                objParameter.PostDataParameters.Add("__req", "4w");

                objParameter = CommonPostDataParameters(account, objParameter);

                url = $" {FdConstants.FbHomeUrl}webgraphql/mutation/";
                url = objParameter.GenerateUrl(url);

                var postData = objParameter.GetPostDataFromParameters();

                var request = _httpHelper.GetRequestParameter();

                request.ContentType = FdConstants.ContentType;

                _httpHelper.SetRequestParameter(request);

                campaignCancellationToken.Token.ThrowIfCancellationRequested();

                _httpHelper.PostRequest(url, postData);

                _delayService.ThreadSleep(2000);

                var activityLodResponse = _httpHelper.GetRequest(FdConstants.ActivityLogGroupPost(account.AccountBaseModel.UserId));


                return new PublisherResponseHandler(activityLodResponse, groupId, publisherParameter.PostDetails.FdSellProductTitle,
                    publisherParameter.PostDetails.FdSellLocation);

                #endregion

            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException("Operation Cancellated!");
            }
            catch (AggregateException ae)
            {
                ae.HandleOperationCancellation();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return null;
        }


        public PublisherResponseHandler ExtractPublisherParameterWall(DominatorAccountModel account, ref PublisherParameter objPublisherParameter,
            CancellationTokenSource campaignCancellationToken)
        {

            campaignCancellationToken.Token.ThrowIfCancellationRequested();

            var url = $"{FdConstants.FbHomeUrl}{account.AccountBaseModel.UserId}";

            var ownWallResponse = _httpHelper.GetRequest(url);

            try
            {
                var response = FdFunctions.GetDecodedResponse(ownWallResponse.Response);

                objPublisherParameter.WebPrivacy = FdRegexUtility.FirstMatchExtractor(response, FdConstants.WebPrivacyRegex);
                objPublisherParameter.ComposerId = FdRegexUtility.FirstMatchExtractor(ownWallResponse.Response, FdConstants.ComposerIdRegex);


                ////webPrivacy = FdRegexUtility.FirstMatchExtractor(response, FdConstants.WebPrivacyRegex);
                ////composerId = FdRegexUtility.FirstMatchExtractor(groupPageResponse.Response, FdConstants.ComposerIdRegex);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                GlobusLogHelper.log.Info(Log.CustomMessage, account.AccountBaseModel.AccountNetwork,
                    account.AccountBaseModel.UserName, "", "LangKeyPageNotAllowVisitorsPost".FromResourceDictionary());

                //Todo :  Analysis why failed with response, if FacebookErrors enum doesn't contains then add to enum list as well.
                FacebookErrors errors = FacebookErrors.UnknownErrorOccurred;
                return new PublisherResponseHandler(ownWallResponse, errors);
            }

            return null;
        }



        public List<string> GetVideoFromPages(DominatorAccountModel account, string pageUrl,
            int videoCount, string[] videoIds)
        {
            var pageId = GetPageIdFromUrl(account, pageUrl);

            ScrapVideoFromPageResponseHandler objScrapVideoFromPageResponseHandler;

            do
            {
                var response = _httpHelper.GetRequest($"{FdConstants.FbHomeUrl}{pageId}/videos");

                objScrapVideoFromPageResponseHandler =
                    new ScrapVideoFromPageResponseHandler(response, videoIds.ToList());

            } while (objScrapVideoFromPageResponseHandler.ListVideoIds.Count <= videoCount);

            videoIds.Shuffle();

            Random random = new Random();

            return videoIds.OrderBy(x => random.Next()).Take(videoCount).ToList();

        }
        #endregion


        #region Publisher supporting Functions such as Tagging, Uploading , Checkin location


        private string GetPublisherParameterImagePage
            (DominatorAccountModel account, PublisherParameter publisherParameter, string postAsPageId = "")
        {
            FdRequestParameter objParameter = new FdRequestParameter();

            FdFunctions objfdFdFunctions = new FdFunctions(account);

            var mentionString = string.Empty;

            var actorId = string.IsNullOrEmpty(postAsPageId) || postAsPageId == "0" ? account.AccountBaseModel.UserId : postAsPageId;

            var array = new string[0];

            var tagsArray = new string[0];

            var mentionsArray = new FdPublisherJsonElement[0];

            if (publisherParameter.Mentions != null)
                mentionsArray = new FdPublisherJsonElement[publisherParameter.Mentions.Length];

            if (publisherParameter.Tags != null)
                tagsArray = new string[publisherParameter.Tags.Length];

            int tagUserCount = 0;

            int offset;

            int length = 0;

            int mentionUserCount = 0;

            int unicodeCharacterCount = 0;

            publisherParameter.Tags?.ForEach(x =>
            {
                var friendId = FdFunctions.GetIntegerOnlyString(x);
                tagsArray[tagUserCount] = friendId;
                tagUserCount++;
            });

            if (publisherParameter.Mentions != null)
            {
                int startIndex;
                if (publisherParameter.PostDetails.PostDescription.Contains("<MENTIONS>") &&
                    publisherParameter.Mentions.Length > 0)
                {
                    startIndex = publisherParameter.PostDetails.PostDescription.IndexOf
                        ("<MENTIONS>", 0, StringComparison.Ordinal);

                    offset = startIndex;
                }
                else
                {
                    publisherParameter.Message = publisherParameter.Message.Replace("\r\n", "\n").Replace("\n\n", "\n");
                    startIndex = publisherParameter.Message.ToArray().Length;

                    unicodeCharacterCount = publisherParameter.Message.Count(c => c > 255);

                    offset = unicodeCharacterCount == 0 ? startIndex : 0;
                }

                publisherParameter.Mentions?.ForEach(x =>
                {
                    try
                    {
                        var friendId = FdFunctions.GetIntegerOnlyString(x);
                        string friendName = objfdFdFunctions.GetFriendName(friendId);
                        offset = length == 0 ? offset : length + offset + 1;
                        length = friendName.Length;
                        FdPublisherJsonElement objMentionUserDetails = new FdPublisherJsonElement()
                        {
                            Offset = offset.ToString(),
                            Length = length.ToString(),
                            Entity = new FdPublisherJsonElement()
                            {
                                Id = friendId
                            }
                        };
                        mentionString += friendName + " ";
                        mentionsArray[mentionUserCount] = objMentionUserDetails;
                        mentionUserCount++;
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                });

                publisherParameter.Message =
                    publisherParameter.Message.Contains("<MENTIONS>")
                        ? publisherParameter.Message.ReplaceAt(startIndex, 10, mentionString)
                        : (unicodeCharacterCount == 0 ? publisherParameter.Message.Insert(publisherParameter.Message.Length, mentionString) :
                            publisherParameter.Message.Insert(0, mentionString));

            }

            var attachmentArray = new FdPublisherJsonElement[publisherParameter.MediaDictionary.Count];

            var count = 0;

            foreach (KeyValuePair<string, string> media in publisherParameter.MediaDictionary)
            {
                if (media.Value == "Image")
                {
                    FdPublisherJsonElement photo = new FdPublisherJsonElement()
                    {
                        Photo = new FdPublisherJsonElement()
                        {
                            Id = media.Key,
                            Tags = array
                        }

                    };
                    attachmentArray[count] = photo;
                }
                else
                {
                    FdPublisherJsonElement video = new FdPublisherJsonElement()
                    {
                        Video = new FdPublisherJsonElement()
                        {
                            Id = media.Key,
                            NotifyWhenProcessed = true
                        }

                    };
                    attachmentArray[count] = video;
                }
                count++;
            }


            var target = string.Empty;
            var composerEntryPoint = string.Empty;
            var composerSourceSurface = string.Empty;
            var refValue = string.Empty;

            if (publisherParameter.TargetType == "Groups")
            {
                target = "group";
                composerEntryPoint = "group";
                composerSourceSurface = "group";
                refValue = "group";
            }
            else if (publisherParameter.TargetType == "Pages")
            {
                target = "page";
                composerEntryPoint = "pages_feed";
                composerSourceSurface = "page";
                refValue = "pages_feed";
            }

            FdPublisherJsonElement jsonelement = new FdPublisherJsonElement()
            {
                ClientMutationId = Utilities.GetGuid(),
                // ClientMutationId = "878beda3-50d2-4b7a-8c8d-ebc94fab2ae0",
                ActorId = actorId,
                Input = new FdPublisherJsonElement()
                {
                    ActorId = actorId,
                    ClientMutationId = Utilities.GetGuid(),
                    //ClientMutationId = "ef2daeb4-29dc-4ed3-b944-bde2753731a4",
                    Source = "WWW",
                    Audience = postAsPageId != publisherParameter.PageId ? new FdPublisherJsonElement()
                    {
                        ToId = publisherParameter.PageId
                    } :
                    new FdPublisherJsonElement()
                    {
                        Privacy = new FdPublisherJsonElement()
                        {
                            BaseState = "EVERYONE"
                        }
                    },
                    Message = new FdPublisherJsonElement()
                    {
                        Text = publisherParameter.Message,
                        Ranges = mentionsArray
                    },
                    Logging = new FdPublisherJsonElement()
                    {
                        ComposerSessionId = publisherParameter.WaterfallId,
                        Ref = refValue
                    },
                    WithTagsId = tagsArray,

                    MultilingualTranslations = array,

                    CameraPostContext = new FdPublisherJsonElement()
                    {
                        DeduplicationId = publisherParameter.WaterfallId,
                        Source = "composer"
                    },
                    ComposerSourceSurface = composerSourceSurface,

                    ComposerEntryPoint = composerEntryPoint,

                    ComposerEntryTime = DateTime.Now.Hour,

                    DirectShareStatus = "NOT_SHARED",

                    SponsorRelationship = "WITH",

                    WebGraphmlMigrationParams = new FdPublisherJsonElement()
                    {
                        TargetType = target,
                        XhpcComposerid = publisherParameter.ComposerId,
                        XhpcContext = "profile",
                        XhpcFinch = true,
                        XhpcPublishType = "FEED_INSERT",
                        XhpcTimeline = true,
                        WaterfallId = publisherParameter.WaterfallId
                    },
                    PlaceAttachmentSetting = "HIDE_ATTACHMENT",
                    Attachments = attachmentArray
                }
            };

            return objParameter.GetJsonString(jsonelement);
        }


        private string GetFriendWallPublisherParameter
         (DominatorAccountModel account, PublisherParameter publisherParameter)
        {
            FdRequestParameter objParameter = new FdRequestParameter();

            FdFunctions objfdFdFunctions = new FdFunctions(account);

            var mentionsArray = new FdPublisherJsonElement[0];

            string mentionString = String.Empty;

            var array = new string[0];

            var tagsArray = new string[0];

            if (publisherParameter.Tags != null)
                tagsArray = new string[publisherParameter.Tags.Length];

            int tagUserCount = 0;

            int offset;

            int length = 0;

            int mentionUserCount = 0;

            int unicodeCharacterCount = 0;

            publisherParameter.Tags?.ForEach(x =>
            {
                var friendId = FdFunctions.GetIntegerOnlyString(x);
                tagsArray[tagUserCount] = friendId;
                tagUserCount++;
            });


            if (publisherParameter.Mentions != null)
            {
                int startIndex;
                if (publisherParameter.PostDetails.PostDescription.Contains("<MENTIONS>") &&
                    publisherParameter.Mentions.Length > 0)
                {
                    startIndex = publisherParameter.PostDetails.PostDescription.IndexOf
                        ("<MENTIONS>", 0, StringComparison.Ordinal);

                    offset = startIndex;
                }
                else
                {
                    publisherParameter.Message = publisherParameter.Message.Replace("\r\n", "\n").Replace("\n\n", "\n");
                    startIndex = publisherParameter.Message.ToArray().Length;

                    unicodeCharacterCount = publisherParameter.Message.Count(c => c > 255);

                    offset = unicodeCharacterCount == 0 ? startIndex : 0;
                }

                publisherParameter.Mentions?.ForEach(x =>
                {
                    try
                    {
                        var friendId = FdFunctions.GetIntegerOnlyString(x);
                        string friendName = objfdFdFunctions.GetFriendName(friendId);
                        offset = length == 0 ? offset : length + offset + 1;
                        length = friendName.Length;
                        FdPublisherJsonElement objMentionUserDetails = new FdPublisherJsonElement()
                        {
                            Offset = offset.ToString(),
                            Length = length.ToString(),
                            Entity = new FdPublisherJsonElement()
                            {
                                Id = friendId
                            }
                        };
                        mentionString += friendName + " ";
                        mentionsArray[mentionUserCount] = objMentionUserDetails;
                        mentionUserCount++;
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                });

                publisherParameter.Message =
                    publisherParameter.Message.Contains("<MENTIONS>")
                        ? publisherParameter.Message.ReplaceAt(startIndex, 10, mentionString)
                        : (unicodeCharacterCount == 0 ? publisherParameter.Message.Insert(publisherParameter.Message.Length, mentionString) :
                            publisherParameter.Message.Insert(0, mentionString));

            }


            var attachmentArray = new FdPublisherJsonElement[publisherParameter.MediaDictionary.Count];

            var count = 0;

            foreach (KeyValuePair<string, string> media in publisherParameter.MediaDictionary)
            {
                if (media.Value == "Image")
                {
                    FdPublisherJsonElement photo = new FdPublisherJsonElement()
                    {
                        Photo = new FdPublisherJsonElement()
                        {
                            Id = media.Key,
                            Tags = array
                        }

                    };
                    attachmentArray[count] = photo;
                }
                else
                {
                    FdPublisherJsonElement video = new FdPublisherJsonElement()
                    {
                        Video = new FdPublisherJsonElement()
                        {
                            Id = media.Key,
                            NotifyWhenProcessed = true
                        }

                    };
                    attachmentArray[count] = video;
                }
                count++;
            }

            var target = "wall";
            var composerEntryPoint = "timeline";
            var composerSourceSurface = "timeline";
            var refValue = "timeline";

            FdPublisherJsonElement jsonelement = new FdPublisherJsonElement()
            {
                ClientMutationId = Utilities.GetGuid(),
                // ClientMutationId = "878beda3-50d2-4b7a-8c8d-ebc94fab2ae0",
                ActorId = account.AccountBaseModel.UserId,
                Input = new FdPublisherJsonElement()
                {
                    ActorId = account.AccountBaseModel.UserId,
                    ClientMutationId = Utilities.GetGuid(),
                    //ClientMutationId = "ef2daeb4-29dc-4ed3-b944-bde2753731a4",
                    Source = "WWW",
                    Audience = new FdPublisherJsonElement()
                    {
                        ToId = publisherParameter.PageId
                    },
                    Message = new FdPublisherJsonElement()
                    {
                        Text = publisherParameter.Message,
                        Ranges = mentionsArray
                    },
                    Logging = new FdPublisherJsonElement()
                    {
                        ComposerSessionId = publisherParameter.WaterfallId,
                        Ref = refValue
                    },
                    WithTagsId = tagsArray,

                    MultilingualTranslations = array,

                    CameraPostContext = new FdPublisherJsonElement()
                    {
                        DeduplicationId = publisherParameter.WaterfallId,
                        Source = "composer"
                    },
                    ComposerSourceSurface = composerSourceSurface,

                    ComposerEntryPoint = composerEntryPoint,

                    ComposerEntryTime = DateTime.Now.Hour,

                    DirectShareStatus = "NOT_SHARED",

                    SponsorRelationship = "WITH",

                    WebGraphmlMigrationParams = new FdPublisherJsonElement()
                    {
                        TargetType = target,
                        XhpcComposerid = publisherParameter.ComposerId,
                        XhpcContext = "profile",
                        XhpcPublishType = "FEED_INSERT",
                        XhpcTimeline = true,
                        WaterfallId = publisherParameter.WaterfallId
                    },
                    PlaceAttachmentSetting = "HIDE_ATTACHMENT",
                    Attachments = attachmentArray
                }
            };

            return objParameter.GetJsonString(jsonelement);
        }


        private string GetPublisherParameterBuySell
           (DominatorAccountModel account, PublisherParameter publisherParameter)
        {
            FdRequestParameter objParameter = new FdRequestParameter();

            FdFunctions objfdFdFunctions = new FdFunctions(account);

            var mentionsArray = new FdPublisherJsonElement[0];

            string mentionString = String.Empty;

            var array = new string[0];

            var tagsArray = new string[0];

            if (publisherParameter.Tags != null)
            {
                tagsArray = new string[publisherParameter.Tags.Length];
            }

            int tagUserCount = 0;

            int offset;

            int length = 0;

            int mentionUserCount = 0;


            publisherParameter.Tags?.ForEach(x =>
            {
                var friendId = FdFunctions.GetIntegerOnlyString(x);
                tagsArray[tagUserCount] = friendId;
                tagUserCount++;
            });


            if (publisherParameter.Mentions != null && publisherParameter.Mentions.Length > 0)
            {
                int startIndex;
                int unicodeCharacterCount = 0;
                if (publisherParameter.PostDetails.PostDescription.Contains("<MENTIONS>") &&
                    publisherParameter.Mentions.Length > 0)
                {
                    startIndex = publisherParameter.PostDetails.PostDescription.IndexOf
                        ("<MENTIONS>", 0, StringComparison.Ordinal);

                    offset = startIndex;
                }
                else
                {
                    publisherParameter.Message = publisherParameter.Message.Replace("\r\n", "\n").Replace("\n\n", "\n");
                    startIndex = publisherParameter.Message.ToArray().Length;

                    unicodeCharacterCount = publisherParameter.Message.Count(c => c > 255);

                    offset = unicodeCharacterCount == 0 ? startIndex : 0;
                }

                publisherParameter.Mentions?.ForEach(x =>
                {
                    try
                    {
                        var friendId = FdFunctions.GetIntegerOnlyString(x);
                        string friendName = objfdFdFunctions.GetFriendName(friendId);
                        offset = length == 0 ? offset : length + offset + 1;
                        length = friendName.Length;
                        FdPublisherJsonElement objMentionUserDetails = new FdPublisherJsonElement()
                        {
                            Offset = offset.ToString(),
                            Length = length.ToString(),
                            Entity = new FdPublisherJsonElement()
                            {
                                Id = friendId
                            }
                        };
                        mentionString += friendName + " ";
                        mentionsArray[mentionUserCount] = objMentionUserDetails;
                        mentionUserCount++;
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                });

                publisherParameter.Message =
                    publisherParameter.Message.Contains("<MENTIONS>")
                        ? publisherParameter.Message.ReplaceAt(startIndex, 10, mentionString)
                        : (unicodeCharacterCount == 0 ? publisherParameter.Message.Insert(publisherParameter.Message.Length, mentionString) :
                            publisherParameter.Message.Insert(0, mentionString));

            }


            var attachmentArray = new FdPublisherJsonElement[publisherParameter.MediaDictionary.Count + 1];

            var count = 0;

            foreach (KeyValuePair<string, string> media in publisherParameter.MediaDictionary)
            {
                if (media.Value == "Image")
                {
                    FdPublisherJsonElement photo = new FdPublisherJsonElement()
                    {
                        Photo = new FdPublisherJsonElement()
                        {
                            Id = media.Key,
                            Tags = array
                        }

                    };
                    attachmentArray[count] = photo;
                }
                else
                {
                    FdPublisherJsonElement video = new FdPublisherJsonElement()
                    {
                        Video = new FdPublisherJsonElement()
                        {
                            Id = media.Key,
                            NotifyWhenProcessed = true
                        }

                    };
                    attachmentArray[count] = video;
                }
                count++;
            }


            FdPublisherJsonElement productItem = new FdPublisherJsonElement()
            {
                ProductItem = new FdPublisherJsonElement()
                {
                    Title = publisherParameter.PostDetails.FdSellProductTitle,
                    ItemPrice = new FdPublisherJsonElement()
                    {
                        Price = publisherParameter.PostDetails.FdSellPrice.ToString(CultureInfo.InvariantCulture),
                        Currency = publisherParameter.Currency
                    },
                    LocationId = publisherParameter.LocationId,
                    Condition = "USED",
                    PickupNote = new FdPublisherJsonElement()
                    {
                        Text = publisherParameter.PostDetails.FdSellLocation,
                        Ranges = mentionsArray
                    },
                    Description = new FdPublisherJsonElement()
                    {
                        Text = publisherParameter.PostDetails.PostDescription,
                        Ranges = mentionsArray
                    }
                }

            };

            attachmentArray[count] = productItem;


            var target = "group";
            var composerEntryPoint = "group";
            var composerSourceSurface = "group";
            var refValue = "group";



            FdPublisherJsonElement jsonelement = new FdPublisherJsonElement()
            {
                ClientMutationId = Utilities.GetGuid(),
                // ClientMutationId = "878beda3-50d2-4b7a-8c8d-ebc94fab2ae0",
                ActorId = account.AccountBaseModel.UserId,
                Input = new FdPublisherJsonElement()
                {
                    ActorId = account.AccountBaseModel.UserId,
                    ClientMutationId = Utilities.GetGuid(),
                    //ClientMutationId = "ef2daeb4-29dc-4ed3-b944-bde2753731a4",
                    Source = "WWW",
                    Audience = new FdPublisherJsonElement()
                    {
                        ToId = publisherParameter.PageId
                    },
                    Message = new FdPublisherJsonElement()
                    {
                        Text = string.Empty,
                        Ranges = mentionsArray
                    },
                    Logging = new FdPublisherJsonElement()
                    {
                        ComposerSessionId = publisherParameter.WaterfallId,
                        Ref = refValue
                    },
                    WithTagsId = tagsArray,

                    MultilingualTranslations = array,

                    CameraPostContext = new FdPublisherJsonElement()
                    {
                        DeduplicationId = publisherParameter.WaterfallId,
                        Source = "composer"
                    },
                    ComposerSourceSurface = composerSourceSurface,

                    ComposerEntryPoint = composerEntryPoint,

                    ComposerEntryTime = DateTime.Now.Hour,

                    DirectShareStatus = "NOT_SHARED",

                    SponsorRelationship = "WITH",

                    WebGraphmlMigrationParams = new FdPublisherJsonElement()
                    {
                        TargetType = target,
                        XhpcComposerid = publisherParameter.ComposerId,
                        XhpcContext = "profile",
                        XhpcPublishType = "FEED_INSERT",
                        WaterfallId = publisherParameter.WaterfallId,
                        XpostTargetIds = publisherParameter.PageId
                    },
                    PlaceAttachmentSetting = "HIDE_ATTACHMENT",
                    Attachments = attachmentArray
                }
            };

            return objParameter.GetJsonString(jsonelement);
        }


        private string GetPublisherParameterWall
            (DominatorAccountModel account, PublisherParameter publisherParameter, PublisherPostlistModel postDetails)
        {
            FdRequestParameter objParameter = new FdRequestParameter();

            FdFunctions objfdFdFunctions = new FdFunctions(account);

            var sessionId = FdFunctions.GetPublisherSessionId();

            var mentionsArray = new FdPublisherJsonElement[0];

            string mentionString = String.Empty;

            var array = new string[0];

            var tagsArray = new string[0];

            if (publisherParameter.Tags != null)
                tagsArray = new string[publisherParameter.Tags.Length];

            if (publisherParameter.Mentions != null)
                mentionsArray = new FdPublisherJsonElement[publisherParameter.Mentions.Length];

            int tagUserCount = 0;

            int mentionUserCount = 0;

            int offset;

            int length = 0;

            int unicodeCharacterCount = 0;

            publisherParameter.Tags?.ForEach(x =>
            {
                var friendId = FdFunctions.GetIntegerOnlyString(x);
                tagsArray[tagUserCount] = friendId;
                tagUserCount++;
            });

            if (publisherParameter.Mentions != null)
            {
                int startIndex;
                if (publisherParameter.PostDetails.PostDescription.Contains("<MENTIONS>") &&
                    publisherParameter.Mentions.Length > 0)
                {
                    startIndex = publisherParameter.PostDetails.PostDescription.IndexOf
                        ("<MENTIONS>", 0, StringComparison.Ordinal);

                    offset = startIndex;
                }
                else
                {
                    publisherParameter.Message = publisherParameter.Message.Replace("\r\n", "\n").Replace("\n\n", "\n");
                    startIndex = publisherParameter.Message.ToArray().Length;

                    unicodeCharacterCount = publisherParameter.Message.Count(c => c > 255);

                    offset = unicodeCharacterCount == 0 ? startIndex : 0;
                }

                publisherParameter.Mentions?.ForEach(x =>
                {
                    try
                    {
                        var friendId = FdFunctions.GetIntegerOnlyString(x);
                        string friendName = objfdFdFunctions.GetFriendName(friendId);
                        offset = length == 0 ? offset : length + offset + 1;
                        length = friendName.Length;
                        FdPublisherJsonElement objMentionUserDetails = new FdPublisherJsonElement()
                        {
                            Offset = offset.ToString(),
                            Length = length.ToString(),
                            Entity = new FdPublisherJsonElement()
                            {
                                Id = friendId
                            }
                        };
                        mentionString += friendName + " ";
                        mentionsArray[mentionUserCount] = objMentionUserDetails;
                        mentionUserCount++;
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                });

                publisherParameter.Message =
                    publisherParameter.Message.Contains("<MENTIONS>")
                        ? publisherParameter.Message.ReplaceAt(startIndex, 10, mentionString)
                        : (unicodeCharacterCount == 0 ? publisherParameter.Message.Insert(publisherParameter.Message.Length, mentionString) :
                            $"{mentionString}\n{publisherParameter.Message}");

            }

            var attachmentArray = new FdPublisherJsonElement[publisherParameter.MediaDictionary.Count];

            var count = 0;

            foreach (KeyValuePair<string, string> media in publisherParameter.MediaDictionary)
            {
                if (media.Value == "Image")
                {
                    FdPublisherJsonElement photo = new FdPublisherJsonElement()
                    {
                        Photo = new FdPublisherJsonElement()
                        {
                            Id = media.Key,
                            Tags = array
                        }

                    };
                    attachmentArray[count] = photo;
                }
                else
                {
                    FdPublisherJsonElement video = new FdPublisherJsonElement()
                    {
                        Video = new FdPublisherJsonElement()
                        {
                            Id = media.Key,
                            NotifyWhenProcessed = true
                        }

                    };
                    attachmentArray[count] = video;
                }
                count++;
            }

            var target = "feed";
            var composerEntryPoint = postDetails.FdPostSettings.IsPostAsStoryPost ? "feedx_sprouts" : "timeline";
            var composerSourceSurface = postDetails.FdPostSettings.IsPostAsStoryPost ? "newsfeed" : "timeline";

            var refValue = postDetails.FdPostSettings.IsPostAsStoryPost ? "feedx_sprouts" : "timeline";
            bool? xhpcTimeline = null;


            FdPublisherJsonElement jsonelement = new FdPublisherJsonElement()
            {
                ClientMutationId = "878beda3-50d2-4b7a-8c8d-ebc94fab2ae0",
                ActorId = account.AccountBaseModel.UserId,
                Input = new FdPublisherJsonElement()
                {
                    ActorId = account.AccountBaseModel.UserId,
                    ClientMutationId = "ef2daeb4-29dc-4ed3-b944-bde2753731a4",
                    Source = "WWW",
                    Audience = postDetails.FdPostSettings.IsPostAsStoryPost ? null :
                            new FdPublisherJsonElement()
                            {
                                WebPrivacyX = publisherParameter.WebPrivacy
                            },
                    Audiences = !postDetails.FdPostSettings.IsPostAsStoryPost ? null :
                            new FdPublisherJsonElement[]
                            {
                                new FdPublisherJsonElement()
                                {
                                      Stories = new FdPublisherJsonElement()
                                          {
                                              Self = new FdPublisherJsonElement()
                                                        {
                                                            TargetId = account.AccountBaseModel.UserId
                                                        }
                                          }
                                }

                            },
                    Message = new FdPublisherJsonElement()
                    {
                        Text = publisherParameter.Message,
                        Ranges = mentionsArray
                    },
                    Logging = new FdPublisherJsonElement()
                    {
                        ComposerSessionId = sessionId,
                        Ref = refValue
                    },
                    WithTagsId = tagsArray,

                    MultilingualTranslations = array,

                    CameraPostContext = new FdPublisherJsonElement()
                    {
                        DeduplicationId = sessionId,
                        Source = "composer"
                    },
                    ComposerSourceSurface = composerSourceSurface,

                    ComposerEntryPoint = composerEntryPoint,

                    ComposerEntryTime = 21,

                    ComposerType = postDetails.FdPostSettings.IsPostAsStoryPost ? composerEntryPoint : null,

                    DirectShareStatus = "NOT_SHARED",

                    SponsorRelationship = "WITH",

                    WebGraphmlMigrationParams = new FdPublisherJsonElement()
                    {
                        TargetType = target,
                        XhpcComposerid = publisherParameter.ComposerId,
                        XhpcContext = "profile",
                        XhpcPublishType = "FEED_INSERT",
                        XhpcTimeline = postDetails.FdPostSettings.IsPostAsStoryPost ? xhpcTimeline : true,
                        IsAlsoPostingVideoToFeed = postDetails.FdPostSettings.IsPostAsStoryPost ? false : true,
                        WaterfallId = sessionId
                    },

                    ExtensibleSproutsRankerRequest = new FdPublisherJsonElement()
                    {
                        RequestId = "cvBjCwABAAAAJDM2YmY5MWQ2LTkwZjUtNDM0Mi1mNGNiLTEzOTY5NTUyNDU1OQoAAgAAAABcTpC1CwADAAAAEEJJUlRIREFZX1NUSUNLRVIGAAQAAAsABQAAABhVTkRJUkVDVEVEX0ZFRURfQ09NUE9TRVIA",
                    },

                    ExternalMovieData = array,
                    PlaceAttachmentSetting = "HIDE_ATTACHMENT",
                    Attachments = attachmentArray
                }
            };

            return objParameter.GetJsonString(jsonelement);
        }


        private string GetPublisherDescrWithMentionForShare(DominatorAccountModel account,
            ref string postDescription, List<string> mentionList)
        {
            FdFunctions objFdFunctions = new FdFunctions(account);

            var mention = string.Empty;

            var mentionsUsed = string.Empty;

            try
            {
                mentionList.ForEach(x =>
                   {
                       var friendId = FdFunctions.GetIntegerOnlyString(x);
                       string friendName = objFdFunctions.GetFriendName(friendId);
                       mention += $"@[{friendId}:{friendName}] ";

                       mentionsUsed += $"{friendName} ";
                   });

                mention = $"{postDescription}\n{mention}";

                postDescription = $"{postDescription}\n{mentionsUsed}";

                return mention;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return string.Empty;
        }

        //private Dictionary<string, string> UploadImageAndGetMediaId
        //    (PublisherPostlistModel postDetails, DominatorAccountModel account,
        //    string targetId, string waterfallId, string postSource, string av = "")
        //{
        //    try
        //    {

        //        var mediaId = string.Empty;

        //        av = FdFunctions.GetIntegerOnlyString(av);

        //        var fbDtsg = Uri.UnescapeDataString(account.SessionId);

        //        List<string> fbSupportedVideoFormat = new List<string> { "gif", "3g2", "3gp", "3gpp", "asf", "avi", "dat", "divx", "dv", "f4v", "flv", "m2ts", "m4v", "mkv", "mod", "mov", "mp4", "mpe", "mpeg", "mpeg4", "mpg", "mts", "nsv", "ogm", "ogv", "qt", "tod", "ts", "vob", "wmv" };

        //        Dictionary<string, string> mediaDictionary = new Dictionary<string, string>();

        //        foreach (var media in postDetails.MediaList)
        //        {
        //            bool isUploaded;

        //            GlobusLogHelper.log.Info(Log.UploadingMedia, account.AccountBaseModel.AccountNetwork, account.AccountBaseModel.UserName, media);

        //            FdRequestParameter objParameter = new FdRequestParameter();

        //            string fileExtension = FdFunctions.ReversegetBetween(media + "$", "$", ".");

        //            if (postDetails.PostSource == DominatorHouseCore.Enums.SocioPublisher.PostSource.ScrapedPost)
        //            {
        //                fileExtension = fileExtension.Split('?')[0];
        //            }



        //            if (fbSupportedVideoFormat.Contains(fileExtension))
        //            {
        //                try
        //                {
        //                    NameValueCollection nvc = new NameValueCollection();

        //                    byte[] buffer;

        //                    if (!File.Exists(media))
        //                    {
        //                        buffer = MediaUtilites.GetImageBytesFromUrl(media);
        //                    }
        //                    else
        //                    {
        //                        var br = new BinaryReader(new FileStream(media, FileMode.Open, FileAccess.Read));
        //                        buffer = br.ReadBytes((int)br.BaseStream.Length);
        //                    }

        //                    long length = buffer.Length;

        //                    objParameter.PostDataParameters.Add("file_size", length.ToString());
        //                    objParameter.PostDataParameters.Add("file_extension", fileExtension);
        //                    objParameter.PostDataParameters.Add("target_id", targetId);
        //                    objParameter.PostDataParameters.Add("source", "composer");
        //                    objParameter.PostDataParameters.Add("composer_dialog_version", "");
        //                    objParameter.PostDataParameters.Add("waterfall_id", waterfallId);

        //                    if (postSource == "Pages")
        //                    {
        //                        objParameter.PostDataParameters.Add("composer_entry_point_ref", "pages_feed");
        //                    }
        //                    else if (postSource == "OwnWall")
        //                    {
        //                        objParameter.PostDataParameters.Add("composer_entry_point_ref", "timeline");
        //                    }
        //                    else if (postSource == "Groups")
        //                    {
        //                        objParameter.PostDataParameters.Add("composer_entry_point_ref", "group");
        //                    }

        //                    objParameter.PostDataParameters.Add("has_file_been_replaced", "false");
        //                    objParameter.PostDataParameters.Add("supports_chunking", "true");
        //                    objParameter.PostDataParameters.Add("supports_file_api", "true");
        //                    objParameter.PostDataParameters.Add("partition_start_offset", "0");
        //                    objParameter.PostDataParameters.Add("partition_end_offset", length.ToString());
        //                    objParameter.PostDataParameters.Add("creator_product", "2");
        //                    objParameter.PostDataParameters.Add("spherical", "false");
        //                    objParameter.PostDataParameters.Add("__req", "1k");

        //                    objParameter = CommonPostDataParameters(account, objParameter);

        //                    string url = FdConstants.VideoUploadUrl;

        //                    objParameter.UrlParameters.Add("av",
        //                        string.IsNullOrEmpty(av) ? account.AccountBaseModel.UserId : av);
        //                    objParameter.UrlParameters.Add("dpr", "1.5");
        //                    objParameter.UrlParameters.Add("__a", "1");

        //                    url = objParameter.GenerateUrl(url);

        //                    byte[] postData = objParameter.GetPostDataFromParameters();

        //                    var request = _httpHelper.GetRequestParameter();

        //                    request.ContentType = FdConstants.ContentType;

        //                    request.Headers["X_FB_VIDEO_WATERFALL_ID"] = waterfallId;

        //                    _httpHelper.SetRequestParameter(request);

        //                    var videoUploadResponse = _httpHelper.PostRequest(url, postData);

        //                    string videoId = FdRegexUtility.FirstMatchExtractor(videoUploadResponse.Response, FdConstants.VideoIdScrapeRegex);


        //                    var filenameArray = Regex.Split(media, @"\\");

        //                    string fileName = filenameArray[filenameArray.Length - 1];

        //                    objParameter.IsMultiPart = true;
        //                    objParameter.IsMultiPartVideo = true;


        //                    objParameter.IsMultiPartVideo = true;


        //                    FileData objimage = new FileData(nvc, fileName, buffer);

        //                    objParameter.FileList.Add(media, objimage);

        //                    postData = objParameter.GetPostDataFromParameters();

        //                    url = FdConstants.ExtractVideoId;

        //                    objParameter.UrlParameters.Clear();

        //                    objParameter.UrlParameters.Add("av",
        //                        string.IsNullOrEmpty(av) ? account.AccountBaseModel.UserId : av);
        //                    objParameter.UrlParameters.Add("dpr", "1");
        //                    objParameter.UrlParameters.Add("video_id", videoId);
        //                    objParameter.UrlParameters.Add("start_offset", "0");
        //                    objParameter.UrlParameters.Add("source", "composer");
        //                    objParameter.UrlParameters.Add("target_id", targetId);
        //                    objParameter.UrlParameters.Add("waterfall_id", waterfallId);
        //                    objParameter.UrlParameters.Add("composer_entry_point_ref", "");
        //                    objParameter.UrlParameters.Add("supports_chunking", "true");
        //                    objParameter.UrlParameters.Add("upload_speed", "");
        //                    objParameter.UrlParameters.Add("partition_start_offset", "0");
        //                    objParameter.UrlParameters.Add("partition_end_offset", buffer.Length.ToString());
        //                    objParameter.UrlParameters.Add("__req", "1k");

        //                    objParameter.UrlParameters.Add("__user", account.AccountBaseModel.UserId);
        //                    objParameter.UrlParameters.Add("__a", "1");
        //                    objParameter.UrlParameters.Add("__dyn", FdConstants.DynParameter);
        //                    objParameter.UrlParameters.Add("__be", "0");
        //                    objParameter.UrlParameters.Add("fb_dtsg", fbDtsg);
        //                    objParameter.UrlParameters.Add("jazoest", FdConstants.JazoestParameterGender);

        //                    url = objParameter.GenerateUrl(url);

        //                    request = _httpHelper.GetRequestParameter();


        //                    request.Accept = "*/*";
        //                    request.Headers[FdConstants.HostKey] = "upload.facebook.com";

        //                    request.ContentType = objParameter.ContentType;
        //                    _httpHelper.SetRequestParameter(request);
        //                    _httpHelper.PostRequest(url, postData);

        //                    mediaDictionary.Add(videoId, "Video");

        //                    isUploaded = true;
        //                }
        //                catch (Exception ex)
        //                {
        //                    isUploaded = false;
        //                    GlobusLogHelper.log.Info(Log.UploadingMediaFailed, account.AccountBaseModel.AccountNetwork, account.AccountBaseModel.UserName, media);
        //                    GlobusLogHelper.log.Info(Log.UploadingMediaFailedReason, account.AccountBaseModel.AccountNetwork, account.AccountBaseModel.UserName, ex.Message);
        //                }
        //            }
        //            else
        //            {
        //                try
        //                {
        //                    byte[] buffer;

        //                    if (!File.Exists(media) || ImageExtracter.IsValidUrl(media))
        //                    {
        //                        var newMedia = media.Replace("||", string.Empty);
        //                        if (newMedia.Contains("https://external") && newMedia.Contains("url="))
        //                        {
        //                            newMedia = Utilities.GetBetween(newMedia, "url=", "&");
        //                            newMedia = Uri.UnescapeDataString(newMedia);
        //                        }
        //                        buffer = MediaUtilites.GetImageBytesFromUrl(newMedia);
        //                    }
        //                    else
        //                    {
        //                        var br = new BinaryReader(new FileStream(media, FileMode.Open, FileAccess.Read));
        //                        buffer = br.ReadBytes((int)br.BaseStream.Length);
        //                    }

        //                    objParameter.IsMultiPart = true;

        //                    var filenameArray = Regex.Split(media, @"\\");

        //                    string fileName = filenameArray[filenameArray.Length - 1];

        //                    NameValueCollection nvc = new NameValueCollection
        //                    {
        //                        {"fb_dtsg", fbDtsg},
        //                        {"qn", waterfallId},
        //                        {"target_id", targetId},
        //                        {"source", "8"},
        //                        {"profile_id", account.AccountBaseModel.UserId},
        //                        {"waterfallxapp", "web_react_composer"}
        //                    };



        //                    FileData objimage = new FileData(nvc, fileName, buffer);

        //                    objParameter.FileList.Add(media, objimage);

        //                    var postData = objParameter.GetPostDataFromParameters();

        //                    var url = FdConstants.ExtractPhotoIdUrl;

        //                    objParameter.UrlParameters.Add("av",
        //                        string.IsNullOrEmpty(av) ? account.AccountBaseModel.UserId : av);
        //                    objParameter.UrlParameters.Add("dpr", "1.5");
        //                    objParameter.UrlParameters.Add("__user", account.AccountBaseModel.UserId);
        //                    objParameter.UrlParameters.Add("__a", "1");
        //                    objParameter.UrlParameters.Add("__dyn", FdConstants.DynParameter);
        //                    objParameter.UrlParameters.Add("__req", "1k");
        //                    objParameter.UrlParameters.Add("__be", "0");
        //                    objParameter.UrlParameters.Add("fb_dtsg", fbDtsg);
        //                    objParameter.UrlParameters.Add("jazoest", FdConstants.JazoestParameterGender);

        //                    url = objParameter.GenerateUrl(url);

        //                    var request = _httpHelper.GetRequestParameter();


        //                    request.Accept = "*/*";
        //                    request.Headers[FdConstants.HostKey] = "upload.facebook.com";

        //                    request.ContentType = objParameter.ContentType;
        //                    _httpHelper.SetRequestParameter(request);
        //                    var imageUploadResponse = _httpHelper.PostRequest(url, postData);

        //                    if (imageUploadResponse.Response == null)
        //                    {
        //                        Thread.Sleep(2000);

        //                        _httpHelper.GetRequest(FdConstants.FbHomeUrl + av);

        //                        imageUploadResponse = _httpHelper.PostRequest(url, postData);
        //                    }

        //                    if (imageUploadResponse.Response.Contains("\"imageSrc\":\"") && imageUploadResponse.Response.Contains("\"photoID\":\""))
        //                    {
        //                        mediaId = FdRegexUtility.FirstMatchExtractor(imageUploadResponse.Response, FdConstants.PhotoIdRegex);
        //                        //mediaId = FdRegexUtility.FirstMatchExtractor(imageUploadResponse, FdConstants.PhotoIdRegex);
        //                    }
        //                    mediaDictionary.Add(mediaId, "Image");

        //                    isUploaded = true;
        //                }
        //                catch (Exception ex)
        //                {
        //                    isUploaded = false;
        //                    GlobusLogHelper.log.Info(Log.UploadingMediaFailed, account.AccountBaseModel.AccountNetwork, account.AccountBaseModel.UserName, media);
        //                    GlobusLogHelper.log.Info(Log.UploadingMediaFailedReason, account.AccountBaseModel.AccountNetwork, account.AccountBaseModel.UserName, ex.Message);
        //                }
        //            }
        //            if (isUploaded)
        //            {
        //                GlobusLogHelper.log.Info(Log.UploadingMediaSuccessful, account.AccountBaseModel.AccountNetwork, account.AccountBaseModel.UserName, media);
        //            }
        //        }
        //        return mediaDictionary;
        //    }
        //    catch (Exception ex)
        //    {
        //        ex.DebugLog();
        //    }
        //    return new Dictionary<string, string>();
        //}

        private Dictionary<string, string> UploadImageAndGetMediaId
       (PublisherPostlistModel postDetails, DominatorAccountModel account,
       string targetId, string waterfallId, string postSource, string av = "", string type = "")
        {
            try
            {

                var mediaId = string.Empty;

                av = FdFunctions.GetIntegerOnlyString(av);

                var fbDtsg = Uri.UnescapeDataString(account.SessionId);

                List<string> fbSupportedVideoFormat = new List<string> { "gif", "3g2", "3gp", "3gpp", "asf", "avi", "dat", "divx", "dv", "f4v", "flv", "m2ts", "m4v", "mkv", "mod", "mov", "mp4", "mpe", "mpeg", "mpeg4", "mpg", "mts", "nsv", "ogm", "ogv", "qt", "tod", "ts", "vob", "wmv" };

                Dictionary<string, string> mediaDictionary = new Dictionary<string, string>();

                foreach (var media in postDetails.MediaList)
                {
                    bool isUploaded;

                    GlobusLogHelper.log.Info(Log.UploadingMedia, account.AccountBaseModel.AccountNetwork, account.AccountBaseModel.UserName, media);

                    FdRequestParameter objParameter = new FdRequestParameter();

                    string fileExtension = FdFunctions.ReversegetBetween(media + "$", "$", ".");

                    if (postDetails.PostSource == DominatorHouseCore.Enums.SocioPublisher.PostSource.ScrapedPost)
                        fileExtension = fileExtension.Split('?')[0];


                    if (fbSupportedVideoFormat.Contains(fileExtension))
                    {
                        try
                        {
                            NameValueCollection nvc = new NameValueCollection();

                            byte[] buffer;

                            if (!File.Exists(media))
                                buffer = MediaUtilites.GetImageBytesFromUrl(media);
                            else
                            {
                                var br = new BinaryReader(new FileStream(media, FileMode.Open, FileAccess.Read));
                                buffer = br.ReadBytes((int)br.BaseStream.Length);
                            }

                            long length = buffer.Length;

                            objParameter.PostDataParameters.Add("file_size", length.ToString());
                            objParameter.PostDataParameters.Add("file_extension", fileExtension);
                            objParameter.PostDataParameters.Add("target_id", targetId);

                            objParameter.UrlParameters.Add("source", type == "Event"
                                ? "event_cover_video"
                                : "composer");

                            objParameter.PostDataParameters.Add("composer_dialog_version", "");
                            objParameter.PostDataParameters.Add("waterfall_id", waterfallId);

                            if (postSource == "Pages")
                                objParameter.PostDataParameters.Add("composer_entry_point_ref", "pages_feed");
                            else if (postSource == "OwnWall")
                                objParameter.PostDataParameters.Add("composer_entry_point_ref", "timeline");
                            else if (postSource == "Groups")
                                objParameter.PostDataParameters.Add("composer_entry_point_ref", "group");

                            objParameter.PostDataParameters.Add("has_file_been_replaced", "false");
                            objParameter.PostDataParameters.Add("supports_chunking", "true");
                            objParameter.PostDataParameters.Add("supports_file_api", "true");
                            objParameter.PostDataParameters.Add("partition_start_offset", "0");
                            objParameter.PostDataParameters.Add("partition_end_offset", length.ToString());
                            objParameter.PostDataParameters.Add("creator_product", "2");
                            objParameter.PostDataParameters.Add("spherical", "false");
                            objParameter.PostDataParameters.Add("__req", "1k");

                            objParameter = CommonPostDataParameters(account, objParameter);

                            string url = FdConstants.VideoUploadUrl;

                            objParameter.UrlParameters.Add("av",
                                string.IsNullOrEmpty(av) || av == "0" ? account.AccountBaseModel.UserId : av);
                            objParameter.UrlParameters.Add("dpr", "1.5");
                            objParameter.UrlParameters.Add("__a", "1");

                            url = objParameter.GenerateUrl(url);

                            byte[] postData = objParameter.GetPostDataFromParameters();

                            var request = _httpHelper.GetRequestParameter();

                            request.ContentType = FdConstants.ContentType;

                            request.Headers["X_FB_VIDEO_WATERFALL_ID"] = waterfallId;

                            _httpHelper.SetRequestParameter(request);

                            var videoUploadResponse = _httpHelper.PostRequest(url, postData);

                            string videoId = FdRegexUtility.FirstMatchExtractor(videoUploadResponse.Response, FdConstants.VideoIdScrapeRegex);


                            var filenameArray = Regex.Split(media, @"\\");

                            string fileName = filenameArray[filenameArray.Length - 1];

                            objParameter.IsMultiPart = true;
                            objParameter.IsMultiPartVideo = true;

                            objParameter.IsMultiPartVideo = true;

                            FileData objimage = new FileData(nvc, fileName, buffer);

                            objParameter.FileList.Add(media, objimage);

                            postData = objParameter.GetPostDataFromParameters();

                            url = FdConstants.ExtractVideoId;

                            objParameter.UrlParameters.Clear();

                            objParameter.UrlParameters.Add("av",
                                string.IsNullOrEmpty(av) ? account.AccountBaseModel.UserId : av);
                            objParameter.UrlParameters.Add("dpr", "1");
                            objParameter.UrlParameters.Add("video_id", videoId);
                            objParameter.UrlParameters.Add("start_offset", "0");
                            objParameter.UrlParameters.Add("source", type == "Event" ? "event_cover_video" : "composer");
                            objParameter.UrlParameters.Add("target_id", targetId);
                            objParameter.UrlParameters.Add("waterfall_id", waterfallId);
                            objParameter.UrlParameters.Add("composer_entry_point_ref", "");
                            objParameter.UrlParameters.Add("supports_chunking", "true");
                            objParameter.UrlParameters.Add("upload_speed", "");
                            objParameter.UrlParameters.Add("partition_start_offset", "0");
                            objParameter.UrlParameters.Add("partition_end_offset", buffer.Length.ToString());
                            objParameter.UrlParameters.Add("__req", "1k");
                            CommonUrlParameters(account, objParameter);

                            url = objParameter.GenerateUrl(url);

                            request = _httpHelper.GetRequestParameter();

                            request.Accept = "*/*";
                            request.Headers[FdConstants.HostKey] = "upload.facebook.com";

                            request.ContentType = objParameter.ContentType;
                            _httpHelper.SetRequestParameter(request);
                            _httpHelper.PostRequest(url, postData);

                            mediaDictionary.Add(videoId, "Video");

                            isUploaded = true;
                        }
                        catch (Exception ex)
                        {
                            isUploaded = false;
                            GlobusLogHelper.log.Info(Log.UploadingMediaFailed, account.AccountBaseModel.AccountNetwork, account.AccountBaseModel.UserName, media);
                            GlobusLogHelper.log.Info(Log.UploadingMediaFailedReason, account.AccountBaseModel.AccountNetwork, account.AccountBaseModel.UserName, ex.Message);
                        }
                    }
                    else
                    {
                        try
                        {
                            byte[] buffer;

                            if (!File.Exists(media) || ImageExtracter.IsValidUrl(media))
                            {
                                var newMedia = media.Replace("||", string.Empty);
                                if (newMedia.Contains("https://external") && newMedia.Contains("url="))
                                {
                                    newMedia = Utilities.GetBetween(newMedia, "url=", "&");
                                    newMedia = Uri.UnescapeDataString(newMedia);
                                }
                                buffer = MediaUtilites.GetImageBytesFromUrl(newMedia);
                            }
                            else
                            {
                                var br = new BinaryReader(new FileStream(media, FileMode.Open, FileAccess.Read));
                                buffer = br.ReadBytes((int)br.BaseStream.Length);
                            }

                            objParameter.IsMultiPart = true;

                            var filenameArray = Regex.Split(media, @"\\");

                            string fileName = filenameArray[filenameArray.Length - 1];

                            NameValueCollection nvc = new NameValueCollection
                            {
                                {"fb_dtsg", fbDtsg},
                                {"qn", waterfallId},
                                {"target_id", targetId},
                                {"source", "8"},
                                {"profile_id", account.AccountBaseModel.UserId},
                                {"waterfallxapp", "web_react_composer"}
                            };



                            FileData objimage = new FileData(nvc, fileName, buffer);

                            objParameter.FileList.Add(media, objimage);

                            var postData = objParameter.GetPostDataFromParameters();

                            var url = FdConstants.ExtractPhotoIdUrl;

                            objParameter.UrlParameters.Add("av",
                                string.IsNullOrEmpty(av) || av == "0" ? account.AccountBaseModel.UserId : av);
                            objParameter.UrlParameters.Add("dpr", "1.5");
                            objParameter.UrlParameters.Add("__user", account.AccountBaseModel.UserId);
                            objParameter.UrlParameters.Add("__a", "1");
                            objParameter.UrlParameters.Add("__dyn", FdConstants.DynParameter);
                            objParameter.UrlParameters.Add("__req", "1k");
                            objParameter.UrlParameters.Add("__be", "0");
                            objParameter.UrlParameters.Add("fb_dtsg", fbDtsg);
                            objParameter.UrlParameters.Add("jazoest", FdConstants.JazoestParameterGender);

                            url = objParameter.GenerateUrl(url);

                            var request = _httpHelper.GetRequestParameter();


                            request.Accept = "*/*";
                            request.Headers[FdConstants.HostKey] = "upload.facebook.com";

                            request.ContentType = objParameter.ContentType;
                            _httpHelper.SetRequestParameter(request);
                            var imageUploadResponse = _httpHelper.PostRequest(url, postData);

                            if (imageUploadResponse.Response == null)
                            {
                                _delayService.ThreadSleep(2000);

                                _httpHelper.GetRequest(FdConstants.FbHomeUrl + av);

                                imageUploadResponse = _httpHelper.PostRequest(url, postData);
                            }

                            if (imageUploadResponse.Response.Contains("\"imageSrc\":\"") && imageUploadResponse.Response.Contains("\"photoID\":\""))
                            {
                                mediaId = FdRegexUtility.FirstMatchExtractor(imageUploadResponse.Response, FdConstants.PhotoIdRegex);
                                //mediaId = FdRegexUtility.FirstMatchExtractor(imageUploadResponse, FdConstants.PhotoIdRegex);
                            }
                            mediaDictionary.Add(mediaId, "Image");

                            isUploaded = true;
                        }
                        catch (Exception ex)
                        {
                            isUploaded = false;
                            GlobusLogHelper.log.Info(Log.UploadingMediaFailed, account.AccountBaseModel.AccountNetwork, account.AccountBaseModel.UserName, media);
                            GlobusLogHelper.log.Info(Log.UploadingMediaFailedReason, account.AccountBaseModel.AccountNetwork, account.AccountBaseModel.UserName, ex.Message);
                        }
                    }
                    if (isUploaded)
                        GlobusLogHelper.log.Info(Log.UploadingMediaSuccessful, account.AccountBaseModel.AccountNetwork, account.AccountBaseModel.UserName, media);

                }
                return mediaDictionary;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return new Dictionary<string, string>();
        }


        private string UploadImageAndGetMediaIdForMessage
            (DominatorAccountModel account, List<string> mediaList)
        {

            try
            {

                var mediaId = string.Empty;

                //long length = 0;

                // ReSharper disable once CollectionNeverQueried.Local
                Dictionary<string, string> mediaDictionary = new Dictionary<string, string>();

                foreach (var media in mediaList)
                {
                    bool isUploaded;

                    GlobusLogHelper.log.Info(Log.UploadingMedia, account.AccountBaseModel.AccountNetwork, account.AccountBaseModel.UserName, media);

                    FdRequestParameter objParameter = new FdRequestParameter();

                    try
                    {
                        byte[] buffer;

                        if (!File.Exists(media))
                            buffer = MediaUtilites.GetImageBytesFromUrl(media);
                        else
                        {
                            var br = new BinaryReader(new FileStream(media, FileMode.Open, FileAccess.Read));
                            buffer = br.ReadBytes((int)br.BaseStream.Length);
                        }

                        objParameter.IsMultiPart = true;
                        objParameter.IsMultiPartForMessage = true;

                        var filenameArray = Regex.Split(media, @"\\");

                        string fileName = filenameArray[filenameArray.Length - 1];

                        NameValueCollection nvc = new NameValueCollection { { "attach_id", "" }, { "images_only", "true" } };


                        FileData objimage = new FileData(nvc, fileName, buffer);

                        objParameter.FileList.Add(media, objimage);

                        byte[] postData = objParameter.GetPostDataFromParameters();

                        var url = FdConstants.ExtractPhotoIdUrlForMessage;

                        objParameter.UrlParameters.Add("dpr", "1");
                        objParameter.UrlParameters.Add("__req", "1k");

                        objParameter = CommonUrlParameters(account, objParameter);

                        url = objParameter.GenerateUrl(url);

                        url += "&ft[tn]=%2BM";

                        var request = _httpHelper.GetRequestParameter();


                        request.Accept = "*/*";
                        request.Headers[FdConstants.HostKey] = "upload.facebook.com";

                        request.ContentType = objParameter.ContentType;
                        _httpHelper.SetRequestParameter(request);
                        var imageUploadResponse = _httpHelper.PostRequest(url, postData).Response;

                        if (imageUploadResponse.Contains("src\":\""))
                        {
                            mediaId = FdRegexUtility.FirstMatchExtractor(imageUploadResponse, FdConstants.MediaIdModRegex);
                            //mediaId = FdRegexUtility.FirstMatchExtractor(imageUploadResponse, "\"photoID\":\"(.*?)\"");
                        }
                        mediaDictionary.Add(mediaId, "Image");

                        isUploaded = true;
                    }
                    catch (Exception ex)
                    {
                        isUploaded = false;
                        GlobusLogHelper.log.Info(Log.UploadingMediaFailed, account.AccountBaseModel.AccountNetwork, account.AccountBaseModel.UserName, media);
                        GlobusLogHelper.log.Info(Log.UploadingMediaFailedReason, account.AccountBaseModel.AccountNetwork, account.AccountBaseModel.UserName, ex.Message);
                    }
                    if (isUploaded)
                        GlobusLogHelper.log.Info(Log.UploadingMediaSuccessful, account.AccountBaseModel.AccountNetwork, account.AccountBaseModel.UserName, media);

                }
                return mediaId;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return string.Empty;
        }

        private async Task<List<string>> UploadImageAndGetMediaIdForMessageAsyc
            (DominatorAccountModel account, List<string> mediaList)
        {

            try
            {

                var mediaId = string.Empty;

                //long length = 0;

                // ReSharper disable once CollectionNeverQueried.Local
                Dictionary<string, string> mediaDictionary = new Dictionary<string, string>();

                foreach (var media in mediaList)
                {
                    bool isUploaded;

                    GlobusLogHelper.log.Info(Log.UploadingMedia, account.AccountBaseModel.AccountNetwork, account.AccountBaseModel.UserName, media);

                    FdRequestParameter objParameter = new FdRequestParameter();

                    try
                    {
                        byte[] buffer;

                        if (!File.Exists(media))
                            buffer = MediaUtilites.GetImageBytesFromUrl(media);
                        else
                        {
                            var br = new BinaryReader(new FileStream(media, FileMode.Open, FileAccess.Read));
                            buffer = br.ReadBytes((int)br.BaseStream.Length);
                        }

                        objParameter.IsMultiPart = true;
                        objParameter.IsMultiPartForMessage = true;

                        var filenameArray = Regex.Split(media, @"\\");

                        string fileName = filenameArray[filenameArray.Length - 1];

                        NameValueCollection nvc = new NameValueCollection { { "attach_id", "" }, { "images_only", "true" } };


                        FileData objimage = new FileData(nvc, fileName, buffer);

                        objParameter.FileList.Add(media, objimage);

                        byte[] postData = objParameter.GetPostDataFromParameters();

                        var url = FdConstants.ExtractPhotoIdUrlForMessage;

                        objParameter.UrlParameters.Add("dpr", "1");
                        objParameter.UrlParameters.Add("__req", "1k");

                        objParameter = CommonUrlParameters(account, objParameter);

                        url = objParameter.GenerateUrl(url);

                        url += "&ft[tn]=%2BM";

                        var request = _httpHelper.GetRequestParameter();


                        request.Accept = "*/*";
                        request.Headers[FdConstants.HostKey] = "upload.facebook.com";

                        request.ContentType = objParameter.ContentType;
                        _httpHelper.SetRequestParameter(request);
                        var imageUploadResponse = await _httpHelper.PostRequestAsync(url, postData, account.Token);

                        if (imageUploadResponse.Response.Contains("src\":\""))
                        {
                            mediaId = FdRegexUtility.FirstMatchExtractor(imageUploadResponse.Response, FdConstants.MediaIdModRegex);
                            //mediaId = FdRegexUtility.FirstMatchExtractor(imageUploadResponse, "\"photoID\":\"(.*?)\"");
                        }
                        mediaDictionary.Add(mediaId, "Image");

                        isUploaded = true;
                    }
                    catch (Exception ex)
                    {
                        isUploaded = false;
                        GlobusLogHelper.log.Info(Log.UploadingMediaFailed, account.AccountBaseModel.AccountNetwork, account.AccountBaseModel.UserName, media);
                        GlobusLogHelper.log.Info(Log.UploadingMediaFailedReason, account.AccountBaseModel.AccountNetwork, account.AccountBaseModel.UserName, ex.Message);
                    }
                    if (isUploaded)
                        GlobusLogHelper.log.Info(Log.UploadingMediaSuccessful, account.AccountBaseModel.AccountNetwork, account.AccountBaseModel.UserName, media);

                }
                return mediaDictionary.Keys.ToList();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return new List<string>();
        }


        private List<string> TagFriends(FacebookModel advanceSettingsModel,
            DominatorAccountModel account)
        {
            try
            {
                int count = 0;
                FdFunctions objFdFunctions = new FdFunctions(account);

                List<string> randomFriends = new List<string>();

                if (advanceSettingsModel.IsTagOptionChecked)
                {
                    if (advanceSettingsModel.IsTagSpecificFriends)
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage, account.AccountBaseModel.AccountNetwork,
                            account.AccountBaseModel.UserName, "", "LangKeyStartTagFriend".FromResourceDictionary());

                        int noOfTags = advanceSettingsModel.UsersForEachPost.GetRandom();

                        randomFriends = objFdFunctions.TagSpecificFriend(account, noOfTags, advanceSettingsModel);

                        randomFriends.RemoveAll(string.IsNullOrEmpty);

                        if (randomFriends.Count > 0)
                            GlobusLogHelper.log.Info(Log.CustomMessage, account.AccountBaseModel.AccountNetwork,
                                account.AccountBaseModel.UserName, "", "LangKeyGetFriendsCustomList".FromResourceDictionary());

                        randomFriends.ForEach(x =>
                        {
                            FbUserIdResponseHandler frienddeatils = GetFriendUserId(account, x);
                            advanceSettingsModel.ListCustomTaggedUser.Remove(x);

                            advanceSettingsModel.SelectFriendsDetailsModel.AccountFriendsPair.Add(new KeyValuePair<string, string>(account.AccountId, $"{FdConstants.FbHomeUrl}{frienddeatils.UserId}"));

                            _delayService.ThreadSleep(1000);
                        });

                        randomFriends = advanceSettingsModel.SelectFriendsDetailsModel.AccountFriendsPair.Where(x => x.Key == account.AccountId).Select(x => x.Value).ToList();

                        if (randomFriends.Count == 0)
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage, account.AccountBaseModel.AccountNetwork,
                                account.AccountBaseModel.UserName, "", "LangKeyNoFriendsToTag".FromResourceDictionary());
                        }
                        randomFriends = randomFriends.OrderBy(x => new Random().Next()).Take(noOfTags).ToList();
                    }

                    else if (advanceSettingsModel.IsAutoTagFriends)
                    {
                        int noOfTags = advanceSettingsModel.UsersForEachPost.GetRandom();

                        randomFriends = objFdFunctions.GetRandomFriends(account, noOfTags, advanceSettingsModel);

                    }


                    FdRequestParameter objParameter = new FdRequestParameter();

                    string url = FdConstants.TagFriendsUrl;

                    objParameter.UrlParameters.Add("dpr", "1");

                    url = objParameter.GenerateUrl(url);

                    foreach (var friendid in randomFriends)
                    {
                        var friendIdNew = FdFunctions.GetIntegerOnlyString(friendid);
                        objParameter.PostDataParameters.Add($"people[{count}]", friendIdNew);
                        count++;
                    }

                    objParameter.PostDataParameters.Add("iconPickerOpened", "false");
                    objParameter.PostDataParameters.Add("__req", "4w");

                    objParameter = CommonPostDataParameters(account, objParameter);

                    var postData = objParameter.GetPostDataFromParameters();

                    var request = _httpHelper.GetRequestParameter();

                    request.ContentType = FdConstants.ContentType;

                    _httpHelper.SetRequestParameter(request);

                    var tagFriendsResponse = _httpHelper.PostRequest(url, postData);

                    if (!tagFriendsResponse.Response.Contains("metadataFragment"))
                    {
                        _delayService.ThreadSleep(1000);
                        tagFriendsResponse = _httpHelper.PostRequest(url, postData);
                    }

                    if (tagFriendsResponse.Response.Contains("metadataFragment"))
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage, account.AccountBaseModel.AccountNetwork,
                            account.AccountBaseModel.UserName, "", string.Format("LangKeyTaggedFriendsForPost".FromResourceDictionary(), $"{randomFriends.Count}"));
                        _delayService.ThreadSleep(2000);
                        return randomFriends;
                    }
                    else
                        GlobusLogHelper.log.Info(Log.CustomMessage, account.AccountBaseModel.AccountNetwork,
                            account.AccountBaseModel.UserName, "", "LangKeyFailedToTagFriendsForPost".FromResourceDictionary());
                }

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return new List<string>();
        }

        private List<string> MentionUsers(FacebookModel advanceSettingsModel, DominatorAccountModel account)
        {
            try
            {
                FdFunctions objFdFunctions = new FdFunctions(account);

                if (advanceSettingsModel.IsMentionSpecificFriends)
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, account.AccountBaseModel.AccountNetwork,
                        account.AccountBaseModel.UserName, "", "LangKeyStartTagFriend".FromResourceDictionary());

                    int noOfMentions = advanceSettingsModel.MentionUsersForEachPost.GetRandom();

                    var randomFriends = objFdFunctions.MentionSpecificFriendFroPost(account, noOfMentions, advanceSettingsModel);

                    randomFriends.RemoveAll(string.IsNullOrEmpty);

                    if (randomFriends.Count > 0)
                        GlobusLogHelper.log.Info(Log.CustomMessage, account.AccountBaseModel.AccountNetwork,
                            account.AccountBaseModel.UserName, "", "LangKeyGetFriendsCustomList".FromResourceDictionary());

                    randomFriends.ForEach(x =>
                    {
                        var friendId = GetFriendUserId(account, x);
                        advanceSettingsModel.ListCustomMentionUser.Remove(x);
                        advanceSettingsModel.SelectFriendsDetailsModelForMention.AccountFriendsPair.Add(new KeyValuePair<string, string>(account.AccountId, $"{FdConstants.FbHomeUrl}{friendId}"));
                        _delayService.ThreadSleep(1000);
                    });

                    randomFriends = advanceSettingsModel.SelectFriendsDetailsModelForMention.AccountFriendsPair.Where(x => x.Key == account.AccountId).Select(x => x.Value).ToList();

                    if (randomFriends.Count == 0)
                        GlobusLogHelper.log.Info(Log.CustomMessage, account.AccountBaseModel.AccountNetwork,
                            account.AccountBaseModel.UserName, "", "LangKeyNoFriendsToTag".FromResourceDictionary());

                    return randomFriends.OrderBy(x => new Random().Next()).Take(noOfMentions).ToList();
                }

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return new List<string>();
        }



        public string GetLocationDetails(DominatorAccountModel account, String location, string composerId)
        {
            FdRequestParameter objParameter = new FdRequestParameter();
            objParameter.UrlParameters.Add("dpr", "1.5");
            objParameter.UrlParameters.Add("request_id", "581026e9-e0c4-435b-bf07-e7cdf3f3f46b");
            objParameter.UrlParameters.Add("value", location);
            objParameter.UrlParameters.Add("existing_ids", "");
            objParameter.UrlParameters.Add("sid", "55188899676");
            objParameter.UrlParameters.Add("include_likes", "false");
            objParameter.UrlParameters.Add("include_subtext", "true");
            objParameter.UrlParameters.Add("include_address", "1");
            objParameter.UrlParameters.Add("exact_match", "false");
            objParameter.UrlParameters.Add("map_height", "");
            objParameter.UrlParameters.Add("map_width", "");
            objParameter.UrlParameters.Add("use_unicorn", "true");
            objParameter.UrlParameters.Add("allow_places", "true");
            objParameter.UrlParameters.Add("allow_cities", "true");
            objParameter.UrlParameters.Add("render_map", "false");
            objParameter.UrlParameters.Add("limit", "15");
            objParameter.UrlParameters.Add("use_searchable_entries", "true");
            objParameter.UrlParameters.Add("hide_geo", "false");
            objParameter.UrlParameters.Add("__req", "4w");

            objParameter = CommonUrlParameters(account, objParameter);

            var url = objParameter.GenerateUrl(FdConstants.LocationIdUrl);

            var locationResponse = _httpHelper.GetRequest(url);

            return FdRegexUtility.FirstMatchExtractor(locationResponse.Response, FdConstants.UniqueLocationRegx);

        }



        public bool DeletePostFromGroup(DominatorAccountModel account, string postId, string groupUrl, string ftEntIdentifier)
        {
            try
            {
                var url = groupUrl;

                var groupId = string.Empty;

                var groupPageResponse = _httpHelper.GetRequest(groupUrl);

                try
                {
                    string decodedResponse = FdFunctions.GetDecodedResponse(groupPageResponse.Response);
                    groupId = FdRegexUtility.FirstMatchExtractor(decodedResponse, FdConstants.GroupIdRegex);

                    if (string.IsNullOrEmpty(groupId))
                        groupId = Utilities.GetBetween(groupPageResponse.Response, "groups%2F", "%2F");

                    groupId = FdFunctions.GetIntegerOnlyString(groupId);
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
                if (string.IsNullOrEmpty(groupId))
                {
                    try
                    {
                        groupUrl = url.Split('?')[0];

                        groupUrl = !groupUrl.Contains("/about/")
                            ? groupUrl + "about/"
                            : groupUrl + "/about/";

                        //if (!groupUrl.Contains("/about/"))
                        //{
                        //    if (groupUrl[groupUrl.Length - 1] == '/')
                        //    {
                        //        groupUrl = groupUrl + "about/";
                        //    }
                        //    else
                        //    {
                        //        groupUrl = groupUrl + "/about/";
                        //    }
                        //}
                        groupPageResponse = _httpHelper.GetRequest(groupUrl);

                        groupId = FdRegexUtility.FirstMatchExtractor(groupPageResponse.Response, FdConstants.GroupIdRegex);
                        groupId = FdFunctions.GetIntegerOnlyString(groupId);
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                }


                FdRequestParameter objParameter = new FdRequestParameter();

                url = FdConstants.DeleteFromGroupUrl;

                objParameter.UrlParameters.Add("dpr", "1");

                url = objParameter.GenerateUrl(url);

                var splitPostId = Regex.Split(postId, "/");

                postId = splitPostId[splitPostId.Length - 1];

                if (!string.IsNullOrEmpty(ftEntIdentifier) && ftEntIdentifier != postId)
                    postId = ftEntIdentifier;


                objParameter.PostDataParameters.Add("surface", "group_post_chevron");
                objParameter.PostDataParameters.Add("story_dom_id", "u_4p_0");
                objParameter.PostDataParameters.Add("post_id", postId);
                objParameter.PostDataParameters.Add("nctr[_mod]", "pagelet_group_");
                objParameter.PostDataParameters.Add("location", "2");
                objParameter.PostDataParameters.Add("group_id", groupId);
                objParameter.PostDataParameters.Add("ft[top_level_post_id]", postId);
                objParameter.PostDataParameters.Add("ft[tn]", "-R-R");
                objParameter.PostDataParameters.Add("ft[fbfeed_location]", "2");
                objParameter.PostDataParameters.Add("confirmed", "1");
                objParameter.PostDataParameters.Add("__req", "4w");

                objParameter = CommonPostDataParameters(account, objParameter);


                var postData = objParameter.GetPostDataFromParameters();

                var request = _httpHelper.GetRequestParameter();

                request.ContentType = FdConstants.ContentType;

                _httpHelper.SetRequestParameter(request);

                _httpHelper.PostRequest(url, postData);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return false;
        }

        public bool DeletePostFromTimeline(DominatorAccountModel account, string postId, PostDeletionModel postDeletionModel, string ftEntIdentifier)
        {
            try
            {
                var entityId = string.Empty;
                FdRequestParameter objParameter = new FdRequestParameter();

                if (postDeletionModel.DestinationType == "Wall")
                    entityId = account.AccountBaseModel.UserId;

                if (postDeletionModel.DestinationType == "Pages")
                {
                    var fanPageResponse = _httpHelper.GetRequest(postDeletionModel.DestinationUrl);

                    entityId = FdRegexUtility.FirstMatchExtractor(fanPageResponse.Response, FdConstants.PageIdRegex);
                }

                if (postDeletionModel.DestinationType == "Friend")
                {
                    var friendResponse = _httpHelper.GetRequest(postDeletionModel.DestinationUrl);

                    entityId = FdRegexUtility.FirstMatchExtractor(friendResponse.Response, FdConstants.ProfileIdRegex);
                }

                string nodeId = string.Empty;
                if (!string.IsNullOrEmpty(entityId))
                {
                    var profileResponse = _httpHelper.GetRequest(FdConstants.FbHomeUrl + entityId);
                    nodeId = new PostDeleteNodeIdResponseHandler(profileResponse).ToString();
                }


                var splitPostId = Regex.Split(postId, "/");

                postId = splitPostId[splitPostId.Length - 1];

                if (!string.IsNullOrEmpty(ftEntIdentifier) && ftEntIdentifier != postId)
                    postId = ftEntIdentifier;


                string identifier = $"S:_I{entityId}:{postId}";

                //identifier = Uri.EscapeDataString(identifier);

                objParameter.UrlParameters.Add("identifier", identifier);
                objParameter.UrlParameters.Add("location", "9");
                objParameter.UrlParameters.Add("story_dom_id", nodeId);
                objParameter.UrlParameters.Add("render_location", "10");
                objParameter.UrlParameters.Add("is_notification_preview", "0");
                objParameter.UrlParameters.Add("dpr", "1");

                string url = FdConstants.DeleteFromPageUrl;

                url = objParameter.GenerateUrl(url);

                objParameter.PostDataParameters.Add("__req", "1i");
                objParameter = CommonPostDataParameters(account, objParameter);

                var postData = objParameter.GetPostDataFromParameters();

                var request = _httpHelper.GetRequestParameter();

                request.ContentType = FdConstants.ContentType;
                _httpHelper.SetRequestParameter(request);

                _httpHelper.PostRequest(url, postData);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return false;
        }

        public bool StopCommenting(DominatorAccountModel account, string postId, string ftEntIdentifier)
        {
            try
            {
                var splitPostId = Regex.Split(postId, "/");

                postId = splitPostId[splitPostId.Length - 1];

                if (!string.IsNullOrEmpty(ftEntIdentifier) && ftEntIdentifier != postId)
                    postId = ftEntIdentifier;

                FdRequestParameter objParameter = new FdRequestParameter();

                string url = FdConstants.StopCommentingUrl;

                objParameter.UrlParameters.Add("dpr", "1");

                url = objParameter.GenerateUrl(url);

                objParameter.PostDataParameters.Add("ft_ent_identifier", postId);
                objParameter.PostDataParameters.Add("disable_comments", "1");
                objParameter.PostDataParameters.Add("__req", "4w");

                objParameter = CommonPostDataParameters(account, objParameter);

                var postData = objParameter.GetPostDataFromParameters();

                var request = _httpHelper.GetRequestParameter();

                request.ContentType = FdConstants.ContentType;

                _httpHelper.SetRequestParameter(request);

                var tagFriendsResponse = _httpHelper.PostRequest(url, postData);

                if (tagFriendsResponse.Response.Contains("cancomment\":"))
                {
                    var status = FdRegexUtility.FirstMatchExtractor(tagFriendsResponse.Response, FdConstants.CanCommentRegx);

                    return status == "false";
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return false;
        }

        public bool StopNotification(DominatorAccountModel account, string postId, string ftEntIdentifier)
        {
            try
            {

                var splitPostId = Regex.Split(postId, "/");

                postId = splitPostId[splitPostId.Length - 1];

                if (!string.IsNullOrEmpty(ftEntIdentifier) && ftEntIdentifier != postId)
                    postId = ftEntIdentifier;

                FdRequestParameter objParameter = new FdRequestParameter();

                string url = FdConstants.StopNotificationUrl;

                objParameter.UrlParameters.Add("dpr", "2");

                url = objParameter.GenerateUrl(url);

                objParameter.PostDataParameters.Add("message_id", postId);
                objParameter.PostDataParameters.Add("follow", "0");
                objParameter.PostDataParameters.Add("__req", "4w");

                objParameter = CommonPostDataParameters(account, objParameter);

                var postData = objParameter.GetPostDataFromParameters();

                var request = _httpHelper.GetRequestParameter();

                request.ContentType = FdConstants.ContentType;

                _httpHelper.SetRequestParameter(request);

                var tagFriendsResponse = _httpHelper.PostRequest(url, postData);

                return !tagFriendsResponse.Response.Contains("error");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return false;
            }

        }

        public bool DeleteStoryPost(DominatorAccountModel account, string postId, string storyId)
        {
            FdRequestParameter objParameter = new FdRequestParameter();

            objParameter.PostDataParameters.Add("queries",
                FdConstants.FbDeleteStoryQueries(account.AccountBaseModel.UserId, storyId));
            objParameter.PostDataParameters.Add("__req", "1t");
            objParameter = CommonPostDataParameters(account, objParameter);

            var postData = objParameter.GetPostDataFromParameters();

            var request = _httpHelper.GetRequestParameter();

            request.ContentType = FdConstants.ContentType;

            _httpHelper.SetRequestParameter(request);

            var tagFriendsResponse = _httpHelper.PostRequest(FdConstants.GetMessagesUrl, postData);

            return !tagFriendsResponse.Response.Contains("deleted_story_thread_ids");

        }


        public bool HidePostFromOwnPage(DominatorAccountModel account, string postId, string pageUrl)
        {
            var fanPageResponse = _httpHelper.GetRequest(pageUrl);

            var entityId = FdRegexUtility.FirstMatchExtractor(fanPageResponse.Response, FdConstants.PageIdRegex);

            FdRequestParameter objParameter = new FdRequestParameter();

            objParameter.PostDataParameters.Clear();

            objParameter.PostDataParameters.Add("av", entityId);
            objParameter.PostDataParameters.Add("options_button_id", "u_0_22");
            objParameter.PostDataParameters.Add("story_location", "page");
            objParameter.PostDataParameters.Add("initial_action_name", "HIDE_FROM_TIMELINE");
            objParameter.PostDataParameters.Add("hideable_token", "MzYzszQyNDEwNjc0szA3q3PNKwkuSSwpLXYuSk0syczPCy7JL6qsqzMytDC2NDexsDA1szQys6yrM6gDAA");
            objParameter.PostDataParameters.Add("story_permalink_token", $"S:_I{entityId}:{postId}");
            objParameter.PostDataParameters.Add("nctr[_mod]", "pagelet_timeline_main_column");
            objParameter.PostDataParameters.Add("__req", "4w");

            objParameter = CommonPostDataParameters(account, objParameter);

            objParameter.PostDataParameters.Add("confirmed", "1");

            var url = FdConstants.HidePostFromPageUrl;

            var postData = objParameter.GetPostDataFromParameters();

            var request = _httpHelper.GetRequestParameter();

            request.ContentType = FdConstants.ContentType;

            _httpHelper.SetRequestParameter(request);

            var tagFriendsResponse = _httpHelper.PostRequest(url, postData);

            return !tagFriendsResponse.Response.Contains("error");
        }

        //public string GetPostEntIdentifier(DominatorAccountModel account, string postUrl)
        //{
        //    try
        //    {
        //        var response = _httpHelper.GetRequest(postUrl);
        //        var splitResponse = Regex.Split(response.Response, "ft_ent_identifier").Skip(1).ToArray();
        //        return Utilities.GetBetween(splitResponse[0], "value=\"", "\"");
        //    }
        //    catch (Exception ex)
        //    {
        //        ex.DebugLog();
        //        return string.Empty;
        //    }

        //}

        public bool StopNotificationGroups(DominatorAccountModel account, string postId, string groupId, string ftEntIdentifier)
        {
            try
            {

                var splitPostId = Regex.Split(postId, "/");

                postId = splitPostId[splitPostId.Length - 1];

                if (!string.IsNullOrEmpty(ftEntIdentifier) && ftEntIdentifier != postId)
                    postId = ftEntIdentifier;

                var splitgroupId = Regex.Split(groupId, "/");

                groupId = splitgroupId[splitgroupId.Length - 1];

                FdRequestParameter objParameter = new FdRequestParameter();

                string url = FdConstants.StopNotificationUrl;


                objParameter.UrlParameters.Add("dpr", "2");


                url = objParameter.GenerateUrl(url);


                objParameter.PostDataParameters.Add("group_id", groupId);
                objParameter.PostDataParameters.Add("message_id", postId);
                objParameter.PostDataParameters.Add("follow", "0");
                objParameter.PostDataParameters.Add("__req", "4w");

                objParameter = CommonPostDataParameters(account, objParameter);

                var postData = objParameter.GetPostDataFromParameters();

                var request = _httpHelper.GetRequestParameter();

                request.ContentType = FdConstants.ContentType;

                _httpHelper.SetRequestParameter(request);

                var tagFriendsResponse = _httpHelper.PostRequest(url, postData);

                if (!tagFriendsResponse.Response.Contains("error"))
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return false;
        }


        public string GetCurrencyDetails(DominatorAccountModel account, string groupId, string composerId)
        {
            try
            {

                FdRequestParameter objParameter = new FdRequestParameter();

                string url = FdConstants.BuySellPostComposerUrl;


                objParameter.PostDataParameters.Add("composer_id", composerId);
                objParameter.PostDataParameters.Add("composer_type", "group");
                objParameter.PostDataParameters.Add("target_id", groupId);
                objParameter.PostDataParameters.Add("av", account.AccountBaseModel.UserId);
                objParameter.UrlParameters.Add("dpr", "1");

                url = objParameter.GenerateUrl(url);

                objParameter.PostDataParameters.Add("__req", "4w");

                objParameter = CommonPostDataParameters(account, objParameter);

                var postData = objParameter.GetPostDataFromParameters();

                var request = _httpHelper.GetRequestParameter();

                request.ContentType = FdConstants.ContentType;

                _httpHelper.SetRequestParameter(request);

                var tagFriendsResponse = _httpHelper.PostRequest(url, postData);

                var currency = FdRegexUtility.FirstMatchExtractor(tagFriendsResponse.Response, FdConstants.CurrencyRegx);

                return currency;

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return string.Empty;
        }

        #endregion

        #region Share Posts

        public PublisherResponseHandler ShareToPages(DominatorAccountModel account,
            string pageUrl, PublisherPostlistModel postDetails,
            CancellationTokenSource campaignCancellationToken, GeneralModel generalSettingsModel,
            FacebookModel advanceSettingsModel)
        {
            try
            {
                int count = 0;

                var pageId = string.Empty;

                campaignCancellationToken.Token.ThrowIfCancellationRequested();

                var objFdFunctions = new FdFunctions(account);

                var postAsId = string.Empty;

                var homePageResponse = _httpHelper.GetRequest(FdConstants.FbHomeUrl).Response;

                var decodedResponse = FdFunctions.GetDecodedResponse(homePageResponse);

                var appId = FdRegexUtility.FirstMatchExtractor(decodedResponse, FdConstants.MessangerAppIdRegx);

                if (string.IsNullOrEmpty(appId))
                    appId = FdRegexUtility.FirstMatchExtractor(decodedResponse, FdConstants.MessangerAppIdModRegx);

                if (!string.IsNullOrEmpty(appId) && !string.IsNullOrEmpty(postDetails.ShareUrl))
                {

                    campaignCancellationToken.Token.ThrowIfCancellationRequested();

                    pageId = GetPageIdFromUrl(account, pageUrl);

                    var sharePostPopupUrl = $"{FdConstants.FbHomeUrl}dialog/share?app_id=" + appId + "&display=popup&href=" + Uri.EscapeDataString(postDetails.ShareUrl);

                    var popupResponse = _httpHelper.GetRequest(sharePostPopupUrl).Response;

                    var objectUrl = "{\"object\":\"" + postDetails.ShareUrl + "\"}";

                    var shareActionTypeId = string.Empty;
                    var shareprivacyx = string.Empty;
                    var taggerSessionId = string.Empty;

                    var splitShareActionTypeId = Regex.Split(popupResponse, "share_action_type_id");

                    if (splitShareActionTypeId.Length > 1)
                    {
                        shareActionTypeId = FdRegexUtility.FirstMatchExtractor(splitShareActionTypeId[1], "value=\"(.*?)\"");
                        ////shareActionTypeId = FdRegexUtility.FirstMatchExtractor(splitShareActionTypeId[1], "value=\"(.*?)\"");
                    }

                    var splitprivacyx = Regex.Split(popupResponse, "privacy_option_0_").Skip(1).ToArray();

                    if (splitprivacyx.Length > 1)
                    {
                        shareprivacyx = Regex.Split(splitprivacyx.FirstOrDefault(x => x.Contains("Public")), "\"")[0];

                        //shareprivacyx = FdRegexUtility.FirstMatchExtractor(splitprivacyx[1], "value=\"(.*?)\"");
                    }


                    var splittaggerSessionId = Regex.Split(popupResponse, "tagger_session_id");
                    if (splittaggerSessionId.Length > 1)
                    {
                        taggerSessionId = FdRegexUtility.FirstMatchExtractor(splittaggerSessionId[1], "value=\"(.*?)\"");
                        //taggerSessionId = FdRegexUtility.FirstMatchExtractor(splittaggerSessionId[1], "value=\"(.*?)\"");
                    }


                    #region Tag Friends

                    if (advanceSettingsModel.IsPostAsPage)
                    {
                        var randomPages = objFdFunctions.GetRandomPageActor(account, 1, advanceSettingsModel);

                        randomPages.ForEach(x =>
                        {
                            var pageIdNew = GetPageIdFromUrl(account, x);
                            advanceSettingsModel.ListCustomPageUrl.Remove(x);
                            advanceSettingsModel.SelectPageDetailsModel.AccountPagesBoardsPair.Add(new KeyValuePair<string, string>(account.AccountId, $"{FdConstants.FbHomeUrl}{pageIdNew}"));
                            _delayService.ThreadSleep(1000);
                        });

                        randomPages = advanceSettingsModel.SelectPageDetailsModel.AccountPagesBoardsPair.Where(x => x.Key == account.AccountId).Select(x => x.Value).ToList();

                        Random random = new Random();

                        postAsId = randomPages.OrderBy(x => random.Next()).Take(1).FirstOrDefault();
                    }
                    else if (advanceSettingsModel.IsPostAsSamePage)
                        postAsId = pageId;
                    else
                        postAsId = account.AccountBaseModel.UserId;

                    var randomFriends = TagFriends(advanceSettingsModel, account);

                    var mentions = MentionUsers(advanceSettingsModel, account);

                    var messageText = postDetails.PostDescription;

                    var messageWithMention = GetPublisherDescrWithMentionForShare(account, ref messageText, mentions);

                    #endregion

                    if (!string.IsNullOrEmpty(shareprivacyx) && !string.IsNullOrEmpty(taggerSessionId))
                    {
                        campaignCancellationToken.Token.ThrowIfCancellationRequested();

                        var objParameter = new FdRequestParameter();
                        objParameter.PostDataParameters.Clear();
                        objParameter.PostDataParameters.Add("fb_dtsg", account.SessionId);
                        objParameter.PostDataParameters.Add("mode", "page");
                        objParameter.PostDataParameters.Add("audience_page", pageId);
                        objParameter.PostDataParameters.Add("audience_targets", pageId);
                        objParameter.PostDataParameters.Add("av", FdFunctions.GetIntegerOnlyString(postAsId));
                        objParameter.PostDataParameters.Add("app_id", appId);
                        objParameter.PostDataParameters.Add("redirect_uri", "https%3A%2F%2Fwww.facebook.com%2Fdialog%2Freturn%2Fclose");
                        objParameter.PostDataParameters.Add("display", "popup");
                        objParameter.PostDataParameters.Add("access_token", "");
                        objParameter.PostDataParameters.Add("sdk", "");
                        objParameter.PostDataParameters.Add("from_post", "1");
                        objParameter.PostDataParameters.Add("xhpc_context", "home");
                        objParameter.PostDataParameters.Add("xhpc_ismeta", "1");
                        objParameter.PostDataParameters.Add("xhpc_timeline", "");
                        objParameter.PostDataParameters.Add("xhpc_targetid", account.AccountBaseModel.UserId);
                        objParameter.PostDataParameters.Add("xhpc_publish_type", "1");
                        objParameter.PostDataParameters.Add("xhpc_message_text", Uri.EscapeDataString(messageText));
                        objParameter.PostDataParameters.Add("xhpc_message", Uri.EscapeDataString(messageWithMention));
                        objParameter.PostDataParameters.Add("quote", "");
                        objParameter.PostDataParameters.Add("is_explicit_place", "");
                        objParameter.PostDataParameters.Add("composertags_place", "");
                        objParameter.PostDataParameters.Add("composertags_place_name", "");
                        objParameter.PostDataParameters.Add("tagger_session_id", taggerSessionId);
                        objParameter.PostDataParameters.Add("action_type_id[0]", "");
                        objParameter.PostDataParameters.Add("object_str[0]", "");
                        objParameter.PostDataParameters.Add("object_id[0]", "");
                        objParameter.PostDataParameters.Add("hide_object_attachment", "0");
                        objParameter.PostDataParameters.Add("og_suggestion_mechanism", "");
                        objParameter.PostDataParameters.Add("og_suggestion_logging_data", "");
                        objParameter.PostDataParameters.Add("icon_id", "");
                        objParameter.PostDataParameters.Add("share_action_properties", Uri.EscapeDataString(objectUrl));
                        if (!string.IsNullOrEmpty(shareActionTypeId))
                            objParameter.PostDataParameters.Add("share_action_type_id", shareActionTypeId);
                        objParameter.PostDataParameters.Add("device_code", "");
                        objParameter.PostDataParameters.Add("device_shared", "");
                        objParameter.PostDataParameters.Add("ref", "");
                        objParameter.PostDataParameters.Add("media", "");
                        objParameter.PostDataParameters.Add("dialog_url", Uri.EscapeDataString(sharePostPopupUrl));
                        objParameter.PostDataParameters.Add("composertags_city", "");
                        objParameter.PostDataParameters.Add("disable_location_sharing", "false");
                        objParameter.PostDataParameters.Add("composer_predicted_city", "");
                        objParameter.PostDataParameters.Add("privacyx", shareprivacyx);
                        objParameter.PostDataParameters.Add("__CONFIRM__", "1");
                        objParameter.PostDataParameters.Add("__user", account.AccountBaseModel.UserId);
                        objParameter.PostDataParameters.Add("__a", "1");
                        objParameter.PostDataParameters.Add("__dyn", FdConstants.DynParameter);
                        objParameter.PostDataParameters.Add("__af", "j0");
                        objParameter.PostDataParameters.Add("__req", "a");
                        objParameter.PostDataParameters.Add("__be", "0");
                        objParameter.PostDataParameters.Add("jazoest", FdConstants.JazoestParameterGender);

                        foreach (var friendid in randomFriends)
                        {
                            var friendIdNew = FdFunctions.GetIntegerOnlyString(friendid);
                            objParameter.PostDataParameters.Add($"composertags_with[{count}]", friendIdNew);
                            count++;
                        }

                        var postData = objParameter.GetPostDataFromParameters();

                        var request = _httpHelper.GetRequestParameter();

                        request.ContentType = FdConstants.ContentType;

                        _httpHelper.SetRequestParameter(request);

                        campaignCancellationToken.Token.ThrowIfCancellationRequested();

                        var sharePostUrl = $"{FdConstants.FbHomeUrl}v1.0/dialog/share/submit?dpr=1";

                        var publisherResponse = _httpHelper.PostRequest(sharePostUrl, postData);

                        if (publisherResponse.Response.Contains("redirectPageTo") || publisherResponse.Response.Contains("post_fbid\":") || publisherResponse.Response.Contains("story_id\":"))
                        {

                            var url = FdFunctions.GetIntegerOnlyString(postAsId) == pageId
                                ? $"{FdConstants.FbHomeUrl}pg/{pageId}/?ref=page_internal"
                                : $"{FdConstants.FbHomeUrl}{pageId}/posts_to_page/";

                            var getPostIdResponse = _httpHelper.GetRequest(url);

                            return new PublisherResponseHandler(getPostIdResponse, pageUrl, "Page");

                        }
                        else if (publisherResponse.Response.Contains("Error"))
                        {
                            try
                            {
                                var decodedErrorResponse = FdFunctions.GetDecodedResponse(publisherResponse.Response);

                                var errorDetails = FdRegexUtility.FirstMatchExtractor(decodedErrorResponse, "error_message\":\"(.*?)\"");

                                if (errorDetails.Contains("privacy setting"))
                                {
                                    FacebookErrors errors = FacebookErrors.PostPrivacySetting;
                                    return new PublisherResponseHandler(publisherResponse, errors);
                                }
                            }
                            catch (Exception)
                            {
                                FacebookErrors errors = FacebookErrors.UnknownErrorOccurred;
                                return new PublisherResponseHandler(publisherResponse, errors);
                            }
                        }
                        else
                        {
                            //Todo :  Analysis why failed with response, if FacebookErrors enum doesn't contains then add to enum list as well.
                            FacebookErrors errors = FacebookErrors.UnknownErrorOccurred;
                            return new PublisherResponseHandler(publisherResponse, errors);
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException("Operation Cancellated!");
            }
            catch (AggregateException ae)
            {
                ae.HandleOperationCancellation();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return null;
        }



        public PublisherResponseHandler ShareToEvents(DominatorAccountModel account,
            string eventUrl, PublisherPostlistModel postDetails,
            CancellationTokenSource campaignCancellationToken, GeneralModel generalSettingsModel,
            FacebookModel advanceSettingsModel)
        {
            try
            {
                campaignCancellationToken.Token.ThrowIfCancellationRequested();

                int count = 0;

                var homePageResponse = _httpHelper.GetRequest(FdConstants.FbHomeUrl).Response;

                var decodedResponse = FdFunctions.GetDecodedResponse(homePageResponse);

                var appId = FdRegexUtility.FirstMatchExtractor(decodedResponse, FdConstants.MessangerAppIdRegx);

                if (string.IsNullOrEmpty(appId))
                {
                    appId = FdRegexUtility.FirstMatchExtractor(decodedResponse, FdConstants.MessangerAppIdModRegx);
                }

                if (!string.IsNullOrEmpty(appId) && !string.IsNullOrEmpty(postDetails.ShareUrl))
                {
                    campaignCancellationToken.Token.ThrowIfCancellationRequested();

                    var pagePageResponse = _httpHelper.GetRequest(eventUrl);
                    var decodedPageResponse = FdFunctions.GetDecodedResponse(pagePageResponse.Response);
                    var eventId = FdRegexUtility.FirstMatchExtractor(decodedPageResponse, "eventID\":\"(.*?)\"");
                    eventId = FdFunctions.GetIntegerOnlyString(eventId);


                    string eventName = FdFunctions.GetEntityName(decodedPageResponse);


                    var sharePostPopupUrl = $"{FdConstants.FbHomeUrl}dialog/share?app_id=" + appId + "&display=popup&href=" + Uri.EscapeDataString(postDetails.ShareUrl);

                    var popupResponse = _httpHelper.GetRequest(sharePostPopupUrl).Response;

                    var objectUrl = "{\"object\":\"" + postDetails.ShareUrl + "\"}";

                    var shareActionTypeId = string.Empty;
                    var shareprivacyx = string.Empty;
                    var taggerSessionId = string.Empty;

                    var splitShareActionTypeId = Regex.Split(popupResponse, "share_action_type_id");

                    if (splitShareActionTypeId.Length > 1)
                    {
                        shareActionTypeId = FdRegexUtility.FirstMatchExtractor(splitShareActionTypeId[1], "value=\"(.*?)\"");
                        //shareActionTypeId = FdRegexUtility.FirstMatchExtractor(splitShareActionTypeId[1], "value=\"(.*?)\"");
                    }

                    var splitprivacyx = Regex.Split(popupResponse, "privacy_option_0_").Skip(1).ToArray();

                    if (splitprivacyx.Length > 1)
                    {
                        shareprivacyx = Regex.Split(splitprivacyx.FirstOrDefault(x => x.Contains("Public")), "\"")[0];

                        //shareprivacyx = FdRegexUtility.FirstMatchExtractor(splitprivacyx[1], "value=\"(.*?)\"");
                    }


                    var splittaggerSessionId = Regex.Split(popupResponse, "tagger_session_id");
                    if (splittaggerSessionId.Length > 1)
                    {
                        taggerSessionId = FdRegexUtility.FirstMatchExtractor(splittaggerSessionId[1], "value=\"(.*?)\"");
                        //taggerSessionId = FdRegexUtility.FirstMatchExtractor(splittaggerSessionId[1], "value=\"(.*?)\"");
                    }

                    #region Tag Friends

                    var randomFriends = TagFriends(advanceSettingsModel, account);

                    //var mentions = MentionUsers(advanceSettingsModel, account);

                    #endregion

                    if (!string.IsNullOrEmpty(shareprivacyx) && !string.IsNullOrEmpty(taggerSessionId))
                    {
                        campaignCancellationToken.Token.ThrowIfCancellationRequested();

                        var objParameter = new FdRequestParameter();
                        objParameter.PostDataParameters.Clear();
                        objParameter.PostDataParameters.Add("fb_dtsg", account.SessionId);
                        objParameter.PostDataParameters.Add("mode", "event");
                        objParameter.PostDataParameters.Add("audience_event", Uri.EscapeDataString(eventName));
                        objParameter.PostDataParameters.Add("audience_targets", eventId);
                        objParameter.PostDataParameters.Add("av", "");
                        objParameter.PostDataParameters.Add("app_id", appId);
                        objParameter.PostDataParameters.Add("redirect_uri", "https%3A%2F%2Fwww.facebook.com%2Fdialog%2Freturn%2Fclose");
                        objParameter.PostDataParameters.Add("display", "popup");
                        objParameter.PostDataParameters.Add("access_token", "");
                        objParameter.PostDataParameters.Add("sdk", "");
                        objParameter.PostDataParameters.Add("from_post", "1");
                        objParameter.PostDataParameters.Add("xhpc_context", "home");
                        objParameter.PostDataParameters.Add("xhpc_ismeta", "1");
                        objParameter.PostDataParameters.Add("xhpc_timeline", "");
                        objParameter.PostDataParameters.Add("xhpc_targetid", account.AccountBaseModel.UserId);
                        objParameter.PostDataParameters.Add("xhpc_publish_type", "1");
                        objParameter.PostDataParameters.Add("xhpc_message_text", Uri.EscapeDataString(postDetails.PostDescription));
                        objParameter.PostDataParameters.Add("xhpc_message", Uri.EscapeDataString(postDetails.PostDescription));
                        objParameter.PostDataParameters.Add("quote", "");
                        objParameter.PostDataParameters.Add("is_explicit_place", "");
                        objParameter.PostDataParameters.Add("composertags_place", "");
                        objParameter.PostDataParameters.Add("composertags_place_name", "");
                        objParameter.PostDataParameters.Add("tagger_session_id", taggerSessionId);
                        objParameter.PostDataParameters.Add("action_type_id[0]", "");
                        objParameter.PostDataParameters.Add("object_str[0]", "");
                        objParameter.PostDataParameters.Add("object_id[0]", "");
                        objParameter.PostDataParameters.Add("hide_object_attachment", "0");
                        objParameter.PostDataParameters.Add("og_suggestion_mechanism", "");
                        objParameter.PostDataParameters.Add("og_suggestion_logging_data", "");
                        objParameter.PostDataParameters.Add("icon_id", "");
                        objParameter.PostDataParameters.Add("share_action_properties", Uri.EscapeDataString(objectUrl));
                        if (!string.IsNullOrEmpty(shareActionTypeId))
                            objParameter.PostDataParameters.Add("share_action_type_id", shareActionTypeId);
                        objParameter.PostDataParameters.Add("device_code", "");
                        objParameter.PostDataParameters.Add("device_shared", "");
                        objParameter.PostDataParameters.Add("ref", "");
                        objParameter.PostDataParameters.Add("media", "");
                        objParameter.PostDataParameters.Add("dialog_url", Uri.EscapeDataString(sharePostPopupUrl));
                        objParameter.PostDataParameters.Add("composertags_city", "");
                        objParameter.PostDataParameters.Add("disable_location_sharing", "false");
                        objParameter.PostDataParameters.Add("composer_predicted_city", "");
                        objParameter.PostDataParameters.Add("privacyx", shareprivacyx);
                        objParameter.PostDataParameters.Add("__CONFIRM__", "1");
                        objParameter.PostDataParameters.Add("__user", account.AccountBaseModel.UserId);
                        objParameter.PostDataParameters.Add("__a", "1");
                        objParameter.PostDataParameters.Add("__dyn", FdConstants.DynParameter);
                        objParameter.PostDataParameters.Add("__af", "j0");
                        objParameter.PostDataParameters.Add("__req", "a");
                        objParameter.PostDataParameters.Add("__be", "0");
                        objParameter.PostDataParameters.Add("jazoest", FdConstants.JazoestParameterGender);

                        foreach (var friendid in randomFriends)
                        {
                            var friendIdNew = FdFunctions.GetIntegerOnlyString(friendid);
                            objParameter.PostDataParameters.Add($"composertags_with[{count}]", friendIdNew);
                            count++;
                        }

                        var postData = objParameter.GetPostDataFromParameters();

                        var request = _httpHelper.GetRequestParameter();

                        request.ContentType = FdConstants.ContentType;

                        _httpHelper.SetRequestParameter(request);

                        campaignCancellationToken.Token.ThrowIfCancellationRequested();

                        var sharePostUrl = $"{FdConstants.FbHomeUrl}v1.0/dialog/share/submit?dpr=1";

                        var publisherResponse = _httpHelper.PostRequest(sharePostUrl, postData);

                        if (publisherResponse.Response.Contains("redirectPageTo") || publisherResponse.Response.Contains("post_fbid\":") || publisherResponse.Response.Contains("story_id\":"))
                        {
                            var url = $"{FdConstants.FbHomeUrl}{account.AccountBaseModel.UserId}/allactivity?privacy_source=activity_log&log_filter=cluster_11";

                            //FdLoginProcess.RequestParameterInitialize(account);

                            var getPostIdResponse = _httpHelper.GetRequest(url);

                            var responseHandler = new PublisherResponseHandler(getPostIdResponse);

                            return responseHandler;
                        }
                        else if (publisherResponse.Response.Contains("Error"))
                        {
                            try
                            {
                                var decodedErrorResponse = FdFunctions.GetDecodedResponse(publisherResponse.Response);

                                var errorDetails = FdRegexUtility.FirstMatchExtractor(decodedErrorResponse, "error_message\":\"(.*?)\"");

                                if (errorDetails.Contains("privacy setting"))
                                {
                                    FacebookErrors errors = FacebookErrors.PostPrivacySetting;
                                    var responseHandler = new PublisherResponseHandler(publisherResponse, errors);
                                    return responseHandler;
                                }
                            }
                            catch (Exception)
                            {
                                FacebookErrors errors = FacebookErrors.UnknownErrorOccurred;
                                var responseHandler = new PublisherResponseHandler(publisherResponse, errors);
                                return responseHandler;
                            }
                        }
                        else
                        {
                            //Todo :  Analysis why failed with response, if FacebookErrors enum doesn't contains then add to enum list as well.
                            FacebookErrors errors = FacebookErrors.UnknownErrorOccurred;
                            var responseHandler = new PublisherResponseHandler(publisherResponse, errors);
                            return responseHandler;
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException("Operation Cancellated!");
            }
            catch (AggregateException ae)
            {
                ae.HandleOperationCancellation();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return null;
        }

        public PublisherResponseHandler ShareToFriendProfiles(DominatorAccountModel account,
            string friendUrl, PublisherPostlistModel postDetails,
            CancellationTokenSource campaignCancellationToken, GeneralModel generalSettingsModel,
            FacebookModel advanceSettingsModel)
        {
            try
            {
                campaignCancellationToken.Token.ThrowIfCancellationRequested();

                int count = 0;

                var homePageResponse = _httpHelper.GetRequest(FdConstants.FbHomeUrl).Response;

                var decodedResponse = FdFunctions.GetDecodedResponse(homePageResponse);

                var appId = FdRegexUtility.FirstMatchExtractor(decodedResponse, FdConstants.MessangerAppIdRegx);

                if (string.IsNullOrEmpty(appId))
                    appId = FdRegexUtility.FirstMatchExtractor(decodedResponse, FdConstants.MessangerAppIdModRegx);

                if (!string.IsNullOrEmpty(appId) && !string.IsNullOrEmpty(postDetails.ShareUrl))
                {
                    campaignCancellationToken.Token.ThrowIfCancellationRequested();

                    var friendPageResponse = _httpHelper.GetRequest(friendUrl);
                    var decodedPageResponse = FdFunctions.GetDecodedResponse(friendPageResponse.Response);

                    var friendId = FdRegexUtility.FirstMatchExtractor(decodedPageResponse, FdConstants.EntityIdRegex);
                    friendId = FdFunctions.GetIntegerOnlyString(friendId);

                    var friendName = string.Empty;
                    var friendNameSplit = Regex.Split(decodedPageResponse, "pageTitle");

                    if (friendNameSplit.Length > 1)
                        friendName = FdRegexUtility.FirstMatchExtractor(friendNameSplit[1], FdConstants.EntityNameRegex);

                    var sharePostPopupUrl = $"{FdConstants.FbHomeUrl}dialog/share?app_id=" + appId + "&display=popup&href=" + Uri.EscapeDataString(postDetails.ShareUrl);

                    campaignCancellationToken.Token.ThrowIfCancellationRequested();

                    var responseHandler = _httpHelper.GetRequest(sharePostPopupUrl);
                    var popupResponse = responseHandler.Response;

                    var objectUrl = "{\"object\":\"" + postDetails.ShareUrl + "\"}";

                    var shareActionTypeId = string.Empty;
                    var shareprivacyx = string.Empty;
                    var taggerSessionId = string.Empty;

                    var splitShareActionTypeId = Regex.Split(popupResponse, "share_action_type_id");

                    if (splitShareActionTypeId.Length > 1)
                    {
                        shareActionTypeId = FdRegexUtility.FirstMatchExtractor(splitShareActionTypeId[1], "value=\"(.*?)\"");
                        //shareActionTypeId = FdRegexUtility.FirstMatchExtractor(splitShareActionTypeId[1], "value=\"(.*?)\"");
                    }

                    var splitprivacyx = Regex.Split(popupResponse, "privacy_option_0_").Skip(1).ToArray();

                    if (splitprivacyx.Length > 1)
                    {
                        shareprivacyx = Regex.Split(splitprivacyx.FirstOrDefault(x => x.Contains("Public")), "\"")[0];

                        //shareprivacyx = FdRegexUtility.FirstMatchExtractor(splitprivacyx[1], "value=\"(.*?)\"");
                    }


                    var splittaggerSessionId = Regex.Split(popupResponse, "tagger_session_id");
                    if (splittaggerSessionId.Length > 1)
                    {
                        taggerSessionId = FdRegexUtility.FirstMatchExtractor(splittaggerSessionId[1], "value=\"(.*?)\"");
                        //taggerSessionId = FdRegexUtility.FirstMatchExtractor(splittaggerSessionId[1], "value=\"(.*?)\"");
                    }

                    #region Tag Friends

                    var randomFriends = TagFriends(advanceSettingsModel, account);

                    var mentions = MentionUsers(advanceSettingsModel, account);

                    var messageText = postDetails.PostDescription;

                    var messageWithMention = GetPublisherDescrWithMentionForShare(account, ref messageText, mentions);


                    #endregion

                    if (!string.IsNullOrEmpty(shareprivacyx) &&
                        !string.IsNullOrEmpty(taggerSessionId))
                    {
                        var objParameter = new FdRequestParameter();
                        objParameter.PostDataParameters.Clear();
                        objParameter.PostDataParameters.Add("fb_dtsg", account.SessionId);
                        objParameter.PostDataParameters.Add("mode", "friend");
                        objParameter.PostDataParameters.Add("friendTarget", Uri.EscapeDataString(friendName));
                        objParameter.PostDataParameters.Add("audience_targets", friendId);
                        objParameter.PostDataParameters.Add("av", "");
                        objParameter.PostDataParameters.Add("app_id", appId);
                        objParameter.PostDataParameters.Add("redirect_uri",
                            "https%3A%2F%2Fwww.facebook.com%2Fdialog%2Freturn%2Fclose");
                        objParameter.PostDataParameters.Add("display", "popup");
                        objParameter.PostDataParameters.Add("access_token", "");
                        objParameter.PostDataParameters.Add("sdk", "");
                        objParameter.PostDataParameters.Add("from_post", "1");
                        objParameter.PostDataParameters.Add("xhpc_context", "home");
                        objParameter.PostDataParameters.Add("xhpc_ismeta", "1");
                        objParameter.PostDataParameters.Add("xhpc_timeline", "");
                        objParameter.PostDataParameters.Add("xhpc_targetid", account.AccountBaseModel.UserId);
                        objParameter.PostDataParameters.Add("xhpc_publish_type", "1");
                        objParameter.PostDataParameters.Add("xhpc_message_text",
                            Uri.EscapeDataString(messageText));
                        objParameter.PostDataParameters.Add("xhpc_message",
                            Uri.EscapeDataString(messageWithMention));
                        objParameter.PostDataParameters.Add("quote", "");
                        objParameter.PostDataParameters.Add("is_explicit_place", "");
                        objParameter.PostDataParameters.Add("composertags_place", "");
                        objParameter.PostDataParameters.Add("composertags_place_name", "");
                        objParameter.PostDataParameters.Add("tagger_session_id", taggerSessionId);
                        objParameter.PostDataParameters.Add("action_type_id[0]", "");
                        objParameter.PostDataParameters.Add("object_str[0]", "");
                        objParameter.PostDataParameters.Add("object_id[0]", "");
                        objParameter.PostDataParameters.Add("hide_object_attachment", "0");
                        objParameter.PostDataParameters.Add("og_suggestion_mechanism", "");
                        objParameter.PostDataParameters.Add("og_suggestion_logging_data", "");
                        objParameter.PostDataParameters.Add("icon_id", "");
                        objParameter.PostDataParameters.Add("share_action_properties", Uri.EscapeDataString(objectUrl));
                        if (!string.IsNullOrEmpty(shareActionTypeId))
                            objParameter.PostDataParameters.Add("share_action_type_id", shareActionTypeId);
                        objParameter.PostDataParameters.Add("device_code", "");
                        objParameter.PostDataParameters.Add("device_shared", "");
                        objParameter.PostDataParameters.Add("ref", "");
                        objParameter.PostDataParameters.Add("media", "");
                        objParameter.PostDataParameters.Add("dialog_url", Uri.EscapeDataString(sharePostPopupUrl));
                        objParameter.PostDataParameters.Add("composertags_city", "");
                        objParameter.PostDataParameters.Add("disable_location_sharing", "false");
                        objParameter.PostDataParameters.Add("composer_predicted_city", "");
                        objParameter.PostDataParameters.Add("privacyx", shareprivacyx);
                        objParameter.PostDataParameters.Add("__CONFIRM__", "1");
                        objParameter.PostDataParameters.Add("__user", account.AccountBaseModel.UserId);
                        objParameter.PostDataParameters.Add("__a", "1");
                        objParameter.PostDataParameters.Add("__dyn", FdConstants.DynParameter);
                        objParameter.PostDataParameters.Add("__af", "j0");
                        objParameter.PostDataParameters.Add("__req", "a");
                        objParameter.PostDataParameters.Add("__be", "0");
                        objParameter.PostDataParameters.Add("jazoest", FdConstants.JazoestParameterGender);

                        foreach (var friendid in randomFriends)
                        {
                            var friendIdNew = FdFunctions.GetIntegerOnlyString(friendid);
                            objParameter.PostDataParameters.Add($"composertags_with[{count}]", friendIdNew);
                            count++;
                        }

                        var postData = objParameter.GetPostDataFromParameters();

                        var request = _httpHelper.GetRequestParameter();

                        request.ContentType = FdConstants.ContentType;

                        _httpHelper.SetRequestParameter(request);

                        var sharePostUrl = $"{FdConstants.FbHomeUrl}v1.0/dialog/share/submit?dpr=1";

                        campaignCancellationToken.Token.ThrowIfCancellationRequested();

                        var publisherResponse = _httpHelper.PostRequest(sharePostUrl, postData);

                        if (publisherResponse.Response.Contains("redirectPageTo") ||
                            publisherResponse.Response.Contains("post_fbid\":") ||
                            publisherResponse.Response.Contains("story_id\":"))
                        {
                            var url =
                                $"{FdConstants.FbHomeUrl}{account.AccountBaseModel.UserId}/allactivity?privacy_source=activity_log&log_filter=cluster_11";

                            //FdLoginProcess.RequestParameterInitialize(account);

                            var getPostIdResponse = _httpHelper.GetRequest(url);

                            var publisherResponseHandler = new PublisherResponseHandler(getPostIdResponse);

                            return publisherResponseHandler;
                        }
                        else if (publisherResponse.Response.Contains("Error"))
                        {
                            try
                            {
                                var decodedErrorResponse = FdFunctions.GetDecodedResponse(publisherResponse.Response);

                                var errorDetails =
                                    FdRegexUtility.FirstMatchExtractor(decodedErrorResponse, "error_message\":\"(.*?)\"");

                                if (errorDetails.Contains("privacy setting"))
                                {
                                    FacebookErrors errors = FacebookErrors.PostPrivacySetting;
                                    var publisherResponseHandler = new PublisherResponseHandler(publisherResponse, errors);
                                    return publisherResponseHandler;
                                }
                            }
                            catch (Exception)
                            {
                                FacebookErrors errors = FacebookErrors.UnknownErrorOccurred;
                                var publisherResponseHandler = new PublisherResponseHandler(publisherResponse, errors);
                                return publisherResponseHandler;
                            }
                        }
                        else
                        {
                            //Todo :  Analysis why failed with response, if FacebookErrors enum doesn't contains then add to enum list as well.
                            FacebookErrors errors = FacebookErrors.UnknownErrorOccurred;
                            var publisherResponseHandler = new PublisherResponseHandler(publisherResponse, errors);
                            return publisherResponseHandler;
                        }
                    }
                    else
                    {
                        FacebookErrors errors = FacebookErrors.PostPrivacySetting;
                        var publisherResponseHandler = new PublisherResponseHandler(responseHandler, errors);
                        return publisherResponseHandler;
                    }
                }
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException("Operation Cancellated!");
            }
            catch (AggregateException ae)
            {
                ae.HandleOperationCancellation();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }



            return null;
        }

        public PublisherResponseHandler ShareToOwnWall(DominatorAccountModel account, PublisherPostlistModel postDetails,
            CancellationTokenSource campaignCancellationToken, GeneralModel generalSettingsModel,
            FacebookModel advanceSettingsModel)
        {

            try
            {
                campaignCancellationToken.Token.ThrowIfCancellationRequested();

                int count = 0;

                var homePageResponse = _httpHelper.GetRequest(FdConstants.FbHomeUrl).Response;

                var decodedResponse = FdFunctions.GetDecodedResponse(homePageResponse);

                var appId = FdRegexUtility.FirstMatchExtractor(decodedResponse, FdConstants.MessangerAppIdRegx);

                if (string.IsNullOrEmpty(appId))
                {
                    appId = FdRegexUtility.FirstMatchExtractor(decodedResponse, FdConstants.MessangerAppIdModRegx);
                }

                if (!string.IsNullOrEmpty(appId) && !string.IsNullOrEmpty(postDetails.ShareUrl))
                {


                    campaignCancellationToken.Token.ThrowIfCancellationRequested();

                    var sharePostPopupUrl = $"{FdConstants.FbHomeUrl}dialog/share?app_id=" + appId + "&display=popup&href=" + Uri.EscapeDataString(postDetails.ShareUrl);

                    var popupResponse = _httpHelper.GetRequest(sharePostPopupUrl).Response;

                    var objectUrl = "{\"object\":\"" + postDetails.ShareUrl + "\"}";

                    var shareActionTypeId = string.Empty;
                    var shareprivacyx = string.Empty;
                    var taggerSessionId = string.Empty;

                    var splitShareActionTypeId = Regex.Split(popupResponse, "share_action_type_id");

                    if (splitShareActionTypeId.Length > 1)
                    {
                        shareActionTypeId = FdRegexUtility.FirstMatchExtractor(splitShareActionTypeId[1], "value=\"(.*?)\"");
                        ////shareActionTypeId = FdRegexUtility.FirstMatchExtractor(splitShareActionTypeId[1], "value=\"(.*?)\"");
                    }

                    var splitprivacyx = Regex.Split(popupResponse, "privacy_option_0_").Skip(1).ToArray();

                    if (splitprivacyx.Length > 1)
                    {
                        shareprivacyx = Regex.Split(splitprivacyx.FirstOrDefault(x => x.Contains("Public")), "\"")[0];

                        //shareprivacyx = FdRegexUtility.FirstMatchExtractor(splitprivacyx[1], "value=\"(.*?)\"");
                    }


                    var splittaggerSessionId = Regex.Split(popupResponse, "tagger_session_id");
                    if (splittaggerSessionId.Length > 1)
                    {
                        taggerSessionId = FdRegexUtility.FirstMatchExtractor(splittaggerSessionId[1], "value=\"(.*?)\"");

                        //taggerSessionId = FdRegexUtility.FirstMatchExtractor(splittaggerSessionId[1], "value=\"(.*?)\"");
                    }

                    #region Tag Friends

                    var randomFriends = TagFriends(advanceSettingsModel, account);

                    var mentions = MentionUsers(advanceSettingsModel, account);

                    var messageText = postDetails.PostDescription;

                    var messageWithMention = GetPublisherDescrWithMentionForShare(account, ref messageText, mentions);


                    #endregion


                    if (!string.IsNullOrEmpty(shareprivacyx) && !string.IsNullOrEmpty(taggerSessionId))
                    {

                        var storyDetails = GetLastStoryDetails(account);

                        campaignCancellationToken.Token.ThrowIfCancellationRequested();

                        var objParameter = new FdRequestParameter();
                        objParameter.PostDataParameters.Clear();
                        objParameter.PostDataParameters.Add("fb_dtsg", account.SessionId);
                        objParameter.PostDataParameters.Add("mode", "self");
                        objParameter.PostDataParameters.Add("audience_targets", account.AccountBaseModel.UserId);
                        objParameter.PostDataParameters.Add("av", "");
                        objParameter.PostDataParameters.Add("app_id", appId);
                        objParameter.PostDataParameters.Add("redirect_uri", "https%3A%2F%2Fwww.facebook.com%2Fdialog%2Freturn%2Fclose");
                        objParameter.PostDataParameters.Add("display", "popup");
                        objParameter.PostDataParameters.Add("access_token", "");
                        objParameter.PostDataParameters.Add("sdk", "");
                        objParameter.PostDataParameters.Add("from_post", "1");
                        objParameter.PostDataParameters.Add("xhpc_context", "home");
                        objParameter.PostDataParameters.Add("xhpc_ismeta", "1");
                        objParameter.PostDataParameters.Add("xhpc_timeline", "");
                        objParameter.PostDataParameters.Add("xhpc_targetid", account.AccountBaseModel.UserId);
                        objParameter.PostDataParameters.Add("xhpc_publish_type", "1");
                        objParameter.PostDataParameters.Add("xhpc_message_text", Uri.EscapeDataString(messageText));
                        objParameter.PostDataParameters.Add("xhpc_message", Uri.EscapeDataString(messageWithMention));
                        objParameter.PostDataParameters.Add("quote", "");
                        objParameter.PostDataParameters.Add("is_explicit_place", "");
                        objParameter.PostDataParameters.Add("composertags_place", "");
                        objParameter.PostDataParameters.Add("composertags_place_name", "");
                        objParameter.PostDataParameters.Add("tagger_session_id", taggerSessionId);
                        objParameter.PostDataParameters.Add("action_type_id[0]", "");
                        objParameter.PostDataParameters.Add("object_str[0]", "");
                        objParameter.PostDataParameters.Add("object_id[0]", "");
                        objParameter.PostDataParameters.Add("hide_object_attachment", "0");
                        objParameter.PostDataParameters.Add("og_suggestion_mechanism", "");
                        objParameter.PostDataParameters.Add("og_suggestion_logging_data", "");
                        objParameter.PostDataParameters.Add("icon_id", "");
                        objParameter.PostDataParameters.Add("share_action_properties", Uri.EscapeDataString(objectUrl));
                        if (!string.IsNullOrEmpty(shareActionTypeId))
                            objParameter.PostDataParameters.Add("share_action_type_id", shareActionTypeId);
                        objParameter.PostDataParameters.Add("device_code", "");
                        objParameter.PostDataParameters.Add("device_shared", "");
                        objParameter.PostDataParameters.Add("ref", "");
                        objParameter.PostDataParameters.Add("media", "");
                        objParameter.PostDataParameters.Add("dialog_url", Uri.EscapeDataString(sharePostPopupUrl));
                        objParameter.PostDataParameters.Add("composertags_city", "");
                        objParameter.PostDataParameters.Add("disable_location_sharing", "false");
                        objParameter.PostDataParameters.Add("composer_predicted_city", "");
                        objParameter.PostDataParameters.Add("privacyx", shareprivacyx);

                        objParameter.PostDataParameters.Add(
                            postDetails.FdPostSettings.IsPostAsStoryPost ? "stories_selector" : "feed_selector", "on");

                        //if (postDetails.FdPostSettings.IsPostAsStoryPost)
                        //    objParameter.PostDataParameters.Add("stories_selector", "on");
                        //else
                        //    objParameter.PostDataParameters.Add("feed_selector", "on");

                        objParameter.PostDataParameters.Add("__CONFIRM__", "1");
                        objParameter.PostDataParameters.Add("__user", account.AccountBaseModel.UserId);
                        objParameter.PostDataParameters.Add("__a", "1");
                        objParameter.PostDataParameters.Add("__dyn", FdConstants.DynParameter);
                        objParameter.PostDataParameters.Add("__af", "j0");
                        objParameter.PostDataParameters.Add("__req", "a");
                        objParameter.PostDataParameters.Add("__be", "0");
                        objParameter.PostDataParameters.Add("jazoest", FdConstants.JazoestParameterGender);


                        foreach (var friendid in randomFriends)
                        {
                            objParameter.PostDataParameters.Add($"composertags_with[{count}]", friendid);
                            count++;
                        }

                        var postData = objParameter.GetPostDataFromParameters();

                        var request = _httpHelper.GetRequestParameter();

                        request.ContentType = FdConstants.ContentType;

                        _httpHelper.SetRequestParameter(request);

                        campaignCancellationToken.Token.ThrowIfCancellationRequested();

                        var sharePostUrl = $"{FdConstants.FbHomeUrl}v1.0/dialog/share/submit?dpr=1";

                        var publisherResponse = _httpHelper.PostRequest(sharePostUrl, postData);

                        if (publisherResponse.Response.Contains("redirectPageTo") && postDetails.FdPostSettings.IsPostAsStoryPost)
                        {
                            _delayService.ThreadSleep(2000);
                            var currentStoryDetails = GetLastStoryDetails(account);

                            if (currentStoryDetails.ListOwnStories.Count > storyDetails.ListOwnStories.Count
                                && currentStoryDetails.ListOwnStories.LastOrDefault().PostedDateTime > storyDetails.ListOwnStories.LastOrDefault().PostedDateTime)
                            {
                                var storyId = currentStoryDetails.ListOwnStories.LastOrDefault().Id;
                                return new PublisherResponseHandler(publisherResponse, storyId);
                            }
                        }
                        else if (publisherResponse.Response.Contains("redirectPageTo") || publisherResponse.Response.Contains("post_fbid\":") || publisherResponse.Response.Contains("story_id\":"))
                        {

                            var url = $"{FdConstants.FbHomeUrl}{account.AccountBaseModel.UserId}/allactivity?privacy_source=activity_log&log_filter=cluster_11";

                            //FdLoginProcess.RequestParameterInitialize(account);

                            var getPostIdResponse = _httpHelper.GetRequest(url);

                            var responseHandler = new PublisherResponseHandler(getPostIdResponse);

                            return responseHandler;
                        }
                        else if (publisherResponse.Response.Contains("Error"))
                        {
                            try
                            {
                                var decodedErrorResponse = FdFunctions.GetDecodedResponse(publisherResponse.Response);

                                var errorDetails = FdRegexUtility.FirstMatchExtractor(decodedErrorResponse, "error_message\":\"(.*?)\"");

                                if (errorDetails.Contains("privacy setting"))
                                {
                                    FacebookErrors errors = FacebookErrors.PostPrivacySetting;
                                    var responseHandler = new PublisherResponseHandler(publisherResponse, errors);
                                    return responseHandler;
                                }
                            }
                            catch (Exception)
                            {
                                FacebookErrors errors = FacebookErrors.UnknownErrorOccurred;
                                var responseHandler = new PublisherResponseHandler(publisherResponse, errors);
                                return responseHandler;
                            }
                        }
                        else
                        {
                            //Todo :  Analysis why failed with response, if FacebookErrors enum doesn't contains then add to enum list as well.
                            FacebookErrors errors = FacebookErrors.UnknownErrorOccurred;
                            var responseHandler = new PublisherResponseHandler(publisherResponse, errors);
                            return responseHandler;
                        }

                    }
                }
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException("Operation Cancellated!");
            }
            catch (AggregateException ae)
            {
                ae.HandleOperationCancellation();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return null;
        }

        private GetStoryDetailsResponseHandler GetLastStoryDetails(DominatorAccountModel account)
        {
            var requestParamater = new FdRequestParameter();

            requestParamater.PostDataParameters.Add("__req", "j");
            requestParamater.PostDataParameters.Add("dpr", "1");
            requestParamater.PostDataParameters.Add("queries", FdConstants.FbGetStoryQueries);

            CommonPostDataParameters(account, requestParamater);

            var postData = requestParamater.GetPostDataFromParameters();

            var request = _httpHelper.GetRequestParameter();

            request.ContentType = FdConstants.ContentType;

            _httpHelper.SetRequestParameter(request);

            var ownStoryResponse = _httpHelper.PostRequest(FdConstants.GetMessagesUrl, postData);

            return new GetStoryDetailsResponseHandler(ownStoryResponse);
        }

        public PublisherResponseHandler ShareToGroups(DominatorAccountModel account,
            string groupUrl, PublisherPostlistModel postDetails,
            CancellationTokenSource campaignCancellationToken, GeneralModel generalSettingsModel,
            FacebookModel advanceSettingsModel)
        {
            try
            {
                campaignCancellationToken.Token.ThrowIfCancellationRequested();

                int count = 0;


                var homePageResponse = _httpHelper.GetRequest(FdConstants.FbHomeUrl).Response;

                var decodedResponse = FdFunctions.GetDecodedResponse(homePageResponse);

                var appId = FdRegexUtility.FirstMatchExtractor(decodedResponse, FdConstants.MessangerAppIdRegx);

                if (string.IsNullOrEmpty(appId))
                {
                    appId = FdRegexUtility.FirstMatchExtractor(decodedResponse, FdConstants.MessangerAppIdModRegx);
                }

                if (!string.IsNullOrEmpty(appId) && !string.IsNullOrEmpty(postDetails.ShareUrl))
                {
                    campaignCancellationToken.Token.ThrowIfCancellationRequested();

                    var groupPageResponse = _httpHelper.GetRequest(groupUrl);

                    var GroupType = groupPageResponse.Response.Contains("admin_activities") ? "OwnGroup" : "JoinedGroup";

                    var groupId = FdRegexUtility.FirstMatchExtractor(groupPageResponse.Response, FdConstants.GroupIdRegex);

                    groupId = FdFunctions.GetIntegerOnlyString(groupId);


                    if (string.IsNullOrEmpty(groupId))
                    {
                        if (!groupUrl.Contains("/about/") && !groupUrl.Contains("groups"))
                        {
                            groupUrl = $"{FdConstants.FbHomeUrl}groups/{FdFunctions.GetIntegerOnlyString(groupUrl)}/about";
                        }
                        else if (!groupUrl.Contains("/about/"))
                            groupUrl = groupUrl + "/about/";

                        groupPageResponse = _httpHelper.GetRequest(groupUrl);
                    }

                    var decodedgroupPageResponse = FdFunctions.GetDecodedResponse(groupPageResponse.Response);

                    var groupNameSplit = Regex.Split(decodedgroupPageResponse, "groupConfig");

                    var groupName = string.Empty;

                    if (groupNameSplit.Length > 1)
                        groupName = FdRegexUtility.FirstMatchExtractor(groupNameSplit[1], "name:\"(.*?)\"");

                    var sharePostPopupUrl = $"{FdConstants.FbHomeUrl}dialog/share?app_id=" + appId + "&display=popup&href=" + Uri.EscapeDataString(postDetails.ShareUrl);

                    var popupResponse = _httpHelper.GetRequest(sharePostPopupUrl).Response;

                    var objectUrl = "{\"object\":\"" + postDetails.ShareUrl + "\"}";

                    var shareActionTypeId = string.Empty;
                    var shareprivacyx = string.Empty;
                    var taggerSessionId = string.Empty;

                    var splitShareActionTypeId = Regex.Split(popupResponse, "share_action_type_id");

                    if (splitShareActionTypeId.Length > 1)
                    {
                        shareActionTypeId = FdRegexUtility.FirstMatchExtractor(splitShareActionTypeId[1], "value=\"(.*?)\"");
                        //shareActionTypeId = FdRegexUtility.FirstMatchExtractor(splitShareActionTypeId[1], "value=\"(.*?)\"");
                    }

                    var splitprivacyx = Regex.Split(popupResponse, "privacy_option_0_").Skip(1).ToArray();

                    if (splitprivacyx.Length > 1)
                    {
                        shareprivacyx = Regex.Split(splitprivacyx.FirstOrDefault(x => x.Contains("Public")), "\"")[0];

                        //shareprivacyx = FdRegexUtility.FirstMatchExtractor(splitprivacyx[1], "value=\"(.*?)\"");
                    }


                    var splittaggerSessionId = Regex.Split(popupResponse, "tagger_session_id");
                    if (splittaggerSessionId.Length > 1)
                    {
                        taggerSessionId = FdRegexUtility.FirstMatchExtractor(splittaggerSessionId[1], "value=\"(.*?)\"");
                        //taggerSessionId = FdRegexUtility.FirstMatchExtractor(splittaggerSessionId[1], "value=\"(.*?)\"");
                    }


                    #region Tag Friends

                    var randomFriends = TagFriends(advanceSettingsModel, account);

                    var mentions = MentionUsers(advanceSettingsModel, account);

                    var messageText = postDetails.PostDescription;

                    var messageWithMention = GetPublisherDescrWithMentionForShare(account, ref messageText, mentions);

                    #endregion

                    if (!string.IsNullOrEmpty(shareprivacyx) && !string.IsNullOrEmpty(taggerSessionId))
                    {

                        campaignCancellationToken.Token.ThrowIfCancellationRequested();

                        var objParameter = new FdRequestParameter();
                        objParameter.PostDataParameters.Clear();
                        objParameter.PostDataParameters.Add("fb_dtsg", account.SessionId);
                        objParameter.PostDataParameters.Add("mode", "group");
                        objParameter.PostDataParameters.Add("audience_group", groupName);
                        objParameter.PostDataParameters.Add("audience_targets", groupId);
                        objParameter.PostDataParameters.Add("av", account.AccountBaseModel.UserId);
                        objParameter.PostDataParameters.Add("app_id", appId);
                        objParameter.PostDataParameters.Add("redirect_uri", "https%3A%2F%2Fwww.facebook.com%2Fdialog%2Freturn%2Fclose");
                        objParameter.PostDataParameters.Add("display", "popup");
                        objParameter.PostDataParameters.Add("access_token", "");
                        objParameter.PostDataParameters.Add("sdk", "");
                        objParameter.PostDataParameters.Add("from_post", "1");
                        objParameter.PostDataParameters.Add("xhpc_context", "home");
                        objParameter.PostDataParameters.Add("xhpc_ismeta", "1");
                        objParameter.PostDataParameters.Add("xhpc_timeline", "");
                        objParameter.PostDataParameters.Add("xhpc_targetid", account.AccountBaseModel.UserId);
                        objParameter.PostDataParameters.Add("xhpc_publish_type", "1");
                        objParameter.PostDataParameters.Add("xhpc_message_text", Uri.EscapeDataString(messageText));
                        objParameter.PostDataParameters.Add("xhpc_message", Uri.EscapeDataString(messageWithMention));
                        objParameter.PostDataParameters.Add("quote", "");
                        objParameter.PostDataParameters.Add("is_explicit_place", "");
                        objParameter.PostDataParameters.Add("composertags_place", "");
                        objParameter.PostDataParameters.Add("composertags_place_name", "");
                        objParameter.PostDataParameters.Add("tagger_session_id", taggerSessionId);
                        objParameter.PostDataParameters.Add("action_type_id[0]", "");
                        objParameter.PostDataParameters.Add("object_str[0]", "");
                        objParameter.PostDataParameters.Add("object_id[0]", "");
                        objParameter.PostDataParameters.Add("hide_object_attachment", "0");
                        objParameter.PostDataParameters.Add("og_suggestion_mechanism", "");
                        objParameter.PostDataParameters.Add("og_suggestion_logging_data", "");
                        objParameter.PostDataParameters.Add("icon_id", "");
                        objParameter.PostDataParameters.Add("share_action_properties", Uri.EscapeDataString(objectUrl));
                        if (!string.IsNullOrEmpty(shareActionTypeId))
                            objParameter.PostDataParameters.Add("share_action_type_id", shareActionTypeId);
                        objParameter.PostDataParameters.Add("device_code", "");
                        objParameter.PostDataParameters.Add("device_shared", "");
                        objParameter.PostDataParameters.Add("ref", "");
                        objParameter.PostDataParameters.Add("media", "");
                        objParameter.PostDataParameters.Add("dialog_url", Uri.EscapeDataString(sharePostPopupUrl));
                        objParameter.PostDataParameters.Add("composertags_city", "");
                        objParameter.PostDataParameters.Add("disable_location_sharing", "false");
                        objParameter.PostDataParameters.Add("composer_predicted_city", "");
                        objParameter.PostDataParameters.Add("privacyx", shareprivacyx);
                        objParameter.PostDataParameters.Add("__CONFIRM__", "1");
                        objParameter.PostDataParameters.Add("__user", account.AccountBaseModel.UserId);
                        objParameter.PostDataParameters.Add("__a", "1");
                        objParameter.PostDataParameters.Add("__dyn", FdConstants.DynParameter);
                        objParameter.PostDataParameters.Add("__af", "j0");
                        objParameter.PostDataParameters.Add("__req", "a");
                        objParameter.PostDataParameters.Add("__be", "0");
                        objParameter.PostDataParameters.Add("jazoest", FdConstants.JazoestParameterGender);

                        foreach (var friendid in randomFriends)
                        {
                            objParameter.PostDataParameters.Add($"composertags_with[{count}]", friendid);
                            count++;
                        }

                        var postData = objParameter.GetPostDataFromParameters();

                        var request = _httpHelper.GetRequestParameter();

                        request.ContentType = FdConstants.ContentType;

                        _httpHelper.SetRequestParameter(request);

                        campaignCancellationToken.Token.ThrowIfCancellationRequested();

                        var publisherResponse = _httpHelper.PostRequest($"{FdConstants.FbHomeUrl}v1.0/dialog/share/submit?dpr=1", postData);

                        PublisherResponseHandler responseHandler = null;
                        if (publisherResponse.Response.Contains("redirectPageTo") || publisherResponse.Response.Contains("post_fbid\":") || publisherResponse.Response.Contains("story_id\":"))
                        {
                            _delayService.ThreadSleep(2000);
                            var url = $"{groupUrl}";
                            IResponseParameter groupResponse = null;
                            string postUrl = string.Empty;
                            if (GroupType == "OwnGroup")
                            {
                                groupResponse = _httpHelper.GetRequest(url);

                                return new PublisherResponseHandler(account, groupResponse);

                                //postUrl = FdRegexUtility.FirstMatchExtractor(groupResponse.Response, "_5pcq\" href=\"(.*?)\"");
                                //postUrl = FdRegexUtility.FirstMatchExtractor(postUrl, "permalink/(.*?)/");
                                //postUrl = string.IsNullOrEmpty(postUrl) ? string.Empty : $"{FdConstants.FbHomeUrl}{postUrl}";

                            }
                            else
                            {
                                url = $"{FdConstants.FbHomeUrl}groups/{groupId}/pending/";
                                groupResponse = _httpHelper.GetRequest(url);

                                responseHandler = new PublisherResponseHandler(account, groupResponse);

                                //postUrl = FdRegexUtility.FirstMatchExtractor(groupResponse.Response, "feed_subtitle_(.*?):");
                                //postUrl = string.IsNullOrEmpty(postUrl) ? string.Empty : $"{FdConstants.FbHomeUrl}{postUrl}";

                                postUrl = responseHandler.ObjFdScraperResponseParameters.PostDetails.PostUrl;
                                //if (string.IsNullOrEmpty(responseHandler.PostUrl))
                                //{
                                //    groupResponse = _httpHelper.GetRequest($"{FdConstants.FbHomeUrl}groups/{groupId}/");

                                //    return new PublisherResponseHandler(account, groupResponse);
                                //}
                            }

                            if (string.IsNullOrEmpty(postUrl))
                            {

                                new Exception().DebugLog("Enetered into ActivitityLog for sharetogroup");

                                url = FdConstants.ActivityLogGroupPost(account.AccountBaseModel.UserId);
                                groupResponse = _httpHelper.GetRequest(url);
                                postUrl = FdRegexUtility.FirstMatchExtractor(groupResponse.Response, "pam _5shk uiBoxWhite bottomborder(.*?)</tr>");

                                if (postUrl.Contains(groupId))
                                    postUrl = FdRegexUtility.FirstMatchExtractor(postUrl, "_5shl fss\"><a href=\"/(.*?)\"");

                                postUrl = string.IsNullOrEmpty(postUrl)
                                    ? string.Empty
                                    : $"{FdConstants.FbHomeUrl}{postUrl}";
                            }

                            responseHandler = new PublisherResponseHandler(groupResponse, postUrl);

                            return responseHandler;
                        }
                        else if (publisherResponse.Response.Contains("Error"))
                        {
                            try
                            {
                                var decodedErrorResponse = FdFunctions.GetDecodedResponse(publisherResponse.Response);

                                var errorDetails = FdRegexUtility.FirstMatchExtractor(decodedErrorResponse, "error_message\":\"(.*?)\"");

                                if (errorDetails.Contains("privacy setting"))
                                {
                                    FacebookErrors errors = FacebookErrors.PostPrivacySetting;
                                    responseHandler = new PublisherResponseHandler(publisherResponse, errors);
                                    return responseHandler;
                                }
                                else
                                {
                                    //Todo :  Analysis why failed with response, if FacebookErrors enum doesn't contains then add to enum list as well.
                                    FacebookErrors errors = FacebookErrors.UnknownErrorOccurred;
                                    responseHandler = new PublisherResponseHandler(publisherResponse, errors);
                                    return responseHandler;
                                }
                            }
                            catch (Exception)
                            {
                                FacebookErrors errors = FacebookErrors.UnknownErrorOccurred;
                                responseHandler = new PublisherResponseHandler(publisherResponse, errors);
                                return responseHandler;
                            }
                        }
                        else
                        {
                            //Todo :  Analysis why failed with response, if FacebookErrors enum doesn't contains then add to enum list as well.
                            FacebookErrors errors = FacebookErrors.UnknownErrorOccurred;
                            responseHandler = new PublisherResponseHandler(publisherResponse, errors);
                            return responseHandler;
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException("Operation Cancellated!");
            }
            catch (AggregateException ae)
            {
                ae.HandleOperationCancellation();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return null;
        }

        public PublisherResponseHandler ShareToFriendAsPrivateMessage(DominatorAccountModel account, string friendUrl, PublisherPostlistModel postDetails, CancellationTokenSource campaignCancellationToken, DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting.FacebookModel advanceSettingsModel)
        {
            try
            {
                campaignCancellationToken.Token.ThrowIfCancellationRequested();

                var homePageResponse = _httpHelper.GetRequest(FdConstants.FbHomeUrl).Response;

                var decodedResponse = FdFunctions.GetDecodedResponse(homePageResponse);

                var appId = FdRegexUtility.FirstMatchExtractor(decodedResponse, FdConstants.MessangerAppIdRegx);

                if (string.IsNullOrEmpty(appId))
                    appId = FdRegexUtility.FirstMatchExtractor(decodedResponse, FdConstants.MessangerAppIdModRegx);

                if (!string.IsNullOrEmpty(appId) && !string.IsNullOrEmpty(postDetails.ShareUrl))
                {
                    campaignCancellationToken.Token.ThrowIfCancellationRequested();

                    var friendPageResponse = _httpHelper.GetRequest(friendUrl);
                    var decodedPageResponse = FdFunctions.GetDecodedResponse(friendPageResponse.Response);

                    var friendId = FdRegexUtility.FirstMatchExtractor(decodedPageResponse, FdConstants.EntityIdRegex);
                    friendId = FdFunctions.GetIntegerOnlyString(friendId);

                    var sharePostPopupUrl = $"{FdConstants.FbHomeUrl}dialog/share?app_id=" + appId + "&display=popup&href=" + Uri.EscapeDataString(postDetails.ShareUrl);

                    var popupResponse = _httpHelper.GetRequest(sharePostPopupUrl).Response;

                    var objectUrl = "{\"object\":\"" + postDetails.ShareUrl + "\"}";

                    var shareActionTypeId = string.Empty;
                    var shareprivacyx = string.Empty;
                    var taggerSessionId = string.Empty;

                    var splitShareActionTypeId = Regex.Split(popupResponse, "share_action_type_id");

                    if (splitShareActionTypeId.Length > 1)
                    {
                        shareActionTypeId = FdRegexUtility.FirstMatchExtractor(splitShareActionTypeId[1], "value=\"(.*?)\"");
                        //shareActionTypeId = FdRegexUtility.FirstMatchExtractor(splitShareActionTypeId[1], "value=\"(.*?)\"");
                    }

                    var splitprivacyx = Regex.Split(popupResponse, "privacy_option_0_").Skip(1).ToArray();

                    if (splitprivacyx.Length > 1)
                    {
                        shareprivacyx = Regex.Split(splitprivacyx.FirstOrDefault(x => x.Contains("Public")), "\"")[0];

                        //shareprivacyx = FdRegexUtility.FirstMatchExtractor(splitprivacyx[1], "value=\"(.*?)\"");
                    }


                    var splittaggerSessionId = Regex.Split(popupResponse, "tagger_session_id");
                    if (splittaggerSessionId.Length > 1)
                    {
                        taggerSessionId = FdRegexUtility.FirstMatchExtractor(splittaggerSessionId[1], "value=\"(.*?)\"");
                        //taggerSessionId = FdRegexUtility.FirstMatchExtractor(splittaggerSessionId[1], "value=\"(.*?)\"");
                    }


                    var mentions = MentionUsers(advanceSettingsModel, account);

                    var messageText = postDetails.PostDescription;

                    var messageWithMention = GetPublisherDescrWithMentionForShare(account, ref messageText, mentions);


                    if (!string.IsNullOrEmpty(shareprivacyx) && !string.IsNullOrEmpty(taggerSessionId))
                    {

                        campaignCancellationToken.Token.ThrowIfCancellationRequested();

                        var objParameter = new FdRequestParameter();
                        objParameter.PostDataParameters.Clear();
                        objParameter.PostDataParameters.Add("fb_dtsg", account.SessionId);
                        objParameter.PostDataParameters.Add("mode", "message");
                        objParameter.PostDataParameters.Add("audience_targets", friendId);
                        objParameter.PostDataParameters.Add("av", "");
                        objParameter.PostDataParameters.Add("app_id", appId);
                        objParameter.PostDataParameters.Add("redirect_uri", "https%3A%2F%2Fwww.facebook.com%2Fdialog%2Freturn%2Fclose");
                        objParameter.PostDataParameters.Add("display", "popup");
                        objParameter.PostDataParameters.Add("access_token", "");
                        objParameter.PostDataParameters.Add("sdk", "");
                        objParameter.PostDataParameters.Add("from_post", "1");
                        objParameter.PostDataParameters.Add("xhpc_context", "home");
                        objParameter.PostDataParameters.Add("xhpc_ismeta", "1");
                        objParameter.PostDataParameters.Add("xhpc_timeline", "");
                        objParameter.PostDataParameters.Add("xhpc_targetid", account.AccountBaseModel.UserId);
                        objParameter.PostDataParameters.Add("xhpc_publish_type", "1");
                        objParameter.PostDataParameters.Add("xhpc_message_text", Uri.EscapeDataString(messageText));
                        objParameter.PostDataParameters.Add("xhpc_message", Uri.EscapeDataString(messageWithMention));
                        objParameter.PostDataParameters.Add("quote", "");
                        objParameter.PostDataParameters.Add("is_explicit_place", "");
                        objParameter.PostDataParameters.Add("composertags_place", "");
                        objParameter.PostDataParameters.Add("composertags_place_name", "");
                        objParameter.PostDataParameters.Add("tagger_session_id", taggerSessionId);
                        objParameter.PostDataParameters.Add("action_type_id[0]", "");
                        objParameter.PostDataParameters.Add("object_str[0]", "");
                        objParameter.PostDataParameters.Add("object_id[0]", "");
                        objParameter.PostDataParameters.Add("hide_object_attachment", "0");
                        objParameter.PostDataParameters.Add("og_suggestion_mechanism", "");
                        objParameter.PostDataParameters.Add("og_suggestion_logging_data", "");
                        objParameter.PostDataParameters.Add("icon_id", "");
                        objParameter.PostDataParameters.Add("share_action_properties", Uri.EscapeDataString(objectUrl));
                        objParameter.PostDataParameters.Add("share_action_type_id", shareActionTypeId);
                        objParameter.PostDataParameters.Add("device_code", "");
                        objParameter.PostDataParameters.Add("device_shared", "");
                        objParameter.PostDataParameters.Add("ref", "");
                        objParameter.PostDataParameters.Add("media", "");
                        objParameter.PostDataParameters.Add("dialog_url", Uri.EscapeDataString(sharePostPopupUrl));
                        objParameter.PostDataParameters.Add("composertags_city", "");
                        objParameter.PostDataParameters.Add("disable_location_sharing", "false");
                        objParameter.PostDataParameters.Add("composer_predicted_city", "");
                        objParameter.PostDataParameters.Add("privacyx", shareprivacyx);
                        objParameter.PostDataParameters.Add("__CONFIRM__", "1");
                        objParameter.PostDataParameters.Add("__user", account.AccountBaseModel.UserId);
                        objParameter.PostDataParameters.Add("__a", "1");
                        objParameter.PostDataParameters.Add("__dyn", FdConstants.DynParameter);
                        objParameter.PostDataParameters.Add("__af", "j0");
                        objParameter.PostDataParameters.Add("__req", "a");
                        objParameter.PostDataParameters.Add("__be", "0");
                        objParameter.PostDataParameters.Add("jazoest", FdConstants.JazoestParameterGender);

                        var postData = objParameter.GetPostDataFromParameters();

                        var request = _httpHelper.GetRequestParameter();

                        request.ContentType = FdConstants.ContentType;

                        _httpHelper.SetRequestParameter(request);

                        campaignCancellationToken.Token.ThrowIfCancellationRequested();

                        var sharePostUrl = $"{FdConstants.FbHomeUrl}v1.0/dialog/share/submit?dpr=1";

                        var publisherResponse = _httpHelper.PostRequest(sharePostUrl, postData);

                        if (publisherResponse.Response.Contains("redirectPageTo") || publisherResponse.Response.Contains("post_fbid\":") || publisherResponse.Response.Contains("story_id\":"))
                        {
                            var url = $"{FdConstants.FbHomeUrl}{account.AccountBaseModel.UserId}/allactivity?privacy_source=activity_log&log_filter=cluster_11";

                            //FdLoginProcess.RequestParameterInitialize(account);

                            var getPostIdResponse = _httpHelper.GetRequest(url);

                            var messageUrl = $"{FdConstants.FbHomeUrl}messages/t/{friendId}";

                            var responseHandler = new PublisherResponseHandler(getPostIdResponse, messageUrl);

                            return responseHandler;
                        }
                        else if (publisherResponse.Response.Contains("Error"))
                        {
                            try
                            {
                                var decodedErrorResponse = FdFunctions.GetDecodedResponse(publisherResponse.Response);

                                var errorDetails = FdRegexUtility.FirstMatchExtractor(decodedErrorResponse, "error_message\":\"(.*?)\"");

                                if (errorDetails.Contains("privacy setting"))
                                {
                                    FacebookErrors errors = FacebookErrors.PostPrivacySetting;
                                    var responseHandler = new PublisherResponseHandler(publisherResponse, errors);
                                    return responseHandler;
                                }
                            }
                            catch (Exception)
                            {
                                FacebookErrors errors = FacebookErrors.UnknownErrorOccurred;
                                var responseHandler = new PublisherResponseHandler(publisherResponse, errors);
                                return responseHandler;
                            }
                        }
                        else
                        {
                            //Todo :  Analysis why failed with response, if FacebookErrors enum doesn't contains then add to enum list as well.
                            FacebookErrors errors = FacebookErrors.UnknownErrorOccurred;
                            var responseHandler = new PublisherResponseHandler(publisherResponse, errors);
                            return responseHandler;
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException("Operation Cancellated!");
            }
            catch (AggregateException ae)
            {
                ae.HandleOperationCancellation();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return null;
        }

        //        public ScrapedLinkDetails GetLinkDetails(string response)
        //        {
        //            return new ScrapedLinkDetails();
        //        }

        #endregion

        #region FbChat

        //public IResponseHandler GetRecentFriendMessageDetails
        //   (DominatorAccountModel account, IResponseHandler responseHandler, CancellationToken token)
        //{
        //    try
        //    {
        //        MessageSenderResponseHandler messageSenderResponseHandler;

        //        if (responseHandler == null)
        //        {
        //            var messangerUrl = FdConstants.MessengerUrl;

        //            token.ThrowIfCancellationRequested();

        //            var messengerResponse = _httpHelper.GetRequest(messangerUrl);

        //            token.ThrowIfCancellationRequested();

        //            messageSenderResponseHandler = new MessageSenderResponseHandler(messengerResponse, account.AccountBaseModel.UserId, account.AccountBaseModel.AccountId);

        //        }
        //        else
        //        {
        //            MessageType messageType = MessageType.Inbox;

        //            FdRequestParameter objFdRequestParameter = new FdRequestParameter();

        //            var url = objFdRequestParameter.GenerateUrl(FdConstants.GetMessagesUrl);

        //            objFdRequestParameter.PostDataParameters.Clear();

        //            objFdRequestParameter.PostDataParameters.Add("batch_name", "MessengerGraphQLThreadlistFetcher");
        //            objFdRequestParameter.PostDataParameters.Add("__req", "11");

        //            objFdRequestParameter = CommonPostDataParameters(account, objFdRequestParameter);

        //            objFdRequestParameter.PostDataParameters.Add("queries", FdConstants.GetMessageQueries
        //                ("100", responseHandler.ObjFdScraperResponseParameters.PaginationTimestamp, messageType));


        //            var postData = objFdRequestParameter.GetPostDataFromParameters();

        //            var request = _httpHelper.GetRequestParameter();

        //            request.ContentType = FdConstants.ContentType;

        //            _httpHelper.SetRequestParameter(request);

        //            token.ThrowIfCancellationRequested();

        //            var incommingMessageResponse = _httpHelper.PostRequest(url, postData);

        //            token.ThrowIfCancellationRequested();

        //            messageSenderResponseHandler = new MessageSenderResponseHandler(incommingMessageResponse, account.AccountBaseModel.UserId, account.AccountBaseModel.AccountId);


        //            if (responseHandler.ObjFdScraperResponseParameters.PaginationTimestamp == messageSenderResponseHandler.ObjFdScraperResponseParameters.PaginationTimestamp)
        //            {
        //                messageSenderResponseHandler.HasMoreResults = false;
        //            }
        //        }

        //        return messageSenderResponseHandler;
        //    }
        //    catch (Exception ex)
        //    {
        //        ex.DebugLog();
        //    }

        //    return null;
        //}



        public IResponseHandler GetRecentMessageDetails
                (DominatorAccountModel account, IResponseHandler responseHandler, LiveChatModel liveChatModel, CancellationToken token)
        {
            try
            {
                string friendId = liveChatModel.SenderDetails.SenderId;

                string friendName = liveChatModel.SenderDetails.SenderName;

                UserMessageResponseHandler messageSenderResponseHandler;

                var homepageResponse = _httpHelper.GetRequest(FdConstants.FbHomeUrl);

                var userIdResponseHandler = new FbUserIdResponseHandler(homepageResponse);

                string fbDtsg = Uri.UnescapeDataString(userIdResponseHandler.FbDtsg);

                account.SessionId = fbDtsg;

                FdRequestParameter objFdRequestParameter = new FdRequestParameter();

                var url = objFdRequestParameter.GenerateUrl(FdConstants.GetMessagesUrl);

                if (responseHandler == null)
                {

                    objFdRequestParameter.PostDataParameters.Clear();

                    objFdRequestParameter.PostDataParameters.Add("batch_name", "MessengerGraphQLThreadlistFetcher");
                    objFdRequestParameter.PostDataParameters.Add("__req", "11");

                    objFdRequestParameter = CommonPostDataParameters(account, objFdRequestParameter);

                    objFdRequestParameter.PostDataParameters.Add("queries", FdConstants.GetMessageQueriesForUser
                        ("10", friendId));


                    var postData = objFdRequestParameter.GetPostDataFromParameters();

                    var request = _httpHelper.GetRequestParameter();

                    request.ContentType = FdConstants.ContentType;

                    _httpHelper.SetRequestParameter(request);

                    token.ThrowIfCancellationRequested();

                    var incommingMessageResponse = _httpHelper.PostRequest(url, postData);

                    token.ThrowIfCancellationRequested();

                    messageSenderResponseHandler = new UserMessageResponseHandler(incommingMessageResponse,
                        account.AccountBaseModel.UserId, account.AccountBaseModel.AccountId, friendName);

                    //messageSenderResponseHandler.ListSenderDetails = messageSenderResponseHandler.ListSenderDetails.OrderBy(x => x.LastMessegeDateTime).ToList();

                    if (messageSenderResponseHandler.ObjFdScraperResponseParameters.ListChatDetails.Count > 0)
                        messageSenderResponseHandler.PageletData = messageSenderResponseHandler
                            .ObjFdScraperResponseParameters.ListChatDetails[0].Time;
                }
                else
                {
                    objFdRequestParameter.PostDataParameters.Clear();

                    objFdRequestParameter.PostDataParameters.Add("batch_name", "MessengerGraphQLThreadlistFetcher");
                    objFdRequestParameter.PostDataParameters.Add("__user", account.AccountBaseModel.UserId);
                    objFdRequestParameter.PostDataParameters.Add("__a", "1");
                    objFdRequestParameter.PostDataParameters.Add("__dyn", FdConstants.DynParameter);
                    objFdRequestParameter.PostDataParameters.Add("__req", "11");
                    objFdRequestParameter.PostDataParameters.Add("__be", "1");
                    objFdRequestParameter.PostDataParameters.Add("fb_dtsg", fbDtsg);
                    objFdRequestParameter.PostDataParameters.Add("jazoest", FdConstants.JazoestParameterGender);
                    objFdRequestParameter.PostDataParameters.Add("queries",
                        FdConstants.GetMessageQueriesForUserPagination
                            ("100", responseHandler.PageletData, friendId));


                    var postData = objFdRequestParameter.GetPostDataFromParameters();

                    var request = _httpHelper.GetRequestParameter();

                    request.ContentType = FdConstants.ContentType;

                    _httpHelper.SetRequestParameter(request);

                    token.ThrowIfCancellationRequested();

                    var incommingMessageResponse = _httpHelper.PostRequest(url, postData);

                    token.ThrowIfCancellationRequested();

                    messageSenderResponseHandler = new UserMessageResponseHandler(incommingMessageResponse,
                        account.AccountBaseModel.UserId, account.AccountBaseModel.AccountId, friendName);

                    if (messageSenderResponseHandler.ObjFdScraperResponseParameters.ListChatDetails.Count > 0)
                        messageSenderResponseHandler.PageletData = messageSenderResponseHandler
                            .ObjFdScraperResponseParameters.ListChatDetails[0].Time;

                    if (messageSenderResponseHandler.ObjFdScraperResponseParameters.ListChatDetails.Count < 100)
                    {
                        messageSenderResponseHandler.HasMoreResults = false;
                    }

                }

                return messageSenderResponseHandler;
            }
            catch (AssertFailedException)
            {
                throw;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return null;
        }

        #endregion


        #region Scrap Guests for Events

        /// <summary>
        /// Scrap Guests Interested for Events
        /// </summary>
        /// <param name="account"></param>
        /// <param name="responseHandler"></param>
        /// <param name="eventUrl"></param>
        /// <param name="objEventGuestType"></param>
        /// <returns></returns>
        public IResponseHandler GetInterestedGuestsForEvents(DominatorAccountModel account,
          IResponseHandler responseHandler, string eventUrl, EventGuestType objEventGuestType)
        {
            string url;

            string fbDtsg = Uri.UnescapeDataString(account.SessionId);

            EventGuestsResponseHandler searchPeopleResponseHandler = null;

            FdRequestParameter objFdRequestParameter = new FdRequestParameter();

            if (responseHandler == null)
            {

                url = eventUrl;

                var eventResponse = _httpHelper.GetRequest(url);

                var eventId = FdRegexUtility.FirstMatchExtractor(eventResponse.Response, "eventid\":(.*?),");


                objFdRequestParameter.UrlParameters.Clear();

                objFdRequestParameter.UrlParameters.Add("event_id", eventId);
                objFdRequestParameter.UrlParameters.Add("tabs[0]", "watched");
                objFdRequestParameter.UrlParameters.Add("tabs[1]", "going");
                objFdRequestParameter.UrlParameters.Add("tabs[2]", "invited");
                objFdRequestParameter.UrlParameters.Add("order[declined]", "affinity");
                objFdRequestParameter.UrlParameters.Add("order[going]", "affinity");
                objFdRequestParameter.UrlParameters.Add("order[invited]", "affinity");
                objFdRequestParameter.UrlParameters.Add("order[maybe]", "affinity");
                objFdRequestParameter.UrlParameters.Add("order[watched]", "affinity");
                objFdRequestParameter.UrlParameters.Add("bucket_schema[watched]", "friends");
                objFdRequestParameter.UrlParameters.Add("bucket_schema[invited]", "friends");
                objFdRequestParameter.UrlParameters.Add("bucket_schema[going]", "friends");
                objFdRequestParameter.UrlParameters.Add("dpr", "1");
                objFdRequestParameter.UrlParameters.Add("__req", "18");

                objFdRequestParameter = CommonUrlParameters(account, objFdRequestParameter);

                url = objFdRequestParameter.GenerateUrl(FdConstants.EventGuestUrl);

                var searchPeopleResponse = _httpHelper.GetRequest(url);

                searchPeopleResponseHandler = new EventGuestsResponseHandler(searchPeopleResponse, objEventGuestType)
                {
                    PageletData = searchPeopleResponse.Response
                };

            }

            else
            {

                try
                {
                    //paginationResponseHandler.IsPagination = true;

                    objFdRequestParameter.UrlParameters.Clear();

                    objFdRequestParameter.UrlParameters.Add("event_id", responseHandler.EntityId);
                    objFdRequestParameter.UrlParameters.Add("tabs[0]", "watched");
                    objFdRequestParameter.UrlParameters.Add("order[watched]", "affinity");
                    objFdRequestParameter.UrlParameters.Add("bucket_schema[watched]", "friends");
                    objFdRequestParameter.UrlParameters.Add("cursor[watched]", responseHandler.PageletData);
                    objFdRequestParameter.UrlParameters.Add("dpr", "1");
                    objFdRequestParameter.UrlParameters.Add("fb_dtsg", fbDtsg);
                    objFdRequestParameter.UrlParameters.Add("__user", account.AccountBaseModel.UserId);
                    objFdRequestParameter.UrlParameters.Add("__a", "1");
                    objFdRequestParameter.UrlParameters.Add("__dyn", FdConstants.DynParameter);
                    objFdRequestParameter.UrlParameters.Add("__req", "32");
                    objFdRequestParameter.UrlParameters.Add("__be", "1");

                    url = objFdRequestParameter.GenerateUrl(FdConstants.FbFanpageLikerPageleUrl);

                    var paginationResponse = _httpHelper.GetRequest(url);

                    new EventGuestsResponseHandler(paginationResponse, EventGuestType.Interested).HasMoreResults
                       = !string.IsNullOrEmpty(searchPeopleResponseHandler.PageletData);

                }
                catch (Exception ex)
                {
                    ex.DebugLog(ex.Message);
                }
            }


            return searchPeopleResponseHandler;
        }


        /*internal IResponseHandler GetGoingGuestsForEvents(DominatorAccountModel account,
            IResponseHandler responseHandler, string eventUrl)
        {
            string url;

            string paginationData = string.Empty;

            string fbDtsg = Uri.UnescapeDataString(account.SessionId);

            EventGuestsResponseHandler searchPeopleResponseHandler = null;

            FdRequestParameter objFdRequestParameter = new FdRequestParameter();

            if (responseHandler == null)
            {

                url = eventUrl;

                var eventResponse = _httpHelper.GetRequest(url);

                var eventId = FdRegexUtility.FirstMatchExtractor(eventResponse.Response, "eventid\":(.*?),");


                objFdRequestParameter.UrlParameters.Clear();

                objFdRequestParameter.UrlParameters.Add("event_id", eventId);
                objFdRequestParameter.UrlParameters.Add("tabs[0]", "watched");
                objFdRequestParameter.UrlParameters.Add("tabs[1]", "going");
                objFdRequestParameter.UrlParameters.Add("tabs[2]", "invited");
                objFdRequestParameter.UrlParameters.Add("order[declined]", "affinity");
                objFdRequestParameter.UrlParameters.Add("order[going]", "affinity");
                objFdRequestParameter.UrlParameters.Add("order[invited]", "affinity");
                objFdRequestParameter.UrlParameters.Add("order[maybe]", "affinity");
                objFdRequestParameter.UrlParameters.Add("order[watched]", "affinity");
                objFdRequestParameter.UrlParameters.Add("bucket_schema[watched]", "friends");
                objFdRequestParameter.UrlParameters.Add("bucket_schema[invited]", "friends");
                objFdRequestParameter.UrlParameters.Add("bucket_schema[going]", "friends");
                objFdRequestParameter.UrlParameters.Add("dpr", "1");
                objFdRequestParameter.UrlParameters.Add("fb_dtsg", fbDtsg);
                objFdRequestParameter.UrlParameters.Add("__user", account.AccountBaseModel.UserId);
                objFdRequestParameter.UrlParameters.Add("__a", "1");
                objFdRequestParameter.UrlParameters.Add("__dyn", FdConstants.DynParameter);
                objFdRequestParameter.UrlParameters.Add("__req", "18");
                objFdRequestParameter.UrlParameters.Add("__be", "1");

                url = objFdRequestParameter.GenerateUrl(FdConstants.FbFanpageLikerPageleUrl);

                var searchPeopleResponse = _httpHelper.GetRequest(url);

                searchPeopleResponseHandler = new EventGuestsResponseHandler(searchPeopleResponse, EventGuestType.Going);

            }
            else
            {
                try
                {
                    //paginationResponseHandler.IsPagination = true;

                    objFdRequestParameter.UrlParameters.Clear();

                    objFdRequestParameter.UrlParameters.Add("event_id", responseHandler.EntityId);
                    objFdRequestParameter.UrlParameters.Add("tabs[0]", "going");
                    objFdRequestParameter.UrlParameters.Add("order[going]", "affinity");
                    objFdRequestParameter.UrlParameters.Add("bucket_schema[going]", "friends");
                    objFdRequestParameter.UrlParameters.Add("cursor[going]", responseHandler.PageletData);
                    objFdRequestParameter.UrlParameters.Add("dpr", "1");
                    objFdRequestParameter.UrlParameters.Add("fb_dtsg", fbDtsg);
                    objFdRequestParameter.UrlParameters.Add("__user", account.AccountBaseModel.UserId);
                    objFdRequestParameter.UrlParameters.Add("__a", "1");
                    objFdRequestParameter.UrlParameters.Add("__dyn", FdConstants.DynParameter);
                    objFdRequestParameter.UrlParameters.Add("__req", "32");
                    objFdRequestParameter.UrlParameters.Add("__be", "1");

                    url = objFdRequestParameter.GenerateUrl(FdConstants.FbFanpageLikerPageleUrl);

                    var paginationResponse = _httpHelper.GetRequest(url);


                    searchPeopleResponseHandler = new EventGuestsResponseHandler(paginationResponse, EventGuestType.Going);

                    if (!string.IsNullOrEmpty(searchPeopleResponseHandler.PageletData))
                        searchPeopleResponseHandler.HasMoreResults = true;
                }
                catch (Exception ex)
                {
                    ex.DebugLog(ex.Message);
                }
            }


            return searchPeopleResponseHandler;
        }*/

        /// <summary>
        /// Get Guests Invited To events
        /// </summary>
        /// <param name="account"></param>
        /// <param name="friendId"></param>
        /// <returns></returns>
        /*internal IResponseHandler GetInvitedGuestsForEvents(DominatorAccountModel account,
          IResponseHandler responseHandler, string eventUrl)
        {
            string url;

            string paginationData = string.Empty;

            string fbDtsg = Uri.UnescapeDataString(account.SessionId);

            EventGuestsResponseHandler searchPeopleResponseHandler = null;

            FdRequestParameter objFdRequestParameter = new FdRequestParameter();

            if (responseHandler == null)
            {

                url = eventUrl;

                var eventResponse = _httpHelper.GetRequest(url);

                var eventId = FdRegexUtility.FirstMatchExtractor(eventResponse.Response, "eventid\":(.*?),");


                objFdRequestParameter.UrlParameters.Clear();

                objFdRequestParameter.UrlParameters.Add("event_id", eventId);
                objFdRequestParameter.UrlParameters.Add("tabs[0]", "watched");
                objFdRequestParameter.UrlParameters.Add("tabs[1]", "going");
                objFdRequestParameter.UrlParameters.Add("tabs[2]", "invited");
                objFdRequestParameter.UrlParameters.Add("order[declined]", "affinity");
                objFdRequestParameter.UrlParameters.Add("order[going]", "affinity");
                objFdRequestParameter.UrlParameters.Add("order[invited]", "affinity");
                objFdRequestParameter.UrlParameters.Add("order[maybe]", "affinity");
                objFdRequestParameter.UrlParameters.Add("order[watched]", "affinity");
                objFdRequestParameter.UrlParameters.Add("bucket_schema[watched]", "friends");
                objFdRequestParameter.UrlParameters.Add("bucket_schema[invited]", "friends");
                objFdRequestParameter.UrlParameters.Add("bucket_schema[going]", "friends");
                objFdRequestParameter.UrlParameters.Add("dpr", "1");
                objFdRequestParameter.UrlParameters.Add("fb_dtsg", fbDtsg);
                objFdRequestParameter.UrlParameters.Add("__user", account.AccountBaseModel.UserId);
                objFdRequestParameter.UrlParameters.Add("__a", "1");
                objFdRequestParameter.UrlParameters.Add("__dyn", FdConstants.DynParameter);
                objFdRequestParameter.UrlParameters.Add("__req", "18");
                objFdRequestParameter.UrlParameters.Add("__be", "1");

                url = objFdRequestParameter.GenerateUrl(FdConstants.EventGuestUrl);

                var searchPeopleResponse = _httpHelper.GetRequest(url);

                searchPeopleResponseHandler = new EventGuestsResponseHandler(searchPeopleResponse, EventGuestType.Interested);


            }

            else
            {

                try
                {
                    //paginationResponseHandler.IsPagination = true;

                    objFdRequestParameter.UrlParameters.Clear();

                    objFdRequestParameter.UrlParameters.Add("event_id", responseHandler.EntityId);
                    objFdRequestParameter.UrlParameters.Add("tabs[0]", "invited");
                    objFdRequestParameter.UrlParameters.Add("order[invited]", "affinity");
                    objFdRequestParameter.UrlParameters.Add("bucket_schema[invited]", "friends");
                    objFdRequestParameter.UrlParameters.Add("cursor[invited]", responseHandler.PageletData);
                    objFdRequestParameter.UrlParameters.Add("dpr", "1");
                    objFdRequestParameter.UrlParameters.Add("fb_dtsg", fbDtsg);
                    objFdRequestParameter.UrlParameters.Add("__user", account.AccountBaseModel.UserId);
                    objFdRequestParameter.UrlParameters.Add("__a", "1");
                    objFdRequestParameter.UrlParameters.Add("__dyn", FdConstants.DynParameter);
                    objFdRequestParameter.UrlParameters.Add("__req", "32");
                    objFdRequestParameter.UrlParameters.Add("__be", "1");

                    url = objFdRequestParameter.GenerateUrl(FdConstants.FbFanpageLikerPageleUrl);

                    var paginationResponse = _httpHelper.GetRequest(url);


                    searchPeopleResponseHandler = new EventGuestsResponseHandler(paginationResponse, EventGuestType.Interested);

                    if (!string.IsNullOrEmpty(searchPeopleResponseHandler.PageletData))
                        searchPeopleResponseHandler.HasMoreResults = true;
                }
                catch (Exception ex)
                {
                    ex.DebugLog(ex.Message);
                }
            }


            return searchPeopleResponseHandler;
        }*/

        #endregion

        #region MyRegion

        public List<FacebookUser> GetAllMutualFriends(DominatorAccountModel account, string friendId)
        {
            var messengerResponse = _httpHelper.GetRequest(FdConstants.MutualFriendUrl(friendId));

            return new MutualFriendsResponseHandler(messengerResponse).ObjFdScraperResponseParameters.ListUser;

        }


        //it is common for both As Page and As User
        public IResponseHandler LikeComments(DominatorAccountModel accountModel, FdPostCommentDetails commentDetails, ReactionType objReactionType, FanpageDetails fanpageDetails = null)
        {

            string legacyId = commentDetails.CommentId;

            string ftEntIdentifier = commentDetails.PostId;

            string commentId = $"{legacyId}:{ftEntIdentifier}";

            //CommentLikerResponseHandler commentLikerResponseHandler = null;

            FdRequestParameter objFdRequestParameter = new FdRequestParameter();

            var url = objFdRequestParameter.GenerateUrl(FdConstants.LikeCommentsUrl);



            objFdRequestParameter.PostDataParameters.Clear();

            //objFdRequestParameter.PostDataParameters.Add("batch_name", "MessengerGraphQLThreadlistFetcher");

            switch (objReactionType)
            {
                case ReactionType.Like:
                    objFdRequestParameter.PostDataParameters.Add("reaction_type", ((int)ReactionType.Like).ToString());
                    break;

                case ReactionType.Love:
                    objFdRequestParameter.PostDataParameters.Add("reaction_type", ((int)ReactionType.Love).ToString());
                    break;

                case ReactionType.Haha:
                    objFdRequestParameter.PostDataParameters.Add("reaction_type", ((int)ReactionType.Haha).ToString());
                    break;

                case ReactionType.Wow:
                    objFdRequestParameter.PostDataParameters.Add("reaction_type", ((int)ReactionType.Wow).ToString());
                    break;

                case ReactionType.Sad:
                    objFdRequestParameter.PostDataParameters.Add("reaction_type", ((int)ReactionType.Sad).ToString());
                    break;

                case ReactionType.Angry:
                    objFdRequestParameter.PostDataParameters.Add("reaction_type", ((int)ReactionType.Angry).ToString());
                    break;

                case ReactionType.Unlike:
                    objFdRequestParameter.PostDataParameters.Add("reaction_type", ((int)ReactionType.Unlike).ToString());
                    break;

            }

            objFdRequestParameter = CommonPostDataParameters(accountModel, objFdRequestParameter);

            objFdRequestParameter.PostDataParameters.Add("source", "1");

            objFdRequestParameter.PostDataParameters.Add("av",
                fanpageDetails == null ? accountModel.AccountBaseModel.UserId : fanpageDetails.FanPageID);

            objFdRequestParameter.PostDataParameters.Add("client_id", "1535803053981:2369078201");
            objFdRequestParameter.PostDataParameters.Add("__req", "2w");
            objFdRequestParameter.PostDataParameters.Add("session_id", "3a91793b");
            objFdRequestParameter.PostDataParameters.Add("batch_name", "MessengerGraphQLThreadlistFetcher");
            objFdRequestParameter.PostDataParameters.Add("ft_ent_identifier", ftEntIdentifier);
            objFdRequestParameter.PostDataParameters.Add("comment_id", commentId);
            objFdRequestParameter.PostDataParameters.Add("legacy_id", legacyId);
            objFdRequestParameter.PostDataParameters.Add("instance_id", "u_ps_0_0_d");

            var postData = objFdRequestParameter.GetPostDataFromParameters();

            var request = _httpHelper.GetRequestParameter();

            request.ContentType = FdConstants.ContentType;

            _httpHelper.SetRequestParameter(request);

            IResponseParameter responseParameter = _httpHelper.PostRequest(url, postData);

            return new CommentLikerResponseHandler(new ResponseParameter() { Response = responseParameter.Response }, objReactionType);

        }


        public bool ChangeActor(DominatorAccountModel account, string postId, string pageId)
        {
            FdRequestParameter objFdRequestParameter = new FdRequestParameter();

            var url = $"{FdConstants.FbHomeUrl}ufi/actor/change/?dpr=1";

            objFdRequestParameter.PostDataParameters.Clear();
            objFdRequestParameter.PostDataParameters.Add("from_actor_id", account.AccountBaseModel.UserId);
            objFdRequestParameter.PostDataParameters.Add("ft_ent_identifier", postId);
            objFdRequestParameter.PostDataParameters.Add("av", pageId);
            objFdRequestParameter.PostDataParameters.Add("__req", "1i");
            objFdRequestParameter = CommonPostDataParameters(account, objFdRequestParameter);

            var postData = objFdRequestParameter.GetPostDataFromParameters();

            var request = _httpHelper.GetRequestParameter();

            request.ContentType = FdConstants.ContentType;

            _httpHelper.SetRequestParameter(request);

            var incommingMessageResponse = _httpHelper.PostRequest(url, postData);

            return incommingMessageResponse.Response.Contains("MultiBootstrapDataSource")
                   || incommingMessageResponse.Response.Contains("bootstrapData");
        }


        public bool ChangeActorForPost(DominatorAccountModel account, string postId, string pageId,
            string actorId, string composerId)
        {
            FdRequestParameter objFdRequestParameter = new FdRequestParameter();

            actorId = FdFunctions.GetIntegerOnlyString(actorId);

            var url = $"{FdConstants.FbHomeUrl}react_composer/actor/change/";

            objFdRequestParameter.UrlParameters.Clear();
            objFdRequestParameter.UrlParameters.Add("composer_id", composerId);
            objFdRequestParameter.UrlParameters.Add("composer_type", "pages_feed");
            objFdRequestParameter.UrlParameters.Add("target_id", pageId);
            objFdRequestParameter.UrlParameters.Add("av", actorId);
            objFdRequestParameter.UrlParameters.Add("dpr", "1");

            url = objFdRequestParameter.GenerateUrl(url);

            objFdRequestParameter.PostDataParameters.Clear();

            objFdRequestParameter.PostDataParameters.Add("__req", "1i");

            objFdRequestParameter = CommonPostDataParameters(account, objFdRequestParameter);

            var postData = objFdRequestParameter.GetPostDataFromParameters();

            var request = _httpHelper.GetRequestParameter();

            request.ContentType = FdConstants.ContentType;

            _httpHelper.SetRequestParameter(request);

            var incommingMessageResponse = _httpHelper.PostRequest(url, postData);

            if (incommingMessageResponse.Response.Contains("ReactComposerMediaSproutContainer"))
                return true;

            url = $"{FdConstants.FbHomeUrl}react_composer/actor/change/";
            objFdRequestParameter.UrlParameters.Remove("av");
            objFdRequestParameter.UrlParameters.Add("av", pageId);
            url = objFdRequestParameter.GenerateUrl(url);
            incommingMessageResponse = _httpHelper.PostRequest(url, postData);

            return incommingMessageResponse.Response.Contains("ReactComposerMediaSproutContainer");

        }

        #endregion


        #region ReplyToComment
        public IResponseHandler ReplyOnPost(DominatorAccountModel account, string postId, string commentText
            , string commentId, string pageId = "")
        {
            FdRequestParameter objFdRequestParameter = new FdRequestParameter();


            objFdRequestParameter.UrlParameters.Add("dpr", "1");
            var url = objFdRequestParameter.GenerateUrl(FdConstants.PostCommentUrl);
            var parentCommentId = postId + "_" + commentId;
            objFdRequestParameter.PostDataParameters.Add("ft_ent_identifier", postId);
            objFdRequestParameter.PostDataParameters.Add("comment_text", commentText);
            objFdRequestParameter.PostDataParameters.Add("source", "1");
            objFdRequestParameter.PostDataParameters.Add("client_id", "1536912522897:94090525");
            objFdRequestParameter.PostDataParameters.Add("session_id", "7abd414e");
            objFdRequestParameter.PostDataParameters.Add("reply_fbid", commentId);
            objFdRequestParameter.PostDataParameters.Add("parent_comment_id", parentCommentId);
            objFdRequestParameter.PostDataParameters.Add("attached_sticker_fbid", "0");
            objFdRequestParameter.PostDataParameters.Add("attached_photo_fbid", "0");
            objFdRequestParameter.PostDataParameters.Add("attached_video_fbid", "0");
            objFdRequestParameter.PostDataParameters.Add("attached_file_fbid", "0");
            objFdRequestParameter.PostDataParameters.Add("attached_share_url", "");
            objFdRequestParameter.PostDataParameters.Add("av",
                string.IsNullOrEmpty(pageId) ? account.AccountBaseModel.UserId : pageId);
            objFdRequestParameter.PostDataParameters.Add("__req", "11");

            objFdRequestParameter = CommonPostDataParameters(account, objFdRequestParameter);

            var postData = objFdRequestParameter.GetPostDataFromParameters();
            var request = _httpHelper.GetRequestParameter();
            request.ContentType = FdConstants.ContentType;
            _httpHelper.SetRequestParameter(request);
            var commentOnPostResponse = _httpHelper.PostRequest(url, postData);
            return new CommentOnPostResponseHandler(commentOnPostResponse);

        }
        #endregion

        private static int page;
        public IResponseHandler GetPostListFromAlbums
              (DominatorAccountModel account, IResponseHandler responseHandler, string albumUrl = null)
        {

            var url = string.Empty;

            FdRequestParameter objFdRequestParameter = new FdRequestParameter();

            try
            {
                ScrapPostFromAlbumsResponseHandler scrapPostFromAlbumsResponseHandler;

                if (responseHandler == null)
                {
                    if (albumUrl != null)
                    {
                        var id = FdRegexUtility.FirstMatchExtractor(albumUrl.ToLower(), "set=a.(.*?)&");
                        var user = FdRegexUtility.FirstMatchExtractor(albumUrl.ToLower(), "https://www.facebook.com(.*?)media_set");

                        if (string.IsNullOrEmpty(user))
                            user = $"/{FdRegexUtility.FirstMatchExtractor(albumUrl.ToLower(), "id=(.*?)&")}/";

                        url = $"https://m.facebook.com{user}albums/{id}/";
                    }

                    var requestParam = _httpHelper.GetRequestParameter();

                    if (requestParam.Headers != null)
                        requestParam.Headers["Host"] = "m.facebook.com";

                    _httpHelper.SetRequestParameter(requestParam);

                    var newsFeedResponse = _httpHelper.GetRequest(url);

                    scrapPostFromAlbumsResponseHandler = new ScrapPostFromAlbumsResponseHandler(newsFeedResponse, string.Empty);

                    requestParam = _httpHelper.GetRequestParameter();

                    if (requestParam.Headers != null)
                        requestParam.Headers["Host"] = "www.facebook.com";

                    _httpHelper.SetRequestParameter(requestParam);
                }
                else
                {
                    objFdRequestParameter.UrlParameters.Clear();
                    objFdRequestParameter.PostDataParameters.Clear();

                    var id = FdRegexUtility.FirstMatchExtractor(albumUrl.ToLower(), "set=(.*?)&");
                    var pagination = Convert.ToString(++page * 12);

                    objFdRequestParameter.UrlParameters.Add("s", pagination);
                    objFdRequestParameter.UrlParameters.Add("set", id);
                    objFdRequestParameter.UrlParameters.Add("mode", "albumpermalink");

                    objFdRequestParameter.PostDataParameters.Add("m_sess", "");
                    objFdRequestParameter.PostDataParameters.Add("fb_dtsg", account.SessionId);
                    objFdRequestParameter.PostDataParameters.Add("jazoest", account.AccountBaseModel.UserId);
                    objFdRequestParameter.PostDataParameters.Add("__dyn", FdConstants.DynParameter);
                    objFdRequestParameter.PostDataParameters.Add("__req", "17");
                    objFdRequestParameter.PostDataParameters.Add("__ajax__", responseHandler.ObjFdScraperResponseParameters.FinalEncodedQuery);
                    objFdRequestParameter.PostDataParameters.Add("__user", account.AccountBaseModel.UserId);

                    url = objFdRequestParameter.GenerateUrl("https://m.facebook.com/media/set/");

                    var requestParam = _httpHelper.GetRequestParameter();

                    requestParam.Headers["Host"] = "m.facebook.com";

                    requestParam.Headers["X-Requested-With"] = "XMLHttpRequest";

                    requestParam.ContentType = "application/x-www-form-urlencoded";

                    _httpHelper.SetRequestParameter(requestParam);

                    var postData = objFdRequestParameter.GetPostDataFromParameters();

                    var paginationResponse = _httpHelper.PostRequest(url, postData);

                    scrapPostFromAlbumsResponseHandler = new ScrapPostFromAlbumsResponseHandler(paginationResponse, responseHandler.ObjFdScraperResponseParameters.AlbumName);

                    requestParam = _httpHelper.GetRequestParameter();

                    requestParam.Headers.Remove("X-Requested-With");

                    requestParam.Headers["Host"] = "www.facebook.com";

                    _httpHelper.SetRequestParameter(requestParam);
                }

                return scrapPostFromAlbumsResponseHandler;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return null;
        }

        public IResponseHandler GetPostListFromKeyWords
              (DominatorAccountModel account, IResponseHandler responseHandler, string keyword)
        {
            FdRequestParameter objFdRequestParameter = new FdRequestParameter();

            try
            {

                var url = FdConstants.FbScrapPostByKeyWordUrl(keyword.Replace(" ", "+"), FdConstants.PublicPostFilters);

                url = $"{FdConstants.FbHomeUrl}search/posts/?q={keyword}";

                if (responseHandler == null)
                {
                    var newsFeedResponse = _httpHelper.GetRequest(url);

                    List<KeyValuePair<string, string>> listPostReaction = new PostReactionListResponseHandler(newsFeedResponse).ListPostReaction;

                    return new PostScraperForKeywordResponseHandler(newsFeedResponse,
                        new FdScraperResponseParameters(), listPostReaction);

                }
                else
                {
                    objFdRequestParameter.UrlParameters.Clear();
                    objFdRequestParameter.PostDataParameters.Clear();

                    url = objFdRequestParameter.GenerateUrl(FdConstants.FbFanpageLikerPageleUrl);

                    objFdRequestParameter.UrlParameters.Add("dpr", "1");
                    objFdRequestParameter.UrlParameters.Add("data", responseHandler.PageletData);
                    objFdRequestParameter.UrlParameters.Add("__req", "10");
                    objFdRequestParameter = CommonUrlParameters(account, objFdRequestParameter);

                    url = objFdRequestParameter.GenerateUrl(url);

                    var paginationResponse = _httpHelper.GetRequest(url);

                    List<KeyValuePair<string, string>> listPostReaction = new PostReactionListResponseHandler(paginationResponse).ListPostReaction;

                    return new PostScraperForKeywordResponseHandler(paginationResponse,
                        responseHandler.ObjFdScraperResponseParameters, listPostReaction);


                }

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return null;
        }

        public async Task<Dictionary<IpLocationDetails, string>> GetIpDetails(DominatorAccountModel account, bool isLocalIp = false)
        {
            var dictIpDetails = new Dictionary<IpLocationDetails, string>();
            try
            {
                try
                {
                    Proxy objProxy = _httpHelper.GetRequestParameter().Proxy;


                    string ip = string.Empty;

                    objProxy = objProxy ?? new Proxy();

                    if (isLocalIp)
                    {
                        try
                        {
                            var objHttpHelper = new FdHttpHelper();
                            var requestParameter = _httpHelper.GetRequestParameter();
                            requestParameter.Proxy = new Proxy();
                            objHttpHelper.SetRequestParameter(requestParameter);
                            var localIpResponse = await _httpHelper.GetApiRequestAsync(FdConstants.GetLocationUrl);
                            var localCountry = Utilities.GetBetween(localIpResponse.Response, "<div class=\"ptih-title\">", "</div>");
                            dictIpDetails.Add(IpLocationDetails.Country, !string.IsNullOrEmpty(localCountry) ? localCountry : string.Empty);
                            return dictIpDetails;
                        }
                        catch (Exception)
                        {
                            return dictIpDetails;
                        }
                    }

                    if (string.IsNullOrEmpty(objProxy.ProxyIp))
                    {
                        try
                        {
                            var ipResponse = await _httpHelper.GetApiRequestAsync(FdConstants.GetLocationUrl);
                            ip = ipResponse.Response;
                            ip = Utilities.GetBetween(ip, "pti-header bgm-green\">", "/h2>");
                            ip = Utilities.GetBetween(ip, ">", "<");
                        }
                        catch (Exception)
                        {

                        }
                    }
                    else
                        ip = objProxy.ProxyIp;

                    ip = ip?.Replace("\r\n", string.Empty);

                    Match match = Regex.Match(objProxy.ProxyUsername ?? string.Empty, @"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}");

                    var locationUrl = FdConstants.GetLocationApiUrl();

                    var locationResponse = await _httpHelper.GetApiRequestAsync(locationUrl);

                    var proxyLocationDetails = locationResponse.Response;

                    if (proxyLocationDetails.Contains("INVALID_ADDRESS") && !match.Success)
                    {
                        var proxyIpResponse = await _httpHelper.GetApiRequestAsync(FdConstants.GetLocationUrl);
                        ip = proxyIpResponse.Response;
                        ip = Utilities.GetBetween(ip, "pti-header bgm-green\">", "/h2>");
                        ip = Utilities.GetBetween(ip, ">", "<");

                        locationUrl = FdConstants.GetLocationApiUrl();

                        var proxyLocationResponse = await _httpHelper.GetApiRequestAsync(locationUrl);
                        proxyLocationDetails = proxyLocationResponse.Response;
                    }
                    else if (match.Success)
                    {
                        ip = match.Groups[0].ToString().Trim();

                        locationUrl = FdConstants.GetLocationApiUrl();

                        locationResponse = await _httpHelper.GetApiRequestAsync(locationUrl);

                        proxyLocationDetails = locationResponse.Response;

                        proxyLocationDetails = FdFunctions.GetNewPrtialDecodedResponse(proxyLocationDetails);
                    }

                    if (proxyLocationDetails.Contains("\"error\""))
                    {
                        locationUrl = FdConstants.GetLocationApiUrlFree();
                        var proxyLocationResponse = await _httpHelper.GetApiRequestAsync(locationUrl);
                        proxyLocationDetails = proxyLocationResponse.Response;

                    }

                    var city = Utilities.GetBetween(proxyLocationDetails, "city\": \"", "\"");
                    city = string.IsNullOrEmpty(city) ? Utilities.GetBetween(proxyLocationDetails, "city\":\"", "\"") : city;
                    var state = Utilities.GetBetween(proxyLocationDetails, "stateProv\": \"", "\"");
                    state = string.IsNullOrEmpty(state) ? Utilities.GetBetween(proxyLocationDetails, "regionName\":\"", "\"") : state;
                    var country = Utilities.GetBetween(proxyLocationDetails, "countryName\": \"", "\"");
                    country = string.IsNullOrEmpty(country) ? Utilities.GetBetween(proxyLocationDetails, "country\":\"", "\"") : country;

                    dictIpDetails.Add(IpLocationDetails.City, !string.IsNullOrEmpty(city) ? city : string.Empty);
                    dictIpDetails.Add(IpLocationDetails.State, !string.IsNullOrEmpty(state) ? state : string.Empty);
                    dictIpDetails.Add(IpLocationDetails.Country, !string.IsNullOrEmpty(country) ? country : string.Empty);

                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error(ex.StackTrace);
                }

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return dictIpDetails;
        }

        //public bool EditName(DominatorAccountModel account, string fullName)
        //{
        //    string userId = account.AccountBaseModel.UserId;

        //    FdRequestParameter objParameter = new FdRequestParameter();

        //    string homePageResponse = _httpHelper.GetRequest(FdConstants.FbOwnTimelineUrl(userId)).Response;

        //    var splitName = fullName.Split(' ');

        //    var firstName = splitName.Length >= 1 ? splitName[0] : string.Empty;

        //    var middleName = splitName.Length >= 2 ? splitName[1] : string.Empty;

        //    var lastName = splitName.Length >= 3 ? splitName[2] : string.Empty;

        //    var ajaxToken = Utilities.GetBetween(homePageResponse, "ajaxpipe_token\":\"", "\"");
        //    var quickLingVersion = Utilities.GetBetween(homePageResponse, "[],{version:\"", "\"");

        //    objParameter.AddUrlParameters("dpr", "1");
        //    objParameter.AddUrlParameters("ajaxpipe", "1");
        //    objParameter.AddUrlParameters("ajaxpipe_token", ajaxToken);
        //    objParameter.AddUrlParameters("no_script_path", "1");
        //    objParameter.AddUrlParameters("quickling[version]", Uri.EscapeDataString(quickLingVersion));
        //    objParameter.AddUrlParameters("__user", account.AccountBaseModel.UserId);
        //    objParameter.AddUrlParameters("__adt", "2");


        //    objParameter.PostDataParameters.Add("primary_first_name", Uri.EscapeDataString(firstName));
        //    objParameter.PostDataParameters.Add("primary_middle_name", Uri.EscapeDataString(middleName));
        //    objParameter.PostDataParameters.Add("primary_last_name", Uri.EscapeDataString(lastName));
        //    objParameter.PostDataParameters.Add("show_alternate", "1");
        //    objParameter.PostDataParameters.Add("alternate_name", "");

        //    objParameter = CommonPostDataParameters(account, objParameter);

        //    var postUrl = FdConstants.NameChangePreviewUrl;

        //    var objRequestParamter = _httpHelper.GetRequestParameter();

        //    objRequestParamter.Referer = $"{FdConstants.FbHomeUrl}settings?tab=account&section=name&view";

        //    objRequestParamter.ContentType = FdConstants.ContentType;

        //    _httpHelper.SetRequestParameter(objRequestParamter);

        //    var postData = objParameter.GetPostDataFromParameters();

        //    string editNameResponse = _httpHelper.PostRequest(postUrl, postData).Response;

        //    var message = Utilities.GetBetween(editNameResponse, "u003C\\/i>", ".");

        //    if (string.IsNullOrEmpty(message))
        //    {
        //        objParameter.PostDataParameters.Add("display_format", "complete");
        //        objParameter.PostDataParameters.Add("save_password", Uri.EscapeDataString(account.AccountBaseModel.Password));
        //        objParameter.PostDataParameters.Add("primary_first_name", Uri.EscapeDataString(firstName));
        //        objParameter.PostDataParameters.Add("primary_middle_name", Uri.EscapeDataString(middleName));
        //        objParameter.PostDataParameters.Add("primary_last_name", Uri.EscapeDataString(lastName));
        //        objParameter.PostDataParameters.Add("show_alternate", "1");
        //        objParameter.PostDataParameters.Add("alternate_name", "");

        //        objParameter = CommonPostDataParameters(account, objParameter);

        //        var finalPostData = objParameter.GetPostDataFromParameters();

        //        var finalPostUrl = FdConstants.FinalNameChnageUrl;

        //        string changeNameResponse = _httpHelper.PostRequest(finalPostUrl, finalPostData).Response;

        //        if (changeNameResponse.Contains("\\u003Cstrong>"))
        //        {
        //            GlobusLogHelper.log.Info("Success: Name Changed Successfully");
        //            return true;
        //        }
        //        else
        //        {
        //            GlobusLogHelper.log.Info("Failed: " + message);
        //            return false;
        //        }
        //    }
        //    else
        //    {
        //        GlobusLogHelper.log.Info("Failed: " + message);
        //        return false;
        //    }

        //}

        public async Task ScrapOwnProfileInfoAsync(DominatorAccountModel account)
        {
            try
            {
                var url = FdConstants.SaveAdsUrlMain + "fb_user_data";
                //      var url = FdConstants.SaveAdsDataDev + "fb_user_data";
                FacebookUser objUser = new FacebookUser();
                if (!string.IsNullOrEmpty(account.AccountBaseModel.UserId))
                    objUser = new FacebookUser { UserId = account.AccountBaseModel.UserId };
                else
                    objUser = new FacebookUser { Username = account.AccountBaseModel.UserName };

                var userDetails = await GetDetailedInfoUserMobileScraperAsync(objUser, account, true, true, account.Token);

                objUser = userDetails.ObjFdScraperResponseParameters.FacebookUser;
                account.AccountBaseModel.UserId = string.IsNullOrEmpty(account.AccountBaseModel.UserId) ? objUser.UserId : account.AccountBaseModel.UserId;
                int age = 0;
                var pattern = @"\d{4}";
                var objRegex = new Regex(pattern);
                try
                {
                    var dob = objRegex.Match(objUser.DateOfBirth).ToString();
                    var year = !string.IsNullOrEmpty(dob) ? int.Parse(dob) : 0;
                    if (year != 0)
                        age = DateTime.Now.Year - year;
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }


                var reqParameter = new FdRequestParameter();
                reqParameter.PostDataParameters.Add("facebook_id", account.AccountBaseModel.UserId);
                reqParameter.PostDataParameters.Add("current_country", Uri.EscapeDataString(objUser.Currentcity?.Replace(",", string.Empty)));
                reqParameter.PostDataParameters.Add("name", Uri.EscapeDataString(objUser.Familyname));
                reqParameter.PostDataParameters.Add("others_places_lived", Uri.EscapeDataString(objUser.Hometown?.Replace(",", string.Empty)));
                reqParameter.PostDataParameters.Add("Gender", objUser.Gender);
                reqParameter.PostDataParameters.Add("age", age.ToString());
                reqParameter.PostDataParameters.Add("server_user", "2");
                reqParameter.PostDataParameters.Add("relationship_status", objUser.RelationShip);

                var objRequestParameter = _httpHelper.GetRequestParameter();

                objRequestParameter.AddHeader("Host", "api.poweradspy.com");


                objRequestParameter.ContentType = FdConstants.ContentType;

                _httpHelper.SetRequestParameter(objRequestParameter);

                var postData = reqParameter.GetPostDataFromParameters();

                try
                {
                    await _httpHelper.PostApiRequestAsync(url, postData);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

        }

        //public async Task UpdateProfileInfo(DominatorAccountModel account, FacebookUser objUser)
        //{
        //    try
        //    {
        //        var url = FdConstants.SaveAdsUrlMain + "fb_user_data";
        //   //     var url = FdConstants.SaveAdsDataDev + "fb_user_data";
        //        account.AccountBaseModel.UserId = string.IsNullOrEmpty(account.AccountBaseModel.UserId) ? objUser.UserId : account.AccountBaseModel.UserId;
        //        int age = 0;
        //        var pattern = @"\d{4}";
        //        var objRegex = new Regex(pattern);
        //        try
        //        {
        //            var dob = objRegex.Match(objUser.DateOfBirth).ToString();
        //            var year = !string.IsNullOrEmpty(dob) ? int.Parse(dob) : 0;
        //            if(year != 0)
        //                age = DateTime.Now.Year - year;
        //        }
        //        catch (Exception ex)
        //        {
        //            ex.DebugLog();
        //        }


        //        var reqParameter = new FdRequestParameter();
        //        reqParameter.PostDataParameters.Add("facebook_id", account.AccountBaseModel.UserId);
        //        reqParameter.PostDataParameters.Add("current_country", Uri.EscapeDataString(objUser.Currentcity?.Replace(",", string.Empty)));
        //        reqParameter.PostDataParameters.Add("name", Uri.EscapeDataString(objUser.Familyname));
        //        reqParameter.PostDataParameters.Add("others_places_lived", Uri.EscapeDataString(objUser.Hometown?.Replace(",", string.Empty)));
        //        reqParameter.PostDataParameters.Add("Gender", objUser.Gender);
        //        if (age == 0)
        //            reqParameter.PostDataParameters.Add("age", "");
        //        else
        //            reqParameter.PostDataParameters.Add("age",age.ToString());

        //        reqParameter.PostDataParameters.Add("server_user", "2");
        //        reqParameter.PostDataParameters.Add("relationship_status", objUser.RelationShip);

        //        var objRequestParameter = _httpHelper.GetRequestParameter();

        //        objRequestParameter.AddHeader("Host", "api.poweradspy.com");


        //        objRequestParameter.ContentType = FdConstants.ContentType;

        //        _httpHelper.SetRequestParameter(objRequestParameter);

        //        var postData = reqParameter.GetPostDataFromParameters();

        //        try
        //        {
        //            await _httpHelper.PostApiRequestAsync(url, postData);
        //        }
        //        catch (Exception e)
        //        {
        //            Console.WriteLine(e);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ex.DebugLog();
        //    }
        //}

        //public void EditDateOfBirth(DominatorAccountModel account, DateTime dob)
        //{
        //    var parameter = _httpHelper.GetRequestParameter();

        //    parameter.ContentType = FdConstants.ContentType;

        //    FdRequestParameter objParameter = new FdRequestParameter();

        //    objParameter.PostDataParameters.Add("field_type", "birthday");
        //    objParameter.PostDataParameters.Add("nctr[_mod]", "pagelet_basic");
        //    objParameter.PostDataParameters.Add("__req", "s");

        //    objParameter = CommonPostDataParameters(account, objParameter);

        //    objParameter.PostDataParameters.Add("fb_dtsg", account.SessionId);

        //    var postData = objParameter.GetPostDataFromParameters();

        //    var objRequestParamter = _httpHelper.GetRequestParameter();

        //    objRequestParamter.ContentType = FdConstants.ContentType;

        //    _httpHelper.SetRequestParameter(objRequestParamter);

        //    string homePageResponse = _httpHelper.PostRequest(FdConstants.GetInfoTabUrl, postData).Response;

        //    string decodedResponse = FdFunctions.GetPrtialDecodedResponse(homePageResponse);

        //    string privacyDetails = FdRegexUtility.FirstMatchExtractor(decodedResponse, "name=\"privacy(.*?)>");

        //    var privacyId = FdRegexUtility.FirstMatchExtractor(privacyDetails, "[(.*?)]");

        //    var privacyValue = FdRegexUtility.FirstMatchExtractor(privacyDetails, "value=\"(.*?)\"");

        //    objParameter.PostDataParameters.Clear();

        //    objParameter.PostDataParameters.Add($"privacy[{privacyId}]", $"{privacyValue}");
        //    objParameter.PostDataParameters.Add($"privacy[{privacyId}]", $"{privacyValue}");
        //    objParameter.PostDataParameters.Add("birthday_day", dob.Day.ToString());
        //    objParameter.PostDataParameters.Add("birthday_month", dob.Month.ToString());
        //    objParameter.PostDataParameters.Add("birthday_year", dob.Year.ToString());
        //    objParameter.PostDataParameters.Add("__submit__", "1");
        //    objParameter.PostDataParameters.Add("ft[tn]", "-k");
        //    objParameter.PostDataParameters.Add("nctr[_mod]", "pagelet_basic");
        //    objParameter.PostDataParameters.Add("__req", "s");

        //    objParameter = CommonPostDataParameters(account, objParameter);

        //    objParameter.PostDataParameters.Add("fb_dtsg", account.SessionId);

        //    postData = objParameter.GetPostDataFromParameters();

        //    _httpHelper.PostRequest(FdConstants.EditDobUrl, postData);


        //}

        //public bool EditGender(DominatorAccountModel account, int genderValue)
        //{
        //    var objParameter = new FdRequestParameter();

        //    objParameter.PostDataParameters.Clear();

        //    objParameter.PostDataParameters.Add("field_type", "gender");
        //    objParameter.PostDataParameters.Add("nctr[_mod]", "pagelet_basic");

        //    objParameter = CommonPostDataParameters(account, objParameter);

        //    string editGenderFormUrl = FdConstants.EditGenderUrl;


        //    var postData = objParameter.GetPostDataFromParameters();

        //    var objRequestParamter = _httpHelper.GetRequestParameter();

        //    objRequestParamter.ContentType = FdConstants.ContentType;

        //    _httpHelper.SetRequestParameter(objRequestParamter);

        //    string editGenderFormResponse = _httpHelper.PostRequest(editGenderFormUrl, postData).Response;

        //    var privacyValue = Utilities.GetBetween(editGenderFormResponse, "privacy[237760973066217]\\\" value=\\\"", "\\\"");


        //    if (!string.IsNullOrEmpty(editGenderFormResponse))
        //    {
        //        var finalPostUrl = FdConstants.EditGenderUrl;

        //        objParameter.PostDataParameters.Clear();

        //        objParameter.PostDataParameters.Add("sex", genderValue.ToString());
        //        objParameter.PostDataParameters.Add("privacy[237760973066217]", privacyValue);
        //        objParameter.PostDataParameters.Add("sex_preferred_pronouns", "1");
        //        objParameter.PostDataParameters.Add("__submit__", "1");
        //        objParameter.PostDataParameters.Add("nctr[_mod]", "pagelet_basic");

        //        objParameter = CommonPostDataParameters(account, objParameter);

        //        var requestParameter = _httpHelper.GetRequestParameter();

        //        requestParameter.ContentType = FdConstants.ContentType;

        //        _httpHelper.SetRequestParameter(requestParameter);

        //        var finalPostData = objParameter.GetPostDataFromParameters();

        //        _httpHelper.PostRequest(finalPostUrl, finalPostData);

        //        return true;
        //    }

        //    if (editGenderFormResponse != null && editGenderFormResponse.Contains("I change my birthday?"))
        //    {
        //        GlobusLogHelper.log.Info("Failed: Gender update");
        //        return false;
        //    }

        //    return false;
        //}

        //public bool EditBio(DominatorAccountModel account, string bio)
        //{
        //    string editBioFormUrl = FdConstants.EditBioUrl;

        //    var objParameter = new FdRequestParameter();

        //    objParameter.PostDataParameters.Clear();

        //    objParameter.PostDataParameters.Add("bio", "gender");
        //    objParameter.PostDataParameters.Add("bio_expiration_time", Uri.EscapeDataString(bio));
        //    objParameter.PostDataParameters.Add("intro_card_session_id", "-1");

        //    objParameter = CommonPostDataParameters(account, objParameter);

        //    var requestParameter = _httpHelper.GetRequestParameter();

        //    requestParameter.ContentType = FdConstants.ContentType;

        //    _httpHelper.SetRequestParameter(requestParameter);

        //    var finalPostData = objParameter.GetPostDataFromParameters();

        //    string editBioResponse = _httpHelper.PostRequest(editBioFormUrl, finalPostData).Response;

        //    return editBioResponse.Contains("profile_intro_card_bio");

        //}

        //public bool UploadProfilePic(DominatorAccountModel account, string imagePath)
        //{
        //    #region Upload Image Process

        //    try
        //    {

        //        var fbIdValue = UploadImageAndGetMediaIdForMessage(account, new List<string>() { imagePath });

        //        if (!string.IsNullOrEmpty(fbIdValue))
        //        {

        //            var objParameter = new FdRequestParameter();

        //            objParameter.PostDataParameters.Clear();

        //            objParameter.UrlParameters.Add("x", "1");
        //            objParameter.UrlParameters.Add("y", "1");
        //            objParameter.UrlParameters.Add("width", "1");
        //            objParameter.UrlParameters.Add("height", "1");
        //            objParameter.UrlParameters.Add("photo_id", fbIdValue);
        //            objParameter.UrlParameters.Add("profile_id", account.AccountBaseModel.UserId);
        //            objParameter.UrlParameters.Add("source", "unknown");
        //            objParameter.UrlParameters.Add("method", "upload");
        //            objParameter.UrlParameters.Add("dpr", "1");

        //            var finalPostUrl = objParameter.GenerateUrl(FdConstants.EditProfilePictureUrl);

        //            objParameter.PostDataParameters.Clear();

        //            objParameter.PostDataParameters.Add("field_type", "gender");
        //            objParameter.PostDataParameters.Add("nctr[_mod]", "pagelet_basic");

        //            objParameter = CommonPostDataParameters(account, objParameter);

        //            var finalPostData = objParameter.GetPostDataFromParameters();

        //            string finalResponse = _httpHelper.PostRequest(finalPostUrl, finalPostData).Response;

        //            if (finalResponse.Contains("ConfirmationDialog"))
        //            {
        //                GlobusLogHelper.log.Info("Successful: Profile Picture changed");
        //            }

        //            return true;
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
        //    }

        //    return false;

        //    #endregion
        //}

        public FdErrorDetails InviteFriendOrPage(DominatorAccountModel account, string recipientId, ref FacebookPostDetails watchPartyDetails,
            bool isInviteInMessaenger = true)
        {
            try
            {
                var watchPartyUrl = (watchPartyDetails.Id.Contains(FdConstants.FbHomeUrl)) ? watchPartyDetails.Id :
                        FdConstants.FbHomeUrl + watchPartyDetails.Id;

                var inviteInMessangerParameter = "false";



                var responseParameters = _httpHelper.GetRequest(watchPartyUrl);

                if (responseParameters.Response.Contains("LiveVideoCurrentlyWatchingVideoPresenceConfig"))
                {
                    FdRequestParameter objFdRequestParameter = new FdRequestParameter();
                    string url = $"{FdConstants.FbHomeUrl}api/graphql/ ";
                    string user = account.AccountBaseModel.UserId;
                    string livingRoomId = Utilities.GetBetween(watchPartyUrl, "wp/", "/");

                    if (string.IsNullOrEmpty(livingRoomId))
                        livingRoomId = FdFunctions.GetIntegerOnlyString(watchPartyUrl);

                    watchPartyDetails.Id = livingRoomId;


                    string variableData1 = $"\"client_mutation_id\":\"100\",\"actor_id\":{"\"" + user + "\""},\"living_room_id\":{"\"" + livingRoomId + "\""},\"recipient_ids\":[{"\"" + recipientId + "\""}],\"sender_id\":{"\"" + user + "\""},\"send_in_messenger\":{inviteInMessangerParameter}";
                    string variableData2 = "\"input\":{" + variableData1 + "}";
                    string variableData = "{" + $"{variableData2}" + "}";

                    objFdRequestParameter.PostDataParameters.Add("av", account.AccountBaseModel.UserId);
                    objFdRequestParameter.PostDataParameters.Add("__req", "5j");
                    objFdRequestParameter.PostDataParameters.Add("variables", Uri.EscapeDataString(variableData));
                    objFdRequestParameter.PostDataParameters.Add("doc_id", "1540579336057750");
                    objFdRequestParameter.PostDataParameters.Add("fb_api_caller_class", "RelayModern");
                    objFdRequestParameter = CommonPostDataParameters(account, objFdRequestParameter);
                    var postData = objFdRequestParameter.GeneratePostData();
                    var parameter = _httpHelper.GetRequestParameter();
                    parameter.ContentType = FdConstants.ContentType;
                    _httpHelper.SetRequestParameter(parameter);
                    objFdRequestParameter.Referer = $"{FdConstants.FbHomeUrl}watchparty/" + livingRoomId + "/";

                    var commentOnPostResponse = _httpHelper.PostRequest(url, postData);

                    if (commentOnPostResponse.Response.Contains("client_mutation_id"))
                        return null;
                    else
                    {
                        inviteInMessangerParameter = "true";
                        variableData1 = $"\"client_mutation_id\":\"100\",\"actor_id\":{"\"" + user + "\""},\"living_room_id\":{"\"" + livingRoomId + "\""},\"recipient_ids\":[{"\"" + recipientId + "\""}],\"sender_id\":{"\"" + user + "\""},\"send_in_messenger\":{inviteInMessangerParameter}";
                        variableData2 = "\"input\":{" + variableData1 + "}";
                        variableData = "{" + $"{variableData2}" + "}";
                        objFdRequestParameter.PostDataParameters.Remove("variables");
                        objFdRequestParameter.PostDataParameters.Add("variables", Uri.EscapeDataString(variableData));


                        postData = objFdRequestParameter.GeneratePostData();
                        commentOnPostResponse = _httpHelper.PostRequest(url, postData);

                        if (!commentOnPostResponse.Response.Contains("client_mutation_id"))
                        {
                            FdErrorDetails objFdErrorDetails = new FdErrorDetails()
                            {
                                Description = "User is Not a Group Member or friend"
                            };

                            return objFdErrorDetails;
                        }


                        return null;
                    }
                }
                else
                {
                    GlobusLogHelper.log.Info("Watch Party With Url" + watchPartyUrl + "Got Expired");
                    FdErrorDetails objFdErrorDetails = new FdErrorDetails()
                    {
                        Description = "Watch Party Expired!"
                    };

                    return objFdErrorDetails;
                }

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            FdErrorDetails objFdErrorDetailsFailed = new FdErrorDetails()
            {
                Description = "Unknown Reason!"
            };

            return objFdErrorDetailsFailed;
        }

        public IResponseHandler GetUsersBirtdayResponse
            (DominatorAccountModel account, IResponseHandler responseHandler)
        {
            FdUserBirthdayResponseHandlerMobile objFdUserBirthdayResponseHandlerMobile;

            string birthdayPageUrl = FdConstants.BirthDayUrl;

            var objParameter = _httpHelper.GetRequestParameter();

            if (objParameter.Headers != null)
                objParameter.Headers["Host"] = "m.facebook.com";

            objParameter.Referer = "https://m.facebook.com/";

            _httpHelper.SetRequestParameter(objParameter);

            if (responseHandler == null)
            {
                var aboutPageResponse = _httpHelper.GetRequest(birthdayPageUrl);

                objFdUserBirthdayResponseHandlerMobile = new FdUserBirthdayResponseHandlerMobile(aboutPageResponse);
            }
            else
            {
                var objFdRequestParameter = new FdRequestParameter();

                birthdayPageUrl = FdConstants.BirthDayPaginationUrl;

                FdJsonElement objFdJsonElement = new FdJsonElement()
                {
                    ActionHistory = "null"
                };

                objFdRequestParameter.UrlParameters.Add("acontext", JsonConvert.SerializeObject(objFdJsonElement));

                objFdRequestParameter.UrlParameters.Add("cursor", responseHandler.PageletData);
                //objFdRequestParameter.UrlParameters.Add("cursor", responseHandler.PaginationToken);

                birthdayPageUrl = objFdRequestParameter.GenerateUrl(birthdayPageUrl);

                var paginationResponse = _httpHelper.GetRequest(birthdayPageUrl);

                objFdUserBirthdayResponseHandlerMobile = new FdUserBirthdayResponseHandlerMobile(paginationResponse);

            }

            var parameter = _httpHelper.GetRequestParameter();

            if (objParameter.Headers != null)
                parameter.Headers["Host"] = "www.facebook.com";

            Thread.Sleep(1000);

            _httpHelper.SetRequestParameter(parameter);

            return objFdUserBirthdayResponseHandlerMobile;
        }

        //public IncommingPageMessageResponseHandler GetMessageDetailsFromPage
        //   (DominatorAccountModel account, ref string timeStampPrecise, MessageType messageType, string pageId)
        //{
        //    IncommingPageMessageResponseHandler incommingPageMessageResponseHandler = null;

        //    FdRequestParameter objFdRequestParameter = new FdRequestParameter();
        //    objFdRequestParameter.UrlParameters.Add("dpr", "1");
        //    try
        //    {
        //        string url = objFdRequestParameter.GenerateUrl(FdConstants.GetMessagesUrl);

        //        int limit = 21;
        //        if (!string.IsNullOrEmpty(timeStampPrecise))
        //            limit = 22;

        //        string[] array = new string[] { "INBOX" };
        //        FdPublisherJsonElement jsonelement = new FdPublisherJsonElement()
        //        {
        //            o0 = new FdPublisherJsonElement()
        //            {
        //                DocId = "2276493972392017",
        //                QueryParams = new FdPublisherJsonElement()
        //                {
        //                    Limit = limit,
        //                    Before = timeStampPrecise,
        //                    Tags = array,
        //                    IsWorkUser = false,
        //                    IncludeDeliveryReceipts = true,
        //                    IncludeSeqID = false,
        //                }
        //            },
        //        };

        //        var jsonValue = Uri.EscapeDataString(objFdRequestParameter.GetJsonString(jsonelement));

        //        objFdRequestParameter.PostDataParameters.Add("batch_name", "MessengerGraphQLThreadlistFetcher");
        //        objFdRequestParameter.PostDataParameters.Add("__req", "m");
        //        objFdRequestParameter.PostDataParameters.Add("av", pageId);
        //        objFdRequestParameter.PostDataParameters.Add("queries", jsonValue);
        //        objFdRequestParameter = CommonPostDataParameters(account, objFdRequestParameter);
        //        var postData = objFdRequestParameter.GetPostDataFromParameters();

        //        var request = _httpHelper.GetRequestParameter();
        //        request.ContentType = FdConstants.ContentType;
        //        _httpHelper.SetRequestParameter(request);

        //        var messageDetailsResponse = _httpHelper.PostRequest(url, postData);

        //        incommingPageMessageResponseHandler = new IncommingPageMessageResponseHandler(account, messageDetailsResponse, ref timeStampPrecise, pageId);

        //        return incommingPageMessageResponseHandler;
        //    }
        //    catch (Exception ex)
        //    {
        //        ex.DebugLog();
        //        return incommingPageMessageResponseHandler;
        //    }
        //}

        //public bool LikeWebPostComment(DominatorAccountModel account, string commentId, ReactionType reactionType)
        //{
        //    FdRequestParameter fdRequestParameter = new FdRequestParameter();

        //    fdRequestParameter.UrlParameters.Add("action_like", "true");
        //    fdRequestParameter.UrlParameters.Add("av", account.AccountBaseModel.UserId);
        //    fdRequestParameter.UrlParameters.Add("dpr", "1");
        //    string likeUrl = fdRequestParameter.GenerateUrl(FdConstants.WebpostLike);

        //    fdRequestParameter.PostDataParameters.Add("app_id", "116656161708917");
        //    fdRequestParameter.PostDataParameters.Add("comment_id", commentId);
        //    fdRequestParameter.PostDataParameters.Add("locale", "en_US");
        //    fdRequestParameter.PostDataParameters.Add("__req", "4");
        //    fdRequestParameter = CommonPostDataParameters(account, fdRequestParameter);

        //    var postdata = fdRequestParameter.GetPostDataFromParameters();
        //    var request = _httpHelper.GetRequestParameter();
        //    request.ContentType = FdConstants.ContentType;
        //    _httpHelper.SetRequestParameter(request);

        //    var likeResponse = _httpHelper.PostRequest(likeUrl, postdata).Response;

        //    return likeResponse.Contains(commentId) ? true : false;
        //}



        //public WebPostCommentLikerResponseHandler GetPostCommentorForWebPage
        //  (DominatorAccountModel account, string postUrl, WebPostCommentLikerResponseHandler webPageCommentLikerResponseHandler)
        //{

        //    FdRequestParameter objFdRequestParameter = new FdRequestParameter();

        //    string url = postUrl.Contains(FdConstants.FbHomeUrl)
        //        ? postUrl
        //        : $"{FdConstants.FbHomeUrl}{postUrl}";

        //    string unUrl = Uri.UnescapeDataString(url);

        //    string href = FdRegexUtility.FirstMatchExtractor(unUrl, "href=(.*?)?fb");
        //    string commentId = FdRegexUtility.FirstMatchExtractor(unUrl, ("fb_comment_id=(.*?)&"));
        //    string appId = "116656161708917";

        //    //Reply to Comments
        //    if (webPageCommentLikerResponseHandler == null)
        //    {
        //        objFdRequestParameter.UrlParameters.Add("app_id", appId);
        //        objFdRequestParameter.UrlParameters.Add("channel", "");
        //        objFdRequestParameter.UrlParameters.Add("container_width", "892");
        //        objFdRequestParameter.UrlParameters.Add("fb_comment_id", commentId);
        //        objFdRequestParameter.UrlParameters.Add("height", "100");
        //        objFdRequestParameter.UrlParameters.Add("href", href);
        //        objFdRequestParameter.UrlParameters.Add("locale", "en_US");
        //        objFdRequestParameter.UrlParameters.Add("sdk", "joey");
        //        objFdRequestParameter.UrlParameters.Add("width", "900");

        //        var request = _httpHelper.GetRequestParameter();
        //        request.ContentType = FdConstants.ContentType;
        //        _httpHelper.SetRequestParameter(request);

        //        string replyUrl = objFdRequestParameter.GenerateUrl(FdConstants.PluginFeedBackUrl);

        //        var postUrlResponse = _httpHelper.GetRequest(replyUrl);

        //        return new WebPostCommentLikerResponseHandler(postUrlResponse, commentId, account);
        //    }
        //    else
        //    {
        //        string afterCursor = webPageCommentLikerResponseHandler.AfterCursor;

        //        string paginationUrl = FdConstants.WebPostComment + commentId + "/pager/?dpr=1 ";
        //        objFdRequestParameter.PostDataParameters.Clear();
        //        objFdRequestParameter.PostDataParameters.Add("app_id", appId);
        //        objFdRequestParameter.PostDataParameters.Add("limit", "10");
        //        objFdRequestParameter.PostDataParameters.Add("after_cursor", afterCursor);
        //        objFdRequestParameter = CommonPostDataParameters(account, objFdRequestParameter);

        //        var postdata = objFdRequestParameter.GetPostDataFromParameters();
        //        var request = _httpHelper.GetRequestParameter();
        //        request.ContentType = FdConstants.ContentType;
        //        _httpHelper.SetRequestParameter(request);

        //        var postResponse = _httpHelper.PostRequest(paginationUrl, postdata);

        //        return new WebPostCommentLikerResponseHandler(postResponse, commentId, account);
        //    }
        //}

        public FdRequestParameter CommonPostDataParameters(DominatorAccountModel account, FdRequestParameter fdRequestParameter)
        {

            string fbDtsg = Uri.UnescapeDataString(account.SessionId);

            fdRequestParameter.PostDataParameters.Add("__user", account.AccountBaseModel.UserId);
            fdRequestParameter.PostDataParameters.Add("__a", "1");
            fdRequestParameter.PostDataParameters.Add("__dyn", FdConstants.DynParameter);
            //fdRequestParameter.PostDataParameters.Add("__be", "1");
            fdRequestParameter.PostDataParameters.Add("fb_dtsg", fbDtsg);
            fdRequestParameter.PostDataParameters.Add("jazoest", FdConstants.JazoestParameterGender);

            return fdRequestParameter;
        }

        public FdRequestParameter CommonUrlParameters(DominatorAccountModel account, FdRequestParameter fdRequestParameter)
        {

            string fbDtsg = Uri.UnescapeDataString(account.SessionId);

            fdRequestParameter.UrlParameters.Add("__user", account.AccountBaseModel.UserId);
            fdRequestParameter.UrlParameters.Add("__a", "1");
            fdRequestParameter.UrlParameters.Add("__dyn", FdConstants.DynParameter);
            fdRequestParameter.UrlParameters.Add("__be", "1");
            fdRequestParameter.UrlParameters.Add("fb_dtsg", fbDtsg);
            fdRequestParameter.UrlParameters.Add("jazoest", FdConstants.JazoestParameterGender);

            return fdRequestParameter;
        }

        public AccountMarketplaceDetailsHandler GetAccountMarketPlaceDetails(DominatorAccountModel account)
        {
            try
            {

                FdRequestParameter requestParameters = new FdRequestParameter();

                requestParameters.PostDataParameters.Add("av", account.AccountBaseModel.UserId);
                requestParameters.PostDataParameters.Add("fb_api_caller_class", "RelayModern");
                requestParameters.PostDataParameters.Add("variables", "{}");
                requestParameters.PostDataParameters.Add("doc_id", "2023846941060089");
                requestParameters = CommonPostDataParameters(account, requestParameters);
                string url = FdConstants.PageMembersUrl;

                byte[] postDataBytes = requestParameters.GeneratePostData();

                var postRequestParameter = _httpHelper.GetRequestParameter();
                postRequestParameter.ContentType = FdConstants.ContentType;

                _httpHelper.SetRequestParameter(postRequestParameter);

                var categoryResponse = _httpHelper.PostRequest(url, postDataBytes);

                return new AccountMarketplaceDetailsHandler(categoryResponse);

            }
            catch (Exception)
            {
            }

            return null;
        }

        public AccountMarketplaceDetailsHandler ChangeMarketplaceLocation(DominatorAccountModel account,
            string location, MarketplaceFilterModel marketplaceFilterModel)
        {
            try
            {

                var locationDictionary =
                    ExecuteMarketplaceLocationValues(account, location).CityList;

                if (locationDictionary.Count == 0)
                {
                    if (location.Contains(','))
                    {
                        foreach (var locationCSValue in location.Split(',').ToList())
                        {
                            locationDictionary =
                                ExecuteMarketplaceLocationValues(account, locationCSValue).CityList;

                            if (locationDictionary.Count == 0)
                            {
                                foreach (var locationSSValue in locationCSValue.Split(' ').ToList())
                                {
                                    locationDictionary =
                                        ExecuteMarketplaceLocationValues(account, locationSSValue).CityList;

                                    if (locationDictionary.Count > 0)
                                        break;
                                }
                            }

                            if (locationDictionary.Count > 0)
                                break;
                        }
                    }
                }

                var locationDetails = locationDictionary.FirstOrDefault();

                string marketplaceurl = FdConstants.MarketplaceGraphUrl;

                var locationVariables = FdConstants.ChangeMarketplaceLocationUrl(account.AccountBaseModel.UserId,
                    locationDetails.Item3, locationDetails.Item4);

                FdRequestParameter objChangeLocationRequestParameters = new FdRequestParameter();
                objChangeLocationRequestParameters.PostDataParameters.Add("av", account.AccountBaseModel.UserId);
                objChangeLocationRequestParameters.PostDataParameters.Add("__req", "1a");
                objChangeLocationRequestParameters.PostDataParameters.Add("dpr", "1");
                objChangeLocationRequestParameters.PostDataParameters.Add("fb_api_caller_class", "RelayModern");
                objChangeLocationRequestParameters.PostDataParameters.Add("variables", Uri.EscapeDataString(locationVariables));
                objChangeLocationRequestParameters.PostDataParameters.Add("doc_id", "1588712214546036");

                CommonPostDataParameters(account, objChangeLocationRequestParameters);

                var locationParameter = _httpHelper.GetRequestParameter();
                locationParameter.ContentType = FdConstants.ContentType;
                _httpHelper.SetRequestParameter(locationParameter);

                var postDataTargetLocation = objChangeLocationRequestParameters.GetPostDataFromParameters();

                if (marketplaceFilterModel.IsLocationDistanceChecked)
                    ChangeLocationDistance(account, marketplaceFilterModel.SelectedLocationDistance);

                _httpHelper.PostRequest(marketplaceurl, postDataTargetLocation);

                return GetAccountMarketPlaceDetails(account);

            }
            catch (Exception)
            {
                //ex.DebugLog();
            }

            return null;
        }

        private void ChangeLocationDistance(DominatorAccountModel account, string locationDistance)
        {

            try
            {
                string marketplaceurl = FdConstants.MarketplaceGraphUrl;

                var locationDistanceVariables = FdConstants.LocationDistanceVariables(account.AccountBaseModel.UserId, locationDistance);

                FdRequestParameter objChangeLocationRequestParameters = new FdRequestParameter();
                objChangeLocationRequestParameters.PostDataParameters.Add("av", account.AccountBaseModel.UserId);
                objChangeLocationRequestParameters.PostDataParameters.Add("__req", "2k");
                objChangeLocationRequestParameters.PostDataParameters.Add("dpr", "1");
                objChangeLocationRequestParameters.PostDataParameters.Add("fb_api_caller_class", "RelayModern");
                objChangeLocationRequestParameters.PostDataParameters.Add("variables", Uri.EscapeDataString(locationDistanceVariables));
                objChangeLocationRequestParameters.PostDataParameters.Add("doc_id", "2025527690861217");

                CommonPostDataParameters(account, objChangeLocationRequestParameters);

                var locationParameter = _httpHelper.GetRequestParameter();
                locationParameter.ContentType = FdConstants.ContentType;
                _httpHelper.SetRequestParameter(locationParameter);

                var postDataTargetLocation = objChangeLocationRequestParameters.GetPostDataFromParameters();
                _httpHelper.PostApiRequest(marketplaceurl, postDataTargetLocation);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

        }

        public MarketplaceScraperResponseHandler GetProductListFromMarketplace(DominatorAccountModel account
            , string keyword, AccountMarketplaceDetailsHandler accountMarketPlaceDetails, MarketplaceFilterModel filterModel,
            MarketplaceScraperResponseHandler responseHandler)
        {
            try
            {
                FdRequestParameter objChangeLocationRequestParameters = new FdRequestParameter();
                string marketplaceurl = FdConstants.MarketplaceGraphUrl;

                var marketplaceVariables = responseHandler == null
                    ? GetMarketplaceVariables(account, keyword, filterModel, accountMarketPlaceDetails, null, false)
                    : GetMarketplaceVariables(account, keyword, filterModel, accountMarketPlaceDetails,
                        responseHandler.SessionCookies, true);

                objChangeLocationRequestParameters.PostDataParameters.Add("av", account.AccountBaseModel.UserId);
                objChangeLocationRequestParameters.PostDataParameters.Add("__req", "1t");
                objChangeLocationRequestParameters.PostDataParameters.Add("dpr", "1");
                objChangeLocationRequestParameters.PostDataParameters.Add("variables", Uri.EscapeDataString(marketplaceVariables));
                objChangeLocationRequestParameters.PostDataParameters.Add("fb_api_caller_class", "RelayModern");
                objChangeLocationRequestParameters.PostDataParameters.Add("fb_api_req_friendly_name", "MarketplaceSearchResultsPageContainerNewQuery");
                objChangeLocationRequestParameters.PostDataParameters.Add("doc_id", "2657577624314699");

                CommonPostDataParameters(account, objChangeLocationRequestParameters);

                var locationParameter = _httpHelper.GetRequestParameter();
                locationParameter.ContentType = FdConstants.ContentType;
                _httpHelper.SetRequestParameter(locationParameter);

                var postDataTargetLocation = objChangeLocationRequestParameters.GetPostDataFromParameters();
                var productDetails = _httpHelper.PostRequest(marketplaceurl, postDataTargetLocation);

                return new MarketplaceScraperResponseHandler(productDetails);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return null;
        }

        public string GetMarketplaceVariables(DominatorAccountModel account, string keyword,
            MarketplaceFilterModel filterModel, AccountMarketplaceDetailsHandler accountMarketPlaceDetails
            , string cursor, bool isPagination = false)
        {
            try
            {
                var filterRadiusKm = filterModel.IsLocationDistanceChecked ? int.Parse(FdFunctions.GetIntegerOnlyString
                    (filterModel.SelectedLocationDistance)) : 20;

                FdRequestParameter objParameter = new FdRequestParameter();

                var categoryDetails = new long[100];

                int? count = null;

                if (isPagination)
                    count = 50;

                categoryDetails = FdConstants.MarketplaceCategoryValues(filterModel.SelectedMainCategory);


                FdMarketplaceJsonElements objNewJsonElements = new FdMarketplaceJsonElements()
                {
                    Count = count,
                    Cursor = isPagination ? cursor : null,
                    FeedItemWidth = 246,
                    VerticalPhotoWidth = 40.ToString(),
                    MerchantLogoScale = null,
                    MarketplaceParameters = new FdMarketplaceJsonElements()
                    {
                        BqfElement = new FdMarketplaceJsonElements()
                        {
                            CallSite = "COMMERCE_MKTPLACE_WWW",
                            Query = keyword
                        },
                        BrowseRequestParam = new FdMarketplaceJsonElements()
                        {
                            FilterLocationId = accountMarketPlaceDetails.LocationDetails.LocationId.ToString(),
                            CommerceSearchSortBy = FdConstants.MarketPlaceSortOptionValue(filterModel.SelectedSortOption),
                            FilterRadiusKm = filterRadiusKm == 0 ? null : filterRadiusKm.ToString(),
                            PriceLowerBound = filterModel.PriceBetween.StartValue == 0 ? null : filterModel.PriceBetween.StartValue.ToString(),
                            PriceUpperBound = filterModel.PriceBetween.EndValue == 0 ? null : filterModel.PriceBetween.EndValue.ToString(),
                            SearchRpCategoryId = categoryDetails
                        },
                        CustomRequestParams = new FdMarketplaceJsonElements()
                        {
                            Surface = "SEARCH",
                            SearchVertical = "C2C"
                        }

                    }
                };

                return objNewJsonElements == null
                    ? string.Empty
                    : objParameter.GetJsonString(objNewJsonElements);
            }
            catch (Exception)
            {

            }

            return string.Empty;
        }

        public MarketplaceLocationResponseHandler ExecuteMarketplaceLocationValues(DominatorAccountModel account,
            string location)
        {
            FdRequestParameter requestParameters = new FdRequestParameter();

            string[] array = new string[] { };

            FdPublisherJsonElement jsonElement = new FdPublisherJsonElement()
            {
                QueryParams = new FdPublisherJsonElement()
                {
                    ViewerCoordinates = array,
                    Provider = "facebook",
                    SearchType = "place_typeahead",
                    IntegrationStrategy = "string_match",
                    ResultOrdering = "interleave",
                    Caller = "graphql",
                    PageCategory = new FdPublisherJsonElement()
                    {
                        Zero = "city",
                        One = "neighborhood",
                        Two = "postal_code"
                    },
                    GeocodeFallback = false,
                    Query = location,
                },
                MaxResults = 5,
                PhotoWidth = 50,
                PhotoHeight = 50
            };

            string variableData = requestParameters.GetJsonString(jsonElement);

            requestParameters.UrlParameters.Add("query_id", "201531893907334");
            requestParameters.UrlParameters.Add("variables", variableData);

            string locationUrl = $" {FdConstants.FbHomeUrl}webgraphql/query/";

            locationUrl = requestParameters.GenerateUrl(locationUrl);

            requestParameters = CommonPostDataParameters(account, requestParameters);

            byte[] postData = requestParameters.GeneratePostData();

            var locationRequest = _httpHelper.GetRequestParameter();

            locationRequest.ContentType = FdConstants.ContentType;

            _httpHelper.SetRequestParameter(locationRequest);

            var locationResponse = _httpHelper.PostRequest(locationUrl, postData);

            return new MarketplaceLocationResponseHandler(locationResponse, location);
        }

        public EventCreaterResponseHandler EventCreater(DominatorAccountModel account, EventCreaterManagerModel eventCreaterManagerModel)
        {
            if (eventCreaterManagerModel.EventStartDate < DateTime.Now)
                return new EventCreaterResponseHandler(new ResponseParameter(), eventCreaterManagerModel)
                {
                    ErrorMsg = "Date Should be more then current Time",
                };

            string location = string.IsNullOrEmpty(eventCreaterManagerModel.EventLocation) ? string.Empty : eventCreaterManagerModel.IsSelectLocation
               ? GetLocationsList(account, eventCreaterManagerModel.EventLocation) : eventCreaterManagerModel.EventLocation;

            if (location == null)
                location = eventCreaterManagerModel.EventLocation;

            var dateTimeFormats = eventCreaterManagerModel.EventStartDate.GetDateTimeFormats();

            TimeSpan ts;
            string startDtae = dateTimeFormats[0];
            TimeSpan.TryParse(dateTimeFormats[108], out ts);
            //time is required timespanformate
            string startTime = ts.TotalSeconds.ToString();

            dateTimeFormats = eventCreaterManagerModel.EventEndDate.GetDateTimeFormats();
            string endDate = dateTimeFormats[0];
            TimeSpan.TryParse(dateTimeFormats[108], out ts);
            //time is required timespanformate
            string endTime = ts.TotalSeconds.ToString();

            string locationId = !string.IsNullOrEmpty(location)
                ? GetLocationIdForEvent(account, location)
                : string.Empty;

            string timeZone = GetTimeZoneForEvent(account, locationId, startDtae).Replace("\\", string.Empty);

            FdRequestParameter fdRequestParameter = new FdRequestParameter();

            PublisherParameter publisherParameter = new PublisherParameter
            {
                WaterfallId = FdFunctions.GetRandomHexNumber(32).ToLower()
            };
            var mediaId = new Dictionary<string, string>();

            var postDetails = new PublisherPostlistModel();

            if (eventCreaterManagerModel.FbMultiMediaModel.MediaPaths.Count != 0)
                eventCreaterManagerModel.FbMultiMediaModel.MediaPaths?.ForEach
                (media => postDetails.MediaList.Add(media.MediaPath));

            if (postDetails.MediaList.Count > 0)
            {
                GlobusLogHelper.log.Info(Log.CustomMessage, account.AccountBaseModel.AccountNetwork, account.AccountBaseModel.UserName, "", "Uploading media files and getting media Ids");

                mediaId = UploadImageAndGetMediaId(postDetails, account, account.AccountBaseModel.UserId, publisherParameter.WaterfallId, "OwnWall", "", "Event");
            }

            string title = char.ToUpper(eventCreaterManagerModel.EventName[0]) + eventCreaterManagerModel.EventName.Substring(1);
            string description = eventCreaterManagerModel.EventDescription;

            FdPublisherJsonElement jsonElement = new FdPublisherJsonElement()
            {
                Ref = "2",
                Source = eventCreaterManagerModel.EventType == DominatorHouseCore.Enums.FdQuery.EventType.CreatePrivateEvent.ToString() ? "2" : "1",
                SidCreate = "2444552616",
                ActionHistory = "[{\"mechanism\":\"user_create_dialog\",\"surface\":\"create_dialog\",\"extra_data\":\"[]\"},{\"mechanism\":\"main_list\",\"surface\":\"dashboard\",\"extra_data\":\"{\\\"dashboard_filter\\\":\\\"upcoming\\\"}\"},{\"mechanism\":\"user_create_dialog\",\"surface\":\"create_dialog\",\"extra_data\":\"[]\"},{\"mechanism\":\"surface\",\"surface\":\"permalink\",\"extra_data\":\"[]\"},{\"mechanism\":\"user_create_dialog\",\"surface\":\"create_dialog\",\"extra_data\":\"[]\"},{\"mechanism\":\"surface\",\"surface\":\"permalink\",\"extra_data\":\"[]\"},{\"surface\":\"create_dialog\",\"mechanism\":\"user_create_dialog\",\"extra_data\":[]}]",
                HasSource = true,
            };
            var variables = fdRequestParameter.GetJsonString(jsonElement);

            fdRequestParameter.UrlParameters.Add("title", title);

            if (!string.IsNullOrEmpty(description))
                fdRequestParameter.UrlParameters.Add("description", description);
            if (!string.IsNullOrEmpty(location))
            {
                fdRequestParameter.UrlParameters.Add("location", location);
                fdRequestParameter.UrlParameters.Add("location_id", locationId);
            }
            fdRequestParameter.UrlParameters.Add("timezone", timeZone);

            if (mediaId.Count > 0)
            {
                string coverId = mediaId.First().Key;
                fdRequestParameter.UrlParameters.Add(mediaId.First().Value.Equals("Image") ? "cover_id" : "cover_video_id", coverId);
            }

            fdRequestParameter.UrlParameters.Add("cover_focus[x]", "0.5");
            fdRequestParameter.UrlParameters.Add("cover_focus[y]", "0.5");
            fdRequestParameter.UrlParameters.Add("start_date", startDtae);
            fdRequestParameter.UrlParameters.Add("start_time", startTime);
            if (!string.IsNullOrEmpty(endDate) || !string.IsNullOrEmpty(endTime))
            {
                fdRequestParameter.UrlParameters.Add("end_date", endDate);
                fdRequestParameter.UrlParameters.Add("end_time", endTime);
            }

            fdRequestParameter.UrlParameters.Add("acontext", variables);
            fdRequestParameter.UrlParameters.Add("event_ent_type",
                eventCreaterManagerModel.EventType == DominatorHouseCore.Enums.FdQuery.EventType.CreatePrivateEvent.ToString() ? "1" : "2");

            fdRequestParameter.UrlParameters.Add("guests_can_invite_friends",
                eventCreaterManagerModel.IsGuestCanInviteFriends ? "true" : "false");

            fdRequestParameter.UrlParameters.Add("guest_list_enabled",
                eventCreaterManagerModel.IsShowGuestList ? "true" : "false");

            if (eventCreaterManagerModel.EventType == DominatorHouseCore.Enums.FdQuery.EventType.CreatePublicEvent.ToString())
            {
                fdRequestParameter.UrlParameters.Add("post_approval_required", eventCreaterManagerModel.IsPostMustApproved ? "true" : "false");
                fdRequestParameter.UrlParameters.Add("only_admins_can_post", eventCreaterManagerModel.IsAnyOneCanPostForAllPost ? "false" : "true");
            }

            fdRequestParameter.UrlParameters.Add("is_host_collect_payment", "false");

            fdRequestParameter.UrlParameters.Add("save_as_draft", "false");
            fdRequestParameter.UrlParameters.Add("friend_birthday_prompt_xout_id", "");
            fdRequestParameter.UrlParameters.Add("cover_video_offset_type", "0");
            fdRequestParameter.UrlParameters.Add("category_id", eventCreaterManagerModel.CategoryId);
            fdRequestParameter.UrlParameters.Add("cover_video_offset", "0");
            fdRequestParameter.UrlParameters.Add("dialog_entry_point", "others");
            fdRequestParameter.UrlParameters.Add("interception_flow_type", "dialog");
            string url = fdRequestParameter.GenerateUrl($"{FdConstants.FbHomeUrl}ajax/create/event/submit/");
            url = url.Replace("+", "%20");

            fdRequestParameter.PostDataParameters.Add("__req", "2e");
            CommonPostDataParameters(account, fdRequestParameter);

            var postdata = fdRequestParameter.GetPostDataFromParameters();
            var request = _httpHelper.GetRequestParameter();
            request.ContentType = FdConstants.ContentType;
            _httpHelper.SetRequestParameter(request);

            _delayService.ThreadSleep(1000);
            IResponseParameter responseParameter = _httpHelper.PostRequest(url, postdata);

            return new EventCreaterResponseHandler(responseParameter, eventCreaterManagerModel);

        }

        private string GetLocationsList(DominatorAccountModel account, string eventLocation)
        {
            var variables =
                "{\"query_params\":{\"query\":\"" + eventLocation + "\",\"viewer_coordinates\":{},\"provider\":\"here_thrift\",\"search_type\":\"street_place_typeahead\",\"integration_strategy\":\"string_match\",\"result_ordering\":\"interleave\",\"caller\":\"events_creation\",\"geocode_fallback\":false},\"max_results\":10,\"photo_width\":50,\"photo_height\":50}";

            FdRequestParameter fdRequestParameter = new FdRequestParameter();
            fdRequestParameter.UrlParameters.Add("query_id", "201531893907334");
            fdRequestParameter.UrlParameters.Add("variables", variables);
            string url = fdRequestParameter.GenerateUrl($"{FdConstants.FbHomeUrl}webgraphql/query/");

            CommonPostDataParameters(account, fdRequestParameter);
            var postdata = fdRequestParameter.GeneratePostData();

            IResponseParameter responseParameter = _httpHelper.PostRequest(url, postdata);

            var responseHandler = new LocationDetailsResponseHandler(responseParameter, eventLocation);

            if (string.IsNullOrEmpty(responseHandler.Location))
            {
                string location = eventLocation;

                location = location.Replace(" ", "+");

                url = $"https://www.google.co.in/search?q={location}+coordinate&oq={location}+coordinate";

                responseParameter = _httpHelper.GetRequest(url);

                var LatLangResponseHandler = new LattitudeAndLangitudeResponseHandler(responseParameter);


                return responseHandler.listOfLatLon.FirstOrDefault(x =>
                           x.Item2.StartsWith(LatLangResponseHandler.Latitude)
                           && x.Item3.StartsWith(LatLangResponseHandler.Longitude))?.Item1;

            }

            return responseHandler.Location;
        }

        private string GetTimeZoneForEvent(DominatorAccountModel accountModel, string locationId, string startDate)
        {
            FdRequestParameter objFdRequestParameter = new FdRequestParameter();
            objFdRequestParameter.PostDataParameters.Add("place_id", locationId);
            objFdRequestParameter.PostDataParameters.Add("date_str", startDate);

            string url = $"{FdConstants.FbHomeUrl}ajax/plans/create/timezone.php?dpr=1";
            objFdRequestParameter = CommonPostDataParameters(accountModel, objFdRequestParameter);
            var postData = objFdRequestParameter.GetPostDataFromParameters();
            var request = _httpHelper.GetRequestParameter();
            request.ContentType = FdConstants.ContentType;
            _httpHelper.SetRequestParameter(request);

            return FdRegexUtility.FirstMatchExtractor(_httpHelper.PostRequest(url, postData).Response, "tz_identifier\":\"(.*?)\"");

        }

        private string GetLocationIdForEvent(DominatorAccountModel accountModel, string location)
        {
            FdRequestParameter objFdRequestParameter = new FdRequestParameter();

            objFdRequestParameter.UrlParameters.Add("query_id", "201531893907334");
            var viewerCoordinates = new string[0];

            FdPublisherJsonElement jsonElement = new FdPublisherJsonElement()
            {
                QueryParams = new FdPublisherJsonElement()
                {
                    Query = location,
                    ViewerCoordinates = viewerCoordinates,
                    Provider = "here_thrift",
                    SearchType = "street_place_typeahead",
                    IntegrationStrategy = "string_match",
                    ResultOrdering = "interleave",
                    Caller = "events_creation",
                    GeocodeFallback = false,
                },
                MaxResults = 10,
                PhotoWidth = 50,
                PhotoHeight = 50,
            };
            var variables = objFdRequestParameter.GetJsonString(jsonElement);

            objFdRequestParameter.UrlParameters.Add("variables", variables);
            objFdRequestParameter.UrlParameters.Add("dpr", "1");

            string url = objFdRequestParameter.GenerateUrl($"{FdConstants.FbHomeUrl}webgraphql/query/");
            objFdRequestParameter.PostDataParameters.Add("__req", "4m");
            objFdRequestParameter = CommonPostDataParameters(accountModel, objFdRequestParameter);
            var postData = objFdRequestParameter.GetPostDataFromParameters();

            var request = _httpHelper.GetRequestParameter();
            request.ContentType = FdConstants.ContentType;
            _httpHelper.SetRequestParameter(request);

            return FdRegexUtility.FirstMatchExtractor(_httpHelper.PostRequest(url, postData).Response, "city\":{\"id\":\"(.*?)\"");

        }

        public bool IsGroupAdmin(DominatorAccountModel accountModel, string groupUrl)
            => _httpHelper.GetRequest(groupUrl).Response.Contains("\"label\":\"Moderate group\"");

        public GetGroupMembersResponseHandler GetAllGroupMembers(DominatorAccountModel accountModel, string groupId)
        {

            _httpHelper.GetRequest(FdConstants.FbHomeUrl + groupId);

            FdRequestParameter objFdRequestParameter = new FdRequestParameter();
            //Getting values from 
            string url = objFdRequestParameter.GenerateUrl($"{FdConstants.FbHomeUrl}api/graphql/");

            FdPublisherJsonElement jsonElement = new FdPublisherJsonElement()
            {
                GroupID = groupId,
                Count = 1000000,
                Cursor = ""
            };

            var jsonString = objFdRequestParameter.GetJsonString(jsonElement);

            objFdRequestParameter.PostDataParameters.Add("av", accountModel.AccountBaseModel.UserId);
            objFdRequestParameter.PostDataParameters.Add("fb_api_caller_class", "RelayModern");
            objFdRequestParameter.PostDataParameters.Add("fb_api_req_friendly_name", "GroupChatAudienceListContainerRootPaginationQuery");
            objFdRequestParameter.PostDataParameters.Add("variables", jsonString);
            objFdRequestParameter.PostDataParameters.Add("__req", "2u");
            objFdRequestParameter.PostDataParameters.Add("doc_id", "2098272000269269");

            objFdRequestParameter = CommonPostDataParameters(accountModel, objFdRequestParameter);

            var postData = objFdRequestParameter.GetPostDataFromParameters();

            var request = _httpHelper.GetRequestParameter();
            request.ContentType = FdConstants.ContentType;
            _httpHelper.SetRequestParameter(request);

            var groupMembers = _httpHelper.PostRequest(url, postData);

            return new GetGroupMembersResponseHandler(groupMembers);

        }

        public MakeAdminResponseHandler MakeGroupAdmin(DominatorAccountModel accountModel, string groupId, string userId)
        {
            FdRequestParameter objFdRequestParameter = new FdRequestParameter();

            objFdRequestParameter.UrlParameters.Add("user_id", userId);
            objFdRequestParameter.UrlParameters.Add("group_id", groupId);
            objFdRequestParameter.UrlParameters.Add("source", "profile_browser");
            objFdRequestParameter.UrlParameters.Add("action", "invite_admin");
            string url = objFdRequestParameter.GenerateUrl($"{FdConstants.FbHomeUrl}ajax/groups/admins_get.php/");

            objFdRequestParameter.PostDataParameters.Add("group_id", groupId);
            objFdRequestParameter.PostDataParameters.Add("user_id", userId);
            objFdRequestParameter.PostDataParameters.Add("action", "invite_admin");
            objFdRequestParameter.PostDataParameters.Add("source", "profile_browser");
            objFdRequestParameter = CommonPostDataParameters(accountModel, objFdRequestParameter);

            var postData = objFdRequestParameter.GetPostDataFromParameters();

            var request = _httpHelper.GetRequestParameter();
            request.ContentType = FdConstants.ContentType;
            _httpHelper.SetRequestParameter(request);

            _httpHelper.PostRequest(url, postData);

            objFdRequestParameter.UrlParameters.Clear();
            objFdRequestParameter.PostDataParameters.Clear();
            objFdRequestParameter.UrlParameters.Add("user_id", userId);
            objFdRequestParameter.UrlParameters.Add("group_id", groupId);
            objFdRequestParameter.UrlParameters.Add("source", "profile_browser");
            objFdRequestParameter.UrlParameters.Add("operation", "confirm_invite_admin");
            url = objFdRequestParameter.GenerateUrl($"{FdConstants.FbHomeUrl}ajax/groups/admin_post/");

            objFdRequestParameter.PostDataParameters.Add("invite_admin", "1");
            objFdRequestParameter = CommonPostDataParameters(accountModel, objFdRequestParameter);
            postData = objFdRequestParameter.GetPostDataFromParameters();

            var redirectGroupAdminResponse = _httpHelper.PostRequest(url, postData);

            return new MakeAdminResponseHandler(redirectGroupAdminResponse);
        }

        public bool GetUserLikedPages
            (DominatorAccountModel accountModel, IResponseHandler mobileUserFanPageScraper
            , FacebookUser user, string pageId = "")
        {
            pageId = Regex.Split(pageId, FdConstants.FbHomeUrl)[1];

            FdRequestParameter fdRequestParameter = new FdRequestParameter();

            var userFanPageresponse = _httpHelper.GetRequest($"https://m.facebook.com/timeline/app_collection/?collection_token={user.UserId}%3A2409997254%3A96");
            var userFanPageresponseHandler = new MobileUserFanPageScraperResponseHandler(userFanPageresponse);


            while (!userFanPageresponse.Response.Contains(pageId) && !string.IsNullOrEmpty(userFanPageresponseHandler.PageletData))
            {
                fdRequestParameter = new FdRequestParameter();
                fdRequestParameter = CommonPostDataParameters(accountModel, fdRequestParameter);

                fdRequestParameter.AddPostDataParameters("m_sess", "");
                fdRequestParameter.AddPostDataParameters("__ajax__", userFanPageresponseHandler.ObjFdScraperResponseParameters.AjaxToken);

                var postData = fdRequestParameter.GetPostDataFromParameters();

                var request = _httpHelper.GetRequestParameter();
                request.ContentType = FdConstants.ContentType;
                _httpHelper.SetRequestParameter(request);

                userFanPageresponse = _httpHelper.PostRequest($"https://m.facebook.com/{userFanPageresponseHandler.PageletData}", postData);

                userFanPageresponseHandler = new MobileUserFanPageScraperResponseHandler(userFanPageresponse);

                Thread.Sleep(10);
            }

            if (userFanPageresponse.Response.Contains(pageId))
                return true;

            return false;
        }

        //public bool CommnentOnWebPostComment(DominatorAccountModel account, FdPostCommentDetails fdPostCommentDetails, string comment)
        //{
        //    FdRequestParameter fdRequestParameter = new FdRequestParameter();

        //    string commentId = fdPostCommentDetails.CommentId;

        //    fdRequestParameter.UrlParameters.Add("dpr", "1");
        //    fdRequestParameter.UrlParameters.Add("av", account.AccountBaseModel.UserId);
        //    string commentUrl = fdRequestParameter.GenerateUrl(FdConstants.WebPostReplyComment + fdPostCommentDetails.PostId + "/");

        //    fdRequestParameter.PostDataParameters.Add("app_id", "116656161708917");
        //    if (!string.IsNullOrEmpty(fdPostCommentDetails.CommenterName))
        //    {
        //        string commenterName = fdPostCommentDetails.CommenterName;
        //        string commenterId = fdPostCommentDetails.CommenterID;
        //        string space = Uri.EscapeDataString("  ");
        //        string text = $"{Uri.EscapeDataString("@")}[{Uri.EscapeDataString($"{commenterId}:{commenterName}")}]{space}{Uri.EscapeDataString(comment)}";
        //        fdRequestParameter.PostDataParameters.Add("text", text);
        //    }
        //    else
        //        fdRequestParameter.PostDataParameters.Add("text", Uri.EscapeDataString(comment));

        //    fdRequestParameter.PostDataParameters.Add("replied_to", commentId);
        //    fdRequestParameter.PostDataParameters.Add("locale", "en_US");
        //    fdRequestParameter.PostDataParameters.Add("__req", "4");
        //    fdRequestParameter = CommonPostDataParameters(account, fdRequestParameter);

        //    var postdata = fdRequestParameter.GetPostDataFromParameters();
        //    var request = _httpHelper.GetRequestParameter();
        //    request.ContentType = FdConstants.ContentType;
        //    _httpHelper.SetRequestParameter(request);

        //    var commentResponse = _httpHelper.PostRequest(commentUrl, postdata).Response;

        //    return commentResponse.Contains(commentId);

        //}

        //public IResponseHandler UpdateFriendsFromPageSync
        //(DominatorAccountModel account, FriendsUpdateResponseHandler responseHandler, CancellationToken token
        //    , List<string> lstPageId)
        //{
        //    return UpdateFriendsFromPage(account, responseHandler, token, lstPageId).Result;
        //}

        public IResponseHandler GetCommentReplies(DominatorAccountModel accountModel, FdPostCommentDetails commentDetails)
        {
            var responseParameter = _httpHelper.GetRequest($"{FdConstants.FbHomeUrl}{commentDetails.CommentId}");

            return new CommentRepliesResponseHandler(responseParameter, commentDetails);
        }
    }

}
