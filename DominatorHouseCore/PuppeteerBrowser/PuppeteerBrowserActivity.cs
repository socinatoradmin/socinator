using DominatorHouseCore.Annotations;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using PuppeteerSharp;
using PuppeteerSharp.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using Application = System.Windows.Application;
using Clipboard = System.Windows.Clipboard;
using IPage = PuppeteerSharp.IPage;

namespace DominatorHouseCore.PuppeteerBrowser
{
    public class PuppeteerBrowserActivity: INotifyPropertyChanged, IDisposable
    {
        #region Properties
        private Dictionary<string, string> Headers = new Dictionary<string, string>();
        private bool IsHeaderNeeded { get; set; } = false;
        public static string ProgramFilesX86 => Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
        public static string ProgramFiles => Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
        public static string AppDataLocalPath => Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        public static string BravePath1 => $"{AppDataLocalPath}\\BraveSoftware\\Brave-Browser\\Application\\brave.exe";
        public static string BravePath2 => $"{ProgramFiles}\\BraveSoftware\\Brave-Browser\\Application\\brave.exe";
        public static string ChromePath1 => $"{ProgramFilesX86}\\Google\\Chrome\\Application\\chrome.exe";
        public static string ChromePath2 => $"{ProgramFiles}\\Google\\Chrome\\Application\\chrome.exe";

        private bool CustomUse { get; }
        private LoginType browserLoginType {  get; }
        private static readonly DateTime DateUtc1970 = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        public bool VerifyingAccount { get; set; }
        private DominatorAccountModel _dominatorAccountModel;

        public bool IsCaptchaSolved { get; set; }
        public string CaptchaResponse { get; set; }
        public IPuppeteerBrowserManager browserManager;
        public DominatorAccountModel DominatorAccountModel
        {
            get => _dominatorAccountModel;
            set
            {
                _dominatorAccountModel = value;
                OnPropertyChanged(nameof(DominatorAccountModel));
            }
        }

        private string _browserName;

        public string BrowserName
        {
            get => _browserName;
            set
            {
                if (value == _browserName)
                    return;
                _browserName = value;
                OnPropertyChanged(nameof(BrowserName));
            }
        }

        private string _searchUrl;

        public string SearchUrl
        {
            get => _searchUrl;
            set
            {
                if (_searchUrl == value)
                    return;
                _searchUrl = value;
                OnPropertyChanged(nameof(SearchUrl));
            }
        }

        private double _searchUrlTextBoxWidth = 830;

        public double SearchUrlTextBoxWidth
        {
            get => _searchUrlTextBoxWidth;
            set
            {
                if (_searchUrlTextBoxWidth == value)
                    return;
                _searchUrlTextBoxWidth = value;
                OnPropertyChanged(nameof(SearchUrlTextBoxWidth));
            }
        }

        public bool BrowserLoginMessage { get; set; } = true;

        public bool IsLoaded { get; set; }

        public bool IsLoggedIn { get; set; }

        private bool IsNeedResourceData { get; set; }


        public IPage Page { get; set; }
        public IBrowser browser { get; set; }
        public HashSet<string> ResponseList { get; set; } = new HashSet<string>();

        public static string AdGuardPath = "C:\\Extensions\\AdGuard";
        public static string RandomUserAgentPath = "C:\\Extensions\\RandomUserAgent";

        public static string DownloadPath = "C:";
        public Stopwatch VideoPlayTime;
        public bool SkipYoutubeAd { get; set; }
        public bool FoundAd { get; set; }

        public static string chromePath_1 = @"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe";

        public static string chromePath_2 = @"C:\Program Files\Google\Chrome\Application\chrome.exe";
        public CancellationTokenSource cancellationToken;

        public event PropertyChangedEventHandler PropertyChanged;
        public string CurrentData { get; set; }
        public bool SetVideoQuality { get; set; }
        public string TargetUrl { get; set; } = string.Empty;
        public bool IsTiktok { get;set; }
        #endregion

        #region Constructor.

        public static PuppeteerBrowserActivity Instance(DominatorAccountModel dominatorAccountModel, bool customUse = false,
            bool skipAd = false, bool isNeedResourceData = false,
            bool browserLoginMessageToDisplay = true, string NetworkDomain = "in.pinterest.com", string targetUrl = "", LoginType loginType = LoginType.AutomationLogin,
            bool IsTiktok=false)
            => new PuppeteerBrowserActivity(dominatorAccountModel, customUse, skipAd, isNeedResourceData, browserLoginMessageToDisplay,
                NetworkDomain, targetUrl, loginType,isTiktok:IsTiktok);
        public PuppeteerBrowserActivity()
        {
            browserManager = browserManager ?? InstanceProvider.GetInstance<IPuppeteerBrowserManager>();
        }
        public PuppeteerBrowserActivity(DominatorAccountModel dominatorAccountModel, bool customUse = false,
            bool skipAd = false, bool isNeedResourceData = false,
            bool browserLoginMessageToDisplay = true,string NetworkDomain= "in.pinterest.com",string targetUrl = "",LoginType loginType = LoginType.AutomationLogin,
            bool isTiktok = false):this()
        {
            IsTiktok = isTiktok;
            browserLoginType = loginType;
            IsNeedResourceData = isNeedResourceData;
            TargetUrl = string.IsNullOrEmpty(targetUrl)? GetNetworkHomePage(dominatorAccountModel.AccountBaseModel.AccountNetwork, NetworkDomain):targetUrl;
            CustomUse = customUse;
            DominatorAccountModel = dominatorAccountModel;
            cancellationToken = DominatorAccountModel.CancellationSource ?? new CancellationTokenSource();
        }

        private string GetNetworkHomePage(SocialNetworks accountNetwork,string NetworkDomain)
        {
            switch(accountNetwork)
            {
                case SocialNetworks.Instagram:
                    return "https://www.instagram.com/";
                case SocialNetworks.YouTube:
                    return "https://www.youtube.com/";
                case SocialNetworks.Tumblr:
                    return "https://www.tumblr.com/dashboard";
                case SocialNetworks.Quora:
                    return "https://www.quora.com/";
                case SocialNetworks.Reddit:
                    return "https://www.reddit.com/login";
                case SocialNetworks.Facebook:
                    return "https://www.facebook.com/";
                case SocialNetworks.Twitter:
                    return "https://twitter.com/home";
                case SocialNetworks.LinkedIn:
                    return "https://www.linkedin.com/feed/";
                case SocialNetworks.Pinterest:
                    return $"https://{NetworkDomain}/";
                case SocialNetworks.TikTok:
                    return "https://www.tiktok.com/foryou?lang=en";
                default:
                    return string.Empty;
            }
        }
        #endregion

        #region Methods.
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
        public static string GetExecutablePath()
        {
            if (File.Exists(chromePath_1))
                return chromePath_1;

            return chromePath_2;
            //return
            //    File.Exists(BravePath1) ? BravePath1 :
            //    File.Exists(BravePath2) ?BravePath2 :
            //    File.Exists(ChromePath1) ? ChromePath1 :
            //    ChromePath2;
        }
        public async Task SendKeyEventAsCharacter(int WinKeyCode = 13, bool IsShiftDown = true, int delayBefore = 1, int delayAtLast = 2)
        {
            if (delayBefore > 0)
                await Task.Delay(TimeSpan.FromSeconds(delayBefore), cancellationToken.Token);


            if (delayAtLast > 0)
                await Task.Delay(TimeSpan.FromSeconds(delayAtLast), cancellationToken.Token);
        }
        public Dictionary<string, string> GetHeaders(bool IsClear = false)
        {
            if (IsClear)
                Headers?.Clear();
            return Headers;
        }
        public void SetHeaders(bool HeaderNeeded = false)
        {
            if (!HeaderNeeded)
                GetHeaders(true);
            this.IsHeaderNeeded = HeaderNeeded;
        }
        public bool IsProxied()
        {
            return !string.IsNullOrEmpty(DominatorAccountModel.AccountBaseModel.AccountProxy.ProxyUsername) &&
                            !string.IsNullOrEmpty(DominatorAccountModel.AccountBaseModel.AccountProxy.ProxyPassword);
        }
        public async Task SetUserAgentAsync(string UserAgent)
        {
            try
            {
                await Page.SetUserAgentAsync(UserAgent);
            }
            catch { }
        }
        public async Task<bool> IsBrowserOpenend()
        {
            try
            {
                var pages = await browser.PagesAsync();
                return pages.Any(x=>!string.IsNullOrEmpty(x?.Url));
            }
            catch { return false; }
        }
        public string GetCurrentPageurl()
        {
            return Page.Url;
        }
        public async Task ClosePageAsync(params string[] urlParams)
        {
            try
            {
                var pages = await browser.PagesAsync();
                foreach(var url in urlParams)
                {
                    var targetPage = pages.FirstOrDefault(x => x.Url == url);
                    if (targetPage != null)
                        await targetPage.CloseAsync();
                }
            }
            catch { }
        }
        public async Task GotoCustomUrl(DominatorAccountModel account, CancellationToken accountCancellationToken, string url = "",int delayInSec=5,string UserAgent="")
        {
            try
            {
                if (!string.IsNullOrEmpty(UserAgent))
                    await SetUserAgentAsync(UserAgent);
                await Page.GoToAsync(url,waitUntil:WaitUntilNavigation.DOMContentLoaded);
                await Task.Delay(TimeSpan.FromSeconds(delayInSec));
            }
            catch (AggregateException e)
            { e.DebugLog(); }
            catch (Exception e)
            { e.DebugLog(); }

        }
        public async Task<string> GetPageSourceAsync()
        {
            try
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
                return await Page.GetContentAsync();
            }
            catch (ArgumentException)
            {
            }
            catch
            {
                // ignored
            }

