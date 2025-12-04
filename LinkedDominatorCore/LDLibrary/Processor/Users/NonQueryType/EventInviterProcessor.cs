using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using LinkedDominatorCore.Factories;
using LinkedDominatorCore.LDLibrary.DAL;
using LinkedDominatorCore.LDModel;
using LinkedDominatorCore.LDModel.GrowConnection;
using LinkedDominatorCore.LDUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using ThreadUtils;

namespace LinkedDominatorCore.LDLibrary.Processor.Users.NonQueryType
{

    public class EventInviterProcessor: BaseLinkedinUserProcessor, IQueryProcessor
    {
        InviteMemberToFollowPageModel InviteMemberToFollowPageModel { get; }
        public EventInviterProcessor(ILdJobProcess jobProcess,
            IDbCampaignService campaignService, ILdFunctionFactory ldFunctionFactory, IDelayService delayService, IProcessScopeModel processScopeModel) :
            base(jobProcess, campaignService, ldFunctionFactory, delayService, processScopeModel)
        {
            InviteMemberToFollowPageModel = processScopeModel.GetActivitySettingsAs<InviteMemberToFollowPageModel>();
        }
        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            try
            {
                var listInteractedJobsFromAccountDb = DbAccountService.GetInteractedUsers(ActivityTypeString).ToList();
                var listLinkedInUsers = new List<LinkedinUser>();
                GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType, "Getting Connections");
                var Instance = InstanceProvider.GetInstance<ILdAccountUpdateFactory>();
                Instance.UpdateConnections(DominatorAccountModel, LdFunctions, DbAccountService).Wait(LdJobProcess.JobCancellationTokenSource.Token);
                var CustomPageUrl = InviteMemberToFollowPageModel.UrlList;
                if (InviteMemberToFollowPageModel.IsCheckedLangKeyCustomAdminPageList)
                    DbAccountService.GetConnections()?.ForEach(x => listLinkedInUsers.Add(ClassMapper.MappedConnectionToLinkedInUser(x)));
                foreach (var PageUrl in CustomPageUrl)
                {
                    if (listInteractedJobsFromAccountDb != null)
                        listLinkedInUsers.RemoveAll(x => listInteractedJobsFromAccountDb.Any(y => y.QueryValue.Contains(PageUrl) && (y.UserProfileUrl == x.ProfileUrl || x.FullName == y.UserFullName)));
                    queryInfo.QueryValue = PageUrl;
                    queryInfo.CustomFilters = Utils.GenerateTrackingId();
                    ProcessLinkedinUsersFromUserList(queryInfo, ref jobProcessResult, listLinkedInUsers);
                    if (!jobProcessResult.IsProcessSuceessfull)
                        jobProcessResult.HasNoResult = true;
                    LdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                }
            }
            catch(Exception ex) { ex.DebugLog(); }
        }
    }
}
