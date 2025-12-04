using DominatorHouseCore;
using DominatorHouseCore.Enums.TumblrQuery;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using TumblrDominatorCore.Interface;
using TumblrDominatorCore.Models;
using TumblrDominatorCore.TmblrUtility;
using TumblrDominatorCore.TumblrRequest;
using TumblrDominatorCore.TumblrResponseHandler;
using JsonHandler = DominatorHouseCore.Utility.JsonHandler;

namespace TumblrDominatorCore.TumblrLibrary.TumblrFunction
{
    public interface ITumblrFunct
    {
        string Csrf_Token { get; set; }
        string Participant_key { get; set; }


        /// <summary>
        ///     Log In Account
        /// </summary>
        /// <returns></returns>
        LogInResponseHandler LogIn(DominatorAccountModel dominatorAccount);

        /// <summary>
        ///     Follow Function
        /// </summary>
        /// <returns></returns>
        FollowResponseHandler Follow(DominatorAccountModel tumbleraccount, TumblrUser tumblrUser, string tumblrFormKey);

        /// <summary>
        ///     Api Response
        /// </summary>
        /// <param name="tumbleraccount"></param>
        /// <param name="url"></param>
        /// <param name="bearerToken"></param>
        /// <returns></returns>
        ApiResponseHandler GetApiResponse(DominatorAccountModel tumbleraccount, string url, string bearerToken, string referer = "");
        /// <summary>
        ///     UnFollow Function
        /// </summary>
        /// <returns></returns>
        UnFollowResponseHandler UnFollow(DominatorAccountModel tumbleraccount, TumblrUser userName,
            string tumblrFormKey);


        SearchLikedPostResponse SearchLikedPost(DominatorAccountModel tumbleraccount, string nextPage);





        /// <summary>
        ///     Search Posts from Hashtag
        /// </summary>
        /// <param name="tumbleraccount"></param>
        /// <param name="keyword"></param>
        /// <param name="pageId"></param>
        /// <param name="xTumblrFormKey"></param>
        /// <returns></returns>
        SearchPostsResonseHandler SearchPostsWithHashTag(DominatorAccountModel tumbleraccount, string url, string referer = "");

        /// <summary>
        ///     Search Posts from Queries
        /// </summary>
        /// <param name="tumbleraccount"></param>
        /// <param name="keyword"></param>
        /// <param name="pageId"></param>
        /// <param name="xTumblrFormKey"></param>
        /// <returns></returns>
        SearchPostsResonseHandler ScrapePostsfromkeyword(DominatorAccountModel tumbleraccount, string url, string referer = "");

        /// <summary>
        ///     Search Users with some one's following
        /// </summary>
        /// <param name="tumbleraccount"></param>
        /// <param name="username"></param>
        /// <param name="pageId"></param>
        /// <returns></returns>
        SearchforFollowingsorFollowersResponse SearchUsernameForFollowing(DominatorAccountModel tumbleraccount,
            string username, int pageId, string Pageurl = "");

        /// <summary>
        ///     Search some one page feeds
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="xTumblrFormKey"></param>
        /// <param name="pageId"></param>
        /// <returns></returns>
        SearchPostForUserRespones SearchPostFromUser(string userName, string xTumblrFormKey, int pageId = 0,
            CookieContainer cookies = null);

        /// <summary>
        ///     Like the tumblr post
        /// </summary>
        /// <param name="tumbleraccount"></param>
        /// <param name="tumblrPost"></param>
        /// <param name="tumblrFormkey"></param>
        /// <returns></returns>
        LikePostResponse LikePost(DominatorAccountModel tumbleraccount, TumblrPost tumblrPost, string tumblrFormkey);

        UnLikePostResponse UnLikePost(DominatorAccountModel tumbleraccount, TumblrPost tumblrPost,
            string tumblrFormkey);

        PostScraperResponse PostScraper(DominatorAccountModel tumbleraccount, TumblrPost tumblrPost,
            string tumblrFormkey);

        CommentScraperResponse CommentScraper(DominatorAccountModel tumbleraccount, TumblrPost tumblrPost,
            string tumblrFormkey);


        UserScraperResponse UserScraper(DominatorAccountModel tumbleraccount, TumblrUser tumblrUser,
            string tumblrFormkey);

        /// <summary>
        ///     Comment the tumblr post
        /// </summary>
        /// <param name="tumbleraccount"></param>
        /// <param name="tumblrPost"></param>
        /// <param name="tumblrFormkey"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        CommentPostResponse CommentPost(DominatorAccountModel tumbleraccount, TumblrPost tumblrPost,
            string tumblrFormkey, string message);

        /// <summary>
        ///     Reblog the tumblr post
        /// </summary>
        /// <param name="tumblrPost"></param>
        /// <param name="tumblrFormkey"></param>
        /// <returns></returns>
        ReblogPostResponse ReblogPost(DominatorAccountModel tumbleraccount, TumblrPost tumblrPost,
            string tumblrFormkey);

        /// <summary>
        ///     Upload Posts and get the PhotoUrl Out
        /// </summary>
        /// <param name="tumblrFormkey"></param>
        /// <param name="dominatorAccountModel"></param>
        /// <param name="postDetails"></param>
        /// <returns></returns>
        PublishResponseHandler JobforPublishPost(string tumblrFormkey, DominatorAccountModel dominatorAccountModel,
            PublisherPostlistModel postDetails);

        /// <summary>
        ///     Publish Posts on Tumblr Blog
        /// </summary>
        /// <param name="postDetails"></param>
        /// <param name="tumblrFormkey"></param>
        /// <param name="uploadurl"></param>
        /// <param name="blogName"></param>
        /// <returns></returns>
        PublishResponseHandler PublishPost(DominatorAccountModel tumbleraccount, PublisherPostlistModel postDetails,
            string tumblrFormkey, List<UploadedMediaModel> uploadMedia, string blogName);

        /// <summary>
        ///     Sending normal text Message
        /// </summary>
        /// <param name="tumbleraccount"></param>
        /// <param name="userName"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        MessageUserResponse Messageuser(DominatorAccountModel tumbleraccount, TumblrUser userName, string key);

        /// <summary>
        ///     Get Tumblr user information
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        UserInfoResponeHandler GetUserInfo(DominatorAccountModel tumbleraccount);

        /// <summary>
        ///     Get Account Own Followings
        /// </summary>
        /// <param name="tumbleraccount"></param>
        /// <param name="pageCount"></param>
        /// <returns></returns>
        SearchforFollowingsorFollowersResponse GetAcccountFollowings(DominatorAccountModel tumbleraccount, int pageCount = 0, string Pageurl = "");

        /// <summary>
        ///     Get Account Own Followers
        /// </summary>
        /// <param name="tumbleraccount"></param>
        /// <param name="pageCount"></param>
        /// <returns></returns>
        SearchforFollowingsorFollowersResponse GetAcccountFollowers(DominatorAccountModel tumbleraccount, string nextPageUrl = "");

        /// <summary>
        /// Get other user details
        /// </summary>
        /// <param name="tumbleraccount"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        UserDetailResponseHandler GetUserDetails(DominatorAccountModel tumbleraccount, TumblrUser user);

        /// <summary>
        ///     Get Account Own Followers
        /// </summary>
        /// <param name="username"></param>
        /// <param name="conversation"></param>
        /// <param name="tumbleraccount"></param>
        /// <param name="feedId"></param>
        /// <param name="beforeStamp"></param>
        /// <returns></returns>
        FeedInteractionResponseHandler GetFeedInteractions(DominatorAccountModel tumbleraccount, string feedId,
            string username, QueryInfo queryInfo, string beforeStamp = "");

        /// <summary>
        ///     Requests to Upload Image
        /// </summary>
        /// <param name="account"></param>
        /// <param name="media"></param>
        /// <param name="waterfallId"></param>
        /// <returns></returns>
        UploadedMediaModel UploadMedia(DominatorAccountModel account, string media, string waterfallId);


        string UploadImageinMessage(DominatorAccountModel account, string media, string waterfallId,
            TumblrUser objUser);

        IResponseParameter GetUserDetailsResponseApi(DominatorAccountModel dominatorAccountModel, string username);

