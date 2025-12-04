using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
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

    public class GroupJoinerProcess : FdJobProcessInteracted<AccountInteractedGroups>
    {
        public GroupJoinerModel GroupJoinerModel { get; set; }

        public DominatorAccountModel Account { get; set; }

        private readonly IFdRequestLibrary _fdRequestLibrary;

        public GroupJoinerProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IFdQueryScraperFactory queryScraperFactory,
            IFdHttpHelper fdHttpHelper, IFdLoginProcess fdLogInProcess, IFdRequestLibrary fdRequestLibrary, IDbCampaignServiceScoped dbCampaignServiceScoped)
            : base(processScopeModel, accountServiceScoped, queryScraperFactory, fdHttpHelper, fdLogInProcess, dbCampaignServiceScoped)
        {
            _fdRequestLibrary = fdRequestLibrary;
            GroupJoinerModel = processScopeModel.GetActivitySettingsAs<GroupJoinerModel>();
            AccountModel = DominatorAccountModel;
            base.CheckJobProcessLimitsReached();
        }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {

            JobProcessResult jobProcessResult = new JobProcessResult();

            GroupDetails objGroupDetails = (GroupDetails)scrapeResult.ResultGroup;

            var jobActivityConfigurationManager = InstanceProvider.GetInstance<IJobActivityConfigurationManager>();

            var modulesetting = jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];

            if (modulesetting == null)
                return jobProcessResult;


            if (modulesetting.IsTemplateMadeByCampaignMode && GroupJoinerModel.IsUniqueGroupsShouldBeJoinedFromEachAccount)
            {
                var fdCampaignInteractionDetails = InstanceProvider.GetInstance<ICampaignInteractionDetails>();
                try
                {
                    fdCampaignInteractionDetails.AddInteractedData(SocialNetworks, CampaignId, objGroupDetails.GroupUrl);
                }
                catch (Exception)
                {
                    jobProcessResult.IsProcessSuceessfull = false;
                    return jobProcessResult;
                }
            }


            try
            {
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                if (AccountModel.IsRunProcessThroughBrowser && scrapeResult.QueryInfo.QueryTypeEnum == "CustomGroupUrl")
                    FdLogInProcess._browserManager.LoadPageSource(AccountModel, objGroupDetails.GroupUrl);



                var isSuccess = AccountModel.IsRunProcessThroughBrowser ?
                   FdLogInProcess._browserManager.JoinGroups(AccountModel, objGroupDetails, GroupJoinerModel)
              : _fdRequestLibrary.GetGroupJoiningStatus(AccountModel, objGroupDetails.GroupUrl);

                if (isSuccess)
                {
                    GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                        DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType, objGroupDetails.GroupId);
                    IncrementCounters();
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
                if (ex.Message != "One or more errors occurred.")
                    ex.DebugLog();
            }

            return jobProcessResult;
        }

        public override void StartOtherConfiguration(ScrapeResultNew scrapeResult)
        {
            try
            {

                base.StartOtherConfiguration(scrapeResult);


                var countTotalRequestedUsers = ObjDbCampaignService.Count<AccountInteractedGroups>(x => x.ActivityType == ActivityType.ToString());


                GlobusLogHelper.log.Info(Log.OtherConfigurationStarted,
                    DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.UserName,
                    ActivityType);


                #region Sttop Send Request and Start Withdraw
                if (GroupJoinerModel.IsChkEnableAutoGroupJoinerUnJoinerChecked)
                {

                    if (GroupJoinerModel.IsChkStopGroupUnjoinerToolWhenReachChecked)
                    {
                        if (GroupJoinerModel.IsChkStopGroupUnjoinerToolWhenReachChecked && (countTotalRequestedUsers >= GroupJoinerModel.StopFollowToolWhenReach.GetRandom()))
                        {
                            try
                            {
                                var dominatorScheduler = InstanceProvider.GetInstance<IDominatorScheduler>();
                                if (!dominatorScheduler.EnableDisableModules(ActivityType.GroupJoiner, ActivityType.GroupUnJoiner, DominatorAccountModel.AccountId))
                                {
                                    GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.UserName, ActivityType, "Group Joiner activity has met your Auto Enable configuration for UnJoiner, but you do not have Unjoiner configuration saved. Please save the Unjoiner configuration manually, to restart the Group joiner/Unjoiner activity from this account");
                                }

                            }
                            catch (InvalidOperationException ex)
                            {
                                GlobusLogHelper.log.Info(Log.CustomMessage,
                                    DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    DominatorAccountModel.UserName, ActivityType,
                                    ex.Message.Contains("1001")
                                        ? "Group Joiner activity has met your Auto Enable configuration for UnJoiner, but you do not have Unjoiner configuration saved. Please save the Unjoiner configuration manually, to restart the Group joiner/Unjoiner activity from this account"
                                        : "");
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
                    DbCampaignService.Add(new CampaignInteractedGroups
                    {
                        ActivityType = ActivityType.ToString(),
                        QueryType = scrapeResult.QueryInfo.QueryType,
                        QueryValue = scrapeResult.QueryInfo.QueryValue,
                        AccountEmail = DominatorAccountModel.AccountBaseModel.UserName,
                        GroupUrl = group.GroupUrl,
                        GroupName = group.GroupName,
                        TotalMembers = group.GroupMemberCount,
                        MembershipStatus = group.GroupJoinStatus,
                        InteractionTimeStamp = DateTimeUtilities.GetEpochTime(),
                        InteractionDateTime = DateTime.Now

                    });
                }

                DbAccountService.Add(new AccountInteractedGroups
                {
                    ActivityType = ActivityType.ToString(),
                    QueryType = scrapeResult.QueryInfo.QueryType,
                    QueryValue = scrapeResult.QueryInfo.QueryValue,
                    GroupUrl = group.GroupUrl,
                    GroupName = group.GroupName,
                    TotalMembers = group.GroupMemberCount,
                    MembershipStatus = group.GroupJoinStatus,
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
