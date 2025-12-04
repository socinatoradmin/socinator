using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.GdTables.Accounts;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDModel;
using Newtonsoft.Json;
using GramDominatorCore.Request;
using GramDominatorCore.GDLibrary.DAL;
using DominatorHouseCore.Process.JobLimits;
using GramDominatorCore.Factories;
using GramDominatorCore.GDLibrary.InstagramBrowser;
using ThreadUtils;
using System.Net;

namespace GramDominatorCore.GDLibrary
{
    public class AutoReplyToNewMessagesProcess : GdJobProcessInteracted<InteractedUsers>
    {
        public AutoReplyToNewMessageModel AutoReplyToNewMessageModel { get; set; }
        private int _actionBlockedCount;
        private List<UserConversation> _lstConversationUserList = new List<UserConversation>();
        public AutoReplyToNewMessagesProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IGdQueryScraperFactory queryScraperFactory, IGdHttpHelper httpHelper, IGdBrowserManager gdBrowser, IDelayService _delayService) :
            base(processScopeModel, accountServiceScoped, queryScraperFactory, httpHelper, gdBrowser, _delayService)
        {
            AutoReplyToNewMessageModel = JsonConvert.DeserializeObject<AutoReplyToNewMessageModel>(templateModel.ActivitySettings);
        }
        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            GlobusLogHelper.log.Info(Log.StartedActivity, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                DominatorAccountModel.AccountBaseModel.UserName, ActivityType, scrapeResult.ResultUser.Username);

