using CefSharp;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.PuppeteerBrowser;
using DominatorHouseCore.Request;
using DominatorHouseCore.Utility;
using EmbeddedBrowser;
using PinDominatorCore.Interface;
using PinDominatorCore.PDEnums;
using PinDominatorCore.PDLibrary.BrowserManager;
using PinDominatorCore.PDModel;
using PinDominatorCore.PDUtility;
using PinDominatorCore.Response;
using PinDominatorCore.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
namespace PinDominatorCore.PDLibrary
{
    public interface IPdBrowserManager : IBrowserManager
    {
        string Domain { get; set; }
        List<BrowserWindow> BrowserWindows { get; set; }
        string CurrentData { get; set; }
        void SetAccount(DominatorAccountModel account);
        PinPostStatusResponseHandler IsValidImage(string PageResponse);
        void AddNew(CancellationTokenSource cancellationToken, string openWithUrl = "https://www.pinterest.com",
            LoginType loginType = LoginType.AutomationLogin);
        void CloseLast(bool getCookies = false);
        void OpenBrowser(DominatorAccountModel account, string url, CancellationTokenSource cancellationToken,
            LoginType loginType = LoginType.AutomationLogin);
        JavascriptResponse BrowserDispatcher(BrowserFuncts function, CancellationTokenSource cancellationToken, double delayBefore = 0, params object[] paramS);
        void ChangeLanguage(CancellationTokenSource cancellationToken);
        List<PinterestPin> SearchPinsByKeyword(DominatorAccountModel account, string keyWord, CancellationTokenSource cancellationToken,
            bool isScroll = false, int scroll = 10);
        BoardInfoPtResponseHandler SearchByCustomBoard(DominatorAccountModel account, string boardUrl, CancellationTokenSource cancellationToken);
        List<PinterestBoard> SearchBoardsOfUser(DominatorAccountModel account, string userUrl, CancellationTokenSource cancellationToken,
            bool isScroll = false, int scroll = 10);
        PinInfoPtResponseHandler SearchByCustomPin(DominatorAccountModel account, string pin, CancellationTokenSource cancellationToken);
        UserNameInfoPtResponseHandler SearchByCustomUser(DominatorAccountModel account, string user, CancellationTokenSource cancellationToken);
        BoardResponse FollowBoard(DominatorAccountModel account, string boardUrl, CancellationTokenSource cancellationToken);
        FriendshipsResponse FollowUser(DominatorAccountModel account, string user, CancellationTokenSource cancellationToken);
        FriendshipsResponse UnfollowUser(DominatorAccountModel account, string user, CancellationTokenSource cancellationToken);
        MessageResponseHandler Message(DominatorAccountModel account, string user, string msg, CancellationTokenSource cancellationToken);
        List<PinterestUser> GetUserFollowings(DominatorAccountModel account, string user, CancellationTokenSource cancellationToken,
            bool isScroll = false, int scroll = 10);
        List<PinterestUser> GetUserFollowers(DominatorAccountModel account, string user, CancellationTokenSource cancellationToken,
            bool isScroll = false, int scroll = 10);
        List<PinterestUser> GetBoardFollowers(DominatorAccountModel account, string boardUrl, CancellationTokenSource cancellationToken,
         bool isScroll = false, int scroll = 10);
        CommentResponse Comment(DominatorAccountModel account, string pinId, string comment, CancellationTokenSource token);
        TryResponse Try(DominatorAccountModel account, string pinId, string note, string path, CancellationTokenSource token);
        RepostPinResponseHandler Post(DominatorAccountModel account, string boardUrl, PublisherPostlistModel postDetails, CancellationTokenSource token,SectionDetails sectionDetails=null);
        List<PinterestBoard> SearchBoardsByKeyword(DominatorAccountModel account, string keyWord, CancellationTokenSource cancellationToken,
            bool isScroll = false, int scroll = 10);
        List<PinterestPin> SearchPinsOfBoard(DominatorAccountModel account, string keyWord, CancellationTokenSource cancellationToken,
            bool isScroll = false, int scroll = 10);
        List<PinterestPin> SearchPinsOfUser(DominatorAccountModel account, string userUrl, CancellationTokenSource cancellationToken,
            bool isScroll = false, int scroll = 10);
        DeletePinResponseHandler DeletePin(DominatorAccountModel account, string pinId, CancellationTokenSource cancellationToken);
        BoardInvitationsResponseHandler SearchBoardsToAcceptInvitaion(DominatorAccountModel account, CancellationTokenSource cancellationToken);
        FindMessageResponseHandler SearchForMessagesToReply(DominatorAccountModel account, CancellationTokenSource cancellationToken,
            bool isScroll = false);
        AcceptBoardInvitationResponseHandler AcceptBoardInvitation(DominatorAccountModel account, string boardUrl, CancellationTokenSource cancellationToken);
        List<PinterestPin> SearchNewsFeedPins(DominatorAccountModel account, CancellationTokenSource cancellationToken,
            bool isScroll = false, int scroll = 10);
        BoardResponse CreateBoard(DominatorAccountModel account, BoardInfo boardInfo, CancellationTokenSource cancellationToken);
        SendBoardInvitationResponseHandler SendBoardInvitation(DominatorAccountModel account, PinterestBoard board, CancellationTokenSource cancellationToken);
        RepostPinResponseHandler EditPin(DominatorAccountModel account, PinterestPin pin, CancellationTokenSource cancellationToken);
        RepostPinResponseHandler Repin(DominatorAccountModel account, string pinId, string boardName, CancellationTokenSource cancellationToken,SectionDetails section=null);
        List<PinterestUser> SearchUsersByKeyword(DominatorAccountModel account, string keyWord, CancellationTokenSource cancellationToken,
            bool isScroll = false, int scroll = 10);
        List<PinterestUser> GetUsersWhoTriedPin(DominatorAccountModel account, string pinId, CancellationTokenSource cancellationToken,
            bool isScroll = false, int scroll = 10);
        PinPostStatusResponseHandler SwithcToBusinessAccount(DominatorAccountModel dominatorAccount,IPdBrowserManager browserManager);
        void DelayBeforeOperation(int delay = 4000);
    }

    public class PdBrowserManager : IPdBrowserManager
    {
        DominatorAccountModel _account;
        public List<BrowserWindow> BrowserWindows { get; set; }
        //public List<PuppeteerBrowserActivity> BrowserWindows { get; set; }
        public string CurrentData { get; set; }
        public bool VerifyingAccount { get; set; }
        public bool CustomUse { get; set; }
        public IPDAccountSessionManager sessionManager {  get; set; }
        public string Domain { get; set; }
        private readonly JsonJArrayHandler handler = JsonJArrayHandler.GetInstance;
        public PdBrowserManager()
        {
            BrowserWindows = new List<BrowserWindow>();
            sessionManager = sessionManager ?? InstanceProvider.GetInstance<IPDAccountSessionManager>();
        }

        #region Browser Functions
        public void SetAccount(DominatorAccountModel account)
         => _account = account;

        public void OpenBrowser(DominatorAccountModel account, string url, CancellationTokenSource cancellationToken,
            LoginType loginType = LoginType.AutomationLogin)
        {
            if (BrowserWindows.Count == 0)
            {
                SetAccount(account);
                AddNew(cancellationToken, url, loginType);
            }
        }

        public bool BrowserLogin(DominatorAccountModel account, CancellationToken token, LoginType loginType = LoginType.AutomationLogin, VerificationType verificationType = 0)
        {
            sessionManager.AddOrUpdateSession(ref account);
            SetAccount(account);
            BrowserWindows?.Clear();
            if (account.Cookies != null && account.Cookies.Count > 0)
            {
                Domain = account.Cookies["csrftoken"]?.Domain;
                Domain = Domain[0].Equals('.') ? Domain.Remove(0, 1) : Domain;
            }
            else
                Domain = "www.pinterest.com";

            OpenBrowser(account, $"https://{Domain}/login", account.CancellationSource, loginType);
            VerifyingAccount = true;

            BrowserLoginOnLoaded();

            var last2Min = DateTime.Now;
            AccountStatus status = _account.AccountBaseModel.Status;
            while (true)
            {
                BrowserDispatcher(BrowserFuncts.GetPageSource, account.CancellationSource, 0, "pinterest", false);

                if (CurrentData.Contains("There's been some strange activity on your account, so we put it in safe mode to protect your Pins. To get going again, just reset your password.")
                   || CurrentData.Contains("We noticed some strange activity on your Pinterest account so we reset your password and logged everyone out (including you).")
                   ||CurrentData.Contains("Reset your password"))
                    status = AccountStatus.SetNewPassword;

                else if (CurrentData.Contains("The password you entered is incorrect") ||
                    CurrentData.Contains("that password isn't right"))
                    status = AccountStatus.InvalidCredentials;

                else if (!string.IsNullOrEmpty(CurrentData) && (CurrentData.Contains("\"isAuth\": true") || CurrentData.Contains("\"isAuth\":true")
                    || CurrentData.Contains("\\\"isAuth\\\":true") || CurrentData.Contains("\\\"isAuth\\\": true")||CurrentData.Contains("\"isAuthenticated\":true")))
                    status = AccountStatus.Success;

                else if (CurrentData.Contains("It looks like you’re logging in a lot. Log in with Facebook or Google if you’re connected, or reset your password. Or you can wait 30 minutes and try again."))
                    status = AccountStatus.TemporarilyBlocked;

                else if (CurrentData.Contains("Sorry! Something went wrong on our end!"))
                    status = AccountStatus.Failed;

                else if (CurrentData.Contains("Oops! You logged in too quickly. Please try again with the reCAPTCHA"))
                {
                    #region Captcha Implementation
                    var genericFileManager = InstanceProvider.GetInstance<IGenericFileManager>();
                    var imageCaptchaServicesModel = genericFileManager.GetModel<DominatorHouseCore.Models.Config.ImageCaptchaServicesModel>(ConstantVariable.GetImageCaptchaServicesFile());
                    var content = BrowserWindows.Last().GetElementValueAsync(ActType.GetValue,
                    AttributeType.ClassName, "g-recaptcha").Result;

                    var source = Utilities.GetBetween(content, "iframe src=\"", "\"");
                    var k = Utilities.GetBetween(source, "&amp;k=", "&amp;");
                    ImageTypersHelper imageTyperz = new ImageTypersHelper(imageCaptchaServicesModel.Token);//5EA22F642D6D4C9BAFCD966685E9B4D1
                    var captchaId = imageTyperz.SubmitSiteKey("https://www.pinterest.com/login", k);
                    string capcthaResponse = imageTyperz.GetGResponseCaptcha(captchaId, account.AccountBaseModel.UserName);
                    var style = BrowserWindows.Last().ExecuteScriptAsync("document.getElementById('g-recaptcha-response').getAttribute('style')").Result;
                    string modifiedStyle = style.Result.ToString().Replace("display: none;", "display");
                    BrowserWindows.Last().ExecuteScriptAsync($"document.getElementById('g-recaptcha-response').setAttribute('style','{modifiedStyle}')").Wait();
                    BrowserWindows.Last().ExecuteScriptAsync($"document.getElementById('g-recaptcha-response').value = \"{capcthaResponse}\"").Wait();

                    var submitClass = CurrentData.Contains("type=\"email\"") ? "red SignupButton active" : "SignupButton";
                    BrowserDispatcher(BrowserFuncts.BrowserAct, account.CancellationSource, 2, ActType.Click, AttributeType.ClassName, submitClass);
                    Thread.Sleep(15000);

                    #endregion

                    status = AccountStatus.TemporarilyBlocked;
                }

                _account.Token.ThrowIfCancellationRequested();
                if ((!VerifyingAccount && status != AccountStatus.TryingToLogin) || last2Min.AddMinutes(2) < DateTime.Now)
                {
                    if (_account != null)
                        break;
                }
                Thread.Sleep(2000);
            }
            _account.AccountBaseModel.Status = status;
            if (status != AccountStatus.Success)
            {
                _account.IsUserLoggedIn = false;
                PdStatic.FailedLog(_account,PdStatic.GetFailedMessage(status));
            }
            SocinatorAccountBuilder.Instance(_account.AccountBaseModel.AccountId)
                    .AddOrUpdateDominatorAccountBase(_account.AccountBaseModel)
                    .AddOrUpdateLoginStatus(_account.IsUserLoggedIn)
                    .AddOrUpdateBrowserCookies(_account.Cookies)
                    .AddOrUpdateCookies(_account.Cookies)
                    .SaveToBinFile();
            return _account.IsUserLoggedIn;
        }

        public void ChangeLanguage(CancellationTokenSource cancellationToken)
        {
            BrowserDispatcher(BrowserFuncts.GoToUrl, cancellationToken, 2, $"https://{Domain}/settings/", "Account settings");
            BrowserDispatcher(BrowserFuncts.MouseClick, cancellationToken, 2, 352, 174);
            BrowserDispatcher(BrowserFuncts.MouseClick, cancellationToken, 2, 914, 500);
            BrowserDispatcher(BrowserFuncts.MouseClick, cancellationToken, 2, 914, 500);
            BrowserDispatcher(BrowserFuncts.PressKey, cancellationToken, 2, 38, 40, 100);
            BrowserDispatcher(BrowserFuncts.PressKey, cancellationToken, 2, 40, 6, 100);
            BrowserDispatcher(BrowserFuncts.MouseClick, cancellationToken, 2, 1043, 152);
        }

        //public void Navigate(string url, byte[] postDataBytes, string csrfToken, string referer)
        //{
        //    try
        //    {
        //        IFrame frame = BrowserWindows.Last().GetFrame("https://www.google.com/recaptcha/api2/anchor").Result;
        //        IRequest request = frame.CreateRequest();
        //        request.Url = url;
        //        request.Method = "POST";
        //        request.InitializePostData();
        //        var element = request.PostData.CreatePostDataElement();
        //        element.Bytes = postDataBytes;
        //        request.PostData.AddElement(element);

        //        System.Collections.Specialized.NameValueCollection headers = new System.Collections.Specialized.NameValueCollection();
        //        headers.Add("Content-Type", "application/x-www-form-urlencoded");
        //        headers.Add("Connection", "keep-alive");
        //        headers.Add("X-Requested-With", "XMLHttpRequest");
        //        headers.Add("X-Pinterest-AppState", "background");
        //        headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/75.0.3770.100 Safari/537.36");
        //        headers.Add("Accept", "application/json, text/javascript, */*, q=0.01");
        //        headers.Add("X-CSRFToken", csrfToken);
        //        headers.Add("Referer", referer);
        //        request.Headers = headers;
        //        frame.LoadRequest(request);
        //    }
        //    catch (Exception ex)
        //    {
        //        ex.DebugLog();
        //    }
        //}

        public void BrowserLoginOnLoaded()
        {
            var isRunning = true;
            var last2Min = DateTime.Now.AddMinutes(2);
            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    while (!BrowserWindows.Last().IsLoggedIn && last2Min > DateTime.Now)
                    {
                        _account.Token.ThrowIfCancellationRequested();

                        if (BrowserWindows.Last().IsDisposed) return;
                        if (!BrowserWindows.Last().IsLoaded)
                        {
                            await Task.Delay(1000);
                            continue;
                        }

                        await PinterestBrowserLogin();
                        await Task.Delay(2000);
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
                isRunning = false;
            });
            while (isRunning)
            {
                Thread.Sleep(TimeSpan.FromSeconds(2));
            }
        }

        private async Task PinterestBrowserLogin()
        {
            await Task.Delay(TimeSpan.FromSeconds(10));
            var last2Min = DateTime.Now.AddMinutes(2);
            var pageSource = await BrowserWindows.Last().GetPageSourceAsync();
            while (pageSource.Equals("<html><head></head><body></body></html>") && last2Min > DateTime.Now)
            {
                await BrowserWindows.Last().GoToCustomUrl("https://www.pinterest.com/login/", 2);
                BrowserWindows.Last().Refresh();
                await Task.Delay(TimeSpan.FromSeconds(3));
                pageSource = await BrowserWindows.Last().GetPageSourceAsync();
            }

            if (!string.IsNullOrEmpty(pageSource) && (pageSource.Contains("\"isAuth\": true")
                || pageSource.Contains("\"isAuth\":true") || pageSource.Contains("\\\"isAuth\\\":true")||pageSource.Contains("\"isAuthenticated\":true")))
            {
                await BrowserWindows.Last().GoToCustomUrl(BrowserWindows.Last().SearchUrl?.Replace("/login", ""));
            }

            if ((pageSource.Contains("type=\"email\"") || pageSource.Contains("type=\"password\"")) && !pageSource.Contains("isAuth\":true")
                && !pageSource.Contains("\"isAuth\":true") && !pageSource.Contains("\\\"isAuth\\\":true")&& !pageSource.Contains("\"isAuthenticated\":true")
                && !string.IsNullOrEmpty(_account.AccountBaseModel.UserName)
                && !string.IsNullOrEmpty(_account.AccountBaseModel.Password))
            {
                var getPageText = await BrowserWindows.Last().PageText();
                if (getPageText.Contains("that password isn't right.") || getPageText.ToLower().Contains("reset your password")
                    || getPageText.Contains("doesn't look like an email address or phone number") || getPageText.Contains("The email you entered does not belong to any account.")
                    || getPageText.Contains("Oops! You logged in too quickly. Please try again with the reCAPTCHA")
                    || getPageText.Contains("We noticed some strange activity on your account. Reset your password or log in with Facebook or Google to get back into your account.")
                    || getPageText.Contains("Hmm, wrong email or password. Try again!") || getPageText.Contains("Sorry! Something went wrong on our end!"))
                    return;

                if (getPageText.Contains("Already a member? Log in"))
                    await BrowserWindows.Last().BrowserActAsync(ActType.Click, AttributeType.ClassName,
                        "RCK Hsu mix Vxj aZc GmH adn a_A gpV hNT iyn BG7 gn8 L4E kVc", "", delayAfter: 5);

                // Click on username textbox              
                await BrowserWindows.Last().BrowserActAsync(ActType.Click, AttributeType.Name, "id", "", delayAfter: 0.5);

                // Enter account's username in username textbox
                await BrowserWindows.Last().EnterCharsAsync(_account.AccountBaseModel.UserName, delayBefore: 1, delayAtLast: 2);

                // Press Tab button now
                await BrowserWindows.Last().PressAnyKeyUpdated(9, delayAtLast: 1);

                // Click on password textbox
                await BrowserWindows.Last().BrowserActAsync(ActType.Click, AttributeType.Name, "password", "", delayAfter: 0.5);

                // Enter account's password in password textbox
                await BrowserWindows.Last().EnterCharsAsync(" " + _account.AccountBaseModel.Password, delayBefore: 1, delayAtLast: 2);

                // Press Tab button now                
                await BrowserWindows.Last().PressAnyKeyUpdated(9, delayAtLast: 1);

                // Click on Login button                 
                await BrowserWindows.Last().ExecuteScriptAsync("document.querySelector('div[data-test-id=\"registerFormSubmitButton\"]>button').click();", delayInSec:5);
                VerifyingAccount = false;
            }

            if (!BrowserWindows.Last().IsLoggedIn)
            {
                var result = await BrowserWindows.Last().GetPageSourceAsync();
                if (!string.IsNullOrEmpty(result) && (result.Contains("\"isAuth\": true") || result.Contains("\"isAuth\":true")
                    || result.Contains("\\\"isAuth\\\":true")||result.Contains("\"isAuthenticated\":true")||!result.Contains("Login to see more"))
                    && await BrowserWindows.Last().SaveCookies(!result.Contains("Reset your password")))
                {
                    VerifyingAccount = false;
                    _account = BrowserWindows?.LastOrDefault()?.DominatorAccountModel ??_account;
                    sessionManager.AddOrUpdateSession(ref _account,true);
                    using (var globalDbOperation = new DbOperations(SocinatorInitialize.GetGlobalDatabase().GetSqlConnection()))
                    {
                        globalDbOperation.UpdateAccountDetails(_account);
                    }
                }
            }
        }

