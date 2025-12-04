using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDFactories;
using FaceDominatorCore.FDLibrary.DAL;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDModel.FriendsModel;
using FaceDominatorCore.FDRequest;
using System;
using AccountInteractedUsres = DominatorHouseCore.DatabaseHandler.FdTables.Accounts.InteractedUsers;
using CampaignInteractedUsres = DominatorHouseCore.DatabaseHandler.FdTables.Campaigns.InteractedUsers;

namespace FaceDominatorCore.FDLibrary.FdProcesses
{

    public class CancelFriendRequestProcess : FdJobProcessInteracted<AccountInteractedUsres>
    {
        public CancelSentRequestModel SendRequestModel { get; set; }

        public IFdRequestLibrary FdRequestLibrary { get; set; }



        public CancelFriendRequestProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IFdQueryScraperFactory queryScraperFactory,
            IFdHttpHelper fdHttpHelper, IFdLoginProcess fdLogInProcess, IFdRequestLibrary fdRequestLibrary,
            IDbCampaignServiceScoped dbCampaignServiceScoped)
            : base(processScopeModel, accountServiceScoped, queryScraperFactory, fdHttpHelper, fdLogInProcess, dbCampaignServiceScoped)
        {
            FdRequestLibrary = fdRequestLibrary;
            SendRequestModel = processScopeModel.GetActivitySettingsAs<CancelSentRequestModel>();
            AccountModel = DominatorAccountModel;
            CheckJobProcessLimitsReached();
        }

        public override void StartOtherConfiguration(ScrapeResultNew scrapeResult)
        {


        }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            JobProcessResult jobProcessResult = new JobProcessResult();

            FacebookUser objFacebookUser = (FacebookUser)scrapeResult.ResultUser;


            try
            {
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                bool isSuccess = false;

                isSuccess = AccountModel.IsRunProcessThroughBrowser ?
                   FdLogInProcess._browserManager.CancelSentRequest(AccountModel, objFacebookUser) :
                   FdRequestLibrary.CancelSentRequest(AccountModel, objFacebookUser.UserId);

                if (isSuccess)
                {
                    GlobusLogHelper.log.Info(Log.ActivitySuccessful, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType, objFacebookUser.UserId);

                    IncrementCounters();

                    AddUnfriendDataToDatabase(scrapeResult);
                    jobProcessResult.IsProcessSuceessfull = true;
                }
                else
                {
                    GlobusLogHelper.log.Info(Log.ActivityFailed,
                        DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType, objFacebookUser.UserId, "");
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


        private void AddUnfriendDataToDatabase(ScrapeResultNew scrapeResult)
        {
            try
            {

                FacebookUser user = (FacebookUser)scrapeResult.ResultUser;

                var jobActivityConfigurationManager = InstanceProvider.GetInstance<IJobActivityConfigurationManager>();

                var demodulating = jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];

                if (demodulating == null)
                    return;

                if (demodulating.IsTemplateMadeByCampaignMode)
                {
                    DbCampaignService.Add(new CampaignInteractedUsres
                    {
                        ActivityType = ActivityType.ToString(),
                        QueryType = scrapeResult.QueryInfo.QueryType,
                        QueryValue = scrapeResult.QueryInfo.QueryValue,
                        AccountEmail = DominatorAccountModel.AccountBaseModel.UserName,
                        UserId = user.UserId,
                        Username = user.Username,
                        UserProfileUrl = $"{FdConstants.FbHomeUrl}{user.UserId}",
                        Gender = user.Gender,
                        University = user.University,
                        Workplace = user.WorkPlace,
                        CurrentCity = user.Currentcity,
                        HomeTown = user.Hometown,
                        BirthDate = user.DateOfBirth,
                        ContactNo = user.ContactNo,
                        ProfilePic = user.ProfilePicUrl,
                        InteractionTimeStamp = DateTimeUtilities.GetEpochTime(),
                        InteractionDateTime = DateTime.Now

                    });
                }

                DbAccountService.Add(new AccountInteractedUsres
                {
                    ActivityType = ActivityType.ToString(),
                    QueryType = scrapeResult.QueryInfo.QueryType,
                    QueryValue = scrapeResult.QueryInfo.QueryValue,
                    UserId = user.UserId,
                    Username = user.Username,
                    UserProfileUrl = $"{FdConstants.FbHomeUrl}{user.UserId}",
                    InteractionTimeStamp = DateTimeUtilities.GetEpochTime(),
                    InteractionDateTime = DateTime.Now
                });

                if (ModuleSetting.ManageBlackWhiteListModel.IsAddToBlacklistOnceRequestWithdrawn)
                {
                    BlackListWhitelistHandler.AddToBlackList(user.UserId, user.Username);
                }
            }
            catch (Exception ex)
            {
                ex.ErrorLog(ex.Message);
            }
        }
    }

}
