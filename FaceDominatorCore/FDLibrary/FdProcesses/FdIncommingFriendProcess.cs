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
using Newtonsoft.Json;
using System;
using AccountInteractedUsres = DominatorHouseCore.DatabaseHandler.FdTables.Accounts.InteractedUsers;
using CampaignInteractedUsres = DominatorHouseCore.DatabaseHandler.FdTables.Campaigns.InteractedUsers;

namespace FaceDominatorCore.FDLibrary.FdProcesses
{

    public class FdIncommingFriendProcess : FdJobProcessInteracted<AccountInteractedUsres>
    {
        public IncommingFriendRequestModel IncommingFriendRequestModel { get; set; }

        public DominatorAccountModel Account { get; set; }

        public IFdRequestLibrary FdRequestLibrary { get; set; }

        public FdIncommingFriendProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IFdQueryScraperFactory queryScraperFactory
            , IFdHttpHelper fdHttpHelper, IFdLoginProcess fdLogInProcess, IFdRequestLibrary fdRequestLibrary,
            IDbCampaignServiceScoped dbCampaignServiceScoped)
            : base(processScopeModel, accountServiceScoped, queryScraperFactory, fdHttpHelper, fdLogInProcess, dbCampaignServiceScoped)
        {
            FdRequestLibrary = fdRequestLibrary;
            IncommingFriendRequestModel = processScopeModel.GetActivitySettingsAs<IncommingFriendRequestModel>();
            AccountModel = DominatorAccountModel;
            // TemplateId = template;
        }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            JobProcessResult jobProcessResult = new JobProcessResult();

            FacebookUser objFacebookUser = (FacebookUser)scrapeResult.ResultUser;

            GlobusLogHelper.log.Info(Log.StartedActivity, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                DominatorAccountModel.AccountBaseModel.UserName, ActivityType, objFacebookUser.UserId);

            try
            {

                var isSuccess = false;

                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                string className = string.Empty;

                className = scrapeResult.QueryInfo.QueryType.Contains("Accept")
                    ? FdConstants.AcceptRequestElement
                    : FdConstants.CancelRequestElement;

                if (AccountModel.IsRunProcessThroughBrowser)
                    isSuccess = FdLogInProcess._browserManager.AcceptCancelFriendRequest(AccountModel,
                        objFacebookUser, className, scrapeResult.QueryInfo.QueryType);
                else
                    isSuccess = scrapeResult.QueryInfo.QueryType.Contains("Accept")
                        ? FdRequestLibrary.AcceptFriendRequest(AccountModel, objFacebookUser.UserId)
                        : FdRequestLibrary.CancelIncomingRequest(AccountModel, objFacebookUser.UserId);



                if (isSuccess)
                {
                    var activity = scrapeResult.QueryInfo.QueryType.Contains("Accept") ? "Accept Request" : "Cancel Request";
                    GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                        $"Successful to {activity} to account {objFacebookUser.UserId}");
                    IncrementCounters();
                    AddSendRequestDataToDatabase(scrapeResult);
                    jobProcessResult.IsProcessSuceessfull = true;
                }
                else
                {
                    GlobusLogHelper.log.Info(Log.ActivityFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType, objFacebookUser.UserId, "");
                    jobProcessResult.IsProcessSuceessfull = false;
                }
                DelayBeforeNextActivity();
            }

            catch (OperationCanceledException)
            {
                FdLogInProcess._browserManager.CloseBrowser(AccountModel);
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return jobProcessResult;
        }


        public override void StartOtherConfiguration(ScrapeResultNew scrapeResult)
        {
            //base.ModuleSetting.


        }



        private void AddSendRequestDataToDatabase(ScrapeResultNew scrapeResult)
        {
            try
            {

                FacebookUser user = (FacebookUser)scrapeResult.ResultUser;
                var jobActivityConfigurationManager = InstanceProvider.GetInstance<IJobActivityConfigurationManager>();

                var modulesetting = jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];

                if (modulesetting == null)
                    return;

                DbAccountService.Add(new AccountInteractedUsres
                {
                    ActivityType = ActivityType.ToString(),
                    QueryType = scrapeResult.QueryInfo.QueryType,
                    QueryValue = string.Empty,
                    UserId = user.UserId,
                    Username = user.Familyname,
                    UserProfileUrl = user.ProfileUrl,
                    InteractionTimeStamp = DateTimeUtilities.GetEpochTime(),
                    InteractionDateTime = DateTime.Now,
                    DetailedUserInfo = JsonConvert.SerializeObject(user)

                });

                if (modulesetting.IsTemplateMadeByCampaignMode)
                {
                    DbCampaignService.Add(new CampaignInteractedUsres
                    {
                        ActivityType = ActivityType.ToString(),
                        QueryType = scrapeResult.QueryInfo.QueryType,
                        QueryValue = string.Empty,
                        AccountEmail = DominatorAccountModel.AccountBaseModel.UserName,
                        UserId = user.UserId,
                        Username = user.Familyname,
                        UserProfileUrl = user.ProfileUrl,
                        Gender = user.Gender,
                        University = user.University,
                        Workplace = user.WorkPlace,
                        CurrentCity = user.Currentcity,
                        HomeTown = user.Hometown,
                        BirthDate = user.DateOfBirth,
                        ContactNo = user.ContactNo,
                        ProfilePic = user.ProfilePicUrl,
                        InteractionTimeStamp = DateTimeUtilities.GetEpochTime(),
                        InteractionDateTime = DateTime.Now,
                        DetailedUserInfo = JsonConvert.SerializeObject(user)


                    });
                }
            }
            catch (Exception ex)
            {
                ex.ErrorLog(ex.Message);
            }
        }

    }
}
