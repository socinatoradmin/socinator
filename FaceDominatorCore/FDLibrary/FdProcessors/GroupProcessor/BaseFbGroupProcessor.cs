using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
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
using FaceDominatorCore.FDModel.FilterModel;
using System;
using System.Collections.Generic;
using System.Threading;
using ThreadUtils;

namespace FaceDominatorCore.FDLibrary.FdProcessors.GroupProcessor
{
    public class BaseFbGroupProcessor : BaseFbProcessor
    {
        /*
                private readonly object _lockReachedMaxTweetActionPerUser = new object();
        */

        readonly FdJobProcess _objFdJobProcess;

        private readonly IAccountScopeFactory _accountScopeFactory;

        protected readonly IFdUpdateAccountProcess FdUpdateAccountProcess;
        private readonly IDelayService _delayService;

        protected BaseFbGroupProcessor(IFdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService,
            IDbCampaignServiceScoped campaignService, IFdRequestLibrary objFdRequestLibrary,
            IFdBrowserManager browserManager, IProcessScopeModel processScopeModel)
            : base(jobProcess, dbAccountService, campaignService, objFdRequestLibrary, browserManager, processScopeModel)

        {
            _accountScopeFactory = InstanceProvider.GetInstance<IAccountScopeFactory>();
            _objFdJobProcess = (FdJobProcess)jobProcess;
            FdUpdateAccountProcess = InstanceProvider.GetInstance<IFdUpdateAccountProcess>();
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {

        }

        protected bool CheckGroupUniqueNess(JobProcessResult jobProcessResult, GroupDetails groupDetails
            , ActivityType activityType)
        {

            if (_objFdJobProcess.ModuleSetting.GroupFilterModel.IsUniqueGroupsChecked)
            {
                try
                {
                    var fdCampaignInteractionDetails = InstanceProvider.GetInstance<ICampaignInteractionDetails>();

                    fdCampaignInteractionDetails.AddInteractedData(SocialNetworks.Facebook, JobProcess.CampaignId,
                        groupDetails.GroupUrl);
                }
                catch (Exception)
                {
                    return false;
                }
            }
            return true;
        }

        public void ProcessDataOfGroups(QueryInfo queryInfo, ref JobProcessResult jobProcessResult,
                    List<GroupDetails> objLstGroupDetails)
        {
            var filteredGoups = false;
            foreach (var group in objLstGroupDetails)
            {
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                try
                {
                    if (AlreadyInteractedGroups(group))
                    { filteredGoups = true; continue; }

                    if (!CheckGroupUniqueNess(jobProcessResult, group, _ActivityType))
                        continue;

                    if (group.GroupJoinStatus != null && (group.GroupJoinStatus.Contains("Member") || group.GroupJoinStatus.Contains("Request Sent")))
                    {
                        if (_ActivityType == ActivityType.GroupJoiner)
                            continue;

                        Thread.Sleep(500);
                    }
                    if (AccountModel.IsRunProcessThroughBrowser &&
                        FdFunctions.FdFunctions.IsIntegerOnly(group.GroupMemberCount) && group.GroupMemberCount.EndsWith("000"))
                        group.GroupMemberCount = Browsermanager.GetGroupCountDetails(AccountModel, group);
                    if (AccountModel.IsRunProcessThroughBrowser && (string.IsNullOrEmpty(group.GroupMemberCount)
                        || !FdFunctions.FdFunctions.IsIntegerOnly(group.GroupMemberCount)))
                    {

                        group.GroupMemberCount = Browsermanager.GetGroupCountDetails(AccountModel, group);

                        Browsermanager.GetIndividualGroupDetails(AccountModel, group);

                    }
                    else if (string.IsNullOrEmpty(group.GroupMemberCount) || !FdFunctions.FdFunctions.IsIntegerOnly(group.GroupMemberCount))
                        group.GroupMemberCount = ObjFdRequestLibrary.GetGroupMemberCount(AccountModel, group.GroupId);

                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                    FilterData(queryInfo, ref jobProcessResult, group);
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

            }
            if (filteredGoups)
                GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork,
                            AccountModel.AccountBaseModel.UserName, _ActivityType,
                            "Succefully Skipped the Filtered Groups");
        }

        private void FilterData(QueryInfo queryInfo, ref JobProcessResult jobProcessResult, GroupDetails objGroupDetails)
        {
            try
            {
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                var logUrl = objGroupDetails.GroupUrl.Contains(FdConstants.FbHomeUrl) ? string.Format("LangKeyGroupDosentMatchWithFilter".FromResourceDictionary(), $"{objGroupDetails.GroupUrl}") :
                                        string.Format("LangKeyGroupDosentMatchWithFilter".FromResourceDictionary(), $"{FdConstants.FbHomeUrl}{objGroupDetails.GroupUrl}");
                if (JobProcess.ModuleSetting.GroupFilterModel.IsGroupsBetweenChecked)
                {

                    try
                    {
                        if (!JobProcess.ModuleSetting.GroupFilterModel.MemberCountBetween.InRange(Int32.Parse(
                                    FdFunctions.FdFunctions.GetIntegerOnlyString(objGroupDetails.GroupMemberCount))))
                        {
                            _objFdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                            GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork,
                                AccountModel.AccountBaseModel.UserName, _ActivityType, logUrl);
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                }

                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                if (JobProcess.ModuleSetting.GroupFilterModel.IsSkipJoinedGroups)
                {
                    if (!objGroupDetails.GroupJoinStatus.Contains("Not a member"))
                    {
                        _objFdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                        GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork,
                            AccountModel.AccountBaseModel.UserName, _ActivityType, logUrl);
                        return;
                    }
                }

                if (JobProcess.ModuleSetting.GroupFilterModel.IsGroupsJoinedByMyFriends)
                {
                    if (!Browsermanager.IsFriendsInGroup(objGroupDetails))
                    {
                        _objFdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                        GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork,
                            AccountModel.AccountBaseModel.UserName, _ActivityType, logUrl);
                        return;
                    }
                }

                if (JobProcess.ModuleSetting.GroupFilterModel.IsPrivateGroup)
                {
                    if (!objGroupDetails.GroupType.Contains("Private"))
                    {
                        _objFdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                        GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork,
                            AccountModel.AccountBaseModel.UserName, _ActivityType, logUrl);
                        return;
                    }
                }

                if (JobProcess.ModuleSetting.GroupFilterModel.IsPublicGroup)
                {
                    if (!objGroupDetails.GroupType.Contains("Public"))
                    {
                        _objFdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                        GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork,
                            AccountModel.AccountBaseModel.UserName, _ActivityType, logUrl);
                        return;
                    }
                }

            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

