using DominatorHouseCore;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.PuppeteerBrowser;
using DominatorHouseCore.Request;
using DominatorHouseCore.Utility;
using EmbeddedBrowser;
using GramDominatorCore.GDLibrary.Response;
using GramDominatorCore.GDModel;
using GramDominatorCore.GDUtility;
using GramDominatorCore.Response;
using GramDominatorCore.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace GramDominatorCore.GDLibrary.InstagramBrowser
{
    public interface IGdBrowserManager : IBrowserManager
    {
        PuppeteerBrowserActivity BrowserWindow { get; set; }
        void CloseBrowser(bool ClosePuppeteer = true);
        Task<string> SolveCaptchaBrowserMode();

        SearchKeywordIgResponseHandler SearchForkeyword(DominatorAccountModel _dominatorAccount, string keyword,
            CancellationToken token);
        FriendshipsResponse Follow(DominatorAccountModel _dominatorAccountModel,
            CancellationToken token, InstagramUser instagramUser, string mediaid = null);
        SuggestedUsersIgResponseHandler GetSuggestedUsers(DominatorAccountModel _dominatorAccount, CancellationToken token);
        HashTagFeedIgResponseHandler GetHashtagFeedForUserScraper(DominatorAccountModel _dominatorAccount, QueryInfo queryInfo, CancellationToken token);
        MediaCommentsIgResponseHandler GetMediaComments(DominatorAccountModel dominatorAccountModel, string queryValue, CancellationToken token);
        UserFeedResponse GetLikedMedia(DominatorAccountModel dominatorAccountModel, CancellationToken token);
        MediaLikersIgResponseHandler GetMediaLikers(DominatorAccountModel dominatorAccountModel, string queryValue, CancellationToken token);
        FollowerAndFollowingIgResponseHandler GetUserFollowers(DominatorAccountModel dominatorAccountModel, string userId, CancellationToken token, bool IsCloseFriend = false, string maxID = "");
        FollowerAndFollowingIgResponseHandler GetUserFollowerChrome(DominatorAccountModel dominatorAccountModel, string userId, CancellationToken token, bool IsCloseFriend = false, string maxID = "", List<string> CloseFriends = null);
        CommonIgResponseHandler LikePostComment(DominatorAccountModel dominatorAccountModel, string PostUrl, CancellationToken token);
        CommonIgResponseHandler SeenUserStory(DominatorAccountModel dominatorAccountModel, InstagramUser instagramUser, CancellationToken token);
        MediaInfoIgResponseHandler MediaInfo(DominatorAccountModel dominatorAccountModel, string postUrl, CancellationToken token);
        CommonIgResponseHandler GetFeedTimeLineData(DominatorAccountModel dominatorAccountModel, CancellationToken token);
        FollowerAndFollowingIgResponseHandler GetUserFollowings(DominatorAccountModel dominatorAccountModel, string userId, CancellationToken token);
        CommonIgResponseHandler ReplyComment(DominatorAccountModel dominatorAccountModel, string PostUrl, string comments, CancellationToken token);
        CommonIgResponseHandler UnlikeMedia(DominatorAccountModel dominatorAccountModel, string code, CancellationToken token);
        FriendshipsResponse RemoveFollowers(DominatorAccountModel dominatorAccountModel, AccountModel accountModel, string username, CancellationToken token);
        UserFriendshipResponse UserFriendship(DominatorAccountModel dominatorAccountModel, AccountModel accountModel, string userId, CancellationToken token);
        FriendshipsResponse Block(DominatorAccountModel dominatorAccountModel, CancellationToken token, InstagramUser instagramUser);
        FriendshipsResponse Unfollow(DominatorAccountModel dominatorAccountModel, AccountModel accountModel, CancellationToken token, InstagramUser instagramUser);
        UsernameInfoIgResponseHandler GetUserInfo(DominatorAccountModel dominatorAccountModel, string usernme, CancellationToken token);
        LikeResponse Like(DominatorAccountModel dominatorAccountModel, AccountModel accountModel, string codeUrl, CancellationToken token);
        UserFeedIgResponseHandler GetUserFeed(DominatorAccountModel objDominatorAccountModel, string userName, CancellationToken none, int MaxCount = 0);
        CommentResponse Comment(DominatorAccountModel dominatorAccountModel, AccountModel accountModel, string codeUrl, string commentText, CancellationToken token);
        PostStoryResponse GetStoriesUser(DominatorAccountModel dominatorAccountModel, InstagramUser user, CancellationToken token);
        SendMessageIgResponseHandler SendMessage(DominatorAccountModel dominatorAccountModel, InstagramUser instagramUser, string threadId, string message, string mediaPath, CancellationToken token, List<string> Medias = null, bool SkipAlreadyReceivedMessage = false);
        UploadMediaResponse UploadMedia(DominatorAccountModel dominatorAccountModel, InstagramPost instagramPost, List<string> mediaList, CancellationToken token);
        V2InboxResponse GetInbox(DominatorAccountModel dominatorAccountModel, bool isPendingUsers = false, CancellationToken token = default, bool IsAutoReplyToNewMessage = false, int ScrollCount = 0);
        DeleteMediaIgResponseHandler DeleteMedia(DominatorAccountModel dominatorAccountModel, InstagramPost instagramPost, CancellationToken token);
        Task<CommonIgResponseHandler> SetBiographyAsync(DominatorAccountModel dominatorAccountModel, string bio);
        Task<UsernameInfoIgResponseHandler> EditProfileAsync(DominatorAccountModel dominatorAccountModel, EditProfileModel editProfileModel, CancellationToken none);
        Task<CommonIgResponseHandler> SetGenderAsync(DominatorAccountModel dominatorAccountModel, EditProfileModel editProfileModel, CancellationToken none);
        Task<UsernameInfoIgResponseHandler> ChangeProfilePictureAsync(DominatorAccountModel dominatorAccountModel, EditProfileModel editProfileModel, CancellationToken none);
        List<string> CheckAndAcceptAllPendingMessageRequest(DominatorAccountModel dominatorAccount, int ScrollCount = 4);
        Task<DominatorAccountModel> CheckAndAssignUserName(DominatorAccountModel dominatorAccount);
        Task<(bool, string, List<string>)> GetListJsonResponse();
        Task<CloseFriendsResponseHandler> MakeCloseFriends(DominatorAccountModel dominatorAccount, InstagramUser user);
        Task<CloseFriendsResponseHandler> GetCloseFriendList(DominatorAccountModel dominatorAccount);
        Task<string> VisitPage(DominatorAccountModel dominatorAccount, string Url);
    }

    public class GdBrowserManager : IGdBrowserManager
    {
        public PuppeteerBrowserActivity BrowserWindow { get; set; }
        public PuppeteerBrowserActivity PuppeteerBrowser { get; set; }
        DominatorAccountModel _account;
        bool isverification;
        bool IsSendSecuritycode;
        bool IsCodeSend;
        bool IsAddPhoneNumber;
        bool IsWrongPassword;
        CancellationToken _token;
        private readonly JsonJArrayHandler handler = JsonJArrayHandler.GetInstance;
        public bool BrowserLogin(DominatorAccountModel account, CancellationToken token, LoginType loginType = LoginType.AutomationLogin, VerificationType verification = 0)
        {
            _token = token;
            _account = account;
            var taskCompletionSource = new TaskCompletionSource<bool>();

            // Start background task
            Task.Run(async () =>
            {
                try
                {
                    // Run UI-bound BrowserWindow initialization on UI thread
                    //                    await Application.Current.Dispatcher.InvokeAsync(() =>
                    //                    {
                    //                        try
                    //                        {
                    //                            BrowserWindow = new BrowserWindow(_account, isNeedResourceData: true, NeedRequestHeaders: true)
                    //                            {
                    //                                Visibility = loginType == LoginType.InitialiseBrowser ? Visibility.Hidden : Visibility.Visible,
                    //                                IsLoggedIn = false
                    //                            };

                    //#if DEBUG
                    //                            BrowserWindow.Visibility = Visibility.Visible;
                    //#endif
                    //                            BrowserWindow.SetCookie();
                    //                            BrowserWindow.Show();
                    //                        }
                    //                        catch (Exception ex)
                    //                        {
                    //                            ex.DebugLog();
                    //                            taskCompletionSource.TrySetResult(false); // Fail early if window setup fails
                    //                        }
                    //                    });
                    BrowserWindow = new PuppeteerBrowserActivity(account, isNeedResourceData: true,loginType:loginType);
                    BrowserWindow.SetHeaders(true);
                    await BrowserWindow.LaunchBrowserAsync(false);
                    // Perform login after UI initialized
                    if (BrowserWindow == null || BrowserWindow.IsDisposed)
                    {
                        taskCompletionSource.TrySetResult(false);
                        return;
                    }
                    if (string.IsNullOrEmpty(account.VarificationCode))
                    {
                        try
                        {
                            await MobileLoginOnLoaded(account, loginType, verification);
                        }
                        catch (OperationCanceledException)
                        {
                            taskCompletionSource.TrySetCanceled();
                            return;
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }
                    }

                    taskCompletionSource.TrySetResult(true);
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                    taskCompletionSource.TrySetResult(false);
                }
            }, token);

            // Wait synchronously for completion or cancellation
            try
            {
                taskCompletionSource.Task.Wait(token);
            }
            catch (OperationCanceledException)
            {
                // Handle cancellation gracefully
                return false;
            }

            // After login attempt (success or fail)
            try
            {
                if (BrowserWindow != null && !BrowserWindow.IsDisposed)
                {
                    BrowserWindow.SetHeaders();
                }

                if (BrowserWindow != null && !BrowserWindow.IsDisposed &&
                    (IsWrongPassword || loginType == LoginType.CheckLogin))
                {
                    Application.Current.Dispatcher.Invoke(() => BrowserWindow.Close());
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return taskCompletionSource.Task.Result;
        }

        public async Task<string> SolveCaptchaBrowserMode()
        {
            string response = string.Empty;
            var isCaptchaSolved = false;
            //Application.Current.Dispatcher.InvokeAsync(async () =>
            //{
            //    BrowserWindow = new BrowserWindow()
            //    {
            //        Visibility = Visibility.Visible
            //    };
            //    BrowserWindow.Show();
            //    await Task.Delay(TimeSpan.FromSeconds(5));
            //    BrowserWindow.CaptchaResponse = "";
            //    BrowserWindow.IsCaptchaSolved = false;
            //    BrowserWindow.GoToUrl("https://www.fbsbx.com/captcha/recaptcha/iframe/?compact=0&referer=https%3A%2F%2Fi.instagram.com&locale=en_US&__cci=ig_captcha_iframe");
            //    while (true)
            //    {
            //        if (BrowserWindow.IsCaptchaSolved)
            //        {
            //            response = BrowserWindow.CaptchaResponse;
            //            isCaptchaSolved = BrowserWindow.IsCaptchaSolved;
            //            break;
            //        }
            //        await Task.Delay(TimeSpan.FromSeconds(1));
            //    }
            //}).Wait();
            //while (!isCaptchaSolved)
            //{
            //    await Task.Delay(TimeSpan.FromSeconds(1));
            //}
            return response;
        }


        public async Task InstaMobileLogin(DominatorAccountModel account, LoginType loginType = LoginType.AutomationLogin)
        {
            try
            {
                var pageText = await BrowserWindow.PageText();
                if (string.IsNullOrEmpty(pageText))
                {
                    BrowserWindow.Refresh();
                    await Task.Delay(TimeSpan.FromSeconds(8));
                    while (!BrowserWindow.IsLoaded)
                    {
                        if (BrowserWindow.IsDisposed)
                            return;
                        await Task.Delay(1500);
                        continue;
                    }
                }
                var pageSource = await BrowserWindow.GetPageSourceAsync();
                if (pageSource == "<html><head></head><body></body></html>")
                {
                    //If login page is blocked by instagram then redirect to this page.
                    await BrowserWindow.GoToCustomUrl("https://www.instagram.com/", delayAfter: 10);
                    pageSource = await BrowserWindow.GetPageSourceAsync();
                }
                if (pageSource.Contains("Allow all cookies"))
                    await BrowserWindow.BrowserActAsync(ActType.Click, AttributeType.ClassName, "_a9-- _a9_0", delayAfter: 5, index: 0);
                pageSource = await BrowserWindow.GetPageSourceAsync();
                if (!string.IsNullOrEmpty(pageSource) && pageSource.Contains("Have an account?"))
                {
                    await BrowserWindow.GoToCustomUrl("https://www.instagram.com/accounts/login/?source=auth_switcher", delayAfter: 8);
                    pageSource = await BrowserWindow.GetPageSourceAsync();
                }
                var NewUI = pageSource.Contains("Mobile number, username or email");
                if (pageSource.Contains("Phone number, username, or email") || pageSource.Contains("not-logged-in client-root")
                    ||NewUI)
                {
                    var script = NewUI ? "document.querySelector('input[type=\"{0}\"]').getBoundingClientRect().{1};" :  "document.querySelector('input[name=\"{0}\"]').getBoundingClientRect().{1};";
                    var X = await BrowserWindow.ExecuteScriptAsync(string.Format(script, NewUI ? "text": "username", "x"), delayInSec: 3);
                    var Y = await BrowserWindow.ExecuteScriptAsync(string.Format(script, NewUI ? "text" : "username", "y"), delayInSec: 3);
                    var XCord = GetCordinate(X?.Result?.ToString());
                    var YCord = GetCordinate(Y?.Result?.ToString());
                    // Press Tab to get focus on Username textBox  
                    await BrowserWindow.MouseClickAsync(XCord + 10, YCord + 3, delayAfter: 3);
                    // Enter username
                    await BrowserWindow.EnterCharsAsync(_account.AccountBaseModel.UserName, 0, delayAtLast: 2);
                    X = await BrowserWindow.ExecuteScriptAsync(string.Format(script, "password", "x"), delayInSec: 3);
                    Y = await BrowserWindow.ExecuteScriptAsync(string.Format(script, "password", "y"), delayInSec: 3);
                    XCord = GetCordinate(X?.Result?.ToString());
                    YCord = GetCordinate(Y?.Result?.ToString());
                    await BrowserWindow.MouseClickAsync(XCord + 10, YCord + 3, delayAfter: 3);
                    // Enter password
                    await BrowserWindow.EnterCharsAsync(_account.AccountBaseModel.Password, 0, delayAtLast: 2);

                    // Get Loaded PageSource
                    var updatedHtml = await BrowserWindow.GetPageSourceAsync();
                    var require = updatedHtml.Contains("choice_1") && updatedHtml.Contains("choice_0");
                    if (!require && !updatedHtml.Contains("Submit") || updatedHtml.Contains("type=\"submit\"")
                        ||NewUI)
                    {
                        await BrowserWindow.ExecuteScriptAsync(NewUI ? "[...document.querySelectorAll('div[role=\"button\"]')].find(x=>x.innerText===\"Log in\").click();" : "document.querySelector('button[type=\"submit\"]').click();", delayInSec: 10);
                        await BrowserWindow.ExecuteScriptAsync("document.querySelector('div[aria-label=\"Dismiss\"]').click();", delayInSec: 6);
                        //await BrowserWindow.BrowserActAsync(ActType.Click, AttributeType.ClassName, "_acan _acap _acas", delayAfter: 10, index: 0);
                    }
                }
                else if(pageSource.Contains("aria-label=\"Continue\""))
                {
                    var result = await BrowserWindow.ExecuteScriptAsync("document.querySelector('div[aria-label=\"Continue\"]').click();", delayInSec: 5);
                    if(result != null && result.Success)
                    {
                        NewUI = true;
                        var script = NewUI ? "document.querySelector('input[type=\"{0}\"]').getBoundingClientRect().{1};" : "document.querySelector('input[name=\"{0}\"]').getBoundingClientRect().{1};";
                        var X = await BrowserWindow.ExecuteScriptAsync(string.Format(script, "password", "x"), delayInSec: 3);
                        var Y = await BrowserWindow.ExecuteScriptAsync(string.Format(script, "password", "y"), delayInSec: 3);
                        var XCord = GetCordinate(X?.Result?.ToString());
                        var YCord = GetCordinate(Y?.Result?.ToString());
                        await BrowserWindow.MouseClickAsync(XCord + 10, YCord + 3, delayAfter: 3);
                        await BrowserWindow.EnterCharsAsync(_account.AccountBaseModel.Password,0, delayAtLast: 3);
                        await Task.Delay(TimeSpan.FromSeconds(2));
                        await BrowserWindow.ExecuteScriptAsync(NewUI ? "[...document.querySelectorAll('div[role=\"button\"]')].find(x=>x.innerText===\"Log in\").click();" : "document.querySelector('button[type=\"submit\"]').click();", delayInSec: 10);
                        await BrowserWindow.ExecuteScriptAsync("document.querySelector('div[aria-label=\"Dismiss\"]').click();", delayInSec: 6);
                    }
                }
                pageSource = await BrowserWindow.GetPageSourceAsync();
                var currentUrl = BrowserWindow.CurrentUrl();
                if (currentUrl.Contains("/accounts/suspended/"))
                {
                    account.AccountBaseModel.Status = AccountStatus.ProfileSuspended;
                    account.IsUserLoggedIn = false;
                    BrowserWindow.IsLoggedIn = false;
                    isverification = true;
                    SaveAccountStatus(account);
                    return;
                }
                if (pageSource != null &&
                    (pageSource.Contains("Enter the code we sent to your number ending in")
                    || pageSource.Contains("Enter the code we sent via")
                    || pageSource.Contains("login code generated by")
                    || currentUrl.Contains("/challenge/action")
                    || currentUrl.Contains("/challenge")
                    || currentUrl.Contains("/auth_platform/codeentry/")))
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, account.AccountBaseModel.AccountNetwork, account.AccountBaseModel.UserName, "Login", "Socinator ! I am waiting for 2FA Code Submit,Please Check your 2FA device and submit code.");
                    await WaitForTwoFactorCodeSubmit();
                    GlobusLogHelper.log.Info(Log.CustomMessage, account.AccountBaseModel.AccountNetwork, account.AccountBaseModel.UserName, "Login", "Socinator ! Waiting time is over.Sorry ! if you not able to submit code please try again later.");
                    pageSource = await BrowserWindow.GetPageSourceAsync();
                }
                if (pageSource != null && (pageSource.Contains("Save your login info?")))
                {
                    await BrowserWindow.ExecuteScriptAsync("[...document.querySelectorAll('button[type=\"button\"]')].find(x=>x.innerText==\"Save info\").click();", delayInSec: 3);
                }
                if (pageSource != null && pageSource.Contains("Send Security Code"))
                {
                    account.AccountBaseModel.Status = AccountStatus.NeedsVerification;
                    account.IsUserLoggedIn = true;
                    BrowserWindow.IsLoggedIn = true;
                    isverification = true;
                    SaveAccountStatus(account);
                    return;
                }
                if (pageSource != null && pageSource.Contains("Sorry, your password was incorrect. Please double-check your password"))
                {
                    account.AccountBaseModel.Status = AccountStatus.InvalidCredentials;
                    account.IsUserLoggedIn = false;
                    BrowserWindow.IsLoggedIn = false;
                    IsWrongPassword = true;
                    GlobusLogHelper.log.Info(Log.CustomMessage, account.AccountBaseModel.AccountNetwork, account.AccountBaseModel.UserName, "Login", "Invalid Credential Please Check Username Or Password.");
                    SaveAccountStatus(account);
                }
                var count = 0;
            TryAgain:
                pageSource = await BrowserWindow.GetPageSourceAsync();
                var text = HtmlParseUtility.GetListInnerTextFromPartialTagNamecontains(pageSource, "span", "class", "x1lliihq x193iq5w x6ikm8r x10wlt62 xlyipyv xuxw1ft");
                while (count++ <= 4 && (text == null || text.Count == 0))
                {
                    BrowserWindow.Refresh();
                    await Task.Delay(TimeSpan.FromSeconds(10));
                    goto TryAgain;
                }
                var IsLoggedIn = text != null && text.Any(x => x.Contains("Profile") || x.Contains("profile"));
                if (pageSource != null && (pageSource.Contains("x193iq5w xeuugli x1fj9vlw x13faqbe x1vvkbs xt0psk2 x1i0vuye xvs91rp xo1l8bm x1roi4f4 x10wh9bi x1wdrske x8viiok x18hxmgj") || pageSource.Contains($"/{account.AccountBaseModel.UserName}/") || IsLoggedIn))
                {
                    await BrowserWindow.SaveCookies();
                    var headers = BrowserWindow.GetHeaders();
                    account.IsUserLoggedIn = true;
                    try
                    {
                        account.DeviceDetails.IGXClaim = headers.FirstOrDefault(x => x.Key == "x-ig-set-www-claim").Value;
                        if (string.IsNullOrEmpty(account.AccountBaseModel.ProfileId))
                        {
                            var ProfileData = await BrowserWindow.GetPageSourceAsync();
                            account.AccountBaseModel.ProfileId = Utilities.GetBetween(ProfileData, "\"username\":\"", "\"");
                        }
                    }
                    catch { }
                    BrowserWindow.IsLoggedIn = true;
                    isverification = false;
                    account.AccountBaseModel.Status = AccountStatus.Success;
                    SaveAccountStatus(account);
                }
            }
            catch
            {

            }
        }

        private void SaveAccountStatus(DominatorAccountModel account)
        {
            try
            {
                SocinatorAccountBuilder.SaveAccount(account);
            }
            catch { }
        }

        private async Task WaitForTwoFactorCodeSubmit()
        {
            var counter = 0;
            try
            {
                var page = await BrowserWindow.GetPageSourceAsync();
                var currentUrl = BrowserWindow.CurrentUrl();
                while (counter++ < 10 &&
                    (page.Contains("Enter the code we sent to your number ending in")
                    || page.Contains("Enter the code we sent via")
                    || page.Contains("login code generated by")
                    || currentUrl.Contains("/challenge/action")
                    || currentUrl.Contains("/challenge")
                    || currentUrl.Contains("/auth_platform/codeentry/")))
                {
                    await Task.Delay(TimeSpan.FromSeconds(10));
                    page = await BrowserWindow.GetPageSourceAsync();
                    currentUrl = BrowserWindow.CurrentUrl();
                }
            }
            catch { }
        }
        public async Task MobileLoginOnLoaded(DominatorAccountModel account, LoginType loginType, VerificationType verification = 0)
        {
            try
            {
                if (BrowserWindow.IsDisposed)
                    return;
                while (!BrowserWindow.IsLoaded)
                {
                    if (BrowserWindow.IsDisposed)
                        return;
                    await Task.Delay(1000);
                    continue;
                }
                await Task.Delay(10000);
                while (!BrowserWindow.IsLoggedIn && !isverification)
                {
                    if (BrowserWindow.IsDisposed)
                        return;
                    await Task.Delay(1000);
                    await InstaMobileLogin(account, loginType);
                    if (IsWrongPassword)
                        break;
                }

            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
        public void CloseBrowser(bool ClosePuppeteer = true)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    try
                    {
                        BrowserWindow.Close();
                        BrowserWindow.Dispose();
                    }
                    catch
                    {
                    }
                });
                if (PuppeteerBrowser != null && ClosePuppeteer)
                    PuppeteerBrowser.ClosedBrowser();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
        
        public SearchKeywordIgResponseHandler SearchForkeyword(DominatorAccountModel _dominatorAccount, string keyword, CancellationToken token)
        {
            bool isRunning = true;
            string pageSource = string.Empty;
            List<string> sideList = new List<string>();
            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    BrowserWindow.ClearResources();
                    await BrowserWindow.GoToCustomUrl(Constants.gdHomeUrl, 5);
                    await BrowserWindow.ExecuteScriptAsync("[...document.querySelectorAll('a[href=\"#\"]')].find(x=>x.innerText===\"Search\").click();", delayInSec: 3);
                    await BrowserWindow.EnterCharsAsync($" {keyword}", typingDelay: 0.3, delayAtLast: 5);
                    token.ThrowIfCancellationRequested();
                    await Task.Delay(5000, token);
                    var ScrollClass = "x9f619 xjbqb8w x78zum5 x168nmei x13lgxp2 x5pf9jr xo71vjh xxbr6pl xbbxn1n xwib8y2 x1y1aw1k x1uhb9sk x1plvlek xryxfnj x1c4vz4f x2lah0s xdt5ytf xqjyukv x1qjc9v5 x1oa3qoh x1nhvcw1";
                    var count = 0;
                    var lastCount = 0;
                ScrollAgain:
                    var Nodes = HtmlParseUtility.GetListNodesFromClassName(BrowserWindow.GetPageSource(), ScrollClass);
                    var ScrollIndex = Nodes != null && Nodes.Count > 0 ? Nodes.Count - 1 : 0;
                    await BrowserWindow.ExecuteScriptAsync($"document.getElementsByClassName(\"{ScrollClass}\")[{ScrollIndex}].scrollIntoViewIfNeeded();", delayInSec: 3);
                    while (count++ <= 6)
                    {
                        if (lastCount == Nodes.Count)
                            break;
                        lastCount = Nodes.Count;
                        goto ScrollAgain;
                    }
                    var lst = await BrowserWindow.GetPaginationDataList("{\"data\":{\"xdt_api__v1__fbsearch__topsearch_connection\"", true);
                    if (lst.Count == 0)
                        lst = await BrowserWindow.GetPaginationDataList("{\"users\":[{\"position\"", true);
                    pageSource = JsonConvert.SerializeObject(lst);
                }
                catch (Exception ex)
                {
                    ex.DebugLog(ex.Message);
                }
                isRunning = false;
            });

            while (isRunning)
                Task.Delay(500, token).Wait();

            return new SearchKeywordIgResponseHandler(new ResponseParameter { Response = pageSource });
        }

        public FriendshipsResponse Follow(DominatorAccountModel _dominatorAccountModel, CancellationToken token, InstagramUser instagramUser, string mediaid = null)
        {
            bool isRunning = true;
            string pageSource = string.Empty;
            var FriendShipResponse = string.Empty;
            var SuggestedUsers = string.Empty;
            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    BrowserWindow?.ClearResources();
                    var url = Constants.gdHomeUrl + "/" + instagramUser.Username;
                    await BrowserWindow.GoToCustomUrl(url, 6);
                    FriendShipResponse = await BrowserWindow.GetPaginationData("{\"friendship_status\":{\"following", true);
                    await Task.Delay(2000, token);
                    BrowserWindow.ClearResources();
                    var Clicked = await BrowserWindow.ExecuteScriptAsync("document.getElementsByClassName('_acan _acap _acas _aj1-')[0].click();", delayInSec: 5);
                    if(Clicked is null || !Clicked.Success)
                        Clicked = await BrowserWindow.ExecuteScriptAsync("document.getElementsByClassName('_aswp _aswr _aswu _asw_ _asx2')[0].click();", delayInSec: 5);
                    pageSource = await BrowserWindow.GetPageSourceAsync();
                    if (!string.IsNullOrEmpty(pageSource) && pageSource.Contains(ConstantHelpDetails.ActivityLimitMessage))
                    {
                        pageSource = ConstantHelpDetails.ActivityLimitMessage;
                    }else if(instagramUser?.Username == _dominatorAccountModel?.AccountBaseModel?.UserName)
                    {
                        pageSource = "Can't Follow Self Account";
                    }
                    else
                        pageSource = await BrowserWindow.GetPaginationData("{\"data\":{\"xdt_create_friendship\"", true);
                    SuggestedUsers = await BrowserWindow.GetPaginationData("for (;;);{\"payload\":{\"payloads\"", true);
                    SuggestedUsers = !string.IsNullOrEmpty(SuggestedUsers) ? SuggestedUsers?.Replace("for (;;);", "") : SuggestedUsers;
                    BrowserWindow.ClearResources();
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
                isRunning = false;
            });

            while (isRunning)
                Task.Delay(500, token).Wait();

            return new FriendshipsResponse(new ResponseParameter { Response = pageSource }, FriendShipResponse, SuggestedUsers);
        }

        public SuggestedUsersIgResponseHandler GetSuggestedUsers(DominatorAccountModel _dominatorAccount, CancellationToken token)
        {
            bool isRunning = true;
            string pageSource = string.Empty;
            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    BrowserWindow.ClearResources();
                    var url = Constants.gdHomeUrl + "/explore/people";
                    int failedCount = 0;
                    do
                    {
                        await BrowserWindow.GoToCustomUrl(url, 10);
                        pageSource = await BrowserWindow.GetPaginationData("{\"groups\":[{\"type\":\"Recommended\",\"items\"", true);
                        if (string.IsNullOrEmpty(pageSource))
                            pageSource = await BrowserWindow.GetPaginationData("{\"more_available\":false,\"max_id\"", true);
                        if (string.IsNullOrEmpty(pageSource))
                            failedCount++;
                    } while (string.IsNullOrEmpty(pageSource) && failedCount < 5);
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
                isRunning = false;
            });

            while (isRunning)
                Task.Delay(500, token).Wait();

            return new SuggestedUsersIgResponseHandler(new ResponseParameter { Response = pageSource });
        }

        public HashTagFeedIgResponseHandler GetHashtagFeedForUserScraper(DominatorAccountModel _dominatorAccount, QueryInfo queryInfo, CancellationToken token)
        {
            bool isRunning = true;
            string pageSource = string.Empty;
            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    string hashtag = queryInfo.QueryValue.Contains("#") ? queryInfo.QueryValue.TrimStart('#') : queryInfo.QueryValue;
                    await BrowserWindow.GoToCustomUrl($"{Constants.gdHomeUrl}/explore/tags/{hashtag}/", 5);
                    var PostListClass = "_aagu";
                    token.ThrowIfCancellationRequested();
                    int count = 0;
                    do
                    {
                        await Task.Delay(2000, token);
                        pageSource = await BrowserWindow.GetPaginationData(",\"data\":{\"id\":", true);
                        pageSource = string.IsNullOrEmpty(pageSource) ? await BrowserWindow.GetPaginationData("\"media_grid\":{\"refinements\"", true) : pageSource;
                        if (!string.IsNullOrEmpty(pageSource))
                        {
                            break;
                        }
                    } while (++count < 5);
                    count = 0;
                    var lastCount = 0;
                    do
                    {
                        var source = await BrowserWindow.GetPageSourceAsync();
                        var Nodes = HtmlParseUtility.GetListInnerHtmlFromTagName(source, "div", "class", PostListClass);
                        var Index = Nodes != null && Nodes.Count > 0 ? Nodes.Count - 1 : 0;
                        await BrowserWindow.ExecuteScriptAsync($"document.getElementsByClassName('{PostListClass}')[{Index}].scrollIntoViewIfNeeded();", 5);
                        pageSource = await BrowserWindow.GetPaginationData(",\"data\":{\"id\":", true);
                        pageSource = string.IsNullOrEmpty(pageSource) ? await BrowserWindow.GetPaginationData("\"media_grid\":{\"refinements\"", true) : pageSource;
                        if (Nodes.Count == lastCount)
                            break;
                        lastCount = Nodes.Count;
                    } while (count++ < 10);
                    pageSource = await BrowserWindow.GetPaginationData(",\"data\":{\"id\":", true);
                    pageSource = string.IsNullOrEmpty(pageSource) ? await BrowserWindow.GetPaginationData("\"media_grid\":{\"refinements\"", true) : pageSource;
                    BrowserWindow.ClearResources();
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
                isRunning = false;
            });
            while (isRunning)
                Task.Delay(500, token).Wait();
            return new HashTagFeedIgResponseHandler(new ResponseParameter { Response = pageSource });
        }

        public MediaLikersIgResponseHandler GetMediaLikers(DominatorAccountModel dominatorAccountModel, string queryValue, CancellationToken token)
        {
            bool isRunning = true;
            string pageSource = string.Empty;
            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    await BrowserWindow.GoToCustomUrl(GdUtilities.CreatePostUrl(queryValue) + "liked_by", 5);
                    await Task.Delay(5000, token);
                    pageSource = await BrowserWindow.GetPaginationData("{\"users\":[{\"pk", true);
                    BrowserWindow.ClearResources();

                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
                isRunning = false;
            });

            while (isRunning)
                Task.Delay(500, token).Wait();

            return new MediaLikersIgResponseHandler(new ResponseParameter { Response = pageSource });
        }

        public MediaCommentsIgResponseHandler GetMediaComments(DominatorAccountModel dominatorAccountModel, string queryValue, CancellationToken token)
        {
            bool isRunning = true;
            string pageSource = string.Empty;
            var jsonResponse = string.Empty;
            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    BrowserWindow.ClearResources();
                    await BrowserWindow.GoToCustomUrl(queryValue, 5);
                    await Task.Delay(2000, token);
                    pageSource = await BrowserWindow.GetPageSourceAsync();
                    int count = 0;
                    var LastNodes = 0;
                    var ScrollClass = "_ap3a _aaco _aacw _aacx _aad7 _aade";
                    while (count < 25)
                    {
                        pageSource = await BrowserWindow.GetPageSourceAsync();
                        if (!string.IsNullOrEmpty(pageSource) && pageSource.Contains("View hidden comments"))
                            await BrowserWindow.ExecuteScriptAsync($"document.querySelector('svg+div>div[role=\"button\"]').click();", 3);
                        var Nodes = HtmlParseUtility.GetListNodesFromClassName(pageSource, ScrollClass);
                        var Index = Nodes != null && Nodes.Count > 0 ? Nodes.Count - 1 : 0;
                        await BrowserWindow.ExecuteScriptAsync($"document.getElementsByClassName('{ScrollClass}')[{Index}].scrollIntoView();", 5);
                        if (Nodes != null && Nodes.Count == LastNodes)
                            break;
                        LastNodes = Nodes != null && Nodes.Count > 0 ? Nodes.Count : 0;
                        count++;
                    }
                    jsonResponse = await GetJsonResponseForComment();
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
                isRunning = false;
            });
            while (isRunning)
                Task.Delay(500, token).Wait();
            return new MediaCommentsIgResponseHandler(new ResponseParameter { Response = jsonResponse });
        }

        private async Task<string> GetJsonResponseForComment()
        {
            var res = await BrowserWindow.GetPaginationDataList("\"data\":{\"xdt_api__v1__media__media_id__comments__connection", true);
            res = res == null || res.Count == 0 ? await BrowserWindow.GetPaginationDataList("{\"data\":{\"xdt_api__v1__media__media_id__comments__connection", true) : res;
            try
            {
                if (res.Count == 1 && res.Any(x => x.StartsWith("<!DOCTYPE html>")))
                {
                    var pageSource = await BrowserWindow.GetPageSourceAsync();
                    var json = await GetJsonFromPageSource(pageSource);
                    res[0] = handler.GetJTokenValue(handler.ParseJsonToJObject(json), "data", "xdt_api__v1__media__media_id__comments__connection");
                }
                else if (res.Count > 1)
                {
                    for (int i = 0; i < res.Count; i++)
                    {
                        if (res[i].StartsWith("<!DOCTYPE html>"))
                        {
                            var json = await GetJsonFromPageSource(res[i]);
                            res[i] = handler.GetJTokenValue(handler.ParseJsonToJObject(json), "data", "xdt_api__v1__media__media_id__comments__connection");
                        }
                        else
                        {
                            res[i] = handler.GetJTokenValue(handler.ParseJsonToJObject(res[i]), "data", "xdt_api__v1__media__media_id__comments__connection");
                        }
                    }
                }
                res.RemoveAll(x => x.StartsWith("<!DOCTYPE html>"));
            }
            catch
            {
            }
            return JsonConvert.SerializeObject(res);
        }

        private async Task<string> GetJsonFromPageSource(string pageSource)
        {

            try
            {
                var jsonPtn = @"\{(?:[^\{\}]|(?<o>\{)|(?<-o>\}))+(?(o)(?!))\}";
                var input = pageSource.Substring(pageSource.IndexOf("{\"data\":{\"xdt_api__v1__media__media_id__comments__connection"));
                Match match = Regex.Matches(input, jsonPtn, RegexOptions.Multiline | RegexOptions.IgnoreCase)[0];
                pageSource = match.Groups[0].Value;
            }
            catch { }
            return pageSource;
        }

        public FollowerAndFollowingIgResponseHandler GetUserFollowers(DominatorAccountModel dominatorAccountModel, string userId, CancellationToken token, bool IsCloseFriend = false, string maxID = "")
        {
            bool isRunning = true;
            string pageSource = string.Empty;
            bool hasMore = true;
            var listuser = new List<InstagramUser>();
            FollowerAndFollowingIgResponseHandler igResponseHandler = null;
            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    hasMore = true;
                    if (string.Equals(userId, dominatorAccountModel.AccountBaseModel.UserName) || userId.Contains("@"))
                    {
                        dominatorAccountModel = await CheckAndAssignUserName(dominatorAccountModel);
                        userId = dominatorAccountModel.AccountBaseModel.ProfileId;
                    }
                    BrowserWindow.ClearResources();
                    await BrowserWindow.GoToCustomUrl($"https://www.instagram.com/{userId}/followers/", 5);
                    var DialogResult = await BrowserWindow.ExecuteScriptAsync("document.querySelector('div[role=\"dialog\"]').innerText", delayInSec: 2);
                    if (DialogResult == null || !DialogResult.Success || string.IsNullOrEmpty(DialogResult.Result.ToString()))
                    {
                        await BrowserWindow.ExecuteScriptAsync($"document.querySelector('a[href=\"/{userId}/followers/\"]').click();", delayInSec: 2);
                    }
                    await Task.Delay(5000);
                    var userDataList = new List<string>();
                    var lastScrollCount = 0;
                    var userList = await BrowserWindow.GetPaginationDataList("{\"xdt_api__v1__friendships__followers__connection\"", true);
                    userList = userList == null || userList.Count == 0 ? await BrowserWindow.GetPaginationDataList("{\"users\":[{", true) : userList;
                    userDataList.AddRange(userList);
                    do
                    {
                        var Nodes = HtmlParseUtility.GetListNodesFromClassName(await BrowserWindow.GetPageSourceAsync(), "_aaco");
                        if (lastScrollCount == Nodes?.Count)
                            break;
                        lastScrollCount = Nodes.Count;
                        var Index = Nodes != null && Nodes.Count > 0 ? Nodes.Count - 1 : 0;
                        await BrowserWindow.BrowserActAsync(ActType.CustomActType,
                        AttributeType.ClassName,
                                                    "_aaco",
                                                    index: Index,
                                                    value: "scrollIntoViewIfNeeded();", delayAfter: 3);
                        userList = await BrowserWindow.GetPaginationDataList("{\"xdt_api__v1__friendships__followers__connection\"", true);
                        userList = userList == null || userList.Count == 0 ? await BrowserWindow.GetPaginationDataList("{\"users\":[{", true) : userList;
                        if (userList.Count > 0)
                        {
                            var lastItem = userList[userList.Count - 1];
                            var handle = new JsonHandler(lastItem);
                            maxID = handle.GetElementValue("next_max_id");
                            maxID = string.IsNullOrEmpty(maxID) ? handle.GetElementValue("data", "xdt_api__v1__friendships__followers__connection", "page_info", "end_cursor") : maxID;
                            hasMore = !string.IsNullOrEmpty(maxID);
                        }
                        else
                            hasMore = false;
                        userDataList.AddRange(userList);
                        pageSource = JsonConvert.SerializeObject(userDataList);
                        igResponseHandler = new FollowerAndFollowingIgResponseHandler(new ResponseParameter { Response = pageSource }, hasMoreResult: hasMore, IsCloseFriend: IsCloseFriend, maxID);
                        listuser.AddRange(igResponseHandler.UsersList);
                        BrowserWindow.ClearResources();
                    } while (hasMore && listuser.Count < 50);
                    BrowserWindow.ClearResources();
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
                isRunning = false;
            });
            while (isRunning)
                Task.Delay(500, token).Wait();
            igResponseHandler.UsersList = listuser;
            return igResponseHandler;
        }

        public FriendshipsResponse RemoveFollowers(DominatorAccountModel dominatorAccountModel, AccountModel accountModel, string username, CancellationToken token)
        {
            bool isRunning = true;
            string pageSource = string.Empty;
            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    var userName = dominatorAccountModel.UserName;
                    if (string.Equals(userName, dominatorAccountModel.AccountBaseModel.UserName) || userName.Contains("@"))
                    {
                        dominatorAccountModel = await CheckAndAssignUserName(dominatorAccountModel);
                        userName = dominatorAccountModel.AccountBaseModel.ProfileId;
                    }
                    var url = $"{Constants.gdHomeUrl}/{dominatorAccountModel.UserName}/followers";
                    url = $"{Constants.gdHomeUrl}/{userName}/followers";
                    await BrowserWindow.GoToCustomUrl(url, 5);
                    await BrowserWindow.PressAnyKeyUpdated(winKeyCode: 9);
                    await BrowserWindow.PressAnyKeyUpdated(winKeyCode: 9);
                    await BrowserWindow.EnterCharsAsync(" " + username, 0, delayAtLast: 1);
                    await Task.Delay(3000, token);
                    int count = await TryGetIndexByInnerText("x1pi30zi x1n2onr6 x2b8uid xlyipyv x87ps6o x14atkfc xcdnw81 x1gjpkn9 x5n08af xsz8vos", "Remove");

                    await BrowserWindow.BrowserActAsync(ActType.Click,
                                                    AttributeType.ClassName,
                                                    "x1pi30zi x1n2onr6 x2b8uid xlyipyv x87ps6o x14atkfc xcdnw81 x1gjpkn9 x5n08af xsz8vos",
                                                    index: count,
                                                    delayAfter: 5);
                    await BrowserWindow.BrowserActAsync(ActType.Click,
                                                    AttributeType.ClassName,
                                                    "_a9-- _a9-_",
                                                    delayAfter: 5);
                    var res = await BrowserWindow.GetPaginationData("{\"status\":\"ok\"}", true);
                    if (res.Contains("{\"status\":\"ok\"}"))
                        pageSource = res;
                    BrowserWindow.ClearResources();
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
                isRunning = false;
            });

            while (isRunning)
                Task.Delay(500, token).Wait();

            return new FriendshipsResponse(new ResponseParameter { Response = pageSource });
        }

        public UserFriendshipResponse UserFriendship(DominatorAccountModel dominatorAccountModel, AccountModel accountModel, string userId, CancellationToken token)
        {
            bool isRunning = true;
            string pageSource = string.Empty;
            var frienshipRes = string.Empty;
            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    bool hasMore = true;
                    var userName = dominatorAccountModel.UserName;
                    if (string.Equals(userName, dominatorAccountModel.AccountBaseModel.UserName) || userName.Contains("@"))
                    {
                        dominatorAccountModel = await CheckAndAssignUserName(dominatorAccountModel);
                        userName = dominatorAccountModel.AccountBaseModel.ProfileId;
                    }
                    await BrowserWindow.GoToCustomUrl($"https://www.instagram.com/{userName}/followers/", 5);
                    var DialogResult = await BrowserWindow.ExecuteScriptAsync("document.querySelector('div[role=\"dialog\"]').innerText", delayInSec: 2);
                    if (DialogResult == null || !DialogResult.Success || string.IsNullOrEmpty(DialogResult.Result.ToString()))
                    {
                        await BrowserWindow.ExecuteScriptAsync($"document.querySelector('a[href=\"/{userName}/followers/\"]').click();", delayInSec: 2);
                    }
                    await Task.Delay(5000, token);
                    var userDataList = new List<string>();
                    int failedCount = 0;
                    do
                    {
                        var Nodes = HtmlParseUtility.GetListNodesFromClassName(await BrowserWindow.GetPageSourceAsync(), "_aaco");
                        var Index = Nodes != null && Nodes.Count > 0 ? Nodes.Count - 1 : 0;
                        await BrowserWindow.BrowserActAsync(ActType.CustomActType,
                                                    AttributeType.ClassName,
                                                    "_aaco",
                                                    index: Index,
                                                    value: "scrollIntoViewIfNeeded();", delayAfter: 2);
                        var userList = await BrowserWindow.GetPaginationDataList("{\"friendship_statuses\":", true);
                        frienshipRes = userList.FirstOrDefault(x => x.Contains(userId))?.ToString();
                        if (!string.IsNullOrEmpty(frienshipRes))
                            hasMore = false;
                        else
                        {
                            failedCount++;
                            continue;
                        }

                        BrowserWindow.ClearResources();
                        userDataList.Add(frienshipRes);
                    } while (hasMore && failedCount < 10);

                    if (userDataList.Count == 0)
                    {
                        await BrowserWindow.GoToCustomUrl($"https://www.instagram.com/{userName}/following/", 5);
                        await Task.Delay(5000, token);
                        failedCount = 0;
                        do
                        {
                            var Nodes = HtmlParseUtility.GetListNodesFromClassName(await BrowserWindow.GetPageSourceAsync(), "_aaco");
                            var Index = Nodes != null && Nodes.Count > 0 ? Nodes.Count - 1 : 0;
                            await BrowserWindow.BrowserActAsync(ActType.CustomActType,
                                                        AttributeType.ClassName,
                                                        "_aaco",
                                                        index: Index,
                                                        value: "scrollIntoViewIfNeeded();", delayAfter: 2);
                            var userList = await BrowserWindow.GetPaginationDataList("{\"friendship_statuses\":", true);
                            frienshipRes = userList.FirstOrDefault(x => x.Contains(userId))?.ToString();
                            if (!string.IsNullOrEmpty(frienshipRes))
                                hasMore = false;
                            else
                            {
                                failedCount++;
                                continue;
                            }

                            BrowserWindow.ClearResources();
                            userDataList.Add(frienshipRes);
                        } while (hasMore && failedCount < 10);
                    }

                    pageSource = userDataList.FirstOrDefault();
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
                isRunning = false;
            });
            while (isRunning)
                Task.Delay(15000, token).Wait();
            return new UserFriendshipResponse(new ResponseParameter { Response = pageSource }, userId);
        }

        public FriendshipsResponse Unfollow(DominatorAccountModel dominatorAccountModel, AccountModel accountModel, CancellationToken token, InstagramUser instagramUser)
        {
            bool isRunning = true;
            string pageSource = string.Empty;
            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    if (instagramUser.Username.StartsWith("/") || instagramUser.Username.EndsWith("/"))
                        instagramUser.Username = instagramUser.Username.Trim('/');
                    var userName = dominatorAccountModel.UserName;
                    if (string.Equals(userName, dominatorAccountModel.AccountBaseModel.UserName) || userName.Contains("@"))
                    {
                        dominatorAccountModel = await CheckAndAssignUserName(dominatorAccountModel);
                        userName = dominatorAccountModel.AccountBaseModel.ProfileId;
                    }
                    var url = $"{Constants.gdHomeUrl}/{dominatorAccountModel.UserName}/following/";
                    url = $"{Constants.gdHomeUrl}/{userName}/following";
                    await BrowserWindow.GoToCustomUrl(url, 5);
                    var DialogResult = await BrowserWindow.ExecuteScriptAsync("document.querySelector('div[role=\"dialog\"]').innerText", delayInSec: 2);
                    if (DialogResult == null || !DialogResult.Success || string.IsNullOrEmpty(DialogResult.Result.ToString()))
                    {
                        await BrowserWindow.ExecuteScriptAsync($"document.querySelector('a[href=\"/{userName}/following/\"]').click();", delayInSec: 2);
                    }
                    string uname = instagramUser.Username.Length > 20 ? instagramUser.Username.Substring(0, 20) : instagramUser.Username;
                    var X = await BrowserWindow.ExecuteScriptAsync("document.querySelector('input[aria-label=\"Search input\" i]').getBoundingClientRect().x", delayInSec: 2);
                    var Y = await BrowserWindow.ExecuteScriptAsync("document.querySelector('input[aria-label=\"Search input\" i]').getBoundingClientRect().y", delayInSec: 2);
                    await BrowserWindow.MouseClickAsync(GetCordinate(X.Result.ToString()) + 10, GetCordinate(Y.Result.ToString()) + 5, delayAfter: 3);
                    await BrowserWindow.EnterCharsAsync(" " + uname, 0, delayAtLast: 1);
                    await Task.Delay(3000, token);
                    int count = await TryGetIndexByInnerText("x1q0g3np xqjyukv x6s0dn4 x1oa3qoh xl56j7k", "Following");

                    await BrowserWindow.BrowserActAsync(ActType.Click,
                                                    AttributeType.ClassName,
                                                    "x1q0g3np xqjyukv x6s0dn4 x1oa3qoh xl56j7k",
                                                    index: count,
                                                    delayAfter: 5);
                    BrowserWindow.ClearResources();
                    await BrowserWindow.BrowserActAsync(ActType.Click,
                                                    AttributeType.ClassName,
                                                    "_a9-- _a9-_",
                                                    delayAfter: 5);
                    var res = await BrowserWindow.GetPaginationData("{\"status\":\"ok\"}", true);
                    res = string.IsNullOrEmpty(res) ? await BrowserWindow.GetPaginationData("{\"friendship_status\":{\"following\"", true) : res;
                    res = string.IsNullOrEmpty(res) ? await BrowserWindow.GetPaginationData("{\"friendship_status\"", true) : res;
                    if (res.Contains("{\"status\":\"ok\"}") || res.Contains("{\"friendship_status\":{\"following\"")
                    || res.Contains("{\"friendship_status\""))
                        pageSource = res;
                    BrowserWindow.ClearResources();
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
                isRunning = false;
            });

            while (isRunning)
                Task.Delay(500, token).Wait();

            return new FriendshipsResponse(new ResponseParameter { Response = pageSource });
        }
        private int GetCordinate(string cord)
        {
            try
            {
                var deciMal = Convert.ToDecimal(cord);
                return Convert.ToInt32(deciMal);
            }
            catch (Exception)
            {
                int.TryParse(cord, out int cordinate);
                return cordinate;
            }
        }
        public UsernameInfoIgResponseHandler GetUserInfo(DominatorAccountModel dominatorAccountModel, string username, CancellationToken token)
        {
            bool isRunning = true;
            string pageSource = string.Empty;
            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    BrowserWindow.ClearResources();
                    if (string.Equals(username, dominatorAccountModel.AccountBaseModel.UserName) || username.Contains("@"))
                    {
                        dominatorAccountModel = await CheckAndAssignUserName(dominatorAccountModel);
                        username = dominatorAccountModel.AccountBaseModel.ProfileId;
                    }
                    await VisitPage(dominatorAccountModel, $"https://www.instagram.com/{username}");
                    pageSource = await BrowserWindow.GetPaginationData("{\"data\":{\"user\":{\"ai_agent_type\":null,\"biography\"", true);
                    if (string.IsNullOrEmpty(pageSource))
                        pageSource = await BrowserWindow.GetPaginationData("{\"data\":{\"user\":{\"ai_agent_type\":null,\"biography\"", true);
                    if (string.IsNullOrEmpty(pageSource))
                        pageSource = await BrowserWindow.GetPaginationData("{\"data\":{\"user\":{\"friendship_status\"", true);
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
                isRunning = false;
            });

            while (isRunning)
                Task.Delay(500, token).Wait();

            return new UsernameInfoIgResponseHandler(new ResponseParameter { Response = pageSource });
        }

        public FollowerAndFollowingIgResponseHandler GetUserFollowings(DominatorAccountModel dominatorAccountModel, string userId, CancellationToken token)
        {
            bool isRunning = true;
            string pageSource = string.Empty;
            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    bool hasMore = true;
                    if (string.Equals(userId, dominatorAccountModel.AccountBaseModel.UserName) || userId.Contains("@"))
                    {
                        dominatorAccountModel = await CheckAndAssignUserName(dominatorAccountModel);
                        userId = dominatorAccountModel.AccountBaseModel.ProfileId;
                    }
                    var url = $"https://www.instagram.com/{userId.Trim()}/following/";
                    await BrowserWindow.GoToCustomUrl(url, 5);
                    var DialogResult = await BrowserWindow.ExecuteScriptAsync("document.querySelector('div[role=\"dialog\"]').innerText", delayInSec: 2);
                    if (DialogResult == null || !DialogResult.Success || string.IsNullOrEmpty(DialogResult.Result.ToString()))
                    {
                        await BrowserWindow.ExecuteScriptAsync($"document.querySelector('a[href=\"/{userId}/following/\"]').click();", delayInSec: 2);
                    }
                    await Task.Delay(5000);
                    var userDataList = new List<string>();
                    var lastScrollCount = 0;
                    var userList = await BrowserWindow.GetPaginationDataList("{\"xdt_api__v1__friendships__following__connection\"", true);
                    userList = userList == null || userList.Count == 0 ? await BrowserWindow.GetPaginationDataList("{\"users\":[{", true) : userList;
                    userDataList.AddRange(userList);
                    do
                    {
                        BrowserWindow.ClearResources();
                        var Nodes = HtmlParseUtility.GetListNodesFromClassName(await BrowserWindow.GetPageSourceAsync(), "_aaco");
                        if (lastScrollCount == Nodes?.Count)
                            break;
                        lastScrollCount = Nodes.Count;
                        var Index = Nodes != null && Nodes.Count > 0 ? Nodes.Count - 1 : 0;
                        await BrowserWindow.BrowserActAsync(ActType.CustomActType,
                                                    AttributeType.ClassName,
                                                    "_aaco",
                                                    index: Index,
                                                    value: "scrollIntoViewIfNeeded();", delayAfter: 3);
                        userList = await BrowserWindow.GetPaginationDataList("{\"xdt_api__v1__friendships__following__connection\"", true);
                        userList = userList == null || userList.Count == 0 ? await BrowserWindow.GetPaginationDataList("{\"users\":[{", true) : userList;
                        if (userList.Count > 0)
                        {
                            var lastItem = userList[userList.Count - 1];
                            var handle = new JsonHandler(lastItem);
                            var next_max_id = handle.GetElementValue("next_max_id");
                            next_max_id = string.IsNullOrEmpty(next_max_id) ? handle.GetElementValue("data", "xdt_api__v1__friendships__following__connection", "page_info", "end_cursor") : next_max_id;
                            if (string.IsNullOrEmpty(next_max_id))
                                hasMore = false;
                        }
                        else
                            hasMore = false;
                        userDataList.AddRange(userList);
                    } while (hasMore);
                    BrowserWindow.ClearResources();
                    pageSource = JsonConvert.SerializeObject(userDataList);
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
                isRunning = false;
            });
            while (isRunning)
                Task.Delay(500, token).Wait();
            return new FollowerAndFollowingIgResponseHandler(new ResponseParameter { Response = pageSource });
        }

        public UserFeedIgResponseHandler GetUserFeed(DominatorAccountModel objDominatorAccountModel, string userName, CancellationToken token,int MaxCount = 0)
        {
            bool isRunning = true;
            string pageSource = string.Empty;
            var posts = new List<InstagramPost>();
            bool hasMore = true;
            var responseHandler = new UserFeedIgResponseHandler(new ResponseParameter { Response = string.Empty });
            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    if (userName.StartsWith("/"))
                        userName = userName.Trim('/');
                    if (string.Equals(userName, objDominatorAccountModel.AccountBaseModel.UserName) || userName.Contains("@"))
                    {
                        objDominatorAccountModel = await CheckAndAssignUserName(objDominatorAccountModel);
                        userName = objDominatorAccountModel.AccountBaseModel.ProfileId;
                    }
                    BrowserWindow.ClearResources();
                    await BrowserWindow.GoToCustomUrl($"https://www.instagram.com/{userName}/", 5);
                    await Task.Delay(5000);
                    pageSource = await BrowserWindow.GetPageSourceAsync();
                    if (pageSource.Contains("This Account is Private") || pageSource.Contains("Sorry, this page isn't available"))
                    {
                        isRunning = false;
                        return;
                    }
                    do
                    {
                        if (MaxCount > 0 ? posts.Count >= MaxCount : posts.Count > 150)
                            break;
                        if (hasMore)
                            await BrowserWindow.BrowserActAsync(ActType.CustomActType,
                                                    AttributeType.ClassName,
                                                    "_9dls js-focus-visible",
                                                    value: "scrollBy(10000,10000)", delayAfter: 5);
                        var JsonResponse = await GetListJsonResponse();
                        var userList = JsonResponse.Item3;
                        hasMore = JsonResponse.Item1;
                        BrowserWindow.ClearResources();
                        responseHandler = new UserFeedIgResponseHandler(new ResponseParameter { Response = JsonConvert.SerializeObject(userList) });
                        posts.AddRange(responseHandler.Items);
                    } while (hasMore);
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
                isRunning = false;
            });
            while (isRunning)
                Task.Delay(500, token).Wait();
            responseHandler.HasMoreResults = hasMore;
            responseHandler.Items = posts;
            return responseHandler;
        }

        public FriendshipsResponse Block(DominatorAccountModel dominatorAccountModel, CancellationToken token, InstagramUser instagramUser)
        {
            bool isRunning = true;
            string pageSource = string.Empty;
            List<string> optionList = new List<string>();
            int index = 0;
            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    if (instagramUser.Username.StartsWith("/") || instagramUser.Username.EndsWith("/"))
                        instagramUser.Username = instagramUser.Username.Trim('/');
                    var url = $"{Constants.gdHomeUrl}/{instagramUser.Username}";
                    await BrowserWindow.GoToCustomUrl(url, 5);
                    await Task.Delay(6000, token);

                    await BrowserWindow.BrowserActAsync(ActType.Click,
                                                    AttributeType.ClassName,
                                                    "x78zum5 xl56j7k x1y1aw1k x1sxyh0 xwib8y2 xurb0ha xcdnw81",
                                                    delayAfter: 5);
                    var blockFollowerClass = "x52vrxo x4gyw5p xkmlbd1 x1xlr1w8";
                    optionList = await BrowserWindow.GetListInnerHtml(ActType.GetValue, AttributeType.ClassName, blockFollowerClass);
                    optionList.Reverse();
                    index = optionList.IndexOf(optionList.FirstOrDefault(x => x.Contains("Block")));
                    await BrowserWindow.BrowserActAsync(ActType.Click,
                                                    AttributeType.ClassName,
                                                    blockFollowerClass,
                                                    index: index,
                                                    delayAfter: 5);
                    await BrowserWindow.BrowserActAsync(ActType.Click,
                                                    AttributeType.ClassName,
                                                    blockFollowerClass,
                                                    index: 3,
                                                    delayAfter: 5);
                    BrowserWindow.ClearResources();
                    var dismissClass = "xurb0ha x2b8uid x87ps6o xxymvpz xh8yej3 x52vrxo x4gyw5p x5n08af";
                    var node = HtmlParseUtility.GetListNodesFromClassName(BrowserWindow.GetPageSource(), dismissClass);
                    var dismissIndex = node != null && node.Count > 0 ? node.IndexOf(node.FirstOrDefault(x => x.InnerText?.ToLower() == "dismiss")) : 4;
                    await BrowserWindow.BrowserActAsync(ActType.Click,
                                                    AttributeType.ClassName,
                                                    dismissClass,
                                                    index: dismissIndex,
                                                    delayAfter: 8);
                    pageSource = await BrowserWindow.GetPaginationData("{\"data\":{\"user\":{\"biography\":\"", true);
                    pageSource = string.IsNullOrEmpty(pageSource) ? await BrowserWindow.GetPaginationData("{\"data\":{\"user\":{\"ai_agent_type\":", true) : pageSource;
                    pageSource = string.IsNullOrEmpty(pageSource) ? await BrowserWindow.GetPaginationData("{\"data\":{\"user\":{\"friendship_status\":", true) : pageSource;
                    BrowserWindow.ClearResources();
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
                isRunning = false;
            });

            while (isRunning)
                Task.Delay(500, token).Wait();

            return new FriendshipsResponse(new ResponseParameter { Response = pageSource });
        }

        public LikeResponse Like(DominatorAccountModel dominatorAccountModel, AccountModel accountModel, string codeUrl, CancellationToken token)
        {
            bool isRunning = true;
            string pageSource = string.Empty;
            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    var url = Constants.gdHomeUrl + $"/p/{codeUrl}";
                    BrowserWindow.ClearResources();
                    await BrowserWindow.GoToCustomUrl(url, 5);
                    pageSource = await BrowserWindow.GetPageSourceAsync();
                    await Task.Delay(2000, token);
                    int count = 0;
                    do
                    {
                        var Clicked = await BrowserWindow.ExecuteScriptAsync("document.getElementsByClassName('x1y1aw1k xf159sx xwib8y2 xmzvs34 xcdnw81')[0].click();", delayInSec: 3);
                        pageSource = await BrowserWindow.GetPaginationData("{\"data\":{\"xdt_api__v1__media__media_id__like\"", true);
                        pageSource = string.IsNullOrEmpty(pageSource) ? await BrowserWindow.GetPaginationData("{\"data\":{\"xdt_mark_media_like\"", true) : pageSource;
                        count++;
                    } while (count < 5 && string.IsNullOrEmpty(pageSource));
                    BrowserWindow.ClearResources();
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
                isRunning = false;
            });

            while (isRunning)
                Task.Delay(500, token).Wait();

            return new LikeResponse(new ResponseParameter { Response = pageSource });
        }

        public CommentResponse Comment(DominatorAccountModel dominatorAccountModel, AccountModel accountModel, string codeUrl, string commentText, CancellationToken token)
        {
            bool isRunning = true;
            string pageSource = string.Empty;
            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    var url = Constants.gdHomeUrl + $"/p/{codeUrl}";
                    await BrowserWindow.GoToCustomUrl(url, 5);
                    var xAndy = await BrowserWindow.GetXAndYAsync(AttributeType.ClassName, "xyri2b x18d9i69 x1c1uobl xtt52l0 xnalus7 xs3hnx8 x1bq4at4 xaqnwrm");
                    await BrowserWindow.MouseClickAsync(xAndy.Key, xAndy.Value, delayAfter: 3);
                    await Task.Delay(2000, token);
                    var comments = commentText?.Replace("\r", "").Split('\n');
                    if (comments.Length > 1)
                    {
                        foreach (var comment in comments)
                        {
                            await BrowserWindow.EnterCharsAsync(comment, 0, delayAtLast: 3);
                            if (comments.LastOrDefault() != comment)
                                await BrowserWindow.SendKeyEventAsCharacter();
                        }
                    }
                    else
                        await BrowserWindow.EnterCharsAsync(commentText, 0, delayAtLast: 3);
                    var failedCount = 0;
                    do
                    {
                        pageSource = await BrowserWindow.GetPageSourceAsync();
                        var Clicked = await BrowserWindow.ExecuteScriptAsync("document.getElementsByClassName('xkhd6sd x1n2onr6 x1n5bzlp x173jzuc x1yc6y37')[0].click();", delayInSec: 5);
                        if(Clicked is null || !Clicked.Success)
                            Clicked = await BrowserWindow.ExecuteScriptAsync("[...document.querySelectorAll('div[role=\"button\"]')].find(x=>x.innerText===\"Post\").click();", delayInSec: 5);
                        await Task.Delay(2000, token);
                        pageSource = await BrowserWindow.GetPaginationData("{\"data\":{\"xdt_web__comments__media_id__add_queryable\"", true);
                    } while (failedCount++ <= 5 && string.IsNullOrEmpty(pageSource));


                    BrowserWindow.ClearResources();
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
                isRunning = false;
            });

            while (isRunning)
                Task.Delay(500, token).Wait();

            return new CommentResponse(new ResponseParameter { Response = pageSource });
        }

        public MediaInfoIgResponseHandler MediaInfo(DominatorAccountModel dominatorAccountModel, string postUrl, CancellationToken token)
        {
            bool isRunning = true;
            string pageSource = string.Empty;
            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    var url = postUrl.Contains(Constants.gdHomeUrl) ? postUrl?.Replace("/reels/", "/reel/") : Constants.gdHomeUrl + $"/p/{postUrl}";
                    await BrowserWindow.GoToCustomUrl(url, 5);
                    await Task.Delay(2000, token);
                    pageSource = await BrowserWindow.GetPageSourceAsync();
                    string jsonPtn = @"\{(?:[^\{\}]|(?<o>\{)|(?<-o>\}))+(?(o)(?!))\}";
                    string input = pageSource.Substring(pageSource.IndexOf("{\"items\":[{\"code\":"));
                    Match match = Regex.Matches(input, jsonPtn, RegexOptions.Multiline | RegexOptions.IgnoreCase)[0];
                    pageSource = match.Groups[0].Value;
                    BrowserWindow.ClearResources();
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
                isRunning = false;
            });

            while (isRunning)
                Task.Delay(500, token).Wait();

            return new MediaInfoIgResponseHandler(new ResponseParameter { Response = pageSource });
        }

        public CommonIgResponseHandler LikePostComment(DominatorAccountModel dominatorAccountModel, string PostUrl, CancellationToken token)
        {
            bool isRunning = true;
            string pageSource = string.Empty;
            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    BrowserWindow.ClearResources();
                    await BrowserWindow.GoToCustomUrl(PostUrl, 5);
                    await BrowserWindow.ExecuteScriptAsync("[...document.querySelectorAll('div[role=\"button\"]>div>span>svg[aria-label=\"Like\"]')][1].parentNode.parentNode.click();", delayInSec: 5);
                    pageSource = await BrowserWindow.GetPaginationData("{\"status\":\"ok\"}", true);
                    BrowserWindow.ClearResources();
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
                isRunning = false;
            });

            while (isRunning)
                Task.Delay(500, token).Wait();

            return new CommonIgResponseHandler(new ResponseParameter { Response = pageSource });
        }

        public UserFeedResponse GetLikedMedia(DominatorAccountModel dominatorAccountModel, CancellationToken token)
        {
            bool isRunning = true;
            string pageSource = string.Empty;
            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    BrowserWindow.ClearResources();
                    bool hasMore = true;
                    await BrowserWindow.GoToCustomUrl($"{Constants.gdHomeUrl}/your_activity/interactions/likes/", 5);
                    await Task.Delay(5000, token);
                    var userDataList = new List<string>();
                    do
                    {
                        if (userDataList.Count > 25)
                            break;

                        await BrowserWindow.BrowserActAsync(ActType.ScrollByQuery,
                                                    AttributeType.DataBlocksName,
                                                    "bk.components.Collection",
                                                    value: "10000,10000",
                                                    delayAfter: 2);
                        var userList = await BrowserWindow.GetPaginationDataList("for (;;);{\"__ar\":1,\"payload\":{\"layout\":{\"bloks_payload", true);
                        userList = userList is null || userList.Count == 0 ? await BrowserWindow.GetPaginationDataList("LikedScreen", true):userList;
                        if (userList.Count > 0)
                        {
                            var lastItem = userList[userList.Count - 1];
                            if (!lastItem.Contains("initial_cursor"))
                                hasMore = false;
                        }
                        else
                        {
                            hasMore = false;
                        }
                        BrowserWindow.ClearResources();
                        if (userList.Count > 0)
                        {
                            for (int i = 0; i < userList.Count; i++)
                            {
                                var item = userList[i].Replace("for (;;);", "");
                                var hand = new JsonHandler(item);
                                userList[i] = hand.GetElementValue("payload", "layout", "bloks_payload");
                            }
                        }
                        userDataList.AddRange(userList);
                    } while (hasMore);

                    pageSource = JsonConvert.SerializeObject(userDataList);
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
                isRunning = false;
            });
            while (isRunning)
                Task.Delay(500, token).Wait();
            return new UserFeedResponse(new ResponseParameter { Response = pageSource });
        }

        public CommonIgResponseHandler UnlikeMedia(DominatorAccountModel dominatorAccountModel, string code, CancellationToken token)
        {
            bool isRunning = true;
            string pageSource = string.Empty;
            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    var url = $"{Constants.gdHomeUrl}/p/{code}";
                    await BrowserWindow.GoToCustomUrl(url, 5);
                    await Task.Delay(2000, token);
                    await BrowserWindow.BrowserActAsync(ActType.Click,
                                                    AttributeType.ClassName,
                                                    "x1y1aw1k xf159sx xwib8y2 xmzvs34 xcdnw81",
                                                    index: 0,
                                                    delayAfter: 5);
                    pageSource = await BrowserWindow.GetPaginationData("{\"data\":{\"xdt_api__v1__media__media_id__unlike\"", true);
                    BrowserWindow.ClearResources();
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
                isRunning = false;
            });

            while (isRunning)
                Task.Delay(500, token).Wait();

            return new CommonIgResponseHandler(new ResponseParameter { Response = pageSource });
        }

        public CommonIgResponseHandler ReplyComment(DominatorAccountModel dominatorAccountModel, string PostUrl, string comments, CancellationToken token)
        {
            bool isRunning = true;
            string pageSource = string.Empty;
            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    await BrowserWindow.GoToCustomUrl(PostUrl, 5);
                    await Task.Delay(5000, token);
                    var dataList = await BrowserWindow.GetListInnerHtml(ActType.GetValue, AttributeType.ClassName, "_a9ze", ValueTypes.InnerText);
                    dataList.Reverse();
                    int index = dataList.IndexOf(dataList.FirstOrDefault(x => x.Contains("Reply")));
                    await BrowserWindow.BrowserActAsync(ActType.Click,
                                                    AttributeType.ClassName,
                                                    "_a9ze",
                                                    index: index,
                                                    delayAfter: 5);
                    await BrowserWindow.CopyPasteContentAsync(comments, 86, delay: 1, delayAtLast: 5);
                    BrowserWindow.ClearResources();
                    var PostButtonClass = "x18d9i69 xkhd6sd x1n2onr6 x1n5bzlp x173jzuc x1yc6y37";
                    var Nodes = HtmlParseUtility.GetListNodesFromClassName(BrowserWindow.GetPageSource(), PostButtonClass);
                    var PostClickIndex = Nodes != null && Nodes.Count > 0 && Nodes.Any(x => x.InnerText.Contains("Post")) ? Nodes.IndexOf(Nodes.FirstOrDefault(x => x.InnerText.Contains("Post"))) : 0;
                    PostClickIndex = PostClickIndex < 0 ? 0 : PostClickIndex;
                    await BrowserWindow.BrowserActAsync(ActType.Click,
                                                    AttributeType.ClassName,
                                                    PostButtonClass,
                                                    index: PostClickIndex,
                                                    delayAfter: 5);
                    pageSource = await BrowserWindow.GetPaginationData("{\"id\":\"", true);
                    BrowserWindow.ClearResources();
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
                isRunning = false;
            });

            while (isRunning)
                Task.Delay(500, token).Wait();

            return new CommonIgResponseHandler(new ResponseParameter { Response = pageSource });
        }

        public CommonIgResponseHandler SeenUserStory(DominatorAccountModel dominatorAccountModel, InstagramUser instagramUser, CancellationToken token)
        {
            bool isRunning = true;
            string pageSource = string.Empty;
            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    foreach (var item in instagramUser.UserStories)
                    {
                        await BrowserWindow.GoToCustomUrl($"https://www.instagram.com/stories/{instagramUser.Username}/{item.PostId}", 3);
                        await BrowserWindow.ExecuteScriptAsync("[...document.querySelectorAll('div[role=\"button\"]')].find(x=>x.innerText==\"View story\").click();", delayInSec: 3);
                        await Task.Delay(2000, token);
                    }
                    pageSource = await BrowserWindow.GetPaginationData("\"xdt_api__v1__stories__reel__seen\"", true);
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
                isRunning = false;
            });

            while (isRunning)
                Task.Delay(500, token).Wait();

            return new CommonIgResponseHandler(new ResponseParameter { Response = pageSource });
        }

        public PostStoryResponse GetStoriesUser(DominatorAccountModel dominatorAccountModel, InstagramUser user, CancellationToken token)
        {
            bool isRunning = true;
            string pageSource = string.Empty;
            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    BrowserWindow.ClearResources();
                    await BrowserWindow.GoToCustomUrl($"https://www.instagram.com/stories/{user.Username}", 5);
                    await Task.Delay(4000, token);
                    await BrowserWindow.ExecuteScriptAsync("[...document.querySelectorAll('div[role=\"button\"]')].find(x=>x.innerText==\"View story\").click();", delayInSec: 3);
                    pageSource = await BrowserWindow.GetPageSourceAsync(2);
                    var splittedScripts = Regex.Split(pageSource, "<script type=\"application/json\"");
                    var finalScript = splittedScripts.FirstOrDefault(x => x.Contains("xdt_api__v1__feed__reels_media"));
                    pageSource = Regex.Match(finalScript, "data-sjs=\"\">(.*?)</script>").Groups[1].Value;
                    pageSource = string.IsNullOrEmpty(pageSource) ? Regex.Match(finalScript, "\">(.*?)</script>").Groups[1].Value : pageSource;

                    BrowserWindow.ClearResources();
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
                isRunning = false;
            });

            while (isRunning)
                Task.Delay(500, token).Wait();

            return new PostStoryResponse(new ResponseParameter { Response = pageSource });
        }

        public SendMessageIgResponseHandler SendMessage(DominatorAccountModel dominatorAccountModel, InstagramUser instagramUser, string threadId, string message, string mediaPath, CancellationToken token, List<string> Medias = null, bool SkipAlreadyReceivedMessage = false)
        {
            bool isRunning = true;
            string pageSource = string.Empty;
            var ErrorMessage = string.Empty;
            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    if (!string.IsNullOrEmpty(threadId))
                    {
                        var url = Constants.gdHomeUrl + $"/direct/t/{threadId}";
                        await BrowserWindow.GoToCustomUrl(url, 10);
                        await Task.Delay(3000, token);
                    }
                    else
                    {
                        await BrowserWindow.GoToCustomUrl(Constants.gdHomeUrl + $"/{instagramUser?.Username}", delayAfter: 8);
                        var result = await BrowserWindow.ExecuteScriptAsync("[...document.querySelectorAll('div[role=\"button\"]')].find(x=>x.textContent.trim() === \"Message\").click();", 6);
                        if (result is null || !result.Success)
                        {
                            await BrowserWindow.ExecuteScriptAsync("document.querySelector('div[role=\"button\"]>div>svg[aria-label=\"Options\"]').parentNode.click();", delayInSec: 4);
                            await Task.Delay(2000, token);
                            await BrowserWindow.ExecuteScriptAsync("[...document.querySelectorAll('button')].find(x=>x.textContent.trim() === \"Send message\").click();", delayInSec: 7);
                        }
                    }
                    if (SkipAlreadyReceivedMessage)
                    {
                        pageSource = await BrowserWindow.GetPageSourceAsync();
                        var messageNodes = HtmlParseUtility.GetListInnerHtmlFromPartialTagNamecontains(pageSource,"div", "aria-label", "Double tap to like");
                        if (messageNodes != null && messageNodes.Count > 0)
                        {
                            var lastMessage = messageNodes.LastOrDefault();
                            if (!string.IsNullOrEmpty(lastMessage))
                            {
                                ErrorMessage = "Already Received Message";
                                goto Exit;
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(mediaPath))
                    {
                        BrowserWindow.ChooseFileFromDialog(mediaPath, Medias);
                        var Script = "[...document.querySelectorAll('div[role=\"button\"]')].find(x=>x.innerHTML.includes(\"aria-label=\\\"Add Photo or Video\\\"\")).getBoundingClientRect().{0}";
                        var xy = await BrowserWindow.GetXAndYAsync(customScriptX:string.Format(Script,"x"),customScriptY:string.Format(Script,"y"));
                        await BrowserWindow.MouseClickAsync(xy.Key + 20, xy.Value);
                        await Task.Delay(5000, token);
                        var LimitReached = await BrowserWindow.ExecuteScriptAsync($"document.querySelector('div[aria-label=\"Close\" i]').click();", delayInSec: 3);
                        if (LimitReached != null && LimitReached.Success)
                            ErrorMessage = GramStatic.MediaSizeLimitReached;
                    }
                    else
                        await Task.Delay(2000, token);
                    var Result = await BrowserWindow.ExecuteScriptAsync("document.querySelector('div[aria-label=\"Message\"]').className", 3);
                    if (Result != null && !Result.Success)
                    {
                        ErrorMessage = GramStatic.MessageRestricted;
                        goto Exit;
                    }
                    var xAndy = await BrowserWindow.GetXAndYAsync(AttributeType.ClassName,Result?.Result?.ToString());
                    await BrowserWindow.MouseClickAsync(xAndy.Key + 10, xAndy.Value + 5);
                    await Task.Delay(2000, token);
                    await BrowserWindow.EnterCharsAsync(message, 0, delayAtLast: 1);
                    var Clicked = await BrowserWindow.ExecuteScriptAsync("document.getElementsByClassName('xkhd6sd x1n2onr6 x1n5bzlp x173jzuc x1yc6y37 xfs2ol5')[0].click();", delayInSec: 5);
                    if (Clicked is null || !Clicked.Success)
                        Clicked = await BrowserWindow.ExecuteScriptAsync("[...document.querySelectorAll('div[role=\"button\"]')].find(x=>x.innerText===\"Send\").click();", delayInSec: 5);
                    await Task.Delay(2000, token);
                Exit:
                    pageSource = await BrowserWindow.GetPageSourceAsync();

                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
                isRunning = false;
            });

            while (isRunning)
                Task.Delay(500, token).Wait();

            return new SendMessageIgResponseHandler(new ResponseParameter { Response = pageSource }, ErrorMessage);
        }

        public UploadMediaResponse UploadMedia(DominatorAccountModel dominatorAccountModel, InstagramPost instagramPost, List<string> mediaList, CancellationToken token)
        {
            bool isRunning = true;
            string pageSource = string.Empty;
            var ProfileId = string.Empty;
            Application.Current.Dispatcher.Invoke(async () =>
            {
                BrowserWindow.ClearResources();
                var SharePostUrl = mediaList.FirstOrDefault(x => x.StartsWith("http"));
                if (!string.IsNullOrEmpty(SharePostUrl))
                {
                    dominatorAccountModel = await CheckAndAssignUserName(dominatorAccountModel);
                    ProfileId = dominatorAccountModel.AccountBaseModel.ProfileId;
                    var SelectUserClass = "x1lku1pv x1a2a7pz x1dm5mii x16mil14 xiojian x1yutycm x1lliihq x193iq5w xh8yej3";
                    await BrowserWindow.GoToCustomUrl(SharePostUrl, 5);
                    await BrowserWindow.BrowserActAsync(ActType.Click, AttributeType.ClassName,
                                                        "_abl-", index: 0, delayAfter: 5);
                    var xy = await BrowserWindow.GetXAndYAsync(AttributeType.ClassName, "x1rvh84u x1ejq31n xd10rxx x1sy0etr x17r0tee x5ib6vp xjbqb8w xzd0ubt", index: 0);
                    await BrowserWindow.MouseClickAsync(xy.Key + 20, xy.Value);
                    await Task.Delay(5000, token);
                    await BrowserWindow.EnterCharsAsync($" {ProfileId}", typingDelay: 0.1, delayAtLast: 2);
                    await Task.Delay(6000, token);
                    pageSource = await BrowserWindow.GetPageSourceAsync();
                    var Nodes = HtmlParseUtility.GetListNodesFromClassName(pageSource, SelectUserClass);
                    var Index = Nodes != null && Nodes.Count > 0 ? Nodes.IndexOf(Nodes.FirstOrDefault(x => x.InnerHtml.Contains(ProfileId))) : 0;
                    xy = await BrowserWindow.GetXAndYAsync(AttributeType.ClassName, SelectUserClass, index: Index);
                    await BrowserWindow.MouseClickAsync(xy.Key + 20, xy.Value + 5);
                    await Task.Delay(5000, token);
                    await BrowserWindow.BrowserActAsync(ActType.Click, AttributeType.ClassName,
                                                        "xlyipyv x87ps6o xcdnw81 x1i0vuye xh8yej3 x1tu34mt xzloghq x3nfvp2", index: 0, delayAfter: 5);
                    pageSource = await BrowserWindow.GetPaginationData("{\"ranked_recipients\":[{\"user\":", true);
                }
                else
                {
                    await BrowserWindow.GoToCustomUrl(Constants.gdHomeUrl, 5);
                    await BrowserWindow.ExecuteScriptAsync("[...document.querySelectorAll('a[href=\"#\"]')].find(x=>x.innerText ===\"Create\").click();", delayInSec: 3);
                    await BrowserWindow.ExecuteScriptAsync("[...document.querySelectorAll('a[href=\"#\"]')].find(x=>x.innerText ===\"Post\").click();", delayInSec: 3);
                    CheckPathExtension(mediaList);
                    BrowserWindow.ChooseFileFromDialog(pathList: mediaList);
                    var script = "[...document.querySelectorAll('button[type=\"button\"]')].find(x=>x.innerText.toLowerCase().includes(\"select from computer\")).getBoundingClientRect().{0};";
                    var xy = await BrowserWindow.GetXAndYAsync(customScriptX: string.Format(script, "x"), customScriptY: string.Format(script,"y"));
                    await BrowserWindow.MouseClickAsync(xy.Key + 10, xy.Value + 5, delayAfter: 3);
                    await Task.Delay(5000, token);
                    pageSource = await BrowserWindow.GetPageSourceAsync();
                    if (!string.IsNullOrEmpty(pageSource) && pageSource.Contains("Files must be 1KB or more"))
                    {
                        pageSource = "Too small file.It must be atleast 1KB or more.";
                        goto Exit;
                    }
                    if (instagramPost != null && instagramPost.IsCheckedCropMedia && !string.IsNullOrEmpty(instagramPost.CropRatio))
                    {
                        await BrowserWindow.ExecuteScriptAsync("document.querySelector('svg[aria-label=\"Select Crop\" i]').parentElement.parentElement.click();", delayInSec: 4);
                        var SelectRatioClass = "x1t137rt x1o1ewxj x3x9cwd x1e5q0jg x13rtm0m x3nfvp2 x1q0g3np x87ps6o x1lku1pv x1a2a7pz";
                        var Nodes = HtmlParseUtility.GetListNodesFromClassName(BrowserWindow.GetPageSource(), SelectRatioClass);
                        var ClickIndex = Nodes != null && Nodes.Count > 0 ? Nodes.IndexOf(Nodes.FirstOrDefault(x => x.InnerText == instagramPost.CropRatio || !string.IsNullOrEmpty(x.InnerText) && x.InnerText.Contains(instagramPost.CropRatio)) ?? Nodes.FirstOrDefault()) : 0;
                        await BrowserWindow.ExecuteScriptAsync($"document.getElementsByClassName('{SelectRatioClass}')[{ClickIndex}].click();", delayInSec: 4);
                    }
                    await BrowserWindow.ExecuteScriptAsync("[...document.querySelectorAll('div[role=\"button\"]')].find(x=>x.innerText==\"Next\").click();", delayInSec: 4);
                    await Task.Delay(5000, token);
                    await BrowserWindow.ExecuteScriptAsync("[...document.querySelectorAll('div[role=\"button\"]')].find(x=>x.innerText==\"Next\").click();", delayInSec: 4);
                    await Task.Delay(5000, token);

                    if (instagramPost.UserTags.Count != 0)
                    {
                        try
                        {
                            var CordinateClass = "x78zum5 x5yr21d xl56j7k x1n2onr6 x1cy8zhl x1bzprkw";
                            double left = double.Parse(BrowserWindow.EvaluateScript($"document.getElementsByClassName('{CordinateClass}')[0].getBoundingClientRect().left").Result.ToString());

                            double right = double.Parse(BrowserWindow.EvaluateScript($"document.getElementsByClassName('{CordinateClass}')[0].getBoundingClientRect().right").Result.ToString());
                            double top = double.Parse(BrowserWindow.EvaluateScript($"document.getElementsByClassName('{CordinateClass}')[0].getBoundingClientRect().top").Result.ToString());
                            double bottom = double.Parse(BrowserWindow.EvaluateScript($"document.getElementsByClassName('{CordinateClass}')[0].getBoundingClientRect().bottom").Result.ToString());
                            Dictionary<string, double> imageCordinates = new Dictionary<string, double>()
                    {
                        {"Left",left },
                        {"Right", right },
                        {"Top", top },
                        {"Bottom", bottom }
                    };

                            var xAndY = new KeyValuePair<int, int>();
                            foreach (var tags in instagramPost.UserTags)
                            {
                                var FailedCount = 0;
                                Random r = new Random();
                            RetryGetXY:
                                int x = r.Next((int)imageCordinates["Left"], (int)imageCordinates["Right"]);
                                int y = r.Next((int)imageCordinates["Top"], (int)imageCordinates["Bottom"]);
                                xAndY = new KeyValuePair<int, int>(x, y);
                                await BrowserWindow.MouseClickAsync(xAndY.Key, xAndY.Value, delayAfter: 3);
                                var pageResponse = await BrowserWindow.GetPageSourceAsync();
                                if (pageResponse != null && !pageResponse.Contains("Tag:") && FailedCount++ <= 5)
                                    goto RetryGetXY;
                                FailedCount = 0;
                                await BrowserWindow.EnterCharsAsync($" {tags.Username}", typingDelay: 0.1, delayAtLast: 4);
                                var Nodes = HtmlParseUtility.GetListNodesFromClassName(await BrowserWindow.GetPageSourceAsync(), "_acmu");
                                var ClickIndex = Nodes != null && Nodes.Count > 0 ? Nodes.IndexOf(Nodes.FirstOrDefault(z => !string.IsNullOrEmpty(z?.InnerText) && (z.InnerText == tags.Username || z.InnerText.Contains(tags.Username)))) : 0;
                                await BrowserWindow.BrowserActAsync(ActType.Click, AttributeType.ClassName, " _acmy", index: ClickIndex, delayAfter: 4);
                            }
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }
                    }

                    if (instagramPost.Location != null)
                    {
                        var x = await BrowserWindow.ExecuteScriptAsync("document.querySelector('label > input[placeholder=\"Add Location\" i]').getBoundingClientRect().x", 2);
                        var y = await BrowserWindow.ExecuteScriptAsync("document.querySelector('label > input[placeholder=\"Add Location\" i]').getBoundingClientRect().y", 2);
                        await BrowserWindow.MouseClickAsync(GetCordinate(x?.Result?.ToString()) + 200, GetCordinate(y?.Result?.ToString()) + 15);
                        await BrowserWindow.EnterCharsAsync($" {instagramPost.Location.Name}", typingDelay: 0.1, delayAtLast: 4);
                        var count = await TryGetIndexByInnerText("_acmx _acmz", $"{instagramPost.Location.Name}");
                        await BrowserWindow.BrowserActAsync(ActType.Click, AttributeType.ClassName, "_acmx _acmz", index: 1, delayAfter: 3);
                    }

                    if (!string.IsNullOrEmpty(instagramPost.Caption))
                    {
                        script = "document.querySelector('div[role=\"textbox\"]').getBoundingClientRect().{0};";
                        var xAndy = await BrowserWindow.GetXAndYAsync(customScriptX:string.Format(script,"x"),customScriptY:string.Format(script,"y"));
                        await BrowserWindow.MouseClickAsync(xAndy.Key +10, xAndy.Value + 15, delayAfter: 4);
                        var captions = instagramPost.Caption?.Replace("\r\n", "\n")?.Split('\n');
                        if (captions != null && captions.Length > 1)
                        {
                            foreach (var text in captions)
                            {
                                await BrowserWindow.EnterCharsAsync($"{text}", typingDelay: 0.1, delayAtLast: 2);
                                if (text != captions.LastOrDefault())
                                    await BrowserWindow.SendKeyEventAsCharacter();
                            }
                        }
                        else
                            await BrowserWindow.EnterCharsAsync($"{instagramPost.Caption}", typingDelay: 0.1, delayAtLast: 2);
                    }
                    await Task.Delay(5000, token);
                    if (instagramPost.CommentsDisabled)
                    {
                        await BrowserWindow.BrowserActAsync(ActType.Click, AttributeType.ClassName, "x6s0dn4 x1ypdohk x78zum5 x1q0g3np x1qughib xyinxu5 x1pi30zi x1g2khh7 x1swvt13 x87ps6o", index: 1, delayAfter: 3);
                        await Task.Delay(5000, token);
                        await BrowserWindow.BrowserActAsync(ActType.ActByQuery, AttributeType.Role, "switch", index: 1, delayAfter: 3);
                    }
                    BrowserWindow.ClearResources();
                    await BrowserWindow.ExecuteScriptAsync("[...document.querySelectorAll('div[role=\"button\"]')].find(x=>x.innerText==\"Share\").click();", delayInSec: 4);
                    await Task.Delay(5000, token);
                    int tries = 0;
                    var pageStatus = await BrowserWindow.GetPageSourceAsync();
                    var finalResponse = string.Empty;
                    while (!pageStatus.Contains("Your post has been shared.") && ++tries < 75 && string.IsNullOrEmpty(SharePostUrl)
                    && string.IsNullOrEmpty(finalResponse))
                    {
                        await Task.Delay(2000);
                        pageStatus = await BrowserWindow.GetPageSourceAsync();
                        finalResponse = await BrowserWindow.GetPaginationData("{\"media\":{\"taken_at\":", true);
                        finalResponse = string.IsNullOrEmpty(finalResponse) ? await BrowserWindow.GetPaginationData("\"media\":{\"taken_at\":", true) : finalResponse;
                        finalResponse = string.IsNullOrEmpty(finalResponse) ? await BrowserWindow.GetPaginationData("{\"media\":{\"pk\"", true) : finalResponse;
                        finalResponse = string.IsNullOrEmpty(finalResponse) ? await BrowserWindow.GetPaginationData("\"media\":{\"pk\"", true) : finalResponse;
                    }
                    if (string.IsNullOrEmpty(finalResponse))
                    {
                        finalResponse = await BrowserWindow.GetPaginationData("{\"media\":{\"taken_at\":", true);
                        finalResponse = string.IsNullOrEmpty(finalResponse) ? await BrowserWindow.GetPaginationData("\"media\":{\"taken_at\":", true) : finalResponse;
                        finalResponse = string.IsNullOrEmpty(finalResponse) ? await BrowserWindow.GetPaginationData("{\"media\":{\"pk\"", true) : finalResponse;
                        finalResponse = string.IsNullOrEmpty(finalResponse) ? await BrowserWindow.GetPaginationData("\"media\":{\"pk\"", true) : finalResponse;
                    }
                    await BrowserWindow.ExecuteScriptAsync("[...document.querySelectorAll('div[role=\"button\"]')].find(x=>x.innerHTML.includes('aria-label=\"Close\"')).click();", delayInSec: 3);//Close Popup Window
                    pageSource = finalResponse;
                }
            Exit:
                isRunning = false;
            });

            while (isRunning)
                Task.Delay(500, token).Wait();

            return new UploadMediaResponse(new ResponseParameter
            {
                Response = pageSource
            }, ProfileId);
        }

        private void CheckPathExtension(List<string> mediaList)
        {
            for (int i = 0; i < mediaList.Count; i++)
            {
                var result = System.IO.Path.ChangeExtension(mediaList[i], ".jpg");
                System.IO.File.Move(mediaList[i], result);
                mediaList[i] = result;
            }
        }

        public async Task<int> TryGetIndexByInnerText(string className, string matchText)
        {
            int count = 0;
            do
            {
                var innerTexts = await BrowserWindow.GetElementValueAsync(ActType.GetValue, AttributeType.ClassName,
                                                className,
                                                valueType: ValueTypes.InnerText, clickIndex: count);
                if (innerTexts.Equals(matchText))
                    break;
            } while (++count > 0 && count < 50);
            return count;
        }

        public async Task<int> TryGetIndexByOuterHtml(string className, string matchText)
        {
            int count = 0;
            var classLst = await BrowserWindow.GetListInnerHtml(ActType.GetValue, AttributeType.ClassName, className,
                                                            valueType: ValueTypes.OuterHtml);
            classLst.Reverse();
            count = classLst.IndexOf(classLst.FirstOrDefault(x => x.Contains(matchText)));
            return count;
        }

        public CommonIgResponseHandler GetFeedTimeLineData(DominatorAccountModel dominatorAccountModel, CancellationToken token)
        {
            bool isRunning = true;
            int count = 0;
            int prevCount = 0;
            int failedCount = 0;
            string pageSource = string.Empty;
            var userDataList = new List<string>();
            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    int previousCount = 0;
                Retry:
                    await BrowserWindow.SetCookies(dominatorAccountModel);
                    BrowserWindow.ClearResources();

                    await BrowserWindow.GoToCustomUrl(Constants.gdHomeUrl, 5);
                    await Task.Delay(2000, token);
                    await BrowserWindow.SaveCookies();
                    var screenResolution = Constants.GetScreenResolution();
                    var lst = new List<string>();

                    failedCount = 0;
                    prevCount = 0;
                    do
                    {
                        count = 0;
                        await BrowserWindow.MouseScrollAsync(screenResolution.Key / 2, screenResolution.Value / 2
                                , 0, -300, delayAfter: 5, scrollCount: 5);
                        lst = await BrowserWindow.GetPaginationDataList("{\"data\":{\"xdt_api__v1__feed__timeline__connection\"", true);

                        lst.ForEach(x => count += Regex.Matches(x.ToString(), "{\"ad_id\":").Count);
                        if (lst.Count == 10)
                            break;
                        if (count == 0)
                            failedCount++;
                        if (count <= prevCount)
                            failedCount++;
                        else
                        {
                            failedCount = 0;
                            prevCount = count;
                        }
                    } while (count < 50 && failedCount < 10);
                    lst.RemoveAll(x => x.StartsWith("<!DOCTYPE html>"));
                    userDataList.AddRange(lst);
                    count = 0;
                    userDataList.ForEach(x => count += Regex.Matches(x.ToString(), "{\"ad_id\":").Count);
                    if (failedCount >= 10 || (count < 50 && count > previousCount))
                    {
                        previousCount = count;
                        await BrowserWindow.SaveCookies();
                        goto Retry;
                    }
                    await Task.Delay(2000, token);
                    pageSource = JsonConvert.SerializeObject(userDataList);
                }
                catch (Exception ex)
                {
                    ex.DebugLog(ex.Message);
                }
                isRunning = false;
            });

            while (isRunning)
                Task.Delay(500, token).Wait();

            return new CommonIgResponseHandler(new ResponseParameter { Response = pageSource });
        }

        public V2InboxResponse GetInbox(DominatorAccountModel dominatorAccountModel, bool isPendingUsers = false, CancellationToken token = default, bool IsAutoReplyToNewMessage = false, int ScrollCount = 0)
        {
            bool isRunning = true;
            string pageSource = string.Empty;
            var PendingUsers = new List<string>();
            V2InboxResponse MessageResponse = null;
            var FailedCount = 0;
            if (isPendingUsers)
                PendingUsers = CheckAndAcceptAllPendingMessageRequest(dominatorAccountModel);
            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    var url = Constants.gdHomeUrl + "/direct/inbox/";
                    var Senders = new List<SenderDetails>();
                    do
                    {
                        BrowserWindow.ClearResources();
                        await BrowserWindow.GoToCustomUrl(url, 5);
                        await Task.Delay(8000, token);
                        pageSource = await GetMessageResponse();
                    } while (!pageSource.Contains("truncateTablesForSyncGroup") && FailedCount++ <= 3);
                    pageSource = pageSource.Replace(@"\", "");
                    MessageResponse = new V2InboxResponse(new ResponseParameter { Response = pageSource }, isPendingUsers, IsAutoReplyToNewMessage, PendingUsers, ScrollCount);
                    Senders.AddRange(MessageResponse?.LstSenderDetails);
                    //Scroll
                    var count = 0;
                    var MessageListClass = "x13dflua x19991ni";
                    var lastNodes = 0;
                    do
                    {
                        pageSource = await BrowserWindow.GetPageSourceAsync();
                        var Nodes = HtmlParseUtility.GetListNodesFromClassName(pageSource, MessageListClass);
                        if (lastNodes == Nodes.Count)
                            break;
                        var ScrollIndex = Nodes != null && Nodes.Count > 0 ? Nodes.Count - 1 : 0;
                        await BrowserWindow.ExecuteScriptAsync($"document.getElementsByClassName('{MessageListClass}')[{ScrollIndex}].scrollIntoViewIfNeeded();", 4);
                        lastNodes = Nodes.Count;
                        pageSource = await GetMessageResponse();
                        pageSource = pageSource.Replace(@"\", "");
                        MessageResponse = new V2InboxResponse(new ResponseParameter { Response = pageSource }, isPendingUsers, IsAutoReplyToNewMessage, PendingUsers, ScrollCount);
                        Senders.AddRange(MessageResponse?.LstSenderDetails);
                    } while (count++ <= 5);
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
                isRunning = false;
            });

            while (isRunning)
                Task.Delay(500, token).Wait();
            return MessageResponse;
        }

        private async Task<string> GetMessageResponse()
        {
            var Response = string.Empty;
            try
            {
                var Page = await BrowserWindow.GetPageSourceAsync();
                Response = Regex.Match(Page, "{\"data\":{\"lightspeed_web_request_for_igd\":{\"payload\":\"(.*?)\",\"dependencies\":").Groups[1].Value;
                if (string.IsNullOrEmpty(Response))
                {
                    var dataList = await BrowserWindow.GetPaginationDataList("{\"data\":{\"lightspeed_web_request_for_igd\":{\"payload\":", true);
                    dataList.RemoveAll(x => !x.Contains("truncateTablesForSyncGroup"));
                    Response = Regex.Match(dataList[0].ToString(), "{\"data\":{\"lightspeed_web_request_for_igd\":{\"payload\":\"(.*?)\",\"dependencies\":").Groups[1].Value;
                }
                if (Response.Length < 300 || !Response.Contains("truncateTablesForSyncGroup"))
                {
                    var Split = Regex.Split(Page, "<script type=\"application/json\"");
                    if (Split.Length > 0)
                    {
                        var Res = Split.FirstOrDefault(x => x.Contains("truncateTablesForSyncGroup"));
                        Response = !string.IsNullOrEmpty(Res) ? Regex.Match(Res, "{\"data\":{\"lightspeed_web_request_for_igd\":{\"payload\":\"(.*?)\",\"dependencies\":").Groups[1].Value : Response;
                    }
                }
            }
            catch (Exception) { }
            return Response;
        }

        public DeleteMediaIgResponseHandler DeleteMedia(DominatorAccountModel dominatorAccountModel, InstagramPost instagramPost, CancellationToken token)
        {
            bool isRunning = true;
            string pageSource = string.Empty;
            Application.Current.Dispatcher.Invoke(async () =>
            {
                var url = Constants.gdHomeUrl + $"/p/{instagramPost.Code}";
                try
                {
                    BrowserWindow.ClearResources();
                    await BrowserWindow.GoToCustomUrl(url, 5);
                    await Task.Delay(4000, token);
                    await BrowserWindow.BrowserActAsync(ActType.Click, AttributeType.ClassName, "xcdnw81 xexx8yu x4uap5 x18d9i69 xkhd6sd",
                                            index: 0, delayAfter: 3);
                    var index = await TryGetIndexByInnerText("_a9-- _a9-_", "Delete");

                    await BrowserWindow.BrowserActAsync(ActType.Click, AttributeType.ClassName, "_a9-- _a9-_",
                                            index: index, delayAfter: 3);
                    await Task.Delay(2000, token);
                    index = await TryGetIndexByInnerText("_a9-- _a9-_", "Delete");
                    await BrowserWindow.BrowserActAsync(ActType.Click, AttributeType.ClassName, "_a9-- _a9-_",
                                            index: index, delayAfter: 3);
                    await Task.Delay(2000, token);
                    pageSource = await BrowserWindow.GetPaginationData("{\"did_delete\":", true);
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
                isRunning = false;
            });

            while (isRunning)
                Task.Delay(500, token).Wait();

            return new DeleteMediaIgResponseHandler(new ResponseParameter { Response = pageSource });
        }

        public async Task<CommonIgResponseHandler> SetBiographyAsync(DominatorAccountModel dominatorAccountModel, string bio)
        {
            bool isRunning = true;
            string pageSource = string.Empty;
            await Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    await BrowserWindow.GoToCustomUrl($"{Constants.gdHomeUrl}/accounts/edit/", 5);
                    var xy = await BrowserWindow.GetXAndYAsync(customScriptX: "document.querySelectorAll('[id=\"pepBio\"]')[0].getBoundingClientRect().x",
                                                              customScriptY: "document.querySelectorAll('[id=\"pepBio\"]')[0].getBoundingClientRect().y");
                    await BrowserWindow.MouseClickAsync(xy.Key, xy.Value, 3);
                    BrowserWindow.SelectAllText();
                    await BrowserWindow.PressAnyKeyUpdated(8, delay: 3);
                    await Task.Delay(3000);
                    await BrowserWindow.EnterCharsAsync(" " + bio, 0.2, 5);
                    BrowserWindow.ClearResources();
                    var index = await TryGetIndexByInnerText("_ab47", "Submit");
                    xy = await BrowserWindow.GetXAndYAsync(AttributeType.ClassName, "_ab47", index);

                    await BrowserWindow.MouseClickAsync(xy.Key + 5, xy.Value + 10, 5);
                    await Task.Delay(3000);
                    pageSource = await BrowserWindow.GetPaginationData("{\"status\":\"ok\"}", true);
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
                isRunning = false;
            });

            while (isRunning)
                Task.Delay(500).Wait();

            return new CommonIgResponseHandler(new ResponseParameter { Response = pageSource });
        }

        public async Task<UsernameInfoIgResponseHandler> EditProfileAsync(DominatorAccountModel dominatorAccountModel, EditProfileModel editProfileModel, CancellationToken none)
        {
            bool isRunning = true;
            string pageSource = string.Empty;
            await Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    BrowserWindow.ClearResources();
                    await BrowserWindow.GoToCustomUrl($"{Constants.gdHomeUrl}/accounts/edit/", 5);
                    await Task.Delay(2000, none);
                    pageSource = await BrowserWindow.GetPaginationData("{\"form_data\":", true);
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
                isRunning = false;
            });

            while (isRunning)
                Task.Delay(500).Wait();

            return new UsernameInfoIgResponseHandler(new ResponseParameter { Response = pageSource }, editProfileModel);
        }

        public async Task<CommonIgResponseHandler> SetGenderAsync(DominatorAccountModel dominatorAccountModel, EditProfileModel editProfileModel, CancellationToken none)
        {
            bool isRunning = true;
            string pageSource = string.Empty;
            await Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    await BrowserWindow.GoToCustomUrl($"{Constants.gdHomeUrl}/accounts/edit/", 5);
                    await Task.Delay(2000, none);
                    BrowserWindow.ClearResources();
                    await BrowserWindow.BrowserActAsync(ActType.Click, AttributeType.ClassName, "_acan _acao _acaq _acat _aj1-", delayAfter: 5);
                    int gender = editProfileModel.IsMaleChecked ? 2 : editProfileModel.IsFemaleChecked ? 1 : 3;
                    await BrowserWindow.BrowserActAsync(ActType.ActByQuery, AttributeType.Type, "radio", index: gender, delayAfter: 3);
                    await BrowserWindow.BrowserActAsync(ActType.Click, AttributeType.ClassName, "_acan _acap _acas _acav _aj1-", delayAfter: 5);
                    await Task.Delay(12000, none);
                    pageSource = await BrowserWindow.GetPaginationData("{\"status\":\"ok\"}", true);
                    var index = await TryGetIndexByInnerText("_ab47", "Submit");
                    var xy = await BrowserWindow.GetXAndYAsync(AttributeType.ClassName, "_ab47", index);
                    await BrowserWindow.MouseClickAsync(xy.Key + 5, xy.Value + 10, 5);
                    await Task.Delay(3000, none);
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
                isRunning = false;
            });

            while (isRunning)
                Task.Delay(500).Wait();

            return new CommonIgResponseHandler(new ResponseParameter { Response = pageSource });
        }

        public async Task<UsernameInfoIgResponseHandler> ChangeProfilePictureAsync(DominatorAccountModel dominatorAccountModel, EditProfileModel editProfileModel, CancellationToken none)
        {
            bool isRunning = true;
            string pageSource = string.Empty;
            await Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    await BrowserWindow.GoToCustomUrl($"{Constants.gdHomeUrl}/accounts/edit/", 5);
                    await Task.Delay(2000, none);
                    BrowserWindow.ClearResources();
                    await BrowserWindow.BrowserActAsync(ActType.ActByQuery, AttributeType.Role, "button", index: 0, delayAfter: 5);
                    BrowserWindow.ChooseFileFromDialog(editProfileModel.ProfilePicPath);
                    var xy = await BrowserWindow.GetXAndYAsync(AttributeType.ClassName, "_a9-- _a9_0");
                    await BrowserWindow.MouseClickAsync(xy.Key, xy.Value, delayAfter: 10);
                    await Task.Delay(3000, none);
                    pageSource = await BrowserWindow.GetPaginationData("{\"changed_profile\":", true);
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
                isRunning = false;
            });

            while (isRunning)
                Task.Delay(500).Wait();

            return new UsernameInfoIgResponseHandler(new ResponseParameter { Response = pageSource }, editProfileModel);
        }

        public List<string> CheckAndAcceptAllPendingMessageRequest(DominatorAccountModel dominatorAccount, int scrollCount = 4)
        {
            bool isRunning = true;
            string pageSource = string.Empty;
            var ScrollCount = 0;
            var pendingUsers = new List<string>();
            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    var url = Constants.gdHomeUrl + "/direct/requests/";
                    var PrevNodeCount = 0;
                    BrowserWindow.ClearResources();
                    await BrowserWindow.GoToCustomUrl(url, 5);
                    await Task.Delay(8000, dominatorAccount.Token);
                    var UserNodesClass = "x9f619 x1n2onr6 x1ja2u2z x1qjc9v5 x78zum5 xdt5ytf x1iyjqo2 xl56j7k xeuugli xxsgkw5";
                    var Nodes = new List<string>();
                    do
                    {

                        pageSource = await BrowserWindow.GetPageSourceAsync();
                        var currentNodeCount = 0;
                        Nodes = HtmlParseUtility.GetListInnerHtmlFromTagName(pageSource, "div", "class", UserNodesClass);
                        currentNodeCount = Nodes.Count > 0 ? Nodes.Count : currentNodeCount;
                        //ScrollLogic
                        var index = Nodes.Count > 0 ? Nodes.Count - 1 : 0;
                        await BrowserWindow.ExecuteScriptAsync($"document.getElementsByClassName('{UserNodesClass}')[{index}].scrollIntoView();", 3);
                        pageSource = await BrowserWindow.GetPageSourceAsync();
                        Nodes = HtmlParseUtility.GetListInnerHtmlFromTagName(pageSource, "div", "class", UserNodesClass);
                        if (PrevNodeCount == currentNodeCount)
                            break;
                        PrevNodeCount = currentNodeCount;
                    } while (ScrollCount++ <= scrollCount);
                    //Accepting All Pending Requests.
                    if (Nodes.Count > 0)
                    {
                        foreach (var node in Nodes)
                        {
                            await BrowserWindow.ExecuteScriptAsync($"document.getElementsByClassName('{UserNodesClass}')[{Nodes.IndexOf(node)}].click();", 6);
                            //Getting Pending UserNames.
                            var page = await BrowserWindow.GetPageSourceAsync();
                            var UserName = HtmlParseUtility.GetListInnerHtmlFromPartialTagNamecontains(page, "div", "class", "x1h91t0o xkh2ocl x78zum5 xdt5ytf x13a6bvl x193iq5w x1c4vz4f xcrg951 x1sxyh0 x1nvil2r");
                            UserName = UserName is null || UserName.Count == 0 ? HtmlParseUtility.GetListInnerHtmlFromPartialTagNamecontains(page, "div", "class", "x1h91t0o xkh2ocl x78zum5 xdt5ytf x13a6bvl x193iq5w x1c4vz4f x1eb86dx x1sxyh0 x1nvil2r") : UserName;
                            var userName = Utilities.GetBetween(UserName?.FirstOrDefault(), "href=\"/", "\"");
                            pendingUsers.Add(userName);
                            //Hitting Accept Button.
                            await BrowserWindow.ExecuteScriptAsync($"document.getElementsByClassName('x1qhh985 xm0m39n xt0psk2 xt7dq6l xexx8yu x4uap5 x18d9i69 xkhd6sd x1n2onr6 x1n5bzlp xqnirrm xj34u2y')[0].click();", 6);
                            pageSource = await BrowserWindow.GetPageSourceAsync();
                            if (pageSource != null && pageSource.Contains("Move messages from"))
                                await BrowserWindow.ExecuteScriptAsync($"document.getElementsByClassName('xurb0ha x2b8uid x87ps6o xxymvpz xh8yej3 x52vrxo x4gyw5p x5n08af')[0].click();", 6);
                            Nodes = HtmlParseUtility.GetListInnerHtmlFromTagName(pageSource, "div", "class", UserNodesClass);
                        }
                    }

                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
                isRunning = false;
            });
            while (isRunning)
                Task.Delay(500, dominatorAccount.Token).Wait();
            return pendingUsers;
        }

        public async Task<DominatorAccountModel> CheckAndAssignUserName(DominatorAccountModel dominatorAccount)
        {
            try
            {
                bool isRunning = true;
                await Application.Current.Dispatcher.Invoke(async () =>
                {
                    if (string.IsNullOrEmpty(dominatorAccount.AccountBaseModel.ProfileId))
                    {
                        await BrowserWindow.GoToCustomUrl($"https://www.instagram.com/", 5);
                        var ProfileId = await BrowserWindow.GetElementValueAsync(ActType.GetValue, AttributeType.ClassName, "x4uap5 x18d9i69 xkhd6sd x16tdsg8 x1hl2dhg xggy1nq x1a2a7pz _aak1 _a6hd", valueType: ValueTypes.InnerText);
                        ProfileId = string.IsNullOrEmpty(ProfileId) ? await BrowserWindow.GetElementValueAsync(ActType.GetValue, AttributeType.ClassName, "x16tdsg8 x1hl2dhg xggy1nq x1a2a7pz x5n08af xwhw2v2 x6ikm8r x10wlt62 xlyipyv x9n4tj2 _a6hd", ValueTypes.InnerText) : ProfileId;
                        if (string.IsNullOrEmpty(ProfileId))
                        {
                            var Nodes = HtmlParseUtility.GetListNodesFromClassName(await BrowserWindow.GetPageSourceAsync(), "x4k7w5x x1h91t0o x1h9r5lt x1jfb8zj xv2umb2 x1beo9mf xaigb6o x12ejxvf x3igimt xarpa2k xedcshv x1lytzrv x1t2pt76 x7ja8zs x1qrby5j");
                            var Node = Nodes != null && Nodes.Count > 0 ? Nodes.FirstOrDefault(x => x.InnerText.Contains("Profile") || x.InnerText.Contains("profile")) : null;
                            if (Node != null)
                                ProfileId = Utilities.GetBetween(Node.InnerHtml, "href=\"/", "/\"");
                        }
                        dominatorAccount.AccountBaseModel.ProfileId = ProfileId?.Trim();
                    }
                    isRunning = false;
                });
                while (isRunning) Task.Delay(500, dominatorAccount.Token).Wait();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return dominatorAccount;
        }

        public async Task<(bool, string, List<string>)> GetListJsonResponse()
        {
            var list = new List<string>();
            var HasMore = true;
            var maxId = string.Empty;
            try
            {
                list = await BrowserWindow.GetPaginationDataList("{\"items\":[{\"taken_at\":", true);
                list = list is null || list.Count == 0 ? await BrowserWindow.GetPaginationDataList("{\"data\":{\"xdt_api__v1__feed__user_timeline_graphql_connection\"", true) : list;
                list = list is null || list.Count == 0 ? await BrowserWindow.GetPaginationDataList("\"items\":[{\"taken_at\":", true) : list;
                //Getting Max ID.
                if (list != null && list.Count > 0)
                {
                    var lastNode = list.LastOrDefault(x => !x.StartsWith("{\"ad_media_items\"")) ?? list.LastOrDefault();
                    var jObject = handler.ParseJsonToJObject(lastNode);
                    maxId = handler.GetJTokenValue(jObject, "next_max_id");
                    if (string.IsNullOrEmpty(maxId))
                        maxId = handler.GetJTokenValue(jObject, "data", "xdt_api__v1__feed__user_timeline_graphql_connection", "page_info", "end_cursor");
                    HasMore = !string.IsNullOrEmpty(maxId) && !maxId.Contains("None");
                }
                foreach (var item in list.ToList())
                {
                    if (string.IsNullOrEmpty(item) || !item.Contains("<!DOCTYPE html>"))
                        continue;
                    var index = list.IndexOf(item);
                    var json = "{\"data\":{\"xdt_api__v1__feed__user_timeline_graphql_connection\":" + Utilities.GetBetween(item, "{\"data\":{\"xdt_api__v1__feed__user_timeline_graphql_connection\":", ",\"sequence_number\"");
                    if (handler.IsValidJson(json))
                        list[index] = json;
                }
            }
            catch (Exception) { }
            return (HasMore, maxId, list);
        }

        public async Task<CloseFriendsResponseHandler> MakeCloseFriends(DominatorAccountModel dominatorAccount, InstagramUser user)
        {
            var pageResponse = string.Empty;
            var IsRunning = true;
            await Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    if (PuppeteerBrowser is null || !await PuppeteerBrowser.IsBrowserOpenend())
                    {
                        PuppeteerBrowser = new PuppeteerBrowserActivity(dominatorAccount, isNeedResourceData: true, targetUrl: GramStatic.GetCloseFriendListUrl);
                        await PuppeteerBrowser.LaunchBrowserAsync();
                    }
                    if (user != null)
                    {
                        if (user.IsFollowing || (user.UserDetails != null && user.UserDetails.IsFollowing))
                        {
                            await PuppeteerBrowser.ChangeTabs($"https://www.instagram.com/{user.Username}/");
                            var failed = 0;
                            while (failed++ <= 5 && !PuppeteerBrowser.IsLoaded)
                                await Task.Delay(4000);
                            await PuppeteerBrowser.ExecuteScriptAsync("[...document.querySelectorAll('button[type=\"button\"]')].filter(x=>x.innerText.includes(\"Following\"))[0].click();", delayInSec: 4);
                            var directScript = "[...document.querySelectorAll('div[role=\"button\"]')].filter(x=>x.innerText.toLowerCase()==\"{0}\")[0].{1}";
                            var IsClosedFriend = await PuppeteerBrowser.ExecuteScriptAsync(string.Format(directScript, "close friend", "innerText"), delayInSec: 2);
                            if (IsClosedFriend != null && IsClosedFriend.Result != null && IsClosedFriend.Result.ToString().ToLower().Contains("close friend"))
                            {
                                pageResponse = "Already Have Close Friend";
                                goto Exit;
                            }
                            PuppeteerBrowser.ClearResources();
                            await PuppeteerBrowser.ExecuteScriptAsync(string.Format(directScript, "add to close friends list", "click();"), delayInSec: 5);
                            pageResponse = await PuppeteerBrowser.GetPaginationData("\"xdt_set_besties\"", true);
                        }
                        else
                        {
                            var script = "document.querySelector('input[data-bloks-name=\"bk.components.TextInput\"]').getBoundingClientRect().{0};";
                            var IsExist = await PuppeteerBrowser.ChangeTabs(GramStatic.GetCloseFriendListUrl, false, true);
                            if (!IsExist)
                            {
                                await PuppeteerBrowser.ChangeTabs("https://www.instagram.com/accounts/edit/", true, true);
                                await Task.Delay(4000);
                                await PuppeteerBrowser.ExecuteScriptAsync("document.querySelector('a[href=\"/accounts/close_friends/\"]').click();", delayInSec: 6);
                            }
                            else
                                await Task.Delay(2000);
                            var failed = 0;
                            while (failed++ <= 5 && !PuppeteerBrowser.IsLoaded)
                                await Task.Delay(4000);
                            var xy = await PuppeteerBrowser.GetXAndYAsync(customScriptX: string.Format(script, "x"), customScriptY: string.Format(script, "y"));
                            await PuppeteerBrowser.MouseClickAsync(xy.Key + 10, xy.Value + 5, delayAfter: 3);
                            PuppeteerBrowser.SelectAllText();
                            await PuppeteerBrowser.PressAnyKeyUpdated(8, delayAtLast: 3);
                            await PuppeteerBrowser.EnterCharsAsync(user.Username, delayAtLast: 5);
                            var closeFriendscript = "[...document.querySelectorAll('div[role=\"button\"]')].filter(x=>x.innerText.trim().split(\"\\n\")[0] === \"{0}\")[0].{1}";
                            var IsClosedFriend = await PuppeteerBrowser.ExecuteScriptAsync(string.Format(closeFriendscript, user.Username, "InnerHtml.contains(\"circle-check\");"), delayInSec: 2);
                            if (IsClosedFriend != null && IsClosedFriend?.Result?.ToString()?.ToLower() == "true")
                            {
                                pageResponse = "Already Have Close Friend";
                                goto Exit;
                            }
                            PuppeteerBrowser.ClearResources();
                            await Task.Delay(4000);
                            await PuppeteerBrowser.ExecuteScriptAsync(string.Format(closeFriendscript, user.Username, "click();"), delayInSec: 5);
                            pageResponse = await PuppeteerBrowser.GetPaginationData("xdt__settings__get_screen_dependencies", true);
                            if (string.IsNullOrEmpty(pageResponse))
                            {
                                IsClosedFriend = await PuppeteerBrowser.ExecuteScriptAsync(string.Format(closeFriendscript, user.Username, "InnerHtml.contains(\"circle-check\");"), delayInSec: 2);
                                if (IsClosedFriend != null && IsClosedFriend?.Result?.ToString()?.ToLower() == "true")
                                {
                                    pageResponse = "Friends";
                                    goto Exit;
                                }
                            }
                        }
                    }
                Exit:
                    IsRunning = false;
                }
                catch { IsRunning = false; }
            });
            return new CloseFriendsResponseHandler(new ResponseParameter { Response = pageResponse });
        }

        public FollowerAndFollowingIgResponseHandler GetUserFollowerChrome(DominatorAccountModel dominatorAccountModel, string userId, CancellationToken token, bool IsCloseFriend = false, string maxID = "", List<string> CloseFriends = null)
        {
            bool isRunning = true;
            string pageSource = string.Empty;
            bool hasMore = true;
            var listuser = new List<InstagramUser>();
            FollowerAndFollowingIgResponseHandler igResponseHandler = null;
            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    if (PuppeteerBrowser is null || !await PuppeteerBrowser.IsBrowserOpenend())
                    {
                        PuppeteerBrowser = new PuppeteerBrowserActivity(dominatorAccountModel, isNeedResourceData: true);
                        await PuppeteerBrowser.LaunchBrowserAsync();
                    }
                    hasMore = true;
                    if (string.Equals(userId, dominatorAccountModel.AccountBaseModel.UserName) || userId.Contains("@"))
                    {
                        if (string.IsNullOrEmpty(dominatorAccountModel.AccountBaseModel.ProfileId))
                        {
                            await PuppeteerBrowser.GoToCustomUrl($"https://www.instagram.com/", 5);
                            var ProfileId = await PuppeteerBrowser.GetElementValueAsync(ActType.GetValue, AttributeType.ClassName, "x4uap5 x18d9i69 xkhd6sd x16tdsg8 x1hl2dhg xggy1nq x1a2a7pz _aak1 _a6hd", valueType: ValueTypes.InnerText);
                            ProfileId = string.IsNullOrEmpty(ProfileId) ? await PuppeteerBrowser.GetElementValueAsync(ActType.GetValue, AttributeType.ClassName, "x16tdsg8 x1hl2dhg xggy1nq x1a2a7pz x5n08af xwhw2v2 x6ikm8r x10wlt62 xlyipyv x9n4tj2 _a6hd", ValueTypes.InnerText) : ProfileId;
                            if (string.IsNullOrEmpty(ProfileId))
                            {
                                var Nodes = HtmlParseUtility.GetListNodesFromClassName(await PuppeteerBrowser.GetPageSourceAsync(), "x4k7w5x x1h91t0o x1h9r5lt x1jfb8zj xv2umb2 x1beo9mf xaigb6o x12ejxvf x3igimt xarpa2k xedcshv x1lytzrv x1t2pt76 x7ja8zs x1qrby5j");
                                var Node = Nodes != null && Nodes.Count > 0 ? Nodes.FirstOrDefault(x => x.InnerText.Contains("Profile") || x.InnerText.Contains("profile")) : null;
                                if (Node != null)
                                    ProfileId = Utilities.GetBetween(Node.InnerHtml, "href=\"/", "/\"");
                            }
                            dominatorAccountModel.AccountBaseModel.ProfileId = ProfileId?.Trim();
                        }
                        userId = dominatorAccountModel.AccountBaseModel.ProfileId;
                    }
                    PuppeteerBrowser.ClearResources();
                    await PuppeteerBrowser.ChangeTabs($"https://www.instagram.com/{userId}/followers/", true, true);
                    await Task.Delay(5000);
                    var DialogResult = await PuppeteerBrowser.ExecuteScriptAsync("document.querySelector('div[role=\"dialog\"]').innerText", delayInSec: 2);
                    if (DialogResult == null || !DialogResult.Success || string.IsNullOrEmpty(DialogResult.Result.ToString()))
                    {
                        await PuppeteerBrowser.ExecuteScriptAsync($"document.querySelector('a[href=\"/{userId}/followers/\"]').click();", delayInSec: 2);
                    }
                    await Task.Delay(5000);
                    var userDataList = new List<string>();
                    var lastScrollCount = 0;
                    var userList = await PuppeteerBrowser.GetPaginationDataList("{\"xdt_api__v1__friendships__followers__connection\"", true);
                    userList = userList == null || userList.Count == 0 ? await PuppeteerBrowser.GetPaginationDataList("{\"users\":[{", true) : userList;
                    userDataList.AddRange(userList);
                    do
                    {
                        var Nodes = HtmlParseUtility.GetListNodesFromClassName(await PuppeteerBrowser.GetPageSourceAsync(), "_aaco");
                        if (lastScrollCount == Nodes?.Count)
                            break;
                        lastScrollCount = Nodes.Count;
                        var Index = Nodes != null && Nodes.Count > 0 ? Nodes.Count - 1 : 0;
                        await PuppeteerBrowser.BrowserActAsync(ActType.CustomActType,
                        AttributeType.ClassName,
                                                    "_aaco",
                                                    index: Index,
                                                    value: "scrollIntoViewIfNeeded();", delayAfter: 3);
                        userList = await PuppeteerBrowser.GetPaginationDataList("{\"xdt_api__v1__friendships__followers__connection\"", true);
                        userList = userList == null || userList.Count == 0 ? await PuppeteerBrowser.GetPaginationDataList("{\"users\":[{", true) : userList;
                        if (userList.Count > 0)
                        {
                            var lastItem = userList[userList.Count - 1];
                            var handle = new JsonHandler(lastItem);
                            maxID = handle.GetElementValue("next_max_id");
                            maxID = string.IsNullOrEmpty(maxID) ? handle.GetElementValue("data", "xdt_api__v1__friendships__followers__connection", "page_info", "end_cursor") : maxID;
                            hasMore = !string.IsNullOrEmpty(maxID);
                        }
                        else
                            hasMore = false;
                        userDataList.AddRange(userList);
                        pageSource = JsonConvert.SerializeObject(userDataList);
                        igResponseHandler = new FollowerAndFollowingIgResponseHandler(new ResponseParameter { Response = pageSource }, hasMoreResult: hasMore, IsCloseFriend: IsCloseFriend, maxID, CloseFriends);
                        listuser.AddRange(igResponseHandler.UsersList);
                        userDataList.Clear();
                        PuppeteerBrowser.ClearResources();
                    } while (hasMore && listuser.Count < 50);
                    PuppeteerBrowser.ClearResources();
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
                isRunning = false;
            });
            while (isRunning)
                Task.Delay(500, token).Wait();
            igResponseHandler.UsersList = listuser;
            return igResponseHandler;
        }

        public async Task<CloseFriendsResponseHandler> GetCloseFriendList(DominatorAccountModel dominatorAccount)
        {
            bool isRunning = true;
            string pageSource = string.Empty;
            bool hasMore = true;
            var listFriends = new List<string>();
            CloseFriendsResponseHandler closeFriendsResponse = null;
            await Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    if (PuppeteerBrowser is null || !await PuppeteerBrowser.IsBrowserOpenend())
                    {
                        PuppeteerBrowser = new PuppeteerBrowserActivity(dominatorAccount, isNeedResourceData: true);
                        await PuppeteerBrowser.LaunchBrowserAsync();
                    }
                    var IsExist = await PuppeteerBrowser.ChangeTabs(GramStatic.GetCloseFriendListUrl, false);
                    if (!IsExist)
                    {
                        await PuppeteerBrowser.ChangeTabs("https://www.instagram.com/accounts/edit/", true, true);
                        await Task.Delay(4000);
                        PuppeteerBrowser.ClearResources();
                        await PuppeteerBrowser.ExecuteScriptAsync("document.querySelector('a[href=\"/accounts/close_friends/\"]').click();", delayInSec: 6);
                    }
                    var list = await PuppeteerBrowser.GetPaginationDataList("\"data\":{\"initial_lispy\"", true);
                    pageSource = JsonConvert.SerializeObject(list.Select(x => x.Replace("for (;;);", "").Replace(@"\", "")).ToList());
                    closeFriendsResponse = new CloseFriendsResponseHandler(new ResponseParameter { Response = pageSource }, true);
                    var InnerHtml = HtmlParseUtility.GetListInnerHtmlFromPartialTagNamecontains(await PuppeteerBrowser.GetPageSourceAsync(), "div", "data-bloks-name", "bk.components.Collection");
                    var NodeCollection = HtmlParseUtility.GetListNodeFromPartialTagNamecontains(InnerHtml.FirstOrDefault(), "div", "role", "button");
                    var closedUsers = await GetClosedUsers(NodeCollection);
                    //if (closedUsers != null && closedUsers.Count > 0)
                    //    closeFriendsResponse.CloseFriendsList.RemoveAll(x => !closedUsers.Any(y => y == x));
                    listFriends.AddRange(closedUsers);
                    hasMore = closeFriendsResponse.HasMore;
                    var LastNodeCount = 0;
                    while (hasMore)
                    {
                        PuppeteerBrowser.ClearResources();
                        var Nodes = HtmlParseUtility.GetListNodeFromPartialTagNamecontains(await PuppeteerBrowser.GetPageSourceAsync(), "div", "data-bloks-name", "ig.components.Icon");
                        if (LastNodeCount == Nodes.Count)
                            break;
                        LastNodeCount = Nodes.Count;
                        var Index = Nodes.Count - 1;
                        await PuppeteerBrowser.ExecuteScriptAsync($"document.querySelectorAll('div[data-bloks-name=\"ig.components.Icon\"]')[{Index}].scrollIntoViewIfNeeded();", delayInSec: 3);
                        list = await PuppeteerBrowser.GetPaginationDataList("\"data\":{\"initial_lispy\"", true);
                        pageSource = JsonConvert.SerializeObject(list.Select(x => x.Replace("for (;;);", "").Replace(@"\", "")).ToList());
                        closeFriendsResponse = new CloseFriendsResponseHandler(new ResponseParameter { Response = pageSource }, true);
                        InnerHtml = HtmlParseUtility.GetListInnerHtmlFromPartialTagNamecontains(await PuppeteerBrowser.GetPageSourceAsync(), "div", "data-bloks-name", "bk.components.Collection");
                        NodeCollection = HtmlParseUtility.GetListNodeFromPartialTagNamecontains(InnerHtml.FirstOrDefault(), "div", "role", "button");
                        closedUsers = await GetClosedUsers(NodeCollection);
                        //if (closedUsers != null && closedUsers.Count > 0)
                        //    closeFriendsResponse.CloseFriendsList.RemoveAll(x => !closedUsers.Any(y => y == x));
                        listFriends.AddRange(closedUsers);
                        hasMore = closeFriendsResponse.HasMore;
                    }
                    await PuppeteerBrowser.ExecuteScriptAsync("document.querySelector('input[data-bloks-name=\"bk.components.TextInput\"]').scrollIntoViewIfNeeded();", delayInSec: 1);
                    await PuppeteerBrowser.ClosePageAsync("https://www.instagram.com/", "https://httpbin.org/ip");
                    listFriends = listFriends.Distinct().ToList();
                }
                catch { }
                isRunning = false;
            });
            while (isRunning)
                Task.Delay(500, dominatorAccount.Token).Wait();
            closeFriendsResponse.CloseFriendsList = listFriends;
            return closeFriendsResponse;
        }

        private async Task<List<string>> GetClosedUsers(List<HtmlAgilityPack.HtmlNode> nodeCollection)
        {
            var list = new List<string>();
            try
            {
                await Task.Run(() =>
                {
                    foreach (var node in nodeCollection)
                    {
                        var userName = HtmlParseUtility.GetListInnerTextFromPartialTagNamecontains(node.InnerHtml, "span", "data-bloks-name", "bk.components.Text")?.FirstOrDefault();
                        var CheckString = HtmlParseUtility.GetListInnerHtmlFromPartialTagNamecontains(node.InnerHtml, "div", "aria-label", "Toggle checkbox");
                        if (!string.IsNullOrEmpty(userName) && (!string.IsNullOrEmpty(CheckString?.FirstOrDefault()) && CheckString.FirstOrDefault().Contains("circle-check__filled")))
                            list.Add(userName);
                    }
                });
            }
            catch { }
            return list;
        }

        public async Task<string> VisitPage(DominatorAccountModel dominatorAccount, string Url)
        {
            var page = string.Empty;
            try
            {
                await BrowserWindow.GoToCustomUrl(Url, 5);
                page = await BrowserWindow.GetPageSourceAsync();
            }
            catch { }
            return page;
        }
    }
}