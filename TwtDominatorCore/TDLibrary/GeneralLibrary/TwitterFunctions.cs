#region using

using CefSharp;
using CefSharp.DevTools.Network;
using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.TdTables.Accounts;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.Config;
using DominatorHouseCore.Request;
using DominatorHouseCore.Utility;
using EmbeddedBrowser;
using HtmlAgilityPack;
using Microsoft.VisualBasic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using ThreadUtils;
using TwtDominatorCore.Database;
using TwtDominatorCore.Requests;
using TwtDominatorCore.Response;
using TwtDominatorCore.Response.ProfileReponseHandlerPack;
using TwtDominatorCore.TDFactories;
using TwtDominatorCore.TDLibrary.GeneralLibrary;
using TwtDominatorCore.TDLibrary.GeneralLibrary.Processors;
using TwtDominatorCore.TDModels;
using TwtDominatorCore.TDUtility;
using Unity;
using static TwtDominatorCore.TDEnums.Enums;
using Cookie = System.Net.Cookie;

#endregion


namespace TwtDominatorCore.TDLibrary
{
    public interface ITwitterFunctions : IBrowserManager
    {
        bool IsScheduleNext { get; set; }
        int UsedQueryCount { get; set; }

        Task<AccountDetails> GetAccountDetailsAsync(DominatorAccountModel twitterAccount,
            CancellationToken CancellationToken, string HomePageResponse = null);

        UnlikeResponseHandler Unlike(DominatorAccountModel twitterAccount, string TweetId, string UserName);
        AcceptPendingReqResponseHandler AcceptPendingRequest(DominatorAccountModel twitterAccount, string UserId);

        PendingFollowerRequestResponseHandler GetPendingRequests(DominatorAccountModel twitterAccount,
            string MinPosition = null);

        MuteResponseHandler TurnOnUserNotifications(DominatorAccountModel twitterAccount, string UserId, string UserName);

        MuteResponseHandler Mute(DominatorAccountModel twitterAccount, string UserId, string UserName);
        UnfollowResponseHandler Unfollow(DominatorAccountModel twitterAccount, string UserId, string UserName);

        Task<UserFeedResponseHandler> GetTweetsFromUserFeedAsync(DominatorAccountModel twitterAccount,
            string UserName, CancellationToken CancellationToken, string MinPosition = null,
            ActivityType ActivityType = ActivityType.Tweet,bool IsTweetWithReply = true,int MaxTweetCount=0);

        TrackNewMessagesResponseHandler getNewMessages(DominatorAccountModel twitterAccount,
            string MinPosition = null, bool isliveChat = false);

        MediaLikersResponseHandler GetUsersWhoLikedTweet(DominatorAccountModel twitterAccount, string TweetUrl);

        MediaRetweetsResponseHandler GetUsersWhoRetweetedTweet(DominatorAccountModel twitterAccount,
            string TweetUrl);

        List<TwitterUser> GetNewFollowedUserFromNotification(DominatorAccountModel twitterAccount,
            IDbInsertionHelper dbInsertionHelper);

        List<TagDetails> GetTweetListFromNotification(DominatorAccountModel twitterAccount);

        Task<FollowerFollowingResponseHandler> GetUserFollowingsAsync(DominatorAccountModel twitterAccount,
            string UserName, CancellationToken CancellationToken, string MinPosition = null);

        bool ReTypeEmail(DominatorAccountModel twitterAccount, ChallengeDetails challengeDetails);

        string ResolveCaptcha(LogInResponseHandler response_Login, DominatorAccountModel twAccountModel,
            ChallengeDetails challengeDetails, string uri_Matrix = null, bool IsFunCaptcha = false);

        bool ReTypePhoneNumber(DominatorAccountModel twitterAccount, ChallengeDetails challengeDetails);

        FollowResponseHandler Follow(DominatorAccountModel twitterAccount, string UserId, string UserName = null,
            string queryType = null);

        CommentResponseHandler Comment(DominatorAccountModel twitterAccount, string TweetId, string UserName,
            string Text, string queryType, List<string> ListFilepath = null, List<string> ListUserNameToTag = null);

        LikeResponseHandler Like(DominatorAccountModel twitterAccount, string TweetId, string UserName,
            string queryType);

        MediaCommentsResponseHandler GetUsersWhoCommentedOnTweet(DominatorAccountModel twitterAccount,
            string TweetUrl, string MinPosition = null);

        Task<FollowerFollowingResponseHandler> GetUserFollowersAsync(DominatorAccountModel twitterAccount,
            string UserName, CancellationToken CancellationToken, string MinPosition = null);

        Task<LogInResponseHandler> LogInAsync(DominatorAccountModel twitterAccount,
            CancellationToken cancellationToken);

        Task<LogInResponseHandler> LoginUsingFlowToken(DominatorAccountModel twitterAccount,
            CancellationToken cancellationToken);

        SearchTagResponseHandler SearchForTag(DominatorAccountModel twitterAccount, string keyword,
            string queryType, CancellationToken token, string minPosition = null, string productType = "Top");

        List<string> DownloadVideoUsingThirdParty(string TweetId, string UserName, string FolderPath, string FileName,
            int quality = 0);

        DeleteResponseHandler Delete(DominatorAccountModel twitterAccount, string TweetId,
            ActivityType ActivityType = ActivityType.Tweet);

        TweetResponseHandler Tweet(DominatorAccountModel twitterAccount, string TweetBody,
            CancellationToken CancellationToken, string Id, string Username, string queryType,
            ActivityType ActivityType, bool IsTweetContainedVideo = false, List<string> ListFilepath = null,
            List<string> ListUserNameToTag = null);

        UpdateProfileBioResponseHandler UpdateProfileBiography(DominatorAccountModel twitterAccount,
            string Biography);

        UpdateProfileScreenNameResponseHandler UpdateProfileScreenName(DominatorAccountModel twitterAccount,
            string screenName);

        UpdateProfilePicResponseHandler UpdateProfilePic(DominatorAccountModel twitterAccount, string ImagePath);

        UpdateProfileEmailResponseHandler UpdateProfileEmail(DominatorAccountModel twitterAccount,
            string EmailAddress);

        UpdateProfileWebsiteUrlResponseHandler UpdateProfileWebsite(DominatorAccountModel twitterAccount,
            string websiteUrl);

        UpdateProfileContactNumberResponseHandler UpdateProfileContact(DominatorAccountModel twitterAccount,
            string contactNumber);

        UpdateProfileFullNameResponseHandler UpdateProfileFullName(DominatorAccountModel twitterAccount,
            string fullName);

        UpdateProfileGenderResponseHandler UpdateProfileGender(DominatorAccountModel twitterAccount,
            string gender);

        RetweetResponseHandler Retweet(DominatorAccountModel twitterAccount, string TweetId, string UserName,string queryType="");
        RepostTweetResponseHandler RepostTweet(DominatorAccountModel dominatorAccount, string TweetUrl);
        TweetResponseHandler QuoteTweets(DominatorAccountModel twitterAccount, string userName, string tweetId, string tweetBody);

        UndoRetweetResponseHandler UndoRetweet(DominatorAccountModel twitterAccount, string TweetId);

        UserDetailsResponseHandler GetUserDetails(DominatorAccountModel twitterAccount, string UserName,
            string queryType,
            bool IsScrapeAllTweets = false, string MinPosition = null);

        DirectMessageResponseHandler SendDirectMessage(DominatorAccountModel twitterAccount, string UserId,
            string MessageBody, string username, string FilePath = null);

        SingleTweetDetailsResponseHandler GetSingleTweetDetails(DominatorAccountModel twitterAccount,
            string Tweeturl);

        bool SwitchToOldTwitterUi(DominatorAccountModel twitterAccount);

        void SetBrowser(DominatorAccountModel dominatorAccountModel, CancellationToken cancellationToken,
            BrowserInstanceType browserInstanceType = BrowserInstanceType.Primary);

        void SetHeaderForEditProfile(string referer, string csrfToken);
        bool GettingTweetMedia(DominatorAccountModel dominatorAccount, TagDetails tagDetails, ref List<string> tweetMedia,ActivityType activityType,string DownloadPath="");
        Task<ProfileDetailsResponseHandler> GetProfileDetails(DominatorAccountModel dominatorAccount, string UserName);
        void SetCsrfToken(ref TdRequestParameters tdRequestParameters,bool isJsonRequest=false,SearchType type=SearchType.None, string Response = "", string Path = "", string Method = "",string GuestID="");
        Task<bool> SolveFunCaptcha(DominatorAccountModel dominatorAccount,ChallengeDetails challengeDetails);
        Task<string> GetUserID(DominatorAccountModel dominatorAccount,string UserName);
        string GuestID(DominatorAccountModel dominatorAccount);
        Task<BookMarkTweetResponseHandler> BookMarkTweet(DominatorAccountModel dominatorAccount, string ID);
    }

    public class TwitterFunctions : ITwitterFunctions
    {
        private readonly IAccountScopeFactory _accountScopeFactory;
        protected readonly IContentUploaderService _contentUploaderService;
        private readonly IDateProvider _dateProvider;
        private readonly ITdHttpHelper _httpHelper;
        private readonly IDelayService _delayService;
        private ITwitterFunctions browserFunction {  get; set; }
        public int Count;
        private static string Domain => TdConstants.Domain;
        private readonly JsonJArrayHandler handler = JsonJArrayHandler.GetInstance;
        public TwitterFunctions(
            IContentUploaderService contentUploaderService, IDateProvider dateProvider, ITdHttpHelper httpHelper,
            IDelayService delayService,
            IAccountScopeFactory accountScopeFactory)
        {
            _accountScopeFactory = accountScopeFactory;
            _contentUploaderService = contentUploaderService;
            _dateProvider = dateProvider;
            _httpHelper = httpHelper;
            _delayService = delayService;
            browserFunction = InstanceProvider.GetInstance<ITwitterFunctions>("browser");
        }

        public bool IsScheduleNext { get; set; }
        public int UsedQueryCount { get; set; }


        public async Task<LogInResponseHandler> LogInAsync(DominatorAccountModel twitterAccount,
            CancellationToken cancellationToken)
        {
            var tdRequestParameter = (TdRequestParameters)_httpHelper.GetRequestParameter();
            var account = new AccountModel(twitterAccount);
            var MaxIteration = TdConstants.MaxIterationForLogin;
            var logInProcess = _accountScopeFactory[twitterAccount.AccountId].Resolve<ITwtLogInProcess>();
            try
            {
                while (MaxIteration > 0)
                {
                    var responseForUriMatrix = _httpHelper
                        .GetRequestAsync(TdConstants.UriMatrixUrl, cancellationToken).Result.Response;
                    var UriMatrix = TdUtility.getUriMatrix(responseForUriMatrix);
                    try
                    {
                        // required this in captcha solving
                        // removing already saved 
                        if (twitterAccount.ExtraParameters.Keys.Contains("UriMatrix"))
                            twitterAccount.ExtraParameters.Remove("UriMatrix");
                        twitterAccount.ExtraParameters.Add("UriMatrix", UriMatrix);
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                    var EscapeMatrixdata = Uri.EscapeDataString(UriMatrix);
                    var codemvtk = TdUtility.GetRandomHexNumber(32).ToLower();
                    _httpHelper.GetRequestParameter().Cookies.Add(new Cookie("_mb_tk", codemvtk) { Domain = TdConstants.Domain });
                    _httpHelper.GetRequestParameter().Cookies.Add(new Cookie("_sl", "1") { Domain = TdConstants.Domain });
                    var PostData = $"redirect_after_login=%2F&remember_me=1&authenticity_token={codemvtk}&wfa=1&ui_metrics={EscapeMatrixdata}&session%5Busername_or_email%5D={HttpUtility.UrlEncode(twitterAccount.AccountBaseModel.UserName)}&session%5Bpassword%5D={HttpUtility.UrlEncode(twitterAccount.AccountBaseModel.Password)}";

                    logInProcess.SetRequestHeader("text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9", $"https://{Domain}/", "document");
                    var responseHandler = new LogInResponseHandler(
                        await _httpHelper.PostRequestAsync(TdConstants.SessionUrl, PostData,
                            cancellationToken));
                    //responseHandler = new LogInResponseHandler(
                    //    await _httpHelper.GetRequestAsync(TdConstants.MainUrl, 
                    //      cancellationToken));

                    await _delayService.DelayAsync(TdConstants.ConsecutiveGetReqInteval, cancellationToken);


                    if (responseHandler.Success && responseHandler.Response.Contains("\"screen_name\":\""))
                    {
                        twitterAccount.AccountBaseModel.Status = responseHandler.DominatorStatus;
                        twitterAccount.Cookies = _httpHelper.GetRequestParameter().Cookies;
                        // don't concat this condition to &&  
                        twitterAccount.AccountBaseModel.Status = responseHandler.DominatorStatus;
                    }
                    else if (string.IsNullOrEmpty(responseHandler.Response))

                    {
                        twitterAccount.AccountBaseModel.Status = AccountStatus.Failed;
                        MaxIteration--;
                        continue;
                    }

                    return responseHandler;
                }

                return null;
            }
            catch (Exception ex)
            {
                ex.DebugLog("Error in LogInResponseHandler");
                return new LogInResponseHandler();
            }
        }

        public bool SwitchToOldTwitterUi(DominatorAccountModel twitterAccount)
        {
            BrowserWindow _browserWindow = null;
            try
            {
                Task.Factory.StartNew(() =>
                {
                    Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        try
                        {
                            twitterAccount.Token.ThrowIfCancellationRequested();
                            _browserWindow = new BrowserWindow(twitterAccount) { Visibility = Visibility.Hidden };
                            _browserWindow.SetCookie();
                            // make hidden after work is done
#if (DEBUG)
                            _browserWindow.Visibility = Visibility.Visible;
#endif
                            _browserWindow.Show();
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }
                    });
                }, twitterAccount.Token).Wait();

                _delayService.ThreadSleep(7000);
                _browserWindow.Browser.Load($"https://{Domain}/i/optout");
                _delayService.ThreadSleep(5000);
                var html = _browserWindow.Browser.GetSourceAsync().Result;
                if (html.Contains("css-76zvg2 css-16my406 css-bfa6kz r-1qd0xha r-ad9z0x r-bcqeeo r-qvutc0"))
                    _browserWindow.Browser.ExecuteScriptAsync(
                        "document.getElementsByClassName('css-76zvg2 css-16my406 css-bfa6kz r-1qd0xha r-ad9z0x r-bcqeeo r-qvutc0')[0].click()");

                _delayService.ThreadSleep(10000);
                var cookieCollection =
                    TdUtility.GetCookieCollectionFromEmbeddedBrowser(_browserWindow, out var lstCookies);

                var requestParameters = _httpHelper.GetRequestParameter();
                requestParameters.Cookies = cookieCollection;
                _httpHelper.SetRequestParameter(requestParameters);
                twitterAccount.IsUserLoggedIn = true;
                twitterAccount.Cookies = cookieCollection;
                Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    _browserWindow.Dispose();
                    _browserWindow.Close();
                });

