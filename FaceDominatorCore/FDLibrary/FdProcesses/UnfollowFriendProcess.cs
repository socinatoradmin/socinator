using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
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
using System.Globalization;
using AccountInteractedUsers = DominatorHouseCore.DatabaseHandler.FdTables.Accounts.InteractedUsers;
using CampaignInteractedUers = DominatorHouseCore.DatabaseHandler.FdTables.Campaigns.InteractedUsers;

namespace FaceDominatorCore.FDLibrary.FdProcesses
{

    public class UnfollowFriendProcess : FdJobProcessInteracted<AccountInteractedUsers>
    {
        public UnfollowFriendModel UnfollowModel { get; set; }

        public IFdRequestLibrary FdRequestLibrary { get; set; }

        private readonly IAccountScopeFactory _accountScopeFactory;

        public UnfollowFriendProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IFdQueryScraperFactory queryScraperFactory,
            IFdHttpHelper fdHttpHelper, IFdLoginProcess fdLogInProcess, IFdRequestLibrary fdRequestLibrary,
            IDbCampaignServiceScoped dbCampaignServiceScoped)
            : base(processScopeModel, accountServiceScoped, queryScraperFactory, fdHttpHelper, fdLogInProcess, dbCampaignServiceScoped)
        {
            _accountScopeFactory = InstanceProvider.GetInstance<IAccountScopeFactory>();
            FdRequestLibrary = fdRequestLibrary;
            UnfollowModel = processScopeModel.GetActivitySettingsAs<UnfollowFriendModel>();
            AccountModel = DominatorAccountModel;
            CheckJobProcessLimitsReached();
        }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            JobProcessResult jobProcessResult = new JobProcessResult();

            FacebookUser objFacebookUser = (FacebookUser)scrapeResult.ResultUser;

            try
            {
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                if (AccountModel.IsRunProcessThroughBrowser)
                {
                    string url = string.IsNullOrEmpty(objFacebookUser.ScrapedProfileUrl)
                        ? objFacebookUser.ProfileUrl
                        : objFacebookUser.ScrapedProfileUrl;
                    url = string.IsNullOrEmpty(url) ? $"{FdConstants.FbHomeUrl}{objFacebookUser.UserId}" : url;

                    FdLogInProcess._browserManager.LoadPageSource(AccountModel, $"{url}");
                }

                bool isSuccess = AccountModel.IsRunProcessThroughBrowser
                    ? FdLogInProcess._browserManager.UnfollowFriend(AccountModel)
                    : FdRequestLibrary.Unfriend(AccountModel, ref objFacebookUser).IsCancelledRequest;

                if (isSuccess)
                {
                    GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                        DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType, objFacebookUser.UserId);
                    IncrementCounters();
                    AddUnfollowDataToDatabase(scrapeResult);
                    //RemoveFromFriendsList(objFacebookUser.UserId);
                    jobProcessResult.IsProcessSuceessfull = true;
                }
                else
                {
                    GlobusLogHelper.log.Info(Log.ActivityFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                        $"{objFacebookUser.UserId}");
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

        private void RemoveFromFriendsList(string friendId)
        {
            FdFunctions.FdFunctions objFunction = new FdFunctions.FdFunctions(AccountModel);
            objFunction.RemoveFriendAfterUnfriend(friendId);
            objFunction.UpdateFriendCountToAccountModelNew(AccountModel);
            objFunction.RemoveFriendFromBinAfterUnfriend(friendId, AccountModel);
        }

        private void AddUnfollowDataToDatabase(ScrapeResultNew scrapeResult)
        {
            try
            {

                FacebookUser user = (FacebookUser)scrapeResult.ResultUser;

                if (string.IsNullOrEmpty(user.InteractionDate))
                    user.InteractionDate = DateTime.Now.ToString(CultureInfo.InvariantCulture);

                var interactionDate = DateTime.Now;
                DateTime.TryParse(user.InteractionDate, out interactionDate);

                if ((DateTime.Now - interactionDate).Days > 20000)
                    interactionDate = DateTime.Now;

                var interactionTimeStamp = interactionDate.ConvertToEpoch();

                var jobActivityConfigurationManager = InstanceProvider.GetInstance<IJobActivityConfigurationManager>();

                var modulesetting = jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];

                if (modulesetting == null)
                    return;

                if (modulesetting.IsTemplateMadeByCampaignMode)
                {
                    if (string.IsNullOrEmpty(user.Familyname) &&
                        !string.IsNullOrEmpty(user.FullName))
                    {
                        user.Familyname = user.FullName;
                    }

                    DbCampaignService.Add(new CampaignInteractedUers
                    {
                        ActivityType = ActivityType.ToString(),
                        QueryType = scrapeResult.QueryInfo.QueryType,
                        QueryValue = scrapeResult.QueryInfo.QueryValue,
                        AccountEmail = DominatorAccountModel.AccountBaseModel.UserName,
                        UserId = user.UserId,
                        Username = user.Familyname,
                        UserProfileUrl = string.IsNullOrEmpty(user.ProfileUrl) ? $"{FdConstants.FbHomeUrl}{user.UserId}" : user.ProfileUrl,
                        InteractionTimeStamp = interactionTimeStamp,
                        InteractionDateTime = DateTime.Now,
                        Gender = user.Gender,
                        University = user.University,
                        Workplace = user.WorkPlace,
                        CurrentCity = user.Currentcity,
                        HomeTown = user.Hometown,
                        BirthDate = user.DateOfBirth,
                        ContactNo = user.ContactNo,
                        ProfilePic = user.ProfilePicUrl
                    });
                }

                DbAccountService.Add(new AccountInteractedUsers
                {


                    ActivityType = ActivityType.ToString(),
                    QueryType = scrapeResult.QueryInfo.QueryType,
                    QueryValue = scrapeResult.QueryInfo.QueryValue,
                    UserId = user.UserId,
                    Username = user.Familyname,
                    UserProfileUrl = string.IsNullOrEmpty(user.ProfileUrl) ? $"{FdConstants.FbHomeUrl}{user.UserId}" : user.ProfileUrl,
                    InteractionTimeStamp = interactionTimeStamp,
                    InteractionDateTime = DateTime.Now
                });

                if (ModuleSetting.ManageBlackWhiteListModel.IsAddToBlackListOnceUnfriend)
                {
                    BlackListWhitelistHandler.AddToBlackList(user.UserId, user.UserId);
                }
            }
            catch (AggregateException)
            {

            }
            catch (Exception ex)
            {
                ex.ErrorLog(ex.Message);
            }
        }

    }
}
