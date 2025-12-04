using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.Config;
using DominatorHouseCore.Models.Publisher;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Request;
using DominatorHouseCore.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RedditDominatorCore.RDLibrary.AdsReddit;
using RedditDominatorCore.RDModel;
using RedditDominatorCore.RDRequest;
using RedditDominatorCore.RDUtility;
using RedditDominatorCore.Response;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using ThreadUtils;
using Unity;
using static RedditDominatorCore.RDEnums.Enums;

namespace RedditDominatorCore.RDLibrary
{
    public interface IRedditFunction
    {
        Task StartScrapingAds(DominatorAccountModel accountModel, int ScrollCount);
        List<RedditUser> GetInteractedUsersLastMessage(List<string> userId, DominatorAccountModel account);
        void SaveUserDetailsInDb(DominatorAccountModel account, Dictionary<IpLocationDetails, string> ipDetails);
        string GetLatestPostUrlWithGateWay(PaginationParameter accountParameters, DominatorAccountModel accountModel);

        SubmitPageResponseHandler GetSubmitPage(DominatorAccountModel accountModel,
            PaginationParameter accountParameters);

        SubRedditSubmitValidationResponseHandler GetSubRedditSubmitValidation(DominatorAccountModel accountModel,
            PaginationParameter accountParameters, string groupUserName);

        PublishPostResponseHandler PublishPost(DominatorAccountModel accountModel, PublishPostModel redditPostDetails,
            PaginationParameter accountParameters, OtherConfigurationModel otherConfiguration);

        PublisherPostUploadResponseHandler GetUploadedMediaId(DominatorAccountModel accountModel,
            PaginationParameter accountParameters, string media, string kind);
        Dictionary<string, string> UploadImageAndGetMediaId(PublisherPostUploadResponseHandler response,
            PublisherPostlistModel postDetails, DominatorAccountModel accountModel, string kind,
            string thumbnailPath = "");

        void ApprovePublishPost(DominatorAccountModel account, IResponseParameter response,
            PublishPostModel postDetails, PaginationParameter accountParameters);

        string FetchTitleOfUrl(DominatorAccountModel account, PublishPostModel postDetails);

        RedditPostResponseHandler ScrapePostsByUrl(DominatorAccountModel account, string postLink, QueryInfo queryInfo,
            RedditPostResponseHandler redditPostResponseHandler, bool isPaginationRequire = false, string sortBy = "");

        PublishPostResponseHandler PublishPostCrossPost(DominatorAccountModel accountModel,
            PublishPostModel redditPostDetails, PaginationParameter accountParameters);

        BroadcastResponseHandler NewMessage(DominatorAccountModel dominatorAccountModel, RedditUser redditUser);
        CommentResponseHandler NewComment(DominatorAccountModel dominatorAccountModel, RedditPost newRedditPost);
        CommentResponseHandler EditComment(DominatorAccountModel account, RedditPost newRedditPost);
        UpvoteResponseHandler NewUpvote(DominatorAccountModel dominatorAccountModel, RedditPost newRedditPost);

        Task<UserNameInfoRdResponseHandler> GetUserDetailsAsync(DominatorAccountModel objDominatorAccountModel,
            CancellationToken token);

        Task<CommunitiesScraperResponseHandler> ScrapeCommunitiesAsync(DominatorAccountModel account,
            CancellationToken token);

        DownVoteResponseHandler NewDownvote(DominatorAccountModel dominatorAccountModel, RedditPost newRedditPost);
        FollowResponseHandler NewFollow(DominatorAccountModel dominatorAccountModel, RedditUser newRedditUser);
        RemoveVoteResponseHandler NewRemoveVote(DominatorAccountModel dominatorAccountModel, RedditPost newRedditPost);
        ReplyResponseHandler NewReply(DominatorAccountModel dominatorAccountModel, RedditPost newRedditPost);
        SubscribeResponseHandler NewSubscribe(DominatorAccountModel dominatorAccountModel, SubRedditModel subReddit);
        UnFollowResponseHandler NewUnfollow(DominatorAccountModel dominatorAccountModel, RedditUser newRedditUser);

        UnSubscribeResponseHandler NewUnSubscribe(DominatorAccountModel dominatorAccountModel,
            SubRedditModel subReddit);

        Task<bool> RedditAds(IResponseParameter accloginResp, DominatorAccountModel dominatorAccountModel, string DestinationUrl, int ScrollCount = 15, bool isSubreddit = false);

        SubredditResponseHandler ScrapeSubRedditsByUrl(DominatorAccountModel jobProcessDominatorAccountModel,
            string queryInfoQueryValue, QueryInfo queryInfo, SubredditResponseHandler subredditResponseHandler);

        SubredditResponseHandler ScrapeSubRedditsByKeywords(DominatorAccountModel account, object url,
            QueryInfo queryInfo, SubredditResponseHandler subredditResponseHandler);

        RedditCommentRespondHandler ScrapeCommentByUrl(DominatorAccountModel account, string postLink,
            QueryInfo queryInfo, RedditCommentRespondHandler redditCommentRespondHandler, string userid = "");

        RedditPostResponseHandler ScrapePostsByKeywords(DominatorAccountModel jobProcessDominatorAccountModel,
            object queryInfoQueryValue, QueryInfo queryInfo, RedditPostResponseHandler redditPostResponseHandler, string SearchUrl = "");
        RedditUserResponseHandler ScrapeUsersByKeywords(DominatorAccountModel jobProcessDominatorAccountModel,
            object queryInfoQueryValue, QueryInfo queryInfo, RedditUserResponseHandler redditPostResponseHandler, string SearchUrl = "");

        List<string> SubRedditSubcription(DominatorAccountModel jobProcessDominatorAccountModel);

        RedditPostResponseHandler ScrapePostsByCampaignIdUrl(DominatorAccountModel jobProcessDominatorAccountModel,
            string postLink, RedditPostResponseHandler redditPostResponseHandler);

        UserResponseHandler GetUserDetailsByUsername(DominatorAccountModel jobProcessDominatorAccountModel,
            string username);

        List<string> FollowedUserList(DominatorAccountModel jobProcessDominatorAccountModel);

        ScrapeCommentedUsersResponseHandler ScrapeCommentedUsersOnPosts(
            DominatorAccountModel jobProcessDominatorAccountModel, string queryInfoQueryValue, QueryInfo queryInfo,
            ScrapeCommentedUsersResponseHandler scrapeCommentedUsersResponseHandler);

        RequestParameters SetRequestParametersAndProxy(DominatorAccountModel accountModel);
        PaginationParameter GetPaginationParameter(DominatorAccountModel account);
        ActivityResposneHandler BroadCastMessage(ActivityType activityType, DominatorAccountModel dominatorAccount, RedditUser redditUser);
        void SubscribeQuarantineCommunity(DominatorAccountModel account,
            SubredditResponseHandler subredditResponsehandler, string queryValue);
        IResponseParameter GetConversationDetails(DominatorAccountModel dominatorAccount, RedditUser redditUser);
        bool IsSuspended(IRdHttpHelper httpHelperForAd, string destinationUrl);
        IResponseParameter ApiResponse(string api);
    }

    public class RedditFunction : IRedditFunction
    {
        private readonly IAccountScopeFactory _accountScopeFactory;
        private readonly IDelayService _delayService;
        private readonly IRdHttpHelper _httpHelper;
        private List<string> QueryString { get; set; } = new List<string>();
        private readonly string HomePage = RdConstants.GetRedditHomePageAPI;
        private string DefaultQueryString { get; set; } = "Between big life decisions heartbreaks tragedies and even simple bad days when nothing seems to go right it's easy to get down on your outlook on life You are the sum total of everything you've ever seen heard eaten smelled been told forgot—it's all there Everything influences each of us and because of that I try to make sure that my experiences are positive";
        public RedditFunction(IRdHttpHelper httpHelper, IDelayService delayService)
        {
            _httpHelper = httpHelper;
            _delayService = delayService;
            _accountScopeFactory = InstanceProvider.GetInstance<IAccountScopeFactory>();
        }

