using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Enums.FdQuery;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDFactories;
using FaceDominatorCore.FDLibrary.DAL;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDModel.InviterModel;
using FaceDominatorCore.FDRequest;
using Newtonsoft.Json;
using System;
using AccountInteractedPosts = DominatorHouseCore.DatabaseHandler.FdTables.Accounts.InteractedPosts;
using CampaignInteractedPosts = DominatorHouseCore.DatabaseHandler.FdTables.Campaigns.InteractedPosts;


namespace FaceDominatorCore.FDLibrary.FdProcesses
{
    public class WatchPartyInviterProcss : FdJobProcessInteracted<AccountInteractedPosts>
    {

        public IFdRequestLibrary FdRequestLibrary { get; set; }

        public WatchPartyInviterProcss(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IFdQueryScraperFactory queryScraperFactory,
            IFdHttpHelper fdHttpHelper, IFdLoginProcess fdLogInProcess, IFdRequestLibrary fdRequestLibrary,
            IDbCampaignServiceScoped dbCampaignServiceScoped)
            : base(processScopeModel, accountServiceScoped, queryScraperFactory, fdHttpHelper, fdLogInProcess, dbCampaignServiceScoped)
        {
            FdRequestLibrary = fdRequestLibrary;
            processScopeModel.GetActivitySettingsAs<WatchPartyInviterModel>();
            AccountModel = DominatorAccountModel;
            CheckJobProcessLimitsReached();
        }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            JobProcessResult jobProcessResult = new JobProcessResult();

            FacebookUser objFacebookUser = (FacebookUser)scrapeResult.ResultUser;


            FacebookPostDetails objFacebookPostDetails = (FacebookPostDetails)scrapeResult.ResultEntity;

            GlobusLogHelper.log.Info(Log.StartedActivity, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                DominatorAccountModel.AccountBaseModel.UserName, ActivityType, objFacebookUser.UserId);

            try
            {

                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                FdErrorDetails objFdErrorDetails;


                if (string.IsNullOrEmpty(objFacebookPostDetails.PostUrl) && objFacebookPostDetails.Id.Contains(FdConstants.FbHomeUrl))
                    objFacebookPostDetails.PostUrl = objFacebookPostDetails.Id;

                if (string.IsNullOrEmpty(objFacebookPostDetails.Id))
                    objFacebookPostDetails.Id = objFacebookPostDetails.PostUrl;



                if (objFacebookPostDetails.EntityType == FbEntityTypes.Group)
                    objFdErrorDetails = FdRequestLibrary.InviteFriendOrPage(AccountModel, objFacebookUser.UserId,
                       ref objFacebookPostDetails, false);
                else
                    objFdErrorDetails = FdRequestLibrary.InviteFriendOrPage(AccountModel, objFacebookUser.UserId,
                       ref objFacebookPostDetails);



                if (objFdErrorDetails == null)
                {
                    GlobusLogHelper.log.Info(Log.ActivitySuccessful, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType, objFacebookUser.UserId);
                    IncrementCounters();
                    AddSendRequestDataToDatabase(scrapeResult);
                    jobProcessResult.IsProcessSuceessfull = true;
                }
                else
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType, $"Failed to invite user {objFacebookUser.ProfileUrl} with error: {objFdErrorDetails.Description}");
                    jobProcessResult.IsProcessSuceessfull = false;

                    if (objFdErrorDetails.Description.Contains("Watch Party Expired!"))
                    {
                        jobProcessResult.HasNoResult = true;
                        Stop();
                        JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                    }
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

        private void AddSendRequestDataToDatabase(ScrapeResultNew scrapeResult)
        {
            try
            {

                FacebookUser objFacebookUser = (FacebookUser)scrapeResult.ResultUser;

                FacebookPostDetails group = (FacebookPostDetails)scrapeResult.ResultEntity;

                var jobActivityConfigurationManager = InstanceProvider.GetInstance<IJobActivityConfigurationManager>();

                var modulesetting = jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];



                if (modulesetting == null)
                    return;

                if (modulesetting.IsTemplateMadeByCampaignMode)
                {

                    DbCampaignService.Add(new CampaignInteractedPosts
                    {
                        ActivityType = ActivityType.ToString(),
                        QueryType = group.QueryType,
                        QueryValue = group.QueryValue,
                        AccountEmail = DominatorAccountModel.AccountBaseModel.UserName,
                        PostDescription = JsonConvert.SerializeObject(group),
                        PostId = group.Id,
                        InteractionTimeStamp = DateTimeUtilities.GetEpochTime(),
                        InteractionDateTime = DateTime.Now,
                        WatchPartInvitedTo = objFacebookUser.UserId,
                        WatchPartInvitedToUserName = objFacebookUser.Familyname
                    });
                }

                DbAccountService.Add(new AccountInteractedPosts
                {
                    ActivityType = ActivityType.ToString(),
                    QueryType = group.QueryType,
                    QueryValue = group.QueryValue,
                    PostId = group.Id,
                    PostDescription = JsonConvert.SerializeObject(group),
                    InteractionTimeStamp = DateTimeUtilities.GetEpochTime(),
                    InteractionDateTime = DateTime.Now,
                    WatchPartInvitedTo = objFacebookUser.UserId,
                    WatchPartInvitedToUserName = objFacebookUser.Familyname

                });
            }
            catch (Exception ex)
            {
                ex.ErrorLog(ex.Message);
            }
        }
    }
}
