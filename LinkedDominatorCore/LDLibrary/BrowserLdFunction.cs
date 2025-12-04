using CefSharp;
using DominatorHouseCore;
using DominatorHouseCore.EmailService;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.PuppeteerBrowser;
using DominatorHouseCore.Request;
using DominatorHouseCore.Utility;
using EmbeddedBrowser;
using HtmlAgilityPack;
using LinkedDominatorCore.DetailedInfo;
using LinkedDominatorCore.Enums;
using LinkedDominatorCore.Interfaces;
using LinkedDominatorCore.LDModel;
using LinkedDominatorCore.LDUtility;
using LinkedDominatorCore.Request;
using LinkedDominatorCore.Response;
using LinkedDominatorCore.Utility;
using MahApps.Metro.Controls;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using UserScraperModel = LinkedDominatorCore.LDModel.SalesNavigatorScraper.UserScraperModel;
using Utils = LinkedDominatorCore.LDUtility.Utils;

namespace LinkedDominatorCore.LDLibrary
{
    public class BrowserLdFunction : ILdFunctions
    {
        private readonly LdDataHelper _ldDataHelper= LdDataHelper.GetInstance;

        // if ILdHttpHelper encapsulated (means private and make it accessible using GetInnerHttpHelper)
        // if ILdHttpHelper is public  it will not be null in BaseLinkedinProcessor constructor
        // it will arise a problem setting same cookies in all accounts when run multiple accounts
        // conclusion : keep ILdHttpHelper protected so that it will accessible within child
        // since serviceLocator container is register in 'new HierarchicalLifetimeManager()'