        public async Task<UserNameInfoRdResponseHandler> GetUserDetailsAsync(DominatorAccountModel account,
            CancellationToken token)
        {
            try
            {
                // Initialize Header Parameters
                new RequestParameter(_httpHelper);
                //var url = "https://www.reddit.com/";
                var url = RdConstants.GetProfileDetailsAPI(account.AccountBaseModel.UserName);
                var response = await _httpHelper.GetRequestAsync(url, token);
                if(response !=null && response.Response !=null && response.Response.Contains("Not Found"))
                {
                    try
                    {
                        response = await _httpHelper.GetRequestAsync("https://www.reddit.com/svc/shreddit/reddit-chat", token);
                        if (response != null && response.Response != null)
                        {
                            var username = Utilities.GetBetween(response.Response, "display-name=\"", "\"");
                            if (!string.IsNullOrEmpty(username))
                            {
                                response = await _httpHelper.GetRequestAsync(RdConstants.UserProfileUrlByUsername(username), token);
                            }
                        }
                        
                    }
                    catch (Exception)
                    {
                    }
                    
                }
                var objUserNameInfoPtResponseHandler = new UserNameInfoRdResponseHandler(response);
                return objUserNameInfoPtResponseHandler;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public List<RedditUser> GetInteractedUsersLastMessage(List<string> userId, DominatorAccountModel account)
        {
            List<RedditUser> userLastMessageList = new List<RedditUser>();
            foreach (var eachUserChat in userId)
            {
                var userChatsReqUrl = $"https://sendbirdproxyk8s.chat.redditmedia.com/v3/group_channels/sendbird_group_channel_{eachUserChat}/messages?is_sdk=true&prev_limit=40&next_limit=0&include=false&reverse=true&message_ts=9007199254740991&custom_types=*&with_sorted_meta_array=false&include_reactions=false&include_thread_info=false&include_replies=false&include_parent_message_text=false";
                var requestParameter = new RequestParameter();
                //var requestParameter = _httpHelper.GetRequestParameter();
                requestParameter.Accept = "application/json, text/plain, *";
                requestParameter.Headers.Add("Accept-Encoding", "gzip, deflate, br");
                requestParameter.Headers.Add("Accept-Language", "en-US,en;q=0.9");
                requestParameter.Referer = $"{HomePage}/";
                requestParameter.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/90.0.4430.93 Safari/537.36";
                requestParameter.Headers.Add("SB-User-Agent", "JS%2Fc3.0.148%2F%2F");
                requestParameter.Headers.Add("SendBird", "JS,Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML. like Gecko) Chrome/90.0.4430.93 Safari/537.36,3.0.148,2515BDA8-9D3A-47CF-9325-330BC37ADA13s");
                requestParameter.Headers.Add("Origin", $"{HomePage}");
                requestParameter.Headers.Add("Connection", "keep-alive");
                requestParameter.Headers.Add("Host", "sendbirdproxyk8s.chat.redditmedia.com");
                requestParameter.AddHeader("Session-Key", "19d64b91d59e7b82335a1e1763a4ff4c0cf19bdf");
                requestParameter.Cookies = account.Cookies;
                _httpHelper.SetRequestParameter(requestParameter);
                var response = _httpHelper.GetRequest(userChatsReqUrl);
                RedditChatResponseHandler chatResponseHadler = new RedditChatResponseHandler(response, "");
                if (!string.IsNullOrEmpty(chatResponseHadler.user.Username) && chatResponseHadler.user.Username != account.UserName)
                    userLastMessageList.Add(chatResponseHadler.user);
            }
            return userLastMessageList;
        }

        public RedditPostResponseHandler ScrapePostsByKeywords(DominatorAccountModel account, object url,
            QueryInfo queryInfo, RedditPostResponseHandler redditPostResponseHandler, string SearchUrl = "")
        {
            RedditPostResponseHandler objRedditPostResponseHandler = null;

            #region Scraper

            try
            {
                if (redditPostResponseHandler == null)
                {
                    // Initialize Header Parameters
                    new RequestParameter(_httpHelper);
                    var searchUrl = string.IsNullOrEmpty(SearchUrl) ? $"{HomePage}/search/?q={queryInfo.QueryValue}&type=people" : SearchUrl;
                    var response = _httpHelper.GetRequest(searchUrl);
                    objRedditPostResponseHandler = new RedditPostResponseHandler(response, false, null);
                    if (!string.IsNullOrEmpty(response.Response) && objRedditPostResponseHandler != null)
                    {
                        objRedditPostResponseHandler.HasMoreResults = objRedditPostResponseHandler.LstRedditPost.Count > 0;
                        objRedditPostResponseHandler.Success = true;
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(redditPostResponseHandler.PaginationParameter.LastPaginationId))
                    {
                        objRedditPostResponseHandler.HasMoreResults = false;
                        objRedditPostResponseHandler.Success = true;
                        return objRedditPostResponseHandler;
                    }
                    // Assign Authorization Parameters
                    var rdParameters = GetRdParameters(redditPostResponseHandler.PaginationParameter);
                    rdParameters.RefererValue = $"{HomePage}/search/?q=" + queryInfo.QueryValue +
                                                "&type=posts";

                    // Assign Extra Header Parameters
                    var rdRequestParameter = new RequestParameter(_httpHelper);
                    rdRequestParameter.AddExtraHeaders(account, rdParameters);

                    var gatewayUrl = string.IsNullOrEmpty(SearchUrl) ? "https://gateway.reddit.com/desktopapi/v1/search?allow_over18=&q=" +
                                     queryInfo.QueryValue + "&sort=relevance&t=all&type=link&include_over_18=&after=" +
                                     redditPostResponseHandler.PaginationParameter.LastPaginationId : SearchUrl;
                    var NextPageUrl = $"{HomePage}/search/?q={queryInfo.QueryValue}&type=posts&cursor={redditPostResponseHandler.PaginationParameter.LastPaginationId}";
                    var response = _httpHelper.GetRequest(NextPageUrl);
                    objRedditPostResponseHandler = new RedditPostResponseHandler(response, true,
                        redditPostResponseHandler.PaginationParameter);

                    if (!string.IsNullOrEmpty(response.Response) && objRedditPostResponseHandler != null)
                    {
                        objRedditPostResponseHandler.HasMoreResults = objRedditPostResponseHandler.LstRedditPost.Count > 0;
                        objRedditPostResponseHandler.Success = true;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            #endregion

            return objRedditPostResponseHandler;
        }

        public RedditUserResponseHandler ScrapeUsersByKeywords(DominatorAccountModel account, object url,
           QueryInfo queryInfo, RedditUserResponseHandler redditUserResponseHandler, string SearchUrl = "")
        {
            #region Scraper

            try
            {
                if (redditUserResponseHandler == null)
                {
                    // Initialize Header Parameters
                    new RequestParameter(_httpHelper);
                    var searchUrl = string.IsNullOrEmpty(SearchUrl) ? $"{HomePage}/search/?q={queryInfo.QueryValue}&type=people" : SearchUrl;
                    var response = _httpHelper.GetRequest(searchUrl);
                    redditUserResponseHandler = new RedditUserResponseHandler(response, false, null);
                    if (!string.IsNullOrEmpty(response.Response) && redditUserResponseHandler != null)
                    {
                        redditUserResponseHandler.HasMoreResults = redditUserResponseHandler.LstRedditUser.Count > 0;
                        redditUserResponseHandler.Success = true;
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(redditUserResponseHandler.PaginationParameter.LastPaginationId))
                    {
                        redditUserResponseHandler.HasMoreResults = false;
                        redditUserResponseHandler.Success = false;
                        return redditUserResponseHandler;
                    }
                    // Assign Authorization Parameters
                    var rdParameters = GetRdParameters(redditUserResponseHandler.PaginationParameter);
                    rdParameters.RefererValue = $"{HomePage}/search/?q=" + queryInfo.QueryValue +
                                                "&type=people";

                    // Assign Extra Header Parameters
                    var rdRequestParameter = new RequestParameter(_httpHelper);
                    rdRequestParameter.AddExtraHeaders(account, rdParameters);

                    var gatewayUrl = string.IsNullOrEmpty(SearchUrl) ? "https://gateway.reddit.com/desktopapi/v1/search?allow_over18=&q=" +
                                     queryInfo.QueryValue + "&sort=relevance&t=all&type=link&include_over_18=&after=" +
                                     redditUserResponseHandler.PaginationParameter.LastPaginationId : SearchUrl;
                    var NextPageUrl = $"{HomePage}/search/?q={queryInfo.QueryValue}&type=people&cursor={redditUserResponseHandler.PaginationParameter.LastPaginationId}";
                    var response = _httpHelper.GetRequest(NextPageUrl);
                    redditUserResponseHandler = new RedditUserResponseHandler(response, true,
                        redditUserResponseHandler.PaginationParameter);

                    if (!string.IsNullOrEmpty(response.Response) && redditUserResponseHandler != null)
                    {
                        redditUserResponseHandler.HasMoreResults = redditUserResponseHandler.LstRedditUser.Count > 0;
                        redditUserResponseHandler.Success = true;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            #endregion

            return redditUserResponseHandler;
        }
        public string ConvertToShredditUrl(string redditUrl)
        {
            // Parse the original URL
            var uri = new Uri(redditUrl);
            var segments = uri.Segments; // e.g. ["/r/", "TravisScottIndia/", "comments/", "1ohb2n0/", "buying_1_ticket/"]

            // Extract subreddit name and post ID
            string subreddit = segments[2].TrimEnd('/');
            string postId = segments[4].TrimEnd('/');

            // Construct the shreddit API URL
            return $"https://www.reddit.com/svc/shreddit/comments/r/{subreddit}/t3_{postId}?render-mode=partial&seeker-session=true&utm_source=share";
        }

        public ScrapeCommentedUsersResponseHandler ScrapeCommentedUsersOnPosts(DominatorAccountModel account,
            string postUrl, QueryInfo queryInfo,
            ScrapeCommentedUsersResponseHandler scrapeCommentedUsersResponseHandler)
        {
            ScrapeCommentedUsersResponseHandler objScrapeCommentedUsersResponseHandler = null;

            #region Scraper

            try
            {
                if (scrapeCommentedUsersResponseHandler == null)
                {
                    // Initialize Header Parameters
                    new RequestParameter(_httpHelper);
                    //https://www.reddit.com/svc/shreddit/comments/r/travisscottindia/t3_1ohb2n0?render-mode=partial&seeker-session=true&utm_source=share
                    var response = _httpHelper.GetRequest(ConvertToShredditUrl(Utils.UpdateDomain(postUrl)));
                    objScrapeCommentedUsersResponseHandler =
                        new ScrapeCommentedUsersResponseHandler(response, false, null);
                    if (!string.IsNullOrEmpty(response.Response) && objScrapeCommentedUsersResponseHandler != null)
                    {
                        objScrapeCommentedUsersResponseHandler.HasMoreResults = objScrapeCommentedUsersResponseHandler.LstCommentedUser.Count > 0;
                        objScrapeCommentedUsersResponseHandler.Success = true;
                    }
                }
                else
                {
                    // Assign Authorization Parameters
                    var rdParameters = GetRdParameters(scrapeCommentedUsersResponseHandler.PaginationParameter);
                    rdParameters.RefererValue = $"{HomePage}/search?q" + queryInfo.QueryValue +
                                                "&type=link&include_over_18=";

                    // Assign Extra Header Parameters
                    var rdRequestParameter = new RequestParameter(_httpHelper);
                    rdRequestParameter.AddExtraHeaders(account, rdParameters);

                    //var url =$"https://gateway.reddit.com/desktopapi/v1/morecomment/{scrapeCommentedUsersResponseHandler.PaginationParameter.LastPaginationId}??rtj=debug&allow_over18=";
                    //var response = _httpHelper.GetRequest(url);
                    var response = _httpHelper.GetRequest(postUrl);
                    objScrapeCommentedUsersResponseHandler = new ScrapeCommentedUsersResponseHandler(response, true,
                        scrapeCommentedUsersResponseHandler.PaginationParameter);

                    if (!string.IsNullOrEmpty(response.Response) && objScrapeCommentedUsersResponseHandler != null)
                    {
                        objScrapeCommentedUsersResponseHandler.HasMoreResults = objScrapeCommentedUsersResponseHandler.LstCommentedUser.Count > 0;
                        objScrapeCommentedUsersResponseHandler.Success = true;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            #endregion

            return objScrapeCommentedUsersResponseHandler;
        }

        #region SubReddits Methods

        public SubredditResponseHandler ScrapeSubRedditsByKeywords(DominatorAccountModel account, object url,
            QueryInfo queryInfo, SubredditResponseHandler subredditResponseHandler)
        {
            SubredditResponseHandler objSubredditResponseHandler = null;

            #region Scraper

            try
            {
                if (subredditResponseHandler == null)
                {
                    // Initialize Header Parameters
                    new RequestParameter(_httpHelper);
                    var searchUrl = $"{HomePage}/search?q=" + queryInfo.QueryValue + "&type=sr%2Cuser";
                    var response = _httpHelper.GetRequest(searchUrl);
                    objSubredditResponseHandler = new SubredditResponseHandler(response, false, null);
                    if (!string.IsNullOrEmpty(response.Response) && objSubredditResponseHandler != null)
                    {
                        objSubredditResponseHandler.HasMoreResults = objSubredditResponseHandler.LstSubReddit.Count > 0;
                        objSubredditResponseHandler.Success = true;
                    }
                }
                else
                {
                    // Assign Authorization Parameters
                    var rdParameters = GetRdParameters(subredditResponseHandler.PaginationParameter);
                    rdParameters.RefererValue = $"{HomePage}/search?q" + queryInfo.QueryValue +
                                                "&type=sr%2Cuser&include_over_18=";

                    // Assign Extra Header Parameters
                    var rdRequestParameter = new RequestParameter(_httpHelper);
                    rdRequestParameter.AddExtraHeaders(account, rdParameters);

                    var gatewayUrl = "https://gateway.reddit.com/desktopapi/v1/search?allow_over18=&q=" +
                                     queryInfo.QueryValue +
                                     "&sort=relevance&t=all&type=sr%2Cuser&include_over_18=&after=" +
                                     subredditResponseHandler.PaginationParameter.LastPaginationId;

                    var response = _httpHelper.GetRequest(gatewayUrl);
                    objSubredditResponseHandler = new SubredditResponseHandler(response, true,
                        subredditResponseHandler.PaginationParameter);

                    if (!string.IsNullOrEmpty(response.Response) && objSubredditResponseHandler != null)
                    {
                        objSubredditResponseHandler.HasMoreResults = objSubredditResponseHandler.LstSubReddit.Count > 0;
                        objSubredditResponseHandler.Success = true;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            #endregion

            return objSubredditResponseHandler;
        }

        #endregion

        public async Task<CommunitiesScraperResponseHandler> ScrapeCommunitiesAsync(DominatorAccountModel account,
            CancellationToken token)
        {
            CommunitiesScraperResponseHandler communitiesScraperResponseHandler = null;
            try
            {
                // Assign Authorization Parameters
                var authorizationParameters = GetPaginationParameter(account);
                var rdParameters = GetRdParameters(authorizationParameters);

                // Initialize Header Parameters & Assign Extra Header Values
                var rdRequestParameter = new RequestParameter(_httpHelper);
                rdRequestParameter.AddExtraHeaders(account, rdParameters);

                var response = await _httpHelper.GetRequestAsync(RdConstants.CommunityListUrl, token);
                communitiesScraperResponseHandler = new CommunitiesScraperResponseHandler(response);
                if (!string.IsNullOrEmpty(response.Response) && communitiesScraperResponseHandler != null)
                {
                    communitiesScraperResponseHandler.HasMoreResults = communitiesScraperResponseHandler.LstCommunities.Count > 0;
                    communitiesScraperResponseHandler.Success = true;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return communitiesScraperResponseHandler;
        }

        public RedditPostResponseHandler ScrapePostsByUrl(DominatorAccountModel account, string postLink,
            QueryInfo queryInfo, RedditPostResponseHandler redditPostResponseHandler, bool isPaginationRequire = false,
            string sortBy = "")
        {
            RedditPostResponseHandler objRedditPostResponseHandler = null;

            #region Scraper

            try
            {
                if (redditPostResponseHandler == null)
                {
                    // Initialize Header Parameters
                    new RequestParameter(_httpHelper);
                    var TryCount = 0;
                TryAgain:
                    var response = _httpHelper.GetRequest(postLink);
                    objRedditPostResponseHandler = new RedditPostResponseHandler(response, false, null);
                    while (TryCount++ <= 2 && objRedditPostResponseHandler != null && objRedditPostResponseHandler.LstRedditPost?.Count == 0)
                        goto TryAgain;
                    if (!string.IsNullOrEmpty(response?.Response) && objRedditPostResponseHandler != null)
                    {
                        objRedditPostResponseHandler.HasMoreResults = objRedditPostResponseHandler.LstRedditPost.Count > 0;
                        objRedditPostResponseHandler.BookMark = postLink;
                        objRedditPostResponseHandler.Success = true;
                    }
                }

                //For pagination part of urlscraper, userpost and subreddit post
                else
                {
                    if (isPaginationRequire)
                    {
                        var  _req = _httpHelper.GetRequestParameter();
                        _req.Referer = queryInfo.QueryValue;
                        _httpHelper.SetRequestParameter(_req);

                        var paginationUrl = string.Empty;
                        if (queryInfo.QueryType.Equals("Specific User's Post"))
                        {
                            //var userPostUrl = queryInfo.QueryValue?.TrimEnd('/').Replace("/posts", string.Empty);
                            //var userName = Utils.GetLastWordFromUrl(userPostUrl)?.Replace("?to=", string.Empty);
                            //paginationUrl = "https://gateway.reddit.com/desktopapi/v1/user/"
                            //                + userName +
                            //                "/posts?rtj=only&allow_quarantined=true&allow_over18=&include=identity&after="
                            //                + redditPostResponseHandler.PaginationParameter.LastPaginationId
                            //                + "&dist=25&sort=new&t=all&layout=classic";
                            paginationUrl = postLink.Replace("amp;","");
                        }
                        else
                        {
                            var subreddit = Utils.GetLastWordFromUrl(postLink)?.Replace("?to=", string.Empty);
                            paginationUrl = "https://gateway.reddit.com/desktopapi/v1/subreddits/"
                                            + subreddit +
                                            "?rtj=only&redditWebClient=web2x&app=web2x-client-production&after="
                                            + redditPostResponseHandler.PaginationParameter.LastPaginationId
                                            + "&dist=14&layout=card&sort=" + sortBy.ToLower() +
                                            "&allow_over18=&include=identity";
                        }

                        var response = _httpHelper.GetRequest(paginationUrl);
                        objRedditPostResponseHandler = new RedditPostResponseHandler(response, true,
                            redditPostResponseHandler.PaginationParameter);
                        if (!string.IsNullOrEmpty(response.Response) && objRedditPostResponseHandler != null)
                        {
                            objRedditPostResponseHandler.HasMoreResults = objRedditPostResponseHandler.LstRedditPost.Count > 0;
                            objRedditPostResponseHandler.Success = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            #endregion

            return objRedditPostResponseHandler;
        }

        public RedditPostResponseHandler ScrapePostsByCampaignIdUrl(DominatorAccountModel account, string postLink,
            RedditPostResponseHandler redditPostResponseHandler)
        {
            RedditPostResponseHandler objRedditPostResponseHandler = null;

            #region Scraper

            try
            {
                if (redditPostResponseHandler == null)
                {
                    // Initialize Header Parameters
                    new RequestParameter(_httpHelper);
                    var response = _httpHelper.GetRequest(postLink);
                    objRedditPostResponseHandler = new RedditPostResponseHandler(response, false, null);
                    if (!string.IsNullOrEmpty(response.Response) && objRedditPostResponseHandler != null)
                    {
                        objRedditPostResponseHandler.HasMoreResults = objRedditPostResponseHandler.LstRedditPost.Count > 0;
                        objRedditPostResponseHandler.Success = true;
                    }
                }
            }

            catch (Exception ex)
            {
                ex.DebugLog();
            }

            #endregion

            return objRedditPostResponseHandler;
        }

        public RedditCommentRespondHandler ScrapeCommentByUrl(DominatorAccountModel account, string postLink,
            QueryInfo queryInfo, RedditCommentRespondHandler redditCommentRespondHandler, string userid = "")
        {
            RedditCommentRespondHandler objRedditCommentRespondHandler = null;
            try
            {
                if (redditCommentRespondHandler == null)
                {
                    // Initialize Header Parameters
                    new RequestParameter(_httpHelper);
                    var response = _httpHelper.GetRequest(postLink);
                    var url=Utils.GetBetween(response.Response, "TopCommentsPermalink_L3jhvG\"\n        src=\"", "\"");
                    if (string.IsNullOrEmpty(url))
                    {
                        url =RdConstants.GetRedditHomePageAPI+ Utils.GetBetween(response.Response, "TopComments_9T32hK\" src=\"", "\"");
                    }
                    response = _httpHelper.GetRequest(url);
                    objRedditCommentRespondHandler = new RedditCommentRespondHandler(response, false, null, userid);
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return objRedditCommentRespondHandler;
        }

        public SubredditResponseHandler ScrapeSubRedditsByUrl(DominatorAccountModel account, string postLink,
            QueryInfo queryInfo, SubredditResponseHandler unSubscribeResponseHandler)
        {
            SubredditResponseHandler objSubredditResponseHandler = null;

            #region Scraper

            try
            {
                if (unSubscribeResponseHandler == null)
                {
                    var url = postLink;
                    if (!url.Contains($"{HomePage}"))
                    {
                        url = url.Contains("/") ? url.Split('/').LastOrDefault(y => y != string.Empty) : url;
                        url = $"{HomePage}/r/{url}";
                    }

                    // Initialize Header Parameters
                    new RequestParameter(_httpHelper);
                    var response = _httpHelper.GetRequest(url);
                    var queryValue = url;
                    // Passing queryValue for identifying particular user to store in SubRedditList, in case of custom query  
                    objSubredditResponseHandler = new SubredditResponseHandler(response, false, null, queryValue);
                    if (!string.IsNullOrEmpty(response.Response) && objSubredditResponseHandler != null)
                    {
                        objSubredditResponseHandler.HasMoreResults = objSubredditResponseHandler.LstSubReddit.Count > 0;
                        objSubredditResponseHandler.Success = true;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            #endregion

            return objSubredditResponseHandler;
        }

        public List<string> SubRedditSubcription(DominatorAccountModel account)
        {
            var subscriptionsList = new List<string>();
            try
            {
                // Assign Authorization Parameters
                var paginationParameters = GetPaginationParameter(account);
                var rdParameters = GetRdParameters(paginationParameters);

                // Initialize Header Parameters & Assign Extra Header Values
                var rdRequestParameter = new RequestParameter(_httpHelper);
                rdRequestParameter.AddExtraHeaders(account, rdParameters);

                var response = _httpHelper.GetRequest(RdConstants.CommunityListUrl);
                var ListCommunities = new CommunitiesScraperResponseHandler(response).LstCommunities;
                subscriptionsList = ListCommunities.Select(c => c.DisplayText.Replace("r/", "")).ToList();
                #region OldCode
                //var jsonObject = JObject.Parse(response.Response);
                //IEnumerable<JToken> subscriptionJson = jsonObject["subreddits"].Children();
                //foreach (var subscription in subscriptionJson)
                //{
                //    var subscriptionOfSubscriber = subscription.ToString();
                //    subscriptionOfSubscriber =
                //        subscriptionOfSubscriber.Substring(
                //            subscriptionOfSubscriber.IndexOf("{", StringComparison.Ordinal));
                //    var jsonobjectSubscription = JObject.Parse(subscriptionOfSubscriber)["url"].ToString();
                //    subscriptionsList.Add(jsonobjectSubscription);
                //}
                #endregion
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return subscriptionsList;
        }

        public List<string> FollowedUserList(DominatorAccountModel account)
        {
            var followedUserList = new List<string>();
            try
            {
                // Assign Authorization Parameters
                var paginationParameters = GetPaginationParameter(account);
                var rdParameters = GetRdParameters(paginationParameters);
                rdParameters.RefererValue = $"{ RdConstants.NewRedditHomePageAPI}/user/{account.UserName }";
                // Initialize Header Parameters & Assign Extra Header Values
                var requestParameters = new RequestParameter(_httpHelper);
                requestParameters.AddExtraHeaders(account, rdParameters);

                var response = _httpHelper.GetRequest($"{ RdConstants.NewRedditHomePageAPI}/user/{account.UserName }/followers");
                var jsonString = RdConstants.GetJsonPageResponse(response.Response);
                var handler = JsonHandler.GetInstance;
                handler = new JsonHandler(jsonString);
                var followedUserJson = handler.GetJToken("pages", "followers", "models");
                foreach (var followedUser in followedUserJson)
                {
                    try
                    {
                        var handler2 = new JsonHandler(followedUser.First);
                        var data = handler2.GetElementValue("name");
                        followedUserList.Add(data);
                    }
                    catch (Exception)
                    {
                    }
                }
                #region OldCode
                //var response = _httpHelper.GetRequest(RdConstants.CommunityListUrl);
                //var jsonObject = JObject.Parse(response.Response);
                //IEnumerable<JToken> followedUserJson = jsonObject["profiles"].Children();
                //foreach (var followedUser in followedUserJson)
                //{
                //    var followedUserOfFollow = followedUser.ToString();
                //    followedUserOfFollow =
                //        followedUserOfFollow.Substring(followedUserOfFollow.IndexOf("{", StringComparison.Ordinal));
                //    var jsonobjectFollowedUser = JObject.Parse(followedUserOfFollow)["displayText"]?.ToString()
                //        ?.Replace("u/", "");
                //    followedUserList.Add(jsonobjectFollowedUser);
                //}
                #endregion
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return followedUserList;
        }

        public UserResponseHandler GetUserDetailsByUsername(DominatorAccountModel account, string username)
        {
            var rdParameter = new RdParameters();
            rdParameter.RefererValue = RdConstants.UserProfileUrlByUsername(username);
            var rdRequestParameter = new RequestParameter(_httpHelper);
            rdRequestParameter.AddExtraHeaders(account, rdParameter);
            var url = username;
            if (!username.Contains("https://www.reddit.com/user/") || !username.Contains(HomePage))
                url = $"{HomePage}/user/{username}";

            var response = _httpHelper.GetRequest(url);
            var userResponseHandler = new UserResponseHandler(response);
            return userResponseHandler;
        }

        public FollowResponseHandler NewFollow(DominatorAccountModel account, RedditUser user)
        {
            var rdParameters = new RdParameters();
            rdParameters.SessionValue = _httpHelper.GetRequestParameter().Cookies["session_tracker"]?.Value;
            rdParameters.LoidValue = _httpHelper.GetRequestParameter().Cookies["loid"]?.Value;
            rdParameters.OriginValue = "https://www.reddit.com";
            //rdParameters.RefererValue = user.Url;

            // Assign Extra Header Parameters
            var rdRequestParameter = new RequestParameter(_httpHelper);
            rdRequestParameter.AddExtraHeaders(account, rdParameters);
            //_httpHelper.GetRequestParameter().ContentType = "application/x-www-form-urlencoded";

            var csrftoken = _httpHelper.GetRequestParameter().Cookies["csrf_token"]?.Value;
            var FollowResponse = _httpHelper.PostRequest(RdConstants.GetFinalPostSubmitApi, Encoding.UTF8.GetBytes(RdConstants.GetPostDataForUserIdentity(csrftoken)));
            csrftoken = _httpHelper.GetRequestParameter().Cookies["csrf_token"]?.Value;
            FollowResponse = _httpHelper.PostRequest(RdConstants.GetFinalPostSubmitApi, Encoding.UTF8.GetBytes(RdConstants.GetPostDataForFollowingUser(user.Id, csrftoken)));
            var response = new FollowResponseHandler(FollowResponse);

            return response;
        }

        public UnFollowResponseHandler NewUnfollow(DominatorAccountModel account, RedditUser user)
        {
            var rdParameters = new RdParameters();
            rdParameters.SessionValue = _httpHelper.GetRequestParameter().Cookies["session_tracker"]?.Value;
            rdParameters.LoidValue = _httpHelper.GetRequestParameter().Cookies["loid"]?.Value;
            rdParameters.OriginValue = "https://www.reddit.com";
            var rdRequestParameter = new RequestParameter(_httpHelper);
            rdRequestParameter.AddExtraHeaders(account, rdParameters);

            var csrftoken = _httpHelper.GetRequestParameter().Cookies["csrf_token"]?.Value;
            var UnFollowResponse = _httpHelper.PostRequest(RdConstants.GetFinalPostSubmitApi, Encoding.UTF8.GetBytes(RdConstants.GetPostDataForUserIdentity(csrftoken)));
            csrftoken = _httpHelper.GetRequestParameter().Cookies["csrf_token"]?.Value;
            UnFollowResponse = _httpHelper.PostRequest(RdConstants.GetFinalPostSubmitApi, Encoding.UTF8.GetBytes(RdConstants.GetPostDataForFollowingUser(user.Id, csrftoken, false)));

            return new UnFollowResponseHandler(UnFollowResponse);
        }

        public ReplyResponseHandler NewReply(DominatorAccountModel account, RedditPost newRedditPost)
        {
            ReplyResponseHandler response = null;
            try
            {
                // Assign authorization parameters
                var rdParameters = new RdParameters();
                rdParameters.SessionValue = newRedditPost.PaginationParameter.SessionTracker;
                rdParameters.AuthorizationValue = $"Bearer {newRedditPost.PaginationParameter.AccessToken}";
                rdParameters.RefererValue = newRedditPost.Permalink;
                rdParameters.LoidValue = newRedditPost.PaginationParameter.Loid;
                rdParameters.RdHostValue = "oauth.reddit.com";

                // Assign Extra Header Parameters
                var rdRequestParameter = new RequestParameter(_httpHelper);
                rdRequestParameter.AddExtraHeaders(account, rdParameters);

                RdJsonElement[] cJson =
                {
                    new RdJsonElement
                    {
                        E = "text",
                        T = newRedditPost.Commenttext
                    }
                };

                RdJsonElement[] documentJson =
                {
                    new RdJsonElement
                    {
                        E = "par",
                        C = cJson
                    }
                };

                var jsonElements = new RdJsonElement
                {
                    Text = "",
                    RichtextJson = new RdJsonElement
                    {
                        Document = documentJson
                    },
                    ApiType = "json",
                    ThingId = $"{newRedditPost.Id}",
                    ReturnRtjson = true
                };

                var postData = rdRequestParameter.GetPostDataFromJson(jsonElements);
                var response1 = _httpHelper.PostRequest(RdConstants.CommentOAuthUrl, postData);
                response = new ReplyResponseHandler(response1);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return response;
        }

        public CommentResponseHandler NewComment(DominatorAccountModel account, RedditPost newRedditPost)
        {
            CommentResponseHandler response = null;
            try
            {
                var rdParameters = new RdParameters();
                rdParameters.SessionValue = _httpHelper.GetRequestParameter().Cookies["session_tracker"]?.Value;
                rdParameters.LoidValue = _httpHelper.GetRequestParameter().Cookies["loid"]?.Value;
                rdParameters.OriginValue = "https://www.reddit.com";
                rdParameters.RefererValue = newRedditPost.Permalink;

                
                
                var csrftoken = _httpHelper.GetRequestParameter().Cookies["csrf_token"]?.Value;
                var commentOnPostResponse = _httpHelper.PostRequest(RdConstants.GetFinalPostSubmitApi, Encoding.UTF8.GetBytes(RdConstants.PostDataToUpdateToken(csrftoken,newRedditPost.Id)));
                csrftoken = _httpHelper.GetRequestParameter().Cookies["csrf_token"]?.Value;
                // Assign Extra Header Parameters
                var rdRequestParameter = new RequestParameter(_httpHelper);
                rdRequestParameter.AddExtraHeaders(account, rdParameters);
                _httpHelper.GetRequestParameter().ContentType = "application/x-www-form-urlencoded";
                commentOnPostResponse = _httpHelper.PostRequest($"https://www.reddit.com/svc/shreddit/{newRedditPost.Id}/create-comment", Encoding.UTF8.GetBytes(RdConstants.PostDataForCommentOnPost(csrftoken,newRedditPost.Commenttext)));               
                response = new CommentResponseHandler(new ResponseParameter{ Response = commentOnPostResponse.Response, HasError=!commentOnPostResponse.Response.Contains($"{newRedditPost.Id}") });

                if (!string.IsNullOrEmpty(response.ErrorMessage))
                {
                    //GlobusLogHelper.log.Info(Log.CustomMessage, account.AccountBaseModel.AccountNetwork,
                    //  account.UserName, "Comment", $"[Error From Website] - {response.ErrorMessage}");

                    var delayMin = Regex.Match(response.ErrorMessage, @"\d+")?.Value;

                    if (!string.IsNullOrEmpty(delayMin))
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage, account.AccountBaseModel.AccountNetwork,
                            account.UserName, "Comment",
                            $"Please Wait - Activity will starts after {int.Parse(delayMin) + 2} minute, " +
                            "As we got response from website");

                        _delayService.DelayAsync(TimeSpan.FromMinutes(int.Parse(delayMin) + 2)).Wait();

                        commentOnPostResponse = _httpHelper.PostRequest($"https://www.reddit.com/svc/shreddit/{newRedditPost.Id}/create-comment", Encoding.UTF8.GetBytes(RdConstants.PostDataForCommentOnPost(csrftoken, newRedditPost.Commenttext)));
                        response = new CommentResponseHandler(new ResponseParameter { Response = commentOnPostResponse.Response, HasError = !commentOnPostResponse.Response.Contains($"{newRedditPost.Id}") });
                    }
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return response;
        }

        public CommentResponseHandler EditComment(DominatorAccountModel account, RedditPost newRedditPost)
        {
            CommentResponseHandler response = null;
            try
            {
                // Assign Authorization Parameters
                var rdParameters = new RdParameters();
                rdParameters.SessionValue = newRedditPost.PaginationParameter.SessionTracker;
                rdParameters.AuthorizationValue = $"Bearer {newRedditPost.PaginationParameter.AccessToken}";
                rdParameters.LoidValue = newRedditPost.PaginationParameter.Loid;
                rdParameters.RdHostValue = "oauth.reddit.com";

                // Assign Extra Header Parameters
                var rdRequestParameter = new RequestParameter(_httpHelper);
                rdRequestParameter.AddExtraHeaders(account, rdParameters);

                RdJsonElement[] cJson =
                {
                    new RdJsonElement
                    {
                        E = "text",
                        T = newRedditPost.Commenttext
                    }
                };

                RdJsonElement[] documentJson =
                {
                    new RdJsonElement
                    {
                        E = "par",
                        C = cJson
                    }
                };

                var jsonElements = new RdJsonElement
                {
                    Text = "",
                    RichtextJson = new RdJsonElement
                    {
                        Document = documentJson
                    },
                    ApiType = "json",
                    ThingId = newRedditPost.Id,
                    ReturnRtjson = true
                };

                var postData = rdRequestParameter.GetPostDataFromJson(jsonElements);
                var response1 = _httpHelper.PostRequest(RdConstants.EditCommenOAuthtUrl, postData);
                response = new CommentResponseHandler(response1);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return response;
        }

        public SubscribeResponseHandler NewSubscribe(DominatorAccountModel account, SubRedditModel subReddit)
        {
            SubscribeResponseHandler response = null;

            try
            {
                // Assign Authorization Parameters
                var rdParameters = new RdParameters();
                rdParameters.SessionValue = _httpHelper.GetRequestParameter().Cookies["session_tracker"]?.Value;
                rdParameters.LoidValue = _httpHelper.GetRequestParameter().Cookies["loid"]?.Value;
                rdParameters.OriginValue = $"{RdConstants.GetRedditHomePageAPI}";

                var rdRequestParameter = new RequestParameter(_httpHelper);
                rdRequestParameter.AddExtraHeaders(account, rdParameters);

                var jsonElements = new RdJsonElement
                {
                    Action = "sub",
                    SrName = $"{subReddit.DisplayText.Replace("r/", "")}",
                    ApiType = "json"
                };
                var csrftoken = _httpHelper.GetRequestParameter().Cookies["csrf_token"]?.Value;
                var SubscribeResponse = _httpHelper.PostRequest(RdConstants.GetFinalPostSubmitApi, Encoding.UTF8.GetBytes(RdConstants.GetPostDataForUserIdentity(csrftoken)));
                csrftoken = _httpHelper.GetRequestParameter().Cookies["csrf_token"]?.Value;
                //var postData = rdRequestParameter.GetPostDataFromJson(jsonElements);
                SubscribeResponse = _httpHelper.PostRequest(RdConstants.GraphQlAPI, Encoding.UTF8.GetBytes(RdConstants.GetPostDataForChannelSubscription(subReddit.Id, csrftoken, true)));
                response = new SubscribeResponseHandler(SubscribeResponse);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return response;
        }

        public UnSubscribeResponseHandler NewUnSubscribe(DominatorAccountModel account, SubRedditModel subReddit)
        {
            UnSubscribeResponseHandler response = null;

            try
            {
                // Assign Authorization Parameters
                var rdParameters = new RdParameters();
                rdParameters.SessionValue = _httpHelper.GetRequestParameter().Cookies["session_tracker"]?.Value;
                rdParameters.LoidValue = _httpHelper.GetRequestParameter().Cookies["loid"]?.Value;
                rdParameters.OriginValue = $"{RdConstants.GetRedditHomePageAPI}";

                var rdRequestParameter = new RequestParameter(_httpHelper);
                rdRequestParameter.AddExtraHeaders(account, rdParameters);

                var jsonElements = new RdJsonElement
                {
                    Action = "unsub",
                    SrName = $"{subReddit.DisplayText.Replace("r/", "")}",
                    ApiType = "json"
                };

                var csrftoken = _httpHelper.GetRequestParameter().Cookies["csrf_token"]?.Value;
                var unSubscribeResponse = _httpHelper.PostRequest(RdConstants.GetFinalPostSubmitApi, Encoding.UTF8.GetBytes(RdConstants.GetPostDataForUserIdentity(csrftoken)));
                csrftoken = _httpHelper.GetRequestParameter().Cookies["csrf_token"]?.Value;
                unSubscribeResponse = _httpHelper.PostRequest(RdConstants.GraphQlAPI, Encoding.UTF8.GetBytes(RdConstants.GetPostDataForChannelSubscription(subReddit.Id, csrftoken)));

                response = new UnSubscribeResponseHandler(unSubscribeResponse);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return response;
        }

        public UpvoteResponseHandler NewUpvote(DominatorAccountModel account, RedditPost post)
        {
            try
            {
                // Assign Authorization Parameters
                var rdParameters = new RdParameters { RefererValue = post.Permalink };
                rdParameters.SessionValue = _httpHelper.GetRequestParameter().Cookies["session_tracker"]?.Value;
                rdParameters.LoidValue = _httpHelper.GetRequestParameter().Cookies["loid"]?.Value;
                rdParameters.OriginValue = "https://www.reddit.com";
                rdParameters.RefererValue = post.Permalink;

                // Assign Extra Header Parameters
                var rdRequestParameter = new RequestParameter(_httpHelper);
                rdRequestParameter.AddExtraHeaders(account, rdParameters);
                var csrftoken = _httpHelper.GetRequestParameter().Cookies["csrf_token"]?.Value;
                var upVoteResponse = _httpHelper.PostRequest(RdConstants.GetFinalPostSubmitApi, Encoding.UTF8.GetBytes(RdConstants.GetPostDataForUserIdentity(csrftoken)));
                csrftoken = _httpHelper.GetRequestParameter().Cookies["csrf_token"]?.Value;
                if (string.IsNullOrEmpty(post.Commenttext))
                    upVoteResponse = _httpHelper.PostRequest(RdConstants.GraphQlAPI, Encoding.UTF8.GetBytes(RdConstants.GetPostDataForVote(post.Id, csrftoken)));
                else
                    upVoteResponse = _httpHelper.PostRequest(RdConstants.GraphQlAPI, Encoding.UTF8.GetBytes(RdConstants.GetPostDataForVote(post.Id, csrftoken, true, false, true)));
                var response = new UpvoteResponseHandler(upVoteResponse);
                return response;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public BroadcastResponseHandler NewMessage(DominatorAccountModel account, RedditUser redditUser)
        {
            var URL = $"{HomePage}/api/compose?embedded=true";
            BroadcastResponseHandler response = null;
            try
            {
                // Replace with rd requestparameter
                var requestParameter = new RequestParameters();
                requestParameter.Headers.Add("Host", "www.reddit.com");
                requestParameter.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
                requestParameter.Headers.Add("Connection", "keep-alive");
                requestParameter.Accept = "application/json, text/javascript, *; q=0.01";
                requestParameter.Headers.Add("X-Requested-With", "XMLHttpRequest");
                requestParameter.UserAgent =
                    "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/67.0.3396.99 Safari/537.36";
                requestParameter.Referer = $"{HomePage}/";
                requestParameter.Headers["Accept-Language"] = "en-US,en;q=0.9";
                requestParameter.Headers.Add("Origin", "");
                requestParameter.Headers.Add("Upgrade-Insecure-Requests", "1");
                requestParameter.Headers.Add("Accept-Language", "en-US,en;q=0.9");
                requestParameter.Cookies = account.Cookies;
                var getRespo = _httpHelper.GetRequest($"{HomePage}/user/");
                var response0 = getRespo.Response;
                var uh = Utilities.GetBetween(response0, "name=\"uh\" value=\"", "\"");
                var postData = $"uh={uh}&to=" + redditUser.DisplayName + "&undefined=&subject=Nothing&thing_id=&text=" +
                               redditUser.Text +
                               "&source=compose&embedded=web2x&id=%23compose-message&renderstyle=html";
                var response1 = _httpHelper.PostRequest(URL, postData);
                if (response1.Response.Contains("\".error.BAD_CAPTCHA.field-captcha\""))
                {
                    var captchaKeyResponse = CaptchaSolver(redditUser);
                    postData = $"uh={uh}&to=" + redditUser.DisplayName + "&undefined=&subject=Nothing&thing_id=&text=" +
                               redditUser.Text + "&g-recaptcha-response=" + captchaKeyResponse +
                               "&source=compose&embedded=web2x&id=%23compose-message&renderstyle=html";
                    response1 = _httpHelper.PostRequest(URL, postData);
                }

                response = new BroadcastResponseHandler(response1);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return response;
        }

        public void ApprovePublishPost(DominatorAccountModel account, IResponseParameter response,
            PublishPostModel postDetails, PaginationParameter accountParameters)
        {
            try
            {
                // Initialize Header Parameters
                var rdRequestParameter = new RequestParameter(_httpHelper);

                // To get publish postId 
                var publishResponseData = new ApprovePostResponseHandler(response);
                var postId = string.Empty;
                postId = publishResponseData.name;

                // To get latest postId in case of media and video
                if (string.IsNullOrEmpty(postId))
                {
                    _delayService.ThreadSleep(10000);

                    // Assign Authorization Parameters
                    var rdParameter = new RdParameters();
                    rdParameter.RdHostValue = "oauth.reddit.com";
                    rdParameter.AuthorizationValue = $"Bearer {accountParameters.AccessToken}";
                    rdParameter.LoidValue = accountParameters.Loid;
                    rdParameter.SessionValue = accountParameters.SessionTracker;
                    rdParameter.RefererValue = $"{HomePage}/user/{account.AccountBaseModel.UserName}/posts";

                    // Assign Extra Header Parameters
                    rdRequestParameter.AddExtraHeaders(account, rdParameter);

                    var url =
                        $"https://gateway.reddit.com/desktopapi/v1/user/{account.AccountBaseModel.UserName}/posts?rtj=debug&layout=classic&sort=new&allow_over18=&include=identity";
                    var latestPostResponse = _httpHelper.GetRequest(url);
                    var json2 = latestPostResponse.Response;
                    var jsonobject = JObject.Parse(json2);
                    postId = jsonobject["postIds"][0].ToString();
                }

                // Assign Authorization Parameters
                var rdParameters = new RdParameters();
                rdParameters.SessionValue = accountParameters.SessionTracker;
                rdParameters.AuthorizationValue = $"Bearer {accountParameters.AccessToken}";
                rdParameters.LoidValue = accountParameters.Loid;
                rdParameters.RdHostValue = "oauth.reddit.com";
                rdParameters.RefererValue = publishResponseData.url;

                // Assign Extra Header Parameters
                rdRequestParameter.AddExtraHeaders(account, rdParameters);

                var jsonElements = new RdJsonElement
                {
                    Id = postId
                };

                var postData = rdRequestParameter.GetPostDataFromJson(jsonElements);
                var response1 = _httpHelper.PostRequest(RdConstants.ApproveUrl, postData);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public string FetchTitleOfUrl(DominatorAccountModel account, PublishPostModel postDetails)
        {
            // Initialize Header Parameters
            var rdRequestParameter = new RequestParameter(_httpHelper);

            var pageInfo = _httpHelper.GetRequest($"{HomePage}/submit");
            var objPaginationResponseHandler = new PaginationResponseHandler(pageInfo);

            // Assign Authorization Parameters
            var rdParameters = new RdParameters();
            rdParameters.RdHostValue = "oauth.reddit.com";
            rdParameters.AuthorizationValue = $"Bearer {objPaginationResponseHandler.AccessToken}";
            rdParameters.LoidValue = objPaginationResponseHandler.Loid;
            rdParameters.SessionValue = objPaginationResponseHandler.SessionTracker;

            // Assign Extra Header Parameters
            rdRequestParameter.AddExtraHeaders(account, rdParameters);

            var jsonElements = new RdJsonElement
            {
                FetchTitleUrl = $"{Uri.EscapeDataString(postDetails.Url)}",
                ApiType = "json"
            };

            var postData = rdRequestParameter.GetPostDataFromJson(jsonElements);
            var response = _httpHelper.PostRequest(RdConstants.FetchTitleOfUrl, postData);
            var response1 = new FetchTitleOfUrlResponseHandler(response);
            return response1.title;
        }

        public string GetLatestPostUrlWithGateWay(PaginationParameter accountParameter, DominatorAccountModel account)
        {
            try
            {
                _delayService.ThreadSleep(10000);

                // Assign Authorization Parameters
                var rdParameters = new RdParameters();
                rdParameters.RdHostValue = "oauth.reddit.com";
                rdParameters.AuthorizationValue = $"Bearer {accountParameter.AccessToken}";
                rdParameters.LoidValue = accountParameter.Loid;
                rdParameters.SessionValue = accountParameter.SessionTracker;
                rdParameters.RefererValue = $"{HomePage}/user/{account.AccountBaseModel.UserName}/posts";

                // Assign Extra Header Parameters
                var rdRequestParameter = new RequestParameter(_httpHelper);
                rdRequestParameter.AddExtraHeaders(account, rdParameters);

                var url =
                    $"https://gateway.reddit.com/desktopapi/v1/user/{account.AccountBaseModel.UserName}/posts?rtj=debug&layout=classic&sort=new&allow_over18=&include=identity";
                var response = _httpHelper.GetRequest(url);
                var json2 = response.Response;
                var jsonobject = JObject.Parse(json2);
                var postedId = jsonobject["postIds"][0].ToString();
                postedId = Utils.GetRedditId(postedId);
                return postedId;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public PublisherPostUploadResponseHandler GetUploadedMediaId(DominatorAccountModel account,
            PaginationParameter accountParameter, string filePath, string fileType)
        {
            // Assign Authorization Parameters
            var rdParameters = new RdParameters();
            rdParameters.SessionValue = _httpHelper.GetRequestParameter().Cookies["session_tracker"]?.Value;
            //rdParameters.AuthorizationValue = $"Bearer {accountParameter.AccessToken}";
            rdParameters.LoidValue = _httpHelper.GetRequestParameter().Cookies["loid"]?.Value;
            rdParameters.OriginValue = "https://www.reddit.com";
            rdParameters.RefererValue = $"https://www.reddit.com/user/{account.UserName.Replace("@outlook.com", "")}/submit/?type=TEXT";
            #region Old Code

            //rdParameters.RdHostValue = "gateway.reddit.com";
            //if (string.IsNullOrEmpty(fileType))
            //    fileType = MediaUtilites.GetMimeTypeByFilePath(filePath);
            //// Assign Extra Header Parameters
            //var rdRequestParameter = new RequestParameter(_httpHelper);
            //rdRequestParameter.AddExtraHeaders(account, rdParameters);

            //var jsonElements = new RdPostMediaUploadJsonElements
            //{
            //    FilePath = filePath,
            //    FileType = fileType
            //};
            //var postData = rdRequestParameter.GetPostDataFromJson(jsonElements);
            //if (fileType.Contains("%2F"))
            //    postData = Encoding.UTF8.GetBytes($"filepath={filePath}&mimetype={fileType}");
            //var response1 = _httpHelper.PostRequest(RdConstants.UploadMediaOAuthUrl, postData);

            #endregion
            // Assign Extra Header Parameters
            var rdRequestParameter = new RequestParameter(_httpHelper);
            rdRequestParameter.AddExtraHeaders(account, rdParameters);
            var token =_httpHelper.GetRequestParameter().Cookies["csrf_token"]?.Value;
            var response1 = _httpHelper.PostRequest(RdConstants.GetFinalPostSubmitApi, Encoding.UTF8.GetBytes(RdConstants.GetPostDataForUserIdentity(token)));
            token = _httpHelper.GetRequestParameter().Cookies["csrf_token"]?.Value;
            response1 = _httpHelper.PostRequest(RdConstants.UploadMediaOAuthUrl1, Encoding.UTF8.GetBytes(RdConstants.PostDataForMediaUpload(token, !fileType.Contains("jpeg"))));
            var response = new PublisherPostUploadResponseHandler(response1);
            return response;
        }

        public Dictionary<string, string> UploadImageAndGetMediaId(PublisherPostUploadResponseHandler response,
            PublisherPostlistModel postDetails,
            DominatorAccountModel account, string contentType, string thumbnailPath = "")
        {
            try
            {
                var mediaId = string.Empty;
                var medialist = postDetails.MediaList.ToList();
                var mediaDictionary = new Dictionary<string, string>();

                if (!string.IsNullOrEmpty(thumbnailPath))
                    medialist = new List<string> { thumbnailPath };

                foreach (var media in medialist)
                {
                    // Initialize Header Parameters
                    var objParameter = new RequestParameter(_httpHelper);
                    var br = new BinaryReader(new FileStream(media, FileMode.Open, FileAccess.Read));
                    var buffer = br.ReadBytes((int)br.BaseStream.Length);
                    objParameter.IsMultiPart = true;
                    var filenameArray = Regex.Split(media, @"\\");
                    var fileName = filenameArray[filenameArray.Length - 1];
                    var objimage = new FileData(response.nameValueCollection, fileName, buffer);
                    objParameter.FileList.Add(media, objimage);
                    var postData = objParameter.GetPostDataFromParameters(contentType);
                    var url = $"https:{response.UrlPath}";
                    url = objParameter.GenerateUrl(url);
                    var request = _httpHelper.GetRequestParameter();
                    request.Accept = "*";
                    var host = response.UrlPath?.Replace("/", string.Empty);
                    request.Headers["Host"] = host;
                    request.Headers["Content-Type"] = objParameter.ContentType;
                    request.ContentType = objParameter.ContentType;
                    _httpHelper.SetRequestParameter(request);
                    var imageUploadResponse = _httpHelper.PostRequest(url, postData).Response;

                    if (imageUploadResponse.Contains("<Location>") && imageUploadResponse.Contains("</Location>"))
                        mediaId = Utilities.GetBetween(imageUploadResponse, "<Location>", "</Location>");

                    mediaDictionary.Add(Uri.UnescapeDataString(mediaId), contentType);
                }

                return mediaDictionary;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return new Dictionary<string, string>();
        }

        public RemoveVoteResponseHandler NewRemoveVote(DominatorAccountModel account, RedditPost post)
        {
            try
            {
                // Assign Authorization Parameters
                var rdParameters = new RdParameters { RefererValue = post.Permalink };
                rdParameters.SessionValue = _httpHelper.GetRequestParameter().Cookies["session_tracker"]?.Value;
                rdParameters.LoidValue = _httpHelper.GetRequestParameter().Cookies["loid"]?.Value;
                rdParameters.OriginValue = $"{RdConstants.GetRedditHomePageAPI}";
                rdParameters.RefererValue = post.Permalink;

                // Assign Extra Header Parameters
                var rdRequestParameter = new RequestParameter(_httpHelper);
                rdRequestParameter.AddExtraHeaders(account, rdParameters);

                var csrftoken = _httpHelper.GetRequestParameter().Cookies["csrf_token"]?.Value;
                var removeVoteResponse = _httpHelper.PostRequest(RdConstants.GetFinalPostSubmitApi, Encoding.UTF8.GetBytes(RdConstants.GetPostDataForUserIdentity(csrftoken)));
                csrftoken = _httpHelper.GetRequestParameter().Cookies["csrf_token"]?.Value;
                removeVoteResponse = _httpHelper.PostRequest(RdConstants.GraphQlAPI, Encoding.UTF8.GetBytes(RdConstants.GetPostDataForVote(post.Id, csrftoken, false, false, !string.IsNullOrEmpty(post.Commenttext))));
                var response = new RemoveVoteResponseHandler(removeVoteResponse);
                return response;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public DownVoteResponseHandler NewDownvote(DominatorAccountModel account, RedditPost post)
        {
            try
            {
                // Assign Authorization Parameters
                var rdParameters = new RdParameters { RefererValue = post.Permalink };
                rdParameters.SessionValue = _httpHelper.GetRequestParameter().Cookies["session_tracker"]?.Value;
                rdParameters.LoidValue = _httpHelper.GetRequestParameter().Cookies["loid"]?.Value;
                rdParameters.OriginValue = $"{RdConstants.GetRedditHomePageAPI}";
                rdParameters.RefererValue = post.Permalink;

                // Assign Extra Header Parameters
                var rdRequestParameter = new RequestParameter(_httpHelper);
                rdRequestParameter.AddExtraHeaders(account, rdParameters);

                var csrftoken = _httpHelper.GetRequestParameter().Cookies["csrf_token"]?.Value;
                var downVoteResponse = _httpHelper.PostRequest(RdConstants.GetFinalPostSubmitApi, Encoding.UTF8.GetBytes(RdConstants.GetPostDataForUserIdentity(csrftoken)));
                csrftoken = _httpHelper.GetRequestParameter().Cookies["csrf_token"]?.Value;
                if (string.IsNullOrEmpty(post.Commenttext))
                    downVoteResponse = _httpHelper.PostRequest(RdConstants.GraphQlAPI, Encoding.UTF8.GetBytes(RdConstants.GetPostDataForVote(post.Id, csrftoken, false, true)));
                else
                    downVoteResponse = _httpHelper.PostRequest(RdConstants.GraphQlAPI, Encoding.UTF8.GetBytes(RdConstants.GetPostDataForVote(post.Id, csrftoken, false, true, true)));
                var response = new DownVoteResponseHandler(downVoteResponse);
                return response;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public PaginationParameter GetPaginationParameter(DominatorAccountModel account)
        {
            var accountParameters = new PaginationParameter();
            try
            {
                var response = _httpHelper.GetRequest(HomePage);
                var jsonReponse = RdConstants.GetJsonPageResponse(response.Response);
                var jsonHand = new JsonHandler(jsonReponse);
                accountParameters.SessionTracker = jsonHand.GetElementValue("user", "sessionTracker");
                accountParameters.Reddaid = jsonHand.GetElementValue("user", "reddaid");
                var loid = jsonHand.GetElementValue("user", "loid", "loid");
                var blob = jsonHand.GetElementValue("user", "loid", "blob");
                var created = jsonHand.GetElementValue("user", "loid", "loidCreated");
                var version = jsonHand.GetElementValue("user", "loid", "version");
                accountParameters.Loid = loid + "." + version + "." + created + "." + blob;
                accountParameters.AccessToken = jsonHand.GetElementValue("user", "session", "accessToken");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return accountParameters;
        }

        public void SubscribeQuarantineCommunity(DominatorAccountModel account,
            SubredditResponseHandler subredditResponseHandler, string queryValue)
        {
            if (subredditResponseHandler == null)
                subredditResponseHandler =
                    new SubredditResponseHandler(_httpHelper.GetRequest(HomePage), false, null);

            // Assign Authorization Parameters
            var rdParameters = new RdParameters();
            rdParameters.AuthorizationValue = $"Bearer {subredditResponseHandler.PaginationParameter.AccessToken}";
            rdParameters.LoidValue = subredditResponseHandler.PaginationParameter.Loid;
            rdParameters.SessionValue = subredditResponseHandler.PaginationParameter.SessionTracker;
            rdParameters.RdHostValue = "oauth.reddit.com";

            // Assign Extra Header Parameters
            var rdRequestParameter = new RequestParameter(_httpHelper);
            rdRequestParameter.AddExtraHeaders(account, rdParameters);

            var subreddit = queryValue.Contains(HomePage)
                ? Regex.Replace(queryValue, $"{HomePage}/r/", "").Replace("/", "")
                : Regex.Replace(queryValue, "/r/", "").Replace("/", "");

            var jsonElements = new RdJsonElement
            {
                SrName = subreddit,
                ApiType = "json",
                Accept = "true"
            };

            var postData = rdRequestParameter.GetPostDataFromJson(jsonElements);
            _httpHelper.PostRequest(RdConstants.QuarantineUrl, postData);
        }

        public RequestParameters SetRequestParametersAndProxy(DominatorAccountModel accountModel)
        {
            try
            {
                var userAgent = RdConstants.UserAgentValue;
                var requestParameters = new RequestParameters();
                requestParameters.Headers.Add("Host", "www.reddit.com");
                requestParameters.Headers.Add("Origin", HomePage);
                requestParameters.Headers["Content-Type"] = RdConstants.ContentType;
                requestParameters.Headers.Add("User-Agent", userAgent);
                requestParameters.Headers["Accept"] = RdConstants.AcceptType;

                requestParameters.KeepAlive = RdConstants.KeepAlive;
                requestParameters.Accept = RdConstants.AcceptType;
                requestParameters.UserAgent = userAgent;
                requestParameters.ContentType = RdConstants.ContentType;

                try
                {
                    requestParameters.Proxy = accountModel.AccountBaseModel.AccountProxy;
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                requestParameters.Cookies = new CookieCollection();
                return requestParameters;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public async Task<bool> RedditAds(IResponseParameter accloginResp, DominatorAccountModel dominatorAccountModel, string DestinationUrl, int ScrollCount = 15, bool IsSubreddit = false)
        {
            var isSuspended = false;
            var paginationParameter = new PaginationParameter();
            var postResponse = new RedditAdsResponseHandler(accloginResp, false, paginationParameter, IsSubreddit);
            var paginationValues = postResponse.PaginationParameter;
            if (postResponse == null || postResponse.LstRedditPost.Count == 0)
                return false;
            //Creating new Object of RdHttpHelper to set requestParameter for httpHelerForAds only
            var httpHelperForAd = _accountScopeFactory[$"{dominatorAccountModel.AccountId}_AdScraper"]
                .Resolve<IRdHttpHelper>();
            var reqForAd = httpHelperForAd.GetRequestParameter();
            reqForAd.Headers["Host"] = "www.reddit.com";
            reqForAd.Headers["Origin"] = DestinationUrl;
            reqForAd.Cookies = dominatorAccountModel.Cookies;
            reqForAd.Accept = RdConstants.AcceptType;
            reqForAd.ContentType = RdConstants.ContentType;
            reqForAd.UserAgent = RdConstants.UserAgentValue;
            reqForAd.KeepAlive = true;
            reqForAd.Proxy = dominatorAccountModel.AccountBaseModel.AccountProxy;
            httpHelperForAd.SetRequestParameter(reqForAd);

            var location = GetIpDetails(dominatorAccountModel, httpHelperForAd);
            //SaveUserDetailsInDb(dominatorAccountModel, location);
            var ipValue = GetIpValue(dominatorAccountModel, httpHelperForAd);
            foreach (var post in postResponse.LstRedditPost)
                if (post.IsSponsored && (post.MediaType.Equals("image") || post.MediaType.Equals("video") && !string.IsNullOrEmpty(post.VideoUrl)))
                    SaveAdsForFirstPage(dominatorAccountModel, post, location, ipValue);
            var i = 1;
            while (!string.IsNullOrEmpty(postResponse.PaginationParameter.LastPaginationId) && i++ < ScrollCount)
            {
                if (postResponse.LstRedditPost.Count == 0 || postResponse.Response.Contains("Authorization Required"))
                    break;
                // Assign Authorization Parameters
                paginationValues.LastPaginationId = postResponse.PaginationParameter.LastPaginationId;
                var rdParameters = GetRdParameters(paginationValues);
                rdParameters.RdHostValue = "gql.reddit.com";

                // Assign Extra Header Parameters              
                var req = httpHelperForAd.GetRequestParameter();
                req.Headers[RdConstants.AuthorizationKey] = rdParameters.AuthorizationValue;
                req.Headers[RdConstants.LoidKey] = rdParameters.LoidValue;
                req.Headers[RdConstants.RdSessionKey] = rdParameters.SessionValue;
                req.Headers["Sec-Fetch-Mode"] = "cors";
                req.Headers["x-reddaid"] = "ZGRYJ3MVXX7ZAGIA";
                req.Headers["Sec-Fetch-Site"] = "same-site";
                req.Headers["Referer"] = DestinationUrl;
                req.Headers["Accept"] = "*/*";
                req.Headers["Connection"] = "keep-alive";
                req.ContentType = "application/json";
                httpHelperForAd.SetRequestParameter(req);

                var currentTimeStamp = DateTime.Now.GetCurrentEpochTimeMilliSeconds();
                var redditAdsPostUrl = $"https://gql.reddit.com/?request_timestamp={currentTimeStamp}";

                var lastPostId = postResponse.PaginationParameter.LastPaginationId.Base64Encode();
                RdAdJsonElement redditAdsPostData = null;
                if (IsSubreddit)
                {
                    redditAdsPostData = new RdAdJsonElement
                    {
                        ID = "e111e3a11997",
                        AdsVariables = new AdsVariables
                        {
                            RecentPostIds = new string[0],
                            SubredditNames = new string[0],
                            Name = DestinationUrl.Split('/').LastOrDefault(y => !string.IsNullOrEmpty(y)),
                            AdContext = new AdContext
                            {
                                layout = "CARD"
                            },
                            IncludeIdentity = false,
                            IncludeFeatured = true,
                            IncludeTrending = true,
                            IncludeSubRedditRanking = true,
                            IncludeSubredditChannels = true,
                            IncludeRecents = false,
                            IsAdHocMulti = false,
                            IsAll = false,
                            IsLoggedOutGatedOptedin = false,
                            IsLoggedOutQuarantineOptedin = false,
                            IsPopular = false,
                            IsFake = false,
                            IncludeDevPlatformMetadata = true,
                            Sort = "HOT",
                            PageSize = 25,
                            After = lastPostId
                        }
                    };
                }
                else
                {
                    redditAdsPostData = new RdAdJsonElement
                    {
                        ID = "a7d66b2939ee",
                        AdsVariables = new AdsVariables
                        {
                            RecentPostIds = new string[0],

                            IncludeIdentity = false,
                            IncludeFeatured = true,
                            AdContext = new AdContext
                            {
                                layout = "CARD",
                                Reddaid = "ZGRYJ3MVXX7ZAGIA"
                            },
                            Sort = "TOP",
                            Range = "DAY",
                            PageSize = 25,
                            After = lastPostId
                        }
                    };
                }
                var rdRequestParameter = new RequestParameter();
                var adsPostData = Encoding.UTF8.GetBytes(rdRequestParameter.GetJsonString(redditAdsPostData));

                var postResponseAd = await httpHelperForAd.PostRequestAsync(redditAdsPostUrl, adsPostData, dominatorAccountModel.Token);
                postResponse = new RedditAdsResponseHandler(postResponseAd, true, paginationParameter, IsSubreddit);
                foreach (var post in postResponse.LstRedditPost)
                    if (post.IsSponsored && (post.MediaType.Equals("image") || post.MediaType.Equals("video") && !string.IsNullOrEmpty(post.VideoUrl)))
                        SaveAdsForNextPage(dominatorAccountModel, post, location, ipValue);
                await Task.Delay(TimeSpan.FromSeconds(RandomUtilties.GetRandomNumber(30, 15)));
            }
            return isSuspended;
        }
        private string GetIpValue(DominatorAccountModel dominatorAccountModel, IRdHttpHelper rdHttpHelper, bool GetMyIp = false)
        {
            if (string.IsNullOrEmpty(dominatorAccountModel.AccountBaseModel.AccountProxy.ProxyIp) || GetMyIp)
            {
                var ip = rdHttpHelper.GetRequest(RdConstants.GetMyIPAPI).Response;
                ip = Utilities.GetBetween(ip, "pti-header bgm-green\">", "/h2>");
                ip = Utilities.GetBetween(ip, ">", "<").Trim();
                return ip;
            }
            else
                return dominatorAccountModel.AccountBaseModel.AccountProxy.ProxyIp;
        }

        public bool IsSuspended(IRdHttpHelper httpHelperForAd, string destinationUrl)
        {
            return httpHelperForAd.GetRequest(destinationUrl).Response.Contains(RdConstants.SuspendedMessage);
        }

        private RdParameters GetRdParameters(PaginationParameter paginationParameter)
        {
            try
            {
                var rdParameters = new RdParameters();
                rdParameters.RdHostValue = "oauth.reddit.com";
                rdParameters.AuthorizationValue = $"Bearer {paginationParameter.AccessToken}";
                rdParameters.LoidValue = paginationParameter.Loid;
                rdParameters.SessionValue = paginationParameter.SessionTracker;
                return rdParameters;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private void SaveAdsForFirstPage(DominatorAccountModel dominatorAccountModel, RedditPost redditPost,
            Dictionary<IpLocationDetails, string> location, string ipValue)
        {
            var adDetailModel = new AdDetailModel();
            try
            {
                var jObject = JObject.Parse(redditPost.Source);
                adDetailModel.NewsFeedDescription = jObject["displayText"]?.ToString();
                adDetailModel.DestinationUrl = jObject["url"]?.ToString();
                adDetailModel.Preview = Utilities.GetBetween(redditPost.Preview, "\"url\": \"", "\"");

                if (redditPost.MediaType.Equals("video"))
                {
                    var outboundUrl = Utilities.GetBetween(redditPost.Source, "\"outboundUrl\": \"", "\"");

                    if (!string.IsNullOrEmpty(outboundUrl))
                        adDetailModel.DestinationUrl = outboundUrl;
                    else
                        adDetailModel.DestinationUrl =
                            "https://www." + Utilities.GetBetween(redditPost.Source, "\"displayText\": \"", "\"");

                    adDetailModel.Preview = string.IsNullOrEmpty(redditPost.Preview) ? Utilities.GetBetween(redditPost.Source, "\"url\": \"", "\"") +
                                            $"/DASH_{redditPost.MediaResolution}" : Utils.GetBetween(redditPost.Preview, "\"url\": \"", "\"");
                }

                SaveAdData(dominatorAccountModel, redditPost, location, ipValue, adDetailModel);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private void SaveAdsForNextPage(DominatorAccountModel dominatorAccountModel, RedditPost redditPost,
            Dictionary<IpLocationDetails, string> location, string ipValue)
        {
            var adDetailModel = new AdDetailModel();
            try
            {
                adDetailModel.NewsFeedDescription = redditPost.DominForAd;
                adDetailModel.DestinationUrl = redditPost.Source;
                adDetailModel.Preview = redditPost.Preview;

                if (redditPost.MediaType.Equals("video"))
                {
                    adDetailModel.DestinationUrl = "https://" + redditPost.DominForAd;
                    adDetailModel.Preview = string.IsNullOrEmpty(redditPost.Preview) ? redditPost.Source + $"/DASH_{redditPost.MediaResolution}" : redditPost.Preview;
                }

                SaveAdData(dominatorAccountModel, redditPost, location, ipValue, adDetailModel);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public void SaveAdData(DominatorAccountModel dominatorAccountModel, RedditPost redditPost,
            Dictionary<IpLocationDetails, string> location, string ipValue, AdDetailModel adDetailModel)
        {
            try
            {
                //const string apiUrl = "https://reddit.dev.poweradspy.com/redditAdsData";
                var redditAdId = redditPost.PostId.Split('_')[1];
                var base64EncodedImage_Video = string.Empty;

                if (redditPost.MediaType.Equals("video"))
                {
                    //base64EncodedImage_Video = adDetailModel.Preview;
                    base64EncodedImage_Video = adDetailModel.Preview;
                    adDetailModel.Preview = redditPost.VideoUrl.Replace("DASH_96", $"DASH_360").Replace("DASH_64", $"DASH_360");

                }
                else if (redditPost.MediaType.Equals("image"))
                {
                    if (adDetailModel.Preview.Contains("url\": \""))
                        adDetailModel.Preview = Utilities.GetBetween(adDetailModel.Preview, "url\": \"", "\",");
                    var downLoadImage = MediaUtilites.GetImageBytesFromUrl(adDetailModel.Preview);
                    base64EncodedImage_Video = Convert.ToBase64String(downLoadImage, 0, downLoadImage.Length);
                }
                else
                {
                    base64EncodedImage_Video = null;
                    adDetailModel.Preview = null;
                    redditPost.MediaType = "TEXT";
                }

                var adsData = new SortedDictionary<string, string>
                {
                    {"image_video_url_original", adDetailModel.Preview},
                    {"ad_id", redditAdId},
                    {"ad_position", "FEED"},
                    {"ad_text", ""},
                    {"ad_title", redditPost.Title},
                    {"ad_url", redditPost.Permalink},
                    {"call_to_action", redditPost.CallToAction},
                    {"category", ""},
                    {"city", location[IpLocationDetails.City]},
                    {"comment",redditPost.NumComments.ToString()},
                    {"country", location[IpLocationDetails.Country]},
                    {"destination_url", adDetailModel.DestinationUrl},
                    {"ip_address", ipValue},
                    {"likes", redditPost.Score.ToString()},
                    {"lower_age", "20"},
                    {"news_feed_description", adDetailModel.NewsFeedDescription},
                    {"platform", "2"},
                    {"post_date", redditPost.Created.ToString()},
                    {"post_owner", redditPost.Author},
                    {"post_owner_image", ""},
                    {"reddit_id", dominatorAccountModel.UserName},
                    {"share",redditPost.NumCrossposts.ToString()},
                    {"state", location[IpLocationDetails.State]},
                    {"type", redditPost.MediaType.ToUpper()},
                    {"upper_age", "65"},
                    {"version", "1.0.1"},
                    {"source", "desktop"},
                    {"image_video_url", base64EncodedImage_Video}
                };

                var result = string.Empty;
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(RdConstants.RedditAdsDataMain);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";
                httpWebRequest.KeepAlive = true;

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    var serializedData = JsonConvert.SerializeObject(adsData, Formatting.Indented);
                    streamWriter.Write(serializedData);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    result = streamReader.ReadToEnd();
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public string ConvertAlphaToNumber(string postId)
        {
            var digits = "01234567890123456789012345";
            var alphas = "abcdefghijklmnopqrstuvwxyz";
            var result = string.Empty;
            foreach (var c in postId.ToCharArray())
            {
                var pos = alphas.IndexOf(c);
                result += pos == -1 ? c : digits.ElementAt(pos);
            }

            return result;
        }

        /// <summary>
        ///     To get the user location using ip
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public Dictionary<IpLocationDetails, string> GetIpDetails(DominatorAccountModel account,
            IRdHttpHelper httpHelperForAds)
        {
            var lockerRequest = new object();
            var dictIpDetails = new Dictionary<IpLocationDetails, string>();
            var jsonHandler = JsonJArrayHandler.GetInstance;
            var proxyLocationDetails = string.Empty;
            try
            {
                lock (lockerRequest)
                {
                    try
                    {
                        _delayService.ThreadSleep(2000);
                        var objProxy = new Proxy
                        {
                            ProxyIp = account.AccountBaseModel.AccountProxy.ProxyIp,
                            ProxyPort = account.AccountBaseModel.AccountProxy.ProxyPort,
                            ProxyUsername = account.AccountBaseModel.AccountProxy.ProxyUsername,
                            ProxyPassword = account.AccountBaseModel.AccountProxy.ProxyPassword
                        };

                        var parameter = httpHelperForAds.GetRequestParameter();

                        parameter.Proxy = objProxy;
                        var ip = GetIpValue(account, httpHelperForAds);
                        try
                        {
                            //var locationUrl = "https://api.db-ip.com/v2/eb79c26170d0e9921e5b8372b2e212f86afa399c/" + ip.Trim();
                            var locationUrl = RdConstants.GetLocationAPI(ip);
                            proxyLocationDetails = httpHelperForAds.GetRequest(locationUrl).Response;
                            if (proxyLocationDetails.Contains("INVALID_ADDRESS"))
                            {
                                ip = GetIpValue(account, httpHelperForAds, true);
                                locationUrl = RdConstants.GetLocationAPI(ip);
                                proxyLocationDetails = httpHelperForAds.GetRequest(locationUrl).Response;
                            }
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }

                        //To get district,city , state and country
                        //var district = Utilities.GetBetween(proxyLocationDetails, "district\": \"", "\"");
                        var jsonObject = jsonHandler.ParseJsonToJObject(proxyLocationDetails);
                        var city = jsonHandler.GetJTokenValue(jsonObject, "city");
                        var state = jsonHandler.GetJTokenValue(jsonObject, "regionName");
                        var country = jsonHandler.GetJTokenValue(jsonObject, "country");
                        //Added the information into dictionary
                        dictIpDetails.Add(IpLocationDetails.City, !string.IsNullOrEmpty(city) ? city : string.Empty);
                        dictIpDetails.Add(IpLocationDetails.State, !string.IsNullOrEmpty(state) ? state : string.Empty);
                        dictIpDetails.Add(IpLocationDetails.Country,
                            !string.IsNullOrEmpty(country) ? country : string.Empty);
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return dictIpDetails;
        }

        private string CaptchaSolver(RedditUser redditUser)
        {
            var captchaKeyResponse = string.Empty;
            var firstUrl = $"{HomePage}/message/compose/?to={redditUser.DisplayName}";
            var firstResp = _httpHelper.GetRequest(firstUrl);
            try
            {
                if (firstResp.Response.Contains("recaptcha"))
                {
                    var genericFileManager = InstanceProvider.GetInstance<IGenericFileManager>();
                    var imageCaptchaServicesModel =
                        genericFileManager.GetModel<ImageCaptchaServicesModel>(ConstantVariable
                            .GetImageCaptchaServicesFile()) ?? new ImageCaptchaServicesModel();

                    var getCaptchaResultFirstUrl =
                        "http://captchatypers.com/captchaapi/UploadRecaptchav1.ashx?action=UPLOADCAPTCHA&username=" +
                        $"{imageCaptchaServicesModel.UserName}" + "&password=" +
                        $"{imageCaptchaServicesModel.Password}" +
                        "&googlekey=6LeTnxkTAAAAAN9QEuDZRpn90WwKk_R1TRW_g-JC&pageurl=" + firstUrl;

                    var captchaIdGotFromImageTyperz = _httpHelper.GetRequest(getCaptchaResultFirstUrl);

                    if (!string.IsNullOrEmpty(captchaIdGotFromImageTyperz.Response))
                    {
                        var getCaptchaResultSecondUrl =
                            "http://captchatypers.com/captchaapi/GetRecaptchaText.ashx?action=GETTEXT&username=" +
                            $"{imageCaptchaServicesModel.UserName}" + "&password=" +
                            $"{imageCaptchaServicesModel.Password}" + "&Captchaid=" +
                            captchaIdGotFromImageTyperz.Response;

                        while (string.IsNullOrEmpty(captchaKeyResponse) ||
                               captchaKeyResponse.Contains("ERROR: NOT_DECODED"))
                        {
                            var responseCaptchaResultSecondUrl = _httpHelper.GetRequest(getCaptchaResultSecondUrl);
                            captchaKeyResponse = responseCaptchaResultSecondUrl.Response;
                            if (responseCaptchaResultSecondUrl.Response.Contains("ERROR: IMAGE_TIMED_OUT"))
                            {
                                Console.WriteLine(@"Time Out");
                                return string.Empty;
                            }

                            _delayService.ThreadSleep(3 * 1000);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return captchaKeyResponse;
        }

        #region Publisher

        public PublishPostResponseHandler PublishPost(DominatorAccountModel account, PublishPostModel postDetails,
            PaginationParameter accountParameters, OtherConfigurationModel otherConfiguration)
        {
            PublishPostResponseHandler response = null;

            // Assign Authorization Parameters
            var rdParameters = new RdParameters();
            rdParameters.SessionValue = accountParameters.SessionTracker;
            rdParameters.AuthorizationValue = $"Bearer {accountParameters.AccessToken}";
            rdParameters.RefererValue = accountParameters.LastPaginationId;
            rdParameters.LoidValue = accountParameters.Loid;
            rdParameters.RdHostValue = "oauth.reddit.com";

            // Assign Extra Header Parameters
            var rdRequestParameter = new RequestParameter(_httpHelper);
            rdRequestParameter.AddExtraHeaders(account, rdParameters);

            try
            {
                var jsonElements = new RdPostJsonElements
                {
                    Category = postDetails.Category,
                    Nsfw = postDetails.Nsfw,
                    OriginalCcontent = postDetails.OriginalContent,
                    Spoiler = postDetails.Spoiler,
                    ApiType = postDetails.ApiType,
                    Title = Uri.EscapeDataString(postDetails.Title),
                    Kind = Regex.Split(postDetails.Kind, "/")[0],
                    SubmitType = postDetails.SubmitType,
                    Sendreplies = postDetails.Sendreplies,
                    ValidateOnSubmit = postDetails.ValidateOnSubmit,
                    Sr = postDetails.Sr
                };

                if (postDetails.Url != null)
                {
                    jsonElements.Url = Uri.EscapeDataString(postDetails.Url);
                }

                else
                {
                    // If postDetails.Text containing signature text and Assingning JArray inside JObject into document
                    if (!string.IsNullOrEmpty(postDetails.Text))
                    {
                        if (postDetails.Text.Contains("\r\n"))
                        {
                            var regex = Regex.Split(postDetails.Text, "\r\n");

                            var document = new List<JObject>();

                            foreach (var item in regex)
                            {
                                var cJObject = new List<JObject>
                                {
                                    new JObject(new JProperty("e", "text"), new JProperty("t", item))
                                };

                                var documentJObject = new JObject
                                {
                                    new JProperty("e", "par"), new JProperty("c", cJObject)
                                };

                                document.Add(documentJObject);
                            }

                            var documentObject = JObject.FromObject(new { document },
                                new JsonSerializer { NullValueHandling = NullValueHandling.Ignore });

                            jsonElements.RichTextJson =
                                Uri.EscapeDataString(JsonConvert.SerializeObject(documentObject));
                        }
                        // If postDetails.Text donot containing singnature text
                        else
                        {
                            jsonElements.Text = postDetails.Text;
                        }
                    }
                }

                if (postDetails.Kind.Contains("video"))
                    jsonElements.VideoPosterURL = postDetails.ThumbnailUrl;

                var requestParameters = _httpHelper.GetRequestParameter();
                var postData = rdRequestParameter.GetPostDataFromJson(jsonElements);
                requestParameters.Referer = $"{HomePage}/user/{postDetails.Sr}/submit";
                _httpHelper.SetRequestParameter(requestParameters);
                var response1 = _httpHelper.PostRequest(RdConstants.SubmitOAuthUrl, postData);

                response = new PublishPostResponseHandler(response1);

                if (!string.IsNullOrEmpty(response.FailureMessage))
                {
                    var delayMin = Regex.Match(response.FailureMessage, @"\d+")?.Value;

                    if (!string.IsNullOrEmpty(delayMin))
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage, account.AccountBaseModel.AccountNetwork,
                            account.UserName, "Publish",
                            $"Please Wait - Activity will starts after {int.Parse(delayMin) + 2} minute, " +
                            "As we got response from website");

                        _delayService.DelayAsync(TimeSpan.FromMinutes(int.Parse(delayMin) + 2)).Wait();

                        response1 = _httpHelper.PostRequest(RdConstants.SubmitOAuthUrl, postData);
                        response = new PublishPostResponseHandler(response1);
                    }
                }

                // For Approve Post during publishing
                if (otherConfiguration.IsCheckedForApprovePost)
                    ApprovePublishPost(account, response1, postDetails, accountParameters);
            }
            catch (AssertFailedException)
            {
                throw;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return response;
        }

        public PublishPostResponseHandler PublishPostCrossPost(DominatorAccountModel account,
            PublishPostModel postDetails, PaginationParameter accountParameters)
        {
            PublishPostResponseHandler response = null;
            try
            {
                // Assign Authorization Parameters
                var rdParameters = GetRdParameters(accountParameters);

                // Initialize Header Parameters & Assign Extra Header Values
                var rdRequestParameter = new RequestParameter(_httpHelper);
                rdRequestParameter.AddExtraHeaders(account, rdParameters);
                var req = _httpHelper.GetRequestParameter();
                req.Headers.Add("Sec-Fetch-Mode", "cors");
                req.Headers.Add("Sec-Fetch-Site", "same-site");
                _httpHelper.SetRequestParameter(req);

                var jsonElements = new RdPostJsonElements
                {
                    PostToTwitter = "false",
                    Nsfw = postDetails.Nsfw,
                    OriginalCcontent = postDetails.OriginalContent,
                    Spoiler = postDetails.Spoiler,
                    ApiType = postDetails.ApiType,
                    Title = Uri.EscapeDataString(postDetails.Title),
                    Kind = "crosspost",
                    SubmitType = postDetails.SubmitType,
                    Sendreplies = true,
                    ValidateOnSubmit = postDetails.ValidateOnSubmit,
                    Sr = postDetails.Sr,
                    CrossPostId = postDetails.CrosspostFullname,
                    ShowErrorList = "true"
                };
                var postData = rdRequestParameter.GetPostDataFromJson(jsonElements);
                var response1 = _httpHelper.PostRequest(RdConstants.CrossPostPublishUrl, postData);
                response = new PublishPostResponseHandler(response1);
            }
            catch (AssertFailedException)
            {
                throw;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return response;
        }

        public SubmitPageResponseHandler GetSubmitPage(DominatorAccountModel account,
            PaginationParameter accountParameters)
        {
            // Assign Authorization Parameters
            var rdParameters = new RdParameters();
            rdParameters.SessionValue = accountParameters.SessionTracker;
            rdParameters.AuthorizationValue = $"Bearer {accountParameters.AccessToken}";
           // rdParameters.RefererValue = $"{HomePage}/submit";
            rdParameters.RefererValue = $"https://www.reddit.com/user/{account.UserName.Replace("@outlook.com","")}/submit?type=TEXT";
            rdParameters.LoidValue = accountParameters.Loid;

            // Assign Extra Header Parameters
            var requestParameter = new RequestParameter(_httpHelper);
            requestParameter.AddExtraHeaders(account, rdParameters);

            //var response1 = _httpHelper.GetRequest(RdConstants.SubmitPageGatewayUrl);
            var response1 = _httpHelper.GetRequest($"https://www.reddit.com/user/{account.UserName.Replace("@outlook.com", "")}/submit?type=TEXT");
            var response = new SubmitPageResponseHandler(response1);
            return response;
        }

        public SubRedditSubmitValidationResponseHandler GetSubRedditSubmitValidation(DominatorAccountModel account,
            PaginationParameter accountParameters, string subRedditId)
        {
            // Assign Authorization Parameters
            var rdParameters = new RdParameters();
            rdParameters.SessionValue = accountParameters.SessionTracker;
            rdParameters.AuthorizationValue = $"Bearer {accountParameters.AccessToken}";
            rdParameters.RefererValue = $"{HomePage}/r/{subRedditId}/submit";
            rdParameters.LoidValue = accountParameters.Loid;
            rdParameters.RdHostValue = "gateway.reddit.com";

            // Assign Extra Header Parameters
            var rdRequestParameter = new RequestParameter(_httpHelper);
            rdRequestParameter.AddExtraHeaders(account, rdParameters);

            var response1 = _httpHelper.GetRequest(RdConstants.SubmitPageGatewayUrl);
            var response = new SubRedditSubmitValidationResponseHandler(response1);
            return response;
        }

        public ActivityResposneHandler BroadCastMessage(ActivityType activityType, DominatorAccountModel dominatorAccount, RedditUser redditUser)
        {
            var Response = ActivityResposneHandler.GetInstance;
            try
            {
                var AuthorizationToken = GetAndSetAuthorizationToken(redditUser);
                int i = 0;
            TryAgain:
                Thread.Sleep(8000);
                dominatorAccount.Token.ThrowIfCancellationRequested();
                var CreateRoomResponse = _httpHelper.PostRequest(RdConstants.CreateChatRoomAPI, RdConstants.CreateChatRoomPostData(redditUser.Id)).Response;
                var RoomID = Utils.GetBetween(CreateRoomResponse, "\"com.reddit.existing_room_id\":\"", "\"}");
                RoomID = string.IsNullOrEmpty(RoomID) ? Utils.GetBetween(CreateRoomResponse, "{\"room_id\":\"", "\"}") : RoomID;
                RoomID = string.IsNullOrEmpty(RoomID) ? redditUser.ThreadID : RoomID;
                if (string.IsNullOrEmpty(RoomID) && i++ <= 1 && !string.IsNullOrEmpty(CreateRoomResponse) && !CreateRoomResponse.Contains("\"error\":\"User is flagged for spam\""))
                    goto TryAgain;
                if(!string.IsNullOrEmpty(CreateRoomResponse) && CreateRoomResponse.Contains("\"error\":\"User is flagged for spam\""))
                {
                    Response.Status = false;
                    Response.ResponseMessage = Utils.GetBetween(CreateRoomResponse, "\"error\":\"", "\"");
                    return Response;
                }
                i = 0;
            TryAgain1:
                var postData = Encoding.UTF8.GetBytes(RdConstants.GetSendMessagePostData(redditUser.Text.Trim()));
                var request = (HttpWebRequest)WebRequest.Create(RdConstants.GetSendMessageAPI(RoomID));
                SetParameterForPutRequest(ref request, _httpHelper.GetRequestParameter());
                var BroadCastMessageResponse = PutRequest(request, AuthorizationToken, postData);
                if (string.IsNullOrEmpty(BroadCastMessageResponse) && i++ <= 1)
                    goto TryAgain1;
                var eventId = Utils.GetBetween(BroadCastMessageResponse, "{\"event_id\":\"", "\"}");
                Response.Status = !string.IsNullOrEmpty(eventId);
                if (Response.Status)
                    _httpHelper.PostRequest(RdConstants.GetMarkAsReadAPI(RoomID), RdConstants.MarkAsReadPostData(eventId));
                Response.ResponseMessage = string.IsNullOrEmpty(eventId) ? "Failed" : "Success";
            }
            catch (Exception ex) { ex.DebugLog(); Response.Status = false; Response.ResponseMessage = "Failed Due To Error"; }
            return Response;
        }

        private string GetAndSetAuthorizationToken(RedditUser redditUser)
        {
            var tokenResponse = _httpHelper.GetRequest($"https://chat.reddit.com/?redditUserId={redditUser.Id}").Response;
            var token = Utils.GetBetween(RdConstants.GetDecodedResponse(tokenResponse, true, true), "token=\"{\"token\":\"", "\",\"expires\":");
            Thread.Sleep(10000);
            var login = _httpHelper.PostRequest("https://matrix.redditspace.com/_matrix/client/r0/login", $"{{\"type\":\"com.reddit.token\",\"token\":\"{token}\",\"device_id\":\"{Guid.NewGuid().ToString()}\",\"initial_device_display_name\":\"Reddit Web Client\"}}").Response;
            token = Utils.GetBetween(login, "\"access_token\":\"", "\",\"home_server\":");
            _httpHelper.GetRequestParameter().Headers["Authorization"] = $"Bearer {token}";
            return token;
        }

        private string PutRequest(HttpWebRequest request, string token, byte[] postData)
        {
            try
            {
                request.Method = "PUT";
                request.ContentLength = postData.Length;
                request.CookieContainer = _httpHelper.Request.CookieContainer;
                request.Headers["Authorization"] = $"Bearer {token}";
                using (var stream = request.GetRequestStream())
                {
                    stream.Write(postData, 0, postData.Length);
                }
                return new StreamReader(request.GetResponse().GetResponseStream()).ReadToEnd();
            }
            catch (Exception )
            {
                return string.Empty;
            }
        }

        private void SetParameterForPutRequest(ref HttpWebRequest webRequest, IRequestParameters requestParameter)
        {
            try
            {
                if (requestParameter == null)
                    return;

                #region Set the Headers

                webRequest.Headers = new WebHeaderCollection();

                if (requestParameter.Headers != null)
                    foreach (var eachHeader in requestParameter.Headers)
                        try
                        {
                            var headerName = eachHeader.ToString();

                            var headerValue = requestParameter.Headers[headerName];

                            if (headerName == "X-CSRFToken")
                            {
                                var token = requestParameter.Cookies["csrftoken"]?.Value;
                                webRequest.Headers.Add(eachHeader.ToString(), token);
                            }
                            else
                            {
                                if (!WebHeaderCollection.IsRestricted(headerName))
                                    webRequest.Headers.Add(headerName, headerValue);
                            }
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }

                webRequest.Host = webRequest.RequestUri.Host;
                webRequest.KeepAlive = requestParameter.KeepAlive;
                webRequest.UserAgent = requestParameter.UserAgent;
                webRequest.ContentType = requestParameter.ContentType;
                webRequest.Referer = requestParameter.Referer;
                webRequest.Accept = requestParameter.Accept;

                #endregion

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                                                       | SecurityProtocolType.Tls11
                                                       | SecurityProtocolType.Tls12
                                                       | SecurityProtocolType.Ssl3;

                if (ServicePointManager.Expect100Continue) ServicePointManager.Expect100Continue = false;

                #region Set the Cookies

                if (requestParameter.Cookies != null)
                {
                    webRequest.CookieContainer = new CookieContainer();

                    foreach (Cookie eachCookie in requestParameter.Cookies)
                        try
                        {
                            var cookieData = new Cookie(eachCookie.Name, eachCookie.Value, "/", eachCookie.Domain);
                            webRequest.CookieContainer.Add(cookieData);
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }
                }

                #endregion
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public IResponseParameter GetConversationDetails(DominatorAccountModel dominatorAccount, RedditUser redditUser)
        {
            GetAndSetAuthorizationToken(redditUser);
            return _httpHelper.GetRequest(RdConstants.NewMessageAPI);
        }

        public async Task StartScrapingAds(DominatorAccountModel accountModel, int ScrollCount)
        {
            int count = 0;
            try
            {
                while (count++ < ScrollCount)
                {
                    if (!IsSuspended(_httpHelper, RdConstants.NewRedditHomePageAPI))
                    {
                        if (count % 2 == 0)
                            await ScrapeCommunities(_httpHelper, accountModel);
                        else if (count % 3 == 0)
                            await ScrapeHomeFeed(_httpHelper, accountModel);
                        else if (count % 5 == 0)
                            await FollowRandom(_httpHelper, accountModel);
                        else if (count % 7 == 0)
                            await JoinCommunities(_httpHelper, accountModel);
                        else
                            await UpvoteRandom(_httpHelper, accountModel);
                        await Task.Delay(TimeSpan.FromSeconds(RandomUtilties.GetRandomNumber(40, 30)));
                    }
                    else
                    {
                        //accountModel.AccountBaseModel.Status = AccountStatus.TemporarilyBlocked;
                        if (count % 2 == 0)
                            await ScrapeCommunities(_httpHelper, accountModel);
                        else if (count % 3 == 0)
                            await ScrapeHomeFeed(_httpHelper, accountModel);
                        if (count > ScrollCount / 2)
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog(ex.Message);
            }
            await Task.Delay(TimeSpan.FromMinutes(RandomUtilties.GetRandomNumber(30, 10)));
        }

        private async Task ScrapeCommunities(IRdHttpHelper _httpHelper, DominatorAccountModel accountModel)
        {
            var Channels = await GetSubReddit(accountModel);
            QueryString.Clear();
            foreach (var channel in Channels)
            {
                if (!string.IsNullOrEmpty(channel.PublicDescription))
                    QueryString.Add(channel.PublicDescription);
                var PaginationCount = RandomUtilties.GetRandomNumber(8, 2);
                var paginationParam = new PaginationParameter();
                var Url = $"{HomePage}/r/{channel.Name}/";
                var PageResponse = _httpHelper.GetRequest(Url);
                await RedditAds(PageResponse, accountModel, Url, PaginationCount, true);
                await Task.Delay(TimeSpan.FromSeconds(RandomUtilties.GetRandomNumber(20, 10)));
            }
        }
        private async Task ScrapeHomeFeed(IRdHttpHelper _httpHelper, DominatorAccountModel accountModel)
        {
            var Destinations = new List<string>() { $"{HomePage}/", $"{HomePage}/top/?t=hour", $"{HomePage}/top/?t=day", $"{HomePage}/top/?t=week", $"{HomePage}/top/?t=month", $"{HomePage}/top/?t=year", $"{HomePage}/top/?t=all", $"{HomePage}/rising/", $"{HomePage}/hot/", $"{HomePage}/best/" };
            var DestinationUrl = Destinations.GetRandomItem();
            _httpHelper.GetRequestParameter().Cookies = accountModel.Cookies;
            var reqParam = _httpHelper.GetRequestParameter();
            _httpHelper.SetRequestParameter(reqParam);
            var pageResponse = await _httpHelper.GetRequestAsync(DestinationUrl, accountModel.Token);
            await RedditAds(pageResponse, accountModel, DestinationUrl, RandomUtilties.GetRandomNumber(8, 4));
        }

        private async Task UpvoteRandom(IRdHttpHelper _httpHelper, DominatorAccountModel accountModel)
        {
            var Posts = await GetPost(accountModel);
            QueryString.Clear();
            foreach (var post in Posts)
            {
                await Task.Delay(TimeSpan.FromSeconds(RandomUtilties.GetRandomNumber(10, 5)));
                var FollowResponse = NewUpvote(accountModel, post);
                if (!string.IsNullOrEmpty(post.Title))
                    QueryString.Add(post.Title);
            }
            await Task.Delay(TimeSpan.FromSeconds(RandomUtilties.GetRandomNumber(8, 4)));
        }

        private async Task FollowRandom(IRdHttpHelper _httpHelper, DominatorAccountModel accountModel)
        {
            var Users = await GetPost(accountModel);
            QueryString.Clear();
            foreach (var user in Users)
            {
                var RedditUser = GetUserDetailsByUsername(accountModel, user.Author).RedditUser;
                if (!RedditUser.IsFollowing)
                {
                    await Task.Delay(TimeSpan.FromSeconds(RandomUtilties.GetRandomNumber(20, 15)));
                    var FollowResponse = NewFollow(accountModel, RedditUser);
                    if (!string.IsNullOrEmpty(user.Title))
                        QueryString.Add(user.Title);
                }
                await Task.Delay(4000);
            }
        }

        private async Task<List<RedditPost>> GetPost(DominatorAccountModel accountModel)
        {
            var query = GetQueryValue();
            var searchUrl = $"{HomePage}/search?q={query}&type=link";
            var response = await _httpHelper.GetRequestAsync(searchUrl, accountModel.Token);
            var postResponseHandler = new RedditPostResponseHandler(response, false, null);
            var RandomCount = RandomUtilties.GetRandomNumber(10, 5);
            return postResponseHandler.LstRedditPost.Count > 0 ? postResponseHandler.LstRedditPost.Take(RandomCount <= postResponseHandler.LstRedditPost.Count ? RandomCount : RandomUtilties.GetRandomNumber(postResponseHandler.LstRedditPost.Count)).ToList() : new List<RedditPost>();
        }

        private string GetQueryValue()
        {
            return QueryString.Count == 0 ? Regex.Split(DefaultQueryString, " ").GetRandomItem() : Regex.Split(QueryString.GetRandomItem(), " ").GetRandomItem();
        }

        private async Task JoinCommunities(IRdHttpHelper _httpHelper, DominatorAccountModel accountModel)
        {
            var Channels = await GetSubReddit(accountModel);
            QueryString.Clear();
            foreach (var channel in Channels)
            {
                var Response = NewSubscribe(accountModel, channel);
                if (!string.IsNullOrEmpty(channel.DisplayText))
                    QueryString.Add(channel.PublicDescription);
                await Task.Delay(TimeSpan.FromSeconds(RandomUtilties.GetRandomNumber(10, 5)));
            }
        }

        private async Task<List<SubRedditModel>> GetSubReddit(DominatorAccountModel accountModel)
        {
            var query = GetQueryValue();
            var searchUrl = $"{HomePage}/search?q={query}&type=sr%2Cuser";
            var response = await _httpHelper.GetRequestAsync(searchUrl, accountModel.Token);
            var subredditResponse = new SubredditResponseHandler(response, false, null);
            var RandomCount = RandomUtilties.GetRandomNumber(10, 1);
            return subredditResponse.LstSubReddit.Count > 0 ? subredditResponse.LstSubReddit.Take(RandomCount <= subredditResponse.LstSubReddit.Count ? RandomCount : RandomUtilties.GetRandomNumber(subredditResponse.LstSubReddit.Count)).ToList() : new List<SubRedditModel>();
        }

        public void SaveUserDetailsInDb(DominatorAccountModel account, Dictionary<IpLocationDetails, string> ipDetails)
        {
            try
            {
                #region Save User Details on server.
                //var httpHelperForAds = _httpHelper;
                //var req = httpHelperForAds.GetRequestParameter();
                //req.Cookies = account.Cookies;
                //req.Proxy = account.AccountBaseModel.AccountProxy;
                //req.ContentType = "application/json";
                //req.KeepAlive = true;
                //httpHelperForAds.SetRequestParameter(req);
                //var apiUrl = RdConstants.SaveUserDetailsInDev;

                //var adsData = new SortedDictionary<string, string>
                //{
                //    {"quora_id", account.AccountBaseModel.UserId},
                //    {"current_country", ipDetails[IpLocationDetails.Country]},
                //    {"age", ""},
                //    {"Gender", ""},
                //    {"relationship_status", ""},
                //    {"name", account.AccountBaseModel.UserFullName},
                //    {"server_user", "2"}
                //};
                //var postData = JsonConvert.SerializeObject(adsData, Formatting.Indented);
                //httpHelperForAds.PostRequest(apiUrl, postData);
                #endregion
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public IResponseParameter ApiResponse(string api) => _httpHelper.GetRequest(api);
        #endregion
    }
}