        IResponseParameter TumblrServiceApi(DominatorAccountModel dominatorAccountModel, object servicePostData);
        void UpdateCsrf();
    }

    public class TumblrFunct : ITumblrFunct
    {
        private readonly ITumblrHttpHelper HttpHelper;
        private readonly ITumblrAccountSession accountSession;
        public TumblrFunct(ITumblrHttpHelper _httpHelper,ITumblrAccountSession tumblrAccountSession):this(_httpHelper)
        {
            accountSession = tumblrAccountSession;
        }
        public TumblrFunct(ITumblrHttpHelper _httpHelper)
        {
            HttpHelper = _httpHelper;
        }
        public string Csrf_Token { get; set; }
        public string Participant_key { get; set; }
        public string ETag { get; set; }
        public ClientDetails client { get; set; }

        public ApiResponseHandler GetApiResponse(DominatorAccountModel dominatorAccount, string url,
            string bearerToken, string referer = "")
        {
            var failedCount = 0;
        TryAgainOnce:
            accountSession.AddOrUpdateSession(ref dominatorAccount);
            var tdRequestParameter = (TumblrRequestParameter)HttpHelper.GetRequestParameter();
            tdRequestParameter.Accept = "application/json;format=camelcase";
            try
            {
                var scrf = HttpHelper.GetXCsrfTokenFromResp();
                if (!string.IsNullOrEmpty(scrf))
                    Csrf_Token = scrf;
            }
            catch (Exception)
            { }
            tdRequestParameter.Headers["Authorization"] = ConstantHelpDetails.BearerToken;
            tdRequestParameter.Headers.Remove("Origin");
            tdRequestParameter.Headers.Remove("Upgrade-Insecure-Requests");
            tdRequestParameter.Headers.Remove("X-CSRF");
            tdRequestParameter.Headers.Remove("Sec-Fetch-User");
            tdRequestParameter.Headers["X-Version"] = "redpop/3/0//redpop/";
            tdRequestParameter.Headers["Sec-Fetch-Dest"] = "empty";
            tdRequestParameter.Headers["Sec-Fetch-Mode"] = "cors";
            tdRequestParameter.Headers["Sec-Fetch-Site"] = "same-origin";
            tdRequestParameter.Headers["X-Ad-Blocker-Enabled"] = "0";
            //    tdRequestParameter.Headers["Accept-Encoding"] = "gzip, deflate, br, zstd";
            if (!string.IsNullOrEmpty(Csrf_Token))
                tdRequestParameter.Headers["X-CSRF"] = Csrf_Token;
            tdRequestParameter.Headers["Host"] = ConstantHelpDetails.TumblrUrl;
            tdRequestParameter.Referer = string.IsNullOrEmpty(tdRequestParameter.Referer) && string.IsNullOrEmpty(referer) ? ConstantHelpDetails.HomePageUrl :
                 !string.IsNullOrEmpty(referer) ? referer : tdRequestParameter.Referer;
            tdRequestParameter.UserAgent = "Mozilla/5.0 (Linux; Android 10; K) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/127.0.0.0 Mobile Safari/537.36";
            setCookieForRequest(tdRequestParameter, dominatorAccount);
            tdRequestParameter.ContentType = "application/json; charset=utf-8";
            if (!string.IsNullOrEmpty(dominatorAccount.AccountBaseModel.AccountProxy.ProxyIp))
                tdRequestParameter.Proxy = dominatorAccount.AccountBaseModel.AccountProxy;
            HttpHelper.SetRequestParameter(tdRequestParameter);
            var apiResponseHandle = new ApiResponseHandler(HttpHelper.GetRequest(url));
            UpdateCsrf();
            while (failedCount++ <= 4 && (apiResponseHandle == null || string.IsNullOrEmpty(apiResponseHandle?.Response?.Response) || apiResponseHandle.Response.Response.Contains("403 Forbidden")))
                goto TryAgainOnce;
            //Reset UserAgent.
            {
                tdRequestParameter.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/125.0.6316.226 Safari/537.36";
                HttpHelper.SetRequestParameter(tdRequestParameter);
            }
            return apiResponseHandle;
        }
        public UserDetailResponseHandler GetUserDetails(DominatorAccountModel tumbleraccount, TumblrUser tumblrUser)
        {
            var tdRequestParameter = (TumblrRequestParameter)HttpHelper.GetRequestParameter();
            tdRequestParameter.Headers.Remove("If-None-Match");
            tdRequestParameter.Headers.Remove("Upgrade-Insecure-Requests");
            if (!string.IsNullOrEmpty(Csrf_Token)) tdRequestParameter.Headers["X-Requested-With"] = "XMLHttpRequest";
            tdRequestParameter.Accept = "*/*";
            setCookieForRequest(tdRequestParameter, tumbleraccount);
            HttpHelper.SetRequestParameter(tdRequestParameter);
            string url = $"https://www.tumblr.com/svc/tumblelog_popover/{tumblrUser.Username}?is_tumblelog_popover=true&is_user_mention=false";
            return new UserDetailResponseHandler(HttpHelper.GetRequest(url));
        }

        public LogInResponseHandler LogIn(DominatorAccountModel dominatorAccount)
        {
            LogInResponseHandler responseparameterLogin = new LogInResponseHandler();
            try
            {
                if (dominatorAccount.CookieHelperList.Any(x => x.Name.Contains("Etag") && !string.IsNullOrEmpty(x.Value)))
                    ETag = dominatorAccount.Cookies["Etag"].Value;
                var tdRequestParameter = (TumblrRequestParameter)HttpHelper.GetRequestParameter();
                if (tdRequestParameter.Referer != null) tdRequestParameter.Referer = null;
                if (!string.IsNullOrEmpty(ETag)) tdRequestParameter.Headers["If-None-Match"] = ETag;
                if (!string.IsNullOrEmpty(Csrf_Token)) tdRequestParameter.Headers["X-CSRF"] = Csrf_Token;
                tdRequestParameter.Headers["Connection"] = "keep-alive";
                tdRequestParameter.Headers["sec-ch-ua-platform"] = "\"Windows\"";
                tdRequestParameter.Headers["sec-ch-ua-mobile"] = "?0";
                tdRequestParameter.Headers["Sec-Fetch-Dest"] = "document";
                tdRequestParameter.Headers["Sec-Fetch-Mode"] = "navigate";
                tdRequestParameter.Headers["Sec-Fetch-Site"] = "none";
                tdRequestParameter.Headers["Sec-Fetch-User"] = "?1";
                tdRequestParameter.Headers["Upgrade-Insecure-Requests"] = "1";
                tdRequestParameter.Headers.Remove("Authorization");
                tdRequestParameter.Headers.Remove("X-Ad-Blocker-Enabled");
                tdRequestParameter.Headers.Remove("X-Version");
                tdRequestParameter.Headers.Remove("Origin");
                tdRequestParameter.KeepAlive = true;
                if (dominatorAccount.AccountBaseModel.AccountProxy.ProxyIp != null && !string.IsNullOrEmpty(dominatorAccount.AccountBaseModel.AccountProxy.ProxyIp))
                    tdRequestParameter.Proxy = dominatorAccount.AccountBaseModel.AccountProxy;
                setCookieForRequest(tdRequestParameter, dominatorAccount);
                HttpHelper.SetRequestParameter(tdRequestParameter);
                responseparameterLogin = new LogInResponseHandler(HttpHelper.GetRequest(ConstantHelpDetails.LoginUrl));
                if (responseparameterLogin.Success)
                {
                    try
                    {
                        var requireData = Utilities.GetBetween(WebUtility.HtmlDecode(responseparameterLogin.Response.Response), "window['___INITIAL_STATE___'] =", "</script>")?.Trim()?.TrimEnd(';');
                        requireData = string.IsNullOrEmpty(requireData) ? Utilities.GetBetween(WebUtility.HtmlDecode(responseparameterLogin.Response.Response), "id=\"___INITIAL_STATE___\">", "</script>")?.Trim()?.TrimEnd(';') : requireData;
                        JsonHandler hand1 = new JsonHandler(requireData);
                        try
                        {
                            var eTag = HttpHelper.Response.Headers["ETag"];
                            if (!string.IsNullOrEmpty(eTag))
                            {
                                ETag = eTag;
                                dominatorAccount.Cookies.Add(new Cookie("Etag", ETag));
                                tdRequestParameter.Cookies.Add(new Cookie("Etag", ETag));
                            }
                            HttpHelper.SetRequestParameter(tdRequestParameter);
                        }
                        catch
                        { }
                        Csrf_Token = hand1.GetElementValue("csrfToken");
                        client = new ClientDetails
                        {
                            Platform = hand1.GetElementValue("analyticsInfo", "kraken", "clientDetails", "platform"),
                            OsName = hand1.GetElementValue("analyticsInfo", "kraken", "clientDetails", "os_name"),
                            OsVersion = hand1.GetElementValue("analyticsInfo", "kraken", "clientDetails", "os_version"),
                            Language = hand1.GetElementValue("analyticsInfo", "kraken", "clientDetails", "language"),
                            BuildVersion = hand1.GetElementValue("analyticsInfo", "kraken", "clientDetails", "build_version"),
                            FormFactor = hand1.GetElementValue("analyticsInfo", "kraken", "clientDetails", "form_factor"),
                            Model = hand1.GetElementValue("analyticsInfo", "kraken", "clientDetails", "model"),
                            Connection = hand1.GetElementValue("analyticsInfo", "kraken", "clientDetails", "connection"),
                            Carrier = hand1.GetElementValue("analyticsInfo", "kraken", "clientDetails", "carrier"),
                            BrowserName = hand1.GetElementValue("analyticsInfo", "kraken", "clientDetails", "browser_name"),
                            BrowserVersion = hand1.GetElementValue("analyticsInfo", "kraken", "clientDetails", "browser_version"),
                            Manufacturer = hand1.GetElementValue("analyticsInfo", "kraken", "clientDetails", "manufacturer"),
                        };
                        dominatorAccount.SessionId = hand1.GetElementValue("analyticsInfo", "kraken", "sessionId");
                        var servicePostData = new ServicePostData
                        {
                            FlushTime = DateTimeUtilities.GetCurrentEpochTimeMilliSeconds(DateTime.Now),
                            KrakenCLientdetails = client,
                            KrackEvents = new KrackEvents[]
                            {
                                    new KrackEvents{
                                        EventName="SessionStart",
                                        Experiments=new object(),
                                        EventDetails= new EventDetails{
                                        Action="start",
                                        Pathname="/login",
                                        Hostname=ConstantHelpDetails.TumblrUrl
                                    },
                                TimeStamp=DateTimeUtilities.GetCurrentEpochTimeMilliSeconds(DateTime.Now),
                                SessionId=dominatorAccount.SessionId,
                                Page=hand1.GetElementValue("analyticsInfo", "kraken", "basePage")
                        } }
                            ,
                            Trackers = new object[] { }
                        };
                        var response = TumblrServiceApi(dominatorAccount, servicePostData);
                    }
                    catch (Exception)
                    { }
                    if (responseparameterLogin.IsLoggedIn)
                    {
                        UpdateCsrf();
                        var userResponse = GetUserInfo(dominatorAccount);
                        dominatorAccount.CrmUuid = userResponse.TumblrUser.Uuid;
                        dominatorAccount.UserUuid = userResponse.TumblrUser.UserUuid;
                        Participant_key = userResponse.TumblrUser.Uuid;
                        return responseparameterLogin;
                    }
                }
                #region code for Login through credentials
                if (responseparameterLogin.Success && !responseparameterLogin.IsLoggedIn)
                {
                    var postelement = new PostDataElement
                    {
                        Password = dominatorAccount.AccountBaseModel.Password,
                        GrantType = "password",
                        Username = dominatorAccount.UserName
                    };
                    tdRequestParameter = (TumblrRequestParameter)HttpHelper.GetRequestParameter();
                    if (tdRequestParameter.Headers["X-Requested-With"] != null)
                        tdRequestParameter.Headers.Remove("X-Requested-With");
                    tdRequestParameter.Accept = "application/json;format=camelcase";
                    tdRequestParameter.Headers["Origin"] = ConstantHelpDetails.TumblrUrl;
                    tdRequestParameter.Headers["sec-ch-ua-mobile"] = "?0";
                    tdRequestParameter.Headers["X-Version"] = "redpop/3/0//redpop/";
                    tdRequestParameter.Headers["accept-language"] = "en-us";
                    tdRequestParameter.Headers["sec-ch-ua-platform"] = "\"Windows\"";
                    tdRequestParameter.Headers["Sec-Fetch-Dest"] = "empty";
                    tdRequestParameter.Referer = ConstantHelpDetails.LoginUrl;
                    tdRequestParameter.Headers["Sec-Fetch-Mode"] = "cors";
                    tdRequestParameter.Headers["Sec-Fetch-Site"] = "same-origin";
                    tdRequestParameter.Headers["X-Ad-Blocker-Enabled"] = "0";
                    tdRequestParameter.Headers.Remove("If-None-Match");
                    tdRequestParameter.Headers.Remove("Upgrade-Insecure-Requests");
                    tdRequestParameter.Headers["Authorization"] = ConstantHelpDetails.BearerToken;
                    if (!string.IsNullOrEmpty(Csrf_Token)) tdRequestParameter.Headers["X-CSRF"] = Csrf_Token;
                    var postData = Encoding.UTF8.GetBytes(TumblrUtility.GenerateJsonPayload(postelement));
                    HttpHelper.SetRequestParameter(tdRequestParameter);
                    var LoginResponse = new LogInResponseHandler(HttpHelper.PostRequest(ConstantHelpDetails.AuthUrl, postData));
                    if (LoginResponse.IsLoggedIn)
                        UpdateCsrf();
                    var userResponse = GetUserInfo(dominatorAccount);
                    dominatorAccount.CrmUuid = userResponse.TumblrUser.Uuid;
                    dominatorAccount.UserUuid = userResponse.TumblrUser.UserUuid;
                    Participant_key = userResponse.TumblrUser.Uuid;
                    responseparameterLogin = new LogInResponseHandler(userResponse.Response);
                }
                #endregion
                return responseparameterLogin;
            }
            catch (Exception)
            {
                return responseparameterLogin;
            }
        }


        public FollowResponseHandler Follow(DominatorAccountModel tumbleraccount, TumblrUser tumblrUser,
            string tumblrFormKey)
        {
            try
            {
                var tdRequestParameter = (TumblrRequestParameter)HttpHelper.GetRequestParameter();
                tdRequestParameter.Headers.Remove("If-None-Match");
                tdRequestParameter.Headers.Remove("Upgrade-Insecure-Requests");
                tdRequestParameter.Headers["Authorization"] = ConstantHelpDetails.BearerToken;
                if (!string.IsNullOrEmpty(Csrf_Token)) tdRequestParameter.Headers["X-CSRF"] = Csrf_Token;
                tdRequestParameter.Headers["Origin"] = ConstantHelpDetails.TumblrUrl;
                tdRequestParameter.Headers["Host"] = "www.tumblr.com";
                tdRequestParameter.Accept = "application/json;format=camelcase";
                tdRequestParameter.Headers.Remove("ContentType");
                tdRequestParameter.ContentType = "application/json; charset=utf8";
                tdRequestParameter.Headers["X-Version"] = "redpop/3/0//redpop/";
                tdRequestParameter.Headers["Sec-Fetch-Site"] = "same-origin";
                tdRequestParameter.Headers["Sec-Fetch-Mode"] = "cors";
                tdRequestParameter.Headers["X-Ad-Blocker-Enabled"] = "0";
                tdRequestParameter.Headers["Sec-Fetch-Dest"] = "empty";
                setCookieForRequest(tdRequestParameter, tumbleraccount);
                dynamic json = new JObject();
                if (tumblrUser.PageUrl.StartsWith(ConstantHelpDetails.TumblrUrl))
                    json.url = "https://" + Utilities.GetBetween(tumblrUser.PageUrl + "/", "https://www.tumblr.com/", "/") + ".tumblr.com/";
                else json.url = tumblrUser.PageUrl;
                if (!string.IsNullOrEmpty(tumblrUser.PlacementId))
                    json.placement_id = tumblrUser.PlacementId;
                var postData = json.ToString();
                postData = Regex.Replace(postData, "[\n\r ]+", "");
                byte[] postdata = Encoding.UTF8.GetBytes(postData);
                HttpHelper.SetRequestParameter(tdRequestParameter);
                return new FollowResponseHandler(HttpHelper.PostRequest("https://www.tumblr.com/api/v2/user/follow",
                    postdata));
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



        public UnFollowResponseHandler UnFollow(DominatorAccountModel tumbleraccount, TumblrUser tumblrUser,
            string tumblrFormKey)
        {
            try
            {
                var tdRequestParameter = (TumblrRequestParameter)HttpHelper.GetRequestParameter();
                tdRequestParameter.Headers.Remove("If-None-Match");
                tdRequestParameter.Headers.Remove("Upgrade-Insecure-Requests");
                tdRequestParameter.Headers["Authorization"] = ConstantHelpDetails.BearerToken;
                if (!string.IsNullOrEmpty(Csrf_Token)) tdRequestParameter.Headers["X-CSRF"] = Csrf_Token;
                tdRequestParameter.Cookies = tumbleraccount.Cookies;
                tdRequestParameter.Headers["Origin"] = ConstantHelpDetails.TumblrUrl;
                tdRequestParameter.Accept = "application/json;format=camelcase";
                tdRequestParameter.Headers.Remove("ContentType");
                tdRequestParameter.ContentType = "application/json; charset=utf8";
                tdRequestParameter.Headers["X-Version"] = "redpop/3/0//redpop/";
                tdRequestParameter.Headers["Sec-Fetch-Site"] = "same-origin";
                tdRequestParameter.Headers["Sec-Fetch-Mode"] = "cors";
                tdRequestParameter.Headers["X-Ad-Blocker-Enabled"] = "0";
                tdRequestParameter.Headers["Sec-Fetch-Dest"] = "empty";
                setCookieForRequest(tdRequestParameter, tumbleraccount);
                HttpHelper.SetRequestParameter(tdRequestParameter);
                dynamic json = new JObject();
                if (!string.IsNullOrEmpty(tumblrUser.PageUrl))
                {
                    if (tumblrUser.PageUrl.StartsWith(ConstantHelpDetails.TumblrUrl))
                        json.url = "https://" + Utilities.GetBetween(tumblrUser.PageUrl + "/", "https://www.tumblr.com/", "/") + ".tumblr.com/";
                    else json.url = tumblrUser.PageUrl;
                }
                else
                    json.url = ConstantHelpDetails.GetUSerUnfollowAPI(tumblrUser.Username);
                //json.placement_id = tumblrUser.PlacementId;
                var postData = json.ToString();
                postData = Regex.Replace(postData, "[\n\r ]+", "");
                byte[] postdata = Encoding.UTF8.GetBytes(postData);
                return new UnFollowResponseHandler(
                    HttpHelper.PostRequest("https://www.tumblr.com/api/v2/user/unfollow", postdata));
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






        public SearchPostsResonseHandler SearchPostsWithHashTag(DominatorAccountModel tumbleraccount, string url, string referer = "")
        {
            try
            {
                return ScrapePostsfromkeyword(tumbleraccount, url);
            }
            catch (Exception)
            {
                return null;
            }
        }


        public SearchPostsResonseHandler ScrapePostsfromkeyword(DominatorAccountModel tumbleraccount, string url, string referer = "")
        {
            try
            {
                var queryvalue = Utilities.GetBetween(url, "&query=", "&mode");
                var _referer = string.IsNullOrEmpty(referer) ?
                    $"https://www.tumblr.com/search/{queryvalue}?src=typed_query" : referer;
                var response = GetApiResponse(tumbleraccount, url, ConstantHelpDetails.BearerToken, _referer);
                UpdateCsrf();
                return new SearchPostsResonseHandler(response.Response);
            }
            catch (Exception e)
            {
                e.DebugLog();
                return new SearchPostsResonseHandler();
            }
        }


        public SearchforFollowingsorFollowersResponse SearchUsernameForFollowing(DominatorAccountModel tumbleraccount,
            string username, int pageId, string Pageurl = "")
        {
            try
            {
                var tdRequestParameter = (TumblrRequestParameter)HttpHelper.GetRequestParameter();
                tdRequestParameter.Accept =
                    "application/json;format=camelcase";

                username = username.Contains("@") ? username.Split('@')[0] : username;
                if (string.IsNullOrEmpty(Pageurl))
                    Pageurl = ConstantHelpDetails.GetSomeOnesFollowingsAPIByUserName(username);
                setCookieForRequest(tdRequestParameter, tumbleraccount);
                tdRequestParameter.Referer = $"https://www.tumblr.com/{username}/following";
                tdRequestParameter.Headers["Authorization"] = ConstantHelpDetails.BearerToken;
                tdRequestParameter.Headers.Remove("ContentType");
                tdRequestParameter.ContentType = "application/json; charset=utf8";
                tdRequestParameter.Headers["X-Version"] = "redpop/3/0//redpop/";
                tdRequestParameter.Accept = "application/json;format=camelcase";
                tdRequestParameter.Headers["sec-ch-ua"] = "\"Not/A)Brand\";v=\"99\", \"Google Chrome\";v=\"115\", \"Chromium\";v=\"115\"";
                tdRequestParameter.Headers["sec-ch-ua-mobile"] = "?0";
                tdRequestParameter.Headers["sec-ch-ua-platform"] = "\"Windows\"";
                tdRequestParameter.Headers["Sec-Fetch-Dest"] = "empty";
                tdRequestParameter.Headers["Sec-Fetch-Mode"] = "cors";
                tdRequestParameter.Headers["Host"] = "www.tumblr.com";
                tdRequestParameter.Headers["Sec-Fetch-Site"] = "same-origin";
                tdRequestParameter.Headers["X-Ad-Blocker-Enabled"] = "0";
                tdRequestParameter.Headers.Remove("Accept-Encoding");
                HttpHelper.SetRequestParameter(tdRequestParameter);
                Pageurl = WebUtility.UrlDecode(Pageurl);
                var followerResponse = HttpHelper.GetRequest(Pageurl);

                return new SearchforFollowingsorFollowersResponse(followerResponse);
            }
            catch (Exception)
            {
                return null;
            }
        }


        public SearchPostForUserRespones SearchPostFromUser(string userName, string xTumblrFormKey, int pageId = 0,
            CookieContainer cookies = null)
        {
            userName = userName.Contains("@") ? userName.Split('@')[0] : userName;

            var urlToGetPostOfUser =
                "https://www.tumblr.com/svc/indash_blog?tumblelog_name_or_id=" + userName +
                "&post_id=&limit=10&offset=" + pageId * 10 +
                "&should_bypass_safemode=false&should_bypass_tagfiltering=false";
            var tdRequestParameter = (TumblrRequestParameter)HttpHelper.GetRequestParameter();
            tdRequestParameter.Headers.Remove("X-tumblr-form-key");
            tdRequestParameter.KeepAlive = true;
            if (cookies != null)
                tdRequestParameter.Cookies = cookies.GetCookies(new Uri("https://www.tumblr.com"));
            if (tdRequestParameter.Headers["X-tumblr-form-key"] == null)
                tdRequestParameter.Headers.Add("X-tumblr-form-key", xTumblrFormKey);
            if (tdRequestParameter.Headers["X-Requested-With"] == null)
                tdRequestParameter.Headers.Add("X-Requested-With", "XMLHttpRequest");
            if (tdRequestParameter.Accept == null)
                tdRequestParameter.Accept = "application/json, text/javascript, */*; q=0.01";
            HttpHelper.SetRequestParameter(tdRequestParameter);
            return new SearchPostForUserRespones(HttpHelper.GetRequest(urlToGetPostOfUser));
        }

        public LikePostResponse LikePost(DominatorAccountModel tumbleraccount, TumblrPost tumblrPost,
            string tumblrFormkey)
        {
            var tdRequestParameter = (TumblrRequestParameter)HttpHelper.GetRequestParameter();
            var urlLike = ConstantHelpDetails.LikeAPIUrl;
            //Csrf_Token = HttpHelper.GetXCsrfTokenFromResp();
            UpdateCsrf();
            tdRequestParameter.Headers["Authorization"] = ConstantHelpDetails.BearerToken;
            if (!string.IsNullOrEmpty(Csrf_Token)) tdRequestParameter.Headers["X-CSRF"] = Csrf_Token;
            tdRequestParameter.Headers.Remove("ContentType");
            tdRequestParameter.ContentType = "application/json; charset=utf8";
            tdRequestParameter.Headers["X-Version"] = "redpop/3/0//redpop/";
            setCookieForRequest(tdRequestParameter, tumbleraccount);
            HttpHelper.SetRequestParameter(tdRequestParameter);
            dynamic json = new JObject();
            if (tdRequestParameter.Referer.Contains("src=typed_query"))
            {
                json.context = "Search";
                json.display_mode = "2";
            }
            else
            {
                json.context = "BlogView";
                json.display_mode = "0";
            }
            json.id = tumblrPost.Id;
            if (string.IsNullOrEmpty(tumblrPost.PlacementId))
                json.placement_id = tumblrPost.PlacementId;
            json.reblog_key = tumblrPost.RebloggedRootId;
            json.tumblelog_name = tumblrPost.OwnerUsername;
            if (tumblrPost.isFollowedPostOwner) json.tab = ConstantHelpDetails.following;
            var postData = json.ToString();
            postData = Regex.Replace(postData, "[\n\r ]+", "");
            byte[] postdata = Encoding.UTF8.GetBytes(postData);

            return new LikePostResponse(HttpHelper.PostRequest(urlLike, postdata));
        }

        public UnLikePostResponse UnLikePost(DominatorAccountModel tumbleraccount, TumblrPost tumblrPost,
            string tumblrFormkey)
        {
            var postDetailsUrl = ConstantHelpDetails.GetUserPostDetailsAPIByName(tumblrPost.OwnerUsername, tumblrPost.Id);
            var searchUserPostResponseHandler = new SearchPostsResonseHandler(GetApiResponse(tumbleraccount, postDetailsUrl, ConstantHelpDetails.BearerToken).Response);
            tumblrPost = searchUserPostResponseHandler.LstTumblrPosts.FirstOrDefault(x => x.Id == tumblrPost.Id) ?? tumblrPost;
            UpdateCsrf();
            var urlLike = "https://www.tumblr.com/api/v2/user/unlike";
            var tdRequestParameter = (TumblrRequestParameter)HttpHelper.GetRequestParameter();
            tdRequestParameter.Headers["Authorization"] = ConstantHelpDetails.BearerToken;
            if (!string.IsNullOrEmpty(Csrf_Token)) tdRequestParameter.Headers["X-CSRF"] = Csrf_Token;
            tdRequestParameter.Headers["Origin"] = "https://www.tumblr.com";
            tdRequestParameter.Headers.Remove("ContentType");
            tdRequestParameter.ContentType = "application/json; charset=utf8";
            tdRequestParameter.Headers["X-Version"] = "redpop/3/0//redpop/";
            setCookieForRequest(tdRequestParameter, tumbleraccount);
            HttpHelper.SetRequestParameter(tdRequestParameter);
            dynamic json = new JObject();
            if (!string.IsNullOrEmpty(tumblrPost.Id))
                tumblrPost.Id = Regex.Split(tumblrPost.PostUrl, "/").Skip(4).ToList()[0];
            json.context = ConstantHelpDetails.Dashboard;
            json.display_mode = "0";
            json.id = tumblrPost.Id;
            // json.placement_id = tumblrPost.PlacementId;
            json.reblog_key = tumblrPost.RebloggedRootId;
            if (tumblrPost.isFollowedPostOwner) json.tab = ConstantHelpDetails.following;
            json.tumblelog_name = tumblrPost.OwnerUsername;
            var postData = json.ToString();
            postData = Regex.Replace(postData, "[\n\r]+", "");
            byte[] postdata = Encoding.UTF8.GetBytes(postData);
            return new UnLikePostResponse(HttpHelper.PostRequest(urlLike, postdata));
        }

        public PostScraperResponse PostScraper(DominatorAccountModel tumbleraccount, TumblrPost tumblrPost,
            string tumblrFormkey)
        {
            string url;
            var userName = tumblrPost.OwnerUsername;
            if (userName == "" && !tumblrPost.ProfileUrl.Contains("https://www."))
                url = "https://www.tumblr.com/svc/tumblelog/" + userName + "/" + tumblrPost.Id +
                      "/notes?mode=conversation";
            else
                url = "https://www.tumblr.com/svc/tumblelog/" + userName + "/" + tumblrPost.Id +
                      "/notes?mode=conversation";
            return new PostScraperResponse(HttpHelper.GetRequest(url), tumblrPost);
        }

        public CommentScraperResponse CommentScraper(DominatorAccountModel tumbleraccount, TumblrPost tumblrPost,
            string tumblrFormkey)
        {
            var Url = "https://www.tumblr.com/svc/tumblelog/" + tumblrPost.OwnerUsername + "/" + tumblrPost.Id +
                      "/notes?mode=conversation";
            return new CommentScraperResponse(HttpHelper.GetRequest(Url));
        }


        public UserScraperResponse UserScraper(DominatorAccountModel tumbleraccount, TumblrUser tumblrUser,
            string tumblrFormkey)
        {
            var Url = "https://www.tumblr.com/svc/tumblelog/" + tumblrUser.FullName + "/" + tumblrUser.UserId +
                      "/notes?mode=conversation";
            return new UserScraperResponse(HttpHelper.GetRequest(Url));
        }


        public CommentPostResponse CommentPost(DominatorAccountModel tumbleraccount, TumblrPost tumblrPost,
            string tumblrFormkey, string message)
        {
            var tdRequestParameter = (TumblrRequestParameter)HttpHelper.GetRequestParameter();
            var urlComment =
                "https://www.tumblr.com/api/v2/user/post/reply";
            tdRequestParameter.Headers.Add("Authorization", ConstantHelpDetails.BearerToken);
            tdRequestParameter.Headers.Remove("If-None-Match");
            if (!string.IsNullOrEmpty(Csrf_Token)) tdRequestParameter.Headers["X-CSRF"] = Csrf_Token;
            tdRequestParameter.Headers["Origin"] = ConstantHelpDetails.LoginUrl;
            tdRequestParameter.Referer = tumblrPost.PostUrl;
            tdRequestParameter.Headers["sec-ch-ua"] = "\"Not/A)Brand\";v=\"99\", \"Google Chrome\";v=\"115\", \"Chromium\";v=\"115\"";
            tdRequestParameter.Headers["sec-ch-ua-mobile"] = "?0";
            tdRequestParameter.Headers["X-Version"] = "redpop/3/0//redpop/";
            tdRequestParameter.Headers["accept-language"] = "en-us";
            tdRequestParameter.Headers["sec-ch-ua-platform"] = "\"Windows\"";
            tdRequestParameter.Headers["Sec-Fetch-Dest"] = "empty";
            tdRequestParameter.Headers["Sec-Fetch-Mode"] = "cors";
            tdRequestParameter.Headers["Sec-Fetch-Site"] = "same-origin";
            tdRequestParameter.Headers["X-Ad-Blocker-Enabled"] = "0";
            tdRequestParameter.ContentType = "application/json";
            //      tdRequestParameter.Headers["Accept-Encoding"] = "gzip, deflate, br, zstd";
            setCookieForRequest(tdRequestParameter, tumbleraccount);
            HttpHelper.SetRequestParameter(tdRequestParameter);
            var CommentPostData = new CommentPostData()
            {
                PostId = tumblrPost.Id,
                ReplyText = message,
                ReblogKey = tumblrPost.RebloggedRootId,
                TumbleLog = tumblrPost.BlogName,
                PlacementId = tumblrPost.PlacementId,
                ReplyAs = tumbleraccount.AccountBaseModel.UserId,
                Scope = ""
            };
            var jsonCommentPostData = JsonConvert.SerializeObject(CommentPostData);
            return new CommentPostResponse(HttpHelper.PostRequest(urlComment, Encoding.UTF8.GetBytes(jsonCommentPostData)));
        }


        public ReblogPostResponse ReblogPost(DominatorAccountModel tumbleraccount, TumblrPost tumblrPost,
            string tumblrFormkey)
        {
            var urlReblog = ConstantHelpDetails.GetPublishPostAPI(tumbleraccount.CrmUuid);

            var tdRequestParameter = HttpHelper.GetRequestParameter();
            var blocks = new List<int>();
            blocks.Add(0);
            Display display = new Display()
            {
                Blocks = blocks
            };
            var displays = new List<Display>();

            displays.Add(display);

            Layout layout = new Layout()
            {
                Type = "rows",
                Display = displays
            };
            var layouts = new List<Layout>();
            layouts.Add(layout);
            Content content = new Content()
            {
                Type = "text",
                Text = ""
            };
            var contents = new List<Content>();
            contents.Add(content);
            var reblogPostData = new ReblogPostData
            {
                PlacementId = tumblrPost.PlacementId,
                Layout = layouts,
                ReblogKey = tumblrPost.PostKey,
                HideTrail = false,
                ParentTumblelogUuid = tumblrPost.Uuid,
                ParentPostId = tumblrPost.Id,
                Content = contents,
                CanBeTipped = null,
                InteractabilityReblog = "everyone",
                Tags = "",
                HasCommunityLabel = false,
                CommunityLabelCategories = new List<object>()
            };
            var jsonReblogPostData = JsonConvert.SerializeObject(reblogPostData);

            #region Headers
            tdRequestParameter.Accept = "application/json;format=camelcase";
            tdRequestParameter.Headers["Authorization"] = ConstantHelpDetails.BearerToken;
            if (!string.IsNullOrEmpty(Csrf_Token)) tdRequestParameter.Headers["X-CSRF"] = Csrf_Token;
            tdRequestParameter.Headers["sec-ch-ua-mobile"] = "?0";
            tdRequestParameter.Headers["X-Version"] = "redpop/3/0//redpop/";
            tdRequestParameter.Headers["accept-language"] = "en-us";
            tdRequestParameter.Headers["sec-ch-ua-platform"] = "\"Windows\"";
            tdRequestParameter.Headers["Sec-Fetch-Dest"] = "empty";
            tdRequestParameter.Headers["Sec-Fetch-Mode"] = "cors";
            tdRequestParameter.Headers["Sec-Fetch-Site"] = "same-origin";
            tdRequestParameter.Headers["X-Ad-Blocker-Enabled"] = "0";
            tdRequestParameter.Headers["Accept-Encoding"] = "gzip, deflate";
            tdRequestParameter.Headers["Connection"] = "keep-alive";
            tdRequestParameter.ContentType = "application/json";
            setCookieForRequest(tdRequestParameter, tumbleraccount);
            #endregion

            HttpHelper.SetRequestParameter(tdRequestParameter);

            return new ReblogPostResponse(HttpHelper.PostRequest(urlReblog,
                Encoding.UTF8.GetBytes(jsonReblogPostData)));
        }


        public PublishResponseHandler JobforPublishPost(string tumblrFormkey,
            DominatorAccountModel dominatorAccountModel, PublisherPostlistModel postDetails)
        {
            var uploadFile = new List<UploadedMediaModel>();

            var blogname = string.Empty;
            if (postDetails.CurrentMediaUrl.Contains("www.tumblr.com"))
                blogname = postDetails.CurrentMediaUrl.Replace("www.tumblr.com/", "");
            else
                blogname = postDetails.CurrentMediaUrl.Replace(".tumblr.com", "");
            foreach (var media in postDetails.MediaList)
            {
                if (!File.Exists(media) && !ImageExtracter.IsValidUrl(media)) continue;
                try
                {
                    if (uploadFile.Count == 30) break;
                    var uploadedMedia = UploadMedia(dominatorAccountModel, media, tumblrFormkey);
                    uploadFile.Add(uploadedMedia);
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }

            var obj = PublishPost(dominatorAccountModel, postDetails, tumblrFormkey, uploadFile, blogname);
            if (obj.Success)
                postDetails.FetchedPostIdOrUrl = obj.PublishedUrl;
            return obj;
        }

        public PublishResponseHandler PublishPost(DominatorAccountModel tumbleraccount,
            PublisherPostlistModel postDetails, string tumblrFormkey, List<UploadedMediaModel> uploadMedia, string blogName)
        {
            try
            {
                var PublishPost_url = ConstantHelpDetails.GetPublishPostAPI(tumbleraccount.CrmUuid);
                var tdRequestParameter = (TumblrRequestParameter)HttpHelper.GetRequestParameter();
                int row = 0;
                var contents = new ArrayList();
                var displays = new List<Display>();
                var title = postDetails?.PublisherInstagramTitle;
                contents.Add(new TitleContent()
                {
                    Type = "text",
                    SubType = "heading1",
                    Text = string.IsNullOrEmpty(title) ? string.Empty : title
                });
                displays.Add(new Display()
                {
                    Blocks = new List<int> { row++ }
                });
                //Description.
                var descrription = postDetails?.PostDescription;
                if (!string.IsNullOrEmpty(descrription))
                {
                    if (descrription.Contains("https://"))
                    {
                        var url = descrription.Replace("\n", " ").Split(' ').FirstOrDefault(x => x.StartsWith("https://")) ?? "";
                        var formattingArray = new ArrayList();
                        formattingArray.Add(new FormattingType()
                        {
                            Type = "link",
                            start = descrription.IndexOf(url),
                            end = descrription.IndexOf(url) + url.Length,
                            url = url
                        });
                        contents.Add(new LinkDescriptionContent()
                        {
                            Type = "text",
                            Text = descrription,
                            formatting = formattingArray
                        });
                    }
                    else
                    {
                        contents.Add(new DescriptionContent()
                        {
                            Type = "text",
                            Text = descrription
                        });
                    }
                    displays.Add(new Display()
                    {
                        Blocks = new List<int> { row++ }
                    });
                }

                if (postDetails.MediaList.Count > 0)
                {
                    foreach (var x in uploadMedia)
                    {
                        ArrayList medias = new ArrayList();
                        if (x.type == "image/jpeg" || x.type == "image/png")
                        {
                            medias.Add(new Media()
                            {
                                Type = x.type,
                                Width = x.width,
                                Height = x.height,
                                Url = x.url

                            });
                            contents.Add(new ImageContent()
                            {
                                Type = "image",
                                File = new file(),
                                Media = medias
                            });
                            displays.Add(new Display()
                            {
                                Blocks = new List<int> { row++ }
                            });
                        }
                        if (x.type == "video/x-m4v" || x.type == "video/mp4")
                        {
                            contents.Add(new VideoContent()
                            {
                                Type = "video",
                                Media = new Media()
                                {
                                    Type = "video/mp4",
                                    Width = x.width,
                                    Height = x.height,
                                    Url = x.url

                                },
                                Provider = "tumblr",
                                Url = x.url
                            });
                            displays.Add(new Display()
                            {
                                Blocks = new List<int> { row++ }
                            });
                        }
                        if (x.type == "audio/mpeg")
                        {
                            medias.Add(new AudioMedia()
                            {
                                Type = x.type,
                                Url = x.url
                            });
                            contents.Add(new AudioContent()
                            {
                                Type = "audio",
                                Provider = "tumblr",
                                Url = x.url,
                                Media = medias
                            });
                            displays.Add(new Display()
                            {
                                Blocks = new List<int> { row++ }
                            });
                        }
                    }
                }
                var publishPostData = new PublishPostData
                {

                    Layout = new List<Layout> { new Layout { Type = "rows", Display = displays } },
                    HideTrail = false,
                    Content = contents,
                    CanBeTipped = null,
                    Tags = postDetails.TumblrPostSettings.TagUserList,
                    HasCommunityLabel = false,
                    CommunityLabelCategories = new List<object>()
                };

                var jsonReblogPostData = JsonConvert.SerializeObject(publishPostData);

                #region Headers
                tdRequestParameter.Headers["Host"] = "www.tumblr.com";
                tdRequestParameter.Headers["sec-ch-ua-mobile"] = "?0";
                tdRequestParameter.Headers["X-Version"] = "redpop/3/0//redpop/";
                tdRequestParameter.Headers["X-Ad-Blocker-Enabled"] = "0";
                tdRequestParameter.Headers["accept-language"] = "en-us";
                tdRequestParameter.Headers["sec-ch-ua-platform"] = "\"Windows\"";
                tdRequestParameter.Headers["Origin"] = "https://www.tumblr.com";
                tdRequestParameter.Headers["Sec-Fetch-Dest"] = "empty";
                tdRequestParameter.Headers["Sec-Fetch-Mode"] = "cors";
                tdRequestParameter.Headers["Sec-Fetch-Site"] = "same-origin";
                //tdRequestParameter.Headers["Accept-Encoding"] = "gzip, deflate, br";
                tdRequestParameter.Referer = "https://www.tumblr.com/new/text";
                tdRequestParameter.Headers["Connection"] = "keep-alive";
                tdRequestParameter.Headers["Authorization"] = ConstantHelpDetails.BearerToken;
                tdRequestParameter.Headers["Accept"] = "application/json;format=camelcase";
                if (!string.IsNullOrEmpty(Csrf_Token))
                {
                    tdRequestParameter.Headers["X-CSRF"] = Csrf_Token;
                    tdRequestParameter.CsrfToken = Csrf_Token;
                }
                tdRequestParameter.ContentType = "application/json; charset=utf8";
                tdRequestParameter.KeepAlive = true;
                setCookieForRequest(tdRequestParameter, tumbleraccount);
                HttpHelper.SetRequestParameter(tdRequestParameter);
                #endregion
                var UserId = string.IsNullOrEmpty(tumbleraccount.AccountBaseModel.UserId) ? blogName : tumbleraccount.AccountBaseModel.UserId;
                return new PublishResponseHandler(HttpHelper.PostRequest(PublishPost_url,
                    Encoding.UTF8.GetBytes(jsonReblogPostData)), UserId);


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


        public MessageUserResponse Messageuser(DominatorAccountModel tumbleraccount, TumblrUser user, string key)
        {

            var tdRequestParameter = (TumblrRequestParameter)HttpHelper.GetRequestParameter();
            var Messageurl = ConstantHelpDetails.GetUserMessageAPI();
            dynamic json = new JObject();
            json.participants = new JArray(Participant_key, user.Uuid);
            json.type = "TEXT";
            json.participant = Participant_key;
            json.message = user.Message;
            var postElement = json.ToString().Replace("\r", "").Replace("\t", "").Replace("\n", "");
            #region Headers

            tdRequestParameter.Headers["X-Version"] = "redpop/3/0//redpop/";
            tdRequestParameter.Headers["X-CSRF"] = Csrf_Token;
            tdRequestParameter.Accept = "application/json;format=camelcase";
            tdRequestParameter.Headers["Authorization"] = ConstantHelpDetails.BearerToken;
            tdRequestParameter.Headers["Accept-Language"] = "en-us";
            tdRequestParameter.Headers["Origin"] = "https://www.tumblr.com";
            tdRequestParameter.ContentType = "application/json; charset=utf8";
            tdRequestParameter.Headers.Remove("If-None-Match");
            tdRequestParameter.Headers.Remove("Upgrade-Insecure-Requests");
            setCookieForRequest(tdRequestParameter, tumbleraccount);
            #endregion

            HttpHelper.SetRequestParameter(tdRequestParameter);
            byte[] postData = Encoding.UTF8.GetBytes(postElement);
            return new MessageUserResponse(HttpHelper.PostRequest(Messageurl, postData));
        }

        /// <summary>
        ///     Get Tumblr user information
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public UserInfoResponeHandler GetUserInfo(DominatorAccountModel tumbleraccount)
        {
            try
            {
                var tdRequestParameter = (TumblrRequestParameter)HttpHelper.GetRequestParameter();
                tdRequestParameter.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7";
                tdRequestParameter.ContentType = "text/html; charset=utf-8";
                tdRequestParameter.Headers["Accept-Language"] = "en-US,en;q=0.9";
                tdRequestParameter.Headers["Connection"] = "keep-alive";
                tdRequestParameter.Headers["sec-ch-ua-platform"] = "\"Windows\"";
                tdRequestParameter.Headers["sec-ch-ua-mobile"] = "?0";
                tdRequestParameter.Headers["Sec-Fetch-Dest"] = "document";
                tdRequestParameter.Headers["Sec-Fetch-Mode"] = "navigate";
                tdRequestParameter.Headers["Sec-Fetch-Site"] = "none";
                tdRequestParameter.Headers["Sec-Fetch-User"] = "?1";
                tdRequestParameter.Headers["Upgrade-Insecure-Requests"] = "1";
                tdRequestParameter.Headers.Remove("Authorization");
                tdRequestParameter.Headers.Remove("X-Ad-Blocker-Enabled");
                tdRequestParameter.Headers.Remove("X-Version");
                tdRequestParameter.Headers.Remove("X-CSRF");
                tdRequestParameter.Headers.Remove("Origin");
                tdRequestParameter.KeepAlive = true;
                setCookieForRequest(tdRequestParameter, tumbleraccount);
                HttpHelper.SetRequestParameter(tdRequestParameter);
                var userInfoResponse = new UserInfoResponeHandler(HttpHelper.GetRequest(ConstantHelpDetails.HomePageUrl));
                if (userInfoResponse.Success)
                {
                    try
                    {
                        var requireData = Utilities.GetBetween(WebUtility.HtmlDecode(userInfoResponse.Response.Response), "window['___INITIAL_STATE___'] =", "</script>")?.Trim()?.TrimEnd(';');
                        requireData = string.IsNullOrEmpty(requireData) ? Utilities.GetBetween(WebUtility.HtmlDecode(userInfoResponse.Response.Response), "id=\"___INITIAL_STATE___\">", "</script>")?.Trim()?.TrimEnd(';') : requireData;
                        JsonHandler hand1 = new JsonHandler(requireData);
                        try
                        {
                            var eTag = HttpHelper.Response.Headers["ETag"];
                            if (!string.IsNullOrEmpty(eTag))
                            {
                                ETag = eTag;
                                tdRequestParameter.Cookies.Add(new Cookie("Etag", ETag));
                                tumbleraccount.Cookies.Add(new Cookie("Etag", ETag));
                            }
                        }
                        catch
                        { }
                        Csrf_Token = hand1.GetElementValue("csrfToken");
                        client = new ClientDetails
                        {
                            Platform = hand1.GetElementValue("analyticsInfo", "kraken", "clientDetails", "platform"),
                            OsName = hand1.GetElementValue("analyticsInfo", "kraken", "clientDetails", "os_name"),
                            OsVersion = hand1.GetElementValue("analyticsInfo", "kraken", "clientDetails", "os_version"),
                            Language = hand1.GetElementValue("analyticsInfo", "kraken", "clientDetails", "language"),
                            BuildVersion = hand1.GetElementValue("analyticsInfo", "kraken", "clientDetails", "build_version"),
                            FormFactor = hand1.GetElementValue("analyticsInfo", "kraken", "clientDetails", "form_factor"),
                            Model = hand1.GetElementValue("analyticsInfo", "kraken", "clientDetails", "model"),
                            Connection = hand1.GetElementValue("analyticsInfo", "kraken", "clientDetails", "connection"),
                            Carrier = hand1.GetElementValue("analyticsInfo", "kraken", "clientDetails", "carrier"),
                            BrowserName = hand1.GetElementValue("analyticsInfo", "kraken", "clientDetails", "browser_name"),
                            BrowserVersion = hand1.GetElementValue("analyticsInfo", "kraken", "clientDetails", "browser_version"),
                            Manufacturer = hand1.GetElementValue("analyticsInfo", "kraken", "clientDetails", "manufacturer"),
                        };
                        tumbleraccount.SessionId = hand1.GetElementValue("analyticsInfo", "kraken", "sessionId");
                        var servicePostData = new ServicePostData
                        {
                            FlushTime = DateTimeUtilities.GetCurrentEpochTimeMilliSeconds(DateTime.Now),
                            KrakenCLientdetails = client,
                            KrackEvents = new KrackEvents[]
                            {
                                new KrackEvents{
                                    EventName="SessionStart",
                                    Experiments=new object(),
                                    EventDetails= new EventDetails{
                                    Action="start",
                                    Pathname="/dashboard",
                                    Hostname=ConstantHelpDetails.TumblrUrl
                                },
                            TimeStamp=DateTimeUtilities.GetCurrentEpochTimeMilliSeconds(DateTime.Now),
                            SessionId=tumbleraccount.SessionId,
                            Page=hand1.GetElementValue("analyticsInfo", "kraken", "basePage")
                       } }
                            ,
                            Trackers = new object[] { }
                        };
                        var response = TumblrServiceApi(tumbleraccount, servicePostData);
                    }
                    catch (Exception)
                    { }
                }
                return userInfoResponse;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public SearchforFollowingsorFollowersResponse GetAcccountFollowings(DominatorAccountModel tumbleraccount,
            int pageCount = 0, string PageUrl = "")
        {
            try
            {
                if (string.IsNullOrEmpty(PageUrl))
                    PageUrl = ConstantHelpDetails.GetUsersOwnFollowingsAPI;
                var tdRequestParameter = (TumblrRequestParameter)HttpHelper.GetRequestParameter();
                tdRequestParameter.Headers.Remove("If-None-Match");
                tdRequestParameter.Headers.Remove("Upgrade-Insecure-Requests");
                tdRequestParameter.ContentType = "application/json; charset=utf8";
                setCookieForRequest(tdRequestParameter, tumbleraccount);
                tdRequestParameter.Headers["Authorization"] = ConstantHelpDetails.BearerToken;
                tdRequestParameter.Headers["X-Ad-Blocker-Enabled"] = "0";
                tdRequestParameter.Accept = "application/json;format=camelcase";
                tdRequestParameter.Referer = "https://www.tumblr.com/following";
                HttpHelper.SetRequestParameter(tdRequestParameter);
                return new SearchforFollowingsorFollowersResponse(HttpHelper.GetRequest(PageUrl));
            }
            catch (Exception)
            {
                return null;
            }
        }


        public SearchforFollowingsorFollowersResponse GetAcccountFollowers(DominatorAccountModel tumbleraccount,
            string nextPageUrl = "")
        {
            try
            {
                var getFollowingUrl = "";
                if (string.IsNullOrEmpty(nextPageUrl))
                    getFollowingUrl = "https://www.tumblr.com/api/v2/blog/" + tumbleraccount.AccountBaseModel.UserId +
                                         "/followers";
                else
                    getFollowingUrl = nextPageUrl;
                var tdRequestParameter = (TumblrRequestParameter)HttpHelper.GetRequestParameter();
                tdRequestParameter.Headers.Remove("If-None-Match");
                tdRequestParameter.Headers.Remove("Upgrade-Insecure-Requests");
                tdRequestParameter.ContentType = "application/json; charset=utf8";
                setCookieForRequest(tdRequestParameter, tumbleraccount);
                tdRequestParameter.Headers["Authorization"] = ConstantHelpDetails.BearerToken;
                tdRequestParameter.Headers["X-Ad-Blocker-Enabled"] = "0";
                tdRequestParameter.Accept = "application/json;format=camelcase";
                tdRequestParameter.Referer = $"https://www.tumblr.com/blog/{tumbleraccount.AccountBaseModel.UserId}/followers";
                HttpHelper.SetRequestParameter(tdRequestParameter);
                return new SearchforFollowingsorFollowersResponse(HttpHelper.GetRequest(getFollowingUrl));
            }
            catch (Exception)
            {
                return null;
            }
        }


        public FeedInteractionResponseHandler GetFeedInteractions(DominatorAccountModel tumbleraccount, string feedId,
            string username, QueryInfo queryInfo, string beforeStamp = "")
        {
            var responseUrl = "";
            try
            {

                if (queryInfo.QueryType == EnumUtility.GetQueryFromEnum(TumblrQuery.UserCommentedOnPost))
                {
                    var request = HttpHelper.GetRequestParameter();

                    //     var getFollowingUrl = "https://www.tumblr.com/svc/tumblelog/" + username + "/" + feedId +
                    //                          "/notes?mode=conversation";
                    responseUrl = "https://www.tumblr.com/svc/tumblelog/" + username + "/" + feedId +
                                         "/notes?mode=replies";
                    if (!string.IsNullOrEmpty(beforeStamp))
                    {
                        responseUrl = feedId == string.Empty
                            ? responseUrl
                            : responseUrl + "&before_timestamp=" + beforeStamp;
                    }

                }

                else if (queryInfo.QueryType == EnumUtility.GetQueryFromEnum(TumblrQuery.UserLikedThePost))
                {
                    responseUrl = "https://www.tumblr.com/svc/tumblelog/" + username + "/" + feedId +
                                         "/notes?mode=likes&sort=desc";
                    if (!string.IsNullOrEmpty(beforeStamp))
                    {
                        responseUrl = feedId == string.Empty
                            ? responseUrl
                            : responseUrl + "&before_timestamp=" + beforeStamp;
                    }
                }
                else if (queryInfo.QueryType == EnumUtility.GetQueryFromEnum(TumblrQuery.UserReblogedThePost))
                {
                    responseUrl = "https://www.tumblr.com/svc/tumblelog/" + username + "/" + feedId +
                                         "/notes?mode=reblogs_only&sort=desc";
                    if (!string.IsNullOrEmpty(beforeStamp))
                    {
                        responseUrl = feedId == string.Empty
                            ? responseUrl
                            : responseUrl + "&before_timestamp=" + beforeStamp;
                    }

                }
                else
                {
                    responseUrl = "https://www.tumblr.com/svc/tumblelog/" + username + "/" + feedId +
                                      "/notes?mode=conversation";
                }
                return new FeedInteractionResponseHandler(HttpHelper.GetRequest(responseUrl));

            }
            catch (Exception)
            {
                return null;
            }
        }


        public UploadedMediaModel UploadMedia(DominatorAccountModel account, string media, string waterfallId)
        {
            var uploadUrl = "";
            byte[] postData = null;
            try
            {
                var postDetails = new PublisherPostlistModel();
                postDetails.MediaList.Add(media);
                var objParameter = new TumblrRequestParameter();

                foreach (var mediapath in postDetails.MediaList)
                {
                    objParameter.IsMultiPart = true;
                    string fileExtension = Path.GetExtension(mediapath);
                    if (TumblrUtility.IsVedio(fileExtension))
                    {
                        byte[] byteArray = null;
                        var filenameArray = Regex.Split(mediapath, @"\\");
                        var fileName = filenameArray[filenameArray.Length - 1];
                        if (mediapath.Contains("https://") || mediapath.Contains("http://"))
                        {
                            byteArray = DownloadMediaFromUrl(mediapath);
                            objParameter.TumblrAddFileList("video", byteArray, fileName);
                        }
                        fileExtension = fileExtension.ToLower();
                        if (fileExtension != null)
                        {
                            if (byteArray == null)
                            {
                                byteArray = TumblrUtility.ConvertMediaFileToByte(mediapath);
                                objParameter.TumblrAddFileList("video", byteArray, fileName);
                            }
                        }
                        postData = objParameter.GetPostDataFromParameters();
                        var urlphoto = "https://www.tumblr.com/api/v2/media/video";
                        uploadUrl = objParameter.GenerateUrl(urlphoto);
                    }
                    else if (TumblrUtility.isImage(fileExtension))
                    {
                        byte[] byteArray = null;
                        var filenameArray = Regex.Split(mediapath, @"\\");
                        var fileName = filenameArray[filenameArray.Length - 1];
                        if (mediapath.Contains("https://") || mediapath.Contains("http://"))
                        {
                            byteArray = DownloadMediaFromUrl(mediapath);
                            objParameter.TumblrAddFileList("photo", byteArray, fileName);
                        }

                        fileExtension = fileExtension.ToLower();
                        if (fileExtension != null && fileExtension.ToLower().Contains("gif"))
                        {
                            byteArray = ImageToByteArray(mediapath, ImageFormat.Gif);
                            objParameter.TumblrAddFileList("photo", byteArray, fileName);
                        }
                        else if (fileExtension != null &&
                                 (fileExtension.ToLower().Contains("jpeg") || fileExtension.ToLower().Contains("jpg") ||
                                  fileExtension.ToLower().Contains(".jpg")))
                        {
                            if (byteArray == null)
                            {
                                byteArray = ImageToByteArray(mediapath, ImageFormat.Jpeg);
                                objParameter.TumblrAddFileList("photo", byteArray, fileName);
                            }
                        }
                        else if (fileExtension != null && fileExtension.ToLower().Contains(".png"))
                        {
                            if (byteArray == null)
                            {
                                byteArray = ImageToByteArray(mediapath, ImageFormat.Png);
                                objParameter.TumblrAddFileList("photo", byteArray, fileName);
                            }
                        }
                        else
                        {
                            if (byteArray == null && fileExtension != null)
                            {
                                byteArray = ImageToByteArray(mediapath, ImageFormat.Jpeg);
                                objParameter.TumblrAddFileList("photo", byteArray, fileName);
                            }
                        }

                        postData = objParameter.GetPostDataFromParameters();
                        var urlphoto = "https://www.tumblr.com/api/v2/media/image";
                        uploadUrl = objParameter.GenerateUrl(urlphoto);

                    }
                    else if (TumblrUtility.isAudio(fileExtension))
                    {
                        byte[] byteArray = null;
                        var filenameArray = Regex.Split(mediapath, @"\\");
                        var fileName = filenameArray[filenameArray.Length - 1];
                        if (mediapath.Contains("https://") || mediapath.Contains("http://"))
                        {
                            byteArray = DownloadMediaFromUrl(mediapath);
                            objParameter.TumblrAddFileList("audio", byteArray, fileName);
                        }
                        fileExtension = fileExtension.ToLower();
                        if (fileExtension != null)
                        {
                            if (byteArray == null)
                            {
                                byteArray = TumblrUtility.ConvertMediaFileToByte(mediapath);
                                objParameter.TumblrAddFileList("audio", byteArray, fileName);
                            }
                        }
                        postData = objParameter.GetPostDataFromParameters();
                        var urlphoto = "https://www.tumblr.com/api/v2/media/audio";
                        uploadUrl = objParameter.GenerateUrl(urlphoto);
                    }
                    var tdRequestParameter = (TumblrRequestParameter)HttpHelper.GetRequestParameter();
                    tdRequestParameter.Accept = "application/json;format=camelcase";
                    tdRequestParameter.Referer = "https://www.tumblr.com/new/text";
                    tdRequestParameter.Headers["Authorization"] = ConstantHelpDetails.BearerToken;
                    if (!string.IsNullOrEmpty(Csrf_Token)) tdRequestParameter.Headers["X-CSRF"] = Csrf_Token;
                    tdRequestParameter.Headers["sec-ch-ua-mobile"] = "?0";
                    tdRequestParameter.Headers["X-Version"] = "redpop/3/0//redpop/";
                    tdRequestParameter.Headers["accept-language"] = "en-us";
                    tdRequestParameter.Headers["sec-ch-ua-platform"] = "\"Windows\"";
                    tdRequestParameter.Headers["Sec-Fetch-Dest"] = "empty";
                    tdRequestParameter.Headers["Sec-Fetch-Mode"] = "cors";
                    tdRequestParameter.Headers["Sec-Fetch-Site"] = "same-origin";
                    tdRequestParameter.Headers["X-Ad-Blocker-Enabled"] = "0";
                    //tdRequestParameter.Headers["Accept-Encoding"] = "gzip, deflate, br";
                    tdRequestParameter.Headers["Connection"] = "keep-alive";
                    tdRequestParameter.KeepAlive = true;
                    tdRequestParameter.Headers.Remove("ContentType");
                    tdRequestParameter.ContentType = objParameter.ContentType;
                    setCookieForRequest(tdRequestParameter, account);
                    HttpHelper.SetRequestParameter(tdRequestParameter);
                    var imageUploadResponse = new MediaUploadResponseHandler(HttpHelper.PostRequest(uploadUrl, postData));
                    UpdateCsrf();
                    return imageUploadResponse.MediaUploaded != null ? imageUploadResponse.MediaUploaded : new UploadedMediaModel();
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return null;
        }

        public string UploadImageinMessage(DominatorAccountModel account, string media, string waterfallId,
            TumblrUser objUser)
        {
            var mediaId = string.Empty;
            byte[] postData = null;
            try
            {
                var postDetails = new PublisherPostlistModel();
                postDetails.MediaList.Add(media);
                var objParameter = new TumblrRequestParameter();
                var urlphoto = "";
                foreach (var mediapath in postDetails.MediaList)
                {
                    objParameter.IsMultiPart = true;

                    var fileExtension = Path.GetExtension(mediapath);
                    var headers = new NameValueCollection();
                    if (TumblrUtility.isImage(fileExtension))
                    {
                        byte[] byteArray = null;
                        var filenameArray = Regex.Split(mediapath, @"\\");
                        var fileName = filenameArray[filenameArray.Length - 1];
                        headers.Add("type", "IMAGE");
                        headers.Add("participant", $"{account.CrmUuid}");
                        headers.Add("participants", $"{account.CrmUuid},{objUser.Uuid}");
                        if (mediapath.Contains("https://") || mediapath.Contains("http://"))
                        {
                            byteArray = DownloadMediaFromUrl(mediapath);
                            objParameter.TumblrAddFileList("photo", byteArray, fileName);
                        }

                        fileExtension = fileExtension.ToLower();
                        if (fileExtension != null && fileExtension.ToLower().Contains("gif"))
                        {
                            byteArray = ImageToByteArray(mediapath, ImageFormat.Gif);
                            objParameter.TumblrAddFileList("photo", byteArray, fileName);
                        }
                        else if (fileExtension != null &&
                                 (fileExtension.ToLower().Contains("jpeg") || fileExtension.ToLower().Contains("jpg") ||
                                  fileExtension.ToLower().Contains(".jpg")))
                        {
                            if (byteArray == null)
                            {
                                byteArray = ImageToByteArray(mediapath, ImageFormat.Jpeg);
                                objParameter.TumblrAddFileList("photo", byteArray, fileName);
                            }
                        }
                        else if (fileExtension != null && fileExtension.ToLower().Contains(".png"))
                        {
                            if (byteArray == null)
                            {
                                byteArray = ImageToByteArray(mediapath, ImageFormat.Png);
                                objParameter.TumblrAddFileList("photo", byteArray, fileName);
                            }
                        }
                        else
                        {
                            if (byteArray == null && fileExtension != null)
                            {
                                byteArray = ImageToByteArray(mediapath, ImageFormat.Jpeg);
                                objParameter.TumblrAddFileList("photo", byteArray, fileName);
                            }
                        }

                        postData = objParameter.CreateMultipartBodyForBroadCastMessage(headers.ToString());
                        var messageUrl = ConstantHelpDetails.GetUserMessageAPI();
                        urlphoto = objParameter.GenerateUrl(messageUrl);
                    }
                    var request = HttpHelper.GetRequestParameter();
                    request.Accept = "application / json; format = camelcase";
                    request.Headers["Host"] = "www.tumblr.com";
                    request.Headers["Origin"] = "https://www.tumblr.com";
                    request.Headers["Authorization"] = ConstantHelpDetails.BearerToken;
                    if (!string.IsNullOrEmpty(Csrf_Token)) request.Headers["X-CSRF"] = Csrf_Token;
                    request.Headers["sec-ch-ua-mobile"] = "?0";
                    request.Headers["X-Version"] = "redpop/3/0//redpop/";
                    request.Headers["accept-language"] = "en-us";
                    request.Headers["sec-ch-ua-platform"] = "\"Windows\"";
                    request.Headers["Sec-Fetch-Dest"] = "empty";
                    request.Headers["Sec-Fetch-Mode"] = "cors";
                    request.Headers["Sec-Fetch-Site"] = "same-origin";
                    request.KeepAlive = true;
                    request.Headers.Remove("ContentType");
                    request.ContentType = objParameter.ContentType;
                    setCookieForRequest(request, account);
                    HttpHelper.SetRequestParameter(request);
                    var imageUploadResponse = HttpHelper.PostRequest(urlphoto, postData).Response;
                    UpdateCsrf();
                    if (imageUploadResponse.Contains("\"url\":\""))
                        mediaId = Regex.Matches(imageUploadResponse, "\"url\":\"(.*?)\"")[0].Groups[1].ToString();
                    return mediaId;

                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return mediaId;
        }


        public SearchLikedPostResponse SearchLikedPost(DominatorAccountModel tumbleraccount, string nextPageUrl)
        {
            try
            {
                var response = GetApiResponse(tumbleraccount, nextPageUrl, ConstantHelpDetails.BearerToken, referer: "https://www.tumblr.com/likes");
                UpdateCsrf();
                return new SearchLikedPostResponse(response.Response);
            }
            catch (Exception)
            {
                return null;
            }
        }


        public static byte[] ImageToByteArray(string imageFile, ImageFormat format)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var image = Image.FromFile(imageFile))
                {
                    System.Drawing.Imaging.ImageFormat format1;

                    switch (format)
                    {
                        case ImageFormat.Jpeg:
                            format1 = System.Drawing.Imaging.ImageFormat.Jpeg;
                            break;
                        case ImageFormat.Png:
                            format1 = System.Drawing.Imaging.ImageFormat.Png;
                            break;
                        case ImageFormat.Gif:
                            format1 = System.Drawing.Imaging.ImageFormat.Gif;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    image.Save(memoryStream, format1);
                    return memoryStream.ToArray();
                }
            }
        }
        /// <summary>
        /// </summary>
        /// <param name="MediaPath"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static byte[] DownloadMediaFromUrl(string mediaPath)
        {
            try
            {
                var Client = new WebClient();
                return Client.DownloadData(mediaPath);
            }
            catch (Exception)
            {
            }

            return new byte[0];
        }
        /// <summary>
        ///     Search user with username
        /// </summary>
        /// <param name="tumbleraccount"></param>
        /// <param name="keyword"></param>
        /// <param name="pageId"></param>
        /// <param name="xTumblrFormKey"></param>
        /// <returns></returns>
        public IResponseParameter GetUserDetailsResponseApi(DominatorAccountModel tumbleraccount, string username)
        {
            string searchUserDetailsUrl = ConstantHelpDetails.GetUserDetailsAPIByUserName(username);
            try
            {
                var tdRequestParameter = (TumblrRequestParameter)HttpHelper.GetRequestParameter();
                tdRequestParameter.Accept = "application/json;format=camelcase";

                if (!string.IsNullOrEmpty(ConstantHelpDetails.BearerToken)) tdRequestParameter.Headers["Authorization"] = ConstantHelpDetails.BearerToken;
                tdRequestParameter.Headers.Remove("Origin");
                tdRequestParameter.Headers.Remove("Upgrade-Insecure-Requests");
                tdRequestParameter.Headers.Remove("X-CSRF");
                tdRequestParameter.Headers["X-Version"] = "redpop/3/0//redpop/";
                tdRequestParameter.Headers.Remove("ContentType");
                tdRequestParameter.ContentType = "application/json; charset=utf8";
                tdRequestParameter.Accept = "application/json;format=camelcase";
                tdRequestParameter.Headers["sec-ch-ua"] = "\"Not/A)Brand\";v=\"99\", \"Google Chrome\";v=\"115\", \"Chromium\";v=\"115\"";
                tdRequestParameter.Headers["sec-ch-ua-mobile"] = "?0";
                tdRequestParameter.Headers["sec-ch-ua-platform"] = "\"Windows\"";
                tdRequestParameter.Headers["Sec-Fetch-Dest"] = "empty";
                tdRequestParameter.Headers["Sec-Fetch-Mode"] = "cors";
                tdRequestParameter.Headers["Origin"] = "www.tumblr.com";
                tdRequestParameter.Headers["Sec-Fetch-Site"] = "same-origin";
                tdRequestParameter.Headers["X-Ad-Blocker-Enabled"] = "0";
                tdRequestParameter.Referer = $"https://www.tumblr.com/{username}";
                setCookieForRequest(tdRequestParameter, tumbleraccount);
                if (tumbleraccount.AccountBaseModel.AccountProxy.ProxyIp != null)
                    tdRequestParameter.Proxy = tumbleraccount.AccountBaseModel.AccountProxy;
                HttpHelper.SetRequestParameter(tdRequestParameter);
                IResponseParameter resposeForUser = HttpHelper.GetRequest(searchUserDetailsUrl);
                UpdateCsrf();
                return resposeForUser;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public IResponseParameter TumblrServiceApi(DominatorAccountModel dominatorAccountModel, object servicePostData)
        {
            try
            {
                var tdRequestparameter = (TumblrRequestParameter)HttpHelper.GetRequestParameter();
                tdRequestparameter.Headers["Authorization"] = ConstantHelpDetails.BearerToken;
                tdRequestparameter.Headers.Remove("ContentType");
                tdRequestparameter.ContentType = "application/json; charset=utf8";
                tdRequestparameter.Accept = "application/json;format=camelcase";
                if (dominatorAccountModel.AccountBaseModel.AccountProxy.ProxyIp != null)
                    HttpHelper.GetRequestParameter().Proxy = dominatorAccountModel.AccountBaseModel.AccountProxy;
                HttpHelper.SetRequestParameter(tdRequestparameter);
                var _servicePostData = JsonConvert.SerializeObject(servicePostData);
                var response = HttpHelper.PostRequest(ConstantHelpDetails.ServiceApiUrl, Encoding.UTF8.GetBytes(_servicePostData));
                UpdateCsrf();
                return response;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public void setCookieForRequest(IRequestParameters requestParameters, DominatorAccountModel dominatorAccountModel)
        {
            if (requestParameters.Cookies is null || requestParameters.Cookies.Count == 0
                || requestParameters.Cookies.Cast<Cookie>().Any(x => x.Name.Contains("logged_in") && x.Value == "0"))
            {
                if ((dominatorAccountModel.Cookies?.Count == 0 ||
                    dominatorAccountModel.CookieHelperList.Any(x => x.Name.Contains("logged_in") && x.Value == "0"))
                    && dominatorAccountModel.BrowserCookies.Count > 0)
                {
                    requestParameters.Cookies = dominatorAccountModel.BrowserCookies;
                }
                else if (dominatorAccountModel.Cookies?.Count != 0 &&
                    !dominatorAccountModel.CookieHelperList.Any(x => x.Name.Contains("logged_in") && x.Value == "0"))
                    requestParameters.Cookies = dominatorAccountModel.Cookies;
            }

        }

        public void UpdateCsrf()
        {
            try
            {
                var csrf = HttpHelper.Response.Headers["X-Csrf"];
                if (!string.IsNullOrEmpty(csrf))
                    Csrf_Token = csrf;
            }
            catch { }
        }
    }


    public enum ImageFormat
    {
        Jpeg,
        Png,
        Gif
    }
}