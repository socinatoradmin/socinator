using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Request;
using DominatorHouseCore.Utility;
using EmbeddedBrowser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using TumblrDominatorCore.Interface;
using TumblrDominatorCore.Models;
using TumblrDominatorCore.TmblrUtility;
using TumblrDominatorCore.TumblrResponseHandler;
using ActType = DominatorHouseCore.PuppeteerBrowser.ActType;
using AttributeType = DominatorHouseCore.PuppeteerBrowser.AttributeType;
using ValueTypes = DominatorHouseCore.PuppeteerBrowser.ValueTypes;

namespace TumblrDominatorCore.TumblrLibrary.TumblrFunction.TumblrBrowserManager
{
    public interface ITumblrBrowserManager : IBrowserManager
    {
        //     PuppeteerBrowserActivity BrowserWindow { get; set; }
        BrowserWindow BrowserWindow { get; set; }
        CancellationToken CancellationToken { get; set; }
        string CurrentData { get; set; }
        string LasAutomationLoginResponse { get; set; }
        SearchPostsResonseHandler GetTumblrPostsFromKeyWord(DominatorAccountModel dominatorAccountModel, string keyWord, SearchFilterModel searchFilter, bool IsHasTag = false);
        SearchUsersForKeywordRespone GetTumblrUsersFromKeyWord(DominatorAccountModel dominatorAccountModel, string keyWord, SearchFilterModel searchFilter, bool IsHasTag = false);
        string SearchPostDetails(DominatorAccountModel dominatorAccountModel, ref TumblrPost tumblrPost);
        string SearchUserDetails(DominatorAccountModel dominatorAccountModel, ref TumblrUser tumblrUser);
        List<TumblrPost> GetUserPostsDetails(DominatorAccountModel dominatorAccountModel, TumblrUser tumblrUser);


        SearchforFollowingsorFollowersResponse GetFollowingsOrFollowers(string pageUrl, int? pageCount = null);
        SearchforFollowingsorFollowersResponse GetSomeOnesFollowings(string pageUrl, int? pageCount = null);
        (List<TumblrComments>, List<TumblrComments>, List<TumblrComments>) GetFeedDetails(DominatorAccountModel account, TumblrPost tumblrPost, bool GetComment = false, bool GetReblog = false, bool GetLikes = false);
        IResponseParameter LoginResponse(DominatorAccountModel account);
        string GetCurrentPageResponse(DominatorAccountModel account);
        FollowResponseHandler Follow(DominatorAccountModel DominatorAccountModel, TumblrUser tumblrUser);
        UnFollowResponseHandler UnFollow(DominatorAccountModel DominatorAccountModel, TumblrUser tumblrUser);
        bool Comment(DominatorAccountModel account, string message, ref TumblrPost tumblrPost);
        LikePostResponse LikePost(DominatorAccountModel account, TumblrPost tumblrPost);
        UnLikePostResponse UnLikePost(DominatorAccountModel account, TumblrPost tumblrPost);
        ReblogPostResponse ReblogPost(DominatorAccountModel DominatorAccountModel, TumblrPost tumblrPost);
        bool BroadCastMessage(DominatorAccountModel DominatorAccountModel, string message, string media, ref TumblrUser tumblrUser);
        PublishResponseHandler PublishPost(DominatorAccountModel dominatorAccountModel, PublisherPostlistModel updatedPostModel);
        void CloseBrowser(DominatorAccountModel account);
        Task WaitToLoadPageResponse(DominatorAccountModel dominatorAccount, string ConditionalString, bool CheckContains = false, int RepeatCheckCount = 15, int DefaultDelayInSec = 6, int CheckDelayInSec = 5);
    }

    public class TumblrBrowserManager : ITumblrBrowserManager
    {
        public BrowserWindow BrowserWindow { get; set; }
        //    public PuppeteerBrowserActivity BrowserWindow { get; set; }
        public CancellationToken CancellationToken { get; set; }
        public List<TumblrUser> FollowUserlist { get; set; }
        private string _html { get; set; }
        private string _Pagetext { get; set; }
        public string CurrentData { get; set; }
        private IResponseParameter Response => new ResponseParameter { Response = CurrentData };
        private readonly ITumblrAccountSession tumblrAccountSession;
        public TumblrBrowserManager(ITumblrAccountSession accountSession)
        {
            tumblrAccountSession = accountSession;
        }
        public string LasAutomationLoginResponse { get; set; }