            return string.Empty;
        }
        public async Task<List<string>> GetListInnerHtml(ActType actType, AttributeType attributeType, string attributeValue,
            ValueTypes valueType = ValueTypes.InnerHtml, string value = "", bool isDefaultTab = false)
        {
            List<string> listNodes = new List<string>();
            if (Page.IsClosed)
                return listNodes;
            try
            {
                int itemCount = actType == ActType.CustomActByQueryType ? int.Parse(await GetElementValueAsync(ActType.GetLengthByCustomQuery, attributeType,
                                attributeValue, valueType, value: value, isDefaultTab: isDefaultTab))
                                : actType == ActType.GetValue ? int.Parse(await GetElementValueAsync(ActType.GetLength, attributeType,
                                attributeValue, valueType, value: value, isDefaultTab: isDefaultTab)) :
                                int.Parse(await GetElementValueAsync(ActType.GetLengthByQuery, attributeType,
                                attributeValue, valueType, value: value, isDefaultTab: isDefaultTab));

                while (itemCount > 0)
                {
                    itemCount--;
                    listNodes.Add(await GetElementValueAsync(actType, attributeType, attributeValue, valueType, value: value
                        , clickIndex: itemCount, isDefaultTab: isDefaultTab));
                }
            }
            catch (Exception)
            {

            }

            return listNodes;
        }
        public async Task<int> GetItemCountInnerHtml(ActType actType, AttributeType attributeType,
            string attributeValue,
            ValueTypes valueType = ValueTypes.InnerHtml, string value = "")
        {
            if (Page.IsClosed)
                return 0;

            var itemCount = actType == ActType.ActByQuery
                ? int.Parse(await GetElementValueAsync(ActType.GetLengthByQuery, attributeType, attributeValue))
                : int.Parse(await GetElementValueAsync(ActType.GetLength, attributeType, attributeValue));

            return itemCount;
        }
        public async Task<int> GetCountInnerHtmlChildElement(ActType actType, AttributeType parentAttributeType,
            string parentAttributeValue, AttributeType childAttributeName, string childAttributeValue,
            ValueTypes valueType = ValueTypes.InnerHtml, double delayBefore = 0, int parentIndex = 0,
            int childIndex = 0)
        {

            if (Page.IsClosed)
                return 0;
            var itemCount = actType == ActType.CustomActByQueryType ? int.Parse(await GetChildElementValueAsync(
                    ActType.GetLengthByCustomQuery, parentAttributeType,
                    parentAttributeValue, childAttributeName, childAttributeValue, valueType, delayBefore, parentIndex,
                    childIndex))
                : actType == ActType.GetValue ? int.Parse(await GetChildElementValueAsync(ActType.GetLength,
                    parentAttributeType,
                    parentAttributeValue, childAttributeName, childAttributeValue, valueType, delayBefore, parentIndex,
                    childIndex)) : int.Parse(await GetChildElementValueAsync(ActType.GetLengthByQuery,
                    parentAttributeType,
                    parentAttributeValue, childAttributeName, childAttributeValue, valueType, delayBefore, parentIndex,
                    childIndex));

            return itemCount;
        }
        public async Task<string> GetChildElementValueAsync(ActType actType, AttributeType parentAttributeType,
            string parentAttributeValue, AttributeType childAttributeName, string childAttributeValue,
            ValueTypes valueType = ValueTypes.InnerHtml, double delayBefore = 0, int parentIndex = 0,
            int childIndex = 0, bool isDefaultTab = false)
        {
            string jsResponse = string.Empty;

            if (delayBefore > 0)
                await Task.Delay(TimeSpan.FromSeconds(delayBefore));

            var currentPage = Page;

            var doc =
                $"document.getElementsBy{parentAttributeType}('{parentAttributeValue}')[{parentIndex}].getElementsBy{childAttributeName}('{childAttributeValue}')[{childIndex}].{valueType.GetDescriptionAttr()}";

            var doc2 =
                $"document.getElementsBy{parentAttributeType}('{parentAttributeValue}')[{parentIndex}].querySelectorAll('[{childAttributeName.GetDescriptionAttr()}=\"{childAttributeValue}\"]').length";

            if (Page.IsClosed) return "";
            try
            {
                switch (actType)
                {
                    case ActType.GetValue:
                        jsResponse = await currentPage.EvaluateExpressionAsync<string>($"document.getElementsBy{parentAttributeType}('{parentAttributeValue}')[{parentIndex}].getElementsBy{childAttributeName}('{childAttributeValue}')[{childIndex}].{valueType.GetDescriptionAttr()}") ?? "";
                        break;
                    case ActType.GetLength:
                        jsResponse = await currentPage.EvaluateExpressionAsync<string>($"document.getElementsBy{parentAttributeType}('{parentAttributeValue}')[{parentIndex}].getElementsBy{childAttributeName}('{childAttributeValue}').length") ?? "0";
                        break;

                    case ActType.GetLengthByQuery:
                        jsResponse = await currentPage.EvaluateExpressionAsync<string>($"document.querySelectorAll('[{parentAttributeType.GetDescriptionAttr()}=\"{parentAttributeValue}\"]')[{parentIndex}].getElementsBy{childAttributeName}('{childAttributeValue}').length") ?? "0";
                        break;
                    case ActType.GetLengthByCustomQuery:
                        jsResponse = await currentPage.EvaluateExpressionAsync<string>($"document.getElementsBy{parentAttributeType}('{parentAttributeValue}')[{parentIndex}].querySelectorAll('[{childAttributeName.GetDescriptionAttr()}=\"{childAttributeValue}\"]').length") ?? "0";
                        break;
                    case ActType.GetAttribute:
                        jsResponse = await currentPage.EvaluateExpressionAsync<string>($"document.getElementsBy{parentAttributeType}('{parentAttributeValue}')[{parentIndex}].getElementsBy{childAttributeName}('{childAttributeValue}')[{childIndex}].getAttribute('{valueType.GetDescriptionAttr()}')") ?? "";
                        break;
                    case ActType.ActByQuery:
                        jsResponse = await currentPage.EvaluateExpressionAsync<string>($"document.querySelectorAll('[{parentAttributeType.GetDescriptionAttr()}=\"{parentAttributeValue}\"]')[{parentIndex}].getElementsBy{childAttributeName}('{childAttributeValue}')[{childIndex}].{valueType.GetDescriptionAttr()}") ?? "";
                        break;
                    default:
                        jsResponse = await currentPage.EvaluateExpressionAsync<string>($"document.getElementsBy{parentAttributeType}('{parentAttributeValue}')[{parentIndex}].querySelectorAll('[{childAttributeName.GetDescriptionAttr()}=\"{childAttributeValue}\"]')[{childIndex}].{valueType.GetDescriptionAttr()}") ?? "";
                        break;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return jsResponse;
        }
        public async Task SetCookies(DominatorAccountModel AccountModel)
        {
            List<CookieParam> cookies = new List<CookieParam>();
            if(AccountModel.AccountBaseModel.AccountNetwork == SocialNetworks.Instagram
                || AccountModel.AccountBaseModel.AccountNetwork == SocialNetworks.TikTok)
            {
                foreach (Cookie cookie in AccountModel.Cookies)
                {
                    var cookieCef = new CookieParam
                    {
                        Name = cookie.Name,
                        Value = cookie.Value,
                        Domain = cookie.Domain,
                        HttpOnly = cookie.HttpOnly,
                    };
                    cookies.Add(cookieCef);
                }
            }
            else
            {
                foreach (Cookie cookie in AccountModel.Cookies)
                {
                    var cookieCef = new CookieParam
                    {
                        Name = cookie.Name,
                        Value = cookie.Value,
                        Domain = cookie.Domain,
                        Path = cookie.Path,
                        Expires = cookie.Expires.Ticks,
                        HttpOnly = cookie.HttpOnly,
                        Secure = cookie.Secure,
                        Url = !string.IsNullOrEmpty(TargetUrl) ? TargetUrl : string.Empty
                    };
                    cookies.Add(cookieCef);
                }
            }
            await Page.SetCookieAsync(cookies.ToArray());

        }
        private async void OnPageRequest(object sender, RequestEventArgs requestEvent)
        {
            try
            {
                if (IsNeedResourceData)
                {
                    if (requestEvent.Request.ResourceType == ResourceType.Xhr || requestEvent.Request.ResourceType == ResourceType.Fetch || (requestEvent.Request.Url.StartsWith("https://www.instagram.com/api/v1/media") || requestEvent.Request.Url.StartsWith(TargetUrl)))
                    {
                        await requestEvent.Request.ContinueAsync();
                        while (requestEvent.Request.Response == null)
                        {
                            await Task.Delay(500, cancellationToken.Token);
                        }
                        ResponseList.Add(await requestEvent.Request.Response.TextAsync());
                    }
                    else
                        await requestEvent.Request.ContinueAsync();
                }else
                    await requestEvent.Request.ContinueAsync();
            }
            catch {}

        }
        public void CloseUnWantedTabs(string value, CancellationTokenSource cancellationTokenSource)
        {
            bool isRunning = true;

            Task.Factory.StartNew(async () =>
            {
            try
            {
                    int count = 0;
                    var pages = await Page.Browser.PagesAsync();
                    foreach (var page in pages)
                {
                        if (pages[pages.ToList().IndexOf(page)].Url.Contains(value))
                            count++;
                    }
                    cancellationTokenSource.Token.ThrowIfCancellationRequested();

                    if (count > 0)
                    {
                        foreach (var page in pages)
                        {
                            var index = pages[pages.ToList().IndexOf(page)].Url;

                            if (pages[pages.ToList().IndexOf(page)].Url.Contains(value))
                                Page = page;
                            else
                                await pages[pages.ToList().IndexOf(page)].CloseAsync();
                    }
                }
                    else
                {
                        Page = pages[0];
                        for (int i = 1; i < pages.Count(); i++)
                            await pages[i].CloseAsync();
                }
                    cancellationTokenSource.Token.ThrowIfCancellationRequested();
                await Task.Delay(1000);
                    isRunning = false;
            }
                catch (OperationCanceledException)
            {
                    isRunning = false;
                throw new OperationCanceledException();
            }
                catch (Exception) { isRunning = false; }
                isRunning = false;
            });

            while (isRunning)
                Task.Delay(500).Wait(cancellationTokenSource.Token);
            }
        public async Task<IPage> LoadBrowser(DominatorAccountModel dominatorAccount, CancellationTokenSource cancellationTokenSource, string Url= "https://www.instagram.com",int timeOut=1200000)
        {
            TargetUrl = Url;
            cancellationToken = cancellationTokenSource;
            if(Page is null && IsTiktok)
            {
                var pages = await browser.PagesAsync();
                Page = pages?.FirstOrDefault() ?? await OpenNewPage();
            }else
                Page = await OpenNewPage();
            //CloseUnWantedTabs(Url, cancellationToken);
            Page.Response += OnPageResponse;
            await Page.SetRequestInterceptionAsync(true);
            Page.DOMContentLoaded += OnPageLoaded;
            Page.Request += OnPageRequest;
            await Page.SetViewportAsync(
                new ViewPortOptions()
                {
                    Width = Convert.ToInt32(SystemParameters.PrimaryScreenWidth),
                    Height = Convert.ToInt32(SystemParameters.PrimaryScreenHeight - 150)
                });
            await SetCookies(dominatorAccount);
            await Page.GoToAsync(Url, timeout: timeOut, waitUntil: new[] { WaitUntilNavigation.Networkidle2, WaitUntilNavigation.DOMContentLoaded });
            try
            {
                var pages = await browser.PagesAsync();
                if (pages != null && pages.Count() > 1)
                {
                    var target = pages.FirstOrDefault(x => x != null && !string.IsNullOrEmpty(x?.Url) && (x.Url == "https://httpbin.org/ip" || x.Url == "about:blank" || x.Url.Contains("https://httpbin.org/ip")
                    || x.Url == "https://ipinfo.io/what-is-my-ip" || x.Url.Contains("https://ipinfo.io/what-is-my-ip") || x.Url.Contains("https://ipinfo.io/json")));
                    if (target != null && !target.IsClosed)
                    {
                        await target.CloseAsync();
                    }
                }
            }
            catch { }

            await Task.Delay(3000);
            return Page;
        }

        private async void OnPageLoaded(object sender, EventArgs e)
        {
            IsLoaded = true;
            await ApplyCss();
        }

        public void Sleep(double seconds = 1)
        {
            try
            {
                Task.Delay(TimeSpan.FromSeconds(seconds)).Wait(cancellationToken.Token);
            }
            catch (OperationCanceledException)
            {

            }
            catch (Exception) { }
        }
        public async void SetVideoQualityAs144P(DominatorAccountModel dominatorAccount)
        {
            try
            {
                CurrentData = GetCurrentPageurl();
                if (SetVideoQuality ||
                   !Uri.UnescapeDataString(CurrentData.ToLower()).Contains("www.youtube.com/watch?")) return;
                Sleep(4);
                await ClickEvent(AttributeType.ClassName, "ytp-volume-slider", index: 0);
                //Press Down Arrow key 21 times to mute the music
                await PressKeyAsync("ArrowDown", 1, 21, 100);
                new Task(async () =>
                {
                    while (!browser.IsClosed)
                    {
                        Sleep(3);
                        if (!browser.IsClosed) CheckAd();
                        if (!browser.IsClosed && SkipYoutubeAd && FoundAd)
                            await ClickEvent(AttributeType.ClassName, "ytp-ad-skip-button-icon", index: 0);
                    }
                }).Start();
                Sleep(3.5);
                if (FoundAd)
                    while (FoundAd)
                        Sleep(3.5);
                else
                    PlayPauseVideo(dominatorAccount);

                VideoPlayTime = new Stopwatch();
                VideoPlayTime.Start();

                #region decreasing video to minimum  quality 
                // Click YouTube MediaPlayer Setting Button
                await ClickEvent(AttributeType.ClassName, "ytp-button ytp-settings-button", index: 0);

                await PressKeyAsync("ArrowDown", 1, 4, 100); //Press Down Arrow key 4 times
                await PressKeyAsync("ArrowRight", 1, 4, 0); //Press right Arrow key 1 time
                await PressKeyAsync("ArrowDown", 1, 6, 100); //Press Down Arrow key 6 times
                await PressKeyAsync("ArrowUp", 1, 1, 0); //Press up Arrow key 1 time
                await PressKeyAsync("Enter", 1, 1, 0); //Press Enter key

                SetVideoQuality = true;
            }
            catch (OperationCanceledException)
            {

            }
            catch (AggregateException)
            { }
            catch (Exception)
            {
            }

            #endregion
        }
        public async void PlayPauseVideo(DominatorAccountModel dominatorAccount)
        {
            try
            {
                if (dominatorAccount.IsRunProcessThroughBrowser)
                    await ClickEvent(AttributeType.ClassName, "ytp-play-button ytp-button", index: 0);
                else
                    await MouseClickAsync(540, 400, delayBefore: 1);
            }
            catch (Exception)
            {

            }
        }
        public async void CheckAd()
        {
            var lengthValue = await GetElementValueAsync(ActType.GetLength,
                                  AttributeType.ClassName, "ytp-ad-skip-button-icon");
            FoundAd = lengthValue == "1";
        }
        public void SetResourceLoadInstance(bool IsNeedToLoadResource = true)
        {
            IsNeedResourceData = IsNeedToLoadResource;
        }
        private async void OnPageResponse(object sender, ResponseCreatedEventArgs responseEvent)
        {
            try
            {
                if (IsNeedResourceData)
                {
                    if (responseEvent.Response.Request.ResourceType == ResourceType.Xhr || responseEvent.Response.Request.ResourceType == ResourceType.Fetch || responseEvent.Response.Url.StartsWith(TargetUrl))
                    {
                        while (responseEvent.Response == null)
                        {
                            await Task.Delay(500, cancellationToken.Token);

                        }
                        ResponseList.Add(await responseEvent.Response.TextAsync());
                    }
                }
                if (IsHeaderNeeded)
                {
                    try
                    {
                        var headers = responseEvent.Response.Headers;
                        if (headers != null)
                        {
                            foreach (var header in headers)
                            {
                                if (!Headers.ContainsKey(header.Key))
                                    Headers.Add(header.Key, header.Value);
                            }
                        }
                    }
                    catch { }
                }
            }
            catch (Exception)
            {
            }
        }
        public async Task<bool> LaunchBrowserAsync(bool HeadLess = false,string targetUrl="",bool loadExtension=false,List<string> ExtensionCollection=null,string ProfileDir="")
        {
            var IsLaunched = false;
            try
            {
                if(!string.IsNullOrEmpty(targetUrl))
                    TargetUrl = targetUrl;
                await browserManager.RemoveBrowser(DominatorAccountModel.AccountId+browserLoginType.ToString());
                await LaunchPuppeteerBrowser(DominatorAccountModel.AccountBaseModel.AccountProxy.GetProxy(),cancellationToken, HeadLess,LoadExtension: loadExtension,Extensions:ExtensionCollection,ProfileDir:ProfileDir);
                Page = await LoadBrowser(DominatorAccountModel,cancellationToken,TargetUrl);
                IsLaunched = Page!=null;
            }
            catch { }
            return IsLaunched;
        }
        public async Task<bool> LaunchPuppeteerBrowser(string proxy, CancellationTokenSource token, bool headLess = false,bool AddTestArgument=true,bool LoadExtension=false,List<string> Extensions=null,string ProfileDir="")
        {
            try
            {
                string executablePath = GetExecutablePath();
                await Task.Delay(new Random().Next(1000, 5000));
                token.Token.ThrowIfCancellationRequested();
                LaunchOptions launchOptions = new LaunchOptions
                {
                    Headless = headLess,
                    ExecutablePath = executablePath,
                    Timeout = 60000,
                    DumpIO = true,
                    IgnoredDefaultArgs = new[] {"--enable-automation"}
                };
                var workingArea = Screen.PrimaryScreen.WorkingArea;
                var args = new List<string>
                {
                    "https://ipinfo.io/json",
                    "--start-maximized", // simulate user maximizing the window
                    "--disable-blink-features=AutomationControlled", // critical for Puppeteer detection
                    "--disable-infobars", // removes “Chrome is being controlled...” banner
                    "--enable-features=NetworkService,NetworkServiceInProcess", // sometimes helps prevent detection
                    "--no-sandbox", // needed for most Puppeteer environments
                    "--disable-setuid-sandbox",
                    "--use-gl=desktop", // disables SwiftShader (important for WebGL spoofing)
                    "--enable-webgl",
                    "--ignore-gpu-blacklist", // prevent Chrome from disabling GPU features
                    "--mute-audio", // useful if you're running headless and want no sound

                    // Optional but useful:
                    "--disable-popup-blocking",
                    "--disable-translate",
                    "--no-first-run",
                    "--lang=en-US", // set default language
                    "--disable-translate",//Disable language translate
                    "--disable-features=TranslateUI",
                    //$"--window-size={workingArea.Width},{workingArea.Height}",
                    //$"--window-position={workingArea.X},{workingArea.Y}",

                };
                if (IsTiktok)
                {
                    //args.Add("--incognito");
                }
                if (LoadExtension && Extensions!=null && Extensions.Count > 0)
                {
                    var CapSolverExtension = GetExtensionsCollection(Extensions);
                    args.Add($"--disable-extensions-except={CapSolverExtension}");
                    args.Add($"--load-extension={CapSolverExtension}");
                    args.Add("--enable-automation");
                    launchOptions.IgnoredDefaultArgs = new string[] { };
                }
                else
                {
                    args.Add("--disable-extensions");
                }
                if(!string.IsNullOrEmpty(ProfileDir))
                {
                    args.Add($"--user-data-dir={ProfileDir}");
                }
                if (!string.IsNullOrEmpty(DominatorAccountModel?.AccountBaseModel?.AccountProxy?.ProxyIp))
                    args.Add($"--proxy-server={DominatorAccountModel.AccountBaseModel.AccountProxy.ProxyIp}:{DominatorAccountModel.AccountBaseModel.AccountProxy.ProxyPort}");
                launchOptions.Args = args.ToArray();
                browser = await Puppeteer.LaunchAsync(launchOptions);
                browser.Closed += OnBrowserClosing;
                await browserManager.SaveBrowserAsync(DominatorAccountModel.AccountId+browserLoginType.ToString(),browser);
                token.Token.ThrowIfCancellationRequested();
            }
            catch (OperationCanceledException ex)
            {
                await browser.CloseAsync();
                throw new OperationCanceledException(ex.StackTrace);
            }
            catch (ArgumentException)
            {
                
            }
            catch (Exception)
            {
                
            }
            return true;
        }

        private string GetExtensionsCollection(List<string> extensions)
        {
            var extension = string.Empty;
            extension = string.Join(",", extensions);
            return extension;
        }

        private async void OnBrowserClosing(object sender, EventArgs e)
        {
            try
            {
                await browserManager.RemoveBrowser(DominatorAccountModel.AccountId+browserLoginType.ToString(),true);
            }
            catch { }
        }

        public int GetProcessId()
        {
            return browser.Process.Id;
        }
        public async Task MouseScrollAsync(int xLoc, int yLoc, int scrollByXLoc = 0, int scrollByYLoc = 0,
           double delayBefore = 0, double delayAfter = 0,
           int clickLeavEvent = 0, bool isDefaultPage = false, int scrollCount = 0)
        {

            if (delayBefore > 0)
                await Task.Delay(TimeSpan.FromSeconds(delayBefore), cancellationToken.Token);
            var currentPage = Page;
            if (Page.IsClosed) return;

            try
            {

                await currentPage.Mouse.MoveAsync(xLoc, yLoc, new MoveOptions() { Steps = 10 });
                do
                {
                    await currentPage.Mouse.WheelAsync(xLoc, yLoc);
                    await Task.Delay(TimeSpan.FromSeconds(0.3));
                } while (scrollCount-- > 0);

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            if (delayAfter > 0)
                await Task.Delay(TimeSpan.FromSeconds(delayAfter), cancellationToken.Token);
        }
        public async Task<IPage> OpenNewPage()
        {
            Page = await browser.NewPageAsync();
            if (DominatorAccountModel.AccountBaseModel.AccountNetwork == SocialNetworks.Reddit)
                await Page.SetUserAgentAsync("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/140.0.0.0 Safari/537.36");
            if (!string.IsNullOrEmpty(DominatorAccountModel?.AccountBaseModel?.AccountProxy?.ProxyPassword) && !string.IsNullOrEmpty(DominatorAccountModel?.AccountBaseModel?.AccountProxy?.ProxyUsername))
            {
                await Page.AuthenticateAsync(new Credentials
                {
                    Username = DominatorAccountModel.AccountBaseModel.AccountProxy.ProxyUsername,
                    Password = DominatorAccountModel.AccountBaseModel.AccountProxy.ProxyPassword
                });
            }
            var workingArea = Screen.PrimaryScreen.WorkingArea;
            await Page.SetViewportAsync(new ViewPortOptions
            {
                Width = workingArea.Width,
                Height = workingArea.Height - 150
            });
            await StealthPlugin.ApplyAsync(Page);
            return Page;
        }
        public void Close()
        {
            ClosedBrowser();
        }
        public async void ClosedBrowser()
        {
            if (browser != null)
            {
                await browser.CloseAsync();
            }
        }

        public async Task<string> GetValue(AttributeType attributeType, string attributeValue, int index, ValueTypes valueType=ValueTypes.InnerText)
        {
            try
            {
                return await Page.EvaluateExpressionAsync<string>($"document.getElementsBy{attributeType}('{attributeValue}')[{index}].{valueType}");
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public async Task ChooseFileFromDialog(string path, string button, int count = 0)
        {
            var filechooserdialogtask = Page.WaitForFileChooserAsync();
            await Task.WhenAll(filechooserdialogtask, Page.EvaluateExpressionAsync($"document.getElementsByClassName('{button}')[{count}].click()"));
            var filechooser = await filechooserdialogtask;
            await filechooser.AcceptAsync(path);
        }

        public async Task<string> ClickEvent(AttributeType attributeType, string attributeValue, int index)
        {
            try
            {
                var Response = await Page.EvaluateExpressionAsync<string>($"document.getElementsBy{attributeType}('{attributeValue}')[{index}].click()");
                return Response;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
        public async Task<KeyValuePair<int, int>> GetXAndYByClassNameAsync(AttributeType attributeType = AttributeType.Id,
            string elementName = "", int index = 0)
        {
            var scripty = $"document.getElementsByClassName('{elementName}')[{index}].getBoundingClientRect().y";
            var scriptx = $"document.getElementsByClassName('{elementName}')[{index}].getBoundingClientRect().x";
            var x = await Page.EvaluateExpressionAsync(scriptx);
            var y = await Page.EvaluateExpressionAsync(scripty);
            var xVal = x.ToString();
            var yVal = y.ToString();
            var xAndY = new KeyValuePair<int, int>(Convert.ToInt32(Double.Parse(xVal)), Convert.ToInt32(Double.Parse(yVal)));
            return xAndY;
        }
        public async Task<List<string>> GetListInnerHtmlAsync(ActType actType, AttributeType attributeType, string attributeValue,
            ValueTypes valueType = ValueTypes.InnerHtml, string value = "", bool isDefaultTab = false, Page page = null)
        {
            List<string> listNodes = new List<string>();

            try
            {
                int itemCount = actType == ActType.CustomActByQueryType ? int.Parse(await GetElementValueAsync(ActType.GetLengthByCustomQuery, attributeType,
                                attributeValue, valueType, value: value, isDefaultTab: isDefaultTab))
                                : actType == ActType.GetValue ? int.Parse(await GetElementValueAsync(ActType.GetLength, attributeType,
                                attributeValue, valueType, value: value, isDefaultTab: isDefaultTab)) :
                                int.Parse(await GetElementValueAsync(ActType.GetLengthByQuery, attributeType,
                                attributeValue, valueType, value: value, isDefaultTab: isDefaultTab));

                while (itemCount > 0)
                {
                    itemCount--;
                    listNodes.Add(await GetElementValueAsync(actType, attributeType, attributeValue, valueType, value: value
                        , clickIndex: itemCount, isDefaultTab: isDefaultTab));
                }
            }
            catch (Exception)
            {

            }

            return listNodes;
        }

        public async Task<string> GetElementValueAsync(ActType actType, AttributeType attributeType, string attributeValue,
             ValueTypes valueType = ValueTypes.InnerHtml, double delayBefore = 0, int clickIndex = 0, string value = "", bool isDefaultTab = false)
        {
            //await CheckAndResetBrowser();

            string jsResponse = string.Empty;

            if (delayBefore > 0)
                await Task.Delay(TimeSpan.FromSeconds(delayBefore));

            var currentPage = Page;

            try
            {
                switch (actType)
                {
                    case ActType.GetValue:
                        jsResponse = (await Page.EvaluateExpressionAsync<string>($"document.getElementsBy{attributeType}('{attributeValue}')[{clickIndex}].{valueType}"))?.ToString();
                        break;

                    case ActType.GetLength:
                        jsResponse = (await Page.EvaluateExpressionAsync<int>($"document.getElementsBy{attributeType}('{attributeValue}').length")).ToString();
                        break;

                    case ActType.GetLengthByQuery:
                        return (await Page.EvaluateExpressionAsync<int>($"document.querySelectorAll('[{attributeType}=\"{attributeValue}\"]').length")).ToString();

                    case ActType.GetLengthByCustomQuery:
                        jsResponse = (await Page.EvaluateExpressionAsync<int>($"document.querySelectorAll('[{attributeType}=\"{attributeValue}\"]')[{clickIndex}].{value}.length")).ToString();
                        break;

                    case ActType.GetAttribute:
                        jsResponse = await Page.EvaluateExpressionAsync<string>($"document.getElementsBy{attributeType}('{attributeValue}')[{clickIndex}].getAttribute('{valueType}')");
                        break;

                    case ActType.CustomActByQueryType:
                        jsResponse = await Page.EvaluateExpressionAsync<string>($"document.querySelectorAll('[{attributeType}=\"{attributeValue}\"]')[{clickIndex}].{value}");
                        break;
                    case ActType.CustomActType:
                        jsResponse = await Page.EvaluateExpressionAsync<string>($"document.getElementsBy{attributeType}('{attributeValue}')[{clickIndex}].{value}.{valueType}");
                        break;
                    default:
                        jsResponse = await Page.EvaluateExpressionAsync<string>($"document.querySelectorAll('[{attributeType}=\"{attributeValue}\"]')[{clickIndex}].{valueType}");
                        break;
                }
            }
            catch (Exception)
            {
                jsResponse = string.Empty;
            }

            return jsResponse;
        }
        public async Task PressKeyAsync(string keyCode, double delayAfter, int numCount, int delayBetween)
        {
            var currentPage = Page;
            try
            {
                while (numCount > 0)
                {
                    await currentPage.Keyboard.PressAsync(keyCode, new PressOptions()
                    {
                        Delay = delayBetween
                    });
                    numCount--;
                }
            }
            catch (Exception)
            {

            }
            if (delayAfter > 0)
                await Task.Delay(TimeSpan.FromSeconds(1));
        }
        public async Task MouseScrollByScrollbarAsync(int xLoc, int yLoc, int scrollByXLoc = 0, int scrollByYLoc = 0,
           double delayBefore = 0, double delayAfter = 0, int clickLeavEvent = 0, bool isDefaultPage = false)
        {

            if (delayBefore > 0)
                await Task.Delay(TimeSpan.FromSeconds(delayBefore));

            var currentPage = Page;

            try
            {
                await Page.Mouse.MoveAsync(xLoc, yLoc, new MoveOptions() { Steps = 2 });
                await Page.Mouse.DownAsync(new ClickOptions() { Button = MouseButton.Left });
                await Task.Delay(TimeSpan.FromSeconds(delayAfter));
                await Page.Mouse.UpAsync(new ClickOptions() { Button = MouseButton.Left });
            }
            catch (Exception)
            {

            }

            if (delayAfter > 0)
                await Task.Delay(TimeSpan.FromSeconds(1));
        }

        public void Dispose()
        {
            if(browser!=null)
                browser.Dispose();
        }
        #endregion

        #region CefSharp Utilities
        public void GoToUrl(string url = null)
        {
            Page.GoToAsync(url ?? SearchUrl,waitUntil:WaitUntilNavigation.DOMContentLoaded);
        }
        public void LoadPostPage(bool isLoggedIn)
        {
            if (isLoggedIn)
            {
                Page.GoToAsync(TargetUrl, waitUntil: WaitUntilNavigation.DOMContentLoaded);
            }
        }
        public async Task LoadPostPageAsync(bool isLoggedIn)
        {
            if(isLoggedIn)
                await Page.GoToAsync(TargetUrl, waitUntil: WaitUntilNavigation.DOMContentLoaded);
        }

        public bool IsDisposed => Page is null || Page.IsClosed;

        public void LoadPostPage()
        {
            if (CustomUse || string.IsNullOrEmpty(TargetUrl))
                return;
            var Gone = Page.GoToAsync(TargetUrl, waitUntil: WaitUntilNavigation.DOMContentLoaded).Result;
        }
        /// <summary>
        ///     Get Current PageSource
        /// </summary>
        /// <returns></returns>
        public string GetPageSource()
        {
            return Page.GetContentAsync().Result;
        }

        public void SelectAllText()
        {
            var selected = PressControlWithkey(winkeyCode: 65, delayInSec: 3);
        }

        public async Task<string> PageText()
        {
            try
            {
                return await Page.GetContentAsync();
            }
            catch
            {
                return string.Empty;
            }
        }
        public async Task<bool> SaveCookies(bool showLoginSuccessLog = true)
        {
            if (IsLoggedIn) return false;

            try
            {
                await Task.Delay(1000, cancellationToken.Token);

                IsLoggedIn = true;

                DominatorAccountModel.Cookies = await BrowserCookiesIntoModel();

                DominatorAccountModel.IsUserLoggedIn = true;
                DominatorAccountModel.AccountBaseModel.Status = AccountStatus.Success;

                SocinatorAccountBuilder.Instance(DominatorAccountModel.AccountBaseModel.AccountId)
                    .AddOrUpdateDominatorAccountBase(DominatorAccountModel.AccountBaseModel)
                    .AddOrUpdateLoginStatus(DominatorAccountModel.IsUserLoggedIn)
                    .AddOrUpdateBrowserCookies(DominatorAccountModel.Cookies)
                    .AddOrUpdateCookies(DominatorAccountModel.Cookies)
                    .SaveToBinFile();
                if (showLoginSuccessLog)
                    CustomLog("Browser login successful.");
                return true;
            }
            catch (Exception ex)
            {
                ex.DebugLog(ex.StackTrace);
                return false;
            }
        }
        public bool MoveMouseAtLocation(int xLoc, int yLoc)
        {
            var IsMoved = true;
            try
            {
                PuppeteerWIN32.POINT point = new PuppeteerWIN32.POINT(xLoc, yLoc);
                PuppeteerWIN32.ClientToScreen(PuppeteerWIN32.GetDesktopWindow(), ref point);
                PuppeteerWIN32.SetCursorPos(point.x, point.y);
            }
            catch (Exception) { IsMoved = false; }
            return IsMoved;
        }
        public async Task MultipleMouseHoverAsync(int xLoc, int yLoc, double delayBefore = 0, double delayAfter = 0,
            int MouseHoverCount = 1)
        {
            if (delayBefore > 0)
                await Task.Delay(TimeSpan.FromSeconds(delayBefore), cancellationToken.Token);

            
            if (delayAfter > 0)
                await Task.Delay(TimeSpan.FromSeconds(delayAfter), cancellationToken.Token);
        }
        public async Task<bool> BrowserSaveCookies(bool showLoginSuccessLog = true)
        {
            if (IsLoggedIn) return false;

            try
            {
                await Task.Delay(1000, cancellationToken.Token);

                IsLoggedIn = true;

                DominatorAccountModel.BrowserCookies = await BrowserCookiesIntoModel();

                DominatorAccountModel.AccountBaseModel.Status = AccountStatus.Success;

                SocinatorAccountBuilder.Instance(DominatorAccountModel.AccountBaseModel.AccountId)
                    .AddOrUpdateDominatorAccountBase(DominatorAccountModel.AccountBaseModel)
                    .AddOrUpdateLoginStatus(DominatorAccountModel.IsUserLoggedIn)
                    .AddOrUpdateBrowserCookies(DominatorAccountModel.BrowserCookies)
                    .SaveToBinFile();
                DominatorAccountModel.IsUserLoggedIn = true;
                if (showLoginSuccessLog)
                    CustomLog("Browser Login Successful.");
                return true;
            }
            catch (Exception ex)
            {
                ex.DebugLog(ex.StackTrace);
                return false;
            }
        }

        public async Task<CookieCollection> BrowserCookiesIntoModel()
        {
            try
            {
                var cookieCollection = new CookieCollection();

                foreach (var item in await BrowserCookies())
                    try
                    {
                        var cookie = new System.Net.Cookie
                        {
                            Name = item.Name,
                            Value = item.Value,
                            Domain = item.Domain,
                            Path = item.Path,
                            Secure = item.Secure
                        };
                        if (item.Expires != null)
                            cookie.Expires = (DateTime)item.Expires;
                        else
                            cookie.Expires = DateTime.Now.AddYears(1);

                        cookieCollection.Add(cookie);
                    }
                    catch
                    {
                        /*ignored*/
                    }

                return cookieCollection;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }
        public async Task ClearCookies()
        {
            try
            {
                await Page.DeleteCookieAsync();
            }
            catch { }
        }
        public async Task<List<Cookie>> BrowserCookies()
        {
            var browserCookies = await Page.GetCookiesAsync();
            var listCookies = new List<Cookie>();
            foreach(var cookie in browserCookies)
            {
                listCookies.Add(new Cookie
                {
                    Name = cookie.Name,
                    Value = cookie.Value,
                    Domain = cookie.Domain,
                    Path = cookie.Path,
                    Expires = GetExpires(cookie.Expires),
                    HttpOnly = (bool)cookie.HttpOnly,
                    Secure = (bool)cookie.Secure
                });
            }
            return listCookies;
        }
        private DateTime GetExpires(double? expires)
        {
            try
            {
                return DateUtc1970.AddSeconds((double)expires);
            }
            catch { return DateTime.Now.AddDays(30); }
        }
        #endregion

        #region Window UI Interaction
        public void GoBack(int nTimes = 1)
        {
            while (nTimes > 0)
            {
                //if (!Page.GoBackAsync())
                //    return;
                //Browser.GetBrowser().GoBack();
                nTimes--;
                if (nTimes != 0)
                    Sleep(0.5);
            }
        }

        public void GoForward(int nTimes = 1)
        {
            while (nTimes > 0)
            {
                //if (!Browser.CanGoForward)
                //    return;
                //Browser.Forward();
                nTimes--;
                if (nTimes != 0)
                    Sleep(0.5);
            }
        }

        public void Refresh()
        {
            Page.ReloadAsync();
        }

        #endregion

        #region Browser Automation Changes

        public async void ChooseFileFromDialog(string filePath = "", List<string> pathList = null)
        {
            var fileChooserDialog = Page.WaitForFileChooserAsync();
            var dialog = await fileChooserDialog;
            var medias = pathList == null || pathList.Count == 0 ? new List<string> { filePath } : pathList;
            foreach(var file in medias)
                await dialog.AcceptAsync(file);
        }

        /// <summary>
        ///     Browser actions
        /// </summary>
        /// <param name="actType">Type of activity doing on browser window</param>
        /// <param name="element">type of element by which the action gonna be performed</param>
        /// <param name="delayBefore">delay before the action (In seconds)</param>
        /// <param name="clickIndex">Sometimes multiple buttons have same tag-value</param>
        public string GetElementValue(ActType actType, string element, double delayBefore = 0, int clickIndex = 0)
        {
            if (delayBefore > 0)
                Sleep(delayBefore);

            if (Page.IsClosed) return "";
            switch (actType)
            {
                case ActType.GetValueByName:
                    return Page.EvaluateExpressionAsync($"document.getElementsByName('{element}')[{clickIndex}].value")
                               .Result?.ToString() ?? "";
                case ActType.GetLengthByClass:
                    return Page.EvaluateExpressionAsync($"document.getElementsByClassName('{element}').length").Result
                               ?.ToString() ?? "";
                default:
                    return "";
            }
        }

        /// <summary>
        ///     Press any key n times with delay between each pressed
        /// </summary>
        /// <param name="n">Number of pressing</param>
        /// <param name="delay">Delay between each press  (In milliseconds)</param>
        /// <param name="ke">Browser KeyEvent</param>
        /// <param name="winKeyCode">WindowsKeycode of any key in keyboard</param>
        /// ///
        /// <param name="delayAtLast">Set delay at last (In seconds)</param>
        /// <param name="isShiftDown"></param>
        public void PressAnyKey(int n = 1, double delay = 1,
            int winKeyCode = 0, double delayAtLast = 0, bool isShiftDown = false)
        {
            
            if (delayAtLast > 0)
                Sleep(delayAtLast);
        }

        /// <summary>
        ///     Get the Mouse to click on a specific location(xLoc,yLoc)
        /// </summary>
        /// <param name="xLoc">x-cordinate location</param>
        /// <param name="yLoc">y-cordinate location</param>
        /// <param name="mouseButton">Mouse Button Type</param>
        /// <param name="delayBefore">Delay before click</param>
        /// <param name="delayAfter">Delay after click</param>
        public void MouseClick(int xLoc, int yLoc,
            double delayBefore = 0, double delayAfter = 0, int clickCount = 1)
        {
            if (delayBefore > 0)
                Sleep(delayBefore);
            Page.Mouse.ClickAsync(xLoc, yLoc, new ClickOptions { Count = clickCount });
            if (delayAfter > 0)
                Sleep(delayAfter);
        }

        /// <summary>
        ///     Enter Characters in TextBox
        /// </summary>
        /// <param name="charString">String to be entered</param>
        /// <param name="typingDelay">Delay between typing</param>
        /// <param name="delayBefore">Set delay before the typing</param>
        /// <param name="delayAtLast">Set delay at last</param>
        public void EnterChars(string charString, double typingDelay = 0.09, double delayBefore = 0,
            double delayAtLast = 3)
        {
            if (string.IsNullOrEmpty(charString)) return;

            if (delayBefore > 0)
                Sleep(delayBefore);
            Application.Current.Dispatcher.InvokeAsync(async () =>
            {
                await Page.Keyboard.TypeAsync(charString, new TypeOptions { Delay = 100 });
            });
            if (delayAtLast > 0)
                Sleep(delayAtLast);
        }

        public void MouseScroll(int xLoc, int yLoc, int scrollByXLoc = 0, int scrollByYLoc = 0,
           double delayBefore = 0, double delayAfter = 0,
           int clickLeavEvent = 0, int scrollCount = 0)
        {

            if (delayBefore > 0)
                Sleep(delayBefore);

            try
            {
                Application.Current.Dispatcher.InvokeAsync(async ()=>
                {
                    await Page.Mouse.MoveAsync(xLoc, yLoc, new MoveOptions { Steps = 1 });
                    if (scrollCount > 0)
                    {
                        while(scrollCount-- > 0)
                        {
                            await Page.Mouse.WheelAsync(scrollByXLoc, scrollByYLoc);
                            Sleep(3);
                        }
                    }
                    else
                    {
                        await Page.Mouse.WheelAsync(scrollByXLoc,scrollByYLoc);
                    }
                });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            if (delayAfter > 0)
                Sleep(delayAfter);
        }

        public async Task<string> GetPageSourceAsync(double delay=1)
        {
            try
            {
                await Task.Delay(TimeSpan.FromSeconds(delay), cancellationToken.Token);
                return await Page.GetContentAsync();
            }
            catch (ArgumentException)
            {
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch
            {
            }

            return string.Empty;
        }
        public async Task<string> GoToCustomUrl(string url, int delayAfter = 0,bool ClearResources=false)
        {
            if(ClearResources)
                ResponseList.Clear();
            await Page.GoToAsync(url,waitUntil:WaitUntilNavigation.DOMContentLoaded);
            await Task.Delay(TimeSpan.FromSeconds(delayAfter), cancellationToken.Token);
            return await Page.GetContentAsync();
        }
        public async Task<bool> ChangeTabs(string TargetPageUrl,bool OpenNewIfNotExist=true,bool FocusOnTab=false)
        {
            var IsOpenedNew=false;
            try
            {
                var pages = await browser.PagesAsync();
                var targetPage = pages.FirstOrDefault(x => !string.IsNullOrEmpty(x.Url) && (x.Url == TargetPageUrl || x.Url.Contains(TargetPageUrl)));
                if(!OpenNewIfNotExist && targetPage is null)
                    return false;
                if (targetPage is null)
                {
                    targetPage = await OpenNewPage();
                    targetPage.Response += OnPageResponse;
                    targetPage.SetRequestInterceptionAsync(true);
                    targetPage.DOMContentLoaded += OnPageLoaded;
                    targetPage.Request += OnPageRequest;
                    
                    await targetPage.SetViewportAsync(
                        new ViewPortOptions()
                        {
                            Width = Convert.ToInt32(SystemParameters.PrimaryScreenWidth),
                            Height = Convert.ToInt32(SystemParameters.PrimaryScreenHeight)
                        });
                    await SetCookies(DominatorAccountModel);
                    await targetPage.GoToAsync(TargetPageUrl, waitUntil: WaitUntilNavigation.DOMContentLoaded);
                    await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken.Token);
                }
                if(targetPage != null)
                {
                    IsOpenedNew = true;
                    Page = targetPage;
                }
                if (FocusOnTab)
                {
                    bool isPageInFront = await Page.EvaluateFunctionAsync<bool>("() => document.visibilityState === 'visible'");
                    if(!isPageInFront)
                        await Page.BringToFrontAsync();
                }
            }
            catch { }
            return IsOpenedNew;
        }

        private async Task ApplyCss()
        {
            try
            {
                await Page.WaitForSelectorAsync("body");
                await Page.AddStyleTagAsync(new AddTagOptions { Content = StealthPlugin.ScrollBarCss });
            }
            catch { }
        }

        public async Task PressAnyKeyUpdated(int winKeyCode = 13, int n = 1, int delay = 90, double delayAtLast = 0,
            bool isShiftDown = false)
        {
            var keyname = KeyCodeHelper.FromVirtualKey((uint)winKeyCode);
            await Page.Keyboard.PressAsync(keyname, new PressOptions { Delay = delay });
            if (delayAtLast > 0)
                await Task.Delay(TimeSpan.FromSeconds(delayAtLast), cancellationToken.Token);
        }

        public async Task PressCombinedKey(int winFirstKeyCode, int winSecondKeyCode,
            double delayAtLast = 0)
        {
            var keyname = KeyCodeHelper.FromVirtualKey((uint)winFirstKeyCode);
            var secondKeyName = KeyCodeHelper.FromVirtualKey((uint)winSecondKeyCode);
            await Page.Keyboard.DownAsync(keyname);
            await Page.Keyboard.PressAsync(secondKeyName);
            await Page.Keyboard.UpAsync(keyname);
            if (delayAtLast > 0)
                await Task.Delay(TimeSpan.FromSeconds(delayAtLast), cancellationToken.Token);
        }
        public async Task PressControlWithkey(int winkeyCode = 79,double delayInSec=4,string ModifierKey="Control")
        {
            var secondKeyName = KeyCodeHelper.FromVirtualKey((uint)winkeyCode);
            await Page.Keyboard.DownAsync(ModifierKey);
            await Page.Keyboard.PressAsync(secondKeyName);
            await Page.Keyboard.UpAsync(ModifierKey);
            if (delayInSec > 0)
                await Task.Delay(TimeSpan.FromSeconds(delayInSec), cancellationToken.Token);
        }
        public async Task EnterCharsAsync(string charString, double typingDelay = 0.09, double delayBefore = 0,
            double delayAtLast = 0)
        {
            if (string.IsNullOrEmpty(charString)) return;
            if (delayBefore > 0)
                await Task.Delay(TimeSpan.FromSeconds(delayBefore), cancellationToken.Token);
            await Page.Keyboard.TypeAsync(charString, new TypeOptions { Delay = 100 });
            if (delayAtLast > 0)
                await Task.Delay(TimeSpan.FromSeconds(delayAtLast), cancellationToken.Token);
        }

        public async Task BrowserActAsync(ActType actType, AttributeType attributeType, string attributeValue,
            string value = "", double delayBefore = 0, double delayAfter = 0, int index = 0, int scrollByPixel = 100)
        {
            if (delayBefore > 0)
                await Task.Delay(TimeSpan.FromSeconds(delayBefore), cancellationToken.Token);

            if (Page.IsClosed)
                return;

            if (!string.IsNullOrEmpty(attributeValue) && attributeValue.Contains(@"\"))
                attributeValue = attributeValue.Replace(@"\", "\\\\");

            switch (actType)
            {
                case ActType.EnterByQuery:
                    await Page.EvaluateExpressionAsync(
                        $"document.querySelectorAll('[{attributeType.GetDescriptionAttr()}=\"{attributeValue}\"]')[{index}].value= '{value}'");
                    break;

                case ActType.EnterValue:
                    await Page.EvaluateExpressionAsync(
                        $"document.getElementsBy{attributeType}('{attributeValue}')[{index}].value= '{value}'");
                    break;

                case ActType.ActByQuery:
                    await Page.EvaluateExpressionAsync(
                        $"document.querySelectorAll('[{attributeType.GetDescriptionAttr()}=\"{attributeValue}\"]')[{index}].click()");
                    break;

                case ActType.ScrollWindow:
                    await Page.EvaluateExpressionAsync($"window.scrollBy(0, {scrollByPixel});");
                    break;

                case ActType.ScrollIntoView:
                    await Page.EvaluateExpressionAsync(
                        $"document.getElementsBy{attributeType}('{attributeValue}')[{index}].scrollIntoView()");
                    break;

                case ActType.ScrollIntoViewQuery:
                    await Page.EvaluateExpressionAsync(
                        $"document.querySelectorAll('[{attributeType.GetDescriptionAttr()}=\"{attributeValue}\"]')[{index}].scrollIntoView()");
                    break;

                case ActType.ScrollByQuery:
                    await Page.EvaluateExpressionAsync(
                        $"document.querySelector('[{attributeType.GetDescriptionAttr()}=\"{attributeValue}\"]').scrollBy({value})");
                    break;

                case ActType.ScrollIntoViewChildQuery:
                    await Page.EvaluateExpressionAsync(
                        $"document.querySelectorAll('[{attributeType.GetDescriptionAttr()}=\"{attributeValue}\"]')[{index}]{value}.scrollIntoView()");
                    break;

                case ActType.CustomActType:
                    await Page.EvaluateExpressionAsync(
                        $"document.getElementsBy{attributeType}('{attributeValue}')[{index}].{value}");
                    break;

                case ActType.CustomActByQueryType:
                    await Page.EvaluateExpressionAsync(
                        $"document.querySelectorAll('[{attributeType.GetDescriptionAttr()}=\"{attributeValue}\"]')[{index}].{value}");
                    break;

                case ActType.EnterValueById:
                    await Page.EvaluateExpressionAsync($"document.getElementById('{attributeValue}').value= '{value}'");
                    break;

                case ActType.Click:
                    await Page.EvaluateExpressionAsync($"document.getElementsBy{attributeType}('{attributeValue}')[{index}].click()");
                    break;

                case ActType.ClickById:
                    await Page.EvaluateExpressionAsync($"document.getElementById('{attributeValue}').click()");
                    break;
                default:
                    await Page.EvaluateExpressionAsync(
                        $"document.getElementsBy{attributeType}('{attributeValue}')[{index}].{actType.GetDescriptionAttr()}");
                    break;
            }

            if (delayAfter > 0)
                await Task.Delay(TimeSpan.FromSeconds(delayAfter), cancellationToken.Token);
        }

        /// <summary>
        ///     Browser actions
        /// </summary>
        /// <param name="actType">Type of activity doing on browser window</param>
        /// <param name="element">type of element by which the action gonna be performed</param>
        /// <param name="delayBefore">delay before the action (In seconds)</param>
        /// <param name="delayAfter">delay after the action (In seconds)</param>
        /// <param name="value">value which is going to be entered</param>
        /// <param name="clickIndex">Sometimes multiple buttons have same tag-value</param>
        /// <param name="attributeType"></param>
        /// <param name="attributeValue"></param>
        /// <param name="scrollByPixel"></param>
        public void BrowserAct(ActType actType, string element = "", double delayBefore = 0, double delayAfter = 0,
            string value = "", int clickIndex = 0,
            AttributeType attributeType = AttributeType.Null, string attributeValue = "", int scrollByPixel = 100)
        {
            if (delayBefore > 0)
                Sleep(delayBefore);

            if (Page.IsClosed) return;

            if (!string.IsNullOrEmpty(value) && value.Contains(@"\"))
                value = value.Replace(@"\", "\\\\");

            switch (actType)
            {
                case ActType.ClickByClass:
                    Page.EvaluateExpressionAsync($"document.getElementsByClassName('{element}')[{clickIndex}].click()");
                    break;

                case ActType.ClickById:
                    Page.EvaluateExpressionAsync($"document.getElementById('{element}').click()");
                    break;

                case ActType.ClickByName:
                    Page.EvaluateExpressionAsync($"document.getElementsByName('{element}')[{clickIndex}].click()");
                    break;

                case ActType.EnterValueByClass:
                    Page.EvaluateExpressionAsync(
                        $"document.getElementsByClassName('{element}')[{clickIndex}].value= '{value}'");
                    break;

                case ActType.EnterValueById:
                    Page.EvaluateExpressionAsync($"document.getElementById('{element}').value= '{value}'");
                    break;

                case ActType.EnterValueByName:
                    Page.EvaluateExpressionAsync(
                        $"document.getElementsByName('{element}')[{clickIndex}].value= '{value}'");
                    break;

                case ActType.EnterByQuery:
                    Page.EvaluateExpressionAsync(
                        $"document.querySelectorAll('[{attributeType.GetDescriptionAttr()}=\"{attributeValue}\"]')[{clickIndex}].value= '{value}'");
                    break;

                case ActType.EnterValue:
                    Page.EvaluateExpressionAsync(
                        $"document.getElementsBy{attributeType}('{attributeValue}')[{clickIndex}].value= '{value}'");
                    break;

                case ActType.ActByQuery:
                    Page.EvaluateExpressionAsync(
                        $"document.querySelectorAll('[{attributeType.GetDescriptionAttr()}=\"{attributeValue}\"]')[{clickIndex}].click()");
                    break;

                case ActType.ScrollWindow:
                    Page.EvaluateExpressionAsync($"window.scrollBy(0, {scrollByPixel});");
                    break;

                case ActType.ScrollIntoView:
                    Page.EvaluateExpressionAsync(
                        $"document.getElementsBy{attributeType}('{attributeValue}')[{clickIndex}].scrollIntoView()");
                    break;

                case ActType.ScrollIntoViewQuery:
                    Page.EvaluateExpressionAsync(
                        $"document.querySelectorAll('[{attributeType.GetDescriptionAttr()}=\"{attributeValue}\"]')[{clickIndex}].scrollIntoView()");
                    break;

                case ActType.CustomActType:
                    Page.EvaluateExpressionAsync(
                        $"document.getElementsBy{attributeType}('{attributeValue}')[{clickIndex}].{value}");
                    break;

                case ActType.CustomActByQueryType:
                    Page.EvaluateExpressionAsync(
                        $"document.querySelectorAll('[{attributeType.GetDescriptionAttr()}=\"{attributeValue}\"]')[{clickIndex}].{value}");
                    break;

                default:
                    Page.EvaluateExpressionAsync(
                        $"document.getElementsBy{attributeType}('{attributeValue}')[{clickIndex}].{actType.GetDescriptionAttr()}");
                    break;
            }

            if (delayAfter > 0)
                Sleep(delayAfter);
        }


        public async Task MouseClickAsync(int xLoc, int yLoc, double delayBefore = 0, double delayAfter = 0,
            MouseClickType mouseClickType = MouseClickType.Left)
        {
            var mouseClickButtonType = mouseClickType == MouseClickType.Left ? MouseButton.Left :
                mouseClickType == MouseClickType.Right ? MouseButton.Right :
                MouseButton.Middle;
            try
            {
                await Page.Mouse.ClickAsync(xLoc, yLoc, new ClickOptions()
                {
                    Delay = Convert.ToInt32(delayBefore),
                    Count = 1,
                    Button = mouseClickButtonType,
                    OffSet = new Offset(xLoc, yLoc)
                });
            }
            catch (Exception)
            {

            }
            if (delayAfter > 0)
                await Task.Delay(TimeSpan.FromSeconds(delayAfter), cancellationToken.Token);
        }

        public async Task MouseHoverAsync(int xLoc, int yLoc, double delayBefore = 0, double delayAfter = 0,
            int clickLeavEvent = 0)
        {
            if (delayBefore > 0)
                await Task.Delay(TimeSpan.FromSeconds(delayBefore), cancellationToken.Token);

            await Page.Mouse.MoveAsync(xLoc, yLoc);
            if (delayAfter > 0)
                await Task.Delay(TimeSpan.FromSeconds(delayAfter), cancellationToken.Token);
        }

        public void MouseHover(int xLoc, int yLoc, double delayBefore = 0, double delayAfter = 0,
           int clickLeavEvent = 0)
        {
            if (delayBefore > 0)
                Sleep(delayBefore);
            Application.Current.Dispatcher.InvokeAsync(async delegate
            {
                await Page.Mouse.MoveAsync(xLoc, yLoc);
            });
            if (delayAfter > 0)
                Sleep(delayAfter);
        }
        public async Task<List<string>> GetListInnerHtmlChildElement(ActType actType, AttributeType parentAttributeType,
            string parentAttributeValue, AttributeType childAttributeName, string childAttributeValue,
            ValueTypes valueType = ValueTypes.InnerHtml, double delayBefore = 0, int parentIndex = 0,
            int childIndex = 0)
        {
            if (Page.IsClosed)
                return
                    new List<string>();

            var listNodes = new List<string>();

            var itemCount = actType == ActType.CustomActByQueryType ? int.Parse(await GetChildElementValueAsync(
                                                                          ActType.GetLengthByCustomQuery,
                                                                          parentAttributeType,
                                                                          parentAttributeValue, childAttributeName,
                                                                          childAttributeValue, valueType, delayBefore,
                                                                          parentIndex, childIndex)) - 1
                : actType == ActType.GetValue ? int.Parse(await GetChildElementValueAsync(ActType.GetLength,
                                                    parentAttributeType,
                                                    parentAttributeValue, childAttributeName, childAttributeValue,
                                                    valueType, delayBefore, parentIndex, childIndex)) - 1
                : int.Parse(await GetChildElementValueAsync(ActType.GetLengthByQuery, parentAttributeType,
                      parentAttributeValue, childAttributeName, childAttributeValue, valueType, delayBefore,
                      parentIndex, childIndex)) - 1;

            while (itemCount >= 0)
            {
                listNodes.Add(await GetChildElementValueAsync(actType, parentAttributeType,
                    parentAttributeValue, childAttributeName, childAttributeValue, valueType, delayBefore, parentIndex,
                    itemCount));
                itemCount--;
            }

            return listNodes;
        }
        public async Task<string> GetChildElementValueAsync(ActType actType, AttributeType parentAttributeType,
            string parentAttributeValue, AttributeType childAttributeName, string childAttributeValue,
            ValueTypes valueType = ValueTypes.InnerHtml, double delayBefore = 0, int parentIndex = 0,
            int childIndex = 0)
        {
            if (delayBefore > 0)
                await Task.Delay(TimeSpan.FromSeconds(delayBefore), cancellationToken.Token);

            var doc =
                $"document.getElementsBy{parentAttributeType}('{parentAttributeValue}')[{parentIndex}].getElementsBy{childAttributeName}('{childAttributeValue}')[{childIndex}].{valueType.GetDescriptionAttr()}";

            var doc2 =
                $"document.getElementsBy{parentAttributeType}('{parentAttributeValue}')[{parentIndex}].querySelectorAll('[{childAttributeName.GetDescriptionAttr()}=\"{childAttributeValue}\"]').length";

            if (Page.IsClosed) return "";

            //if (!Browser.CanExecuteJavascriptInMainFrame)
            //    return "";

            switch (actType)
            {
                case ActType.GetValue:
                    return (await Page.EvaluateExpressionAsync(
                               $"document.getElementsBy{parentAttributeType}('{parentAttributeValue}')[{parentIndex}].getElementsBy{childAttributeName}('{childAttributeValue}')[{childIndex}].{valueType.GetDescriptionAttr()}")
                           )?.ToString() ?? "";

                case ActType.GetLength:
                    return (await Page.EvaluateExpressionAsync(
                               $"document.getElementsBy{parentAttributeType}('{parentAttributeValue}')[{parentIndex}].getElementsBy{childAttributeName}('{childAttributeValue}').length")
                           )?.ToString() ?? "0";

                case ActType.GetLengthByQuery:
                    return (await Page.EvaluateExpressionAsync(
                               $"document.querySelectorAll('[{parentAttributeType.GetDescriptionAttr()}=\"{parentAttributeValue}\"]')[{parentIndex}].getElementsBy{childAttributeName}('{childAttributeValue}').length")
                           )?.ToString() ?? "0";

                case ActType.GetLengthByCustomQuery:
                    return (await Page.EvaluateExpressionAsync(
                               $"document.getElementsBy{parentAttributeType}('{parentAttributeValue}')[{parentIndex}].querySelectorAll('[{childAttributeName.GetDescriptionAttr()}=\"{childAttributeValue}\"]').length")
                           )?.ToString() ?? "0";

                case ActType.GetAttribute:
                    return (await Page.EvaluateExpressionAsync(
                               $"document.getElementsBy{parentAttributeType}('{parentAttributeValue}')[{parentIndex}].getElementsBy{childAttributeName}('{childAttributeValue}')[{childIndex}].getAttribute('{valueType.GetDescriptionAttr()}')")
                           )?.ToString() ?? "";

                case ActType.ActByQuery:
                    return (await Page.EvaluateExpressionAsync(
                               $"document.querySelectorAll('[{parentAttributeType.GetDescriptionAttr()}=\"{parentAttributeValue}\"]')[{parentIndex}].getElementsBy{childAttributeName}('{childAttributeValue}')[{childIndex}].{valueType.GetDescriptionAttr()}")
                           )?.ToString() ?? "";
                case ActType.GetLengthByCustomAttributes:
                    return (await Page.EvaluateExpressionAsync(
                               $"document.querySelectorAll('[{parentAttributeType.GetDescriptionAttr()}=\"{parentAttributeValue}\"]')[{parentIndex}].querySelectorAll('[{childAttributeName.GetDescriptionAttr()}=\"{childAttributeValue}\"]').length")
                           )?.ToString() ?? "0";
                default:
                    return (await Page.EvaluateExpressionAsync(
                               $"document.getElementsBy{parentAttributeType}('{parentAttributeValue}')[{parentIndex}].querySelectorAll('[{childAttributeName.GetDescriptionAttr()}=\"{childAttributeValue}\"]')[{childIndex}].{valueType.GetDescriptionAttr()}")
                           )?.ToString() ?? "";
            }
        }


        public int LastCurrentCount = -1;

        public async Task ExpandAllSeeMore()
        {
            var postCount = int.Parse(await GetElementValueAsync(ActType.GetLength, AttributeType.ClassName,
                "see_more_link_inner",
                ValueTypes.OuterHtml));

            while (postCount-- > 0)
                await BrowserActAsync(ActType.Click, AttributeType.ClassName, "see_more_link_inner", delayAfter: 0.25,
                    index: postCount);
        }

        public async Task<List<Tuple<int, string, string, string, string>>> ExpandAllAdViewOptions(int postCount,
            int lastCount, int lastCurrentAdCount = 0)
        {

            var tupleAdsDetals = new List<Tuple<int, string, string, string, string>>();
            await Task.Delay(5000, cancellationToken.Token);

            while (LastCurrentCount++ <= postCount * (lastCount + 1))
            {
                await Page.EvaluateExpressionAsync(
                    $"document.getElementsByClassName('_5jmm _5pat _3lb4')[{LastCurrentCount}].querySelectorAll('[data-testid=\"post_chevron_button\"]')[0].scrollIntoView()");
                var fullAdDetails = await GetElementValueAsync(ActType.GetValue, AttributeType.ClassName,
                    "_5jmm _5pat _3lb4", ValueTypes.OuterHtml, clickIndex: LastCurrentCount);
                if (!fullAdDetails.Contains("sponsored") || !fullAdDetails.Contains("Sponsored"))
                {
                    await Task.Delay(3000, cancellationToken.Token);
                    continue;
                }

                await Task.Delay(2000, cancellationToken.Token);
                await BrowserActAsync(ActType.ScrollWindow, AttributeType.Null, "", scrollByPixel: -50);
                var javascriptResponse =
                    await ExecuteScriptAsync(
                        $"document.getElementsByClassName('_5jmm _5pat _3lb4')[{LastCurrentCount}].OuterHtml");

                var values =
                    Utilities.GetBetween(javascriptResponse.Result.ToString(), "id=\"feed_subtitle",
                        "\""); //_263;2085460778154235;0;3006433072723663;1583140012:8116025295315885125:5:0:32239
                var splittedValues = Regex.Split(values, ";");
                var ownerId = splittedValues[1];
                var postId = splittedValues[3];
                splittedValues = Regex.Split(splittedValues[4], ":");
                var AdId = splittedValues[1];
                var dateTime = splittedValues[0];

                tupleAdsDetals.Add(
                    new Tuple<int, string, string, string, string>(LastCurrentCount, postId, AdId, ownerId, dateTime));
            }

            return tupleAdsDetals;
        }

        public PuppeteerJavascriptResponse ExecuteScript(string script, int delayInSec = 2)
        {
            try
            {
                var resp = Page.EvaluateExpressionAsync<string>(script).Result;
                Task.Delay(TimeSpan.FromSeconds(delayInSec)).Wait(cancellationToken.Token);
                return new PuppeteerJavascriptResponse { Result = resp,Success=true };
            }
            catch(Exception) { return new PuppeteerJavascriptResponse { Result = string.Empty, Success = false }; }
            
        }

        public async Task<PuppeteerJavascriptResponse> ExecuteScriptAsync(string script, int delayInSec = 2)
        {
            try
            {
                var resp = await Page.EvaluateExpressionAsync(script);
                await Task.Delay(TimeSpan.FromSeconds(delayInSec), cancellationToken.Token);
                return new PuppeteerJavascriptResponse { Result =resp,Success=true };
            }
            catch(Exception) { return new PuppeteerJavascriptResponse { Result = string.Empty,Success= false }; }
        }

        public KeyValuePair<int, int> GetXAndY(AttributeType attributeType = AttributeType.Id, string elementName = "",
            int index = 0)
        {
            var xAndY = new KeyValuePair<int, int>();
            var scripty = attributeType == AttributeType.Id
                ? $"document.getElementById('{elementName}').getBoundingClientRect().y"
                : $"document.getElementsByClassName('{elementName}')[{index}].getBoundingClientRect().top";
            var scriptx = attributeType == AttributeType.Id
                ? $"document.getElementById('{elementName}').getBoundingClientRect().x"
                : $"document.getElementsByClassName('{elementName}')[{index}].getBoundingClientRect().left";

            if (ExecuteScript(scriptx, 0).Success)
            {
                var scriptResponse = ExecuteScript(scriptx, 0);
                var x = ConvertDoubleAndInt(scriptResponse.Result.ToString());
                scriptResponse = ExecuteScript(scripty, 0);
                var y = ConvertDoubleAndInt(scriptResponse.Result.ToString());
                xAndY = new KeyValuePair<int, int>(x, y);
                return xAndY;
            }

            return xAndY;
        }

        public async Task<KeyValuePair<int, int>> GetXAndYAsync(AttributeType attributeType = AttributeType.Id,
            string elementName = "", int index = 0,
            string customScriptX = "", string customScriptY = "",
            CoordinateDirection horizontalDirection = CoordinateDirection.Left,
            CoordinateDirection verticalDirection = CoordinateDirection.Top)
        {
            var xAndY = new KeyValuePair<int, int>();

            var scripty = !string.IsNullOrEmpty(customScriptY)
                ? customScriptY
                : attributeType == AttributeType.Id
                    ? $"$('#{elementName}').offset().{verticalDirection.GetDescriptionAttr()}" :
                    attributeType != AttributeType.ClassName ?
                    $"document.querySelector('[{attributeType.GetDescriptionAttr()}=\"{elementName}\"]').getBoundingClientRect().{verticalDirection.GetDescriptionAttr()}" 
                    :$"document.getElementsByClassName('{elementName}')[{index}].getBoundingClientRect().{verticalDirection.GetDescriptionAttr()}";
            var scriptx = !string.IsNullOrEmpty(customScriptX)
                ? customScriptX
                : attributeType == AttributeType.Id
                    ? $"$('#{elementName}').offset().{horizontalDirection.GetDescriptionAttr()}" :
                    attributeType != AttributeType.ClassName ?
                    $"document.querySelector('[{attributeType.GetDescriptionAttr()}=\"{elementName}\"]').getBoundingClientRect().{horizontalDirection.GetDescriptionAttr()}"
                    :$"document.getElementsByClassName('{elementName}')[{index}].getBoundingClientRect().{horizontalDirection.GetDescriptionAttr()}";


            if ((await ExecuteScriptAsync(scriptx, 0)).Success)
            {
                var scriptResponse = await ExecuteScriptAsync(scriptx, 0);
                var x = ConvertDoubleAndInt(scriptResponse.Result.ToString());
                scriptResponse = await ExecuteScriptAsync(scripty, 0);
                var y = ConvertDoubleAndInt(scriptResponse.Result.ToString());
                xAndY = new KeyValuePair<int, int>(x, y);
                return xAndY;
            }

            return xAndY;
        }

        public static int ConvertDoubleAndInt(string input)
        {
            try
            {
                var doubleResult = Convert.ToDouble(input);
                return Convert.ToInt32(doubleResult);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        //public string GetNetworksLoginUrl()
        //{
        //    switch (DominatorAccountModel.AccountBaseModel.AccountNetwork)
        //    {
        //        case SocialNetworks.Facebook:
        //            return "https://www.facebook.com";
        //        case SocialNetworks.Instagram:
        //            return "https://www.instagram.com/accounts/login/";
        //        case SocialNetworks.Twitter:
        //            return "https://twitter.com/login";
        //        case SocialNetworks.Pinterest:
        //            return "https://www.pinterest.com/login/";
        //        case SocialNetworks.LinkedIn:
        //            return "https://www.linkedin.com";
        //        case SocialNetworks.Reddit:
        //            return "https://www.reddit.com/login";
        //        case SocialNetworks.Quora:
        //            return "https://www.quora.com/";
        //        case SocialNetworks.Gplus:
        //            return "https://accounts.google.com/signin";
        //        case SocialNetworks.YouTube:
        //            return "https://accounts.google.com/signin";
        //        case SocialNetworks.Tumblr:
        //            return "https://www.tumblr.com/login";
        //        case SocialNetworks.Social:
        //            return "https://www.google.com";
        //        default:
        //            throw new ArgumentOutOfRangeException();
        //    }
        //}

        //private string SocialHomeUrls()
        //{
        //    switch (DominatorAccountModel.AccountBaseModel.AccountNetwork)
        //    {
        //        case SocialNetworks.Gplus:
        //            return "https://plus.google.com/";
        //        case SocialNetworks.YouTube:
        //            return "https://www.youtube.com/";
        //        default:
        //            throw new ArgumentOutOfRangeException();
        //    }
        //}

        private void CustomLog(string message)
        {
            GlobusLogHelper.log.Info(Log.CustomMessage,
                DominatorAccountModel.AccountBaseModel.AccountNetwork,
                DominatorAccountModel.AccountBaseModel.UserName, "Account Browser Login", message);
        }

        //For reddit json data
        public async Task<string> GoToCustomUrlAndGetPageSource(string url, string startSearchText, string startEndText,
            int delayAfter = 0)
        {
            var response = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(url))
                    await Page.GoToAsync(url, waitUntil: WaitUntilNavigation.DOMContentLoaded);

                await Task.Delay(TimeSpan.FromSeconds(delayAfter), cancellationToken.Token);
                return response;
            }
            catch
            {

                // ignored
            }
            finally
            {
                
            }

            cancellationToken.Token.ThrowIfCancellationRequested();
            return response;
        }

        //For deleting data present in responseList
        public void ClearResources()
        {
            if(ResponseList!=null && ResponseList.Count > 0)
                ResponseList.Clear();
        }
        //Get json data for pagination
        public async Task<string> GetPaginationData(string startSearchText, bool isContains = false
            , string endString = "", bool ViewJsonResponse = false)
        {
            var response = string.Empty;
            try
            {
                response = ResponseList?.FirstOrDefault(x=>!string.IsNullOrEmpty(x) && x.Contains(startSearchText)) ?? string.Empty;
            }
            catch
            {
            }

            cancellationToken.Token.ThrowIfCancellationRequested();
            return response;
        }


        public async Task<List<string>> GetPaginationDataList(string startSearchText, bool isContains = false
            , string endString = "")
        {
            var responseList = new List<string>();
            try
            {
                await Task.Delay(10, cancellationToken.Token);
                responseList = ResponseList?.Where(x => !string.IsNullOrEmpty(x) && x.Contains(startSearchText))?.ToList() ?? new List<string>();
                return responseList;
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch
            {
            }

            cancellationToken.Token.ThrowIfCancellationRequested();
            return responseList;
        }

        //To check reddit json data 
        //private bool GetPaginatoinDataFromByte(byte[] data, string startSearchText, bool isContains = false,
        //    string endString = "")
        //{
        //    try
        //    {
        //        //for (;;);{__ar:
        //        var searchText = Encoding.UTF8.GetString(data);
        //        if (isContains && searchText.Contains(startSearchText))
        //            return true;
        //        if (isContains && string.IsNullOrEmpty(endString) && searchText.Contains(endString) &&
        //            searchText.Contains(startSearchText))
        //            return true;
        //        if (searchText.StartsWith(startSearchText))
        //            return true;
        //        return false;
        //    }
        //    catch
        //    {
        //        // ignored
        //    }

        //    return false;
        //}

        //Get json data list for pagination(for pinterest)
        public async Task<List<string>> GetPaginationDataList(string startSearchText, bool isContains = false)
        {
            var lstJsonData = new List<string>();
            try
            {
                await Task.Delay(100, cancellationToken.Token);
                lstJsonData = ResponseList?.Where(x => !string.IsNullOrEmpty(x) && x.Contains(startSearchText))?.ToList() ?? new List<string>();
                return lstJsonData;
            }
            catch
            {
            }
            cancellationToken.Token.ThrowIfCancellationRequested();
            return lstJsonData;
        }

        public string CurrentUrl()
        {
            var urlNow = Page is null||Page.Url is null ?string.Empty:Page.Url;
            return urlNow;
            //if (!Application.Current.Dispatcher.CheckAccess())
            //    Application.Current.Dispatcher.Invoke(delegate { urlNow = Page.Url; });
            //else
            //    urlNow = Page.Url;
            //return urlNow;
        }

        public KeyValuePair<int, int> GetEndXAndY(AttributeType attributeType = AttributeType.Id,
            string elementName = "")
        {
            var xAndY = new KeyValuePair<int, int>();
            var scriptY = attributeType == AttributeType.Id
                ? $"$('#{elementName}').offset().bottom"
                : $"document.getElementsByClassName('{elementName}')[0].getBoundingClientRect().bottom";
            var scriptX = attributeType == AttributeType.Id
                ? $"$('#{elementName}').offset().right"
                : $"document.getElementsByClassName('{elementName}')[0].getBoundingClientRect().right";

            if (ExecuteScript(scriptX, 0).Success)
            {
                var scriptResponse = ExecuteScript(scriptX, 0);
                var x = ConvertDoubleAndInt(scriptResponse.Result.ToString());
                scriptResponse = ExecuteScript(scriptY, 0);
                var y = ConvertDoubleAndInt(scriptResponse.Result.ToString());
                xAndY = new KeyValuePair<int, int>(x, y);
                return xAndY;
            }

            return xAndY;
        }

        public async Task<IFrame> GetFrame(string url)
        {
            IFrame frame = null;
            var Frames = Page.Frames;
            if (Frames == null || Frames.Length == 0)
                return frame;
            foreach (var item in Frames)
            {
                if (item.Url.Contains(url))
                    return frame;
                var document = await item.GetContentAsync();
            }
            return frame;
        }

        public async Task<string> GetElementValueAsyncFromFrame(IFrame frame, string script)
        {
            await Task.Delay(1000, cancellationToken.Token);
            var jsResponse = await frame.EvaluateExpressionAsync(script);
            return jsResponse?.ToString();
        }

        public async Task ExecuteJSAsyncFromFrame(IFrame frame, string script)
        {
            await Task.Delay(10000, cancellationToken.Token);
            await frame.EvaluateExpressionAsync(script);
        }

        public async Task SelectTextAsync(int stratXlocation, int startYLocation, int moveToXLocation,
            int moveToYLocation, double delayBefore = 0, double delayAfter = 0,
            int clickLeavEvent = 0)
        {
            if (delayBefore > 0)
                await Task.Delay(TimeSpan.FromSeconds(delayBefore), cancellationToken.Token);

            if (Page.IsClosed) return;
            await Page.Keyboard.DownAsync("Control");
            await Page.Keyboard.PressAsync("A");
            await Page.Keyboard.UpAsync("Control");
            if (delayAfter > 0)
                await Task.Delay(TimeSpan.FromSeconds(delayAfter), cancellationToken.Token);
        }

        public PuppeteerJavascriptResponse EvaluateScript(string script, int delayInSec = 2)
        {
            var resp = Page.EvaluateExpressionAsync(script).Result;
            Task.Delay(TimeSpan.FromSeconds(delayInSec)).Wait(cancellationToken.Token);
            return new PuppeteerJavascriptResponse { Result = resp};
        }
        public List<KeyValuePair<string, PuppeteerMemoryStreamResponseFilter>> TwitterJsonResponse()
        {
            try
            {
                return new List<KeyValuePair<string, PuppeteerMemoryStreamResponseFilter>>();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> CopyPasteContentAsync(string message = "", int winKeyCode = 13, int delay = 90,
            double delayAtLast = 0)
        {
            try
            {
                Clipboard.SetText(message);
                await Page.Keyboard.DownAsync("Control");
                await Page.Keyboard.PressAsync("V");
                await Page.Keyboard.UpAsync("Control");
                Clipboard.Clear();
                return false;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return false;
        }

        public void CopyPasteContent(string message = "", int winKeyCode = 13, int delay = 90, double delayAtLast = 0)
        {
            CopyPasteContentAsync(message, winKeyCode, delay, delayAtLast).Wait();
        }
        public async Task ChooseFile(string MediaPath,string Script)
        {
            try
            {
                var fileChooserHandler = Page.WaitForFileChooserAsync();
                await Task.WhenAll(fileChooserHandler,ExecuteScriptAsync(Script));
                var fileChooser = await fileChooserHandler;
                await fileChooser.AcceptAsync(MediaPath);
            }
            catch { }
        }
        public async Task WaitForElement(string Script)
        {
            try
            {
                await Page.WaitForExpressionAsync(Script);
            }
            catch { }
        }
        public async Task WaitForSelector(string Selector)
        {
            try
            {
                await Page.WaitForSelectorAsync(Selector);
            }
            catch { }
        }

        public async Task<string> ExecuteJsOnDom(string response)
        {
            try
            {
                var page = await browser.NewPageAsync();

                // Minimal HTML to run the script
                await page.SetContentAsync("<html><body><input name='ui_metrics' /></body></html>");
                await page.EvaluateExpressionAsync(response);
                var json = await page.EvaluateExpressionAsync<string>("document.getElementsByName('ui_metrics')[0].value");
                return json;
            }
            catch { }
            return response;
        }
        #endregion
    }
}
