using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ThreadUtils;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.LdTables.Account;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using LinkedDominatorCore.Factories;
using LinkedDominatorCore.LDLibrary.DAL;
using LinkedDominatorCore.LDModel;

namespace LinkedDominatorCore.LDLibrary.Processor.Users
{
    public class Only1StConnectionProcessor : BaseLinkedinUserProcessor, IQueryProcessor
    {
        private readonly IProcessScopeModel _processScopeModel;

        public Only1StConnectionProcessor(ILdJobProcess jobProcess,
            IDbCampaignService campaignService, ILdFunctionFactory ldFunctionFactory,
            IProcessScopeModel processScopeModel, IDelayService delayService, IProcessScopeModel ProcessScopeModel) :
            base(jobProcess, campaignService, ldFunctionFactory, delayService, ProcessScopeModel)
        {
            _processScopeModel = processScopeModel;
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            try
            {
                var listInteractedUsersFromAccountDb = new List<InteractedUsers>();
                List<DominatorHouseCore.DatabaseHandler.LdTables.Campaign.InteractedUsers>
                    listInteractedUsersFromCampaignDb = null;

                if (string.IsNullOrEmpty(queryInfo.QueryValue))
                    queryInfo.QueryValue = "N/A";
                if (string.IsNullOrEmpty(LdJobProcess.CurrentCampaignId))
                    listInteractedUsersFromAccountDb = DbAccountService.GetInteractedUsers(ActivityTypeString).ToList();
                else
                    listInteractedUsersFromCampaignDb =
                        DbCampaignService.GetInteractedUsers(ActivityTypeString).ToList();

                var userScraperModel = _processScopeModel.GetActivitySettingsAs<UserScraperModel>();


                var connectionApi =
                    queryInfo.QueryValue!=null && queryInfo.QueryValue != "N/A" && queryInfo.QueryValue != "[Own]" && queryInfo.QueryValue != "[own]" && queryInfo.QueryValue!="[OWN]" && queryInfo.QueryValue != "OWN" && queryInfo.QueryValue != "Own"
                        ? $"https://www.linkedin.com/voyager/api/relationships/connections?keyword={queryInfo.QueryValue}"
                        : "https://www.linkedin.com/voyager/api/relationships/connections?sortType=RECENTLY_ADDED";


                var loopCount = 0;
                var start = 0;

                if (IsBrowser)
                {
                    var connections = new List<LinkedinUser>();
                    DbAccountService.GetConnections()
                        ?.ForEach(x => connections.Add(ClassMapper.MappedConnectionToLinkedInUser(x)));
                    if (connections.Count == 0)
                        GlobusLogHelper.log.Info(Log.CustomMessage,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                            "please update your connections by clicking 'Update Friendship' in Accounts Manager");
                    else
                        FilterAndProcessUsers(queryInfo, connections, listInteractedUsersFromAccountDb,
                            listInteractedUsersFromCampaignDb, userScraperModel, start);

                    jobProcessResult.HasNoResult = true;
                    return;
                }

                while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult &&
                       !jobProcessResult.IsProcessCompleted)
                {
                    loopCount = ++loopCount;
                    var actionUrl = $"{connectionApi}&count=40&start={start}";

                    LdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    var searchConnectionResponseHandler = LdFunctions.SearchForLinkedinConnections(actionUrl);
                    if (searchConnectionResponseHandler.Success)
                    {
                        if (searchConnectionResponseHandler.ConnectionsList.Count > 0)
                            start = FilterAndProcessUsers(queryInfo, searchConnectionResponseHandler.ConnectionsList,
                                listInteractedUsersFromAccountDb, listInteractedUsersFromCampaignDb, userScraperModel,
                                start);
                        else
                            jobProcessResult.HasNoResult = true;
                    }
                    else
                    {
                        jobProcessResult.HasNoResult = true;
                    }
                }

                jobProcessResult.HasNoResult = true;
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException("Operation Cancelled!");
            }
            catch (Exception ex)
            {
                jobProcessResult.HasNoResult = true;
                ex.DebugLog();
            }
        }

        private int FilterAndProcessUsers(QueryInfo queryInfo, List<LinkedinUser> connections,
            List<InteractedUsers> listInteractedUsersFromAccountDb,
            List<DominatorHouseCore.DatabaseHandler.LdTables.Campaign.InteractedUsers>
                listInteractedUsersFromCampaignDb, UserScraperModel userScraperModel, int start)
        {
            var jobProcessResult = new JobProcessResult();

            #region  Filter Already Interacted

            connections.RemoveAll(x => listInteractedUsersFromAccountDb.Any(y => y.UserProfileUrl == x.ProfileUrl));
            if (listInteractedUsersFromCampaignDb != null && listInteractedUsersFromCampaignDb.Count > 0)
                connections.RemoveAll(x =>
                    listInteractedUsersFromCampaignDb.Any(y =>
                        y.UserProfileUrl == x.ProfileUrl ||
                        !string.IsNullOrEmpty(x.ProfileId) && y.ProfileId == x.ProfileId));

            #endregion

            if (userScraperModel.LDUserFilterModel.IsCheckedFilterProfileImageCheckbox)
            {
                connections.RemoveAll(x => x.ProfilePicUrl == null || x.ProfilePicUrl == "N/A");
                GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                    "successfully filtered users having no profile picture");
            }

            if (userScraperModel.IsChkSkipBlackListedUser &&
                (userScraperModel.IsChkPrivateBlackList || userScraperModel.IsChkGroupBlackList))
                FilterBlacklistedUsers(connections, userScraperModel.IsChkPrivateBlackList,
                    userScraperModel.IsChkGroupBlackList);

            if (connections.Count > 0)
                ProcessLinkedinUsersFromUserList(queryInfo, ref jobProcessResult, connections);
            else
                GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                    "sorry no results found navigating to next page...");


            start += 40;
            Thread.Sleep(3000);
            return start;
        }
    }
}