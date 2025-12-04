using System;
using System.Collections.Generic;
using DominatorHouseCore;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using QuoraDominatorCore.Models;
using QuoraDominatorCore.QdLibrary.DAL;
using QuoraDominatorCore.QdLibrary.QdFunctions;
using QuoraDominatorCore.QdUtility;
using QuoraDominatorCore.Request;
using QuoraDominatorCore.Response;

namespace QuoraDominatorCore.QdLibrary.Processors.Message
{
    public class AutoReplyToNewMessageProcessor : BaseQuoraProcessor
    {
        private readonly IQdHttpHelper _httpHelper;
        private AutoReplyToNewMessageModel ActivitySetting;
        public AutoReplyToNewMessageProcessor(IQuoraBrowserManager browser, IQdJobProcess jobProcess,
            IDbAccountServiceScoped dbAccountService,
            IDbGlobalService globalService, IDbCampaignService campaignService, IQuoraFunctions objQuoraFunct,
            IQdHttpHelper httpHelper, IProcessScopeModel processScopeModel) :
            base(browser, jobProcess, dbAccountService, globalService, campaignService, objQuoraFunct,
                processScopeModel)
        {
            _httpHelper = httpHelper;
            ActivitySetting = ActivitySetting??processScopeModel.GetActivitySettingsAs<AutoReplyToNewMessageModel>();
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobprocessresult)
        {
            try
            {
                var scrapeMessageResponseHandler = quoraFunct.ScrapeMessagesId(JobProcess.DominatorAccountModel,"-1");
                jobprocessresult = FilterAndStartFinalProcessForMessage(jobprocessresult,scrapeMessageResponseHandler.LstMessageDetails, ActivitySetting);
                StartPagination(ref jobprocessresult, scrapeMessageResponseHandler, ActivitySetting);
            }
            catch (OperationCanceledException)
            {
                if (_browser.BrowserWindow != null) _browser.CloseBrowser();
                throw new OperationCanceledException();
            }
            catch (Exception)
            {
            }
        }

        private void StartPagination(ref JobProcessResult jobprocessresult,
            ScrapeMessageResponseHandler scrapeMessageResponseHandler, AutoReplyToNewMessageModel autoreplymodel)
        {
            var message = autoreplymodel.IsReplyToAllMessagesChecked ? "AutoReply To All Message" : autoreplymodel.IsReplyToMessagesThatContainSpecificWordChecked ? "Auto Reply To Message That Contain Specific Word" : autoreplymodel.IsReplyToPendingMessagesChecked ? "Auto Reply To Pending Message" : "Auto Reply To New Message";
            GlobusLogHelper.log.Info(Log.CustomMessage,JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,JobProcess.DominatorAccountModel.AccountBaseModel.UserName, ActivityType,$"Naviagting To Next Page For {message}");
            while (!jobprocessresult.IsProcessCompleted && scrapeMessageResponseHandler.HasMoreResult)
            {
                scrapeMessageResponseHandler =quoraFunct.ScrapeMessagesId(JobProcess.DominatorAccountModel, scrapeMessageResponseHandler.PaginationCount.ToString());
                if (scrapeMessageResponseHandler.LstMessageDetails.Count != 0)
                    jobprocessresult = FilterAndStartFinalProcessForMessage(jobprocessresult, scrapeMessageResponseHandler.LstMessageDetails, autoreplymodel);
                else if (!scrapeMessageResponseHandler.HasMoreResult && scrapeMessageResponseHandler.LstMessageDetails.Count == 0)
                    break;
                else
                    continue;
            }
            GlobusLogHelper.log.Info(Log.CustomMessage,JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,JobProcess.DominatorAccountModel.AccountBaseModel.UserName, ActivityType, $"No Unique Message Found For {message}");
        }

        private JobProcessResult FilterAndStartFinalProcessForMessage(JobProcessResult jobProcessResult,
            List<MessageDetails> messageDetails, AutoReplyToNewMessageModel autoreplymodel)
        {
            foreach (var message in messageDetails)
            {
                try
                {
                    if (message.LastMessage.StartsWith("You:")) continue;
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    var userinfo = quoraFunct.UserInfo(JobProcess.DominatorAccountModel,
                         message.UserProfileUrl.Contains("https")?message.UserProfileUrl: $"{QdConstants.HomePageUrl}/" + message.UserProfileUrl);
                    userinfo.Url = $"{QdConstants.HomePageUrl}/messages/thread/" + message.MessageId;
                    if (Blacklistuser.Contains(message.UserFullName)
                        || PrivateBlacklistedUser != null &&
                        PrivateBlacklistedUser.Contains(message.UserFullName))
                        continue;
                    if (autoreplymodel.IsReplyToMessagesThatContainSpecificWord﻿Checked)
                    {
                        if (!message.LastMessage.ToLower().Contains(autoreplymodel.SpecificWord.ToLower())) continue;
                    }

                    if (autoreplymodel.IsReplyToPendingMessages﻿﻿Checked && userinfo.FollowedBack == 1)
                        continue;

                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    FilterAndStartFinalProcessForEachMessage(out jobProcessResult, userinfo, userinfo);
                }
                catch (OperationCanceledException)
                {
                    if (_browser.BrowserWindow != null) _browser.CloseBrowser();
                    throw new OperationCanceledException();
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                if (jobProcessResult.IsProcessCompleted)
                    break;
            }

            return jobProcessResult;
        }

        private void FilterAndStartFinalProcessForEachMessage(out JobProcessResult jobProcessResult,
            QuoraUser quoraUser, UserInfoResponseHandler userInfo)
        {
            if (!UserFilterApply(userInfo))
                jobProcessResult = JobProcess.FinalProcess(new ScrapeResultNew
                {
                    ResultUser = quoraUser
                });

            else
                jobProcessResult = new JobProcessResult();
        }
    }
}