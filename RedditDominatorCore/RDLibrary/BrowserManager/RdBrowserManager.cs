using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.Publisher;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.PuppeteerBrowser;
using DominatorHouseCore.Request;
using DominatorHouseCore.Utility;
using EmbeddedBrowser;
using RedditDominatorCore.Interface;
using RedditDominatorCore.RDLibrary.BrowserManager.BrowserUtility;
using RedditDominatorCore.RDModel;
using RedditDominatorCore.RDUtility;
using RedditDominatorCore.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace RedditDominatorCore.RDLibrary.BrowserManager
{
    public interface IRdBrowserManager : IBrowserManager
    {
        //BrowserWindow BrowserWindow { get; set; }
        PuppeteerBrowserActivity BrowserWindow { get; set; }
        void CloseBrowser();

        IResponseParameter SearchByCustomUrl(DominatorAccountModel dominatorAccountModel, string queryValue,
            bool isOpenCustomWindow = false, bool IsEnabledSafeSearch = false);

        IResponseParameter ScrollWindowAndGetNextPageData(DominatorAccountModel account, string SearchString = "", bool IsContain = false);

        IResponseParameter ScrollWindowAndGetNextPageDataForCustomUrlScraper(DominatorAccountModel account,
            int delay = 10);

        string GetUrlFromQuery(ActivityType activityType, string queryValue);
        ActivityResposneHandler Follow(DominatorAccountModel accountModel, string Url = "");
        Dictionary<string, string> UnFollow();
        Tuple<bool, bool> Subscribe(string Url = "", DominatorAccountModel accountModel=null);
        bool UnSubscribe();
        ActivityResposneHandler Comment(DominatorAccountModel account, string commentText, string Id="");
        ActivityResposneHandler CommentInPublisherPost(DominatorAccountModel account, string commentText);
        bool Reply(DominatorAccountModel account, string commentText);
        bool EditComment(DominatorAccountModel account, string commentText);
        Tuple<bool, bool> UpVote(ActivityType activityType, string destinationUrl, bool UpvoteDownvoteComment = false, string id = "");
        bool DownVote(ActivityType activityType, string destinationUrl,string id="");
        bool RemoveVote(ActivityType activityType, RedditPost post=null);
        bool Publisher(DominatorAccountModel AccountModel, PublisherPostlistModel updatedPostDetails, string chooseCommunity,
            OtherConfigurationModel otherConfiguration, out string FailedMessage);
        IResponseParameter SearchByCustomUrlAndGetPageSource(DominatorAccountModel account, string url);
        void ClosePopUpWindow();
        IResponseParameter TryAndGetResponse(DominatorAccountModel dominatorAccount, string Url, int TryCount = 2, string ContainString = "", bool NewBrowser = false, bool InverseCheck = false, bool IsSafeSearchEnabled = false);
        Task<AutoActivityResponseHandler> ScrollFeedAndGetResponse(DominatorAccountModel dominatorAccount, string Url, bool OnlyScroll = false);
        void CheckBrowserLogin(DominatorAccountModel dominatorAccount);
        Task<bool> AcceptPendingMessageRequest(DominatorAccountModel dominatorAccount, RedditUser redditUser);
        ActivityResposneHandler BroadCastMessage(ActivityType activityType, DominatorAccountModel dominatorAccount, RedditUser redditUser);
    }

    public class RdBrowserManager : IRdBrowserManager
    {
        private CancellationToken _cancellationToken;
        //public BrowserWindow BrowserWindow { get; set; }
        public PuppeteerBrowserActivity BrowserWindow { get; set; }
        private readonly IRDAccountSessionManager sessionManager;
        public RdBrowserManager(IRDAccountSessionManager rDAccountSession)
        {
            sessionManager = rDAccountSession;
        }
        public bool BrowserLogin(DominatorAccountModel account, CancellationToken cancellationToken,
            LoginType loginType = LoginType.AutomationLogin, VerificationType verificationType = 0)
        {
            sessionManager.AddOrUpdateSession(ref account);
            _cancellationToken = cancellationToken;
            var isRunning = true;
            Application.Current.Dispatcher.Invoke(async () =>
            {
                //                BrowserWindow = new BrowserWindow(account, targetUrl: RdConstants.NewRedditHomePageAPI, isNeedResourceData: false)
                //                {
                //                    Visibility = Visibility.Hidden
                //                };
                //                BrowserWindow.SetCookie();

                //#if DEBUG
                //                BrowserWindow.Visibility = Visibility.Visible;
                //#endif
                //                if (loginType == LoginType.BrowserLogin)
                //                    BrowserWindow.Visibility = Visibility.Visible;

                //                BrowserWindow.Show();

                #region Code for Run Through Puppeteer
                var HeadLess = true;
#if DEBUG
                HeadLess = false;
#endif
                if (loginType == LoginType.BrowserLogin)
                    HeadLess = false;
                BrowserWindow = new PuppeteerBrowserActivity(account, isNeedResourceData: true, loginType: loginType);
                var BrowserLaunched = await BrowserWindow.LaunchBrowserAsync(HeadLess);
                isRunning = false;
                #endregion
            });
            while(isRunning)  Sleep(2);
            Sleep(2);
            var last2Min = DateTime.Now.AddMinutes(1);
            if (!BrowserWindow.IsLoggedIn )
            {
                _cancellationToken.ThrowIfCancellationRequested();
                while (!BrowserWindow.IsLoaded && last2Min >= DateTime.Now)
                {
                    Sleep();
                    continue;
                }
                Sleep(4);
                var PageSource = BrowserWindow.GetPageSource();
                //account.Token.ThrowIfCancellationRequested();
                if (last2Min.AddMinutes(3.5) > DateTime.Now)
                {
                    if (!BrowserWindow.IsLoggedIn && PageSource.Contains("loginUsername"))

                    {
                        BrowserWindow.BrowserAct(ActType.EnterValueById, "loginUsername", 3, 1,
                            account.AccountBaseModel.UserName);
                        BrowserWindow.BrowserAct(ActType.EnterValueById, "loginPassword", 0, 1,
                            account.AccountBaseModel.Password);
                        BrowserWindow.BrowserAct(ActType.ClickByClass, "AnimatedForm__submitButton", delayAfter: 4);
                    }
                    if (!PageSource.Contains("By continuing, you agree to our"))
                    {
                        BrowserWindow.ExecuteScript("document.getElementById('login-button').click();", delayInSec: 6);
                        PageSource = BrowserWindow.GetPageSource();
                    }
                    if (!BrowserWindow.IsLoggedIn && (PageSource.Contains("By continuing, you agree to our")|| PageSource.Contains("username-field") || PageSource.Contains("login-username-container")))
                    {
                        Sleep(5);
                        var X = BrowserWindow.ExecuteScript("document.querySelector('faceplate-text-input[name=\"username\"]').getBoundingClientRect().x.toString()", delayInSec: 2).Result;
                        var Y = BrowserWindow.ExecuteScript("document.querySelector('faceplate-text-input[name=\"username\"]').getBoundingClientRect().y.toString()", delayInSec: 2).Result;
                        BrowserWindow.MouseClick((int)Convert.ToDouble(X) + 5, (int)Convert.ToDouble(Y) + 5, delayBefore:2, delayAfter: 2);
                        BrowserWindow.EnterChars(account.AccountBaseModel.UserName, typingDelay: 0.25, delayAtLast: 3);
                        X = BrowserWindow.ExecuteScript("document.querySelector('faceplate-text-input[name=\"password\"]').getBoundingClientRect().x.toString()", delayInSec: 2).Result;
                        Y = BrowserWindow.ExecuteScript("document.querySelector('faceplate-text-input[name=\"password\"]').getBoundingClientRect().y.toString()", delayInSec: 2).Result;
                        BrowserWindow.MouseClick((int)Convert.ToDouble(X) + 5, (int)Convert.ToDouble(Y) + 5, delayAfter: 2);
                        BrowserWindow.EnterChars(account.AccountBaseModel.Password, typingDelay: 0.25, delayAtLast: 3);
                        //BrowserWindow.ExecuteScript("document.querySelector(\"body>shreddit-app>shreddit-overlay-display\").shadowRoot.querySelector(\"shreddit-signup-drawer\").shadowRoot.querySelector(\"shreddit-drawer>div>shreddit-async-loader>div>shreddit-slotter\").shadowRoot.querySelector(\"#login>faceplate-tabpanel>auth-flow-modal:nth-child(1)>div.w-100>faceplate-tracker>button\").click();");
                        //BrowserWindow.ExecuteScript("document.querySelector(\"shreddit-overlay-display\").shadowRoot.querySelector(\"shreddit-signup-drawer\").shadowRoot.querySelector(\"shreddit-async-loader>div>shreddit-slotter\").shadowRoot.querySelector(\"div.w-100>faceplate-tracker>button\").click();");
                        //BrowserWindow.ExecuteScript("[...document.querySelectorAll('faceplate-tracker[source=\\\"onboarding\\\"] > button[type=\"button\"]')].find(x=>x.innerText===\"Log In\").click();");
                        
                        isRunning = true;
                        Application.Current.Dispatcher.Invoke(async ()=>
                        {
                            await BrowserWindow.PressAnyKeyUpdated();
                            isRunning = false;
                        });
                        while (isRunning) Sleep(2);
                        Sleep(8);
                        //X = BrowserWindow.ExecuteScript("[...document.querySelectorAll('faceplate-tracker[source=\\\"onboarding\\\"] > button[type=\"button\"]')].find(x=>x.innerText===\"Log In\").getBoundingClientRect().x.toString()", delayInSec: 2).Result;
                        //Y = BrowserWindow.ExecuteScript("[...document.querySelectorAll('faceplate-tracker[source=\\\"onboarding\\\"] > button[type=\"button\"]')].find(x=>x.innerText===\"Log In\").getBoundingClientRect().y.toString()", delayInSec: 2).Result;
                        //BrowserWindow.MouseClick((int)Convert.ToDouble(X) + 25, (int)Convert.ToDouble(Y) + 15, delayAfter: 2);
                        //Sleep(8);

                    }
                }

                if (!BrowserWindow.IsLoggedIn)
                {
                    PageSource = BrowserWindow.GetPageSource();
                    var ErrorMessage = BrowserWindow.ExecuteScript("document.querySelector(\"#login-password\").shadowRoot.querySelector(\"faceplate-form-helper-text\").shadowRoot.querySelector(\"#helper-text\").innerText;").Result?.ToString();
                    if (PageSource.Contains("Incorrect password") || PageSource.Contains("Incorrect username or password") || ErrorMessage == "incorrect username or password" || ErrorMessage == "Invalid username or password.")
                    {
                        account.AccountBaseModel.Status = AccountStatus.InvalidCredentials;
                        return false;
                    }
                    //if (!BrowserWindow.CurrentUrl().Contains("www.reddit.com"))
                    //{
                    //    BrowserWindow.GoToUrl(RdConstants.NewRedditHomePageAPI);
                    //    Sleep(8);
                    //}
                    Sleep(2);
                    PageSource = BrowserWindow.GetPageSource();
                    
                    //if (PageSource.Contains("Popular "))
                    //{
                    //   PageSource= BrowserWindow.GoToCustomUrl($"{RdConstants.NewRedditHomePageAPI}/explore/").Result;
                    //    Sleep(2);
                    //   BrowserWindow.ExecuteScript("document.getElementsByClassName(\"_3zbhtNO0bdck0oYbYRhjMC HNozj_dKjQZ59ZsfEegz8\")[2].click();",2);
                    //}
                    if (!string.IsNullOrEmpty(PageSource) && !PageSource.Contains("Log In") &&
                       (PageSource.ToLower().Contains(account.AccountBaseModel.UserName.ToLower().Trim()) ||
                         PageSource.Contains("Log out") || PageSource.Contains("logged in") || 
                         PageSource.Contains("user-logged-in=\"true\"") || PageSource.Contains("is-logged-in-user=\"true\"") || 
                         PageSource.Contains("id=\"USER_DROPDOWN_ID\"") || PageSource.Contains("User account menu")
                         || PageSource.Contains("is-logged-in=\"true\"")))
                    {
                        Sleep(2);
                        BrowserWindow.SaveCookies(account.IsRunProcessThroughBrowser);
                        Sleep(2);
                        UpdateSession(ref account);
                        Sleep(3);
                        sessionManager.AddOrUpdateSession(ref account, true);
                        using (var globalDbOperation = new DbOperations(SocinatorInitialize.GetGlobalDatabase().GetSqlConnection()))
                        {
                            globalDbOperation.UpdateAccountDetails(account);
                        }
                    }
                    else if (loginType != LoginType.BrowserLogin && !string.IsNullOrEmpty(PageSource) 
                        && !PageSource.Contains("user-logged-in=\"true") 
                        && !PageSource.Contains("is-logged-in=\"true\"")
                        && !PageSource.ToLower().Contains(account.AccountBaseModel.UserName.ToLower().Trim()) 
                        && !PageSource.Contains("is-logged-in-user=\"true\""))
                    {
                        account.AccountBaseModel.Status = AccountStatus.Failed;
                        if (BrowserWindow != null)
                            CloseBrowser();
                        return false;
                    }

                }
                Sleep(2);

                if (account.IsUserLoggedIn)
                {
                    var response = BrowserWindow.GetPageSource();
                    if (!string.IsNullOrEmpty(response) && response.Contains(RdConstants.SuspendedMessage))
                        account.AccountBaseModel.Status = AccountStatus.TemporarilyBlocked;
                    else if (!string.IsNullOrEmpty(response) && response.Contains(RdConstants.PermanentlyBanned))
                        account.AccountBaseModel.Status = AccountStatus.PermanentlyBlocked;
                    else if (!string.IsNullOrEmpty(response) && response.Contains(RdConstants.LockedMessage))
                        account.AccountBaseModel.Status = AccountStatus.TemporarilyBlocked;
                }

            }

            return account.IsUserLoggedIn;
        }

        private void UpdateSession(ref DominatorAccountModel account)
        {
            try
            {
                if (account != null)
                {
                    var Session = account?.Cookies?.Cast<Cookie>().FirstOrDefault(x => x.Name == "reddit_session");
                    if (string.IsNullOrEmpty(Session?.Value))
                    {
                        var Rsession = BrowserWindow.DominatorAccountModel?.Cookies?.Cast<Cookie>()?.FirstOrDefault(x => x.Name == "reddit_session");
                        account.Cookies.Add(Rsession);
                    }
                }
            }
            catch { }
        }

        public void CloseBrowser()
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
                        GC.WaitForPendingFinalizers();
                        GC.Collect();
                    });
            }
            catch (Exception)
            {
                // ignored
            }
        }

        /// <summary>
        ///     To do cancellationtoken
        /// </summary>
        /// <param name="account"></param>
        /// <param name="url"></param>
        /// <param name="isOpenCustomWindow"></param>
        /// <returns></returns>
        public IResponseParameter SearchByCustomUrl(DominatorAccountModel account, string url,
            bool isOpenCustomWindow = false, bool IsEnabledSafeSearch = false)
        {
            var isRunning = true;
            CheckBrowserLogin(account);
            var ResourceResponse = string.Empty;
            var response = new ResponseParameter();
            Application.Current.Dispatcher.Invoke(async () =>
            {
                BrowserWindow.ClearResources();
                BrowserWindow.GoToUrl(url);
                await Task.Delay(TimeSpan.FromSeconds(8));
                if (IsEnabledSafeSearch)
                    await BrowserWindow.BrowserActAsync(ActType.ClickById, AttributeType.Id, "safe-search-toggle", "", 0, 5);
                ResourceResponse = await BrowserWindow.GetPageSourceAsync();
                //ResourceResponse = await CheckWindowLoaded(ResourceResponse, url);
                isRunning = false;
            });
            while (isRunning) Sleep(2);
            ClosePopUpWindow();
            return new ResponseParameter { Response = ResourceResponse };
        }

        private async Task<string> CheckWindowLoaded(string ResourceResponse, string url)
        {
            var i = 0;
            while (ResourceResponse != null && ++i <= 5)
            {
                BrowserWindow.ClearResources();
                BrowserWindow.GoToUrl(url);
                await Task.Delay(TimeSpan.FromSeconds(8));
                ResourceResponse = await BrowserWindow.GetPageSourceAsync(4);
            }
            return ResourceResponse;
        }

        public IResponseParameter ScrollWindowAndGetNextPageData(DominatorAccountModel account, string SearchString = "", bool IsContain = false)
        {
            var isRunning = true;
            var response = new ResponseParameter();
            SearchString = string.IsNullOrEmpty(SearchString) ? "{\"viewTreatment\"" : SearchString;
            BrowserWindow.ClearResources();
            Application.Current.Dispatcher.Invoke(async () =>
            {
                await BrowserWindow.BrowserActAsync(ActType.ScrollWindow, AttributeType.Null, "", delayBefore: 5,
                    delayAfter: 5, scrollByPixel: 3000);
                response.Response = await BrowserWindow.GetPaginationData(SearchString, IsContain);
                var i = 0;
                while (!response.Response.Contains(SearchString) && ++i <= 5)
                {
                    response.Response = await BrowserWindow.GetPaginationData(SearchString);
                    await Task.Delay(3000);
                }

                isRunning = false;
            });
            while (isRunning) Sleep(2);
            return response;
        }

        public IResponseParameter ScrollWindowAndGetNextPageDataForCustomUrlScraper(DominatorAccountModel account,
            int delay = 10)
        {
            var isRunning = true;
            var response = new ResponseParameter();
            BrowserWindow.ClearResources();
            Application.Current.Dispatcher.Invoke(async () =>
            {
                await BrowserWindow.BrowserActAsync(ActType.ScrollWindow, AttributeType.Null, "", delayBefore: delay,
                    delayAfter: 5, scrollByPixel: 30000);
                response.Response = await BrowserWindow.GetPaginationData("{\"subredditPermissions\":");
                var i = 0;
                while (!response.Response.Contains("{\"subredditPermissions\":") && ++i <= 5)
                {
                    response.Response = await BrowserWindow.GetPaginationData("{\"subredditPermissions\":");
                    await Task.Delay(TimeSpan.FromSeconds(2));
                }

                isRunning = false;
            });
            while (isRunning) Sleep(2);
            return response;
        }
        public string GetUrlFromQuery(ActivityType activityType, string queryValue)
        {
            switch (activityType)
            {
                case ActivityType.Subscribe:
                    return GetUrlForSubscribeAndUnSubscribe(queryValue);
                case ActivityType.UnSubscribe:
                    return GetUrlForSubscribeAndUnSubscribe(queryValue);
            }

            return queryValue;
        }

        public ActivityResposneHandler Follow(DominatorAccountModel accountModel, string Url = "")
        {
            var isRunning = true;
            var followPageSource = string.Empty;
            var FollowResponse = ActivityResposneHandler.GetInstance;
            var followClass = "_2q1wcTx60QKM_bQ1Maev7b _2iuoyPiKHN3kfOoeIQalDT _10BQ7pjWbeYP63SAPNS8Ts HNozj_dKjQZ59ZsfEegz8 ";
            CheckBrowserLogin(accountModel);
            BrowserWindow.ClearResources();
            Application.Current.Dispatcher.Invoke(async () =>
            {
                if (!string.IsNullOrEmpty(Url))
                    BrowserWindow.GoToUrl(Url);
                await Task.Delay(TimeSpan.FromSeconds(5));
                ClosePopUpWindow();
                followPageSource = await BrowserWindow.GetPageSourceAsync();
                await Task.Delay(TimeSpan.FromSeconds(5));
                if (followPageSource.Contains(RdConstants.UserSuspendedArticleUrl))
                    FollowResponse.ResponseMessage = "This  account has been suspended";
                //var Nodes1 = HtmlUtility.GetListInnerTextFromClassName(followPageSource, followClass);
                var Nodes = HtmlUtility.GetListOfNodesFromTagName(followPageSource,"aside", "aria-label", "Profile information");
                var followIndex = Nodes.Count > 0 ? Nodes.IndexOf(Nodes.FirstOrDefault(x => x.InnerText.Contains("Follow"))) : 0;
                if (followIndex < 0)
                    FollowResponse.ResponseMessage = "Can't Follow This User,Because There Is No Option To Follow.";
                //await BrowserWindow.BrowserActAsync(ActType.Click, AttributeType.ClassName, followClass, index: followIndex,
                //    delayBefore: 2, delayAfter: 2);
                await BrowserWindow.ExecuteScriptAsync($"document.getElementsByTagName(\"follow-button\")[0].click();", 2);
                followPageSource = await BrowserWindow.GetPageSourceAsync();
                isRunning = false;
            });
            while (isRunning) Sleep(2);
            FollowResponse.Status = !string.IsNullOrEmpty(BrowserUtilities.GetPath(followPageSource, "button", "Unfollow"));
            return FollowResponse;
        }

        public Dictionary<string, string> UnFollow()
        {
            var isRunning = true;
            var unfollowPageSource = string.Empty;
            var FinalResponse = new Dictionary<string, string>();
            var IsFollowing = true;
            BrowserWindow.ClearResources();
            Application.Current.Dispatcher.Invoke(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(8));
                unfollowPageSource = await BrowserWindow.GetPageSourceAsync();
                IsFollowing = unfollowPageSource.Contains("is-followed");
                if (!IsFollowing)
                    goto Skip;
                await BrowserWindow.ExecuteScriptAsync($"document.getElementsByTagName(\"follow-button\")[0].click();", 2);
                unfollowPageSource = await BrowserWindow.GetPageSourceAsync();
            Skip:
                isRunning = false;
            });
            while (isRunning) Sleep(2);
            FinalResponse.Add("UnFollow", (!IsFollowing) ? "Already UnFollow or You Are Not Following This User." : "");
            return FinalResponse;
        }

        public Tuple<bool, bool> Subscribe(string Url = "", DominatorAccountModel accountModel = null)
        {
            var isRunning = true;
            if(BrowserWindow ==null)
                 CheckBrowserLogin(accountModel);
            var isAlreadySubscribed = false;
            var SuccessfullyUpvoted = false;
            BrowserWindow.ClearResources();
            Application.Current.Dispatcher.Invoke(async () =>
            {
                if (!string.IsNullOrEmpty(Url))
                    BrowserWindow.GoToUrl(Url);
                await Task.Delay(TimeSpan.FromSeconds(8));
                if (bool.TryParse(( await BrowserWindow.ExecuteScriptAsync($"document.querySelectorAll(\"shreddit-subreddit-header-buttons\")[0].shadowRoot.querySelectorAll(\"shreddit-join-button\")[0].subscribed;")).Result.ToString(),out isAlreadySubscribed) && isAlreadySubscribed)
                {
                    goto Exit;
                }
                await BrowserWindow.ExecuteScriptAsync($"document.querySelector(\"shreddit-subreddit-header-buttons\").shadowRoot.querySelector(\"shreddit-join-button\").shadowRoot.querySelector(\"button\").click()");
                bool.TryParse((await BrowserWindow.ExecuteScriptAsync($"document.querySelectorAll(\"shreddit-subreddit-header-buttons\")[0].shadowRoot.querySelectorAll(\"shreddit-join-button\")[0].subscribed;")).Result.ToString(), out SuccessfullyUpvoted);
            Exit:
                isRunning = false;
            });

            while (isRunning) Sleep(2);
            return new Tuple<bool, bool>(SuccessfullyUpvoted, isAlreadySubscribed);
        }

        public bool UnSubscribe()
        {
            var isRunning = true;
            var isAlreadyUnSubscribed = true;
            BrowserWindow.ClearResources();
            Application.Current.Dispatcher.Invoke(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(8));
                bool.TryParse((await BrowserWindow.ExecuteScriptAsync($"document.querySelectorAll(\"shreddit-subreddit-header-buttons\")[0].shadowRoot.querySelectorAll(\"shreddit-join-button\")[0].subscribed;")).Result.ToString(), out isAlreadyUnSubscribed);
                if (!isAlreadyUnSubscribed)
                    goto Exit;
                await BrowserWindow.ExecuteScriptAsync($"document.querySelector(\"shreddit-subreddit-header-buttons\").shadowRoot.querySelector(\"shreddit-join-button\").shadowRoot.querySelector(\"button\").click()");
                bool.TryParse((await BrowserWindow.ExecuteScriptAsync($"document.querySelectorAll(\"shreddit-subreddit-header-buttons\")[0].shadowRoot.querySelectorAll(\"shreddit-join-button\")[0].subscribed;")).Result.ToString(), out isAlreadyUnSubscribed);
            Exit:
                isRunning = false;
            });
            while (isRunning) Sleep(2);
            return !isAlreadyUnSubscribed;
        }

        public ActivityResposneHandler Comment(DominatorAccountModel account, string commentText,string Id="")
        {
            var isRunning = true;
            var attributeValue = string.Empty;
            var commentPageSource = string.Empty;
            var commentCount = 0;
            var ErrorResponse = string.Empty;
            var Response = ActivityResposneHandler.GetInstance;
            BrowserWindow.ClearResources();
            Application.Current.Dispatcher.Invoke(async () =>
            {
                int.TryParse(( await BrowserWindow.ExecuteScriptAsync($"document.querySelector(\"#{Id}\").shadowRoot.querySelector(\"button[name='comments-action-button']\").querySelector(\"faceplate-number\").getAttribute(\"number\");")).Result.ToString(),out commentCount);
                await BrowserWindow.ExecuteScriptAsync($"document.querySelectorAll(\"[bundlename='comment_composer']\")[0].scrollIntoViewIfNeeded();");
                var xyForCommentBox=await BrowserWindow.GetXAndYAsync(customScriptX: $"document.querySelector(\"[noun='add_comment_button']\").getBoundingClientRect().left", customScriptY: $"document.querySelector(\"[noun='add_comment_button']\").getBoundingClientRect().top");
                await BrowserWindow.MouseClickAsync(xyForCommentBox.Key + 15, xyForCommentBox.Value +2, 2,5);
                await BrowserWindow.EnterCharsAsync(" " + commentText, delayBefore: 3, delayAtLast: 3);
                BrowserWindow.ClearResources();
                xyForCommentBox = await BrowserWindow.GetXAndYAsync(customScriptX: $"document.querySelectorAll(\"button[slot='submit-button']\")[0].getBoundingClientRect().left", customScriptY: $"document.querySelectorAll(\"button[slot='submit-button']\")[0].getBoundingClientRect().top");
                await BrowserWindow.MouseClickAsync(xyForCommentBox.Key + 15, xyForCommentBox.Value + 2, 2, 5);
                await Task.Delay(TimeSpan.FromSeconds(5));
                BrowserWindow.Refresh();
                await Task.Delay(TimeSpan.FromSeconds(5));
                commentPageSource = await BrowserWindow.GetPageSourceAsync();
                int.TryParse((await BrowserWindow.ExecuteScriptAsync($"document.querySelector(\"#{Id}\").shadowRoot.querySelector(\"button[name='comments-action-button']\").querySelector(\"faceplate-number\").getAttribute(\"number\");")).Result.ToString(), out int updatedCount);
                Response.Status = updatedCount>commentCount;
                isRunning = false;
            });
            while (isRunning) Sleep(2);
            return Response;
        }
        public ActivityResposneHandler CommentInPublisherPost(DominatorAccountModel account, string commentText)
        {
            var isRunning = true;
            var attributeValue = string.Empty;
            var commentPageSource = string.Empty;
            var ErrorResponse = string.Empty;
            var Response = ActivityResposneHandler.GetInstance;
            BrowserWindow.ClearResources();
            Application.Current.Dispatcher.Invoke(async () =>
            {
                var url = BrowserWindow.CurrentUrl();
                await BrowserWindow.BrowserActAsync(ActType.ScrollIntoView, AttributeType.ClassName,
                    "px-md py-sm outline-none relative resize-y overflow-y-scroll min-h-[44px] leading-5 box-border", delayBefore: 2, delayAfter: 1);
                commentPageSource = await BrowserWindow.GetPageSourceAsync();
                int.TryParse(Utilities.GetBetween(commentPageSource, "comment-count=\"", "\""), out int BeforecommentCount);
                var xyForCommentBox = BrowserWindow.GetXAndY(AttributeType.ClassName, "disabled:cursor-not-allowed cursor-text disabled:opacity-50 w-full text-neutral-content-weak text-14 leading-6 font-normal px-md py-xs bg-neutral-background border-neutral-border border-solid border tracking-tight text-left h-auto", 1);
                await BrowserWindow.MouseClickAsync(xyForCommentBox.Key + 5, xyForCommentBox.Value + 10, 1, 5);
                await BrowserWindow.EnterCharsAsync(" " + commentText, delayBefore: 3, delayAtLast: 3);
                commentPageSource = await BrowserWindow.GetPageSourceAsync();
                attributeValue = BrowserUtilities.GetPath(commentPageSource, "button", "submit");
                var xyForComment = BrowserWindow.GetXAndY(AttributeType.ClassName, string.IsNullOrEmpty(attributeValue) ? "relative button-small px-[var(--rem10)] button-primary button inline-flex items-center justify-center cursor-pointer" : attributeValue, 0);
                await BrowserWindow.MouseClickAsync(xyForComment.Key + 5, xyForComment.Value + 5, 2, 1);
                await Task.Delay(TimeSpan.FromSeconds(5));
                //       commentPageSource = await BrowserWindow.GetPageSourceAsync();
                await BrowserWindow.GoToCustomUrl(url); Sleep(10);
                commentPageSource = await BrowserWindow.GetPageSourceAsync();
                int.TryParse(Utilities.GetBetween(commentPageSource, "comment-count=\"", "\""), out int AftercommentCount);
                Response.Status = (BeforecommentCount < AftercommentCount) ? true : commentPageSource.Contains("Unable to create comment") || commentPageSource.Contains("We had a server error");
                isRunning = false;
            });
            while (isRunning) Sleep(2);
            if (!Response.Status)
            {
                var CommentNode = HtmlUtility.GetListInnerTextFromClassName(commentPageSource, "py-0 xs:mx-xs mx-2xs inline-block max-w-full");
                var Status = CommentNode.Count > 0 ? CommentNode.Any(Node => Node.Contains(commentText.Trim()) || string.Equals(Node, commentText.Trim())) : false;
                Response.Status = Status;
            }
            return Response;
        }
        public bool Reply(DominatorAccountModel account, string commentText)
        {
            var isRunning = true;
            var attributeValue = string.Empty;
            var replyPageSource = string.Empty;
            BrowserWindow.ClearResources();
            Application.Current.Dispatcher.Invoke(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(5));
                replyPageSource = await BrowserWindow.GetPageSourceAsync();
                attributeValue = BrowserUtilities.GetPath(replyPageSource, "button", "Reply");
                await BrowserWindow.BrowserActAsync(ActType.Click, AttributeType.ClassName, attributeValue,
                    delayBefore: 2, delayAfter: 2);
                replyPageSource = await BrowserWindow.GetPageSourceAsync();

                while (!replyPageSource.Contains("notranslate public-DraftEditor-content"))
                {
                    await Task.Delay(TimeSpan.FromSeconds(3));
                    replyPageSource = await BrowserWindow.GetPageSourceAsync();
                    attributeValue = BrowserUtilities.GetPath(replyPageSource, "button", "Reply");
                    await BrowserWindow.BrowserActAsync(ActType.Click, AttributeType.ClassName, attributeValue,
                        delayBefore: 2, delayAfter: 2);
                    replyPageSource = await BrowserWindow.GetPageSourceAsync();
                }

                await BrowserWindow.BrowserActAsync(ActType.ScrollIntoView, AttributeType.ClassName,
                    "notranslate public-DraftEditor-content", delayBefore: 2, delayAfter: 1);
                await BrowserWindow.BrowserActAsync(ActType.ScrollWindow, AttributeType.Null, "", delayBefore: 2,
                    delayAfter: 1, scrollByPixel: -150);
                replyPageSource = await BrowserWindow.GetPageSourceAsync();
                ClosePopUpWindow();
                var xyForCommentBox =
                    BrowserWindow.GetXAndY(AttributeType.ClassName, "notranslate public-DraftEditor-content");
                await BrowserWindow.MouseClickAsync(xyForCommentBox.Key + 5, xyForCommentBox.Value + 5,
                    2, 1);
                await BrowserWindow.EnterCharsAsync(commentText, 0.3, delayAtLast: 2);
                replyPageSource = await BrowserWindow.GetPageSourceAsync();
                var xyForreplyBox =
                     BrowserWindow.GetXAndY(AttributeType.ClassName, "_22S4OsoDdOqiM-hPTeOURa _2iuoyPiKHN3kfOoeIQalDT _10BQ7pjWbeYP63SAPNS8Ts _3uJP0daPEH2plzVEYyTdaH ", 0);
                await BrowserWindow.MouseClickAsync(xyForreplyBox.Key + 5, xyForreplyBox.Value + 5,
                    2, 1);
                await Task.Delay(TimeSpan.FromSeconds(5));
                var containText = replyPageSource.StartsWith("https://www.reddit.com/") ? "submit" : "Reply";
                await Task.Delay(TimeSpan.FromSeconds(5));
                replyPageSource = await BrowserWindow.GetPageSourceAsync();
                isRunning = false;
            });
            while (isRunning) Sleep(2);
            var firstSplitedResp = Regex.Split(replyPageSource, "name=\"Comment\"")[0];
            return firstSplitedResp.Contains("_374Hkkigy4E4srsI2WktEd _2hr3tRWszeMRQ0u_Whs7t8 _14hLFU5cIJCyxH9DSgsCov");
        }

        public bool EditComment(DominatorAccountModel account, string commentText)
        {
            var isRunning = true;
            var editCommentPageSource = string.Empty;
            BrowserWindow.ClearResources();
            Application.Current.Dispatcher.Invoke(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(5));
                editCommentPageSource = await BrowserWindow.GetPageSourceAsync();
                var editButtonAttributeValue = BrowserUtilities.GetPath(editCommentPageSource, "button", "Edit");

                await BrowserWindow.BrowserActAsync(ActType.Click, AttributeType.ClassName, "_374Hkkigy4E4srsI2WktEd _2hr3tRWszeMRQ0u_Whs7t8 _14hLFU5cIJCyxH9DSgsCov",
                    index: 1, delayBefore: 2, delayAfter: 2);
                editCommentPageSource = await BrowserWindow.GetPageSourceAsync();

                while (!editCommentPageSource.Contains("notranslate public-DraftEditor-content"))
                {
                    await Task.Delay(TimeSpan.FromSeconds(5));
                    editCommentPageSource = await BrowserWindow.GetPageSourceAsync();
                    editButtonAttributeValue = BrowserUtilities.GetPath(editCommentPageSource, "button", "Edit");

                    await BrowserWindow.BrowserActAsync(ActType.Click, AttributeType.ClassName,
                        editButtonAttributeValue,
                        index: 1, delayBefore: 2, delayAfter: 2);
                    editCommentPageSource = await BrowserWindow.GetPageSourceAsync();
                }

                await BrowserWindow.BrowserActAsync(ActType.ScrollIntoView, AttributeType.ClassName,
                    "notranslate public-DraftEditor-content", delayBefore: 2, delayAfter: 1);
                await BrowserWindow.BrowserActAsync(ActType.ScrollWindow, AttributeType.Null, "", delayBefore: 2,
                    delayAfter: 1,
                    scrollByPixel: -150);
                var postTextLocStart = BrowserWindow.GetXAndY(AttributeType.ClassName, "notranslate public-DraftEditor-content");
                await BrowserWindow.MouseClickAsync(postTextLocStart.Key + 2, postTextLocStart.Value + 2, delayAfter: 1);
                await BrowserWindow.PressAnyKeyUpdated(winKeyCode: 40, isShiftDown: true);
                await BrowserWindow.EnterCharsAsync($" {commentText}", .05, delayAtLast: 2, delayBefore: 3);

                editCommentPageSource = await BrowserWindow.GetPageSourceAsync();
                var saveEditButtonAttributeValue =
                    BrowserUtilities.GetPath(editCommentPageSource, "button", "Save Edits");
                await BrowserWindow.BrowserActAsync(ActType.Click, AttributeType.ClassName,
                    saveEditButtonAttributeValue,
                    delayBefore: 1, delayAfter: 5);
                editCommentPageSource = await BrowserWindow.GetPageSourceAsync();
                isRunning = false;
            });
            while (isRunning) Sleep(2);
            return editCommentPageSource.Contains(account.AccountBaseModel.UserName) && editCommentPageSource.Contains(commentText);
        }

        public Tuple<bool, bool> UpVote(ActivityType activityType, string destinationUrl, bool UpvoteDownvoteComment = false, string id="")
        {
            destinationUrl = Utils.UpdateDomain(destinationUrl);
            var isRunning = true;
            var mainAttributeValue = string.Empty;
            var attributeValue = string.Empty;
            var Success = false;
            var Downvoted = false;
            var isAlreadyUpvoted = false;
            BrowserWindow.ClearResources();
            switch (activityType)
            {
                case ActivityType.Upvote:

                    var upVotePageSource = string.Empty;
                    Application.Current.Dispatcher.Invoke(async () =>
                    {
                        BrowserWindow.GoToUrl(destinationUrl);
                        await Task.Delay(TimeSpan.FromSeconds(8));
                        if (UpvoteDownvoteComment)
                        {
                            var currentScroll = 0;
                            var ScrollCount = RandomUtilties.GetRandomNumber(30, 5);
                            var lastCount = 0;
                        ScrollMore:
                            var Nodes = HtmlParseUtility.GetListInnerHtmlFromPartialTagName(await BrowserWindow.GetPageSourceAsync(), "button", "aria-label", "upvote");
                            var Index = Nodes.Count > 0 ? Nodes.IndexOf(Nodes[RandomUtilties.GetRandomNumber(Nodes.Count, 1)]) : 1;
                            while (currentScroll++ <= ScrollCount && lastCount != Nodes.Count)
                            {
                                lastCount = Nodes.Count;
                                await BrowserWindow.ExecuteScriptAsync($"document.querySelectorAll('button[data-click-id=\"upvote\"]')[{Nodes.Count - 1}].scrollIntoViewIfNeeded();", 4);
                                goto ScrollMore;
                            }
                            await BrowserWindow.ExecuteScriptAsync($"document.querySelectorAll('button[data-click-id=\"upvote\"]')[{Index}].scrollIntoViewIfNeeded();", 2);
                            var status = await BrowserWindow.ExecuteScriptAsync($"document.querySelectorAll('button[data-click-id=\"upvote\"]')[{Index}].ariaPressed", 2);
                            bool.TryParse(status.Result.ToString(), out bool upvoted);
                            if (status.Success && upvoted)
                            {
                                var downvote = await BrowserWindow.ExecuteScriptAsync($"document.querySelectorAll('button[data-click-id=\"downvote\"]')[{Index}].click();", 2);
                                Downvoted = downvote.Success;
                            }
                            else
                            {
                                var upvote = await BrowserWindow.ExecuteScriptAsync($"document.querySelectorAll('button[data-click-id=\"upvote\"]')[{Index}].click();", 2);
                                Success = upvote.Success;
                            }
                            await Task.Delay(TimeSpan.FromSeconds(5));
                            BrowserWindow.GoToUrl(destinationUrl);
                            await Task.Delay(TimeSpan.FromSeconds(5));
                            upVotePageSource = await BrowserWindow.GetPageSourceAsync();
                            var i = 1;
                            while (Success || Downvoted ? false : !CheckVoteStatus(upVotePageSource, ActivityType.Upvote, "div", "class", mainAttributeValue,id:id) && i++ <= 3)
                            {
                                await Task.Delay(TimeSpan.FromSeconds(5));
                                BrowserWindow.GoToUrl();
                                await Task.Delay(TimeSpan.FromSeconds(5));
                                upVotePageSource = await BrowserWindow.GetPageSourceAsync();
                            }
                            isRunning = false;
                        }
                        else
                        {
                            var count = 0;
                            bool.TryParse((await BrowserWindow.ExecuteScriptAsync($"document.querySelectorAll(\"#{id}\")[0].shadowRoot.querySelectorAll(\"button\")[0].ariaPressed;")).Result.ToString(), out isAlreadyUpvoted);
                            if (isAlreadyUpvoted)
                                goto Skip;
                        TryAgain:
                            var response = await BrowserWindow.ExecuteScriptAsync($"document.querySelectorAll(\"#{id}\")[0].shadowRoot.querySelectorAll(\"button\")[0].click();", 3);
                            await Task.Delay(TimeSpan.FromSeconds(5));
                            if (!response.Success && count++ < 3) goto TryAgain;
                            Success = response.Success;
                            bool.TryParse((await BrowserWindow.ExecuteScriptAsync($"document.querySelectorAll(\"#{id}\")[0].shadowRoot.querySelectorAll(\"button\")[0].ariaPressed;")).Result.ToString(), out isAlreadyUpvoted);
                        Skip:
                            upVotePageSource = await BrowserWindow.GetPageSourceAsync();
                            isRunning = false;
                        }
                    });
                    while (isRunning) Sleep(2);
                    return new Tuple<bool, bool>(!UpvoteDownvoteComment ? isAlreadyUpvoted : CheckVoteStatus(upVotePageSource, ActivityType.Upvote, "div", "class", mainAttributeValue,id:id), Downvoted);

                case ActivityType.UpvoteComment:

                    var upVoteCommentPageSource = string.Empty;
                    Application.Current.Dispatcher.Invoke(async () =>
                    {
                        BrowserWindow.GoToUrl();
                        await Task.Delay(TimeSpan.FromSeconds(5));
                        await BrowserWindow.ExecuteScriptAsync($"document.querySelector(\"[ui='desktop']\").scrollIntoViewIfNeeded()");
                        await Task.Delay(TimeSpan.FromSeconds(5));
                        await BrowserWindow.ExecuteScriptAsync($"document.querySelector(\"shreddit-comment-action-row\").shadowRoot.querySelector(\"button[upvote]\").click()");
                        await Task.Delay(TimeSpan.FromSeconds(5));
                        bool.TryParse((await BrowserWindow.ExecuteScriptAsync($"document.querySelector(\"shreddit-comment-action-row\").shadowRoot.querySelector(\"button[upvote]\").ariaPressed")).Result.ToString(), out isAlreadyUpvoted);
                        Success = isAlreadyUpvoted;
                        isRunning = false;
                    });
                    while (isRunning) Sleep(2);
                    return new Tuple<bool, bool>(Success,isAlreadyUpvoted);
            }
            return new Tuple<bool, bool>(Success, Downvoted);
        }

        public bool DownVote(ActivityType activityType, string destinationUrl,string id="")
        {
            var isRunning = true;
            var mainAttributeValue = string.Empty;
            var attributeValue = string.Empty;
            var isAlreadyDownvoted = false;
            BrowserWindow.ClearResources();
            switch (activityType)
            {
                case ActivityType.Downvote:

                    var downVotePageSource = string.Empty;
                    Application.Current.Dispatcher.Invoke(async () =>
                    {
                        BrowserWindow.GoToUrl(destinationUrl);
                        await Task.Delay(TimeSpan.FromSeconds(8));
                        downVotePageSource = await BrowserWindow.GetPageSourceAsync();
                        bool.TryParse((await BrowserWindow.ExecuteScriptAsync($"document.querySelectorAll(\"#{id}\")[0].shadowRoot.querySelectorAll(\"button\")[1].ariaPressed;")).Result.ToString(), out isAlreadyDownvoted);
                        if (isAlreadyDownvoted)
                            goto Skip;
                        var response = await BrowserWindow.ExecuteScriptAsync($"document.querySelectorAll(\"#{id}\")[0].shadowRoot.querySelectorAll(\"button\")[1].click();", 3);
                        await Task.Delay(TimeSpan.FromSeconds(5));
                        bool.TryParse((await BrowserWindow.ExecuteScriptAsync($"document.querySelectorAll(\"#{id}\")[0].shadowRoot.querySelectorAll(\"button\")[1].ariaPressed;")).Result.ToString(), out isAlreadyDownvoted);
                    Skip:
                        downVotePageSource = await BrowserWindow.GetPageSourceAsync();
                        isRunning = false;
                    });
                    while (isRunning) Thread.Sleep(TimeSpan.FromSeconds(2));
                    return isAlreadyDownvoted;
                case ActivityType.DownvoteComment:

                    Application.Current.Dispatcher.Invoke(async () =>
                    {
                        await BrowserWindow.ExecuteScriptAsync($"document.querySelector(\"[ui='desktop']\").scrollIntoViewIfNeeded()");
                        await Task.Delay(TimeSpan.FromSeconds(5));
                        await BrowserWindow.ExecuteScriptAsync($"document.querySelector(\"shreddit-comment-action-row\").shadowRoot.querySelector(\"button[downvote]\").click()");
                        await Task.Delay(TimeSpan.FromSeconds(5));
                        bool.TryParse((await BrowserWindow.ExecuteScriptAsync($"document.querySelector(\"shreddit-comment-action-row\").shadowRoot.querySelector(\"button[downvote]\").ariaPressed")).Result.ToString(), out isAlreadyDownvoted);
                        isRunning = false;
                    });
                    while (isRunning) Thread.Sleep(TimeSpan.FromSeconds(2));
                    return isAlreadyDownvoted;
            }

            return false;
        }
        public bool RemoveVote(ActivityType activityType,RedditPost post)
        {
            var isRunning = true;
            var pageSource = string.Empty;
            bool removedVote = true;
            var mainAttributeValue = string.Empty;
            BrowserWindow.ClearResources();
            switch (activityType)
            {
                case ActivityType.RemoveVote:
                    if (post.Upvoted)
                    {
                        Application.Current.Dispatcher.Invoke(async () =>
                        {
                            await Task.Delay(TimeSpan.FromSeconds(5));
                            //pageSource = await BrowserWindow.GetPageSourceAsync();
                            var response = await BrowserWindow.ExecuteScriptAsync($"document.querySelectorAll(\"#{post.Id}\")[0].shadowRoot.querySelectorAll(\"button\")[0].click();", 3);
                            await Task.Delay(TimeSpan.FromSeconds(5));
                            bool.TryParse((await BrowserWindow.ExecuteScriptAsync($"document.querySelectorAll(\"#{post.Id}\")[0].shadowRoot.querySelectorAll(\"button\")[0].ariaPressed;")).Result.ToString(), out removedVote);

                            isRunning = false;
                        });
                        while (isRunning) Thread.Sleep(TimeSpan.FromSeconds(2));
                        return !removedVote;
                    }
                    else if (post.Downvoted)
                    {
                        Application.Current.Dispatcher.Invoke(async () =>
                        {
                            await Task.Delay(TimeSpan.FromSeconds(5));
                            //pageSource = await BrowserWindow.GetPageSourceAsync();
                            var response = await BrowserWindow.ExecuteScriptAsync($"document.querySelectorAll(\"#{post.Id}\")[0].shadowRoot.querySelectorAll(\"button\")[1].click();", 3);
                            await Task.Delay(TimeSpan.FromSeconds(5));
                            bool.TryParse((await BrowserWindow.ExecuteScriptAsync($"document.querySelectorAll(\"#{post.Id}\")[0].shadowRoot.querySelectorAll(\"button\")[1].ariaPressed;")).Result.ToString(), out removedVote);
                            isRunning = false;
                        });
                        while (isRunning) Thread.Sleep(TimeSpan.FromSeconds(2));
                        return !removedVote;
                    }
                    return false;
                case ActivityType.RemoveVoteComment:

                    Application.Current.Dispatcher.Invoke(async () =>
                    {
                        await BrowserWindow.ExecuteScriptAsync($"document.querySelector(\"[ui='desktop']\").scrollIntoViewIfNeeded()");
                        await Task.Delay(TimeSpan.FromSeconds(5));
                        var votedRes = await BrowserWindow.ExecuteScriptAsync($"document.querySelector(\"shreddit-comment-action-row\").shadowRoot.querySelector(\"button[upvote]\").ariaPressed");
                        if (votedRes.Result.ToString().Contains("true"))
                        {
                            await BrowserWindow.ExecuteScriptAsync($"document.querySelector(\"shreddit-comment-action-row\").shadowRoot.querySelector(\"button[upvote]\").click()");
                            await Task.Delay(TimeSpan.FromSeconds(5));
                            bool.TryParse(( await BrowserWindow.ExecuteScriptAsync($"document.querySelector(\"shreddit-comment-action-row\").shadowRoot.querySelector(\"button[upvote]\").ariaPressed")).Result.ToString(),out removedVote);
                        }
                        else
                        {
                            votedRes = await BrowserWindow.ExecuteScriptAsync($"document.querySelector(\"shreddit-comment-action-row\").shadowRoot.querySelector(\"button[downvote]\").ariaPressed");
                            if (votedRes.Result.ToString().Contains("true"))
                            {
                                await BrowserWindow.ExecuteScriptAsync($"document.querySelector(\"shreddit-comment-action-row\").shadowRoot.querySelector(\"button[downvote]\").click()");
                                await Task.Delay(TimeSpan.FromSeconds(5));
                                bool.TryParse((await BrowserWindow.ExecuteScriptAsync($"document.querySelector(\"shreddit-comment-action-row\").shadowRoot.querySelector(\"button[downvote]\").ariaPressed")).Result.ToString(), out removedVote);
                            }
                        }
                        isRunning = false;
                    });
                    while (isRunning) Thread.Sleep(TimeSpan.FromSeconds(2));
                    return !removedVote;
            }
            return false;
        }
        public IResponseParameter SearchByCustomUrlAndGetPageSource(DominatorAccountModel account, string url)
        {
            url = Utils.UpdateDomain(url);
            var isRunning = true;
            CheckBrowserLogin(account);
            BrowserWindow.ClearResources();
            var response = new ResponseParameter();
            Application.Current.Dispatcher.Invoke(async () =>
            {
                response.Response = await BrowserWindow.GoToCustomUrl(url, 8);
                var i = 0;
                while (response.Response.Contains("Fetching messages") && ++i <= 5)
                {
                    await BrowserWindow.GoToCustomUrl(url, 10);
                    await Task.Delay(TimeSpan.FromSeconds(10));
                    response.Response = await BrowserWindow.GetPageSourceAsync();
                }
                isRunning = false;
            });
            while (isRunning) Sleep(2);
            return response;
        }

        public bool Publisher(DominatorAccountModel AccountModel, PublisherPostlistModel updatedPostDetails, string chooseCommunity,
            OtherConfigurationModel otherConfiguration, out string FailedMessage)
        {
            var isRunning = true;
            var attributeValueForCreatePost = string.Empty;
            var ChooseCommunityClass = "_2_6Q3rlmltjQM8nEBoNJr-";
            var publisherPageSource = string.Empty;
            try
            {
                BrowserWindow.ClearResources();
                BrowserWindow.GoToUrl();
                ClosePopUpWindow();
                Application.Current.Dispatcher.Invoke(async () =>
                {
                    await Task.Delay(TimeSpan.FromSeconds(5));
                    publisherPageSource = await BrowserWindow.GetPageSourceAsync(2);
                    if (!publisherPageSource.Contains("placeholder=\"Create Post"))
                    {
                        publisherPageSource = await BrowserWindow.GoToCustomUrl($"{RdConstants.NewRedditHomePageAPI}/explore/");
                        await Task.Delay(TimeSpan.FromSeconds(3));
                        await BrowserWindow.ExecuteScriptAsync("document.getElementsByClassName(\"_3zbhtNO0bdck0oYbYRhjMC HNozj_dKjQZ59ZsfEegz8\")[2].click();");
                    }
                    await Task.Delay(TimeSpan.FromSeconds(8));
                    publisherPageSource = await BrowserWindow.GetPageSourceAsync();
                    attributeValueForCreatePost = BrowserUtilities.GetPath(publisherPageSource, "button", "Create Post");
                    isRunning = false;
                });
                while (isRunning) Thread.Sleep(TimeSpan.FromSeconds(2));
                //For crosspost
                if (updatedPostDetails.PostSource.ToString() == "SharePost" && !string.IsNullOrEmpty(updatedPostDetails.ShareUrl))
                {
                    isRunning = true;
                    Application.Current.Dispatcher.Invoke(async () =>
                    {
                        await Task.Delay(TimeSpan.FromSeconds(10));
                        await BrowserWindow.GoToCustomUrl(updatedPostDetails.ShareUrl);
                        await Task.Delay(TimeSpan.FromSeconds(5));
                        var xyForChooseCommunity = BrowserWindow.GetXAndY(AttributeType.ClassName, "anPJr_ybRailY8NbAunl2");
                        await BrowserWindow.MouseClickAsync(xyForChooseCommunity.Key + 50, xyForChooseCommunity.Value + 10,
                            2, 2);
                        await BrowserWindow.EnterCharsAsync(chooseCommunity, 0, 2, 2);
                        //var xyForSearch = BrowserWindow.GetXAndY(AttributeType.ClassName, "_2MCEtCukiOUDUHF1PDgWwH");
                        var xyForSearch = BrowserWindow.GetXAndY(AttributeType.ClassName, "_3oyS3dPRsa51zDEONlIdts");
                        await BrowserWindow.MouseClickAsync(xyForSearch.Key + 5, xyForSearch.Value + 5,
                            2, 2);
                        await Task.Delay(TimeSpan.FromSeconds(15));
                        var ChooseCommunity = BrowserWindow.ExecuteScript("document.querySelector('input[placeholder=\"Choose a community\"]').value;").Result.ToString();
                        if (string.IsNullOrEmpty(ChooseCommunity))
                        {
                            await BrowserWindow.MouseClickAsync(xyForChooseCommunity.Key + 50, xyForChooseCommunity.Value + 10,
                            2, 2);
                            await BrowserWindow.EnterCharsAsync(chooseCommunity, 0, 2, 2);
                            var xyForSearch1 = BrowserWindow.GetXAndY(AttributeType.ClassName, "_2MCEtCukiOUDUHF1PDgWwH");
                            await BrowserWindow.MouseClickAsync(xyForSearch1.Key + 5, xyForSearch1.Value + 5,
                                2, 2);
                            await Task.Delay(TimeSpan.FromSeconds(5));
                        }
                        publisherPageSource = BrowserWindow.GetPageSource();

                        var attributeValueForPostButton =
                                BrowserUtilities.GetPath(publisherPageSource, "button", "\">Post</button>");
                        await BrowserWindow.BrowserActAsync(ActType.Click, AttributeType.ClassName,
                            attributeValueForPostButton,
                            delayBefore: 1, delayAfter: 11);

                        publisherPageSource = BrowserWindow.GetPageSource();
                        isRunning = false;
                    }); while (isRunning) Thread.Sleep(TimeSpan.FromSeconds(2));
                    if (string.IsNullOrEmpty(BrowserUtilities.GetPath(publisherPageSource, "button", "\">Post</button>")))
                    {
                        //For approving post
                        if (otherConfiguration.IsCheckedForApprovePost)
                            ApprovePost(publisherPageSource);

                        return true;
                    }
                    return false;
                }

                //To post with title or title with description
                else if (!string.IsNullOrEmpty(updatedPostDetails.PublisherInstagramTitle) &&
                    updatedPostDetails.MediaList.Count == 0 &&
                    string.IsNullOrEmpty(updatedPostDetails.PdSourceUrl))
                {
                    isRunning = true;
                    Application.Current.Dispatcher.Invoke(async () =>
                    {
                        await BrowserWindow.BrowserActAsync(ActType.Click, AttributeType.ClassName,
                            attributeValueForCreatePost,
                            index: 1, delayBefore: 2, delayAfter: 3);
                        publisherPageSource = await BrowserWindow.GetPageSourceAsync();

                        var i = 1;
                        while (!publisherPageSource.Contains("Create a post") && i++ <= 3)
                        {
                            if (!publisherPageSource.Contains("placeholder=\"Create Post"))
                            {
                                publisherPageSource = await BrowserWindow.GoToCustomUrl($"{RdConstants.NewRedditHomePageAPI}/explore/");
                                await Task.Delay(TimeSpan.FromSeconds(2));
                                await BrowserWindow.ExecuteScriptAsync("document.getElementsByClassName(\"_3zbhtNO0bdck0oYbYRhjMC HNozj_dKjQZ59ZsfEegz8\")[2].click();", 2);
                                await Task.Delay(TimeSpan.FromSeconds(2));
                                publisherPageSource = await BrowserWindow.GetPageSourceAsync();
                            }
                            await Task.Delay(TimeSpan.FromSeconds(3));
                            if(string.IsNullOrEmpty(attributeValueForCreatePost))
                                    attributeValueForCreatePost = BrowserUtilities.GetPath(publisherPageSource, "button", "Create Post");
                            await BrowserWindow.BrowserActAsync(ActType.Click, AttributeType.ClassName,
                                attributeValueForCreatePost,
                                index: 1, delayBefore: 3, delayAfter: 2);
                            publisherPageSource = await BrowserWindow.GetPageSourceAsync();
                        }

                        await Task.Delay(TimeSpan.FromSeconds(5));
                        var xyForChooseCommunity = BrowserWindow.GetXAndY(AttributeType.ClassName, "anPJr_ybRailY8NbAunl2");
                        await BrowserWindow.MouseClickAsync(xyForChooseCommunity.Key + 50, xyForChooseCommunity.Value + 10,
                            2, 2);
                        await BrowserWindow.EnterCharsAsync(chooseCommunity, 0, 2, 2);
                        var xyForSearch = BrowserWindow.GetXAndY(AttributeType.ClassName, "_2MCEtCukiOUDUHF1PDgWwH");
                        //var xyForSearch = BrowserWindow.GetXAndY(AttributeType.ClassName, "_3oyS3dPRsa51zDEONlIdts");
                        await BrowserWindow.MouseClickAsync(xyForSearch.Key + 5, xyForSearch.Value + 5,
                            2, 2);
                        await Task.Delay(TimeSpan.FromSeconds(5));
                        var ChooseCommunity = BrowserWindow.ExecuteScript("document.querySelector('input[placeholder=\"Choose a community\"]').value;").Result.ToString();
                        if (string.IsNullOrEmpty(ChooseCommunity))
                        {
                            await BrowserWindow.MouseClickAsync(xyForChooseCommunity.Key + 50, xyForChooseCommunity.Value + 10,
                            2, 2);
                            await BrowserWindow.EnterCharsAsync(chooseCommunity, 0, 2, 2);
                            var xyForSearch1 = BrowserWindow.GetXAndY(AttributeType.ClassName, "_3oyS3dPRsa51zDEONlIdts");
                            await BrowserWindow.MouseClickAsync(xyForSearch1.Key + 5, xyForSearch1.Value + 5,
                                2, 2);
                            await Task.Delay(TimeSpan.FromSeconds(5));
                        }
                        publisherPageSource = await BrowserWindow.GetPageSourceAsync();
                        var isPostPublishable = BrowserUtilities.IsPostPublishable(publisherPageSource, "button", "Post");
                        if (isPostPublishable)
                        {
                            var lstOfTextAreaBox = Regex.Split(publisherPageSource, "<textarea ");
                            var attributeValueForTitle = Utilities.GetBetween(lstOfTextAreaBox[1], "class=\"", "\"");
                            var xyForTitle = BrowserWindow.GetXAndY(AttributeType.ClassName, attributeValueForTitle);
                            await BrowserWindow.MouseClickAsync(xyForTitle.Key + 5, xyForTitle.Value + 5, 2, 2);
                            await BrowserWindow.EnterCharsAsync(updatedPostDetails.PublisherInstagramTitle, 0, delayAtLast: 2);
                            var xyForDescription = BrowserWindow.GetXAndY(AttributeType.ClassName,
                                "notranslate public-DraftEditor-content");
                            await BrowserWindow.MouseClickAsync(xyForDescription.Key + 5, xyForDescription.Value + 5, 2, 2);
                            await BrowserWindow.EnterCharsAsync(updatedPostDetails.PostDescription, 0, delayAtLast: 2);

                            if (updatedPostDetails.RedditPostSetting.IsOriginalContent)
                                await BrowserWindow.BrowserActAsync(ActType.Click, AttributeType.ClassName,
                                "Nb7NCPTlQuxN_WDPUg5Q2 zprH8YpG-gVpFuEr-eQJw", index: 0,
                                delayBefore: 1, delayAfter: 3);

                            if (updatedPostDetails.RedditPostSetting.IsSpoiler)
                                await BrowserWindow.BrowserActAsync(ActType.Click, AttributeType.ClassName,
                                "Nb7NCPTlQuxN_WDPUg5Q2 zprH8YpG-gVpFuEr-eQJw", index: 1,
                                delayBefore: 1, delayAfter: 3);
                            if (updatedPostDetails.RedditPostSetting.IsNsfw)
                                await BrowserWindow.BrowserActAsync(ActType.Click, AttributeType.ClassName,
                                "Nb7NCPTlQuxN_WDPUg5Q2 zprH8YpG-gVpFuEr-eQJw", index: 2,
                                delayBefore: 1, delayAfter: 3);

                            if (updatedPostDetails.RedditPostSetting.IsDisableSendingReplies)
                                await BrowserWindow.BrowserActAsync(ActType.Click, AttributeType.ClassName,
                                "_1sBmqB8geWKIW5Nt8svFgc", index: 0,
                                delayBefore: 1, delayAfter: 3);

                            var attributeValueForPostButton =
                                BrowserUtilities.GetPath(publisherPageSource, "button", "\">Post</button>");
                            await BrowserWindow.BrowserActAsync(ActType.Click, AttributeType.ClassName,
                                attributeValueForPostButton,
                                delayBefore: 1, delayAfter: 11);

                            publisherPageSource = await BrowserWindow.GetPageSourceAsync();
                            isRunning = false;
                        }
                        else
                        {

                            GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName, "Publish", "Post with Title and Description not allowed on " + chooseCommunity);
                            isRunning = false;
                        }

                    });
                    while (isRunning) Thread.Sleep(TimeSpan.FromSeconds(2));
                    if (string.IsNullOrEmpty(BrowserUtilities.GetPath(publisherPageSource, "button", "\">Post</button>")))
                    {
                        //For approving post
                        if (otherConfiguration.IsCheckedForApprovePost)
                            ApprovePost(publisherPageSource);

                        return true;
                    }

                    return false;
                }
                //For media
                if (!string.IsNullOrEmpty(updatedPostDetails.PublisherInstagramTitle) &&
                    updatedPostDetails.MediaList.Count > 0)
                {
                    isRunning = true;
                    Application.Current.Dispatcher.Invoke(async () =>
                    {
                        await BrowserWindow.BrowserActAsync(ActType.Click, AttributeType.ClassName,
                            "zgT5MfUrDMC54cpiCpZFu",
                            index: 0, delayBefore: 3, delayAfter: 2);
                        publisherPageSource = await BrowserWindow.GetPageSourceAsync();
                        var i = 1;
                        var j = 0;
                        List<string> lst = new List<string>();
                        while (!publisherPageSource.Contains("Create a post") && i++ <= 3)
                        {
                            if (!publisherPageSource.Contains("placeholder=\"Create Post"))
                            {
                                publisherPageSource = await BrowserWindow.GoToCustomUrl($"{RdConstants.NewRedditHomePageAPI}/explore/");
                                await Task.Delay(TimeSpan.FromSeconds(2));
                                await BrowserWindow.ExecuteScriptAsync("document.getElementsByClassName(\"_3zbhtNO0bdck0oYbYRhjMC HNozj_dKjQZ59ZsfEegz8\")[2].click();", 2);
                                await Task.Delay(TimeSpan.FromSeconds(2));
                                publisherPageSource = await BrowserWindow.GetPageSourceAsync();
                            }
                            await Task.Delay(TimeSpan.FromSeconds(3));
                            if (string.IsNullOrEmpty(attributeValueForCreatePost))
                                attributeValueForCreatePost = BrowserUtilities.GetPath(publisherPageSource, "button", "Create Post");
                            await BrowserWindow.BrowserActAsync(ActType.Click, AttributeType.ClassName,
                                attributeValueForCreatePost,
                                index: 1, delayBefore: 3, delayAfter: 2);
                            //await Task.Delay(TimeSpan.FromSeconds(3));
                            publisherPageSource = await BrowserWindow.GetPageSourceAsync();
                        }
                        var xyForChooseCommunity = BrowserWindow.GetXAndY(AttributeType.ClassName, "anPJr_ybRailY8NbAunl2");
                        await BrowserWindow.MouseClickAsync(xyForChooseCommunity.Key + 50, xyForChooseCommunity.Value + 10,
                            2, 2);
                        await BrowserWindow.EnterCharsAsync(chooseCommunity.Replace("@outlook.com",""), 0, 2, 2);
                        xyForChooseCommunity = BrowserWindow.GetXAndY(AttributeType.ClassName, ChooseCommunityClass);
                        await BrowserWindow.MouseClickAsync(xyForChooseCommunity.Key + 5, xyForChooseCommunity.Value + 5,
                            2, 2);
                        await Task.Delay(TimeSpan.FromSeconds(5));
                        publisherPageSource = await BrowserWindow.GetPageSourceAsync();
                        var isPostPublishable = BrowserUtilities.IsPostPublishable(publisherPageSource, "button", "Images &amp; Video");
                        if (isPostPublishable)
                        {
                            var attributeValueForTitle = Utilities.GetBetween(Regex.Split(publisherPageSource, "<textarea ")[1],
                                                    "class=\"", "\"");
                            var xyForTitle = BrowserWindow.GetXAndY(AttributeType.ClassName, attributeValueForTitle);
                            await BrowserWindow.MouseClickAsync(xyForTitle.Key + 5, xyForTitle.Value + 5, 2, 2);
                            await BrowserWindow.EnterCharsAsync(updatedPostDetails.PublisherInstagramTitle, 0, delayAtLast: 2);
                            var xyForDescription = BrowserWindow.GetXAndY(AttributeType.ClassName,
                                "notranslate public-DraftEditor-content");
                            await BrowserWindow.MouseClickAsync(xyForDescription.Key + 5, xyForDescription.Value + 5, 2, 2);
                            await BrowserWindow.EnterCharsAsync(updatedPostDetails.PostDescription, 0, delayAtLast: 2);
                            if (updatedPostDetails.MediaList.Count > 1)
                            {
                                for (i = 0; i < updatedPostDetails.MediaList.Count; i++)
                                {
                                    var mediaFilePath = updatedPostDetails.MediaList[i];
                                    if (updatedPostDetails.IsChangeHashOfMedia)
                                        mediaFilePath = MediaUtilites.CalculateMD5Hash(mediaFilePath);
                                }
                                //To generate new Hash code for media
                                for (j = 0; j < updatedPostDetails.MediaList.Count; j++)
                                {

                                    var mediaFilePath = updatedPostDetails.MediaList[j];
                                    if (updatedPostDetails.IsChangeHashOfMedia)
                                        mediaFilePath = MediaUtilites.CalculateMD5Hash(mediaFilePath);
                                    lst.Add(mediaFilePath);

                                }
                                BrowserWindow.ChooseFileFromDialog(pathList: lst);
                            }
                            else
                                BrowserWindow.ChooseFileFromDialog(pathList: updatedPostDetails.MediaList.ToList());
                            var xyForMedia = BrowserWindow.GetXAndY(AttributeType.ClassName, "Nb7NCPTlQuxN_WDPUg5Q2", 13);
                            if (xyForMedia.Key == 0 && xyForMedia.Value == 0)
                                xyForMedia = BrowserWindow.GetXAndY(AttributeType.ClassName, "_3O09Fh0CTb1KXH9g--pyTm _2iuoyPiKHN3kfOoeIQalDT _2tU8R9NTqhvBrhoNAXWWcP HNozj_dKjQZ59ZsfEegz8 ", 0);
                            await BrowserWindow.MouseClickAsync(xyForMedia.Key + 5, xyForMedia.Value + 5, 2, 2);
                            publisherPageSource = await BrowserWindow.GetPageSourceAsync();
                            await Task.Delay(TimeSpan.FromSeconds(10));
                            await BrowserWindow.ExecuteScriptAsync("document.getElementsByClassName('_18Bo5Wuo3tMV - RDB8 - kh8Z _2iuoyPiKHN3kfOoeIQalDT _10BQ7pjWbeYP63SAPNS8Ts HNozj_dKjQZ59ZsfEegz8')[0].click()");
                            publisherPageSource = await BrowserWindow.GetPageSourceAsync();
                            var attributeValueForPostButton = BrowserUtilities.GetPath(publisherPageSource, "button", "\">Post</button>");
                            var postBoxXAndY = await BrowserWindow.GetXAndYAsync(AttributeType.ClassName, attributeValueForPostButton, 0);
                            await BrowserWindow.BrowserActAsync(ActType.Click, AttributeType.ClassName, attributeValueForPostButton, delayBefore: 1, delayAfter: 11);
                            await BrowserWindow.MouseClickAsync(postBoxXAndY.Key, postBoxXAndY.Value, delayAfter: 3);
                            await BrowserWindow.BrowserActAsync(ActType.Click, AttributeType.ClassName, attributeValueForPostButton, delayBefore: 1, delayAfter: 5);
                            await Task.Delay(TimeSpan.FromSeconds(10));
                            if (updatedPostDetails.RedditPostSetting.IsOriginalContent)
                                await ApplySetting(0, BrowserWindow);
                            if (updatedPostDetails.RedditPostSetting.IsSpoiler)
                                await ApplySetting(1, BrowserWindow);
                            if (updatedPostDetails.RedditPostSetting.IsNsfw)
                                await ApplySetting(2, BrowserWindow);
                            if (updatedPostDetails.RedditPostSetting.IsDisableSendingReplies)
                                await ApplySetting(3, BrowserWindow);
                            await Task.Delay(TimeSpan.FromSeconds(10));
                            publisherPageSource = await BrowserWindow.GetPageSourceAsync();
                            isRunning = false;
                        }
                        else
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName, "Publish", "Images and Videos are not allowed on https://www.reddit.com/r/" + chooseCommunity);
                            isRunning = false;
                        }
                    });
                    while (isRunning) Thread.Sleep(TimeSpan.FromSeconds(2));
                    if (string.IsNullOrEmpty(BrowserUtilities.GetPath(publisherPageSource, "button", "\">Post</button>")))
                    {
                        //For approving post
                        if (!otherConfiguration.IsCheckedForApprovePost)
                            ApprovePost(publisherPageSource);

                        return true;
                    }

                    return false;
                }

                //For link url

                if (!string.IsNullOrEmpty(updatedPostDetails.PublisherInstagramTitle))
                {
                    isRunning = true;
                    Application.Current.Dispatcher.Invoke(async () =>
                    {
                        await BrowserWindow.BrowserActAsync(ActType.Click, AttributeType.ClassName,
                            attributeValueForCreatePost,
                            index: 1, delayBefore: 3, delayAfter: 2);
                        await Task.Delay(TimeSpan.FromSeconds(3));
                        publisherPageSource = await BrowserWindow.GetPageSourceAsync();
                        var i = 1;
                        while (!publisherPageSource.Contains("Create a post") && i++ <= 3)
                        {
                            if (!publisherPageSource.Contains("placeholder=\"Create Post"))
                            {
                                publisherPageSource = await BrowserWindow.GoToCustomUrl($"{RdConstants.NewRedditHomePageAPI}/explore/");
                                await Task.Delay(TimeSpan.FromSeconds(2));
                                await BrowserWindow .ExecuteScriptAsync("document.getElementsByClassName(\"_3zbhtNO0bdck0oYbYRhjMC HNozj_dKjQZ59ZsfEegz8\")[2].click();", 2);
                                await Task.Delay(TimeSpan.FromSeconds(2));
                                publisherPageSource = await BrowserWindow.GetPageSourceAsync();
                            }
                            await Task.Delay(TimeSpan.FromSeconds(3));
                            if (string.IsNullOrEmpty(attributeValueForCreatePost))
                                attributeValueForCreatePost = BrowserUtilities.GetPath(publisherPageSource, "button", "Create Post");
                            await BrowserWindow.BrowserActAsync(ActType.Click, AttributeType.ClassName,
                                attributeValueForCreatePost,
                                index: 1, delayBefore: 5, delayAfter: 2);
                            await Task.Delay(TimeSpan.FromSeconds(3));
                            publisherPageSource = await BrowserWindow.GetPageSourceAsync();
                        }

                        var linkPageAttributeValue = BrowserUtilities.GetAttributeValueForActionForMediaAndLink(
                            publisherPageSource, "div",
                            "class", "_3vyt9N_0jfZuOwByiKDi9x", 2);
                        await BrowserWindow.BrowserActAsync(ActType.Click, AttributeType.ClassName, linkPageAttributeValue,
                            delayBefore: 4, delayAfter: 2, index: 2);
                        var xyForChooseCommunity = BrowserWindow.GetXAndY(AttributeType.ClassName, "anPJr_ybRailY8NbAunl2");
                        await BrowserWindow.MouseClickAsync(xyForChooseCommunity.Key + 50, xyForChooseCommunity.Value + 10,
                            2, 2);
                        await BrowserWindow.EnterCharsAsync(chooseCommunity, 0, 2, 2);
                        xyForChooseCommunity = BrowserWindow.GetXAndY(AttributeType.ClassName, ChooseCommunityClass);
                        await BrowserWindow.MouseClickAsync(xyForChooseCommunity.Key + 5, xyForChooseCommunity.Value + 5,
                            2, 2);
                        await Task.Delay(TimeSpan.FromSeconds(5));
                        publisherPageSource = await BrowserWindow.GetPageSourceAsync();
                        var isPostPublishable = BrowserUtilities.IsPostPublishable(publisherPageSource, "button", "Link");
                        if (isPostPublishable)
                        {
                            var lstOfTextAreaBox = Regex.Split(publisherPageSource, "<textarea ");
                            var attributeValueForTitle = Utilities.GetBetween(lstOfTextAreaBox[1], "class=\"", "\"");
                            var xyForTitle = BrowserWindow.GetXAndY(AttributeType.ClassName, attributeValueForTitle);
                            await BrowserWindow.MouseClickAsync(xyForTitle.Key + 5, xyForTitle.Value + 5, 2, 2);
                            await BrowserWindow.EnterCharsAsync(updatedPostDetails.PublisherInstagramTitle, 0, delayAtLast: 2);
                            var attributeValueForLink = Utilities.GetBetween(lstOfTextAreaBox[2], "class=\"", "\"");
                            var xyForLink = BrowserWindow.GetXAndY(AttributeType.ClassName, attributeValueForLink);
                            await BrowserWindow.MouseClickAsync(xyForLink.Key + 5, xyForLink.Value + 5, 2, 2);
                            await BrowserWindow.EnterCharsAsync(updatedPostDetails.PdSourceUrl, 0, delayAtLast: 2);

                            if (updatedPostDetails.RedditPostSetting.IsOriginalContent)
                                await BrowserWindow.BrowserActAsync(ActType.Click, AttributeType.ClassName,
                                "Nb7NCPTlQuxN_WDPUg5Q2 zprH8YpG-gVpFuEr-eQJw", index: 0,
                                delayBefore: 1, delayAfter: 3);

                            if (updatedPostDetails.RedditPostSetting.IsSpoiler)
                                await BrowserWindow.BrowserActAsync(ActType.Click, AttributeType.ClassName,
                                "Nb7NCPTlQuxN_WDPUg5Q2 zprH8YpG-gVpFuEr-eQJw", index: 1,
                                delayBefore: 1, delayAfter: 3);
                            if (updatedPostDetails.RedditPostSetting.IsNsfw)
                                await BrowserWindow.BrowserActAsync(ActType.Click, AttributeType.ClassName,
                                "Nb7NCPTlQuxN_WDPUg5Q2 zprH8YpG-gVpFuEr-eQJw", index: 2,
                                delayBefore: 1, delayAfter: 3);

                            if (updatedPostDetails.RedditPostSetting.IsDisableSendingReplies)
                                await BrowserWindow.BrowserActAsync(ActType.Click, AttributeType.ClassName,
                                "_1sBmqB8geWKIW5Nt8svFgc", index: 0,
                                delayBefore: 1, delayAfter: 3);

                            var attributeValueForPostButton =
                                BrowserUtilities.GetPath(publisherPageSource, "button", "\">Post</button>");
                            await BrowserWindow.BrowserActAsync(ActType.Click, AttributeType.ClassName,
                                attributeValueForPostButton,
                                delayBefore: 1, delayAfter: 11);

                            publisherPageSource = await BrowserWindow.GetPageSourceAsync();
                            isRunning = false;
                        }
                        else
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName, "Publish", "Links are not allowed on" + chooseCommunity);
                            isRunning = false;
                        }

                    });
                    while (isRunning) Thread.Sleep(TimeSpan.FromSeconds(2));
                    FailedMessage = Utils.GetBetween(publisherPageSource, "", "");
                    if (publisherPageSource.Contains(updatedPostDetails.PublisherInstagramTitle) ||
                        publisherPageSource.Contains("just now"))
                    {
                        //For approving post
                        if (otherConfiguration.IsCheckedForApprovePost)
                            ApprovePost(publisherPageSource);

                        return true;
                    }

                    return false;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            finally
            {
                FailedMessage = Utils.GetBetween(publisherPageSource, "<span class=\"_3h_9YwxjuOr77VhScPrjCI\">", "</span>");
            }
            return false;
        }

        private async Task ApplySetting(int Index, BrowserWindow browserWindow)
        {
            var SettingClass = "_2eawLPCtwzvTZhWKtaUgZQ _3LyKu57c-QkPvlFvAgWop5";
            await browserWindow.ExecuteScriptAsync("document.querySelector('button[aria-label=\"more options\"]').scrollIntoViewIfNeeded();", 3);
            await browserWindow.ExecuteScriptAsync("document.querySelector('button[aria-label=\"more options\"]').click();", 3);
            await browserWindow.BrowserActAsync(ActType.Click, AttributeType.ClassName, SettingClass, index: Index, delayBefore: 1, delayAfter: 3);
        }
        private async Task ApplySetting(int Index, PuppeteerBrowserActivity browserWindow)
        {
            var SettingClass = "_2eawLPCtwzvTZhWKtaUgZQ _3LyKu57c-QkPvlFvAgWop5";
            await browserWindow.ExecuteScriptAsync("document.querySelector('button[aria-label=\"more options\"]').scrollIntoViewIfNeeded();", 3);
            await browserWindow.ExecuteScriptAsync("document.querySelector('button[aria-label=\"more options\"]').click();", 3);
            await browserWindow.BrowserActAsync(ActType.Click, AttributeType.ClassName, SettingClass, index: Index, delayBefore: 1, delayAfter: 3);
        }
        private void Sleep(double seconds = 1)
        {
            Task.Delay(TimeSpan.FromSeconds(seconds)).Wait( /*_cancellationToken*/);
        }
        private string GetUrlForSubscribeAndUnSubscribe(string queryValue)
        {
            queryValue = Utils.GetLastWordFromUrl(queryValue);
            return RdConstants.NewRedditHomePageAPI + "/r/" + queryValue;
        }

        private bool CheckVoteStatus(string pageSource, ActivityType activityType, string tagName, string attributeName,
            string attributeValue,string id="")
        {
            //var checkVoteStatus = string.Empty;
            bool isVoted = true;
            bool isRunning = true;
            try
            {
                Application.Current.Dispatcher.Invoke(async () =>
                {
                    switch (activityType)
                    {
                        case ActivityType.Upvote:
                            bool.TryParse((await BrowserWindow.ExecuteScriptAsync($"document.querySelectorAll(\"#{id}\")[0].shadowRoot.querySelectorAll(\"button\")[0].ariaPressed;")).Result.ToString(), out isVoted);
                            break;
                         case ActivityType.Downvote:
                            bool.TryParse((await BrowserWindow.ExecuteScriptAsync($"document.querySelectorAll(\"#{id}\")[0].shadowRoot.querySelectorAll(\"button\")[1].ariaPressed;")).Result.ToString(), out isVoted);
                            break;
                    }
                    isRunning = false;
                });
                while (isRunning) Thread.Sleep(TimeSpan.FromSeconds(2));
                //var InnerHtml =
                //    HtmlParseUtility.GetOuterHtmlFromTagName(pageSource, tagName, attributeName, attributeValue);
                //switch (activityType)
                //{
                //    case ActivityType.Upvote:
                //        checkVoteStatus = Utilities.GetBetween(InnerHtml, "aria-pressed=\"", "\"");
                //        return checkVoteStatus.Equals("true") ? true : false;

                //    case ActivityType.Downvote:
                //        var lstInnerHtml = Regex.Split(InnerHtml, "aria-label=\"downvote\"");
                //        checkVoteStatus = Utilities.GetBetween(lstInnerHtml[1], "aria-pressed=\"", "\"");
                //        if (string.IsNullOrEmpty(checkVoteStatus))
                //        {
                //            var downVoteString = Utilities.GetBetween(pageSource, "<button aria-label=\"downvote\"", "data-click-id=\"downvote\"");
                //            checkVoteStatus = Utilities.GetBetween(downVoteString, "aria-pressed=\"", "\"");
                //        }
                //        return checkVoteStatus.Equals("true") ? true : false;
                //}
            }
            catch (Exception)
            {
                return false;
            }
            return isVoted;
        }

        private void ApprovePost(string publisherPageSource)
        {
            var isRunning = true;
            var approveAttributeValue = BrowserUtilities.GetPath(publisherPageSource, "span", "approve");
            Application.Current.Dispatcher.Invoke(async () =>
            {
                await BrowserWindow.BrowserActAsync(ActType.Click, AttributeType.ClassName, approveAttributeValue,
                    delayBefore: 3, delayAfter: 2);
                isRunning = false;
            });
            while (isRunning) Thread.Sleep(TimeSpan.FromSeconds(2));
        }
        public void ClosePopUpWindow() => BrowserWindow.ExecuteScript("document.querySelector('button[aria-label=\"Close\"]').click();", 2);

        public IResponseParameter TryAndGetResponse(DominatorAccountModel dominatorAccount, string Url, int TryCount = 2, string ContainString = "", bool NewBrowser = false, bool InverseCheck = false, bool IsSafeSearchEnabled = false)
        {
            IResponseParameter Response = new ResponseParameter();
            try
            {
                Response = SearchByCustomUrl(dominatorAccount, Url, NewBrowser, IsSafeSearchEnabled);
                while (TryCount-- >= 0 && (string.IsNullOrEmpty(ContainString) ? string.IsNullOrEmpty(Response.Response) : InverseCheck ? !Response.Response.Contains(ContainString) : Response.Response.Contains(ContainString)))
                {
                    Thread.Sleep(3000);
                    Response = SearchByCustomUrl(dominatorAccount, Url, false, IsSafeSearchEnabled);
                }
            }
            catch { }
            return Response;
        }

        public async Task<AutoActivityResponseHandler> ScrollFeedAndGetResponse(DominatorAccountModel dominatorAccount, string Url, bool OnlyScroll = false)
        {
            var pageSource = string.Empty;
            var LastScrolledPage = string.Empty;
            var IsRunning = true;
            CheckBrowserLogin(dominatorAccount);
            await Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    await Task.Delay(7000);
                    pageSource = await CheckWindowLoaded(pageSource, Url);
                    dominatorAccount.Token.ThrowIfCancellationRequested();
                    var ScrollCount = RandomUtilties.GetRandomNumber(60, 20);
                    var CurrentCount = 0;
                    while (OnlyScroll ? CurrentCount++ < ScrollCount : CurrentCount++ <= 5)
                    {
                        dominatorAccount.Token.ThrowIfCancellationRequested();
                        var max = RandomUtilties.GetRandomNumber(10, 8);
                        var min = RandomUtilties.GetRandomNumber(5, 3);
                        var scroll = RandomUtilties.GetRandomNumber(2500, 2000);
                        await BrowserWindow.ExecuteScriptAsync($"window.scrollBy(0,{scroll});", delayInSec: RandomUtilties.GetRandomNumber(max, min));
                        dominatorAccount.Token.ThrowIfCancellationRequested();
                        LastScrolledPage = await BrowserWindow.GetPageSourceAsync();
                    }
                    IsRunning = false;
                }
                catch
                {
                    IsRunning = false;
                }
            });
            while (IsRunning) await Task.Delay(2000);
            return new AutoActivityResponseHandler(new ResponseParameter { Response = pageSource }, false, null, OnlyScroll, true, LastScrolledPage);
        }

        public void CheckBrowserLogin(DominatorAccountModel dominatorAccount)
        {
            if (BrowserWindow == null)
                BrowserLogin(dominatorAccount, dominatorAccount.Token);
        }

        public async Task<bool> AcceptPendingMessageRequest(DominatorAccountModel dominatorAccount, RedditUser redditUser)
        {
            try
            {
                var IsRequestAccepted = false;
                var IsRunning = true;
                CheckBrowserLogin(dominatorAccount);
                await Application.Current.Dispatcher.InvokeAsync(async () =>
                {
                    try
                    {
                        await BrowserWindow.GoToCustomUrl($"https://chat.reddit.com/room/{redditUser.ThreadID?.Replace(":reddit.com", "%3Areddit.com")}", delayAfter: 7);
                        var IsAccepted = await BrowserWindow.ExecuteScriptAsync($"[...document.querySelector('rs-app[locale=\"en-US\"]').shadowRoot.querySelector('rs-room-overlay-manager[room=\"{redditUser.ThreadID}\"]>rs-room').shadowRoot.querySelector('main>rs-room-invite').shadowRoot.querySelectorAll('div>button')].filter(x=>x.textContent.trim().includes(\"Accept\"))[0].click();", delayInSec: 4);
                        IsRequestAccepted = IsAccepted != null && IsAccepted.Success;
                        IsRunning = false;
                    }
                    catch
                    {
                        IsRunning = false;
                    }
                });
                while (IsRunning)
                    Task.Delay(2000).Wait(dominatorAccount.Token);
                return IsRequestAccepted;
            }
            finally
            {
                if (BrowserWindow != null && !BrowserWindow.IsDisposed)
                    Application.Current.Dispatcher.Invoke(delegate
                    { BrowserWindow.Close(); });
            }
        }

        public ActivityResposneHandler BroadCastMessage(ActivityType activityType, DominatorAccountModel dominatorAccount, RedditUser redditUser)
        {
            var isRunning = true;
            var messageResponse = string.Empty;
            var ResponseHandler = new ActivityResposneHandler();
            BrowserWindow.ClearResources();
            _cancellationToken.ThrowIfCancellationRequested();
            Application.Current.Dispatcher.Invoke(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(5));
                messageResponse = await BrowserWindow.GetPageSourceAsync();
                try
                {
                    // get x and y path of start chat button
                    var XandYPath = await BrowserWindow.GetXAndYAsync(customScriptX:$"[...document.querySelectorAll('a')].filter(x=>x.innerText.includes('Start Chat'))[0].getBoundingClientRect().x;",
                        customScriptY:$"[...document.querySelectorAll('a')].filter(x=>x.innerText.includes('Start Chat'))[0].getBoundingClientRect().y");
                    await BrowserWindow.MouseClickAsync(XandYPath.Key + 10, XandYPath.Value + 5, delayAfter: 5);
                    await Task.Delay(10000);
                    int.TryParse((await BrowserWindow.ExecuteScriptAsync($"document.querySelectorAll(\"shreddit-app>div\").length")).Result.ToString(), out int count);
                    if (count > 1)
                    {
                        _cancellationToken.ThrowIfCancellationRequested();
                        // Get XandY path of Message box
                        XandYPath = await BrowserWindow.GetXAndYAsync(customScriptX: $"document.querySelectorAll(\"shreddit-app>div\")[1].shadowRoot.querySelector(\"rs-app\").shadowRoot.querySelector(\"rs-direct-chat\").shadowRoot.querySelector(\"rs-message-composer\").shadowRoot.querySelector(\"textarea\").getBoundingClientRect().x",
                            customScriptY: $"document.querySelectorAll(\"shreddit-app>div\")[1].shadowRoot.querySelector(\"rs-app\").shadowRoot.querySelector(\"rs-direct-chat\").shadowRoot.querySelector(\"rs-message-composer\").shadowRoot.querySelector(\"textarea\").getBoundingClientRect().y");
                        await BrowserWindow.MouseClickAsync(XandYPath.Key + 15, XandYPath.Value + 5, delayAfter: 5);
                        await BrowserWindow.EnterCharsAsync($" {redditUser.Text.Trim()}", .05, delayAtLast: 5, delayBefore: 3);
                        messageResponse = await BrowserWindow.GetPageSourceAsync();
                        _cancellationToken.ThrowIfCancellationRequested();
                        //Get XandY path of submit button
                        XandYPath = await BrowserWindow.GetXAndYAsync(customScriptX: $"document.querySelectorAll(\"shreddit-app>div\")[1].shadowRoot.querySelector(\"rs-app\").shadowRoot.querySelector(\"rs-direct-chat\").shadowRoot.querySelector(\"rs-message-composer\").shadowRoot.querySelector(\"button[aria-label='Send message']\").getBoundingClientRect().x",
                            customScriptY: $"document.querySelectorAll(\"shreddit-app>div\")[1].shadowRoot.querySelector(\"rs-app\").shadowRoot.querySelector(\"rs-direct-chat\").shadowRoot.querySelector(\"rs-message-composer\").shadowRoot.querySelector(\"button[aria-label='Send message']\").getBoundingClientRect().y");

                        await BrowserWindow.MouseClickAsync(XandYPath.Key + 5, XandYPath.Value + 5, delayAfter: 5);
                    }
                    messageResponse = await BrowserWindow.GetPageSourceAsync();
                    var listofData = await BrowserWindow.GetPaginationDataList("User is flagged for spam", true);
                    if (listofData != null && listofData.Count > 0)
                    {
                        ResponseHandler.Status = false;
                    }
                    else
                    {
                        _cancellationToken.ThrowIfCancellationRequested();
                        //for checking button is disabled after sendmessage
                        var result = await BrowserWindow.ExecuteScriptAsync($"document.querySelectorAll(\"shreddit-app>div\")[1].shadowRoot.querySelector(\"rs-app\").shadowRoot.querySelector(\"rs-direct-chat\").shadowRoot.querySelector(\"rs-message-composer\").shadowRoot.querySelector(\"button[aria-label='Send message']\").disabled");
                        bool.TryParse(result.Result.ToString(), out bool status);
                        ResponseHandler.Status = status;
                    }
                }
                catch (Exception)
                {
                }
                
                isRunning = false;
            });
            while (isRunning) Sleep(2);
            return ResponseHandler;
        }
    }
}