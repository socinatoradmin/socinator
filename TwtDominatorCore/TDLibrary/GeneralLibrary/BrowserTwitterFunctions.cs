#region using

using CefSharp;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.TdTables.Accounts;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.TdQuery;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.Config;
using DominatorHouseCore.PuppeteerBrowser;
using DominatorHouseCore.Request;
using DominatorHouseCore.Utility;
using EmbeddedBrowser;
using EmbeddedBrowser.BrowserHelper;
using HtmlAgilityPack;
using MahApps.Metro.Controls;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using ThreadUtils;
using TwtDominatorCore.Database;
using TwtDominatorCore.Interface;
using TwtDominatorCore.Requests;
using TwtDominatorCore.Response;
using TwtDominatorCore.Response.ProfileReponseHandlerPack;
using TwtDominatorCore.TDFactories;
using TwtDominatorCore.TDLibrary.GeneralLibrary;
using TwtDominatorCore.TDModels;
using TwtDominatorCore.TDUtility;
using static TwtDominatorCore.TDEnums.Enums;
using Application = System.Windows.Application;
using Cookie = System.Net.Cookie;

#endregion

namespace TwtDominatorCore.TDLibrary
{
    public class BrowserTwitterFunctions : ITwitterFunctions
    {
        protected readonly IContentUploaderService _contentUploaderService;
        private readonly IDateProvider _dateProvider;
        private readonly IDelayService _delayService;
        private readonly ITdHttpHelper _httpHelper;
        private CancellationToken _token;
        private readonly ITwitterAccountSessionManager twitterAccountSession;
        public BrowserTwitterFunctions()
        {
            _dateProvider = InstanceProvider.GetInstance<IDateProvider>();
            _delayService = InstanceProvider.GetInstance<IDelayService>();
            _httpHelper = InstanceProvider.GetInstance<ITdHttpHelper>();
            _contentUploaderService = InstanceProvider.GetInstance<IContentUploaderService>();
            twitterAccountSession = InstanceProvider.GetInstance<ITwitterAccountSessionManager>();
        }
        private static string Domain => TdConstants.Domain;
        private BrowserWindow BrowserWindow { get; set; }
        private BrowserAutomationExtension _AutomationExtension { get; set; }
        private BrowserWindow SecondaryBrowserWindow { get; set; }
        public bool IsScheduleNext { get; set; }
        public int UsedQueryCount { get; set; }
        public DominatorAccountModel accountModel { get; set; }
        public async Task<LogInResponseHandler> LoginUsingFlowToken(DominatorAccountModel twitterAccount,
            CancellationToken cancellationToken)
        {
            var tdRequestParameter = (TdRequestParameters)_httpHelper.GetRequestParameter();
            var account = new AccountModel(twitterAccount);
            accountModel = twitterAccount;
            tdRequestParameter.SetupHeaders();
            return null;
        }

