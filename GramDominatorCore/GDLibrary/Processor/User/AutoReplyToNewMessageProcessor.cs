using ThreadUtils;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDFactories;
using GramDominatorCore.GDLibrary.DAL;
using GramDominatorCore.GDLibrary.InstagramBrowser;
using GramDominatorCore.GDModel;
using GramDominatorCore.Response;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using GramDominatorCore.Utility;

namespace GramDominatorCore.GDLibrary.Processor.User
{
    public class AutoReplyToNewMessageProcessor : BaseInstagramUserProcessor
    {
        AutoReplyToNewMessageModel AutoReplyToNewMessageModel { get; }
        public AutoReplyToNewMessageProcessor(IGdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService, IDbCampaignService campaignService, IProcessScopeModel processScopeModel, IDelayService _delayService, IGdBrowserManager gdBrowserManager) :
            base(jobProcess, dbAccountService, campaignService, processScopeModel, _delayService,gdBrowserManager)
        {
            AutoReplyToNewMessageModel = JsonConvert.DeserializeObject<AutoReplyToNewMessageModel>(TemplateModel.ActivitySettings);
        }
        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            try
            {
                Token.ThrowIfCancellationRequested();
                var lstInstaUsers = new List<InstagramUser>();
                var lstSpecificWord = new List<string>();
                V2InboxResponse v2InboxResponse;
                //SenderDetails senderDetail;
                var lstSenderDetails = new List<SenderDetails>();
                var browser = GramStatic.IsBrowser;
                #region Get Users to message from respected input source type
                try
                {
                    //do
                    {
                        v2InboxResponse = 
                            browser ?
                            InstaFunction.GdBrowserManager.GetInbox(DominatorAccountModel, AutoReplyToNewMessageModel.IsReplyToPendingMessages﻿﻿Checked, Token, true)
                            : InstaFunction.Getv2Inbox(DominatorAccountModel, AutoReplyToNewMessageModel.IsReplyToPendingMessages﻿﻿Checked, jobProcessResult.maxId).Result;
                        if (!CheckingLoginRequiredResponse(v2InboxResponse?.ToString(), "", queryInfo))
                            return;

                        if (v2InboxResponse.Success)
                        {
                            if (AutoReplyToNewMessageModel.IsReplyToPendingMessages﻿﻿Checked)
                            {
                                if (!v2InboxResponse.HasPendingRequests)
                                {
                                    GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.UserName, ActivityType, "No pending requests found to perform operation.");
                                    JobProcess.Stop();
                                    Token.ThrowIfCancellationRequested();
                                }
                                v2InboxResponse.LstInviterDetails.ForEach(pendingUsers => lstInstaUsers.Add(pendingUsers));
                                v2InboxResponse.LstSenderDetails.ForEach(pendiUsers => lstSenderDetails.Add(pendiUsers));
                            }
                            else if (AutoReplyToNewMessageModel.IsReplyToAllMessagesChecked)
                            {
                                v2InboxResponse.LstSenderDetails.ForEach(lastSender =>
                                {
                                    if (lastSender.LastMessageOwnerId != DominatorAccountModel.AccountBaseModel.UserId)
                                    {
                                        lstSenderDetails.Add(lastSender);
                                    }
                                });
                            }
                            jobProcessResult.maxId = v2InboxResponse.CursorId;
                            Token.ThrowIfCancellationRequested();
                        }
                        else
                        {
                            jobProcessResult.maxId = null;
                            jobProcessResult.HasNoResult = true;
                        }
                        DelayService.ThreadSleep(TimeSpan.FromSeconds(1));
                        foreach (var senderDetails in lstSenderDetails)
                        {
                            InstagramUser userInfo =
                                browser ?
                                InstaFunction.GdBrowserManager.GetUserInfo(DominatorAccountModel, senderDetails.SenderName, Token)
                                : InstaFunction.SearchUsername(DominatorAccountModel, senderDetails.SenderName, Token);
                            if (userInfo != null)
                            {
                                if (userInfo?.UserDetails is null)
                                    userInfo.UserDetails = new InstagramUserDetails();
                                userInfo.UserDetails.ChatID = senderDetails.ThreadId;
                            }
                            lstInstaUsers.Add(userInfo);
                            DelayService.ThreadSleep(1000);
                        }

                    } //while (!jobProcessResult.IsProcessCompleted && !string.IsNullOrEmpty(jobProcessResult.maxId));
                }
                catch (Exception)
                {
                }

                #endregion

                if (AutoReplyToNewMessageModel.IsReplyToMessagesThatContainSpecificWord﻿Checked)
                {
                    if (lstSpecificWord.Count == 0)
                        lstSpecificWord = AutoReplyToNewMessageModel.LstMessage;
                }

                if (ModuleSetting.IsSkipUserWhoReceivedMessage)
                {
                    LstInteractedUsers = DbAccountService.GetInteractedUsersMessageData().ToList();
                    lstInstaUsers.RemoveAll(x => x == null);
                    lstInstaUsers.RemoveAll(x => LstInteractedUsers.Any(y => y.InteractedUsername == x.Username));
                }

                foreach (var instaUser in lstInstaUsers)
                {
                    Token.ThrowIfCancellationRequested();

                    #region Filter messages by specific words
                    if (instaUser == null)
                        continue;
                    SenderDetails senderDetail = lstSenderDetails.FirstOrDefault(detail => detail.SenderName == instaUser.Username);
                    if (AutoReplyToNewMessageModel.IsReplyToMessagesThatContainSpecificWord﻿Checked)
                    {
                        if (lstSpecificWord.All(word => senderDetail != null && (string.IsNullOrEmpty(senderDetail.LastMesseges) || !senderDetail.LastMesseges.ToLower().Contains(word.Trim().ToLower()))))
                            continue;
                    }
                    #endregion
                    if (senderDetail != null)
                    {
                        string essentialSenderDetails = (AutoReplyToNewMessageModel.IsReplyToPendingMessages﻿﻿Checked
                                                            ? "true" : "false") + "<:>" + senderDetail.LastMesseges + "<:>" + senderDetail.ThreadId;
                        FilterAndStartFinalProcessForOneUser(new QueryInfo() { QueryTypeDisplayName = essentialSenderDetails }, ref jobProcessResult, instaUser);
                    }
                }
            }
            catch (OperationCanceledException )
            {
                throw new OperationCanceledException();
            }
            catch (AggregateException e)
            {
                foreach (Exception ex in e.InnerExceptions)
                    Console.WriteLine(ex.Message);
            }
            catch (Exception)
            {
                
            }
            finally { jobProcessResult.IsProcessCompleted = true; }
        }
    }
}