        public void AddNew(CancellationTokenSource cancellationToken, string openWithUrl = null,
            LoginType loginType = LoginType.AutomationLogin)
        {
            CurrentData = "";
            BrowserWindows.Add(null);
            var url = string.IsNullOrEmpty(openWithUrl) ? "https://www.pinterest.com" : openWithUrl;
            BrowserOpenDispatcher(cancellationToken, loginType, url, true);
        }

        public void CloseLast(bool getCookies = false)
        {
            try
            {
                if (BrowserWindows.LastOrDefault() == null)
                    return;
                if (getCookies)
                    BrowserDispatcher(BrowserFuncts.GetBrowserCookies, _account.CancellationSource);
                CurrentData = "";
                BrowserDispatcher(BrowserFuncts.Close, _account.CancellationSource);
                BrowserWindows.RemoveAt(BrowserWindows.Count - 1);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        async Task LoadUrlUntillGetExact(string url, CancellationTokenSource cancellationToken, string conditionalString = "pinterest")
        {
            BrowserWindows.Last().GoToUrl(url);
            await Task.Delay(2000);
            await WaitUntillGetExact(cancellationToken, conditionalString);
        }

        async Task WaitUntillGetExact(CancellationTokenSource cancellationToken, string conditionalString = "pinterest")
        {
            var last2Min = DateTime.Now.AddMinutes(2);
            CurrentData = "";
            var leng = 0;
            while (last2Min >= DateTime.Now)
            {
                try
                {
                    cancellationToken.Token.ThrowIfCancellationRequested();
                    await Task.Delay(3000, cancellationToken.Token);
                    CurrentData = await BrowserWindows.Last().GetPageSourceAsync();
                    if (CurrentData.Length>1000&&CurrentData.Contains(conditionalString))
                        break;
                    while (last2Min > DateTime.Now && CurrentData.Contains("<html><head></head><body></body></html>"))
                    {
                        await Task.Delay(3000, cancellationToken.Token);
                        CurrentData = await BrowserWindows.Last().GetPageSourceAsync();
                    }
                    leng = CurrentData.Length;
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
        }

        public void BrowserOpenDispatcher(CancellationTokenSource cancellationToken,
            LoginType loginType = LoginType.AutomationLogin, params object[] paramS)
        {
            cancellationToken.Token.ThrowIfCancellationRequested();

            bool isRunning = true;
            Application.Current.Dispatcher.Invoke(async () =>
            {
                if (BrowserWindows.Last() == null)
                {
                    var url = paramS.Length > 0 ? paramS[0].ToString() : "https://www.pinterest.com";
                    var isCustom = paramS.Length > 1 ? (bool)paramS[1] : false;

                    var visibility = loginType == LoginType.BrowserLogin ? Visibility.Visible : Visibility.Hidden;
#if DEBUG
                    visibility = Visibility.Visible;
#endif

                    BrowserWindows[BrowserWindows.Count - 1] = new BrowserWindow(_account, cancellationToken.Token, url, isCustom, isNeedResourceData: true)
                    { Visibility = visibility };

                    await BrowserWindows.Last().SetCookie();

                    if (BrowserWindows.Count > 1)
                        BrowserWindows.Last().IsLoggedIn = true;
                    BrowserWindows.Last().Show();

                    BrowserWindows.Last().Visibility = visibility;


//                    #region Code for Run Through Puppeteer
//                    var HeadLess = true;
//#if DEBUG
//                    HeadLess = false;
//#endif
//                    if (loginType == LoginType.BrowserLogin)
//                        HeadLess = false;
//                    BrowserWindows[BrowserWindows.Count - 1] = new PuppeteerBrowserActivity(_account, isNeedResourceData: true, loginType: loginType);
//                    var BrowserLaunched = await BrowserWindows[BrowserWindows.Count - 1].LaunchBrowserAsync(HeadLess);
//                    #endregion
                }
                isRunning = false;
            });

            while (isRunning)
            {
                cancellationToken.Token.ThrowIfCancellationRequested();
                Thread.Sleep(TimeSpan.FromSeconds(1.5));
            }
        }

        public JavascriptResponse BrowserDispatcher(BrowserFuncts function, CancellationTokenSource cancellationToken, double delayBefore = 0, params object[] paramS)
        {
            JavascriptResponse response = null;
            cancellationToken.Token.ThrowIfCancellationRequested();

            if (function != BrowserFuncts.Close)
                Thread.Sleep(TimeSpan.FromSeconds(delayBefore));

            cancellationToken.Token.ThrowIfCancellationRequested();

            bool isRunning = true;
            Application.Current.Dispatcher.Invoke(async () =>
           {
                try
                {
                    switch (function)
                    {
                        case BrowserFuncts.GoToUrl:
                            var conditionalString = paramS.Length > 1 ? paramS[1].ToString() : "pinterest";
                            await LoadUrlUntillGetExact(paramS[0].ToString(), cancellationToken, conditionalString);
                            break;

                        case BrowserFuncts.EnterChars:
                            var chars = paramS[0].ToString();
                            double typingDelay = paramS.Length > 1 ? (double)paramS[1] : 0.09;
                            await BrowserWindows.Last().EnterCharsAsync(chars, typingDelay);
                            break;

                        case BrowserFuncts.MouseClick:
                            var developerScreenWidth = paramS.Length > 2 ? (int)paramS[2] : 1366;
                            var widthRatio = (double)PdStatic.ScreenResolution.Width / (double)developerScreenWidth;
                            var developerScreenHeight = paramS.Length > 3 ? (int)paramS[3] : 768;
                            var heightRatio = (double)PdStatic.ScreenResolution.Height / (double)developerScreenHeight;

                            int xLoc = (int)((int)paramS[0] * widthRatio);
                            var yLoc = (int)((int)paramS[1] * heightRatio);
                            await BrowserWindows.Last().MouseClickAsync(xLoc, yLoc);
                            break;

                        case BrowserFuncts.PressKey:
                            var winKeyCode = (int)paramS[0];
                            var nTimes = paramS.Length > 1 ? (int)paramS[1] : 1;
                            var delayBetween = paramS.Length > 2 ? (int)paramS[2] : 90;
                            await BrowserWindows.Last().PressAnyKeyUpdated(winKeyCode, nTimes, delayBetween, 3);
                            break;

                        case BrowserFuncts.GetPageSource:
                            var conditionalString1 = paramS.Length > 0 ? paramS[0].ToString() : "pinterest";
                            await WaitUntillGetExact(cancellationToken, conditionalString1);
                            break;

                        case BrowserFuncts.GetCurrentUrl:
                            CurrentData = BrowserWindows.Last().CurrentUrl();
                            break;

                        case BrowserFuncts.GetPageText:
                            CurrentData = await BrowserWindows.Last().PageText();
                            break;

                        case BrowserFuncts.GetPaginationData:
                            var startSearchText = paramS[0].ToString();
                            var isContains = paramS.Length > 1 ? (bool)paramS[1] : false;
                            CurrentData = await BrowserWindows.Last().GetPaginationData(startSearchText, isContains);
                            break;

                        case BrowserFuncts.BrowserAct:
                            var actType = (ActType)paramS[0];
                            var attributeType = (AttributeType)paramS[1];
                            var attributeValue = paramS.Length > 2 ? paramS[2].ToString() : "";
                            var value = paramS.Length > 3 ? paramS[3].ToString() : "";
                            int clickIndex = paramS.Length > 4 ? (int)paramS[4] : 0;
                            int scrollByPixel = paramS.Length > 5 ? (int)paramS[5] : 100;
                            await BrowserWindows.Last().BrowserActAsync(actType, attributeType, attributeValue, value, 0, 0, clickIndex, scrollByPixel);
                            break;

                        case BrowserFuncts.Scroll:
                            int scrollByPixel1 = (int)paramS[0];
                            int delayAfter = (int)paramS[1];
                            await BrowserWindows.Last().BrowserActAsync(ActType.ScrollWindow, AttributeType.Null, "", delayBefore: delayBefore,
                                delayAfter: delayAfter, scrollByPixel: scrollByPixel1);
                            break;

                        case BrowserFuncts.GetElementValue:
                            var actType1 = (ActType)paramS[0];
                            var attributeType1 = (AttributeType)paramS[1];
                            var attributeValue1 = paramS.Length > 2 ? paramS[2].ToString() : "";
                            var valueType = paramS.Length > 3 ? (ValueTypes)paramS[3] : ValueTypes.InnerHtml;
                            int clickIndex1 = paramS.Length > 4 ? (int)paramS[4] : 0;
                            CurrentData = (await BrowserWindows.Last().GetElementValueAsync(actType1, attributeType1, attributeValue1, valueType, 0, clickIndex1)) ?? "";
                            //"Play (k)"
                            break;

                        case BrowserFuncts.GetXY:
                            var attribute = (AttributeType)paramS[0];
                            var elementName = paramS[1].ToString();
                            int index = paramS.Length > 2 ? (int)paramS[2] : 0;
                            _lastXandY = BrowserWindows.Last().GetXAndY(attribute, elementName, index);
                            break;

                        case BrowserFuncts.GoBack:
                            var nTimesGoBack = paramS.Length > 0 ? (int)paramS[0] : 1;
                            BrowserWindows.Last().GoBack(nTimesGoBack);
                            break;

                        case BrowserFuncts.RefreshPage:
                            BrowserWindows.Last().Refresh();
                            break;

                        case BrowserFuncts.GetBrowserCookies:
                            _account.Cookies = await BrowserWindows.Last().BrowserCookiesIntoModel();
                            break;

                        case BrowserFuncts.Close:
                            BrowserWindows.Last().Close();
                            break;

                        case BrowserFuncts.ClickByAttributeName:
                            actType = (ActType)paramS[0];
                            attributeType = (AttributeType)paramS[1];
                            attributeValue = paramS.Length > 2 ? paramS[2].ToString() : "";
                            valueType = paramS.Length > 3 ? (ValueTypes)paramS[3] : ValueTypes.InnerHtml;
                            var lstInnerText = await BrowserWindows.Last().GetListInnerHtml(actType, attributeType,
                            attributeValue, valueType);
                            lstInnerText.Reverse();
                            string attributeName = paramS.Length > 4 ? (string)paramS[4] : "";
                            if (lstInnerText.Any(x => x != null && x.Contains(attributeName)))
                            {
                                int indexItr = paramS.Length > 5 ? (int)paramS[5] : 0;
                                string attributeContent = lstInnerText.FirstOrDefault(x => x != null && x.Contains(attributeName));
                                index = lstInnerText.IndexOf(attributeContent);
                                while (indexItr-- != 0)
                                    index = lstInnerText.IndexOf(attributeContent, index + 1);

                                await BrowserWindows.Last().BrowserActAsync(ActType.Click, attributeType, attributeValue, "", 0, 0, index);
                            }
                            int delay = paramS.Length > 6 ? (int)paramS[6] : 2000;
                            await Task.Delay(delay);
                            break;
                       case BrowserFuncts.ExecuteCustomScript:
                           var Script = (string)paramS[0];
                           delay = paramS.Length > 1 ? (int)paramS[1] : 3;
                           response = await BrowserWindows.Last().ExecuteScriptAsync(script: Script, delayInSec: delay);
                           await Task.Delay(delay*1000);
                           break;
                    }
                }
                catch
                {
                    // ignored
                }

                isRunning = false;
            });

            double waitTime = 0;
            while (isRunning && waitTime <= 150)
            {
                cancellationToken.Token.ThrowIfCancellationRequested();
                DelayBeforeOperation(1500);
                waitTime += 1.5;
            }
            return response;
        }

        private IResponseParameter Response => new ResponseParameter { Response = CurrentData };
        private KeyValuePair<int, int> _lastXandY;

        #endregion

        #region Pinterest Functions

        public IResponseParameter HitMainPage(string url, CancellationTokenSource cancellationToken)
        {
            BrowserDispatcher(BrowserFuncts.GoToUrl, cancellationToken, 2, url, ".pinterest.");
            return Response;
        }

        #endregion

        public List<PinterestPin> SearchPinsByKeyword(DominatorAccountModel account, string keyWord, CancellationTokenSource cancellationToken,
            bool isScroll = false, int scroll = 10)
        {
            var lstOfPin = new List<PinterestPin>();
            try
            {
                if (!isScroll)
                {
                    var key = Uri.EscapeDataString(keyWord);
                    string url = $"https://{Domain}/search/pins/?q=" + key;
                    BrowserDispatcher(BrowserFuncts.GoToUrl, cancellationToken, 0, url, account.AccountBaseModel.ProfileId);
                    lstOfPin.AddRange(new SearchAllPinResponseHandler(new ResponseParameter { Response = PdRequestHeaderDetails.GetJsonString(CurrentData,true) }).LstPin);
                }

                if (scroll > 0)
                {
                    var responseList = ScrollWindowAndGetData(account, scroll, "\"name\":\"BaseSearchResource\",", cancellationToken);
                    foreach (string response in responseList)
                        lstOfPin.AddRange(new SearchAllPinResponseHandler(new ResponseParameter { Response = response }).LstPin);
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
            return lstOfPin;
        }

        public List<PinterestUser> GetUserFollowings(DominatorAccountModel account, string user, CancellationTokenSource cancellationToken,
            bool isScroll = false, int scroll = 10)
        {
            List<PinterestUser> lstPinterestUser = new List<PinterestUser>();
            try
            {
                string userUrl = user;
                if (!user.Contains("https:"))
                    userUrl = $"https://{Domain}" + "/" + user + "/_saved";
                else
                    userUrl = $"https://{Domain}" + "/" + user.Split('/')[3] + "/_saved";

                user = userUrl.Split('/')[3];
                BrowserWindows.Last().ClearResources();

                if (!isScroll)
                {
                    BrowserDispatcher(BrowserFuncts.GoToUrl, cancellationToken, 0, userUrl, user);
                    DelayBeforeOperation(5000);
                    BrowserDispatcher(BrowserFuncts.ExecuteCustomScript, cancellationToken, 1, "document.querySelector('[data-test-id=\"profile-following-count\"]').childNodes[0].click();", 2);
                    DelayBeforeOperation(3000);
                    var json = BrowserWindows.LastOrDefault().GetPaginationData("\"endpoint_name\":\"v3_get_user_following_handler\"", true).Result;
                    lstPinterestUser.AddRange(new FollowerAndFollowingPtResponseHandler(new ResponseParameter { Response = json }, account.AccountBaseModel.ProfileId, user).UsersList);
                }

                if (scroll > 0)
                {
                    var responseList = ScrollWindowAndGetData(account, scroll,
                        "\"path\":\"/resource/UserFollowingResource/get/\"", cancellationToken);

                    foreach (string response in responseList)
                    {
                        lstPinterestUser.AddRange(new FollowerAndFollowingPtResponseHandler(new ResponseParameter { Response = response }, account.AccountBaseModel.ProfileId).UsersList);
                    }
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
            return lstPinterestUser;
        }

        public List<PinterestUser> GetUserFollowers(DominatorAccountModel account, string user, CancellationTokenSource cancellationToken,
          bool isScroll = false, int scroll = 10)
        {
            List<PinterestUser> lstPinterestUser = new List<PinterestUser>();
            try
            {
                string userUrl = user;
                if (string.IsNullOrEmpty(user))
                    user = account.AccountBaseModel.UserFullName;
                if (!user.Contains("https:"))
                    userUrl = $"https://{Domain}" + "/" + user + PdConstants.Followers;
                else
                    userUrl = $"https://{Domain}" + "/" + user.Split('/')[3] + PdConstants.Followers;

                user = userUrl.Split('/')[3];
                BrowserWindows.Last().ClearResources();

                if (!isScroll)
                {
                    userUrl = $"https://in.pinterest.com/{user}/_saved/";
                    BrowserDispatcher(BrowserFuncts.GoToUrl, cancellationToken, 0, userUrl, user);
                    Application.Current.Dispatcher.Invoke(async () =>
                    {
                        await BrowserWindows.Last().BrowserActAsync(ActType.ScrollWindow, AttributeType.Null, "",
                            delayAfter: 2, scrollByPixel: 200);
                    });
                    DelayBeforeOperation(5000);
                    BrowserDispatcher(BrowserFuncts.ExecuteCustomScript, cancellationToken, 1, "document.querySelector('[data-test-id=\"profile-followers-count\"]').childNodes[0].click();", 2);
                    DelayBeforeOperation(3000);
                    var json = BrowserWindows.LastOrDefault().GetPaginationData("\"__typename\":\"V3GetUserHandler\",\"data\":{\"followers\":", true).Result;
                    lstPinterestUser.AddRange(new FollowerAndFollowingPtResponseHandler(new ResponseParameter { Response = json }, account.AccountBaseModel.ProfileId, user).UsersList);
                }

                if (scroll > 0)
                {
                    var responseList = ScrollWindowAndGetData(account, scroll,
                        "\"__typename\":\"V3GetUserHandler\",\"data\":{\"followers\":", cancellationToken);

                    foreach (string response in responseList)
                    {
                        lstPinterestUser.AddRange(new FollowerAndFollowingPtResponseHandler(new ResponseParameter { Response = response }, account.AccountBaseModel.ProfileId).UsersList);
                    }
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
            return lstPinterestUser;
        }

        public List<PinterestUser> GetBoardFollowers(DominatorAccountModel account, string boardUrl, CancellationTokenSource cancellationToken,
         bool isScroll = false, int scroll = 10)
        {
            List<PinterestUser> lstPinterestUser = new List<PinterestUser>();
            try
            {
                BrowserWindows.Last().ClearResources();

                if (!isScroll)
                {
                    retry:
                    BrowserDispatcher(BrowserFuncts.GoToUrl, cancellationToken, 0, boardUrl, "MIw QLY Rym ojN p6V zI7 iyn Hsu");
                    DelayBeforeOperation(2000);
                    List<string> lstInnerText = new List<string>();
                    bool isClicked = false;
                    Application.Current.Dispatcher.Invoke(async () =>
                    {
                        lstInnerText = await BrowserWindows.Last().GetListInnerHtml(ActType.GetValue, AttributeType.ClassName,
                            "tBJ dyH iFc sAJ O2T tg7 IZT H2s", ValueTypes.InnerHtml);
                        isClicked = true;
                    });
                    DelayBeforeOperation(1000);
                    int count = 0;
                    while (!isClicked && count < 20)
                    {
                        count++;
                        Thread.Sleep(2000);
                    }
                    lstInnerText.Reverse();
                    if (lstInnerText.Count <= 0)
                    {
                        BrowserWindows.Last().Refresh();
                        goto retry;
                    }
                    int index = lstInnerText.IndexOf(lstInnerText.FirstOrDefault(x => x.Contains("followers")));

                    BrowserDispatcher(BrowserFuncts.BrowserAct, cancellationToken, 2, ActType.Click, AttributeType.ClassName,
                        "tBJ dyH iFc sAJ O2T tg7 IZT H2s", "", index);
                    DelayBeforeOperation(2000);
                    List<string> lstResponse = new List<string>();

                    for (int i = 0; i < 5; i++)
                    {
                        lstResponse = BrowserWindows.Last().GetPaginationDataList("\"path\":\"/resource/BoardFollowersResource/get/\"", true).Result;
                        lstResponse = lstResponse.Count > 0 ? lstResponse : BrowserWindows.Last().GetPaginationDataList("v3_get_board_followers", true).Result;
                        if (lstResponse.Count > 0)
                            break;
                        DelayBeforeOperation(3000);
                    }
                    lstResponse.ForEach(userList =>{lstPinterestUser.AddRange(new FollowerAndFollowingPtResponseHandler(new ResponseParameter { Response = userList }, account.AccountBaseModel.ProfileId).UsersList);});
                }

                if (scroll > 0)
                {
                    List<string> responseList = ScrollWindowAndGetData(account, scroll,
                        "\"path\":\"/resource/BoardFollowersResource/get/\"", cancellationToken, isClickSeeMore: true);
                    responseList = responseList.Count > 0 ? responseList : ScrollWindowAndGetData(account, scroll, "v3_get_board_followers", cancellationToken, isClickSeeMore: true);
                    responseList.ForEach(userList => { lstPinterestUser.AddRange(new FollowerAndFollowingPtResponseHandler(new ResponseParameter { Response = userList }, account.AccountBaseModel.ProfileId).UsersList); });
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
            return lstPinterestUser;
        }

        public BoardInfoPtResponseHandler SearchByCustomBoard(DominatorAccountModel account, string boardUrl,
            CancellationTokenSource cancellationToken)
        {
            if (!boardUrl.Contains("http"))
            {
                boardUrl = boardUrl.Remove(0, 1).Insert(0, "");
                // boardUrl = $"https://{Domain}" +"/"+account.AccountBaseModel.ProfileId+"/"+ boardUrl+"/";
                boardUrl = $"https://{Domain}{"/"}{boardUrl}";
            }
            
            BrowserDispatcher(BrowserFuncts.GoToUrl, cancellationToken, 2, boardUrl, "BoardResource");
            BoardInfoPtResponseHandler boardInfo = new BoardInfoPtResponseHandler(new ResponseParameter
            { Response = CurrentData });
            return boardInfo;
        }

        public List<PinterestBoard> SearchBoardsOfUser(DominatorAccountModel account, string user, CancellationTokenSource cancellationToken,
            bool isScroll = false, int scroll = 10)
        {
            List<PinterestBoard> lstPinterestBoards = new List<PinterestBoard>();
            try
            {
                if (!isScroll)
                {
                    string userUrl = user;
                    if (!user.Contains("https:"))
                        userUrl = $"https://{Domain}" + "/" + user;
                    else
                        userUrl = $"https://{Domain}" + "/" + user.Split('/')[3];
                    if (account.DisplayColumnValue11.Equals("Business Mode") || account.AccountBaseModel.ProfileId!=user)
                        userUrl = userUrl + "/_saved/";

                    user = userUrl.Split('/')[3];
                    BrowserWindows.Last().ClearResources();
                    BrowserDispatcher(BrowserFuncts.GoToUrl, cancellationToken, 0, userUrl, user);

                    if (account.AccountBaseModel.PinterestAccountType.ToString().Equals("Active"))
                        BrowserDispatcher(BrowserFuncts.BrowserAct, cancellationToken, 1, ActType.Click, AttributeType.ClassName,
                            "Lfz fZz hUC zI7 iyn Hsu", 0, 2);

                    string jsonResponse = Utilities.GetBetween(CurrentData, PdConstants.ApplicationJsonDoubleInitialStateSingle, PdConstants.Script);
                    if (string.IsNullOrEmpty(jsonResponse))
                        jsonResponse = Utilities.GetBetween(CurrentData, PdConstants.ApplicationJsonDoubleInitialStateDouble, PdConstants.Script);
                    if (string.IsNullOrEmpty(jsonResponse))
                        jsonResponse = Utilities.GetBetween(CurrentData, PdConstants.InitialStateDoubleApplicationJsonDouble, PdConstants.Script);

                    lstPinterestBoards.AddRange(new BoardsOfUserResponseHandler(new ResponseParameter { Response = jsonResponse }).BoardsList);
                    string Response = BrowserWindows.Last().GetPaginationDataList("\"path\":\"/resource/BoardsResource/get/\"", true).Result.Last();
                    lstPinterestBoards.AddRange(new BoardsOfUserResponseHandler(new ResponseParameter { Response = Response }).BoardsList);
                }
                if (scroll > 0)
                {
                    List<string> responseList = ScrollWindowAndGetData(account, scroll,
                        "\"path\":\"/resource/BoardsResource/get/\"", cancellationToken);

                    foreach (string response in responseList)
                    {
                        lstPinterestBoards.AddRange(new BoardsOfUserResponseHandler(new ResponseParameter { Response = response }).BoardsList);
                    }
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
            return lstPinterestBoards;
        }

        public PinInfoPtResponseHandler SearchByCustomPin(DominatorAccountModel account, string pin, CancellationTokenSource cancellationToken)
        {
            try
            {
                string pinUrl = string.Empty;
                if (!pin.Contains("pinterest"))
                    pinUrl = $"https://{Domain}/pin/{pin}/";
                else
                    pinUrl = $"https://{Domain}/pin/{pin.Split('/')[4]}/";

                pin = pinUrl.Split('/')[4];
                BrowserDispatcher(BrowserFuncts.GoToUrl, cancellationToken, 0, pinUrl, pin);
                PinInfoPtResponseHandler pinInfo = new PinInfoPtResponseHandler(new ResponseParameter
                { Response = CurrentData }, pin);
                return pinInfo;
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return new PinInfoPtResponseHandler(new ResponseParameter
                { Response = CurrentData }, "")
                { Success = false };
            }
        }

        public UserNameInfoPtResponseHandler SearchByCustomUser(DominatorAccountModel account, string user, CancellationTokenSource cancellationToken)
        {
            string userUrl = user;
            if (!user.Contains("https:"))
                userUrl = $"https://{Domain}" + "/" + user; //+ "/_saved";
            else
                userUrl = $"https://{Domain}" + "/" + user.Split('/')[3] + "/_saved";

            user = userUrl.Split('/')[3];
            BrowserDispatcher(BrowserFuncts.GoToUrl, cancellationToken, 0, userUrl, user);
            DelayBeforeOperation(5000);
            UserNameInfoPtResponseHandler userInfo = new UserNameInfoPtResponseHandler(new ResponseParameter
            { Response = CurrentData });
            return userInfo;
        }

        public List<string> ScrollWindowAndGetData(DominatorAccountModel account, int noOfPageToScroll,
            string conditonString, CancellationTokenSource token, bool isIncreaseScrollAfter1Page = false,
            int lastPageNo = 0, bool isClickSeeMore = false)
        {
            List<string> itemList = new List<string>();
            bool isScraped = false;

            Application.Current.Dispatcher.Invoke(async () =>
            {
                await Task.Delay(3000);
                while (lastPageNo < noOfPageToScroll)
                {
                    try
                    {
                        await Task.Delay(2000);
                        BrowserWindows.Last().ClearResources();
                        lastPageNo++;
                        if (isClickSeeMore)
                            BrowserDispatcher(BrowserFuncts.BrowserAct, token, 2, ActType.Click, AttributeType.ClassName,
                                "moreItems Button Module btn hasText rounded");
                        else if (isIncreaseScrollAfter1Page && lastPageNo > 1)
                            await BrowserWindows.Last().BrowserActAsync(ActType.ScrollWindow, AttributeType.Null,
                                HtmlTags.Window, scrollByPixel: 8000, delayAfter: 5);
                        else
                            await BrowserWindows.Last().BrowserActAsync(ActType.ScrollWindow, AttributeType.Null,
                                HtmlTags.Window, scrollByPixel: 5000, delayAfter: 5);

                        List<string> lstResponse = new List<string>();

                        for (int i = 0; i < 5; i++)
                        {
                            token.Token.ThrowIfCancellationRequested();
                            await Task.Delay(2000);
                            lstResponse = await BrowserWindows.Last().GetPaginationDataList(conditonString, true);
                            if (lstResponse.Count >= 2)
                                break;
                        }
                        if (lstResponse.Count == 0)
                            break;
                        itemList.AddRange(lstResponse);
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

                await Task.Delay(1000);
                isScraped = true;
            });

            while (!isScraped)
            {
                DelayBeforeOperation(2000);
            }
            return itemList;
        }

        public BoardResponse FollowBoard(DominatorAccountModel account, string boardUrl, CancellationTokenSource cancellationToken)
        {
            var BoardUrl = $"https://{Domain}" + boardUrl;
            BrowserDispatcher(BrowserFuncts.GoToUrl, cancellationToken, 0, BoardUrl);
            DelayBeforeOperation(5000);
            try
            {
                var followBoardPageSource = BrowserWindows.Last().GetPageSource();
                BrowserDispatcher(BrowserFuncts.ExecuteCustomScript, cancellationToken, 2,string.Format(PDClassesConstant.ScriptConstant.ScriptWithQuerySelectorToClick,HtmlTags.Button,HtmlTags.HtmlAttribute.AriaLabel, "Other actions"), 2);
                DelayBeforeOperation(3000);
                var OptionItemNodes = HtmlAgilityHelper.GetListInnerHtmlOrInnerTextOrOuterHtmlFromIdOrClass(response:BrowserWindows.Last().GetPageSource(),Id:"actionBarMenuButton-item",getInnerText:true);
                var BoardFollowIndex = OptionItemNodes.IndexOf(OptionItemNodes.FirstOrDefault(x=>string.Equals(x,"Follow")));
                BrowserDispatcher(BrowserFuncts.BrowserAct, cancellationToken, 2, ActType.ClickById, AttributeType.Id,$"actionBarMenuButton-item-{BoardFollowIndex}");
                DelayBeforeOperation(3000);
                followBoardPageSource = BrowserWindows.Last().GetPageSource();
                return new BoardResponse(new ResponseParameter { Response = string.Empty }) { Success = true };
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception)
            {
                return new BoardResponse(new ResponseParameter { Response = string.Empty }) { Success = false };
            }
        }

        public FriendshipsResponse FollowUnfollowUser(DominatorAccountModel account, string user, bool isFollow,
            CancellationTokenSource cancellationToken)
        {
            string userUrl = user;
            if (!user.Contains("https:"))
                userUrl = $"https://{Domain}" + "/" + user;
            else
                userUrl = $"https://{Domain}" + "/" + user.Split('/')[3];

            user = userUrl.Split('/')[3];
            cancellationToken.Token.ThrowIfCancellationRequested();

            BrowserDispatcher(BrowserFuncts.GoToUrl, cancellationToken, 0, userUrl, "Jea XiG zI7 iyn Hsu");

            bool isBusiness = string.IsNullOrEmpty(BrowserWindows.Last().GetElementValueAsync(ActType.GetValue,
                       AttributeType.ClassName, "BusinessProfileTabBar").Result) ? false : true;
            string pageSource = BrowserWindows.Last().GetPageSource();
            if (!pageSource.Contains(account.AccountBaseModel.ProfileId))
            {
                BrowserWindows.Last().Refresh();
                DelayBeforeOperation(3000);
            }

            pageSource = BrowserWindows.Last().GetPageSource();
            string followUnfollowClass;

            if (!isBusiness && isFollow)
                followUnfollowClass = BrowserUtilities.GetPath(pageSource,HtmlTags.Button, "Follow");
            else if (!isBusiness && !isFollow)
                followUnfollowClass = BrowserUtilities.GetPath(pageSource,HtmlTags.Button, "Following");
            else if (isBusiness && isFollow)
                followUnfollowClass = "tBJ dyH iFc SMy _S5 erh DrD IZT mWe";
            else
                followUnfollowClass = "tBJ dyH iFc SMy _S5 pBj DrD IZT mWe";
            int failedCount = 0;
            while (failedCount++ < 4 && !CurrentData.Contains(followUnfollowClass))
                BrowserDispatcher(BrowserFuncts.GoToUrl, cancellationToken, 0, userUrl, followUnfollowClass);
            if (!CurrentData.Contains(followUnfollowClass))
                return new FriendshipsResponse(new ResponseParameter { Response = string.Empty }, "")
                {
                    Success = false,
                    Issue = new PinterestIssue
                    { Message = "LangKeyCheckInternet".FromResourceDictionary() }
                };
            try
            {
                string followButtonContent = string.Empty;

                if (isBusiness && isFollow)
                    followButtonContent = BrowserWindows.Last().GetElementValueAsync(ActType.GetValue,
                        AttributeType.ClassName, followUnfollowClass).Result;
                else if (isBusiness && !isFollow)
                    followButtonContent = BrowserWindows.Last().GetElementValueAsync(ActType.GetValue,
                        AttributeType.ClassName, followUnfollowClass, clickIndex: 1).Result;
                else if (!isBusiness && isFollow)
                    followButtonContent = BrowserWindows.Last().GetElementValueAsync(ActType.GetValue,
                        AttributeType.ClassName, followUnfollowClass).Result;
                else if (!isBusiness && !isFollow)
                    followButtonContent = BrowserWindows.Last().GetElementValueAsync(ActType.GetValue,
                        AttributeType.ClassName, followUnfollowClass, clickIndex: 1).Result;

                failedCount = 0;
                BrowserWindows.Last().ClearResources();
                while (failedCount++ < 5 && ((isFollow && followButtonContent.Contains("Follow")) ||
                    (!isFollow && followButtonContent.Contains("Following"))))
                {
                    cancellationToken.Token.ThrowIfCancellationRequested();
                    if (isBusiness)
                        BrowserDispatcher(BrowserFuncts.BrowserAct, cancellationToken, 2, ActType.Click,
                        AttributeType.ClassName, "rLK iyn DI9 BG7 Xs7 gL3 FTD L4E");
                    else if (!isBusiness && isFollow)
                        BrowserDispatcher(BrowserFuncts.BrowserAct, cancellationToken, 2, ActType.Click,
                        AttributeType.ClassName, followUnfollowClass); //RCK Hsu Vxj aZc GmH adn Il7 Jrn hNT iyn BG7 NTm KhY jJP
                    else if (!isBusiness && !isFollow)
                        BrowserDispatcher(BrowserFuncts.BrowserAct, cancellationToken, 2, ActType.Click,
                        AttributeType.ClassName, followUnfollowClass, "", 1);

                    DelayBeforeOperation(2000);

                    if (isBusiness && isFollow)
                        followButtonContent = BrowserWindows.Last().GetElementValueAsync(ActType.GetValue,
                            AttributeType.ClassName, followUnfollowClass).Result;
                    else if (isBusiness && !isFollow)
                        followButtonContent = BrowserWindows.Last().GetElementValueAsync(ActType.GetValue,
                            AttributeType.ClassName, followUnfollowClass, clickIndex: 1).Result;
                    else if (!isBusiness && isFollow)
                        followButtonContent = BrowserWindows.Last().GetElementValueAsync(ActType.GetValue,
                            AttributeType.ClassName, followUnfollowClass).Result;
                    else if (!isBusiness && !isFollow)
                        followButtonContent = BrowserWindows.Last().GetElementValueAsync(ActType.GetValue,
                            AttributeType.ClassName, followUnfollowClass, clickIndex: 1).Result;
                }

                string response = BrowserWindows.Last().GetPaginationData("\"resource\":{\"name\":\"UserFollowResource\"", true).Result;
                string userId = Utilities.GetBetween(response, "\"user_id\":\"", "\"");

                return new FriendshipsResponse(new ResponseParameter { Response = response }, userId) { Success = true };
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return new FriendshipsResponse(new ResponseParameter { Response = string.Empty }, "") { Success = false };
            }
        }

        public FriendshipsResponse FollowUser(DominatorAccountModel account, string user, CancellationTokenSource cancellationToken)
        {
            string response = string.Empty;
            int count = 0;
            while (string.IsNullOrEmpty(response) && count++ < 3)
            {
                string userUrl = user;
                if (!user.Contains("https:"))
                    userUrl = $"https://{Domain}" + "/" + user;
                else
                    userUrl = $"https://{Domain}" + "/" + user.Split('/')[3];

                user = userUrl.Split('/')[3];
                cancellationToken.Token.ThrowIfCancellationRequested();

                BrowserDispatcher(BrowserFuncts.GoToUrl, cancellationToken, 0, userUrl, "user-follow-button");
                string pageSource = BrowserWindows.Last().GetPageSource();

                int i = 0;
                while (!pageSource.Contains(account.AccountBaseModel.ProfileId) && i++ <= 5)
                {
                    BrowserWindows.Last().Refresh();
                    DelayBeforeOperation(3000);
                    pageSource = BrowserWindows.Last().GetPageSource();
                }
                Application.Current.Dispatcher.Invoke(async () =>
                {
                    await BrowserWindows.Last().BrowserActAsync(ActType.ScrollWindow, AttributeType.Null, "",
                        delayAfter: 2, scrollByPixel: 200);

                });
                DelayBeforeOperation(5000);
                BrowserDispatcher(BrowserFuncts.ExecuteCustomScript, cancellationToken, 0,string.Format(PDClassesConstant.ScriptConstant.ScriptWithQuerySelectorAllToFilterNormal, $"{HtmlTags.Button} {HtmlTags.Div} {HtmlTags.Div}","Follow","0","click();"),2);
                DelayBeforeOperation(3000);

                response = BrowserWindows.Last().GetPaginationData("\"resource\":{\"name\":\"UserFollowResource\"", true).Result;
                int k = 0;
                while (string.IsNullOrEmpty(response) && k <= 4)
                {
                    response = BrowserWindows.Last().GetPaginationData("\"resource\":{\"name\":\"UserFollowResource\"", true).Result;
                    k++;
                }
            }

            string userId = Utilities.GetBetween(response, "\"user_id\":\"", "\"");

            return new FriendshipsResponse(new ResponseParameter { Response = response }, userId) { Success = true };
        }

        public FriendshipsResponse UnfollowUser(DominatorAccountModel account, string user, CancellationTokenSource cancellationToken)
        {
            string userUrl = user;
            if (!user.Contains("https:"))
                userUrl = $"https://{Domain}" + "/" + user;
            else
                userUrl = $"https://{Domain}" + "/" + user.Split('/')[3];

            user = userUrl.Split('/')[3];
            cancellationToken.Token.ThrowIfCancellationRequested();

            BrowserDispatcher(BrowserFuncts.GoToUrl, cancellationToken, 0, userUrl, "Jea XiG zI7 iyn Hsu");
            string pageSource = BrowserWindows.Last().GetPageSource();

            int i = 0;
            while (!pageSource.Contains(account.AccountBaseModel.ProfileId) && i++ <= 5)
            {
                BrowserWindows.Last().GoToCustomUrl("https://www.pinterest.com/login/", 2);
                BrowserWindows.Last().Refresh();
                Thread.Sleep(TimeSpan.FromSeconds(3));
                pageSource = BrowserWindows.Last().GetPageSource();
            }

            var unFollowAttribute = BrowserUtilities.GetPath(pageSource,HtmlTags.Button, "Following");

            int failedCount = 0;
            while (failedCount++ < 4 && !CurrentData.Contains(unFollowAttribute))
                BrowserDispatcher(BrowserFuncts.GoToUrl, cancellationToken, 0, userUrl, unFollowAttribute);

            if (!CurrentData.Contains(unFollowAttribute))
                return new FriendshipsResponse(new ResponseParameter { Response = string.Empty }, "")
                {
                    Success = false,
                    Issue = new PinterestIssue
                    { Message = "LangKeyCheckInternet".FromResourceDictionary() }
                };

            var chkBusinessPageSource = BrowserWindows.Last().GetPageSource();
            var isBusiness = false;
            var checkWithBusinessProfile = HtmlAgilityHelper.GetStringInnerTextFromClassName(chkBusinessPageSource, "tBJ dyH iFc MF7 B9u DrD mWe");
            if (!string.IsNullOrEmpty(chkBusinessPageSource) && checkWithBusinessProfile.Equals("Analytics")
                 || !string.IsNullOrEmpty(Utilities.GetBetween(chkBusinessPageSource, "business_name\":\"", "\",")))
                isBusiness = true;

            if (isBusiness)
                BrowserDispatcher(BrowserFuncts.BrowserAct, cancellationToken, 2, ActType.Click, AttributeType.ClassName, unFollowAttribute);
            else
                BrowserDispatcher(BrowserFuncts.BrowserAct, cancellationToken, 2, ActType.Click, AttributeType.ClassName, unFollowAttribute, 0, 1);
            BrowserDispatcher(BrowserFuncts.ExecuteCustomScript, cancellationToken, 0,string.Format(PDClassesConstant.ScriptConstant.ScriptWithQuerySelectorAllToFilterNormal,$"{HtmlTags.Button} {HtmlTags.Div} {HtmlTags.Div}","Following","0","click();"), 2);
            DelayBeforeOperation(3000);

            string response = BrowserWindows.Last().GetPaginationData("resource_response\":{\"status", true).Result;
            string userId = "Not getting user_id in unfollow";

            return new FriendshipsResponse(new ResponseParameter { Response = response }, userId) { Success = true };
        }

        public MessageResponseHandler Message(DominatorAccountModel account, string user, string msg,
            CancellationTokenSource cancellationToken)
        {
            string userUrl = user;
            if (!user.Contains("https:"))
                userUrl = $"https://{Domain}" + "/" + user;
            else
                userUrl = $"https://{Domain}" + "/" + user.Split('/')[3];

            user = userUrl.Split('/')[3];
            BrowserDispatcher(BrowserFuncts.GoToUrl, cancellationToken, 2, userUrl, "mostRecentBoard");
            DelayBeforeOperation(5000);
            try
            {

                if (CurrentData.Contains("show_error%3Dtrue"))
                    return new MessageResponseHandler(new ResponseParameter { Response = string.Empty })
                    {
                        Success = false,
                        Issue = new PinterestIssue { Message = "LangKeyUserNotExistMessage".FromResourceDictionary() }
                    };


                var pageSource = BrowserWindows.Last().GetPageSource();
                var IsClickedOnMessageButton = BrowserDispatcher(BrowserFuncts.ExecuteCustomScript, cancellationToken, 0,string.Format(PDClassesConstant.ScriptConstant.ScriptWithQuerySelectorAllToFilterNormal,$"{HtmlTags.Button} {HtmlTags.Div} {HtmlTags.Div}","Message","0","click();"), 2).Success;
                if (IsClickedOnMessageButton)
                {
                    BrowserDispatcher(BrowserFuncts.GetXY, cancellationToken, 3, AttributeType.Id, "message");
                    BrowserWindows.Last().MouseClick(Convert.ToInt32(_lastXandY.Key) + 5, Convert.ToInt32(_lastXandY.Value) + 5, delayAfter: 2);

                    BrowserDispatcher(BrowserFuncts.EnterChars, cancellationToken, 3, msg);

                    BrowserWindows.Last().ClearResources();
                    BrowserDispatcher(BrowserFuncts.ClickByAttributeName, cancellationToken, 2, ActType.GetValue,
                        AttributeType.TagName, HtmlTags.Button, ValueTypes.InnerText, "Send");
                    DelayBeforeOperation(2000);

                    string response = BrowserWindows.Last().GetPaginationData("\"resource\":{\"name\":\"ConversationsResource\"", true).Result;

                    return new MessageResponseHandler(new ResponseParameter { Response = response });
                }
                else if (BrowserDispatcher(BrowserFuncts.ExecuteCustomScript, cancellationToken, 0, string.Format(PDClassesConstant.ScriptConstant.ScriptWithQuerySelectorAllToClick, HtmlTags.Button, HtmlTags.HtmlAttribute.AriaLabel, "Other actions", "0"), 2).Success)
                {
                    int.TryParse( BrowserDispatcher(BrowserFuncts.ExecuteCustomScript, cancellationToken, 2, $"[...document.querySelectorAll('span[title=\"Message\"]')].filter(x=>x.textContent.includes(\"Message\")).length").Result.ToString(),out int messagebutton);
                    if (messagebutton > 0)
                    {
                        BrowserDispatcher(BrowserFuncts.ClickByAttributeName, cancellationToken, 2, ActType.GetValue,
                        AttributeType.TagName, HtmlTags.Span, ValueTypes.InnerText, "Message");
                        BrowserDispatcher(BrowserFuncts.GetXY, cancellationToken, 3, AttributeType.Id, "message");
                        BrowserWindows.Last().MouseClick(Convert.ToInt32(_lastXandY.Key) + 5, Convert.ToInt32(_lastXandY.Value) + 5, delayAfter: 2);

                        BrowserDispatcher(BrowserFuncts.EnterChars, cancellationToken, 3, msg);

                        BrowserWindows.Last().ClearResources();
                        BrowserDispatcher(BrowserFuncts.ClickByAttributeName, cancellationToken, 2, ActType.GetValue,
                            AttributeType.TagName, HtmlTags.Button, ValueTypes.InnerText, "Send");
                        DelayBeforeOperation(2000);

                        string response = BrowserWindows.Last().GetPaginationData("\"resource\":{\"name\":\"ConversationsResource\"", true).Result;

                        return new MessageResponseHandler(new ResponseParameter { Response = response });
                    }
                    else return new MessageResponseHandler(new ResponseParameter { Response= "Unable to send message because of recipient's message settings" });
                }
                else
                {
                    var username = BrowserDispatcher(BrowserFuncts.ExecuteCustomScript, cancellationToken, 2, $"document.querySelectorAll('div[data-test-id=\"profile-name\"]')[0].innerText").Result;
                    BrowserDispatcher(BrowserFuncts.ClickByAttributeName, cancellationToken, 2, ActType.GetAttribute,
                    AttributeType.TagName, HtmlTags.Div, ValueTypes.AriaLabel, "Messages");
                    DelayBeforeOperation(2000);

                    BrowserDispatcher(BrowserFuncts.ExecuteCustomScript, cancellationToken, 0, "[...document.querySelectorAll('div[role=\"button\"]')].filter(x=>x.textContent===\"New message\")[0].click()");
                    DelayBeforeOperation(2000);

                    pageSource = BrowserWindows.Last().GetPageSource();
                    BrowserDispatcher(BrowserFuncts.EnterChars, cancellationToken, 2, username);
                    DelayBeforeOperation(2000);

                    BrowserDispatcher(BrowserFuncts.ExecuteCustomScript, cancellationToken, 0, $"[...document.querySelectorAll('div[role=\"button\"]')].filter(x=>x.textContent.includes(\"{username}\"))[0].click()");
                    DelayBeforeOperation(2000);

                    int.TryParse(BrowserDispatcher(BrowserFuncts.ExecuteCustomScript, cancellationToken, 0, $"[...document.querySelectorAll('button[type=\"button\"]')].filter(x=>x.textContent.includes(\"Next\")).length").Result.ToString(),out int isPresentNextButton);
                    if (isPresentNextButton > 0)
                    {
                        BrowserDispatcher(BrowserFuncts.ExecuteCustomScript, cancellationToken, 0, $"[...document.querySelectorAll('button[type=\"button\"]')].filter(x=>x.textContent.includes(\"Next\"))[0].click()");
                        DelayBeforeOperation(3000);
                    }

                    BrowserDispatcher(BrowserFuncts.EnterChars, cancellationToken, 2, msg);
                    DelayBeforeOperation(2000);

                    BrowserDispatcher(BrowserFuncts.ClickByAttributeName, cancellationToken, 2, ActType.GetAttribute,
                        AttributeType.TagName, HtmlTags.Button, ValueTypes.AriaLabel, "Send message to conversation");

                    pageSource = BrowserWindows.Last().GetPageSource();
                    var attributeValueForSearchBox = BrowserUtilities.GetPath(pageSource, HtmlTags.Button, "Send message to conversation");

                    if (string.IsNullOrEmpty(attributeValueForSearchBox) && !pageSource.Contains("<html><head></head><body></body></html>"))
                        return new MessageResponseHandler(new ResponseParameter { Response = string.Empty }) { Success = true };
                    else
                        return new MessageResponseHandler(new ResponseParameter { Response = string.Empty }) { Success = false };
                }
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return new MessageResponseHandler(new ResponseParameter { Response = string.Empty }) { Success = false };
            }
        }

        public CommentResponse Comment(DominatorAccountModel account, string pinId, string comment,
            CancellationTokenSource token)
        {
            string pinUrl = string.Empty;
            if (!pinId.Contains("pinterest"))
                pinUrl = $"https://{Domain}/pin/{pinId}/";
            else
                pinUrl = $"https://{Domain}/pin/{pinId.Split('/')[4]}/";

            pinId = pinUrl.Split('/')[4];

            BrowserDispatcher(BrowserFuncts.GoToUrl, token, 0, pinUrl, pinId);

            var messageButtonContent = BrowserWindows.Last().GetElementValueAsync(ActType.GetValue,
                    AttributeType.ClassName, "Tbq iyn Hsu aZc _O1 qT6 undefined mQ8 adn Lfz e43 BG7 f-T YbY B9u").Result;

            if (messageButtonContent.Contains("Comments"))
            {
                bool isClicked = false;
                Application.Current.Dispatcher.Invoke(async () =>
                {
                    await BrowserWindows.Last().BrowserActAsync(ActType.Click, AttributeType.ClassName, "tBJ dyH iFc SMy yTZ B9u DrD IZT mWe",
                        delayAfter: 3);
                    isClicked = true;
                });

                while (!isClicked)
                    DelayBeforeOperation(2000);

                isClicked = false;
                Application.Current.Dispatcher.Invoke(async () =>
                {
                    await BrowserWindows.Last().BrowserActAsync(ActType.ScrollIntoViewQuery, AttributeType.Name, "communityItemTextBox",
                        delayAfter: 4);
                    isClicked = true;
                });

                while (!isClicked)
                    DelayBeforeOperation(2000);

                isClicked = false;
                Application.Current.Dispatcher.Invoke(async () =>
                {
                    await BrowserWindows.Last().BrowserActAsync(ActType.ScrollWindow, AttributeType.Null, "",
                        delayAfter: 4, scrollByPixel: -200);
                    isClicked = true;
                });

                while (!isClicked)
                    DelayBeforeOperation(2000);

                BrowserDispatcher(BrowserFuncts.MouseClick, token, 4, 877, 214);
                BrowserWindows.Last().EnterChars(comment, delayAtLast: 2);
                BrowserDispatcher(BrowserFuncts.EnterChars, token, 5, comment);

                var idOrClass = "//button[@class='RCK Hsu Vxj aZc GmH adn OWt gpV NTm KhY jJP']";
                var resp =
                    BrowserWindows.Last()
                        .ExecuteScript(
                            $"document.evaluate('{idOrClass.Replace("\'", "\\'")}', document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue.disabled = false;") ??
                    throw new ArgumentNullException(
                        "BrowserWindows.Last().ExecuteScript($\"document.evaluate(\'{idOrClass.Replace(\"\\\'\", \"\\\\\'\")}\', document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue.disabled = false;\")");
                var resp2 = BrowserWindows.Last().ExecuteScript($"document.evaluate('{"//div[@name='communityItemTextBox']".Replace("\'", "\\'")}', document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).InnerHtml='sdfdsf';");
                var resp3 = BrowserWindows.Last().ExecuteScript($"document.getElementsByClassName('RCK Hsu Vxj aZc GmH adn OWt gpV NTm KhY jJP')[0].disabled = false;");
                isClicked = false;
                Application.Current.Dispatcher.Invoke(async () =>
                {
                    await BrowserWindows.Last().BrowserActAsync(ActType.Click, AttributeType.ClassName, "RCK Hsu Vxj aZc GmH adn Il7 Jrn hNT iyn BG7 NTm KhY jJP",
                        delayAfter: 7);
                    isClicked = true;
                });

                while (!isClicked)
                    DelayBeforeOperation(2000);
            }

            return null;
        }

        public TryResponse Try(DominatorAccountModel account, string pinId, string note, string path, CancellationTokenSource token)
        {
            try
            {
                string pinUrl = string.Empty;
                if (!pinId.Contains("pinterest"))
                    pinUrl = $"https://{Domain}/pin/{pinId}/";
                else
                    pinUrl = $"https://{Domain}/pin/{pinId.Split('/')[4]}/";

                pinId = pinUrl.Split('/')[4];

                BrowserDispatcher(BrowserFuncts.GoToUrl, token, 0, pinUrl, pinId);

                DelayBeforeOperation(2000);
                string attributeNameForTryPin = BrowserUtilities.GetAttributeNameWithInnerText(CurrentData, "button", "Add photo");

                //click on add photo
                BrowserDispatcher(BrowserFuncts.BrowserAct, token, 0, ActType.Click, AttributeType.ClassName,
                    "RCK Hsu USg adn CCY czT F10 xD4 fZz hUC a_A gpV hNT BG7 gn8 L4E kVc", 0, 1);
                DelayBeforeOperation(2000);
                var pageSource = BrowserWindows.Last().GetPageSource();
                if (!pageSource.Contains("Add a photo of This item is unavailable"))
                {
                    BrowserDispatcher(BrowserFuncts.BrowserAct, token, 0, ActType.Click, AttributeType.ClassName,
                   "RCK Hsu USg adn CCY czT F10 xD4 fZz hUC a_A gpV hNT BG7 gn8 L4E kVc", 0, 0);
                    DelayBeforeOperation(5000);
                }

                if (!string.IsNullOrEmpty(attributeNameForTryPin) && CurrentData.Contains("Add photo"))
                {
                    pageSource = BrowserWindows.Last().GetPageSource();

                    DelayBeforeOperation(2000);
                    BrowserWindows.Last().ChooseFileFromDialog(path);
                    BrowserDispatcher(BrowserFuncts.GetXY, token, 2, AttributeType.ClassName, "hjj oqv zI7 iyn Hsu");
                    BrowserDispatcher(BrowserFuncts.MouseClick, token, 2, _lastXandY.Key, _lastXandY.Value);
                    DelayBeforeOperation(5000);
                    BrowserDispatcher(BrowserFuncts.GetXY, token, 2, AttributeType.ClassName,
                        "Gnj Hsu tBJ dyH iFc yTZ L4E unP iyn Pve pBj qJc aKM LJB");
                    BrowserDispatcher(BrowserFuncts.MouseClick, token, 2, _lastXandY.Key + 5, _lastXandY.Value + 5);
                    BrowserDispatcher(BrowserFuncts.EnterChars, token, 2, note);

                    DelayBeforeOperation(3000);
                    pageSource = BrowserWindows.Last().GetPageSource();
                    var attributeValueForDone = BrowserUtilities.GetAttributeNameWithInnerText(pageSource, "button", "Done");

                    BrowserDispatcher(BrowserFuncts.BrowserAct, token, 2, ActType.Click, AttributeType.ClassName, "RCK Hsu USg adn CCY czT Vxj aZc Zr3 hA- Il7 Jrn hNT BG7 NTm KhY iyn");
                    DelayBeforeOperation(3000);
                    pageSource = BrowserWindows.Last().GetPageSource();
                    if (!pageSource.Contains("Drag and drop or click to upload"))
                        return new TryResponse(new ResponseParameter { Response = string.Empty }) { Success = true };
                }
                else
                    return new TryResponse(new ResponseParameter { Response = string.Empty })
                    { Success = false, Issue = new PinterestIssue { Message = "LangKeyThisPinNotHasTryOption".FromResourceDictionary() } };
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return new TryResponse(new ResponseParameter { Response = string.Empty }) { Success = false };
        }

        //post single pins/multiple pins response handler
        public RepostPinResponseHandler Post(DominatorAccountModel account, string boardUrl, PublisherPostlistModel postDetails, CancellationTokenSource token,SectionDetails sectionDetails=null)
        {
            try
            {
                if (!boardUrl.Contains("pinterest"))
                {
                    if (boardUrl[0] == '/')
                        boardUrl = $"https://{Domain}{boardUrl}";
                    else
                        boardUrl = $"https://{Domain}/{boardUrl}";
                }
                var boardInfo = SearchByCustomBoard(account, boardUrl, token);
                //custom Pin Posting
                if (!string.IsNullOrEmpty(postDetails.ShareUrl) && postDetails.MediaList.Count==0)
                {
                    BrowserDispatcher(BrowserFuncts.GoToUrl, token, 0, postDetails.ShareUrl);
                    DelayBeforeOperation(5000);
                    BrowserDispatcher(BrowserFuncts.ExecuteCustomScript, token, 2,string.Format(PDClassesConstant.ScriptConstant.ScriptWithQuerySelectorToClick,HtmlTags.Div,HtmlTags.HtmlAttribute.DataTestId, "PinBetterSaveDropdown"), 2);
                    DelayBeforeOperation();
                    BrowserDispatcher(BrowserFuncts.EnterChars, token, 3, boardInfo.BoardName);
                    DelayBeforeOperation(5000);
                    BrowserDispatcher(BrowserFuncts.ExecuteCustomScript, token, 3,string.Format(PDClassesConstant.ScriptConstant.ScriptWithQuerySelectorToClick,HtmlTags.Div,HtmlTags.HtmlAttribute.DataTestId,$"board-row-{boardInfo.BoardName}"), 2);
                    DelayBeforeOperation(5000);
                    string response = BrowserWindows.Last().GetPaginationData("\"resource\":{\"name\":\"RepinResource\"", true).Result;
                    int failedCount = 0;
                    while(failedCount++<=2 &&(string.IsNullOrEmpty(response) || !response.Contains("\"status\":\"success\"")))
                    {
                        response= BrowserWindows.Last().GetPaginationData("\"resource\":{\"name\":\"RepinResource\"", true).Result;
                    }
                    return new RepostPinResponseHandler(new ResponseParameter { Response = response }, boardUrl) { Success = true };
                }
                DelayBeforeOperation(5000);
                int clickIndex = account.DisplayColumnValue11.Equals("Normal Mode") ? 2 : 0;
                BrowserDispatcher(BrowserFuncts.BrowserAct, token, 2, ActType.Click, AttributeType.ClassName,
                        "tBJ dyH iFc sAJ O2T tg7 H2s",string.Empty, clickIndex);
                DelayBeforeOperation(2000);
                var filePath = postDetails.MediaList.GetRandomItem();
                BrowserDispatcher(BrowserFuncts.ClickByAttributeName, token, 2, ActType.GetAttribute, AttributeType.TagName,
                        "a",ValueTypes.Href, "/pin-builder/");
                DelayBeforeOperation(5000);
                //In Some Account An Additional Pop Will Appear,So Simply Close It.
                BrowserDispatcher(BrowserFuncts.ExecuteCustomScript, token, 2,string.Format(PDClassesConstant.ScriptConstant.ScriptWithQuerySelectorToClick,HtmlTags.Button,HtmlTags.HtmlAttribute.AriaLabel,"Cancel"), 2);
                DelayBeforeOperation(2000);
                if(!BrowserWindows.Last().GetPageSource().Contains("File upload"))
                {
                    BrowserDispatcher(BrowserFuncts.ExecuteCustomScript, token, 2, "document.querySelector('button[aria-haspopup=\"dialog\"]').click();", 2);
                    var Page = BrowserWindows.Last().GetPageSource();
                    var Nodes = HtmlAgilityHelper.GetListInnerHtmlOrInnerTextOrOuterHtmlFromIdOrClass(Page, "board_actions-item", true,string.Empty, false);
                    var Index = Nodes.Count > 0 ?Nodes.IndexOf(Nodes.FirstOrDefault(x=>x.Equals("Pin"))): 0;
                    BrowserDispatcher(BrowserFuncts.ExecuteCustomScript, token, 2, string.Format(PDClassesConstant.ScriptConstant.ScriptWithQuerySelectorAllToClick,HtmlTags.Div,HtmlTags.HtmlAttribute.Id, $"board_actions-item-{Index}",Index), 4);
                }
                //Select Target Board.
                BrowserDispatcher(BrowserFuncts.ExecuteCustomScript, token, 2,string.Format(PDClassesConstant.ScriptConstant.ScriptWithQuerySelectorToClick,HtmlTags.Button,HtmlTags.HtmlAttribute.DataTestId, "board-dropdown-select-button"), 2);
                DelayBeforeOperation();
                BrowserDispatcher(BrowserFuncts.EnterChars, token, 3, boardInfo.BoardName);
                DelayBeforeOperation();
                BrowserDispatcher(BrowserFuncts.ExecuteCustomScript, token, 3,string.Format(PDClassesConstant.ScriptConstant.ScriptWithQuerySelectorToClick,HtmlTags.Div,HtmlTags.HtmlAttribute.DataTestId, $"board-row-{boardInfo.BoardName}"), 2);
                var PageResponse = BrowserWindows.Last().GetPageSource();
                if (sectionDetails != null || PageResponse.Contains("Choose section"))
                {
                    if (!string.IsNullOrEmpty(sectionDetails?.SectionTitle))
                        BrowserDispatcher(BrowserFuncts.ExecuteCustomScript, token, 3, string.Format(PDClassesConstant.ScriptConstant.ScriptWithQuerySelectorToClick, HtmlTags.Div, HtmlTags.HtmlAttribute.DataTestId, $"section-row-{sectionDetails.SectionTitle}"), 2);
                    else
                        BrowserDispatcher(BrowserFuncts.ExecuteCustomScript, token, 3, string.Format(PDClassesConstant.ScriptConstant.ScriptWithQuerySelectorAllToClick, HtmlTags.Div, HtmlTags.Title,boardInfo.BoardName,1), 2);
                }
                DelayBeforeOperation(3000);
                if (CurrentData.Contains("Not now") && CurrentData.Contains("Edit my home feed"))
                    BrowserDispatcher(BrowserFuncts.BrowserAct, token, 2, ActType.Click, AttributeType.ClassName,
                      "tBJ dyH iFc SMy yTZ erh tg7 mWe");

                if (postDetails.PostSource == DominatorHouseCore.Enums.SocioPublisher.PostSource.NormalPost
                    || postDetails.PostSource == DominatorHouseCore.Enums.SocioPublisher.PostSource.MonitorFolderPost
                    || postDetails.PostSource == DominatorHouseCore.Enums.SocioPublisher.PostSource.RssFeedPost)
                {
                    BrowserWindows.Last().ChooseFileFromDialog(filePath);
                    BrowserDispatcher(BrowserFuncts.GetXY, token, 2, AttributeType.ClassName,
                         "XiG Zr3 hUC s2n sLG");
                    if(_lastXandY.Key==0&&_lastXandY.Value==0)
                        BrowserDispatcher(BrowserFuncts.GetXY, token, 2, AttributeType.ClassName,
                         "DUt XiG zI7 iyn Hsu");
                    if (_lastXandY.Key == 0 && _lastXandY.Value == 0)
                    {
                        BrowserDispatcher(BrowserFuncts.ExecuteCustomScript, token, 2, string.Format(PDClassesConstant.ScriptConstant.ScriptWithQuerySelectorToClick,HtmlTags.Button,HtmlTags.HtmlAttribute.AriaLabel, "Collapse drafts sidebar"), 3);
                        BrowserDispatcher(BrowserFuncts.ExecuteCustomScript, token, 2, "document.querySelector(\"label[for='storyboard-selector-title']\").scrollIntoViewIfNeeded();", 3);
                        BrowserDispatcher(BrowserFuncts.GetXY, token, 2, AttributeType.Id,
                         "storyboard-upload-input");
                    }
                    BrowserDispatcher(BrowserFuncts.MouseClick, token, 2, _lastXandY.Key+10, _lastXandY.Value+10);
                    DelayBeforeOperation(3000);
                    var pageSource = BrowserWindows.Last().GetPageSource();
                    var ImageStatus = IsValidImage(pageSource);
                    if (!ImageStatus.status)
                        return new RepostPinResponseHandler(new ResponseParameter { Response = ImageStatus.Response }, boardUrl) { Success=false,Issue=new PinterestIssue { Message=ImageStatus.Response,Status="failed"} };
                    if (!string.IsNullOrEmpty(postDetails.PublisherInstagramTitle))
                    {
                        //var TitleclassName = HtmlParseUtility.GetAttributeValueFromTagName(pageSource, "input", "placeholder", "Add a title", "class");
                        //BrowserDispatcher(BrowserFuncts.GetXY, token, 2, AttributeType.ClassName,
                        //    TitleclassName);
                        //if (_lastXandY.Key == 0 && _lastXandY.Value == 0)
                        //    BrowserDispatcher(BrowserFuncts.GetXY, token, 2, AttributeType.Id,
                        //    "storyboard-selector-title");
                        //BrowserDispatcher(BrowserFuncts.MouseClick, token, 2, _lastXandY.Key + 5, _lastXandY.Value + 5);

                        var X = BrowserWindows.Last().ExecuteScript("document.querySelector('input[placeholder=\"Add a title\"]').getBoundingClientRect().x", delayInSec: 2).Result;
                        var Y = BrowserWindows.Last().ExecuteScript("document.querySelector('input[placeholder=\"Add a title\"]').getBoundingClientRect().y", delayInSec: 2).Result;
                        BrowserWindows.Last().MouseClick(Convert.ToInt32(X) + 5, Convert.ToInt32(Y) + 5, delayAfter: 2);
                        BrowserDispatcher(BrowserFuncts.EnterChars, token, 2, $" {postDetails.PublisherInstagramTitle.Trim()}");
                        DelayBeforeOperation(2000);
                    }
                    if (!string.IsNullOrEmpty(postDetails.PostDescription))
                    {
                        BrowserDispatcher(BrowserFuncts.GetXY, token, 2, AttributeType.ClassName,
                        "public-DraftEditorPlaceholder-root", 0);
                        BrowserWindows.Last().MouseClick(_lastXandY.Key + 5, _lastXandY.Value + 5, delayAfter: 2);
                        BrowserWindows.Last().CopyPasteContent(postDetails.PostDescription.Trim(), winKeyCode: 86, delayAtLast: 2);
                        //BrowserDispatcher(BrowserFuncts.EnterChars, token, 2, postDetails.PostDescription.Trim());
                        DelayBeforeOperation(1000);
                    }
                    if (!string.IsNullOrEmpty(postDetails.PdSourceUrl))
                    {
                        //pageSource = BrowserWindows.Last().GetPageSource();
                        //var sourceurlclassName = HtmlParseUtility.GetAttributeValueFromTagName(pageSource, "input", "placeholder", "Add a link", "class");
                        //BrowserWindows.Last().BrowserActAsync(ActType.ScrollIntoView, AttributeType.ClassName, sourceurlclassName,
                        //        delayAfter: 4);
                        //BrowserDispatcher(BrowserFuncts.GetXY, token, 2, AttributeType.ClassName,
                        //    sourceurlclassName);
                        //if (_lastXandY.Key == 0 && _lastXandY.Value == 0)
                        //    BrowserDispatcher(BrowserFuncts.GetXY, token, 2, AttributeType.Id,
                        //    "WebsiteField");
                        //BrowserDispatcher(BrowserFuncts.MouseClick, token, 2, _lastXandY.Key + 5, _lastXandY.Value + 5);

                        var X = BrowserWindows.Last().ExecuteScript("document.querySelector('input[placeholder=\"Add a link\"]').getBoundingClientRect().x", delayInSec: 2).Result;
                        var Y = BrowserWindows.Last().ExecuteScript("document.querySelector('input[placeholder=\"Add a link\"]').getBoundingClientRect().y", delayInSec: 2).Result;
                        BrowserWindows.Last().MouseClick(Convert.ToInt32(X) + 5, Convert.ToInt32(Y) + 5, delayAfter: 2);
                        BrowserDispatcher(BrowserFuncts.EnterChars, token, 2, postDetails.PdSourceUrl.Trim());
                        DelayBeforeOperation(2000);
                    }
                    var saveAttribute = BrowserUtilities.GetAttributeNameWithInnerText(pageSource,HtmlTags.Div, "Save");
                    if (string.IsNullOrEmpty(saveAttribute))
                        saveAttribute = BrowserUtilities.GetAttributeNameWithInnerText(pageSource,HtmlTags.Div, "Publish");
                    BrowserWindows.Last().ClearResources();
                    DelayBeforeOperation(1000);
                    BrowserDispatcher(BrowserFuncts.BrowserAct, token, 2, ActType.Click, AttributeType.ClassName, saveAttribute);
                    DelayBeforeOperation(5000);
                    pageSource = BrowserWindows.Last().GetPageSource();
                    if (pageSource.Contains("Changes stored!") || pageSource.Contains("Saving...") || pageSource.Contains("storyboard-creation-nav-done"))
                        BrowserDispatcher(BrowserFuncts.ExecuteCustomScript,token,2,string.Format(PDClassesConstant.ScriptConstant.ScriptWithQuerySelectorAllToFilter,HtmlTags.Button,HtmlTags.HtmlAttribute.Type,HtmlTags.Button,"Publish", 0,"click();"));
                    DelayBeforeOperation(10000);
                    pageSource = BrowserWindows.Last().GetPageSource();
                    BrowserDispatcher(BrowserFuncts.ExecuteCustomScript, token, 2, string.Format(PDClassesConstant.ScriptConstant.ScriptWithQuerySelectorToClick, HtmlTags.Button, HtmlTags.HtmlAttribute.AriaLabel, "Close"), 2);
                    var text = pageSource;
                    if (text.Contains("Your upload failed because it is in the wrong format.") ||
                        text.Contains("An image is required to create a Pin."))
                    {
                        return new RepostPinResponseHandler(new ResponseParameter { Response = text })
                        { Success = false, Issue = new PinterestIssue { Message = "Image Not Found" } };
                    }
                    if (text.Contains("Sorry, we've blocked this description because it contains a link that may lead to inappropriate content.") ||
                        text.Contains("Sorry, we blocked this link because it may lead to spam."))
                    {
                        return new RepostPinResponseHandler(new ResponseParameter { Response = text })
                        { Success = false, Issue = new PinterestIssue { Message = "Sorry, we blocked this link because it may lead to spam." } };

                    }
                    if (text.Contains("Not a valid URL."))
                    {
                        return new RepostPinResponseHandler(new ResponseParameter { Response = text })
                        { Success = false, Issue = new PinterestIssue { Message = "Not a valid URL." } };
                    }
                    if (text.Contains("See it now"))
                    {
                        int length = 0;
                        int.TryParse(BrowserWindows.Last().GetElementValueAsync(ActType.GetLength,
                                AttributeType.TagName,HtmlTags.Button).Result, out length);
                        BrowserDispatcher(BrowserFuncts.BrowserAct, token, 2, ActType.Click,
                            AttributeType.TagName,HtmlTags.Button, "", length - 1);
                        DelayBeforeOperation(2000);
                        BrowserDispatcher(BrowserFuncts.GetCurrentUrl, token, 3);
                        string pinid = Utilities.GetBetween(CurrentData + "#", "pin/", "#");
                        return new RepostPinResponseHandler(new ResponseParameter { Response = string.Empty }) { Success = true, PinId = pinid };
                    }
                    else if (text.Contains("See your Pin"))
                    {
                        var seeYourPinAttr = BrowserUtilities.GetAttributeNameWithInnerText(text,HtmlTags.Button, "See your Pin");
                        BrowserDispatcher(BrowserFuncts.BrowserAct, token, 2, ActType.Click, AttributeType.ClassName, seeYourPinAttr);
                        DelayBeforeOperation(2000);
                        pageSource = BrowserWindows.Last().GetPageSource();
                        string pinid = Utilities.GetBetween(pageSource + "#", "pin/", "/");
                        return new RepostPinResponseHandler(new ResponseParameter { Response = string.Empty }) { Success = true, PinId = pinid };
                    }
                    else
                    {
                        DelayBeforeOperation(5000);
                        var response = BrowserWindows.Last().GetPaginationData($"{Domain}/resource/PinResource/create", true).Result;
                        if (string.IsNullOrEmpty(response))
                            response = BrowserWindows.Last().GetPaginationData("v3_create_story_pin_data", true).Result;
                        int i = 0;
                        while (string.IsNullOrEmpty(response) && i++ < 5)
                        {
                            response = BrowserWindows.Last().GetPaginationData($"{Domain}/resource/PinResource/create", true).Result;
                            DelayBeforeOperation(5000);
                        }
                        if (!string.IsNullOrEmpty(response))
                            return new RepostPinResponseHandler(new ResponseParameter { Response = response }, boardUrl) { Success = true };
                        else
                            return new RepostPinResponseHandler(new ResponseParameter { Response = string.Empty }, boardUrl) { Success = false };

                    }
                }
                else if (postDetails.PostSource == DominatorHouseCore.Enums.SocioPublisher.PostSource.ScrapedPost
                    || postDetails.MediaList.GetRandomItem().Contains("http")
                    || postDetails.PostSource == DominatorHouseCore.Enums.SocioPublisher.PostSource.SharePost
                    || postDetails.PostSource==DominatorHouseCore.Enums.SocioPublisher.PostSource.ScrapeImages)
                {
                    var downloadPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\Socinator\\ScrapePins";
                    if (!Directory.Exists(downloadPath))
                        Directory.CreateDirectory(downloadPath);

                    var webClient = new System.Net.WebClient();
                    var downloadDataByte = webClient.DownloadData(filePath);
                    File.WriteAllBytes($"{downloadPath}\\{ postDetails.FetchedPostIdOrUrl}.jpg", downloadDataByte);
                    var mediaFilePath = $"{downloadPath}\\{ postDetails.FetchedPostIdOrUrl}.jpg";
                    BrowserWindows.Last().ChooseFileFromDialog(mediaFilePath);
                    BrowserDispatcher(BrowserFuncts.GetXY, token, 2, AttributeType.ClassName,
                         PDClassesConstant.SocioPublisherPost.SelectMediaClass1);
                    if(_lastXandY.Key==0 && _lastXandY.Value==0)
                        BrowserDispatcher(BrowserFuncts.GetXY, token, 2, AttributeType.ClassName,
                         PDClassesConstant.SocioPublisherPost.SelectMediaClass2);
                    if (_lastXandY.Key == 0 && _lastXandY.Value == 0)
                    {
                        BrowserDispatcher(BrowserFuncts.ExecuteCustomScript, token, 2, string.Format(PDClassesConstant.ScriptConstant.ScriptWithQuerySelectorToClick, HtmlTags.Button, HtmlTags.HtmlAttribute.AriaLabel, "Collapse drafts sidebar"), 3);
                        BrowserDispatcher(BrowserFuncts.ExecuteCustomScript, token, 2, "document.querySelector(\"label[for='storyboard-selector-title']\").scrollIntoViewIfNeeded();", 3);
                        BrowserDispatcher(BrowserFuncts.GetXY, token, 2, AttributeType.Id,
                         "storyboard-upload-input");
                    }
                    BrowserDispatcher(BrowserFuncts.MouseClick, token, 2, _lastXandY.Key+10, _lastXandY.Value+10);
                    DelayBeforeOperation(3000);
                    var pageSource = BrowserWindows.Last().GetPageSource();
                    var ImageStatus = IsValidImage(pageSource);
                    if (!ImageStatus.status)
                        return new RepostPinResponseHandler(new ResponseParameter { Response = ImageStatus.Response }, boardUrl) { Success = false, Issue = new PinterestIssue { Message = ImageStatus.Response, Status = "failed" } };
                    if (!string.IsNullOrEmpty(postDetails.PublisherInstagramTitle))
                    {
                        var TitleclassName = HtmlParseUtility.GetAttributeValueFromTagName(pageSource, HtmlTags.TextArea, "placeholder", "Add your title", "class");
                        BrowserDispatcher(BrowserFuncts.GetXY, token, 2, AttributeType.ClassName,
                            TitleclassName);
                        if (_lastXandY.Key == 0 && _lastXandY.Value == 0)
                            BrowserDispatcher(BrowserFuncts.GetXY, token, 2, AttributeType.Id,
                            "storyboard-selector-title");
                        BrowserDispatcher(BrowserFuncts.MouseClick, token, 2, _lastXandY.Key + 5, _lastXandY.Value + 5);
                        BrowserDispatcher(BrowserFuncts.EnterChars, token, 2, $" {postDetails.PublisherInstagramTitle.Trim()}");
                        DelayBeforeOperation(2000);
                    }
                    if (!string.IsNullOrEmpty(postDetails.PostDescription))
                    {
                        BrowserDispatcher(BrowserFuncts.GetXY, token, 2, AttributeType.ClassName,
                        PDClassesConstant.SocioPublisherPost.PinDescriptionTextAreaClass);
                        BrowserDispatcher(BrowserFuncts.MouseClick, token, 2, _lastXandY.Key + 5, _lastXandY.Value + 5);
                        BrowserDispatcher(BrowserFuncts.EnterChars, token, 2, postDetails.PostDescription.Trim());
                        DelayBeforeOperation(1000);
                    }
                    if (!string.IsNullOrEmpty(postDetails.PdSourceUrl))
                    {
                        pageSource = BrowserWindows.Last().GetPageSource();
                        var sourceurlclassName = HtmlParseUtility.GetAttributeValueFromTagName(pageSource, HtmlTags.TextArea, HtmlTags.HtmlAttribute.PlaceHolder, "Add a destination link", "class");
                        BrowserWindows.Last().BrowserActAsync(ActType.ScrollIntoView, AttributeType.ClassName, sourceurlclassName,
                                delayAfter: 4);
                        BrowserDispatcher(BrowserFuncts.GetXY, token, 2, AttributeType.ClassName,
                            sourceurlclassName);
                        if (_lastXandY.Key == 0 && _lastXandY.Value == 0)
                            BrowserDispatcher(BrowserFuncts.GetXY, token, 2, AttributeType.Id,
                            "WebsiteField");
                        BrowserDispatcher(BrowserFuncts.MouseClick, token, 2, _lastXandY.Key + 5, _lastXandY.Value + 5);
                        BrowserDispatcher(BrowserFuncts.EnterChars, token, 2, postDetails.PdSourceUrl.Trim());
                        DelayBeforeOperation(2000);
                    }
                    pageSource = BrowserWindows.Last().GetPageSource();
                    var text = pageSource;
                    var saveAttribute = BrowserUtilities.GetAttributeNameWithInnerText(pageSource, HtmlTags.Div, "Save");
                    if (string.IsNullOrEmpty(saveAttribute))
                        saveAttribute = BrowserUtilities.GetAttributeNameWithInnerText(pageSource,HtmlTags.Div, "Publish");
                    BrowserDispatcher(BrowserFuncts.BrowserAct, token, 2, ActType.Click, AttributeType.ClassName, saveAttribute);
                    DelayBeforeOperation(5000);
                    if (pageSource.Contains("Changes stored!") || pageSource.Contains("Saving...") || pageSource.Contains("storyboard-creation-nav-done"))
                        BrowserDispatcher(BrowserFuncts.ExecuteCustomScript, token, 2, string.Format(PDClassesConstant.ScriptConstant.ScriptWithQuerySelectorAllToFilter, HtmlTags.Button, HtmlTags.HtmlAttribute.Type, HtmlTags.Button, "Publish", 0, "click();"));
                    DelayBeforeOperation(10000);
                    pageSource = BrowserWindows.Last().GetPageSource();
                    BrowserDispatcher(BrowserFuncts.ExecuteCustomScript, token, 2,string.Format(PDClassesConstant.ScriptConstant.ScriptWithQuerySelectorToClick,HtmlTags.Button,HtmlTags.HtmlAttribute.AriaLabel,"Close"), 2);
                    text = pageSource;
                    if (text.Contains("Sorry, we've blocked this description because it contains a link that may lead to inappropriate content.") ||
                        text.Contains("Sorry, we blocked this link because it may lead to spam."))
                    {
                        return new RepostPinResponseHandler(new ResponseParameter { Response = text })
                        { Success = false, Issue = new PinterestIssue { Message = "Sorry, we blocked this link because it may lead to spam." } };

                    }
                    if (text.Contains("Not a valid URL."))
                    {
                        return new RepostPinResponseHandler(new ResponseParameter { Response = text })
                        { Success = false, Issue = new PinterestIssue { Message = "Not a valid URL." } };
                    }
                    if (text.Contains("See it now"))
                    {
                        int length = 0;
                        int.TryParse(BrowserWindows.Last().GetElementValueAsync(ActType.GetLength,
                                AttributeType.TagName, HtmlTags.Button).Result, out length);
                        BrowserDispatcher(BrowserFuncts.BrowserAct, token, 2, ActType.Click,
                            AttributeType.TagName,HtmlTags.Button, "", length - 1);
                        DelayBeforeOperation(2000);
                        BrowserDispatcher(BrowserFuncts.GetCurrentUrl, token, 3);
                        string pinid = Utilities.GetBetween(CurrentData + "#", "pin/", "#");
                        return new RepostPinResponseHandler(new ResponseParameter { Response = string.Empty }) { Success = true, PinId = pinid };
                    }
                    else if (text.Contains("See your Pin"))
                    {
                        var seeYourPinAttr = BrowserUtilities.GetAttributeNameWithInnerText(text, "button", "See your Pin");
                        BrowserDispatcher(BrowserFuncts.BrowserAct, token, 2, ActType.Click, AttributeType.ClassName, seeYourPinAttr);
                        DelayBeforeOperation(2000);
                        pageSource = BrowserWindows.Last().GetPageSource();
                        string pinid = Utilities.GetBetween(pageSource + "#", "pin/", "/");
                        return new RepostPinResponseHandler(new ResponseParameter { Response = string.Empty }) { Success = true, PinId = pinid };
                    }
                    else
                    {
                        DelayBeforeOperation(5000);
                        var response = BrowserWindows.Last().GetPaginationData($"{Domain}/resource/PinResource/create", true).Result;
                        if (string.IsNullOrEmpty(response))
                            response = BrowserWindows.Last().GetPaginationData("v3_create_story_pin_data", true).Result;
                        int i = 0;
                        while (string.IsNullOrEmpty(response) && i++ < 5)
                        {
                            response = BrowserWindows.Last().GetPaginationData($"{Domain}/resource/PinResource/create", true).Result;
                            DelayBeforeOperation(5000);
                        }

                        if (!string.IsNullOrEmpty(response))
                            return new RepostPinResponseHandler(new ResponseParameter { Response = response }, boardUrl) { Success = true };
                        else
                            return new RepostPinResponseHandler(new ResponseParameter { Response = string.Empty }, boardUrl) { Success = false };

                    }

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
            return new RepostPinResponseHandler(new ResponseParameter { Response = string.Empty }, "") { Success = false };
        }
        public RepostPinResponseHandler Repin(DominatorAccountModel account, string pinId, string board, CancellationTokenSource cancellationToken,SectionDetails section=null)
        {
            string pinUrl = string.Empty;
            if (!pinId.Contains("pinterest"))
                pinUrl = $"https://{Domain}/pin/{pinId}/";
            else
                pinUrl = $"https://{Domain}/pin/{pinId.Split('/')[4]}/";

            string boardUrl = $"https://{Domain}{board}";
            pinId = pinUrl.Split('/')[4];
            var boardInfo = SearchByCustomBoard(account, boardUrl, cancellationToken);
            if (string.IsNullOrEmpty(boardInfo.BoardName))
                boardInfo = SearchByCustomBoard(account, boardUrl, cancellationToken);

            BrowserDispatcher(BrowserFuncts.GoToUrl, cancellationToken, 5, pinUrl, pinId);
            int failedCount = 0;
            DelayBeforeOperation(5000);

            while (failedCount++ < 5 && !CurrentData.Contains("PinBetterSaveDropdown") && !CurrentData.Contains("Save"))
            {
                BrowserWindows.Last().Refresh();
                BrowserDispatcher(BrowserFuncts.GoToUrl, cancellationToken, 5, pinUrl, pinId);
                DelayBeforeOperation(5000);
            }
            if (!CurrentData.Contains(pinId))
                BrowserDispatcher(BrowserFuncts.GoToUrl, cancellationToken, 5, pinUrl, pinId);

            try
            {
                DelayBeforeOperation(5000);
                var isClicked=BrowserDispatcher(BrowserFuncts.ExecuteCustomScript, cancellationToken, 2, $"document.querySelector('{HtmlTags.Button}[data-test-id=\"PinBetterSaveDropdown\"]').click();", 2).Success;
                if (!isClicked)
                    BrowserDispatcher(BrowserFuncts.ExecuteCustomScript, cancellationToken, 2, $"document.querySelector('{HtmlTags.Div}[data-test-id=\"quick-save-button\"]').childNodes[0].click();", 2);
                DelayBeforeOperation(5000);
                BrowserDispatcher(BrowserFuncts.EnterChars, cancellationToken, 3, boardInfo.BoardName);
                DelayBeforeOperation(5000);
                BrowserDispatcher(BrowserFuncts.ExecuteCustomScript, cancellationToken, 3, $"document.querySelector('{HtmlTags.Div}[data-test-id=\"board-row-{boardInfo.BoardName}\"]').click();", 2);
                DelayBeforeOperation(5000);
                var PageResponse = BrowserWindows.Last().GetPageSource();
                if (PageResponse.Contains($"board-row-{boardInfo.BoardName}") || PageResponse.Contains("Save Pin to"))
                {
                    if(section!=null)
                        BrowserDispatcher(BrowserFuncts.ExecuteCustomScript, cancellationToken, 3, $"document.querySelector('div[data-test-id=\"section-row-{section.SectionTitle}\"]').parentNode.click();", 3);
                    else
                        BrowserDispatcher(BrowserFuncts.ExecuteCustomScript, cancellationToken, 2, string.Format(PDClassesConstant.ScriptConstant.ScriptWithQuerySelectorAllToClick, HtmlTags.Div, HtmlTags.HtmlAttribute.DataTestId, $"board-row-{boardInfo.BoardName}", 1), 2);
                }
                string response = BrowserWindows.Last().GetPaginationData("\"resource\":{\"name\":\"RepinResource\"", true).Result;
                int i = 0;
                while (string.IsNullOrEmpty(response) && i++ < 5)
                {
                    BrowserWindows.Last().Refresh();
                    response = BrowserWindows.Last().GetPaginationData("\"resource\":{\"name\":\"RepinResource\"", true).Result;
                }
                return new RepostPinResponseHandler(new ResponseParameter { Response = response }, boardUrl);
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return new RepostPinResponseHandler(new ResponseParameter { Response = string.Empty }) { Success = false };
            }
        }

        public DeletePinResponseHandler DeletePin(DominatorAccountModel account, string pinId, CancellationTokenSource cancellationToken)
        {
            bool isClicked = false;
            string pinUrl = string.Empty;
            if (!pinId.Contains("pinterest"))
                pinUrl = $"https://{Domain}/pin/{pinId}/";
            else
                pinUrl = $"https://{Domain}/pin/{pinId.Split('/')[4]}/";

            pinId = pinUrl.Split('/')[4];

            BrowserDispatcher(BrowserFuncts.GoToUrl, cancellationToken, 5, pinUrl, pinId);

            if (!CurrentData.Contains(pinId))
                BrowserDispatcher(BrowserFuncts.GoToUrl, cancellationToken, 0, pinUrl, pinId);

            try
            {
                int failedCount = 0;
                int length = 0;
                while (failedCount++ < 5 && !isClicked)
                {
                    var lstInnerText = BrowserWindows.Last().GetListInnerHtml(ActType.GetAttribute, AttributeType.TagName,
                         HtmlTags.Button, ValueTypes.AriaLabel).Result;
                    lstInnerText.Reverse();
                    int index = lstInnerText.IndexOf("More options");
                    BrowserDispatcher(BrowserFuncts.BrowserAct, cancellationToken, 2, ActType.Click,
                        AttributeType.TagName,HtmlTags.Button, "", index);
                    DelayBeforeOperation(2000);

                    lstInnerText.Reverse();
                    index = lstInnerText.IndexOf("Delete");

                    BrowserDispatcher(BrowserFuncts.BrowserAct, cancellationToken, 2, ActType.Click,
                       AttributeType.TagName, HtmlTags.Button, "", index);
                    DelayBeforeOperation(2000);
                    int.TryParse(BrowserWindows.Last().GetElementValueAsync(ActType.GetLength,
                    AttributeType.TagName, HtmlTags.Button).Result, out length);
                    BrowserDispatcher(BrowserFuncts.GetElementValue, cancellationToken, 2, ActType.GetValue,
                        AttributeType.TagName, HtmlTags.Button, ValueTypes.InnerText, length - 1);
                    isClicked = CurrentData.Contains("Delete Pin");
                }

                BrowserWindows.Last().ClearResources();
                failedCount = 0;
                isClicked = false;
                while (failedCount++ < 5 && !isClicked)
                {
                    int.TryParse(BrowserWindows.Last().GetElementValueAsync(ActType.GetLength,
                    AttributeType.TagName, HtmlTags.Button).Result, out length);
                    BrowserDispatcher(BrowserFuncts.BrowserAct, cancellationToken, 2, ActType.Click,
                        AttributeType.TagName, HtmlTags.Button, "", length - 1);
                    BrowserDispatcher(BrowserFuncts.GetCurrentUrl, cancellationToken, 5);
                    if (!CurrentData.Contains(pinId))
                        isClicked = true;
                }
                string response = BrowserWindows.Last().GetPaginationData("\"path\":\"/resource/PinResource/delete/\"", true).Result;

                return new DeletePinResponseHandler(new ResponseParameter { Response = response }) { Success = true };
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception)
            {
                return new DeletePinResponseHandler(new ResponseParameter { Response = string.Empty }) { Success = false };
            }
        }

        public RepostPinResponseHandler EditPin(DominatorAccountModel account, PinterestPin pin, CancellationTokenSource cancellationToken)
        {
            try
            {
                string pinUrl = string.Empty;
                string pinId = pin.PinId;
                if (!pinId.Contains("pinterest"))
                    pinUrl = $"https://{Domain}/pin/{pinId}/";
                else
                    pinUrl = $"https://{Domain}/pin/{pinId.Split('/')[4]}/";
                if (!pin.BoardUrl.Contains("https://"))
                    pin.BoardUrl = $"https://{Domain}/{account.AccountBaseModel.ProfileId}/{pin.BoardName}/";
                if (!string.IsNullOrEmpty(pin.BoardName) && (pin.BoardName.Contains("https://") || pin.BoardName.Contains("http://")))
                    pin.BoardName = pin.BoardName.Split('/').Last(x => x.ToString() != string.Empty);
                BoardInfoPtResponseHandler boardInfo = SearchByCustomBoard(account, pin.BoardUrl, cancellationToken);
                string sectionTitle = string.Empty;
                if (!string.IsNullOrEmpty(pin.Section) && pin.Section.Contains("http"))
                {
                    BrowserDispatcher(BrowserFuncts.GoToUrl, cancellationToken, 0, pin.Section, "Find some ideas for this section:");
                    string jsonData = Utilities.GetBetween(CurrentData, "<script type=\"application/json\" id=\"initial-state\">", "</script>");
                    if (!string.IsNullOrEmpty(jsonData))
                    {
                        try
                        {
                            JsonHandler jsonHand = new JsonHandler(jsonData);
                            sectionTitle = jsonHand.GetJTokenValue(jsonHand.GetJToken("sections").First().First(), "title");
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }
                    }
                }

                pinId = pinUrl.Split('/').Last(x => x.ToString() != string.Empty);
                BrowserDispatcher(BrowserFuncts.GoToUrl, cancellationToken, 0, pinUrl, pinId);
                DelayBeforeOperation(5000);
                BrowserDispatcher(BrowserFuncts.ClickByAttributeName, cancellationToken, 2, ActType.GetAttribute,
                    AttributeType.TagName, HtmlTags.Button, ValueTypes.AriaLabel, "More options");
                if(BrowserWindows.Last().GetPageSource().Contains("pin-action-dropdown") && !BrowserWindows.Last().GetPageSource().Contains("Edit Pin"))
                    return new RepostPinResponseHandler(new ResponseParameter { Response = string.Empty }) { Success = false,Issue=new PinterestIssue {Status="failure",Message= "You are not permitted to access this resource" } };
                BrowserDispatcher(BrowserFuncts.ExecuteCustomScript, cancellationToken, 2, $"document.querySelector('{HtmlTags.Div}[data-test-id=\"pin-action-dropdown-edit-pin\"]').click();", 2);
                DelayBeforeOperation(5000);
                //for select or create new board
                var xyForBoardName = BrowserWindows.Last().GetXAndY(AttributeType.ClassName, "i_s h5B xcv L4E zI7 iyn Hsu");
                BrowserDispatcher(BrowserFuncts.MouseClick, cancellationToken, 1, xyForBoardName.Key + 5, xyForBoardName.Value + 5);
                BrowserDispatcher(BrowserFuncts.EnterChars, cancellationToken, 3, pin.BoardName?.Replace("-"," "));
                var PageSource = BrowserWindows.Last().GetPageSource();
                int ClickIndex = account.DisplayColumnValue11.Equals("Normal Mode") ? 2 : 4;
                if (!PageSource.Contains("board-selection"))
                    BrowserDispatcher(BrowserFuncts.BrowserAct, cancellationToken, 2, ActType.Click, AttributeType.ClassName, "RCK Hsu USg adn CCY NTm KhY oRi lnZ wsz YbY", "",ClickIndex);
                else
                    BrowserWindows.Last().PressAnyKey(1, winKeyCode: 13, delayAtLast: 2);
                DelayBeforeOperation(2000);
                var pageSource = BrowserWindows.Last().GetPageSource();

                //create new board if board is not exist
                if (pageSource.Contains("RCK Hsu USg adn CCY NTm KhY czT Vxj aZc Zr3 hA- a_A gpV hNT BG7 hDj _O1 gjz mQ8 FTD L4E"))
                {
                    DelayBeforeOperation(2000);
                    BrowserDispatcher(BrowserFuncts.ExecuteCustomScript, cancellationToken, 0, "document.getElementsByClassName('RCK Hsu USg adn CCY NTm KhY czT Vxj aZc Zr3 hA- a_A gpV hNT BG7 hDj _O1 gjz mQ8 FTD L4E')[0].click();", 2);
                }
                else
                {
                    BrowserDispatcher(BrowserFuncts.MouseClick, cancellationToken, 1, xyForBoardName.Key + 20, xyForBoardName.Value + 120);
                }

                //for select or create new section
                if (!string.IsNullOrEmpty(pin.Section))
                {
                    BrowserDispatcher(BrowserFuncts.MouseClick, cancellationToken, 1, xyForBoardName.Key + 5, xyForBoardName.Value + 100);
                    BrowserDispatcher(BrowserFuncts.EnterChars, cancellationToken, 3," " + pin.Section);
                    pageSource = BrowserWindows.Last().GetPageSource();
                    if (!pageSource.Contains("board-selection"))
                        BrowserDispatcher(BrowserFuncts.BrowserAct, cancellationToken, 2, ActType.Click, AttributeType.ClassName, "RCK Hsu USg adn CCY NTm KhY oRi lnZ wsz YbY", "",ClickIndex);
                    else
                        BrowserWindows.Last().PressAnyKey(1, winKeyCode: 13, delayAtLast: 2);
                }
                DelayBeforeOperation(2000);
                pageSource = BrowserWindows.Last().GetPageSource();
                //create new section if section is not exist
                if (pageSource.Contains("RCK Hsu USg adn CCY NTm KhY czT Vxj aZc Zr3 hA- a_A gpV hNT BG7 hDj _O1 gjz mQ8 FTD L4E"))
                {
                    DelayBeforeOperation(2000);
                    BrowserDispatcher(BrowserFuncts.ExecuteCustomScript, cancellationToken, 0, "document.getElementsByClassName('RCK Hsu USg adn CCY NTm KhY czT Vxj aZc Zr3 hA- a_A gpV hNT BG7 hDj _O1 gjz mQ8 FTD L4E')[0].click();", 2);
                }
                else
                {
                    BrowserDispatcher(BrowserFuncts.MouseClick, cancellationToken, 1, xyForBoardName.Key + 50, xyForBoardName.Value + 200);
                }

                DelayBeforeOperation(2000);
                pageSource = BrowserWindows.Last().GetPageSource();
                //for some of Pins ui is different (only borad,section and note is needed)
                if (pageSource.Contains("Add a private note to remember your ideas about this Pin") && !pageSource.Contains("TitleField") &&
                    !pageSource.Contains("DescriptionField"))
                {
                    var xyForNotes = BrowserWindows.Last().GetXAndY(AttributeType.ClassName, "Gnj Hsu tBJ dyH iFc yTZ L4E unP iyn Pve pBj qJc aKM LJB");
                    if (xyForNotes.Key == 0 && xyForNotes.Value == 0)
                        xyForNotes = BrowserWindows.Last().GetXAndY(AttributeType.ClassName, "Gnj Hsu tBJ dyH iFc sAJ L4E Bvi iyn H_e pBj qJc TKt LJB");
                    BrowserDispatcher(BrowserFuncts.MouseClick, cancellationToken, 1, xyForNotes.Key, xyForNotes.Value + 15);
                    BrowserWindows.Last().PressAnyKey(5, winKeyCode: 40, isShiftDown: true);
                    BrowserWindows.Last().PressAnyKey(winKeyCode: 8);
                    BrowserDispatcher(BrowserFuncts.EnterChars, cancellationToken, 3, " " + pin.Description);
                }
                else
                {
                    var titlexandy = Utilities.GetBetween(pageSource, $"<{HtmlTags.Input} aria-invalid=\"false\" class=\"", "\" id=\"");
                    var xyForPinTitle = BrowserWindows.Last().GetXAndY(AttributeType.ClassName, titlexandy);
                    BrowserDispatcher(BrowserFuncts.MouseClick, cancellationToken, 1, xyForPinTitle.Key + 4, xyForPinTitle.Value + 4);
                    BrowserWindows.Last().PressAnyKey(winKeyCode: 40, isShiftDown: true);
                    BrowserWindows.Last().PressAnyKey(winKeyCode: 8);
                    BrowserDispatcher(BrowserFuncts.EnterChars, cancellationToken, 3, " " + pin.PinName);
                    DelayBeforeOperation(1000);
                    var xyForPinDescrp = BrowserWindows.Last().GetXAndY(AttributeType.ClassName,PDClassesConstant.SocioPublisherPost.PinDescriptionTextAreaClass,1);
                    BrowserDispatcher(BrowserFuncts.MouseClick, cancellationToken, 1, xyForPinDescrp.Key + 1, xyForPinDescrp.Value + 5);
                    BrowserWindows.Last().PressAnyKey(5, winKeyCode: 40, isShiftDown: true);
                    BrowserWindows.Last().PressAnyKey(winKeyCode: 8);
                    BrowserDispatcher(BrowserFuncts.EnterChars, cancellationToken, 3, " " + pin.Description);
                    BrowserDispatcher(BrowserFuncts.MouseClick, cancellationToken, 1, xyForPinTitle.Key + 5, xyForPinTitle.Value + 200);
                    BrowserWindows.Last().PressAnyKey(winKeyCode: 35, isShiftDown: true);
                    BrowserWindows.Last().PressAnyKey(winKeyCode: 8);
                    BrowserDispatcher(BrowserFuncts.EnterChars, cancellationToken, 3, " " + pin.PinWebUrl);
                    DelayBeforeOperation(2000);
                }
                //click on save button
                BrowserDispatcher(BrowserFuncts.ExecuteCustomScript, cancellationToken, 0, $"document.querySelector('{HtmlTags.Div}[data-test-id=\"edit-pin-save\"]').childNodes[0].click();",2);
                DelayBeforeOperation(2000);
                pageSource = BrowserWindows.Last().GetPageSource();
                if (!pageSource.Contains("Edit this Pin"))
                    return new RepostPinResponseHandler(new ResponseParameter { Response = string.Empty }) { Success = true };
                else
                    return new RepostPinResponseHandler(new ResponseParameter { Response = string.Empty }) { Success = false };
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception)
            {
                return new RepostPinResponseHandler(new ResponseParameter { Response = string.Empty }) { Success = false };
            }
        }

        public AcceptBoardInvitationResponseHandler AcceptBoardInvitation(DominatorAccountModel account, string boardUrl, CancellationTokenSource cancellationToken)
        {
            try
            {
                boardUrl = $"https://{Domain}{boardUrl}";
                BrowserWindows.Last().ClearResources();
                BrowserDispatcher(BrowserFuncts.GoToUrl, cancellationToken, 2, boardUrl, "mostRecentBoard");
                DelayBeforeOperation(3000);
                if (BrowserDispatcher(BrowserFuncts.ExecuteCustomScript, cancellationToken, 2, $"[...document.querySelectorAll('{HtmlTags.Button}[type=\"{HtmlTags.Button}\"]')].filter(x=>x.textContent.trim().includes(\"Accept\"))[0].click();", 2).Success)
                    Thread.Sleep(3000);
                string response = BrowserWindows.Last().GetPaginationData("\"resource\":{\"name\":\"BoardInviteResource\"", true).Result;
                return new AcceptBoardInvitationResponseHandler(new ResponseParameter { Response = response });
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception)
            {
                return new AcceptBoardInvitationResponseHandler(new ResponseParameter { Response = string.Empty }) { Success = false };
            }
        }

        public BoardResponse CreateBoard(DominatorAccountModel account, BoardInfo boardInfo, CancellationTokenSource cancellationToken)
        {
            try
            {
                int CreateBoardPageResponseFailedCount = 0;
                BrowserWindows.Last().ClearResources();
                string url = $"https://{Domain}/{account.AccountBaseModel.ProfileId}/boards";
                if (string.IsNullOrEmpty(account.AccountBaseModel.ProfileId))
                {
                    BrowserDispatcher(BrowserFuncts.GoToUrl, cancellationToken, 0, $"https://{Domain}", "pinterest://feed/home");
                    var ProfileId = PdRequestHeaderDetails.GetRequestHeader(CurrentData,TokenDetailsType.Users).Username;
                    account.AccountBaseModel.ProfileId = string.IsNullOrEmpty(ProfileId) ?account.AccountBaseModel.ProfileId:ProfileId;
                    new DominatorHouseCore.Diagnostics.SocinatorAccountBuilder(account.AccountBaseModel.AccountId)
                        .AddOrUpdateDominatorAccountBase(account.AccountBaseModel)
                        .SaveToBinFile();
                    url = $"https://{Domain}/{account.AccountBaseModel.ProfileId}/boards";
                }

                var chkBusinessPageSource = BrowserWindows.Last().GetPageSource();
                var checkWithBusinessProfile = BrowserUtilities.GetAttributeNameWithInnerText(chkBusinessPageSource, "div", "Analytics");
                BrowserDispatcher(BrowserFuncts.GoToUrl, cancellationToken, 0, url, "boardActionsButton");
                while (CreateBoardPageResponseFailedCount++ < 4 && !CurrentData.Contains("boardActionsButton"))
                    BrowserDispatcher(BrowserFuncts.GoToUrl, cancellationToken, 0, url, "boardActionsButton");
                DelayBeforeOperation(2000);
                BrowserDispatcher(BrowserFuncts.ExecuteCustomScript, cancellationToken, 2, $"document.querySelector('{HtmlTags.Div}[data-test-id=\"boardActionsButton\"]').childNodes[0].childNodes[0].click();", 2);
                DelayBeforeOperation(2000);
                BrowserDispatcher(BrowserFuncts.ExecuteCustomScript, cancellationToken, 2, $"document.querySelectorAll('{HtmlTags.Span}[title=\"Board\"]')[0].click();",2);
                DelayBeforeOperation(2000);
                var X = BrowserDispatcher(BrowserFuncts.ExecuteCustomScript, cancellationToken, 2, string.Format(PDClassesConstant.ScriptConstant.ScriptWithQuerySelectorToGetXandY,"","for", "boardEditName","x"), 2).Result;
                var Y = BrowserDispatcher(BrowserFuncts.ExecuteCustomScript, cancellationToken, 2, string.Format(PDClassesConstant.ScriptConstant.ScriptWithQuerySelectorToGetXandY,"","for", "boardEditName","y"), 2).Result;
                DelayBeforeOperation(2000);
                BrowserDispatcher(BrowserFuncts.MouseClick, cancellationToken, 2,Convert.ToInt32(X),Convert.ToInt32(Y));
                DelayBeforeOperation(2000);
                BrowserDispatcher(BrowserFuncts.EnterChars, cancellationToken, 2, "  " + boardInfo.BoardName);
                if(boardInfo.KeepBoardSecret)
                    BrowserDispatcher(BrowserFuncts.ExecuteCustomScript, cancellationToken, 2, "document.querySelector('label[for=\"secret\"]').click();", 2);
                var ClickIndex = account.DisplayColumnValue11.Equals("Business Mode") ? 1 : 0;
                BrowserDispatcher(BrowserFuncts.ClickByAttributeName, cancellationToken, 2, ActType.GetValue, AttributeType.TagName,
                       HtmlTags.Button, ValueTypes.InnerText, "Create",ClickIndex);
                DelayBeforeOperation(2000);
                var pageSource = BrowserWindows.Last().GetPageSource();
                if(pageSource!=null && pageSource.Contains("Create board"))
                    BrowserDispatcher(BrowserFuncts.ClickByAttributeName, cancellationToken, 2, ActType.GetValue, AttributeType.TagName,
                       HtmlTags.Button, ValueTypes.InnerText, "Create", 0);
                if (pageSource.Contains("Board could not be saved. Try a different board name (you've already got one like this)."))
                {
                    return new BoardResponse(new ResponseParameter { Response = string.Empty })
                    { Success = false, Issue = new PinterestIssue { Message = "LangKeyBoardCouldNotBeSavedTryDifferentBoardName".FromResourceDictionary() } };
                }
                BrowserDispatcher(BrowserFuncts.GetCurrentUrl, cancellationToken, 2);

                if (!CurrentData.Contains("boards"))
                {
                    BrowserDispatcher(BrowserFuncts.ClickByAttributeName, cancellationToken, 2, ActType.GetAttribute,
                    AttributeType.TagName, HtmlTags.Button, ValueTypes.AriaLabel,"Done");
                    DelayBeforeOperation(2000);
                    var CreateBoardPageSource = BrowserWindows.Last().GetPageSource();
                    if (CreateBoardPageSource.Contains("Create menu dropdown") || CreateBoardPageSource.Contains("Create menu drop-down") || CreateBoardPageSource.Contains("addPinButton"))
                        BrowserDispatcher(BrowserFuncts.ClickByAttributeName, cancellationToken, 2, ActType.GetAttribute,
                        AttributeType.TagName, HtmlTags.Button, ValueTypes.AriaLabel, "More Board options");
                    else
                        BrowserDispatcher(BrowserFuncts.ClickByAttributeName, cancellationToken, 2, ActType.GetAttribute,
                        AttributeType.TagName, HtmlTags.Button, ValueTypes.AriaLabel, "More board options");
                    if(!BrowserWindows.Last().GetPageSource().Contains("Edit board"))
                        BrowserDispatcher(BrowserFuncts.ClickByAttributeName, cancellationToken, 2, ActType.GetAttribute,AttributeType.TagName, HtmlTags.Button, ValueTypes.AriaLabel, "More board options");
                    if(!BrowserWindows.Last().GetPageSource().Contains("Edit board"))
                        BrowserDispatcher(BrowserFuncts.ClickByAttributeName, cancellationToken, 2, ActType.GetAttribute,
                        AttributeType.TagName, HtmlTags.Button, ValueTypes.AriaLabel, "More Board options");
                    DelayBeforeOperation(2000);
                    BrowserDispatcher(BrowserFuncts.ExecuteCustomScript, cancellationToken, 0, $"document.querySelectorAll('{HtmlTags.Span}[title=\"Edit board\"]')[0].click();", 2);
                    DelayBeforeOperation(2000);
                    if (!string.IsNullOrEmpty(boardInfo.BoardDescription))
                    {
                        X = BrowserDispatcher(BrowserFuncts.ExecuteCustomScript, cancellationToken, 2, string.Format(PDClassesConstant.ScriptConstant.ScriptWithQuerySelectorToGetXandY, "", "for", "boardEditDescription", "x"), 2).Result;
                        Y = BrowserDispatcher(BrowserFuncts.ExecuteCustomScript, cancellationToken, 2, string.Format(PDClassesConstant.ScriptConstant.ScriptWithQuerySelectorToGetXandY, "", "for", "boardEditDescription", "y"), 2).Result;
                        BrowserDispatcher(BrowserFuncts.MouseClick, cancellationToken, 2,Convert.ToInt32(X),Convert.ToInt32(Y));
                        BrowserDispatcher(BrowserFuncts.EnterChars, cancellationToken, 2, boardInfo.BoardDescription);
                    }
                    DelayBeforeOperation(2000);
                    BrowserDispatcher(BrowserFuncts.ExecuteCustomScript, cancellationToken, 0, $"document.querySelectorAll('{HtmlTags.Button}[type=\"submit\"]')[0].click();", 2);
                    DelayBeforeOperation(2000);
                    pageSource = BrowserWindows.Last().GetPageSource();
                    var boardResponse = BrowserWindows.Last().GetPaginationData("\"resource\":{\"name\":\"BoardFeedResource\"", true).Result;
                    return new BoardResponse(new ResponseParameter { Response = boardResponse });
                }
                else
                {
                    BrowserDispatcher(BrowserFuncts.GetPageSource, cancellationToken, 2);
                    if (CurrentData.Contains("LangKeyBoardCouldNotBeSavedTryDifferentBoardName".FromResourceDictionary()))
                    {
                        return new BoardResponse(new ResponseParameter { Response = string.Empty })
                        { Success = false, Issue = new PinterestIssue { Message = "LangKeyBoardCouldNotBeSavedTryDifferentBoardName".FromResourceDictionary() } };
                    }
                    return new BoardResponse(new ResponseParameter { Response = string.Empty }) { Success = false };
                }
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception)
            {
                return new BoardResponse(new ResponseParameter { Response = string.Empty }) { Success = false };
            }
        }

        public SendBoardInvitationResponseHandler SendBoardInvitation(DominatorAccountModel account, PinterestBoard board, CancellationTokenSource cancellationToken)
        {
            string url = $"https://{Domain}{board.BoardUrl}";
            BrowserDispatcher(BrowserFuncts.GoToUrl, cancellationToken, 0, url, "mostRecentBoard");

            try
            {
                string pageSource = BrowserWindows.Last().GetPageSource();
                if (pageSource.Contains("Invite collaborators. Opens a modal.") && !pageSource.Contains("Search by name or email address"))
                {
                    BrowserWindows.Last().ExecuteScript($"document.querySelector('div[aria-label=\"Invite collaborators. Opens a modal.\"]').click();");
                    DelayBeforeOperation(2000);
                    pageSource = BrowserWindows.Last().GetPageSource();
                }
                else if (pageSource.Contains("Board with collaborators. Keep inviting collaborators. Opens a modal.") && !pageSource.Contains("Search by name or email address"))
                {
                    BrowserWindows.Last().ExecuteScript($"document.querySelector('div[aria-label=\"Board with collaborators. Keep inviting collaborators. Opens a modal.\"]').click();");
                    DelayBeforeOperation(2000);
                    pageSource = BrowserWindows.Last().GetPageSource();
                }
                
                if (pageSource.Contains("Invite collaborators") && !pageSource.Contains("Search by name or email address"))
                {
                    BrowserWindows.Last().ExecuteScript($"document.querySelector('button[aria-label=\"Invite collaborators\"]').click();");
                    DelayBeforeOperation(2000);
                    pageSource = BrowserWindows.Last().GetPageSource();
                }
                string searchUserClass = BrowserUtilities.GetPath(pageSource,HtmlTags.Input, "Search by name or email");
                BrowserDispatcher(BrowserFuncts.GetXY, cancellationToken, 2, AttributeType.ClassName, searchUserClass);
                BrowserDispatcher(BrowserFuncts.MouseClick, cancellationToken, 2, _lastXandY.Key + 50, _lastXandY.Value + 5, Constants.GetScreenResolution().Key, Constants.GetScreenResolution().Value);

                DelayBeforeOperation(2000);
                var emailseparte = board.EmailToCollaborate.Split('@');
                BrowserDispatcher(BrowserFuncts.EnterChars, cancellationToken, 2, " " + emailseparte[0]);
                pageSource = BrowserWindows.Last().GetPageSource();
                DelayBeforeOperation(5000);
                if (pageSource.Contains("Invited"))
                    return new SendBoardInvitationResponseHandler(new ResponseParameter { Response = pageSource });
                DelayBeforeOperation(5000);
                BrowserWindows.Last().ClearResources();
                var Nodes = HtmlAgilityHelper.GetListInnerHtmlOrInnerTextOrOuterHtmlFromIdOrClass(BrowserWindows.Last().GetPageSource(), "", true, "JlN zDA IZT CKL eSP dyH p3S bkV NoJ");
                var ClickIndex = Nodes.Count > 0 ?Nodes.IndexOf(Nodes.FirstOrDefault(x=>string.Equals(x,emailseparte.First(),StringComparison.OrdinalIgnoreCase))): 0;
                BrowserDispatcher(BrowserFuncts.ClickByAttributeName, cancellationToken, 2, ActType.GetValue, AttributeType.TagName,
                    HtmlTags.Button, ValueTypes.InnerText, "Invite",ClickIndex>0?ClickIndex-1:ClickIndex, 2000);


                DelayBeforeOperation(3000);
                string response = BrowserWindows.Last().GetPaginationData("BoardInvitesResource", true).Result;
                if (string.IsNullOrEmpty(response))
                {
                    BrowserWindows.Last().ClearResources();
                    BrowserDispatcher(BrowserFuncts.ClickByAttributeName, cancellationToken, 2, ActType.GetValue, AttributeType.TagName,
                        HtmlTags.Button, ValueTypes.InnerText, "Invite", 1, 2000);
                    DelayBeforeOperation(3000);
                    response = BrowserWindows.Last().GetPaginationData("BoardInvitesResource", true).Result;
                }

                return new SendBoardInvitationResponseHandler(new ResponseParameter { Response = response });
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception)
            {
                return new SendBoardInvitationResponseHandler(new ResponseParameter { Response = string.Empty }) { Success = false };
            }
        }

        public List<PinterestBoard> SearchBoardsByKeyword(DominatorAccountModel account, string keyWord, CancellationTokenSource cancellationToken,
            bool isScroll = false, int scroll = 10)
        {
            var lstPinBoards = new List<PinterestBoard>();
            try
            {
                if (!isScroll)
                {
                    var key = Uri.EscapeDataString(keyWord);
                    if (string.IsNullOrEmpty(Domain))
                        Domain = "www.pinterest.com";
                    string url = $"https://{Domain}/search/boards/?q={key}&rs=filter";
                    BrowserDispatcher(BrowserFuncts.GoToUrl, cancellationToken, 0, url);
                    DelayBeforeOperation(5000);
                    lstPinBoards.AddRange(new BoardsWithKeywordsPtResponseHandler(new ResponseParameter { Response = PdRequestHeaderDetails.GetJsonString(CurrentData, true) }).BoardsList);
                }
                if (scroll > 0)
                {
                    var responseList = ScrollWindowAndGetData(account, scroll,
                        "\"path\":\"/resource/BaseSearchResource/get/\"", cancellationToken);
                    foreach (string response in responseList)
                        lstPinBoards.AddRange(new BoardsWithKeywordsPtResponseHandler(new ResponseParameter { Response = response }).BoardsList);
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
            return lstPinBoards;
        }

        public List<PinterestPin> SearchPinsOfBoard(DominatorAccountModel account, string boardUrl, CancellationTokenSource cancellationToken,
            bool isScroll = false, int scroll = 10)
        {
            List<PinterestPin> lstPinterestPins = new List<PinterestPin>();
            try
            {
                OpenBrowser(account, boardUrl, cancellationToken);
                BrowserWindows.Last().ClearResources();
                if (!isScroll)
                {
                    if (!boardUrl.Contains("http"))
                        boardUrl = $"https://{Domain}{boardUrl}";
                    if (!boardUrl.Contains(Domain))
                        boardUrl = String.Concat("https://", Regex.Replace(boardUrl, "https://[^/]*", Domain));
                    BrowserDispatcher(BrowserFuncts.GoToUrl, cancellationToken, 0, boardUrl, "mostRecentBoard");
                    string response = string.Empty;
                    for (int i = 0; i < 5; i++)
                    {
                        response = BrowserWindows.Last().GetPaginationData("\"resource\":{\"name\":\"BoardFeedResource\"", true).Result;
                        if (!string.IsNullOrEmpty(response))
                            break;
                        DelayBeforeOperation(5000);
                    }
                    lstPinterestPins.AddRange(new PinsByBoardUrlResponseHandler(new ResponseParameter { Response = response }).LstBoardPin);
                }

                if (scroll > 0)
                {
                    List<string> responseList = ScrollWindowAndGetData(account, scroll,
                        "\"resource\":{\"name\":\"BoardFeedResource\"", cancellationToken);

                    foreach (string response in responseList)
                    {
                        lstPinterestPins.AddRange(new PinsByBoardUrlResponseHandler(new ResponseParameter { Response = response }).LstBoardPin);
                    }
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
            return lstPinterestPins;
        }

        public List<PinterestPin> SearchPinsOfUser(DominatorAccountModel account, string user, CancellationTokenSource cancellationToken,
            bool isScroll = false, int scroll = 10)
        {
            List<PinterestPin> lstPinterestPins = new List<PinterestPin>();
            try
            {
                if (!isScroll)
                {
                    string userUrl = user;
                    if (string.IsNullOrEmpty(user))
                        user = account.AccountBaseModel.UserFullName;
                    if (!user.Contains("https:"))
                        userUrl = $"https://{Domain}/{user}/pins";
                    else
                        userUrl = $"https://{Domain}/{user.Split('/')[3]}/pins";

                    user = userUrl.Split('/')[3];

                    BrowserWindows.Last().ClearResources();
                    BrowserDispatcher(BrowserFuncts.GoToUrl, cancellationToken, 0, userUrl, "mostRecentBoard");

                    List<string> lstResponse = new List<string>();

                    for (int i = 0; i < 5; i++)
                    {
                        lstResponse = BrowserWindows.Last().GetPaginationDataList("\"path\":\"/resource/UserPinsResource/get/\"", true).Result;
                        if (lstResponse.Count >= 2)
                            break;
                        DelayBeforeOperation(3000);
                    }

                    //For BussinessProfil User Pins
                    if (lstResponse.Count == 0)
                        lstResponse = BrowserWindows.Last().GetPaginationDataList("\"path\":\"/resource/BusinessProfileUserPinsResource/get/\"", true).Result;

                    foreach (string response in lstResponse)
                        lstPinterestPins.AddRange(new PinsFromSpecificUserResponseHandler(new ResponseParameter { Response = response }).LstUserPin);
                }

                if (scroll > 0)
                {
                    List<string> responseList = ScrollWindowAndGetData(account, scroll, "\"path\":\"/resource/UserPinsResource/get/\"", cancellationToken);

                    //For BussinessProfil User Pins
                    if (responseList.Count == 0)
                        responseList = BrowserWindows.Last().GetPaginationDataList("\"path\":\"/resource/BusinessProfileUserPinsResource/get/\"", true).Result;

                    foreach (string response in responseList)
                    {
                        lstPinterestPins.AddRange(new PinsFromSpecificUserResponseHandler(new ResponseParameter { Response = response }).LstUserPin);
                    }
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
            return lstPinterestPins;
        }

        public List<PinterestUser> SearchUsersByKeyword(DominatorAccountModel account, string keyWord, CancellationTokenSource cancellationToken,
            bool isScroll = false, int scroll = 10)
        {
            List<PinterestUser> LstPinUser = new List<PinterestUser>();
            try
            {
                BrowserWindows.Last().ClearResources();
                if (!isScroll)
                {
                    var key = Uri.EscapeDataString(keyWord);
                    string url = $"https://{Domain}/search/users/?q=" + key;
                    BrowserDispatcher(BrowserFuncts.GoToUrl, cancellationToken, 0, url, account.AccountBaseModel.ProfileId);
                    DelayBeforeOperation(2000);
                    LstPinUser.AddRange(new SearchPeopleResponseHandler(new ResponseParameter { Response = CurrentData }).UsersList);
                }

                if (scroll > 0)
                {
                    List<string> responseList = ScrollWindowAndGetData(account, scroll,
                        "\"path\":\"/resource/BaseSearchResource/get/\"", cancellationToken);

                    foreach (string response in responseList)
                    {
                        LstPinUser.AddRange(new FollowerAndFollowingPtResponseHandler(new ResponseParameter { Response = response }, account.AccountBaseModel.ProfileId).UsersList);
                    }
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
            return LstPinUser;
        }

        public List<PinterestUser> GetUsersWhoTriedPin(DominatorAccountModel account, string pinId, CancellationTokenSource cancellationToken,
            bool isScroll = false, int scroll = 10)
        {
            List<PinterestUser> lstPinterestUser = new List<PinterestUser>();
            try
            {
                BrowserWindows.Last().ClearResources();

                if (!isScroll)
                {
                    string pinUrl = string.Empty;
                    if (!pinId.Contains("pinterest"))
                        pinUrl = $"https://{Domain}/pin/{pinId}/activity/tried";
                    else
                        pinUrl = $"https://{Domain}/pin/{pinId.Split('/')[4]}/activity/tried";

                    pinId = pinUrl.Split('/')[4];
                    BrowserDispatcher(BrowserFuncts.GoToUrl, cancellationToken, 0, pinUrl, "mostRecentBoard");

                    DelayBeforeOperation(2000);
                    if (!CurrentData.Contains("tried"))
                        return lstPinterestUser;

                    string jsonResponse = Utilities.GetBetween(CurrentData, PdConstants.ApplicationJsonDoubleInitialStateSingle, PdConstants.Script);
                    if (string.IsNullOrEmpty(jsonResponse))
                        jsonResponse = Utilities.GetBetween(CurrentData, PdConstants.ApplicationJsonDoubleInitialStateDouble, PdConstants.Script);
                    if (string.IsNullOrEmpty(jsonResponse))
                        jsonResponse = Utilities.GetBetween(CurrentData, PdConstants.InitialStateDoubleApplicationJsonDouble, PdConstants.Script);
                    if (string.IsNullOrEmpty(jsonResponse))
                        jsonResponse = Utilities.GetBetween(CurrentData, "script id=\"__PWS_DATA__\" type=\"application/json\">", "</script><script");

                    lstPinterestUser.AddRange(new PinLikersResponseHandler(new ResponseParameter { Response = jsonResponse }).UserList);

                    List<string> lstResponse = new List<string>();

                    for (int i = 0; i < 5; i++)
                    {
                        lstResponse = BrowserWindows.Last().GetPaginationDataList("\"path\":\"/resource/AggregatedActivityFeedResource/get/\"", true).Result;
                        if (lstResponse.Count >= 2)
                            break;
                        DelayBeforeOperation(3000);
                    }
                    foreach (string response in lstResponse)
                        lstPinterestUser.AddRange(new PinLikersResponseHandler(new ResponseParameter { Response = response }).UserList);
                }

                if (scroll > 0)
                {
                    List<string> responseList = ScrollWindowAndGetData(account, scroll,
                        "\"path\":\"/resource/AggregatedActivityFeedResource/get/\"", cancellationToken);

                    foreach (string response in responseList)
                    {
                        lstPinterestUser.AddRange(new PinLikersResponseHandler(new ResponseParameter { Response = response }).UserList);
                    }
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
            return lstPinterestUser;
        }

        public BoardInvitationsResponseHandler SearchBoardsToAcceptInvitaion(DominatorAccountModel account, CancellationTokenSource cancellationToken)
        {
            try
            {
                BrowserWindows.Last().ClearResources();
                BrowserDispatcher(BrowserFuncts.GoToUrl, cancellationToken, 0, $"https://{Domain}", "mostRecentBoard");
                BrowserDispatcher(BrowserFuncts.ExecuteCustomScript, cancellationToken, 0, paramS: "document.querySelector('button[aria-label=\"Messages\"]').click();");
                DelayBeforeOperation(2000);
                if (BrowserDispatcher(BrowserFuncts.ExecuteCustomScript, cancellationToken, 2, "[...document.querySelectorAll('div[role=\"button\"]')].filter(x=>x.innerText.includes(\"View all requests\"))[0].click();", 2).Success)
                    DelayBeforeOperation(2000);
                var response = BrowserWindows.Last().GetPaginationData("\"resource\":{\"name\":\"BoardInvitesResource\"", true).Result;
                BoardInvitationsResponseHandler boardResponse = new BoardInvitationsResponseHandler(new ResponseParameter { Response = response }, true);

                response = BrowserWindows.Last().GetPaginationData("\"resource\":{\"name\":\"ContactRequestsResource\"", true).Result;
                boardResponse = new BoardInvitationsResponseHandler(new ResponseParameter { Response = response }, false, boardResponse);

                return boardResponse;
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

        public FindMessageResponseHandler SearchForMessagesToReply(DominatorAccountModel account, CancellationTokenSource cancellationToken,
            bool isScroll = false)
        {
            try
            {
                BrowserWindows.Last().ClearResources();
                string response = string.Empty;
                if (!isScroll)
                {
                    BrowserDispatcher(BrowserFuncts.ClickByAttributeName, cancellationToken, 2, ActType.GetAttribute,
                    AttributeType.TagName,HtmlTags.Div, ValueTypes.AriaLabel, "Messages");
                    DelayBeforeOperation(3000);

                    response = BrowserWindows.Last().GetPaginationData("\"resource\":{\"name\":\"ConversationsResource\"", true).Result;
                }
                else
                {
                    BrowserDispatcher(BrowserFuncts.GetElementValue, cancellationToken, 2, ActType.GetLength, AttributeType.ClassName,
                        "ConversationListItemReact");
                    int index = 0;
                    int.TryParse(CurrentData, out index);

                    if (index > 0)
                        BrowserDispatcher(BrowserFuncts.BrowserAct, cancellationToken, 2, ActType.ScrollIntoView, AttributeType.ClassName,
                           "ConversationListItemReact", "", index - 1);

                    DelayBeforeOperation(3000);
                    response = BrowserWindows.Last().GetPaginationData("\"resource\":{\"name\":\"ConversationsResource\"", true).Result;
                }
                return new FindMessageResponseHandler(new ResponseParameter { Response = response });
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return new FindMessageResponseHandler(new ResponseParameter { Response = "" }) { Success = false };
        }

        public List<PinterestPin> SearchNewsFeedPins(DominatorAccountModel account, CancellationTokenSource cancellationToken,
            bool isScroll = false, int scroll = 10)
        {
            List<PinterestPin> lstPinterestPins = new List<PinterestPin>();
            try
            {
                BrowserWindows.Last().ClearResources();
                BrowserWindows.Last().SetResourceLoadInstance();
                if (!isScroll)
                {

                    BrowserDispatcher(BrowserFuncts.GoToUrl, cancellationToken, 0, $"https://{Domain}", "mostRecentBoard");

                    string response = string.Empty;
                    for (int i = 0; i < 5; i++)
                    {
                        response = BrowserWindows.Last().GetPaginationData("\"data\":{\"UserHomefeedResource\"", true).Result;

                        if (!string.IsNullOrEmpty(response))
                        {
                            response = Utilities.GetBetween(response, PdConstants.ApplicationJsonDoubleInitialStateDouble, PdConstants.Script).Trim();
                            break;
                        }
                        DelayBeforeOperation(3000);
                    }
                    lstPinterestPins.AddRange(new NewsFeedPinsResponseHandler(new ResponseParameter { Response = response }).PinList);
                }

                if (scroll > 0)
                {
                    List<string> responseList = ScrollWindowAndGetData(account, scroll,
                        "\"path\":\"/resource/UserHomefeedResource/get/\"", cancellationToken);

                    foreach (string response in responseList)
                    {
                        lstPinterestPins.AddRange(new NewsFeedPinsResponseHandler(new ResponseParameter { Response = response }).PinList);
                    }
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
            return lstPinterestPins;
        }
        public PinPostStatusResponseHandler IsValidImage(string PageResponse)
        {
            var response =string.IsNullOrEmpty(PageResponse)?string.Empty:PageResponse.Contains(PdConstants.InvalidImage) ? PdConstants.InvalidImage :PageResponse.Contains(PdConstants.TooSmallImage)?PdConstants.TooSmallImage:PageResponse.Contains(PdConstants.TooSmallImage1)?PdConstants.TooSmallImage1:PageResponse.Contains(PdConstants.WrongImageFormat)?PdConstants.WrongImageFormat:PageResponse.Contains(PdConstants.WrongImageFormat1)?PdConstants.WrongImageFormat1:string.Empty;
            return new PinPostStatusResponseHandler() { Response = response, status = string.IsNullOrEmpty(response)};
        }

        public PinPostStatusResponseHandler SwithcToBusinessAccount(DominatorAccountModel dominatorAccount, IPdBrowserManager browserManager)
        {
            PinPostStatusResponseHandler Response = null;
            try
            {
                var IsCompleted = false;
                var HomePageUrl = PdConstants.GetHomePageUrl(PdConstants.IsBusinessAccount(dominatorAccount), Domain);
                var CancellationResource = dominatorAccount.CancellationSource;
                browserManager.OpenBrowser(dominatorAccount,HomePageUrl, dominatorAccount.CancellationSource);
                DelayBeforeOperation(10000);
                BrowserDispatcher(BrowserFuncts.GoToUrl,CancellationResource, 2, HomePageUrl);
                DelayBeforeOperation(10000);
                var pageSource= browserManager.BrowserWindows.Last().GetPageSource();
                if (pageSource.Contains("Business Hub"))
                    goto AlreadyHasBusinessAccount;
                if (pageSource.Contains("Build your profile"))
                    goto BuildProfile;
                BrowserDispatcher(BrowserFuncts.ExecuteCustomScript, CancellationResource, 3, string.Format(PDClassesConstant.ScriptConstant.ScriptWithQuerySelectorToClick, HtmlTags.Button, HtmlTags.HtmlAttribute.AriaLabel, "Accounts and more options"));
                DelayBeforeOperation(5000);
                pageSource = browserManager.BrowserWindows.Last().GetPageSource();
                if (!pageSource.Contains("header-menu-business-account"))
                {
                    Response= new PinPostStatusResponseHandler() { status = false, Response = "Already Have Business Account." };
                    return Response;
                }
                BrowserDispatcher(BrowserFuncts.ExecuteCustomScript, CancellationResource, 3, string.Format(PDClassesConstant.ScriptConstant.ScriptWithQuerySelectorToClick, HtmlTags.Div, HtmlTags.HtmlAttribute.DataTestId, "header-menu-business-account"));
                DelayBeforeOperation(10000);
                BrowserDispatcher(BrowserFuncts.ExecuteCustomScript, CancellationResource, 2, string.Format(PDClassesConstant.ScriptConstant.ScrollWindowByXXPixel, 0, 4000));
                DelayBeforeOperation(5000);
                BrowserDispatcher(BrowserFuncts.ExecuteCustomScript, CancellationResource, 2, string.Format(PDClassesConstant.ScriptConstant.ScriptGetElementsByClassNameToClick, "RCK Hsu USg adn CCY gn8 L4E kVc oRi lnZ wsz YbY",1));
                DelayBeforeOperation(8000);
                BuildProfile:
                BrowserDispatcher(BrowserFuncts.GetXY, CancellationResource, 2, AttributeType.Id, "businessName");
                BrowserDispatcher(BrowserFuncts.MouseClick, CancellationResource,2, _lastXandY.Key+5, _lastXandY.Value+5);
                DelayBeforeOperation(3000);
                BrowserDispatcher(BrowserFuncts.EnterChars, CancellationResource,2, dominatorAccount.AccountBaseModel.UserFullName + "_Business");
                DelayBeforeOperation(2000);
                BrowserDispatcher(BrowserFuncts.ExecuteCustomScript, CancellationResource, 2, string.Format(PDClassesConstant.ScriptConstant.ScriptWithQuerySelectorToClick,"label","for", "websiteNotYet"));
                DelayBeforeOperation(2000);
                BrowserDispatcher(BrowserFuncts.ExecuteCustomScript, CancellationResource, 2, string.Format(PDClassesConstant.ScriptConstant.ScrollWindowByXXPixel, 0, 4000));
                DelayBeforeOperation(2000);
                BrowserDispatcher(BrowserFuncts.ExecuteCustomScript, CancellationResource, 2, string.Format(PDClassesConstant.ScriptConstant.ScriptGetElementsByClassNameToClick, "RCK Hsu USg adn CCY gn8 L4E kVc iyn oRi lnZ wsz YbY", 0));
                DelayBeforeOperation(2000);
                BrowserDispatcher(BrowserFuncts.ExecuteCustomScript, CancellationResource, 2, string.Format(PDClassesConstant.ScriptConstant.ScrollWindowByXXPixel, 0, -3000));
                DelayBeforeOperation(2000);
                BrowserDispatcher(BrowserFuncts.GetXY, CancellationResource, 2, AttributeType.ClassName, "CCY S9z ho- Tbt L4E e8F BG7",0);
                DelayBeforeOperation(2000);
                BrowserDispatcher(BrowserFuncts.MouseClick, CancellationResource, 2, _lastXandY.Key + 5, _lastXandY.Value + 5);
                DelayBeforeOperation(2000);
                var Nodes = HtmlAgilityHelper.GetListInnerHtmlOrInnerTextOrOuterHtmlFromIdOrClass(browserManager.BrowserWindows.Last().GetPageSource(), string.Empty, true, "Jea KS5 s2n urM zI7 iyn Hsu", false);
                var Index = Nodes.Count > 0 ?Nodes.IndexOf(Nodes?.FirstOrDefault(x=>x.Contains("Education"))): 0;
                BrowserDispatcher(BrowserFuncts.BrowserAct, CancellationResource, 2,ActType.Click, AttributeType.ClassName,"Jea KS5 s2n urM zI7 iyn Hsu","",Index);
                DelayBeforeOperation(2000);
                BrowserDispatcher(BrowserFuncts.ExecuteCustomScript, CancellationResource, 2, string.Format(PDClassesConstant.ScriptConstant.ScrollWindowByXXPixel, 0, 3000));
                DelayBeforeOperation(2000);
                BrowserDispatcher(BrowserFuncts.ExecuteCustomScript, CancellationResource, 2, string.Format(PDClassesConstant.ScriptConstant.ScriptWithQuerySelectorToClick, "label", "for", "create_content"));
                BrowserDispatcher(BrowserFuncts.ExecuteCustomScript, CancellationResource, 2, string.Format(PDClassesConstant.ScriptConstant.ScriptWithQuerySelectorToClick, "label", "for", "grow_awareness"));
                BrowserDispatcher(BrowserFuncts.ExecuteCustomScript, CancellationResource, 2, string.Format(PDClassesConstant.ScriptConstant.ScriptWithQuerySelectorToClick, "label", "for", "generate_leads"));
                DelayBeforeOperation(2000);
                GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Pinterest, dominatorAccount.AccountBaseModel.UserName, "Switch To Business", "It's All Done.Please Wait Just A Moment While Finishing The Business Account Conversion Process.");
                BrowserDispatcher(BrowserFuncts.ExecuteCustomScript, CancellationResource, 2, string.Format(PDClassesConstant.ScriptConstant.ScriptGetElementsByClassNameToClick, "RCK Hsu USg adn CCY gn8 L4E kVc oRi lnZ wsz YbY", 1));
                DelayBeforeOperation(4000);
                BrowserDispatcher(BrowserFuncts.ExecuteCustomScript, CancellationResource, 2, string.Format(PDClassesConstant.ScriptConstant.ScriptWithQuerySelectorToClick, "label", "for", "publisher_or_media"));
                DelayBeforeOperation(2000);
                BrowserDispatcher(BrowserFuncts.ExecuteCustomScript, CancellationResource, 2, string.Format(PDClassesConstant.ScriptConstant.ScriptGetElementsByClassNameToClick, "RCK Hsu USg adn CCY gn8 L4E kVc oRi lnZ wsz YbY", 1));
                DelayBeforeOperation(2000);
                BrowserDispatcher(BrowserFuncts.ExecuteCustomScript, CancellationResource, 2, string.Format(PDClassesConstant.ScriptConstant.ScriptWithQuerySelectorToClick, "label", "for", "notSureIntent"));
                DelayBeforeOperation(2000);
                BrowserDispatcher(BrowserFuncts.ExecuteCustomScript, CancellationResource, 2, string.Format(PDClassesConstant.ScriptConstant.ScriptGetElementsByClassNameToClick, "RCK Hsu USg adn CCY gn8 L4E kVc oRi lnZ wsz YbY", 1));
                DelayBeforeOperation(8000);
                for(int i = 0; i <= 2; i++)
                {
                    var JavaScriptResponse = BrowserDispatcher(BrowserFuncts.ExecuteCustomScript, CancellationResource, 3, string.Format(PDClassesConstant.ScriptConstant.ScriptWithQuerySelectorToClick, HtmlTags.Button, HtmlTags.HtmlAttribute.AriaLabel, "Dismiss new user onboarding page"));
                    if (!IsCompleted)
                        IsCompleted = JavaScriptResponse.Success;
                    pageSource = browserManager.BrowserWindows.Last().GetPageSource();
                    if (!pageSource.Contains("Where would you like to start?"))
                        break;
                }
                DelayBeforeOperation(4000);
                pageSource = browserManager.BrowserWindows.Last().GetPageSource();
                AlreadyHasBusinessAccount:
                var IsConverted = pageSource.Contains("Business Hub");
                Response = new PinPostStatusResponseHandler() { status = IsConverted && IsCompleted, Response = IsConverted && IsCompleted ?"Success":"Failed" };
            }
            catch(Exception ex) { ex.DebugLog();Response = new PinPostStatusResponseHandler() { status = false, Response = ex.Message };}
            finally { browserManager.CloseLast(); }
            return Response;
        }

        public void DelayBeforeOperation(int delay = 4000) => Thread.Sleep(delay);
    }
}
