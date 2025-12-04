using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.FdTables.Accounts;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDEnums;
using FaceDominatorCore.FDLibrary.DAL;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDLibrary.FdFunctions.FdBrowserManager;
using FaceDominatorCore.FDLibrary.FdProcesses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaceDominatorCore.FDLibrary.FdProcessors.GroupProcessor
{
    public class GroupUnjoinerProcessor : BaseFbGroupProcessor
    {
        private readonly CancellationToken _token;
        public GroupUnjoinerProcessor(IFdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService,
            IDbCampaignServiceScoped campaignService, IFdRequestLibrary objFdRequestLibrary,
            IFdBrowserManager browserManager, IProcessScopeModel processScopeModel)
            : base(jobProcess, dbAccountService, campaignService, objFdRequestLibrary, browserManager, processScopeModel)
        {

        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            try
            {

                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                List<string> queryList = new List<string>();

                if (JobProcess.ModuleSetting.UnfriendOptionModel.IsAddedThroughSoftware
                    && JobProcess.ModuleSetting.UnfriendOptionModel.IsAddedOutsideSoftware)
                    queryList.Add("Both");

                if (queryList.Count == 0 && (JobProcess.ModuleSetting.UnfriendOptionModel.IsAddedThroughSoftware
                    || JobProcess.ModuleSetting.UnfriendOptionModel.IsAddedOutsideSoftware))
                    queryList.Add(JobProcess.ModuleSetting.UnfriendOptionModel.IsAddedThroughSoftware ? "Through Software" : "Outside Software");

                FilterAndStartFinalProcessForUnjoinGroups(ref jobProcessResult, queryList);
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException("Requested Cancelled !");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        /// <summary>
        /// UnjoinGroups
        /// </summary>
        /// <param name="jobProcessResult"></param>
        /// <param name="query"></param>
        /// <param name="perOptionCount"></param>
        private void FilterAndStartFinalProcessForUnjoinGroups(ref JobProcessResult jobProcessResult, List<string> queryList)
        {
            try
            {
                var lstFilteredGroupIds = new List<GroupDetails>();
                GroupType objGroupType = GroupType.Any;
                GroupMemberShip objGroupMemberShip = GroupMemberShip.AnyGroup;

                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                if (IsGroupFilterActive())
                    ApplyGroupFilter(ref objGroupMemberShip, ref objGroupType, new QueryInfo());

                objGroupMemberShip = GroupMemberShip.MyGroups;
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                var query = queryList.Contains("Both") ? "Both" :
                    queryList.Contains("Through Software") ? "Through Software" : "Outside Software";

                lstFilteredGroupIds = FilterBySourceTypeWithQueryList(new QueryInfo() { QueryType = query }).Result;

                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                if (lstFilteredGroupIds.Count != 0)
                {
                    GlobusLogHelper.log.Info(Log.FoundXResults, AccountModel.AccountBaseModel.AccountNetwork,
                        AccountModel.AccountBaseModel.UserName, lstFilteredGroupIds.Count, query, "", _ActivityType);
                    AppplyPostFilterAndStartFinalProcess(new QueryInfo(), ref jobProcessResult, lstFilteredGroupIds);
                    jobProcessResult.HasNoResult = true;
                    jobProcessResult.maxId = null;

                }
                else
                {
                    _token.ThrowIfCancellationRequested();
                    jobProcessResult.HasNoResult = true;
                    jobProcessResult.maxId = null;
                }
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException("Requested Cancelled !");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

        }

        public async Task<List<GroupDetails>> FilterBySourceTypeWithQueryList(QueryInfo queryInfo)
        {
            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            var lastUpate = DateTimeUtilities.EpochToDateTimeUtc(AccountModel.LastUpdateTime);

            if (lastUpate.AddDays(2) < DateTime.Now)
            {
                if (AccountModel.IsRunProcessThroughBrowser)
                    await FdUpdateAccountProcess.UpdateGroupsFromBrowser(AccountModel, JobProcess.JobCancellationTokenSource.Token, Browsermanager);
                else
                    await FdUpdateAccountProcess.UpdateGroupsNew(AccountModel, CancellationToken.None);
            }


            var interesctedlstGroup = _dbAccountService.GetInteractedGroups(ActivityType.GroupJoiner);
            var joinedGroups = _dbAccountService.Get<OwnGroups>().ToList();

            var lstFilteredGroupIds = new List<GroupDetails>();

            if (queryInfo.QueryType.Contains("Both"))
            {
                joinedGroups.ForEach(x =>
                {
                    lstFilteredGroupIds.Add(new GroupDetails()
                    {
                        GroupId = x.GroupId,
                        GroupUrl = x.GroupUrl,
                        GroupName = x.GroupName,
                        GroupType = x.GroupType,
                        GroupJoinStatus = "Member"
                    });
                });
            }
            if (queryInfo.QueryType.Contains("Through Software"))
            {
                joinedGroups.ForEach(x =>
                {
                    if (interesctedlstGroup.FirstOrDefault(y => y.GroupUrl == x.GroupUrl) != null
                    || interesctedlstGroup.FirstOrDefault(y => x.GroupUrl.Contains(y.GroupUrl)) != null
                    || interesctedlstGroup.FirstOrDefault(y => y.GroupUrl.Contains(x.GroupId)) != null)
                        lstFilteredGroupIds.Add(new GroupDetails()
                        {
                            GroupId = x.GroupId,
                            GroupUrl = x.GroupUrl,
                            GroupName = x.GroupName,
                            GroupType = x.GroupType,
                            GroupJoinStatus = "Member"

                        });
                });
            }
            if (queryInfo.QueryType.Contains("Outside Software"))
            {
                joinedGroups.ForEach(x =>
                {
                    if (interesctedlstGroup.FirstOrDefault(y => y.GroupUrl == x.GroupUrl) == null)
                        lstFilteredGroupIds.Add(new GroupDetails()
                        {
                            GroupId = x.GroupId,
                            GroupUrl = x.GroupUrl,
                            GroupName = x.GroupName,
                            GroupType = x.GroupType,
                            GroupJoinStatus = "Member"
                        });
                });
            }

            return lstFilteredGroupIds;
        }
    }
}