            SendToPerformActivity(ref jobProcessResult, objGroupDetails, queryInfo);

        }

        protected void ApplyGroupFilter(ref GroupMemberShip objGroupMemberShip, ref GroupType objGroupType,
                    QueryInfo queryInfo)
        {

            if (queryInfo.IsCustomFilterSelected)
            {
                FdGroupFilterModel queryGroupFilterModel = Newtonsoft.Json.JsonConvert.DeserializeObject<FdGroupFilterModel>(queryInfo.CustomFilters);

                if (queryGroupFilterModel.IsGroupTypeChecked)
                    objGroupType = queryGroupFilterModel.IsPrivateGroup ? GroupType.Closed : GroupType.Public;

                if (queryGroupFilterModel.IsGroupsJoinedByMyFriends)
                    objGroupMemberShip = GroupMemberShip.FriendsGroups;
            }
            else
            {
                if (JobProcess.ModuleSetting.GroupFilterModel.IsGroupTypeChecked)
                    objGroupType = JobProcess.ModuleSetting.GroupFilterModel.IsPrivateGroup ? GroupType.Closed : GroupType.Public;

                if (JobProcess.ModuleSetting.GroupFilterModel.IsGroupsJoinedByMyFriends)
                    objGroupMemberShip = GroupMemberShip.FriendsGroups;
            }
        }


        // ReSharper disable once RedundantAssignment
        public void SendToPerformActivity(ref JobProcessResult jobProcessResult, GroupDetails objGroupDetails,
                    QueryInfo queryInfo)
        {
            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

            jobProcessResult = JobProcess.FinalProcess(new ScrapeResultNew()
            {
                ResultGroup = objGroupDetails,
                QueryInfo = queryInfo
            });
        }

        protected bool IsGroupFilterActive()
            => JobProcess.ModuleSetting.GroupFilterModel.IsGroupTypeChecked || JobProcess.ModuleSetting.GroupFilterModel.IsGroupsJoinedByMyFriends;

