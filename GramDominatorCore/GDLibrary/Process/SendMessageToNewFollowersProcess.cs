using ThreadUtils;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.GdTables.Accounts;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using GramDominatorCore.Factories;
using GramDominatorCore.GDLibrary.DAL;
using GramDominatorCore.GDLibrary.InstagramBrowser;
using GramDominatorCore.GDModel;
using GramDominatorCore.Request;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace GramDominatorCore.GDLibrary
{
    public class SendMessageToNewFollowersProcess : GdJobProcessInteracted<InteractedUsers>
    {
        private SendMessageToFollowerModel SendMessageToFollowerModel { get; }

        private int _actionBlockedCount;
        // private readonly Queue<ManageMessagesModel> _queManageMessagesModel  = new Queue<ManageMessagesModel>();

        public SendMessageToNewFollowersProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IGdQueryScraperFactory queryScraperFactory, IGdHttpHelper httpHelper, IGdBrowserManager gdBrowser, IDelayService _delayService) :
            base(processScopeModel, accountServiceScoped, queryScraperFactory, httpHelper, gdBrowser, _delayService)
        {
            SendMessageToFollowerModel = JsonConvert.DeserializeObject<SendMessageToFollowerModel>(templateModel.ActivitySettings);
        }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            GlobusLogHelper.log.Info(Log.StartedActivity, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                DominatorAccountModel.AccountBaseModel.UserName, ActivityType, scrapeResult.ResultUser.Username);

            JobProcessResult jobProcessResult = new JobProcessResult();
            MessageProcessUtility processUtility = new MessageProcessUtility();
            SendMessageIgResponseHandler response = null;
            InstagramUser instagramUser = (InstagramUser)scrapeResult.ResultUser;
            string ThreadId = string.Empty;
            string message = SendMessageToFollowerModel.TextMessage;
            string mediaPath = SendMessageToFollowerModel.MediaPath;
            var medias = SendMessageToFollowerModel.Medias;
            if (message.Contains("[Full Name]") || message.Contains("[User Name]"))
                message = processUtility.MakeMentionInMessage(message, instagramUser, DominatorAccountModel, ActivityType);//MakeMentionInMessage(message, instagramUser);

            if (ModuleSetting.IsChkMakeCaptionAsSpinText)
                message = " " + SpinTexHelper.GetSpinText(message) + " ";

            List<UserConversation> lstConversationUserList = DbAccountService.GetConversationUser().ToList();
            if (lstConversationUserList.Any(x => x.SenderName == instagramUser.Username))
            {
                var UserThread = lstConversationUserList.Where(x => x.SenderName == instagramUser.Username).ToList();
                ThreadId = UserThread[0].ThreadId;
            }

        retry:
            if (!string.IsNullOrEmpty(mediaPath) && !DominatorAccountModel.IsRunProcessThroughBrowser)
            {
                processUtility.MediaSendMessageResponse(DominatorAccountModel, AccountModel, instagramUser.UserId ?? instagramUser.Pk, instagramUser.Username, mediaPath, instaFunct,ActivityType, JobCancellationTokenSource.Token);
            }
                

            try
            {
                response = processUtility.SendMessageResponse(DominatorAccountModel, AccountModel, instagramUser, message, mediaPath, instaFunct, ThreadId, JobCancellationTokenSource,SkipAlreadyReceivedMessage:SendMessageToFollowerModel.IsSkipUserWhoReceivedMessage
                    ,Medias:medias?.Select(x=>x?.MediaPath)?.ToList());
                if (response != null && response.Success)
                {
                    GlobusLogHelper.log.Info(Log.ActivitySuccessful, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType, scrapeResult.ResultUser.Username);
                    IncrementCounters();

                    processUtility.AddMessageDataToDataBase(scrapeResult, message, AccountModel, DominatorAccountModel, CampaignDbOperation, AccountDbOperation, ActivityType, campaignId, response.ThreadId, JobCancellationTokenSource);

                    jobProcessResult.IsProcessSuceessfull = true;
                }else if(response?.ErrorMessage == "Already Received Message")
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, 
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType, $"Failed To {ActivityType} With Error ==> {instagramUser?.Username} {response?.ErrorMessage}");
                    jobProcessResult.IsProcessSuceessfull = false;
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
                        if (!string.IsNullOrEmpty(mediaPath))
                            processUtility.MediaSendMessageResponse(DominatorAccountModel, AccountModel, instagramUser.UserId ?? instagramUser.Pk,instagramUser.Username, mediaPath, instaFunct,ActivityType, JobCancellationTokenSource.Token);

                        response = processUtility.SendMessageResponse(DominatorAccountModel, AccountModel, instagramUser, message, mediaPath, instaFunct, ThreadId, JobCancellationTokenSource,SkipAlreadyReceivedMessage:SendMessageToFollowerModel.IsSkipUserWhoReceivedMessage);

                        if (response != null && response.Success)
                        {
                            GlobusLogHelper.log.Info(Log.ActivitySuccessful, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                DominatorAccountModel.AccountBaseModel.UserName, ActivityType, scrapeResult.ResultUser.Username);

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
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return jobProcessResult;
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
