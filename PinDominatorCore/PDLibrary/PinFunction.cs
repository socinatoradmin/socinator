using CommonServiceLocator;
using ThreadUtils;
using DominatorHouseCore;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.EmailService;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.SocioPublisher;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Request;
using DominatorHouseCore.Utility;
using Newtonsoft.Json.Linq;
using PinDominatorCore.PDModel;
using PinDominatorCore.PDUtility;
using PinDominatorCore.Request;
using PinDominatorCore.Response;
using PinDominatorCore.Utility;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Unity;
using Newtonsoft.Json;
using DominatorHouseCore.PuppeteerBrowser;
using System.Windows;
using PinDominatorCore.Interface;

namespace PinDominatorCore.PDLibrary
{
    public interface IPinFunction
    {
        string Domain { get; set; }
        IPdHttpHelper HttpHelper { get; set; }
        Task<UserNameInfoPtResponseHandler> GetUserDetails(string user, DominatorAccountModel dominatorAccount, bool isNeedPinsCount=true);
        Task<UserNameInfoPtResponseHandler> GetUserDetailsAsync(DominatorAccountModel dominatorAccount);
        BoardInfoPtResponseHandler GetBoardDetails(string boardUrl, DominatorAccountModel dominatorAccount);
        PinInfoPtResponseHandler GetPinDetails(string pinId, DominatorAccountModel dominatorAccount,bool IsCommentRequired=false);
        Task<int> GetUserPinCount(string ProfileId, DominatorAccountModel dominatorAccountModel);
        Task<int> GetUserFollowerCount(string ProfileId,DominatorAccountModel dominatorAccountModel);
        FollowerAndFollowingPtResponseHandler GetUserFollowings(string screenName, DominatorAccountModel dominatorAccount, string bookMark = null, string ticks = null);
        Task<FollowerAndFollowingPtResponseHandler> GetUserFollowingsAsync(string screenName, CancellationToken token, DominatorAccountModel dominatorAccount, string bookMark = null, string ticks = null);
        FollowerAndFollowingPtResponseHandler GetUserFollowers(string screenName, DominatorAccountModel dominatorAccount, string bookMark = null, string ticks = null);
        Task<FollowerAndFollowingPtResponseHandler> GetUserFollowersAsync(string screenName, CancellationToken token, DominatorAccountModel dominatorAccount, string bookMark = null, string ticks = null);
        Task<SearchPeopleResponseHandler> GetUserByKeywords(string keyword, DominatorAccountModel dominatorAccount, string bookMark = null, string ticks = null);
        FollowerAndFollowingPtResponseHandler GetBoardFollowers(string boardUrl, DominatorAccountModel dominatorAccount, string bookMark = null, string ticks = null);
        BoardsWithKeywordsPtResponseHandler GetBoardsByKeywords(string keyword, DominatorAccountModel dominatorAccount, string bookMark = null, string ticks = null);
        BoardsOfUserResponseHandler GetBoardsOfUser(string user, DominatorAccountModel dominatorAccount, string ticks = null);
        Task<List<PinterestBoard>> GetBoardsOfUserAsync(DominatorAccountModel dominatorAccount, CancellationToken token, string ticks = null);
        SearchAllPinResponseHandler GetAllPinsByKeyword(string keyword, DominatorAccountModel dominatorAccount, string bookMark = null, string ticks = null,bool NeedCommentCount=false);
        PinsByBoardUrlResponseHandler GetPinsByBoardUrl(string boardUrl, DominatorAccountModel dominatorAccount, string bookMark = null, string ticks = null, bool NeedCommentCount = false);
        PinsFromSpecificUserResponseHandler GetPinsFromSpecificUser(string userName, DominatorAccountModel dominatorAccount, string bookMark = null, string ticks = null, bool NeedCommentCount = false);
        NewsFeedPinsResponseHandler GetNewsFeedPins(DominatorAccountModel dominatorAccount, string bookMark = null, string ticks = null);
        FriendshipsResponse FollowUserSingle(DominatorAccountModel _dominatorAccount, string userName);
        BoardResponse FollowBoardSingle(string board, DominatorAccountModel dominatorAccount);
        FriendshipsResponse UnFollowSingleUser(PinterestUser pinUser, DominatorAccountModel dominatorAccount);
        CommentResponse CommentOnPin(string pinId, string message, DominatorAccountModel dominatorAccount);
        RepostPinResponseHandler PostPin(DominatorAccountModel dominatorAccount, string boardUrl, PublisherPostlistModel postDetails,SectionDetails sectionDetails=null);
        void PostPin(string path, string description, string boardUrl, string link, DominatorAccountModel dominatorAccount);
        RepostPinResponseHandler RepostPin(string pinUrl, string boardUrl, DominatorAccountModel dominatorAccount,string userboardurl, string QueryType,CancellationTokenSource jobCancellationTokenSource,SectionDetails sectionDetails=null);
        RepostPinResponseHandler UpdatePin(PinterestPin pin, DominatorAccountModel dominatorAccount);
        void DeletePins(List<string> pinIds, DominatorAccountModel dominatorAccount);
        DeletePinResponseHandler DeletePin(string pinId, DominatorAccountModel dominatorAccount);
        TryResponse TryPin(string path, string note, string pinUrl, DominatorAccountModel dominatorAccount);
        string PinOwner(string pinId, DominatorAccountModel dominatorAccount);
        BoardResponse CreateBoard(BoardInfo info, DominatorAccountModel dominatorAccount);
        BoardResponse CreateBoardSection(BoardResponse BoardResponse,BoardInfo info, DominatorAccountModel dominatorAccount);
        CommentsOfPinResponseHandler GetCommentsOfPin(string pinId, DominatorAccountModel dominatorAccount, string bookMark = null);
        LikeCommentResponseHandler LikeSomeonesComment(string commentId, string pinId, DominatorAccountModel dominatorAccount);
        PinLikersResponseHandler GetUsersWhoTriedPin(string pin, DominatorAccountModel dominatorAccount, string bookMark = null, string ticks = null);
        MessageResponseHandler Message(string userName, string message, DominatorAccountModel dominatorAccount, bool isSendPinAsAMessage = false);
        MessageInvitationsResponseHandler GetMessageInvitations(DominatorAccountModel dominatorAccount, string ticks = null);
        FindMessageResponseHandler FindNewMessage(DominatorAccountModel dominatorAccount, string bookMark = null, string ticks = null);
        BoardInvitationsResponseHandler GetBoardInvitations(DominatorAccountModel dominatorAccount, string ticks = null);
        int GetBoardInvitationsCount(DominatorAccountModel dominatorAccount, string ticks = null);
        AcceptBoardInvitationResponseHandler AcceptBoardInvitation(PinterestBoard pinterestBoard, DominatorAccountModel dominatorAccount);
        AcceptMessageInvitationResponseHandler AcceptMessageInvitation(PinterestUser pinterestUser, DominatorAccountModel dominatorAccount);
        SendBoardInvitationResponseHandler SendBoardInvitation(PinterestBoard pinterestBoard, DominatorAccountModel dominatorAccount);
        Task<SendResetPasswordLinkResponseHandler> SendResetPasswordLink(DominatorAccountModel dominatorAccount);
        bool ReadResetPasswordLinkFromEmail(DominatorAccountModel dominatorAccount);
        Task<bool> ResetPasswordWithLink(DominatorAccountModel accountModel);
        IRequestParameters SetHeaders(DominatorAccountModel objDominatorAccountModel);
        IRequestParameters SetHeadersToChangeDomain(DominatorAccountModel objDominatorAccountModel);
        IRequestParameters SetHeadersForHandShake(DominatorAccountModel objDominatorAccountModel, string unauth_id);
        IRequestParameters SetHeadersForHandShakePost(DominatorAccountModel dominatorAccountModel, string appVersion, string experimentHash);
        IRequestParameters SetRequestHeaders(DominatorAccountModel objDominatorAccountModel);
        List<string> UserConnectedWithMessage(DominatorAccountModel dominatorAccountModel);
        bool SwitchToPrivate(DominatorAccountModel dominatorAccount);
        string GetBoardId(string Boardurl, DominatorAccountModel dominatorAccount);  
        Task<CreateAccountRespHandler> CreateAccount(CreateAccountInfo accountInfo, DominatorAccountModel dominatorAccount);
        Task<int> GetUserFollowingCountAsync(string ProfileId, CancellationToken cancellationToken);
        void GetPinComments(ref List<PinterestPin> PinList, DominatorAccountModel dominatorAccount);
        void SetCsrfToken(ref IRequestParameters reqestHeader, ref DominatorAccountModel dominatorAccountModel);
        Task UpdatePinterestBoardSections(DominatorAccountModel dominatorAccount,ref List<PinterestBoard> Boards);
        void DelayBeforeOperation(int delay);
        IResponseParameter TryPost(string Url,string postData,byte[] BytesPostData,string ContainString="",int TryCount=2,bool IsLocked=false);
        Task<IResponseParameter> PuppeteerLogin(DominatorAccountModel dominatorAccount,string Domain,bool IsCloseBrowser=true);
    }

    [Localizable(false)]
    public class PinFunction : IPinFunction
    {
        private readonly IDelayService _delayService;
        private readonly JsonFunct _objJsonFunct = JsonFunct.GetInstance;
        private readonly PdRequestParameters _objRequestParameters = new PdRequestParameters();
        public IPdHttpHelper HttpHelper { get; set; }
        public bool HasUnifiedInbox => false;
        private JsonJArrayHandler jsonHandler = JsonJArrayHandler.GetInstance;
        public string Domain { get; set; }
        private readonly IAccountScopeFactory _accountScopeFactory;
        private IPDAccountSessionManager sessionManager {  get; set; }
        public PuppeteerBrowserActivity browserActivity { get; set; }
        public PinFunction(IPdHttpHelper httpHelper, IDelayService delayService)
        {
            _delayService = delayService;
            HttpHelper = httpHelper;
            _accountScopeFactory = InstanceProvider.GetInstance<IAccountScopeFactory>();
            sessionManager = sessionManager ?? InstanceProvider.GetInstance<IPDAccountSessionManager>();
        }