        protected void AppplyPostFilterAndStartFinalProcess(QueryInfo queryInfo, ref JobProcessResult jobProcessResult, List<GroupDetails> lstGroupDetails)
        {
            try
            {
                List<GroupDetails> lstFilteredGroups = _ActivityType == ActivityType.GroupUnJoiner
                    ? IsDateFilterApplied() ? ApplyDateFilter(lstGroupDetails) : lstGroupDetails
                    : lstGroupDetails;

                foreach (var groupDetails in lstFilteredGroups)
                {
                    try
                    {
                        bool isIgnore = false;
                        if (_ActivityType == ActivityType.GroupJoiner)
                            if (CheckDuplicateDataForGroups(groupDetails))
                                continue;

                        if (groupDetails.GroupJoinStatus != null && groupDetails.GroupJoinStatus.Contains("Member"))
                            groupDetails.GroupMemberCount = AccountModel.IsRunProcessThroughBrowser
                                ? Browsermanager.GetGroupCountDetails(AccountModel, groupDetails, true)
                                : ObjFdRequestLibrary.GetGroupMemberCount(AccountModel, groupDetails.GroupId);
                        if (string.IsNullOrEmpty(groupDetails.GroupType))
                            Browsermanager.GetIndividualGroupDetails(AccountModel, groupDetails);
                        try
                        {
                            if (IsPostGroupFilterActive())
                            {
                                if (ApplyPostGroupFilter(groupDetails))
                                {
                                    var logUrl = groupDetails.GroupUrl.Contains(FdConstants.FbHomeUrl) ? string.Format("LangKeyGroupDosentMatchWithFilter".FromResourceDictionary(), $"{groupDetails.GroupUrl}") :
                                        string.Format("LangKeyGroupDosentMatchWithFilter".FromResourceDictionary(), $"{FdConstants.FbHomeUrl}{groupDetails.GroupUrl}");
                                    GlobusLogHelper.log.Info(Log.CustomMessage,
                                        AccountModel.AccountBaseModel.AccountNetwork,
                                        AccountModel.AccountBaseModel.UserName, _ActivityType, logUrl);

                                    continue;
                                }

                            }
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }

                        if (!isIgnore)
                            SendToPerformActivity(ref jobProcessResult, groupDetails, queryInfo);

                        if (JobProcess.IsStopped())
                            break;

                    }
                    catch (OperationCanceledException)
                    {
                        throw new OperationCanceledException();
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                }

            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private bool IsDateFilterApplied() =>
            JobProcess.ModuleSetting.UnfriendOptionModel.IsFilterApplied ? true : false;


        private List<GroupDetails> ApplyDateFilter(List<GroupDetails> lstGroupDetails)
        {
            List<GroupDetails> lstFilteredGroups = new List<GroupDetails>();

            foreach (var groupDetails in lstGroupDetails)
            {
                int hours = (DateTime.Now - groupDetails.JoinDate).Hours;
                int days = (DateTime.Now - groupDetails.JoinDate).Days;

                if (days > JobProcess.ModuleSetting.UnfriendOptionModel.DaysBefore && hours > JobProcess.ModuleSetting.UnfriendOptionModel.HoursBefore)
                    lstFilteredGroups.Add(groupDetails);
            }

            GlobusLogHelper.log.Info(Log.FilterApplied, AccountModel.AccountBaseModel.AccountNetwork,
                AccountModel.AccountBaseModel.UserName, _ActivityType, lstFilteredGroups.Count);

            return lstFilteredGroups;

        }

        private bool ApplyPostGroupFilter(GroupDetails groupDetails)
        {
            try
            {
                if (JobProcess.ModuleSetting.GroupFilterModel.IsGroupsBetweenChecked)
                {

                    try
                    {
                        if (!JobProcess.ModuleSetting.GroupFilterModel.MemberCountBetween.InRange(Int32.Parse(FdFunctions.FdFunctions.GetIntegerOnlyString(groupDetails.GroupMemberCount))))
                            return true;
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                }
                if (JobProcess.ModuleSetting.GroupFilterModel.IsSkipJoinedGroups)
                {
                    if (!groupDetails.GroupJoinStatus.Contains("Not a member"))
                        return true;
                }

                if (JobProcess.ModuleSetting.GroupFilterModel.IsPrivateGroup)
                {
                    if (string.IsNullOrEmpty(groupDetails.GroupType) || groupDetails.GroupType.Contains("Public"))
                        return true;
                }

                if (JobProcess.ModuleSetting.GroupFilterModel.IsPublicGroup)
                {
                    if (string.IsNullOrEmpty(groupDetails.GroupType) || groupDetails.GroupType.Contains("Private"))
                        return true;
                }

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return false;
        }

        public bool CheckDuplicateDataForGroups(GroupDetails groupDetails)
        {
            return !groupDetails.GroupJoinStatus.Contains("Not a member")
                ? true
                : _dbAccountService.DoesInteractedGroupsExist(groupDetails.GroupId, _ActivityType);
        }

        private bool IsPostGroupFilterActive()
            => JobProcess.ModuleSetting.GroupFilterModel.IsGroupsBetweenChecked ||
                   JobProcess.ModuleSetting.GroupFilterModel.IsSkipJoinedGroups || JobProcess.ModuleSetting.GroupFilterModel.IsPrivateGroup
                    || JobProcess.ModuleSetting.GroupFilterModel.IsPublicGroup;
    }
}