        protected readonly ILdHttpHelper HttpHelper;
        private BrowserAutomationExtension _automationExtension;
        private readonly ILDAccountSessionManager sessionManager;
        private readonly LdFunctions ldFunctions = null;
        public BrowserLdFunction(DominatorAccountModel dominatorAccountModel, ILdHttpHelper httpHelper,ILDAccountSessionManager lDAccountSession)
        {
            try
            {
                HttpHelper = httpHelper;
                if (dominatorAccountModel.AccountBaseModel?.AccountId != null)
                    _linkedInAccountModel = dominatorAccountModel;
                ldFunctions = ldFunctions ?? new LdFunctions(dominatorAccountModel, httpHelper, lDAccountSession);
                sessionManager= lDAccountSession;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private DominatorAccountModel _linkedInAccountModel { get; set; }
        public BrowserWindow SecondaryBrowser { get; set; }
        public bool IsBrowser { get; } = true;
        public BrowserWindow BrowserWindow { get; set; }

        public void SetCookieAndProxy(DominatorAccountModel dominatorAccountModel, IHttpHelper httpHelper,
            BrowserInstanceType browserInstanceType = BrowserInstanceType.Primary)
        {
            // sometimes not setting the cookies
            // if already set cookies to ObjLdRequestParameters no need to set again
            sessionManager.AddOrUpdateSession(ref dominatorAccountModel);
            if (string.IsNullOrEmpty(_linkedInAccountModel?.AccountId))
                _linkedInAccountModel = dominatorAccountModel;

            try
            {
                var objLdRequestParameters = httpHelper.GetRequestParameter();
                if (objLdRequestParameters.Cookies == null)
                    objLdRequestParameters.Cookies = new CookieCollection();
                if (dominatorAccountModel.Cookies.Count > 0 && objLdRequestParameters.Cookies?.Count == 0)
                    objLdRequestParameters.Cookies = dominatorAccountModel.Cookies;
                if (dominatorAccountModel.AccountBaseModel.AccountProxy != null)
                    objLdRequestParameters.Proxy = dominatorAccountModel.AccountBaseModel.AccountProxy;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11;
                httpHelper.SetRequestParameter(objLdRequestParameters);
                SetRequestParametersAndProxy_MobileLogin(true);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            var name = LDAccountsBrowserDetails.GetBrowserName(dominatorAccountModel, browserInstanceType);

            BrowserWindow = LDAccountsBrowserDetails.GetInstance().AccountBrowserCollections[name];
            _automationExtension = new BrowserAutomationExtension(BrowserWindow);
        }

        public string NormalMessageProcess(List<string> imageSource, SenderDetails objLinkedinUser,
            string postString, string finalMessage)
        {
            return "";
        }
        public void NavigateSalesProfile()
        {
            try
            {
                if(BrowserWindow != null && !IsCookieExist(_linkedInAccountModel.Cookies, "li_a") && LdConstants.IsSalesAccount(BrowserWindow.GetPageSource()))
                {
                    _automationExtension.LoadPageUrlAndWait(LdConstants.GetSalesHomePageUrl, delayInSec: 6);
                    BrowserWindow.SaveCookies();
                    _linkedInAccountModel = BrowserWindow.DominatorAccountModel;
                }
            }
            catch { }
        }
        public bool IsCookieExist(CookieCollection Cookies, string CookieNameOrValue)
        {
            var ExistCookie = false;
            try
            {
                ExistCookie = Cookies.Cast<System.Net.Cookie>().Any(cookie => string.Equals(cookie.Name, CookieNameOrValue, StringComparison.OrdinalIgnoreCase) || string.Equals(cookie.Value, CookieNameOrValue, StringComparison.OrdinalIgnoreCase));
                return ExistCookie;
            }
            catch (Exception) { return ExistCookie; }
        }
        public void SetRequestParametersAndProxy_MobileLogin(bool IsLogin = false)
        {
            try
            {
                var csrfToken = GetCsrfToken();
                var objLdRequestParameters = HttpHelper.GetRequestParameter();

                objLdRequestParameters.Headers.Clear();

                objLdRequestParameters.AddHeader("Host", "www.linkedin.com");
                objLdRequestParameters.KeepAlive = true;
                objLdRequestParameters.UserAgent = IsLogin ? "ANDROID OS" :
                    string.IsNullOrEmpty(_linkedInAccountModel.UserAgentMobile)
                    ? LdConstants.UserAgent
                    : _linkedInAccountModel.UserAgentMobile;
                objLdRequestParameters.AddHeader("X-RestLi-Protocol-Version", "2.0.0");
                objLdRequestParameters.AddHeader("Accept-Language", "en-US");
                if (!string.IsNullOrEmpty(csrfToken))
                    objLdRequestParameters.AddHeader("Csrf-Token", csrfToken);
                objLdRequestParameters.ContentType = @"application/x-www-form-urlencoded";

                objLdRequestParameters.AddHeader("X-LI-Lang", "en_US");

                objLdRequestParameters.AddHeader("X-UDID", "d0da3223-5b25-4c75-904b-2013fc0d640e");
                objLdRequestParameters.AddHeader("X-LI-Track",
                    "{\"osName\":\"Android OS\",\"osVersion\":\"6.0\",\"clientVersion\":\"4.1.256\",\"clientMinorVersion\":116400,\"model\":\"LAVA_V23GB\",\"dpi\":\"xhdpi\",\"deviceType\":\"android\",\"appId\":\"com.linkedin.android\",\"deviceId\":\"d0da3223-5b25-4c75-904b-2013fc0d640e\",\"timezoneOffset\":5,\"storeId\":\"us_googleplay\",\"isAdTrackingLimited\":false,\"mpName\":\"voyager-android\",\"mpVersion\":\"0.329.59\"}");
                if(IsLogin)
                    objLdRequestParameters.AddHeader("X-LI-User-Agent",LdConstants.UserAgent);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public string SendGroupInviter(string actionUrlOrId, string postDataOrMessage)
        {
            return HttpHelper.HandlePostResponse(actionUrlOrId, postDataOrMessage)?.Response;
        }


        public string MessageDetails(string url)
        {
            var response = string.Empty;
            try
            {
                _automationExtension.LoadPageUrlAndWait(url);
                response = BrowserWindow.GetPageSource();
                var MessageButtonClass = LDClassesConstant.Messenger.MessageButtonClass;
                var ClickIndex = 1;
                var nodes = HtmlAgilityHelper.GetListInnerHtmlOrInnerTextOrOuterHtmlFromIdOrClassName(response, "", true,MessageButtonClass);
                var IsMyConnection = !nodes.Any(x => x.Contains("Connect") || x.Contains("Follow"));
                if (!IsMyConnection)
                {
                    MessageButtonClass = MessageButtonClass?.Replace("primary", "secondary");
                    ClickIndex = 4;
                }
                var success =
                    _automationExtension.ExecuteScript(AttributeIdentifierType.ClassName,MessageButtonClass, 1,ClickIndex, EventType.click);
                if (!success.Success)
                {
                    success = _automationExtension.ExecuteScript(AttributeIdentifierType.Xpath,
                        $"//{HTMLTags.Span}[text()='Message']");
                }

                response = BrowserWindow.GetPageSource();
                var closewindow = _automationExtension.ExecuteScript(AttributeIdentifierType.ClassName,
                    LDClassesConstant.Scrapper.CloseConversationWindowClass1);
                if (!closewindow.Success)
                    closewindow = _automationExtension.ExecuteScript("document.querySelector('div[data-testid=\"interop-shadowdom\"]').shadowRoot.querySelector('button>svg[data-test-icon=\"close-small\"]').parentNode.click();", 3);
            }
            catch (Exception)
            {
                return null;
            }

            return response;
        }

        public LiveChatMessageUserdetailResponseHandler LivechatUserMessage(DominatorAccountModel linkedInAccount,
            long nextPage, ILdFunctions ldFunctions)
        {
            return null;
        }

        public string Getputresponse(string url, byte[] imageByte)
        {
            return null;
        }
        #region Requests Implemented With Mobile Device

        public string PreLoginResponse(CancellationToken cancellationToken, bool isSalesNavigator)
        {
            string response;
            try
            {
                if (BrowserWindow == null)
                {
                    BrowserWindow _browserWindow;
                    LDAccountsBrowserDetails.GetInstance().AccountBrowserCollections
                        .TryGetValue(_linkedInAccountModel.UserName, out _browserWindow);
                    var Account = _linkedInAccountModel;
                    sessionManager.AddOrUpdateSession(ref Account);
                    _linkedInAccountModel = Account;
                    if (_browserWindow == null)
                    {
                        LDAccountsBrowserDetails.GetInstance()
                            .StartBrowserLogin(_linkedInAccountModel, cancellationToken, 
                            true,BrowserInstanceType.Primary,sessionManager);

                        LDAccountsBrowserDetails.GetInstance().AccountBrowserCollections
                            .TryGetValue(_linkedInAccountModel.UserName, out _browserWindow);
                    }

                    Thread.Sleep(15000);
                }

                response = BrowserWindow.GetPageSourceAsync().Result;
                response = HttpUtility.HtmlDecode(response);
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                response = null;
                ex.DebugLog();
            }

            return response;
        }

        public async Task<LoginResponseHandler> Login(CancellationToken cancellationToken, string challengeId = "",
            string actionUrls = "",params object[] OptionalValues)
        {
            try
            {
                return new LoginResponseHandler(BrowserWindow.GetPageSource());
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        /// <summary>
        ///     here we checking if
        /// </summary>
        /// <param name="postString"></param>
        /// <param name="actionUrl"></param>
        /// <param name="flagship3ProfileViewBase"></param>
        /// <returns></returns>
        public IResponseParameter SendConnectionRequestAlternativeMethod(string postString, string actionUrl,
            string flagship3ProfileViewBase)
        {
            IResponseParameter respParam = null;
            try
            {
                var resp = false;
                var pageSource = BrowserWindow.GetPageSource();
                if (pageSource.Contains("text-heading-xlarge inline t-24 v-align-middle break-words") ||LdDataHelper.Instance.IsSalesProfile(actionUrl))
                {
                    _automationExtension.LoadPageUrlAndWait(actionUrl);
                    resp = _automationExtension.ExecuteScript(string.Format(LDClassesConstant.ScriptConstant.ScriptWithQuerySelectorToClick, HTMLTags.Button, HTMLTags.HtmlAttribute.AriaLabel, "Open actions overflow menu"), 4).Success;
                    pageSource = BrowserWindow.GetPageSource();
                    var username = Utils.GetBetween(pageSource, "class=\"_headingText_e3b563 _default_1i6ulk _sizeXLarge_e3b563\">", "</h1>");
                    username = Regex.Replace(username, "\\(.*\\)", "");
                    username = WebUtility.HtmlEncode(username)?.Replace("\r\n","\n")?.Replace("\t","")?.Replace("\n","")?.Trim();
                    if (pageSource.Contains("Remove Connection"))
                    {
                        return respParam = new ResponseParameter { Response = LdConstants.UserIsAlreadyYourConnection };
                    }
                    else if (pageSource.Contains($"Withdraw invitation sent to {username}") ||
                        pageSource.Contains($"Pending, click to withdraw invitation sent to {username}")
                        || pageSource.Contains("Pending, click to withdraw invitation sent to")
                        || pageSource.Contains("Connect — Pending"))
                    {
                        return respParam = new ResponseParameter { Response = LdConstants.InvitationHasBeenSent };
                    }
                    if (pageSource.Contains($"Invite {username} to connect") || pageSource.Contains("Invite") || pageSource.Contains("Connect"))
                    {
                        var IsClickedConnect = _automationExtension.ExecuteScript(string.Format(LDClassesConstant.ScriptConstant.ScriptWithQuerySelectorAllToClick, HTMLTags.Button, HTMLTags.HtmlAttribute.AriaLabel, $"Invite {username} to connect", 1), 2).Success;
                        if (!IsClickedConnect)
                        {
                            var IsClickedMoreOption = _automationExtension.ExecuteScript(string.Format(LDClassesConstant.ScriptConstant.ScriptWithQuerySelectorAllToClick, HTMLTags.Button, HTMLTags.HtmlAttribute.AriaLabel, "More actions", 1), 3).Success;
                            pageSource = BrowserWindow.GetPageSource();
                            if (pageSource.Contains("Remove Connection"))
                            {
                                return respParam = new ResponseParameter { Response = LdConstants.UserIsAlreadyYourConnection };
                            }
                            else if (pageSource.Contains($"Withdraw invitation sent to {username}"))
                            {
                                return respParam = new ResponseParameter { Response = LdConstants.InvitationHasBeenSent };
                            }
                            var IsClickedConnectFromOptionMenu = false;
                            var ListNodes = HtmlAgilityHelper.GetListNodesFromAttibute(pageSource, HTMLTags.Button, AttributeIdentifierType.ClassName, null, "ember-view _item_1xnv7i");
                            if(ListNodes!=null && ListNodes.Count>0)
                            {
                                var ConnectId = ListNodes.FirstOrDefault(x => (!string.IsNullOrEmpty(x.Attributes["aria-label"]?.Value) && x.Attributes["aria-label"].Value.Contains("Invite") && x.Attributes["aria-label"].Value.Contains(username))
                                ||(!string.IsNullOrEmpty(x?.InnerText) && x.InnerText.Contains("Connect")))?.Id;
                                IsClickedConnectFromOptionMenu = _automationExtension.ExecuteScript(string.Format(LDClassesConstant.ScriptConstant.ScriptGetElementByIdToClick,ConnectId), 2).Success;
                                ListNodes.Clear();
                            }else
                                IsClickedConnectFromOptionMenu = _automationExtension.ExecuteScript(string.Format(LDClassesConstant.ScriptConstant.ScriptWithQuerySelectorAllToClick, HTMLTags.ListIcon, HTMLTags.HtmlAttribute.Type, "connect", 1), 2).Success;
                            var PopUpPageSource = BrowserWindow.GetPageSource();
                            if (PopUpPageSource.Contains("You can add a note to personalize your invitation to") ||PopUpPageSource.Contains("LinkedIn members are more likely to accept invitations that include a note")
                                ||PopUpPageSource.Contains("Include a personal message"))
                            {
                                IsClickedConnect = true;
                            }
                            else
                            {
                                _automationExtension.ExecuteScript(string.Format(LDClassesConstant.ScriptConstant.ScriptWithQuerySelectorAllToClick, HTMLTags.Button, HTMLTags.HtmlAttribute.AriaLabel, "Other", 0), 2);
                                IsClickedConnect = _automationExtension.ExecuteScript(string.Format(LDClassesConstant.ScriptConstant.ScriptWithQuerySelectorAllToClick, HTMLTags.Button, HTMLTags.HtmlAttribute.AriaLabel, "Connect", 0), 3).Success;
                            }
                        }
                        if (!string.IsNullOrEmpty(postString) && IsClickedConnect)
                        {
                            var IsClickedAddNote = _automationExtension.ExecuteScript(string.Format(LDClassesConstant.ScriptConstant.ScriptWithQuerySelectorAllToClick, HTMLTags.Button, HTMLTags.HtmlAttribute.AriaLabel, "Add a note", 0), 2).Success;
                            var axisX = BrowserWindow.ExecuteScript(string.Format(LDClassesConstant.ScriptConstant.ScriptWithQuerySelectorAllToGetXandY, HTMLTags.TextArea, HTMLTags.HtmlAttribute.Id, "connect-cta-form__invitation", 0, "x")).Result;
                            var axisY = BrowserWindow.ExecuteScript(string.Format(LDClassesConstant.ScriptConstant.ScriptWithQuerySelectorAllToGetXandY, HTMLTags.TextArea, HTMLTags.HtmlAttribute.Id, "connect-cta-form__invitation", 0, "y")).Result;
                            BrowserWindow.MouseClick(Convert.ToInt32(axisX) + 5, Convert.ToInt32(axisY) + 5, delayAfter: 5);
                            BrowserWindow.EnterChars($" {postString} ", 0.2);
                        }
                        pageSource = BrowserWindow.GetPageSource();
                        var AriaLabel = pageSource.Contains("Send invitation") ? "Send invitation" : pageSource.Contains("Send without a note") ? "Send without a note" : "Send now";
                        var IsSentConnectionRequest = _automationExtension.ExecuteScript(string.Format(LDClassesConstant.ScriptConstant.ScriptWithQuerySelectorAllToClick, HTMLTags.Button, HTMLTags.HtmlAttribute.AriaLabel, AriaLabel, 0), 2).Success;
                        if(!IsSentConnectionRequest)
                            IsSentConnectionRequest = _automationExtension.ExecuteScript(string.Format(LDClassesConstant.ScriptConstant.ScriptWithQuerySelectorAllToClick, HTMLTags.Button, HTMLTags.HtmlAttribute.Class, "button-primary-medium connect-cta-form__send", 0), 2).Success;
                        Thread.Sleep(3000);
                        if (IsSentConnectionRequest)
                        {
                            resp = _automationExtension.ExecuteScript(string.Format(LDClassesConstant.ScriptConstant.ScriptWithQuerySelectorToClick, HTMLTags.Button, HTMLTags.HtmlAttribute.AriaLabel, "Open actions overflow menu"), 4).Success;
                            pageSource = BrowserWindow.GetPageSource();
                            if (pageSource.Contains($"Withdraw invitation sent to {username}") 
                                || pageSource.Contains("Your invitation to connect was sent") 
                                || pageSource.Contains($"Pending, click to withdraw invitation sent to {username}") 
                                || pageSource.Contains($"Pending, click to withdraw invitation sent to {WebUtility.HtmlDecode(username)}") 
                                || pageSource.Contains("Pending, click to withdraw invitation sent to")
                                || pageSource.Contains("Connect — Pending"))
                            {
                                return respParam = new ResponseParameter { Response = LdConstants.InvitationSentSuccessFully };
                            }
                            else if (pageSource.Contains(LdConstants.UnableToConnectToUser(username)))
                            {
                                return respParam = new ResponseParameter { Response = LdConstants.UnableToConnectToUser(username) };
                            }
                        }
                        else if (pageSource.Contains(LdConstants.UnableToConnectToUser(username)))
                        {
                            return respParam = new ResponseParameter { Response = LdConstants.UnableToConnectToUser(username) };
                        }
                    }
                    else
                    {
                        return respParam = new ResponseParameter { Response = LdConstants.UserIsAlreadyYourConnection };
                    }
                }
                else
                {
                    resp = _automationExtension.ExecuteScript(string.Format(LDClassesConstant.ScriptConstant.ScriptWithQuerySelectorToClick, HTMLTags.Button, HTMLTags.HtmlAttribute.AriaLabel, "Open actions overflow menu"), 4).Success;
                    pageSource = BrowserWindow.GetPageSource();
                    if (!resp || !pageSource.Contains("Connect"))
                        _automationExtension.ExecuteScript(string.Format(LDClassesConstant.ScriptConstant.ScriptWithQuerySelectorToClick,HTMLTags.Button,HTMLTags.HtmlAttribute.DataViewName, "profile-overflow-button"),2);
                    pageSource = BrowserWindow.GetPageSource();
                    if (pageSource.Contains("Connect — Pending") || pageSource.Contains("Remove connection"))
                    {
                        return respParam = new ResponseParameter { Response = LdConstants.InvitationHasBeenSent };
                    }
                    pageSource = BrowserWindow.GetPageSource();
                    var XandY = _automationExtension.GetXAndY("_text-container_1xnv7i", AttributeIdentifierType.ClassName);
                    if(XandY.Key > 0 && XandY.Value > 0)
                        BrowserWindow.MouseClick(XandY.Key+5,XandY.Value+5,delayAfter:5);
                    //if again showing connect button 
                    if (pageSource.Contains("More") && pageSource.Contains("Connect"))
                    {
                        _automationExtension.ExecuteScript(string.Format(LDClassesConstant.ScriptConstant.ScrollWindow, 0, 0));
                        resp = _automationExtension.ExecuteScript(AttributeIdentifierType.Xpath, $"//{HTMLTags.Span}[text()='More']").Success;
                        resp = _automationExtension.ExecuteScript(AttributeIdentifierType.Xpath, $"//{HTMLTags.Paragraph}[text()='Connect']").Success;
                        if(!resp)
                            resp = _automationExtension.ExecuteScript(AttributeIdentifierType.Xpath, $"//{HTMLTags.Span}[text()='Connect']").Success;
                    }
                    pageSource = BrowserWindow.GetPageSource();
                    if (pageSource.Contains("to see their posts or send a message instead. We encourage you to only connect with people you know") && pageSource.Contains("Connect") && pageSource.Contains("Message"))
                    {
                        resp = _automationExtension.ExecuteScript(AttributeIdentifierType.Xpath, $"//{HTMLTags.Span}[text()='Connect']").Success;
                    }
                    if (string.IsNullOrWhiteSpace(postString))
                    {
                        pageSource = BrowserWindow.GetPageSource();
                        resp = _automationExtension.ExecuteScript("document.querySelector('div[data-testid=\"interop-shadowdom\"]').shadowRoot.querySelector('button[aria-label=\"Send without a note\"]').click();", 5).Success;
                        resp = _automationExtension.ExecuteScript(string.Format(LDClassesConstant.ScriptConstant.ScriptGetElementsByClassNameToClick, "button-primary-medium connect-cta-form__send", 0), 5).Success;
                        pageSource = BrowserWindow.GetPageSource();
                        if (pageSource.Contains("Send now"))
                            resp = _automationExtension.ExecuteScript(string.Format(LDClassesConstant.ScriptConstant.ScriptWithQuerySelectorToClick, "", HTMLTags.HtmlAttribute.AriaLabel, "Send now"))
                                .Success;

                        if (pageSource.Contains("Send Invitation"))
                            resp = _automationExtension.ExecuteScript(string.Format(LDClassesConstant.ScriptConstant.ScriptGetElementsByClassNameToClick, "button-primary-medium connect-cta-form__send", 0), 2).Success;
                        pageSource = BrowserWindow.GetPageSource();
                        if (pageSource.Contains("artdeco-modal artdeco-modal--layer-default ip-fuse-limit-alert") ||
                            pageSource.Contains("You’re out of invitations for now"))
                        {
                            return new ResponseParameter { Response = LdConstants.YouHaveReachedWeeklyInvitationLimit };
                        }
                        if (pageSource.Contains(
                            "pv-s-profile-actions pv-s-profile-actions--follow ml2 artdeco-button artdeco-button--2 artdeco-button--primary ember-view")
                        )
                            resp = _automationExtension.ExecuteScript(AttributeIdentifierType.ClassName,
                                    "pv-s-profile-actions pv-s-profile-actions--follow ml2 artdeco-button artdeco-button--2 artdeco-button--primary ember-view")
                                .Success;
                        _automationExtension.LoadPageUrlAndWait(actionUrl, 5);
                        resp = _automationExtension.ExecuteScript(string.Format(LDClassesConstant.ScriptConstant.ScriptWithQuerySelectorToClick, HTMLTags.Button, HTMLTags.HtmlAttribute.AriaLabel, "Open actions overflow menu"), 2).Success;
                        pageSource = BrowserWindow.GetPageSource();
                        if (pageSource.Contains(
                                "pv-s-profile-actions pv-s-profile-actions--connect ml2  artdeco-button artdeco-button--2 artdeco-button--primary artdeco-button--disabled ember-view") &&
                            pageSource.Contains("An invitation has been sent") ||
                            (pageSource.Contains("Pending, click to withdraw invitation sent to"))||
                            pageSource.Contains("Your invitation to connect was sent.") ||
                            pageSource.Contains("Withdraw invitation sent to") || pageSource.Contains("Connection sent:") || pageSource.Contains("Connect — Pending") ||
                            (pageSource.Contains("pending-connection") && pageSource.Contains("Connect — Pending"))
                            )
                            respParam = new ResponseParameter { Response = LdConstants.InvitationSentSuccessFully };
                        else if (!pageSource.Contains("3rd"))
                            respParam = new ResponseParameter { Response = LdConstants.UserIsAlreadyYourConnection };
                    }
                    else
                    {
                        var success = _automationExtension.ExecuteScript("document.querySelector('div[data-testid=\"interop-shadowdom\"]').shadowRoot.querySelector('button[aria-label=\"Add a free note\"]').click();", 3);
                        if(!success.Success)
                            success = _automationExtension.ExecuteScript("document.querySelector('div[data-testid=\"interop-shadowdom\"]').shadowRoot.querySelector('button[aria-label=\"Add a note\"]').click();", 3);
                        var source = BrowserWindow.GetPageSource();
                        var ModelText =  _automationExtension.ExecuteScript("document.querySelector('div[data-testid=\"interop-shadowdom\"]').shadowRoot.querySelector('h2[id=\"send-invite-modal\"]').innerText", 3);
                        if (source.Contains("Include a personal message (optional):")||source.Contains("[Optional] Add a note about how you know each other")||(ModelText?.Result != null && ModelText.Result.ToString().Contains("Add a note to your invitation")))
                        {
                            var scriptToGetMessageArea = "document.querySelector('div[data-testid=\"interop-shadowdom\"]').shadowRoot.querySelector('textarea[name=\"message\"]').getBoundingClientRect().{0};";
                            var textAreaXCoordinate = _automationExtension.ExecuteScript(string.Format(scriptToGetMessageArea,"x"), 3).Result;
                            var textAreaYCoordinate = _automationExtension.ExecuteScript(string.Format(scriptToGetMessageArea, "y"), 3).Result;
                            BrowserWindow.MouseClick(xLoc: Convert.ToInt32(textAreaXCoordinate) + 4, yLoc: Convert.ToInt32(textAreaYCoordinate) + 4, delayAfter: 3);
                            BrowserWindow.EnterChars(postString, 0.09);
                        }
                        resp = _automationExtension.ExecuteScript("document.querySelector('div[data-testid=\"interop-shadowdom\"]').shadowRoot.querySelector('button[aria-label=\"Send invitation\"]').click();", 5).Success;
                        if(!resp)
                            resp = _automationExtension.ExecuteScript(string.Format(LDClassesConstant.ScriptConstant.ScriptGetElementsByClassNameToClick, "artdeco-button artdeco-button--2 artdeco-button--primary ember-view ml1", 0), 5).Success;
                        _automationExtension.LoadPageUrlAndWait(actionUrl, 5);
                        resp = _automationExtension.ExecuteScript(string.Format(LDClassesConstant.ScriptConstant.ScriptWithQuerySelectorToClick, HTMLTags.Button, HTMLTags.HtmlAttribute.AriaLabel, "Open actions overflow menu"), 2).Success;
                        if(!resp)
                            resp = _automationExtension.ExecuteScript(string.Format(LDClassesConstant.ScriptConstant.ScriptWithQuerySelectorToClick, HTMLTags.Button, HTMLTags.HtmlAttribute.AriaLabel, "More actions"), 2).Success;
                        pageSource = BrowserWindow.GetPageSource();
                        if (pageSource.Contains(
                                "pv-s-profile-actions pv-s-profile-actions--connect ml2  artdeco-button artdeco-button--2 artdeco-button--primary artdeco-button--disabled ember-view") &&
                            pageSource.Contains("An invitation has been sent") ||
                            pageSource.Contains("Your invitation to connect was sent.") ||
                            pageSource.Contains("Withdraw invitation sent to") || pageSource.Contains("Connection sent:") ||
                            pageSource.Contains("pending-connection") || pageSource.Contains("Connect — Pending")
                            || pageSource.Contains("Pending, click to withdraw invitation sent to"))
                            return respParam = new ResponseParameter { Response = LdConstants.InvitationSentSuccessFully };


                        //add a note
                        if (!_automationExtension
                            .ExecuteScript(
                                @"document.evaluate('//button[@class=\'artdeco-button artdeco-button--secondary artdeco-button--3 mr1\']', document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue.click()")
                            .Success)
                            resp = _automationExtension.ExecuteScript(
                                    @"document.evaluate('//button[@class=\'mr1 artdeco-button artdeco-button--muted artdeco-button--3 artdeco-button--secondary ember-view\']', document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue.click()")
                                .Success;

                        //for some of countries add note is not working
                        if (!resp)
                            resp = _automationExtension.ExecuteScript(string.Format(LDClassesConstant.ScriptConstant.ScriptWithQuerySelectorToClick, "", HTMLTags.HtmlAttribute.AriaLabel, "Add a note"))
                                .Success;
                        if (!resp)
                            resp = _automationExtension.ExecuteScript($"document.getElementById('connect-cta-form__invitation').value='{postString}'")
                                .Success;

                        // here sometimes button is disabled unable to execute javascript
                        BrowserWindow.EnterChars($" {postString} ", 0.2);
                        //resp = _automationExtension.ExecuteScript(@"document.evaluate('//textarea[@id=\'custom-message\']', document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue.value='" + postString?.Replace("\'", "\\'") + "';", delayInSec: 2).Success;
                        pageSource = BrowserWindow.GetPageSource();


                        if (!_automationExtension
                            .ExecuteScript(string.Format(LDClassesConstant.ScriptConstant.ScriptGetElementsByClassNameToClick, "ml1 artdeco-button artdeco-button--3 artdeco-button--primary ember-view", 0))
                            .Success)
                            resp = _automationExtension
                                .ExecuteScript(string.Format(LDClassesConstant.ScriptConstant.ScriptGetElementsByClassNameToClick, "artdeco-button artdeco-button--3 ml1", 0))
                                .Success;
                        if (!resp)
                            resp = _automationExtension.ExecuteScript(string.Format(LDClassesConstant.ScriptConstant.ScriptWithQuerySelectorToClick, "", HTMLTags.HtmlAttribute.AriaLabel, "Send now"))
                                .Success;

                        if (!resp)
                            resp = _automationExtension.ExecuteScript(AttributeIdentifierType.ClassName,
                                    "button-primary-medium connect-cta-form__send")
                                .Success;
                    }
                    pageSource = BrowserWindow.GetPageSource();
                    if (pageSource.Contains("artdeco-modal artdeco-modal--layer-default ip-fuse-limit-alert") ||
                            pageSource.Contains("You’re out of invitations for now"))
                    {
                        return new ResponseParameter { Response = LdConstants.YouHaveReachedWeeklyInvitationLimit };
                    }
                    if (pageSource.Contains("Vous avez atteint la limite d’invitations hebdomadaire"))
                    {
                        return respParam = new ResponseParameter { Response = "Vous avez atteint la limite d’invitations hebdomadaire" };
                    }

                }
                if (string.IsNullOrEmpty(respParam.Response))
                    respParam = new ResponseParameter { Response = "" };
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return respParam;
        }

        public IResponseParameter SendConnectionRequestWithoutVistingProfile(string postString, string nodeId,
            string flagship3ProfileViewBase)
        {
            IResponseParameter respParam = null;
            try
            {
                _automationExtension.ScrollWindow(isDown: false);
                BrowserWindow.ExecuteScript($"document.querySelector('#{nodeId}').scrollIntoViewIfNeeded();");
                var ConnectionPageSource = BrowserWindow.GetPageSource();
                var buttonNode = HtmlAgilityHelper.MethodGetInnerStringFromId(ConnectionPageSource, nodeId);
                if (!string.IsNullOrEmpty(buttonNode) && (buttonNode.Contains("Message") || buttonNode.Contains("Follow") || buttonNode.Contains("Pending")))
                    return respParam = new ResponseParameter { Response = LdConstants.InvitationHasBeenSent };
                BrowserWindow.ExecuteScript($"document.querySelector('#{nodeId}').click();");
                //var Success = _automationExtension.ExecuteScript(AttributeIdentifierType.Xpath, "//span[text()='Connect']").Success;
                Thread.Sleep(5000);
                var resp = false;
                if (string.IsNullOrWhiteSpace(postString))
                {
                    resp = _automationExtension.ExecuteScript(AttributeIdentifierType.Xpath, $"//{HTMLTags.Span}[text()='Done']", 5)
                        .Success;
                    if (!resp)
                        resp = _automationExtension
                            .ExecuteScript(AttributeIdentifierType.Xpath, $"//{HTMLTags.Span}[text()='Send now']", 5).Success;
                    if (!resp)
                        resp = _automationExtension
                            .ExecuteScript(AttributeIdentifierType.Xpath, $"//{HTMLTags.Span}[text()='Send']", 5).Success;
                }
                else
                {
                    _automationExtension.ExecuteScript(string.Format(LDClassesConstant.ScriptConstant.ScriptWithQuerySelectorToClick,HTMLTags.Button,HTMLTags.HtmlAttribute.AriaLabel, "Add a note"), 2);
                    // here sometimes button is disabled unable to execute javascript
                    BrowserWindow.EnterChars($" {postString} ", 0.2);
                    resp=_automationExtension.ExecuteScript(string.Format(LDClassesConstant.ScriptConstant.ScriptWithQuerySelectorToClick, HTMLTags.Button, HTMLTags.HtmlAttribute.AriaLabel, "Send now"), 2).Success;
                }
                var connectionPageSource = BrowserWindow.GetPageSource();
                if (connectionPageSource.Contains(LdConstants.UnableToConnectToUser("")) || connectionPageSource.Contains(LdConstants.YourInvitationToConnectWasNotSent))
                    return respParam = new ResponseParameter { Response = LdConstants.YourInvitationToConnectWasNotSent};
                if (resp)
                    respParam = new ResponseParameter { Response = LdConstants.InvitationSentSuccessFully };
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return respParam;
        }



        public IResponseParameter FollowComapanyPages(string postString, string actionUrl)
        {
            _automationExtension.LoadPageUrlAndWait(actionUrl,13);
            if (_automationExtension.LoadAndClick(AttributeTypes.Button, AttributeIdentifierType.ClassName,
                "org-company-follow-button org-top-card-primary-actions__action artdeco-button artdeco-button--primary"))
            {
                //Turn on Notification of page after follow action.
                _automationExtension.ExecuteScript(AttributeIdentifierType.Xpath, "//span[text()=\"Turn on\"]");
                return new ResponseParameter { Response = "" };
            }
            return new ResponseParameter { Response = null };
        }

        //not needed till yet
        public string GetUploadResponse(string actionUrl, string encryptionId, string encryption, FileInfo objFileInfo)
        {
            return "";
        }

        public void SetWebRequestParametersforjobsearchurl(string referer, string dFlagship3SearchSrpPeople)
        {
        }

        public void SetWebRequestParametersforChangingProfilePic(string referer, string dFlagship3SearchSrpPeople)
        {
        }

        public void SetWebRequestParametersforCaptcha(string referer)
        {
        }

        public string RemoveConnection(LinkedinUser linkedInUser)
        {
            try
            {
                if (!BrowserWindow.CurrentUrl().Contains(linkedInUser.ProfileUrl))
                {
                    BrowserWindow.Browser.Load(linkedInUser.ProfileUrl);
                    Thread.Sleep(15000);
                }
                _automationExtension
                                 .ExecuteScript(AttributeIdentifierType.Xpath, $"//{HTMLTags.Span}[text()='Remove Connection']");
                return "";
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public string WithDrawConnectionRequest(LinkedinUser linkedInUser)
        {
            try
            {
                var count = 0;
                CheckAndAssignBrowser();
                var isSuccess = false;
                if (!BrowserWindow.CurrentUrl().Contains("https://www.linkedin.com/mynetwork/invitation-manager/sent/"))
                    _automationExtension.LoadAndScroll("https://www.linkedin.com/mynetwork/invitation-manager/sent/",
                        15, true, 5000);

                else
                    _automationExtension.ScrollWindow(5000);
                string CurrentNodeId;
                if (string.IsNullOrEmpty(linkedInUser.NodeResponse))
                {
                    var getAllNode = HtmlAgilityHelper.GetListNodesFromClassName(BrowserWindow.GetPageSource(),
                    "invitation-card invitation-card--selectable ember-view");
                    do
                    {
                        count++;
                        _automationExtension.LoadAndScroll(
                            $"https://www.linkedin.com/mynetwork/invitation-manager/sent/?invitationType=&page={count}", 15,
                            true, 5000);
                        getAllNode = HtmlAgilityHelper.GetListNodesFromClassName(BrowserWindow.GetPageSource(),
                            "invitation-card invitation-card--selectable ember-view");
                        if (getAllNode.Count <= 0)
                            getAllNode = HtmlAgilityHelper.GetListNodesFromClassName(BrowserWindow.GetPageSource(),
                            "invitation-card artdeco-list__item");
                    } while (!getAllNode.Any(x => x.OuterHtml.Contains(linkedInUser.FullName)));
                    var Node = getAllNode.Where(x => x.OuterHtml.Contains(linkedInUser.FullName))?.FirstOrDefault();
                    CurrentNodeId = HtmlAgilityHelper.GetListNodesFromClassName(Node?.OuterHtml, HTMLTags.Button)?.FirstOrDefault()?.Id;
                }
                else
                {
                    CurrentNodeId = _automationExtension.GetPath(linkedInUser.NodeResponse, HTMLTags.Button, AttributeIdentifierType.Id, "Withdraw");
                }
                _automationExtension.ExecuteScript($"document.getElementById('{CurrentNodeId}').scrollIntoView();");
                isSuccess = _automationExtension
                    .ExecuteScript(string.Format(LDClassesConstant.ScriptConstant.ScriptGetElementByIdToClick, CurrentNodeId)).Success;
                // clicking on pop up dialog of withdraw button
                CurrentNodeId = _automationExtension.GetPath(BrowserWindow.GetPageSource(), HTMLTags.Button,
                    AttributeIdentifierType.Id, "data-test-dialog-primary-btn");
                isSuccess = _automationExtension
                    .ExecuteScript(string.Format(LDClassesConstant.ScriptConstant.ScriptGetElementByIdToClick, CurrentNodeId)).Success;
                if (isSuccess)
                    return $"urn:li:fs_relInvitation:{linkedInUser.InvitationId}";

                Thread.Sleep(15000);
            }
            catch (Exception exception)
            {
                exception.DebugLog();
            }

            return "";
        }

        public string ByteEncodedPostResponse(string actionUrl, string postString)
        {
            try
            {
                return null;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public string UploadImageAndGetContentIdForMessaging(string imageUploadUrl, FileInfo fileInfo,
            string message = "", List<string> Medias = null)
        {
            try
            {
                // closing all the opened conversation window because more than 3 window not get opened 
                while (_automationExtension.LoadAndClick(HTMLTags.Button, AttributeIdentifierType.Id,
                    LDClassesConstant.Messenger.CloseConversationWindow));
                // if imageUploadUrl not contains http means user sending message from groups to its members
                var MessageButtonClass = LDClassesConstant.Messenger.MessageButtonClass;
                if (imageUploadUrl.Contains("https"))
                {
                    if (!BrowserWindow.CurrentUrl().Contains(imageUploadUrl))
                        _automationExtension.LoadPageUrlAndWait(imageUploadUrl, 15);
                    var pageResponse = BrowserWindow.GetPageSource();
                    if (!pageResponse.Contains(LDClassesConstant.Messenger.ConnectButtonClass))
                    {
                        Thread.Sleep(15000);
                        var success = _automationExtension.ExecuteScript(string.Format(LDClassesConstant.ScriptConstant.ScriptGetElementsByClassNameToClick,MessageButtonClass,1), 5).Success;
                        if (!success)
                            _automationExtension.LoadAndClick(AttributeTypes.Button,
                                AttributeIdentifierType.ClassName, $"{MessageButtonClass}");
                    }
                    else
                    {
                        _automationExtension.LoadPageUrlAndWait(imageUploadUrl, 15);
                        _automationExtension.LoadAndClick(AttributeTypes.Button,
                            AttributeIdentifierType.ClassName, "More…");
                        _automationExtension.ExecuteScript(string.Format(LDClassesConstant.ScriptConstant.ScriptGetElementsByClassNameToClick, "message-anywhere-button link-without-visted-state t-black--light t-normal pv-s-profile-actions__label display-flex", 0),
                            5);
                        BrowserWindow.PressAnyKey(winKeyCode: 9);
                    }
                }
                else
                {
                    var nodes = HtmlAgilityHelper.GetListNodesFromClassName(BrowserWindow.GetPageSource(),
                        "artdeco-list__item groups-members-list__typeahead-result relative p0");
                    //groups - members - list__typeahead - result relative ember-view
                    if (nodes.Count == 0)
                    {
                        Application.Current.Dispatcher.Invoke(() => { BrowserWindow.GoBack(); });
                        Thread.Sleep(15000);
                        _automationExtension.ScrollWindow(5000, isDocumentEnd: true);
                        nodes = HtmlAgilityHelper.GetListNodesFromClassName(BrowserWindow.GetPageSource(),
                            "groups-members-list__typeahead-result relative artdeco-typeahead__result ember-view");
                    }

                    var id = "";
                    foreach (var htmlNode in nodes)
                        if (htmlNode.OuterHtml.Contains(imageUploadUrl))
                        {
                            id = Utils.GetBetween(htmlNode.OuterHtml, $"aria-label=\"Message {imageUploadUrl}\"", "class=\"")
                                .Replace("id=", "").Replace("\"", "")?.Trim();
                            break;
                        }
                    if (string.IsNullOrEmpty(id))
                    {
                        _automationExtension.ExecuteScript(string.Format(LDClassesConstant.ScriptConstant.ScriptWithQuerySelectorToClick,"",HTMLTags.HtmlAttribute.AriaLabel,$"Send message to {imageUploadUrl}"));
                    }
                    else
                    {
                        _automationExtension.ExecuteScript(AttributeIdentifierType.Xpath,
                            $"//{HTMLTags.Button}[@id='{id}']//{HTMLTags.Span}[text()='Message']");
                    }

                }

                // trimming message and media path 
                message = message.Contains("<:>") ? message.Substring(0, message.IndexOf("<:>")) : message;
                // entering characters
                var Messages = !string.IsNullOrEmpty(message)?message?.Replace("\r","").Split('\n') : new string[0];
                Messages.ForEach(msg =>
                {
                    BrowserWindow.EnterChars(" " + msg,delayAtLast:1);
                    if (msg != Messages.LastOrDefault())
                        BrowserWindow.PressAnyKey(1, winKeyCode: 13, isShiftDown: true,delayAtLast:1);
                });
                if (fileInfo != null)
                {
                    
                    // here we scrolling to top so that it will get exact position of upload media button
                    _automationExtension.ExecuteScript(string.Format(LDClassesConstant.ScriptConstant.ScrollWindow,0,0));
                    _automationExtension.ExecuteScript(string.Format(LDClassesConstant.ScriptConstant.ScrollWindowToXXPixel, 0, 0));
                    var axis = _automationExtension.GetXAndYPositionByScript(AttributeTypes.Button,
                        AttributeIdentifierType.Id,
                        "Attach an image");
                    if(Medias!=null && Medias.Count > 0)
                    {
                        foreach (var media in Medias)
                        {
                            BrowserWindow.ChooseFileFromDialog(media);
                            BrowserWindow.MouseClick(axis.Key + 5, axis.Value + 5, delayAfter: 5);
                        }
                    }
                    else
                    {
                        BrowserWindow.ChooseFileFromDialog(fileInfo.FullName);
                        BrowserWindow.MouseClick(axis.Key + 5, axis.Value + 5, delayAfter: 25);
                    }
                    _automationExtension.ExecuteScript(string.Format(LDClassesConstant.ScriptConstant.ScrollWindow, 0, "document.documentElement.scrollHeight"), 5);
                }
                // This is for if user select the enter to send message in message process
                if (BrowserWindow.GetPageSource().Contains("msg-form__hint-text t-12 t-black--light t-normal"))
                {
                    BrowserWindow.PressAnyKey(1, winKeyCode: 13);
                    var pagresponse = BrowserWindow.GetPageSource();
                    if (!pagresponse.Contains("artdeco-toast-item__icon artdeco-toast-item__icon--error"))
                        return "{\"value\":{\"createdAt\":";
                }
                else
                {
                    var SendMessageButtonClass = LDClassesConstant.Messenger.SendMessageButtonClass;
                    var ListOFSendButton = _automationExtension.GetPathList(BrowserWindow.GetPageSource(),HTMLTags.Button, AttributeIdentifierType.ClassName, false, SendMessageButtonClass);
                    var ClickIndex = ListOFSendButton.Count > 1 ? ListOFSendButton.Count-1 : 0;
                    // finally executing sending message part
                    if (_automationExtension.ExecuteScript(string.Format(LDClassesConstant.ScriptConstant.ScriptGetElementsByClassNameToClick,SendMessageButtonClass,ClickIndex))
                        .Success)
                    {
                        var pagresponse = BrowserWindow.GetPageSource();
                        if (!pagresponse.Contains("artdeco-toast-item__icon artdeco-toast-item__icon--error"))
                            return "{\"value\":{\"createdAt\":";
                        _automationExtension.ExecuteScript(string.Format(LDClassesConstant.ScriptConstant.ScriptGetElementsByClassNameToClick, "artdeco-toast-item__dismiss artdeco-button artdeco-button--circle artdeco-button--muted artdeco-button--1 artdeco-button--tertiary ember-view", 0));
                    }
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return null;
        }

        public string GetSingleUploadResponse(string singleUploadUrl, FileInfo objFileInfo, string referer)
        {
            return "";
        }


        /// <summary>
        ///     here we performing publishing media in wall and group part
        /// </summary>
        /// <param name="isPublishOnOwnWall"></param>
        /// <param name="actionUrl"></param>
        /// <param name="postString"></param>
        /// <param name="dFlagship3Feed"></param>
        /// <returns></returns>
        public string FinalPostRequest(bool isPublishOnOwnWall, string actionUrl, string postString,
            string dFlagship3Feed, string title)
        {
            var response = "";
            var isVideo = false;
            var isDocPdf = false;

            try
            {
                var listOfImageOrVideo = postString.Split(',').ToList();
                if (isPublishOnOwnWall)
                {
                    if (!_automationExtension.ExecuteScript(AttributeIdentifierType.ClassName,
                        LDClassesConstant.SocioPublisherPost.ClassForMediaUploadPanel1).Success)
                        if (!_automationExtension.ExecuteScript(AttributeIdentifierType.ClassName,
                        LDClassesConstant.SocioPublisherPost.ClassForMediaUploadPanel2).Success)
                        {
                            Thread.Sleep(3000);
                            if (!_automationExtension.ExecuteScript("[...document.querySelectorAll('button')].filter(x=>x.innerText==\"Create\"||x.innerText==\"Start a post\")[0].click();", delayInSec: 2).Success)
                                return response;
                            else
                                _automationExtension.ExecuteScript("document.querySelector('a[data-test-org-menu-item=\"POSTS\"]').click();", delayInSec: 2);
                        }
                }
                else
                {
                    if (!_automationExtension.ExecuteScript(AttributeIdentifierType.ClassName, LDClassesConstant.SocioPublisherPost.ClassForMediaUploadPanelOnGroup).Success)
                    {
                        Thread.Sleep(3000);
                        if (!_automationExtension.ExecuteScript("[...document.querySelectorAll('button')].filter(x=>x.innerText==\"Create\" || x.innerText == \"Start a public post\")[0].click();", delayInSec:2).Success)
                            return response;
                        else
                            _automationExtension.ExecuteScript("document.querySelector('a[data-test-org-menu-item=\"POSTS\"]').click();", delayInSec:2);
                    }
                }
                Thread.Sleep(10000);
                //to add description 
                var descriptions = Regex.Split(string.IsNullOrEmpty(actionUrl)?string.IsNullOrEmpty(title)?string.Empty:title :actionUrl, "\r\n");
                descriptions.ForEach(description =>
                {
                    BrowserWindow.EnterChars(" " + description, delayAtLast: 2);
                    if (description != descriptions.LastOrDefault())
                        BrowserWindow.PressAnyKey(winKeyCode:13);
                });
                //to upload images or videos
                if (!string.IsNullOrEmpty(postString) && !(postString.StartsWith("https") || postString.StartsWith("http")))
                {
                    var fileInfo = new FileInfo(listOfImageOrVideo[0]);
                    isVideo = ConstantVariable.SupportedVideoFormat.Contains(fileInfo.Extension.Replace(".", ""));
                    isDocPdf = fileInfo.Extension.Contains(".doc") || fileInfo.Extension.Contains(".docx") || fileInfo.Extension.Contains(".pdf") || fileInfo.Extension.Contains(".ppt") || fileInfo.Extension.Contains(".pptx");
                    var fileDialogHandler = new TempFileDialogHandler(BrowserWindow, listOfImageOrVideo);
                    BrowserWindow.Browser.DialogHandler = fileDialogHandler;
                    if (isVideo)
                    {
                        _automationExtension.ScrollPostWindow(500);
                        var XandY = GetXYForMediaUpload(BrowserWindow,isVideo,false,false);
                        BrowserWindow.MouseClick(XandY.Key+10,XandY.Value+10, delayAfter: 5);
                        var pageSource = BrowserWindow.GetPageSource();
                        //if auto caption popup appear just close it.
                        _automationExtension.ExecuteScript(AttributeIdentifierType.Xpath, $"//{HTMLTags.Span}[text()='Got it']", 2);
                        if (pageSource.Contains(LdConstants.TooSmallFile))
                            return LdConstants.TooSmallFile;
                        _automationExtension.ExecuteScript(AttributeIdentifierType.Xpath, $"//{HTMLTags.Span}[text()='Next']", 5);
                        _automationExtension.ScrollPostWindow(1500);
                        _automationExtension.ExecuteScript(AttributeIdentifierType.Xpath, $"//{HTMLTags.Span}[text()='Done']", 5);
                        _automationExtension.ExecuteScript(AttributeIdentifierType.Xpath, $"//{HTMLTags.Span}[text()='Post']", 3);
                        pageSource = BrowserWindow.GetPageSource();
                        var stopWatch = new Stopwatch();
                        stopWatch.Start();
                        var isFirst = true;
                        while (pageSource.Contains(LDClassesConstant.SocioPublisherPost.ClassForVideoUploadProcessingLabel))
                        {
                            if(isFirst)
                                GlobusLogHelper.log.Info(Log.CustomMessage,"LinkedIn","", "Publish","Please Wait While Uploading Video Is On Processing.....");
                            isFirst = false;
                            Thread.Sleep(15000);
                            pageSource = BrowserWindow.GetPageSource();
                            if (stopWatch.Elapsed.Minutes > 10)
                                break;
                        }
                        stopWatch.Stop();
                        _automationExtension.ScrollPostWindow(1500);
                    }
                    else if (isDocPdf)
                    {
                        var AddDocument = _automationExtension.ExecuteScript(AttributeIdentifierType.Xpath, $"//{HTMLTags.Span}[text()='Add a document']");
                        GetXYForMediaUpload(BrowserWindow, false, isDocPdf, false);
                        Thread.Sleep(4000);
                        //to add title 
                        var axisfortitle = _automationExtension.GetXAndY(LDClassesConstant.SocioPublisherPost.ClassForTitleOfDocument,
                        AttributeIdentifierType.ClassName);
                        BrowserWindow.MouseClick(axisfortitle.Key + 15, axisfortitle.Value + 15, delayAfter: 5);
                        AssignFileNameAsTitleIfEmpty(listOfImageOrVideo.FirstOrDefault(),ref title);
                        BrowserWindow.EnterChars(" " + title, typingDelay: 0.10, delayAtLast: 2);
                        var done = _automationExtension.ExecuteScript(AttributeIdentifierType.Xpath, $"//{HTMLTags.Span}[text()='Done']").Success;
                        Thread.Sleep(10000);
                        if (done)
                        {
                            var succ = _automationExtension
                                .ExecuteScript(AttributeIdentifierType.Xpath, $"//{HTMLTags.Span}[text()='Post']").Success;
                        }
                        var count = 0;
                        WaitForUpload:
                        Thread.Sleep(10000);
                        while(count++<=5 && BrowserWindow.GetPageSource().Contains("share-status-container__preview")|| BrowserWindow.GetPageSource().Contains("Processing"))
                            goto WaitForUpload;
                    }
                    else
                    {
                        var XandY = GetXYForMediaUpload(BrowserWindow, false, false, true);
                        BrowserWindow.MouseClick(XandY.Key+10,XandY.Value+10, delayAfter: 5);
                        var Success = BrowserWindow.ExecuteScript(string.Format(LDClassesConstant.ScriptConstant.ScriptWithQuerySelectorAllToClick, HTMLTags.Button, HTMLTags.HtmlAttribute.Class, "ml2 artdeco-button artdeco-button--2 artdeco-button--primary ember-view", 0)).Success;
                        Thread.Sleep(10000);
                        if (Success)
                        {
                            Success = BrowserWindow.ExecuteScript(string.Format(LDClassesConstant.ScriptConstant.ScriptWithQuerySelectorAllToClick, HTMLTags.Button, HTMLTags.HtmlAttribute.Class, "share-actions__primary-action artdeco-button artdeco-button--2 artdeco-button--primary ember-view", 0)).Success;
                            Thread.Sleep(10000);
                        }
                        if (!Success)
                            Success = _automationExtension
                                .ExecuteScript(AttributeIdentifierType.Xpath, $"//{HTMLTags.Span}[text()='Done']").Success;
                        if (!Success)
                            Success = _automationExtension
                                .ExecuteScript(AttributeIdentifierType.Xpath, $"//{HTMLTags.Span}[text()='Next']").Success;
                        _automationExtension.ExecuteScript(AttributeIdentifierType.Xpath, $"//{HTMLTags.Span}[text()='Not now']");
                        Thread.Sleep(1000);
                        _automationExtension.ExecuteScript(AttributeIdentifierType.Xpath, $"//{HTMLTags.Span}[text()='Post']");
                        Thread.Sleep(10000);
                        _automationExtension.ExecuteScript(AttributeIdentifierType.Xpath, $"//{HTMLTags.Span}[text()='Not now']");
                        Thread.Sleep(5000);
                        _automationExtension.ScrollPostWindow(8000,false);
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(postString))
                    {
                        BrowserWindow.EnterChars(" " + postString, typingDelay: 0.10, delayAtLast: 2);
                        Thread.Sleep(10000);
                        BrowserWindow.SelectAllText();
                        BrowserWindow.PressAnyKey(winKeyCode: 8, delayAtLast: 5);
                    }
                    _automationExtension.ExecuteScript(AttributeIdentifierType.Xpath, $"//{HTMLTags.Span}[text()='Post']");
                    Thread.Sleep(5000);
                    var pageSource = BrowserWindow.GetPageSource();
                    if (!string.IsNullOrEmpty(pageSource) && pageSource.Contains(LdConstants.UnableToSharePost))
                        return LdConstants.UnableToSharePost;
                    _automationExtension.ExecuteScript(AttributeIdentifierType.Xpath, $"//{HTMLTags.Span}[text()='No thanks']");
                    Thread.Sleep(5000);
                    _automationExtension.ExecuteScript(AttributeIdentifierType.Xpath, $"//{HTMLTags.Span}[text()='Not now']");
                    Thread.Sleep(5000);
                    _automationExtension.ScrollPostWindow(8000, false);
                }
                response = isDocPdf ? GetDocPostLink(BrowserWindow.GetPageSource()) : GetActivityUrl(isVideo, dFlagship3Feed, title, actionUrl);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                response = null;
            }

            return string.IsNullOrEmpty(response)?GetActivityUrl(isVideo,dFlagship3Feed,title,actionUrl):response;
        }

        private void AssignFileNameAsTitleIfEmpty(string FilePath, ref string title)
        {

            try
            {
                if(!string.IsNullOrEmpty(title)) return;
                var objDetailedFileInfo = new FileInfo(FilePath);
                title = objDetailedFileInfo.Name?.Split('.')?.FirstOrDefault();
            }
            catch (Exception)
            {
            }
            finally {
                title = title.Length >= 58 ? title.Substring(0, 50) : title;
            }
        }

        private KeyValuePair<int,int> GetXYForMediaUpload(BrowserWindow browserWindow,bool isVideo=false,bool isDoc=false,bool isImage=false)
        {
            var BrowserExtension = new BrowserAutomationExtension(browserWindow);
            var mediaUploadClass = LDClassesConstant.SocioPublisherPost.ClassForPickImageOrVideoFromDialog;
            var documentUploadClass = LDClassesConstant.SocioPublisherPost.ClassForPickDocumentFromDialog;
            var documentUploadClass1 = LDClassesConstant.SocioPublisherPost.ClassForPickDocumentFromDialog1;
            var pageSource = browserWindow.GetPageSource();
            var isUniqueAccount = IsUniqueAccount(pageSource, mediaUploadClass) || IsUniqueAccount(pageSource, documentUploadClass);
            mediaUploadClass = IsUniqueAccount(pageSource, mediaUploadClass) ? mediaUploadClass : IsUniqueAccount(pageSource, documentUploadClass) ? documentUploadClass : documentUploadClass1;
            if (isUniqueAccount)
            {
                var UploadClass =isVideo ? mediaUploadClass + 2 : isImage ? mediaUploadClass + 1 : documentUploadClass + 4;
                if (isDoc)
                {
                    //Click on more option to get X and Y for document option.
                    var XandY = BrowserExtension.GetXAndY(documentUploadClass + "more", AttributeIdentifierType.ClassName);
                    BrowserWindow.MouseClick(XandY.Key + 10, XandY.Value + 10, delayAfter: 4);
                    Thread.Sleep(2000);
                    XandY = BrowserExtension.GetXAndY(UploadClass, AttributeIdentifierType.ClassName);
                    BrowserWindow.MouseClick(XandY.Key + 10, XandY.Value + 10, delayAfter: 3);
                    Thread.Sleep(3000);
                    XandY = BrowserExtension.GetXAndY(documentUploadClass1, AttributeIdentifierType.ClassName);
                    BrowserWindow.MouseClick(XandY.Key + 10, XandY.Value + 10, delayAfter: 3);
                }
                return BrowserExtension.GetXAndY(UploadClass, AttributeIdentifierType.ClassName);
            }
            else
            {
                var uploadClass = LDClassesConstant.SocioPublisherPost.ClassForPickImageOrVideoFromDialog1;
                if(IsUniqueAccount(pageSource, uploadClass))
                {
                    var Index = isVideo || isImage ? 0 : 3;
                    if (isDoc)
                    {
                        BrowserWindow.ExecuteScript(string.Format(LDClassesConstant.ScriptConstant.ScriptGetElementsByClassNameToClick,uploadClass,Index), 2);
                        pageSource = browserWindow.GetPageSource();
                        var Nodes = HtmlAgilityHelper.GetListNodesFromAttibute(pageSource,HTMLTags.Button,AttributeIdentifierType.ClassName,null, "share-promoted-detour-button");
                        Index = Nodes != null && Nodes.Count > 0 ?Nodes.IndexOf(Nodes.FirstOrDefault(x=>x.OuterHtml.Contains("Add a document"))) : 6;
                        BrowserWindow.ExecuteScript(string.Format(LDClassesConstant.ScriptConstant.ScriptGetElementsByClassNameToClick, uploadClass, Index), 2);
                        goto DocumentUpload;
                    }
                    return new KeyValuePair<int, int>(Convert.ToInt32(BrowserWindow.ExecuteScript(string.Format(LDClassesConstant.ScriptConstant.ScriptGetElementsByClassNameToGetXY,uploadClass,Index, "x")).Result), Convert.ToInt32(BrowserWindow.ExecuteScript(string.Format(LDClassesConstant.ScriptConstant.ScriptGetElementsByClassNameToGetXY,uploadClass,Index, "y")).Result));
                }
                DocumentUpload:
                var mediaType = isVideo ? "video" : isImage ? "image" :documentUploadClass1;
                var Tag = isVideo || isImage ? HTMLTags.ListIcon : HTMLTags.Input;
                var Type = isVideo || isImage ? HTMLTags.HtmlAttribute.Type : HTMLTags.HtmlAttribute.Class;
                var XandY = new KeyValuePair<int, int>(Convert.ToInt32(BrowserWindow.ExecuteScript(string.Format(LDClassesConstant.ScriptConstant.ScriptWithQuerySelectorAllToGetXandY, Tag, Type, mediaType, 0, "x")).Result), Convert.ToInt32(BrowserWindow.ExecuteScript(string.Format(LDClassesConstant.ScriptConstant.ScriptWithQuerySelectorAllToGetXandY, Tag, Type, mediaType, 0, "y")).Result));
                if(isDoc && !isUniqueAccount)
                {
                    BrowserWindow.MouseClick(XandY.Key + 10, XandY.Value + 10, delayAfter: 3);
                    Thread.Sleep(3000);
                }
                return XandY;
            }
        }
        private bool IsUniqueAccount(string PageSource,string ContainsString)
        {
            return PageSource.Contains(ContainsString) || PageSource.Contains(ContainsString);
        }
        private string GetDocPostLink(string pageResponse)
        {
            //get post link here
            pageResponse =
                    Utilities.GetBetween(
                        HtmlAgilityHelper.GetStringFromClassName(pageResponse, "artdeco-toast-item__cta"), "href=\"",
                        "\"");
            return pageResponse;
        }

        public void SetWenRequestparamtersForsalesUrl(string refer,bool IsSalesWebRequest=false)
        {
            ldFunctions.SetDominatorAccount(_linkedInAccountModel);
            ldFunctions.SetWenRequestparamtersForsalesUrl(refer, IsSalesWebRequest);
        }

        private string GetActivityUrl(bool isVideo, string destination,string title,string description)
        {
            var response = "";
            if (!isVideo)
            {
                Thread.Sleep(10000);
                var OnProcessingPostClass = LDClassesConstant.SocioPublisherPost.ClassForVideoUploadProcessingLabel;
                var pageResponse = BrowserWindow.GetPageSource();
                var Title = string.IsNullOrEmpty(title) ?string.IsNullOrEmpty(description)?string.Empty:description:title;
                var Description = string.IsNullOrEmpty(description) ? string.Empty : description;
                Description = Description?.Replace("\r","")?.Replace("\n","<br>")?.Replace("\"","\\\"");
                var lastSharedFeedClass = destination.Equals("OnPage") ? "artdeco-card mb2" : "ember-view  occludable-update ";
                var lastSharedFeedNodes = HtmlAgilityHelper.GetListNodesFromClassName(pageResponse, lastSharedFeedClass);
                HtmlNode lastSharedFeedNode = null;
                if (!string.IsNullOrEmpty(Title)||!string.IsNullOrEmpty(Description))
                    lastSharedFeedNode = destination.Equals("OnPage") ?lastSharedFeedNodes.FirstOrDefault(x => x.InnerHtml.Contains("urn:li:activity:")||x.InnerHtml.Contains(Title) || x.InnerHtml.Contains(Description)) : lastSharedFeedNodes.FirstOrDefault(x=> x.InnerHtml.Contains(Title) || x.InnerHtml.Contains(Description));
                else
                    lastSharedFeedNode = destination.Equals("OnPage") ? lastSharedFeedNodes.FirstOrDefault(x => x.InnerHtml.Contains("urn:li:activity:")) : lastSharedFeedNodes.FirstOrDefault();
                lastSharedFeedNode = lastSharedFeedNode==null && pageResponse.Contains(OnProcessingPostClass) ?HtmlAgilityHelper.GetListNodesFromClassName(pageResponse,OnProcessingPostClass).FirstOrDefault(x=>x.InnerHtml.Contains("urn:li:")): lastSharedFeedNode;
                lastSharedFeedNode = lastSharedFeedNode == null ? lastSharedFeedNodes.FirstOrDefault(x=>x.InnerHtml.Contains("urn:li:")):lastSharedFeedNode;
                if (!string.IsNullOrEmpty(lastSharedFeedNode.InnerHtml))
                {
                    var ActivityId = Utils.GetBetween(lastSharedFeedNode.InnerHtml, "data-urn=\"", "\" role=\"region\">");
                    ActivityId =string.IsNullOrEmpty(ActivityId)? Utils.GetBetween(lastSharedFeedNode.InnerHtml, "data-urn=\"", "\">"):ActivityId;
                    ActivityId = string.IsNullOrEmpty(ActivityId) ?Utils.GetBetween(lastSharedFeedNode.InnerHtml,"/update/","\" target"): ActivityId;
                    response = LdConstants.GetSharedFeedAPI(ActivityId);
                    if (pageResponse.Contains("artdeco-modal artdeco-modal--layer-default "))
                        _automationExtension
                                       .ExecuteScript(AttributeIdentifierType.Xpath, $"//{HTMLTags.Span}[text()='Not now']");
                }
                else
                    response = Utilities.GetBetween(
                        HtmlAgilityHelper.GetStringFromClassName(pageResponse, "artdeco-toast-item__cta"), "href=\"",
                        "\"");
                if (pageResponse.Contains("Not now") && pageResponse.Contains("Post successful"))
                {
                    pageResponse = BrowserWindow.GetPageSource();
                    //gettng post url 
                    response = Utils.GetBetween(HtmlAgilityHelper.GetStringFromClassName(pageResponse, "post-post-framework-toast-cta t-black"), "href=\"", "\"");
                    _automationExtension.ExecuteScript(AttributeIdentifierType.Xpath, $"//{HTMLTags.Span}[text()='Not now']");
                    Thread.Sleep(3000);
                }
                return response;
            }
            _linkedInAccountModel.ExtraParameters.TryGetValue("PublicIdentifier", out _);
            response = GetResponseAfterVideoUploaded();
            return response;
        }

        // getting media id after video is uploaded of the posted video
        private string GetResponseAfterVideoUploaded()
        {
            string response = "";
            var isGetPostedCount = 0;
            // here we refreshing the page so that page have latest upload post 
            while (isGetPostedCount <=4)
            {
                var htmlResponse = BrowserWindow.GetPageSource();
                var lasVideoPostedNode = HtmlAgilityHelper.GetListNodesFromClassName(htmlResponse, "ember-view  occludable-update ").FirstOrDefault();
                var postId = Utilities.GetBetween(lasVideoPostedNode.InnerHtml,"data-urn=\"","\">");
                response = $"https://www.linkedin.com/feed/update/{postId}/";
                if (response.Contains("urn:li:"))
                {
                    return response;
                }
                Thread.Sleep(TimeSpan.FromSeconds(15));
            }

            return response;
        }
        public string FinalPostRequest_VideoUploading(string actionUrl, string postString)
        {
            return "";
        }
        public string FinalPostRequest_DocumentUploading(string actionUrl, string postString,string dFlagship3Feed)
        {
            return "";
        }
        public string GetDocfileUploadResponseStatus(string actionUrl, string dFlagship3Feed)
        {
            return "";
        }
        public string GetputresponseforDocPdfFiles(string url, byte[] FileByte,string FileContentType)
        {
            return "";
        }
        public string SendGroupJoinRequest(string actionUrl)
        {
            var response = string.Empty;
            try
            {
                var pageSource = BrowserWindow.GetPageSource();
                // here we getting id of the button contains 'Request to join'


                var buttonId = _automationExtension.GetPath(pageSource,HTMLTags.Button, AttributeIdentifierType.Id,
                    "Request to join");
                if (string.IsNullOrEmpty(buttonId))
                    buttonId = _automationExtension.GetPath(pageSource,HTMLTags.Button, AttributeIdentifierType.Id,
                   "Join");
                pageSource = BrowserWindow.GetPageSource();


                if (string.IsNullOrEmpty(buttonId) && pageSource.Contains("Withdraw Request"))
                    return response = "Withdraw Request";


                if (_automationExtension.ExecuteScript(string.Format(LDClassesConstant.ScriptConstant.ScriptGetElementByIdToClick,buttonId), 5).Success)
                {
                    pageSource = BrowserWindow.GetPageSource();
                    var htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(pageSource);
                    var Caption = Utils.RemoveHtmlTags(HtmlAgilityHelper.GetStringInnerHtmlFromClassName("",
                        "artdeco-toast-item__message", htmlDoc));
                    if (Caption.Contains("You have reached your request limit"))
                        return response = "You have reached your request limit";
                    if (pageSource.Contains("Cancel Request") || Caption.Contains("Successfully sent the request") ||
                        pageSource.Contains("Your join request is sent to the group admin. We will notify you once approved.") ||
                        pageSource.Contains("You are in! Join the conversations with group members.") || Caption.Contains("Your join request is sent to the group admin. We’ll notify you once approved."))
                        return
                            $"urn:li:fs_groupFollowingInfo:{Utils.GetBetween(actionUrl + "$$", "https://www.linkedin.com/groups/", "$$")?.Replace("/", "")}";
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return response;
        }

        public string UnJoinGroupsRequest(string actionUrl, string postString)
        {
            string response = null;
            try
            {
                _automationExtension.LoadPageUrlAndWait(actionUrl);
                var page = BrowserWindow.GetPageSource();
                if (page.Contains("white-space-nowrap mt4 artdeco-button artdeco-button--2 artdeco-button--primary ember-view") && page.Contains("Request to join"))
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                    _linkedInAccountModel.AccountBaseModel.AccountNetwork,
                    _linkedInAccountModel.AccountBaseModel.UserName, ActivityType.AutoReplyToNewMessage,
                    "Already unjoin that Group");
                    return null;
                }
                else
                {
                    var groupId = _ldDataHelper.GetGroupIdFromGroupUrl(actionUrl);
                    _automationExtension.ExecuteScript(string.Format(LDClassesConstant.ScriptConstant.ScriptGetElementsByClassNameToClick, "groups-action-dropdown__trigger", 0), delayInSec: 5);
                    var ListNodes = HtmlAgilityHelper.GetListNodesFromClassName(BrowserWindow.GetPageSource(), "groups-action-dropdown__item artdeco-dropdown__item artdeco-dropdown__item--is-dropdown ember-view");
                    var GroupUnjoinId = ListNodes.FirstOrDefault(x => x.InnerHtml.Contains("Leave this group"))?.Id;
                    _automationExtension.ExecuteScript(string.Format(LDClassesConstant.ScriptConstant.ScriptGetElementByIdToClick,GroupUnjoinId), 2);
                    Thread.Sleep(3000);
                    ListNodes = HtmlAgilityHelper.GetListNodesFromClassName(BrowserWindow.GetPageSource(), "artdeco-button artdeco-button--2 artdeco-button--primary ember-view");
                    GroupUnjoinId = ListNodes.FirstOrDefault(x => x.InnerHtml.Contains("Leave"))?.Id;
                    _automationExtension.ExecuteScript(string.Format(LDClassesConstant.ScriptConstant.ScriptGetElementByIdToClick, GroupUnjoinId), 2);
                    page = BrowserWindow.GetPageSource();
                    var htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(page);
                    var Caption = Utils.RemoveHtmlTags(HtmlAgilityHelper.GetStringInnerHtmlFromClassName("",
                           "artdeco-toast-item__message", htmlDoc));
                    if (Caption == "You have left the group.")
                        return $"urn:li:fs_group:{groupId}";
                }

            }
            catch (Exception ex)
            {
                ex.DebugLog();
                response = "Error In Post Request";
            }

            return response;
        }

        public string SendPageInvitationRequest(string actionUrl, string postString)
        {
            var page = "";
            _automationExtension.LoadAndScroll(actionUrl, 10, false, 2000);
            page = BrowserWindow.GetPageSource();
            var PageId = actionUrl != null ?actionUrl.Contains("admin")?Utils.GetBetween(actionUrl,"/company/","/admin"):Utils.GetBetween(actionUrl,"/company/","/"): "";
            long.TryParse(PageId, out long pageId);
            var InviteButtonClass = LDClassesConstant.GrowConnection.InviteButtonClass;
            var MoreButtonClass = LDClassesConstant.GrowConnection.MoreOptionClass;
            var ClassToInvite = page.Contains(InviteButtonClass) ? "Invite connections to follow page" : page.Contains(MoreButtonClass) ? MoreButtonClass : "";
            var Tags = page.Contains(InviteButtonClass) ? HTMLTags.Button : HTMLTags.Div;
            var Type = ClassToInvite.Contains(MoreButtonClass) ? HTMLTags.HtmlAttribute.Class : HTMLTags.HtmlAttribute.AriaLabel;
            if (pageId==0 && (page.Contains(InviteButtonClass) || page.Contains(MoreButtonClass)))
            {
                if (ClassToInvite.Contains(MoreButtonClass))
                {
                    var X = _automationExtension.ExecuteScript(string.Format(LDClassesConstant.ScriptConstant.ScriptWithQuerySelectorAllToGetXandY, Tags, Type, ClassToInvite, 0, "x"), 5).Result;
                    var Y = _automationExtension.ExecuteScript(string.Format(LDClassesConstant.ScriptConstant.ScriptWithQuerySelectorAllToGetXandY, Tags, Type, ClassToInvite, 0, "y"), 5).Result;
                    BrowserWindow.MouseClick(Convert.ToInt32(X) + 5, Convert.ToInt32(Y) + 5, delayAfter: 3);
                    _automationExtension.ExecuteScript(string.Format(LDClassesConstant.ScriptConstant.ScriptWithQuerySelectorToClick,HTMLTags.Button,HTMLTags.HtmlAttribute.Class, "org-overflow-menu__item"), 3);
                }else if(page.Contains(MoreButtonClass))
                {
                    var IsMoreOptionClicked = _automationExtension.ExecuteScript(AttributeIdentifierType.Xpath, "//button[text()='More']");
                    if(IsMoreOptionClicked.Success)
                    {
                        page = BrowserWindow.GetPageSource();
                        if (!page.Contains("Invite connections"))
                            return "There is no option found in that Page";
                        var Nodes = HtmlAgilityHelper.GetListInnerHtmlOrInnerTextOrOuterHtmlFromIdOrClassName(page,string.Empty,true, "org-overflow-menu__item");
                        var index = Nodes != null && Nodes.Count > 0 ?Nodes.IndexOf(Nodes.FirstOrDefault(x=>x.Contains("Invite connections"))): 0;
                        var IsClickedInviteConnection= _automationExtension.ExecuteScript(string.Format(LDClassesConstant.ScriptConstant.ScriptWithQuerySelectorAllToClick, HTMLTags.Button, HTMLTags.HtmlAttribute.Class, "org-overflow-menu__item",index), 3);
                    }
                }
                else
                    _automationExtension.ExecuteScript(string.Format(LDClassesConstant.ScriptConstant.ScriptWithQuerySelectorAllToClick, Tags, Type, ClassToInvite, 0), 3);
                page = BrowserWindow.GetPageSource();
                if(!page.Contains("Invite connections"))
                {
                    return "There is no option found in that Page";
                }
                goto EnterUserName;
            }
            else
            {
                page = BrowserWindow.GetPageSource();
                if (!page.Contains("Invite connections"))
                {
                    return "There is no option found in that Page";
                }
            }
            var secondclick = _automationExtension.ExecuteScript(AttributeIdentifierType.Xpath, "//span[text()='Invite connections']");
            if (secondclick != null && !secondclick.Success)
                _automationExtension.ExecuteScript("document.querySelector('a[id=\"org-menu-INVITE_TO_FOLLOW\"]').click();", delayInSec: 20);
            EnterUserName:
            var Enternameclick = _automationExtension.GetXAndY("artdeco-typeahead__input ", AttributeIdentifierType.ClassName);
            BrowserWindow.MouseClick(Enternameclick.Key + 15, Enternameclick.Value + 15, delayAfter: 5);
            BrowserWindow.EnterChars(" "+postString, delayAtLast: 5);
            BrowserWindow.PressAnyKey(winKeyCode: 13, delayAtLast: 5);
            page = BrowserWindow.GetPageSource();
            var InvitedString = HtmlAgilityHelper.GetStringInnerTextFromClassName(page, "artdeco-button artdeco-button--2 artdeco-button--primary artdeco-button--disabled ember-view");
            if (page.Contains("invitee-picker-connections-result-item__checkbox") && page.Contains("visibility-hidden") ||!string.IsNullOrEmpty(InvitedString) && InvitedString.Contains("Invited"))
            {
                return $"Already Send Page Invitation To {postString}";
            }
            var selecteuser = _automationExtension.GetXAndY("flex-1 inline-block align-self-center pl2 mr5", AttributeIdentifierType.ClassName);
            BrowserWindow.MouseClick(selecteuser.Key + 15, selecteuser.Value + 15, delayAfter: 5);
            var success = _automationExtension
                   .ExecuteScript(AttributeIdentifierType.Xpath, $"//{HTMLTags.Span}[text()='Invite 1']").Success;
            Thread.Sleep(10000);
            var AlreadySentInvitationClass = LDClassesConstant.GrowConnection.AlreadySendPageInvitationClass;
            if (success)
            {
                _automationExtension
                   .ExecuteScript(AttributeIdentifierType.Xpath, $"//{HTMLTags.Span}[text()='No thanks']");
                var pageResponse = BrowserWindow.GetPageSource();
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(pageResponse);
                var Caption = Utils.RemoveHtmlTags(HtmlAgilityHelper.GetStringInnerHtmlFromClassName("",
                    "artdeco-toast-item__message", htmlDoc));
                if (Caption == "1 person invited to follow Page")
                    return "";
                return null;

            }
            else if(BrowserWindow.GetPageSource().Contains(AlreadySentInvitationClass))
            {
                return $"Already Send Page Invitation To {postString}";
            }
            else
            {
                return "There is no option found in that Page";
            }
        }

        public SearchLinkedinUsersResponseHandler SearchForLinkedinUsers(string actionUrl,
            string dFlagshipSearchSrpPeople)
        {
            try
            {
                _automationExtension.LoadAndScroll(actionUrl, 10, true, 2000);
                return new SearchLinkedinUsersResponseHandler(new ResponseParameter
                { Response = BrowserWindow.GetPageSource() });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        // ReSharper disable once UnusedMember.Local

        public SearchLinkedinUsersResponseHandler SalesNavigatorLinkedinUsersByKeyword(string actionUrl)
        {
            try
            {
                _automationExtension.LoadAndScroll(actionUrl, 10, true, 4000);
                return new SearchLinkedinUsersResponseHandler(
                    new ResponseParameter { Response = BrowserWindow.GetPageSource() }, true);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        //  SearchLinkedinUsersResponseHandler  for sales navigator, search url
        public SearchLinkedinUsersResponseHandler SalesNavigatorLinkedinUsersFromSearchUrl(string actionUrl,
            string sessionId)
        {
            try
            {

                _automationExtension.LoadAndScroll(actionUrl, 10, true, 4000);
                return new SearchLinkedinUsersResponseHandler(
                    new ResponseParameter { Response = BrowserWindow.GetPageSource() }, true);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public LiveChatUsermessagesResponseHandler UserLiveChatMessage(DominatorAccountModel linkedInAccount,
            SenderDetails senderdetail, ILdFunctions ldfunction)
        {
            return null;
        }


        public async Task<SearchConnectionResponseHandler> SearchForLinkedinConnectionsAsync(string actionUrl,
            CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrEmpty(actionUrl))
                {
                    var PageSource = BrowserWindow.GetPageSource();
                    if (PageSource.Contains("Show more results"))
                        _automationExtension.ExecuteScript(string.Format(LDClassesConstant.ScriptConstant.ScriptWithQuerySelectorAllToFilterNormal, ".artdeco-button__text", "Show more results",0,"click();"), 3);
                    _automationExtension.ScrollWindow(5000, isDocumentEnd: true);
                }
                else
                    _automationExtension.LoadAndScroll(actionUrl, 10, true, 5000);

                return new SearchConnectionResponseHandler(new ResponseParameter
                { Response = BrowserWindow.GetPageSource() });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public SearchConnectionResponseHandler SearchForLinkedinConnections(string actionUrl)
        {
            return SearchForLinkedinConnectionsAsync(actionUrl, _token).Result;
        }

        public SearchConnectionResponseHandler SearchForLinkedinConnections(string actionUrl,
            bool isReplyToAllMessagesChecked, string specificWords, bool isReplyToAllUsersWhodidnotReply, string userId)
        {
            try
            {
                _automationExtension.LoadPageUrlAndWait(actionUrl, 25);
                var htmlPageSource = BrowserWindow.GetPageSource();
                var ScrollCount = 0;
                //Thread.Sleep(TimeSpan.FromMinutes(5));
                var lastScroll = 0;
                var MessageNodes = new List<HtmlNode>();
            ScrollMore:
                var Nodes = HtmlParseUtility.GetListNodesFromClassName(BrowserWindow.GetPageSource(), "msg-conversation-card msg-conversations-container__pillar");
                BrowserWindow.ExecuteScript($"document.getElementsByClassName('msg-conversation-card msg-conversations-container__pillar')[{Nodes.Count-1}].scrollIntoView();", delayInSec: 3);
                BrowserWindow.ClearResources();
                foreach (var node in Nodes)
                {
                    if(!MessageNodes.Any(x=>x?.Id == node.Id))
                        MessageNodes.Add(node);
                }
                while (ScrollCount ++ <= 5)
                {
                    if (lastScroll == Nodes.Count)
                        break;
                    lastScroll = Nodes.Count;
                    goto ScrollMore;
                }
                // sometimes a pop up window is there we have to click on button having text 'Got it'
                if (htmlPageSource.Contains("Got it"))
                {
                    //http://prntscr.com/o0a1p3
                    var attribute = _automationExtension.GetPath(htmlPageSource,HTMLTags.Button, AttributeIdentifierType.Id,
                        "Got it");
                    _automationExtension.ExecuteScript(AttributeIdentifierType.Id, attribute, 15);
                }

                GlobusLogHelper.log.Info(Log.CustomMessage,
                    _linkedInAccountModel.AccountBaseModel.AccountNetwork,
                    _linkedInAccountModel.AccountBaseModel.UserName, ActivityType.AutoReplyToNewMessage,
                    "please wait while getting all messages of users");
                return new SearchConnectionResponseHandler(
                    new ResponseParameter { Response = BrowserWindow.GetPageSource() }, isReplyToAllMessagesChecked,
                    specificWords, BrowserWindow, _automationExtension, isReplyToAllUsersWhodidnotReply,MessageNodes);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public PostsResponseHandler SearchForLinkedinPosts(string actionUrl, ActivityType activityType,
            string currentAccountProfileId, string publicIdentifier, List<string> lstCommentInDom)
        {
            try
            {
                var response = new ResponseParameter { Response = BrowserWindow.GetPageSource() };
                return new PostsResponseHandler(response, activityType, currentAccountProfileId, lstCommentInDom);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }
        public PostsResponseHandler SearchPostByKeywordResponseHandler(string actionUrl, ActivityType activityType,
            string currentAccountProfileId, string publicIdentifier, List<string> lstCommentInDom)
        {
            try
            {
                //through browser we are not getting list of comments post's for some accounts
                var response = HttpHelper.GetRequest(actionUrl);
                if (string.IsNullOrEmpty(response.Response))
                    response = HttpHelper.GetRequest(actionUrl);

                return new PostsResponseHandler(response, activityType, currentAccountProfileId, lstCommentInDom);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }
        public NotificationDetailsResponseHandler GetNotificationDetails(string actionUrl)
        {
            try
            {
                _automationExtension.LoadAndScroll(actionUrl, 10, true, 6000);
                return new NotificationDetailsResponseHandler(new ResponseParameter
                { Response = BrowserWindow.GetPageSource() });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public ReceivedInvitationsResponseHandler SearchForLinkedinInvitations(string actionUrl)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(actionUrl))
                    _automationExtension.ScrollWindow();
                else
                    _automationExtension.LoadAndScroll("https://www.linkedin.com/mynetwork/invitation-manager/", 10,
                        true);
                var pageSource = BrowserWindow.GetPageSource();
                return new ReceivedInvitationsResponseHandler(new ResponseParameter { Response = pageSource });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public AllPendingConnectionRequestResponseHandler AllPendingConnectionRequest(string actionUrl)
        {
            try
            {
                CheckAndAssignBrowser();
                _automationExtension.LoadAndScroll(actionUrl, 10, true, 15000);
                return new AllPendingConnectionRequestResponseHandler(new ResponseParameter
                { Response = BrowserWindow.GetPageSource() });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public async Task<AllPendingConnectionRequestResponseHandler> AllPendingConnectionRequest(string actionUrl,
            CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrEmpty(actionUrl))
                    _automationExtension.ScrollWindow(5000);
                else
                    _automationExtension.LoadAndScroll(actionUrl, 15, true, 7000);
                return new AllPendingConnectionRequestResponseHandler(new ResponseParameter
                { Response = BrowserWindow.GetPageSource() });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public LinkedinGroupsSearchResponseHandler SearchForLinkedinGroups(string actionUrl)
        {
            _automationExtension.LoadAndScroll(actionUrl, 10, true, 4000);
            return new LinkedinGroupsSearchResponseHandler(new ResponseParameter
            { Response = BrowserWindow.GetPageSource() });
        }

        public async Task<MyGroupsResponseHandler> SearchForMyGroups(string actionUrl, bool onlyJoinedGroups,
            CancellationToken cancellationToken)
        {
            _automationExtension.LoadAndScroll(actionUrl, 15, true, 7000);
            var PageSource = BrowserWindow.GetPageSource();
            var ScrollCount = 0;
            while(!string.IsNullOrEmpty(PageSource) && PageSource.Contains("Show more results") && ScrollCount++ <= 15)
            {
                _automationExtension.ExecuteScript(string.Format(LDClassesConstant.ScriptConstant.ScriptWithQuerySelectorAllToFilterNormal, ".artdeco-button__text", "Show more results", 0, "click();"), 3);
                _automationExtension.ScrollWindow(5000, isDocumentEnd: false);
                PageSource = BrowserWindow.GetPageSource();
            }
            return new MyGroupsResponseHandler(new ResponseParameter { Response = BrowserWindow.GetPageSource() },
                onlyJoinedGroups);
        }

        public async Task<MyPageResponseHandler> SearchForMyPages(string actionUrl, bool onlyJoinedGroups,
            CancellationToken cancellationToken)
        {
            var MyPageResponse = TryAndGetResponse(actionUrl);
            return new MyPageResponseHandler(new ResponseParameter { Response = MyPageResponse });
        }

        public LinkedinGroupMemberResponseHandler GetGroupMemberInfo(string actionUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(actionUrl))
                    _automationExtension.ScrollWindow(5000);
                else
                    _automationExtension.LoadAndScroll(actionUrl, 15, true, 7000);

                return new LinkedinGroupMemberResponseHandler(new ResponseParameter
                { Response = BrowserWindow.GetPageSource() });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        /// <summary>
        ///     Updated 28 Feb 2019 handler
        ///     get details of scraped user from sales search
        /// </summary>
        /// <param name="salesNavigatorUserScraperModel"></param>
        /// <param name="profileUrl"></param>
        /// <param name="objLdFunctions"></param>
        /// <param name="linkedinUser"></param>
        /// <returns></returns>
        public SalesNavigatorDetailsResponseHandler GetSalesNavigatorProfileDetails(
            UserScraperModel salesNavigatorUserScraperModel, string profileUrl,
            LinkedinUser linkedinUser)
        {
            try
            {
                _automationExtension.LoadAndScroll(profileUrl, 10, true, 5000,
                    breakIfContains: "LinkedIn Corporation ©");
                // scrolling and clicking show more
                var response = BrowserWindow.GetPageSource();
                var fullName = Utils.RemoveHtmlTags(HtmlAgilityHelper.GetStringInnerHtmlFromClassName(response,
                    "_name_1sdjqx _small_1sdjqx _bodyText_1e5nen _default_1i6ulk _weightBold_1e5nen")).Trim();
                if (string.IsNullOrEmpty(fullName))
                    fullName = linkedinUser.FullName;
                if (fullName.Contains("LinkedIn Member"))
                    return null;
                var showAllNodes = HtmlAgilityHelper.GetListNodesFromAttibute(response,HTMLTags.Button,
                    AttributeIdentifierType.ClassName, null, "Show all");
                foreach (var showAllNode in showAllNodes)
                {
                    var className = Utils.GetBetween(showAllNode.OuterHtml, "class=\"", "\"");
                    // we skipping clicking on contact button else it will open a popup window for it
                    if (className.Contains("contact"))
                        continue;
                    _automationExtension.ExecuteScript(string.Format(LDClassesConstant.ScriptConstant.ScriptGetElementsByClassNameToClick,className,0),4);
                    _automationExtension.ScrollWindow(1000);
                }
                showAllNodes = HtmlAgilityHelper.GetListNodesFromAttibute(response, HTMLTags.Button,
                    AttributeIdentifierType.ClassName, null, "ember-view _button_ps32ck _small_ps32ck _tertiary_ps32ck _center_ps32ck _right_ps32ck _container_iq15dg show-all-button _bodyText");
                foreach (var showAllNode in showAllNodes)
                {
                    var className = Utils.GetBetween(showAllNode.OuterHtml, "class=\"", "\"");
                    _automationExtension.ExecuteScript(string.Format(LDClassesConstant.ScriptConstant.ScriptGetElementsByClassNameToClick, className,showAllNodes.IndexOf(showAllNode)), 4);
                    _automationExtension.ScrollWindow(1000);
                }
                return new SalesNavigatorDetailsResponseHandler(salesNavigatorUserScraperModel,
                    new ResponseParameter { Response = BrowserWindow.GetPageSource() }, this, linkedinUser);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }


        public SalesNavigatorDetailsResponseHandler GetSalesNavigatorCompanyDetails(string companyDetailsactionUrl,
            string companyUrl)
        {
            try
            {
                _automationExtension.LoadAndScroll(companyUrl, 15, true, 2000);
                return new SalesNavigatorDetailsResponseHandler(
                    new ResponseParameter { Response = ldFunctions.GetInnerLdHttpHelper().GetRequest(LdConstants.GetSalesCompanyDetailsAPI(string.IsNullOrEmpty(companyUrl) ? string.Empty : companyUrl.Split('/').LastOrDefault(x => x != string.Empty))).Response }, companyUrl);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public SalesNavigatorProfileDetails GetSalesNavigatorProfileDetails(string profileUrl)
        {
            return null;
        }

        public PageSearchResponseHandler LinkedinPageResponseByKeyword(string actionurl)
        {
            try
            {
                _automationExtension.LoadAndScroll(actionurl, 15, true, 2000);
                return new PageSearchResponseHandler(new ResponseParameter { Response = BrowserWindow.GetPageSource() });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public string GetHtmlFromUrlForMobileRequest(string actionUrl, string flagship3ProfileViewBase)
        {
            // it will scroll down if flagship3ProfileViewBase is not NullOrEmpty and till it finds element
            var isScroll = !string.IsNullOrEmpty(flagship3ProfileViewBase) && !flagship3ProfileViewBase.Contains("=");
            _automationExtension.LoadAndScroll(actionUrl, 10, isScroll, breakIfContains: flagship3ProfileViewBase);
            return BrowserWindow.GetPageSource();
        }

        public string GetHtmlFromUrlNormalMobileRequest(string actionUrl)
        {
            string response;
            try
            {
                _automationExtension.LoadPageUrlAndWait(actionUrl);
                response = BrowserWindow.GetPageSource();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }

            return response;
        }

        public string GetVideoUploadResponseStatus(string actionUrl)
        {
            return null;
        }


        public JobSearchResponseHandler JobSearch(string actionUrl)
        {
            _automationExtension.LoadAndScroll(actionUrl, 15);
            for (var index = 0; index < 3; index++)
                _automationExtension.ExecuteScript(
                    "document.getElementsByClassName('jobs-search-results jobs-search-results--is-two-pane')[0].scrollTop += 1000;");
            return new JobSearchResponseHandler(new ResponseParameter { Response = BrowserWindow.GetPageSource() });
        }

        /// <summary>
        ///     csrf token is changed after taking cookies from embedded browser for sales navigator
        /// </summary>
        /// <param name="actionUrl"></param>                                                                                                                                                          
        /// <param name="isSalesNavigator"></param>
        /// <param name="isUpdateCsrfToken"></param>
        /// <returns></returns>
        public CompanySearchResponseHandler CompanySearch(string actionUrl, bool isSalesNavigator,
            bool isUpdateCsrfToken = false)
        {
            try
            {
                _automationExtension.LoadAndScroll(actionUrl, 10, true, 4000);
                var response = BrowserWindow.GetPageSource();
                var CompanyScrapperResponseFailedCount = 0;
                if (string.IsNullOrEmpty(response)?true:response.Contains("<!DOCTYPE html>"))
                {
                    var CompanySearchAPI = string.Empty;
                    var searchId = string.Empty;
                    if (isSalesNavigator)
                    {
                        searchId = Utils.GetBetween(actionUrl, "savedSearchId=", "&sessionId=");
                        var sessionId = Utils.GetBetween(actionUrl, "&sessionId=", "&page=");
                        sessionId= string.IsNullOrEmpty(sessionId)?Utils.GetBetween(actionUrl + "**", "sessionId=", "**"):sessionId;
                        var paginationCount = Utils.GetBetween(actionUrl + "**", "page=", "**");
                        CompanySearchAPI = LdConstants.GetSalesCompanyScrapperAPI(searchId, sessionId,string.IsNullOrEmpty(paginationCount)?0:Convert.ToInt32(paginationCount));
                        ldFunctions.SetWenRequestparamtersForsalesUrl("", isSalesNavigator);
                    }
                    else
                    {
                        var Keyword = Utils.GetBetween(actionUrl, "&keywords=", "&origin=");
                        Keyword = string.IsNullOrEmpty(Keyword) ? Utils.GetBetween(actionUrl, "?keywords=", "&origin=") : Keyword;
                        searchId = Utils.GetBetween(actionUrl, "&searchId=", "&sid=");
                        searchId = string.IsNullOrEmpty(searchId) ?Utils.GetBetween(actionUrl,"&sid=","&"): searchId;
                        var queryParameter = Utils.GetBetween(actionUrl, "heroEntityKey=", "&keywords=");
                        var origin = Utils.GetBetween(actionUrl, "&origin=", "&");
                        CompanySearchAPI = LdConstants.GetCompanyScrapperAPI(Keyword,queryParameter,searchId,origin,0);
                    }
                    response = HttpHelper.GetRequest(CompanySearchAPI).Response;
                    while (CompanyScrapperResponseFailedCount++ < 3 && string.IsNullOrEmpty(response))
                        response = HttpHelper.GetRequest(CompanySearchAPI).Response;
                }
                return isSalesNavigator
                    ? new CompanySearchResponseHandler(new ResponseParameter { Response = response }, true)
                    : new CompanySearchResponseHandler(new ResponseParameter { Response = response });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public void SetRequestParameterForUploadVideo(string referer, string dFlagship3SearchSrpPeople)
        {

        }
        /// <summary>
        ///     dominatorAccountModel only for salesNav
        /// </summary>
        /// <param name="dominatorAccountModel"></param>
        /// <returns></returns>
        // ReSharper disable once IdentifierTypo
        private string GetCsrfToken(DominatorAccountModel dominatorAccountModel = null)
        {
            var csrfCookieToken = string.Empty;

            try
            {
                //  we get changed csrf token in sales navigator
                if (dominatorAccountModel == null)
                    dominatorAccountModel = _linkedInAccountModel;
                try
                {
                    csrfCookieToken = HttpHelper.GetRequestParameter().Cookies["JSESSIONID"]?.Value.Replace("\"", "");
                }
                catch (Exception)
                {
                    // ignored
                }

                if (string.IsNullOrEmpty(csrfCookieToken))
                    csrfCookieToken = dominatorAccountModel.Cookies["JSESSIONID"]?.Value.Replace("\"", "");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return csrfCookieToken;
        }

        /// <summary>
        ///     sometimes not getting proper response because of useragent
        ///     removing useragent and assigning null
        /// </summary>
        /// <param name="url"></param>
        /// <param name="isDecode"></param>
        public string GetRequestUpdatedUserAgent(string url, bool isDecode = false)
        {
            _automationExtension.LoadAndScroll(url, 10, true, 1800);
            return BrowserWindow.GetPageSource();
        }

        public string GetSecondaryBrowserResponse(string actionUrl, int scollDown = 0)
        {
            if (SecondaryBrowser == null)
            {
                var Account = _linkedInAccountModel;
                sessionManager.AddOrUpdateSession(ref Account);
                _linkedInAccountModel = Account;
                LDAccountsBrowserDetails.GetInstance().StartBrowserLogin(_linkedInAccountModel, _token, true,
                    BrowserInstanceType.Secondary,sessionManager);
                Thread.Sleep(25000);
                var name = LDAccountsBrowserDetails.GetBrowserName(_linkedInAccountModel,
                    BrowserInstanceType.Secondary);
                SecondaryBrowser = LDAccountsBrowserDetails.GetInstance().AccountBrowserCollections[name];
            }

            var automationExtension = new BrowserAutomationExtension(SecondaryBrowser);
            if (string.IsNullOrEmpty(actionUrl) && scollDown != 0)
                automationExtension.ScrollWindow(scollDown);
            else
                automationExtension.LoadAndScroll(actionUrl, 15, scollDown != 0, scollDown);
            return SecondaryBrowser.GetPageSource();
        }

        private void CheckAndAssignBrowser()
        {
            if (BrowserWindow == null)
            {
                var collection = LDAccountsBrowserDetails.GetInstance().AccountBrowserCollections;
                BrowserWindow = collection[_linkedInAccountModel.UserName];
            }
        }
        public void GoBrowserBack()
        {
            Application.Current.Dispatcher.Invoke(() => { BrowserWindow.GoBack(); });
            Thread.Sleep(15000);
        }

        public IResponseParameter BlockUserResponse(string blockActionUrl)
        {
            IResponseParameter responseParameter = null;
            try
            {
                _automationExtension.LoadAndScroll(blockActionUrl, 10);

                var pageSource = BrowserWindow.GetPageSource();
                if (pageSource.Contains("This profile is not available"))
                    return responseParameter = new ResponseParameter(){ Response = "This profile is not available", HasError = true };
                var axis = _automationExtension.GetXAndY("pv-s-profile-actions__overflow ember-view",
                         AttributeIdentifierType.ClassName);
                if(axis.Key > 0 && axis.Value > 0)
                    BrowserWindow.MouseClick(axis.Key + 10, axis.Value + 10, delayAfter: 5);
                if (_automationExtension.ExecuteScript(AttributeIdentifierType.Xpath, $"//{HTMLTags.Span}[text()='Report / Block']").Success)
                {
                    _automationExtension.ExecuteScript(string.Format(LDClassesConstant.ScriptConstant.ScriptGetElementsByClassNameToClick, "trust-action-card-compact-plain__text text-body-small-bold", 0), 5);
                    if (_automationExtension.ExecuteScript(AttributeIdentifierType.Xpath, $"//{HTMLTags.Span}[text()='Block']").Success)
                    {
                        Thread.Sleep(10000);
                        responseParameter = new ResponseParameter() { Response = "responseCode\":200" };
                    }
                }
            }
            catch (Exception exception)
            {
                exception.DebugLog();
            }

            return responseParameter;
        }

        public IResponseParameter DeleteUserMessagesResponse(string actionUrl)
        {
            IResponseParameter responseParameter = null;
            try
            {
                var messageButton = "msg-thread-actions__control artdeco-button artdeco-button--circle artdeco-button--tertiary artdeco-button--2 artdeco-button--muted artdeco-dropdown__trigger artdeco-dropdown__trigger--placement-bottom ember-view";
                _automationExtension.ScrollPostWindow(3000, false, messageButton);
                var axis = _automationExtension.GetXAndY(
                    messageButton,
                    AttributeIdentifierType.ClassName);
                if(axis.Key == 0 && axis.Value == 0)
                {
                    var script = "document.querySelector('div[class=\"shared-title-bar__title msg-title-bar__title-bar-title\"]>div>button').getBoundingClientRect().{0}";
                    var X = _automationExtension.ExecuteScript(string.Format(script, "x"), delayInSec: 3);
                    var Y = _automationExtension.ExecuteScript(string.Format(script, "y"), delayInSec: 3);
                    decimal.TryParse(X.Result.ToString(), out decimal decimalIntX);
                    decimal.TryParse(Y.Result.ToString(), out decimal decimalIntY);
                    var roundedX = Math.Round(decimalIntX);
                    var roundedY = Math.Round(decimalIntY);
                    axis = new KeyValuePair<int, int>((int)roundedX,(int)roundedY);
                }
                BrowserWindow.MouseClick(axis.Key + 15, axis.Value + 15, delayAfter: 5);
                var totalHeight = 425;
                var neededHeight = totalHeight - axis.Value;
                BrowserWindow.MouseClick(axis.Key - 50, axis.Value + neededHeight, delayAfter: 5);
                var pageSource = BrowserWindow.GetPageSource();
                if (pageSource.Contains("Let us know why you're reporting this conversation"))
                {
                    _automationExtension.ExecuteScript("document.querySelector('button[aria-label=\"Dismiss\"]').click();", delayInSec: 4);
                    axis = _automationExtension.GetXAndY(
                    messageButton,
                    AttributeIdentifierType.ClassName);
                    if (axis.Key == 0 && axis.Value == 0)
                    {
                        var script = "document.querySelector('div[class=\"shared-title-bar__title msg-title-bar__title-bar-title\"]>div>button').getBoundingClientRect().{0}";
                        var X = _automationExtension.ExecuteScript(string.Format(script, "x"), delayInSec: 3);
                        var Y = _automationExtension.ExecuteScript(string.Format(script, "y"), delayInSec: 3);
                        decimal.TryParse(X.Result.ToString(), out decimal decimalIntX);
                        decimal.TryParse(Y.Result.ToString(), out decimal decimalIntY);
                        var roundedX = Math.Round(decimalIntX);
                        var roundedY = Math.Round(decimalIntY);
                        axis = new KeyValuePair<int, int>((int)roundedX, (int)roundedY);
                    }
                    neededHeight = totalHeight - axis.Value;
                    BrowserWindow.MouseClick(axis.Key + 15, axis.Value + 15, delayAfter: 5);
                    BrowserWindow.MouseClick(axis.Key - 50, axis.Value + neededHeight+20, delayAfter: 5);
                }
                if (pageSource.Contains("Why am I seeing this ad?"))
                {
                    _automationExtension.ExecuteScript("document.querySelector('button[aria-label=\"Dismiss\"]').click();", delayInSec: 4);
                    axis = _automationExtension.GetXAndY(
                    messageButton,
                    AttributeIdentifierType.ClassName);
                    neededHeight = totalHeight - axis.Value;
                    BrowserWindow.MouseClick(axis.Key + 15, axis.Value + 15, delayAfter: 5);
                    BrowserWindow.MouseClick(axis.Key - 50, axis.Value + neededHeight - 20, delayAfter: 5);
                }
                if (_automationExtension.LoadAndClick(AttributeTypes.Button, AttributeIdentifierType.Id,
                    "Delete"))
                    responseParameter = new ResponseParameter { Response = "" };
            }
            catch (Exception exception)
            {
                exception.DebugLog();
            }

            return responseParameter;
        }

        #endregion


        #region Requests Implemented with Web Device

        public async Task<string> WebLogin()
        {
            return "";
        }

        public async Task<string> RedirectChallenge(DominatorAccountModel dominatorAccount, LoginResponseHandler loginResponseHandler)
        {
            string response;
            try
            {
                // var preLoginPageSource = string.Empty;
                var reqParams = HttpHelper.GetRequestParameter();
                reqParams.Headers.Add("X-Requested-With", "com.linkedin.android");
                var checkPointPostUrl = loginResponseHandler.ChallengeUrl;
                var checkPointPostData =
                    $"session_key={Uri.UnescapeDataString(_linkedInAccountModel.UserName)}&session_password={_linkedInAccountModel.AccountBaseModel.Password}&session_redirect=https%3A%2F%2Fwww.linkedin.com%2Fnhome";

                var checkPointResponse = await HttpHelper.PostRequestAsync(checkPointPostUrl, checkPointPostData,dominatorAccount.Token);
                response = checkPointResponse.Response;
                // string redirectChallengeUrl = HttpHelper.Request.Address.ToString();


                if (checkPointResponse.Response.Contains("d_checkpoint_rp_forceResetEmailSent"))
                    GlobusLogHelper.log.Info(Log.LoginFailed, _linkedInAccountModel.AccountBaseModel.AccountNetwork,
                        _linkedInAccountModel.AccountBaseModel.UserName, "Found Reset password link.");

                // send reset password link to this email
                // var sendResetResponse = SendResetPasswordLinkToMail(checkPointResponse);

                else if (checkPointResponse.Response.Contains("captcha"))
                    GlobusLogHelper.log.Info(Log.LoginFailed, _linkedInAccountModel.AccountBaseModel.AccountNetwork,
                        _linkedInAccountModel.AccountBaseModel.UserName, "Found Captcha.");
                else if (checkPointResponse.Response.Contains("form__input--text input_verification_pin"))
                    GlobusLogHelper.log.Info(Log.LoginFailed, _linkedInAccountModel.AccountBaseModel.AccountNetwork,
                        _linkedInAccountModel.AccountBaseModel.UserName, "Found Captcha.");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                response = null;
            }

            return response;
        }

        /// <summary>
        ///     sending reset password link if we get reset password
        /// </summary>
        /// <param name="loginResponseParameter"></param>
        /// <returns></returns>
        public IResponseParameter SendResetPasswordLinkToMail(IResponseParameter loginResponseParameter)
        {
            IResponseParameter responseParameter = null;
            var response = loginResponseParameter.Response;
            try
            {
                // var fullUrl = $"https://www.linkedin.com/checkpoint/{checkPointUrl}";
                var passwordResetMemberToken = Utils.GetBetween(response, "passwordResetMemberToken=", "\"");
                var dCheckpointRpForceResetEmailSent =
                    Utils.GetBetween(response, "d_checkpoint_rp_forceResetEmailSent;", "\"");
                var pwdReset = Utils.GetBetween(response, "Pwd-Reset:", "\"");
                var requestSubmitPasswordUrl =
                    $"https://www.linkedin.com/checkpoint/rp/request-password-reset-submit?passwordResetMemberToken={passwordResetMemberToken}";
                var randomWebKitFormBoundaryString =
                    RandomUtilties.GetRandomString(16); //Utils.generateTrackingId();//"gPuBxdbLPOY0rRTY";
                var jsessionId = HttpHelper.GetRequestParameter().Cookies["JSESSIONID"]?.Value.Replace("\"", "");
                var absoluteUri = HttpHelper.Request.Address.AbsoluteUri;
                var reqParams = HttpHelper.GetRequestParameter();
                reqParams.Headers.Clear();
                reqParams.UserAgent =
                    "LIAuthLibrary:0.0.3 com.linkedin.android:4.1.256 LAVA_V23GB:android_6.0";
                reqParams.Headers.Add("Origin", "https://www.linkedin.com");
                reqParams.Referer = absoluteUri;
                reqParams.ContentType =
                    $"multipart/form-data; boundary=----WebKitFormBoundary{randomWebKitFormBoundaryString}";
                reqParams.Accept = "*/*";
                reqParams.Headers.Add("X-Requested-With", "com.linkedin.android");
                //   reqParams.Headers.Add("Accept-Encoding", "gzip, deflate");
                reqParams.Headers.Add("Accept-Language", "en-IN,en-US;q=0.8");


                var objLdRequestParameters = new LdRequestParameters();

                var postDataParams = new Dictionary<string, string>
                {
                    {"csrfToken", jsessionId},
                    {
                        "pageInstance",
                        $"urn:li:page:d_checkpoint_rp_forceResetEmailSent;{dCheckpointRpForceResetEmailSent}"
                    },
                    {"sid", $"Pwd-Reset:{pwdReset}"},
                    {"userName", _linkedInAccountModel.UserName},
                    {"resetOption", "resendEmail"}
                };

                objLdRequestParameters.PostDataParameters = postDataParams;
                var contentType = "";
                var postData =
                    objLdRequestParameters.CreateMultipartBody(objLdRequestParameters.PostDataParameters,
                        out contentType);
                reqParams.ContentType = contentType;
                responseParameter = HttpHelper.PostRequestAsync(requestSubmitPasswordUrl, postData, _token).Result;

                if (string.IsNullOrEmpty(responseParameter.Response))
                    GlobusLogHelper.log.Info(Log.AccountLogin, _linkedInAccountModel.AccountBaseModel.AccountNetwork,
                        $"successfully sent reset password to this email {_linkedInAccountModel.AccountBaseModel.UserName}");
                else
                    GlobusLogHelper.log.Info(Log.AccountLogin, _linkedInAccountModel.AccountBaseModel.AccountNetwork,
                        $"failed to sent reset password this email {_linkedInAccountModel.AccountBaseModel.UserName}");
                ;

                //https://www.linkedin.com/checkpoint/rp/password-reset?requestSubmissionId=AgHydV328jKVVAAAAWfGAT0IcvXpPjwecQI7wfgyPPYp6dw4p2HpqjWZqhnhI02io82jsYuLYFVxDWdW5ZzYkhlawXipZj0L4EYecoaXR-w&lipi=urn%3Ali%3Apage%3Aemail_security_password_reset_checkpoint%3BNYDZ9i1hTTOijDR8PXMoNA%3D%3D&userName=AgFzbm2S9JQkigAAAWfGATz_o4QEJzle0IWHGuKxsk30MrJIKhtAST7jozC9WMnoZ7mzWrhjfgQBQ5-G_A&oneTimeToken=9034971400828793797&trk=eml-jav-saved-job&midToken=AQHQMo8dwqsP4A&fromEmail=fromEmail&ut=2uw5JZvbwg-8w1
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return responseParameter;
        }

        public void ReadVerificationCodeFromEmail(DominatorAccountModel accountModel)
        {
            try
            {
                GlobusLogHelper.log.Info(Log.CustomMessage, accountModel.AccountBaseModel.AccountNetwork,
                    accountModel.AccountBaseModel.UserName, "Auto Email Verification", "Getting code from email");
                // Thread.Sleep(TimeSpan.FromMinutes(2));
                //pop.mail.ru
                //pop.mail.yahoo.com
                var model =
                    new MailCredentials
                    {
                        Hostname = accountModel.MailCredentials.Hostname,
                        Port = accountModel.MailCredentials.Port,
                        Username = accountModel.MailCredentials.Username,
                        Password = accountModel.MailCredentials.Password
                    };

                var messageData = EmailClient.FetchLastMailFromSender(model, true, "security-noreply@linkedin.com");
                accountModel.VarificationCode = Utilities.GetBetween(messageData.Message,
                    "this verification code to complete your sign in:", "\n\r").Trim();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public async Task<string> PreLoginResponseWebRequest()
        {
            try
            {
                const string linkedinLink = "https://www.linkedin.com";
                var preLoginResponseWebRequest = await HttpHelper.GetRequestAsync(linkedinLink, _token);
                return HttpUtility.HtmlDecode(preLoginResponseWebRequest.Response);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }


        public GroupJoinerResponseHandler CheckGroupMemberShip(string actionUrl)
        {
            try
            {
                _automationExtension.LoadPageUrlAndWait(actionUrl);
                return new GroupJoinerResponseHandler(
                    new ResponseParameter { Response = BrowserWindow.GetPageSource() });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public IHttpHelper GetInnerHttpHelper()
        {
            return HttpHelper;
        }

        public ILdHttpHelper GetInnerLdHttpHelper()
        {
            return HttpHelper;
        }

        public string Like(string actionUrl, string postStringOrNodeId)
        {
            _automationExtension.LoadPageUrlAndWait(actionUrl);
            var PostPageSource = BrowserWindow.GetPageSource();
            if (PostPageSource.Contains("react-button__text--like"))
            {
                return LdConstants.AlreadyLikedFeed;
            }
            if (_automationExtension.ExecuteScript(string.Format(LDClassesConstant.ScriptConstant.ScriptGetElementsByClassNameToClick,LDClassesConstant.Engage.PostLikeClass,0), 2).Success)
                Thread.Sleep(5000);
            return string.Empty;
        }

        public string Comment(string actionUrlOrComment, string postStringOrNodeId, string url, string queryType)
        {
            _automationExtension.LoadPageUrlAndWait(url);
            var commentTextAreaClass = LDClassesConstant.Engage.CommentTextAreaClass;
            _automationExtension.ExecuteScript($"document.getElementsByClassName('{commentTextAreaClass}')[0].scrollIntoViewIfNeeded();", 2);
            var commentDisabled = string.Empty;
            var pageSource = BrowserWindow.GetPageSource();
            var Comments = Regex.Split(actionUrlOrComment, "\r\n");
            commentDisabled = CheckCommentDisabled(pageSource);
            if (commentDisabled.Contains("group"))
                return commentDisabled;
            if (queryType == "Custom Posts List")
            {
                var axis = _automationExtension.GetXAndY(commentTextAreaClass,
                    AttributeIdentifierType.ClassName);
                BrowserWindow.MouseClick(axis.Key + 15, axis.Value + 15, delayAfter: 5);
                Comments.ForEach(comment =>
                {
                    BrowserWindow.EnterChars(" "+comment, delayAtLast: 2);
                    if (comment != Comments.LastOrDefault())
                        BrowserWindow.PressCombinedKey(16, 13);
                    Thread.Sleep(3000);
                });
                pageSource = BrowserWindow.GetPageSource();
                var className = _automationExtension.GetPath(pageSource,HTMLTags.Button,
                    AttributeIdentifierType.ClassName,LDClassesConstant.Engage.SubmitCommentClass);
                if (_automationExtension
                    .ExecuteScript(string.Format(LDClassesConstant.ScriptConstant.ScriptGetElementsByClassNameToClick,className,0)).Success)
                    return "Successfully";
            }
            else
            {
                _automationExtension
                    .ExecuteScript($"document.getElementsByClassName('social-details-social-activity update-v2-social-activity')[0].scrollIntoView()");
                var axis = _automationExtension.GetXAndY(commentTextAreaClass,
                    AttributeIdentifierType.ClassName);
                BrowserWindow.MouseClick(axis.Key + 15, axis.Value + 15, delayAfter: 5);
                Comments.ForEach(comment =>
                {
                    BrowserWindow.EnterChars(" "+comment, delayAtLast: 2);
                    if (comment != Comments.LastOrDefault())
                        BrowserWindow.PressCombinedKey(16, 13);
                    Thread.Sleep(3000);
                });
                pageSource = BrowserWindow.GetPageSource();
                var nodeId = _automationExtension.GetPath(pageSource,HTMLTags.Button, AttributeIdentifierType.Id,
                    LDClassesConstant.Engage.SubmitCommentClass);
                if (_automationExtension.ExecuteScript(string.Format(LDClassesConstant.ScriptConstant.ScriptGetElementByIdToClick,nodeId)).Success)
                    return "Successfully";
                if (_automationExtension.ExecuteScript(AttributeIdentifierType.Xpath, $"//{HTMLTags.Span}[text()='Post']")
                    .Success)
                    return "Successfully";
            }

            return null;
        }
        private string CheckCommentDisabled(string PageSource)
        {
            var result = string.Empty;
            if (PageSource.Contains("Join the group to comment on this post. You can still react or share it.") || PageSource.Contains("Only group members can comment on this post. You can still react or repost it"))
            {
                var nodes = HtmlAgilityHelper.GetListInnerHtmlOrInnerTextOrOuterHtmlFromIdOrClassName(PageSource, "", false, "app-aware-link  profile-rail-card__profile-link t-16 t-black t-bold tap-target",true);
                result = Regex.Replace(Utils.GetBetween(nodes.Last(), "href=\"", "\""),"\\?q=(.*)","");
                return result;
            }
            else
                return result;
        }
        public string Share(string actionUrl, string postStringOrNodeId, string composeIconId)
        {
            // here our sharing is done in 3 steps 
            //1. click on share button/div by id
            //2. click on pop up 'share in post'
            //3. click on post button
            _automationExtension.LoadPageUrlAndWait(actionUrl, 3);
            var ShareScript = $"document.getElementsByClassName('{LDClassesConstant.Engage.PostShareClass}')[0]";
            BrowserWindow.ExecuteScript($"{ShareScript}.scrollIntoViewIfNeeded();");
            if (_automationExtension.ExecuteScript($"{ShareScript}.click();", 5)
                .Success)
            {
                var shared = _automationExtension.ExecuteScript(string.Format(LDClassesConstant.ScriptConstant.ScriptGetElementsByClassNameToClick,LDClassesConstant.Engage.RepostPostClass,1), 3).Success;
                Thread.Sleep(2000);
                if (shared)
                {
                    var SharedSuccessPageSource = BrowserWindow.GetPageSource();
                    var SuccessMessage = Utils.GetBetween(SharedSuccessPageSource, "role=\"alert\">", "</span>");
                    return SuccessMessage;
                }
                var scriptPost = _automationExtension.ScriptContructor(AttributeIdentifierType.Xpath,
                    $"//artdeco-dropdown-item[@id='{composeIconId}']");
                if (_automationExtension.ExecuteScript(AttributeIdentifierType.Xpath, $"//{HTMLTags.Span}[text()='Post']").Success)
                    return "urn:li:share:";

                if (_automationExtension.ExecuteScript(scriptPost, 5).Success
                    || _automationExtension.LoadAndClick(HTMLTags.Button, AttributeIdentifierType.Id,
                        "share-box__actions share-actions mlA ember-view"))
                    return "urn:li:share:";
            }


            return null;
        }

        public string SendGreeting(string actionUrlOrId, string postDataOrMessage)
        {
            _automationExtension.LoadAndScroll(actionUrlOrId, 10);
            //here we closing all message windows
            while (_automationExtension.LoadAndClick(HTMLTags.Button, AttributeIdentifierType.Id,
                LDClassesConstant.Messenger.CloseConversationWindow)) ;

            var page = BrowserWindow.GetPageSource();
            var getXandy =
                _automationExtension.GetXAndY(LDClassesConstant.Engage.CommentTextAreaClass, AttributeIdentifierType.ClassName);
            BrowserWindow.MouseClick(getXandy.Key + 5, getXandy.Value + 5, delayAfter: 5);
            BrowserWindow.PressAnyKey(2, winKeyCode: 8);
            BrowserWindow.EnterChars(" " + postDataOrMessage, 0.2);

            if (_automationExtension.ExecuteScript(string.Format(LDClassesConstant.ScriptConstant.ScriptGetElementsByClassNameToClick, "comments-comment-box__submit-button artdeco-button artdeco-button--1 mt3", 0))
                .Success)
                return "{\"value\":{\"createdAt\":";


            return "";
        }

        public void SetJobCancellationTokenInBrowser(DominatorAccountModel dominatorAccountModel,
            CancellationToken cancellationToken)
        {
            _automationExtension = new BrowserAutomationExtension(BrowserWindow, cancellationToken);
        }

        public void BrowserMailverfication(string pin)
        {
            BrowserWindow.EnterChars(pin, delayAtLast: 2);
            var success = _automationExtension.ExecuteScript(AttributeIdentifierType.ClassName,
                         "form__submit form__submit--stretch").Result;
            Thread.Sleep(15000);
        }

        private CancellationToken _token;

        public bool BrowserLogin(DominatorAccountModel account, CancellationToken cancellationToken,
            LoginType loginType = LoginType.AutomationLogin, VerificationType verificationType = 0)
        {
            _token = cancellationToken;
            sessionManager.AddOrUpdateSession(ref account);
            // here BrowserWindow will be on null browserLogin
            LDAccountsBrowserDetails.GetInstance()
                .StartBrowserLogin(account, _token, false, BrowserInstanceType.BrowserLogin,sessionManager);
            return true;
        }

        public string GroupJoinRequest(string groupid)
        {
            return "";
        }

        public string TryAndGetResponse(string Url, string NotContainString = "", int TryCount = 2)
        {
            var Response = string.Empty;
            try
            {
                if (Url != null && _ldDataHelper.IsSalesProfile(Url))
                    ldFunctions.SetWenRequestparamtersForsalesUrl(Url, true);
                Response = IsBrowser ? GetInnerHttpHelper().GetRequest(Url).Response : GetHtmlFromUrlNormalMobileRequest(Url);
                while (TryCount-- >= 0 && (string.IsNullOrEmpty(Response) || (string.IsNullOrEmpty(NotContainString) ? string.IsNullOrEmpty(Response) : !Response.Contains(NotContainString))))
                {
                    Thread.Sleep(4000);
                    Response = IsBrowser ? GetInnerHttpHelper().GetRequest(Url).Response : GetHtmlFromUrlNormalMobileRequest(Url);
                }
            }
            catch (Exception ex) { ex.DebugLog(); }
            return Response;
        }

        public async Task<ScrapePostResponseHandler> ScrapeKeywordPost(DominatorAccountModel dominatorAccount, string QueryValue, int PaginationCount = 0)
        {
            return new ScrapePostResponseHandler(new ResponseParameter());
        }

        public string CommentWithAlternateMethod(IDetailsFetcher detailsFetcher, LinkedinPost objLinkedinPost, UserScraperDetailedInfo userScraperDetailedInfo, string comment, string querytype)
        {
            return Comment(comment,string.Empty,objLinkedinPost.InvitationId,querytype);
        }

        public string CommentOnCustomPost(string comment, string PostUrl)
        {
            return Comment(comment, string.Empty, PostUrl, "Custom Posts List");
        }

        public string BroadCastMessage(string ImageSource, LinkedinUser linkedinUser, bool IsGroup, string FinalMessage,string OriginToken,List<string> Medias=null)
        {
            throw new NotImplementedException();
        }
        public string GetMediaIDs(List<string> imageSource)
        {
            return string.Empty;
        }

        public async Task<EventInviteResponseHandler> SendEventInvitation(DominatorAccountModel dominatorAccount, string ActionUrl, string PostString)
        {
            IResponseParameter response = new ResponseParameter();
            var pageResponse = string.Empty;
            try
            {
                var currentPage = BrowserWindow.CurrentUrl();
                if(!string.IsNullOrEmpty(currentPage) &&!currentPage.Contains(ActionUrl))
                    await BrowserWindow.GoToCustomUrl(ActionUrl,delayAfter:10);
                pageResponse = await BrowserWindow.GetPageSourceAsync();
                if(!string.IsNullOrEmpty(pageResponse) && pageResponse.Contains("Sorry, something went wrong!"))
                {
                    pageResponse = "Can't Invite";
                    goto Exit;
                }
                if(!string.IsNullOrEmpty(pageResponse) && !pageResponse.Contains("id=\"invitee-picker__modal\""))
                {
                    await BrowserWindow.ExecuteScriptAsync("[...document.querySelectorAll('div[class=\"events-live-top-card__cta\"] > div>button')].filter(x=>x.innerText === \"Share\")[0].scrollIntoViewIfNeeded();", delayInSec: 3);
                    await BrowserWindow.ExecuteScriptAsync("[...document.querySelectorAll('div[class=\"events-live-top-card__cta\"] > div>button')].filter(x=>x.innerText === \"Share\")[0].click();", delayInSec: 3);
                    var Nodes1 = HtmlParseUtility.GetListNodeFromPartialTagNamecontains(await BrowserWindow.GetPageSourceAsync(), "li", "role", "menuitem");
                    if (Nodes1 != null && Nodes1.Count > 0 && !Nodes1.Any(x => !string.IsNullOrEmpty(x?.InnerText) && x.InnerText.Contains("Invite")))
                    {
                        pageResponse = "Can't Invite";
                        goto Exit;
                    }
                    var clickIndex1 = Nodes1 != null && Nodes1.Count > 0 ? Nodes1.IndexOf(Nodes1.FirstOrDefault(x => !string.IsNullOrEmpty(x?.InnerText) && x.InnerText.Contains("Invite"))) : 0;
                    await BrowserWindow.ExecuteScriptAsync($"document.querySelectorAll('li[role=\"menuitem\"]')[{clickIndex1}].click();", delayInSec: 5);
                }
                var xy = await BrowserWindow.GetXAndYAsync(AttributeType.ClassName, "artdeco-typeahead__input");
                await BrowserWindow.MouseClickAsync(xy.Key + 10, xy.Value + 5, delayAfter: 3);
                BrowserWindow.SelectAllText();
                await BrowserWindow.PressAnyKeyUpdated(8, delayAtLast: 2);
                await BrowserWindow.EnterCharsAsync(" "+PostString, delayAtLast: 5);
                pageResponse = await BrowserWindow.GetPageSourceAsync();
                var Nodes = HtmlParseUtility.GetListNodeFromPartialTagNamecontains(pageResponse, "li", "role", "option");
                var data = Nodes.FirstOrDefault(x => !string.IsNullOrEmpty(x?.InnerText) && x.InnerText.Contains(PostString));
                if(!string.IsNullOrEmpty(data?.InnerText) && data.InnerText.Contains("Invited"))
                {
                    pageResponse = "Already Invited";
                    goto Exit;
                }
                var clickIndex = Nodes != null && Nodes.Count > 0 ?Nodes.IndexOf(data) : 0;
                var script = "document.querySelectorAll('li[role=\"option\"]')[{0}].getBoundingClientRect().{1};";
                xy = await BrowserWindow.GetXAndYAsync(customScriptX: string.Format(script, clickIndex, "x"), customScriptY: string.Format(script, clickIndex, "y"));
                await BrowserWindow.MouseClickAsync(xy.Key + 10,xy.Value + 10,delayAfter: 3);
                await BrowserWindow.ExecuteScriptAsync("document.querySelector('button[class=\"artdeco-button artdeco-button--2 artdeco-button--primary ember-view\"]').click();", delayInSec: 4);
                pageResponse = await BrowserWindow.GetPageSourceAsync();
                if (!string.IsNullOrEmpty(pageResponse) && pageResponse.Contains("1 person invited to event"))
                {
                    pageResponse = "Invited";
                    await BrowserWindow.ExecuteScriptAsync("[...document.querySelectorAll('button')].filter(x=>x.innerText === \"Dismiss\")[0].click();", delayInSec: 3);
                }
            Exit:
                response.Response = pageResponse;
            }
            catch { }
            return new EventInviteResponseHandler(response,true);
        }

        public async Task<string> GenerateAIPrompt(DominatorAccountModel dominatorAccount, string QueryValue)
        {
            return QueryValue;
        }

        public async Task<OwnFollowerResponseHandler> GetAccountsFollowers(DominatorAccountModel dominatorAccount, string actionUrl)
        {
            IResponseParameter response = new ResponseParameter();
            var list = new List<string>();
            var count = 0;
            var lastCount = 0;
            await BrowserWindow.GoToCustomUrl(actionUrl, delayAfter: 6);
            var Nodes = await BrowserWindow.GetPaginationDataList("", true);
            list.AddRange(Nodes);
            while(count++< 6)
            {
                BrowserWindow.ClearResources();
                Nodes = await BrowserWindow.GetPaginationDataList("", true);
                if (lastCount == Nodes.Count)
                    break;
                lastCount = Nodes.Count;
            }
            response.Response = JsonConvert.SerializeObject(list);
            return new OwnFollowerResponseHandler(response,true);
        }

        public void SetDominatorAccount(DominatorAccountModel dominatorAccount)
        {
            if(_linkedInAccountModel is null && dominatorAccount != null)
                _linkedInAccountModel = dominatorAccount;
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

        public bool OnFileDialog(IWebBrowser chromiumWebBrowser, IBrowser browser, CefFileDialogMode mode, string title, string defaultFilePath, List<string> acceptFilters, IFileDialogCallback callback)
        {
            callback.Continue(_filesPath);
            return true;
        }
    }
}