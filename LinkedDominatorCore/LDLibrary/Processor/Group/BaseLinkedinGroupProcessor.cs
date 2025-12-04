using System;
using System.Collections.Generic;
using ThreadUtils;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.LdTables.Account;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using LinkedDominatorCore.Factories;
using LinkedDominatorCore.LDLibrary.DAL;
using LinkedDominatorCore.LDModel;
using LinkedDominatorCore.LDUtility;

namespace LinkedDominatorCore.LDLibrary.Processor.Group
{
    public abstract class BaseLinkedinGroupProcessor : BaseLinkedinProcessor
    {
        private readonly IDbCampaignService _campaignService;
        private readonly IDbAccountService _dbAccountService;


        protected BaseLinkedinGroupProcessor(ILdJobProcess ldJobProcess, IDbAccountService dbAccountService,
            IDbCampaignService campaignService, ILdFunctionFactory ldFunctionFactory, IDelayService delayService,
            IProcessScopeModel processScopeModel)
            : base(ldJobProcess, campaignService, ldFunctionFactory, delayService, processScopeModel)
        {
            _dbAccountService = dbAccountService;
            _campaignService = campaignService;
        }

        public void ProcessLinkedinGroupFromGroup(QueryInfo queryInfo, ref JobProcessResult jobProcessResult,
            List<LinkedinGroup> lstLinkedinGroup)
        {
            try
            {
                foreach (var group in lstLinkedinGroup)
                {
                    LdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    SendToPerformActivity(ref jobProcessResult, group, queryInfo);
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

        public void SendToPerformActivity(ref JobProcessResult jobProcessResult, LinkedinGroup linkedinGroup,
            QueryInfo queryInfo)
        {
            try
            {
                jobProcessResult = LdJobProcess.FinalProcess(new ScrapeResultNew
                {
                    ResultGroup = linkedinGroup,
                    QueryInfo = queryInfo
                });
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

        public List<LinkedinGroup> CustomJoinedGroupList(List<string> lstCustomGroups)
        {
            try
            {
                var lstCustomGroupWithDetails = new List<LinkedinGroup>();

                foreach (var groupUrl in lstCustomGroups)
                {
                    LdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    var groupId = string.Empty;

                    var objGroups = DbAccountService.GetSingleGroup(groupUrl?.Trim('/'));

                    if (objGroups != null)
                    {
                        groupId = Utils.GetBetween(objGroups.GroupUrl + "**", "groups/", "**");
                        var objLinkedinGroup = new LinkedinGroup(groupId);

                        objLinkedinGroup.GroupName = objGroups.GroupName;
                        objLinkedinGroup.TotalMembers = objGroups.TotalMembers;
                        objLinkedinGroup.CommunityType = objGroups.CommunityType;
                        objLinkedinGroup.MembershipStatus = objGroups.MembershipStatus;
                        if (!lstCustomGroupWithDetails.Contains(objLinkedinGroup))
                            lstCustomGroupWithDetails.Add(objLinkedinGroup);
                    }
                }

                return lstCustomGroupWithDetails;
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException("Operation Cancelled!");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public List<LinkedinGroup> GetGroups(List<Groups> lstGroup)
        {
            try
            {
                var lstGroupsBySoftware = new List<LinkedinGroup>();
                lstGroup.ForEach(x =>
                {
                    LdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    lstGroupsBySoftware.Add(new LinkedinGroup
                    {
                        GroupName = x.GroupName,
                        GroupUrl = x.GroupUrl,
                        TotalMembers = x.TotalMembers,
                        CommunityType = x.CommunityType,
                        MembershipStatus = x.MembershipStatus
                    });
                });
                return lstGroupsBySoftware;
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException("Operation Cancelled!");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }
    }
}