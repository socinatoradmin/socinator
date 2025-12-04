using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using Newtonsoft.Json;
using RedditDominatorCore.RDLibrary.BrowserManager;
using RedditDominatorCore.RDLibrary.DAL;
using RedditDominatorCore.RDModel;
using RedditDominatorCore.RDUtility;
using RedditDominatorCore.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using ThreadUtils;
using Unity;

namespace RedditDominatorCore.RDLibrary.Processors.User
{
    internal abstract class BaseRedditUserProcessor : BaseRedditProcessor
    {
        private readonly IAccountScopeFactory _accountScopeFactory;
        private IRdBrowserManager _browserManager;
        private readonly IDbAccountService _dbAccountService;
        private readonly IDelayService _delayService;
        protected BaseRedditUserProcessor(IRdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService,
            IDbGlobalService globalService,
            IDbCampaignService campaignService, IRedditFunction redditFunction, IRdBrowserManager browserManager)
            : base(jobProcess, dbAccountService, campaignService, redditFunction, browserManager)
        {
            _dbAccountService = dbAccountService;
            _browserManager = browserManager;
            _accountScopeFactory = InstanceProvider.GetInstance<IAccountScopeFactory>();
            _delayService = InstanceProvider.GetInstance<IDelayService>();
        }

        public void StartCustomUserProcess(QueryInfo queryInfo, ref JobProcessResult jobProcessResult,
            RedditUser redditUser)
        {
            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            var newUserFilter = new ScrapeFilter.User(JobProcess.ModuleSetting);

            if (newUserFilter.IsFilterApplied() && !newUserFilter.AppplyFilters(redditUser))
            {
                GlobusLogHelper.log.Info(Log.CustomMessage, JobProcess.SocialNetworks, JobProcess.DominatorAccountModel.AccountBaseModel.UserName, JobProcess.ActivityType, $"Filtered not matched for {redditUser.Username}");
                return;
            }
            redditUser.CookieCollection = JobProcess.DominatorAccountModel.Cookies;
            if (ActivityType == ActivityType.Follow && string.IsNullOrEmpty(redditUser.Username))
            {
                _delayService.ThreadSleep(1000);
                GlobusLogHelper.log.Info(Log.NoMoreDataToPerform, JobProcess.SocialNetworks,
                    JobProcess.DominatorAccountModel.AccountBaseModel.UserName, JobProcess.ActivityType);
                return;
            }
            var InteractedUsers = _dbAccountService.GetInteractedUsers(JobProcess.ActivityType);
            if (InteractedUsers != null && InteractedUsers.Any(x => x.InteractedUsername == redditUser.Username))
            {
                GlobusLogHelper.log.Info(Log.CustomMessage, JobProcess.SocialNetworks, JobProcess.DominatorAccountModel.AccountBaseModel.UserName, JobProcess.ActivityType, $"Skipped User {redditUser.Username} As Already Interacted User.");
                return;
            }
            StartFinalUserProcess(ref jobProcessResult, redditUser, queryInfo);
        }


        public void StartKeywordUserProcess(QueryInfo queryInfo, ref JobProcessResult jobProcessResult,
            List<RedditUser> lstNewRedditUsers)
        {
            try
            {
                foreach (var user in lstNewRedditUsers)
                {
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    var objUser = new ScrapeFilter.User(JobProcess.ModuleSetting);
                    if (AlreadyInteractedUser(user.Username)) continue;
                    if (!CheckUserUniqueNess(jobProcessResult, user.Username, ActivityType)) continue;

                    if (objUser.IsFilterApplied() && !objUser.AppplyFilters(user))
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage, JobProcess.SocialNetworks, JobProcess.DominatorAccountModel.AccountBaseModel.UserName, JobProcess.ActivityType, $"Filtered not matched for {user.Username}");
                        continue;
                    }
                    StartFinalUserProcess(ref jobProcessResult, user, queryInfo);
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
            finally { if (_browserManager != null && _browserManager.BrowserWindow != null) _browserManager.CloseBrowser(); }
        }

