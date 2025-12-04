using CommonServiceLocator;
using ThreadUtils;
using DominatorHouseCore;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using EmbeddedBrowser;
using LinkedDominatorCore.Factories;
using LinkedDominatorCore.LDLibrary.DAL;
using LinkedDominatorCore.LDModel;
using LinkedDominatorCore.LDModel.GrowConnection;
using LinkedDominatorCore.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace LinkedDominatorCore.LDLibrary.Processor.Users.NonQueryType
{

    internal class SendGroupMemberInvitationProcessor : BaseLinkedinUserProcessor, IQueryProcessor
    {
        SendInvitationToGroupMemberModel SendInvitationToGroupMemberModel { get; }
        private BrowserWindow _browserWindow;

        public SendGroupMemberInvitationProcessor(ILdJobProcess jobProcess,
            IDbCampaignService campaignService, ILdFunctionFactory ldFunctionFactory, IDelayService delayService, IProcessScopeModel processScopeModel) :
            base(jobProcess, campaignService, ldFunctionFactory, delayService, processScopeModel)
        {
            SendInvitationToGroupMemberModel = processScopeModel.GetActivitySettingsAs<SendInvitationToGroupMemberModel>();
        }
        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            try
            {
                List<DominatorHouseCore.DatabaseHandler.LdTables.Account.InteractedUsers> listInteractedJobsFromAccountDb = null;
                listInteractedJobsFromAccountDb = DbAccountService.GetInteractedUsers(ActivityTypeString).ToList();
                var listLinkedInUsers = new List<LinkedinUser>();
                GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType, "Getting Connections");
                var Instance = InstanceProvider.GetInstance<ILdAccountUpdateFactory>();
                Instance.UpdateConnections(DominatorAccountModel, LdFunctions, DbAccountService).Wait(LdJobProcess.JobCancellationTokenSource.Token);
                var CustomGroupUrl = SendInvitationToGroupMemberModel.UrlList;
                if (SendInvitationToGroupMemberModel.IsCheckedLangKeyCustomAdminGroupList)
                {
                    queryInfo.QueryValue = SendInvitationToGroupMemberModel.UrlInput;
                    DbAccountService.GetConnections()?.ForEach(x => listLinkedInUsers.Add(ClassMapper.MappedConnectionToLinkedInUser(x)));

                }

                if (SendInvitationToGroupMemberModel.IsChkSkipBlackListedUser && (SendInvitationToGroupMemberModel.IsChkPrivateBlackList || SendInvitationToGroupMemberModel.IsChkGroupBlackList))
                    FilterBlacklistedUsers(listLinkedInUsers, SendInvitationToGroupMemberModel.IsChkPrivateBlackList, SendInvitationToGroupMemberModel.IsChkGroupBlackList);

                foreach (var GroupUrl in CustomGroupUrl)
                {
                    var GroupId = "";
                    queryInfo.QueryValue = GroupUrl;
                    if (queryInfo.QueryValue.Contains("admin"))
                    {
                        GroupId = Utilities.GetBetween(GroupUrl, "/groups/", "/admin");
                    }
                    else
                    {
                        GroupId = Utilities.GetBetween(GroupUrl, "/groups/", "/");
                    }
                    GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType, "Checking Group Details");
                    var isGroupMember = CheckGroupmember(GroupUrl); 
                    if (isGroupMember)
                    {
                        if (listInteractedJobsFromAccountDb != null)
                            listLinkedInUsers.RemoveAll(x => listInteractedJobsFromAccountDb.Any(y => y.QueryValue.Contains(GroupUrl) && y.UserProfileUrl == x.ProfileUrl));

                        ProcessLinkedinUsersFromUserList(queryInfo, ref jobProcessResult, listLinkedInUsers);
                        if (!jobProcessResult.IsProcessSuceessfull)
                            jobProcessResult.HasNoResult = true;
                    }
                    else
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType, "Sorry! You are not a member of this Group Or No option found to send invitation in this Group");
                        jobProcessResult.HasNoResult = true;
                    }
                    
                }
            }
            catch (OperationCanceledException)
            {
                CloseBrowserWhenNoMoreResults();
                throw new OperationCanceledException("Operation Cancelled!");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                CloseBrowserWhenNoMoreResults();
            }

        }
        /// <summary>
        /// runs on only browser mode
        /// </summary>
        /// <param name="GroupUrl"></param>
        /// <returns></returns>
        private bool CheckGroupmember(string GroupUrl)
        {
            if (_browserWindow == null)
                LDAccountsBrowserDetails.GetInstance().AccountBrowserCollections
                    .TryGetValue(DominatorAccountModel.UserName, out _browserWindow);

            if (_browserWindow != null)
            {
                _browserWindow.Browser.Load(GroupUrl);
                Thread.Sleep(10000);
            }
            var PageSource = _browserWindow.GetPageSource();
            var htmlDoc = new HtmlAgilityPack.HtmlDocument();
            htmlDoc.LoadHtml(PageSource);
            return !PageSource.Contains("Request to join");
        }
    }
}
