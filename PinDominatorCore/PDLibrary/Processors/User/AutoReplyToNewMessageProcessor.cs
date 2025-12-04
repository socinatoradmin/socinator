using ThreadUtils;
using DominatorHouseCore;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using Newtonsoft.Json;
using PinDominatorCore.PDLibrary.DAL;
using PinDominatorCore.PDModel;
using PinDominatorCore.Response;
using System.Collections.Generic;
using System.Linq;

namespace PinDominatorCore.PDLibrary.Processors.User
{
    public class AutoReplyToNewMessageProcessor : BasePinterestUserProcessor
    {
        private FindMessageResponseHandler _findMessage;
        public AutoReplyToNewMessageProcessor(IPdJobProcess jobProcess, IDbGlobalService globalService,
            IDbCampaignService campaignService, IPinFunction objPinFunct, IDelayService delayService) :
            base(jobProcess, globalService, campaignService, objPinFunct, delayService)
        {
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            GlobusLogHelper.log.Info(Log.CustomMessage,
                JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                JobProcess.DominatorAccountModel.UserName, JobProcess.ActivityType,
                "LangKeySearchingForNewMessagesToReply".FromResourceDictionary());
            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            AutoReplyToNewMessageModel autoReplySetting = JsonConvert.DeserializeObject<AutoReplyToNewMessageModel>(TemplateModel.ActivitySettings);

            MessageInvitationsResponseHandler messageInvitationsResponse = PinFunction.GetMessageInvitations(JobProcess.DominatorAccountModel);

            try
            {
                var messageInvites = messageInvitationsResponse.UsersList.Take(5);
                foreach (var invite in messageInvites)
                {
                    PinFunction.AcceptMessageInvitation(invite, JobProcess.DominatorAccountModel);
                }
            }
            catch (System.Exception ex)
            {
                ex.DebugLog();
            }

            bool isScroll = false;
            while (jobProcessResult.HasNoResult == false)
            {

                if (JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser)
                {
                    _findMessage = BrowserManager.SearchForMessagesToReply(JobProcess.DominatorAccountModel,
                          JobProcess.JobCancellationTokenSource, isScroll);
                    isScroll = true;
                }
                else
                    _findMessage = PinFunction.FindNewMessage(JobProcess.DominatorAccountModel, jobProcessResult.maxId);
                if (_findMessage == null || !_findMessage.Success
                                         || _findMessage.ListUsersAndText.Count == 0)
                {
                    GlobusLogHelper.log.Info(Log.NoMoreDataToPerform,
                        JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        JobProcess.DominatorAccountModel.UserName, ActivityType);
                    jobProcessResult.HasNoResult = true;
                    jobProcessResult.maxId = null;
                }
                else
                {
                    var lstUserName = _findMessage.ListUsersAndText.Select(x => x.Key.Value).ToList();
                    var lstUserAndMessages = new Dictionary<string, string>();
                    _findMessage.ListUsersAndText.ForEach(x =>
                    {
                        if (!lstUserAndMessages.Keys.Contains(x.Key.Value))
                            lstUserAndMessages.Add(x.Key.Value, x.Value);
                    });
                    lstUserName.RemoveAll(x => x.Equals(JobProcess.DominatorAccountModel.AccountBaseModel.ProfileId));
                    lstUserAndMessages.Remove(JobProcess.DominatorAccountModel.AccountBaseModel.ProfileId);
                    queryInfo = new QueryInfo { QueryType = "Auto Reply To New Message" };

                    var containsWord = false;
                    if (autoReplySetting.IsReplyToPendingMessagesChecked)
                    {
                        queryInfo.QueryValue =
                            "Reply only to new pending request (Replies to the first message request from the people you are not following)";
                        if (autoReplySetting.IsCheckedReplyToMessagesThatContainsSpecificWord)
                            lstUserAndMessages.ForEach(x =>
                            {
                                containsWord = false;
                                autoReplySetting.LstMessagesContainsSpecificWords.ForEach(y =>
                                {
                                    if (x.Value.Contains(y))
                                        containsWord = true;
                                });
                                if (!containsWord)
                                {
                                    // ReSharper disable once AccessToModifiedClosure
                                    lstUserName.Remove(x.Key);
                                }
                            });
                    }
                    else if (autoReplySetting.IsReplyToAllMessagesChecked)
                    {
                        queryInfo.QueryValue = "Reply To All Messages";
                    }

                    if (autoReplySetting.IsReplyToAllMessagesChecked &&
                        autoReplySetting.IsCheckedReplyToMessagesThatContainsSpecificWord)
                        if (autoReplySetting.IsCheckedReplyToMessagesThatContainsSpecificWord)
                            lstUserAndMessages.ForEach(x =>
                            {
                                containsWord = false;
                                autoReplySetting.LstMessagesContainsSpecificWords.ForEach(y =>
                                {
                                    if (x.Value.Contains(y))
                                        containsWord = true;
                                });
                                if (!containsWord)
                                {
                                    // ReSharper disable once AccessToModifiedClosure
                                    lstUserName.Remove(x.Key);
                                }
                            });
                    var lastTotalUserCount = lstUserName.Count;
                    lstUserName = FilterBlackListUser(TemplateModel, new List<string>(lstUserName));
                  if(lastTotalUserCount-lstUserName.Count > 0)
                    {
                        var message = autoReplySetting.IsChkGroupBlackList && autoReplySetting.IsChkPrivateBlackList ?
                            "Skipped Group BlackListed And Private BlackListed Users" :
                            autoReplySetting.IsChkPrivateBlackList ? "Skipped Private BlackListed Users" :
                            autoReplySetting.IsChkGroupBlackList ? "Skipped Group BlackListed Users" :
                            "Skipped Users";
                        GlobusLogHelper.log.Info(Log.CustomMessage,JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,JobProcess.DominatorAccountModel.AccountBaseModel.UserName, ActivityType,message);
                    }
                    if(lstUserName.Count > 0)
                    {
                        GlobusLogHelper.log.Info(Log.FoundXResults, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        JobProcess.DominatorAccountModel.AccountBaseModel.UserName, lstUserName.Count, queryInfo.QueryType, "", ActivityType);
                    }
                    if (_findMessage.SenderAndReceiver.Count > 0)
                        StartProcessForListOfUsers(queryInfo, ref jobProcessResult, lstUserName);
                    else
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage,
                            JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            JobProcess.DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                            "LangKeyProcessCompletedNoUsersFoundToReply".FromResourceDictionary());
                        jobProcessResult.IsProcessCompleted = true;
                    }

                    jobProcessResult.maxId = _findMessage.BookMark;
                    if (_findMessage.HasMoreResults == false)
                        jobProcessResult.HasNoResult = true;
                }
            }
        }
    }
}