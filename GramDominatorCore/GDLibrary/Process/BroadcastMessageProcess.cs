using CommonServiceLocator;
using ThreadUtils;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.GdTables.Accounts;
using DominatorHouseCore.Interfaces;
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
    public class BroadcastMessageProcess : GdJobProcessInteracted<InteractedUsers>
    {
        public BroadcastMessagesModel BroadcastMessagesModel { get; set; }

        private readonly Queue<ManageMessagesModel> _queManageMessagesModel = new Queue<ManageMessagesModel>();

        private static readonly object LockUniqueUser = new object();

        private QueryInfo _lastUsedQueryInfo;

        private int _actionBlockedCount;

        public BroadcastMessageProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IGdQueryScraperFactory queryScraperFactory, IGdHttpHelper httpHelper, IGdBrowserManager gdBrowser, IDelayService _delayService) :
            base(processScopeModel, accountServiceScoped, queryScraperFactory, httpHelper, gdBrowser, _delayService)
        {
            BroadcastMessagesModel = JsonConvert.DeserializeObject<BroadcastMessagesModel>(templateModel.ActivitySettings);
        }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            GlobusLogHelper.log.Info(Log.StartedActivity, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                DominatorAccountModel.AccountBaseModel.UserName, ActivityType, scrapeResult.ResultUser.Username);

            JobProcessResult jobProcessResult = new JobProcessResult();
            MessageProcessUtility processUtility = new MessageProcessUtility();
            int delay = ModuleSetting.DelayBetweenEachActionBlock.GetRandom();
            
            try
            {
                ManageMessagesModel manageMessagesModel = GetMessageModel(scrapeResult.QueryInfo);
                //System.Threading.Thread.Sleep(TimeSpan.FromMinutes(5));

                #region Declaration with initialization
                InstagramUser instagramUser = (InstagramUser)scrapeResult.ResultUser;
                string ThreadId = instagramUser?.UserDetails?.ChatID ?? string.Empty;
                string message;
                List<UserConversation> lstConversationUserList = DbAccountService.GetConversationUser().ToList();
                if (lstConversationUserList.Any(x => x.SenderName == instagramUser.Username))
                {
                    var UserThread = lstConversationUserList.Where(x => x.SenderName == instagramUser.Username).ToList();
                    ThreadId = UserThread[0].ThreadId;
                }

                if (ModuleSetting.IsSendMessageToUniqueUserFromAllAccount)
                {
                    if (!CheckUniqueUserFromAllAccount(scrapeResult, jobProcessResult, instagramUser))
                        return jobProcessResult;
                }
                if (manageMessagesModel.MessagesText.Contains("[Full Name]") || manageMessagesModel.MessagesText.Contains("[User Name]"))
                    message = processUtility.MakeMentionInMessage(manageMessagesModel.MessagesText, instagramUser, DominatorAccountModel, ActivityType);//MakeMentionInMessage(manageMessagesModel.MessagesText, instagramUser);                
                else
                    message = manageMessagesModel.MessagesText;

                if (ModuleSetting.IsChkMakeCaptionAsSpinText)
                    message = " " + SpinTexHelper.GetSpinText(message) + " ";

                SendMessageIgResponseHandler sendPhotoResponse = null;
                SendMessageIgResponseHandler sendTextOrLinkMessageResponse = null;

                bool sendDirectMessageStatus = false;
            #endregion
            if (!string.IsNullOrEmpty(manageMessagesModel.MediaPath)&& !DominatorAccountModel.IsRunProcessThroughBrowser)
                    sendPhotoResponse = processUtility.MediaSendMessageResponse(DominatorAccountModel, AccountModel, instagramUser.UserId ?? instagramUser.Pk,instagramUser.Username, manageMessagesModel.MediaPath, instaFunct,ActivityType, JobCancellationTokenSource.Token);

                string mediaPath = manageMessagesModel.MediaPath;
                var Medias = manageMessagesModel.Medias?.Select(x => x.MediaPath)?.ToList();
                //   var messageBoxInfo=instaFunct.PersistentBadging(DominatorAccountModel,AccountModel);
                sendTextOrLinkMessageResponse = processUtility.SendMessageResponse(DominatorAccountModel, AccountModel, instagramUser, message, mediaPath, instaFunct, ThreadId, JobCancellationTokenSource, Medias,SkipAlreadyReceivedMessage:BroadcastMessagesModel.IsSkipUserWhoReceivedMessage);

                sendDirectMessageStatus = (sendPhotoResponse?.Success ?? false) || (sendTextOrLinkMessageResponse?.Success ?? false);

                if (sendDirectMessageStatus)
                {
                    GlobusLogHelper.log.Info(Log.ActivitySuccessful, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType, scrapeResult.ResultUser.Username);
                    IncrementCounters();
                    processUtility.AddMessageDataToDataBase(scrapeResult, message, AccountModel, DominatorAccountModel, CampaignDbOperation, AccountDbOperation, ActivityType, campaignId, sendTextOrLinkMessageResponse.ThreadId, JobCancellationTokenSource);
                    jobProcessResult.IsProcessSuceessfull = true;
                }
                else
                {
                    if (sendTextOrLinkMessageResponse == null)
                        return jobProcessResult;
                    if (!sendTextOrLinkMessageResponse.Success && sendTextOrLinkMessageResponse.Issue != null && sendTextOrLinkMessageResponse.Issue.Message == "You must write ContentLength bytes to the request stream before calling [Begin]GetResponse.")
                    {
                        delayservice.ThreadSleep(TimeSpan.FromSeconds(5));
                    }
                    if (sendTextOrLinkMessageResponse.ToString().Contains("This block will expire on"))
                    {
                        string expireDate = Utilities.GetBetween(sendTextOrLinkMessageResponse.ToString(), "This block will expire on", ".");
                        GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType, $" action has been blocked.This block will expire on {expireDate}");
                        Stop();
                        JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    }
                    else if (sendTextOrLinkMessageResponse.ToString().Contains("Action Blocked") && sendTextOrLinkMessageResponse.ToString().Contains("\"feedback_required\""))
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
                            if (!string.IsNullOrEmpty(manageMessagesModel.MediaPath))
                                sendPhotoResponse = processUtility.MediaSendMessageResponse(DominatorAccountModel, AccountModel, instagramUser.UserId ?? instagramUser.Pk,instagramUser.Username, manageMessagesModel.MediaPath, instaFunct,ActivityType, JobCancellationTokenSource.Token);

                            string mediaPath1 = manageMessagesModel.MediaPath;
                            sendTextOrLinkMessageResponse = processUtility.SendMessageResponse(DominatorAccountModel, AccountModel, instagramUser, message, mediaPath1, instaFunct, ThreadId, JobCancellationTokenSource, Medias,SkipAlreadyReceivedMessage:BroadcastMessagesModel.IsSkipUserWhoReceivedMessage);

                            sendDirectMessageStatus = (sendPhotoResponse?.Success ?? false) || (sendTextOrLinkMessageResponse?.Success ?? false);
                            if (sendDirectMessageStatus)
                            {
                                GlobusLogHelper.log.Info(Log.ActivitySuccessful, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType, scrapeResult.ResultUser.Username);
                                IncrementCounters();
                                processUtility.AddMessageDataToDataBase(scrapeResult, message, AccountModel, DominatorAccountModel, CampaignDbOperation, AccountDbOperation, ActivityType, campaignId, sendTextOrLinkMessageResponse.ThreadId, JobCancellationTokenSource);
                                jobProcessResult.IsProcessSuceessfull = true;
                            }
                            else
                            {
                                if (!CheckResponse.CheckProcessResponse(sendTextOrLinkMessageResponse, DominatorAccountModel, ActivityType, scrapeResult, ref _actionBlockedCount, delay))
                                {
                                    Stop();
                                    JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                                }
                            }
                        }
                    }
                    else if (!string.IsNullOrEmpty(sendTextOrLinkMessageResponse.ErrorMessage))
                    {
                        GlobusLogHelper.log.Info(Log.ActivityFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType, $" ==> {sendTextOrLinkMessageResponse.ErrorMessage}");
                        if(sendTextOrLinkMessageResponse.ErrorMessage.Contains("There is a limit to the number of new conversations"))
                        {
                            Stop();
                            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                        }
                    }
                    else
                    {
                        if (!CheckResponse.CheckProcessResponse(sendTextOrLinkMessageResponse, DominatorAccountModel, ActivityType, scrapeResult, ref _actionBlockedCount, delay))
                        {
                            Stop();
                            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                        }
                    }
                    if(scrapeResult.QueryInfo.QueryType == "Custom Users List")
                    {
                        jobProcessResult.IsProcessCompleted = true;
                        jobProcessResult.HasNoResult = true;
                    }
                    

                    jobProcessResult.IsProcessSuceessfull = false;
                }
                DelayBeforeNextActivity();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return jobProcessResult;
        }

        private ManageMessagesModel GetMessageModel(QueryInfo queryInfo)
        {
            if (_lastUsedQueryInfo == null)
                _lastUsedQueryInfo = queryInfo;
            else
            {
                if (queryInfo.QueryType != _lastUsedQueryInfo.QueryType ||
                    queryInfo.QueryValue != _lastUsedQueryInfo.QueryValue)
                {
                    _queManageMessagesModel.Clear();
                    _lastUsedQueryInfo = queryInfo;
                }
            }

            if (_queManageMessagesModel.Count == 0)
            {
                var getManageMessagesModels = BroadcastMessagesModel.LstDisplayManageMessageModel.Where(x =>
                    x.SelectedQuery.FirstOrDefault(y =>
                        (y.Content.QueryType == queryInfo.QueryType) &&
                        (y.Content.QueryValue == queryInfo.QueryValue)) != null).ToList();

                getManageMessagesModels.ForEach(x => _queManageMessagesModel.Enqueue(x));
            }
            // if(_queManageMessagesModel.Count!=0 )
            var manageMessagesModel = _queManageMessagesModel.Dequeue();
            _queManageMessagesModel.Enqueue(manageMessagesModel);

            return manageMessagesModel;
        }
        public override void StartOtherConfiguration(ScrapeResultNew scrapeResult)
        {

        }

        public bool CheckUniqueUserFromAllAccount(ScrapeResultNew scrapeResult, JobProcessResult jobProcessResult, InstagramUser instagramUser)
        {
            lock (LockUniqueUser)
            {
                try
                {
                    var instance = InstanceProvider.GetInstance<ICampaignInteractionDetails>();
                    instance.AddInteractedData(SocialNetworks, $"{CampaignId}.SendMessage", instagramUser.Username);
                }
                catch (Exception)
                {
                    jobProcessResult.IsProcessSuceessfull = false;
                    return false;
                }
            }
            return true;
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