        public bool BrowserLogin(DominatorAccountModel account, CancellationToken cancellationToken, LoginType loginType = LoginType.AutomationLogin, VerificationType verificationType = 0)
        {
            try
            {
                tumblrAccountSession.AddOrUpdateSession(ref account);
                Application.Current.Dispatcher.Invoke(async () =>
                    {
                        try
                        {
                            #region Code for Run Through Puppeteer
                            //                            var HeadLess = true;
                            //#if DEBUG
                            //                            HeadLess = false;
                            //#endif
                            //                            BrowserWindow = new PuppeteerBrowserActivity(account, isNeedResourceData: true);
                            //                            var BrowserLaunched = await BrowserWindow.LaunchBrowserAsync(HeadLess);
                            //                            BrowserWindow.CloseUnWantedTabs(BrowserWindow.TargetUrl,account.CancellationSource);
                            #endregion
                            #region Code for Run Through CefBrowser
                            BrowserWindow = new BrowserWindow(account, isNeedResourceData: true)
                            {
                                Visibility = Visibility.Hidden
                            };
                            if (loginType == LoginType.BrowserLogin)
                                BrowserWindow.Visibility = Visibility.Visible;
                            else
                            {
                                BrowserWindow.Visibility = Visibility.Hidden;
                            }
#if DEBUG
                            BrowserWindow.Visibility = Visibility.Visible;
#endif
                            // Always make it Non async to avoid browserwindow visibility issue oe else it will open browser window
                            if ((account.Cookies?.Count == 0 || account.CookieHelperList.Any(x => x.Name.Contains("logged_in") && x.Value == "0")) && account.BrowserCookies?.Count > 0)
                                BrowserWindow.BrowserSetCookie();
                            else
                                BrowserWindow.SetCookie();

                            BrowserWindow.Show();
                            #endregion
                            await TumblrBrowserLogin(account, loginType);
                        }
                        catch (Exception)
                        {
                        }
                    });
                var last3Min = DateTime.Now.AddMinutes(2);
                while (account.AccountBaseModel.Status != AccountStatus.ProfileSuspended
                    && !BrowserWindow.IsLoggedIn && last3Min >= DateTime.Now)
                {
                    account.Token.ThrowIfCancellationRequested();
                    if (last3Min.AddMinutes(3.5) < DateTime.Now && account.IsUserLoggedIn)
                    {
                        if (BrowserWindow.DominatorAccountModel != null)
                            // ReSharper disable once RedundantAssignment
                            account = BrowserWindow.DominatorAccountModel;
                        break;
                    }
                    Thread.Sleep(2000);
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            finally
            {
                LasAutomationLoginResponse = GetCurrentPageResponse(account);
                if (loginType != LoginType.AutomationLogin && loginType != LoginType.BrowserLogin && BrowserWindow != null && !BrowserWindow.IsDisposed)
                {
                    if (!Application.Current.Dispatcher.CheckAccess())
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            BrowserWindow.Close();
                        });
                    }
                    else
                        BrowserWindow.Close();
                }

            }
            return account.IsUserLoggedIn;

        }
        public string GetCurrentPageResponse(DominatorAccountModel account)
        {
            bool isRunning = true;
            string response = string.Empty;
            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    response = await BrowserWindow.GetPageSourceAsync();
                    isRunning = false;
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                    isRunning = false;
                    return;
                }
            });
            while (isRunning)
            {
                TimeSpan.FromSeconds(2);
            }
            return response;
        }
        public IResponseParameter LoginResponse(DominatorAccountModel account)
        {
            bool isRunning = true;
            var response = new ResponseParameter();
            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    response.Response = await BrowserWindow.GetPageSourceAsync();
                    var res = await BrowserWindow.GetPaginationData(ConstantHelpDetails.LoginPageKeyword, true);
                    var item = await BrowserWindow.GetPaginationDataList(ConstantHelpDetails.LoginPageKeyword, true);
                    while (string.IsNullOrEmpty(res))
                    {
                        res = await BrowserWindow.GetPaginationData(ConstantHelpDetails.LoginPageKeyword, true);
                        if (item.Count == 1)
                        {
                            res = item?.FirstOrDefault();
                        }
                    }
                    response.Response = res;
                    isRunning = false;
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                    isRunning = false;
                    return;
                }
            });
            while (isRunning)
            {
                TimeSpan.FromSeconds(2);
            }
            return response;
        }
        public void CloseBrowser(DominatorAccountModel account)
        {
            try
            {

                if (BrowserWindow != null)
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        BrowserWindow.Close();
                        BrowserWindow.Dispose();
                        BrowserWindow = null;
                        GC.Collect();
                    });
            }
            catch (Exception)
            {

            }

            Thread.Sleep(1000);
        }

        public (List<TumblrComments>, List<TumblrComments>, List<TumblrComments>) GetFeedDetails(DominatorAccountModel account, TumblrPost tumblrPost, bool GetComment = false, bool GetReblog = false, bool GetLikes = false)
        {
            bool isRunning = true;
            var commentList = new List<TumblrComments>();
            var likeList = new List<TumblrComments>();
            var reblogList = new List<TumblrComments>();
            LoadPageSource(account, tumblrPost.PostUrl, clearandNeedResource: true);
            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    _Pagetext = await BrowserWindow.PageText();
                    var noteText = await BrowserWindow.GetElementValueAsync(ActType.GetValue, AttributeType.ClassName, "ePsyd", ValueTypes.InnerText, clickIndex: 0);
                    if (string.IsNullOrEmpty(noteText))
                        noteText = await BrowserWindow.GetElementValueAsync(ActType.GetValue, AttributeType.ClassName, "xu5ZG", ValueTypes.InnerText, clickIndex: 0);

                    if (!string.IsNullOrEmpty(noteText) && !noteText.Contains("Close notes") && !_Pagetext.Contains("Close notes"))
                    {
                        await BrowserWindow.BrowserActAsync(ActType.ScrollIntoView, AttributeType.ClassName, "xu5ZG", index: 0);
                        var noteScript = ConstantHelpDetails.GetScriptforTwoAttributesAndValue(ActType.Click, AttributeType.ClassName, "xu5ZG",
                        AttributeType.Title, $"{noteText}", clickIndex: 0);
                        await BrowserWindow.ExecuteScriptAsync(noteScript, 3);
                    }
                    if (GetComment)
                    {
                        var ReplyList = await ScrapeReplyofPost(account, tumblrPost);
                        commentList.AddRange(ReplyList);
                    }
                    if (GetReblog)
                    {
                        var ReblogList = await ScrapeReblogofPost(account, tumblrPost);
                        reblogList.AddRange(ReblogList);
                    }
                    if (GetLikes)
                    {
                        var LikeList = await ScrapeLikeofPost(account, tumblrPost);
                        likeList.AddRange(LikeList);
                    }
                    isRunning = false;
                }
                catch (OperationCanceledException)
                {
                    isRunning = false;
                    throw new OperationCanceledException();
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                    isRunning = false;
                    return;
                }

            });
            while (isRunning)
            {
                TimeSpan.FromSeconds(2);
            }
            return (commentList, reblogList, likeList);
        }
        private async Task<List<TumblrComments>> ScrapeReplyofPost(DominatorAccountModel accountModel, TumblrPost tumblrPost)
        {
            List<TumblrComments> replyList = new List<TumblrComments>();
            try
            {
                var pageSource = await BrowserWindow.GetPageSourceAsync();
                var ReplyResponseList = await BrowserWindow.GetPaginationDataList("\"response\":{\"timeline\":{\"elements\":[{\"objectType\":\"post_note\"", true);
                replyList = new CommentScraperResponse(new ResponseParameter() { Response = pageSource }, ReplyResponseList).post.ListComments;
            }
            catch (Exception)
            {
                return replyList;
            }
            return replyList;
        }
        private async Task<List<TumblrComments>> ScrapeLikeofPost(DominatorAccountModel accountModel, TumblrPost tumblrPost)
        {
            BrowserWindow.ClearResources();
            var clicklikeClassName = HtmlParseUtility.GetAttributeValueFromTagName(CurrentData, "button", "title", "Likes", "class");
            if (!string.IsNullOrEmpty(clicklikeClassName))
            {
                await BrowserWindow.BrowserActAsync(ActType.Click, AttributeType.ClassName, clicklikeClassName, delayAfter: 2);
            }
            CurrentData = await BrowserWindow.GetPageSourceAsync();
            List<TumblrComments> likeList = new List<TumblrComments>();
            try
            {
                var ReplyResponseList = await BrowserWindow.GetPaginationDataList("\"response\":{\"notes\":[{\"type\":\"like\"", true);
                likeList = new CommentScraperResponse(new ResponseParameter() { Response = CurrentData }, ReplyResponseList).post.ListComments;
            }
            catch (Exception)
            {
                return likeList;
            }
            return likeList;

        }
        private async Task<List<TumblrComments>> ScrapeReblogofPost(DominatorAccountModel accountModel, TumblrPost tumblrPost)
        {
            BrowserWindow.ClearResources();
            var clickReblogClassName = HtmlParseUtility.GetAttributeValueFromTagName(CurrentData, "button", "title", "Reblogs", "class");
            if (!string.IsNullOrEmpty(clickReblogClassName))
            {
                await BrowserWindow.BrowserActAsync(ActType.Click, AttributeType.ClassName, clickReblogClassName, delayAfter: 3);
            }
            CurrentData = await BrowserWindow.GetPageSourceAsync();
            List<TumblrComments> reblogList = new List<TumblrComments>();
            try
            {
                var ReplyResponseList = await BrowserWindow.GetPaginationDataList("\"response\":{\"timeline\":{\"elements\":[{\"objectType\":\"post_note\"", true);
                reblogList = new CommentScraperResponse(new ResponseParameter() { Response = CurrentData }, ReplyResponseList).post.ListComments;
            }
            catch (Exception)
            {
                return reblogList;
            }

            return reblogList;
        }
        public SearchforFollowingsorFollowersResponse GetFollowingsOrFollowers(string pageUrl, int? pageCount = null)
        {
            var userList = new List<TumblrUser>();
            bool isRunning = true;
            int elementLength = 0;
            int elementindex = 0;
            SearchforFollowingsorFollowersResponse followingresponse = new SearchforFollowingsorFollowersResponse();

            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    BrowserWindow.ClearResources();
                    CurrentData = await BrowserWindow.GoToCustomUrl(pageUrl, 5);
                    followingresponse = new SearchforFollowingsorFollowersResponse(Response);
                    if (followingresponse.Success && followingresponse.TotalFollowings == 0) return;
                    while (elementLength < followingresponse.TotalFollowings)
                    {
                        var elementlengthresp = await BrowserWindow.GetElementValueAsync(ActType.GetLength, AttributeType.ClassName, "Ut4iZ eXQ6G veU9u");
                        if (!string.IsNullOrEmpty(elementlengthresp)) elementLength = Convert.ToInt32(elementlengthresp);
                        while (elementindex < elementLength)
                        {
                            var elementurl = await BrowserWindow.GetElementValueAsync(ActType.GetValue, AttributeType.ClassName, "Ro4PU BSUG4", ValueTypes.Href, clickIndex: elementindex);
                            var userName = Utilities.GetBetween(elementurl, "https://", ".tumblr.com/");
                            if (userName.Contains("www")) userName = Utilities.GetBetween(elementurl + "/", "https://www.tumblr.com/", "/");
                            if (!string.IsNullOrEmpty(userName))
                                userList.Add(
                                    new TumblrUser()
                                    {
                                        Username = userName,
                                        PageUrl = $"https://www.tumblr.com/{userName}"
                                    }
                                    );
                            elementindex++;
                        }
                        if (elementLength == followingresponse.TotalFollowings) break;
                        await BrowserWindow.BrowserActAsync(ActType.ScrollWindow, AttributeType.ClassName, "", scrollByPixel: 1000, delayAfter: 5);
                    }

                    isRunning = false;
                }
                catch (OperationCanceledException)
                {
                    isRunning = false;
                    throw new OperationCanceledException();
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                    isRunning = false;
                    return;
                }
            });
            while (isRunning)
            {
                TimeSpan.FromSeconds(2);
            }
            followingresponse.LstTumblrUser = userList;
            return followingresponse;
        }
        public string SearchPostDetails(DominatorAccountModel dominatorAccountModel, ref TumblrPost tumblrPost)
        {
            var url = tumblrPost.PostUrl;
            bool isRunning = true;
            var status = "";
            SearchPostsResonseHandler postDetails = new SearchPostsResonseHandler();
            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    BrowserWindow.ClearResources();
                    await BrowserWindow.GoToCustomUrl(url, 5);
                    _Pagetext = await BrowserWindow.PageText();
                    if (_Pagetext.Contains("This post isn't here anymore") || _Pagetext.Contains("This post is no more") || _Pagetext.Contains("This post has ceased to exist"))
                    {
                        status = "The Post is Not Available AnyMore";
                        isRunning = false;
                        return;
                    }
                    CurrentData = await BrowserWindow.GetPageSourceAsync();
                    if (CurrentData != null)
                    {
                        postDetails = new SearchPostsResonseHandler(Response);
                        status = "true";
                        isRunning = false;
                    }
                    else
                    {
                        status = "false";
                        isRunning = false;
                    }

                }
                catch (OperationCanceledException)
                {
                    isRunning = false;
                    status = "false";
                    throw new OperationCanceledException();
                }
                catch (Exception ex)
                {
                    status = "false";
                    ex.DebugLog();
                    isRunning = false;
                    return;
                }
            });
            while (isRunning)
            {
                TimeSpan.FromSeconds(2);
            }
            if (postDetails.Success) tumblrPost = postDetails.LstTumblrPosts?.FirstOrDefault();
            return status;
        }
        public FollowResponseHandler Follow(DominatorAccountModel DominatorAccountModel, TumblrUser tumblrUser)
        {
            bool isRunning = true;
            var followResponseList = new List<string>();
            var blogUrl = string.IsNullOrEmpty(tumblrUser.PageUrl) && !string.IsNullOrEmpty(tumblrUser.UserId) ?
                ConstantHelpDetails.BlogViewUrl(tumblrUser.UserId) :
                string.IsNullOrEmpty(tumblrUser.PageUrl) && !string.IsNullOrEmpty(tumblrUser.Username) ? ConstantHelpDetails.BlogViewUrl(tumblrUser.Username)
                : tumblrUser.PageUrl;
            LoadPageSource(DominatorAccountModel, blogUrl, true);
            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    BrowserWindow.ClearResources();
                    await BrowserWindow.BrowserActAsync(ActType.ActByQuery, AttributeType.AriaLabel, "Follow", delayAfter: 3);
                    followResponseList = await BrowserWindow.GetPaginationDataList("\"meta\":{\"status\":200,\"msg\":\"OK\"", true, "\"followed\":true");
                    isRunning = false;
                }
                catch (OperationCanceledException)
                {
                    isRunning = false;
                    throw new OperationCanceledException();
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                    isRunning = false;
                    return;
                }

            });
            while (isRunning)
            {
                TimeSpan.FromSeconds(2);
            }
            return new FollowResponseHandler(new ResponseParameter() { Response = followResponseList.FirstOrDefault() });
        }

        public UnFollowResponseHandler UnFollow(DominatorAccountModel DominatorAccountModel, TumblrUser tumblrUser)
        {

            bool isRunning = true;
            var unfollowResponseList = new List<string>();
            var blogUrl = string.IsNullOrEmpty(tumblrUser.PageUrl) && !string.IsNullOrEmpty(tumblrUser.UserId) ?
                ConstantHelpDetails.BlogViewUrl(tumblrUser.UserId) :
                string.IsNullOrEmpty(tumblrUser.PageUrl) && !string.IsNullOrEmpty(tumblrUser.Username) ? ConstantHelpDetails.BlogViewUrl(tumblrUser.Username)
                : tumblrUser.PageUrl;
            LoadPageSource(DominatorAccountModel, blogUrl, true);
            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    BrowserWindow.ClearResources();
                    await BrowserWindow.BrowserActAsync(ActType.ActByQuery, AttributeType.AriaLabel, "Unfollow", delayAfter: 3);
                    unfollowResponseList = await BrowserWindow.GetPaginationDataList("\"meta\":{\"status\":200,\"msg\":\"OK\"", true, "\"followed\":false");
                    isRunning = false;

                }
                catch (OperationCanceledException)
                {
                    isRunning = false;
                    throw new OperationCanceledException();
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                    isRunning = false;
                    return;
                }

            });
            while (isRunning)
            {
                TimeSpan.FromSeconds(2);
            }
            return new UnFollowResponseHandler(new ResponseParameter() { Response = unfollowResponseList.FirstOrDefault() });
        }
        public LikePostResponse LikePost(DominatorAccountModel account, TumblrPost tumblrPost)
        {
            bool isRunning = true;
            var likeResponseList = new List<string>();
            var url = string.IsNullOrEmpty(tumblrPost.PostUrl) && !string.IsNullOrEmpty(tumblrPost.OwnerUsername) && !string.IsNullOrEmpty(tumblrPost.Id) ?
                ConstantHelpDetails.GetPostUrlByUserNameAndPostId(tumblrPost.OwnerUsername, tumblrPost.Id) :
                !tumblrPost.PostUrl.StartsWith("https://www.tumblr.com") && !string.IsNullOrEmpty(tumblrPost.OwnerUsername) && !string.IsNullOrEmpty(tumblrPost.Id) ?
                ConstantHelpDetails.GetPostUrlByUserNameAndPostId(tumblrPost.OwnerUsername, tumblrPost.Id)
                : tumblrPost.PostUrl;
            LoadPageSource(account, url, true);
            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    BrowserWindow.ClearResources();
                    await BrowserWindow.BrowserActAsync(ActType.ActByQuery, AttributeType.AriaLabel, "Like", index: 0, delayAfter: 3);
                    likeResponseList = await BrowserWindow.GetPaginationDataList("\"meta\":{\"status\":200,\"msg\":\"OK\"", true);
                    isRunning = false;
                }
                catch (OperationCanceledException)
                {
                    isRunning = false;
                    throw new OperationCanceledException();
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                    isRunning = false;
                    throw;
                }
            });
            while (isRunning)
            {
                TimeSpan.FromSeconds(2);
            }
            return new LikePostResponse(new ResponseParameter() { Response = likeResponseList.FirstOrDefault() });
        }
        public UnLikePostResponse UnLikePost(DominatorAccountModel account, TumblrPost tumblrPost)
        {
            bool isRunning = true;
            var unlikeResponseList = new List<string>();
            var url = string.IsNullOrEmpty(tumblrPost.PostUrl) && !string.IsNullOrEmpty(tumblrPost.OwnerUsername) && !string.IsNullOrEmpty(tumblrPost.Id) ?
                ConstantHelpDetails.GetPostUrlByUserNameAndPostId(tumblrPost.OwnerUsername, tumblrPost.Id) :
                !tumblrPost.PostUrl.StartsWith("https://www.tumblr.com") && !string.IsNullOrEmpty(tumblrPost.OwnerUsername) && !string.IsNullOrEmpty(tumblrPost.Id) ?
                ConstantHelpDetails.GetPostUrlByUserNameAndPostId(tumblrPost.OwnerUsername, tumblrPost.Id)
                : tumblrPost.PostUrl;
            LoadPageSource(account, url, true);
            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    BrowserWindow.ClearResources();
                    await BrowserWindow.BrowserActAsync(ActType.ActByQuery, AttributeType.AriaLabel, "Unlike", index: 0, delayAfter: 4);
                    unlikeResponseList = await BrowserWindow.GetPaginationDataList("\"meta\":{\"status\":200,\"msg\":\"OK\"", true);
                    CancellationToken.ThrowIfCancellationRequested();

                    isRunning = false;
                }
                catch (OperationCanceledException)
                {
                    isRunning = false;
                    throw new OperationCanceledException();
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                    isRunning = false;
                    throw;
                }
            });
            while (isRunning)
            {
                TimeSpan.FromSeconds(2);
            }
            return new UnLikePostResponse(new ResponseParameter() { Response = unlikeResponseList.FirstOrDefault() });
        }
        public bool Comment(DominatorAccountModel account, string message, ref TumblrPost tumblrPost)
        {
            bool isRunning = true;
            var url = tumblrPost.PostUrl;
            SearchPostsResonseHandler postDetails = new SearchPostsResonseHandler();
            if (BrowserWindow == null)
                BrowserLogin(account, CancellationToken);
            bool status = false;
            Application.Current.Dispatcher.Invoke(async () =>
            {

                try
                {
                    BrowserWindow.ClearResources();
                    await BrowserWindow.GoToCustomUrl(url, delayAfter: 6);
                    CurrentData = await BrowserWindow.GetPageSourceAsync();
                    if (CurrentData != null) postDetails = new SearchPostsResonseHandler(Response);
                    if (postDetails.LstTumblrPosts.FirstOrDefault().CanReply)
                    {
                        await BrowserWindow.BrowserActAsync(ActType.CustomActByQueryType, AttributeType.AriaLabel, "Reply", value: "scrollIntoViewIfNeeded()", index: 0, delayAfter: 4);
                        int replyFieldCount = 0;
                        if (int.TryParse(await BrowserWindow.GetElementValueAsync(ActType.GetLengthByQuery, AttributeType.AriaLabel, "Reply field"), out replyFieldCount) && replyFieldCount == 0)
                            await BrowserWindow.BrowserActAsync(ActType.ActByQuery, AttributeType.AriaLabel, "Reply", index: 0, delayAfter: 5);
                        var xy = await BrowserWindow.GetXAndYAsync(customScriptX: $"{ConstantHelpDetails.replyTextAreaScript}.getBoundingClientRect().x",
                            customScriptY: $"{ConstantHelpDetails.replyTextAreaScript}.getBoundingClientRect().y");
                        if (xy.Key != 0 && xy.Value != 0)
                            await BrowserWindow.MouseClickAsync(xy.Key + 23, xy.Value + 11, delayAfter: 2);
                        await BrowserWindow.EnterCharsAsync(message, delayAtLast: 2);
                        var replySendButtonScript = ConstantHelpDetails.GetScriptforTwoAttributesAndValue(ActType.Click, AttributeType.AriaLabel,
                        "Reply", AttributeType.DataTestId, "reply-button", clickIndex: 0);
                        var replyResp = await BrowserWindow.ExecuteScriptAsync(replySendButtonScript, delayInSec: 6);
                        if (replyResp.Success)
                            status = true;
                        else
                        {
                            await BrowserWindow.BrowserActAsync(ActType.Click, AttributeType.ClassName, ConstantHelpDetails.CommentClass, delayAfter: 6);
                            status = true;
                        }
                        CancellationToken.ThrowIfCancellationRequested();
                    }
                    else
                    {
                        status = false;
                    }
                    isRunning = false;
                }
                catch (OperationCanceledException)
                {
                    isRunning = false;
                    throw new OperationCanceledException();
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                    status = false;
                    isRunning = false;
                    return;
                }

            });
            while (isRunning)
            {
                TimeSpan.FromSeconds(2);
            }
            if (postDetails.Success) tumblrPost = postDetails.LstTumblrPosts?.FirstOrDefault();
            return status;
        }
        public ReblogPostResponse ReblogPost(DominatorAccountModel DominatorAccountModel, TumblrPost tumblrPost)
        {
            bool isRunning = true;
            var publishResponseList = new List<string>();
            LoadPageSource(DominatorAccountModel, tumblrPost.PostUrl, clearandNeedResource: true);
            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    await BrowserWindow.BrowserActAsync(ActType.ActByQuery, AttributeType.AriaLabel, "Reblog", index: 0, delayAfter: 8);
                    CancellationToken.ThrowIfCancellationRequested();
                    await BrowserWindow.ExecuteScriptAsync($"{ConstantHelpDetails.ReblogButtonScript}.click()", delayInSec: 8);
                    publishResponseList = await BrowserWindow.GetPaginationDataList("\"meta\":{\"status\":201,\"msg\":\"Created\"", true, "\"state\":\"published\"");
                    isRunning = false;
                }
                catch (OperationCanceledException)
                {
                    isRunning = false;
                    throw new OperationCanceledException();
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                    isRunning = false;
                    return;
                }
            });
            while (isRunning)
            {
                TimeSpan.FromSeconds(2);
            }
            return new ReblogPostResponse(new ResponseParameter() { Response = publishResponseList.FirstOrDefault() });
        }
        public bool BroadCastMessage(DominatorAccountModel DominatorAccountModel, string message, string media, ref TumblrUser tumblrUser)
        {
            bool isRunning = true;
            bool status = false;
            SearchUsersForKeywordRespone userDetails = new SearchUsersForKeywordRespone();
            var blogUrl = string.IsNullOrEmpty(tumblrUser.PageUrl) && !string.IsNullOrEmpty(tumblrUser.UserId) ?
                ConstantHelpDetails.BlogViewUrl(tumblrUser.UserId) :
                string.IsNullOrEmpty(tumblrUser.PageUrl) && !string.IsNullOrEmpty(tumblrUser.Username) ? ConstantHelpDetails.BlogViewUrl(tumblrUser.Username)
                : tumblrUser.PageUrl;
            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    if (!string.IsNullOrEmpty(BrowserWindow.CurrentUrl()) && BrowserWindow.CurrentUrl() != blogUrl)
                    {
                        BrowserWindow.ClearResources();
                        await BrowserWindow.GoToCustomUrl(blogUrl, 5);
                    }
                    CurrentData = await BrowserWindow.GetPageSourceAsync();
                    if (CurrentData != null) userDetails = new SearchUsersForKeywordRespone(Response);
                    if (userDetails.Success && userDetails.LstTumblrUser.FirstOrDefault().CanMessage)
                    {
                        var clickMessageButtonClass = HtmlParseUtility.GetAttributeValueFromTagName(CurrentData, "button", "aria-label", "Message", "class");
                        await BrowserWindow.BrowserActAsync(ActType.Click, AttributeType.ClassName, clickMessageButtonClass, delayAfter: 2);
                        var xy = await BrowserWindow.GetXAndYAsync(AttributeType.ClassName, ConstantHelpDetails.TextAreaClass, index: 0);
                        await BrowserWindow.MouseClickAsync(xy.Key + 11, xy.Value + 6);
                        if (!string.IsNullOrEmpty(message))
                        {
                            await BrowserWindow.EnterCharsAsync(message);
                            await Task.Delay(1000);
                        }
                        if (!string.IsNullOrEmpty(media))
                        {
                            BrowserWindow.ChooseFileFromDialog(filePath: media);
                            await Task.Delay(10000);
                            await BrowserWindow.BrowserActAsync(ActType.Click, AttributeType.ClassName, "TRX6J MD5Ec RR_oP", delayAfter: 5, delayBefore: 5);
                        }
                        await Task.Delay(3000);
                        var MessageSendButtonScript = ConstantHelpDetails.GetScriptforTwoAttributesAndValue(ActType.Click, AttributeType.ClassName,
                        "TRX6J nWfaK", AttributeType.AriaLabel, "Send", clickIndex: 0);
                        var MessageSendResponse = await BrowserWindow.ExecuteScriptAsync(MessageSendButtonScript, 5);
                        if (MessageSendResponse.Success) status = true;
                        else
                            await BrowserWindow.BrowserActAsync(ActType.Click, AttributeType.ClassName, ConstantHelpDetails.SubmitClass, delayAfter: 5, delayBefore: 1);
                        status = true;
                    }
                    else
                        status = false;
                    isRunning = false;
                }
                catch (OperationCanceledException)
                {
                    isRunning = false;
                    throw new OperationCanceledException();
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                    status = false;
                    isRunning = false;
                    return;
                }
            });
            while (isRunning)
            {
                TimeSpan.FromSeconds(2);
            }
            if (userDetails.Success) tumblrUser = userDetails.LstTumblrUser?.FirstOrDefault();
            return status;
        }
        public PublishResponseHandler PublishPost(DominatorAccountModel dominatorAccountModel, PublisherPostlistModel updatedPostModel)
        {
            bool isRunning = true;
            var publishResponse = string.Empty;
            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    int imagecounter = 0;
                    int vediocounter = 0;
                    await BrowserWindow.GoToCustomUrl("https://www.tumblr.com/new/text", delayAfter: 7);
                    await WaitToLoadPageResponse(dominatorAccountModel, "aria-label=\"Block: Heading\"");
                    var getPageSource = await BrowserWindow.GetPageSourceAsync();
                    var postTextXY = await BrowserWindow.GetXAndYAsync(customScriptX: $"{ConstantHelpDetails.posTextClickScript}[0].getBoundingClientRect().x"
                                                , customScriptY: $"{ConstantHelpDetails.posTextClickScript}[0].getBoundingClientRect().y");
                    if (postTextXY.Key != 0 && postTextXY.Value != 0 && postTextXY.Key > 100 && postTextXY.Value > 100)
                        await BrowserWindow.MouseClickAsync(postTextXY.Key + 15, postTextXY.Value + 10, delayAfter: 2);

                    if (!string.IsNullOrEmpty(updatedPostModel.PostDescription))
                        await BrowserWindow.CopyPasteContentAsync(updatedPostModel.PostDescription, 86, delayAtLast: 3);
                    if (updatedPostModel.MediaList.Count != 0)
                    {
                        if (!string.IsNullOrEmpty(updatedPostModel.PostDescription))
                        {
                            await BrowserWindow.PressAnyKeyUpdated();
                            await Task.Delay(2000, cancellationToken: CancellationToken);
                        }
                        getPageSource = await BrowserWindow.GetPageSourceAsync();
                        var addPhotoClassName = HtmlParseUtility.GetAttributeValueFromTagName(getPageSource, "button", "aria-label", "Add photo", "class");
                        var addVedioClassname = HtmlParseUtility.GetAttributeValueFromTagName(getPageSource, "button", "aria-label", "Add video", "class");
                        var addAudioClassname = HtmlParseUtility.GetAttributeValueFromTagName(getPageSource, "button", "aria-label", "Add audio", "class");
                        List<string> imagefiles = new List<string>();
                        List<string> vediofiles = new List<string>();
                        List<string> audiofiles = new List<string>();
                        foreach (var media in updatedPostModel.MediaList)
                        {
                            if (!File.Exists(media) && !ImageExtracter.IsValidUrl(media)) continue;
                            var fileExtension = Path.GetExtension(media);

                            try
                            {
                                var filePath = updatedPostModel.IsChangeHashOfMedia ? MediaUtilites.CalculateMD5Hash(media) : media;
                                if (TumblrUtility.isImage(fileExtension))
                                {
                                    if (imagecounter++ == 30) continue;
                                    imagefiles.Add(filePath);
                                }
                                else if (TumblrUtility.IsVedio(fileExtension))
                                {
                                    if (vediocounter++ == 1) continue;
                                    vediofiles.Add(filePath);
                                }
                                else if (TumblrUtility.isAudio(fileExtension))
                                {
                                    if (vediocounter++ == 10) continue;
                                    audiofiles.Add(filePath);
                                }
                                else
                                {
                                    GlobusLogHelper.log.Info(Log.CustomMessage, dominatorAccountModel.AccountBaseModel.AccountNetwork,
                                        dominatorAccountModel.AccountBaseModel.UserName, "Can Not Upload This File As Not Supported This File Format =>" + fileExtension);
                                }
                            }
                            catch (Exception ex)
                            {
                                ex.DebugLog();
                            }
                        }
                        if (imagefiles.Count > 0 && !string.IsNullOrEmpty(addPhotoClassName))
                        {
                            await BrowserWindow.BrowserActAsync(ActType.Click, AttributeType.ClassName, addPhotoClassName, delayAfter: 10);
                            getPageSource = await BrowserWindow.GetPageSourceAsync();
                            await Task.Delay(5000, cancellationToken: CancellationToken);
                            BrowserWindow.ChooseFileFromDialog(pathList: imagefiles);
                            var clickinputimageButtonClass = HtmlParseUtility.GetAttributeValueFromTagName(getPageSource, "button", "aria-label", "Upload an image", "class");
                            var XandY = BrowserWindow.GetXAndY(AttributeType.ClassName, clickinputimageButtonClass, 0);
                            await BrowserWindow.MouseClickAsync(XandY.Key + 50, XandY.Value + 10);
                            await Task.Delay(10000, cancellationToken: CancellationToken);
                        }
                        if (vediofiles.Count > 0 && !string.IsNullOrEmpty(addVedioClassname))
                        {
                            if (imagefiles.Count > 0) await BrowserWindow.PressAnyKeyUpdated();
                            await Task.Delay(5000, cancellationToken: CancellationToken);
                            await BrowserWindow.BrowserActAsync(ActType.Click, AttributeType.ClassName, addVedioClassname, index: 4, delayAfter: 10);
                            getPageSource = await BrowserWindow.GetPageSourceAsync();
                            await Task.Delay(5000, cancellationToken: CancellationToken);
                            BrowserWindow.ChooseFileFromDialog(pathList: vediofiles);
                            var clickinputvedioButtonClass = HtmlParseUtility.GetAttributeValueFromTagName(getPageSource, "button", "aria-label", "Upload a video", "class");
                            var XandY = BrowserWindow.GetXAndY(AttributeType.ClassName, clickinputvedioButtonClass, 0);
                            await BrowserWindow.MouseClickAsync(XandY.Key + 50, XandY.Value + 10, delayAfter: 5);
                        }
                        if (audiofiles.Count > 0 && !string.IsNullOrEmpty(addAudioClassname))
                        {
                            await BrowserWindow.PressAnyKeyUpdated();
                            await Task.Delay(5000, cancellationToken: CancellationToken);
                            await BrowserWindow.BrowserActAsync(ActType.Click, AttributeType.ClassName, addAudioClassname, index: 3, delayAfter: 10);
                            getPageSource = await BrowserWindow.GetPageSourceAsync();
                            await Task.Delay(5000, cancellationToken: CancellationToken);
                            BrowserWindow.ChooseFileFromDialog(pathList: audiofiles);
                            var clickinputAudioButtonClass = HtmlParseUtility.GetAttributeValueFromTagName(getPageSource, "button", "aria-label", "Upload an audio", "class");
                            var XandY = BrowserWindow.GetXAndY(AttributeType.ClassName, clickinputAudioButtonClass, 0);
                            await BrowserWindow.MouseClickAsync(XandY.Key + 50, XandY.Value + 10);
                            await Task.Delay(20000, cancellationToken: CancellationToken);
                        }
                        await Task.Delay(20000, cancellationToken: CancellationToken);

                    }
                    if (!string.IsNullOrEmpty(updatedPostModel.PublisherInstagramTitle))
                    {
                        var clickTextClassName = HtmlParseUtility.GetAttributeValueFromTagName(getPageSource, "h1", "aria-label", "Block: Heading", "class");
                        if (!string.IsNullOrEmpty(clickTextClassName))
                        {
                            var xy = await BrowserWindow.GetXAndYAsync(AttributeType.ClassName, clickTextClassName, index: 0);
                            await BrowserWindow.MouseClickAsync(xy.Key + 50, xy.Value + 10);
                            await Task.Delay(3000, CancellationToken);
                            await BrowserWindow.EnterCharsAsync(" " + updatedPostModel.PublisherInstagramTitle, 0.10);
                            await Task.Delay(5000, CancellationToken);
                        }
                    }

                    if (updatedPostModel.TumblrPostSettings.IsTagUser)
                    {
                        var classTagUsers = HtmlParseUtility.GetAttributeValueFromTagName(getPageSource, "textarea", "aria-label", "Tags editor", "class");
                        if (!string.IsNullOrEmpty(classTagUsers))
                        {
                            var xy = await BrowserWindow.GetXAndYAsync(AttributeType.ClassName, classTagUsers, index: 0);
                            await BrowserWindow.MouseClickAsync(xy.Key + 60, xy.Value + 15);
                            await Task.Delay(5000, cancellationToken: CancellationToken);
                            await BrowserWindow.EnterCharsAsync(updatedPostModel.TumblrPostSettings.TagUserList.Replace("#", ""), 0.10);
                            await Task.Delay(5000, cancellationToken: CancellationToken);
                        }
                    }
                    await WaitToLoadPageResponse(dominatorAccountModel, "class=\"TRX6J VxmZd\" disabled", true);
                    //Wait Some Extra Time To Load Media Properly.
                    await Task.Delay(50000, cancellationToken: CancellationToken);
                    BrowserWindow.SetResourceLoadInstance();
                    BrowserWindow.ClearResources();
                    await BrowserWindow.BrowserActAsync(ActType.Click, AttributeType.ClassName, "TRX6J VxmZd", delayAfter: 5);
                    _Pagetext = await BrowserWindow.PageText();
                    if (_Pagetext.Contains("Post without tags?") && _Pagetext.Contains("Post"))
                    {
                        _Pagetext = await BrowserWindow.GetPageSourceAsync();
                        var clickPostClassName = HtmlParseUtility.GetAttributeValueFromTagName(_Pagetext, "button", "aria-label", "Post", "class");
                        if (!string.IsNullOrEmpty(clickPostClassName))
                            await BrowserWindow.BrowserActAsync(ActType.Click, AttributeType.ClassName, clickPostClassName, delayAfter: 5);
                        _Pagetext = await BrowserWindow.PageText();
                    }
                    await WaitToLoadPageResponse(dominatorAccountModel, ConstantHelpDetails.ProcessingMedia, true);
                    var ListResponse = await BrowserWindow.GetPaginationDataList("\"meta\":{\"status\":201,\"msg\":\"Created\"", true);
                    publishResponse = ListResponse != null && ListResponse.Count > 0 ? ListResponse?.FirstOrDefault(x => !string.IsNullOrEmpty(x) && x.Contains("\"state\":\"published\"")) ?? ListResponse?.FirstOrDefault() : string.Empty;
                    isRunning = false;
                }
                catch (OperationCanceledException)
                {
                    isRunning = false;
                    throw new OperationCanceledException();
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                    isRunning = false;
                }
                finally
                {
                    if (BrowserWindow != null && !BrowserWindow.IsDisposed)
                        BrowserWindow.Close();
                }
            });
            while (isRunning)
            {
                TimeSpan.FromSeconds(2);
            }
            var UserId = dominatorAccountModel.AccountBaseModel.UserId;
            return new PublishResponseHandler(new ResponseParameter() { Response = publishResponse }, UserId);
        }
        private async Task TumblrBrowserLogin(DominatorAccountModel account, LoginType loginType)
        {
            var last3Min = DateTime.Now.AddMinutes(2);
            while (!BrowserWindow.IsLoggedIn && !BrowserWindow.IsDisposed)
            {
                if (!BrowserWindow.IsLoaded)
                {
                    await Task.Delay(3000);
                    continue;
                }
                var getPageText = await BrowserWindow.GetPageSourceAsync();
                if (getPageText.Contains("There was a problem logging in, try again later.")) return;

                if (getPageText.Contains("\"queryKey\":[\"user-info\",true]") && getPageText.Contains("\"isLoggedIn\":true"))
                {

                    await Task.Delay(2000);
                    account.IsUserLoggedIn = true;
                    if (loginType == LoginType.HiddenLogin)
                        await BrowserWindow.SaveCookies(false);
                    else
                        await BrowserWindow.SaveCookies();
                    tumblrAccountSession.AddOrUpdateSession(ref account, true);
                    break;
                }
                if (getPageText.Contains("email"))
                {
                    var classNameofemailInput = HtmlParseUtility.GetAttributeValueFromTagName(getPageText, "input", "name", "email", "class");
                    if (!string.IsNullOrEmpty(classNameofemailInput))
                    {
                        var xy = await BrowserWindow.GetXAndYAsync(AttributeType.ClassName, classNameofemailInput, index: 0);
                        await BrowserWindow.MouseClickAsync(xy.Key + 40, xy.Value + 10);
                        await BrowserWindow.EnterCharsAsync(account.AccountBaseModel.UserName, delayBefore: 1, delayAtLast: 2);
                        var clickNextClassName = HtmlParseUtility.GetAttributeValueFromTagName(getPageText, "button", "aria-label", "Next", "class");
                        if (!string.IsNullOrEmpty(clickNextClassName))
                        {
                            BrowserWindow.BrowserAct(ActType.ClickByClass, clickNextClassName, delayAfter: 1);
                        }
                    }
                }
                if (getPageText.Contains("password"))
                {
                    var classNameofPasswordInput = HtmlParseUtility.GetAttributeValueFromTagName(getPageText, "input", "name", "password", "class");
                    if (!string.IsNullOrEmpty(classNameofPasswordInput))
                    {
                        var xy = await BrowserWindow.GetXAndYAsync(AttributeType.ClassName, classNameofPasswordInput, index: 1);
                        await BrowserWindow.MouseClickAsync(xy.Key + 50, xy.Value + 10);
                        await BrowserWindow.EnterCharsAsync(account.AccountBaseModel.Password, delayBefore: 1, delayAtLast: 2);

                        var clickLoginClassName = HtmlParseUtility.GetAttributeValueFromTagName(getPageText, "button", "aria-label", "Log in", "class");
                        if (!string.IsNullOrEmpty(clickLoginClassName))
                        {
                            BrowserWindow.BrowserAct(ActType.ClickByClass, clickLoginClassName, delayAfter: 6);
                            await Task.Delay(10000);
                        }
                    }

                }
                if (getPageText.Contains("signup_view magiclink active"))
                {
                    BrowserWindow.BrowserAct(ActType.ClickByClass, "forgot_password_link", delayAfter: 1);
                }
                if (getPageText.Contains("loginUsername"))
                {
                    BrowserWindow.BrowserAct(ActType.EnterValueById, "loginUsername", 1, 1, account.AccountBaseModel.UserName);
                    BrowserWindow.BrowserAct(ActType.EnterValueById, "loginPassword", 0, 1, account.AccountBaseModel.Password);
                    BrowserWindow.BrowserAct(ActType.ClickByClass, "AnimatedForm__submitButton", delayAfter: 1);
                }

                if (getPageText.Contains("login_login-main"))
                {
                    BrowserWindow.BrowserAct(ActType.EnterValueByName, "user", 1, 1, account.AccountBaseModel.UserName);
                    BrowserWindow.BrowserAct(ActType.EnterValueByName, "passwd", 0, 1, account.AccountBaseModel.Password);
                    BrowserWindow.BrowserAct(ActType.ClickByClass, "submit", delayAfter: 1);
                }
                CancellationToken.ThrowIfCancellationRequested();
                if (last3Min < DateTime.Now)
                {
                    var item = await BrowserWindow.GetPaginationDataList("Tumblr", true);
                    if (item.Count == 0)
                    {
                        account.AccountBaseModel.Status = AccountStatus.ProxyNotWorking;
                        account.IsUserLoggedIn = false;
                        break;
                    }
                }
                if (!BrowserWindow.IsLoggedIn)
                {
                    var result = await BrowserWindow.GetPageSourceAsync();
                    if (!string.IsNullOrEmpty(result) && result.Contains("\"isLoggedIn\":true"))
                    {
                        await Task.Delay(2000);
                        account.IsUserLoggedIn = true;
                        if (loginType == LoginType.HiddenLogin)
                            await BrowserWindow.SaveCookies(false);
                        else
                            await BrowserWindow.SaveCookies();
                        BrowserWindow.LoadPostPage(account.IsUserLoggedIn);
                    }
                    if (!string.IsNullOrEmpty(result) && result.Contains("Your account has been terminated"))
                    {
                        account.IsUserLoggedIn = false;
                        account.AccountBaseModel.Status = AccountStatus.ProfileSuspended;
                        BrowserWindow.IsLoggedIn = false;
                        break;
                    }
                    if (!string.IsNullOrEmpty(result) && result.Contains("Your email or password were incorrect"))
                    {
                        account.IsUserLoggedIn = false;
                        account.AccountBaseModel.Status = AccountStatus.InvalidCredentials;
                        BrowserWindow.IsLoggedIn = false;
                        break;
                    }

                }
                if (account.IsUserLoggedIn)
                {
                    tumblrAccountSession.AddOrUpdateSession(ref account, true);
                    break;
                }
            }

        }

        public SearchPostsResonseHandler GetTumblrPostsFromKeyWord(DominatorAccountModel dominatorAccountModel, string keyWord, SearchFilterModel searchFilter, bool IsHasTag = false)
        {
            var PostList = new List<TumblrPost>();
            bool isRunning = true;
            var screenResolution = TumblrUtility.GetScreenResolution();
            var url = ConstantHelpDetails.GetSearchFilterUrl(Uri.EscapeDataString(keyWord), searchFilter, true, IsHasTag).Item1;
            SearchPostsResonseHandler searchPostsResonse = new SearchPostsResonseHandler();
            try
            {
                Application.Current.Dispatcher.Invoke(async () =>
                {
                    try
                    {
                        BrowserWindow.ClearResources();
                        await BrowserWindow.GoToCustomUrl(url, 10);
                        int scrollCount = 0;
                        while (scrollCount < 2)
                        {
                            await BrowserWindow.MouseScrollAsync(screenResolution.Key / 2, screenResolution.Value / 2
                                        , 0, -500, delayAfter: 5, scrollCount: 6);
                            scrollCount++;
                        }
                        var postResponseList = await BrowserWindow.GetPaginationDataList("\"meta\":{\"status\":200,\"msg\":\"OK\"},\"response\":{\"timeline\":{\"elements\":[{\"objectType\":\"post\"", true);
                        foreach (var postResponse in postResponseList)
                        {
                            var tempResponse = new SearchPostsResonseHandler(new ResponseParameter { Response = postResponse });
                            PostList.AddRange(tempResponse.LstTumblrPosts);
                        }
                        searchPostsResonse = new SearchPostsResonseHandler(true) { LstTumblrPosts = PostList, NextPageUrl = string.Empty, tumblr_form_key = string.Empty };

                        isRunning = false;
                    }
                    catch (OperationCanceledException)
                    {
                        isRunning = false;
                        throw new OperationCanceledException();
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                        isRunning = false;
                    }
                });
                while (isRunning)
                {
                    TimeSpan.FromSeconds(2);
                }
            }
            catch (Exception)
            {
                return searchPostsResonse;
            }
            return searchPostsResonse;
        }

        public SearchUsersForKeywordRespone GetTumblrUsersFromKeyWord(DominatorAccountModel dominatorAccountModel, string keyWord, SearchFilterModel searchFilter, bool IsHasTag = false)
        {
            var UserList = new List<TumblrUser>();
            bool isRunning = true;
            var url = ConstantHelpDetails.GetSearchFilterUrl(Uri.EscapeDataString(keyWord), searchFilter, true, IsHasTag).Item1;
            var jsonresponseList = new List<string>();
            SearchUsersForKeywordRespone searchuserResponseHandler = new SearchUsersForKeywordRespone();
            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    BrowserWindow.ClearResources();
                    await BrowserWindow.GoToCustomUrl(url, 10);
                    bool isShowBlogPresent = true;
                    while (isShowBlogPresent)
                    {
                        var ShowBlog = await BrowserWindow.GetElementValueAsync(ActType.GetLengthByQuery, AttributeType.AriaLabel, "Show more blogs");
                        isShowBlogPresent = !string.IsNullOrEmpty(ShowBlog) && int.Parse(ShowBlog) > 0 ? true : false;
                        var tempuserResponseList = await BrowserWindow.GetPaginationDataList("\"meta\":{\"status\":200,\"msg\":\"OK\"},\"response\":{\"timeline\":{\"elements\":[{\"objectType\":\"blog\"", true);
                        tempuserResponseList.RemoveAll(x => jsonresponseList.Any(y => y.Contains(x)));
                        jsonresponseList.AddRange(tempuserResponseList);
                        CancellationToken.ThrowIfCancellationRequested();
                        if (isShowBlogPresent) await BrowserWindow.BrowserActAsync(ActType.ActByQuery, AttributeType.AriaLabel, "Show more blogs", delayAfter: 8);
                        else break;
                    }
                    foreach (var userResponse in jsonresponseList)
                    {
                        var tempResponse = new SearchUsersForKeywordRespone(new ResponseParameter { Response = userResponse });
                        UserList.AddRange(tempResponse.LstTumblrUser);
                    }
                    searchuserResponseHandler = new SearchUsersForKeywordRespone(true) { LstTumblrUser = UserList, Cursor = "" };
                    isRunning = false;
                }
                catch (OperationCanceledException)
                {
                    isRunning = false;
                    throw new OperationCanceledException();
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                    isRunning = false;
                    return;
                }
            });
            while (isRunning)
            {
                TimeSpan.FromSeconds(2);
            }
            return searchuserResponseHandler;

        }

        public string SearchUserDetails(DominatorAccountModel dominatorAccountModel, ref TumblrUser tumblrUser)
        {
            var url = tumblrUser.PageUrl;
            bool isRunning = true;
            var status = "";
            SearchUsersForKeywordRespone userDetails = new SearchUsersForKeywordRespone();
            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    BrowserWindow.ClearResources();
                    await BrowserWindow.GoToCustomUrl(url, 5);
                    CurrentData = await BrowserWindow.GetPageSourceAsync();
                    if (CurrentData != null)
                    {
                        userDetails = new SearchUsersForKeywordRespone(Response);
                        status = "true";
                        isRunning = false;
                    }
                    else
                    {
                        status = "false";
                        isRunning = false;
                    }

                }
                catch (OperationCanceledException)
                {
                    isRunning = false;
                    status = "false";
                    throw new OperationCanceledException();
                }
                catch (Exception ex)
                {
                    status = "false";
                    ex.DebugLog();
                    isRunning = false;
                    return;
                }
            });
            while (isRunning)
            {
                TimeSpan.FromSeconds(2);
            }
            if (userDetails.Success) tumblrUser = userDetails.LstTumblrUser[0];
            return status;
        }

        public List<TumblrPost> GetUserPostsDetails(DominatorAccountModel dominatorAccountModel, TumblrUser tumblrUser)
        {
            var url = tumblrUser.PageUrl;
            bool isRunning = true;
            SearchPostsResonseHandler userpostDetails = new SearchPostsResonseHandler();
            var postList = new List<TumblrPost>();
            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    BrowserWindow.ClearResources();
                    await BrowserWindow.GoToCustomUrl(url, 5);
                    CurrentData = await BrowserWindow.GetPageSourceAsync();
                    if (CurrentData != null)
                    {
                        userpostDetails = new SearchPostsResonseHandler(Response);
                        postList = userpostDetails.LstTumblrPosts;
                        isRunning = false;
                    }
                    else
                        isRunning = false;
                }
                catch (OperationCanceledException)
                {
                    isRunning = false;
                    throw new OperationCanceledException();
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                    isRunning = false;
                    return;
                }
            });
            while (isRunning)
            {
                TimeSpan.FromSeconds(2);
            }
            return postList;
        }

        public SearchforFollowingsorFollowersResponse GetSomeOnesFollowings(string pageUrl, int? pageCount = null)
        {
            var userList = new List<TumblrUser>();
            bool isRunning = true;
            int elementLength = 0;
            int elementindex = 0;

            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    BrowserWindow.ClearResources();
                    CurrentData = await BrowserWindow.GoToCustomUrl(pageUrl, 6);
                    var elementlengthresp = await BrowserWindow.GetElementValueAsync(ActType.GetLength, AttributeType.ClassName, "rZlUD W45iW");
                    if (!string.IsNullOrEmpty(elementlengthresp)) elementLength = Convert.ToInt32(elementlengthresp);
                    while (elementindex < elementLength)
                    {
                        var elementNametext = await BrowserWindow.GetElementValueAsync(ActType.GetValue, AttributeType.ClassName, "rZlUD W45iW", ValueTypes.InnerText, clickIndex: elementindex);
                        var userName = Utilities.GetBetween("1/" + elementNametext, "1/", "\n");
                        if (!string.IsNullOrEmpty(userName))
                            userList.Add(
                                new TumblrUser()
                                {
                                    Username = userName,
                                    PageUrl = $"https://www.tumblr.com/{userName}"
                                }
                                );
                        elementindex++;
                        if (elementindex == elementLength)
                        {
                            await BrowserWindow.BrowserActAsync(ActType.ScrollWindow, AttributeType.ClassName, "", scrollByPixel: 1000, delayAfter: 5);
                            elementlengthresp = await BrowserWindow.GetElementValueAsync(ActType.GetLength, AttributeType.ClassName, "rZlUD W45iW");
                            if (!string.IsNullOrEmpty(elementlengthresp)) elementLength = Convert.ToInt32(elementlengthresp);
                        }
                    }
                    await Task.Delay(3000);
                    isRunning = false;
                }
                catch (OperationCanceledException)
                {
                    isRunning = false;
                    throw new OperationCanceledException();
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                    isRunning = false;
                    return;
                }
            });
            while (isRunning)
            {
                TimeSpan.FromSeconds(2);
            }
            return new SearchforFollowingsorFollowersResponse()
            { LstTumblrUser = userList };
        }
        public string LoadPageSource(DominatorAccountModel account, string url, bool clearandNeedResource = false)
        {
            bool isRunning = true;
            string pageSource = string.Empty;
            DateTime currentDate = DateTime.Now;
            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {

                    if (BrowserWindow == null)
                    {
                        BrowserWindow = new BrowserWindow(account, targetUrl: url, isNeedResourceData: clearandNeedResource, customUse: true)
                        {
                            Visibility = Visibility.Hidden
                        };
#if DEBUG
                        BrowserWindow.Visibility = Visibility.Visible;
#endif
                        if ((account.Cookies?.Count == 0 || account.CookieHelperList.Any(x => x.Name.Contains("logged_in") && x.Value == "0")) && account.BrowserCookies?.Count > 0)
                            BrowserWindow.BrowserSetCookie();
                        else
                            BrowserWindow.SetCookie();
                        BrowserWindow.Show();
                        await Task.Delay(10000);
                    }
                    else if (BrowserWindow != null && (!BrowserWindow.CurrentUrl().Contains(url) || clearandNeedResource))
                    {
                        if (clearandNeedResource)
                        {
                            BrowserWindow.ClearResources();
                            BrowserWindow.SetResourceLoadInstance();
                        }
                        await BrowserWindow.GoToCustomUrl(url, 7);
                    }
                    do
                    {
                        await Task.Delay(1500);
                        pageSource = await BrowserWindow.GetPageSourceAsync();
                    } while (!BrowserWindow.IsLoaded || (string.IsNullOrEmpty(pageSource) && (DateTime.Now - currentDate).TotalSeconds < 15));

                }
                catch (Exception ex) { ex.DebugLog(); }
                isRunning = false;
            });
            while (isRunning)
                Task.Delay(2000, CancellationToken).Wait();
            return pageSource;
        }

        public async Task WaitToLoadPageResponse(DominatorAccountModel dominatorAccount, string ConditionalString, bool CheckContains = false, int RepeatCheckCount = 10, int DefaultDelayInSec = 6, int CheckDelayInSec = 5)
        {
            try
            {
                await Task.Delay(DefaultDelayInSec * 1000, dominatorAccount.Token);
                var PageResponse = await BrowserWindow.GetPageSourceAsync();
                while (RepeatCheckCount-- > 0 && (!string.IsNullOrEmpty(PageResponse) && !string.IsNullOrEmpty(ConditionalString) && (CheckContains ? PageResponse.Contains(ConditionalString) : !PageResponse.Contains(ConditionalString))))
                {
                    await Task.Delay(CheckDelayInSec * 1000, dominatorAccount.Token);
                    PageResponse = await BrowserWindow.GetPageSourceAsync();
                }
            }
            catch { }
        }
    }

}