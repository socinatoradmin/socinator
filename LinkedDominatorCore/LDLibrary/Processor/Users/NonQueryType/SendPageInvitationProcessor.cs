using CommonServiceLocator;
using ThreadUtils;
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

namespace LinkedDominatorCore.LDLibrary.Processor.Users.NonQueryType
{
    internal class SendPageInvitationProcessor : BaseLinkedinUserProcessor, IQueryProcessor
    {
        InviteMemberToFollowPageModel InviteMemberToFollowPageModel { get; }

        public SendPageInvitationProcessor(ILdJobProcess jobProcess, 
            IDbCampaignService campaignService, ILdFunctionFactory ldFunctionFactory, IDelayService delayService, IProcessScopeModel processScopeModel) :
            base(jobProcess, campaignService, ldFunctionFactory, delayService, processScopeModel)
        {
            InviteMemberToFollowPageModel = processScopeModel.GetActivitySettingsAs<InviteMemberToFollowPageModel>();
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
                var CustomPageUrl = InviteMemberToFollowPageModel.UrlList;
                if (InviteMemberToFollowPageModel.IsCheckedLangKeyCustomAdminPageList)
                {
                    queryInfo.QueryValue = InviteMemberToFollowPageModel.UrlInput;
                    DbAccountService.GetConnections()?.ForEach(x => listLinkedInUsers.Add(ClassMapper.MappedConnectionToLinkedInUser(x)));
                   
                }

                if (InviteMemberToFollowPageModel.IsChkSkipBlackListedUser && (InviteMemberToFollowPageModel.IsChkPrivateBlackList || InviteMemberToFollowPageModel.IsChkGroupBlackList))
                    FilterBlacklistedUsers(listLinkedInUsers, InviteMemberToFollowPageModel.IsChkPrivateBlackList, InviteMemberToFollowPageModel.IsChkGroupBlackList);

                foreach (var PageUrl in CustomPageUrl)
                {
                    var PageId = "";
                    queryInfo.QueryValue = PageUrl;
                    PageId = PageUrl != null ?PageUrl.Contains("admin")?Utils.GetBetween(PageUrl, "/company/", "/admin"):Utils.GetBetween(PageUrl, "/company/", "/"):"";
                    var failedCount = 0;
                    long.TryParse(PageId, out long pageId);
                    //var Details =IsBrowser? LdFunctions.GetHtmlFromUrlNormalMobileRequest(queryInfo.QueryValue) : LdFunctions.GetInnerHttpHelper().GetRequest(pageId>0?LdConstants.GetPageDetailsById(PageId):LdConstants.GetCompanyDetailsAPI(PageId)).Response;
                    var PageDetailsAPI = pageId > 0 ? LdConstants.GetPageDetailsById(PageId) : LdConstants.GetCompanyDetailsAPI(PageId);
                    var Details =LdFunctions.GetInnerHttpHelper().GetRequest(PageDetailsAPI).Response;
                    while (failedCount++ <= 2 && string.IsNullOrEmpty(Details))
                        Details = LdFunctions.GetInnerHttpHelper().GetRequest(PageDetailsAPI).Response;
                    bool CanInviteUser =GetPagedetailandCheckForAdmin(Details);
                    GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType, "Checking Page Details");
                    if (CanInviteUser)
                    {
                        if (listInteractedJobsFromAccountDb != null)
                            listLinkedInUsers.RemoveAll(x => listInteractedJobsFromAccountDb.Any(y => y.QueryValue.Contains(PageUrl) && y.UserProfileUrl== x.ProfileUrl));

                        ProcessLinkedinUsersFromUserList(queryInfo, ref jobProcessResult, listLinkedInUsers);
                        if (!jobProcessResult.IsProcessSuceessfull)
                            jobProcessResult.HasNoResult = true;
                    }
                    else
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType, "Sorry! You are not a Admin of this page Or No option found to send invitation in this Page");
                        jobProcessResult.HasNoResult = true;
                    }

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

        }

        private static bool GetPagedetailandCheckForAdmin(string Details)
        {            
            var jsonHandler = Utility.JsonJArrayHandler.GetInstance;
            Details = Details != null ?Details.Contains("\"organizationDashCompaniesByUniversalName\"") ?jsonHandler.GetJTokenValue(jsonHandler.ParseJsonToJObject(Details), "data", "organizationDashCompaniesByUniversalName", "elements",0) :Details: Details;
            var jsonObject = jsonHandler.ParseJsonToJObject(Details);
            var CanInviteUser = jsonHandler.GetJTokenValue(jsonObject, "viewerPermissions", "canEditEvents") == "True" || jsonHandler.GetJTokenValue(jsonObject, "viewerPermissions", "canMembersInviteToFollow") =="True" || jsonHandler.GetJTokenValue(jsonObject, "viewerPermissions", "canEmployeesInviteToFollow") == "True" || jsonHandler.GetJTokenValue(jsonObject, "viewerPermissions", "canInviteMemberToFollow") == "True";
            return CanInviteUser;
        }
    }
}
