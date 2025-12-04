using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Request;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDFactories;
using FaceDominatorCore.FDLibrary.DAL;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDRequest;
using FaceDominatorCore.FDResponse.GroupsResponse;
using System;
using AccountInteractedGroups = DominatorHouseCore.DatabaseHandler.FdTables.Accounts.InteractedGroups;
using CampaignInteractedGroups = DominatorHouseCore.DatabaseHandler.FdTables.Campaigns.InteractedGroups;

namespace FaceDominatorCore.FDLibrary.FdProcesses
{
    public class MakeGroupAdminProcess : FdJobProcessInteracted<AccountInteractedGroups>
    {

        private readonly IFdRequestLibrary _fdRequestLibrary;

        public MakeGroupAdminProcess(IProcessScopeModel processScopeModel, IDbAccountServiceScoped accountServiceScoped,
            IFdQueryScraperFactory queryScraperFactory, IFdHttpHelper fdHttpHelper, IFdLoginProcess fdLogInProcess,
            IFdRequestLibrary fdRequestLibrary, IDbCampaignServiceScoped dbCampaignServiceScoped)
            : base(processScopeModel, accountServiceScoped, queryScraperFactory, fdHttpHelper, fdLogInProcess,
                dbCampaignServiceScoped)
        {
            AccountModel = DominatorAccountModel;
            _fdRequestLibrary = fdRequestLibrary;
        }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            JobProcessResult jobProcessResult = new JobProcessResult();

            FacebookUser facebookUser = (FacebookUser)scrapeResult.ResultUser;
            GroupDetails groupDetails = (GroupDetails)scrapeResult.ResultGroup;

            GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                $"Trying to Invite MakeAdmin user {facebookUser.ProfileUrl} for group {groupDetails.GroupUrl}");

            try
            {
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                MakeAdminResponseHandler responseHandler = new MakeAdminResponseHandler(new ResponseParameter());

                if (AccountModel.IsRunProcessThroughBrowser)
                    FdLogInProcess._browserManager.MakeGroupAdmin(AccountModel, groupDetails.GroupId, facebookUser);
                else
                    _fdRequestLibrary.MakeGroupAdmin(AccountModel, groupDetails.GroupId, facebookUser.UserId);

                if (responseHandler.Status)
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                        $"Successfull to Invite Make Admin user {facebookUser.ProfileUrl} for group {groupDetails.GroupUrl}");

                    IncrementCounters();
                    AddMakeGroupAdminToDatabase(scrapeResult);
                    jobProcessResult.IsProcessSuceessfull = true;
                }
                else
                {
                    if (!string.IsNullOrEmpty(responseHandler.ErrorValue))
                        GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                            responseHandler.ErrorValue);

                    GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                        $"Failed to Invite Make Admin user {facebookUser.ProfileUrl} for group {groupDetails.GroupId}");
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

        private void AddMakeGroupAdminToDatabase(ScrapeResultNew scrapeResult)
        {
            try
            {
                FacebookUser facebookUser = (FacebookUser)scrapeResult.ResultUser;

                GroupDetails groupDetails = (GroupDetails)scrapeResult.ResultGroup;

                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                var jobActivityConfigurationManager = InstanceProvider.GetInstance<IJobActivityConfigurationManager>();

                var moduleConfiguration = jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];

                if (moduleConfiguration != null && moduleConfiguration.IsTemplateMadeByCampaignMode)
                {
                    DbCampaignService.Add(new CampaignInteractedGroups
                    {
                        ActivityType = ActivityType.ToString(),
                        AccountEmail = DominatorAccountModel.AccountBaseModel.UserName,
                        UserId = facebookUser.UserId,
                        GroupUrl = FdConstants.FbHomeUrl + groupDetails.GroupId,
                        GroupName = groupDetails.GroupName,
                        InteractionTimeStamp = DateTimeUtilities.GetEpochTime(),
                        InteractionDateTime = DateTime.Now,
                    });
                }

                DbAccountService.Add(new AccountInteractedGroups
                {
                    ActivityType = ActivityType.ToString(),
                    UserId = facebookUser.UserId,
                    GroupUrl = FdConstants.FbHomeUrl + groupDetails.GroupId,
                    GroupName = groupDetails.GroupName,
                    InteractionTimeStamp = DateTimeUtilities.GetEpochTime(),
                    InteractionDateTime = DateTime.Now,
                });
            }
            catch (Exception ex)
            {
                ex.ErrorLog(ex.Message);
            }
        }
    }
}