        public void StartDataOfUsersWhoCommentedProcess(QueryInfo queryInfo, ref JobProcessResult jobProcessResult,
            List<string> commentedUsersList)
        {
            foreach (var user in commentedUsersList)
                try
                {
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    var objUser = new ScrapeFilter.User(JobProcess.ModuleSetting);
                    if (AlreadyInteractedUser(user)) continue;
                    if (!CheckUserUniqueNess(jobProcessResult, user, ActivityType)) continue;

                    var redditUser = new RedditUser();
                    if (!JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser)
                    {
                        redditUser = RedditFunction.GetUserDetailsByUsername(JobProcess.DominatorAccountModel, user)
                            .RedditUser;
                    }
                    //For browser automation
                    else
                    {
                        var url = string.Empty;
                        if (!user.Contains("https://www.reddit.com/user/")) url = $"https://www.reddit.com/user/{user}";
                        var response = _browserManager.TryAndGetResponse(JobProcess.DominatorAccountModel, url, 3, string.Empty, true);
                        var userResponseHandler = new UserResponseHandler(response);
                        redditUser = userResponseHandler.RedditUser;

                        //To accept permission to view adult content
                        if (response != null && (response.Response.Contains("You must be 18+ to view this community")
                                                 || response.Response.Contains("You must be at least eighteen years old to view this content. Are you over eighteen and willing to see adult content?")))
                            AllowPermissionForAdultPageInBrowser(response);
                    }
                    if (objUser.IsFilterApplied() && !objUser.AppplyFilters(redditUser))
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage, JobProcess.SocialNetworks, JobProcess.DominatorAccountModel.AccountBaseModel.UserName, JobProcess.ActivityType, $"Filtered not matched for {redditUser.Username}");
                        continue;
                    }
                    StartFinalUserProcess(ref jobProcessResult, redditUser, queryInfo);
                }
                catch (OperationCanceledException)
                {
                    throw new OperationCanceledException();
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
                finally { if (_browserManager != null && _browserManager.BrowserWindow != null) _browserManager.CloseBrowser(); }
        }

        public void StartMatchLastMessageAndAutoReplyProcess(QueryInfo queryInfo, ref JobProcessResult jobProcessResult,
            List<ConversationDetails> userChatUrls)
        {
            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            var templateFileManager = InstanceProvider.GetInstance<ITemplatesFileManager>();
            var templateModel = templateFileManager.GetTemplateById(JobProcess.TemplateId);
            var autoReplyModel = JsonConvert.DeserializeObject<AutoReplyModel>(templateModel.ActivitySettings);
            List<string> specificWords = new List<string>();
            if (autoReplyModel.SpecificWord.Contains(","))
                specificWords = autoReplyModel.SpecificWord.Split(',').ToList();
            else
                specificWords.Add(autoReplyModel.SpecificWord.ToString());
            var message = autoReplyModel.Message.ToString();
            userChatUrls.RemoveAll(x => x.username == JobProcess.DominatorAccountModel.AccountBaseModel.UserName || string.IsNullOrEmpty(x.username));
            userChatUrls.RemoveAll(z => z.Messages.Count == 0);
            userChatUrls.RemoveAll(y => !Utils.IsMutualWordPresent(specificWords, y.Messages.LastOrDefault().Message));
            foreach (var chatUrl in userChatUrls)
                try
                {
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    if (!string.IsNullOrEmpty(chatUrl.username))
                    {
                        JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                        var userDetails = RedditFunction.GetUserDetailsByUsername(JobProcess.DominatorAccountModel, chatUrl.username);
                        userDetails.RedditUser.Text = autoReplyModel.IsSpintax ? SpinTexHelper.GetSpinText(message) : message.ToString();
                        userDetails.RedditUser.Url = string.IsNullOrEmpty(userDetails.RedditUser.Url) ? chatUrl.ProfileUrl : userDetails.RedditUser.Url;
                        userDetails.RedditUser.IsPending = chatUrl.Messages.LastOrDefault().IsPending;
                        userDetails.RedditUser.ThreadID = chatUrl.RoomID;
                        JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                        StartFinalUserProcess(ref jobProcessResult, userDetails.RedditUser, queryInfo);
                    }
                    else
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork, JobProcess.DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                                $"Not suitable To AutoReplyMessage for User URL : {chatUrl.ProfileUrl}");
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
                finally { if (_browserManager != null && _browserManager.BrowserWindow != null) _browserManager.CloseBrowser(); }
        }
        protected void StartFinalUserProcess(ref JobProcessResult jobProcessResult, RedditUser redditUser,
            QueryInfo queryInfo)
        {
            jobProcessResult = JobProcess.FinalProcess(new ScrapeResultNew
            {
                ResultUser = redditUser,
                QueryInfo = queryInfo
            });
        }
    }
}