        public async Task<LogInResponseHandler> LogInAsync(DominatorAccountModel twitterAccount,
            CancellationToken cancellationToken)
        {
            var tdRequestParameter = (TdRequestParameters) _httpHelper.GetRequestParameter();
            var account = new AccountModel(twitterAccount);
            var MaxIteration = TdConstants.MaxIterationForLogin;
            try
            {
                while (MaxIteration > 0)
                {
                    tdRequestParameter.SetupHeaders();
                    var ResponseForUriMatrix = _httpHelper
                        .GetRequestAsync(TdConstants.UriMatrixUrl, cancellationToken).Result.Response;
                    var UriMatrix = TdUtility.getUriMatrix(ResponseForUriMatrix);
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

                    var PostData = "session%5Busername_or_email%5D=" +
                                   HttpUtility.UrlEncode(twitterAccount.AccountBaseModel.UserName) +
                                   "&session%5Bpassword%5D=" +
                                   HttpUtility.UrlEncode(twitterAccount.AccountBaseModel.Password) +
                                   "&authenticity_token=" + account.postAuthenticityToken + "&ui_metrics=" +
                                   UriMatrix + "&scribe_log=&redirect_after_login=&authenticity_token=" +
                                   account.postAuthenticityToken;
                    tdRequestParameter.SetupHeaders(Path: TdConstants.SessionUrl,Method:"POST", GuestID: GuestID(twitterAccount));
                    tdRequestParameter.Referer = TdConstants.MainUrl + "login";
                    var responseHandler = new LogInResponseHandler(
                        await _httpHelper.PostRequestAsync(TdConstants.SessionUrl, PostData,
                            cancellationToken));


                    await _delayService.DelayAsync(TdConstants.ConsecutiveGetReqInteval, cancellationToken);


                    if (responseHandler.Success && responseHandler.Response.Contains("\"screen_name\":\""))
                    {
                        twitterAccount.AccountBaseModel.Status = responseHandler.DominatorStatus;
                        twitterAccount.Cookies = _httpHelper.GetRequestParameter().Cookies;
                        // don't concat this condition to &&  
                        if (SwitchToOldTwitterUi(twitterAccount))
                        {
                            twitterAccount.AccountBaseModel.Status = responseHandler.DominatorStatus;
                        }
                        else
                        {
                            twitterAccount.AccountBaseModel.Status = AccountStatus.Failed;
                            return responseHandler;
                        }
                    }
                    else if (responseHandler.Success)
                    {
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
                            _browserWindow = new BrowserWindow(twitterAccount)
                            {
                                Visibility = Visibility.Hidden
                            }; //,userAgent: "Mozilla/5.0 (Windows NT 6.3; Trident/7.0; rv:11.0) like Gecko"
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
            if (IsHitUrl)
                HomePageResponse = BrowserWindow.GetPageSource();
            // var responseResponseResult = BrowserWindow.TwitterJsonResponse();
            // var key = $"https://api.{Domain}/2/timeline/home.json?include_profile_interstitial_type=1&include_blocking=1&include_blocked_by=1&include_followed_by=1&include_want_retweets=1&include_mute_edge=1&include_can_dm=1&include_can_media_tag=1&skip_status=1&cards_platform=Web-12&include_cards=1&include_composer_source=true&include_ext_alt_text=true&include_reply_count=1&tweet_mode=extended&include_entities=true&include_user_entities=true&include_ext_media_color=true&include_ext_media_availability=true&send_error_codes=true&earned=1&count=20&lca=true&ext=mediaStats%2ChighlightedLabel%2CcameraMoment";
            // HomePageResponse= TdUtility.GetResponseHandlerWithkeys(responseResponseResult, key);

            var objAccountDetails = new AccountDetails();
            var UserName = Utilities.GetBetween(HomePageResponse, "\"screen_name\":\"", "\"");
            var TweetCount = string.Empty;
            var FollowerCount = string.Empty;
            var FollowingCount = string.Empty;
            try
            {
                var htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(HomePageResponse);
                var userId = Utilities.GetBetween(HomePageResponse, "\"user_id\":\"", "\"");

                try
                {
                    TweetCount = Utilities.GetBetween(HomePageResponse, "\"statuses_count\":", ",");
                    FollowingCount = Utilities.GetBetween(HomePageResponse, "\"friends_count\":", ",");
                    FollowerCount = Utilities.GetBetween(HomePageResponse, "\"followers_count\":", ",");
                }
                catch (Exception exception)
                {
                    exception.DebugLog();
                }

                objAccountDetails.ProfileName = Utilities.GetBetween(HomePageResponse, "\"name\":\"", "\"");

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

                // update only when check account is selected from Account manager
                if (IsHitUrl)
                {
                    await _delayService.DelayAsync(TdConstants.ConsecutiveGetReqInteval, CancellationToken);
                    var SettingRespose =
                        (await _httpHelper.GetRequestAsync(TdConstants.MainUrl + "settings/account",
                            CancellationToken)).Response;
                    SettingRespose = SettingRespose == null ? "" : SettingRespose;
                    objAccountDetails.UserName = string.IsNullOrEmpty(UserName)
                        ? Utilities.GetBetween(SettingRespose, "orig_uname\" value=\"", "\"")
                        : UserName;
                    objAccountDetails.Email = Utilities.GetBetween(SettingRespose, "orig_email\" value=\"", "\"");
                    objAccountDetails.UserId = string.IsNullOrEmpty(userId)
                        ? HtmlAgilityHelper.getValueWithAttributeNameFromInnerHtml(SettingRespose, "nav right-actions",
                            "data-user-id")
                        : userId;
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

            var ResponseResult = string.Empty;
            try
            {
                string key;
                UserName = UserName.Trim();
                var userId = "";
                if (UserName.Contains(TdConstants.MainUrl))
                    UserName = TdUtility.GetUserNameFromUrl(UserName);
                FollowerFollowingResponseHandler ResponseHandler = null;
                while (maxIteration >= 0)
                {
                    string requestUrl;
                    if (string.IsNullOrEmpty(MinPosition))
                    {
                        requestUrl = $"{TdConstants.MainUrl}{UserName}/followers";
                        _AutomationExtension.LoadAndScroll(requestUrl, 15, true, 5000);
                        var responseResponseResult = new List<KeyValuePair<string, MemoryStreamResponseFilter>>();
                        try {
                            responseResponseResult = TdUtility.GetResponseData(BrowserWindow); //BrowserWindow.TwitterJsonResponse();
                            var userKey = TdConstants.GetUserDetailsFromFollowersResponse(UserName.ToLower());
                            ResponseResult = TdUtility.GetResponse(responseResponseResult, userKey, UserName.ToLower(),
                                UserName);

                            if (string.IsNullOrEmpty(userId = Utilities.GetBetween(ResponseResult, "id_str\":\"", "\"")))
                                userId = Utilities.GetBetween(ResponseResult, "\"rest_id\":\"", "\"");
                            key = TdConstants.GetUserFollowerAndFollowings(userId, NewUIFollowType.followers);
                            ResponseResult = TdUtility.GetResponseHandlerWithkeys(responseResponseResult, key);
                            if (string.IsNullOrEmpty(ResponseResult))
                            {
                                key = $"https://api.{Domain}/graphql/";
                                ResponseResult = TdUtility.GetResponseHandlerWithkeysContain(responseResponseResult, key);
                            }
                        }
                        catch(Exception)
                        {
                            responseResponseResult?.ForEach(x => x.Value?.Dispose());
                            throw;
                        }
                        finally
                        {
                            responseResponseResult?.ForEach(x => x.Value?.Dispose());                            
                        } 
                    }
                    else
                    {
                        var PaginationResponseResult = new List<KeyValuePair<string, MemoryStreamResponseFilter>>();
                        try
                        {
                            _AutomationExtension.ScrollWindow(5000);
                            PaginationResponseResult = TdUtility.GetResponseData(BrowserWindow);
                            var userKey = TdConstants.GetUserDetailsFromFollowersResponse(UserName);
                            ResponseResult = TdUtility.GetResponse(PaginationResponseResult, userKey, UserName.ToLower(),
                                UserName);

                            if (string.IsNullOrWhiteSpace(
                                userId = Utilities.GetBetween(ResponseResult, "id_str\":\"", "\"")))
                                userId = Utilities.GetBetween(ResponseResult, "rest_id\":\"", "\"");


                            key = TdConstants.GetUserFollowerAndFollowings(userId, NewUIFollowType.followers, MinPosition);
                            ResponseResult = TdUtility.GetResponseHandlerWithkeys(PaginationResponseResult, key);
                            if (string.IsNullOrEmpty(ResponseResult))
                            {
                                var SpiltMinPostion = MinPosition.Split('|');
                                key = $"https://api.{Domain}/graphql/";
                                ResponseResult = TdUtility.GetResponseHandlerWithkeysContain(PaginationResponseResult, key,
                                    secondarykeys: SpiltMinPostion[0]);
                            } 
                        }
                        catch (Exception)
                        {
                            PaginationResponseResult?.ForEach(x => x.Value?.Dispose());
                            throw;
                        }
                        finally
                        {
                            PaginationResponseResult?.ForEach(x => x.Value?.Dispose());
                        }
                    }

                    ResponseHandler =
                        new FollowerFollowingResponseHandler(
                            new ResponseParameter {Response = ResponseResult});
                    if (ResponseHandler.Success) break;

                    _delayService.ThreadSleep(TdConstants.FloodWait);
                    maxIteration--;
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


        public async Task<FollowerFollowingResponseHandler> GetUserFollowingsAsync(DominatorAccountModel twitterAccount,
            string UserName, CancellationToken CancellationToken, string MinPosition = null)
        {
            var MaxIteration = TdConstants.MaxIteration;
            try
            {
                var ResponseResult = string.Empty;
                UserName = UserName.Trim();
                if (UserName.Contains(TdConstants.MainUrl))
                    UserName = TdUtility.GetUserNameFromUrl(UserName);
                FollowerFollowingResponseHandler ResponseHandler = null;
                while (MaxIteration >= 0)
                {
                    string requestUrl;
                    string userId;
                    string key;
                    if (MinPosition == null)
                    {
                        var responseResponseResult = new List<KeyValuePair<string, MemoryStreamResponseFilter>>();
                        try
                        {
                            requestUrl = $"{TdConstants.MainUrl}{UserName}/following";
                            _AutomationExtension.LoadAndScroll(requestUrl, 10, true, 5000);
                            responseResponseResult = TdUtility.GetResponseData(BrowserWindow);
                            var userKey = TdConstants.GetUserDetailsFromFollowersResponse(UserName.ToLower());
                            ResponseResult = TdUtility.GetResponseHandlerWithkeysContain(responseResponseResult, userKey);
                            if (string.IsNullOrEmpty(ResponseResult))
                            {
                                userKey = UserName;
                                ResponseResult =
                                    TdUtility.GetResponseHandlerWithkeysContain(responseResponseResult, userKey);
                            }

                            userId = Utilities.GetBetween(ResponseResult, "id_str\":\"", "\"");
                            if (string.IsNullOrEmpty(userId))
                            {
                                userKey = UserName.ToLower();
                                ResponseResult =
                                    TdUtility.GetResponseHandlerWithkeysContain(responseResponseResult, userKey);
                                userId = Utilities.GetBetween(ResponseResult, "\"user_id\":\"", "\"");
                            }

                            key = TdConstants.GetUserFollowerAndFollowings(userId, NewUIFollowType.friends);
                            ResponseResult = TdUtility.GetResponseHandlerWithkeysContain(responseResponseResult, key);
                            if (string.IsNullOrEmpty(ResponseResult))
                            {
                                key = $"https://api.{Domain}/graphql/";
                                ResponseResult = TdUtility.GetResponseHandlerWithkeysContain(responseResponseResult, key,
                                    secondarykeys: "Following");
                            }
                        }
                        catch (Exception)
                        {
                            responseResponseResult?.ForEach(x => x.Value?.Dispose());
                            throw;
                        }
                        finally
                        {
                            responseResponseResult?.ForEach(x => x.Value?.Dispose());
                        } 
                    }
                    else
                    {
                        if (MinPosition == "0")
                            break;
                        var PaginationResponseResult = new List<KeyValuePair<string, MemoryStreamResponseFilter>>();
                        try
                        {
                            _AutomationExtension.ScrollWindow(5000);
                            PaginationResponseResult = TdUtility.GetResponseData(BrowserWindow);
                            var userKey = TdConstants.GetUserDetailsFromFollowersResponse(UserName.ToLower());
                            ResponseResult = TdUtility.GetResponseHandlerWithkeys(PaginationResponseResult, userKey);
                            if (string.IsNullOrEmpty(ResponseResult))
                            {
                                userKey = UserName;
                                ResponseResult =
                                    TdUtility.GetResponseHandlerWithkeysContain(PaginationResponseResult, userKey);
                            }

                            userId = Utilities.GetBetween(ResponseResult, "id_str\":\"", "\"");
                            if (string.IsNullOrEmpty(userId))
                            {
                                userKey = UserName.ToLower();
                                ResponseResult =
                                    TdUtility.GetResponseHandlerWithkeysContain(PaginationResponseResult, userKey);
                                userId = Utilities.GetBetween(ResponseResult, "\"user_id\":\"", "\"");
                            }

                            key = TdConstants.GetUserFollowerAndFollowings(userId, NewUIFollowType.friends, MinPosition);
                            ResponseResult = TdUtility.GetResponseHandlerWithkeys(PaginationResponseResult, key);
                            if (string.IsNullOrEmpty(ResponseResult))
                            {
                                var SpiltMinPostion = MinPosition.Split('|');
                                key = $"https://api.{Domain}/graphql/";
                                ResponseResult = TdUtility.GetResponseHandlerWithkeysContain(PaginationResponseResult, key,
                                    secondarykeys: SpiltMinPostion[0]);
                            }
                        }
                        catch (Exception)
                        {
                            PaginationResponseResult?.ForEach(x => x.Value?.Dispose());
                            throw;
                        }
                        finally
                        {
                            PaginationResponseResult?.ForEach(x => x.Value?.Dispose());
                        }  
                    }

                    ResponseHandler =
                        new FollowerFollowingResponseHandler(
                            new ResponseParameter {Response = ResponseResult});

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
            string ReqUrl;
            var TweetId = string.Empty;
            var responseData = new List<KeyValuePair<string, MemoryStreamResponseFilter>>();

            try
            {
                TweetUrl = TweetUrl.Trim();
                if (TweetUrl.Contains(TdConstants.MainUrl))
                {
                    TweetId = TdUtility.GetTweetIdFromUrl(TweetUrl);
                    var UserName = TdUtility.GetUserNameFromUrl(TweetUrl);
                    ReqUrl = $"{TdConstants.MainUrl}{UserName}/status/{TweetId}";
                }
                else
                {
                    ReqUrl = TdConstants.MainUrl + "anyuser/status/" + TweetUrl;
                }

                if (string.IsNullOrEmpty(TweetId))
                    TweetId = TweetUrl;
                _AutomationExtension.LoadAndScroll(ReqUrl, 15, true, 2000);

                responseData = TdUtility.GetResponseData(BrowserWindow);
                var key = TdConstants.GetUserCommentTweet(TweetId);
                var FinaResponsedata = TdUtility.GetResponseHandlerWithkeysContain(responseData, key);

                var responseHandler = new SingleTweetDetailsResponseHandler(new ResponseParameter
                    {Response = $"\"{TweetId}\"{FinaResponsedata}"});
                return responseHandler;
            }
            catch (OperationCanceledException)
            {
                responseData?.ForEach(x => x.Value?.Dispose()); 
                throw new OperationCanceledException();
            }
            catch (Exception)
            {
                responseData?.ForEach(x => x.Value?.Dispose()); 
                return null;
            }
            finally
            {
                responseData?.ForEach(x => x.Value?.Dispose());
            }
        }

        public SearchTagResponseHandler SearchForTag(DominatorAccountModel twitterAccount, string keyword,
            string queryType, CancellationToken token, string minPosition = null,string productType="Top")
        {
            var responseData = new List<KeyValuePair<string, MemoryStreamResponseFilter>>();
            try
            {
                var FinalResponseData = string.Empty;
                var key = string.Empty;
                var requestUrl = string.Empty;
                if (queryType == "Hashtags")
                {
                    if (!keyword.Contains("#"))
                        keyword = $"#{keyword}";
                    keyword = Uri.EscapeDataString(keyword);
                }
                else if (queryType == "Location Users" || queryType == "Location Tweets")
                {
                    var keys = Regex.Split(keyword, ":");

                    requestUrl =
                        $"https://{Domain}/search?q={keys[0]}%20near%3A{keys[1]}%2C{keys[2]}%20within%3A{keys[3]}mi&src=typed_query";
                    keyword = Utilities.GetBetween(requestUrl, "search?q=", "&src=").Replace("\"", "");
                }
                else if (queryType == "Near My Location")
                {
                    requestUrl = $"https://{Domain}/search?q={keyword}&src=typed_query&lf=on";
                    keyword = $"{keyword}%20near%3Ame";
                }
                else if (queryType == "Specific User Tweets")
                {
                    keyword = keyword.Trim();
                    requestUrl = $"https://{Domain}/{keyword}";
                }

                if (string.IsNullOrEmpty(minPosition))
                {
                    keyword = keyword.Trim();
                    if (queryType != "Location Users" && queryType != "Near My Location" &&
                        queryType != "Location Tweets" && queryType != "Specific User Tweets")
                        requestUrl = $"https://{Domain}/search?q={keyword}&src=typed_query";
                    _AutomationExtension.LoadAndScroll(requestUrl, 15, true, 5000);
                    if (queryType == "Specific User Tweets")
                    {
                        var pageResponse = BrowserWindow.GetPageSource();
                        var userId = Utilities.GetBetween(pageResponse, "profile_banners/", "/");
                        key = TdConstants.GetTweets(userId, "");
                        responseData = TdUtility.GetResponseData(BrowserWindow);
                        FinalResponseData = TdUtility.GetResponseHandlerWithkeysContain(responseData, key);
                    }
                    else
                    {
                        if (queryType != "Hashtags" && queryType != "Near My Location" &&
                            queryType != "Location Users" && queryType != "Location Tweets")
                            keyword = WebUtility.UrlEncode(keyword);
                        key = TdConstants.GetSearchTweets(keyword, minPosition);
                        responseData = TdUtility.GetResponseData(BrowserWindow);
                        FinalResponseData = TdUtility.GetResponseHandlerWithkeysContain(responseData, key);
                    }
                }
                else
                {
                    if (queryType != "Specific User Tweets")
                        requestUrl = $"https://{Domain}/search?q={keyword}&src=typed_query";
                    _AutomationExtension.LoadAndScroll(requestUrl, 20, true, 5000);
                    responseData = TdUtility.GetResponseData(BrowserWindow);
                    if (queryType != "Specific User Tweets")
                    {
                        key = TdConstants.GetSearchTweets(keyword, minPosition);
                    }
                    else
                    {
                        var pageResponse = BrowserWindow.GetPageSource();
                        var userId = Utilities.GetBetween(pageResponse, "profile_banners/", "/");
                        key = TdConstants.GetTweetsWithCursor(userId, minPosition);
                    }

                    FinalResponseData = TdUtility.GetResponseHandlerWithkeysContain(responseData, key);
                }

                var ResponseHandler =
                    new SearchTagResponseHandler(new ResponseParameter {Response = FinalResponseData});

                _delayService.ThreadSleep(TdConstants.FloodWait);


                token.ThrowIfCancellationRequested();
                return ResponseHandler;
            }
            catch (OperationCanceledException)
            {
                responseData?.ForEach(x => x.Value?.Dispose());
                throw new OperationCanceledException();
            }
            catch (Exception Ex)
            {
                responseData?.ForEach(x => x.Value?.Dispose());
                Ex.ErrorLog();
                return null;
            }
            finally
            {
                responseData?.ForEach(x => x.Value?.Dispose());
            }
        }

        public MuteResponseHandler Mute(DominatorAccountModel twitterAccount, string UserId, string UserName)
        {
            try
            {
                var Success = false;
                UserName = UserName.Trim();
                MuteResponseHandler ResponseHandler = null;
                var RequestUrl = TdConstants.MainUrl + UserName;

                _AutomationExtension.LoadAndScroll(RequestUrl, 15);
                _AutomationExtension.ExecuteScript(
                    "document.getElementsByClassName('css-18t94o4 css-1dbjc4n r-1niwhzg r-p1n3y5 r-sdzlij r-1phboty r-rs99b7 r-1w2pmg r-15d164r r-zso239 r-1vuscfd r-53xb7h r-mk0yit r-o7ynqc r-6416eg r-lrvibr')[0].click()",
                    5, BrowserWindow);

                var getaxis = _AutomationExtension.GetXAndY($"//span[contains(text(),'Mute @{UserName}')]",
                    AttributeIdentifierType.Xpath);
                if (!getaxis.Equals(null) && getaxis.Key != 0 && getaxis.Value != 0)
                {
                    BrowserWindow.MouseClick(getaxis.Key + 5, getaxis.Value + 5, delayAfter: 15);
                    Success = true;
                }


                // BrowserWindow.BrowserAct(ActType.ClickByClass, "r-13gxpu9 r-4qtqp9 r-yyyyoo r-1q142lx r-50lct3 r-dnmrzs r-bnwqim r-1plcrui r-lrvibr", delayAfter: 5);


                ResponseHandler = new MuteResponseHandler(new ResponseParameter
                    {Response = Success ? BrowserWindow.GetPageSource() : ""});
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

        public FollowResponseHandler Follow(DominatorAccountModel twitterAccount, string UserId, string UserName = null,
            string queryType = null)
        {
            try
            {
                var browser = CheckandAssignBrowser(twitterAccount, queryType);
                var autom = new BrowserAutomationExtension(browser, _AutomationExtension.jobCancellationToken);
                autom.LoadAndScroll($"https://{Domain}/{UserName}", 15);
                autom.ExecuteScript("window.scrollTo(0, 0)", 2);
                //var isSuccess = autom.ExecuteScript("document.getElementsByClassName('user-actions-follow-button js-follow-btn follow-button')[0].click()", 5, browser).Success;
                var getaxis =
                    autom.GetXAndY(
                        "css-18t94o4 css-1dbjc4n r-1niwhzg r-p1n3y5 r-sdzlij r-1phboty r-rs99b7 r-1w2pmg r-1vuscfd r-1dhvaqw r-1fneopy r-o7ynqc r-6416eg r-lrvibr",
                        AttributeIdentifierType.ClassName);
                browser.MouseClick(getaxis.Key + 6, getaxis.Value + 6, delayAfter: 8);
                var response = new FollowResponseHandler(new ResponseParameter {Response = browser.GetPageSource()});

                if (response.Issue?.Message ==
                    "You must write ContentLength bytes to the request stream before calling")
                {
                    _delayService.ThreadSleep(TimeSpan.FromSeconds(1.5));
                    response = new FollowResponseHandler(new ResponseParameter
                        {Response = SecondaryBrowserWindow.GetPageSource()});
                }

                return response;
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

        public UnfollowResponseHandler Unfollow(DominatorAccountModel twitterAccount, string UserId, string Username)
        {
            try
            {
                var Success = false;
                UnfollowResponseHandler ResponseHandler = null;
                var RequestUrl = TdConstants.MainUrl + Username;

                //assigning secondary browser for user outside s/w
                AssignSecondaryBrowser(twitterAccount);
                var automation =
                    new BrowserAutomationExtension(SecondaryBrowserWindow, _AutomationExtension.jobCancellationToken);
                automation.LoadAndScroll(RequestUrl, 15);

                var getaxis = automation.GetXAndY(
                    "//div[@class='css-18t94o4 css-1dbjc4n r-urgr8i r-42olwf r-sdzlij r-1phboty r-rs99b7 r-1w2pmg r-1vuscfd r-1dhvaqw r-1fneopy r-o7ynqc r-6416eg r-lrvibr']//div[@class='css-901oao r-1awozwy r-jwli3a r-6koalj r-18u37iz r-16y2uox r-1qd0xha r-a023e6 r-vw2c0b r-1777fci r-eljoum r-dnmrzs r-bcqeeo r-q4m81j r-qvutc0']",
                    AttributeIdentifierType.Xpath);
                SecondaryBrowserWindow.MouseClick(getaxis.Key, getaxis.Value, delayAfter: 7);

                var getfinalaxis = automation.GetXAndY(
                    "//span[@class='css-901oao css-16my406 css-bfa6kz r-1qd0xha r-ad9z0x r-bcqeeo r-qvutc0']//span[@class='css-901oao css-16my406 r-1qd0xha r-ad9z0x r-bcqeeo r-qvutc0'][contains(text(),'Unfollow')]",
                    AttributeIdentifierType.Xpath);
                if (!getfinalaxis.Equals(null))
                {
                    SecondaryBrowserWindow.MouseClick(getfinalaxis.Key, getfinalaxis.Value, delayAfter: 5);
                    Success = true;
                }

                ResponseHandler =
                    new UnfollowResponseHandler(new ResponseParameter
                        {Response = Success ? SecondaryBrowserWindow.GetPageSource() : ""});
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

        public LikeResponseHandler Like(DominatorAccountModel twitterAccount, string TweetId, string UserName,
            string queryType)
        {
            try
            {
                TweetId = TweetId.Trim();
                UserName = UserName.Trim();
                LikeResponseHandler ResponseHandler = null;
                var RequestUrl = $"{TdConstants.MainUrl}{UserName}/status/{TweetId}";
                var browser = CheckandAssignBrowser(twitterAccount, queryType);
                var automationExtension =
                    new BrowserAutomationExtension(browser, _AutomationExtension.jobCancellationToken);
                automationExtension.LoadAndScroll(RequestUrl, 25, true, 250);
                var Success = automationExtension
                    .ExecuteScript(
                        "document.getElementsByClassName('css-1dbjc4n r-sdzlij r-1p0dtai r-xoduu5 r-1d2f490 r-xf4iuw r-u8s1d r-zchlnj r-ipm5af r-o7ynqc r-6416eg')[2].click()",
                        5).Success;

                ResponseHandler = new LikeResponseHandler(new ResponseParameter
                    {Response = Success ? browser.GetPageSource() : ""});
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

        public UnlikeResponseHandler Unlike(DominatorAccountModel twitterAccount, string TweetId, string UserName)
        {
            try
            {
                TweetId = TweetId.Trim();
                UserName = UserName.Trim();
                UnlikeResponseHandler ResponseHandler = null;
                var RequestUrl = $"{TdConstants.MainUrl}{UserName}/status/{TweetId}";
                _AutomationExtension.LoadAndScroll(RequestUrl, 15, true, 1000);
                var Success = _AutomationExtension
                    .ExecuteScript(
                        "document.getElementsByClassName('css-1dbjc4n r-sdzlij r-1p0dtai r-xoduu5 r-1d2f490 r-xf4iuw r-u8s1d r-zchlnj r-ipm5af r-o7ynqc r-6416eg')[2].click()",
                        5).Success;

                ResponseHandler =
                    new UnlikeResponseHandler(new ResponseParameter
                        {Response = Success ? BrowserWindow.GetPageSource() : ""});
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

        public CommentResponseHandler Comment(DominatorAccountModel twitterAccount, string tweetId, string userName,
            string text, string queryType, List<string> listFilepath = null, List<string> listUserNameToTag = null)
        {
            try
            {
                tweetId = tweetId.Trim();
                userName = userName.Trim();
                CommentResponseHandler responseHandler = null;
                var reqUrl = $"{TdConstants.MainUrl}{userName}/status/{tweetId}";
                text = text.Trim();
                if (text.Length > TdConstants.MaxCharactersAllowedToTweet)
                {
                    GlobusLogHelper.log.Info(string.Format(
                        "LangKeyCommentTextMoreThanSomeAndReduceToSomeCharacter".FromResourceDictionary(),
                        "LangKeyComment".FromResourceDictionary(), TdConstants.MaxCharactersAllowedToTweet,
                        TdConstants.MaxCharactersAllowedToTweet));
                    text = text.Substring(0, TdConstants.MaxCharactersAllowedToTweet);
                }

                var browser = CheckandAssignBrowser(twitterAccount, queryType,true);
                SetBrowser(twitterAccount,twitterAccount.Token, BrowserInstanceType.Secondary);
                var automation = new BrowserAutomationExtension(browser, _AutomationExtension.jobCancellationToken);
                automation.LoadAndScroll(reqUrl, 10);
                automation.ExecuteScript("document.querySelector('button[data-testid=\"reply\"]').scrollIntoViewIfNeeded();", delayInSec: 3);
                var XResult = automation.ExecuteScript("document.querySelector('button[data-testid=\"reply\"]').getBoundingClientRect().x", 3);
                var YResult = automation.ExecuteScript("document.querySelector('button[data-testid=\"reply\"]').getBoundingClientRect().y", 3);
                var X = TdUtility.ConvertDoubleAndInt(XResult?.Result?.ToString());
                var Y = TdUtility.ConvertDoubleAndInt(YResult?.Result?.ToString());
                browser.MouseClick(X + 5, Y + 5, delayAfter: 6);
                var xyArea = browser.GetXAndY(AttributeType.ClassName, "public-DraftStyleDefault-block public-DraftStyleDefault-ltr");
                browser.MouseClick(xyArea.Key + 5, xyArea.Value + 5);
                var Lines = text?.Replace("\r\n","\n")?.Split('\n')?.ToList();
                foreach(var line in Lines)
                {
                    if(!string.IsNullOrEmpty(line))
                        browser.EnterChars(" "+line, delayAtLast: 3);
                    if (line != Lines.LastOrDefault())
                        browser.PressAnyKey(winKeyCode:13,delayAtLast:2);
                }
                var counter = browser.ExecuteScript("document.querySelector('div[data-testid=\"countdown-circle\"]').innerText", delayInSec: 2);
                if(counter != null && counter.Result != null)
                {
                    var countLimit = Regex.Match(counter?.Result?.ToString(), "[0-9]")?.Value;
                    int.TryParse(countLimit, out int count);
                    if (count > 0)
                    {
                        while (count-- > 0)
                            browser.PressAnyKey(winKeyCode: 8, delayAtLast: 1);
                    }
                }
                if(listFilepath != null && listFilepath.Count> 0)
                {
                    var script = "document.querySelector('button[aria-label=\"Add photos or video\"]').getBoundingClientRect().{0}";
                    browser.ChooseFileFromDialog(string.Empty, listFilepath);
                    XResult = automation.ExecuteScript(string.Format(script,"x"), 3);
                    YResult = automation.ExecuteScript(string.Format(script,"y"), 3);
                    X = TdUtility.ConvertDoubleAndInt(XResult?.Result?.ToString());
                    Y = TdUtility.ConvertDoubleAndInt(YResult?.Result?.ToString());
                    browser.MouseClick(X + 5, Y + 5, delayAfter: 6);
                }
                var Success = _AutomationExtension.ExecuteScript(
                    "document.querySelector('button[data-testid=\"tweetButton\"]').click();",
                    5, browser).Success;
                if(!Success)
                    Success = _AutomationExtension.ExecuteScript(
                    "document.querySelector('button[data-testid=\"tweetButtonInline\"]').click();",
                    5, browser).Success;
                var response = browser.GetPaginationData("{\"data\":{\"create_tweet\":{\"tweet_results\":{\"result\":{\"rest_id\"", true).Result;
                responseHandler = new CommentResponseHandler(new ResponseParameter
                    {Response = response});
                return responseHandler;
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
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
            var isSuccess = false;
            tweetId = tweetId.Trim();
            string reqUrl;
            if (twitterAccount.UserName.Contains("@"))
                reqUrl = $"{TdConstants.MainUrl}{twitterAccount.AccountBaseModel.ProfileId}/status/{tweetId}";
            else
                reqUrl = $"{TdConstants.MainUrl}{twitterAccount.UserName}/status/{tweetId}";
            _AutomationExtension.LoadAndScroll(reqUrl, 15);
            var PageResponse = BrowserWindow.GetPageSource();
            if (PageResponse.Contains("Sorry, that page doesn’t exist!"))
                return new DeleteResponseHandler(new ResponseParameter {Response = isSuccess ? PageResponse : ""});

            if (ActivityType == ActivityType.DeleteComment)
            {
                if (PageResponse.Contains("This Tweet is unavailable"))
                    _AutomationExtension.ExecuteScript(
                        "document.getElementsByClassName('css-1dbjc4n r-xoduu5')[1].click()", 6);
                else
                    _AutomationExtension.ExecuteScript(
                        "document.getElementsByClassName('css-1dbjc4n r-sdzlij r-1p0dtai r-xoduu5 r-1d2f490 r-podbf7 r-u8s1d r-zchlnj r-ipm5af r-o7ynqc r-6416eg')[1].click()",
                        6);
                _AutomationExtension.ExecuteScript(
                    "document.getElementsByClassName('css-1dbjc4n r-1loqt21 r-18u37iz r-1j3t67a r-9qu9m4 r-o7ynqc r-1j63xyz r-13qz1uu')[0].click()",
                    5);
                isSuccess = _AutomationExtension
                    .ExecuteScript(
                        "document.getElementsByClassName('css-901oao r-1awozwy r-jwli3a r-6koalj r-18u37iz r-16y2uox r-1qd0xha r-a023e6 r-vw2c0b r-1777fci r-eljoum r-dnmrzs r-bcqeeo r-q4m81j r-qvutc0')[0].click()",
                        5).Success;
            }
            else
            {
                _AutomationExtension.ExecuteScript(
                    "document.getElementsByClassName('css-1dbjc4n r-sdzlij r-1p0dtai r-xoduu5 r-1d2f490 r-podbf7 r-u8s1d r-zchlnj r-ipm5af r-o7ynqc r-6416eg')[0].click()",
                    8);
                _AutomationExtension.ExecuteScript(AttributeIdentifierType.Xpath, "//span[text()='Delete']");
                isSuccess = _AutomationExtension
                    .ExecuteScript(
                        "document.getElementsByClassName('css-901oao r-1awozwy r-jwli3a r-6koalj r-18u37iz r-16y2uox r-1qd0xha r-a023e6 r-vw2c0b r-1777fci r-eljoum r-dnmrzs r-bcqeeo r-q4m81j r-qvutc0')[0].click()",
                        5).Success;
            }

            return new DeleteResponseHandler(new ResponseParameter
                {Response = isSuccess ? BrowserWindow.GetPageSource() : ""});
        }

        public RetweetResponseHandler Retweet(DominatorAccountModel twitterAccount, string tweetId, string userName, string queryType = "")
        {
            try
            {
                var browser = CheckandAssignBrowser(twitterAccount, queryType, IsSave: true);
                var url = TdUtility.GetTweetUrl(userName, tweetId);
                var automation = new BrowserAutomationExtension(browser, twitterAccount.Token);
                _delayService.ThreadSleep(10000);
                automation.LoadAndScroll($"{TdConstants.MainUrl}", 15);
                automation.ExecuteScript("document.querySelector('a[href=\"/compose/post\"]').click();", 3);
                var InboxScript = "document.querySelector('div[role=\"textbox\"]').getBoundingClientRect().{0};";
                var XCord = automation.ExecuteScript(string.Format(InboxScript, "x"), 2);
                var YCord = automation.ExecuteScript(string.Format(InboxScript, "y"), 2);
                var X = TdUtility.ConvertDoubleAndInt(XCord?.Result?.ToString());
                var Y = TdUtility.ConvertDoubleAndInt(YCord?.Result?.ToString());
                browser.MouseClick(X + 20, Y + 10, delayAfter: 2);
                browser.EnterChars(" "+ url, delayAtLast: 3, typingDelay: 0.05);
                browser.ClearResources();
                automation.ExecuteScript("document.querySelector('button[data-testid=\"tweetButton\"]').click();", 5);
                var response = browser.GetPaginationData("\"data\":{\"create_tweet\"", true).Result;
                return new RetweetResponseHandler(new ResponseParameter
                    {Response = response });
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

        public UndoRetweetResponseHandler UndoRetweet(DominatorAccountModel twitterAccount, string tweetId)
        {
            try
            {
                tweetId = tweetId.Trim();
                var reqUrl = string.Empty;
                if (twitterAccount.UserName.Contains("@"))
                    reqUrl = $"{TdConstants.MainUrl}{twitterAccount.AccountBaseModel.ProfileId}/status/{tweetId}";
                else
                    reqUrl = $"{TdConstants.MainUrl}{twitterAccount.UserName}/status/{tweetId}";

                _AutomationExtension.LoadAndScroll(reqUrl, 15);
                _AutomationExtension.ExecuteScript(
                    "document.getElementsByClassName('css-1dbjc4n r-sdzlij r-1p0dtai r-xoduu5 r-1d2f490 r-xf4iuw r-u8s1d r-zchlnj r-ipm5af r-o7ynqc r-6416eg')[1].click()",
                    5);
                var isSuccess = _AutomationExtension
                    .ExecuteScript(
                        "document.getElementsByClassName('css-1dbjc4n r-1loqt21 r-18u37iz r-1j3t67a r-9qu9m4 r-o7ynqc r-1j63xyz r-13qz1uu')[0].click()",
                        5).Success;
                var responseHandler = new UndoRetweetResponseHandler(new ResponseParameter
                    {Response = isSuccess ? BrowserWindow.GetPageSource() : ""});
                return responseHandler;
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

        public UserDetailsResponseHandler GetUserDetails(DominatorAccountModel twitterAccount, string userName,
            string queryType,
            bool isScrapeAllTweets = false, string minPosition = null)
        {
            userName = userName.Trim();
            var key = string.Empty;
            var FinalResponseData = string.Empty;
            if (userName.Contains(TdConstants.MainUrl))
                userName = TdUtility.GetUserNameFromUrl(userName);

            // assigning secondary browser for scraper modules
            var browser = CheckandAssignBrowser(twitterAccount, queryType);
            var automation = new BrowserAutomationExtension(browser, _AutomationExtension.jobCancellationToken);

            string reqUrl;
            if (minPosition == null)
            {
                reqUrl = $"{TdConstants.MainUrl}{userName.Trim()}";
                automation.LoadAndScroll(reqUrl, 15, isScrapeAllTweets, 5000);
                key = twitterAccount.AccountBaseModel.UserName.Equals(userName.ToLower(),
                    StringComparison.OrdinalIgnoreCase)
                    ? TdConstants.GetTweets(twitterAccount.AccountBaseModel.UserId, "")
                    : userName.ToLower();
                var Responsedetail = new List<KeyValuePair<string, MemoryStreamResponseFilter>>();
                try
                {
                    Responsedetail = TdUtility.GetResponseData(browser);
                    FinalResponseData = TdUtility.GetResponseHandlerWithkeysContain(Responsedetail, key);

                    if (queryType == "Followers Of Someone's Followings Tweets" ||
                        queryType == "Followers Of Someone's Followers Tweets" ||
                        queryType == "Someone's Followers Tweets" ||
                        queryType == "Someone's Followings Tweets" ||
                        queryType == "Followers Of Someone's Followings Tweets" ||
                        queryType == "Liked User's Tweets" ||
                        queryType == "Commented User's Tweets"
                        || queryType == "Retweeted User's Tweets")
                    {
                        var userId = Utilities.GetBetween(FinalResponseData, "\"rest_id\":\"", "\"");
                        if (!string.IsNullOrEmpty(userId))
                            key = TdConstants.GetTweets(userId, "");
                        FinalResponseData = TdUtility.GetResponseHandlerWithkeysContain(Responsedetail, key);
                        if (string.IsNullOrEmpty(FinalResponseData))
                        {
                            key = TdConstants.GetMediaifnotGetResult(userId, "");
                            FinalResponseData = TdUtility.GetResponseHandlerWithkeysContain(Responsedetail, key);
                        }
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(FinalResponseData))
                        {
                            key = TdConstants.GetTweets(twitterAccount.AccountBaseModel.UserId, "");
                            FinalResponseData = TdUtility.GetResponseHandlerWithkeysContain(Responsedetail, key);
                        }
                    }

                }
                catch (Exception)
                {
                    Responsedetail?.ForEach(x => x.Value?.Dispose());
                    throw;
                }
                finally
                {
                    Responsedetail?.ForEach(x => x.Value?.Dispose());
                }
            }
            else
            {
                _AutomationExtension.ScrollWindow(5000);
            }

            var responseHandler = new UserDetailsResponseHandler(
                new ResponseParameter {Response = $"{userName}\"{FinalResponseData}"}
                , isScrapeAllTweets);
            if (string.IsNullOrEmpty(responseHandler.UserDetail.Username))
                responseHandler.UserDetail.Username = userName;
            return responseHandler;
        }

        public DirectMessageResponseHandler SendDirectMessage(DominatorAccountModel twitterAccount, string userId,
            string messageBody, string username, string filePath = null)
        {
            try
            {
                DirectMessageResponseHandler responseHandler = null;
                var isMessageConatinImages = false;
                var updateFactory = InstanceProvider.GetInstance<ITDAccountUpdateFactory>();
                var account = new AccountModel(twitterAccount);
                if (string.IsNullOrEmpty(twitterAccount.AccountBaseModel.UserId))
                    updateFactory.CheckStatusAsync(twitterAccount, new CancellationToken()).Wait();
                userId = userId.Trim();
                messageBody = messageBody.Trim();
                var timeStampId = (TdUtility.UnixTimestampFromDateTime(_dateProvider.UtcNow()) * 1000).ToString();
                var reqUrl = $"{TdConstants.MainUrl}{username}";
                _AutomationExtension.LoadAndScroll(reqUrl, 15);
                _AutomationExtension.ExecuteScript(
                    "document.getElementsByClassName('css-18t94o4 css-1dbjc4n r-1niwhzg r-p1n3y5 r-sdzlij r-1phboty r-rs99b7 r-1w2pmg r-15d164r r-zso239 r-1vuscfd r-53xb7h r-mk0yit r-o7ynqc r-6416eg r-lrvibr')[1].click()",
                    5);
                var getAxis = _AutomationExtension.GetXAndY(
                    "public-DraftStyleDefault-block public-DraftStyleDefault-ltr", AttributeIdentifierType.ClassName);
                BrowserWindow.MouseClick(getAxis.Key + 5, getAxis.Value + 5, delayAfter: 5);
                var messages = Regex.Split(messageBody, "\r\n");
                var i = 0;
                foreach (var message in messages)
                    if (i == 0)
                    {
                        BrowserWindow.EnterChars($"{message}", 0.1, delayAtLast: 0.3);
                        i++;
                    }
                    else
                    {
                        BrowserWindow.PressAnyKey(winKeyCode: 13, isShiftDown: true);
                        BrowserWindow.EnterChars($" {message} ", 0.1, delayAtLast: 0.3);
                    }

                _delayService.ThreadSleep(5000);
                // BrowserWindow.EnterChars(messageBody);

                #region If message contains image 

                if (!string.IsNullOrEmpty(filePath))
                {
                    responseHandler = SendImagesHttpsResponse(twitterAccount, userId, ref messageBody, filePath,
                        out responseHandler, account, timeStampId, out reqUrl);
                    isMessageConatinImages = true;

                    #region CommentedBrowsersendImagecode

                    //#region sendimagesbyBrowser
                    //var pageSource = BrowserWindow.GetPageSource();
                    //var getaxis = _AutomationExtension.GetXAndY("//div[@class='DMComposer-mediaPicker TweetBoxExtras-item']//input[@name='media_empty']", AttributeIdentifierType.Xpath);
                    //var fileDialogHandler = new TempFileDialogHandler(BrowserWindow, new List<string>() { filePath });
                    //BrowserWindow.Browser.DialogHandler = fileDialogHandler;
                    //BrowserWindow.MouseClick(getaxis.Key + 5, getaxis.Value + 5, delayAfter: 15); 
                    //#endregion 

                    #endregion
                }

                #endregion

                if (!isMessageConatinImages)
                {
                    var isSuccess = _AutomationExtension.ExecuteScript(
                        "document.getElementsByClassName('css-18t94o4 css-1dbjc4n r-1niwhzg r-42olwf r-sdzlij r-1phboty r-rs99b7 r-1w2pmg r-1f6r7vd r-1vuscfd r-53xb7h r-mk0yit r-o7ynqc r-6416eg r-lrvibr')[0].click()",
                        5).Success;
                    _delayService.ThreadSleep(15000);
                    responseHandler = new DirectMessageResponseHandler(new ResponseParameter
                        {Response = isSuccess ? BrowserWindow.GetPageSource() : ""});
                }

                return responseHandler;
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

        public TweetResponseHandler Tweet(DominatorAccountModel twitterAccount, string tweetBody,
            CancellationToken cancellationToken, string Id, string Username, string queryType,
            ActivityType ActivityType, bool isTweetContainedVideo = false, List<string> listFilepath = null,
            List<string> listUserNameToTag = null)
        {
            try
            {
                var isImageProcess = false;

                TweetResponseHandler responseHandler = null;
                tweetBody = tweetBody.Trim();
                //int charCount = tweetBody.Count(c => !Char.IsWhiteSpace(c));
                if (tweetBody.Length > 280)
                {
                    GlobusLogHelper.log.Info(string.Format(
                        "LangKeyCommentTextMoreThanSomeAndReduceToSomeCharacter".FromResourceDictionary(),
                        "LangKeyTweet".FromResourceDictionary(), 280, 280));
                    tweetBody = tweetBody.Substring(0, 280);
                }

                var browser = CheckandAssignBrowser(twitterAccount, queryType,IsSave:true);
                var automation = new BrowserAutomationExtension(browser, twitterAccount.Token);
                _delayService.ThreadSleep(10000);
                automation.LoadAndScroll($"{TdConstants.MainUrl}", 15);

                automation.ExecuteScript("document.querySelector('a[href=\"/compose/post\"]').click();", 3);
                var InboxScript = "document.querySelector('div[role=\"textbox\"]').getBoundingClientRect().{0};";
                var XCord = automation.ExecuteScript(string.Format(InboxScript, "x"), 2);
                var YCord = automation.ExecuteScript(string.Format(InboxScript, "y"), 2);
                var X = TdUtility.ConvertDoubleAndInt(XCord?.Result?.ToString());
                var Y = TdUtility.ConvertDoubleAndInt(YCord?.Result?.ToString());
                browser.MouseClick(X + 20, Y + 10,delayAfter:2);
                var Lines = tweetBody?.Replace("\r\n", "\n")?.Split('\n')?.ToList();
                foreach (var line in Lines)
                {
                    var hash = !string.IsNullOrEmpty(line) && (line.Contains("@") || line.Contains("#"));
                    if (!string.IsNullOrEmpty(line))
                        browser.EnterChars(hash ? line : " " + line, delayAtLast: 3,typingDelay:1);
                    if (line != Lines.LastOrDefault())
                        browser.PressAnyKey(winKeyCode: 13, delayAtLast: 2);
                }
                if (listFilepath != null && listFilepath.Count > 0)
                {
                    var script = "document.querySelector('button[aria-label=\"Add photos or video\"]').getBoundingClientRect().{0}";
                    browser.ChooseFileFromDialog(string.Empty, listFilepath);
                    var XResult = automation.ExecuteScript(string.Format(script, "x"), 3);
                    var YResult = automation.ExecuteScript(string.Format(script, "y"), 3);
                    X = TdUtility.ConvertDoubleAndInt(XResult?.Result?.ToString());
                    Y = TdUtility.ConvertDoubleAndInt(YResult?.Result?.ToString());
                    browser.MouseClick(X + 5, Y + 5, delayAfter: 6);
                }
                browser.ClearResources();
                automation.ExecuteScript("document.querySelector('button[data-testid=\"tweetButton\"]').click();", 5);
                var response = browser.GetPaginationData("\"data\":{\"create_tweet\"", true).Result;
                #region OLD Code to create tweet.
                //automation.ExecuteScript(
                //    "document.getElementsByClassName('css-901oao r-1awozwy r-jwli3a r-6koalj r-18u37iz r-16y2uox r-1qd0xha r-a023e6 r-vw2c0b r-1777fci r-eljoum r-dnmrzs r-bcqeeo r-q4m81j r-qvutc0')[0].click()",
                //    5, browser);
                //var response = false;
                //if (!isTweetContainedVideo)
                //    response = automation
                //        .ExecuteScript(
                //            $"document.getElementsByClassName('tweet-box rich-editor is-showPlaceholder')[1].InnerHtml='{tweetBody}';",
                //            5).Success;
                //if (!response)
                //    _delayService.ThreadSleep(1000);
                //browser.EnterChars(tweetBody);

                //#region if tweet contains image/video

                //cancellationToken.ThrowIfCancellationRequested();

                //if (listFilepath != null && listFilepath.Count > 0 || listUserNameToTag != null ||
                //    listUserNameToTag?.Count <= 0 || isTweetContainedVideo)
                //{
                //    var getaxis = automation.GetXAndY(
                //        "//body/div[@id='react-root']/div[@class='css-1dbjc4n r-13awgt0 r-12vffkv']/div[@class='css-1dbjc4n r-13awgt0 r-12vffkv']/div[@class='r-1d2f490 r-u8s1d r-zchlnj r-ipm5af r-184en5c']/div[@class='css-1dbjc4n r-aqfbo4 r-1d2f490 r-12vffkv r-1xcajam r-zchlnj']/div[@class='css-1dbjc4n r-1p0dtai r-1adg3ll r-1d2f490 r-bnwqim r-zchlnj r-ipm5af']/div[@class='r-1oszu61 r-1phboty r-1yadl64 r-1p0dtai r-deolkf r-1adg3ll r-eqz5dr r-1d2f490 r-crgep1 r-ifefl9 r-bcqeeo r-t60dpp r-bnwqim r-zchlnj r-ipm5af r-417010']/div[@class='css-1dbjc4n r-1pz39u2 r-16y2uox r-1wbh5a2']/div[@class='css-1dbjc4n r-1habvwh r-18u37iz r-1pi2tsx r-1777fci r-1xcajam r-ipm5af r-g6jmlv']/div[@class='css-1dbjc4n r-t23y2h r-1wbh5a2 r-rsyp9y r-1pjcn9w r-htvplk r-1udh08x r-1potc6q']/div[@class='css-1dbjc4n r-14lw9ot r-1f0042m r-16y2uox r-1wbh5a2']/div[@class='css-1dbjc4n r-16y2uox r-1wbh5a2 r-1jgb5lz r-1ye8kvj r-13qz1uu']/div[@class='css-1dbjc4n r-16y2uox r-1wbh5a2 r-1dqxon3']/div[@class='css-1dbjc4n r-156q2ks']/div/div[@class='css-1dbjc4n']/div[@class='css-1dbjc4n r-1j3t67a']/div[@class='css-1dbjc4n r-18u37iz']/div[@class='css-1dbjc4n r-1iusvr4 r-46vdb2 r-15d164r r-9cviqr r-bcqeeo r-1bylmt5 r-13tjlyg r-7qyjyx r-1ftll1t']/div/div[@class='css-1dbjc4n']/div[@class='css-1dbjc4n r-1awozwy r-14lw9ot r-18u37iz r-1w6e6rj r-1wtj0ep r-id7aif r-184en5c']/div[1]/div[1]/div[1]/*[1]",
                //        AttributeIdentifierType.Xpath);
                //    if (getaxis.Key == 0)
                //        automation.GetXAndY("//div[@id='Tweetstorm-tweet-box-0']//input[@name='media_empty']",
                //            AttributeIdentifierType.Xpath);
                //    var tdRequestParameter = SetHttprequestParameter(twitterAccount);
                //    tweetBody = HttpUtility.UrlEncode(tweetBody.Trim());
                //    var objJsonElements = new JsonElementsForPostReq
                //    {
                //        BatchMode = TdConstants.Off,
                //        Status = tweetBody,
                //        WeightedCharacterCount = TdConstants.True
                //    };
                //    if (isTweetContainedVideo)
                //    {
                //        #region Tweet with video   

                //        objJsonElements.MediaIds = UploadVideo(twitterAccount, listFilepath[0], cancellationToken);
                //        if (string.IsNullOrEmpty(objJsonElements.MediaIds))
                //            objJsonElements.MediaIds = UploadVideo(twitterAccount, listFilepath[0], cancellationToken);
                //        responseHandler = HttpUploadHandler(twitterAccount, objJsonElements, tdRequestParameter);
                //        if (!responseHandler.Success)
                //            responseHandler = HttpUploadHandler(twitterAccount, objJsonElements, tdRequestParameter);

                //        #endregion
                //    }
                //    else
                //    {
                //        var mediaId =
                //            _contentUploaderService.UploadMediaContent(twitterAccount, listFilepath.ToArray());
                //        if (string.IsNullOrEmpty(mediaId))
                //            mediaId = _contentUploaderService.UploadMediaContent(twitterAccount,
                //                listFilepath.ToArray());
                //        var mediaIdEncoded = HttpUtility.UrlEncode(mediaId);

                //        isImageProcess = true;

                //        #region TagUsers

                //        if (!isTweetContainedVideo && listUserNameToTag != null && listUserNameToTag.Count > 0)
                //        {
                //            var listUserId = new List<string>();
                //            foreach (var userName in listUserNameToTag)
                //            {
                //                listUserId.Add(GetUserIdFromUserName(twitterAccount, userName));
                //                _delayService.ThreadSleep(TdConstants.ConsecutiveGetReqInteval);
                //                if (listUserId.Count >= 10)
                //                    break; //As we can tag Atmax 10 users at a time So no need to give extra requests.
                //            }

                //            var mediaTag = TdUtility.GetMediaTagFormat(mediaId.Split(',')[0], listUserId);
                //            objJsonElements.MediaTags = mediaTag;

                //            #endregion
                //        }

                //        #endregion

                //        objJsonElements.MediaIds = mediaIdEncoded;
                //        responseHandler = HttpUploadHandler(twitterAccount, objJsonElements, tdRequestParameter);
                //        if (responseHandler.Success)
                //        {
                //            _delayService.ThreadSleep(10000);
                //            automation.LoadAndScroll($"{TdConstants.MainUrl}", 15);
                //        }
                //        else
                //        {
                //            responseHandler = HttpUploadHandler(twitterAccount, objJsonElements, tdRequestParameter);
                //        }
                //    }
                //}

                //var isSuccess = false;
                //if ((listUserNameToTag == null || listUserNameToTag.Count == 0) && isTweetContainedVideo == false &&
                //    !isImageProcess)
                //{
                //    isSuccess = automation
                //        .ExecuteScript(
                //            "document.getElementsByClassName('css-18t94o4 css-1dbjc4n r-urgr8i r-42olwf r-sdzlij r-1phboty r-rs99b7 r-1w2pmg r-1n0xq6e r-1vuscfd r-1dhvaqw r-1fneopy r-o7ynqc r-6416eg r-lrvibr')[0].click()",
                //            10).Success;

                //    responseHandler = new TweetResponseHandler(new ResponseParameter
                //    {
                //        Response = isSuccess
                //            ? TdUtility.GetResponseData(browser, $"https://api.{Domain}/1.1/statuses/update.json",
                //                true)
                //            : ""
                //    });
                //}
                #endregion
                responseHandler = new TweetResponseHandler(new ResponseParameter { Response = response });
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

        public MediaLikersResponseHandler GetUsersWhoLikedTweet(DominatorAccountModel twitterAccount, string TweetUrl)
        {
            var Responsedata = new List<KeyValuePair<string, MemoryStreamResponseFilter>>();

            try
            {
                TweetUrl = TweetUrl.Trim();
                var TweetId = Utilities.GetBetween($"{TweetUrl}/", "status/", "/");
                _AutomationExtension.LoadAndScroll(TweetUrl, 15);
                _AutomationExtension.ExecuteScript(
                    "document.getElementsByClassName('css-4rbku5 css-18t94o4 css-901oao r-hkyrab r-1loqt21 r-1qd0xha r-a023e6 r-16dba41 r-ad9z0x r-bcqeeo r-qvutc0')[1].click()",
                    5);
                var length = BrowserWindow.GetElementValue(ActType.GetLengthByClass,
                    "css-1dbjc4n r-1iusvr4 r-46vdb2 r-1777fci r-5f2r5o r-bcqeeo");
                for (var i = 10; i <= int.Parse(length); i += 10)
                {
                    _AutomationExtension.ExecuteScript(
                        $"document.getElementsByClassName('css-1dbjc4n r-1iusvr4 r-46vdb2 r-1777fci r-5f2r5o r-bcqeeo')[{i}].scrollIntoView();",
                        7);
                    length = BrowserWindow.GetElementValue(ActType.GetLengthByClass,
                        "css-1dbjc4n r-1iusvr4 r-46vdb2 r-1777fci r-5f2r5o r-bcqeeo");
                }

                Responsedata = TdUtility.GetResponseData(BrowserWindow);
                var key = TdConstants.GetUsertypesOfTweet(TweetId,true);
                var FinalResponse = TdUtility.GetResponseHandlerWithkeysContain(Responsedata, key);
                var responseHandler = new MediaLikersResponseHandler(new ResponseParameter {Response = FinalResponse});
                return responseHandler;
            }
            catch (OperationCanceledException)
            {
                Responsedata?.ForEach(x => x.Value?.Dispose());
                throw new OperationCanceledException();
            }
            catch (Exception)
            {
                Responsedata?.ForEach(x => x.Value?.Dispose());
                return null;
            }
            finally
            {
                Responsedata?.ForEach(x => x.Value?.Dispose());
            }
        }

        public MediaCommentsResponseHandler GetUsersWhoCommentedOnTweet(DominatorAccountModel twitterAccount,
            string TweetUrl, string MinPosition = null)
        {
            var Responsedata = new List<KeyValuePair<string, MemoryStreamResponseFilter>>();

            try
            {
                TweetUrl = TweetUrl.Trim();
                var TweetId = Utilities.GetBetween($"{TweetUrl}/", "status/", "/");
                var key = string.Empty;
                var FinalResponse = string.Empty;
                if (MinPosition == null)
                {
                    _AutomationExtension.LoadAndScroll(TweetUrl, 15, true, 5000);
                    Responsedata = TdUtility.GetResponseData(BrowserWindow);
                    key = TdConstants.GetUserCommentTweet(TweetId);
                    FinalResponse = TdUtility.GetResponseHandlerWithkeysContain(Responsedata, key);
                }
                else
                {
                    _AutomationExtension.ScrollWindow(5000);
                    Responsedata = TdUtility.GetResponseData(BrowserWindow);
                    key = TdConstants.GetUserCommentPaginationTweet(TweetId, MinPosition);
                    FinalResponse = TdUtility.GetResponseHandlerWithkeysContain(Responsedata, key);
                }

                var ResponseHandler =
                    new MediaCommentsResponseHandler(new ResponseParameter {Response = FinalResponse});
                return ResponseHandler;
            }
            catch (OperationCanceledException)
            {
                Responsedata?.ForEach(x => x.Value?.Dispose());
                throw new OperationCanceledException();
            }
            catch (Exception)
            {
                Responsedata?.ForEach(x => x.Value?.Dispose());
                return null;
            }
            finally
            {
                Responsedata?.ForEach(x => x.Value?.Dispose());
            }
        }

        public MediaRetweetsResponseHandler GetUsersWhoRetweetedTweet(DominatorAccountModel twitterAccount,
            string TweetUrl)
        {
            TweetUrl = TweetUrl.Trim();
            var TweetId = Utilities.GetBetween($"{TweetUrl}/", "status/", "/");
            var Responsedata = new List<KeyValuePair<string, MemoryStreamResponseFilter>>();

            try
            {
                _AutomationExtension.LoadAndScroll(TweetUrl, 15);
                _AutomationExtension.ExecuteScript(
                    "document.getElementsByClassName('css-4rbku5 css-18t94o4 css-901oao r-hkyrab r-1loqt21 r-1qd0xha r-a023e6 r-16dba41 r-ad9z0x r-bcqeeo r-qvutc0')[0].click()",
                    5);
                _delayService.ThreadSleep(15000);
                Responsedata = TdUtility.GetResponseData(BrowserWindow);
                var key = TdConstants.GetUsertypesOfTweet(TweetId, false);
                var FinalResponsedata = TdUtility.GetResponseHandlerWithkeysContain(Responsedata, key);
                var responseHandler =
                    new MediaRetweetsResponseHandler(new ResponseParameter {Response = FinalResponsedata});
                return responseHandler;
            }
            catch (OperationCanceledException)
            {
                Responsedata?.ForEach(x => x.Value?.Dispose());
                throw new OperationCanceledException();
            }
            catch (Exception)
            {
                Responsedata?.ForEach(x => x.Value?.Dispose());
                return null;
            }
            finally
            {
                Responsedata?.ForEach(x => x.Value?.Dispose());
            }
        }

        public async Task<UserFeedResponseHandler> GetTweetsFromUserFeedAsync(DominatorAccountModel twitterAccount,
            string UserName, CancellationToken CancellationToken, string MinPosition = null,
            ActivityType ActivityType = ActivityType.Tweet,bool IsTweetWithReply=true,int MaxTweetCount = 0)
        {
            try
            {
                var key = string.Empty;
                var ResponsRequest = string.Empty;
                UserName = UserName.Trim();
                var MaxIteration = TdConstants.MaxIteration;
                UserFeedResponseHandler ResponseHandler = null;
                var ReqUrl = string.Empty;
                while (MaxIteration > 0)

                {
                    var responseDetail = new List<KeyValuePair<string, MemoryStreamResponseFilter>>();
                    var responseData = new List<KeyValuePair<string, MemoryStreamResponseFilter>>();
                    try
                    {
                        if (ActivityType == ActivityType.Tweet)
                        {
                            if (MinPosition == null)
                            {
                                ReqUrl = TdConstants.MainUrl + UserName + "/with_replies";
                                _AutomationExtension.LoadAndScroll(ReqUrl, 15, true, 5000);
                                key = UserName.ToLower();
                                responseDetail = TdUtility.GetResponseData(_AutomationExtension._browserWindow);
                                TdUtility.GetResponseHandlerWithkeysContain(responseDetail, key);
                                ResponsRequest = TdUtility.GetResponse(responseDetail, key, UserName);
                                var userId = string.Empty;
                                if (string.IsNullOrEmpty(userId =
                                    Utilities.GetBetween(ResponsRequest, "\"rest_id\":\"", "\"")))
                                    userId = Utilities.GetBetween(ResponsRequest, "\"user_id\":\"", "\"");
                                key = TdConstants.GetTweetsSecond(userId, "");
                                responseData = TdUtility.GetResponseData(_AutomationExtension._browserWindow);
                                ResponsRequest = TdUtility.GetResponseHandlerWithkeysContain(responseData, key);
                            }
                            else
                            {
                                _AutomationExtension.ScrollWindow(5000);
                                key = TdConstants.GetTweetsWithCursor(twitterAccount.AccountBaseModel.UserId, MinPosition);
                                responseDetail = TdUtility.GetResponseData(_AutomationExtension._browserWindow);
                                ResponsRequest = TdUtility.GetResponseHandlerWithkeysContain(responseDetail, key);
                            }
                        }
                        else if (ActivityType == ActivityType.TweetScraper)
                        {
                            if (MinPosition == null)
                            {
                                ReqUrl = $"{TdConstants.MainUrl + UserName}/likes"; //+ "/with_replies";
                                _AutomationExtension.LoadAndScroll(ReqUrl, 15, true, 5000);
                                key = UserName.ToLower();
                                responseDetail = TdUtility.GetResponseData(_AutomationExtension._browserWindow);
                                ResponsRequest = TdUtility.GetResponseHandlerWithkeysContain(responseDetail, key);
                                if (string.IsNullOrEmpty(ResponsRequest))
                                {
                                    key = UserName;
                                    ResponsRequest = TdUtility.GetResponseHandlerWithkeysContain(responseDetail, key);
                                }

                                var userId = string.Empty;

                                if (string.IsNullOrEmpty(userId =
                                    Utilities.GetBetween(ResponsRequest, "\"rest_id\":\"", "\"")))
                                    userId = Utilities.GetBetween(ResponsRequest, "\"user_id\":\"", "\"");
                                key = TdConstants.GetUserlikedTweet(userId);
                                ResponsRequest = TdUtility.GetResponseHandlerWithkeysContain(responseDetail, key);
                            }
                            else
                            {
                                _AutomationExtension.ScrollWindow(5000);
                            }
                        }


                        ResponseHandler =
                            new UserFeedResponseHandler(new ResponseParameter { Response = ResponsRequest });
                        if (ResponseHandler.Success) break;

                    }
                    catch (Exception)
                    {
                        responseDetail?.ForEach(x => x.Value?.Dispose());
                        responseData?.ForEach(x => x.Value?.Dispose());
                        throw;
                    }
                    finally
                    {
                        responseData?.ForEach(x => x.Value?.Dispose());
                        responseDetail?.ForEach(x => x.Value?.Dispose());
                    }
                    

                    _delayService.ThreadSleep(TdConstants.FloodWait);
                    MaxIteration--;
                }

                return ResponseHandler;
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
                return null;
            }
        }

        //private List<KeyValuePair<string, MemoryStreamResponseFilter>> GetResponseData()
        //{
        //    List<KeyValuePair<string, MemoryStreamResponseFilter>> Responsedata;
        //    do
        //    {
        //        Responsedata = BrowserWindow.TwitterJsonResponse();
        //        _delayService.ThreadSleep(2000);
        //    } while (Responsedata == null);
        //    return Responsedata;
        //}

        public TrackNewMessagesResponseHandler getNewMessages(DominatorAccountModel twitterAccount,
            string MinPosition = null, bool isliveChat = false)
        {
            TrackNewMessagesResponseHandler ResponseHandler = null;
            var RequestUrl = string.Empty;
            var FinalresponseData = string.Empty;

            if (MinPosition == null)
            {
                RequestUrl = TdConstants.MainUrl + "messages";
                _AutomationExtension.LoadAndScroll(RequestUrl, 20);

                var responseData = new List<KeyValuePair<string, MemoryStreamResponseFilter>>();

                try
                {
                    responseData = TdUtility.GetResponseData(BrowserWindow);
                    var key = TdConstants.GetInboxmessages();
                    FinalresponseData = TdUtility.GetResponseHandlerWithkeysContain(responseData, key);

                    var length = BrowserWindow.GetElementValue(ActType.GetLengthByClass, "DMInboxItem-title account-group");

                    for (var i = 10; i <= int.Parse(length); i += 10)
                    {
                        _AutomationExtension.ExecuteScript(
                            $"document.getElementsByClassName('DMInboxItem-title account-group')[{i}].scrollIntoView();",
                            7);
                        length = BrowserWindow.GetElementValue(ActType.GetLengthByClass, "DMInboxItem-title account-group");
                    }
                }
                catch (Exception)
                {
                    responseData?.ForEach(x => x.Value?.Dispose());
                    throw;
                }
                finally
                {
                    responseData?.ForEach(x => x.Value?.Dispose());
                }

            }
            else
            {
                _AutomationExtension.ScrollWindow(5000);
            }

            ResponseHandler = new TrackNewMessagesResponseHandler(
                new ResponseParameter {Response = $"{twitterAccount.AccountBaseModel.UserId}\"{FinalresponseData}"},
                MinPosition != null, isliveChat);

            return ResponseHandler;
        }

        public List<TwitterUser> GetNewFollowedUserFromNotification(DominatorAccountModel twitterAccount,
            IDbInsertionHelper dbInsertionHelper)
        {
            var listNewFollowedUsers = new List<TwitterUser>();

            var NotificationbrowserDataResponse = new List<KeyValuePair<string, MemoryStreamResponseFilter>>();
            try
            {
                #region initializing value

                var objDbInsertion = dbInsertionHelper;
                var requestUrl = $"https://{Domain}/i/notifications";
                _AutomationExtension.LoadAndScroll(requestUrl, 15, true, 5000);

                var NotificationDataResponse = BrowserWindow.GetPageSource();

                NotificationbrowserDataResponse = TdUtility.GetResponseData(BrowserWindow);
                var Keys =
                    $"https://api.{Domain}/2/notifications/all.json?include_profile_interstitial_type=1&include_blocking=1&include_blocked_by=1&include_followed_by=1&include_want_retweets=1&include_mute_edge=1&include_can_dm=1&include_can_media_tag=1&skip_status=1&cards_platform=Web-12&include_cards=1&include_composer_source=true&include_ext_alt_text=true&include_reply_count=1&tweet_mode=extended&include_entities=true&include_user_entities=true&include_ext_media_color=true&include_ext_media_availability=true&send_error_codes=true&count=20";
                var htmlResponse = "";
                NotificationDataResponse =
                    TdUtility.GetResponseHandlerWithkeysContain(NotificationbrowserDataResponse, Keys);

                #endregion

                if (NotificationDataResponse.Contains("<!DOCTYPE html>"))
                {
                    htmlResponse = NotificationDataResponse;
                }
                else
                {
                    BrowserNotificationResponse(listNewFollowedUsers, objDbInsertion, NotificationDataResponse);
                    var jsonObject = new Jsonhandler(NotificationDataResponse);
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
                        var matches = Regex.Matches(tempData,
                            "js-profile-popup-actionable js-tooltip(.*?)href=(.*?)data-user-id=(.*?)original-title=.(.*?)\"");
                        foreach (Match match in matches)
                        {
                            var twtUser = new TwitterUser
                            {
                                Username = SpecialCharRemover(match.Groups[2].ToString()),
                                UserId = SpecialCharRemover(match.Groups[3].ToString()),
                                FullName = SpecialCharRemover(match.Groups[4].ToString()),
                                JoiningDate = DateTime.Now
                            };

                            objDbInsertion.AddFriendshipData(twtUser, FollowType.Followers, 0);

                            listNewFollowedUsers.Add(twtUser);
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                NotificationbrowserDataResponse?.ForEach(x => x.Value?.Dispose());
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                NotificationbrowserDataResponse?.ForEach(x => x.Value?.Dispose());
                ex.DebugLog();
            }
            finally
            {
                NotificationbrowserDataResponse?.ForEach(x => x.Value?.Dispose());
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
                var tdRequestParameter = (TdRequestParameters) _httpHelper.GetRequestParameter();
                tdRequestParameter.SetupHeaders(Path: ReqUrl,Method:"POST", GuestID: GuestID(twitterAccount));
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
                        "LangKeyPasswordChangeRequired".FromResourceDictionary());
                else if (ResponseParameter.Response.Contains(
                    "class=\"Form-message is-errored\">Incorrect. Please try again.<"))
                    GlobusLogHelper.log.Info(Log.UploadingMediaFailedReason,
                        twitterAccount.AccountBaseModel.AccountNetwork, twitterAccount.UserName,
                        "LangKeyIncorrectTryAgain".FromResourceDictionary());
                else
                    return true;
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

        public bool ReTypeEmail(DominatorAccountModel twitterAccount, ChallengeDetails challengeDetails)
        {
            try
            {
                var ReqUrl = TdConstants.MainUrl + "account/login_challenge";
                var tdRequestParameter = (TdRequestParameters) _httpHelper.GetRequestParameter();
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
                        twitterAccount.AccountBaseModel.UserName,
                        "LangKeyPasswordChangeRequired".FromResourceDictionary());
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
            ChallengeDetails challengeDetails, string uri_Matrix = null,bool IsFunCaptcha=false)
        {
            var responseData = "";
            try
            {
                if(IsFunCaptcha)
                {

                    return responseData;
                }
                var accessUrl = $"https://{Domain}/account/access";
                var googleSiteKey = "6Lc5hC4UAAAAAEx-pIfqjpmg-_-1dLnDwIZ8RToe";


                #region getting  creds

                var genericFileManager = InstanceProvider.GetInstance<IGenericFileManager>();
                var imageCaptchaServicesModel =
                    genericFileManager.GetModel<ImageCaptchaServicesModel>(
                        ConstantVariable.GetImageCaptchaServicesFile()) ?? new ImageCaptchaServicesModel();

                if (string.IsNullOrEmpty(imageCaptchaServicesModel.UserName) ||
                    string.IsNullOrEmpty(imageCaptchaServicesModel.Password))
                {
                    GlobusLogHelper.log.Info(Log.LoginFailed, twitterAccountModel.AccountBaseModel.AccountNetwork,
                        twitterAccountModel.AccountBaseModel.UserName,
                        "LangKeyNotFoundValidUserNamePassworImageTyperz".FromResourceDictionary());
                    return "no userName password found.";
                }

                #endregion

                // if already redirected to captcha page no need to do this

                #region redirecting from start to captcha

                if (!response_Login.Response.Contains("recaptcha_element"))
                {
                    var reqParams = _httpHelper.GetRequestParameter();
                    reqParams.Referer = accessUrl;
                    var uriMatrix = Uri.EscapeDataString(twitterAccountModel.ExtraParameters["UriMatrix"]);

                    var recaptchaPostData =
                        $"authenticity_token={challengeDetails.PostAuthenticityToken}&assignment_token={challengeDetails.AssignmentToken}&lang=en-gb&flow=&ui_metrics={uriMatrix}";
                    _httpHelper.PostRequest(accessUrl, recaptchaPostData);
                }

                #endregion

                var getCaptchaIdUrl =
                    "http://captchatypers.com/captchaapi/UploadRecaptchav1.ashx?action=UPLOADCAPTCHA&username=" +
                    imageCaptchaServicesModel.UserName?.Trim() + "&password=" +
                    imageCaptchaServicesModel.Password?.Trim()
                    + "&googlekey=" + googleSiteKey + "&pageurl=" + accessUrl;

                var solvedCaptchaResponse = string.Empty;
                var response = _httpHelper.GetRequest(getCaptchaIdUrl);
                var captchaIdGotFromImageTyperz = response.Response;
                if (response.Response.Contains("ERROR: AUTHENTICATION_FAILED"))
                {
                    GlobusLogHelper.log.Info(Log.LoginFailed, twitterAccountModel.AccountBaseModel.AccountNetwork,
                        twitterAccountModel.AccountBaseModel.UserName, "Invalid userName and password of ImageTyperz.");
                    return response.Response;
                }


                if (!string.IsNullOrEmpty(captchaIdGotFromImageTyperz))
                {
                    var getcamptchaesultFirstUrl2 =
                        "http://captchatypers.com/captchaapi/GetRecaptchaText.ashx?action=GETTEXT&username=" +
                        imageCaptchaServicesModel.UserName + "&password=" + imageCaptchaServicesModel.Password +
                        "&Captchaid=" + captchaIdGotFromImageTyperz;

                    // running while not solved captcha
                    while (string.IsNullOrEmpty(solvedCaptchaResponse) ||
                           solvedCaptchaResponse.Contains("ERROR: NOT_DECODED"))
                        try
                        {
                            solvedCaptchaResponse = _httpHelper.GetRequest(getcamptchaesultFirstUrl2).Response;
                            if (solvedCaptchaResponse.Contains("IMAGE_TIMED_OUT"))
                            {
                                GlobusLogHelper.log.Info("LangKeyTimeOut".FromResourceDictionary());
                                break;
                            }

                            _delayService.ThreadSleep(5 * 1000);
                        }
                        catch (Exception Ex)
                        {
                            if (Ex.ToString().Contains("IMAGE_TIMED_OUT"))
                            {
                                GlobusLogHelper.log.Info("LangKeyExceptionImageTimeOutWhileInProgress"
                                    .FromResourceDictionary());
                                break;
                            }
                        }
                }
                else
                {
                    GlobusLogHelper.log.Info(Log.LoginFailed, twitterAccountModel.AccountBaseModel.AccountNetwork,
                        twitterAccountModel.AccountBaseModel.UserName,
                        Application.Current.FindResource("LangKeyFailed") +
                        "LangKeyCouldNotGetCaptchaId".FromResourceDictionary());
                    return "could not get captchaIdGotFromImageTyperz";
                }

                // post hit after successfully solving captcha redirect to main page
                var postData_GoToTwitter = "authenticity_token=" + challengeDetails.PostAuthenticityToken +
                                           "&assignment_token=" + challengeDetails.AssignmentToken +
                                           "&lang=ru&g-recaptcha-response=" + solvedCaptchaResponse +
                                           "&verification_string=" + solvedCaptchaResponse;
                responseData = _httpHelper.PostRequest(accessUrl, postData_GoToTwitter).Response;
            }
            catch (Exception e)
            {
                e.DebugLog();
                return "";
            }

            return responseData;
        }

        #endregion


        public void SetBrowser(DominatorAccountModel dominatorAccountModel, CancellationToken cancellationToken,
            BrowserInstanceType browserInstanceType = BrowserInstanceType.Primary)
        {
            var name = TdAccountsBrowserDetails.GetBrowserName(dominatorAccountModel, browserInstanceType);
            BrowserWindow = TdAccountsBrowserDetails.GetInstance().AccountBrowserCollections[name];
            _AutomationExtension = new BrowserAutomationExtension(BrowserWindow, cancellationToken);
        }

        public bool BrowserLogin(DominatorAccountModel twitterModel, CancellationToken cancellationToken,
            LoginType loginType = LoginType.AutomationLogin, VerificationType verificationType = VerificationType.Email)
        {
            twitterAccountSession.AddOrUpdateSession(ref twitterModel);
            _token = cancellationToken;
            TdAccountsBrowserDetails.GetInstance()
                .CreateBrowser(twitterModel, _token, twitterAccountSession, BrowserInstanceType.BrowserLogin);
            return true;
        }

        private BrowserWindow CheckandAssignBrowser(DominatorAccountModel twitterAccount, string queryType = "",bool IsSave=false)
        {
            var browserqueryTypes = new List<string>();
            //"LangKeyCustomTweetLists" non Browser Query
            var nonBrowserqueryTypes = new List<string>
            {
                "LangKeyUsersWhoLikedTweet", "LangKeySocinatorUserScraperCampaign", "LangKeyUsersWhoRetweetedTweet",
                "LangKeyCustomUsersList", "Publish"
            };

            Enum.GetValues(typeof(TdUserInteractionQueryEnum)).Cast<TdUserInteractionQueryEnum>().ToList().ForEach(
                query =>
                {
                    if (nonBrowserqueryTypes.Contains(query.GetDescriptionAttr()))
                        return;
                    browserqueryTypes.Add(Application.Current.FindResource(query.GetDescriptionAttr()).ToString());
                });


            Enum.GetValues(typeof(TdTweetInteractionQueryEnum)).Cast<TdTweetInteractionQueryEnum>().ToList().ForEach(
                query =>
                {
                    if (nonBrowserqueryTypes.Contains(query.GetDescriptionAttr()))
                        return;
                    browserqueryTypes.Add(Application.Current.FindResource(query.GetDescriptionAttr()).ToString());
                });

            BrowserWindow browser = null;

            if (browserqueryTypes.Contains(queryType) || string.IsNullOrEmpty(queryType) || BrowserWindow is null)
            {
                AssignSecondaryBrowser(twitterAccount,IsSave);
                browser = SecondaryBrowserWindow;
            }
            else
            {
                browser = BrowserWindow;
            }

            return browser;
        }

        private void AssignSecondaryBrowser(DominatorAccountModel twitterAccount, bool IsSave = false)
        {
            if (SecondaryBrowserWindow == null || SecondaryBrowserWindow.IsDisposed)
            {
                try
                {
                    if(SecondaryBrowserWindow != null && SecondaryBrowserWindow.IsDisposed)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            SecondaryBrowserWindow.Close();
                        });
                    }
                }
                catch { }
                twitterAccountSession.AddOrUpdateSession(ref twitterAccount);
                TdAccountsBrowserDetails.GetInstance()
                    .CreateBrowser(twitterAccount, _token, twitterAccountSession, BrowserInstanceType.Secondary,isSave: IsSave,ShowLogin:false);
                _delayService.ThreadSleep(5000);
                SecondaryBrowserWindow = TdAccountsBrowserDetails.GetInstance()
                    .AccountBrowserCollections[$"{twitterAccount.UserName}{BrowserInstanceType.Secondary}"];
                // TwitterFunctions.BrowserLogin(twitterAccount);
            }
        }

        private DirectMessageResponseHandler SendImagesHttpsResponse(DominatorAccountModel twitterAccount,
            string userId, ref string messageBody, string filePath, out DirectMessageResponseHandler responseHandler,
            AccountModel account, string timeStampId, out string reqUrl)
        {
            var tdRequestParameter = (TdRequestParameters) _httpHelper.GetRequestParameter();
            if (tdRequestParameter.Cookies == null || tdRequestParameter.Cookies.Count <= 5)
                tdRequestParameter.Cookies = twitterAccount.Cookies;
            var mediaId = _contentUploaderService.UploadMediaContent(twitterAccount, filePath);
            mediaId = HttpUtility.UrlEncode(mediaId);

            var objMessageId = new MessageId();
            messageBody = HttpUtility.UrlEncode(messageBody.Trim());

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
            var tempSplit = filePath.Split('.');
            var extension = tempSplit[tempSplit.Length - 1];
            objJsonElements.ResendId = ++objMessageId.ResendId;
            objJsonElements.MediaData_FileId = objMessageId.MediaDataField;
            objJsonElements.MediaData_FileType = "image";
            objJsonElements.MediaData_MediaCategory = "dm_image";
            objJsonElements.MediaData_UploadId = ++objMessageId.MediaUploadId;
            objJsonElements.MediaData_MediaType = "image%2F" + extension;
            objJsonElements.MediaId = mediaId;
            var postData = tdRequestParameter.GeneratePostBody(objJsonElements);
            reqUrl = TdConstants.MainUrl + "i/direct_messages/new";
            tdRequestParameter.SetupHeaders("XML",Path:reqUrl,Method:"POST", GuestID: GuestID(twitterAccount));
            responseHandler = new DirectMessageResponseHandler(_httpHelper.PostRequest(reqUrl, postData));
            return responseHandler;
        }

        private TdRequestParameters SetHttprequestParameter(DominatorAccountModel twitterAccount)
        {
            var tdRequestParameter = (TdRequestParameters) _httpHelper.GetRequestParameter();
            tdRequestParameter.Cookies = twitterAccount.Cookies;
            return tdRequestParameter;
        }

        private TweetResponseHandler HttpUploadHandler(DominatorAccountModel twitterAccount,
            JsonElementsForPostReq objJsonElements, TdRequestParameters tdRequestParameter)
        {
            TweetResponseHandler responseHandler;

            var postData = tdRequestParameter.GeneratePostBody(objJsonElements);
            var reqUrls = TdConstants.ApiUrl + "statuses/update.json";
            SetCsrfToken(ref tdRequestParameter,true, Path: reqUrls, Method: "POST",GuestID:GuestID(twitterAccount));
            responseHandler = new TweetResponseHandler(_httpHelper.PostRequest(reqUrls, postData));
            return responseHandler;
        }

        public string GetUserIdFromUserName(DominatorAccountModel twitterAccount, string UserName)
        {
            var UserId = string.Empty;
            try
            {
                UserName = UserName.Trim();
                var ReqUrl = TdConstants.MainUrl + UserName;
                var Response = _httpHelper.GetRequest(ReqUrl).Response;
                UserId = Utilities.GetBetween(Response, "role=\"navigation\" data-user-id=\"", "\"");
                if (string.IsNullOrEmpty(UserId))
                    UserId = HtmlAgilityHelper.getValueWithAttributeNameFromInnerHtml(Response,
                        "ProfileNav-item--userActions", "data-user-id");
            }
            catch (Exception)
            {
            }

            return UserId;
        }

        //public LiveChatUsermessagesResponseHandler UserLiveChatMessage(DominatorAccountModel twitterAccount, SenderDetails senderdetail)
        //{
        //    var user = Regex.Split(senderdetail.ThreadId, "-");
        //    var UserId = user[0];
        //    string senderId = string.Empty;
        //    LiveChatUsermessagesResponseHandler ResponseHandler = null;
        //    var RequestUrl = string.Empty;
        //    var tdRequestParameter = (TdRequestParameters)_httpHelper.GetRequestParameter();
        //    RequestUrl = $"{TdConstants.MainUrl}messages/with/conversation?id={senderdetail.ThreadId}";        //https://{Domain}/messages/with/conversation?id=935056001841037312-1121680783826022400 
        //    tdRequestParameter.Referer = TdConstants.MainUrl;

        //    //else
        //    //{
        //    //    RequestUrl = TdConstants.MainUrl + "inbox/paginate?is_trusted=true&max_entry_id=" + MinPosition;
        //    //    tdRequestParameter.Referer = TdConstants.MainUrl + "i/notifications";
        //    //}

        //    ResponseHandler = new LiveChatUsermessagesResponseHandler(_httpHelper.GetRequest(RequestUrl), UserId, senderId);
        //    return ResponseHandler;
        //}

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
            var hasProfilePic = false;
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
                // NotificationItem["message"]["text"].ToString();
                if (messagetext.Contains("followed you"))
                {
                    userId = Jssonhand.GetJTokenValue(Token, "message", "entities", 0, "ref", "user", "id");
                    foreach (var user in users)
                    {
                        var UserToken = user.First();
                        var userid = Jssonhand.GetJTokenValue(UserToken, "id");
                        if (userid == userId)
                        {
                            Username = Jssonhand.GetJTokenValue(UserToken, "screen_name");
                            Fullname = Jssonhand.GetJTokenValue(UserToken, "name");
                            hasProfilePic = Jssonhand.GetJTokenValue(UserToken, "profile_image_url_https") != null;


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

        private string UploadVideo(DominatorAccountModel twitterAccount, string videoPath,
            CancellationToken CancellationToken)
        {
            var Count = 0;
            var tdRequestParameter = (TdRequestParameters) _httpHelper.GetRequestParameter();
            var Extrareq = new ExtraRequests(_httpHelper);
            var MediaId = string.Empty;
            var fileLength = new FileInfo(videoPath).Length.ToString();


            var FirstReqUrl = $"https://upload.{Domain}/i/media/upload.json?command=INIT&total_bytes=" + fileLength +
                              "&media_type=video%2Fmp4&media_category=tweet_video";
            tdRequestParameter.SetupHeaders(Path:FirstReqUrl,Method:"POST", GuestID: GuestID(twitterAccount));
            var FirstPostResponse = _httpHelper.PostRequest(FirstReqUrl, "").Response ??
                                    _httpHelper.PostRequest(FirstReqUrl, "").Response;
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
                        int remaining = (int) input.Length, bytesRead = 0;
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
                _httpHelper
                    .PostRequest(
                        $"https://upload.{Domain}/i/media/upload.json?command=STATUS&media_id=" + nvc.Get("MediaID"),
                        "");
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
        public TweetResponseHandler QuoteTweets(DominatorAccountModel twitterAccount, string userName, string tweetId, string tweetBody)
        {
            return null;
        }

        public void SetHeaderForEditProfile(string referer, string csrfToken)
        {
        }

        #region Private User Accounts

        public AcceptPendingReqResponseHandler AcceptPendingRequest(DominatorAccountModel twitterAccount, string UserId)
        {
            try
            {
                var tdRequestParameter = (TdRequestParameters) _httpHelper.GetRequestParameter();
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
                var tdRequestParameter = (TdRequestParameters) _httpHelper.GetRequestParameter();
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
                    tdRequestParameter.SetupHeaders("XML",Path:RequestUrl,Method:"GET", GuestID: GuestID(twitterAccount));
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
                var account = new AccountModel(twitterAccount);
                var tdRequestParameter = (TdRequestParameters) _httpHelper.GetRequestParameter();
                var mediaId = _contentUploaderService.UploadMediaContent(twitterAccount, ImagePath);
                tdRequestParameter.Referer = TdConstants.MainUrl + twitterAccount.AccountBaseModel.ProfileId;

                UpdateProfilePicResponseHandler ResponseHandler = null;
                var updateProfileUrl = $"https://{Domain}/i/profiles/update_profile_image";
                var updateProfilePostData =
                    $"authenticity_token={account.postAuthenticityToken}&height=512&mediaId={mediaId}&offsetLeft=0&offsetTop=0&page_context=me&scribeElement=upload&section_context=profile&uploadType=avatar&width=512"; //$"authenticity_token={account.postAuthenticityToken}&mediaId={mediaId}&page_context=me&scribeElement=upload&section_context=profile&uploadType=header";

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
                var tdRequestParameter = (TdRequestParameters) _httpHelper.GetRequestParameter();
                var account = new AccountModel(twitterAccount);
                UpdateProfileScreenNameResponseHandler ResponseHandler = null;
                var PostUrl = $"https://{Domain}/settings/accounts/update";
                var UserDetails = GetUserProfileDetails(twitterAccount);
                var email = UserDetails.Email;
                if (string.IsNullOrEmpty(account.CsrfToken))
                    account.CsrfToken = twitterAccount.Cookies.OfType<Cookie>().SingleOrDefault(x => x.Name == "ct0")
                        .Value;

                var ReqParam = _httpHelper.GetRequestParameter();
                ReqParam.Referer = TdConstants.MainUrl + twitterAccount.AccountBaseModel.ProfileId;
                var changeScreenNamePostData =
                    $"_method=PUT&authenticity_token={account.CsrfToken}&orig_uname=narendra_globus&orig_email={WebUtility.UrlEncode(email)}&user%5Bscreen_name%5D={screenName}&user%5Bemail%5D={WebUtility.UrlEncode(email)}&user%5Blang%5D=en&user%5Btime_zone%5D=Pacific+Time+%28US+%26+Canada%29&user%5Bno_username_only_password_reset%5D=0&user%5Bcountry%5D=in&user%5Bautoplay_disabled%5D=0&user%5Bautoplay_disabled%5D=1&user%5Bpersonalize_timeline%5D=1&user%5Bpersonalize_timeline%5D=0&auth_password={twitterAccount.AccountBaseModel.Password}&secret-code=";
                tdRequestParameter.SetupHeaders(account.CsrfToken,Path:PostUrl,Method:"POST", GuestID: GuestID(twitterAccount));
                ReqParam.Headers = tdRequestParameter.Headers;
                _httpHelper.SetRequestParameter(ReqParam);
                tdRequestParameter.Referer = TdConstants.MainUrl + twitterAccount.AccountBaseModel.ProfileId;

                ResponseHandler =
                    new UpdateProfileScreenNameResponseHandler(
                        _httpHelper.PostRequest(PostUrl, changeScreenNamePostData));
                return ResponseHandler;
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
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
                var tdRequestParameter = (TdRequestParameters) _httpHelper.GetRequestParameter();
                var account = new AccountModel(twitterAccount);
                UpdateProfileContactNumberResponseHandler ResponseHandler = null;
                var UserDetails = GetUserProfileDetails(twitterAccount);
                if (string.IsNullOrEmpty(account.CsrfToken))
                    account.CsrfToken = twitterAccount.Cookies.OfType<Cookie>().SingleOrDefault(x => x.Name == "ct0")
                        .Value;

                var PostUrl = $"https://{Domain}/settings/add_phone";
                var DevicePageSource = _httpHelper.GetRequest($"https://{Domain}/settings/devices")
                    .Response;
                var changePhoneNumberPostData =
                    GetPhoneNumberPostData(DevicePageSource, contactNumber, UserDetails);
                var ReqParam = _httpHelper.GetRequestParameter();
                ReqParam.Referer = PostUrl;
                tdRequestParameter.SetupHeaders(account.CsrfToken,Path:PostUrl,Method:"POST", GuestID: GuestID(twitterAccount));
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
                        tdRequestParameter.SetupHeaders(account.CsrfToken, Path: PostUrl, Method: "POST", GuestID: GuestID(twitterAccount));
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
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
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
                var tdRequestParameter = (TdRequestParameters) _httpHelper.GetRequestParameter();
                var account = new AccountModel(twitterAccount);
                UpdateProfileFullNameResponseHandler ResponseHandler = null;
                var PostUrl = $"https://{Domain}/i/profiles/update ";
                var ReqParam = _httpHelper.GetRequestParameter();

                if (string.IsNullOrEmpty(account.CsrfToken))
                    account.CsrfToken = twitterAccount.Cookies.OfType<Cookie>().FirstOrDefault(x => x.Name == "ct0" && x.Domain == ".x.com")
                        .Value ?? twitterAccount.Cookies.OfType<Cookie>().FirstOrDefault(x => x.Name == "ct0")
                        .Value;

                var postData =
                    $"authenticity_token={account.CsrfToken}&page_context=me&section_context=profile&user%5Bname%5D={WebUtility.UrlEncode(fullName)}";
                tdRequestParameter.SetupHeaders("XML",Path:PostUrl,Method:"POST", GuestID: GuestID(twitterAccount));
                ReqParam.Headers = tdRequestParameter.Headers;
                ReqParam.Headers.Remove("X-Asset-Version"); // objWebHeaderCollection.Add("X-Asset-Version", "2f2f06");
                ReqParam.Referer = TdConstants.MainUrl + twitterAccount.AccountBaseModel.ProfileId;
                _httpHelper.SetRequestParameter(ReqParam);
                // tdRequestParameter.AddHeader("Upgrade-Insecure-Requests","1");

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
                var tdRequestParameter = (TdRequestParameters) _httpHelper.GetRequestParameter();
                var UserProfileDetails = GetUserProfileDetails(twitterAccount);
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
                var ResponseHandler = new UpdateProfileEmailResponseHandler(
                    _httpHelper.PostRequest(PostUrl, changeEmailPostData));
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

        public UpdateProfileBioResponseHandler UpdateProfileBiography(DominatorAccountModel twitterAccount,
            string Biography)
        {
            try
            {
                var tdRequestParameter = (TdRequestParameters) _httpHelper.GetRequestParameter();
                var account = new AccountModel(twitterAccount);
                UpdateProfileBioResponseHandler ResponseHandler = null;
                var bioData = WebUtility.UrlEncode(Biography);
                tdRequestParameter.Referer = TdConstants.MainUrl + twitterAccount.AccountBaseModel.ProfileId;
                var postData =
                    $"authenticity_token={account.postAuthenticityToken}&page_context=me&section_context=profile&user%5Bdescription%5D={bioData}";
                ResponseHandler = new UpdateProfileBioResponseHandler(
                    _httpHelper.PostRequest($"https://{Domain}/i/profiles/update", postData));
                // changePostResponse = twtUser.globusHttpHelper.postFormData(new Uri("https://{Domain}/i/profiles/update"), postData, "https://{Domain}/" + tempTwtUser.UserScreenName, "https://{Domain}");
                var changePostResponse = WebUtility.HtmlDecode(ResponseHandler.PageResponse);
                if (!changePostResponse.Contains(WebUtility.UrlDecode(bioData)))
                {
                    var len = Biography.Length;
                    if (len > 160)
                        GlobusLogHelper.log.Info(Log.ActivitySuccessful, twitterAccount.AccountBaseModel.AccountNetwork,
                            twitterAccount.UserName, "LangKeyEditTwitterProfile".FromResourceDictionary(),
                            string.Format("LangKeyExceededBioContentMaxLength".FromResourceDictionary(), 160));
                    else
                        GlobusLogHelper.log.Info(Log.ActivitySuccessful, twitterAccount.AccountBaseModel.AccountNetwork,
                            twitterAccount.UserName, "LangKeyEditTwitterProfile".FromResourceDictionary(),
                            "LangKeyUnableToChangeBioContent".FromResourceDictionary());
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
                var tdRequestParameter = (TdRequestParameters) _httpHelper.GetRequestParameter();
                var account = new AccountModel(twitterAccount);
                if (string.IsNullOrEmpty(account.CsrfToken))
                    account.CsrfToken = twitterAccount.Cookies.OfType<Cookie>().SingleOrDefault(x => x.Name == "ct0")
                        .Value;

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
                tdRequestParameter.SetupHeaders("Json", account.CsrfToken,Path:postUrl,Method:"POST", GuestID: GuestID(twitterAccount));
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
                var tdRequestParameter = (TdRequestParameters) _httpHelper.GetRequestParameter();
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
                var postData =
                    $"authenticity_token={UserProfileDetails.authenticityToken}&page_context=me&section_context=profile&user%5Burl%5D={WebUtility.UrlEncode(websiteUrl)}";
                var ReqUrl = $"https://{Domain}/i/profiles/update";
                tdRequestParameter.SetupHeaders("XML",Path:ReqUrl,Method:"POST", GuestID: GuestID(twitterAccount));
                tdRequestParameter.Headers.Remove("X-Asset-Version");
                reqParams.Headers = tdRequestParameter.Headers;
                reqParams.Referer = TdConstants.MainUrl + twitterAccount.AccountBaseModel.ProfileId;
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

        public MuteResponseHandler TurnOnUserNotifications(DominatorAccountModel twitterAccount, string UserId, string UserName)
        {
            throw new NotImplementedException();
        }

        public List<TagDetails> GetTweetListFromNotification(DominatorAccountModel twitterAccount)
        {
            throw new NotImplementedException();
        }

        public bool GettingTweetMedia(DominatorAccountModel dominatorAccount, TagDetails tagDetails, ref List<string> tweetMedia, ActivityType activityType, string DownloadPath = "")
        {
            return false;
        }

        public Task<ProfileDetailsResponseHandler> GetProfileDetails(DominatorAccountModel dominatorAccount, string UserName)
        {
            throw new NotImplementedException();
        }

        public void SetCsrfToken(ref TdRequestParameters tdRequestParameters, bool isJsonRequest = false, SearchType type = SearchType.None, string Response = "", string Path = "",string Method="", string GuestID = "")
        {
            var CsrfToken = tdRequestParameters.Cookies.OfType<Cookie>().FirstOrDefault(x => x.Name == "ct0" && x.Domain == ".x.com")?.Value
                ?? tdRequestParameters.Cookies.OfType<Cookie>().FirstOrDefault(x => x.Name == "ct0")?.Value;
            tdRequestParameters.Cookies.Add(new Cookie { Name = "dnt", Value = "1", Domain = "x.com" });
            tdRequestParameters.SetupHeaders("Json", CsrfToken,IsJsonRequest: isJsonRequest,type,Response,Path, Method,GuestID);
        }

        public async Task<bool> SolveFunCaptcha(DominatorAccountModel dominatorAccount, ChallengeDetails challengeDetails)
        {
            return false;
        }

        public async Task<string> GetUserID(DominatorAccountModel dominatorAccount, string UserName)
        {
            return UserName;
        }

        public RepostTweetResponseHandler RepostTweet(DominatorAccountModel dominatorAccount, string TweetUrl)
        {
            return null;
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
            return null;
        }

        #endregion
    }

    public class TempFileDialogHandler : IDialogHandler
    {
        private readonly List<string> _filesPath = new List<string>();
        private MetroWindow _mainForm;

        public TempFileDialogHandler(MetroWindow form, List<string> filesPath)
        {
            _mainForm = form;
            _filesPath = filesPath;
        }

        //public bool OnFileDialog(IWebBrowser chromiumWebBrowser, IBrowser browser, CefFileDialogMode mode,
        //    CefFileDialogFlags flags, string title, string defaultFilePath, List<string> acceptFilters,
        //    int selectedAcceptFilter, IFileDialogCallback callback)
        //{
        //    callback.Continue(selectedAcceptFilter, _filesPath);
        //    return true;
        //}

        public bool OnFileDialog(IWebBrowser chromiumWebBrowser, IBrowser browser, CefFileDialogMode mode, string title, string defaultFilePath, List<string> acceptFilters, IFileDialogCallback callback)
        {
            callback.Continue(_filesPath);
            return true;
        }
    }
}