using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDFactories;
using FaceDominatorCore.FDLibrary.DAL;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDModel.ScraperModel;
using FaceDominatorCore.FDRequest;
using System;
using AccountInteractedGroups = DominatorHouseCore.DatabaseHandler.FdTables.Accounts.InteractedGroups;
using CampaignInteractedGroups = DominatorHouseCore.DatabaseHandler.FdTables.Campaigns.InteractedGroups;

namespace FaceDominatorCore.FDLibrary.FdProcesses
{

    public class FdGroupScraperProcess : FdJobProcessInteracted<AccountInteractedGroups>
    {
        public GroupScraperModel GroupScraperModel { get; set; }

        public DominatorAccountModel Account { get; set; }

        public FdGroupScraperProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IFdQueryScraperFactory queryScraperFactory,
            IFdHttpHelper fdHttpHelper, IFdLoginProcess fdLogInProcess, IDbCampaignServiceScoped dbCampaignServiceScoped)
            : base(processScopeModel, accountServiceScoped, queryScraperFactory, fdHttpHelper, fdLogInProcess, dbCampaignServiceScoped)
        {
            GroupScraperModel = processScopeModel.GetActivitySettingsAs<GroupScraperModel>();
            AccountModel = DominatorAccountModel;
            CheckJobProcessLimitsReached();
        }

        public override void StartOtherConfiguration(ScrapeResultNew scrapeResult)
        {

        }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            JobProcessResult jobProcessResult = new JobProcessResult();

            GroupDetails objGroupDetails = (GroupDetails)scrapeResult.ResultGroup;


            try
            {
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                GlobusLogHelper.log.Info(Log.ActivitySuccessful, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType, objGroupDetails.GroupId);
                IncrementCounters();
                AddProfileScraperDataToDatabase(scrapeResult);
                jobProcessResult.IsProcessSuceessfull = true;
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
                        TotalMembers = FdFunctions.FdFunctions.GetIntegerOnlyString(group.GroupMemberCount),
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
                    TotalMembers = FdFunctions.FdFunctions.GetIntegerOnlyString(group.GroupMemberCount),
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
