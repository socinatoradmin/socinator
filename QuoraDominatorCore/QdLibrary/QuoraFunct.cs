using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Request;
using DominatorHouseCore.Utility;
using HtmlAgilityPack;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using QuoraDominatorCore.Enums;
using QuoraDominatorCore.Models;
using QuoraDominatorCore.QdUtility;
using QuoraDominatorCore.Request;
using QuoraDominatorCore.Response;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QuoraDominatorCore.QdLibrary
{
    public interface IQuoraFunctions
    {
        Task<string> GetProfileUrlAsync(DominatorAccountModel accountModel);
        UpvoteResponseHandler StartUpvote(DominatorAccountModel accountModel, string pageresponse, string jsInit);
        string GetUpvotePostData(string pageresponse, string jsInit);
        List<UserInfoResponseHandler> GetUserFollowers(DominatorAccountModel accountModel, string profileUrl);
        Task<UserInfoResponseHandler> UserInfoAsync(DominatorAccountModel accountModel, string url);
        Task<UpvoteResponseHandler> UpvoteAnswer(DominatorAccountModel accountModel, string pageresponse, QuoraUser quoraUser);
        PostUpvoteResponseHandler PostUpvote(DominatorAccountModel accountModel, string response, int biid);
        Task<DownvoteResponseHandler> DownvoteAnswer(DominatorAccountModel accountModel, string pagesource, string url);
        Task<FollowResponseHandler> Follow(DominatorAccountModel accountModel, string pagesource, string referUrl);
        FollowResponseHandler FollowAfterCompleteProcess(DominatorAccountModel accountModel, string profilepagesource);
        FollowerResponseHandler Follower(DominatorAccountModel accountModel, string url);
        Task<FollowerResponseHandler> FollowerAsync(DominatorAccountModel accountModel, string url);
        FollowingsResponseHandler Following(DominatorAccountModel accountModel, string url);
        Task<FollowingsResponseHandler> FollowingAsync(DominatorAccountModel accountModel, string url);
        TopicFollowersResponseHandler TopicFollowers(DominatorAccountModel accountModel, string url);
        List<UserInfoResponseHandler> GetUserFollowing(DominatorAccountModel accountModel, string profileUrl);

        Task<List<UserInfoResponseHandler>>
            GetUserFollowersAsync(DominatorAccountModel accountModel, string profileUrl);

        Task<List<UserInfoResponseHandler>> GetUserFollowingAsync(DominatorAccountModel accountModel, string profileUrl,
            bool isUsernamesNeeded = false);

        Task< ReportUserResponseHandler>OldReportUser(DominatorAccountModel accountModel, ScrapeResultNew scrapeResult);
        Task<ReportAnswerResponseHandler> ReportAnswer(DominatorAccountModel accountModel, string response,string ReportDetails,string AnswerUrl);

        PostAnswerResponseHandler PostAnswer(DominatorAccountModel accountModel, string formkey, string postKey,
            string windowId, string qId);

        AnswerByKeywordResponseHandler AnswerByKeyword(DominatorAccountModel accountModel, string serializedArgs,
            string responseFollowerFirstPage);

        AnswerByKeywordResponseHandler AnswerByKeywordWithoutCookies(DominatorAccountModel accountModel,
            string questionList);

        UserAnswerResponseHandler UserAnswer(DominatorAccountModel accountModel, string UserId,int PaginationCount=0);

        Task<AnswerUpvotersResponseHandler> AnswerUpvoters(DominatorAccountModel accountModel, string url, BasePostData post,
            string aid = null,string AnswerNodeId="",int PaginationCount=0);

        Task<AnswerOnQuestionResponseHandler> AnswerOnQuestion(DominatorAccountModel accountModel,
            AnswerQuestionModel answerScraperModel, string questionUrl, string comment);

        UserFromAnswerOnQuestionResponseHandler UserNameAnsweredOnQuestion(DominatorAccountModel accountModel, string PaginationId, int PaginationCount = 0);

        //UploadImageResponseHandler UploadImage(DominatorAccountModel dominatorAccountModel, AnswerQuestionModel _answerQuestionModel);

        AnswerDetailsResponseHandler AnswerDetails(DominatorAccountModel accountModel, string url);

        Task<PostQuestionResponseHandler> PostQuestion(DominatorAccountModel accountModel, string pagesource, string question,
            string questionLink);
        Task<PostQuestionResponseHandler> CreatePost(DominatorAccountModel accountModel, string pagesource, string file, string Title);
        Task<PostQuestionResponseHandler> SharePost(DominatorAccountModel accountModel, string pagesource, string shareUrl, string Title="");
        QuestionByKeywordResponseHandler QuestionByKeyword(DominatorAccountModel accountModel, string url);
        UserQuestionResponseHandler UserQuestion(DominatorAccountModel accountModel, string url,int PaginationCount=0);

        Task<DownvoteQuestionResponseHandler> DownvoteQuestion(DominatorAccountModel accountModel, QuoraUser quoraUser,
            string PageResponse);

        QuestionDetailsResponseHandler QuestionDetails(DominatorAccountModel accountModel, string url);

        SendMessageResponseHandler SendMessage(DominatorAccountModel accountModel, BasePostData postcontent,
            string UserId, string message, string numOfKnownMessages,string Referer="",string ThreadId="");

        SendMessageResponseHandler SendMessageToNew(DominatorAccountModel accountModel, BasePostData postcontent,
            string jsInit, string message);

        SendMessageResponseHandler SendMessage(DominatorAccountModel accountModel, string formkey, string postKey,
            string windowId, string uId, string message);

        ScrapeMessageResponseHandler ReadMessage(DominatorAccountModel accountModel, BasePostData basePostData, string PaginationId, bool IsReadAllMessage = false, bool IsReadAllUsers = false);
        ScrapeMessageResponseHandler ScrapeMessagesId(DominatorAccountModel accountModel, string paginationCount);
        UserNameByKeywordResponseHandler UserNameByKeyword(DominatorAccountModel accountModel,string queryValue,int paginationCount=9,bool IsBrowser=true);
        UserInfoResponseHandler UserInfo(DominatorAccountModel accountModel, string url);

        CommentOnAnswerResponseHandler UserNameCommentOnAnswer(DominatorAccountModel accountModel, string PaginationId,
            int PaginationCount);

        CommentOnQuestionResponseHandler UserNameCommentOnQuestion(DominatorAccountModel accountModel,string PaginatioId,int PaginationCount);

        CommentResponseHandler CommentOnPost(DominatorAccountModel accountModel, string responseFollowerFirstPage,
            string message, int biid);

        string GetProfileUrl(DominatorAccountModel accountModel);
        Task<UnFollowResponseHandler> UnFollow(DominatorAccountModel accountModel, string response,string ProfileUrl);

        PostResponseHandler Post(DominatorAccountModel accountModel, string user, string formkey = null,
            string postKey = null, string windowId = null, string PostData = null);
        string GetChatPaginationPostData(BasePostData basePostData);
        string GetFinalChatPaginationPostData(BasePostData basePostData);

        Task<List<AdScraperModel>> StartScrapingAds(DominatorAccountModel accountModel, int noOfPages);

        Task SaveDetailsInDb(DominatorAccountModel accountModel, AdScraperModel currentAd,
            Dictionary<IpLocationDetails, string> ipDetails);

        Task SaveUserDetailsInDb(DominatorAccountModel account, Dictionary<IpLocationDetails, string> ipDetails);

        Task<Dictionary<IpLocationDetails, string>> GetIpDetails(DominatorAccountModel account, bool isLocalIp = false);
        void SetRequestParameter(IRequestParameters requestParameters,DominatorAccountModel accountModel,BasePostData basePostData);
        TopicResponseHandler Topic(DominatorAccountModel accountModel, string url,SearchQueryType QueryType, string QueryValue,int paginationCount=0);
        IResponseParameter SearchQueryByType(string Url, SearchQueryType QueryType, string QueryValue, DominatorAccountModel dominatorAccount,int paginationCount);
        BasePostData GetBasePostData(string Url, DominatorAccountModel accountModel,string Response="");
        Task SetGeneralHeaders(IRequestParameters request,DominatorAccountModel account, string Url = "",BasePostData basePostData=null, string Host = "", string ContentType = "");
        Task<IResponseParameter> GetUserActivityDetailsByType(string UserId, UserActivityType activityType, DominatorAccountModel dominatorAccount, int PaginationCount=0,string OrderBy="",string Id="",string MutifeedAfter="",string PageData="",string Domain= "www.quora.com",string hash= "");
        string GetUserId(string ProfileUrl);
        string GetAnswerId(string AnswerUrl,out string AnswerNodeId);
        Task<IResponseParameter> GetQuestionsFromTopics(string TopicId, DominatorAccountModel accountModel);
        string GetQuestionId(string QuestionUrl);
        Task<string> GetAnswerOfQuestionPaginationId(DominatorAccountModel dominatorAccount, string QuestionUrl,string QuestionId="");
        Task<string> UploadMedia(DominatorAccountModel accountModel,string file,BasePostData basePostData,IRequestParameters ReqParameter);
        Task<string> PostRequest(string PostAPI, byte[] PostBody, string UserId,HttpWebRequest webRequest=null,string Domain="", string Origin = "", string Referer = "");
        IResponseParameter MarkMessageAsRead(string ThreadId);
        string GetThreadID(string UserId);
        Task<string> GetCredentialId(DominatorAccountModel dominatorAccount);
        Task<ScrapePostResponseHandler> ScrapeKeyWordPost(DominatorAccountModel dominatorAccount, string QueryValue, int PaginationCount = 0);
        IResponseParameter CustomPostResponse(string Url);
        Task<UpvoteResponseHandler> UpvotePost(DominatorAccountModel accountModel, string pageresponse, PostDetails postDetail);
        Task<DownvoteResponseHandler> DownvotePost(DominatorAccountModel accountModel, string pageresponse, PostDetails postDetail);
    }

    public class QuoraFunct : IQuoraFunctions
    {
        public readonly IQdHttpHelper _httpHelper;
        public readonly JsonJArrayHandler jsonHandler = JsonJArrayHandler.GetInstance;
        public QuoraFunct(IQdHttpHelper httpHelper)
        {
            _httpHelper = httpHelper;
        }

        public async Task<DownvoteResponseHandler> DownvoteAnswer(DominatorAccountModel accountModel, string pageSource,
            string referer)
        {
            var postData = GetBasePostData(string.Empty,accountModel,pageSource);
            var decodedPageSource = QdUtilities.GetDecodedResponse(pageSource);

            var broadcastId = Utilities.GetBetween(decodedPageSource, "broadcastId\":", ",").Replace("\"", "");
            var shareId = Utilities.GetBetween(decodedPageSource, "shareId\":", ",");
            var aId = Utilities.GetBetween(decodedPageSource, "\"aid\": ", ",");
            var url = string.Empty;
            var reqParams = _httpHelper.GetRequestParameter();
            await SetGeneralHeaders(reqParams, accountModel, referer, postData);
            if (referer.StartsWith(QdConstants.HomePageUrl))
            {
                reqParams.Headers["Host"] = QdConstants.HomePageUrl.Replace("https://","");
                reqParams.Headers["Origin"] = $"{QdConstants.HomePageUrl}";
                //url = "https://www.quora.com/graphql/gql_POST?q=AnswerDownvoteButton_answerDownvoteAdd_Mutation";
                url = $"{QdConstants.HomePageUrl}/graphql/gql_POST?q=voteUtils_voteChange_Mutation";
            }
            else
            {
                var spaceName = Utilities.GetBetween(referer, "https://", ".quora.com");
                reqParams.Headers["Host"] = spaceName + ".quora.com";
                reqParams.Headers["Origin"] = "https://" + spaceName + ".quora.com";
                url = "https://" + spaceName + ".quora.com/graphql/gql_POST?q=AnswerDownvoteButton_answerDownvoteAdd_Mutation";
            }
            //reqParams.Headers["Quora-Broadcast-Id"] = broadcastId;
            _httpHelper.SetRequestParameter(reqParams);
            //var post = "{\"queryName\":\"AnswerDownvoteButton_answerDownvoteAdd_Mutation\",\"extensions\":{\"hash\":\"b040c95da32328634235d82ccdb7c8665573880200cc830e88fbd9255be9ff4b\"},\"variables\":{\"aid\":" + aId + "}}";
            var post = $"{{\"queryName\":\"voteUtils_voteChange_Mutation\",\"variables\":{{\"oid\":{aId},\"voteType\":\"downvote\",\"entityType\":\"answer\",\"feedStoryHash\":null}},\"extensions\":{{\"hash\":\"1ed80dcdea807f14a363cced5cabf07136288210cf20e4662bb746504e725a09\"}}}}";
            return new DownvoteResponseHandler(_httpHelper.PostRequest(url, post));
        }

        #region UnFollow

        public async Task<UnFollowResponseHandler> UnFollow(DominatorAccountModel accountModel, string response, string ProfileUrl)
        {
            var post = GetBasePostData(string.Empty,accountModel,response);
            var UserId = GetUserId(ProfileUrl);
            await SetGeneralHeaders(_httpHelper.GetRequestParameter(), accountModel, ProfileUrl, post);
            return new UnFollowResponseHandler(_httpHelper.PostRequest(QdConstants.GetUnfollowUserAPI,QdConstants.GetUserUnfollowBody(UserId)));
        }

        #endregion

        public PostResponseHandler Post(DominatorAccountModel accountModel, string user, string formkey = null,
            string postKey = null, string windowId = null, string PostData = null)
        {
            var requestParameter = new QdRequestParameters(_httpHelper.GetRequestParameter());
            if (formkey == null)
            {
                requestParameter.Url = $"{QdConstants.HomePageUrl}/profile/" + user + "/all_posts";
                var url = requestParameter.GenerateUrl();
                return new PostResponseHandler(_httpHelper.GetRequest(url));
            }
            else
            {
                var elements = new QdJsonElements
                {
                    Formkey = formkey,
                    Postkey = postKey,
                    WindowId = windowId
                };
                requestParameter.Body = elements;
                requestParameter.PostDataParameters.Add("json", PostData);
                var postData = requestParameter.GenerateBody();
                requestParameter.Url = "webnode2/server_call_POST?_v=QG2IoEDL3Ygx4X&_m=get_next_page";
                var url = $"{QdConstants.HomePageUrl}/" + requestParameter.GenerateUrl();
                return new PostResponseHandler(_httpHelper.PostRequest(url, postData));
            }
        }
        public async Task<List<AdScraperModel>> StartScrapingAds(DominatorAccountModel accountModel, int noOfPages)
        {
            var ListAds = new List<AdScraperModel>();

            try
            {
                _httpHelper.GetRequestParameter().Cookies = accountModel.Cookies;
                IResponseParameter pageResponse = null;
                ScrapeAdsResponseHandler objScrapeAdsResponseHandler = null;
                BasePostData basePostData = null;
                var paginationParameter = new PaginationParameter();
                noOfPages++;
                do
                {
                    string decodedResponseFollowerPage;
                    //for First Page Hit.
                    if (objScrapeAdsResponseHandler == null)
                    {
                        var reqParam = _httpHelper.GetRequestParameter();
                        _httpHelper.SetRequestParameter(reqParam);
                        pageResponse = await _httpHelper.GetRequestAsync("https://www.quora.com/", accountModel.Token);
                        decodedResponseFollowerPage = QdUtilities.GetDecodedResponse(pageResponse.Response);
                        basePostData = GetBasePostData(string.Empty, accountModel, decodedResponseFollowerPage);
                        objScrapeAdsResponseHandler = new ScrapeAdsResponseHandler(pageResponse, paginationParameter);
                    }
                    else
                    {
                        //For Pagination.
                        try
                        {
                            decodedResponseFollowerPage = QdUtilities.GetDecodedResponse(pageResponse.Response);
                            var broadCastId = Utilities.GetBetween(decodedResponseFollowerPage, "broadcastId\":\"", "\"");
                            var reqParams = _httpHelper.GetRequestParameter();
                            await SetGeneralHeaders(reqParams,accountModel,string.Empty,basePostData);
                            if (string.IsNullOrEmpty(basePostData.Broadcast_Id))
                                reqParams.Headers["Quora-Broadcast-Id"] =broadCastId;
                            reqParams.ContentType = QdConstants.GetContentType;
                            //var hashes ="{ \"queryName\":\"MultifeedQuery\",\"extensions\":{ \"hash\":\"7df333bc09189f9c856e55ce84f205c1b0a3ad3d221296a371a559ed19a2e5d4\"},\"variables\":{ \"first\":8,\"multifeedAfter\":\"7784391393070473096\",\"multifeedNumBundlesOnClient\":6,\"injectionType\":null,\"injectionData\":null,\"multifeedPage\":1,\"pageData\":0} }";
                            //var url = "https://www.quora.com/graphql/gql_para_POST?q=MultifeedQuery";
                            int.TryParse(paginationParameter.EndCursorPosition, out int endCursor);
                            var hashes = QdConstants.AdsPostData(paginationParameter.PaginationID,endCursor);
                            var url = QdConstants.AdsScrapperAPI;
                            pageResponse = _httpHelper.PostRequest(url, hashes);
                            objScrapeAdsResponseHandler = new ScrapeAdsResponseHandler(pageResponse, paginationParameter, true);
                            if(objScrapeAdsResponseHandler?.LstOfAds?.Count > 0)
                                ListAds.AddRange(objScrapeAdsResponseHandler.LstOfAds);
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }
                    }
                } while (noOfPages-- > 0);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            //Updating Ads In DataBase.
            var ipDetails = await GetIpDetails(accountModel);
            //await _quoraFunct.SaveUserDetailsInDb(account, ipDetails);
            var scrapeAds = ListAds;
            foreach (var singleAds in scrapeAds) await SaveDetailsInDb(accountModel, singleAds, ipDetails);
            return ListAds;
        }

        public async Task SetGeneralHeaders(IRequestParameters reqParams,DominatorAccountModel accountModel,string Url="",BasePostData basePostData=null,string Host="", string ContentType = "")
        {
            if (basePostData == null)
            {
                var pageResponse = await _httpHelper.GetRequestAsync(string.IsNullOrEmpty(Url) ? string.IsNullOrEmpty(Host) ? $"{QdConstants.HomePageUrl}/" : $"https://{Host}/" : Url, accountModel.Token);
                var decodedResponse = QdUtilities.GetDecodedResponse(pageResponse.Response);
                basePostData = GetBasePostData(string.Empty, accountModel, decodedResponse);
            }
            reqParams.Headers["Host"] = string.IsNullOrEmpty(Host)? "www.quora.com":Host;
            reqParams.KeepAlive = true;
            reqParams.Headers["Quora-Revision"] = basePostData.Revision;
            reqParams.Headers["Quora-Broadcast-Id"] = basePostData.Broadcast_Id;
            reqParams.Headers["Quora-Canary-Revision"] = "false";
            reqParams.Headers["Quora-Window-Id"] = basePostData.WindowId;
            reqParams.Headers["Quora-Formkey"] = basePostData.FormKey;
            reqParams.Headers["Quora-Page-Creation-Time"] = QdConstants.GetTimeStamp;
            reqParams.UserAgent =
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/83.0.4103.97 Safari/537.36";
            reqParams.ContentType =string.IsNullOrEmpty(ContentType)?"application/json":ContentType;
            reqParams.Accept = "*/*";
            reqParams.Headers["Origin"] = string.IsNullOrEmpty(Host) ? QdConstants.HomePageUrl : $"https://{Host}";
            reqParams.Headers["Sec-Fetch-Site"] = "same-origin";
            reqParams.Headers["Sec-Fetch-Mode"] = "cors";
            reqParams.Headers["Sec-Fetch-Dest"] = "empty";
            reqParams.Referer =string.IsNullOrEmpty(Url)? string.IsNullOrEmpty(Host) ?$"{QdConstants.HomePageUrl}/":$"https://{Host}/":Url;
            reqParams.Headers["Accept-Language"] = "en-US,en;q=0.9";
            if (string.IsNullOrEmpty(Host))
                reqParams.Cookies = accountModel.Cookies;
            else
                reqParams.Cookies = _httpHelper.GetRequestParameter().Cookies;
            _httpHelper.SetRequestParameter(reqParams);
        }

        public async Task SaveDetailsInDb(DominatorAccountModel account, AdScraperModel currentAd,
            Dictionary<IpLocationDetails, string> ipDetails)
        {
            try
            {
                var base64EncodedImage = "";

                var ad_Id = QdUtilities.CalculateMD5Hash(currentAd.AdTitle, currentAd.ImageOrVideoUrl);
                var httpHelperForAds = _httpHelper;
                var req = httpHelperForAds.GetRequestParameter();
                req.Cookies = account.Cookies;
                req.Proxy = account.AccountBaseModel.AccountProxy;

                req.ContentType = "application/json";
                req.KeepAlive = true;

                var adImage = currentAd.ImageOrVideoUrl.Replace("\"", "");

                if (!string.IsNullOrEmpty(adImage))
                {
                    var downLoadImage = MediaUtilites.GetImageBytesFromUrl(adImage);
                    base64EncodedImage = Convert.ToBase64String(downLoadImage, 0, downLoadImage.Length);
                }
                var apiUrl = QdConstants.UpdateAdsInDev;

                var adsData = new SortedDictionary<string, string>
                {
                    {"ad_type", "1"},
                    {"type", "TEXT"},
                    {"category", ""},
                    {"post_owner", currentAd.PostOwner},
                    {"ad_title", currentAd.AdTitle},
                    {"news_feed_description", currentAd.NewsFeedDescription},
                    {"likes",currentAd.Upvote},
                    {"share", ""},
                    {"comment", ""},
                    {"platform", "12"},
                    {"call_to_action", currentAd.CallToAction},
                    {"image_video_url", base64EncodedImage},
                    {"side_url", "Not implemented yet"},
                    {"ad_id", ad_Id},
                    {"post_date", currentAd.TimeStamp},
                    {"source", "desktop"},
                    {"city", ipDetails[IpLocationDetails.City]},
                    {"state", ipDetails[IpLocationDetails.State]},
                    {"country", ipDetails[IpLocationDetails.Country]},
                    {"lower_age", ""},
                    {"upper_age", "65"},
                    {"post_owner_image", currentAd.PostOwnerImage},
                    {"ad_position", "FEED"},
                    {"ad_text", ""},
                    {"quora_id", account.AccountBaseModel.UserId},
                    {"ad_url", ""},
                    {"version", "1.0.0"},
                    {"destination_url", currentAd.DestinationUrl},
                    {"ip_address", ipDetails[IpLocationDetails.Ip]},
                    {"image_url_original", currentAd.ImageOrVideoUrl}
                };
                //{"quora_id", account.AccountBaseModel.UserId},
                var postData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(adsData, Formatting.Indented));
                httpHelperForAds.SetRequestParameter(req);
                var response = httpHelperForAds.PostRequest(apiUrl, postData);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public async Task SaveUserDetailsInDb(DominatorAccountModel account,
            Dictionary<IpLocationDetails, string> ipDetails)
        {
            try
            {
                var httpHelperForAds = _httpHelper;
                var req = httpHelperForAds.GetRequestParameter();
                req.Cookies = account.Cookies;
                req.Proxy = account.AccountBaseModel.AccountProxy;
                req.ContentType = "application/json";
                req.KeepAlive = true;
                httpHelperForAds.SetRequestParameter(req);
                var apiUrl = QdConstants.SaveUserDetailsInDev;

                var adsData = new SortedDictionary<string, string>
                {
                    {"quora_id", account.AccountBaseModel.UserId},
                    {"current_country", ipDetails[IpLocationDetails.Country]},
                    {"age", ""},
                    {"Gender", ""},
                    {"relationship_status", ""},
                    {"name", account.AccountBaseModel.UserFullName},
                    {"server_user", "2"}
                };
                var postData = JsonConvert.SerializeObject(adsData, Formatting.Indented);
                httpHelperForAds.PostRequest(apiUrl, postData);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }


        public async Task<Dictionary<IpLocationDetails, string>> GetIpDetails(DominatorAccountModel account,
            bool isLocalIp = false)
        {
            var dictIpDetails = new Dictionary<IpLocationDetails, string>();
            try
            {
                try
                {
                    var objProxy = _httpHelper.GetRequestParameter().Proxy;


                    string ip;

                    objProxy = objProxy ?? new Proxy();

                    if (isLocalIp)
                    {
                        var objHttpHelper = new QdHttpHelper();
                        var requestParameter = _httpHelper.GetRequestParameter();
                        requestParameter.Proxy = new Proxy();
                        objHttpHelper.SetRequestParameter(requestParameter);
                        var localIpResponse = await _httpHelper.GetApiRequestAsync(QdConstants.GetLocationUrl);
                        var localCountry = Utilities.GetBetween(localIpResponse.Response, "<div class=\"ptih-title\">",
                            "</div>");
                        dictIpDetails.Add(IpLocationDetails.Country,
                            !string.IsNullOrEmpty(localCountry) ? localCountry : string.Empty);
                        return dictIpDetails;
                    }

                    if (string.IsNullOrEmpty(objProxy.ProxyIp))
                    {
                        var ipResponse = await _httpHelper.GetApiRequestAsync(QdConstants.GetLocationUrl);
                        ip = ipResponse.Response;
                        ip = Utilities.GetBetween(ip, "pti-header bgm-green\">", "/h2>");
                        ip = Utilities.GetBetween(ip, ">", "<").Trim();
                    }
                    else
                    {
                        ip = objProxy.ProxyIp;
                    }

                    ip = ip.Replace("\r\n", string.Empty);

                    var match = Regex.Match(objProxy.ProxyUsername ?? string.Empty,
                        @"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}");

                    //var locationUrl = QdConstants.GetLocationApiUrl(ip.Trim());
                    var locationUrl= $"http://ip-api.com/json/{ip.Trim()}";
                    var locationResponse = await _httpHelper.GetApiRequestAsync(locationUrl);

                    var proxyLocationDetails = locationResponse.Response;

                    if (proxyLocationDetails.Contains("INVALID_ADDRESS") && !match.Success)
                    {
                        var proxyIpResponse = await _httpHelper.GetApiRequestAsync(QdConstants.GetLocationUrl);
                        ip = proxyIpResponse.Response;
                        ip = Utilities.GetBetween(ip, "pti-header bgm-green\">", "/h2>");
                        ip = Utilities.GetBetween(ip, ">", "<");

                        locationUrl = QdConstants.GetLocationApiUrl(ip.Trim());

                        var proxyLocationResponse = await _httpHelper.GetApiRequestAsync(locationUrl);
                        proxyLocationDetails = proxyLocationResponse.Response;
                    }
                    else if (match.Success)
                    {
                        ip = match.Groups[0].ToString().Trim();

                        locationUrl = QdConstants.GetLocationApiUrl(ip);

                        locationResponse = await _httpHelper.GetApiRequestAsync(locationUrl);

                        proxyLocationDetails = locationResponse.Response;

                        proxyLocationDetails = QdUtilities.GetNewPrtialDecodedResponse(proxyLocationDetails);
                    }

                    if (proxyLocationDetails.Contains("\"error\""))
                    {
                        locationUrl = QdConstants.GetLocationApiUrlFree(ip.Trim());
                        var proxyLocationResponse = await _httpHelper.GetApiRequestAsync(locationUrl);
                        proxyLocationDetails = proxyLocationResponse.Response;
                    }

                    var city = Utilities.GetBetween(proxyLocationDetails, "city\":\"", "\"");
                    var state = Utilities.GetBetween(proxyLocationDetails, "regionName\":\"", "\"");
                    var country = Utilities.GetBetween(proxyLocationDetails, "country\":\"", "\"");

                    dictIpDetails.Add(IpLocationDetails.City, !string.IsNullOrEmpty(city) ? city : string.Empty);
                    dictIpDetails.Add(IpLocationDetails.State, !string.IsNullOrEmpty(state) ? state : string.Empty);
                    dictIpDetails.Add(IpLocationDetails.Country,
                        !string.IsNullOrEmpty(country) ? country : string.Empty);
                    dictIpDetails.Add(IpLocationDetails.Ip, !string.IsNullOrEmpty(ip) ? ip : string.Empty);
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


        public UploadImageResponseHandler UploadImage(DominatorAccountModel dominatorAccountModel,
            AnswerQuestionModel _answerQuestionModel, BasePostData postContent, string questionUrl)
        {
            try
            {
                var jsonElements = new QdJsonElements
                {
                    Formkey = postContent.FormKey,
                    Kind = "qtext",
                    NID = "0"
                };
                var file =  string.IsNullOrEmpty(_answerQuestionModel.MediaPath)?_answerQuestionModel.ManageCommentModel.MediaPath:_answerQuestionModel.MediaPath;
                var newImage = MediaUtilites.CalculateMD5Hash(file);

                var photoBytes = File.ReadAllBytes(newImage);

                var requestParameters = new QdRequestParameters(_httpHelper.GetRequestParameter());
                requestParameters.FileList = new Dictionary<string, FileData>();
                requestParameters.Body = jsonElements;
                var mediaName = GetFileName(file);
                requestParameters.QuoraAddFileList("file", photoBytes, $"{mediaName}");

                var url = "https://upload.quora.com/_/imgupload/upload_POST";
                var postData = requestParameters.GenerateBodyForMedia();
                requestParameters.IsMultiPart = false;
                requestParameters.Headers["Content-Type"] = requestParameters.ContentType;
                requestParameters.Referer = $"{QdConstants.HomePageUrl}/" + $"{questionUrl}";
                requestParameters.AddHeader("Origin", QdConstants.HomePageUrl);

                if (requestParameters.Cookies != null)
                {
                    var cookieContainer = new CookieCollection();

                    foreach (Cookie eachCookie in requestParameters.Cookies)
                        try
                        {
                            if (string.IsNullOrEmpty(eachCookie.Domain)) eachCookie.Domain = "www.quora.com";
                            var cookieData = new Cookie(eachCookie.Name, eachCookie.Value, "/", eachCookie.Domain);
                            cookieContainer.Add(cookieData);
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }

                    requestParameters.Cookies = new CookieCollection();
                    foreach (Cookie cookie in cookieContainer) requestParameters.Cookies.Add(cookie);
                }

                _httpHelper.SetRequestParameter(requestParameters);

                var response = _httpHelper.PostRequest(url, postData);
                var uploadImageResponseHandler = new UploadImageResponseHandler(response);

                return uploadImageResponseHandler;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return null;
        }

        private string GetFileName(string fileName)
        {
            var mediaName = string.Empty;
            try
            {
                var mediaNameArray = Regex.Split(fileName, "\\\\");
                mediaName = mediaNameArray[mediaNameArray.Length - 1];
            }
            catch (Exception)
            {
            }
            return mediaName;
        }
        public AnswerDetailsResponseHandler AnswerDetails(DominatorAccountModel accountModel, string url)
        {
            var request = _httpHelper.GetRequestParameter();
            request.Cookies = accountModel.Cookies;
            _httpHelper.SetRequestParameter(request);
            return new AnswerDetailsResponseHandler(_httpHelper.GetRequest(url));

        }
        public IResponseParameter CustomPostResponse(string Url)
        {
            try
            {
                var response = _httpHelper.GetRequest(Url);
                return response;
            }
            catch (Exception) { return null; }
        }

        #region AskQuestion
        public async  Task<PostQuestionResponseHandler> PostQuestion(DominatorAccountModel accountModel, string pagesource, string question, string questionLink)
        {
            try
            {
                var decodedResponse = QdUtilities.GetDecodedResponse(pagesource);
                var reqParams = _httpHelper.GetRequestParameter();
                await SetGeneralHeaders(reqParams, accountModel, string.Empty, GetBasePostData(string.Empty, accountModel, decodedResponse));
                reqParams.ContentType = "application/json";
                _httpHelper.SetRequestParameter(reqParams);
                var CreateQuestionAPI = QdConstants.CreateQuestionAPI;
                var CreateQuestionPostBody = QdConstants.CreateQuestionPostBody(question.EndsWith("?")?question:question+"?",true);
                var QuestionCreateResponse = _httpHelper.PostRequest(CreateQuestionAPI, CreateQuestionPostBody);
                var jsonObject = jsonHandler.ParseJsonToJObject(QuestionCreateResponse.Response);
                var CorrectSuggestedQuestion = jsonHandler.GetJTokenValue(jsonObject, "data", "questionCreate", "correctionSuggestion");
                question = string.IsNullOrEmpty(CorrectSuggestedQuestion) ? question : CorrectSuggestedQuestion;
                CreateQuestionPostBody = QdConstants.CreateQuestionPostBody(question.EndsWith("?") ? question : question + "?", false);
                QuestionCreateResponse = _httpHelper.PostRequest(CreateQuestionAPI,Encoding.UTF8.GetBytes(CreateQuestionPostBody));
                return new PostQuestionResponseHandler(QuestionCreateResponse);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return new PostQuestionResponseHandler(new ResponseParameter());
            }
        }
        #endregion
        public QuestionByKeywordResponseHandler QuestionByKeyword(DominatorAccountModel accountModel, string url) => new QuestionByKeywordResponseHandler(_httpHelper.GetRequest(url));



        #region Upvote method

        public async Task<UpvoteResponseHandler> UpvoteAnswer(DominatorAccountModel accountModel, string pageresponse,
            QuoraUser quoraUser)
        {
            var postData = GetBasePostData(string.Empty, accountModel, pageresponse);
            var broadcastId = Utilities.GetBetween(pageresponse, "broadcastId\": ", ",").Replace("\"", "");
            var aid = Utilities.GetBetween(pageresponse, "aid\\\":", ",");
            var reqParams = _httpHelper.GetRequestParameter();
            var Host = quoraUser.Url?.Split('/')?.FirstOrDefault(x => x.Contains(".quora.com"));
            Host = string.IsNullOrEmpty(Host) ? "www.quora.com" : Host;
            var referer =!QdConstants.ContainsHindi(quoraUser.Url)?quoraUser.Url : QdConstants.HomePageUrl;
            var FailedCount = 0;
            TryWithDomain:
            await SetGeneralHeaders(reqParams, accountModel, referer, postData,Host);
            var url = $"https://{Host}/graphql/gql_POST?q=voteUtils_voteChange_Mutation";
            //reqParams.Headers["Quora-Broadcast-Id"] = broadcastId;
            _httpHelper.SetRequestParameter(reqParams);
            var post = $"{{\"queryName\":\"voteUtils_voteChange_Mutation\",\"variables\":{{\"oid\":{aid},\"voteType\":\"upvote\",\"entityType\":\"answer\",\"feedStoryHash\":null}},\"extensions\":{{\"hash\":\"1ed80dcdea807f14a363cced5cabf07136288210cf20e4662bb746504e725a09\"}}}}";
            var postResponse = _httpHelper.PostRequest(url, post);
            if(FailedCount++ <=2 && postResponse != null && postResponse.HasError && postResponse.Exception!=null && postResponse.Exception.Message.Contains("Bad Request"))
            {
                Host = "www.quora.com";
                postData = GetBasePostData($"https://{Host}/profile/{quoraUser.Username}/answers", accountModel, string.Empty);
                goto TryWithDomain;
            }
            return new UpvoteResponseHandler(postResponse);
        }
        public async Task<UpvoteResponseHandler> UpvotePost(DominatorAccountModel accountModel, string pageresponse,
            PostDetails postDetail)
        {
            var postData = GetBasePostData(string.Empty, accountModel, pageresponse);
            int.TryParse(Utilities.GetBetween(pageresponse, "pid\\\":", ","),out int oid);
            var reqParams = _httpHelper.GetRequestParameter();
            var Host = postDetail.PostUrl?.Split('/')?.FirstOrDefault(x => x.Contains(".quora.com"));
            Host = string.IsNullOrEmpty(Host) ? "www.quora.com" : Host;
            var referer = !QdConstants.ContainsHindi(postDetail.PostUrl) ? postDetail.PostUrl : QdConstants.HomePageUrl;
            await SetGeneralHeaders(reqParams, accountModel, referer, postData, Host);
            var url = $"https://{Host}/graphql/gql_POST?q=voteUtils_voteChange_Mutation";
            _httpHelper.SetRequestParameter(reqParams);
            var postBody = QdConstants.GetPostDataForVotings(VoteQueryType.UpVote, oid, PostQueryType.Post);
            var postResponse = _httpHelper.PostRequest(url, postBody);
            return new UpvoteResponseHandler(postResponse);
        }
        public async Task<DownvoteResponseHandler> DownvotePost(DominatorAccountModel accountModel, string pageresponse,
            PostDetails postDetail)
        {
            var postData = GetBasePostData(string.Empty, accountModel, pageresponse);
            int.TryParse(Utilities.GetBetween(pageresponse, "pid\\\":", ","), out int oid);
            var reqParams = _httpHelper.GetRequestParameter();
            var Host = postDetail.PostUrl?.Split('/')?.FirstOrDefault(x => x.Contains(".quora.com"));
            Host = string.IsNullOrEmpty(Host) ? "www.quora.com" : Host;
            var referer = !QdConstants.ContainsHindi(postDetail.PostUrl) ? postDetail.PostUrl : QdConstants.HomePageUrl;
            await SetGeneralHeaders(reqParams, accountModel, referer, postData, Host);
            var url = $"https://{Host}/graphql/gql_POST?q=voteUtils_voteChange_Mutation";
            _httpHelper.SetRequestParameter(reqParams);
            var postBody = QdConstants.GetPostDataForVotings(VoteQueryType.DownVote, oid, PostQueryType.Post);
            var postResponse = _httpHelper.PostRequest(url, postBody);
            return new DownvoteResponseHandler(postResponse);
        }
        public QuestionDetailsResponseHandler QuestionDetails(DominatorAccountModel accountModel, string url)
        {
            var reqParameter = _httpHelper.GetRequestParameter();
            reqParameter.Cookies = accountModel.Cookies;
            _httpHelper.SetRequestParameter(reqParameter);
            return new QuestionDetailsResponseHandler(_httpHelper.GetRequest(url));
        }


        public UpvoteResponseHandler StartUpvote(DominatorAccountModel accountModel, string pageresponse, string jsInit)
        {
            var url = $"{QdConstants.HomePageUrl}/webnode2/server_call_POST?_h=O3OKq%2Fvx4d3FHU&_m=press";
            var postData = GetUpvotePostData(pageresponse, jsInit);
            var response = new UpvoteResponseHandler(_httpHelper.PostRequest(url, postData));
            return response;
        }

        public string GetUpvotePostData(string pageresponse, string jsInit)
        {
            var post = new BasePostData(pageresponse);
            var postData =
                "json=%7B%22args%22%3A%5B%5D%2C%22kwargs%22%3A%7B%22feed_story_hash%22%3Anull%7D%7D&revision=" +
                post.Revision + "&formkey=" + post.FormKey + "&postkey=" + post.PostKey + "&window_id=" +
                post.WindowId +
                "&referring_controller=question&referring_action=answer&_lm_transaction_id=0.5983679545034071&_lm_window_id=dep4705-3092796045477332013&__hmac=O3OKq%2Fvx4d3FHU&__method=press&__e2e_action_id=f50flplosc&js_init=" +
                jsInit + "&__metadata=%7B%7D";
            return postData;
        }

        public PostUpvoteResponseHandler PostUpvote(DominatorAccountModel accountModel, string response, int biid)
        {
            var formkey = Utilities.GetBetween(response, "\"formkey\": \"", "\", \"");
            var postkey = Utilities.GetBetween(response, "\"postkey\": \"", "\"");
            var windowId = Utilities.GetBetween(response, "\"windowId\": \"", "\"");
            var url = $"{QdConstants.HomePageUrl}/webnode2/server_call_POST?_v=LzaHPN7DWur11Z&_m=press";
            var elements = new QdJsonElements
            {
                Formkey = formkey,
                Postkey = postkey,
                WindowId = windowId
            };
            var requestParameter = new QdRequestParameters(_httpHelper.GetRequestParameter());
            requestParameter.PostDataParameters.Add("json",
                "%7B%22args%22%3A%5B%5D%2C%22kwargs%22%3A%7B%7D%7D&revision=27969eaf2af5b3899a66e2eae13485fc83d1dc8d&referring_controller=user&referring_action=all_posts&_lm_transaction_id=0.09178584111427668&_lm_window_id=" +
                windowId +
                "&__vcon_json=%5B%22LzaHPN7DWur11Z%22%5D&__vcon_method=press&__e2e_action_id=f05szw1sz2&js_init=%7B%22qfeed_log_press_action%22%3Anull%2C%22disabled%22%3Afalse%2C%22pressed%22%3Afalse%2C%22url_to_dirty%22%3Anull%2C%22qfeed_log_unpress_action%22%3Anull%2C%22should_change_text_on_press%22%3Afalse%2C%22optimistic_text%22%3A%22Upvoted%22%2C%22optimistic_count%22%3A%2211%22%2C%22is_upvoted%22%3Afalse%2C%22is_disabled%22%3Afalse%2C%22biid%22%3A" +
                biid + "%7D&__metadata=%7B%7D");
            requestParameter.Body = elements;
            var postdata = requestParameter.GenerateBody();
            return new PostUpvoteResponseHandler(_httpHelper.PostRequest(url, postdata));
        }

        #endregion

        #region Follow

        public async  Task<FollowResponseHandler> Follow(DominatorAccountModel accountModel, string pagesource, string referUrl)
        {
            FollowResponseHandler followResponse = new FollowResponseHandler(new ResponseParameter());
            try
            {
                var basePost = GetBasePostData(referUrl, accountModel);
                var uid = Utilities.GetBetween(pagesource, "uid\": ", ",");
                uid = string.IsNullOrEmpty(uid) ? Utilities.GetBetween(basePost.Uid, "", ",") : uid;
                basePost.Broadcast_Id = Utilities.GetBetween(pagesource, "broadcastId\\\":\\\"", "\\\"");
                var requestParam = _httpHelper.GetRequestParameter();
                var referer = $"{QdConstants.HomePageUrl}/profile/"+WebUtility.UrlEncode(referUrl?.Split('/').LastOrDefault(x=>x!=string.Empty));
                await SetGeneralHeaders(requestParam, accountModel, referer);
                requestParam.Headers["Quora-Broadcast-Id"] = basePost.Broadcast_Id;
                _httpHelper.SetRequestParameter(requestParam);
                //var newPostdata ="{\"queryName\":\"UserFollow_userAddFollower_Mutation\",\"extensions\":{\"hash\":\"892d4dabe430ab57ef9fcaba7a731ecf2938485b3c34ad031fa49db7a942eed7\"},\"variables\":{\"uid\":" +uid + ",\"followSource\":\"profile_page\"}}";
                var followPostData = $"{{\"queryName\":\"UserFollow_userFollow_Mutation\",\"variables\":{{\"uid\":{uid},\"followSource\":\"profile_page\"}},\"extensions\":{{\"hash\":\"360304607b1e61c525e8938849ec5b6195c57d0cab7e2f3878c6116dceeecccd\"}}}}";
                //var newUrl = "https://www.quora.com/graphql/gql_POST?q=UserFollow_userAddFollower_Mutation";
                var followAPI = $"{QdConstants.HomePageUrl}/graphql/gql_POST?q=UserFollow_userFollow_Mutation";
                followResponse= new FollowResponseHandler(_httpHelper.PostRequest(followAPI, followPostData));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return followResponse;
        }

        public FollowResponseHandler FollowAfterCompleteProcess(DominatorAccountModel accountModel,
            string profilepagesource)
        {
            var formkey = Utilities.GetBetween(profilepagesource, "\"formkey\": \"", "\", \"");
            var postkey = Utilities.GetBetween(profilepagesource, "\"postkey\": \"", "\"");
            var windowId = Utilities.GetBetween(profilepagesource, "\"windowId\": \"", "\"");
            var uid = Utilities.GetBetween(profilepagesource, "uid&quot;:", "}").Trim();
            var jsonElements = new QdJsonElements
            {
                Formkey = formkey,
                Postkey = postkey,
                WindowId = windowId
            };
            var requestParameter = new QdRequestParameters(_httpHelper.GetRequestParameter())
            {
                Body = jsonElements,
                Url = "webnode2/server_call_POST?_v=bjrTlCMZxGokAP&_m=press"
            };
            var url = requestParameter.GenerateUrl("webnode2/server_call_POST?_v=bjrTlCMZxGokAP&_m=press");
            url = $"{QdConstants.HomePageUrl}/" + url;
            requestParameter.PostDataParameters.Add("json",
                "%7B%22args%22%3A%5B%5D%2C%22kwargs%22%3A%7B%22from_hovercard%22%3Anull%7D%7D&revision=666a9196d49eb5815a05c2088ca03f8fc06d446a");
            requestParameter.PostDataParameters.Add("referring_controller",
                "user&referring_action=profile&_lm_transaction_id=0.24581335621830225&_lm_window_id=" + windowId +
                "&__vcon_json=%5B%22bjrTlCMZxGokAP%22%5D&__vcon_method=press&__e2e_action_id=ex8be8iltu&js_init=%7B%22qfeed_log_press_action%22%3Anull%2C%22disabled%22%3Afalse%2C%22pressed%22%3Afalse%2C%22url_to_dirty%22%3Anull%2C%22qfeed_log_unpress_action%22%3Anull%2C%22should_change_text_on_press%22%3Atrue%2C%22optimistic_text%22%3A%22Following%22%2C%22optimistic_count%22%3A%2237.7k%22%2C%22target_uid%22%3A" +
                uid + "%2C%22scribe_log_category%22%3Afalse%2C%22should_show_pmsg%22%3Afalse%7D&__metadata=%7B%7D");
            var postData = requestParameter.GenerateBody();
            return new FollowResponseHandler(_httpHelper.PostRequest(url, postData));
        }


        public FollowerResponseHandler Follower(DominatorAccountModel accountModel, string url)
        {
            return FollowerAsync(accountModel, url).Result;
        }

        public async Task<FollowerResponseHandler> FollowerAsync(DominatorAccountModel accountModel, string url)
        {
            var request = _httpHelper.GetRequestParameter();
            request.Cookies = accountModel.Cookies;
            _httpHelper.SetRequestParameter(request);
            return new FollowerResponseHandler(await _httpHelper.GetRequestAsync(url, accountModel.Token));
        }

        public FollowingsResponseHandler Following(DominatorAccountModel accountModel, string url)
        {
            return FollowingAsync(accountModel, url).Result;
        }

        public async Task<FollowingsResponseHandler> FollowingAsync(DominatorAccountModel accountModel, string url)
        {

            var userResponse = _httpHelper.GetRequest(url).Response;
            SetHeaders(accountModel, userResponse, url);

            var postData = "{ \"queryName\":\"UserProfileFollowingPeople_ProfileTopics_Query\",\"extensions\":{ \"hash\":\"a202e1863be2040d64873d6d91d4b3fc908459dddcc13a590dff68f4698788c8\"},\"variables\":{ \"uid\":" +
                Utilities.GetBetween(userResponse, "{\"uid\": ", ",")
                + "}}";
            return new FollowingsResponseHandler(await _httpHelper.PostRequestAsync($"{QdConstants.HomePageUrl}/graphql/gql_para_POST?q=UserProfileFollowingPeople_ProfileTopics_Query ", postData, accountModel.Token));
        }

        public void SetHeaders(DominatorAccountModel accountModel, string userPagaSource, string referer)
        {
            var postData = new BasePostData(userPagaSource);
            var broadcastId = Utilities.GetBetween(userPagaSource, "broadcastId\": ", ",").Replace("\"", "");
            var aid = Utilities.GetBetween(userPagaSource, "aid\\\":", ",");
            var reqParams = _httpHelper.GetRequestParameter();

            reqParams.Headers["Host"] = "www.quora.com";
            reqParams.Headers["Origin"] = QdConstants.HomePageUrl;
            reqParams.KeepAlive = true;
            reqParams.Headers["connection"] = "keep-alive";
            reqParams.Headers["Quora-Revision"] = postData.Revision;
            reqParams.Headers["Quora-Broadcast-Id"] = broadcastId;
            reqParams.Headers["Quora-Formkey"] = postData.FormKey;
            reqParams.Headers["Quora-Canary-Revision"] = "false";
            reqParams.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/85.0.4183.102 Safari/537.36";
            reqParams.Headers["Quora-Window-Id"] = postData.WindowId;
            reqParams.ContentType = "application/json";
            reqParams.Accept = "*/*";
            reqParams.Headers["sec-ch-ua"] = "\"Google Chrome\";v=\"93\", \" Not; A Brand\";v=\"99\", \"Chromium\";v=\"93\"";
            reqParams.Headers["sec-ch-ua-mobile"] = "?0";
            reqParams.Headers["Sec-Fetch-Site"] = "same-origin";
            reqParams.Headers["Sec-Fetch-Mode"] = "cors";
            reqParams.Headers["Sec-Fetch-Dest"] = "empty";
            reqParams.Headers["sec-ch-ua-platform"] = "\"Windows\"";
            reqParams.Referer = referer;
            reqParams.Headers["Accept-Language"] = "en-US,en;q=0.9";
            reqParams.Headers["Accept-Encoding"] = "gzip, deflate, br";
            reqParams.Cookies = accountModel.Cookies;

            _httpHelper.SetRequestParameter(reqParams);
        }

        public TopicFollowersResponseHandler TopicFollowers(DominatorAccountModel accountModel, string url)
        {
            var requestParameter = new QdRequestParameters(_httpHelper.GetRequestParameter()) { Url = url };
            var urlFinal = requestParameter.GenerateUrl();
            return new TopicFollowersResponseHandler(_httpHelper.GetRequestAsync(urlFinal, accountModel.Token).Result);
        }

        public List<UserInfoResponseHandler> GetUserFollowers(DominatorAccountModel accountModel, string profileUrl)
        {
            return GetUserFollowersAsync(accountModel, profileUrl).Result;
        }

        public async Task<List<UserInfoResponseHandler>> GetUserFollowersAsync(DominatorAccountModel accountModel,
            string profileUrl)
        {
            var lstUserFollowers = new List<UserInfoResponseHandler>();
            var followerResponse = await FollowerAsync(accountModel, profileUrl + "/followers");
            try
            {
                followerResponse?.FollowerList?.ForEach(async follower =>
                {
                    var followerInfo = await UserInfoAsync(accountModel, $"{QdConstants.HomePageUrl}/profile/" + follower);
                    lstUserFollowers.Add(followerInfo);
                });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            var responseFollowerFirstPage = followerResponse.Response.Response;
            var post = new BasePostData(responseFollowerFirstPage);
            do
            {
                try
                {
                    var urlServerCallPost =
                        $"{QdConstants.HomePageUrl}/webnode2/server_call_POST?_v=" + Uri.EscapeDataString(post.VconJson) +
                        "&_m=increase_count";
                    var postDataCallPost = "json=%7B%22args%22%3A%5B%5D%2C%22kwargs%22%3A%7B%22cid%22%3A%22" +
                                           post.PagedList +
                                           "%22%2C%22num%22%3A18%2C%22current%22%3A72%7D%7D&revision=" + post.Revision +
                                           "&formkey=" + post.FormKey + "&postkey=" + post.PostKey + "&window_id=" +
                                           post.WindowId +
                                           "&referring_controller=user&referring_action=followers&__vcon_json=%5B%22" +
                                           "" + Uri.EscapeDataString(post.VconJson) +
                                           "%22%5D&__vcon_method=increase_count&__e2e_action_id=exz34w8dfi&js_init=%7B%22object_id%22%3A44794736%2C%22initial_count%22%3A18%2C%22buffer_count%22%3A18%2C%22crawler%22%3Afalse%2C%22has_more%22%3Atrue%2C%22retarget_links%22%3Atrue%2C%22fixed_size_paged_list%22%3Afalse%2C%22auto_paged%22%3Atrue%7D&__metadata=%7B%7D";
                    _httpHelper.PostRequest(urlServerCallPost, postDataCallPost);
                    var urlPagination = "https://tch678384.tch.quora.com/up/" + post.Chan + "/updates?&callback=" +
                                        post.JsonP + "&" +
                                        "min_seq=" + post.MinSeq + "&channel=" + post.Channel + "&hash=" + post.Hash +
                                        "&_=" + DateTime.Now.GetCurrentEpochTimeMilliSeconds();

                    followerResponse = Follower(accountModel, urlPagination);
                    if (followerResponse.FollowerList.Count == 0)
                        break;
                    followerResponse.FollowerList?.ForEach(follower =>
                    {
                        var followerInfo = UserInfo(accountModel, $"{QdConstants.HomePageUrl}/profile/" + follower);
                        lstUserFollowers.Add(followerInfo);
                    });
                    var followerPaginationData = followerResponse.Response.Response;
                    if (followerPaginationData.Contains("jsonp"))
                        post.JsonP = "jsonp" + Utilities.GetBetween(followerPaginationData, "jsonp", "({");
                    if (followerPaginationData.Contains("min_seq\":"))
                        post.MinSeq = Utilities.GetBetween(followerPaginationData, "min_seq\":", "})");
                }
                catch (Exception)
                {
                    break;
                }
            } while (!string.IsNullOrEmpty(post.MinSeq));

            return lstUserFollowers;
        }

        public List<UserInfoResponseHandler> GetUserFollowing(DominatorAccountModel accountModel, string profileUrl)
        {
            return GetUserFollowingAsync(accountModel, profileUrl).Result;
        }

        public async Task<List<UserInfoResponseHandler>> GetUserFollowingAsync(DominatorAccountModel accountModel,
            string profileUrl, bool isUsernamesNeeded = false)
        {
            var lstUserFollowing = new List<UserInfoResponseHandler>();

            var followingResponse = await FollowingAsync(accountModel, profileUrl + "/following");

            if (isUsernamesNeeded)
            {
                followingResponse?.FollowingList?.ForEach(following =>
                {
                    lstUserFollowing.Add(
                        new UserInfoResponseHandler(new ResponseParameter()) { Username = following });
                });

                return lstUserFollowing;
            }

            var responseFollowerFirstPage = followingResponse.Response.Response;
            var post = new BasePostData(responseFollowerFirstPage);
            do
            {
                try
                {
                    followingResponse?.FollowingList?.ForEach(async following =>
                    {
                        try
                        {
                            var followingInfo = await UserInfoAsync(accountModel,
                                $"{QdConstants.HomePageUrl}/profile/" + following);
                            lstUserFollowing.Add(followingInfo);
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }
                    });
                    var followerpaginationData = followingResponse.Response.Response;
                    if (followerpaginationData.Contains("jsonp"))
                        post.JsonP = "jsonp" + Utilities.GetBetween(followerpaginationData, "jsonp", "({");

                    if (followerpaginationData.Contains("min_seq\":"))
                        post.MinSeq = Utilities.GetBetween(followerpaginationData, "min_seq\":", "})");
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                    break;
                }
            } while (!string.IsNullOrEmpty(post.MinSeq));

            return lstUserFollowing;
        }

        #endregion

        #region Report

        public async Task<ReportUserResponseHandler> OldReportUser(DominatorAccountModel accountModel, ScrapeResultNew scrapeResult)
        {
            IResponseParameter FinalReportResponse = new ResponseParameter();
            try
            {
                var ResportDetails = scrapeResult.QueryInfo.CustomFilters;
                var ReportModel = QdUtilities.GetReportDetails(ResportDetails);
                var mainUrl = $"{QdConstants.HomePageUrl}/profile/" + scrapeResult.ResultUser.Username;
                var linkresp =await _httpHelper.GetRequestAsync(mainUrl,accountModel.Token);
                var basePostData = GetBasePostData(mainUrl, accountModel, linkresp.Response);
                await SetGeneralHeaders(_httpHelper.GetRequestParameter(), accountModel, mainUrl, basePostData);
                var AuthorID = Utilities.GetBetween(linkresp.Response, "{\\\"user\\\":{\\\"id\\\":\\\"", "\\\"");
                var ReportModelResponse = await _httpHelper.PostRequestAsync(QdConstants.ReportModelAPI, QdConstants.ReportModelPostBody(AuthorID), accountModel.Token);
                var jsonObject = jsonHandler.ParseJsonToJObject(ReportModelResponse.Response);
                var ReportableID = jsonHandler.GetJTokenValue(jsonObject, "data","node", "reportableId");
                var ReportUserAPI = QdConstants.GetReportAPI;
                var reportOption= ReportModel.ReportOptionTitle.Contains("User Credential") ? "credential" : ReportModel.ReportOptionTitle.ToLower().Replace(" ", "_");
                var ReportUserPostBody = QdConstants.GetReportPostBody(QdUtilities.GetReportReason(ReportModel.Title), ReportableID, ReportModel.ReportDescription, reportOption);
                linkresp =await _httpHelper.GetRequestAsync(mainUrl + "/answers",accountModel.Token);
                basePostData=GetBasePostData(mainUrl+"/answers", accountModel, linkresp.Response);
                await SetGeneralHeaders(_httpHelper.GetRequestParameter(), accountModel, mainUrl + "/answers", basePostData);
                FinalReportResponse =await _httpHelper.PostRequestAsync(ReportUserAPI, Encoding.UTF8.GetBytes(ReportUserPostBody),accountModel.Token);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return new ReportUserResponseHandler(FinalReportResponse);
        }
        public async Task<ReportAnswerResponseHandler> ReportAnswer(DominatorAccountModel accountModel, string response, string ReportDetails, string AnswerUrl)
        {
            IResponseParameter FinalReportAnswerResponse = new ResponseParameter();
            try
            {
                var ReportModel = QdUtilities.GetReportDetails(ReportDetails);
                var BasePostData = GetBasePostData(AnswerUrl, accountModel, response);
                await SetGeneralHeaders(_httpHelper.GetRequestParameter(), accountModel, AnswerUrl, BasePostData);
                GetAnswerId(AnswerUrl, out string AuthorID);
                var ReportModelResponse = await _httpHelper.PostRequestAsync(QdConstants.ReportModelAPI, QdConstants.ReportModelPostBody(AuthorID), accountModel.Token);
                var jsonObject = jsonHandler.ParseJsonToJObject(ReportModelResponse.Response);
                var ReportableID = jsonHandler.GetJTokenValue(jsonObject, "data", "node", "reportableId");
                var ReportAnswerAPI = QdConstants.GetReportAPI;
                var title = ReportModel.SubOption != null && ReportModel.SubOption.HaveSubOption && !string.IsNullOrEmpty(ReportModel.SubOption.Title)?
                    ReportModel.SubOption.Title : ReportModel.Title;
                var ReportAnswerPostBody = QdConstants.GetReportPostBody(QdUtilities.GetReportReason(title), ReportableID, ReportModel.ReportDescription, "answer");
                FinalReportAnswerResponse = await _httpHelper.PostRequestAsync(ReportAnswerAPI, Encoding.UTF8.GetBytes(ReportAnswerPostBody), accountModel.Token);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return new ReportAnswerResponseHandler(FinalReportAnswerResponse);
        }

        #endregion

        #region Answer

        public PostAnswerResponseHandler PostAnswer(DominatorAccountModel accountModel, string formkey, string postKey,
            string windowId, string qId)
        {
            var jsonElements = new QdJsonElements
            {
                Formkey = formkey,
                Postkey = postKey,
                WindowId = windowId
            };
            var requestParameter = new QdRequestParameters(_httpHelper.GetRequestParameter())
            {
                Body = jsonElements,
                Url = "webnode2/server_call_POST?_v=PXmhlXEVsUUz6S&_m=edit"
            };
            var url = requestParameter.GenerateUrl();
            url = Constants.ApiUrl + url;
            requestParameter.PostDataParameters.Add("json",
                "%7B%22args%22%3A%5B%5D%2C%22kwargs%22%3A%7B%22id%22%3A" + qId +
                "%2C%22input%22%3A%7B%22sections%22%3A%5B%7B%22type%22%3A%22plain%22%2C%22indent%22%3A0%2C%22quoted%22%3Afalse%2C%22spans%22%3A%5B%7B%22modifiers%22%3A%7B%7D%2C%22text%22%3A%22yes+i+candjdfkdfdfd+check%22%7D%5D%7D%5D%2C%22caret%22%3A%7B%22start%22%3A%7B%22spanIdx%22%3A0%2C%22sectionIdx%22%3A0%2C%22offset%22%3A13%7D%2C%22end%22%3A%7B%22spanIdx%22%3A0%2C%22sectionIdx%22%3A0%2C%22offset%22%3A13%7D%7D%7D%2C%22disclaimer_update%22%3A%22%22%2C%22fb_uid%22%3Anull%2C%22fb_access_token%22%3Anull%2C%22fb_expires%22%3Anull%2C%22updated_question_modal_shown%22%3Afalse%2C%22redirect_to_answer_page%22%3Atrue%2C%22share_values%22%3A%7B%22fb%22%3Afalse%2C%22tw%22%3Afalse%7D%7D%7D&revision=60d5dd97c94ac5766e30e23907554a2aca3c3e80");
            requestParameter.PostDataParameters.Add("referring_controller",
                "answer&referring_action=index&_lm_transaction_id=0.7446459501539677&_lm_window_id=" + windowId +
                "&__vcon_json=%5B%22PXmhlXEVsUUz6S%22%5D&__vcon_method=edit&__e2e_action_id=eygdg635ax&js_init=%7B%22id%22%3A" +
                qId + "%2C%22input%22%3A%22answer_content_text%22%2C%22typing_area%22%3A%22qa-" + qId +
                "%22%2C%22draft_space%22%3A%7B%22type%22%3A%22new_answer%22%2C%22qid%22%3A" + qId +
                "%7D%2C%22unsaved_content_msg%22%3A%22Your+answer+has+not+been+saved.%22%2C%22focus_onload%22%3Afalse%2C%22is_qtext%22%3Atrue%2C%22require_comment%22%3Afalse%2C%22require_value%22%3Afalse%2C%22content_type%22%3A%22answer%22%2C%22submit_text%22%3A%22Submit%22%2C%22show_editor%22%3Afalse%2C%22shouldShowShareWarningDialog%22%3Afalse%2C%22hasAnswerDistributionMenu%22%3Atrue%2C%22is_answer_page%22%3Afalse%2C%22redirect_to_answer_page%22%3Atrue%7D&__metadata=%7B%7D");
            var postData = requestParameter.GenerateBody();
            return new PostAnswerResponseHandler(_httpHelper.PostRequest(url, postData));
        }

        public PostAnswerResponseHandler PostAnswer(DominatorAccountModel accountModel, string formkey, string postKey,
            string windowId, string qId, string answer)
        {
            var jsonElements = new QdJsonElements
            {
                Formkey = formkey,
                Postkey = postKey,
                WindowId = windowId
            };
            var requestParameter = new QdRequestParameters(_httpHelper.GetRequestParameter())
            {
                Body = jsonElements,
                Url = "webnode2/server_call_POST?_v=PXmhlXEVsUUz6S&_m=edit"
            };
            var url = requestParameter.GenerateUrl();
            requestParameter.PostDataParameters.Add("json",
                "%7B%22args%22%3A%5B%5D%2C%22kwargs%22%3A%7B%22id%22%3A" + qId +
                "%2C%22input%22%3A%7B%22sections%22%3A%5B%7B%22type%22%3A%22plain%22%2C%22indent%22%3A0%2C%22quoted%22%3Afalse%2C%22spans%22%3A%5B%7B%22modifiers%22%3A%7B%7D%2C%22text%22%3A%22" +
                answer +
                "%22%7D%5D%7D%5D%2C%22caret%22%3A%7B%22start%22%3A%7B%22spanIdx%22%3A0%2C%22sectionIdx%22%3A0%2C%22offset%22%3A13%7D%2C%22end%22%3A%7B%22spanIdx%22%3A0%2C%22sectionIdx%22%3A0%2C%22offset%22%3A13%7D%7D%7D%2C%22disclaimer_update%22%3A%22%22%2C%22fb_uid%22%3Anull%2C%22fb_access_token%22%3Anull%2C%22fb_expires%22%3Anull%2C%22updated_question_modal_shown%22%3Afalse%2C%22redirect_to_answer_page%22%3Atrue%2C%22share_values%22%3A%7B%22fb%22%3Afalse%2C%22tw%22%3Afalse%7D%7D%7D&revision=60d5dd97c94ac5766e30e23907554a2aca3c3e80");
            requestParameter.PostDataParameters.Add("referring_controller",
                "answer&referring_action=index&_lm_transaction_id=0.7446459501539677&_lm_window_id=" + windowId +
                "&__vcon_json=%5B%22PXmhlXEVsUUz6S%22%5D&__vcon_method=edit&__e2e_action_id=eygdg635ax&js_init=%7B%22id%22%3A" +
                qId + "%2C%22input%22%3A%22answer_content_text%22%2C%22typing_area%22%3A%22qa-" + qId +
                "%22%2C%22draft_space%22%3A%7B%22type%22%3A%22new_answer%22%2C%22qid%22%3A" + qId +
                "%7D%2C%22unsaved_content_msg%22%3A%22Your+answer+has+not+been+saved.%22%2C%22focus_onload%22%3Afalse%2C%22is_qtext%22%3Atrue%2C%22require_comment%22%3Afalse%2C%22require_value%22%3Afalse%2C%22content_type%22%3A%22answer%22%2C%22submit_text%22%3A%22Submit%22%2C%22show_editor%22%3Afalse%2C%22shouldShowShareWarningDialog%22%3Afalse%2C%22hasAnswerDistributionMenu%22%3Atrue%2C%22is_answer_page%22%3Afalse%2C%22redirect_to_answer_page%22%3Atrue%7D&__metadata=%7B%7D");
            var postData = requestParameter.GenerateBody();
            return new PostAnswerResponseHandler(_httpHelper.PostRequest(url, postData));
        }

        public AnswerByTopicResponseHandler AnswerByTopic(DominatorAccountModel accountModel, string serializedArgs,
            BasePostData post, string parentCid, string parentDomainId)
        {
            var requestParameter = new QdRequestParameters
            {
                Headers =
                {
                    ["Host"] = "www.quora.com",
                    ["Connection"] = "keep-alive",
                    ["Origin"] = "https://www.quora.com",
                    ["X-Requested-With"] = "XMLHttpRequest",
                    ["Content-Type"] = "application/x-www-form-urlencoded; charset=UTF-8",
                    ["Accept-Language"] = "en-US,en;q=0.9"
                }
            };
            var parameter = accountModel.Cookies;
            requestParameter.Cookies = parameter;
            requestParameter.Accept = "application/json, text/javascript, */*; q=0.01";
            requestParameter.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            requestParameter.UserAgent =
                "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/68.0.3440.106 Safari/537.36";

            _httpHelper.SetRequestParameter(requestParameter);
            var url =
                $"{QdConstants.HomePageUrl}/webnode2/server_call_POST?_h=Nvk9Re%2F2T5nlwG&_m=maybe_show_quora_share_tooltip";

            var postData = "json=%7B%22args%22%3A%5B%5D%2C%22kwargs%22%3A%7B%22content_type%22%3A4%7D%7D&revision=" +
                           post.Revision + "&formkey=" + post.FormKey + "&postkey=" + post.PostKey + "&window_id=" +
                           post.WindowId + "&referring_controller=question&referring_action=q&parent_cid=" + parentCid +
                           "&parent_domid=" + parentDomainId +
                           "&domids_to_remove=%5B%5D&__hmac=Nvk9Re%2F2T5nlwG&__method=maybe_show_quora_share_tooltip&__e2e_action_id=f4ly4pmox7&js_init=%7B%22disabled%22%3Afalse%2C%22oid%22%3A95748127%2C%22content_type%22%3A4%7D&__metadata=%7B%7D";
            var response = _httpHelper.PostRequest(url, postData);

            return new AnswerByTopicResponseHandler(response);
        }

        public AnswerByKeywordResponseHandler AnswerByKeyword(DominatorAccountModel accountModel, string serializedArgs,
            string responseFollowerFirstPage)
        {
            var id = Utilities.GetBetween(responseFollowerFirstPage, "Toggle", ">");
            var id2 = Utilities.GetBetween(id, "id=\"__w2_", "__truncated");
            if (string.IsNullOrEmpty(id2))
            {
                id2 = Utilities.GetBetween(id, "id='__w2_", "__truncated");
            }
            var parentId = Utilities.GetBetween(responseFollowerFirstPage, "{\"people_selector\": ",
                "shouldOpenModalOnLoad\": ");
            var parentId2 = Regex.Split(parentId, id2)[2].Split('"')[2];
            var post = new BasePostData(responseFollowerFirstPage);
            var jsonElement = new QdJsonElements
            {
                Formkey = post.FormKey,
                Postkey = post.PostKey,
                WindowId = post.WindowId,

                ParentCid = id2,
                ParentDomId = parentId2
            };
            var requestParameter = new QdRequestParameters(_httpHelper.GetRequestParameter())
            {
                Body = jsonElement,
                Url = "webnode2/server_call_POST?_h=GAnhGgc3t%2BOyrc&_m=fetch_toggled_component"
                //https://www.quora.com/webnode2/server_call_POST?_h=GAnhGgc3t%2BOyrc&_m=fetch_toggled_component
            };
            var url = requestParameter.GenerateUrl();
            url = $"{QdConstants.HomePageUrl}/" + url;

            var postData = "json=%7B%22args%22%3A%5B%5D%2C%22kwargs%22%3A%7B%22serialized_args%22%3A%22" +
                           serializedArgs + "%3D%3D%5Cn%22%7D%7D" +
                           "&revision=" + post.Revision + "&formkey=" + post.FormKey + "&postkey=" + post.PostKey +
                           "&window_id=" + post.WindowId + "&referring_controller=" + post.ReferringController +
                           "&referring_action=" + post.ReferringAction + "&parent_cid=" + id2 + "&parent_domid=" +
                           parentId2 +
                           "&domids_to_remove=%5B%5D&__hmac=GAnhGgc3t%2BOyrc&__method=fetch_toggled_component&__e2e_action_id=f9ugplybpr&__metadata=%7B%7D";

            return new AnswerByKeywordResponseHandler(_httpHelper.PostRequest(url, postData));
        }

        public AnswerByKeywordResponseHandler AnswerByKeywordWithoutCookies(DominatorAccountModel accountModel,
            string questionUrl)
        {
            var requestParameter = new QdRequestParameters(_httpHelper.GetRequestParameter()) { Url = questionUrl };
            var urlFinal = requestParameter.GenerateUrl();
            return new AnswerByKeywordResponseHandler(_httpHelper.GetRequestAsync(urlFinal, accountModel.Token).Result);
        }

        public UserAnswerResponseHandler UserAnswer(DominatorAccountModel accountModel, string UserId, int PaginationCount = 0)
        {
            var count = 0;
            var list = new List<AnswerDetails>();
            SearchMore:
            var response = new UserAnswerResponseHandler(GetUserActivityDetailsByType(UserId, UserActivityType.Answers,
                accountModel, PaginationCount, "MostRecent", string.Empty).Result,false);
            list.AddRange(response.AnswerList);
            while (count++ <= 5 && response.HasMoreResult)
            {
                PaginationCount = response.EndCursor;
                goto SearchMore;
            }
            response.AnswerList = list;
            return response;
        }
        public async Task<AnswerUpvotersResponseHandler> AnswerUpvoters(DominatorAccountModel accountModel, string url,
            BasePostData post, string aid = null,string AnswerNodeId = "", int PaginationCount = 0)
        {
            var reqParam = _httpHelper.GetRequestParameter();
            await SetGeneralHeaders(reqParam, accountModel, url, null);
            _httpHelper.SetRequestParameter(reqParam);
            var UpvotersResponse = GetUserActivityDetailsByType(string.Empty, UserActivityType.AnswerUpvoter, accountModel, PaginationCount, PaginationCount > 0 ? "Paginated" : "MostRecent", AnswerNodeId, string.Empty, string.Empty).Result;
            return new AnswerUpvotersResponseHandler(UpvotersResponse,false);
        }

        public async Task<AnswerOnQuestionResponseHandler> AnswerOnQuestion(DominatorAccountModel accountModel,
            AnswerQuestionModel answerScraperModel, string questionUrl, string commentText)
        {
            var reqParams = _httpHelper.GetRequestParameter();
            reqParams.Cookies = accountModel.Cookies;
            _httpHelper.SetRequestParameter(reqParams);
            var response = _httpHelper.GetRequest(questionUrl).Response;
            var decodedResponseFollowerPage = QdUtilities.GetDecodedResponse(response);
            var qid = Utilities.GetBetween(response, "\\\"qid\\\":", ",").Trim();
            var basePost = GetBasePostData(questionUrl, accountModel, decodedResponseFollowerPage);
            var requestParam = _httpHelper.GetRequestParameter();
            await SetGeneralHeaders(reqParams, accountModel, questionUrl, basePost);
            var MediaUrl = string.Empty;
            _httpHelper.SetRequestParameter(requestParam);
            if (answerScraperModel != null && !string.IsNullOrEmpty(answerScraperModel.LstManageCommentModel.FirstOrDefault().MediaPath) && File.Exists(answerScraperModel.LstManageCommentModel.FirstOrDefault().MediaPath))
            {
                var ImageUploadResponse = await UploadMedia(accountModel, answerScraperModel.LstManageCommentModel.FirstOrDefault().MediaPath,basePost, _httpHelper.GetRequestParameter());
                MediaUrl = ImageUploadResponse;
            }
            var GeneratedPostBody = QdUtilities.GetJsonPostBody(commentText,MediaUrl);
            return new AnswerOnQuestionResponseHandler(await _httpHelper.PostRequestAsync(QdConstants.AnswerOnQuestionAPI, QdConstants.AnswerOnQuestionBody(qid,GeneratedPostBody),accountModel.Token));
        }


        public UserFromAnswerOnQuestionResponseHandler UserNameAnsweredOnQuestion(DominatorAccountModel accountModel,string PaginationId,int PaginationCount=0)
        {
            var UsersFromAnswerResponse = GetUserActivityDetailsByType(string.Empty,UserActivityType.AnswersOfQuestion,accountModel,PaginationCount,PaginationCount > 0?"Paginated":"MostRecent",PaginationId,string.Empty,string.Empty).Result;
            return new UserFromAnswerOnQuestionResponseHandler(UsersFromAnswerResponse,false);
        }

        public async Task<AnswerDetailsResponseHandler> AnswerDetailsAsync(DominatorAccountModel accountModel,
            string url)
        {
            var requestParameter = new QdRequestParameters(_httpHelper.GetRequestParameter()) { Url = url };
            var urlFinal = requestParameter.GenerateUrl();
            return new AnswerDetailsResponseHandler(await _httpHelper.GetRequestAsync(urlFinal, accountModel.Token));
        }

        #endregion

        #region Question

        public UserQuestionResponseHandler UserQuestion(DominatorAccountModel accountModel, string url,int PaginationCount=0)=>
            new UserQuestionResponseHandler(GetUserActivityDetailsByType(GetUserId(url), UserActivityType.Questions,accountModel,PaginationCount,string.Empty).Result,false);

        public async Task<DownvoteQuestionResponseHandler> DownvoteQuestion(DominatorAccountModel accountModel, QuoraUser quoraUser,
            string PageResponse)
        {
            
            var Host = quoraUser.Url?.Split('/')?.FirstOrDefault(x=>x.Contains(".quora.com"));
            Host=string.IsNullOrEmpty(Host) ? "www.quora.com" : Host;
            var decodedResponse = QdUtilities.GetDecodedResponse(PageResponse);
            var qid = Utilities.GetBetween(decodedResponse, "Question\",\"qid\":", ",");
            if(string.IsNullOrEmpty(qid))
                qid = Utilities.GetBetween(decodedResponse, "\"qid\":", ",");
            var broadcastId = Utilities.GetBetween(PageResponse, "broadcastId\": ", ",").Replace("\"", "");
            var reqParams = _httpHelper.GetRequestParameter();
            var FailedCount = 0;
            var Referer = quoraUser.Url;
            var postData = GetBasePostData(string.Empty, accountModel, PageResponse);
        TryWithDomain:
            await SetGeneralHeaders(reqParams, accountModel, Referer, postData,Host);
            reqParams.Headers["Quora-Broadcast-Id"] = broadcastId;
            _httpHelper.SetRequestParameter(reqParams);
            var url = $"https://{Host}/graphql/gql_POST?q=questionDownvoteUtils_questionDownvoteAdd_Mutation";
            var postData1 = $"{{\"queryName\":\"questionDownvoteUtils_questionDownvoteAdd_Mutation\",\"variables\":{{\"qid\":{qid}}},\"extensions\":{{\"hash\":\"3ee0d4cf2239f8b379a3920b892f346e01fb36a291d15f97d265d438a75ea88f\"}}}}";
            var DownVoteResponse = _httpHelper.PostRequest(url, postData1);
            if(FailedCount++ <=2 && (DownVoteResponse==null || DownVoteResponse.Response.Contains("Server Error")|| DownVoteResponse.HasError ||DownVoteResponse.Exception!=null && DownVoteResponse.Exception.Message.Contains("Bad Request")))
            {
                Host = QdConstants.HomePageUrl.Replace("https://","");
                Referer = $"{QdConstants.HomePageUrl}/profile/{quoraUser.Username}/questions";
                postData = GetBasePostData(Referer, accountModel, string.Empty);
                goto TryWithDomain;
            }
            return new DownvoteQuestionResponseHandler(DownVoteResponse);
        }


        #endregion

        #region Messages

        public SendMessageResponseHandler SendMessage(DominatorAccountModel accountModel, BasePostData basePostData,
            string UserId, string message, string numOfKnownMessages,string Referer="",string ThreadId="")
        {
            var reqParams = _httpHelper.GetRequestParameter();
            var MarkAsReadResponse = string.Empty;
            var EncodedUserName = WebUtility.UrlEncode(Referer?.Split('/').LastOrDefault(x=>x!=string.Empty));
            Referer =!string.IsNullOrEmpty(Referer) && !Referer.Contains("thread")? $"{QdConstants.HomePageUrl}/profile/{EncodedUserName}":Referer;
            SetGeneralHeaders(reqParams, accountModel,Referer, basePostData);
            _httpHelper.SetRequestParameter(reqParams);
            ThreadId = string.IsNullOrEmpty(ThreadId) ? GetThreadID(UserId) : ThreadId;
            numOfKnownMessages = string.IsNullOrEmpty(numOfKnownMessages) ? "0":numOfKnownMessages;
            if (!string.IsNullOrEmpty(ThreadId) && !string.IsNullOrEmpty(Referer) && !Referer.Contains("thread"))
            {
                Referer = $"{QdConstants.HomePageUrl}/messages/thread/{ThreadId}";
                SetGeneralHeaders(_httpHelper.GetRequestParameter(), accountModel, Referer, null);
                _httpHelper.SetRequestParameter(reqParams);
            }
            var BroadCastMessageAPI = QdConstants.GetBroadCastMessageAPI(ThreadId);
            var BroadCastMessagePostBody =QdConstants.GetBroadCastMessagePostBody(ThreadId,UserId,message.Replace("\"",""),numOfKnownMessages);
            var BroadCastMessageResponse = _httpHelper.PostRequest(BroadCastMessageAPI,Encoding.UTF8.GetBytes(BroadCastMessagePostBody));
            var Response = new SendMessageResponseHandler(BroadCastMessageResponse);
            if (Response != null && Response.Success && !string.IsNullOrEmpty(ThreadId))
                MarkAsReadResponse = MarkMessageAsRead(ThreadId).Response;
            return Response;
        }
        public IResponseParameter MarkMessageAsRead(string ThreadId)
        {
            return _httpHelper.PostRequest($"{QdConstants.HomePageUrl}/graphql/gql_POST?q=markThreadAsRead_threadMarkRead_Mutation", Encoding.UTF8.GetBytes($"{{\"queryName\":\"markThreadAsRead_threadMarkRead_Mutation\",\"variables\":{{\"threadId\":{ThreadId}}},\"extensions\":{{\"hash\":\"92a7ba56a1abb2a0ce12456ecc10bac94423d61492b32e9a94ed34868369166c\"}}}}"));
        }
        public SendMessageResponseHandler SendMessageToNew(DominatorAccountModel accountModel, BasePostData postcontent,
            string jsInit, string message)
        {
            var firstUrl = $"{QdConstants.HomePageUrl}/webnode2/server_call_POST?_h=cmDoKa3UZ%2FHgvu&_m=load_message_modal";
            var firstPostdata = "json=%7B%22args%22%3A%5B%5D%2C%22kwargs%22%3A%7B%22target_uid%22%3A" +
                                postcontent.TargetUid + "%7D%7D&revision=" + postcontent.Revision + "&formkey=" +
                                postcontent.FormKey + "&postkey=" + postcontent.PostKey + "&window_id=" +
                                postcontent.WindowId +
                                "&referring_controller=user&referring_action=profile&parent_cid=*modal*_0&parent_domid=&domids_to_remove=%5B%5D&__hmac=cmDoKa3UZ%2FHgvu&__method=load_message_modal&__e2e_action_id=f4kmkib2io&js_init=%7B%22close_on_click%22%3Atrue%2C%22target_uid%22%3A" +
                                postcontent.TargetUid + "%7D&__metadata=%7B%7D";
            var response = _httpHelper.PostRequest(firstUrl, firstPostdata).Response;
            var js_Init = Utilities.GetBetween(response, "MessagesModalBase\\\", ", "]],");
            var orig = ("{" + Utilities.GetBetween(js_Init, "{", ", {}")).Replace("\\", "").Replace(": ", ":")
                .Replace(": \"", ":\"").Replace(", \"", ",\"");
            js_Init = Uri.EscapeDataString(orig).Replace("%20", "+");
            var toUid = Utilities.GetBetween(response, "to_uid\\\": ", ",");
            var url = $"{QdConstants.HomePageUrl}/webnode2/server_call_POST?_h=osK88btjWPpI33&_m=submit_message";
            var postdata = "json=%7B%22args%22%3A%5B%5D%2C%22kwargs%22%3A%7B%22to_uid%22%3A%22" + toUid +
                           "%22%2C%22msg%22%3A%22" + message.Replace(" ", "+") + "%22%7D%7D&revision=" +
                           postcontent.Revision + "&formkey=" + postcontent.FormKey + "&postkey=" +
                           postcontent.PostKey + "&window_id=" + postcontent.WindowId +
                           "&referring_controller=user&referring_action=profile&__hmac=osK88btjWPpI33&__method=submit_message&__e2e_action_id=f4kq0jjqar&js_init=" +
                           js_Init + "&__metadata=%7B%7D";
            var res = new SendMessageResponseHandler(_httpHelper.PostRequest(url, postdata));
            return res;
        }

        public SendMessageResponseHandler SendMessage(DominatorAccountModel accountModel, string formkey,
            string postKey, string windowId, string uId, string message)
        {

            return new SendMessageResponseHandler(_httpHelper.PostRequest(string.Empty, string.Empty));
        }

        public SendMessageResponseHandler OldSendMessage(DominatorAccountModel accountModel, string formkey,
            string postKey, string windowId, string uId, string message)
        {
            var jsonElements = new QdJsonElements
            {
                Formkey = formkey,
                Postkey = postKey,
                WindowId = windowId
            };
            var requestParameter = new QdRequestParameters(_httpHelper.GetRequestParameter())
            {
                Body = jsonElements,
                Url = "webnode2/server_call_POST?_v=OZq880BkuIvXF6&_m=submit_message"
            };
            var url = requestParameter.GenerateUrl();
            url = $"{QdConstants.HomePageUrl}/" + url;
            requestParameter.PostDataParameters.Remove("json");
            requestParameter.PostDataParameters.Remove("referring_controller");
            requestParameter.PostDataParameters.Add("json",
                "%7B%22args%22%3A%5B%5D%2C%22kwargs%22%3A%7B%22to_uid%22%3A%22" + uId +
                "%22%2C%22msg%22%3A%7B%22sections%22%3A%5B%7B%22type%22%3A%22plain%22%2C%22indent%22%3A0%2C%22quoted%22%3Afalse%2C%22spans%22%3A%5B%7B%22modifiers%22%3A%7B%7D%2C%22text%22%3A%22" +
                message +
                "%22%7D%5D%7D%5D%2C%22caret%22%3A%7B%22start%22%3A%7B%22spanIdx%22%3A0%2C%22sectionIdx%22%3A0%2C%22offset%22%3A19%7D%2C%22end%22%3A%7B%22spanIdx%22%3A0%2C%22sectionIdx%22%3A0%2C%22offset%22%3A19%7D%7D%7D%7D%7D&revision=60d5dd97c94ac5766e30e23907554a2aca3c3e80");
            requestParameter.PostDataParameters.Add("referring_controller",
                "messages&referring_action=index&__vcon_json=%5B%22OZq880BkuIvXF6%22%5D&__vcon_method=submit_message&__e2e_action_id=eygcp07fr7&js_init=%7B%22object_id%22%3Anull%7D&__metadata=%7B%7D");
            var postData = requestParameter.GenerateBody();
            return new SendMessageResponseHandler(_httpHelper.PostRequest(url, postData));
        }

        public ScrapeMessageResponseHandler ReadMessage(DominatorAccountModel accountModel, BasePostData basePostData, string paginationId, bool IsReadAllMessage = false, bool IsReadAllUsers = false)
        {
            int.TryParse(paginationId, out int Pagination);
            var reqParams = _httpHelper.GetRequestParameter();
            SetRequestParameter(reqParams,accountModel,basePostData);
            reqParams.Referer = QdConstants.MessageUrl;
            var msgUrl = IsReadAllMessage?QdConstants.GetAllMessageAPI: QdConstants.GetMessageAPI;
            var postData =IsReadAllMessage?QdConstants.GetAllMessagePostData(paginationId) :QdConstants.GetMessagePostData(Pagination);
            var msgResponse = _httpHelper.PostRequest(msgUrl, postData);
            return new ScrapeMessageResponseHandler(msgResponse, IsReadAllMessage,IsReadAllUsers,accountModel.AccountBaseModel.UserId);
        }

        public ScrapeMessageResponseHandler ScrapeMessagesId(DominatorAccountModel accountModel, string PaginationCount)
        {
            ScrapeMessageResponseHandler scrapeMessageResponseHandler = null;
            try
            {
                var basePostData = GetBasePostData(QdConstants.MessageUrl, accountModel);
                return ReadMessage(accountModel, basePostData, PaginationCount);
            }
            catch (AssertFailedException)
            {
                throw;
            }
            catch (OperationCanceledException ex)
            {
                ex.DebugLog();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return scrapeMessageResponseHandler;
        }

        public string GetChatPaginationPostData(BasePostData basePostData)
        {
            try
            {
                var postData =
                    "json=%7B%22args%22%3A%5B%5D%2C%22kwargs%22%3A%7B%7D%7D&revision=" + basePostData.Revision +
                    "&formkey=" +
                    basePostData.FormKey + "&postkey=" + basePostData.PostKey + "&window_id=" + basePostData.WindowId +
                    "&referring_controller=" + basePostData.ReferringController + "&referring_action=" +
                    basePostData.ReferringAction + "&parent_cid=" + basePostData.ParentCid +
                    "&parent_domid=" + basePostData.ParentDomId +
                    "&parent_domid=w6Wg2dv713&domids_to_remove=%5B%5D&__hmac=xc9pualkCoCoMv&__method=load_menu&__e2e_action_id=f7daty278u&js_init=%7B%22show_menu%22%3Atrue%2C%22alignment%22%3A%22left%22%2C%22css_positioning%22%3Atrue%2C%22attach_to_body%22%3Atrue%2C%22on%22%3Anull%2C%22click_open%22%3Atrue%2C%22load_on_pageload%22%3Afalse%2C%22should_preload_menu%22%3Afalse%2C%22should_show_hover_menu%22%3Atrue%7D&__metadata=%7B%7D";

                return postData;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return string.Empty;
            }
        }

        public string GetFinalChatPaginationPostData(BasePostData basePostData)
        {
            try
            {
                var postData =
                    "json=%7B%22args%22%3A%5B%5D%2C%22kwargs%22%3A%7B%7D%7D&revision=" + basePostData.Revision +
                    "&formkey=" +
                    basePostData.FormKey + "&postkey=" + basePostData.PostKey + "&window_id=" + basePostData.WindowId +
                    "&referring_controller=" + basePostData.ReferringController + "&referring_action=" +
                    basePostData.ReferringAction + "&parent_cid=" + basePostData.ParentCid +
                    "&parent_domid=" + basePostData.ParentDomId +
                    "&parent_domid=w6Wg2dv713&domids_to_remove=%5B%5D&__hmac=xc9pualkCoCoMv&__method=load_menu&__e2e_action_id=f7daty278u&js_init=%7B%22show_menu%22%3Atrue%2C%22alignment%22%3A%22left%22%2C%22css_positioning%22%3Atrue%2C%22attach_to_body%22%3Atrue%2C%22on%22%3Anull%2C%22click_open%22%3Atrue%2C%22load_on_pageload%22%3Afalse%2C%22should_preload_menu%22%3Afalse%2C%22should_show_hover_menu%22%3Atrue%7D&__metadata=%7B%7D";

                postData =
                    "json=%7B%22args%22%3A%5B%5D%2C%22kwargs%22%3A%7B%22previous_url%22%3A%22https%3A%2F%2Fwww.quora.com%2F%22%2C%22previous_title%22%3A%22(%2F28)+Home+-+Quora%22%7D%7D&revision=" +
                    basePostData.Revision +
                    "&formkey=" + basePostData.FormKey +
                    "&postkey=" + basePostData.PostKey +
                    "&window_id=" + basePostData.WindowId +
                    "&referring_controller=" + basePostData.ReferringController +
                    "&referring_action=" + basePostData.ReferringAction +
                    "&parent_cid=*modal*_0&parent_domid=&domids_to_remove=%5B%5D&__hmac=" + basePostData.Hmac +
                    "&__method=load_messages_modal&__e2e_action_id=f7datyuc6k&js_init=%7B%7D&__metadata=%7B%7D";
                return postData;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return string.Empty;
            }
        }

        #endregion

        #region User

        public UserNameByKeywordResponseHandler UserNameByKeyword(DominatorAccountModel accountModel,string queryValue, int paginationCount = 9,bool IsBrowser=true)
        {
            return UserNameByKeywordAsync(accountModel,queryValue,paginationCount,IsBrowser).Result;
        }

        public async Task<UserNameByKeywordResponseHandler> UserNameByKeywordAsync(DominatorAccountModel accountModel,
            string queryValue, int paginationCount = 9, bool IsBrowser = true)
        {
            var request = _httpHelper.GetRequestParameter();
            request.Cookies = accountModel.Cookies;
            await SetGeneralHeaders(request, accountModel);
            var SearchResponse=SearchQueryByType(QdConstants.GetSearchQueryAPI, SearchQueryType.Profiles, queryValue, accountModel, paginationCount);
            return new UserNameByKeywordResponseHandler(SearchResponse,IsBrowser);
        }


        public UserInfoResponseHandler UserInfo(DominatorAccountModel accountModel, string url)
        {
            return UserInfoAsync(accountModel, url).Result;
        }

        public async Task<UserInfoResponseHandler> UserInfoAsync(DominatorAccountModel accountModel, string url)
        {
            if (!url.Contains("https://qr.ae"))
                if (!url.Contains(".quora.com"))
                    url = $"{QdConstants.HomePageUrl}/profile/" + url;
            UserInfoResponseHandler objUserInfoResponseHandler = null;
            var request = _httpHelper.GetRequestParameter();
            request.UserAgent =
                "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/76.0.3809.132 Safari/537.36";
            request.Accept =
                "text / html,application / xhtml + xml,application / xml; q = 0.9,image / webp,image / apng,*/*;q=0.8,application/signed-exchange;v=b3";
            request.Cookies = accountModel.Cookies;
            _httpHelper.SetRequestParameter(request);

            objUserInfoResponseHandler =
                new UserInfoResponseHandler(await _httpHelper.GetRequestAsync(url, accountModel.Token));
            try
            {
                if (objUserInfoResponseHandler.Biography != null && objUserInfoResponseHandler.Biography.Contains("more"))
                {
                    var response = objUserInfoResponseHandler.Response.Response;
                    //string response = _dominatorAccount.HttpHelper.GetRequest("https://www.quora.com/profile/Pooja-Yadav-157").Response;
                    var formkey1 = Utilities.GetBetween(response, "\"formkey\": \"", "\", \"");
                    var postkey1 = Utilities.GetBetween(response, "\"postkey\": \"", "\"");
                    var windowId1 = Utilities.GetBetween(response, "\"windowId\": \"", "\"");
                    var parentHtmlDocument = new HtmlDocument();
                    parentHtmlDocument.LoadHtml(response);
                    var parentdomainId = parentHtmlDocument.DocumentNode
                        .SelectSingleNode("//div[@class='ProfileDescription']//div").Attributes["id"].Value;
                    var parentcid = parentHtmlDocument.DocumentNode
                        .SelectSingleNode("//div[@class='ProfileDescription']//div//div").Attributes["id"].Value
                        .Replace("__w2_", "").Replace("__truncated", "");
                    var elements = new QdJsonElements
                    {
                        Formkey = formkey1,
                        Postkey = postkey1,
                        WindowId = windowId1
                    };
                    var serializeArgs = Utilities.GetBetween(response, "serialized_args\": \"", "==\\n\"")
                        .Replace("/", "%2F").Replace("+", "%2B").Replace("\\", "%5C");
                    _httpHelper.GetRequestParameter().UserAgent =
                        "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/55.0.2883.87 Safari/537.36";
                    var requestParameter = new QdRequestParameters(_httpHelper.GetRequestParameter())
                    {
                        Body = elements,
                        Url = "webnode2/server_call_POST?_v=5hUx08UvS1LWd5&_m=fetch_toggled_component"
                    };
                    url = $"{QdConstants.HomePageUrl}/" + requestParameter.GenerateUrl();
                    requestParameter.PostDataParameters.Add("json",
                        "%7B%22args%22%3A%5B%5D%2C%22kwargs%22%3A%7B%22serialized_args%22%3A%22" + serializeArgs +
                        "%3D%3D%5Cn%22%7D%7D&revision=376f5de5f7bc737d10412563c1b0425ce5b1d79d");
                    requestParameter.PostDataParameters.Add("referring_controller",
                        "user&referring_action=profile&parent_cid=" + parentcid + "&parent_domid=" + parentdomainId +
                        "&domids_to_remove=%5B%5D&__vcon_json=%5B%225hUx08UvS1LWd5%22%5D&__vcon_method=fetch_toggled_component&__e2e_action_id=ez8x847fgw&__metadata=%7B%7D");
                    var postdata = requestParameter.GenerateBody();

                    var temp = new UserInfoResponseHandler(
                        await _httpHelper.PostRequestAsync(url, postdata, accountModel.Token));
                    objUserInfoResponseHandler.Biography = temp.Biography;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }


            return objUserInfoResponseHandler;
        }

        #endregion

        #region Comment

        public CommentOnAnswerResponseHandler UserNameCommentOnAnswer(DominatorAccountModel accountModel, string PaginationId,int PaginationCount)
        {
            var CommentorResponse = GetUserActivityDetailsByType(string.Empty, UserActivityType.AnswerCommentor, accountModel, PaginationCount, PaginationCount < 0 ? "MostRecent" : "Paginated", PaginationId, string.Empty, string.Empty).Result;
            return new CommentOnAnswerResponseHandler(CommentorResponse,false);
        }

        public CommentOnQuestionResponseHandler UserNameCommentOnQuestion(DominatorAccountModel accountModel,string PaginationId,int PaginationCount)
        {
            var CommentorOnQuestionsResponse = GetUserActivityDetailsByType(string.Empty, UserActivityType.QuestionCommentor,accountModel, PaginationCount, PaginationCount < 0 ? "MostRecent" : "Paginated", PaginationId, string.Empty, string.Empty).Result;
            return new CommentOnQuestionResponseHandler(CommentorOnQuestionsResponse,false);
        }

        public CommentResponseHandler CommentOnPost(DominatorAccountModel accountModel,
            string responseFollowerFirstPage, string message, int biid)
        {
            var formkey = Utilities.GetBetween(responseFollowerFirstPage, "\"formkey\": \"", "\", \"");
            var postkey = Utilities.GetBetween(responseFollowerFirstPage, "\"postkey\": \"", "\"");
            var windowId = Utilities.GetBetween(responseFollowerFirstPage, "\"windowId\": \"", "\"");
            var url = $"{QdConstants.HomePageUrl}/webnode2/server_call_POST?_h=1oGABa%2Ff%2F7efYI&_m=add_comment";

            var elements = new QdJsonElements
            {
                Formkey = formkey,
                Postkey = postkey,
                WindowId = windowId
            };
            var requestParameter =
                new QdRequestParameters(_httpHelper.GetRequestParameter()) { Body = elements };
            requestParameter.PostDataParameters.Add("json",
                "%7B%22args%22%3A%5B%5D%2C%22kwargs%22%3A%7B%22oid%22%3A" + biid +
                "%2C%22comment%22%3A%7B%22sections%22%3A%5B%7B%22type%22%3A%22plain%22%2C%22indent%22%3A0%2C%22quoted%22%3Afalse%2C%22spans%22%3A%5B%7B%22modifiers%22%3A%7B%7D%2C%22text%22%3A%22" +
                message +
                "+%22%7D%5D%7D%5D%2C%22caret%22%3A%7B%22start%22%3A%7B%22spanIdx%22%3A0%2C%22sectionIdx%22%3A0%2C%22offset%22%3A8%7D%2C%22end%22%3A%7B%22spanIdx%22%3A0%2C%22sectionIdx%22%3A0%2C%22offset%22%3A8%7D%7D%7D%7D%7D&revision=2632e70624cb9a1d5d661d281246b09c3c449417&referring_controller=board&referring_action=item&_lm_transaction_id=0.5589031972707974&_lm_window_id=" +
                windowId +
                "&__vcon_json=%5B%221oGABa%2Ff%2F7efYI%22%5D&__vcon_method=add_comment&__e2e_action_id=f066mqorju&js_init=%7B%22object_id%22%3A" +
                biid +
                "%2C%22draft_space%22%3A%22%7B%5C%22type%5C%22%3A+%5C%22board_item%5C%22%2C+%5C%22board_item_id%5C%22%3A+" +
                biid + "%7D%22%2C%22comment_type%22%3A%22board_item%22%7D&__metadata=%7B%7D");
            var postdata =
                requestParameter
                    .GenerateBody(); //https://readersguild.quora.com/webnode2/server_call_POST?_v=1oGABa%2Ff%2F7efYI&_m=add_comment 
            return new CommentResponseHandler(_httpHelper.PostRequest(url, postdata));
        }

        #endregion

        #region Profile Url

        public string GetProfileUrl(DominatorAccountModel accountModel)
        {
            return GetProfileUrlAsync(accountModel).Result;
        }

        public async Task<string> GetProfileUrlAsync(DominatorAccountModel accountModel)
        {
            var req = _httpHelper.GetRequestParameter();
            req.Cookies = accountModel.Cookies;
            _httpHelper.SetRequestParameter(req);
            var response = (await _httpHelper.GetRequestAsync(QdConstants.HomePageUrl, accountModel.Token)).Response;
            response = QdUtilities.GetDecodedWebResponse(response,true,true);
            var user = Utilities.GetBetween(Utilities.GetBetween(response, "<a class='user'", ">"), "href='", "'");
            if (string.IsNullOrEmpty(user))
            {
                user = Utilities.GetBetween(response, "profileUrl\":\"", "\"");
                if (string.IsNullOrEmpty(user))
                    user = Utilities.GetBetween(response, "profileUrl\\\":\\\"/profile/", "\\\"");
            }
            user =user.Contains(".quora.com")?user: $"{QdConstants.HomePageUrl}/profile/" + QdUtilities.EncodeGermanSpecialCharacterToUTF8(user);
            return user;
        }

        #endregion

        #region No ref

        public BlogResponseHandler Blog(DominatorAccountModel accountModel, string url) //user blog
        {
            return new BlogResponseHandler(_httpHelper.GetRequest(url));
        }
        public ActivityResponseHandler Activity(DominatorAccountModel accountModel, string url)
        {
            return new ActivityResponseHandler(_httpHelper.GetRequest(url));
        }

        public UserNameByKeywordResponseHandler UserBykeyword(DominatorAccountModel accountModel, string url)
        {
            return new UserNameByKeywordResponseHandler(_httpHelper.GetRequest(url));
        }

        public async Task<AnswerUpvotersResponseHandler> AnswerUpvoters(DominatorAccountModel accountModel, string url,
            string formkey = null, string postkey = null, string windowId = null, string aid = null)
        {
            if (formkey == null)
            {
                return new AnswerUpvotersResponseHandler(await _httpHelper.GetRequestAsync(url, accountModel.Token));
            }

            var elements = new QdJsonElements
            {
                Formkey = formkey,
                Postkey = postkey,
                WindowId = windowId
            };
            var requestParameter =
                new QdRequestParameters(_httpHelper.GetRequestParameter()) { Body = elements };
            requestParameter.PostDataParameters.Add("json",
                "%7B%22args%22%3A%5B%5D%2C%22kwargs%22%3A%7B%22object_id%22%3A" + aid +
                "%7D%7D&revision=60d5dd97c94ac5766e30e23907554a2aca3c3e80&referring_controller=question&referring_action=answer&parent_cid=*modal*_0&parent_domid=&domids_to_remove=%5B%5D&__vcon_json=%5B%22boDT8H8PgPms%2Bj%22%5D&__vcon_method=load_inner&__e2e_action_id=eyghet0ev2&js_init=%7B%22object_id%22%3A" +
                aid + "%7D&__metadata=%7B%7D");
            var urlFinal = url;
            var postData = requestParameter.GenerateBody();
            return new AnswerUpvotersResponseHandler(
                await _httpHelper.PostRequestAsync(urlFinal, postData, accountModel.Token));
        }

        public void SetRequestParameter(IRequestParameters reqParams, DominatorAccountModel accountModel,BasePostData basePostData)
        {
            reqParams.Headers.Clear();
            reqParams.Headers["Host"] = QdConstants.HomePageUrl.Replace("https://","");
            reqParams.KeepAlive = true;
            reqParams.Headers["sec-ch-ua"] = "\" Not A; Brand\";v=\"99\", \"Chromium\";v=\"96\", \"Google Chrome\";v=\"96\"";
            reqParams.Headers["sec-ch-ua-mobile"] = "?0";
            reqParams.UserAgent =
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/83.0.4103.97 Safari/537.36";
            reqParams.Headers["Content-Type"] = QdConstants.GetContentType;
            reqParams.Headers["Quora-Revision"] = basePostData.Revision;
            reqParams.Headers["Quora-Broadcast-Id"] = basePostData.Broadcast_Id;
            reqParams.Headers["Quora-Formkey"] = basePostData.FormKey;
            reqParams.Headers["Quora-Canary-Revision"] = "false";
            reqParams.Headers["Quora-Window-Id"] = basePostData.WindowId;
            reqParams.Headers["sec-ch-ua-platform"] = "\"Windows\"";
            reqParams.Accept = "*/*";
            reqParams.Headers["Origin"] = QdConstants.HomePageUrl;
            reqParams.Headers["Sec-Fetch-Site"] = "same-origin";
            reqParams.Headers["Sec-Fetch-Mode"] = "cors";
            reqParams.Headers["Sec-Fetch-Dest"] = "empty";
            reqParams.Headers["Quora-Page-Creation-Time"] = QdConstants.GetTimeStamp;
            reqParams.Headers["Accept-Language"] = "en-US,en;q=0.9";
            reqParams.Cookies = accountModel.Cookies;
        }

        public TopicResponseHandler Topic(DominatorAccountModel accountModel, string url, SearchQueryType QueryType, string QueryValue,int paginationCount=0)=> 
            new TopicResponseHandler(SearchQueryByType(url, QueryType, QueryValue, accountModel, paginationCount),false);

        public IResponseParameter SearchQueryByType(string Url, SearchQueryType QueryType, string QueryValue, DominatorAccountModel dominatorAccount,int paginationCount)
        {
            IResponseParameter responseParameter = null;
            try
            {
                var failedCount = 0;
                var BasePostData = GetBasePostData(QdConstants.HomePageUrl, dominatorAccount);
                SetRequestParameter(_httpHelper.GetRequestParameter(), dominatorAccount, BasePostData);
            TryAgain:
                responseParameter = _httpHelper.PostRequest(Url, QdConstants.GetPostDataForSearchQuery(QueryType, QueryValue, paginationCount));
                while (responseParameter.Response == null ? false : failedCount++ <= 5 && !responseParameter.Response.Contains("\"hasNextPage\""))
                    goto TryAgain;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return responseParameter;
        }

        public BasePostData GetBasePostData(string Url, DominatorAccountModel accountModel,string Response="")
        {
            var response = string.Empty;
            _httpHelper.GetRequestParameter().Cookies = accountModel.Cookies;
            if (string.IsNullOrEmpty(Response) && !string.IsNullOrEmpty(Url))
                response = _httpHelper.GetRequest(Url).Response;
            else
                response = Response;
            return new BasePostData(response);
        }

        public async Task<IResponseParameter> GetUserActivityDetailsByType(string UserId, UserActivityType activityType, DominatorAccountModel dominatorAccount, int PaginationCount=0,string OrderBy="",string Id="",string MultifeedAfter="",string PageData="",string Domain="www.quora.com",string hash = "")
        {
            IResponseParameter response = new ResponseParameter();
            try
            {
                int FailedCount = 0;
                string QueryName = string.Empty;
                var SearchAPI = QdConstants.UserActivityDetailsAPI(activityType, OrderBy,Domain);
                var SearchPostData = QdConstants.UserActivityDetailsPostBody(activityType, UserId, PaginationCount, OrderBy,Id,MultifeedAfter,PageData, hash);
                TryAgain:
                await SetGeneralHeaders(_httpHelper.GetRequestParameter(), dominatorAccount,string.Empty,null,Domain);
                var postBody = Encoding.UTF8.GetBytes(SearchPostData);
                response = _httpHelper.PostRequest(SearchAPI, postBody);
                while (FailedCount++ <= 4 && string.IsNullOrEmpty(response.Response))
                    goto TryAgain;
            }
            catch(Exception ex) { ex.DebugLog(); }
            return response;
        }
        public string GetUserId(string ProfileUrl)
        {
            try
            {
                var response = _httpHelper.GetRequest(ProfileUrl);
                var UserId=Utilities.GetBetween(response.Response, "\"uid\": ", ",");
                UserId = string.IsNullOrEmpty(UserId) ?Utilities.GetBetween(response.Response, "\\\"uid\\\":", ","):UserId;
                return UserId;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public string GetAnswerId(string AnswerUrl,out string AnswerNodeId)
        {
            var response = _httpHelper.GetRequest(AnswerUrl).Response;
            AnswerNodeId = Utilities.GetBetween(response, "\\\"answer\\\":{\\\"id\\\":\\\"", "\\\"");
            AnswerNodeId = string.IsNullOrEmpty(AnswerNodeId) ?Utilities.GetBetween(response, "\\\"contentType\\\":\\\"answer\\\",\\\"id\\\":\\\"", "\\\""): AnswerNodeId;
            return Utilities.GetBetween(response, "\\\"aid\\\":", ",");
        }

        public async Task<IResponseParameter> GetQuestionsFromTopics(string TopicId, DominatorAccountModel accountModel)
        {
            await SetGeneralHeaders(_httpHelper.GetRequestParameter(), accountModel);
            return _httpHelper.PostRequest($"{QdConstants.HomePageUrl}/graphql/gql_para_POST?q=TopicReadMultifeedLoggedIn_Query", Encoding.UTF8.GetBytes($"{{\"queryName\":\"TopicReadMultifeedLoggedIn_Query\",\"variables\":{{\"multifeedAfter\":null,\"multifeedNumBundlesOnClient\":0,\"tid\":{TopicId},\"first\":10}},\"extensions\":{{\"hash\":\"ce7345e6e21bb63e9fe0c89388e3a44a55223ba616b0a7717fffd279127c5241\"}}}}"));
        }

        public string GetQuestionId(string QuestionUrl)
        {
            var response = _httpHelper.GetRequest(QuestionUrl).Response;
            return Utilities.GetBetween(response, "\\\"qid\\\":", ",");
        }

        public async Task<string> GetAnswerOfQuestionPaginationId(DominatorAccountModel accountModel, string QuestionUrl,string QuestionId="")
        {
            var FailedCount = 0;
        TryAgain:
            QuestionId = string.IsNullOrEmpty(QuestionId) && !string.IsNullOrEmpty(QuestionUrl)?
                GetQuestionId(QuestionUrl): QuestionId;
            var reqParam = _httpHelper.GetRequestParameter();
            await SetGeneralHeaders(reqParam, accountModel, QuestionUrl, null);
            _httpHelper.SetRequestParameter(reqParam);
            var PaginationResponse = _httpHelper.PostRequest($"{QdConstants.HomePageUrl}/graphql/gql_para_POST?q=QuestionCollapsedAnswerLoaderQuery", Encoding.UTF8.GetBytes($"{{\"queryName\":\"QuestionCollapsedAnswerLoaderQuery\",\"variables\":{{\"qid\":{QuestionId}}},\"extensions\":{{\"hash\":\"60e26e0657e582fbcc3964d7fde5de1f919a24a332c71a310098d45e248df4f6\"}}}}")).Response;
            while (FailedCount++ <= 2 && string.IsNullOrEmpty(PaginationResponse))
                goto TryAgain;
            var jsonObject = jsonHandler.ParseJsonToJObject(PaginationResponse);
            var Id = jsonHandler.GetJTokenValue(jsonObject, "data", "question", "id");
            while(FailedCount++<=5 && string.IsNullOrEmpty(Id))
                goto TryAgain;
            FailedCount = 0;
            return Id;
        }

        public async Task<PostQuestionResponseHandler> CreatePost(DominatorAccountModel accountModel, string pagesource, string file, string Title)
        {
            IResponseParameter Response = null;
            try
            {
                var CreatePostAPI = QdConstants.CreatePostAPI;
                var MediaUploadResponse = string.Empty;
                var ReqParameter = _httpHelper.GetRequestParameter();
                var MediaUrl = string.Empty;
                var basePostData = GetBasePostData(string.Empty, accountModel,pagesource);
                if (!string.IsNullOrEmpty(file))
                    MediaUrl = await UploadMedia(accountModel, file, basePostData,ReqParameter);
                else
                    _httpHelper.SetRequestParameter(ReqParameter);
                await SetGeneralHeaders(ReqParameter, accountModel, string.Empty, GetBasePostData(string.Empty,accountModel,pagesource));
                var CreatePostPostBody = QdConstants.CreatePostBodyData(Title.Replace("\"",""), MediaUrl);
                //Post Posting.
                Response = await _httpHelper.PostRequestAsync(CreatePostAPI, Encoding.UTF8.GetBytes(CreatePostPostBody),accountModel.Token);
                var CredentialID = await GetCredentialId(accountModel);
                //FinalPosting.
                var UploadPostPostBody = QdConstants.UploadedPostUrlPostBodyData(Title.Replace("\"",""), MediaUrl, string.IsNullOrEmpty(CredentialID) ? "null" : CredentialID);
                Response = await _httpHelper.PostRequestAsync(QdConstants.FetchUploadPostUrlAPI, Encoding.UTF8.GetBytes(UploadPostPostBody), accountModel.Token);
            }
            catch (Exception Ex)
            {
                Ex.DebugLog();
            }
            return new PostQuestionResponseHandler(Response);
        }

        public async Task<PostQuestionResponseHandler> SharePost(DominatorAccountModel accountModel, string pagesource, string shareUrl, string Title="")
        {
            IResponseParameter Response = null;
            try
            {
                var CreatePostAPI = QdConstants.CreatePostAPI;
                var ReqParameter = _httpHelper.GetRequestParameter();
                var basePostData = GetBasePostData(string.Empty, accountModel, pagesource);
                    _httpHelper.SetRequestParameter(ReqParameter);
                await SetGeneralHeaders(ReqParameter, accountModel, string.Empty, GetBasePostData(string.Empty, accountModel, pagesource));
                var SharePostPostBody = QdConstants.SharePostBodyData(shareUrl);
                Response = await _httpHelper.PostRequestAsync(CreatePostAPI, Encoding.UTF8.GetBytes(SharePostPostBody), accountModel.Token);
                var CredentialID = await GetCredentialId(accountModel);
                //FinalPosting.
                var UploadShareUrlPostBody = QdConstants.UploadedShareUrlPostBodyData(shareUrl, string.IsNullOrEmpty(CredentialID) ? "null" : CredentialID);
                Response = await _httpHelper.PostRequestAsync(QdConstants.FetchUploadPostUrlAPI, Encoding.UTF8.GetBytes(UploadShareUrlPostBody), accountModel.Token);
            }
            catch (Exception Ex)
            {
                Ex.DebugLog();
            }
            return new PostQuestionResponseHandler(Response);
        }

        private async Task SetRequestHeaderForCustomPost(HttpWebRequest reqParam,string UserId,string domain,string Origin="",string Referer="")
        {
            reqParam.Headers["sec-ch-ua"] = "\"Chromium\";v=\"116\", \"Not)A;Brand\";v=\"24\", \"Google Chrome\";v=\"116\"";
            reqParam.Headers["sec-ch-ua-platform"] = "\"Windows\"";
            reqParam.Headers["sec-ch-ua-mobile"] = "?0";
            reqParam.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/116.0.0.0 Safari/537.36";
            reqParam.Accept = "*/*";
            reqParam.Headers["Origin"] =string.IsNullOrEmpty(Origin)?QdConstants.HomePageUrl:Origin;
            reqParam.Headers["Sec-Fetch-Site"] = "same-origin";
            reqParam.Headers["Sec-Fetch-Mode"] = "cors";
            reqParam.Headers["Sec-Fetch-Dest"] = "empty";
            reqParam.Referer = string.IsNullOrEmpty(Referer)? $"{QdConstants.HomePageUrl}/" : Referer;
            reqParam.Headers["Accept-Language"] = "en-GB,en-US;q=0.9,en;q=0.8";
            var cookieContainer = new CookieContainer();
            cookieContainer.Add(new Cookie { Name = "m-b", Value = "cRGEiIo0f_L9dCvNFTiRdA==", Domain = domain });
            cookieContainer.Add(new Cookie { Name = "m-b_lax", Value = "cRGEiIo0f_L9dCvNFTiRdA==", Domain = domain });
            cookieContainer.Add(new Cookie { Name = "m-b_strict", Value = "cRGEiIo0f_L9dCvNFTiRdA==", Domain = domain });
            cookieContainer.Add(new Cookie { Name = "m-s", Value = "uD-5kOwdEnImf3FlqakjJQ==", Domain = domain });
            cookieContainer.Add(new Cookie { Name = "m-login", Value = "1", Domain = domain });
            cookieContainer.Add(new Cookie { Name = "m-uid", Value = UserId, Domain = domain });
            reqParam.CookieContainer = cookieContainer;
        }
        public async Task<string> UploadMedia(DominatorAccountModel accountModel, string file, BasePostData basePostData,IRequestParameters ReqParameter)
        {
            try
            {
                var UploadMediaAPI = QdConstants.MediaUploadAPI;
                var request = (HttpWebRequest)WebRequest.Create(UploadMediaAPI);
                var UserId = string.IsNullOrEmpty(accountModel.AccountBaseModel.UserId) ? GetUserId($"{QdConstants.HomePageUrl}/profile/{accountModel.AccountBaseModel.UserFullName}") : accountModel.AccountBaseModel.UserId;
                var UploadMediaPostBody = QdUtilities.GeneratePostBodyForMediaUpload(new List<string> { file },request, basePostData);
                var finalResponse = await PostRequest(UploadMediaAPI, UploadMediaPostBody, UserId, request, "upload.quora.com", $"{QdConstants.HomePageUrl}", $"{QdConstants.HomePageUrl}/");
                var jsonObject = jsonHandler.ParseJsonToJObject(finalResponse);
                var jArray = jsonHandler.GetJArrayElement(jsonHandler.GetJTokenValue(jsonObject, "qimg_urls"));
                var MediaUrl= jArray.FirstOrDefault().ToString()?.Replace("\"", "");
                return MediaUrl;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return string.Empty;
            }
        }

        public async Task<string> PostRequest(string PostAPI, byte[] PostBody, string UserId, HttpWebRequest request = null,string Domain = "",string Origin="",string Referer="")
        {
            var FinalResponse = string.Empty;
            try
            {
                if (request == null)
                    request = (HttpWebRequest)WebRequest.Create(PostAPI);
                request.Method = "POST";
                request.ContentLength = PostBody.Length;
                await SetRequestHeaderForCustomPost(request,UserId,Domain,Origin,Referer);
                using(var stream = request.GetRequestStream())
                {
                    stream.Write(PostBody, 0, PostBody.Length);
                    using (var streamReader = new StreamReader(request.GetResponse().GetResponseStream()))
                        FinalResponse = streamReader.ReadToEnd();
                }
            }catch(Exception ex) { ex.DebugLog();}
            return FinalResponse;
        }

        public string GetThreadID(string UserId)
        {
            var ThreadId=string.Empty;
            try
            {
                var ThreadResponse = _httpHelper.PostRequest($"{QdConstants.HomePageUrl}/graphql/gql_para_POST?q=UserSendMessageQuery", $"{{\"queryName\":\"UserSendMessageQuery\",\"variables\":{{\"uid\":{UserId}}},\"extensions\":{{\"hash\":\"c75e38689a28916e49252fe6b34d34f9568e2127f2f97dfd1336394cc8902b4b\"}}}}");
                var jsonObject = jsonHandler.ParseJsonToJObject(ThreadResponse.Response);
                ThreadId=jsonHandler.GetJTokenValue(jsonObject,"data","user","viewerThread","url")?.Split('/')?.LastOrDefault(x=>x!=string.Empty);
            }catch(Exception ex) { ex.DebugLog();}
            return ThreadId;
        }

        public async Task<string> GetCredentialId(DominatorAccountModel dominatorAccount)
        {
            var CreateQuestionStepResponse = await _httpHelper.PostRequestAsync($"{QdConstants.HomePageUrl}/graphql/gql_para_POST?q=AskQuestionStepQuery", $"{{\"queryName\":\"AskQuestionStepQuery\",\"variables\":{{\"canShowTranslationStep\":true,\"targetOid\":0,\"isTribeOid\":false,\"isUserOid\":false}},\"extensions\":{{\"hash\":\"1089355e23831662780931be1d995bc5997f8d2450a52b8cea8a6d7b8ff2bc15\"}}}}", dominatorAccount.Token);
            return jsonHandler.GetJTokenValue(jsonHandler.ParseJsonToJObject(CreateQuestionStepResponse.Response), "data", "viewer", "user", "bestCredential", "credentialId");
        }
        public async Task<ScrapePostResponseHandler> ScrapeKeyWordPost(DominatorAccountModel dominatorAccount, string QueryValue, int PaginationCount = 0)
        {
            ScrapePostResponseHandler scrapePost=null;
            try
            {
                var Response = SearchQueryByType(QdConstants.GetSearchQueryAPI,SearchQueryType.Posts, QueryValue, dominatorAccount, PaginationCount);
                scrapePost = new ScrapePostResponseHandler(Response, false);
            }catch(Exception ex) { ex.DebugLog();}
            return scrapePost;
        }
        #endregion
    }
}