                return true;
            }
            catch (Exception exception)
            {
                exception.DebugLog();
                return false;
            }
        }

        public async Task<AccountDetails> GetAccountDetailsAsync(DominatorAccountModel twitterAccount,
            CancellationToken CancellationToken, string HomePageResponse = null)
        {
            var IsHitUrl = string.IsNullOrEmpty(HomePageResponse);
            var tdRequestParameter = (TdRequestParameters)_httpHelper.GetRequestParameter();
            tdRequestParameter.SetupHeaders(Path: TdConstants.MainUrl,Method:"GET", GuestID: GuestID(twitterAccount));

            if (IsHitUrl)
                HomePageResponse =
                    (await _httpHelper.GetRequestAsync(TdConstants.MainUrl, CancellationToken)).Response;
            var objAccountDetails = new AccountDetails();
            try
            {
                var htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(HomePageResponse);
                var UserName = string.Empty;
                var UserID = string.Empty;
                var profiledetails = HtmlAgilityHelper.getStringInnerHtmlFromClassName(HomePageResponse,
                    "DashboardProfileCard-content", htmlDocument);
                var ScreenName =
                    HtmlAgilityHelper.getStringInnerHtmlFromClassName(profiledetails, "DashboardProfileCard-name");
                var tempUserDatas = HtmlAgilityHelper
                    .getListInnerHtmlFromClassName(profiledetails, "ProfileCardStats-statLink").ToList();
                tdRequestParameter.SetupHeaders("XML", Path: TdConstants.MainUrl, Method: "GET", GuestID: GuestID(twitterAccount));
                UserID = Utilities.GetBetween(HomePageResponse, "\"user_id\":\"", "\"");

                UserName = Utilities.GetBetween(HomePageResponse, "\"screen_name\":\"", "\"");
                var TweetCount = string.Empty;
                var FollowerCount = string.Empty;
                var FollowingCount = string.Empty;
                try
                {
                    // for english language
                    foreach (var profileDataData in tempUserDatas)
                        if (profileDataData.Contains(">Tweets</span>"))
                            TweetCount = Utilities.GetBetween(profileDataData, "data-count=\"", "\"");
                        else if (profileDataData.Contains(">Following</span>"))
                            FollowingCount = Utilities.GetBetween(profileDataData, "data-count=\"", "\"");
                        else if (profileDataData.Contains(">Followers</span>"))
                            FollowerCount = Utilities.GetBetween(profileDataData, "data-count=\"", "\"");

                    // for new UI not getting count therefore using this
                    if (tempUserDatas.Count == 0)
                    {
                        TweetCount = Utilities.GetBetween(HomePageResponse, "\"statuses_count\":", ",");
                        FollowingCount = Utilities.GetBetween(HomePageResponse, "\"friends_count\":", ",");
                        FollowerCount = Utilities.GetBetween(HomePageResponse, "\"followers_count\":", ",");
                    }
                    // we get Tweet, Following, Follower count only for english other language we don't
                    // for non english language
                    else if (TdUtility.IsZeroOrEmpty(TweetCount) && TdUtility.IsZeroOrEmpty(FollowingCount) ||
                             TdUtility.IsZeroOrEmpty(FollowerCount))
                    {
                        TweetCount = Utilities.GetBetween(tempUserDatas[0], "data-count=\"", "\"");
                        FollowingCount = Utilities.GetBetween(tempUserDatas[1], "data-count=\"", "\"");
                        FollowerCount = Utilities.GetBetween(tempUserDatas[2], "data-count=\"", "\"");
                    }
                }
                catch (Exception exception)
                {
                    exception.DebugLog();
                }

                objAccountDetails.ProfileName =
                    Utilities.GetBetween(HomePageResponse, "\"name\":\"", "\"");
                objAccountDetails.TweetCount = !string.IsNullOrEmpty(TweetCount) && Regex.IsMatch(TweetCount, @"^\d+$")
                    ? int.Parse(TweetCount)
                    : 0;
                objAccountDetails.FollowerCount =
                    !string.IsNullOrEmpty(FollowerCount) && Regex.IsMatch(FollowerCount, @"^\d+$")
                        ? int.Parse(FollowerCount)
                        : 0;
                objAccountDetails.FollowingCount =
                    !string.IsNullOrEmpty(FollowingCount) && Regex.IsMatch(FollowingCount, @"^\d+$")
                        ? int.Parse(FollowingCount)
                        : 0;
                objAccountDetails.IsPrivate = ScreenName.Contains("Icon--protected");

                // update only when check account is selected from Account manager
                if (IsHitUrl)
                {
                    await _delayService.DelayAsync(TdConstants.ConsecutiveGetReqInteval, CancellationToken);
                    var url = $"https://{Domain}/i/api/1.1/users/email_phone_info.json?include_pending_email=true";
                    SetCsrfToken(ref tdRequestParameter,Path:url,Method:"GET",GuestID:GuestID(twitterAccount));
                    var reponse = _httpHelper.GetRequest(url).Response;
                    var SettingRespose = reponse;
                    SettingRespose = SettingRespose == null ? "" : SettingRespose;
                    objAccountDetails.UserName = string.IsNullOrEmpty(UserName)
                        ? Utilities.GetBetween(SettingRespose, "orig_uname\" value=\"", "\"")
                        : UserName;
                    objAccountDetails.Email = Utilities.GetBetween(SettingRespose, "email\":\"", "\"");
                    Int64.TryParse(Utilities.GetBetween(SettingRespose, "phone_number\":\"", "\""), out objAccountDetails.PhoneNumber);

                    objAccountDetails.UserId = string.IsNullOrEmpty(UserID)
                        ? HtmlAgilityHelper.getValueWithAttributeNameFromInnerHtml(SettingRespose, "nav right-actions",
                            "data-user-id")
                        : UserID;
                }
                else
                {
                    // take last saved user details
                    objAccountDetails.UserName = string.IsNullOrEmpty(UserName) ? twitterAccount.UserName : UserName;
                    objAccountDetails.UserId = twitterAccount.AccountBaseModel.UserId;
                    var serializedUserData =
                        twitterAccount.ExtraParameters[ModuleExtraDetails.UserProfileDetails.ToString()];
                    var UserDetails = JsonConvert.DeserializeObject<UserProfileDetails>(serializedUserData);
                    objAccountDetails.Email = UserDetails.Email;
                }

                return objAccountDetails;
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
                return objAccountDetails;
            }
        }

        public async Task<FollowerFollowingResponseHandler> GetUserFollowersAsync(DominatorAccountModel twitterAccount,
            string UserName, CancellationToken CancellationToken, string MinPosition = null)
        {
            var maxIteration = TdConstants.MaxIteration;
            try
            {
                UserName = UserName.Trim();
                var UserId = "";
                if (UserName.Contains(TdConstants.MainUrl))
                    UserName = TdUtility.GetUserNameFromUrl(UserName);
                FollowerFollowingResponseHandler ResponseHandler = null;
                var tdRequestParameter = (TdRequestParameters)_httpHelper.GetRequestParameter();
                while (maxIteration >= 0)
                {
                    string requestUrl;
                    if (string.IsNullOrEmpty(MinPosition))
                    {
                        var refererUrl = TdConstants.MainUrl + UserName + "/followers";
                        tdRequestParameter.Referer = refererUrl;
                        UserId = await GetUserID(twitterAccount, UserName);
                        requestUrl = TdConstants.GetUserFollowerAndFollowings(UserId, NewUIFollowType.followers);
                    }
                    else
                    {
                        if (MinPosition == "0")
                            break;
                        UserId = await GetUserID(twitterAccount, UserName);
                        requestUrl =
                            TdConstants.GetUserFollowerAndFollowings(UserId, NewUIFollowType.followers, MinPosition);
                    }
                    tdRequestParameter = (TdRequestParameters)_httpHelper.GetRequestParameter();
                    SetCsrfToken(ref tdRequestParameter, true, SearchType.Follower,Path:requestUrl,Method:"GET", GuestID: GuestID(twitterAccount));
                    ResponseHandler =
                        new FollowerFollowingResponseHandler(
                            await _httpHelper.GetRequestAsync(requestUrl, CancellationToken));


                    if (ResponseHandler.Success) break;
                    if(ResponseHandler?.Issue?.Error == TDEnums.TwitterError.Challenge_AccountSuspended)
                    {
                        twitterAccount.AccountBaseModel.Status = AccountStatus.ProfileSuspended;
                        break;
                    }

                    _delayService.ThreadSleep(TdConstants.FloodWait);
                    maxIteration--;
                }

                return ResponseHandler;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<FollowerFollowingResponseHandler> GetUserFollowingsAsync(DominatorAccountModel twitterAccount,
            string UserName, CancellationToken CancellationToken, string MinPosition = null)
        {
            var MaxIteration = TdConstants.MaxIteration;
            try
            {
                UserName = UserName.Trim();
                var UserId = "";
                if (UserName.Contains(TdConstants.MainUrl))
                    UserName = TdUtility.GetUserNameFromUrl(UserName);
                FollowerFollowingResponseHandler ResponseHandler = null;
                var tdRequestParameter = (TdRequestParameters)_httpHelper.GetRequestParameter();
                while (MaxIteration >= 0)
                {
                    string requestUrl;
                    if (string.IsNullOrEmpty(MinPosition))
                    {
                        var refereUrl = TdConstants.MainUrl + UserName + "/following";
                        //var userRequestUrl = TdConstants.GetUserDetailsFromFollowersResponse(UserName.ToLower());
                        //tdRequestParameter.Referer = refereUrl;
                        //var response = _httpHelper.GetRequest(userRequestUrl).Response;
                        //UserId = Utilities.GetBetween(response, "\"id_str\":\"", "\"");
                        UserId = await GetUserID(twitterAccount, UserName);
                        requestUrl = TdConstants.GetUserFollowerAndFollowings(UserId, NewUIFollowType.friends);
                    }
                    else
                    {
                        if (MinPosition == "0")
                            break;
                        //var userRequestUrl = TdConstants.GetUserDetailsFromFollowersResponse(UserName);
                        //var response = _httpHelper.GetRequest(userRequestUrl).Response;
                        //UserId = Utilities.GetBetween(response, "\"id_str\":\"", "\"");
                        UserId = await GetUserID(twitterAccount, UserName);
                        requestUrl =
                            TdConstants.GetUserFollowerAndFollowings(UserId, NewUIFollowType.friends, MinPosition);
                    }
                    SetCsrfToken(ref tdRequestParameter, Path:requestUrl,Method:"GET", GuestID: GuestID(twitterAccount));
                    ResponseHandler =
                        new FollowerFollowingResponseHandler(
                            await _httpHelper.GetRequestAsync(requestUrl, CancellationToken));

                    if (ResponseHandler.Success) break;

                    _delayService.ThreadSleep(TdConstants.FloodWait);
                    MaxIteration--;
                }

                return ResponseHandler;
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public SingleTweetDetailsResponseHandler GetSingleTweetDetails(DominatorAccountModel twitterAccount,
            string TweetUrl)
        {
            var ReqUrl = string.Empty;
            var TweetId = string.Empty;

            try
            {
                if (TweetUrl.Contains("?"))
                    TweetUrl = TweetUrl.Split('?')[0];
                TweetUrl = TweetUrl.Trim();
                TweetId = TdUtility.GetTweetIdFromUrl(TweetUrl)?.Replace("/photo", "");
                ReqUrl = TdConstants.GetUserCommentTweet(TweetId);
                var tdRequestParameter = (TdRequestParameters)_httpHelper.GetRequestParameter();
                SetCsrfToken(ref tdRequestParameter,true,SearchType.CustomTweet,Method:"GET",Path:ReqUrl, GuestID: GuestID(twitterAccount));
                tdRequestParameter.UserAgent =
                    "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/80.0.3987.149 Safari/537.36";
                tdRequestParameter.Referer = TweetUrl;
                _httpHelper.SetRequestParameter(tdRequestParameter);
                var SingleResponse = _httpHelper.GetRequest(ReqUrl);
                var responseHandler = new SingleTweetDetailsResponseHandler(new ResponseParameter
                { Response = $"\"{TweetId}\"{SingleResponse.Response}" });
                return responseHandler;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public SearchTagResponseHandler SearchForTag(DominatorAccountModel twitterAccount, string keyword,
            string queryType, CancellationToken token, string minPosition = null, string productType = "Top")
        {
            var maxIteration = TdConstants.MaxIteration;
            SearchTagResponseHandler ResponseHandler = null;
            List<TagDetails> listDetails = new List<TagDetails>();
            try
            {
                var tdRequestParameter = (TdRequestParameters)_httpHelper.GetRequestParameter();
                keyword = keyword.Trim();
                var requestUrl = "";
                int failedCount = 0;
                if (queryType == "Specific User Tweets" || queryType == "Keywords")
                {
                    while(maxIteration > 0)
                    {
                        keyword = keyword?.Replace("#", "%23");
                        if (queryType != "Keywords")
                        {
                            var url = TdConstants.GetUserProfileUrl(keyword);
                            SetCsrfToken(ref tdRequestParameter,false,SearchType.CustomUser,Path:url,Method:"GET",GuestID:GuestID(twitterAccount));
                            var userDetails = _httpHelper.GetRequest(url, tdRequestParameter);
                            var userId = Utilities.GetBetween(userDetails.Response, "\"rest_id\":\"", "\"");
                            if (string.IsNullOrEmpty(minPosition))
                                requestUrl = TdConstants.GetTweets(userId, "");
                            else
                                requestUrl = TdConstants.GetTweets(userId, minPosition);
                        }
                        else
                        {
                            requestUrl = TdConstants.GetSearchTweets(keyword, minPosition, productType);
                        }
                        tdRequestParameter = (TdRequestParameters)_httpHelper.GetRequestParameter();
                        SetCsrfToken(ref tdRequestParameter, true, SearchType.Search, Path: requestUrl, Method: "GET", GuestID: GuestID(twitterAccount));
                        _httpHelper.SetRequestParameter(tdRequestParameter);
                        ResponseHandler = new SearchTagResponseHandler(
                            _httpHelper.GetRequest(requestUrl, tdRequestParameter),productType == "People");

                        if (!ResponseHandler.Success)
                            failedCount++;

                        if (!ResponseHandler.Success && failedCount > 5)
                            break;
                        if (ResponseHandler.Success)
                        {
                            minPosition = ResponseHandler.MinPosition;

                            listDetails.AddRange(ResponseHandler.ListTagDetails);
                            _delayService.ThreadSleep(TdConstants.FloodWait);
                        }
                        
                        if (listDetails.Count > 20)
                            maxIteration--;
                    }
                }
                else
                {
                    if (queryType == "Hashtags")
                    {
                        if (!keyword.Contains("#"))
                            keyword = $"#{keyword}";
                        keyword = Uri.EscapeDataString(keyword);
                    }
                    else if (queryType == "Location Users" || queryType == "Location Tweets")
                    {
                        keyword = TdUtility.GetLocationWiseSearchFormat(keyword, out _);
                    }
                    else if (queryType == "Near My Location")
                    {
                        requestUrl = $"https://{Domain}/search?q={keyword}&src=typed_query&lf=on";
                        keyword = $"{keyword}%20near%3Ame";
                    }


                    while (maxIteration > 0)
                    {
                        token.ThrowIfCancellationRequested();
                        SetCsrfToken(ref tdRequestParameter, true, SearchType.Search,Path:requestUrl,Method:"GET", GuestID: GuestID(twitterAccount));
                        _httpHelper.SetRequestParameter(tdRequestParameter);
                        requestUrl = TdConstants.GetSearchTweets(keyword, minPosition);
                        ResponseHandler =
                            new SearchTagResponseHandler(
                                _httpHelper.GetRequest(requestUrl, tdRequestParameter));

                        minPosition = ResponseHandler.MinPosition;

                        if (!ResponseHandler.Success)
                            failedCount++;

                        if (!ResponseHandler.Success && failedCount > 5)
                            break;

                        listDetails.AddRange(ResponseHandler.ListTagDetails);

                        //if (ResponseHandler.Success) break;

                        _delayService.ThreadSleep(TdConstants.FloodWait);
                        if(listDetails.Count() > 20 || ResponseHandler?.ListTagDetails?.Count == 0)
                            maxIteration--;
                    }
                }

                ResponseHandler.ListTagDetails = listDetails;

                token.ThrowIfCancellationRequested();
                return ResponseHandler;
            }
            catch (Exception Ex)
            {
                Ex.ErrorLog();
                return null;
            }
        }

        public MuteResponseHandler TurnOnUserNotifications(DominatorAccountModel twitterAccount, string UserId, string UserName)
        {
            try
            {
                var tdRequestParameter = (TdRequestParameters)_httpHelper.GetRequestParameter();
                var account = new AccountModel(twitterAccount);
                UserId = UserId.Trim();
                UserName = UserName.Trim();
                MuteResponseHandler ResponseHandler = null;
                var RequestUrl = TdConstants.MainUrl + "i/api/1.1/friendships/update.json ";
                if (string.IsNullOrEmpty(UserName))
                {
                    UserName = GetUserNameFromUserId(UserId);
                    _delayService.ThreadSleep(TdConstants.ConsecutiveGetReqInteval);
                }

                var ObjJsonElements = new JsonElementsForPostReq
                {
                    IncludeProfileInterstitialType = TdConstants.True,
                    IncludeBlockedBy = TdConstants.True,
                    IncludeBlocking = TdConstants.True,
                    IncludeFollowedBy = TdConstants.True,
                    IncludeWantRetweets = TdConstants.True,
                    IncludeMuteEdge = TdConstants.True,
                    IncludeCanDm = TdConstants.True,
                    IncludeCanMediaTtag = TdConstants.True,
                    SkipStatus = TdConstants.True,
                    Cursor = "-1",
                    Id = UserId,
                    Device = TdConstants.True
                };
                var PostData = tdRequestParameter.GeneratePostBody(ObjJsonElements);
                SetCsrfToken(ref tdRequestParameter,Path:RequestUrl,Method:"POST", GuestID: GuestID(twitterAccount));
                tdRequestParameter.Referer = TdConstants.MainUrl + UserName;
                ResponseHandler = new MuteResponseHandler(_httpHelper.PostRequest(RequestUrl, PostData));
                return ResponseHandler;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public MuteResponseHandler Mute(DominatorAccountModel twitterAccount, string UserId, string UserName)
        {
            try
            {
                var tdRequestParameter = (TdRequestParameters)_httpHelper.GetRequestParameter();
                var account = new AccountModel(twitterAccount);
                UserId = UserId.Trim();
                UserName = UserName.Trim();
                MuteResponseHandler ResponseHandler = null;
                var RequestUrl = TdConstants.MainUrl + "i/api/1.1/mutes/users/create.json";
                if (string.IsNullOrEmpty(UserName))
                {
                    UserName = GetUserNameFromUserId(UserId);
                    _delayService.ThreadSleep(TdConstants.ConsecutiveGetReqInteval);
                }

                var ObjJsonElements = new JsonElementsForPostReq
                {
                    AuthenticityToken = account.postAuthenticityToken,
                    UserId = UserId
                };
                var PostData = tdRequestParameter.GeneratePostBody(ObjJsonElements);
                SetCsrfToken(ref tdRequestParameter,Path:RequestUrl,Method:"POST", GuestID: GuestID(twitterAccount));
                tdRequestParameter.Referer = TdConstants.MainUrl + UserName;
                ResponseHandler = new MuteResponseHandler(_httpHelper.PostRequest(RequestUrl, PostData));
                return ResponseHandler;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public FollowResponseHandler Follow(DominatorAccountModel twitterAccount, string UserId, string UserName = null,
            string queryType = null)
        {
            //var requestUrl = TdConstants.ApiUrl + "friendships/create.json";
            var requestUrl = "https://x.com/i/api/1.1/friendships/create.json";
            try
            {
                var tdRequestParameter = (TdRequestParameters)_httpHelper.GetRequestParameter();
                var account = new AccountModel(twitterAccount);
                UserId = UserId.Trim();

                if (string.IsNullOrEmpty(UserName))
                {
                    UserName = GetUserNameFromUserId(UserId);
                    _delayService.ThreadSleep(TdConstants.ConsecutiveGetReqInteval);
                }
                else
                {
                    UserName = UserName.Trim();
                }

                //var objJsonElements = new JsonElementsForPostReq
                //{
                //    ChallengesPassed = TdConstants.False,
                //    HandlesChallenges = 1,
                //    IncludeBlockedBy = TdConstants.True,
                //    IncludeBlocking = TdConstants.True,
                //    IncludeCanDm = TdConstants.True,
                //    IncludeFollowedBy = TdConstants.True,
                //    IncludeMuteEdge = TdConstants.True,
                //    SkipStatus = TdConstants.True,
                //    UserId = UserId
                //};
                // sometime issue came because of different csrf token
                // therefore also check once csrf token also of account save in account.CsrfToken and 'ct0' in cookies are must be same
                //var postData = tdRequestParameter.GeneratePostBody(objJsonElements);
                var postData = Encoding.UTF8.GetBytes(TdConstants.UnFollowBody(UserId));
                SetCsrfToken(ref tdRequestParameter,Path:requestUrl,Method:"POST", GuestID: GuestID(twitterAccount));
                tdRequestParameter.Referer = TdConstants.MainUrl + UserName;

                var response = new FollowResponseHandler(_httpHelper.PostRequest(requestUrl, postData));
                if (response.Issue != null &&
                    response.Issue.Message.Contains(
                        "You must write ContentLength bytes to the request stream before calling"))
                {
                    _delayService.ThreadSleep(TimeSpan.FromSeconds(1.5));
                    response = new FollowResponseHandler(_httpHelper.PostRequest(requestUrl, postData));
                }

                return response;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public UnfollowResponseHandler Unfollow(DominatorAccountModel twitterAccount, string UserId, string UserName)
        {
            try
            {
                var tdRequestParameter = (TdRequestParameters)_httpHelper.GetRequestParameter();
                var account = new AccountModel(twitterAccount);
                UserId = UserId.Trim();
                UnfollowResponseHandler ResponseHandler = null;
                var RequestUrl = "https://x.com/i/api/1.1/friendships/destroy.json";
                #region OLD Unfollow Request Body.
                //var RequestUrl = TdConstants.ApiUrl + "friendships/destroy.json";
                //var ObjJsonElements = new JsonElementsForPostReq
                //{
                //    ChallengesPassed = TdConstants.False,
                //    HandlesChallenges = 1,
                //    ImpressionId = string.Empty,
                //    IncludeBlockedBy = TdConstants.True,
                //    IncludeBlocking = TdConstants.True,
                //    IncludeCanDm = TdConstants.True,
                //    IncludeFollowedBy = TdConstants.True,
                //    IncludeMuteEdge = TdConstants.True,
                //    SkipStatus = TdConstants.True,
                //    UserId = UserId
                //};
                //var PostData = tdRequestParameter.GeneratePostBody(ObjJsonElements);
                #endregion
                var PostData = Encoding.UTF8.GetBytes(TdConstants.UnFollowBody(UserId));
                SetCsrfToken(ref tdRequestParameter, Path: RequestUrl, Method: "POST", GuestID: GuestID(twitterAccount));
                tdRequestParameter.Referer =
                    TdConstants.MainUrl + twitterAccount.AccountBaseModel.ProfileId + "/following";
                ResponseHandler =
                    new UnfollowResponseHandler(_httpHelper.PostRequest(RequestUrl, PostData));
                return ResponseHandler;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public LikeResponseHandler Like(DominatorAccountModel twitterAccount, string TweetId, string UserName,
            string queryType)
        {
            try
            {
                var tdRequestParameter = (TdRequestParameters)_httpHelper.GetRequestParameter();
                var account = new AccountModel(twitterAccount);
                TweetId = TweetId.Trim();
                UserName = UserName.Trim();
                SetCsrfToken(ref tdRequestParameter,true,SearchType.Like,Path: TdConstants.LikeAPI,Method:"POST", GuestID: GuestID(twitterAccount));
                _httpHelper.SetRequestParameter(tdRequestParameter);
                tdRequestParameter.Referer = TdConstants.MainUrl + UserName + "/status/" + TweetId;
                var ResponseHandler = new LikeResponseHandler(_httpHelper.PostRequest(TdConstants.LikeAPI, Encoding.UTF8.GetBytes(TdConstants.GetLikePostBody(TweetId))));
                return ResponseHandler;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public UnlikeResponseHandler Unlike(DominatorAccountModel twitterAccount, string TweetId, string UserName)
        {
            try
            {
                var tdRequestParameter = (TdRequestParameters)_httpHelper.GetRequestParameter();
                var account = new AccountModel(twitterAccount);
                TweetId = TweetId.Trim();
                UserName = UserName.Trim();
                UnlikeResponseHandler ResponseHandler = null;
                SetCsrfToken(ref tdRequestParameter,true,SearchType.Unlike,Method:"POST",Path: TdConstants.UnLikeAPI, GuestID: GuestID(twitterAccount));
                tdRequestParameter.Referer = TdConstants.MainUrl + UserName + "/status/" + TweetId;
                _httpHelper.SetRequestParameter(tdRequestParameter);
                ResponseHandler =
                    new UnlikeResponseHandler(_httpHelper.PostRequest(TdConstants.UnLikeAPI, Encoding.UTF8.GetBytes(TdConstants.GetUnlikePostBody(TweetId))));
                return ResponseHandler;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public CommentResponseHandler Comment(DominatorAccountModel twitterAccount, string tweetId, string userName,
            string text, string queryType, List<string> listFilepath = null, List<string> listUserNameToTag = null)
        {
            try
            {
                text = GetTrimmedTweetBody(text);
                return browserFunction.Comment(twitterAccount,tweetId,userName,text,queryType,listFilepath,listUserNameToTag);
                #region HTTP Code for comment.
                var tdRequestParameter = (TdRequestParameters)_httpHelper.GetRequestParameter();
                var account = new AccountModel(twitterAccount);
                tweetId = tweetId.Trim();
                userName = userName.Trim();
                //var reqUrl = TdConstants.CreateTweetAPI;
                var reqUrl = "https://x.com/i/api/graphql/DM2Ap9n3_LGeXehtiljvww/CreateTweet";
                text = text.Trim();
                text = GetTrimmedTweetBody(text);
                var objJsonElements = new JsonElementsForPostReq
                {
                    AuthenticityToken = account.postAuthenticityToken,
                    AutoPopulateReplyMetaData = TdConstants.True,
                    BatchMode = TdConstants.Off,
                    InReplyToStatusId = tweetId,
                    IsPermalinkPage = TdConstants.False,
                    PlaceId = string.Empty,
                    Status = text,
                    TaggedUsers = string.Empty,
                    WeightedCharacterCount = TdConstants.True
                };

                #region if Comments contain image
                var listUserId = new List<string>();
                var listMediaIds = new List<string>();
                if (listFilepath != null && listFilepath.Count > 0)
                {
                    var mediaIDs = _contentUploaderService.UploadMediaContent(twitterAccount, listFilepath.ToArray());
                    listMediaIds.AddRange(mediaIDs?.Split(','));
                    var mediaIdEncoded = HttpUtility.UrlEncode(mediaIDs);

                    #region Tag users

                    if (listUserNameToTag?.Count > 0)
                    {
                        foreach (var user in listUserNameToTag)
                        {
                            listUserId.Add(GetUserIdFromUserName(twitterAccount, user));
                            _delayService.ThreadSleep(TdConstants.ConsecutiveGetReqInteval);
                            if (listUserId.Count >= 10)
                                break; //As we can tag Atmax 10 users at a time So no need to give extra requests.
                        }
                        var taggedUsers = string.Join(",", listUserId);
                        objJsonElements.TaggedUsers = taggedUsers;
                    }

                    #endregion

                    objJsonElements.MediaIds = mediaIdEncoded;
                }

                #endregion
                SetCsrfToken(ref tdRequestParameter,true,SearchType.Comment,Method:"POST",Path:reqUrl, GuestID: GuestID(twitterAccount));
                tdRequestParameter.Referer = "https://x.com/compose/post";
                var postData = TdConstants.GetTweetPostData(tweetId, text?.Replace("\"", "\\\""), listMediaIds,listUserId);
                var responseHandler = new CommentResponseHandler(_httpHelper.PostRequest(reqUrl, Encoding.UTF8.GetBytes(postData)));
                return responseHandler;
                #endregion
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return null;
        }

        public DeleteResponseHandler Delete(DominatorAccountModel twitterAccount, string tweetId,
            ActivityType ActivityType = ActivityType.Tweet)
        {
            var tdRequestParameter = (TdRequestParameters)_httpHelper.GetRequestParameter();
            var account = new AccountModel(twitterAccount);
            tweetId = tweetId.Trim();
            //var reqUrl = TdConstants.MainUrl + "i/api/1.1/statuses/destroy.json";
            var reqUrl = TdConstants.DeleteTweetAPI;
            var objJsonElements = new JsonElementsForPostReq
            {
                Method = TdConstants.Delete,
                AuthenticityToken = account.postAuthenticityToken,
                Id = tweetId
            };
            //var postData = tdRequestParameter.GeneratePostBody(objJsonElements);
            var postData = TdConstants.DeleteTweetPostData(tweetId);
            SetCsrfToken(ref tdRequestParameter,Path:reqUrl,Method:"POST", GuestID: GuestID(twitterAccount));
            tdRequestParameter.Referer =
                TdConstants.MainUrl + twitterAccount.AccountBaseModel.ProfileId + "/with_replies";
            tdRequestParameter.ContentType = TdConstants.ContentTypeJson;
            var responseHandler = new DeleteResponseHandler(_httpHelper.PostRequest(reqUrl, postData));
            return responseHandler;
        }

        public RetweetResponseHandler Retweet(DominatorAccountModel twitterAccount, string tweetId, string userName, string queryType = "")
        {
            try
            {
                return browserFunction.Retweet(twitterAccount, tweetId, userName, queryType);
                var tdRequestParameter = (TdRequestParameters)_httpHelper.GetRequestParameter();
                var account = new AccountModel(twitterAccount);
                tweetId = tweetId.Trim();
                userName = userName.Trim();
                RetweetResponseHandler ResponseHandler = null;
                var reqUrl = TdConstants.GetRepostAPI;
                var tweetUrl = TdUtility.GetTweetUrl(userName, tweetId);
                var postData =Encoding.UTF8.GetBytes(TdConstants.RepostPostData(tweetUrl));
                SetCsrfToken(ref tdRequestParameter,true,SearchType.Retweet, GuestID: GuestID(twitterAccount),Method:"POST",Path:reqUrl);
                tdRequestParameter.Referer = TdConstants.MainUrl + userName + "/status/" + tweetId;
                _httpHelper.SetRequestParameter(tdRequestParameter);
                ResponseHandler = new RetweetResponseHandler(_httpHelper.PostRequest(reqUrl, postData));
                return ResponseHandler;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public UndoRetweetResponseHandler UndoRetweet(DominatorAccountModel twitterAccount, string tweetId)
        {
            try
            {
                var tdRequestParameter = (TdRequestParameters)_httpHelper.GetRequestParameter();
                var account = new AccountModel(twitterAccount);
                tweetId = tweetId.Trim();
                var reqUrl = TdConstants.GetUndoRetweetAPI;
                var postData=Encoding.UTF8.GetBytes(TdConstants.GetUndoRetweetPostBody(tweetId));
                SetCsrfToken(ref tdRequestParameter,Path:reqUrl,Method:"POST", GuestID: GuestID(twitterAccount));
                var contentType= tdRequestParameter.ContentType;
                tdRequestParameter.ContentType= TdConstants.ContentTypeJson;
                tdRequestParameter.Referer = TdConstants.MainUrl + twitterAccount.AccountBaseModel.ProfileId;
                var responseHandler = new UndoRetweetResponseHandler(_httpHelper.PostRequest(reqUrl, postData));
                tdRequestParameter.ContentType = TdConstants.ContentTypeJson;
                _httpHelper.SetRequestParameter(tdRequestParameter);
                return responseHandler;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public UserDetailsResponseHandler GetUserDetails(DominatorAccountModel twitterAccount, string userName,
            string queryType,
            bool isScrapeAllTweets = false, string minPosition = null)
        {
            string FollowerCount = string.Empty, FollowingsCount = string.Empty;
            var UserId = string.Empty;
            bool IsPrivate = false;
            var website = string.Empty;
            var tdRequestParameter = (TdRequestParameters)_httpHelper.GetRequestParameter();
            userName = userName.Trim();
            if (userName.Contains(TdConstants.MainUrl))
                userName = TdUtility.GetUserNameFromUrl(userName);
            string reqUrl=string.Empty;
            var ReferreqUrl = TdConstants.MainUrl + userName.Trim();
            tdRequestParameter.Referer = ReferreqUrl;
            if (minPosition == null)
            {
                //if(twitterAccount.AccountBaseModel.UserName.Equals(userName.ToLower(),
                //        StringComparison.OrdinalIgnoreCase))
                //{
                //    reqUrl = TdConstants.GetTweets(twitterAccount.AccountBaseModel.UserId, "");
                //}
                //else
                {
                    var ProfileResponse = GetProfileDetails(twitterAccount, userName).Result;
                    if(ProfileResponse != null && ProfileResponse.Success)
                    {
                        reqUrl = TdConstants.GetTweets(ProfileResponse.UserProfileDetails?.UserId, string.Empty);
                        IsPrivate = ProfileResponse.UserProfileDetails.IsVerified;
                        FollowerCount = ProfileResponse.UserProfileDetails?.FollowerCount.ToString();
                        FollowingsCount = ProfileResponse?.UserProfileDetails?.FollowingCount.ToString();
                        UserId = ProfileResponse?.UserProfileDetails?.UserId;
                        website = ProfileResponse?.UserProfileDetails?.WebSiteUrl;
                    }
                    else
                    {
                        reqUrl = $"https://api.{Domain}/graphql/-xfUfZsnR_zqjFd-IfrN5A/UserByScreenName?variables=%7B%22screen_name%22%3A%22{userName}%22%2C%22withHighlightedLabel%22%3Atrue%7D";
                        SetCsrfToken(ref tdRequestParameter, Path: reqUrl, Method: "GET", GuestID: GuestID(twitterAccount));
                        var userdetails = _httpHelper.GetRequest(reqUrl).Response;
                        UserId = Utilities.GetBetween(userdetails, "\"rest_id\":\"", "\",");
                        IsPrivate = Utilities.GetBetween(userdetails, "\"verified\":", ",") == "True";
                        FollowerCount = Utilities.GetBetween(userdetails, "\"followers_count\":", ",");
                        FollowingsCount = Utilities.GetBetween(userdetails, "\"friends_count\":", ",");
                        reqUrl = TdConstants.GetTweets(UserId, "");
                    }
                }
            }
            else
            {
                reqUrl = TdConstants.MainUrl + "i/profiles/show/" + userName +
                         "/timeline/tweets?include_available_features=1&include_entities=1&max_position=" +
                         minPosition + "&reset_error_state=false";
                tdRequestParameter.Referer = TdConstants.MainUrl + userName;
            }
            SetCsrfToken(ref tdRequestParameter, Path: reqUrl, Method: "GET", GuestID: GuestID(twitterAccount));
            var Response = _httpHelper.GetRequest(reqUrl).Response;
            var responseHandler =
                new UserDetailsResponseHandler(new ResponseParameter { Response = $"{userName}\"{Response}" },
                    isScrapeAllTweets);
            if (string.IsNullOrEmpty(responseHandler.UserDetail.Username))
            {
                responseHandler.UserDetail.Username = userName;
                int.TryParse(FollowerCount, out int followerCount);
                responseHandler.UserDetail.FollowersCount = followerCount;
                int.TryParse(FollowingsCount, out int followingCount);
                responseHandler.UserDetail.FollowingsCount = followingCount;
                responseHandler.UserDetail.IsPrivate = IsPrivate;
                responseHandler.UserDetail.UserId = UserId;
            }
            if(string.IsNullOrEmpty(responseHandler?.UserDetail?.WebPageURL))
                responseHandler.UserDetail.WebPageURL = website;
            return responseHandler;
        }

        public DirectMessageResponseHandler SendDirectMessage(DominatorAccountModel twitterAccount, string userId,
            string messageBody, string username, string filePath = null)
        {
            try
            {
                var objMessageId = new MessageId();
                var tdRequestParameter = (TdRequestParameters)_httpHelper.GetRequestParameter();
                var updateFactory = InstanceProvider.GetInstance<ITDAccountUpdateFactory>();
                var account = new AccountModel(twitterAccount);
                if (string.IsNullOrEmpty(twitterAccount.AccountBaseModel.UserId))
                    updateFactory.CheckStatusAsync(twitterAccount, new CancellationToken()).Wait();

                userId = userId.Trim();
                messageBody = HttpUtility.UrlEncode(messageBody.Trim());
                var timeStampId = (TdUtility.UnixTimestampFromDateTime(_dateProvider.UtcNow()) * 1000).ToString();
                var objJsonElements = new JsonElementsForPostReq
                {
                    AuthenticityToken = account.postAuthenticityToken,
                    ConversationId = userId + "-" + twitterAccount.AccountBaseModel.UserId,
                    ResendId = ++objMessageId.ResendId,
                    ScribeContextComponent = "tweet_box_dm",
                    TaggedUsers = string.Empty,
                    Text = messageBody,
                    TweetBoxId = "swift_tweetbox_" + timeStampId
                };

                #region If message contains image 

                if (!string.IsNullOrEmpty(filePath))
                {
                    var mediaId = _contentUploaderService.UploadMediaContent(twitterAccount, filePath);
                    mediaId = HttpUtility.UrlEncode(mediaId);
                    var tempSplit = filePath.Split('.');
                    var extension = tempSplit[tempSplit.Length - 1];
                    objJsonElements.ResendId = ++objMessageId.ResendId;
                    objJsonElements.MediaData_FileId = objMessageId.MediaDataField;
                    objJsonElements.MediaData_FileType = "image";
                    objJsonElements.MediaData_MediaCategory = "dm_image";
                    objJsonElements.MediaData_UploadId = ++objMessageId.MediaUploadId;
                    objJsonElements.MediaData_MediaType = "image%2F" + extension;
                    objJsonElements.MediaId = mediaId;
                }

                #endregion

                var postData = tdRequestParameter.GeneratePostBody(objJsonElements);
                var reqUrl = $"https://api.{Domain}/1.1/dm/new.json";
                SetCsrfToken(ref tdRequestParameter,Path:reqUrl,Method:"POST", GuestID: GuestID(twitterAccount));
                var responseHandler = new DirectMessageResponseHandler(_httpHelper.PostRequest(reqUrl, postData));
                return responseHandler;
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

        public TweetResponseHandler Tweet(DominatorAccountModel twitterAccount, string tweetBody,
            CancellationToken cancellationToken, string Id, string Username, string queryType,
            ActivityType ActivityType, bool isTweetContainedVideo = false, List<string> listFilepath = null,
            List<string> listUserNameToTag = null)
        {
            try
            {
                return browserFunction.Tweet(twitterAccount, tweetBody, cancellationToken,Id,Username,queryType, ActivityType, isTweetContainedVideo, listFilepath, listUserNameToTag);
                var tdRequestParameter = (TdRequestParameters)_httpHelper.GetRequestParameter();
                var account = new AccountModel(twitterAccount);
                tweetBody = tweetBody.Trim();
                if(listUserNameToTag != null && listUserNameToTag.Count() != 0)
                {
                    var tagString = string.Empty;
                    foreach (var tag in listUserNameToTag)
                    {
                        if (string.IsNullOrEmpty(tag)) continue;
                        tagString += (tag.StartsWith("@") ? tag:$"@{tag}")+Environment.NewLine;
                    }
                    tweetBody = $"{tweetBody}\n{tagString}"?.Trim();
                }
                tweetBody = GetTrimmedTweetBody(tweetBody);
                var reqUrl = TdConstants.CreateTweetAPI;
                var objJsonElements = new JsonElementsForPostReq
                {
                    BatchMode = TdConstants.Off,
                    Status = tweetBody,
                    WeightedCharacterCount = TdConstants.True
                };
                cancellationToken.ThrowIfCancellationRequested();

                #region if tweet contains image/video
                var listMediaID = new List<string>();
                var listTaggedUser = new List<string>();
                if (listFilepath != null && listFilepath.Count > 0 && !listFilepath.Contains(""))
                {
                    var imageList = new List<string>();
                    foreach(var item in listFilepath )
                    {
                        #region Tweet with video
                        isTweetContainedVideo = item.Contains(".mp4") ? true : false;
                        if (item.Contains(".jpg") || item.Contains(".jpeg") || item.Contains(".png"))
                        {
                            imageList.Add(item);
                            continue;
                        }
                            
                        var mediaId = string.Empty;
                        if (isTweetContainedVideo)
                        {
                            mediaId = UploadVideo(twitterAccount, item, cancellationToken);
                            if (string.IsNullOrEmpty(mediaId))
                                mediaId = UploadVideo(twitterAccount, item, cancellationToken);
                            listMediaID.Add(mediaId);
                            objJsonElements.MediaIds = mediaId;
                        }

                        #endregion

                        
                    }
                    #region Tweet with image

                    if (!isTweetContainedVideo)
                    {
                        var mediaId =
                            _contentUploaderService.UploadMediaContent(twitterAccount, imageList.ToArray());
                        listMediaID.AddRange(mediaId?.Split(','));
                        var mediaIdEncoded = HttpUtility.UrlEncode(mediaId);

                        #region Tag user

                        if (listUserNameToTag != null && listUserNameToTag.Count > 0)
                        {
                            var listUserId = new List<string>();
                            foreach (var userName in listUserNameToTag)
                            {
                                listUserId.Add(GetUserIdFromUserName(twitterAccount, userName));
                                _delayService.ThreadSleep(TdConstants.ConsecutiveGetReqInteval);
                                if (listUserId.Count >= 10)
                                    break; //As we can tag at max 10 users at a time So no need to give extra requests.
                            }

                            var mediaTag = TdUtility.GetMediaTagFormat(mediaId.Split(',')[0], listUserId);
                            objJsonElements.MediaTags = mediaTag;
                        }

                        #endregion
                        var finalMediaIds = objJsonElements.MediaIds + "," + mediaId;
                        mediaIdEncoded = HttpUtility.UrlEncode(finalMediaIds);
                        objJsonElements.MediaIds = mediaIdEncoded;
                    }

                    #endregion

                }

                #endregion
                var TweetBody = tweetBody?.Replace("\"","\\\"")?.Replace("”", "\\\"")?.Replace("\r\n","\n")?.Replace("\n","\\n")?.Replace("“", "\\\"")?.Replace("’", "\\\\\'")?.Replace("👁️","");
                var postData = TdConstants.GetCreateTweetPostData(TweetBody, listMediaID, listTaggedUser);
                SetCsrfToken(ref tdRequestParameter,true,Path:reqUrl,Method:"POST", GuestID: GuestID(twitterAccount));
                foreach(var id in listMediaID)
                {
                    var downloadUrl = "https://x.com/i/api/1.1/media/metadata/create.json";
                    var downloadBody = $"{{\"media_id\":\"{id}\",\"allow_download_status\":{{\"allow_download\":\"true\"}}}}";
                    var makeDownloadable = _httpHelper.PostRequest(downloadUrl, Encoding.UTF8.GetBytes(downloadBody));
                }
                tdRequestParameter.ContentType = TdConstants.ContentTypeJson;
                var responseHandler = new TweetResponseHandler(_httpHelper.PostRequest(reqUrl, postData));
                return responseHandler;
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException("Operation cancelled!");
            }
            catch (Exception)
            {
                return null;
            }
        }


        #region Web Testing Request
        public TDWebParameter GetWebParameter(DominatorAccountModel dominatorAccount)
        {
            var cookie_Container = new CookieContainer();
            var proxy = dominatorAccount?.AccountBaseModel?.AccountProxy;
            var httpClient1 = new HttpClientHandler
            {
                UseCookies = true,
                CookieContainer = cookie_Container,
                AllowAutoRedirect = true,
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };
            var CsrfToken = dominatorAccount?.Cookies?.OfType<Cookie>()?.FirstOrDefault(x => x.Name == "ct0" && x.Domain == ".x.com")?.Value
                ?? dominatorAccount.Cookies?.OfType<Cookie>()?.FirstOrDefault(x => x.Name == "ct0")?.Value;
            foreach(Cookie cookie in dominatorAccount.Cookies)
            {
                try
                {
                    if(!string.IsNullOrEmpty(cookie.Value) && cookie.Value.Contains("\":\""))
                        cookie_Container.Add(new Cookie { Name = cookie.Name, Value = $"\"{cookie.Value}\"", Domain = cookie.Domain });
                    else
                        cookie_Container.Add(new Cookie { Name = cookie.Name, Value = cookie.Value, Domain = cookie.Domain });
                }
                catch(Exception e) { }
            }
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
            return new TDWebParameter
            {
                cookieContainer = cookie_Container,
                httpClient = httpClient1,
                CsrfToken = CsrfToken
            };
        }
        public async Task<string> CreateTweetAsync(DominatorAccountModel dominatorAccount,string url, string jsonPayload)
        {
            var param = GetWebParameter(dominatorAccount);
            using (var client = new HttpClient(param.httpClient))
            {
                var id = TdUtility.GetXClientTransactionID("POST", url);
                // Add headers
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
                //client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br, zstd");
                client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");
                client.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.9");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",
                    "AAAAAAAAAAAAAAAAAAAAANRILgAAAAAAnNwIzUejRCOuH5E6I8xnZz4puTs=1Zv7ttfk8LF81IUq16cHjhLTvJu4FA33AGWWjCpTnA");
                client.DefaultRequestHeaders.Add("Connection", "keep-alive");
                client.DefaultRequestHeaders.Add("Origin", "https://x.com");
                client.DefaultRequestHeaders.Add("Referer", "https://x.com/compose/post");
                client.DefaultRequestHeaders.Add("sec-ch-ua", "\"Google Chrome\";v=\"141\", \"Not?A_Brand\";v=\"8\", \"Chromium\";v=\"141\"");
                client.DefaultRequestHeaders.Add("sec-ch-ua-mobile", "?0");
                client.DefaultRequestHeaders.Add("sec-ch-ua-platform", "\"Windows\"");
                client.DefaultRequestHeaders.Add("Sec-Fetch-Dest", "empty");
                client.DefaultRequestHeaders.Add("Sec-Fetch-Mode", "cors");
                client.DefaultRequestHeaders.Add("Sec-Fetch-Site", "same-origin");
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/141.0.0.0 Safari/537.36");
                client.DefaultRequestHeaders.Add("x-client-transaction-id",id);
                client.DefaultRequestHeaders.Add("x-csrf-token", param.CsrfToken); // Must match ct0 cookie
                client.DefaultRequestHeaders.Add("x-twitter-active-user", "yes");
                client.DefaultRequestHeaders.Add("x-twitter-auth-type", "OAuth2Session");
                client.DefaultRequestHeaders.Add("x-twitter-client-language", "en");
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("https://x.com/i/api/graphql/QBGSJ27mdJ7KlPN7gm3XuQ/CreateTweet", content);
                return await response.Content.ReadAsStringAsync();
            }
        }
        #endregion



        public TweetResponseHandler QuoteTweets(DominatorAccountModel twitterAccount, string userName, string tweetId, string tweetBody)
        {
            try
            {
                var tdRequestParameter = (TdRequestParameters)_httpHelper.GetRequestParameter();
                var account = new AccountModel(twitterAccount);
                tweetBody = tweetBody.Trim();
                tweetBody = GetTrimmedTweetBody(tweetBody);
                var AttachmentUrl = $"https://{Domain}/{userName}/status/{tweetId}";
                var reqUrl = "https://x.com/i/api/graphql/NFLiwu9YyFJTAeqo69D-eA/CreateTweet"; /*TdConstants.CreateTweetAPI;*/
                var postData = TdConstants.GetQuoteTweetPostBody(tweetBody?.Replace("\"","\\\""),AttachmentUrl,new List<string>());
                SetCsrfToken(ref tdRequestParameter,true, Path: reqUrl,type:SearchType.QuoteTweet,Method: "POST", GuestID: GuestID(twitterAccount));
                tdRequestParameter.Referer = $"https://{Domain}/compose/post";
                var responseHandler = new TweetResponseHandler(_httpHelper.PostRequest(reqUrl, postData));
                return responseHandler;

            }
            catch (Exception)
            {

                return null;
            }

        }

        private string GetTrimmedTweetBody(string tweetBody)
        {
            try
            {
                var urls = Regex.Matches(tweetBody, @"(https?|ftp|file)://[-a-zA-Z0-9+&@#/%?=~_|!:,.;]*[-a-zA-Z0-9+&@#/%=~_|]");
                var tempString = tweetBody;
                var hasUrls = false;
                if (urls != null && urls.Count > 0)
                {
                    foreach (Match match in urls)
                        tempString = tempString.Replace(match.Value, "{}");
                    hasUrls = true;
                }
                if (hasUrls)
                {
                    var urlLength = urls.Count * 2;
                    if (!string.IsNullOrEmpty(tempString) && (tempString.Length + urlLength) > TdConstants.MaxCharactersAllowedToTweet)
                    {
                        GlobusLogHelper.log.Info(
                            "Text is more than 280 character.Software will reduce it to 280 character.");
                        tempString = tempString.Substring(0, TdConstants.MaxCharactersAllowedToTweet-urlLength);
                    }
                    foreach (Match item in urls)
                    {
                        tempString = tempString.Replace("{}", item.Value);
                    }
                    tweetBody = tempString;
                }
                else
                {
                    if (!string.IsNullOrEmpty(tweetBody) && tweetBody.Length > TdConstants.MaxCharactersAllowedToTweet)
                    {
                        GlobusLogHelper.log.Info(
                            "Text is more than 280 character.Software will reduce it to 280 character.");
                        tweetBody = tweetBody.Substring(0, TdConstants.MaxCharactersAllowedToTweet);
                    }
                }
            }
            catch
            {
            }
            return tweetBody;
        }

        public MediaLikersResponseHandler GetUsersWhoLikedTweet(DominatorAccountModel twitterAccount, string TweetUrl)
        {
            try
            {
                List<TwitterUser> userDetails = new List<TwitterUser>();
                var userId = TdUtility.GetTweetIdFromUrl(TweetUrl);
                var ReqUrl = TdConstants.GetUsertypesOfTweet(userId, true);
                var tdRequestParameter = (TdRequestParameters)_httpHelper.GetRequestParameter();
                TweetUrl = TweetUrl.Trim();
                SetCsrfToken(ref tdRequestParameter,Path: ReqUrl, Method: "GET", GuestID: GuestID(twitterAccount));
                tdRequestParameter.Referer = TweetUrl;
                
                
                var responseHandler =
                    new MediaLikersResponseHandler(_httpHelper.GetRequest(ReqUrl, tdRequestParameter));

                // Pagination
                userDetails = responseHandler.UserList;
                while (responseHandler.HasMoreResults && !string.IsNullOrEmpty(responseHandler.MinPosition) && userDetails.Count < 100)
                {
                    var minPosition = WebUtility.UrlEncode(responseHandler.MinPosition);
                    ReqUrl = TdConstants.GetUsertypesOfTweet(userId, true, minPosition);
                    SetCsrfToken(ref tdRequestParameter, Path: ReqUrl, Method: "GET", GuestID: GuestID(twitterAccount));
                    responseHandler =
                        new MediaLikersResponseHandler(_httpHelper.GetRequest(ReqUrl, tdRequestParameter));
                    userDetails.AddRange(responseHandler.UserList);
                }
                responseHandler.UserList.AddRange(userDetails);
                return responseHandler;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public MediaCommentsResponseHandler GetUsersWhoCommentedOnTweet(DominatorAccountModel twitterAccount,
            string TweetUrl, string MinPosition = null)
        {
            try
            {
                var tdRequestParameter = (TdRequestParameters)_httpHelper.GetRequestParameter();
                TweetUrl = TweetUrl.Trim();
                var TweetId = Utilities.GetBetween($"{TweetUrl}/", "status/", "/");
                if (string.IsNullOrEmpty(TweetId))
                    TweetId = TweetUrl;
                var ReqUrl = TdConstants.GetUserCommentTweet(TweetId, MinPosition);
                SetCsrfToken(ref tdRequestParameter,true,SearchType.CustomTweet, Path: ReqUrl, Method: "GET", GuestID: GuestID(twitterAccount));
                tdRequestParameter.Referer = TweetUrl;
                _httpHelper.SetRequestParameter(tdRequestParameter);
                
                var ResponseHandler =
                    new MediaCommentsResponseHandler(_httpHelper.GetRequest(ReqUrl, tdRequestParameter));
                return ResponseHandler;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public MediaRetweetsResponseHandler GetUsersWhoRetweetedTweet(DominatorAccountModel twitterAccount,
            string TweetUrl)
        {
            TweetUrl = TweetUrl.Trim();
            try
            {
                var ReqUrl = TdConstants.GetUsertypesOfTweet(TdUtility.GetTweetIdFromUrl(TweetUrl), false);
                var tdRequestParameter = (TdRequestParameters)_httpHelper.GetRequestParameter();
                SetCsrfToken(ref tdRequestParameter,true,SearchType.Retweeters,Path:ReqUrl,Method:"GET", GuestID: GuestID(twitterAccount));
                
                tdRequestParameter.Referer = TweetUrl;
                _httpHelper.SetRequestParameter(tdRequestParameter);
                var responseHandler =
                    new MediaRetweetsResponseHandler(_httpHelper.GetRequest(ReqUrl, tdRequestParameter));
                return responseHandler;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<UserFeedResponseHandler> GetTweetsFromUserFeedAsync(DominatorAccountModel twitterAccount,
            string UserName, CancellationToken CancellationToken, string MinPosition = null,
            ActivityType ActivityType = ActivityType.Tweet, bool IsTweetWithReply = true,int MaxtweetCount = 0)
        {
            UserFeedResponseHandler ResponseHandler = null;
            var feedDetails = new List<TagDetails>();
            try
            {
                var tdRequestParameter = (TdRequestParameters)_httpHelper.GetRequestParameter();
                UserName = UserName.Trim();
                var UserId = "";
                var MaxIteration = TdConstants.MaxIteration;
                var ReqUrl = string.Empty;
                while (MaxIteration > 0)
                {
                    if (ActivityType == ActivityType.Tweet)
                    {
                        //var userDetailsUrl = TdConstants.GetUserDetailsFromFollowersResponse(UserName.ToLower());
                        //var response = _httpHelper.GetRequest(userDetailsUrl).Response;
                        //UserId = Utilities.GetBetween(response, "\"id_str\":\"", "\"");
                        if(string.IsNullOrEmpty(UserId))
                            UserId = await GetUserID(twitterAccount, UserName);
                        ReqUrl = TdConstants.GetTweets(UserId, MinPosition, IsTweetWithReply);
                        tdRequestParameter.Referer = TdConstants.MainUrl + UserName + (IsTweetWithReply ? "/with_replies" : "/");
                    }
                    else if (ActivityType == ActivityType.TweetScraper)
                    {
                        var referUrl = $"{TdConstants.MainUrl + UserName}/likes";
                        tdRequestParameter.Referer = referUrl;
                        //var userDetailsUrl = TdConstants.GetUserDetailsFromFollowersResponse(UserName.ToLower());
                        //var response = _httpHelper.GetRequest(userDetailsUrl).Response;
                        //UserId = Utilities.GetBetween(response, "\"id_str\":\"", "\"");
                        if (string.IsNullOrEmpty(UserId))
                            UserId = await GetUserID(twitterAccount, UserName);
                        ReqUrl = TdConstants.GetLikedTweetAPI(UserId, MinPosition);
                    }
                    tdRequestParameter.Headers.Clear();
                    SetCsrfToken(ref tdRequestParameter,Path:ReqUrl,Method:"GET", GuestID: GuestID(twitterAccount));
                    ResponseHandler =
                        new UserFeedResponseHandler(
                            await _httpHelper.GetRequestAsync(ReqUrl, CancellationToken));
                    feedDetails.AddRange(ResponseHandler.UserTweetsDetail);
                    if ((MaxtweetCount > 0 && feedDetails.Count >= MaxtweetCount) || !ResponseHandler.hasmore || string.IsNullOrEmpty(ResponseHandler.MinPosition))
                        break;
                    if (ResponseHandler.hasmore && ActivityType != ActivityType.TweetScraper)
                    {
                        MinPosition = ResponseHandler.MinPosition;
                        continue;
                    }
                    _delayService.ThreadSleep(TdConstants.FloodWait);
                    MaxIteration--;
                }
                UserId = string.Empty;
                ReqUrl = string.Empty;
                
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }
            finally
            {
                if (feedDetails.Count > 0)
                {
                    ResponseHandler.UserTweetsDetail = feedDetails;
                    ResponseHandler.Success = true;
                }
            }
            return ResponseHandler;
        }

        public TrackNewMessagesResponseHandler getNewMessages(DominatorAccountModel twitterAccount,
            string MinPosition = null, bool isliveChat = false)
        {
            TrackNewMessagesResponseHandler ResponseHandler = null;
            var RequestUrl = string.Empty;
            var tdRequestParameter = (TdRequestParameters)_httpHelper.GetRequestParameter();
            
            var refer = TdConstants.MainUrl + "messages";
            tdRequestParameter.Referer = refer;
            if (MinPosition == null)
            {
                RequestUrl = TdConstants.GetInboxmessages();

            }
            else
            {
                RequestUrl = TdConstants.Getpaginationmessage(MinPosition);
            }
            SetCsrfToken(ref tdRequestParameter,Path:RequestUrl,Method:"GET", GuestID: GuestID(twitterAccount));
            var response = _httpHelper.GetRequest(RequestUrl).Response;
            ResponseHandler = new TrackNewMessagesResponseHandler(new ResponseParameter { Response = $"{twitterAccount.AccountBaseModel.UserId}\"{response}" },
                MinPosition != null, isliveChat);
            return ResponseHandler;
        }

        public List<TwitterUser> GetNewFollowedUserFromNotification(DominatorAccountModel twitterAccount,
            IDbInsertionHelper dbInsertionHelper)
        {
            var listNewFollowedUsers = new List<TwitterUser>();

            try
            {
                var account = new AccountModel(twitterAccount);

                #region initializing value

                var objDbInsertion = _accountScopeFactory[twitterAccount.AccountId].Resolve<IDbInsertionHelper>();
                var requestUrl =
                    $"https://api.{Domain}/2/notifications/all.json?include_profile_interstitial_type=1&include_blocking=1&include_blocked_by=1&include_followed_by=1&include_want_retweets=1&include_mute_edge=1&include_can_dm=1&include_can_media_tag=1&skip_status=1&cards_platform=Web-12&include_cards=1&include_ext_alt_text=true&include_quote_count=true&include_reply_count=1&tweet_mode=extended&include_entities=true&include_user_entities=true&include_ext_media_color=true&include_ext_media_availability=true&send_error_codes=true&simple_quoted_tweet=true&count=20&ext=mediaStats%2ChighlightedLabel";
                var tdRequestParameter = (TdRequestParameters)_httpHelper.GetRequestParameter();
                SetCsrfToken(ref tdRequestParameter,Path:requestUrl,Method:"GET", GuestID: GuestID(twitterAccount));
                var jsonNotificationData = _httpHelper.GetRequest(requestUrl);
                var htmlResponse = "";

                #endregion

                if (jsonNotificationData.Response.Contains("<!DOCTYPE html>"))
                {
                    htmlResponse = jsonNotificationData.Response;
                }

                else
                {
                    BrowserNotificationResponse(listNewFollowedUsers, objDbInsertion, jsonNotificationData.Response);
                    var jsonObject = new Jsonhandler(jsonNotificationData.Response);
                    htmlResponse = jsonObject.GetJTokenValue("page");
                }

                var ListOfClass =
                    HtmlAgilityHelper.getListInnerHtmlFromClassName(htmlResponse, "ActivityItem-mainBody");


                for (var i = 0; i < ListOfClass.Count; i++)
                {
                    var tempData = ListOfClass[i];
                    var notificationDateTime = DateTime.Now.ConvertToEpoch();
                    int.TryParse(Utilities.GetBetween(tempData, "data-time=\"", "\"").Trim(), out notificationDateTime);

                    // break if notification before 24 hour
                    if (DateTime.Now.ConvertToEpoch() - notificationDateTime > 86400)
                        return listNewFollowedUsers;

                    // add if notification contains follows you
                    if (tempData != null && tempData.Contains("followed you"))
                    {
                        var matches = Regex.Matches(tempData.Replace("\n", ""),
                            "js-profile-popup-actionable js-tooltip(.*?)href=(.*?)data-user-id=(.*?)original-title=.(.*?)\"(.*?)avatar size24 js-user-profile-link(.*?)src=\"(.*?)\"");

                        foreach (Match match in matches)
                        {
                            var twtUser = new TwitterUser
                            {
                                Username = SpecialCharRemover(match.Groups[2].ToString()),
                                UserId = SpecialCharRemover(match.Groups[3].ToString()),
                                FullName = SpecialCharRemover(match.Groups[4].ToString()),
                                JoiningDate = DateTime.Now,
                                HasProfilePic =
                                    !string.IsNullOrWhiteSpace(SpecialCharRemover(match.Groups[7].ToString()))
                            };


                            objDbInsertion.AddFriendshipData(twtUser, FollowType.Followers, 0);

                            listNewFollowedUsers.Add(twtUser);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return listNewFollowedUsers;
        }

        public List<string> DownloadVideoUsingThirdParty(string tweetId, string userName, string folderPath, string fileName,
            int quality = 0)
        {
            var videoUrl = TdUtility.GetVideoUrlFromThirdParty(tweetId, userName, quality);
            if (string.IsNullOrEmpty(videoUrl))
                return new List<string>();

            return TdUtility.DownloadFileFromTwitter(videoUrl, ".mp4", folderPath, fileName);
        }

        public bool ReTypePhoneNumber(DominatorAccountModel twitterAccount, ChallengeDetails challengeDetails)
        {
            try
            {
                var ReqUrl = TdConstants.MainUrl + "account/login_challenge";
                var tdRequestParameter = (TdRequestParameters)_httpHelper.GetRequestParameter();
                tdRequestParameter.SetupHeaders(Path:ReqUrl,Method:"POST", GuestID: GuestID(twitterAccount));
                tdRequestParameter.Referer =
                    challengeDetails.RefererUrl.Replace("remember_me=false", "remember_me=true");
                var postData =
                    $"authenticity_token={challengeDetails.PostAuthenticityToken}&challenge_id={challengeDetails.ChallengeId}&enc_user_id={challengeDetails.UserId}" +
                    $"&challenge_type={challengeDetails.ChallengeType}&platform=web&redirect_after_login=&remember_me=true&challenge_response={twitterAccount.VarificationCode}";
                var ResponseParameter = _httpHelper.PostRequest(ReqUrl, postData);

                if (ResponseParameter.Response.Contains($"https://{Domain}/login/error"))
                    GlobusLogHelper.log.Debug(ResponseParameter.Response); //
                else if (ResponseParameter.Response.Contains("Password change required"))
                    GlobusLogHelper.log.Info(Log.UploadingMediaFailedReason,
                        twitterAccount.AccountBaseModel.AccountNetwork, twitterAccount.UserName,
                        "Password change required");
                else if (ResponseParameter.Response.Contains(
                    "class=\"Form-message is-errored\">Incorrect. Please try again.<"))
                    GlobusLogHelper.log.Info(Log.UploadingMediaFailedReason,
                        twitterAccount.AccountBaseModel.AccountNetwork, twitterAccount.UserName,
                        "Incorrect. Please try again.");
                else
                    return true;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return false;
        }

        public bool ReTypeEmail(DominatorAccountModel twitterAccount, ChallengeDetails challengeDetails)
        {
            try
            {
                var ReqUrl = TdConstants.MainUrl + "account/login_challenge";
                var tdRequestParameter = (TdRequestParameters)_httpHelper.GetRequestParameter();
                tdRequestParameter.SetupHeaders(Path:ReqUrl,Method:"POST", GuestID: GuestID(twitterAccount));
                tdRequestParameter.Referer =
                    challengeDetails.RefererUrl.Replace("remember_me=false", "remember_me=true");
                
                var postData =
                    $"authenticity_token={challengeDetails.PostAuthenticityToken}&challenge_id={challengeDetails.ChallengeId}&enc_user_id={challengeDetails.UserId}" +
                    $"&challenge_type={challengeDetails.ChallengeType}&platform=web&redirect_after_login=&remember_me=true&challenge_response={twitterAccount.VarificationCode}";
                var ResponseParameter = _httpHelper.PostRequest(ReqUrl, postData);

                if (ResponseParameter.Response.Contains($"https://{Domain}/login/error"))
                    GlobusLogHelper.log.Debug(ResponseParameter.Response);
                else if (ResponseParameter.Response.Contains("Password change required"))
                    // "Password change required"
                    GlobusLogHelper.log.Info(Log.LoginFailed, twitterAccount.AccountBaseModel.AccountNetwork,
                        twitterAccount.AccountBaseModel.UserName, "Password change required");
                else if (ResponseParameter.Response.Contains("/account/login_challenge"))

                    return false;
                else
                    return true;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return false;
        }

        #region captcha solving

        public string ResolveCaptcha(LogInResponseHandler response_Login, DominatorAccountModel twitterAccountModel,
            ChallengeDetails challengeDetails, string uri_Matrix = null, bool IsFunCaptcha = false)
        {
            var responseData = "";
            var errorMesssage = "";
            try
            {
                if(IsFunCaptcha)
                {

                    return responseData;
                }
                var accessUrl = $"https://{Domain}/account/access";
                var googleSiteKey = "6Lc5hC4UAAAAAEx-pIfqjpmg-_-1dLnDwIZ8RToe";
                var capcthaResult = "";
                var posturl = "";
                IRequestParameters reqParams;
                var recaptchaPostData = "";
                var uriMatrix = "";

                #region getting  creds

                var genericFileManager = InstanceProvider.GetInstance<IGenericFileManager>();
                var imageCaptchaServicesModel =
                    genericFileManager.GetModel<ImageCaptchaServicesModel>(
                        ConstantVariable.GetImageCaptchaServicesFile()) ?? new ImageCaptchaServicesModel();

                if ((string.IsNullOrEmpty(imageCaptchaServicesModel.UserName) && string.IsNullOrEmpty(imageCaptchaServicesModel.Password)) && string.IsNullOrEmpty(imageCaptchaServicesModel.Token))
                {
                    GlobusLogHelper.log.Info(Log.LoginFailed, twitterAccountModel.AccountBaseModel.AccountNetwork,
                        twitterAccountModel.AccountBaseModel.UserName,
                        "Not found valid userName and password or token for ImageTyperz.");
                    return "no userName password or Token found.";
                }

                #endregion

                // if already redirected to captcha page no need to do this

                #region redirecting from start to captcha

                if (!response_Login.Response.Contains("recaptcha_element"))
                {
                    posturl = $"https://{Domain}/account/access?lang={challengeDetails.lang}";
                    reqParams = _httpHelper.GetRequestParameter();
                    reqParams.Referer = accessUrl;
                    uriMatrix = Uri.EscapeDataString(twitterAccountModel.ExtraParameters["UriMatrix"]);

                    recaptchaPostData =
                        $"authenticity_token={challengeDetails.PostAuthenticityToken}&assignment_token={challengeDetails.AssignmentToken}&lang={challengeDetails.lang}&flow=&ui_metrics={uriMatrix}";
                    _httpHelper.PostRequest(posturl, recaptchaPostData);

                }

                #endregion
                try
                {
                    if (!string.IsNullOrEmpty(imageCaptchaServicesModel.Token))
                    {
                        var imageHelper = new ImageTypersHelper(imageCaptchaServicesModel.Token);

                        var captchaId = imageHelper.SubmitSiteKey(accessUrl, googleSiteKey);
                        capcthaResult = imageHelper.GetGResponseCaptcha(captchaId);
                    }
                }
                catch (Exception ex)
                {
                    if (ex.ToString().Contains("AUTHENTICATION_FAILED"))
                    {
                        errorMesssage = "AUTHENTICATION_FAILED";
                    }

                }
                if (string.IsNullOrEmpty(errorMesssage) && errorMesssage == "AUTHENTICATION_FAILED")
                {
                    GlobusLogHelper.log.Info(Log.LoginFailed, twitterAccountModel.AccountBaseModel.AccountNetwork,
                        twitterAccountModel.AccountBaseModel.UserName, "Invalid userName and password of ImageTyperz.");
                    return "";
                }


                var solvedCaptchaResponse = string.Empty;
                var captchapostdata = $"authenticity_token={challengeDetails.PostAuthenticityToken}&assignment_token={challengeDetails.AssignmentToken}&lang={challengeDetails.lang}&flow=&g-recaptcha-response={capcthaResult}&verification_string={capcthaResult}";
                solvedCaptchaResponse = _httpHelper.PostRequest(accessUrl, captchapostdata).Response;
                posturl = $"https://{Domain}/account/access?lang={challengeDetails.lang}";
                reqParams = _httpHelper.GetRequestParameter();
                reqParams.Referer = accessUrl;
                uriMatrix = Uri.EscapeDataString(twitterAccountModel.ExtraParameters["UriMatrix"]);

                recaptchaPostData =
                    $"authenticity_token={challengeDetails.PostAuthenticityToken}&assignment_token={challengeDetails.AssignmentToken}&lang={challengeDetails.lang}&flow=&ui_metrics={uriMatrix}";
                _httpHelper.PostRequest(posturl, recaptchaPostData);
                responseData = _httpHelper.GetRequest($"https://{Domain}/?lang={challengeDetails.lang}").Response;

            }
            catch (Exception e)
            {
                e.DebugLog();
                return "";
            }

            return responseData;
        }

        #endregion

        public JavascriptResponse ExecuteScript(string script, int sleepSeconds = 2, BrowserWindow browserWindow = null)
        {
            return new JavascriptResponse();
        }


        private UserProfileDetails GetUserProfileDetails(DominatorAccountModel twitterAccount)
        {
            var UserProfileDetails = new UserProfileDetails();
            try
            {
                var serializedUserProfileDetails =
                    twitterAccount.ExtraParameters[ModuleExtraDetails.UserProfileDetails.ToString()];
                UserProfileDetails = JsonConvert.DeserializeObject<UserProfileDetails>(serializedUserProfileDetails);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return UserProfileDetails;
        }

        private static void BrowserNotificationResponse(List<TwitterUser> listNewFollowedUsers,
            IDbInsertionHelper objDbInsertion, string NotificationDataResponse)
        {
            var userId = string.Empty;
            var Username = string.Empty;
            var Fullname = string.Empty;
            var Jssonhand = new Jsonhandler(NotificationDataResponse);

            var Notification = Jssonhand.GetJToken("globalObjects", "notifications");
            var users = Jssonhand.GetJToken("globalObjects", "users");
            foreach (var NotificationItem in Notification)
            {
                var Token = NotificationItem.First();
                var messagetext = string.Empty;
                var time = Jssonhand.GetJTokenValue(Token, "timestampMs");
                long Time = 0;
                long.TryParse(time, out Time);
                if (DateTime.Now.GetCurrentEpochTimeMilliSeconds() - Time > 86400)
                    messagetext = Jssonhand.GetJTokenValue(Token, "message", "text");
                if (messagetext.Contains("followed you"))
                {
                    userId = Jssonhand.GetJTokenValue(Token, "template", "aggregateUserActionsV1","fromUsers", 0, "user", "id");
                    foreach (var user in users)
                    {
                        var UserToken = user.First();
                        var userid = Jssonhand.GetJTokenValue(UserToken, "id");
                        if (userid == userId)
                        {
                            Username = Jssonhand.GetJTokenValue(UserToken, "screen_name");
                            Fullname = Jssonhand.GetJTokenValue(UserToken, "name");
                            Jssonhand.GetJTokenValue(UserToken, "created_at");
                            var hasProfilePic = Jssonhand.GetJTokenValue(UserToken, "profile_image_url_https") != null;


                            var twtUser = new TwitterUser
                            {
                                Username = Username,
                                UserId = userId,
                                FullName = Fullname,
                                HasProfilePic = hasProfilePic,
                                JoiningDate = DateTime.Now
                            };

                            objDbInsertion.AddFriendshipData(twtUser, FollowType.Followers, 0);

                            listNewFollowedUsers.Add(twtUser);
                        }
                    }
                }
            }
        }

        public string GetUserIdFromUserName(DominatorAccountModel twitterAccount, string UserName)
        {
            var UserId = string.Empty;
            try
            {
                var tdRequestParameter = (TdRequestParameters)_httpHelper.GetRequestParameter();
                UserName = UserName?.Replace("@","").Trim();
                
                var ReferreqUrl = TdConstants.MainUrl + UserName;
                tdRequestParameter.Referer = ReferreqUrl;
                var userdetails =
                    $"https://api.{Domain}/graphql/-xfUfZsnR_zqjFd-IfrN5A/UserByScreenName?variables=%7B%22screen_name%22%3A%22{UserName}%22%2C%22withHighlightedLabel%22%3Atrue%7D";
                SetCsrfToken(ref tdRequestParameter,Path:userdetails,Method:"GET", GuestID: GuestID(twitterAccount));
                var Response = _httpHelper.GetRequest(userdetails).Response;
                UserId = Utilities.GetBetween(Response, "\"rest_id\":\"", "\"");
                if (string.IsNullOrEmpty(UserId))
                    UserId = HtmlAgilityHelper.getValueWithAttributeNameFromInnerHtml(Response,
                        "ProfileNav-item--userActions", "data-user-id");
            }
            catch (Exception)
            {
            }

            return UserId;
        }

        private string GetUserNameFromUserId(string UserId)
        {
            var userName = string.Empty;
            try
            {
                userName = null;
                var userNameResponse = _httpHelper
                    .GetRequest(TdConstants.MainUrl + "intent/user?user_id=" + UserId).Response;
                userName = Utilities.GetBetween(userNameResponse, "name=\"screen_name\" value=\"", "\"");
                if (string.IsNullOrEmpty(userName))
                    userName = Utilities.GetBetween(userNameResponse, "class=\"nickname\">", "<");
                userName = userName.Replace("@", "");
            }
            catch (Exception)
            {
            }

            return userName;
        }


        private string UploadVideo(DominatorAccountModel twitterAccount, string videoPath,
            CancellationToken CancellationToken)
        {
            var tdRequestParameter = (TdRequestParameters)_httpHelper.GetRequestParameter();
            var Extrareq = new ExtraRequests(_httpHelper);
            var MediaId = string.Empty;
            var fileLength = string.Empty;
            try
            {
                fileLength = new FileInfo(videoPath).Length.ToString();
            }
            catch (Exception ex)
            {
                ex.DebugLog(ex.Message);
            }
            var FirstReqUrl = $"https://upload.{Domain}/i/media/upload.json?command=INIT&total_bytes=" + fileLength +
                              "&media_type=video%2Fmp4&media_category=tweet_video";
            tdRequestParameter.SetupHeaders(Path:FirstReqUrl,Method:"POST", GuestID: GuestID(twitterAccount));
            var FirstPostResponse = _httpHelper.PostRequest(FirstReqUrl, "").Response;
            var nvc = new NameValueCollection();
            nvc.Add("MediaID", Utilities.GetBetween(FirstPostResponse, "media_id\":", ","));
            var str = Utilities.GetBetween(FirstPostResponse, "media_id\":", ",");
            MediaId = str;
            var nvcUpload = new NameValueCollection();
            nvcUpload.Add("media", videoPath + "<:><:><:>application/octet-stream");

            var mainUrl = $"https://{Domain}/";
            var appendChunkedUrl = $"https://upload.{Domain}/i/media/upload.json?command=APPEND&media_id=" + MediaId +
                                   "&segment_index=";

            try
            {
                var VideoName = videoPath.Substring(videoPath.LastIndexOf('\\') + 1);

                if (Count == 0)
                {
                    GlobusLogHelper.log.Info(Log.UploadingMedia, twitterAccount.AccountBaseModel.AccountNetwork,
                        twitterAccount.AccountBaseModel.UserName, VideoName);
                    Count++;
                }

                CancellationToken.ThrowIfCancellationRequested();
                if (int.Parse(fileLength) < 500 * 1024)
                {
                    Extrareq.UploadVideoByMultipartFormData(appendChunkedUrl + 0, mainUrl, nvcUpload, "media",
                        "blob");
                }

                else
                {
                    #region larger file

                    using (Stream input = File.OpenRead(videoPath))
                    {
                        var chunkSize = new Random().Next(150 * 1024, 200 * 1024);
                        int remaining = (int)input.Length, bytesRead = 0;
                        var index = 0;
                        var bufferbytes = new byte[chunkSize];

                        while (remaining > 0 &&
                               (bytesRead = input.Read(bufferbytes, 0, Math.Min(remaining, chunkSize))) > 0)
                        {
                            CancellationToken.ThrowIfCancellationRequested();
                            var URL = appendChunkedUrl + index;
                            Extrareq.UploadVideoByMultipartFormDataTest(URL, mainUrl, nvcUpload,
                                "media", "blob", bufferbytes);
                            chunkSize = new Random().Next(500 * 1024, 600 * 1024);
                            remaining -= bytesRead;
                            if (remaining > 0)
                                if (remaining < chunkSize)
                                    bufferbytes = new byte[remaining];
                                else
                                    bufferbytes = new byte[chunkSize];
                            else
                                bufferbytes = new byte[remaining];

                            ++index;
                        }
                    }

                    #endregion
                }

                _httpHelper
                    .PostRequest(
                        $"https://upload.{Domain}/i/media/upload.json?command=FINALIZE&media_id=" +
                        nvc.Get("MediaID") + "&allow_async=true", "");
                _delayService.ThreadSleep(TdConstants.ConsecutiveGetReqInteval);
                //_httpHelper
                //    .PostRequest(
                //        $"https://upload.{Domain}/i/media/upload.json?command=STATUS&media_id=" + nvc.Get("MediaID"),
                //        "");
                return MediaId;
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception)
            {
            }

            return "";
        }

        private static string SpecialCharRemover(string InputString)
        {
            var inputString = "";
            try
            {
                inputString = InputString;
                inputString = Regex.Replace(inputString, "[\"/\\\\]", "").Trim();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return inputString;
        }

        #region Private User Accounts

        public AcceptPendingReqResponseHandler AcceptPendingRequest(DominatorAccountModel twitterAccount, string UserId)
        {
            try
            {
                var tdRequestParameter = (TdRequestParameters)_httpHelper.GetRequestParameter();
                var account = new AccountModel(twitterAccount);
                AcceptPendingReqResponseHandler ResponseHandler = null;
                var ReqUrl = TdConstants.MainUrl + "i/user/accept";
                var ObjJsonElements = new JsonElementsForPostReq
                {
                    AuthenticityToken = account.postAuthenticityToken,
                    UserId = UserId
                };
                var PostData = tdRequestParameter.GeneratePostBody(ObjJsonElements);
                tdRequestParameter.SetupHeaders("XML",Path:ReqUrl,Method:"POST", GuestID: GuestID(twitterAccount));
                tdRequestParameter.Referer = TdConstants.MainUrl + "follower_requests";
                ResponseHandler =
                    new AcceptPendingReqResponseHandler(_httpHelper.PostRequest(ReqUrl, PostData));
                return ResponseHandler;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public PendingFollowerRequestResponseHandler GetPendingRequests(DominatorAccountModel twitterAccount,
            string MinPosition = null)
        {
            var RequestUrl = string.Empty;
            try
            {
                var tdRequestParameter = (TdRequestParameters)_httpHelper.GetRequestParameter();
                PendingFollowerRequestResponseHandler ResponseHandler = null;

                if (MinPosition == null)
                {
                    RequestUrl = TdConstants.MainUrl + "follower_requests";
                    tdRequestParameter.SetupHeaders(Path:RequestUrl,Method:"GET", GuestID: GuestID(twitterAccount));
                    tdRequestParameter.Referer =
                        TdConstants.MainUrl + twitterAccount.AccountBaseModel.ProfileId + "/followers";
                }
                else
                {
                    RequestUrl = TdConstants.MainUrl +
                                 "follower_requests/users?include_available_features=1&include_entities=1&max_position=" +
                                 MinPosition + "&reset_error_state=false";
                    tdRequestParameter.SetupHeaders("XML", Path: RequestUrl, Method: "GET", GuestID: GuestID(twitterAccount));
                    tdRequestParameter.Referer = TdConstants.MainUrl + "follower_requests";
                }

                ResponseHandler =
                    new PendingFollowerRequestResponseHandler(_httpHelper.GetRequest(RequestUrl));
                return ResponseHandler;
            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion

        #region UpdateProfile 

        public UpdateProfilePicResponseHandler UpdateProfilePic(DominatorAccountModel twitterAccount, string ImagePath)
        {
            try
            {
                var tdRequestParameter = (TdRequestParameters)_httpHelper.GetRequestParameter();
                var mediaId = _contentUploaderService.UploadMediaContent(twitterAccount, ImagePath);
                
                tdRequestParameter.Referer = TdConstants.MainUrl + twitterAccount.AccountBaseModel.ProfileId;

                UpdateProfilePicResponseHandler ResponseHandler = null;

                var updateProfileUrl = $"https://{Domain}/i/api/1.1/account/update_profile_image.json";
                var updateProfilePostData =
                    $"include_profile_interstitial_type=1&include_blocking=1&include_blocked_by=1&include_followed_by=1&include_want_retweets=1&include_mute_edge=1&include_can_dm=1&include_can_media_tag=1&skip_status=1&return_user=true&media_id={mediaId}";
                SetCsrfToken(ref tdRequestParameter,Path:updateProfilePostData,Method:"POST", GuestID: GuestID(twitterAccount));
                ResponseHandler =
                    new UpdateProfilePicResponseHandler(
                        _httpHelper.PostRequest(updateProfileUrl, updateProfilePostData));
                return ResponseHandler;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public UpdateProfileScreenNameResponseHandler UpdateProfileScreenName(DominatorAccountModel twitterAccount,
            string screenName)
        {
            try
            {

                UpdateProfileScreenNameResponseHandler ResponseHandler = null;
                var PostUrl = $"https://{Domain}/i/api/1.1/account/settings.json";
                var changeScreenNamePostData =
                    $"include_mention_filter=true&include_nsfw_user_flag=true&include_nsfw_admin_flag=true&include_ranked_timeline=true&include_alt_text_compose=true&screen_name={screenName}";
                ResponseHandler =
                    new UpdateProfileScreenNameResponseHandler(
                        _httpHelper.PostRequest(PostUrl, changeScreenNamePostData));

                #region If Need we can user following commented code.
                //var tdRequestParameter = (TdRequestParameters)_httpHelper.GetRequestParameter();
                //var account = new AccountModel(twitterAccount);
                //var UserDetails = GetUserProfileDetails(twitterAccount);
                //var email = UserDetails.Email;
                //if (string.IsNullOrEmpty(account.CsrfToken))
                //    account.CsrfToken = twitterAccount.Cookies.OfType<Cookie>().SingleOrDefault(x => x.Name == "ct0")
                //        .Value;

                //var ReqParam = _httpHelper.GetRequestParameter();
                //ReqParam.Referer = TdConstants.MainUrl + twitterAccount.AccountBaseModel.ProfileId;

                //tdRequestParameter.SetupHeaders(account.CsrfToken);
                //ReqParam.Headers = tdRequestParameter.Headers;
                //_httpHelper.SetRequestParameter(ReqParam);
                //tdRequestParameter.Referer = TdConstants.MainUrl + twitterAccount.AccountBaseModel.ProfileId;
                #endregion

                return ResponseHandler;
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
                return null;
            }
        }

        public UpdateProfileContactNumberResponseHandler UpdateProfileContact(DominatorAccountModel twitterAccount,
            string contactNumber)
        {
            try
            {
                var tdRequestParameter = (TdRequestParameters)_httpHelper.GetRequestParameter();
                var account = new AccountModel(twitterAccount);
                UpdateProfileContactNumberResponseHandler ResponseHandler = null;
                var UserDetails = GetUserProfileDetails(twitterAccount);
                var cookies = twitterAccount.Cookies.OfType<Cookie>();
                if (string.IsNullOrEmpty(account.CsrfToken))
                    account.CsrfToken = cookies.FirstOrDefault(x => x.Name == "ct0" && x.Domain == ".x.com").Value 
                        ?? cookies.FirstOrDefault(x => x.Name == "ct0").Value;

                var PostUrl = $"https://{Domain}/settings/add_phone";
                var DevicePageSource = _httpHelper.GetRequest($"https://{Domain}/settings/devices")
                    .Response;
                var changePhoneNumberPostData =
                    GetPhoneNumberPostData(DevicePageSource, contactNumber, UserDetails);
                var ReqParam = _httpHelper.GetRequestParameter();
                ReqParam.Referer = PostUrl;
                SetCsrfToken(ref tdRequestParameter,Path:PostUrl,Method:"POST", GuestID: GuestID(twitterAccount));
                ReqParam.Headers = tdRequestParameter.Headers;
                _httpHelper.SetRequestParameter(ReqParam);
                ResponseHandler =
                    new UpdateProfileContactNumberResponseHandler(
                        _httpHelper.PostRequest(PostUrl, changePhoneNumberPostData));

                #region after successfull send message to contact Num

                if (ResponseHandler.Success)
                {
                    var result = Interaction.InputBox("Enter Confirmation code", "Confirm code");
                    if (!result.Equals(""))
                    {
                        tdRequestParameter.SetupHeaders(account.CsrfToken,Path:PostUrl,Method:"POST", GuestID: GuestID(twitterAccount));
                        ReqParam.Headers = tdRequestParameter.Headers;
                        ReqParam.Referer = PostUrl;
                        _httpHelper.SetRequestParameter(ReqParam);

                        var confirmCodePostData =
                            GetPhoneNumberPostData(DevicePageSource, contactNumber, UserDetails, result);
                        ResponseHandler =
                            new UpdateProfileContactNumberResponseHandler(
                                _httpHelper.PostRequest(PostUrl, confirmCodePostData), true);
                    }
                }

                #endregion

                return ResponseHandler;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public UpdateProfileFullNameResponseHandler UpdateProfileFullName(DominatorAccountModel twitterAccount,
            string fullName)
        {
            try
            {

                var tdRequestParameter = (TdRequestParameters)_httpHelper.GetRequestParameter();
                tdRequestParameter.Headers.Clear();
                tdRequestParameter.ContentType = "application/json";

                var url = TdConstants.GetUserProfileUrl(twitterAccount.UserName?.Replace("@",""));
                SetCsrfToken(ref tdRequestParameter,Path:url,Method:"GET", GuestID: GuestID(twitterAccount));
                var userDetails = _httpHelper.GetRequest(url, tdRequestParameter);

                var userId = Utilities.GetBetween(userDetails.Response, "\"rest_id\":\"", "\"");
                var jsonObject = handler.ParseJsonToJObject(userDetails.Response);
                var bithdayToken = handler.GetJTokenOfJToken(jsonObject, "data", "user", "legacy_extended_profile", "birthdate");
                bithdayToken = bithdayToken is null || !bithdayToken.HasValues ? handler.GetJTokenOfJToken(jsonObject, "data", "user", "result", "legacy_extended_profile", "birthdate") : bithdayToken;
                var birthdate_day = handler.GetJTokenValue(bithdayToken,"day");
                var birthdate_month = handler.GetJTokenValue(bithdayToken, "month");
                var birthdate_year = handler.GetJTokenValue(bithdayToken, "year");
                var birthdate_visibility = handler.GetJTokenValue(bithdayToken, "visibility")?.ToLower();
                var birthdate_year_visibility = handler.GetJTokenValue(bithdayToken, "year_visibility")?.ToLower();
                //var tdRequestParameter = (TdRequestParameters) _httpHelper.GetRequestParameter();
                var account = new AccountModel(twitterAccount);
                UpdateProfileFullNameResponseHandler ResponseHandler = null;
                //var PostUrl = $"https://{Domain}/i/profiles/update ";
                var PostUrl = $"https://{Domain}/i/api/1.1/account/update_profile.json ";
                var cookies = twitterAccount.Cookies.OfType<Cookie>();
                if (string.IsNullOrEmpty(account.CsrfToken))
                    account.CsrfToken = cookies.FirstOrDefault(x => x.Name == "ct0" && x.Domain == ".x.com").Value
                        ?? cookies.FirstOrDefault(x => x.Name == "ct0").Value;
                var referrer = $"https://{Domain}/settings/profile";
                SetCsrfToken(ref tdRequestParameter,Path:PostUrl,Method:"POST", GuestID: GuestID(twitterAccount));
                fullName = Uri.EscapeUriString(fullName);
                var postData =
                    $"birthdate_day={birthdate_day}&birthdate_month={birthdate_month}&birthdate_year={birthdate_year}&birthdate_visibility={birthdate_visibility}&birthdate_year_visibility={birthdate_year_visibility}&displayNameMaxLength=50&name={fullName}";

                ResponseHandler =
                    new UpdateProfileFullNameResponseHandler(_httpHelper.PostRequest(PostUrl, postData));
                return ResponseHandler;
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
                return null;
            }
        }

        public UpdateProfileEmailResponseHandler UpdateProfileEmail(DominatorAccountModel twitterAccount,
            string EmailAddress)
        {
            try
            {
                var tdRequestParameter = (TdRequestParameters)_httpHelper.GetRequestParameter();
                var UserProfileDetails = GetUserProfileDetails(twitterAccount);
                UpdateProfileEmailResponseHandler ResponseHandler = null;
                EmailAddress = EmailAddress.Trim();
                var PostUrl = $"https://{Domain}/settings/accounts/update";


                var changeEmailPostData =
                    $"_method=PUT&authenticity_token={UserProfileDetails.authenticityToken}&orig_uname={WebUtility.UrlEncode(UserProfileDetails.UserName)}&orig_email={WebUtility.UrlEncode(UserProfileDetails.Email)}"
                    + $"&user%5Bscreen_name%5D={WebUtility.UrlEncode(UserProfileDetails.UserName)}&user%5Bemail%5D={WebUtility.UrlEncode(EmailAddress)}&user%5Blang%5D=en&user%5Btime_zone%5D=Pacific+Time+%28US+%26+Canada%29&user%5Bno_username_only_password_reset%5D=0&user%5Bcountry%5D=in"
                    + $"&user%5Bautoplay_disabled%5D=0&user%5Bautoplay_disabled%5D=1&user%5Bpersonalize_timeline%5D=1&user%5Bpersonalize_timeline%5D=0&auth_password={twitterAccount.AccountBaseModel.Password}&secret-code=";

                var ReqParam = _httpHelper.GetRequestParameter();
                ReqParam.Referer = $"https://{Domain}/settings/account";
                tdRequestParameter.SetupHeaders(UserProfileDetails.csrfToken,Path:PostUrl,Method:"POST", GuestID: GuestID(twitterAccount));
                ReqParam.Headers = tdRequestParameter.Headers;
                _httpHelper.SetRequestParameter(ReqParam);
                ResponseHandler =
                    new UpdateProfileEmailResponseHandler(
                        _httpHelper.PostRequest(PostUrl, changeEmailPostData));
                return ResponseHandler;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public UpdateProfileBioResponseHandler UpdateProfileBiography(DominatorAccountModel twitterAccount,
            string Biography)
        {
            try
            {
                var tdRequestParameter = (TdRequestParameters)_httpHelper.GetRequestParameter();
                var account = new AccountModel(twitterAccount);
                UpdateProfileBioResponseHandler ResponseHandler = null;
                var bioData = WebUtility.UrlEncode(Biography);
                var cookies = twitterAccount.Cookies.OfType<Cookie>();
                SetHeaderForEditProfile($"https://{Domain}/settings/profile", 
                    cookies.FirstOrDefault(x => x.Name == "ct0" && x.Domain == ".x.com").Value
                        ?? cookies.FirstOrDefault(x => x.Name == "ct0").Value);
                tdRequestParameter.Referer = $"https://{Domain}/settings/profile";

                var postData = $"displayNameMaxLength=50&description={bioData}";

                ResponseHandler = new UpdateProfileBioResponseHandler(
                    _httpHelper.PostRequest($"https://{Domain}/i/api/1.1/account/update_profile.json", postData));
                var changePostResponse = WebUtility.HtmlDecode(ResponseHandler.PageResponse);
                if (!changePostResponse.Contains(WebUtility.UrlDecode(bioData)))
                {
                    var len = Biography.Length;
                    if (len > 160)
                        GlobusLogHelper.log.Info(Log.ActivitySuccessful, twitterAccount.AccountBaseModel.AccountNetwork,
                            twitterAccount.UserName, "Edit Twitter Profile",
                            "Exceeded the Bio Content Max length of '160'");
                    else
                        GlobusLogHelper.log.Info(Log.ActivitySuccessful, twitterAccount.AccountBaseModel.AccountNetwork,
                            twitterAccount.UserName, "Edit Twitter Profile", "Unable to Changed the Bio Content.");
                }

                return ResponseHandler;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public UpdateProfileGenderResponseHandler UpdateProfileGender(DominatorAccountModel twitterAccount,
            string gender)
        {
            try
            {
                var tdRequestParameter = (TdRequestParameters)_httpHelper.GetRequestParameter();
                var account = new AccountModel(twitterAccount);
                var cookies = twitterAccount.Cookies.OfType<Cookie>();
                if (string.IsNullOrEmpty(account.CsrfToken))
                    account.CsrfToken = cookies.FirstOrDefault(x => x.Name == "ct0" && x.Domain == ".x.com").Value
                        ?? cookies.FirstOrDefault(x => x.Name == "ct0").Value;

                UpdateProfileGenderResponseHandler ResponseHandler = null;
                var postUrl = $"https://api.{Domain}/1.1/account/personalization/p13n_preferences.json";
                var referer = $"https://{Domain}/settings/your_twitter_data";

                var postData = "";
                if (gender.Equals("female"))
                    postData =
                        "{\"preferences\":{\"gender_preferences\":{\"gender_override\":{\"type\":\"female\",\"value\":\"female\"}}}}";
                else if (gender.Equals("male"))
                    postData =
                        "{\"preferences\":{\"gender_preferences\":{\"gender_override\":{\"type\":\"male\",\"value\":\"male\"}}}}";
                else
                    postData =
                        "{\"preferences\":{\"gender_preferences\":{\"gender_override\":{\"type\":\"custom\",\"value\":\"Not specified\"}}}}";

                var reqParams = _httpHelper.GetRequestParameter();
                SetCsrfToken(ref tdRequestParameter,Path:postUrl,Method:"POST", GuestID: GuestID(twitterAccount));
                reqParams.Headers = tdRequestParameter.Headers;
                reqParams.Referer = referer;
                reqParams.Accept = "text/plain, */*; q=0.01";
                reqParams.ContentType = "application/json; charset=utf-8";
                _httpHelper.SetRequestParameter(reqParams);

                ResponseHandler =
                    new UpdateProfileGenderResponseHandler(_httpHelper.PostRequest(postUrl, postData));

                return ResponseHandler;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public UpdateProfileWebsiteUrlResponseHandler UpdateProfileWebsite(DominatorAccountModel twitterAccount,
            string websiteUrl)
        {
            try
            {
                var tdRequestParameter = (TdRequestParameters)_httpHelper.GetRequestParameter();
                var account = new AccountModel(twitterAccount);
                var UserProfileDetails = new UserProfileDetails();

                #region user details

                try
                {
                    var serializedUserProfileDetails =
                        twitterAccount.ExtraParameters[ModuleExtraDetails.UserProfileDetails.ToString()];
                    UserProfileDetails =
                        JsonConvert.DeserializeObject<UserProfileDetails>(serializedUserProfileDetails);
                    if (string.IsNullOrEmpty(account.CsrfToken))
                        account.CsrfToken = UserProfileDetails.csrfToken;
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                #endregion

                websiteUrl = websiteUrl.Trim();
                var reqParams = _httpHelper.GetRequestParameter();
                UpdateProfileWebsiteUrlResponseHandler ResponseHandler = null;
                var postData = $"displayNameMaxLength=50&url={WebUtility.UrlEncode(websiteUrl)}";
                var ReqUrl = $"https://{Domain}/i/api/1.1/account/update_profile.json";
                var cookies = twitterAccount.Cookies.OfType<Cookie>();
                SetHeaderForEditProfile($"https://{Domain}/settings/profile",
                    cookies.FirstOrDefault(x => x.Name == "ct0" && x.Domain == ".x.com").Value
                        ?? cookies.FirstOrDefault(x => x.Name == "ct0").Value);
                reqParams.Headers = tdRequestParameter.Headers;
                _httpHelper.SetRequestParameter(reqParams);

                ResponseHandler =
                    new UpdateProfileWebsiteUrlResponseHandler(_httpHelper.PostRequest(ReqUrl, postData));
                return ResponseHandler;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private string GetPhoneNumberPostData(string DevicePageSource, string contactNumber,
            UserProfileDetails UserDetails, string confirmationCode = null)
        {
            var PhoneNumberPostData = "";
            try
            {
                var CountryIsoCode =
                    Utilities.GetBetween(HtmlAgilityHelper.MethodGetStringFromId(DevicePageSource, "iso_code"),
                        "value=\"", "\""); //IN
                var CountryCode =
                    HtmlAgilityHelper.MethodGetInnerStringFromId(DevicePageSource, "country_code_display"); //91

                if (string.IsNullOrEmpty(confirmationCode))
                    PhoneNumberPostData =
                        $"device_type=phone&authenticity_token={UserDetails.authenticityToken}&device%5Bcountry_code%5D={CountryCode}&country_code={CountryCode}&edit_phone=false&iso_code={CountryIsoCode}&phone_number={contactNumber}";
                else
                    PhoneNumberPostData =
                        $"device_type=phone&authenticity_token={UserDetails.authenticityToken}&phone_number=%2B{CountryCode.Replace("+", "")}{contactNumber}&iso_code=IN&resend=false&numeric_pin={confirmationCode}&edit_phone=false";
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return PhoneNumberPostData;
        }

        public void SetHeaderForEditProfile(string referer, string csrfToken)
        {
            try
            {
                var objTdRequestParameters = _httpHelper.GetRequestParameter();
                objTdRequestParameters.Headers.Clear();
                objTdRequestParameters.Accept = "*/*";
                objTdRequestParameters.KeepAlive = true;
                objTdRequestParameters.AddHeader("Host", "x.com");
                objTdRequestParameters.AddHeader("sec-ch-ua", "\"Chromium\";v=\"88\", \"Google Chrome\";v=\"88\", \";Not A Brand\";v=\"99\"");
                objTdRequestParameters.AddHeader("x-twitter-client-language", "en");
                objTdRequestParameters.AddHeader("x-csrf-token", csrfToken);
                objTdRequestParameters.AddHeader("authorization", "Bearer AAAAAAAAAAAAAAAAAAAAANRILgAAAAAAnNwIzUejRCOuH5E6I8xnZz4puTs%3D1Zv7ttfk8LF81IUq16cHjhLTvJu4FA33AGWWjCpTnA");
                objTdRequestParameters.AddHeader("x-twitter-auth-type", "OAuth2Session");
                objTdRequestParameters.AddHeader("x-twitter-active-user", "yes");
                objTdRequestParameters.AddHeader("Accept-Language", "en-US,en;q=0.9");
                objTdRequestParameters.Referer = referer;
                objTdRequestParameters.UserAgent = "Mozilla/5.0 (Windows NT 6.3; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/88.0.4324.104 Safari/537.36";
                objTdRequestParameters.ContentType = "application/x-www-form-urlencoded";
            }
            catch (Exception ex)
            {

                ex.DebugLog();
            }
        }

        public void SetBrowser(DominatorAccountModel dominatorAccountModel, CancellationToken cancellationToken,
            BrowserInstanceType browserInstanceType = BrowserInstanceType.Primary)
        {
        }


        public bool BrowserLogin(DominatorAccountModel account, CancellationToken cancellationToken,
            LoginType loginType = LoginType.AutomationLogin, VerificationType verificationtype = VerificationType.Email)
        {
            return true;
        }

        public List<TagDetails> GetTweetListFromNotification(DominatorAccountModel twitterAccount)
        {
            List<TagDetails> ListTagDetails = new List<TagDetails>();

            try
            {
                var account = new AccountModel(twitterAccount);

                #region initializing value

                
                bool following = false, followBack = false, Ismute = false, Isverfied = false, Isprotected = false, IsLiked = false, IsRetweeted = false;
                string UserId = string.Empty, TweeetId = string.Empty;

                 var requestUrl =
                    $"https://api.{Domain}/2/notifications/all.json?include_profile_interstitial_type=1&include_blocking=1&include_blocked_by=1&include_followed_by=1&include_want_retweets=1&include_mute_edge=1&include_can_dm=1&include_can_media_tag=1&skip_status=1&cards_platform=Web-12&include_cards=1&include_ext_alt_text=true&include_quote_count=true&include_reply_count=1&tweet_mode=extended&include_entities=true&include_user_entities=true&include_ext_media_color=true&include_ext_media_availability=true&send_error_codes=true&simple_quoted_tweet=true&count=20&ext=mediaStats%2ChighlightedLabel";
                var tdRequestParameter = (TdRequestParameters)_httpHelper.GetRequestParameter();
                SetCsrfToken(ref tdRequestParameter,Path:requestUrl,Method:"POST", GuestID: GuestID(twitterAccount));
                var jsonNotificationData = _httpHelper.GetRequest(requestUrl);

                #endregion
                
                var Jssonhand = new Jsonhandler(jsonNotificationData.Response);

                var Notification = Jssonhand.GetJToken("globalObjects", "notifications");
                var users = Jssonhand.GetJToken("globalObjects", "users");
                var tweets = Jssonhand.GetJToken("globalObjects", "tweets");
                foreach (var NotificationItem in Notification)
                {
                    var Token = NotificationItem.First();
                    var messagetext = string.Empty;
                    var time = Jssonhand.GetJTokenValue(Token, "timestampMs");
                    long Time = 0;
                    long.TryParse(time, out Time);
                    if (DateTime.Now.GetCurrentEpochTimeMilliSeconds() - Time > 86400)
                        messagetext = Jssonhand.GetJTokenValue(Token, "message", "text");
                    if (messagetext.StartsWith("In case you missed"))
                    {

                        List<string> ListImagePath = new List<string>();
                        var cursorValue = Jssonhand.GetJTokenValue(
                                                    Jssonhand.GetJToken("timeline", "instructions", 1, "addEntries", "entries").Last(), "content",
                                                    "operation", "cursor", "value");
                        var MinPosition = Uri.EscapeDataString(cursorValue);
                        var userName = string.Empty;
                        var profileImage = string.Empty;


                        foreach (var item in tweets)
                        {
                            ListImagePath.Clear();
                            var token = item.First();
                            var TwtText = Jssonhand.GetJTokenValue(token, "full_text");
                            var ListImagePathtoken = Jssonhand.GetJTokenOfJToken(token, "extended_entities", "media");
                            foreach (var Imagepath in ListImagePathtoken)
                            {
                                var images = Jssonhand.GetJTokenValue(Imagepath, "media_url_https");
                                var url = Jssonhand.GetJTokenValue(Imagepath, "url");
                                if (TwtText.Contains(url))
                                    TwtText = TwtText.Replace(url, "");
                                ListImagePath.Add(images);
                            }

                            if (TwtText.Contains("pic.x.com/")) TwtText = Regex.Split(TwtText, "pic.x.com/")[0];
                            var RetweetCount = Jssonhand.GetJTokenValue(token, "retweet_count");
                            var dt = TdTimeStampUtility.ConvertTimestamp(Jssonhand, token);
                            var TwtTimeStamp = dt.ConvertToEpoch();
                            if (token["extended_entities"] != null
                                ? token["extended_entities"]["media"][0]["video_info"] != null
                                : false)
                            {
                                ListImagePath.Clear();
                                var ContentType = Jssonhand.GetJTokenOfJToken(item.First(), "extended_entities", "media", 0,
                                    "video_info", "variants");
                                foreach (var videos in ContentType)
                                {
                                    var contentstype = Jssonhand.GetJTokenValue(videos, "bitrate");
                                    if (!string.IsNullOrEmpty(contentstype))
                                    {
                                        ListImagePath.Add(Jssonhand.GetJTokenValue(videos, "url"));
                                        break;
                                    }
                                }
                            }

                            IsLiked = Jssonhand.GetJTokenValue(token, "favorited") == "True";
                            IsRetweeted = Jssonhand.GetJTokenValue(token, "retweeted") == "True";
                            var CommentCount = Jssonhand.GetJTokenValue(token, "reply_count");
                            var TweetId = Jssonhand.GetJTokenValue(token, "id_str");
                            var LikeCount = Jssonhand.GetJTokenValue(token, "favorite_count");
                            var userid = Jssonhand.GetJTokenValue(token, "user_id_str");

                            foreach (var userdetail in users)
                            {
                                var jToken = userdetail.First();
                                UserId = Jssonhand.GetJTokenValue(jToken, "id_str");
                                if (UserId == userid)
                                {
                                    userName = Jssonhand.GetJTokenValue(jToken, "screen_name");
                                    profileImage = Jssonhand.GetJTokenValue(jToken, "profile_image_url");
                                    following = Jssonhand.GetJTokenValue(jToken, "following") == "True";
                                    followBack = Jssonhand.GetJTokenValue(jToken, "followed_by") == "True";
                                    Ismute = Jssonhand.GetJTokenValue(jToken, "muting") == "True";
                                    Isverfied = Jssonhand.GetJTokenValue(jToken, "verified") == "True";
                                    Isprotected = Jssonhand.GetJTokenValue(jToken, "protected") == "True";
                                    break;
                                }
                            }

                            ListTagDetails.Add(new TagDetails
                            {
                                Id = TweetId,
                                Username = userName,
                                UserId = userid,
                                DateTime = dt,
                                Caption = HttpUtility.HtmlDecode(TwtText),
                                TweetedTimeStamp = TwtTimeStamp,
                                Code = string.Join("\n", ListImagePath),
                                IsAlreadyLiked = IsLiked,
                                IsAlreadyRetweeted = IsRetweeted,
                                ProfilePicUrl = profileImage,
                                //IsRetweet = item.Contains("Icon--retweeted"),
                                IsVerified = Isverfied,
                                IsMuted = Ismute,
                                CommentCount = string.IsNullOrEmpty(CommentCount) ? 0 : int.Parse(CommentCount),
                                RetweetCount = string.IsNullOrEmpty(RetweetCount) ? 0 : int.Parse(RetweetCount),
                                LikeCount = string.IsNullOrEmpty(LikeCount) ? 0 : int.Parse(LikeCount),
                                FollowStatus = following,
                                FollowBackStatus = followBack,
                                IsPrivate = Isprotected,
                                HasProfilePic = profileImage.Contains("profile_images"),
                                IsTweetContainedVideo = token["extended_entities"] != null
                                    ? token["extended_entities"]["media"][0]["video_info"] != null
                                    : false
                            });
                        }
                    }
                }


            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return ListTagDetails;
        }
        private async Task<TdRequestParameters> SetLoginHeader(DominatorAccountModel twitterAccount, string url)
        {
            var tdRequestParameter = (TdRequestParameters)_httpHelper.GetRequestParameter();
            try
            {
                tdRequestParameter.Headers.Clear();
                var account = new AccountModel(twitterAccount);
                tdRequestParameter.ContentType = "application/json";
                tdRequestParameter.Referer = TdConstants.MainUrl;
                tdRequestParameter.Headers.Add("authorization", "Bearer AAAAAAAAAAAAAAAAAAAAANRILgAAAAAAnNwIzUejRCOuH5E6I8xnZz4puTs%3D1Zv7ttfk8LF81IUq16cHjhLTvJu4FA33AGWWjCpTnA");
                var guestToken = tdRequestParameter.Cookies["gt"]?.Value;
                var guestID = GuestID(twitterAccount);
                if (string.IsNullOrEmpty(guestID))
                    guestID = tdRequestParameter.Cookies["guest_id"]?.Value;
                if (!string.IsNullOrEmpty(guestToken))
                    tdRequestParameter.Headers.Add("x-guest-token", guestToken);
                else
                    tdRequestParameter.Headers.Add("x-guest-token", "1800479288640368883");
                tdRequestParameter.Headers.Add("sec-ch-ua", "\"Not)A;Brand\";v=\"8\", \"Chromium\";v=\"138\", \"Google Chrome\";v=\"138\"");
                tdRequestParameter.Headers.Add("x-twitter-client-language", "en");
                tdRequestParameter.Headers.Add("sec-ch-ua-mobile", "?0");
                tdRequestParameter.Headers.Add("x-twitter-active-user", "no");
                var id = await Task.Run(() => TdUtility.GetXClientTransactionID("POST", url));
                id = string.IsNullOrEmpty(id) ? TdUtility.GetTransactionID(SearchType.Login) : id;
                if (!string.IsNullOrEmpty(id))
                    tdRequestParameter.Headers.Add("x-client-transaction-id", id);
                tdRequestParameter.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/138.0.0.0 Safari/537.36";
                tdRequestParameter.Headers.Add("DNT", "1");
                var forwarded = TdUtility.GetXForwardedFor(guestID);
                if (!string.IsNullOrEmpty(forwarded))
                    tdRequestParameter.Headers.Add("x-xp-forwarded-for", forwarded);
                tdRequestParameter.Accept = "*/*";
                tdRequestParameter.Headers.Add("Sec-Fetch-Site", "same-site");
                tdRequestParameter.Headers.Add("Sec-Fetch-Mode", "cors");
                tdRequestParameter.Headers.Add("Sec-Fetch-Dest", "empty");
                tdRequestParameter.Headers.Add("Accept-Language", "en-US,en;q=0.9");
                tdRequestParameter.Headers.Add("Accept-Encoding", "gzip, deflate");
                tdRequestParameter.Headers.Remove("x-twitter-auth-type");
                _httpHelper.SetRequestParameter(tdRequestParameter);
            }
            catch
            {

            }
            return tdRequestParameter;
        }
        public async Task<LogInResponseHandler> LoginUsingFlowToken(DominatorAccountModel twitterAccount, CancellationToken cancellationToken)
        {
            IResponseParameter LoginResponse = new ResponseParameter();
            try
            {
                var url = $"https://api.x.com/1.1/onboarding/task.json?flow_name=login";
                var tdRequestParameter = await SetLoginHeader(twitterAccount, url);
                var postData = "{\"input_flow_data\":{\"flow_context\":{\"debug_overrides\":{},\"start_location\":" +
                    "{\"location\":\"splash_screen\"}}},\"subtask_versions\":{\"action_list\":2,\"alert_dialog\":1," +
                    "\"app_download_cta\":1,\"check_logged_in_account\":1,\"choice_selection\":3,\"contacts_live_sync_permission_prompt\":0," +
                    "\"cta\":7,\"email_verification\":2,\"end_flow\":1,\"enter_date\":1,\"enter_email\":2,\"enter_password\":5," +
                    "\"enter_phone\":2,\"enter_recaptcha\":1,\"enter_text\":5,\"enter_username\":2,\"generic_urt\":3,\"in_app_notification\":1," +
                    "\"interest_picker\":3,\"js_instrumentation\":1,\"menu_dialog\":1,\"notifications_permission_prompt\":2,\"open_account\":2," +
                    "\"open_home_timeline\":1,\"open_link\":1,\"phone_verification\":4,\"privacy_options\":1,\"security_key\":3," +
                    "\"select_avatar\":4,\"select_banner\":2,\"settings_list\":7,\"show_code\":1,\"sign_up\":2,\"sign_up_review\":4," +
                    "\"tweet_selection_urt\":1,\"update_users\":1,\"upload_media\":1,\"user_recommendations_list\":4,\"user_recommendations_urt\":1," +
                    "\"wait_spinner\":3,\"web_modal\":1}}";
                var response = _httpHelper.PostRequest(url, postData);
                var responseJson = await GetResponseJson(twitterAccount);
                var JsInstrumentalResponse = string.IsNullOrEmpty(responseJson) ?
                    "{\\\"rf\\\":{\\\"ebe9c0b31d999ec0b38a518c15e6e2efa3964759432df910cd6412a1926bc862\\\":56,"
                    + "\\\"a9aa514b9752cc0aa2b4ef8ea4b88b2ad950c6a31bb4bc1635637a3436d3ba1f\\\":-9,\\\"a3ea40614c5e4c7dc005295ea49533073733760618d5e087dab0c52059472afe\\\":202,"
                    + "\\\"a9a9dae65fcbf3c4f86d53f30083c48e3af1a9919b95ff57cc25fa6fad690711\\\":0},"
                    + "\\\"s\\\":\\\"_LXye-ZeE8LVoebXAdZ32vl1hKkE14BF2JaztUmtOIABMWYyBX6YLxTnJQFhP08L54IYYShMJyiSSmD3AbT6ZMWOi45QjYQQ2WwnIAwomb0H9WrRZLi3EJhNg42gEBqJ50bnVNwGfoxi5SAtp2OKDw-olzfheY-ewyDp2Xa_9-b_fWkpI8zV7QgzngtlFPlk5rp3rlaEUZfsUEfJHrxBjAYWc7AJk9orAH-4Ttr82lDbzHSDmONLPun97CCd_Zt5arcUp4x6_sJH8dzRv0_4gf2wEtMEpkiiwvtaW6m7K0pq8yhBx2kPyhjW7dTA6HnMEgkbO9j3lA2DGLGy1v4lmwAAAYQ87nwC\\\"}"
                    : responseJson?.Replace("\"", "\\\"");
                var flowToken = GetFlowToken(response?.Response);
                tdRequestParameter.ContentType = "application/json";
                postData = "{\"flow_token\":\"" + flowToken + "\",\"subtask_inputs\":[{\"subtask_id\":\"LoginJsInstrumentationSubtask\"," +
                    $"\"js_instrumentation\":{{\"response\":\"{JsInstrumentalResponse}\"," +"\"link\":\"next_link\"}}]}";
                url = $"https://{Domain}/i/api/1.1/onboarding/task.json";
                tdRequestParameter = await SetLoginHeader(twitterAccount, url);
                response = _httpHelper.PostRequest(url, postData);
                flowToken = GetFlowToken(response?.Response);
                postData = "{\"flow_token\":\"" + flowToken + "\",\"subtask_inputs" +
                    "\":[{\"subtask_id\":\"LoginEnterUserIdentifierSSO\",\"settings_list\":{" +
                    "\"setting_responses\":[{\"key\":\"user_identifier\",\"response_data\":{\"text_data\":{" +
                    "\"result\":\"" + twitterAccount.AccountBaseModel.UserName?.Replace("@", "") + "\"}}}],\"link\":\"next_link\"}}]}";
                url = $"https://{Domain}/i/api/1.1/onboarding/task.json";
                tdRequestParameter = await SetLoginHeader(twitterAccount, url);
                response = _httpHelper.PostRequest(url, postData);
                if (InvalidAccounts(response?.Response))
                    return new LogInResponseHandler(response);
                flowToken = GetFlowToken(response?.Response);
                postData = "{\"flow_token\":\"" + flowToken + "\",\"subtask_inputs\"" +
                    ":[{\"subtask_id\":\"LoginEnterPassword\",\"enter_password\":" +
                    "{\"password\":\"" + twitterAccount.AccountBaseModel.Password + "\",\"link\":\"next_link\"}}]}";

                url = $"https://{Domain}/i/api/1.1/onboarding/task.json";
                tdRequestParameter = await SetLoginHeader(twitterAccount, url);
                //SetCsrfToken(ref tdRequestParameter, Path: url, Method: "POST", GuestID: GuestID(twitterAccount));
                response = _httpHelper.PostRequest(url, postData);
                if (InvalidAccounts(response?.Response))
                    return new LogInResponseHandler(response);
                if (response != null && !string.IsNullOrEmpty(response?.Response)
                    && response.Response.Contains("\"status\":\"success\""))
                {
                    var obj = handler.ParseJsonToJObject(response?.Response);
                    if (string.IsNullOrEmpty(twitterAccount.AccountBaseModel.UserId))
                        twitterAccount.AccountBaseModel.UserId = handler.GetJTokenValue(obj, "subtasks", 0, "open_account", "user", "id");
                    if (string.IsNullOrEmpty(twitterAccount.AccountBaseModel.UserName)
                        || twitterAccount.AccountBaseModel.UserName.Contains("@"))
                        twitterAccount.AccountBaseModel.UserName = handler.GetJTokenValue(obj, "subtasks", 0, "open_account", "user", "screen_name");
                    if (string.IsNullOrEmpty(twitterAccount.AccountBaseModel.UserFullName))
                        twitterAccount.AccountBaseModel.UserFullName = handler.GetJTokenValue(obj, "subtasks", 0, "open_account", "user", "name");
                }
                flowToken = GetFlowToken(response?.Response);
                postData = "{\"flow_token\":\"" + flowToken + "\",\"subtask_inputs\":[{" +
                    "\"subtask_id\":\"AccountDuplicationCheck\",\"check_logged_in_account\":{" +
                    "\"link\":\"AccountDuplicationCheck_false\"}}]}";
                url = $"https://{Domain}/i/api/1.1/onboarding/task.json";
                SetCsrfToken(ref tdRequestParameter, isJsonRequest: true, Path: url, Method: "POST", GuestID: GuestID(twitterAccount));
                LoginResponse = await _httpHelper.PostRequestAsync(url, postData, cancellationToken);
            }
            catch
            {
                twitterAccount.IsUserLoggedIn = false;
            }
            var responseHandler = new LogInResponseHandler(LoginResponse);
            return responseHandler;
        }

        private async Task<string> GetResponseJson(DominatorAccountModel twitterAccount)
        {
            var response = string.Empty;
            var browserActivity = new DominatorHouseCore.PuppeteerBrowser.PuppeteerBrowserActivity(twitterAccount);
            try
            {
                var jsResponse = await _httpHelper.GetRequestAsync("https://twitter.com/i/js_inst?c_name=ui_metrics", twitterAccount.Token);
                await browserActivity.LaunchBrowserAsync(true);
                response = await browserActivity.ExecuteJsOnDom(jsResponse?.Response);
            }
            catch { }
            finally
            {
                try
                {
                    browserActivity?.ClosedBrowser();
                }
                catch { }
            }
            return response;
        }

        private bool InvalidAccounts(string response)
        {
            try
            {
                return !string.IsNullOrEmpty(response)
                    && (response.Contains("Sorry, we could not find your account")
                    ||response.Contains("\"hint_text\":\"Confirmation code\"")
                    ||response.Contains("This request requires a matching csrf cookie and header")
                    ||response.Contains("We blocked an attempt to access your account because we weren't sure it was really you"));
            }
            catch { return false; }
        }

        private string GetFlowToken(string response)
        {
            try
            {
                var jObject = handler.ParseJsonToJObject(response);
                if (handler.GetJTokenValue(jObject, "status").Equals("success"))
                {
                    return handler.GetJTokenValue(jObject, "flow_token");
                }
                return string.Empty;
            }
            catch { return string.Empty; }
        }

        public bool GettingTweetMedia(DominatorAccountModel dominatorAccount,TagDetails tagDetails, ref List<string> tweetMedia,ActivityType activityType, string DownloadPath = "")
        {
            var isSuccess = true;
            try
            {
                var tagForProcess = tagDetails;
                var downloadPath = string.IsNullOrEmpty(DownloadPath) ?Environment.GetFolderPath(Environment.SpecialFolder.MyPictures): DownloadPath;
                if (tagForProcess.IsTweetContainedVideo)
                {
                    //var fileName = _twtFunc.DownloadVideoUsingThirdParty(tagForProcess.Id,
                    //    tagForProcess.Username, RepostModel.DownloadFolderPath, tagForProcess.Id);
                    var fileName = new List<string>();
                    if (fileName.Count == 0)
                        fileName = TdUtility.DownloadFileFromTwitter(tagForProcess.Code, ".mp4", downloadPath, tagForProcess.Id);

                    tweetMedia.AddRange(fileName);

                    //TODO : If you make CheckDuration non static, you can add cover this if-case in ReposterProcessTests
                    if (TdUtility.CheckDuration(new Uri(tweetMedia[0])))
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage,
                            dominatorAccount.AccountBaseModel.AccountNetwork, dominatorAccount.UserName,
                            activityType, "couldn't repost with video size more than 140secs");
                        isSuccess = false;
                    }
                }
                else
                {
                    tweetMedia = string.IsNullOrEmpty(tagForProcess.Code)
                        ? null
                        : Regex.Split(tagForProcess.Code, "\n").ToList();
                }
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                //TODO : If you make TdUtility non static and cover it tests, you can uncommit this line
                //isSuccess = false;
                ex.DebugLog();
            }

            return isSuccess;
        }

        public async Task<ProfileDetailsResponseHandler> GetProfileDetails(DominatorAccountModel dominatorAccount, string UserName)
        {
            ProfileDetailsResponseHandler responseHandler = null;
            try
            {
                var tdRequestParameter = (TdRequestParameters)_httpHelper.GetRequestParameter();
                var reqUrl = TdConstants.GetUserProfileUrl(UserName);
                SetCsrfToken(ref tdRequestParameter, Path: reqUrl, Method: "GET", GuestID: GuestID(dominatorAccount));
                responseHandler = new ProfileDetailsResponseHandler(await _httpHelper.GetRequestAsync(TdConstants.GetUserProfileUrl(UserName), dominatorAccount.Token));
            }catch (Exception) { }
            return responseHandler;
        }

        public void SetCsrfToken(ref TdRequestParameters tdRequestParameters, bool isJsonRequest = false, SearchType type = SearchType.None,string Response="",string Path="", string Method = "", string GuestID = "")
        {
            //tdRequestParameters.Headers.Clear();
            var CsrfToken = tdRequestParameters?.Cookies?.OfType<Cookie>()?.FirstOrDefault(x => x.Name == "ct0" && x.Domain == ".x.com")?.Value
                ?? tdRequestParameters.Cookies?.OfType<Cookie>()?.FirstOrDefault(x => x.Name == "ct0")?.Value;
            tdRequestParameters.Cookies.Add(new Cookie { Name="dnt", Value = "1", Domain = ".x.com" });
            tdRequestParameters.SetupHeaders("Json", CsrfToken,IsJsonRequest: isJsonRequest,type,Response,Path, Method,GuestID);
        }

        public async Task<bool> SolveFunCaptcha(DominatorAccountModel dominatorAccount, ChallengeDetails challengeDetails)
        {
            try
            {
                var RequestParam = _httpHelper.GetRequestParameter();
                var contentType = RequestParam.ContentType;
                RequestParam.ContentType = "application/x-www-form-urlencoded";
                RequestParam.Cookies = dominatorAccount.Cookies;
                var AccountAccessAPI = "https://x.com/account/access";
                RequestParam.Referer = AccountAccessAPI;
                _httpHelper.SetRequestParameter(RequestParam);
                var ChallengePostBody = $"authenticity_token={challengeDetails.PostAuthenticityToken}&assignment_token={challengeDetails.AssignmentToken}&lang={challengeDetails.lang}&flow=&verification_string={WebUtility.UrlEncode(challengeDetails.VerificationString)}&language_code={challengeDetails.lang}";
                var AccessResponse = _httpHelper.PostRequest(AccountAccessAPI, ChallengePostBody);
                RequestParam.ContentType = contentType;
                _httpHelper.SetRequestParameter(RequestParam);
                return AccessResponse != null && AccessResponse.Response.Contains("Account unlocked");
            }
            catch { return false; }
        }

        public async Task<string> GetUserID(DominatorAccountModel dominatorAccount, string UserName)
        {
            var userId = string.Empty;
            try
            {
                var userDetailsUrl = TdConstants.GetUserDetailsFromFollowersResponse(UserName.ToLower());
                var tdRequestParameter = (TdRequestParameters)_httpHelper.GetRequestParameter();
                SetCsrfToken(ref tdRequestParameter, true, SearchType.CustomUser, Path: userDetailsUrl, Method: "GET", GuestID: GuestID(dominatorAccount));
                _httpHelper.SetRequestParameter(tdRequestParameter);
                var response = await _httpHelper.GetRequestAsync(userDetailsUrl,dominatorAccount.Token);
                userId = Utilities.GetBetween(response?.Response, "\"id_str\":\"", "\"");
                userId = string.IsNullOrEmpty(userId)? Utilities.GetBetween(response?.Response, "\"rest_id\":\"", "\"") : userId;
            }
            catch { }
            return userId;
        }

        public RepostTweetResponseHandler RepostTweet(DominatorAccountModel dominatorAccount, string TweetUrl)
        {
            try
            {
                var tdRequestParameter = (TdRequestParameters)_httpHelper.GetRequestParameter();
                var account = new AccountModel(dominatorAccount);
                var reqUrl = TdConstants.GetReTweetAPI;
                var id = TdUtility.GetTweetIdFromUrl(TweetUrl);
                var postData = Encoding.UTF8.GetBytes(TdConstants.GetPostDataForReTweet(id));
                SetCsrfToken(ref tdRequestParameter,true,SearchType.Repost,Path:reqUrl,Method:"POST", GuestID: GuestID(dominatorAccount));
                tdRequestParameter.Referer = TdConstants.MainUrl;
                _httpHelper.SetRequestParameter(tdRequestParameter);
                var ResponseHandler = new RepostTweetResponseHandler(_httpHelper.PostRequest(reqUrl, postData));
                return ResponseHandler;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public string GuestID(DominatorAccountModel dominatorAccount)
        {
            try
            {
                return dominatorAccount.Cookies.OfType<Cookie>()
                .FirstOrDefault(x => x.Name == "guest_id" && x.Domain == ".x.com")?.Value
                ?? dominatorAccount.Cookies.OfType<Cookie>().FirstOrDefault(x => x.Name == "guest_id")?.Value;
            }
            catch { return string.Empty; }
        }

        public async Task<BookMarkTweetResponseHandler> BookMarkTweet(DominatorAccountModel dominatorAccount, string ID)
        {
            try
            {
                var tdRequestParameter = (TdRequestParameters)_httpHelper.GetRequestParameter();
                var account = new AccountModel(dominatorAccount);
                var reqUrl = TdConstants.BookMarkAPI;
                var postData = Encoding.UTF8.GetBytes(TdConstants.BookMarkBody(ID));
                SetCsrfToken(ref tdRequestParameter, true, SearchType.BookMark, Path: reqUrl, Method: "POST", GuestID: GuestID(dominatorAccount));
                tdRequestParameter.Referer = TdConstants.MainUrl;
                _httpHelper.SetRequestParameter(tdRequestParameter);
                return new BookMarkTweetResponseHandler(await _httpHelper.PostRequestAsync(reqUrl, postData,dominatorAccount.Token));
            }
            catch { return null; }
        }

        #endregion

    }
}