            MessageProcessUtility processUtility = new MessageProcessUtility();
            JobProcessResult jobProcessResult = new JobProcessResult();
            InstagramUser instagramUser = (InstagramUser)scrapeResult.ResultUser;
            SendMessageIgResponseHandler response;
            string ThreadId = instagramUser?.UserDetails?.ChatID;
            try
            {
                var mangMessagesModel = GetMessageText(scrapeResult.QueryInfo);
                string message = string.Empty;
                if (mangMessagesModel.MessagesText.Contains("[Full Name]") || mangMessagesModel.MessagesText.Contains("[User Name]"))
                    message = processUtility.MakeMentionInMessage(message, instagramUser, DominatorAccountModel, ActivityType);// MakeMentionInMessage(mangMessagesModel.MessagesText, instagramUser);
                else
                    message = mangMessagesModel.MessagesText;
                if (ModuleSetting.IsChkMakeCaptionAsSpinText)
                    message = " " + SpinTexHelper.GetSpinText((string.IsNullOrEmpty(message) ? mangMessagesModel.MessagesText:message))+ " ";
                _lstConversationUserList = DbAccountService.GetConversationUser().ToList();
                if (_lstConversationUserList.Any(x => x.SenderName == instagramUser.Username))
                {
                    var UserThread = _lstConversationUserList.Where(x => x.SenderName == instagramUser.Username).ToList();
                    ThreadId = UserThread[0].ThreadId;
                }
            retry:
                if (!string.IsNullOrEmpty(mangMessagesModel.MediaPath) && !DominatorAccountModel.IsRunProcessThroughBrowser)
                    processUtility.MediaSendMessageResponse(DominatorAccountModel, AccountModel, instagramUser.UserId ?? instagramUser.Pk, instagramUser.Username, mangMessagesModel.MediaPath, instaFunct, ActivityType, JobCancellationTokenSource.Token);

                string mediaPath = mangMessagesModel.MediaPath;
                var Medias = mangMessagesModel.Medias?.Select(x => x.MediaPath)?.ToList();
                response = processUtility.SendMessageResponse(DominatorAccountModel, AccountModel, instagramUser, message, mediaPath, instaFunct, ThreadId, JobCancellationTokenSource, Medias,SkipAlreadyReceivedMessage:AutoReplyToNewMessageModel.IsSkipUserWhoReceivedMessage);

                if (response != null && response.Success)
                {
                    var Message = !string.IsNullOrEmpty(response.ErrorMessage) ? $"{scrapeResult.ResultUser.Username} With Error => {response.ErrorMessage}" : scrapeResult.ResultUser.Username;
                    GlobusLogHelper.log.Info(Log.ActivitySuccessful, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType, Message);

                    IncrementCounters();

                    processUtility.AddMessageDataToDataBase(scrapeResult, message, AccountModel, DominatorAccountModel, CampaignDbOperation, AccountDbOperation, ActivityType, campaignId, response.ThreadId, JobCancellationTokenSource);

                    jobProcessResult.IsProcessSuceessfull = true;
                }
                else if (!response.Success && response.Issue != null && response.Issue.Message == "You must write ContentLength bytes to the request stream before calling [Begin]GetResponse.")
                {
                    delayservice.ThreadSleep(TimeSpan.FromSeconds(5));
                    goto retry;
                }
                else if (response.ToString().Contains("This block will expire on"))
                {
                    string expireDate = Utilities.GetBetween(response.ToString(), "This block will expire on", ".");
                    GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType, $" action has been blocked.This block will expire on {expireDate}");
                    Stop();
                    JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                }
                else if (response.ToString().Contains("Action Blocked") && response.ToString().Contains("\"feedback_required\""))
                {
                    if (string.IsNullOrEmpty(ThreadId))
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType, $"Your account action has been blocked for sending message to new User.please try later ");
                        Stop();
                        JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    }
                    delayservice.ThreadSleep(TimeSpan.FromSeconds(30));
                    bool LoginStatus = false;
                    var BackupCookie = DominatorAccountModel.Cookies;
                    var logOutStatus = instaFunct.Logout(DominatorAccountModel, AccountModel);
                    if (logOutStatus.Success)
                    {
                        ResetCookies(BackupCookie);
                        LoginStatus = loginProcess.LoginWithAlternativeMethodForBlocking(DominatorAccountModel);
                    }
                    if (LoginStatus)
                    {
                        delayservice.ThreadSleep(TimeSpan.FromSeconds(10));
                        if (!string.IsNullOrEmpty(mangMessagesModel.MediaPath))
                            processUtility.MediaSendMessageResponse(DominatorAccountModel, AccountModel, instagramUser.UserId ?? instagramUser.Pk, instagramUser.Username, mangMessagesModel.MediaPath, instaFunct, ActivityType, JobCancellationTokenSource.Token);

                        response = processUtility.SendMessageResponse(DominatorAccountModel, AccountModel, instagramUser, message, mediaPath, instaFunct, ThreadId, JobCancellationTokenSource, Medias,SkipAlreadyReceivedMessage:AutoReplyToNewMessageModel.IsSkipUserWhoReceivedMessage);

                        if (response != null && response.Success)
                        {
                            var Message = !string.IsNullOrEmpty(response.ErrorMessage) ?$"{scrapeResult.ResultUser.Username} ==> With Error {response.ErrorMessage}" : scrapeResult.ResultUser.Username;
                            GlobusLogHelper.log.Info(Log.ActivitySuccessful, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                DominatorAccountModel.AccountBaseModel.UserName, ActivityType, Message);

                            IncrementCounters();

                            processUtility.AddMessageDataToDataBase(scrapeResult, message, AccountModel, DominatorAccountModel, CampaignDbOperation, AccountDbOperation, ActivityType, campaignId, response.ThreadId, JobCancellationTokenSource);

                            jobProcessResult.IsProcessSuceessfull = true;
                        }
                        else
                        {
                            if (!CheckResponse.CheckProcessResponse(response, DominatorAccountModel, ActivityType, scrapeResult, ref _actionBlockedCount))
                            {
                                Stop();
                                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                            }
                            jobProcessResult.IsProcessSuceessfull = false;
                        }
                    }
                }
                else
                {
                    if (!CheckResponse.CheckProcessResponse(response, DominatorAccountModel, ActivityType, scrapeResult, ref _actionBlockedCount))
                    {
                        Stop();
                        JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    }
                    jobProcessResult.IsProcessSuceessfull = false;
                }
                // Delay between each activity
                DelayBeforeNextActivity();
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return jobProcessResult;
        }

        private ManageMessagesModel GetMessageText(QueryInfo queryInfo)
        {
            var getManageMessagesModels = new List<ManageMessagesModel>();
            string caption = Regex.Split(queryInfo.QueryTypeDisplayName, "<:>")[1];

            if (AutoReplyToNewMessageModel.IsReplyToMessagesThatContainSpecificWord﻿Checked)
            {
                List<string> lstContainingSpecificWord = AutoReplyToNewMessageModel.LstMessage
                    .Where(x => caption.ToLower().Contains(x.ToLower())).ToList();

                lstContainingSpecificWord.ForEach(word =>
                {
                    getManageMessagesModels.AddRange(AutoReplyToNewMessageModel.LstDisplayManageMessageModel.Where(x =>
                        x.SelectedQuery.FirstOrDefault(y => y.Content.QueryValue == word) != null));
                });
            }
            else
            {
                getManageMessagesModels = AutoReplyToNewMessageModel.LstDisplayManageMessageModel.Where(x => x.SelectedQuery.Count == 0).ToList();
            }

            return getManageMessagesModels.GetRandomItem();
        }
        public override void StartOtherConfiguration(ScrapeResultNew scrapeResult)
        {

        }
        public void ResetCookies(CookieCollection Cookies)
        {
            DominatorAccountModel.Cookies = new CookieCollection();
            foreach (Cookie cookie in Cookies)
            {
                var cookieHelper = new CookieHelper();
                cookieHelper.Name = cookie.Name;
                cookieHelper.Value = cookie.Value;
                cookieHelper.Domain = cookie.Domain;
                cookieHelper.Expires = cookie.Expires;
                cookieHelper.HttpOnly = cookie.HttpOnly;
                cookieHelper.Secure = cookie.Secure;

                if (cookie.Name.Contains("mid") || cookie.Name.Contains("csrftoken") || cookie.Name.Contains("sessionid") || cookie.Name.Contains("ds_user_id")
                    || cookie.Name.Contains("rur") || cookie.Name.Contains("ds_user") || cookie.Name.Contains("igfl"))
                {
                    DominatorAccountModel.CookieHelperList.Add(cookieHelper);
                    DominatorAccountModel.Cookies.Add(cookie);
                }

            }
        }

    }
}
