using System;
using System.Collections.Generic;
using System.Linq;
using CommonServiceLocator;
using ThreadUtils;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.LdTables.Account;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using LinkedDominatorCore.Factories;
using LinkedDominatorCore.LDLibrary.DAL;
using LinkedDominatorCore.LDModel;
using DominatorHouseCore.Utility;

namespace LinkedDominatorCore.LDLibrary.Processor.Group.NonQueryType
{
    public class GroupUnJoinerProcessor : BaseLinkedinGroupProcessor, IQueryProcessor
    {
        private readonly GroupUnJoinerModel _groupUnJoinerModel;

        public GroupUnJoinerProcessor(ILdJobProcess jobProcess, IDbAccountService dbAccountService,
            IDbCampaignService campaignService,
            ILdFunctionFactory ldFunctionFactory, IProcessScopeModel processScopeModel, IDelayService delayService) :
            base(jobProcess, dbAccountService, campaignService, ldFunctionFactory, delayService, processScopeModel)
        {
            _groupUnJoinerModel =processScopeModel.GetActivitySettingsAs<GroupUnJoinerModel>();
        }


        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            try
            {
                #region Initializations

                var lstGroupsFromSelectedSource = new List<LinkedinGroup>();
                var lstCustomGroups = new List<string>();
                List<UnjoinedGroups> listUnJoinedGroups = null;
                var Instance = InstanceProvider.GetInstance<ILdAccountUpdateFactory>();
                Instance.UpdateGroups(DominatorAccountModel, LdFunctions, DbAccountService)
                    .Wait(LdJobProcess.JobCancellationTokenSource.Token);

                #endregion

                try
                {
                    lstCustomGroups = _groupUnJoinerModel.UrlList.Count > 0
                        ? _groupUnJoinerModel.UrlList
                        : _groupUnJoinerModel.UrlInput.Split('\n').ToList();
                    listUnJoinedGroups = DbAccountService.GetUnJoinedGroups()?.ToList();
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                if (_groupUnJoinerModel.IsCheckedBySoftware)
                {
                    #region  Add LstJoinedGroupBySoftware To LstGroupsFromSelectedSource

                    try
                    {
                        List<LinkedinGroup> lstJoinedGroupBySoftware = null;

                        var lstJoinedGroup = DbAccountService.GetJoinedGroups().ToList();

                        if (listUnJoinedGroups != null)
                            lstJoinedGroup.RemoveAll(x => listUnJoinedGroups.Any(y => y.GroupUrl == x.GroupUrl));
                        if (lstJoinedGroup.Count > 0)
                            lstJoinedGroupBySoftware = GetGroups(lstJoinedGroup);
                        LdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                        if (lstJoinedGroupBySoftware != null && lstJoinedGroupBySoftware.Count > 0)
                            lstGroupsFromSelectedSource.AddRange(lstJoinedGroupBySoftware);
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                    #endregion
                }

                if (_groupUnJoinerModel.IsCheckedOutSideSoftware)
                {
                    #region Add LstJoinedOutSideSoftware To LstGroupsFromSelectedSource

                    try
                    {
                        List<LinkedinGroup> lstJoinedOutSideSoftware = null;

                        #region MyRegion

                        var lstJoinedGroup = DbAccountService.GetJoinedGroups().ToList();
                        var lstJoinedGroupBySoftware =
                            DbAccountService.GetInteractedGroups(ActivityTypeString).ToList();
                        if (listUnJoinedGroups != null)
                            lstJoinedGroup.RemoveAll(x => listUnJoinedGroups.Any(y => y.GroupUrl == x.GroupUrl));
                        lstJoinedGroup.RemoveAll(x => lstJoinedGroupBySoftware.Any(y => y.GroupUrl == x.GroupUrl));
                        if (lstJoinedGroup.Count > 0)
                            lstJoinedOutSideSoftware = GetGroups(lstJoinedGroup);
                        LdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                        #endregion

                        if (lstJoinedOutSideSoftware != null && lstJoinedOutSideSoftware.Count > 0)
                            lstGroupsFromSelectedSource.AddRange(lstJoinedOutSideSoftware);
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                    #endregion
                }

                if (_groupUnJoinerModel.IsCheckedCustomGroupList)
                {
                    #region Add LstGroupsFromCustomGroupList To LstGroupsFromSelectedSource

                    try
                    {
                        if (lstCustomGroups.Count != 0 || lstCustomGroups != null)
                            lstCustomGroups.RemoveAll(groupUrl =>
                                listUnJoinedGroups.Any(x => x.GroupUrl == groupUrl.Trim('/')));

                        var lstGroupsFromCustomGroupList = CustomJoinedGroupList(lstCustomGroups);
                        LdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                        if (lstGroupsFromCustomGroupList != null && lstGroupsFromCustomGroupList.Count > 0)
                            lstGroupsFromSelectedSource.AddRange(lstGroupsFromCustomGroupList);
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                    #endregion
                }

                if (lstGroupsFromSelectedSource.Count > 0)
                {
                    lstGroupsFromSelectedSource = lstGroupsFromSelectedSource.Distinct().ToList();
                    ProcessLinkedinGroupFromGroup(QueryInfo.NoQuery, ref jobProcessResult, lstGroupsFromSelectedSource);
                }
                else
                {
                    jobProcessResult.HasNoResult = true;
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
    }
}