using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Request;
using DominatorHouseCore.Utility;
using EmbeddedBrowser;
using GramDominatorCore.GDLibrary.InstagramBrowser;
using GramDominatorCore.GDLibrary.Response;
using GramDominatorCore.GDModel;
using GramDominatorCore.GDUtility;
using GramDominatorCore.Request;
using GramDominatorCore.Response;
using GramDominatorCore.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NReco.VideoConverter;
using NReco.VideoInfo;
using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using ThreadUtils;
using static Newtonsoft.Json.JsonConvert;
using AssertFailedException = Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException;
using Cookie = System.Net.Cookie;

namespace GramDominatorCore.GDLibrary
{
    public interface IInstaFunction
    {
        BrowserWindow BrowserWindow { get; set; }
        BrowserWindow SecondaryBrowserWindow { get; set; }
        BrowserWindow fourthBrowserWindow { get; set; }
        BrowserWindow ThirdBrowserWindow { get; set; }
        void CloseBrowser();
        UsernameInfoIgResponseHandler GetProfileDetails(DominatorAccountModel _dominatorAccount);

        Task<UsernameInfoIgResponseHandler> ChangeProfilePictureAsync(DominatorAccountModel _dominatorAccount,
            AccountModel _Account, string image, string UploadId);

        Task<CommonIgResponseHandler> SetBiographyAsync(DominatorAccountModel _dominatorAccount, AccountModel _Account,
            string bioText);

        Task<UsernameInfoIgResponseHandler> EditProfileAsync(DominatorAccountModel _dominatorAccount,
            AccountModel _Account, string url, string phone, string fullName, string bio, string email, int gender,
            string username);

        Task<CheckUsernameResponse> CheckUsernameAsync(DominatorAccountModel _dominatorAccount, AccountModel _Account,
            string username);

        Task<CommentResponse> Comment(DominatorAccountModel _dominatorAccount, AccountModel accountModel,
            CancellationToken token, string mediaId,
            string comment, string replyCommentId = null, string module = "comments_v2", string userBread = null,
            string IdempotenceToken = null);

        CheckOffensiveCommentResponseHandler CheckOffensiveComment(DominatorAccountModel _dominatorAccount,
            AccountModel accountModel, CancellationToken token, string mediaId,
            string comment);

        Task<SendMessageIgResponseHandler> SendMessage(DominatorAccountModel _dominatorAccount, AccountModel accountModel,
            string userId,
            string message, string threadId, CancellationToken token);

        SendMessageIgResponseHandler SendMessageWithLink(DominatorAccountModel _dominatorAccount,
            CancellationToken token, string userId,
            string message, List<string> lstLinkUrls, string isAlreadyMessaged, AccountModel _Account = null,
            bool isUnitTest = false);

        FriendshipsResponse Follow(DominatorAccountModel _dominatorAccountModel, AccountModel _account,
            CancellationToken token, string userId, string mediaid = null);

        Task<FriendshipsResponse> FollowAsync(DominatorAccountModel _dominatorAccount, AccountModel _Account,
            CancellationToken token,
            string userId, string mediaId = null);


        FeedIgResponseHandler GetHashtagFeed(DominatorAccountModel _dominatorAccount, string hashtag,
            string maxid = null);


        Task<HashTagFeedIgResponseHandler> GetHashtagFeedForUserScraper(DominatorAccountModel _dominatorAccount,
            AccountModel _Account, string hashtag, CancellationToken token, int nextPageCount, bool IsRecent = false,
            string maxid = null, string NextMediaId = null);

        HashTagFeedIgResponseHandler GetHashtagFeedForRecentUserScraper(DominatorAccountModel _dominatorAccount,
            AccountModel _Account, string hashtag, int nextPageCount,
            string maxid = null, string NextMediaId = null);

        Task<FeedIgResponseHandlerAlternate> GetLocationFeedAlternate(DominatorAccountModel _dominatorAccount,
            AccountModel _Account, string locationId, CancellationToken token, string maxid = null, string page = null,string PostUrl=null);
        MediaCommentsIgResponseHandler GetMediaComments(DominatorAccountModel _dominatorAccount, string mediaId,
            CancellationToken token, string maxid = null);

        MediaLikersIgResponseHandler GetMediaLikers(DominatorAccountModel _dominatorAccount, string mediaId,
            CancellationToken token, string maxid = null);

        Task<SuggestedUsersIgResponseHandler> GetSuggestedUsers(DominatorAccountModel _dominatorAccount,
            AccountModel _Account, string userid, CancellationToken token, string max_id);

        UserFeedIgResponseHandler GetUserFeed(DominatorAccountModel
                _dominatorAccount, AccountModel _Account, string userId, CancellationToken token, string maxId = null,
            string minTimestamp = null, bool isNewUserBrowser = false);

        Task<UserFeedIgResponseHandler> GetUserFeedAsync(DominatorAccountModel _dominatorAccount, AccountModel _Account,
            string userId, CancellationToken token, string maxId = null, string minTimestamp = null,
            bool isNewUserBrowser = false);

        Task<FollowerAndFollowingIgResponseHandler> GetUserFollowers(DominatorAccountModel _dominatorAccount,
            string usernameId, CancellationToken token, string maxid = null, string ProfileID = "",bool IsWeb=false);

        Task<FollowerAndFollowingIgResponseHandler> GetUserFollowersAsync(DominatorAccountModel _dominatorAccount,
            string usernameId, CancellationToken token,string maxid = null, string ProfileID = "", bool IsWeb = false);

        Task<FollowerAndFollowingIgResponseHandler> GetUserFollowings(DominatorAccountModel _dominatorAccount,
            AccountModel accountModel, string usernameId, CancellationToken token, string QueryType = null,
            string maxid = null, string ProfileID = "", bool IsWeb = false);

        Task<FollowerAndFollowingIgResponseHandler> GetUserFollowingsAsync(DominatorAccountModel _dominatorAccount,
            AccountModel _Account, string usernameId, CancellationToken token, string QueryType = null,
            string maxid = null,string ProfileID="", bool IsWeb = false);

        Task<V2InboxResponse> Getv2Inbox(DominatorAccountModel _dominatorAccount, bool isPendingUsers = false,
            string cursorId = null);

        CommonIgResponseHandler AcceptMessageRequest(string threadId);

        Task<LikeResponse> Like(DominatorAccountModel _dominatorAccount, AccountModel _Account, CancellationToken token,
            string mediaId,
            string username, string userId, QueryInfo QueryInfo, bool isUnitTest = false);

        Task<CommonIgResponseHandler> SendSecurityCodeAsync(string challengeUrl, string challengeContext,
            string choiceNo, DominatorAccountModel _dominatorAccount, AccountModel _Account);

        Task<CommonIgResponseHandler> ActionBlockSendSecurityCodeAsync(string challengeUrl, string challengeContext,
            string choiceNo, DominatorAccountModel _dominatorAccount, AccountModel _Account);

        CommonIgResponseHandler ActionBlockSendSecurityCode(string challengeUrl, string challengeContext, string nonce_block,
            string choiceNo, DominatorAccountModel _dominatorAccount, AccountModel _Account);

        Task<CommonIgResponseHandler> SendSecurityCodeAsyncTwoFactorLogin(DominatorAccountModel dominatorAccountModel,
            AccountModel accountModel, string challengeUrl, string choiceNo);

        Task<CommonIgResponseHandler> SendSecurityCodeAsyncForSomeAccounts(string challengeUrl, string challengeContext,
            string choiceNo, DominatorAccountModel _dominatorAccount, AccountModel _Account);

        Task<LoginIgResponseHandler> SubmitChallengeAsync(DominatorAccountModel _dominatorAccount,
            AccountModel _Account, string challenge_context, string challengeUrl, string securityCode);

        Task<LoginIgResponseHandler> SubmitChallengeAsyncForDifferentAccounts(DominatorAccountModel _dominatorAccount,
            AccountModel _Account, string challenge_context, string challengeUrl, string securityCode);

        Task<LoginIgResponseHandler> ActionBlockSubmitChallengeAsync(DominatorAccountModel _dominatorAccount,
            AccountModel _Account, string challenge_context, string challengeUrl, string securityCode);

        Task<LoginIgResponseHandler> VerifyTwoFactorAccount(string challengeUrl, string securityCode);

        Task<SearchTagIgResponseHandler> SearchForTag(DominatorAccountModel _dominatorAccount, AccountModel _Account,
            string keyword, string[] excludeTagList = null);

        Task<SearchKeywordIgResponseHandler> SearchForkeyword(DominatorAccountModel _dominatorAccount, string keyword,
            CancellationToken token);

        UsernameInfoIgResponseHandler SearchUsername(DominatorAccountModel DominatorAccountModel, string username,
            CancellationToken token, bool isOtherUser = false);

        Task<UsernameInfoIgResponseHandler> SearchUsernameAsync(DominatorAccountModel _dominatorAccount,
            string username, CancellationToken token, bool isOtherUser = false);

        Task<UsernameInfoIgResponseHandler> SearchUserInfoById(DominatorAccountModel _dominatorAccount,
            AccountModel accountModel, string usernameId, CancellationToken token, string queryType = null);

        LoginIgResponseHandler TwoFactorLogin(DominatorAccountModel _dominatorAccount, AccountModel _Account,
            string twoFactorIdentifier);

        ResendTwoFactorLoginCodeResponseHandler SendAgainTwoFactorLoginCode(string twoFactorIdentifier);

        Task<FriendshipsResponse> Unfollow(DominatorAccountModel _dominatorAccount, AccountModel _Account,
            CancellationToken token, string userId);

        UserFriendshipResponse UserFriendship(DominatorAccountModel _dominatorAccount, AccountModel _Account,
            string userId);

        Task<FriendshipsResponse> Block(DominatorAccountModel _dominatorAccount, AccountModel _Account,
            CancellationToken token, string userId);

        Task<FriendshipsResponse> UnBlock(DominatorAccountModel _dominatorAccount, AccountModel _Account,
            CancellationToken token, string userId);

        Task<DeleteMediaIgResponseHandler> DeleteMedia(DominatorAccountModel _dominatorAccount, AccountModel _Account,
            CancellationToken token,
            string mediaId);

        VisualThreadResponse GetVisualThread(DominatorAccountModel _dominatorAccount, string threadId,
            string cursorId = null);

        Task<MediaInfoIgResponseHandler> MediaInfo(DominatorAccountModel _dominatorAccount, AccountModel _Account,
            string mediaId, CancellationToken token);

        Task<UserFeedResponse> GetLikedMedia(DominatorAccountModel _dominatorAccoun, string maxId = null);

        Task<CommonIgResponseHandler> UnlikeMedia(DominatorAccountModel _dominatorAccount, AccountModel _Account,
            CancellationToken token,
            string generatedMediaId);

        FollowerAndFollowingIgResponseHandler GetRecentFollowers(DominatorAccountModel _dominatorAccount);

        Task<LocationIgReponseHandler> SearchForLocation(DominatorAccountModel _dominatorAccount, string locations,
            bool isLocation = false);

        LocationIdIgReponseHandler SearchLocationId(DominatorAccountModel _dominatorAccount, string LocationId);

        UploadMediaResponse UploadVideo(DominatorAccountModel _dominatorAccount, AccountModel accountModel,
            CancellationToken token, string videoFilePath, string thumbnailFilePath, string caption = "",
            string tagLocation = null, List<string> lstTagUserIds = null);

        UploadMediaResponse ConfigureVideo(DominatorAccountModel _dominatorAccount, AccountModel accountModel,
            CancellationToken token, string videoFilePath, string thumbnailFilePath, string uploadId,
            string caption = "", string tagLocation = null, List<string> lstTagUserIds = null);

        UploadMediaResponse ConfigurevideoAlbum(List<ImageDetails> imageList, string caption, string tagLocation = null,
            List<string> lstTagUserIds = null);

        Task<CommonIgResponseHandler> LikeOnComment(DominatorAccountModel _dominatorAccount, AccountModel _Account,
            CancellationToken token,
            string mediaId, string CommentID = "");

        Task<SendMessageIgResponseHandler> SendPhotoAsDirectMessage(DominatorAccountModel _dominatorAccount,
            AccountModel _Account, CancellationToken token, string userId, string photoPath, string ThreadId = null);

        UploadMediaResponse Story_Configure_To_Reel(string photo, string uploadId, List<string> lstTagUserIds,
            string tagLocation = null);

        StoryAdsResponse GetStoryAds(DominatorAccountModel dominatorAccountModel, AccountModel accountModel,
            List<string> userList);

        CommonIgResponseHandler MuteFollowerPostUser(InstagramUser instagramUser,
            DominatorAccountModel dominatorAccountModel, AccountModel accountModel);

        CommonIgResponseHandler MuteFollowerStoryUser(InstagramUser instagramUser,
            DominatorAccountModel dominatorAccountModel, AccountModel accountModel);

        Task<LoginIgResponseHandler> LoginAsync(DominatorAccountModel _dominatorAccount, AccountModel _Account);
        Task<bool> TypeKeyboardPreLoginAsync(DominatorAccountModel _dominatorAccount, AccountModel _Account);

        Task<LoginIgResponseHandler> LoginAsyncForTwoFactor(DominatorAccountModel _dominatorAccount,
            AccountModel _Account);

        CommonIgResponseHandler ResetSendRequest(string challengeApiPathUrl, DominatorAccountModel _dominatorAccount,
            AccountModel _Account);
        UploadMediaResponse UploadPhotoAlbum(DominatorAccountModel dominatorAccountModel, AccountModel _Account,
            CancellationToken token, List<string> imagesList, string caption, string tagLocation = null,
            List<string> lstTagUserIds = null);

        UploadMediaResponse ConfigurePhotoAlbum(DominatorAccountModel dominatorAccountModel, AccountModel _Account,
            CancellationToken token, List<string> uploadIds, List<int> imageWidht, List<int> imageHeight,
            string caption, string tagLocation = null,
            List<string> lstTagUserIds = null);

        UploadMediaResponse UploadPhoto(DominatorAccountModel _dominatorAccountModel, AccountModel accountModel,
            CancellationToken token, string photo, List<string> lstTagUserIds, string uploadId = null,
            bool isSideCar = false, string caption = "",
            string tagLocation = null);

        UploadMediaResponse ConfigurePhoto(DominatorAccountModel _dominatorAccountModel, AccountModel accountModel,
            CancellationToken token, string photo, string uploadId, List<string> lstTagUserIds, string caption = "",
            string tagLocation = "");

        UploadMediaResponse UploadTimeLinePhoto(DominatorAccountModel _dominatorAccountModel, AccountModel accountModel,
            CancellationToken token, string photo, List<string> lstTagUserIds, string uploadId = null,
            bool isSideCar = false, string caption = "",
            string tagLocation = null);

        UploadMediaResponse Configure(DominatorAccountModel _dominatorAccountModel, AccountModel accountModel,
            CancellationToken token, string photo, string uploadId, List<string> lstTagUserIds, string caption = "",
            string tagLocation = null);

        CommonIgResponseHandler SavePost(string mediaId, DominatorAccountModel _dominatorAccountModel,
            AccountModel accountModel, CancellationToken token);

        UploadMediaResponse VideoStory(string videoFilePath, string VideoThumnail, string uploadId,
            List<string> lstTagUserIds = null, string tagLocation = null, int noOfStoryPost = 1);

        UploadMediaResponse Story_Upload_IG_Video(string VideoFilePath, string uploadId,
            List<string> lstTagUserIds = null, string tagLocation = null, int noOfStoryPost = 1,
            string thumbnailImagePath = null);

        UploadMediaResponse PhotoForVideoStory(string photoFilePath, string uploadId,
            List<string> lstTagUserIds = null, string tagLocation = null, int noOfStoryPost = 1);

        UploadMediaResponse Story_Video_Upload_IG_Photo(string photoFilePath, string uploadId,
            List<string> lstTagUserIds = null, string tagLocation = null, int noOfStoryPost = 1);


        UploadMediaResponse Story_Upload_IG_Photo(string photoFilePath, string uploadId,
            List<string> lstTagUserIds = null, List<ImagePosition> imagePosition = null, string tagLocation = null,
            int noOfStoryPost = 1);

        UploadMediaResponse Story_Photo_Configure_To_Reel(string photo, string uploadId, List<string> lstTagUserIds,
            List<ImagePosition> imagePosition, string tagLocation = null, int noOfStoryPost = 1);

        UploadMediaResponse Story_Video_Configue_To_Reel(string photo, string uploadId,
            List<string> lstTagUserIds, string tagLocation = null, int noOfStoryPost = 1);

        CommonIgResponseHandler CheckEmail(DominatorAccountModel _dominatorAccount,
            AccountModel _Account);

        CommonIgResponseHandler SignUp_FetchHeader(DominatorAccountModel _dominatorAccount,
            AccountModel _Account);

        CommonIgResponseHandler ConsentNewUserFlowBegins(DominatorAccountModel _dominatorAccount,
            AccountModel _Account);

        CommonIgResponseHandler DynamicOnBoardingGetSteps(DominatorAccountModel _dominatorAccount,
            AccountModel _Account);

        CommonIgResponseHandler signUp(DominatorAccountModel _dominatorAccount, AccountModel _Account);

        CommonIgResponseHandler AccountContactPointPrefillForSignUp(DominatorAccountModel _dominatorAccount,
            AccountModel _Account);

        IGdHttpHelper GetGdHttpHelper();

        TaggedPostResponseHandler TaggedPost(DominatorAccountModel _dominatorAccount, AccountModel _Account);

        TaggedPostResponseHandler SomeoneTaggedPost(DominatorAccountModel _dominatorAccount, string userId,
            CancellationToken token, string maxid = null);

        FriendShipPendingResponseHandler PendingRequest(DominatorAccountModel _dominatorAccount);

        FriendshipsResponse AcceptRequest(DominatorAccountModel _dominatorAccount, AccountModel _Account,
            CancellationToken token, string Userid);

        CommonIgResponseHandler GetFeedTimeLineData(DominatorAccountModel dominatorAccountModel,
            AccountModel accountModel);

        MsisdnHeaderResponse ReadMsisdnHeader(AccountModel accountModel);

        Task<MsisdnHeaderResponse> ReadMsisdnHeaderAsync(DominatorAccountModel _dominatorAccount,
            AccountModel accountModel);

        Task<CommonIgResponseHandler> AccountContactPointPrefillAsync(DominatorAccountModel _dominatorAccount,
            AccountModel accountModel);

        Task<LogAttributionResponse> LoginAttributionAsync(DominatorAccountModel _dominatorAccount,
            AccountModel accountModel);

        Task<CommonIgResponseHandler> LauncherSyncAsyncBeforeLogin(DominatorAccountModel _dominatorAccount,
            AccountModel accountModel);

        Task<CommonIgResponseHandler> B_ZrTokenAsyncBeforeLogin(DominatorAccountModel _dominatorAccount,
            AccountModel accountModel);

        Task<CommonIgResponseHandler> LauncherSyncAsyncAfterLogin(DominatorAccountModel _dominatorAccount,
            AccountModel accountModel);

        Task<SyncIgResponseHandler> SyncDeviceFeaturesAsyncBeforeLogin(DominatorAccountModel _dominatorAccountModel,
            AccountModel accountModel);

        Task<SyncIgResponseHandler> B_SyncDeviceFeaturesAsyncBeforeLogin(DominatorAccountModel _dominatorAccountModel,
            AccountModel accountModel);

        Task<SyncIgResponseHandler> SyncDeviceFeaturesAsyncAfterLogin(DominatorAccountModel _dominatorAccountModel,
            AccountModel accountModel);


        Task<CommonIgResponseHandler> GetExtraCookiesResponse(DominatorAccountModel _dominatorAccountModel,
            AccountModel accountModel);

        IGdBrowserManager GdBrowserManager { get; set; }

        bool IsAlreadyFollowedByBrowser(string userName, DominatorAccountModel _dominatorAccount,
            ActivityType activityType, CancellationToken token);

        FollowerAndFollowingIgResponseHandler GetUserFollowersBrowser(DominatorAccountModel _dominatorAccount,
            string usernameId, CancellationToken token, string QueryType = null, string maxid = null);

        CommonIgResponseHandler HighlightsUser(DominatorAccountModel _dominatorAccountModel, AccountModel accountModel,
            string userId);

        CommonIgResponseHandler StoryHighlightUser(DominatorAccountModel _dominatorAccountModel,
            AccountModel accountModel, string userId);

        CommonIgResponseHandler Account_Recs(DominatorAccountModel _dominatorAccountModel, AccountModel accountModel,
            string userId);

        FriendshipsShowManyResponse ShowManyUserFriends(DominatorAccountModel _dominatorAccountModel,
            AccountModel accountModel, List<string> userList);

        CommonIgResponseHandler Logout(DominatorAccountModel dominatorAccountModel, AccountModel accountModel);

        Task<ReelsTrayFeedResponse> GetReelsTrayFeed(DominatorAccountModel dominatorAccountModel,
            AccountModel accountModel);

        Task<BlockedMediaResponse> GetBlockedMedia(DominatorAccountModel dominatorAccountModel,
            AccountModel accountModel);

        Task<ProfileNoticeResponse> GetProfileNotice(DominatorAccountModel dominatorAccountModle,
            AccountModel accounModel);

        Task<ActivityNewsResponse> GetRecentActivity(DominatorAccountModel dominatorAccount);

        Task<CommonIgResponseHandler> SuggestedSearches(DominatorAccountModel dominatorAccountModel,
            AccountModel accountModel);

        Task<CommonIgResponseHandler> RecentSearches(DominatorAccountModel dominatorAccountModel,
            AccountModel accountModel);

        Task<CommonIgResponseHandler> SuggestedSearches1(DominatorAccountModel dominatorAccountModel,
            AccountModel accountModel);

        Task<CommonIgResponseHandler> ranked_recipients(DominatorAccountModel dominatorAccountModel,
            AccountModel accountModel);

        Task<CommonIgResponseHandler> PersistentBadging(DominatorAccountModel dominatorAccountModel,
            AccountModel accountModel);

        Task<CommonIgResponseHandler> GetPresence(DominatorAccountModel dominatorAccountModel,
            AccountModel accountModel);

        Task<CommonIgResponseHandler> topical_explore(DominatorAccountModel dominatorAccountModel,
            AccountModel accountModel);

        Task<CommonIgResponseHandler> Scores(DominatorAccountModel dominatorAccountModel, AccountModel accountModel);

        Task<CommonIgResponseHandler> PushRegister(DominatorAccountModel dominatorAccountModel,
            AccountModel accountModel);

        Task<CommonIgResponseHandler> TokenResult(DominatorAccountModel _dominatorAcount, AccountModel accountModel);

        Task<ChatInfoIgResponseHandler> GetChatInfo(DominatorAccountModel dominatorAccountModel,
            AccountModel accountModel);

        Task<CommonIgResponseHandler> GetLinkageStatus(DominatorAccountModel dominatorAccountModel,
            AccountModel accountModel);

        Task<CommonIgResponseHandler> ranked_recipients_Mode_Raven(DominatorAccountModel dominatorAccountModel,
            AccountModel accountModel);

        string RandomString(int size);

        PostStoryResponse GetStoriesUsers(DominatorAccountModel dominatorAccountModel, AccountModel accountModel,
            List<string> lstUser);

        CommonIgResponseHandler SeenUserStory(DominatorAccountModel dominatorAccountModel, AccountModel accountModel,
            List<UsersPostStory> User);

        Task<FriendshipsResponse> RemoveFollowers(DominatorAccountModel dominatorAccountModel, AccountModel accountModel,
            string userId, CancellationToken token,string Username="");

        SendMessageIgResponseHandler IgSendUploadPhoto(DominatorAccountModel _dominatorAccount, AccountModel _Account,
            CancellationToken token, string userId, string photoPath, string threadId = null);

        Task<CommonIgResponseHandler> ReplyComment(DominatorAccountModel dominatorAccountModel, AccountModel accountModel,
            string CommentId, string comment, string mediaId, CancellationToken token,string Username="");

        UploadMediaResponse IgVideo_uploadingVideo(DominatorAccountModel _dominatorAccount,
           AccountModel accountModel, CancellationToken token, string videoFilePath, string thumbnailFilePath,
           string uploadId, MediaInfo mediaInfo, bool isVideoAlbum = false, string caption = "",
           string tagLocation = null, List<string> lstTagUserIds = null);

        UploadMediaResponse igphoto_uploadingThumnail(DominatorAccountModel _dominatorAccount,
            AccountModel accountModel, CancellationToken token, string thumbnailFilePath, string uploadId,
            MediaInfo mediaInfo, string videoFilePath, bool isVideoAlbum = false, string caption = "",
            string tagLocation = null, List<string> lstTagUserIds = null);

        UploadMediaResponse UploadProfilePicture(DominatorAccountModel _dominatorAccount, AccountModel _Account,
            string photo, ref string UploadId, ref string hashCode, CancellationToken token);

        string UPloadProfileInitialRequest(DominatorAccountModel _dominatorAccount, AccountModel _Account, string photo,
            ref string UploadId, ref string hashCode, CancellationToken token);

        UploadMediaResponse SendVideoMessage(DominatorAccountModel _dominatorAccount, AccountModel _Account,
            string mediaPath, string thumbnailPhotoPath, string userId, CancellationToken token);

        Task<CommonIgResponseHandler> GetPrefillCandidatesBeforeLogin(DominatorAccountModel _dominatorAccount,
            AccountModel accountModel);

        CaptchaRequestResponseHandler SubmittingCaptchaSolution(DominatorAccountModel _dominatorAccount,
            AccountModel accountModel, string response, string apiUrl, CancellationToken token);

        SubmitPhoneCodeForCaptchaResponseHandler SubmitPhoneNumberRequest(DominatorAccountModel _dominatorAccount, AccountModel accountModel, string chellagenContext, string PhoneNumber, string apiUrl, CancellationToken token);
        SubmitPhoneNumberAfterCaptchaResponseHandler ConfirmPhoneNumberRequest(DominatorAccountModel _dominatorAccount, AccountModel accountModel, string chellagenContext, string PhoneNumber, string apiUrl, CancellationToken token);

        UsernameInfoIgResponseHandler Get_AccountUserid(DominatorAccountModel _dominatorAccount,
          AccountModel accountModel, string usernameId, CancellationToken token, string queryType = null);
        void UpdateImportantHeadersOnEachRequest(IgRequestParameters requestParameters, AccountModel accountModel);
        UploadMediaResponse UploadingVideoWeb(DominatorAccountModel _dominatorAccount,
            AccountModel accountModel, CancellationToken token, string videoFilePath, string thumbnailFilePath,
            string uploadId, MediaInfo mediaInfo, bool isVideoAlbum = false, string caption = "",
            string tagLocation = null, List<string> lstTagUserIds = null);
        Task<ThreadIDDetails> GetThreadID(DominatorAccountModel dominatorAccountModel, string userId,string profileId,bool IsBrowser=true);
        Task<UploadMediaResponse> UploadMediaHttp(DominatorAccountModel dominatorAccountModel, InstagramPost instagramPost, List<string> mediaList, CancellationToken token);
        Task<UploadMediaResponse> UploadMedia(DominatorAccountModel dominatorAccountModel, InstagramPost instagramPost, List<string> mediaList);
        Task<UploadMediaResponse> StoryUpload(DominatorAccountModel dominatorAccountModel, InstagramPost instagramPost, string StoryMedia);
        Task<MediaCommentsIgResponseHandler> GetCommentDetails(DominatorAccountModel dominatorAccount, string MediaCode, string CommentID);
        IGWebParameter GetWebParameter(DominatorAccountModel dominatorAccount, bool MobileRequest = false);

        #region WebLoginMethod.

        Task<WebLoginIgResponseHandler> IGWebLogin(DominatorAccountModel dominatorAccount);

        #endregion
    }

    [Localizable(false)]
    public class InstaFunct : IInstaFunction
    {
        private readonly IDateProvider _dateProvider;

        private DominatorAccountModel _dominatorAccount { get; set; }
        private AccountModel _Account;
        protected IGdHttpHelper httpHelper { get; set; }
        public IGdBrowserManager GdBrowserManager { get; set; }
        public IDelayService delayService { get; set; }
        private CancellationToken CancellationToken { get; }

        public InstaFunct(DominatorAccountModel dominatorAccountModel, IGdHttpHelper HttpHelper,
            IGdBrowserManager gdBrowserManager, IDelayService _delayService, IDateProvider dateProvider)
        {
            try
            {
                GdBrowserManager = gdBrowserManager;
                httpHelper = HttpHelper;
                _dominatorAccount = dominatorAccountModel;
                _Account = new AccountModel(_dominatorAccount);
                delayService = _delayService;
                _dateProvider = dateProvider;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private bool HasUnifiedInbox => true;

        public BrowserWindow BrowserWindow { get; set; }
        public BrowserWindow SecondaryBrowserWindow { get; set; }
        public BrowserWindow fourthBrowserWindow { get; set; }
        public BrowserWindow ThirdBrowserWindow { get; set; }

        #region Edit Instagram Profile Requests

        public UsernameInfoIgResponseHandler GetProfileDetails(DominatorAccountModel _dominatorAccount)
        {
            try
            {
                var requestParameters = (IgRequestParameters)httpHelper.GetRequestParameter();
                requestParameters.Url = "accounts/current_user/?edit=true";

                var url = Constants.ApiUrl + "accounts/current_user/?edit=true";

                return new UsernameInfoIgResponseHandler(httpHelper.GetRequest(url));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public async Task<UsernameInfoIgResponseHandler> ChangeProfilePictureAsync(
            DominatorAccountModel _dominatorAccount, AccountModel _Account, string image, string uploadId)
        {
            try
            {
                var jsonElements = new JsonElements()
                {
                    Csrftoken = _Account.CsrfToken,
                    Uuid = _Account.Uuid,
                    UseFbuploader = true,
                    UploadId = uploadId
                };

                //{"upload_id":"1572516964567","_csrftoken":"h2X3Ztuky8WvB9iPHVoBzjfRyJzRXZoH","use_fbuploader":true,"_uuid":"52f9b1c7-1b7f-4cab-8bfd-b028e7ada843"}
                var data = File.ReadAllBytes(image);
                var requestParameter = (IgRequestParameters)httpHelper.GetRequestParameter();
                requestParameter.FileList = new Dictionary<string, FileData>();
                requestParameter.Body = jsonElements;
                requestParameter.AddFileList("profile_pic", data, "profile_pic");

                requestParameter.Url = "accounts/change_profile_picture/";
                var url = Constants.ApiUrl + requestParameter.GenerateUrl("accounts/change_profile_picture/");

                var postData = requestParameter.GenerateBody();
                requestParameter.IsMultiPart = false;

                var editProfileResponse = await httpHelper.PostRequestAsync(url, postData, CancellationToken);

                requestParameter.ContentType = Constants.ContentTypeDefault;

                return new UsernameInfoIgResponseHandler(editProfileResponse);
            }
            catch (AssertFailedException)
            {
                throw;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public async Task<CommonIgResponseHandler> SetBiographyAsync(DominatorAccountModel _dominatorAccount,
            AccountModel _Account, string bioText)
        {
            try
            {
                var jsonElements = new JsonElements()
                {
                    Csrftoken = _Account.CsrfToken,
                    Uid = _dominatorAccount.AccountBaseModel.UserId,
                    Uuid = _Account.Uuid,
                    RawText = bioText
                };

                var requestParameter = (IgRequestParameters)httpHelper.GetRequestParameter();

                requestParameter.Body = jsonElements;
                requestParameter.Url = "accounts/set_biography/";

                var url = Constants.ApiUrl + "accounts/set_biography/";

                var postData = requestParameter.GenerateBody();

                return new CommonIgResponseHandler(await httpHelper.PostRequestAsync(url, postData, CancellationToken));
            }
            catch (AssertFailedException)
            {
                throw;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public async Task<UsernameInfoIgResponseHandler> EditProfileAsync(DominatorAccountModel _dominatorAccount,
            AccountModel _Account, string url, string phone, string fullName, string bio, string email, int gender,
            string username)
        {
            try
            {
                var jsonElements = new JsonElements()
                {
                    ExternalUrl = url,
                    Gender = gender,
                    PhoneNumber = phone,
                    Csrftoken = _Account.CsrfToken,
                    Username = username,
                    FirstName = fullName,
                    Uid = _dominatorAccount.AccountBaseModel.UserId,
                    Biography = bio,
                    Uuid = _Account.Uuid,
                    Email = email
                };

                var requestParameter = (IgRequestParameters)httpHelper.GetRequestParameter();

                requestParameter.Body = jsonElements;
                requestParameter.Url = "accounts/edit_profile/";

                url = Constants.ApiUrl + requestParameter.GenerateUrl();

                var postData = requestParameter.GenerateBody();

                return new UsernameInfoIgResponseHandler(
                    await httpHelper.PostRequestAsync(url, postData, CancellationToken));
            }
            catch (AssertFailedException)
            {
                throw;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        #endregion

        public async Task<CheckUsernameResponse> CheckUsernameAsync(DominatorAccountModel _dominatorAccount,
            AccountModel _Account, string username)
        {
            try
            {
                var jsonElements = new JsonElements()
                {
                    Csrftoken = _Account.CsrfToken,
                    Username = username,
                    Uid = _dominatorAccount.AccountBaseModel.UserId,
                    Uuid = _Account.Uuid
                };

                var requestParameters = (IgRequestParameters)httpHelper.GetRequestParameter();

                requestParameters.Body = jsonElements;
                requestParameters.Url = "users/check_username/";

                var url = Constants.ApiUrl + requestParameters.GenerateUrl();

                var postData = requestParameters.GenerateBody();

                return new CheckUsernameResponse(await httpHelper.PostRequestAsync(url, postData, CancellationToken));
            }
            catch (AssertFailedException)
            {
                throw;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public CheckOffensiveCommentResponseHandler CheckOffensiveComment(DominatorAccountModel _dominatorAccount,
            AccountModel accountModel, CancellationToken token, string mediaId, string comment)
        {
            var jsonElements = new JsonElements()
            {
                Uuid = accountModel.Uuid,
                Uid = _dominatorAccount.AccountBaseModel.UserId,
                Csrftoken = accountModel.CsrfToken,
                CommentText = comment,
                MediaId = $"{mediaId}_{_dominatorAccount.AccountBaseModel.UserId}",
            };
            var requestParameters =
                (IgRequestParameters)httpHelper.GetRequestParameter();
            requestParameters.Body = jsonElements;
            requestParameters.AddHeader("X-IG-VP9-Capable", "false");
            var url = $"media/comment/check_offensive_comment/";
            url = Constants.ApiUrl + url;

            var postData = requestParameters.GenerateBody();
            token.ThrowIfCancellationRequested();
            var OffensiveCommentCheck =
                new CheckOffensiveCommentResponseHandler(httpHelper.PostRequest(url, postData));
            requestParameters.Headers.Remove("X-IG-VP9-Capable");
            return OffensiveCommentCheck;
        }

        public async Task<CommentResponse> Comment(DominatorAccountModel _dominatorAccount, AccountModel accountModel,
            CancellationToken token, string mediaId, string comment, string replyCommentId = null,
            string module = "comments_v2", string userBreadcrumb = null, string IdempotenceToken = null)
        {
            token.ThrowIfCancellationRequested();
            CommentResponse commentResponse1 = null;
            try
            {
                #region OLD Comment Code
                //var jsonElements = new JsonElements()
                //{
                //    UserBreadcrumb = InstagramHelper.GenerateUserBreadcrumb(comment.Length),
                //    Delivery_Class = "organic",
                //    IdempotenceToken = StringHelper.GenerateGuid(),
                //    Csrftoken = accountModel.CsrfToken,
                //    RadioType = "wifi-none",
                //    Uid = _dominatorAccount.AccountBaseModel.UserId,
                //    Uuid = accountModel.Uuid,
                //    CommentText = comment,
                //    IsCarouselBumpedPost = "false",
                //    Containermodule =
                //        "comments_v2_feed_contextual_profile", //comments_v2  comments_v2_feed_contextual_chain
                //    FeedPosition = Convert.ToString(new Random().Next(0, 9)),
                //};

                ////only for Unit testing 
                //if (!string.IsNullOrEmpty(userBreadcrumb))
                //    jsonElements = new JsonElements()
                //    {
                //        UserBreadcrumb = userBreadcrumb,
                //        IdempotenceToken = IdempotenceToken,
                //        Uuid = accountModel.Uuid,
                //        Uid = _dominatorAccount.AccountBaseModel.UserId,
                //        Csrftoken = accountModel.CsrfToken,
                //        CommentText = comment,
                //        Containermodule = module,
                //        RadioType = "wifi-none",
                //        DeviceId = accountModel.Device_Id,
                //    };

                //if (replyCommentId != null) jsonElements.RepliedToCommentId = replyCommentId;

                //var requestParameters =
                //    (IgRequestParameters)httpHelper.GetRequestParameter();
                //requestParameters.Body = jsonElements;
                //requestParameters.AddHeader("X-IG-VP9-Capable", "false");
                //var url = $"media/{mediaId + "_" + _dominatorAccount.AccountBaseModel.UserId + ""}/comment/";
                //url = Constants.ApiUrl + url;

                //var postData = requestParameters.GenerateBody();
                //token.ThrowIfCancellationRequested();
                //var commentResponse =
                //    new CommentResponse(httpHelper.PostRequest(url, postData));
                //requestParameters.Headers.Remove("X-IG-VP9-Capable");
                ////requestParameters.Headers.Remove("Content-Type");
                //return commentResponse;
                #endregion
                var mediaID = GramStatic.GetCodeFromIDOrUrl(mediaId);
                var url = $"https://www.instagram.com/api/v1/web/comments/{mediaID}/add/";
                var param = GetWebParameter(_dominatorAccount);
                using (var client = new HttpClient(param.httpClient))
                {
                    var uri = new Uri("https://www.instagram.com/");
                    var PostBody = $"comment_text={Uri.EscapeDataString(comment)}&jazoest={GramStatic.CreateJazoest(_dominatorAccount?.DeviceDetails?.PhoneId)}";
                    var content = new StringContent(PostBody, Encoding.UTF8, "application/x-www-form-urlencoded");
                    client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/137.0.0.0 Safari/537.36");
                    client.DefaultRequestHeaders.Add("X-CSRFToken", param.CsrfToken);
                    client.DefaultRequestHeaders.Add("X-IG-App-ID", "936619743392459");
                    client.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
                    client.DefaultRequestHeaders.Add("X-Instagram-AJAX", GramStatic.InstagramAjax());
                    //client.DefaultRequestHeaders.Add("X-Web-Session-ID", "8b53zz:faqbmw:u44yaq");
                    //client.DefaultRequestHeaders.Add("X-ASBD-ID", "359341");
                    if(!string.IsNullOrEmpty(param.X_IG_Claim))
                        client.DefaultRequestHeaders.Add("X-IG-WWW-Claim", param.X_IG_Claim);
                    client.DefaultRequestHeaders.Referrer = new Uri($"https://www.instagram.com/p/{mediaId}/");

                    var response = await client.PostAsync(url, content);

                    var responseBody = await response.Content.ReadAsStreamAsync();
                    var ResponseText = HttpHelper.Decode(responseBody, string.Join(",", response.Content.Headers.ContentEncoding));
                    commentResponse1 = new CommentResponse(ResponseText);
                }
            }
            catch (AssertFailedException)
            {
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return commentResponse1;
        }

        public async Task<SendMessageIgResponseHandler> SendMessage(DominatorAccountModel _dominatorAccount,
            AccountModel accountModel, string userId, string message, string threadId, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            try
            {
                #region OLD Broadcast message code
                //var clientContext = Utilities.GetGuid();
                //var jsonElements = new JsonElements();
                //if (string.IsNullOrEmpty(threadId))
                //    jsonElements = new JsonElements()
                //    {
                //        Action = "send_item",
                //        DeviceId = accountModel.Device_Id,
                //        ClientContext = clientContext,
                //        MutationToken = clientContext,
                //        OfflineThreadingId = clientContext,
                //        Uuid = accountModel.Uuid,
                //        Text = message,
                //        RecipientUsers = $"[[{userId}]]",
                //    };
                //else
                //    jsonElements = new JsonElements()
                //    {
                //        Action = "send_item",
                //        DeviceId = accountModel.Device_Id,
                //        ClientContext = clientContext,
                //        MutationToken = clientContext,
                //        OfflineThreadingId = clientContext,
                //        Uuid = accountModel.Uuid,
                //        Text = message,
                //        ThreadIds = $"[{threadId}]",
                //    };


                //var requestParameter = (IgRequestParameters)httpHelper.GetRequestParameter();
                //requestParameter.Body = jsonElements;
                ////requestParameter.ContentType = Constants.ContentTypeDefault;

                //var url = Constants.ApiUrl + "direct_v2/threads/broadcast/text/";
                //requestParameter.DontSign();
                //var postData = requestParameter.GenerateBody();
                //// requestParameter.IsMultiPartForBroadCast = false;
                //token.ThrowIfCancellationRequested();
                ////string postData =
                ////   $"recipient_users=%5B%5B%22{userId}%22%5D%5D&client_context=%22{Guid.NewGuid()}%22&text={WebUtility.UrlEncode(message)}";

                ////  byte[] postDataInByte = Encoding.UTF8.GetBytes(postData);
                //token.ThrowIfCancellationRequested();
                //return new SendMessageIgResponseHandler(httpHelper.PostRequest(url, postData));
                #endregion
                var param = GetWebParameter(_dominatorAccount,true);
                var util = IGMobileUtilities.Instance(_dominatorAccount);
                var url = "https://i.instagram.com/api/v1/direct_v2/threads/broadcast/text/";

                var client = new HttpClient();
                var IgDeviceID = _dominatorAccount?.DeviceDetails?.PhoneId ?? "cba40baf-8663-4ee8-a0bb-74dc5d2e0a98";
                var request = new HttpRequestMessage(HttpMethod.Post, url);

                // Add headers
                request.Headers.Add("User-Agent", _dominatorAccount?.DeviceDetails?.Useragent ?? "Instagram 283.0.0.20.105 Android (31/12; 320dpi; 720x1470; vivo; V2029; 2027; qcom; en_US; 475221264)");
                request.Headers.Add("X-IG-App-Locale", "en_US");
                request.Headers.Add("X-IG-Device-Locale", "en_US");
                request.Headers.Add("X-IG-Mapped-Locale", "en_US");
                request.Headers.Add("X-Pigeon-Session-Id", "UFS-"+(_dominatorAccount?.DeviceDetails?.PhoneId ?? Guid.NewGuid().ToString())+"-2");
                request.Headers.Add("X-Pigeon-Rawclienttime", GdUtilities.GetRowClientTime());
                //request.Headers.Add("X-IG-Bandwidth-Speed-KBPS", "243.000");
                //request.Headers.Add("X-IG-Bandwidth-TotalBytes-B", "10453899");
                //request.Headers.Add("X-IG-Bandwidth-TotalTime-MS", "29814");
                request.Headers.Add("X-Bloks-Version-Id", "f5fbf62cc3c51dc0e6f4ffd3a79e0c5929ae0b8af58c54acd1e186871a92fb27");
                request.Headers.Add("X-IG-WWW-Claim", param?.X_IG_Claim);
                request.Headers.Add("X-Bloks-Is-Layout-RTL", "false");
                request.Headers.Add("X-IG-Device-ID", IgDeviceID);
                request.Headers.Add("X-IG-Family-Device-ID", _dominatorAccount?.DeviceDetails?.FamilyId ?? "6a1184d2-87c2-454f-93c5-a491e0040ad7");
                request.Headers.Add("X-IG-Android-ID", _dominatorAccount?.DeviceDetails?.DeviceId ?? "android-b10069c5ba7bbd58");
                request.Headers.Add("X-IG-Timezone-Offset", "19800");
                request.Headers.Add("X-IG-Nav-Chain", "MainFeedFragment:feed_timeline:1:cold_start:1750560451.671:10#230#301:3645324241189314859,UserDetailFragment:profile:20:media_owner:1750567697.782::,ProfileMediaTabFragment:profile:21:button:1750567700.993::,DirectThreadFragment:direct_thread:22:message_button:1750567702.526::,DirectThreadFragment:direct_thread:23:button:1750567702.529::");
                request.Headers.Add("X-IG-CLIENT-ENDPOINT", "DirectThreadFragment:direct_thread");
                //request.Headers.Add("X-IG-SALT-LOGGER-IDS", "399507457,20122678,25624577,974460658,231352080,42991645,25952257,42991646,61669378");
                request.Headers.Add("X-FB-Connection-Type", "WIFI");
                request.Headers.Add("X-IG-Connection-Type", "WIFI");
                request.Headers.Add("X-IG-Capabilities", "3brTv10=");
                request.Headers.Add("X-IG-App-ID", "567067343352427");
                request.Headers.Add("Accept-Language", "en-US");
                request.Headers.Add("Authorization", param.Authorization);
                request.Headers.Add("X-MID", param.MID);
                request.Headers.Add("IG-U-DS-USER-ID", param.DsUserId);
                //request.Headers.Add("IG-U-RUR", "CCO,73228986484,1782103703:01fe1b2a376125330f867f617eafc66f984459a0a9009c41747b1ef5bf8ed766d58f36b8");
                request.Headers.Add("IG-INTENDED-USER-ID", param.DsUserId);
                request.Headers.Add("Accept-Encoding", GramStatic.AcceptEncoding);
                var haveThreadID = !string.IsNullOrEmpty(threadId) && threadId.Length >= 35;
                var context = haveThreadID ? util.GetClientContext():threadId;
                // Form data:
                var formData = new StringBuilder();
                if(!haveThreadID)
                    formData.Append($"recipient_users=%5B%5B{userId}%5D%5D");
                formData.Append("&action=send_item");
                formData.Append("&is_x_transport_forward=false");
                if (haveThreadID)
                    formData.Append($"&thread_ids=[{threadId}]");
                formData.Append("&is_shh_mode=0");
                formData.Append("&send_silently=false");
                if (haveThreadID)
                    formData.Append("&send_attribution=direct_thread");
                else
                    formData.Append("&send_attribution=message_button");
                formData.Append($"&client_context={context}");
                formData.Append($"&text={Uri.EscapeDataString(message)}");
                formData.Append("&device_id=android-b10069c5ba7bbd58");
                formData.Append($"&mutation_token={context}");
                formData.Append($"&_uuid={IgDeviceID}");
                formData.Append("&btt_dual_send=false");
                if(!haveThreadID)
                    formData.Append("&nav_chain=MainFeedFragment%3Afeed_timeline%3A1%3Acold_start%3A1750560451.671%3A10%23230%23301%3A3645324241189314859%2CUserDetailFragment%3Aprofile%3A20%3Amedia_owner%3A1750567697.782%3A%3A%2CProfileMediaTabFragment%3Aprofile%3A21%3Abutton%3A1750567700.993%3A%3A%2CDirectThreadFragment%3Adirect_thread%3A22%3Amessage_button%3A1750567702.526%3A%3A%2CDirectThreadFragment%3Adirect_thread%3A23%3Abutton%3A1750567702.529%3A%3A");
                else
                    formData.Append("&nav_chain=MainFeedFragment:feed_timeline:1:cold_start:1750582966.780:10#230#301:3660034980283411224,UserDetailFragment:profile:15:media_owner:1750584370.229::,ProfileMediaTabFragment:profile:16:button:1750584373.618::,DirectThreadFragment:direct_thread:17:message_button:1750584375.171::,DirectThreadFragment:direct_thread:18:button:1750584375.173::");
                formData.Append($"&offline_threading_id={context}");

                request.Content = new StringContent(formData.ToString(), Encoding.UTF8, "application/x-www-form-urlencoded");

                // Send the request
                var response = await client.SendAsync(request);
                var ResponseText = await HttpHelper.Decode(response);
                var ErrorMessage = Utilities.GetBetween(ResponseText?.Response, "\"client_facing_error_message\":\"", "\"");
                return new SendMessageIgResponseHandler(ResponseText,
                    !string.IsNullOrEmpty(ErrorMessage)?Uri.UnescapeDataString(ErrorMessage):ErrorMessage);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public SendMessageIgResponseHandler SendMessageWithLink(DominatorAccountModel _dominatorAccount,
            CancellationToken token, string userId, string message, List<string> lstLinkUrls, string threadId,
            AccountModel _Account = null, bool isUnitTest = false)
        {
            token.ThrowIfCancellationRequested();
            try
            {
                var linkurls = string.Empty;
                lstLinkUrls.ForEach(linkUrl =>
                {
                    // linkUrl = linkUrl.Replace("/",@"\/");
                    if (string.IsNullOrEmpty(linkurls))
                        linkurls += $"\"{linkUrl}\"";
                    else
                        linkurls += $",\"{linkUrl}\"";
                });
                message = WebUtility.UrlEncode(message);
                var a = DateTime.Now.ToString("yyyyMMddHHmmssf");
                linkurls = linkurls?.Replace("/", "\\/");
                var decodedId = WebUtility.UrlEncode($"[[{userId}]]");
                var decodedUrl = WebUtility.UrlEncode($"[{linkurls}]");
                //var clientContext = Utilities.GetGuid();
                //int i = Guid.NewGuid().GetHashCode();
                Random generator = new Random();
                //string r = generator.Next(0, 10000000).ToString("D16");         
                var clientContext = 679 + generator.Next(0, 10000000).ToString("D16");
                var jsonElements = new JsonElements();
                var listUrl = new List<string>();
                if (string.IsNullOrEmpty(threadId))
                    jsonElements = new JsonElements()
                    {
                        //ShhMode=0,
                        LinkText = message,
                        //LinkUrls = $"[{linkurls}]",
                        LinkUrls = decodedUrl,
                        Action = "send_item",
                        ClientContext = clientContext,
                        DeviceId = _Account.Device_Id,
                        MutationToken = clientContext,
                        Uuid = _Account.Uuid,
                        OfflineThreadingId = clientContext,
                        //RecipientUsers = $"[[{userId}]]",
                        RecipientUsers = decodedId,
                        //ThreadIds = $"[{threadIdr}]",
                    };
                else
                    jsonElements = new JsonElements()
                    {
                        Action = "send_item",
                        DeviceId = _Account.Device_Id,
                        ClientContext = clientContext,
                        MutationToken = clientContext,
                        OfflineThreadingId = clientContext,
                        Uuid = _Account.Uuid,
                        //LinkUrls = $"[{linkurls}]",
                        LinkUrls = decodedUrl,
                        LinkText = message,
                        ThreadIds = $"[{threadId}]",
                    };

                var requestParameter =
                    (IgRequestParameters)httpHelper.GetRequestParameter();

                requestParameter.Body = jsonElements;
                // requestParameter.IsMultiPartForBroadCast = true;
                requestParameter.ContentType = Constants.ContentTypeDefault;
                requestParameter.Url = "direct_v2/threads/broadcast/link/";
                var url = Constants.ApiUrl + "direct_v2/threads/broadcast/link/";
                requestParameter.DontSign();
                var postData = requestParameter.GenerateBody();
                // requestParameter.IsMultiPartForBroadCast = false;
                token.ThrowIfCancellationRequested();
                return new SendMessageIgResponseHandler(httpHelper.PostRequest(url, postData));
            }
            catch (AssertFailedException)
            {
                throw;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public FriendshipsResponse Follow(DominatorAccountModel _dominatorAccountModel, AccountModel _account,
            CancellationToken token, string userId, string mediaId = null)
        {
            return FollowAsync(_dominatorAccountModel, _account, token, userId, mediaId).Result;
        }

        public async Task<FriendshipsResponse> FollowAsync(DominatorAccountModel _dominatorAccount,
            AccountModel _Account, CancellationToken token, string userId, string mediaId = null)
        {

            #region OLD Follow Code.
            //token.ThrowIfCancellationRequested();
            //JsonElements jsonElements = null;
            //try
            //{
            //    if (string.IsNullOrEmpty(mediaId))
            //        jsonElements = new JsonElements()
            //        {
            //            //Csrftoken = _Account.CsrfToken,
            //            UserId = userId,
            //            RadioType = "wifi-none", //"mobile-hspa+"
            //            Uid = _dominatorAccount.AccountBaseModel.UserId,
            //            DeviceId = _Account.Device_Id,
            //            Uuid = _Account.Uuid,
            //            NavChain = $"MainFeedFragment:feed_timeline:1:cold_start:{GdUtilities.GetRowClientTime()}::",
            //        };
            //    else
            //        jsonElements = new JsonElements()
            //        {
            //            //Csrftoken = _Account.CsrfToken,
            //            UserId = userId,
            //            RadioType = "wifi-none",
            //            Uid = _dominatorAccount.AccountBaseModel.UserId,
            //            DeviceId = _Account.Device_Id,
            //            Uuid = _Account.Uuid,
            //            MediaIdAttribution = mediaId,
            //            NavChain = $"MainFeedFragment:feed_timeline:1:cold_start:{GdUtilities.GetRowClientTime()}::",
            //        };

            //    var requestParameter =
            //        (IgRequestParameters)httpHelper.GetRequestParameter();
            //    requestParameter.UrlParameters = new Dictionary<string, string>();
            //    requestParameter.Headers["X-IG-WWW-Claim"] = _Account.WwwClaim;
            //    requestParameter.Body = jsonElements;
            //    requestParameter.Url = $"friendships/create/{(object)userId}/";
            //    var url = requestParameter.GenerateUrl($"friendships/create/{(object)userId}/");
            //    url = Constants.ApiUrl + url;
            //    var postData = requestParameter.GenerateBody();
            //    token.ThrowIfCancellationRequested();
            //    var friendshipsResponse =
            //        new FriendshipsResponse(await httpHelper.PostRequestAsync(url, postData, CancellationToken));
            //    // requestParameter.Headers.Remove("Content-Type");
            //    UpdateImportantHeadersOnEachRequest(requestParameter, _Account);
            //    return friendshipsResponse;
            //}
            //catch (AssertFailedException)
            //{
            //    throw;
            //}
            //catch (Exception ex)
            //{
            //    ex.DebugLog();
            //    return null;
            //}
            #endregion
            FriendshipsResponse friendshipsResponse = new FriendshipsResponse();
            try
            {
                var param = GetWebParameter(_dominatorAccount);
                using (var client = new HttpClient(param.httpClient))
                {
                    client.BaseAddress = new Uri("https://www.instagram.com");

                    // Add headers
                    client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/137.0.0.0 Safari/537.36");
                    client.DefaultRequestHeaders.Referrer = new Uri($"https://www.instagram.com/{mediaId}/");
                    client.DefaultRequestHeaders.Accept.ParseAdd("*/*");
                    client.DefaultRequestHeaders.Add("X-IG-App-ID", "936619743392459");
                    client.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
                    if(!string.IsNullOrEmpty(param.CsrfToken))
                        client.DefaultRequestHeaders.Add("X-CSRFToken", param.CsrfToken);
                    client.DefaultRequestHeaders.Add("X-Instagram-AJAX", GramStatic.InstagramAjax());
                    if(!string.IsNullOrEmpty(param.X_IG_Claim))
                        client.DefaultRequestHeaders.Add("X-IG-WWW-Claim",param.X_IG_Claim);
                    client.DefaultRequestHeaders.Add("Origin", "https://www.instagram.com");
                    client.DefaultRequestHeaders.Add("Sec-Fetch-Site", "same-origin");
                    client.DefaultRequestHeaders.Add("Sec-Fetch-Mode", "cors");
                    client.DefaultRequestHeaders.Add("Sec-Fetch-Dest", "empty");
                    client.DefaultRequestHeaders.Add("Accept-Encoding", GramStatic.AcceptEncoding);
                    // Form data
                    var form = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("user_id", userId),
                        new KeyValuePair<string, string>("jazoest", GramStatic.CreateJazoest(_dominatorAccount?.DeviceDetails?.PhoneId))
                    });

                    var response = await client.PostAsync($"/api/v1/friendships/create/{userId}/", form);
                    var responseBody = await response.Content.ReadAsStreamAsync();
                    var response1 = HttpHelper.Decode(responseBody, string.Join(",", response.Content.Headers.ContentEncoding));
                    friendshipsResponse = new FriendshipsResponse(response1);
                    return friendshipsResponse;
                }
            }
            catch { return friendshipsResponse; }
        }

        public FeedIgResponseHandler GetHashtagFeed(DominatorAccountModel _dominatorAccount, string hashtag,
            string maxid = null)
        {
            try
            {
                if (hashtag.Contains("#"))
                    hashtag = hashtag.TrimStart('#');

                var requestParameters = (IgRequestParameters)httpHelper.GetRequestParameter();
                requestParameters.UrlParameters = new Dictionary<string, string>();
                if (!string.IsNullOrEmpty(maxid))
                    requestParameters.AddUrlParameters("max_id", maxid);

                requestParameters.Url = $"feed/tag/{(object)hashtag}/";
                var url =
                    requestParameters.GenerateUrl(
                        $"feed/tag/{(object)hashtag}/?maxid={null}&rank_token=&ranked_content=true");
                url = Constants.ApiUrl + url;
                return new FeedIgResponseHandler(httpHelper.GetRequest(url));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public async Task<HashTagFeedIgResponseHandler> GetHashtagFeedForUserScraper(DominatorAccountModel _dominatorAccount,
            AccountModel AccountModel, string hashtag, CancellationToken token, int nextpageCount,
            bool recentTag = false, string maxid = null, string nextMediaId = null)
        {
            try
            {
                #region OLD Code for hastag post
                //supported_tabs=["top","recent"]&_csrftoken=oNWT0JnKtU6oE7hfcOfhvHDb4gFahEUG&lat=21.2122853&lng=81.3246947
                //&_uuid=fcecdf58-4663-456c-9adf-e9e9496eb74b&include_persistent=true&rank_token=36073fcc-484d-49f2-a27b-a11d4d2007fa
                //JsonElements jsonElement;
                //hashtag = hashtag.Contains("#") ? hashtag.TrimStart('#') : hashtag;
                //if (string.IsNullOrEmpty(maxid))
                //    jsonElement = new JsonElements
                //    {
                //        SupportedTabs = "[\"top\", \"recent\", \"places\"]",
                //        Csrftoken = AccountModel.CsrfToken,
                //        Uuid = AccountModel.Uuid
                //    };
                //else
                //    jsonElement = new JsonElements
                //    {
                //        MaxId = maxid,
                //        Page = Convert.ToString(nextpageCount),
                //        Tab = "top",
                //        Uuid = AccountModel.Uuid,
                //        IncludePersistent = false,
                //        RankToken = AccountModel.Guid,
                //        NextMediaIds = "[" + nextMediaId + "]"
                //    };


                //var requestParameters = (IgRequestParameters)httpHelper.GetRequestParameter();
                //requestParameters.UrlParameters = new Dictionary<string, string>();
                //requestParameters.Body = jsonElement;
                //requestParameters.Url = $"tags/{hashtag}/sections/";
                //var url = requestParameters.GenerateUrl($"tags/{hashtag}/sections/");
                //url = Constants.ApiUrl + url;
                //requestParameters.DontSign();
                //var postData = requestParameters.GenerateBody();
                //var friendshipsResponse =
                //    new HashTagFeedIgResponseHandler(httpHelper.PostRequest(url, postData));
                //requestParameters.CreateSign();
                //return friendshipsResponse;
                #endregion
                var param = GetWebParameter(_dominatorAccount);
                using (var client = new HttpClient(param.httpClient))
                {
                    var tag = Uri.EscapeDataString(!hashtag.StartsWith("#")?$"#{hashtag}":hashtag);
                    var maxID = string.IsNullOrEmpty(maxid) ? $"&search_session_id={Guid.NewGuid()}" : $"&search_session_id=&next_max_id={maxid}&rank_token={nextMediaId}";
                    var url = $"https://www.instagram.com/api/v1/fbsearch/web/top_serp/?enable_metadata=true&query={tag}{maxID}";

                    var request = new HttpRequestMessage(HttpMethod.Get, url);

                    // Headers
                    request.Headers.Add("Host", "www.instagram.com");
                    request.Headers.Add("Connection", "keep-alive");
                    request.Headers.Add("sec-ch-ua-full-version-list", "\"Google Chrome\";v=\"137.0.7151.104\", \"Chromium\";v=\"137.0.7151.104\", \"Not/A)Brand\";v=\"24.0.0.0\"");
                    request.Headers.Add("sec-ch-ua-platform", "\"Windows\"");
                    request.Headers.Add("viewport-width", "1745");
                    request.Headers.Add("sec-ch-ua", "\"Google Chrome\";v=\"137\", \"Chromium\";v=\"137\", \"Not/A)Brand\";v=\"24\"");
                    request.Headers.Add("sec-ch-ua-model", "\"\"");
                    request.Headers.Add("sec-ch-ua-mobile", "?0");
                    request.Headers.Add("X-IG-App-ID", "936619743392459");
                    request.Headers.Add("X-Requested-With", "XMLHttpRequest");
                    request.Headers.Add("Accept", "*/*");
                    request.Headers.Add("X-CSRFToken", param.CsrfToken);
                    //request.Headers.Add("X-Web-Session-ID", "sjto4f:cyde1c:fpo8x5");
                    //request.Headers.Add("X-ASBD-ID", "359341");
                    request.Headers.Add("dpr", "1.1");
                    request.Headers.Add("sec-ch-prefers-color-scheme", "light");
                    request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/137.0.0.0 Safari/537.36");
                    request.Headers.Add("X-IG-WWW-Claim", param.X_IG_Claim);
                    request.Headers.Add("sec-ch-ua-platform-version", "\"10.0.0\"");
                    request.Headers.Add("Sec-Fetch-Site", "same-origin");
                    request.Headers.Add("Sec-Fetch-Mode", "cors");
                    request.Headers.Add("Sec-Fetch-Dest", "empty");
                    request.Headers.Add("Referer", $"https://www.instagram.com/explore/search/keyword/?q={tag}");
                    request.Headers.Add("Origin", "https://www.instagram.com");
                    request.Headers.Add("Accept-Encoding", GramStatic.AcceptEncoding);
                    request.Headers.Add("Accept-Language", "en-GB,en-US;q=0.9,en;q=0.8");
                    // Send request
                    var response = await client.SendAsync(request);
                    var content = await response.Content.ReadAsStreamAsync();
                    var responseText = HttpHelper.Decode(content, string.Join(",", response.Content.Headers.ContentEncoding));
                    return new HashTagFeedIgResponseHandler(responseText);
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public HashTagFeedIgResponseHandler GetHashtagFeedForRecentUserScraper(DominatorAccountModel _dominatorAccount,
            AccountModel AccountModel, string hashtag, int nextpageCount, string maxid = null,
            string nextMediaId = null)
        {
            try
            {
                JsonElements jsonElement;
                if (hashtag.Contains("#"))
                    hashtag = hashtag.TrimStart('#');
                if (string.IsNullOrEmpty(maxid))
                    jsonElement = new JsonElements
                    {
                        Tab = "recent",
                        Uuid = AccountModel.Uuid,
                        IncludePersistent = false,
                    };
                else
                    jsonElement = new JsonElements
                    {
                        MaxId = maxid,
                        Tab = "recent",
                        Uuid = AccountModel.Uuid,
                        Page = Convert.ToString(nextpageCount),
                    };
                //_csrftoken=oNWT0JnKtU6oE7hfcOfhvHDb4gFahEUG&lat=21.2122853&lng=81.3246947&tab=recent&_uuid=fcecdf58-4663-456c-9adf-e9e9496eb74b&include_persistent=false
                //&rank_token=8f4c70f8-ad73-4e0e-8be5-cbe0d51acf1e

                var requestParameters = (IgRequestParameters)httpHelper.GetRequestParameter();
                requestParameters.UrlParameters = new Dictionary<string, string>();
                requestParameters.Body = jsonElement;
                requestParameters.Url = $"tags/{hashtag}/sections/";
                var url = requestParameters.GenerateUrl($"tags/{hashtag}/sections/");
                url = Constants.ApiUrl + url;
                requestParameters.DontSign();
                var postData = requestParameters.GenerateBody();
                var friendshipsResponse =
                    new HashTagFeedIgResponseHandler(httpHelper.PostRequest(url, postData));
                requestParameters.CreateSign();
                return friendshipsResponse;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public async Task<FeedIgResponseHandlerAlternate> GetLocationFeedAlternate(DominatorAccountModel _dominatorAccount,
            AccountModel _Account,
            string locationId, CancellationToken token, string maxid = null, string page = null, string PostUrl = null)
        {
            try
            {
                var IsUrl = locationId.Contains("locations");
                if (IsUrl)
                {
                    PostUrl = locationId;
                    locationId = Regex.Match(locationId, "[0-9]+")?.Value;
                }
                var MaxID = string.Empty;
                var DocID = "9605751372876865";
                if (!string.IsNullOrEmpty(maxid))
                {
                    MaxID = $"\"after\":\"{maxid}\",\"first\":12,";
                    DocID = "23964501783185348";
                }
                //{"location_id":"{locationId}","page_size_override":6,"tab":"ranked"}
                //{"after":"8e02314603704b87a03eff140f52e451","first":12,"location_id":"102947824793205","page_size_override":6,"tab":"ranked"}
                var variables = Uri.EscapeDataString($"{{{MaxID}\"location_id\":\"{locationId}\",\"page_size_override\":6,\"tab\":\"ranked\"}}");
                #region OLD Code for location feed
                //JsonElements jsonElements = null;
                //if (maxid == null)
                //    jsonElements = new JsonElements()
                //    {
                //        Uuid = _Account.Uuid,
                //        Tab = "recent",
                //        SessionId = _Account.AdId,
                //        Uid = _dominatorAccount.AccountBaseModel.UserId,
                //    };
                //else
                //    jsonElements = new JsonElements()
                //    {
                //        MaxId = maxid,
                //        Tab = "recent",
                //        Page = page,
                //        Uuid = _Account.Uuid,
                //        Uid = _dominatorAccount.AccountBaseModel.UserId,
                //        NextMediaIds = "[]",
                //        SessionId = _Account.AdId
                //    };
                //var requestParameters = (IgRequestParameters)httpHelper.GetRequestParameter();
                //requestParameters.UrlParameters = new Dictionary<string, string>();
                //requestParameters.Body = jsonElements;
                //requestParameters.Headers.Add("Content-Type", " application/x-www-form-urlencoded; charset=UTF-8");
                //requestParameters.Url = $"locations/{(object)locationId}/sections/";
                //var url = requestParameters.GenerateUrl($"locations/{(object)locationId}/sections/");
                //url = Constants.ApiUrl + url;
                //requestParameters.DontSign();
                //var postData = requestParameters.GenerateBody();
                //var friendshipsResponse =
                //    new FeedIgResponseHandlerAlternate(httpHelper.PostRequest(url, postData));
                //requestParameters.Headers.Remove("Content-Type");
                //requestParameters.CreateSign();
                //return friendshipsResponse;
                #endregion
                var param = GetWebParameter(_dominatorAccount);
                using (var client = new HttpClient(param.httpClient))
                {
                    var request = new HttpRequestMessage(HttpMethod.Post, "https://www.instagram.com/graphql/query");
                    request.Headers.Add("Host", "www.instagram.com");
                    request.Headers.Add("Connection", "keep-alive");
                    request.Headers.Add("sec-ch-ua-full-version-list", "\"Google Chrome\";v=\"137.0.7151.120\", \"Chromium\";v=\"137.0.7151.120\", \"Not/A)Brand\";v=\"24.0.0.0\"");
                    request.Headers.Add("sec-ch-ua-platform", "\"Windows\"");
                    request.Headers.Add("viewport-width", "1745");
                    request.Headers.Add("X-Root-Field-Name", "xdt_location_get_web_info_tab");
                    request.Headers.Add("sec-ch-ua", "\"Google Chrome\";v=\"137\", \"Chromium\";v=\"137\", \"Not/A)Brand\";v=\"24\"");
                    request.Headers.Add("sec-ch-ua-model", "");
                    request.Headers.Add("sec-ch-ua-mobile", "?0");
                    request.Headers.Add("X-IG-App-ID", "936619743392459");
                    request.Headers.Add("X-CSRFToken", param.CsrfToken);
                    request.Headers.Add("X-FB-Friendly-Name", "PolarisLocationPageTabContentQuery");
                    request.Headers.Add("X-BLOKS-VERSION-ID", "80875d0dbff5faa964af32b8cd7ab9a8e5ba53e71cf6430c7661fe77ca7f40d1");
                    request.Headers.Add("dpr", "1.1");
                    request.Headers.Add("sec-ch-prefers-color-scheme", "light");
                    request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/137.0.0.0 Safari/537.36");
                    request.Headers.Add("sec-ch-ua-platform-version", "\"10.0.0\"");
                    request.Headers.Add("Accept", "*/*");
                    request.Headers.Add("Origin", "https://www.instagram.com");
                    request.Headers.Add("Sec-Fetch-Site", "same-origin");
                    request.Headers.Add("Sec-Fetch-Mode", "cors");
                    request.Headers.Add("Sec-Fetch-Dest", "empty");
                    if(!string.IsNullOrEmpty(PostUrl)|| IsUrl)
                        request.Headers.Add("Referer",PostUrl);
                    request.Headers.Add("Accept-Encoding", GramStatic.AcceptEncoding);
                    request.Headers.Add("Accept-Language", "en-GB,en-US;q=0.9,en;q=0.8");
                    // Raw form content (URL encoded)
                    var postData = "__user=0&__a=1&__req=1o&__hs=20270.HYP%3Ainstagram_web_pkg.2.1...0" +
                        "&dpr=1&__ccg=EXCELLENT" +
                        "&fb_api_req_friendly_name=PolarisLocationPageTabContentQuery&" +  // Truncated for brevity
                        $"variables={variables}" +
                        $"&server_timestamps=true&doc_id={DocID}";

                    request.Content = new StringContent(postData, Encoding.UTF8, "application/x-www-form-urlencoded");

                    var response = await client.SendAsync(request);
                    var ResponseData = await HttpHelper.Decode(response);
                    return new FeedIgResponseHandlerAlternate(ResponseData);
                }
            }
            catch (AssertFailedException)
            {
                throw;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }
        public MediaCommentsIgResponseHandler GetMediaComments(DominatorAccountModel _dominatorAccount, string mediaId,
            CancellationToken token, string maxid = null)
        {
            token.ThrowIfCancellationRequested();
            var ResponseString = string.Empty;
            try
            {
                //var requestParameters = (IgRequestParameters)httpHelper.GetRequestParameter();
                //requestParameters.UrlParameters = new Dictionary<string, string>();
                //var url = string.Empty;
                //requestParameters.AddUrlParameters("ig_sig_key_version", Constants.SIG_KEY_VERSION);
                //if (maxid != null)
                //{
                //    requestParameters.AddUrlParameters("max_id", maxid);
                //    requestParameters.Url =
                //        $"media/{(object)mediaId}/comments/?min_id={maxid}&can_support_threading=true";
                //    url = Constants.ApiUrl + requestParameters.Url;
                //}
                //else
                //{
                //    requestParameters.Url = $"media/{(object)mediaId}/comments/?can_support_threading=true";
                //    url = Constants.ApiUrl + requestParameters.Url;
                //}

                //token.ThrowIfCancellationRequested();
                var splitted = mediaId.Split('/').ToList();
                var code = splitted != null && splitted.Count > 4 ? splitted[4]:splitted?.LastOrDefault();
                var url = $"https://www.instagram.com/graphql/query/?query_hash=33ba35852cb50da46f5b5e889df7d159&variables=%7B%22shortcode%22:%22{code}%22,%22first%22:50,%22after%22:%22{maxid}%22%7D";
                var param = GetWebParameter(_dominatorAccount);
                using (var client = new HttpClient(param.httpClient))
                {
                    // Add headers
                    client.DefaultRequestHeaders.Add("x-instagram-ajax", "1");
                    client.DefaultRequestHeaders.Add("sec-ch-ua-platform", "\"Windows\"");
                    client.DefaultRequestHeaders.Add("x-csrftoken", param.CsrfToken);
                    client.DefaultRequestHeaders.Add("sec-ch-ua", "\"Google Chrome\";v=\"135\", \"Not-A.Brand\";v=\"8\", \"Chromium\";v=\"135\"");
                    client.DefaultRequestHeaders.Add("sec-ch-ua-mobile", "?0");
                    client.DefaultRequestHeaders.Add("x-ig-app-id", "936619743392459");
                    client.DefaultRequestHeaders.Add("x-asbd-id", "198387");
                    client.DefaultRequestHeaders.Add("x-requested-with", "XMLHttpRequest");
                    client.DefaultRequestHeaders.Add("Accept-Encoding", GramStatic.AcceptEncoding);
                    client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/135.0.0.0 Safari/537.36");
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    if (!string.IsNullOrEmpty(param.X_IG_Claim))
                        client.DefaultRequestHeaders.Add("x-ig-www-claim", param.X_IG_Claim);

                    try
                    {
                        var response = client.GetAsync(url).Result;
                        response.EnsureSuccessStatusCode();
                        var stream = response.Content.ReadAsStreamAsync().Result;
                        ResponseString = HttpHelper.Decode(stream, string.Join(", ", response.Content.Headers.ContentEncoding)).Response;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: " + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
            return new MediaCommentsIgResponseHandler(new ResponseParameter { Response = ResponseString });
        }


        public MediaLikersIgResponseHandler GetMediaLikers(DominatorAccountModel _dominatorAccount, string mediaId,
            CancellationToken token, string maxid = null)
        {
            token.ThrowIfCancellationRequested();
            var ResponseText = string.Empty;
            try
            {
                //var requestParameters =
                //    (IgRequestParameters)httpHelper.GetRequestParameter();
                //requestParameters.UrlParameters = new Dictionary<string, string>();

                //if (maxid != null) requestParameters.AddUrlParameters("max_id", maxid);

                //requestParameters.Url = $"media/{(object)mediaId}/likers/";
                //var url = requestParameters.GenerateUrl($"media/{(object)mediaId}/likers/");
                //url = Constants.ApiUrl + url;
                //token.ThrowIfCancellationRequested();
                //return new MediaLikersIgResponseHandler(httpHelper.GetRequest(url));
                var param = GetWebParameter(_dominatorAccount);
                using (var client = new HttpClient(param.httpClient))
                {
                    var uri = new Uri($"https://www.instagram.com/api/v1/media/{mediaId}/likers/");
                    // Set headers
                    client.DefaultRequestHeaders.Add("x-instagram-ajax", "1");
                    client.DefaultRequestHeaders.Add("sec-ch-ua-platform", "\"Windows\"");
                    client.DefaultRequestHeaders.Add("sec-ch-ua", "\"Google Chrome\";v=\"135\", \"Not-A.Brand\";v=\"8\", \"Chromium\";v=\"135\"");
                    client.DefaultRequestHeaders.Add("sec-ch-ua-mobile", "?0");
                    client.DefaultRequestHeaders.Add("X-CSRFToken", param.CsrfToken);
                    client.DefaultRequestHeaders.Add("X-IG-App-ID", "936619743392459");
                    client.DefaultRequestHeaders.Add("x-requested-with", "XMLHttpRequest");
                    client.DefaultRequestHeaders.Add("Sec-Fetch-Site", "none");
                    client.DefaultRequestHeaders.Add("Sec-Fetch-Mode", "cors");
                    client.DefaultRequestHeaders.Add("Sec-Fetch-Dest", "empty");
                    client.DefaultRequestHeaders.Add("Sec-Fetch-Storage-Access", "active");
                    client.DefaultRequestHeaders.Add("Accept-Encoding", GramStatic.AcceptEncoding);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/135.0.0.0 Safari/537.36");

                    try
                    {
                        var response = client.GetAsync(uri).Result;
                        var stream = response.Content.ReadAsStreamAsync().Result;
                        ResponseText = HttpHelper.Decode(stream, string.Join(", ", response.Content.Headers.ContentEncoding)).Response;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Request failed: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return new MediaLikersIgResponseHandler(new ResponseParameter { Response = ResponseText });
        }

        public SuggestedUsersIgResponseHandler GetSuggestedUsers1(DominatorAccountModel _dominatorAccount,
            AccountModel _Account, string max_id, CancellationToken token)
        {
            try
            {
                JsonElements jsonElements;
                if (string.IsNullOrEmpty(max_id))
                    jsonElements = new JsonElements
                    {
                        PhoneId = _Account.PhoneId,
                        Module = "discover_people",
                        Csrftoken = _Account.CsrfToken,
                        Uuid = _Account.Uuid,
                        paginate = "true"
                    };
                else
                    jsonElements = new JsonElements
                    {
                        PhoneId = _Account.PhoneId,
                        MaxId = max_id,
                        Module = "discover_people",
                        Csrftoken = _Account.CsrfToken,
                        Uuid = _Account.Uuid,
                        paginate = "true"
                    };

                var requestParameter = (IgRequestParameters)httpHelper.GetRequestParameter();
                requestParameter.UrlParameters = new Dictionary<string, string>();
                requestParameter.Body = jsonElements;
                requestParameter.DontSign();

                requestParameter.Url = "discover/ayml/";
                var url = requestParameter.GenerateUrl("discover/ayml/");
                url = Constants.ApiUrl + url;
                var postData = requestParameter.GenerateBody();
                requestParameter.CreateSign();
                var suggestedUsersIgResponseHandler =
                    new SuggestedUsersIgResponseHandler(httpHelper.PostRequest(url, postData));
                if (string.IsNullOrEmpty(suggestedUsersIgResponseHandler.ToString()))
                {
                    delayService.ThreadSleep(TimeSpan.FromSeconds(15));
                    //Thread.Sleep(TimeSpan.FromSeconds(15));//Thread.Sleep(TimeSpan.FromSeconds(5));
                    suggestedUsersIgResponseHandler = suggestedUsersIgResponseHandler =
                        new SuggestedUsersIgResponseHandler(httpHelper.PostRequest(url, postData));
                }

                return suggestedUsersIgResponseHandler;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public async Task<SuggestedUsersIgResponseHandler> GetSuggestedUsers(DominatorAccountModel _dominatorAccount,
            AccountModel _Account, string userId, CancellationToken token, string max_id)
        {
            try
            {
                #region OLD Code.
                //var requestParameter = (IgRequestParameters)httpHelper.GetRequestParameter();
                //requestParameter.UrlParameters = new Dictionary<string, string>();
                ////https://i.instagram.com/api/v1/discover/chaining/?target_id=510093911
                ////https://i.instagram.com/api/v1/discover/chaining/?target_id=5977372153 
                //requestParameter.Url = $"discover/chaining/?module=profile&target_id={userId}";
                //var url = requestParameter.GenerateUrl($"discover/chaining/?module=profile&target_id={userId}");
                //url = Constants.ApiUrl + url;
                //var suggestedUsersIgResponseHandler =
                //    new SuggestedUsersIgResponseHandler(httpHelper.GetRequest(url));
                //if (string.IsNullOrEmpty(suggestedUsersIgResponseHandler.ToString()))
                //{
                //    delayService.ThreadSleep(TimeSpan.FromSeconds(5));

                //    //Thread.Sleep(TimeSpan.FromSeconds(5));//Thread.Sleep(TimeSpan.FromSeconds(5));
                //    suggestedUsersIgResponseHandler = suggestedUsersIgResponseHandler =
                //        new SuggestedUsersIgResponseHandler(httpHelper.GetRequest(url));
                //}
                //return suggestedUsersIgResponseHandler;
                #endregion
                var param = GetWebParameter(_dominatorAccount);
                using (var client = new HttpClient(param.httpClient))
                {
                    var request = new HttpRequestMessage(HttpMethod.Post, "https://www.instagram.com/api/v1/discover/ayml/");

                    request.Headers.Add("Host", "www.instagram.com");
                    request.Headers.Add("Connection", "keep-alive");
                    request.Headers.Add("sec-ch-ua-full-version-list", "\"Google Chrome\";v=\"137.0.7151.104\", \"Chromium\";v=\"137.0.7151.104\", \"Not/A)Brand\";v=\"24.0.0.0\"");
                    request.Headers.Add("sec-ch-ua-platform", "\"Windows\"");
                    request.Headers.Add("viewport-width", "1745");
                    request.Headers.Add("sec-ch-ua", "\"Google Chrome\";v=\"137\", \"Chromium\";v=\"137\", \"Not/A)Brand\";v=\"24\"");
                    request.Headers.Add("sec-ch-ua-model", "\"\"");
                    request.Headers.Add("sec-ch-ua-mobile", "?0");
                    request.Headers.Add("X-IG-App-ID", "936619743392459");
                    request.Headers.Add("X-Requested-With", "XMLHttpRequest");
                    request.Headers.Add("Accept", "*/*");
                    request.Headers.Add("X-Instagram-AJAX", GramStatic.InstagramAjax());
                    request.Headers.Add("X-CSRFToken",param.CsrfToken);
                    //request.Headers.Add("X-Web-Session-ID", "ovr2f8:cyde1c:7ulytt");
                    //request.Headers.Add("X-ASBD-ID", "359341");
                    request.Headers.Add("dpr", "1.1");
                    request.Headers.Add("sec-ch-prefers-color-scheme", "light");
                    request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/137.0.0.0 Safari/537.36");
                    request.Headers.Add("X-IG-WWW-Claim", "0");
                    request.Headers.Add("sec-ch-ua-platform-version", "\"10.0.0\"");
                    request.Headers.Add("Origin", "https://www.instagram.com");
                    request.Headers.Add("Sec-Fetch-Site", "same-origin");
                    request.Headers.Add("Sec-Fetch-Mode", "cors");
                    request.Headers.Add("Sec-Fetch-Dest", "empty");
                    request.Headers.Add("Referer", "https://www.instagram.com/explore/people");
                    request.Headers.Add("Accept-Encoding", GramStatic.AcceptEncoding);
                    request.Headers.Add("Accept-Language", "en-GB,en-US;q=0.9,en;q=0.8");
                    var maxID = string.IsNullOrEmpty(max_id) ? "max_id=[]&" : $"max_id={Uri.EscapeDataString(max_id)}&";
                    var postData = $"{maxID}max_number_to_display=30&module=discover_people&paginate=true&jazoest={param.Jazoest}";
                    request.Content = new StringContent(postData, Encoding.UTF8, "application/x-www-form-urlencoded");
                    var response = await client.SendAsync(request);
                    var responseBody = await response.Content.ReadAsStreamAsync();
                    var responseString = HttpHelper.Decode(responseBody, string.Join(", ", response.Content.Headers.ContentEncoding));
                    return new SuggestedUsersIgResponseHandler(responseString,false);
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public UserFeedIgResponseHandler GetUserFeed(DominatorAccountModel
                _dominatorAccount, AccountModel _Account, string userId, CancellationToken token, string maxId = null,
            string minTimestamp = null, bool isNewUserBrowser = false)
        {
            return GetUserFeedAsync(_dominatorAccount, _Account, userId, token, maxId, minTimestamp, isNewUserBrowser)
                .Result;
        }

        public async Task<UserFeedIgResponseHandler> GetUserFeedAsync(DominatorAccountModel _dominatorAccount,
            AccountModel _Account, string userId, CancellationToken token, string maxId = null,
            string minTimestamp = null, bool isNewBrowser = false)
        {
            UserFeedIgResponseHandler userFeedResponse = new UserFeedIgResponseHandler();
            var posts = new List<InstagramPost>();
            var hasMore = true;
            try
            {
                #region OLD Code.

                //while (true)
                //{
                //    var requestParameter =
                //        (IgRequestParameters)httpHelper.GetRequestParameter();
                //    requestParameter.UrlParameters = new Dictionary<string, string>();
                //    requestParameter.AddUrlParameters("exclude_comment", "false");
                //    requestParameter.AddUrlParameters("only_fetch_first_carousel_media", "false");
                //    if (maxId != null)
                //        requestParameter.AddUrlParameters("max_id", maxId);
                //    if (minTimestamp != null)
                //        requestParameter.AddUrlParameters("min_timestamp", minTimestamp);

                //    requestParameter.Url = $"feed/user/{(object)userId}/";
                //    var url = requestParameter.GenerateUrl($"feed/user/{(object)userId}/");
                //    url = Constants.ApiUrl + url;
                //    //https://i.instagram.com/api/v1/feed/user/7425066841/?exclude_comment=false&only_fetch_first_carousel_media=false
                //    //https://i.instagram.com/api/v1/feed/user/12499625/?exclude_comment=false&only_fetch_first_carousel_media=false
                //    //https://i.instagram.com/api/v1/feed/user/12499625/?rank_token=&ranked_content=true
                //    userFeedResponse =
                //        new UserFeedIgResponseHandler(await httpHelper.GetRequestAsync(url, CancellationToken));
                //    UpdateImportantHeadersOnEachRequest(requestParameter, _Account);
                //    if (!userFeedResponse.Success && userFeedResponse.Issue.Error == InstagramError.RateLimit)
                //        Thread.Sleep(Constants.FloodWait);
                //    else
                //        break;
                //}

                #endregion
                var param = GetWebParameter(_dominatorAccount);
                while (hasMore && posts.Count < 100)
                {
                    var max = string.IsNullOrEmpty(maxId) ? "" : $"&max_id={maxId}";
                    using (var client = new HttpClient(param.httpClient))
                    {
                        client.BaseAddress = new Uri("https://i.instagram.com");

                        // Add headers
                        client.DefaultRequestHeaders.Add("x-asbd-id", "359341");
                        if (!string.IsNullOrEmpty(param.CsrfToken))
                            client.DefaultRequestHeaders.Add("x-csrftoken", param.CsrfToken);
                        client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/136.0.0.0 Safari/537.36");
                        //client.DefaultRequestHeaders.Add("content-type", "application/x-www-form-urlencoded");
                        client.DefaultRequestHeaders.Add("x-ig-app-id", "936619743392459");
                        if (!string.IsNullOrEmpty(_dominatorAccount?.DeviceDetails?.IGXClaim))
                            client.DefaultRequestHeaders.Add("x-ig-www-claim", _dominatorAccount.DeviceDetails.IGXClaim);
                        client.DefaultRequestHeaders.Add("Accept", "*/*");
                        client.DefaultRequestHeaders.Add("Accept-Encoding", GramStatic.AcceptEncoding);
                        client.DefaultRequestHeaders.Add("Accept-Language", "en-GB,en-US;q=0.9,en;q=0.8");
                        var endpoint = $"/api/v1/feed/user/{userId}/username/?count=12{max}";
                        var response = await client.GetAsync(endpoint);

                        response.EnsureSuccessStatusCode();

                        var responseBody = await response.Content.ReadAsStreamAsync();
                        var responseString = HttpHelper.Decode(responseBody, string.Join(", ", response.Content.Headers.ContentEncoding));
                        var responseData = new UserFeedIgResponseHandler(responseString, true);
                        posts.AddRange(responseData.Items);
                        hasMore = responseData.HasMoreResults;
                        maxId = responseData.MaxId;
                        await Task.Delay(TimeSpan.FromSeconds(3), _dominatorAccount.Token);
                        param = GetWebParameter(_dominatorAccount);
                    }
                }

            }
            catch (Exception)
            {
            }
            finally
            {
                userFeedResponse.Items = posts;
                userFeedResponse.MaxId = maxId;
                userFeedResponse.Success = posts.Count > 0;
                userFeedResponse.HasMoreResults = hasMore;
            }
            return userFeedResponse;
        }

        public async Task<FollowerAndFollowingIgResponseHandler> GetUserFollowers(DominatorAccountModel _dominatorAccount,
            string usernameId, CancellationToken token,string maxid = null, string ProfileID = "", bool IsWeb = false)
        {
            return await GetUserFollowersAsync(_dominatorAccount, usernameId, token, maxid, ProfileID,IsWeb);
        }

        public async Task<FollowerAndFollowingIgResponseHandler> GetUserFollowersAsync(
            DominatorAccountModel _dominatorAccount, string usernameId, CancellationToken token,
            string maxid = null, string ProfileID = "", bool IsWeb = false)
        {
            try
            {
                FollowerAndFollowingIgResponseHandler followerResponse;
                #region OLD Followers Code.
                //int counter = 0;
                //while (true)
                //{
                //    counter++;
                //    var requestParameter =
                //        (IgRequestParameters)httpHelper.GetRequestParameter();
                //    requestParameter.UrlParameters = new Dictionary<string, string>();

                //    if (maxid != null)
                //        requestParameter.AddUrlParameters("max_id", maxid);

                //    requestParameter.Url = $"friendships/{usernameId}/following/?includes_hashtags=true&search_surface=follow_list_page&query=&enable_groups=true&rank_token={Utilities.GetGuid()}";
                //    var url = requestParameter.GenerateUrl($"friendships/{usernameId}/following/?includes_hashtags=true&search_surface=follow_list_page&query=&enable_groups=true&rank_token={Utilities.GetGuid()}");
                //    url = Constants.ApiUrl + url;
                //    //144 version request//https://i.instagram.com/api/v1/friendships/6975636384/followers/?search_surface=follow_list_page&order=default&query=&enable_groups=true&rank_token=f091ff9c-5aab-4d25-a0d3-dbb67ff5199c 
                //    followerResponse =
                //        new FollowerAndFollowingIgResponseHandler(
                //            await httpHelper.GetRequestAsync(url, CancellationToken));
                //    if (string.IsNullOrEmpty(followerResponse.ToString()))
                //    {
                //        delayService.ThreadSleep(TimeSpan.FromSeconds(5));
                //        // Thread.Sleep(TimeSpan.FromSeconds(5));//Thread.Sleep(TimeSpan.FromSeconds(5));
                //        followerResponse =
                //            new FollowerAndFollowingIgResponseHandler(
                //                await httpHelper.GetRequestAsync(url, CancellationToken));
                //    }
                //    UpdateImportantHeadersOnEachRequest(requestParameter, _Account);
                //    if (!followerResponse.Success && followerResponse.Issue != null && !followerResponse.ToString()
                //            .Contains("Please wait a few minutes before you try again"))
                //    {
                //        if (followerResponse.Issue.Error == InstagramError.RateLimit)
                //            Thread.Sleep(Constants.FloodWait);
                //    }
                //    else
                //    {
                //        break;
                //    }
                //    if (counter > 5)
                //    {
                //        break;
                //    }
                //}
                #endregion
                IsWeb = GramStatic.GetWebFollowerFollowing;
                var param = GetWebParameter(_dominatorAccount);
                if (IsWeb)
                {
                    var postBody = $"{{\"id\":\"{usernameId}\",\"after\":\"{maxid}\",\"first\":50}}";
                    var url = $"https://www.instagram.com/graphql/query/?query_hash=37479f2b8209594dde7facb0d904896a&variables={Uri.EscapeDataString(postBody)}";
                    using (var client = new HttpClient(param.httpClient))
                    {
                        // Required headers
                        client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/137.0.0.0 Safari/537.36");
                        client.DefaultRequestHeaders.Add("Accept", "*/*");
                        client.DefaultRequestHeaders.Add("sec-ch-ua-platform", "\"Windows\"");
                        client.DefaultRequestHeaders.Add("sec-ch-ua", "\"Google Chrome\";v=\"137\", \"Chromium\";v=\"137\", \"Not/A)Brand\";v=\"24\"");
                        client.DefaultRequestHeaders.Add("sec-ch-ua-mobile", "?0");
                        client.DefaultRequestHeaders.Add("Sec-Fetch-Site", "none");
                        client.DefaultRequestHeaders.Add("Sec-Fetch-Mode", "cors");
                        client.DefaultRequestHeaders.Add("Sec-Fetch-Dest", "empty");
                        client.DefaultRequestHeaders.Add("Sec-Fetch-Storage-Access", "active");
                        client.DefaultRequestHeaders.Add("Accept-Encoding", GramStatic.AcceptEncoding);
                        client.DefaultRequestHeaders.Add("Accept-Language", "en-GB,en-US;q=0.9,en;q=0.8");
                        var response = await client.GetAsync(url);
                        var ResponseData = await HttpHelper.Decode(response);
                        return new FollowerAndFollowingIgResponseHandler(ResponseData, IsWeb:IsWeb);
                    }
                }
                else
                {
                    var MaxID = string.IsNullOrEmpty(maxid) ? "&search_surface=follow_list_page" : $"&max_id={Uri.EscapeDataString(maxid)}";

                    var url = $"https://www.instagram.com/api/v1/friendships/{usernameId}/followers/?count=12{MaxID}";

                    using (var client = new HttpClient(param.httpClient))
                    {
                        // Set headers
                        client.DefaultRequestHeaders.Add("X-IG-App-ID", "936619743392459");
                        client.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
                        client.DefaultRequestHeaders.Add("X-CSRFToken", param.CsrfToken);
                        //client.DefaultRequestHeaders.Add("X-Web-Session-ID", "aczwqp:1s12oh:g48gav");
                        //client.DefaultRequestHeaders.Add("X-ASBD-ID", "359341");
                        if (!string.IsNullOrEmpty(param.X_IG_Claim))
                            client.DefaultRequestHeaders.Add("X-IG-WWW-Claim", param.X_IG_Claim);
                        client.DefaultRequestHeaders.Add("sec-ch-ua-full-version-list", "\"Google Chrome\";v=\"137.0.7151.104\", \"Chromium\";v=\"137.0.7151.104\", \"Not/A)Brand\";v=\"24.0.0.0\"");
                        client.DefaultRequestHeaders.Add("sec-ch-ua", "\"Google Chrome\";v=\"137\", \"Chromium\";v=\"137\", \"Not/A)Brand\";v=\"24\"");
                        client.DefaultRequestHeaders.Add("sec-ch-ua-platform", "\"Windows\"");
                        client.DefaultRequestHeaders.Add("sec-ch-ua-platform-version", "\"10.0.0\"");
                        client.DefaultRequestHeaders.Add("sec-ch-ua-model", "\"\"");
                        client.DefaultRequestHeaders.Add("sec-ch-ua-mobile", "?0");
                        client.DefaultRequestHeaders.Add("sec-ch-prefers-color-scheme", "light");
                        client.DefaultRequestHeaders.Add("Referer", $"https://www.instagram.com/{ProfileID}/followers/");
                        client.DefaultRequestHeaders.Add("Origin", "https://www.instagram.com");
                        client.DefaultRequestHeaders.Add("Accept-Encoding", GramStatic.AcceptEncoding);
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
                        client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/137.0.0.0 Safari/537.36");
                        var response = await client.GetAsync(url);
                        var responseBody = await response.Content.ReadAsStreamAsync();
                        var response1 = HttpHelper.Decode(responseBody, string.Join(",", response.Content.Headers.ContentEncoding));
                        followerResponse = new FollowerAndFollowingIgResponseHandler(response1);
                        return followerResponse;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public async Task<FollowerAndFollowingIgResponseHandler> GetUserFollowings(DominatorAccountModel _dominatorAccount,
            AccountModel accountModel, string usernameId, CancellationToken token, string QueryType = null,
            string maxid = null, string ProfileID = "", bool IsWeb = false)
        {
            return await GetUserFollowingsAsync(_dominatorAccount, accountModel, usernameId, token, QueryType, maxid,ProfileID, IsWeb);
        }

        public async Task<FollowerAndFollowingIgResponseHandler> GetUserFollowingsAsync(
            DominatorAccountModel _dominatorAccount, AccountModel _Account, string usernameId, CancellationToken token,
            string QueryType = null, string maxid = null, string ProfileID = "", bool IsWeb = false)
        {
            try
            {
                FollowerAndFollowingIgResponseHandler followingResponse;
                #region OLD Following Code.
                //var url = string.Empty;
                //while (true)
                //{
                //    var requestParameter =
                //        (IgRequestParameters)httpHelper.GetRequestParameter();
                //    requestParameter.UrlParameters = new Dictionary<string, string>();
                //    requestParameter.AddUrlParameters("rank_token", _Account.RankToken);
                //    if (maxid != null)
                //    {
                //        requestParameter.AddUrlParameters("max_id", maxid);
                //        url = $"friendships/{usernameId}/followers/?search_surface=follow_list_page&query=&enable_groups=true&rank_token={Utilities.GetGuid()}&max_id={maxid}";
                //    }
                //    else
                //    {
                //        url = $"friendships/{usernameId}/followers/?search_surface=follow_list_page&query=&enable_groups=true&rank_token={Utilities.GetGuid()}";
                //    }

                //    //144 version request//https://i.instagram.com/api/v1/friendships/6975636384/following/?includes_hashtags=true&search_surface=follow_list_page&query=&enable_groups=true&rank_token=b338b366-6829-4f51-b629-311e8c2e20ce
                //    url = Constants.ApiUrl + url;
                //    followingResponse =
                //        new FollowerAndFollowingIgResponseHandler(
                //            await httpHelper.GetRequestAsync(url, CancellationToken));
                //    if (string.IsNullOrEmpty(followingResponse.ToString()))
                //    {
                //        delayService.ThreadSleep(TimeSpan.FromSeconds(5));
                //        //Thread.Sleep(TimeSpan.FromSeconds(5));// Thread.Sleep(TimeSpan.FromSeconds(5));
                //        followingResponse =
                //            new FollowerAndFollowingIgResponseHandler(
                //                await httpHelper.GetRequestAsync(url, CancellationToken));
                //    }
                //    UpdateImportantHeadersOnEachRequest(requestParameter, _Account);
                //    if (!followingResponse.Success && followingResponse.Issue.Error == InstagramError.RateLimit &&
                //        !followingResponse.ToString().Contains("Please wait a few minutes before you try again."))
                //        Thread.Sleep(Constants.FloodWait);
                //    else
                //        break;
                //}
                #endregion
                IsWeb = GramStatic.GetWebFollowerFollowing;
                var param = GetWebParameter(_dominatorAccount);
                if (IsWeb)
                {
                    var postBody = $"{{\"id\":\"{usernameId}\",\"after\":\"{maxid}\",\"first\":50}}";
                    var url = $"https://www.instagram.com/graphql/query/?query_hash=58712303d941c6855d4e888c5f0cd22f&variables={Uri.EscapeDataString(postBody)}";

                    using (var client = new HttpClient(param.httpClient))
                    {
                        // Set headers
                        client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/137.0.0.0 Safari/537.36");
                        client.DefaultRequestHeaders.Add("Accept", "*/*");
                        client.DefaultRequestHeaders.Add("sec-ch-ua-platform", "\"Windows\"");
                        client.DefaultRequestHeaders.Add("sec-ch-ua", "\"Google Chrome\";v=\"137\", \"Chromium\";v=\"137\", \"Not/A)Brand\";v=\"24\"");
                        client.DefaultRequestHeaders.Add("sec-ch-ua-mobile", "?0");
                        client.DefaultRequestHeaders.Add("Sec-Fetch-Site", "none");
                        client.DefaultRequestHeaders.Add("Sec-Fetch-Mode", "cors");
                        client.DefaultRequestHeaders.Add("Sec-Fetch-Dest", "empty");
                        client.DefaultRequestHeaders.Add("Sec-Fetch-Storage-Access", "active");
                        client.DefaultRequestHeaders.Add("Accept-Language", "en-GB,en-US;q=0.9,en;q=0.8");
                        client.DefaultRequestHeaders.Add("Accept-Encoding", GramStatic.AcceptEncoding);
                        var response = await client.GetAsync(url);
                        var ResponseData = await HttpHelper.Decode(response);
                        return new FollowerAndFollowingIgResponseHandler(ResponseData,IsWeb:IsWeb);
                    }
                }
                else
                {
                    var maxID = string.IsNullOrEmpty(maxid) ? "" : $"&max_id={maxid}";
                    var url = $"https://www.instagram.com/api/v1/friendships/{usernameId}/following/?count=12{maxID}";
                    using (var client = new HttpClient(param.httpClient))
                    {
                        var baseUri = new Uri("https://www.instagram.com");
                        // ✅ Headers
                        client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/137.0.0.0 Safari/537.36");
                        if (!string.IsNullOrEmpty(param.CsrfToken))
                            client.DefaultRequestHeaders.Add("X-CSRFToken", param.CsrfToken);
                        client.DefaultRequestHeaders.Add("X-IG-App-ID", "936619743392459");
                        client.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
                        //client.DefaultRequestHeaders.Add("X-Web-Session-ID", "327srh:1s12oh:g48gav");
                        //client.DefaultRequestHeaders.Add("X-ASBD-ID", "359341");
                        if (!string.IsNullOrEmpty(param.X_IG_Claim))
                            client.DefaultRequestHeaders.Add("X-IG-WWW-Claim", param.X_IG_Claim);
                        client.DefaultRequestHeaders.Add("Accept", "*/*");
                        client.DefaultRequestHeaders.Add("Accept-Encoding", GramStatic.AcceptEncoding);
                        client.DefaultRequestHeaders.Referrer = new Uri($"https://www.instagram.com/{ProfileID}/following/");
                        // ✅ Make GET Request
                        var response = await client.GetAsync(url);
                        var responseBody = await response.Content.ReadAsStreamAsync();
                        var ResponseText = HttpHelper.Decode(responseBody, string.Join(", ", response.Content.Headers.ContentEncoding));
                        followingResponse = new FollowerAndFollowingIgResponseHandler(ResponseText);
                    }
                    return followingResponse;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public async Task<V2InboxResponse> Getv2Inbox(DominatorAccountModel _dominatorAccount, bool isPendingUsers = false,
            string cursorId = null)
        {
            try
            {
                var extra = new Dictionary<string, string>
                {
                    {"X-Pigeon-Session-Id","UFS-63d99619-f19c-4408-8c8d-3f21377b2ca0-0" }
                };
                SetRequestParameter(_dominatorAccount, ExtraParam: extra);
                var url = string.IsNullOrEmpty(cursorId) ?
                    $"https://i.instagram.com/api/v1/direct_v2/inbox/?visual_message_return_type=unseen&no_pending_badge=true&thread_message_limit=1&persistentBadging=true&limit=12&is_prefetching=false&fetch_reason=initial_snapshot"
                    : $"https://i.instagram.com/api/v1/direct_v2/inbox/?visual_message_return_type=unseen&cursor={cursorId}&folder=0&direction=older&seq_id=593783&no_pending_badge=true&thread_message_limit=1&persistentBadging=true&limit=12&is_prefetching=false&fetch_reason=page_scroll";
                var inboxResponse = await httpHelper.GetRequestAsync(url, _dominatorAccount.Token);
                return new V2InboxResponse(inboxResponse);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public CommonIgResponseHandler AcceptMessageRequest(string threadId)
        {
            try
            {
                var jsonElements = new JsonElements()
                {
                    Csrftoken = _Account.CsrfToken,
                    Uuid = _Account.Uuid
                };


                var requestParameter =
                    (IgRequestParameters)httpHelper.GetRequestParameter();
                requestParameter.PostDataParameters = new Dictionary<string, string>();
                requestParameter.Body = jsonElements;

                requestParameter.Url = $"direct_v2/threads/{threadId}/approve/";
                var url = requestParameter.GenerateUrl($"direct_v2/threads/{threadId}/approve/");
                url = Constants.ApiUrl + url;

                var postData = requestParameter.GenerateBody();

                return new CommonIgResponseHandler(httpHelper.PostRequest(url, postData));
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<LikeResponse> Like(DominatorAccountModel _dominatorAccount, AccountModel _Account,
            CancellationToken token, string mediaId, string username, string userId, QueryInfo queryInfo,
            bool isUnitTest = false)
        {
            token.ThrowIfCancellationRequested();
            try
            {
                #region OLD Post Like Code.
                //JsonElements jsonElements = null;
                //if (queryInfo.QueryType == "Hashtag Post(S)")
                //    jsonElements = new JsonElements()
                //    {
                //        Delivery_Class = "organic",
                //        MediaId = $"{mediaId}_{_dominatorAccount.AccountBaseModel.UserId}",
                //        RadioType = "wifi-none",
                //        Uid = _dominatorAccount.AccountBaseModel.UserId,
                //        Uuid = _Account.Uuid,
                //        IsCarouselBumpedPost = "false",
                //        ContainerModuleForLike = "feed_contextual_hashtag", //"feed_timeline"  //photo_view_profile    
                //        FeedPosition = isUnitTest ? "4" : Convert.ToString(new Random().Next(0, 9)),

                //    };
                //else if (queryInfo.QueryType == "Location Posts")
                //    jsonElements = new JsonElements()
                //    {
                //        Delivery_Class = "organic",
                //        MediaId = $"{mediaId}_{_dominatorAccount.AccountBaseModel.UserId}",
                //        RadioType = "wifi-none",
                //        Uid = _dominatorAccount.AccountBaseModel.UserId,
                //        Uuid = _Account.Uuid,
                //        EntityPageId =
                //            "103115376395004", //TO.... DO... Task //https://i.instagram.com/api/v1/locations/662544956/location_info/    
                //        IsCarouselBumpedPost = "false",
                //        ContainerModuleForLike =
                //            "feed_contextual_location", //"feed_timeline"  //photo_view_profile     
                //        FeedPosition = isUnitTest ? "4" : Convert.ToString(new Random().Next(0, 9)),
                //        EntityPageName =
                //            "Raipur", ////TO.... DO... Task //https://i.instagram.com/api/v1/locations/662544956/location_info/   
                //        //Username = username,                   
                //        // DeviceId = _Account.Device_Id,


                //        LocationId = queryInfo.QueryValue
                //    };
                //else
                //    jsonElements = new JsonElements()
                //    {
                //        InventorySource = "media_or_ad",
                //        Delivery_Class = "organic",
                //        TapSource = "button",
                //        MediaId = $"{mediaId}_{_dominatorAccount.AccountBaseModel.UserId}",
                //        RadioType = "wifi-none",
                //        Uid = _dominatorAccount.AccountBaseModel.UserId,
                //        Uuid = _Account.Uuid,
                //        IsCarouselBumpedPost = "false",
                //        ContainerModuleForLike = "feed_timeline", //"feed_contextual_profile",
                //        FeedPosition = isUnitTest ? "4" : Convert.ToString(new Random().Next(0, 9)),
                //        NavChain = $"MainFeedFragment:feed_timeline:1:cold_start:{GdUtilities.GetRowClientTime()}::",
                //        LoggingInfoToken = ""
                //    };

                ////&d = 0

                //var requestParameter =
                //    (IgRequestParameters)httpHelper.GetRequestParameter();
                //// requestParameter.Cookies = null;
                ////requestParameter.AddHeader("X-IG-VP9-Capable", "false");
                //requestParameter.Headers["X-IG-WWW-Claim"] = _Account.WwwClaim;
                //requestParameter.UrlParameters = new Dictionary<string, string>();
                //requestParameter.PostDataParameters = new Dictionary<string, string>();
                //requestParameter.Body = jsonElements;
                ////  requestParameter.AddHeader("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
                ////requestParameter.Url = $"media/{(object)mediaId}/like/?d=0";
                //var url = $"media/{(object)mediaId}_{_dominatorAccount.AccountBaseModel.UserId}/like/?d=0";
                //url = Constants.ApiUrl + url;
                //var postData = requestParameter.GenerateBody();
                //var likeResponse = new LikeResponse(httpHelper.PostRequest(url, postData));
                //return likeResponse;
                #endregion
                var Code = GramStatic.GetCodeFromIDOrUrl(mediaId);
                var param = GetWebParameter(_dominatorAccount);
                using (var client = new HttpClient(param.httpClient))
                {
                    var uri = new Uri($"https://www.instagram.com/api/v1/web/likes/{Code}/like/");
                    // Headers
                    client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
                    if (!string.IsNullOrEmpty(param.CsrfToken))
                        client.DefaultRequestHeaders.Add("X-CSRFToken", param.CsrfToken);
                    client.DefaultRequestHeaders.Add("X-Instagram-AJAX", GramStatic.InstagramAjax());
                    client.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
                    client.DefaultRequestHeaders.Add("X-IG-App-ID", "936619743392459");
                    if (!string.IsNullOrEmpty(_dominatorAccount?.DeviceDetails?.IGXClaim))
                        client.DefaultRequestHeaders.Add("X-IG-WWW-Claim", param.X_IG_Claim);
                    client.DefaultRequestHeaders.Referrer = new Uri($"https://www.instagram.com/p/{mediaId}/");
                    client.DefaultRequestHeaders.Add("Accept-Encoding", GramStatic.AcceptEncoding);
                    // Make the request
                    var response = await client.PostAsync(uri, new StringContent("", Encoding.UTF8, "application/x-www-form-urlencoded"));
                    var responseBody = await response.Content.ReadAsStreamAsync();
                    token.ThrowIfCancellationRequested();
                    var strResponse = HttpHelper.Decode(responseBody, string.Join(", ", response.Content.Headers.ContentEncoding));
                    var likeResponse = new LikeResponse(strResponse);
                    return likeResponse;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return default;
            }
        }

        public LoginIgResponseHandler Login()
        {
            return LoginAsyncOld().Result;
        }

        public async Task<LoginIgResponseHandler> LoginAsyncOld()
        {
            try
            {
                var jsonElements = new JsonElements()
                {
                    PhoneId = _dominatorAccount.DeviceDetails.PhoneId,
                    DeviceId = _dominatorAccount.DeviceDetails.DeviceId,
                    Guid = _dominatorAccount.DeviceDetails.Guid,
                    Adid = _dominatorAccount.DeviceDetails.AdId,
                    Username = _dominatorAccount.AccountBaseModel.UserName,
                    Password = _dominatorAccount.AccountBaseModel.Password,
                    LoginAttemptCount = "0",
                    Csrftoken = _Account.CsrfToken
                };


                var requestParameter = (IgRequestParameters)httpHelper.GetRequestParameter();
                requestParameter.PostDataParameters = new Dictionary<string, string>();
                requestParameter.Body = jsonElements;

                requestParameter.Url = "accounts/login/";
                var url = requestParameter.GenerateUrl("accounts/login/");
                url = Constants.ApiUrl + url;

                var postData = requestParameter.GenerateBody();

                return new LoginIgResponseHandler(await httpHelper.PostRequestAsync(url, postData, CancellationToken));
            }
            catch (WebException ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public async Task<CommonIgResponseHandler> SendSecurityCodeAsync(string challengeUrl, string challengeContext,
            string choiceNo, DominatorAccountModel _dominatorAccount, AccountModel _Account)
        {
            try
            {
                var jsonElements = new JsonElements()
                {
                    Choice = choiceNo,
                    //Csrftoken = _Account.CsrfToken,
                    Guid = _Account.Guid,
                    DeviceId = _Account.Device_Id,
                    Challenge_Context = challengeContext,
                    ShouldPromoteAccountStatus = "0"
                };
                //"challenge_context":"{\"step_name\": \"select_verify_method\", \"nonce_code\": \"RkO6Qywxjp\", \"user_id\": 35448295554, \"is_stateless\": false}"}
                //string sendSecurityPostData = $"choice={choiceNo}";
                var requestParameter =
                    (IgRequestParameters)httpHelper.GetRequestParameter();
                requestParameter.PostDataParameters = new Dictionary<string, string>();
                requestParameter.Headers["X-IG-App-ID"] = "567067343352427";
                requestParameter.AddHeader("Sec-Fetch-Site", "same-origin");
                requestParameter.AddHeader("Sec-Fetch-Mode", "core");
                requestParameter.AddHeader("Sec-Fetch-Dest", "empty");
                requestParameter.AddHeader("X-Requested-With", "XMLHttpRequest");
                requestParameter.AddHeader("X-IG-WWW-Claim", "0");
                requestParameter.Body = jsonElements;
                requestParameter.Url = challengeUrl;
                var url = requestParameter.GenerateUrl(challengeUrl);
                url = ConstantVariable.ApiUrl + url.Remove(0, 1);
                requestParameter.CreateSign();
                var postData = requestParameter.GenerateBody();
                //https://i.instagram.com/challenge/31700159223/FmXu6yRgEB/
                //byte[] postData = Encoding.ASCII.GetBytes(sendSecurityPostData);
                return new CommonIgResponseHandler(await httpHelper.PostRequestAsync(url, postData, CancellationToken));
            }
            catch (AssertFailedException)
            {
                throw;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }


        public async Task<CommonIgResponseHandler> ActionBlockSendSecurityCodeAsync(string challengeUrl, string challengeContext,
           string choiceNo, DominatorAccountModel _dominatorAccount, AccountModel _Account)
        {
            var userId = System.Text.RegularExpressions.Regex.Split(challengeUrl, "/")[2];
            var nonceCode = System.Text.RegularExpressions.Regex.Split(challengeUrl, "/")[3];
            try
            {
                var jsonElements = new JsonElements()
                {
                    //Csrftoken = _Account.CsrfToken,
                    UserId = userId,
                    Nonce_Code = nonceCode,
                    Challenge_Context = WebUtility.UrlEncode(challengeContext),
                    BlockVersioningId = Constants.BlockVersionningId,
                    ShouldPromoteAccountStatus = "0"
                };

                var requestParameter = (IgRequestParameters)httpHelper.GetRequestParameter();
                requestParameter.PostDataParameters = new Dictionary<string, string>();

                requestParameter.Headers["X-IG-WWW-Claim"] = "0";
                requestParameter.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
                requestParameter.Body = jsonElements;
                var url = requestParameter.GenerateUrl(challengeUrl);
                url = ConstantVariable.ApiUrl + "bloks/apps/com.instagram.challenge.navigation.take_challenge/";
                requestParameter.DontSign();
                var postData = requestParameter.GenerateBody();

                var check = new CommonIgResponseHandler(await httpHelper.PostRequestAsync(url, postData, CancellationToken));

                return ActionBlockSendSecurityCode(challengeUrl, challengeContext, choiceNo, nonceCode, _dominatorAccount, _Account);
            }
            catch (AssertFailedException)
            {
                throw;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public CommonIgResponseHandler ActionBlockSendSecurityCode(string challengeUrl, string challengeContext,
          string choiceNo, string nonce_Code, DominatorAccountModel _dominatorAccount, AccountModel _Account)
        {
            try
            {
                var jsonElements = new JsonElements()
                {
                    NestDataManifest = true,
                    Choice = choiceNo,
                    //Csrftoken = _Account.CsrfToken,
                    Challenge_Context = WebUtility.UrlEncode(challengeContext),
                    BlockVersioningId = Constants.BlockVersionningId
                };

                var requestParameter = (IgRequestParameters)httpHelper.GetRequestParameter();
                requestParameter.PostDataParameters = new Dictionary<string, string>();
                requestParameter.Body = jsonElements;
                var url = requestParameter.GenerateUrl(challengeUrl);
                url = ConstantVariable.ApiUrl + "bloks/apps/com.instagram.challenge.navigation.take_challenge/";
                requestParameter.DontSign();
                var postData = requestParameter.GenerateBody();
                return new CommonIgResponseHandler(httpHelper.PostRequest(url, postData));

            }
            catch (AssertFailedException)
            {
                throw;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }


        public async Task<CommonIgResponseHandler> SendSecurityCodeAsyncForSomeAccounts(string challengeUrl,
            string challengeContext, string choiceNo, DominatorAccountModel _dominatorAccount, AccountModel _Account)
        {
            try
            {
                _dominatorAccount.UserAgentMobileWeb =
                    $"Mozilla / 5.0(Linux; Android 10; RMX2027 Build/ QP1A.190711.020; wv) AppleWebKit / 537.36(KHTML, like Gecko) Version / 4.0 Chrome / 83.0.4103.106 Mobile Safari/ 537.36 {_dominatorAccount.UserAgentMobile}";
                var requestParameter = new IgRequestParameters(_dominatorAccount.UserAgentMobileWeb);
                //requestParameter.Cookies = _dominatorAccount.Cookies;            
                httpHelper.SetRequestParameter(requestParameter);
                var sendSecurityPostData = $"choice={choiceNo}";
                requestParameter.Headers["X-IG-App-ID"] = Constants.AppId;
                requestParameter.AddHeader("X-IG-WWW-Claim", "0");
                //requestParameter.Headers["X-CSRFToken"]= _Account.CsrfToken;              
                requestParameter.Url = challengeUrl;
                var url = requestParameter.GenerateUrl(challengeUrl);
                url = ConstantVariable.ApiUrl + url.Remove(0, 1);
                var postData = Encoding.ASCII.GetBytes(sendSecurityPostData);
                return new CommonIgResponseHandler(await httpHelper.PostRequestAsync(url, postData, CancellationToken));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public async Task<CommonIgResponseHandler> SendSecurityCodeAsyncTwoFactorLogin(
            DominatorAccountModel dominatorAccountModel, AccountModel accountModel, string challengeUrl,
            string choiceNo)
        {
            try
            {
                var jsonElements = new JsonElements()
                {
                    Choice = choiceNo,
                    Guid = accountModel.Guid,
                    DeviceId = dominatorAccountModel.DeviceDetails.DeviceId,
                    //Csrftoken = accountModel.CsrfToken,
                };
                var requestParameter =
                    (IgRequestParameters)httpHelper.GetRequestParameter();
                requestParameter.PostDataParameters = new Dictionary<string, string>();

                requestParameter.Body = jsonElements;
                requestParameter.Url = challengeUrl;
                var url = requestParameter.GenerateUrl(challengeUrl);
                url = ConstantVariable.ApiUrl + url.Remove(0, 1);
                requestParameter.CreateSign();
                var postData = requestParameter.GenerateBody();

                return new CommonIgResponseHandler(await httpHelper.PostRequestAsync(url, postData, CancellationToken));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public async Task<LoginIgResponseHandler> SubmitChallengeAsync(DominatorAccountModel _dominatorAccount,
            AccountModel _Account, string challenge_Context, string challengeUrl, string securityCode)
        {
            try
            {
                var jsonElements = new JsonElements()
                {
                    DeviceId = _Account.Device_Id,
                    SecurityCode = securityCode,
                    Guid = _Account.Guid,
                    Challenge_Context = challenge_Context,
                    ShouldPromoteAccountStatus = "0"
                };
                var requestParameter =
                    (IgRequestParameters)httpHelper.GetRequestParameter();
                requestParameter.PostDataParameters = new Dictionary<string, string>();

                requestParameter.Body = jsonElements;
                requestParameter.Url = challengeUrl;
                var url = requestParameter.GenerateUrl(challengeUrl);
                url = Constants.ApiUrl + url.Remove(0, 1);
                requestParameter.CreateSign();
                var postData = requestParameter.GenerateBody();

                return new LoginIgResponseHandler(await httpHelper.PostRequestAsync(url, postData, CancellationToken));
            }
            catch (AssertFailedException)
            {
                throw;
            }
            catch (Exception ex)
            {
                // Here error can only come for invalid Security Code because as per now request is working fine.
                ex.DebugLog();
                return new LoginIgResponseHandler(new ResponseParameter()
                { Response = "{\"action\": \"close\", \"status\": \"ok\"}", Exception = ex, HasError = true });
            }
        }

        public async Task<LoginIgResponseHandler> SubmitChallengeAsyncForDifferentAccounts(
            DominatorAccountModel _dominatorAccount, AccountModel _Account, string challenge_Context,
            string challengeUrl, string securityCode)
        {
            try
            {
                _dominatorAccount.UserAgentMobileWeb =
                    $"Mozilla / 5.0(Linux; Android 10; RMX2027 Build/ QP1A.190711.020; wv) AppleWebKit / 537.36(KHTML, like Gecko) Version / 4.0 Chrome / 83.0.4103.106 Mobile Safari/ 537.36 {_dominatorAccount.UserAgentMobile}";
                var requestParameter = new IgRequestParameters(_dominatorAccount.UserAgentMobileWeb);
                httpHelper.SetRequestParameter(requestParameter);
                var sendSecurityPostData = $"security_code={securityCode}";
                requestParameter.Headers["X-IG-App-ID"] = "1217981644879628";
                requestParameter.AddHeader("X-IG-WWW-Claim", "0");
                requestParameter.Url = challengeUrl;
                var url = requestParameter.GenerateUrl(challengeUrl);
                url = ConstantVariable.ApiUrl + url.Remove(0, 1);
                var postData = Encoding.ASCII.GetBytes(sendSecurityPostData);
                return new LoginIgResponseHandler(await httpHelper.PostRequestAsync(url, postData, CancellationToken));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }

            //try
            //{

            //    IgRequestParameters requestParameter =
            //        (IgRequestParameters)httpHelper.GetRequestParameter();
            //    requestParameter.PostDataParameters = new Dictionary<string, string>();

            //   // requestParameter.Body = jsonElements;
            //    requestParameter.Url = challengeUrl;
            //    string url = requestParameter.GenerateUrl(challengeUrl);
            //    url = Constants.ApiUrl + url.Remove(0, 1);
            //    requestParameter.CreateSign();
            //    byte[] postData = requestParameter.GenerateBody();

            //    return new LoginIgResponseHandler(await httpHelper.PostRequestAsync(url, postData, CancellationToken));
            //}
            //catch (Exception ex)
            //{
            //    // ex.DebugLog();
            //    // Here error can only come for invalid Security Code because as per now request is working fine.
            //    ex.DebugLog();
            //    // return null;
            //    return new LoginIgResponseHandler(new ResponseParameter() { Response = "{\"action\": \"close\", \"status\": \"ok\"}", Exception = ex, HasError = true });
            //}
        }

        public async Task<LoginIgResponseHandler> VerifyTwoFactorAccount(string challengeUrl, string securityCode)
        {
            try
            {
                // TODO:need to check invalid security code for exception
                var jsonElements = new JsonElements()
                {
                    DeviceId = _Account.Device_Id,
                    SecurityCode = securityCode,
                    //Csrftoken = _Account.CsrfToken,
                    Guid = _dominatorAccount.DeviceDetails.Guid,
                };

                var requestParameter =
                    (IgRequestParameters)httpHelper.GetRequestParameter();
                requestParameter.PostDataParameters = new Dictionary<string, string>();

                requestParameter.Body = jsonElements;
                challengeUrl = challengeUrl.Substring(1);
                requestParameter.Url = challengeUrl;
                var url = requestParameter.GenerateUrl(challengeUrl);
                url = Constants.ApiUrl + url;
                requestParameter.CreateSign();
                var postData = requestParameter.GenerateBody();

                return new LoginIgResponseHandler(await httpHelper.PostRequestAsync(url, postData, CancellationToken));
            }
            catch (Exception ex)
            {
                // Here error can only come for invalid Security Code because as per now request is working fine.
                ex.DebugLog();
                return null;
                //  return new LoginIgResponseHandler(new ResponseParameter() { Response = "Invalid Security code", Exception = ex, HasError = true });
            }
        }

        public async Task<LoginIgResponseHandler> ActionBlockSubmitChallengeAsync(DominatorAccountModel _dominatorAccount,
           AccountModel _Account, string challenge_Context, string challengeUrl, string securityCode)
        {
            try
            {
                var userId = System.Text.RegularExpressions.Regex.Split(challengeUrl, "/")[2];
                var jsonElements = new JsonElements()
                {
                    NestDataManifest = true,
                    ShouldPromoteAccountStatus = "0",
                    SecurityCode = securityCode,
                    //Csrftoken = _Account.CsrfToken,
                    PerfLogging_id = _Account.PerfLoggingId,
                    Challenge_Context = WebUtility.UrlEncode(challenge_Context),
                    BlockVersioningId = Constants.BlockVersionningId//Instagram|jacobypalmer8:nKagmYBfKvqS
                };

                var requestParameter = (IgRequestParameters)httpHelper.GetRequestParameter();
                requestParameter.PostDataParameters = new Dictionary<string, string>();
                requestParameter.Headers["X-IG-WWW-Claim"] = "0";
                requestParameter.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
                requestParameter.Headers.Remove("X-CSRFToken");
                requestParameter.Headers.Remove("X-FB-HTTP-Engine");
                requestParameter.Headers.Remove("X-FB-Client-IP");
                requestParameter.Headers.Remove("X-IG-App-Startup-Country");
                //requestParameter.AddHeader("X-IG-App-Startup-Country", _Account.CountryName);
                //requestParameter.Headers["X-IG-App-Startup-Country"] = _Account.CountryName;
                requestParameter.AddHeader("X-MID", _Account.MidHeader);
                if (string.IsNullOrEmpty(_Account.MidHeader))
                {
                    _Account.MidHeader = GetGdHttpHelper().Response.Headers["ig-set-x-mid"];
                    requestParameter.AddHeader("X-MID", _Account.MidHeader);
                }
                //httpHelper.GetRequestParameter().AddHeader("X-MID", httpHelper.Response.Headers["ig-set-x-mid"].Trim(','));
                requestParameter.Body = jsonElements;
                var url = "bloks/apps/com.instagram.challenge.navigation.take_challenge/";
                url = Constants.ApiUrl + url;
                requestParameter.DontSign();
                var postData = requestParameter.GenerateBody();

                return new LoginIgResponseHandler(await httpHelper.PostRequestAsync(url, postData, CancellationToken));
            }
            catch (AssertFailedException)
            {
                throw;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("layout") && ex.Message.Contains("profile_pic_url"))
                {
                    return new LoginIgResponseHandler(new ResponseParameter() { Response = ex.Message, HasError = false });
                }
                // Here error can only come for invalid Security Code because as per now request is working fine.
                ex.DebugLog();
                return new LoginIgResponseHandler(new ResponseParameter()
                { Response = "{\"action\": \"close\", \"status\": \"ok\"}", Exception = ex, HasError = true });
            }
        }


        public async Task<SearchTagIgResponseHandler> SearchForTag(DominatorAccountModel _dominatorAccount, AccountModel _Account,
            string keyword, string[] excludeTagList = null)
        {
            try
            {
                if (!keyword.StartsWith("#"))
                    keyword = $"#{keyword}";
                #region OLD Code for hastag search.
                //var requestParameter = (IgRequestParameters)httpHelper.GetRequestParameter();

                //requestParameter.Headers.Remove("Accept-Encoding");
                //requestParameter.UrlParameters = new Dictionary<string, string>();
                //requestParameter.AddUrlParameters("timezone_offset", _dateProvider.GetTimezoneOffset().ToString());
                //requestParameter.AddUrlParameters("q", keyword);
                //requestParameter.AddUrlParameters("count", "50");
                //requestParameter.AddUrlParameters("rank_token", _Account?.RankToken ?? ""); // TODO: account is null
                //if (excludeTagList != null && excludeTagList.Length != 0)
                //{
                //    if (excludeTagList.Length > 65)
                //        throw new InstagramException(
                //            "You are not allowed to provide more than 65 hashtags to exclude from the search.");
                //    requestParameter.AddUrlParameters("exclude_list",
                //        $"[{(object)string.Join(", ", excludeTagList)}]");
                //}

                //requestParameter.Url = "tags/search/";
                //var url = requestParameter.GenerateUrl("tags/search/");
                //url = Constants.ApiUrl + url;
                //return new SearchTagIgResponseHandler(httpHelper.GetRequest(url));
                #endregion
                var param = GetWebParameter(_dominatorAccount);
                using (var client = new HttpClient(param.httpClient))
                {
                    var random = new Random();
                    var value = Math.Round(random.NextDouble(), 15);
                    var url = $"https://www.instagram.com/api/v1/web/search/topsearch/?context=user&include_reel=true&query={Uri.EscapeDataString(keyword)}&rank_token={value}&search_session_id=&search_surface=web_top_search";

                    var request = new HttpRequestMessage(HttpMethod.Get, url);

                    // Add headers
                    request.Headers.Add("Host", "www.instagram.com");
                    request.Headers.Add("Connection", "keep-alive");
                    request.Headers.Add("sec-ch-ua-full-version-list", "\"Google Chrome\";v=\"137.0.7151.104\", \"Chromium\";v=\"137.0.7151.104\", \"Not/A)Brand\";v=\"24.0.0.0\"");
                    request.Headers.Add("sec-ch-ua-platform", "\"Windows\"");
                    request.Headers.Add("viewport-width", "1745");
                    request.Headers.Add("sec-ch-ua", "\"Google Chrome\";v=\"137\", \"Chromium\";v=\"137\", \"Not/A)Brand\";v=\"24\"");
                    request.Headers.Add("sec-ch-ua-mobile", "?0");
                    request.Headers.Add("X-IG-App-ID", "936619743392459");
                    request.Headers.Add("X-Requested-With", "XMLHttpRequest");
                    request.Headers.Add("Accept", "*/*");
                    request.Headers.Add("X-CSRFToken", param.CsrfToken);
                    //request.Headers.Add("X-Web-Session-ID", "zeu2dr:e3ngp4:fb6izj");
                    //request.Headers.Add("X-ASBD-ID", "359341");
                    request.Headers.Add("dpr", "1.1");
                    request.Headers.Add("sec-ch-prefers-color-scheme", "light");
                    request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/137.0.0.0 Safari/537.36");
                    request.Headers.Add("X-IG-WWW-Claim", param.X_IG_Claim);
                    request.Headers.Add("sec-ch-ua-platform-version", "\"10.0.0\"");
                    request.Headers.Add("Sec-Fetch-Site", "same-origin");
                    request.Headers.Add("Sec-Fetch-Mode", "cors");
                    request.Headers.Add("Sec-Fetch-Dest", "empty");
                    request.Headers.Add("Referer", "https://www.instagram.com");
                    request.Headers.Add("Accept-Encoding", GramStatic.AcceptEncoding);
                    request.Headers.Add("Accept-Language", "en-GB,en-US;q=0.9,en;q=0.8");
                    var response = await client.SendAsync(request);
                    var responseBody = await response.Content.ReadAsStreamAsync();
                    var ResponseText = HttpHelper.Decode(responseBody, string.Join(", ", response.Content.Headers.ContentEncoding));
                    return new SearchTagIgResponseHandler(ResponseText);
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public async Task<SearchKeywordIgResponseHandler> SearchForkeyword(DominatorAccountModel _dominatorAccount, string keyword,
            CancellationToken token)
        {
            try
            {
                #region old search code.
                //https://i.instagram.com/api/v1/fbsearch/account_serp/?search_surface=user_serp&timezone_offset=19800&count=30&query=virat
                //https://i.instagram.com/api/v1/fbsearch/account_serp/?search_surface=user_serp&timezone_offset=19800&count=30&query=virat&next_max_id=0bdeb7615efa4497ab516ef9817cdcd1&rank_token=1684930961727%7Cd54641d51001bac0f3050cc23231a59a78488786d586a255f2abe2dd0b8d6a3f&page_token=0bdeb7615efa4497ab516ef9817cdcd1&paging_token=%7B%22total_num_items%22%3A22%7D
                token.ThrowIfCancellationRequested();
                //var requestParameter =
                //    (IgRequestParameters)httpHelper.GetRequestParameter();

                //requestParameter.UrlParameters = new Dictionary<string, string>();
                //requestParameter.AddUrlParameters("search_surface", "user_serp");
                //requestParameter.AddUrlParameters("timezone_offset", _dateProvider.GetTimezoneOffset().ToString());
                //requestParameter.AddUrlParameters("count", "30");
                //requestParameter.AddUrlParameters("query", keyword);

                //requestParameter.Url = "fbsearch/account_serp/";
                //var url = requestParameter.GenerateUrl("fbsearch/account_serp/");
                //url = Constants.ApiUrl + url;
                //requestParameter.Headers.Remove("Accept-Encoding");

                //token.ThrowIfCancellationRequested();
                //var searchKeywordIgResponseHandler =
                //    new SearchKeywordIgResponseHandler(httpHelper.GetRequest(url));
                //if (searchKeywordIgResponseHandler == null || !searchKeywordIgResponseHandler.Success)

                //{
                //    delayService.ThreadSleep(TimeSpan.FromSeconds(7));
                //    searchKeywordIgResponseHandler = new SearchKeywordIgResponseHandler(httpHelper.GetRequest(url));
                //}
                //UpdateImportantHeadersOnEachRequest(requestParameter, _Account);
                //return searchKeywordIgResponseHandler;
                #endregion

                var param = GetWebParameter(_dominatorAccount);
                using (var client = new HttpClient(param.httpClient))
                {
                    var random = new Random();
                    var value = Math.Round(random.NextDouble(), 15);
                    var url = $"https://www.instagram.com/api/v1/web/search/topsearch/?context=user&include_reel=true&query={Uri.EscapeDataString(keyword)}&rank_token={value}&search_session_id=&search_surface=web_top_search";

                    var request = new HttpRequestMessage(HttpMethod.Get, url);

                    // Add headers
                    request.Headers.Add("Host", "www.instagram.com");
                    request.Headers.Add("Connection", "keep-alive");
                    request.Headers.Add("sec-ch-ua-full-version-list", "\"Google Chrome\";v=\"137.0.7151.104\", \"Chromium\";v=\"137.0.7151.104\", \"Not/A)Brand\";v=\"24.0.0.0\"");
                    request.Headers.Add("sec-ch-ua-platform", "\"Windows\"");
                    request.Headers.Add("viewport-width", "1745");
                    request.Headers.Add("sec-ch-ua", "\"Google Chrome\";v=\"137\", \"Chromium\";v=\"137\", \"Not/A)Brand\";v=\"24\"");
                    request.Headers.Add("sec-ch-ua-mobile", "?0");
                    request.Headers.Add("X-IG-App-ID", "936619743392459");
                    request.Headers.Add("X-Requested-With", "XMLHttpRequest");
                    request.Headers.Add("Accept", "*/*");
                    request.Headers.Add("X-CSRFToken",param.CsrfToken);
                    //request.Headers.Add("X-Web-Session-ID", "zeu2dr:e3ngp4:fb6izj");
                    //request.Headers.Add("X-ASBD-ID", "359341");
                    request.Headers.Add("dpr", "1.1");
                    request.Headers.Add("sec-ch-prefers-color-scheme", "light");
                    request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/137.0.0.0 Safari/537.36");
                    request.Headers.Add("X-IG-WWW-Claim",param.X_IG_Claim);
                    request.Headers.Add("sec-ch-ua-platform-version", "\"10.0.0\"");
                    request.Headers.Add("Sec-Fetch-Site", "same-origin");
                    request.Headers.Add("Sec-Fetch-Mode", "cors");
                    request.Headers.Add("Sec-Fetch-Dest", "empty");
                    request.Headers.Add("Referer", "https://www.instagram.com");
                    request.Headers.Add("Accept-Encoding", GramStatic.AcceptEncoding);
                    request.Headers.Add("Accept-Language", "en-GB,en-US;q=0.9,en;q=0.8");
                    var response = await client.SendAsync(request);
                    var responseBody = await response.Content.ReadAsStreamAsync();
                    var ResponseText = HttpHelper.Decode(responseBody, string.Join(", ", response.Content.Headers.ContentEncoding));
                    return new SearchKeywordIgResponseHandler(ResponseText,IsBrowser:false);
                }
            }
            catch (AssertFailedException)
            {
                throw;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public UsernameInfoIgResponseHandler SearchUsername(DominatorAccountModel DominatorAccountModel,
            string username, CancellationToken token, bool isOtherUser = false)
        {
            return SearchUsernameAsync(DominatorAccountModel, username, token, isOtherUser).Result;
        }

        public async Task<UsernameInfoIgResponseHandler> SearchUsernameAsync(DominatorAccountModel _dominatorAccount,
            string username, CancellationToken token, bool isOtherUser = false)
        {
            if (!isOtherUser && !string.IsNullOrEmpty(_Account.DsUserId))
            {
                username = _Account.DsUserId;
            }
            if (string.IsNullOrEmpty(username))
            {
                return null;
            }
            try
            {
                #region Old Code to get userProfile.
                //do
                //{
                //    counter++;
                //    if (flag)
                //        flag = false;
                //    else
                //        Thread.Sleep(Constants.FloodWait);
                //    var url = "";

                //    delayService.ThreadSleep(TimeSpan.FromSeconds(5));
                //    var id = (isOtherUser) ? $"users/{(object)username}/usernameinfo/" : $"users/{(object)username}/info/";
                //    url = Constants.ApiUrl + id;

                //    usernameInfoResponse = new UsernameInfoIgResponseHandler(await httpHelper.GetRequestAsync(url, CancellationToken));

                //} while (!usernameInfoResponse.Success && usernameInfoResponse.Issue.Error == InstagramError.RateLimit && counter < 5);
                #endregion
                var url = $"https://www.instagram.com/api/v1/users/web_profile_info/?username={username}";
                var param = GetWebParameter(_dominatorAccount);
                using (var client = new HttpClient(param.httpClient))
                {
                    // Add headers
                    client.DefaultRequestHeaders.Add("X-CSRFToken",param.CsrfToken);
                    //client.DefaultRequestHeaders.Add("X-Web-Session-ID", "obiknh:fd8n3s:nzanw0");
                    client.DefaultRequestHeaders.Add("X-IG-App-ID", "936619743392459");
                    //client.DefaultRequestHeaders.Add("X-ASBD-ID", "359341");
                    client.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
                    client.DefaultRequestHeaders.Add("X-IG-WWW-Claim",param.X_IG_Claim);
                    client.DefaultRequestHeaders.Add("sec-ch-ua", "\"Google Chrome\";v=\"137\", \"Chromium\";v=\"137\", \"Not/A)Brand\";v=\"24\"");
                    client.DefaultRequestHeaders.Add("sec-ch-ua-platform", "\"Windows\"");
                    client.DefaultRequestHeaders.Add("sec-ch-ua-platform-version", "\"10.0.0\"");
                    client.DefaultRequestHeaders.Add("sec-ch-ua-full-version-list", "\"Google Chrome\";v=\"137.0.7151.104\", \"Chromium\";v=\"137.0.7151.104\", \"Not/A)Brand\";v=\"24.0.0.0\"");
                    client.DefaultRequestHeaders.Add("sec-ch-ua-mobile", "?0");
                    client.DefaultRequestHeaders.Add("sec-ch-prefers-color-scheme", "light");
                    client.DefaultRequestHeaders.Add("Sec-Fetch-Site", "same-origin");
                    client.DefaultRequestHeaders.Add("Sec-Fetch-Mode", "cors");
                    client.DefaultRequestHeaders.Add("Sec-Fetch-Dest", "empty");
                    client.DefaultRequestHeaders.Add("Accept-Encoding", GramStatic.AcceptEncoding);
                    client.DefaultRequestHeaders.Referrer = new Uri($"https://www.instagram.com/{username}/");
                    client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/137.0.0.0 Safari/537.36");
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
                    var response = await client.GetAsync(url);
                    var content = await response.Content.ReadAsStreamAsync();
                    var ResponseString = HttpHelper.Decode(content, string.Join(", ", response.Content.Headers.ContentEncoding));
                    return new UsernameInfoIgResponseHandler(ResponseString);
                }
            }
            catch (Exception)
            {
                //ex.DebugLog();
                return null;
            }
        }

        public async Task<UsernameInfoIgResponseHandler> SearchUserInfoById(DominatorAccountModel _dominatorAccount,
            AccountModel accountModel, string usernameId, CancellationToken token, string queryType = null)
        {
            try
            {
                #region OLD Code to get userinfo by id.
                //UsernameInfoIgResponseHandler usernameInfoIgResponseHandler;
                //var requestParameters = (IgRequestParameters)httpHelper.GetRequestParameter();
                //if (accountModel != null && !string.IsNullOrEmpty(accountModel.AuthorizationHeader))
                //    requestParameters.Headers["Authorization"] = accountModel.AuthorizationHeader;
                //if (accountModel != null && !string.IsNullOrEmpty(accountModel.WwwClaim))
                //    requestParameters.Headers["X-IG-WWW-Claim"] = accountModel.WwwClaim;
                //var url = requestParameters.GenerateUrl($"users/{(object)usernameId}/info/");
                //url = Constants.ApiUrl + url;
                ////https://i.instagram.com/api/v1/users/6975636384/info/ 

                //usernameInfoIgResponseHandler = new UsernameInfoIgResponseHandler(httpHelper.GetRequest(url));

                //if (!usernameInfoIgResponseHandler.Success || usernameInfoIgResponseHandler
                //    == null || usernameInfoIgResponseHandler.Username == null)
                //{
                //    delayService.ThreadSleep(TimeSpan.FromSeconds(5));
                //    //Thread.Sleep(TimeSpan.FromSeconds(2));// Thread.Sleep(2 * 1000);
                //    usernameInfoIgResponseHandler = new UsernameInfoIgResponseHandler(httpHelper.GetRequest(url));
                //}
                //UpdateImportantHeadersOnEachRequest(requestParameters, accountModel);
                //return usernameInfoIgResponseHandler;
                #endregion
                var param = GetWebParameter(_dominatorAccount);
                using (var client = new HttpClient(param.httpClient))
                    {
                        client.BaseAddress = new Uri("https://i.instagram.com");
                        if (!string.IsNullOrEmpty(param.CsrfToken))
                            client.DefaultRequestHeaders.Add("x-csrftoken", param.CsrfToken);
                        client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/136.0.0.0 Safari/537.36");
                        //client.DefaultRequestHeaders.Add("content-type", "application/x-www-form-urlencoded");
                        client.DefaultRequestHeaders.Add("x-ig-app-id", "936619743392459");
                        if (!string.IsNullOrEmpty(_dominatorAccount?.DeviceDetails?.IGXClaim))
                            client.DefaultRequestHeaders.Add("x-ig-www-claim", _dominatorAccount.DeviceDetails.IGXClaim);
                        client.DefaultRequestHeaders.Add("Accept", "*/*");
                        client.DefaultRequestHeaders.Add("Accept-Encoding", GramStatic.AcceptEncoding);
                        client.DefaultRequestHeaders.Add("Accept-Language", "en-GB,en-US;q=0.9,en;q=0.8");
                        var endpoint = $"/api/v1/feed/user/{usernameId}/username/?count=12";
                        var response = await client.GetAsync(endpoint);
                        var responseString = await HttpHelper.Decode(response);
                        return new UsernameInfoIgResponseHandler(responseString);
                    }

            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }


        public UsernameInfoIgResponseHandler Get_AccountUserid(DominatorAccountModel _dominatorAccount,
           AccountModel accountModel, string usernameId, CancellationToken token, string queryType = null)
        {
            try
            {
                UsernameInfoIgResponseHandler usernameInfoIgResponseHandler;
                var requestParameters = (IgRequestParameters)httpHelper.GetRequestParameter();
                if (accountModel != null && !string.IsNullOrEmpty(accountModel.AuthorizationHeader))
                    requestParameters.Headers["Authorization"] = accountModel.AuthorizationHeader;
                if (accountModel != null && !string.IsNullOrEmpty(accountModel.WwwClaim))
                    requestParameters.Headers["X-IG-WWW-Claim"] = accountModel.WwwClaim;
                var url = requestParameters.GenerateUrl($"multiple_accounts/get_account_family/");
                url = Constants.ApiUrl + url;
                //https://i.instagram.com/api/v1/users/6975636384/info/ 

                usernameInfoIgResponseHandler = new UsernameInfoIgResponseHandler(httpHelper.GetRequest(url));

                if (usernameInfoIgResponseHandler == null)
                {
                    delayService.ThreadSleep(TimeSpan.FromSeconds(5));
                    //Thread.Sleep(TimeSpan.FromSeconds(2));// Thread.Sleep(2 * 1000);
                    usernameInfoIgResponseHandler = new UsernameInfoIgResponseHandler(httpHelper.GetRequest(url));
                }

                return usernameInfoIgResponseHandler;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }
        //{"verification_code":"602076",
        //"phone_id":"38e42cea-a11c-4838-8e67-bcc936035899",
        //"_csrftoken":"nr1oxTNHdKbWcJuPiynxpSrTajMwfSNz",
        //"two_factor_identifier":"0V6dx7OgU4",
        //"username":"sachinsaw_india",
        //"trust_this_device":"1",
        //"guid":"2f8a2b55-8663-403b-8f71-7e3b4f9d7bcb",
        //"device_id":"android-af44bd3f9305bc84",
        //"waterfall_id":"61e9a9fb-3e70-4da0-8bfe-50f3dbc46f22",
        //"verification_method":"1"}
        public LoginIgResponseHandler TwoFactorLogin(DominatorAccountModel dominatorAccountModel,
            AccountModel accountModel, string twoFactorIdentifier)
        {
            try
            {
                var jsonElements = new JsonElements()
                {
                    VerificationCode = dominatorAccountModel.VarificationCode,
                    PhoneId = accountModel.PhoneId,
                    Csrftoken = accountModel.CsrfToken,
                    TwoFactorIdentifier = twoFactorIdentifier,
                    Username = dominatorAccountModel.AccountBaseModel.UserName,
                    Guid = accountModel.Guid,
                    DeviceId = accountModel.Device_Id,
                    WaterfallId = accountModel.Uuid,
                    VerificationMethod = "1",
                    TrustThisDevice = "1"
                };
                var requestParameter =
                    (IgRequestParameters)httpHelper.GetRequestParameter();
                requestParameter.Body = jsonElements;
                requestParameter.Url = "accounts/two_factor_login/";
                var url = requestParameter.GenerateUrl("accounts/two_factor_login/");
                url = Constants.ApiUrl + url;
                var postData = requestParameter.GenerateBody();

                return new LoginIgResponseHandler(httpHelper.PostRequest(url, postData));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public ResendTwoFactorLoginCodeResponseHandler SendAgainTwoFactorLoginCode(string twoFactorIdentifier)
        {
            try
            {
                var jsonElements = new JsonElements()
                {
                    Csrftoken = _Account.CsrfToken,
                    TwoFactorIdentifier = twoFactorIdentifier,
                    Username = _dominatorAccount.AccountBaseModel.UserName,
                    Guid = _dominatorAccount.DeviceDetails.Guid,
                    DeviceId = _Account.Device_Id,
                };
                var requestParameter =
                    (IgRequestParameters)httpHelper.GetRequestParameter();
                requestParameter.Body = jsonElements;
                requestParameter.Url = "accounts/send_two_factor_login_sms/";
                var url = requestParameter.GenerateUrl("accounts/send_two_factor_login_sms/");
                url = Constants.ApiUrl + url;
                var postData = requestParameter.GenerateBody();

                return new ResendTwoFactorLoginCodeResponseHandler(httpHelper.PostRequest(url, postData));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }


        public async Task<FriendshipsResponse> Unfollow(DominatorAccountModel _dominatorAccount, AccountModel _Account,
            CancellationToken token, string userId)
        {
            token.ThrowIfCancellationRequested();
            try
            {
                #region OLD Unfollow Code.
                //var jsonElements = new JsonElements()
                //{
                //    Surface = "profile",
                //    //Csrftoken = _Account.CsrfToken,
                //    UserId = userId,
                //    RadioType = "wifi-none",
                //    Uid = _dominatorAccount.AccountBaseModel.UserId,
                //    Uuid = _Account.Uuid,
                //};
                //var requestParameter =
                //    (IgRequestParameters)httpHelper.GetRequestParameter();
                //requestParameter.UrlParameters = new Dictionary<string, string>();
                //requestParameter.Headers["X-IG-WWW-Claim"] = _Account.WwwClaim;
                //requestParameter.Body = jsonElements;
                //requestParameter.Url = $"friendships/destroy/{(object)userId}/";
                //var url = requestParameter.GenerateUrl($"friendships/destroy/{(object)userId}/");
                //url = Constants.ApiUrl + url;
                //var postData = requestParameter.GenerateBody();
                //token.ThrowIfCancellationRequested();
                //var friendshipResponse = new FriendshipsResponse(httpHelper.PostRequest(url, postData));
                //UpdateImportantHeadersOnEachRequest(requestParameter, _Account);
                #endregion
                var param = GetWebParameter(_dominatorAccount);
                var url = $"https://www.instagram.com/api/v1/friendships/destroy/{userId}/";
                using (var client = new HttpClient(param.httpClient))
                {
                    // Add Instagram domain cookies
                    var instagramUri = new Uri("https://www.instagram.com");
                    // Prepare POST data
                    var postData = new StringContent(
                        $"container_module=profile&nav_chain=PolarisNavChain+PolarisNavChain+PolarisProfilePostsTabRoot%3AprofilePage%3A2%3Atopnav-link%2CPolarisNavChain+PolarisNavChain+PolarisProfilePostsTabRoot%3AprofilePage%3A3%3Aunexpected&user_id={userId}&jazoest={param.Jazoest}",
                        Encoding.UTF8,
                        "application/x-www-form-urlencoded"
                    );

                    // Set headers
                    client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/137.0.0.0 Safari/537.36");
                    client.DefaultRequestHeaders.Add("X-IG-App-ID", "936619743392459");
                    client.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
                    client.DefaultRequestHeaders.Add("X-Instagram-AJAX", GramStatic.InstagramAjax());
                    client.DefaultRequestHeaders.Add("X-CSRFToken", param.CsrfToken);//"6vlVHfr30gwgE5AZ5BPLetA8ataWsV7Z"
                    //client.DefaultRequestHeaders.Add("X-Web-Session-ID", "wmkzwe:ugopeh:f20mea");
                    //client.DefaultRequestHeaders.Add("X-ASBD-ID", "359341");
                    client.DefaultRequestHeaders.Add("X-IG-WWW-Claim",param.X_IG_Claim);
                    client.DefaultRequestHeaders.Add("Accept-Encoding", GramStatic.AcceptEncoding);
                    client.DefaultRequestHeaders.Referrer = new Uri($"https://www.instagram.com/{_dominatorAccount.AccountBaseModel.ProfileId}/following/");

                    // Send request
                    var response = await client.PostAsync(url, postData);
                    var responseStream = await response.Content.ReadAsStreamAsync();
                    var responseString = HttpHelper.Decode(responseStream, string.Join(", ", response.Content.Headers.ContentEncoding));
                    return new FriendshipsResponse(responseString);
                }
            }
            catch (AssertFailedException)
            {
                throw;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public UserFriendshipResponse UserFriendship(DominatorAccountModel _dominatorAccount, AccountModel _Account,
            string userId)
        {
            try
            {
                UserFriendshipResponse userFriendshipResponse;
                var url = $"friendships/show/{userId}/";
                url = Constants.ApiUrl + url;
                userFriendshipResponse = new UserFriendshipResponse(httpHelper.GetRequest(url));
                return userFriendshipResponse;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public async Task<FriendshipsResponse> Block(DominatorAccountModel _dominatorAccount, AccountModel _Account,
            CancellationToken token, string userId)
        {
            token.ThrowIfCancellationRequested();
            
            try
            {
                #region OLD Code to block user.
                //var jsonElements = new JsonElements()
                //{
                //    Uuid = _Account.Uuid,
                //    Uid = _dominatorAccount.AccountBaseModel.UserId,
                //    UserId = userId,
                //};

                //var requestParameters =
                //    (IgRequestParameters)httpHelper.GetRequestParameter();
                //requestParameters.Body = jsonElements;
                //requestParameters.Headers["X-IG-WWW-Claim"] = _Account.WwwClaim;
                //requestParameters.Url = $"friendships/block/{userId}/";
                //var url = requestParameters.GenerateUrl($"friendships/block/{userId}/");
                //url = Constants.ApiUrl + url;

                //var postData = requestParameters.GenerateBody();
                //token.ThrowIfCancellationRequested();
                //return new FriendshipsResponse(httpHelper.PostRequest(url, postData));
                #endregion

                #region Mobile Block Request.
                //var param = GetWebParameter(_dominatorAccount, true);
                //var httpClient = new HttpClient();
                //var request = new HttpRequestMessage(HttpMethod.Post, $"https://i.instagram.com/api/v1/friendships/block/{userId}/");
                //var IgDeviceID = "cba40baf-8663-4ee8-a0bb-74dc5d2e0a98";
                //// Headers
                //request.Headers.Add("User-Agent", "Instagram 283.0.0.20.105 Android (31/12; 320dpi; 720x1470; vivo; V2029; 2027; qcom; en_US; 475221264)");
                //request.Headers.Add("X-IG-App-Locale", "en_US");
                //request.Headers.Add("X-IG-Device-Locale", "en_US");
                //request.Headers.Add("X-IG-Mapped-Locale", "en_US");
                //request.Headers.Add("X-Pigeon-Session-Id", "UFS-" + (_dominatorAccount?.DeviceDetails?.Id ?? "75e76e6b-2b3c-4609-aade-8fc1be12f136") + "0");
                //request.Headers.Add("X-Pigeon-Rawclienttime", GdUtilities.GetRowClientTime());
                ////request.Headers.Add("X-IG-Bandwidth-Speed-KBPS", "407.000");
                ////request.Headers.Add("X-IG-Bandwidth-TotalBytes-B", "4810035");
                ////request.Headers.Add("X-IG-Bandwidth-TotalTime-MS", "11038");
                //request.Headers.Add("X-Bloks-Version-Id", "f5fbf62cc3c51dc0e6f4ffd3a79e0c5929ae0b8af58c54acd1e186871a92fb27");
                //request.Headers.Add("X-IG-WWW-Claim",param?.X_IG_Claim);
                //request.Headers.Add("X-Bloks-Is-Layout-RTL", "false");
                //request.Headers.Add("X-IG-Device-ID", IgDeviceID);
                //request.Headers.Add("X-IG-Family-Device-ID", _dominatorAccount?.DeviceDetails?.FamilyId ?? "6a1184d2-87c2-454f-93c5-a491e0040ad7");
                //request.Headers.Add("X-IG-Android-ID", "android-b10069c5ba7bbd58");
                //request.Headers.Add("X-IG-Timezone-Offset", "19800");
                ////request.Headers.Add("X-IG-Nav-Chain", "MainFeedFragment:feed_timeline:1:cold_start:1750501412.506:10#230#301:3644732301466486575,UserDetailFragment:profile:3:media_owner:1750501491.747::,ProfileMediaTabFragment:profile:4:button:1750501493.205::,MultiBlockBottomSheetFragment:multi_block_bottom_sheet:5:button:1750501501.693::");
                ////request.Headers.Add("X-IG-CLIENT-ENDPOINT", "MultiBlockBottomSheetFragment:multi_block_bottom_sheet");
                //request.Headers.Add("X-FB-Connection-Type", "WIFI");
                //request.Headers.Add("X-IG-Connection-Type", "WIFI");
                //request.Headers.Add("X-IG-Capabilities", "3brTv10=");
                //request.Headers.Add("X-IG-App-ID", "567067343352427");
                //request.Headers.Add("Accept-Language", "en-US");
                //request.Headers.Add("Authorization", param.Authorization);
                //request.Headers.Add("X-MID", param.MID);
                //request.Headers.Add("IG-U-DS-USER-ID", param.DsUserId);
                //request.Headers.Add("Accept-Encoding", GramStatic.AcceptEncoding);
                ////request.Headers.Add("IG-U-RUR", "CCO,73228986484,1782037493:01fe5e68a7cf740e947e2bd25ee4e15dca324494ae941cff3ad425b4fea47a5ff8ba7eef");
                //request.Headers.Add("IG-INTENDED-USER-ID", param.DsUserId);
                //var postBody = $"signed_body=SIGNATURE.{Uri.EscapeDataString($"{{\"surface\":\"profile\",\"is_auto_block_enabled\":\"true\",\"user_id\":\"{userId}\",\"_uid\":\"{param.DsUserId}\",\"_uuid\":\"{IgDeviceID}\"}}")}";
                //// Body
                //var content = new StringContent(postBody, Encoding.UTF8, "application/x-www-form-urlencoded");
                //request.Content = content;

                //// Send the request
                //var response = await httpClient.SendAsync(request);
                //var responseText = await HttpHelper.Decode(response);
                //return new FriendshipsResponse(responseText);
                #endregion
                var param = GetWebParameter(_dominatorAccount);
                using (HttpClient client = new HttpClient(param.httpClient))
                {
                    // URL
                    var url = "https://www.instagram.com/graphql/query";

                    // Headers
                    client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/137.0.0.0 Safari/537.36");
                    client.DefaultRequestHeaders.Add("Accept", "*/*");
                    client.DefaultRequestHeaders.Add("Origin", "https://www.instagram.com");
                    client.DefaultRequestHeaders.Add("Referer", "https://www.instagram.com/instagram/");
                    client.DefaultRequestHeaders.Add("X-IG-App-ID", "936619743392459");
                    client.DefaultRequestHeaders.Add("X-CSRFToken", param.CsrfToken);
                    client.DefaultRequestHeaders.Add("X-FB-Friendly-Name", "usePolarisBlockManyMutation");
                    client.DefaultRequestHeaders.Add("X-Root-Field-Name", "xdt_block_many");
                    client.DefaultRequestHeaders.Add("X-BLOKS-VERSION-ID", "baaa014f499a77b984e16c1d6bfbc1de5c78f62c3202481ef57729a2e3383641");
                    client.DefaultRequestHeaders.Add("sec-ch-ua", "\"Google Chrome\";v=\"137\", \"Chromium\";v=\"137\", \"Not/A)Brand\";v=\"24\"");
                    client.DefaultRequestHeaders.Add("sec-ch-ua-platform", "\"Windows\"");
                    client.DefaultRequestHeaders.Add("sec-ch-ua-platform-version", "\"10.0.0\"");
                    client.DefaultRequestHeaders.Add("sec-ch-ua-full-version-list", "\"Google Chrome\";v=\"137.0.7151.120\", \"Chromium\";v=\"137.0.7151.120\", \"Not/A)Brand\";v=\"24.0.0.0\"");
                    client.DefaultRequestHeaders.Add("sec-ch-ua-mobile", "?0");
                    client.DefaultRequestHeaders.Add("sec-ch-prefers-color-scheme", "light");
                    client.DefaultRequestHeaders.Add("Sec-Fetch-Site", "same-origin");
                    client.DefaultRequestHeaders.Add("Sec-Fetch-Mode", "cors");
                    client.DefaultRequestHeaders.Add("Sec-Fetch-Dest", "empty");
                    // Content (form-urlencoded)
                    var content = new StringContent(
                        "__crn=comet.igweb.PolarisProfilePostsTabRoute" +
                        "&fb_api_caller_class=RelayModern" +
                        "&fb_api_req_friendly_name=usePolarisBlockManyMutation" +
                        $"&variables=%7B%22target_user_ids%22%3A%5B%22{userId}%22%5D%7D" +
                        "&server_timestamps=true" +
                        "&doc_id=9575321849242740" + 
                        $"&jazoest={GramStatic.CreateJazoest(_dominatorAccount?.DeviceDetails?.PhoneId)}"
                        + "&__comet_req=7"
                        + "&__ccg=EXCELLENT"
                        + "&__user=0"
                        + "&__hs=20269.HYP:instagram_web_pkg.2.1...0",
                        Encoding.UTF8,
                        "application/x-www-form-urlencoded"
                    );

                    // Send request
                    var response = await client.PostAsync(url, content);
                    var responseText = await HttpHelper.Decode(response);
                    return new FriendshipsResponse(responseText);
                }

            }
            catch (AssertFailedException)
            {
                throw;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public async Task<FriendshipsResponse> UnBlock(DominatorAccountModel _dominatorAccount, AccountModel _Account,
            CancellationToken token, string userId)
        {
            token.ThrowIfCancellationRequested();
            try
            {
                #region UnBlock User OLD Code base.
                //var jsonElements = new JsonElements()
                //{
                //    Uuid = _Account.Uuid,
                //    Uid = _dominatorAccount.AccountBaseModel.UserId,
                //    UserId = userId,
                //};

                //var requestParameters =
                //    (IgRequestParameters)httpHelper.GetRequestParameter();
                //requestParameters.Body = jsonElements;
                //requestParameters.Headers["X-IG-WWW-Claim"] = _Account.WwwClaim;
                //requestParameters.Url = $"friendships/unblock/{userId}/";
                //var url = requestParameters.GenerateUrl($"friendships/unblock/{userId}/");
                //url = Constants.ApiUrl + url;

                //var postData = requestParameters.GenerateBody();
                //token.ThrowIfCancellationRequested();
                //return new FriendshipsResponse(httpHelper.PostRequest(url, postData));
                #endregion

                var param = GetWebParameter(_dominatorAccount, true);
                using (HttpClient client = new HttpClient())
                {
                    var request = new HttpRequestMessage(HttpMethod.Post, $"https://i.instagram.com/api/v1/friendships/unblock/{userId}/");
                    var IgDeviceID = _dominatorAccount?.DeviceDetails?.PhoneId ?? "cba40baf-8663-4ee8-a0bb-74dc5d2e0a98";
                    // Add headers
                    request.Headers.Add("User-Agent",_dominatorAccount?.DeviceDetails?.Useragent ?? "Instagram 283.0.0.20.105 Android (31/12; 320dpi; 720x1470; vivo; V2029; 2027; qcom; en_US; 475221264)");
                    request.Headers.Add("X-IG-App-Locale", "en_US");
                    request.Headers.Add("X-IG-Device-Locale", "en_US");
                    request.Headers.Add("X-IG-Mapped-Locale", "en_US");
                    request.Headers.Add("X-Pigeon-Session-Id", ("UFS -" + _dominatorAccount?.DeviceDetails?.Id ?? "75e76e6b-2b3c-4609-aade-8fc1be12f136" + "1"));
                    request.Headers.Add("X-Pigeon-Rawclienttime",GdUtilities.GetRowClientTime());
                    //request.Headers.Add("X-IG-Bandwidth-Speed-KBPS", "479.000");
                    //request.Headers.Add("X-IG-Bandwidth-TotalBytes-B", "13657219");
                    //request.Headers.Add("X-IG-Bandwidth-TotalTime-MS", "25425");
                    request.Headers.Add("X-Bloks-Version-Id", "f5fbf62cc3c51dc0e6f4ffd3a79e0c5929ae0b8af58c54acd1e186871a92fb27");
                    request.Headers.Add("X-IG-WWW-Claim",param.X_IG_Claim);
                    request.Headers.Add("X-Bloks-Is-Layout-RTL", "false");
                    request.Headers.Add("X-IG-Device-ID", IgDeviceID);
                    request.Headers.Add("X-IG-Family-Device-ID", _dominatorAccount?.DeviceDetails?.FamilyId ?? "6a1184d2-87c2-454f-93c5-a491e0040ad7");
                    request.Headers.Add("X-IG-Android-ID", _dominatorAccount?.DeviceDetails?.DeviceId ?? "android-b10069c5ba7bbd58");
                    request.Headers.Add("X-IG-Timezone-Offset", "19800");
                    //request.Headers.Add("X-IG-Nav-Chain", "ExploreFragment:explore_popular:2:main_search:1750514302.874::,SingleSearchTypeaheadTabFragment:search_typeahead:3:button:1750514303.998::,UserDetailFragment:profile:4:search_result:1750514312.997::");
                    //request.Headers.Add("X-IG-CLIENT-ENDPOINT", "UserDetailFragment:profile");
                    request.Headers.Add("X-FB-Connection-Type", "WIFI");
                    request.Headers.Add("X-IG-Connection-Type", "WIFI");
                    request.Headers.Add("X-IG-Capabilities", "3brTv10=");
                    request.Headers.Add("X-IG-App-ID", "567067343352427");
                    request.Headers.Add("Authorization", param.Authorization);
                    request.Headers.Add("X-MID", param.MID);
                    request.Headers.Add("IG-U-DS-USER-ID",param.DsUserId);
                    //request.Headers.Add("IG-U-RUR", "CCO,73228986484,1782050320:01fe89fff87db52ff990d96159854385f1d50b7fdae39bd5c1b06eab44d231f6b1aef6c5");
                    request.Headers.Add("IG-INTENDED-USER-ID",param.DsUserId);
                    request.Headers.AcceptLanguage.Add(new StringWithQualityHeaderValue("en-US"));
                    request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
                    var PostData = $"signed_body=SIGNATURE.{Uri.EscapeDataString($"{{\"user_id\":\"{userId}\",\"_uid\":\"{param.DsUserId}\",\"_uuid\":\"{IgDeviceID}\",\"surface\":\"profile\"}}")}";
                    // Content (empty form for unblock request)
                    request.Content = new StringContent(PostData, Encoding.UTF8, "application/x-www-form-urlencoded");
                    // Send the request
                    HttpResponseMessage response = await client.SendAsync(request);
                    var ResponseData = await HttpHelper.Decode(response);
                    return new FriendshipsResponse(ResponseData);
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public async Task<DeleteMediaIgResponseHandler> DeleteMedia(DominatorAccountModel _dominatorAccount, AccountModel _Account,
            CancellationToken token, string mediaId)
        {
            token.ThrowIfCancellationRequested();
            try
            {
                var PK = GramStatic.GetCodeFromIDOrUrl(mediaId);
                #region Delete Media OLD Code.
                //var jsonElements = new JsonElements()
                //{
                //    Uuid = _Account.Uuid,
                //    Uid = _dominatorAccount.AccountBaseModel.UserId,
                //    //Csrftoken = _Account.CsrfToken,
                //    MediaId = mediaId
                //};

                //var requestParameter =
                //    (IgRequestParameters)httpHelper.GetRequestParameter();
                //requestParameter.Body = jsonElements;

                //requestParameter.Url = $"media/{mediaId}/delete/";
                //var url = Constants.ApiUrl + requestParameter.GenerateUrl();

                //var postData = requestParameter.GenerateBody();
                //token.ThrowIfCancellationRequested();
                //return new DeleteMediaIgResponseHandler(httpHelper.PostRequest(url, postData));
                #endregion

                var param = GetWebParameter(_dominatorAccount);
                using (var client = new HttpClient(param.httpClient))
                {
                    var request = new HttpRequestMessage(HttpMethod.Post,
                        $"https://www.instagram.com/api/v1/web/create/{PK}/delete/");

                    request.Headers.Add("Host", "www.instagram.com");
                    request.Headers.Add("Connection", "keep-alive");
                    request.Headers.Add("sec-ch-ua-full-version-list", "\"Google Chrome\";v=\"137.0.7151.104\", \"Chromium\";v=\"137.0.7151.104\", \"Not/A)Brand\";v=\"24.0.0.0\"");
                    request.Headers.Add("sec-ch-ua-platform", "\"Windows\"");
                    request.Headers.Add("viewport-width", "1745");
                    request.Headers.Add("sec-ch-ua", "\"Google Chrome\";v=\"137\", \"Chromium\";v=\"137\", \"Not/A)Brand\";v=\"24\"");
                    request.Headers.Add("sec-ch-ua-mobile", "?0");
                    request.Headers.Add("X-IG-App-ID", "936619743392459");
                    request.Headers.Add("X-Requested-With", "XMLHttpRequest");
                    request.Headers.Add("Accept", "*/*");
                    request.Headers.Add("X-CSRFToken", param.CsrfToken);
                    //request.Headers.Add("X-Web-Session-ID", "e3vcfd:d6m0st:3m95nu");
                    //request.Headers.Add("X-ASBD-ID", "359341");
                    request.Headers.Add("dpr", "1.1");
                    request.Headers.Add("sec-ch-prefers-color-scheme", "light");
                    request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/137.0.0.0 Safari/537.36");
                    request.Headers.Add("X-IG-WWW-Claim", param.X_IG_Claim);
                    request.Headers.Add("sec-ch-ua-platform-version", "\"10.0.0\"");
                    request.Headers.Add("Origin", "https://www.instagram.com");
                    request.Headers.Add("Sec-Fetch-Site", "same-origin");
                    request.Headers.Add("Sec-Fetch-Mode", "cors");
                    request.Headers.Add("Sec-Fetch-Dest", "empty");
                    request.Headers.Add("Referer", $"https://www.instagram.com/p/{mediaId}/");
                    request.Headers.Add("Accept-Encoding", GramStatic.AcceptEncoding);
                    request.Headers.Add("Accept-Language", "en-GB,en-US;q=0.9,en;q=0.8");

                    // Set Content-Type and empty body
                    request.Content = new StringContent(string.Empty);
                    request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

                    var response = await client.SendAsync(request);
                    var content = await response.Content.ReadAsStreamAsync();
                    var responseString = HttpHelper.Decode(content, string.Join(", ", response.Content.Headers.ContentEncoding));
                    return new DeleteMediaIgResponseHandler(responseString);
                }
            }
            catch (AssertFailedException)
            {
                throw;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public VisualThreadResponse GetVisualThread(DominatorAccountModel _dominatorAccount, string threadId,
            string cursorId = null)
        {
            try
            {
                var requestParameter =
                    (IgRequestParameters)httpHelper.GetRequestParameter();

                requestParameter.UrlParameters = new Dictionary<string, string>();

                if (HasUnifiedInbox)
                    requestParameter.AddUrlParameters("use_unified_inbox", "true");

                if (!string.IsNullOrEmpty(cursorId))
                    requestParameter.AddUrlParameters("cursor", cursorId);

                requestParameter.Url = $"direct_v2/threads/{threadId}/";
                var url = requestParameter.GenerateUrl($"direct_v2/threads/{threadId}/");
                url = Constants.ApiUrl + url;

                return new VisualThreadResponse(httpHelper.GetRequest(url));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public async Task<MediaInfoIgResponseHandler> MediaInfo(DominatorAccountModel _dominatorAccount, AccountModel _Account,
            string mediaId, CancellationToken token)
        {
            try
            {
                #region OLD MediaInfo Code.
                //var requestParameter = (IgRequestParameters)httpHelper.GetRequestParameter();
                //requestParameter.UrlParameters = new Dictionary<string, string>();
                //// requestParameter.Body = jsonElements;
                //requestParameter.Url = $"media/{mediaId}/info/";
                //var url = requestParameter.GenerateUrl($"media/{mediaId}/info/");
                //url = Constants.ApiUrl + url;
                //var mediaInfo = new MediaInfoIgResponseHandler(httpHelper.GetRequest(url));
                //if (mediaInfo.Issue != null && mediaInfo.Issue.Status == null)
                //{
                //    delayService.ThreadSleep(TimeSpan.FromSeconds(5));
                //    //Thread.Sleep(TimeSpan.FromSeconds(3));// Thread.Sleep(TimeSpan.FromSeconds(3));
                //    mediaInfo = new MediaInfoIgResponseHandler(httpHelper.GetRequest(url));
                //}

                //return mediaInfo;
                #endregion
                var MediaID = GramStatic.GetCodeFromIDOrUrl(mediaId);
                MediaInfoIgResponseHandler mediaInfo = null;
                var url = $"https://www.instagram.com/api/v1/media/{MediaID}/info/";
                var param = GetWebParameter(_dominatorAccount);
                using (var client = new HttpClient(param.httpClient))
                {
                    var baseUri = new Uri("https://www.instagram.com");
                    // ✅ Headers to simulate real browser
                    client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/137.0.0.0 Safari/537.36");
                    client.DefaultRequestHeaders.Add("X-CSRFToken", param.CsrfToken);
                    client.DefaultRequestHeaders.Add("X-IG-App-ID", "936619743392459");
                    client.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
                    client.DefaultRequestHeaders.Add("X-Instagram-AJAX", GramStatic.InstagramAjax());
                    //client.DefaultRequestHeaders.Add("X-Web-Session-ID", "ahil24:tvgxqg:4s9dlu");
                    //client.DefaultRequestHeaders.Add("X-ASBD-ID", "359341");
                    if(!string.IsNullOrEmpty(param.X_IG_Claim))
                        client.DefaultRequestHeaders.Add("X-IG-WWW-Claim", param.X_IG_Claim);
                    client.DefaultRequestHeaders.Add("Accept", "*/*");
                    client.DefaultRequestHeaders.Add("Accept-Encoding", GramStatic.AcceptEncoding);
                    client.DefaultRequestHeaders.Referrer = new Uri($"https://www.instagram.com/p/{mediaId}/");

                    // ✅ Send GET request
                    var response = await client.GetAsync(url);
                    var responseBody = await response.Content.ReadAsStreamAsync();
                    var ResponseText = HttpHelper.Decode(responseBody, string.Join(",", response.Content.Headers.ContentEncoding));
                    mediaInfo = new MediaInfoIgResponseHandler(ResponseText);
                }
                return mediaInfo;
                
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public async Task<UserFeedResponse> GetLikedMedia(DominatorAccountModel _dominatorAccount, string maxId = null)
        {
            try
            {
                #region GetOwnLikedMedia.
                //var requestParameters = (IgRequestParameters)httpHelper.GetRequestParameter();

                //requestParameters.Url = "feed/liked/";
                //var url = "feed/liked/";

                //if (!string.IsNullOrEmpty(maxId))
                //{
                //    requestParameters.Url += $"?max_id={maxId}";
                //    url += $"?max_id={maxId}";
                //}

                //url = Constants.ApiUrl + url;

                //return new UserFeedResponse(httpHelper.GetRequest(url));
                #endregion
                var param = GetWebParameter(_dominatorAccount);
                var url = $"https://www.instagram.com/async/wbloks/fetch/?appid=com.instagram.privacy.activity_center.liked_media_screen&type=app&__bkv={_dominatorAccount?.DeviceDetails?.PhoneId}";

                using (var client = new HttpClient(param.httpClient))
                {
                    client.DefaultRequestHeaders.Add("sec-ch-ua-full-version-list", "\"Google Chrome\";v=\"137.0.7151.120\", \"Chromium\";v=\"137.0.7151.120\", \"Not/A)Brand\";v=\"24.0.0.0\"");
                    client.DefaultRequestHeaders.Add("sec-ch-ua-platform", "Windows");
                    client.DefaultRequestHeaders.Add("viewport-width", "1745");
                    client.DefaultRequestHeaders.Add("sec-ch-ua", "\"Google Chrome\";v=\"137\", \"Chromium\";v=\"137\", \"Not/A)Brand\";v=\"24\"");
                    client.DefaultRequestHeaders.Add("sec-ch-ua-model", "");
                    client.DefaultRequestHeaders.Add("sec-ch-ua-mobile", "?0");
                    client.DefaultRequestHeaders.Add("dpr", "1.1");
                    client.DefaultRequestHeaders.Add("sec-ch-prefers-color-scheme", "light");
                    client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/137.0.0.0 Safari/537.36");
                    client.DefaultRequestHeaders.Add("sec-ch-ua-platform-version", "10.0.0");
                    client.DefaultRequestHeaders.Add("Accept", "*/*");
                    client.DefaultRequestHeaders.Add("Origin", "https://www.instagram.com");
                    client.DefaultRequestHeaders.Add("Sec-Fetch-Site", "same-origin");
                    client.DefaultRequestHeaders.Add("Sec-Fetch-Mode", "cors");
                    client.DefaultRequestHeaders.Add("Sec-Fetch-Dest", "empty");
                    client.DefaultRequestHeaders.Add("Referer", "https://www.instagram.com/your_activity/interactions/likes");
                    client.DefaultRequestHeaders.Add("Accept-Encoding", GramStatic.AcceptEncoding);
                    client.DefaultRequestHeaders.Add("Accept-Language", "en-GB,en-US;q=0.9,en;q=0.8");

                    var postData = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("__user", "0"),
                        new KeyValuePair<string, string>("__a", "1"),
                        new KeyValuePair<string, string>("__req", "5"),
                        new KeyValuePair<string, string>("__hs", "20269.HYP:instagram_web_pkg.2.1...0"),
                        new KeyValuePair<string, string>("dpr", "1"),
                        new KeyValuePair<string, string>("__ccg", "EXCELLENT"),
                        new KeyValuePair<string, string>("__comet_req", "7"),
                        new KeyValuePair<string, string>("jazoest", GramStatic.CreateJazoest(_dominatorAccount?.DeviceDetails?.PhoneId)),
                        new KeyValuePair<string, string>("__crn", "comet.igweb.PolarisYourActivityInteractionsRoute"),
                        new KeyValuePair<string, string>("params", "{}")
                    });

                    var response = await client.PostAsync(url, postData);
                    var ResponseText = await HttpHelper.Decode(response);
                    return new UserFeedResponse(ResponseText);
                }

            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public async Task<CommonIgResponseHandler> UnlikeMedia(DominatorAccountModel _dominatorAccount, AccountModel _Account,
            CancellationToken token, string generatedMediaId)
        {
            token.ThrowIfCancellationRequested();
            try
            {
                #region OLD Unlike Code
                //var jsonElements = new JsonElements()
                //{
                //    InventorySource = "media_or_ad",
                //    Delivery_Class = "organic",
                //    ModuleName = "photo_view_other",
                //    MediaId = generatedMediaId,
                //    Csrftoken = _Account.CsrfToken,
                //    RadioType = "mobile-hspa+",
                //    Uid = _dominatorAccount.AccountBaseModel.UserId,
                //    Uuid = _Account.Uuid,
                //    IsCarouselBumpedPost = "false",
                //    Containermodule = "feed_timeline",
                //    FeedPosition = Convert.ToString(new Random().Next(0, 9)),
                //    // &d = 0   //To...Do... Task
                //};
                //var requestParameter = (IgRequestParameters)httpHelper.GetRequestParameter();

                //requestParameter.PostDataParameters = new Dictionary<string, string>();

                //requestParameter.Body = jsonElements;
                //requestParameter.Url = $"media/{generatedMediaId}/unlike/";
                //var url = requestParameter.GenerateUrl($"media/{generatedMediaId}/unlike/");
                //url = Constants.ApiUrl + url;
                //var postData = requestParameter.GenerateBody();
                //token.ThrowIfCancellationRequested();
                //return new CommonIgResponseHandler(httpHelper.PostRequest(url, postData));
                #endregion
                var MediaCodeID = GramStatic.GetCodeFromIDOrUrl(generatedMediaId);
                var url = $"https://www.instagram.com/api/v1/web/likes/{MediaCodeID}/unlike/";
                var param = GetWebParameter(_dominatorAccount);
                using (var client = new HttpClient(param.httpClient))
                {
                    var baseUri = new Uri("https://www.instagram.com");
                    // ✅ Headers
                    client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/137.0.0.0 Safari/537.36");
                    client.DefaultRequestHeaders.Add("X-CSRFToken", param.CsrfToken);
                    client.DefaultRequestHeaders.Add("X-IG-App-ID", "936619743392459");
                    client.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
                    //client.DefaultRequestHeaders.Add("X-Web-Session-ID", "epxwk5:wpdedu:hdizgh");
                    client.DefaultRequestHeaders.Add("X-ASBD-ID", "359341");
                    if (!string.IsNullOrEmpty(param.X_IG_Claim))
                        client.DefaultRequestHeaders.Add("X-IG-WWW-Claim", param.X_IG_Claim);
                    client.DefaultRequestHeaders.Add("X-Instagram-AJAX", GramStatic.InstagramAjax());
                    client.DefaultRequestHeaders.Add("Accept", "*/*");
                    client.DefaultRequestHeaders.Add("Accept-Encoding", GramStatic.AcceptEncoding);
                    client.DefaultRequestHeaders.Referrer = new Uri($"https://www.instagram.com/p/{generatedMediaId}/");

                    var content = new StringContent("", Encoding.UTF8, "application/x-www-form-urlencoded");

                    // ✅ POST Request to Unlike
                    var response = await client.PostAsync(url, content);
                    var responseBody = await response.Content.ReadAsStreamAsync();
                    var ResponseText = HttpHelper.Decode(responseBody, string.Join(",", response.Content.Headers.ContentEncoding));
                    return new CommonIgResponseHandler(ResponseText);
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public FollowerAndFollowingIgResponseHandler GetRecentFollowers(DominatorAccountModel _dominatorAccount)
        {
            try
            {
                var requestParameters = (IgRequestParameters)httpHelper.GetRequestParameter();

                requestParameters.Url = "friendships/recent_followers/";
                var url = "friendships/recent_followers/";
                url = Constants.ApiUrl + url;

                return new FollowerAndFollowingIgResponseHandler(httpHelper.GetRequest(url));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public async Task<LocationIgReponseHandler> SearchForLocation(DominatorAccountModel _dominatorAccount, string locations,
            bool isLocation = false)
        {
            try
            {
                //var requestParameters = (IgRequestParameters)httpHelper.GetRequestParameter();
                //requestParameters.Url =
                //    $"location_search/?latitude=33.985805&longitude=-118.25411166666666&search_query={locations}";
                //var url =
                //    $"location_search/?latitude=33.985805&longitude=-118.25411166666666&search_query={locations}";
                //url = Constants.ApiUrl + url;
                var ID = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
                var url = $"https://www.instagram.com/api/v1/location_search/?latitude=0&longitude=0&rankToken={Guid.NewGuid().ToString()}&timestamp={ID}&search_query={locations}";
                var param = GetWebParameter(_dominatorAccount);
                using (var client = new HttpClient(param.httpClient))
                {
                    var request = new HttpRequestMessage(HttpMethod.Get, url);

                    // Headers
                    request.Headers.Host = "www.instagram.com";
                    request.Headers.Add("Connection", "keep-alive");
                    request.Headers.Add("sec-ch-ua-full-version-list", "\"Google Chrome\";v=\"137.0.7151.104\", \"Chromium\";v=\"137.0.7151.104\", \"Not/A)Brand\";v=\"24.0.0.0\"");
                    request.Headers.Add("sec-ch-ua-platform", "\"Windows\"");
                    request.Headers.Add("viewport-width", "1745");
                    request.Headers.Add("sec-ch-ua", "\"Google Chrome\";v=\"137\", \"Chromium\";v=\"137\", \"Not/A)Brand\";v=\"24\"");
                    request.Headers.Add("sec-ch-ua-model", "\"\"");
                    request.Headers.Add("sec-ch-ua-mobile", "?0");
                    request.Headers.Add("X-IG-App-ID", "936619743392459");
                    request.Headers.Add("X-Requested-With", "XMLHttpRequest");
                    request.Headers.Add("X-CSRFToken", param.CsrfToken);
                    //request.Headers.Add("X-Web-Session-ID", "rpyxrk:mzimnh:z7ki3q");
                    //request.Headers.Add("X-ASBD-ID", "359341");
                    request.Headers.Add("dpr", "1.1");
                    request.Headers.Add("sec-ch-prefers-color-scheme", "light");
                    if(string.IsNullOrEmpty(param.X_IG_Claim))
                        request.Headers.Add("X-IG-WWW-Claim", param.X_IG_Claim);
                    request.Headers.Add("sec-ch-ua-platform-version", "\"10.0.0\"");
                    request.Headers.Add("Sec-Fetch-Site", "same-origin");
                    request.Headers.Add("Sec-Fetch-Mode", "cors");
                    request.Headers.Add("Sec-Fetch-Dest", "empty");
                    request.Headers.Referrer = new Uri("https://www.instagram.com/");
                    request.Headers.Add("Origin", "https://www.instagram.com");
                    request.Headers.Add("Accept-Encoding", GramStatic.AcceptEncoding);
                    request.Headers.Add("Accept-Language", "en-GB,en-US;q=0.9,en;q=0.8");
                    request.Headers.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/137.0.0.0 Safari/537.36");
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));

                    try
                    {
                        var response = await client.SendAsync(request);
                        var ResponseText = await HttpHelper.Decode(response);
                        return new LocationIgReponseHandler(ResponseText,locations);
                    }
                    catch
                    {
                        return null;
                    }
                }
                //return new LocationIgReponseHandler(httpHelper.GetRequest(url));
            }
            catch (Exception ex)
            {
                ex.DebugLog();

                return null;
            }
        }

        public LocationIdIgReponseHandler SearchLocationId(DominatorAccountModel _dominatorAccount, string LocationId)
        {
            try
            {
                var requestParameters = (IgRequestParameters)httpHelper.GetRequestParameter();
                requestParameters.Url = $"feed/location/{LocationId}";
                var Locationurl = Constants.ApiUrl + $"feed/location/{LocationId}";
                var LocationIdIgReponseHandler =
                    new LocationIdIgReponseHandler(httpHelper.GetRequest(Locationurl));
                return LocationIdIgReponseHandler;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public UploadMediaResponse ConfigurevideoAlbum(List<ImageDetails> imageList, string caption,
            string tagLocation = null, List<string> lstTagUserIds = null)
        {
            var sidecarId = ((long)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds).ToString();
            var childrenArray = new JArray();

            foreach (var image in imageList)
                childrenArray.Add(new JObject
                {
                    {"timezone_offset", "19800"},
                    {"source_type", "4"},
                    {"video_result", ""},
                    {"date_time_original", false},
                    {"filter_type", "0"},
                    {"upload_id", image.imageUploadId},
                    {"caption", caption},
                    {
                        "device",
                        "{\"manufacturer\":\"" + _dominatorAccount.DeviceDetails.Manufacturer + "\",\"model\":\"" +
                        _dominatorAccount.DeviceDetails.Model + "\",\"android_version\":" +
                        _dominatorAccount.DeviceDetails.AndroidVersion + ",\"android_release\":\"" +
                        _dominatorAccount.DeviceDetails.AndroidRelease + "\"}"
                    },
                    {"extra", "{\"source_width\":" + image.imageWidh + ",\"source_height\":" + image.imageHeight + "}"},
                    {"clips", "[{\"length\":" + image.imageLength + ",\"source_type\":\"4\"}]"},
                    {"audio_muted", false},
                    {"poster_frame_index", "0"},
                    {"length", image.imageLength}
                });

            var jsonElements = new JsonElements()
            {
                TimezoneOffset = "19800",
                Csrftoken = _Account.CsrfToken,
                SourceType = "4",
                Uid = _dominatorAccount.AccountBaseModel.UserId,
                DeviceId = _Account.Device_Id,
                Uuid = _Account.Uuid,
                Caption = caption,
                ClientSidecarId = sidecarId,
                UploadId = sidecarId,
                Device = new JsonElements.DeviceJson()
                {
                    AndroidRelease = _dominatorAccount.DeviceDetails.AndroidRelease,
                    AndroidVersion = int.Parse(_dominatorAccount.DeviceDetails.AndroidVersion),
                    Manufacturer = _dominatorAccount.DeviceDetails.Manufacturer,
                    Model = _dominatorAccount.DeviceDetails.Model
                },
                ChildrenMetadata = childrenArray
            };
            var requestParameters = (IgRequestParameters)httpHelper.GetRequestParameter();
            requestParameters.Headers.Remove("Session-ID");
            requestParameters.Headers.Remove("Content-Disposition");
            requestParameters.Headers.Remove("job");
            requestParameters.Headers.Remove("Content-Range");

            requestParameters.AddHeader("retry_context",
                "{\"num_step_auto_retry\":0,\"num_reupload\":0,\"num_step_manual_retry\":0}");
            requestParameters.AddHeader("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
            requestParameters.Body = jsonElements;

            var url = Constants.ApiUrl + requestParameters.GenerateUrl("media/configure_sidecar/");
            requestParameters.CreateSign();
            var postData = requestParameters.GenerateBody();
            var uploadMediaResponse = new UploadMediaResponse(httpHelper.PostRequest(url, postData));
            requestParameters.Headers.Remove("retry_context");
            requestParameters.Headers.Remove("Content-Type");
            return uploadMediaResponse;
        }

        public async Task<CommonIgResponseHandler> LikeOnComment(DominatorAccountModel _dominatorAccount, AccountModel _Account,
            CancellationToken token, string mediaId, string CommentID = "")
        {
            token.ThrowIfCancellationRequested();
            try
            {
                #region OLD LikeComment Code.
                //var jSOnElements = new JsonElements()
                //{
                //    Csrftoken = _Account.CsrfToken,
                //    Uid = _dominatorAccount.AccountBaseModel.UserId,
                //    Uuid = _Account.Uuid
                //};

                //var requestParameters = (IgRequestParameters)httpHelper.GetRequestParameter();

                //requestParameters.Body = jSOnElements;

                //requestParameters.Url = $"media/{mediaId}/comment_like/";
                //var url = Constants.ApiUrl + $"media/{mediaId}/comment_like/";

                //var postData = requestParameters.GenerateBody();
                //token.ThrowIfCancellationRequested();
                //return new CommonIgResponseHandler(httpHelper.PostRequest(url, postData));
                #endregion
                var url = $"https://www.instagram.com/api/v1/web/comments/like/{CommentID}/";
                var param = GetWebParameter(_dominatorAccount);
                using (var client = new HttpClient(param.httpClient))
                {
                    client.DefaultRequestHeaders.Add("X-IG-App-ID", "936619743392459");
                    client.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
                    client.DefaultRequestHeaders.Add("Accept", "*/*");
                    if (!string.IsNullOrEmpty(param.CsrfToken))
                        client.DefaultRequestHeaders.Add("X-CSRFToken", param.CsrfToken);
                    client.DefaultRequestHeaders.Add("Accept-Encoding", GramStatic.AcceptEncoding);
                    client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/137.0.0.0 Safari/537.36");
                    client.DefaultRequestHeaders.Add("Referer", $"https://www.instagram.com/p/{mediaId}/c/{CommentID}/");
                    // POST empty content
                    var response = await client.PostAsync(url, new StringContent(string.Empty, Encoding.UTF8, "application/x-www-form-urlencoded"));
                    var result = await response.Content.ReadAsStreamAsync();
                    var responseContent = HttpHelper.Decode(result, string.Join(",", response.Content.Headers.ContentEncoding));
                    return new CommonIgResponseHandler(responseContent);
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return default;
            }
        }

        public async Task<SendMessageIgResponseHandler> SendPhotoAsDirectMessage(DominatorAccountModel _dominatorAccount,
            AccountModel _Account, CancellationToken token, string userId, string photoPath, string threadId = null)
        {
            token.ThrowIfCancellationRequested();
            SendMessageIgResponseHandler sendPhotoInMessage = null;
            try
            {
                #region OLD Code for media send.
                //var jsonElements = new JsonElements()
                //{
                //    RecipientUsers = $"[[{userId}]]",
                //    Action = "send_item",
                //    ClientContext = Utilities.GetGuid(),
                //    Csrftoken = _Account.CsrfToken,
                //    Uuid = _Account.Uuid,
                //    // ThreadIds=threadId
                //};

                //var photoBytes = File.ReadAllBytes(photoPath);

                //var requestParameters = (IgRequestParameters)httpHelper.GetRequestParameter();
                //requestParameters.FileList = new Dictionary<string, FileData>();

                //requestParameters.Body = jsonElements;
                //requestParameters.AddFileList("photo", photoBytes,
                //    $"direct_temp_photo_{DateTimeUtilities.GetEpochTime()}.jpg");

                //requestParameters.Url = "direct_v2/threads/broadcast/upload_photo/";
                //var url = Constants.ApiUrl + "direct_v2/threads/broadcast/upload_photo/";

                //var postData = requestParameters.GenerateBody();
                //requestParameters.IsMultiPart = false;
                //token.ThrowIfCancellationRequested();
                //sendPhotoInMessage = new SendMessageIgResponseHandler(httpHelper.PostRequest(url, postData));
                //requestParameters.ContentType = Constants.ContentTypeDefault;
                #endregion

                var param = GetWebParameter(_dominatorAccount,true);
                var utl = IGMobileUtilities.Instance(_dominatorAccount);
                var imageBytes = await GetMediaData(photoPath);
                var entityName = GramStatic.GetMediaContext(); // Dynamic if needed
                var url = $"https://rupload.facebook.com/messenger_image/{entityName}";

                using (var client = new HttpClient())
                {
                    // Add required headers
                    client.DefaultRequestHeaders.Add("User-Agent",_dominatorAccount?.DeviceDetails?.Useragent ?? "Instagram 283.0.0.20.105 Android (31/12; 320dpi; 720x1470; vivo; V2029; 2027; qcom; en_US; 475221264)");
                    client.DefaultRequestHeaders.Add("X-Entity-Length", imageBytes.Length.ToString());
                    client.DefaultRequestHeaders.Add("X-Entity-Name", entityName);
                    client.DefaultRequestHeaders.Add("X-Entity-Type", "image/jpeg");
                    //client.DefaultRequestHeaders.Add("X-IG-SALT-IDS", "51052545");
                    client.DefaultRequestHeaders.Add("image_type", "FILE_ATTACHMENT");
                    client.DefaultRequestHeaders.Add("Offset", "0");
                    client.DefaultRequestHeaders.Add("Priority", "u=6, i");
                    client.DefaultRequestHeaders.Add("Accept-Language", "en-US");
                    client.DefaultRequestHeaders.Add("Authorization", param.Authorization);
                    client.DefaultRequestHeaders.Add("X-MID", param.MID);
                    client.DefaultRequestHeaders.Add("IG-U-DS-USER-ID", param.DsUserId);
                    //client.DefaultRequestHeaders.Add("IG-U-RUR", "CCO,73228986484,1782107269:01fecfacdccb027157f6a32e94f41a75fd90663fd595f5292126c20901f5a6444278f67a");
                    client.DefaultRequestHeaders.Add("IG-INTENDED-USER-ID", param.DsUserId);
                    client.DefaultRequestHeaders.Add("Accept-Encoding", GramStatic.AcceptEncoding);

                    // Create content as raw byte stream (octet-stream)
                    var content = new ByteArrayContent(imageBytes);
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                    // Send the POST request
                    var response = await client.PostAsync(url, content);
                    var ResponseText = await HttpHelper.Decode(response);
                    if(ResponseText !=null && !string.IsNullOrEmpty(ResponseText?.Response))
                    {
                        var mediaID = Utilities.GetBetween(ResponseText?.Response, "{\"media_id\":", "}");
                        var url2 = "https://i.instagram.com/api/v1/direct_v2/threads/broadcast/photo_attachment/";
                        var IgDeviceID = _dominatorAccount?.DeviceDetails?.PhoneId ?? "cba40baf-8663-4ee8-a0bb-74dc5d2e0a98";
                        var client2 = new HttpClient();
                        var request = new HttpRequestMessage(HttpMethod.Post, url2);

                        // Set required headers
                        request.Headers.Add("User-Agent", _dominatorAccount?.DeviceDetails?.Useragent  ?? "Instagram 283.0.0.20.105 Android (31/12; 320dpi; 720x1470; vivo; V2029; 2027; qcom; en_US; 475221264)");
                        request.Headers.Add("X-IG-App-Locale", "en_US");
                        request.Headers.Add("X-IG-Device-Locale", "en_US");
                        request.Headers.Add("X-IG-Mapped-Locale", "en_US");
                        request.Headers.Add("X-Pigeon-Session-Id", "UFS-"+ (_dominatorAccount?.DeviceDetails?.PhoneId ?? Guid.NewGuid().ToString()) + "-4");
                        request.Headers.Add("X-Pigeon-Rawclienttime", GdUtilities.GetRowClientTime());
                        //request.Headers.Add("X-IG-Bandwidth-Speed-KBPS", "353.000");
                        //request.Headers.Add("X-IG-Bandwidth-TotalBytes-B", "31183865");
                        //request.Headers.Add("X-IG-Bandwidth-TotalTime-MS", "75521");
                        request.Headers.Add("X-Bloks-Version-Id", "f5fbf62cc3c51dc0e6f4ffd3a79e0c5929ae0b8af58c54acd1e186871a92fb27");
                        request.Headers.Add("X-IG-WWW-Claim", param.X_IG_Claim);
                        request.Headers.Add("X-Bloks-Is-Layout-RTL", "false");
                        request.Headers.Add("X-IG-Device-ID", IgDeviceID);
                        request.Headers.Add("X-IG-Family-Device-ID",_dominatorAccount?.DeviceDetails?.FamilyId ?? "6a1184d2-87c2-454f-93c5-a491e0040ad7");
                        request.Headers.Add("X-IG-Android-ID", _dominatorAccount?.DeviceDetails?.DeviceId ?? "android-b10069c5ba7bbd58");
                        request.Headers.Add("X-IG-Timezone-Offset", "19800");
                        request.Headers.Add("X-IG-Nav-Chain", "ExploreFragment:explore_popular:26:main_search:1750570534.0::,ForYouSerpGridFragment:serp_top:28:button:1750570539.871::,UserSerpGridFragment:serp_users:29:button:1750570543.626::,ForYouSerpGridFragment:serp_top:31:button:1750570555.883::,UserSerpGridFragment:serp_users:32:button:1750570556.994::,UserSerpGridFragment:serp_users:49:button:1750571257.35::,UserDetailFragment:profile:50:search_result:1750571259.299::,ProfileMediaTabFragment:profile:51:button:1750571260.769::,DirectThreadFragment:direct_thread:52:message_button:1750571261.876::,DirectThreadFragment:direct_thread:53:button:1750571261.880::");
                        request.Headers.Add("X-IG-CLIENT-ENDPOINT", "DirectThreadFragment:direct_thread");
                        //request.Headers.Add("X-IG-SALT-IDS", "51052545");
                        //request.Headers.Add("X-IG-SALT-LOGGER-IDS", "231352080,42991645,25952257,42991646");
                        request.Headers.Add("X-FB-Connection-Type", "WIFI");
                        request.Headers.Add("X-IG-Connection-Type", "WIFI");
                        request.Headers.Add("X-IG-Capabilities", "3brTv10=");
                        request.Headers.Add("X-IG-App-ID", "567067343352427");
                        request.Headers.Add("Authorization", param.Authorization);
                        request.Headers.Add("X-MID", param.MID);
                        request.Headers.Add("IG-U-DS-USER-ID", param.DsUserId);
                        //request.Headers.Add("IG-U-RUR", "CCO,73228986484,1782107274:01fe9157667141722b7cfb05159b20320e0eca4f226f44b7d80d5907bb0a71e3d8055c08");
                        request.Headers.Add("IG-INTENDED-USER-ID", param.DsUserId);
                        request.Headers.Add("Accept-Language", "en-US");
                        request.Headers.Add("Accept-Encoding", GramStatic.AcceptEncoding);
                        var haveThreadID = !string.IsNullOrEmpty(threadId) && threadId.Length >= 35;
                        var context = haveThreadID ? utl.GetClientContext() : threadId;
                        // Form body
                        var body = new StringBuilder();
                        if(!haveThreadID)
                            body.Append($"recipient_users=%5B%5B{userId}%5D%5D");
                        body.Append("&action=send_item");
                        body.Append("&is_x_transport_forward=false");
                        body.Append("&is_shh_mode=0");
                        if (haveThreadID)
                            body.Append($"&thread_ids=[{threadId}]");
                        if(haveThreadID)
                            body.Append("&send_attribution=direct_thread");
                        else
                            body.Append("&send_attribution=message_button");
                        body.Append($"&client_context={context}");
                        body.Append($"&attachment_fbid={mediaID}"); // <-- previously uploaded image ID
                        body.Append($"&device_id={_dominatorAccount?.DeviceDetails?.DeviceId}");
                        body.Append($"&mutation_token={context}");
                        body.Append($"&_uuid={IgDeviceID}");
                        body.Append("&allow_full_aspect_ratio=true");
                        body.Append("&btt_dual_send=false");
                        if (!haveThreadID)
                            body.Append("&nav_chain=ExploreFragment%3Aexplore_popular%3A26%3Amain_search%3A1750570534.0%3A%3A%2CForYouSerpGridFragment%3Aserp_top%3A28%3Abutton%3A1750570539.871%3A%3A%2CUserSerpGridFragment%3Aserp_users%3A29%3Abutton%3A1750570543.626%3A%3A%2CForYouSerpGridFragment%3Aserp_top%3A31%3Abutton%3A1750570555.883%3A%3A%2CUserSerpGridFragment%3Aserp_users%3A32%3Abutton%3A1750570556.994%3A%3A%2CUserSerpGridFragment%3Aserp_users%3A49%3Abutton%3A1750571257.35%3A%3A%2CUserDetailFragment%3Aprofile%3A50%3Asearch_result%3A1750571259.299%3A%3A%2CProfileMediaTabFragment%3Aprofile%3A51%3Abutton%3A1750571260.769%3A%3A%2CDirectThreadFragment%3Adirect_thread%3A52%3Amessage_button%3A1750571261.876%3A%3A%2CDirectThreadFragment%3Adirect_thread%3A53%3Abutton%3A1750571261.880%3A%3A");
                        else
                            body.Append("&nav_chain=MainFeedFragment:feed_timeline:1:cold_start:1750582966.780:10#230#301:3660034980283411224,UserDetailFragment:profile:15:media_owner:1750584370.229::,ProfileMediaTabFragment:profile:16:button:1750584373.618::,DirectThreadFragment:direct_thread:17:message_button:1750584375.171::,DirectThreadFragment:direct_thread:18:button:1750584375.173::");
                        body.Append($"&offline_threading_id={context}");

                        // Set body content
                        request.Content = new StringContent(body.ToString(), Encoding.UTF8, "application/x-www-form-urlencoded");

                        // Send request
                        var response2 = await client2.SendAsync(request);
                        var responseText = await HttpHelper.Decode(response2);
                        sendPhotoInMessage = new SendMessageIgResponseHandler(responseText);
                        #region Get Visual V2 Inbox
                        //if (responseText != null && !string.IsNullOrEmpty(responseText?.Response) && responseText.Response.Contains("\"status\":\"ok\""))
                        //{
                        //    var currentThreadID = string.IsNullOrEmpty(threadId) ? Utilities.GetBetween(responseText?.Response, "\"thread_id\":\"", "\""):threadId;

                        //    var url4 = $"https://i.instagram.com/api/v1/direct_v2/threads/{currentThreadID}/?visual_message_return_type=unseen&seq_id=149&limit=20";

                        //    using (var client4 = new HttpClient())
                        //    {
                        //        // Required headers
                        //        client4.DefaultRequestHeaders.UserAgent.ParseAdd(_dominatorAccount?.DeviceDetails?.Useragent ?? "Instagram 283.0.0.20.105 Android (31/12; 320dpi; 720x1470; vivo; V2029; 2027; qcom; en_US; 475221264)");
                        //        client4.DefaultRequestHeaders.Add("X-IG-App-Locale", "en_US");
                        //        client4.DefaultRequestHeaders.Add("X-IG-Device-Locale", "en_US");
                        //        client4.DefaultRequestHeaders.Add("X-IG-Mapped-Locale", "en_US");
                        //        client4.DefaultRequestHeaders.Add("X-Pigeon-Session-Id", "UFS-"+ (_dominatorAccount?.DeviceDetails?.PhoneId ?? Guid.NewGuid().ToString()) + "-1");
                        //        client4.DefaultRequestHeaders.Add("X-Pigeon-Rawclienttime", GdUtilities.GetRowClientTime());
                        //        //client.DefaultRequestHeaders.Add("X-IG-Bandwidth-Speed-KBPS", "166.000");
                        //        //client.DefaultRequestHeaders.Add("X-IG-Bandwidth-TotalBytes-B", "2752953");
                        //        //client.DefaultRequestHeaders.Add("X-IG-Bandwidth-TotalTime-MS", "13797");
                        //        client4.DefaultRequestHeaders.Add("X-Bloks-Version-Id", "f5fbf62cc3c51dc0e6f4ffd3a79e0c5929ae0b8af58c54acd1e186871a92fb27");
                        //        client4.DefaultRequestHeaders.Add("X-IG-WWW-Claim", param.X_IG_Claim);
                        //        client4.DefaultRequestHeaders.Add("X-Bloks-Is-Layout-RTL", "false");
                        //        client4.DefaultRequestHeaders.Add("X-IG-Device-ID", IgDeviceID);
                        //        client4.DefaultRequestHeaders.Add("X-IG-Family-Device-ID", _dominatorAccount?.DeviceDetails?.FamilyId ?? "6a1184d2-87c2-454f-93c5-a491e0040ad7");
                        //        client4.DefaultRequestHeaders.Add("X-IG-Android-ID", _dominatorAccount?.DeviceDetails?.DeviceId ?? "android-b10069c5ba7bbd58");
                        //        client4.DefaultRequestHeaders.Add("X-IG-Timezone-Offset", "19800");
                        //        client4.DefaultRequestHeaders.Add("X-IG-Nav-Chain", "MainFeedFragment:feed_timeline:1:cold_start:1750582966.780:10#230#301:3660470155479824545,UserDetailFragment:profile:4:media_owner:1750582981.965::,ClipsProfileTabFragment:clips_profile:5:button:1750582984.659::,DirectThreadFragment:direct_thread:6:message_button:1750582986.708::,DirectThreadFragment:direct_thread:7:button:1750582986.713::");
                        //        client4.DefaultRequestHeaders.Add("X-IG-CLIENT-ENDPOINT", "DirectThreadFragment:direct_thread");
                        //        //client.DefaultRequestHeaders.Add("X-IG-SALT-LOGGER-IDS", "974456048,25624577,31795244,231352080,42991645,25952257,42991646,974462634");
                        //        client4.DefaultRequestHeaders.Add("X-FB-Connection-Type", "WIFI");
                        //        client4.DefaultRequestHeaders.Add("X-IG-Connection-Type", "WIFI");
                        //        client4.DefaultRequestHeaders.Add("X-IG-Capabilities", "3brTv10=");
                        //        client4.DefaultRequestHeaders.Add("X-IG-App-ID", "567067343352427");
                        //        client4.DefaultRequestHeaders.Add("Priority", "u=3");
                        //        client4.DefaultRequestHeaders.Add("Accept-Language", "en-US");
                        //        client4.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",param.Authorization);
                        //        client4.DefaultRequestHeaders.Add("X-MID",param.MID);
                        //        client4.DefaultRequestHeaders.Add("IG-U-DS-USER-ID", param.DsUserId);
                        //        //client.DefaultRequestHeaders.Add("IG-U-RUR", "CCO,73228986484,1782118989:01feaff823f8ebf47927f54331150a7b1b4d7cafc063c88e398658f1636b4691c9443113");
                        //        client.DefaultRequestHeaders.Add("IG-INTENDED-USER-ID", param.DsUserId);

                        //        // Send request
                        //        var response3 = await client4.GetAsync(url4);
                        //        var ResponseData = await HttpHelper.Decode(response3);
                        //    }
                        //}
                        #endregion
                    }
                    return sendPhotoInMessage;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return sendPhotoInMessage;
            }
        }

        public SendMessageIgResponseHandler IgSendUploadPhoto(DominatorAccountModel _dominatorAccount,
            AccountModel _Account, CancellationToken token, string userId, string photoPath, string threadId = null)
        {
            token.ThrowIfCancellationRequested();
            SendMessageIgResponseHandler UploadResponse = null;
            try
            {
                var uploadId =
                    ((long)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds).ToString();
                var storyImagePostDetails = new StoryImagePostDetails()
                {
                    upload_id = uploadId,
                    image_compression = "{\"lib_name\":\"moz\",\"lib_version\":\"3.1.m\",\"quality\":\"80\"}",
                    retry_context = "{\"num_step_auto_retry\":0,\"num_reupload\":0,\"num_step_manual_retry\":0}",
                    media_type = "1",
                    xsharing_user_ids = new string[] { },
                };
                var ruploadparam = SerializeObject(storyImagePostDetails);
                var requestParameters = (IgRequestParameters)httpHelper.GetRequestParameter();
                requestParameters.Headers.Add("X-Entity-Type", "image/jpeg");
                requestParameters.Headers.Add("X-Entity-Name", $"{uploadId}_{userId}");
                requestParameters.Headers.Add("Offset", "0");
                requestParameters.Headers["X-Instagram-Rupload-Params"] = ruploadparam;
                requestParameters.Url = $"rupload_igphoto/{uploadId}";
                var url = requestParameters.GenerateUrl($"rupload_igphoto/{uploadId}_{userId}");
                url = "https://i.instagram.com/" + url;
                var photoByteInArray = File.ReadAllBytes(photoPath);
                requestParameters.Headers.Add("X-Entity-Length", photoByteInArray.Length.ToString());
                var response = new UploadMediaResponse(httpHelper.PostRequest(url, photoByteInArray));
                if (response.Success)
                {
                    requestParameters.Headers.Remove("X-Entity-Type");
                    requestParameters.Headers.Remove("X-Entity-Name");
                    requestParameters.Headers.Remove("Offset");
                    requestParameters.Headers.Remove("X-Instagram-Rupload-Params");
                    requestParameters.Headers.Remove("X-Entity-Length");
                    UploadResponse =
                        BroadcastConfigurePhoto(_dominatorAccount, _Account, token, userId, photoPath, uploadId);
                    return UploadResponse;
                }

                return UploadResponse;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public SendMessageIgResponseHandler BroadcastConfigurePhoto(DominatorAccountModel _dominatorAccount,
            AccountModel _Account, CancellationToken token, string userId, string photoPath, string uploadId)
        {
            token.ThrowIfCancellationRequested();
            SendMessageIgResponseHandler sendPhotoInMessage = null;
            try
            {
                var jsonElements = new JsonElements()
                {
                    UploadId = uploadId,
                    RecipientUsers = $"[[{userId}]]",
                    ClientContext = Utilities.GetGuid(),
                    Uuid = _Account.Uuid,
                    Csrftoken = _Account.CsrfToken,
                    Action = "send_item",
                    DeviceId = _Account.Device_Id,
                    MutationToken = Utilities.GetGuid(),
                    AllowFullAspectRatio = true,
                    // ThreadIds=threadId
                };

                var photoBytes = File.ReadAllBytes(photoPath);

                var requestParameters = (IgRequestParameters)httpHelper.GetRequestParameter();
                requestParameters.FileList = new Dictionary<string, FileData>();

                requestParameters.Body = jsonElements;
                requestParameters.AddFileList("photo", photoBytes,
                    $"direct_temp_photo_{DateTimeUtilities.GetEpochTime()}.jpg");

                requestParameters.Url = "direct_v2/threads/broadcast/configure_photo/";
                var url = Constants.ApiUrl + "direct_v2/threads/broadcast/configure_photo/";

                var postData = requestParameters.GenerateBody();
                requestParameters.IsMultiPart = false;
                token.ThrowIfCancellationRequested();
                sendPhotoInMessage = new SendMessageIgResponseHandler(httpHelper.PostRequest(url, postData));

                requestParameters.ContentType = Constants.ContentTypeDefault;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return sendPhotoInMessage;
        }

        public CommonIgResponseHandler StoryUploadIGVideo(string videoFilePath, out string uploadId)
        {
            // CommonIgResponseHandler response = null;

            uploadId = ((long)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds).ToString();
            //FFProbe ffProbe = new FFProbe();
            //MediaInfo mediaInfo = ffProbe.GetMediaInfo(videoFilePath);
            var mediaInfo = Utility.GramStatic.GetMediaInfo(videoFilePath);
            var storyPostDetails = new StoryVideoPostDetails()
            {
                retry_context = "{\"num_step_auto_retry\":0,\"num_reupload\":0,\"num_step_manual_retry\":0}",
                media_type = "2",
                upload_id = uploadId,
                upload_media_height = mediaInfo.Streams[0].Height.ToString(),
                for_album = "1",
                for_direct_story = "1",
                upload_media_width = mediaInfo.Streams[0].Width.ToString(),
                upload_media_duration_ms =
                    (mediaInfo.Duration.TotalMilliseconds * 1000).ToString(CultureInfo.InvariantCulture),
                potential_share_types = new[] { "story", "direct_story" }
            };
            var ruploadparam = SerializeObject(storyPostDetails);
            var requestParameters = (IgRequestParameters)httpHelper.GetRequestParameter();

            requestParameters.Headers.Add("X-Entity-Name", uploadId);
            requestParameters.Headers.Add("X_FB_VIDEO_WATERFALL_ID", _Account.Uuid);
            requestParameters.Headers.Add("X-Entity-Type", "video/webm");
            requestParameters.Headers.Add("X-Instagram-Rupload-Params", ruploadparam);
            requestParameters.Headers.Add("Offset", "0");
            requestParameters.Url = $"rupload_igvideo/{uploadId}";
            var url = requestParameters.GenerateUrl($"rupload_igvideo/{uploadId}");
            url = "https://i.instagram.com/" + url;
            var photoByteInArray = File.ReadAllBytes(videoFilePath);
            requestParameters.Headers.Add("X-Entity-Length", photoByteInArray.Length.ToString());
            return new CommonIgResponseHandler(httpHelper.PostRequest(url, photoByteInArray));
        }

        public UploadMediaResponse StoryUploadIGPhoto(string photoFilePath, string uploadId,
            List<string> lstTagUserIds = null)
        {
            //UploadMediaResponse igPhotoResponse = null;
            // List<string> lst_location = new List<string>();
            var storyImagePostDetails = new StoryImagePostDetails()
            {
                upload_id = uploadId,
                image_compression = "{\"lib_name\":\"moz\",\"lib_version\":\"3.1.m\",\"quality\":\"87\"}",
                retry_context = "{\"num_step_auto_retry\":0,\"num_reupload\":0,\"num_step_manual_retry\":0}",
                media_type = "2",
            };
            var ruploadparam = SerializeObject(storyImagePostDetails);
            var requestParameters = (IgRequestParameters)httpHelper.GetRequestParameter();
            requestParameters.Headers["X-Entity-Type"] = "image / jpeg";
            requestParameters.Headers["X-Instagram-Rupload-Params"] = ruploadparam;
            requestParameters.Url = $"rupload_igphoto/{uploadId}";
            var url = requestParameters.GenerateUrl($"rupload_igphoto/{uploadId}");
            url = "https://i.instagram.com/" + url;
            var photoByteInArray = File.ReadAllBytes(photoFilePath);
            requestParameters.Headers["X-Entity-Length"] = photoByteInArray.Length.ToString();
            new UploadMediaResponse(httpHelper.PostRequest(url, photoByteInArray));
            return Story_Configure_To_Reel(photoFilePath, uploadId, lstTagUserIds);
        }

        public UploadMediaResponse Story_Configure_To_Reel(string photo, string uploadId, List<string> lstTagUserIds,
            string tagLocation = null)
        {
            UploadMediaResponse igPhotoResponse;
            var image = File.Exists(photo)
                ? Image.FromFile(photo)
                : Image.FromStream(new MemoryStream(new WebClient().DownloadData(photo)));

            var jsonElement = new JsonElements()
            {
                SourceType = "4",
                UploadId = uploadId,
                Csrftoken = _Account.CsrfToken,
                ClientTimestamp = (DateTimeUtilities.GetEpochTime() - RandomUtilties.GetRandomNumber(10, 3)).ToString(),
                ClientSharedAt = DateTimeUtilities.GetEpochTime().ToString(),
                ConfigureMode = "1",
                Device = new JsonElements.DeviceJson()
                {
                    AndroidRelease = _dominatorAccount.DeviceDetails.AndroidRelease,
                    AndroidVersion = int.Parse(_dominatorAccount.DeviceDetails.AndroidVersion),
                    Manufacturer = _dominatorAccount.DeviceDetails.Manufacturer,
                    Model = _dominatorAccount.DeviceDetails.Model
                },
                Extra = new JsonElements.ExtraJson()
                {
                    SourceHeight = image.Height,
                    SourceWidth = image.Width
                },
                Edits = new JsonElements.EditsJson()
                {
                    CropOriginalSize = new[] { image.Width, image.Height },
                    CropCenter = new[] { 0.0, 0.0 },
                    CropZoom = 1
                },
            };
            var requestParameters = (IgRequestParameters)httpHelper.GetRequestParameter();
            requestParameters.Body = jsonElement;
            var url = requestParameters.GenerateUrl("media/configure_to_story/");
            url = Constants.ApiUrl + url;
            var postData = requestParameters.GenerateBody();
            igPhotoResponse = new UploadMediaResponse(httpHelper.PostRequest(url, postData));
            return igPhotoResponse;
        }

        public UploadMediaResponse ConfigureStoryVideo(string videoFilePath, string thumbnailFilePath, string uploadId,
            string caption = "", string tagLocation = null)
        {
            //FFProbe ffProbe = new FFProbe();
            //MediaInfo mediaInfo = ffProbe.GetMediaInfo(videoFilePath);
            var mediaInfo = Utility.GramStatic.GetMediaInfo(videoFilePath);
            var jsonElement = new JsonElements()
            {
                Csrftoken = _Account.CsrfToken,
                ConfigureMode = "1",
                FilterType = "0",
                TimezoneOffset = "19800",
                PosterFrameIndex = 0,
                capture_type = "normal",
                audience = "default",
                MasOptIn = "NOT_PROMPTED",
                SourceType = "4",
                UploadId = uploadId,
                AudioMuted = false,
                AllowMultiConfigures = "1",
                VideoResult = "deprecated",
                ClientTimestamp = (DateTimeUtilities.GetEpochTime() - RandomUtilties.GetRandomNumber(10, 3)).ToString(),
                Uid = _dominatorAccount.AccountBaseModel.UserId,
                Uuid = _Account.Uuid,
                ClientSharedAt = DateTimeUtilities.GetEpochTime().ToString(),
                Device = new JsonElements.DeviceJson()
                {
                    AndroidRelease = _dominatorAccount.DeviceDetails.AndroidRelease,
                    AndroidVersion = int.Parse(_dominatorAccount.DeviceDetails.AndroidVersion),
                    Manufacturer = _dominatorAccount.DeviceDetails.Manufacturer,
                    Model = _dominatorAccount.DeviceDetails.Model
                },
                Length = Convert.ToInt32(mediaInfo.Duration.TotalSeconds),
                Clips = new[]{new JsonElements.ClipJson()
                {
                    Length = Convert.ToInt32(mediaInfo.Duration.TotalSeconds),
                    SourceType = "4"
                }},
                Extra = new JsonElements.ExtraJson()
                {
                    SourceHeight = mediaInfo.Streams[0].Height,
                    SourceWidth = mediaInfo.Streams[0].Width
                }
            };
            var requestParameters = (IgRequestParameters)httpHelper.GetRequestParameter();
            requestParameters.Headers.Remove("X-Entity-Name");
            requestParameters.Headers.Remove("X_FB_VIDEO_WATERFALL_ID");
            requestParameters.Headers.Remove("X-Entity-Length");
            requestParameters.Headers.Remove("X-Entity-Type");
            requestParameters.Headers.Remove("X-Instagram-Rupload-Params");
            requestParameters.Headers.Remove("Offset");
            requestParameters.Headers.Remove("retry_context");
            requestParameters.Body = jsonElement;
            var url = requestParameters.GenerateUrl("media/configure_to_story/?video=1");
            url = Constants.ApiUrl + url;
            var postData = requestParameters.GenerateBody();

            return new UploadMediaResponse(httpHelper.PostRequest(url, postData));
        }

        public PostStoryResponse GetStoriesUsers(DominatorAccountModel dominatorAccountModel, AccountModel accountModel,
            List<string> lstUser = null)
        {
            try
            {
                var capebilities =
                    "[{\"name\":\"SUPPORTED_SDK_VERSIONS\",\"value\":\"119.0,120.0,121.0,122.0,123.0,124.0,125.0,126.0,127.0,128.0,129.0,130.0,131.0,132.0,133.0,134.0,135.0,136.0,137.0,138.0,139.0,140.0,141.0,142.0,143.0,144.0,145.0,146.0,147.0\"},{\"name\":\"FACE_TRACKER_VERSION\",\"value\":\"14\"},{\"name\":\"segmentation\",\"value\":\"segmentation_enabled\"},{\"name\":\"COMPRESSION\",\"value\":\"ETC2_COMPRESSION\"},{\"name\":\"world_tracker\",\"value\":\"world_tracker_enabled\"},{\"name\":\"gyroscope\",\"value\":\"gyroscope_enabled\"}]";


                var userll = new string[lstUser.Count];
                foreach (var dummy in lstUser)
                    for (var i = 0; i < userll.Length; i++)
                        userll[i] = lstUser[i];

                var jsonElements = new JsonElements()
                {
                    SupportedCapabilitiesNew = capebilities,
                    Uuid = accountModel.Uuid,
                    Uid = dominatorAccountModel.AccountBaseModel.UserId,
                    Source = "feed_timeline",
                    UserIds = userll,
                };
                var requestParameters = (IgRequestParameters)httpHelper.GetRequestParameter();
                requestParameters.UrlParameters = new Dictionary<string, string>();
                requestParameters.PostDataParameters = new Dictionary<string, string>();
                requestParameters.Body = jsonElements;
                requestParameters.Url = "feed/reels_media/";
                var url = requestParameters.GenerateUrl("feed/reels_media/");
                url = Constants.ApiUrl + url;
                var postData = requestParameters.GenerateBody();
                var storyAdsResponse = new PostStoryResponse(httpHelper.PostRequest(url, postData));
                return storyAdsResponse;
            }
            catch (Exception)
            {
            }

            return null;
        }

        public CommonIgResponseHandler SeenUserStory(DominatorAccountModel dominatorAccountModel,
            AccountModel accountModel, List<UsersPostStory> StoryPost)
        {
            try
            {
                var lstDicationary = new Dictionary<string, string[]>();

                var stories = new string[1];

                for (var post = 0; post < StoryPost.Count; post++)
                {
                    var mediaId = StoryPost[post].UserMediaId + "_" + StoryPost[post].UserId;
                    var time = StoryPost[post].PostTime + "_" + StoryPost[post].currentTime;
                    stories[0] = time;
                    lstDicationary.Add(mediaId, stories);
                    stories = new string[1];
                }

                var jsonElements = new JsonElements()
                {
                    Uuid = accountModel.Uuid,
                    Uid = dominatorAccountModel.AccountBaseModel.UserId,
                    ContainerModuleForLike = "feed_timeline",
                    Csrftoken = accountModel.CsrfToken,
                    ReelMediaSkipped = new Dictionary<string, string>(),
                    LiveVodsSkipped = new Dictionary<string, string>(),
                    Nuxes = new Dictionary<string, string>(),
                    NuxesSkipped = new Dictionary<string, string>(),
                    LiveVods = new Dictionary<string, string>(),
                    Reels = lstDicationary,
                };
                var requestParameters = (IgRequestParameters)httpHelper.GetRequestParameter();
                requestParameters.UrlParameters = new Dictionary<string, string>();
                requestParameters.PostDataParameters = new Dictionary<string, string>();
                requestParameters.Body = jsonElements;
                requestParameters.Url = "media/seen/?reel=1&live_vod=0";
                var url = requestParameters.GenerateUrl("media/seen/?reel=1&live_vod=0");
                url = Constants.ApiUrlV2 + url;
                var postData = requestParameters.GenerateBody();
                var storyAdsResponse =
                    new CommonIgResponseHandler(httpHelper.PostRequest(url, postData));
                return storyAdsResponse;
            }
            catch (Exception)
            {
            }

            return null;
        }

        public StoryAdsResponse GetStoryAds(DominatorAccountModel dominatorAccountModel, AccountModel accountModel,
            List<string> userList)
        {
            try
            {
                var userll = new string[userList.Count];
                foreach (var dummy in userList)
                    for (var i = 0; i < userll.Length; i++)
                        userll[i] = userList[i];

                var jsonElements = new JsonElements()
                {
                    PhoneId = accountModel.PhoneId,
                    EntryPointIndex = "0",
                    traySessionId = accountModel.PhoneId,
                    Uid = dominatorAccountModel.AccountBaseModel.UserId,
                    Uuid = accountModel.Uuid,
                    ViewerSessionId = accountModel.Uuid,
                    TrayUserids = userll,
                };
                var requestParameters = (IgRequestParameters)httpHelper.GetRequestParameter();
                requestParameters.UrlParameters = new Dictionary<string, string>();
                requestParameters.PostDataParameters = new Dictionary<string, string>();
                requestParameters.Body = jsonElements;
                requestParameters.Url = "feed/injected_reels_media/";
                var url = requestParameters.GenerateUrl("feed/injected_reels_media/");
                url = Constants.ApiUrl + url;
                var postData = requestParameters.GenerateBody();
                requestParameters.CreateSign();
                var storyAdsResponse = new StoryAdsResponse(httpHelper.PostRequest(url, postData));
                return storyAdsResponse;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public CommonIgResponseHandler MuteFollowerPostUser(InstagramUser instagramUser,
            DominatorAccountModel _dominatorAccount, AccountModel _Account)
        {
            // CommonIgResponseHandler commonIgResponseHandler = null;
            var jsonElements = new JsonElements()
            {
                Uuid = _Account.Uuid,
                Uid = _dominatorAccount.AccountBaseModel.UserId,
                Csrftoken = _Account.CsrfToken,
                Target_posts_author_id = instagramUser.Pk,
            };
            var requestParameters = (IgRequestParameters)httpHelper.GetRequestParameter();

            requestParameters.Body = jsonElements;

            var url = Constants.ApiUrl +
                      requestParameters.GenerateUrl("friendships/mute_posts_or_story_from_follow/");

            var postData = requestParameters.GenerateBody();
            return new CommonIgResponseHandler(httpHelper.PostRequest(url, postData));
            // return commonIgResponseHandler;
        }

        public CommonIgResponseHandler MuteFollowerStoryUser(InstagramUser instagramUser,
            DominatorAccountModel _dominatorAccount, AccountModel _Account)
        {
            // CommonIgResponseHandler commonIgResponseHandler = null;

            var jsonElements = new JsonElements()
            {
                Uuid = _Account.Uuid,
                Uid = _dominatorAccount.AccountBaseModel.UserId,
                Csrftoken = _Account.CsrfToken,
                Target_reel_author_id = instagramUser.Pk,
            };
            var requestParameters = (IgRequestParameters)httpHelper.GetRequestParameter();

            requestParameters.Body = jsonElements;

            var url = Constants.ApiUrl +
                      requestParameters.GenerateUrl("friendships/mute_posts_or_story_from_follow/");

            var postData = requestParameters.GenerateBody();
            return new CommonIgResponseHandler(httpHelper.PostRequest(url, postData));
        }

        //&request_id=fddffa31-5ed7-499d-b1c4-51a3bc421d71 

        public CommonIgResponseHandler GetFeedTimeLineData(DominatorAccountModel dominatorAccountModel,
            AccountModel accountModel)
        {
            _Account = accountModel;
            CommonIgResponseHandler mobilePageSrcNew = null;
            if (string.IsNullOrEmpty(dominatorAccountModel.SessionId))
                dominatorAccountModel.SessionId = Utilities.GetGuid();
            var json1 = new JsonElements()
            {
                HasCameraPermission = 1,
                FeedViewInfo = "[]",
                PhoneId = accountModel.PhoneId,
                Reason = "cold_start_fetch",
                BatteryLevel = new Random().Next(90, 100),
                TimezoneOffset = "19800",
                DeviceId = accountModel.Uuid,
                RequestId = accountModel.Id,
                IsPullToRefresh = 0,
                Uuid = accountModel.Uuid,
                PanavisionMode = "",
                IsCharging = 1,
                Is_Dark_Mode = 0,
                WillSoundOn = 1,
                SessionId = dominatorAccountModel.SessionId,
                BlockVersioningId = Constants.BlockVersionningId
            };
            var requestParameters = (IgRequestParameters)httpHelper.GetRequestParameter();
            requestParameters.AddHeader("X-Attribution-ID", accountModel.AttributionId);
            requestParameters.AddHeader("X-Ads-Opt-Out", "0");
            requestParameters.AddHeader("X-Google-AD-ID", accountModel.GoogleId);
            requestParameters.AddHeader("X-DEVICE-ID", accountModel.Id);
            requestParameters.AddHeader("X-FB", "1");
            requestParameters.AddHeader("X-CM-Latency", "-1.000");
            requestParameters.AddHeader("X-CM-Bandwidth-KBPS", "-1.000");

            if (!string.IsNullOrEmpty(_Account.WwwClaim))
            {
                //_Account.WwwClaim = GetGdHttpHelper().Response.Headers["x-ig-set-www-claim"] ?? accountModel.WwwClaim;
                //requestParameters.Headers["x-ig-set-www-claim"]=accountModel.WwwClaim;
                requestParameters.Headers["X-IG-WWW-Claim"] = _Account.WwwClaim;
            }
            if (!string.IsNullOrEmpty(accountModel.AuthorizationHeader))
            {
                //requestParameters.AddHeader("Authorization", "");
                requestParameters.Headers["Authorization"] = accountModel.AuthorizationHeader;
            }
            if (!string.IsNullOrEmpty(accountModel.MidHeader))
            {
                requestParameters.Headers["X-MID"] = accountModel.MidHeader;
            }
            requestParameters.AddHeader("IG-U-RUR", "VLL");
            var time = GdUtilities.GetRowClientTime();
            requestParameters.AddHeader("IG-U-SHBTS", time);
            if (!string.IsNullOrEmpty(accountModel.DsUserId))
                requestParameters.AddHeader("IG-U-DS-USER-ID", accountModel.DsUserId);
            requestParameters.UrlParameters = new Dictionary<string, string>();
            requestParameters.Body = json1;
            requestParameters.DontSign();
            var postData = requestParameters.GenerateBody();
            var url = Constants.Api_B_Url + "feed/timeline/";
            mobilePageSrcNew = new CommonIgResponseHandler(httpHelper.PostRequest(url, postData));
            if (string.IsNullOrEmpty(accountModel.WwwClaim))
            {
                accountModel.WwwClaim = httpHelper.Response.Headers["x-ig-set-www-claim"];
            }
            if (string.IsNullOrEmpty(accountModel.DsUserId))
            {
                accountModel.DsUserId = httpHelper.Response.Headers["ig-set-ig-u-ds-user-id"];
            }
            UpdateImportantHeadersOnEachRequest(requestParameters, accountModel);
            requestParameters.Headers.Remove("X-Attribution-ID");
            requestParameters.Headers.Remove("X-Ads-Opt-Out");
            requestParameters.Headers.Remove("X-Google-AD-ID");
            requestParameters.Headers.Remove("X-DEVICE-ID");
            requestParameters.Headers.Remove("X-FB");
            requestParameters.Headers.Remove("X-CM-Latency");
            requestParameters.Headers.Remove("X-CM-Bandwidth-KBPS");
            requestParameters.CreateSign();
            return mobilePageSrcNew;
        }

        public void UpdateImportantHeadersOnEachRequest(IgRequestParameters requestParameters, AccountModel accountModel)
        {
            var updated_shbid = httpHelper.Response.Headers["ig-set-ig-u-shbid"];
            var updated_shbts = httpHelper.Response.Headers["ig-set-ig-u-shbts"];
            var updated_rur = httpHelper.Response.Headers["ig-set-ig-u-rur"];
            if (!string.IsNullOrEmpty(updated_shbid))
                requestParameters.Headers["IG-U-SHBID"] = updated_shbid;
            if (!string.IsNullOrEmpty(updated_shbts))
                requestParameters.Headers["IG-U-SHBTS"] = updated_shbts;
            if (!string.IsNullOrEmpty(updated_rur))
                requestParameters.Headers["IG-U-RUR"] = updated_rur;
            requestParameters.Headers["IG-U-DS-USER-ID"] = accountModel.DsUserId;
            requestParameters.Headers["IG-INTENDED-USER-ID"] = accountModel.DsUserId;
            requestParameters.Headers["X-IG-Nav-Chain"] = $"MainFeedFragment:feed_timeline:1:cold_start:{GdUtilities.GetRowClientTime()}";
        }
        public async Task<LoginIgResponseHandler> LoginAsync117(DominatorAccountModel _dominatorAccount,
            AccountModel _Account)
        {
            try
            {
                var jsonElements = new JsonElements()
                {
                    Jazoest = $"22{new Random().Next(101, 999)}",
                    CountryCodes = "[{\"country_code\":\"1\",\"source\":[\"default\"]}]",
                    PhoneId = _Account.PhoneId,
                    Csrftoken = _Account.CsrfToken,
                    Username = _dominatorAccount.AccountBaseModel.UserName,
                    Adid = _Account.AdId,
                    Guid = _Account.Guid,
                    DeviceId = _Account.Device_Id, //"android-15dfe7cbd99c64cf"
                    // Enc_Password = _dominatorAccount.AccountBaseModel.Password,
                    GoogleTokens = "[]",
                    LoginAttemptCount = "0",
                    Password = _dominatorAccount.AccountBaseModel.Password
                };

                var requestParameter = (IgRequestParameters)httpHelper.GetRequestParameter();
                requestParameter.PostDataParameters = new Dictionary<string, string>();
                requestParameter.Body = jsonElements;
                requestParameter.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                requestParameter.Headers.Remove("X-CSRFToken");
                requestParameter.Url = "accounts/login/";
                var url = requestParameter.GenerateUrl("accounts/login/");
                url = Constants.ApiUrl + url;

                var postData = requestParameter.GenerateBody();

                return new LoginIgResponseHandler(await httpHelper.PostRequestAsync(url, postData, CancellationToken));
            }
            catch (WebException ex)
            {
                ex.DebugLog();
                return null;
            }
        }


        public async Task<LoginIgResponseHandler> LoginAsync(DominatorAccountModel _dominatorAccount,
            AccountModel _Account)
        {
            try
            {
                //CountrySource coutrySource = new CountrySource();
                //coutrySource.country_code = _Account.CountryCode;
                //coutrySource.source = "default";
                _Account.MidHeader = httpHelper.Response.Headers["ig-set-x-mid"];
                // var serialize = JsonConvert.SerializeObject(coutrySource);
                var clientParams = new JsonElements
                {
                    DeviceId = _Account.Device_Id,
                    LoginAttemptCount = "1",
                    SecureFamilyDeviceId = "",
                    MachineId = _Account.MidHeader,
                    AccountsList = new List<object>(),
                    AuthASecureDeviceId = "",
                    Password = _Account.EncPwd,
                    FamilyDeviceId = _Account.FamilyId,
                    FbIgDeviceId = new List<object>(),
                    DeviceEmails = new List<object>(),
                    TryNum = "1",
                    EventFlow = "login_manual",
                    EventStep = "home_page",
                    OpenIdToken = new object(),
                    ContactPoint = _dominatorAccount.AccountBaseModel.UserName
                };

                var ServerParams = new JsonElements
                {

                    UsernameTextInputId = _Account.UsernameTextId,
                    DeviceId = _Account.Device_Id,
                    ServerLoginSource = "login",
                    WaterfallId = _Account.WaterfallId,
                    LoginSource = "Login",
                    INTERNALLatencyInstanceId = _Account.InstanceId,
                    IsPLatformLogin = 0,
                    CredentialType = "password",
                    FamilyDeviceId = _Account.FamilyId,
                    INTERNALLatencyMarkerId = _Account.MarkerId,
                    OfflineExperimentGroup = Constants.OfflineExperimentGroup,
                    INTERNAL_INFRA_THEME = "harm_f",
                    PasswordTextInputId = _Account.PasswordTextId,
                    QeDeviceId = Utilities.GetGuid(),
                    ArEventSource = "login_home_page",
                };
                var BkClientContext = new JsonElements
                {
                    BlockVersion = Constants.BlockVersionningId,
                    InstaStyIeId = Constants.InstaStyleId
                };
                var Params = new JsonElements
                {
                    ClientInputParams = clientParams,
                    ServerParams = ServerParams
                };

                var jsonElements = new JsonElements()
                {
                    Params = Params,
                    BkClientContext = BkClientContext,
                    BlockVersioningId = Constants.BlockVersionningId
                };

                var requestParameter = (IgRequestParameters)httpHelper.GetRequestParameter();
                requestParameter.PostDataParameters = new Dictionary<string, string>();
                requestParameter.Body = jsonElements;
                requestParameter.AddHeader("X-IG-App-Startup-Country", "unknown");
                requestParameter.Url = "accounts/login/";
                var url = requestParameter.GenerateUrl("bloks/apps/com.bloks.www.bloks.caa.login.async.send_login_request/");
                url = Constants.ApiUrl + url;
                requestParameter.DontSign();
                var postData = requestParameter.GenerateBody(true);

                return new LoginIgResponseHandler(await httpHelper.PostRequestAsync(url, postData, CancellationToken));
            }
            catch (WebException ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public async Task<bool> TypeKeyboardPreLoginAsync(DominatorAccountModel _dominatorAccount,
            AccountModel _Account)
        {
            try
            {
                //CountrySource coutrySource = new CountrySource();
                //coutrySource.country_code = _Account.CountryCode;
                //coutrySource.source = "default";
                _Account.MidHeader = httpHelper.Response.Headers["ig-set-x-mid"];
                var splittedUsername = _dominatorAccount.AccountBaseModel.UserName.ToArray();
                // var serialize = JsonConvert.SerializeObject(coutrySource);
                var generatedText = "";
                foreach (var item in splittedUsername)
                {
                    generatedText += item;
                    var clientParams = new JsonElements
                    {
                        AccountCenters = new string[0],
                        Query = generatedText,
                    };

                    var ServerParams = new JsonElements
                    {
                        TextInputId = _Account.TextInputId,
                        TypeaheadId = _Account.TypeAheadId,
                        TextComponentId = _Account.ComponentId,
                        INTERNALLatencyMarkerId = _Account.MarkerId,
                        INTERNAL_INFRA_THEME = "harm_f",
                        Fdid = _Account.Fdid,
                        WaterfallId = _Account.WaterfallId,
                        ScreenId = _Account.ScreenId,
                        INTERNALLatencyInstanceId = _Account.TextInstanceId
                    };
                    var BkClientContext = new JsonElements
                    {
                        BlockVersion = Constants.BlockVersionningId,
                        InstaStyIeId = Constants.InstaStyleId
                    };
                    var Params = new JsonElements
                    {
                        ClientInputParams = clientParams,
                        ServerParams = ServerParams
                    };

                    var jsonElements = new JsonElements()
                    {
                        Params = Params,
                        BkClientContext = BkClientContext,
                        BlockVersioningId = Constants.BlockVersionningId
                    };

                    var requestParameter = (IgRequestParameters)httpHelper.GetRequestParameter();
                    requestParameter.PostDataParameters = new Dictionary<string, string>();
                    requestParameter.Body = jsonElements;
                    requestParameter.Headers["X-IG-App-Startup-Country"] = "unknown";
                    requestParameter.Headers["X-FB-HTTP-Engine"] = "Liger";
                    requestParameter.Headers["X-FB-Client-IP"] = "True";
                    requestParameter.Url = "accounts/login/";
                    var url = requestParameter.GenerateUrl("bloks/apps/com.bloks.www.caa.login.cp_text_input_type_ahead/");
                    url = Constants.ApiUrl + url;
                    requestParameter.DontSign();
                    var postData = requestParameter.GenerateBody(true);

                    var response = await httpHelper.PostRequestAsync(url, postData, CancellationToken);
                    Thread.Sleep(1000);
                }
            }
            catch (WebException ex)
            {
                ex.DebugLog();
                return false;
            }
            return true;
        }


        public CommonIgResponseHandler Logout(DominatorAccountModel dominatorAccountModel, AccountModel accountModel)
        {
            // CommonIgResponseHandler commonIgResponseHandler = null;
            var jsonElements = new JsonElements()
            {
                Csrftoken = accountModel.CsrfToken,
                Guid = accountModel.Guid,
                DeviceId = accountModel.Device_Id,
                PhoneId = accountModel.PhoneId,
                Uuid = accountModel.Uuid,
                // OneTapAppLogin = "true"
            };
            var requestParameters = (IgRequestParameters)httpHelper.GetRequestParameter();
            requestParameters.Body = jsonElements;
            var url = Constants.ApiUrl + "accounts/logout/";
            requestParameters.DontSign();
            var postData = requestParameters.GenerateBody();
            requestParameters.CreateSign();
            return new CommonIgResponseHandler(httpHelper.PostRequest(url, postData));
        }

        public async Task<LoginIgResponseHandler> LoginAsyncForTwoFactor(DominatorAccountModel _dominatorAccount,
            AccountModel _Account)
        {
            //_dominatorAccount.DeviceDetails.DeviceId = "android-78ce4f70386fcd76";//"android-15dfe7cbd99c64cf";
            //_Account.Device_Id = "android-78ce4f70386fcd76";// "android-15dfe7cbd99c64cf";
            //6585385698721908230
            try
            {
                var jsonElements = new JsonElements()
                {
                    //Jazoest = $"22{new Random().Next(101, 999)}",
                    //Enc_Password = _Account.EncPwd,
                    //Csrftoken = _Account.CsrfToken,
                    //Username = _dominatorAccount.AccountBaseModel.UserName,
                    //Adid = _Account.AdId,
                    //Guid = _Account.Guid,
                    //DeviceId = _Account.Device_Id, //"android-15dfe7cbd99c64cf                             // Enc_Password = _dominatorAccount.AccountBaseModel.Password,
                    //GoogleTokens = "[]",
                    //LoginAttemptCount = "0",
                    Jazoest = $"22{new Random().Next(101, 999)}",
                    CountryCodes = "[{\"country_code\":\"1\",\"source\":[\"default\"]}]",
                    Enc_Password = _Account.EncPwd,
                    PhoneId = _Account.PhoneId,
                    Csrftoken = _Account.CsrfToken,
                    Username = _dominatorAccount.AccountBaseModel.UserName,
                    Adid = _Account.AdId,
                    Guid = _Account.Guid,
                    DeviceId = _Account.Device_Id, //"android-15dfe7cbd99c64cf"
                    GoogleTokens = "[]",
                    LoginAttemptCount = "0",
                };
                var requestParameter = (IgRequestParameters)httpHelper.GetRequestParameter();
                requestParameter.PostDataParameters = new Dictionary<string, string>();
                requestParameter.Body = jsonElements;
                requestParameter.Url = "accounts/login/";
                var url = requestParameter.GenerateUrl("accounts/login/");
                url = Constants.ApiUrl + url;

                var postData = requestParameter.GenerateBody();

                return new LoginIgResponseHandler(await httpHelper.PostRequestAsync(url, postData, CancellationToken));
            }
            catch (WebException ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public CommonIgResponseHandler ResetSendRequest(string challengeApiPathUrl,
            DominatorAccountModel _dominatorAccount, AccountModel _Account)
        {
            //CommonIgResponseHandler commonIgResponseHandler = null;
            var jsonElements = new JsonElements()
            {
                Csrftoken = _Account.CsrfToken,
                Guid = _Account.Guid,
                DeviceId = _Account.Device_Id,
            };
            var requestParameters = (IgRequestParameters)httpHelper.GetRequestParameter();
            requestParameters.Body = jsonElements;
            challengeApiPathUrl = challengeApiPathUrl.Replace("/challenge", "");
            var url = Constants.ApiUrl + requestParameters.GenerateUrl("challenge/reset" + challengeApiPathUrl + "");
            requestParameters.Headers.Remove("X-CSRFToken");
            var postData = requestParameters.GenerateBody();
            return new CommonIgResponseHandler(httpHelper.PostRequest(url, postData));
        }

        public FriendShipPendingResponseHandler PendingRequest(DominatorAccountModel _dominatorAccount)
        {
            try
            {
                var requestParameters = (IgRequestParameters)httpHelper.GetRequestParameter();
                requestParameters.Url = $"friendships/pending/";
                var url = $"friendships/pending/";
                url = Constants.ApiUrl + url;

                return new FriendShipPendingResponseHandler(httpHelper.GetRequest(url));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public FriendshipsResponse AcceptRequest(DominatorAccountModel _dominatorAccount, AccountModel _Account,
            CancellationToken token, string Userid)
        {
            try
            {
                token.ThrowIfCancellationRequested();
                var jsonElement = new JsonElements()
                {
                    Uid = _dominatorAccount.AccountBaseModel.UserId,
                    Uuid = _Account.Uuid,
                    RadioType = "mobile-hspa+",
                    UserId = Userid
                };
                var requestParameters = (IgRequestParameters)httpHelper.GetRequestParameter();
                requestParameters.Body = jsonElement;
                var url = requestParameters.GenerateUrl($"friendships/approve/{Userid}/");
                url = Constants.ApiUrl + url;
                var postData = requestParameters.GenerateBody();
                token.ThrowIfCancellationRequested();
                return new FriendshipsResponse(httpHelper.PostRequest(url, postData));
            }
            catch (AssertFailedException)
            {
                throw;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public CommonIgResponseHandler AccountContactPointPrefillForSignUp(DominatorAccountModel _dominatorAccount,
            AccountModel _Account)
        {
            CommonIgResponseHandler commonIgResponseHandler = null;
            try
            {
                var jsonElement = new JsonElements()
                {
                    PhoneId = _dominatorAccount.DeviceDetails.PhoneId,
                    Csrftoken = _Account.CsrfToken,
                    Guid = _dominatorAccount.DeviceDetails.Guid,
                    DeviceId = _Account.Device_Id,
                    Usage = "email_register_usage"
                };

                var requestParameter = (IgRequestParameters)httpHelper.GetRequestParameter();
                requestParameter.Body = jsonElement;
                requestParameter.Url = "accounts/contact_point_prefill/";
                var url = requestParameter.GenerateUrl("accounts/contact_point_prefill/");
                url = Constants.ApiUrl + url;
                var postData = requestParameter.GenerateBody();
                commonIgResponseHandler = new CommonIgResponseHandler(httpHelper.PostRequest(url, postData));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return commonIgResponseHandler;
        }

        public CommonIgResponseHandler CheckEmail(DominatorAccountModel _dominatorAccount,
            AccountModel _Account)
        {
            //{"_csrftoken":"P48BFFWTqDSpId8uNuVqt3pGNjgFZdZf","login_nonces":"[]","email":"bhaskarchittiboyena@globussoft.in","qe_id":"fe154bc6-0663-4bf2-b4d0-07948899eb73","waterfall_id":"545afeaf-0ee4-4660-bd3c-211c7d0e2090"}
            CommonIgResponseHandler commonIgResponseHandler = null;
            try
            {
                var jsonElement = new JsonElements()
                {
                    Csrftoken = _Account.CsrfToken,
                    Email = "isaacbaldwin75761e@gmail.com",
                    LoginNonces = "[]",
                    QeId = _dominatorAccount.DeviceDetails.Guid,
                    WaterfallId = _dominatorAccount.DeviceDetails.AdId
                };
                var requestParameter = (IgRequestParameters)httpHelper.GetRequestParameter();
                requestParameter.Body = jsonElement;
                requestParameter.Url = "users/check_email/";
                var url = requestParameter.GenerateUrl("users/check_email/");
                url = Constants.ApiUrl + url;
                var postData = requestParameter.GenerateBody();
                commonIgResponseHandler = new CommonIgResponseHandler(httpHelper.PostRequest(url, postData));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return commonIgResponseHandler;
        }

        public CommonIgResponseHandler SignUp_FetchHeader(DominatorAccountModel _dominatorAccount,
            AccountModel _Account)
        {
            CommonIgResponseHandler commonIgResponseHandler = null;
            //https://i.instagram.com/api/v1/si/fetch_headers/?guid=fe154bc606634bf2b4d007948899eb73&challenge_type=signup 
            var requestParameters = (IgRequestParameters)httpHelper.GetRequestParameter();
            requestParameters.UrlParameters = new Dictionary<string, string>();
            var url = Constants.ApiUrl +
                      $"si/fetch_headers/?guid={_dominatorAccount.DeviceDetails.Guid}&challenge_type=signup";
            commonIgResponseHandler = new CommonIgResponseHandler(httpHelper.GetRequest(url));
            return commonIgResponseHandler;
        }
        public CommonIgResponseHandler ConsentNewUserFlowBegins(DominatorAccountModel _dominatorAccount,
            AccountModel _Account)
        {
            CommonIgResponseHandler commonIgResponseHandler = null;
            try
            {
                var jsonElement = new JsonElements()
                {
                    Csrftoken = _Account.CsrfToken,
                    DeviceId = _Account.Device_Id,
                };
                var requestParameter = (IgRequestParameters)httpHelper.GetRequestParameter();
                requestParameter.Body = jsonElement;
                requestParameter.Url = "dynamic_onboarding/get_steps/";
                var url = requestParameter.GenerateUrl("dynamic_onboarding/get_steps/");
                url = Constants.ApiUrl + url;
                var postData = requestParameter.GenerateBody();
                commonIgResponseHandler = new CommonIgResponseHandler(httpHelper.PostRequest(url, postData));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return commonIgResponseHandler;
        }

        public CommonIgResponseHandler DynamicOnBoardingGetSteps(DominatorAccountModel _dominatorAccount,
            AccountModel _Account)
        {
            CommonIgResponseHandler commonIgResponseHandler = null;
            try
            {
                var jsonElement = new JsonElements()
                {
                    PhoneId = _dominatorAccount.DeviceDetails.PhoneId,
                    Csrftoken = _Account.CsrfToken,
                    Guid = _dominatorAccount.DeviceDetails.Guid,
                    WaterfallId = _dominatorAccount.DeviceDetails.Guid,
                    FbConnected = "false",
                    SeenPosts = "[]",
                    ProgressState = "prefetch",
                    FbInstalled = "false",
                    Locale = "en_US",
                    TimezoneOffset = "19800",
                    NetworkType = "WIFI-UNKNOWN",
                    IsCi = "false",
                    AndroidId = _Account.Device_Id,
                    RegFlowTaken = "email",
                    TosAccepted = "false"
                };
                var requestParameter = (IgRequestParameters)httpHelper.GetRequestParameter();
                requestParameter.Body = jsonElement;
                requestParameter.Url = "accounts/contact_point_prefill/";
                var url = requestParameter.GenerateUrl("accounts/contact_point_prefill/");
                url = Constants.ApiUrl + url;
                var postData = requestParameter.GenerateBody();
                commonIgResponseHandler = new CommonIgResponseHandler(httpHelper.PostRequest(url, postData));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return commonIgResponseHandler;
        }

        public CommonIgResponseHandler signUp(DominatorAccountModel _dominatorAccount, AccountModel _Account)
        {
            CommonIgResponseHandler commonIgResponseHandler = null;
            try
            {
                var jsonElement = new JsonElements()
                {
                    Csrftoken = _Account.CsrfToken,
                    DeviceId = _dominatorAccount.DeviceDetails.DeviceId,
                    TosVersion = "row",
                    AllowContactsSync = "true",
                    SnResult = "API_ERROR: class X.1ym:null", //"API_ERROR: null",
                    PhoneId = _dominatorAccount.DeviceDetails.PhoneId,
                    Username = "isaacbaldwin51",
                    FirstName = "isaacbaldwin",
                    Adid = _dominatorAccount.DeviceDetails.AdId,
                    Guid = _dominatorAccount.DeviceDetails.Guid,
                    Email = "isaacbaldwin75761e@gmail.com",
                    //SnNonce = "QmVubmV0dEJyYXVlcjEzQGdtYWlsLmNvbXwxNTQ3NzIwNTk4fMK+QqL/GAfBi8tVW37dQUf/5gc49lXi9Q==",
                    SnNonce = "bml0ZXNoLmt1bWFyQGdsb2J1c3NvZnQuaW58MTU0ODMyNTczMHzKmfPotjzjETmaFCwmoz2xLvTXjMm/4xk=",
                    ForceSignUpCode = "",
                    WaterfallId = _dominatorAccount.DeviceDetails.Guid,
                    QsStamp = "",
                    Password = "SAsa_5920"
                };
                var requestParameter = (IgRequestParameters)httpHelper.GetRequestParameter();
                requestParameter.Body = jsonElement;
                requestParameter.Url = "accounts/create/";
                var url = requestParameter.GenerateUrl("accounts/create/");
                url = Constants.ApiUrl + url;
                var postData = requestParameter.GenerateBody();
                commonIgResponseHandler = new CommonIgResponseHandler(httpHelper.PostRequest(url, postData));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return commonIgResponseHandler;
        }

        public TaggedPostResponseHandler TaggedPost(DominatorAccountModel _dominatorAccount, AccountModel _Account)
        {
            try
            {
                var requestParameters = (IgRequestParameters)httpHelper.GetRequestParameter();
                requestParameters.Url = $"usertags/{_dominatorAccount.AccountBaseModel.UserId}/feed/";
                var url = $"usertags/{_dominatorAccount.AccountBaseModel.UserId}/feed/";
                url = Constants.ApiUrl + url;

                return new TaggedPostResponseHandler(httpHelper.GetRequest(url));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public TaggedPostResponseHandler SomeoneTaggedPost(DominatorAccountModel _dominatorAccount, string userId,
            CancellationToken token, string maxid = null)
        {
            try
            {
                var requestParameters = (IgRequestParameters)httpHelper.GetRequestParameter();
                var url = string.Empty;
                if (maxid != null)
                {
                    requestParameters.Url = $"usertags/{userId}/feed/?max_id={maxid}";
                    url = $"usertags/{userId}/feed/?max_id={maxid}";
                }
                else
                {
                    requestParameters.Url = $"usertags/{userId}/feed/";
                    url = $"usertags/{userId}/feed/";
                }

                url = Constants.ApiUrl + url;

                return new TaggedPostResponseHandler(httpHelper.GetRequest(url));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        #region Upload video     

        public UploadMediaResponse IgVideo_uploadingVideo(DominatorAccountModel _dominatorAccount,
            AccountModel accountModel, CancellationToken token, string videoFilePath, string thumbnailFilePath,
            string uploadId, MediaInfo mediaInfo, bool isVideoAlbum = false, string caption = "",
            string tagLocation = null, List<string> lstTagUserIds = null)
        {
            UploadMediaResponse uploadMediaResponse = null;
            var videoPostDetails = new VideoPostDetails();
            var albumVideoPostDetails = new AlbumVideoPostDetails();
            token.ThrowIfCancellationRequested();
            try
            {
                if (!isVideoAlbum)
                    videoPostDetails = new VideoPostDetails()
                    {
                        retry_context = "{\"num_step_auto_retry\":0,\"num_reupload\":0,\"num_step_manual_retry\":0}",
                        // retry_context= "{\"num_reupload\":0,\"num_step_auto_retry\":0,\"num_step_manual_retry\":0}",
                        upload_media_width = mediaInfo.Streams[0].Width.ToString(),
                        media_type = "2",
                        xsharing_user_ids = new string[] { },
                        upload_media_duration_ms =
                            (mediaInfo.Duration.TotalMilliseconds * 1000).ToString(CultureInfo.InvariantCulture),
                        upload_media_height = mediaInfo.Streams[0].Height.ToString(),
                        upload_id = uploadId,
                        //is_fmp4="1",
                        //content_tags= "use_default_cover",
                    };
                else
                    albumVideoPostDetails = new AlbumVideoPostDetails()
                    {
                        retry_context = "{\"num_step_auto_retry\":0,\"num_reupload\":0,\"num_step_manual_retry\":0}",
                        upload_media_width = mediaInfo.Streams[0].Width.ToString(),
                        media_type = "2",
                        is_sidecar = "1",
                        xsharing_user_ids = new string[] { },
                        upload_media_duration_ms =
                            (mediaInfo.Duration.TotalMilliseconds * 1000).ToString(CultureInfo.InvariantCulture),
                        upload_media_height = mediaInfo.Streams[0].Height.ToString(),
                        upload_id = uploadId,
                    };

                var hasCode = uploadId.GetHashCode();
                var ruploadparam = string.Empty;
                if (!isVideoAlbum)
                    ruploadparam = SerializeObject(videoPostDetails);
                else
                    ruploadparam = SerializeObject(albumVideoPostDetails);

                var requestParameters = (IgRequestParameters)httpHelper.GetRequestParameter();
                requestParameters.Headers.Add("X_FB_VIDEO_WATERFALL_ID", _Account.Uuid);
                requestParameters.Headers["X-Instagram-Rupload-Params"] = ruploadparam;
                requestParameters.Headers.Remove("Connection");
                var AUTH = requestParameters.Headers["Authorization"];
                requestParameters.Headers["Connection"] = "Keep-Alive";
                requestParameters.Headers["Authorization"] = AUTH;
                requestParameters.Url = $"rupload_igvideo/{uploadId}";

                var url = requestParameters.GenerateUrl($"rupload_igvideo/{uploadId}_0_{hasCode}");
                url = "https://i.instagram.com/" + url;

                token.ThrowIfCancellationRequested();
                var response = httpHelper.GetRequest(url).Response;
                if (!string.IsNullOrEmpty(response))
                    uploadMediaResponse = IgVideo_PostVideo(_dominatorAccount, accountModel, uploadId, mediaInfo, token,
                        videoFilePath, thumbnailFilePath, hasCode, caption, tagLocation, lstTagUserIds);
            }
            catch (Exception)
            {
                //ignored
            }

            return uploadMediaResponse;
        }


        public UploadMediaResponse IgUploading_Reels_Video(DominatorAccountModel _dominatorAccount,
             AccountModel accountModel, CancellationToken token, string videoFilePath, string thumbnailFilePath,
             string uploadId, MediaInfo mediaInfo, bool isVideoAlbum = false, string caption = "",
             string tagLocation = null, List<string> lstTagUserIds = null)
        {
            UploadMediaResponse uploadMediaResponse = null;
            var videoPostDetails = new VideoPostDetails();
            token.ThrowIfCancellationRequested();
            try
            {

                videoPostDetails = new VideoPostDetails()
                {
                    retry_context = "{\"num_reupload\":0,\"num_step_auto_retry\":0,\"num_step_manual_retry\":0}",
                    upload_media_width = mediaInfo.Streams[0].Width.ToString(),
                    media_type = "2",
                    is_clips_video = "1",
                    content_tags = "use_default_cover",
                    xsharing_user_ids = new string[] { },
                    upload_media_duration_ms =
                            (mediaInfo.Duration.TotalMilliseconds).ToString(CultureInfo.InvariantCulture),
                    upload_media_height = mediaInfo.Streams[0].Height.ToString(),
                    upload_id = uploadId,
                };

                var hasCode = uploadId.GetHashCode();
                var ruploadparam = string.Empty;
                ruploadparam = SerializeObject(videoPostDetails);
                var requestParameters = (IgRequestParameters)httpHelper.GetRequestParameter();
                requestParameters.Headers.Add("X_FB_VIDEO_WATERFALL_ID", _Account.Uuid);
                requestParameters.Headers["X-Instagram-Rupload-Params"] = ruploadparam;
                //requestParameters.Headers.Remove("Connection");
                var AUTH = requestParameters.Headers["Authorization"];
                requestParameters.Headers["Connection"] = "Keep-Alive";
                requestParameters.Headers["Authorization"] = AUTH;
                requestParameters.Url = $"rupload_igvideo/{uploadId}";
                var url = requestParameters.GenerateUrl($"rupload_igvideo/{uploadId}_0_{hasCode}");
                url = "https://i.instagram.com/" + url;

                token.ThrowIfCancellationRequested();
                var response = httpHelper.GetRequest(url).Response;
                if (!string.IsNullOrEmpty(response))
                    uploadMediaResponse = IgVideo_PostVideo(_dominatorAccount, accountModel, uploadId, mediaInfo, token,
                        videoFilePath, thumbnailFilePath, hasCode, caption, tagLocation, lstTagUserIds);
            }
            catch (Exception)
            {
                //ignored
            }

            return uploadMediaResponse;
        }


        public UploadMediaResponse IgVideo_PostVideo(DominatorAccountModel _dominatorAccount, AccountModel accountModel,
            string uploadId, MediaInfo mediaInfo, CancellationToken token, string videoFilePath,
            string thumbnailFilePath, int hashCode, string caption = "", string tagLocation = null,
            List<string> lstTagUserIds = null)
        {
            UploadMediaResponse uploadMediaResponse = null;
            token.ThrowIfCancellationRequested();
            try
            {
                var requestParameters = (IgRequestParameters)httpHelper.GetRequestParameter();
                requestParameters.Headers["X-Entity-Name"] = $"{uploadId}_0_{hashCode}";
                requestParameters.Headers["X-Entity-Type"] = "video/mp4";
                requestParameters.Headers["Offset"] = "0";
                requestParameters.Url = $"rupload_igvideo/{uploadId}";
                var url = requestParameters.GenerateUrl($"rupload_igvideo/{uploadId}_0_{hashCode}");
                url = "https://i.instagram.com/" + url;
                var photoByteInArray = File.ReadAllBytes(videoFilePath);
                requestParameters.Headers["X-Entity-Length"] = photoByteInArray.Length.ToString();
                token.ThrowIfCancellationRequested();
                uploadMediaResponse = new UploadMediaResponse(httpHelper.PostRequest(url, photoByteInArray));
                requestParameters.Headers.Remove("X-Entity-Name");
                requestParameters.Headers.Remove("X-Entity-Type");
                requestParameters.Headers.Remove("Offset");
                requestParameters.Headers.Remove("X-Entity-Length");
                requestParameters.Headers.Remove("Content-Type");
                requestParameters.Headers.Remove("X-Instagram-Rupload-Params");
                requestParameters.Headers.Remove("X_FB_VIDEO_WATERFALL_ID");
            }
            catch (Exception)
            {
            }

            return uploadMediaResponse;
        }


        public UploadMediaResponse IgVideo_ReelsClips_Assets(DominatorAccountModel _dominatorAccount, AccountModel accountModel,
         string uploadId, MediaInfo mediaInfo, CancellationToken token, string videoFilePath,
         string thumbnailFilePath, int hashCode, string caption = "", string tagLocation = null,
         List<string> lstTagUserIds = null)
        {
            UploadMediaResponse uploadMediaResponse = null;
            token.ThrowIfCancellationRequested();
            try
            {
                var jsonElements = new JsonElements()
                {
                    Csrftoken = _Account.CsrfToken,
                    Uuid = _Account.Uuid,
                    speed = "0.0"
                };
                var requestParameters = (IgRequestParameters)httpHelper.GetRequestParameter();
                requestParameters.Body = jsonElements;
                requestParameters.Url = "creatives/clips_assets/";
                var url = requestParameters.GenerateUrl("creatives/clips_assets/");
                url = Constants.ApiUrl + url;
                //var photoByteInArray = File.ReadAllBytes(videoFilePath);
                //requestParameters.Headers["X-Entity-Length"] = photoByteInArray.Length.ToString();
                var postData = requestParameters.GenerateBody();
                token.ThrowIfCancellationRequested();
                uploadMediaResponse = new UploadMediaResponse(httpHelper.PostRequest(url, postData));
            }
            catch (Exception)
            {
            }

            return uploadMediaResponse;
        }

        public UploadMediaResponse igphoto_uploadingThumnail(DominatorAccountModel _dominatorAccount,
            AccountModel accountModel, CancellationToken token, string thumbnailFilePath, string uploadId,
            MediaInfo mediaInfo, string videoFilePath, bool isVideoAlbum = false, string caption = "",
            string tagLocation = null, List<string> lstTagUserIds = null)
        {
            UploadMediaResponse uploadMediaResponse = null;
            var albumvideoImagePostDetails = new AlbumVideoImagePostDetails();
            var VideoImagePostDetails = new StoryImagePostDetails();
            token.ThrowIfCancellationRequested();
            if (!isVideoAlbum)
                VideoImagePostDetails = new StoryImagePostDetails()
                {
                    upload_id = uploadId,
                    image_compression = "{\"lib_name\":\"moz\",\"lib_version\":\"3.1.m\",\"quality\":\"0\"}",
                    retry_context = "{\"num_reupload\":0,\"num_step_auto_retry\":0,\"num_step_manual_retry\":0}",
                    media_type = "2",
                    xsharing_user_ids = new string[] { },
                };
            else
                albumvideoImagePostDetails = new AlbumVideoImagePostDetails()
                {
                    upload_id = uploadId,
                    image_compression = "{\"lib_name\":\"moz\",\"lib_version\":\"3.1.m\",\"quality\":\"80\"}",
                    retry_context = "{\"num_step_auto_retry\":0,\"num_reupload\":0,\"num_step_manual_retry\":0}",
                    media_type = "1",
                    is_sidecar = "1",
                    xsharing_user_ids = new string[] { },
                };

            var ruploadparam = string.Empty;

            if (!isVideoAlbum)
                ruploadparam = SerializeObject(VideoImagePostDetails);
            else
                ruploadparam = SerializeObject(albumvideoImagePostDetails);

            var requestParameters = (IgRequestParameters)httpHelper.GetRequestParameter();
            requestParameters.Headers["X-Instagram-Rupload-Params"] = ruploadparam;
            requestParameters.Headers["X_FB_PHOTO_WATERFALL_ID"] = _dominatorAccount.DeviceDetails.Guid;
            requestParameters.Headers["X-Entity-Type"] = "image / jpeg";
            requestParameters.Headers["X-Entity-Name"] = $"{uploadId}_0_471181928";
            requestParameters.Headers["Offset"] = "0";
            requestParameters.Url = $"rupload_igphoto/{uploadId}";
            var url = requestParameters.GenerateUrl($"rupload_igphoto/{uploadId}_0_471181928");
            url = "https://i.instagram.com/" + url;
            var photoByteInArray = File.ReadAllBytes(thumbnailFilePath);
            requestParameters.Headers["X-Entity-Length"] = photoByteInArray.Length.ToString();
            token.ThrowIfCancellationRequested();
            uploadMediaResponse = new UploadMediaResponse(httpHelper.PostRequest(url, photoByteInArray));
            requestParameters.Headers.Remove("X-Entity-Type");
            requestParameters.Headers.Remove("X-Entity-Name");
            requestParameters.Headers.Remove("Content-Type");
            requestParameters.Headers.Remove("Offset");
            requestParameters.Headers.Remove("X-Entity-Length");
            requestParameters.Headers.Remove("X_FB_PHOTO_WATERFALL_ID");
            requestParameters.Headers.Remove("X-Instagram-Rupload-Params");
            //if (!isVideoAlbum)
            //    uploadMediaResponse = Configure(_dominatorAccount, _Account, token, thumbnailFilePath, uploadId,
            //        lstTagUserIds, caption, tagLocation);
            return uploadMediaResponse;
        }

        #endregion

        #region StoryUploadIGPhotoForDemo

        public UploadMediaResponse VideoStory(string videoFilePath, string VideoThumnail, string uploadId,
            List<string> lstTagUserIds = null, string tagLocation = null, int noOfStoryPost = 1)
        {
            var mediaInfo = Utility.GramStatic.GetMediaInfo(videoFilePath);
            var storyImagePostDetails = new StoryVideoFeedDetails()
            {
                upload_media_height = mediaInfo.Streams[0].Height.ToString(),
                xsharing_user_ids = new string[] { },
                upload_media_width = mediaInfo.Streams[0].Width.ToString(),
                for_direct_story = "1",
                upload_media_duration_ms = mediaInfo.Duration.TotalMilliseconds.ToString(),
                upload_id = uploadId,
                for_album = "1",
                retry_context = "{\"num_step_auto_retry\":0,\"num_reupload\":0,\"num_step_manual_retry\":0}",
                media_type = "2"
            };
            var ruploadparam = SerializeObject(storyImagePostDetails);
            var requestParameters = (IgRequestParameters)httpHelper.GetRequestParameter();
            requestParameters.Headers.Add("X_FB_VIDEO_WATERFALL_ID", _Account.Uuid);
            requestParameters.Headers["X-Instagram-Rupload-Params"] = ruploadparam;
            requestParameters.Url = $"rupload_igphoto/{uploadId}";
            var url = requestParameters.GenerateUrl($"rupload_igvideo/{uploadId}_0_322259861749");
            url = "https://i.instagram.com/" + url;
            var response = httpHelper.GetRequest(url).Response;
            return Story_Upload_IG_Video(videoFilePath, uploadId, lstTagUserIds, tagLocation, noOfStoryPost,
                VideoThumnail);
        }

        public UploadMediaResponse Story_Upload_IG_Video(string VideoFilePath, string uploadId,
            List<string> lstTagUserIds = null, string tagLocation = null, int noOfStoryPost = 1,
            string thumbnailImagePath = null)
        {
            UploadMediaResponse igVideoResponse = null;
            try
            {
                var requestParameters = (IgRequestParameters)httpHelper.GetRequestParameter();
                requestParameters.Headers["X-Entity-Type"] = "video/mp4";
                requestParameters.Headers["Offset"] = "0";
                requestParameters.Headers["X-Entity-Name"] = $"{uploadId}_0_389187795";
                requestParameters.Headers["Content-Type"] = "application/octet-stream";
                requestParameters.Url = $"rupload_igvideo/{uploadId}";
                var url = requestParameters.GenerateUrl($"rupload_igvideo/{uploadId}");
                url = "https://i.instagram.com/" + url;
                var photoByteInArray = File.ReadAllBytes(VideoFilePath);
                requestParameters.Headers["X-Entity-Length"] = photoByteInArray.Length.ToString();
                igVideoResponse = new UploadMediaResponse(httpHelper.PostRequest(url, photoByteInArray));
                requestParameters.Headers.Remove("X-Entity-Type");
                requestParameters.Headers.Remove("Offset");
                requestParameters.Headers.Remove("X-Entity-Name");
                requestParameters.Headers.Remove("Content-Type");
                requestParameters.Headers.Remove("X_FB_VIDEO_WATERFALL_ID");
                requestParameters.Headers.Remove("X-Instagram-Rupload-Params");
                PhotoForVideoStory(thumbnailImagePath, uploadId, lstTagUserIds, tagLocation, noOfStoryPost);
            }
            catch (Exception)
            {
            }

            return igVideoResponse;
        }

        public UploadMediaResponse PhotoForVideoStory(string photoFilePath, string uploadId,
            List<string> lstTagUserIds = null, string tagLocation = null, int noOfStoryPost = 1)
        {
            UploadMediaResponse uploadMediaResponse = null;
            var storyImagePostDetails = new StoryImagePostDetails()
            {
                upload_id = uploadId,
                image_compression = "{\"lib_name\":\"moz\",\"lib_version\":\"3.1.m\",\"quality\":\"84\"}",
                retry_context = "{\"num_step_auto_retry\":0,\"num_reupload\":0,\"num_step_manual_retry\":0}",
                media_type = "2",
                xsharing_user_ids = new string[] { },
            };
            var ruploadparam = SerializeObject(storyImagePostDetails);
            var requestParameters = (IgRequestParameters)httpHelper.GetRequestParameter();
            requestParameters.Headers.Add("X_FB_PHOTO_WATERFALL_ID", _Account.Uuid);
            requestParameters.Headers["X-Instagram-Rupload-Params"] = ruploadparam;
            requestParameters.Url = $"rupload_igphoto/{uploadId}";
            var url = requestParameters.GenerateUrl($"rupload_igphoto/{uploadId}_0_471181928");
            url = "https://i.instagram.com/" + url;
            var response = httpHelper.GetRequest(url).Response;
            uploadMediaResponse =
                Story_Video_Upload_IG_Photo(photoFilePath, uploadId, lstTagUserIds, tagLocation, noOfStoryPost);
            return uploadMediaResponse;
        }

        public UploadMediaResponse Story_Video_Upload_IG_Photo(string photoFilePath, string uploadId,
            List<string> lstTagUserIds = null, string tagLocation = null, int noOfStoryPost = 1)
        {
            var requestParameters = (IgRequestParameters)httpHelper.GetRequestParameter();

            requestParameters.Headers["X-Entity-Type"] = "image/jpeg";
            requestParameters.Headers["X-Entity-Name"] = uploadId;
            requestParameters.Headers["Content-Type"] = "application/octet-stream";
            requestParameters.Headers["Offset"] = "0";
            requestParameters.Url = $"rupload_igphoto/{uploadId}";
            var url = requestParameters.GenerateUrl($"rupload_igphoto/{uploadId}");
            url = "https://i.instagram.com/" + url;
            var photoByteInArray = File.ReadAllBytes(photoFilePath);
            requestParameters.Headers["X-Entity-Length"] = photoByteInArray.Length.ToString();
            httpHelper.PostRequest(url, photoByteInArray);
            Story_Configure_To_Reel(photoFilePath, uploadId, lstTagUserIds);
            return Story_Video_Configue_To_Reel(photoFilePath, uploadId, lstTagUserIds, tagLocation, noOfStoryPost);
        }


        public UploadMediaResponse Story_Upload_IG_Photo(string photoFilePath, string uploadId,
            List<string> lstTagUserIds = null, List<ImagePosition> imagePostion = null, string tagLocation = null,
            int noOfStoryPost = 1)
        {
            var storyImagePostDetails = new StoryImagePostDetails()
            {
                upload_id = uploadId,
                image_compression = "{\"lib_name\":\"moz\",\"lib_version\":\"3.1.m\",\"quality\":\"80\"}",
                retry_context = "{\"num_step_auto_retry\":0,\"num_reupload\":0,\"num_step_manual_retry\":0}",
                media_type = "1",
                original_photo_pdq_hash = "47702328ad4e89ff43b23d808cdc42fd330bb94ac9b6673514cddcd9da317217:100",
                xsharing_user_ids = new string[] { },
            };
            var ruploadparam = SerializeObject(storyImagePostDetails);

            var requestParameters = (IgRequestParameters)httpHelper.GetRequestParameter();
            requestParameters.Headers["X-Instagram-Rupload-Params"] = ruploadparam;
            requestParameters.Headers.Add("X_FB_PHOTO_WATERFALL_ID", _Account.Uuid);
            requestParameters.Headers["X-Entity-Type"] = "image/jpeg";
            requestParameters.Headers["X-Entity-Name"] = uploadId;
            requestParameters.Headers["Content-Type"] = "application/octet-stream";
            requestParameters.ContentType = "application/octet-stream";
            requestParameters.Headers["Offset"] = "0";
            requestParameters.Url = $"rupload_igphoto/{uploadId}";
            var url = requestParameters.GenerateUrl($"rupload_igphoto/{uploadId}");
            url = "https://i.instagram.com/" + url;
            var photoByteInArray = File.ReadAllBytes(photoFilePath);
            requestParameters.Headers["X-Entity-Length"] = photoByteInArray.Length.ToString();
            httpHelper.PostRequest(url, photoByteInArray);
            return Story_Photo_Configure_To_Reel(photoFilePath, uploadId, lstTagUserIds, imagePostion, tagLocation,
                noOfStoryPost);
        }

        public UploadMediaResponse Story_Photo_Configure_To_Reel(string photo, string uploadId,
            List<string> lstTagUserIds, List<ImagePosition> imagePostion = null, string tagLocation = null,
            int noOfStoryPost = 1)
        {
            UploadMediaResponse igPhotoResponse;

            try
            {
                var caption = string.Empty;

                var image = File.Exists(photo)
                    ? Image.FromFile(photo)
                    : Image.FromStream(new MemoryStream(new WebClient().DownloadData(photo)));
                // string support = "[{\"value\":\"13.0,14.0,15.0,16.0,17.0,18.0,19.0,20.0,21.0,22.0,23.0,24.0,25.0,26.0,27.0,28.0,29.0,30.0,31.0,32.0,33.0,34.0,35.0,36.0,37.0,38.0,39.0,40.0,41.0,42.0,43.0,44.0,45.0,46.0,47.0\",\"name\":\"SUPPORTED_SDK_VERSIONS\"},{\"value\":\"12\",\"name\":\"FACE_TRACKER_VERSION\"},{\"value\":\"PVR_COMPRESSION\",\"name\":\"COMPRESSION\"},{\"value\":\"world_tracker_enabled\",\"name\":\"world_tracker\"},{\"value\":\"gyroscope_enabled\",\"name\":\"gyroscope\"}]";
                var support =
                    "[{\"name\":\"SUPPORTED_SDK_VERSIONS\",\"value\":\"119.0,120.0,121.0,122.0,123.0,124.0,125.0,126.0,127.0,128.0,129.0,130.0,131.0,132.0,133.0,134.0,135.0,136.0,137.0,138.0,139.0,140.0,141.0,142.0,143.0,144.0,145.0,146.0,147.0\"},{\"name\":\"FACE_TRACKER_VERSION\",\"value\":\"14\"},{\"name\":\"segmentation\",\"value\":\"segmentation_enabled\"},{\"name\":\"COMPRESSION\",\"value\":\"ETC2_COMPRESSION\"},{\"name\":\"world_tracker\",\"value\":\"world_tracker_enabled\"},{\"name\":\"gyroscope\",\"value\":\"gyroscope_enabled\"}]";
                var jsonElement = new JsonElements()
                {
                    SupportedCapabilitiesNew = support,
                    AllowMultiConfigures = "1",
                    TimezoneOffset = "19800",
                    Csrftoken = _Account.CsrfToken,
                    ClientSharedAt = DateTimeUtilities.GetEpochTime().ToString(),
                    MediaFolder = "Screenshots",
                    ConfigureMode = "1",
                    SourceType = "4",
                    Uid = _dominatorAccount.AccountBaseModel.UserId,
                    DeviceId = _dominatorAccount.DeviceDetails.DeviceId,
                    Uuid = _Account.Uuid,
                    ImportedTakenAt = DateTimeUtilities.GetEpochTime().ToString(),
                    capture_type = "normal",
                    audience = "default",
                    UploadId = uploadId,
                    MasOptIn = "NOT_PROMPTED",
                    ClientTimestamp = DateTimeUtilities.GetEpochTime().ToString(),
                    Device = new JsonElements.DeviceJson()
                    {
                        AndroidRelease = _dominatorAccount.DeviceDetails.AndroidRelease,
                        AndroidVersion = int.Parse(_dominatorAccount.DeviceDetails.AndroidVersion),
                        Manufacturer = _dominatorAccount.DeviceDetails.Manufacturer,
                        Model = _dominatorAccount.DeviceDetails.Model
                    },
                    //ImplicitLocation = "{\"posting_location\":{\"lat\":\"21.2138594\",\"lng\":\"81.3231496\"}}",//lat,lng
                    Edits = new JsonElements.EditsJson()
                    {
                        CropOriginalSize = new[] { image.Width, image.Height },
                        CropCenter = new[] { 0.0, 0.0 },
                        CropZoom = 1
                    },
                    Extra = new JsonElements.ExtraJson()
                    {
                        SourceHeight = image.Height,
                        SourceWidth = image.Width
                    },
                };
                if (noOfStoryPost > 1)
                {
                    jsonElement.IsMultiUpload = "1";
                    jsonElement.MultiUploadSessionId = _dominatorAccount.DeviceDetails.Guid;
                }

                if (!string.IsNullOrEmpty(caption))
                {
                    jsonElement.RichTextFormatTypes = "[\"default\"]";
                    jsonElement.Caption = "";
                }

                if (lstTagUserIds.Count > 0)
                {
                    var mentioncount = 0;
                    var tagNoOfUser = 0;
                    jsonElement.ReelMentions = "["; //GdUtilities.GetRandomNumber(8)
                    // foreach (var tagUserId in lstTagUserIds)
                    for (var tag = 0; tag < lstTagUserIds.Count; tag++)
                    {
                        var ActualWidth = float.Parse(imagePostion[tag].XValue) +
                                          float.Parse(imagePostion[tag].Width) / 1.5f;
                        var minPercentOfImage = image.Width / 100f;
                        var xvalue = Convert.ToInt32(ActualWidth / minPercentOfImage);
                        float ActualHeigth = image.Height - Convert.ToInt32(imagePostion[tag].Height);
                        float actualHeigthPosition = Convert.ToInt32(imagePostion[tag].Yvalue);
                        var yValue = actualHeigthPosition / ActualHeigth;
                        if (mentioncount == 0) //imagePostion[tag].width
                        {
                            jsonElement.ReelMentions += "{\"x\":0." + xvalue + ",\"y\":" + yValue + ",\"z\":" +
                                                        tagNoOfUser +
                                                        ",\"width\":0.5,\"height\":0.103125,\"rotation\":0.0,\"user_id\":\"" +
                                                        lstTagUserIds[tag] + "\",\"is_sticker\":true}";
                            // jsonElement.ReelMentions += "{\"x\":0.5,\"y\":0.5,\"z\":" + tagNoOfUser + ",\"width\":0.5,\"height\":0.5,\"rotation\":0.0,\"user_id\":\"" + lstTagUserIds[tag] + "\",\"is_sticker\":true}";
                            jsonElement.StoryStickerIds = "mention_sticker_vibrant";
                            mentioncount++;
                            tagNoOfUser++;
                        }
                        else
                        {
                            jsonElement.ReelMentions += ",{\"x\":0." + xvalue + ",\"y\":" + yValue + ",\"z\":" +
                                                        tagNoOfUser +
                                                        ",\"width\":0.5,\"height\":0.103125,\"rotation\":0.0,\"user_id\":\"" +
                                                        lstTagUserIds[tag] + "\",\"is_sticker\":true}";
                            jsonElement.StoryStickerIds += ",mention_sticker_vibrant";
                            tagNoOfUser++;
                        }
                    }

                    jsonElement.ReelMentions += "]";
                }

                // jsonElement.StoryHashtags= "[{\"tag_name\":\"ssachin_saw\",\"x\":0.49445212,\"y\":0.59530544,\"z\":0,\"width\":0.3,\"height\":0.0609375,\"rotation\":0.0,\"is_sticker\":true}]";
                // jsonElement.StoryLocations = "[{\"x\":0.49445212,\"y\":0.59530544,\"z\":0,\"width\":0.3,\"height\":0.0609375,\"rotation\":0.0,\"location_id\":\"" + locationId + "\",\"is_sticker\":true}]";
                // jsonElement.StoryStickerIds += "hashtag_sticker_rainbow";
                var requestParameters = (IgRequestParameters)httpHelper.GetRequestParameter();
                requestParameters.Headers.Remove("X-Entity-Length");
                requestParameters.Headers.Remove("X-Entity-Type");
                requestParameters.Headers.Remove("X-Instagram-Rupload-Params");
                requestParameters.Headers.Remove("X_FB_PHOTO_WATERFALL_ID");
                requestParameters.Headers.Remove("X-Entity-Name");
                requestParameters.Headers.Remove("Offset");
                // requestParameters.Headers.Remove("retry_context");

                requestParameters.Headers.Remove("Content-Type");
                requestParameters.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
                requestParameters.AddHeader("retry_context",
                    "{\"num_step_auto_retry\":0,\"num_reupload\":0,\"num_step_manual_retry\":0}");
                requestParameters.Body = jsonElement;
                requestParameters.IsMultiPart = false;

                var url = requestParameters.GenerateUrl("media/configure_to_story/");
                url = Constants.ApiUrl + url;
                var postData = requestParameters.GenerateBody();
                igPhotoResponse = new UploadMediaResponse(httpHelper.PostRequest(url, postData));
                if (igPhotoResponse != null && igPhotoResponse.Success)
                    try
                    {
                        image.Dispose();
                        File.Delete(photo);
                    }
                    catch (Exception)
                    {
                        //ignored                     
                    }

                return igPhotoResponse;
            }
            catch (Exception)
            {
                //ignored
            }

            return null;
        }

        public UploadMediaResponse Story_Video_Configue_To_Reel(string photo, string uploadId,
            List<string> lstTagUserIds, string tagLocation = null, int noOfStoryPost = 1)
        {
            var lat = string.Empty;
            var lng = string.Empty;
            if (!string.IsNullOrEmpty(tagLocation))
            {
                var location = JObject.Parse(tagLocation);
                lat = location["lat"].ToString();
                lng = location["lng"].ToString();
            }

            UploadMediaResponse igVideoResponse = null;
            var image = File.Exists(photo)
                ? Image.FromFile(photo)
                : Image.FromStream(new MemoryStream(new WebClient().DownloadData(photo)));
            //FFProbe ffProbe = new FFProbe();
            //MediaInfo mediaInfo = ffProbe.GetMediaInfo(photo);
            var mediaInfo = Utility.GramStatic.GetMediaInfo(photo);
            var support =
                "[{\"name\":\"SUPPORTED_SDK_VERSIONS\",\"value\":\"119.0,120.0,121.0,122.0,123.0,124.0,125.0,126.0,127.0,128.0,129.0,130.0,131.0,132.0,133.0,134.0,135.0,136.0,137.0,138.0,139.0,140.0,141.0,142.0,143.0,144.0,145.0,146.0,147.0\"},{\"name\":\"FACE_TRACKER_VERSION\",\"value\":\"14\"},{\"name\":\"segmentation\",\"value\":\"segmentation_enabled\"},{\"name\":\"COMPRESSION\",\"value\":\"ETC2_COMPRESSION\"},{\"name\":\"world_tracker\",\"value\":\"world_tracker_enabled\"},{\"name\":\"gyroscope\",\"value\":\"gyroscope_enabled\"}]";

            var jsonElement = new JsonElements()
            {
                SupportedCapabilitiesNew = support,
                AllowMultiConfigures = "1",
                TimezoneOffset = "19800",
                Csrftoken = _Account.CsrfToken,
                ClientSharedAt = DateTimeUtilities.GetEpochTime().ToString(),
                MediaFolder = "Screenshots",
                ConfigureMode = "1",
                SourceType = "4",
                Uid = _dominatorAccount.AccountBaseModel.UserId,
                DeviceId = _dominatorAccount.DeviceDetails.DeviceId,
                Uuid = _Account.Uuid,
                ImportedTakenAt = DateTimeUtilities.GetEpochTime().ToString(),
                capture_type = "normal",
                audience = "default",
                UploadId = uploadId,
                MasOptIn = "NOT_PROMPTED",
                FilterType = "0",
                VideoResult = "",
                ClientTimestamp = DateTimeUtilities.GetEpochTime().ToString(),
                Device = new JsonElements.DeviceJson()
                {
                    AndroidRelease = _dominatorAccount.DeviceDetails.AndroidRelease,
                    AndroidVersion = int.Parse(_dominatorAccount.DeviceDetails.AndroidVersion),
                    Manufacturer = _dominatorAccount.DeviceDetails.Manufacturer,
                    Model = _dominatorAccount.DeviceDetails.Model
                },
                ImplicitLocation = "{\"posting_location\":{\"lat\":" + lat + ",\"lng\":" + lng + "}}",
                Edits = new JsonElements.EditsJson()
                {
                    CropOriginalSize = new[] { image.Width, image.Height },
                    CropCenter = new[] { 0.0, 0.0 },
                    CropZoom = 1
                },
                Extra = new JsonElements.ExtraJson()
                {
                    SourceHeight = image.Height,
                    SourceWidth = image.Width
                },
                Clips = new[]{new JsonElements.ClipJson()
                {
                    Length = Convert.ToInt32(mediaInfo.Duration.TotalSeconds),
                    SourceType = "4"
                }},
                Length = mediaInfo.Duration.Seconds,
                PosterFrameIndex = 0,
            };
            var requestParameters = (IgRequestParameters)httpHelper.GetRequestParameter();
            requestParameters.Headers.Remove("X-Entity-Length");
            requestParameters.Headers.Remove("X-Entity-Type");
            requestParameters.Headers.Remove("X-Instagram-Rupload-Params");
            requestParameters.Headers.Remove("X_FB_PHOTO_WATERFALL_ID");
            requestParameters.Headers.Remove("X-Entity-Name");
            requestParameters.Headers.Remove("Offset");
            requestParameters.Headers.Remove("Content-Type");
            requestParameters.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            requestParameters.AddHeader("retry_context",
                "{\"num_step_auto_retry\":0,\"num_reupload\":0,\"num_step_manual_retry\":0}");
            requestParameters.Body = jsonElement;
            requestParameters.IsMultiPart = false;

            var url = requestParameters.GenerateUrl("media/configure_to_story/?video=1");
            url = Constants.ApiUrl + url;
            var postData = requestParameters.GenerateBody();
            igVideoResponse = new UploadMediaResponse(httpHelper.PostRequest(url, postData));
            return igVideoResponse;
        }

        #endregion

        #region Upload Photo Album Process

        public UploadMediaResponse uploadPhotoAlbum1(DominatorAccountModel dominatorAccountModel, AccountModel _Account,
            CancellationToken token, List<string> imagesList, string caption, string tagLocation = null,
            List<string> lstTagUserIds = null)
        {
            token.ThrowIfCancellationRequested();
            try
            {
                var uploadIds = new List<string>();
                var ImageWidth = new List<int>();
                var ImageHeight = new List<int>();
                var requestParameter = (IgRequestParameters)httpHelper.GetRequestParameter();
                GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Instagram, dominatorAccountModel.UserName,
                    "Publisher", "Please wait... started to publish photo album");
                for (var iterationCount = 0; iterationCount < imagesList.Count; iterationCount++)
                {
                    var imagePath = imagesList[iterationCount];
                    delayService.ThreadSleep(TimeSpan.FromSeconds(new Random().Next(2, 4)));
                    //Thread.Sleep(TimeSpan.FromSeconds(new Random().Next(2, 4))); // Thread.Sleep(TimeSpan.FromSeconds(new Random().Next(2, 4)));
                    var uploadId = ((long)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds)
                        .ToString();
                    var image = Image.FromFile(imagePath);
                    var jsonElements = new JsonElements
                    {
                        UploadId = uploadId,
                        Uuid = _Account.Uuid,
                        Csrftoken = _Account.CsrfToken,
                        ImageCompression =
                            SerializeObject(new { lib_name = "jt", lib_version = "1.3.0", quality = "87" }),
                        IsSidecar = "1"
                    };

                    requestParameter.UrlParameters = new Dictionary<string, string>();
                    requestParameter.PostDataParameters = new Dictionary<string, string>();
                    requestParameter.FileList = new Dictionary<string, FileData>();
                    requestParameter.Body = jsonElements;
                    var photoByteInArray = !File.Exists(imagePath)
                        ? MediaUtilites.GetImageBytesFromUrl(imagePath)
                        : File.ReadAllBytes(imagePath);

                    requestParameter.AddFileList("photo", photoByteInArray, $"pending_media_{uploadId}.jpg");

                    var url = Constants.ApiUrl + requestParameter.GenerateUrl("upload/photo/");
                    var postData = requestParameter.GenerateBody();
                    token.ThrowIfCancellationRequested();
                    var uploadMediaResponse =
                        new UploadMediaResponse(httpHelper.PostRequest(url, postData));
                    requestParameter.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";

                    if (!uploadMediaResponse.Success)
                    {
                        return uploadMediaResponse;
                    }
                    else
                    {
                        uploadIds.Add(uploadId);
                        ImageWidth.Add(image.Width);
                        ImageHeight.Add(image.Height);
                    }
                }

                requestParameter.IsMultiPart = false;
                token.ThrowIfCancellationRequested();
                return ConfigurePhotoAlbum(dominatorAccountModel, _Account, token, uploadIds, ImageWidth, ImageHeight,
                    caption, tagLocation, lstTagUserIds);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public UploadMediaResponse UploadPhotoAlbum(DominatorAccountModel dominatorAccountModel, AccountModel _Account,
            CancellationToken token, List<string> imagesList, string caption, string tagLocation = null,
            List<string> lstTagUserIds = null)
        {
            token.ThrowIfCancellationRequested();
            try
            {
                var uploadIds = new List<string>();
                var ImageWidth = new List<int>();
                var ImageHeight = new List<int>();
                UploadMediaResponse igPhotoResponse = null;
                GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Instagram, dominatorAccountModel.UserName,
                    "Publisher", "Please wait... started to publish photo album");
                var requestParameters = (IgRequestParameters)httpHelper.GetRequestParameter();
                for (var iterationCount = 0; iterationCount < imagesList.Count; iterationCount++)
                {
                    token.ThrowIfCancellationRequested();
                    var imagePath = imagesList[iterationCount];
                    delayService.ThreadSleep(TimeSpan.FromSeconds(new Random().Next(2, 4)));
                    if (!File.Exists(imagePath))
                        continue;
                    var image = Image.FromFile(imagePath);
                    // Thread.Sleep(TimeSpan.FromSeconds(new Random().Next(2, 4)));//Thread.Sleep(TimeSpan.FromSeconds(new Random().Next(2, 4)));
                    var uploadId = ((long)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds)
                        .ToString();
                    //string uploadParameters= "{\"image_compression\":\"{\"lib_name\":\"moz\",\"lib_version\":\"3.1.m\",\"quality\":\"70\"}\",\"retry_context\":\"{\"num_step_auto_retry\":0,\"num_reupload\":0,\"num_step_manual_retry\":0}\",\"upload_id\":\""+uploadId+"\",\"media_type\":\"1\",\"is_sidecar\":\"1\",\"xsharing_user_ids\":\"[]\"}";

                    var storyImagePostDetails = new ImageAlbumPostDetails()
                    {
                        upload_id = uploadId,
                        image_compression = "{\"lib_name\":\"moz\",\"lib_version\":\"3.1.m\",\"quality\":\"70\"}",
                        retry_context = "{\"num_step_auto_retry\":0,\"num_reupload\":0,\"num_step_manual_retry\":0}",
                        media_type = "1",
                        is_sidecar = "1",
                        xsharing_user_ids = new string[] { },
                    };
                    var ruploadparam = SerializeObject(storyImagePostDetails);
                    requestParameters.Headers.Add("X-Entity-Type", "image/jpeg");
                    requestParameters.Headers.Add("X-Instagram-Rupload-Params", ruploadparam);
                    requestParameters.Headers.Add("X-Entity-Name", uploadId);
                    requestParameters.Headers.Add("Offset", "0");
                    requestParameters.Headers.Add("X_FB_PHOTO_WATERFALL_ID", dominatorAccountModel.DeviceDetails.Guid);
                    requestParameters.Url = $"rupload_igphoto/{uploadId}";
                    var url = $"https://i.instagram.com/rupload_igphoto/{uploadId}";
                    var photoByteInArray = File.ReadAllBytes(imagesList[iterationCount]);
                    requestParameters.Headers["X-Entity-Length"] = photoByteInArray.Length.ToString();
                    igPhotoResponse = new UploadMediaResponse(httpHelper.PostRequest(url, photoByteInArray));
                    if (!igPhotoResponse.Success || igPhotoResponse == null)
                    {
                        delayService.ThreadSleep(5);
                        igPhotoResponse = new UploadMediaResponse(httpHelper.PostRequest(url, photoByteInArray));
                    }


                    if (igPhotoResponse != null && igPhotoResponse.Success)
                    {
                        uploadIds.Add(uploadId);
                        ImageWidth.Add(image.Width);
                        ImageHeight.Add(image.Height);
                        requestParameters.Headers.Remove("X-Entity-Type");
                        requestParameters.Headers.Remove("X-Entity-Name");
                        requestParameters.Headers.Remove("X-Entity-Length");
                        requestParameters.Headers.Remove("Offset");
                        requestParameters.Headers.Remove("X-Instagram-Rupload-Params");
                        requestParameters.Headers.Remove("X_FB_PHOTO_WATERFALL_ID");
                        requestParameters.Headers.Remove("retry_context");
                    }
                }

                return ConfigurePhotoAlbum(dominatorAccountModel, _Account, token, uploadIds, ImageWidth, ImageHeight,
                    caption, tagLocation, lstTagUserIds);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public UploadMediaResponse ConfigurePhotoAlbum(DominatorAccountModel _dominatorAccount, AccountModel _Account,
            CancellationToken token, List<string> uploadIds, List<int> imageWidht, List<int> imageHeight,
            string caption, string tagLocation = null, List<string> lstTagUserIds = null)
        {
            token.ThrowIfCancellationRequested();
            try
            {
                var sidecarId = ((long)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds)
                    .ToString();

                var childrenArray = new JArray();
                var usertags = string.Empty;
                if (lstTagUserIds != null && lstTagUserIds.Count > 0)
                {
                    usertags = "{\"in\":[";
                    foreach (var tagUserId in lstTagUserIds)
                        if (!usertags.Contains("user_id"))
                            usertags += "{\"user_id\":" + tagUserId + ",\"position\":[0." +
                                        GdUtilities.GetRandomNumber(8) + ",0." + GdUtilities.GetRandomNumber(8) + "]}";
                        else
                            usertags += ",{\"user_id\":" + tagUserId + ",\"position\":[0." +
                                        GdUtilities.GetRandomNumber(8) + ",0." + GdUtilities.GetRandomNumber(8) + "]}";

                    usertags += "]}";
                }

                // foreach (string uploadId in uploadIds)
                for (var Id = 0; Id < uploadIds.Count; Id++)
                {
                    var edit = "{\"crop_original_size\":[" + imageWidht[Id] + "," + imageHeight[Id] +
                               "],\"crop_center\":[0.0,0.0],\"crop_zoom\":1}";
                    var device = "{\"manufacturer\":\"" + _dominatorAccount.DeviceDetails.Manufacturer +
                                 "\",\"model\":\"" + _dominatorAccount.DeviceDetails.Model +
                                 "\",\"android_version\":\"" + _dominatorAccount.DeviceDetails.AndroidVersion +
                                 "\",\"android_release\":\"" + _dominatorAccount.DeviceDetails.AndroidRelease + "\"}";
                    var extra = "{\"source_width\":" + imageWidht[Id] + ",\"source_height\":" + imageHeight[Id] + "}";

                    //need to work on it 
                    childrenArray.Add(new JObject
                    {
                        //{"scene_capture_type", "standard"},
                        //{"mas_opt_in", "NOT_PROMPTED"},
                        //{"camera_position", "unknown"},
                        //{"allow_multi_configures", false},
                        //{"geotag_enabled", false},
                        //{"disable_comments", false},
                        {"edits", edit},
                        {"device", device},
                        {"extra", extra},
                        {"source_type", 4},
                        {"upload_id", uploadIds[Id]},
                        {"usertags", usertags},
                        {"caption", null}
                    });
                }


                var jSonElements = new JsonElements
                {
                    Uuid = _Account.Uuid,
                    Uid = _dominatorAccount.AccountBaseModel.UserId,
                    Csrftoken = _Account.CsrfToken,
                    Caption = caption,
                    ClientSidecarId = sidecarId,
                    GeotagEnabled = false,
                    DisableComments = false,
                    ChildrenMetadata = childrenArray,
                };
                if (tagLocation != null) jSonElements.Location = tagLocation;

                var requestParameters = (IgRequestParameters)httpHelper.GetRequestParameter();

                requestParameters.Body = jSonElements;

                var url = Constants.ApiUrl + requestParameters.GenerateUrl("media/configure_sidecar/");

                var postData = requestParameters.GenerateBody();

                token.ThrowIfCancellationRequested();
                var albumResponse = new UploadMediaResponse(httpHelper.PostRequest(url, postData));
                if (!albumResponse.Success)
                {
                    delayService.ThreadSleep(5);
                    return new UploadMediaResponse(httpHelper.PostRequest(url, postData));
                }

                return albumResponse;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        #endregion

        #region Working Upload Photo Requests and Responses

        public string UPloadProfileInitialRequest(DominatorAccountModel _dominatorAccount, AccountModel _Account,
            string photo, ref string UploadId, ref string hashCode, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var igPhotoResponse = string.Empty;
            try
            {
                UploadId = ((long)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds).ToString();

                var profileImage = new ProfilePicture()
                {
                    upload_id = UploadId,
                    image_compression = "{\"lib_name\":\"moz\",\"lib_version\":\"3.1.m\",\"quality\":\"80\"}",
                    media_type = "1",
                };
                var ruploadparam = SerializeObject(profileImage);

                var requestParameters = (IgRequestParameters)httpHelper.GetRequestParameter();
                requestParameters.Headers.Remove("X-IG-App-Locale");
                requestParameters.Headers.Remove("X-IG-Device-Locale");
                requestParameters.Headers.Remove("X-IG-Mapped-Locale");
                requestParameters.Headers.Remove("X-Pigeon-Session-Id");
                requestParameters.Headers.Remove("X-Pigeon-Rawclienttime");
                requestParameters.Headers.Remove("X-IG-Connection-Speed");
                requestParameters.Headers.Remove("X-IG-Bandwidth-Speed-KBPS");
                requestParameters.Headers.Remove("X-IG-Bandwidth-TotalBytes-B");
                requestParameters.Headers.Remove("X-IG-Bandwidth-TotalTime-MS");
                requestParameters.Headers.Remove("X-Bloks-Version-Id");
                requestParameters.Headers.Remove("X-Bloks-Is-Layout-RTL");
                requestParameters.Headers.Remove("X-IG-Device-ID");
                requestParameters.Headers.Remove("X-IG-Android-ID");
                requestParameters.Headers["X-Instagram-Rupload-Params"] = ruploadparam;
                requestParameters.Headers.Add("X_FB_PHOTO_WATERFALL_ID", _Account.Uuid);
                //var authoBearer = httpHelper.Response.Headers["ig-set-authorization"];
                //var mid= httpHelper.Response.Headers["mid"];
                //requestParameters.Headers.Add("Authorization", authoBearer);
                // requestParameters.Headers.Add("X-MID",mid);
                requestParameters.Headers.Add("IG-U-IG-DIRECT-REGION-HINT", "PRN");
                requestParameters.Headers.Add("IG-U-SHBID", "11322");
                var Rawclienttime = GdUtilities.GetRowClientTime();
                requestParameters.Headers.Add("IG-U-SHBTS", Rawclienttime);
                requestParameters.Headers.Add("IG-U-DS-USER-ID", _dominatorAccount.AccountBaseModel.UserId);
                requestParameters.Headers.Add("IG-U-RUR", "ATN");
                hashCode = UploadId.GetHashCode().ToString();
                var url = $"https://i.instagram.com/rupload_igphoto/{UploadId}_0_{hashCode}";
                //byte[] photoByteInArray = File.ReadAllBytes(photo);
                //requestParameters.Headers.Add("X-Entity-Length", photoByteInArray.Length.ToString());
                igPhotoResponse = httpHelper.GetRequest(url).Response;
                requestParameters.Headers.Remove("IG-U-IG-DIRECT-REGION-HINT");
                requestParameters.Headers.Remove("IG-U-SHBID");
                requestParameters.Headers.Remove("IG-U-SHBTS");
                requestParameters.Headers.Remove("IG-U-DS-USER-ID");
                requestParameters.Headers.Remove("IG-U-RUR");
                requestParameters.Headers.Remove("X-Instagram-Rupload-Params");
                requestParameters.Headers.Remove("Authorization");
            }
            catch (Exception)
            {
            }

            return igPhotoResponse;
        }


        public UploadMediaResponse UploadProfilePicture(DominatorAccountModel _dominatorAccount, AccountModel _Account,
            string photo, ref string uploadId, ref string hashCode, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            UploadMediaResponse igPhotoResponse = null;
            try
            {
                uploadId = ((long)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds).ToString();

                var profilePicture = new ProfilePicture()
                {
                    upload_id = uploadId,
                    image_compression = "{\"lib_name\":\"moz\",\"lib_version\":\"3.1.m\",\"quality\":\"80\"}",
                    media_type = "1",
                };
                var ruploadparam = SerializeObject(profilePicture);

                var requestParameters = (IgRequestParameters)httpHelper.GetRequestParameter();

                requestParameters.Headers["X-Instagram-Rupload-Params"] = ruploadparam;
                requestParameters.Headers.Add("IG-U-SHBID", "11322");
                var Rawclienttime = GdUtilities.GetRowClientTime();
                requestParameters.Headers.Add("IG-U-SHBTS", Rawclienttime);
                requestParameters.Headers.Add("IG-U-DS-USER-ID", _dominatorAccount.AccountBaseModel.UserId);
                requestParameters.Headers.Add("IG-U-RUR", "ATN");
                requestParameters.Headers.Add("X-Entity-Type", "image/jpeg");
                requestParameters.Headers.Add("X-Entity-Name", uploadId);
                requestParameters.Headers.Add("Offset", "0");
                requestParameters.Headers["Authorization"] = _Account.AuthorizationHeader;
                requestParameters.ContentType = "application/octet-stream";
                var url = $"https://i.instagram.com/rupload_igphoto/{uploadId}_0_{hashCode}";
                var photoByteInArray = File.ReadAllBytes(photo);
                requestParameters.Headers.Add("X-Entity-Length", photoByteInArray.Length.ToString());
                igPhotoResponse = new UploadMediaResponse(httpHelper.PostRequest(url, photoByteInArray));
                requestParameters.Headers.Remove("IG-U-IG-DIRECT-REGION-HINT");
                requestParameters.Headers.Remove("IG-U-SHBID");
                requestParameters.Headers.Remove("IG-U-SHBTS");
                requestParameters.Headers.Remove("IG-U-DS-USER-ID");
                requestParameters.Headers.Remove("IG-U-RUR");
                requestParameters.Headers.Remove("X-Instagram-Rupload-Params");
                //requestParameters.Headers.Remove("Authorization");
            }
            catch (Exception)
            {
            }

            return igPhotoResponse;
        }

        public UploadMediaResponse UploadTimeLinePhoto(DominatorAccountModel _dominatorAccount, AccountModel _Account,
            CancellationToken token, string photo, List<string> lstTagUserIds, string uploadId = null,
            bool isSideCar = false, string caption = "", string tagLocation = null)
        {
            token.ThrowIfCancellationRequested();
            try
            {
                if (uploadId == null)
                    uploadId = _dateProvider.UtcNowUnix().ToString();
                //{"upload_id":"179209433549627","media_type":"1","retry_context":"{\"num_reupload\":0,\"num_step_auto_retry\":0,\"num_step_manual_retry\":0}",
                //"original_photo_pdq_hash":"47702328ad4e89ff43b23d808cdc42fd330bb94ac9b6673514cddcd9da317217:100",
                //"image_compression":"{\"lib_name\":\"moz\",\"lib_version\":\"3.1.m\",\"quality\":\"70\",\"ssim\":0.9892721176147461}","xsharing_user_ids":"[]"}

                var storyImagePostDetails = new StoryImagePostDetails()
                {
                    upload_id = uploadId,
                    image_compression = "{\"lib_name\":\"moz\",\"lib_version\":\"3.1.m\",\"quality\":\"84\",\"ssim\":0.980280339717865}",
                    retry_context = "{\"num_reupload\":0,\"num_step_auto_retry\":0,\"num_step_manual_retry\":0}",
                    media_type = "1",
                    //original_photo_pdq_hash = "47702328ad4e89ff43b23d808cdc42fd330bb94ac9b6673514cddcd9da317217:100",
                    xsharing_user_ids = new string[] { },
                };
                var ruploadparam = SerializeObject(storyImagePostDetails);

                var requestParameters = (IgRequestParameters)httpHelper.GetRequestParameter();
                requestParameters.Headers["X-Instagram-Rupload-Params"] = ruploadparam;
                requestParameters.Headers["X-IG-WWW-Claim"] = _Account.WwwClaim;
                requestParameters.Headers.Add("X_FB_PHOTO_WATERFALL_ID", _Account.Uuid);
                var url = $"https://i.instagram.com/rupload_igphoto/{uploadId}";
                //var response = httpHelper.GetRequest(url).Response;
                return UploadPhoto(_dominatorAccount, _Account, token, photo, lstTagUserIds, uploadId, false, caption,
                    tagLocation);

            }
            catch (AssertFailedException)
            {
                throw;
            }
            catch
            {
                return null;
            }
        }

        public UploadMediaResponse UploadPhoto(DominatorAccountModel _dominatorAccount, AccountModel _Account,
            CancellationToken token, string photo, List<string> lstTagUserIds, string uploadId = null,
            bool isSideCar = false, string caption = "", string tagLocation = null)
        {
            token.ThrowIfCancellationRequested();
            UploadMediaResponse igPhotoResponse = null;
            try
            {
                var requestParameters = (IgRequestParameters)httpHelper.GetRequestParameter();
                requestParameters.Headers["X-Entity-Type"] = "image/jpeg";
                requestParameters.Headers["X-Entity-Name"] = uploadId;
                requestParameters.Headers["Content-Type"] = "application/octet-stream";
                requestParameters.ContentType = "application/octet-stream";
                requestParameters.Headers["Offset"] = "0";
                requestParameters.Url = $"rupload_igphoto/{uploadId}";
                var url = $"https://i.instagram.com/rupload_igphoto/{uploadId}";
                var photoByteInArray = File.ReadAllBytes(photo);
                requestParameters.Headers["X-Entity-Length"] = photoByteInArray.Length.ToString();
                var auth = requestParameters.Headers["Authorization"];
                requestParameters.Headers["Authorization"] = auth;
                igPhotoResponse = new UploadMediaResponse(httpHelper.PostRequest(url, photoByteInArray));
                if (igPhotoResponse != null && igPhotoResponse.Success)
                {
                    requestParameters.Headers.Remove("X-Entity-Type");
                    requestParameters.Headers.Remove("X-Entity-Name");
                    requestParameters.Headers.Remove("X-Entity-Length");
                    requestParameters.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
                    requestParameters.Headers.Remove("Offset");
                    requestParameters.Headers.Remove("X-Instagram-Rupload-Params");
                    requestParameters.Headers.Remove("X_FB_PHOTO_WATERFALL_ID");
                    requestParameters.Headers.Remove("retry_context");
                    requestParameters.AddHeader("retry_context",
                        "{\"num_step_auto_retry\":0,\"num_reupload\":0,\"num_step_manual_retry\":0}");
                }

                return ConfigurePhoto(_dominatorAccount, _Account, token, photo, uploadId, lstTagUserIds, caption,
                    tagLocation);
            }
            catch (AssertFailedException)
            {
                throw;
            }
            catch
            {
                return null;
            }
        }

        public UploadMediaResponse Configure(DominatorAccountModel _dominatorAccount, AccountModel _Account,
            CancellationToken token, string photo, string uploadId, List<string> lstTagUserIds, string caption = "",
            string tagLocation = null)
        {
            token.ThrowIfCancellationRequested();
            try
            {
                UploadMediaResponse uploadMediaresponse;

                if (!File.Exists(photo))
                    return null;

                var image = Image.FromFile(photo);
                var jsonElements = new JsonElements()
                {
                    Csrftoken = _Account.CsrfToken,
                    MediaFolder = "Instagram",
                    SourceType = "4",
                    Uid = _dominatorAccount.AccountBaseModel.UserId,
                    Uuid = _Account.Uuid,
                    Caption = caption,
                    UploadId = uploadId,
                    Device = new JsonElements.DeviceJson()
                    {
                        AndroidRelease = _dominatorAccount.DeviceDetails.AndroidRelease,
                        AndroidVersion = int.Parse(_dominatorAccount.DeviceDetails.AndroidVersion),
                        Manufacturer = _dominatorAccount.DeviceDetails.Manufacturer,
                        Model = _dominatorAccount.DeviceDetails.Model
                    },
                    Edits = new JsonElements.EditsJson()
                    {
                        CropOriginalSize = new[] { image.Width, image.Height },
                        CropCenter = new[] { 0.0, 0.0 },
                        CropZoom = 1
                    },
                    Extra = new JsonElements.ExtraJson()
                    {
                        SourceHeight = image.Width,
                        SourceWidth = image.Height
                    }
                };
                if (!string.IsNullOrEmpty(tagLocation))
                    jsonElements.Location = tagLocation;


                if (lstTagUserIds != null && lstTagUserIds.Count > 0)
                {
                    jsonElements.Usertags = "{\"in\":[";
                    foreach (var tagUserId in lstTagUserIds)
                        if (!jsonElements.Usertags.Contains("user_id"))
                            jsonElements.Usertags += "{\"user_id\":" + tagUserId + ",\"position\":[0." +
                                                     GdUtilities.GetRandomNumber(8) + ",0." +
                                                     GdUtilities.GetRandomNumber(8) + "]}";
                        else
                            jsonElements.Usertags += ",{\"user_id\":" + tagUserId + ",\"position\":[0." +
                                                     GdUtilities.GetRandomNumber(8) + ",0." +
                                                     GdUtilities.GetRandomNumber(8) + "]}";

                    jsonElements.Usertags += "]}";
                }


                var requestParameters = (IgRequestParameters)httpHelper.GetRequestParameter();
                requestParameters.UrlParameters = new Dictionary<string, string>();
                requestParameters.PostDataParameters = new Dictionary<string, string>();
                requestParameters.Body = jsonElements;

                requestParameters.CreateSign();

                requestParameters.Url = "media/configure/";
                var url = requestParameters.GenerateUrl("media/configure/");
                url = Constants.ApiUrl + url;

                var postData = requestParameters.GenerateBody();
                //delayService.ThreadSleep(TimeSpan.FromSeconds(15));
                image = Image.FromFile(photo);
                image.Dispose();
                token.ThrowIfCancellationRequested();
                uploadMediaresponse = new UploadMediaResponse(httpHelper.PostRequest(url, postData));

                if (string.IsNullOrEmpty(uploadMediaresponse?.ToString()))
                {
                    delayService.ThreadSleep(TimeSpan.FromSeconds(10)); //Thread.Sleep(TimeSpan.FromSeconds(10));
                    uploadMediaresponse = new UploadMediaResponse(httpHelper.PostRequest(url, postData));
                }

                image.Dispose();
                return uploadMediaresponse;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        public UploadMediaResponse ConfigurePhoto(DominatorAccountModel _dominatorAccount, AccountModel _Account,
            CancellationToken token, string photo, string uploadId, List<string> lstTagUserIds, string caption = "",
            string tagLocation = null)
        {
            token.ThrowIfCancellationRequested();
            try
            {
                UploadMediaResponse uploadMediaresponse;

                if (!File.Exists(photo))
                    return null;

                using (var image = Image.FromFile(photo))
                {
                    var dateTime = DateTime.Now;
                    var jsonElements = new JsonElements()
                    {
                        //Csrftoken = _Account.CsrfToken,
                        MediaFolder = "Instagram",
                        SourceType = "4",
                        Uid = _dominatorAccount.AccountBaseModel.UserId,
                        Uuid = _Account.Uuid,
                        Caption = caption,
                        UploadId = uploadId,
                        DeviceId = _Account.Device_Id,
                        DateTimeDigitalized = Convert.ToString(dateTime).Replace('/', ':'), // "2019:10:03 11:25:32",
                        DateTimeOriginal = Convert.ToString(dateTime).Replace('/', ':'),
                        SceneCaptureType = "standard",
                        CameraModel = _dominatorAccount.DeviceDetails.Model,
                        CameraMake = _dominatorAccount.DeviceDetails.Manufacturer,
                        CreationLoggerSessionId = Utilities.GetGuid(),
                        Device = new JsonElements.DeviceJson()
                        {
                            AndroidRelease = _dominatorAccount.DeviceDetails.AndroidRelease,
                            AndroidVersion = int.Parse(_dominatorAccount.DeviceDetails.AndroidVersion),
                            Manufacturer = _dominatorAccount.DeviceDetails.Manufacturer,
                            Model = _dominatorAccount.DeviceDetails.Model
                        },
                        Edits = new JsonElements.EditsJson()
                        {
                            CropOriginalSize = new[] { image.Width, image.Height },
                            CropCenter = new[] { 0.0, 0.0 },
                            CropZoom = 1
                        },
                        Extra = new JsonElements.ExtraJson()
                        {
                            SourceHeight = image.Height,
                            SourceWidth = image.Width
                        }
                    };
                    if (!string.IsNullOrEmpty(tagLocation))
                        jsonElements.Location = tagLocation;

                    //Adding Tag user into post data
                    if (lstTagUserIds != null && lstTagUserIds.Count > 0)
                    {
                        jsonElements.Usertags = "{\"in\":[";
                        foreach (var tagUserId in lstTagUserIds)
                            if (!jsonElements.Usertags.Contains("user_id"))
                                jsonElements.Usertags += "{\"user_id\":" + tagUserId + ",\"position\":[0." +
                                                         GdUtilities.GetRandomNumber(8) + ",0." +
                                                         GdUtilities.GetRandomNumber(8) + "]}";
                            else
                                jsonElements.Usertags += ",{\"user_id\":" + tagUserId + ",\"position\":[0." +
                                                         GdUtilities.GetRandomNumber(8) + ",0." +
                                                         GdUtilities.GetRandomNumber(8) + "]}";

                        jsonElements.Usertags += "]}";
                    }

                    var requestParameters = (IgRequestParameters)httpHelper.GetRequestParameter();
                    requestParameters.UrlParameters = new Dictionary<string, string>();
                    requestParameters.PostDataParameters = new Dictionary<string, string>();
                    requestParameters.Body = jsonElements;

                    requestParameters.CreateSign();

                    requestParameters.Url = "media/configure/";
                    var url = requestParameters.GenerateUrl("media/configure/");
                    url = Constants.ApiUrl + url;

                    var postData = requestParameters.GenerateBody();
                    delayService.ThreadSleep(TimeSpan.FromSeconds(15));
                    token.ThrowIfCancellationRequested();
                    uploadMediaresponse = new UploadMediaResponse(httpHelper.PostRequest(url, postData));

                    if (string.IsNullOrEmpty(uploadMediaresponse.ToString()))
                    {
                        delayService.ThreadSleep(TimeSpan.FromSeconds(10));
                        uploadMediaresponse = new UploadMediaResponse(httpHelper.PostRequest(url, postData));
                    }

                    return uploadMediaresponse;
                }
            }
            catch (AssertFailedException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        public CommonIgResponseHandler SavePost(string mediaId, DominatorAccountModel _dominatorAccount,
            AccountModel _Account, CancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();
                var jsonElements = new JsonElements()
                {
                    ModuleName = "feed_timeline",
                    Csrftoken = _Account.CsrfToken,
                    RadioType = "mobile-hspa+",
                    Uuid = _Account.Uuid
                };
                var requestParameters = (IgRequestParameters)httpHelper.GetRequestParameter();
                requestParameters.UrlParameters = new Dictionary<string, string>();
                requestParameters.Body = jsonElements;
                var url =
                    requestParameters.GenerateUrl(
                        $"media/{(object)mediaId}_{_dominatorAccount.AccountBaseModel.UserId}/save/");
                url = Constants.ApiUrl + url;

                var postData = requestParameters.GenerateBody();
                token.ThrowIfCancellationRequested();
                return new CommonIgResponseHandler(httpHelper.PostRequest(url, postData));
            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion

        #region Upload video feature  

        public UploadMediaResponse UploadVideo(DominatorAccountModel _dominatorAccount, AccountModel accountModel,
            CancellationToken token, string videoFilePath, string thumbnailFilePath, string caption = "",
            string tagLocation = null, List<string> lstTagUserIds = null)
        {
            token.ThrowIfCancellationRequested();
            try
            {
                GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Instagram, _dominatorAccount.UserName,
                    "Publisher", "Please wait... started to publish video");
                UploadMediaResponse uploadMediaResponse = null;
                var uploadId =
                    ((long)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds).ToString();
                //FFProbe ffProbe = new FFProbe();
                //MediaInfo mediaInfo = ffProbe.GetMediaInfo(videoFilePath);
                var mediaInfo = GramStatic.GetMediaInfo(videoFilePath);
                if ((float)mediaInfo.Streams[0].Width / (float)mediaInfo.Streams[0].Height < 0.8)
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Instagram, _dominatorAccount.UserName,
                    "Publisher", "Unable to process reels video in post timeline");
                    return null;
                }
                uploadMediaResponse = IgVideo_uploadingVideo(_dominatorAccount, accountModel, token, videoFilePath,
                    thumbnailFilePath, uploadId, mediaInfo, false, "");



                if (uploadMediaResponse != null && uploadMediaResponse.Success)
                    uploadMediaResponse = igphoto_uploadingThumnail(_dominatorAccount, accountModel, token,
                        thumbnailFilePath, uploadId, mediaInfo, videoFilePath, false, caption, tagLocation,
                        lstTagUserIds);

                if (uploadMediaResponse != null && uploadMediaResponse.Success)
                    uploadMediaResponse = ConfigureVideo(_dominatorAccount, accountModel, token, videoFilePath,
                        thumbnailFilePath, uploadId, caption, tagLocation, lstTagUserIds);
                return uploadMediaResponse;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        public UploadMediaResponse ConfigureVideo(DominatorAccountModel _dominatorAccount, AccountModel _Account,
            CancellationToken token, string videoFilePath, string thumbnailFilePath, string uploadId,
            string caption = "", string tagLocation = null, List<string> lstTagUserIds = null)
        {
            token.ThrowIfCancellationRequested();
            try
            {
                //FFProbe ffProbe = new FFProbe();
                //MediaInfo mediaInfo = ffProbe.GetMediaInfo(videoFilePath);
                var mediaInfo = Utility.GramStatic.GetMediaInfo(videoFilePath);
                var jsonElements = new JsonElements()
                {
                    FilterType = "0",
                    TimezoneOffset = "19800",
                    Csrftoken = _Account.CsrfToken,
                    SourceType = "4",
                    VideoResult = "",
                    Uid = _dominatorAccount.AccountBaseModel.UserId,
                    Uuid = _Account.Uuid,
                    Caption = caption,
                    UploadId = uploadId,
                    DeviceId = _dominatorAccount.DeviceDetails.DeviceId,
                    Device = new JsonElements.DeviceJson()
                    {
                        AndroidRelease = _dominatorAccount.DeviceDetails.AndroidRelease,
                        AndroidVersion = int.Parse(_dominatorAccount.DeviceDetails.AndroidVersion),
                        Manufacturer = _dominatorAccount.DeviceDetails.Manufacturer,
                        Model = _dominatorAccount.DeviceDetails.Model
                    },

                    Clips = new[]{
                        new JsonElements.ClipJson()
                    {

                        Length = mediaInfo.Duration.TotalSeconds,
                        SourceType = "4",
                    }
                    },
                    Extra = new JsonElements.ExtraJson()
                    {
                        //SourceHeight = mediaInfo.Streams[0].Height,
                        SourceHeight = mediaInfo.Streams[0].Height,
                        SourceWidth = mediaInfo.Streams[0].Width
                    },
                    Length = mediaInfo.Duration.TotalSeconds,
                    AudioMuted = false,
                    PosterFrameIndex = 0
                };


                if (tagLocation != null) jsonElements.Location = tagLocation;

                if (lstTagUserIds != null && lstTagUserIds.Count > 0)
                {
                    jsonElements.Usertags = "{\"in\":[";
                    foreach (var tagUserId in lstTagUserIds)
                        if (!jsonElements.Usertags.Contains("user_id"))
                            jsonElements.Usertags += "{\"user_id\":" + tagUserId + ",\"position\":[0." +
                                                     GdUtilities.GetRandomNumber(8) + ",0." +
                                                     GdUtilities.GetRandomNumber(8) + "]}";
                        else
                            jsonElements.Usertags += ",{\"user_id\":" + tagUserId + ",\"position\":[0." +
                                                     GdUtilities.GetRandomNumber(8) + ",0." +
                                                     GdUtilities.GetRandomNumber(8) + "]}";

                    jsonElements.Usertags += "]}";
                }

                var requestParameters = (IgRequestParameters)httpHelper.GetRequestParameter();
                requestParameters.UrlParameters = new Dictionary<string, string>();
                requestParameters.PostDataParameters = new Dictionary<string, string>();

                requestParameters.Body = jsonElements;

                requestParameters.Url = "media/configure/?video=1";
                var url = requestParameters.GenerateUrl("media/configure/?video=1");
                url = Constants.ApiUrl + url;
                var finalUrl = Constants.ApiUrl + "media/upload_finish/?video=1";
                requestParameters.Headers["X-IG-WWW-Claim"] = _Account.WwwClaim;
                requestParameters.Headers["retry_context"] = "{\"num_reupload\":0,\"num_step_auto_retry\":0,\"num_step_manual_retry\":0}";
                requestParameters.Headers.Remove("Accept-Encoding");

                var regionHint = httpHelper.Response.Headers["ig-set-ig-u-ig-direct-region-hint"];
                var rur = httpHelper.Response.Headers["ig-set-ig-u-rur"];
                requestParameters.Headers["Priority"] = "u=3";
                if (!string.IsNullOrEmpty(rur))
                    requestParameters.Headers["IG-U-RUR"] = rur;
                requestParameters.Headers["X-FB-HTTP-Engine"] = "Liger";



                var postData = requestParameters.GenerateBody();
                token.ThrowIfCancellationRequested();
                var finishUpload = new UploadMediaResponse(httpHelper.PostRequest(finalUrl, postData));
                if (!finishUpload.Success)
                    return finishUpload;

                return new UploadMediaResponse(httpHelper.PostRequest(url, postData));
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        #endregion

        #region Post Video in Reels

        public UploadMediaResponse Configure_Reels_Video(DominatorAccountModel _dominatorAccount, AccountModel _Account,
            CancellationToken token, string videoFilePath, string thumbnailFilePath, string uploadId,
            string caption = "", string tagLocation = null, List<string> lstTagUserIds = null)
        {
            token.ThrowIfCancellationRequested();
            try
            {

                var mediaInfo = Utility.GramStatic.GetMediaInfo(videoFilePath);
                var jsonElements = new JsonElements()
                {
                    CameraSessionid = Utilities.GetGuid(),
                    FilterType = "0",
                    TimezoneOffset = "19800",
                    Csrftoken = _Account.CsrfToken,
                    SourceType = "4",
                    VideoResult = "",
                    Uid = _dominatorAccount.AccountBaseModel.UserId,
                    Uuid = _Account.Uuid,
                    Caption = caption,
                    capture_type = "clips_v2",
                    UploadId = uploadId,
                    DeviceId = _dominatorAccount.DeviceDetails.DeviceId,
                    Device = new JsonElements.DeviceJson()
                    {
                        AndroidRelease = _dominatorAccount.DeviceDetails.AndroidRelease,
                        AndroidVersion = int.Parse(_dominatorAccount.DeviceDetails.AndroidVersion),
                        Manufacturer = _dominatorAccount.DeviceDetails.Manufacturer,
                        Model = _dominatorAccount.DeviceDetails.Model
                    },

                    Clips = new[]{
                        new JsonElements.ClipJson()
                    {

                        Length = mediaInfo.Duration.TotalSeconds,
                        SourceType = "4",
                    }
                    },
                    Extra = new JsonElements.ExtraJson()
                    {
                        SourceHeight = mediaInfo.Streams[0].Height,
                        SourceWidth = mediaInfo.Streams[0].Width
                    },

                    AdditionalInfo = new JsonElements.AdditionalAudioInfo()
                    {
                        hasVoiceoverAttribution = "0"
                    },
                    ClipsSegmentMetadata = new JsonElements.ClipsSegmentsMetadataJson()
                    {
                        Numsegment = 1,
                        ClipsSegments = new[]
                       {
                          new JsonElements.ClipsSegmentsJson()
                          {
                              Index=0,
                              FaceEffectedid=null,
                              Speed=100,
                              Source="library",
                              DurationInMs=(mediaInfo.Duration.TotalMilliseconds).ToString(CultureInfo.InvariantCulture),
                              AudioType="original",
                              FromDraft="0",
                              CameraPosition=-1,
                              MediaFolder=new DirectoryInfo(System.IO.Path.GetDirectoryName(videoFilePath)).Name,
                              OriginalMediaType ="video",
                              Media_type="video"
                          }
                       }
                    },

                    Length = mediaInfo.Duration.TotalSeconds,
                    AudioMuted = false,
                    PosterFrameIndex = 0
                };

                #region future requirment 
                //if (tagLocation != null) jsonElements.Location = tagLocation;

                //if (lstTagUserIds != null && lstTagUserIds.Count > 0)
                //{
                //    jsonElements.Usertags = "{\"in\":[";
                //    foreach (var tagUserId in lstTagUserIds)
                //        if (!jsonElements.Usertags.Contains("user_id"))
                //            jsonElements.Usertags += "{\"user_id\":" + tagUserId + ",\"position\":[0." +
                //                                     GdUtilities.GetRandomNumber(8) + ",0." +
                //                                     GdUtilities.GetRandomNumber(8) + "]}";
                //        else
                //            jsonElements.Usertags += ",{\"user_id\":" + tagUserId + ",\"position\":[0." +
                //                                     GdUtilities.GetRandomNumber(8) + ",0." +
                //                                     GdUtilities.GetRandomNumber(8) + "]}";

                //    jsonElements.Usertags += "]}";
                //}
                #endregion
                var requestParameters = (IgRequestParameters)httpHelper.GetRequestParameter();
                requestParameters.UrlParameters = new Dictionary<string, string>();
                requestParameters.PostDataParameters = new Dictionary<string, string>();

                requestParameters.Body = jsonElements;
                requestParameters.Url = "media/configure_to_clips/?video=1";
                var url = requestParameters.GenerateUrl("media/configure_to_clips/?video=1");
                url = Constants.ApiUrl + url;

                requestParameters.Headers["X-IG-WWW-Claim"] = _Account.WwwClaim;
                requestParameters.Headers["retry_context"] = "{\"num_reupload\":0,\"num_step_auto_retry\":0,\"num_step_manual_retry\":0}";
                requestParameters.Headers.Remove("Accept-Encoding");
                requestParameters.Headers["Priority"] = "u=3";
                requestParameters.Headers.Add("X-Bloks-Is-Panorama-Enable", "true");
                requestParameters.Headers.Add("is_clips_video", "1");

                var postData = requestParameters.GenerateBody();
                token.ThrowIfCancellationRequested();
                return new UploadMediaResponse(httpHelper.PostRequest(url, postData));
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        #endregion


        public async Task<LogAttributionResponse> LoginAttributionAsync(DominatorAccountModel _dominatorAccount,
            AccountModel accountModel)
        {
            try
            {
                var jsonElements = new JsonElements()
                {
                    Adid = accountModel.AdId
                };

                var requestParameter = (IgRequestParameters)httpHelper.GetRequestParameter();
                requestParameter.PostDataParameters = new Dictionary<string, string>();
                requestParameter.Body = jsonElements;
                requestParameter.Url = "attribution/log_attribution/";
                var url = requestParameter.GenerateUrl("attribution/log_attribution/");
                url = Constants.ApiUrl + url;
                var postData = requestParameter.GenerateBody();

                return new LogAttributionResponse(await httpHelper.PostRequestAsync(url, postData, CancellationToken));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public async Task<CommonIgResponseHandler> TokenResult(DominatorAccountModel _dominatorAcount,
            AccountModel accountModel)
        {
            try
            {
                var url =
                    $"zr/token/result/?token_hash=&custom_device_id={accountModel.Id}&device_id={accountModel.Device_Id}&fetch_reason=token_expired";
                url = Constants.ApiUrl + url;
                return new CommonIgResponseHandler(await httpHelper.GetRequestAsync(url, CancellationToken));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public async Task<CommonIgResponseHandler> AccountContactPointPrefillAsync(
            DominatorAccountModel _dominatorAccount, AccountModel accountModel)
        {
            try
            {
                var jsonElements = new JsonElements
                {
                    PhoneId = accountModel.PhoneId,
                    Usage = "prefill"
                };

                var requestParameter =
                    (IgRequestParameters)httpHelper.GetRequestParameter();
                requestParameter.Body = jsonElements;
                requestParameter.PostDataParameters = new Dictionary<string, string>();
                // requestParameter.Headers.Add("Expect", "100-continue");
                // signed_body = SIGNATURE.{ "phone_id":"5219f45b-955c-4e97-941d-53de9f9f3df7","_csrftoken":"im2tYt38WL0g3zgWRZyGBZwt35IeOv8D","usage":"prefill"}

                //signed_body=77c09d1f23a542ecc935f58b882c63da883ba83fdb922e687e1998561c0d882e.{"_csrftoken":"uXT5CgGY3JcfU5qzIL0DdHKNarioiOHQ","phone_id":"efb3f15e-6c67-42e5-bfa5-73548d4f38ee",
                //"usage":"prefill"}&ig_sig_key_version=4

                //signed_body=SIGNATURE.{"_csrftoken":"im2tYt38WL0g3zgWRZyGBZwt35IeOv8D","id":"666e9e0a-d663-4693-9fab-a0dfd51cf921","server_config_retrieval":"1"}
                requestParameter.Url = "accounts/contact_point_prefill/";
                var url = requestParameter.GenerateUrl("accounts/contact_point_prefill/");
                url = Constants.ApiUrl + url;
                var postData = requestParameter.GenerateBody();
                var response =
                    new CommonIgResponseHandler(await httpHelper.PostRequestAsync(url, postData, CancellationToken));
                return response;
            }
            catch (Exception)
            {
                return null;
            }
        }
        //remove
        public MsisdnHeaderResponse ReadMsisdnHeader(AccountModel accountModel)
        {
            try
            {
                var jsonElements = new JsonElements()
                {
                    DeviceId = accountModel.Device_Id,
                    Csrftoken = _Account.CsrfToken,
                    MobileSubnoUsage = "default",
                };
                var requestParameter =
                    (IgRequestParameters)httpHelper.GetRequestParameter();
                requestParameter.PostDataParameters = new Dictionary<string, string>();

                requestParameter.Body = jsonElements;
                requestParameter.Url = "accounts/read_msisdn_header/";
                var url = requestParameter.GenerateUrl("accounts/read_msisdn_header/");
                url = Constants.ApiUrl + url;
                var postData = requestParameter.GenerateBody();

                return new MsisdnHeaderResponse(httpHelper.PostRequest(url, postData));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }
        //remove
        public async Task<MsisdnHeaderResponse> ReadMsisdnHeaderAsync(DominatorAccountModel _dominatorAccount,
            AccountModel accountModel)
        {
            try
            {
                var jsonElements = new JsonElements()
                {
                    DeviceId = accountModel.Uuid,
                    Csrftoken = "null",
                    MobileSubnoUsage = "default"
                };

                var requestParameter = (IgRequestParameters)httpHelper.GetRequestParameter();
                requestParameter.Headers.Add("X-DEVICE-ID", _dominatorAccount.DeviceDetails.Id);
                requestParameter.Body = jsonElements;
                requestParameter.Url = "accounts/read_msisdn_header/";
                var url = requestParameter.GenerateUrl("accounts/read_msisdn_header/");
                url = Constants.ApiUrl + url;
                var postData = requestParameter.GenerateBody();

                var response =
                    new MsisdnHeaderResponse(await httpHelper.PostRequestAsync(url, postData, CancellationToken));
                requestParameter.Headers.Remove("X-DEVICE-ID");
                accountModel.CsrfToken = httpHelper.GetRequestParameter().Cookies["csrftoken"].Value;
                return response;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public async Task<CommonIgResponseHandler> GetPrefillCandidatesBeforeLogin(
            DominatorAccountModel _dominatorAccount, AccountModel accountModel)
        {
            try
            {
                var jsonElements = new JsonElements()
                {
                    AndroidDeviceId = accountModel.Device_Id,
                    Usages = "[\"account_recovery_omnibox\"]",
                    DeviceId = accountModel.Id,
                    PhoneId = accountModel.PhoneId

                };
                var requestParameter =
                    (IgRequestParameters)httpHelper.GetRequestParameter();
                requestParameter.Headers["Content-Type"] = "application/x-www-form-urlencoded";
                requestParameter.PostDataParameters = new Dictionary<string, string>();
                requestParameter.Body = jsonElements;
                requestParameter.Url = "accounts/get_prefill_candidates/";
                var url = requestParameter.GenerateUrl("accounts/get_prefill_candidates/");
                url = Constants.Api_B_Url + url;
                requestParameter.CreateSign();
                var postData = requestParameter.GenerateBody();
                return new CommonIgResponseHandler(await httpHelper.PostRequestAsync(url, postData, CancellationToken));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }


        public async Task<CommonIgResponseHandler> B_ZrTokenAsyncBeforeLogin(DominatorAccountModel _dominatorAccount, AccountModel accountModel)
        {
            try
            {
                var jsonElements = new JsonElements()
                {
                    DeviceId = accountModel.Device_Id,
                    TokenHash = "",
                    CustomDeviceId = accountModel.PhoneId,
                    FetchReason = "token_expired"

                };
                var requestParameter = (IgRequestParameters)httpHelper.GetRequestParameter();
                requestParameter.Headers["Content-Type"] = "application/x-www-form-urlencoded; charset=UTF-8";
                requestParameter.PostDataParameters = new Dictionary<string, string>();
                requestParameter.Body = jsonElements;
                requestParameter.Url = "zr/tokens/";
                var url = requestParameter.GenerateUrl("zr/dual_tokens/");
                url = Constants.Api_B_Url + url;
                requestParameter.Headers.Remove("X-MID");
                requestParameter.DontSign();
                var postData = requestParameter.GenerateBody();
                return new CommonIgResponseHandler(await httpHelper.PostRequestAsync(url, postData, CancellationToken));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }


        public async Task<CommonIgResponseHandler> LauncherSyncAsyncBeforeLogin(DominatorAccountModel _dominatorAccount,
            AccountModel accountModel)
        {
            try
            {
                var jsonElements = new JsonElements()
                {
                    BoolOptPolicy = 0,
                    MobileConfigSessionless = "",
                    ApiVersion = 3,
                    UnitType = 1,
                    QueryHash = Constants.QueryHash,
                    DeviceId = accountModel.Device_Id,
                    FetchType = "ASYNC_FULL",
                    FamilyDeviceId = accountModel.FamilyId


                };
                //signed_body=SIGNATURE.{"_csrftoken":"im2tYt38WL0g3zgWRZyGBZwt35IeOv8D","id":"666e9e0a-d663-4693-9fab-a0dfd51cf921","server_config_retrieval":"1"}
                var requestParameter =
                    (IgRequestParameters)httpHelper.GetRequestParameter();
                requestParameter.PostDataParameters = new Dictionary<string, string>();
                requestParameter.Body = jsonElements;
                requestParameter.Url = "launcher/mobileconfig/";
                var url = requestParameter.GenerateUrl("launcher/mobileconfig/");
                url = Constants.Api_B_Url + url;
                var postData = requestParameter.GenerateBody();

                return new CommonIgResponseHandler(await httpHelper.PostRequestAsync(url, postData, CancellationToken));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        //remove
        public async Task<CommonIgResponseHandler> LauncherSyncAsyncAfterLogin(DominatorAccountModel _dominatorAccount,
            AccountModel accountModel)
        {
            try
            {
                var jsonElements = new JsonElements()
                {
                    Id = _dominatorAccount.AccountBaseModel.UserId,
                    Uid = _dominatorAccount.AccountBaseModel.UserId,
                    Uuid = accountModel.Uuid,
                    Configs = Constants.AfterLoginConfig
                };

                var requestParameter =
                    (IgRequestParameters)httpHelper.GetRequestParameter();
                requestParameter.PostDataParameters = new Dictionary<string, string>();
                requestParameter.Body = jsonElements;
                requestParameter.Url = "launcher/sync/";
                var url = requestParameter.GenerateUrl("launcher/sync/");
                url = Constants.ApiUrl + url;
                var postData = requestParameter.GenerateBody();

                return new CommonIgResponseHandler(await httpHelper.PostRequestAsync(url, postData, CancellationToken));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }
        //remove
        public async Task<SyncIgResponseHandler> SyncDeviceFeaturesAsyncBeforeLogin(DominatorAccountModel _dominatorAccountModel, AccountModel accountModel)
        {
            try
            {
                var jsonElements = new JsonElements()
                {
                    Id = accountModel.Id,
                    Experiments = Constants.BeforeLoginExperiments,
                    Csrftoken = accountModel.CsrfToken,
                    ServerConfigRetrieval = "1"
                };
                var requestParameter =
                    (IgRequestParameters)httpHelper.GetRequestParameter();
                requestParameter.PostDataParameters = new Dictionary<string, string>();
                requestParameter.Headers.Remove("Accept-Encoding");

                requestParameter.Body = jsonElements;
                requestParameter.Url = "qe/sync/";
                var url = requestParameter.GenerateUrl("qe/sync/");
                url = Constants.ApiUrl + url;
                var postData = requestParameter.GenerateBody();
                return new SyncIgResponseHandler(await httpHelper.PostRequestAsync(url, postData, CancellationToken));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }


        public async Task<SyncIgResponseHandler> B_SyncDeviceFeaturesAsyncBeforeLogin(
            DominatorAccountModel _dominatorAccountModel, AccountModel accountModel)
        {
            try
            {
                var ServerParams = new JsonElements
                {
                    LoggedOutUser = "",
                    FamilyDeviceId = accountModel.FamilyId,
                    DeviceId = accountModel.Device_Id,
                    OfflineExperimentGroup = Constants.OfflineExperimentGroup,
                    WaterfallId = accountModel.WaterfallId,
                    QplJoinId = Utilities.GetGuid(),
                    ShowInternalSettings = false,
                    QeDeviceId = Utilities.GetGuid(),
                    AccountList = new List<object>(),
                    BlockedUid = new List<object>(),
                    INTERNAL_INFRA_THEME = "harm_f"

                };
                var BkClientContext = new JsonElements
                {
                    BlockVersion = Constants.BlockVersionningId,
                    InstaStyIeId = Constants.InstaStyleId
                };
                var Params = new JsonElements
                {
                    ServerParams = ServerParams
                };

                var jsonElements = new JsonElements()
                {
                    Params = ServerParams,
                    BkClientContext = BkClientContext,
                    BlockVersioningId = Constants.BlockVersionningId
                };
                var requestParameter =
                    (IgRequestParameters)httpHelper.GetRequestParameter();
                requestParameter.PostDataParameters = new Dictionary<string, string>();
                requestParameter.Headers.Remove("Accept-Encoding");

                requestParameter.Body = jsonElements;
                requestParameter.Url = "bloks/apps/com.bloks.www.bloks.caa.login.process_client_data_and_redirect/";
                var url = requestParameter.GenerateUrl("bloks/apps/com.bloks.www.bloks.caa.login.process_client_data_and_redirect/");
                url = Constants.ApiUrl + url;
                requestParameter.DontSign();
                requestParameter.Headers["X-MID"] = _Account.MidHeader = httpHelper.Response?.Headers["ig-set-x-mid"];
                var postData = requestParameter.GenerateBody(true);
                requestParameter.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
                return new SyncIgResponseHandler(await httpHelper.PostRequestAsync(url, postData, CancellationToken));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        //remove
        public async Task<SyncIgResponseHandler> SyncDeviceFeaturesAsyncAfterLogin(
            DominatorAccountModel _dominatorAccountModel, AccountModel accountModel)
        {
            try
            {
                var jsonElements = new JsonElements()
                {
                    Id = _dominatorAccountModel.AccountBaseModel.UserId,
                    Csrftoken = accountModel.CsrfToken,
                    Uid = _dominatorAccountModel.AccountBaseModel.UserId,
                    Uuid = accountModel.Uuid,
                    Experiments = Constants.AfterLoginExperiments,
                };
                var requestParameter =
                    (IgRequestParameters)httpHelper.GetRequestParameter();
                requestParameter.PostDataParameters = new Dictionary<string, string>();
                requestParameter.Headers.Remove("Accept-Encoding");
                requestParameter.Body = jsonElements;
                requestParameter.Url = "qe/sync/";
                var url = requestParameter.GenerateUrl("qe/sync/");
                url = Constants.ApiUrl + url;
                var postData = requestParameter.GenerateBody();

                return new SyncIgResponseHandler(await httpHelper.PostRequestAsync(url, postData, CancellationToken));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        //remove
        public async Task<CommonIgResponseHandler> GetExtraCookiesResponse(DominatorAccountModel _dominatorAccountModel,
            AccountModel accountModel)
        {
            //https://i.instagram.com/api/v1/direct_v2/inbox/?visual_message_return_type=unseen&persistentBadging=true&limit=0

            try
            {
                var requestParameter =
                    (IgRequestParameters)httpHelper.GetRequestParameter();
                requestParameter.Url =
                    $"direct_v2/inbox/?visual_message_return_type=unseen&persistentBadging=true&limit=0";
                var url =
                    requestParameter.GenerateUrl(
                        $"direct_v2/inbox/?visual_message_return_type=unseen&persistentBadging=true&limit=0");
                url = Constants.ApiUrl + url;
                // byte[] postData = requestParameter.GenerateBody();

                return new CommonIgResponseHandler(await httpHelper.GetRequestAsync(url, CancellationToken));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        //remove
        public CommonIgResponseHandler HighlightsUser(DominatorAccountModel _dominatorAccountModel,
            AccountModel accountModel, string userId)
        {
            //https://i.instagram.com/api/v1/highlights/10881619368/highlights_tray/?supported_capabilities_new=[{"name":"SUPPORTED_SDK_VERSIONS","value":"13.0,14.0,15.0,16.0,17.0,18.0,19.0,20.0,21.0,22.0,23.0,24.0,25.0,26.0,27.0,28.0,29.0,30.0,31.0,32.0,33.0,34.0,35.0,36.0,37.0,38.0,39.0,40.0,41.0,42.0,43.0,44.0,45.0,46.0,47.0,48.0,49.0,50.0,51.0,52.0,53.0,54.0,55.0,56.0,57.0,58.0,59.0,60.0,61.0,62.0,63.0"},{"name":"FACE_TRACKER_VERSION","value":"12"},{"name":"COMPRESSION","value":"PVR_COMPRESSION"},{"name":"world_tracker","value":"world_tracker_enabled"},{"name":"gyroscope","value":"gyroscope_enabled"}]&phone_id=cc1a3085-5315-4019-8938-16a639f73f4c&battery_level=70&is_charging=0&will_sound_on=0
            try
            {
                var deviceSystem =
                    "[{\"name\":\"SUPPORTED_SDK_VERSIONS\",\"value\":\"119.0,120.0,121.0,122.0,123.0,124.0,125.0,126.0,127.0,128.0,129.0,130.0,131.0,132.0,133.0,134.0,135.0,136.0,137.0,138.0,139.0,140.0,141.0,142.0,143.0,144.0,145.0,146.0,147.0\"},{\"name\":\"FACE_TRACKER_VERSION\",\"value\":\"14\"},{\"name\":\"segmentation\",\"value\":\"segmentation_enabled\"},{\"name\":\"COMPRESSION\",\"value\":\"ETC2_COMPRESSION\"},{\"name\":\"world_tracker\",\"value\":\"world_tracker_enabled\"},{\"name\":\"gyroscope\",\"value\":\"gyroscope_enabled\"}]";
                var requestParameter =
                    (IgRequestParameters)httpHelper.GetRequestParameter();
                requestParameter.Url =
                    $"highlights/{userId}/highlights_tray/?supported_capabilities_new={deviceSystem}&phone_id={_dominatorAccount.DeviceDetails.PhoneId}&battery_level=70&is_charging=0&will_sound_on=0";
                var url = requestParameter.GenerateUrl(
                    $"highlights/{userId}/highlights_tray/?supported_capabilities_new={deviceSystem}&phone_id={_dominatorAccount.DeviceDetails.PhoneId}&battery_level=70&is_charging=0&will_sound_on=0");
                url = Constants.ApiUrl + url;
                return new CommonIgResponseHandler(httpHelper.GetRequest(url));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        //remove
        public CommonIgResponseHandler StoryHighlightUser(DominatorAccountModel _dominatorAccountModel,
            AccountModel accountModel, string userId)
        {
            try
            {
                var deviceSystem =
                    "[{\"name\":\"SUPPORTED_SDK_VERSIONS\",\"value\":\"119.0,120.0,121.0,122.0,123.0,124.0,125.0,126.0,127.0,128.0,129.0,130.0,131.0,132.0,133.0,134.0,135.0,136.0,137.0,138.0,139.0,140.0,141.0,142.0,143.0,144.0,145.0,146.0,147.0\"},{\"name\":\"FACE_TRACKER_VERSION\",\"value\":\"14\"},{\"name\":\"segmentation\",\"value\":\"segmentation_enabled\"},{\"name\":\"COMPRESSION\",\"value\":\"ETC2_COMPRESSION\"},{\"name\":\"world_tracker\",\"value\":\"world_tracker_enabled\"},{\"name\":\"gyroscope\",\"value\":\"gyroscope_enabled\"}]";
                var requestParameter =
                    (IgRequestParameters)httpHelper.GetRequestParameter();
                requestParameter.Url =
                    $"feed/user/10881619368/story/?supported_capabilities_new=={deviceSystem}&phone_id={_dominatorAccount.DeviceDetails.PhoneId}&battery_level=70&is_charging=0&will_sound_on=0";
                var url = requestParameter.GenerateUrl(
                    $"feed/user/10881619368/story/?supported_capabilities_new=={deviceSystem}&phone_id={_dominatorAccount.DeviceDetails.PhoneId}&battery_level=70&is_charging=0&will_sound_on=0");
                url = Constants.ApiUrl + url;
                // byte[] postData = requestParameter.GenerateBody();

                return new CommonIgResponseHandler(httpHelper.GetRequest(url));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        //remove
        public CommonIgResponseHandler Account_Recs(DominatorAccountModel _dominatorAccountModel,
            AccountModel accountModel, string userId)
        {
            try
            {
                var requestParameter =
                    (IgRequestParameters)httpHelper.GetRequestParameter();
                requestParameter.Url = $"fbsearch/accounts_recs/?surface=discover_page&target_user_id={userId}";
                var url =
                    requestParameter.GenerateUrl(
                        $"fbsearch/accounts_recs/?surface=discover_page&target_user_id={userId}");
                url = Constants.ApiUrl + url;
                return new CommonIgResponseHandler(httpHelper.GetRequest(url));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }
        public IGdHttpHelper GetGdHttpHelper()
        {
            return httpHelper;
        }

        public bool IsAlreadyFollowedByBrowser(string userName, DominatorAccountModel _dominatorAccount,
            ActivityType type, CancellationToken token)
        {
            return true;
        }

        public FollowerAndFollowingIgResponseHandler GetUserFollowersBrowser(DominatorAccountModel _dominatorAccount,
            string usernameId, CancellationToken token, string QueryType = null, string maxid = null)
        {
            return null;
        }

        public void CloseBrowser()
        {
        }

        #region GetProfileNotice
        //remove
        public async Task<ProfileNoticeResponse> GetProfileNotice(DominatorAccountModel dominatorAccountModle,
            AccountModel accounModel)
        {
            try
            {
                var jsonElements = new JsonElements()
                {
                    Uuid = accounModel.Uuid,
                    Uid = dominatorAccountModle.AccountBaseModel.UserId,
                    Csrftoken = accounModel.CsrfToken
                };

                var requestParameter =
                    (IgRequestParameters)httpHelper.GetRequestParameter();
                requestParameter.Body = jsonElements;
                requestParameter.Url = "users/profile_notice/";
                var url = requestParameter.GenerateUrl("users/profile_notice/");
                url = Constants.ApiUrl + url;
                var postData = requestParameter.GenerateBody();

                return new ProfileNoticeResponse(await httpHelper.PostRequestAsync(url, postData,dominatorAccountModle.Token));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        #endregion

        #region GetChatInfo

        //remove
        public async Task<ChatInfoIgResponseHandler> GetChatInfo(DominatorAccountModel dominatorAccountModel,
            AccountModel accountModel)
        {
            try
            {
                var requestParameter = (IgRequestParameters)httpHelper.GetRequestParameter();
                requestParameter.UrlParameters = new Dictionary<string, string>();

                requestParameter.AddUrlParameters("persistentBadging", "true");
                requestParameter.AddUrlParameters("use_unified_inbox", "true");

                requestParameter.Url = "direct_v2/inbox/";

                var url = Constants.ApiUrl + requestParameter.GenerateUrl();

                return new ChatInfoIgResponseHandler(await httpHelper.GetRequestAsync(url,dominatorAccountModel.Token));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        #endregion

        #region GetRecentActivity

        public async Task<ActivityNewsResponse> GetRecentActivity(DominatorAccountModel dominatorAccount)
        {
            try
            {
                var url = "news/inbox/";
                url = Constants.ApiUrl + url;

                return new ActivityNewsResponse(await httpHelper.GetRequestAsync(url,dominatorAccount.Token));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        #endregion

        public async Task<ReelsTrayFeedResponse> GetReelsTrayFeed(DominatorAccountModel dominatorAccountModel,
            AccountModel accountModel)
        {
            try
            {
                var support =
                    "[{\"name\":\"SUPPORTED_SDK_VERSIONS\",\"value\":\"119.0,120.0,121.0,122.0,123.0,124.0,125.0,126.0,127.0,128.0,129.0,130.0,131.0,132.0,133.0,134.0,135.0,136.0,137.0,138.0,139.0,140.0,141.0,142.0,143.0,144.0,145.0,146.0,147.0\"},{\"name\":\"FACE_TRACKER_VERSION\",\"value\":\"14\"},{\"name\":\"segmentation\",\"value\":\"segmentation_enabled\"},{\"name\":\"COMPRESSION\",\"value\":\"ETC2_COMPRESSION\"},{\"name\":\"world_tracker\",\"value\":\"world_tracker_enabled\"},{\"name\":\"gyroscope\",\"value\":\"gyroscope_enabled\"}]";
                var jsonElements = new JsonElements()
                {
                    SupportedCapabilitiesNew = support,
                    Uuid = accountModel.Uuid,
                    Reason = "cold_start",
                    RequestId = Guid.NewGuid().ToString(),
                    TimezoneOffset = "19800",
                    traySessionId = Guid.NewGuid().ToString(),
                    ReelTrayImpression = "{}",
                };

                var requestParameter =
                    (IgRequestParameters)httpHelper.GetRequestParameter();
                requestParameter.Body = jsonElements;
                var url = "feed/reels_tray/";
                url = Constants.ApiUrl + url;
                var postData = requestParameter.GenerateBody();

                return new ReelsTrayFeedResponse(await httpHelper.PostRequestAsync(url, postData, dominatorAccountModel.Token));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        //remove
        public async Task<CommonIgResponseHandler> SuggestedSearches(DominatorAccountModel dominatorAccountModel,
            AccountModel accountModel)
        {
            try
            {
                var url = Constants.ApiUrl + "fbsearch/suggested_searches/?type=users";
                return new CommonIgResponseHandler(await httpHelper.GetRequestAsync(url, dominatorAccountModel.Token));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        //remove
        public async Task<CommonIgResponseHandler> RecentSearches(DominatorAccountModel dominatorAccountModel,
            AccountModel accountModel)
        {
            try
            {
                var url = Constants.ApiUrl + "fbsearch/recent_searches/";
                return new CommonIgResponseHandler(await httpHelper.GetRequestAsync(url, dominatorAccountModel.Token));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }
        //remove
        public async Task<CommonIgResponseHandler> SuggestedSearches1(DominatorAccountModel dominatorAccountModel,
            AccountModel accountModel)
        {
            try
            {
                var url = Constants.ApiUrl + "fbsearch/suggested_searches/?type=blended";
                return new CommonIgResponseHandler(await httpHelper.GetRequestAsync(url, dominatorAccountModel.Token));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }
        //remove
        public async Task<CommonIgResponseHandler> ranked_recipients(DominatorAccountModel dominatorAccountModel,
            AccountModel accountModel)
        {
            try
            {
                var url = Constants.ApiUrl + "direct_v2/ranked_recipients/?mode=reshare&show_threads=True";
                return new CommonIgResponseHandler(await httpHelper.GetRequestAsync(url, dominatorAccountModel.Token));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }
        //remove
        public async Task<CommonIgResponseHandler> ranked_recipients_Mode_Raven(
            DominatorAccountModel dominatorAccountModel, AccountModel accountModel)
        {
            try
            {
                var url = Constants.ApiUrl + "direct_v2/ranked_recipients/?mode=raven&show_threads=True";
                return new CommonIgResponseHandler(await httpHelper.GetRequestAsync(url, dominatorAccountModel.Token));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }
        //remove
        public async Task<CommonIgResponseHandler> PersistentBadging(DominatorAccountModel dominatorAccountModel,
            AccountModel accountModel)
        {
            try
            {
                var url = Constants.ApiUrl + "direct_v2/inbox/?persistentBadging=true&use_unified_inbox=true";

                return new CommonIgResponseHandler(await httpHelper.GetRequestAsync(url, dominatorAccountModel.Token));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }
        //remove
        public async Task<CommonIgResponseHandler> GetPresence(DominatorAccountModel dominatorAccountModel,
            AccountModel accountModel)
        {
            try
            {
                var url = Constants.ApiUrl + "direct_v2/get_presence/";

                return new CommonIgResponseHandler(await httpHelper.GetRequestAsync(url, dominatorAccountModel.Token));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }
        //remove
        public async Task<CommonIgResponseHandler> topical_explore(DominatorAccountModel dominatorAccountModel,
            AccountModel accountModel)
        {
            try
            {
                var url = Constants.ApiUrl +
                          $"discover/topical_explore/?is_prefetch=True&use_sectional_payload=true&has_title=false&session_id={dominatorAccountModel.SessionId}&is_adjacent_prefetch=false";

                return new CommonIgResponseHandler(await httpHelper.GetRequestAsync(url, dominatorAccountModel.Token));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public async Task<CommonIgResponseHandler> Scores(DominatorAccountModel dominatorAccountModel,
            AccountModel accountModel)
        {
            try
            {
                var url = Constants.ApiUrl +
                          $"scores/bootstrap/users/?surfaces=[\"autocomplete_user_list\",\"coefficient_besties_list_ranking\",\"coefficient_rank_recipient_user_suggestion\",\"coefficient_ios_section_test_bootstrap_ranking\",\"coefficient_direct_recipients_ranking_variant_2\"]";
                return new CommonIgResponseHandler(await httpHelper.GetRequestAsync(url, dominatorAccountModel.Token));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public async Task<CommonIgResponseHandler> GetLinkageStatus(DominatorAccountModel dominatorAccountModel,
            AccountModel accountModel)
        {
            try
            {
                var url = Constants.ApiUrl + "linked_accounts/get_linkage_status/";
                return new CommonIgResponseHandler(await httpHelper.GetRequestAsync(url, dominatorAccountModel.Token));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }
        //remove
        public async Task<CommonIgResponseHandler> PushRegister(DominatorAccountModel dominatorAccountModel,
            AccountModel accountModel)
        {
            try
            {
                var check = RandomString(18);
                var check1 = RandomString(28);
                CommonIgResponseHandler response = null;
                var PostUrl =
                    $"device_type=android_mqtt&is_main_push_channel=true&device_sub_type=2&device_token=%257B%2522k%2522%253A%2522eyJwbiI6ImNvbS5pbnN0YWdyYW0uYW5kcm9pZCIsImRpIjoi{check}00{check1}IiwiYWkiOiI1NjcwNjczNDMzNTI0MjciLCJjayI6IjEyNDAyNDU3NDI4NzQxNCJ9%2522%252C%2522v%2522%253A0%252C%2522t%2522%253A%2522fbns-b64%2522%257D&_csrftoken={accountModel.CsrfToken}&guid={accountModel.Guid}&_uuid={accountModel.Guid}&users={dominatorAccountModel.AccountBaseModel.UserId}&family_device_id={accountModel.FamilyId}";
                var requestParameter = (IgRequestParameters)httpHelper.GetRequestParameter();
                var url = Constants.ApiUrl + "push/register/";
                var postData = Encoding.ASCII.GetBytes(PostUrl);
                response = new CommonIgResponseHandler(await httpHelper.PostRequestAsync(url, postData, dominatorAccountModel.Token));
                requestParameter.CreateSign();
                return response;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public async Task<FriendshipsResponse> RemoveFollowers(DominatorAccountModel dominatorAccountModel,
            AccountModel accountModel, string userId, CancellationToken token, string Username = "")
        {
            FriendshipsResponse friendshipsResponse = null;
            try
            {
                #region OLD RemoveFollowers Code
                //var jsonElements = new JsonElements()
                //{
                //    //Csrftoken = accountModel.CsrfToken,
                //    UserId = userId,
                //    RadioType = "wifi-none",
                //    Uid = dominatorAccountModel.AccountBaseModel.UserId,
                //    Uuid = accountModel.Uuid,
                //};
                //var requestParameters =
                //    (IgRequestParameters)httpHelper.GetRequestParameter();
                //requestParameters.Headers["X-IG-WWW-Claim"] = _Account.WwwClaim;
                //requestParameters.Body = jsonElements;
                //requestParameters.Url = $"friendships/remove_follower/{userId}/";
                //var url = requestParameters.GenerateUrl($"friendships/remove_follower/{userId}/");
                //url = Constants.ApiUrl + url;
                //var postData = requestParameters.GenerateBody();
                //response = new FriendshipsResponse(httpHelper.PostRequest(url, postData));
                #endregion

                var param = GetWebParameter(dominatorAccountModel);
                var url = $"https://www.instagram.com/api/v1/web/friendships/{userId}/remove_follower/";
                using (var client = new HttpClient(param.httpClient))
                {
                    // Add headers
                    client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/137.0.0.0 Safari/537.36");
                    client.DefaultRequestHeaders.Add("X-IG-App-ID", "936619743392459");
                    client.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
                    client.DefaultRequestHeaders.Add("X-Instagram-AJAX", GramStatic.InstagramAjax());
                    client.DefaultRequestHeaders.Add("X-CSRFToken", param.CsrfToken);
                    //client.DefaultRequestHeaders.Add("X-Web-Session-ID", "qzlrgt:ugopeh:f20mea");
                    //client.DefaultRequestHeaders.Add("X-ASBD-ID", "359341");
                    client.DefaultRequestHeaders.Add("X-IG-WWW-Claim",param.X_IG_Claim);
                    client.DefaultRequestHeaders.Add("Accept-Encoding", GramStatic.AcceptEncoding);
                    client.DefaultRequestHeaders.Referrer = new Uri($"https://www.instagram.com/{Username}/followers/");

                    // POST request with empty body
                    var response = await client.PostAsync(url, new StringContent(""));

                    var responseBody = await response.Content.ReadAsStreamAsync();
                    var responseText = HttpHelper.Decode(responseBody, string.Join(",", response.Content.Headers.ContentEncoding));
                    friendshipsResponse = new FriendshipsResponse(responseText);
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return friendshipsResponse;
        }

        public string RandomString(int size)
        {
            var random = new Random();
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var randomString = new string(Enumerable.Repeat(chars, size)
                .Select(s => s[random.Next(s.Length)]).ToArray());
            var firstchar = randomString.ToCharArray();
            var first = firstchar[0].ToString().ToUpper();
            randomString = randomString.ReplaceAt(0, 1, first);
            return randomString;
        }


        #region UsersFriendship

        public FriendshipsShowManyResponse ShowManyUserFriends(DominatorAccountModel _dominatorAccountModel,
            AccountModel accountModel, List<string> userList)
        {
            try
            {
                if (userList == null || userList.Count == 0)
                    throw new ArgumentNullException($"{(object)nameof(userList)} is empty");
                var jsonElements = new JsonElements()
                {
                    Uuid = _Account.Uuid,
                    Csrftoken = _Account.CsrfToken,
                    // UserIds = string.Join(",", userList)//Pending
                };

                var requestParameters =
                    (IgRequestParameters)httpHelper.GetRequestParameter();
                requestParameters.Body = jsonElements;

                requestParameters.Url = "friendships/show_many/";
                var url = requestParameters.GenerateUrl("friendships/show_many/");
                url = Constants.ApiUrl + url;
                requestParameters.DontSign();
                var postData = requestParameters.GenerateBody();

                var friendsResponse = new FriendshipsShowManyResponse(httpHelper.PostRequest(url, postData));
                requestParameters.CreateSign();
                return friendsResponse;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        #endregion

        #region GetBlockedMedia

        public async Task<BlockedMediaResponse> GetBlockedMedia(DominatorAccountModel dominatorAccountModel,
            AccountModel accountModel)
        {
            try
            {

                var requestParameter = (IgRequestParameters)httpHelper.GetRequestParameter();
                var url = "media/blocked/";
                url = Constants.ApiUrl + url;
                return new BlockedMediaResponse(await httpHelper.GetRequestAsync(url, dominatorAccountModel.Token));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        #endregion


        public async Task<CommonIgResponseHandler> ReplyComment(DominatorAccountModel dominatorAccountModel,
            AccountModel accountModel, string CommentId, string comment, string mediaId, CancellationToken token, string Username = "")
        {
            CommonIgResponseHandler response = null;
            try
            {
                #region OLD ReplyComment Code
                //var jsonElements = new JsonElements()
                //{
                //    UserBreadcrumb = InstagramHelper.GenerateUserBreadcrumb(comment.Length),
                //    IdempotenceToken = StringHelper.GenerateGuid(),
                //    Uuid = accountModel.Uuid,
                //    Uid = dominatorAccountModel.AccountBaseModel.UserId,
                //    Csrftoken = accountModel.CsrfToken,
                //    CommentText = comment,
                //    Containermodule = "comments_v2_feed_timeline",
                //    RadioType = "wifi-none",
                //    DeviceId = accountModel.Device_Id,
                //    RepliedToCommentId = CommentId,
                //};
                //var requestParameters = (IgRequestParameters)httpHelper.GetRequestParameter();
                //requestParameters.Body = jsonElements;

                //requestParameters.Url = $"media/{mediaId}/comment/";
                //var url = requestParameters.GenerateUrl($"media/{mediaId}/comment/");
                //url = Constants.ApiUrl + url;
                //var postData = requestParameters.GenerateBody();
                //response = new CommonIgResponseHandler(httpHelper.PostRequest(url, postData));
                #endregion

                var MediaID = GramStatic.GetCodeFromIDOrUrl(mediaId);
                var url = $"https://www.instagram.com/api/v1/web/comments/{MediaID}/add/";
                var param = GetWebParameter(dominatorAccountModel);
                var TaggedUsername = string.IsNullOrEmpty(Username) ? string.Empty : $"\r\n@{Username} ";
                using (var client = new HttpClient(param.httpClient))
                {
                    var baseUri = new Uri("https://www.instagram.com");
                    // ✅ POST data
                    var postData = $"comment_text={Uri.EscapeDataString($"{TaggedUsername}{comment}")}&replied_to_comment_id={CommentId}&jazoest={GramStatic.CreateJazoest(dominatorAccountModel?.DeviceDetails?.PhoneId)}";
                    var content = new StringContent(postData, Encoding.UTF8, "application/x-www-form-urlencoded");

                    // ✅ Required Headers
                    client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/137.0.0.0 Safari/537.36");
                    client.DefaultRequestHeaders.Add("X-CSRFToken",param.CsrfToken);
                    client.DefaultRequestHeaders.Add("X-IG-App-ID", "936619743392459");
                    client.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
                    client.DefaultRequestHeaders.Add("X-Instagram-AJAX", GramStatic.InstagramAjax());
                    //client.DefaultRequestHeaders.Add("X-Web-Session-ID", "9z0lzh:faqbmw:u44yaq");
                    //client.DefaultRequestHeaders.Add("X-ASBD-ID", "359341");
                    if(string.IsNullOrEmpty(param.X_IG_Claim))
                        client.DefaultRequestHeaders.Add("X-IG-WWW-Claim",param.X_IG_Claim);
                    client.DefaultRequestHeaders.Add("Accept-Encoding", GramStatic.AcceptEncoding);
                    client.DefaultRequestHeaders.Referrer = new Uri($"https://www.instagram.com/p/{mediaId}/c/{CommentId}/");
                    client.DefaultRequestHeaders.Add("Accept", "*/*");

                    // ✅ Send POST request
                    var response1 = await client.PostAsync(url, content);
                    var responseBody = await response1.Content.ReadAsStreamAsync();
                    var ResponseText = HttpHelper.Decode(responseBody, string.Join(",", response1.Content.Headers.ContentEncoding));
                    response = new CommonIgResponseHandler(ResponseText);
                }

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return response;
        }

        public UploadMediaResponse SendVideoMessage(DominatorAccountModel _dominatorAccount, AccountModel _Account,
            string mediaPath, string thumbnailFilePath, string userId, CancellationToken token)
        {
            GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Instagram, _dominatorAccount.UserName,
                "Publisher", "Please wait... started to sending video");
            token.ThrowIfCancellationRequested();
            UploadMediaResponse uploadMediaResponse = null;
            var videoPostDetails = new SendVideoMessage();
            var uploadId = ((long)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds).ToString();
            //FFProbe ffProbe = new FFProbe();
            //MediaInfo mediaInfo = ffProbe.GetMediaInfo(mediaPath);
            var mediaInfo = Utility.GramStatic.GetMediaInfo(mediaPath);
            try
            {
                videoPostDetails = new SendVideoMessage()
                {
                    retry_context = "{\"num_step_auto_retry\":0,\"num_reupload\":0,\"num_step_manual_retry\":0}",
                    upload_media_width = mediaInfo.Streams[0].Width.ToString(),
                    media_type = "2",
                    xsharing_user_ids = new string[] { },
                    upload_media_duration_ms =
                        (mediaInfo.Duration.TotalMilliseconds * 1000).ToString(CultureInfo.InvariantCulture),
                    upload_media_height = mediaInfo.Streams[0].Height.ToString(),
                    upload_id = uploadId,
                    direct_v2 = "1",
                };
                var hasCode = uploadId.GetHashCode();
                var ruploadparam = string.Empty;
                ruploadparam = SerializeObject(videoPostDetails);

                var requestParameters = (IgRequestParameters)httpHelper.GetRequestParameter();
                requestParameters.Headers.Add("X_FB_VIDEO_WATERFALL_ID", _Account.Uuid);
                requestParameters.Headers["X-Instagram-Rupload-Params"] = ruploadparam;
                requestParameters.Headers["Connection"] = "Keep-Alive";
                requestParameters.Url = $"rupload_igvideo/{uploadId}";

                var url = requestParameters.GenerateUrl($"rupload_igvideo/{uploadId}_0_{hasCode}");
                url = "https://i.instagram.com/" + url;
                token.ThrowIfCancellationRequested();
                var response = httpHelper.GetRequest(url).Response;
                if (!string.IsNullOrEmpty(response))
                    uploadMediaResponse = IgVideo_PostVideo(_dominatorAccount, _Account, uploadId, mediaInfo, token,
                        mediaPath, "", hasCode, "", null, null);

                if (uploadMediaResponse != null && uploadMediaResponse.Success)
                    uploadMediaResponse = uploadingThumnailForSendingVideoMessage(_dominatorAccount, _Account,
                        thumbnailFilePath, uploadId, userId, token);
            }
            catch (Exception)
            {
                //ignored
            }

            return uploadMediaResponse;
        }

        public UploadMediaResponse uploadingThumnailForSendingVideoMessage(DominatorAccountModel dominatorAccountModel,
            AccountModel accountModel, string thumbnailFilePath, string uploadId, string userId,
            CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var VideoImagePostDetails = new SendVideoThumbnailMessage()
            {
                upload_id = uploadId,
                image_compression = "{\"lib_name\":\"moz\",\"lib_version\":\"3.1.m\",\"quality\":\"80\"}",
                retry_context = "{\"num_step_auto_retry\":0,\"num_reupload\":0,\"num_step_manual_retry\":0}",
                media_type = "1",
                xsharing_user_ids = new string[] { },
            };

            var ruploadparam = string.Empty;
            ruploadparam = SerializeObject(VideoImagePostDetails);

            var requestParameters = (IgRequestParameters)httpHelper.GetRequestParameter();
            requestParameters.Headers["X-Instagram-Rupload-Params"] = ruploadparam;
            requestParameters.Headers["X_FB_PHOTO_WATERFALL_ID"] = _dominatorAccount.DeviceDetails.Guid;
            requestParameters.Headers["X-Entity-Type"] = "image / jpeg";
            requestParameters.Headers["X-Entity-Name"] = $"{uploadId}";
            requestParameters.Headers["Offset"] = "0";
            requestParameters.Url = $"rupload_igphoto/{uploadId}";
            var url = requestParameters.GenerateUrl($"rupload_igphoto/{uploadId}_0_471181928");
            url = "https://i.instagram.com/" + url;
            var photoByteInArray = File.ReadAllBytes(thumbnailFilePath);
            requestParameters.Headers["X-Entity-Length"] = photoByteInArray.Length.ToString();
            token.ThrowIfCancellationRequested();
            var uploadMediaResponse = new UploadMediaResponse(httpHelper.PostRequest(url, photoByteInArray));
            if (uploadMediaResponse != null && uploadMediaResponse.Success)
            {
                requestParameters.Headers.Remove("X-Entity-Type");
                requestParameters.Headers.Remove("X-Entity-Name");
                requestParameters.Headers.Remove("Content-Type");
                requestParameters.Headers.Remove("Offset");
                requestParameters.Headers.Remove("X-Entity-Length");
                requestParameters.Headers.Remove("X_FB_PHOTO_WATERFALL_ID");
                requestParameters.Headers.Remove("X-Instagram-Rupload-Params");
                uploadMediaResponse =
                    ConfigureBroadcastuploadingThumnail(dominatorAccountModel, accountModel, uploadId, userId, token);
            }

            return uploadMediaResponse;
        }

        public UploadMediaResponse ConfigureBroadcastuploadingThumnail(DominatorAccountModel dominatorAccountModel,
            AccountModel accountModel, string uploadId, string userId, CancellationToken token)
        {
            UploadMediaResponse uploadMediaResponse = null;
            var clientContext = Utilities.GetGuid();
            var jsonElements = new JsonElements()
            {
                UploadId = uploadId,
                Action = "send_item",
                DeviceId = accountModel.Device_Id,
                Csrftoken = accountModel.CsrfToken,
                ClientContext = clientContext,
                OfflineThreadingId = clientContext,
                Uuid = accountModel.Uuid,
                Sampled = "True",
                RecipientUsers = $"[[{userId}]]",
                VideoResult = null
            };
            var requestParameters =
                (IgRequestParameters)httpHelper.GetRequestParameter();
            requestParameters.Body = jsonElements;
            //https://i.instagram.com/api/v1/direct_v2/threads/broadcast/configure_video/
            var url = requestParameters.GenerateUrl($"direct_v2/threads/broadcast/configure_video/");
            url = Constants.ApiUrl + url;
            var postData = requestParameters.GenerateBody();
            uploadMediaResponse = new UploadMediaResponse(httpHelper.PostRequest(url, postData));
            return uploadMediaResponse;
        }

        public CaptchaRequestResponseHandler SubmittingCaptchaSolution(DominatorAccountModel _dominatorAccount, AccountModel accountModel, string captchaResponse, string apiUrl, CancellationToken token)
        {
            CaptchaRequestResponseHandler response = null;
            try
            {
                string postUrl = $"g-recaptcha-response={captchaResponse}";
                var requestParameter = (IgRequestParameters)httpHelper.GetRequestParameter();

                requestParameter.AddHeader("X-Instagram-AJAX", GramStatic.InstagramAjax());
                requestParameter.AddHeader("X-CSRFToken", accountModel.CsrfToken);
                requestParameter.Referer = apiUrl;
                requestParameter.AddHeader("X-IG-App-ID", "1217981644879628");
                requestParameter.Headers["X-Requested-With"] = "XMLHttpRequest";
                requestParameter.Headers["Content-Type"] = "application/x-www-form-urlencoded";
                requestParameter.Accept = "*/*";

                requestParameter.Headers["X-IG-WWW-Claim"] = "0";
                requestParameter.Headers["Origin"] = "https://i.instagram.com";
                requestParameter.Headers["Sec-Fetch-Site"] = "same-origin";
                requestParameter.Headers["Sec-Fetch-Mode"] = "cors";
                requestParameter.Headers["Sec-Fetch-Dest"] = "empty";


                requestParameter.Headers.Remove("Sec-Fetch-User");
                requestParameter.Headers.Remove("Upgrade-Insecure-Requests");
                requestParameter.UserAgent = $"Mozilla/5.0 (Linux; Android 10; SM-M317F Build/QP1A.190711.020; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/87.0.4280.101 Mobile Safari/537.36 {_dominatorAccount.UserAgentMobile}";
                var url = apiUrl;
                var postData = Encoding.ASCII.GetBytes(postUrl);
                var resp = httpHelper.PostRequest(url, postData);
                response = new CaptchaRequestResponseHandler(resp);
                requestParameter.CreateSign();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return response;
        }

        public SubmitPhoneNumberAfterCaptchaResponseHandler ConfirmPhoneNumberRequest(DominatorAccountModel _dominatorAccount, AccountModel accountModel, string chellagenContext, string PhoneNumber, string apiUrl, CancellationToken token)
        {
            SubmitPhoneNumberAfterCaptchaResponseHandler response = null;
            try
            {
                string postUrl = $"phone_number={PhoneNumber}&challenge_context={chellagenContext}";
                var requestParameter = (IgRequestParameters)httpHelper.GetRequestParameter();
                requestParameter.Headers["X-IG-App-ID"] = "1217981644879628";
                requestParameter.Headers["Sec-Fetch-Site"] = "same-origin";
                requestParameter.Headers["Sec-Fetch-Mode"] = "cors";
                requestParameter.Headers["Sec-Fetch-Dest"] = "empty";
                requestParameter.Headers["X-Requested-With"] = "XMLHttpRequest";
                requestParameter.Headers["Accept-Language"] = "en-US,q=0.9,en;q=0.8";
                requestParameter.Headers["Content-Type"] = "application/x-www-form-urlencoded";
                requestParameter.Headers["X-Instagram-AJAX"] = GramStatic.InstagramAjax();
                requestParameter.Headers["X-CSRFToken"] = accountModel.CsrfToken;
                requestParameter.Referer = apiUrl;
                // requestParameter.Headers["X -IG-WWW-Claim"] = "0";
                requestParameter.Headers["Origin"] = "https://i.instagram.com";
                requestParameter.UserAgent = $"Mozilla/5.0 (Linux; Android 10; SM-M317F Build/QP1A.190711.020; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/87.0.4280.101 Mobile Safari/537.36 {_dominatorAccount.UserAgentMobile}";
                var url = apiUrl;
                var postData = Encoding.ASCII.GetBytes(postUrl);
                var resp = httpHelper.PostRequest(url, postData);
                response = new SubmitPhoneNumberAfterCaptchaResponseHandler(resp);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            // requestParameter.CreateSign();
            return response;
        }
        public SubmitPhoneCodeForCaptchaResponseHandler SubmitPhoneNumberRequest(DominatorAccountModel _dominatorAccount, AccountModel accountModel, string chellagenContext, string otp, string apiUrl, CancellationToken token)
        {
            SubmitPhoneCodeForCaptchaResponseHandler response = null;
            try
            {
                string postUrl = $"security_code={otp}&challenge_context={chellagenContext}";
                var requestParameter = (IgRequestParameters)httpHelper.GetRequestParameter();
                requestParameter.Headers["X-IG-App-ID"] = "1217981644879628";
                requestParameter.Headers["Sec-Fetch-Site"] = "same-origin";
                requestParameter.Headers["Sec-Fetch-Mode"] = "cors";
                requestParameter.Headers["Sec-Fetch-Dest"] = "empty";
                requestParameter.Headers["X-Requested-With"] = "XMLHttpRequest";
                requestParameter.Headers["Accept-Language"] = "en-US,q=0.9,en;q=0.8";
                requestParameter.Headers["Content-Type"] = "application/x-www-form-urlencoded";
                requestParameter.Headers["X-Instagram-AJAX"] = GramStatic.InstagramAjax();
                requestParameter.Headers["X-CSRFToken"] = accountModel.CsrfToken;
                requestParameter.Referer = apiUrl;
                // requestParameter.Headers["X -IG-WWW-Claim"] = "0";
                requestParameter.Headers["Origin"] = "https://i.instagram.com";
                requestParameter.UserAgent = $"Mozilla/5.0 (Linux; Android 10; SM-M317F Build/QP1A.190711.020; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/87.0.4280.101 Mobile Safari/537.36 {_dominatorAccount.UserAgentMobile}";
                var url = apiUrl;
                var postData = Encoding.ASCII.GetBytes(postUrl);
                var resp = httpHelper.PostRequest(url, postData);
                response = new SubmitPhoneCodeForCaptchaResponseHandler(resp);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            // requestParameter.CreateSign();
            return response;
        }
        
        public UploadMediaResponse UploadingVideoWeb(DominatorAccountModel _dominatorAccount,
            AccountModel accountModel, CancellationToken token, string videoFilePath, string thumbnailFilePath,
            string uploadId, MediaInfo mediaInfo, bool isVideoAlbum = false, string caption = "",
            string tagLocation = null, List<string> lstTagUserIds = null)
        {
            UploadMediaResponse uploadMediaResponse = null;
            uploadId = ((long)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds)
                        .ToString();
            token.ThrowIfCancellationRequested();
            try
            {
                
                //{"client-passthrough":"1","is_clips_video":"1","is_sidecar":"0","media_type":2,"for_album":false,"video_format":"","upload_id":"1693000645404","upload_media_duration_ms":16740,"upload_media_height":1920,"upload_media_width":1080,"video_transform":null,"video_edit_params":{"crop_height":1080,"crop_width":1080,"crop_x1":0,"crop_y1":420,"mute":false,"trim_end":16.740998,"trim_start":0}}
                var videoPostDetails = new WebVideoPostDetails()
                {
                    clientpassthrough = "1",
                    is_clips_video = "1",
                    is_sidecar = "0",
                    media_type = 2,
                    for_album = false,
                    video_format = "",
                    upload_id = uploadId,
                    upload_media_duration_ms = Convert.ToInt32(mediaInfo.Duration.TotalMilliseconds),
                    upload_media_height = mediaInfo.Streams[0].Height,
                    upload_media_width = mediaInfo.Streams[0].Width,
                    video_transform = null,
                    video_edit_params = new VideoEditParams
                    {
                        crop_height=1080,
                        crop_width=1080,
                        crop_y1=420,
                        mute=false,
                        trim_end= mediaInfo.Duration.TotalMilliseconds/1000,
                        trim_start=0
                    }
                };

                var hasCode = uploadId.GetHashCode();
                var ruploadparam = string.Empty;
                ruploadparam = SerializeObject(videoPostDetails);


                //--------Get video post
                var requestParameters = (IgRequestParameters)httpHelper.GetRequestParameter();
                requestParameters = new IgRequestParameters("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/116.0.0.0 Safari/537.36");
                
                requestParameters.Cookies = _dominatorAccount.Cookies;
                requestParameters.Headers["X-IG-App-ID"] = "936619743392459";
                requestParameters.Headers["X-ASBD-ID"] = "129477";
                requestParameters.Url = $"rupload_igvideo/{uploadId}";

                var url = requestParameters.GenerateUrl($"rupload_igvideo/{uploadId}");
                url = "https://i.instagram.com/" + url;

                var response = httpHelper.GetRequest(url,requestParameters);
                token.ThrowIfCancellationRequested();

                //-------Upload Video Post
                requestParameters.Headers["X-Entity-Name"] = $"fb_uploader_{uploadId}";
                requestParameters.Headers["X-Instagram-AJAX"] = GramStatic.InstagramAjax();
                requestParameters.Headers["X-Instagram-Rupload-Params"] = ruploadparam;
                requestParameters.Headers["Offset"] = "0";
                requestParameters.Headers["Content_Type"] = "application/x-www-form-urlencoded";
                requestParameters.Url = $"rupload_igvideo/{uploadId}";
                url = requestParameters.GenerateUrl($"rupload_igvideo/fb_uploader_{uploadId}");
                url = "https://i.instagram.com/" + url;
                var photoByteInArray = File.ReadAllBytes(videoFilePath);
                requestParameters.Headers["X-Entity-Length"] = photoByteInArray.Length.ToString();
                token.ThrowIfCancellationRequested();
                uploadMediaResponse = new UploadMediaResponse(httpHelper.PostRequest(url, photoByteInArray));
                requestParameters = new IgRequestParameters("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/116.0.0.0 Safari/537.36");

                //-------------Upload Thumnail-------------


                token.ThrowIfCancellationRequested();
                UploadMediaResponse igPhotoResponse = null;
                try
                {
                    requestParameters = (IgRequestParameters)httpHelper.GetRequestParameter();
                    requestParameters = new IgRequestParameters("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/116.0.0.0 Safari/537.36");
                    requestParameters.Cookies = _dominatorAccount.Cookies;
                    requestParameters.Headers["X-IG-App-ID"] = "936619743392459";
                    requestParameters.Headers["X-ASBD-ID"] = "129477";
                    requestParameters.Headers["X-Entity-Type"] = "image/jpeg";
                    requestParameters.Headers["X-Entity-Name"] = uploadId;
                    requestParameters.Headers["Content-Type"] = "application/octet-stream";
                    requestParameters.ContentType = "application/octet-stream";
                    requestParameters.Headers["Offset"] = "0";
                    requestParameters.Url = $"rupload_igphoto/{uploadId}";
                    url = $"https://i.instagram.com/rupload_igphoto/{uploadId}";
                    photoByteInArray = File.ReadAllBytes(thumbnailFilePath);
                    requestParameters.Headers["X-Entity-Length"] = photoByteInArray.Length.ToString();
                    var auth = requestParameters.Headers["Authorization"];
                    requestParameters.Headers["Authorization"] = auth;
                    igPhotoResponse = new UploadMediaResponse(httpHelper.PostRequest(url, photoByteInArray));
                    if (igPhotoResponse != null && igPhotoResponse.Success)
                    {
                        requestParameters.Headers.Remove("X-Entity-Type");
                        requestParameters.Headers.Remove("X-Entity-Name");
                        requestParameters.Headers.Remove("X-Entity-Length");
                        requestParameters.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
                        requestParameters.Headers.Remove("Offset");
                        requestParameters.Headers.Remove("X-Instagram-Rupload-Params");
                        requestParameters.Headers.Remove("X_FB_PHOTO_WATERFALL_ID");
                        requestParameters.Headers.Remove("retry_context");
                        requestParameters.AddHeader("retry_context",
                            "{\"num_step_auto_retry\":0,\"num_reupload\":0,\"num_step_manual_retry\":0}");
                    }
                    var configResponse =  ConfigureWeb(_dominatorAccount,_Account,token,videoFilePath,thumbnailFilePath,uploadId,caption,tagLocation,lstTagUserIds);
                    
                }
                catch (AssertFailedException)
                {
                    throw;
                }
                catch
                {
                    return null;
                }
            }
            catch (Exception)
            {
                //ignored
            }

            return uploadMediaResponse;
        }

        public UploadMediaResponse ConfigureWeb(DominatorAccountModel _dominatorAccount, AccountModel _Account,
            CancellationToken token, string videoFilePath, string thumbnailFilePath, string uploadId,
            string caption = "", string tagLocation = null, List<string> lstTagUserIds = null)
        {
            token.ThrowIfCancellationRequested();
            try
            {
                var csrf = "";
                foreach(Cookie item in _dominatorAccount.Cookies)
                {
                    if (item.Name == "csrftoken")
                    {
                        csrf = item.Value;
                        break;
                    }
                        
                }
                //FFProbe ffProbe = new FFProbe();
                //MediaInfo mediaInfo = ffProbe.GetMediaInfo(videoFilePath);
                var mediaInfo = Utility.GramStatic.GetMediaInfo(videoFilePath);
                //source_type=library&caption=hello&upload_id=1693000645404&disable_comments=0
                //&like_and_view_counts_disabled=0&igtv_share_preview_to_feed=1&is_unified_
                //video_subtitles_enabled=0&clips_share_preview_to_feed=1&disable_oa_reuse=false

                string postDataString = $"source_type=library&caption={caption}&upload_id={uploadId}&disable_comments=0&" +
                    $"like_and_view_counts_disabled=0&igtv_share_preview_to_feed=1&is_unified_video=1&" +
                    "video_subtitles_enabled=0&clips_share_preview_to_feed=1&disable_oa_reuse=false";

                var requestParameters = (IgRequestParameters)httpHelper.GetRequestParameter();
                requestParameters = new IgRequestParameters("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/116.0.0.0 Safari/537.36");
                requestParameters.Cookies = _dominatorAccount.Cookies;
                requestParameters.Headers["X-IG-App-ID"] = "936619743392459";
                requestParameters.Headers["X-ASBD-ID"] = "129477";
                requestParameters.Headers["dpr"] = "1.25";
                requestParameters.Headers["sec-ch-ua-full-version-list"] = "\"Chromium\";v=\"116.0.5845.111\", \"Not)A;Brand\";v=\"24.0.0.0\", \"Google Chrome\";v=\"116.0.5845.111\"";
                requestParameters.Headers["X-Requested-With"] = "XMLHttpRequest";
                requestParameters.Headers["X-CSRFToken"] = csrf;
                requestParameters.Headers["X-Instagram-AJAX"] = GramStatic.InstagramAjax();
                requestParameters.Headers["sec-ch-prefers-color-scheme"] = "light";
                requestParameters.Url = "media/configure_to_clip/";
                var url = requestParameters.GenerateUrl("media/configure_to_clip/");
                url = Constants.ApiUrl + url;
                var finalUrl = Constants.ApiUrl + "media/upload_finish/?video=1";
                



                var postData = Encoding.UTF8.GetBytes(postDataString);
                token.ThrowIfCancellationRequested();
                var finishUpload = new UploadMediaResponse(httpHelper.PostRequest(finalUrl, postData));
                if (!finishUpload.Success)
                    return finishUpload;

                return new UploadMediaResponse(httpHelper.PostRequest(url, postData));
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        public async Task<ThreadIDDetails> GetThreadID(DominatorAccountModel dominatorAccountModel, string userId, string profileId, bool IsBrowser = true)
        {
            try
            {
                //var url = $"https://i.instagram.com/api/v1/direct_v2/threads/get_by_participants/?recipient_users=%5B{userId}%5D&seq_id=168&limit=20";
                //var param = GetWebParameter(dominatorAccountModel,true);
                //using (var client = new HttpClient())
                //{
                //    // Default headers
                //    client.DefaultRequestHeaders.UserAgent.ParseAdd(dominatorAccountModel?.DeviceDetails?.Useragent ?? "Instagram 283.0.0.20.105 Android (31/12; 320dpi; 720x1470; vivo; V2029; 2027; qcom; en_US; 475221264)");
                //    client.DefaultRequestHeaders.Add("X-IG-App-Locale", "en_US");
                //    client.DefaultRequestHeaders.Add("X-IG-Device-Locale", "en_US");
                //    client.DefaultRequestHeaders.Add("X-IG-Mapped-Locale", "en_US");
                //    client.DefaultRequestHeaders.Add("X-Pigeon-Session-Id", "UFS-"+(_dominatorAccount?.DeviceDetails?.PhoneId ?? Guid.NewGuid().ToString())+"-0");
                //    client.DefaultRequestHeaders.Add("X-Pigeon-Rawclienttime",GdUtilities.GetRowClientTime());
                //    //client.DefaultRequestHeaders.Add("X-IG-Bandwidth-Speed-KBPS", "90.000");
                //    //client.DefaultRequestHeaders.Add("X-IG-Bandwidth-TotalBytes-B", "1354473");
                //    //client.DefaultRequestHeaders.Add("X-IG-Bandwidth-TotalTime-MS", "13818");
                //    client.DefaultRequestHeaders.Add("X-Bloks-Version-Id", "f5fbf62cc3c51dc0e6f4ffd3a79e0c5929ae0b8af58c54acd1e186871a92fb27");
                //    client.DefaultRequestHeaders.Add("X-IG-WWW-Claim", param.X_IG_Claim);
                //    client.DefaultRequestHeaders.Add("X-Bloks-Is-Layout-RTL", "false");
                //    client.DefaultRequestHeaders.Add("X-IG-Device-ID",dominatorAccountModel?.DeviceDetails?.PhoneId ?? "cba40baf-8663-4ee8-a0bb-74dc5d2e0a98");
                //    client.DefaultRequestHeaders.Add("X-IG-Family-Device-ID", dominatorAccountModel?.DeviceDetails?.FamilyId ?? "6a1184d2-87c2-454f-93c5-a491e0040ad7");
                //    client.DefaultRequestHeaders.Add("X-IG-Android-ID",dominatorAccountModel?.DeviceDetails?.DeviceId ?? "android-b10069c5ba7bbd58");
                //    client.DefaultRequestHeaders.Add("X-IG-Timezone-Offset", "19800");
                //    //client.DefaultRequestHeaders.Add("X-IG-Nav-Chain", "MainFeedFragment:feed_timeline:1:cold_start:1750586717.156:10#230#301:3650792978401944102,UserDetailFragment:profile:2:media_owner:1750586730.819::,ProfileMediaTabFragment:profile:3:button:1750586736.397::,DirectThreadFragment:direct_thread:4:message_button:1750586738.130::,DirectThreadFragment:direct_thread:5:button:1750586738.135::");
                //    //client.DefaultRequestHeaders.Add("X-IG-CLIENT-ENDPOINT", "DirectThreadFragment:direct_thread");
                //    //client.DefaultRequestHeaders.Add("X-IG-SALT-LOGGER-IDS", "974456048,25624577,31784972,248448614,861807764,61669377,42991645,25952257,42991646,61669378,861798404,25101347,974462634");
                //    client.DefaultRequestHeaders.Add("X-FB-Connection-Type", "WIFI");
                //    client.DefaultRequestHeaders.Add("X-IG-Connection-Type", "WIFI");
                //    client.DefaultRequestHeaders.Add("X-IG-Capabilities", "3brTv10=");
                //    client.DefaultRequestHeaders.Add("X-IG-App-ID", "567067343352427");
                //    client.DefaultRequestHeaders.Add("Priority", "u=3");
                //    client.DefaultRequestHeaders.Add("Accept-Language", "en-US");
                //    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",param.Authorization);
                //    client.DefaultRequestHeaders.Add("X-MID", param.MID);
                //    client.DefaultRequestHeaders.Add("IG-U-DS-USER-ID", param.DsUserId);
                //    //client.DefaultRequestHeaders.Add("IG-U-RUR", "CLN,73228986484,1782122736:01fe7a70d6c112c914905f8d3e58b3f53438c55fb1129480fc97f1ca3195932912fc3143");
                //    client.DefaultRequestHeaders.Add("IG-INTENDED-USER-ID",param.DsUserId);

                //    // Send the GET request
                //    var response3 = await client.GetAsync(url);
                //    var ResponseText = await HttpHelper.Decode(response3);
                //    if(ResponseText != null && !string.IsNullOrEmpty(ResponseText?.Response)
                //        && ResponseText.Response.Contains(""))
                //    {
                //        threadID = Utilities.GetBetween(ResponseText?.Response, "\"thread_id\":\"", "\"");
                //        if (!string.IsNullOrEmpty(threadID))
                //            return threadID;
                //    }
                //}
                var reqParam = httpHelper.GetRequestParameter();
                var param = GetWebParameter(dominatorAccountModel);
                reqParam.Headers.Clear();
                reqParam.Cookies = dominatorAccountModel.Cookies;
                reqParam.KeepAlive = true;
                reqParam.Headers["sec-ch-ua-full-version-list"] = "\"(Not(A:Brand\";v=\"99.0.0.0\", \"Google Chrome\";v=\"132\", \"Chromium\";v=\"132\"";
                reqParam.Headers["sec-ch-ua-platform"] = "\"Windows\"";
                reqParam.Headers["sec-ch-ua"] = "\"(Not(A:Brand\";v=\"99\", \"Google Chrome\";v=\"132\", \"Chromium\";v=\"132\"";
                reqParam.Headers["sec-ch-ua-model"] = "\"\"";
                reqParam.Headers["sec-ch-ua-mobile"] = "?0";
                reqParam.Headers["X-Requested-With"] = "XMLHttpRequest";
                reqParam.Headers["X-IG-App-ID"] = "936619743392459";
                reqParam.Headers["X-ASBD-ID"] = "129477";
                reqParam.Headers["X-IG-WWW-Claim"] = param.X_IG_Claim;
                reqParam.ContentType = "application/x-www-form-urlencoded";
                reqParam.Headers["X-CSRFToken"] = param.CsrfToken;
                reqParam.Accept = "*/*";
                reqParam.UserAgent = "Mozilla/5.0 (Windows NT 6.2; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/132.0.6817.66 Safari/537.36";
                reqParam.Headers["Origin"] = "https://www.instagram.com";
                reqParam.Headers["Sec-Fetch-Site"] = "same-origin";
                reqParam.Headers["Sec-Fetch-Mode"] = "cors";
                reqParam.Headers["Sec-Fetch-Dest"] = "empty";
                reqParam.Referer = $"https://www.instagram.com/{profileId}";
                reqParam.Headers["Accept-Language"] = "en-GB,en-US;q=0.9,en;q=0.8";
                reqParam.Headers["Accept-Encoding"] = GramStatic.AcceptEncoding;
                httpHelper.SetRequestParameter(reqParam);
                var body = $"recipient_users=%5B%22{userId}%22%5D";
                var response = await httpHelper.PostRequestAsync("https://www.instagram.com/api/v1/direct_v2/create_group_thread/", Encoding.UTF8.GetBytes(body),dominatorAccountModel.Token);
                return new ThreadIDDetails(response?.Response, IsBrowser);
            }
            catch { return new ThreadIDDetails(); }
        }
        public void SetRequestParameter(DominatorAccountModel dominatorAccount, bool IsJson = false, string encoding = "gzip", bool IsPost = false, Dictionary<string, string> ExtraParam = null)
        {
            var igMobile = IGMobileUtilities.Instance(dominatorAccount);
            var requestParam = httpHelper.GetRequestParameter();
            var UserID = dominatorAccount.AccountBaseModel.UserId;
            #region Test Headers.

            requestParam.Headers.Clear();
            requestParam.UserAgent = "Instagram 283.0.0.20.105 Android (31/12; 320dpi; 720x1470; vivo; V2029; 2027; qcom; en_US; 475221264)";
            requestParam.Headers["X-IG-App-Locale"] = "en_US";
            requestParam.Headers["X-IG-Device-Locale"] = "en_US";
            requestParam.Headers["X-IG-Mapped-Locale"] = "en_US";
            requestParam.Headers["X-Pigeon-Session-Id"] = "UFS-38ce0c6d-8e1e-4314-8181-ac4dd2507f30-6";//UFS-b708c450-3042-44f1-9038-2966d7ba3d8c-6
            requestParam.Headers["X-Pigeon-Rawclienttime"] = igMobile.GetRawClientTime();
            requestParam.Headers["X-IG-Bandwidth-Speed-KBPS"] = "930.000";
            requestParam.Headers["X-IG-Bandwidth-TotalBytes-B"] = "8214605";
            requestParam.Headers["X-IG-Bandwidth-TotalTime-MS"] = "10408";
            requestParam.Headers["X-Bloks-Version-Id"] = "f5fbf62cc3c51dc0e6f4ffd3a79e0c5929ae0b8af58c54acd1e186871a92fb27";
            requestParam.Headers["X-IG-WWW-Claim"] = dominatorAccount.DeviceDetails.IGXClaim;//"hmac.AR1iNm7SsvL7FCYXKHndMHZcGcMkjJojnlA0z92V-RGby5Kr";
            requestParam.Headers["X-Bloks-Is-Layout-RTL"] = "false";
            requestParam.Headers["X-IG-Device-ID"] = "25dacd1a-d663-4bab-8eac-df97de764262";
            requestParam.Headers["X-IG-Family-Device-ID"] = "afaa3371-9b98-4a45-a491-44dfa6850913";
            requestParam.Headers["X-IG-Android-ID"] = "android-b10069c5ba7bbd58";
            requestParam.Headers["X-IG-Timezone-Offset"] = igMobile.GetTimeZoneOffSet();
            //requestParam.Headers["X-IG-Nav-Chain"] = "SelfFragment:self_profile:5:main_profile:1726106734.551::,ProfileMediaTabFragment:self_profile:6:button:1726106734.912::,com.instagram.portable_settings.settings:com.instagram.portable_settings.settings:19:button:1726107588.77::";
            requestParam.Headers["X-FB-Connection-Type"] = "WIFI";
            requestParam.Headers["X-IG-Connection-Type"] = "WIFI";
            requestParam.Headers["X-IG-Capabilities"] = "3brTv10=";
            requestParam.Headers["X-IG-App-ID"] = "567067343352427";
            requestParam.Headers["Priority"] = "u=3";
            requestParam.Headers["Accept-Language"] = "en-US";
            requestParam.Headers["Authorization"] = $"Bearer IGT:2:{igMobile.GetBearer()}";
            //requestParam.Headers["Authorization"] = $"Bearer IGT:2:eyJkc191c2VyX2lkIjoiNjg4Mzk4NDcyMzAiLCJzZXNzaW9uaWQiOiI2ODgzOTg0NzIzMCUzQTZyWnhQeVFxQ1dSc2pqJTNBMSUzQUFZZmU1NC14Q1lhbktHbTlqWTZzWGxYYXk3eTdTWEM1QlN6VVhsRFROZyJ9";
            requestParam.Headers["X-MID"] = "ZuJKIAABAAFmgv17h9hs1_Vxvrlu";
            //requestParam.Headers["IG-U-IG-DIRECT-REGION-HINT"] = $"RVA,{UserID},1757643814:01f765e62516f73b6867e15202c9d19f419aa75dde3468f2a096db840d6d72efb5fec774";
            //requestParam.Headers["IG-U-SHBID"] = igMobile.GetSHBID(UserID);
            //requestParam.Headers["IG-U-SHBTS"] = igMobile.GetSHBTS(UserID);
            requestParam.Headers["IG-U-DS-USER-ID"] = UserID;
            //requestParam.Headers["IG-U-RUR"] = igMobile.GetRUR(UserID);
            requestParam.Headers["IG-U-RUR"] = $"CLN,{UserID},{igMobile.GetTimeStamp()}:01f701f6b27d2bc650e5b16a0b630a7d3370cc31313501feb0fa2c223e874f04b86f9fbe";
            requestParam.Headers["IG-INTENDED-USER-ID"] = UserID;
            if (IsPost)
                requestParam.ContentType = IsJson ? "application/json" : "application/x-www-form-urlencoded; charset=UTF-8";
            //requestParam.Headers["Accept-Encoding"] = encoding;
            requestParam.Headers["Accept-Encoding"] = GramStatic.AcceptEncoding;
            requestParam.KeepAlive = true;
            requestParam.Cookies = new CookieCollection();
            if (ExtraParam != null && ExtraParam.Count > 0)
            {
                foreach (var data in ExtraParam)
                    requestParam.Headers[data.Key] = data.Value;
            }
            #endregion
            if (string.IsNullOrEmpty(dominatorAccount.AccountBaseModel.UserId))
                dominatorAccount.AccountBaseModel.UserId = igMobile.GetUserID();
            try
            {
                if (dominatorAccount.AccountBaseModel.AccountProxy != null)
                {
                    var proxy = dominatorAccount.AccountBaseModel.AccountProxy;
                    if (!string.IsNullOrEmpty(proxy.ProxyIp) && !string.IsNullOrEmpty(proxy.ProxyPort))
                    {
                        requestParam.Proxy = new Proxy
                        {
                            ProxyIp = proxy?.ProxyIp,
                            ProxyPort = proxy?.ProxyPort,
                            ProxyUsername = proxy?.ProxyUsername,
                            ProxyPassword = proxy?.ProxyPassword,
                        };
                    }
                }
            }
            catch { }
            httpHelper.SetRequestParameter(requestParam);
        }
        public async Task<UploadMediaResponse> UploadMediaHttp(DominatorAccountModel dominatorAccountModel, InstagramPost instagramPost, List<string> mediaList, CancellationToken token)
        {
            UploadMediaResponse uploadMediaResponse = null;
            try
            {
                instagramPost.MediaType = MediaType.Video;
                var uploadID = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
                var mediaType = instagramPost.MediaType == MediaType.Video ? 2 :instagramPost.MediaType == MediaType.Image? 1 :instagramPost.MediaType == MediaType.Album?8:0;
                var type = mediaType == 2 ? "reels" : "p";
                var param = GetWebParameter(dominatorAccountModel);
                //var cropped = CropToInstagramPortraitWithPadding(mediaList.FirstOrDefault());
                var items = GetCropRatio(instagramPost, mediaList.FirstOrDefault());
                using (var client = new HttpClient(param.httpClient))
                {
                    var videoBytes = File.ReadAllBytes(mediaList.FirstOrDefault());
                    var content = new ByteArrayContent(videoBytes);
                    var request4 = new HttpRequestMessage(HttpMethod.Post,$"https://www.instagram.com/rupload_igvideo/fb_uploader_{uploadID}")
                    {
                        Content = content
                    };
                    request4.Content.Headers.ContentLength = videoBytes.Length;
                    request4.Headers.Add("x-ig-app-id", "1217981644879628");
                    request4.Headers.Add("x-requested-with", "XMLHttpRequest");
                    request4.Headers.Add("x-instagram-rupload-params",
                        $"{{\"media_type\":\"{mediaType}\",\"upload_id\":{uploadID},\"video_format\":\"\",\"video_transform\":null,\"client-passthrough\":\"1\",\"is_sidecar\":\"0\",\"for_album\":false,\"is_clips_video\":true,\"upload_media_height\":{items.Item2},\"upload_media_width\":{items.Item1}}}");
                    request4.Headers.Add("x-instagram-ajax", GramStatic.InstagramAjax());
                    request4.Headers.Add("x-csrftoken", param.CsrfToken);
                    request4.Headers.Add("x-entity-length",videoBytes.Length.ToString());
                    request4.Headers.Add("x-entity-name", $"reels_{uploadID}");
                    request4.Headers.Add("x-asbd-id", "359341");
                    request4.Headers.Add("sec-ch-ua-full-version-list", "\"(Not(A:Brand\";v=\"99.0.0.0\", \"Google Chrome\";v=\"134\", \"Chromium\";v=\"134\"");
                    request4.Headers.Add("sec-ch-ua-platform", "\"Linux\"");
                    request4.Headers.Add("offset", "0");
                    request4.Headers.Add("sec-ch-ua", "\"(Not(A:Brand\";v=\"99\", \"Google Chrome\";v=\"134\", \"Chromium\";v=\"134\"");
                    request4.Headers.Add("sec-ch-ua-model", "\"\"");
                    request4.Headers.Add("sec-ch-ua-mobile", "?0");
                    request4.Headers.Add("Sec-Fetch-Site", "same-origin");
                    request4.Headers.Add("Sec-Fetch-Mode", "cors");
                    request4.Headers.Add("Sec-Fetch-Dest", "empty");
                    request4.Headers.Add("Accept-Language", "en-GB,en-US;q=0.9,en;q=0.8");
                    request4.Headers.Add("Accept-Encoding", GramStatic.AcceptEncoding);
                    request4.Headers.Referrer = new Uri($"https://www.instagram.com/{type}/{instagramPost.Code}");
                    request4.Headers.Add("Origin", "https://www.instagram.com");
                    request4.Headers.UserAgent.ParseAdd("Mozilla/5.0 (X11; Debian; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/134.0.6813.61 Safari/537.36");

                    var response = await client.SendAsync(request4);
                    var responseText = await response.Content.ReadAsStreamAsync();
                    var responseTextString = HttpHelper.Decode(responseText, string.Join(",", response.Content.Headers.ContentEncoding));
                    if (string.IsNullOrEmpty(responseTextString?.Response) || !responseTextString.Response.Contains("\"media_id\""))
                        return uploadMediaResponse;
                    var url = $"https://www.instagram.com/rupload_igphoto/fb_uploader_{uploadID}";
                    var imagePath = instagramPost.Images.GetRandomItem().Url;
                    byte[] imageBytes = await GetMediaData(imagePath);

                    using (var client1 = new HttpClient(param.httpClient))
                    {
                        var request = new HttpRequestMessage(HttpMethod.Post, url)
                        {
                            Content = new ByteArrayContent(imageBytes)
                        };

                        // Headers
                        request.Headers.Add("x-instagram-rupload-params", $"{{\"media_type\":\"{mediaType}\",\"upload_id\":{uploadID},\"upload_media_height\":{items.Item2},\"upload_media_width\":{items.Item1}}}");
                        request.Headers.Add("x-instagram-ajax", GramStatic.InstagramAjax());
                        request.Headers.Add("x-csrftoken",param.CsrfToken);
                        request.Headers.Add("offset", "0");
                        request.Headers.Add("x-entity-length", imageBytes.Length.ToString());
                        request.Headers.Add("x-ig-app-id", "1217981644879628");
                        request.Headers.Add("x-asbd-id", "359341");
                        request.Headers.Add("x-entity-type", "image/jpeg");
                        request.Headers.Add("x-requested-with", "XMLHttpRequest");
                        request.Headers.Add("x-entity-name", $"reels_{uploadID}");
                        request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/135.0.0.0 Safari/537.36");
                        request.Headers.Add("sec-ch-ua-full-version-list", "\"(Not(A:Brand\";v=\"99.0.0.0\", \"Google Chrome\";v=\"134\", \"Chromium\";v=\"134\"");
                        request.Headers.Add("sec-ch-ua-platform", "\"Linux\"");
                        request.Headers.Add("sec-ch-ua", "\"(Not(A:Brand\";v=\"99\", \"Google Chrome\";v=\"134\", \"Chromium\";v=\"134\"");
                        request.Headers.Add("sec-ch-ua-model", "\"\"");
                        request.Headers.Add("sec-ch-ua-mobile", "?0");
                        request.Headers.Add("Sec-Fetch-Site", "same-origin");
                        request.Headers.Add("Sec-Fetch-Mode", "cors");
                        request.Headers.Add("Sec-Fetch-Dest", "empty");
                        request.Headers.Add("Accept-Language", "en-GB,en-US;q=0.9,en;q=0.8");
                        request.Headers.Add("Accept-Encoding", GramStatic.AcceptEncoding);
                        request.Content.Headers.ContentLength = imageBytes.Length;

                        try
                        {
                            HttpResponseMessage response1 = await client1.SendAsync(request);
                            var responseText1 = await response1.Content.ReadAsStreamAsync();
                            var decodeString = HttpHelper.Decode(responseText1, string.Join(",", response1.Content.Headers.ContentEncoding));
                            if (string.IsNullOrEmpty(decodeString?.Response) || !decodeString.Response.Contains("\"upload_id\""))
                                return uploadMediaResponse;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error during upload:");
                            Console.WriteLine(ex.Message);
                        }
                        var tags = string.Empty;
                        if(instagramPost.UserTags != null && instagramPost.UserTags.Count > 0)
                        {
                            var random = new Random();
                            var userTags = string.Join(",", instagramPost.UserTags.Select(x => $"{{\"user_id\":\"{FetchAnonStory(x.Username).Result}\"}}"));
                            tags += "&usertags=" +WebUtility.UrlEncode($"{{\"in\":[{userTags}]}}");
                        }
                        var commentDisabled = instagramPost.CommentsDisabled ? 1 : 0;
                        var HideLikeAndViewCount = instagramPost.CommentLikesEnabled ? 1 : 0;
                        var url2 = "https://www.instagram.com/api/v1/media/configure_to_clips/";
                        var postData = $"upload_id={uploadID}&caption={Uri.EscapeUriString(instagramPost.Caption)}&clips_share_preview_to_feed=1&disable_comments={commentDisabled}&like_and_view_counts_disabled={HideLikeAndViewCount}{tags}";
                        using (var client2 = new HttpClient(param.httpClient))
                        {
                            var request1 = new HttpRequestMessage(HttpMethod.Post, url2)
                            {
                                Content = new StringContent(postData)
                            };

                            // Content-Type header
                            request1.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded")
                            {
                                CharSet = "UTF-8"
                            };

                            // Headers from browser request
                            //request1.Headers.Add("x-asbd-id", "359341");
                            request1.Headers.Add("x-instagram-ajax", GramStatic.InstagramAjax());
                            request1.Headers.Add("x-csrftoken", param.CsrfToken);
                            request1.Headers.Add("x-requested-with", "XMLHttpRequest");
                            request1.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/135.0.0.0 Safari/537.36");
                            request1.Headers.Add("x-ig-app-id", "1217981644879628");
                            request1.Headers.Add("sec-ch-ua-full-version-list", "\"(Not(A:Brand\";v=\"99.0.0.0\", \"Google Chrome\";v=\"134\", \"Chromium\";v=\"134\"");
                            request1.Headers.Add("sec-ch-ua-platform", "\"Linux\"");
                            request1.Headers.Add("sec-ch-ua", "\"(Not(A:Brand\";v=\"99\", \"Google Chrome\";v=\"134\", \"Chromium\";v=\"134\"");
                            request1.Headers.Add("sec-ch-ua-model", "\"\"");
                            request1.Headers.Add("sec-ch-ua-mobile", "?0");
                            request1.Headers.Add("Sec-Fetch-Site", "same-origin");
                            request1.Headers.Add("Sec-Fetch-Mode", "cors");
                            request1.Headers.Add("Sec-Fetch-Dest", "empty");
                            request1.Headers.Add("Accept-Language", "en-GB,en-US;q=0.9,en;q=0.8");
                            request1.Headers.Add("Accept-Encoding", GramStatic.AcceptEncoding);

                            try
                            {
                                var response2 = await client2.SendAsync(request1);
                                var stream1 = await response2.Content.ReadAsStreamAsync();
                                var responseText1 = HttpHelper.Decode(stream1,string.Join(",",response2.Content.Headers.ContentEncoding));
                                uploadMediaResponse = new UploadMediaResponse(responseText1);
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }
                }
            }
            catch { }
            return uploadMediaResponse;
        }

        private async Task<byte[]> GetMediaData(string imagePath)
        {
            using (var httpClient = new HttpClient())
            {
                try
                {
                    byte[] imageBytes = await httpClient.GetByteArrayAsync(imagePath);

                   return imageBytes;
                }
                catch (Exception)
                {
                    try
                    {
                        return File.ReadAllBytes(imagePath);
                    }
                    catch { return new byte[0]; }
                }
            }
        }
        public string CropToInstagramPortraitWithPadding(string inputPath)
        {
            var outputPath = Path.Combine(Path.GetTempPath(), $"Video-{DateTime.Now.Ticks.ToString()}.mp4");
            var resolution = GetVideoResolution(inputPath);
            int inW = resolution.Width;
            int inH = resolution.Height;

            // Target 4:5 aspect ratio crop size
            double targetAspect = 4.0 / 5.0;

            int cropW = inW;
            int cropH = (int)(cropW / targetAspect);

            if (cropH > inH)
            {
                cropH = inH;
                cropW = (int)(cropH * targetAspect);
            }

            int cropX = (inW - cropW) / 2;
            int cropY = (inH - cropH) / 2;

            // Pad crop back to original dimensions
            int padX = (inW - cropW) / 2;
            int padY = (inH - cropH) / 2;

            // Compose FFmpeg filter: crop + pad
            string filter = $"crop={cropW}:{cropH}:{cropX}:{cropY},pad={inW}:{inH}:{padX}:{padY}:black";

            var ffMpeg = new FFMpegConverter();
            ffMpeg.ConvertMedia(
                inputPath,
                null,
                outputPath,
                Format.mp4,
                new ConvertSettings()
                {
                    CustomOutputArgs = $"-vf \"{filter}\" -preset fast -c:a copy"
                }
            );
            return outputPath;
        }
        private static readonly HttpClient client = new HttpClient();

        public async Task<string> FetchAnonStory(string teamName)
        {
            var userId = "";
            // Construct raw auth string
            string authRaw = $"-1::{teamName}::LTE6Om11cmllbGdhbGxlOjpySlAydEJSS2Y2a3RiUnFQVUJ0UkU5a2xnQldiN2Q-";

            // Base64 encode
            string authEncoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(authRaw));

            // Set headers
            var request = new HttpRequestMessage(HttpMethod.Post, "https://anonstories.com/api/v1/story");
            request.Headers.Add("Host", "anonstories.com");
            request.Headers.Add("Connection", "keep-alive");
            request.Headers.Add("sec-ch-ua-platform", "\"Linux\"");
            request.Headers.Add("X-Requested-With", "XMLHttpRequest");
            request.Headers.Add("User-Agent", "Mozilla/5.0 (X11; Ubuntu; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/134.0.6784.83 Safari/537.36");
            request.Headers.Add("Accept", "*/*");
            request.Headers.Add("sec-ch-ua", "\"(Not(A:Brand\";v=\"99\", \"Google Chrome\";v=\"134\", \"Chromium\";v=\"134\"");
            request.Headers.Add("sec-ch-ua-mobile", "?0");
            request.Headers.Add("Origin", "https://anonstories.com");
            request.Headers.Add("Sec-Fetch-Site", "same-origin");
            request.Headers.Add("Sec-Fetch-Mode", "cors");
            request.Headers.Add("Sec-Fetch-Dest", "empty");
            request.Headers.Add("Referer", "https://anonstories.com/");
            request.Headers.Add("Accept-Encoding", GramStatic.AcceptEncoding);
            request.Headers.Add("Accept-Language", "en-GB,en-US;q=0.9,en;q=0.8");

            // Set content
            var formData = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("auth", authEncoded)
            });
            request.Content = formData;
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

            try
            {
                var response = await client.SendAsync(request);
                var stream1 = await response.Content.ReadAsStreamAsync();
                var responseText1 = HttpHelper.Decode(stream1, string.Join(",", response.Content.Headers.ContentEncoding));
                userId = Utilities.GetBetween(responseText1?.Response, "\"id\":", ",\"");
            }
            catch (Exception)
            {
            }
            return userId;
        }

        private (int Width, int Height) GetVideoResolution(string inputPath)
        {
            var ffProbe = new System.Diagnostics.Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "ffprobe",
                    Arguments = $"-v error -select_streams v:0 -show_entries stream=width,height -of csv=s=x:p=0 \"{inputPath}\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            ffProbe.Start();
            string output = ffProbe.StandardOutput.ReadLine();
            ffProbe.WaitForExit();

            var parts = output.Split('x');
            return (int.Parse(parts[0]), int.Parse(parts[1]));
        }


        private (int, int) GetCropRatio(InstagramPost instagramPost, string filePath)
        {
            try
            {
                var info = GramStatic.GetMediaInfo(filePath);
                if (info != null)
                {
                    var stream = info.Streams.FirstOrDefault(x => x.CodecType == "video");
                    if (stream != null)
                        return (stream.Width, stream.Height);
                    return (720,1280);
                }
            }
            catch { }
            return (720,1280);
        }

        public async Task<UploadMediaResponse> UploadMedia(DominatorAccountModel dominatorAccountModel, InstagramPost instagramPost, List<string> mediaList)
        {
            UploadMediaResponse uploadMediaResponse = null;

            try
            {
                if(mediaList != null)
                {
                    var uploadID = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
                    var param = GetWebParameter(dominatorAccountModel,true);
                    if (mediaList.Count > 1)
                    {
                        var listOfMediaIDs = new List<Dictionary<bool,string>>();
                        foreach (var media in mediaList)
                        {
                            var mediaInfo = GramStatic.GetMediaInfo(media);
                            var uploaded = false;
                            var IsVideo = mediaInfo.FormatName.Contains("mp4")
                                || mediaInfo.FormatName.Contains("m4a");
                            uploadID = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
                            if (!IsVideo)
                                uploaded = await UploadImage(dominatorAccountModel, instagramPost, media, uploadID, param.cookieContainer);
                            else
                                uploaded = await UploadVideoHttp(dominatorAccountModel, instagramPost, media, uploadID, param.cookieContainer,1);
                            if (uploaded)
                                listOfMediaIDs.Add(new Dictionary<bool, string>()
                                {
                                    { IsVideo, uploadID }
                                });
                            await Task.Delay(TimeSpan.FromSeconds(4), dominatorAccountModel.Token);
                        }
                        if (listOfMediaIDs.Count > 0)
                        {
                            var count = 0;
                            Retry:
                            var tags = string.Empty;
                            var url2 = "https://www.instagram.com/api/v1/media/configure_sidecar/";
                            var haveVideo = false;
                            if (instagramPost.UserTags != null && instagramPost.UserTags.Count > 0)
                            {
                                var random = new Random();
                                haveVideo = listOfMediaIDs.Any(x => x.FirstOrDefault().Key == true);
                                var userTags = string.Join(",", instagramPost.UserTags.Select(x => !haveVideo ? $"{{\\\"position\\\":[{random.NextDouble().ToString("F16")},{random.NextDouble().ToString("F16")}],\\\"user_id\\\":\\\"{x.UserId}\\\"}}": $"{{\\\"user_id\\\":\\\"{x.UserId}\\\"}}"));
                                tags = $",\"usertags\":\"{{\\\"in\\\":[{userTags}]}}\"";
                            }
                            var ids = string.Empty;
                            var tagsData = string.Empty;
                            var assigned = false;
                            foreach (var id in listOfMediaIDs)
                            {

                                if (haveVideo)
                                {
                                    if (haveVideo && id.Keys.FirstOrDefault())
                                    {
                                        tagsData = tags;
                                        assigned = true;
                                    }
                                    ids += $"{{\"upload_id\":\"{id.FirstOrDefault().Value}\"{tagsData}}}";
                                    if (id != listOfMediaIDs.LastOrDefault())
                                        ids += ",";
                                }
                                else
                                {
                                    tagsData = tags;
                                    ids += $"{{\"upload_id\":\"{id.FirstOrDefault().Value}\"{tagsData}}}";
                                    if (id != listOfMediaIDs.LastOrDefault())
                                        ids += ",";
                                    assigned = true;
                                }
                                if (assigned)
                                {
                                    tags = string.Empty;
                                    tagsData = string.Empty;
                                    assigned = false;
                                }
                            }
                            var childrenMetaData = $",\"children_metadata\":[{ids}]";
                            var isLocationEnabled = instagramPost.HasLocation ? "true":"false";
                            var location = "";
                            if (instagramPost.HasLocation)
                            {
                                if (instagramPost.IsLocationPost)
                                {
                                    var locationResponse = await SearchForLocation(dominatorAccountModel, instagramPost?.Location?.Name, true);
                                    if(!string.IsNullOrEmpty(locationResponse.location.Name))
                                        instagramPost.Location = locationResponse?.location;
                                }
                                location = $",\"location\":{{\"facebook_places_id\":\"{instagramPost.Location.Id}\",\"lat\":{instagramPost.Location.Lat},\"lng\":{instagramPost.Location.Lng}}}";
                            }
                            var commentDisabled = instagramPost.CommentsDisabled ? 1 : 0;
                            var HideLikeAndViewCount = instagramPost.CommentLikesEnabled ? 1 : 0;
                            using (var client2 = new HttpClient(param.httpClient))
                            {
                                var postData = $"{{\"archive_only\":false,\"caption\":\"{instagramPost.Caption?.Replace("\r\n", "\n")?.Replace("\n", "\\n")}\"{childrenMetaData},\"client_sidecar_id\":\"{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString()}\",\"disable_comments\":\"{commentDisabled}\",\"is_meta_only_post\":false,\"is_open_to_public_submission\":false,\"like_and_view_counts_disabled\":{HideLikeAndViewCount},\"media_share_flow\":\"creation_flow\",\"share_to_facebook\":\"\",\"share_to_fb_destination_type\":\"USER\",\"source_type\":\"library\",\"geotag_enabled\":\"{isLocationEnabled}\"{location},\"jazoest\":\"{GramStatic.CreateJazoest()}\"}}";

                                var request2 = new HttpRequestMessage(HttpMethod.Post, url2)
                                {
                                    Content = new StringContent(postData)
                                };

                                // Add headers
                                request2.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                                request2.Headers.Add("X-Instagram-AJAX", GramStatic.InstagramAjax());
                                request2.Headers.Add("X-CSRFToken", param.CsrfToken);
                                request2.Headers.Add("X-IG-App-ID", "936619743392459");
                                request2.Headers.Add("sec-ch-ua-full-version-list", "\"Not)A;Brand\";v=\"8.0.0.0\", \"Chromium\";v=\"138.0.7204.96\", \"Google Chrome\";v=\"138.0.7204.96\"");
                                request2.Headers.Add("sec-ch-ua-platform", "\"Windows\"");
                                request2.Headers.Add("sec-ch-ua", "\"Not)A;Brand\";v=\"8\", \"Chromium\";v=\"138\", \"Google Chrome\";v=\"138\"");
                                request2.Headers.Add("sec-ch-ua-model", "\"\"");
                                request2.Headers.Add("sec-ch-ua-mobile", "?0");
                                request2.Headers.Add("sec-ch-prefers-color-scheme", "light");
                                request2.Headers.Add("Sec-Fetch-Site", "same-origin");
                                request2.Headers.Add("Sec-Fetch-Mode", "cors");
                                request2.Headers.Add("Sec-Fetch-Dest", "empty");
                                request2.Headers.Add("Accept-Language", "en-GB,en-US;q=0.9,en;q=0.8");
                                request2.Headers.Add("Accept-Encoding", GramStatic.AcceptEncoding);
                                request2.Headers.Add("X-ASBD-ID", "359341");
                                request2.Headers.Add("Origin", "https://www.instagram.com");
                                request2.Headers.Add("dpr", "1.1");
                                request2.Headers.Add("viewport-width", "1745");
                                request2.Headers.Add("sec-ch-ua-platform-version", "\"10.0.0\"");
                                request2.Headers.Add("X-Requested-With", "XMLHttpRequest");
                                request2.Headers.Add("X-Web-Session-ID", GramStatic.GetWebSessionID());
                                request2.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/138.0.0.0 Safari/537.36");
                                request2.Headers.Referrer = new Uri("https://www.instagram.com/");
                                request2.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
                                var response2 = await client2.SendAsync(request2);
                                var responseContent = await response2.Content.ReadAsStreamAsync();
                                var responseText2 = HttpHelper.Decode(responseContent, string.Join(",", response2.Content.Headers.ContentEncoding));
                                if (count++ <= 3 && responseText2 != null && !string.IsNullOrEmpty(responseText2.Response) && (responseText2.Response.Contains("Transcode not finished yet.")
                                    ||responseText2.Response.Contains("\"message\":\"feedback_required\"")))
                                {
                                    await Task.Delay(TimeSpan.FromSeconds(10), dominatorAccountModel.Token);
                                    param = GetWebParameter(dominatorAccountModel);
                                    goto Retry;
                                }
                                if (responseText2 != null && !string.IsNullOrEmpty(responseText2.Response) && responseText2.Response.Contains("\"status\":\"ok\""))
                                {
                                    uploadMediaResponse = new UploadMediaResponse(responseText2);
                                }
                            }
                        }
                    }
                    else
                    {
                        var media = mediaList.FirstOrDefault();
                        var mediaInfo = GramStatic.GetMediaInfo(media);
                        var uploaded = false;
                        var IsVideo = mediaInfo.FormatName.Contains("mp4");
                        if (IsVideo)
                            uploaded = await UploadVideoHttp(dominatorAccountModel, instagramPost, media, uploadID,param.cookieContainer);
                        else
                            uploaded = await UploadImage(dominatorAccountModel, instagramPost, media, uploadID, param.cookieContainer);
                        if (uploaded)
                        {
                            try
                            {
                                var count = 0;
                              Retry:
                                var url2 = IsVideo ? "https://www.instagram.com/api/v1/media/configure_to_clips/" : "https://www.instagram.com/api/v1/media/configure/";
                                var tags = string.Empty;
                                if (instagramPost.UserTags != null && instagramPost.UserTags.Count > 0)
                                {
                                    var random = new Random();
                                    var userTags = string.Join(",", instagramPost.UserTags.Select(x => !IsVideo ? $"{{\"position\":[{random.NextDouble().ToString("F16")},{random.NextDouble().ToString("F16")}],\"user_id\":\"{x.UserId}\"}}": $"{{\"user_id\":\"{x.UserId}\"}}"));
                                    tags += "&usertags=" + WebUtility.UrlEncode($"{{\"in\":[{userTags}]}}");
                                }
                                var isLocationEnabled = instagramPost.HasLocation;
                                var location = string.Empty;
                                if (isLocationEnabled)
                                {
                                    if (instagramPost.IsLocationPost)
                                    {
                                        var locationResponse = await SearchForLocation(dominatorAccountModel, instagramPost?.Location?.Name, true);
                                        if (!string.IsNullOrEmpty(locationResponse.location.Name))
                                            instagramPost.Location = locationResponse?.location;
                                    }
                                    location = $"&location=" + WebUtility.UrlEncode($"{{\"facebook_places_id\":\"{instagramPost.Location.Id}\",\"lat\":{instagramPost.Location.Lat},\"lng\":{instagramPost.Location.Lng}}}");
                                }
                                var commentDisabled = instagramPost.CommentsDisabled ? 1 : 0;
                                var HideLikeAndViewCount = instagramPost.CommentLikesEnabled ? 1 : 0;
                                using (var client2 = new HttpClient(param.httpClient))
                                {
                                    var postData = $"archive_only=false&caption={Uri.EscapeUriString(instagramPost.Caption)}&clips_share_preview_to_feed=1&disable_comments={commentDisabled}&disable_oa_reuse=false&igtv_share_preview_to_feed=1&is_meta_only_post=0&is_unified_video=1&like_and_view_counts_disabled={HideLikeAndViewCount}&media_share_flow=creation_flow&share_to_facebook=&share_to_fb_destination_type=USER&source_type=library&upload_id={uploadID}{tags}&video_subtitles_enabled=0&geotag_enabled={isLocationEnabled}{location}&jazoest={GramStatic.CreateJazoest(dominatorAccountModel.DeviceDetails.PhoneId)}";

                                    var request2 = new HttpRequestMessage(HttpMethod.Post, url2)
                                    {
                                        Content = new StringContent(postData, Encoding.UTF8, "application/x-www-form-urlencoded")
                                    };

                                    // Add headers
                                    request2.Headers.Add("X-Instagram-AJAX", GramStatic.InstagramAjax());
                                    request2.Headers.Add("X-CSRFToken", param.CsrfToken);
                                    request2.Headers.Add("X-IG-App-ID", "936619743392459");
                                    request2.Headers.Add("sec-ch-ua-full-version-list", "\"Google Chrome\";v=\"137.0.7151.120\", \"Chromium\";v=\"137.0.7151.120\", \"Not/A)Brand\";v=\"24.0.0.0\"");
                                    request2.Headers.Add("sec-ch-ua-platform", "\"Windows\"");
                                    request2.Headers.Add("sec-ch-ua", "\"Google Chrome\";v=\"137\", \"Chromium\";v=\"137\", \"Not/A)Brand\";v=\"24\"");
                                    request2.Headers.Add("sec-ch-ua-model", "\"\"");
                                    request2.Headers.Add("sec-ch-ua-mobile", "?0");
                                    request2.Headers.Add("sec-ch-prefers-color-scheme", "light");
                                    request2.Headers.Add("Sec-Fetch-Site", "same-origin");
                                    request2.Headers.Add("Sec-Fetch-Mode", "cors");
                                    request2.Headers.Add("Sec-Fetch-Dest", "empty");
                                    request2.Headers.Add("Accept-Language", "en-GB,en;q=0.9");
                                    //request2.Headers.Add("X-ASBD-ID", "359341");
                                    request2.Headers.Add("X-Requested-With", "XMLHttpRequest");
                                    request2.Headers.Add("Accept-Encoding", GramStatic.AcceptEncoding);
                                    request2.Headers.Add("X-Web-Session-ID", GramStatic.GetWebSessionID());
                                    request2.Headers.Add("X-IG-WWW-Claim", param?.X_IG_Claim);
                                    request2.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/137.0.0.0 Safari/537.36");
                                    request2.Headers.Referrer = new Uri("https://www.instagram.com/");
                                    request2.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
                                    var response2 = await client2.SendAsync(request2);
                                    var responseContent = await response2.Content.ReadAsStreamAsync();
                                    var responseText2 = HttpHelper.Decode(responseContent, string.Join(",", response2.Content.Headers.ContentEncoding));
                                    if(count ++ <=3 && responseText2 != null && !string.IsNullOrEmpty(responseText2.Response) && (responseText2.Response.Contains("Transcode not finished yet.")
                                        || responseText2.Response.Contains("\"message\":\"feedback_required\"")))
                                    {
                                        await Task.Delay(TimeSpan.FromSeconds(10), dominatorAccountModel.Token);
                                        param = GetWebParameter(dominatorAccountModel);
                                        goto Retry;
                                    }
                                    if (responseText2 != null && !string.IsNullOrEmpty(responseText2.Response) && responseText2.Response.Contains("\"status\":\"ok\""))
                                    {
                                        uploadMediaResponse = new UploadMediaResponse(responseText2);
                                    }
                                    else
                                    {
                                        uploadMediaResponse = new UploadMediaResponse(responseText2);
                                    }
                                }
                            }
                            catch (HttpRequestException e)
                            {
                                Console.WriteLine("Error: " + e.Message);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {

            }

            return uploadMediaResponse;
        }

        private async Task<bool> UploadVideoHttp(DominatorAccountModel dominatorAccountModel, InstagramPost instagramPost, string media, string uploadID, CookieContainer cookieContainer,int IsSideCar=0)
        {
            try
            {
                //Option Request.
                var ClipParam = IsSideCar == 0 ? "\"is_clips_video\":\"1\"" : "\"is_unified_video\":\"0\"";
                var url = $"https://i.instagram.com/rupload_igvideo/fb_uploader_{uploadID}";

                using (var client = new HttpClient())
                {
                    var request = new HttpRequestMessage(HttpMethod.Options, url);
                    request.Headers.Add("Accept", "*/*");
                    request.Headers.Add("Access-Control-Request-Method", "GET");
                    request.Headers.Add("Access-Control-Request-Headers", "x-ig-app-id");
                    request.Headers.Add("Origin", "https://www.instagram.com");
                    request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/137.0.0.0 Safari/537.36");
                    request.Headers.Add("Sec-Fetch-Mode", "cors");
                    request.Headers.Add("Sec-Fetch-Site", "same-site");
                    request.Headers.Add("Sec-Fetch-Dest", "empty");
                    request.Headers.Add("Referer", "https://www.instagram.com/");
                    request.Headers.Add("Accept-Encoding", GramStatic.AcceptEncoding);
                    request.Headers.Add("Accept-Language", "en-GB,en-US;q=0.9,en;q=0.8");

                    try
                    {
                        var response = await client.SendAsync(request);
                    }
                    catch
                    {
                    }
                }

                var mediaInfoData = GramStatic.GetMediaInfo(media);
                var url1 = $"https://i.instagram.com/rupload_igvideo/fb_uploader_{uploadID}";
                byte[] videoData = await GetMediaData(media);

                var handler = new HttpClientHandler
                {
                    UseCookies = true,
                    CookieContainer = cookieContainer
                };
                using (var client1 = new HttpClient(handler))
                {
                    var request1 = new HttpRequestMessage(HttpMethod.Post, url1)
                    {
                        Content = new ByteArrayContent(videoData)
                    };

                    request1.Content.Headers.ContentLength = videoData.Length;
                    request1.Headers.Host = "i.instagram.com";
                    request1.Headers.Add("X-Instagram-Rupload-Params", $"{{\"client-passthrough\":\"1\",{ClipParam},\"is_sidecar\":\"{IsSideCar}\",\"media_type\":2,\"for_album\":false,\"video_format\":\"\",\"upload_id\":\"" + uploadID + $"\",\"upload_media_duration_ms\":{mediaInfoData.Duration.TotalMilliseconds},\"upload_media_height\":640,\"upload_media_width\":360,\"video_transform\":null}}");
                    request1.Headers.Add("X-Instagram-AJAX", GramStatic.InstagramAjax());
                    //request1.Headers.Add("X-Web-Session-ID", "1rgqy3:fn0893:bw2iso");
                    request1.Headers.Add("Offset", "0");
                    request1.Headers.Add("X-Entity-Length", videoData.Length.ToString());
                    request1.Headers.Add("X-IG-App-ID", "936619743392459");
                    //request1.Headers.Add("X-ASBD-ID", "359341");
                    request1.Headers.Add("X-Entity-Name", $"fb_uploader_{uploadID}");
                    request1.Headers.Add("sec-ch-ua", "\"Google Chrome\";v=\"137\", \"Chromium\";v=\"137\", \"Not/A)Brand\";v=\"24\"");
                    request1.Headers.Add("sec-ch-ua-platform", "\"Windows\"");
                    request1.Headers.Add("sec-ch-ua-mobile", "?0");
                    request1.Headers.Add("Sec-Fetch-Site", "same-site");
                    request1.Headers.Add("Sec-Fetch-Mode", "cors");
                    request1.Headers.Add("Sec-Fetch-Dest", "empty");
                    request1.Headers.Add("Accept-Encoding", GramStatic.AcceptEncoding);
                    request1.Headers.Referrer = new Uri("https://www.instagram.com/");
                    request1.Headers.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/137.0.0.0 Safari/537.36");
                    request1.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
                    request1.Headers.Add("Origin", "https://www.instagram.com");
                    request1.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                    try
                    {
                        var response = await client1.SendAsync(request1);
                        var stream = await response.Content.ReadAsStreamAsync();
                        var ResponseText = HttpHelper.Decode(stream, string.Join(",", response.Content.Headers.ContentEncoding));
                        if(ResponseText != null && !string.IsNullOrEmpty(ResponseText.Response) && ResponseText.Response.Contains("\"status\":\"ok\""))
                        {
                            //UploadCoverPhoto
                            var thumbnailImage = new MediaUtilites().GetThumbnail(media);
                            return await UploadImage(dominatorAccountModel, instagramPost, thumbnailImage, uploadID, cookieContainer,MediaType:"2");
                        }
                        return false;
                    }
                    catch
                    {
                        return false;
                    }
                }
            }
            catch { return false; }
        }

        private async Task<bool> UploadImage(DominatorAccountModel dominatorAccountModel, InstagramPost instagramPost, string media, string uploadID, CookieContainer cookieContainer,string MediaType="1")
        {
            var url = $"https://i.instagram.com/rupload_igphoto/fb_uploader_{uploadID}";
            var client = new HttpClient();

            var request = new HttpRequestMessage(HttpMethod.Options, url);

            // Add headers
            request.Headers.Add("Host", "i.instagram.com");
            request.Headers.Add("Connection", "keep-alive");
            request.Headers.Add("Accept", "*/*");
            request.Headers.Add("Access-Control-Request-Method", "POST");
            request.Headers.Add("Access-Control-Request-Headers", "content-type,x-entity-length,x-entity-name,x-entity-type,x-ig-app-id,x-instagram-ajax,x-instagram-rupload-params");
            request.Headers.Add("Origin", "https://www.instagram.com");
            request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/137.0.0.0 Safari/537.36");
            request.Headers.Add("Sec-Fetch-Mode", "cors");
            request.Headers.Add("Sec-Fetch-Site", "same-site");
            request.Headers.Add("Sec-Fetch-Dest", "empty");
            request.Headers.Add("Referer", "https://www.instagram.com/");
            request.Headers.Add("Accept-Encoding", GramStatic.AcceptEncoding);
            request.Headers.Add("Accept-Language", "en-GB,en-US;q=0.9,en;q=0.8");

            // Send the request
            var response = await client.SendAsync(request);
            // Load image as byte array
            byte[] imageData = await GetMediaData(media);
            var handler = new HttpClientHandler
            {
                CookieContainer = cookieContainer,
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };

            using (var client1 = new HttpClient(handler))
            {
                // Set cookies manually for domain
                var uri = new Uri("https://i.instagram.com");
                var request1 = new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Content = new ByteArrayContent(imageData)
                };

                // Headers
                request1.Headers.Add("X-Instagram-Rupload-Params", $"{{\"media_type\":{MediaType},\"upload_id\":\"{uploadID}\",\"upload_media_height\":1280,\"upload_media_width\":720}}");
                request1.Headers.Add("X-Instagram-AJAX", GramStatic.InstagramAjax());
                request1.Headers.Add("sec-ch-ua-platform", "\"Windows\"");
                //request1.Headers.Add("X-Web-Session-ID", "7w88rv:z2i9n0:yrffu2");
                request1.Headers.Add("Offset", "0");
                request1.Headers.Add("sec-ch-ua", "\"Google Chrome\";v=\"137\", \"Chromium\";v=\"137\", \"Not/A)Brand\";v=\"24\"");
                request1.Headers.Add("sec-ch-ua-mobile", "?0");
                request1.Headers.Add("X-Entity-Length", imageData.Length.ToString());
                request1.Headers.Add("X-IG-App-ID", "936619743392459");
                //request1.Headers.Add("X-ASBD-ID", "359341");
                request1.Headers.Add("X-Entity-Type", "image/jpeg");
                request1.Headers.Add("X-Entity-Name", $"fb_uploader_{uploadID}");
                request1.Headers.Add("Origin", "https://www.instagram.com");
                request1.Headers.Add("Sec-Fetch-Site", "same-site");
                request1.Headers.Add("Sec-Fetch-Mode", "cors");
                request1.Headers.Add("Sec-Fetch-Dest", "empty");
                request1.Headers.Referrer = new Uri("https://www.instagram.com/");
                request1.Headers.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/137.0.0.0 Safari/537.36");
                request1.Headers.Accept.ParseAdd("*/*");
                request1.Headers.Add("Accept-Encoding", GramStatic.AcceptEncoding);
                request1.Headers.AcceptLanguage.ParseAdd("en-GB,en-US;q=0.9,en;q=0.8");

                // Content-Type
                request1.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
                request1.Content.Headers.ContentLength = imageData.Length;

                try
                {
                    var response1 = await client1.SendAsync(request1);
                    var result1 = await response1.Content.ReadAsStreamAsync();
                    var responseText1 = HttpHelper.Decode(result1, string.Join(",", response1.Content.Headers.ContentEncoding));
                    return responseText1 != null && !string.IsNullOrEmpty(responseText1.Response) && responseText1.Response.Contains("\"status\":\"ok\"");
                }
                catch
                {
                    return false;
                }
            }
        }

        public async Task<MediaCommentsIgResponseHandler> GetCommentDetails(DominatorAccountModel dominatorAccount, string MediaCode, string CommentID)
        {
            MediaCommentsIgResponseHandler mediaComments = null;
            try
            {
                var mediaID = GramStatic.GetCodeFromIDOrUrl(MediaCode);
                var url = $"https://www.instagram.com/api/v1/media/{mediaID}/comments/?can_support_threading=true&target_comment_id={CommentID}&permalink_enabled=true";
                var param = GetWebParameter(dominatorAccount);
                using (var client = new HttpClient(param.httpClient))
                {
                    var baseUri = new Uri("https://www.instagram.com");
                    // ✅ Required headers
                    client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/137.0.0.0 Safari/537.36");
                    client.DefaultRequestHeaders.Add("X-CSRFToken", param.CsrfToken);
                    client.DefaultRequestHeaders.Add("X-IG-App-ID", "936619743392459");
                    client.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
                    //client.DefaultRequestHeaders.Add("X-Web-Session-ID", "ahil24:tvgxqg:4s9dlu");
                    client.DefaultRequestHeaders.Add("X-ASBD-ID", "359341");
                    if(!string.IsNullOrEmpty(param.X_IG_Claim))
                        client.DefaultRequestHeaders.Add("X-IG-WWW-Claim", param.X_IG_Claim);
                    client.DefaultRequestHeaders.Add("Accept", "*/*");
                    client.DefaultRequestHeaders.Add("Accept-Encoding", GramStatic.AcceptEncoding);
                    client.DefaultRequestHeaders.Referrer = new Uri($"https://www.instagram.com/p/{MediaCode}/c/{CommentID}/");
                    // ✅ Make request
                    var response = await client.GetAsync(url);
                    var content = await response.Content.ReadAsStreamAsync();
                    var responseText = HttpHelper.Decode(content, string.Join(",", response.Content.Headers.ContentEncoding));
                    mediaComments = new MediaCommentsIgResponseHandler(responseText, true);
                }
            }
            catch { }
            return mediaComments;
        }

        public IGWebParameter GetWebParameter(DominatorAccountModel dominatorAccount,bool MobileRequest=false)
        {
            var cookie_Container = new CookieContainer();
            var proxy = dominatorAccount?.AccountBaseModel?.AccountProxy;
            var csrf = string.Empty;
            var x_ig_claim = dominatorAccount?.DeviceDetails?.IGXClaim;
            var s = string.Empty;
            var dsuid = string.Empty;
            var mid = string.Empty;
            foreach (Cookie cookie in dominatorAccount?.Cookies)
            {
                if (cookie.Name == "test_cookie" || cookie.Name == "fr")
                    continue;
                cookie_Container.Add(new Cookie { Name = cookie.Name, Value = cookie.Value, Domain = cookie.Domain });
                if (cookie.Name == "csrftoken")
                    csrf = cookie.Value;
                if (MobileRequest)
                {
                    if (cookie.Name == "sessionid")
                        s = cookie.Value;
                    if (cookie.Name == "ds_user_id")
                        dsuid = cookie.Value;
                    if (cookie.Name == "mid")
                        mid = cookie.Value;
                }
            }
            var httpClient1 = new HttpClientHandler
            {
                UseCookies = true,
                CookieContainer = cookie_Container,
                AllowAutoRedirect = true,
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };
            if (!string.IsNullOrEmpty(proxy?.ProxyIp) && !string.IsNullOrEmpty(proxy?.ProxyPort))
            {
                var webProxy = new WebProxy($"http://{proxy.ProxyIp}:{proxy.ProxyPort}");
                if (!string.IsNullOrEmpty(proxy?.ProxyUsername) && !string.IsNullOrEmpty(proxy?.ProxyPassword))
                {
                    webProxy.Credentials = new NetworkCredential(proxy.ProxyUsername, proxy.ProxyPassword);
                }
                httpClient1.Proxy = webProxy;
                httpClient1.UseProxy = true;
            }
            return new IGWebParameter
            {
                CsrfToken = csrf,
                DsUserId = MobileRequest ? dsuid :string.Empty,
                MID = MobileRequest ? mid:string.Empty,
                Authorization = MobileRequest ? $"Bearer IGT:2:{GramStatic.GetValidBase64(Convert.ToBase64String(Encoding.UTF8.GetBytes($"{{\"ds_user_id\":\"{dsuid}\",\"sessionid\":\"{s}\"}}")))}" :string.Empty,
                cookieContainer = cookie_Container,
                X_IG_Claim = x_ig_claim,
                Jazoest = GramStatic.CreateJazoest(dominatorAccount?.DeviceDetails?.PhoneId),
                httpClient = httpClient1
            };
        }

        public async Task<UploadMediaResponse> StoryUpload(DominatorAccountModel dominatorAccountModel, InstagramPost instagramPost, string StoryMedia)
        {
            try
            {
                var param = GetWebParameter(dominatorAccountModel, true);
                var mediaInfo = GramStatic.GetMediaInfo(StoryMedia);
                var uploaded = false;
                var IsVideo = mediaInfo.FormatName.Contains("mp4")
                    || mediaInfo.FormatName.Contains("m4a");
                var uploadID = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
                if (IsVideo)
                {
                    var url = $"https://i.instagram.com/rupload_igvideo/story_{uploadID}";
                    using (var client = new HttpClient())
                    using (var request = new HttpRequestMessage(HttpMethod.Options, url))
                    {
                        // Headers for preflight OPTIONS
                        request.Headers.Add("Host", "i.instagram.com");
                        request.Headers.Add("Connection", "keep-alive");
                        request.Headers.Add("Accept", "*/*");
                        request.Headers.Add("Access-Control-Request-Method", "POST");
                        request.Headers.Add("Access-Control-Request-Headers", "content-type,offset,x-entity-length,x-entity-name,x-ig-app-id,x-instagram-rupload-params");
                        request.Headers.Add("Origin", "https://www.instagram.com");
                        request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/138.0.0.0 Safari/537.36");
                        request.Headers.Add("Sec-Fetch-Mode", "cors");
                        request.Headers.Add("Sec-Fetch-Site", "same-site");
                        request.Headers.Add("Sec-Fetch-Dest", "empty");
                        request.Headers.Add("Referer", "https://www.instagram.com/");
                        request.Headers.Add("Accept-Encoding", GramStatic.AcceptEncoding);
                        request.Headers.Add("Accept-Language", "en-GB,en-US;q=0.9,en;q=0.8");
                        // Send the OPTIONS request
                        var response = await client.SendAsync(request);
                        var result = await response.Content.ReadAsStringAsync();
                    }
                    var url2 = $"https://i.instagram.com/rupload_igvideo/story_{uploadID}";

                    byte[] videoBytes = await GetMediaData(StoryMedia);

                    using (var client = new HttpClient(param.httpClient))
                    using (var content = new ByteArrayContent(videoBytes))
                    using (var request = new HttpRequestMessage(HttpMethod.Post, url2))
                    {
                        // Content Headers
                        content.Headers.ContentType = new MediaTypeHeaderValue("video/mp4");
                        content.Headers.ContentLength = videoBytes.Length;

                        request.Content = content;

                        // Custom Headers
                        var ruploadParams = new
                        {
                            is_sidecar = 0,
                            media_type = 2,
                            client_passthrough = 1,
                            video_edit_params = new { muted = false },
                            upload_id = uploadID,
                            for_album = true,
                            is_unified_video = 0
                        };

                        request.Headers.Add("Host", "i.instagram.com");
                        request.Headers.Add("Connection", "keep-alive");
                        request.Headers.Add("x-instagram-rupload-params", JsonConvert.SerializeObject(ruploadParams));
                        request.Headers.Add("offset", "0");
                        request.Headers.Add("x-entity-length", videoBytes.Length.ToString());
                        request.Headers.Add("x-entity-name", $"story_{uploadID}");
                        request.Headers.Add("x-ig-app-id", "936619743392459");
                        request.Headers.Add("accept", "*/*");
                        request.Headers.Add("Origin", "https://www.instagram.com");
                        request.Headers.Add("Referer", "https://www.instagram.com/");
                        request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/138.0.0.0 Safari/537.36");
                        request.Headers.Add("Accept-Encoding", GramStatic.AcceptEncoding);
                        request.Headers.Add("Accept-Language", "en-GB,en-US;q=0.9,en;q=0.8");
                        request.Headers.Add("sec-ch-ua", "\"Not)A;Brand\";v=\"8\", \"Chromium\";v=\"138\", \"Google Chrome\";v=\"138\"");
                        request.Headers.Add("sec-ch-ua-platform", "\"Windows\"");
                        request.Headers.Add("sec-ch-ua-mobile", "?0");
                        request.Headers.Add("Sec-Fetch-Site", "same-site");
                        request.Headers.Add("Sec-Fetch-Mode", "cors");
                        request.Headers.Add("Sec-Fetch-Dest", "empty");
                        var response = await client.SendAsync(request);
                        var ResponseData = await HttpHelper.Decode(response);
                        if (!string.IsNullOrEmpty(ResponseData?.Response)
                            && ResponseData.Response.Contains("\"status\":\"ok\""))
                        {
                            var thumbnailImage = new MediaUtilites().GetThumbnail(StoryMedia);
                            uploaded = await UploadStory(uploadID, thumbnailImage, param.httpClient, 2);
                        }
                    }
                }
                else
                {
                    uploaded = await UploadStory(uploadID, StoryMedia, param.httpClient, 1);
                }
                if (uploaded)
                {
                    var FailedCount = 0;
                TryAgain:
                    param = GetWebParameter(dominatorAccountModel, true);
                    var url = "https://www.instagram.com/api/v1/web/create/configure_to_story/";
                    // This is URL-encoded JSON
                    var postData = $"upload_id={uploadID}";
                    using (var client = new HttpClient(param.httpClient))
                    using (var request = new HttpRequestMessage(HttpMethod.Post, url))
                    {
                        request.Content = new StringContent(postData);
                        request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

                        // Headers (some values may need to be updated with real session/csrf tokens)
                        request.Headers.Add("Host", "www.instagram.com");
                        request.Headers.Add("Connection", "keep-alive");
                        request.Headers.Add("x-csrftoken", param.CsrfToken); // must match session
                        request.Headers.Add("viewport-width", "1745");
                        request.Headers.Add("sec-ch-ua", "\"Not)A;Brand\";v=\"8\", \"Chromium\";v=\"138\", \"Google Chrome\";v=\"138\"");
                        request.Headers.Add("sec-ch-ua-platform", "\"Windows\"");
                        request.Headers.Add("sec-ch-ua-mobile", "?0");
                        request.Headers.Add("x-ig-app-id", "936619743392459");
                        request.Headers.Add("dpr", "1.1");
                        request.Headers.Add("sec-ch-prefers-color-scheme", "light");
                        request.Headers.Add("accept", "*/*");
                        request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/138.0.0.0 Safari/537.36");
                        request.Headers.Add("sec-ch-ua-model", "\"\"");
                        request.Headers.Add("sec-ch-ua-platform-version", "\"10.0.0\"");
                        request.Headers.Add("Origin", "https://www.instagram.com");
                        request.Headers.Add("Sec-Fetch-Site", "same-origin");
                        request.Headers.Add("Sec-Fetch-Mode", "cors");
                        request.Headers.Add("Sec-Fetch-Dest", "empty");
                        request.Headers.Add("Referer", "https://www.instagram.com/");
                        request.Headers.Add("Accept-Encoding", GramStatic.AcceptEncoding);
                        request.Headers.Add("Accept-Language", "en-GB,en-US;q=0.9,en;q=0.8");
                        var response = await client.SendAsync(request);
                        var ResponseData = await HttpHelper.Decode(response);
                        while (FailedCount++ <= 3 && !string.IsNullOrEmpty(ResponseData?.Response)
                            && (ResponseData.Response.Contains("\"message\":\"Transcode not finished yet.\"")))
                        {
                            await Task.Delay(TimeSpan.FromSeconds(5), dominatorAccountModel.Token);
                            goto TryAgain;
                        }
                        return new UploadMediaResponse(ResponseData, string.Empty, true);
                    }
                }
                return null;
            }
            catch { return null; }
        }

        public async Task<bool> UploadStory(string MediaID,string Media, HttpClientHandler httpClientHandler,int MediaType=1)
        {
            try
            {
                var url = $"https://i.instagram.com/rupload_igphoto/story_{MediaID}";
                using (var httpClient = new HttpClient())
                {
                    var request = new HttpRequestMessage(HttpMethod.Options, url);
                    // Required headers
                    request.Headers.Add("Host", "i.instagram.com");
                    request.Headers.Add("Connection", "keep-alive");
                    request.Headers.Add("Accept", "*/*");
                    request.Headers.Add("Access-Control-Request-Method", "POST");
                    request.Headers.Add("Access-Control-Request-Headers", "content-type,offset,x-entity-length,x-entity-name,x-ig-app-id,x-instagram-rupload-params");
                    request.Headers.Add("Origin", "https://www.instagram.com");
                    request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/138.0.0.0 Safari/537.36");
                    request.Headers.Add("Sec-Fetch-Mode", "cors");
                    request.Headers.Add("Sec-Fetch-Site", "same-site");
                    request.Headers.Add("Sec-Fetch-Dest", "empty");
                    request.Headers.Add("Referer", "https://www.instagram.com/");
                    request.Headers.Add("Accept-Encoding", GramStatic.AcceptEncoding);
                    request.Headers.Add("Accept-Language", "en-GB,en-US;q=0.9,en;q=0.8");
                    // Send the request
                    var response = await httpClient.SendAsync(request);
                    var result = await response.Content.ReadAsStringAsync();
                }
                var imageData = await GetMediaData(Media);
                using (var client = new HttpClient(httpClientHandler))
                using (var content = new ByteArrayContent(imageData))
                using (var request = new HttpRequestMessage(HttpMethod.Post, url))
                {
                    // Set content headers
                    content.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
                    content.Headers.ContentLength = imageData.Length;

                    request.Content = content;
                    var width = MediaType == 1 ? "4000" : "720";
                    var height = MediaType == 1 ? "4000" : "1280";
                    // Set custom headers
                    request.Headers.Host = "i.instagram.com";
                    request.Headers.ConnectionClose = false;
                    request.Headers.Add("x-instagram-rupload-params",
                        $"{{\"upload_id\":\"{MediaID}\",\"media_type\":{MediaType},\"upload_media_width\":{width},\"upload_media_height\":{height}}}");
                    request.Headers.Add("offset", "0");
                    request.Headers.Add("x-entity-length", imageData.Length.ToString());
                    request.Headers.Add("x-entity-name", $"story_{MediaID}");
                    request.Headers.Add("x-ig-app-id", "936619743392459");
                    request.Headers.Add("accept", "*/*");
                    request.Headers.Add("Origin", "https://www.instagram.com");
                    request.Headers.Add("Referer", "https://www.instagram.com/");
                    request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/138.0.0.0 Safari/537.36");
                    request.Headers.Add("Accept-Encoding", GramStatic.AcceptEncoding);
                    request.Headers.Add("Accept-Language", "en-GB,en-US;q=0.9,en;q=0.8");
                    request.Headers.Add("sec-ch-ua", "\"Not)A;Brand\";v=\"8\", \"Chromium\";v=\"138\", \"Google Chrome\";v=\"138\"");
                    request.Headers.Add("sec-ch-ua-platform", "\"Windows\"");
                    request.Headers.Add("sec-ch-ua-mobile", "?0");
                    request.Headers.Add("Sec-Fetch-Site", "same-site");
                    request.Headers.Add("Sec-Fetch-Mode", "cors");
                    request.Headers.Add("Sec-Fetch-Dest", "empty");
                    // Send request
                    var response = await client.SendAsync(request);
                    var ResponseData = await HttpHelper.Decode(response);
                    return !string.IsNullOrEmpty(ResponseData?.Response)
                        && ResponseData.Response.Contains("\"status\":\"ok\"");
                }
            }
            catch { return false; }
        }

        #region Web Login

        public async Task<WebLoginIgResponseHandler> IGWebLogin(DominatorAccountModel dominatorAccount)
        {
            WebLoginIgResponseHandler webLoginIgResponse = null;
            try
            {
                var param = GetWebParameter(dominatorAccount);
                var LoginData = await PasswordEncryptionProcess.GetPreLoginData();
                using (var client = new HttpClient(param.httpClient))
                {
                    var url = "https://www.instagram.com/api/v1/web/accounts/login/ajax/";
                    var pwd = await PasswordEncryptionProcess.EncryptInstagramPassword(dominatorAccount.AccountBaseModel.Password,LoginData);
                    // Form data
                    var postData = $"enc_password={pwd}" +
                                    $"&caaF2DebugGroup=0&isPrivacyPortalReq=false&loginAttemptSubmissionCount=0&optIntoOneTap=false&queryParams=%7B%7D&trustedDeviceRecords=%7B%7D&username={dominatorAccount.AccountBaseModel.UserName}&jazoest={GramStatic.CreateJazoest(dominatorAccount?.DeviceDetails?.PhoneId)}";

                    var request = new HttpRequestMessage(HttpMethod.Post, url)
                    {
                        Content = new StringContent(postData, Encoding.UTF8, "application/x-www-form-urlencoded")
                    };

                    // Headers
                    request.Headers.Add("Host", "www.instagram.com");
                    request.Headers.Add("Connection", "keep-alive");
                    request.Headers.Add("sec-ch-ua-full-version-list", "\"Not)A;Brand\";v=\"8.0.0.0\", \"Chromium\";v=\"138.0.7204.96\", \"Google Chrome\";v=\"138.0.7204.96\"");
                    request.Headers.Add("sec-ch-ua-platform", "\"Windows\"");
                    request.Headers.Add("sec-ch-ua", "\"Not)A;Brand\";v=\"8\", \"Chromium\";v=\"138\", \"Google Chrome\";v=\"138\"");
                    request.Headers.Add("sec-ch-ua-model", "\"\"");
                    request.Headers.Add("sec-ch-ua-mobile", "?0");
                    request.Headers.Add("X-IG-App-ID", "936619743392459");
                    request.Headers.Add("X-Requested-With", "XMLHttpRequest");
                    request.Headers.Add("X-Instagram-AJAX", GramStatic.InstagramAjax());
                    request.Headers.Add("X-CSRFToken", LoginData.CsrfToken); // Must match cookie
                    request.Headers.Add("X-Web-Device-Id", LoginData.WebDeviceID);
                    request.Headers.Add("X-Web-Session-ID",GramStatic.GetWebSessionID());
                    request.Headers.Add("X-ASBD-ID", "359341");
                    request.Headers.Add("sec-ch-prefers-color-scheme", "light");
                    request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/138.0.0.0 Safari/537.36");
                    request.Headers.Add("X-IG-WWW-Claim", "0");
                    request.Headers.Add("sec-ch-ua-platform-version", "\"10.0.0\"");
                    request.Headers.Add("Origin", "https://www.instagram.com");
                    request.Headers.Add("Sec-Fetch-Site", "same-origin");
                    request.Headers.Add("Sec-Fetch-Mode", "cors");
                    request.Headers.Add("Sec-Fetch-Dest", "empty");
                    request.Headers.Add("Referer", "https://www.instagram.com/");
                    request.Headers.Add("Accept-Encoding", GramStatic.AcceptEncoding);
                    request.Headers.Add("Accept-Language", "en-GB,en;q=0.9");
                    var response = await client.SendAsync(request);
                    var ResponseData = await HttpHelper.Decode(response);
                    webLoginIgResponse = new WebLoginIgResponseHandler(ResponseData);
                    if (webLoginIgResponse.IsLogged)
                    {
                        dominatorAccount.DeviceDetails.IGXClaim = response.Headers.GetValues("x-ig-www-claim")?.FirstOrDefault();
                        dominatorAccount.Cookies = param.GetCookies();
                        param = GetWebParameter(dominatorAccount);
                        if (webLoginIgResponse.OneTapPrompt)
                        {
                            using (var client1 = new HttpClient(param.httpClient))
                            {
                                var uri = new Uri("https://www.instagram.com/accounts/onetap/?next=%2F");

                                var request1 = new HttpRequestMessage(HttpMethod.Get, uri);

                                // Add headers
                                request1.Headers.Add("Host", "www.instagram.com");
                                request1.Headers.Add("Connection", "keep-alive");
                                request1.Headers.Add("dpr", "1.1");
                                request1.Headers.Add("viewport-width", "1745");
                                request1.Headers.Add("sec-ch-ua", "\"Not)A;Brand\";v=\"8\", \"Chromium\";v=\"138\", \"Google Chrome\";v=\"138\"");
                                request1.Headers.Add("sec-ch-ua-mobile", "?0");
                                request1.Headers.Add("sec-ch-ua-platform", "\"Windows\"");
                                request1.Headers.Add("sec-ch-ua-platform-version", "\"10.0.0\"");
                                request1.Headers.Add("sec-ch-ua-model", "\"\"");
                                request1.Headers.Add("sec-ch-ua-full-version-list", "\"Not)A;Brand\";v=\"8.0.0.0\", \"Chromium\";v=\"138.0.7204.96\", \"Google Chrome\";v=\"138.0.7204.96\"");
                                request1.Headers.Add("sec-ch-prefers-color-scheme", "light");
                                request1.Headers.Add("Upgrade-Insecure-Requests", "1");
                                request1.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/138.0.0.0 Safari/537.36");
                                request1.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7");
                                request1.Headers.Add("Sec-Fetch-Site", "same-origin");
                                request1.Headers.Add("Sec-Fetch-Mode", "navigate");
                                request1.Headers.Add("Sec-Fetch-Dest", "document");
                                request1.Headers.Add("Referer", "https://www.instagram.com/");
                                request1.Headers.Add("Accept-Encoding", GramStatic.AcceptEncoding);
                                request1.Headers.Add("Accept-Language", "en-GB,en;q=0.9");
                                var response1 = await client1.SendAsync(request1);
                                var responseText1 = await HttpHelper.Decode(response1);
                            }
                            using (var client2 = new HttpClient(param.httpClient))
                            {
                                var uri = new Uri("https://www.instagram.com/api/v1/web/accounts/request_one_tap_login_nonce/");

                                var request2 = new HttpRequestMessage(HttpMethod.Post, uri)
                                {
                                    Content = new StringContent(string.Empty, Encoding.UTF8, "application/x-www-form-urlencoded")
                                };

                                // Headers
                                request2.Headers.Add("Host", "www.instagram.com");
                                request2.Headers.Add("Connection", "keep-alive");
                                request2.Headers.Add("sec-ch-ua-full-version-list", "\"Not)A;Brand\";v=\"8.0.0.0\", \"Chromium\";v=\"138.0.7204.96\", \"Google Chrome\";v=\"138.0.7204.96\"");
                                request2.Headers.Add("sec-ch-ua-platform", "\"Windows\"");
                                request2.Headers.Add("sec-ch-ua", "\"Not)A;Brand\";v=\"8\", \"Chromium\";v=\"138\", \"Google Chrome\";v=\"138\"");
                                request2.Headers.Add("sec-ch-ua-model", "\"\"");
                                request2.Headers.Add("sec-ch-ua-mobile", "?0");
                                request2.Headers.Add("X-IG-App-ID", "936619743392459");
                                request2.Headers.Add("X-Requested-With", "XMLHttpRequest");
                                request2.Headers.Add("Accept", "*/*");
                                request2.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                                request2.Headers.Add("X-Instagram-AJAX", GramStatic.InstagramAjax());
                                request2.Headers.Add("X-CSRFToken", "D3pvBRxrdS8aDpqmhzUndx"); // must match Cookie
                                request2.Headers.Add("X-Web-Session-ID", GramStatic.GetWebSessionID());
                                request2.Headers.Add("X-ASBD-ID", "359341");
                                request2.Headers.Add("sec-ch-prefers-color-scheme", "light");
                                request2.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/138.0.0.0 Safari/537.36");
                                request2.Headers.Add("X-IG-WWW-Claim", dominatorAccount?.DeviceDetails?.IGXClaim);
                                request2.Headers.Add("sec-ch-ua-platform-version", "\"10.0.0\"");
                                request2.Headers.Add("Origin", "https://www.instagram.com");
                                request2.Headers.Add("Sec-Fetch-Site", "same-origin");
                                request2.Headers.Add("Sec-Fetch-Mode", "cors");
                                request2.Headers.Add("Sec-Fetch-Dest", "empty");
                                request2.Headers.Add("Referer", "https://www.instagram.com/accounts/onetap/?next=%2F");
                                request2.Headers.Add("Accept-Encoding", GramStatic.AcceptEncoding);
                                request2.Headers.Add("Accept-Language", "en-GB,en;q=0.9");
                                var response2 = await client2.SendAsync(request2);
                                var LoginResponse = await HttpHelper.Decode(response2);
                                if(LoginResponse != null && !string.IsNullOrEmpty(LoginResponse.Response) && LoginResponse.Response.Contains("\"status\":\"ok\""))
                                {
                                    dominatorAccount.LoginNonce = Utilities.GetBetween(LoginResponse.Response, "\"login_nonce\":\"", "\"");
                                    dominatorAccount.Cookies = param.GetCookies();
                                    dominatorAccount = await AssignProfileID(dominatorAccount);
                                }
                            }
                        }
                        else
                        {
                            dominatorAccount = await AssignProfileID(dominatorAccount);
                        }
                    }
                }
            }
            catch { }
            return webLoginIgResponse;
        }

        public async Task<DominatorAccountModel> AssignProfileID(DominatorAccountModel dominatorAccount)
        {
            try
            {
                var param = GetWebParameter(dominatorAccount);
                using (var client = new HttpClient(param.httpClient))
                {
                    var uri = new Uri("https://www.instagram.com/");
                    var request = new HttpRequestMessage(HttpMethod.Get, uri);
                    // Add all headers
                    request.Headers.Add("Host", "www.instagram.com");
                    request.Headers.Add("Connection", "keep-alive");
                    request.Headers.Add("dpr", "1.1");
                    request.Headers.Add("viewport-width", "1745");
                    request.Headers.Add("sec-ch-ua", "\"Not)A;Brand\";v=\"8\", \"Chromium\";v=\"138\", \"Google Chrome\";v=\"138\"");
                    request.Headers.Add("sec-ch-ua-mobile", "?0");
                    request.Headers.Add("sec-ch-ua-platform", "\"Windows\"");
                    request.Headers.Add("sec-ch-ua-platform-version", "\"10.0.0\"");
                    request.Headers.Add("sec-ch-ua-model", "\"\"");
                    request.Headers.Add("sec-ch-ua-full-version-list", "\"Not)A;Brand\";v=\"8.0.0.0\", \"Chromium\";v=\"138.0.7204.96\", \"Google Chrome\";v=\"138.0.7204.96\"");
                    request.Headers.Add("sec-ch-prefers-color-scheme", "light");
                    request.Headers.Add("Upgrade-Insecure-Requests", "1");
                    request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/138.0.0.0 Safari/537.36");
                    request.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7");
                    request.Headers.Add("Sec-Fetch-Site", "same-origin");
                    request.Headers.Add("Sec-Fetch-Mode", "navigate");
                    request.Headers.Add("Sec-Fetch-User", "?1");
                    request.Headers.Add("Sec-Fetch-Dest", "document");
                    request.Headers.Add("Referer", "https://www.instagram.com/accounts/onetap/?next=%2F");
                    request.Headers.Add("Accept-Encoding", GramStatic.AcceptEncoding);
                    request.Headers.Add("Accept-Language", "en-GB,en;q=0.9");
                    var response = await client.SendAsync(request);
                    var LoginResponse = await HttpHelper.Decode(response);
                    dominatorAccount.AccountBaseModel.ProfileId = Utilities.GetBetween(LoginResponse?.Response, "\"username\":\"", "\"");
                }
            }
            catch { }
            return dominatorAccount;
        }

        #endregion
    }
}