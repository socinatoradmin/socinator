using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDFactories;
using FaceDominatorCore.FDLibrary.DAL;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDModel.GroupsModel;
using FaceDominatorCore.FDRequest;
using System;
using AccountInteractedGroups = DominatorHouseCore.DatabaseHandler.FdTables.Accounts.InteractedGroups;
using CampaignInteractedGroups = DominatorHouseCore.DatabaseHandler.FdTables.Campaigns.InteractedGroups;


namespace FaceDominatorCore.FDLibrary.FdProcesses
{

    public class GroupUnjoinerProecss : FdJobProcessInteracted<AccountInteractedGroups>
    {
        public GroupUnjoinerModelNew GroupUnjoinerModel { get; set; }

        public DominatorAccountModel Account { get; set; }

        private readonly IFdRequestLibrary _fdRequestLibrary;

        public GroupUnjoinerProecss(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped,
            IFdQueryScraperFactory queryScraperFactory,
            IFdHttpHelper fdHttpHelper, IFdLoginProcess fdLogInProcess, IFdRequestLibrary fdRequestLibrary,
            IDbCampaignServiceScoped dbCampaignServiceScoped)
            : base(processScopeModel, accountServiceScoped, queryScraperFactory, fdHttpHelper, fdLogInProcess, dbCampaignServiceScoped)
        {
            this._fdRequestLibrary = fdRequestLibrary;
            GroupUnjoinerModel = processScopeModel.GetActivitySettingsAs<GroupUnjoinerModelNew>();
            AccountModel = DominatorAccountModel;
            CheckJobProcessLimitsReached();
        }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)

        {
            JobProcessResult jobProcessResult = new JobProcessResult();

            GroupDetails objGroupDetails = (GroupDetails)scrapeResult.ResultGroup;

            GlobusLogHelper.log.Info(Log.StartedActivity, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                DominatorAccountModel.AccountBaseModel.UserName, ActivityType, objGroupDetails.GroupId);

            try
            {
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                var isSuccess = AccountModel.IsRunProcessThroughBrowser
                    ? FdLogInProcess._browserManager.UnjoinGroups(AccountModel, objGroupDetails)
                    : _fdRequestLibrary.UnjoinGroup(AccountModel, objGroupDetails.GroupId);

                if (isSuccess)
                {
                    GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                        DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType, objGroupDetails.GroupId);
                    IncrementCounters();
                    RemoveFromFriendsList(objGroupDetails.GroupId);
                    AddProfileScraperDataToDatabase(scrapeResult);
                    jobProcessResult.IsProcessSuceessfull = true;
                }
                else
                {
                    GlobusLogHelper.log.Info(Log.ActivityFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType, objGroupDetails.GroupId, "");
                    jobProcessResult.IsProcessSuceessfull = false;
                }
                DelayBeforeNextActivity();
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return jobProcessResult;
        }

        private void RemoveFromFriendsList(string groupId)
        {
            FdFunctions.FdFunctions objFunction = new FdFunctions.FdFunctions(AccountModel);
            objFunction.RemoveGroupAfterUnjoin(groupId);
            objFunction.UpdateGroupCountToAccountModelNew(AccountModel);
        }

        public override void StartOtherConfiguration(ScrapeResultNew scrapeResult)
        {
            try
            {

                base.StartOtherConfiguration(scrapeResult);


                var countTotalRequestedUsers = ObjDbAccountService.Count<AccountInteractedGroups>(x => x.ActivityType == ActivityType.ToString());


                GlobusLogHelper.log.Info(Log.OtherConfigurationStarted,
                    DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.UserName,
                    ActivityType);
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                #region Sttop Send Request and Start Withdraw
                if (GroupUnjoinerModel.IsChkEnableAutoGroupJoinerUnJoinerChecked)
                {

                    if (GroupUnjoinerModel.IsChkStopGroupUnjoinerToolWhenReachChecked)
                    {
                        if (GroupUnjoinerModel.IsChkStopGroupUnjoinerToolWhenReachChecked && (countTotalRequestedUsers >= GroupUnjoinerModel.StopFollowToolWhenReach.GetRandom()))
                        {
                            try
                            {
                                var dominatorScheduler = InstanceProvider.GetInstance<IDominatorScheduler>();
                                if (!dominatorScheduler.EnableDisableModules(ActivityType.GroupUnJoiner, ActivityType.GroupJoiner, DominatorAccountModel.AccountId))
                                {
                                    GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.UserName, ActivityType, "Group Unjoiner activity has met your Auto Enable configuration for Group Joiner, but you do not have Group Joiner configuration saved. Please save the Group Joiner configuration manually, to restart the Group Joiner/Unjoiner activity from this account");
                                }

                            }
                            catch (InvalidOperationException ex)
                            {
                                GlobusLogHelper.log.Info(Log.CustomMessage,
                                    DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    DominatorAccountModel.UserName, ActivityType,
                                    ex.Message.Contains("1001")
                                        ? "Group Unjoiner activity has met your Auto Enable configuration for Group Joiner, but you do not have Group Joiner configuration saved. Please save the Group Joiner configuration manually, to restart the Group Joiner/Unjoiner activity from this account" : "");
                            }
                            catch (Exception ex)
                            {
                                ex.DebugLog();
                            }
                        }
                    }


                }

                #endregion



            }
            catch (Exception ex)
            {
                ex.DebugLog();
                //   GlobusLogHelper.log.Error($"TwtDominator : [Account: {DominatorAccountModel.AccountBaseModel.UserName}] => Error : {ex.Message}. StackTrace => {ex.StackTrace}  (Module => {ActivityType.ToString()})");
            }

        }

        private void AddProfileScraperDataToDatabase(ScrapeResultNew scrapeResult)
        {
            try
            {

                GroupDetails group = (GroupDetails)scrapeResult.ResultGroup;
                var jobActivityConfigurationManager = InstanceProvider.GetInstance<IJobActivityConfigurationManager>();

                var modulesetting = jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];

                if (modulesetting == null)
                    return;

                if (modulesetting.IsTemplateMadeByCampaignMode)
                {
                    ObjDbCampaignService.Add(new CampaignInteractedGroups
                    {
                        ActivityType = ActivityType.ToString(),
                        QueryType = GroupUnjoinerModel.UnfriendOptionModel.IsAddedOutsideSoftware ? "Outside Software" : "Through Software",
                        AccountEmail = DominatorAccountModel.AccountBaseModel.UserName,
                        GroupUrl = group.GroupUrl,
                        GroupName = group.GroupName,
                        TotalMembers = group.GroupMemberCount,
                        MembershipStatus = "Not a Member",
                        InteractionTimeStamp = DateTimeUtilities.GetEpochTime(),
                        InteractionDateTime = DateTime.Now

                    });
                }

                ObjDbAccountService.Add(new AccountInteractedGroups
                {
                    ActivityType = ActivityType.ToString(),
                    QueryType = GroupUnjoinerModel.UnfriendOptionModel.IsAddedOutsideSoftware ? "Outside Software" : "Through Software",
                    GroupUrl = group.GroupUrl,
                    GroupName = group.GroupName,
                    TotalMembers = group.GroupMemberCount,
                    MembershipStatus = "Not a Member",
                    GroupType = group.GroupDescription,
                    InteractionTimeStamp = DateTimeUtilities.GetEpochTime(),
                    InteractionDateTime = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                ex.ErrorLog(ex.Message);
            }
        }
    }
}