        public async Task<UserNameInfoPtResponseHandler> GetUserDetails(string user, DominatorAccountModel dominatorAccount, bool isNeedPinsCount=true)
        {
            try
            {
                if (user.Contains("http:") || user.Contains("https:"))
                    user = user.Split('/').LastOrDefault(x => x != string.Empty);
                _objRequestParameters.PdPostElements = _objJsonFunct.GetDataFromJsonFollow(user);
                
                var data = _objRequestParameters.GetJsonString();

                var ticks = PdConstants.GetTicks;

                var objPtRequestParameters = new PdRequestParameters();

                objPtRequestParameters.UrlParameters.Add("source_url", "/" + user+ "_create");
                objPtRequestParameters.UrlParameters.Add("data", data);
                objPtRequestParameters.UrlParameters.Add("_", ticks);
                var url = PdConstants.GetUserDetailsAPI(user, Domain);
                
                SetHeaders(dominatorAccount);

                var response = HttpHelper.GetRequest(url);
                if (response == null || string.IsNullOrEmpty(response.Response)|| response.Response.Contains("\"status\":\"failure\""))
                {
                    url = PdConstants.GetUserProfileAPI(user,Domain);
                    response = HttpHelper.GetRequest(url);
                }
                if (response == null || response != null && string.IsNullOrEmpty(response.Response))
                {
                    DelayBeforeOperation(2000);
                    SetHeaders(dominatorAccount);
                    response = HttpHelper.GetRequest(url);
                    if (response == null || response != null && string.IsNullOrEmpty(response.Response))
                        GlobusLogHelper.log.Info(Log.CustomMessage, dominatorAccount.AccountBaseModel.AccountNetwork,
                            dominatorAccount.AccountBaseModel.UserName, "LangKeyUserInfo".FromResourceDictionary(),
                            "LangKeyThereMightBeServerError".FromResourceDictionary());
                }
                var profileId = string.IsNullOrEmpty(user) ?dominatorAccount.AccountBaseModel.ProfileId: user;
                var objUserNameInfoPtResponseHandler = new UserNameInfoPtResponseHandler(response);
                objUserNameInfoPtResponseHandler.FollowingCount = objUserNameInfoPtResponseHandler.FollowingCount==0?await GetUserFollowingCountAsync(user, dominatorAccount.Token):objUserNameInfoPtResponseHandler.FollowingCount;
                objUserNameInfoPtResponseHandler.FollowerCount = objUserNameInfoPtResponseHandler.FollowerCount == 0 ? await GetUserFollowerCount(profileId,dominatorAccount) : objUserNameInfoPtResponseHandler.FollowerCount;
                if(isNeedPinsCount)
                    objUserNameInfoPtResponseHandler.PinsCount = objUserNameInfoPtResponseHandler.PinsCount == 0 ? await GetUserPinCount(profileId, dominatorAccount) : objUserNameInfoPtResponseHandler.PinsCount;
                return objUserNameInfoPtResponseHandler;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public async Task<UserNameInfoPtResponseHandler> GetUserDetailsAsync(DominatorAccountModel dominatorAccount)
        {
            try
            {
                var url = PdConstants.Https + Domain + "/" + dominatorAccount.AccountBaseModel.ProfileId; //+PdConstants.Following;
                SetHeaders(dominatorAccount);
                var response = await HttpHelper.GetRequestAsync(url, dominatorAccount.Token);
                var objUserNameInfoPtResponseHandler = new UserNameInfoPtResponseHandler(response);
                objUserNameInfoPtResponseHandler.FollowingCount =objUserNameInfoPtResponseHandler.FollowingCount==0? await GetUserFollowingCountAsync(dominatorAccount.AccountBaseModel.ProfileId, dominatorAccount.Token):objUserNameInfoPtResponseHandler.FollowingCount;
                objUserNameInfoPtResponseHandler.PinsCount =objUserNameInfoPtResponseHandler.PinsCount==0? await GetUserPinCount(dominatorAccount.AccountBaseModel.ProfileId,dominatorAccount):objUserNameInfoPtResponseHandler.PinsCount;
                objUserNameInfoPtResponseHandler.FollowerCount = objUserNameInfoPtResponseHandler.FollowerCount == 0 ? await GetUserFollowerCount(dominatorAccount.AccountBaseModel.ProfileId,dominatorAccount) : objUserNameInfoPtResponseHandler.FollowerCount;
                return objUserNameInfoPtResponseHandler;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public BoardInfoPtResponseHandler GetBoardDetails(string boardUrl, DominatorAccountModel dominatorAccount)
        {
            try
            {
                if (!boardUrl.Contains("http"))
                {
                    if (boardUrl[0] == '/')
                        boardUrl = boardUrl.Remove(0, 1);
                    boardUrl = PdConstants.Https + Domain + "/" + boardUrl;
                }
                else
                    boardUrl = PdConstants.Https + Domain + "/" + $"{boardUrl.Split('/')[3]}/{boardUrl.Split('/')[4]}/";

                SetHeaders(dominatorAccount);

                var response = HttpHelper.GetRequest(boardUrl);
                if (!response.Response.Contains("follower_count")&& !response.Response.Contains("pin_count"))
                {
                    DelayBeforeOperation(5000);
                    string ticks = DateTime.Now.Ticks.ToString();
                    var url = $"https://www.pinterest.ca/resource/BoardResource/get/?source_url=%2F{boardUrl.Split('/')[3]}%2F{boardUrl.Split('/')[4]}%2F&data=%7B%22options%22%3A%7B%22username%22%3A%22{boardUrl.Split('/')[3]}%22%2C%22slug%22%3A%22{boardUrl.Split('/')[4]}%22%2C%22field_set_key%22%3A%22detailed%22%2C%22no_fetch_context_on_resource%22%3Afalse%7D%2C%22context%22%3A%7B%7D%7D&_={ticks}";
                    response = HttpHelper.GetRequest(url);
                }
                var objBoardInfoPtResponseHandler = new BoardInfoPtResponseHandler(response);

                if (!objBoardInfoPtResponseHandler.Url.Contains("http"))
                    objBoardInfoPtResponseHandler.Url = PdConstants.Https + Domain + objBoardInfoPtResponseHandler.Url;
                return objBoardInfoPtResponseHandler;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public PinInfoPtResponseHandler GetPinDetails(string pinId, DominatorAccountModel dominatorAccount,bool IsCommentRequired=false)
        {
            try
            {
                var pinUrl = string.Empty;
                if (!pinId.Contains("pinterest"))
                    pinUrl = PdConstants.Https + Domain + PdConstants.Pin + "/" + pinId + "/";
                else
                    pinUrl = PdConstants.Https + Domain + PdConstants.Pin + "/" + pinId.Split('/')[4] + "/";
                SetHeaders(dominatorAccount);
                if (pinId.Contains("pinterest"))
                    pinId = pinId.Split('/')[4];
                var response = HttpHelper.GetRequest(PdConstants.GetPinDetailsAPI(pinId,Domain));
                var objPinInfoPtResponseHandler = new PinInfoPtResponseHandler(response, pinId);
                if (IsCommentRequired)
                {
                    var listOfComments = GetCommentsOfPin(pinId, dominatorAccount);
                    var PaginationToken = listOfComments.BookMark;
                    var hasMoreResult = string.IsNullOrEmpty(PaginationToken) ? false : !PaginationToken.Contains("end");
                    objPinInfoPtResponseHandler.CommentCount = listOfComments.LstComments.Count;
                    while (hasMoreResult)
                    {
                        listOfComments = GetCommentsOfPin(pinId, dominatorAccount, PaginationToken);
                        PaginationToken = listOfComments.BookMark;
                        hasMoreResult = string.IsNullOrEmpty(PaginationToken) ? false : !PaginationToken.Contains("end");
                        objPinInfoPtResponseHandler.CommentCount += listOfComments.LstComments.Count;
                    }
                }
                return objPinInfoPtResponseHandler;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public FollowerAndFollowingPtResponseHandler GetUserFollowings(string screenName, DominatorAccountModel dominatorAccount,
            string bookMark = null, string ticks = null)
        {
            try
            {
                SetRequestHeaders(dominatorAccount);
                var url = string.Empty;
                if (screenName.Contains("pinterest")) screenName = screenName.Split('/')[3];
                string[] bookMarks = { bookMark };
                _objRequestParameters.PdPostElements =
                    _objJsonFunct.GetDataFromUsersFollowersAndFollowings(screenName, bookMarks);
                var data = _objRequestParameters.GetJsonString();

                if (string.IsNullOrEmpty(ticks))
                    ticks = DateTime.Now.Ticks.ToString();

                var objPtRequestParameters = new PdRequestParameters();

                objPtRequestParameters.UrlParameters.Add("source_url", "/" + screenName + PdConstants.Following);
                objPtRequestParameters.UrlParameters.Add("data", data);
                objPtRequestParameters.UrlParameters.Add("_", ticks);

                url = objPtRequestParameters.GenerateUrl(PdConstants.Https + Domain + PdConstants.Resource +
                                                         PdConstants.UserFollowingsResourceGet);

                var pageSource = HttpHelper.GetRequest(url);
                bookMark = Utilities.GetBetween(pageSource.Response, PdConstants.BookMark, "\"");
                var followerResponse =
                    new FollowerAndFollowingPtResponseHandler(pageSource, dominatorAccount.AccountBaseModel.ProfileId)
                    {
                        BookMark = bookMark,
                        HasMoreResults = !bookMark.Contains("end")
                    };
                return followerResponse;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public async Task<FollowerAndFollowingPtResponseHandler> GetUserFollowingsAsync(string screenName, CancellationToken token,
            DominatorAccountModel dominatorAccount, string bookMark = null, string ticks = null)
        {
            try
            {
                SetRequestHeaders(dominatorAccount);
                var url = string.Empty;
                string[] bookMarks = { bookMark };
                _objRequestParameters.PdPostElements =
                    _objJsonFunct.GetDataFromUsersFollowersAndFollowings(screenName, bookMarks);
                var data = _objRequestParameters.GetJsonString();

                var objPtRequestParameters = new PdRequestParameters();

                if (string.IsNullOrEmpty(ticks))
                    ticks = DateTime.Now.Ticks.ToString();

                objPtRequestParameters.UrlParameters.Add("source_url", "/" + screenName + PdConstants.Following);
                objPtRequestParameters.UrlParameters.Add("data", data);
                objPtRequestParameters.UrlParameters.Add("_", ticks);

                url = objPtRequestParameters.GenerateUrl(PdConstants.Https + Domain + PdConstants.Resource +
                                                         PdConstants.UserFollowingsResourceGet);

                var pageSource = await HttpHelper.GetRequestAsync(url, token);

                var followerResponse =
                    new FollowerAndFollowingPtResponseHandler(pageSource, dominatorAccount.AccountBaseModel.ProfileId);
                return followerResponse;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public FollowerAndFollowingPtResponseHandler GetUserFollowers(string screenName, DominatorAccountModel dominatorAccount,
            string bookMark = null, string ticks = null)
        {
            try
            {
                SetRequestHeaders(dominatorAccount);
                if (screenName.Contains("pinterest"))
                    screenName = screenName.Split('/')[3];

                string[] bookMarks = { bookMark };
                _objRequestParameters.PdPostElements = _objJsonFunct.GetDataFromUsersFollowersAndFollowings(screenName, bookMarks);
                var data = _objRequestParameters.GetJsonString();

                if (string.IsNullOrEmpty(ticks))
                    ticks = DateTime.Now.Ticks.ToString();

                var objPtRequestParameters = new PdRequestParameters();

                objPtRequestParameters.UrlParameters.Add("source_url", "/" + screenName + PdConstants.Followers);
                objPtRequestParameters.UrlParameters.Add("data", data);
                objPtRequestParameters.UrlParameters.Add("_", ticks);

                var url = objPtRequestParameters.GenerateUrl(PdConstants.Https + Domain + PdConstants.Resource +
                                                         PdConstants.UserFollowersResourceGet);

                var pageSource = HttpHelper.GetRequest(url);
                bookMark = Utilities.GetBetween(pageSource.Response, PdConstants.BookMark, "\"");
                var followerResponse = new FollowerAndFollowingPtResponseHandler(pageSource, dominatorAccount.AccountBaseModel.ProfileId);
                return followerResponse;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public async Task<FollowerAndFollowingPtResponseHandler> GetUserFollowersAsync(string screenName, CancellationToken token,
            DominatorAccountModel dominatorAccount, string bookMark = null, string ticks = null)
        {
            try
            {
                SetRequestHeaders(dominatorAccount);
                var url = string.Empty;

                string[] bookMarks = { bookMark };
                _objRequestParameters.PdPostElements = _objJsonFunct.GetDataFromUsersFollowersAndFollowings(screenName, bookMarks);
                var data = _objRequestParameters.GetJsonString();

                if (string.IsNullOrEmpty(ticks))
                    ticks = DateTime.Now.Ticks.ToString();

                var objPtRequestParameters = new PdRequestParameters();

                objPtRequestParameters.UrlParameters.Add("source_url", "/" + screenName + PdConstants.Followers);
                objPtRequestParameters.UrlParameters.Add("data", data);
                objPtRequestParameters.UrlParameters.Add("_", ticks);

                url = objPtRequestParameters.GenerateUrl(PdConstants.Https + Domain + PdConstants.Resource +
                                                         PdConstants.UserFollowersResourceGet);

                var pageSource = await HttpHelper.GetRequestAsync(url, token);
                bookMark = Utilities.GetBetween(pageSource.Response, PdConstants.BookMark, "\"");
                var followerResponse = new FollowerAndFollowingPtResponseHandler(pageSource, dominatorAccount.AccountBaseModel.ProfileId)
                {
                    BookMark = bookMark,
                    HasMoreResults = !bookMark.Contains("end")
                };
                return followerResponse;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public async  Task<SearchPeopleResponseHandler> GetUserByKeywords(string keyword, DominatorAccountModel dominatorAccount,
            string bookMark = null, string ticks = null)
        {
            try
            {
                SetRequestHeaders(dominatorAccount);

                string[] bookMarks = { bookMark };
                _objRequestParameters.PdPostElements = _objJsonFunct.GetUsersDataFromKetwords(keyword, bookMarks);
                var data = _objRequestParameters.GetJsonString();

                if (string.IsNullOrEmpty(ticks))
                    ticks = DateTime.Now.Ticks.ToString();

                var objPtRequestParameters = new PdRequestParameters();
                objPtRequestParameters.UrlParameters.Add("source_url", "/search/users/?q=" + Uri.EscapeDataString(keyword) + "&rs=filter");
                objPtRequestParameters.UrlParameters.Add("data", data);
                objPtRequestParameters.UrlParameters.Add("_", ticks);

                var url = objPtRequestParameters.GenerateUrl(PdConstants.Https + Domain + PdConstants.Resource +
                                                         PdConstants.BaseSearchResourceGet);

                var pageSource = HttpHelper.GetRequest(url);
                bookMark = Utilities.GetBetween(pageSource.Response, PdConstants.BookMark, "\"");
                var objSearchPeopleResponseHandler = new SearchPeopleResponseHandler(pageSource)
                {
                    BookMark = bookMark,
                    HasMoreResults = !bookMark.Contains("end")
                };
                return objSearchPeopleResponseHandler;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public FollowerAndFollowingPtResponseHandler GetBoardFollowers(string boardUrl, DominatorAccountModel dominatorAccount,
            string bookMark = null, string ticks = null)
        {
            try
            {
                if (!boardUrl.Contains("pinterest"))
                {
                    if (boardUrl[0] == '/')
                        boardUrl = boardUrl.Remove(0, 1);
                    boardUrl = PdConstants.Https + Domain + "/" + boardUrl;
                }
                else
                    boardUrl = PdConstants.Https + Domain + "/" + boardUrl.Split('/')[3] + "/" + boardUrl.Split('/')[4];
                SetHeaders(dominatorAccount);
                var boardPage = HttpHelper.GetRequest(boardUrl).Response;
                var userName = boardUrl.Split('/')[3];
                var boardUserName = boardUrl.Split('/')[4];
                var RequestHeaderDetails = PdRequestHeaderDetails.GetRequestHeader(boardPage);
                var boardId = RequestHeaderDetails.BoardID;
                if (string.IsNullOrEmpty(boardId))
                    boardId = GetBoardId(boardUrl, dominatorAccount);
                string[] bookMarks = { bookMark };
                _objRequestParameters.PdPostElements = _objJsonFunct.GetDataFromBoardFollowers(boardId, bookMarks);
                var data = _objRequestParameters.GetJsonString();
                var objPtRequestParameters = new PdRequestParameters();
                if (string.IsNullOrEmpty(ticks))
                    ticks = PdConstants.GetTicks;
                objPtRequestParameters.UrlParameters.Add("source_url", "/" + userName + "/" + boardUserName + "/");
                objPtRequestParameters.UrlParameters.Add("data", data);
                objPtRequestParameters.UrlParameters.Add("_", ticks);
                var url = objPtRequestParameters.GenerateUrl(PdConstants.Https + Domain + "/_ngjs" + PdConstants.Resource +
                                                         PdConstants.BoardFollowersResourceGet);
                SetRequestHeaders(dominatorAccount);
                var pageSource = HttpHelper.GetRequest(url);
                bookMark = Utilities.GetBetween(pageSource.Response, PdConstants.BookMark, "\"");
                var followerResponse = new FollowerAndFollowingPtResponseHandler(pageSource, dominatorAccount.AccountBaseModel.ProfileId)
                {
                    BookMark = bookMark,
                    HasMoreResults = !bookMark.Contains("end")
                };
                return followerResponse;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public BoardsWithKeywordsPtResponseHandler GetBoardsByKeywords(string keyword,
            DominatorAccountModel dominatorAccount,
            string bookMark = null, string ticks = null)
        {
            try
            {
                SetRequestHeaders(dominatorAccount);
                var url = string.Empty;

                string[] bookMarks = { bookMark };
                _objRequestParameters.PdPostElements = _objJsonFunct.GetBoardsDataFromKetwords(keyword, bookMarks);
                var data = _objRequestParameters.GetJsonString();
                var objPtRequestParameters = new PdRequestParameters();

                if (string.IsNullOrEmpty(ticks))
                    ticks = DateTime.Now.Ticks.ToString();

                objPtRequestParameters.UrlParameters.Add("source_url",
                    "/search/boards/?q=" + Uri.EscapeDataString(keyword) + "&rs=typed&term_meta[]=" +
                    Uri.EscapeDataString(keyword) + "%7Ctyped");
                objPtRequestParameters.UrlParameters.Add("data", data);
                objPtRequestParameters.UrlParameters.Add("_", ticks);

                url = objPtRequestParameters.GenerateUrl(PdConstants.Https + Domain + PdConstants.Resource +
                                                         PdConstants.BaseSearchResourceGet);

                var pageSource = HttpHelper.GetRequest(url);
                bookMark = Utilities.GetBetween(pageSource.Response, PdConstants.BookMark, "\"");
                var boardsResponse =
                    new BoardsWithKeywordsPtResponseHandler(pageSource)
                    {
                        BookMark = bookMark,
                        HasMoreResults = !bookMark.Contains("end")
                    };
                return boardsResponse;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public BoardsOfUserResponseHandler GetBoardsOfUser(string user, DominatorAccountModel dominatorAccount,
            string ticks = null)
        {
            try
            {
                if (user.Contains("pinterest")) user = user.Split('/')[3];
                SetRequestHeaders(dominatorAccount);
                var url = string.Empty;

                _objRequestParameters.PdPostElements = _objJsonFunct.GetBoardsDataFromUser(user);
                var data = _objRequestParameters.GetJsonString();

                if (string.IsNullOrEmpty(ticks))
                    ticks = DateTime.Now.Ticks.ToString();

                var objPtRequestParameters = new PdRequestParameters();
                objPtRequestParameters.UrlParameters.Add("source_url", "/" + user + "/boards/");
                objPtRequestParameters.UrlParameters.Add("data", data);
                objPtRequestParameters.UrlParameters.Add("_", ticks);

                url = objPtRequestParameters.GenerateUrl(PdConstants.Https + Domain + PdConstants.Resource +
                                                         PdConstants.BoardsResourceGet);

                var pageSource = HttpHelper.GetRequest(url);
                var boardsResponse = new BoardsOfUserResponseHandler(pageSource);

                return boardsResponse;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public async Task<List<PinterestBoard>> GetBoardsOfUserAsync(DominatorAccountModel dominatorAccount,
            CancellationToken token, string ticks = null)
        {
            try
            {
                var lstUserBoard = new List<PinterestBoard>();

                var user = dominatorAccount.AccountBaseModel.ProfileId;
                if (user.Contains("pinterest"))
                {
                    if (user[user.Length - 1] != '/')
                        user = user + "/";
                    user = user.Split('/')[3];
                }

                SetRequestHeaders(dominatorAccount);
                var url = string.Empty;
                _objRequestParameters.PdPostElements = _objJsonFunct.GetBoardsDataFromUser(user);
                var data = _objRequestParameters.GetJsonString();
                var objPtRequestParameters = new PdRequestParameters();

                if (string.IsNullOrEmpty(ticks))
                    ticks = DateTime.Now.Ticks.ToString();

                objPtRequestParameters.UrlParameters.Add("source_url", "/" + user + "/boards/");
                objPtRequestParameters.UrlParameters.Add("data", data);
                objPtRequestParameters.UrlParameters.Add("_", ticks);

                url = objPtRequestParameters.GenerateUrl(PdConstants.Https + Domain + PdConstants.Resource +
                                                         PdConstants.BoardsResourceGet);

                var pageSource = await HttpHelper.GetRequestAsync(url, token);
                var boardsResponse = new BoardsOfUserResponseHandler(pageSource);

                lstUserBoard.AddRange(boardsResponse.BoardsList);

                //For accounts - Board detail is not updating
                if (lstUserBoard.Count == 0)
                {

                    GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Pinterest, dominatorAccount.AccountBaseModel.UserName, 
                        "Board Update", "Please wait Board synchronization is going on...");

                    var urlNew = string.Empty;
                    var count = 100;
                    var bookmarkOfMissingBoard = string.Empty;
                    var missingBoardFailedCount = 0;
                    BoardsOfUserResponseHandler boardsResponseNew = null;
                    do
                    {
                        await _delayService.DelayAsync(TimeSpan.FromSeconds(2), token);
                        string[] bookMark = { boardsResponseNew?.BookMark };

                        _objRequestParameters.PdPostElements = _objJsonFunct.GetUserBoardData(user, count, bookMark);
                        var dataNew = _objRequestParameters.GetJsonString();
                        var ptRequestParameters = new PdRequestParameters();

                        ticks = DateTime.Now.Ticks.ToString();

                        ptRequestParameters.UrlParameters.Add("source_url", "/" + user + "/boards/");
                        ptRequestParameters.UrlParameters.Add("data", dataNew);
                        ptRequestParameters.UrlParameters.Add("_", ticks);

                        urlNew = ptRequestParameters.GenerateUrl(PdConstants.Https + Domain + PdConstants.Resource +
                                                                 PdConstants.BoardsResourceGet);

                        var pageSourceNew = await HttpHelper.GetRequestAsync(urlNew, token);
                        boardsResponseNew = new BoardsOfUserResponseHandler(pageSourceNew);

                        lstUserBoard.AddRange(boardsResponseNew.BoardsList);

                        if (pageSourceNew.Response.Contains("Our server is experiencing a mild case of the hiccups")
                            || pageSourceNew.Response.Contains("upstream request timeout"))
                        {
                            count = 50;
                            boardsResponseNew.BookMark = bookmarkOfMissingBoard;
                            missingBoardFailedCount++;

                            if (missingBoardFailedCount > 0)
                                count = 10;
                        }
                        else
                            bookmarkOfMissingBoard = boardsResponseNew.BookMark;
                    }
                    while (lstUserBoard.Count != 0 && !string.IsNullOrEmpty(boardsResponseNew.BookMark));
                }

                return lstUserBoard;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public SearchAllPinResponseHandler GetAllPinsByKeyword(string keyword, DominatorAccountModel dominatorAccount,
            string bookMark = null, string ticks = null,bool NeedCommentCount=false)
        {
            try
            {
                SetRequestHeaders(dominatorAccount);
                var url = string.Empty;

                string[] bookMarks = { bookMark };
                _objRequestParameters.PdPostElements = _objJsonFunct.GetPinsDataFromKetwords(keyword, bookMarks);
                var data = _objRequestParameters.GetJsonString();
                var objPtRequestParameters = new PdRequestParameters();

                if (string.IsNullOrEmpty(ticks))
                    ticks = PdConstants.GetTicks;

                objPtRequestParameters.UrlParameters.Add("source_url",
                    "/search/pins/?q=" + Uri.EscapeDataString(keyword) + "&rs=typed&term_meta[]=" +
                    Uri.EscapeDataString(keyword) + "%7Ctyped");
                objPtRequestParameters.UrlParameters.Add("data", data);
                objPtRequestParameters.UrlParameters.Add("_", ticks);

                url = objPtRequestParameters.GenerateUrl(PdConstants.Https + Domain + PdConstants.Resource +
                                                         PdConstants.BaseSearchResourceGet);

                var response = HttpHelper.GetRequest(url);
                var objSearchAllPinResponseHandler = new SearchAllPinResponseHandler(response);
                var PinList = objSearchAllPinResponseHandler.LstPin;
                if(NeedCommentCount)
                    GetPinComments(ref PinList,dominatorAccount);
                return objSearchAllPinResponseHandler;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public PinsByBoardUrlResponseHandler GetPinsByBoardUrl(string boardUrl, DominatorAccountModel dominatorAccount,
            string bookMark = null, string ticks = null, bool NeedCommentCount = false)
        {
            try
            {
                SetHeaders(dominatorAccount);

                if (!boardUrl.Contains("pinterest"))
                {
                    if (boardUrl[0] == '/')
                        boardUrl = boardUrl.Remove(0, 1);
                    boardUrl = PdConstants.Https + Domain + "/" + boardUrl;
                }
                else
                    boardUrl = PdConstants.Https + Domain + "/" + boardUrl.Split('/')[3] + "/" + boardUrl.Split('/')[4];

                var boardUrlEdited = "/" + boardUrl.Split('/')[3] + "/" + boardUrl.Split('/')[4] + "/";
                var boardResponse = string.Empty;
                var failedToGetREsponse = 0;
                while (string.IsNullOrEmpty(boardResponse) && failedToGetREsponse <= 5)
                {
                    boardResponse = HttpHelper.GetRequest(boardUrl).Response;
                    failedToGetREsponse++;
                }
                var RequestHeaderDetails = PdRequestHeaderDetails.GetRequestHeader(boardResponse);
                var boardId = RequestHeaderDetails.BoardID;
                if (string.IsNullOrEmpty(boardId))
                    boardId = GetBoardId(boardUrl, dominatorAccount);
                if (string.IsNullOrEmpty(ticks))
                    ticks = PdConstants.GetTicks;

                string[] bookMarks = string.IsNullOrEmpty(bookMark) ? null : new[] { bookMark };
                _objRequestParameters.PdPostElements =
                    _objJsonFunct.GetPinsDataFromBoardUrl(boardId, boardUrlEdited, bookMarks);
                var data = _objRequestParameters.GetJsonString();
                var objPtRequestParameters = new PdRequestParameters();

                objPtRequestParameters.UrlParameters.Add("source_url", boardUrlEdited);
                objPtRequestParameters.UrlParameters.Add("data", data);
                objPtRequestParameters.UrlParameters.Add("_", ticks);
                //objPtRequestParameters.UrlParameters.Add("currentFilter", "-1");
                //objPtRequestParameters.UrlParameters.Add("field_set_key", "react_grid_pin");
                var url = objPtRequestParameters.GenerateUrl(PdConstants.Https + Domain + PdConstants.Resource +
                                                         PdConstants.BoardFeedResourceGet);
                
                var req = SetRequestHeaders(dominatorAccount);
                req.AddHeader("Sec-Fetch-User", "?0");
                req.AddHeader("X-APP-VERSION", RequestHeaderDetails.AppVersion);
                req.AddHeader("Sec-Fetch-Site", "same-origin");
                req.AddHeader("Sec-Fetch-Mode", "cors");
                req.AddHeader("Sec-Fetch-Dest", "empty");
                req.AddHeader("sec-ch-ua-platform", "\"Windows\"");
                req.AddHeader("X-Pinterest-Source-Url", boardUrlEdited);
                req.AddHeader("X-Pinterest-PWS-Handler", "www/[username]/[slug].js");
                req.AddHeader("sec-ch-ua", "\" Not A;Brand\"; v = \"99\",\"Chromium\"; v=\"100\",\"Google Chrome\"; v = \"100\"");
                //req.AddHeader("Accept-Encoding", "gzip, deflate, br");
                req.AddHeader("Accept-Encoding", "gzip, deflate");
                SetCsrfToken(ref req, ref dominatorAccount);
                HttpHelper.GetRequestParameter().Headers = req.Headers;

                var response = HttpHelper.GetRequest(url);
                var objPinsByBoardUrlResponseHandler = new PinsByBoardUrlResponseHandler(response);
                var Pins = objPinsByBoardUrlResponseHandler.LstBoardPin;
                if(NeedCommentCount)
                    GetPinComments(ref Pins,dominatorAccount);
                return objPinsByBoardUrlResponseHandler;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
            
        }
        public string GetBoardId(string Boardurl, DominatorAccountModel dominatorAccount)
        {
          var username = string.Empty;
          var boardname = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(Boardurl))
                {
                    string[] arrlist = Boardurl.Split('/');
                    username = arrlist[3];
                    boardname = arrlist[4];
                }
            }
            catch(Exception)
            {
                
            }
            var url = PdConstants.GetBoardDetailsAPI(Domain, username, boardname);
            var req = HttpHelper.GetRequestParameter();
            HttpHelper.SetRequestParameter(req);
            var boardresponse = HttpHelper.GetRequest(url).Response;
            var jsonObject = jsonHandler.ParseJsonToJObject(boardresponse);               
            return jsonHandler.GetJTokenValue(jsonObject, "resource_response","data","id");
        }
        public PinsFromSpecificUserResponseHandler GetPinsFromSpecificUser(string userName,
            DominatorAccountModel dominatorAccount, string bookMark = null,
            string ticks = null, bool NeedCommentCount = false)
        {
            try
            {
                if (userName.Contains("pinterest"))
                    userName = userName.Split('/')[3];

                SetHeaders(dominatorAccount);
                var url = string.Empty;
                string[] bookmarks = { bookMark };
                _objRequestParameters.PdPostElements = _objJsonFunct.GetPinsDataForSpecificUser(bookmarks, userName);
                var data = _objRequestParameters.GetJsonString();

                if (string.IsNullOrEmpty(ticks))
                    ticks = DateTime.Now.Ticks.ToString();

                var objPtRequestParameters = new PdRequestParameters();
                objPtRequestParameters.UrlParameters.Add("source_url", "/" + userName + "/pins/");
                objPtRequestParameters.UrlParameters.Add("data", data);
                objPtRequestParameters.UrlParameters.Add("_", ticks);

                url = objPtRequestParameters.GenerateUrl(PdConstants.Https + Domain + "/_ngjs" + PdConstants.Resource +
                                                         PdConstants.UserPinsResourceGet);
                SetRequestHeaders(dominatorAccount);
                var response = HttpHelper.GetRequest(url);
                var objPinsFromSpecificUserResponseHandler = new PinsFromSpecificUserResponseHandler(response);
                var Pins = objPinsFromSpecificUserResponseHandler.LstUserPin;
                if(NeedCommentCount)
                    GetPinComments(ref Pins,dominatorAccount);
                return objPinsFromSpecificUserResponseHandler;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public NewsFeedPinsResponseHandler GetNewsFeedPins(DominatorAccountModel dominatorAccount,
            string bookMark = null, string ticks = null)
        {
            try
            {
                SetHeaders(dominatorAccount);
                var url = string.Empty;

                if (string.IsNullOrEmpty(ticks))
                    ticks = DateTime.Now.Ticks.ToString();
                if (string.IsNullOrEmpty(bookMark))
                    url = PdConstants.Https + Domain + "/_ngjs" + PdConstants.Resource +
                          "/UserHomefeedResource/get/?source_url=%2F&data=%7B%22options%22%3A%7B%22isPrefetch%22%3Afalse%2C%22field_set_key%22%3A%22hf_grid%22%2C%22in_nux%22%3Afalse%2C%22prependPartner%22%3Afalse%2C%22prependUserNews%22%3Afalse%2C%22repeatRequestBookmark%22%3A%22%22%2C%22static_feed%22%3Afalse%7D%2C%22context%22%3A%7B%7D%7D&_=" +
                          ticks;
                else
                    url = PdConstants.Https + Domain + "/_ngjs" + PdConstants.Resource +
                          "/UserHomefeedResource/get/?source_url=%2F&data=%7B%22options%22%3A%7B%22bookmarks%22%3A%5B%22" +
                          bookMark +
                          "%3D%3D%22%5D%2C%22isPrefetch%22%3Afalse%2C%22field_set_key%22%3A%22hf_grid%22%2C%22in_nux%22%3Afalse%2C%22prependPartner%22%3Afalse%2C%22prependUserNews%22%3Afalse%2C%22repeatRequestBookmark%22%3A%22%22%2C%22static_feed%22%3Afalse%7D%2C%22context%22%3A%7B%7D%7D&_=" +
                          ticks;
                SetRequestHeaders(dominatorAccount);
                var response = HttpHelper.GetRequest(url);

                var objNewsFeedPinsResponseHandler = new NewsFeedPinsResponseHandler(response);
                var Pins = objNewsFeedPinsResponseHandler.PinList;
                GetPinComments(ref Pins,dominatorAccount);
                return objNewsFeedPinsResponseHandler;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public FriendshipsResponse FollowUserSingle(DominatorAccountModel dominatorAccount, string userName)
        {
            try
            {
                var failedCount = 0;
                TryToFollow:
                if (string.IsNullOrEmpty(userName)) return new FriendshipsResponse(new ResponseParameter(), "");
                var getresponse = HttpHelper.GetRequest(PdConstants.GetUserDetailsAPI(userName,Domain)).Response;
                var jsonHand = new JsonHandler(getresponse);
                FriendshipsResponse objFriendshipsResponse = null;
                var userId = jsonHand.GetElementValue("resource_response", "data", "id");
                var appversion = jsonHand.GetElementValue("client_context", "app_version");
                var isFollowedByMe =
                    jsonHand.GetJTokenValue("resource_response", "data", "explicitly_followed_by_me") == "True" ? true : false;

                if (!isFollowedByMe)
                {
                    var posturl = PdConstants.Https + Domain + PdConstants.Resource + PdConstants.UserFollowResourceCreate;
                    var postdata = $"source_url=%2F{userName}%2F_saved%2F&data=%7B%22options%22%3A%7B%22user_id%22%3A%22{userId}%22%2C%22no_fetch_context_on_resource%22%3Afalse%7D%2C%22context%22%3A%7B%7D%7D";

                    var requestHeader = SetRequestHeaders(dominatorAccount);
                    requestHeader.AddHeader("X-APP-VERSION", appversion);
                    requestHeader.AddHeader("X-Pinterest-PWS-Handler", "www/[username]/_saved.js");
                    var sourceUrl = $"/{userName}/_saved/";
                    requestHeader.AddHeader("X-Pinterest-Source-Url", sourceUrl);
                    requestHeader.AddHeader("sec-ch-ua-platform", "\"Windows\"");
                    requestHeader.AddHeader("Sec-Fetch-Site", "same-origin");
                    requestHeader.AddHeader("Sec-Fetch-Mode", "cors");
                    requestHeader.AddHeader("Sec-Fetch-Dest", "empty");
                    requestHeader.Headers["sec-ch-ua-mobile"] = "?0";
                    requestHeader.Headers["Accept"] = "application/json, text/javascript, */*, q=0.01";
                    //requestHeader.Headers["Accept-Encoding"] = "gzip, deflate, br";
                    requestHeader.Headers["Accept-Encoding"] = "gzip, deflate";
                    requestHeader.Headers["sec-ch-ua"] = "\" Not A; Brand\";v=\"99\", \"Chromium\";v=\"100\", \"Google Chrome\";v=\"100\"";
                    DelayBeforeOperation(2000);
                    var resp = HttpHelper.PostRequest(posturl, postdata);
                    while (failedCount++ < 3 && !resp.Response.Contains("\"status\":\"success\""))
                    {
                        DelayBeforeOperation(3000);
                        goto TryToFollow;
                    }
                    objFriendshipsResponse = new FriendshipsResponse(resp, userId);
                }
                else
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, dominatorAccount.AccountBaseModel.AccountNetwork,
                        dominatorAccount.AccountBaseModel.UserName, ActivityType.Follow,
                        String.Format("LangKeyAlreadyFollowedUserOutsideSoftware".FromResourceDictionary(), userName));
                    objFriendshipsResponse =
                        new FriendshipsResponse(new ResponseParameter { Response = string.Empty }, string.Empty);
                    objFriendshipsResponse.Issue = new PinterestIssue
                    { Message = String.Format("LangKeyAlreadyFollowedUserOutsideSoftware".FromResourceDictionary(), userName) };
                }

                return objFriendshipsResponse;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public BoardResponse FollowBoardSingle(string board, DominatorAccountModel dominatorAccount)
        {
            try
            {
                var boardFollowResponseFailedCount = 0;
                TryToFollowBoard:
                var url = PdConstants.Https + Domain + PdConstants.Resource + PdConstants.BoardFollowResourceCreate;
                var boardUrl = string.Empty;
                if (!board.Contains("pinterest"))
                {
                    if (board[0] == '/')
                        board = board.Remove(0, 1);
                    boardUrl = PdConstants.Https + Domain + "/" + board;
                }
                else
                {
                    boardUrl = board;
                    board = board.Split('/')[3] + "/" + board.Split('/')[4];
                }

                var boardPage = HttpHelper.GetRequest(boardUrl).Response;
                var RequestDetails = PdRequestHeaderDetails.GetRequestHeader(boardPage);
                var boardId = RequestDetails.BoardID;
                if (string.IsNullOrEmpty(boardId))
                {
                    boardId = GetBoardId(boardUrl, dominatorAccount);
                }
                if (board[0] == '/' && board[board.Length - 1] == '/')
                    board = Utilities.GetBetween(board, "/", "/");

                _objRequestParameters.PdPostElements = _objJsonFunct.GetPostDataFromFollowUnfollowBoard(board, boardId);
                var postData =Encoding.UTF8.GetBytes(PdConstants.GetCustomBoardFollowPostData(board,boardId));
                var req=SetRequestHeaders(dominatorAccount);
                req.Headers["X-APP-VERSION"] = RequestDetails.AppVersion;
                req.Headers["X-Pinterest-PWS-Handler"] = "www/[username]/[slug].js";
                req.Headers["X-Pinterest-Source-Url"] =board;
                req.Headers["Accept-Encoding"] = "gzip, deflate";
                DelayBeforeOperation(3000);
                var respFollowBoard = HttpHelper.PostRequest(url, postData);
                while (boardFollowResponseFailedCount++ < 3 && !respFollowBoard.Response.Contains("\"status\":\"success\""))
                {
                    DelayBeforeOperation(3000);
                    goto TryToFollowBoard;
                }
                var objBoardResponse = new BoardResponse(respFollowBoard);
                return objBoardResponse;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }
        public FriendshipsResponse UnFollowSingleUser(PinterestUser pinUser, DominatorAccountModel dominatorAccount)
        {
            try
            {
                if (string.IsNullOrEmpty(pinUser.UserId))
                {
                    SetHeaders(dominatorAccount);
                    var referer = PdConstants.Https + Domain + "/" + pinUser.Username;
                    var userPage = HttpHelper.GetRequest(referer);

                    var count = 0;
                    while (count++ < 5 && string.IsNullOrEmpty(userPage.Response))
                    {
                        _delayService.ThreadSleep(5 * 1000);
                        userPage = HttpHelper.GetRequest(referer);
                    }
                    var RequestHeaderDetails = PdRequestHeaderDetails.GetRequestHeader(userPage?.Response, TokenDetailsType.Users);
                    pinUser.UserId = RequestHeaderDetails.ProfileID;
                }

                var userUrl = PdConstants.Https + Domain + PdConstants.UserFollowResourceDelete;
                _objRequestParameters.PdPostElements = _objJsonFunct.GetPostDataFromUnfollowUser(pinUser.Username, pinUser.UserId);
                var postData = _objRequestParameters.GetPostDataFromJson();
                SetRequestHeaders(dominatorAccount);
                DelayBeforeOperation(3000);
                var resp = HttpHelper.PostRequest(userUrl, postData);
                var failedCount = 0;
                while (failedCount++ < 5 && string.IsNullOrEmpty(resp.Response))
                {
                    DelayBeforeOperation(5000);
                    resp = HttpHelper.PostRequest(userUrl, postData);
                }

                SetHeaders(dominatorAccount);
                var objFriendshipsResponse = new FriendshipsResponse(resp, pinUser.UserId);
                return objFriendshipsResponse;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public void UnFollowSingleBoard(string board, DominatorAccountModel dominatorAccount)
        {
            try
            {
                SetHeaders(dominatorAccount);
                var url = PdConstants.Https + Domain + PdConstants.Resource + PdConstants.BoardFollowResourceDelete;
                var boardUrl = string.Empty;
                var userName = string.Empty;
                if (!board.Contains("pinterest"))
                {
                    boardUrl = PdConstants.Https + Domain + "/" + board;
                    userName = board.Split('/')[0];
                }
                else
                {
                    boardUrl = board;
                    userName = board.Split('/')[3];
                    board = board.Split('/')[3] + "/" + board.Split('/')[4];
                }

                var boardPage = HttpHelper.GetRequest(boardUrl).Response;

                var jsonData = Utilities.GetBetween(boardPage, PdConstants.ApplicationJsonDoubleInitialStateSingle,
                    PdConstants.Script);
                if (string.IsNullOrEmpty(jsonData))
                    jsonData = Utilities.GetBetween(boardPage,
                        PdConstants.ApplicationJsonDoubleInitialStateDouble, PdConstants.Script);
                if (string.IsNullOrEmpty(jsonData))
                    jsonData = Utilities.GetBetween(boardPage, PdConstants.WindowJsonString, PdConstants.Script);
                var jsonHand = new JsonHandler(jsonData);
                var jsonToken = jsonHand.GetJToken("boards", "content")?.First()?.First();
                var boardId = jsonHand.GetJTokenValue(jsonToken, "id");

                board = board.Replace("/", "%2F");
                _objRequestParameters.PdPostElements = _objJsonFunct.GetPostDataFromFollowUnfollowBoard(board, boardId);
                var postData = _objRequestParameters.GetPostDataFromJson();
                SetRequestHeaders(dominatorAccount);
                DelayBeforeOperation(3000);
                HttpHelper.PostRequest(url, postData);

                SetHeaders(dominatorAccount);
                boardPage = HttpHelper.GetRequest(boardUrl).Response;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public CommentResponse CommentOnPin(string pinId, string message, DominatorAccountModel dominatorAccount)
        {
            try
            {
                var failedCount = 0;
                var pinUrl = string.Empty;
                SetHeaders(dominatorAccount);

                if (!pinId.Contains(PdConstants.Pin))
                    pinUrl = PdConstants.Https + Domain + PdConstants.Pin + "/" + pinId + "/";
                else if (!pinId[pinId.Length - 1].Equals('/'))
                    pinUrl = pinId + "/";
                else
                    pinUrl = pinId;
                var pinResponse = HttpHelper.GetRequest(pinUrl);
                while (string.IsNullOrEmpty(pinResponse.Response) && failedCount++ < 5)
                {
                    _delayService.ThreadSleep(5 * 1000);
                    pinResponse = HttpHelper.GetRequest(pinUrl);
                }

                var url = PdConstants.Https + Domain + PdConstants.AggregatedCommentResourceCreate.Replace("_ngjs/","");
                var RequestHeaderDetails = PdRequestHeaderDetails.GetRequestHeader(pinResponse.Response,TokenDetailsType.Pins);
                pinId = Utilities.GetBetween(pinUrl, "pin/", "/");
                var appVirsion= RequestHeaderDetails.AppVersion;
                var aggregatedPinDataId = RequestHeaderDetails.AggregatedPinID;
                var postData = Encoding.UTF8.GetBytes(PdConstants.GetPinCommentPostBody(pinId,aggregatedPinDataId, message?.Replace("\r\n", "\\n")?.Replace("\r", "\\n")));
                var req=SetRequestHeaders(dominatorAccount);
                req.Headers["X-APP-VERSION"] = appVirsion;
                req.Headers["X-Pinterest-PWS-Handler"] = "www/pin/[id].js";
                req.Headers["sec-ch-ua-mobile"] = "?0";
                req.Headers["Content-Type"] = "application/x-www-form-urlencoded";
                req.Headers["Accept"] = "application/json, text/javascript, */*, q=0.01";
                req.Headers["X-Pinterest-Source-Url"] = $"/pin/{pinId}/";
                req.Headers["Sec-Fetch-Site"] = "same-origin";
                req.Headers["Sec-Fetch-Mode"] = "cors";
                req.Headers["Sec-Fetch-Dest"] = "empty";
                //req.Headers["Accept-Encoding"] = "gzip, deflate, br";
                req.Headers["Accept-Encoding"] = "gzip, deflate";
                req.Headers["sec-ch-ua"] = "\" Not A; Brand\";v=\"99\", \"Chromium\";v=\"100\", \"Google Chrome\";v=\"100\"";
                req.Headers["sec-ch-ua-platform"] = "\"Windows\"";
                DelayBeforeOperation(3000);
                var commentPagesource = HttpHelper.PostRequest(url, postData);
                var commentResponse = new CommentResponse(commentPagesource, pinId);

                failedCount = 0;
                while (string.IsNullOrEmpty(commentPagesource.Response) && failedCount++ < 5)
                {
                    _delayService.ThreadSleep(5 * 1000);
                    commentPagesource = HttpHelper.PostRequest(url, postData);
                    commentResponse = new CommentResponse(commentPagesource, pinId);
                }

                return commentResponse;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public RepostPinResponseHandler PostPin(DominatorAccountModel dominatorAccount, string boardUrl, PublisherPostlistModel postDetails,SectionDetails sectionDetails=null)
        {
            try
            {
                var error = string.Empty;
                if (!boardUrl.Contains("pinterest") || !boardUrl.StartsWith("https"))
                {
                    if (boardUrl[0] == '/')
                        boardUrl = PdConstants.Https + Domain + boardUrl;
                    else
                        boardUrl = PdConstants.Https + Domain + "/" + boardUrl;
                }
                SetHeaders(dominatorAccount);
                var boardResponse = HttpHelper.GetRequest(boardUrl).Response;
                var RequestHeaderDetails = PdRequestHeaderDetails.GetRequestHeader(boardResponse);
                var AppVersion = RequestHeaderDetails.AppVersion;
                var boardId = RequestHeaderDetails.BoardID;
                if (string.IsNullOrEmpty(boardId))
                    boardId= GetBoardId(boardUrl, dominatorAccount);
                var imageUrl = string.Empty;
                var SectionId = string.Empty;
                if (sectionDetails != null)
                    SectionId = sectionDetails.SectionId;
                if (postDetails.PostSource == PostSource.ScrapedPost || postDetails.PostSource == PostSource.ScrapeImages
                    || (postDetails.MediaList != null && postDetails.MediaList.Count > 0 && postDetails.MediaList.GetRandomItem().Contains("http")))
                {
                    var image = postDetails.MediaList.GetRandomItem();

                    #region Download Pins and publish
                    var downloadPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\Socinator\\ScrapePins";
                    if (!Directory.Exists(downloadPath))
                        Directory.CreateDirectory(downloadPath);

                    var webClient = new System.Net.WebClient();
                    var downloadDataByte = webClient.DownloadData(image);
                    File.WriteAllBytes($"{downloadPath}\\{ postDetails.FetchedPostIdOrUrl}{dominatorAccount.AccountBaseModel.AccountId}.jpg", downloadDataByte);
                    var mediaFilePath = $"{downloadPath}\\{ postDetails.FetchedPostIdOrUrl}{dominatorAccount.AccountBaseModel.AccountId}.jpg";

                    var fileName = mediaFilePath.Split('\\').Last();
                    #endregion

                    if (postDetails.IsChangeHashOfMedia)
                        image = MediaUtilites.CalculateMD5Hash(image);

                    var url = PdConstants.Https + Domain + PdConstants.UploadImage.Replace("?img=", "");
                    var uploadFile = UploadPicture(url, fileName, mediaFilePath, dominatorAccount);

                    if (string.IsNullOrEmpty(uploadFile) || uploadFile.Contains("Uh oh! Something went wrong."))
                    {
                        url = PdConstants.Https + Domain + PdConstants.UploadImage.Replace("?img=", "");
                        uploadFile = UploadPicture(url, fileName, mediaFilePath, dominatorAccount);
                    }
                    if (Utilities.GetBetween(uploadFile, "\"success\": ", ",") == "false")
                    {
                        error = Utilities.GetBetween(uploadFile, "\"error\": \"", "\"");
                        return new RepostPinResponseHandler(new ResponseParameter
                        {
                            HasError = true,
                            Response = error
                        });
                    }

                    imageUrl = Utilities.GetBetween(uploadFile, PdConstants.ImageUrl, "\"");
                    imageUrl = Uri.EscapeDataString(imageUrl);
                    imageUrl = string.IsNullOrEmpty(imageUrl) ? uploadFile : imageUrl;
                }
                else if (postDetails.PostSource == PostSource.NormalPost || postDetails.PostSource == PostSource.MonitorFolderPost
                         || postDetails.PostSource == PostSource.RssFeedPost)
                {
                    var filePath = postDetails.MediaList.GetRandomItem();
                    if (postDetails.IsChangeHashOfMedia)
                        filePath = MediaUtilites.CalculateMD5Hash(filePath);

                    var fileName = filePath.Split('\\').Last();

                    //For uploading video
                    var mediaType = MediaUtilites.GetMimeTypeByFilePath(filePath);
                    if (mediaType.Contains("video/mp4"))
                        return UploadVideo(dominatorAccount, filePath, fileName, boardUrl, postDetails, boardId,SectionId);

                    var url = PdConstants.Https + Domain + PdConstants.UploadImage.Replace("?img=", "");
                    var uploadFile = UploadPicture(url, fileName, filePath, dominatorAccount);

                    if (string.IsNullOrEmpty(uploadFile) || uploadFile.Contains("Uh oh! Something went wrong."))
                    {
                        url = PdConstants.Https + Domain + PdConstants.UploadImage.Replace("?img=", "");
                        uploadFile = UploadPicture(url, fileName, filePath, dominatorAccount);
                    }

                    if (string.IsNullOrEmpty(uploadFile) || uploadFile.Contains("\"success\": false"))
                        return new RepostPinResponseHandler(new ResponseParameter { Response = string.Empty },
                            string.Empty)
                        {
                            Issue = new PinterestIssue
                            {
                                Message = "Image Not Found"
                            }
                        };

                    imageUrl = Utilities.GetBetween(uploadFile, PdConstants.ImageUrl, "\"");
                    imageUrl = Uri.EscapeDataString(imageUrl);
                    imageUrl = string.IsNullOrEmpty(imageUrl) ? uploadFile : "";
                }
                var description = string.Empty;
                var title = string.Empty;
                if (postDetails.PostDescription.Trim().Length > 500)
                    description = postDetails.PostDescription.Trim().Substring(0, 500);
                else
                    description = postDetails.PostDescription.Trim();
                //description = Uri.EscapeDataString(description);
                if (!string.IsNullOrEmpty(postDetails.PdSourceUrl) && Regex.Split(postDetails.PdSourceUrl, "\r\n").Count() > 1)
                    postDetails.PdSourceUrl = Regex.Split(postDetails.PdSourceUrl, "\r\n").GetRandomItem();

                var websiteLink = !string.IsNullOrEmpty(postDetails.PdSourceUrl) ? Uri.EscapeDataString(postDetails.PdSourceUrl.Trim())
                    : string.Empty;

                if (postDetails.PublisherInstagramTitle == null)
                    postDetails.PublisherInstagramTitle = string.Empty;
                title = postDetails.PublisherInstagramTitle.Trim();
                _objRequestParameters.PdPostElements = _objJsonFunct.GetPostDataFromPost(boardId, dominatorAccount.AccountBaseModel.ProfileId,
                    title, description, websiteLink, imageUrl);

                //var postData = _objRequestParameters.GetPostDataFromJson();
                var PostData = PdConstants.GetUploadMediaPostData(boardId, imageUrl,PdUtility.RemoveSpecialCharacters(description), websiteLink,PdUtility.RemoveSpecialCharacters(title),SectionId);
                var postData = Encoding.UTF8.GetBytes(PostData);
                var postUrl = PdConstants.Https + Domain + PdConstants.Resource + PdConstants.PinResourceCreate;
                var req= SetRequestHeaders(dominatorAccount);
                req.Headers["X-Pinterest-AppState"] = "active";
                req.Headers["X-Pinterest-PWS-Handler"] = "www/pin-builder.js";
                req.Headers["sec-ch-ua-mobile"] = "?0";
                req.Headers["X-APP-VERSION"] = AppVersion;
                req.Headers["X-Pinterest-Source-Url"] = "/pin-builder/";
                req.Headers["sec-ch-ua-platform"] = "\"Windows\"";
                req.Headers["Sec-Fetch-Site"] = "same-origin";
                req.Headers["Sec-Fetch-Mode"] = "cors";
                req.Headers["Sec-Fetch-Dest"] = "empty";
                //req.Headers["Accept-Encoding"] = "gzip, deflate, br";
                req.AddHeader("Accept-Encoding", "gzip, deflate");
                req.Headers["Accept"] = "application/json, text/javascript, */*, q=0.01";
                req.Headers["Content-Type"] = "application/x-www-form-urlencoded";
                req.Headers["sec-ch-ua"] = "\" Not A; Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\"";
                req.Cookies=dominatorAccount.Cookies;
                HttpHelper.SetRequestParameter(req);
                IResponseParameter pinResponse = null;
                var PinPostFailedCount = 0;
                while(PinPostFailedCount++<3)
                {
                    DelayBeforeOperation(3000);
                    pinResponse = HttpHelper.LockedPostRequest(postUrl, postData);
                    if (pinResponse == null || pinResponse.Response.Contains("Something went wrong"))
                        continue;
                    if (!pinResponse.HasError)
                        break;
                }
                var objRepostPinResponseHandler = new RepostPinResponseHandler(pinResponse, boardUrl);
                return objRepostPinResponseHandler;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public RepostPinResponseHandler UploadVideo(DominatorAccountModel dominatorAccount, string localVideoPath,
            string fileName, string boardUrl, PublisherPostlistModel postDetails, string boardId,string SectionId="")
        {
            RepostPinResponseHandler objRepostPinResponseHandler = null;
            try
            {
                var vipResUrl = PdConstants.GetVideoUploadParameterAPI(Domain);
                //_objRequestParameters.PdPostElements = _objJsonFunct.GetPostDataForVideoParameters();
                //var videoParaPostdata = _objRequestParameters.GetPostDataFromJson();
                var videoParaPostdata = PdConstants.GetIdeaPinUploadParameterPostData(localVideoPath);
                SetRequestHeaders(dominatorAccount);

                VideoUploadParametersRespHand videoUploadParametersRespHand = null;
                int i = 0;
                while (videoUploadParametersRespHand == null || string.IsNullOrEmpty(videoUploadParametersRespHand.UploadId) && i++ < 5)
                {
                    var videoParameterResp = HttpHelper.PostRequest(vipResUrl, videoParaPostdata);
                    videoUploadParametersRespHand = new VideoUploadParametersRespHand(videoParameterResp);
                }

                var pdRequestParameters = new PdRequestParameters();
                var fileReader = new FileStream(localVideoPath, FileMode.Open, FileAccess.Read);
                var br = new BinaryReader(fileReader);
                var buffer1 = br.ReadBytes((int)br.BaseStream.Length);
                fileReader.Close();
                br.Close();
                var nvc = new NameValueCollection
                {
                    {"x-amz-date", videoUploadParametersRespHand.Xamzdate},
                    {"x-amz-signature", videoUploadParametersRespHand.Xamzsignature},
                    {"x-amz-security-token", videoUploadParametersRespHand.Xamzsecuritytoken},
                    {"x-amz-algorithm", videoUploadParametersRespHand.Xamzalgorithm},
                    {"key", videoUploadParametersRespHand.Key},
                    {"policy", videoUploadParametersRespHand.Policy},
                    {"x-amz-credential", videoUploadParametersRespHand.Xamzcredential},
                    {"Content-Type", videoUploadParametersRespHand.ContentType}
                };
                var objVideo = new FileData(nvc, fileName, buffer1);
                pdRequestParameters.IsMultiPart = true;
                pdRequestParameters.FileList.Add(localVideoPath, objVideo);
                var postData = pdRequestParameters.GetPostDataFromParameters("video/mp4");

                var req = HttpHelper.GetRequestParameter();
                req.Accept = "*/*";
                req.Headers["Content-Type"] = pdRequestParameters.ContentType;
                req.Headers["Connection"]= "keep-alive";
                req.ContentType = pdRequestParameters.ContentType;
                req.Referer = "https://" + Domain;
                req.UserAgent = PdConstants.UserAgent;
                req.Headers.Remove("X-Pinterest-AppState");
                req.Headers.Remove("X-Requested-With");
                req.Headers.Remove("Sec-Fetch-Site");
                req.Headers.Remove("Sec-Fetch-Mode");
                req.Headers.Remove("Sec-Fetch-Dest");
                req.Headers.Remove("X-CSRFToken");
                req.Headers["Sec-Fetch-Site"]="cross-site";
                req.Headers["Sec-Fetch-Mode"]="cors";
                req.Headers["Sec-Fetch-Dest"]= "empty";

                HttpHelper.SetRequestParameter(req);
                var uploadVideoUrl = PdConstants.GetVideoUploadAPI;
                var failedCount = 0;
            TryAgain:
                var VideoUploadResponse = HttpHelper.PostRequest(uploadVideoUrl, postData).Response;
                while (failedCount++ <= 2 && VideoUploadResponse == null)
                    goto TryAgain;
                var videoSignature = string.Empty;
                if (string.IsNullOrEmpty(VideoUploadResponse))
                {
                    if (!string.IsNullOrEmpty(HttpHelper.Response.Headers["ETag"]))
                        videoSignature = HttpHelper.Response.Headers["ETag"].ToString().Replace("\"", "");
                    videoSignature = string.IsNullOrEmpty(videoSignature) ? "" : videoSignature;
                }

                var objMediaUtilites = new MediaUtilites();
                var thumbnailFilePath = objMediaUtilites.GetThumbnailPng(localVideoPath);
                var url = PdConstants.Https + Domain + PdConstants.UploadImage.Replace("?img=", "");
                var uploadFile = UploadPicture(url, fileName, thumbnailFilePath, dominatorAccount,true);
                var UploadFailedCount = 0;
                while(UploadFailedCount++<2 && uploadFile.Contains("PreconditionFailed"))
                    uploadFile = UploadPicture(url, fileName, thumbnailFilePath, dominatorAccount,true);
                var imageUrl = Utilities.GetBetween(uploadFile, PdConstants.ImageUrl, "\"");
                imageUrl = string.IsNullOrEmpty(imageUrl) ? uploadFile : imageUrl;
                imageUrl = Uri.EscapeDataString(imageUrl);

                var description = string.Empty;
                if (postDetails.PostDescription.Trim().Length > 500)
                {
                    description = postDetails.PostDescription.Trim().Substring(0, 500);
                    GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Pinterest,
                    dominatorAccount.AccountBaseModel.UserName, ActivityType.Post, "Description Is More Than 500 Character, So It will be trimmed upto 500 Character");
                }
                else
                    description = postDetails.PostDescription.Trim();
                description = description.Replace("\"", "\\\"").Replace("\r\n", "").Replace("\r", "");
                //description = Uri.EscapeDataString(description);

                if (!string.IsNullOrEmpty(postDetails.PdSourceUrl) && Regex.Split(postDetails.PdSourceUrl, "\r\n").Count() > 1)
                    postDetails.PdSourceUrl = Regex.Split(postDetails.PdSourceUrl, "\r\n").GetRandomItem();

                var websiteLink = !string.IsNullOrEmpty(postDetails.PdSourceUrl) ? Uri.EscapeDataString(postDetails.PdSourceUrl.Trim())
                    : string.Empty;

                if (postDetails.PublisherInstagramTitle == null)
                    postDetails.PublisherInstagramTitle = string.Empty;
                //var title = Uri.EscapeDataString(postDetails.PublisherInstagramTitle.Trim());
                var title = postDetails.PublisherInstagramTitle.Trim()?.Replace("\r\n", "")?.Replace("\r","");
                _objRequestParameters.PdPostElements = _objJsonFunct.GetPostForPublishVideo(boardId, description, websiteLink,
                    title, imageUrl, videoUploadParametersRespHand.UploadId);
                var UserId = dominatorAccount.AccountBaseModel.UserId;
                //var videoPostData = _objRequestParameters.GetPostDataFromJson();
                if(!string.IsNullOrEmpty(title) && title.Length > 100)
                {
                    title = title.Substring(0, 100);
                    GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Pinterest,
                    dominatorAccount.AccountBaseModel.UserName, ActivityType.Post, "Title Is More Than 100 Character, So It will be trimmed upto 100 Character");
                }
                var videoPostData = _objRequestParameters.GeneratePostDataForVideoUpload(HttpHelper.GetRequest(PdConstants.GetVideoDetailsAPI(string.IsNullOrEmpty(UserId)?videoUploadParametersRespHand.UserId:UserId, Domain)).Response,boardId,PdConstants.EncodeString(description),PdConstants.EncodeString(title),websiteLink,imageUrl,videoSignature,videoUploadParametersRespHand.UploadId,thumbnailFilePath,SectionId);
                //var postUrl = PdConstants.Https + Domain + PdConstants.Resource + PdConstants.PinResourceCreate;
                var postUrl = PdConstants.GetIdeaPinCreateAPI(Domain);
                SetRequestHeaders(dominatorAccount);
                DelayBeforeOperation(3000);
                var pinResponse = HttpHelper.LockedPostRequest(postUrl, videoPostData);
                failedCount = 0;
                while (failedCount++ < 2 && !pinResponse.Response.Contains("\"status\":\"success\"")){DelayBeforeOperation(2500);pinResponse = HttpHelper.LockedPostRequest(postUrl, videoPostData);}
                objRepostPinResponseHandler = new RepostPinResponseHandler(pinResponse, boardUrl);
                if(objRepostPinResponseHandler != null && !string.IsNullOrEmpty(objRepostPinResponseHandler.PinId))
                    RepostPin($"{PdConstants.Https}{Domain}{PdConstants.Pin}/{objRepostPinResponseHandler.PinId}", boardUrl, dominatorAccount, boardUrl, "Custom Pin", dominatorAccount.CancellationSource, new SectionDetails { SectionId = SectionId });
            }
            catch (Exception)
            {
                // ignored
            }

            return objRepostPinResponseHandler;
        }

        public void PostPin(string path, string description, string boardUrl, string link,
            DominatorAccountModel dominatorAccount)
        {
            var fileName = path.Split('\\').Last();
            var url = $"https://{Domain}/upload-image/?img=" + fileName;
            var uploadFile = UploadPicture(url, fileName, path, dominatorAccount);
            var imageUrl = Utilities.GetBetween(uploadFile, "\"image_url\": \"", "\"");
            imageUrl = Uri.EscapeDataString(imageUrl);
            var boardName = boardUrl.Split('/')[4];
            SetHeaders(dominatorAccount);
            var boardResponse = HttpHelper.GetRequest(boardUrl).Response;
            var boardId = Utilities.GetBetween(boardResponse, "{\"field_set_key=", "\"category\":");
            boardId = Utilities.GetBetween(boardId, "\"}, \"id\": \"", "\"");
            var postData = PdConstants.PDsourceUrl + dominatorAccount.AccountBaseModel.ProfileId + "%2F" + boardName +
                           "%2F&data=%7B%22options%22%3A%7B%22description%22%3A%22" + description +
                           "%22%2C%22link%22%3A%22" + link +
                           "%22%2C%22title%22%3A%22%22%2C%22upload_metric%22%3A%7B%22source%22%3A%22pinner_upload_standalone%22%7D%2C%22board_id%22%3A%22" +
                           boardId + "%22%2C%22method%22%3A%22uploaded%22%2C%22image_url%22%3A%22" + imageUrl +
                           "%22%2C%22share_facebook%22%3Afalse%2C%22share_twitter%22%3Afalse%7D%2C%22context%22%3A%7B%7D%7D";
            var postUrl = $"https://{Domain}/resource/PinResource/create/";
            SetRequestHeaders(dominatorAccount);
            DelayBeforeOperation(3000);
            HttpHelper.PostRequest(postUrl, postData);
        }

        public string UploadPicture(string url, string fileName, string localImagePath, DominatorAccountModel dominatorAccount,bool IsVideoThumbnail=false)
        {
            var uploadLink = string.Empty;
            try
            {
                var boardResponse = HttpHelper.GetRequest("https://www.pinterest.ca/").Response;
                var pdRequestParameters = new PdRequestParameters();
                var fileReader = new FileStream(localImagePath, FileMode.Open, FileAccess.Read);
                var br = new BinaryReader(fileReader);
                var buffer1 = br.ReadBytes((int)br.BaseStream.Length);
                fileReader.Close();
                br.Close();
                var nvc = new NameValueCollection();
                var ojbImage = new FileData(nvc, fileName, buffer1);
                pdRequestParameters.IsMultiPart = true;
                pdRequestParameters.FileList.Add(localImagePath, ojbImage);
                SetHeaders(dominatorAccount);
                var failedCount = 0;
                var PinCreateAPI = IsVideoThumbnail ?PdConstants.GetVideoUploadParameterAPI(Domain): PdConstants.GetPinCreateAPI(Domain);
                var PinCreatePostBody = IsVideoThumbnail ?PdConstants.GetThumnailUploadPostData: PdConstants.GetPinCreatePostData;
            TryAgain:
                var CreatePinResponse = HttpHelper.PostRequest(PinCreateAPI, PinCreatePostBody).Response;
                while (failedCount++ < 3 && (string.IsNullOrEmpty(CreatePinResponse) ||!CreatePinResponse.Contains("\"status\":\"success\"")))
                {
                    _delayService.ThreadSleep(2000);
                    goto TryAgain;
                }
                //var postData = pdRequestParameters.GetPostDataFromParameters("image/jpeg");
                var postData = pdRequestParameters.GeneratePostDataForPinUpload($"image/{fileName.Split('.').Last()}", CreatePinResponse);
                var req = HttpHelper.GetRequestParameter();
                req.Accept = "*/*";
                req.Headers["Content-Type"] = pdRequestParameters.ContentType;
                req.Headers["Connection"]= "keep-alive";
                req.ContentType = pdRequestParameters.ContentType;
                req.Referer = "https://" + Domain;
                req.UserAgent = PdConstants.UserAgent;
                req.Headers.Remove("X-Pinterest-AppState");
                req.Headers.Remove("X-Requested-With");
                req.Headers.Remove("Sec-Fetch-Site");
                req.Headers.Remove("Sec-Fetch-Mode");
                req.Headers["Sec-Fetch-Site"]="same-origin";
                req.Headers["Sec-Fetch-Mode"]= "cors";
                req.Headers["Accept-Language"] = "en-US,en;q=0.9";
                HttpHelper.SetRequestParameter(req);
                DelayBeforeOperation(3000);
                var UploadFailedCount = 0;
                var PinUploadAPI = IsVideoThumbnail ?PdConstants.GetThumbnailUploadAPI : PdConstants.GetPostUploadAPI;
            TryAgainPost:
                var imageUploadResponse = HttpHelper.PostRequest(PdConstants.GetPostUploadAPI, postData).Response;
                while(UploadFailedCount++<2 && (imageUploadResponse==null || imageUploadResponse.Contains("Request time out")))
                {
                    DelayBeforeOperation(2000);
                    goto TryAgainPost;
                }
                string ETag = "";
                if (string.IsNullOrEmpty(imageUploadResponse))
                {
                    if(!string.IsNullOrEmpty(HttpHelper.Response.Headers["ETag"]))
                        ETag = HttpHelper.Response.Headers["ETag"].ToString().Replace("\"","");
                    imageUploadResponse = string.IsNullOrEmpty(ETag) ? "" : ETag;
                }
                if (string.IsNullOrEmpty(imageUploadResponse) || imageUploadResponse.Contains("Uh oh! Something went wrong.")
                    || !imageUploadResponse.Contains("success\":true"))
                {
                    if (HttpHelper.GetRequestParameter().Cookies != null && HttpHelper.GetRequestParameter().Cookies.Count != 0)
                        dominatorAccount.Cookies = HttpHelper.GetRequestParameter().Cookies;

                    if (HttpHelper.GetRequestParameter().Cookies != null && HttpHelper.GetRequestParameter().Cookies.Count != 0)
                    {
                        Domain = HttpHelper.GetRequestParameter().Cookies["csrftoken"].Domain;
                        Domain = Domain.Trim('.');
                    }
                    else if (dominatorAccount.Cookies != null && dominatorAccount.Cookies.Count != 0)
                    {
                        Domain = dominatorAccount.Cookies["csrftoken"].Domain;
                        Domain = Domain.Trim('.');
                    }

                    uploadLink = imageUploadResponse;
                }
                if (!string.IsNullOrEmpty(imageUploadResponse) && imageUploadResponse.Contains("success\":true"))
                    uploadLink = imageUploadResponse;
            }
            catch (Exception)
            {
                // ignored
            }

            return uploadLink;
        }
        public RepostPinResponseHandler RepostPin(string pinUrl, string boardUrl, DominatorAccountModel dominatorAccount,string userboardurl,string queryType, CancellationTokenSource JobCancellationTokenSource,SectionDetails sectionDetails=null)
        {
            try
            {
                var PwsHandler = string.Empty;
                var url = PdConstants.Https + Domain + PdConstants.RepinResourceCreate.Replace("/_ngjs", "");
                if (!pinUrl.Contains("pinterest"))
                    pinUrl = PdConstants.Https + Domain + PdConstants.Pin + "/" + pinUrl;
                else
                    pinUrl = PdConstants.Https + Domain + PdConstants.Pin + "/" + pinUrl.Split('/')[4];
                if (!boardUrl.Contains("http"))
                {
                    if (boardUrl[0] == '/')
                        boardUrl = PdConstants.Https + Domain + boardUrl;
                    else
                        boardUrl = PdConstants.Https + Domain + "/" + boardUrl;
                }

                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                var pinResponseOld = HttpHelper.GetRequest(pinUrl);
                var StoryPinID = Utilities.GetBetween(pinResponseOld.Response, "story_pin_data_id\":\"", "\",");
                var pinResponse = string.Empty;
                if (pinResponseOld != null)
                    pinResponse = pinResponseOld.Response;

                var pinResponseFailCount = 1;
                while (string.IsNullOrEmpty(pinResponse) && pinResponseFailCount++ < 5)
                {
                    JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    pinResponseOld = HttpHelper.GetRequest(pinUrl);
                    if (pinResponseOld != null)
                    {
                        pinResponse = pinResponseOld.Response;
                    }
                    _delayService.ThreadSleep(3000);
                }

                if (pinResponse.Contains("show_error=true"))
                    return new RepostPinResponseHandler(new ResponseParameter { Response = string.Empty }) { Success = false, Issue = new PinterestIssue() { Message = String.Format("LangKeyPinUrlNotExist".FromResourceDictionary(), pinUrl) } };
                var RequestHeaderDetails = PdRequestHeaderDetails.GetRequestHeader(pinResponse,TokenDetailsType.Pins);
                var appVersion = RequestHeaderDetails.AppVersion;
                if (pinUrl[pinUrl.Length - 1] != '/')
                    pinUrl = pinUrl + '/';
                var pinId = Utilities.GetBetween(pinUrl, "pin/", "/");
                var description = jsonHandler.GetJTokenValue(RequestHeaderDetails.jToken, "closeup_unified_description").Replace("\n","\\n");
                var websiteUrl = jsonHandler.GetJTokenValue(RequestHeaderDetails.jToken, "link");
                var trackingParam = jsonHandler.GetJTokenValue(RequestHeaderDetails.jToken, "tracking_params");
                var title = jsonHandler.GetJTokenValue(RequestHeaderDetails.jToken, "title");
                title = string.IsNullOrEmpty(title)? jsonHandler.GetJTokenValue(RequestHeaderDetails.jToken, "closeup_unified_title") :title;
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                var boardResponse = HttpHelper.GetRequest(boardUrl).Response;
                var boardResponseFailCount = 1;
                while (string.IsNullOrEmpty(boardResponse) && boardResponseFailCount++ < 5)
                {
                    JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    boardResponse = HttpHelper.GetRequest(boardUrl).Response;
                    _delayService.ThreadSleep(3000);
                }
                RequestHeaderDetails = PdRequestHeaderDetails.GetRequestHeader(boardResponse);
                var errorMessage = "";
                if (!jsonHandler.GetJTokenOfJToken(RequestHeaderDetails.jToken).HasValues)
                    try
                    {
                        errorMessage = jsonHandler.GetJTokenValue(RequestHeaderDetails.jToken, "error", "message");
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                if (!string.IsNullOrEmpty(errorMessage))
                    return new RepostPinResponseHandler(new ResponseParameter { Response = "" }, "")
                    {
                        Issue = new PinterestIssue
                        {
                            Message = errorMessage
                        }
                    };
                var boardId = RequestHeaderDetails.BoardID;
                if (string.IsNullOrEmpty(boardId))
                {                   
                    var boardUri = $"https://www.pinterest.com/resource/BoardsResource/get/?source_url=%2F{boardUrl.Split('/')[3]}%2F{boardUrl.Split('/')[4]}%2F&data=%7B%22options%22%3A%7B%22username%22%3A%22{boardUrl.Split('/')[3]}%22%2C%22slug%22%3A%22{boardUrl.Split('/')[4]}%22%2C%22field_set_key%22%3A%22detailed%22%2C%22no_fetch_context_on_resource%22%3Afalse%7D%2C%22context%22%3A%7B%7D%7D&_={DateTime.Now.Ticks.ToString()}";
                    boardResponse = HttpHelper.GetRequest(boardUri).Response;
                    var jsonHan = new JsonHandler(boardResponse);
                    var jsonToken = jsonHan.GetJToken("resource_response", "data");
                    boardId = jsonHan.GetJTokenValue(jsonToken, "id");
                }
                if (string.IsNullOrEmpty(boardId))
                    boardId = GetBoardId(boardUrl, dominatorAccount);
                if (queryType == "Custom Pin"||queryType== "Keywords")
                {
                    _objRequestParameters.PdPostElements =
                    _objJsonFunct.GetPostDataFromCustomRePin(pinId, boardId, trackingParam, description.Trim(), websiteUrl, userboardurl);
                    userboardurl = $"/pin/{pinId}/";
                    PwsHandler = "www/pin/[id].js";
                }
                else
                {
                    _objRequestParameters.PdPostElements =
                        _objJsonFunct.GetPostDataFromRePin(pinId, boardId, trackingParam, description.Trim(), websiteUrl, userboardurl);
                    PwsHandler = "www/[username]/[slug].js";
                }
                //var postData = _objRequestParameters.GetPostDataFromJson();
                var SectionId = string.Empty;
                if (sectionDetails != null)
                    SectionId = sectionDetails.SectionId;
                var postData =Encoding.UTF8.GetBytes(PdConstants.GetRepinPostData(pinId, trackingParam, description, url, boardId, title, false, false, SectionId,storyPinId: StoryPinID));
                var req= SetRequestHeaders(dominatorAccount);               
                req.AddHeader("Sec-Fetch-User","?0");
                req.AddHeader("X-APP-VERSION" , appVersion);
                req.AddHeader("Sec-Fetch-Site","same-origin");
                req.AddHeader("Sec-Fetch-Mode","cors");
                req.AddHeader("Sec-Fetch-Dest" ,"empty");
                req.AddHeader("sec-ch-ua-platform","\"Windows\"");
                req.AddHeader("X-Pinterest-Source-Url", userboardurl);
                req.AddHeader("X-Pinterest-PWS-Handler", PwsHandler);
                req.AddHeader("sec-ch-ua", "\" Not A;Brand\"; v = \"99\",\"Chromium\"; v=\"100\",\"Google Chrome\"; v = \"100\"");
                //req.AddHeader("Accept-Encoding", "gzip, deflate, br");
                req.AddHeader("Accept-Encoding", "gzip, deflate");
                SetCsrfToken(ref req, ref dominatorAccount); 
                HttpHelper.GetRequestParameter().Headers = req.Headers;
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                DelayBeforeOperation(3000);
                var postResponse = TryPost(url,string.Empty,postData, "\"status\":\"success\"",2,true);
                var objRepostPinResponseHandler = new RepostPinResponseHandler(postResponse, boardUrl);
                return objRepostPinResponseHandler;
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public RepostPinResponseHandler UpdatePin(PinterestPin pin, DominatorAccountModel dominatorAccount)
        {
            RepostPinResponseHandler repostPinResponseHandler = null;
            try
            {
                var boardName = pin.BoardName;
                pin.BoardName = !string.IsNullOrEmpty(boardName) ? boardName.Contains("http:") || boardName.Contains("https:") ? boardName.Split('/').LastOrDefault(x => x != string.Empty) : boardName : boardName;
                SetHeaders(dominatorAccount);
                var pinUrl = $"{PdConstants.Https}{Domain}{PdConstants.Pin}/{pin.PinId}";
                var pinPageSource = HttpHelper.GetRequest(pinUrl).Response;
                var RequestDetails = PdRequestHeaderDetails.GetRequestHeader(pinPageSource,TokenDetailsType.Pins);
                var sectionId = jsonHandler.GetJTokenValue(RequestDetails.jToken, "section");
                if (!string.IsNullOrEmpty(sectionId) && (sectionId.Contains("title") || sectionId.Contains("id") || sectionId.Contains("slug")) && string.IsNullOrEmpty(pin.Section))
                    sectionId = Utilities.GetBetween(sectionId, " \"id\": \"", "\"");
                else
                    sectionId = string.Empty;
                var pinTitle = jsonHandler.GetJTokenValue(RequestDetails.jToken, "title");
                var boardUrlToChange = string.Empty;
                if (string.IsNullOrEmpty(dominatorAccount.AccountBaseModel.ProfileId))
                    dominatorAccount.AccountBaseModel.ProfileId = dominatorAccount.AccountBaseModel.UserName.Split('@').FirstOrDefault().ToLower();
                if (pin.BoardUrl.Contains("http"))
                {
                    boardUrlToChange = pin.BoardUrl;
                }
                else
                {
                    if (pin.BoardUrl.Contains("/"))
                        boardUrlToChange =
                            $"{PdConstants.Https}{Domain}/{dominatorAccount.AccountBaseModel.ProfileId}/{pin.BoardUrl.Split('/')[pin.BoardUrl.Split('/').Length - 2]}";
                    else
                        boardUrlToChange =
                            $"{PdConstants.Https}{Domain}/{dominatorAccount.AccountBaseModel.ProfileId}/{pin.BoardUrl}";
                }
                boardUrlToChange = boardUrlToChange.Replace(" ", "-");
                var boardIdToChange = GetBoardId(boardUrlToChange, dominatorAccount);
                //create new board if board is not exist
                if (string.IsNullOrEmpty(boardIdToChange))
                {
                    boardIdToChange = CreateBoardIfNotExist(pin, dominatorAccount, pinPageSource);
                }
                //create new section if section is not exist in board
                if (string.IsNullOrEmpty(sectionId) && !string.IsNullOrEmpty(pin.Section))
                {
                    DelayBeforeOperation(2000);
                    sectionId = CreateSectionIfNotExist(pin, dominatorAccount, pinPageSource, boardIdToChange);
                }
                var boardSectionId = string.Empty;
                var PinId = string.Empty;
                if (pin.Id.Contains("pinterest"))
                    PinId = pin.Id.Split('/')[4];
                else
                    PinId = pin.Id;

                if (!string.IsNullOrEmpty(pin.Section) && pin.Section.Contains("http"))
                {
                    var boardSectionPageSource = HttpHelper.GetRequest(pin.Section).Response;
                    boardSectionId = Utilities.GetBetween(boardSectionPageSource, "\"sections\": {\"", "\"");
                }
                else if (!string.IsNullOrEmpty(sectionId) &&
                         (string.IsNullOrEmpty(pin.Section) || !pin.Section.ToLower().Contains("no section")))
                    boardSectionId = sectionId;
                else
                    boardSectionId = null;

                var Title = string.Empty;
                if (!string.IsNullOrEmpty(pin.PinName))
                    Title = Uri.UnescapeDataString(pin.PinName);
                else
                    Title = Uri.UnescapeDataString(pinTitle);

                var description = Uri.UnescapeDataString(pin.Description);
                var pinWebUrl = Uri.UnescapeDataString(pin.PinWebUrl);

                _objRequestParameters.PdPostElements = _objJsonFunct.GetPostDataFromEditPin
                (dominatorAccount.AccountBaseModel.ProfileId, pin.BoardName, boardIdToChange, boardSectionId, Title,
                description, PinId, pinWebUrl);
                var postData = Encoding.UTF8.GetBytes(PdConstants.GetEditPinPostBody(pin.PinId,Title,description,boardIdToChange,sectionId, pinWebUrl, "false","false"));
                var urlToEditPin = $"{PdConstants.Https}{Domain}{PdConstants.PinResourceUpdate}";
                var requestHeader = SetRequestHeaders(dominatorAccount);
                requestHeader.Headers["X-APP-VERSION"]=Utilities.GetBetween(pinPageSource, "app_version\":\"", "\"");
                requestHeader.Headers["Accept"]= "application/json, text/javascript, *, q=0.01";
                requestHeader.Headers["Connection"] = "keep-alive";
                requestHeader.Headers["sec-ch-ua"] = "\"Chromium\";v=\"118\", \"Google Chrome\";v=\"118\", \"Not=A?Brand\";v=\"99\"";
                requestHeader.Headers["X-Pinterest-AppState"] = "active";
                requestHeader.Headers["X-Pinterest-PWS-Handler"] = "www/pin/[id].js";
                requestHeader.Headers["sec-ch-ua-mobile"] = "?0";
                requestHeader.Headers["sec-ch-ua-platform-version"] = "10.0.0";
                requestHeader.Headers["X-Requested-With"] = "XMLHttpRequest";
                requestHeader.Headers["X-Pinterest-Source-Url"] = $"/pin/{pin.PinId}/";
                HttpHelper.GetRequestParameter().Headers = requestHeader.Headers;
                DelayBeforeOperation(2000);
                var editPinResponse = TryPost(urlToEditPin, string.Empty, postData, "\"status\":\"success\"", 5);
                repostPinResponseHandler = new RepostPinResponseHandler(editPinResponse, boardUrlToChange + "/");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return repostPinResponseHandler;
        }
        /// <summary>
        /// This method is use to create board if not exits 
        ///use at the time of Edit pins
        /// </summary>
        /// <param name="pin"></param>
        /// <param name="dominatorAccount"></param>
        private string CreateBoardIfNotExist(PinterestPin pin, DominatorAccountModel dominatorAccount, string pinPageSource)
        {
            var boardid = string.Empty;
            try
            {
                var url = $"{PdConstants.Https}{Domain}{PdConstants.Resource}{PdConstants.BoardsResourceCreate}";
                _objRequestParameters.PdPostElements = _objJsonFunct.GetPostDataFromCreateBoard(pin.PinId,
                    pin.BoardUrl, "", "");
                var PostData = _objRequestParameters.GetPostDataFromJson();
                var requestHeaders = SetRequestHeaders(dominatorAccount);
                requestHeaders.AddHeader("X-APP-VERSION", Utilities.GetBetween(pinPageSource, "app_version\":\"", "\""));
                HttpHelper.GetRequestParameter().Headers = requestHeaders.Headers;
                var PinResponse = HttpHelper.PostRequest(url, PostData);
                JsonHandler json = new JsonHandler(PinResponse.Response);
                boardid = json.GetElementValue("resource", "options", "board_id");
                boardid = string.IsNullOrEmpty(boardid) ? json.GetElementValue("resource_response", "data", "id") : boardid;
            }
            catch (Exception)
            {
            }
            return boardid;
        }
        /// <summary>
        /// this method is use to create section if not exist in board
        /// </summary>
        /// <param name="pin"></param>
        /// <param name="dominatorAccount"></param>
        /// <param name="pinPageSource"></param>
        /// <param name="boardIdToChange"></param>
        /// <returns></returns>
        private string CreateSectionIfNotExist(PinterestPin pin, DominatorAccountModel dominatorAccount, string pinPageSource, string boardIdToChange)
        {
            var sectionid = string.Empty;
            try
            {
                var url = $"{PdConstants.Https}{Domain}{PdConstants.Resource}{PdConstants.CreateBoardSectionResource}";
                _objRequestParameters.PdPostElements = _objJsonFunct.GetPostDataFromCreateSectionBoard(pin.PinId, boardIdToChange, pin.Section);
                var PostData = _objRequestParameters.GetPostDataFromJson();
                var requestHeaders = SetRequestHeaders(dominatorAccount);
                requestHeaders.AddHeader("X-APP-VERSION", Utilities.GetBetween(pinPageSource, "app_version\":\"", "\""));
                HttpHelper.GetRequestParameter().Headers = requestHeaders.Headers;
                var BoardSectionResponse = HttpHelper.PostRequest(url, PostData);
                //check if selected board section is already exist or not if exist then get board section id.
                if (!string.IsNullOrEmpty(BoardSectionResponse.Response) && BoardSectionResponse.Response.Contains("A section with that name already exists"))
                {
                    var BoardSectionresponse = HttpHelper.GetRequest(PdConstants.GetBoardSectionAPI(dominatorAccount.AccountBaseModel.ProfileId, pin.BoardName, boardIdToChange,Domain)).Response;
                    var BoardSections = jsonHandler.GetJArrayElement(jsonHandler.GetJTokenValue(jsonHandler.ParseJsonToJObject(BoardSectionresponse), "resource_response", "data"));
                    BoardSectionResponse.Response = BoardSections.Count > 0 ? BoardSections.FirstOrDefault(x => jsonHandler.GetJTokenValue(x, "type")?.Trim() == "board_section" && (jsonHandler.GetJTokenValue(x, "title")?.Trim() == pin.Section || jsonHandler.GetJTokenValue(x, "slug")?.Trim() == pin.Section)).ToString():BoardSectionResponse.Response;
                }
                var json = jsonHandler.ParseJsonToJObject(BoardSectionResponse.Response);
                sectionid = jsonHandler.GetJTokenValue(json,"resource", "options", "section_id");
                sectionid = string.IsNullOrEmpty(sectionid) ? jsonHandler.GetJTokenValue(json,"resource_response", "data", "id") : sectionid;
                sectionid = string.IsNullOrEmpty(sectionid) ? jsonHandler.GetJTokenValue(json,"id") : sectionid;
            }
            catch (Exception ex)
            {
                ex.DebugLog(ex.Message);
            }
            return sectionid;
        }
        public DeletePinResponseHandler DeletePin(string pinId, DominatorAccountModel dominatorAccount)
        {
            try
            {
                var failedCount = 0;
                deletePin:
                SetHeaders(dominatorAccount);
                _objRequestParameters.PdPostElements =
                    _objJsonFunct.GetPostDataFromDeletePins(pinId, dominatorAccount.AccountBaseModel.ProfileId);
                var urlToDeletePin = PdConstants.Https + Domain + PdConstants.Resource + PdConstants.PinResourceDelete;
                var PinDetails = HttpHelper.GetRequest(PdConstants.GetPinDetailsAPI(pinId,Domain));
                if(!string.IsNullOrEmpty(PinDetails.Response) && PinDetails.Response.Contains("Pin not found."))
                    return new DeletePinResponseHandler(PinDetails);
                var jsonObject = jsonHandler.ParseJsonToJObject(PinDetails.Response) ?? jsonHandler.ParseJsonToJObject(HttpHelper.GetRequest(PdConstants.GetPinDetailsAPI(pinId,Domain)).Response);
                var ClientTrackingParams = jsonHandler.GetJTokenValue(jsonObject, "resource_response", "data", "tracking_params");
                var postData = PdConstants.GetPinDeletePostData(pinId, ClientTrackingParams);
               var req= SetRequestHeaders(dominatorAccount);                
                //req.Headers["X-APP-VERSION"] = appVirsion;
                req.Headers["X-Pinterest-PWS-Handler"] = "www/pin/[id].js";
                req.Headers["sec-ch-ua-mobile"] = "?0";
                req.Headers["Content-Type"] = "application/x-www-form-urlencoded";
                req.Headers["Accept"] = "application/json, text/javascript, */*, q=0.01";
                req.Headers["X-Pinterest-Source-Url"] = $"/pin/{pinId}/";
                req.Headers["Sec-Fetch-Site"] = "same-origin";
                req.Headers["Sec-Fetch-Mode"] = "cors";
                req.Headers["Sec-Fetch-Dest"] = "empty";
                //req.Headers["Accept-Encoding"] = "gzip, deflate, br";
                req.AddHeader("Accept-Encoding", "gzip, deflate");
                req.Headers["sec-ch-ua"] = "\" Not A; Brand\";v=\"99\", \"Chromium\";v=\"100\", \"Google Chrome\";v=\"100\"";
                req.Headers["sec-ch-ua-platform"] = "\"Windows\"";
                req.Headers["Content-Length"] = postData.Length.ToString();
                HttpHelper.SetRequestParameter(req);
                DelayBeforeOperation(3000);
                var deletePinResponse = HttpHelper.PostRequest(urlToDeletePin, postData);
                while (failedCount++ < 5 && !deletePinResponse.Response.Contains("\"status\":\"success\""))
                {
                    DelayBeforeOperation(5000);
                    goto deletePin;
                }
                var objDeletePinResponseHandler = new DeletePinResponseHandler(deletePinResponse);
                return objDeletePinResponseHandler;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public TryResponse TryPin(string path, string note, string pinUrl, DominatorAccountModel dominatorAccount)
        {
            try
            {
                var pinId = string.Empty;
                if (pinUrl.Contains("http"))
                {
                    if (pinUrl[pinUrl.Length - 1] == '/')
                        pinId = Utilities.GetBetween(pinUrl, PdConstants.Pin, "/");
                    else
                        pinId = Utilities.GetBetween(pinUrl + "/", "pin/", "/");
                }
                else
                    pinId = pinUrl;

                if (!pinUrl.Contains("http"))
                    pinUrl = PdConstants.Https + Domain + PdConstants.Pin + "/" + pinUrl;

                var fileName = path.Split('\\').Last();
                var url = $"https://{Domain}/upload-image/";
                var uploadFile = UploadPicture(url, fileName, path, dominatorAccount);

                var imageUrl = Utilities.GetBetween(uploadFile, "\"image_url\":\"", "\"");
                var postUrlImageUpload = PdConstants.Https + Domain + PdConstants.DidItImageUploadResourceCreate;

                _objRequestParameters.PdPostElements = _objJsonFunct.GetPostDataForTryImageSig(pinId, imageUrl);
                var postDataImageUpload = _objRequestParameters.GetPostDataFromJson();

                SetRequestHeaders(dominatorAccount);
                var responseImage = HttpHelper.PostRequest(postUrlImageUpload, postDataImageUpload).Response;
                var imageSignature = Utilities.GetBetween(responseImage, "\"image_signature\":\"", "\"");

                var postUrl = PdConstants.Https + Domain + PdConstants.DidItActivityResourceCreate;
                _objRequestParameters.PdPostElements = _objJsonFunct.GetPostDataForTryPin(pinId, note, imageSignature);
                var postData = _objRequestParameters.GetPostDataFromJson();
                DelayBeforeOperation(3000);
                var response = HttpHelper.PostRequest(postUrl, postData);
                var objTryResponse = new TryResponse(response);
                return objTryResponse;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public string PinOwner(string pinId, DominatorAccountModel dominatorAccount)
        {
            try
            {
                var pinUrl = string.Empty;
                if (!pinId.Contains(PdConstants.Pin))
                    pinUrl = PdConstants.Https + Domain + PdConstants.Pin + "/" + pinId + "/";
                else
                {
                    if (pinId[pinId.Length - 1] == '/')
                        pinUrl = pinId;
                    else
                        pinUrl = pinId + "/";
                }
                pinId = Utilities.GetBetween(pinUrl, PdConstants.Https + Domain + PdConstants.Pin + "/", "/");
                SetHeaders(dominatorAccount);
                var pinResponse = HttpHelper.GetRequest(pinUrl).Response;
                var RequestDetails = PdRequestHeaderDetails.GetRequestHeader(pinResponse, TokenDetailsType.Pins);
                var ownerUserName = jsonHandler.GetJTokenValue(RequestDetails.jToken,"pinner", "username");
                return ownerUserName;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public BoardResponse CreateBoard(BoardInfo info, DominatorAccountModel dominatorAccount)
        {
            try
            {
                var category = GetCategory(info.Category);
                var description = info.BoardDescription;

                var url = PdConstants.Https + Domain + PdConstants.Resource + PdConstants.BoardsResourceCreate;
                var boardName = Uri.EscapeDataString(info.BoardName);
                _objRequestParameters.PdPostElements = _objJsonFunct.GetPostDataFromCreateBoard(
                    dominatorAccount.AccountBaseModel.ProfileId,
                    boardName, description, category,info.KeepBoardSecret);
                var postData = _objRequestParameters.GetPostDataFromJson();
                SetRequestHeaders(dominatorAccount);
                DelayBeforeOperation(3000);
                var response = HttpHelper.PostRequestAsync(url, postData, dominatorAccount.CancellationSource.Token)
                    .Result;
                var failedCount = 0;
                while (failedCount++ < 5 && response.Exception != null && response.Exception.Message ==
                       "Cannot re-call BeginGetRequestStream/BeginGetResponse while a previous call is still in progress."
                )
                {
                    DelayBeforeOperation(5000);
                    response = HttpHelper.PostRequest(url, postData);
                }

                while (failedCount++ < 5 && response.Response.ToLower().Contains("<!doctype html>"))
                {
                    HttpHelper.GetRequest(PdConstants.Https + Domain);
                    SetRequestHeaders(dominatorAccount);
                    response = HttpHelper.PostRequest(url, postData);
                }
                if (response.Response.Contains("Our server is experiencing a mild case of the hiccups"))
                    return new BoardResponse(new ResponseParameter { Response = string.Empty })
                    {
                        Issue = new PinterestIssue
                        {
                            Message = "LangKeyServerIsExperiencingMildCaseHiccups".FromResourceDictionary()
                        }
                    };
                var boardResponse = new BoardResponse(response);
                return boardResponse;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public string GetCategory(string category)
        {
            switch (category)
            {
                case "Animals and pets":
                    return "animals";

                case "Architecture":
                    return "architecture";

                case "Art":
                    return "art";

                case "Cars and motorcycles":
                    return "cars_motorcycles";

                case "Celebrities":
                    return "celebrities";

                case "DIY and crafts":
                    return "diy_crafts";

                case "Design":
                    return "design";

                case "Education":
                    return "education";

                case "Entertainment":
                    return "film_music_books";

                case "Food and drink":
                    return "food_drink";

                case "Gardening":
                    return "gardening";

                case "Geek":
                    return "geek";

                case "Hair and beauty":
                    return "hair_beauty";

                case "Health and fitness":
                    return "health_fitness";

                case "History":
                    return "history";

                case "Holidays and events":
                    return "holidays_events";

                case "Home decor":
                    return "home_decor";

                case "Humor":
                    return "humor";

                case "Illustrations and posters":
                    return "illustrations_posters";

                case "Kids and parenting":
                    return "kids";

                case "Men's fashion":
                    return "mens_fashion";

                case "Outdoors":
                    return "outdoors";

                case "Photography":
                    return "photography";

                case "Products":
                    return "products";

                case "Quotes":
                    return "quotes";

                case "Science and nature":
                    return "science_nature";

                case "Sports":
                    return "sports";

                case "Tattoos":
                    return "tattoos";

                case "Technology":
                    return "technology";

                case "Travel":
                    return "travel";

                case "Weddings":
                    return "weddings";

                case "Women's fashion":
                    return "womens_fashion";

                case "Other":
                    return "other";
                default:
                    return "";
            }
        }

        public CommentsOfPinResponseHandler GetCommentsOfPin(string pinId, DominatorAccountModel dominatorAccount,
            string bookMark = null)
        {
            try
            {
                var pinUrl = string.Empty;
                SetHeaders(dominatorAccount);
                if (!pinId.Contains(PdConstants.Pin))
                    pinUrl = PdConstants.Https + Domain + PdConstants.Pin + "/" + pinId + "/";
                else
                {
                    if (pinId[pinId.Length - 1] == '/')
                        pinUrl = pinId;
                    else
                        pinUrl = pinId + "/";
                }

                var pinResponse = HttpHelper.GetRequest(pinUrl).Response;
                var RequestDetails = PdRequestHeaderDetails.GetRequestHeader(pinResponse, TokenDetailsType.Pins);
                pinId = Utilities.GetBetween(pinUrl, "pin/", "/");
                var objectId = RequestDetails.AggregatedPinID;
                var url = string.Empty;
                SetRequestHeaders(dominatorAccount);
                if (bookMark == null)
                    url = PdConstants.GetPinCommentAPI(pinId, objectId, Domain);
                else
                    url = PdConstants.GetPinCommentAPI(pinId, objectId, Domain, PdConstants.GetBookMarkData(bookMark));
                DelayBeforeOperation(2000);
                var response = HttpHelper.GetRequest(url);
                var objCommentsOfPinResponseHandler = new CommentsOfPinResponseHandler(response);

                return objCommentsOfPinResponseHandler;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public LikeCommentResponseHandler LikeSomeonesComment(string commentId, string pinId,
            DominatorAccountModel dominatorAccount)
        {
            try
            {
                if (pinId.Contains(PdConstants.Pin))
                    pinId = Utilities.GetBetween(pinId, PdConstants.Https + Domain + PdConstants.Pin + "/", "/");
                var referer = PdConstants.Https + Domain + PdConstants.Resource +
                              PdConstants.AggregatedCommentLikeResourceCreate;
                _objRequestParameters.PdPostElements =
                    _objJsonFunct.GetPostDataFromLikeSomeonesComment(pinId, commentId);
                //var postData = _objRequestParameters.GetPostDataFromJson();
                var postData = _objRequestParameters.GeneratePostDataForPinLike(pinId,commentId,RandomUtilties.GetRandomNumber(10,1)%2!=0);
                DelayBeforeOperation(3000);
                var response = HttpHelper.PostRequest(PdConstants.GetVideoUploadParameterAPI(Domain), postData);
                var likeCommentResponseHandler = new LikeCommentResponseHandler(response);
                return likeCommentResponseHandler;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public PinLikersResponseHandler GetUsersWhoTriedPin(string pin, DominatorAccountModel dominatorAccount,
            string bookMark = null, string ticks = null)
        {
            try
            {
                var url = string.Empty;
                SetHeaders(dominatorAccount);
                var pinUrl = string.Empty;
                if (string.IsNullOrEmpty(ticks))
                    ticks = DateTime.Now.Ticks.ToString();
                if (!pin.Contains(PdConstants.Pin))
                    pinUrl = PdConstants.Https + Domain + PdConstants.Pin + "/" + pin;
                else
                {
                    pinUrl = pin;
                    if (pin[pin.Length - 1] == '/')
                        pin = Utilities.GetBetween(pin, PdConstants.Pin + "/", "/");
                    else
                        pin = Utilities.GetBetween(pin + "/", PdConstants.Pin + "/", "/");
                }

                var pinResponse = HttpHelper.GetRequest(pinUrl).Response;
                var RequestDetails = PdRequestHeaderDetails.GetRequestHeader(pinResponse, TokenDetailsType.Pins);
                var pinDataId = RequestDetails.AggregatedPinID;
                string[] bookmarks = { bookMark };
                _objRequestParameters.PdPostElements = _objJsonFunct.GetUsersDataFromWhoTriedPin(bookmarks, pinDataId);
                var data = _objRequestParameters.GetJsonString();
                var objPtRequestParameters = new PdRequestParameters();
                objPtRequestParameters.UrlParameters.Add("source_url", "/pin/" + pin + "/activity/tried/");
                objPtRequestParameters.UrlParameters.Add("data", data);
                objPtRequestParameters.UrlParameters.Add("_", ticks);

                url = objPtRequestParameters.GenerateUrl(PdConstants.Https + Domain + PdConstants.Resource +
                                                         PdConstants.AggregatedActivityFeedResourceCreate);

                SetRequestHeaders(dominatorAccount);
                DelayBeforeOperation(3000);
                var response = HttpHelper.GetRequest(url);
                var objPinLikersResponseHandler = new PinLikersResponseHandler(response);
                return objPinLikersResponseHandler;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public MessageResponseHandler Message(string userName, string message, DominatorAccountModel dominatorAccount,
            bool isSendPinAsAMessage = false)
        {
            try
            {
                var failedCount = 0;
                SetHeaders(dominatorAccount);
                JsonHandler jsonHand = null;
                var userId = string.Empty;
                var url = $"https://in.pinterest.com/resource/UserResource/get/?source_url=%2F{userName}%2F_created%2F&data=%7B%22options%22%3A%7B%22isPrefetch%22%3Afalse%2C%22username%22%3A%22{userName}%22%2C%22field_set_key%22%3A%22profile%22%2C%22no_fetch_context_on_resource%22%3Afalse%7D%2C%22context%22%3A%7B%7D%7D&_={DateTime.Now.Ticks.ToString()}";
                var userresponse = HttpHelper.GetRequest(url);
                while (failedCount++ < 2 && string.IsNullOrEmpty(userresponse.Response))
                    userresponse = HttpHelper.GetRequest(url);
                try
                {   
                    jsonHand = new JsonHandler(userresponse.Response);
                    userId = jsonHand.GetElementValue("resource_response", "data", "profile_cover", "id");
                    if (string.IsNullOrEmpty(userId))
                        userId=jsonHand.GetElementValue("resource_response", "data", "id");
                }
                catch (Exception)
                {
                }
                SetRequestHeaders(dominatorAccount);
                message = message.Replace("\r\n", "\\n").Replace("\"", "\\\"");
                message = Uri.EscapeDataString(message);
                var postData = string.Empty;
                
                var postUrl= PdConstants.Https + Domain + PdConstants.ConversationContaxtResource;
                postData = "source_url=%2F"+userName+
                    "%2F_saved%2F&data=%7B%22options%22%3A%7B%22user_ids%22%3A%5B%22"+userId+
                    "%22%5D%2C%22emails%22%3A%5B%5D%2C%22text%22%3A%22"+message+
                    "%22%2C%22pinId%22%3A%22%22%2C%22no_fetch_context_on_resource%22%3Afalse%7D%2C%22context%22%3A%7B%7D%7D";
                DelayBeforeOperation(3000);
                var messageResponse = HttpHelper.PostRequest(postUrl, postData);
                failedCount = 0;
                var objMessageResponseHandler = new MessageResponseHandler(messageResponse);
                while (failedCount++ < 5 && (string.IsNullOrEmpty(messageResponse.Response) || objMessageResponseHandler != null
                    && objMessageResponseHandler.Issue != null && objMessageResponseHandler.Issue.Message.Contains("We could not complete that request.")))
                {
                    DelayBeforeOperation(5000);
                    messageResponse = HttpHelper.PostRequest(postUrl, postData);
                    objMessageResponseHandler = new MessageResponseHandler(messageResponse);
                }

                return objMessageResponseHandler;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public FindMessageResponseHandler FindNewMessage(DominatorAccountModel dominatorAccount, string bookMark = null,
            string ticks = null)
        {
            try
            {
                var userName = dominatorAccount.AccountBaseModel.UserName;
                SetRequestHeaders(dominatorAccount);
                var url = string.Empty;
                if (string.IsNullOrEmpty(ticks))
                    ticks = DateTime.Now.Ticks.ToString();
                if (string.IsNullOrEmpty(bookMark))
                    url = PdConstants.Https + Domain + PdConstants.ConversationsResourceGet
                          + "?source_url=%2F" + userName +
                          "%2F&data=%7B%22options%22%3A%7B%22isPrefetch%22%3Afalse%7D%2C%22context%22%3A%7B%7D%7D&_="
                          + ticks;
                else
                    url = PdConstants.Https + Domain + PdConstants.ConversationsResourceGet +
                          "?source_url=%2F" + userName +
                          "%2F&data=%7B%22options%22%3A%7B%22bookmarks%22%3A%5B%22" + bookMark +
                          "%22%5D%2C%22isPrefetch%22%3Afalse%7D%2C%22context%22%3A%7B%7D%7D&_=" +
                          "%22%5D%7D%2C%22context%22%3A%7B%7D%7D&_=" + ticks;
                DelayBeforeOperation(3000);
                var response = HttpHelper.GetRequest(url);
                var objFindMessageResponseHandler = new FindMessageResponseHandler(response);

                return objFindMessageResponseHandler;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public MessageInvitationsResponseHandler GetMessageInvitations(DominatorAccountModel dominatorAccount,
           string ticks = null)
        {
            try
            {
                SetHeaders(dominatorAccount);
                var objPtRequestParameters = new PdRequestParameters();
                objPtRequestParameters.UrlParameters.Add("source_url", "//");
                objPtRequestParameters.UrlParameters.Add("data", "{\"options\":{\"isPrefetch\":false},\"context\":{}}");
                if (string.IsNullOrEmpty(ticks))
                    ticks = DateTime.Now.Ticks.ToString();
                objPtRequestParameters.UrlParameters.Add("_", ticks.ToString());
                DelayBeforeOperation(3000);
                IResponseParameter pageSource = HttpHelper.GetRequest(PdConstants.GetMessageInvitationAPI(Domain,dominatorAccount.AccountBaseModel.ProfileId));
                var objMessageInvitationsResponseHandler = new MessageInvitationsResponseHandler(pageSource);

                return objMessageInvitationsResponseHandler;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public BoardInvitationsResponseHandler GetBoardInvitations(DominatorAccountModel dominatorAccount,
            string ticks = null)
        {
            try
            {
                SetRequestHeaders(dominatorAccount);
                var url = string.Empty;
                if (string.IsNullOrEmpty(ticks))
                    ticks = DateTime.Now.Ticks.ToString();

                var objPtRequestParameters = new PdRequestParameters();
                objPtRequestParameters.UrlParameters.Add("source_url", "//");
                objPtRequestParameters.UrlParameters.Add("data", "{\"options\":{\"isPrefetch\":false},\"context\":{}}");
                objPtRequestParameters.UrlParameters.Add("_", ticks.ToString());
                url = objPtRequestParameters.GenerateUrl(PdConstants.Https + Domain + PdConstants.ContactRequestsResourceGet);
                IResponseParameter pageSource = HttpHelper.GetRequest(url);
                BoardInvitationsResponseHandler boardsResponse = new BoardInvitationsResponseHandler(pageSource, false);

                if (boardsResponse.BoardsList.Count == 0)
                {
                    objPtRequestParameters = new PdRequestParameters();
                    objPtRequestParameters.UrlParameters.Add("source_url", "//");
                    objPtRequestParameters.UrlParameters.Add("data", "{\"options\":{\"isPrefetch\":false,\"current_user\":true,\"field_set_key\":\"news\"},\"context\":{}}");
                    objPtRequestParameters.UrlParameters.Add("_", ticks.ToString());
                    url = objPtRequestParameters.GenerateUrl(PdConstants.Https + Domain + PdConstants.BoardInvitesResourceGet);
                    pageSource = HttpHelper.GetRequest(url);
                    boardsResponse = new BoardInvitationsResponseHandler(pageSource, true);
                }

                if (boardsResponse.Success == false && boardsResponse.BoardsList.Count > 0)
                    boardsResponse.Success = true;
                return boardsResponse;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public AcceptBoardInvitationResponseHandler AcceptBoardInvitation(PinterestBoard pinterestBoard,
            DominatorAccountModel dominatorAccount)
        {
            try
            {
                _objRequestParameters.PdPostElements = _objJsonFunct.GetPostDataFromAccepBoard(pinterestBoard);
                var postData = _objRequestParameters.GetPostDataFromJson();
                SetRequestHeaders(dominatorAccount);

                var postUrl = PdConstants.Https + Domain + PdConstants.BoardInviteAcceptResourceUpdate;
                DelayBeforeOperation(3000);
                var messageResponse = HttpHelper.PostRequest(postUrl, postData);

                var objAcceptBoardInvitationResponseHandler = new AcceptBoardInvitationResponseHandler(messageResponse);

                return objAcceptBoardInvitationResponseHandler;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public AcceptMessageInvitationResponseHandler AcceptMessageInvitation(PinterestUser pinterestUser,
            DominatorAccountModel dominatorAccount)
        {
            try
            {
                _objRequestParameters.PdPostElements = _objJsonFunct.GetPostDataFromAccepMessage(pinterestUser);
                //var postData = _objRequestParameters.GetPostDataFromJson();
                var postData = Encoding.UTF8.GetBytes(PdConstants.GetAcceptMessageRequestBody(pinterestUser.ContactRequestId,dominatorAccount.AccountBaseModel.ProfileId));
                SetRequestHeaders(dominatorAccount);

                var postUrl = PdConstants.GetVideoUploadParameterAPI(Domain);
                DelayBeforeOperation(3000);
                var messageResponse = HttpHelper.PostRequest(postUrl, postData);

                var objAcceptMessageInvitationResponseHandler = new AcceptMessageInvitationResponseHandler(messageResponse);

                return objAcceptMessageInvitationResponseHandler;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public SendBoardInvitationResponseHandler SendBoardInvitation(PinterestBoard pinterestBoard,
            DominatorAccountModel dominatorAccount)
        {
            try
            {
                var Collaborater_response = GetUserDetails(pinterestBoard.EmailToCollaborate, dominatorAccount).Result;
                var postData = $"{{\"queryHash\":\"42ceaa3423a096a27613e5738f83aaadcee449c55d4dd03624d2f08a6b6dca04\",\"variables\":{{\"collaboratorIds\":[\"{Collaborater_response.UserId}\"],\"boardId\":\"{pinterestBoard.Id}\",\"message\":\"\"}}}}";
                SetRequestHeaders(dominatorAccount);
                var contentType = HttpHelper.GetRequestParameter().ContentType;
                HttpHelper.GetRequestParameter().ContentType = "application/json";
                var postUrl = "https://in.pinterest.com/_/graphql/";
                DelayBeforeOperation(3000);
                var messageResponse = HttpHelper.PostRequest(postUrl,Encoding.UTF8.GetBytes(postData));
                var failedCount = 0;
                while (failedCount++ < 5 && string.IsNullOrEmpty(messageResponse.Response))
                {
                    DelayBeforeOperation(5000);
                    messageResponse = HttpHelper.PostRequest(postUrl, postData);
                }
                HttpHelper.GetRequestParameter().ContentType = contentType;
                var objSendBoardInvitationResponseHandler = new SendBoardInvitationResponseHandler(messageResponse);

                return objSendBoardInvitationResponseHandler;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public int GetBoardInvitationsCount(DominatorAccountModel dominatorAccount, string ticks = null)
        {
            try
            {
                SetRequestHeaders(dominatorAccount);
                var url = string.Empty;

                if (string.IsNullOrEmpty(ticks))
                    ticks = DateTime.Now.Ticks.ToString();

                var objPtRequestParameters = new PdRequestParameters();

                objPtRequestParameters.UrlParameters.Add("source_url", "//");
                objPtRequestParameters.UrlParameters.Add("data", "{\"options\":{\"isPrefetch\":false},\"context\":{}}");
                objPtRequestParameters.UrlParameters.Add("_", ticks);

                url = objPtRequestParameters.GenerateUrl(PdConstants.Https + Domain +
                                                         PdConstants.ContactRequestsCountGetResource);

                var pageSource = HttpHelper.GetRequest(url);
                var jsonHand = new JsonHandler(pageSource.Response);

                var invitationCount = 0;
                int.TryParse(jsonHand.GetElementValue("resource_response", "data"), out invitationCount);

                return invitationCount;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return 0;
            }
        }

        public List<string> UserConnectedWithMessage(DominatorAccountModel dominatorAccountModel)
        {
            var lstUser = new List<string>();
            try
            {
                var pdRequestParameters = new PdRequestParameters();
                var ticks = DateTime.Now.Ticks.ToString();
                pdRequestParameters.UrlParameters.Add("source_url", "//");
                pdRequestParameters.UrlParameters.Add("data", "{\"options\":{\"isPrefetch\":false,\"field_set_key\":\"default\"},\"context\":{}}");
                pdRequestParameters.UrlParameters.Add("_", ticks.ToString());

                var messageUrl = pdRequestParameters.GenerateUrl(PdConstants.Https + Domain + PdConstants.Resource +
                                                         "/ConversationsResource/get/");

                var messageResponse = HttpHelper.GetRequest(messageUrl);
                var userMessageDetailsResponseHandler = new UserConnectedWithMessageResponseHandler(messageResponse);

                lstUser.AddRange(userMessageDetailsResponseHandler.LstUserConnectedWithMessage);

                while (!string.IsNullOrEmpty(userMessageDetailsResponseHandler.Bookmark))
                {
                    string[] bookmarks = { userMessageDetailsResponseHandler.Bookmark };
                    _objRequestParameters.PdPostElements = _objJsonFunct.GetUserConnectedWithMessage(bookmarks);
                    var data = _objRequestParameters.GetJsonString();

                    ticks = DateTime.Now.Ticks.ToString();
                    var pdRequestParameter = new PdRequestParameters();
                    pdRequestParameter.UrlParameters.Add("source_url", "//");
                    pdRequestParameter.UrlParameters.Add("data", data);
                    pdRequestParameter.UrlParameters.Add("_", ticks.ToString());

                    var messageUrlForNextPage = pdRequestParameter.GenerateUrl(PdConstants.Https + Domain + PdConstants.Resource +
                                                             "/ConversationsResource/get/");

                    var messageResponse1 = HttpHelper.GetRequest(messageUrlForNextPage);
                    userMessageDetailsResponseHandler = new UserConnectedWithMessageResponseHandler(messageResponse1);

                    lstUser.AddRange(userMessageDetailsResponseHandler.LstUserConnectedWithMessage);
                }
                lstUser.RemoveAll(x => x == dominatorAccountModel.AccountBaseModel.ProfileId);
                return lstUser;
            }
            catch (Exception)
            {
                return lstUser;
            }
        }

        public async Task<SendResetPasswordLinkResponseHandler> SendResetPasswordLink(
            DominatorAccountModel dominatorAccount)
        {
            var url = string.Empty;
            var reqParam = HttpHelper.GetRequestParameter();
            reqParam.Cookies = new CookieCollection();
            reqParam.Headers = new WebHeaderCollection();
            reqParam.Accept = null;
            reqParam.ContentType = null;
            reqParam.Referer = null;
            reqParam.UserAgent = null;
            HttpHelper.SetRequestParameter(reqParam);
            SetHeaders(dominatorAccount);
            await HttpHelper.GetRequestAsync($"https://{Domain}", dominatorAccount.Token);
            url = PdConstants.Https + Domain + PdConstants.UserResetPasswordResourceCreate;
            SetRequestHeaders(dominatorAccount);
            var postData = "source_url=%2F&data=%7B%22options%22%3A%7B%22username_or_email%22%3A%22" +
                           Uri.EscapeDataString(dominatorAccount.AccountBaseModel.UserName) +
                           "%22%7D%2C%22context%22%3A%7B%7D%7D";
            DelayBeforeOperation(3000);
            var pageSource = HttpHelper.PostRequest(url, postData);

            var resetPasswordLink = new SendResetPasswordLinkResponseHandler(pageSource);

            return resetPasswordLink;
        }

        public bool ReadResetPasswordLinkFromEmail(DominatorAccountModel accountModel)
        {
            GlobusLogHelper.log.Info(Log.CustomMessage, accountModel.AccountBaseModel.AccountNetwork,
                accountModel.AccountBaseModel.UserName, "LangKeyAutoEmailVerifications".FromResourceDictionary(),
                "LangKeyFetchResetLinkFromAssociatedEmailAddress".FromResourceDictionary());
            Thread.Sleep(TimeSpan.FromMinutes(1));
            var model =
                new MailCredentials
                {
                    Hostname = accountModel.MailCredentials.Hostname,
                    Port = accountModel.MailCredentials.Port,
                    Username = accountModel.MailCredentials.Username,
                    Password = accountModel.MailCredentials.Password
                };
            IncomingData messageData = null;
            try
            {
                messageData = EmailClient.FetchLastMailFromSender(model, true, "ohno@account.pinterest.com");
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                        accountModel.AccountBaseModel.AccountNetwork,
                        accountModel.AccountBaseModel.UserName, "LangKeyAutoEmailVerifications".FromResourceDictionary(), ex.InnerException.Message);
                else
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                        accountModel.AccountBaseModel.AccountNetwork,
                        accountModel.AccountBaseModel.UserName, "LangKeyAutoEmailVerifications".FromResourceDictionary(), ex.Message);
                return false;
            }

            accountModel.ResetPasswordLink = Utilities.GetBetween(messageData.Message,
                "To view this content, open the following URL in your browser: ", "\r\n");

            if (string.IsNullOrEmpty(accountModel.ResetPasswordLink))
                accountModel.ResetPasswordLink = Utilities.GetBetween(messageData.Message,
                    "To view this content open the following URL in your browser: ", "\r\n");

            if (!string.IsNullOrEmpty(accountModel.ResetPasswordLink))
            {
                GlobusLogHelper.log.Info(Log.CustomMessage,
                    accountModel.AccountBaseModel.AccountNetwork,
                    accountModel.AccountBaseModel.UserName, "LangKeyAutoEmailVerifications".FromResourceDictionary(),
                    "LangKeyResetPasswordLinkFetched".FromResourceDictionary());
                return true;
            }

            GlobusLogHelper.log.Info(Log.CustomMessage,
                accountModel.AccountBaseModel.AccountNetwork,
                accountModel.AccountBaseModel.UserName, "LangKeyAutoEmailVerifications".FromResourceDictionary(),
                "LangKeyFailedToFetchPasswordLink".FromResourceDictionary());
            return false;
        }

        public Task<bool> ResetPasswordWithLink(DominatorAccountModel accountModel)
        {
            try
            {
                var resetPasswordLink = Uri.UnescapeDataString(accountModel.ResetPasswordLink);
                resetPasswordLink = Utilities.GetBetween(resetPasswordLink + "$", "pw/", "$");
                var userName = Utilities.GetBetween("$" + resetPasswordLink, "$", "?");
                userName = userName.Replace("/", "");
                var token = Utilities.GetBetween(resetPasswordLink, "t=", "&");
                var expiration = Utilities.GetBetween(resetPasswordLink, "e=", "&");

                var postUrl = PdConstants.Https + Domain + "/resource/ResetPasswordFromEmailResource/update/";
                SetRequestHeaders(accountModel);
                var postData = "source_url=%2Fpw%2F" + Uri.EscapeDataString(resetPasswordLink) +
                               "&data=%7B%22options%22%3A%7B%22username%22%3A%22" + userName +
                               "%22%2C%22token%22%3A%22" + token +
                               "%22%2C%22expiration%22%3A%22" + expiration +
                               "%22%2C%22new_password%22%3A%22" + accountModel.NewPassword +
                               "%22%2C%22new_password_confirm%22%3A%22" + accountModel.NewPassword +
                               "%22%7D%2C%22context%22%3A%7B%7D%7D";
                var pageSource = HttpHelper.PostRequest(postUrl, postData);

                var resetPasswordWithLinkResponse = new ResetPasswordWithLinkResponseHandler(pageSource);
                if (resetPasswordWithLinkResponse.Success)
                {
                    accountModel.AccountBaseModel.Password = accountModel.NewPassword;
                    accountModel.AccountBaseModel.Status = AccountStatus.Success;
                    SocinatorAccountBuilder.Instance(accountModel.AccountBaseModel.AccountId)
                        .AddOrUpdateDominatorAccountBase(accountModel.AccountBaseModel)
                        .AddOrUpdateCookies(accountModel.Cookies)
                        .AddOrUpdateUserAgentWeb(accountModel.UserAgentWeb)
                        .SaveToBinFile();
                    return Task.FromResult(true);
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return Task.FromResult(false);
        }

        public IRequestParameters SetHeaders(DominatorAccountModel objDominatorAccountModel)
        {
            var requestHeader = HttpHelper.GetRequestParameter();

            objDominatorAccountModel.UserAgentWeb = PdConstants.UserAgent;
            if(!string.IsNullOrEmpty(objDominatorAccountModel.AccountBaseModel.AccountProxy.ProxyIp))
               requestHeader.Proxy = objDominatorAccountModel.AccountBaseModel.AccountProxy;
            requestHeader.Headers = new WebHeaderCollection();
            requestHeader.UserAgent = PdConstants.UserAgent;
            requestHeader.ContentType = "application/x-www-form-urlencoded";
            requestHeader.Accept = "application/json, text/javascript, *; q=0.01";
            requestHeader.KeepAlive = true;
            requestHeader.Headers["Accept-Language"] = "en-US,en;q=0.9";
            requestHeader.Headers["X-Requested-With"] = "XMLHttpRequest";
            requestHeader.Headers["X-Pinterest-Source-Url"] = "/";
            requestHeader.Headers["X-Pinterest-PWS-Handler"] = "www/index.js";
            requestHeader.Headers.Add("X-Pinterest-AppState", "active");
            requestHeader.Referer = "";

            if (string.IsNullOrEmpty(Domain) && HttpHelper.GetRequestParameter().Cookies != null &&
                HttpHelper.GetRequestParameter().Cookies.Count > 0)
            {
                Domain = HttpHelper.GetRequestParameter()?.Cookies["csrftoken"].Domain;
                Domain = Domain[0].Equals('.') ? Domain.Remove(0, 1) : Domain;
            }

            //Assign browser cookies if Http mode don't have cookies
            if ((requestHeader.Cookies == null || requestHeader.Cookies.Count < 5) && objDominatorAccountModel.Cookies.Count != 0)
                requestHeader.Cookies = objDominatorAccountModel.Cookies;
            if (requestHeader.Cookies != null)
                foreach (Cookie item in requestHeader.Cookies)
                    if (item.Name == "csrftoken")
                    {
                        var csrftokenValue = item.Value;
                        requestHeader.Headers.Add("X-CSRFToken", csrftokenValue);
                        requestHeader.Headers.Add("Cookie", csrftokenValue);
                        break;
                    }
            requestHeader.Cookies = objDominatorAccountModel.Cookies;
            HttpHelper.SetRequestParameter(requestHeader);
            return requestHeader;
        }

        /// <summary>
        ///     setRequestHeaders is used to set RequestParameter for getting Response in Json
        /// </summary>
        /// <param name="objDominatorAccountModel"></param>
        /// <returns></returns>
        public IRequestParameters SetRequestHeaders(DominatorAccountModel objDominatorAccountModel)
        {
            var host = string.Empty;
            var referer = string.Empty;
            var requestHeader = HttpHelper.GetRequestParameter();
            if (HttpHelper.GetRequestParameter().Cookies != null && HttpHelper.GetRequestParameter().Cookies.Count > 0)
            {
                Domain = HttpHelper.GetRequestParameter()?.Cookies["csrftoken"].Domain;
                Domain = Domain[0].Equals('.') ? Domain.Remove(0, 1) : Domain;
                host = Domain;
                referer = "https://" + Domain + "/";
            }
            if (requestHeader.Proxy == null)
                requestHeader.Proxy = objDominatorAccountModel.AccountBaseModel.AccountProxy;

            requestHeader.Headers = new WebHeaderCollection
            {
                ["Host"] = host,
                ["Origin"] = referer,
                ["Accept-Language"] = "en-US,en;q=0.9",
                ["X-Pinterest-AppState"] = "active",
                ["X-Requested-With"] = "XMLHttpRequest",
                ["X-Pinterest-PWS-Handler"] = "www/index.js",
                ["X-Pinterest-Source-Url"] = "/"
            };
            requestHeader.UserAgent = PdConstants.UserAgent;
            requestHeader.ContentType = "application/x-www-form-urlencoded";
            requestHeader.Accept = "application/json, text/javascript, */*; q=0.01";
            requestHeader.Referer = referer;
            requestHeader.KeepAlive = true;
            if (requestHeader.Cookies != null && requestHeader.Cookies.Count > 0)
            {
                foreach (Cookie item in requestHeader.Cookies)
                {
                    if (item.Name == "csrftoken")
                    {
                        var csrftokenValue = item.Value;
                        requestHeader.Headers.Add("X-CSRFToken", csrftokenValue);
                        break;
                    }
                }
            }
            else
            {
                requestHeader.Cookies = objDominatorAccountModel.Cookies;
            }
            HttpHelper.SetRequestParameter(requestHeader);
            return requestHeader;
        }

        public IRequestParameters SetHeadersToChangeDomain(DominatorAccountModel objDominatorAccountModel)
        {
            var requestHeader = HttpHelper.GetRequestParameter();

            requestHeader.Proxy = objDominatorAccountModel.AccountBaseModel.AccountProxy;
            requestHeader.Headers = new WebHeaderCollection();
            requestHeader.UserAgent = PdConstants.UserAgent;
            requestHeader.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9";
            requestHeader.Headers["Accept-Language"] = "en-US,en;q=0.9";
            requestHeader.Headers["Sec-Fetch-User"] = "?1";
            requestHeader.Headers["Sec-Fetch-Site"] = "none";
            requestHeader.Headers["Sec-Fetch-Mode"] = "navigate";
            requestHeader.Headers["Upgrade-Insecure-Requests"] = "1";
            requestHeader.Headers["Sec-Fetch-Dest"] = "document";
            if (HttpHelper.GetRequestParameter().Cookies != null && HttpHelper.GetRequestParameter().Cookies.Count > 0)
            {
                Domain = HttpHelper.GetRequestParameter()?.Cookies["csrftoken"].Domain;
                Domain = Domain[0].Equals('.') ? Domain.Remove(0, 1) : Domain;
            }

            if (requestHeader.Cookies == null || requestHeader.Cookies.Count < 5)
                requestHeader.Cookies = objDominatorAccountModel.Cookies;

            HttpHelper.SetRequestParameter(requestHeader);
            return requestHeader;
        }

        public IRequestParameters SetHeadersForHandShake(DominatorAccountModel objDominatorAccountModel, string unauthId)
        {
            var req = SetHeaders(objDominatorAccountModel);
            req.Headers = new WebHeaderCollection();
            req.Headers.Add("X-Pinterest-InstallId", unauthId);
            req.Headers.Add("User-Agent", PdConstants.UserAgent);
            req.Headers.Add("Content-type", "application/x-www-form-urlencoded");
            req.Headers.Add("Accept", "*/*");
            req.Headers.Add("Sec-Fetch-Site", "cross-site");
            req.Headers.Add("Sec-Fetch-Mode", "cors");
            req.Headers.Add("Accept-Language", "en-US,en;q=0.9");

            HttpHelper.SetRequestParameter(req);
            return req;
        }

        public IRequestParameters SetHeadersForHandShakePost(DominatorAccountModel dominatorAccountModel, string appVersion, string experimentHash)
        {
            var reqHeader = HttpHelper.GetRequestParameter();
            reqHeader.Headers = new WebHeaderCollection();
            reqHeader.UserAgent = PdConstants.UserAgent;
            reqHeader.Accept = "application/json, text/javascript";
            reqHeader.ContentType = "application/x-www-form-urlencoded";
            reqHeader.Headers.Add("X-APP-VERSION", appVersion);
            reqHeader.Headers.Add("X-Requested-With", "XMLHttpRequest");
            reqHeader.Headers.Add("X-Pinterest-AppState", "active");
            reqHeader.Headers.Add("X-Pinterest-ExperimentHash", experimentHash);
            reqHeader.Headers["Accept-Language"] = "en-IN,en-GB;q=0.9,en-US;q=0.8,en;q=0.7";
            reqHeader.Headers["Sec-Fetch-Site"] = "same-origin";
            reqHeader.Headers["Sec-Fetch-Mode"] = "cors";

            var referer = string.Empty;
            if (HttpHelper.GetRequestParameter().Cookies != null &&
                HttpHelper.GetRequestParameter().Cookies.Count > 0)
            {
                try
                {
                    Domain = HttpHelper.GetRequestParameter()?.Cookies["_routing_id"].Domain;
                    Domain = Domain[0].Equals('.') ? Domain.Remove(0, 1) : Domain;
                    referer = "https://" + Domain + "/";
                }
                catch (Exception)
                {
                    // ignored
                }
            }
            else
                referer = $"https://{Domain}";

            reqHeader.Referer = referer;
            if (reqHeader.Cookies != null)
                foreach (Cookie item in reqHeader.Cookies)
                {
                    if (item.Name == "csrftoken")
                    {
                        var csrftokenValue = item.Value;
                        reqHeader.Headers.Add("X-CSRFToken", csrftokenValue);
                        break;
                    }
                }

            HttpHelper.SetRequestParameter(reqHeader);
            return reqHeader;
        }
        public bool SwitchToPrivate(DominatorAccountModel dominatorAccount)
        {
            var isPrivate = false;
            try
            {
                if (!CheckAccountWithBusinessOrPrivateMode())
                    return true;

                SetRequestHeaders(dominatorAccount);

                var _objRequestParameters = new PdRequestParameters();
                _objRequestParameters.PdPostElements = _objJsonFunct.GetPostDataForSwitchToPrivate();
                var postdata = _objRequestParameters.GetPostDataFromJson();
                var switcthToPrivateResp = HttpHelper.PostRequest("https://www.pinterest.com/resource/UserSessionResource/delete/", postdata);

                var switchProfileRespHandler = new SwitchProfileRespHandler(switcthToPrivateResp);

                if (switchProfileRespHandler.Success)
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Pinterest, dominatorAccount.UserName,
                        String.Format("LangKeySwitchingProfile".FromResourceDictionary()),
                        String.Format("LangKeySuccessfullySwitchedToNormalProfile".FromResourceDictionary()));

                    dominatorAccount.Cookies = HttpHelper.GetRequestParameter().Cookies;
                }
                return switchProfileRespHandler.Success;
            }
            catch (Exception)
            {
                // ignored
            }

            return isPrivate;
        }

        public bool CheckAccountWithBusinessOrPrivateMode()
        {
            var isBusiness = false;
            try
            {
                var response = HttpHelper.GetRequest(PdConstants.Https + Domain).Response;

                var checkWithBusinessProfile = HtmlAgilityHelper.GetStringInnerTextFromClassName(response, "tBJ dyH iFc MF7 B9u DrD mWe");

                if (!string.IsNullOrEmpty(response) && !string.IsNullOrEmpty(Utilities.GetBetween(response, "business_name\":\"", "\","))
                    || checkWithBusinessProfile.Equals("Analytics"))
                    isBusiness = true;
            }
            catch (Exception)
            {
                // ignored
            }

            return isBusiness;
        }

        public async Task<CreateAccountRespHandler> CreateAccount(CreateAccountInfo createAccountInfo, DominatorAccountModel accountModel)
        {
            CreateAccountRespHandler createAccountRespHandler = null;
            try
            {
                #region HTTP Method of create account.


                //SetHeaders(accountModel);

                //var defaultUrl = "https://www.pinterest.com";
                //var firstResponse = await HitFrontPage(defaultUrl, accountModel.Token);
                //var csrftoken = "";
                //if (HttpHelper.GetRequestParameter().Cookies != null && HttpHelper.GetRequestParameter().Cookies.Count > 0)
                //{
                //    Domain = HttpHelper.GetRequestParameter().Cookies["csrftoken"].Domain;
                //    Domain = Domain[0].Equals('.') ? Domain.Remove(0, 1) : Domain;
                //    csrftoken = HttpHelper.GetRequestParameter().Cookies["csrftoken"].ToString();
                //}
                ////Request Headers for create account
                //SetRequestHeaders(accountModel);
                //var appVersion = Utilities.GetBetween(firstResponse.Response, "app_version\":\"", "\"");
                //var experimentHash = Utilities.GetBetween(firstResponse.Response, "triggerable_experiments_hash\":\"", "\"");
                //var browserlocale = Utilities.GetBetween(firstResponse.Response, "browser_locale\":\"", "\"");
                //var browsecountry = Utilities.GetBetween(firstResponse.Response, "country\":\"", "\"");
                //if(string.IsNullOrEmpty(browsecountry))
                //    browsecountry= Utilities.GetBetween(firstResponse.Response, "country_from_ip\":\"", "\"");
                //var req = HttpHelper.GetRequestParameter();
                //req.Headers["X-APP-VERSION"] = appVersion;
                //req.Headers["Sec-Fetch-Site"] = "same-origin";
                //req.Headers["Sec-Fetch-Mode"] = "cors";
                //req.Headers["Sec-Fetch-Dest"] = "empty";
                //req.Headers["X-CSRFToken"] = csrftoken.Replace("csrftoken=", "");
                ////req.Headers["X-Pinterest-ClientState"] = "920de9f8bc4b10c66aba7aaaf8844aea";
                //req.Headers["X-Requested-With"] = "XMLHttpRequest";
                //req.Headers["sec-ch-ua-mobile"] = "?0";
                //HttpHelper.SetRequestParameter(req);

                //var ErrorMessage = CheckAccountVAlidationCreads(createAccountInfo);
                //if (!string.IsNullOrEmpty(ErrorMessage) && ErrorMessage.Equals("LangKeyEnterValidEmail"))
                //{
                //    return new CreateAccountRespHandler(new ResponseParameter { Response = string.Empty })
                //    { Success = false, Issue = new PinterestIssue { Message = ErrorMessage.FromResourceDictionary() } };
                //}
                //if (!string.IsNullOrEmpty(ErrorMessage) && ErrorMessage.Equals("LangKeyYourPasswordIstooShort"))
                //{
                //    return new CreateAccountRespHandler(new ResponseParameter { Response = string.Empty })
                //    { Success = false, Issue = new PinterestIssue { Message = ErrorMessage.FromResourceDictionary() } };
                //}
                //if (!string.IsNullOrEmpty(ErrorMessage) && ErrorMessage.Equals("LangKeyPasswordMustbeStronger"))
                //{
                //    return new CreateAccountRespHandler(new ResponseParameter { Response = string.Empty })
                //    { Success = false, Issue = new PinterestIssue { Message = ErrorMessage.FromResourceDictionary() } };
                //}

                #region OLD Create Account Logic.
                //string[] usernamearraystring = createAccountInfo.Email.Split('@');
                //var postdata = PdConstants.GetaccountcreaterPostData(createAccountInfo, browsecountry, usernamearraystring[0].ToString());//"{\"options\":{\"container\":\"home_page\",\"email\":\"" + "nitishpatil@gmail.com" + "\",\"password\":\"" + createAccountInfo.Password + "\",\"age\":\"" + createAccountInfo.Age + "\",\"country\":\"" + browsecountry + "\",\"signupSource\":\"homePage\",\"first_name\":\"" + usernamearraystring[0].ToString() + "\",\"last_name\":\"\",\"hybridTier\":\"open\",\"page\":\"home\",\"no_fetch_context_on_resource\":false},\"context\":{}}";
                //var data = Uri.EscapeDataString(postdata);
                //var finalPostData = "source_url=%2F&data=" + data;
                //string accountCreatePosturl = PdConstants.Https + Domain + PdConstants.Resource + PdConstants.UserRegisterCrete;
                //DelayBeforeOperation(3000);
                //var accres = await HttpHelper.PostRequestAsync(accountCreatePosturl, finalPostData, accountModel.Token);
                //createAccountRespHandler = new CreateAccountRespHandler(new ResponseParameter { Response = accres.Response });

                //if (!createAccountRespHandler.Success)
                //    return createAccountRespHandler;

                ////Nextbutton post data
                //req.Headers["X-Pinterest-ExperimentHash"] = experimentHash;
                //var objParametersfornextbutton = new PdRequestParameters
                //{ PdPostElements = _objJsonFunct.GetPostDataFromJsonForNextButton("NUX_WELCOME_NAME_STEP_STATUS", "2") };
                //string nextbuttonposturl = PdConstants.Https + Domain + PdConstants.Resource + PdConstants.UserStateResourcecreate;//"https://www.pinterest.com/resource/UserStateResource/create/ ";
                //var nextbuttonresponse = await PostHitAccountCreate(nextbuttonposturl, objParametersfornextbutton.GetPostDataFromJson(), accountModel.Token);

                //createAccountRespHandler = new CreateAccountRespHandler(new ResponseParameter { Response = nextbuttonresponse });
                //if (!createAccountRespHandler.Success)
                //    return createAccountRespHandler;
                //Thread.Sleep(TimeSpan.FromSeconds(2));

                ////Gender selection
                //var objParameters1 = new PdRequestParameters
                //{ PdPostElements = _objJsonFunct.GetPostDataFromJsonGenderSelection(createAccountInfo) };
                //var genderselectposturl = PdConstants.Https + Domain + PdConstants.Resource + PdConstants.UserSettingResourceupdate; //"https://www.pinterest.com/resource/UserSettingsResource/update/";
                //await PostHitAccountCreate(genderselectposturl, objParameters1.GetPostDataFromJson(), accountModel.Token);
                //objParametersfornextbutton = new PdRequestParameters
                //{ PdPostElements = _objJsonFunct.GetPostDataFromJsonForNextButton("NUX_GENDER_STEP_STATUS", "1") };
                //nextbuttonresponse = await PostHitAccountCreate(nextbuttonposturl, objParametersfornextbutton.GetPostDataFromJson(), accountModel.Token);

                //createAccountRespHandler = new CreateAccountRespHandler(new ResponseParameter { Response = nextbuttonresponse });
                //if (!createAccountRespHandler.Success)
                //    return createAccountRespHandler;
                //Thread.Sleep(TimeSpan.FromSeconds(2));

                ////Country selection               
                //objParameters1 = new PdRequestParameters
                //{ PdPostElements = _objJsonFunct.GetPostDataFromJsonStateSelection(browserlocale, browsecountry) };
                //var selectcountryurl = PdConstants.Https + Domain + PdConstants.Resource + PdConstants.UserSettingResourceupdate;
                //var countryresponse = await PostHitAccountCreate(selectcountryurl, objParameters1.GetPostDataFromJson(), accountModel.Token);

                //createAccountRespHandler = new CreateAccountRespHandler(new ResponseParameter { Response = countryresponse });
                //if (!createAccountRespHandler.Success)
                //    return createAccountRespHandler;

                //objParametersfornextbutton = new PdRequestParameters
                //{ PdPostElements = _objJsonFunct.GetPostDataFromJsonForNextButton("NUX_COUNTRY_LOCALE_STEP_STATUS", "2") };
                //await PostHitAccountCreate(nextbuttonposturl, objParametersfornextbutton.GetPostDataFromJson(), accountModel.Token);
                //Thread.Sleep(TimeSpan.FromSeconds(2));

                ////5 topic selection
                //string interestid = "935249274030,900013937947,961238559656,905860166503,905661505034";
                //var selectedtopicObjParameters = new PdRequestParameters
                //{ PdPostElements = _objJsonFunct.GetDataFromJsonFiveChoiceSelection(createAccountInfo, interestid) };
                //string selectedtopicUrl = PdConstants.Https + Domain + PdConstants.Resource + PdConstants.OrientationContaxtResource;// "https://www.pinterest.com/resource/OrientationContextResource/create/";
                //await PostHitAccountCreate(selectedtopicUrl, selectedtopicObjParameters.GetPostDataFromJson(), accountModel.Token);

                //objParametersfornextbutton = new PdRequestParameters
                //{ PdPostElements = _objJsonFunct.GetPostDataFromJsonForNextButton("NUX_TOPIC_PICKER_STEP_STATUS", "2") };
                //await PostHitAccountCreate(nextbuttonposturl, objParametersfornextbutton.GetPostDataFromJson(), accountModel.Token);

                //objParametersfornextbutton = new PdRequestParameters
                //{ PdPostElements = _objJsonFunct.GetPostDataFromJsonForNextButton("NUX_LOADING_STEP_STATUS", "1") };
                //await PostHitAccountCreate(nextbuttonposturl, objParametersfornextbutton.GetPostDataFromJson(), accountModel.Token);

                //createAccountRespHandler = new CreateAccountRespHandler(new ResponseParameter { Response = countryresponse });
                //if (!createAccountRespHandler.Success)
                //    return createAccountRespHandler;
                #endregion

                #region Updated Create Account Logic.

                //var CheckExist = await HttpHelper.GetRequestAsync(PdConstants.CheckExistAPI(Domain, createAccountInfo.Email), accountModel.Token);
                //createAccountRespHandler = new CreateAccountRespHandler(CheckExist, SignUpActivityType.CheckExist);
                //if(createAccountRespHandler != null && createAccountRespHandler.Success)
                //{
                //    var checkValid = await HttpHelper.GetRequestAsync(PdConstants.CheckValidAPI(Domain, createAccountInfo.Email), accountModel.Token);
                //    createAccountRespHandler = new CreateAccountRespHandler(checkValid, SignUpActivityType.CheckEmailValid);
                //    if(createAccountRespHandler != null && createAccountRespHandler.Success)
                //    {
                //        SetUpHeaderForCreateAccount(accountModel, SignUpActivityType.Register, createAccountRespHandler.Token);
                //        int.TryParse(createAccountInfo.Age, out int userage);
                //        var register = await HttpHelper.PostRequestAsync(PdConstants.SignUpAPI, PdConstants.SignUpBody(createAccountInfo.Email, createAccountInfo.Password, userage), accountModel.Token);
                //        createAccountRespHandler = new CreateAccountRespHandler(register, SignUpActivityType.Register);
                //        if (createAccountRespHandler != null && createAccountRespHandler.Success)
                //        {
                //            SetUpHeaderForCreateAccount(accountModel, SignUpActivityType.SignUpHandShake, string.Empty, appVersion, csrftoken);
                //            var signUp = await HttpHelper.PostRequestAsync(PdConstants.HandShakeAPI(Domain), PdConstants.HandShakeBody(createAccountRespHandler.Token), accountModel.Token);
                //            createAccountRespHandler = new CreateAccountRespHandler(signUp, SignUpActivityType.SignUpHandShake);
                //            if (createAccountRespHandler != null && createAccountRespHandler.Success)
                //            {
                //                SetUpHeaderForCreateAccount(accountModel, SignUpActivityType.Register, createAccountRespHandler.Token);
                //                var Login = await HttpHelper.PostRequestAsync(PdConstants.LoginAPI, PdConstants.LoginBody(createAccountInfo.Email, createAccountInfo.Password), accountModel.Token);
                //                createAccountRespHandler = new CreateAccountRespHandler(signUp, SignUpActivityType.Login);
                //                if (createAccountRespHandler != null && createAccountRespHandler.Success)
                //                {
                //                    SetUpHeaderForCreateAccount(accountModel, SignUpActivityType.SignUpHandShake, string.Empty, appVersion, csrftoken);
                //                    var loginHandShake = await HttpHelper.PostRequestAsync(PdConstants.HandShakeAPI(Domain), PdConstants.HandShakeBody(createAccountRespHandler.Token), accountModel.Token);
                //                    createAccountRespHandler = new CreateAccountRespHandler(loginHandShake, SignUpActivityType.LoginHandShake);
                //                    if (createAccountRespHandler != null && createAccountRespHandler.Success)
                //                    {
                //                        //Setup Account After Login.
                //                        SetUpHeaderForCreateAccount(accountModel, SignUpActivityType.SignUpHandShake, string.Empty, appVersion, csrftoken);
                //                        DelayBeforeOperation(2000);
                //                        var firstName = PdConstants.GetFirstname(createAccountInfo.Email);
                //                        await PostHitAccountCreate($"https://{Domain}/resource/UserSettingsResource/update/", Encoding.UTF8.GetBytes($"source_url=%2F&data=%7B%22options%22%3A%7B%22full_name%22%3A%22{firstName}%22%2C%22first_name%22%3A%22{firstName}%22%2C%22surface_tag%22%3A%22nux%22%7D%2C%22context%22%3A%7B%7D%7D"), accountModel.Token);
                //                        DelayBeforeOperation(2000);
                //                        await PostHitAccountCreate(PdConstants.AccountSetupAPI(Domain), PdConstants.AccountSetupBody("NUX_WELCOME_NAME_STEP_STATUS", 2), accountModel.Token);
                //                        DelayBeforeOperation(2000);
                //                        var gender = createAccountInfo?.Gender;
                //                        await PostHitAccountCreate($"https://{Domain}/resource/UserSettingsResource/update/", Encoding.UTF8.GetBytes($"source_url=%2F&data=%7B%22options%22%3A%7B%22gender%22%3A%22{gender}%22%2C%22surface_tag%22%3A%22nux%22%7D%2C%22context%22%3A%7B%7D%7D"), accountModel.Token);
                //                        DelayBeforeOperation(2000);
                //                        await PostHitAccountCreate(PdConstants.AccountSetupAPI(Domain), PdConstants.AccountSetupBody("NUX_GENDER_STEP_STATUS", createAccountInfo.Male ? 2 : 3), accountModel.Token);
                //                        DelayBeforeOperation(2000);
                //                        await PostHitAccountCreate($"https://{Domain}/resource/UserSettingsResource/update/", Encoding.UTF8.GetBytes($"source_url=%2F&data=%7B%22options%22%3A%7B%22country%22%3A%22{browsecountry}%22%2C%22locale%22%3A%22{browserlocale}%22%2C%22surface_tag%22%3A%22nux%22%7D%2C%22context%22%3A%7B%7D%7D"), accountModel.Token);
                //                        DelayBeforeOperation(2000);
                //                        await PostHitAccountCreate(PdConstants.AccountSetupAPI(Domain), PdConstants.AccountSetupBody("NUX_COUNTRY_LOCALE_STEP_STATUS", 2), accountModel.Token);
                //                        DelayBeforeOperation(2000);
                //                        var interestPicker = await HttpHelper.GetRequestAsync($"https://www.pinterest.co.uk/resource/NuxInterestsResource/get/?source_url=%2F&data=%7B%22options%22%3A%7B%22is_renux%22%3Afalse%2C%22logNetworkTimer%22%3Atrue%2C%22loggerComponentName%22%3A%22nux_picker%22%2C%22extraImageSignal%22%3A%5B%5D%2C%22urlParams%22%3A%7B%7D%7D%2C%22context%22%3A%7B%7D%7D&_={PdConstants.GetTicks}", accountModel.Token);
                //                        var interestPickerResponse = new InterestPickerResponse(interestPicker);
                //                        await PostHitAccountCreate(PdConstants.AccountSetupAPI(Domain), PdConstants.AccountSetupBody("NUX_TOPIC_PICKER_STEP_STATUS", 1), accountModel.Token);
                //                        DelayBeforeOperation(2000);
                //                        if (interestPickerResponse != null && interestPickerResponse.Success)
                //                        {
                //                            SetUpHeaderForCreateAccount(accountModel, SignUpActivityType.InterestUpdate, string.Empty, appVersion, csrftoken);
                //                            DelayBeforeOperation(2000);
                //                            var interestResponse = await PostHitAccountCreate($"https://{Domain}/_/graphql/", Encoding.UTF8.GetBytes(GetInterestBody(interestPickerResponse.InterestCollection, gender)), accountModel.Token);
                //                            DelayBeforeOperation(2000);
                //                        }
                //                    }
                //                }
                //            }
                //        }
                //    }
                //}
                #endregion

                #endregion

                var userAgent = $"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/{RandomUtilties.GetRandomNumber(115,107)}.0.0.0 Safari/537.36";
                #region Puppeteer Browser Method of create account.
                Domain = accountModel.Cookies["csrftoken"].Domain;
                Domain = Domain[0].Equals('.') ? Domain.Remove(0, 1) : Domain;
                #region Code for Run Through Puppeteer
                var HeadLess = true;
#if DEBUG
                HeadLess = false;
#endif
                browserActivity = new PuppeteerBrowserActivity(accountModel, isNeedResourceData: true, targetUrl: $"https://{Domain}/");
                var BrowserLaunched = await browserActivity.LaunchBrowserAsync(HeadLess);
                #endregion
                var pageSource = await browserActivity.GetPageSourceAsync();
                await Task.Delay(TimeSpan.FromSeconds(6));
                int.TryParse(createAccountInfo.Age, out int age);
                var time = DateTime.Now.AddYears(-age);
                var Date = time.Day > 9 ? $"{time.Day}" : $"0{time.Day}";
                var birthdate = $"{Date}{time.Month}{time.Year}";
                if (string.IsNullOrEmpty(pageSource) || !pageSource.Contains("aria-label=\"Your profile\""))
                    await PuppeteerLogin(accountModel, Domain,false);
                await browserActivity.GotoCustomUrl(accountModel, accountModel.Token, $"https://{Domain}/create-personal", delayInSec: 6,userAgent);
                browserActivity.ClearResources();
                var XandY = await browserActivity.GetXAndYAsync(customScriptX: "document.querySelector('input[id=\"email\"]').getBoundingClientRect().x;",customScriptY: "document.querySelector('input[id=\"email\"]').getBoundingClientRect().y;");
                await browserActivity.MouseClickAsync(XandY.Key + 10, XandY.Value + 5, delayAfter: 3);
                await browserActivity.EnterCharsAsync(createAccountInfo.Email, delayAtLast: 2);
                XandY = await browserActivity.GetXAndYAsync(customScriptX: "document.querySelector('input[id=\"password\"]').getBoundingClientRect().x;", customScriptY: "document.querySelector('input[id=\"password\"]').getBoundingClientRect().y;");
                await browserActivity.MouseClickAsync(XandY.Key + 10, XandY.Value + 5, delayAfter: 3);
                pageSource = await browserActivity.GetPaginationData("/v3/email/validation/", true);
                createAccountRespHandler = new CreateAccountRespHandler(new ResponseParameter { Response= pageSource },SignUpActivityType.CheckEmailValid);
                if(createAccountRespHandler != null && createAccountRespHandler.Success || string.IsNullOrEmpty(pageSource))
                {
                    pageSource = await browserActivity.GetPaginationData("/v3/register/exists/", true);
                    createAccountRespHandler = new CreateAccountRespHandler(new ResponseParameter { Response = pageSource }, SignUpActivityType.CheckExist);
                    if (createAccountRespHandler != null && createAccountRespHandler.Success || string.IsNullOrEmpty(pageSource))
                    {
                        await browserActivity.EnterCharsAsync(createAccountInfo.Password, delayAtLast: 2);
                        XandY = await browserActivity.GetXAndYAsync(customScriptX: "document.querySelector('input[id=\"birthdate\"]').getBoundingClientRect().x;", customScriptY: "document.querySelector('input[id=\"birthdate\"]').getBoundingClientRect().y;");
                        await browserActivity.MouseClickAsync(XandY.Key + 10, XandY.Value + 5, delayAfter: 3);
                        await browserActivity.EnterCharsAsync(birthdate, delayAtLast: 2);
                        XandY = await browserActivity.GetXAndYAsync(customScriptX: "document.querySelector('button[type=\"submit\"]').getBoundingClientRect().x;", customScriptY: "document.querySelector('button[type=\"submit\"]').getBoundingClientRect().y;");
                        await browserActivity.MouseClickAsync(XandY.Key + 10, XandY.Value + 5, delayAfter: 8);
                        pageSource = await browserActivity.GetPaginationData("not eligible to sign up for Pinterest", true);
                        createAccountRespHandler = new CreateAccountRespHandler(new ResponseParameter { Response = pageSource });
                        if(createAccountRespHandler != null && createAccountRespHandler.Success || string.IsNullOrEmpty(pageSource))
                        {
                            pageSource = await browserActivity.GetPageSourceAsync();
                            if (pageSource.Contains("Make sure this age is accurate") ||pageSource.Contains("Nice to meet you! What's your name?"))
                                await browserActivity.ExecuteScriptAsync("document.querySelector('button[type=\"submit\" i]').click();", delayInSec: 4);
                            await browserActivity.ExecuteScriptAsync("document.querySelector('button[aria-label=\"Next\" i]').click();", delayInSec: 4);
                            var aria_label = createAccountInfo.Gender == "male" ? "nux-gender-male-label" : createAccountInfo.Gender == "female" ? "nux-gender-female-label" : "nux-gender-unspecified-label";
                            XandY = await browserActivity.GetXAndYAsync(customScriptX: $"document.querySelector('div[data-test-id=\"{aria_label}\" i]').getBoundingClientRect().x;", customScriptY: $"document.querySelector('div[data-test-id=\"{aria_label}\" i]').getBoundingClientRect().y;");
                            if (XandY.Key == 0 && XandY.Value == 0)
                                XandY = await browserActivity.GetXAndYAsync(customScriptX: $"document.querySelector('button[data-test-id=\"{aria_label}\" i]').getBoundingClientRect().x;", customScriptY: $"document.querySelector('button[data-test-id=\"{aria_label}\" i]').getBoundingClientRect().y;");
                            await browserActivity.MouseClickAsync(XandY.Key + 10, XandY.Value + 5, delayAfter: 3);
                            await browserActivity.ExecuteScriptAsync($"document.querySelector('div[data-test-id=\"nux-locale-country-next-btn\" i]').click();", delayInSec: 4);
                            pageSource = await browserActivity.GetPageSourceAsync();
                            var Nodes = HtmlParseUtility.GetListNodeFromPartialTagNamecontains(pageSource, "div", "data-test-id", "nux-picker-topic");
                            var counter = 0;
                            var cloned = Nodes.ToList();
                            var script = "document.querySelectorAll('div[data-test-id=\"nux-picker-topic\" i]')[{0}].{1};";
                            while (counter++ < Nodes.Count)
                            {
                                var randomnux = cloned.GetRandomItem();
                                var clickIndex = cloned.IndexOf(randomnux);
                                await browserActivity.ExecuteScriptAsync(string.Format(script,clickIndex, "scrollIntoViewIfNeeded()"), delayInSec: 3);
                                XandY = await browserActivity.GetXAndYAsync(customScriptX: string.Format(script, clickIndex, "getBoundingClientRect().x"), customScriptY: string.Format(script, clickIndex, "getBoundingClientRect().y"));
                                if(XandY.Key == 0 && XandY.Value == 0)
                                {
                                    counter--;
                                    continue;
                                }
                                await browserActivity.MouseClickAsync(XandY.Key + 10, XandY.Value + 10, delayAfter: 2);
                                pageSource = await browserActivity.GetPageSourceAsync();
                                if (!string.IsNullOrEmpty(pageSource) && pageSource.Contains("Meet your home feed"))
                                    break;
                                cloned.Remove(randomnux);
                                await Task.Delay(TimeSpan.FromSeconds(3));
                            }
                            browserActivity.ClearResources();
                            await Task.Delay(TimeSpan.FromSeconds(5));
                            await browserActivity.ExecuteScriptAsync($"document.querySelector('button[type=\"submit\" i]').click();", delayInSec:12);
                            pageSource = await browserActivity.GetPaginationData("v3_complete_experience", true);
                            //Switch to original Account after account creation.
                            {
                                await browserActivity.ExecuteScriptAsync($"document.querySelector('div[data-test-id=\"header-accounts-options-button\"] button').click();", delayInSec: 5);
                                var username = accountModel.AccountBaseModel.UserName?.Split('@')?.ToList()?.FirstOrDefault();
                                username = Regex.Match(username, "[a-zA-Z]+").Value;
                                script = "[...document.querySelectorAll('div[role=\"menuitem\"]')].filter(x=>x.innerText.trim().toLowerCase().includes(\"{0}\")||x.innerText.trim().toLowerCase().includes(\"{1}\"))[0].childNodes[0].{2};";
                                XandY = await browserActivity.GetXAndYAsync(customScriptX: string.Format(script, accountModel.AccountBaseModel.UserFullName,username, "getBoundingClientRect().x"), customScriptY: string.Format(script, accountModel.AccountBaseModel.UserFullName, username, "getBoundingClientRect().y"));
                                await browserActivity.MouseClickAsync(XandY.Key+10,XandY.Value + 10,delayAfter:7);
                            }
                            createAccountRespHandler = new CreateAccountRespHandler(new ResponseParameter { Response = pageSource });
                            pageSource = string.IsNullOrEmpty(pageSource) ? await browserActivity.GetPageSourceAsync() : pageSource;
                            if (!string.IsNullOrEmpty(pageSource) || pageSource.Contains(PdConstants.CheckLoginStatusString))
                            {
                                await browserActivity.SaveCookies();
                                accountModel = browserActivity.DominatorAccountModel;
                                sessionManager.AddOrUpdateSession(ref accountModel, true);
                            }
                        }
                    }
                }
                #endregion
            }
            catch (Exception)
            {
                // ignored
            }
            finally {
                try
                {
                    browserActivity.ClosedBrowser();
                }
                catch { }
            }
            return createAccountRespHandler;
        }

        private string GetInterestBody(List<InterestData> interestCollection,string gender)
        {
            var finalResponse = "{\"queryHash\":\"8e86b68b0558fb3376be38e5b3131be2f7a6ac47870c9c060e2b72774f9afa3c\",\"variables\":{\"interests\":{0},\"logData\":\"{1}\",\"redoHomefeed\":false,\"userBehaviorData\":\"{\\\"signupInterestsPickerScrollDown\\\":true,\\\"signupInterestsPickerTimeSpent\\\":9}\"}}";
            var interests = new List<string>();
            var logData = new List<string>();
            while(interestCollection!= null && interestCollection.Count > 5 && interests.Count < 5)
            {
                var random = interestCollection.GetRandomItem();
                if (!interests.Any(x => x == random.InterestID))
                {
                    interests.Add(random.InterestID);
                    logData.Add($"\\\"{random.LogData?.Replace("\"", "\\\\\\\"").Replace("unspecified", gender).Replace("static", "ranked_by_engagement_manas")}\\\"");
                }
            }
            try
            {
                var inter = JsonConvert.SerializeObject(interests);
                var djson = $"[{string.Join(",",logData)}]";
                finalResponse = finalResponse.Replace("{0}", inter).Replace("{1}",djson);
                return finalResponse;
            }
            catch (Exception) { }
            return finalResponse;
        }

        private void SetUpHeaderForCreateAccount(DominatorAccountModel accountModel,SignUpActivityType signUpActivity,string token="",string appVersion="",string csrf="", IRequestParameters oldRequestParameter=null)
        {
            if(oldRequestParameter != null)
            {
                HttpHelper.SetRequestParameter(oldRequestParameter);
                return;
            }
            else
            {
                var reqParam = HttpHelper.GetRequestParameter();
                reqParam.Headers.Clear();
                reqParam.Headers["Origin"] = $"https://{Domain}";
                reqParam.Headers["sec-ch-ua"] = "\"(Not(A:Brand\";v=\"99\", \"Google Chrome\";v=\"126\", \"Chromium\";v=\"126\"";
                reqParam.Headers["sec-ch-ua-platform"] = signUpActivity != SignUpActivityType.InterestUpdate ? "\"macOS\"": "\"Linux\"";
                reqParam.Referer = $"https://{Domain}/";
                reqParam.Headers["Accept-Encoding"] = "gzip, deflate";
                reqParam.Headers["Accept-Language"] = "en-GB,en-US;q=0.9,en;q=0.8";
                reqParam.UserAgent = signUpActivity != SignUpActivityType.InterestUpdate ? "Mozilla/5.0 (Macintosh; Intel Mac OS X 11_10) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/126.0.6305.215 Safari/537.36":
                    "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/125.0.6282.208 Safari/537.36";
                reqParam.Headers["sec-ch-ua-mobile"] = "?0";
                reqParam.Headers["Sec-Fetch-Mode"] = "cors";
                if (signUpActivity == SignUpActivityType.Register)
                {
                    reqParam.Headers["X-Pinterest-InstallId"] = token;
                    reqParam.Headers["Sec-Fetch-Site"] = "cross-site";
                    reqParam.Headers["Sec-Fetch-Dest"] = "empty";
                    reqParam.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
                    reqParam.Accept = "*/*";
                }else if(signUpActivity == SignUpActivityType.SignUpHandShake)
                {
                    reqParam.Headers["X-Pinterest-AppState"] = "background";
                    reqParam.Headers["X-APP-VERSION"] = appVersion;
                    reqParam.Headers["X-Pinterest-PWS-Handler"] = "www/index.js";
                    reqParam.Headers["X-Requested-With"] = "XMLHttpRequest";
                    reqParam.Headers["X-Pinterest-Source-Url"] = "/";
                    reqParam.Headers["X-CSRFToken"] = csrf;
                    reqParam.Headers["Sec-Fetch-Site"] = "same-origin";
                    reqParam.Headers["Sec-Fetch-Dest"] = "empty";
                    reqParam.Headers["sec-ch-ua-model"] = "\"\"";
                    reqParam.Headers["sec-ch-ua-full-version-list"] = "\"(Not(A:Brand\";v=\"99.0.0.0\", \"Google Chrome\";v=\"126\", \"Chromium\";v=\"126\"";
                    reqParam.ContentType = "application/x-www-form-urlencoded";
                    reqParam.Accept = "application/json, text/javascript, */*, q=0.01";
                }else if(signUpActivity == SignUpActivityType.InterestUpdate)
                {
                    reqParam.Headers["X-Pinterest-AppState"] = "active";
                    reqParam.Headers["X-Pinterest-PWS-Handler"] = "www/index.js";
                    reqParam.Headers["X-Pinterest-GraphQL-Name"] = "InterestGridMutation";
                    reqParam.Headers["X-Requested-With"] = "XMLHttpRequest";
                    reqParam.Headers["X-Pinterest-Source-Url"] = "/";
                    reqParam.Headers["X-CSRFToken"] = csrf;
                    reqParam.Headers["Sec-Fetch-Site"] = "same-origin";
                    reqParam.Headers["Sec-Fetch-Dest"] = "empty";
                    reqParam.Headers["sec-ch-ua-model"] = "\"\"";
                    reqParam.Headers["sec-ch-ua-full-version-list"] = "\"(Not(A:Brand\";v=\"99.0.0.0\", \"Google Chrome\";v=\"126\", \"Chromium\";v=\"126\"";
                    reqParam.ContentType = "application/json";
                }
                HttpHelper.SetRequestParameter(reqParam);
            }
        }

        /// <summary>
        /// check for validate account creds if user enter invalid details
        /// </summary>
        /// <param name="createAccountInfo"></param>
        /// <returns></returns>
        private string CheckAccountVAlidationCreads(CreateAccountInfo createAccountInfo)
        {
            string errorMessage = string.Empty;
            string usernameinput = createAccountInfo.Email;
            if (!usernameinput.Contains("@"))
                errorMessage = "LangKeyEnterValidEmail";
            if (createAccountInfo.Password.Length <= 5)
                errorMessage = "LangKeyYourPasswordIstooShort";
            if (createAccountInfo.Password.Equals("123456789"))
                errorMessage = "LangKeyPasswordMustbeStronger";
            return errorMessage;
        }

        async Task<IResponseParameter> HitFrontPage(string defaultUrl, CancellationToken cancellationToken)
        {
            IResponseParameter firstreponce;
            int delayTime = 0;
            do
            {
                DelayBeforeOperation(3000);
                firstreponce = await HttpHelper.GetRequestAsync(defaultUrl, cancellationToken);
                if (delayTime++ > 5)
                    break;
            }
            while (string.IsNullOrEmpty(firstreponce?.Response) || HttpHelper.GetRequestParameter()?.Cookies?.Count == 0);

            return firstreponce;
        }

        async Task<string> PostHitAccountCreate(string defaultUrl, byte[] postData, CancellationToken cancellationToken)
        {
            string responce = null;
            try
            {
                var delayTime = 1;
                do
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    DelayBeforeOperation(3000);
                    responce = (await HttpHelper.PostRequestAsync(defaultUrl, postData, cancellationToken)).Response;
                    if (delayTime++ > 5)
                        break;
                }
                while (string.IsNullOrEmpty(responce));

            }
            catch (Exception)
            {
                // ignored
            }

            return responce;
        }

        string ProxyFailedReason(IResponseParameter response)
        {
            if (response.Exception != null &&
                (response.Exception.Message.Contains("The remote name could not be resolved:") ||
                 response.Exception.Message == "Unable to connect to the remote server"))
                return "LangKeyUnableToConnectToTheRemoteServer".FromResourceDictionary();

            if (response.Response.Contains("Authentication failure, your IP is not authorized to access this proxy."))
                return "LangKeyAuthenticationFailureIPIsNotAuthorized".FromResourceDictionary();

            if (response.Response.Contains("Please ensure this IP is set in your Authorized IP list"))
                return "LangKeyEnsureThisIPIsSetInAuthorizedIPList".FromResourceDictionary();

            if (response.Response.Contains("Proxy Authentication Required"))
                return "LangKeyProxyAuthenticationRequired".FromResourceDictionary();

            if (response.Response.Contains("Access control configuration prevents your request from being allowed at this time. Please contact your service provider if you feel this is incorrect."))
                return "LangKeyAccessControlConfigurationPreventsRequestAllowedAtThisTime".FromResourceDictionary();

            if (response.Response.Contains("The server encountered an internal error or\nmisconfiguration and was unable to complete\nyour request"))
                return "LangKeyServerEncounteredAnInternalError".FromResourceDictionary();

            if (response.Response.Contains("ERR_FORWARDING_DENIED"))
                return "LangKeyERRFORWARDINGDENIED".FromResourceDictionary();

            if (response.Response.Contains("ERR_ACCESS_DENIED"))
                return "LangKeyERRACCESSDENIED".FromResourceDictionary();

            return "";
        }



        #region Organize(Not Useful for Now)

        public void MovePins(List<string> pinIds, string boardUrlToMove, DominatorAccountModel dominatorAccount)
        {
            SetHeaders(dominatorAccount);
            var pinUrl = $"https://{Domain}/pin/{pinIds[0]}";
            var pinPageSource = HttpHelper.GetRequest(pinUrl).Response;
            var boardName = Utilities.GetBetween(pinPageSource, "\"], \"name\": \"", "\"");
            var boardToMovePageSource = HttpHelper.GetRequest(boardUrlToMove).Response;
            var boardIdToMove = Utilities.GetBetween(boardToMovePageSource,
                PdConstants.UserName + boardUrlToMove.Split('/')[3] +
                "\"", "\"type\": \"board\"");
            boardIdToMove = Utilities.GetBetween(boardIdToMove, "}, \"id\": \"", "\"");

            var pinIdsPost = string.Empty;
            for (var i = 0; i < pinIds.Count - 1; i++) pinIdsPost += pinIds[i] + "%22%2C%22";
            pinIdsPost += pinIds[pinIds.Count - 1];
            var urlToMovePin = $"https://{Domain}/resource/BulkEditResource/update/";
            var postData = $"https://{Domain}/resource/" + dominatorAccount.AccountBaseModel.ProfileId + "%2F" + boardName +
                           "%2F&data=%7B%22options%22%3A%7B%22board_id%22%3A%22" + boardIdToMove +
                           "%22%2C%22pin_ids%22%3A%5B%22" + pinIdsPost + "%22%5D%7D%2C%22context%22%3A%7B%7D%7D";
            SetRequestHeaders(dominatorAccount);
            HttpHelper.PostRequest(urlToMovePin, postData);
        }

        public void CopyPins(List<string> pinIds, string boardUrlToCopy, DominatorAccountModel dominatorAccount)
        {
            SetHeaders(dominatorAccount);
            var pinUrl = $"https://{Domain}/pin/pinIds[0]";
            var pinPageSource = HttpHelper.GetRequest(pinUrl).Response;
            var boardName = Utilities.GetBetween(pinPageSource, "\"], \"name\": \"", "\"");
            var boardToCopyPageSource = HttpHelper.GetRequest(boardUrlToCopy).Response;
            var boardIdToCopy = Utilities.GetBetween(boardToCopyPageSource,
                PdConstants.UserName + boardUrlToCopy.Split('/')[3] +
                "\"", "\"type\": \"board\"");
            boardIdToCopy = Utilities.GetBetween(boardIdToCopy, "}, \"id\": \"", "\"");

            var pinIdsPost = string.Empty;
            for (var i = 0; i < pinIds.Count - 1; i++) pinIdsPost += pinIds[i] + "%22%2C%22";
            pinIdsPost += pinIds[pinIds.Count - 1];
            var urlToCopyPin = $"https://{Domain}/resource/BulkEditResource/create/";
            var postData = PdConstants.PDsourceUrl + dominatorAccount.AccountBaseModel.ProfileId + "%2F" + boardName +
                           "%2F&data=%7B%22options%22%3A%7B%22board_id%22%3A%22" + boardIdToCopy +
                           "%22%2C%22pin_ids%22%3A%5B%22"
                           + pinIdsPost + "%22%5D%7D%2C%22context%22%3A%7B%7D%7D";
            SetRequestHeaders(dominatorAccount);
            HttpHelper.PostRequest(urlToCopyPin, postData);
        }

        public void DeletePins(List<string> pinIds, DominatorAccountModel dominatorAccount)
        {
            SetHeaders(dominatorAccount);
            var pinUrl = $"https://{Domain}/pin/pinIds[0]";
            var pinPageSource = HttpHelper.GetRequest(pinUrl).Response;
            var boardName = Utilities.GetBetween(pinPageSource, "\"], \"name\": \"", "\"");

            var pinIdsPost = string.Empty;
            for (var i = 0; i < pinIds.Count - 1; i++) pinIdsPost += pinIds[i] + "%22%2C%22";
            pinIdsPost += pinIds[pinIds.Count - 1];
            var urlToDeletePin = $"https://{Domain}/resource/BulkEditResource/delete/";
            var postData = PdConstants.PDsourceUrl + dominatorAccount.AccountBaseModel.ProfileId + "%2F" + boardName +
                           "%2F&data=%7B%22options%22%3A%7B%22pin_ids%22%3A%5B%22" + pinIdsPost +
                           "%22%5D%7D%2C%22context%22%3A%7B%7D%7D";
            SetRequestHeaders(dominatorAccount);
            HttpHelper.PostRequest(urlToDeletePin, postData);
        }
        #endregion
        #region GetUserFollowingCount.
        public async Task<int> GetUserFollowingCountAsync(string ProfileId,CancellationToken cancellationToken)
        {
            var followingCount = 0;
            try
            {
                var followingResponse = await HttpHelper.GetRequestAsync(PdConstants.GetUserFollowingAPI(Domain, ProfileId), cancellationToken);
                if (!string.IsNullOrEmpty(followingResponse.Response) && followingResponse.Response.Contains("\"status\":\"success\""))
                {
                    var jsonhandler = new JsonHandler(followingResponse.Response);
                    var bookmark = jsonHandler.GetJTokenValue(jsonhandler.GetJToken("resource", "options", "bookmarks"), 0);
                    followingCount = jsonHandler.GetJArrayElement(jsonhandler.GetJToken("resource_response", "data").ToString()).Count;
                    while (!string.IsNullOrEmpty(bookmark) && bookmark != "-end-")
                    {
                        followingResponse = await HttpHelper.GetRequestAsync(PdConstants.GetUserFollowingAPI(Domain, ProfileId,$",\"bookmarks\":[\"{bookmark}\"]"), cancellationToken);
                        jsonhandler = new JsonHandler(followingResponse.Response);
                        followingCount += jsonHandler.GetJArrayElement(jsonhandler.GetJToken("resource_response", "data").ToString()).Count;
                        bookmark = jsonHandler.GetJTokenValue(jsonhandler.GetJToken("resource", "options", "bookmarks"), 0);
                    }
                }
            }
            catch (Exception)
            {
                return followingCount;
            }
            return followingCount;
        }

        public void GetPinComments(ref List<PinterestPin> PinList,DominatorAccountModel dominatorAccount)
        {
            GlobusLogHelper.log.Info(Log.CustomMessage,dominatorAccount.AccountBaseModel.AccountNetwork,dominatorAccount.UserName,"Comment Details", "Please Wait While Getting Pin Comment Count");
            PinList.ForEach(pin =>
            {
                var PinId = string.Empty;
                try
                {
                    PinId = string.IsNullOrEmpty(pin.Id) ? pin.PinId : pin.Id;
                    var PinDetails = GetPinDetails(PinId, dominatorAccount,true);
                    pin.CommentCount = PinDetails.CommentCount;
                    pin.Description = PinDetails.Description;
                }
                catch (Exception ex)
                {
                    ex.DebugLog(ex.Message);
                }
                DelayBeforeOperation(950);
            });
            GlobusLogHelper.log.Info(Log.CustomMessage, dominatorAccount.AccountBaseModel.AccountNetwork, dominatorAccount.UserName, "Comment Details", "Successfully Got The Comment Count");
        }
        public void SetCsrfToken(ref IRequestParameters RequestHeader, ref DominatorAccountModel DominatorAccountModel)
        {
            foreach (Cookie cookie in DominatorAccountModel.Cookies)
                if (cookie.Name == "csrftoken")
                {
                    RequestHeader.Headers["X-CSRFTOKEN"] = cookie.Value;
                    break;
                }
        }

        public async Task<int> GetUserPinCount(string ProfileId, DominatorAccountModel dominatorAccountModel)
        {
            var PinCount = 0;
            try
            {
                var PinResponse = await HttpHelper.GetRequestAsync(PdConstants.GetUserPinDetailsAPI(ProfileId,Domain), dominatorAccountModel.Token);
                if (!string.IsNullOrEmpty(PinResponse.Response) && PinResponse.Response.Contains("\"status\":\"success\""))
                {
                    var jsonhandler = new JsonHandler(PinResponse.Response);
                    var bookmark = jsonHandler.GetJTokenValue(jsonhandler.GetJToken("resource", "options", "bookmarks"), 0);
                    PinCount = jsonHandler.GetJArrayElement(jsonhandler.GetJToken("resource_response", "data").ToString()).Count(x=>!string.IsNullOrEmpty(x.Path) &&!x.ToString().Contains("\"name\": \"AddPinRep\""));
                    while (!string.IsNullOrEmpty(bookmark) && bookmark != "-end-")
                    {
                        PinResponse = await HttpHelper.GetRequestAsync(PdConstants.GetUserPinDetailsAPI(ProfileId,Domain, bookmark),dominatorAccountModel.Token);
                        jsonhandler = new JsonHandler(PinResponse.Response);
                        PinCount += jsonHandler.GetJArrayElement(jsonhandler.GetJToken("resource_response", "data").ToString()).Count(x => !string.IsNullOrEmpty(x.Path) && !x.ToString().Contains("\"name\": \"AddPinRep\""));
                        bookmark = jsonHandler.GetJTokenValue(jsonhandler.GetJToken("resource", "options", "bookmarks"), 0);
                    }
                }
                return PinCount;
            }
            catch (Exception)
            {
                return PinCount;
            }
        }

        public async Task<int> GetUserFollowerCount(string ProfileId, DominatorAccountModel dominatorAccountModel)
        {
            var FollowerCount = 0;
            try
            {
                var FollowerResponse = await GetUserFollowersAsync(ProfileId, dominatorAccountModel.Token, dominatorAccountModel, null, DateTime.Now.Ticks.ToString());
                FollowerCount = FollowerResponse.UsersList.Count;
                while (FollowerResponse.HasMoreResults)
                {
                    FollowerResponse = await GetUserFollowersAsync(ProfileId, dominatorAccountModel.Token, dominatorAccountModel, FollowerResponse.BookMark, DateTime.Now.Ticks.ToString());
                    FollowerCount += FollowerResponse.UsersList.Count;
                }
                return FollowerCount;
            }
            catch (Exception)
            {
                return FollowerCount;
            }
        }
        #endregion
        public void DelayBeforeOperation(int delay = 4000) => _delayService.ThreadSleep(delay);

        public Task UpdatePinterestBoardSections(DominatorAccountModel dominatorAccount,ref List<PinterestBoard> Boards)
        {
            GlobusLogHelper.log.Info(Log.CustomMessage, dominatorAccount.AccountBaseModel.AccountNetwork, dominatorAccount.UserName, "Board Section Info", "Please Wait While Updating Board Sections..");
            var jsonHandler = JsonJArrayHandler.GetInstance;
            Boards.ForEach(board =>
            {
                var sections = new List<PinterestBoardSections>();
                var sectionResponse = HttpHelper.GetRequest(PdConstants.GetBoardSectionAPI(dominatorAccount.AccountBaseModel.ProfileId, board.BoardName, board.Id, Domain)).Response;
                var jsonObject = jsonHandler.ParseJsonToJObject(sectionResponse);
                var Sections = jsonHandler.GetJArrayElement(jsonHandler.GetJTokenValue(jsonObject, "resource_response", "data"));
                if (Sections.HasValues)
                    Sections.ForEach(x =>
                    {
                        sections.Add(new PinterestBoardSections { SectionTitle = jsonHandler.GetJTokenValue(x, "title"), SectionID = jsonHandler.GetJTokenValue(x, "id"),BoardName=board.BoardName,BoardDescription=board.BoardDescription,BoardId=board.Id,BoardUrl=board.BoardUrl });
                    });
                board.BoardSections.AddRange(sections);
                DelayBeforeOperation(700);
            });
            return null;
        }

        public BoardResponse CreateBoardSection(BoardResponse BoardResponse, BoardInfo info, DominatorAccountModel dominatorAccount)
        {
            try
            {
                if (info.SectionList.Count > 0 && BoardResponse.Success)
                {
                    var boardInfo = GetBoardDetails(BoardResponse.Url, dominatorAccount);
                    var Sections = new List<PinterestBoardSections>();
                    info.SectionList.ForEach(section =>
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage, dominatorAccount.AccountBaseModel.AccountNetwork, dominatorAccount.AccountBaseModel.UserName, "Create Section", $"Trying To Create Section {{{section}}}");
                        var createSectionAPI = $"{PdConstants.Https}{Domain}{PdConstants.Resource}{PdConstants.CreateBoardSectionResource}";
                        var CreateSectionResponse = HttpHelper.PostRequest(createSectionAPI, PdConstants.GetSectionCreatePostData(dominatorAccount.AccountBaseModel.ProfileId, boardInfo.BoardName, boardInfo.BoardId, section));
                        var SectionResponse = new SectionResponseHandler(CreateSectionResponse);
                        if (SectionResponse.Success)
                            GlobusLogHelper.log.Info(Log.CustomMessage, dominatorAccount.AccountBaseModel.AccountNetwork, dominatorAccount.AccountBaseModel.UserName, "Create Section", $"Successful to create section {{{section}}}");
                        else
                            GlobusLogHelper.log.Info(Log.CustomMessage, dominatorAccount.AccountBaseModel.AccountNetwork, dominatorAccount.AccountBaseModel.UserName, "Create Section", $"Failed to create Section with error==>{SectionResponse?.Issue?.Message}");
                        var delay = RandomUtilties.GetRandomNumber(30,20);
                        Sections.Add(new PinterestBoardSections { SectionTitle = SectionResponse.SectionTitle, SectionID = SectionResponse.SectionId, BoardId = SectionResponse.BoardId, BoardDescription = SectionResponse.BoardDescription, BoardName = SectionResponse.BoardName, BoardUrl = SectionResponse.BoardUrl });
                        if (section != info.SectionList.LastOrDefault())
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage, dominatorAccount.AccountBaseModel.AccountNetwork, dominatorAccount.AccountBaseModel.UserName, "Create Section", $"Next Section will Create in {delay} seconds");
                            DelayBeforeOperation(delay*1000);
                        }
                    });
                    BoardResponse.BoardSections = Sections;
                }
                return BoardResponse;
            }
            catch(Exception ex)
            {
                ex.DebugLog(ex.GetBaseException().Message);
                return BoardResponse;
            }
        }

        public IResponseParameter TryPost(string Url, string postData, byte[] BytesPostData, string ContainString = "", int TryCount = 2,bool IsLocked=false)
        {
            IResponseParameter Response = new ResponseParameter();
            try
            {
                var postBody = BytesPostData.Length > 0 ? BytesPostData : Encoding.UTF8.GetBytes(postData);
                TryAgain:
                if(IsLocked)
                    Response = HttpHelper.LockedPostRequest(Url, postBody);
                else
                    Response = HttpHelper.PostRequest(Url, postBody);
                while (TryCount-- >= 0 && (string.IsNullOrEmpty(ContainString)?string.IsNullOrEmpty(Response.Response)||Response.Response.Contains("<DOCTYPE html>") :string.IsNullOrEmpty(Response.Response) || !Response.Response.Contains(ContainString)))
                {
                    DelayBeforeOperation(2 * 1000);
                    goto TryAgain;
                }
            }
            catch(Exception ex) { ex.DebugLog(); }
            return Response;
        }

        public async Task<IResponseParameter> PuppeteerLogin(DominatorAccountModel dominatorAccount,string domain,bool IsCloseBrowser=true)
        {
            var pageResponse = string.Empty;
            try
            {
                Domain = string.IsNullOrEmpty(domain) ? dominatorAccount.Cookies["csrftoken"].Domain:domain;
                Domain = Domain[0].Equals('.') ? Domain.Remove(0, 1) : Domain;
                browserActivity = new PuppeteerBrowserActivity(dominatorAccount,isNeedResourceData:true,targetUrl: $"https://{domain}/login/");
                var IsHeadLess = true;
#if DEBUG
                IsHeadLess = false;
#endif
                await browserActivity.LaunchBrowserAsync(IsHeadLess);
                var XandY = await browserActivity.GetXAndYAsync(customScriptX: "document.querySelector('input[id=\"email\"]').getBoundingClientRect().x;", customScriptY: "document.querySelector('input[id=\"email\"]').getBoundingClientRect().y;");
                await browserActivity.MouseClickAsync(XandY.Key+10,XandY.Value+5,delayAfter:4);
                await browserActivity.EnterCharsAsync(dominatorAccount.AccountBaseModel.UserName,delayAtLast:3);
                XandY = await browserActivity.GetXAndYAsync(customScriptX: "document.querySelector('input[id=\"password\"]').getBoundingClientRect().x;", customScriptY: "document.querySelector('input[id=\"password\"]').getBoundingClientRect().y;");
                await browserActivity.MouseClickAsync(XandY.Key + 10, XandY.Value + 5, delayAfter: 4);
                await browserActivity.EnterCharsAsync(dominatorAccount.AccountBaseModel.Password, delayAtLast: 3);
                browserActivity.ClearResources();
                await browserActivity.ExecuteScriptAsync("document.querySelector('div[data-test-id=\"registerFormSubmitButton\"]').childNodes[0].click();", delayInSec: 12);
                pageResponse = await browserActivity.GetPaginationData("v3_get_user_settings", true);
                if (string.IsNullOrEmpty(pageResponse))
                    pageResponse = await browserActivity.GetPaginationData("v3_home_feed", true);
                if (!string.IsNullOrEmpty(pageResponse))
                {
                    await browserActivity.SaveCookies(false);
                    dominatorAccount = browserActivity.DominatorAccountModel;
                }
            }
            catch(Exception ex) { ex.DebugLog();}
            finally
            {
                if (browserActivity != null && IsCloseBrowser)
                    browserActivity.ClosedBrowser();
            }
            return new ResponseParameter() { Response = pageResponse }; ;
        }
    }
}