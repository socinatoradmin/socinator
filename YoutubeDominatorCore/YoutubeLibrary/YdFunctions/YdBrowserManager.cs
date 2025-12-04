using CefSharp;
using DominatorHouseCore;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.Config;
using DominatorHouseCore.Models.Publisher;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.PuppeteerBrowser;
using DominatorHouseCore.Request;
using DominatorHouseCore.Utility;
using EmbeddedBrowser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using YoutubeDominatorCore.Response;
using YoutubeDominatorCore.YDEnums;
using YoutubeDominatorCore.YDUtility;
using YoutubeDominatorCore.YoutubeModels;

namespace YoutubeDominatorCore.YoutubeLibrary.YdFunctions
{
    public interface IYdBrowserManager : IBrowserManager
    {
        BrowserWindow BrowserWindow { get; set; }
        BrowserWindow CustomBrowserWindow { get; set; }
        string CurrentData { get; set; }
        string _html { get; set; }
        JavascriptResponse LastScriptResp { get; set; }
        bool SetVideoQuality { get; set; }
        void SetAccount(DominatorAccountModel account);
        void SetCancellationToken(CancellationToken token);
        void BrowserDispatcher(BrowserFuncts function, double delayBefore = 0, params object[] paramS);

        void SwitchChannel(string pageId, double delayBeforeHit = 0, bool SwitchChannelForPublisher = false);

        PostInfoYdResponseHandler PostInfo(ActivityType activity, string url, bool hasVideoDuration = false,
            double delayBeforeHit = 0, bool sortCommentsByNew = false, bool needChannelDetails = false, bool pauseVideo = true);
        void SearchByKeywordOrHashTag(DominatorAccountModel account, string keyWordorHashtag, bool isHashTag = false, string searchFilterUrlParam = "EgIQAQ%3D%3D");
        Task<bool> ViewIncreaserVideo(DominatorAccountModel accountModel, YoutubePost youtubePost, int delay,
            string channelPageIdSource, bool mozillaSelected, bool browserHidden, bool skipAd,
            double delayBeforeHit = 0);

        ChannelInfoResponseHandler GetChannelDetails(string channelUrl, double delayBeforeHit = 0);

        LikeDislikeResponseHandler LikeDislikeVideo(ActivityType activityType, YoutubePost youtubePost,
            double delayBeforeHit = 0);

        LikeDislikeResponseHandler LikeDislikeVideoAfterComment(ActivityType activityType, YoutubePost youtubePost,
            double delayBeforeHit = 0);

        SubscribeResponseHandler SubscribeChannel(DominatorAccountModel accountModel, YoutubeChannel youtubeChannel, double delayAfter = 0);
        UnsubscribeResponseHandler UnsubscribeChannel(DominatorAccountModel accountModel, YoutubeChannel youtubeChannel, double delayAfter = 0);

        CommentResponseHandler CommentOnVideo(YoutubePost youtubePost,
            string messageText, bool isReply, double delayBeforeHit = 0, int firstReplyClickIndex = 0);

        ReportVideoResponseHandler ReportToVideo(DominatorAccountModel dominatorAccount, YoutubePost youtubePost,
            int option, int subOption, string text, int mins = 0, int sec = 0, double delayBeforeHit = 0, string ReportText = "");
        List<YoutubePost> ScrapPostsFromChannel(string keyword, ActivityType actType, ref int lastIndex,
            ref string channelId, ref string channelUsername, bool firstPage, double delayBeforeHit = 0);

        List<YoutubeChannel> ScrapChannelsFromKeyword(string keyword, ActivityType actType, ref int lastIndex,
            bool firstPage, double delayBeforeHit = 0);

        List<YoutubeChannel> GetSubscribedChannels(ref int lastIndex, bool firstPage, double delayBeforeHit = 0);

        List<YoutubePostCommentModel> ScrapePostCommentsDetails(ActivityType actType, string lastCommentId,
            ref int lastIndex, double delayBeforeHit = 0, int noOfScraping = 5);

        bool LikeComments(YoutubePost postWithTheComment,
            string messageText, double delayBeforeHit = 0);

        string PublishVideoAndGetVideoId(PublisherPostlistModel postDetails, string channelPageId, OtherConfigurationModel otherConfigurationModel,
            double delayBeforeHit = 0, bool SwicthChannelForPublisher = false);
        void ScrollScreen(int toPixels);
        void CloseBrowserCustom();
        void CloseBrowser();
        string LoadPageSource(DominatorAccountModel account, string url, bool clearandNeedResource = false, bool isNewWindow = false, int timeSec = 15);
        SearchPostsResponseHandler ScrollWindowAndGetPosts(DominatorAccountModel account, ActivityType actType, int noOfPagetoScroll, int lastPageno = 0);


    }

    public class YdBrowserManager : IYdBrowserManager
    {
        private const string HomeUrl = "https://www.youtube.com/";
        private readonly YoutubeModel _youtubeModel;
        private DominatorAccountModel _account;
        private CancellationToken _cancellationToken;

        public YdBrowserManager()
        {
            _isDisposed = false;
            _isLoaded = false;
            _UserCount = 0;
            _langEngSet = false;
            try
            {
                InitializeGoogleLoginStatusActions();

                var genericFileManager = InstanceProvider.GetInstance<IGenericFileManager>();
                _youtubeModel =
                    genericFileManager.GetModel<YoutubeModel>(ConstantVariable.GetOtherYoutubeSettingsFile());
            }
            catch
            {
                _youtubeModel = new YoutubeModel();
            }
        }
        public JavascriptResponse LastScriptResp { get; set; }
        public KeyValuePair<int, int> ScreenResolution { get; set; } = YdStatic.GetScreenResolution();
        public string _html { get; set; }
        private string _pageText { get; set; }
        public bool VerifyingAccount { get; set; }
        public string TargetUrl { get; set; }
        public bool CustomUse { get; set; }
        public bool IsLoggedIn { get; set; }
        private bool SkipYoutubeAd { get; set; }
        public bool FoundAd { get; set; }
        public BrowserWindow BrowserWindow { get; set; }
        public BrowserWindow CustomBrowserWindow { get; set; }
        public string CurrentData { get; set; }
        public List<string> ResultList { get; set; }
        private KeyValuePair<int, int> lastCustomXandY;
        #region Browser Functions

        public void SetAccount(DominatorAccountModel account)
        {
            _account = account;
        }

        public void SetCancellationToken(CancellationToken token)
        {
            _cancellationToken = token;
        }




        private async Task LoadUrlUntillGetExact(string url, string conditionalString = "youtube")
        {
            await BrowserWindow.GoToCustomUrl(url);
            await Task.Delay(2000);
            await WaitUntillGetExact(conditionalString);
        }

        private async Task WaitUntillGetExact(string conditionalString = "youtube")
        {
            var last2Min = DateTime.Now.AddMinutes(2);
            CurrentData = "";
            var leng = 0;
            while (last2Min >= DateTime.Now)
            {
                _cancellationToken.ThrowIfCancellationRequested();
                await Task.Delay(1000);
                CurrentData = await BrowserWindow.GetPageSourceAsync();
                if (leng == CurrentData.Length && CurrentData.Contains(conditionalString))
                    break;
                leng = CurrentData.Length;
            }
        }

