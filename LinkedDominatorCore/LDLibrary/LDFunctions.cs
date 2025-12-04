using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using DominatorHouseCore;
using DominatorHouseCore.EmailService;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Request;
using DominatorHouseCore.Utility;
using EmbeddedBrowser;
using LinkedDominatorCore.DetailedInfo;
using LinkedDominatorCore.Interfaces;
using LinkedDominatorCore.LDModel;
using LinkedDominatorCore.LDUtility;
using LinkedDominatorCore.Request;
using LinkedDominatorCore.Response;
using LinkedDominatorCore.Utility;
using Newtonsoft.Json;
using UserScraperModel = LinkedDominatorCore.LDModel.SalesNavigatorScraper.UserScraperModel;

// ReSharper disable once IdentifierTypo
namespace LinkedDominatorCore.LDLibrary
{
    public interface ILdFunctions : IBrowserManager
    {
        bool IsBrowser { get; }
        BrowserWindow BrowserWindow { get; set; }
        string PreLoginResponse(CancellationToken cancellationToken, bool isSalesNavigator);

        //IResponseParameter HandleGetResponse(string postUrl);
        Task<LoginResponseHandler> Login(CancellationToken cancellationToken, string challengeid = "",
            string actionUrls = "", params object[] OptionalValues);

        // IResponseParameter HandlePostResponse(string actionUrlOrId, string postStringOrNodeId);
        IResponseParameter SendConnectionRequestAlternativeMethod(string postString, string actionUrl,
            string flagship3ProfileViewBase);

        IResponseParameter SendConnectionRequestWithoutVistingProfile(string postString, string actionUrl,
            string flagship3ProfileViewBase);

        IResponseParameter FollowComapanyPages(string postString, string actionUrl);

        //  string ShortPress(string url, string postStringOrNodeId);
        string GetUploadResponse(string actionUrl, string encryptionId, string encryption, FileInfo objFileInfo);
        string Getputresponse(string url, byte[] imageByte);
        string GetputresponseforDocPdfFiles(string url, byte[] FileByte,string FileContentType);
        string RemoveConnection(LinkedinUser linkedInUser);
        string WithDrawConnectionRequest(LinkedinUser linkedInUser);

        string ByteEncodedPostResponse(string actionUrl, string postString);


        string Share(string actionUrl, string postStringOrNodeId, string composeIconId = null);
        string Like(string actionUrl, string postStringOrNodeId);
        string Comment(string actionUrlOrComment, string postStringOrNodeId, string url = "", string queryType = "");

        string UploadImageAndGetContentIdForMessaging(string imageUploadUrl, FileInfo fileInfo,
            string message = "",List<string> Medias = null);


        string NormalMessageProcess(List<string> imageSource, SenderDetails objLinkedinUser,
            string postString, string finalMessage);
        string BroadCastMessage(string ImageSource, LinkedinUser linkedinUser, bool IsGroup,string FinalMessage, string OriginToken,List<string> Medias=null);
        string GetSingleUploadResponse(string singleUploadUrl, FileInfo objFileInfo, string referer);
        string FinalPostRequest(bool isPublishOnOwnWall, string actionUrl, string postString, string dFlagshipFeed, string title);
        string FinalPostRequest_VideoUploading(string actionUrl, string postString);
        string FinalPostRequest_DocumentUploading(string actionUrl, string postString, string dFlagship3Feed);
        string GetDocfileUploadResponseStatus(string actionUrl, string dFlagship3Feed);
        string SendGroupJoinRequest(string actionUrl);
        string SendGreeting(string actionUrlOrId, string postDataOrMessage);
        string GroupJoinRequest(string groupid);
        string SendGroupInviter(string actionUrlOrId, string postDataOrMessage);
        string UnJoinGroupsRequest(string actionUrl, string postString);
        string SendPageInvitationRequest(string actionUrl, string postString);
        Task<EventInviteResponseHandler> SendEventInvitation(DominatorAccountModel dominatorAccount, string ActionUrl, string PostString);
        SearchLinkedinUsersResponseHandler SearchForLinkedinUsers(string actionUrl, string dFlagshipSearchSrpPeople);
        SearchLinkedinUsersResponseHandler SalesNavigatorLinkedinUsersByKeyword(string actionUrl);
        SearchLinkedinUsersResponseHandler SalesNavigatorLinkedinUsersFromSearchUrl(string actionUrl, string sessionId);
        PageSearchResponseHandler LinkedinPageResponseByKeyword(string actionurl);

        Task<SearchConnectionResponseHandler> SearchForLinkedinConnectionsAsync(string actionUrl,
            CancellationToken cancellationToken);

        SearchConnectionResponseHandler SearchForLinkedinConnections(string actionUrl);

        SearchConnectionResponseHandler SearchForLinkedinConnections(string actionUrl, bool isReplyToAllMessagesChecked,
            string specificWords, bool isReplyToAllUsersWhodidnotReply, string userId);

        PostsResponseHandler SearchForLinkedinPosts(string actionUrl, ActivityType activityType,
            string currentAccountProfileId, string publicIdentifier, List<string> lstCommentInDom);
        PostsResponseHandler SearchPostByKeywordResponseHandler(string actionUrl, ActivityType activityType,
            string currentAccountProfileId, string publicIdentifier, List<string> lstCommentInDom);
        NotificationDetailsResponseHandler GetNotificationDetails(string actionUrl);
        ReceivedInvitationsResponseHandler SearchForLinkedinInvitations(string actionUrl);
        AllPendingConnectionRequestResponseHandler AllPendingConnectionRequest(string actionUrl);

        Task<AllPendingConnectionRequestResponseHandler> AllPendingConnectionRequest(string actionUrl,
            CancellationToken cancellationToken);

        LinkedinGroupsSearchResponseHandler SearchForLinkedinGroups(string actionUrl);

        Task<MyGroupsResponseHandler> SearchForMyGroups(string actionUrl, bool onlyJoinedGroups,
            CancellationToken cancellationToken);

        Task<MyPageResponseHandler> SearchForMyPages(string actionUrl, bool onlyJoinedGroups,
            CancellationToken cancellationToken);

        LinkedinGroupMemberResponseHandler GetGroupMemberInfo(string actionUrl);
        Task<OwnFollowerResponseHandler> GetAccountsFollowers(DominatorAccountModel dominatorAccount, string actionUrl);
        // SalesNavigatorDetailsResponseHandler GetSalesNavigatorProfileDetails(LDModel.SalesNavigatorScraper.UserScraperModel salesNavigatorUserScraperModel, string profileUrl, LdFunctions objLdFunctions);
        SalesNavigatorDetailsResponseHandler GetSalesNavigatorProfileDetails(
            UserScraperModel salesNavigatorUserScraperModel, string profileUrl,
            LinkedinUser linkedinUser);

        SalesNavigatorDetailsResponseHandler GetSalesNavigatorCompanyDetails(string companyDetailsactionUrl,
            string companyUrl);

        SalesNavigatorProfileDetails GetSalesNavigatorProfileDetails(string profileUrl);

        LiveChatUsermessagesResponseHandler UserLiveChatMessage(DominatorAccountModel linkedInAccount,
            SenderDetails senderdetail, ILdFunctions ldFunction);

        LiveChatMessageUserdetailResponseHandler LivechatUserMessage(DominatorAccountModel linkedInAccount,
            long nextPage, ILdFunctions ldFunction);

        string GetHtmlFromUrlForMobileRequest(string actionUrl, string flagshipProfileViewBase);


        string GetHtmlFromUrlNormalMobileRequest(string actionUrl);
        string GetSecondaryBrowserResponse(string actionUrl, int scollDown = 0);

        string GetVideoUploadResponseStatus(string actionUrl);
        JobSearchResponseHandler JobSearch(string actionUrl);

        //string GetHtmlFromUrlSalesNavigatorMobileRequest(string actionUrlOrId);
        //void SetRequestParametersAndProxy_MobileLogin();

        string GetRequestUpdatedUserAgent(string url, bool isDecode = false);

        
        CompanySearchResponseHandler CompanySearch(string actionUrl, bool isSalesNavigator,
            bool isUpdateCsrfToken = false);

        // void ExtraPreLoginHits();
        // string PremiumAccessRequest(string actionUrlOrId, string postStringOrNodeId);
        Task<string> WebLogin();
        GroupJoinerResponseHandler CheckGroupMemberShip(string actionUrl);
        IResponseParameter BlockUserResponse(string blockActionUrl);
        IResponseParameter DeleteUserMessagesResponse(string actionUrl);

        void SetCookieAndProxy(DominatorAccountModel dominatorAccountModel, IHttpHelper httpHelper,
            BrowserInstanceType browserInstanceType = BrowserInstanceType.Primary);

        void SetJobCancellationTokenInBrowser(DominatorAccountModel dominatorAccountModel,
            CancellationToken cancellationToken);

        void SetWebRequestParametersforjobsearchurl(string referer, string dFlagship3SearchSrpPeople);
        void SetWebRequestParametersforCaptcha(string referer);
        void SetWenRequestparamtersForsalesUrl(string referer,bool IsSalesWebRequest=false);
        void SetWebRequestParametersforChangingProfilePic(string referer, string dFlagship3SearchSrpPeople);
        void SetRequestParametersAndProxy_MobileLogin(bool IsLogin=false);
        void SetRequestParameterForUploadVideo(string referer, string dFlagship3SearchSrpPeople);
        void BrowserMailverfication(string pin);
        void GoBrowserBack();
        IHttpHelper GetInnerHttpHelper();
        ILdHttpHelper GetInnerLdHttpHelper();

        string MessageDetails(string url);

        //  void LoginWithWeb_SetRequestParametersAndProxy(DominatorAccountModel dominatorAccountModel);
        string TryAndGetResponse(string Url, string NotContainString = "", int TryCount = 2);
        string CommentWithAlternateMethod(IDetailsFetcher detailsFetcher, LinkedinPost objLinkedinPost,UserScraperDetailedInfo userScraperDetailedInfo,string comment, string querytype);
        string CommentOnCustomPost(string Comment, string PostUrl);
        #region verification

        IResponseParameter SendResetPasswordLinkToMail(IResponseParameter loginResponseParameter);
        void ReadVerificationCodeFromEmail(DominatorAccountModel accountModel);
        Task<string> RedirectChallenge(DominatorAccountModel dominatorAccount, LoginResponseHandler loginResponseHandler);

        #endregion
        Task<ScrapePostResponseHandler> ScrapeKeywordPost(DominatorAccountModel dominatorAccount,string QueryValue, int PaginationCount = 0);
        string GetMediaIDs(List<string> imageSource);
        void NavigateSalesProfile();
        bool IsCookieExist(CookieCollection Cookies, string CookieNameOrValue);
        Task<string> GenerateAIPrompt(DominatorAccountModel dominatorAccount, string QueryValue);
        void SetDominatorAccount(DominatorAccountModel dominatorAccount);
    }

    public class LdFunctions : ILdFunctions
    {
        private readonly LdDataHelper _ldDataHelper= LdDataHelper.GetInstance;

        // if ILdHttpHelper encapsulated (means private and make it accessible using GetInnerHttpHelper)
        // if ILdHttpHelper is public  it will not be null in BaseLinkedinProcessor constructor
        // it will arise a problem setting same cookies in all accounts when run multiple accounts
        // conclusion : keep ILdHttpHelper protected so that it will accessible within child
        // since serviceLocator container is register in 'new HierarchicalLifetimeManager()'

