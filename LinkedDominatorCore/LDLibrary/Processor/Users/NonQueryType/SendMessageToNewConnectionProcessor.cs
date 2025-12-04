using System;
using System.Collections.Generic;
using System.Linq;
using CommonServiceLocator;
using ThreadUtils;
using DominatorHouseCore;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using LinkedDominatorCore.Factories;
using LinkedDominatorCore.LDLibrary.DAL;
using LinkedDominatorCore.LDModel;
using LinkedDominatorCore.LDModel.Messenger;
using Newtonsoft.Json;

namespace LinkedDominatorCore.LDLibrary.Processor.Users.NonQueryType
{
    internal class SendMessageToNewConnectionProcessor : BaseLinkedinUserProcessor, IQueryProcessor
    {
        public SendMessageToNewConnectionProcessor(ILdJobProcess jobProcess,
            IDbCampaignService campaignService, ILdFunctionFactory ldFunctionFactory, IDelayService delayService,
            IProcessScopeModel processScopeModel) :
            base(jobProcess, campaignService, ldFunctionFactory, delayService, processScopeModel)
        {
            var templatesFileManager = InstanceProvider.GetInstance<ITemplatesFileManager>();
            SendMessageToNewConnectionModel = JsonConvert.DeserializeObject<SendMessageToNewConnectionModel>(
                templatesFileManager.Get().FirstOrDefault(x => x.Id == LdJobProcess.TemplateId).ActivitySettings);
        }

        private SendMessageToNewConnectionModel SendMessageToNewConnectionModel { get; }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            #region Get New Connections within 10days  from DB if not selected days before

            try
            {
                var Instance = InstanceProvider.GetInstance<LdAccountUpdateFactory>();
                Instance.UpdateConnections(DominatorAccountModel, LdFunctions, DbAccountService)
                    .Wait(LdJobProcess.JobCancellationTokenSource.Token);
                GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType, "Getting New Connections");
                long timeStampFromUserInput = 0;
                if (SendMessageToNewConnectionModel.IsCheckedConnectedBefore)
                {
                    var days = SendMessageToNewConnectionModel.Days;
                    var hours = SendMessageToNewConnectionModel.Hours;
                    timeStampFromUserInput =
                        DateTime.Now.AddDays(-days).AddHours(-hours).GetCurrentEpochTimeMilliSeconds();
                }
                else
                {
                    timeStampFromUserInput = DateTime.Now.AddDays(-10).GetCurrentEpochTimeMilliSeconds();
                }

                var lstNewConnections = DbAccountService.GetNewConnections(timeStampFromUserInput).ToList();
                if (lstNewConnections.Count <= 0)
                {
                    jobProcessResult.HasNoResult = true;
                    return;
                }

                var listLinkedInUsers = new List<LinkedinUser>();
                MapConnectionToLinkedInUsers(listLinkedInUsers, lstNewConnections);

                LdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                var TotalUsers = listLinkedInUsers.Count;
                GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                        $"Successfully  Found {TotalUsers} Users.");
                RemoveOrSkipAlreadySendMessageToUsers(listLinkedInUsers);
                var SkippedUsers = TotalUsers - listLinkedInUsers.Count;
                if(SkippedUsers > 0)
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                        $"Successfully Filtered {SkippedUsers} Already Sent Message To Users.");
                }
                // filter connections having no profile picture
                if (SendMessageToNewConnectionModel.LDUserFilterModel.IsCheckedFilterProfileImageCheckbox)
                {
                    listLinkedInUsers.RemoveAll(x => x.ProfilePicUrl == null);
                    GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                        "successfully filtered connections having no profile picture");
                }

                if (SendMessageToNewConnectionModel.IsChkSkipBlackListedUser &&
                    (SendMessageToNewConnectionModel.IsChkPrivateBlackList ||
                     SendMessageToNewConnectionModel.IsChkGroupBlackList))
                {
                    FilterBlacklistedUsers(listLinkedInUsers, SendMessageToNewConnectionModel.IsChkPrivateBlackList,
                        SendMessageToNewConnectionModel.IsChkGroupBlackList);
                    var FilteredBlackListedUsers = SkippedUsers - listLinkedInUsers.Count;
                    if (FilteredBlackListedUsers > 0)
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                            $"Successfully Filtered {FilteredBlackListedUsers} BlackListed Users.");
                    }
                }
                if (listLinkedInUsers.Count > 0)
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                        "Successful to get " + listLinkedInUsers.Count + " new connections");
                    ProcessLinkedinUsersFromUserList(QueryInfo.NoQuery, ref jobProcessResult, listLinkedInUsers);
                    if (!jobProcessResult.IsProcessSuceessfull)
                        jobProcessResult.HasNoResult = true;
                }
                else
                {
                    jobProcessResult.HasNoResult = true;
                    GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                        "Sorry! no more new connections found");
                }
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException("Operation Cancelled!");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            #endregion
        }
    }
}