        public void BrowserDispatcher(BrowserFuncts function, double delayBefore = 0, params object[] paramS)
        {
            _cancellationToken.ThrowIfCancellationRequested();

            Sleep(delayBefore);

            _cancellationToken.ThrowIfCancellationRequested();

            var isRunning = true;
            lock (this)
            {
                Application.Current.Dispatcher.InvokeAsync(async () =>
                {
                    if (BrowserWindow == null || BrowserWindow != null && BrowserWindow.IsDisposed)
                    {
                        isRunning = false;
                        return;
                    }

                    try
                    {
                        switch (function)
                        {
                            case BrowserFuncts.GoToUrl:
                                var conditionalString = paramS.Length > 1 ? paramS[1].ToString() : "youtube";
                                await LoadUrlUntillGetExact(paramS[0].ToString(), conditionalString);
                                break;

                            case BrowserFuncts.HitUrl:
                                await BrowserWindow.GoToCustomUrl(paramS[0].ToString());
                                break;

                            case BrowserFuncts.EnterChars:
                                var chars = paramS[0].ToString();
                                var typingDelay = paramS.Length > 1 ? (double)paramS[1] : 0.09;
                                await BrowserWindow.EnterCharsAsync(" " + chars, typingDelay);
                                break;

                            case BrowserFuncts.MouseClick:
                                var developerScreenWidth = paramS.Length > 2 ? (int)paramS[2] : 1366;
                                var widthRatio = YdStatic.ScreenResolution().Width / (double)developerScreenWidth;
                                //var extarWidth = (int) (56 * widthRatio);
                                var extarWidth = 0;
                                var xLoc = (int)paramS[0] + extarWidth;
                                var yLoc = (int)paramS[1];
                                await BrowserWindow.MouseClickAsync(xLoc, yLoc);
                                break;

                            case BrowserFuncts.PressKey:
                                var winKeyCode = (int)paramS[0];
                                var nTimes = paramS.Length > 1 ? (int)paramS[1] : 1;
                                var delayBetween = paramS.Length > 2 ? (int)paramS[2] : 90;
                                var delayAtLast = paramS.Length > 3 ? (int)paramS[3] : 3;
                                await BrowserWindow
                                    .PressAnyKeyUpdated(winKeyCode, nTimes, delayBetween, delayAtLast);
                                break;
                            case BrowserFuncts.GetPageSource:
                                var conditionalString1 = paramS.Length > 0 ? paramS[0].ToString() : "youtube";
                                await WaitUntillGetExact(conditionalString1);
                                break;

                            case BrowserFuncts.GetPageSourceNoCondition:
                                _html = await BrowserWindow.GetPageSourceAsync();
                                break;

                            case BrowserFuncts.GetCurrentUrl:
                                CurrentData = BrowserWindow.CurrentUrl();
                                break;

                            case BrowserFuncts.GetPageText:
                                CurrentData = await BrowserWindow.PageText();
                                _pageText = CurrentData;
                                break;
                            case BrowserFuncts.GetPaginationData:
                                var startSearchText = paramS[0].ToString();
                                var isContains = paramS.Length > 1 ? (bool)paramS[1] : false;
                                CurrentData = await BrowserWindow
                                    .GetPaginationData(startSearchText, isContains);
                                break;


                            case BrowserFuncts.BrowserAct:
                                var actType = (ActType)paramS[0];
                                var attributeType = (AttributeType)paramS[1];
                                var attributeValue = paramS.Length > 2 ? paramS[2].ToString() : "";
                                var value = paramS.Length > 3 ? paramS[3].ToString() : "";
                                var clickIndex = paramS.Length > 4 ? (int)paramS[4] : 0;
                                var scrollByPixel = paramS.Length > 5 ? (int)paramS[5] : 100;
                                await BrowserWindow.BrowserActAsync(actType, attributeType, attributeValue,
                                    value, 0, 0, clickIndex, scrollByPixel);
                                break;

                            case BrowserFuncts.MouseScroll:
                                var xLocation = (int)paramS[0];
                                var yLocation = (int)paramS[1];
                                var ScrollX = paramS.Length > 2 ? (int)paramS[2] : 0;
                                var ScrollY = paramS.Length > 3 ? (int)paramS[3] : 0;
                                var delayAfter = paramS.Length > 4 ? (int)paramS[4] : 0;
                                var noOfScroll = paramS.Length > 5 ? (int)paramS[5] : 1;
                                await BrowserWindow.MouseScrollAsync(xLocation, yLocation, ScrollX,
                                    ScrollY, 0, delayAfter: delayAfter, clickLeavEvent: 0, scrollCount: noOfScroll);
                                break;

                            case BrowserFuncts.CheckAd:
                                var lengthValue = await BrowserWindow.GetElementValueAsync(ActType.GetLength,
                                    AttributeType.ClassName, "ytp-ad-skip-button-icon");
                                FoundAd = lengthValue == "1";
                                break;

                            case BrowserFuncts.Scroll:
                                var scrollByPixel1 = (int)paramS[0];
                                await BrowserWindow.BrowserActAsync(ActType.ScrollWindow, AttributeType.Null,
                                    "", scrollByPixel: scrollByPixel1);
                                break;

                            case BrowserFuncts.GetElementValue:
                                var actType1 = (ActType)paramS[0];
                                var attributeType1 = (AttributeType)paramS[1];
                                var attributeValue1 = paramS.Length > 2 ? paramS[2].ToString() : "";
                                var ValueTypess = (ValueTypes)paramS[3];
                                var clickIndex1 = paramS.Length > 4 ? (int)paramS[4] : 0;
                                CurrentData = await BrowserWindow.GetElementValueAsync(actType1, attributeType1,
                                                  attributeValue1, ValueTypess, 0, clickIndex1) ?? "";
                                //"Play (k)"
                                break;

                            case BrowserFuncts.GetXY:
                                var attribute = (AttributeType)paramS[0];
                                var elementName = paramS[1].ToString();
                                var index = paramS.Length > 2 ? (int)paramS[2] : 0;
                                lastXandY = await BrowserWindow.GetXAndYAsync(attribute, elementName, index);
                                break;
                            case BrowserFuncts.GetCustomXY:
                                var customx = paramS[0].ToString();
                                var customy = paramS[1].ToString();
                                lastCustomXandY = await BrowserWindow.GetXAndYAsync(customScriptX: customx, customScriptY: customy);
                                break;
                            case BrowserFuncts.SelectFileFromDialog:
                                var filePath = paramS[0].ToString();
                                BrowserWindow.ChooseFileFromDialog(filePath);
                                break;

                            case BrowserFuncts.GoBack:
                                var nTimesGoBack = paramS.Length > 0 ? (int)paramS[0] : 1;
                                BrowserWindow.GoBack(nTimesGoBack);
                                break;

                            case BrowserFuncts.RefreshPage:
                                BrowserWindow.Refresh();
                                break;
                            case BrowserFuncts.IsDisposed:
                                _isDisposed = BrowserWindow.IsDisposed;
                                break;

                            case BrowserFuncts.IsLoaded:
                                _isLoaded = BrowserWindow.IsLoaded;
                                break;

                            case BrowserFuncts.SelectAll:
                                BrowserWindow.SelectAllText();
                                break;

                            case BrowserFuncts.GetBrowserCookies:
                                _account.Cookies = await BrowserWindow.BrowserCookiesIntoModel();
                                break;

                            case BrowserFuncts.SaveCookies:
                                var printLog = paramS.Length > 0 ? (bool)paramS[0] : false;
                                if (!IsLoggedIn)
                                {
                                    await BrowserWindow.SaveCookies(printLog);
                                    IsLoggedIn = true;
                                    _loginFailed = false;
                                }

                                break;

                            case BrowserFuncts.GetListInnerHtml:
                                var actType2 = (ActType)paramS[0];
                                var attributeType2 = (AttributeType)paramS[1];
                                var attributeValue2 = paramS.Length > 2 ? paramS[2].ToString() : "";
                                var valuetype = paramS.Length > 3 ? (ValueTypes)paramS[3] : ValueTypes.InnerHtml;
                                ResultList = await BrowserWindow.GetListInnerHtml(actType2, attributeType2, attributeValue2, valuetype);
                                break;


                            case BrowserFuncts.Close:
                                BrowserWindow.Close();
                                BrowserWindow.Dispose();
                                GC.Collect();
                                break;
                            case BrowserFuncts.LoadSource:
                                DateTime currentTime = DateTime.Now;
                                string pageSource;
                                do
                                {
                                    await Task.Delay(TimeSpan.FromSeconds(2), _cancellationToken);
                                    pageSource = await BrowserWindow.GetPageSourceAsync();
                                    _cancellationToken.ThrowIfCancellationRequested();
                                } while ((!BrowserWindow.IsLoaded && (DateTime.Now - currentTime).TotalSeconds < delayBefore + 45) || (string.IsNullOrEmpty(pageSource) && (DateTime.Now - currentTime).TotalSeconds < delayBefore));
                                _html = pageSource = CurrentData;
                                break;
                            case BrowserFuncts.ExecuteScript:
                                int delay = 0;
                                string script = (string)paramS[0];
                                if (paramS.Length > 1)
                                    delay = (int)paramS[1];
                                LastScriptResp = await BrowserWindow.ExecuteScriptAsync(script, delay);
                                break;
                            case BrowserFuncts.CopyPasteContent:
                                int delaylast = 0;
                                string content = (string)paramS[0];
                                if (paramS.Length > 1)
                                    delaylast = (int)paramS[1];
                                await BrowserWindow.CopyPasteContentAsync(content, 86, delayAtLast: delaylast);
                                break;

                        }
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                    isRunning = false;
                });
                while (isRunning)
                {
                    _cancellationToken.ThrowIfCancellationRequested();
                    Sleep(0.1);
                }
            }

        }


        private bool _isDisposed;
        private bool _isLoaded;

        private IResponseParameter Response => new ResponseParameter { Response = CurrentData };
        private KeyValuePair<int, int> lastXandY;

        #endregion

        #region YouTube Functions

        public bool BrowserLogin(DominatorAccountModel account, CancellationToken cancellationToken,
            LoginType loginType = LoginType.AutomationLogin, VerificationType verification = 0)
        {
            bool isRunning = true;
            InitializeProperties();
            _cancellationToken = cancellationToken;
            SetAccount(account);
            int tryLoginCount = 0;
            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    if (BrowserWindow == null || BrowserWindow != null && BrowserWindow.IsDisposed)
                    {
                        VerifyingAccount = _account.IsVerificationCodeSent;
                        var visible = loginType == LoginType.BrowserLogin ? true : false;
                        if (!visible)
                        {
#if DEBUG
                            visible = true;
#endif
                        }
                        BrowserWindow = new BrowserWindow(_account, customUse: CustomUse);
                        // Always make it Non async to avoid browserwindow visibility issue oe else it will open browser window
                        if (_account.Cookies?.Count > 0)
                            BrowserWindow.SetCookie();
                        BrowserWindow.Visibility = visible ? Visibility.Visible : Visibility.Hidden;
                        BrowserWindow.Show();
                        await Task.Delay(5000, _cancellationToken);
                        await LoadSource(timesec: 4);
                        TargetUrl = "https://www.youtube.com/";
                        while (!IsLoggedIn && tryLoginCount < 3)
                        {
                            tryLoginCount++;
                            _cancellationToken.ThrowIfCancellationRequested();
                            if (BrowserWindow.IsDisposed) return;
                            await GoogleBrowserLogin(loginType);
                            if (_loginFailed)
                                return;
                            if (!IsLoggedIn)
                                await Task.Delay(2000, _cancellationToken);

                        }
                    }
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
                { isRunning = false; }
            });
            while (isRunning)
                Task.Delay(2000).Wait(_cancellationToken);
            return account.IsUserLoggedIn;
        }
        public void SearchByKeywordOrHashTag(DominatorAccountModel account, string keyWordorHashtag, bool isHashTag = false, string searchFilterUrlParam = "EgIQAQ%3D%3D")
        {
            bool isRunning = true;
            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    var url = $"https://www.youtube.com/results?search_query={Uri.EscapeDataString(keyWordorHashtag)}";
                    if (!string.IsNullOrEmpty(searchFilterUrlParam))
                    {
                        BrowserWindow.SetResourceLoadInstance();
                        BrowserWindow.ClearResources();
                        url += $"&sp={searchFilterUrlParam}";
                        await BrowserWindow.GoToCustomUrl(url,delayAfter:5);
                    }
                    else
                    {
                        var textBoxPosition = await BrowserWindow.GetXAndYAsync(customScriptX: $"{string.Format(YdStatic.ByTagWithAttributesAndValuesButtonScript, "div>input", "[aria-label=\"Search\"i],div>input[placeholder=\"Search\"i]", "name", "search_query")}[0].getBoundingClientRect().x"
                                           , customScriptY: $"{string.Format(YdStatic.ByTagWithAttributesAndValuesButtonScript, "div>input", "[aria-label=\"Search\"i],div>input[placeholder=\"Search\"i]", "name", "search_query")}[0].getBoundingClientRect().y");
                        if (textBoxPosition.Key != 0 && textBoxPosition.Value != 0)
                            BrowserWindow.MouseClick(textBoxPosition.Key + 25, textBoxPosition.Value + 6, delayAfter: 2);
                        await BrowserWindow.EnterCharsAsync($" {keyWordorHashtag}", typingDelay: 0.3, delayAtLast: 5);
                        _cancellationToken.ThrowIfCancellationRequested();
                        BrowserWindow.SetResourceLoadInstance();
                        BrowserWindow.ClearResources();
                        await ClickOrScrollToViewAndClick($"{string.Format(YdStatic.ByTagWithAttributesAndExactValuesScript, "li>div", "[role=\"Option\"i]", "textContent", keyWordorHashtag, "textContent", $"Remove{keyWordorHashtag}Suggestion removed")}", url, timetoWait: 8, isLoadSource: true);
                        _cancellationToken.ThrowIfCancellationRequested();
                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                isRunning = false;
            });
            while (isRunning)
                Task.Delay(2000, _cancellationToken).Wait();
        }
        private void InitializeProperties()
        {
            _htmlHasUserName = false;
            _isLoaded = false;
            _langEngSet = false;
            _loginFailed = false;
            _pageText = "";
            _UserCount = 0;
            _UserStop = 1;
            IsLoggedIn = false;
            IsLoggedIn = false;
            CurrentData = "";
            CustomUse = false;
        }
        public string LoadPageSource(DominatorAccountModel account, string url, bool clearandNeedResource = false, bool isNewWindow = false, int timeSec = 15)
        {
            string pageSource = string.Empty;
            DateTime currentTime = DateTime.Now;
            bool isRunning = true;
            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    if (isNewWindow)
                    {
                        CustomBrowserWindow = new BrowserWindow(account, targetUrl: url, isNeedResourceData: clearandNeedResource, customUse: true)
                        {
                            Visibility = Visibility.Hidden
                        };
#if DEBUG
                        CustomBrowserWindow.Visibility = Visibility.Visible;
#endif
                        CustomBrowserWindow.SetCookie();
                        CustomBrowserWindow.Show();
                        await Task.Delay(7000, _cancellationToken);
                        do
                        {
                            await Task.Delay(1500);
                            pageSource = await CustomBrowserWindow.GetPageSourceAsync();
                        } while ((!CustomBrowserWindow.IsLoaded && (DateTime.Now - currentTime).TotalSeconds < timeSec + 15) || (string.IsNullOrEmpty(pageSource) && (DateTime.Now - currentTime).TotalSeconds < timeSec));
                    }
                    else
                    {
                        if (BrowserWindow == null || BrowserWindow != null && BrowserWindow.IsDisposed)
                        {

                            BrowserWindow = new BrowserWindow(account, isNeedResourceData: clearandNeedResource)
                            {
                                Visibility = Visibility.Hidden
                            };
                            BrowserWindow.SetCookie();
#if DEBUG
                            BrowserWindow.Visibility = Visibility.Visible;
#endif

                            BrowserWindow.Show();
                            await Task.Delay(5000, _cancellationToken);
                            await LoadSource(timesec: 4);
                            await BrowserWindow.GoToCustomUrl(url, 7);
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
                        } while ((!BrowserWindow.IsLoaded && (DateTime.Now - currentTime).TotalSeconds < timeSec + 15) || (string.IsNullOrEmpty(pageSource) && (DateTime.Now - currentTime).TotalSeconds < timeSec));
                    }
                    pageSource = _html = CurrentData = await BrowserWindow.GetPageSourceAsync();
                }
                catch (Exception ex) { ex.DebugLog(); }
                isRunning = false;
            });
            while (isRunning)
                Task.Delay(2000, _cancellationToken).Wait();
            return pageSource;
        }
        public async Task LoadSource(int timesec = 15)
        {
            DateTime currentTime = DateTime.Now;
            do
            {
                await Task.Delay(TimeSpan.FromSeconds(2), _cancellationToken);
                CurrentData = await BrowserWindow.GetPageSourceAsync();
                _cancellationToken.ThrowIfCancellationRequested();
            } while ((!BrowserWindow.IsLoaded && (DateTime.Now - currentTime).TotalSeconds < timesec + 15) || (string.IsNullOrEmpty(CurrentData) && (DateTime.Now - currentTime).TotalSeconds < timesec));

        }

        #region Google Login

        private bool _htmlHasUserName;
        private int _UserCount;
        private int _UserStop = 1;

        private async Task GoogleBrowserLogin(LoginType loginType)
        {
            try
            {
                _html = await BrowserWindow.GetPageSourceAsync();
                if ((_html.Contains("\"LOGGED_IN\":true") || _html.Contains("\"logged_in\",\"value\":\"1\"")) && !IsLoggedIn)
                {
                    await BrowserWindow.SaveCookies(loginType == LoginType.BrowserLogin);
                    IsLoggedIn = true;
                    _loginFailed = false;
                    return;
                }
                if (_html.Contains("Sign in to like videos, comment, and subscribe."))
                {
                    await BrowserWindow.ClearCookies();
                    await Task.Delay(TimeSpan.FromSeconds(5), _cancellationToken);
                    _account.CookieHelperList.Clear();
                    _account.BrowserCookieHelperList.Clear();
                    await ClickOnSignInIfYoutubeNotLoggedIn();
                }
                if (_html == "<html><head></head><body></body></html>") return;
                bool isSuspended = await Task.Run(() => CheckSuspended());
                if (IsLoggedIn || _isDisposed || isSuspended)
                {
                    IsLoggedIn = true;
                    return;
                }
                CurrentData = await BrowserWindow.PageText();
                _pageText = CurrentData;
                _htmlHasUserName = !string.IsNullOrWhiteSpace(Utilities.GetBetween(_html, "\"oPEP7c\":\"", "\"")) || _html.Contains("\"logged_in\",\"value\":\"1\"") ||
                                  _html.Contains("\"LOGGED_IN\":true") || _pageText.Contains("Protect your account") ||
                                  _pageText.Contains("secure your account");
                if (!_langEngSet)
                    await SetGoogleLangAsEng();
                lastCustomXandY = await BrowserWindow.GetXAndYAsync(customScriptX: $"{YdStatic.DismissButtonScript}[0].getBoundingClientRect().x", customScriptY: $"{YdStatic.DismissButtonScript}[0].getBoundingClientRect().y");

                if (lastCustomXandY.Key != 0 && lastCustomXandY.Value != 0)
                    await BrowserWindow.MouseClickAsync(lastCustomXandY.Key + 10, lastCustomXandY.Value + 5);
                if (_pageText.Contains("Choose an account\n"))
                {
                    await BrowserWindow.PressAnyKeyUpdated(9, 1, 1, 0);
                    await BrowserWindow.PressAnyKeyUpdated(13, 1, 1, 7);
                    CurrentData = await BrowserWindow.PageText();
                    _pageText = CurrentData;
                }
                if (_pageText.Contains("We've detected a problem with your cookie settings"))
                {
                    await BrowserWindow.ClearCookies();
                    await Task.Delay(TimeSpan.FromSeconds(5), _cancellationToken);
                    _account.CookieHelperList.Clear();
                    _account.BrowserCookieHelperList.Clear();
                    _account.AccountBaseModel.Status = AccountStatus.TryingToLogin;
                    await BrowserWindow.GoToCustomUrl("https://accounts.google.com/signin", delayAfter: 6);
                    await LoadSource();
                }
                if (!IsLoggedIn &&
                   (_pageText.Contains("I'm not a robot") || _pageText.Contains("Verify your identity") ||
                    _pageText.Contains("\n\nEnter verification code\n\n") || _pageText.Contains("English (")))

                {
                    //Here _UserCont and _UserStop are used just for entering username if language change is made  
                    if ((!(_UserCount++ > _UserStop) || _pageText.Contains("\nForgot email?\n") &&
                                                               (_pageText.Contains("\nEmail or phone\n") ||
                                                                _pageText.ToLower()
                                                                    .Contains($"\n{_account.AccountBaseModel.UserName.Trim().ToLower()}\n")) && !_pageText.Contains(
                                                                   "Confirm the recovery email address")) &&
                                                              !_pageText.Contains("\nEnter your password"))
                    {

                        if (!_pageText.Contains("\nEmail or phone\n") && !_pageText.Contains("English ("))
                        {
                            _langEngSet = false;
                            return;
                        }
                        _langEngSet = true;
                        lastCustomXandY = await BrowserWindow.GetXAndYAsync(customScriptX: $"{string.Format(YdStatic.ByTagAttributeAndValueButtonScript, "div>input", "type", "email", "ariaLabel", "email or phone")}[0].getBoundingClientRect().x", customScriptY: $"{string.Format(YdStatic.ByTagAttributeAndValueButtonScript, "div>input", "type", "email", "ariaLabel", "email or phone")}[0].getBoundingClientRect().y");

                        if (lastCustomXandY.Key != 0 && lastCustomXandY.Value != 0)
                        {
                            await BrowserWindow.MouseClickAsync(lastCustomXandY.Key + 35, lastCustomXandY.Value + 15);
                            var inputText = (await BrowserWindow.ExecuteScriptAsync($"{string.Format(YdStatic.ByTagAttributeAndValueButtonScript, "div>input", "type", "email", "ariaLabel", "email or phone")}[0].innerText"))?.Result?.ToString() ?? "";
                            if (!string.IsNullOrEmpty(inputText))
                            {
                                BrowserWindow.SelectAllText();
                                await BrowserWindow.PressAnyKeyUpdated(8);
                            }
                            await BrowserWindow.EnterCharsAsync(" " + _account.AccountBaseModel.UserName, delayAtLast: 1);
                            lastCustomXandY = await BrowserWindow.GetXAndYAsync(customScriptX: $"{string.Format(YdStatic.ByTagAttributeAndValueButtonScript, "div>button", "type", "button", "textContent", "next")}[0].getBoundingClientRect().x", customScriptY: $"{string.Format(YdStatic.ByTagAttributeAndValueButtonScript, "div>button", "type", "button", "textContent", "next")}[0].getBoundingClientRect().y");

                            if (lastCustomXandY.Key != 0 && lastCustomXandY.Value != 0)
                                await BrowserWindow.MouseClickAsync(lastCustomXandY.Key + 35, lastCustomXandY.Value + 15, delayAfter: 5);
                            else
                                await BrowserWindow.PressAnyKeyUpdated(13, 1, 1, 6);
                        }

                        CurrentData = await BrowserWindow.PageText();
                        _pageText = CurrentData;
                        if ((_pageText.Contains("Type the text you hear or see") &&
                           _pageText.Contains("\nForgot email?\n") && _pageText.Contains("\nEmail or phone\n") &&
                           BrowserWindow.Visibility == Visibility.Visible) || (_pageText.Contains("Verify it’s you") && _pageText.Contains("Confirm you’re not a robot") && BrowserWindow.Visibility == Visibility.Visible))
                        {
                            try
                            {
                                await Task.Run(() => SolveCaptcha());
                            }
                            catch (Exception)
                            { }
                        }

                    }
                    if (_pageText.Contains("Confirm that you're not a robot"))
                    {
                        var tryToSolveCaptcha = 5;

                        while (_pageText.Contains("Confirm that you're not a robot") &&
                              BrowserWindow.Visibility == Visibility.Visible && tryToSolveCaptcha > 0)
                        {
                            BrowserDispatcher(BrowserFuncts.BrowserAct, 0, ActType.EnterValue, AttributeType.Name,
                               "password", _account.AccountBaseModel.Password);
                            ToasterNotification.ShowInfomation(
                               $"Please enter Captcha manualy within {_youtubeModel.TimeoutToSolveCaptchaManually} seconds.\nand do not click any button by yourself.");
                            await Task.Delay(_youtubeModel.TimeoutToSolveCaptchaManually * 1000, _cancellationToken);
                            await BrowserWindow.PressAnyKeyUpdated(13, 1);
                            tryToSolveCaptcha--;
                        }
                    }
                    if (_pageText.Contains("I'm not a robot") ||
                       _pageText.ToLower().Contains(_account.AccountBaseModel.UserName.ToLower()) ||
                       _pageText.Contains("To continue, first verify it's you") ||
                       _pageText.Contains("\nEnter your password\n"))
                    {
                        if (_pageText.Contains("Enter your password"))
                        {
                            lastCustomXandY = await BrowserWindow.GetXAndYAsync(customScriptX: $"{string.Format(YdStatic.ByTagAttributeAndValueButtonScript, "div>input", "type", "password", "ariaLabel", "enter your password")}[0].getBoundingClientRect().x", customScriptY: $"{string.Format(YdStatic.ByTagAttributeAndValueButtonScript, "div>input", "type", "password", "ariaLabel", "enter your password")}[0].getBoundingClientRect().y");

                            if (lastCustomXandY.Key != 0 && lastCustomXandY.Value != 0)
                                await BrowserWindow.MouseClickAsync(lastCustomXandY.Key + 35, lastCustomXandY.Value + 15);
                            await BrowserWindow.EnterCharsAsync(" " + _account.AccountBaseModel.Password, delayAtLast: 1);
                            lastCustomXandY = await BrowserWindow.GetXAndYAsync(customScriptX: $"{string.Format(YdStatic.ByTagAttributeAndValueButtonScript, "div>button", "type", "button", "textContent", "next")}[0].getBoundingClientRect().x", customScriptY: $"{string.Format(YdStatic.ByTagAttributeAndValueButtonScript, "div>button", "type", "button", "textContent", "next")}[0].getBoundingClientRect().y");

                            if (lastCustomXandY.Key != 0 && lastCustomXandY.Value != 0)
                                await BrowserWindow.MouseClickAsync(lastCustomXandY.Key + 35, lastCustomXandY.Value + 15, delayAfter: 10);
                            else
                                await BrowserWindow.PressAnyKeyUpdated(13, 1, 1, 10);
                            CurrentData = await BrowserWindow.PageText();
                            _pageText = CurrentData;
                        }
                        if (_pageText.Contains("Wrong password") || _pageText.Contains("Your password was changed"))
                        {
                            _account.AccountBaseModel.Status = AccountStatus.InvalidCredentials;
                            _loginFailed = true;
                            return;
                        }
                        if (_pageText.Contains("We've detected a problem with your cookie settings"))
                        {

                            await BrowserWindow.ClearCookies();
                            _account.CookieHelperList.Clear();
                            _account.BrowserCookieHelperList.Clear();
                            _account.AccountBaseModel.Status = AccountStatus.TryingToLogin;
                            await BrowserWindow.GoToCustomUrl("https://accounts.google.com/signin", delayAfter: 7);
                            await LoadSource();
                            return;
                        }
                        if (_pageText.Contains("Verify it’s you") || _pageText.Contains("Confirm your recovery email") || _pageText.Contains("2-Step Verification"))
                        {
                            var verfyWays = Utilities.GetBetween(_pageText, "\nChoose how you want to sign in:\n", "\nGet help\n").Split(new char[] { '\n' });
                            var count = 1;
                            if (_pageText.Contains("Confirm your recovery email"))
                            {
                                count = verfyWays.IndexOf("Confirm your recovery email") + 3;
                                await BrowserWindow.PressAnyKeyUpdated(9, count, delayAtLast: 1);
                                await BrowserWindow.PressAnyKeyUpdated(13, 1, delayAtLast: 3);
                                await BrowserWindow.EnterCharsAsync(" " + _account.AccountBaseModel.AlternateEmail, delayAtLast: 2);
                                await BrowserWindow.PressAnyKeyUpdated(13, 1, delayAtLast: 8);
                            }
                            if (_pageText.Contains("Google needs to verify it’s you. Please sign in again to continue"))
                            {
                                lastCustomXandY = await BrowserWindow.GetXAndYAsync(customScriptX: $"{string.Format(YdStatic.ByTagAttributeAndValueButtonScript, "div>button", "type", "button", "textContent", "next")}[0].getBoundingClientRect().x", customScriptY: $"{string.Format(YdStatic.ByTagAttributeAndValueButtonScript, "div>button", "type", "button", "textContent", "next")}[0].getBoundingClientRect().y");

                                if (lastCustomXandY.Key != 0 && lastCustomXandY.Value != 0)
                                    await BrowserWindow.MouseClickAsync(lastCustomXandY.Key + 35, lastCustomXandY.Value + 15, delayAfter: 5);
                                else
                                    await BrowserWindow.PressAnyKeyUpdated(13, 1, 1, 6);
                            }
                            if (_pageText.Contains("Get a verification code at") && _pageText.Contains("Choose how you want to sign in"))
                            {
                                if (!_account.IsVerificationCodeSent)
                                {
                                    _account.AccountBaseModel.Status = AccountStatus.EmailVerification;
                                    _loginFailed = true;
                                    return;
                                }
                                else
                                {

                                    if (int.TryParse((await BrowserWindow.ExecuteScriptAsync($"{string.Format(YdStatic.ByTagAttributeAndValueButtonScript, "li>div", "role", "link", "textContent", "get a verification code at")}.length"))?.Result?.ToString(), out int inputCount) && inputCount > 0)
                                    {
                                        lastCustomXandY = await BrowserWindow.GetXAndYAsync(customScriptX: $"{string.Format(YdStatic.ByTagAttributeAndValueButtonScript, "li>div", "role", "link", "textContent", "get a verification code at")}[0].getBoundingClientRect().x", customScriptY: $"{string.Format(YdStatic.ByTagAttributeAndValueButtonScript, "li>div", "role", "link", "textContent", "get a verification code at")}[0].getBoundingClientRect().y");

                                        if (lastCustomXandY.Key != 0 && lastCustomXandY.Value != 0)
                                            await BrowserWindow.MouseClickAsync(lastCustomXandY.Key + 35, lastCustomXandY.Value + 15);
                                    }
                                    else
                                    {
                                        verfyWays.ForEach(x =>
                                        {
                                            if (x.Contains("Get a verification code at"))
                                                count = verfyWays.IndexOf(x) + 3;
                                        });
                                        await BrowserWindow.PressAnyKeyUpdated(9, count, delayAtLast: 1);
                                        await BrowserWindow.PressAnyKeyUpdated(13, 1, delayAtLast: 3);
                                    }
                                    _account.IsVerificationCodeSent = false;
                                    var timeForNext5Minutes = DateTime.Now.AddMinutes(4);
                                    do
                                    {
                                        await Task.Delay(1000, _cancellationToken);
                                    }
                                    while (DateTime.Now < timeForNext5Minutes && (string.IsNullOrEmpty(_account.VarificationCode) || !_account.IsVerifyButtonClicked));

                                    if (!string.IsNullOrEmpty(_account.VarificationCode))
                                    {
                                        await BrowserWindow.EnterCharsAsync(" " + _account.VarificationCode, delayAtLast: 2);
                                        await BrowserWindow.PressAnyKeyUpdated(13, 1, delayAtLast: 8);
                                    }
                                    else
                                    {
                                        _account.AccountBaseModel.Status = AccountStatus.EmailVerification;
                                        _loginFailed = true;
                                        return;
                                    }
                                }
                            }
                            if (_pageText.Contains("2-Step Verification") && _pageText.Contains("Get a verification code"))
                            {
                                if (int.TryParse((await BrowserWindow.ExecuteScriptAsync($"{string.Format(YdStatic.ByTagAttributeAndValueButtonScript, "input", "type", "tel", "id", "phonenumberid")}.length"))?.Result?.ToString(), out int inputCount) && inputCount > 0)
                                {
                                    if (!_account.IsVerificationCodeSent)
                                    {
                                        _account.AccountBaseModel.Status = AccountStatus.PhoneVerification;
                                        _loginFailed = true;
                                        return;
                                    }
                                    else
                                    {
                                        if (_account.IsVerificationCodeSent && string.IsNullOrEmpty(_account.AccountBaseModel.PhoneNumber))
                                        {
                                            _account.AccountBaseModel.Status = AccountStatus.AddPhoneNumberToYourAccount;
                                            _loginFailed = true;
                                            return;
                                        }
                                        if (!string.IsNullOrEmpty(_account.AccountBaseModel.PhoneNumber))
                                        {
                                            await BrowserWindow.EnterCharsAsync(" " + _account.AccountBaseModel.PhoneNumber, delayAtLast: 2);
                                            await BrowserWindow.PressAnyKeyUpdated(13, 1, delayAtLast: 8);
                                            _account.IsVerificationCodeSent = false;
                                        }
                                        var timeForNext5Minutes = DateTime.Now.AddMinutes(4);
                                        do
                                        {
                                            await Task.Delay(1000, _cancellationToken);
                                        }
                                        while (DateTime.Now < timeForNext5Minutes && (string.IsNullOrEmpty(_account.VarificationCode) || !_account.IsVerifyButtonClicked));

                                        if (!string.IsNullOrEmpty(_account.VarificationCode))
                                        {
                                            await BrowserWindow.EnterCharsAsync(" " + _account.VarificationCode, delayAtLast: 2);
                                            await BrowserWindow.PressAnyKeyUpdated(13, 1, delayAtLast: 8);
                                        }
                                        else
                                        {
                                            _account.AccountBaseModel.Status = AccountStatus.PhoneVerification;
                                            _loginFailed = true;
                                            return;
                                        }
                                    }
                                }


                            }
                            if (_pageText.Contains("Google sent a notification") && _pageText.Contains("Try another way"))
                            {
                                await BrowserWindow.PressAnyKeyUpdated(13, 1, delayAtLast: 6);
                            }
                        }
                        //var tryToSolveCaptcha = 5;

                        //while (_pageText.Contains("Type the text you hear or see") &&
                        //       BrowserWindows.Last().Visibility == Visibility.Visible && tryToSolveCaptcha > 0)
                        //{
                        //    BrowserDispatcher(BrowserFuncts.BrowserAct, 0, ActType.EnterValue, AttributeType.Name,
                        //        "password", _account.AccountBaseModel.Password);
                        //    ToasterNotification.ShowInfomation(
                        //        $"Please enter Captcha manualy within {_youtubeModel.TimeoutToSolveCaptchaManually} seconds.\nand do not click any button by yourself.");
                        //    Sleep(_youtubeModel.TimeoutToSolveCaptchaManually);
                        //    BrowserDispatcher(BrowserFuncts.PressKey, 1, 13, 1,
                        //        0); //Press Enter key //BrowserAct(ActType.ClickById,"passwordNext", 2, 2);
                        //    BrowserDispatcher(BrowserFuncts.GetPageText, 4);
                        //    if (_pageText.Contains("Type the text you hear or see"))
                        //    {
                        //        --tryToSolveCaptcha;
                        //        ToasterNotification.ShowInfomation(
                        //            $"Wrong captcha text entered.\nplease try again\n{tryToSolveCaptcha} chances left to solve captcha.");
                        //    }
                        //}
                        CurrentData = await BrowserWindow.PageText();
                        _pageText = CurrentData;
                        _html = await BrowserWindow.GetPageSourceAsync();
                        _htmlHasUserName = _html.Contains("\"LOGGED_IN\":true") || _html.Contains("\"logged_in\",\"value\":\"1\"") ||
                                          _pageText.Contains("Protect your account") ||
                                          _pageText.Contains("secure your account") ||

                                          !string.IsNullOrWhiteSpace(
                                              Utilities.GetBetween(_html, "\"oPEP7c\":\"", "\""));
                    }
                }
                if (await ClickOnSignInIfYoutubeNotLoggedIn()) return;

                if (CustomUse == true)
                    CustomUse = false;

                if (!IsLoggedIn && _htmlHasUserName && !CustomUse)
                {
                    if (string.IsNullOrEmpty(TargetUrl))
                        TargetUrl = HomeUrl;
                    CurrentData = BrowserWindow.CurrentUrl();
                    if (!(CustomUse || string.IsNullOrEmpty(TargetUrl)) && CurrentData != TargetUrl)
                    {
                        await BrowserWindow.GoToCustomUrl(TargetUrl, delayAfter: 7);
                        await LoadSource();
                        CurrentData = await BrowserWindow.PageText();
                        _pageText = CurrentData;
                        if (_pageText.Contains("Select a channel"))
                        {
                            await BrowserWindow.PressAnyKeyUpdated(9, delayAtLast: 1);
                            await BrowserWindow.PressAnyKeyUpdated(13, 1, delayAtLast: 7);
                        }
                    }
                    if (await CheckGoogleLogin() && !IsLoggedIn)
                    {
                        await BrowserWindow.SaveCookies(loginType == LoginType.BrowserLogin);
                        IsLoggedIn = true;
                        _loginFailed = false;
                    }

                }
            }
            catch
            {
            }
        }


        private void SolveCaptcha()
        {
            bool iscaptchSolved = true;
            int n = 0;
            try
            {
                var genericFileManager = InstanceProvider.GetInstance<IGenericFileManager>();
                var imageCaptchaServicesModel = genericFileManager.GetModel<ImageCaptchaServicesModel>(ConstantVariable.GetImageCaptchaServicesFile());
                ImageTypersHelper imagetyperz = new ImageTypersHelper(imageCaptchaServicesModel.Token);
                while (iscaptchSolved && n < 3)
                {
                    Sleep(1);
                    _pageText = BrowserWindow.GetPageSource();
                    var imagepath = "https://accounts.google.com" + HtmlParseUtility.GetAttributeValueFromId(_pageText, "captchaimg", "src").Replace("amp;", "");
                    var downloadpath = YoutubeUtilities.GetDownloadedMediaPath();
                    string fileName = $"{downloadpath}\\{DateTime.Now.GetCurrentEpochTime().ToString()}.jpg";
                    WebClient web = new WebClient();
                    web.DownloadFile(imagepath, fileName);
                    string captchaText = imagetyperz.submit_image(fileName, _account.AccountBaseModel.UserName);
                    BrowserWindow.ExecuteScript($"document.getElementById('ca').value='{captchaText}'");
                    Sleep(3);
                    _pageText = BrowserWindow.GetPageSource();
                    if (_pageText.Contains("Confirm you’re not a robot"))
                        BrowserWindow.ExecuteScript($"document.getElementById('recaptcha-anchor').click();");
                    Sleep(3);
                    BrowserWindow.ExecuteScript("document.getElementsByClassName('VfPpkd-LgbsSe VfPpkd-LgbsSe-OWXEXe-k8QpJ VfPpkd-LgbsSe-OWXEXe-dgl2Hf nCP5yc AjY5Oe DuMIQc qIypjc TrZEUc lw1w4b')[0].click()");
                    Sleep(5);
                    _pageText = BrowserWindow.GetPageSource();
                    if (_pageText.Contains("Enter your password") && _pageText.Contains("Show password"))
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.YouTube, _account.AccountBaseModel.UserName, "Captcha", "Captcha Solved");
                        iscaptchSolved = false;
                    }
                    else
                    {
                        n++;
                    }
                }



            }
            catch (Exception)
            {

            }
        }
        public async Task<bool> ClickOrScrollToViewAndClick(string script, string destinationUrl = "", int timetoWait = 3, bool isLoadSource = false)
        {
            var Success = false;
            try
            {

                var elementLocation = await BrowserWindow.GetXAndYAsync(customScriptX: $"{script}[0].getBoundingClientRect().x", customScriptY: $"{script}[0].getBoundingClientRect().y");

                if (elementLocation.Key != 0 && elementLocation.Value != 0 && (elementLocation.Value < 60 || elementLocation.Value > ScreenResolution.Value - 150))
                {
                    await BrowserWindow.ExecuteScriptAsync($"{script}[0].scrollIntoViewIfNeeded()", delayInSec: 4);
                    elementLocation = await BrowserWindow.GetXAndYAsync(customScriptX: $"{script}[0].getBoundingClientRect().x", customScriptY: $"{script}[0].getBoundingClientRect().y");

                }
                if (elementLocation.Value > 60 && elementLocation.Value < ScreenResolution.Value - 150)
                {
                    BrowserWindow.MouseClick(elementLocation.Key + 25, elementLocation.Value + 11, delayAfter: timetoWait);
                    Success = true;
                }
                else
                {
                    if (string.IsNullOrEmpty(destinationUrl))
                        Success = (await BrowserWindow.ExecuteScriptAsync($"{script}[0].click()", timetoWait)).Success;
                    else
                    {
                        await BrowserWindow.GoToCustomUrl(destinationUrl, timetoWait);
                        Success = true;
                    }
                }
                if (isLoadSource && ((elementLocation.Key != 0 && elementLocation.Value != 0) || (!string.IsNullOrEmpty(destinationUrl))))
                    await LoadSource();
            }
            catch (Exception e) { e.DebugLog(); }
            return Success;
        }

        private bool _langEngSet;

        private async Task SetGoogleLangAsEng()
        {
            try
            {
                if (!_langEngSet)
                {
                    CurrentData = BrowserWindow.CurrentUrl();
                    if (IsLoggedIn || Uri.UnescapeDataString(CurrentData.ToLower()).Contains("www.youtube.com/watch?")
                                   || CurrentData == "https://www.youtube.com/"
                                   || CurrentData == "https://youtube.com/"
                                   || _htmlHasUserName || string.IsNullOrEmpty(_pageText) ||
                                   _pageText == "Account\n\n\n"
                                   || _pageText.Contains("Protect your account") ||
                                   _pageText.Contains("Loading, please wait ...")
                                   || /*(_pageText.Contains("English (") ||*/
                                   _pageText.Contains("Use your Google Account") ||
                                   _pageText.Contains("Enter your password") || _pageText.Contains("Personal info") || _pageText.Contains("English ("))
                    {
                        _langEngSet = true;
                        return;
                    }

                    _langEngSet = true;
                    // Open Google Language Box in Browser
                    await ClickOrScrollToViewAndClick($"{string.Format(YdStatic.ByTagAttributeAndValueAndClassScript, "div", "role", "combobox", "ariaHasPopup", "listbox", "VfPpkd-ksKsZd-mWPk3d")}", timetoWait: 5);
                    await ClickOrScrollToViewAndClick($"{string.Format(YdStatic.ByTagAttributeAndValueButtonScript, "li", "role", "option", "textContent", "english (united states)")}", timetoWait: 5);
                    await LoadSource(7);

                }
            }
            catch
            {
                /* Ignored */
            }
        }

        private Dictionary<Predicate<string>, Func<bool>> _predicateDict;
        private bool _loginFailed;

        private void InitializeGoogleLoginStatusActions()
        {
            _predicateDict = new Dictionary<Predicate<string>, Func<bool>>
            {
                {
                    pageData =>
                        pageData.Contains("Couldn't find your Google Account") || pageData.Contains(
                                                                                   "Enter a valid email or phone number")
                                                                               || pageData.Contains(
                                                                                   "Wrong password. Try again or click Forgot password to reset it")
                                                                               || pageData.Contains(
                                                                                   "Your password was changed")
                                                                               || pageData.Contains("Wrong password. Try again or click ‘Forgot password’ to reset it."),
                    InvalidCredentials
                },
                {
                    pageData => pageData.Contains("Change password")
                                && pageData.Contains(
                                    "There's been suspicious activity on your Google Account. For your protection, you need to change your password.")
                                && pageData.Contains("Those passwords didn't match. Try again."),
                    SetNewPasswordNotMatched
                },
                {
                    pageData => pageData.Contains("Change password")
                                && pageData.Contains(
                                    "There's been suspicious activity on your Google Account. For your protection, you need to change your password.")
                                && pageData.Contains(
                                    "Use a mix of letters, numbers and symbols to create a stronger password"),
                    SetNewPasswordCreateStrongPassword
                },
                {
                    pageData => pageData.Contains("Change password")
                                && pageData.Contains(
                                    "There's been suspicious activity on your Google Account. For your protection, you need to change your password.")
                                && pageData.Contains("Create password"),
                    SetNewPasswordAfterSuspiciousActivity
                },
                {
                    pageData =>
                        pageData.Contains("try to restore your account by submitting a request for review."),
                    AccountDisabled
                },
                {
                    pageData => pageData.Contains("Couldn't sign you in") &&
                                (pageData.Contains("doesn't allow us to keep your account secure.") ||
                                 pageData.Contains("app may not be secure.")),
                    BrowserNotSecure
                },
                {
                    pageData => pageData.Contains("Try another way to sign in\nGet a verification code at") &&
                                pageData.Contains("\nConfirm your recovery phone number\n"),
                    ClickOptionConfirmRecoveryClickIndex2
                },
                {
                    pageData =>
                        pageData.Contains("Unavailable because of too many attempts. Please try again later.")
                        || pageData.Contains(
                            "It is not available because too many attempts have been failed. Try again in a few hours.")
                        || pageData.Contains("Too many failed attempts") &&
                        pageData.Contains("Unavailable because of too many failed attempts. Try again in a few hours."),
                    ManyAttemptsOnPhoneVerification
                },
                {
                    pageData => pageData.Contains("Confirm your recovery email") || // Confirm your recovery email
                                pageData.Contains("Confirm your recovery phone number"),
                    ClickOptionConfirmRecovery
                },
                {pageData => pageData.Contains("Confirm the recovery email address"), ConfirmRecoveryEmailAddress},
                {pageData => pageData.Contains("Confirm the phone number"), ConfirmRecoveryPhoneNumber},
                {
                    pageData =>
                        pageData.Contains("Enter a phone number to get a text message with a verification code") ||
                        pageData.Contains(
                            "Provide a phone number to continue. We'll send a verification code you can use to sign in.") ||
                        pageData.Contains(
                            "Provide a phone number to continue. We'll send a verification code that you can use to sign in."),
                    AddPhoneNumber
                },
                {
                    pageData => pageData.Contains("Enter verification code") ||
                                pageData.Contains("A text message with a 6-digit verification code was just sent to"),
                    VerifyCodeFromPhone
                },
                {
                    pageData => pageData.Contains("Get a verification code")
                                || pageData.Contains("Do you have your phone?")
                                || pageData.Contains("Google will send a verification code to") &&
                                pageData.Contains("Standard rates apply"),
                    NeedPhoneVerification
                },
                {
                    pageData => pageData.Contains("Get a verification code")
                                && !pageData.Contains("Google will send a notification to"),
                    NeedEmailVerification
                },
                {
                    pageData => pageData.Contains("Type the text you hear or see")
                                || pageData.Contains("Google couldn't verify this account belongs to you."),
                    NeedsVerification
                },
                {
                    pageData => pageData.Contains("An error occurred. please try again.") ||
                                pageData.ToLower().Contains("something went wrong"),
                    FailedGotUnknownError
                },
                {
                    pageData => pageData.Contains("Protect your account") &&
                                pageData.Contains("Tell Google how to reach you in case you forget your password"),
                    () =>
                    {
                        _html = _account.UserName.ToLower();
                        return false;
                    }
                },
                {pageData => pageData.Contains("You've tried to sign in too many times."), TooManyAttemptsOnSignIn}
            };
        }



        private bool CheckSuspended()
        {
            CurrentData = BrowserWindow.CurrentUrl();
            if (!string.IsNullOrEmpty(CurrentData) && CurrentData.Equals(
                    "https://support.google.com/accounts/answer/40039?p=youtube",
                    StringComparison.InvariantCultureIgnoreCase))
            {
                _account.AccountBaseModel.Status = AccountStatus.ProfileSuspended;
                _loginFailed = true;
                IsLoggedIn = false;
                return true;
            }

            return false;
        }

        private bool RetypeEmail()
        {
            var isRetype = true;
            if (_account.AccountBaseModel.Status != AccountStatus.ReTypeEmail &&
                !string.IsNullOrEmpty(_account.AccountBaseModel.AlternateEmail) &&
                !_account.AccountBaseModel.AlternateEmail.Contains("•"))
            {
                BrowserDispatcher(BrowserFuncts.BrowserAct, 1.5, ActType.EnterValueById, AttributeType.Id,
                    "identifierId", _account.AccountBaseModel.AlternateEmail);
                BrowserDispatcher(BrowserFuncts.EnterChars, 0, _account.AccountBaseModel.AlternateEmail);
                BrowserDispatcher(BrowserFuncts.PressKey, 2, 13, 1,
                    0); // Press Enter Key  //BrowserAct(ActType.ClickByClass, "RveJvd snByac", delayAfter: 2.5);
                BrowserDispatcher(BrowserFuncts.GetPageText, 3.5);

                if (_pageText.Contains(
                        "Provide a phone number to continue. We'll send a verification code you can use to sign in") ||
                    _pageText.Contains("Enter a phone number to get a text message with a verification code"))
                    AddPhoneNumber();

                isRetype = _pageText.Contains("The email you entered is incorrect. Try again.") ||
                           _pageText.Contains("Try again with a valid email address");
                if (isRetype)
                    CustomLog("Alternate Email is incorrect for verification");
            }

            if (isRetype)
                _account.AccountBaseModel.Status = AccountStatus.ReTypeEmail;

            return isRetype;
        }

        private bool RetypePhoneNumber()
        {
            var isRetype = true;
            if (_account.AccountBaseModel.Status != AccountStatus.ReTypePhoneNumber &&
                !string.IsNullOrEmpty(_account.AccountBaseModel.PhoneNumber) &&
                !_account.AccountBaseModel.PhoneNumber.Contains("•"))
            {
                if (_cameHereByClickingOption)
                {
                    BrowserDispatcher(BrowserFuncts.PressKey, 0, 9, 5, 150); // Press Tab 5 times 
                    BrowserDispatcher(BrowserFuncts.BrowserAct, 1, ActType.EnterValueById, AttributeType.Id,
                        "phoneNumberId", _account.AccountBaseModel.PhoneNumber);
                    _cameHereByClickingOption = false;
                }
                else
                {
                    BrowserDispatcher(BrowserFuncts.EnterChars, 0, _account.AccountBaseModel.PhoneNumber);
                }

                BrowserDispatcher(BrowserFuncts.PressKey, 0, 13,
                    1); //Press Enter key // BrowserAct(ActType.ClickByClass, "RveJvd snByac", delayAfter: 3);
                BrowserDispatcher(BrowserFuncts.GetPageText, 3);

                isRetype = _pageText.Contains("This number doesn't match the one you provided. Try again.");
                if (isRetype)
                    CustomLog(
                        $"This number({_account.AccountBaseModel.PhoneNumber}) doesn't match the one you provided. Try again.");
            }

            if (isRetype)
                _account.AccountBaseModel.Status = AccountStatus.ReTypePhoneNumber;

            return isRetype;
        }

        private bool VerifyCodeFromPhone()
        {
            if ((_account.AccountBaseModel.Status == AccountStatus.PhoneVerification && !VerifyingAccount
                 || _account.AccountBaseModel.Status == AccountStatus.TooManyAttemptsOnPhoneVerification
                 || _account.AccountBaseModel.Status == AccountStatus.AddPhoneNumberToYourAccount) &&
                !_account.IsVerificationCodeSent)
                return true;
            BrowserDispatcher(BrowserFuncts.GetPageText);

            if (!_pageText.Contains("\n\nEnter verification code\n\n"))
                BrowserDispatcher(BrowserFuncts.PressKey, 0, 9, 3, 200); //Press Tab 3 Times key

            var isWrong = true;
            var codeBefore = "";
            do
            {
                var last2Min = DateTime.Now;

                while ((!_account.IsVerificationCodeSent || codeBefore == _account.VarificationCode.Trim() ||
                        _account.VarificationCode.Trim().Length < 6) && !_isDisposed &&
                       last2Min.AddMinutes(2) > DateTime.Now)
                    Sleep(2); // Waiting to get code from UI

                _account.VarificationCode.Trim();
                if (_account.VarificationCode.Trim().Length > 0)
                {
                    BrowserDispatcher(BrowserFuncts.SelectAll);
                    BrowserDispatcher(BrowserFuncts.PressKey, 0, 8);
                    BrowserDispatcher(BrowserFuncts.EnterChars, 2, " " + _account.VarificationCode.Trim(),
                        0.3); //Entering Verification Code
                    BrowserDispatcher(BrowserFuncts.GetPageText, 2);

                    //(don't delete) Enter verification code------------->> will use this (don't delete)
                    //(don't delete) Verify your identity-------------->> will use this (don't delete) 


                    if (!_pageText.Contains("\n\nEnter verification code\n\n"))
                        BrowserDispatcher(BrowserFuncts.PressKey, 0, 13); //Press Enter key
                    else
                        BrowserDispatcher(BrowserFuncts.BrowserAct, 1, ActType.Click, AttributeType.Name,
                            "VerifyPhone");

                    BrowserDispatcher(BrowserFuncts.GetPageText, 4);

                    isWrong = _pageText.Contains("That code doesn't match the one we sent.") ||
                              _pageText.Contains("Code has numbers only. Try again.") ||
                              _pageText.Contains("Wrong code. Try again.") ||
                              _pageText.Contains("Wrong number of digits. Please try again.");
                    if (isWrong)
                    {
                        ToasterNotification.ShowError(
                            $"Wrong Verification Code. \n [ {_account.AccountBaseModel.UserName} ]");
                        CustomLog("You have entered wrong Verification code.");
                    }
                    else
                    {
                        isWrong =
                            _pageText.Contains(
                                "Too many failed attempts. The previous code sent to you is no longer valid.") ||
                            _pageText.Contains("Too many attempts. Please try again later.") ||
                            _pageText.Contains("Too many failed attempts")
                            && _pageText.Contains(
                                "Unavailable because of too many failed attempts. Try again in a few hours.");
                        if (isWrong)
                        {
                            CustomLog("Too many failed attempts on Phone Verification. Try again in a few hours.");
                            _account.AccountBaseModel.Status = AccountStatus.TooManyAttemptsOnPhoneVerification;
                        }
                    }
                }

                break;
            } while (true);

            if (isWrong && _account.AccountBaseModel.Status != AccountStatus.TooManyAttemptsOnPhoneVerification)
                _account.AccountBaseModel.Status = AccountStatus.PhoneVerification;
            else
                _account.AccountBaseModel.Status = AccountStatus.TryingToLogin;

            VerifyingAccount = _account.IsVerificationCodeSent = false;

            _account.VarificationCode = "";
            return isWrong;
        }

        private bool InvalidCredentials()
        {
            _account.AccountBaseModel.Status = AccountStatus.InvalidCredentials;
            return true;
        }

        private bool SetNewPasswordNotMatched()
        {
            CustomLog("Those passwords didn't match. Try again.");
            _account.AccountBaseModel.Status = AccountStatus.SetNewPassword;
            return true;
        }

        private bool SetNewPasswordCreateStrongPassword()
        {
            CustomLog("Use a mix of letters, numbers and symbols to create a stronger password");
            _account.AccountBaseModel.Status = AccountStatus.SetNewPassword;
            return true;
        }

        private bool SetNewPasswordAfterSuspiciousActivity()
        {
            _account.AccountBaseModel.Status = AccountStatus.SetNewPassword;
            return true;
        }

        private bool ManyAttemptsOnPhoneVerification()
        {
            CustomLog("Too many failed attempts on Phone Verification. Try again in a few hours.");
            _account.AccountBaseModel.Status = AccountStatus.TooManyAttemptsOnPhoneVerification;
            return true;
        }

        private bool BrowserNotSecure()
        {
            CustomLog("doesn't allow us to keep your account secure.");
            _account.AccountBaseModel.Status = AccountStatus.BrowerNotSecure;
            return true;
        }

        private bool AccountDisabled()
        {
            CustomLog("Submit a request to restore your Google Account.");
            _account.AccountBaseModel.Status = AccountStatus.AccountDisabled;
            return true;
        }

        private bool ClickOptionConfirmRecovery()
        {
            // BrowserDispatcher(BrowserFuncts.BrowserAct, 1, ActType.Click, AttributeType.ClassName, "vdE7Oc");
            BrowserDispatcher(BrowserFuncts.PressKey, 0, 9, 4, 200); //Press Tab 4 Times key
            BrowserDispatcher(BrowserFuncts.PressKey, 0, 13); //Press Enter key
            Sleep(2.5);
            return false;
        }

        private bool _cameHereByClickingOption;

        private bool ClickOptionConfirmRecoveryClickIndex2()
        {
            BrowserDispatcher(BrowserFuncts.BrowserAct, 1, ActType.Click, AttributeType.ClassName, "vdE7Oc", "", 2);
            Sleep(2.5);
            _cameHereByClickingOption = true;
            return false;
        }

        private bool ConfirmRecoveryEmailAddress()
        {
            var loginFailed = RetypeEmail();
            var gotEmailFromPage = Utilities.GetBetween(_pageText, "your account:", "\n").Trim();
            if (string.IsNullOrEmpty(_account.AccountBaseModel.AlternateEmail.Trim()) &&
                !string.IsNullOrEmpty(gotEmailFromPage) ||
                !IsExistingEmailOrNumberSame(_account.AccountBaseModel.AlternateEmail.Trim(), gotEmailFromPage))
                _account.AccountBaseModel.AlternateEmail = gotEmailFromPage;
            return loginFailed;
        }

        private bool ConfirmRecoveryPhoneNumber()
        {
            var loginFailed = RetypePhoneNumber();
            var gotNumberFromPage = Utilities.GetBetween(_pageText, "security settings:", "\n").Replace(" ", "")
                .Replace("(", "").Replace(")", "").Replace("-", "").Replace("_", "").Trim();
            if (string.IsNullOrEmpty(_account.AccountBaseModel.PhoneNumber.Trim()) &&
                !string.IsNullOrEmpty(gotNumberFromPage) ||
                !IsExistingEmailOrNumberSame(_account.AccountBaseModel.PhoneNumber.Trim(), gotNumberFromPage))
                _account.AccountBaseModel.PhoneNumber = gotNumberFromPage;
            return loginFailed;
        }

        private bool AddPhoneNumber()
        {
            if (string.IsNullOrEmpty(_account.AccountBaseModel.PhoneNumber)
                || _account.AccountBaseModel.PhoneNumber.Contains("•"))
            {
                _account.AccountBaseModel.Status = AccountStatus.AddPhoneNumberToYourAccount;
                _account.IsVerificationCodeSent = false;
                return true;
            }

            if (!_account.IsVerificationCodeSent)
            {
                _account.AccountBaseModel.Status = AccountStatus.PhoneVerification;
                return true;
            }

            var isWrong = true;
            if (!(_account.AccountBaseModel.Status == AccountStatus.TooManyAttemptsOnPhoneVerification
                  || _account.AccountBaseModel.Status == AccountStatus.AddPhoneNumberToYourAccount))
            {
                _account.AccountBaseModel.Status = AccountStatus.TryingToLogin;
                BrowserDispatcher(BrowserFuncts.GetPageText);
                BrowserDispatcher(BrowserFuncts.EnterChars, 0, _account.AccountBaseModel.PhoneNumber);
                if (_pageText.Contains(
                        "Provide a phone number to continue. We'll send a verification code that you can use to sign in.")
                    || _pageText.Contains(
                        "Provide a phone number to continue. We'll send a verification code you can use to sign in."))
                {
                    BrowserDispatcher(BrowserFuncts.BrowserAct, 1, ActType.Click, AttributeType.Name, "SendCode");
                }
                else if (_pageText.Contains("Enter a phone number to get a text message with a verification code"))
                {
                    BrowserDispatcher(BrowserFuncts.BrowserAct, 1, ActType.Click, AttributeType.Id,
                        "identifierNext"); //Press Next Key
                    // BrowserDispatcher(BrowserFuncts.PressKey, 1, ActType.Click, AttributeType.ClassName, "Vwe4Vb MbhUzd"); //Press Next Key option2
                    if (_pageText.Contains(
                            "Provide a phone number to continue. We'll send a verification code that you can use to sign in.")
                        || _pageText.Contains(
                            "Provide a phone number to continue. We'll send a verification code you can use to sign in.")
                    )
                        BrowserDispatcher(BrowserFuncts.BrowserAct, 1, ActType.Click, AttributeType.Name, "SendCode");
                }

                else
                {
                    BrowserDispatcher(BrowserFuncts.PressKey, 0,
                        13); //Press Enter key // BrowserAct(ActType.ClickByClass, "RveJvd snByac", delayAfter: 3);
                }

                BrowserDispatcher(BrowserFuncts.BrowserAct, 1, ActType.Click, AttributeType.ClassName,
                    "Vwe4Vb MbhUzd"); //Press Next Key
                BrowserDispatcher(BrowserFuncts.GetPageText, 4);

                isWrong = !(_pageText.Contains("A text message with a 6-digit verification code was just sent to") ||
                            _pageText.Contains("\n\nEnter verification code\n\n"));

                if (!isWrong)
                {
                    var number = Utilities.GetBetween(_pageText, "code was just sent to", "\n");
                    if (string.IsNullOrEmpty(number))
                        number = "the number you submitted";
                    CustomLog(
                        $"A text message with a 6-digit verification code was just sent to {number}. Enter the code within 2 minutes.");
                }

                else
                {
                    isWrong = _pageText.Contains("The phone number was invalid. Please correct it and try again.")
                              || _pageText.Contains("There was a problem with your phone number")
                              || _pageText.Contains(
                                  "Sorry, Google didn't recognise the number that you have entered. Please check the country and number.") ||
                              _pageText.Contains(
                                  "Sorry, Google didn't recognize the number that you have entered. Please check the country and number.") ||
                              _pageText.Contains(
                                  "This phone number has already been used too many times for verification.")
                              || _pageText.Contains(
                                  "Sorry, Google didn't recognise the number that you have entered. Please check the country and number.");

                    if (isWrong && _pageText.Contains("There was a problem with your phone number"))
                    {
                        CustomLog(
                            $"There was a problem with your phone number({_account.AccountBaseModel.PhoneNumber}). Please use another phone number");
                        _account.AccountBaseModel.Status = AccountStatus.AddPhoneNumberToYourAccount;
                    }
                    else if (isWrong && _pageText.Contains(
                                 "This phone number has already been used too many times for verification."))
                    {
                        CustomLog(
                            $"This phone number({_account.AccountBaseModel.PhoneNumber}) has already been used too many times for verification. Please use another phone number");
                        _account.AccountBaseModel.PhoneNumber = "";
                        _account.AccountBaseModel.Status = AccountStatus.AddPhoneNumberToYourAccount;
                    }
                    else if (isWrong)
                    {
                        CustomLog(
                            $"Google didn't recognize the number that you have entered. Please check the country code and number({_account.AccountBaseModel.PhoneNumber}).Please re-enter the phone number and try again later");
                        _account.AccountBaseModel.Status = AccountStatus.AddPhoneNumberToYourAccount;
                    }
                }
            }

            if (isWrong && _account.AccountBaseModel.Status != AccountStatus.TooManyAttemptsOnPhoneVerification)
            {
                VerifyingAccount = false;
                _account.AccountBaseModel.Status = AccountStatus.AddPhoneNumberToYourAccount;
            }

            _account.IsVerificationCodeSent = isWrong;
            return isWrong;
        }

        private bool NeedPhoneVerification()
        {
            _account.AccountBaseModel.Status = AccountStatus.PhoneVerification;

            if (VerifyingAccount && _pageText.Contains("Text") && _pageText.Contains("Call"))
            {
                BrowserDispatcher(BrowserFuncts.PressKey, 0, 9, 3, 200); //Press Tab 3 Times key
                BrowserDispatcher(BrowserFuncts.PressKey, 0,
                    13); //Press Enter key // BrowserAct(ActType.ClickByClass, "RveJvd snByac", delayAfter: 3);
                BrowserDispatcher(BrowserFuncts.GetPageText, 4);

                var isWrong =
                    !(_pageText.Contains("A text message with a 6-digit verification code was just sent to") ||
                      _pageText.Contains("\n\nEnter verification code\n\n"));

                if (!isWrong)
                {
                    var number = Utilities.GetBetween(_pageText, "code was just sent to", "\n");
                    if (string.IsNullOrEmpty(number))
                        number = "the number you submitted";
                    CustomLog(
                        $"A text message with a 6-digit verification code was just sent to {number}. Enter the code within 2 minutes.");
                }

                return false;
            }

            if (VerifyingAccount &&
                _pageText.Contains("Google will send a notification to your phone to verify that it's you"))
            {
                BrowserDispatcher(BrowserFuncts.PressKey, 0, 9, 4, 200); //Press Tab 4 Times key
                BrowserDispatcher(BrowserFuncts.PressKey, 0,
                    13); //Press Enter key // BrowserAct(ActType.ClickByClass, "RveJvd snByac", delayAfter: 3);
                BrowserDispatcher(BrowserFuncts.GetPageText, 4);

                var isWrong =
                    !_pageText.Contains(
                        "Google sent a notification to your phone. Tap Yes on the notification, then tap");

                if (!isWrong)
                {
                    var number = Utilities.GetBetween(_pageText, "Check your phone\n", "\n");

                    CustomLog($"Check your phone. {number} . Perform it within 2 minutes.");
                }

                return false;
            }

            return true;
        }

        private bool NeedEmailVerification()
        {
            _account.AccountBaseModel.Status = AccountStatus.EmailVerification;
            return true;
        }

        private bool NeedsVerification()
        {
            if (_pageText.Contains("Type the text you hear or see") &&
                _account.AccountBaseModel.Status != AccountStatus.NeedsVerification)
                CustomLog("Captcha Found (Reason: Might be same IP is getting hit so fast with google login)");
            _account.AccountBaseModel.Status = AccountStatus.NeedsVerification;
            return true;
        }

        private bool FailedGotUnknownError()
        {
            _account.AccountBaseModel.Status = AccountStatus.Failed;
            return true;
        }

        private bool TooManyAttemptsOnSignIn()
        {
            _account.AccountBaseModel.Status = AccountStatus.TooManyAttemptsOnSignIn;
            return true;
        }

        private bool IsExistingEmailOrNumberSame(string enteredNumber, string alreadyExistedNumberInAccount)
        {
            try
            {
                var reverseAlreadyExistedNumberInAccount = alreadyExistedNumberInAccount.ToLower().Reverse().ToList();

                var indexIterate = 0;
                foreach (var eachNumb in enteredNumber.ToLower().Reverse())
                {
                    var selectedDigit = reverseAlreadyExistedNumberInAccount[indexIterate];
                    if (selectedDigit != '•' && selectedDigit != eachNumb)
                        return false;
                    ++indexIterate;
                }

                return true;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return true;
            }
        }

        private async Task<bool> ClickOnSignInIfYoutubeNotLoggedIn()
        {
            CurrentData = BrowserWindow.CurrentUrl();
            if (CurrentData.ToLower().Contains("youtube.com"))
                if (_html.ToLower().Contains("href=\"https://www.youtube.com/opensearch") &&
                    !(_html.ToLower().Contains(_account.UserName.ToLower()) || _html.Contains("\"logged_in\",\"value\":\"1\"") || _html.Contains("\"LOGGED_IN\":true")))
                {
                    lastCustomXandY = await BrowserWindow.GetXAndYAsync(customScriptX: $"{string.Format(YdStatic.ByTagAttributeAndValueButtonScript, "yt-button-shape>a", "aria-label", "Sign in", "href", "https://accounts.google.com/servicelogin?service=youtube")}[0].getBoundingClientRect().x", customScriptY: $"{string.Format(YdStatic.ByTagAttributeAndValueButtonScript, "yt-button-shape>a", "aria-label", "Sign in", "href", "https://accounts.google.com/servicelogin?service=youtube")}[0].getBoundingClientRect().y");

                    if (lastCustomXandY.Key != 0 && lastCustomXandY.Value != 0)
                        await BrowserWindow.MouseClickAsync(lastCustomXandY.Key + 35, lastCustomXandY.Value + 15, delayAfter: 10);
                    return true;
                }
            return false;
        }

        private async Task<bool> CheckGoogleLogin()
        {
            _html = await BrowserWindow.GetPageSourceAsync();
            if (await ClickOnSignInIfYoutubeNotLoggedIn())
                return false;
            await CreateChannelOnYoutube();
            VerifyingAccount = _account.IsVerificationCodeSent = false;
            return true;
        }

        private async Task CreateChannelOnYoutube()
        {
            try
            {
                if (!(_account.DisplayColumnValue1 == null || _account.DisplayColumnValue1 == 0)) return;
                await BrowserWindow.GoToCustomUrl("https://www.youtube.com/create_channel", delayAfter: 6);
                await LoadSource();
                for (var i = 0; i < 10; i++)
                {
                    _html = await BrowserWindow.GetPageSourceAsync();
                    if (_html.Contains("aria-label=\"Create channel") &&
                        _html.Contains("id=\"create-channel-button"))
                    {
                        lastCustomXandY = await BrowserWindow.GetXAndYAsync(customScriptX: $"{string.Format(YdStatic.ByTagAttributeAndValueButtonScript, "yt-button-shape>button", "aria-label", "Create channel", "textContent", "create channel")}[0].getBoundingClientRect().x", customScriptY: $"{string.Format(YdStatic.ByTagAttributeAndValueButtonScript, "yt-button-shape>button", "aria-label", "Create channel", "textContent", "create channel")}[0].getBoundingClientRect().y");

                        if (lastCustomXandY.Key != 0 && lastCustomXandY.Value != 0)
                            await BrowserWindow.MouseClickAsync(lastCustomXandY.Key + 35, lastCustomXandY.Value + 15, delayAfter: 10);
                        else
                        {
                            await BrowserWindow.PressAnyKeyUpdated(9, 8, delayAtLast: 2);
                            await BrowserWindow.PressAnyKeyUpdated(13, 1, delayAtLast: 10);
                        }
                        _account.DisplayColumnValue1 = 1;
                        break;
                    }

                    if (_html.Contains("aria-label=\"Customize channel\"") || _html.Contains("aria-label=\"Customise channel\""))
                    {
                        _account.DisplayColumnValue1 = 1;
                        break;
                    }

                }
                await BrowserWindow.GoToCustomUrl(HomeUrl, delayAfter: 6);
                await LoadSource();
            }
            catch
            {
                /*ignored*/
            }
        }

        public bool SetVideoQuality { get; set; }

        private void SetVideoQualityAs144P()
        {
            BrowserDispatcher(BrowserFuncts.GetCurrentUrl);
            if (SetVideoQuality ||
                !Uri.UnescapeDataString(CurrentData.ToLower()).Contains("www.youtube.com/watch?")) return;

            BrowserDispatcher(BrowserFuncts.BrowserAct, 4, ActType.Click, AttributeType.ClassName,
                "ytp-volume-slider"); // To Open Volume Slider

            BrowserDispatcher(BrowserFuncts.PressKey, 0, 40, 21, 100); //Press Down Arrow key 40 times to mute the music

            new Task(() =>
            {
                while (!_isDisposed)
                {
                    BrowserDispatcher(BrowserFuncts.IsDisposed);
                    if (!_isDisposed)
                        BrowserDispatcher(BrowserFuncts.CheckAd, 3);
                    //if (FoundAd) { /*Just for Checking*/ }
                    if (!_isDisposed && SkipYoutubeAd && FoundAd)
                        BrowserDispatcher(BrowserFuncts.BrowserAct, 0, ActType.Click, AttributeType.ClassName,
                            "ytp-ad-skip-button-icon"); // for Skipping add
                }
            }).Start();

            Sleep(3.5);

            if (FoundAd)
                while (FoundAd)
                    Sleep(3.5);
            else
                PlayPauseVideo();

            VideoPlayTime = new Stopwatch();
            VideoPlayTime.Start();

            #region decreasing video to minimum  quality 

            BrowserDispatcher(BrowserFuncts.BrowserAct, 1, ActType.Click, AttributeType.ClassName,
                "ytp-button ytp-settings-button"); // Click Youtube MediaPlayer Setting Button
            BrowserDispatcher(BrowserFuncts.PressKey, 1.5, 40, 4, 100); //Press Down Arrow key 4 times
            BrowserDispatcher(BrowserFuncts.PressKey, 0.5, 39); //Press right Arrow key 1 time
            BrowserDispatcher(BrowserFuncts.PressKey, 0.5, 40, 6, 100); //Press Down Arrow key 6 times
            BrowserDispatcher(BrowserFuncts.PressKey, 0.5, 38); //Press up Arrow key 1 time
            BrowserDispatcher(BrowserFuncts.PressKey, 0.5, 13); //Press Enter key

            SetVideoQuality = true;
            PlayPauseVideo(true);
            #endregion
        }

        #endregion

        public void SwitchChannel(string channelPageId, double delayBeforeHit = 0, bool SwitchChannelForPublisher = false)
        {
            try
            {
                var channelName = SwitchChannelForPublisher ? channelPageId :
                    !string.IsNullOrEmpty(channelPageId) && channelPageId.Contains(YdStatic.DefaultChannel) ? Utilities.GetBetween(channelPageId, "[", "]")
                    : !string.IsNullOrWhiteSpace(channelPageId) ? channelPageId?.Split('[')?.FirstOrDefault()?.Trim() : channelPageId;

                if (!string.IsNullOrEmpty(channelName) && _account.AccountBaseModel.ProfileId != channelName)
                {
                    SwitchToSelectedChannel(channelName, delayBeforeHit);
                    var homeResponse = new YoutubeHomePageHandler(CurrentData);
                    _account.AccountBaseModel.Status = AccountStatus.Success;
                    _account.AccountBaseModel.UserId = homeResponse.ChannelId;
                    _account.AccountBaseModel.ProfileId = homeResponse.ChannelUsername;
                    BrowserDispatcher(BrowserFuncts.GetBrowserCookies);
                    SocinatorAccountBuilder.Instance(_account.AccountBaseModel.AccountId)
                        .AddOrUpdateDominatorAccountBase(_account.AccountBaseModel)
                        .AddOrUpdateCookies(_account.Cookies)
                        .SaveToBinFile();

                    _cancellationToken.ThrowIfCancellationRequested();
                }

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void SwitchToSelectedChannel(string channelName, double delayBeforeHit)
        {
            BrowserDispatcher(BrowserFuncts.GoToUrl, delayBeforeHit,
                    "https://www.youtube.com/channel_switcher?next=%2Faccount&feature=settings");
            ClickOrScrollToViewAndClick($"{string.Format(YdStatic.ByTagAttributeAndValueButtonScript, "ytd-account-item-renderer>tp-yt-paper-icon-item", "role", "option", "textContent", channelName.ToLower())}", timetoWait: 6, isLoadSource: true).Wait(_cancellationToken);
            BrowserDispatcher(BrowserFuncts.GoToUrl, delayBeforeHit,
                   "https://www.youtube.com");
        }
        public void ExtractChannelPageId(ref string pageIdContent)
        {
            pageIdContent =
                string.IsNullOrEmpty(pageIdContent) || pageIdContent.Contains($" [{YdStatic.DefaultChannel}]")
                    ? ""
                    : pageIdContent;

            if (string.IsNullOrEmpty(pageIdContent)) return;

            var reg = Regex
                .Matches(pageIdContent, @"\d{19,21}");
            if (reg.Count > 0)
                pageIdContent = $"&pageid={reg[0]}";
        }



        private Stopwatch VideoPlayTime;

        public PostInfoYdResponseHandler PostInfo(ActivityType activityType, string url, bool hasVideoDuration = false,
            double delayBeforeHit = 0, bool sortCommentsByNew = false, bool needChannelDetails = false, bool pauseVideo = true)
        {
            try
            {
                var postInfoHandler = new PostInfoYdResponseHandler(Response);
                if (needChannelDetails)
                {
                    //For getting channel views count we are visiting that channel page and collecting data.
                    BrowserDispatcher(BrowserFuncts.GoToUrl, 0.5, HomeUrl.ToString() + "channel/" + postInfoHandler.YoutubePost.ChannelId.ToString() + "/about");
                    postInfoHandler.YoutubePost.ChannelViewsCount = YoutubeUtilities.YoutubeElementsCountInNumber(Utilities.GetBetween(CurrentData, "viewCountText\":{\"simpleText\":\"", "\""));
                    BrowserDispatcher(BrowserFuncts.GoBack);
                    BrowserDispatcher(BrowserFuncts.GetPageSource, 2);
                }
                if (!hasVideoDuration && postInfoHandler.YoutubePost.VideoLength == 0)
                {
                    // get the video length in searching video page
                    postInfoHandler.YoutubePost.VideoLength =
                        VideoDurationForCustomUrl(postInfoHandler.YoutubePost.Code, 1.5);
                    // Now go back to the privious page (Video page)
                    BrowserDispatcher(BrowserFuncts.GoBack);
                }
                if (postInfoHandler.YoutubePost.CommentCount == null || string.IsNullOrEmpty(postInfoHandler.YoutubePost.CommentCount))
                {
                    BrowserDispatcher(BrowserFuncts.GetElementValue, 0.1, ActType.GetValue, AttributeType.ClassName,
                            "count-text style-scope ytd-comments-header-renderer", ValueTypes.InnerText);
                    if (!string.IsNullOrEmpty(CurrentData))
                        postInfoHandler.YoutubePost.CommentCount = YoutubeUtilities.YoutubeElementsCountInNumber(CurrentData);

                    BrowserDispatcher(BrowserFuncts.GetElementValue, 0.1, ActType.GetValue, AttributeType.ClassName,
                            "style-scope ytd-comments", ValueTypes.InnerHtml, 1);
                    if (!string.IsNullOrEmpty(CurrentData))
                        postInfoHandler.YoutubePost.CommentEnabled = !CurrentData.Contains("https://support.google.com/youtube/answer/9706180");
                }
                if (postInfoHandler.YoutubePost.CommentEnabled && sortCommentsByNew)
                {
                    BrowserDispatcher(BrowserFuncts.MouseScroll, 1, ScreenResolution.Key / 2, ScreenResolution.Key / 2, 0, -500, 0, 8);
                    int index = 5;
                    postInfoHandler.YoutubePost.ListOfCommentsInfo = ScrapePostCommentsDetails(ActivityType.LikeComment, "", ref index, 3, 20);
                }
                return postInfoHandler;
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return new PostInfoYdResponseHandler();
            }
        }

        public int VideoDurationForCustomUrl(string videoId, double delayBeforeHit = 0)
        {
            try
            {
                var videoDuration = string.Empty;

                var url = "https://www.youtube.com/results?search_query=" + Uri.EscapeDataString(videoId);
                // Hit the searching video page url
                BrowserDispatcher(BrowserFuncts.GoToUrl, delayBeforeHit, url);

                var response = Response.Response;
                var jsonData = Utilities.GetBetween(response, "window[\"ytInitialData\"] = ",
                    ";\n    window[\"ytInitialPlayerResponse\"]");
                if (string.IsNullOrEmpty(jsonData))
                    jsonData = Utilities
                        .GetBetween(response, "window[\"ytInitialData\"] = ", "window[\"ytInitialPlayerResponse\"]")
                        .Trim().TrimEnd(';');
                var jsonHand = new JsonHandler(jsonData);

                var videoContents = jsonHand.GetJToken("contents", "twoColumnSearchResultsRenderer", "primaryContents",
                    "sectionListRenderer", "contents", 0, "itemSectionRenderer", "contents");

                foreach (var token in videoContents)
                    if (jsonHand.GetJTokenValue(token, "videoRenderer", "videoId") == videoId)
                    {
                        videoDuration = jsonHand.GetJTokenValue(token, "videoRenderer", "lengthText", "simpleText");
                        break;
                    }

                var seconds = TimeSpan.Parse(videoDuration.Length < 6 ? $"00:{videoDuration}" : videoDuration)
                    .TotalSeconds;

                return Convert.ToInt32(seconds);
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return 0;
        }

        public async Task<bool> ViewIncreaserVideo(DominatorAccountModel acountModel, YoutubePost youtubePost,
            int delay,
            string channelPageIdSource, bool mozillaSelected, bool browserHidden, bool skipAd,
            double delayBeforeHit = 0)
        {

            if (string.IsNullOrEmpty(youtubePost?.PostUrl))
                return false;
            var viewed = false;
            try
            {
                SkipYoutubeAd = skipAd;
                lock (YdStatic.LockViewIncreaser)
                {
                    if (YdStatic.BrowserOpeningViewIncreaser++ > _youtubeModel.LimitNumberOfSimultaneousWatchVideos - 1)
                        Monitor.Wait(YdStatic.LockViewIncreaser);
                }

                _cancellationToken.ThrowIfCancellationRequested();

                GlobusLogHelper.log.Info(Log.StartedActivity, acountModel.AccountBaseModel.AccountNetwork,
                    acountModel.AccountBaseModel.UserName, ActivityType.ViewIncreaser,
                    $"{youtubePost.PostUrl} (Viewing times -> {youtubePost.TotalWatchingCount + 1})");

                await Task.Factory.StartNew(() => viewed = ViewIncreaseWithBrowser(acountModel, youtubePost.PostUrl,
                    delay, mozillaSelected, browserHidden, youtubePost.Title));

                if (_cancellationToken.IsCancellationRequested)
                    return false;
            }
            catch (OperationCanceledException)
            {
                lock (YdStatic.LockViewIncreaser)
                {
                    YdStatic.BrowserOpeningViewIncreaser--;
                    Monitor.Pulse(YdStatic.LockViewIncreaser);
                }

                return false;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return false;
            }

            return viewed;
        }

        public void PlayPauseVideo(bool Play = false)
        {
            try
            {
                if (_account.IsRunProcessThroughBrowser)
                {
                    BrowserDispatcher(BrowserFuncts.GetPageSource);
                    var PlayButtonClass = "ytp-play-button ytp-button";
                    // Pause the video for view Increaser
                    var Node = HtmlParseUtility.GetListNodesFromClassName(CurrentData, PlayButtonClass);
                    if (Play && Node != null && Node.Count > 0 && !string.IsNullOrEmpty(Node.FirstOrDefault().OuterHtml) && Node.FirstOrDefault().OuterHtml.Contains("data-title-no-tooltip=\"Play\""))
                        BrowserDispatcher(BrowserFuncts.BrowserAct, 0.5, ActType.Click, AttributeType.ClassName,
                            PlayButtonClass);
                    else if (Node != null && Node.Count > 0 && !string.IsNullOrEmpty(Node.FirstOrDefault().OuterHtml) && !Node.FirstOrDefault().OuterHtml.Contains("data-title-no-tooltip=\"Play\""))
                        BrowserDispatcher(BrowserFuncts.BrowserAct, 0.5, ActType.Click, AttributeType.ClassName,
                            PlayButtonClass);
                }
                else
                    // get mouse to click over the middle of the video player pixel location
                    BrowserDispatcher(BrowserFuncts.MouseClick, 0, 540, 400);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private bool ViewIncreaseWithBrowser(DominatorAccountModel accountModel, string youtubeVideoUrl, int delay,
            bool mozillaSelected, bool hiddenBrowser, string videoTitle)
        {
            var goWithMozilla = false;
            try
            {
                var isPrivateProxy =
                    !string.IsNullOrEmpty(accountModel.AccountBaseModel.AccountProxy?.ProxyIp?.Trim() ?? "") &&
                    !string.IsNullOrEmpty(accountModel.AccountBaseModel.AccountProxy?.ProxyUsername?.Trim() ?? "");
                goWithMozilla = mozillaSelected
                                && !isPrivateProxy
                                && File.Exists($"{ConstantVariable.GetPlatformBaseDirectory()}/geckodriver.exe");


                //if (goWithMozilla)
                //    return new MozillaBrowser(accountModel, _cancellationToken, hiddenBrowser).StartWatching(
                //        youtubeVideoUrl, delay, videoTitle, SkipYoutubeAd);
                //if (mozillaSelected && !goWithMozilla)
                //    GlobusLogHelper.log.Info(Log.CustomMessage, accountModel.AccountBaseModel.AccountNetwork,
                //        accountModel.AccountBaseModel.UserName, ActivityType.ViewIncreaser,
                //        "Driver to run mozilla browser is Not  SuccessFull So Trying to Play in Embedded Browser");

                var openCef = new Task(() =>
                {
                    try
                    {
                        LoadPageSource(accountModel, youtubeVideoUrl, clearandNeedResource: true);

                        SetVideoQualityAs144P();

                        BrowserDispatcher(BrowserFuncts.GetCurrentUrl);
                        var currentUrl = CurrentData;
                        var increaseCount = VideoPlayTime == null ? 0 : VideoPlayTime.ElapsedMilliseconds / 1000;

                        while (delay > ++increaseCount && currentUrl == CurrentData)
                        {
                            _cancellationToken.ThrowIfCancellationRequested();
                            while (FoundAd)
                            {
                                _cancellationToken.ThrowIfCancellationRequested();
                                Sleep();
                            }

                            Sleep(0.99);
                            BrowserDispatcher(BrowserFuncts.GetCurrentUrl);
                        }

                        PlayPauseVideo();
                    }
                    catch (OperationCanceledException)
                    {
                        /*Just Catch It*/
                    }
                    catch
                    {
                    }

                    if (VideoPlayTime != null)
                    {
                        VideoPlayTime.Stop();
                        VideoPlayTime = null;
                    }
                });
                openCef.Start();
                openCef.Wait(_cancellationToken);

                return true;
            }
            catch (OperationCanceledException)
            {
                return false;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return false;
            }
            finally
            {
                try
                {
                    lock (YdStatic.LockViewIncreaser)
                    {
                        YdStatic.BrowserOpeningViewIncreaser--;
                        Monitor.Pulse(YdStatic.LockViewIncreaser);
                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }
        }


        public ChannelInfoResponseHandler GetChannelDetails(string channelUrl, double delayBeforeHit = 0)
        {
            try
            {
                if (!channelUrl.EndsWith("about"))
                    channelUrl = $"{channelUrl}/about";
                LoadPageSource(_account, channelUrl);
                ClickOrScrollToViewAndClick($"{string.Format(YdStatic.ByTagAttributeAndValueButtonScript, "button", "aria-label", "Close", "className", "yt-spec-button-shape-next--icon-only-default")}").Wait(_cancellationToken);
                var IsSusbscribed = BrowserWindow.ExecuteScript("document.querySelector('div[class=\"ytSubscribeButtonViewModelContainer\"]>button').innerText", delayInSec: 2);
                var channelInfo =  new ChannelInfoResponseHandler(Response, true, false);
                if(IsSusbscribed != null && IsSusbscribed.Result != null)
                    channelInfo.YoutubeChannel.IsSubscribed = IsSusbscribed?.Result?.ToString()?.ToLower() == "subscribed";
                return channelInfo;
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return new ChannelInfoResponseHandler();
            }
        }

        public LikeDislikeResponseHandler LikeDislikeVideo(ActivityType activityType, YoutubePost youtubePost,
            double delayBeforeHit = 0)
        {
            if (BrowserWindow.CurrentUrl() != youtubePost.PostUrl)
                LoadPageSource(_account, youtubePost.PostUrl);
            var success = false;
            if ((activityType == ActivityType.Like && youtubePost.LikeEnabled &&
                youtubePost.Reaction.ToString() != "Like") || (activityType == ActivityType.Dislike &&
                youtubePost.DislikeEnabled && youtubePost.Reaction.ToString() != "Dislike"))
            {
                var buttonType = activityType == ActivityType.Like ? "like" : "dislike";

                var likedOrDislikedResponse = BrowserWindow.ExecuteScript($"document.querySelectorAll('{buttonType}-button-view-model>toggle-button-view-model>button-view-model>button')[0].ariaPressed", 2);

                if (bool.TryParse(likedOrDislikedResponse.Result.ToString(), out bool IsLikedOrDisliked) && !IsLikedOrDisliked)
                {
                    success = ClickOrScrollToViewAndClick($"document.querySelectorAll('{buttonType}-button-view-model>toggle-button-view-model>button-view-model>button')").Result;
                }
                else
                    success = IsLikedOrDisliked;
            }

            return new LikeDislikeResponseHandler() { Success = success };
        }

        #region LikeOrDislke Post After Comment

        public LikeDislikeResponseHandler LikeDislikeVideoAfterComment(ActivityType activityType,
            YoutubePost youtubePost, double delayBeforeHit = 0)
        {
            BrowserDispatcher(BrowserFuncts.GoToUrl, 0, youtubePost.PostUrl);
            if (activityType == ActivityType.Like && youtubePost.LikeEnabled &&
                youtubePost.Reaction.ToString() != "Like" || activityType == ActivityType.Dislike &&
                youtubePost.DislikeEnabled && youtubePost.Reaction.ToString() != "Dislike")
            {
                var clickIndex = activityType == ActivityType.Like ? 0 : 2;
                if (activityType == ActivityType.Dislike && youtubePost.Reaction.ToString() == "Like")
                    clickIndex = 0;

                // Hit like button
                BrowserDispatcher(BrowserFuncts.BrowserAct, delayBeforeHit, ActType.Click, AttributeType.ClassName,
                    "style-scope ytd-toggle-button-renderer style-text", "", clickIndex);

                #region By Mouse click over specific location

                //// get like button pixel position
                //BrowserDispatcher(BrowserFuncts.GetXY, 2, AttributeType.ClassName, "style-scope ytd-toggle-button-renderer style-text", clickIndex);
                //if (lastXandY.Key==0)
                //    BrowserDispatcher(BrowserFuncts.GetXY, 0, AttributeType.ClassName, "yt-simple-endpoint style-scope ytd-toggle-button-renderer", clickIndex+1);
                //// get mouse to click over the like button pixel location
                //BrowserDispatcher(BrowserFuncts.MouseClick, 0.1, lastXandY.Key, lastXandY.Value); 

                #endregion

                // check like button is hit 
                BrowserDispatcher(BrowserFuncts.GetElementValue, 1, ActType.GetAttribute, AttributeType.ClassName,
                    "style-scope ytd-toggle-button-renderer style-default-active", ValueTypes.AriaPressed, 0);

                if (CurrentData == "true")
                    return new LikeDislikeResponseHandler { Success = true };
                return new LikeDislikeResponseHandler { Success = true }; //need to make changes here 
            }

            return new LikeDislikeResponseHandler();
        }

        #endregion


        public SubscribeResponseHandler SubscribeChannel(DominatorAccountModel accountModel, YoutubeChannel youtubeChannel, double delayAfter = 0)
        {
            bool isRunning = true;
            bool isSubscribed = youtubeChannel.IsSubscribed ? true : false;
            if (!isSubscribed)
            {
                if (!BrowserWindow.CurrentUrl().Contains(youtubeChannel.ChannelUrl))
                    LoadPageSource(accountModel, youtubeChannel.ChannelUrl);
                Application.Current.Dispatcher.Invoke(async () =>
                {
                    isSubscribed = await ClickOrScrollToViewAndClick($"document.querySelectorAll('button[aria-label*=\"Subscribe to {youtubeChannel.ChannelName}\"],button[class=\"yt-spec-button-shape-next yt-spec-button-shape-next--filled yt-spec-button-shape-next--mono yt-spec-button-shape-next--size-m\"]')", timetoWait: 6);

                    isRunning = false;
                });
                while (isRunning)
                    Task.Delay(1500).Wait(_cancellationToken);
            }
            return new SubscribeResponseHandler(isSubscribed);
        }

        public UnsubscribeResponseHandler UnsubscribeChannel(DominatorAccountModel accountModel, YoutubeChannel youtubeChannel, double delayAfter = 0)
        {
            bool isRunning = true;
            bool isUnSubscribed = youtubeChannel.IsSubscribed ? false : true;
            if (!isUnSubscribed)
            {
                if (!BrowserWindow.CurrentUrl().Contains(youtubeChannel.ChannelUrl))
                    LoadPageSource(accountModel, youtubeChannel.ChannelUrl);
                Application.Current.Dispatcher.Invoke(async () =>
                {
                    isUnSubscribed = await ClickOrScrollToViewAndClick($"document.querySelectorAll('button[aria-label*=\"Tap to change your notification setting for {youtubeChannel.ChannelName}\"],button[class=\"yt-spec-button-shape-next yt-spec-button-shape-next--tonal yt-spec-button-shape-next--mono yt-spec-button-shape-next--size-m yt-spec-button-shape-next--icon-leading-trailing\"]')", timetoWait: 4, isLoadSource: true);
                    isUnSubscribed = await ClickOrScrollToViewAndClick($"[...document.querySelectorAll('tp-yt-paper-item[role=\"option\"],tp-yt-paper-item[class*=\"style-scope ytd-menu-service-item-renderer\"],yt-list-item-view-model[role=\"menuitem\"]')].filter(x=>x.textContent?.includes(\"Unsubscribe\"))", timetoWait: 4, isLoadSource: true);
                    var result = await BrowserWindow.ExecuteScriptAsync("[...document.querySelectorAll('button[aria-label=\"Unsubscribe\"],button[class=\"yt-spec-button-shape-next yt-spec-button-shape-next--text yt-spec-button-shape-next--call-to-action yt-spec-button-shape-next--size-m\"]')].filter(x=>x.innerText.includes(\"Unsubscribe\"))[0].click();", delayInSec: 3);
                    isUnSubscribed = result != null && result.Success;
                    //isUnSubscribed = await ClickOrScrollToViewAndClick($"document.querySelectorAll('button[aria-label=\"Unsubscribe\"],button[class=\"yt-spec-button-shape-next yt-spec-button-shape-next--text yt-spec-button-shape-next--call-to-action yt-spec-button-shape-next--size-m\"]')", timetoWait: 4, isLoadSource: true);
                    isRunning = false;
                });
                while (isRunning)
                    Task.Delay(1500).Wait(_cancellationToken);
            }
            return new UnsubscribeResponseHandler(isUnSubscribed);
        }

        public CommentResponseHandler CommentOnVideo(YoutubePost youtubePost, string messageText, bool isReply,
            double delayBeforeHit = 0, int firstReplyClickIndex = 0)
        {
            if (isReply)
            {

                BrowserDispatcher(BrowserFuncts.GetCurrentUrl, delayBeforeHit);

                if (!CurrentData.EndsWith($"&lc={youtubePost.InteractingCommentId}"))
                {
                    // hit the video url containing the commentId to interact
                    BrowserDispatcher(BrowserFuncts.GoToUrl, 0,
                        YdStatic.VideoUrl(youtubePost.Code, youtubePost.InteractingCommentId));
                    BrowserDispatcher(BrowserFuncts.MouseScroll, 4, ScreenResolution.Key / 2, ScreenResolution.Key / 2, 0, -500, 0, 1);
                    Sleep(5);
                }
                Sleep(2);
                //BrowserDispatcher(BrowserFuncts.Scroll, 1, 200);

                BrowserDispatcher(BrowserFuncts.MouseScroll, 4, ScreenResolution.Key / 2, ScreenResolution.Key / 2, 0, 300, 10, 1);
                Sleep(5);
                BrowserDispatcher(BrowserFuncts.BrowserAct, 0, ActType.ScrollIntoViewQuery, AttributeType.AriaLabel, "Reply", "", firstReplyClickIndex);
                BrowserDispatcher(BrowserFuncts.MouseScroll, 4, ScreenResolution.Key / 2, ScreenResolution.Key / 2, 0, 200, 0, 1);

                BrowserDispatcher(BrowserFuncts.BrowserAct, 2, ActType.ActByQuery, AttributeType.AriaLabel, "Reply", "", firstReplyClickIndex);
            }
            else
            {
                BrowserDispatcher(BrowserFuncts.MouseScroll, 0, ScreenResolution.Key / 2, ScreenResolution.Key / 2, 0, -300, 4, 1);
                ClickOrScrollToViewAndClick($"{string.Format(YdStatic.ByTagAttributeAndValueAndClassScript, "div", "id", "placeholder-area", "textContent", "Add a comment…", "style-scope ytd-comment-simplebox-renderer")}", timetoWait: 4).Wait(_cancellationToken);
            }
            // Enter the comment in comment box
            BrowserDispatcher(BrowserFuncts.CopyPasteContent, 0, messageText, 3);

            // get comment submit button pixel position

            BrowserDispatcher(BrowserFuncts.BrowserAct, delayBeforeHit, ActType.ClickById, AttributeType.Id, "submit-button");
            Sleep(3);
            /*------------------------------------*/
            //
            // get to know the owner has pinned any comment or not
            BrowserDispatcher(BrowserFuncts.GetElementValue, 3, ActType.GetValue, AttributeType.ClassName,
                "style-scope ytd-pinned-comment-badge-renderer", ValueTypes.InnerHtml);
            var myCommentHtmlIndex = string.IsNullOrWhiteSpace(CurrentData) ? 0 : 1;
            BrowserDispatcher(BrowserFuncts.GetElementValue, 2, ActType.ActByQuery, AttributeType.Id, "contents", ValueTypes.InnerHtml, myCommentHtmlIndex);
            BrowserDispatcher(BrowserFuncts.GetPageSource, 2.5);
            bool isCommentTextPresent = !string.IsNullOrEmpty(CurrentData) && !CurrentData.Contains("We weren't able to add your reply. Please try again");
            // after comment posted, check that it is existing or not and get the commentId of the commmented text


            var commentId = CommentedDoneInThePost(youtubePost.Code, myCommentHtmlIndex);
            if (string.IsNullOrWhiteSpace(commentId)) // Check one more time
                commentId = CommentedDoneInThePost(youtubePost.Code, myCommentHtmlIndex);

            return new CommentResponseHandler { Success = !string.IsNullOrWhiteSpace(commentId) && isCommentTextPresent, CommentId = commentId };
        }

        string CommentedDoneInThePost(string postId, int myCommentHtmlIndex)
        {
            Sleep(2);

            // get the first commented InnerHtml (get commented text url)
            BrowserDispatcher(BrowserFuncts.GetElementValue, 3, ActType.GetValue, AttributeType.ClassName,
                "published-time-text style-scope ytd-comment-renderer", ValueTypes.InnerHtml, myCommentHtmlIndex);

            if (CurrentData.StartsWith("\n") || string.IsNullOrEmpty(CurrentData))
            {
                BrowserDispatcher(BrowserFuncts.GetPageSourceNoCondition, 2);
                var lst = HtmlParseUtility.GetListInnerHtmlFromPartialTagName(_html, "yt-formatted-string",
                    "class", "published-time-text style-scope ytd-comment-renderer");
                lst = lst == null || lst.Count == 0 ? HtmlParseUtility.GetListInnerHtmlFromTagName(_html, "ytd-comment-view-model",
                    "class", "style-scope ytd-comment-thread-renderer") : lst;
                CurrentData = !string.IsNullOrEmpty(lst?.FirstOrDefault()) && lst.FirstOrDefault().Contains("Pinned by") ? lst[myCommentHtmlIndex + 1] : lst[myCommentHtmlIndex];
            }


            try
            {
                //var commentId = hrefCommented.StringMatches(@"(;lc=)(.*)")?[0]?.ToString().Replace(";lc=", "");
                var commentId = Utilities.GetBetween(CurrentData, ";lc=", "\"");
                return commentId;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public ReportVideoResponseHandler ReportToVideo(DominatorAccountModel dominatorAccount, YoutubePost youtubePost,
            int option, int subOption, string text, int mins = 0, int sec = 0, double delayBeforeHit = 0,string ReportText="")
        {
            try
            {
                BrowserDispatcher(BrowserFuncts.Scroll, 5, -300);
                // Click on 3-dot DropDown button to get clickable report button
                BrowserDispatcher(BrowserFuncts.BrowserAct, delayBeforeHit, ActType.ActByQuery, AttributeType.AriaLabel,
                    "More actions", "", 1); // 24
                Sleep(3);
                BrowserDispatcher(BrowserFuncts.GetListInnerHtml, 1, ActType.GetValue, AttributeType.ClassName,
                    "style-scope ytd-menu-service-item-renderer", ValueTypes.InnerText);
                ResultList.Reverse();
                int index = ResultList.IndexOf(ResultList.FirstOrDefault(x => x.Contains("Report")));
                // Now click on report button then you will get report form
                BrowserDispatcher(BrowserFuncts.BrowserAct, 1, ActType.Click, AttributeType.ClassName,
                    "style-scope ytd-menu-service-item-renderer", "", index);

                Sleep(5);
                var NextScript = $"document.querySelector('div[class=\"ytWebReportFormBottomBarViewModelBottomBarContainer\"] button').click();";
                BrowserWindow.ExecuteScript($"document.querySelectorAll('[aria-label=\"{ReportText}\"]')[0].click();", delayInSec: 5);

                BrowserWindow.ExecuteScript(NextScript, delayInSec: 5);
                if (!string.IsNullOrEmpty(text))
                {
                    var script = "document.querySelector('textarea[placeholder=\"Add details...\"]').getBoundingClientRect().{0};";
                    var XCord = BrowserWindow.ExecuteScript(string.Format(script,"x"),delayInSec:2);
                    var YCord = BrowserWindow.ExecuteScript(string.Format(script,"y"),delayInSec:2);
                    var X = GetCordinate(XCord?.Result?.ToString());
                    var Y = GetCordinate(YCord?.Result?.ToString());
                    BrowserWindow.MouseClick(X + 10, Y + 10, delayAfter: 4);
                    BrowserWindow.EnterChars(text, delayAtLast: 2);
                }
                BrowserWindow.ClearResources();
                BrowserWindow.ExecuteScript(NextScript, delayInSec: 5);

                var data = BrowserWindow.GetPaginationData("\"confirmationHeader\":\"Thanks for helping our community\"", true).Result;

                BrowserWindow.ExecuteScript(NextScript, delayInSec: 5);

                #region OLD Code base to report a video.
                // Now select any option
                //BrowserDispatcher(BrowserFuncts.BrowserAct, 3, ActType.Click, AttributeType.ClassName,
                //    "style-scope tp-yt-paper-radio-button", "", option * 4);
                //Sleep(2);

                //// Get Option value
                ////var optionValue = CurrentData;
                //if (!(option == 5 || option == 6 || option == 7))
                //{


                //    if (option > 7)
                //    {
                //        BrowserDispatcher(BrowserFuncts.Scroll, 5, -300);
                //        index = option - 3;
                //    }
                //    else
                //        index = option;

                //    Sleep(5);
                //    BrowserDispatcher(BrowserFuncts.BrowserAct, delayBeforeHit, ActType.ActByQuery, AttributeType.AriaLabel,
                //    "Choose one", "", index);

                //    Sleep(10);
                //    // Now select any sub-option
                //    BrowserDispatcher(BrowserFuncts.PressKey, 1, 40, subOption + 1,
                //        100); //Press Down Arrow key subOption+1 times
                //    BrowserDispatcher(BrowserFuncts.PressKey, 0.5, 13); //Press Enter key
                //}

                ////     Now click on next
                //BrowserDispatcher(BrowserFuncts.GetListInnerHtml, 1, ActType.GetValue, AttributeType.ClassName,
                //"yt-spec-button-shape-next yt-spec-button-shape-next--text yt-spec-button-shape-next--call-to-action yt-spec-button-shape-next--size-m ", ValueTypes.InnerText);
                //ResultList.Reverse();
                //index = ResultList.IndexOf(ResultList.FirstOrDefault(x => x.Contains("Next")));
                //BrowserDispatcher(BrowserFuncts.BrowserAct, 3, ActType.Click, AttributeType.ClassName,
                //    "yt-spec-button-shape-next yt-spec-button-shape-next--text yt-spec-button-shape-next--call-to-action yt-spec-button-shape-next--size-m ", "", index);

                //Sleep(3);


                //BrowserDispatcher(BrowserFuncts.PressKey, 0, 9, 1, 10); // Press Tab
                //if (mins > 0)
                //{
                //    BrowserDispatcher(BrowserFuncts.PressKey, 0.5, 8, 4, 150); // Press backspace key 4 times
                //    BrowserDispatcher(BrowserFuncts.EnterChars, 0, mins);
                //}

                //BrowserDispatcher(BrowserFuncts.PressKey, 0, 9, 1, 10); // Press Tab
                //if (sec > 0)
                //{
                //    BrowserDispatcher(BrowserFuncts.PressKey, 0, 8, 4, 150); // Press backspace key 2 times
                //    BrowserDispatcher(BrowserFuncts.EnterChars, 0.5, sec);
                //}

                //BrowserDispatcher(BrowserFuncts.PressKey, 0, 9, 1, 10); // Press Tab
                //if (!string.IsNullOrWhiteSpace(text))
                //    BrowserDispatcher(BrowserFuncts.EnterChars, 0.5, text);

                #endregion

                // Now click on report now
                //BrowserDispatcher(BrowserFuncts.GetListInnerHtml, 1, ActType.GetValue, AttributeType.ClassName,
                //"yt-spec-button-shape-next yt-spec-button-shape-next--text yt-spec-button-shape-next--call-to-action yt-spec-button-shape-next--size-m ", ValueTypes.InnerText);
                //ResultList.Reverse();
                //index = ResultList.IndexOf(ResultList.FirstOrDefault(x => x.Contains("Report")));
                //BrowserDispatcher(BrowserFuncts.BrowserAct, 3, ActType.Click, AttributeType.ClassName,
                //    "yt-spec-button-shape-next yt-spec-button-shape-next--text yt-spec-button-shape-next--call-to-action yt-spec-button-shape-next--size-m ", "", index);

                //Sleep(3);

                ////get report succeed message
                //BrowserDispatcher(BrowserFuncts.GetElementValue, 2.5, ActType.GetValue, AttributeType.ClassName,
                //    "style-scope yt-fancy-dismissible-dialog-renderer", ValueTypes.InnerText, 0);
                bool sucess = !string.IsNullOrEmpty(data) && data.Contains("\"confirmationHeader\":\"Thanks for helping our community\"");
                //return new ReportVideoResponseHandler
                //{ Success = sucess, SelectedOptionToVideoReport = GetReportString(option, subOption) };
                return new ReportVideoResponseHandler
                { Success = sucess, SelectedOptionToVideoReport = string.IsNullOrEmpty(ReportText) ? GetReportString(option, subOption):ReportText };
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return new ReportVideoResponseHandler();
            }
        }
        private int GetCordinate(string cord)
        {
            try
            {
                var deciMal = Convert.ToDecimal(cord);
                return Convert.ToInt32(deciMal);
            }
            catch (Exception ex)
            {
                int.TryParse(cord, out int cordinate);
                return cordinate;
            }
        }
        private string GetReportString(int optionInt, int subOptionInt)
        {
            optionInt = optionInt == 9 ? optionInt - 1 : optionInt;
            var option = new List<string>
            {
                "Sexual content", "Violent or repulsive content", "Hateful or abusive content","Harassment or bullying",
                "Harmful dangerous acts","Misinformation", "Child abuse", "Promotes terrorism", "Spam or misleading",
            }[optionInt];
            var subOption = string.Empty;

            #region OLD Code for subreport
            //switch (optionInt)
            //{
            //    case 0:
            //        subOption = new List<string>
            //        {
            //            "Graphic sexual activity", "Nudity", "Suggestive, but without nudity",
            //            "Content involving minors", "Abusive title or description", "Other sexual content"
            //        }[subOptionInt];
            //        break;
            //    case 1:
            //        subOption = new List<string> { "Adults fighting", "Physical attack", "Youth violence", "Animal abuse" }[subOptionInt];
            //        break;
            //    case 2:
            //        subOption = new List<string>
            //        {
            //            "Promotes hatred or violence", "Abusing vulnerable individuals",
            //            "Abusive title or description"
            //        }[subOptionInt];
            //        break;
            //    case 3:
            //        subOption = new List<string>
            //        {
            //            "This is harassing me", "This is harassing someone else"
            //        }[subOptionInt];
            //        break;
            //    case 4:
            //        subOption = new List<string>
            //        {
            //            "Pharmaceutical or drug abuse", "Abuse of fire or explosives", "Suicide or self injury",
            //            "Other dangerous acts"
            //        }[subOptionInt];
            //        break;
            //    case 5:
            //    case 6:
            //    case 7:
            //        break;
            //    case 8:
            //        subOption = new List<string>
            //        {
            //            "Mass advertising", "Pharmaceutical drugs for sale", "Misleading text", "Misleading thumbnail",
            //            "Scams/fraud"
            //        }[subOptionInt];
            //        break;
            //    case 9:
            //        subOption = new List<string>
            //        {
            //            "Copyright issue", "Privacy issue", "Trademark infringement", "Defamation", "Counterfeit",
            //            "Other legal issue"
            //        }[subOptionInt];
            //        break;
            //    case 10:
            //        subOption = new List<string>
            //            {"Captions are missing (CVAA)", "Captions are inaccurate", "Captions are abusive"}[subOptionInt];
            //        break;
            //}
            #endregion

            return string.IsNullOrEmpty(subOption) ? $"{option}" : $"{option} [ {subOption} ]";
        }



        public List<YoutubePost> ScrapPostsFromChannel(string keyword, ActivityType actType, ref int lastIndex,
            ref string channelId, ref string channelUsername, bool firstPage, double delayBeforeHit = 0)
        {
            var list = new List<YoutubePost>();
            try
            {
                if (firstPage)
                {
                    var url = keyword.Contains("/videos") ? keyword : keyword + "/videos";
                    if (keyword.Contains("channel"))
                        channelId = Utilities.GetBetween(url, "youtube.com/channel/", "/");
                    else
                        channelUsername = Utilities.GetBetween(url, "youtube.com/user/", "/");

                    BrowserDispatcher(BrowserFuncts.GoToUrl, delayBeforeHit, url);
                    lastIndex = 0;
                    while (!string.IsNullOrEmpty(CurrentData))
                    {

                        BrowserDispatcher(BrowserFuncts.GetElementValue, 0.1, ActType.GetValue, AttributeType.ClassName,
                            "yt-simple-endpoint focus-on-expand style-scope ytd-rich-grid-media", ValueTypes.Href, lastIndex);
                        if (string.IsNullOrEmpty(CurrentData))
                            break;

                        lastIndex++;

                        var post = new YoutubePost();
                        post.Code = Utilities.GetBetween($"{CurrentData}/", "/watch?v=", "/");
                        if (post.Code.Contains("&"))
                            post.Code = Utilities.GetBetween($"MyOwnTextToGetBetween{post.Code}",
                                "MyOwnTextToGetBetween", "&");
                        if (!string.IsNullOrEmpty(post.Code))
                            post.PostUrl = $"https://www.youtube.com/watch?v={post.Code}";
                        post.ChannelUsername = channelUsername;
                        post.ChannelId = channelId;

                        list.Add(post);
                    }

                    // scroll screen 1000 px down
                    BrowserDispatcher(BrowserFuncts.Scroll, 0, 500);

                    return list;
                }

                var doIt4Times = 0;
                while (list.Count < 20)
                {
                    BrowserDispatcher(BrowserFuncts.GetElementValue, 0.1, ActType.GetValue, AttributeType.ClassName,
                        "yt-simple-endpoint inline-block style-scope ytd-thumbnail", ValueTypes.Href, lastIndex);
                    if (string.IsNullOrEmpty(CurrentData))
                    {
                        doIt4Times++;
                        // scroll screen 1000 px down
                        BrowserDispatcher(BrowserFuncts.Scroll, 0, 500);
                        if (doIt4Times >= 4)
                            break;
                        Sleep(2);
                        continue;
                    }

                    doIt4Times = 0;
                    lastIndex++;

                    var post = new YoutubePost();
                    post.Code = Utilities.GetBetween(CurrentData + "\"", "href=\"/watch?v=", "\"");
                    if (string.IsNullOrEmpty(post.Code))
                        post.Code = Utilities.GetBetween(CurrentData + "\"", "/watch?v=", "\"");
                    if (post.Code.Contains("&"))
                        post.Code = Utilities.GetBetween($"MyOwnTextToGetBetween{post.Code}", "MyOwnTextToGetBetween",
                            "&");
                    if (!string.IsNullOrEmpty(post.Code))
                        post.PostUrl = $"https://www.youtube.com/watch?v={post.Code}";
                    post.ChannelUsername = channelUsername;
                    post.ChannelId = channelId;

                    list.Add(post);
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return list;
        }

        public List<YoutubeChannel> ScrapChannelsFromKeyword(string keyword, ActivityType actType, ref int lastIndex,
            bool firstPage, double delayBeforeHit = 0)
        {
            var list = new List<YoutubeChannel>();
            try
            {
                if (firstPage)
                {
                    var url =
                        $"https://www.youtube.com/results?search_query={Uri.EscapeDataString(keyword)}&sp=EgIQAg%3D%3D";
                    BrowserDispatcher(BrowserFuncts.GoToUrl, delayBeforeHit, url);

                    while (!string.IsNullOrEmpty(CurrentData))
                    {
                        BrowserDispatcher(BrowserFuncts.GetElementValue, 0.1, ActType.GetValue, AttributeType.ClassName,
                            "yt-simple-endpoint style-scope ytd-channel-renderer", ValueTypes.Href, lastIndex);
                        if (string.IsNullOrEmpty(CurrentData))
                            break;

                        lastIndex = lastIndex + 2;

                        var channel = new YoutubeChannel();

                        channel.ChannelUsername = Utilities.GetBetween($"{CurrentData}/", "/user/", "/");

                        if (string.IsNullOrEmpty(channel.ChannelUsername))
                            channel.ChannelUsername = Utilities.GetBetween($"{CurrentData}/", "@", "/");

                        if (!string.IsNullOrEmpty(channel.ChannelUsername))
                            channel.HasChannelUsername = true;

                        channel.ChannelId = Utilities.GetBetween($"{CurrentData}/", "/channel/", "/");
                        if (string.IsNullOrEmpty(channel.ChannelId))
                            channel.ChannelId = Utilities.FirstMatchExtractor(CurrentData, ".com/(.*?)$");

                        if (!(string.IsNullOrEmpty(channel.ChannelId) && string.IsNullOrEmpty(channel.ChannelUsername))
                            && !list.Any(x =>
                                !string.IsNullOrEmpty(channel.ChannelId) && x.ChannelId == channel.ChannelId ||
                                !string.IsNullOrEmpty(channel.ChannelUsername) &&
                                x.ChannelUsername == channel.ChannelUsername))
                            list.Add(channel);
                    }

                    // scroll screen 1000 px down
                    BrowserDispatcher(BrowserFuncts.Scroll, 1, 1000);

                    return list;
                }

                var doIt4Times = 0;
                while (list.Count < 20)
                {
                    BrowserDispatcher(BrowserFuncts.GetElementValue, 0.1, ActType.GetValue, AttributeType.ClassName,
                        "yt-simple-endpoint style-scope ytd-channel-renderer", ValueTypes.Href, lastIndex);
                    if (string.IsNullOrEmpty(CurrentData))
                    {
                        doIt4Times++;
                        // scroll screen 1000 px down
                        BrowserDispatcher(BrowserFuncts.Scroll, 1, 2000);
                        if (doIt4Times > 4)
                            break;
                        Sleep(2);
                        continue;
                    }

                    doIt4Times = 0;
                    lastIndex++;

                    var channel = new YoutubeChannel();
                    channel.ChannelUsername = Utilities.GetBetween($"{CurrentData}/", "/user/", "/");
                    if (string.IsNullOrEmpty(channel.ChannelUsername))
                        channel.ChannelUsername = Utilities.GetBetween($"{CurrentData}/", "@", "/");
                    if (!string.IsNullOrEmpty(channel.ChannelUsername))
                        channel.HasChannelUsername = true;

                    channel.ChannelId = Utilities.GetBetween($"{CurrentData}/", "/channel/", "/");

                    if (!(string.IsNullOrEmpty(channel.ChannelUsername) && string.IsNullOrEmpty(channel.ChannelId)))
                        list.Add(channel);
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return list;
        }

        public List<YoutubeChannel> GetSubscribedChannels(ref int lastIndex, bool firstPage, double delayBeforeHit = 0)
        {
            var list = new List<YoutubeChannel>();
            try
            {
                if (firstPage)
                {
                    var url = "https://www.youtube.com/feed/channels";
                    BrowserDispatcher(BrowserFuncts.GoToUrl, delayBeforeHit, url);

                    while (!string.IsNullOrEmpty(CurrentData))
                    {
                        BrowserDispatcher(BrowserFuncts.GetElementValue, 0.1, ActType.GetValue, AttributeType.ClassName,
                            "yt-simple-endpoint style-scope ytd-channel-renderer", ValueTypes.Href, lastIndex);
                        if (string.IsNullOrEmpty(CurrentData))
                            break;

                        lastIndex += 2;

                        var channel = new YoutubeChannel();

                        channel.ChannelUsername = Utilities.GetBetween($"{CurrentData}/", "/user/", "/");
                        if (string.IsNullOrEmpty(channel.ChannelUsername))
                            channel.ChannelUsername = $"@{Utilities.GetBetween($"{CurrentData}/", "@", "/")}";
                        if (!string.IsNullOrEmpty(channel.ChannelUsername))
                            channel.HasChannelUsername = true;

                        channel.ChannelId = Utilities.GetBetween($"{CurrentData}/", "/channel/", "/");

                        if (!(string.IsNullOrEmpty(channel.ChannelUsername) && string.IsNullOrEmpty(channel.ChannelId)))
                            list.Add(channel);
                    }

                    // scroll screen 1000 px down
                    BrowserDispatcher(BrowserFuncts.Scroll, 2, 1000);

                    return list;
                }

                var doIt4Times = 0;
                while (list.Count < 20)
                {
                    BrowserDispatcher(BrowserFuncts.GetElementValue, 0.1, ActType.GetValue, AttributeType.ClassName,
                        "yt-simple-endpoint style-scope ytd-channel-renderer", ValueTypes.Href, lastIndex);
                    if (string.IsNullOrEmpty(CurrentData))
                    {
                        doIt4Times++;
                        // scroll screen 1000 px down
                        BrowserDispatcher(BrowserFuncts.Scroll, 1, 2000);
                        if (doIt4Times > 4)
                            break;
                        Sleep(2);
                        continue;
                    }

                    doIt4Times = 0;
                    lastIndex++;

                    var channel = new YoutubeChannel();
                    channel.ChannelUsername = Utilities.GetBetween($"{CurrentData}/", "/user/", "/");
                    if (!string.IsNullOrEmpty(channel.ChannelUsername))
                        channel.HasChannelUsername = true;

                    channel.ChannelId = Utilities.GetBetween($"{CurrentData}/", "/channel/", "/");

                    if (!(string.IsNullOrEmpty(channel.ChannelUsername) && string.IsNullOrEmpty(channel.ChannelId)))
                        list.Add(channel);
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return list;
        }

        public List<YoutubePostCommentModel> ScrapePostCommentsDetails(ActivityType actType, string lastCommentId,
            ref int lastIndex, double delayBeforeHit = 0, int noOfScraping = 5)
        {
            var list = new List<YoutubePostCommentModel>();
            var lst = new List<string>();
            var foundLastOne = false;
            try
            {

                BrowserDispatcher(BrowserFuncts.GetPageSourceNoCondition, 1);
                lst = HtmlParseUtility.GetListInnerHtmlFromPartialTagName(_html, "ytd-comment-renderer",
                                "id", "comment");
                lst = lst == null || lst.Count == 0 ? HtmlParseUtility.GetListInnerHtmlFromPartialTagName(_html, "ytd-comment-view-model",
                                "id", "comment") : lst;
                int index = 0;
                var doIt4Times = 0;
                while (list.Count < noOfScraping)
                {
                    BrowserDispatcher(BrowserFuncts.GetElementValue, 0.1, ActType.GetValue, AttributeType.ClassName,
                        "style-scope ytd-item-section-renderer", ValueTypes.InnerHtml, lastIndex);
                    if (string.IsNullOrEmpty(CurrentData) ||
                        !CurrentData.Contains("class=\"yt-simple-endpoint style-scope ytd-comment-view-model\" href=\""))
                    {
                        BrowserDispatcher(BrowserFuncts.GetElementValue, 0.1, ActType.ActByQuery, AttributeType.Id,
                        "comment", ValueTypes.InnerHtml, lastIndex);
                        if (string.IsNullOrEmpty(CurrentData) || CurrentData.StartsWith("\n"))
                            CurrentData = lst[index];
                        if (string.IsNullOrEmpty(CurrentData) ||
                        !CurrentData.Contains("class=\"yt-simple-endpoint style-scope ytd-comment-renderer\" href=\""))
                        {
                            doIt4Times++;
                            // scroll screen 1000 px down
                            BrowserDispatcher(BrowserFuncts.Scroll, 1, 500);
                            if (doIt4Times > 3)
                                break;
                            Sleep(2);
                            lastIndex++;
                            continue;
                        }
                    }

                    doIt4Times = 0;
                    lastIndex++;
                    index++;

                    var commentInfo = new CommentInfoFromBrowser(Response);

                    if (!string.IsNullOrEmpty(commentInfo.CommentModel.CommentId) &&
                        (list.Count == 0 || list.Any(x => x.CommentId != commentInfo.CommentModel.CommentId)))
                    {
                        if (!foundLastOne && (!string.IsNullOrEmpty(lastCommentId) && lastCommentId != commentInfo.CommentModel.CommentId))
                        {
                            continue;
                        }

                        if (lastCommentId == commentInfo.CommentModel.CommentId)
                        {
                            foundLastOne = true;
                            continue;
                        }

                        list.Add(commentInfo.CommentModel);
                    }
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return list;
        }
        public bool LikeComments(YoutubePost postWithTheComment, string messageText, double delayBeforeHit = 0)
        {
            bool isLiked = false;
            try
            {
                if (!string.IsNullOrEmpty(postWithTheComment.InteractingCommentId))
                    BrowserDispatcher(BrowserFuncts.GoToUrl, 2, $"{postWithTheComment.PostUrl}&lc={postWithTheComment.InteractingCommentId}");
                BrowserDispatcher(BrowserFuncts.MouseScroll, 2, ScreenResolution.Key / 2, ScreenResolution.Key / 2, 0, -150, 5, 3);
                //Check if already Liked
                #region OLD CODE TO LIKE COMMENT.
                //BrowserDispatcher(BrowserFuncts.GetElementValue, 1, ActType.GetValue, AttributeType.ClassName,
                //YdStatic.LikeCommentClass, ValueTypes.OuterHtml, postWithTheComment.CommentLikeIndex);
                //try
                //{
                //    if (CurrentData.Contains("aria-pressed="))
                //        isLiked = bool.Parse(Regex.Matches(CurrentData, "aria-pressed=\"(.*?)\"")[0].Groups[0].ToString().
                //                    Replace("aria-pressed=", "")
                //                    .Replace("\"", ""));
                //    if (isLiked)
                //    {
                //        postWithTheComment.InteractingCommentLikeStatus = DominatorHouseCore.DatabaseHandler.YdTables.Accounts.InteractedPosts.LikeStatus.Like;
                //        return false;
                //    }
                //}
                //catch (Exception)
                //{ }
                ////if not liked then click like button
                //BrowserDispatcher(BrowserFuncts.BrowserAct, delayBeforeHit, ActType.Click, AttributeType.ClassName,
                //   YdStatic.LikeCommentClass, "",
                //   postWithTheComment.CommentLikeIndex);
                #endregion
                var Result = BrowserWindow.ExecuteScript("document.querySelectorAll('ytd-toggle-button-renderer[id=\"like-button\"]>yt-button-shape>button')[0].ariaPressed", delayInSec: 3);
                if (Result != null && bool.TryParse(Result?.Result?.ToString(), out isLiked))
                {
                    if (isLiked)
                    {
                        postWithTheComment.InteractingCommentLikeStatus = DominatorHouseCore.DatabaseHandler.YdTables.Accounts.InteractedPosts.LikeStatus.Like;
                        return false;
                    }
                }
                BrowserWindow.ExecuteScript("document.querySelectorAll('ytd-toggle-button-renderer[id=\"like-button\"]>yt-button-shape>button[aria-pressed=\"false\"]')[0].click();", delayInSec: 3);
                BrowserDispatcher(BrowserFuncts.GetPageText, 3.5);
                if (_pageText.Contains("Comment as...") && _pageText.Contains("Create channel"))
                {
                    BrowserDispatcher(BrowserFuncts.BrowserAct, delayBeforeHit, ActType.CustomActByQueryType, AttributeType.AriaLabel,
                       "Create channel", "click()", 0);
                    BrowserDispatcher(BrowserFuncts.RefreshPage, 10);
                    BrowserDispatcher(BrowserFuncts.MouseScroll, 10, ScreenResolution.Key / 2, ScreenResolution.Key / 2, 0, -100, 0, 3);

                    BrowserDispatcher(BrowserFuncts.BrowserAct, delayBeforeHit, ActType.Click, AttributeType.ClassName,
                           YdStatic.LikeCommentClass, "",
                           postWithTheComment.CommentLikeIndex);
                }
                // Scroll screen 100 px down
                BrowserDispatcher(BrowserFuncts.MouseScroll, 2, ScreenResolution.Key / 2, ScreenResolution.Key / 2, 0, -100, 0, 1);
                BrowserDispatcher(BrowserFuncts.GetElementValue, 1, ActType.GetValue, AttributeType.ClassName,
               YdStatic.LikeCommentClass, ValueTypes.OuterHtml, postWithTheComment.CommentLikeIndex);
                // Check if Like button is pressed or not
                //if (CurrentData.Contains("aria-pressed="))
                //    isLiked = bool.Parse(Regex.Matches(CurrentData, "aria-pressed=\"(.*?)\"")[0].Groups[0].ToString().
                //            Replace("aria-pressed=", "")
                //            .Replace("\"", ""));
                Result = BrowserWindow.ExecuteScript("document.querySelectorAll('ytd-toggle-button-renderer[id=\"like-button\"]>yt-button-shape>button[aria-pressed=\"true\"]')[0].ariaPressed", delayInSec: 3);
                bool.TryParse(Result?.Result?.ToString(), out isLiked);
                return isLiked;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return isLiked;
            }
        }

        public string PublishVideoAndGetVideoId(PublisherPostlistModel postDetails, string channelPageId, OtherConfigurationModel otherConfigurationModel,
            double delayBeforeHit = 0, bool SwicthChannelForPublisher = false)
        {
            try
            {
                if (channelPageId == YdStatic.DefaultChannel)
                    channelPageId = YdStatic.GetDefaultChannel;

                Sleep(delayBeforeHit);

                // Switch channel if current active channel is different
                SwitchChannel(channelPageId, 2, SwicthChannelForPublisher);
                var filePath = postDetails.IsChangeHashOfMedia
                    ? MediaUtilites.CalculateMD5Hash(postDetails.MediaList.GetRandomItem())
                    : postDetails.MediaList.GetRandomItem();
                var memeType = MediaUtilites.GetMimeTypeByFilePath(filePath);
                var IsImage = !string.IsNullOrEmpty(memeType) && memeType.StartsWith("image", StringComparison.OrdinalIgnoreCase);
                var IsCommunity = otherConfigurationModel.IsYoutubeCommunityPost;
                // Load Upload Video Page
                var url = IsCommunity ? !string.IsNullOrEmpty(channelPageId) && channelPageId.Contains("@")? $"https://www.youtube.com/{channelPageId}": $"https://www.youtube.com/@{channelPageId}" : $"https://www.youtube.com/upload?redirect_to_creator=true&fr=4&ar={DateTime.Now.GetCurrentEpochTimeMilliSeconds()}&nv=1";
                BrowserDispatcher(BrowserFuncts.GoToUrl, 2, url);
                if (!IsCommunity)
                {
                    // get Upload button pixel position  
                    BrowserDispatcher(BrowserFuncts.GetXY, 1, AttributeType.ClassName, "style-scope ytcp-uploads-file-picker-animation");



                    BrowserDispatcher(BrowserFuncts.SelectFileFromDialog, 0, filePath);

                    // get mouse to click over the upload button pixel loation
                    BrowserDispatcher(BrowserFuncts.MouseClick, 0, lastXandY.Key + 68, lastXandY.Value + 90);

                    if (UploadingFailed(5))
                        return "";

                    Sleep(2);

                    // get videoId after publishing
                    return PublishNow(postDetails);
                }
                else
                {
                    return PublishOnCommunity(postDetails,filePath,IsImage);
                }
                
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return "";
            }
        }

        private string PublishOnCommunity(PublisherPostlistModel postDetails,string filePath,bool IsImage)
        {
            var videoID = string.Empty;
            try
            {
                if (!IsImage)
                    return "Invalid media for community post!";
                if (!string.IsNullOrEmpty(postDetails.PostDescription))
                {
                    BrowserDispatcher(BrowserFuncts.ExecuteScript, 1, "document.querySelector('div[id=\"placeholder-area\"]>yt-formatted-string[role=\"button\"]').click();", 3);
                    BrowserDispatcher(BrowserFuncts.EnterChars, 1, postDetails?.PostDescription);
                }
                // get Upload button pixel position  
                var script = "document.querySelector('ytd-button-renderer[id=\"image-button\"] button[aria-label=\"Add an image\"]').getBoundingClientRect().{0}";
                var XandY = BrowserWindow.GetXAndYAsync(customScriptX:string.Format(script,"x"),customScriptY:string.Format(script,"y")).Result;
                if(XandY.Key == 0 && XandY.Value == 0)
                {
                    script = "document.querySelector('span[id=\"image-button\"] button[aria-label=\"Add an image\"]').getBoundingClientRect().{0}";
                    XandY = BrowserWindow.GetXAndYAsync(customScriptX: string.Format(script, "x"), customScriptY: string.Format(script, "y")).Result;
                }
                // get mouse to click over the upload button pixel loation
                BrowserDispatcher(BrowserFuncts.MouseClick, 0, XandY.Key + 10, XandY.Value + 5);
                BrowserDispatcher(BrowserFuncts.SelectFileFromDialog, 0, filePath);
                BrowserDispatcher(BrowserFuncts.ExecuteScript, 1, "document.querySelector('a[id=\"select-link\"]').click();", 6);
                BrowserWindow.ClearResources();
                BrowserDispatcher(BrowserFuncts.ExecuteScript, 1, "document.querySelectorAll('ytd-button-renderer[id=\"submit-button\"] button[aria-label=\"Post\"]')[0].click();", 5);
                var Result = BrowserWindow.ExecuteScript("document.querySelector('button[aria-label=\"Got it\"]').click();", 3);
                if(Result != null && Result.Success)
                    BrowserDispatcher(BrowserFuncts.ExecuteScript, 1, "document.querySelectorAll('ytd-button-renderer[id=\"submit-button\"] button[aria-label=\"Post\"]')[0].click();", 5);
                var Nodes = HtmlParseUtility.GetListNodeFromPartialTagNamecontains(BrowserWindow.GetPageSource(),"a", "aria-label", "Go to post detail");
                if(Nodes != null && Nodes.Count > 0)
                {
                    var recentNode = Nodes?.FirstOrDefault();
                    var postID = recentNode.GetAttributeValue("href", string.Empty)?.Replace("/post/", "")?.Replace("/","");
                    if (!string.IsNullOrEmpty(postID))
                        videoID = $"https://www.youtube.com/post/{postID}";
                }
            }
            catch { }
            return videoID;
        }

        private string PublishNow(PublisherPostlistModel postDetails)
        {
            if (UploadingFailed(5))
                return "";
            var videoId = "";
            var title = !string.IsNullOrWhiteSpace(postDetails.PublisherInstagramTitle) ? postDetails.PublisherInstagramTitle : postDetails.PostDescription.Substring(0, 12);
            //check if the title is present as file name
            BrowserDispatcher(BrowserFuncts.GetElementValue, 1, ActType.ClickById, AttributeType.Id, "textbox", ValueTypes.InnerText);
            //now if present then clear the title textbox
            if (!string.IsNullOrEmpty(CurrentData))
            {
                BrowserDispatcher(BrowserFuncts.GetXY, delayBefore: 1, AttributeType.ClassName, "style-scope ytcp-social-suggestions-textbox", 1);
                BrowserDispatcher(BrowserFuncts.MouseClick, delayBefore: 1, lastXandY.Key + 7, lastXandY.Value + 3);
                BrowserDispatcher(BrowserFuncts.SelectAll);
                BrowserDispatcher(BrowserFuncts.PressKey, delayBefore: 1, 8);
            }
            //Enter Title
            BrowserDispatcher(BrowserFuncts.CopyPasteContent, 0.5, title, 2);
            //Now press 'Tab' key 2 times
            BrowserDispatcher(BrowserFuncts.PressKey, 0.5, 9, 2, 200);
            //Enter Description
            BrowserDispatcher(BrowserFuncts.CopyPasteContent, 1, postDetails.PostDescription);
            //Scroll down in details section
            BrowserDispatcher(BrowserFuncts.BrowserAct, 5, ActType.ScrollIntoView, AttributeType.ClassName, "toggle-section style-scope ytcp-video-metadata-editor");
            //Select Radio button of 'Not made for kids'
            BrowserDispatcher(BrowserFuncts.BrowserAct, 1, ActType.Click, AttributeType.Name, "VIDEO_MADE_FOR_KIDS_NOT_MFK");
            //Now select visibility badge 
            BrowserDispatcher(BrowserFuncts.BrowserAct, 1.5, ActType.Click, AttributeType.ClassName, "step-title style-scope ytcp-stepper", "", 3);
            BrowserDispatcher(BrowserFuncts.BrowserAct, 1.5, ActType.Click, AttributeType.ClassName, "step-title style-scope ytcp-stepper", "", 3);
            //Select Radio button of 'Public'
            BrowserDispatcher(BrowserFuncts.BrowserAct, 1, ActType.Click, AttributeType.Name, "PUBLIC");
            Sleep(5);
            //Now Click on Publish button
            //       BrowserDispatcher(BrowserFuncts.BrowserAct, 1.5, ActType.Click, AttributeType.ClassName, "label style-scope ytcp-button", "", 13);
            BrowserDispatcher(BrowserFuncts.BrowserAct, 1.5, ActType.ClickById, AttributeType.Id, "done-button", "", 0);

            Sleep(5);

            var wait = 0;
            do
            {
                BrowserDispatcher(BrowserFuncts.GetPageSource); // getting whole html to get videoId of unploaded video from in it.
                videoId = Utilities.GetBetween(CurrentData, "href=\"https://youtu.be/", "\"");

                if (string.IsNullOrEmpty(videoId))
                    videoId = Utilities.GetBetween(CurrentData, "href=\"https://youtube.com/shorts/", "\"");

                if (!string.IsNullOrEmpty(videoId))
                    break;

                Sleep(3);
                wait += 3;
            } while (wait <= 30);

            return videoId;
        }

        private bool UploadingFailed(double delayBeforeHit = 0)
        {
            BrowserDispatcher(BrowserFuncts.GetElementValue, delayBeforeHit, ActType.GetValue, AttributeType.ClassName,
                "upload-failure hid", ValueTypes.InnerHtml, 1);
            if (CurrentData.Contains("href=\"//support.google.com"))
            {
                var failedWhy = YdStatic.GetTextRemoveHtmlTags(CurrentData);
                CustomLog(ActivityType.Post, failedWhy);
                return true;
            }

            return false;
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
                    });
            }
            catch (Exception ex)
            { ex.DebugLog(); }
            Thread.Sleep(1000);
        }
        public void CloseBrowserCustom()
        {
            try
            {
                if (CustomBrowserWindow != null)
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        CustomBrowserWindow.Close();
                        CustomBrowserWindow.Dispose();
                        CustomBrowserWindow = null;
                        GC.Collect();
                    });
            }
            catch (Exception ex)
            { ex.DebugLog(); }
            Thread.Sleep(1000);
        }

        void IYdBrowserManager.ScrollScreen(int toPixels)
        {
            BrowserDispatcher(BrowserFuncts.Scroll, 0, 100);
        }

        private void CustomLog(ActivityType activityType, string message)
        {
            GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.YouTube,
                _account.UserName, activityType, message);
        }

        private void CustomLog(string message)
        {
            GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.YouTube,
                _account.UserName, "Browser Login", message);
        }

        private void Sleep(double seconds = 1)
        {
            try
            {
                Task.Delay(TimeSpan.FromSeconds(seconds)).Wait(_cancellationToken);
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception) { }
        }

        public SearchPostsResponseHandler ScrollWindowAndGetPosts(DominatorAccountModel account, ActivityType actType, int noOfPagetoScroll, int lastPageno = 0)
        {

            int failedCount = 0;
            int currentCount = 0;
            int previousCount = 0;
            string pageSource = string.Empty;
            bool isRunning = true;
            var jsonresponseList = new List<string>();
            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    var tempjsonresponselist = new List<string>();
                    while (lastPageno < noOfPagetoScroll && failedCount < 2)
                    {
                        await BrowserWindow.MouseScrollAsync(ScreenResolution.Key / 2 + 50, ScreenResolution.Value / 2, 0, -600, delayAfter: 5, scrollCount: 8);
                        tempjsonresponselist = await BrowserWindow.GetPaginationDataList("\"responseContext\"", true, "\"videoRenderer\":");
                        if (tempjsonresponselist.Count == 0)
                            tempjsonresponselist = await BrowserWindow.GetPaginationDataList("\"responseContext\"", true, "\"itemSectionRenderer\":");

                        tempjsonresponselist.RemoveAll(x => jsonresponseList.Any(y => y.Contains(x)));
                        currentCount += tempjsonresponselist.Count;
                        jsonresponseList.AddRange(tempjsonresponselist);
                        lastPageno++;

                        if (currentCount == previousCount && jsonresponseList.Count == currentCount) failedCount++;
                        else
                        {
                            failedCount = 0;
                            previousCount = currentCount;
                        }
                        _cancellationToken.ThrowIfCancellationRequested();
                    }
                    pageSource = await BrowserWindow.GetPageSourceAsync();
                    BrowserWindow.ClearResources();
                }
                catch (Exception)
                {

                }
                isRunning = false;
            });
            while (isRunning)
                Task.Delay(2000, _cancellationToken).Wait();
            return new SearchPostsResponseHandler(new ResponseParameter() { Response = pageSource }, jsonresponseList, failedCount < 2);

        }


        #endregion
    }
}