        protected readonly ILdHttpHelper HttpHelper;
        private DominatorAccountModel _linkedInAccountModel;
        private readonly ILDAccountSessionManager sessionManager;
        public LdFunctions(DominatorAccountModel dominatorAccountModel, ILdHttpHelper httpHelper, ILDAccountSessionManager accountSessionManager)
        {
            try
            {
                HttpHelper = httpHelper;
                if (dominatorAccountModel.AccountBaseModel?.AccountId != null)
                    _linkedInAccountModel = dominatorAccountModel;
                sessionManager = accountSessionManager;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public bool IsBrowser { get; }
        public BrowserWindow BrowserWindow { get; set; }


        public void SetCookieAndProxy(DominatorAccountModel dominatorAccountModel, IHttpHelper httpHelper,
            BrowserInstanceType browserInstanceType = BrowserInstanceType.Primary)
        {
            // sometimes not setting the cookies
            // if already set cookies to ObjLdRequestParameters no need to set again
            if (string.IsNullOrEmpty(_linkedInAccountModel?.AccountId))
                _linkedInAccountModel = dominatorAccountModel;

            var objLdRequestParameters = httpHelper.GetRequestParameter();

            if (objLdRequestParameters.Cookies == null)
                objLdRequestParameters.Cookies = new CookieCollection();
            if (dominatorAccountModel.Cookies.Count > 0 /*&& objLdRequestParameters.Cookies?.Count == 0*/)
                objLdRequestParameters.Cookies = dominatorAccountModel.Cookies;
            if (dominatorAccountModel.AccountBaseModel.AccountProxy != null)
                objLdRequestParameters.Proxy = dominatorAccountModel.AccountBaseModel.AccountProxy;
            httpHelper.SetRequestParameter(objLdRequestParameters);
        }

        #region Requests Implemented With Mobile Device

        public string PreLoginResponse(CancellationToken cancellationToken, bool isSalesNavigator)
        {
            string response;
            try
            {
                //    ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                                                       | SecurityProtocolType.Tls11
                                                       | SecurityProtocolType.Tls12
                                                       | SecurityProtocolType.Ssl3;
                SetRequestParametersAndProxy_MobileLogin();
                response = HttpUtility.HtmlDecode(HttpHelper.HandleGetResponse("https://www.linkedin.com/feed/").Response);
                //response= HttpUtility.HtmlDecode(HttpHelper.HandleGetResponse("https://www.linkedin.com/mob/sso/you").Response);
                // var check = linkedInAccountModel.HttpHelper.GetRequest("https://whatismyipaddress.com/");
            }
            catch (Exception ex)
            {
                response = null;
                ex.DebugLog();
            }

            return response;
        }

        public async Task<LoginResponseHandler> Login(CancellationToken cancellationToken, string challengeId = "",
            string actionUrls = "", params object[] OptionalValues)
        {
            try
            {
                var objLdRequestParameters = (LdRequestParameters) HttpHelper.GetRequestParameter();
                AddCsrfToken(objLdRequestParameters);
                var actionUrl = string.IsNullOrEmpty(actionUrls)
                    ? //"https://www.linkedin.com/uas/authenticate?nc=" + Utils.GenerateNc()
                    $"https://www.linkedin.com/uas/authenticate"
                    : actionUrls;

                // Requirements for Post
                var ajax = Uri.EscapeDataString(_ldDataHelper.GetFullCsrfTokenFromCookies(objLdRequestParameters));
                var bCookie = Uri.EscapeDataString(_ldDataHelper.GetbcookieFromCookies(objLdRequestParameters));
                var bsCookie = Uri.EscapeDataString(_ldDataHelper.GetbscookieFromCookies(objLdRequestParameters));
                var lidc = Uri.EscapeDataString(_ldDataHelper.GetlidcFromCookies(objLdRequestParameters));
                var li_rm = OptionalValues.Length > 0 ? OptionalValues.FirstOrDefault() as string:string.Empty;
                if (OptionalValues.Length > 0 && !objLdRequestParameters.Cookies.Cast<Cookie>().Any(x => x.Name == "li_rm"))
                    li_rm = OptionalValues.FirstOrDefault() as string;
                else
                    li_rm = objLdRequestParameters.Cookies.Cast<Cookie>().FirstOrDefault(y => y.Name == "li_rm")?.Value;
                li_rm=!string.IsNullOrEmpty(li_rm)? Uri.EscapeDataString(li_rm):string.Empty;
                #region Actual Dynamic Post 

                LdJsonElements jsonElements;

                if (string.IsNullOrEmpty(challengeId))
                    jsonElements = new LdJsonElements
                    {
                        SessionKey = Uri.EscapeDataString(_linkedInAccountModel.AccountBaseModel.UserName.Trim()),
                        SessionPassword = Uri.EscapeDataString(_linkedInAccountModel.AccountBaseModel.Password.Trim()),
                        ClientEnabledFeatures = "ANDROID_NATIVE_CAPTCHA",
                        Jsessionid = ajax,
                        Bcookie = bCookie,
                        LIRM=li_rm,
                        RememberMe=true,
                        Bscookie = bsCookie,
                        Lang = "v%3D2%26lang%3Den_US",
                        Lidc = lidc
                    };
                else
                    jsonElements = new LdJsonElements
                    {
                        SessionKey = Uri.EscapeDataString(_linkedInAccountModel.AccountBaseModel.UserName.Trim()),
                        SessionPassword = Uri.EscapeDataString(_linkedInAccountModel.AccountBaseModel.Password.Trim()),
                        Jsessionid = ajax,
                        Bcookie = bCookie,
                        Bscookie = bsCookie,
                        LIRM = li_rm,
                        RememberMe = true,
                        Lang = "v%3D2%26lang%3Den_US",
                        Lidc = lidc,
                        ChallengeId = challengeId,
                        ClientEnabledFeatures = "ANDROID_NATIVE_CAPTCHA"
                    };
                #endregion

                #region Modify Cookies
                var cookieCollection = new CookieCollection();

                try
                {
                    objLdRequestParameters.Cookies.Cast<Cookie>().ForEach(item =>
                    {
                        if (item != null)
                        {
                            if (item.Value.Contains("\""))
                                item.Value = item.Value.Replace("\"", "");
                            cookieCollection.Add(item);
                        }
                    });
                    objLdRequestParameters.Cookies = cookieCollection;
                    HttpHelper.SetRequestParameter(objLdRequestParameters);
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                #endregion

                SetRequestParametersAndProxy_MobileLogin(true);
                objLdRequestParameters.IsLoginPostRequest = true;
                objLdRequestParameters.Body = jsonElements;

                var postData = objLdRequestParameters.GenerateBody();

                return new LoginResponseHandler(
                    await HttpHelper.PostRequestAsync(actionUrl, postData, cancellationToken));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        /// <summary>
        ///     here we are setting headers if it is not sending request in normal way
        /// </summary>
        /// <param name="postString"></param>
        /// <param name="actionUrl"></param>
        /// <param name="flagship3ProfileViewBase"></param>
        /// <returns></returns>
        public IResponseParameter SendConnectionRequestAlternativeMethod(string postString, string actionUrl,
            string flagship3ProfileViewBase)
        {
            var oldXlipageinstance = string.Empty;
            IResponseParameter respParam = null;
            try
            {
                if (HttpHelper.GetRequestParameter().Headers.ToString().Contains("X-li-page-instance") &&
                    !string.IsNullOrEmpty(flagship3ProfileViewBase))
                {
                    oldXlipageinstance = HttpHelper.GetRequestParameter().Headers["X-li-page-instance"];
                    HttpHelper.GetRequestParameter().Headers.Remove("X-li-page-instance");
                    HttpHelper.GetRequestParameter().Headers.Add("X-li-page-instance",
                        "urn:li:page:d_flagship3_profile_view_base;" + flagship3ProfileViewBase);
                    HttpHelper.GetRequestParameter().Accept = "application/vnd.linkedin.normalized+json+2.1";
                    HttpHelper.GetRequestParameter().ContentType = "application/json; charset=UTF-8";
                }

                //sending request
                respParam = HttpHelper.HandlePostResponse(actionUrl, postString);

                HttpHelper.GetRequestParameter().Headers.Remove("X-li-page-instance");
                HttpHelper.GetRequestParameter().Headers.Add("X-li-page-instance", oldXlipageinstance);
                HttpHelper.GetRequestParameter().Accept = null;
            }
            catch (Exception ex)
            {
                HttpHelper.GetRequestParameter().Headers.Remove("X-li-page-instance");
                HttpHelper.GetRequestParameter().Headers.Add("X-li-page-instance", oldXlipageinstance);
                HttpHelper.GetRequestParameter().Accept = null;
                ex.DebugLog();
            }

            return respParam;
        }

        public IResponseParameter SendConnectionRequestWithoutVistingProfile(string postString, string actionUrl,
            string flagship3ProfileViewBase)
        {
            return null;
        }

        public string Getputresponse(string url,byte[] imageByte)
        {
            try
            {
                var objLdRequestParameters = HttpHelper.GetRequestParameter();
                objLdRequestParameters.AddHeader("media-type-family", "VIDEO");
                objLdRequestParameters.ContentType = "video/mp4";
                objLdRequestParameters.Accept = "*/*";
                var response = HttpHelper.PutRequest(url, imageByte).Response;
                objLdRequestParameters.Headers.Remove("media-type-family");
                objLdRequestParameters.Accept = "application/vnd.linkedin.normalized+json+2.1";
                return response;
            }
            catch (Exception)
            {
                return null;
                throw;
            }
        }

        /// <summary>
        /// This Method is use to upload doc and pdf file format
        /// </summary>
        /// <param name="url"></param>
        /// <param name="FileByte"></param>
        /// <returns></returns>
        public string GetputresponseforDocPdfFiles(string url, byte[] FileByte,string FileContentType)
        {
            try
            {
                var objLdRequestParameters = HttpHelper.GetRequestParameter();
                objLdRequestParameters.AddHeader("media-type-family", "PAGINATEDDOCUMENT");
                //objLdRequestParameters.ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                objLdRequestParameters.ContentType = FileContentType;
                objLdRequestParameters.Accept = "*/*";
                var response = HttpHelper.PutRequest(url, FileByte).Response;
                objLdRequestParameters.Headers.Remove("media-type-family");
                objLdRequestParameters.Accept = LdConstants.AcceptApplicationOrVndLinkedInMobileDedupedJson21;
                return response;
            }
            catch (Exception)
            {
                return null;
                throw;
            }
        }
        // unit test cases start from here
        public string GetUploadResponse(string actionUrl, string encryptionId, string encryption, FileInfo objFileInfo)
        {
            try
            {
                #region Headers Set

                if (!string.IsNullOrEmpty(encryptionId) && !HttpHelper.GetRequestParameter().Headers.ToString()
                        .Contains("x-amz-server-side-encryption-aws-kms-key-id") &&
                    !HttpHelper.GetRequestParameter().Headers.ToString().Contains("x-amz-server-side-encryption"))
                {
                    HttpHelper.GetRequestParameter().Headers
                        .Add("x-amz-server-side-encryption-aws-kms-key-id", encryptionId);
                    HttpHelper.GetRequestParameter().Headers.Add("x-amz-server-side-encryption", encryption);
                }

                HttpHelper.GetRequestParameter().Accept = "*/*";
                HttpHelper.GetRequestParameter().ContentType = "application/octet-stream";

                #endregion

                var list = JsonConvert.DeserializeObject<List<string>>(actionUrl);
                var imageByte = File.ReadAllBytes(objFileInfo.FullName.Replace("_SOCINATORIMAGE.jpg", ""));

                return HttpHelper.PutRequest(list[0], imageByte).Response;
            }
            catch (Exception ex)
            {
                HttpHelper.GetRequestParameter().Headers.Remove("x-amz-server-side-encryption-aws-kms-key-id");
                HttpHelper.GetRequestParameter().Headers.Remove("x-amz-server-side-encryption");
                HttpHelper.GetRequestParameter().ContentType = "application/x-www-form-urlencoded";
                HttpHelper.GetRequestParameter().Accept = null;
                HttpHelper.GetRequestParameter().Referer = null;
                ex.DebugLog();
                return null;
            }
        }

        public string RemoveConnection(LinkedinUser linkedInUser)
        {
            try
            {
                //https://www.linkedin.com/voyager/api/relationships/connections/ACoAAAUcyzcBp_ZRtul99tBhsXtZxUERvubFTQE
                var actionUrl =
                    $"https://www.linkedin.com/voyager/api/relationships/connections/{linkedInUser.ProfileId}";

                return HttpHelper.DeleteRequest(actionUrl).Response;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public string WithDrawConnectionRequest(LinkedinUser linkedInUser)
        {
            //$"https://www.linkedin.com/voyager/actionUrlOrId/relationships/invitations/{linkedInUser.InvitationId}?action=withdraw&nc={Utils.GenerateNc()}";
            var actionUrl =
                $"https://www.linkedin.com/voyager/api/relationships/invitations?action=closeInvitations&nc={Utils.GenerateNc()}";
            return GetInnerLdHttpHelper().HandlePostResponse(actionUrl, linkedInUser.PostData)?.Response;
        }

        public string ByteEncodedPostResponse(string actionUrl, string postString)
        {
            try
            {
                return HttpHelper.HandlePostResponse(actionUrl, postString)?.Response;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }


        public string UploadImageAndGetContentIdForMessaging(string imageUploadUrl, FileInfo fileInfo,
            string message = "",List<string> Medias=null)
        {
            try
            {
                var post = "{\"mediaUploadType\":\"MESSAGING_PHOTO_ATTACHMENT\",\"fileSize\":" + fileInfo.Length +
                           ",\"filename\":\"" + fileInfo.Name + "\"}";
                var postData = Encoding.UTF8.GetBytes(post);
                var imageUploadResponse = HttpHelper.PostRequest(imageUploadUrl, postData).Response;
                return imageUploadResponse;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public string NormalMessageProcess(List<string> imageSource, SenderDetails objLinkedinUser,
            string postString, string finalMessage)
        {
            
            var actionUrl = "https://www.linkedin.com/voyager/api/messaging/conversations?action=create";

            if (imageSource.Count > 0)
            {
                Tuple<bool, string> imageUploadStatusAndPostData = null;
                //    JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                imageUploadStatusAndPostData =
                    GetPostDataToSendMessage(objLinkedinUser.SenderId, finalMessage, imageSource);
                postString = imageUploadStatusAndPostData.Item2;
                finalMessage = $"{finalMessage}<:>{imageSource}";
            }
            else
            {
                #region PostSting and Requirements

                try
                {
                    finalMessage = finalMessage.Replace("\r\n", "\\n").Replace("\"", "\\\"");
                    postString =
                        "{\"conversationCreate\":{\"eventCreate\":{\"value\":{\"com.linkedin.voyager.messaging.create.MessageCreate\":{\"body\":\"" +
                        finalMessage + "\",\"attachments\":[]}}},\"recipients\":[\"" + objLinkedinUser.SenderId +
                        "\"],\"subtype\":\"MEMBER_TO_MEMBER\",\"name\":\"\"}}";
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                #endregion
            }


            //JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            return GetInnerLdHttpHelper().HandlePostResponse(actionUrl, postString)?.Response;
        }


        private Tuple<bool, string> GetPostDataToSendMessage(string profileId, string finalMessage,
            List<string> imageSource)
        {
            try
            {
                #region PostData And PostDataResponse For Message Sending

                #region PhotoUploading
                var MediasIds = GetMediaIDs(imageSource);
                #endregion
                var postData =
                    "{\"conversationCreate\":{\"eventCreate\":{\"value\":{\"com.linkedin.voyager.messaging.create.MessageCreate\":{\"body\":\"" +
                    finalMessage + "\",\"attachments\":[" + MediasIds +
                    "]}}},\"recipients\":[\"" + profileId +
                    "\"],\"subtype\":\"MEMBER_TO_MEMBER\"},\"keyVersion\":\"LEGACY_INBOX\"}";
                return new Tuple<bool, string>(true, postData);

                #endregion
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException("Operation Cancelled!");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return new Tuple<bool, string>(false, "");
            }
        }

        public string GetMediaIDs(List<string> imageSource)
        {
            try
            {
                var imageUploadUrl = LdConstants.GetLDMediaUploadAPI;
                var singleUploadUrl = string.Empty;
                var jsonElements = new MediaPostJsonElement
                {
                    JsonElements = new MediaPostJsonElement[imageSource.Count]
                };
                var referer = "https://www.linkedin.com/messaging/compose/";

                foreach (var item in imageSource)
                {
                    var objFileInfo = new FileInfo(item);
                    var imageUploadResponse = UploadImageAndGetContentIdForMessaging(imageUploadUrl, objFileInfo);
                    if (imageUploadResponse == null || !imageUploadResponse.Contains("{\"value\":{\"urn\":\""))
                        return string.Empty;
                    // success = true;
                    var mediaUrn = Utils.GetBetween(imageUploadResponse, "urn\":\"", "\"");
                    singleUploadUrl = Utils.GetBetween(imageUploadResponse, "singleUploadUrl\":\"", "\"");
                    var singleUploadResponse = GetSingleUploadResponse(singleUploadUrl, objFileInfo, referer);

                    if (singleUploadResponse == null)
                        return string.Empty;
                    var contentType = Utils.GetMediaType(objFileInfo.Extension);
                    var mediaId = mediaUrn.Split(':').Last();

                    jsonElements.JsonElements[imageSource.IndexOf(item)] = new MediaPostJsonElement
                    {
                        Id = mediaUrn,
                        OriginalId = mediaUrn,
                        Name = objFileInfo.Name,
                        ByteSize = objFileInfo.Length,
                        MediaType = contentType,
                        Reference = new MediaPostJsonElement
                        {
                            String = $"blob:https://www.linkedin.com/{mediaId}"
                        }
                    };
                }

                var data = new LdRequestParameters().GenerateMediaUploadJson(jsonElements);
                return Utils.GetBetween(data, "jsonElements\":[", "]");
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public string GetSingleUploadResponse(string singleUploadUrl, FileInfo objFileInfo, string referer)
        {
            string oldContentType = null;
            string oldAccept = null;
            string oldReferrer = null;
            try
            {
                try
                {
                    oldContentType = HttpHelper.GetRequestParameter().ContentType;
                    oldAccept = HttpHelper.GetRequestParameter().Accept;
                    oldReferrer = HttpHelper.GetRequestParameter().Referer;
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                var contentType = Utils.GetMediaType(objFileInfo.Extension);

                if (oldContentType != null && !oldContentType.Contains("image/"))
                    HttpHelper.GetRequestParameter().ContentType = contentType;
                if (string.IsNullOrEmpty(oldAccept))
                    HttpHelper.GetRequestParameter().Accept = "*/*";
                if (string.IsNullOrEmpty(oldReferrer))
                    HttpHelper.GetRequestParameter().Referer = referer;
                if (!HttpHelper.GetRequestParameter().Headers.ToString()
                    .Contains(HttpRequestHeader.ContentLength.ToString()))
                    HttpHelper.GetRequestParameter().Headers
                        .Add(HttpRequestHeader.ContentLength, objFileInfo.Length.ToString());
                HttpHelper.GetRequestParameter().Headers.Remove("media-type-family");
                HttpHelper.GetRequestParameter().Headers.Add("media-type-family", "STILLIMAGE");
                HttpHelper.GetRequestParameter().Headers.Remove("Content-Length");
                var imageByte = File.ReadAllBytes(objFileInfo.FullName);
                var uploadResponse = HttpHelper.PutRequest(singleUploadUrl, imageByte).Response;
                HttpHelper.GetRequestParameter().ContentType = oldContentType;
                HttpHelper.GetRequestParameter().Accept = oldAccept;
                HttpHelper.GetRequestParameter().Referer = oldReferrer;
                HttpHelper.GetRequestParameter().Headers.Remove(HttpRequestHeader.ContentLength.ToString());
                return uploadResponse;
            }
            catch (Exception ex)
            {
                HttpHelper.GetRequestParameter().ContentType = oldContentType;
                HttpHelper.GetRequestParameter().Accept = oldAccept;
                HttpHelper.GetRequestParameter().Referer = oldReferrer;
                HttpHelper.GetRequestParameter().Headers.Remove("Content-Length");
                ex.DebugLog();
                return null;
            }
        }

        public string FinalPostRequest(bool isPublishOnOwnWall, string actionUrl, string postString,
            string dFlagship3Feed,string title)
        {
            string response;
            try
            {
                var postData = Encoding.UTF8.GetBytes(postString?.Replace("\n","\\n"));
                try
                {
                    if (HttpHelper.GetRequestParameter().ContentType.Contains("----"))
                        HttpHelper.GetRequestParameter().ContentType = "application/json; charset=UTF-8";
                    HttpHelper.GetRequestParameter().Accept = LdConstants.AcceptApplicationOrVndLinkedInMobileDedupedJson21;

                    if (isPublishOnOwnWall && !HttpHelper.GetRequestParameter().Headers.ToString()
                            .Contains("X-li-page-instance"))
                        HttpHelper.GetRequestParameter().Headers.Add("X-li-page-instance",
                            "urn:li:page:d_flagship3_feed;" + dFlagship3Feed);

                    var responseParameter = HttpHelper.PostRequest(actionUrl, postData);
                    if(responseParameter==null||responseParameter.Exception==null?false:string.IsNullOrEmpty(responseParameter.Exception?.Message)?false:responseParameter.Exception.Message.Contains("Internal Server Error"))
                    {
                        HttpHelper.GetRequestParameter().Headers.Remove("X-LI-Track");
                        responseParameter= HttpHelper.PostRequest(actionUrl, postData);
                    }
                    response = responseParameter.HasError
                        ? responseParameter.Exception.Message
                        : responseParameter.Response;
                    HttpHelper.GetRequestParameter().Accept = null;
                    HttpHelper.GetRequestParameter().Headers.Remove("X-li-page-instance");
                }
                catch (Exception ex)
                {
                    HttpHelper.GetRequestParameter().Accept = null;
                    HttpHelper.GetRequestParameter().Headers.Remove("X-li-page-instance");
                    ex.DebugLog();
                    response = null;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                response = null;
            }

            return response;
        }


        public string FinalPostRequest_VideoUploading(string actionUrl, string postString)
        {
            string response;
            var reqParams = HttpHelper.GetRequestParameter();
            try
            {
                var postData = Encoding.UTF8.GetBytes(postString);
                try
                {
                    response = HttpHelper.PostRequest(actionUrl, postData).Response;
                }
                catch (Exception ex)
                {
                    HttpHelper.GetRequestParameter().ContentType = "application/x-www-form-urlencoded";
                    reqParams.Accept = null;
                    reqParams.Referer = null;
                    HttpHelper.GetRequestParameter().Headers.Remove("X-li-page-instance");
                    ex.DebugLog();
                    response = null;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                response = null;
            }

            return response;
        }
        public string FinalPostRequest_DocumentUploading(string actionUrl, string postString,string dFlagship3Feed)
        {
            string response;
            var reqParams = HttpHelper.GetRequestParameter();
            try
            {
                var postData = Encoding.UTF8.GetBytes(postString);
                try
                {
                    IsCsrfChanged();
                    reqParams.Referer = "https://www.linkedin.com/feed/";
                    reqParams.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/94.0.4606.81 Safari/537.36";
                    reqParams.Accept = LdConstants.AcceptApplicationOrVndLinkedInMobileDedupedJson21;
                    reqParams.ContentType = "application/json; charset=UTF-8";
                    reqParams.AddHeader("X-LI-Track",
                    "{\"clientVersion\":\"1.9.5787\",\"mpVersion\":\"1.9.5787\",\"osName\":\"web\",\"timezoneOffset\":5.5,\"timezone\":\"Asia/Calcutta\",\"deviceFormFactor\":\"DESKTOP\",\"mpName\":\"voyager-web\",\"displayDensity\":0.75,\"displayWidth\":1024.5,\"displayHeight\":576}");
                    if (!reqParams.Headers.ToString().Contains("X-li-page-instance"))
                        reqParams.Headers.Add("X-li-page-instance", "urn:li:page:d_flagship3_feed;" + dFlagship3Feed);
                    
                    // sending postData
                    response = HttpHelper.PostRequest(actionUrl, postString).Response;

                    HttpHelper.GetRequestParameter().ContentType = "application/x-www-form-urlencoded";
                    reqParams.Accept = null;
                    reqParams.Referer = null;
                    HttpHelper.GetRequestParameter().Headers.Remove("X-li-page-instance");
                }
                catch (Exception ex)
                {
                    HttpHelper.GetRequestParameter().ContentType = "application/x-www-form-urlencoded";
                    reqParams.Accept = null;
                    reqParams.Referer = null;
                    HttpHelper.GetRequestParameter().Headers.Remove("X-li-page-instance");
                    ex.DebugLog();
                    response = null;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                response = null;
            }

            return response;
        }
        public string GetDocfileUploadResponseStatus(string actionUrl, string dFlagship3Feed)
        {
            string response;
            var reqParams = HttpHelper.GetRequestParameter();
            try
            {
                IsCsrfChanged();
                reqParams.Referer = "https://www.linkedin.com/feed/";
                reqParams.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/94.0.4606.81 Safari/537.36";
                reqParams.Accept = "application/vnd.linkedin.normalized+json+2.1";
                reqParams.ContentType = "application/json; charset=UTF-8";
                reqParams.AddHeader("X-LI-Track",
                "{\"clientVersion\":\"1.9.5787\",\"mpVersion\":\"1.9.5787\",\"osName\":\"web\",\"timezoneOffset\":5.5,\"timezone\":\"Asia/Calcutta\",\"deviceFormFactor\":\"DESKTOP\",\"mpName\":\"voyager-web\",\"displayDensity\":0.75,\"displayWidth\":1024.5,\"displayHeight\":576}");
                if (!reqParams.Headers.ToString().Contains("X-li-page-instance"))
                    reqParams.Headers.Add("X-li-page-instance", "urn:li:page:d_flagship3_feed;" + dFlagship3Feed);
                response = HttpHelper.GetRequest(actionUrl).Response;
                if (!string.IsNullOrEmpty(response))
                    response = HttpUtility.HtmlDecode(response);
            }
            catch (Exception ex)
            {
                HttpHelper.GetRequestParameter().ContentType = "application/x-www-form-urlencoded";
                reqParams.Accept = null;
                reqParams.Referer = null;
                HttpHelper.GetRequestParameter().Headers.Remove("X-li-page-instance");
                ex.DebugLog();
                return null;
            }

            return response;
        }
        public string SendGroupJoinRequest(string actionUrl)
        {
            var response = string.Empty;
            var postString = "{\"actionType\":\"SEND_REQUEST\",\"groupUrn\":\"urn:li:fsd_group:10408911\",\"profileUrn\":\"urn:li:fsd_profile:\"" + _linkedInAccountModel.AccountBaseModel.UserId + "\"}";
            var postData = Encoding.Default.GetBytes(postString);

            try
            {
                response = HttpHelper.PostRequest(actionUrl, postData).Response;
                if (string.IsNullOrEmpty(response))
                    return "";
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return response;
        }
        public string GroupJoinRequest(string groupId)
        {
            var actionUrl = LdConstants.GetGroupJoinAPI;
            var response = string.Empty;
            var postString = LdConstants.GetGroupJoinPostData(groupId,_linkedInAccountModel.AccountBaseModel.UserId);
            var postData = Encoding.Default.GetBytes(postString);
            try
            {
                response = HttpHelper.PostRequest(actionUrl, postData).Response;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return string.IsNullOrEmpty(response)?string.Empty:response;
        }
        public string UnJoinGroupsRequest(string actionUrl, string postString)
        {
            string response;
            try
            {
                var postData = Encoding.Default.GetBytes(postString);
                response = HttpHelper.PostRequest(actionUrl, postData).Response;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                response = "Error In Post Request";
            }
            
            return response;
        }

        public string SendPageInvitationRequest(string actionUrl, string postString)
        {
            string response;
            try
            {
                var postData = Encoding.Default.GetBytes(postString);
                var reqParam = HttpHelper.GetRequestParameter();
                reqParam.ContentType = LdConstants.AcceptApplicationOrJson;
                reqParam.Headers["X-UDID"] = "d0da3223-5b25-4c75-904b-2013fc0d640e";
                reqParam.Headers["X-RestLi-Protocol-Version"] = "2.0.0";
                reqParam.Headers["Accept-Language"] = "en-US";
                reqParam.Headers["X-RestLi-Method"] = "batch_create";
                reqParam.Headers["X-li-page-instance"] = $"urn:li:page:p_flagship3_event_send_invitation;{Utils.GenerateTrackingId()}";
                reqParam.Headers["X-LI-PEM-Metadata"] = "Voyager - Invitations - Actions=invite-batch-send,Voyager - Organization - Admin=organization-admin-invite-to-follow";
                reqParam.Headers["x-restli-symbol-table-name"] = "voyager-20141";
                reqParam.Headers["X-LI-Lang"] = "en_US";
                reqParam.Headers["Accept-Encoding"] = "gzip, deflate";
                reqParam.Accept = reqParam.ContentType;
                HttpHelper.SetRequestParameter(reqParam);
                response = HttpHelper.PostRequest(actionUrl, postData).Response;
                response = response == null || (!string.IsNullOrEmpty(response) && response.Contains("{\"elements\":[]}")) ? "There is no option found in this page or already sent invitation" : response;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                response = "Error In Post Request";
            }

            return response;
        }

        public SearchLinkedinUsersResponseHandler SearchForLinkedinUsers(string actionUrl,
            string dFlagshipSearchSrpPeople)
        {
            try
            {
                var reqParams = HttpHelper.GetRequestParameter();
                AddCsrfToken(reqParams);
                reqParams.Accept = LdConstants.AcceptApplicationOrJson;
                reqParams.ContentType = LdConstants.AcceptApplicationOrJson;
                if (reqParams.Headers.ToString().Contains("POST"))
                    reqParams.Headers["Method"]="GET";
                HttpHelper.SetRequestParameter(reqParams);
                var response = HttpHelper.HandleGetResponse(actionUrl);
                if (response.Response == null)
                {
                    reqParams.Accept = LdConstants.AcceptApplicationOrJson;
                    reqParams.ContentType = LdConstants.AcceptApplicationOrJson;
                    response = HttpHelper.HandleGetResponse(actionUrl);
                    reqParams.Accept = LdConstants.AcceptApplicationOrVndLinkedInMobileDedupedJson;
                }
                return new SearchLinkedinUsersResponseHandler(response);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
            finally
            {
                try
                {
                    HttpHelper.GetRequestParameter().UserAgent = LdConstants.UserAgent;
                }
                catch (Exception exception)
                {
                    exception.DebugLog();
                }
            }
        }

        public LiveChatMessageUserdetailResponseHandler LivechatUserMessage(DominatorAccountModel linkedInAccount,
            long nextPage, ILdFunctions ldFunction)
        {
            string createdBefore;
            try
            {
                var getInboxDetailsUrl =
                    "https://www.linkedin.com/voyager/api/messaging/conversations?keyVersion=LEGACY_INBOX";
                if (nextPage == 0)
                {
                    getInboxDetailsUrl =
                        "https://www.linkedin.com/voyager/api/messaging/conversations?keyVersion=LEGACY_INBOX";
                }
                else
                {
                    createdBefore = (nextPage + 89900000).ToString();
                    getInboxDetailsUrl = getInboxDetailsUrl + "&createdBefore=" + createdBefore;
                }

                AddCsrfToken(HttpHelper.GetRequestParameter());
                return new LiveChatMessageUserdetailResponseHandler(HttpHelper.GetRequest(getInboxDetailsUrl));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }
        // ReSharper disable once UnusedMember.Local


        public SearchLinkedinUsersResponseHandler SalesNavigatorLinkedinUsersByKeyword(string actionUrl)
        {
            try
            {
                SetWenRequestparamtersForsalesUrl("",true);
                return new SearchLinkedinUsersResponseHandler(HttpHelper.GetRequest(actionUrl), true);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        //  SearchLinkedinUsersResponseHandler  for sales navigator, search url
        public SearchLinkedinUsersResponseHandler SalesNavigatorLinkedinUsersFromSearchUrl(string actionUrl,
            string sessionId)
        {
            try
            {
                IsCsrfChanged();
                return new SearchLinkedinUsersResponseHandler(HttpHelper.GetRequest(actionUrl), true, sessionId);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public PageSearchResponseHandler LinkedinPageResponseByKeyword(string actionUrl)
        {
            try
            {
                IsCsrfChanged();
                return new PageSearchResponseHandler(HttpHelper.GetRequest(actionUrl));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public async Task<SearchConnectionResponseHandler> SearchForLinkedinConnectionsAsync(string actionUrl,
            CancellationToken cancellationToken)
        {
            try
            {
                AddCsrfToken(HttpHelper.GetRequestParameter());
                return new SearchConnectionResponseHandler(
                    await HttpHelper.GetRequestAsync(actionUrl, cancellationToken));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public SearchConnectionResponseHandler SearchForLinkedinConnections(string actionUrl)
        {
            try
            {
                AddCsrfToken(HttpHelper.GetRequestParameter());
                return new SearchConnectionResponseHandler(HttpHelper.GetRequest(actionUrl));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public SearchConnectionResponseHandler SearchForLinkedinConnections(string actionUrl,
            bool isReplyToAllMessagesChecked, string specificWords, bool isReplyToAllUsersWhodidnotReply, string userId)
        {
            try
            {
                AddCsrfToken(HttpHelper.GetRequestParameter());
                return new SearchConnectionResponseHandler(HttpHelper.GetRequest(actionUrl),
                    isReplyToAllMessagesChecked, specificWords, isReplyToAllUsersWhodidnotReply, userId);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }


        public PostsResponseHandler SearchForLinkedinPosts(string actionUrl, ActivityType activityType,
            string currentAccountProfileId, string publicIdentifier, List<string> lstCommentInDom)
        {
            try
            {
                AddCsrfToken(HttpHelper.GetRequestParameter());
                if (HttpHelper.GetRequestParameter().Referer != null)
                    return new PostsResponseHandler(HttpHelper.GetRequest(actionUrl), activityType,
                        currentAccountProfileId, lstCommentInDom);
                if (HttpHelper.GetRequestParameter().Referer == null)
                    HttpHelper.GetRequestParameter().Referer = publicIdentifier.Contains("hastag")?publicIdentifier:
                        "https://www.linkedin.com/in/" + publicIdentifier + "/detail/recent-activity/";

                return new PostsResponseHandler(HttpHelper.GetRequest(actionUrl), activityType, currentAccountProfileId,
                    lstCommentInDom);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public PostsResponseHandler SearchPostByKeywordResponseHandler(string actionUrl, ActivityType activityType,
           string currentAccountProfileId, string publicIdentifier, List<string> lstCommentInDom)
        {
            try
            {
                return new PostsResponseHandler(HttpHelper.GetRequest(actionUrl), activityType, currentAccountProfileId,
                lstCommentInDom);

            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public NotificationDetailsResponseHandler GetNotificationDetails(string actionUrl)
        {
            try
            {
                AddCsrfToken(HttpHelper.GetRequestParameter());
                return new NotificationDetailsResponseHandler(HttpHelper.GetRequest(actionUrl));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public ReceivedInvitationsResponseHandler SearchForLinkedinInvitations(string actionUrl)
        {
            try
            {
                return HttpHelper == null
                    ? null
                    : new ReceivedInvitationsResponseHandler(HttpHelper.GetRequest(actionUrl));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public AllPendingConnectionRequestResponseHandler AllPendingConnectionRequest(string actionUrl)
        {
            try
            {
                return new AllPendingConnectionRequestResponseHandler(HttpHelper.GetRequest(actionUrl));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public async Task<AllPendingConnectionRequestResponseHandler> AllPendingConnectionRequest(string actionUrl,
            CancellationToken cancellationToken)
        {
            try
            {
                return new AllPendingConnectionRequestResponseHandler(
                    await HttpHelper.GetRequestAsync(actionUrl, cancellationToken));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public LinkedinGroupsSearchResponseHandler SearchForLinkedinGroups(string actionUrl)
        {
            try
            {
                if (!HttpHelper.GetRequestParameter().Headers.ToString().Contains("XMLHttpRequest"))
                    return new LinkedinGroupsSearchResponseHandler(HttpHelper.GetRequest(actionUrl));
                HttpHelper.GetRequestParameter().Accept = "null";
                SetRequestParametersAndProxy_MobileLogin();
                return new LinkedinGroupsSearchResponseHandler(HttpHelper.GetRequest(actionUrl));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }


        public LiveChatUsermessagesResponseHandler UserLiveChatMessage(DominatorAccountModel linkedInAccount,
            SenderDetails senderdetail, ILdFunctions ldFunction)
        {
            var sendedetail = senderdetail.ThreadId;
            var RequestUrl = $"https://www.linkedin.com/voyager/api/messaging/conversations/{sendedetail}/events";
            var ResponseHandler =
                new LiveChatUsermessagesResponseHandler(GetInnerLdHttpHelper().HandleGetResponse(RequestUrl),
                    senderdetail.SenderId, ldFunction);
            return ResponseHandler;
        }

        public async Task<MyGroupsResponseHandler> SearchForMyGroups(string actionUrl, bool onlyJoinedGroups,
            CancellationToken cancellationToken)
        {
            var tempMobileUserAgent =
                "{\"osName\":\"Android OS\",\"osVersion\":\"5.1.1\",\"clientVersion\":\"4.1.241\",\"clientMinorVersion\":114900,\"model\":\"samsung_SM-G930K\",\"dpi\":\"hdpi\",\"deviceType\":\"android\",\"appId\":\"com.linkedin.android\",\"deviceId\":\"d3d60930-f554-4201-b68f-5796eb2a657e\",\"timezoneOffset\":8,\"storeId\":\"us_googleplay\",\"isAdTrackingLimited\":false,\"mpName\":\"voyager-android\",\"mpVersion\":\"0.314.54\"}";
            try
            {
                AddCsrfToken(HttpHelper.GetRequestParameter());

                try
                {
                    HttpHelper.GetRequestParameter().Headers.Remove("X-LI-Track");
                    HttpHelper.GetRequestParameter().Headers.Add("X-LI-Track", tempMobileUserAgent);
                }
                catch (Exception ex)
                {
                    ex.ErrorLog();
                }


                return new MyGroupsResponseHandler(await HttpHelper.GetRequestAsync(actionUrl, cancellationToken),
                    onlyJoinedGroups);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
            finally
            {
                try
                {
                    HttpHelper.GetRequestParameter().Headers.Remove("X-LI-Track");
                    HttpHelper.GetRequestParameter().Headers.Add("X-LI-Track", LdConstants.MobileUserAgent); //= accept;
                }
                catch (Exception exception)
                {
                    exception.DebugLog();
                }
            }
        }

        public async Task<MyPageResponseHandler> SearchForMyPages(string actionUrl, bool onlyJoinedGroups,
            CancellationToken cancellationToken)
        {
            var tempMobileUserAgent =
                "{\"osName\":\"Android OS\",\"osVersion\":\"5.1.1\",\"clientVersion\":\"4.1.241\",\"clientMinorVersion\":114900,\"model\":\"samsung_SM-G930K\",\"dpi\":\"hdpi\",\"deviceType\":\"android\",\"appId\":\"com.linkedin.android\",\"deviceId\":\"d3d60930-f554-4201-b68f-5796eb2a657e\",\"timezoneOffset\":8,\"storeId\":\"us_googleplay\",\"isAdTrackingLimited\":false,\"mpName\":\"voyager-android\",\"mpVersion\":\"0.314.54\"}";
            try
            {
                AddCsrfToken(HttpHelper.GetRequestParameter());

                try
                {
                    HttpHelper.GetRequestParameter().Headers.Remove("X-LI-Track");
                    HttpHelper.GetRequestParameter().Headers.Add("X-LI-Track", tempMobileUserAgent);
                }
                catch (Exception ex)
                {
                    ex.ErrorLog();
                }


                return new MyPageResponseHandler(await HttpHelper.GetRequestAsync(actionUrl, cancellationToken));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
            finally
            {
                try
                {
                    HttpHelper.GetRequestParameter().Headers.Remove("X-LI-Track");
                    HttpHelper.GetRequestParameter().Headers.Add("X-LI-Track", LdConstants.MobileUserAgent); //= accept;
                }
                catch (Exception exception)
                {
                    exception.DebugLog();
                }
            }
        }

        public LinkedinGroupMemberResponseHandler GetGroupMemberInfo(string actionUrl)
        {
            try
            {
                return new LinkedinGroupMemberResponseHandler(HttpHelper.GetRequest(actionUrl));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        /// <summary>
        ///     Updated 28 Feb 2019 handler
        ///     get details of scraped user from sales search
        /// </summary>
        /// <param name="salesNavigatorUserScraperModel"></param>
        /// <param name="profileUrl"></param>
        /// <param name="objLdFunctions"></param>
        /// <param name="linkedinUser"></param>
        /// <returns></returns>
        public SalesNavigatorDetailsResponseHandler GetSalesNavigatorProfileDetails(
            UserScraperModel salesNavigatorUserScraperModel, string profileUrl,
            LinkedinUser linkedinUser)
        {
            try
            {
                SetWenRequestparamtersForsalesUrl(profileUrl,true);
                IResponseParameter responseParameter = null;
                if (IsBrowser)
                {
                    responseParameter.Response =
                        _ldDataHelper.GetUserPageResponse(responseParameter.Response, linkedinUser);
                }
                else
                {
                    responseParameter = HttpHelper.GetRequest(profileUrl);
                }
                return new SalesNavigatorDetailsResponseHandler(salesNavigatorUserScraperModel, responseParameter, this,
                    linkedinUser);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }
        public SalesNavigatorDetailsResponseHandler GetSalesNavigatorCompanyDetails(string companyDetailsactionUrl,
            string companyUrl)
        {
            try
            {
                var response = HttpHelper.GetRequest(companyDetailsactionUrl);
                return new SalesNavigatorDetailsResponseHandler(response, companyUrl);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public SalesNavigatorProfileDetails GetSalesNavigatorProfileDetails(string profileUrl)
        {
            try
            {
                return new SalesNavigatorProfileDetails(HttpHelper.WebClientGetRequest(profileUrl));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public string GetHtmlFromUrlForMobileRequest(string actionUrl, string flagship3ProfileViewBase)
        {
            string response;
            var reqParams = HttpHelper.GetRequestParameter();
            try
            {
                if (reqParams.Headers.ToString().Contains("XMLHttpRequest"))
                {
                    reqParams.Accept = "null";
                    SetRequestParametersAndProxy_MobileLogin();
                }
                if (string.IsNullOrEmpty(reqParams.Referer)) reqParams.Referer = "https://www.linkedin.com/";
                if (!reqParams.Headers.ToString().Contains("X-li-page-instance"))
                    reqParams.Headers.Add("X-li-page-instance",
                        "urn:li:page:d_flagship3_profile_view_base;" + flagship3ProfileViewBase);
                else
                    reqParams.Headers.Remove("X-li-page-instance");

                response = HttpHelper.GetRequest(actionUrl).Response;

                if (!string.IsNullOrEmpty(response) && !actionUrl.Contains("/actionUrlOrId/"))
                    response = HttpUtility.HtmlDecode(response);

                //SetRequestParametersAndProxy_GetRequestMobile(this.AccountModel);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
            finally
            {
                try
                {
                    reqParams.UserAgent = LdConstants.UserAgent;
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }

            return response;
        }

        public string GetHtmlFromUrlNormalMobileRequest(string actionUrl)
        {
            string response;
            try
            {
                if (!actionUrl.Contains("http"))
                    actionUrl = "https://" + actionUrl;
                response = HttpHelper.GetRequest(actionUrl).Response;

                if (!string.IsNullOrEmpty(response))
                    response = HttpUtility.HtmlDecode(response);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }

            return response;
        }

        public string GetVideoUploadResponseStatus(string actionUrl)
        {
            string response;
            var reqParams = HttpHelper.GetRequestParameter();
            try
            { 
                response = HttpHelper.GetRequest(actionUrl).Response;
        
                
                if (!string.IsNullOrEmpty(response))
                    response = HttpUtility.HtmlDecode(response);
            }
            catch (Exception ex)
            {
                HttpHelper.GetRequestParameter().ContentType = "application/x-www-form-urlencoded";
                reqParams.Accept = null;
                reqParams.Referer = null;
                HttpHelper.GetRequestParameter().Headers.Remove("X-li-page-instance");
                ex.DebugLog();
                return null;
            }

            return response;
        }


        public JobSearchResponseHandler JobSearch(string actionUrl)
        {
            return new JobSearchResponseHandler(HttpHelper.GetRequest(actionUrl));
        }

        /// <summary>
        ///     csrf token is changed after taking cookies from embedded browser for sales navigator
        /// </summary>
        /// <param name="actionUrl"></param>
        /// <param name="isSalesNavigator"></param>
        /// <param name="isUpdateCsrfToken"></param>
        /// <returns></returns>
        public CompanySearchResponseHandler CompanySearch(string actionUrl, bool isSalesNavigator,
            bool isUpdateCsrfToken = false)
        {
            try
            {
                if (isUpdateCsrfToken)
                    IsCsrfChanged();
                if(isSalesNavigator)
                    SetWenRequestparamtersForsalesUrl("",isSalesNavigator);
                return isSalesNavigator
                    ? new CompanySearchResponseHandler(HttpHelper.GetRequest(actionUrl), true)
                    : new CompanySearchResponseHandler(HttpHelper.GetRequest(actionUrl));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }


        /// <summary>
        ///     setting headers before doing any action
        /// </summary>
        public void SetRequestParametersAndProxy_MobileLogin(bool IsLogin = false)
        {
            try
            {
                var csrfToken = GetCsrfToken();
                var objLdRequestParameters = HttpHelper.GetRequestParameter();
                objLdRequestParameters.Headers.Clear();
                #region Old Request header.
                objLdRequestParameters.AddHeader("Host", "www.linkedin.com");
                objLdRequestParameters.KeepAlive = true;
                objLdRequestParameters.UserAgent = IsLogin ? "ANDROID OS":
                    string.IsNullOrEmpty(_linkedInAccountModel.UserAgentMobile)?
                    LdConstants.UserAgent
                    : _linkedInAccountModel.UserAgentMobile;
                objLdRequestParameters.AddHeader("X-RestLi-Protocol-Version", "2.0.0");
                objLdRequestParameters.AddHeader("Accept-Language", "en-US");
                if (!string.IsNullOrEmpty(csrfToken))
                    objLdRequestParameters.AddHeader("Csrf-Token", csrfToken);
                objLdRequestParameters.ContentType = @"application/x-www-form-urlencoded";

                objLdRequestParameters.AddHeader("X-LI-Lang", "en_US");

                objLdRequestParameters.AddHeader("X-UDID", "d0da3223-5b25-4c75-904b-2013fc0d640e");
                objLdRequestParameters.AddHeader("X-LI-Track",
                    "{\"osName\":\"Android OS\",\"osVersion\":\"6.0\",\"clientVersion\":\"4.1.256\",\"clientMinorVersion\":116400,\"model\":\"LAVA_V23GB\",\"dpi\":\"xhdpi\",\"deviceType\":\"android\",\"appId\":\"com.linkedin.android\",\"deviceId\":\"{}\",\"timezoneOffset\":5,\"storeId\":\"us_googleplay\",\"isAdTrackingLimited\":false,\"mpName\":\"voyager-android\",\"mpVersion\":\"0.329.59\"}");
                if(IsLogin)
                    objLdRequestParameters.AddHeader("X-LI-User-Agent",LdConstants.UserAgent);
                #endregion
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        /// <summary>
        ///     after login first time with sales nav its csrf token is changed
        ///     here we check and update it
        /// </summary>
        /// <returns></returns>
        public void IsCsrfChanged()
        {
            try
            {
                if (!HttpHelper.GetRequestParameter().Headers.ToString().Contains("Csrf-Token"))
                {
                    HttpHelper.GetRequestParameter().Headers.Add("Csrf-Token",
                        HttpHelper.GetRequestParameter().Cookies["JSESSIONID"]?.Value.Replace("\"", ""));
                    return;
                }

                var header = HttpHelper.GetRequestParameter().Headers["Csrf-Token"];
                if (header.Equals(HttpHelper.GetRequestParameter().Cookies["JSESSIONID"]?.Value
                    .Replace("\"", ""))) return;

                HttpHelper.GetRequestParameter().Headers.Remove("Csrf-Token");
                HttpHelper.GetRequestParameter().Headers.Add("Csrf-Token",
                    HttpHelper.GetRequestParameter().Cookies["JSESSIONID"]?.Value.Replace("\"", ""));
            }
            catch (Exception exception)
            {
                exception.DebugLog();
            }
        }

        public void SetWebRequestParametersforjobsearchurl(string referer, string dFlagship3SearchSrpPeople)
        {
            try
            {
                var csrfToken = GetCsrfToken();
                var objLdRequestParameters = HttpHelper.GetRequestParameter();

                objLdRequestParameters.Headers.Clear();

                objLdRequestParameters.AddHeader("Host", "www.linkedin.com");
                objLdRequestParameters.Accept = "application/vnd.linkedin.normalized+json+2.1";
                objLdRequestParameters.KeepAlive = true;
                objLdRequestParameters.UserAgent =
                    "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/76.0.3809.100 Safari/537.36";
                objLdRequestParameters.Referer = referer;
                objLdRequestParameters.AddHeader("X-RestLi-Protocol-Version", "2.0.0");
                objLdRequestParameters.AddHeader("Accept-Language", "en-US");
                if (!string.IsNullOrEmpty(csrfToken))
                    objLdRequestParameters.AddHeader("Csrf-Token", csrfToken);
                objLdRequestParameters.AddHeader("X-LI-Lang", "en_US");
                objLdRequestParameters.AddHeader("x-requested-with", "XMLHttpRequest");
                objLdRequestParameters.AddHeader("X-LI-Track",
                    "{\"clientVersion\":\"1.5.8236.* \",\"osName\":\"web\",\"timezoneOffset\":5.5,\"deviceFormFactor\":\"DESKTOP\",\"mpName\":\"voyager-web\"}");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void SetWebRequestParametersforChangingProfilePic(string referer, string dFlagship3SearchSrpPeople)
        {
            try
            {
                var csrfToken = GetCsrfToken();
                var objLdRequestParameters = HttpHelper.GetRequestParameter();

                objLdRequestParameters.Headers.Clear();

                objLdRequestParameters.AddHeader("Host", "www.linkedin.com");
                objLdRequestParameters.Accept = "application/vnd.linkedin.normalized+json+2.1";
                objLdRequestParameters.KeepAlive = true;
                objLdRequestParameters.UserAgent =
                    "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/76.0.3809.100 Safari/537.36";
                objLdRequestParameters.Referer = referer;
                objLdRequestParameters.AddHeader("x-restli-protocol-version", "2.0.0");
                objLdRequestParameters.AddHeader("Accept-Language", "en-US");
                if (!string.IsNullOrEmpty(csrfToken))
                    objLdRequestParameters.AddHeader("Csrf-Token", csrfToken);
                objLdRequestParameters.ContentType = "application/json; charset=UTF-8";

                objLdRequestParameters.AddHeader("x-li-lang", "en_US");
                objLdRequestParameters.AddHeader("x-li-track",
                    "{\"clientVersion\":\"1.6.8878\",\"osName\":\"web\",\"timezoneOffset\":5.5,\"deviceFormFactor\":\"DESKTOP\",\"mpName\":\"voyager - web\",\"displayDensity\":0.8999999761581421,\"displayWidth\":1439.9999618530273,\"displayHeight\":809.9999785423279}");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void SetWebRequestParametersforCaptcha(string referer)
        {
            var objLdRequestParameters = HttpHelper.GetRequestParameter();
            objLdRequestParameters.Headers.Clear();
            objLdRequestParameters.AddHeader("Host", "www.linkedin.com");
            objLdRequestParameters.Accept =
                "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9";
            objLdRequestParameters.KeepAlive = true;
            objLdRequestParameters.UserAgent =
                "Mozilla/5.0 (Windows NT 6.3; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.138 Safari/537.36";
            objLdRequestParameters.Referer = referer;
            objLdRequestParameters.AddHeader("Cache-Control", "max-age=0");
            objLdRequestParameters.AddHeader("Accept-Language", "en-US");
            objLdRequestParameters.ContentType = "application/x-www-form-urlencoded";
            objLdRequestParameters.AddHeader("Cache-Control", "max-age=0");
            objLdRequestParameters.AddHeader("Upgrade-Insecure-Requests", "1");
            objLdRequestParameters.AddHeader("Sec-Fetch-Site", "same-origin");
            objLdRequestParameters.AddHeader("Sec-Fetch-Mode", "navigate");
            objLdRequestParameters.AddHeader("Sec-Fetch-User", "?1");
            objLdRequestParameters.AddHeader("Sec-Fetch-Dest", "document");
        }


        public void SetWenRequestparamtersForsalesUrl(string refer, bool IsSalesWebRequest = false)
        {
            if (IsSalesWebRequest)
                PerformSalesAccountAuthentication();
            var csrfToken = GetCsrfToken();
            var objLdRequestParameters = HttpHelper.GetRequestParameter();
            objLdRequestParameters.Headers.Clear();
            objLdRequestParameters.ContentType =LdConstants.AcceptApplicationOrJson;
            objLdRequestParameters.AddHeader("X-RestLi-Protocol-Version", "2.0.0");
            objLdRequestParameters.AddHeader("Accept-Language", "en-US");
            objLdRequestParameters.AddHeader("X-LI-Track", "{\"osName\":\"Android OS\",\"osVersion\":\"6.0\",\"clientVersion\":\"4.1.256\",\"clientMinorVersion\":116400,\"model\":\"LAVA_V23GB\",\"dpi\":\"xhdpi\",\"deviceType\":\"android\",\"appId\":\"com.linkedin.android\",\"deviceId\":\"d0da3223-5b25-4c75-904b-2013fc0d640e\",\"timezoneOffset\":5,\"storeId\":\"us_googleplay\",\"isAdTrackingLimited\":false,\"mpName\":\"voyager-android\",\"mpVersion\":\"0.329.59\"}");
            objLdRequestParameters.AddHeader("X-LI-User-Agent",LdConstants.UserAgent);
            objLdRequestParameters.UserAgent = "ANDROID OS";
            if (!string.IsNullOrEmpty(csrfToken) && !IsHeaderExist(objLdRequestParameters.Headers, "Csrf-Token"))
                objLdRequestParameters.AddHeader("Csrf-Token", csrfToken);
            objLdRequestParameters.Accept = "*/*";
            HttpHelper.SetRequestParameter(objLdRequestParameters);
        }
        public bool IsHeaderExist(WebHeaderCollection webHeaderCollection,string HeaderName)
        {
            try
            {
                return !string.IsNullOrEmpty(webHeaderCollection[HeaderName]);
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool IsCookieExist(CookieCollection Cookies,string CookieNameOrValue)
        {
            var ExistCookie = false;
            try
            {
                ExistCookie = Cookies.Cast<Cookie>().Any(cookie => string.Equals(cookie.Name, CookieNameOrValue, StringComparison.OrdinalIgnoreCase) || string.Equals(cookie.Value, CookieNameOrValue, StringComparison.OrdinalIgnoreCase));
                return ExistCookie;
            }
            catch (Exception) { return ExistCookie; }
        }
        private void PerformSalesAccountAuthentication()
        {
            try
            {
                var RequestParam = GetInnerHttpHelper().GetRequestParameter();
                if (IsCookieExist(RequestParam.Cookies, "li_a"))
                    return;
                if(!IsHeaderExist(RequestParam.Headers,"Csrf-Token"))
                    GetInnerHttpHelper().GetRequestParameter().AddHeader("Csrf-Token",GetCsrfToken());
                #region Old SalesAuthenticationAPI.
                //var SalesIdentityResponse = GetInnerHttpHelper().GetRequest(LdConstants.GetSalesUserIdentityAPI).Response;
                //var JsonHandler =JsonJArrayHandler.GetInstance;
                //var JsonObject = JsonHandler.ParseJsonToJObject(SalesIdentityResponse);
                //var Name = JsonHandler.GetJTokenValue(JsonObject, "elements",0,"name");
                //var ContractId = JsonHandler.GetJTokenValue(JsonObject, "elements",0, "agnosticIdentityUnion", "salesCapIdentity", "contractUrn")?.Replace("urn:li:contract:", "");
                //var seatId = JsonHandler.GetJTokenValue(JsonObject, "elements",0, "agnosticIdentityUnion", "salesCapIdentity", "seatUrn")?.Replace("urn:li:seat:", "");
                //var PostDataforAccountAuthentication = LdConstants.GetSalesAccountAuthenticationPostData(Name, ContractId, seatId);
                //var AuthenticationResponse = GetInnerLdHttpHelper().PostRequest(LdConstants.GetSalesUserAuthenticationAPI, PostDataforAccountAuthentication);
                #endregion
                NavigateSalesProfile();
                GetInnerHttpHelper().SetRequestParameter(GetInnerHttpHelper().GetRequestParameter());
            }
            catch (Exception) {}
        }
        public void NavigateSalesProfile()
        {
            try
            {
                var RequestParam = GetInnerHttpHelper().GetRequestParameter();
                if (IsCookieExist(RequestParam.Cookies, "li_a"))
                    return;
                LDAccountsBrowserDetails.GetInstance().StartBrowserLogin(_linkedInAccountModel, _linkedInAccountModel.Token, true, BrowserInstanceType.Primary, sessionManager,
                        LdConstants.GetSalesHomePageUrl,true);
                LDAccountsBrowserDetails.CloseBrowser(_linkedInAccountModel);
                sessionManager.AddOrUpdateSession(ref _linkedInAccountModel);
                RequestParam.Cookies = _linkedInAccountModel.Cookies;
                GetInnerHttpHelper().SetRequestParameter(RequestParam);
            }
            catch
            {
            }
        }
        /// <summary>
        ///     dominatorAccountModel only for salesNav
        /// </summary>
        /// <param name="dominatorAccountModel"></param>
        /// <returns></returns>
        // ReSharper disable once IdentifierTypo
        private string GetCsrfToken(DominatorAccountModel dominatorAccountModel = null)
        {
            var csrfCookieToken = string.Empty;

            try
            {
                //  we get changed csrf token in sales navigator
                if (dominatorAccountModel == null)
                    dominatorAccountModel = _linkedInAccountModel;
                try
                {
                    csrfCookieToken = HttpHelper.GetRequestParameter().Cookies["JSESSIONID"]?.Value.Replace("\"", "");
                }
                catch (Exception)
                {
                    // ignored
                }

                if (string.IsNullOrEmpty(csrfCookieToken))
                    csrfCookieToken = dominatorAccountModel.Cookies["JSESSIONID"]?.Value.Replace("\"", "");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return csrfCookieToken;
        }

        /// <summary>
        ///     sometimes not getting proper response because of useragent
        ///     removing useragent and assigning null
        /// </summary>
        /// <param name="url"></param>
        /// <param name="isDecode"></param>
        public string GetRequestUpdatedUserAgent(string url, bool isDecode = false)
        {
            var response = "";
            var reqParams = HttpHelper.GetRequestParameter();
            try
            {
                response = HttpHelper.HandleGetResponse(url)?.Response;
                if (isDecode && !string.IsNullOrEmpty(response))
                    response = WebUtility.HtmlDecode(response);
            }
            catch (Exception exception)
            {
                exception.DebugLog();
            }

            return response;
        }

        public IResponseParameter BlockUserResponse(string blockActionUrl)
        {
            IResponseParameter responseParameter = null;
            var reqParams = HttpHelper.GetRequestParameter();
            try
            {
                if (!reqParams.Headers.ToString().Contains("X-IsAJAXForm"))
                    reqParams.Headers.Add("X-IsAJAXForm", "1");

                responseParameter = HttpHelper.PostRequest(blockActionUrl, "");
            }
            catch (Exception exception)
            {
                exception.DebugLog();
            }
            finally
            {
                RemoveHeaders(reqParams, "X-IsAJAXForm");
            }

            return responseParameter;
        }

        public IResponseParameter DeleteUserMessagesResponse(string actionUrl)
        {
            return HttpHelper.DeleteRequest(actionUrl);
        }

        #region handy methods

        public void ExtraPreLoginHits()
        {
            try
            {
                // string firstactionUrlConfigurationUrl = "https://www.linkedin.com/voyager/actionUrlOrId/configuration?nc=" + Utils.GenerateNc();
                var jsessionId = new Random().Next(111111, 999999) +
                                 DateTime.Now.GetCurrentEpochTimeMilliSeconds().ToString();
                var reqParams = HttpHelper.GetRequestParameter();
                reqParams.KeepAlive = true;
                reqParams.Headers.Add("X-UDID", "d0da3223-5b25-4c75-904b-2013fc0d640e");
                reqParams.Headers.Add("X-RestLi-Protocol-Version", "2.0.0");
                reqParams.Headers.Add("Accept-Language", "en-US");
                reqParams.Headers.Add("Csrf-Token", $"ajax:{jsessionId}");
                reqParams.Headers.Add("X-li-page-instance",
                    "urn:li:page:p_flagship3_background;" + Utils.GenerateTrackingId());
                reqParams.Headers.Add("X-LI-Track",
                    "{\"osName\":\"Android OS\",\"osVersion\":\"6.0\",\"clientVersion\":\"4.1.256\",\"clientMinorVersion\":116400,\"model\":\"LAVA_V23GB\",\"dpi\":\"xhdpi\",\"deviceType\":\"android\",\"appId\":\"com.linkedin.android\",\"deviceId\":\"d0da3223-5b25-4c75-904b-2013fc0d640e\",\"timezoneOffset\":5,\"storeId\":\"us_googleplay\",\"isAdTrackingLimited\":false,\"mpName\":\"voyager-android\",\"mpVersion\":\"0.329.59\"}");
                reqParams.Headers.Add("X-LI-Lang", "en_US");
                reqParams.Accept = "application/vnd.linkedin.mobile.deduped+json";
                reqParams.UserAgent =
                    "com.linkedin.android/116400 (Linux; U; Android 6.0; en_US; V23GB; Build/MRA58K; Cronet/60.0.3082.0)";
                reqParams.Cookies.Add(new Cookie
                    {Domain = "linkedin.com", Name = "JSESSIONID", Value = $"ajax:{jsessionId}", Path = "/"});
                reqParams.Cookies.Add(new Cookie
                    {Domain = "linkedin.com", Name = "lang", Value = "v=2&lang=en_US", Path = "/"});
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }


        public void SetParametersForSendingConnectionRequest()
        {
            var ldRequestParameters = new LdRequestParameters();
            var objWebHeaderCollection = new WebHeaderCollection();

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            ldRequestParameters.UserAgent =
                "com.linkedin.android/97214 (Linux; U; Android 5.0.1; en_US; GT-I9500; Build/LRX22C; Cronet/60.0.3082.0)";
            objWebHeaderCollection.Add("Host", "www.linkedin.com");
            ldRequestParameters.KeepAlive = true;
            objWebHeaderCollection.Add("X-UDID", "97f7e5e1-cc82-4b8d-bd56-479fed652fd0");
            objWebHeaderCollection.Add("X-RestLi-Protocol-Version", "2.0.0");
            objWebHeaderCollection.Add("Accept-Language", "en-US");
            objWebHeaderCollection.Add("Csrf-Token", _linkedInAccountModel.Cookies["JSESSIONID"]?.Value);
            objWebHeaderCollection.Add("X-li-page-instance",
                "urn:li:page:p_flagship3_search_srp_top;lKeJs0zeRfuGnnoo6YDT1A==");
            objWebHeaderCollection.Add("X-LI-Track",
                "{\"osName\":\"Android OS\",\"osVersion\":\"5.0.1\",\"clientVersion\":\"4.1.64\",\"clientMinorVersion\":97214,\"model\":\"samsung_GT-I9500\",\"dpi\":\"xxhdpi\",\"deviceType\":\"android\",\"appId\":\"com.linkedin.android\",\"deviceId\":\"97f7e5e1-cc82-4b8d-bd56-479fed652fd0\",\"timezoneOffset\":5,\"storeId\":\"us_googleplay\",\"isAdTrackingLimited\":false,\"mpName\":\"voyager-android\",\"mpVersion\":\"0.137.45\"}");
            objWebHeaderCollection.Add("X-LI-User-Agent",
                "LIAuthLibrary:0.0.3 com.linkedin.android:4.1.64 samsung_GT-I9500:android_5.0.1");
            objWebHeaderCollection.Add("X-LI-Lang", "en_US");
            ldRequestParameters.Accept = "application/vnd.linkedin.mobile.deduped+json";
            ldRequestParameters.ContentType = @"application/json; charset=utf-8";
            ldRequestParameters.AddHeader("Method", "POST");

            try
            {
                ldRequestParameters.Proxy = _linkedInAccountModel.AccountBaseModel.AccountProxy;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            ldRequestParameters.Headers = objWebHeaderCollection;
            try
            {
                ldRequestParameters.Cookies = HttpHelper.GetRequestParameter().Cookies;
            }
            catch (Exception ex)
            {
                ldRequestParameters.Cookies = new CookieCollection();
                ex.DebugLog();
            }

            //AccountModel.HttpHelper = new LDHttpHelper(LDRequestParameters);
        }

        public void SetRequestParametersAndProxy_GetRequestMobile()
        {
            try
            {
                var reqParams = HttpHelper.GetRequestParameter();
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
                reqParams.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*;q=0.8";
                reqParams.UserAgent = "ANDROID OS";
                reqParams.AddHeader("X-UDID", "fc61c979-3c11-4cc0-9537-b08a3dbfcc87");
                reqParams.AddHeader("X-RestLi-Protocol-Version", "2.0.0");
                reqParams.AddHeader("X-li-page-instance",
                    "urn:li:page:p_flagship3_search_srp_loading;UCF70oiVRfqdQfff7yEfwg==");
                reqParams.AddHeader("X-LI-Track",
                    "{\"model\":\"samsung_SM-G925F\",\"appId\":\"com.linkedin.android\",\"osVersion\":\"4.4.2\",\"mpName\":\"voyager-android\",\"timezoneOffset\":5,\"mpVersion\":\"0.225.25\",\"clientMinorVersion\":106032,\"deviceType\":\"android\",\"isAdTrackingLimited\":false,\"dpi\":\"hdpi\",\"storeId\":\"us_googleplay\",\"clientVersion\":\"4.1.152\",\"deviceId\":\"fc61c979-3c11-4cc0-9537-b08a3dbfcc87\",\"osName\":\"Android OS\"}");
                reqParams.AddHeader("X-LI-Lang", "en_US");
                try
                {
                    reqParams.AddHeader("Csrf-Token", HttpHelper.GetRequestParameter().Cookies["JSESSIONID"]?.Value);
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                reqParams.ContentType = "application/vnd.linkedin.mobile.deduped+json";
                reqParams.KeepAlive = true;
                reqParams.AddHeader("Method", "GET");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public string PremiumAccessRequest(string actionUrl, string postString)
        {
            string response;
            try
            {
                var postData = Encoding.UTF8.GetBytes(postString);
                if (!HttpHelper.GetRequestParameter().Headers.ToString().Contains("X-li-page-instance"))
                    HttpHelper.GetRequestParameter().Headers.Add("X-li-page-instance",
                        "urn:li:page:p_flagship3_launcher;" + Utils.GenerateTrackingId());

                response = HttpHelper.PostRequest(actionUrl, postData).Response;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                response = null;
            }

            return response;
        }

        public IResponseParameter BlockUserExtraResponse(string putPostUrl, string putPostData, string deleteActionUrl,
            string postDeleteDataString, string instanceId)
        {
            IResponseParameter responseParameter = null;
            var reqParams = HttpHelper.GetRequestParameter();
            try
            {
                if (!reqParams.Headers.ToString().Contains("X-Requested-With"))
                    reqParams.Headers.Add("X-Requested-With", "com.linkedin.android");

                //var putPostDataByte = Encoding.UTF8.GetBytes(putPostData);
                // var temPresponseParameter = HttpHelper.PutRequest(putPostUrl, putPostDataByte);

                var deletePostDataByte = Encoding.UTF8.GetBytes(postDeleteDataString);
                responseParameter = HttpHelper.PutRequest(deleteActionUrl, deletePostDataByte);

                /*
              var postPutDataString = "{\"entities\":{\"(clientConnectionId:" + dynamicId
                            + ",topic:urn%3Ali-realtime%3ApresenceStatusTopic%3Aurn%3Ali%3Afs_miniProfile%3" + linkedinUserDetails.ProfileId + ")\":{\"clientConnectionFabric\":\"prod-lor1\"}}}";

                        putActionUrl =
                            $"https://www.linkedin.com/realtime/realtimeFrontendSubscriptions?ids=List((clientConnectionId:{dynamicId}"
                            + $",topic:urn%3Ali-realtime%3ApresenceStatusTopic%3Aurn%3Ali%3Afs_miniProfile%3{linkedinUserDetails.ProfileId}))";

                        var postDeleteDataString = "{\"entities\":{\"(clientConnectionId:" + dynamicId + ",topic:urn%3Ali-realtime%3ApresenceStatusTopic%3Aurn%3Ali%3Afs_miniProfile%3" + linkedinUserDetails.ProfileId + ")\":{}}}";

                        deleteActionUrl =
                            $"https://www.linkedin.com/realtime/realtimeFrontendSubscriptions?ids=List((clientConnectionId:{dynamicId},topic:urn%3Ali-realtime%3ApresenceStatusTopic%3Aurn%3Ali%3Afs_miniProfile%3{linkedinUserDetails.ProfileId}))";

                     
             */
            }
            catch (Exception exception)
            {
                exception.DebugLog();
            }
            finally
            {
                try
                {
                    reqParams.Headers.Remove("X-Requested-With");
                }
                catch (Exception)
                {
                    //ignored
                }
            }

            return responseParameter;
        }

        #endregion

        #endregion


        #region Requests Implemented with Web Device

        public async Task<string> WebLogin()
        {
            string response;
            try
            {
                var preLoginPageSource = await PreLoginResponseWebRequest();
                var loginCsrfParam = string.Empty;

                #region loginCsrfParam

                try
                {
                    var regCsrfParamArray = new string[] { };
                    regCsrfParamArray = Regex.Split(preLoginPageSource, "name=\"loginCsrfParam\"");
                    loginCsrfParam = Utils.GetBetween(regCsrfParamArray[1], "value=\"", "\"");
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                #endregion

                var ldRequestParameters = (LdRequestParameters)HttpHelper.GetRequestParameter();

                var jsonElements = new LdJsonElements
                {
                    SessionKey = Uri.EscapeDataString(_linkedInAccountModel.UserName),
                    SessionPassword = Uri.EscapeDataString(_linkedInAccountModel.AccountBaseModel.Password),
                    IsJsEnabled = "false",
                    LoginCsrfParam = loginCsrfParam,
                    FpData = "default"
                };

                var postUrlLogin = "https://www.linkedin.com/uas/login-submit";
                ldRequestParameters.IsLoginPostRequest = true;
                ldRequestParameters.Body = jsonElements;
                var postData = ldRequestParameters.GenerateBody();
                try
                {
                    var loginResponseParameter =
                        await HttpHelper.PostRequestAsync(postUrlLogin, postData, _linkedInAccountModel.Token);
                    response = HttpUtility.HtmlDecode(loginResponseParameter.Response);
                    var checkPointUrl = Utils.GetBetween(response, "form__link\" href=\"/checkpoint", "\"");
                    // send reset password link
                    if (string.IsNullOrEmpty(checkPointUrl?.Trim()))
                    {
                        var requestPasswordSubmitResponse = SendResetPasswordLinkToMail(loginResponseParameter);
                        if (requestPasswordSubmitResponse.Response.Equals("{ .|js |s}"))
                            GlobusLogHelper.log.Info(Log.AccountLogin,
                                _linkedInAccountModel.AccountBaseModel.AccountNetwork,
                                $"successfully sent redirect email to this email {_linkedInAccountModel.AccountBaseModel.UserName}");
                        ;
                    }

                    // ReadVerificationCodeFromEmail(linkedInAccountModel);
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                    response = null;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                response = null;
            }

            return response;
        }

        public async Task<string> RedirectChallenge(DominatorAccountModel dominatorAccount, LoginResponseHandler loginResponseHandler)
        {
            string response;
            try
            {
                // var preLoginPageSource = string.Empty;
                var reqParams = HttpHelper.GetRequestParameter();
                reqParams.Headers.Add("X-Requested-With", "com.linkedin.android");
                var checkPointPostUrl = loginResponseHandler.ChallengeUrl;
                var checkPointPostData =
                    $"session_key={Uri.UnescapeDataString(_linkedInAccountModel.UserName)}&session_password={_linkedInAccountModel.AccountBaseModel.Password}&session_redirect=https%3A%2F%2Fwww.linkedin.com%2Fnhome";

                var checkPointResponse = await HttpHelper.PostRequestAsync(checkPointPostUrl, checkPointPostData, dominatorAccount.Token);
                response = checkPointResponse.Response;
                // string redirectChallengeUrl = HttpHelper.Request.Address.ToString();


                if (checkPointResponse.Response.Contains("d_checkpoint_rp_forceResetEmailSent"))
                    GlobusLogHelper.log.Info(Log.LoginFailed, _linkedInAccountModel.AccountBaseModel.AccountNetwork,
                        _linkedInAccountModel.AccountBaseModel.UserName, "Found Reset password link.");

                // send reset password link to this email
                // var sendResetResponse = SendResetPasswordLinkToMail(checkPointResponse);

                else if (checkPointResponse.Response.Contains("captcha"))
                    GlobusLogHelper.log.Info(Log.LoginFailed, _linkedInAccountModel.AccountBaseModel.AccountNetwork,
                        _linkedInAccountModel.AccountBaseModel.UserName, "Found Captcha.");
                else if (checkPointResponse.Response.Contains("form__input--text input_verification_pin"))
                    GlobusLogHelper.log.Info(Log.LoginFailed, _linkedInAccountModel.AccountBaseModel.AccountNetwork,
                        _linkedInAccountModel.AccountBaseModel.UserName, "Found Email verification");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                response = null;
            }

            return response;
        }

        /// <summary>
        ///     sending reset password link if we get reset password
        /// </summary>
        /// <param name="loginResponseParameter"></param>
        /// <returns></returns>
        public IResponseParameter SendResetPasswordLinkToMail(IResponseParameter loginResponseParameter)
        {
            IResponseParameter responseParameter = null;
            var response = loginResponseParameter.Response;
            try
            {
                var passwordResetMemberToken = Utils.GetBetween(response, "passwordResetMemberToken=", "\"");
                var dCheckpointRpForceResetEmailSent =
                    Utils.GetBetween(response, "d_checkpoint_rp_forceResetEmailSent;", "\"");
                var pwdReset = Utils.GetBetween(response, "Pwd-Reset:", "\"");
                var requestSubmitPasswordUrl =
                    $"https://www.linkedin.com/checkpoint/rp/request-password-reset-submit?passwordResetMemberToken={passwordResetMemberToken}";
                var randomWebKitFormBoundaryString =
                    RandomUtilties.GetRandomString(16); //Utils.generateTrackingId();//"gPuBxdbLPOY0rRTY";
                var jsessionId = HttpHelper.GetRequestParameter().Cookies["JSESSIONID"]?.Value.Replace("\"", "");
                var absoluteUri = HttpHelper.Request.Address.AbsoluteUri;
                var reqParams = HttpHelper.GetRequestParameter();
                reqParams.Headers.Clear();
                reqParams.UserAgent =
                    "LIAuthLibrary:0.0.3 com.linkedin.android:4.1.256 LAVA_V23GB:android_6.0";
                reqParams.Headers.Add("Origin", "https://www.linkedin.com");
                reqParams.Referer = absoluteUri;
                reqParams.ContentType =
                    $"multipart/form-data; boundary=----WebKitFormBoundary{randomWebKitFormBoundaryString}";
                reqParams.Accept = "*/*";
                reqParams.Headers.Add("X-Requested-With", "com.linkedin.android");
                //   reqParams.Headers.Add("Accept-Encoding", "gzip, deflate");
                reqParams.Headers.Add("Accept-Language", "en-IN,en-US;q=0.8");


                var objLdRequestParameters = new LdRequestParameters();

                var postDataParams = new Dictionary<string, string>
                {
                    {"csrfToken", jsessionId},
                    {
                        "pageInstance",
                        $"urn:li:page:d_checkpoint_rp_forceResetEmailSent;{dCheckpointRpForceResetEmailSent}"
                    },
                    {"sid", $"Pwd-Reset:{pwdReset}"},
                    {"userName", _linkedInAccountModel.UserName},
                    {"resetOption", "resendEmail"}
                };

                objLdRequestParameters.PostDataParameters = postDataParams;
                var contentType = "";
                var postData =
                    objLdRequestParameters.CreateMultipartBody(objLdRequestParameters.PostDataParameters,
                        out contentType);
                reqParams.ContentType = contentType;
                responseParameter = HttpHelper
                    .PostRequestAsync(requestSubmitPasswordUrl, postData, _linkedInAccountModel.Token).Result;

                if (string.IsNullOrEmpty(responseParameter.Response))
                    GlobusLogHelper.log.Info(Log.AccountLogin, _linkedInAccountModel.AccountBaseModel.AccountNetwork,
                        $"successfully sent reset password to this email {_linkedInAccountModel.AccountBaseModel.UserName}");
                else
                    GlobusLogHelper.log.Info(Log.AccountLogin, _linkedInAccountModel.AccountBaseModel.AccountNetwork,
                        $"failed to sent reset password this email {_linkedInAccountModel.AccountBaseModel.UserName}");
                ;

                //https://www.linkedin.com/checkpoint/rp/password-reset?requestSubmissionId=AgHydV328jKVVAAAAWfGAT0IcvXpPjwecQI7wfgyPPYp6dw4p2HpqjWZqhnhI02io82jsYuLYFVxDWdW5ZzYkhlawXipZj0L4EYecoaXR-w&lipi=urn%3Ali%3Apage%3Aemail_security_password_reset_checkpoint%3BNYDZ9i1hTTOijDR8PXMoNA%3D%3D&userName=AgFzbm2S9JQkigAAAWfGATz_o4QEJzle0IWHGuKxsk30MrJIKhtAST7jozC9WMnoZ7mzWrhjfgQBQ5-G_A&oneTimeToken=9034971400828793797&trk=eml-jav-saved-job&midToken=AQHQMo8dwqsP4A&fromEmail=fromEmail&ut=2uw5JZvbwg-8w1
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return responseParameter;
        }

        public void ReadVerificationCodeFromEmail(DominatorAccountModel accountModel)
        {
            try
            {
                GlobusLogHelper.log.Info(Log.CustomMessage, accountModel.AccountBaseModel.AccountNetwork,
                    accountModel.AccountBaseModel.UserName, "Auto Email Verification", "Getting code from email");
                //GlobusLogHelper.log.Info(Log.CustomMessage,
                //        accountModel.AccountBaseModel.AccountNetwork,
                //        accountModel.AccountBaseModel.UserName, "Getting reset email password from url.");
                //Thread.Sleep(TimeSpan.FromMinutes(2));
                //pop.mail.ru
                //pop.mail.yahoo.com
                var model =
                    new MailCredentials
                    {
                        Hostname = accountModel.MailCredentials.Hostname,
                        Port = accountModel.MailCredentials.Port,
                        Username = accountModel.MailCredentials.Username,
                        Password = accountModel.MailCredentials.Password
                    };

                var messageData = EmailClient.FetchLastMailFromSender(model, true, "security-noreply@linkedin.com");
                accountModel.VarificationCode = Utilities.GetBetween(messageData.Message,
                    "this verification code to complete your sign in:", "\n\r").Trim();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public async Task<string> PreLoginResponseWebRequest()
        {
            try
            {
                LoginWithWeb_SetRequestParametersAndProxy(_linkedInAccountModel);
                const string linkedinLink = "https://www.linkedin.com";
                var preLoginResponseWebRequest =
                    await HttpHelper.GetRequestAsync(linkedinLink, _linkedInAccountModel.Token);
                return HttpUtility.HtmlDecode(preLoginResponseWebRequest.Response);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }


        public GroupJoinerResponseHandler CheckGroupMemberShip(string actionUrl)
        {
            try
            {
                AddCsrfToken(HttpHelper.GetRequestParameter());
                HttpHelper.GetRequestParameter().Accept = "application/vnd.linkedin.normalized+json+2.1";
                return new GroupJoinerResponseHandler(HttpHelper.GetRequest(actionUrl));
            }
            catch (Exception ex)
            {
                HttpHelper.GetRequestParameter().Accept = null;
                ex.DebugLog();
                return null;
            }
        }

        public void LoginWithWeb_SetRequestParametersAndProxy(DominatorAccountModel dominatorAccountModel)
        {
            try
            {
                var reqParams = HttpHelper.GetRequestParameter();
                var csrfToken = GetCsrfToken();

                reqParams.AddHeader("Host", "www.linkedin.com");
                reqParams.KeepAlive = true;
                reqParams.AddHeader("Cache-Control", "max-age=0");
                reqParams.AddHeader("Origin", "https://www.linkedin.com");
                reqParams.AddHeader("Upgrade-Insecure-Requests", "1");
                reqParams.UserAgent =
                    "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/60.0.3112.113 Safari/537.36";
                if (!string.IsNullOrEmpty(csrfToken)) reqParams.AddHeader("Csrf-Token", csrfToken);
                reqParams.ContentType = "application/x-www-form-urlencoded";
                reqParams.Accept =
                    "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
                reqParams.Referer = "https://www.linkedin.com/";
                //objWebHeaderCollectionWeb.Add("Accept-Encoding", "gzip, deflate, br");
                reqParams.AddHeader("Accept-Language", "en-us,en;q=0.9");
                reqParams.AddHeader("Method", "Post");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void LoginWithWebSalesNavigator_SetRequestParametersAndProxy(DominatorAccountModel dominatorAccountModel)
        {
            try
            {
                var reqParams = HttpHelper.GetRequestParameter();

                var csrfToken = GetCsrfToken(dominatorAccountModel);

                reqParams.AddHeader("Host", "www.linkedin.com");
                reqParams.KeepAlive = true;
                reqParams.AddHeader("Cache-Control", "max-age=0");
                reqParams.AddHeader("Origin", "https://www.linkedin.com");
                reqParams.AddHeader("Upgrade-Insecure-Requests", "1");
                reqParams.UserAgent =
                    "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/60.0.3112.113 Safari/537.36";
                if (!string.IsNullOrEmpty(csrfToken)) reqParams.AddHeader("Csrf-Token", csrfToken);
                reqParams.ContentType = "application/x-www-form-urlencoded";
                reqParams.Accept =
                    "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
                reqParams.Referer = "https://www.linkedin.com/feed/?trk=nav_logo";
                reqParams.AddHeader("Accept-Language", "en-us,en;q=0.9");
                reqParams.AddHeader("Method", "Post");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
        public IHttpHelper GetInnerHttpHelper()
        {
            return HttpHelper;
        }

        public ILdHttpHelper GetInnerLdHttpHelper()
        {
            return HttpHelper;
        }

        public void RemoveHeaders(IRequestParameters requestParameters, params string[] headers)
        {
            try
            {
                foreach (var header in headers)
                    requestParameters.Headers.Remove(header);
            }
            catch (Exception)
            {
                //ignored
            }
        }

        public void AddCsrfToken(IRequestParameters requestParameters)
        {
            var keyVal = new KeyValuePair<string, string>("Csrf-Token", "");
            AddHeaders(HttpHelper.GetRequestParameter(), keyVal);
        }

        public void AddHeaders(IRequestParameters requestParameters, params KeyValuePair<string, string>[] headers)
        {
            try
            {
                var headerCollection = requestParameters.Headers;
                var headersString = requestParameters.Headers.ToString();
                foreach (var header in headers)
                {
                    var headerValue = header.Value;
                    if (headersString.Contains(header.Key))
                        continue;
                    if (header.Key.Equals("Csrf-Token"))
                        headerValue = _ldDataHelper.GetFullCsrfTokenFromCookies(requestParameters);

                    headerCollection.Add(header.Key, headerValue);
                }
            }
            catch (Exception)
            {
                //ignored
            }
        }

        public IResponseParameter FollowComapanyPages(string postString, string actionUrl)
        {
            return HttpHelper.PostRequest(actionUrl, postString);
        }

        public string Share(string actionUrl, string postStringOrNodeId, string composeIconId = null)
        {
            return HttpHelper.HandlePostResponse(actionUrl, postStringOrNodeId)?.Response;
        }

        public string conncetionRequest(string actionUrl, string postStringOrNodeId, string composeIconId = null)
        {
            return null;
        }

        public string Like(string actionUrl, string postStringOrNodeId)
        {
            return HttpHelper.HandlePostResponse(actionUrl, postStringOrNodeId)?.Response;
        }

        public string Comment(string actionUrlOrComment, string postStringOrNodeId, string url="", string querytype = "")
        {
            return HttpHelper.HandlePostResponse(actionUrlOrComment, postStringOrNodeId)?.Response;
        }

        public string SendGreeting(string actionUrlOrId, string postDataOrMessage)
        {
            return HttpHelper.HandlePostResponse(actionUrlOrId, postDataOrMessage)?.Response;
        }

        public string SendGroupInviter(string actionUrlOrId, string postDataOrMessage)
        {
            return HttpHelper.PostRequest(actionUrlOrId, postDataOrMessage)?.Response;
        }

        public string GetSecondaryBrowserResponse(string actionUrl, int scollDown = 0)
        {
            return "";
        }

        public void SetJobCancellationTokenInBrowser(DominatorAccountModel dominatorAccountModel,
            CancellationToken cancellationToken)
        {
        }

        public string MessageDetails(string url)
        {
            return "";
        }

        public void GoBrowserBack()
        {

        }

        public void BrowserMailverfication(string pin)
        {

        }


        public bool BrowserLogin(DominatorAccountModel account, CancellationToken token,
            LoginType loginType = LoginType.AutomationLogin, VerificationType verificationType = 0)
        {
            throw new NotImplementedException();
        }

        public void SetRequestParameterForUploadVideo(string referer, string dFlagship3SearchSrpPeople)
        {
            try
            {
                var csrfToken = GetCsrfToken();
                var objLdRequestParameters = HttpHelper.GetRequestParameter();

                objLdRequestParameters.Headers.Clear();

                objLdRequestParameters.AddHeader("Host", "www.linkedin.com");
                objLdRequestParameters.Accept = "application/vnd.linkedin.normalized+json+2.1";
                objLdRequestParameters.KeepAlive = true;
                objLdRequestParameters.AddHeader("sec-ch-ua", "\"Google Chrome\";v=\"87\", \" Not; A Brand\";v=\"99\", \"Chromium\";v=\"87\"");
               objLdRequestParameters.AddHeader("x-li-page-instance", "urn:li:page:d_flagship3_groups_entity;" + dFlagship3SearchSrpPeople);
                objLdRequestParameters.UserAgent =
                    "Mozilla/5.0 (Windows NT 6.3; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.141 Safari/537.36";
                objLdRequestParameters.Referer = referer;
                objLdRequestParameters.AddHeader("x-restli-protocol-version", "2.0.0");
                objLdRequestParameters.AddHeader("Accept-Language", "en-US");
                if (!string.IsNullOrEmpty(csrfToken))
                    objLdRequestParameters.AddHeader("csrf-token", csrfToken);
                objLdRequestParameters.ContentType = "application/json; charset=UTF-8";
                objLdRequestParameters.AddHeader("x-li-lang", "en_US");
                objLdRequestParameters.AddHeader("x-li-track", "{\"clientVersion\":\"1.7.7937\",\"mpVersion\":\"1.7.7937\",\"osName\":\"web\",\"timezoneOffset\":5.5,\"deviceFormFactor\":\"DESKTOP\",\"mpName\":\"voyager-web\",\"displayDensity\":0.8999999761581421,\"displayWidth\":1439.9999618530273,\"displayHeight\":809.9999785423279}");

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public string TryAndGetResponse(string Url, string NotContainString="", int TryCount = 2)
        {
            var Response = string.Empty;
            try
            {
                Response = IsBrowser ? GetInnerHttpHelper().GetRequest(Url).Response: GetHtmlFromUrlNormalMobileRequest(Url);
                while (string.IsNullOrEmpty(Response) ? false : TryCount-- >= 0 && (string.IsNullOrEmpty(NotContainString) ?string.IsNullOrEmpty(Response) :!Response.Contains(NotContainString)))
                {
                    Thread.Sleep(4000);
                    Response = IsBrowser ? GetInnerHttpHelper().GetRequest(Url).Response: GetHtmlFromUrlNormalMobileRequest(Url);
                }
            }catch(Exception ex) { ex.DebugLog(); }
            return Response;
        }

        public async Task<ScrapePostResponseHandler> ScrapeKeywordPost(DominatorAccountModel dominatorAccount,string QueryValue, int PaginationCount = 0)
        {
            ScrapePostResponseHandler postResponseHandler= null;
            try
            {
                var PostSearchAPI = LdConstants.PostSearchAPI(PaginationCount, QueryValue);
                var PostResponse = await GetInnerLdHttpHelper().GetRequestAsync(PostSearchAPI, dominatorAccount.Token);
                postResponseHandler = new ScrapePostResponseHandler(PostResponse);
            }catch(Exception ex) { ex.DebugLog();}
            return postResponseHandler;
        }

        public string CommentWithAlternateMethod(IDetailsFetcher detailsFetcher, LinkedinPost objLinkedinPost, UserScraperDetailedInfo userScraperDetailedInfo, string comment, string querytype)
        {
            var unixTimestamp = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
            var feedPageResponse = GetRequestUpdatedUserAgent(objLinkedinPost.PostLink, true);

            #region scraping Requirements to comment Feed

            if (!string.IsNullOrEmpty(feedPageResponse) && !(feedPageResponse.Contains("{\"data\":{\"$deletedFields\":[]")) || feedPageResponse.Contains("{\"data\":{\"plainId\""))
            {
                var referer = GetInnerLdHttpHelper().Request.Referer;
                GetInnerLdHttpHelper().Request.Referer = objLinkedinPost.PostLink;
                feedPageResponse = GetRequestUpdatedUserAgent(LdConstants.GetOwnerProfileDetailsAPI, true);
                GetInnerLdHttpHelper().Request.Referer = referer;
            }
            var fileIdentifyingUrlPath =
                detailsFetcher.GetFileIdentifyPath(feedPageResponse, userScraperDetailedInfo.MemberId);
            fileIdentifyingUrlPath.ActionUrl = "https://www.linkedin.com/voyager/api/feed/comments";

            #endregion
            comment = comment?.Replace("\r\n", "\n")?.Replace("\n", "\\n")?.Replace("\r","\\n");
            string postData;
            if (!string.IsNullOrEmpty(objLinkedinPost.ShareUrn))
            {
                var sharePostId = Utils.GetBetween(objLinkedinPost.ShareUrn + "**", "urn:li:", "**");
                postData = "{\"urn\":\"urn:li:comment:" + sharePostId + "\",\"createdTime\":" + unixTimestamp +
                           ",\"threadId\":\"" + sharePostId +
                           "\",\"canDelete\":false,\"actions\":[\"DELETE\",\"EDIT_COMMENT\"],\"edited\":false,\"commenter\":{\"com.linkedin.voyager.feed.MemberActor\":{\"id\":\"" +
                           userScraperDetailedInfo.ProfileId +
                           "\",\"showFollowAction\":false,\"miniProfile\":{\"id\":\"" +
                           userScraperDetailedInfo.ProfileId + "\",\"trackingId\":\"" +
                           userScraperDetailedInfo.TrackingId +
                           "\",\"objectUrn\":\"urn:li:member:" + userScraperDetailedInfo.MemberId +
                           "\",\"entityUrn\":\"urn:li:fs_miniProfile:" + userScraperDetailedInfo.ProfileId +
                           "\",\"firstName\":\"" + userScraperDetailedInfo.Firstname + "\",\"lastName\":\"" +
                           userScraperDetailedInfo.Lastname + "\",\"occupation\":\"" +
                           userScraperDetailedInfo.Occupation +
                           "\",\"publicIdentifier\":\"" + userScraperDetailedInfo.PublicIdentifier +
                           "\",\"picture\":{\"com.linkedin.common.VectorImage\":{\"id\":\"" +
                           userScraperDetailedInfo.ProfileId +
                           ",picture,com.linkedin.common.VectorImage\",\"rootUrl\":\"" +
                           fileIdentifyingUrlPath.RootUrl + "\",\"artifacts\":[{\"id\":\"" +
                           userScraperDetailedInfo.ProfileId +
                           ",picture,com.linkedin.common.VectorImage,artifacts," + fileIdentifyingUrlPath.Artifacts +
                           "-0\",\"fileIdentifyingUrlPathSegment\":\"" +
                           fileIdentifyingUrlPath.FileIdentifyingUrlPathSegment100 +
                           "\",\"height\":100,\"width\":100},{\"id\":\"" + userScraperDetailedInfo.ProfileId +
                           ",picture,com.linkedin.common.VectorImage,artifacts," + fileIdentifyingUrlPath.Artifacts +
                           "-1\",\"fileIdentifyingUrlPathSegment\":\"" +
                           fileIdentifyingUrlPath.FileIdentifyingUrlPathSegment200 +
                           "\",\"height\":200,\"width\":200},{\"id\":\"" + userScraperDetailedInfo.ProfileId +
                           ",picture,com.linkedin.common.VectorImage,artifacts," + fileIdentifyingUrlPath.Artifacts +
                           "-2\",\"fileIdentifyingUrlPathSegment\":\"" +
                           fileIdentifyingUrlPath.FileIdentifyingUrlPathSegment400 +
                           "\",\"height\":400,\"width\":400},{\"id\":\"" + userScraperDetailedInfo.ProfileId +
                           ",picture,com.linkedin.common.VectorImage,artifacts," + fileIdentifyingUrlPath.Artifacts +
                           "-3\",\"fileIdentifyingUrlPathSegment\":\"" +
                           fileIdentifyingUrlPath.FileIdentifyingUrlPathSegment800 +
                           "\",\"height\":800,\"width\":800}]}}},\"skillNames\":[]}},\"comment\":{\"values\":[{\"value\":\"" +
                           comment + "\"}]},\"id\":\"urn:li:comment:" + sharePostId + "\"}";
            }
            else if (querytype == "Custom Posts List")
            {
                var postId = Utils.GetBetween($"{objLinkedinPost.PostLink}/", "activity:", "?utm_source=");
                if (string.IsNullOrEmpty(postId))
                    postId = Utils.GetBetween($"{objLinkedinPost.PostLink}/", "activity-", "-");
                fileIdentifyingUrlPath.ActionUrl = "https://www.linkedin.com/voyager/api/voyagerFeedSocialNormComments";
                postData = "{\"commentary\":{\"text\":\"" + comment + "\",\"attributes\":[]},\"threadUrn\":\"urn:li:activity:" + postId + "\"}";
            }
            else if (string.IsNullOrEmpty(fileIdentifyingUrlPath.GroupPostId))
            {
                var postType = Utils.GetBetween(objLinkedinPost.PostLink + "**", "urn:li:", "**")?.Replace("/","")?.Replace("activity:", "");
                postData = "{\"comment\":{\"values\":[{\"value\":\"" + comment +
                           "\"}]},\"commenter\":{\"undefined\":{\"actorType\":\"member\",\"profileRoute\":\"profile.view\",\"emberEntityName\":\"feed/member-actor\",\"miniProfile\":{\"isFulfilled\":true,\"isRejected\":false,\"content\":{\"trackingId\":\"" +
                           userScraperDetailedInfo.TrackingId + "\",\"objectUrn\":\"urn:li:member:" +
                           userScraperDetailedInfo.MemberId + "\",\"entityUrn\":\"urn:li:fs_miniProfile:" +
                           userScraperDetailedInfo.ProfileId + "\",\"firstName\":\"" +
                           userScraperDetailedInfo.Firstname +
                           "\",\"lastName\":\"" + userScraperDetailedInfo.Lastname + "\",\"occupation\":\"" +
                           userScraperDetailedInfo.Occupation +
                           "\",\"publicIdentifier\":\"" + userScraperDetailedInfo.PublicIdentifier +
                           "\",\"originalType\":null,\"backgroundImage\":null,\"picture\":\"" +
                           userScraperDetailedInfo.ProfileId +
                           ",picture,com.linkedin.common.VectorImage\"}}}},\"commentSocialDetail\":{\"threadId\":\"" +
                           postType + "\"},\"createdTime\":" + unixTimestamp + ",\"threadId\":\"" +
                           postType + "\",\"urn\":\"urn:li:comment:" + postType +
                           "\",\"index\":0}";
            }
            else
            {
                var time = Utils.GenerateNcInMilliSecond();
                var id = Utils.GetBetween($"{objLinkedinPost.PostLink}/", "urn:li:", "/");
                postData = "{\"comment\":{\"values\":[{\"value\":\"" + comment +
                           "\"}]},\"commenter\":{\"com.linkedin.voyager.feed.MemberActor\":{\"actorType\":\"member\",\"profileRoute\":\"profile.view\",\"emberEntityName\":\"feed/member-actor\",\"miniProfile\":{\"emberEntityName\":\"identity/shared/mini-profile\",\"presence\":" +
                           "{\"lastActiveAt\":" + (time - 15000) +
                           ",\"availability\":\"ONLINE\",\"instantlyReachable\":false,\"$type\":\"com.linkedin.voyager.messaging.presence.MessagingPresenceStatus\",\"lastFetchTime\":" +
                           time + "},\"firstName\":\"" + userScraperDetailedInfo.Firstname + "\",\"lastName\":\"" +
                           userScraperDetailedInfo.Lastname + "\",\"occupation\":\"" + objLinkedinPost.Occupation +
                           "\",\"objectUrn\":\"urn:li:member:" + userScraperDetailedInfo.MemberId +
                           "\",\"entityUrn\":\"urn:li:fs_miniProfile:" + userScraperDetailedInfo.ProfileId + "\"," +
                           "\"publicIdentifier\":\"" + objLinkedinPost.PublicIdentifier +
                           "\",\"picture\":{\"com.linkedin.common.VectorImage\":{\"artifacts\":[{\"width\":100,\"fileIdentifyingUrlPathSegment\":\"" +
                           fileIdentifyingUrlPath.FileIdentifyingUrlPathSegment100 +
                           "\",\"expiresAt\":1581552000000,\"height\":100},{\"width\":200,\"fileIdentifyingUrlPathSegment\":\"" +
                           fileIdentifyingUrlPath.FileIdentifyingUrlPathSegment200 +
                           "\",\"expiresAt\":1581552000000,\"height\":200}" +
                           ",{\"width\":400,\"fileIdentifyingUrlPathSegment\":\"" +
                           fileIdentifyingUrlPath.FileIdentifyingUrlPathSegment400 +
                           "\",\"expiresAt\":1581552000000,\"height\":400},{\"width\":800,\"fileIdentifyingUrlPathSegment\":\"" +
                           fileIdentifyingUrlPath.FileIdentifyingUrlPathSegment800 +
                           "\",\"expiresAt\":1581552000000,\"height\":800}],\"rootUrl\":\"https://media.licdn.com/dms/image/C5603AQGR7jivhtF4YA/profile-displayphoto-shrink_\"}},\"trackingId\":\"" +
                           userScraperDetailedInfo.TrackingId + "\"}}}" +
                           ",\"commentSocialDetail\":{\"threadId\":\"" + id + "\"},\"createdTime\":" + time +
                           ",\"threadId\":\"" + id + "\",\"urn\":\"urn:li:comment:" + id + "\",\"index\":0}";
            }
            var CommentResponse = Comment(fileIdentifyingUrlPath.ActionUrl, postData);
            if (string.IsNullOrEmpty(CommentResponse))
            {
                var postType = Utils.GetBetween(objLinkedinPost.PostLink + "**", "urn:li:", "**")?.Replace("/", "");
                postData = LdConstants.GetFeedCommentPostData(comment, string.IsNullOrEmpty(objLinkedinPost.ShareUrn) ? $"urn:li:{postType}": objLinkedinPost.ShareUrn);
                var CommentAPI = LdConstants.GetFeedCommentAPI;
                CommentResponse = Comment(CommentAPI, postData);
                if (string.IsNullOrEmpty(CommentResponse) && !objLinkedinPost.PostLink.Contains("urn:li:activity"))
                    CommentResponse = CommentOnCustomPost(comment, objLinkedinPost.PostLink);
                else
                {
                    CommentResponse = Comment(CommentAPI, LdConstants.GetFeedCommentPostData(comment, $"urn:li:activity:{objLinkedinPost.Id}"));
                }
            }
            return CommentResponse;
        }

        public string CommentOnCustomPost(string comment, string PostUrl)
        {
            var CommentAPI = LdConstants.GetFeedCommentAPI;
            var PostPreviewResponse = _ldDataHelper.GetFeedPreviewResponse(this, PostUrl);
            var ActivityId = Utils.GetBetween(new JsonHandler(PostPreviewResponse).GetJToken("update", "updateMetadata").ToString(), "\"shareUrn\": \"", "\"");
            var CommentResponse = Comment(CommentAPI, LdConstants.GetFeedCommentPostData(comment, ActivityId)) ?? Comment(CommentAPI, LdConstants.GetFeedCommentPostData(comment, Utils.GetBetween(new JsonHandler(PostPreviewResponse).GetJToken("update", "updateMetadata").ToString(), "\"urn\": \"", "\"")));
            return CommentResponse;
        }

        public string BroadCastMessage(string ImageSource, LinkedinUser linkedinUser, bool IsGroup, string FinalMessage, string OriginToken,List<string> Medias = null)
        {
            string FinalResponse = string.Empty;
            try
            {
                var postString = string.Empty;
                var actionUrl = LdConstants.BroadCastMessageAPI;
                OriginToken = string.IsNullOrEmpty(OriginToken) ?Utilities.GetGuid(): OriginToken;
                if(!string.IsNullOrEmpty(ImageSource) || (Medias != null && Medias.Count > 0))
                {
                    var imageUploadStatusAndPostData = _ldDataHelper.GetPostDataToSendMessage(this,
                        linkedinUser.ProfileId, FinalMessage, ImageSource,linkedinUser.MemberId, OriginToken,Medias);
                    if (!imageUploadStatusAndPostData.Item1)
                        return FinalResponse;
                    postString = imageUploadStatusAndPostData.Item2;
                }
                else
                {
                    if (IsGroup)
                    {
                        postString =
                            "{\"keyVersion\":\"LEGACY_INBOX\",\"conversationCreate\":{\"eventCreate\":{\"originToken\":\"" +
                            OriginToken +
                            "\",\"value\":{\"com.linkedin.voyager.messaging.create.MessageCreate\":{\"attributedBody\":{\"text\":\"" +
                            FinalMessage?.Replace("\r\n", "\n")?.Replace("\n", "\\n")?.Replace("\r", "\\n") + "\",\"attributes\":[]},\"attachments\":[]}}},\"recipients\":[\"" +
                            linkedinUser.ProfileId +
                            "\"],\"subtype\":\"MEMBER_TO_MEMBER\",\"contextEntityUrn\":\"urn:li:fs_group:" +
                            linkedinUser.MemberId + "\"}}";
                    }
                    else
                    {
                        postString = "{\"conversationCreate\":{\"eventCreate\":{\"originToken\":\"" + OriginToken +
                                     "\",\"value\":{\"com.linkedin.voyager.messaging.create.MessageCreate\":{\"body\":\"" +
                                     FinalMessage?.Replace("\r\n", "\n")?.Replace("\n","\\n")?.Replace("\r","\\n") + "\",\"attachments\":[],\"attributedBody\":{\"text\":\"" + FinalMessage?.Replace("\r\n", "\n")?.Replace("\n", "\\n")?.Replace("\r", "\\n") +
                                     "\",\"attributes\":[]},\"mediaAttachments\":[]}}},\"recipients\":[\"" +
                                     linkedinUser.ProfileId + "\"],\"subtype\":\"MEMBER_TO_MEMBER\"}}";
                    }
                }
                FinalResponse = GetInnerLdHttpHelper().PostRequest(actionUrl, Encoding.UTF8.GetBytes(postString))?.Response;
            }
            catch (Exception ex) { }
            return FinalResponse;
        }

        public async Task<EventInviteResponseHandler> SendEventInvitation(DominatorAccountModel dominatorAccount, string ActionUrl, string PostString)
        {
            IResponseParameter response = new ResponseParameter();
            try
            {
                var reqParam = HttpHelper.GetRequestParameter();
                reqParam.Headers.Clear();
                var csrf = dominatorAccount.Cookies.Cast<Cookie>().FirstOrDefault(x=>x.Name== "JSESSIONID")?.Value;
                reqParam.Headers["x-restli-method"] = "batch_create";
                reqParam.Headers["x-li-track"] = "{\"clientVersion\":\"1.13.23611\",\"mpVersion\":\"1.13.23611\",\"osName\":\"web\",\"timezoneOffset\":5.5,\"timezone\":\"Asia/Calcutta\",\"deviceFormFactor\":\"DESKTOP\",\"mpName\":\"voyager-web\",\"displayDensity\":1,\"displayWidth\":1366,\"displayHeight\":768}";
                reqParam.Headers["sec-ch-ua"] = "\"(Not(A:Brand\";v=\"99\", \"Google Chrome\";v=\"127\", \"Chromium\";v=\"127\"";
                reqParam.Headers["csrf-token"] = csrf?.Replace("\"","");
                reqParam.Headers["sec-ch-ua-mobile"] = "?0";
                reqParam.Headers["x-li-page-instance"] = $"urn:li:page:d_flagship3_event;{dominatorAccount.CrmUuid}";
                reqParam.Headers["x-restli-protocol-version"] = "2.0.0";
                reqParam.Headers["x-li-lang"] = "en_US";
                reqParam.Headers["x-li-pem-metadata"] = "Voyager - Events - Send Invite=event-send-invitation-to-attend-event";
                reqParam.UserAgent = "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/127.0.6666.156 Safari/537.36";
                reqParam.Accept = "application/vnd.linkedin.normalized+json+2.1";
                reqParam.ContentType = "application/json; charset=UTF-8";
                reqParam.Headers["Sec-Fetch-Site"] = "same-origin";
                reqParam.Headers["Sec-Fetch-Mode"] = "cors";
                reqParam.Headers["Sec-Fetch-Dest"] = "empty";
                reqParam.Headers["Accept-Encoding"] = "gzip, deflate, br, zstd";
                reqParam.Headers["Accept-Language"] = "en-GB,en-US;q=0.9,en;q=0.8";
                reqParam.Headers["sec-ch-ua-full-version-list"] = "\"(Not(A:Brand\";v=\"99.0.0.0\", \"Google Chrome\";v=\"127\", \"Chromium\";v=\"127\"";
                reqParam.Referer = ActionUrl;
                reqParam.Cookies = dominatorAccount.Cookies;
                if(!string.IsNullOrEmpty(dominatorAccount.AccountBaseModel.AccountProxy.ProxyIp) && !string.IsNullOrEmpty(dominatorAccount.AccountBaseModel.AccountProxy.ProxyPort))
                {
                    reqParam.Proxy = new Proxy
                    {
                        ProxyIp = dominatorAccount?.AccountBaseModel?.AccountProxy?.ProxyIp,
                        ProxyPort = dominatorAccount?.AccountBaseModel?.AccountProxy?.ProxyPort,
                        ProxyUsername = dominatorAccount?.AccountBaseModel?.AccountProxy?.ProxyUsername,
                        ProxyPassword = dominatorAccount?.AccountBaseModel?.AccountProxy?.ProxyPassword,
                    };
                }
                HttpHelper.SetRequestParameter(reqParam);
                response = await HttpHelper.PostRequestAsync(ActionUrl, Encoding.UTF8.GetBytes(PostString),dominatorAccount.Token);
            }
            catch(Exception ex) { }
            return new EventInviteResponseHandler(response);
        }

        public async Task<string> GenerateAIPrompt(DominatorAccountModel dominatorAccount, string QueryValue)
        {
            var finalPrompt=string.Empty;

            try
            {
                var reqParam = new RequestParameters();
                reqParam.Headers.Clear();
                reqParam.Headers["sec-ch-ua-platform"] = "\"Windows\"";
                reqParam.Headers["sec-ch-ua"] = "\"Google Chrome\";v=\"129\", \"Not=A?Brand\";v=\"8\", \"Chromium\";v=\"129\"";
                reqParam.Headers["sec-ch-ua-mobile"] = "?0";
                reqParam.Headers["Origin"] = "chrome-extension://bemodjgbgodejfjedggokmadnjljogbh";
                reqParam.Headers["Sec-Fetch-Site"] = "none";
                reqParam.Headers["Sec-Fetch-Mode"] = "cors";
                reqParam.Headers["Sec-Fetch-Dest"] = "empty";
                //reqParam.Headers["Accept-Encoding"] = "gzip, deflate, br, zstd";
                reqParam.Headers["Accept-Encoding"] = "gzip, deflate";
                reqParam.Headers["Accept-Language"] = "en-GB,en-US;q=0.9,en;q=0.8";
                reqParam.Accept = "*/*";
                reqParam.ContentType = "application/json";
                reqParam.KeepAlive = true;
                reqParam.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/129.0.0.0 Safari/537.36";
                var questionBody = $"Generate Reply for {QueryValue?.Replace("\n", "\\n")?.Replace("\t", "\\t")?.Replace("\"", "\\\"")} for linkedin message";
                var body = $"{{\"prompt\":\"{questionBody}\",\"url\":\"ai-content-generator\",\"language\":\"English\"}}";
                var url = "https://api.aifreebox.com/api/openai";
                var response = await HttpHelper.PostRequestAsync(url,body,reqParam,dominatorAccount.Token);
                if(response != null && response?.Response != null)
                    finalPrompt = response?.Response?.ToString()?.Replace("\n","\\n")?.Replace("\"","\\\"");
            }
            catch{ }

            return finalPrompt;
        }

        public async Task<OwnFollowerResponseHandler> GetAccountsFollowers(DominatorAccountModel dominatorAccount, string actionUrl)
        {
            IResponseParameter response = new ResponseParameter();
            var reqParam = HttpHelper.GetRequestParameter();
            reqParam.Headers.Clear();
            var csrf = dominatorAccount.Cookies.Cast<Cookie>().FirstOrDefault(x => x.Name == "JSESSIONID")?.Value;
            reqParam.Headers["x-li-track"] = "{\"clientVersion\":\"1.13.23611\",\"mpVersion\":\"1.13.23611\",\"osName\":\"web\",\"timezoneOffset\":5.5,\"timezone\":\"Asia/Calcutta\",\"deviceFormFactor\":\"DESKTOP\",\"mpName\":\"voyager-web\",\"displayDensity\":1,\"displayWidth\":1366,\"displayHeight\":768}";
            reqParam.Headers["sec-ch-ua"] = "\"(Not(A:Brand\";v=\"99\", \"Google Chrome\";v=\"127\", \"Chromium\";v=\"127\"";
            reqParam.Headers["csrf-token"] = csrf?.Replace("\"", "");
            reqParam.Headers["sec-ch-ua-mobile"] = "?0";
            reqParam.Headers["x-restli-protocol-version"] = "2.0.0";
            reqParam.Headers["x-li-lang"] = "en_US";
            reqParam.UserAgent = "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/127.0.6666.156 Safari/537.36";
            reqParam.Accept = "application/vnd.linkedin.normalized+json+2.1";
            reqParam.Headers["Sec-Fetch-Site"] = "same-origin";
            reqParam.Headers["Sec-Fetch-Mode"] = "cors";
            reqParam.Headers["Sec-Fetch-Dest"] = "empty";
            reqParam.Headers["Accept-Encoding"] = "gzip, deflate, br, zstd";
            reqParam.Headers["Accept-Language"] = "en-GB,en-US;q=0.9,en;q=0.8";
            reqParam.Headers["sec-ch-ua-full-version-list"] = "\"(Not(A:Brand\";v=\"99.0.0.0\", \"Google Chrome\";v=\"127\", \"Chromium\";v=\"127\"";
            reqParam.Referer = $"https://www.linkedin.com/mynetwork/network-manager/people-follow/followers/";
            reqParam.Cookies = dominatorAccount.Cookies;
            if (!string.IsNullOrEmpty(dominatorAccount.AccountBaseModel.AccountProxy.ProxyIp) && !string.IsNullOrEmpty(dominatorAccount.AccountBaseModel.AccountProxy.ProxyPort))
            {
                reqParam.Proxy = new Proxy
                {
                    ProxyIp = dominatorAccount?.AccountBaseModel?.AccountProxy?.ProxyIp,
                    ProxyPort = dominatorAccount?.AccountBaseModel?.AccountProxy?.ProxyPort,
                    ProxyUsername = dominatorAccount?.AccountBaseModel?.AccountProxy?.ProxyUsername,
                    ProxyPassword = dominatorAccount?.AccountBaseModel?.AccountProxy?.ProxyPassword,
                };
            }
            HttpHelper.SetRequestParameter(reqParam);
            response = await HttpHelper.GetRequestAsync(actionUrl,dominatorAccount.Token);
            return new OwnFollowerResponseHandler(response);
        }

        public void SetDominatorAccount(DominatorAccountModel dominatorAccount)
        {
            if(_linkedInAccountModel is null && dominatorAccount != null)
                _linkedInAccountModel = dominatorAccount;
        }

        #endregion
    }
}