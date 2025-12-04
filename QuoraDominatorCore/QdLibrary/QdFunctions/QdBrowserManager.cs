using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.PuppeteerBrowser;
using DominatorHouseCore.Request;
using DominatorHouseCore.Utility;
using QuoraDominatorCore.Enums;
using QuoraDominatorCore.Interface;
using QuoraDominatorCore.Models;
using QuoraDominatorCore.QdUtility;
using QuoraDominatorCore.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace QuoraDominatorCore.QdLibrary.QdFunctions
{
    public interface IQuoraBrowserManager : IBrowserManager, IBrowserManagerAsync
    {
        PuppeteerBrowserActivity BrowserWindow { get; set; }
        void CloseBrowser();
        Dictionary<string, string> Follow(string url);
        bool DownVoteQuestion(string url, DominatorAccountModel account);
        bool UpvoteAnswer(DominatorAccountModel accountModel, string url);
        bool DownVoteAnswer(string url);
        Dictionary<string,string> Unfollow();
        bool ReportUser(string url,string ReportDetails,ref string Result);
        bool ReportAnswer(string url,string ReportDetails,ref string Result);
        bool BroadCastMessage(string message, DominatorAccountModel account, ActivityType activity, string userName);
        IResponseParameter SearchByCustomUser(DominatorAccountModel account, string url,bool ExpandBioField=false);
        IResponseParameter SearchByKeyword(DominatorAccountModel account, string keyword);
        bool AnswerOnQuestion(DominatorAccountModel account, AnswerQuestionModel _answerQuestionModel, string url,
            string AnswerText);
        IResponseParameter GetFollwersAndFollowingsFromProfile(DominatorAccountModel account, UserFollowTypes type,int MaxUsersCount=200);
        IResponseParameter ClickOnViewMoreComments(DominatorAccountModel account, CommentFor type);
        IResponseParameter GetAnswerUpvoters(DominatorAccountModel account, ref int index);
        IResponseParameter SearchByCustomUrl(DominatorAccountModel account, string url);
        IResponseParameter SearchByCustomUrlAndScrollDown(DominatorAccountModel account);
        IResponseParameter PostQuestion(DominatorAccountModel accountModel, string question,
            string questionLink,out string finalUrl);
        IResponseParameter CreatePost(DominatorAccountModel dominatorAccount, string question, string media, out string finalUrl);
        void ScrollDown(DominatorAccountModel account);
        void Sleep(double seconds = 1);
        IResponseParameter TryAndGetResponse(string Url, string ContainString, int TryCount, DominatorAccountModel accountModel);
        void CheckBrowser(DominatorAccountModel account);
        UserInfoResponseHandler GetUserInfo(DominatorAccountModel account,string ProfileUrl, bool IsBrowser = true);
        string GetUserProfile(DominatorAccountModel dominatorAccount);
    }

    public class QdBrowserManager : IQuoraBrowserManager
    {
        private CancellationToken _cancellationToken;
        private bool isRunning = true;
        private IAccountScopeFactory _accountScopeFactory;
        private IQdLogInProcess _qdLoginProcess;
        public PuppeteerBrowserActivity BrowserWindow { get; set; }
        private readonly IQDSessionManager sessionManager;
        public QdBrowserManager(IQDSessionManager qDSessionManager)
        {
            sessionManager = qDSessionManager;
        }
        public bool BrowserLogin(DominatorAccountModel account, CancellationToken cancellationToken,
            LoginType loginType = LoginType.AutomationLogin, VerificationType verificationType = 0)
        { 
            _accountScopeFactory = InstanceProvider.GetInstance<IAccountScopeFactory>();
            var isBrowserOpen = true;
            if (verificationType == VerificationType.Phone) isBrowserOpen = false;
            _cancellationToken = cancellationToken;
            sessionManager.AddOrUpdateSession(ref account);
            Application.Current.Dispatcher.Invoke(async() =>
            {
                BrowserWindow = new PuppeteerBrowserActivity(account, isNeedResourceData: true, loginType: loginType);
                await BrowserWindow.LaunchBrowserAsync(Utilities.GetHeadLessStatus(loginType, account.AccountBaseModel.AccountNetwork));
            });
            Sleep(5);
            var last3Min = DateTime.Now.AddMinutes(2);
            while (!BrowserWindow.IsLoggedIn && last3Min >= DateTime.Now)
            {
                if (!BrowserWindow.IsLoaded)
                {
                    Sleep();
                    continue;
                }

                if (last3Min.AddMinutes(3.5) > DateTime.Now)
                    if (!BrowserWindow.IsLoggedIn && BrowserWindow.GetPageSource().Contains("name=\"password\"") &&
                        BrowserWindow.GetPageSource().Contains("name=\"email\""))
                    {
                        var usernameBoxXandY = BrowserWindow.GetXAndY(AttributeType.Type, "email");
                        BrowserWindow.MouseClick(usernameBoxXandY.Key + 10, usernameBoxXandY.Value + 3, delayBefore: 2, delayAfter: 1);
                        BrowserWindow.EnterChars(account.AccountBaseModel.UserName, .009, delayAtLast: 6);

                        var passwordBoxXandY = BrowserWindow.GetXAndY(AttributeType.Type, "password");
                        BrowserWindow.MouseClick(passwordBoxXandY.Key + 10, passwordBoxXandY.Value + 3, delayBefore: 2, delayAfter: 1);
                        BrowserWindow.EnterChars(account.AccountBaseModel.Password, .009, delayAtLast: 5);
                        var response = BrowserWindow.GetPageSource();
                        if (response.Contains("reCAPTCHA"))
                        {
                            while (!(response.Contains("What is your question or link?") || response.Contains("What do you want to ask or share?")))
                            {
                                Sleep(10);
                                response = BrowserWindow.GetPageSource();
                                cancellationToken.ThrowIfCancellationRequested();
                            }
                            //SolveCaptcha(response);
                        }
                        else
                            BrowserWindow.ExecuteScript($"[...document.querySelectorAll('button')].filter(x=>x.innerText.trim().includes(\"Login\"))[0].click();", 5);
                        Sleep(5);
                    }

                if (!BrowserWindow.IsLoggedIn)
                {
                    var result = BrowserWindow.GetPageSource();
                    if (!string.IsNullOrEmpty(result) && (result.Contains("What is your question or link?") || result.Contains("What do you want to ask or share?")
                        ||result.Contains("aria-label=\"Add question\"")))
                    {
                        //BrowserWindow.SaveCookies(false);
                        account = BrowserWindow.DominatorAccountModel;
                        BrowserWindow.IsLoggedIn = true;
                        account.Cookies = BrowserWindow.BrowserCookiesIntoModel().Result;
                        QdUtilities.RemoveUnUsedCookies(ref account);
                        account.IsUserLoggedIn = true;
                        account.AccountBaseModel.Status = AccountStatus.Success;

                        SocinatorAccountBuilder.Instance(account.AccountBaseModel.AccountId)
                            .AddOrUpdateDominatorAccountBase(account.AccountBaseModel)
                            .AddOrUpdateLoginStatus(account.IsUserLoggedIn)
                            .AddOrUpdateBrowserCookies(account.Cookies)
                            .AddOrUpdateCookies(account.Cookies)
                            .SaveToBinFile();
                        sessionManager.AddOrUpdateSession(ref account, true);
                        using (var globalDbOperation = new DbOperations(SocinatorInitialize.GetGlobalDatabase().GetSqlConnection()))
                        {
                            globalDbOperation.UpdateAccountDetails(account);
                        }
                    }
                }

                Sleep(2);

                if (account.IsUserLoggedIn)
                {
                    if (loginType == LoginType.InitialiseBrowser)
                        CloseBrowser(account);
                    break;
                }
            }

            return account.IsUserLoggedIn;
        }

        private string SolveCaptcha(string res)
        {
            var genericFileManager = InstanceProvider.GetInstance<IGenericFileManager>();
            var imageCaptchaServicesModel = genericFileManager.GetModel<DominatorHouseCore.Models.Config.ImageCaptchaServicesModel>(ConstantVariable.GetImageCaptchaServicesFile());
            var LoginClass = "qu-hover--textDecoration--none ClickWrapper___StyledClickWrapperBox-zoqi4f-0 iyYUZT base___StyledClickWrapper-lx6eke-1 hIqLpn";
            if (!string.IsNullOrEmpty(imageCaptchaServicesModel?.Token?.ToString()))
            {
                var source = Utilities.GetBetween(res, "title=\"reCAPTCHA\" src=\"", "\" width=\"");
                var siteurl = System.Net.WebUtility.HtmlDecode(source);
                var key = Utilities.GetBetween(siteurl, "&k=", "&");
                ImageTypersHelper imagetyperz = new ImageTypersHelper(imageCaptchaServicesModel.Token.ToString());
                var captchaid = imagetyperz.SubmitSiteKey(siteurl, key);
                string captcharesponse = string.Empty;
                while(string.IsNullOrEmpty(captcharesponse)) captcharesponse = imagetyperz.GetGResponseCaptcha(captchaid);
                var enteredValue = string.Empty;
                do
                {
                    enteredValue = BrowserWindow.ExecuteScript($"document.getElementById('g-recaptcha-response').value = '{captcharesponse}'").Result.ToString();
                } while (enteredValue != captcharesponse);
                var checkBoxXandY = BrowserWindow.GetXAndY(AttributeType.ClassName, "q-box qu-mb--medium", 1);
                BrowserWindow.MouseClick(checkBoxXandY.Key + 5, checkBoxXandY.Value + 5, delayBefore: 3, delayAfter: 3);
                BrowserWindow.BrowserAct(ActType.ClickByClass, LoginClass, 1, 3);
            }
            else
            {
                GlobusLogHelper.log.Info(Log.CustomMessage, "",
                            "", "", "Please check- \"I'm not a robot\"");
                Sleep(10);
                BrowserWindow.BrowserAct(ActType.ClickByClass,LoginClass, 3, 5);
            }
            res = BrowserWindow.GetPageSource();
            return res;
        }

        public void CloseBrowser()
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    BrowserWindow.Close();
                    BrowserWindow = null;
                });
            }
            catch (Exception)
            {
            }
        }

        public IResponseParameter SearchByCustomUser(DominatorAccountModel account, string user, bool ExpandBioField = false)
        {
            var isRunning = true;
            var url = "";
            var failedCount = 0;
            url = $"{QdConstants.HomePageUrl}/profile/" + user;
            CheckBrowser(account);
            var response = new ResponseParameter();
            Application.Current.Dispatcher.Invoke(async () =>
            {
                response.Response = await BrowserWindow.GoToCustomUrl(url, 5);

                while (!response.Response.Contains("Credentials"))
                {
                    await Task.Delay(5000);
                    response.Response = await BrowserWindow.GetPageSourceAsync();
                    failedCount++;
                }
                if (ExpandBioField)
                    await BrowserWindow.ExecuteScriptAsync("document.getElementsByClassName('q-text qu-cursor--pointer QTextTruncated__StyledReadMoreLink-sc-1pev100-3 dXJUbS qt_read_more qu-color--blue_dark qu-fontFamily--sans qu-pl--tiny')[0].click();", 5);
                isRunning = false;
            });
            while (isRunning) TimeSpan.FromSeconds(2);
            return response;
        }

        public Dictionary<string, string> Follow(string url)
        {
            var isRunning = true;
            var followResponse = string.Empty;
            Dictionary<string, string> Response = null;
            var IsFollowed = false;
            Application.Current.Dispatcher.Invoke(async () =>
            {
                followResponse = await BrowserWindow.GetPageSourceAsync();
                var ButtonNode = HtmlParseUtility.GetListInnerTextFromPartialTagNamecontains(followResponse,"button","role","button");
                IsFollowed = ButtonNode.Count > 0 && ButtonNode.Any(x => x.Contains("Following"));
                if (IsFollowed) goto Skip;
                await BrowserWindow.ExecuteScriptAsync("[...document.querySelectorAll('button[type=\"button\"]')].filter(x=>x.textContent.trim()===\"Follow\")[0].click();");
                Sleep(3);
                followResponse = await BrowserWindow.GetPaginationData("\"userFollow\": {\"status\": \"success\"", true);
                Skip:
                isRunning = false;
            });
            while (isRunning) Sleep(2);
            Response = Response ?? new Dictionary<string, string>();
            Response.Clear();
            Response.Add("Follow", IsFollowed ? "Already following this user." : "");
            return Response;
        }

        public bool UpvoteAnswer(DominatorAccountModel accountModel, string url)
        {
            var isRunning = true;
            var upvoteAnswerResp = string.Empty;
            CheckBrowser(accountModel);
            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    upvoteAnswerResp = await BrowserWindow.GoToCustomUrl(url, 5);
                    var UpvoteClassName = "puppeteer_test_votable_upvote_button";
                    upvoteAnswerResp = await BrowserWindow.GetPageSourceAsync();
                    var Nodes = HtmlParseUtility.GetListNodesFromClassName(upvoteAnswerResp, UpvoteClassName);
                    var ClickIndex = Nodes != null && Nodes.Count > 0 ?Nodes.IndexOf(Nodes.FirstOrDefault(x=>x.InnerText.Contains("Vote") || x.InnerText.Contains("Upvote") || x.InnerText.Contains("Dukung Naik"))) : 0;
                    var index = upvoteAnswerResp.Contains("Add question") ? 7 : 6;
                    if (ClickIndex < 0)
                        await BrowserWindow.ExecuteScriptAsync($"document.querySelectorAll(\"div div button\")[{index}].click();", 3);
                    else
                        await BrowserWindow.ExecuteScriptAsync($"document.getElementsByClassName('{UpvoteClassName}')[{ClickIndex}].click();");
                    
                    upvoteAnswerResp = await BrowserWindow.GoToCustomUrl(url, 5);
                }
                catch(Exception e) { e.DebugLog(); }
                isRunning = false;
            });
            while (isRunning) Sleep(2);
            return upvoteAnswerResp.Contains("puppeteer_test_votable_upvote_button puppeteer_test_pressed");
        }

        public IResponseParameter SearchByKeyword(DominatorAccountModel account, string keyword)
        {
            CheckBrowser(account);
            var response = new ResponseParameter();
            Application.Current.Dispatcher.Invoke(async () =>
            {
                response.Response = await BrowserWindow.GoToCustomUrl(keyword, 3);
                while (!response.Response.Contains("class=\"user\""))
                {
                    await Task.Delay(2000);
                    response.Response = await BrowserWindow.GetPageSourceAsync();
                }

                isRunning = false;
            });

            while (isRunning) Sleep(2);
            return response;
        }
        public IResponseParameter GetFollwersAndFollowingsFromProfile(DominatorAccountModel account, UserFollowTypes type,int MaxUsersCount=200)
        {
            var isRunning = true;
            CheckBrowser(account);
            var response = new ResponseParameter();
            var ScrollClass = "q-box qu-color--gray_dark qu-borderBottom qu-tapHighlight--none qu-display--flex qu-alignItems--center";
            switch (type)
            {
                case UserFollowTypes.Followers:
                    Application.Current.Dispatcher.Invoke(async () =>
                    {
                        try
                        {
                            //response.Response = await BrowserWindow.GetPageSourceAsync();
                            //var followerButtonAttribute = BrowserUtilities.GetPath(response.Response, "div", "followers");
                            //await BrowserWindow.BrowserActAsync(ActType.Click, AttributeType.ClassName, followerButtonAttribute, index: 0, delayBefore: 3, delayAfter: 5);
                            await BrowserWindow.ExecuteScriptAsync("document.getElementsByClassName('q-click-wrapper c1nud10e qu-display--inline-block qu-tapHighlight--white qu-cursor--pointer qu-hover--textDecoration--underline')[0].click()");
                            int previousCount = 0;
                            while (true)
                            {
                                var countResult = await BrowserWindow.ExecuteScriptAsync("document.getElementsByClassName('q-flex qu-alignItems--center qu-py--small qu-flex--auto qu-overflow--hidden').length");

                                int count = int.Parse(countResult.Result.ToString());

                                if (previousCount == count || previousCount >= MaxUsersCount) break;

                                await BrowserWindow.BrowserActAsync(ActType.ScrollIntoView, AttributeType.ClassName, ScrollClass, index: count - 1, delayAfter: 3);

                                previousCount = count;
                            }
                            response.Response = await BrowserWindow.GetPageSourceAsync();
                        }
                        catch (Exception e) { e.DebugLog(); }
                        isRunning = false;
                    });

                    while (isRunning) Sleep(2);
                    
                    break;


                case UserFollowTypes.Followings:
                    Application.Current.Dispatcher.Invoke(async () =>
                    {
                        await BrowserWindow.ExecuteScriptAsync("document.getElementsByClassName('q-click-wrapper c1nud10e qu-display--inline-block qu-tapHighlight--white qu-cursor--pointer qu-hover--textDecoration--underline')[1].click()");
                        int previousCount = 0;
                        while (true)
                        {
                            var countResult = await BrowserWindow.ExecuteScriptAsync("document.getElementsByClassName('q-flex qu-alignItems--center qu-py--small qu-flex--auto qu-overflow--hidden').length");

                            int count = int.Parse(countResult.Result.ToString());

                            if (previousCount == count || previousCount >= MaxUsersCount) break;

                            await BrowserWindow.BrowserActAsync(ActType.ScrollIntoView, AttributeType.ClassName, ScrollClass, index: count - 1, delayAfter: 3);

                            previousCount = count;
                        }
                        response.Response = await BrowserWindow.GetPageSourceAsync();

                        isRunning = false;
                    });

                    while (isRunning) Sleep(2);
                    break;

                case UserFollowTypes.OwnFollowings:

                    Application.Current.Dispatcher.Invoke(async () =>
                    {
                        await BrowserWindow.BrowserActAsync(ActType.Click, AttributeType.ClassName, "q-click-wrapper c1nud10e qu-display--inline-block qu-tapHighlight--white qu-cursor--pointer qu-hover--textDecoration--underline", index: 4, delayBefore: 2, delayAfter: 5);

                        int previousCount = 0;

                        while (true)
                        {
                            var countResult = await BrowserWindow.ExecuteScriptAsync("document.getElementsByClassName('q-flex qu-alignItems--center qu-py--small qu-flex--auto qu-overflow--hidden').length");

                            int count = int.Parse(countResult.Result.ToString());

                            if (previousCount == count || previousCount >= MaxUsersCount) break;

                            await BrowserWindow.BrowserActAsync(ActType.ScrollIntoView, AttributeType.ClassName, ScrollClass, index: count - 1, delayAfter: 3);

                            previousCount = count;
                        }

                        response.Response = await BrowserWindow.GetPageSourceAsync();

                        isRunning = false;
                    });

                    while (isRunning) Sleep(2);
                    break;
                case UserFollowTypes.OwnFollowers:

                    Application.Current.Dispatcher.Invoke(async () =>
                    {
                        await BrowserWindow.BrowserActAsync(ActType.Click, AttributeType.ClassName, "q-click-wrapper c1nud10e qu-display--inline-block qu-tapHighlight--white qu-cursor--pointer qu-hover--textDecoration--underline", index: 3, delayBefore: 2, delayAfter: 5);

                        int previousCount = 0;

                        while (true)
                        {
                            var countResult = await BrowserWindow.ExecuteScriptAsync("document.getElementsByClassName('q-relative qu-display--inline puppeteer_popper_reference').length");

                            int count = int.Parse(countResult.Result.ToString());

                            if (previousCount == count || previousCount >= MaxUsersCount) break;

                            await BrowserWindow.BrowserActAsync(ActType.ScrollIntoView, AttributeType.ClassName, ScrollClass, index: count - 1, delayAfter: 3);

                            previousCount = count;
                        }

                        response.Response = await BrowserWindow.GetPageSourceAsync();

                        isRunning = false;
                    });

                    while (isRunning)
                    {
                        Sleep(2);
                    }
                    break;
            }
            return response;
        }

        public IResponseParameter SearchByCustomUrlAndScrollDown(DominatorAccountModel account)
        {
            var isRunning = true;
            CheckBrowser(account);
            var response = new ResponseParameter();
            Application.Current.Dispatcher.Invoke(async () =>
            {
                BrowserWindow.PressAnyKey(n: 25, delay: 0.2, winKeyCode: 40);
                await BrowserWindow.BrowserActAsync(ActType.ScrollWindow, AttributeType.Null, "",
                    delayAfter: 3, scrollByPixel: 3000);
                response.Response = await BrowserWindow.GetPageSourceAsync();
                isRunning = false;
            });
            while (isRunning) Thread.Sleep(TimeSpan.FromSeconds(2));

            return response;
        }

        public void ScrollDown(DominatorAccountModel account)
        {
            var isRunning = true;
            CheckBrowser(account);
            var response = new ResponseParameter();
            Application.Current.Dispatcher.Invoke(async () =>
            {
                response.Response = await BrowserWindow.GetPageSourceAsync();
                isRunning = false;
            });
            while (isRunning) Thread.Sleep(TimeSpan.FromSeconds(2));
        }


        public IResponseParameter SearchByCustomUrl(DominatorAccountModel account, string url)
        {
            var response = new ResponseParameter();
            CheckBrowser(account);
            BrowserWindow.ClearResources();
            BrowserWindow.GoToUrl(url);
            Sleep(8);
            response.Response = BrowserWindow.GetPageSourceAsync().Result;
            return response;
        }

        public bool DownVoteQuestion(string url, DominatorAccountModel account)
        {
            bool isRunning = true;
            int pressed = 1;
            CheckBrowser(account);
            var downvoteQuestionResp = string.Empty;
            Application.Current.Dispatcher.Invoke(async () =>
            {
                await BrowserWindow.ExecuteScriptAsync("document.querySelectorAll('button[aria-label=\"Downvote\"][aria-pressed=\"false\"]')[0].click()");
                downvoteQuestionResp = await BrowserWindow.GetPageSourceAsync();
                if (int.TryParse((await BrowserWindow.ExecuteScriptAsync("document.querySelectorAll('button[aria-label=\"Downvote\"][aria-pressed=\"false\"]').length")).Result?.ToString(), out pressed) && pressed > 0)
                {
                    await BrowserWindow.BrowserActAsync(ActType.Click, AttributeType.ClassName, "q-click-wrapper base___StyledClickWrapper-lx6eke-0 puppeteer_test_pressed b10gcu9l bobc9nh b1cg7ppz c1nud10e qu-active--bg--darken qu-borderRadius--pill qu-alignItems--center qu-justifyContent--center qu-whiteSpace--nowrap qu-userSelect--none qu-display--inline-flex qu-tapHighlight--white qu-textAlign--center qu-cursor--pointer qu-hover--bg--darken", index: 0, delayBefore: 2, delayAfter: 5);
                    downvoteQuestionResp = await BrowserWindow.GoToCustomUrl(url, 5);
                    int.TryParse((await BrowserWindow.ExecuteScriptAsync("document.querySelectorAll('button[aria-label=\"Downvote\"][aria-pressed=\"false\"]').length")).Result?.ToString(), out pressed);
                }
                isRunning = false;
            });
            while (isRunning)
            {
                Sleep(2);
            }
            return pressed == 0;
        }

        public bool DownVoteAnswer(string url)
        {
            var isRunning = true;
            var downvoteAnswerResp = string.Empty;

            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    await BrowserWindow.ExecuteScriptAsync("document.querySelectorAll('button[aria-label=\"Downvote\"]')[0].click()");
                    downvoteAnswerResp = await BrowserWindow.GoToCustomUrl(url, 5);
                }
                catch (Exception e) { e.DebugLog(); }


                isRunning = false;
            });
            while (isRunning)
            {
                Sleep(2);
            }
            return downvoteAnswerResp
                .Contains("colors-text-red_error");

        }

        public bool ReportUser(string url, string ReportDetails,ref string Result)
       {
            var isRunning = true;
            var reportUserResp = string.Empty;
            var ReportModel=QdUtilities.GetReportDetails(ReportDetails);
            var result = string.Empty;
            var IsSuccess = false;
            Application.Current.Dispatcher.Invoke(async () =>
            {
                reportUserResp = await BrowserWindow.GetPageSourceAsync();
                var ReportOptionClass = "q-box puppeteer_test_popover_menu";
                await BrowserWindow.ExecuteScriptAsync("document.querySelectorAll('div>div>button[aria-haspopup=\"menu\"]')[2].click();");
                reportUserResp = await BrowserWindow.GetPageSourceAsync();
                var executed = await BrowserWindow.ExecuteScriptAsync("[...document.getElementsByClassName('q-box puppeteer_test_popover_menu')[0].querySelectorAll('*')].find(x=>x.innerText === \"Reported\").click();", delayInSec: 2);
                if(executed != null && executed.Success)
                {
                    result = "Already Reported To This User";
                    goto FinalExit;
                }
                await BrowserWindow.ExecuteScriptAsync("[...document.querySelectorAll(\"div\")].filter(button=>button.textContent.trim()===\"Report\")[0].click();", 2);
                reportUserResp = await BrowserWindow.GetPageSourceAsync();
                var Nodes = HtmlUtility.GetListInnerTextFromClassName(reportUserResp, "q-text qu-bold");
                var ClickIndex = Nodes != null && Nodes.Count > 0 && !string.IsNullOrEmpty(ReportModel.ReportOptionTitle) && !ReportModel.ReportOptionTitle.Contains("User content") ? Nodes.IndexOf(Nodes.FirstOrDefault(x=>x.Contains(ReportModel.ReportOptionTitle))):string.IsNullOrEmpty(ReportModel.ReportOptionTitle)||ReportModel.ReportOptionTitle.Contains("User content") ? 4 : 0;
                await BrowserWindow.ExecuteScriptAsync($"document.getElementsByClassName('q-text qu-bold')[{ClickIndex+1}].click();");
                await Task.Delay(TimeSpan.FromSeconds(3));
                if (string.IsNullOrEmpty(ReportModel.ReportOptionTitle) || ReportModel.ReportOptionTitle.Contains("User content"))
                    goto ExitReporting;
                //Proceeding with More SubOptions.
                reportUserResp = await BrowserWindow.GetPageSourceAsync();
                await BrowserWindow.ExecuteScriptAsync($"[...document.querySelectorAll('label')].find(x=>x.innerText.toLowerCase() === \"{ReportModel?.ReportOptionTitle?.ToLower()}\").click();", delayInSec: 3);
                await BrowserWindow.ExecuteScriptAsync($"[...document.querySelectorAll('label')].find(x=>x.innerText.toLowerCase().includes(\"{ReportModel?.Title?.ToLower()}\")).click();", delayInSec: 3);
                var ReporUserDescriptionClass = "q-click-wrapper qu-display--block qu-tapHighlight--none qu-cursor--pointer ClickWrapper___StyledClickWrapperBox-zoqi4f-0";
                if (ReportModel.Title.Contains("Impersonation violation"))
                    goto ExitReporting;
                await BrowserWindow.ExecuteScriptAsync($"document.getElementsByClassName('{ReporUserDescriptionClass}')[{ClickIndex}].click();");
                await BrowserWindow.EnterCharsAsync(" "+ReportModel.ReportDescription);
            ExitReporting:
                reportUserResp = await BrowserWindow.GetPageSourceAsync();
                var submitButtonValue = BrowserUtilities.GetPath(reportUserResp, "button", "Submit");
                await BrowserWindow.BrowserActAsync(ActType.Click, AttributeType.ClassName, submitButtonValue, index: 0, delayBefore: 3, delayAfter: 5);
                await BrowserWindow.ExecuteScriptAsync("document.querySelectorAll('div>div>button[aria-haspopup=\"menu\"]')[2].click();");
                var value = await BrowserWindow.GetListInnerHtml(ActType.GetValue, AttributeType.ClassName,ReportOptionClass);
                IsSuccess = value!= null&& value.Count > 0 && value.Any(z=>z.Contains("Reported"));
            FinalExit:
                reportUserResp = await BrowserWindow.GoToCustomUrl(url, 5);
                isRunning = false;
            });
            while (isRunning) Sleep(2);
            BrowserWindow.ClearResources();
            if(!IsSuccess && string.IsNullOrEmpty(result))
                IsSuccess = reportUserResp.Contains("You have also blocked") || reportUserResp.Contains("You've blocked") || reportUserResp.Contains("Blocked");
            Result = result;
            return IsSuccess;
        }

        public bool ReportAnswer(string url,string ReportDetails, ref string Result)
        {
            var isRunning = true;
            var reportAnswerResp = string.Empty;
            var IsSuccess = false;
            var result = string.Empty;
            var ReportModel=QdUtilities.GetReportDetails(ReportDetails);
            Application.Current.Dispatcher.Invoke(async () =>
            {
                reportAnswerResp = await BrowserWindow.GoToCustomUrl(url, 5);
                await BrowserWindow.ExecuteScriptAsync("document.querySelectorAll('span[aria-label=\"More\"]')[0].click();");
                reportAnswerResp = await BrowserWindow.GetPageSourceAsync();
                var ReportOptionClass = "puppeteer_test_popover_item";
                var Nodes = await BrowserWindow.GetListInnerHtml(ActType.GetValue, AttributeType.ClassName,ReportOptionClass);
                Nodes.Reverse();
                int index =Nodes!=null && Nodes.Count >0 ? Nodes.IndexOf(Nodes.FirstOrDefault(x => x.Contains("Report"))):0;
                if(Nodes!=null && Nodes.Count >0 && Nodes.Any(node=>node.Contains("Reported")))
                {
                    result = "Already Reported To This Answer";
                    goto FinalExit;
                }
                await BrowserWindow.BrowserActAsync(ActType.Click, AttributeType.ClassName,ReportOptionClass, index: index, delayBefore: 3, delayAfter: 5);
                Nodes = await BrowserWindow.GetListInnerHtml(ActType.GetValue, AttributeType.ClassName, "q-text qu-bold");
                Nodes.Reverse();
                index =Nodes!=null && Nodes.Count > 0? Nodes.IndexOf(Nodes.FirstOrDefault(x => x.Contains(ReportModel.Title)||x.Contains(ReportModel.Description))):0;
                await BrowserWindow.ExecuteScriptAsync($"document.getElementsByClassName('q-text qu-bold')[{index}].click();");
                if(ReportModel != null && ReportModel.SubOption!=null && ReportModel.SubOption.HaveSubOption && !string.IsNullOrEmpty(ReportModel.SubOption.Title))
                {
                    Nodes = await BrowserWindow.GetListInnerHtml(ActType.GetValue, AttributeType.ClassName, "q-flex qu-alignItems--center qu-py--small qu-flex--auto qu-overflow--hidden");
                    Nodes.Reverse();
                    index = Nodes != null && Nodes.Count > 0 ? Nodes.IndexOf(Nodes.FirstOrDefault(x => x.Contains(ReportModel.SubOption.Title) || x.Contains(ReportModel.SubOption.Description))) : 1;
                    await BrowserWindow.ExecuteScriptAsync($"document.getElementsByClassName('q-flex qu-alignItems--center qu-py--small qu-flex--auto qu-overflow--hidden')[{index}].click();",delayInSec:3);
                }
                Sleep(3);
                if (!string.IsNullOrEmpty(ReportModel.ReportDescription))
                {
                    var xy = await BrowserWindow.GetXAndYAsync(AttributeType.ClassName, "doc empty", 0);
                    await BrowserWindow.MouseClickAsync(xy.Key + 10, xy.Value + 20, delayAfter: 5);
                    await BrowserWindow.EnterCharsAsync(" " + ReportModel.ReportDescription,delayAtLast:3);
                }
                reportAnswerResp = await BrowserWindow.GetPageSourceAsync();
                var submitButtonValue = BrowserUtilities.GetPath(reportAnswerResp, "button", "Submit");
                await BrowserWindow.BrowserActAsync(ActType.Click, AttributeType.ClassName,
                    submitButtonValue, delayAfter: 5);
                await BrowserWindow.ExecuteScriptAsync("document.querySelectorAll('span[aria-label=\"More\"]')[0].click();");
                Nodes = await BrowserWindow.GetListInnerHtml(ActType.GetValue, AttributeType.ClassName, ReportOptionClass);
                IsSuccess = Nodes != null && Nodes.Count > 0 && Nodes.Any(node => node.Contains("Reported"));
            FinalExit:
                reportAnswerResp = await BrowserWindow.GoToCustomUrl(url, 5);
                isRunning = false;
            });
            while (isRunning) Sleep(2);
            Result = result;
            if (!IsSuccess && string.IsNullOrEmpty(result))
            {
                var firstSplitedResp = Regex.Split(reportAnswerResp, "name=\"Comment\"")?.FirstOrDefault();
                IsSuccess = firstSplitedResp.Contains("q-text qu-display--inline-flex qu-alignItems--center qu-overflow--hidden puppeteer_test_button_text qu-medium qu-color--white");
            }
            return IsSuccess;
        }

        public bool AnswerOnQuestion(DominatorAccountModel account, AnswerQuestionModel answerScraperModel, string url,
            string AnswerText)
        {
            var isRunning = true;
            var answerOnQuestionResp = string.Empty;
            var currentUrl = string.Empty;
            Application.Current.Dispatcher.Invoke(async () =>
            {
                currentUrl = BrowserWindow.CurrentUrl();

                answerOnQuestionResp = await BrowserWindow.GetPageSourceAsync();
                var answerBoxIndex = currentUrl.StartsWith($"{QdConstants.HomePageUrl}/") ? 1 : 2;
                await BrowserWindow.ExecuteScriptAsync("document.querySelectorAll(\"button div div div\")[4].click();");
                answerOnQuestionResp = await BrowserWindow.GetPageSourceAsync();
                if(!answerOnQuestionResp.Contains("Write your answer"))
                    await BrowserWindow.ExecuteScriptAsync("[...document.querySelectorAll('button div div div')].filter(x=>x.textContent.trim()===\"Answer\")[0].click();");
                var answerBoxXAndY = await BrowserWindow.GetXAndYAsync(AttributeType.ClassName, "doc empty", 0);
                await BrowserWindow.MouseClickAsync(answerBoxXAndY.Key, answerBoxXAndY.Value, delayAfter: 3);
                BrowserWindow.ChooseFileFromDialog(string.IsNullOrEmpty(answerScraperModel.MediaPath)?answerScraperModel.ManageCommentModel.MediaPath:answerScraperModel.MediaPath);
                if(AnswerText.StartsWith("[")&&AnswerText.EndsWith("]") )
                {
                    AnswerText = AnswerText.Substring(1, AnswerText.Length - 2);
                    var UrlList= Regex.Split(AnswerText, ",");
                    foreach(var Answerurl in UrlList)
                        await BrowserWindow.CopyPasteContentAsync(AnswerText, 86, delayAtLast: 2);
                }
                else
                    await BrowserWindow.CopyPasteContentAsync(AnswerText, 86, delayAtLast: 2);
                if (!string.IsNullOrEmpty(answerScraperModel.LstManageCommentModel.FirstOrDefault().MediaPath))
                {
                    var photoBoxXAndY = await BrowserWindow.GetXAndYAsync(AttributeType.ClassName, "q-click-wrapper qu-alignItems--center qu-justifyContent--center qu-mr--small qu-borderRadius--small qu-borderAll qu-display--inline-flex qu-tapHighlight--white qu-cursor--pointer qu-hover--borderColor--blue qu-hover--textDecoration--underline ClickWrapper___StyledClickWrapperBox-zoqi4f-0 fZQsLF", 1);
                    await BrowserWindow.MouseClickAsync(photoBoxXAndY.Key, photoBoxXAndY.Value, delayAfter: 3);
                }
                answerOnQuestionResp = await BrowserWindow.GetPageSourceAsync();
                var containText = currentUrl.StartsWith(QdConstants.HomePageUrl) ? "post" : "Submit";
                await BrowserWindow.BrowserActAsync(ActType.Click, AttributeType.ClassName, "puppeteer_test_modal_submit", string.Empty, 0, 3, 0);
                Sleep(4);
                await BrowserWindow.BrowserActAsync(ActType.Click, AttributeType.ClassName, "puppeteer_test_modal_submit",string.Empty,0,3,0);
                Sleep(4);
                await BrowserWindow.ExecuteScriptAsync("[...document.querySelectorAll('button[aria-haspopup=\"menu\"]')].filter(x=>x.textContent.trim()===\"Recent\"||x.textContent.trim()===\"Upvotes\"||x.textContent.trim()===\"Recommended\")[0].click();");
                Sleep(4);
                answerOnQuestionResp = await BrowserWindow.GetPageSourceAsync();
                var Nodes = HtmlUtility.GetListNodesFromClassName(answerOnQuestionResp, "q-text qu-dynamicFontSize--small qu-color--gray_dark");
                var Index = Nodes.Count > 0 ?Nodes.IndexOf(Nodes.FirstOrDefault(x=>x.InnerText.Contains("Recent"))): 0;
                await BrowserWindow.ExecuteScriptAsync($"document.getElementsByClassName(\"q-text qu-dynamicFontSize--small qu-color--gray_dark\")[{Index}].click();");
                Sleep(4);
                answerOnQuestionResp = await BrowserWindow.GetPageSourceAsync();
                isRunning = false;
            });
            while (isRunning) Sleep(2);
            var AnswerNodes = HtmlUtility.GetListInnerTextFromClassName(answerOnQuestionResp, "q-box dom_annotate_question_answer_item");
            var IsSuccess = AnswerNodes.Count > 0 ? AnswerNodes.Any(x=>x.Contains(AnswerText)) : false;
            return IsSuccess?true:(answerOnQuestionResp.Contains("Edit draft")||answerOnQuestionResp.Contains("You've written an answer") || answerOnQuestionResp.Contains("You can edit or delete it at anytime")|| BrowserWindow.CurrentUrl().Contains("/answer/"));
        }

        public static SemaphoreSlim _copyPasteLock = new SemaphoreSlim(1, 1);

        public async Task<bool> CopyPasteTextAsync(string message, string className = "",
               string activityType = "POST", int parentIndex = 0)
        {
            try
            {
                int retryCount = 0;

                string pageElement = string.Empty;

                await _copyPasteLock.WaitAsync();

                List<string> splitMessage = Regex.Split(message?.Replace("\r\n", "\n"), "\n").Where(x => !string.IsNullOrEmpty(x))?.ToList();

                while (pageElement == null || splitMessage.Any(x => !pageElement.Replace("\r\n", string.Empty).Replace("\n", string.Empty).Contains(x)))
                {
                    var chkpageElement = pageElement?.Replace("\r\n", string.Empty)?.Replace("\n", string.Empty) ?? string.Empty;
                    if (!string.IsNullOrEmpty(chkpageElement) && splitMessage.Any(x => !chkpageElement.Contains(x)))
                        break;

                    if (retryCount > 0)
                        await BrowserWindow.CopyPasteContentAsync(winKeyCode: 90, delayAtLast: 1);

                    if (retryCount >= 5)
                    {
                        await BrowserWindow.CopyPasteContentAsync(winKeyCode: 90, delayAtLast: 1);
                        return false;
                    }

                    try
                    {
                        await BrowserWindow.CopyPasteContentAsync(message, 86, delayAtLast: 1);
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                    if (activityType == ActivityType.AnswerOnQuestions.ToString())
                    {
                        pageElement = await BrowserWindow.GetElementValueAsync(ActType.GetValue, AttributeType.ClassName,
                                className, ValueTypes.InnerText, clickIndex: parentIndex);                      
                    }
                   

                    retryCount++;

                }

                if (retryCount < 5)
                {
                    return true;
                }

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            finally { _copyPasteLock.Release(); }
            return false;
        }

        public bool BroadCastMessage(string message, DominatorAccountModel account, ActivityType activity,
            string userName)
        {
            if (activity != ActivityType.AutoReplyToNewMessage)
            {
                var isRunning = true;
                var messageResp = string.Empty;
                Application.Current.Dispatcher.Invoke(async () =>
                {
                    messageResp = await BrowserWindow.GetPageSourceAsync();
                    await BrowserWindow.ExecuteScriptAsync("document.querySelectorAll(\"div div button\")[11].click();");
                    var executed = await BrowserWindow.ExecuteScriptAsync("[...document.getElementsByClassName('q-box puppeteer_test_popover_menu')[0].querySelectorAll('*')].find(x=>x.innerText === \"Message\").click();", delayInSec: 3);
                    if (executed != null && executed.Success)
                    {
                        var msgBoxXAndY = await BrowserWindow.GetXAndYAsync(AttributeType.ClassName,
                            "doc empty", 0);
                        await BrowserWindow.MouseClickAsync(msgBoxXAndY.Key, msgBoxXAndY.Value, delayAfter: 3);
                        await BrowserWindow.CopyPasteContentAsync(message,86, delayAtLast: 3);
                        messageResp = await BrowserWindow.GetPageSourceAsync();
                        BrowserWindow.ClearResources();
                        var submitButtonValue = BrowserUtilities.GetPath(messageResp, "button", "Send");
                        await BrowserWindow.BrowserActAsync(ActType.Click, AttributeType.ClassName,
                            submitButtonValue, delayAfter: 3);
                        messageResp = await BrowserWindow.GetPaginationData("{\"data\": {\"threadCreate\": {\"status\": \"success\"}}", true);
                        messageResp = string.IsNullOrEmpty(messageResp)?await BrowserWindow.GetPaginationData("{\"data\": {\"threadReplyAdd\": {\"status\": \"success\"", true):messageResp;
                        isRunning = false;
                    }
                    else
                    {
                        GlobusLogHelper.log.Info(Log.ActivityFailed, account.AccountBaseModel.AccountNetwork,
                            account.AccountBaseModel.UserName, activity, $"Message option is not there for {userName}");
                        messageResp = string.Empty;
                        isRunning = false;
                    }
                });
                while (isRunning) Sleep(2);
                return !string.IsNullOrEmpty(messageResp);
            }
            else
            {
                var isRunning = true;
                var messageResp = string.Empty;
                Application.Current.Dispatcher.Invoke(async () =>
                {
                    var messageBoxXandY = await BrowserWindow.GetXAndYAsync(AttributeType.ClassName, "doc empty", 0);
                    await BrowserWindow.MouseClickAsync(messageBoxXandY.Key + 5, messageBoxXandY.Value + 5, delayBefore: 2, delayAfter: 3);
                    await BrowserWindow.EnterCharsAsync(message, .009, delayAtLast: 3);

                    messageResp = await BrowserWindow.GetPageSourceAsync();
                    var sendButtonValue = BrowserUtilities.GetPath(messageResp, "button", "Send");
                    await BrowserWindow.BrowserActAsync(ActType.Click, AttributeType.ClassName, sendButtonValue, index: 0, delayBefore: 3, delayAfter: 5);
                    
                    messageResp = await BrowserWindow.GetPageSourceAsync();
                    isRunning = false;
                });
                while (isRunning) Sleep(2);
                return messageResp.Contains("q-inlineFlex qu-alignItems--center");
            }
        }

        public Dictionary<string,string> Unfollow()
        {
            var isRunning = true;
            var unFollowResponse = string.Empty;
            var FinalResponse =new Dictionary<string, string>();
            bool IsFollowing = true;
            Application.Current.Dispatcher.Invoke(async () =>
            {
                unFollowResponse = await BrowserWindow.GetPageSourceAsync();
                var ProfileBox = Regex.Split(unFollowResponse, "q-flex qu-flexDirection--column");
                var ButtonNode = HtmlUtility.GetListInnerTextFromClassName(unFollowResponse, "q-text qu-ellipsis qu-whiteSpace--nowrap");
                IsFollowing = ButtonNode.Count > 0 && ButtonNode.Any(x => x.ToString().Contains("Following"));
                if (!IsFollowing)
                    goto Skip;
                await BrowserWindow.ExecuteScriptAsync("[...document.querySelectorAll('button[role=\"button\"]')].find(x=>x.innerText===\"Following\").click();", 3);
                unFollowResponse = await BrowserWindow.GetPageSourceAsync(delay: 3);
                Skip:
                isRunning = false;
            });
            while (isRunning) TimeSpan.FromSeconds(2);
            BrowserWindow.ClearResources();
            FinalResponse.Add("UnFollow",(!IsFollowing)?"Your Are Already Unfollowed This User Or Not Following This User.":"");
            return FinalResponse;
        }
        

        public IResponseParameter PostQuestion(DominatorAccountModel accountModel, string question, string questionLink,out string finalUrl)
        
        {
            var isRunning = true;
            finalUrl = string.Empty;
            CheckBrowser(accountModel);
            var URL = string.Empty;
            var SuggestedQuestion = string.Empty;
            var AskQuestionClass = "q-click-wrapper qu-active--bg--darken qu-borderRadius--pill qu-alignItems--center qu-justifyContent--center qu-whiteSpace--nowrap qu-userSelect--none qu-display--flex qu-tapHighlight--white qu-textAlign--center qu-cursor--pointer qu-hover--bg--darken ClickWrapper___StyledClickWrapperBox-zoqi4f-0 iyYUZT base___StyledClickWrapper-lx6eke-1 cdVMwV";
            var response = new ResponseParameter();
            Application.Current.Dispatcher.Invoke(async () =>
            {
                var Nodes = HtmlUtility.GetListInnerTextFromClassName(await BrowserWindow.GetPageSourceAsync(), AskQuestionClass);
                var ClickIndex = Nodes.Count > 0 ? Nodes.IndexOf(Nodes.FirstOrDefault(node => node.Contains("Ask"))) : 0;
                await BrowserWindow.BrowserActAsync(ActType.Click, AttributeType.ClassName, AskQuestionClass, index: ClickIndex, delayBefore: 3, delayAfter: 7);
                await BrowserWindow.EnterCharsAsync(question, typingDelay: .03);
                Sleep(10);
                //Clicking on Add question.
                await BrowserWindow.BrowserActAsync(ActType.Click, AttributeType.ClassName,
                    "ClickWrapper___StyledClickWrapperBox-zoqi4f-0 iyYUZT base___StyledClickWrapper-lx6eke-1 hIqLpn puppeteer_test_modal_submit",index:0, delayBefore: 2, delayAfter: 5);
                response.Response = await BrowserWindow.GetPageSourceAsync();
                if (response.Response.Contains("Double-check your question"))
                {
                    //Clicking on UseSuggestions.
                    await BrowserWindow.BrowserActAsync(ActType.Click, AttributeType.ClassName,
                        "q-click-wrapper qu-active--textDecoration--none qu-focus--textDecoration--none qu-borderRadius--pill qu-alignItems--center qu-justifyContent--center qu-whiteSpace--nowrap qu-userSelect--none qu-display--inline-flex qu-bg--blue qu-tapHighlight--white qu-textAlign--center qu-cursor--pointer qu-hover--textDecoration--none ClickWrapper___StyledClickWrapperBox-zoqi4f-0 iyYUZT base___StyledClickWrapper-lx6eke-1 hIqLpn puppeteer_test_modal_submit", delayBefore: 2, delayAfter: 5, index: 1);
                    response.Response = await BrowserWindow.GetPageSourceAsync();

                    if (response.Response.Contains("Edit topics"))
                    {
                        //For Unchecking topic
                        await BrowserWindow.BrowserActAsync(ActType.Click, AttributeType.ClassName,
                        "q-box qu-py--tiny qu-cursor--pointer qu-tapHighlight--none qu-display--flex qu-alignItems--center", delayBefore: 2, delayAfter: 5, index: 1);

                        //Clicking on submit.
                        await BrowserWindow.BrowserActAsync(ActType.Click, AttributeType.ClassName,
                            "q-click-wrapper qu-active--textDecoration--none qu-focus--textDecoration--none ClickWrapper___StyledClickWrapperBox-zoqi4f-0 bARCkM base___StyledClickWrapper-lx6eke-1 kjJTYv puppeteer_test_modal_submit  qu-borderRadius--pill qu-alignItems--center qu-justifyContent--center qu-whiteSpace--nowrap qu-userSelect--none qu-display--inline-flex qu-bg--blue qu-tapHighlight--white qu-textAlign--center qu-cursor--pointer qu-hover--textDecoration--none", delayBefore: 2, delayAfter: 5, index: 1);

                        Sleep(3);
                        response.Response = await BrowserWindow.GetPageSourceAsync();

                        if (response.Response.Contains("Request Answers"))
                            //Clicking on Done.
                            await BrowserWindow.BrowserActAsync(ActType.Click, AttributeType.ClassName,
                                "q-click-wrapper qu-active--textDecoration--none qu-focus--textDecoration--none ClickWrapper___StyledClickWrapperBox-zoqi4f-0 bARCkM base___StyledClickWrapper-lx6eke-1 kjJTYv puppeteer_test_modal_submit  qu-borderRadius--pill qu-alignItems--center qu-justifyContent--center qu-whiteSpace--nowrap qu-userSelect--none qu-display--inline-flex qu-bg--blue qu-tapHighlight--white qu-textAlign--center qu-cursor--pointer qu-hover--textDecoration--none", delayBefore: 2, delayAfter: 5, index: 3);
                    }
                }
                else if (response.Response.Contains("Edit topics"))
                {
                    //For Unchecking topic
                    await BrowserWindow.BrowserActAsync(ActType.Click, AttributeType.ClassName,
                    "q-box qu-py--tiny qu-cursor--pointer qu-tapHighlight--none qu-display--flex qu-alignItems--center", delayBefore: 2, delayAfter: 5, index: 1);

                    //Clicking on submit.
                    await BrowserWindow.BrowserActAsync(ActType.Click, AttributeType.ClassName,
                        "q-click-wrapper qu-active--textDecoration--none qu-focus--textDecoration--none ClickWrapper___StyledClickWrapperBox-zoqi4f-0 bARCkM base___StyledClickWrapper-lx6eke-1 kjJTYv puppeteer_test_modal_submit  qu-borderRadius--pill qu-alignItems--center qu-justifyContent--center qu-whiteSpace--nowrap qu-userSelect--none qu-display--inline-flex qu-bg--blue qu-tapHighlight--white qu-textAlign--center qu-cursor--pointer qu-hover--textDecoration--none", delayBefore: 2, delayAfter: 5, index: 1);

                    Sleep(3);
                    response.Response = await BrowserWindow.GetPageSourceAsync();

                    if (response.Response.Contains("Request Answers"))
                        //Clicking on Done.
                        await BrowserWindow.BrowserActAsync(ActType.Click, AttributeType.ClassName,
                            "q-click-wrapper qu-active--textDecoration--none qu-focus--textDecoration--none ClickWrapper___StyledClickWrapperBox-zoqi4f-0 bARCkM base___StyledClickWrapper-lx6eke-1 kjJTYv puppeteer_test_modal_submit  qu-borderRadius--pill qu-alignItems--center qu-justifyContent--center qu-whiteSpace--nowrap qu-userSelect--none qu-display--inline-flex qu-bg--blue qu-tapHighlight--white qu-textAlign--center qu-cursor--pointer qu-hover--textDecoration--none", delayBefore: 2, delayAfter: 5, index: 3);
                }
                if (response.Response.Contains("Request Answers"))
                 SuggestedQuestion=   await BrowserWindow.GetElementValueAsync(ActType.GetValue, AttributeType.ClassName,
                           "QuestionTitle___StyledText-exj38m-0 chNUqN puppeteer_test_question_title", valueType: ValueTypes.InnerText, clickIndex: 6);
                //Clicking On Done.
                    await BrowserWindow.BrowserActAsync(ActType.Click, AttributeType.ClassName,
                        "q-click-wrapper qu-active--textDecoration--none qu-focus--textDecoration--none qu-borderRadius--pill qu-alignItems--center qu-justifyContent--center qu-whiteSpace--nowrap qu-userSelect--none qu-display--inline-flex qu-bg--blue qu-tapHighlight--white qu-textAlign--center qu-cursor--pointer qu-hover--textDecoration--none ClickWrapper___StyledClickWrapperBox-zoqi4f-0 iyYUZT base___StyledClickWrapper-lx6eke-1 hIqLpn puppeteer_test_modal_submit", delayBefore: 2, delayAfter: 5, index: 2);
                Sleep(7);
                //Clicking on Skip
                await BrowserWindow.BrowserActAsync(ActType.Click, AttributeType.ClassName, "base___StyledClickWrapper-lx6eke-1 cdVMwV puppeteer_test_modal_cancel", delayBefore: 2, delayAfter: 8, index: 2);
                //response.Response = await BrowserWindow.GetPageSourceAsync();
                URL = BrowserWindow.GetCurrentPageurl();
                isRunning = false;
            });
            while (isRunning) Sleep(2);
            finalUrl = URL.Contains("unanswered")||string.IsNullOrEmpty(SuggestedQuestion)?URL: $"{QdConstants.HomePageUrl}/unanswered/" + SuggestedQuestion.Replace(" ", "-").Replace("?", "").Replace("(", "").Replace(")", "").Replace(",", "");
            return response;
        }

        void IQuoraBrowserManager.Sleep(double seconds)
        {
            Task.Delay(TimeSpan.FromSeconds(seconds)).Wait(_cancellationToken);
        }


        public void CloseBrowser(DominatorAccountModel account)
        {
            var isRunning = true;
            try
            {
                if (BrowserWindow == null) return;
                Application.Current.Dispatcher.Invoke(async () =>
                {
                    try
                    {
                        BrowserWindow.Close();
                        BrowserWindow.Dispose();
                        await Task.Delay(1000);
                        isRunning = false;
                    }
                    catch (Exception)
                    {
                    }
                });
            }
            catch (Exception)
            {
            }

            while (isRunning)
                Task.Delay(500).Wait();
        }


        private void Sleep(double seconds = 1)
        {
            Task.Delay(TimeSpan.FromSeconds(seconds)).Wait(_cancellationToken);
        }
        private async Task SleepAsync(double seconds = 1)
        {
            await Task.Delay(TimeSpan.FromSeconds(seconds),_cancellationToken);
        }
        public IResponseParameter GetAnswerUpvoters(DominatorAccountModel account, ref int index)
        {
            var response = new ResponseParameter();

            int count = index;

            Application.Current.Dispatcher.Invoke(async () =>
            {

                await BrowserWindow.BrowserActAsync(ActType.Click, AttributeType.ClassName, "q-click-wrapper qu-display--inline-block qu-tapHighlight--white qu-cursor--pointer qu-hover--textDecoration--underline ClickWrapper___StyledClickWrapperBox-zoqi4f-0", index: 0, delayBefore: 2, delayAfter: 5);

                await BrowserWindow.BrowserActAsync(ActType.ScrollIntoView, AttributeType.ClassName, "q-box qu-color--gray_dark qu-pl--medium qu-pr--medium qu-borderBottom qu-tapHighlight--none qu-display--flex qu-alignItems--center", index: count, delayAfter: 3);

                var countResult = await BrowserWindow.ExecuteScriptAsync("document.getElementsByClassName('q-box qu-color--gray_dark qu-pl--medium qu-pr--medium qu-borderBottom qu-tapHighlight--none qu-display--flex qu-alignItems--center').length");

                count = int.Parse(countResult.Result.ToString());

                response.Response = await BrowserWindow.GetPageSourceAsync();

                isRunning = false;
            });

            while (isRunning) Sleep(2);
            index = count;

            return response;
        }

        public IResponseParameter ClickOnViewMoreComments(DominatorAccountModel account, CommentFor type)
        {
            var response = new ResponseParameter();
            switch (type)
            {
                case CommentFor.Answer:
                    Application.Current.Dispatcher.Invoke(async () =>
                    {
                        response.Response = await BrowserWindow.GetPageSourceAsync();
                        //var ViewMoreCommentsBox = Regex.Split(response.Response, "q-box qu-px--medium qu-pb--medium");
                        //var mainAttributeValueForViewMoreComments = Utilities.GetBetween(ViewMoreCommentsBox[1], "<button class=\"", "\"");

                        var mainAttributeValueForViewMoreComments = "q-click-wrapper qu-active--textDecoration--none qu-focus--textDecoration--none qu-borderRadius--pill qu-alignItems--center qu-justifyContent--center qu-whiteSpace--nowrap qu-userSelect--none qu-display--flex qu-tapHighlight--white qu-textAlign--center qu-cursor--pointer qu-hover--textDecoration--none qu-hover--bg--darken ClickWrapper___StyledClickWrapperBox-zoqi4f-0 bNPFlF base___StyledClickWrapper-lx6eke-0 kDzBrY";
                        var ViewMoreClassName = "ClickWrapper___StyledClickWrapperBox-zoqi4f-0 bNPFlF base___StyledClickWrapper-lx6eke-0 kDzBrY";
                        var Nodes = HtmlParseUtility.GetListNodesFromClassName(response.Response, ViewMoreClassName);
                        var ClickIndex = Nodes != null && Nodes.Count > 0 ? Nodes.IndexOf(Nodes.FirstOrDefault(x => x.InnerText.Contains("View more comments"))) : 0;
                        if (ClickIndex > -1)
                            await BrowserWindow.ExecuteScriptAsync($"document.getElementsByClassName('{ViewMoreClassName}')[{ClickIndex}].click();");
                        else
                            await BrowserWindow.BrowserActAsync(ActType.Click, AttributeType.ClassName, mainAttributeValueForViewMoreComments, index: 0, delayBefore: 2, delayAfter: 3);

                        response.Response = await BrowserWindow.GetPageSourceAsync();
                        isRunning = false;
                    });
                    while (isRunning) Sleep(2);
                    break;

                case CommentFor.Question:
                    Application.Current.Dispatcher.Invoke(async () =>
                    {

                        response.Response = await BrowserWindow.GetPageSourceAsync();
                        var mainAttributeValueForViewMoreComments = "q-click-wrapper qu-active--textDecoration--none qu-focus--textDecoration--none qu-borderRadius--pill qu-alignItems--center qu-justifyContent--center qu-whiteSpace--nowrap qu-userSelect--none qu-display--flex qu-tapHighlight--white qu-textAlign--center qu-cursor--pointer qu-hover--textDecoration--none qu-hover--bg--darken ClickWrapper___StyledClickWrapperBox-zoqi4f-0 bNPFlF base___StyledClickWrapper-lx6eke-0 kDzBrY";
                        var questionCommentsBox = Regex.Split(response.Response, "q-flex qu-justifyContent--space-between qu-alignItems--center qu-flexWrap--nowrap");
                        var mainAttributeValueForquestionCommentsBox = Utilities.GetBetween(questionCommentsBox[0], "div><button class=\"", "\"");
                        await BrowserWindow.BrowserActAsync(ActType.Click, AttributeType.ClassName, mainAttributeValueForquestionCommentsBox, index: 4, delayBefore: 2, delayAfter: 5);
                        response.Response = await BrowserWindow.GetPageSourceAsync();
                        var ViewMoreClassName = "ClickWrapper___StyledClickWrapperBox-zoqi4f-0 iyYUZT base___StyledClickWrapper-lx6eke-1 fJHGyh";
                        var Nodes = HtmlParseUtility.GetListNodesFromClassName(response.Response, ViewMoreClassName);
                        var ClickIndex = Nodes != null && Nodes.Count > 0 ? Nodes.IndexOf(Nodes.FirstOrDefault(x => x.InnerText.Contains("View more comments"))) : 0;
                        if (ClickIndex > -1)
                           await BrowserWindow.ExecuteScriptAsync($"document.getElementsByClassName('{ViewMoreClassName}')[{ClickIndex}].click();");
                        else
                            await BrowserWindow.BrowserActAsync(ActType.Click, AttributeType.ClassName, mainAttributeValueForViewMoreComments, index: 0, delayBefore: 2, delayAfter: 3);
                        response.Response = await BrowserWindow.GetPageSourceAsync();

                        isRunning = false;
                    });
                    while (isRunning) Sleep(2);
                    break;
            }
            
            return response;
        }

        public IResponseParameter TryAndGetResponse(string Url, string ContainString, int TryCount, DominatorAccountModel accountModel)
        {
            IResponseParameter response = new ResponseParameter();
            try
            {
                response = SearchByCustomUrl(accountModel, Url);
                while(TryCount-- >= 0 && response.Response.Contains(ContainString))
                {
                    Thread.Sleep(3000);
                    response = SearchByCustomUrl(accountModel, Url);
                }
            }
            catch { }
            return response;
        }

        public void CheckBrowser(DominatorAccountModel account)
        {
            if (BrowserWindow == null ||!BrowserWindow.IsBrowserOpenend().Result)
            {
                var value = Task.Run(async() =>
                {
                    return await BrowserLoginAsync(account, _cancellationToken);
                }).Result;
            }
        }

        public IResponseParameter CreatePost(DominatorAccountModel accountModel, string question, string media, out string finalUrl)
        {
            var response = new ResponseParameter();
            finalUrl = string.Empty;
            var isRunning = true;
            var URL = string.Empty;
            CheckBrowser(accountModel);
            Application.Current.Dispatcher.Invoke(async () =>
            {
                //Click on Create Post.
                BrowserWindow.ExecuteScript($"[...document.querySelectorAll('button[role=\"button\"]')].filter(x=>x.textContent.includes(\"Post\"))[0].click();",3);
                await Task.Delay(TimeSpan.FromSeconds(5));
                //Remove Already Pending Post.
                var XY = await BrowserWindow.GetXAndYAsync(AttributeType.ClassName, "section_remove_btn",index:0);
                if(XY.Key > 0 && XY.Value > 0)
                {
                    await BrowserWindow.MouseClickAsync(XY.Key + 3, XY.Value + 3, delayAfter: 4);
                    await BrowserWindow.MouseClickAsync(XY.Key + 3, XY.Value + 3, delayAfter: 4);
                    BrowserWindow.SelectAllText();
                    BrowserWindow.PressAnyKey(winKeyCode: 8,delayAtLast:5);
                    await Task.Delay(TimeSpan.FromSeconds(5));
                    await BrowserWindow.MouseClickAsync(XY.Key + 3, XY.Value + 3, delayAfter: 4);
                }
                else
                {
                    XY = await BrowserWindow.GetXAndYAsync(AttributeType.ClassName, "q-box editor_wrapper u-layout-direction--ltr qu-whiteSpace--pre-wrap", index:0);
                    if(XY.Key > 0 && XY.Value > 0)
                        await BrowserWindow.MouseClickAsync(XY.Key+40, XY.Value+40, delayAfter: 4);
                    BrowserWindow.SelectAllText();
                    BrowserWindow.PressAnyKey(winKeyCode: 8, delayAtLast: 5);
                }
                //Entering Question.
                if (string.IsNullOrEmpty(media))
                    await BrowserWindow.CopyPasteContentAsync(question, 86, 8, 3);
                else
                {
                    // await BrowserWindow.EnterCharsAsync(" " + question, typingDelay: .03, delayAtLast: 5);
                    await BrowserWindow.CopyPasteContentAsync(question, 86, 8, 3);
                    BrowserWindow.ChooseFileFromDialog(media);
                    var script = "document.querySelector('span[aria-label=\"Add image\"]').getBoundingClientRect().{0};";
                    //Clicking on Add Media.
                    XY = await BrowserWindow.GetXAndYAsync(customScriptX:string.Format(script,"x"),customScriptY:string.Format(script,"y"));
                    await BrowserWindow.MouseClickAsync(XY.Key + 5, XY.Value + 5, delayAfter: 4);
                }
                //Remove Fetch Error PopUp.
                var PageResponse = await BrowserWindow.GetPageSourceAsync();
                if (PageResponse != null && PageResponse.Contains(""))
                    await BrowserWindow.BrowserActAsync(ActType.Click, AttributeType.ClassName, "", delayAfter: 3);
                //Click On Post.
                await BrowserWindow.BrowserActAsync(ActType.Click, AttributeType.ClassName,"puppeteer_test_modal_submit", delayBefore: 2, delayAfter: 5);
                await Task.Delay(TimeSpan.FromSeconds(5));
                response.Response = await BrowserWindow.GetPageSourceAsync();
                URL = BrowserWindow.GetCurrentPageurl();
                isRunning = false;
            });
            while (isRunning) Sleep(2);
            finalUrl = URL;
            return response;
        }

        public async Task<bool> BrowserLoginAsync(DominatorAccountModel account, CancellationToken cancellationToken, LoginType loginType = LoginType.AutomationLogin, VerificationType verificationType = 0)
        {
            _accountScopeFactory = InstanceProvider.GetInstance<IAccountScopeFactory>();
            _cancellationToken = cancellationToken;
            sessionManager.AddOrUpdateSession(ref account);
            BrowserWindow = new PuppeteerBrowserActivity(account, isNeedResourceData: true, loginType: loginType);
            await BrowserWindow.LaunchBrowserAsync(Utilities.GetHeadLessStatus(loginType, account.AccountBaseModel.AccountNetwork));
            await SleepAsync(5);
            var last3Min = DateTime.Now.AddMinutes(2);
            while (!BrowserWindow.IsLoggedIn && last3Min >= DateTime.Now)
            {
                if (!BrowserWindow.IsLoaded)
                {
                    await SleepAsync();
                    continue;
                }
                var page = await BrowserWindow.GetPageSourceAsync();
                if (last3Min.AddMinutes(3.5) > DateTime.Now)
                    if (!BrowserWindow.IsLoggedIn && page.Contains("name=\"password\"") &&
                        page.Contains("name=\"email\""))
                    {
                        var usernameBoxXandY = await BrowserWindow.GetXAndYAsync(AttributeType.Name, "email");
                        await BrowserWindow.MouseClickAsync(usernameBoxXandY.Key + 10, usernameBoxXandY.Value + 3, delayBefore: 2, delayAfter: 1);
                        await BrowserWindow.EnterCharsAsync(account.AccountBaseModel.UserName, .009, delayAtLast: 6);

                        var passwordBoxXandY = await BrowserWindow.GetXAndYAsync(AttributeType.Name, "password");
                        await BrowserWindow.MouseClickAsync(passwordBoxXandY.Key + 10, passwordBoxXandY.Value + 7, delayBefore: 2, delayAfter: 1);
                        await BrowserWindow.EnterCharsAsync(account.AccountBaseModel.Password, .009, delayAtLast: 5);
                        page = await BrowserWindow.GetPageSourceAsync();
                        if (page.Contains("reCAPTCHA"))
                        {
                            while (!(page.Contains("What is your question or link?") || page.Contains("What do you want to ask or share?")))
                            {
                                await SleepAsync(10);
                                page = await BrowserWindow.GetPageSourceAsync();
                                cancellationToken.ThrowIfCancellationRequested();
                            }
                            //SolveCaptcha(response);
                        }
                        else
                            await BrowserWindow.ExecuteScriptAsync($"[...document.querySelectorAll('button')].filter(x=>x.innerText.trim().includes(\"Login\"))[0].click();", 5);
                        await SleepAsync(5);
                    }

                if (!BrowserWindow.IsLoggedIn)
                {
                    var result = await BrowserWindow.GetPageSourceAsync();
                    if (!string.IsNullOrEmpty(result) && (result.Contains("What is your question or link?") || result.Contains("What do you want to ask or share?")
                        || result.Contains("aria-label=\"Add question\"")))
                    {
                        //BrowserWindow.SaveCookies(false);
                        account = BrowserWindow.DominatorAccountModel;
                        BrowserWindow.IsLoggedIn = true;
                        account.Cookies = await BrowserWindow.BrowserCookiesIntoModel();
                        //QdUtilities.RemoveUnUsedCookies(ref account);
                        account.IsUserLoggedIn = true;
                        account.AccountBaseModel.Status = AccountStatus.Success;
                        SocinatorAccountBuilder.Instance(account.AccountBaseModel.AccountId)
                            .AddOrUpdateDominatorAccountBase(account.AccountBaseModel)
                            .AddOrUpdateLoginStatus(account.IsUserLoggedIn)
                            .AddOrUpdateBrowserCookies(account.Cookies)
                            .AddOrUpdateCookies(account.Cookies)
                            .SaveToBinFile();
                        sessionManager.AddOrUpdateSession(ref account, true);
                        using (var globalDbOperation = new DbOperations(SocinatorInitialize.GetGlobalDatabase().GetSqlConnection()))
                        {
                            globalDbOperation.UpdateAccountDetails(account);
                        }
                    }
                }

                await SleepAsync(2);

                if (account.IsUserLoggedIn)
                {
                    if (loginType == LoginType.InitialiseBrowser)
                        CloseBrowser(account);
                    break;
                }
            }

            return account.IsUserLoggedIn;
        }

        public UserInfoResponseHandler GetUserInfo(DominatorAccountModel account, string ProfileUrl,bool IsBrowser = true)
        {
            var response = string.Empty;
            var isRunning = true;
            CheckBrowser(account);
            Application.Current.Dispatcher.Invoke(async () =>
            {
                BrowserWindow.ClearResources();
                await BrowserWindow.GoToCustomUrl(ProfileUrl, delayAfter: 6);
                response = IsBrowser ? await BrowserWindow.GetPageSourceAsync() : await BrowserWindow.GetPaginationData("\"data\": {\"user\":", true);
                isRunning = false;
            });
            while (isRunning) Sleep(2);
            return new UserInfoResponseHandler(new ResponseParameter { Response = response }, IsBrowser);
        }

        public string GetUserProfile(DominatorAccountModel dominatorAccount)
        {
            var profileUrl = string.Empty;
            CheckBrowser(dominatorAccount);
            BrowserWindow.GoToUrl(QdConstants.HomePageUrl);
            Sleep(6);
            var pageSource = BrowserWindow.GetPageSource();
            profileUrl = Utilities.GetBetween(pageSource, "\\\"profileUrl\\\":\\\"", "\\\"");
            profileUrl = $"{QdConstants.HomePageUrl}{profileUrl}";
            return profileUrl;
        }
    }
}